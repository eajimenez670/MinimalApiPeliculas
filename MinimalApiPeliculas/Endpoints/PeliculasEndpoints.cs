﻿using AutoMapper;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OutputCaching;
using MinimalApiPeliculas.DTOs;
using MinimalApiPeliculas.Entidades;
using MinimalApiPeliculas.Filtros;
using MinimalApiPeliculas.Repositorios;
using MinimalApiPeliculas.Servicios;
using MinimalApiPeliculas.Utilidades;

namespace MinimalApiPeliculas.Endpoints
{
    public static class PeliculasEndpoints
    {
        private static readonly string _contenedor = "Peliculas";

        public static RouteGroupBuilder MapPeliculas(this RouteGroupBuilder group)
        {
            group.MapGet("/", Obtener)
                .CacheOutput(c => c.Expire(TimeSpan.FromSeconds(60)).Tag("peliculas-get"))
                .AgregarParamterosPaginacionOpenApi();

            group.MapGet("/{id:int}", ObtenerPorId);
            group.MapPost("obtenerPorNombre/{nombre}", ObtenerPorNombre);

            group.MapPost("/", Crear)
                .DisableAntiforgery()
                .AddEndpointFilter<FiltroValidaciones<CrearPeliculaDTO>>()
                .RequireAuthorization("esadmin")
                .WithOpenApi();

            group.MapPut("/{id:int}", Actualizar)
                .DisableAntiforgery()
                .AddEndpointFilter<FiltroValidaciones<CrearPeliculaDTO>>()
                .WithOpenApi();

            group.MapDelete("/{id:int}", Borrar).RequireAuthorization("esadmin");
            group.MapPost("/{id:int}/asignarGeneros", AsignarGeneros).RequireAuthorization("esadmin");
            group.MapPost("/{id:int}/asignarActores", AsignarActores).RequireAuthorization("esadmin");

            group.MapGet("/filtrar", FiltrarPeliculas).AgregarParametrosPeliculasFiltroOpenApi();

            return group;
        }

        static async Task<Created<PeliculaDTO>> Crear([FromForm] CrearPeliculaDTO crearPeliculaDTO,
            IRepositorioPeliculas repositorio, IOutputCacheStore outputCacheStore, IMapper mapper,
            IAlmacenadorArchivos almacenadorArchivos)
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
            PaginacionDTO paginacionDTO)
        {
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

        static async Task<Results<NoContent, NotFound, BadRequest<string>>> AsignarGeneros(int id, List<int> generosIds,
            IRepositorioPeliculas repositorioPeliculas, IRepositorioGeneros repositorioGeneros)
        {
            if (!await repositorioPeliculas.Existe(id))
                return TypedResults.NotFound();

            var generosExistentes = new List<int>();

            if (generosIds.Count != 0)
                generosExistentes = await repositorioGeneros.Existen(generosIds);

            if (generosExistentes.Count != generosIds.Count)
            {
                var generosNoExistentes = generosIds.Except(generosExistentes);
                return TypedResults.BadRequest($"Los géneros de id {string.Join(",", generosNoExistentes)} no existen.");
            }

            await repositorioPeliculas.AsignarGeneros(id, generosIds);
            return TypedResults.NoContent();
        }

        static async Task<Results<NoContent, NotFound, BadRequest<string>>> AsignarActores(int id,
            List<AsignarActorPeliculaDTO> actoresDTO, IRepositorioPeliculas repositorioPeliculas,
            IRepositorioActores repositorioActores, IMapper mapper)
        {
            if (!await repositorioPeliculas.Existe(id))
                return TypedResults.NotFound();

            var actoresExistentes = new List<int>();
            var actoresIds = actoresDTO.Select(a => a.ActorId).ToList();

            if (actoresDTO.Count != 0)
                actoresExistentes = await repositorioActores.Existen(actoresIds);

            if (actoresExistentes.Count != actoresDTO.Count)
            {
                var actoresNoExistentes = actoresIds.Except(actoresExistentes);
                return TypedResults.BadRequest($"Los actores con id {string.Join(",", actoresNoExistentes)} no existen.");
            }

            var actores = mapper.Map<List<ActorPelicula>>(actoresDTO);
            await repositorioPeliculas.AsignarActores(id, actores);
            return TypedResults.NoContent();
        }

        static async Task<Ok<List<PeliculaDTO>>> FiltrarPeliculas(PeliculasFiltrarDTO peliculasFiltrarDTO,
            IRepositorioPeliculas repositorioPeliculas, IMapper mapper)
        {
            var peliculas = await repositorioPeliculas.Filtrar(peliculasFiltrarDTO);
            var peliculasDTO = mapper.Map<List<PeliculaDTO>>(peliculas);
            return TypedResults.Ok(peliculasDTO);
        }
    }
}
