using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ControlPenales
{
    public class cBitacoraPresentaciones
    {
        public string NOMBRE { get; set; }
        public string ESTATUS { get; set; }
        public string NUC { get; set; }
        public string CAUSA_PENAL { get; set; }
        public string DELITO { get; set; }
        public string MEDIDA_JUDICIAL { get; set; }
        public string PARTICULARIDAD { get; set; }
        public byte[] FOTO { get; set; }
    }

    public class cBitacoraPresentacionesDetalle
    {
        public DateTime? FECHA_VENCIMIENTO { get; set; }
        public DateTime? FECHA_CUMPLIMIENTO { get; set; }
        public string LUGAR_CUMPLIMIENTO { get; set; }
    }

}
