using AutoMapper;
using MinimalApiPeliculas.DTOs;
using MinimalApiPeliculas.Entidades;

namespace MinimalApiPeliculas.Utilidades
{
    public class AutoMapper : Profile
    {
        public AutoMapper()
        {
            CreateMap<Genero, GeneroDTO>();
            CreateMap<CrearGeneroDTO, Genero>();
            CreateMap<ActualizarGeneroDTO, Genero>();

            CreateMap<Actor, ActorDTO>();
            CreateMap<CrearActorDTO, Actor>().ForMember(x => x.Foto, p => p.Ignore());

            CreateMap<Pelicula, PeliculaDTO>()
                .ForMember(p => p.Generos, ent => ent.MapFrom(p => p.GenerosPeliculas.Select(gp =>
                    new GeneroDTO
                    {
                        Id = gp.GeneroId,
                        Nombre = gp.Genero.Nombre
                    })))
                .ForMember(p => p.Actores, ent => ent.MapFrom(p =>
                p.ActoresPeliculas.Select(ap => new ActorPeliculaDTO
                {
                    Id = ap.ActorId,
                    Nombre = ap.Actor.Nombre,
                    Personaje = ap.Personaje
                })));
            CreateMap<CrearPeliculaDTO, Pelicula>().ForMember(x => x.Poster, p => p.Ignore());

            CreateMap<Comentario, ComentarioDTO>();
            CreateMap<CrearComentarioDTO, Comentario>();

            CreateMap<AsignarActorPeliculaDTO, ActorPelicula>();

        }
    }
}
