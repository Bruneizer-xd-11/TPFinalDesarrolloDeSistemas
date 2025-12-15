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

        // Crear columnas por defecto al crear un tablero
        await _dao.CrearColumna(new Columna { Nombre = "Por hacer", TableroId = idNuevoTablero });
        await _dao.CrearColumna(new Columna { Nombre = "En progreso", TableroId = idNuevoTablero });
        await _dao.CrearColumna(new Columna { Nombre = "Finalizado", TableroId = idNuevoTablero });

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

        // Construir ViewModel
        var vm = new TableroDetalleViewModel
        {
            TableroId = id,
            Nombre = tareasTablero.FirstOrDefault()?.TableroNombre ?? "Nuevo Tablero"
        };

        // Obtener columnas del tablero (para el formulario de crear tarea)
        var columnas = await _dao.ObtenerColumnasPorTablero(id);
        foreach (var col in columnas)
        {
            vm.Columnas.Add(new TableroColumnViewModel
            {
                ColumnaId = col.Id,
                Nombre = col.Nombre,
                Tareas = tareasTablero.Where(t => t.ColumnaId == col.Id).ToList()
            });
        }

        return View(vm);
    }

    // ======================
    // Crear tarea dentro del tablero
    // ======================
    [HttpPost]
    public async Task<IActionResult> CrearTarea(Tarea tarea)
    {
        if (tarea.ColumnaId == 0)
        {
            ModelState.AddModelError("", "Debe seleccionar una columna válida");
            return RedirectToAction("Detalle", new { id = tarea.TableroId });
        }

        tarea.CreadoPor = UsuarioIdActual;
        await _dao.CrearTarea(tarea);

        return RedirectToAction("Detalle", new { id = tarea.TableroId });
    }
}
