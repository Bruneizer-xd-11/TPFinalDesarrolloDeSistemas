using Microsoft.AspNetCore.Mvc;
using DapperData.Models;
using Persistencia;

namespace MVC.Controllers
{
    public class AuthController : Controller
    {
        private readonly IDao _dao;

        public AuthController(IDao dao)
        {
            _dao = dao;
        }
// sadasdsad
        [HttpGet]
        public IActionResult Login()
        {
            return View(new UsuarioLoginDto { Username = "", Password = "" });
        }

        [HttpPost]
        public async Task<IActionResult> Login(UsuarioLoginDto dto)
        {
            if (!ModelState.IsValid)
                return View(dto);

            var user = await _dao.ObtenerUsuarioPorUsername(dto.Username);

            if (user == null || user.PasswordHash != dto.Password) 
            {
                ModelState.AddModelError("", "Usuario o contraseña incorrectos.");
                return View(dto);
            }

            HttpContext.Session.SetString("UserId", user.Id.ToString());
            HttpContext.Session.SetString("Username", user.Username);

            return RedirectToAction("Index", "Tableros");
        }

        [HttpGet]
        public IActionResult Register()
        {
            return View(new Usuario());
        }

        [HttpPost]
        public async Task<IActionResult> Register(Usuario usuario)
        {
            if (!ModelState.IsValid)
                return View(usuario);

            usuario.PasswordHash = usuario.PasswordHash;

            long id = await _dao.RegistrarUsuario(usuario);

            return RedirectToAction("Login");
        }

        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Login");
        }
    }
}
