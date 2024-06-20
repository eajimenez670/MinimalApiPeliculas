using FluentValidation;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using MinimalApiPeliculas;
using MinimalApiPeliculas.Endpoints;
using MinimalApiPeliculas.Entidades;
using MinimalApiPeliculas.Repositorios;
using MinimalApiPeliculas.Servicios;
using MinimalApiPeliculas.Swagger;
using MinimalApiPeliculas.Utilidades;

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
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "Películas API",
        Description = "Este es un web api para trabajar con datos de películas",
        Contact = new OpenApiContact
        {
            Email = "edisonjimenez@gmail.com",
            Name = "Edison Jimenez"
        },
        License = new OpenApiLicense
        {
            Name = "MIT",
            Url = new Uri("https://opensource.org/license/mit/")
        },
    });

    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header
    });

    c.OperationFilter<FiltroAutorizacion>();

    //c.AddSecurityRequirement(new OpenApiSecurityRequirement
    //{
    //    {
    //        new OpenApiSecurityScheme
    //        {
    //            Reference = new OpenApiReference
    //            {
    //                Type = ReferenceType.SecurityScheme,
    //                Id = "Bearer"
    //            }
    //        }, new string[]{ }
    //    }
    //});
});

builder.Services.AddScoped<IRepositorioGeneros, RepositorioGeneros>();
builder.Services.AddScoped<IRepositorioActores, RepositorioActores>();
builder.Services.AddScoped<IRepositorioPeliculas, RepositorioPeliculas>();
builder.Services.AddScoped<IRepositorioComentarios, RepositorioComentarios>();
builder.Services.AddScoped<IRepositorioErrores, RepositorioErrores>();

builder.Services.AddScoped<IAlmacenadorArchivos, AlmacenadorArchivosLocal>();
builder.Services.AddTransient<IServicioUsuarios, ServicioUsuarios>();

builder.Services.AddHttpContextAccessor();

builder.Services.AddAutoMapper(typeof(Program));

builder.Services.AddValidatorsFromAssemblyContaining<Program>();

builder.Services.AddProblemDetails();

builder.Services.AddAuthentication().AddJwtBearer(opciones =>
{
    opciones.MapInboundClaims = false;

    opciones.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = false,
        ValidateAudience = false,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        //IssuerSigningKey = Llaves.ObtenerLlave(builder.Configuration).First(), // Usar una única llave de autenticación 
        IssuerSigningKeys = Llaves.ObtenerTodasLasLlaves(builder.Configuration), // Usar múltiples llaves de autenticación
        ClockSkew = TimeSpan.Zero
    };
});
builder.Services.AddAuthorization(opciones =>
{
    opciones.AddPolicy("esadmin", politica => politica.RequireClaim("esadmin"));
});

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

app.MapPost("/modelbinding/{nombre}", ([FromRoute] string? nombre) =>
{
    if (nombre is null)
    {
        nombre = "Vacio";
    }

    return TypedResults.Ok(nombre);
});

// Endpoints
app.MapGroup("/generos").MapGeneros();
app.MapGroup("/actores").MapActores();
app.MapGroup("/peliculas").MapPeliculas();
app.MapGroup("/pelicula/{peliculaId:int}/comentarios").MapComentarios();
app.MapGroup("/usuarios").MapUsuarios();

// Fin de área Middleware
app.Run();