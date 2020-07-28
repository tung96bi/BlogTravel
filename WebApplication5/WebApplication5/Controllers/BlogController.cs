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
        public ActionResult Index(int? page)
        {
            var model = db.posts.Where(x=>x.isActive ==(int)Commom.PostStatus.Type.Approved).ToList();
            foreach (var item in model)
            {
                if (!string.IsNullOrEmpty(item.RawContent))
                {
                    item.RawContent = item.RawContent.Substring(0, 180);
                }
            }
            int pageSize = 5;
            int pageNumber = (page ?? 1);
            return View(model.ToPagedList(pageNumber, pageSize));
        }

        public ActionResult Home()
        {
            return RedirectToAction("Index");
        }

        public ActionResult About()
        {
            var model = db.Dusers.Where(x => x.role == (int)Commom.Role.Type.Admin).SingleOrDefault();
            return View(model);
        }

        public ActionResult Contact()
        {
            return View();
        }

        //partial view menu
        public PartialViewResult Menu()
        {
            var model = db.Categories.ToList();
            return PartialView("Menu", model);
        }

        public ActionResult List(string id, int? page)
        {
            var model1 = db.Categories.Where(x => x.id == id).SingleOrDefault();
            
            if (!id.Equals("1") && model1.parentId.Equals("0"))
            {
                var aprroved = (int)Commom.PostStatus.Type.Approved;
                var model = db.posts.Where(x => x.categoryId.Equals(id) && x.isActive == aprroved).ToList();
                foreach (var item in model)
                {
                    if (!string.IsNullOrEmpty(item.RawContent))
                    {
                        if (item.RawContent.Length > 180)
                        {
                            item.RawContent = item.RawContent.Substring(0, 180);
                        }
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
            ViewBag.PostUser = db.Dusers.Where(x => x.id.Equals(model.userId)).SingleOrDefault();
            string url = HttpContext.Request.Url.AbsoluteUri;
            ViewBag.url = url;
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
                    return RedirectToAction("Confirm");
                }
            }
            else
            {
                return View(sign);
            }
        }


        public void BuildEmailTemplate(string name, string pass)
        {
            var chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
            var stringChars = new char[6];
            var random = new Random();

            for (int i = 0; i < stringChars.Length; i++)
            {
                stringChars[i] = chars[random.Next(chars.Length)];
            }

            var finalString = new String(stringChars);
            var hash = Crypto.HashPassword(pass);
            Duser user = new Duser();
            user.activekey = finalString;
            user.username = name;
            user.passNohass = pass;
            user.password = hash;
            user.id = DateTime.Now.ToString("hhmmss");
            user.nickname = "Test";
            user.role = (int)Role.Type.Editor;
            db.Dusers.Add(user);
            db.SaveChanges();
            string body = System.IO.File.ReadAllText(HostingEnvironment.MapPath("~/EmailTemplate/") + "Text" + ".cshtml");
            var url = finalString;
            TempData["User"] = user;
            body = body.Replace("@ViewBag.ConfirmationLink", url);
            body = body.ToString();
            BuildEmailTemplate("Your Account Is Successfully Created", body, name);
        }

        public ActionResult Confirm()
        {
            //var user = (Duser)TempData["User"];
            //var model = db.Dusers.Where(x => x.username.Equals(user.username) && x.password.Equals(user.password)).SingleOrDefault();
            //model.isActive = true;
            //db.SaveChanges();
            //Session["User"] = db.Dusers.Where(x => x.username.Equals(user.username) && x.password.Equals(user.password) && x.isActive == true).SingleOrDefault();
            //return RedirectToAction("Index", "User", new { area = "Admin" });

            return View();
        }

        [HttpPost]
        public ActionResult Confirm(string activeCode, string id)
        {
            var model = db.Dusers.Where(x => x.id.Equals(id)).SingleOrDefault();
            if (model.activekey.Equals(activeCode))
            {
                model.isActive = true;
                db.SaveChanges();
                Session["User"] = model;
                return RedirectToAction("Index", "User", new { area = "Admin" });
            }
            else
            {
                ViewBag.ActiveError = "Your active code is not correct";
                //ModelState.AddModelError("", "Your active code is not correct");
                return View();
            }
        }

        public ActionResult ResendEmail(string id)
        {
            var chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
            var stringChars = new char[6];
            var random = new Random();

            for (int i = 0; i < stringChars.Length; i++)
            {
                stringChars[i] = chars[random.Next(chars.Length)];
            }

            var finalString = new String(stringChars);
            var model = db.Dusers.Where(x => x.id.Equals(id)).SingleOrDefault();
            model.activekey = finalString;
            db.SaveChanges();
            string body = System.IO.File.ReadAllText(HostingEnvironment.MapPath("~/EmailTemplate/") + "Text" + ".cshtml");
            var url = finalString;
            body = body.Replace("@ViewBag.ConfirmationLink", url);
            body = body.ToString();
            BuildEmailTemplate("Your Account Is Successfully Created", body, model.username);
            return Json(true, JsonRequestBehavior.AllowGet);
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
                //await client.SendMailAsync(mail);
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
                if(model != null)
                {
                    if (Crypto.VerifyHashedPassword(model.password, signIn.password))
                    {
                        Session["User"] = model;
                        return RedirectToAction("Index", "User", new { area = "Admin" });
                    }
                }
                ModelState.AddModelError("","Email or Password is incorrect");
                return View(signIn);
            }
            else
            {
                return View(signIn);
            }
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

        public ActionResult Search(string postSearch, int? page)
        {
            if (!string.IsNullOrEmpty(postSearch))
            {
                var model = db.posts.Where(x => x.title.Contains(postSearch)).ToList();
                if( model.Count != 0)
                {
                    foreach (var item in model)
                    {
                        if (!string.IsNullOrEmpty(item.RawContent))
                        {
                            item.RawContent = item.RawContent.Substring(0, 100);
                        }
                    }
                    int pageSize = 5;
                    int pageNumber = (page ?? 1);
                    return View(model.ToPagedList(pageNumber, pageSize));
                }
                else
                {
                    int pageSize = 5;
                    int pageNumber = (page ?? 1);
                    ViewBag.Cantfind = "Opp, no post matching your found";
                    return View(model.ToPagedList(pageNumber, pageSize));
                }
            }
            return RedirectToAction("Index");
        }

        [HttpPost]
        public ActionResult Subcribe(string emailSub)
        {
            if (CheckExistEmail(emailSub))
            {
                var email = new EmailSubcribe();
                email.createOn = DateTime.Now;
                email.email = emailSub;
                db.EmailSubcribes.Add(email);
                db.SaveChanges();
                return Json(true, JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json(false, JsonRequestBehavior.AllowGet);
            }
        }

        public bool CheckExistEmail(string email)
        {
            var model = db.EmailSubcribes.Where(x => x.email.Equals(email)).SingleOrDefault();
            if(model == null)
            {
                return true;
            }
            return false;
        }
    }
}