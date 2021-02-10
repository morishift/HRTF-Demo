
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Test
{
    public class Main : MonoBehaviour
    {
        [SerializeField]
        Text debugText;
        [SerializeField]
        AudioClipStreamingPlayer audioClipStreamingPlayer;

        void Start()
        {
            Application.targetFrameRate = 60;

        }
    }
}

