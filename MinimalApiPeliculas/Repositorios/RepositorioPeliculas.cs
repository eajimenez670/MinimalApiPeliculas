using AutoMapper;
using Microsoft.EntityFrameworkCore;
using MinimalApiPeliculas.DTOs;
using MinimalApiPeliculas.Entidades;
using MinimalApiPeliculas.Utilidades;
using System.Linq.Dynamic.Core;

namespace MinimalApiPeliculas.Repositorios
{
    public class RepositorioPeliculas : IRepositorioPeliculas
    {
        private readonly ApplicationDbContext _dbContext;
        private readonly IMapper _mapper;
        private readonly ILogger<RepositorioPeliculas> _logger;
        private readonly HttpContext _httpContext;

        public RepositorioPeliculas(ApplicationDbContext context, IHttpContextAccessor httpContextAccessor,
            IMapper mapper, ILogger<RepositorioPeliculas> logger)
        {
            _dbContext = context;
            _mapper = mapper;
            _logger = logger;
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

        public async Task<List<Pelicula>> Filtrar(PeliculasFiltrarDTO peliculasFiltrarDTO)
        {
            var peliculasQueryable = _dbContext.Peliculas.AsQueryable();

            if (!string.IsNullOrWhiteSpace(peliculasFiltrarDTO.Titulo))
            {
                peliculasQueryable = peliculasQueryable.Where(p => p.Titulo.Contains(peliculasFiltrarDTO.Titulo));
            }

            if (peliculasFiltrarDTO.EnCines)
            {
                peliculasQueryable = peliculasQueryable.Where(p => p.EnCines);
            }

            if (peliculasFiltrarDTO.ProximosEstrenos)
            {
                peliculasQueryable = peliculasQueryable.Where(p => p.FechaLanzamiento > DateTime.Today);
            }

            if (peliculasFiltrarDTO.GeneroId != 0)
            {
                peliculasQueryable = peliculasQueryable
                    .Where(p => p.GenerosPeliculas.Select(gp => gp.GeneroId)
                    .Contains(peliculasFiltrarDTO.GeneroId));
            }

            if (!string.IsNullOrWhiteSpace(peliculasFiltrarDTO.CampoOrdenar))
            {
                var tipoOrden = peliculasFiltrarDTO.OrdenAscendente ? "ascending" : "descending";
                try
                {
                    peliculasQueryable = peliculasQueryable.OrderBy($"{peliculasFiltrarDTO.CampoOrdenar} {tipoOrden}");
                }
                catch (Exception e)
                {
                    _logger.LogError(e.Message, e);
                }
            }

            await _httpContext.InsertarParametrosPaginacionEnCabecera(peliculasQueryable);
            var peliculas = await peliculasQueryable.Paginar(peliculasFiltrarDTO.PaginacionDTO).ToListAsync();

            return peliculas;
        }
    }
}
