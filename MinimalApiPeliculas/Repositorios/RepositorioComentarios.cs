using Microsoft.EntityFrameworkCore;
using MinimalApiPeliculas.Entidades;

namespace MinimalApiPeliculas.Repositorios
{
    public class RepositorioComentarios : IRepositorioComentarios
    {
        private readonly ApplicationDbContext _dbContext;

        public RepositorioComentarios(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task Actualizar(Comentario comentario)
        {
            _dbContext.Update(comentario);
            await _dbContext.SaveChangesAsync();
        }

        public async Task Borrar(int id)
        {
            await _dbContext.Comentarios.Where(x => x.Id == id).ExecuteDeleteAsync();
        }

        public async Task<int> Crear(Comentario comentario)
        {
            await _dbContext.AddAsync(comentario);
            await _dbContext.SaveChangesAsync();

            return comentario.Id;
        }

        public async Task<bool> Existe(int id)
        {
            return await _dbContext.Comentarios.AnyAsync(x => x.Id == id);
        }

        public async Task<Comentario?> ObtenerPorId(int id)
        {
            return await _dbContext.Comentarios.FirstOrDefaultAsync(x => x.Id == id);
        }

        public async Task<IEnumerable<Comentario>> ObtenerTodo(int peliculaId)
        {
            return await _dbContext.Comentarios.Where(x => x.PeliculaId == peliculaId).ToListAsync();
        }
    }
}
