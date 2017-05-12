using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ControlPenales
{
    class cSocioFamiliaresEMI
    {
        public string VisitaFamiliar { set; get; }
        public string FrecuenciaVF { set; get; }
        public string VisitaIntima { set; get; }
        public string ApoyoEconomico { set; get; }
        public string CantidadAE { set; get; }
        public string FrecuenciaAE { set; get; }

        public string VivePadre { set; get; }
        public string EdadImpFallecioPAdre { set; get; }
        public string ViveMadre { set; get; }
        public string EdadImpFallecioMadre { set; get; }
        public string PadresJuntos { set; get; }
        public string MotivoSeparacionPadres { set; get; }
        public string EdadSeparacionPadres { set; get; }

        public string NivelSocial { set; get; }
        public string NivelCultural { set; get; }
        public string NivelEconomico { set; get; }

        public string TotalParejas { set; get; }
        public string Union { set; get; }

        public string NoHijos { set; get; }
        public string HijosRegistrados { set; get; }
        public string HijosRelacion { set; get; }
        public string HijosVisitan{ set; get; }

        public string ContactoNombre { set; get; }
        public string ContactoPArentesco { set; get; }
        public string ContactoTelefono { set; get; }

        public string AbandoonoFamiliar { set; get; }
        public string DescrAFam { set; get; }
        public string HuidasHogar { set; get; }
        public string MaltratoEmocional { set; get; }
        public string DescrME { set; get; }
        public string MaltratoFisico { set; get; }
        public string DescrMF { set; get; }
        public string AbusoFisico { set; get; }
        public string DescrAF { set; get; }
        public string EdadAbuso { set; get; }
        public string InicioContactoSexual { set; get; }
    }
}
