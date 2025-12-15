using Microsoft.AspNetCore.Mvc;
using DapperData.Models;
using Persistencia;
using System.Linq;
using System.Threading.Tasks;

namespace MVC.Controllers
{
    public class TablerosController : AuthenticatedController
    {
        private readonly IDao _dao;

        public TablerosController(IDao dao)
        {
            _dao = dao;
        }

        // ======================
        // Listar tableros del usuario
        // ======================
        public async Task<IActionResult> Index()
        {
            long usuarioId = UsuarioIdActual;
            var tableros = await _dao.ObtenerTablerosPorUsuario(usuarioId);
            return View(tableros);
        }

        // ======================
        // Crear tablero (GET)
        // ======================
        public IActionResult Create()
        {
            return View();
        }

        // ======================
        // Crear tablero (POST)
        // ======================
        [HttpPost]
        public async Task<IActionResult> Create(Tablero tablero)
        {
            if (!ModelState.IsValid)
                return View(tablero);

            if (string.IsNullOrWhiteSpace(tablero.Nombre))
            {
                ModelState.AddModelError("Nombre", "El nombre del tablero es obligatorio");
                return View(tablero);
            }

            tablero.PropietarioId = UsuarioIdActual;

            var idNuevoTablero = await _dao.CrearTablero(tablero);

            return RedirectToAction("Detalle", new { id = idNuevoTablero });
        }

        // ======================
        // Ver detalle del tablero y sus tareas
        // ======================
        public async Task<IActionResult> Detalle(long id)
        {
            // Obtener tablero
            var tablero = (await _dao.ObtenerTablerosPorUsuario(UsuarioIdActual))
                            .FirstOrDefault(t => t.Id == id);

            if (tablero == null)
                return NotFound();

            // Obtener tareas del tablero
            var tareas = (await _dao.ObtenerTareas()).Where(t => t.TableroId == id).ToList();

            // Construir ViewModel
            var vm = new TableroDetalleViewModel
            {
                TableroId = tablero.Id,
                Nombre = tablero.Nombre
            };

            foreach (var grupo in tareas.GroupBy(t => new { t.ColumnaId, t.ColumnaNombre }))
            {
                vm.Columnas.Add(new TableroColumnViewModel
                {
                    ColumnaId = grupo.Key.ColumnaId,
                    Nombre = grupo.Key.ColumnaNombre,
                    Tareas = grupo.ToList()
                });
            }

            return View(vm);
        }

        // ======================
        // Crear tarea en tablero
        // ======================
        [HttpPost]
        public async Task<IActionResult> CrearTarea(Tarea tarea)
        {
            if (string.IsNullOrWhiteSpace(tarea.Titulo))
            {
                TempData["Error"] = "El título es obligatorio";
                return RedirectToAction("Detalle", new { id = tarea.TableroId });
            }

            tarea.CreadoPor = UsuarioIdActual;

            await _dao.CrearTarea(tarea);

            return RedirectToAction("Detalle", new { id = tarea.TableroId });
        }

        // ======================
        // Eliminar tablero
        // ======================
        [HttpPost]
        public async Task<IActionResult> Eliminar(long id)
        {
            await _dao.EliminarTablero(id);
            return RedirectToAction("Index");
        }
    }
}
