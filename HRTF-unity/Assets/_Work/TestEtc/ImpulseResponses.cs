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
        public class ImpulseResponse
        {
            public ImpulseResponse(int bufsize)
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
        static Dictionary<int, ImpulseResponse> dictionary = new Dictionary<int, ImpulseResponse>();

        /// <summary>
        /// すべてのインパルス応答を読み込む
        /// </summary>
        public static void LoadAll(int bufsize, int irsize)
        {
            dictionary.Clear();

            Fft fft = new Fft(bufsize);

            for (int i = 0; i < 360; i += 5)
            {
                var ir = new ImpulseResponse(bufsize);
                var clip_l = WaveAudioClip.CreateWavAudioClip($"Bytes/elev0/L0e{i:000}a.wav");
                clip_l.GetData(ir.channelLX, 0, irsize);
                fft.Forward(ir.channelLX, ir.channelLY);
                var clip_r = WaveAudioClip.CreateWavAudioClip($"Bytes/elev0/R0e{i:000}a.wav");
                clip_r.GetData(ir.channelLX, 0, irsize);
                fft.Forward(ir.channelRX, ir.channelRY);
                ir.angle = i;
                dictionary[i] = ir;
            }
        }

        /// <summary>
        /// 角度に対するDFT済みのインパルス応答取得
        /// </summary>
        public static void GetTransformedImpulseResponse(int angle, out float[] lx, out float[] ly, out float[] rx, out float[] ry)
        {
            ImpulseResponse ir;
            if (dictionary.TryGetValue(angle, out ir))
            {
                lx = ir.channelLX;
                ly = ir.channelLY;
                rx = ir.channelRX;
                ry = ir.channelRY;
            }
            else
            {
                lx = null;
                ly = null;
                rx = null;
                ry = null;
            }
        }
    }
}
