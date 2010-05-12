//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Xml;
//using MoCS.Business.Objects;

//namespace MoCS.Business.Facade
//{
//    public class TournamentReportGenerator
//    {

//        private static XmlAttribute AppendAttribute(XmlNode node, string name, string value)
//        {
//            XmlAttribute a1 = node.OwnerDocument.CreateAttribute(name);
//            a1.Value = value;
//            node.Attributes.Append(a1);
//            return a1;
//        }

//        private static XmlNode AppendRankingNode(XmlNode parentnode, string description, Team t, Submit s)
//        {
//            XmlNode node = parentnode.OwnerDocument.CreateElement("submit");
//            parentnode.AppendChild(node);
//            AppendAttribute(node, "description", description);
//            AppendAttribute(node, "teamname", t.Name);
//            AppendAttribute(node, "teammembers", t.Members);
//            AppendAttribute(node, "startdate", s.StartDate.ToString());
//            AppendAttribute(node, "submitdate", s.SubmitDate.ToString());
//            AppendAttribute(node, "deltatime", s.DeltaTime.ToString());

//            return node;
//        }

//        private static XmlNode AppendRankingNode(XmlNode parentnode, string description, Team t, Submit s, TournamentAssignment assignment)
//        {
//            XmlNode node = parentnode.OwnerDocument.CreateElement("submit");
//            parentnode.AppendChild(node);
//            AppendAttribute(node, "description", description);
//            AppendAttribute(node, "teamname", t.Name);
//            AppendAttribute(node, "teammembers", t.Members);
//            AppendAttribute(node, "startdate", s.StartDate.ToString());
//            AppendAttribute(node, "submitdate", s.SubmitDate.ToString());
//            AppendAttribute(node, "deltatime", s.DeltaTime.ToString());
//            AppendAttribute(node, "assignment", assignment.AssignmentName);

//            return node;
//        }



//        public static XmlDocument CreateTournamentReport(Tournament tournament, List<Team> teams, List<TournamentAssignment> assignments
//                                , List<Submit> submits)
//        {

//            XmlDocument report = new XmlDocument();

//            XmlNode tournamentNode = report.CreateElement("tournament");
//            report.AppendChild(tournamentNode);
//            AppendAttribute(tournamentNode, "name", tournament.Name);

//            XmlNode assignmentsNode = report.CreateElement("assignments");
//            tournamentNode.AppendChild(assignmentsNode);


//            foreach (TournamentAssignment assignment in assignments)
//            {
//                XmlNode assignmentNode = report.CreateElement("assignment");
//                assignmentsNode.AppendChild(assignmentNode);
//                AppendAttribute(assignmentNode, "name", assignment.AssignmentName);

//                //GOLD, SILVER AND BRONZE MEDALS
//                //for first tournamentassignment, get the successful submits
//                var successful = from s in submits
//                                 where s.CurrentStatus == 1
//                                 where s.TournamentAssignmentId == assignment.TournamentAssignmentId
//                                 orderby s.DeltaTime ascending
//                                 select s;

//                double totalMilliseconds = 0;
//                int index = 1;
//                foreach (var s in successful)
//                {
//                    totalMilliseconds += s.DeltaTime;

//                    //get the team
//                    Team submitTeam = (from t in teams
//                                       where t.Id == s.TeamId
//                                       select t).FirstOrDefault();

//                    XmlNode node = AppendRankingNode(assignmentNode, index.ToString(), submitTeam, s);
//                    index++;

//                }
//                double avg = 0;
//                if (successful.Count() > 0)
//                {
//                    avg = totalMilliseconds / successful.Count();
//                }
//                AppendAttribute(assignmentNode, "averageTime", avg.ToString());
//            }

//            XmlNode variousNode = report.CreateElement("various");
//            tournamentNode.AppendChild(variousNode);

//            //PIONEER MEDAL
//            //get the first successful submit
//            Submit firstSuccessfulSubmit = (from s in submits
//                                            where s.IsFinished = true
//                                            where s.CurrentStatus == 1
//                                            orderby s.SubmitDate ascending
//                                            select s).FirstOrDefault();

//            if (firstSuccessfulSubmit != null)
//            {       //get the team
//                Team submitTeam = (from t in teams
//                                   where t.Id == firstSuccessfulSubmit.TeamId
//                                   select t).FirstOrDefault();

