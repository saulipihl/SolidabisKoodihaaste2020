﻿using System.Web;
using System.Web.Mvc;

namespace Koodihaaste2020Backend
{
    public class FilterConfig
    {
        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            filters.Add(new HandleErrorAttribute());
        }
    }
}
