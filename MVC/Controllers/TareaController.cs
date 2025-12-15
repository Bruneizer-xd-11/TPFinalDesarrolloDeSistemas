using Microsoft.AspNetCore.Mvc;
using DapperData.Models;
using Persistencia;

namespace MVC.Controllers;

public class TareaController : AuthenticatedController
{
    private readonly IDao _dao;

    public TareaController(IDao dao)
    {
        _dao = dao;
    }

    // GET
    public IActionResult Create(long tableroId, long columnaId)
    {
        return View(new TareaCreateViewModel
        {
            TableroId = tableroId,
            ColumnaId = columnaId
        });
    }

    // POST
[HttpPost]
public async Task<IActionResult> Create(TareaCreateViewModel vm)
{
    if (!ModelState.IsValid)
        return View(vm);

    var tarea = new Tarea
    {
        TableroId = vm.TableroId,
        ColumnaId = vm.ColumnaId,
        Titulo = vm.Titulo,
        Descripcion = vm.Descripcion,
        Tipo = vm.Tipo,
        Prioridad = vm.Prioridad,
        TiempoEstimadoMin = vm.TiempoEstimadoMin,
        FechaInicio = vm.FechaInicio,
        FechaFin = vm.FechaFin,
        CreadoPor = vm.CreadoPor,
        CreatedAt = DateTime.UtcNow,
        UpdatedAt = DateTime.UtcNow
    };

    await _dao.CrearTarea(tarea);

    return RedirectToAction("Detalle", "Tableros", new { id = vm.TableroId });
}

}
