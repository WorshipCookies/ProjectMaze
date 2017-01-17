using NAudio.Wave;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ProjectMaze.Audio.Interfaces
{
    // It's important that this ISampleProvider be a Singleton -- LibPD is Singleton so this is important!!
    public interface ILibPD
    {
        
        void initLibPD();

        void resetAllParam();

        IParam getParam(string param_name);

        void changeParamValue(IParam p);

        void addParam(IParam p);

        void removeParam(IParam p);

        void Dispose();

    }
}
