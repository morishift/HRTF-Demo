using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Test
{

    public class Constant
    {
        public int frequency;
        public int impulseResponseSamples;
        public int blockSamples;
        public int blockSize;
        public int audioClipChannelSampleSize;
        public int audioClipBlockCount;

        /// <summary>
        /// 生成するAudioClipの再生時間
        /// </summary>
        public double audioClipLength => audioClipChannelSampleSize / (double)frequency;
        /// <summary>
        /// AudioClip生成時の生成開始時刻オフセット
        /// </summary>
        public double audioClipCreateOffsetTime => audioClipLength * 0.1;

        public Constant()
        {
            SetDefault();
        }

        private void SetDefault()
        {
            frequency = 44100;
            impulseResponseSamples = 512;
            blockSize = 2 << 12;
            blockSamples = blockSize - (impulseResponseSamples - 1);
            audioClipBlockCount = 16;
            audioClipChannelSampleSize = blockSamples * audioClipBlockCount;
        }

        private void SetTest()
        {
            frequency = 44100;
            impulseResponseSamples = 4;
            blockSize = 8;
            blockSamples = blockSize - (impulseResponseSamples - 1);
            audioClipBlockCount = 1;
            audioClipChannelSampleSize = blockSamples * audioClipBlockCount;
        }

        ////////////////////////////////////////////////////////////////

        public static Constant CreateDefault()
        {
            var c = new Constant();
            c.SetDefault();
            return c;
        }

        public static Constant CreateTest()
        {
            var c = new Constant();
            c.SetTest();
            return c;
        }
    }
}
