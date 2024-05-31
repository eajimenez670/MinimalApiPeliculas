using MinimalApiPeliculas.Entidades;

namespace MinimalApiPeliculas.Repositorios
{
    public class RepositorioErrores : IRepositorioErrores
    {
        private readonly ApplicationDbContext _context;

        public RepositorioErrores(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task Crear(Error error)
        {
            _context.Add(error);
            await _context.SaveChangesAsync();
        }
    }
}
