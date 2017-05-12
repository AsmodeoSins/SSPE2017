using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ControlPenales
{
    public class cCredencialBiblioteca
    {
        public short Id_Centro { get; set; }
        public short Id_Anio { get; set; }
        public int Id_Imputado { get; set; }
        public short Id_Ingreso { get; set; }
        public string Nombre { get; set; }
        public string Paterno { get; set; }
        public string Materno { get; set; }
        public string CentroDescr { get; set; }
        public string EdificioDescr { get; set; }
        public string SectorDescr { get; set; }
        public string Celda { get; set; }
        public byte[] Foto { get; set; }
        public byte[] CodigoBarras { get; set; }
        public bool Seleccionado { get; set; }
        public byte[] FOTOINGRESO { get; set; }
        public byte[] FOTOCENTRO { get; set; }
    }
}
