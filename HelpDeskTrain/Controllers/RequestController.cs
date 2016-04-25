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

        public ActionResult Details(int id)
        {
            Request request = db.Requests.Find(id);
            if (request != null)
            {
                var activ = db.Activs.Where(m => m.Id == request.ActivId);
                if (activ.Any())
                {
                    request.Activ = activ.First();
                }
                request.Category = db.Categories.First(m => m.Id == request.CategoryId);
                return PartialView("_Detalis", request);
            }
            return View("Index");
        }

        public ActionResult Executor(int id)
        {
            var executor = db.Users.Find(id);
            if (executor != null)
            {
                return PartialView("_Executor", executor);
            }
            return View("Index");
        }

        public ActionResult Lifecycle(int id)
        {
            var lifecycle = db.Lifecycles.Find(id);
            if (lifecycle != null)
            {
                return PartialView("_Lifecycle", lifecycle);
            }
            return View("Index");
        }

        public ActionResult Delete(int id)
        {
            var request = db.Requests.Find(id);
            var user = db.Users.FirstOrDefault(x => x.Login == HttpContext.User.Identity.Name);
            if (user != null && (request != null && request.UserId == user.Id))
            {
                var lifecycle = db.Lifecycles.FirstOrDefault(x => x.Id == request.LifecycleId);
                db.Lifecycles.Remove(lifecycle);
                db.SaveChanges();
            }
            return View("Index");
        }

        public ActionResult Download(int id)
        {
            Request r = db.Requests.Find(id);
            if (r != null)
            {
                string filename = Server.MapPath("~/Files/" + r.File);
                string contentType = "image/jpeg";

                string ext = filename.Substring(filename.LastIndexOf('.'));
                switch (ext)
                {
                    case "txt":
                        contentType = "text/plain";
                        break;
                    case "png":
                        contentType = "image/png";
                        break;
                    case "tiff":
                        contentType = "image/tiff";
                        break;
                }
                return File(filename, contentType, filename);
            }

            return Content("Файл не найден");
        }

        [Authorize(Roles = "Администратор")]
        public ActionResult RequestList()
        {
            var requests = db.Requests.Include(r => r.Category)
                                    .Include(r => r.Lifecycle)
                                    .Include(r => r.User)
                                    .OrderByDescending(r => r.Lifecycle.Opened);

            return View(requests.ToList());
        }
    }
}