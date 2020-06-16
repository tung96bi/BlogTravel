using PagedList;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Text;
using System.Web;
using System.Web.Helpers;
using System.Web.Hosting;
using System.Web.Mvc;
using WebApplication5.Commom;
using WebApplication5.Models;

namespace WebApplication5.Controllers
{
    public class BlogController : Controller
    {
        private BlogEntities db = new BlogEntities();
        // GET: Blog
        public ActionResult Index()
        {
            var model = db.posts.ToList();
            return View(model);
        }

        //partial view menu
        public PartialViewResult Menu()
        {
            var model = db.Categories.ToList();
            return PartialView("Menu", model);
        }

        public ActionResult List(string id, int? page)
        {
            if (!id.Equals("1"))
            {
                var model = db.posts.Where(x => x.categoryId.Equals(id)).ToList();
                foreach (var item in model)
                {
                    if (item.RawContent.Length > 180)
                    {
                        item.RawContent = item.RawContent.Substring(0, 180);
                    }
                }
                int pageSize = 2;
                int pageNumber = (page ?? 1);
                return View(model.ToPagedList(pageNumber,pageSize));
            }
            return RedirectToAction("Index");
        }

        public ActionResult Detail(int id)
        {
            var model = db.posts.Where(x => x.id == id).SingleOrDefault();
            return View(model);
        }

        public ActionResult SignUp()
        {
            return View();
        }

        [HttpPost]
        public ActionResult SignUp(SignupVM sign)
        {
            if (ModelState.IsValid)
            {
                if (!checkExistUserName(sign.userName))
                {
                    ModelState.AddModelError("", "Username is already in used");
                    return View(sign);
                }
                else
                {
                    BuildEmailTemplate(sign.userName, sign.password);
                    ViewBag.Success = "Please check your email to complete registration";
                    return View();
                }
            }
            else
            {
                return View(sign);
            }
        }

        public void BuildEmailTemplate(string name, string pass)
        {
            var hash = Crypto.HashPassword(pass);
            Duser user = new Duser();
            user.username = name;
            user.password = hash;
            user.id = DateTime.Now.ToString("hhmmss");
            user.nickname = "Test";
            user.role = (int)Role.Type.Editor;
            db.Dusers.Add(user);
            db.SaveChanges();
            Session["User"] = user;
            string body = System.IO.File.ReadAllText(HostingEnvironment.MapPath("~/EmailTemplate/") + "Text" + ".cshtml");
            var url = "http://localhost:49868/Blog/Confirm";
            TempData["User"] = user;
            body = body.Replace("@ViewBag.ConfirmationLink", url);
            body = body.ToString();
            BuildEmailTemplate("Your Account Is Successfully Created", body, name);
        }

        public ActionResult Confirm()
        {
            var user = (Duser)TempData["User"];
            var model = db.Dusers.Where(x => x.username.Equals(user.username) && x.password.Equals(user.password)).SingleOrDefault();
            model.isActive = true;
            db.SaveChanges();
            Session["User"] = db.Dusers.Where(x => x.username.Equals(user.username) && x.password.Equals(user.password) && x.isActive == true).SingleOrDefault();
            return RedirectToAction("Index", "User", new { area = "Admin" });
        }

        public static void BuildEmailTemplate(string subjectText, string bodyText, string sendTo)
        {
            string from, to, bcc, cc, subject, body;
            from = "maixuantung1996@gmail.com";
            to = sendTo.Trim();
            bcc = "";
            cc = "";
            subject = subjectText;
            StringBuilder sb = new StringBuilder();
            sb.Append(bodyText);
            body = sb.ToString();
            MailMessage mail = new MailMessage();
            mail.From = new MailAddress(from,"Eden Blog");
            mail.To.Add(new MailAddress(to));
            if (!string.IsNullOrEmpty(bcc))
            {
                mail.Bcc.Add(new MailAddress(bcc));
            }
            if (!string.IsNullOrEmpty(cc))
            {
                mail.CC.Add(new MailAddress(cc));
            }
            mail.Subject = subject;
            mail.Body = body;
            mail.IsBodyHtml = true;
            SendEmail(mail);
        }

        public static void SendEmail(MailMessage mail)
        {
            SmtpClient client = new SmtpClient();
            client.Host = "smtp.gmail.com";
            client.Port = 587;
            client.EnableSsl = true;
            client.UseDefaultCredentials = false;
            client.DeliveryMethod = SmtpDeliveryMethod.Network;
            client.Credentials = new System.Net.NetworkCredential("maixuantung1996@gmail.com", "");
            try
            {
                client.Send(mail);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public ActionResult Logout()
        {
            Session["User"] = null;
            return RedirectToAction("Index");
        }

        public ActionResult Preview()
        {
            ViewBag.title = TempData["title"];
            ViewBag.content = TempData["content"];
            return View();
        }

        [HttpPost]
        public ActionResult CountView(string id)
        {
           if(id != null)
            {
                var a = Int32.Parse(id);
                var model = db.posts.Where(x => x.id == a).SingleOrDefault();
                if (model.postView == null)
                {
                    model.postView = 0;
                    model.postView += 1;
                }
                else
                {
                    model.postView += 1;
                }
                db.SaveChanges();
            }
            return Json(true, JsonRequestBehavior.AllowGet);
        }

        public ActionResult SignIn()
        {
            return View();
        }

        [HttpPost]
        public ActionResult SignIn(SignupVM signIn)
        {
            if (ModelState.IsValid)
            {
                var model = db.Dusers.Where(x => x.username.Equals(signIn.userName) && x.isActive == true).SingleOrDefault();
                if (Crypto.VerifyHashedPassword(model.password, signIn.password))
                {
                    Session["User"] = model;
                    return RedirectToAction("Index", "User", new { area = "Admin" });
                }
            }
            return View();
        }

        public bool checkExistUserName(string userName)
        {
            var model = db.Dusers.Where(x => x.username.Equals(userName)).SingleOrDefault();
            if(model == null)
            {
                return true;
            }
            return false;
        }
    }
}