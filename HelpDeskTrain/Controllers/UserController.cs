using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data;
using System.Linq;
using System.Security.Cryptography;
using System.Web;
using System.Web.Mvc;
using HelpDeskTrain.Models;

namespace HelpDeskTrain.Controllers
{
    [Authorize(Roles = "Администратор, Модератор, Исполнитель")]
    public class UserController : Controller
    {
        private HelpdeskContext db = new HelpdeskContext();
     
        [HttpGet]
        public ActionResult Index()
        {
            var users = db.Users.Include(u => u.Department).Include(u => u.Role).ToList();

            List<Department> departments = db.Departments.ToList();
            departments.Insert(0, new Department{Name = "All", Id = 0});
            ViewBag.Departments = new SelectList(departments, "Id", "Name");

            List<Role> roles = db.Roles.ToList();
            roles.Insert(0, new Role{Id = 0, Name = "All"});
            ViewBag.Roles = new SelectList(roles, "Id", "Name");

            return View(users);
        }

        [HttpPost]
        public ActionResult Index(int department, int role)
        {
            IEnumerable<User> allUsers = null;

            if (department == 0 && role == 0)
                return RedirectToAction("Index");
            if (role == 0 && department != 0)
            {
                allUsers = from user in db.Users.Include(u => u.Department).Include(u => u.Role)
                    where user.DepartmentId == department
                    select user;
            }
            else if (role != 0 && department == 0)
            {
                allUsers = from user in db.Users.Include(u => u.Department).Include(u => u.Role)
                    where user.RoleId == role
                    select user;

            }
            else if (role != 0 && department != 0)
            {
                allUsers = from user in db.Users.Include(u => u.Department).Include(u => u.Role)
                           where user.RoleId == role && user.DepartmentId == department
                           select user;
            }

            List<Department> departments = db.Departments.ToList();
            departments.Insert(0, new Department { Name = "All", Id = 0 });
            ViewBag.Departments = new SelectList(departments, "Id", "Name");

            List<Role> roles = db.Roles.ToList();
            roles.Insert(0, new Role { Id = 0, Name = "All" });
            ViewBag.Roles = new SelectList(roles, "Id", "Name");

            return View(allUsers.ToList());
        }

        [Authorize(Roles = "Администратор")]
        [HttpGet]
        public ActionResult Create()
        {
            ViewBag.Departments = new SelectList(db.Departments,"Id","Name");
            ViewBag.Roles = new SelectList(db.Roles, "Id", "Name");
            return View();
        }

        [Authorize(Roles = "Администратор")]
        [HttpPost]
        public ActionResult Create(User user)
        {
            if (ModelState.IsValid)
            {
                db.Users.Add(user);
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.Departments = new SelectList(db.Departments, "Id", "Name");
            ViewBag.Roles = new SelectList(db.Roles, "Id", "Name");
            return View(user);
        }

        [Authorize(Roles = "Администратор")]
        [HttpGet]
        public ActionResult Edit(int id)
        {
            var user = db.Users.Find(id);
            ViewBag.Departments = new SelectList(db.Departments, "Id", "Name");
            ViewBag.Roles = new SelectList(db.Roles, "Id", "Name");
            return View(user);
        }

        [Authorize(Roles = "Администратор")]
        [HttpPost]
        public ActionResult Edit(User user)
        {
            if (ModelState.IsValid)
            {
                db.Entry(user).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.Departments = new SelectList(db.Departments, "Id", "Name");
            ViewBag.Roles = new SelectList(db.Roles, "Id", "Name");
            return View(user);
        }

        [Authorize(Roles = "Администратор")]
        [HttpGet]
        public ActionResult Delete(int id)
        {
            var user = db.Users.Find(id);
            db.Users.Remove(user);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

    }
}