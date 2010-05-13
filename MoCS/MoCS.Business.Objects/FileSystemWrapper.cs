using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Xml;
using MoCS.Business.Objects.Interfaces;
using System.Reflection;

namespace MoCS.Business.Objects
{
    public class FileSystemWrapper : IFileSystem
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


        //public bool FileExists(string path)
        //{
        //    return File.Exists(path);
        //}

        public XmlDocument LoadXml(string path)
        {
            XmlDocument doc = new XmlDocument();
            doc.Load(path);
            return doc;
        }

        public bool DirectoryExists(string path)
        {
            return Directory.Exists(path);
        }

        public void CreateDirectory(string path)
        {
            Directory.CreateDirectory(path);
        }

        public Stream FileOpenWrite(string path)
        {
            return File.OpenWrite(path);
        }

        public void FileDelete(string path)
        {
            File.Delete(path);
        }

        public void FileCopy(string pathFrom, string pathTo)
        {
            FileCopy(pathFrom, pathTo, false);
        }

        public void FileCopy(string pathFrom, string pathTo, bool overwrite)
        {
            File.Copy(pathFrom, pathTo, overwrite);
        }


        public void DeleteFileIfExists(string path)
        {
            if (FileExists(path))
            {
                FileDelete(path);
            }
        }


        public void CreateDirectoryIfNotExists(string path)
        {
            if (DirectoryExists(path))
            {
                CreateDirectory(path);
            }
        }



    }
}