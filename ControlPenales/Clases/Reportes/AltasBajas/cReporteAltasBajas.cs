using System;
using System.Text.RegularExpressions;

namespace ControlPenales
{
    public class cReporteAltasBajas
    {
        public int Id_Anio { set; get; }
        public int Id_Imputado { set; get; }
        public int Id_Ingreso { set; get; }
        public string Ubicacion { set; get; }
        public string Edificio { set; get; }
        public string Sector { set; get; }
        public string Id_Celda { set; get; }
        public short? Id_Cama { set; get; }
        public string Nombre { set; get; }
        public string Paterno { set; get; }
        public string Materno { set; get; }
        public int Anio_Gob { set; get; }
        public string Folio_Gob { set; get; }
        public DateTime Fecha { set; get; }
    }
}
