using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Microsoft.Ajax.Utilities;
using MoCS.Data.Entity;
using MoCS.Data.Repositories;
using MoCS.Web.Code;
using MoCS.Web.Models;

namespace MoCS.Web.Controllers
{
    [HandleError]
    [Authorize(Roles = "Admin")]
    public class AssignmentsController : Controller
    {
        private readonly UnitOfWork _unitOfWork = new UnitOfWork();

        //
        // GET: /Assignments/
        [HttpGet]
        public ActionResult Index()
        {
            var assignments = _unitOfWork.AssignmentsRepository.GetAll().ToList();
            return View(assignments);
        }

        [HttpGet]
        public ActionResult Edit(int id)
        {
            var assignment = _unitOfWork.AssignmentsRepository.SingleOrDefault(t => t.Id == id);
            if (assignment == null)
            {
                throw new MoCSHttpException(404, "Invalid assignment id. Try again, dearie.");
            }

            var model = new AssignmentModel(assignment);

            return View("AddOrEdit", model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(AssignmentModel assignmentModel)
        {
            // validate
            var assignment = _unitOfWork.AssignmentsRepository.SingleOrDefault(t => t.Id == assignmentModel.Id);
            if (assignment == null)
            {
                throw new MoCSHttpException(404, "Invalid assignment id. Try again, dearie.");
            }
            // Check if the chosen assignment name isn't already taken
            if (_unitOfWork.AssignmentsRepository.Any(t => t.Id != assignmentModel.Id && t.Name == assignmentModel.Name))
            {
                ModelState.AddModelError("Name", "That assignment name is already taken.");
            }

            if (!ModelState.IsValid)
            {
                return View("AddOrEdit", assignmentModel);
            }

            // apply the changes
            assignmentModel.UpdateAssignment(ref assignment);

            _unitOfWork.Save();

            return RedirectToAction("Index");

        }

        [HttpGet]
        public ActionResult Add()
        {
            var assignmentModel = new AssignmentModel();
            return View("AddOrEdit", assignmentModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Add(AssignmentModel assignmentModel)
        {
            // Check if the chosen assignment name isn't already taken
            if (_unitOfWork.AssignmentsRepository.Any(t => t.Id != assignmentModel.Id && t.Name == assignmentModel.Name))
            {
                ModelState.AddModelError("Name", "That assignment name is already taken.");
            }

            if (!ModelState.IsValid)
            {
                return View("AddOrEdit", assignmentModel);
            }

            // apply the changes
            var newAssignment = new Assignment();
            assignmentModel.UpdateAssignment(ref newAssignment);
            _unitOfWork.AssignmentsRepository.Add(newAssignment);

            _unitOfWork.Save();

            return RedirectToAction("Index");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(int id)
        {
            // validate
            var assignment = _unitOfWork.AssignmentsRepository.SingleOrDefault(t => t.Id == id);
            if (assignment == null)
            {
                throw new MoCSHttpException(404, "Invalid assignment id. Try again, dearie.");
            }

            // check for any enrollments
            if (assignment.TournamentAssignments.Any(ta => ta.AssignmentEnrollments.Any()))
            {
                throw new MoCSHttpException(403, "Cannot delete: there are enrollments for this assignment.");
            }

            _unitOfWork.TournamentAssignmentsRepository.DeleteRange(assignment.TournamentAssignments);
            _unitOfWork.AssignmentsRepository.Delete(assignment);
            _unitOfWork.Save();

            return RedirectToAction("Index");
        }
    }
}