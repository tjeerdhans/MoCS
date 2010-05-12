using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using System.IO;
using System.Reflection;

namespace MoCS.Business.Objects.Interfaces
{
    public interface IFileSystem
    {
        void WriteToFile(string contents, string filePath);
        bool FileExists(string filePath);
        Assembly LoadAssembly(string filePath);
        XmlDocument LoadXmlDocument(string testlogPath);
        List<string> ReadErrorsFromBuildLog(string outputLogPath);

        // From MoCS.Business.Interfaces
        //bool FileExists(string path);
        XmlDocument LoadXml(string path);
        bool DirectoryExists(string path);
        void CreateDirectory(string path);
        Stream FileOpenWrite(string path);
        void FileDelete(string path);
        void FileCopy(string pathFrom, string pathTo);
     }
}