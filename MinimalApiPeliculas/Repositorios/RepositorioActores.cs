using Microsoft.EntityFrameworkCore;
using MinimalApiPeliculas.DTOs;
using MinimalApiPeliculas.Entidades;
using MinimalApiPeliculas.Utilidades;

namespace MinimalApiPeliculas.Repositorios
{
    public class RepositorioActores : IRepositorioActores
    {
        private readonly ApplicationDbContext _dbContext;
        private readonly HttpContext _httpContext;

        public RepositorioActores(ApplicationDbContext dbContext, IHttpContextAccessor httpContextAccessor)
        {
            _dbContext = dbContext;
            _httpContext = httpContextAccessor.HttpContext!;
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

        public async Task<List<int>> Existen(List<int> ids)
        {
            return await _dbContext.Actores.Where(a => ids.Contains(a.Id)).Select(a => a.Id).ToListAsync();
        }

        public async Task<Actor?> ObtenerPorId(int id)
        {
            return await _dbContext.Actores.AsNoTracking().FirstOrDefaultAsync(x => x.Id == id);
        }

        public async Task<IEnumerable<Actor>> ObtenerTodo(PaginacionDTO paginacionDTO)
        {
            var queryable = _dbContext.Actores.AsQueryable();
            await _httpContext.InsertarParametrosPaginacionEnCabecera(queryable);
            return await queryable.OrderBy(x => x.Nombre).Paginar(paginacionDTO).ToListAsync();
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
