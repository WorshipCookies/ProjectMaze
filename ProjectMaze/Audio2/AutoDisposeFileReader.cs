using NAudio.Wave;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ProjectMaze.Audio2
{
    public class AutoDisposeFileReader: WaveStream
    {
        private readonly SoundStream reader;
        private bool isDisposed;

        public AutoDisposeFileReader(SoundStream reader)
        {
            this.reader = reader;
        }

        public override WaveFormat WaveFormat
        {
            get { return reader.WaveFormat; }
        }

        public override unsafe int Read(byte[] buffer, int offset, int sampleCount)
        {
            if (isDisposed)
                return 0;
            int read = reader.Read(buffer, offset, sampleCount);
            if (read == 0)
            {
                reader.Dispose();
                isDisposed = true;
            }
            return read;
        }

        public override long Length
        {
            get { return reader.Length; }
        }

        public override long Position
        {
            get { return reader.Position; }
            set { reader.Position = value; }
        }
    }
}
