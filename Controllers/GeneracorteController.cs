using nicaragua.Models;
using nicaragua.Models.ViewModels;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Web.Mvc;
using OfficeOpenXml;

namespace nicaragua.Controllers
{
    public class GeneracorteController : Controller
    {
        private readonly DataManagementEntities db = new DataManagementEntities();

        public ActionResult Index()
        {
            var model = new GeneraCorte
            {
                Clientes = GetClientes(),
                Farmacias = GetFarmacias()
            };
            return View(model);
        }

        public ActionResult MostrarCorte()
        {
            try
            {
                // Aquí obtienes los datos de la base de datos o cualquier otra fuente
                var query = "SELECT * FROM CA_IP_SUCURSAL_DEPENDIENTE";
                var data = db.Database.SqlQuery<CA_IP_SUCURSAL_DEPENDIENTE>(query).ToList();

                // Si deseas filtrar por algún criterio, como un número de corte específico
                // var corte = 12345; // Por ejemplo, el número de corte deseado
                // var data = db.CA_IP_SUCURSAL_DEPENDIENTE.Where(x => x.Corte == corte).ToList();

                return View(data);
            }
            catch (Exception ex)
            {
                // Manejo de errores, por ejemplo:
                return RedirectToAction("Index", "Home");
            }
        }



        [HttpPost]
        public JsonResult GenerarCorte(GeneraCorte model)
        {
            if (ModelState.IsValid)
            {
                int salidaCorte;
                int salidaIdo;

                try
                {
                    SqlParameter[] parameters =
                    {
                        new SqlParameter("@Pais", model.Pais),
                        new SqlParameter("@Cliente", model.Cliente),
                        new SqlParameter("@Promotor", model.Promotor),
                        new SqlParameter("@Usuario", model.Usuario),
                        new SqlParameter("@Observaciones", model.Observaciones),
                        new SqlParameter("@Corte", model.Corte.HasValue ? (object)model.Corte.Value : DBNull.Value),
                        new SqlParameter("@anio", model.Anio),
                        new SqlParameter("@mes", model.Mes),
                        new SqlParameter("@ExcluirDocumentos", model.ExcluirDocumentos ?? (object)DBNull.Value),
                        new SqlParameter("@IncluirDocumentos", model.IncluirDocumentos ?? (object)DBNull.Value),
                        new SqlParameter("@SalidaCorte", System.Data.SqlDbType.Int) { Direction = System.Data.ParameterDirection.Output },
                        new SqlParameter("@SalidaIdo", System.Data.SqlDbType.Int) { Direction = System.Data.ParameterDirection.Output }
                    };

                    db.Database.ExecuteSqlCommand("EXEC sp_valida_Genera_Genera_Con_Exclusion_Inclusion @Pais, @Cliente, @Promotor, @Usuario, @Observaciones, @Corte, @anio, @mes, @ExcluirDocumentos, @IncluirDocumentos, @SalidaCorte OUTPUT, @SalidaIdo OUTPUT", parameters);

                    salidaCorte = (int)parameters[10].Value;
                    salidaIdo = (int)parameters[11].Value;

                    return Json(new { success = true, salidaCorte = salidaCorte, salidaIdo = salidaIdo });
                }
                catch (Exception ex)
                {
                    return Json(new { success = false, message = "Error al ejecutar el procedimiento almacenado: " + ex.Message });
                }
            }

            return Json(new { success = false, message = "Datos inválidos." });
        }

