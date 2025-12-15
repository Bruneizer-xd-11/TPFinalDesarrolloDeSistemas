using Microsoft.AspNetCore.Mvc;
using DapperData.Models;
using Persistencia;

namespace MVC.Controllers;

public class TablerosController : AuthenticatedController
{
    private readonly IDao _dao;

    public TablerosController(IDao dao)
    {
        _dao = dao;
    }

    public async Task<IActionResult> Index()
    {
        long usuarioId = UsuarioIdActual; // Obtenemos ID del usuario logueado
        var tableros = await _dao.ObtenerTablerosPorUsuario(usuarioId);
        return View(tableros);
    }

    // GET: Tableros/Create
    public IActionResult Create()
    {
        return View();
    }

    // POST: Tableros/Create
    [HttpPost]
    public async Task<IActionResult> Create(Tablero tablero)
    {
        if (!ModelState.IsValid) 
            return View(tablero);
            
        // Validación adicional: nombre obligatorio
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


    public async Task<IActionResult> Detalle(long id)
    {
        var tareas = await _dao.ObtenerTareas();
        var tareasTablero = tareas.Where(t => t.TableroId == id).ToList();

        if (!tareasTablero.Any()) return NotFound();

        var vm = new TableroDetalleViewModel
        {
            TableroId = id,
            Nombre = tareasTablero.First().TableroNombre
        };

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
