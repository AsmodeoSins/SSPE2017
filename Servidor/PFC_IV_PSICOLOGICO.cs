//------------------------------------------------------------------------------
// <auto-generated>
//    This code was generated from a template.
//
//    Manual changes to this file may cause unexpected behavior in your application.
//    Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace SSP.Servidor
{
    using System;
    using System.Collections.Generic;
    
    public partial class PFC_IV_PSICOLOGICO
    {
        public PFC_IV_PSICOLOGICO()
        {
            this.PFC_IV_PROGRAMA = new HashSet<PFC_IV_PROGRAMA>();
        }
    
        public short ID_CENTRO { get; set; }
        public short ID_ANIO { get; set; }
        public int ID_IMPUTADO { get; set; }
        public short ID_INGRESO { get; set; }
        public short ID_ESTUDIO { get; set; }
        public string P1_CONDICIONES_GRALES { get; set; }
        public string P2_EXAMEN_MENTAL { get; set; }
        public string P3_PRINCIPALES_RASGOS { get; set; }
        public Nullable<short> P4_TEST_GUALTICO { get; set; }
        public Nullable<short> P4_TEST_MATRICES { get; set; }
        public Nullable<short> P4_TEST_HTP { get; set; }
        public Nullable<short> P4_INVENTARIO_MULTIFASICO { get; set; }
        public Nullable<short> P4_OTRAS { get; set; }
        public string P4_OTRA_MENCIONAR { get; set; }
        public Nullable<short> P51_NIVEL_INTELECTUAL { get; set; }
        public Nullable<short> P52_DISFUNCION_NEUROLOGICA { get; set; }
        public string P6_INTEGRACION { get; set; }
        public string P8_RASGOS_PERSONALIDAD { get; set; }
        public Nullable<short> P9_DICTAMEN_REINSERCION { get; set; }
        public string P10_MOTIVACION_DICTAMEN { get; set; }
        public string P11_CASO_NEGATIVO { get; set; }
        public string P12_REQUIERE_TRATAMIENTO { get; set; }
        public string P12_CUAL { get; set; }
        public Nullable<System.DateTime> ESTUDIO_FEC { get; set; }
        public string COORDINADOR { get; set; }
        public string ELABORO { get; set; }
    
        public virtual PERSONALIDAD_FUERO_COMUN PERSONALIDAD_FUERO_COMUN { get; set; }
        public virtual PFC_IV_DISFUNCION PFC_IV_DISFUNCION { get; set; }
        public virtual PFC_IV_NIVEL_INTELECTUAL PFC_IV_NIVEL_INTELECTUAL { get; set; }
        public virtual ICollection<PFC_IV_PROGRAMA> PFC_IV_PROGRAMA { get; set; }
    }
}
