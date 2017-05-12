using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ControlPenales
{
    public class cHojaReferenciaMedica
    {
        public string NOMBRE { get; set; }
        public int EDAD { get; set; }
        public string FOLIO { get; set; }
        public string SEXO { get; set; }
        public string FECHA_NAC { get; set; }
        public string CAUSA_SOLICITUD { get; set; }  //en el formato es ESPECIALIDAD/ESTUDIOS SOLICITADOS
        public string ANT_HEREDO_FAMILIARES { get; set; }
        public string ANT_PERSONALES_NO_PAT { get; set; }
        public string ANT_PERSONALES_PAT { get; set; }
        public string PRIMERA { get; set; }
        public string SUBSECUENTE { get; set; }
        public string TENSION_ARTERIAL { get; set; }
        public string FREC_CARDIACA { get; set; }
        public string FREC_RESPIRATORIA { get; set; }
        public string PESO { get; set; }
        public string RESUMEN_CLINICO { get; set; }
        public string DESTINO { get; set; }
        public string TIPO_ATENCION { get; set; }
        public string IMPRESION_DIAGNOSTICA { get; set; }
        public string OBSERVACIONES { get; set; }
        public string CIUDAD { get; set; }
        public string ESTADO { get; set; }
        public string CENTRO { get; set; }
        public string FECHA_REGISTRO_LETRA { get; set; }
        public string EXP_HGT { get; set; }
        public string TALLA { get; set; }
        public string NOMBRE_MEDICO { get; set; }
    }
}
