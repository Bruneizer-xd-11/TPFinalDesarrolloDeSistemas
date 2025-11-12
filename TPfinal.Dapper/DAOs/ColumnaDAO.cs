using Dapper;
using Models;
using TPfinal.Dapper.DapperData;

namespace TPfinal.Dapper.DAOs
{
    public class ColumnaDAO
    {
        private readonly DbConnectionFactory _connectionFactory;

        public ColumnaDAO(DbConnectionFactory connectionFactory)
        {
            _connectionFactory = connectionFactory;
        }

        public async Task<IEnumerable<Columna>> ObtenerPorTableroAsync(long tableroId)
        {
            using var connection = _connectionFactory.CreateConnection();
            const string sql = "SELECT * FROM columnas WHERE tablero_id = @tableroId ORDER BY posicion;";
            return await connection.QueryAsync<Columna>(sql, new { tableroId });
        }

        public async Task<long> InsertarAsync(Columna columna)
        {
            using var connection = _connectionFactory.CreateConnection();
            const string sql = @"
                INSERT INTO columnas (tablero_id, nombre, posicion)
                VALUES (@TableroId, @Nombre, @Posicion);
                SELECT LAST_INSERT_ID();";
            return await connection.ExecuteScalarAsync<long>(sql, columna);
        }

        public async Task<bool> ActualizarAsync(Columna columna)
        {
            using var connection = _connectionFactory.CreateConnection();
            const string sql = @"
                UPDATE columnas
                SET nombre = @Nombre, posicion = @Posicion
                WHERE id = @Id;";
            var filas = await connection.ExecuteAsync(sql, columna);
            return filas > 0;
        }

        public async Task<bool> EliminarAsync(long id)
        {
            using var connection = _connectionFactory.CreateConnection();
            const string sql = "DELETE FROM columnas WHERE id = @id;";
            var filas = await connection.ExecuteAsync(sql, new { id });
            return filas > 0;
        }
    }
}
