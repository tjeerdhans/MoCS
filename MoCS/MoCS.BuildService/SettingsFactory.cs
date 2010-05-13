using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MoCS.BuildService.Business;
using System.Configuration;
using MoCS.Business.Objects;

namespace MoCS.BuildService
{
    public class SettingsFactory
    {
        public static SystemSettings CreateSystemSettings()
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
            sysSettings.NunitTimeOut = int.Parse(ConfigurationManager.AppSettings["ProcessingTimeOut"]);

            sysSettings.AssignmentsBasePath = ConfigurationManager.AppSettings["AssignmentBasePath"];
            return sysSettings;
        }

        public static SubmitSettings CreateSubmitSettings(string teamName, string teamSubmitDirName, string assignmentId)
        {
            SubmitSettings submitSettings = new SubmitSettings();
            submitSettings.TeamId = teamName;
            submitSettings.BasePath = teamSubmitDirName;
            submitSettings.TimeStamp = DateTime.Now;
            submitSettings.AssignmentId = assignmentId;
            return submitSettings;
        }

        public static AssignmentSettings CreateAssignmentSettings(Assignment assignment, string assignmentName)
        {
            AssignmentSettings assignmentSettings = new AssignmentSettings();
            assignmentSettings.AssignmentId = assignmentName;
            assignmentSettings.ClassnameToImplement = assignment.ClassNameToImplement;
            assignmentSettings.InterfaceNameToImplement = assignment.InterfaceNameToImplement;
            return assignmentSettings;
        }
    }
}
