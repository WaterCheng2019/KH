﻿using System.Web;
using System.Web.Mvc;
using KH.filters;

namespace KH
{
    public class FilterConfig
    {
        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            filters.Add(new HandleErrorAttribute());
            filters.Add(new ExceptionFilter());
        }
    }
}
