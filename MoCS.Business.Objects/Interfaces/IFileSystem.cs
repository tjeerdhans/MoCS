using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using System.IO;

namespace MoCS.Business.Objects.Interfaces
{
    public interface IFileSystem
    {
        bool FileExists(string path);
        XmlDocument LoadXml(string path);
        bool DirectoryExists(string path);
        void CreateDirectory(string path);
        Stream FileOpenWrite(string path);
        void FileDelete(string path);
        void FileCopy(string pathFrom, string pathTo);
     }
}