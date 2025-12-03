using System;
using System.ComponentModel.DataAnnotations;

namespace DapperData.Models;


    public class TareaCreateViewModel
    {
        public long TableroId { get; set; }
        public long ColumnaId { get; set; }
        [Required] public string Titulo { get; set; } = "";
        public string Descripcion { get; set; } = "";
        public string? Tipo { get; set; }
        public Prioridad Prioridad { get; set; } = Prioridad.Low;
        public int? TiempoEstimadoMin { get; set; }
        public DateTime? FechaInicio { get; set; }
        public DateTime? FechaFin { get; set; }
        public long? CreadoPor { get; set; }
    }

    public class TareaEditViewModel : TareaCreateViewModel
    {
        public long Id { get; set; }
    }

