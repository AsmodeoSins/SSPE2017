using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ControlPenales
{
    class cSituacionJuridicaEMI
    {
        public string VersionDelito { set; get; }
        public string MenorTiempoEntreIngresos { set; get; }
        public string MayorTiempoEntreIngresos { set; get; }

        public string PracticadoEstudios { set; get; }
        public string CuandoPracticaronEstudios { set; get; }
        public string DeseaTraslado { set; get; }
        public string DondeDeseaTraslado{set;get;}
        public string MotivoDeseaTraslado { set; get; }

    }
}
