namespace MinimalApiPeliculas.Servicios
{
    public interface IAlmacenadorArchivos
    {
        Task Borrar(string? borrar, string contenedor);
        Task<string> Almacenar(string contenedor, IFormFile archivo);
        async Task<string> Editar(string ruta, string contenedor, IFormFile archivo)
        {
            await Borrar(ruta, contenedor);
            return await Almacenar(contenedor, archivo);
        }
    }
}
