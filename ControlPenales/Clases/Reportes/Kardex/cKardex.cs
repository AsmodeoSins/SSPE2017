using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ControlPenales
{
    public class cKardex
    {
        public string EXPEDIENTE { get; set; }
        public string NOMBRE { get; set; }
        public int EDAD { get; set; }
        public string APODO { get; set; }
        public DateTime FECHA_NACIMIENTO { get; set; }
        public string SEXO { get; set; }
        public string ESTADO_CIVIL { get; set; }
        public string CENTRO { get; set; }
        public string ESTATUS_JURIDICO { get; set; }
        public string UBICACION { get; set; }
        public int NO_INGRESO { get; set; }
        public string CAUSA_PENAL { get; set; }
        public string CENTRO_PROCEDENCIA { get; set; }
        public byte[] FOTO { get; set; }
        public DateTime FECHA_IMPRESION { get; set; }
    }

    public class cKardexJuridico
    {
        public int SENTENCIA_ANIOS { get; set; }
        public int SENTENCIA_MESES { get; set; }
        public int SENTENCIA_DIAS { get; set; }
        public int COMPURGADO_ANIOS { get; set; }
        public int COMPURGADO_MESES { get; set; }
        public int COMPURGADO_DIAS { get; set; }
        public int POR_COMPURGADO_ANIOS { get; set; }
        public int POR_COMPURGADO_MESES { get; set; }
        public int POR_COMPURGADO_DIAS { get; set; }
        public string CUMPLE_3_5_PARTES { get; set; }
        public DateTime? FECHA_3_5_PARTES { get; set; }
        public DateTime FECHA_LIBERTAD { get; set; }
    }

    public class cKardexContenido
    {
        public int ID_CONSEC { get; set; }
        public short ID_CENTRO { get; set; }
        public short ID_ANIO { get; set; }
        public int ID_IMPUTADO { get; set; }
        public short ID_INGRESO { get; set; }
        public int? ID_DEPARTAMENTO { get; set; }
        public string DEPARTAMENTO { get; set; }
        public int? ID_TIPO_PROGRAMA { get; set; }
        public string TIPO_PROGRAMA { get; set; }
        public string ESTATUS { get; set; }
        public int? ID_EJE { get; set; }
        public string EJE { get; set; }
        public string GRUPO { get; set; }
        public DateTime? FEC_INICIO { get; set; }
        public DateTime? FEC_FIN { get; set; }
        public string RECURRENCIA { get; set; }
        //public DateTime? HORA_INICIO { get; set; }
        //public DateTime? HORA_TERMINO { get; set; }
        public int? ID_ACTIVIDAD { get; set; }
        public string ACTIVIDAD { get; set; }
        public int? ID_GRUPO { get; set; }
        public DateTime? FEC_REGISTRO { get; set; }
        public string CALIFICACION { get; set; }
    }

    public class cKardexHorario
    {
        public int ID_CENTRO { get; set; }
        public int ID_TIPO_PROGRAMA { get; set; }
        public int ID_ACTIVIDAD { get; set; }
        public int ID_GRUPO { get; set; }
        public int ID_DIA { get; set; }
        public string DIA { get; set; }
        public int ID_GRUPO_HORARIO { get; set; }
        public DateTime? HORA_INICIO { get; set; }
        public DateTime? HORA_TERMINO { get; set; }
    }
}
