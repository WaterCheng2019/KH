using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using KH.filters;

namespace KH.Controllers
{
    public class TestController : Controller
    {

        // GET: Test
        public ActionResult Index()
        {
            int a = int.Parse("DA22");
            return View();
        }
    }
}