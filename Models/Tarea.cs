using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TPFinalAPI.Models
{
    public enum Prioridad
{
    Low,
    High,
    PrendidaFuego
}

public class Tarea
{
    public long Id { get; set; }
    public long TableroId { get; set; }
    public long ColumnaId { get; set; }

    public string Titulo { get; set; }
    public string Descripcion { get; set; }
    public Prioridad Prioridad { get; set; }

    public int? TiempoEstimadoMin { get; set; }

    public DateTime? FechaInicio { get; set; }
    public DateTime? FechaFin { get; set; }

    public string Tipo { get; set; }
    public long? CreadoPor { get; set; }

    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }

    // Propiedades extendidas (por joins)
    public string ColumnaNombre { get; set; }
    public string TableroNombre { get; set; }
}

}