using Persistencia;
using TPFinalAPI.Models;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();

// ← ESTA ES LA LÍNEA MÁGICA QUE MATA EL ERROR PARA SIEMPRE
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new()
    {
        Title = "TPFinal API",
        Version = "v1"
    });
});

builder.Services.AddSingleton<IDao>(sp =>
    new DaoDappers(builder.Configuration.GetConnectionString("DefaultConnection")!)
);

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "TPFinal API v1"));
}

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

// tus endpoints...
app.MapGet("/tareas", async (IDao dao) => await dao.ObtenerTareas());
// ... el resto igual

app.Run();