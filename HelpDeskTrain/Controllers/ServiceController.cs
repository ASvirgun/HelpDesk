using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using HelpDeskTrain.Models;

namespace HelpDeskTrain.Controllers
{
    [Authorize(Roles = "Администратор")]
    public class ServiceController : Controller
    {
        private HelpdeskContext db = new HelpdeskContext();

        [HttpGet]
        public ActionResult Department()
        {
            ViewBag.Departments = db.Departments;
            return View();
        }

        [HttpPost]
        public ActionResult Department(Department depo)
        {
            if (ModelState.IsValid)
            {
                db.Departments.Add(depo);
                db.SaveChanges();
            }
            ViewBag.Departments = db.Departments;
            return View(depo);
        }

        public ActionResult DeleteDepartment(int id)
        {
            var depo = db.Departments.Find(id);
            db.Departments.Remove(depo);
            db.SaveChanges();
            return RedirectToAction("Departments");
        }
    }
}