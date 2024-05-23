
using AutoMapper;
using MinimalApiPeliculas.Repositorios;

namespace MinimalApiPeliculas.Filtros
{
    public class FiltroDePrueba : IEndpointFilter
    {
        public async ValueTask<object?> InvokeAsync(EndpointFilterInvocationContext contexto, EndpointFilterDelegate next)
        {
            // Este código se ejecuta antes del endpoint

            var param1 = contexto.Arguments.OfType<IRepositorioGeneros>().FirstOrDefault();
            var params2 = contexto.Arguments.OfType<int>().FirstOrDefault();
            var params3 = contexto.Arguments.OfType<IMapper>().FirstOrDefault();

            var resultado = await next(contexto);

            // Este código se ejecuta después del endpoint

            return resultado;
        }
    }
}
