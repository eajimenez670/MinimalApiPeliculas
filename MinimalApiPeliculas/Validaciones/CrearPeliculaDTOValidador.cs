using FluentValidation;
using MinimalApiPeliculas.DTOs;

namespace MinimalApiPeliculas.Validaciones
{
    public class CrearPeliculaDTOValidador : AbstractValidator<CrearPeliculaDTO>
    {
        public CrearPeliculaDTOValidador()
        {
            RuleFor(x => x.Titulo).NotEmpty().WithMessage(Utilidades.NotEmptyMensaje)
                                  .MaximumLength(150).WithMessage(Utilidades.MaximunLengthMensaje);
        }
    }
}
