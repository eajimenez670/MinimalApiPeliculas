using FluentValidation;
using MinimalApiPeliculas.DTOs;

namespace MinimalApiPeliculas.Validaciones
{
    public class CredencialesUsuarioDTOValidador : AbstractValidator<CredencialesUsuarioDTO>
    {
        public CredencialesUsuarioDTOValidador()
        {
            RuleFor(x => x.Email)
                .NotEmpty().WithMessage(Utilidades.NotEmptyMensaje)
                .MaximumLength(256).WithMessage(Utilidades.MaximunLengthMensaje)
                .EmailAddress().WithMessage(Utilidades.EmailAddressMensaje);

            RuleFor(x => x.Password)
                .NotEmpty().WithMessage(Utilidades.NotEmptyMensaje);
        }
    }
}
