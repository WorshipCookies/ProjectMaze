using LibPDBinding;
using NAudio.Wave;
using ProjectMaze.Audio.Interfaces;
using ProjectMaze.Visualization;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace ProjectMaze.Audio
{
    public class SoundPoint : WaveStream
    {

        private WaveFileReader wavRead;
        private WaveChannel32 FStream;

        public SoundPoint(int i)  : base()
        {
            if (i == 0)
            {
                this.wavRead = new WaveFileReader(Directory.GetCurrentDirectory() + "\\soundbank\\abandon.wav");
                FStream = new WaveChannel32(wavRead);
            }
            else
            {
                this.wavRead = new WaveFileReader(Directory.GetCurrentDirectory() + "\\soundbank\\mystery.wav");
                FStream = new WaveChannel32(wavRead);
            }
        }


        public int getID()
        {
            throw new NotImplementedException();
        }

        public Visual.Tile getPointPos()
        {
            throw new NotImplementedException();
        }

        public void setPointPos(Visual.Tile t)
        {
            throw new NotImplementedException();
        }

        public Dictionary<string, IParam> getLibPDParam()
        {
            throw new NotImplementedException();
        }

        public void setLibPDParam(Dictionary<string, IParam> paramDictionary)
        {
            throw new NotImplementedException();
        }

        public void addLibPDParam(IParam param)
        {
            throw new NotImplementedException();
        }

        public IParam getParam(string paramName)
        {
            throw new NotImplementedException();
        }

        public void updateParam(string paramName, float value)
        {
            throw new NotImplementedException();
        }

        public bool playSample()
        {
            throw new NotImplementedException();
        }

        public void setPlay(bool to_play)
        {
            throw new NotImplementedException();
        }

        public void setWavFile(string filePath)
        {
            throw new NotImplementedException();
        }

        public string getWavFile()
        {
            throw new NotImplementedException();
        }

        public override unsafe int Read(byte[] buffer, int offset, int sampleCount)
        {
            LibPD.SendFloat("param", GameVisualizer.volume);
            int bytesRead = FStream.Read(buffer, offset, sampleCount);

            var ticks = (sampleCount / (sizeof(float) * WaveFormat.Channels)) / LibPD.BlockSize;

            fixed (byte* outBuff = buffer)
            {
                var output = (float*)outBuff;
                LibPD.Process(ticks, output, output);
            }

            //return sampleCount;
            return bytesRead;
        }

        public WaveChannel32 getStream()
        {
            return FStream;
        }

        public override NAudio.Wave.WaveFormat WaveFormat
        {
            get { return WaveFormat.CreateIeeeFloatWaveFormat(44100, 2); }
        }

        public void Dispose()
        {
            wavRead = new WaveFileReader(Directory.GetCurrentDirectory() + "\\soundbank\\abandon.wav");
        }

        public override long Length
        {
            get { return wavRead.Length; }
        }

        /// <summary>
        /// LoopStream simply passes on positioning to source stream
        /// </summary>
        public override long Position
        {
            get { return wavRead.Position; }
            set { wavRead.Position = value; }
        }

    }
}
