using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace WebApplication5.Controllers
{
    public class TestController : Controller
    {
        // GET: Test
        public ActionResult Index()
        {
            string url = HttpContext.Request.Url.AbsoluteUri;
            ViewBag.url = url;
            return View();
        }
    }
}