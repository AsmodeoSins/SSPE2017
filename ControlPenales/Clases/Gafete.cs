using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ControlPenales
{
    class Gafete
    {
        public string NombreInterno { get; set; }
        public string NombreVisitante { get; set; }
        public string NombreAcompanante { get; set; }
        public string Parentesco { get; set; }
        public string Observacion { get; set; }
        public string NumeroVisita { get; set; }
        public string NIP { get; set; }
        public string TipoVisita { get; set; }
        public string Centro { get; set; }
        public string Ciudad { get; set; }
        public string Fecha { get; set; }
        public string Usuario { get; set; }
        public string NombreDirectorCentro { get; set; }
        public byte[] LogoBackground { get; set; }
        public byte[] ImagenImputado { get; set; }
        public byte[] ImagenVisitante { get; set; }
        public byte[] ImagenMenor1 { get; set; }
        public string NombreMenor1 { get; set; }
        public byte[] ImagenMenor2 { get; set; }
        public string NombreMenor2 { get; set; }
        public byte[] ImagenMenor3 { get; set; }
        public string NombreMenor3 { get; set; }
    }
    class GafeteVisitaExterna
    {
        public string NombreVisitante { get; set; }
        public string Titulo { get; set; }
        public string NumeroCredencial { get; set; }
        public string TipoVisitante { get; set; }
        public string TipoVisita { get; set; }
        public string CERESO { get; set; }
        public string Telefono { get; set; }
        public string Discapacidad { get; set; }
        public string NIP { get; set; }
        public string DirectorCentro { get; set; }
        public string Fecha { get; set; }
        public byte[] ImagenVisitante { get; set; }
    }
    class GafeteAbogado
    {
        public string NombreAbogado { get; set; }
        public string TipoVisitante { get; set; }
        public string NumeroCredencial { get; set; }
        public string TipoVisita { get; set; }
        public string TipoPersona { get; set; }
        public string FechaAlta { get; set; }
        public string Telefono { get; set; }
        public string Discapacidad { get; set; }
        public string NipCereso1 { get; set; }
        public string NipCereso2 { get; set; }
        public string NipCereso3 { get; set; }
        public string NipCereso4 { get; set; }
        public string NipCereso5 { get; set; }
        public string NipCereso6 { get; set; }
        public string DescrCereso1 { get; set; }
        public string DescrCereso2 { get; set; }
        public string DescrCereso3 { get; set; }
        public string DescrCereso4 { get; set; }
        public string DescrCereso5 { get; set; }
        public string DescrCereso6 { get; set; }
        public string RFC { get; set; }
        public string Cedula { get; set; }
        public string Fecha { get; set; }
        public byte[] Imagen { get; set; }
    }
}
