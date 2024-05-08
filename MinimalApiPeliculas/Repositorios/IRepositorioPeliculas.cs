using MinimalApiPeliculas.DTOs;
using MinimalApiPeliculas.Entidades;

namespace MinimalApiPeliculas.Repositorios
{
    public interface IRepositorioPeliculas
    {
        Task Actualizar(Pelicula pelicula);
        Task AsignarActores(int id, List<ActorPelicula> actores);
        Task AsignarGeneros(int id, List<int> generosIds);
        Task Borrar(int id);
        Task<int> Crear(Pelicula pelicula);
        Task<bool> Existe(int id);
        Task<Pelicula?> ObtenerPorId(int id);
        Task<IEnumerable<Pelicula>> ObtenerPorNombre(string nombre);
        Task<IEnumerable<Pelicula>> ObtenerTodo(PaginacionDTO paginacionDTO);
    }
}