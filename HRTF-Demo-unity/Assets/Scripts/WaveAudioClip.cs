using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

namespace Test
{
    public class WaveAudioClip
    {
        /// <summary>
        /// 波形
        /// </summary>
        public float[] waveData;
        /// <summary>
        /// チャンネル数
        /// </summary>
        public int channels;
        /// <summary>
        /// サンプリング周波数
        /// </summary>
        public int frequency;
        /// <summary>
        /// サンプル数
        /// </summary>
        public int samples;

        /// <summary>
        /// 波形データ取得
        /// offset+sizeが終端を超えている場合はループさせた情報を返す
        /// </summary>
        public void GetData(float[] data, int offset, int size)
        {
            offset = offset % samples;
            if (offset + size > samples)
            {
                // ループする場合 サウンドを2つに分けてデータを取得する
                int n1 = samples - offset;                
                // 音源の末尾部
                for (int i = 0, j = offset; i < n1; ++i, ++j)
                {
                    data[i] = waveData[j];
                }
                // 音源の先頭部
                int n2 = size - n1;
                for (int i = n1, j = 0; j < n2; ++i, ++j)
                {
                    data[i] = waveData[j];
                }
            }
            else
            {
                // ループしない場合
                for (int i = 0, j = offset; i < size; ++i, ++j)
                {
                    data[i] = waveData[j];
                }
            }
        }
    
        /// <summary>
        /// wavファイルからAudioClipに対応したフォーマットで情報を生成
        /// </summary>
        public static WaveAudioClip CreateWavAudioClip(string path)
        {
            var clip = new WaveAudioClip();
            var wav = new Data();
            wav.ReadWave(Resources.Load<TextAsset>(path).bytes);
            clip.samples = wav.data.Length;
            clip.channels = wav.header.Channel;
            clip.frequency = wav.header.SampleRate;            
            clip.waveData = new float[wav.data.Length];
            // 16bitデータを-1～1に変換
            for (int i = 0; i < wav.data.Length; ++i)
            {
                clip.waveData[i] = (float)wav.data[i] / Int16.MaxValue;
            }
            return clip;            
        }
    }
}
