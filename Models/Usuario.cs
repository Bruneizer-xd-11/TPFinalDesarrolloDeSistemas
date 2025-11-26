using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
namespace Models;

public class Usuario
{
    public long Id { get; set; }
    public string Nombre { get; set; }
    public string Email { get; set; }
    public string Username { get; set; }
    public string PasswordHash { get; set; }

    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}

