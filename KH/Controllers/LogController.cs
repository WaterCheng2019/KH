using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.IO;
using KH;

namespace KH.Controllers
{
    public class LogController : Controller
    {
        // GET: Log
        public ActionResult Index()
        {
            String logInfo = "";
            try
            {
                String path = HttpContext.Server.MapPath(@"~/log.txt");// @"\log.txt"

                FileStream fs = new FileStream(path, FileMode.Open);
                StreamReader sr = new StreamReader(fs);
                logInfo = sr.ReadToEnd();
                sr.Close();
                fs.Close();
            }
            catch (Exception ex)
            {
                throw;
            }


            return Content(logInfo, "text/plain");
        }

    }
}