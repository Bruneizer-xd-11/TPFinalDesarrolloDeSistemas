using System.Collections.Generic;

namespace DapperData.Models;


    public class TableroColumnViewModel
    {
        public long ColumnaId { get; set; }
        public string Nombre { get; set; } = "";
        public List<Tarea> Tareas { get; set; } = new();
    }

    public class TableroDetalleViewModel
    {
        public long TableroId { get; set; }
        public string Nombre { get; set; } = "";
        public List<TableroColumnViewModel> Columnas { get; set; } = new();
    }

