﻿using nicaragua.Permisos;
using System.Web;
using System.Web.Mvc;
using nicaragua.Permisos;

namespace nicaragua
{
    public class FilterConfig
    {
        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            filters.Add(new HandleErrorAttribute());
            filters.Add(new ValidarSesionAttribute());
        }
    }
}
