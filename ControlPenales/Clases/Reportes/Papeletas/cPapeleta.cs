using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ControlPenales
{
    public class cPapeleta
    {
        public string Encabezado1 { get; set; }
        public string Encabezado2 { get; set; }
        public string Encabezado3 { get; set; }
        public byte[] Logo1 { get; set; }
        public byte[] Logo2 { get; set; }
        public string Nombre { get; set; }
        public string Paterno { get; set; }
        public string Materno { get; set; }
        public short Id_Anio { get; set; }
        public int Id_Imputado { get; set; }
        public string Edificio { get; set; }
        public string Sector { get; set; }
        public string Celda { get; set; }
        public int Cama { get; set; }
        public byte[] FotoInterno { get; set; } 
        public byte[] CNDH {get;set;}
        public byte[] CEDHBC { get; set; }
    }
}
