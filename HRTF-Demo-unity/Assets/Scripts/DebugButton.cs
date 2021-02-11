using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

namespace Test
{
    public class DebugButton : MonoBehaviour
    {
        [SerializeField]
        Button buttonSrc;
        bool initializedFlg;

        List<GameObject> childList = new List<GameObject>();

        void Start()
        {
            Initialize();
        }

        /// <summary>
        /// 初期化
        /// </summary>
        private void Initialize()
        {
            if (initializedFlg)
                return;
            buttonSrc.gameObject.SetActive(false);
            initializedFlg = true;
        }

        /// <summary>
        /// ボタン追加
        /// </summary>
        public void AddButton(string caption, Action action)
        {
            Initialize();
            var go = Instantiate(buttonSrc.gameObject, Vector3.one, Quaternion.identity, transform);
            var text = go.GetComponentInChildren<Text>();
            var button = go.GetComponentInChildren<Button>();
            text.text = caption;
            button.onClick.AddListener(() => action?.Invoke());
            go.SetActive(true);
            childList.Add(go);
        }

        /// <summary>
        /// ボタン削除
        /// </summary>
        public void Clear()
        {
            childList.ForEach(_go => Destroy(_go));
            childList.Clear();
        }
    }
}
