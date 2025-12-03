namespace DapperData.Models;



public class TareaCreateDto
{
    public required string Titulo { get; set; }
    public required string Descripcion { get; set; }
    public required string Tipo { get; set; }
    public required string ColumnaNombre { get; set; }
    public required string TableroNombre { get; set; }
    public long UsuarioId { get; set; }  // ID del usuario dueño
    // Otras props si las tenés (ej: Prioridad, FechaVencimiento)
}