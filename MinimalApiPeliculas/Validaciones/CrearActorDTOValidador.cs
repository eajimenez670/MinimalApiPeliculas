﻿using FluentValidation;
using MinimalApiPeliculas.DTOs;

namespace MinimalApiPeliculas.Validaciones
{
    public class CrearActorDTOValidador : AbstractValidator<CrearActorDTO>
    {
        public CrearActorDTOValidador()
        {
            RuleFor(x => x.Nombre)
                   .NotEmpty().WithMessage(Utilidades.NotEmptyMensaje)
                   .MaximumLength(150).WithMessage(Utilidades.MaximunLengthMensaje);

            var fechaMinima = new DateTime(1900, 1, 1);

            RuleFor(x => x.FechaNacimiento).GreaterThanOrEqualTo(fechaMinima)
                .WithMessage(Utilidades.GreaterThanOrEqualToMensaje(fechaMinima));
        }
    }
}
