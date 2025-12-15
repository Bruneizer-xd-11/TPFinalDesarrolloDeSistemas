using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DapperData.Models;
public class Seccion  // antes Columnas
    {
        public long Id { get; set; }
        public long TableroId { get; set; }
        public string Nombre { get; set; } = string.Empty;
        public int Posicion { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
    }
