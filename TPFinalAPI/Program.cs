using Persistencia;
using TPFinalAPI.Models;

var builder = WebApplication.CreateBuilder(args);

// Servicios
builder.Services.AddControllersWithViews(); // MVC + API
builder.Services.AddRazorPages();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new() { Title = "TPFinal API", Version = "v1" });
});

// Inyección del DAO
builder.Services.AddSingleton<IDao>(sp =>
    new DaoDappers(builder.Configuration.GetConnectionString("DefaultConnection")!)
);

var app = builder.Build();

// Middleware
if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
    app.UseSwagger();
    app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "TPFinal API v1"));
}
else
{
    app.UseExceptionHandler("/Home/Error");
}

app.UseStaticFiles();
app.UseRouting();
app.UseHttpsRedirection();
app.UseAuthorization();

// Rutas MVC
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Tableros}/{action=Index}/{id?}"
);

// API endpoints (mantengo los tuyos y añado mover tarea por API)
app.MapGet("/api/tareas", async (IDao dao) => await dao.ObtenerTareas());
app.MapGet("/api/tareas/{id:long}", async (long id, IDao dao) =>
{
    var tarea = await dao.ObtenerTareaPorId(id);
    return tarea is not null ? Results.Ok(tarea) : Results.NotFound();
});
app.MapPost("/api/tareas", async (TareaCreateDto dto, IDao dao) =>
{
    // Buscamos tablero/columna por nombre — helper simple:
    // NOTA: aquí asumimos que cliente envía TableroNombre y ColumnaNombre; el DAO no tiene métodos para buscar por nombre,
    // así que vamos a mapear antes dentro de los controladores MVC. Para API simple, ideal sería enviar los IDs.
    var tarea = new Tarea
    {
        Titulo = dto.Titulo,
        Descripcion = dto.Descripcion,
        Tipo = dto.Tipo,
        CreadoPor = dto.UsuarioId,
        CreatedAt = DateTime.UtcNow,
        UpdatedAt = DateTime.UtcNow
    };
    var id = await dao.CrearTarea(tarea);
    tarea.Id = id;
    return Results.Created($"/api/tareas/{id}", tarea);
});
app.MapPut("/api/tareas/{id:long}", async (long id, TareaUpdateDto dto, IDao dao) =>
{
    var tarea = await dao.ObtenerTareaPorId(id);
    if (tarea == null) return Results.NotFound();

    if (dto.Titulo != null) tarea.Titulo = dto.Titulo;
    if (dto.Descripcion != null) tarea.Descripcion = dto.Descripcion;
    if (dto.Tipo != null) tarea.Tipo = dto.Tipo;
    if (dto.Prioridad.HasValue) tarea.Prioridad = dto.Prioridad.Value;
    if (dto.TiempoEstimadoMin.HasValue) tarea.TiempoEstimadoMin = dto.TiempoEstimadoMin.Value;
    if (dto.ColumnaNombre != null) {
        // Aquí lo dejaremos para el controlador MVC que sí puede decidir el id
    }
    var ok = await dao.ActualizarTarea(tarea);
    return ok ? Results.Ok(tarea) : Results.NotFound();
});
app.MapDelete("/api/tareas/{id:long}", async (long id, IDao dao) =>
    await dao.EliminarTarea(id) ? Results.Ok() : Results.NotFound());

// Mover tarea (API usado por drag & drop)
app.MapPost("/api/tareas/{id:long}/mover", async (long id, long nuevaColumnaId, IDao dao) =>
{
    var t = await dao.ObtenerTareaPorId(id);
    if (t == null) return Results.NotFound();
    t.ColumnaId = nuevaColumnaId;
    var ok = await dao.ActualizarTarea(t);
    return ok ? Results.Ok() : Results.BadRequest();
});

app.Run();
