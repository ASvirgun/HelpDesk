using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using HelpDeskTrain.Models;
using System.Data.Entity;

namespace HelpDeskTrain.Controllers
{
    [Authorize(Roles = "Администратор")]
    public class ServiceController : Controller
    {
        private HelpdeskContext db = new HelpdeskContext();
        #region Departments
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
#endregion
        #region Activ
        [HttpGet]
        public ActionResult Activ()
        {
            ViewBag.Activs = db.Activs.Include(s => s.Department);
            ViewBag.Departments = new SelectList(db.Departments, "Id", "Name");
            return View();
        }

        [HttpPost]
        public ActionResult Activ(Activ activ)
        {
            if (ModelState.IsValid)
            {
                db.Activs.Add(activ);
                db.SaveChanges();
            }

            ViewBag.Activs = db.Activs.Include(s => s.Department);
            ViewBag.Departments = new SelectList(db.Departments, "Id", "Name");
            return View(activ);
        }

        public ActionResult DeleteActiv(int id)
        {
            Activ activ = db.Activs.Find(id);
            db.Activs.Remove(activ);
            db.SaveChanges();
            return RedirectToAction("Activ");
        }
        #endregion

        #region Category

        [HttpGet]
        public ActionResult Categories()
        {
            ViewBag.Categories = db.Categories;
            return View();
        }

        [HttpPost]
        public ActionResult Categories(Category category)
        {
            if (ModelState.IsValid)
            {
                db.Categories.Add(category);
                db.SaveChanges();
            }
            ViewBag.Categories = db.Categories;
            return View(category);
        }

        public ActionResult DeleteCategory(int id)
        {
            Category category = db.Categories.Find(id);
            db.Categories.Remove(category);
            db.SaveChanges();
            return RedirectToAction("Categories");
        }
        #endregion

    }
}