
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

        float[] x1 = new float[SampleSize] { 1, 2, 3, 4, 5 };
        float[] x2 = new float[SampleSize] { 6, 5, 3, 2, 1 };
        float[] impulseX = new float[IRSize] { 0.1f, 0.2f, 0.3f, 0.4f };

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
            float[] c;
            var v = new OverlapAdd(BufferSize, SampleSize, IRSize);
            v.SetImpulseResponse(impulseX);
            v.Convolution(x1);
            c = v.GetConvolution();

            Debug.Log($"x1 convolution =================================");
            for (int i = 0; i < c.Length; ++i)
            {
                Debug.Log($"[{i}]:{c[i]:0.00}");
            }

            v.Convolution(x2);
            c = v.GetConvolution();
            Debug.Log($"x2 convolution =================================");
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

