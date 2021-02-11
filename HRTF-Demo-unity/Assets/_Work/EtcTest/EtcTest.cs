﻿
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

namespace Test
{
    public class EtcTest : MonoBehaviour
    {
        [SerializeField]
        DebugButton debugButton;
        [SerializeField]
        AudioClipStreamingPlayer audioClipStreamingPlayer;

        float[] x1;
        float[] x2;
        float[] impulseX;        

        [SerializeField]
        bool convertTestFlg;
        [SerializeField]
        bool fftTestFlg;
        [SerializeField]
        bool impulseResponsesFlg;
        [SerializeField]
        bool audioClipStreamingPlayerTestFlg;

        WaveAudioClip waveAudioClip;

        void Start()
        {
            ConvertTest();
            FftTest();
            ImpulseResponsesTest();
            AudioClipStreamingPlayerTest();
        }

        /// <summary>
        /// 変換テスト
        /// </summary>
        private void ConvertTest()
        {
            if (!convertTestFlg)
                return;

            float[] x1 = new float[] { 1, 2, 3, 4, 5 };
            float[] x2 = new float[] { 6, 5, 3, 2, 1 };
            float[] impulseX = new float[] { 0.1f, 0.2f, 0.3f, 0.4f };
            float[] ret;

            debugButton.AddButton("ConvertTest", () =>
            {
                var v = new OverlapAdd(Constant.CreateTest());
                v.SetImpulseResponse(impulseX);
                //v.SetIdentifyImpulseResponse();
                v.Convolution(x1);
                ret = v.GetConvolution();

                Debug.Log($"x1 convolution =================================");
                for (int i = 0; i < ret.Length; ++i)
                {
                    Debug.Log($"[{i}]:{ret[i]:0.00}");
                }

                v.Convolution(x2);
                ret = v.GetConvolution();
                Debug.Log($"x2 convolution =================================");
                for (int i = 0; i < ret.Length; ++i)
                {
                    Debug.Log($"[{i}]:{ret[i]:0.00}");
                }

                Debug.Log($"overlap =================================");
                var overlap = v.GetOverlap();
                for (int i = 0; i < overlap.Length; ++i)
                {
                    Debug.Log($"[{i}]:{overlap[i]:0.00}");
                }
            });
        }

        /// <summary>
        /// FFTテスト
        /// </summary>
        private void FftTest()
        {
            if (!fftTestFlg)
                return;

            debugButton.AddButton("FftTest", () =>
            {
                float[] x = new float[] { 1, 0, 0, 0 };
                float[] y = new float[] { 0, 0, 0, 0 };
                var t = new Fft(4);
                t.Forward(x, y);
                t.Inverse(x, y);
                Debug.Log($"result =================================");
                for (int i = 0; i < x.Length; ++i)
                {
                    Debug.Log($"[{i}]:{x[i]:0.00} {y[i]:0.00}");
                }
            });
        }

        /// <summary>
        /// ImpulseResponsesテスト
        /// </summary>
        private void ImpulseResponsesTest()
        {
            if (!impulseResponsesFlg)
                return;
            debugButton.AddButton("ImpulseResponsesTest", () =>
            {
                ImpulseResponses.LoadAll(Constant.CreateDefault());
            });
        }

        /// <summary>
        /// AudioClipStreamingPlayerのテスト
        /// </summary>
        private void AudioClipStreamingPlayerTest()
        {
            if (!audioClipStreamingPlayerTestFlg)
                return;

            audioClipStreamingPlayer.Initialize(Constant.CreateDefault());
            waveAudioClip = WaveAudioClip.CreateWavAudioClip("Bytes/242_dr_bpm140_4-4_4_pop_mono.wav");
            audioClipStreamingPlayer.onGetBuffer += OnGetBuffer;

            debugButton.AddButton("AudioClipStreamingPlayerTest", () =>
            {
                audioClipStreamingPlayer.Play(AudioSettings.dspTime + 1.0f);
            });
            debugButton.AddButton("AudioClipStreamingPlayerTest Stop", () =>
            {
                audioClipStreamingPlayer.Stop();
            });
        }

        /// <summary>
        /// AudioClipStreamingPlayerからのコールバック
        /// 再生するサウンド情報を渡す
        /// </summary>
        private void OnGetBuffer(double dsptime, int offset, Constant c, AudioClipStreamingPlayer.Buffer buffer)
        {
            Debug.Log($"dsptime:{dsptime:0.000} offset:{offset}");
            waveAudioClip.GetData(buffer.left, offset, buffer.left.Length);
            waveAudioClip.GetData(buffer.right, offset, buffer.right.Length);
        }
    }
}
