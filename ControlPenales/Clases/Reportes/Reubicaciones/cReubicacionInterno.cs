using SSP.Servidor;
using System;
using System.Text.RegularExpressions;

namespace ControlPenales
{
    public class cReubicacionInterno
    {
        public cReubicacionInterno() { }
        public int Centro { get; set; }
        public short? Anio { get; set; }
        public int? Imputado { get; set; }
        public INGRESO_UBICACION_ANT Ingreso { get; set; }
        public short? IdIngreso { get; set; }
        public string Expediente { get; set; }
        public string Nombre { get; set; }
        public string UbicacionActual { get; set; }
        private CAMA cama;
        public CAMA Cama
        {
            get { return cama; }
            set
            {
                cama = value;
                if (value != null)
                {
                    UbicacionActual = string.Format("{0}-{1}-{2}-{3}",
                           value.CELDA.SECTOR.EDIFICIO.DESCR.Trim(),
                           value.CELDA.SECTOR.DESCR.Trim(),
                           value.ID_CELDA.Trim(),
                           value.ID_CAMA);
                }
            }
        }
    }

    public class cReubicacionInternoDetalle
    {
        public int Centro { get; set; }
        public int Anio { get; set; }
        public int Imputado { get; set; }
        public int IdIngreso { get; set; }
        public INGRESO_UBICACION_ANT Ingreso { get; set; }
        public string Ubicacion { get; set; }
        public DateTime? Fecha { get; set; }
        public string Motivo { get; set; }
        private int _CountTotal = 0;
        public int CountTotal
        {
            get { return _CountTotal; }
            set { _CountTotal = value; }
        }
    }
}
