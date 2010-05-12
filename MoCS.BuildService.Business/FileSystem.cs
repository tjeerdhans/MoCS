using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Reflection;
using System.Xml;

namespace MoCS.BuildService.Business
{
    public class FileSystem : IFileSystem
    {
        public void WriteToFile(string contents, string filePath)
        {
            using (FileStream batchFile = File.OpenWrite(filePath))
            {
                StreamWriter w = new StreamWriter(batchFile);
                w.Write(contents);
                w.WriteLine();
                w.Flush();
                w.Close();
            }
        }

        public bool FileExists(string filePath)
        {
            return File.Exists(filePath);
        }

        public Assembly LoadAssembly(string filePath)
        {
            return Assembly.LoadFile(filePath);
        }

        public XmlDocument LoadXmlDocument(string testlogPath)
        {
            XmlDocument doc = new XmlDocument();
            doc.Load(testlogPath);
            return doc;
        }

        public List<string> ReadErrorsFromBuildLog(string outputLogPath)
        {
            List<string> messages = new List<string>();
            using (FileStream stream = File.OpenRead(outputLogPath))
            {
                long size = stream.Length;
                if (size > 0)
                {
                    StreamReader r = new StreamReader(stream);
                    stream.Position = 0;
                    string line = r.ReadLine();
                    while (line != null)
                    {
                        messages.Add(line);
                        line = r.ReadLine();
                    }
                    r.Close();
                }
            }
            return messages;
        }
    }
}
