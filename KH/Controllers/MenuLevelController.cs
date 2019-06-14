using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using KH.Models;

namespace KH.Controllers
{
    public class MenuLevelController : Controller
    {
        // GET: MenuLevel
        HKEntities db = new HKEntities();

        public ActionResult Index()
        {
            return View();
        }


        [HttpPost]
        public ActionResult GetMenuLevelList(int? MenuId, string sort, string order, int page = 1, int rows = 20)
        {
            db.Configuration.ProxyCreationEnabled = false;//避免序列化时造成依赖
            IQueryable<v_menu> menus = null;

            //1.查询
            if (MenuId != null && MenuId != -1)
            {
                menus = from i in db.v_menu where i.MenuID == MenuId select i;
            }
            else
            {
                menus = from i in db.v_menu select i;
            }
            //2.排序
            if (order == "asc")
            {
                switch (sort)
                {
                    case "MenuLevelID":
                        menus=menus.OrderByDescending(i => i.MenuLevelID);
                        break;
                    case "MenuLevelName":
                        menus = menus.OrderByDescending(i => i.MenuLevelName);
                        break;
                }
            }
            else
            {
                switch (sort)
                {
                    case "MenuLevelID":
                        menus = menus.OrderBy(i => i.MenuLevelID);
                        break;
                    case "MenuLevelName":
                        menus = menus.OrderBy(i => i.MenuLevelName);
                        break;
                }
            }
            ////3.分页
            int row = menus.Count();
            if (row > 0)
            {
                if (row <= 1)
                {
                    menus = menus.Take(rows);
                }
                else
                {
                    menus = menus.Skip((page - 1) * rows).Take(rows);
                }
            }

            var obj = new
            {
                total = row,
                rows = menus
            };
            return Json(obj);
        }

        //所属一级菜单下拉框
        public ActionResult GetMenuName()
        {
            List<Menu> menus = db.Menus.ToList();
            menus.Insert(0, new Menu() { MenuID = -1, MenuName = "全部" });//设置默认值
            return Json(menus);
        }

        //编辑、增加
        public ActionResult EditMenuLevel(int? id)
        {
            Bind();
            if (id != null)
            {
                //修改
                MenuLevel m = db.MenuLevels.Find(id);
                return View(m);
            }
            else
            {
                //增加
                return View();
            }
         
        }

        [HttpPost]
        public ActionResult EditMenuLevel(MenuLevel m)
        {
            db.Configuration.ProxyCreationEnabled = false;//避免序列化时造成依赖

            int MenuLevelId = m.MenuLevelID;

            if (MenuLevelId != 0)//修改
            {
                MenuLevel m1 = db.MenuLevels.Find(m.MenuLevelID);
                m1.MenuLevelName = m.MenuLevelName;
                m1.MenuLevelURL = m.MenuLevelURL;
                m1.MenuID = m.MenuID;
            }
            else//增加
            {
                db.MenuLevels.Add(m);
            }
            db.SaveChanges();
            return View("index");
        }

        public void Bind()
        {
            List<Menu> menuList = db.Menus.ToList();

            SelectList menuSelect = new SelectList(menuList, "MenuID", "MenuName");

            ViewData["menulevel"] = menuSelect;
        }

        public ActionResult DelById(int id)
        {
            MenuLevel m = db.MenuLevels.Find(id);
            db.MenuLevels.Remove(m);
            db.SaveChanges();
            return Content("success");
            
            
           
        }
        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }

    }
}