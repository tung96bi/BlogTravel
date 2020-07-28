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
                if(user.role == (int)Commom.Role.Type.Admin)
                {
                    p.isActive = (int)Commom.PostStatus.Type.Approved;
                }
                else
                {
                    var notify = new NotificationBlog();
                    notify.addOn = DateTime.Now;
                    notify.notiType = (int)Commom.Notify.Type.Primary;
                    notify.userID = db.Dusers.Where(x => x.role == (int)Commom.Role.Type.Admin).SingleOrDefault().id;
                    notify.notiName = "A post " + p.title + " are waiting for your aprroval";
                    db.NotificationBlogs.Add(notify);
                    p.isActive = (int)Commom.PostStatus.Type.Pending;
                }
                
                db.posts.Add(p);
                db.SaveChanges();
                if (user.role == (int)Commom.Role.Type.Editor)
                {
                    return RedirectToAction("MyPost", new { id = p.userId });
                }
                else
                {
                    return RedirectToAction("ManagePost");
                }
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
                if (!string.IsNullOrEmpty(item.RawContent))
                {
                    if (item.RawContent.Length > 200)
                    {
                        item.RawContent = item.RawContent.Substring(0, 200);
                    }
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
                var user = (Duser)Session["User"];
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
                model.categoryId = p.categoryId;
                db.SaveChanges();
                if(user.role == (int)Commom.Role.Type.Editor)
                {
                    return RedirectToAction("MyPost", new { id = model.userId });
                }
                else
                {
                    return RedirectToAction("ManagePost");
                }
                
            }
            catch (Exception)
            {
                return View(p);
                throw;

            }
        }

        public ActionResult ManagePost()
        {
            var model = db.posts.ToList();
            var list = new List<ManagePostVM>();
            foreach(var item in model)
            {
                var post = new ManagePostVM();
                post.categoryId = item.categoryId;
                post.categoryName = db.Categories.Where(x => x.id.Equals(item.categoryId)).SingleOrDefault().CatName;
                post.content = item.content;
                post.createOn = item.createOn;
                post.id = item.id;
                post.image = item.image;
                post.isActive = item.isActive;
                post.postLike = item.postLike;
                post.postView = item.postView;
                post.RawContent = item.RawContent;
                post.tag = item.tag;
                post.title = item.title;
                post.userId = item.userId;
                list.Add(post);
            }
            return View(list);
        }

        public ActionResult Remove(int id)
        {
            var model = db.posts.Where(x => x.id == id).SingleOrDefault();
            db.posts.Remove(model);
            db.SaveChanges();
            return Json(true, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult AprrovePost(int id)
        {
            var model = db.posts.Where(x => x.id == id).SingleOrDefault();
            model.isActive = (int)Commom.PostStatus.Type.Approved;
            var notify = new NotificationBlog();
            notify.notiType = (int)Commom.Notify.Type.Primary;
            notify.addOn = DateTime.Now;
            notify.notiName = "Your Post " + model.title + " has been approved";
            notify.userID = model.userId;
            db.NotificationBlogs.Add(notify);
            db.SaveChanges();
            return Json(true, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult RejectPost(int id)
        {
            var model = db.posts.Where(x => x.id == id).SingleOrDefault();
            model.isActive = (int)Commom.PostStatus.Type.NotApproved;
            var notify = new NotificationBlog();
            notify.notiType = (int)Commom.Notify.Type.Primary;
            notify.addOn = DateTime.Now;
            notify.notiName = "Your Post " + model.title + " has been rejected";
            notify.userID = model.userId;
            db.NotificationBlogs.Add(notify);
            db.SaveChanges();
            return Json(true, JsonRequestBehavior.AllowGet);
        }
    }
}