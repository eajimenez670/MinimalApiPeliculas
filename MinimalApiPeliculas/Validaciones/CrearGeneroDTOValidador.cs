using FluentValidation;
using MinimalApiPeliculas.DTOs;

namespace MinimalApiPeliculas.Validaciones
{
    public class CrearGeneroDTOValidador : AbstractValidator<CrearGeneroDTO>
    {
        public CrearGeneroDTOValidador()
        {
            RuleFor(x => x.Nombre).NotEmpty().WithMessage("El campo {PropertyName} es requerido")
                                  .MaximumLength(50).WithMessage("El campo {PropertyName} debe tener menos de {MaxLength} caracteres")
                                  .Must(PrimeraLetraEnMayuscula).WithMessage("El campo {PropertyName} debe comenzar con mayúscula");

        }

        private bool PrimeraLetraEnMayuscula(string valor)
        {
            if (string.IsNullOrWhiteSpace(valor))
            {
                return true;
            }

            var primeraLetra = valor[0].ToString();

            return primeraLetra == primeraLetra.ToUpper();
        }
    }
}
