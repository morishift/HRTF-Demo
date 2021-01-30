
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

namespace Test
{
    public class FFTTest : MonoBehaviour
    {
        [SerializeField]
        DebugButton debugButton;

        const int BufferSize = 1 << 3;
        const int IRSize = 4;
        const int SampleSize = BufferSize - IRSize + 1;

        float[] x = new float[SampleSize] { 1, 2, 3, 4, 5 };
        float[] impulseX = new float[IRSize] { 0.1f, 0.2f, 0.3f, 0.4f };
        float[] resultX = new float[SampleSize];

        void Start()
        {
            debugButton.AddButton("ConvertTest1", () => ConvertTest1());
        }

        /// <summary>
        /// 変換テスト
        /// </summary>
        private void ConvertTest1()
        {
            Debug.Log($"SampleSize:{SampleSize}");

            var v = new OverlapAdd(BufferSize, SampleSize, IRSize);
            x.CopyTo(resultX, 0);
            v.SetImpulseResponse(impulseX);
            v.Convolution(resultX);
            var c = v.GetConvolutionResult();

            Debug.Log($"convolution =================================");
            for (int i = 0; i < c.Length; ++i)
            {
                Debug.Log($"[{i}]:{c[i]:0.00}");
            }

            Debug.Log($"overlap =================================");
            var overlap = v.GetOverlap();
            for (int i = 0; i < overlap.Length; ++i)
            {
                Debug.Log($"[{i}]:{overlap[i]:0.00}");
            }
        }
    }
}

