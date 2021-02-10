using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System;

namespace Test
{
    public class PositionCircleLog : MonoBehaviour
    {
        class AngleAndTime
        {
            public int angle = 0;
            public double time = 0.0;
        }

        [SerializeField]
        PositionCircle positionCircle;

        /// <summary>
        /// 角度と、角度変更した時刻
        /// </summary>        
        List<AngleAndTime> angleAndTimeLog = new List<AngleAndTime>();

        void Awake()
        {
            positionCircle.onChangedAngle += OnChangeAngle;
        }

        /// <summary>
        /// 角度変更時
        /// </summary>
        private void OnChangeAngle()
        {
            double now = AudioSettings.dspTime;
            var a = new AngleAndTime();
            a.angle = positionCircle.GetAngle();
            a.time = now;
            angleAndTimeLog.Add(a);
            // 古いログ情報を削除
            // 5秒以上前のログが2件以上残らないように調整
            while (angleAndTimeLog.Count > 1 && now - angleAndTimeLog[1].time > 5)
            {
                angleAndTimeLog.RemoveAt(0);
            }

            Debug.Log($"angleAndTimeLog.Count:{angleAndTimeLog.Count}");
        }

        /// <summary>
        /// 渡された時刻のときに選択されていた角度
        /// </summary>
        public int GetAngleAtTime(double dsptime)
        {
            for (int i = angleAndTimeLog.Count - 1; i >= 0; --i)
            {
                if (angleAndTimeLog[i].time <= dsptime)
                {
                    return angleAndTimeLog[i].angle;
                }
            }
            if (angleAndTimeLog.Count > 0)
            {
                return angleAndTimeLog[0].angle;
            }
            else
            {
                return 0;
            }
        }
    }
}

