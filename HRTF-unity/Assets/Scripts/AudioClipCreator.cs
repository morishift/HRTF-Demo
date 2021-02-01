using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Test
{
    public class AudioClipCreator 
    {
        float [] subBlockL;
        float [] subBlockR;
        float[] subBlockSamples;
        int subBlockSize;
        PositionCircleLog positionCircleLog;
        WaveAudioClip waveAudioClip;
        OverlapAdd overlapAddL;

        public AudioClipCreator(int blocksz, PositionCircleLog log, WaveAudioClip wave)
        {
            subBlockSize = blocksz;
            positionCircleLog = log;
            waveAudioClip = wave;
            subBlockSamples = new float[blocksz * 2];
            
        }

        public AudioClip CreateAudioClip(int offset, int blockcount)
        {
            
        }

        public void CreateSubBlock(int angle, int offset)
        {
            waveAudioClip.GetData(subBlockL, offset, subBlockSize);
            
        }
    }
}

