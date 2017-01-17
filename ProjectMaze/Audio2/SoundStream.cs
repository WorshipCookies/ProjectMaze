using LibPDBinding;
using NAudio.Wave;
using ProjectMaze.Visualization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ProjectMaze.Audio2
{
    public class SoundStream : WaveStream
    {
        WaveChannel32 FStream;
        WaveStream source;

        private float volume;
        private string fileName;

        private double intensity;

        public SoundStream(WaveStream source)
        {
            this.source = source;
            FStream = new WaveChannel32(source);
            this.volume = 0.05f;
        }

        // With Cache or Not.
        public SoundStream(string fileName, bool cache, double intensity)
        {
            this.fileName = fileName;
            this.intensity = intensity;

            Console.WriteLine("Sound " + fileName + " was loaded to Stream");
            if (cache)
                this.source = new LoopStream(new CacheSound(fileName));
            else
                this.source = new LoopStream(new WaveFileReader(fileName));
            FStream = new WaveChannel32(source);
            this.volume = 0.00f;
        }

        public void setVolume(float volume)
        {
            this.volume = volume;
        }

        public override WaveFormat WaveFormat
        {
            get { return WaveFormat.CreateIeeeFloatWaveFormat(44100, 2); }
        }

        public override unsafe int Read(byte[] buffer, int offset, int sampleCount)
        {
            LibPD.SendFloat("param", this.volume); // Volume Parameter.
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

        public override long Length
        {
            get { return source.Length; }
        }

        public override long Position
        {
            get { return source.Position; }
            set { source.Position = value; }
        }

        public float getVolume()
        {
            return volume;
        }

        public string getFileName()
        {
            return fileName;
        }

        public double getInstensity()
        {
            return this.intensity;
        }
    }
}
