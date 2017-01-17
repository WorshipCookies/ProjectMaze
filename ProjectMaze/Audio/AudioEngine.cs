using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using LibPDBinding;
using NAudio.Wave;
using NAudio.Wave.SampleProviders;

namespace ProjectMaze.Audio
{
    public class AudioEngine
    {
        // Singleton Instance of AudioEngine
        private static AudioEngine instance;

        private WaveOut FDriver;

        private AudioEngine()
        {
            FDriver = new WaveOut();
        }

        public static void initializeEngine()
        {
            getInstance();
        }

        // Singleton Creation Method
        public static AudioEngine getInstance()
        {
            if (instance == null)
            {
                instance = new AudioEngine();
                return instance;
            }
            else
            {
                return instance;
            }
        }

        public WaveOut getDriver()
        {
            return FDriver;
        }

        public void Play(MixingWaveProvider32 mixer)
        {
            FDriver.Init(mixer);
            FDriver.Play();
        }

        public void Stop()
        {
            FDriver.Stop();
            FDriver.Dispose();
            LibPDSample.getInstance().initLibPD();

            FDriver = new WaveOut();
        }

        public void Dispose()
        {
            this.FDriver.Dispose();
            instance = null;
        }
    }
}
