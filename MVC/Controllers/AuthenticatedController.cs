using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace MVC.Controllers
{
    public class AuthenticatedController : Controller
    {
        public override void OnActionExecuting(ActionExecutingContext context)
        {
            if (HttpContext.Session.GetString("UserId") == null)
            {
                context.Result = RedirectToAction("Login", "Auth");
                return;
            }

            base.OnActionExecuting(context);
        }
        protected long UsuarioIdActual
        {
            get
            {
                var userIdStr = HttpContext.Session.GetString("UserId");
                return userIdStr != null ? long.Parse(userIdStr) : 0;
            }
        }
    }
}




