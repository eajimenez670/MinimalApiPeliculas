using Microsoft.EntityFrameworkCore;
using MinimalApiPeliculas.Entidades;

namespace MinimalApiPeliculas.Repositorios
{
    public class RepositorioActores : IRepositorioActores
    {
        private readonly ApplicationDbContext _dbContext;

        public RepositorioActores(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task Actualizar(Actor actor)
        {
            _dbContext.Update(actor);
            await _dbContext.SaveChangesAsync();
        }

        public async Task Borrar(int id)
        {
            await _dbContext.Actores.Where(x => x.Id == id).ExecuteDeleteAsync();
        }

        public async Task<int> Crear(Actor actor)
        {
            await _dbContext.AddAsync(actor);
            await _dbContext.SaveChangesAsync();

            return actor.Id;
        }

        public async Task<bool> Existe(int id)
        {
            return await _dbContext.Actores.AnyAsync(x => x.Id == id);
        }

        public async Task<Actor?> ObtenerPorId(int id)
        {
            return await _dbContext.Actores.AsNoTracking().FirstOrDefaultAsync(x => x.Id == id);
        }

        public async Task<IEnumerable<Actor>> ObtenerTodo()
        {
            return await _dbContext.Actores.OrderBy(x => x.Nombre).ToListAsync();
        }
        public async Task<IEnumerable<Actor>> ObtenerPorNombre(string nombre)
        {
            return await _dbContext.Actores
                .Where(a => a.Nombre.Contains(nombre))
                .OrderBy(a => a.Nombre)
                .ToListAsync();
        }
    }
}
