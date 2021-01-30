using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace Test
{

    /// <summary>
    /// 重畳加算法
    /// </summary>
    public class OverlapAdd
    {
        const int SizeOfFloat = 4;
        int n;
        int nIR;
        int nSample;
        Fft fft;

        float[] frequencyResponseX;
        float[] frequencyResponseY;
        float[] overlap;
        float[] convolutionBuf1X;
        float[] convolutionBuf1Y;
        float[] convolutionBuf2X;
        float[] convolutionBuf2Y;
        float[] convolutionResult;

        public OverlapAdd(int _n, int _nsample, int _nir)
        {
            Debug.Log($"nsample:{_nsample} n:{_n} nir:{_nir}");
            n = _n;
            nIR = _nir;
            nSample = _n - _nir + 1;
            fft = new Fft(_n);
            frequencyResponseX = new float[_n];
            frequencyResponseY = new float[_n];
            overlap = new float[_nir - 1];
            convolutionBuf1X = new float[_n];
            convolutionBuf1Y = new float[_n];
            convolutionBuf2X = new float[_n];
            convolutionBuf2Y = new float[_n];
            convolutionResult = new float[_n - _nir + 1];
        }

        /// <summary>
        /// インパルス応答から周波数応答を設定する
        /// </summary>
        public void SetImpulseResponse(float[] ir)
        {
            Debug.Assert(ir.Length == nIR);

            Buffer.BlockCopy(ir, 0, frequencyResponseX, 0, SizeOfFloat * ir.Length);
            Array.Clear(frequencyResponseX, ir.Length, n - ir.Length);
            Array.Clear(frequencyResponseY, 0, n);
            fft.Forward(frequencyResponseX, frequencyResponseY);
        }

        /// <summary>
        /// 重畳加算法でsamplesとインパルス応答との畳み込みを計算する
        /// </summary>
        public void Convolution(float[] samples)
        {
            Debug.Assert(samples.Length == nSample);

            Buffer.BlockCopy(samples, 0, convolutionBuf1X, 0, SizeOfFloat * samples.Length);
            Array.Clear(convolutionBuf1X, samples.Length, n - samples.Length);
            Array.Clear(convolutionBuf1Y, 0, n);
            fft.Forward(convolutionBuf1X, convolutionBuf1Y);
            ComplexMultiple(convolutionBuf2X, convolutionBuf2Y, convolutionBuf1X, convolutionBuf1Y, frequencyResponseX, frequencyResponseY);
            fft.Inverse(convolutionBuf2X, convolutionBuf2Y);
            for (int i = nSample, j = 0; i < n; ++i, ++j)
            {
                convolutionBuf2X[j] += overlap[j];
                overlap[j] = convolutionBuf2X[i];
            }
            Buffer.BlockCopy(convolutionBuf2X, 0, convolutionResult, 0, SizeOfFloat * nSample);
        }

        /// <summary>
        /// 畳み込み結果
        /// </summary>
        public float[] GetConvolutionResult()
        {
            return convolutionResult;
        }

        /// <summary>
        /// オーバーラップ部取得
        /// </summary>
        public float[] GetOverlap()
        {
            return overlap;
        }

        /// <summary>
        /// 複素数配列の積
        /// </summary>
        private void ComplexMultiple(float[] retx, float[] rety, float[] x1, float[] y1, float[] x2, float[] y2)
        {
            for (int i = 0; i < n; ++i)
            {
                retx[i] = x1[i] * x2[i] - y1[i] * y2[i];
                rety[i] = x1[i] * y2[i] + y1[i] * x2[i];
            }
        }
    }
}
