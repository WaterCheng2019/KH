using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using KH.Models;
using KH.filters;

namespace KH.Controllers
{
    public class MenuController : Controller
    {
        HKEntities db = new HKEntities();
        // GET: Menu
        public ActionResult Index()
        {
            return View();
        }

        [ExceptionFilter]
        public ActionResult GetTreeList()
        {
            List<TreeNode> nodes = new List<TreeNode>();

            //创建根节点
            TreeNode rNode = new TreeNode() { id = 0, text = "英雄角色管理系统" };

            List<Menu> menus = (from i in db.Menus select i).ToList();

            foreach (var menu in menus)
            {
                TreeNode node = new TreeNode() { id = menu.MenuID, text = menu.MenuName };

                AddMenuLevel(menu.MenuID,node);//添加二级菜单

                rNode.children.Add(node);//添加一级菜单
            }

            nodes.Add(rNode);
            return Json(nodes,JsonRequestBehavior.AllowGet);
        }

        //添加二级菜单
        private void AddMenuLevel(int MenuID,TreeNode menu)
        {

            //优化EF,使用贪婪加载
            //1.设置贪婪加载属性false
            //2.查询中使用Include()

            List<MenuLevel> menuLevels = (from i in db.MenuLevels where i.MenuID == MenuID select i).ToList();

            foreach (var menuLevel in menuLevels)
            {
                TreeNode childNode = new TreeNode() { id = menuLevel.MenuID, text = menuLevel.MenuLevelName };
                childNode.attribute.url = menuLevel.MenuLevelURL;
                menu.children.Add(childNode);
            }
        }

   
        
        public ActionResult GetMeunList(String MenuName,String sort,String order)
        {
            IQueryable<Menu> menus = null;

            //1.查询
            if (!string.IsNullOrEmpty(MenuName))
            {
                menus = db.Menus.Where(i=>i.MenuName.Contains(MenuName));
            }
            else
            {
                menus = from i in db.Menus select i;
            }
           

            //2.排序
            if (order=="desc")
            {
                switch (sort)
                {
                    case "MenuID":
                        menus = menus.OrderByDescending(i=>i.MenuID);
                        break;
                    case "MenuName":
                        menus = menus.OrderByDescending(i=>i.MenuName);
                        break;
                }
            }
            else
            {
                switch (sort)
                {
                    case "MenuID":
                        menus = menus.OrderBy(i => i.MenuID);
                        break;
                    case "MenuName":
                        menus = menus.OrderBy(i => i.MenuName);
                        break;
                }
            }



            var obj = new
            {
                total = menus.Count(),
                rows = menus
            };


            return Json(obj);
            
        }


        //修改、增加
        public ActionResult EditMenu(int? id)
        {
            if (id!=null)
            {
                Menu m = (from i in db.Menus where i.MenuID == id select i).FirstOrDefault();//修改
                ViewData["info"] = "修改一级菜单";
                return View(m);
            }
            else
            {
                ViewData["info"] = "增加一级菜单";
                return View();
            }
          
        }

        [HttpPost]
        public ActionResult EditMenu(Menu m)
        {
            int MenuId = m.MenuID;

            if (MenuId!=0)
            {
                Menu m1 = db.Menus.Find(m.MenuID);
                m1.MenuName = m.MenuName;
            }
            else
            {
                db.Menus.Add(m);
            }
         
            db.SaveChanges();
            return View("Index");
        }

        public ActionResult DelMenuById(int id)
        {
            Menu m = db.Menus.Find(id);
            db.Menus.Remove(m);
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