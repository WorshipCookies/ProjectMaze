using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ProjectMaze.Audio2
{
    public class EffectInfo : ISoundInfo
    {
        private string fileName;
        private int effectFamily;
        private int effectSoundID;
        private int baseSoundID;
        private double globalRankValue;

        public EffectInfo(string fileName, int effectFamily, int effectSoundID, int baseSoundID)
        {
            this.fileName = fileName;
            this.effectFamily = effectFamily;
            this.baseSoundID = baseSoundID;
            this.effectSoundID = effectSoundID;
            this.globalRankValue = 0.0;
        }

        public string getFileName()
        {
            return fileName;
        }

        public int getEffectFamily()
        {
            return effectFamily;
        }

        public int getBaseSoundID()
        {
            return baseSoundID;
        }

        public void setGlobalRankValue(double globalRankValue)
        {
            this.globalRankValue = globalRankValue;
        }

        public double getGlobalRankValue()
        {
            return globalRankValue;
        }

        public int getID()
        {
            return effectSoundID;
        }

    }
}
