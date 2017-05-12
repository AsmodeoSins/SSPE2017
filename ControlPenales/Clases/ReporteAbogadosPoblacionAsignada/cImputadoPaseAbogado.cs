using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ControlPenales
{
    public class cImputadoPaseAbogado
    {
        public int ID_ABOGADO { get; set; }
        public short ID_CENTRO { get; set; }
        public short ID_ANIO { get; set; }
        public int ID_IMPUTADO { get; set; }
        public short ID_INGRESO { get; set; }
        public short? ID_CAUSA_PENAL { get; set; }
        public short? CP_ANIO { get; set; }
        public int? CP_FOLIO { get; set; }
        public string FOLIO_COMPUESTO
        {
            get
            {
                var _temp=string.Empty;
                if (CP_ANIO.HasValue)
                    _temp += CP_ANIO.Value.ToString() + "/";
                if (CP_FOLIO.HasValue)
                    _temp += CP_FOLIO.Value.ToString().PadLeft(5, '0');
                return _temp;
            }
        }
        public short? ID_ESTATUS_VISITA { get; set; }
    }
}
