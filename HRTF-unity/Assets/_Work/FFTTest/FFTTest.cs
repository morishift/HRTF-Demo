
// using System.Collections;
// using System.Collections.Generic;
// using UnityEngine;
// using UnityEngine.UI;
// using System;

// namespace Test
// {
//     public class FFTTest : MonoBehaviour
//     {
//         [SerializeField]
//         DebugButton debugButton;

//         float[] x = new float[Constant.DataSamples] { 1, 2, 3, 4, 5};
//         float[] y = new float[Constant.DataSamples] { 0, 0, 0, 0, 0};
//         float[] impulseX = new float[Constant.ImpulseSamples] { 1, 0, 0, 0};
//         float[] impulseY = new float[Constant.ImpulseSamples] { 0, 0, 0, 0};
//         float[] resultX = new float[Constant.DataSamples];

//         void Start()
//         {
//             debugButton.AddButton("SetImpulseL", () =>
//             {
//                 WaveConvolution.SetImpulseL(impulseX);
//                 WaveConvolution.ConvolutionL(resultX, x);
//                 for (int i = 0; i < resultX.Length; ++i)
//                 {
//                     Debug.Log($"{resultX[i]:0.00}");
//                 }
//             });
//         }
//     }
// }

