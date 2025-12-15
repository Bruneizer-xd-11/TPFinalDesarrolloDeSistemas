using DapperData.Models;

namespace MVC.Models
{
    public class TableroColumnViewModel
    {
        public long ColumnaId { get; set; }
        public string Nombre { get; set; } = "";
        public List<Tarea> Tareas { get; set; } = new();
    }

}
