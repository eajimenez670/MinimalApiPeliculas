using Microsoft.EntityFrameworkCore;
using MinimalApiPeliculas.Entidades;

namespace MinimalApiPeliculas.Repositorios
{
    public class RepositorioGeneros : IRepositorioGeneros
    {
        private readonly ApplicationDbContext _dbContext;

        public RepositorioGeneros(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task Actualizar(Genero genero)
        {
            _dbContext.Update(genero);
            await _dbContext.SaveChangesAsync();
        }

        public async Task Borrar(int id)
        {
            await _dbContext.Generos.Where(x => x.Id == id).ExecuteDeleteAsync();
        }

        public async Task<int> Crear(Genero genero)
        {
            await _dbContext.AddAsync(genero);
            await _dbContext.SaveChangesAsync();

            return genero.Id;
        }

        public async Task<bool> Existe(int id)
        {
            return await _dbContext.Generos.AnyAsync(x => x.Id == id);
        }

        public async Task<bool> Existe(int id, string nombre)
        {
            return await _dbContext.Generos.AnyAsync(x => x.Id != id && x.Nombre == nombre);
        }

        public async Task<Genero?> ObtenerPorId(int id)
        {
            return await _dbContext.Generos.FirstOrDefaultAsync(x => x.Id == id);
        }

        public async Task<IEnumerable<Genero>> ObtenerTodo()
        {
            return await _dbContext.Generos.OrderBy(x => x.Nombre).ToListAsync();
        }

        public async Task<List<int>> Existen(List<int> ids)
        {
            return await _dbContext.Generos.Where(g => ids.Contains(g.Id)).Select(g => g.Id).ToListAsync();
        }
    }
}
