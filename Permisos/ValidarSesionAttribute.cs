using System;
using System.Web;
using System.Web.Mvc;

namespace nicaragua.Permisos
{
    public class ValidarSesionAttribute : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            var controller = filterContext.ActionDescriptor.ControllerDescriptor.ControllerName;
            var action = filterContext.ActionDescriptor.ActionName;

            // Excluir las acciones de Login y Registrar
            if (controller == "Acceso" && (action == "Login" || action == "Registrar"))
            {
                return;
            }

            if (HttpContext.Current.Session["usuario"] == null)
            {
                filterContext.Result = new RedirectToRouteResult(
                    new System.Web.Routing.RouteValueDictionary
                    {
                        { "controller", "Acceso" },
                        { "action", "Login" }
                    });
            }
            base.OnActionExecuting(filterContext);
        }
    }
}
