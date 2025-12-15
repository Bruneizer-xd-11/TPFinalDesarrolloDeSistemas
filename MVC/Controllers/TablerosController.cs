using Microsoft.AspNetCore.Mvc;
using DapperData.Models;
using Persistencia;
using System.Linq;
using System.Threading.Tasks;

namespace MVC.Controllers;

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
        long usuarioId = UsuarioIdActual; // ID del usuario logueado
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

        // Asignar propietario
        tablero.PropietarioId = UsuarioIdActual;

        // Crear tablero y obtener ID
        var idNuevoTablero = await _dao.CrearTablero(tablero);

        // Redirigir al detalle del tablero recién creado
        return RedirectToAction("Detalle", new { id = idNuevoTablero });
    }

    // ======================
    // Ver detalle de tablero
    // ======================
    public async Task<IActionResult> Detalle(long id)
    {
        // Obtener todas las tareas
        var tareas = await _dao.ObtenerTareas();

        // Filtrar solo las tareas de este tablero
        var tareasTablero = tareas.Where(t => t.TableroId == id).ToList();

        if (!tareasTablero.Any())
            return NotFound();

        // Construir ViewModel
        var vm = new TableroDetalleViewModel
        {
            TableroId = id,
            Nombre = tareasTablero.First().TableroNombre
        };

        // Agrupar tareas por columnas
        foreach (var grupo in tareasTablero.GroupBy(t => new { t.ColumnaId, t.ColumnaNombre }))
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
}
