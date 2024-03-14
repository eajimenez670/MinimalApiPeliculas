using Microsoft.AspNetCore.Cors;
using Microsoft.EntityFrameworkCore;
using MinimalApiPeliculas;
using MinimalApiPeliculas.Entidades;

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

app.MapGet("/generos", () =>
{
    var generos = new List<Genero>
    {
        new Genero { Id = 1, Nombre = "Drama"},
        new Genero { Id = 2, Nombre = "Acción"},
        new Genero { Id = 3, Nombre = "Comedia"}
    };

    return generos;
}).CacheOutput(c => c.Expire(TimeSpan.FromSeconds(15)));

// Fin de área Middleware
app.Run();
