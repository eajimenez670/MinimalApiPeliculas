using Microsoft.AspNetCore.Cors;
using Microsoft.EntityFrameworkCore;
using MinimalApiPeliculas;
using MinimalApiPeliculas.Endpoints;
using MinimalApiPeliculas.Repositorios;

var builder = WebApplication.CreateBuilder(args);
var origenes = builder.Configuration.GetValue<string>("origenesPermitidos")!;
// �rea de los servicio

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

builder.Services.AddAutoMapper(typeof(Program));

// Fin �rea de servicio
var app = builder.Build();
// �rea Middleware
if (builder.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCors();
app.UseOutputCache();

// Endpoint de prueba
app.MapGet("/", [EnableCors(policyName: "libre")] () => "Hola Mundo");

// Endpoints de g�nero
app.MapGroup("/generos").MapGeneros();

// Fin de �rea Middleware
app.Run();