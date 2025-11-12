using Dapper;
using Models;
using TPfinal.Dapper.DapperData;

namespace TPfinal.Dapper.DAOs
{
    public class TableroDAO
    {
        private readonly DbConnectionFactory _connectionFactory;

        public TableroDAO(DbConnectionFactory connectionFactory)
        {
            _connectionFactory = connectionFactory;
        }

        public async Task<IEnumerable<Tablero>> ObtenerTodosAsync()
        {
            using var connection = _connectionFactory.CreateConnection();
            const string sql = "SELECT * FROM tableros;";
            return await connection.QueryAsync<Tablero>(sql);
        }

        public async Task<Tablero?> ObtenerPorIdAsync(long id)
        {
            using var connection = _connectionFactory.CreateConnection();
            const string sql = "SELECT * FROM tableros WHERE id = @id;";
            return await connection.QueryFirstOrDefaultAsync<Tablero>(sql, new { id });
        }

        public async Task<long> InsertarAsync(Tablero tablero)
        {
            using var connection = _connectionFactory.CreateConnection();
            const string sql = @"
                INSERT INTO tableros (nombre, descripcion)
                VALUES (@Nombre, @Descripcion);
                SELECT LAST_INSERT_ID();";
            return await connection.ExecuteScalarAsync<long>(sql, tablero);
        }

        public async Task<bool> ActualizarAsync(Tablero tablero)
        {
            using var connection = _connectionFactory.CreateConnection();
            const string sql = @"
                UPDATE tableros
                SET nombre = @Nombre, descripcion = @Descripcion
                WHERE id = @Id;";
            var filas = await connection.ExecuteAsync(sql, tablero);
            return filas > 0;
        }

        public async Task<bool> EliminarAsync(long id)
        {
            using var connection = _connectionFactory.CreateConnection();
            const string sql = "DELETE FROM tableros WHERE id = @id;";
            var filas = await connection.ExecuteAsync(sql, new { id });
            return filas > 0;
        }
    }
}
