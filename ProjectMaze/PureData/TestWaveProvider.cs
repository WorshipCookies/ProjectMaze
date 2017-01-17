using LibPDBinding;
using NAudio.Wave;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ProjectMaze.PureData
{
    public class TestWaveProvider : IWaveProvider, IDisposable
    {
        private WaveFileReader reader;
        private WaveChannel32 FStream;
        private AsioOut FDriver;
        private MixingWaveProvider32 mixer;

        //private WaveOut FDriver;

        private string patch_name;
        private string wav_name;

        private float param;

        private bool isPlaying;

        public TestWaveProvider(string patch_name, string wav_name)
        {
            this.patch_name = patch_name;
            this.wav_name = wav_name;
            this.isPlaying = true;
        }

        public unsafe int Read(byte[] buffer, int offset, int sampleCount)
        {

            int bytesRead = FStream.Read(buffer, offset, sampleCount);

            var ticks = (sampleCount / (sizeof(float) * WaveFormat.Channels)) / LibPD.BlockSize;

            fixed (byte* outBuff = buffer)
            {
                var output = (float*)outBuff;
                LibPD.Process(ticks, output, output);
            }

            bool hasAllZeroes = buffer.All(singleByte => singleByte == 0);

            if (hasAllZeroes)
            {
                this.isPlaying = false;
            }
            else
            {
                this.isPlaying = true;
            }

            //return sampleCount;
            return bytesRead;
        }

        public WaveFormat WaveFormat
        {
            get
            {
                return WaveFormat.CreateIeeeFloatWaveFormat(44100, 2);
            }
        }

        public void Play()
        {
            if (FDriver != null)
            {
                Dispose();
            }

            FDriver = new AsioOut();
            reader = new WaveFileReader(wav_name);
            FStream = new WaveChannel32(reader);
            mixer = new MixingWaveProvider32();
            mixer.AddInputStream(this);
            //FStream.PadWithZeroes = false;

            FDriver.PlaybackStopped += new EventHandler<StoppedEventArgs>(audioOutput_PlaybackStopped);


            LibPD.OpenAudio(2, 2, 44100);
            LibPD.OpenPatch(patch_name);
            LibPD.ComputeAudio(true);

            //FDriver.Init(this);
            FDriver.Init(mixer);

            incParam();
            FDriver.Play();
        }

        public void changeWav(string new_wav)
        {
            this.wav_name = new_wav;
            reader = new WaveFileReader(wav_name);
            FStream = new WaveChannel32(reader);
            mixer.AddInputStream(this);
            FDriver.Play();
        }

        public bool ifPlaying()
        {
            return isPlaying;
        }

        public void Dispose()
        {
            LibPD.ReInit();
            FDriver.Dispose();
        }

        public void incParam()
        {
            this.param += 0.1f;
            LibPD.SendFloat("param", this.param);
        }

        public void decParam()
        {
            //if (this.param > 0.05f)
            //{
                this.param -= 0.1f;
                LibPD.SendFloat("param", this.param);
            //}
        }

        public void audioOutput_PlaybackStopped(object sender, StoppedEventArgs e)
        {
            Dispose();
            Console.WriteLine("IT HAS STOPPED!");
        }
    }
}
