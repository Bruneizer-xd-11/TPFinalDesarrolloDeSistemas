using System.Collections.Generic;
using DapperData.Models;

namespace MVC.ViewModels
{
    public class EditarTareaViewModel
    {
        public Tarea Tarea { get; set; } = new Tarea();
        public List<Columna> Columnas { get; set; } = new List<Columna>();
    }
}
