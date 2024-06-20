using AutoMapper;
using Microsoft.AspNetCore.OutputCaching;
using MinimalApiPeliculas.Repositorios;

namespace MinimalApiPeliculas.DTOs
{
    public class CrearGeneroPeticionDTO
    {
        public IRepositorioGeneros Repositorio { get; set; } = null!;
        public IOutputCacheStore OutputCacheStore { get; set; } = null!;
        public IMapper Mapper { get; set; } = null!;
    }
}
