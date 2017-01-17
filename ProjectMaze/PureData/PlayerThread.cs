using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;

namespace ProjectMaze.PureData
{
    public class PlayerThread
    {
        
        public static void playSomething()
        {
            //string patch_name = @"C:\Users\Phil\Desktop\NAudioPDLoader\exp.pd";
            string patch_name = Directory.GetCurrentDirectory() + "\\patches\\volume.pd";
            string wave_name = Directory.GetCurrentDirectory() + "\\soundbank\\abandon.wav";
            string new_wave = Directory.GetCurrentDirectory() + "\\soundbank\\abandon-001.wav";
            // Initialiazation goes here

            Console.WriteLine("Creating Object");
            TestWaveProvider tw = new TestWaveProvider(patch_name, wave_name);
            
            

            Console.WriteLine("Playing Sound");
            tw.Play();

            while (tw.ifPlaying())
            {
                Console.WriteLine(tw.ifPlaying());
                if (Console.ReadKey().Key == ConsoleKey.A)
                {
                    tw.changeWav(new_wave);
                }
                if (Console.ReadKey().Key == ConsoleKey.Q)
                {
                    tw.changeWav(wave_name);
                }
                if (Console.ReadKey().Key == ConsoleKey.X)
                {
                    tw.incParam();
                }
                if (Console.ReadKey().Key == ConsoleKey.Z)
                {
                    tw.decParam();
                }
            }

            tw.Dispose();

            Console.WriteLine("End of Tests.");
            Console.ReadKey();
            tw.Dispose();
        }

    }
}
