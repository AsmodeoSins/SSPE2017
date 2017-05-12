using SSP.Servidor;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ControlPenales
{
    public class cReporte
    {
        public byte[] Logo1{set; get;}
        public byte[] Logo2{set; get;}
        public string Encabezado1 {set; get;}
        public string Encabezado2 {set; get;}
        public string Encabezado3 {set; get;}
        public string Encabezado4 {set; get;}
        public byte[] ImagenIzquierda { get; set; }
        public byte[] ImagenMedio { get; set; }
        public byte[] ImagenDerecha { get; set; }
    }
}
