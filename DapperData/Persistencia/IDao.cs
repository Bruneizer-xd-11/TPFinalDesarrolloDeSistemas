using Models;

namespace Persistencia
{
    public interface IDao
    {
        // ========================= USUARIOS =========================
        Task<IEnumerable<Usuario>> Usuario_ObtenerTodos();
        Task<Usuario?> Usuario_ObtenerPorId(long id);
        Task<long> Usuario_Insertar(Usuario usuario);
        Task<bool> Usuario_Actualizar(Usuario usuario);
        Task<bool> Usuario_Eliminar(long id);

        // ========================= TAREAS =========================
        Task<IEnumerable<Tarea>> Tarea_ObtenerTodas();
        Task<Tarea?> Tarea_ObtenerPorId(long id);
        Task<long> Tarea_Insertar(Tarea tarea);
        Task<bool> Tarea_Actualizar(Tarea tarea);
        Task<bool> Tarea_Eliminar(long id);

        // ========================= TABLEROS =========================
        Task<IEnumerable<Tablero>> Tablero_ObtenerTodos();
        Task<Tablero?> Tablero_ObtenerPorId(long id);
        Task<long> Tablero_Insertar(Tablero tablero);
        Task<bool> Tablero_Actualizar(Tablero tablero);
        Task<bool> Tablero_Eliminar(long id);

        // ========================= COLUMNAS =========================
        Task<IEnumerable<Columna>> Columna_ObtenerPorTablero(long tableroId);
        Task<long> Columna_Insertar(Columna columna);
        Task<bool> Columna_Actualizar(Columna columna);
        Task<bool> Columna_Eliminar(long id);

        // ========================= TABLEROS COMPARTIDOS =========================
        Task TableroCompartir(long tableroId, long userId);
        Task<IEnumerable<Tablero>> Tablero_ObtenerCompartidos(long userId);
        Task RevocarCompartido(long tableroId, long userId);
        Task<IEnumerable<Usuario>> UsuariosConAcceso(long tableroId);
    }
}
