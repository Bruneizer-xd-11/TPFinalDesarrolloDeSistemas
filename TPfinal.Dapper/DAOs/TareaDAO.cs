using Dapper;
using Models;
using TPfinal.Dapper.DapperData;

namespace TPfinal.Dapper.DAOs
{
    public class TareaDAO
    {
        private readonly DbConnectionFactory _connectionFactory;

        public TareaDAO(DbConnectionFactory connectionFactory)
        {
            _connectionFactory = connectionFactory;
        }

        public async Task<IEnumerable<Tarea>> ObtenerTodasAsync()
        {
            using var connection = _connectionFactory.CreateConnection();
            const string sql = "CALL sp_get_tareas();";
            return await connection.QueryAsync<Tarea>(sql);
        }

        public async Task<Tarea?> ObtenerPorIdAsync(long id)
        {
            using var connection = _connectionFactory.CreateConnection();
            const string sql = "CALL sp_get_tarea_por_id(@id);";
            return await connection.QueryFirstOrDefaultAsync<Tarea>(sql, new { id });
        }

        public async Task<long> InsertarAsync(Tarea tarea)
        {
            using var connection = _connectionFactory.CreateConnection();
            const string sql = @"
                CALL sp_crear_tarea(
                    @TableroId, @ColumnaId, @Titulo, @Descripcion, @Prioridad,
                    @TiempoEstimadoMin, @FechaInicio, @FechaFin, @Tipo, @CreadoPor
                );";
            return await connection.ExecuteScalarAsync<long>(sql, tarea);
        }

        public async Task<bool> ActualizarAsync(Tarea tarea)
        {
            using var connection = _connectionFactory.CreateConnection();
            const string sql = @"
                CALL sp_actualizar_tarea(
                    @Id, @TableroId, @ColumnaId, @Titulo, @Descripcion, @Prioridad,
                    @TiempoEstimadoMin, @FechaInicio, @FechaFin, @Tipo, @CreadoPor
                );";
            var filas = await connection.ExecuteScalarAsync<int>(sql, tarea);
            return filas > 0;
        }

        public async Task<bool> EliminarAsync(long id)
        {
            using var connection = _connectionFactory.CreateConnection();
            const string sql = "CALL sp_eliminar_tarea(@id);";
            var filas = await connection.ExecuteScalarAsync<int>(sql, new { id });
            return filas > 0;
        }
    }
}
