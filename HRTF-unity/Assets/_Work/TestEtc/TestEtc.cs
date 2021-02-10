
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

namespace Test
{
    public class TestEtc : MonoBehaviour
    {
        [SerializeField]
        DebugButton debugButton;
        
        void Start()
        {            
            debugButton.AddButton("LoadAll", () => LoadImpulseResponse());
        }

        private void LoadImpulseResponse()
        {
            ImpulseResponses.LoadAll(Constant.CreateDefault());
        }

        /// <summary>
        /// Streaming形式でサウンド作成
        /// </summary>
        private AudioClip CreateAudioClip(int offset, int size)
        {
            // Debug.Log($"channels:{waveAudioClip.channels} frequency:{waveAudioClip.frequency} samples:{waveAudioClip.samples}");

            // waveAudioClip.GetData(waveInputBuffer, offset, size);
            // overlapAddL.Convolution(waveInputBuffer);
            // overlapAddR.Convolution(waveInputBuffer);
            // float[] output_l = overlapAddL.GetConvolution();
            // float[] output_r = overlapAddR.GetConvolution();
            // for (int i = 0, j = 0; j < size; i += 2, ++j)
            // {
            //     waveOutputBuffer[i] = output_l[j];
            //     waveOutputBuffer[i + 1] = output_r[j];
            // }
            // var clip = AudioClip.Create("Sound", size, 2, waveAudioClip.frequency, false);
            // clip.SetData(waveOutputBuffer, 0);
            // return clip;
            return null;
        }
    }
}

