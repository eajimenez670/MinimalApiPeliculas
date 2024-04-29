using AutoMapper;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OutputCaching;
using MinimalApiPeliculas.DTOs;
using MinimalApiPeliculas.Entidades;
using MinimalApiPeliculas.Repositorios;
using MinimalApiPeliculas.Servicios;

namespace MinimalApiPeliculas.Endpoints
{
    public static class PeliculasEndpoints
    {
        private static readonly string _contenedor = "Peliculas";

        public static RouteGroupBuilder MapPeliculas(this RouteGroupBuilder group)
        {
            group.MapGet("/", Obtener).CacheOutput(c => c.Expire(TimeSpan.FromSeconds(60)).Tag("peliculas-get"));
            group.MapGet("/{id:int}", ObtenerPorId);
            group.MapPost("/", Crear).DisableAntiforgery();
            group.MapPost("obtenerPorNombre/{nombre}", ObtenerPorNombre);
            group.MapPut("/{id:int}", Actualizar).DisableAntiforgery();
            group.MapDelete("/{id:int}", Borrar);

            return group;
        }

        static async Task<Created<PeliculaDTO>> Crear([FromForm] CrearPeliculaDTO crearPeliculaDTO, IRepositorioPeliculas repositorio,
           IOutputCacheStore outputCacheStore, IMapper mapper, IAlmacenadorArchivos almacenadorArchivos)
        {
            var pelicula = mapper.Map<Pelicula>(crearPeliculaDTO);

            if (crearPeliculaDTO.Poster is not null)
            {
                var url = await almacenadorArchivos.Almacenar(_contenedor, crearPeliculaDTO.Poster);
                pelicula.Poster = url;
            }

            var id = await repositorio.Crear(pelicula);
            await outputCacheStore.EvictByTagAsync("peliculas-get", default);
            return TypedResults.Created($"/peliculas/{id}", mapper.Map<PeliculaDTO>(pelicula));
        }

        // Métodos
        static async Task<Ok<IEnumerable<PeliculaDTO>>> Obtener(IRepositorioPeliculas repositorio, IMapper mapper,
            int pagina = 1, int recordsPorPagina = 10)
        {
            var paginacionDTO = new PaginacionDTO { Pagina = pagina, RecordsPorPagina = recordsPorPagina };
            var peliculas = await repositorio.ObtenerTodo(paginacionDTO);
            return TypedResults.Ok(mapper.Map<IEnumerable<PeliculaDTO>>(peliculas));
        }

        static async Task<Ok<IEnumerable<PeliculaDTO>>> ObtenerPorNombre(string nombre, IRepositorioPeliculas repositorio, IMapper mapper)
        {
            var peliculas = await repositorio.ObtenerPorNombre(nombre);
            return TypedResults.Ok(mapper.Map<IEnumerable<PeliculaDTO>>(peliculas));
        }

        static async Task<Results<Ok<PeliculaDTO>, NotFound>> ObtenerPorId(int id, IRepositorioPeliculas repositorio, IMapper mapper)
        {
            var pelicula = await repositorio.ObtenerPorId(id);
            if (pelicula is null)
            {
                return TypedResults.NotFound();
            }

            return TypedResults.Ok(mapper.Map<PeliculaDTO>(pelicula));
        }

        static async Task<Results<NotFound, NoContent>> Actualizar(int id, [FromForm] CrearPeliculaDTO crearPeliculaDTO,
            IRepositorioPeliculas repositorio, IAlmacenadorArchivos almacenadorArchivos, IOutputCacheStore outputCacheStore,
            IMapper mapper)
        {
            var peliculaDB = await repositorio.ObtenerPorId(id);
            if (peliculaDB is null)
            {
                return TypedResults.NotFound();
            }

            var peliculaParaActualizar = mapper.Map<Pelicula>(crearPeliculaDTO);
            peliculaParaActualizar.Id = id;
            peliculaParaActualizar.Poster = peliculaDB.Poster;

            if (crearPeliculaDTO.Poster is not null)
            {
                var url = await almacenadorArchivos.Editar(peliculaParaActualizar.Poster, _contenedor, crearPeliculaDTO.Poster);
                peliculaParaActualizar.Poster = url;
            }

            await repositorio.Actualizar(peliculaParaActualizar);
            await outputCacheStore.EvictByTagAsync("peliculas-get", default);
            return TypedResults.NoContent();
        }

        static async Task<Results<NotFound, NoContent>> Borrar(int id, IRepositorioPeliculas repositorio, IOutputCacheStore outputCacheStore,
            IAlmacenadorArchivos almacenadorArchivos)
        {
            var peliculaDB = await repositorio.ObtenerPorId(id);
            if (peliculaDB is null)
            {
                return TypedResults.NotFound();
            }

            await repositorio.Borrar(id);
            await almacenadorArchivos.Borrar(peliculaDB.Poster, _contenedor);
            await outputCacheStore.EvictByTagAsync("peliculas-get", default);
            return TypedResults.NoContent();
        }
    }
}
