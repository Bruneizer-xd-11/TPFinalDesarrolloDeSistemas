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
        // USAMOS EL MÉTODO QUE SÍ TENÉS
        var tareas = await _dao.ObtenerTareas();

        // Agrupar por tablero
        var tableros = tareas
            .GroupBy(t => new { t.TableroId, t.TableroNombre })
            .Select(g => new Tablero
            {
                Id = g.Key.TableroId,
                Nombre = g.Key.TableroNombre
            })
            .ToList();

        return View(tableros);
    }

    public async Task<IActionResult> Detalle(long id)
    {
        var tareas = await _dao.ObtenerTareas();
        var tareasTablero = tareas.Where(t => t.TableroId == id).ToList();

        if (!tareasTablero.Any())
            return NotFound();

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
