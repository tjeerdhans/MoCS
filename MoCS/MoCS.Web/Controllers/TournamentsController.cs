using MoCS.Data.Entity;
using MoCS.Data.Repositories;
using MoCS.Web.Code;
using MoCS.Web.Models;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Web.Mvc;

namespace MoCS.Web.Controllers
{
    [HandleError]
    public class TournamentsController : Controller
    {
        private readonly UnitOfWork _unitOfWork = new UnitOfWork();
        //
        // GET: /Tournaments/
        public ActionResult Index()
        {
            var model = _unitOfWork.TournamentsRepository.GetAll().ToList();
            return View(model);
        }

        [HttpGet]
        [Authorize(Roles = "Admin")]
        public ActionResult Edit(int id)
        {
            var tournament = _unitOfWork.TournamentsRepository.SingleOrDefault(t => t.Id == id);
            if (tournament == null)
            {
                throw new MoCSHttpException(404, "Invalid tournament id. Try again, dearie.");
            }

            var model = new TournamentModel(tournament);
            model.FillAssignmentSelectListItems(_unitOfWork.AssignmentsRepository.GetAll().ToList());

            return View("AddOrEdit", model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public ActionResult Edit(TournamentModel tournamentModel)
        {
            // validate
            var tournament = _unitOfWork.TournamentsRepository.SingleOrDefault(t => t.Id == tournamentModel.Id);
            if (tournament == null)
            {
                throw new MoCSHttpException(404, "Invalid tournament id. Try again, dearie.");
            }
            // Check if the chosen tournament name isn't already taken
            if (_unitOfWork.TournamentsRepository.Any(t => t.Id != tournamentModel.Id && t.Name == tournamentModel.Name))
            {
                ModelState.AddModelError("Name", "That tournament name is already taken.");
            }


            if (!ModelState.IsValid)
            {
                tournamentModel.FillAssignmentSelectListItems(_unitOfWork.AssignmentsRepository.GetAll().ToList());
                return View("AddOrEdit", tournamentModel);
            }

            // apply the changes
            tournament.Name = tournamentModel.Name;

            // deactivate assignments that are no longer in the tournament
            var tournamentAssignmentsToDelete = new List<TournamentAssignment>();
            foreach (var tournamentAssignment in tournament.TournamentAssignments.OrderBy(ta => ta.AssignmentOrder))
            {
                if (!tournamentModel.AssignmentIds.Contains(tournamentAssignment.Assignment.Id.ToString(CultureInfo.InvariantCulture)))
                {
                    tournamentAssignment.IsActive = false;
                    // if the tournamentassignment doesn't have any active enrollments, delete it completely.
                    if (!tournamentAssignment.AssignmentEnrollments.Any())
                    {
                        tournamentAssignmentsToDelete.Add(tournamentAssignment);
                    }
                }
            }

            // add new assignments and apply ordering
            var order = 0;
            foreach (var assignmentId in tournamentModel.AssignmentIds)
            {
                var id = int.Parse(assignmentId);
                var assignment = _unitOfWork.AssignmentsRepository.Single(a => a.Id == id);
                // check if the assignment is already in the tournament
                var tournamentAssignment = tournament.TournamentAssignments.SingleOrDefault(ta => ta.Assignment.Id == id);
                if (tournamentAssignment == null)
                {
                    tournamentAssignment = new TournamentAssignment
                    {
                        Assignment = assignment,
                        Tournament = tournament,
                        CreateDateTime = DateTime.UtcNow,
                        IsActive = true,
                        AssignmentOrder = 0
                    };
                    tournament.TournamentAssignments.Add(tournamentAssignment);
                }
                tournamentAssignment.AssignmentOrder = order;
                order++;
            }

            _unitOfWork.TournamentAssignmentsRepository.DeleteRange(tournamentAssignmentsToDelete);

            // save
            _unitOfWork.Save();

            return RedirectToAction("Index");
        }

        [HttpGet]
        [Authorize(Roles = "Admin")]
        public ActionResult Add()
        {
            var tournamentModel = new TournamentModel();
            tournamentModel.FillAssignmentSelectListItems(_unitOfWork.AssignmentsRepository.GetAll().OrderBy(a => a.Name).ToList());
            return View("AddOrEdit", tournamentModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public ActionResult Add(TournamentModel tournamentModel)
        {
            //validate
            // Check if the chosen tournament name isn't already taken
            if (_unitOfWork.TournamentsRepository.Any(t => t.Id != tournamentModel.Id && t.Name == tournamentModel.Name))
            {
                ModelState.AddModelError("Name", "That tournament name is already taken.");
            }

            if (!ModelState.IsValid)
            {
                tournamentModel.FillAssignmentSelectListItems(_unitOfWork.AssignmentsRepository.GetAll().ToList());
                return View("AddOrEdit", tournamentModel);
            }

            var newTournament = new Tournament
            {
                Name = tournamentModel.Name,
                CreateDateTime = DateTime.UtcNow
            };

            // add new assignments and apply ordering
            var order = 0;
            foreach (var assignmentId in tournamentModel.AssignmentIds)
            {
                var id = int.Parse(assignmentId);
                var assignment = _unitOfWork.AssignmentsRepository.Single(a => a.Id == id);

                var tournamentAssignment = new TournamentAssignment
                 {
                     Assignment = assignment,
                     Tournament = newTournament,
                     CreateDateTime = DateTime.UtcNow,
                     IsActive = true,
                     AssignmentOrder = 0
                 };
                newTournament.TournamentAssignments.Add(tournamentAssignment);

                tournamentAssignment.AssignmentOrder = order;
                order++;
            }
            _unitOfWork.TournamentsRepository.Add(newTournament);

            // save
            _unitOfWork.Save();

            return RedirectToAction("Index");
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(int id)
        {
            // validate
            var tournament = _unitOfWork.TournamentsRepository.SingleOrDefault(t => t.Id == id);
            if (tournament == null)
            {
                throw new MoCSHttpException(404, "Invalid tournament id. Try again, dearie.");
            }

            // check for active enrollments
            if (tournament.TournamentAssignments.Any(ta => ta.AssignmentEnrollments.Any(ae => ae.IsActive)))
            {
                throw new MoCSHttpException(403, "Cannot delete: there are active enrollments for this tournament.");
            }

            // remove any submits
            _unitOfWork.SubmitsRepository.DeleteAllForTournament(tournament);

            // remove the enrollments
            _unitOfWork.AssignmentEnrollmentRepository.DeleteAllForTournament(tournament);

            // remove the tournamentassignments
            _unitOfWork.TournamentAssignmentsRepository.DeleteRange(tournament.TournamentAssignments.ToList());

            // remove the tournament
            _unitOfWork.TournamentsRepository.Delete(tournament);
            _unitOfWork.Save();

            return RedirectToAction("Index");
        }
    }
}