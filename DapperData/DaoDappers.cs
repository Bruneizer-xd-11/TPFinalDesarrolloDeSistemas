using System.Collections.Generic;
using System.Threading.Tasks;
using Dapper;
using MySqlConnector;
using DapperData.Models;

namespace Persistencia;

public class DaoDapper : IDao
{
    private readonly string _connectionString;

    public DaoDapper(string connectionString)
    {
        _connectionString = connectionString;
    }

    private MySqlConnection GetConnection() => new MySqlConnection(_connectionString);

    // =========================
    // TABLEROS
    // =========================
    public async Task<long> CrearTablero(Tablero tablero)
    {
        using var conn = GetConnection();
        return await conn.ExecuteScalarAsync<long>(
            "CALL sp_crear_tablero(@Nombre, @Descripcion, @PropietarioId);",
            tablero);
    }

    public async Task<IEnumerable<Tablero>> ObtenerTablerosPorUsuario(long usuarioId)
    {
        using var conn = GetConnection();
        return await conn.QueryAsync<Tablero>(
            "CALL sp_get_tableros_por_usuario(@usuarioId);",
            new { usuarioId });
    }

    public async Task<bool> EliminarTablero(long id)
    {
        using var conn = GetConnection();
        var filas = await conn.ExecuteAsync(
            "CALL sp_eliminar_tablero(@id);",
            new { id });
        return filas > 0;
    }

    // =========================
    // COLUMNAS
    // =========================
    public async Task<long> CrearColumna(Columna columna)
    {
        using var conn = GetConnection();
        return await conn.ExecuteScalarAsync<long>(
            "CALL sp_crear_columna(@Nombre, @TableroId);",
            columna);
    }

    public async Task<List<Columna>> ObtenerColumnasPorTablero(long tableroId)
    {
        using var conn = GetConnection();
        var columnas = await conn.QueryAsync<Columna>(
            "CALL sp_get_columnas_por_tablero(@tableroId);",
            new { tableroId });
        return columnas.AsList();
    }

    // =========================
    // TAREAS
    // =========================
    public async Task<IEnumerable<Tarea>> ObtenerTareas()
    {
        using var conn = GetConnection();
        return await conn.QueryAsync<Tarea>("CALL sp_get_tareas();");
    }

    public async Task<Tarea?> ObtenerTareaPorId(long id)
    {
        using var conn = GetConnection();
        return await conn.QueryFirstOrDefaultAsync<Tarea>(
            "CALL sp_get_tarea_por_id(@id);", new { id });
    }

    public async Task<long> CrearTarea(Tarea tarea)
    {
        using var conn = GetConnection();
        return await conn.ExecuteScalarAsync<long>(
            "CALL sp_crear_tarea(@TableroId,@ColumnaId,@Titulo,@Descripcion,@Prioridad,@TiempoEstimadoMin,@FechaInicio,@FechaFin,@Tipo,@CreadoPor);",
            tarea);
    }

    public async Task<bool> ActualizarTarea(Tarea tarea)
    {
        using var conn = GetConnection();
        var filas = await conn.ExecuteAsync(
            "CALL sp_actualizar_tarea(@Id,@TableroId,@ColumnaId,@Titulo,@Descripcion,@Prioridad,@TiempoEstimadoMin,@FechaInicio,@FechaFin,@Tipo,@CreadoPor);",
            tarea);
        return filas > 0;
    }

    public async Task<bool> EliminarTarea(long id)
    {
        using var conn = GetConnection();
        var filas = await conn.ExecuteAsync("CALL sp_eliminar_tarea(@id);", new { id });
        return filas > 0;
    }

    // =========================
    // USUARIOS
    // =========================
    public async Task<long> RegistrarUsuario(Usuario usuario)
    {
        using var conn = GetConnection();
        return await conn.ExecuteScalarAsync<long>(
            "CALL sp_registrar_usuario(@Nombre,@Email,@Username,@PasswordHash);",
            usuario);
    }

    public async Task<Usuario?> ObtenerUsuarioPorId(long id)
    {
        using var conn = GetConnection();
        return await conn.QueryFirstOrDefaultAsync<Usuario>(
            "CALL sp_get_usuario_por_id(@id);", new { id });
    }

    public async Task<Usuario?> ObtenerUsuarioPorUsername(string username)
    {
        using var conn = GetConnection();
        return await conn.QueryFirstOrDefaultAsync<Usuario>(
            "CALL sp_get_usuario_por_username(@username);", new { username });
    }

    // =========================
    // FILTROS
    // =========================
    public async Task<IEnumerable<Tarea>> ObtenerTareasPorEstado(string columna)
    {
        using var conn = GetConnection();
        return await conn.QueryAsync<Tarea>(
            "CALL sp_get_tareas_por_estado(@columna);", new { columna });
    }

    public async Task<IEnumerable<Tarea>> ObtenerTareasPorPrioridad(string prioridad)
    {
        using var conn = GetConnection();
        return await conn.QueryAsync<Tarea>(
            "CALL sp_get_tareas_por_prioridad(@prioridad);", new { prioridad });
    }

    // =========================
    // NUEVOS: Tareas de usuario
    // =========================
    public async Task<List<Tarea>> ObtenerTareasDeUsuario(long usuarioId)
    {
        using var conn = GetConnection();
        var tareas = await conn.QueryAsync<Tarea>(
            "CALL sp_get_tareas_de_usuario(@usuarioId);", new { usuarioId });
        return tareas.AsList();
    }

    // =========================
    // TEST CONEXIÓN
    // =========================
    public async Task<MySqlConnection> ProbarConexion()
    {
        var conn = GetConnection();
        await conn.OpenAsync();
        return conn;
    }
}
