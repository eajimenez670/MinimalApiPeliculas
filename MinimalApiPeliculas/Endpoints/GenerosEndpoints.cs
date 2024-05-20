using AutoMapper;
using FluentValidation;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.OutputCaching;
using MinimalApiPeliculas.DTOs;
using MinimalApiPeliculas.Entidades;
using MinimalApiPeliculas.Migrations;
using MinimalApiPeliculas.Repositorios;

namespace MinimalApiPeliculas.Endpoints
{
    public static class GenerosEndpoints
    {
        public static RouteGroupBuilder MapGeneros(this RouteGroupBuilder group)
        {
            group.MapGet("/", ObtenerGeneros).CacheOutput(c => c.Expire(TimeSpan.FromSeconds(60)).Tag("generos-get"));
            group.MapGet("/{id:int}", ObtenerGeneroPorId);
            group.MapPost("/", CrearGenero);
            group.MapPut("/{id:int}", ActualizarGenero);
            group.MapDelete("/{id:int}", BorrarGenero);

            return group;
        }

        // Métodos
        static async Task<Ok<IEnumerable<GeneroDTO>>> ObtenerGeneros(IRepositorioGeneros repositorio, IMapper mapper)
        {
            var generos = await repositorio.ObtenerTodo();
            return TypedResults.Ok(mapper.Map<IEnumerable<GeneroDTO>>(generos));
        }

        static async Task<Results<Ok<GeneroDTO>, NotFound>> ObtenerGeneroPorId(int id, IRepositorioGeneros repositorio, IMapper mapper)
        {
            var genero = await repositorio.ObtenerPorId(id);
            if (genero is null)
            {
                return TypedResults.NotFound();
            }

            return TypedResults.Ok(mapper.Map<GeneroDTO>(genero));
        }

        static async Task<Results<Created<GeneroDTO>, ValidationProblem>> CrearGenero(
            CrearGeneroDTO crearGenero, IRepositorioGeneros repositorio,
            IOutputCacheStore outputCacheStore, IMapper mapper, IValidator<CrearGeneroDTO> validador)
        {
            var resultadoValidacion = await validador.ValidateAsync(crearGenero);
            if (!resultadoValidacion.IsValid)
                return TypedResults.ValidationProblem(resultadoValidacion.ToDictionary());

            var genero = mapper.Map<Genero>(crearGenero);
            var id = await repositorio.Crear(genero);
            await outputCacheStore.EvictByTagAsync("generos-get", default);
            return TypedResults.Created($"/generos/{id}", mapper.Map<GeneroDTO>(genero));
        }

        static async Task<Results<NotFound, NoContent>> ActualizarGenero(int id, CrearGeneroDTO crearGeneroDTO, IRepositorioGeneros repositorio, IOutputCacheStore outputCacheStore, IMapper mapper)
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
