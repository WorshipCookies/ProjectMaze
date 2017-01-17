using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace ProjectMaze.Audio2
{
    public static class SoundCacheUtil
    {

        public static byte[] ToArray(this Stream stream)
        {
            byte[] buffer = new byte[stream.Length];
            int reader = 0;
            MemoryStream memoryStr = new MemoryStream();

            while ((reader = stream.Read(buffer, 0, buffer.Length)) != 0)
                memoryStr.Write(buffer, 0, reader);

            return memoryStr.ToArray();
        }
    }
}
