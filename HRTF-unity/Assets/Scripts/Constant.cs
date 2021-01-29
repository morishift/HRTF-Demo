using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#if false
namespace Test
{

    public static class Constant
    {
        public const int Frequency = 44100;
        public const int ImpulseSamples = 512;
        public const int ConvolutionBufferPow2 = 15; // バッファサイズの2のベキ数
        public const int ConvolutionBufferSize = (1 << ConvolutionBufferPow2);
        public const int DataSamples = ConvolutionBufferSize - (ImpulseSamples - 1);
    }
}

#else

namespace Test
{

    public static class Constant
    {
        public const int Frequency = 44100;
        public const int ImpulseSamples = 4;
        public const int ConvolutionBufferPow2 = 3;
        public const int ConvolutionBufferSize = (1 << ConvolutionBufferPow2);
        public const int SplitTime = 2;
        public const int DataSamples = ConvolutionBufferSize - (ImpulseSamples - 1);
    }
}

#endif

