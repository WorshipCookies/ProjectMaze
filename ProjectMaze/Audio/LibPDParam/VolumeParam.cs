using ProjectMaze.Audio.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ProjectMaze.Audio.LibPDParam
{
    public class VolumeParam : IParam
    {
        private string param_name = "volume";
        private float param_value;

        public VolumeParam()
        {
            this.param_value = 0.0f;
        }

        public string getParamName()
        {
            return this.param_name;
        }

        public float getFloatParam()
        {
            return this.param_value;
        }

        public void setFloatParam(float value)
        {
            this.param_value = value;
        }

        public void resetValue()
        {
            this.param_value = 0.0f;
        }
    }
}
