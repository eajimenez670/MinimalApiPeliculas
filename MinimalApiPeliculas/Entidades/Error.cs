﻿namespace MinimalApiPeliculas.Entidades
{
    public class Error
    {
        public Guid Id { get; set; }
        public string MensajeDeError { get; set; } = null!;
        public string StackTrace { get; set; } = null!;
        public DateTime Fecha { get; set; }
    }
}
