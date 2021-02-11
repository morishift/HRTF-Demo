using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading;
using System.Threading.Tasks;


namespace Test
{
    /// <summary>
    /// AudioClipをストリーミング再生のように再生する。
    /// WebGLの場合async,awaitが使えないためマルチスレッドを使ったバッファ処理は行わない
    /// </summary>
    public class AudioClipStreamingPlayer : MonoBehaviour
    {
        public class BlockBuffer
        {
            public BlockBuffer(Constant c)
            {
                left = new float[c.blockSamples];
                right = new float[c.blockSamples];
            }
            public float[] left;
            public float[] right;
        }

        [SerializeField]
        ScheduledAudioSource[] audioSources;
        Constant c;
        float[] audioClipData;
        int currentAudioSourceIndex;
        Coroutine playCoroutine;
        BlockBuffer blockBuffer;
        IAudioClipStreamingBuffer audioClipStreamingBuffer;
        double[] dspTimes;
        int[] angleAtTime;

        public void Initialize(Constant _c, IAudioClipStreamingBuffer streamingbuf)
        {
            Stop();
            c = _c;
            audioClipData = new float[c.audioClipChannelSampleSize * 2];
            dspTimes = new double[c.audioClipBlockCount];
            angleAtTime = new int[c.audioClipBlockCount];
            audioClipStreamingBuffer = streamingbuf;
            blockBuffer = new BlockBuffer(c);
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
                    AudioClip audioclip = null;
#if UNITY_WEBGL                    
                    CreateAudioClip(dspstart, sampleoffset, (_c) => audioclip = _c);
#else
                    FireAndForget(CreateAudioClip(dspstart, sampleoffset, (_c) => audioclip = _c));
#endif                    
                    while (audioclip == null)
                    {
                        yield return 0;
                    }
                    currentAudioSource.PlayScheduled(audioclip, dspstart);
                    NextAudioSource();
                }
                dspstart += c.audioClipLength;
                sampleoffset += c.audioClipChannelSampleSize;
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
        /// 次のAudioSourceへ遷移
        /// </summary>
        private void NextAudioSource()
        {
            currentAudioSourceIndex = (currentAudioSourceIndex + 1) % audioSources.Length;
        }

#if UNITY_WEBGL
        /// <summary>
        /// 時刻dspstartから再生開始するAudioClipを生成する
        /// </summary>
        private void CreateAudioClip(double dspstart, int sampleoffset, Action<AudioClip> end)
#else
        private async Task CreateAudioClip(double dspstart, int sampleoffset, Action<AudioClip> end)
#endif

        {
            double block_time = dspstart;
            double block_time_length = c.blockSamples / (double)c.frequency;

            // 時刻に対応する角度情報の取得
            for (int i = 0; i < c.audioClipBlockCount; ++i)
            {
                dspTimes[i] = block_time;
                block_time += block_time_length;
            }
            audioClipStreamingBuffer.GetAngleAtTime(dspTimes, angleAtTime);

#if !UNITY_WEBGL
            // 畳み込み計算済みバッファ取得
            await Task.Run(() =>
#endif            
            {
                int k = 0;
                for (int i = 0; i < c.audioClipBlockCount; ++i)
                {
                    audioClipStreamingBuffer.GetBlockBuffer(angleAtTime[i], sampleoffset, blockBuffer);
                    for (int j = 0; j < c.blockSamples; ++j)
                    {
                        audioClipData[k] = blockBuffer.left[j];
                        ++k;
                        audioClipData[k] = blockBuffer.right[j];
                        ++k;
                    }
                    sampleoffset += c.blockSamples;
                }
            }
#if !UNITY_WEBGL            
            );
#endif            

            // AudioClip生成
            var clip = AudioClip.Create("Sound", c.audioClipChannelSampleSize, 2, c.frequency, false);
            clip.SetData(audioClipData, 0);
            end?.Invoke(clip);
        }


#if !UNITY_WEBGL
        /// <summary>
        /// Taskを起動する
        /// </summary>
        private void FireAndForget(Task task)
        {
            // タスクを開始したときのリクエストを保存しておく
            task.ContinueWith(t =>
            {
                if (t.Status == TaskStatus.Canceled)
                {
                    // キャンセルされた
                    Debug.LogWarning("Task Cancelled");
                }
                else if (t.Status == TaskStatus.Faulted)
                {
                    // タスク実行時の例外
                    Debug.LogWarning("Task Faulted FireAndForgetError:" + t.Exception.ToString());
                }
            });
        }
#endif

    }
}
