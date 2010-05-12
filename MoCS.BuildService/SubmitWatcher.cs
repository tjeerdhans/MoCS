using System;
using System.Collections.Generic;
using System.Linq;
using System.Timers;
using System.IO;
using System.Xml;
using MoCS.BuildService.Business;
using MoCS.Business;
using MoCS.Business.Facade;
using System.Configuration;

namespace MoCS.BuildService
{
    public class SubmitWatcher
    {
        private Dictionary<string, string> _processing = new Dictionary<string, string>();
        private Timer _timer;
        private SystemSettings _systemSettings;
        private MoCS.Business.IFileSystem _fileSystem;
        private ClientFacade _clientFacade;

        public SubmitWatcher()
        {
            _systemSettings = CreateSystemSettings();
            _fileSystem = new FileSystemWrapper();

            //get connectionstring from config
            string connectionString = ConfigurationSettings.AppSettings["MOCSconnection"];
            _clientFacade = new ClientFacade();
        }


        private SystemSettings CreateSystemSettings()
        {
            //settings that are used on server side
            SystemSettings sysSettings = new SystemSettings();

            string cscPath = ConfigurationSettings.AppSettings["CscPath"];
            string nunitAssemblyPath = ConfigurationSettings.AppSettings["NunitAssemblyPath"];
            string nunitConsolePath = ConfigurationSettings.AppSettings["NunitConsolePath"];

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
            sysSettings.AssignmentsBasePath = ConfigurationSettings.AppSettings["AssignmentBasePath"];
            return sysSettings;
        }


        private void ProcessTeamSubmit(SubmitToProcess submit)
        {
            Console.WriteLine(string.Format("Processing teamsubmit {0} for assignment {1}", submit.TeamName, submit.AssignmentName));


            string resultBasePath = ConfigurationSettings.AppSettings["ResultBasePath"];
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
            if (!_fileSystem.DirectoryExists(resultBasePath + submit.AssignmentName))
            {
                _fileSystem.CreateDirectory(resultBasePath + submit.AssignmentName);
            }
            string date = Utils.DateToString(submit.SubmitDate);
            string time = Utils.TimeToString(submit.SubmitDate);
            string teamDirName = submit.TeamName + "_" + date + "_" + time;
            string teamSubmitDirName = resultBasePath + submit.AssignmentName + @"\" + teamDirName;
            //create a new directory for the teamsubmit
            _fileSystem.CreateDirectory(teamSubmitDirName);

            //copy the file to this directory
            using (Stream target = _fileSystem.FileOpenWrite(Path.Combine(teamSubmitDirName, submit.FileName)))
            {
                byte[] buffer = new byte[0x10000];
                int bytes;
                try
                {
                    while ((bytes = submit.FileStream.Read(buffer, 0, buffer.Length)) > 0)
                    {
                        target.Write(buffer, 0, bytes);
                    }
                }
                finally
                {
                    target.Flush();
                }
            }

            //lookup the assignment and read the assignment file
            string assignmentDir = Path.Combine(_systemSettings.AssignmentsBasePath, submit.AssignmentName);
            if (!_fileSystem.DirectoryExists(assignmentDir))
            {
                throw new ApplicationException("the assignment cannot be found");
            }
            XmlDocument doc = new XmlDocument();
            doc.Load(Path.Combine(assignmentDir, "assignment.xml"));

            AssignmentRepository repository = new AssignmentRepository(_systemSettings.AssignmentsBasePath);
            MoCS.BuildService.Business.Assignment assignment = repository.ReadAssignment(submit.AssignmentName);

            //delete the file if it existed already
            if (_fileSystem.FileExists(Path.Combine(teamSubmitDirName, assignment.InterfaceFile)))
            {
                _fileSystem.FileDelete(Path.Combine(teamSubmitDirName, assignment.InterfaceFile));
            }
            //copy the interface file
            _fileSystem.FileCopy(Path.Combine(assignmentDir, assignment.InterfaceFile),
                        Path.Combine(teamSubmitDirName, assignment.InterfaceFile));

            //copy the server testfile;    
            //delete the file if it existed already
            if (_fileSystem.FileExists(Path.Combine(teamSubmitDirName, assignment.NunitTestFileServer)))
            {
                _fileSystem.FileDelete(Path.Combine(teamSubmitDirName, assignment.NunitTestFileServer));
            }
            _fileSystem.FileCopy(Path.Combine(assignmentDir, assignment.NunitTestFileServer),
                        Path.Combine(teamSubmitDirName, assignment.NunitTestFileServer));

            //copy additional serverfiles
            foreach (string fileName in assignment.ServerFilesToCopy)
            {
                if (_fileSystem.FileExists(Path.Combine(teamSubmitDirName, fileName)))
                {
                    _fileSystem.FileDelete(Path.Combine(teamSubmitDirName, fileName));
                }
                _fileSystem.FileCopy(Path.Combine(assignmentDir, fileName),
                            Path.Combine(teamSubmitDirName, fileName)); 
            }

            //copy the client testfile;    
            if (assignment.NunitTestFileClient != assignment.NunitTestFileServer)
            {
                //delete the file if it existed already
                if (_fileSystem.FileExists(Path.Combine(teamSubmitDirName, assignment.NunitTestFileClient)))
                {
                    _fileSystem.FileDelete(Path.Combine(teamSubmitDirName, assignment.NunitTestFileClient));
                }
                _fileSystem.FileCopy(Path.Combine(assignmentDir, assignment.NunitTestFileClient),
                            Path.Combine(teamSubmitDirName, assignment.NunitTestFileClient));
            }

            //START PROCESSING

            //settings that are read from the assignment
            AssignmentSettings assignmentSettings = new AssignmentSettings();
            assignmentSettings.AssignmentId = submit.AssignmentName;
            assignmentSettings.ClassnameToImplement = assignment.ClassNameToImplement;
            assignmentSettings.InterfaceNameToImplement = assignment.InterfaceNameToImplement;

            //settings that are from the submitprocess/team submit
            SubmitSettings submitSettings = new SubmitSettings();
            submitSettings.TeamId = submit.TeamName;
            submitSettings.BasePath = teamSubmitDirName;
            submitSettings.TimeStamp = DateTime.Now;
            submitSettings.AssignmentId = assignmentSettings.AssignmentId;

            SubmitProcessor processor = new SubmitProcessor(new FileSystem(), new ExecuteCmd());

            SubmitResult result = processor.Process(_systemSettings, assignmentSettings, submitSettings);

            Console.WriteLine(string.Format("Teamsubmit {0} for assignment {1} ended with status {2}", submit.TeamName, submit.AssignmentName, result.Status));
            try
            {
                SaveStatusToDatabase(submit, result);
            }
            catch (Exception ex)
            {
                Console.WriteLine("unexpected exception:" + ex.Message);
            }

        }


