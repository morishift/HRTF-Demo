using System;
using System.IO;
using System.Windows;

namespace Test
{
    /// <summary>
    /// WAVEヘッダ
    /// </summary>
    public struct Header
    {
        public string RiffHeader;
        public int FileSize;
        public string WaveHeader;
        public string FormatChunk;
        public int FormatChunkSize;
        public short FormatID;
        public short Channel;
        public int SampleRate;
        public int BytePerSec;
        public short BlockSize;
        public short BitPerSample;
        public string DataChunk;
        public int DataChunkSize;
        public int PlayTimeMsec;
    }

    /// <summary>
    /// WAVE 読み込みクラス
    /// </summary>
    public class Data
    {
        public Header header = new Header();
        public Int16[] data
        {
            get;
            private set;
        }

        /// <summary>
        /// bytesから読み込み
        /// </summary>
        public bool ReadWave(byte[] bytes)
        {
            using (var stream = new MemoryStream(bytes))
            {
                return ReadWave(stream);
            }
        }

        /// <summary>WAVE 読み込み</summary>
        /// <param name="waveFilePath">Wave ファイルへのパス</param>
        /// <returns>読み込み結果</returns>
        /// <remarks>fmt チャンクおよび data チャンク以外は読み飛ばします</remarks>
        public bool ReadWave(Stream fs)
        {
            try
            {
                var br = new BinaryReader(fs);
                header.RiffHeader = System.Text.Encoding.GetEncoding(20127).GetString(br.ReadBytes(4));
                header.FileSize = BitConverter.ToInt32(br.ReadBytes(4), 0);
                header.WaveHeader = System.Text.Encoding.GetEncoding(20127).GetString(br.ReadBytes(4));

                var readFmtChunk = false;
                var readDataChunk = false;
                while (readFmtChunk == false || readDataChunk == false)
                {
                    // ChunkIDを取得する
                    var chunk = System.Text.Encoding.GetEncoding(20127).GetString(br.ReadBytes(4));
                    if (chunk.ToLower().CompareTo("fmt ") == 0)
                    {
                        // fmtチャンクの読み込み
                        header.FormatChunk = chunk;
                        header.FormatChunkSize = BitConverter.ToInt32(br.ReadBytes(4), 0);
                        header.FormatID = BitConverter.ToInt16(br.ReadBytes(2), 0);
                        header.Channel = BitConverter.ToInt16(br.ReadBytes(2), 0);
                        header.SampleRate = BitConverter.ToInt32(br.ReadBytes(4), 0);
                        header.BytePerSec = BitConverter.ToInt32(br.ReadBytes(4), 0);
                        header.BlockSize = BitConverter.ToInt16(br.ReadBytes(2), 0);
                        header.BitPerSample = BitConverter.ToInt16(br.ReadBytes(2), 0);
                        readFmtChunk = true;
                    }
                    else if (chunk.ToLower().CompareTo("data") == 0)
                    {
                        // dataチャンクの読み込み
                        header.DataChunk = chunk;
                        header.DataChunkSize = BitConverter.ToInt32(br.ReadBytes(4), 0);
                        byte[] b = br.ReadBytes(header.DataChunkSize);

                        // バッファに読み込み
                        // Note: L/Rに分けたい場合にはこの辺で分割する
                        data = new Int16[header.DataChunkSize / 2];
                        var insertIndex = 0;
                        for (int i = 0; i < b.Length; i += 2)
                        {
                            data[insertIndex] = BitConverter.ToInt16(b, i);
                            ++insertIndex;
                        }

                        // 再生時間を算出する
                        // Note: 使うことが多いのでついでに算出しておく
                        var bytesPerSec = header.SampleRate * header.Channel * header.BlockSize;
                        header.PlayTimeMsec = (int)(((double)header.DataChunkSize / (double)bytesPerSec) * 1000);
                        readDataChunk = true;
                    }
                    else
                    {
                        // 不要なチャンクの読み捨て
                        Int32 size = BitConverter.ToInt32(br.ReadBytes(4), 0);
                        if (0 < size)
                        {
                            br.ReadBytes(size);
                        }
                    }
                }
            }
            catch
            {
                return false;
            }

            return true;
        }
    }
}

