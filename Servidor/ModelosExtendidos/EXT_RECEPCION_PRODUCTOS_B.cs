using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SSP.Servidor.ModelosExtendidos
{
    public class EXT_RECEPCION_PRODUCTOS_B
    {
        public int ID_PRODUCTO { get; set; }
        public decimal ORDENADO { get; set; }
        public decimal RECIBIDO {get; set;}
        public decimal RESTANTE {get;set;}
        public List<EXT_RECEPCION_PRODUCTO_DETALLE_B> RECEPCION_PRODUCTO_DETALLE {get;set;}
        public INCIDENCIA_TIPO INCIDENCIA_TIPO {get;set;}
        public string INCIDENCIA_OBSERVACIONES {get;set;}
        public DateTime? FechaRecalendarizacion {get;set;}
    }
}
