using Persistencia;
using TPFinalAPI.Models;

var builder = WebApplication.CreateBuilder(args);

// ====== Servicios ====== //
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();

// Swagger (la línea mágica anti-CS0234)
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new()
    {
        Title = "TPFinal API",
        Version = "v1"
    });
});

// Inyección de dependencias (el ! evita warning de null)
builder.Services.AddSingleton<IDao>(sp =>
    new DaoDappers(builder.Configuration.GetConnectionString("DefaultConnection")!)
);

var app = builder.Build();

// ====== Middleware ====== //
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "TPFinal API v1"));
}

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

// ================== ENDPOINTS - TAREAS ==================
app.MapGet("/tareas", async (IDao dao) => await dao.ObtenerTareas());

app.MapGet("/tareas/{id:long}", async (long id, IDao dao) =>
{
    var tarea = await dao.ObtenerTareaPorId(id);
    return tarea is not null ? Results.Ok(tarea) : Results.NotFound();
});

app.MapPost("/tareas", async (TareaCreateDto dto, IDao dao) =>
{
    var tarea = new Tarea  // Mapeo DTO a modelo (adaptá si hace falta)
    {
        Titulo = dto.Titulo,
        Descripcion = dto.Descripcion,
        Tipo = dto.Tipo,
        ColumnaNombre = dto.ColumnaNombre,
        TableroNombre = dto.TableroNombre,
       CreadoPor = dto.UsuarioId  // Asumiendo que Tarea tiene UsuarioId
    };
    var id = await dao.CrearTarea(tarea);
    return Results.Created($"/tareas/{id}", tarea);
});

app.MapPut("/tareas/{id:long}", async (long id, TareaUpdateDto dto, IDao dao) =>
{
    var tarea = await dao.ObtenerTareaPorId(id);
    if (tarea == null) return Results.NotFound();

    // Actualizar solo lo que viene en el DTO
    if (dto.Titulo != null) tarea.Titulo = dto.Titulo;
    if (dto.Descripcion != null) tarea.Descripcion = dto.Descripcion;
    if (dto.Tipo != null) tarea.Tipo = dto.Tipo;
    if (dto.ColumnaNombre != null) tarea.ColumnaNombre = dto.ColumnaNombre;
    if (dto.TableroNombre != null) tarea.TableroNombre = dto.TableroNombre;

    return await dao.ActualizarTarea(tarea) ? Results.Ok(tarea) : Results.NotFound();
});

app.MapDelete("/tareas/{id:long}", async (long id, IDao dao) =>
    await dao.EliminarTarea(id) ? Results.Ok() : Results.NotFound());

// NUEVO: Tareas de un usuario específico (para tablero personalizado)
app.MapGet("/usuarios/{idUsuario}/tareas", async (long idUsuario, IDao dao) =>
    await dao.ObtenerTareasDeUsuario(idUsuario));

// ================== ENDPOINTS - USUARIOS ==================
app.MapPost("/usuarios", async (Usuario usuario, IDao dao) =>
{
    var id = await dao.RegistrarUsuario(usuario);
    return Results.Created($"/usuarios/{id}", usuario);
});

// NUEVO: Obtener usuario por ID (útil para perfil en front)
app.MapGet("/usuarios/{id:long}", async (long id, IDao dao) =>
{
    var usuario = await dao.ObtenerUsuarioPorId(id);
    return usuario is not null ? Results.Ok(usuario) : Results.NotFound();
});

// ================== LOGIN ==================
app.MapPost("/login", async (UsuarioLoginDto login, IDao dao) =>
{
    var usuario = await dao.ObtenerUsuarioPorUsername(login.Username);
    if (usuario == null || usuario.PasswordHash != login.Password)  // En prod: usa hashing real
        return Results.Unauthorized();

    return Results.Ok(new { Id = usuario.Id, Nombre = usuario.Nombre, Username = usuario.Username });
});

// ================== TEST ==================
app.MapGet("/testconexion", async (IDao dao) =>
{
    var conn = await ((DaoDappers)dao).ProbarConexion();
    return Results.Ok($"Conexión abierta: {conn.State}");
});

app.Run();