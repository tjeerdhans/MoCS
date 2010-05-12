using System;
using System.Data;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Xml.Linq;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

namespace MoCS.Service
{
    /// <summary>
    /// Extension methods for cloning objects
    /// </summary>
    public static class CloneHelpers
    {
        public static object DeepClone(this object source)
        {
            MemoryStream memoryStream = new MemoryStream();
            BinaryFormatter binaryFormatter = new BinaryFormatter();
            binaryFormatter.Serialize(memoryStream, source);
            memoryStream.Position = 0;
            return binaryFormatter.Deserialize(memoryStream);

        }
        public static bool DeepEquals(this object objectA, object objectB)
        {
            MemoryStream memoryStreamA = serializedStream(objectA);
            MemoryStream memoryStreamB = serializedStream(objectB);
            if (memoryStreamA.Length != memoryStreamA.Length)
                return false;
            while (memoryStreamA.Position < memoryStreamA.Length)
            {
                if (memoryStreamA.ReadByte() != memoryStreamB.ReadByte())
                    return false;
            }
            return true;

        }
        public static MemoryStream serializedStream(this object source)
        {
            MemoryStream memoryStream = new MemoryStream();
            BinaryFormatter binaryFormatter = new BinaryFormatter();
            binaryFormatter.Serialize(memoryStream, source);
            memoryStream.Position = 0;

            return memoryStream;
        }
    }
}
