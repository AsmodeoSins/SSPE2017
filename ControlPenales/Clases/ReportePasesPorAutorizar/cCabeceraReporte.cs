using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ControlPenales.Clases.ReportePasesPorAutorizar
{
    public class cCabeceraReporte
    {
        public byte[] LogoIzquierda { get; set; }
        public byte[] LogoDerecha { get; set; }
        public string Encabezado1 { get; set; }
        public string Encabezado2 { get; set; }
        public string Centro { get; set; }
        public string Titulo { get; set; }                       
    }
}
