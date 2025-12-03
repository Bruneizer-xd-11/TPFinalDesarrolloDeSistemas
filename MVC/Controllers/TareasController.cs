using Microsoft.AspNetCore.Mvc;
using DapperData.Models;
using Persistencia;


namespace MVC.Controllers
{
    public class TareasController : Controller
    {
        private readonly IDao _dao;
        public TareasController(IDao dao) => _dao = dao;

        public IActionResult Crear(long tableroId = 1, long columnaId = 0)
        {
            var vm = new TareaCreateViewModel
            {
                TableroId = tableroId,
                ColumnaId = columnaId
            };
            return View(vm);
        }

        [HttpPost]
        public async Task<IActionResult> Crear(TareaCreateViewModel vm)
        {
            if (!ModelState.IsValid) return View(vm);

            var tarea = new Tarea
            {
                TableroId = vm.TableroId,
                ColumnaId = vm.ColumnaId,
                Titulo = vm.Titulo,
                Descripcion = vm.Descripcion,
                Tipo = vm.Tipo ?? "General",
                Prioridad = vm.Prioridad,
                TiempoEstimadoMin = vm.TiempoEstimadoMin,
                FechaInicio = vm.FechaInicio,
                FechaFin = vm.FechaFin,
                CreadoPor = vm.CreadoPor ?? 1,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            await _dao.CrearTarea(tarea);
            return RedirectToAction("Detalle", "Tableros", new { id = tarea.TableroId });
        }

        public async Task<IActionResult> Editar(long id)
        {
            var t = await _dao.ObtenerTareaPorId(id);
            if (t == null) return NotFound();

            var vm = new TareaEditViewModel
            {
                Id = t.Id,
                Titulo = t.Titulo,
                Descripcion = t.Descripcion,
                Tipo = t.Tipo,
                ColumnaId = t.ColumnaId,
                TableroId = t.TableroId,
                Prioridad = t.Prioridad,
                TiempoEstimadoMin = t.TiempoEstimadoMin,
                FechaInicio = t.FechaInicio,
                FechaFin = t.FechaFin
            };
            return View(vm);
        }

        [HttpPost]
        public async Task<IActionResult> Editar(TareaEditViewModel vm)
        {
            if (!ModelState.IsValid) return View(vm);

            var tarea = await _dao.ObtenerTareaPorId(vm.Id);
            if (tarea == null) return NotFound();

            tarea.Titulo = vm.Titulo;
            tarea.Descripcion = vm.Descripcion;
            tarea.Tipo = vm.Tipo;
            tarea.ColumnaId = vm.ColumnaId;
            tarea.Prioridad = vm.Prioridad;
            tarea.TiempoEstimadoMin = vm.TiempoEstimadoMin;
            tarea.FechaInicio = vm.FechaInicio;
            tarea.FechaFin = vm.FechaFin;
            tarea.UpdatedAt = DateTime.UtcNow;

            await _dao.ActualizarTarea(tarea);
            return RedirectToAction("Detalle", "Tableros", new { id = tarea.TableroId });
        }

        [HttpPost]
        public async Task<IActionResult> Eliminar(long id)
        {
            var tarea = await _dao.ObtenerTareaPorId(id);
            if (tarea == null) return NotFound();

            await _dao.EliminarTarea(id);
            return RedirectToAction("Detalle", "Tableros", new { id = tarea.TableroId });
        }
    }
}
