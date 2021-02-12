using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Test
{
    /// <summary>
    /// 参考:
    /// C言語による標準アルゴリズム事典
    /// https://github.com/okumuralab/algo-c/blob/master/src/fft.c
    /// </summary>
    public class Fft
    {
        float[] sintbl;
        int[] bitrev;
        int n;

        /// <summary>
        /// 初期化
        /// nは2の累乗サイズ
        /// </summary>
        public Fft(int _n)
        {
            n = _n;
            sintbl = new float[n - n / 4];
            bitrev = new int[n];
            MakeSinTbl();
            MakeBitRev();
        }

        /// <summary>
        /// FFT
        /// </summary>
        public void Forward(float[] x, float[] y)
        {
            FftCore(false, x, y);
        }

        /// <summary>
        /// Inverse FFT 
        /// </summary>
        public void Inverse(float[] x, float[] y)
        {
            FftCore(true, x, y);
        }

        /// <summary>
        /// 関数{\tt fft()}の下請けとして三角関数表を作る.
        /// </summary>
        private void MakeSinTbl()
        {
            int i, n2, n4, n8;
            float c, s, dc, ds, t;

            n2 = n / 2;
            n4 = n / 4;
            n8 = n / 8;
            t = Mathf.Sin(Mathf.PI / n);
            dc = 2 * t * t; ds = Mathf.Sqrt(dc * (2 - dc));
            t = 2 * dc; c = sintbl[n4] = 1; s = sintbl[0] = 0;
            for (i = 1; i < n8; i++)
            {
                c -= dc; dc += t * c;
                s += ds; ds -= t * s;
                sintbl[i] = s; sintbl[n4 - i] = c;
            }
            if (n8 != 0) sintbl[n8] = Mathf.Sqrt(0.5f);
            for (i = 0; i < n4; i++)
            {
                sintbl[n2 - i] = sintbl[i];
                sintbl[i + n2] = -sintbl[i];
            }
        }

        /// <summary>
        /// 関数{\tt fft()}の下請けとしてビット反転表を作る.
        /// </summary>
        private void MakeBitRev()
        {
            int i, j, k, n2;

            n2 = n / 2; i = j = 0;
            for (; ; )
            {
                bitrev[i] = j;
                if (++i >= n) break;
                k = n2;
                while (k <= j)
                {
                    j -= k; k /= 2;
                }
                j += k;
            }
        }

        /// <summary>
        ///   高速Fourier変換 (Cooley--Tukeyのアルゴリズム).
        ///   標本点の数 {\tt n} は2の整数乗に限る.
        ///   {\tt x[$k$]} が実部, {\tt y[$k$]} が虚部 ($k = 0$, $1$, $2$,
        ///   \ldots, $|{\tt n}| - 1$).
        ///   結果は {\tt x[]}, {\tt y[]} に上書きされる.
        ///   ${\tt n} = 0$ なら表のメモリを解放する.
        ///   ${\tt n} < 0$ なら逆変換を行う.
        ///   前回と異なる $|{\tt n}|$ の値で呼び出すと,
        ///   三角関数とビット反転の表を作るために多少余分に時間がかかる.
        ///   この表のための記憶領域獲得に失敗すると1を返す (正常終了時
        ///   の戻り値は0).
        ///   これらの表の記憶領域を解放するには ${\tt n} = 0$ として
        ///   呼び出す (このときは {\tt x[]}, {\tt y[]} の値は変わらない).
        /// </summary>
        /// <param name="inverse"></param>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns></returns>
        private void FftCore(bool inverse, float[] x, float[] y)
        {
            int i, j, k, ik, h, d, k2, n4;
            float t, s, c, dx, dy;

            n4 = n / 4;

            for (i = 0; i < n; i++)
            {    /* ビット反転 */
                j = bitrev[i];
                if (i < j)
                {
                    t = x[i]; x[i] = x[j]; x[j] = t;
                    t = y[i]; y[i] = y[j]; y[j] = t;
                }
            }
            for (k = 1; k < n; k = k2)
            {    /* 変換 */
                h = 0; k2 = k + k; d = n / k2;
                for (j = 0; j < k; j++)
                {
                    c = sintbl[h + n4];
                    if (inverse) s = -sintbl[h];
                    else s = sintbl[h];
                    for (i = j; i < n; i += k2)
                    {
                        ik = i + k;
                        dx = s * y[ik] + c * x[ik];
                        dy = c * y[ik] - s * x[ik];
                        x[ik] = x[i] - dx; x[i] += dx;
                        y[ik] = y[i] - dy; y[i] += dy;
                    }
                    h += d;
                }
            }
            // algo-c fft.cオリジナルでは、逆変換でないときにnで割っている
            // この場合convolutionを求めるときにnを掛ける必要が出るため、逆変換のときにnで割るように変更
            // if (inverse == false)  /* 逆変換でないならnで割る */
            if (inverse)  /* 逆変換ならnで割る */
            {
                for (i = 0; i < n; i++)
                {
                    x[i] /= n;
                    y[i] /= n;
                }
            }
        }
    }
}

