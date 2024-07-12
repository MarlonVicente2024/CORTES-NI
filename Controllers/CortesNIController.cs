using nicaragua.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Linq.Dynamic;
using System.Data.SqlClient;

namespace nicaragua.Controllers
{
    public class CortesNIController : Controller
    {
        private readonly DataManagementEntities db = new DataManagementEntities();
        public string draw = "";
        public string start = "";
        public string length = "";
        public string sortColumn = "";
        public string sortColumnDir = "";
        public string searchValue = "";
        public int pageSize, skip, recordsTotal;
        // GET: CortesNI
        public ActionResult Index()
        {
            return View();
        }
        public ActionResult Json(int anio, int mes, string clientepadre)
        {
            var draw = Request.Form.GetValues("draw").FirstOrDefault();
            var start = Request.Form.GetValues("start").FirstOrDefault();
            var length = Request.Form.GetValues("length").FirstOrDefault();
            var sortColumn = Request.Form.GetValues("columns[" + Request.Form.GetValues("order[0][column]").FirstOrDefault() + "][name]").FirstOrDefault();
            var sortColumnDir = Request.Form.GetValues("order[0][dir]").FirstOrDefault();
            var searchValue = Request.Form.GetValues("search[value]").FirstOrDefault();

            int pageSize = length != null ? Convert.ToInt32(length) : 0;
            int skip = start != null ? Convert.ToInt32(start) : 0;
            int recordsTotal = 0;

            var result = db.Database.SqlQuery<SP_VALIDAR_DATOS_CORTES_NI_Result>(
                "SP_VALIDAR_DATOS_CORTES_NI @ANIO, @MES, @CLIENTEPADRE",
                new SqlParameter("@ANIO", anio),
                new SqlParameter("@MES", mes),
                new SqlParameter("@CLIENTEPADRE", clientepadre)
            ).ToList();

            IQueryable<SP_VALIDAR_DATOS_CORTES_NI_Result> query = result.AsQueryable();

            if (!string.IsNullOrEmpty(searchValue))
            {
                query = query.Where(d => d.Documento.Contains(searchValue) ||
                                         //d.producto.Contains(searchValue) ||
                                         d.clientepadre.Contains(searchValue) ||
                                         d.cliente.Contains(searchValue));
            }

            if (!(string.IsNullOrEmpty(sortColumn) && string.IsNullOrEmpty(sortColumnDir)))
            {
                query = query.OrderBy(sortColumn + " " + sortColumnDir);
            }

            recordsTotal = query.Count();
            var data = query.Skip(skip).Take(pageSize).ToList();

            return Json(new { draw = draw, recordsFiltered = recordsTotal, recordsTotal = recordsTotal, data = data }, JsonRequestBehavior.AllowGet);
        }
    }
}

