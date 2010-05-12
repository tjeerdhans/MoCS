using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using System.Xml;

namespace MoCS.BuildService.Business
{
    public interface IFileSystem
    {
        void WriteToFile(string contents, string filePath);
        bool FileExists(string filePath);
        Assembly LoadAssembly(string filePath);
        XmlDocument LoadXmlDocument(string testlogPath);
        List<string> ReadErrorsFromBuildLog(string outputLogPath);
    }
}
