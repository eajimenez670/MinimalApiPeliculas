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
        }
    }
}
