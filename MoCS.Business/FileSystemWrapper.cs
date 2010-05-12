using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Xml;

namespace MoCS.Business
{
    public class FileSystemWrapper : IFileSystem
    {

        public bool FileExists(string path)
        {
            return File.Exists(path);
        }

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
            File.Copy(pathFrom, pathTo);
        }


    }
}