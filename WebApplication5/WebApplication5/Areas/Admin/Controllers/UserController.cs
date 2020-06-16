using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebApplication5.Models;

namespace WebApplication5.Areas.Admin.Controllers
{
    public class UserController : Controller
    {
        private BlogEntities db = new BlogEntities();
        // GET: Admin/User
        public ActionResult Index()
        {
            var user = (Duser)Session["User"];
            var model = db.Dusers.Where(x => x.username.Equals(user.username)).SingleOrDefault();
            ViewBag.image = model.imageUser;
            return View(user);
        }

        [HttpPost]
        public ActionResult Index(Duser user, HttpPostedFileBase upload_image)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    var model = db.Dusers.Where(x => x.username.Equals(user.username)).SingleOrDefault();
                    if (upload_image != null && upload_image.ContentLength > 0)
                    {
                        string filename = System.IO.Path.GetFileName(upload_image.FileName);
                        string url = Server.MapPath("~/Uploads/" + filename);
                        upload_image.SaveAs(Server.MapPath("~/Uploads/" + filename));
                        model.imageUser = "/Uploads/" + filename;
                    }
                    model.nickname = user.nickname;
                    model.password = user.password;
                    model.quote = user.quote;
                    db.SaveChanges();
                    ViewBag.Update = "Update Profile Sucessfull";
                    return View();
                }
                catch (Exception)
                {
                    throw;
                }
            }
            else
            {
                return View(user);
            }
        }

        public ActionResult LogOut()
        {
            Session["User"] = null;
            return RedirectToAction("Index", "Blog", new { area = "" });
        }
    }
}