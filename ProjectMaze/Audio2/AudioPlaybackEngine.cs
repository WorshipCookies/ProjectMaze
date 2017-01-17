using LibPDBinding;
using NAudio.CoreAudioApi;
using NAudio.Wave;
using NAudio.Wave.SampleProviders;
using ProjectMaze.Visualization;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace ProjectMaze.Audio2
{
    public class AudioPlaybackEngine : IDisposable
    {
        private MixingWaveProvider32 mixer;
        private AsioOut FDriver;
        private string patch_name = Directory.GetCurrentDirectory() + "\\patches\\volume.pd";


        public AudioPlaybackEngine(int sampleRate = 44100, int channelCount = 2)
        {
            FDriver = new AsioOut();
            mixer = new MixingWaveProvider32();
            LibPD.OpenAudio(2, 2, 44100);
            LibPD.OpenPatch(this.patch_name);
            LibPD.ComputeAudio(true);

            //LibPD.SendFloat("param", GameVisualizer.volume);

            FDriver.Init(mixer);
            FDriver.Play();
        }

        public void PlaySound(string fileName)
        {
            SoundStream input = new SoundStream(new LoopStream(new CacheSound(fileName))); // Temp Test File.
            AddMixerInput(new AutoDisposeFileReader(input));
        }

        public void AddMixerInput(WaveStream stream)
        {
            mixer.AddInputStream(stream);
            //FDriver.Play();
        }


        public void Dispose()
        {
            FDriver.Dispose();
        }

        public static readonly AudioPlaybackEngine Instance = new AudioPlaybackEngine(44100, 2);
    }
}