        private void SaveStatusToDatabase(SubmitToProcess submit, SubmitResult result)
        {

            string details = "";
            foreach(string detail in result.Messages)
            {
                    details += detail;
            }
            if(details.Length>1000)
            {
                details = details.Substring(0, 999);
            }

            //process result
            switch (result.Status)
            {
                case SubmitStatusCode.Success:  //1
                    _clientFacade.InsertSubmitStatus(submit.TeamID, submit.SubmitID, 1, null);
                    Console.WriteLine(string.Format("Submit {0} for assignment {1} ended with status {2}", submit.TeamName, submit.AssignmentName, result.Status.ToString()));
                    break;
                case SubmitStatusCode.CompilationError: //20
                    _clientFacade.InsertSubmitStatus(submit.TeamID, submit.SubmitID, 20, details);
                    break;
                case SubmitStatusCode.ValidationError:  //21
                    _clientFacade.InsertSubmitStatus(submit.TeamID, submit.SubmitID, 21, details);
                    break;
                case SubmitStatusCode.TestError:        //22
                    _clientFacade.InsertSubmitStatus(submit.TeamID, submit.SubmitID, 22, details);
                    break;
                case SubmitStatusCode.ServerError:      //23
                    _clientFacade.InsertSubmitStatus(submit.TeamID, submit.SubmitID, 23, details);
                    break;
                case SubmitStatusCode.Unknown:          //99
                    _clientFacade.InsertSubmitStatus(submit.TeamID, submit.SubmitID, 90, details);
                    break;
                default:
                    _clientFacade.InsertSubmitStatus(submit.TeamID, submit.SubmitID, 99, details);
                    break;
            }
            _clientFacade.SetTeamSubmitToFinished(submit.SubmitID);
            _processing.Remove(submit.SubmitID.ToString());
        }

       

        public void StartWatching()
        {
            List<SubmitToProcess> submits = _clientFacade.GetUnprocessedSubmits();
            ProcessSubmits(submits);

            string pollingIntervalValue = ConfigurationSettings.AppSettings["PollingInterval"];
            int pollingInterval = Convert.ToInt32(pollingIntervalValue);

            _timer = new Timer();
            _timer.Interval = pollingInterval;
            _timer.Elapsed += new ElapsedEventHandler(_timer_Elapsed);
            _timer.Start();

        }

        void _timer_Elapsed(object sender, ElapsedEventArgs e)
        {
            List<SubmitToProcess> submits = _clientFacade.GetUnprocessedSubmits();
            ProcessSubmits(submits);
        }

        private delegate void DoWorkNeedsTimeOutDelegate(SubmitToProcess submit);
        public void BuildSolution(SubmitToProcess submit)
        {
            try
            {
                ProcessTeamSubmit(submit);
            }
            catch (Exception ex)
            {
                Console.WriteLine(string.Format("ERROR DURING BUILD FOR: {0}-{1}:{2}: ", submit.TeamName, submit.AssignmentName, ex.Message));
            }
        }

        private void ProcessSubmits(List<SubmitToProcess> submits)
        {

            string timeOutValue = ConfigurationSettings.AppSettings["ProcessingTimeOut"];
            int millisecondsToWait = Convert.ToInt32(timeOutValue);

            foreach (SubmitToProcess s in submits)
            {
                if (!_processing.Keys.Contains(s.SubmitID.ToString()))
                {
                    _processing.Add(s.SubmitID.ToString(), s.SubmitID.ToString());

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




    }
}
