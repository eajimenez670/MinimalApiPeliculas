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
        }
    }
}
