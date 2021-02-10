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
        const int SizeofFloat = 4;
        int blockSize;
        int impulseResponseSamples;
        int blockSamples;
        Fft fft;

        float[] frequencyResponseX;
        float[] frequencyResponseY;
        float[] overlap;
        float[] convolutionBuf1X;
        float[] convolutionBuf1Y;
        float[] convolutionBuf2X;
        float[] convolutionBuf2Y;
        float[] convolutionResult;

        //public OverlapAdd(int _n, int _nsample, int _nir)
        public OverlapAdd(Constant c)
        {
            Debug.Log($"samples:{c.blockSamples} size:{c.blockSize} ir:{c.impulseResponseSamples}");
            blockSize = c.blockSize;
            impulseResponseSamples = c.impulseResponseSamples;
            blockSamples = c.blockSamples;
            Debug.Assert(blockSize == impulseResponseSamples - 1 + blockSamples);

            fft = new Fft(blockSize);
            frequencyResponseX = new float[blockSize];
            frequencyResponseY = new float[blockSize];
            overlap = new float[impulseResponseSamples - 1];
            convolutionBuf1X = new float[blockSize];
            convolutionBuf1Y = new float[blockSize];
            convolutionBuf2X = new float[blockSize];
            convolutionBuf2Y = new float[blockSize];
            convolutionResult = new float[blockSamples];
        }

        /// <summary>
        /// インパルス応答から周波数応答を設定する
        /// </summary>
        public void SetImpulseResponse(float[] ir)
        {
            Debug.Assert(ir.Length == impulseResponseSamples);

            Buffer.BlockCopy(ir, 0, frequencyResponseX, 0, SizeofFloat * ir.Length);
            Array.Clear(frequencyResponseX, ir.Length, blockSize - ir.Length);
            Array.Clear(frequencyResponseY, 0, blockSize);
            fft.Forward(frequencyResponseX, frequencyResponseY);
        }

        public void SetIdentifyImpulseResponse()
        {
            for (int i = 0; i < blockSize; ++i)
            {
                frequencyResponseX[i] = 1.0f;
                frequencyResponseY[i] = 1.0f;
            }
        }

        /// <summary>
        /// 重畳加算法でsamplesとインパルス応答との畳み込みを計算する
        /// </summary>
        public void Convolution(float[] samples)
        {
            Debug.Assert(samples.Length == blockSamples);

            Buffer.BlockCopy(samples, 0, convolutionBuf1X, 0, SizeofFloat * samples.Length);
            Array.Clear(convolutionBuf1X, samples.Length, blockSize - samples.Length);
            Array.Clear(convolutionBuf1Y, 0, blockSize);
            fft.Forward(convolutionBuf1X, convolutionBuf1Y);
            ComplexMultiple(convolutionBuf2X, convolutionBuf2Y, convolutionBuf1X, convolutionBuf1Y, frequencyResponseX, frequencyResponseY);
            fft.Inverse(convolutionBuf2X, convolutionBuf2Y);
            for (int i = blockSamples, j = 0; i < blockSize; ++i, ++j)
            {
                convolutionBuf2X[j] += overlap[j];
                overlap[j] = convolutionBuf2X[i];
            }
            Buffer.BlockCopy(convolutionBuf2X, 0, convolutionResult, 0, SizeofFloat * blockSamples);
        }

        /// <summary>
        /// 畳み込み結果
        /// </summary>
        public float[] GetConvolution()
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
            for (int i = 0; i < blockSize; ++i)
            {
                retx[i] = x1[i] * x2[i] - y1[i] * y2[i];
                rety[i] = x1[i] * y2[i] + y1[i] * x2[i];
            }
        }
    }
}
