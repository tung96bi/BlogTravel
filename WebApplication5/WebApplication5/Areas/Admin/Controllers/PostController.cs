using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebApplication5.Models;

namespace WebApplication5.Areas.Admin.Controllers
{
    public class PostController : Controller
    {
        private BlogEntities db = new BlogEntities();
        // GET: Admin/Post
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateInput(false)]
        public ActionResult Create(post post, HttpPostedFileBase image)
        {
            var userSession = (Duser)Session["User"];
            var user = db.Dusers.Where(x => x.username.Equals(userSession.username)).SingleOrDefault();
            try
            {
                post p = new post();
                if (image != null && image.ContentLength > 0)
                {
                    string filename = System.IO.Path.GetFileName(image.FileName);
                    string url = Server.MapPath("~/Uploads/" + filename);
                    image.SaveAs(Server.MapPath("~/Uploads/" + filename));
                    p.image = "/Uploads/" + filename;
                }
                p.title = post.title;
                p.content = post.content;
                p.userId = user.id;
                p.categoryId = post.categoryId;
                p.createOn = DateTime.Now.ToString();
                p.RawContent = post.RawContent;
                db.posts.Add(p);
                db.SaveChanges();
                return View();
            }
            catch (Exception)
            {
                return View(post);
                throw;
                
            }
        }

        [HttpPost]
        [ValidateInput(false)]
        public ActionResult SetDataPreview(string title, string content)
        {
            TempData["title"] = title;
            TempData["content"] = content;
            return Json(true, JsonRequestBehavior.AllowGet);
        }

        public ActionResult Preview()
        {
            var a = TempData["title"];
            var b = TempData["content"];
            TempData["t"] = a;
            TempData["c"] = b;
            return Redirect("/Blog/Preview");
        }

        public ActionResult MyPost(string id)
        {
            var model = db.posts.Where(x => x.userId.Equals(id)).OrderByDescending(x=>x.createOn).ToList();
            foreach(var item in model)
            {
                if(item.content.Length > 200)
                {
                    item.content = item.content.Substring(0, 200);
                }
            }
            return View(model);
        }

        public ActionResult EditPost(int id)
        {
            var model = db.posts.Where(x => x.id == id).SingleOrDefault();
            return View(model);
        }

        [HttpPost]
        [ValidateInput(false)]
        public ActionResult EditPost(post p, HttpPostedFileBase editImage)
        {
            try
            {
                var model = db.posts.Where(x => x.id == p.id).SingleOrDefault();
                if (editImage != null && editImage.ContentLength > 0)
                {
                    string filename = System.IO.Path.GetFileName(editImage.FileName);
                    string url = Server.MapPath("~/Uploads/" + filename);
                    editImage.SaveAs(Server.MapPath("~/Uploads/" + filename));
                    model.image = "/Uploads/" + filename;
                }
                model.title = p.title;
                model.content = p.content;
                model.RawContent = p.RawContent;
                db.SaveChanges();
                return RedirectToAction("MyPost", new { id = model.userId});
            }
            catch (Exception)
            {
                return View(p);
                throw;

            }
        }

        public ActionResult ManagePost()
        {
            return View();
        }

        public ActionResult Remove(int id)
        {
            var model = db.posts.Where(x => x.id == id).SingleOrDefault();
            db.posts.Remove(model);
            db.SaveChanges();
            return Json(true, JsonRequestBehavior.AllowGet);
        }
    }
}