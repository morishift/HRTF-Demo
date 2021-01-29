using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System;

namespace Test
{

    public class PositionCircle : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
    {
        public Action onChangedAngle;

        [SerializeField]
        Button centerButton;
        [SerializeField]
        RectTransform posRectTransform;
        [SerializeField]
        RectTransform pressPosRectTransform;

        float circleRadius;
        int selectedAngle;
        bool pointerDownFlg;
        bool isSelected;
        RectTransform _rectTransformCache;
        int oldAngle = -1000;

        void Start()
        {
            circleRadius = posRectTransform.localPosition.magnitude;
            centerButton.onClick.AddListener(OnClickCenterButton);
            isSelected = false;
            oldAngle = -1000;
            posRectTransform.gameObject.SetActive(false);
        }

        /// <summary>
        /// プレス時
        /// </summary>
        public void OnPointerDown(PointerEventData data)
        {
            pointerDownFlg = true;
        }

        /// <summary>
        /// プレス終了
        /// </summary>
        public void OnPointerUp(PointerEventData data)
        {
            pointerDownFlg = false;
        }

        /// <summary>
        /// 選択中の角度 5度刻み
        /// 正面を0度として右周りに角が大きくなる
        /// [0, 360) の値を5度刻みで返す
        /// </summary>
        public int GetAngle()
        {
            return (360 - selectedAngle + 90) % 360;
        }

        /// <summary>
        /// 選択中ならばtrue
        /// </summary>
        public bool IsSelected()
        {
            return isSelected;
        }

        /// <summary>
        /// 中央のボタンを押した
        /// </summary>
        private void OnClickCenterButton()
        {
            isSelected = false;
            oldAngle = -1000;
            onChangedAngle?.Invoke();
            posRectTransform.gameObject.SetActive(false);
        }

        void Update()
        {
            if (pointerDownFlg)
            {
                posRectTransform.gameObject.SetActive(true);
                pressPosRectTransform.gameObject.SetActive(true);
                isSelected = true;
                Vector2 pos;
                RectTransformUtility.ScreenPointToLocalPointInRectangle(rectTransform, Input.mousePosition, null, out pos);
                // Debug.Log($"pos x:{pos.x:0.00} y:{pos.y:0.00}");
                pos = PositionOnCircumference(pos);
                pressPosRectTransform.localPosition = pos;
                selectedAngle = (int)(PositionToAngle(pos) + 360);
                selectedAngle = ((selectedAngle * 10 + 25) / 50 * 50 / 10) % 360;
                posRectTransform.localPosition = AngleToPositionOnCircumference(selectedAngle);
                if (oldAngle != GetAngle())
                {
                    onChangedAngle?.Invoke();
                    oldAngle = GetAngle();
                    //Debug.Log($"angle:{GetAngle()}");
                }
            }
            else
            {
                pressPosRectTransform.gameObject.SetActive(false);
            }
        }

        /// <summary>
        /// このオブジェクトのRectTransform
        /// </summary>
        /// <value></value>
        private RectTransform rectTransform
        {
            get
            {
                if (_rectTransformCache == null)
                {
                    _rectTransformCache = GetComponent<RectTransform>();
                }
                return _rectTransformCache;
            }
        }

        /// <summary>
        /// 円周上の位置に変換
        /// </summary>
        private Vector2 PositionOnCircumference(Vector2 pos)
        {
            return pos.normalized * circleRadius;
        }

        /// <summary>
        /// 円周上の位置に変換
        /// </summary>
        private float PositionToAngle(Vector2 pos)
        {
            return Mathf.Atan2(pos.y, pos.x) * Mathf.Rad2Deg;
        }

        /// <summary>
        /// 角度から円周上の位置を計算
        /// </summary>
        private Vector2 AngleToPositionOnCircumference(float angle)
        {
            Vector2 p;
            float rad = angle * Mathf.Deg2Rad;
            p.x = Mathf.Cos(rad);
            p.y = Mathf.Sin(rad);
            return p * circleRadius;
        }
    }
}
