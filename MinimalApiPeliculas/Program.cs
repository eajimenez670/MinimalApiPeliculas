using FluentValidation;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using MinimalApiPeliculas;
using MinimalApiPeliculas.Endpoints;
using MinimalApiPeliculas.Entidades;
using MinimalApiPeliculas.Repositorios;
using MinimalApiPeliculas.Servicios;

var builder = WebApplication.CreateBuilder(args);
var origenes = builder.Configuration.GetValue<string>("origenesPermitidos")!;
// Área de los servicio

builder.Services.AddDbContext<ApplicationDbContext>(opt =>
{
    opt.UseSqlServer("name=Default");
});

builder.Services.AddIdentityCore<IdentityUser>()
    .AddEntityFrameworkStores<ApplicationDbContext>()
    .AddDefaultTokenProviders();

builder.Services.AddScoped<UserManager<IdentityUser>>();
builder.Services.AddScoped<SignInManager<IdentityUser>>();

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
builder.Services.AddScoped<IRepositorioActores, RepositorioActores>();
builder.Services.AddScoped<IRepositorioPeliculas, RepositorioPeliculas>();
builder.Services.AddScoped<IRepositorioComentarios, RepositorioComentarios>();
builder.Services.AddScoped<IRepositorioErrores, RepositorioErrores>();

builder.Services.AddScoped<IAlmacenadorArchivos, AlmacenadorArchivosLocal>();
builder.Services.AddHttpContextAccessor();

builder.Services.AddAutoMapper(typeof(Program));

builder.Services.AddValidatorsFromAssemblyContaining<Program>();

builder.Services.AddProblemDetails();

builder.Services.AddAuthentication().AddJwtBearer();
builder.Services.AddAuthorization();

// Fin área de servicio
var app = builder.Build();
// Área Middleware
if (builder.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseExceptionHandler(exceptionHandlerApp => exceptionHandlerApp.Run(async context =>
{
    var exceptionHandlerFeature = context.Features.Get<IExceptionHandlerFeature>();
    var exception = exceptionHandlerFeature?.Error!;

    var error = new Error
    {
        Fecha = DateTime.Now,
        MensajeDeError = exception.Message,
        StackTrace = exception?.StackTrace!
    };

    var repositorio = context.RequestServices.GetRequiredService<IRepositorioErrores>();
    await repositorio.Crear(error);

    await TypedResults.BadRequest(
        new { tipo = "error", mensaje = "ha ocurrido un error inesperado", estatus = 500 }
        ).ExecuteAsync(context);
}));
app.UseStatusCodePages();

app.UseStaticFiles();

app.UseCors();
app.UseOutputCache();

app.UseAuthorization();

// Endpoint de prueba
app.MapGet("/", [EnableCors(policyName: "libre")] () => "Hola Mundo");
app.MapGet("/error", () =>
{
    throw new InvalidOperationException("Error de ejemplo");
});

// Endpoints
app.MapGroup("/generos").MapGeneros();
app.MapGroup("/actores").MapActores();
app.MapGroup("/peliculas").MapPeliculas();
app.MapGroup("/pelicula/{peliculaId:int}/comentarios").MapComentarios();

// Fin de área Middleware
app.Run();