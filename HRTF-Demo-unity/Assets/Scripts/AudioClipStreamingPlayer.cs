using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Test
{
    public class AudioClipStreamingPlayer : MonoBehaviour
    {
        public class Buffer
        {
            public Buffer(Constant c)
            {
                left = new float[c.blockSamples];
                right = new float[c.blockSamples];
            }
            public float[] left;
            public float[] right;
        }

        public Action<double, int, Constant, Buffer> onGetBuffer;

        [SerializeField]
        ScheduledAudioSource[] audioSources;
        Constant c;
        float[] audioClipData;
        int currentAudioSourceIndex;
        Coroutine playCoroutine;

        public void Initialize(Constant _c)
        {
            Stop();
            c = _c;
            audioClipData = new float[c.audioClipChannelSampleSize * 2];
        }

        /// <summary>
        /// AudioClipを疑似的にStreaming再生のような形で再生させる
        /// </summary>
        /// <param name="dspstart">再生開始時刻</param>
        public void Play(double dspstart)
        {
            Stop();
            playCoroutine = StartCoroutine(PlayCoroutine(dspstart));
        }

        /// <summary>
        /// 停止
        /// </summary>
        public void Stop()
        {
            if (playCoroutine != null)
            {
                StopCoroutine(playCoroutine);
                playCoroutine = null;
            }
        }

        /// <summary>
        /// 再生コルーチン
        /// </summary>
        private IEnumerator PlayCoroutine(double dspstart)
        {
            int sampleoffset = 0;
            while (true)
            {
                Debug.Log($"dspstart:{dspstart:0.00}");
                // 過去に生成したAudioClipオブジェクトを解放
                Resources.UnloadUnusedAssets();                
                while (dspstart - c.audioClipCreateOffsetTime > AudioSettings.dspTime)
                {
                    yield return 0;
                }
                if (AudioSettings.dspTime < dspstart)
                {
                    var audioclip = CreateAudioClip(dspstart, sampleoffset);
                    currentAudioSource.PlayScheduled(audioclip, dspstart);
                    NextAudioSource();
                }
                dspstart += c.audioClipLength;
                sampleoffset += c.audioClipChannelSampleSize;
            }
        }

        /// <summary>
        /// 時刻dspstartから再生開始するAudioClipを生成する
        /// </summary>
        private AudioClip CreateAudioClip(double dspstart, int sampleoffset)
        {
            var b = new Buffer(c);
            double block_time = dspstart;
            double block_length = c.blockSamples / (double)c.frequency;
            int k = 0;
            for (int i = 0; i < c.audioClipBlockCount; ++i)
            {
                onGetBuffer(block_time, sampleoffset, c, b);
                for (int j = 0; j < c.blockSamples; ++j)
                {
                    audioClipData[k] = b.left[j];
                    ++k;
                    audioClipData[k] = b.right[j];
                    ++k;
                }
                block_time += block_length;
                sampleoffset += c.blockSamples;
            }
            var clip = AudioClip.Create("Sound", c.audioClipChannelSampleSize, 2, c.frequency, false);
            clip.SetData(audioClipData, 0);
            return clip;
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
        /// 次のAudioSourceへ遷移
        /// </summary>
        private void NextAudioSource()
        {
            currentAudioSourceIndex = (currentAudioSourceIndex + 1) % audioSources.Length;
        }
    }
}

