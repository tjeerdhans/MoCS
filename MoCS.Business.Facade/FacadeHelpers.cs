using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using MoCS.Business.Objects;

namespace MoCS.Business.Facade
{
    public static class FacadeHelpers
    {
        public static byte[] ReadByteArrayFromFile(string path)
        {
            FileStream fs = File.OpenRead(path);
            byte[] result = ConvertStreamToByteArray(fs);
            fs.Close();
            return result;
        }

        public static byte[] ConvertStreamToByteArray(Stream stream)
        {
            byte[] respBuffer = new byte[stream.Length];
            try
            {
                int bytesRead = stream.Read(respBuffer, 0, respBuffer.Length);
            }
            finally
            {
                stream.Close();
            }

            return respBuffer;
        }

       
    }
}
