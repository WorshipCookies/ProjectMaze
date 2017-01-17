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
    public class AudioMixer : IDisposable
    {
        private MixingWaveProvider32 Mixer;
        private AsioOut FDriver;
        private string patch_name = Directory.GetCurrentDirectory() + "\\patches\\volume.pd";

        public AudioMixer()
        {
            LibPD.OpenAudio(2, 2, 44100);
            LibPD.OpenPatch(patch_name);
            LibPD.ComputeAudio(true);

            LibPD.SendFloat("param", GameVisualizer.volume);

            Mixer = new MixingWaveProvider32();
            Mixer.AddInputStream(new SoundPoint(0));
            FDriver = new AsioOut();

            FDriver.Init(Mixer);
            FDriver.Play();
        }

        public void addSoundPoint(SoundPoint p)
        {
            Mixer.AddInputStream(p); // TEMP
            //FDriver.Play();
        }

        public void Play()
        {
            FDriver.Play();
        }

        public void Dispose()
        {
            LibPD.ReInit();
            Mixer = new MixingWaveProvider32();
            FDriver = new AsioOut();
        }
    }
}
