using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ProjectMaze.Audio.Interfaces
{
    public interface IParam
    {
        // Name of the Parameter in LibPD -- Patch must have r:paramname to work!
        string getParamName();

        // The parameter value to be changed
        float getFloatParam();

        void setFloatParam(float value);

        void resetValue();

    }
}
