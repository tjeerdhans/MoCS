using System.Globalization;
using MoCS.Business.Facade;
using MoCS.Business.Objects;
using MoCS.WebClient.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Text;
using System.Web;
using System.Web.Mvc;

namespace MoCS.WebClient.Controllers
{
    public class CurrentAssignmentController : Controller
    {
        //
        // GET: /CurrentAssignment/


        [Authorize]
        public ActionResult Index()
        {
            Team team = SessionUtil.GetTeamFromFormsAuthentication();
            Tournament t = SessionUtil.GetTournamentFromSession();
            TournamentAssignment ta = SessionUtil.GetTournamentAssignmentFromSession();
            Assignment a = SessionUtil.GetAssignmentFromSession();

            if (ta == null)
            {
                return RedirectToAction("Index", "Assignments");
            }

            bool hasEnrolled = false;

            // Find enrollment
            var aeList = ClientFacade.Instance.GetAssignmentEnrollmentsForTeamForTournamentAssignment(ta.Id, team.Id);
            //MoCSServiceProxy.Instance.GetAssignmentEnrollmentsForTeamForTournament(teamId, tournamentId);

            // See if the team has enrolled for the tournament assignment.
            var ae = aeList.Find(i => i.TournamentAssignment.Id == ta.Id && i.IsActive);
            if (ae != null)
            {
                hasEnrolled = true;
                SessionUtil.SetSession(t, ta, a, ae);
            }
            else
            {
                SessionUtil.SetSession(t, ta, a, null);
            }

            var submitsByTeam = new List<Submit>();

            ta = ClientFacade.Instance.GetTournamentAssignmentById(ta.Id, false);
            if (hasEnrolled)
            {
                submitsByTeam = ClientFacade.Instance.GetSubmitsForAssignmentEnrollment(ae.Id);
            }

            // Construct the model
            var caModel = new CurrentAssignmentModel
            {
                HasEnrolled = hasEnrolled,
                AssignmentName = ta.Assignment.FriendlyName,
                AssignmentTagline = ta.Assignment.Tagline,
                AssignmentCategory = ta.Assignment.Category,
                AssignmentDifficulty = ta.Assignment.Difficulty,
                AssignmentEnrollmentTime = DateTime.Now,
                DownloadUrl = "http://google.com",
                SubmitModelList = new List<SubmitModel>(),
                TabContentModelList = new List<TabContentModel>()
            };

            // Get the AssignmentFiles and Submits if the team has enrolled for this assignment

            if (hasEnrolled)
            {
                foreach (var tc in ta.Assignment.AssignmentFiles)
                {
                    caModel.TabContentModelList.Add(new TabContentModel
                    {
                        Name = tc.Name,
                        ContentType = "plaintext",
                        Content = Encoding.UTF8.GetString(tc.Data).Replace(Environment.NewLine, "<br />")
                    });
                }

                foreach (var submit in submitsByTeam)
                {
                    var timeTaken = Math.Floor(submit.SecondsSinceEnrollment / 60d) + ":" + submit.SecondsSinceEnrollment % 60;
                    caModel.SubmitModelList.Add(new SubmitModel
                    {
                        Id = submit.Id,
                        Result = submit.Status,
                        TimeTaken = timeTaken,
                        SubmitDate = submit.SubmitDate,
                        ProcessingDetails = submit.ProcessingDetails,
                        FileContents = submit.FileContents.Replace(Environment.NewLine, "\r\n") //.Replace(Environment.NewLine, "<br />")
                    });
                }
            }

            return View(caModel);
        }

        [Authorize]
        public ActionResult Enroll()
        {
            var team = SessionUtil.GetTeamFromFormsAuthentication();
            var t = SessionUtil.GetTournamentFromSession();
            var ta = SessionUtil.GetTournamentAssignmentFromSession();
            var a = SessionUtil.GetAssignmentFromSession();

            if (ta == null)
            {
                return RedirectToAction("Index", "Assignments");
            }

            // Register the enrollment
            var ae = new AssignmentEnrollment
            {
                IsActive = true,
                StartDate = DateTime.Now,
                Team = new Team { Id = team.Id },
                TournamentAssignment = new TournamentAssignment
                {
                    Id = ta.Id,
                    Tournament = new Tournament { Id = t.Id },
                    Assignment = new Assignment { Id = a.Id }

                }
            };

            ClientFacade.Instance.SaveAssignmentEnrollment(ae);

            return RedirectToAction("Index");
        }

        [Authorize]
        [HttpPost]
        public ActionResult UploadSubmit()
        {
            var team = SessionUtil.GetTeamFromFormsAuthentication();
            var ta = SessionUtil.GetTournamentAssignmentFromSession();
            var ae = SessionUtil.GetAssignmentEnrollmentFromSession();

            if (Request.Files.Count == 1)
            {
                HttpPostedFileBase postedFile = Request.Files[0];

                if (postedFile != null)
                {
                    TempData["SubmittedFileText"] = string.Format("File: {0} submitted", postedFile.FileName);

                    //Check for big file (> 128KB)
                    int submitMaxSize = int.Parse(ConfigurationManager.AppSettings["SubmitMaxSize"]) * 1024;

                    if (postedFile.ContentLength > submitMaxSize)
                    {
                        TempData["SubmittedFileText"] = string.Format("Submitted file is too large. Maximum size: {0}KB", submitMaxSize);
                        return RedirectToAction("Index");
                    }

                    //Check for right file extension
                    if (postedFile.FileName.IndexOf(".cs", StringComparison.Ordinal) != postedFile.FileName.Length - 3)
                    {
                        TempData["SubmittedFileText"] = "Submitted file doesn't have extension '.cs'.";
                        return RedirectToAction("Index");
                    }

                    //Construct the submit
                    var s = new Submit
                    {
                        FileName = Path.GetFileName(postedFile.FileName),
                        Team = team,
                        TournamentAssignment = ta,
                        AssignmentEnrollment = ae,
                        Data = new byte[postedFile.ContentLength]
                    };

                    postedFile.InputStream.Read(s.Data, 0, postedFile.ContentLength);

                    // Do the submit
                    try
                    {
                        ClientFacade.Instance.SaveSubmit(s);
                    }
                    catch (MoCSException e)
                    {
                        TempData["SubmittedFileText"] = "Unable to process submit. Reason: " + e.Message;
                    }
                }
            }

            return RedirectToAction("Index");
        }

        [Authorize]
        [HttpPost]
        public void Download()
        {
            TournamentAssignment ta = SessionUtil.GetTournamentAssignmentFromSession();
            Assignment a = SessionUtil.GetAssignmentFromSession();

            if (ta == null)
            {
                RedirectToAction("Index");
            }

            // Get the byte array with the file
            byte[] zipFile = ClientFacade.Instance.GetAssignmentZip(a);

            Response.ContentType = "application/octet-stream";

            // tell the browser to save rather than display inline
            Response.AddHeader("Content-Disposition", "attachment; filename=" + a.Name + ".zip");

            // tell the browser how big the file is
            Response.AddHeader("Content-Length", zipFile.Length.ToString(CultureInfo.InvariantCulture));

            // send the file to the browser
            Response.BinaryWrite(zipFile);

            // make sure response is sent
            Response.Flush();

            // end response
            Response.End();

        }

    }
}
