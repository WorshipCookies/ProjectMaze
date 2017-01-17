using NAudio.Wave;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace ProjectMaze.Audio2
{
    public class CacheSound : WaveStream
    {
        public byte[] AudioData { get; private set; }
        public WaveFileReader waveReader;

        public CacheSound(string audioFile)
        {
            waveReader = new WaveFileReader(audioFile);
            AudioData = SoundCacheUtil.ToArray(waveReader);
        }

        public override WaveFormat WaveFormat
        {
            get { return waveReader.WaveFormat; }
        }

        public override long Length
        {
            get { return waveReader.Length; }
        }

        public override long Position
        {
            get { return waveReader.Position; }
            set { waveReader.Position = value; }
        }

        public override int Read(byte[] buffer, int offset, int count)
        {
            var availableSamples = AudioData.Length - Position;
            var samplesToCopy = Math.Min(availableSamples, count);
            Array.Copy(AudioData, Position, buffer, offset, samplesToCopy);
            Position += samplesToCopy;
            return (int)samplesToCopy;
        }

    }
}
