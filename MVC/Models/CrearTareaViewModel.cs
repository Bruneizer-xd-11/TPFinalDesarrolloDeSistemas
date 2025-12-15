using System.Collections.Generic;
using DapperData.Models;

namespace MVC.ViewModels
{
    public class CrearTareaViewModel
    {
        public long TableroId { get; set; }
        public List<Columna> Columnas { get; set; } = new List<Columna>();
        public Tarea Tarea { get; set; } = new Tarea();
    }
}
