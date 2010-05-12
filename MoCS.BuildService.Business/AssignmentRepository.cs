using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Xml;

namespace MoCS.BuildService.Business
{
    public class AssignmentRepository
    {

        private string _basePath;
        public AssignmentRepository(string basePath)
        {
            _basePath = basePath;
        }

        public Assignment ReadAssignment(string id)
        {
            string path = Path.Combine(_basePath, id);
            path = Path.Combine(path, "assignment.xml");

            XmlDocument doc = new XmlDocument();
            doc.Load(path);

            Assignment assignment = ReadAssignment(doc);
            return assignment;
           
        }

        public Assignment ReadAssignment(XmlDocument doc)
        {
            Assignment assignment = new Assignment();
            assignment.InterfaceFile = GetNodeValue(doc, "/Assignment/Files/InterfaceFile");
            assignment.NunitTestFileServer = GetNodeValue(doc, "/Assignment/Files/NunitTestFileServer");
            assignment.NunitTestFileClient = GetNodeValue(doc, "/Assignment/Files/NunitTestFileClient");
            assignment.ClassNameToImplement = GetNodeValue(doc, "/Assignment/Rules/ClassNameToImplement");
            assignment.InterfaceNameToImplement = GetNodeValue(doc, "/Assignment/Rules/InterfaceNameToImplement");
            assignment.ServerFilesToCopy = new List<string>();
            XmlNodeList nodes = doc.SelectNodes("/Assignment/Files/ServerFileToCopy");
            foreach (XmlNode node in nodes)
            {
                assignment.ServerFilesToCopy.Add(node.InnerText);
            }
            
            
            return assignment;
        }

        private string GetNodeValue(XmlNode node, string path)
        {
            XmlNode n = node.SelectSingleNode(path);
            if (n != null)
            {
                return n.InnerText; 
            }
            return "";
        }

    }
}
