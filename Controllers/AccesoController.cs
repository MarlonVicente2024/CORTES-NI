using nicaragua.Models;
using System;
using System.Data;
using System.Data.SqlClient;
using System.Web;
using System.Web.Mvc;

namespace nicaragua.Controllers
{
    public class AccesoController : Controller
    {
        static string cadena = "Data Source=192.168.61.200; DataBase=DataManagement; User ID=mvicente; Password=LasanteGT2024*";

        // GET: Acceso
        public ActionResult Login()
        {
            return View();
        }

        public ActionResult Registrar()
        {
            return View();
        }

        public ActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Registrar(Usuarios oUsuario)
        {
            bool registrado;
            string mensaje;

            if (oUsuario.Password == oUsuario.ConfirmarClave)
            {
                // No encriptar la clave
                // oUsuario.Password = ConvertirSha256(oUsuario.Password);
            }
            else
            {
                ViewData["Mensaje"] = "Las contraseñas no coinciden";
                return View();
            }

            using (SqlConnection cn = new SqlConnection(cadena))
            {
                SqlCommand cmd = new SqlCommand("sp_ValidaUsuario_cortesNI", cn);
                cmd.Parameters.AddWithValue("Usuario", oUsuario.Usuario);
                cmd.Parameters.AddWithValue("Password", oUsuario.Password);
                cmd.Parameters.Add("Registrado", SqlDbType.Bit).Direction = ParameterDirection.Output;
                cmd.Parameters.Add("Mensaje", SqlDbType.VarChar, 100).Direction = ParameterDirection.Output;
                cmd.CommandType = CommandType.StoredProcedure;

                cn.Open();
                cmd.ExecuteNonQuery();
                registrado = Convert.ToBoolean(cmd.Parameters["Registrado"].Value);
                mensaje = cmd.Parameters["Mensaje"].Value.ToString();
            }

            ViewData["Mensaje"] = mensaje;

            if (registrado)
            {
                return RedirectToAction("Login", "Acceso");
            }
            else
            {
                return View();
            }
        }

        // Método de login de usuarios 
        [HttpPost]
        public ActionResult Login(Usuarios oUsuario)
        {
            // No encriptar la clave
            // oUsuario.Password = ConvertirSha256(oUsuario.Password);

            using (SqlConnection cn = new SqlConnection(cadena))
            {
                SqlCommand cmd = new SqlCommand("sp_ValidaUsuario_cortesNI", cn);
                cmd.Parameters.AddWithValue("usuario", oUsuario.Usuario);
                cmd.Parameters.AddWithValue("Password", oUsuario.Password);
                cmd.CommandType = CommandType.StoredProcedure;

                cn.Open();
                oUsuario.IdUsuario = Convert.ToInt32(cmd.ExecuteScalar().ToString());
            }

            if (oUsuario.IdUsuario != 0)
            {
                Session["usuario"] = oUsuario; // Aquí se configura la sesión
                return RedirectToAction("Index", "Home");
            }
            else
            {
                ViewData["Mensaje"] = "Usuario no ha sido encontrado!";
                return View();
            }
        }
    }
}
