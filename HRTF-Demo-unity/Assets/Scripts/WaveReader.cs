
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using System.IO;
using System.Text;

/// <summary>
/// RIFFファイルフォーマット
/// http://www.umekkii.jp/data/computer/file_format/riff.cgi
/// 
/// Parsing a WAV file in C
/// http://truelogic.org/wordpress/2015/09/04/parsing-a-wav-file-in-c/
/// </summary>
namespace Test
{
    public class WaveRIFF
    {
        public string riff;
        public UInt32 overallSize;
        public string wave;
        public string fmtChunkMarker;
        public UInt32 lengthOfFmt;
        public UInt16 formatType;
        public UInt16 channels;
        public UInt32 sampleRate;
        public UInt32 byteRate;
        public UInt16 blockAlign;
        public UInt16 bitsPerSample;
        public string dataChunkHeader;
        public UInt32 dataSize;
        public Int16[] data;
    }

    public static class WaveReader
    {
        public static WaveRIFF Load(byte[] binary)
        {
            WaveRIFF w = new WaveRIFF();
            using (var ms = new MemoryStream(binary))
            using (var br = new BinaryReader(ms))
            {
                w.riff = br.ReadUTF8String(4);
                if (w.riff != "RIFF")
                    return null;
                //Debug.Log($"w.riff:{w.riff}");
                w.overallSize = br.ReadUInt32();
                //Debug.Log($"w.overallSize:{w.overallSize}");
                w.wave = br.ReadUTF8String(4);
                //Debug.Log($"w.wave:{w.wave}");
                while (br.EOF() == false)
                {
                    string chunk_marker = br.ReadUTF8String(4);
                    //Debug.Log($"chunk_marker:{chunk_marker}");
                    UInt32 chunk_length = br.ReadUInt32();
                    //Debug.Log($"chunk_length:{chunk_length}");
                    if (chunk_marker == "fmt ")
                    {
                        LoadFmtChunk(w, br, chunk_marker, chunk_length);
                    }
                    else if (chunk_marker == "data")
                    {
                        LoadDataChunk(w, br, chunk_marker, chunk_length);
                    }
                    else
                    {
                        //Debug.Log($"IgnoreChunk marker:{chunk_marker} length:{chunk_length}");
                        br.ReadBytes((int)chunk_length);
                    }
                }
            }
            return w;
        }

        /// <summary>
        /// Fmt Chunk
        /// </summary>
        private static void LoadFmtChunk(WaveRIFF w, BinaryReader br, string fmt, UInt32 sz)
        {
            w.fmtChunkMarker = fmt;
            w.lengthOfFmt = sz;
            w.formatType = br.ReadUInt16();
            //Debug.Log($"w.formatType:{w.formatType}");
            w.channels = br.ReadUInt16();
            //Debug.Log($"w.channels:{w.channels}");
            w.sampleRate = br.ReadUInt32();
            //Debug.Log($"w.sampleRate:{w.sampleRate}");
            w.byteRate = br.ReadUInt32();
            //Debug.Log($"w.byteRate:{w.byteRate}");
            w.blockAlign = br.ReadUInt16();
            //Debug.Log($"w.blockAlign:{w.blockAlign}");
            w.bitsPerSample = br.ReadUInt16();
            //Debug.Log($"w.bitsPerSample:{w.bitsPerSample}");
        }


        /// <summary>
        /// Data Chunk
        /// </summary>
        private static void LoadDataChunk(WaveRIFF w, BinaryReader br, string fmt, UInt32 sz)
        {
            w.dataChunkHeader = fmt;
            w.dataSize = sz;
            Debug.Assert(w.bitsPerSample == 16);
            UInt32 sample_count = w.dataSize / 2;
            w.data = new Int16[sample_count];
            for (int i = 0; i < sample_count; ++i)
            {
                w.data[i] = br.ReadInt16();
            }            
        }

        /// <summary>
        /// 指定された文字数をbrから読む
        /// </summary>
        private static string ReadUTF8String(this BinaryReader br, int n)
        {
            return Encoding.UTF8.GetString(br.ReadBytes(n));
        }

        /// <summary>
        /// brが終端に達していればtrue
        /// </summary>
        private static bool EOF(this BinaryReader br)
        {
            return (br.BaseStream.Position == br.BaseStream.Length);
        }
    }
}


