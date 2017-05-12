using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ControlPenales
{
    public class cImputadoCausaPenalAbogado
    {
        public int ID_ABOGADO { get; set; }
        public short ID_CENTRO { get; set; }
        public short ID_ANIO { get; set; }
        public int ID_IMPUTADO { get; set; }
        public short ID_INGRESO { get; set; }
        public string NOMBRE { get; set; }
        public string PATERNO { get; set; }
        public string MATERNO { get; set; }
        public string NOMBRE_COMPLETO
        {
            get
            {
                var _temp = NOMBRE;
                if (!string.IsNullOrWhiteSpace(PATERNO))
                    _temp += " " + PATERNO;
                if (!string.IsNullOrWhiteSpace(MATERNO))
                    _temp += " " + MATERNO;
                return _temp;
            }
        }
        public string ID_FOLIO
        {
            get
            {
                return ID_ANIO.ToString() + '/' + ID_IMPUTADO.ToString();
            }
        }

    }
}
