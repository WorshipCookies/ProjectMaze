using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ProjectMaze.Audio2
{
    public class BaseSoundInfo : ISoundInfo
    {
        private string fileName;
        private int id;
        private double globalRankValue;

        public BaseSoundInfo(string fileName, int id)
        {
            this.fileName = fileName;
            this.id = id;
        }

        public string getFileName()
        {
            return fileName;
        }

        public int getID()
        {
            return id;
        }

        public void setGlobalRankValue(double globalRankValue)
        {
            this.globalRankValue = globalRankValue;
        }

        public double getGlobalRankValue()
        {
            return globalRankValue;
        }

    }
}
