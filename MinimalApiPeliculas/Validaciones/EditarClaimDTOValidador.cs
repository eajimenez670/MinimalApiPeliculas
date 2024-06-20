using FluentValidation;
using MinimalApiPeliculas.DTOs;

namespace MinimalApiPeliculas.Validaciones
{
    public class EditarClaimDTOValidador : AbstractValidator<EditarClaimDTO>
    {
        public EditarClaimDTOValidador()
        {
            RuleFor(x => x.Email)
                .NotEmpty().WithMessage(Utilidades.NotEmptyMensaje)
                .MaximumLength(256).WithMessage(Utilidades.MaximunLengthMensaje)
                .EmailAddress().WithMessage(Utilidades.EmailAddressMensaje);
        }
    }
}
