using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Test
{
    public class Main : MonoBehaviour
    {
        const int SizeofFloat = 4;

        [SerializeField]
        Text debugText;
        [SerializeField]
        PositionCircle positionCircle;
        [SerializeField]
        PositionCircleLog positionCircleLog;
        [SerializeField]
        AudioClipStreamingPlayer audioClipStreamingPlayer;

        Constant c;
        bool isTouched;
        WaveAudioClip waveAudioClip;
        OverlapAdd overlapAddLeft;
        OverlapAdd overlapAddRight;
        float[] bufferSample;

        void Start()
        {
            Application.targetFrameRate = 60;
            c = Constant.CreateDefault();
            ImpulseResponses.LoadAll(c);
            waveAudioClip = WaveAudioClip.CreateWavAudioClip("Bytes/242_dr_bpm140_4-4_4_pop_mono.wav");
            positionCircle.onTouched += OnTouched;
            audioClipStreamingPlayer.onGetBuffer += OnGetBuffer;
            overlapAddLeft = new OverlapAdd(c);
            overlapAddRight = new OverlapAdd(c);
            bufferSample = new float[c.blockSamples];
            audioClipStreamingPlayer.Initialize(c);
        }

        /// <summary>
        /// 最初にタッチされたときに音再生開始する
        /// </summary>
        private void OnTouched()
        {
            if (isTouched)
            {
                return;
            }
            isTouched = true;
            audioClipStreamingPlayer.Play(AudioSettings.dspTime + 1.0);
        }

        /// <summary>
        /// AudioClipStreamingPlayerからのコールバック
        /// 再生するサウンド情報を渡す
        /// </summary>
        private void OnGetBuffer(double dsptime, int offset, Constant c, AudioClipStreamingPlayer.Buffer buffer)
        {
            // Debug.Log($"dsptime:{dsptime:0.000} offset:{offset}");

            waveAudioClip.GetData(bufferSample, offset, c.blockSamples);

            int angle = positionCircleLog.GetAngleAtTime(dsptime - c.audioClipLength);
            if (angle >= 0)
            {
                ImpulseResponses.Data data;
                data = ImpulseResponses.GetTransformedImpulseResponse(angle);
                overlapAddLeft.SetTransformedImpulseResponse(data.channelLX, data.channelLY);
                overlapAddRight.SetTransformedImpulseResponse(data.channelRX, data.channelRY);
            }
            else
            {
                overlapAddLeft.SetIdentifyImpulseResponse();
                overlapAddRight.SetIdentifyImpulseResponse();
            }
            overlapAddLeft.Convolution(bufferSample);
            overlapAddRight.Convolution(bufferSample);
            float[] ret_l = overlapAddLeft.GetConvolution();
            float[] ret_r = overlapAddRight.GetConvolution();
            Buffer.BlockCopy(ret_l, 0, buffer.left, 0, c.blockSamples * SizeofFloat);
            Buffer.BlockCopy(ret_r, 0, buffer.right, 0, c.blockSamples * SizeofFloat);
        }

        void Update()
        {
            positionCircle.SetTrackAngle(positionCircleLog.GetAngleAtTime(AudioSettings.dspTime - c.audioClipLength));
        }
    }
}

