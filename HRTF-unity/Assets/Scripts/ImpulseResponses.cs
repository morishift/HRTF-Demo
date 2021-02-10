using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace Test
{
    /// <summary>
    /// インパルス応答のリスト
    /// </summary>
    public static class ImpulseResponses
    {
        public class Data
        {
            public Data(int bufsize)
            {
                channelLX = new float[bufsize];
                channelLY = new float[bufsize];
                channelRX = new float[bufsize];
                channelRY = new float[bufsize];
            }
            public float[] channelLX;
            public float[] channelLY;
            public float[] channelRX;
            public float[] channelRY;
            public int angle;
        }
        static Dictionary<int, Data> dictionary = new Dictionary<int, Data>();

        /// <summary>
        /// すべてのインパルス応答を読み込む
        /// </summary>
        public static void LoadAll(Constant c)
        {
            dictionary.Clear();

            Fft fft = new Fft(c.blockSize);

            for (int i = 0; i < 360; i += 5)
            {
                // Debug.Log($"Load angle:{i}");
                var ir = new Data(c.blockSize);
                var clip_l = WaveAudioClip.CreateWavAudioClip($"Bytes/elev0/L0e{i:000}a.wav");
                Debug.Assert(clip_l.samples == c.impulseResponseSamples);
                clip_l.GetData(ir.channelLX, 0, c.impulseResponseSamples);
                fft.Forward(ir.channelLX, ir.channelLY);
                var clip_r = WaveAudioClip.CreateWavAudioClip($"Bytes/elev0/R0e{i:000}a.wav");
                Debug.Assert(clip_r.samples == c.impulseResponseSamples);
                clip_r.GetData(ir.channelRX, 0, c.impulseResponseSamples);
                fft.Forward(ir.channelRX, ir.channelRY);
                ir.angle = i;
                dictionary[i] = ir;
            }
        }

        /// <summary>
        /// 角度に対するDFT済みのインパルス応答取得
        /// </summary>
        public static Data GetTransformedImpulseResponse(int angle)
        {
            Data ir;
            if (dictionary.TryGetValue(angle, out ir))
            {
                return ir;
            }
            else
            {
                return null;
            }
        }
    }
}
