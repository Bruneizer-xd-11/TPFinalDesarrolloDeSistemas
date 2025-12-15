using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
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

        // ======================
        // LOGIN GET
        // ======================
        [HttpGet]
        public IActionResult Login()
        {
            return View(new UsuarioLoginDto
            {
                Username = "",
                Password = ""
            });
        }

        // ======================
        // LOGIN POST
        // ======================
        [HttpPost]
        public async Task<IActionResult> Login(UsuarioLoginDto dto)
        {
            if (!ModelState.IsValid)
                return View(dto);

            var user = await _dao.ObtenerUsuarioPorUsername(dto.Username);

            if (user == null || string.IsNullOrEmpty(user.PasswordHash))
            {
                ModelState.AddModelError("", "Usuario o contraseña incorrectos.");
                return View(dto);
            }

            var hasher = new PasswordHasher<Usuario>();
            var result = hasher.VerifyHashedPassword(
                user,
                user.PasswordHash,
                dto.Password
            );

            if (result == PasswordVerificationResult.Failed)
            {
                ModelState.AddModelError("", "Usuario o contraseña incorrectos.");
                return View(dto);
            }

            // ✅ SESIÓN (ESTO ESTABA ROMPIENDO)
            HttpContext.Session.SetString("UserId", user.Id.ToString());
            HttpContext.Session.SetString("Username", user.Username);

            return RedirectToAction("Index", "Tableros");
        }

        // ======================
        // REGISTER GET
        // ======================
        [HttpGet]
        public IActionResult Register()
        {
            return View(new Usuario());
        }

        // ======================
        // REGISTER POST
        // ======================
        [HttpPost]
        public async Task<IActionResult> Register(Usuario usuario)
        {
            if (!ModelState.IsValid)
                return View(usuario);

            var hasher = new PasswordHasher<Usuario>();
            usuario.PasswordHash = hasher.HashPassword(usuario, usuario.PasswordHash);

            await _dao.RegistrarUsuario(usuario);

            return RedirectToAction("Login");
        }

        // ======================
        // LOGOUT
        // ======================
        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Login");
        }
    }
}

