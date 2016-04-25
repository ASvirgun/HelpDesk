using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using HelpDeskTrain.Models;

namespace HelpDeskTrain.Controllers
{
    [Authorize]
    public class RequestController : Controller
    {

        HelpdeskContext db = new HelpdeskContext();
        public ActionResult Index()
        {
            var user = db.Users.FirstOrDefault(x => x.Login == HttpContext.User.Identity.Name);
            var requests = db.Requests.Where(r => r.UserId == user.Id)
                .Include(r => r.Category)
                .Include(r => r.Lifecycle)
                .Include(r => r.User)
                .OrderByDescending(r => r.Lifecycle.Opened);
            return View(requests.ToList());
        }

        [HttpGet]
        public ActionResult Create()
        {
            User user = db.Users.FirstOrDefault(x => x.Login == HttpContext.User.Identity.Name);
            if (user != null)
            {
                var cabs = from cab in db.Activs
                    where cab.Department == user.Department
                    select cab;
                ViewBag.Cabs = new SelectList(cabs, "Id", "CabNumber");
                ViewBag.Categories = new SelectList(db.Categories, "Id", "Name");
                return View();
            }
            return RedirectToAction("LogOff", "Account");
        }

        [HttpPost]
        public ActionResult Create(Request request, HttpPostedFileBase error)
        {
            User user = db.Users.FirstOrDefault(x => x.Login == HttpContext.User.Identity.Name);
            if (user == null)
            {
                return RedirectToAction("LogOff", "Account");
            }
            if (ModelState.IsValid)
            {
                request.Status = (int)RequestStatus.Open;
                var lifeCycle = new Lifecycle(){Opened = DateTime.Now};
                db.Lifecycles.Add(lifeCycle);
                request.UserId = user.Id;
                if (error != null)
                {
                    string ext = error.FileName.Substring(error.FileName.LastIndexOf('.'));
                    string path = DateTime.Now.ToString("dd/MM/yyyy H:mm:ss").Replace(":", "_").Replace("/", ".") + ext;
                    error.SaveAs(Server.MapPath("~/Files/" + path));
                    request.File = path;
                }
                db.Requests.Add(request);
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(request);
        }
    }
}