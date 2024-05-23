using AutoMapper;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OutputCaching;
using MinimalApiPeliculas.DTOs;
using MinimalApiPeliculas.Entidades;
using MinimalApiPeliculas.Filtros;
using MinimalApiPeliculas.Repositorios;
using MinimalApiPeliculas.Servicios;

namespace MinimalApiPeliculas.Endpoints
{
    public static class ActoresEndpoints
    {
        private static readonly string _contenedor = "Actores";

        public static RouteGroupBuilder MapActores(this RouteGroupBuilder group)
        {
            group.MapGet("/", ObtenerActores).CacheOutput(c => c.Expire(TimeSpan.FromSeconds(60)).Tag("actores-get"));
            group.MapGet("/{id:int}", ObtenerActorPorId);
            group.MapPost("obtenerPorNombre/{nombre}", ObtenerPorNombre);
            group.MapPost("/", CrearActor).DisableAntiforgery().AddEndpointFilter<FiltroValidaciones<CrearActorDTO>>();
            group.MapPut("/{id:int}", ActualizarActor).DisableAntiforgery().AddEndpointFilter<FiltroValidaciones<CrearActorDTO>>();
            group.MapDelete("/{id:int}", BorrarActor);

            return group;
        }

        // Métodos
        static async Task<Ok<IEnumerable<ActorDTO>>> ObtenerActores(IRepositorioActores repositorio, IMapper mapper,
            int pagina = 1, int recordsPorPagina = 10)
        {
            var paginacionDTO = new PaginacionDTO { Pagina = pagina, RecordsPorPagina = recordsPorPagina };
            var actores = await repositorio.ObtenerTodo(paginacionDTO);
            return TypedResults.Ok(mapper.Map<IEnumerable<ActorDTO>>(actores));
        }

        static async Task<Ok<IEnumerable<ActorDTO>>> ObtenerPorNombre(string nombre, IRepositorioActores repositorio, IMapper mapper)
        {
            var actores = await repositorio.ObtenerPorNombre(nombre);
            return TypedResults.Ok(mapper.Map<IEnumerable<ActorDTO>>(actores));
        }

        static async Task<Results<Ok<ActorDTO>, NotFound>> ObtenerActorPorId(int id, IRepositorioActores repositorio, IMapper mapper)
        {
            var actor = await repositorio.ObtenerPorId(id);
            if (actor is null)
            {
                return TypedResults.NotFound();
            }

            return TypedResults.Ok(mapper.Map<ActorDTO>(actor));
        }

        static async Task<Results<Created<ActorDTO>, ValidationProblem>> CrearActor([FromForm] CrearActorDTO crearActorDTO,
            IRepositorioActores repositorio, IOutputCacheStore outputCacheStore, IMapper mapper,
            IAlmacenadorArchivos almacenadorArchivos)
        {
            var actor = mapper.Map<Actor>(crearActorDTO);

            if (crearActorDTO.Foto is not null)
            {
                var url = await almacenadorArchivos.Almacenar(_contenedor, crearActorDTO.Foto);
                actor.Foto = url;
            }

            var id = await repositorio.Crear(actor);
            await outputCacheStore.EvictByTagAsync("actores-get", default);
            return TypedResults.Created($"/actores/{id}", mapper.Map<ActorDTO>(actor));
        }

        static async Task<Results<NotFound, NoContent>> ActualizarActor(int id, [FromForm] CrearActorDTO crearActorDTO,
            IRepositorioActores repositorio, IAlmacenadorArchivos almacenadorArchivos, IOutputCacheStore outputCacheStore,
            IMapper mapper)
        {
            var actorDB = await repositorio.ObtenerPorId(id);
            if (actorDB is null)
            {
                return TypedResults.NotFound();
            }

            var actorParaActualizar = mapper.Map<Actor>(crearActorDTO);
            actorParaActualizar.Id = id;
            actorParaActualizar.Foto = actorDB.Foto;

            if (crearActorDTO.Foto is not null)
            {
                var url = await almacenadorArchivos.Editar(actorParaActualizar.Foto, _contenedor, crearActorDTO.Foto);
                actorParaActualizar.Foto = url;
            }

            await repositorio.Actualizar(actorParaActualizar);
            await outputCacheStore.EvictByTagAsync("actores-get", default);
            return TypedResults.NoContent();
        }

        static async Task<Results<NotFound, NoContent>> BorrarActor(int id, IRepositorioActores repositorio, IOutputCacheStore outputCacheStore,
            IAlmacenadorArchivos almacenadorArchivos)
        {
            var actorDB = await repositorio.ObtenerPorId(id);
            if (actorDB is null)
            {
                return TypedResults.NotFound();
            }

            await repositorio.Borrar(id);
            await almacenadorArchivos.Borrar(actorDB.Foto, _contenedor);
            await outputCacheStore.EvictByTagAsync("actores-get", default);
            return TypedResults.NoContent();
        }

    }
}
