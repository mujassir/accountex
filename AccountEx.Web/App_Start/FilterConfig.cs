﻿using System.Web.Mvc;
using Elmah;

namespace AccountEx.Web
{
    public class FilterConfig
    {
        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
           
            filters.Add(new HandleErrorAttribute());
           
        }
    }
}