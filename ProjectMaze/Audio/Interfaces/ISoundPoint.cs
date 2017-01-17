using NAudio.Wave;
using ProjectMaze.Visual;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ProjectMaze.Audio.Interfaces
{
    public interface ISoundPoint : IWaveProvider, IDisposable
    {

        int getID();

        Tile getPointPos();

        void setPointPos(Tile t);

        Dictionary<string, IParam> getLibPDParam();

        void setLibPDParam(Dictionary<string, IParam> paramDictionary);

        void addLibPDParam(IParam param);

        IParam getParam(string paramName);

        void updateParam(string paramName, float value);

        // Important -- Calculate if this sample is to be played!
        bool playSample();

        void setPlay(bool to_play);

        void setWavFile(string filePath);

        string getWavFile();

    }
}
