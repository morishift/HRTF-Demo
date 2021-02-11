#if false

#if false

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Test
{

    public static class WaveConvolution
    {
        static float[] impulseDFTX_L = new float[Constant.ConvolutionBufferSize];
        static float[] impulseDFTY_L = new float[Constant.ConvolutionBufferSize];
        static float[] impulseDFTX_R = new float[Constant.ConvolutionBufferSize];
        static float[] impulseDFTY_R = new float[Constant.ConvolutionBufferSize];

        static float[] dataDFTX = new float[Constant.ConvolutionBufferSize];
        static float[] dataDFTY = new float[Constant.ConvolutionBufferSize];
        static float[] convolutionBufX = new float[Constant.ConvolutionBufferSize];
        static float[] convolutionBufY = new float[Constant.ConvolutionBufferSize];
        static float[] convolutionOverlapL = new float[Constant.ImpulseResponseSamples - 1];
        static float[] convolutionOverlapR = new float[Constant.ImpulseResponseSamples - 1];

        static float[] sinTable = new float[Constant.ConvolutionBufferSize];

        static WaveConvolution()
        {
            SetImpulseIdentify();
        }

        public static void SetImpulseIdentify()
        {
            float[] t = new float[1] { 1.0f };
            SetImpulseL(t);
            SetImpulseR(t);
        }

        /// <summary>
        /// 重畳加算法で畳み込み計算 左チャンネル
        /// </summary>
        public static void ConvolutionL(float[] result, float[] data)
        {
            Debug.Assert(data.Length == Constant.DataSamples);

            CopyToBuffer(data, dataDFTX, dataDFTY);
            FFT(dataDFTX, dataDFTY, Constant.ConvolutionBufferPow2, 1);
            ComplexMultiple(convolutionBufX, convolutionBufY, dataDFTX, dataDFTY, impulseDFTX_L, impulseDFTY_L);
            FFT(convolutionBufX, convolutionBufY, Constant.ConvolutionBufferPow2, -1);
            for (int i = 0; i < Constant.ImpulseResponseSamples - 1; ++i)
            {
                convolutionBufX[i] += convolutionOverlapL[i];
            }
            for (int i = 0; i < Constant.ImpulseResponseSamples - 1; ++i)
            {
                convolutionOverlapL[i] = convolutionBufX[data.Length + i];
            }
            CopyArray(convolutionBufX, result);
        }

        /// <summary>
        /// 重畳加算法で畳み込み計算 右チャンネル
        /// </summary>
        public static void ConvolutionR(float[] result, float[] data)
        {
            CopyToBuffer(data, dataDFTX, dataDFTY);
            FFT(dataDFTX, dataDFTY, Constant.ConvolutionBufferPow2, 1);
            ComplexMultiple(convolutionBufX, convolutionBufY, dataDFTX, dataDFTY, impulseDFTX_R, impulseDFTY_R);
            FFT(convolutionBufX, convolutionBufY, Constant.ConvolutionBufferPow2, -1);
            for (int i = 0; i < Constant.ImpulseResponseSamples - 1; ++i)
            {
                convolutionBufX[i] += convolutionOverlapR[i];
            }
            for (int i = 0; i < Constant.ImpulseResponseSamples - 1; ++i)
            {
                convolutionOverlapR[i] = convolutionBufX[data.Length + i];
            }
            CopyArray(convolutionBufX, result);
        }

        /// <summary>
        /// インパルス列を設定
        /// </summary>
        public static void SetImpulseL(float[] impulse)
        {
            CopyToBuffer(impulse, impulseDFTX_L, impulseDFTY_L);
            FFT(impulseDFTX_L, impulseDFTY_L, Constant.ConvolutionBufferPow2, 1);
        }

        /// <summary>
        /// インパルス列を設定
        /// </summary>
        public static void SetImpulseR(float[] impulse)
        {
            CopyToBuffer(impulse, impulseDFTX_R, impulseDFTY_R);
            FFT(impulseDFTX_R, impulseDFTY_R, Constant.ConvolutionBufferPow2, 1);
        }

        private static void CopyToBuffer(float[] data, float[] bufx, float[] bufy)
        {
            for (int i = 0; i < data.Length; ++i)
            {
                bufx[i] = data[i];
                bufy[i] = 0.0f;
            }
            for (int i = data.Length; i < bufx.Length; ++i)
            {
                bufx[i] = 0.0f;
                bufy[i] = 0.0f;
            }
        }

        private static void CopyArray(float[] src, float[] dest)
        {
            for (int i = 0; i < src.Length && i < dest.Length; ++i)
            {
                dest[i] = src[i];
            }
        }

        private static void ComplexMultiple(
            float[] retx, float[] rety,
            float[] x1, float[] y1,
            float[] x2, float[] y2)
        {
            for (int i = 0; i < retx.Length; ++i)
            {
                retx[i] = x1[i] * x2[i] - y1[i] * y2[i];
                rety[i] = x1[i] * y2[i] + y1[i] * x2[i];
            }
        }

        /// <summary>
        /// FFT
        /// </summary>
        /// <param name="x">実数値</param>
        /// <param name="y">虚数値</param>
        /// <param name="l">データサイズ。2の指数値。3ならばサイズは8ということ</param>
        /// <param name="f">1ならばFFT,-1ならばIFFT</param>
        public static void FFT(float[] x, float[] y, int l, int f)
        {
            int i, j, j1, j2, l1, l2, l3, l4, l5, n, k, h, n4;
            float s, c, s1, t, tx, ty, arg;

            n = (int)Mathf.Pow(2.0f, l);
            l1 = n;
            n4 = n / 4;
            s1 = f * 2.0f * Mathf.PI / n;

            for (l5 = 0; l5 < l; ++l5)
            {
                l2 = l1 - 1;
                l1 = l1 / 2;
                h = 0;
                arg = 0.0f;

                for (l3 = 0; l3 < l1; ++l3)
                {
                    s = - f * sinTable[h];
                    c = Mathf.Cos(arg);
                    s = Mathf.Sin(arg);
                    arg += s1;

                    for (l4 = l2; l4 < n; l4 += (l2 + 1))
                    {
                        j1 = l4 - l2 + l3;
                        j2 = j1 + l1;
                        tx = x[j1] - x[j2];
                        ty = y[j1] - y[j2];
                        x[j1] = x[j1] + x[j2];
                        y[j1] = y[j1] + y[j2];
                        x[j2] = c * tx + s * ty;
                        y[j2] = c * ty - s * tx;
                    }
                }
                s1 = s1 * 2.0f;
            }

            if (f < 0)
            {
                for (i = 0; i < n; ++i)
                {
                    x[i] = x[i] / n;
                    y[i] = y[i] / n;
                }
            }

            j = 0;
            for (i = 0; i < n - 1; ++i)
            {
                if (i <= j)
                {
                    t = x[i];
                    x[i] = x[j];
                    x[j] = t;
                    t = y[i];
                    y[i] = y[j];
                    y[j] = t;
                }
                k = n / 2;
                while (k <= j)
                {
                    j = j - k;
                    k = k / 2;
                }
                j += k;
            }
        }
    }
}
#else

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Test
{

    public static class WaveConvolution
    {
        static float[] impulseL;
        static float[] impulseR;

        static WaveConvolution()
        {
            SetImpulseIdentify();
        }

        public static void ConvolutionL(float[] result, float[] data)
        {
            for (int i = 0; i < result.Length; ++i)
            {
                result[i] = 0;
                for (int j = 0; j <= i && j < impulseL.Length; ++j)
                {
                    result[i] += impulseL[j] * data[i - j];
                }
            }
        }
        public static void ConvolutionR(float[] result, float[] data)
        {
            for (int i = 0; i < result.Length; ++i)
            {
                result[i] = 0;
                for (int j = 0; j <= i && j < impulseR.Length; ++j)
                {
                    result[i] += impulseR[j] * data[i - j];
                }
            }
        }

        public static void SetImpulseIdentify()
        {
            float[] t = new float[1] { 1.0f };
            SetImpulseL(t);
            SetImpulseR(t);
        }

       /// <summary>
        /// インパルス列を設定
        /// </summary>
        public static void SetImpulseL(float[] impulse)
        {
            impulseL = impulse;
        }

        /// <summary>
        /// インパルス列を設定
        /// </summary>
        public static void SetImpulseR(float[] impulse)
        {
            impulseR = impulse;
        }

    }
}
#endif
#endif
