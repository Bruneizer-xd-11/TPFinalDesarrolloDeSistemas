using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DapperData.Models;


public class Tablero
{
    public long Id { get; set; }
    public string Nombre { get; set; }
    public string Descripcion { get; set; }

    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}

