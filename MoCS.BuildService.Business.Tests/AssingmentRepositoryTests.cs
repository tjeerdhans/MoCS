using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using System.Xml;
using System.IO;
using System.Reflection;

namespace MoCS.BuildService.Business.Tests
{
    [TestFixture]
    public class AssingmentRepositoryTests
    {
        [Test]
        public void ReadAssignment_CorrectValues_ValuesAreSet()
        {
            string basePath = @"c:\fakepath\";
            AssignmentRepository repository = new AssignmentRepository(basePath);

            XmlDocument doc = TestUtils.GetXmlDocFromResources("TextXml1.xml");
            Assignment assignment = repository.ReadAssignment(doc);

            Assert.AreEqual("classnametoimplement", assignment.ClassNameToImplement);
            Assert.AreEqual("interfacenametoimplement", assignment.InterfaceNameToImplement);
            Assert.AreEqual("nunittestfileclient", assignment.NunitTestFileClient);
            Assert.AreEqual("nunittestfileserver", assignment.NunitTestFileServer);
            Assert.AreEqual("interfacefile", assignment.InterfaceFile);

        }


    }
}
