using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TPfinal.Dapper.DAOs
{
    public class TableroCompartidoDAO
    {
        private readonly DbConnectionFactory _connectionFactory;

        public TableroCompartidoDAO(DbConnectionFactory connectionFactory)
        {
            _connectionFactory = connectionFactory;
        }

        public async Task CompartirAsync(long tableroId, long userId)
        {
            const string sql = "INSERT INTO tablero_compartido (tablero_id, usuario_id) VALUES (@tableroId, @usuarioId);";
            using var conn = _connectionFactory.CreateConnection();
            await conn.ExecuteAsync(sql, new { tableroId, usuarioId = userId });
        }

        public async Task<IEnumerable<Tablero>> ObtenerTablerosCompartidos(long userId)
        {
            using var conn = _connectionFactory.CreateConnection();
            const string sql = @"
            SELECT t.*
            FROM tableros t
            JOIN tablero_compartido tc ON t.id = tc.tablero_id
            WHERE tc.usuario_id = @userId;
        ";
            return await conn.QueryAsync<Tablero>(sql, new { userId });
        }

        public async Task RevocarAsync(long tableroId, long userId)
        {
            const string sql = "DELETE FROM tablero_compartido WHERE tablero_id = @tableroId AND usuario_id = @userId;";
            using var conn = _connectionFactory.CreateConnection();
            await conn.ExecuteAsync(sql, new { tableroId, userId });
        }


        public async Task<IEnumerable<Usuario>> ObtenerUsuariosConAcceso(long tableroId)
        {
            using var conn = _connectionFactory.CreateConnection();
            const string sql = @"
        SELECT u.*
        FROM usuarios u
        JOIN tablero_compartido tc ON u.id = tc.usuario_id
        WHERE tc.tablero_id = @tableroId;
    ";
            return await conn.QueryAsync<Usuario>(sql, new { tableroId });
        }


    }

}