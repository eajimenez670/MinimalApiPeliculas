using AutoMapper;
using MinimalApiPeliculas.Repositorios;

namespace MinimalApiPeliculas.DTOs
{
    public class ObtenerGeneroPorIdPeticionDTO
    {
        public int Id { get; set; }
        public IRepositorioGeneros Repositorio { get; set; } = null!;
        public IMapper Mapper { get; set; } = null!;
    }
}
