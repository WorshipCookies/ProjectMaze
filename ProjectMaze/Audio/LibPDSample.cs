using ProjectMaze.Audio.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LibPDBinding;
using System.IO;

namespace ProjectMaze.Audio
{
    public class LibPDSample : ILibPD
    {
        private string patch_name = Directory.GetCurrentDirectory() + "\\patches\\volume.pd";
        private int patchID;
        private Dictionary<string, IParam> param_list;

        private static LibPDSample instance;

        public static void initializeLibPD()
        {
            instance = new LibPDSample();
        }

        public static LibPDSample getInstance()
        {
            if (instance == null)
            {
                return new LibPDSample();
            }
            else
            {
                return instance;
            }
        }

        private LibPDSample()
        {
            // LibPD Initialization
            LibPD.OpenAudio(2, 2, 44100);
            patchID = LibPD.OpenPatch(patch_name);
            LibPD.ComputeAudio(true);
            param_list = new Dictionary<string, IParam>();
        }

        public void initLibPD()
        {
            LibPD.ReInit();

            instance = new LibPDSample();
        }

        public void resetAllParam()
        {
            foreach (IParam p in param_list.Values)
            {
                p.resetValue();
                LibPD.SendFloat(p.getParamName(), p.getFloatParam());
            }
        }

        public void Dispose()
        {
            LibPD.ClosePatch(patchID);
        }

        // To be called before 
        public void changeParamValue(IParam p)
        {
            param_list[p.getParamName()] = p;
            LibPD.SendFloat(p.getParamName(), p.getFloatParam());
        }

        public void changeParamValue(string p)
        {
            IParam par = param_list[p];
            LibPD.SendFloat(par.getParamName(), par.getFloatParam());
        }

        public unsafe void processAudio(int ticks, float* inputBuffer, float* outputBuffer)
        {
            LibPD.Process(ticks, inputBuffer, outputBuffer);
        }

        public void addParam(IParam p)
        {
            param_list.Add(p.getParamName(), p);
        }

        public void removeParam(IParam p)
        {
            param_list.Remove(p.getParamName());
        }

        public IParam getParam(string param_name)
        {
            return param_list[param_name];
        }

        
    }
}
