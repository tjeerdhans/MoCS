using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using System.Reflection;
using System.Xml;

namespace MoCS.BuildService.Business.Tests
{

    /// <summary>
    /// this class contains the integration tests form submitprocessor
    /// </summary>
    [TestFixture]
    public class SubmitProcessorTests
    {
        SystemSettings sys = new SystemSettings();
        AssignmentSettings asettings = new AssignmentSettings();
        SubmitSettings sub = new SubmitSettings();

        [SetUp]
        public void SetUp()
        {
            sub.BasePath = @"c:\test\";
            sys.NunitAssemblyPath = @"c:\nunit\";
            sys.NunitConsolePath = @"c:\nunit\";
            sys.CscPath = @"c:\csc\csc.exe";

            asettings.ClassnameToImplement = "";
            asettings.InterfaceNameToImplement = "";
        }



        [Test]
        public void Process_SuccessfulResult_Ok()
        {

            SetUp();

            FakeFileSystem fakeFs = new FakeFileSystem();
            FakeCommand fakeCmd = new FakeCommand();
            fakeCmd.BuildResult = 0;
            fakeCmd.TestResult = 0;

            SubmitProcessor processor = new SubmitProcessor(fakeFs, fakeCmd);

            asettings.ClassnameToImplement = "FakeFileSystem";
            asettings.InterfaceNameToImplement = "IFileSystem";

            SubmitResult result = processor.Process(sys, asettings, sub);

            Assert.AreEqual(result.Status, SubmitStatusCode.Success);

            //not so nice here en not really tests, but this is the entry point for this for now
            string testBatch = fakeFs.GetTestBatch();
            string buildBatch = fakeFs.GetBuildBatch();

            Assert.IsTrue(buildBatch.Length > 0);
            Assert.IsTrue(testBatch.Length > 0);
        }

        


        [Test]
        public void Process_CompilationFailure_Ok()
        {

            SetUp();

            FakeFileSystem fakeFs = new FakeFileSystem();
            FakeCommand fakeCmd = new FakeCommand();
            fakeCmd.BuildResult = 1;
            fakeFs.AddBuildLog("buildlogerror");

            SubmitProcessor processor = new SubmitProcessor(fakeFs, fakeCmd);

            SubmitResult result = processor.Process(sys, asettings, sub);

            Assert.AreEqual(result.Status, SubmitStatusCode.CompilationError);
            Assert.AreEqual(result.Messages.Count, 1);
            Assert.AreEqual(result.Messages[0], "buildlogerror");

        }

        [Test]
        public void Process_ValidationFailureClassNotFound_ValidationError()
        {


            SetUp();

            FakeFileSystem fakeFs = new FakeFileSystem();
            FakeCommand fakeCmd = new FakeCommand();
            fakeCmd.BuildResult = 0;
  
            SubmitProcessor processor = new SubmitProcessor(fakeFs, fakeCmd);
            asettings.ClassnameToImplement = "UNKNOWNNAME";

            SubmitResult result = processor.Process(sys, asettings, sub);

            Assert.AreEqual(result.Status, SubmitStatusCode.ValidationError);
            Assert.AreEqual(result.Messages.Count, 1);
            Assert.AreEqual(result.Messages[0], "The class to implement (UNKNOWNNAME) is not found");
        }


        [Test]
        public void Process_ValidationFailureInterfaceNotImplemented_ValidationError()
        {

            SetUp();

            FakeFileSystem fakeFs = new FakeFileSystem();
            FakeCommand fakeCmd = new FakeCommand();
            fakeCmd.BuildResult = 0;

            SubmitProcessor processor = new SubmitProcessor(fakeFs, fakeCmd);
            asettings.ClassnameToImplement = "FakeFileSystem";
            asettings.InterfaceNameToImplement = "UNKNOWNINTERFACE";

            SubmitResult result = processor.Process(sys, asettings, sub);

            Assert.AreEqual(result.Status, SubmitStatusCode.ValidationError);
            Assert.AreEqual(result.Messages.Count, 1);
            Assert.AreEqual(result.Messages[0], "The class to implement (FakeFileSystem) does not implement the required interface UNKNOWNINTERFACE");
        }


