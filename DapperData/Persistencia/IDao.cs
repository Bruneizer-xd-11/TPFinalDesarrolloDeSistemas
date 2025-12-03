// Archivo: DapperData/Persistencia/IDao.cs
using MySqlConnector;                    // ← ESTA LÍNEA ES LA QUE FALTABA
using TPFinalAPI.Models;

namespace Persistencia;

public interface IDao
{
    // ==== TAREAS ====
    Task<IEnumerable<Tarea>> ObtenerTareas();
    Task<Tarea?> ObtenerTareaPorId(long id);
    Task<long> CrearTarea(Tarea tarea);
    Task<bool> ActualizarTarea(Tarea tarea);
    Task<bool> EliminarTarea(long id);

    // ==== NUEVOS ====
    Task<List<Tarea>> ObtenerTareasDeUsuario(long usuarioId);
    Task<Usuario?> ObtenerUsuarioPorId(long id);
    Task<Usuario?> ObtenerUsuarioPorUsername(string username);

    // ==== USUARIOS ====
    Task<long> RegistrarUsuario(Usuario usuario);

    // ==== FILTROS ====
    Task<IEnumerable<Tarea>> ObtenerTareasPorEstado(string columna);
    Task<IEnumerable<Tarea>> ObtenerTareasPorPrioridad(string prioridad);

    // ==== TEST ====
    Task<MySqlConnection> ProbarConexion();
}