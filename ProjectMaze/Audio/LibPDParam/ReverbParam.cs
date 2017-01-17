using ProjectMaze.Audio.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ProjectMaze.Audio.LibPDParam
{
    public class ReverbParam : IParam
    {
        private string param_name = "reverb";
        private float param_value;

        public ReverbParam()
        {
            this.param_value = 0.0f;
        }

        public ReverbParam(float value)
        {
            this.param_value = value;
        }

        public string getParamName()
        {
            return param_name;
        }

        public float getFloatParam()
        {
            return param_value;
        }

        public void resetValue()
        {
            param_value = 0.0f;
        }

        public void setFloatParam(float value)
        {
            this.param_value = value;
        }
    }
}
