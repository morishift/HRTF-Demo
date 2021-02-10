#if false

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Test
{
    public class Main : MonoBehaviour
    {
        [SerializeField]
        DebugButton debugButton;
        [SerializeField]
        ScheduledAudioSource[] audioSources;
        [SerializeField]
        Text debugText;
        [SerializeField]
        PositionCircle positionCircle;
        [SerializeField]
        Text fpsText;

        WaveAudioClip waveAudioClip;

        public class LRIR
        {
            public WaveAudioClip waveAudioClipIRL;
            public WaveAudioClip waveAudioClipIRR;
        }
        Dictionary<int, LRIR> lrirDictionary = new Dictionary<int, LRIR>();
        int currentAudioSourceIndex;
        Coroutine playCoroutine;

        OverlapAdd overlapAddL = new OverlapAdd(Constant.BlockSize, Constant.BlockSamples, Constant.ImpulseResponseSamples);
        OverlapAdd overlapAddR = new OverlapAdd(Constant.BlockSize, Constant.BlockSamples, Constant.ImpulseResponseSamples);
        float[] waveInputBuffer = new float[Constant.BlockSamples];
        float[] waveOutputBuffer = new float[Constant.BlockSamples * 2];

        void Start()
        {
            Application.targetFrameRate = 60;
            Debug.Log($"SampleTime:{(float)Constant.BlockSamples / Constant.Frequency:0.000}");

            waveAudioClip = WaveAudioClip.CreateWavAudioClip("Bytes/242_dr_bpm140_4-4_4_pop_mono.wav");

            debugButton.AddButton("ドラム", () =>
            {
                waveAudioClip = WaveAudioClip.CreateWavAudioClip("Bytes/242_dr_bpm140_4-4_4_pop_mono.wav");
            });

            positionCircle.onChangedAngle += OnChangedAngle;

            for (int degree = 0; degree < 360; degree += 5)
            {
                lrirDictionary[degree] = new LRIR();
                lrirDictionary[degree].waveAudioClipIRL = WaveAudioClip.CreateWavAudioClip($"Bytes/elev0/L0e{degree:000}a.wav");
                lrirDictionary[degree].waveAudioClipIRR = WaveAudioClip.CreateWavAudioClip($"Bytes/elev0/R0e{degree:000}a.wav");
            }
        }

        /// <summary>
        /// Streaming形式でサウンド作成
        /// </summary>
        private AudioClip CreateAudioClip(int offset, int size)
        {
            Debug.Log($"channels:{waveAudioClip.channels} frequency:{waveAudioClip.frequency} samples:{waveAudioClip.samples}");

            waveAudioClip.GetData(waveInputBuffer, offset, size);
            overlapAddL.Convolution(waveInputBuffer);
            overlapAddR.Convolution(waveInputBuffer);
            float[] output_l = overlapAddL.GetConvolution();
            float[] output_r = overlapAddR.GetConvolution();
            for (int i = 0, j = 0; j < size; i += 2, ++j)
            {
                waveOutputBuffer[i] = output_l[j];
                waveOutputBuffer[i + 1] = output_r[j];
            }
            var clip = AudioClip.Create("Sound", size, 2, waveAudioClip.frequency, false);
            clip.SetData(waveOutputBuffer, 0);
            return clip;
        }

        /// <summary>
        /// 角度が変化した
        /// </summary>
        private void OnChangedAngle()
        {
            if (playCoroutine == null)
            {
                playCoroutine = StartCoroutine(PlayCoroutine());
            }


            Debug.Log($"angle:{positionCircle.GetAngle()}");
            int degree = positionCircle.GetAngle();
            if (positionCircle.IsSelected())
            {
                DebugText($"{positionCircle.GetAngle()}°");
                overlapAddL.SetImpulseResponse(lrirDictionary[degree].waveAudioClipIRL.waveData);
                overlapAddR.SetImpulseResponse(lrirDictionary[degree].waveAudioClipIRR.waveData);
            }
            else
            {
                DebugText($"通常再生");
                overlapAddL.SetIdentifyImpulseResponse();
                overlapAddR.SetIdentifyImpulseResponse();
            }

            ResetPlayingAudioSource();
            NextAudioSource();
            ///currentAudioSource.Stop();

            double play_time = AudioSettings.dspTime + 0.1f;
            currentAudioSource.SetScheduledEndTime(play_time);
            NextAudioSource();
            int offset = (int)((prevAudioSource.endScheduledTime - playStartTime) * Constant.Frequency);
            currentAudioSource.PlayScheduled(CreateAudioClip(offset, Constant.BlockSamples), prevAudioSource.endScheduledTime);
            NextAudioSource();
        }

        /// <summary>
        /// 再生
        /// </summary>
        private IEnumerator PlayCoroutine()
        {
            playStartTime = AudioSettings.dspTime;
            currentAudioSource.PlayScheduled(CreateAudioClip(0, Constant.BlockSamples), playStartTime);
            // Debug.Log($"dsp:{playStartTime:0.00}");
            NextAudioSource();
            while (true)
            {
                // AudioSourceの再生終了待ち
                while (currentAudioSource.isPlaying)
                {
                    yield return 0;
                }
                // スケジューリング
                int offset = (int)((prevAudioSource.endScheduledTime - playStartTime) * Constant.Frequency);
                currentAudioSource.PlayScheduled(CreateAudioClip(offset, Constant.BlockSamples), prevAudioSource.endScheduledTime);
                // Debug.Log($"play:{currentAudioSource.playScheduledTime:0.00} end:{currentAudioSource.endScheduledTime:0.00}");
                NextAudioSource();
                Resources.UnloadUnusedAssets();
            }
        }

        /// <summary>
        /// 現在の空きScheduledAudioSource
        /// </summary>
        private ScheduledAudioSource currentAudioSource
        {
            get
            {
                return audioSources[currentAudioSourceIndex];
            }
        }

        /// <summary>
        /// ひとつ前のScheduledAudioSource
        /// </summary>
        private ScheduledAudioSource prevAudioSource
        {
            get
            {
                return audioSources[(currentAudioSourceIndex + 1) % 2];
            }
        }

        /// <summary>
        /// 再生開始時刻
        /// </summary>
        public double playStartTime
        {
            get;
            private set;
        }

        /// <summary>
        /// 次のAudioSourceへ遷移
        /// </summary>
        private void NextAudioSource()
        {
            currentAudioSourceIndex = (currentAudioSourceIndex + 1) % audioSources.Length;
        }

        /// <summary>
        /// 今音を再生しているAudioSourceをcurrentAudioSourceに変更する
        /// </summary>
        private void ResetPlayingAudioSource()
        {
            double t = AudioSettings.dspTime;
            for (int i = 0; i < audioSources.Length; ++i)
            {
                if (audioSources[i].playScheduledTime <= t && t <= audioSources[i].endScheduledTime)
                {
                    currentAudioSourceIndex = i;
                    return;
                }
            }
        }

        /// <summary>
        /// ログ出力
        /// </summary>
        private void DebugText(string str)
        {
            debugText.text = str;
        }

        void Update()
        {
            fpsText.text = $"{1 / Time.deltaTime:0.00}";
        }

    }
}

#endif
