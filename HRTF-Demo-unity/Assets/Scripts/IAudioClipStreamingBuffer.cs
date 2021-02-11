using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace Test
{
    public interface IAudioClipStreamingBuffer
    {
        void GetAngleAtTime(double[] dsptimes, int[] angle);
        void GetBlockBuffer(int angle, int sampleoffset, AudioClipStreamingPlayer.BlockBuffer block);
    }
}
