using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Text;
using System.Web;
using System.Web.Hosting;
using System.Web.Mvc;
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
            return PartialView("Menu",model);
        }

        public ActionResult List(string id)
        {
            var model = db.posts.Where(x => x.categoryId.Equals(id)).ToList();
            foreach(var item in model)
            {
                item.content = item.content.Substring(1, 180);
            }
            return View(model);
        }

        public ActionResult Detail(int id)
        {
            var model = db.posts.Where(x => x.id == id ).SingleOrDefault();
            return View(model);
        }

        public ActionResult SignUp()
        {
            return View();
        }

        //[HttpPost]
        //public ActionResult SignUp(SignupVM sign)
        //{
        //    if (ModelState.IsValid)
        //    {
        //        BuildEmailTemplate(sign);
        //    }
        //    return RedirectToAction("Index");
        //}

        public JsonResult BuildEmailTemplate(string name, string pass)
        {
            Duser user = new Duser();
            user.username = name;
            user.password = pass;
            user.id = DateTime.Now.ToString("hhmmss");
            user.nickname = "Test";
            user.role = "2";
            db.Dusers.Add(user);
            db.SaveChanges();
            Session["User"] = user;
            string body = System.IO.File.ReadAllText(HostingEnvironment.MapPath("~/EmailTemplate/") + "Text" + ".cshtml");
            var url = "http://localhost:49868/Blog/Confirm";
            body = body.Replace("@ViewBag.ConfirmationLink", url);
            body = body.ToString();
            BuildEmailTemplate("Your Account Is Successfully Created", body, name);
            return Json(true, JsonRequestBehavior.AllowGet);
        }

        public ActionResult Confirm()
        {
            var user = (Duser)Session["User"];
            var model = db.Dusers.Where(x => x.username.Equals(user.username)).SingleOrDefault();
            model.isActive = true;
            db.SaveChanges();
            return View();
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
            mail.From = new MailAddress(from);
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
            client.Credentials = new System.Net.NetworkCredential("maixuantung1996@gmail.com", "nhocvip96");
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
    }
}