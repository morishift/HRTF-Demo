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

        public Constant()
        {
            SetDefault();
        }

        private void SetDefault()
        {
            frequency = 44100;
            impulseResponseSamples = 512;
            blockSize = 1 << 16;
            blockSamples = blockSize - (impulseResponseSamples - 1);
        }

        private void SetTest()
        {
            frequency = 44100;
            impulseResponseSamples = 4;
            blockSize = 8;
            blockSamples = blockSize - (impulseResponseSamples - 1);
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