//                TournamentAssignment a = (from amt in assignments
//                                          where amt.TournamentAssignmentId == firstSuccessfulSubmit.TournamentAssignmentId
//                                          select amt).FirstOrDefault();

//                AppendRankingNode(variousNode, "FirstSuccess", submitTeam, firstSuccessfulSubmit, a);
//            }



//            //FIRST BLOOD MEDAL
//            //get the first error
//            Submit firstError = (from s in submits
//                                 where s.IsFinished = true
//                                 where s.CurrentStatus != 1
//                                 orderby s.SubmitDate ascending
//                                 select s).FirstOrDefault();

//            if (firstError != null)
//            {       //get the team
//                Team submitTeam = (from t in teams
//                                   where t.Id == firstError.TeamId
//                                   select t).FirstOrDefault();

//                TournamentAssignment a = (from amt in assignments
//                                          where amt.TournamentAssignmentId == firstError.TournamentAssignmentId
//                                          select amt).FirstOrDefault();

//                AppendRankingNode(variousNode, "FirstFailure", submitTeam, firstError, a);
//            }


//            //POWERSHOT MEDAL
//            //get the first successful submit
//            Submit fastestSuccessfulSubmit = (from s in submits
//                                              where s.IsFinished = true
//                                              where s.CurrentStatus == 1
//                                              orderby s.DeltaTime ascending
//                                              select s).FirstOrDefault();

//            if (fastestSuccessfulSubmit != null)
//            {       //get the team
//                Team submitTeam = (from t in teams
//                                   where t.Id == fastestSuccessfulSubmit.TeamId
//                                   select t).FirstOrDefault();

//                TournamentAssignment a = (from amt in assignments
//                                          where amt.TournamentAssignmentId == fastestSuccessfulSubmit.TournamentAssignmentId
//                                          select amt).FirstOrDefault();

//                AppendRankingNode(variousNode, "FastestSuccess", submitTeam, fastestSuccessfulSubmit, a);
//            }

//            //CRAHS N BURN MEDAL
//            //get the fastest error
//            Submit fastestError = (from s in submits
//                                   where s.IsFinished = true
//                                   where s.CurrentStatus != 1
//                                   orderby s.DeltaTime ascending
//                                   select s).FirstOrDefault();

//            if (fastestError != null)
//            {       //get the team
//                Team submitTeam = (from t in teams
//                                   where t.Id == fastestError.TeamId
//                                   select t).FirstOrDefault();

//                TournamentAssignment a = (from amt in assignments
//                                          where amt.TournamentAssignmentId == fastestError.TournamentAssignmentId
//                                          select amt).FirstOrDefault();

//                AppendRankingNode(variousNode, "FastestFailure", submitTeam, fastestError, a);
//            }

//            //MOST FAILING SUBMITS...TODO  (TRIAL AND ERROR MEDAL)

//            //MOST FLAWLESS CHECKINS...TODO  (FLAWLESS)


//            XmlNode teamsNode = report.CreateElement("teams");
//            tournamentNode.AppendChild(teamsNode);


//            foreach (Team t in teams)
//            {
//                if (t.IsAdmin)
//                {
//                    continue;
//                }


//                XmlNode teamNode = report.CreateElement("team");
//                teamsNode.AppendChild(teamNode);
//                AppendAttribute(teamNode, "name", t.Name);
//                AppendAttribute(teamNode, "members", t.Members);

//                int success = 0;
//                int total = 0;
//                int failure = 0;
//                double totalTime = 0;
//                //int compilefailure = 0;
//                //int testfailure = 0;
//                //int otherfailure = 0;

//                var submitsPerTeam = from z in submits
//                                     where z.TeamId == t.Id
//                                     select z;

//                foreach (var s in submitsPerTeam)
//                {
//                    total++;
//                    if (s.CurrentStatus == 1)
//                    {
//                        success++;
//                        totalTime += s.DeltaTime;
//                    }
//                    if (s.CurrentStatus > 1)
//                    {
//                        failure++;
//                    }
//                }
//                AppendAttribute(teamNode, "total", total.ToString());
//                AppendAttribute(teamNode, "success", success.ToString());
//                AppendAttribute(teamNode, "failure", failure.ToString());
//                AppendAttribute(teamNode, "totalTime", totalTime.ToString());
//            }

//            return report;
//        }

//    }
//}
