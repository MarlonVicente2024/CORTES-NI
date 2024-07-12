using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace nicaragua.Models.ViewModels
{
    public class GeneraCorte
    {
        public string Pais { get; set; } = "NI";
        public string Cliente { get; set; }
        public int Promotor { get; set; } = 1106;
        public string Usuario { get; set; } = "PAIT";
        public string Observaciones { get; set; }
        public int? Corte { get; set; }
        public int Anio { get; set; }
        public int Mes { get; set; }
        public string ExcluirDocumentos { get; set; }
        public string IncluirDocumentos { get; set; }
        public int? SalidaCorte { get; set; }
        public int? SalidaIdo { get; set; }
        public IEnumerable<SelectListItem> Clientes { get; set; }
        public IEnumerable<SelectListItem> Farmacias { get; set; } // Nueva propiedad para farmacias
    }
}
