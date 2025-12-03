// Archivo: TareaUpdateDto.cs  (dentro de TPFinalAPI o TPFinalAPI.Models)
namespace TPFinalAPI.Models;

public class TareaUpdateDto
{
    public string? Titulo { get; set; }
    public string? Descripcion { get; set; }
    public string? Tipo { get; set; }
    public string? ColumnaNombre { get; set; }
    public string? TableroNombre { get; set; }
    public Prioridad? Prioridad { get; set; }
    public int? TiempoEstimadoMin { get; set; }
    // etc... las que quieras permitir actualizar
}