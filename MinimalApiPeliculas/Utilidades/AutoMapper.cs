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

            CreateMap<Actor, ActorDTO>();
            CreateMap<CrearActorDTO, Actor>().ForMember(x => x.Foto, p => p.Ignore());

            CreateMap<Pelicula, PeliculaDTO>();
            CreateMap<CrearPeliculaDTO, Pelicula>().ForMember(x => x.Poster, p => p.Ignore());

            CreateMap<Comentario, ComentarioDTO>();
            CreateMap<CrearComentarioDTO, Comentario>();
        }
    }
}
