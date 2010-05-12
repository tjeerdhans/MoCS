using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using MoCS.BuildService.Business.Interfaces;

namespace MoCS.BuildService.Business
{
    public class BatchFileCreator
    {
        private SystemSettings _sysSettings;
        private SubmitSettings _submitSettings;
        private IFileSystem _fileSystem;

        public string BatchfileCompilePath { get; private set; }
        public string BatchfileTestPath { get; private set; }
        public string OutputDllPath { get; private set; }
        public string SourcesPath { get; private set; }
        public string OutputLogPath { get; private set; }
        public string NUnitConsolePath { get; private set; }
        public string TestLogPath { get; private set; }
     
        public BatchFileCreator(SystemSettings sysSettings, SubmitSettings submitSettings, IFileSystem fileSystem)
        {
            Init(sysSettings, submitSettings, fileSystem);
        }

        private void Init(SystemSettings sysSettings, SubmitSettings submitSettings, IFileSystem fileSystem)
        {
            _sysSettings = sysSettings;
            _submitSettings = submitSettings;
            _fileSystem = fileSystem;

            OutputDllPath = Path.Combine(_submitSettings.BasePath, _submitSettings.GetDllName());
            SourcesPath = Path.Combine(_submitSettings.BasePath, "*.cs");
            OutputLogPath = Path.Combine(_submitSettings.BasePath, "build.log");
            BatchfileCompilePath = Path.Combine(submitSettings.BasePath, "buildit.bat");

            BatchfileTestPath = Path.Combine(submitSettings.BasePath, "testit.bat");
            NUnitConsolePath = Path.Combine(sysSettings.NunitConsolePath, "nunit-console.exe");
            TestLogPath = Path.Combine(submitSettings.BasePath, "testlog.xml");
        }


        public string CreateBuildFile()
        {
            //create complile batch file
            _fileSystem.WriteToFile(GetBatchFileContents(), BatchfileCompilePath);
            return BatchfileCompilePath;

        }

        public string CreateTestFile()
        {
            _fileSystem.WriteToFile(GetTestFileContents(), BatchfileTestPath);
            return BatchfileTestPath;
        }

        public string GetBatchFileContents()
        {
            string s = "\"" + _sysSettings.CscPath + "\"" + " /out:" + "\"" + OutputDllPath + "\"" + " /nologo /lib:" + "\"" + _sysSettings.NunitAssemblyPath + "\"" + " /reference:NUnit.Framework.dll /target:library " + "\"" + SourcesPath + "\"" + " >" + "\"" + OutputLogPath + "\"";
            return s;
        }

        public string GetTestFileContents()
        {
            string s = "\"" + NUnitConsolePath + "\"" + " " + "\"" + OutputDllPath + "\"" + " /nologo /xml=" + "\"" + TestLogPath + "\"";
            return s;
        }

    }
}
