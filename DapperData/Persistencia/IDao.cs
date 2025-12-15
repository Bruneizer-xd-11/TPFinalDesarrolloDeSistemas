using System.Collections.Generic;
using System.Threading.Tasks;
using MySqlConnector;
using DapperData.Models;

namespace Persistencia;

public interface IDao
{
    // ==== TABLEROS === //
    Task<long> CrearTablero(Tablero tablero);
    Task<IEnumerable<Tablero>> ObtenerTablerosPorUsuario(long usuarioId);
    Task<bool> EliminarTablero(long id);

    // ==== COLUMNAS === //
    Task<long> CrearColumna(Columna columna);
    Task<List<Columna>> ObtenerColumnasPorTablero(long tableroId);

    // ==== TAREAS ====
    Task<IEnumerable<Tarea>> ObtenerTareas();
    Task<Tarea?> ObtenerTareaPorId(long id);
    Task<long> CrearTarea(Tarea tarea);
    Task<bool> ActualizarTarea(Tarea tarea);
    Task<bool> EliminarTarea(long id);

    // ==== USUARIOS ====
    Task<long> RegistrarUsuario(Usuario usuario);
    Task<Usuario?> ObtenerUsuarioPorId(long id);
    Task<Usuario?> ObtenerUsuarioPorUsername(string username);

    // ==== FILTROS ====
    Task<IEnumerable<Tarea>> ObtenerTareasPorEstado(string columna);
    Task<IEnumerable<Tarea>> ObtenerTareasPorPrioridad(string prioridad);

    // ==== TEST ====
    Task<MySqlConnection> ProbarConexion();
}
