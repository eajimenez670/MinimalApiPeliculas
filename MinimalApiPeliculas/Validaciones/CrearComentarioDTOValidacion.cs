using FluentValidation;
using MinimalApiPeliculas.DTOs;

namespace MinimalApiPeliculas.Validaciones
{
    public class CrearComentarioDTOValidacion : AbstractValidator<CrearComentarioDTO>
    {

        public CrearComentarioDTOValidacion()
        {
            RuleFor(x => x.Cuerpo).NotEmpty().WithMessage(Utilidades.NotEmptyMensaje);
        }

    }
}
