using Microsoft.OpenApi.Any;
using Microsoft.OpenApi.Models;

namespace MinimalApiPeliculas.Utilidades
{
    public static class SwaggerExtensions
    {
        public static TBuilder AgregarParametrosPeliculasFiltroOpenApi<TBuilder>(this TBuilder builder)
            where TBuilder : IEndpointConventionBuilder
        {
            return builder.WithOpenApi(opciones =>
            {
                opciones.Parameters.Add(new OpenApiParameter
                {
                    Name = "pagina",
                    In = ParameterLocation.Query,
                    Schema = new OpenApiSchema
                    {
                        Type = "integer",
                        Default = new OpenApiInteger(1)
                    }
                });

                opciones.Parameters.Add(new OpenApiParameter
                {
                    Name = "recordsPorPagina",
                    In = ParameterLocation.Query,
                    Schema = new OpenApiSchema
                    {
                        Type = "integer",
                        Default = new OpenApiInteger(10)
                    }
                });

                opciones.Parameters.Add(new OpenApiParameter
                {
                    Name = "generoId",
                    In = ParameterLocation.Query,
                    Schema = new OpenApiSchema
                    {
                        Type = "integer",
                        Default = new OpenApiInteger(0)
                    }
                });

                opciones.Parameters.Add(new OpenApiParameter
                {
                    Name = "titulo",
                    In = ParameterLocation.Query,
                    Schema = new OpenApiSchema
                    {
                        Type = "string"
                    }
                });

                opciones.Parameters.Add(new OpenApiParameter
                {
                    Name = "enCines",
                    In = ParameterLocation.Query,
                    Schema = new OpenApiSchema
                    {
                        Type = "boolean",
                        Default = new OpenApiBoolean(false)
                    }
                });

                opciones.Parameters.Add(new OpenApiParameter
                {
                    Name = "proximosEstrenos",
                    In = ParameterLocation.Query,
                    Schema = new OpenApiSchema
                    {
                        Type = "boolean",
                        Default = new OpenApiBoolean(false)
                    }
                });

                opciones.Parameters.Add(new OpenApiParameter
                {
                    Name = "campoOrdenar",
                    In = ParameterLocation.Query,
                    Schema = new OpenApiSchema
                    {
                        Type = "string",
                    }
                });

                opciones.Parameters.Add(new OpenApiParameter
                {
                    Name = "ordenAscendente",
                    In = ParameterLocation.Query,
                    Schema = new OpenApiSchema
                    {
                        Type = "boolean",
                    }
                });

                return opciones;
            });
        }

        public static TBuilder AgregarParamterosPaginacionOpenApi<TBuilder>(this TBuilder builder)
            where TBuilder : IEndpointConventionBuilder
        {
            return builder.WithOpenApi(opciones =>
            {
                opciones.Parameters.Add(new OpenApiParameter
                {
                    Name = "pagina",
                    In = ParameterLocation.Query,
                    Schema = new OpenApiSchema
                    {
                        Type = "integer",
                        Default = new OpenApiInteger(1)
                    }
                });

                opciones.Parameters.Add(new OpenApiParameter
                {
                    Name = "recordsPorPagina",
                    In = ParameterLocation.Query,
                    Schema = new OpenApiSchema
                    {
                        Type = "integer",
                        Default = new OpenApiInteger(10)
                    }
                });

                return opciones;
            });
        }
    }
}
