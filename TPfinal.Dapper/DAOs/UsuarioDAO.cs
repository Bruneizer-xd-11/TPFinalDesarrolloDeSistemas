using Dapper;
using Models;
using TPfinal.Dapper.DapperData;

namespace TPfinal.Dapper.DAOs
{
    public class UsuarioDAO
    {
        private readonly DbConnectionFactory _connectionFactory;

        public UsuarioDAO(DbConnectionFactory connectionFactory)
        {
            _connectionFactory = connectionFactory;
        }

        public async Task<IEnumerable<Usuario>> ObtenerTodosAsync()
        {
            using var connection = _connectionFactory.CreateConnection();
            const string sql = "SELECT * FROM usuarios;";
            return await connection.QueryAsync<Usuario>(sql);
        }

        public async Task<Usuario?> ObtenerPorIdAsync(long id)
        {
            using var connection = _connectionFactory.CreateConnection();
            const string sql = "SELECT * FROM usuarios WHERE id = @id;";
            return await connection.QueryFirstOrDefaultAsync<Usuario>(sql, new { id });
        }

        public async Task<long> InsertarAsync(Usuario usuario)
        {
            using var connection = _connectionFactory.CreateConnection();
            const string sql = @"
                INSERT INTO usuarios (nombre, email, username, password_hash)
                VALUES (@Nombre, @Email, @Username, @PasswordHash);
                SELECT LAST_INSERT_ID();";
            return await connection.ExecuteScalarAsync<long>(sql, usuario);
        }

        public async Task<bool> ActualizarAsync(Usuario usuario)
        {
            using var connection = _connectionFactory.CreateConnection();
            const string sql = @"
                UPDATE usuarios
                SET nombre = @Nombre, email = @Email, username = @Username, password_hash = @PasswordHash
                WHERE id = @Id;";
            var filas = await connection.ExecuteAsync(sql, usuario);
            return filas > 0;
        }

        public async Task<bool> EliminarAsync(long id)
        {
            using var connection = _connectionFactory.CreateConnection();
            const string sql = "DELETE FROM usuarios WHERE id = @id;";
            var filas = await connection.ExecuteAsync(sql, new { id });
            return filas > 0;
        }
    }
}