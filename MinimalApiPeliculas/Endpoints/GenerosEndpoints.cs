using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.OutputCaching;
using MinimalApiPeliculas.Entidades;
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
        static async Task<Ok<IEnumerable<Genero>>> ObtenerGeneros(IRepositorioGeneros repositorio)
        {
            var generos = await repositorio.ObtenerTodo();
            return TypedResults.Ok(generos);
        }

        static async Task<Results<Ok<Genero>, NotFound>> ObtenerGeneroPorId(int id, IRepositorioGeneros repositorio)
        {
            var genero = await repositorio.ObtenerPorId(id);
            if (genero is null)
            {
                return TypedResults.NotFound();
            }

            return TypedResults.Ok(genero);
        }

        static async Task<Created<Genero>> CrearGenero(Genero genero, IRepositorioGeneros repositorio, IOutputCacheStore outputCacheStore)
        {
            var id = await repositorio.Crear(genero);
            await outputCacheStore.EvictByTagAsync("generos-get", default);
            return TypedResults.Created($"/generos/{id}", genero);
        }

        static async Task<Results<NotFound, NoContent>> ActualizarGenero(int id, Genero genero, IRepositorioGeneros repositorio, IOutputCacheStore outputCacheStore)
        {
            var existe = await repositorio.Existe(id);
            if (!existe)
            {
                return TypedResults.NotFound();
            }

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