        [HttpPost]
        public JsonResult AnularCorte(string NumeroCorte, string FechaAnula)
        {
            try
            {
                SqlParameter[] parameters =
                {
                    new SqlParameter("@Corte", NumeroCorte),
                    new SqlParameter("@FechaAnula", FechaAnula)
                };

                db.Database.ExecuteSqlCommand("UPDATE IP SET EstadoIP=4, FechaAnula=@FechaAnula WHERE Corte=@Corte", parameters);

                return Json(new { success = true });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

        [HttpPost]
        public ActionResult GetCorteData(int corte)
        {
            try
            {
                var query = "SELECT * FROM CA_IP_SUCURSAL_DEPENDIENTE WHERE corte = @Corte";
                var parameters = new SqlParameter[] { new SqlParameter("@Corte", corte) };
                var data = db.Database.SqlQuery<CA_IP_SUCURSAL_DEPENDIENTE>(query, parameters).ToList();

                return Json(new { data = data });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }


        [HttpGet]
        public ActionResult DescargarCorte(int Corte)
        {
            var query = "SELECT * FROM CA_IP_SUCURSAL_DEPENDIENTE WHERE corte = @Corte";
            var parameters = new SqlParameter[] { new SqlParameter("@Corte", Corte) };
            var data = db.Database.SqlQuery<CA_IP_SUCURSAL_DEPENDIENTE>(query, parameters).ToList();

            using (var package = new ExcelPackage())
            {
                var worksheet = package.Workbook.Worksheets.Add("Corte");
                worksheet.Cells["A1"].LoadFromCollection(data, true);
                var stream = new System.IO.MemoryStream(package.GetAsByteArray());

                var fileName = $"Corte_{Corte}.xlsx";
                return File(stream, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", fileName);
            }
        }

        private IEnumerable<SelectListItem> GetClientes()
        {
            return new List<SelectListItem>
            {
                new SelectListItem { Value = "83111", Text = "83111 - Farmacia Farmavalue" },
                new SelectListItem { Value = "80156", Text = "80156 - Farmacia Los Paisas" },
                new SelectListItem { Value = "002189", Text = "002189 - Farmacia Maria Inmaculada" },
                new SelectListItem { Value = "1700043", Text = "1700043 - Farmacia Guadalupana" },
                new SelectListItem { Value = "4301", Text = "4301 - Farmacia Samaria" },
                new SelectListItem { Value = "7-81053", Text = "7-81053 - Farmacia Lily Viera" },
                new SelectListItem { Value = "010441-03", Text = "010441-03 - Farmacia Farmas" },
                new SelectListItem { Value = "015176-01", Text = "015176-01 - Farmacia Economica" },
                new SelectListItem { Value = "107197", Text = "107197 - Farmacia Meridional" },
                new SelectListItem { Value = "6-030340-01", Text = "6-030340-01 - Farmacias Farmanet" },
                new SelectListItem { Value = "106791", Text = "106791 - Farmacia Huembes" },
                new SelectListItem { Value = "6-045172-01", Text = "6-045172-01 - Farmacia Bustillo Pereira Express" }
            };
        }

        private IEnumerable<SelectListItem> GetFarmacias()
        {
            return new List<SelectListItem>
            {
                new SelectListItem { Value = "Farmacia Farmavalue", Text = "Farmacia Farmavalue" },
                new SelectListItem { Value = "Farmacia Los Paisas", Text = "Farmacia Los Paisas" },
                new SelectListItem { Value = "Farmacia Maria Inmaculada", Text = "Farmacia Maria Inmaculada" },
                new SelectListItem { Value = "Farmacia Guadalupana", Text = "Farmacia Guadalupana" },
                new SelectListItem { Value = "Farmacia Samaria", Text = "Farmacia Samaria" },
                new SelectListItem { Value = "Farmacia Lily Viera", Text = "Farmacia Lily Viera" },
                new SelectListItem { Value = "Farmacia Farmas", Text = "Farmacia Farmas" },
                new SelectListItem { Value = "Farmacia Economica", Text = "Farmacia Economica" },
                new SelectListItem { Value = "Farmacia Meridional", Text = "Farmacia Meridional" },
                new SelectListItem { Value = "Farmacias Farmanet", Text = "Farmacias Farmanet" },
                new SelectListItem { Value = "Farmacia Huembes", Text = "Farmacia Huembes" },
                new SelectListItem { Value = "Farmacia Bustillo Pereira Express", Text = "Farmacia Bustillo Pereira Express" }
            };
        }
    }
}
