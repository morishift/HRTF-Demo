using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Test
{

    public static class Constant
    {
        public const int Frequency = 44100;
        public const int ImpulseResponseSamples = 512;
        public const int BufferSize = 1 << 16;
        public const int DataSamples = BufferSize - (ImpulseResponseSamples - 1);
    }
}
