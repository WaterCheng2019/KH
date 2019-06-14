using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.IO;

namespace KH.filters
{
    public class ExceptionFilter : FilterAttribute, IExceptionFilter
    {

        //1.创建log.txt日志
        //2.添加filters文件夹,创建类ExceptionFilter,继承类FileterAttribute,实现接口IExceptionFilter
        //3.在FilterConfig.cs上应用到全局
        public void OnException(ExceptionContext filterContext)
        {
            //获取日志文件的的物理路径
            String filePath = filterContext.HttpContext.Server.MapPath(@"~/log.txt");

            //写入日志文件
            using (StreamWriter sw=new StreamWriter(filePath,true))
            {
                sw.WriteLine("时间:{0}",DateTime.Now.ToString());
                sw.WriteLine("控制器:{0}",filterContext.RouteData.Values["controller"]);
                sw.WriteLine("动作方法:{0}",filterContext.RouteData.Values["action"]);
                sw.WriteLine("异常信息:{0}",filterContext.Exception.Message);
                sw.WriteLine();
            }
        }
    }
}