        [Test]
        public void Process_TestFailure_Ok()
        {


            SetUp();

            FakeFileSystem fakeFs = new FakeFileSystem();
            FakeCommand fakeCmd = new FakeCommand();
            fakeCmd.BuildResult = 0;
            fakeCmd.TestResult = 1; //error
 
            //add testlog to fakeFs
            string errormessage = "the error details";
            fakeFs.AddTestLog(CreateXmlTestError(errormessage));

            SubmitProcessor processor = new SubmitProcessor(fakeFs, fakeCmd);

            asettings.ClassnameToImplement = "FakeFileSystem";
            asettings.InterfaceNameToImplement = "IFileSystem";

            SubmitResult result = processor.Process(sys, asettings, sub);

            Assert.AreEqual(result.Status, SubmitStatusCode.TestError);
            Assert.AreEqual(result.Messages.Count, 1);
            Assert.AreEqual(result.Messages[0], errormessage);
        }



        [Test]
        public void ValidationError()
        {
            //TODO
            Assert.AreEqual(1, 1);
        }



        public string CreateXmlTestError(string errormessage)
        {

            XmlDocument d = new XmlDocument();
            XmlElement e = d.CreateElement("test-results");
            d.AppendChild(e);
            XmlAttribute a = d.CreateAttribute("failures");
            a.Value = "1";
            e.Attributes.Append(a);

            XmlElement e1 = d.CreateElement("test-suite");
            e.AppendChild(e1);

            XmlElement e2 = d.CreateElement("results");
            e1.AppendChild(e2);

            XmlElement e3 = d.CreateElement("test-suite");
            e2.AppendChild(e3);

            XmlElement e4 = d.CreateElement("results");
            e3.AppendChild(e4);

            XmlElement e5 = d.CreateElement("test-case");
            e4.AppendChild(e5);
            XmlAttribute attName = d.CreateAttribute("name");
            XmlAttribute attSuccess = d.CreateAttribute("success");
            e5.Attributes.Append(attSuccess);
            e5.Attributes.Append(attName);
            attName.Value = "somename";
            attSuccess.Value = "False";


            XmlElement e6 = d.CreateElement("failure");
            e5.AppendChild(e6);

            XmlElement e7 = d.CreateElement("message");
            e6.AppendChild(e7);
            e7.InnerText = errormessage;

            return d.OuterXml;

        }

      

    }


    

    public class FakeCommand : IExecuteCmd
    {

        #region IExecuteCmd Member

        public int BuildResult {get;set;}
        public int TestResult { get; set; }


        public int ExecuteCommandSync(object command)
        {
            if (command.ToString().EndsWith("buildit.bat"))
            {
                return BuildResult;
            }
            if (command.ToString().EndsWith("testit.bat"))
            {
                return TestResult;
            }
                        
            return 0;
        }

        #endregion
    }


    public class FakeFileSystem : IFileSystem
    {

        #region IFileSystem Members
        private Dictionary<string, string> _files;
        private List<string> _buildErrorMessages;

        public FakeFileSystem()
        {
            _files = new Dictionary<string, string>();
        }
       
        public void WriteToFile(string contents, string filePath)
        {
            _files.Add(filePath, contents);
        }

        public List<string> ReadErrorsFromBuildLog(string outputLogPath)
        {
            return _buildErrorMessages;
        }


        public string GetBuildBatch()
        {
            foreach (string key in _files.Keys)
            {
                if (key.EndsWith("buildit.bat"))
                {
                    return _files[key];
                }
            }
            return "";
        }

        public string GetTestBatch()
        {
            foreach (string key in _files.Keys)
            {
                if (key.EndsWith("testit.bat"))
                {
                    return _files[key];
                }
            }
            return "";
        }

        public void AddTestLog(string log)
        {
            _files.Add("testlog", log);
        }

        public void AddBuildLog(string message)
        {
            _buildErrorMessages = new List<string>();
            _buildErrorMessages.Add(message);
        }

        public bool FileExists(string filePath)
        {
            if (filePath.EndsWith("testlog.xml"))
            {
                 return _files.ContainsKey("testlog");
            }

            if (filePath.EndsWith("build.log"))
            {
                return (_buildErrorMessages != null && _buildErrorMessages.Count > 0);
            }

            return true;
        }

        public Assembly LoadAssembly(string filePath)
        {
            return Assembly.GetExecutingAssembly();
        }

        public XmlDocument LoadXmlDocument(string testlogPath)
        {
            XmlDocument doc = new XmlDocument();
            if (FileExists(testlogPath))
            {
                string content = _files["testlog"];
                doc.LoadXml(content);

            }
            return doc;
        }



        #endregion
    }



}
