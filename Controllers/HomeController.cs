﻿using nicaragua.Permisos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;


namespace nicaragua.Controllers
{
    [ValidarSesion]
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }
        public ActionResult CerrarSesion()
        {
            Session["usuarios"] = null;
            return RedirectToAction("Login", "Acceso");
        }
    }
}