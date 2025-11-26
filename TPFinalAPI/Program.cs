using Microsoft.OpenApi.Models; // solo para Swagger
using Persistencia;              // IDao y DaoDappers
using Models;                    // tus modelos

var builder = WebApplication.CreateBuilder(args);

// ====== Servicios ====== //
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "TPFinal API", Version = "v1" });
});

// Inyección de dependencias
builder.Services.AddSingleton<IDao>(sp =>
    new DaoDappers(builder.Configuration.GetConnectionString("DefaultConnection"))
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

// ====== Endpoints ====== //
// TAREAS
app.MapGet("/tareas", async (IDao dao) => await dao.ObtenerTareas());
app.MapGet("/tareas/{id:long}", async (long id, IDao dao) =>
{
    var tarea = await dao.ObtenerTareaPorId(id);
    return tarea is not null ? Results.Ok(tarea) : Results.NotFound();
});
app.MapPost("/tareas", async (Tarea tarea, IDao dao) =>
{
    var id = await dao.CrearTarea(tarea);
    return Results.Created($"/tareas/{id}", tarea);
});
app.MapPut("/tareas/{id:long}", async (long id, Tarea tarea, IDao dao) =>
{
    tarea.Id = id;
    return await dao.ActualizarTarea(tarea) ? Results.Ok(tarea) : Results.NotFound();
});
app.MapDelete("/tareas/{id:long}", async (long id, IDao dao) =>
    await dao.EliminarTarea(id) ? Results.Ok() : Results.NotFound()
);

// USUARIOS
app.MapPost("/usuarios", async (Usuario usuario, IDao dao) =>
{
    var id = await dao.RegistrarUsuario(usuario);
    return Results.Created($"/usuarios/{id}", usuario);
});

// TEST DE CONEXIÓN
app.MapGet("/testconexion", async (IDao dao) =>
{
    var conn = await ((DaoDappers)dao).ProbarConexion();
    return Results.Ok($"Conexión abierta: {conn.State}");
});

app.Run();
