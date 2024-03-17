using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.OutputCaching;
using Microsoft.EntityFrameworkCore;
using MinimalApiPeliculas;
using MinimalApiPeliculas.Entidades;
using MinimalApiPeliculas.Migrations;
using MinimalApiPeliculas.Repositorios;

var builder = WebApplication.CreateBuilder(args);
var origenes = builder.Configuration.GetValue<string>("origenesPermitidos")!;
// Área de los servicio

builder.Services.AddDbContext<ApplicationDbContext>(opt =>
{
    opt.UseSqlServer("name=Default");
});

builder.Services.AddCors(opt =>
{
    opt.AddDefaultPolicy(conf =>
    {
        conf.WithOrigins(origenes).AllowAnyMethod().AllowAnyHeader();
    });

    opt.AddPolicy("libre", conf =>
    {
        conf.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader();
    });
});

builder.Services.AddOutputCache();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddScoped<IRepositorioGeneros, RepositorioGeneros>();

// Fin área de servicio
var app = builder.Build();
// Área Middleware
if (builder.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCors();

app.UseOutputCache();

app.MapGet("/", [EnableCors(policyName: "libre")] () => "Hola Mundo");

app.MapGet("/generos", async (IRepositorioGeneros repositorio) =>
{
    return await repositorio.ObtenerTodo();
}).CacheOutput(c => c.Expire(TimeSpan.FromSeconds(60)).Tag("generos-get"));

app.MapGet("/generos/{id:int}", async (int id, IRepositorioGeneros repositorio) =>
{
    var genero = await repositorio.ObtenerPorId(id);
    if (genero is null)
    {
        return Results.NotFound();
    }

    return Results.Ok(genero);
});

app.MapPost("/generos", async (Genero genero, IRepositorioGeneros repositorio, IOutputCacheStore outputCacheStore) =>
{
    var id = await repositorio.Crear(genero);
    await outputCacheStore.EvictByTagAsync("generos-get", default);
    return Results.Created($"/generos/{id}", genero);
});

app.MapPut("/generos/{id:int}", async (int id, Genero genero, IRepositorioGeneros repositorio, IOutputCacheStore outputCacheStore) =>
{
    var existe = await repositorio.Existe(id);
    if (!existe)
    {
        return Results.NotFound();
    }

    await repositorio.Actualizar(genero);
    await outputCacheStore.EvictByTagAsync("generos-get", default);
    return Results.NoContent();
});

app.MapDelete("/generos/{id:int}", async (int id, IRepositorioGeneros repositorio, IOutputCacheStore outputCacheStore) =>
{
    var existe = await repositorio.Existe(id);
    if (!existe)
    {
        return Results.NotFound();
    }

    await repositorio.Borrar(id);
    await outputCacheStore.EvictByTagAsync("generos-get", default);
    return Results.NoContent();
});

// Fin de área Middleware
app.Run();
