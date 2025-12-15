using System;
using System.Threading.Tasks;
using Dapper;
using MySqlConnector;
using DapperData.Models;


namespace Persistencia;

public class DaoDappers : IDao
{
    private readonly string _connectionString;

    public DaoDappers(string connectionString)
    {
        _connectionString = connectionString;
    }

    private MySqlConnection GetConnection() => new MySqlConnection(_connectionString);
// ====================== Tableros===================================//
    public async Task<long> CrearTablero(Tablero tablero)
    {
        using var db = GetConnection();
        return await db.ExecuteScalarAsync<long>(
            "sp_crear_tablero",
            new { p_nombre = tablero.Nombre, p_descripcion = tablero.Descripcion, p_propietario_id = tablero.PropietarioId },
            commandType: System.Data.CommandType.StoredProcedure
        );
    }
    public async Task<IEnumerable<Tablero>> ObtenerTablerosPorUsuario(long usuarioId)
    {
        using var db = GetConnection();
        return await db.QueryAsync<Tablero>(
            "sp_get_tableros_por_usuario",
            new { p_usuario_id = usuarioId },
            commandType: System.Data.CommandType.StoredProcedure
        );
    }



    // =====================  TAREAS  ===================== //

    public async Task<IEnumerable<Tarea>> ObtenerTareas()
    {
        using var db = GetConnection();
        return await db.QueryAsync<Tarea>("sp_get_tareas", commandType: System.Data.CommandType.StoredProcedure);
    }

    public async Task<Tarea?> ObtenerTareaPorId(long id)
    {
        using var db = GetConnection();
        return await db.QueryFirstOrDefaultAsync<Tarea>(
            "sp_get_tarea_por_id",
            new { p_id = id },
            commandType: System.Data.CommandType.StoredProcedure
        );
    }

    public async Task<long> CrearTarea(Tarea tarea)
    {
        using var db = GetConnection();
        return await db.ExecuteScalarAsync<long>(
            "sp_crear_tarea",
            new
            {
                p_tablero_id = tarea.TableroId,
                p_columna_id = tarea.ColumnaId,
                p_titulo = tarea.Titulo,
                p_descripcion = tarea.Descripcion,
                p_prioridad = tarea.Prioridad.ToString(),
                p_tiempo_estimado_min = tarea.TiempoEstimadoMin,
                p_fecha_inicio = tarea.FechaInicio,
                p_fecha_fin = tarea.FechaFin,
                p_tipo = tarea.Tipo,
                p_creado_por = tarea.CreadoPor
            },
            commandType: System.Data.CommandType.StoredProcedure
        );
    }

    public async Task<bool> ActualizarTarea(Tarea tarea)
    {
        using var db = GetConnection();
        var result = await db.ExecuteScalarAsync<int>(
            "sp_actualizar_tarea",
            new
            {
                p_id = tarea.Id,
                p_tablero_id = tarea.TableroId,
                p_columna_id = tarea.ColumnaId,
                p_titulo = tarea.Titulo,
                p_descripcion = tarea.Descripcion,
                p_prioridad = tarea.Prioridad.ToString(),
                p_tiempo_estimado_min = tarea.TiempoEstimadoMin,
                p_fecha_inicio = tarea.FechaInicio,
                p_fecha_fin = tarea.FechaFin,
                p_tipo = tarea.Tipo,
                p_creado_por = tarea.CreadoPor
            },
            commandType: System.Data.CommandType.StoredProcedure
        );

        return result > 0;
    }

    public async Task<bool> EliminarTarea(long id)
    {
        using var db = GetConnection();
        var result = await db.ExecuteScalarAsync<int>(
            "sp_eliminar_tarea",
            new { p_id = id },
            commandType: System.Data.CommandType.StoredProcedure
        );

        return result > 0;
    }

    // ===================== NUEVOS MÉTODOS REQUERIDOS ===================== //

    public async Task<List<Tarea>> ObtenerTareasDeUsuario(long usuarioId)
    {
        using var db = GetConnection();
        return (await db.QueryAsync<Tarea>(
            "sp_get_tareas_de_usuario",
            new { p_usuario_id = usuarioId },
            commandType: System.Data.CommandType.StoredProcedure
        )).AsList();
    }

    public async Task<Usuario?> ObtenerUsuarioPorId(long id)
    {
        using var db = GetConnection();
        return await db.QueryFirstOrDefaultAsync<Usuario>(
            "sp_get_usuario_por_id",
            new { p_id = id },
            commandType: System.Data.CommandType.StoredProcedure
        );
    }

    public async Task<Usuario?> ObtenerUsuarioPorUsername(string username)
    {
        using var db = GetConnection();
        return await db.QueryFirstOrDefaultAsync<Usuario>(
            "sp_get_usuario_por_username",
            new { p_username = username },
            commandType: System.Data.CommandType.StoredProcedure
        );
    }

    // ===================== USUARIOS ===================== //

    public async Task<long> RegistrarUsuario(Usuario usuario)
    {
        using var db = GetConnection();
        return await db.ExecuteScalarAsync<long>(
            "sp_registrar_usuario",
            new
            {
                p_nombre = usuario.Nombre,
                p_email = usuario.Email,
                p_username = usuario.Username,
                p_password_hash = usuario.PasswordHash
            },
            commandType: System.Data.CommandType.StoredProcedure
        );
    }

    // ===================== FILTROS ===================== //

    public async Task<IEnumerable<Tarea>> ObtenerTareasPorEstado(string columna)
    {
        using var db = GetConnection();
        return await db.QueryAsync<Tarea>(
            "sp_get_tareas_por_estado",
            new { p_nombre_columna = columna },
            commandType: System.Data.CommandType.StoredProcedure
        );
    }

    public async Task<IEnumerable<Tarea>> ObtenerTareasPorPrioridad(string prioridad)
    {
        using var db = GetConnection();
        return await db.QueryAsync<Tarea>(
            "sp_get_tareas_por_prioridad",
            new { p_prioridad = prioridad },
            commandType: System.Data.CommandType.StoredProcedure
        );
    }

    // ===================== TEST ===================== //

    public async Task<MySqlConnection> ProbarConexion()
    {
        var conn = GetConnection();
        await conn.OpenAsync();
        return conn;
    }
}