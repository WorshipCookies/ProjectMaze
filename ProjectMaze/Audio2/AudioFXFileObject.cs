using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ProjectMaze.Audio2
{
    public class AudioFXFileObject
    {
        private string filename;
        private int fx_ID;
        private int value;

        public AudioFXFileObject(string path, string file)
        {
            this.filename = path + "\\" + file;
            char[] splitter1 = { '.' };
            char[] splitter2 = { '_' };
            string[] parameters = (file.Split(splitter1))[0].Split(splitter2);

            this.fx_ID = Convert.ToInt32(parameters[0]);
            this.value = Convert.ToInt32(parameters[1]);
        }

        public string getFileName()
        {
            return this.filename;
        }

        public int getID()
        {
            return this.fx_ID;
        }

        public int getValue()
        {
            return this.value;
        }

    }
}
