using Microsoft.AspNetCore.Mvc;
using DapperData.Models;
using Persistencia;
using MVC.Models;

namespace MVC.Controllers
{
    public class TablerosController : Controller
    {
        private readonly IDao _dao;
        public TablerosController(IDao dao) => _dao = dao;

        public IActionResult Index()
        {
            return View();
        }

        public async Task<IActionResult> Detalle(long id = 1)
        {
            // Trae todas las tareas desde el DAO
            var tareas = (await _dao.ObtenerTareas())
                .Where(t => t.TableroId == id)
                .ToList();

            // Agrupa por columna
            var columnas = tareas
    .GroupBy(t => new { t.ColumnaId, Nombre = t.ColumnaNombre ?? "Sin columna" })
    .Select(g => new MVC.Models.TableroColumnViewModel
    {
        ColumnaId = g.Key.ColumnaId,
        Nombre = g.Key.Nombre,
        Tareas = g.OrderByDescending(t => t.CreatedAt).ToList()
    })
    .OrderBy(c => c.ColumnaId)
    .ToList();

            var vm = new MVC.Models.TableroDetalleViewModel
            {
                TableroId = id,
                Nombre = "Demo Board",
                Columnas = columnas
            };


            return View(vm);
        }
    }
}
