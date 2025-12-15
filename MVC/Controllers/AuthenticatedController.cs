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
    }
}

