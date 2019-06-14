using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using KH.Models;
using System.IO;

namespace KH.Content
{
    public class HeroController : Controller
    {
        HKEntities db = new HKEntities();
        // GET: Hero
        public ActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public ActionResult GetHreoList(int? typeId, String sort, String order, int page = 1, int rows = 5)
        {
            db.Configuration.ProxyCreationEnabled = false;//避免序列化时造成依赖

            //优化，EF框架，贪婪加载
            //1.设置延迟加载属性false
            //2.查询表达式中，使用Include()

            IQueryable<v_hero> heros = null;

            //1.查询
            if (typeId != null && typeId != -1)
            {
                heros = (from i in db.v_hero where i.HeroTypeID == typeId select i);
            }
            else
            {
                heros = from i in db.v_hero select i;
            }

            //2.排序
            if (order == "desc")
            {
                switch (sort)
                {
                    case "HeroID":
                        heros = heros.OrderByDescending(i => i.HeroID);
                        break;
                    case "AddData":
                        heros = heros.OrderByDescending(i => i.AddData);
                        break;
                }
            }
            else
            {
                switch (sort)
                {
                    case "HeroID":
                        heros = heros.OrderBy(i => i.HeroID);
                        break;
                    case "AddData":
                        heros = heros.OrderBy(i => i.AddData);
                        break;
                }
            }

            //3.分页，分页之前必须先排序
            int r = heros.Count();//总记录条数
            if (r > 0)
            {
                if (page <= 1)
                {
                    heros = heros.Take(rows);
                }
                else
                {
                    heros = heros.Skip((page - 1) * rows).Take(rows);
                }
            }


            var obj = new
            {
                //total= heros.Count(),
                total = r,
                rows = heros
            };

            return Json(obj);
        }


        //英雄详细内容
        public ActionResult GetHeroById(int Id)
        {
            var hero = (from i in db.v_hero where i.HeroID == Id select i).FirstOrDefault();
            return PartialView(hero);
        }


        //编辑、增加
        public ActionResult EditHero(int? id)
        {
            Bind();
            if (id != null)
            {
                //编辑
                ViewData["info"] = "编辑英雄";
                Hero hero = db.Heroes.Find(id);
                return View(hero);
            }
            else
            {
                //增加
                ViewData["info"] = "增加英雄";
                return View();
            }

        }
        [HttpPost]
        public ActionResult EditHero(Hero h, HttpPostedFileBase myfiel)
        {
            if (!ModelState.IsValid)//没通过验证
            {
                Bind();//重新传递SeleteList数据源
                return View();
            }

            int id = h.HeroID;

            if (id != 0)
            {
                //修改影片
                Hero h1 = db.Heroes.Find(h.HeroID);
                h1.HeroName = h.HeroName;
                h1.HeroTypeID = h.HeroTypeID;
                h1.AddData = h.AddData;
                h1.HeroStory = h.HeroStory;

                //如果更新图片
                if (myfiel != null && (Path.GetExtension(myfiel.FileName) == ".jpg"
                                || Path.GetExtension(myfiel.FileName) == ".bmp"
                                || Path.GetExtension(myfiel.FileName) == ".png"
                                || Path.GetExtension(myfiel.FileName) == ".gif"))
                {
                    h1.Picture = myfiel.FileName;//保存文件名
                    myfiel.SaveAs(Server.MapPath("~/Content/Picture/" + myfiel.FileName));
                }

                 db.SaveChanges();
                return View("Index");
            }
            else
            {

                //判断是否重复添加
                if (IsChunZai(h))
                {
                    ModelState.AddModelError("myError", "次英雄已经存在！请勿重复添加！");
                    Bind();
                    return View();
                }


                //增加影片
                //判断是否选择了文件
                if (myfiel == null)
                {
                    ModelState.AddModelError("myError", "请选择新增英雄头像");
                    Bind();
                    return View();
                }
                else
                {
                    //选择了文件
                    String ext = Path.GetExtension(myfiel.FileName);//获取文件扩展名
                    if (ext != ".jpg" && ext != ".bmp" && ext != ".png" && ext != ".gif")
                    {
                        ModelState.AddModelError("myError", "请选择图片文件");
                        Bind();
                        return View();
                    }
                    else
                    {
                        //向数据库添加
                        h.Picture = myfiel.FileName;
                        db.Heroes.Add(h);
                        db.SaveChanges();

                        //上传文件
                        myfiel.SaveAs(Server.MapPath("~/Content/Picture/" + myfiel.FileName));
                        return View("Index");
                    }
                }
            }

        }

        //编辑、增加状态，下拉框绑定
        public void Bind()
        {
            List<HeroType> heroTypes = null;

            heroTypes = db.HeroTypes.ToList();

            SelectList heroTypeList = new SelectList(heroTypes, "TypeID", "TypeName");

            ViewData["heroType"] = heroTypeList;
        }

        //判断是否重复添加
        public bool IsChunZai(Hero h)
        {
            bool falg = false;

            int count = (from i in db.Heroes where i.HeroName == h.HeroName select i).Count();
            if (count>0)
            {
                falg = true;
            }
            return falg;

        }


        //删除
        [HttpPost]
        public ActionResult DelById(int? id)
        {
            Hero h = db.Heroes.Find(id);
            db.Heroes.Remove(h);
            db.SaveChanges();
            return Content("success");
        }

        [HttpPost]
        public ActionResult GetHeroTypeList()
        {
            db.Configuration.ProxyCreationEnabled = false;

            List<HeroType> heroTypes = (from i in db.HeroTypes select i).ToList();

            heroTypes.Insert(0, new HeroType() { TypeID = -1, TypeName = "全部" });
            return Json(heroTypes);
        }



        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}