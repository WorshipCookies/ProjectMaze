using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ProjectMaze.Audio2
{
    public class AudioFileObject
    {
        private string filename;
        private int instrument_ID;
        private int octave;
        private string note;
        private int effectFamily;
        private double value; // This value is just a test of what kind of information we will possibly attach to music in the future.

        public AudioFileObject(string path, string file) 
        {
            this.filename = path + "\\" + file;
            char[] splitter1 = { '.' };
            char[] splitter2 = { '_' };
            string[] parameters = (file.Split(splitter1))[0].Split(splitter2);

            this.instrument_ID = Convert.ToInt32(parameters[0]);
            this.octave = Convert.ToInt32(parameters[1]);
            this.note = parameters[2];
            this.value = Convert.ToDouble(parameters[3]);  
        }

        public AudioFileObject(string path, ISoundInfo sound)
        {
            this.filename = path + "\\" + sound.getFileName();

            if(sound.GetType() == typeof(BaseSoundInfo))
            {
                char[] splitter1 = { '.' };
                char[] splitter2 = { '_' };
                string[] parameters = (sound.getFileName().Split(splitter1))[0].Split(splitter2);

                this.instrument_ID = Convert.ToInt32(parameters[0]);
                this.octave = Convert.ToInt32(parameters[1]);
                this.note = parameters[2];
                this.effectFamily = 0;
            }
            else if(sound.GetType() == typeof(EffectInfo))
            {
                char[] splitter1 = { '.' };
                char[] splitter2 = { '_' };
                string[] parameters = (sound.getFileName().Split(splitter1))[0].Split(splitter2);

                this.instrument_ID = Convert.ToInt32(parameters[0]);
                this.octave = Convert.ToInt32(parameters[1]);
                this.note = parameters[2];
                this.effectFamily = ((EffectInfo)sound).getEffectFamily();
            }
            this.value = sound.getGlobalRankValue();
        }

        public string getFileName()
        {
            return this.filename;
        }

        public int getInstrumentID()
        {
            return this.instrument_ID;
        }

        public int getOctave()
        {
            return this.octave;
        }

        public string getNote()
        {
            return this.note;
        }

        public double getValue()
        {
            return this.value;
        }

        public override bool Equals(object obj)
        {
            if (obj is AudioFileObject)
            {
                AudioFileObject o = (AudioFileObject)obj;
                if (o.getFileName() == this.getFileName())
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            else
            {
                return false;
            }
        }

    }
}
