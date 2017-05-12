using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SSP.Servidor.ModelosExtendidos
{
    public class EXT_RequisicionExtraordinaria_Encabezado
    {
        public string Centro { get; set; }
        public string Almacen_Tipo_Cat { get; set; }
        public string Almacen { get; set; }
        public string Periodo { get; set; }
        public DateTime FechaRequisicion { get; set; }
        public string Estatus { get; set; }
        public string SolicitadoPor { get; set; }
        public int Folio { get; set; }
        public byte[] LOGO_BC { get; set; }
    }
}
