
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

namespace Test
{
    public class PositionCircleTest : MonoBehaviour
    {
        [SerializeField]
        DebugButton debugButton;
        [SerializeField]
        PositionCircle positionCircle;
        [SerializeField]
        PositionCircleLog positionCircleLog;
        [SerializeField]
        Text fpsText;

        [SerializeField]
        int trackAngle;

        void Start()
        {
            Application.targetFrameRate = 60;
            debugButton.AddButton("ConvertTest1", () => ConvertTest1());
        }

        /// <summary>
        /// 変換テスト
        /// </summary>
        private void ConvertTest1()
        {
        }

        void Update()
        {
            fpsText.text = $"{1 / Time.deltaTime:0.00}";
            int angle = positionCircleLog.GetAngleAtTime(AudioSettings.dspTime - 1.0);
            Debug.Log($"angle:{angle}");
            positionCircle.SetTrackAngle(angle);
            trackAngle = angle;
        }
    }
}

