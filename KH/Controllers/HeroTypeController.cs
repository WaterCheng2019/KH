using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using KH.Models;

namespace KH.Controllers
{
    public class HeroTypeController : Controller
    {
        HKEntities db = new HKEntities();
        // GET: HeroType
        public ActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public ActionResult GetTypeNameList(String TypeName, String sort, String order)
        {
            db.Configuration.ProxyCreationEnabled = false;//避免序列化时造成依赖
            IQueryable<HeroType> types = null;

            //1.查询
            if (!string.IsNullOrEmpty(TypeName))
            {
                types = db.HeroTypes.Where(i => i.TypeName.Contains(TypeName));
            }
            else
            {
                types = from i in db.HeroTypes select i;
            }




            //2.排序
            if (order == "desc")
            {
                switch (sort)
                {
                    case "MenuID":
                        types = types.OrderByDescending(i => i.TypeID);
                        break;
                    case "TypeName":
                        types = types.OrderByDescending(i => i.TypeName);
                        break;
                }
            }
            else
            {
                switch (sort)
                {
                    case "MenuID":
                        types = types.OrderBy(i => i.TypeID);
                        break;
                    case "TypeName":
                        types = types.OrderBy(i => i.TypeName);
                        break;
                }
            }



            var obj = new
            {
                total = types.Count(),
                rows = types
            };


            return Json(obj);

        }

        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);    
        }
    }
}