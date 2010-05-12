using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Xml;
using MoCS.Business.Objects.Interfaces;
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

        public static Assignment FillAssignmentDetailsFromXml(Assignment a, IFileSystem fileSystem, bool includeServerFiles)
        {
            if (a == null)
            {
                return null;    //no active assignment
            }

            string path = Path.Combine(a.Path, "assignment.xml");

            if (fileSystem.FileExists(path))
            {
                XmlDocument doc = fileSystem.LoadXml(path);

                a.FriendlyName = GetNodeValue(doc, "Assignment/DisplayName");
                //a.Tagline = GetNodeValue(doc, "Assignment/Hint");

                a.Difficulty = int.Parse(GetNodeValue(doc, "Assignment/Difficulty"));
                a.Author = GetNodeValue(doc, "Assignment/Author");
                a.Category = GetNodeValue(doc, "Assignment/Category");

                a.InterfaceNameToImplement = GetNodeValue(doc, "Assignment/Rules/InterfaceNameToImplement");
                a.ClassNameToImplement = GetNodeValue(doc, "Assignment/Rules/ClassNameToImplement");

                //a.ClassFileName = GetNodeValue(doc, "Assignment/Files/ClassFile");
                //a.InterfaceFileName = GetNodeValue(doc, "Assignment/Files/InterfaceFile");

                //a.UnitTestClientFileName = GetNodeValue(doc, "Assignment/Files/NunitTestFileClient");
                //a.UnitTestServerFileName = GetNodeValue(doc, "Assignment/Files/NunitTestFileServer");

                //a.CaseFileName = GetNodeValue(doc, "Assignment/Files/Case");
                a.AssignmentFiles = new List<AssignmentFile>();
                XmlNode fileNode = doc.SelectSingleNode("Assignment/Files");
                foreach (XmlNode fileChildNode in fileNode.ChildNodes)
                {
                    string nodeName = fileChildNode.Name;
                    string fileName = fileChildNode.InnerText;

                    string filepath = Path.Combine(a.Path, fileName);
                    if (File.Exists(filepath))
                    {
                        if (includeServerFiles || (nodeName != "NunitTestFileServer" && nodeName != "ServerFileToCopy"))
                        {
                            AssignmentFile assignmentFile = new AssignmentFile();
                            assignmentFile.Name = nodeName;
                            assignmentFile.FileName = fileName;
                            assignmentFile.Data = FacadeHelpers.ReadByteArrayFromFile(filepath);
                            a.AssignmentFiles.Add(assignmentFile);
                        }
                    }
                }
            }
            else
            {
                throw new ApplicationException("Details for the assignment could not be found");
            }
            return a;

        }

        private static string GetNodeValue(XmlNode node, string xpath)
        {
            XmlNode n = node.SelectSingleNode(xpath);
            if (n != null)
            {
                return n.InnerText;
            }
            return "";
        }
    }
}
