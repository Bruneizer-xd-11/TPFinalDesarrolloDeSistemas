using DapperData.Models;

namespace MVC.Models
{
public class TableroDetalleViewModel
{
    public long TableroId { get; set; }
    public string Nombre { get; set; }
    public List<Seccion> Secciones { get; set; } = new();
}

}
