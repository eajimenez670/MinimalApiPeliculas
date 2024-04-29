﻿using Microsoft.EntityFrameworkCore;
using MinimalApiPeliculas.DTOs;
using MinimalApiPeliculas.Entidades;
using MinimalApiPeliculas.Utilidades;

namespace MinimalApiPeliculas.Repositorios
{
    public class RepositorioPeliculas : IRepositorioPeliculas
    {
        private readonly ApplicationDbContext _dbContext;
        private readonly HttpContext _httpContext;

        public RepositorioPeliculas(ApplicationDbContext context, IHttpContextAccessor httpContextAccessor)
        {
            _dbContext = context;
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
            return await _dbContext.Peliculas.AsNoTracking().FirstOrDefaultAsync(x => x.Id == id);
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
    }
}
