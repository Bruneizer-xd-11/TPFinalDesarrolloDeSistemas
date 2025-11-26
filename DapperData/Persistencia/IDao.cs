using Models;

namespace Persistencia;

public interface IDao
{
    // ==== TAREAS ====
    Task<IEnumerable<Tarea>> ObtenerTareas();
    Task<Tarea?> ObtenerTareaPorId(long id);
    Task<long> CrearTarea(Tarea tarea);
    Task<bool> ActualizarTarea(Tarea tarea);
    Task<bool> EliminarTarea(long id);

    // ==== USUARIOS ====
    Task<long> RegistrarUsuario(Usuario usuario);

    // ==== FILTROS ====
    Task<IEnumerable<Tarea>> ObtenerTareasPorEstado(string columna);
    Task<IEnumerable<Tarea>> ObtenerTareasPorPrioridad(string prioridad);
}
