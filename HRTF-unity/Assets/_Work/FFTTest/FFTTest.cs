
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

        Constant constant;
        float[] x1;
        float[] x2;
        float[] impulseX;


        void Start()
        {
            constant = new Constant();
            constant.SetTest();
            x1 = new float[] { 1, 2, 3, 4, 5 };
            x2 = new float[] { 6, 5, 3, 2, 1 };
            impulseX = new float[] { 0.1f, 0.2f, 0.3f, 0.4f };
            debugButton.AddButton("ConvertTest1", () => ConvertTest1());
            debugButton.AddButton("FftTestFunc", () => FftTestFunc());
        }

        /// <summary>
        /// 変換テスト
        /// </summary>
        private void ConvertTest1()
        {
            float[] c;
            var v = new OverlapAdd(constant);
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

        private void FftTestFunc()
        {
            float[] x = new float[] { 1, 2, 3, 4 };
            float[] y = new float[] { 0, 0, 0, 0 };
            var t = new Fft(4);
            t.Forward(x, y);
            t.Inverse(x, y);
            Debug.Log($"result =================================");
            for (int i = 0; i < x.Length; ++i)
            {
                Debug.Log($"[{i}]:{x[i]:0.00}");
            }
        }
    }
}

