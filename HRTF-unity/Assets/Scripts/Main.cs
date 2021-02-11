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
        DebugButton debugButton;

        [SerializeField]
        Text messageText1;

        [SerializeField]
        Text messageText2;

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

            waveAudioClip = WaveAudioClip.CreateWavAudioClip("Bytes/DrumLoop2.wav");
            debugButton.AddButton("Drum1", () =>
            {
                waveAudioClip = WaveAudioClip.CreateWavAudioClip("Bytes/DrumLoop2.wav");
            });
            debugButton.AddButton("Drum2", () =>
            {
                waveAudioClip = WaveAudioClip.CreateWavAudioClip("Bytes/TightFunkBreak-mono.wav");
            });
            debugButton.AddButton("OchestraStrings", () =>
            {
                waveAudioClip = WaveAudioClip.CreateWavAudioClip("Bytes/OchestraStrings-mono.wav");
            });
            debugButton.AddButton("Siren", () =>
            {
                waveAudioClip = WaveAudioClip.CreateWavAudioClip("Bytes/PoliseCarSiren-mono.wav");
            });
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
            int angle1 = positionCircle.GetAngle();
            int angle2 = positionCircleLog.GetAngleAtTime(AudioSettings.dspTime - c.audioClipLength);
            positionCircle.SetTrackAngle(angle2);

            messageText1.gameObject.SetActive(angle1 >= 0);
            if (angle1 >= 0)
            {
                messageText1.text = $"{angle1}°";
            }
            messageText2.gameObject.SetActive(angle2 >= 0);
            if (angle2 >= 0)
            {
                messageText2.text = $"{angle2}°";
            }
        }
    }
}

