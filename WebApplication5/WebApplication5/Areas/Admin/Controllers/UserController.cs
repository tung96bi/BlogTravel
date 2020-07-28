using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Helpers;
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
            return View(model);
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
                    if (!model.passNohass.Equals(user.passNohass))
                    {
                        model.password = Crypto.HashPassword(user.passNohass);
                        model.passNohass = user.passNohass;
                    }
                    model.quote = user.quote;
                    model.instagram = user.instagram;
                    model.facebook = user.facebook;
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

        public ActionResult ManageCategory()
        {
            var model = db.Categories.ToList();
            foreach (var item in model)
            {
                if (item.parentId.Equals("1"))
                {
                    item.displayName = "Main Category";
                }
                else
                {
                    item.displayName = "Subcategory";
                }
            }
            return View(model);
        }

        public ActionResult CreateCate()
        {
            return View();
        }

        [HttpPost]
        public ActionResult CreateCate(Category cat)
        {
            if (ModelState.IsValid)
            {
                if (cat.displayName.Equals("1"))
                {
                    cat.parentId = cat.displayName;
                }
                else
                {
                    cat.parentId = cat.displayName;
                }
                var count = db.Categories.ToList().Count;
                cat.id = (count + 1).ToString();
                db.Categories.Add(cat);
                db.SaveChanges();
                return RedirectToAction("ManageCategory");
            }
            else
            {
                return View(cat);
            }
        }

        public ActionResult EditCat(string id)
        {
            var model = db.Categories.Where(x => x.id.Equals(id)).SingleOrDefault();
            if (model.parentId.Equals("1"))
            {
                model.displayName = "Main Category";
            }
            else
            {
                model.displayName = "Subcategory";
            }
            return View(model);
        }

        [HttpPost]
        public ActionResult EditCat(Category cat)
        {
            if (ModelState.IsValid)
            {
                var model = db.Categories.Where(x => x.id.Equals(cat.id)).SingleOrDefault();
                model.CatName = cat.CatName;
                model.parentId = cat.displayName;
                db.SaveChanges();
                return RedirectToAction("ManageCategory");
            }
            else
            {
                return View(cat);
            }
        }

        [HttpPost]
        public ActionResult RemoveCat(string id)
        {
            var model = db.Categories.Where(x => x.id.Equals(id)).SingleOrDefault();
            db.Categories.Remove(model);
            db.SaveChanges();
            return Json(true, JsonRequestBehavior.AllowGet);
        }

        public ActionResult ManageUser()
        {
            var model = db.Dusers.ToList();
            return View(model);
        }

        public ActionResult CreateUser()
        {
            return View();
        }

        [HttpPost]
        public ActionResult CreateUser(Duser user, HttpPostedFileBase editImage)
        {
            if (ModelState.IsValid)
            {
                var duser = new Duser();
                if (editImage != null && editImage.ContentLength > 0)
                {
                    string filename = System.IO.Path.GetFileName(editImage.FileName);
                    string url = Server.MapPath("~/Uploads/" + filename);
                    editImage.SaveAs(Server.MapPath("~/Uploads/" + filename));
                    duser.imageUser = "/Uploads/" + filename;
                }
                duser.id = DateTime.Now.ToString("hhmmss");
                duser.isActive = user.isActive;
                duser.nickname = user.nickname;
                duser.password = Crypto.HashPassword(user.passNohass);
                duser.passNohass = user.passNohass;
                duser.quote = user.quote;
                duser.role = (int)user.role;
                duser.username = user.username;
                db.Dusers.Add(duser);
                db.SaveChanges();
                return RedirectToAction("ManageUser");
            }
            return View(user);
        }

        public ActionResult EditUser(string id)
        {
            var model = db.Dusers.Where(x => x.id.Equals(id)).SingleOrDefault();
            return View(model);
        }

        [HttpPost]
        public ActionResult EditUser(Duser user, HttpPostedFileBase editImage)
        {
            if (ModelState.IsValid)
            {
                var model = db.Dusers.Where(x => x.id.Equals(user.id)).SingleOrDefault();
                if (editImage != null && editImage.ContentLength > 0)
                {
                    string filename = System.IO.Path.GetFileName(editImage.FileName);
                    string url = Server.MapPath("~/Uploads/" + filename);
                    editImage.SaveAs(Server.MapPath("~/Uploads/" + filename));
                    model.imageUser = "/Uploads/" + filename;
                }
                model.isActive = user.isActive;
                model.nickname = user.nickname;
                if (!model.passNohass.Equals(user.passNohass))
                {
                    model.password = Crypto.HashPassword(user.passNohass);
                    model.passNohass = user.passNohass;
                }
                model.quote = user.quote;
                model.role = (int)user.role;
                model.username = user.username;
                db.SaveChanges();
                return RedirectToAction("ManageUser");
            }
            return View();
        }

        [HttpPost]
        public ActionResult RemoveUser(string id)
        {
            var model = db.Dusers.Where(x => x.id.Equals(id)).SingleOrDefault();
            var post = db.posts.Where(x => x.userId.Equals(model.id)).ToList();
            if (post.Count > 0)
            {
                return Json(false, JsonRequestBehavior.AllowGet);
            }
            else
            {
                db.Dusers.Remove(model);
                db.SaveChanges();
                return Json(true, JsonRequestBehavior.AllowGet);
            }
        }
    }
}