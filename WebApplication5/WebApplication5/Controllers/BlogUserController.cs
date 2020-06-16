using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebApplication5.Models;

namespace WebApplication5.Controllers
{
    public class BlogUserController : Controller
    {
        private BlogEntities db = new BlogEntities();
        // GET: BlogUser
        public ActionResult Index(string id)
        {
            var model = db.Dusers.Where(x => x.id.Equals(id)).SingleOrDefault();
            return View(model);
        }
    }
}