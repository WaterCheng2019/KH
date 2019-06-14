using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using KH.filters;
using KH.Models;
using System.Web.Security;

namespace KH.Controllers
{
    public class UserController : Controller
    {
        HKEntities db = new HKEntities();
        // GET: User
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult GetUserList(String UserName, String sort, String order)
        {

            IQueryable<User> uaers = null;

            //1.查询
            if (!string.IsNullOrEmpty(UserName))
            {
                uaers = db.Users.Where(i => i.UserName.Contains(UserName));
            }
            else
            {
                uaers = from i in db.Users select i;
            }




            //2.排序
            if (order == "desc")
            {
                switch (sort)
                {
                    case "MenuID":
                        uaers = uaers.OrderByDescending(i => i.UserID);
                        break;
                    case "MenuName":
                        uaers = uaers.OrderByDescending(i => i.UserName);
                        break;
                }
            }
            else
            {
                switch (sort)
                {
                    case "MenuID":
                        uaers = uaers.OrderBy(i => i.UserID);
                        break;
                    case "MenuName":
                        uaers = uaers.OrderBy(i => i.UserName);
                        break;
                }
            }



            var obj = new
            {
                total = uaers.Count(),
                rows = uaers
            };


            return Json(obj);

        }

        public ActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Login(User u)
        {
            if (ModelState.IsValid&&CheckUser(u))
            {
                return RedirectToAction("Index", "Home");
            }
            else
            {
                ModelState.AddModelError("","账号或密码错误");
                return View(u);
            }
        }
        public bool CheckUser(User u)
        {
            int count = (db.Users.Where(i => i.UserName == u.UserName && i.UserPassword == u.UserPassword).Count());

            if (count<=0)
            {
                return false;//登录失败
            }
            else
            {
                FormsAuthentication.SetAuthCookie(u.UserName,false);//保存身份Cookie
                return true;//登录成功
            }
        }

        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}