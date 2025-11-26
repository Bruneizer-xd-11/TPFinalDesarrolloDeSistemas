using Dapper;
using Models;
using Persistencia;
using TPfinal.Dapper.DapperData;

namespace Persistencia
{
    public class DapperDao : IDao
    {
        private readonly DbConnectionFactory _connectionFactory;

        public DapperDao(DbConnectionFactory connectionFactory)
        {
            _connectionFactory = connectionFactory;
        }

        // ================= USUARIOS =================

        public async Task<IEnumerable<Usuario>> Usuario_ObtenerTodos()
        {
            using var db = _connectionFactory.CreateConnection();
            return await db.QueryAsync<Usuario>("SELECT * FROM usuarios;");
        }

        public async Task<Usuario?> Usuario_ObtenerPorId(long id)
        {
            using var db = _connectionFactory.CreateConnection();
            return await db.QueryFirstOrDefaultAsync<Usuario>("SELECT * FROM usuarios WHERE id=@id", new { id });
        }

        public async Task<long> Usuario_Insertar(Usuario u)
        {
            using var db = _connectionFactory.CreateConnection();
            return await db.ExecuteScalarAsync<long>(@"
                INSERT INTO usuarios (nombre, email, username, password_hash)
                VALUES (@Nombre,@Email,@Username,@PasswordHash);
                SELECT LAST_INSERT_ID();", u);
        }

        public async Task<bool> Usuario_Actualizar(Usuario u)
        {
            using var db = _connectionFactory.CreateConnection();
            return await db.ExecuteAsync(@"
                UPDATE usuarios SET nombre=@Nombre,email=@Email,username=@Username,password_hash=@PasswordHash
                WHERE id=@Id", u) > 0;
        }

        public async Task<bool> Usuario_Eliminar(long id)
        {
            using var db = _connectionFactory.CreateConnection();
            return await db.ExecuteAsync("DELETE FROM usuarios WHERE id=@id", new { id }) > 0;
        }

        // ================= TAREAS (con stored procedures) =================

        public async Task<IEnumerable<Tarea>> Tarea_ObtenerTodas()
        {
            using var db = _connectionFactory.CreateConnection();
            return await db.QueryAsync<Tarea>("CALL sp_get_tareas();");
        }

        public async Task<Tarea?> Tarea_ObtenerPorId(long id)
        {
            using var db = _connectionFactory.CreateConnection();
            return await db.QueryFirstOrDefaultAsync<Tarea>("CALL sp_get_tarea_por_id(@id);", new { id });
        }

        public async Task<long> Tarea_Insertar(Tarea t)
        {
            using var db = _connectionFactory.CreateConnection();
            return await db.ExecuteScalarAsync<long>(@"
                CALL sp_crear_tarea(
                    @TableroId,@ColumnaId,@Titulo,@Descripcion,@Prioridad,
                    @TiempoEstimadoMin,@FechaInicio,@FechaFin,@Tipo,@CreadoPor
                );", t);
        }

        public async Task<bool> Tarea_Actualizar(Tarea t)
        {
            using var db = _connectionFactory.CreateConnection();
            return await db.ExecuteScalarAsync<int>(@"
                CALL sp_actualizar_tarea(
                    @Id,@TableroId,@ColumnaId,@Titulo,@Descripcion,@Prioridad,
                    @TiempoEstimadoMin,@FechaInicio,@FechaFin,@Tipo,@CreadoPor
                );", t) > 0;
        }

        public async Task<bool> Tarea_Eliminar(long id)
        {
            using var db = _connectionFactory.CreateConnection();
            return await db.ExecuteScalarAsync<int>("CALL sp_eliminar_tarea(@id);", new { id }) > 0;
        }

        // ================= TABLEROS =================

        public async Task<IEnumerable<Tablero>> Tablero_ObtenerTodos()
        {
            using var db = _connectionFactory.CreateConnection();
            return await db.QueryAsync<Tablero>("SELECT * FROM tableros;");
        }

        public async Task<Tablero?> Tablero_ObtenerPorId(long id)
        {
            using var db = _connectionFactory.CreateConnection();
            return await db.QueryFirstOrDefaultAsync<Tablero>("SELECT * FROM tableros WHERE id=@id", new { id });
        }

        public async Task<long> Tablero_Insertar(Tablero t)
        {
            using var db = _connectionFactory.CreateConnection();
            return await db.ExecuteScalarAsync<long>(@"
                INSERT INTO tableros(nombre,descripcion) VALUES(@Nombre,@Descripcion);
                SELECT LAST_INSERT_ID();", t);
        }

        public async Task<bool> Tablero_Actualizar(Tablero t)
        {
            using var db = _connectionFactory.CreateConnection();
            return await db.ExecuteAsync(@"
                UPDATE tableros SET nombre=@Nombre,descripcion=@Descripcion WHERE id=@Id", t) > 0;
        }

        public async Task<bool> Tablero_Eliminar(long id)
        {
            using var db = _connectionFactory.CreateConnection();
            return await db.ExecuteAsync("DELETE FROM tableros WHERE id=@id", new { id }) > 0;
        }

        // ================= COLUMNAS =================

        public async Task<IEnumerable<Columna>> Columna_ObtenerPorTablero(long tableroId)
        {
            using var db = _connectionFactory.CreateConnection();
            return await db.QueryAsync<Columna>("SELECT * FROM columnas WHERE tablero_id=@tableroId ORDER BY posicion", new { tableroId });
        }

        public async Task<long> Columna_Insertar(Columna c)
        {
            using var db = _connectionFactory.CreateConnection();
            return await db.ExecuteScalarAsync<long>(@"
                INSERT INTO columnas(tablero_id,nombre,posicion)
                VALUES(@TableroId,@Nombre,@Posicion);
                SELECT LAST_INSERT_ID();", c);
        }

        public async Task<bool> Columna_Actualizar(Columna c)
        {
            using var db = _connectionFactory.CreateConnection();
            return await db.ExecuteAsync(@"
                UPDATE columnas SET nombre=@Nombre,posicion=@Posicion WHERE id=@Id", c) > 0;
        }

        public async Task<bool> Columna_Eliminar(long id)
        {
            using var db = _connectionFactory.CreateConnection();
            return await db.ExecuteAsync("DELETE FROM columnas WHERE id=@id", new { id }) > 0;
        }

        // ================= COMPARTIR TABLEROS =================

        public async Task TableroCompartir(long tableroId, long userId)
        {
            using var db = _connectionFactory.CreateConnection();
            await db.ExecuteAsync("INSERT INTO tablero_compartido VALUES(@tableroId,@userId);", new { tableroId, userId });
        }

        public async Task<IEnumerable<Tablero>> Tablero_ObtenerCompartidos(long userId)
        {
            using var db = _connectionFactory.CreateConnection();
            return await db.QueryAsync<Tablero>(@"
                SELECT t.* FROM tableros t
                JOIN tablero_compartido tc ON t.id=tc.tablero_id
                WHERE tc.usuario_id=@userId;", new { userId });
        }

        public async Task RevocarCompartido(long tableroId, long userId)
        {
            using var db = _connectionFactory.CreateConnection();
            await db.ExecuteAsync("DELETE FROM tablero_compartido WHERE tablero_id=@tableroId AND usuario_id=@userId;", new { tableroId, userId });
        }

        public async Task<IEnumerable<Usuario>> UsuariosConAcceso(long tableroId)
        {
            using var db = _connectionFactory.CreateConnection();
            return await db.QueryAsync<Usuario>(@"
                SELECT u.* FROM usuarios u
                JOIN tablero_compartido tc ON u.id=tc.usuario_id
                WHERE tc.tablero_id=@tableroId;", new { tableroId });
        }
    }
}
