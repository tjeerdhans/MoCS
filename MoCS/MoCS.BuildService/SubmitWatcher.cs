using System;
using System.Collections.Generic;
using System.Linq;
using System.Timers;
using System.IO;
using System.Xml;
using MoCS.BuildService.Business;
using MoCS.Business.Objects;
using MoCS.Business.Facade;
using System.Configuration;
using MoCS.BuildService.Business.Interfaces;

namespace MoCS.BuildService
{
    public class SubmitWatcher
    {
        private Dictionary<string, string> _processing = new Dictionary<string, string>();
        private Timer _timer;
        private SystemSettings _systemSettings;
        private IFileSystem _fileSystem;
        //private ClientFacade _clientFacade;

        private delegate void DoWorkNeedsTimeOutDelegate(Submit submit);

        public SubmitWatcher()
        {
            _systemSettings = CreateSystemSettings();
            _fileSystem = new MoCS.BuildService.Business.FileSystemWrapper();

            //get connectionstring from config
            //string connectionString = ConfigurationSettings.AppSettings["MOCSconnection"];
            //_clientFacade = new ClientFacade();
        }

        private SystemSettings CreateSystemSettings()
        {
            //settings that are used on server side
            SystemSettings sysSettings = new SystemSettings();

            string cscPath = ConfigurationManager.AppSettings["CscPath"];
            string nunitAssemblyPath = ConfigurationManager.AppSettings["NunitAssemblyPath"];
            string nunitConsolePath = ConfigurationManager.AppSettings["NunitConsolePath"];

            //no final slash allowed on nunitPath
            if (nunitAssemblyPath.EndsWith(@"\"))
            {
                nunitAssemblyPath = nunitAssemblyPath.Substring(0, nunitAssemblyPath.Length - 1);
            }

            //no final slash allowed on nunitPath
            if (nunitConsolePath.EndsWith(@"\"))
            {
                nunitConsolePath = nunitConsolePath.Substring(0, nunitAssemblyPath.Length - 1);
            }

            sysSettings.CscPath = cscPath;
            sysSettings.NunitAssemblyPath = nunitAssemblyPath;
            sysSettings.NunitConsolePath = nunitConsolePath;
            sysSettings.AssignmentsBasePath = ConfigurationManager.AppSettings["AssignmentBasePath"];
            return sysSettings;
        }

        public void StartWatching()
        {
            List<Submit> submits = ClientFacade.Instance.GetUnprocessedSubmits();

            ProcessSubmits(submits);

            string pollingIntervalValue = ConfigurationManager.AppSettings["PollingInterval"];
            int pollingInterval = Convert.ToInt32(pollingIntervalValue);

            _timer = new Timer();
            _timer.Interval = pollingInterval;
            _timer.Elapsed += new ElapsedEventHandler(_timer_Elapsed);
            _timer.Start();

        }

        void _timer_Elapsed(object sender, ElapsedEventArgs e)
        {
            List<Submit> submits = ClientFacade.Instance.GetUnprocessedSubmits();
            ProcessSubmits(submits);
        }

        private void ProcessSubmits(List<Submit> submits)
        {

            int millisecondsToWait = int.Parse(ConfigurationManager.AppSettings["ProcessingTimeOut"]);

            foreach (Submit s in submits)
            {
                if (!_processing.Keys.Contains(s.Id.ToString()))
                {
                    _processing.Add(s.Id.ToString(), s.Id.ToString());

                    DoWorkNeedsTimeOutDelegate deleg = new DoWorkNeedsTimeOutDelegate(this.BuildSolution);

                    IAsyncResult ar = deleg.BeginInvoke(s, null, new object());

                    if (!ar.AsyncWaitHandle.WaitOne(millisecondsToWait, false))
                    {
                        //todo: make this a specific exception
                        throw new ApplicationException("TIMEOUT");
                    }
                    else
                    {
                        deleg.EndInvoke(ar);
                    }
                }
            }
        }

        private void ProcessTeamSubmit(Submit submit)
        {
            string teamName = submit.Team.Name;
            string assignmentName = submit.TournamentAssignment.Assignment.Name;

            Console.WriteLine(string.Format("Processing teamsubmit {0} for assignment {1}", teamName, assignmentName));

            string resultBasePath = ConfigurationManager.AppSettings["ResultBasePath"];
            if (!resultBasePath.EndsWith(@"\"))
            {
                resultBasePath += @"\";
            }

            //prepare processing
            //create a new directory for the assignmentresults
            if (!_fileSystem.DirectoryExists(resultBasePath))
            {
                _fileSystem.CreateDirectory(resultBasePath);
            }
            if (!_fileSystem.DirectoryExists(resultBasePath + assignmentName))
            {
                _fileSystem.CreateDirectory(resultBasePath + assignmentName);
            }
            //string date = Utils.DateToString(submit.SubmitDate);
            //string time = Utils.TimeToString(submit.SubmitDate);
            string teamDirName = teamName + submit.SubmitDate.ToString("ddMMyyyy_HHmmss"); // "_" + date + "_" + time;
            string teamSubmitDirName = resultBasePath + assignmentName + @"\" + teamDirName;
            //create a new directory for the teamsubmit
            _fileSystem.CreateDirectory(teamSubmitDirName);

            // Copy nunit.framework.dll to this directory
            _fileSystem.FileCopy(Path.Combine(_systemSettings.NunitAssemblyPath, "nunit.framework.dll"),
                        Path.Combine(teamSubmitDirName, "nunit.framework.dll"), true);

            //copy the file to this directory
            using (Stream target = _fileSystem.FileOpenWrite(Path.Combine(teamSubmitDirName, submit.FileName)))
            {
                //byte[] buffer = new byte[0x10000];
                //int bytes;
                try
                {
                    target.Write(submit.Data, 0, submit.Data.Length);
                    //while ((bytes = submit.Data.Read(buffer, 0, buffer.Length)) > 0)
                    //{
                    //    target.Write(buffer, 0, bytes);
                    //}
                }
                finally
                {
                    target.Flush();
                }
            }

            MoCS.Business.Objects.Assignment assignment = ClientFacade.Instance.GetAssignmentById(submit.TournamentAssignment.Assignment.Id, true);


            // Copy the interface file
            //delete the file if it existed already
            AssignmentFile interfaceFile = assignment.AssignmentFiles.Find(af => af.Name == "InterfaceFile");
            if (_fileSystem.FileExists(Path.Combine(teamSubmitDirName, interfaceFile.FileName)))
            {
                _fileSystem.FileDelete(Path.Combine(teamSubmitDirName, interfaceFile.FileName));
            }
            _fileSystem.FileCopy(Path.Combine(assignment.Path, interfaceFile.FileName),
                        Path.Combine(teamSubmitDirName, interfaceFile.FileName));


            //copy the server testfile
            //delete the file if it existed already
            AssignmentFile serverTestFile = assignment.AssignmentFiles.Find(af => af.Name == "NunitTestFileServer");
            if (_fileSystem.FileExists(Path.Combine(teamSubmitDirName, serverTestFile.FileName)))
            {
                _fileSystem.FileDelete(Path.Combine(teamSubmitDirName, serverTestFile.FileName));
            }
            _fileSystem.FileCopy(Path.Combine(assignment.Path, serverTestFile.FileName),
                        Path.Combine(teamSubmitDirName, serverTestFile.FileName));

            //copy additional serverfiles
            List<AssignmentFile> serverFilesToCopy = assignment.AssignmentFiles.FindAll(af => af.Name == "ServerFileToCopy");
            foreach (AssignmentFile serverFileToCopy in serverFilesToCopy)
            {
                if (_fileSystem.FileExists(Path.Combine(teamSubmitDirName, serverFileToCopy.FileName)))
                {
                    _fileSystem.FileDelete(Path.Combine(teamSubmitDirName, serverFileToCopy.FileName));
                }
                _fileSystem.FileCopy(Path.Combine(assignment.Path, serverFileToCopy.FileName),
                            Path.Combine(teamSubmitDirName, serverFileToCopy.FileName));
            }

            //copy the client testfile
            AssignmentFile clientTestFile = assignment.AssignmentFiles.Find(af => af.Name == "NunitTestFileClient");

            //delete the file if it existed already
            if (_fileSystem.FileExists(Path.Combine(teamSubmitDirName, clientTestFile.FileName)))
            {
                _fileSystem.FileDelete(Path.Combine(teamSubmitDirName, clientTestFile.FileName));
            }
            _fileSystem.FileCopy(Path.Combine(assignment.Path, clientTestFile.FileName),
                        Path.Combine(teamSubmitDirName, clientTestFile.FileName));


            //START PROCESSING

            //settings that are read from the assignment
            AssignmentSettings assignmentSettings = new AssignmentSettings();
            assignmentSettings.AssignmentId = assignmentName;
            assignmentSettings.ClassnameToImplement = assignment.ClassNameToImplement;
            assignmentSettings.InterfaceNameToImplement = assignment.InterfaceNameToImplement;

            //settings that are from the submitprocess/team submit
            SubmitSettings submitSettings = new SubmitSettings();
            submitSettings.TeamId = teamName;
            submitSettings.BasePath = teamSubmitDirName;
            submitSettings.TimeStamp = DateTime.Now;
            submitSettings.AssignmentId = assignmentSettings.AssignmentId;

            SubmitProcessor processor = new SubmitProcessor(new MoCS.BuildService.Business.FileSystemWrapper(), new ExecuteCmd());

            //set status of submit to 'processing'
            ClientFacade.Instance.UpdateSubmitStatusDetails(submit.Id, SubmitStatus.Processing, "This submitted is currently processed.", DateTime.Now);

            SubmitResult result = processor.Process(_systemSettings, assignmentSettings, submitSettings);

            // Delete nunit.framework.dll from the submit dir to keep things clean
            _fileSystem.FileDelete(Path.Combine(teamSubmitDirName, "nunit.framework.dll"));

            Console.WriteLine(string.Format("Processing of submit {0} for assignment {1}, submitted by team {2} ended with status {3}", new object[] { submit.Id, assignmentName, teamName, result.Status }));
            try
            {
                SaveStatusToDatabase(submit, result);
            }
            catch (Exception ex)
            {
                Console.WriteLine("unexpected exception:" + ex.Message);
            }

        }

        private void SaveStatusToDatabase(Submit submit, SubmitResult result)
        {
            string teamName = submit.Team.Name;
            string assignmentName = submit.TournamentAssignment.Assignment.Name;

            string details = "";
            foreach (string detail in result.Messages)
            {
                details += detail;
            }
            if (details.Length > 1000)
            {
                details = details.Substring(0, 999);
            }

            //process result
            switch (result.Status)
            {
                case SubmitStatusCode.Unknown:
                    ClientFacade.Instance.UpdateSubmitStatusDetails(submit.Id, SubmitStatus.ErrorUnknown, details, DateTime.Now);
                    break;
                case SubmitStatusCode.CompilationError:
                    ClientFacade.Instance.UpdateSubmitStatusDetails(submit.Id, SubmitStatus.ErrorCompilation, details, DateTime.Now);
                    break;
                case SubmitStatusCode.ValidationError:
                    ClientFacade.Instance.UpdateSubmitStatusDetails(submit.Id, SubmitStatus.ErrorValidation, details, DateTime.Now);
                    break;
                case SubmitStatusCode.TestError:
                    ClientFacade.Instance.UpdateSubmitStatusDetails(submit.Id, SubmitStatus.ErrorTesting, details, DateTime.Now);
                    break;
                case SubmitStatusCode.ServerError:
                    ClientFacade.Instance.UpdateSubmitStatusDetails(submit.Id, SubmitStatus.ErrorServer, details, DateTime.Now);
                    break;
                case SubmitStatusCode.Success:
                    ClientFacade.Instance.UpdateSubmitStatusDetails(submit.Id, SubmitStatus.Success, details, DateTime.Now);
                    break;
                default:
                    break;
            }

            _processing.Remove(submit.Id.ToString());
        }

        public void BuildSolution(Submit submit)
        {
            try
            {
                ProcessTeamSubmit(submit);
            }
            catch (Exception ex)
            {
                Console.WriteLine(string.Format("ERROR DURING BUILD FOR: {0}-{1}:{2}: ", submit.Team.Name, submit.TournamentAssignment.Assignment.Name, ex.Message));
            }
        }
    }
}
