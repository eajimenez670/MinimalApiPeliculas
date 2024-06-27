using MinimalApiPeliculas.Entidades;
using Error = MinimalApiPeliculas.Entidades.Error;

namespace MinimalApiPeliculas.Repositorios
{
    public interface IRepositorioErrores
    {
        Task Crear(Error error);
    }
}