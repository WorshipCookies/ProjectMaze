using LibPDBinding;
using NAudio.Wave;
using ProjectMaze.Audio.Interfaces;
using ProjectMaze.Audio.LibPDParam;
using ProjectMaze.Visual;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ProjectMaze.Audio
{
    public class RoomSoundPoint : ISoundPoint
    {

        private static int ID_Counter = 0;

        private WaveFileReader wav;
        private WaveChannel32 FStream;


        private int ID;
        private Tile tile;
        private Dictionary<string, IParam> parameters;
        private string path_name;
        private bool to_play;

        public static RoomSoundPoint RoomSoundPointFactory(float room_size, string path_name, Tile t)
        {
            // For now, lets assume the intensity is the volume of 
            RoomSoundPoint rsp = new RoomSoundPoint(ID_Counter, t, path_name, room_size);
            ID_Counter++;
            return rsp;
        }

        private RoomSoundPoint(int ID, Tile t, string path_name, float room_size)
        {
            this.ID = ID;
            this.tile = t;

            this.to_play = false;
            this.path_name = path_name;

            wav = new WaveFileReader(path_name);
            FStream = new WaveChannel32(wav);

            // For this particular Point we only have 2 parameters --> Reverb + Volume
            parameters = new Dictionary<string, IParam>();
            ReverbParam rp = new ReverbParam();
            parameters.Add(rp.getParamName(), rp);
            VolumeParam vp = new VolumeParam();
            parameters.Add(vp.getParamName(), vp);
        }

        public int getID()
        {
            return this.ID;
        }

        public Tile getPointPos()
        {
            return tile;
        }

        public void setPointPos(Tile t)
        {
            this.tile = t;
        }

        public Dictionary<string, IParam> getLibPDParam()
        {
            return parameters;
        }

        public void setLibPDParam(Dictionary<string, IParam> paramDictionary)
        {
            this.parameters = paramDictionary;
        }

        public void addLibPDParam(IParam param)
        {
            parameters.Add(param.getParamName(), param);
        }

        public IParam getParam(string paramName)
        {
            return parameters[paramName];
        }

        public void updateParam(string paramName, float value)
        {
            parameters[paramName].setFloatParam(value);
        }

        public bool playSample()
        {
            return to_play;
        }

        public void setPlay(bool to_play)
        {
            this.to_play = to_play;
        }

        public void setWavFile(string filePath)
        {
            this.path_name = filePath;
        }

        //public unsafe int Read(float[] buffer, int offset, int count)
        //{
        //    //if (playSample())
        //    //{
        //        // Always reset the parameters if your are going to play on your turn!
        //        //LibPDSample.getInstance().resetAllParam();

        //        // For each parameter in this soundpoint modify it in libPD
        //        //foreach (IParam p in parameters.Values)
        //        //{
        //            //LibPDSample.getInstance().changeParamValue(p);
        //        //}

        //        int bytesRead = wav.Read(buffer, offset, count);

        //        var ticks = (count / (sizeof(float) * WaveFormat.Channels)) / LibPD.BlockSize;

        //        LibPD.Process(ticks, buffer, buffer);
                
        //        //fixed (float* outBuff = buffer)
        //        //{
        //            //var output = (float*)outBuff;
        //            //test = LibPD.Process(ticks, output, output);
        //        //}

        //        //return sampleCount;
        //        return bytesRead;
        //    //}
        //    //else
        //    //{
        //      //  return 0; // Don't play!
        //    //}
        //}

        public WaveFormat WaveFormat
        {
            get { return WaveFormat.CreateIeeeFloatWaveFormat(44100, 2); }
        }

        public string getWavFile()
        {
            return path_name;
        }

        public unsafe int Read(byte[] buffer, int offset, int sampleCount)
        {

            //LibPD.SendFloat("volume", 50f);

            int bytesRead = FStream.Read(buffer, offset, sampleCount);

            var ticks = (sampleCount / (sizeof(float) * WaveFormat.Channels)) / LibPD.BlockSize;


            fixed (byte* outBuff = buffer)
            {
                var output = (float*)outBuff;
                LibPD.Process(ticks, output, output);
            }

            return bytesRead;
        }

        public void Dispose()
        {
            
        }
    }
}
