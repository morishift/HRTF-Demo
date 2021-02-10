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
        PositionCircleLog positionCircleLog;
        WaveAudioClip waveAudioClip;
        OverlapAdd overlapAddL;
        Constant constant;

        public AudioClipCreator(Constant c, PositionCircleLog log, WaveAudioClip wave)
        {
            constant = c;
            positionCircleLog = log;
            waveAudioClip = wave;
            subBlockSamples = new float[constant.blockSize];
            overlapAddL = new OverlapAdd(c);
        }

        // public void CreateSubBlock(int angle, int offset)
        // {
        //     waveAudioClip.GetData(subBlockL, offset, subBlockSize);
            
        // }
    }
}

