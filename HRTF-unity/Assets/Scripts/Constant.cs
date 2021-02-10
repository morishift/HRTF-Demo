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

        public void SetDefault()
        {
            frequency = 44100;
            impulseResponseSamples = 512;
            blockSize = 1 << 16;
            blockSamples = blockSize - (impulseResponseSamples - 1);
        }

        public void SetTest()
        {
            frequency = 44100;
            impulseResponseSamples = 4;
            blockSize = 8;
            blockSamples = blockSize - (impulseResponseSamples - 1);
        }
    }
}
