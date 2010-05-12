using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MoCS.Business
{
    public class Submit
    {
        public int ID { get; set; }
        public string TeamName { get; set; }
        public DateTime SubmitDate { get; set; }
        public DateTime StatusDate { get; set; }
        public int CurrentStatus { get; set; }
        public bool IsFinished { get; set; }
        public string TeamMembers { get; set; }
        public int SubmitID { get; set; }
        public string Details { get; set; }
        public string FileContents { get; set; }
        public string FileName { get; set; }
        public int TeamId { get; set; }
        public int AssignmentId { get; set; }
        public DateTime StartDate { get; set; }
        public int TeamTournamentAssignmentId { get; set; }
        public int TournamentAssignmentId { get; set; }
        public byte[] UploadStream{get;set;}
        public int TournamentId { get; set; }

        public double DeltaTime
        {
            get
            {
                TimeSpan span = SubmitDate.Subtract(StartDate);
                return span.TotalMilliseconds;
            }
        }

        public SubmitStatus Status
        {
            get { return ConvertStatus(CurrentStatus); }
        }

        public static SubmitStatus ConvertStatus(int statusCode)
        {
            switch (statusCode)
            {
                case 0:
                    return SubmitStatus.Submitted;
                case 1:
                    return SubmitStatus.Success;
                case 20:
                    return SubmitStatus.ErrorCompilation;
                case 21:
                    return SubmitStatus.ErrorValidation;
                case 22:
                    return SubmitStatus.ErrorTesting;
                case 23:
                    return SubmitStatus.ErrorServer;
                 case 90:
                case 99:
                    return SubmitStatus.ErrorUnknown;
                 default:
                    return SubmitStatus.ErrorUnknown;

            }

        }


    }
}