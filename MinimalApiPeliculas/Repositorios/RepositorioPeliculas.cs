using AutoMapper;
using Microsoft.EntityFrameworkCore;
using MinimalApiPeliculas.DTOs;
using MinimalApiPeliculas.Entidades;
using MinimalApiPeliculas.Utilidades;

namespace MinimalApiPeliculas.Repositorios
{
    public class RepositorioPeliculas : IRepositorioPeliculas
    {
        private readonly ApplicationDbContext _dbContext;
        private readonly IMapper _mapper;
        private readonly HttpContext _httpContext;

        public RepositorioPeliculas(ApplicationDbContext context, IHttpContextAccessor httpContextAccessor,
            IMapper mapper)
        {
            _dbContext = context;
            _mapper = mapper;
            _httpContext = httpContextAccessor.HttpContext!;
        }

        public async Task Actualizar(Pelicula pelicula)
        {
            _dbContext.Update(pelicula);
            await _dbContext.SaveChangesAsync();
        }

        public async Task Borrar(int id)
        {
            await _dbContext.Peliculas.Where(x => x.Id == id).ExecuteDeleteAsync();
        }

        public async Task<int> Crear(Pelicula pelicula)
        {
            await _dbContext.AddAsync(pelicula);
            await _dbContext.SaveChangesAsync();
            return pelicula.Id;
        }

        public async Task<bool> Existe(int id)
        {
            return await _dbContext.Peliculas.AnyAsync(x => x.Id == id);
        }

        public async Task<Pelicula?> ObtenerPorId(int id)
        {
            return await _dbContext.Peliculas
                .Include(p => p.Comentarios)
                .Include(p => p.GenerosPeliculas)
                    .ThenInclude(gp => gp.Genero)
                .Include(p => p.ActoresPeliculas.OrderBy(a => a.Orden))
                    .ThenInclude(ap => ap.Actor)
                .AsNoTracking().FirstOrDefaultAsync(x => x.Id == id);
        }

        public async Task<IEnumerable<Pelicula>> ObtenerTodo(PaginacionDTO paginacionDTO)
        {
            var queryable = _dbContext.Peliculas.AsQueryable();
            await _httpContext.InsertarParametrosPaginacionEnCabecera(queryable);
            return await queryable.OrderBy(x => x.Titulo).Paginar(paginacionDTO).ToListAsync();
        }

        public async Task<IEnumerable<Pelicula>> ObtenerPorNombre(string nombre)
        {
            return await _dbContext.Peliculas
                .Where(a => a.Titulo.Contains(nombre))
                .OrderBy(a => a.Titulo)
                .ToListAsync();
        }

        public async Task AsignarGeneros(int id, List<int> generosIds)
        {
            var pelicula = await _dbContext.Peliculas.Include(p => p.GenerosPeliculas)
                                                      .FirstOrDefaultAsync(p => p.Id == id);

            if (pelicula is null)
                throw new ArgumentException($"No existe una película con el id {id}");

            var generosPeliculas = generosIds.Select(generoId => new GeneroPelicula { GeneroId = generoId });

            pelicula.GenerosPeliculas = _mapper.Map(generosPeliculas, pelicula.GenerosPeliculas);

            await _dbContext.SaveChangesAsync();
        }

        public async Task AsignarActores(int id, List<ActorPelicula> actores)
        {
            for (int i = 1; i <= actores.Count; i++)
            {
                actores[i - 1].Orden = i;
            }

            var pelicula = await _dbContext.Peliculas.Include(p => p.ActoresPeliculas)
                                                     .FirstOrDefaultAsync(p => p.Id == id);

            if (pelicula is null)
                throw new ArgumentException($"No existe una película con el id {id}");

            pelicula.ActoresPeliculas = _mapper.Map(actores, pelicula.ActoresPeliculas);

            await _dbContext.SaveChangesAsync();
        }

        public Task<List<Pelicula>> Filtrar(PeliculasFiltrarDTO peliculasFiltrarDTO)
        {
            
        }
    }
}
