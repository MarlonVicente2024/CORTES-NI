using nicaragua.Models;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Web.Mvc;
using System.Linq.Dynamic;

namespace nicaragua.Controllers
{
    public class Mostrartabla_corteController : Controller
    {
        private readonly mostrarEntities db = new mostrarEntities();

        // GET: Mostrartabla_corte
        public ActionResult index()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Json(int Corte)
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

            var query = db.CA_IP_SUCURSAL_DEPENDIENTE.Where(d => d.Corte == Corte);
            // Obtener los datos del procedimiento almacenado
            //var result = db.Database.SqlQuery<CA_IP_SUCURSAL_DEPENDIENTE>(
            //    "CA_IP_SUCURSAL_DEPENDIENTE @CORTE",
            //    new SqlParameter("@CORTE", Corte)
            //).ToList();

            //// Convertir a IQueryable para aplicar filtros dinámicos
            //IQueryable<CA_IP_SUCURSAL_DEPENDIENTE> query = result.AsQueryable();

            // Aplicar filtro de búsqueda
            if (!string.IsNullOrEmpty(searchValue))
            {
                query = query.Where(d => d.Factura.Contains(searchValue) ||
                                         d.Cliente.Contains(searchValue));
            }

            // Aplicar ordenamiento dinámico
            if (!(string.IsNullOrEmpty(sortColumn) && string.IsNullOrEmpty(sortColumnDir)))
            {
                query = query.OrderBy(sortColumn + " " + sortColumnDir);
            }

            // Obtener el total de registros
            recordsTotal = query.Count();

            // Aplicar paginación
            var data = query.Skip(skip).Take(pageSize).ToList();

            // Devolver datos en formato JSON
            return Json(new { draw = draw, recordsFiltered = recordsTotal, recordsTotal = recordsTotal, data = data }, JsonRequestBehavior.AllowGet);
        }
    }
}
