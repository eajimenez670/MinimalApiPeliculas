using MinimalApiPeliculas.Entidades;

namespace MinimalApiPeliculas.Repositorios
{
    public interface IRepositorioGeneros
    {
        Task<int> Crear(Genero genero);

        Task<IEnumerable<Genero>> ObtenerTodo();

        Task<Genero?> ObtenerPorId(int id);

        Task<bool> Existe(int id);

        Task Actualizar(Genero genero);

        Task Borrar(int id);
        Task<List<int>> Existen(List<int> ids);
    }
}
