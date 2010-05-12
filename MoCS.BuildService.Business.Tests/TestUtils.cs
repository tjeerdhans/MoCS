using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using System.Reflection;
using System.IO;

namespace MoCS.BuildService.Business.Tests
{
    public class TestUtils
    {

        public static XmlDocument GetXmlDocFromResources(string fileName)
        {
            XmlDocument doc = new XmlDocument();
            doc.Load(GetStreamFromResources(fileName));
            return doc;
        }

        public static Stream GetStreamFromResources(string fileName)
        {
            Assembly a = Assembly.GetExecutingAssembly();
            Stream stream = a.GetManifestResourceStream("MoCS.BuildService.Business.Tests.TestFiles." + fileName);
            return stream;
        }
    }
}
