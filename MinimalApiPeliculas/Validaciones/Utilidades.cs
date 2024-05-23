namespace MinimalApiPeliculas.Validaciones
{
    public static class Utilidades
    {
        public static string NotEmptyMensaje = "El campo {PropertyName} es requerido";
        public static string MaximunLengthMensaje = "El campo {PropertyName} debe tener menos de {MaxLength} caracteres";
        public static string PrimeraLetraEnMayusculaMensaje = "El campo {PropertyName} debe comenzar con mayúscula";

        public static string GreaterThanOrEqualToMensaje(DateTime fechaMinima) =>
            "El campo {PropertyName} debe ser posterior a " + fechaMinima.ToString("yyyy-MM-dd");

        public static bool PrimeraLetraEnMayuscula(string valor)
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
