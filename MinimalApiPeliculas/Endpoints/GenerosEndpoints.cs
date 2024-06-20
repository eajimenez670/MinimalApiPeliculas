﻿using AutoMapper;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.OutputCaching;
using MinimalApiPeliculas.DTOs;
using MinimalApiPeliculas.Entidades;
using MinimalApiPeliculas.Filtros;
using MinimalApiPeliculas.Repositorios;

namespace MinimalApiPeliculas.Endpoints
{
    public static class GenerosEndpoints
    {
        public static RouteGroupBuilder MapGeneros(this RouteGroupBuilder group)
        {
            group.MapGet("/", ObtenerGeneros)
                .CacheOutput(c => c.Expire(TimeSpan.FromSeconds(60))
                                   .Tag("generos-get"));

            group.MapGet("/{id:int}", ObtenerGeneroPorId);

            group.MapPost("/", CrearGenero)
                .AddEndpointFilter<FiltroValidaciones<CrearGeneroDTO>>()
                .RequireAuthorization("esadmin");

            group.MapPut("/{id:int}", ActualizarGenero)
                .AddEndpointFilter<FiltroValidaciones<CrearGeneroDTO>>()
                .RequireAuthorization("esadmin")
                .WithOpenApi(opciones =>
                {
                    opciones.Summary = "Actualizar un género";
                    opciones.Description = "Con este edndpoint podemos actualizar un género";
                    opciones.Parameters[0].Description = "El id del género a actualizar";
                    opciones.RequestBody.Description = "El género que se desea actualizar";

                    return opciones;
                });

            group.MapDelete("/{id:int}", BorrarGenero).RequireAuthorization("esadmin");

            return group;
        }

        // Métodos
        static async Task<Ok<IEnumerable<GeneroDTO>>> ObtenerGeneros(IRepositorioGeneros repositorio,
            IMapper mapper, ILoggerFactory loggerFactory)
        {
            var tipo = typeof(GenerosEndpoints);
            var logger = loggerFactory.CreateLogger(tipo.FullName!);
            // logger.LogInformation("Obteniendo el listado de géneros");

            logger.LogTrace("Este es un mensaje de Trace");
            logger.LogDebug("Este es un mensaje de Debug");
            logger.LogInformation("Este es un mensaje de Information");
            logger.LogWarning("Este es un mensaje de Warning");
            logger.LogError("Este es un mensaje de Error");
            logger.LogCritical("Este es un mensaje de Critical");

            var generos = await repositorio.ObtenerTodo();
            return TypedResults.Ok(mapper.Map<IEnumerable<GeneroDTO>>(generos));
        }

        static async Task<Results<Ok<GeneroDTO>, NotFound>> ObtenerGeneroPorId(
            [AsParameters] ObtenerGeneroPorIdPeticionDTO modelo)
        {
            var genero = await modelo.Repositorio.ObtenerPorId(modelo.Id);
            if (genero is null)
            {
                return TypedResults.NotFound();
            }

            return TypedResults.Ok(modelo.Mapper.Map<GeneroDTO>(genero));
        }

        static async Task<Results<Created<GeneroDTO>, ValidationProblem>> CrearGenero(
            CrearGeneroDTO crearGeneroDTO, [AsParameters] CrearGeneroPeticionDTO modelo)
        {
            var genero = modelo.Mapper.Map<Genero>(crearGeneroDTO);
            var id = await modelo.Repositorio.Crear(genero);
            await modelo.OutputCacheStore.EvictByTagAsync("generos-get", default);
            return TypedResults.Created($"/generos/{id}", modelo.Mapper.Map<GeneroDTO>(genero));
        }

        static async Task<Results<NotFound, NoContent, ValidationProblem>> ActualizarGenero(int id,
            CrearGeneroDTO crearGeneroDTO, IRepositorioGeneros repositorio, IOutputCacheStore outputCacheStore,
            IMapper mapper)
        {
            var existe = await repositorio.Existe(id);
            if (!existe)
            {
                return TypedResults.NotFound();
            }

            var genero = mapper.Map<Genero>(crearGeneroDTO);
            genero.Id = id;

            await repositorio.Actualizar(genero);
            await outputCacheStore.EvictByTagAsync("generos-get", default);
            return TypedResults.NoContent();
        }

        static async Task<Results<NotFound, NoContent>> BorrarGenero(int id, IRepositorioGeneros repositorio, IOutputCacheStore outputCacheStore)
        {
            var existe = await repositorio.Existe(id);
            if (!existe)
            {
                return TypedResults.NotFound();
            }

            await repositorio.Borrar(id);
            await outputCacheStore.EvictByTagAsync("generos-get", default);
            return TypedResults.NoContent();
        }
    }
}
