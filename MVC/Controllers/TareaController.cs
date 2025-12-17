using Microsoft.AspNetCore.Mvc;
using Persistencia;
using DapperData.Models;
using MVC.ViewModels;
using System.Threading.Tasks;

namespace MVC.Controllers;

public class TareaController : AuthenticatedController
{
    private readonly IDao _dao;

    public TareaController(IDao dao)
    {
        _dao = dao;
    }

    // Crear tarea (GET)
    public async Task<IActionResult> Create(long tableroId)
    {
        var columnas = await _dao.ObtenerColumnasPorTablero(tableroId);

        var vm = new CrearTareaViewModel
        {
            TableroId = tableroId,
            Columnas = columnas
        };

        return View(vm);
    }

    // Crear tarea (POST)
    [HttpPost]
    public async Task<IActionResult> CrearTarea(CrearTareaViewModel vm)
    {
        if (!ModelState.IsValid)
        {
            vm.Columnas = await _dao.ObtenerColumnasPorTablero(vm.TableroId);
            return View(vm);
        }

        vm.Tarea.CreadoPor = UsuarioIdActual;
        await _dao.CrearTarea(vm.Tarea);

        return RedirectToAction("Detalle", "Tableros", new { id = vm.TableroId });
    }

    // Editar tarea (GET)
    public async Task<IActionResult> EditarTarea(long id)
    {
        var tarea = await _dao.ObtenerTareaPorId(id);
        if (tarea == null) return NotFound();

        var columnas = await _dao.ObtenerColumnasPorTablero(tarea.TableroId);

        var vm = new EditarTareaViewModel
        {
            Tarea = tarea,
            Columnas = columnas
        };

        return View(vm);
    }

    // Editar tarea (POST)
    [HttpPost]
    public async Task<IActionResult> EditarTarea(EditarTareaViewModel vm)
    {
        if (!ModelState.IsValid)
        {
            vm.Columnas = await _dao.ObtenerColumnasPorTablero(vm.Tarea.TableroId);
            return View(vm);
        }

        await _dao.ActualizarTarea(vm.Tarea);
        return RedirectToAction("Detalle", "Tableros", new { id = vm.Tarea.TableroId });
    }

    // Eliminar tarea
    public async Task<IActionResult> BorrarTarea(long id)
    {
        var tarea = await _dao.ObtenerTareaPorId(id);
        if (tarea == null) return NotFound();

        await _dao.EliminarTarea(id);
        return RedirectToAction("Detalle", "Tableros", new { id = tarea.TableroId });
    }
    [HttpPost]
public async Task<IActionResult> Mover([FromBody] MoverTareaDto dto)
{
    var tarea = await _dao.ObtenerTareaPorId(dto.TareaId);
    if (tarea == null) return NotFound();

    tarea.ColumnaId = dto.ColumnaId;

    await _dao.ActualizarTarea(tarea);

    return Ok();
}
}
