using System;
namespace ControlPenales
{
    partial class EntrevistaMultidiciplinariaViewModel
    {
        void setValidacionesFactoresSocioFamiliares()
        {
            try
            {
                base.ClearRules();
                //base.AddRule(() => RecibeVisitaFamiliar, () => !string.IsNullOrEmpty(RecibeVisitaFamiliar), "ESTE CAMPO ES REQUERIDO PARA GRABAR");
                //base.AddRule(() => Frecuencia, () => Frecuencia != null, "ESTE CAMPO ES REQUERIDO PARA GRABAR");
                //base.AddRule(() => VisitaIntima, () => !string.IsNullOrEmpty(VisitaIntima), "ESTE CAMPO ES REQUERIDO PARA GRABAR");
                //base.AddRule(() => ApoyoEconomico, () => !string.IsNullOrEmpty(ApoyoEconomico), "ESTE CAMPO ES REQUERIDO PARA GRABAR");
                //base.AddRule(() => VivePadre, () => !string.IsNullOrEmpty(VivePadre), "ESTE CAMPO ES REQUERIDO PARA GRABAR");
                //base.AddRule(() => ViveMadre, () => !string.IsNullOrEmpty(ViveMadre), "ESTE CAMPO ES REQUERIDO PARA GRABAR");
                //base.AddRule(() => PadresVivenJuntos, () => !string.IsNullOrEmpty(PadresVivenJuntos), "ESTE CAMPO ES REQUERIDO PARA GRABAR");
                //base.AddRule(() => FallecioPadre, () => FallecioPadre != null, "ESTE CAMPO ES REQUERIDO PARA GRABAR");
                //base.AddRule(() => FallecioMadre, () => FallecioMadre != null, "ESTE CAMPO ES REQUERIDO PARA GRABAR");
                //base.AddRule(() => MotivoSeparacion, () => !string.IsNullOrEmpty(MotivoSeparacion), "ESTE CAMPO ES REQUERIDO PARA GRABAR");
                //base.AddRule(() => EdadInternoSeparacionPadres, () => EdadInternoSeparacionPadres != null, "ESTE CAMPO ES REQUERIDO PARA GRABAR");
                //base.AddRule(() => Social, () => Social != null, "ESTE CAMPO ES REQUERIDO PARA GRABAR");
                //base.AddRule(() => Cultural, () => Cultural != null, "ESTE CAMPO ES REQUERIDO PARA GRABAR");
                //base.AddRule(() => Economico, () => Economico != null, "ESTE CAMPO ES REQUERIDO PARA GRABAR");
                //base.AddRule(() => TotalParejas, () => TotalParejas != null, "ESTE CAMPO ES REQUERIDO PARA GRABAR");
                //base.AddRule(() => CuantasUnion, () => CuantasUnion != null, "ESTE CAMPO ES REQUERIDO PARA GRABAR");
                //base.AddRule(() => Hijos, () => Hijos != null, "ESTE CAMPO ES REQUERIDO PARA GRABAR");
                //base.AddRule(() => Registrados, () => Registrados != null, "ESTE CAMPO ES REQUERIDO PARA GRABAR");
                //base.AddRule(() => CuantosMantieneRelacion, () => CuantosMantieneRelacion != null, "ESTE CAMPO ES REQUERIDO PARA GRABAR");
                //base.AddRule(() => CuantosVisitan, () => CuantosVisitan != null, "ESTE CAMPO ES REQUERIDO PARA GRABAR");
                //base.AddRule(() => ContactoNombre, () => ContactoNombre != null, "ESTE CAMPO ES REQUERIDO PARA GRABAR");
                //base.AddRule(() => ContactoParentesco, () => ContactoParentesco != null, "ESTE CAMPO ES REQUERIDO PARA GRABAR");
                //base.AddRule(() => ContactoTelefono, () => ContactoTelefono != null, "ESTE CAMPO ES REQUERIDO PARA GRABAR");
                //base.AddRule(() => AbandonoFamiliar, () => !string.IsNullOrEmpty(AbandonoFamiliar), "ESTE CAMPO ES REQUERIDO PARA GRABAR");
                //base.AddRule(() => EspecifiqueAbandonoFamiliar, () => !string.IsNullOrEmpty(EspecifiqueAbandonoFamiliar), "ESTE CAMPO ES REQUERIDO PARA GRABAR");
                //base.AddRule(() => HuidasHogar, () => HuidasHogar != null, "ESTE CAMPO ES REQUERIDO PARA GRABAR");
                //base.AddRule(() => MaltratoEmocional, () => !string.IsNullOrEmpty(MaltratoEmocional), "ESTE CAMPO ES REQUERIDO PARA GRABAR");
                //base.AddRule(() => EspecifiqueMaltratoEmocional, () => !string.IsNullOrEmpty(EspecifiqueMaltratoEmocional), "ESTE CAMPO ES REQUERIDO PARA GRABAR");
                //base.AddRule(() => MaltratoFisico, () => !string.IsNullOrEmpty(MaltratoFisico), "ESTE CAMPO ES REQUERIDO PARA GRABAR");
                //base.AddRule(() => EspecifiqueMaltratoFisico, () => !string.IsNullOrEmpty(EspecifiqueMaltratoFisico), "ESTE CAMPO ES REQUERIDO PARA GRABAR");
                //base.AddRule(() => AbusoSexual, () => !string.IsNullOrEmpty(AbusoSexual), "ESTE CAMPO ES REQUERIDO PARA GRABAR");
                //base.AddRule(() => EspecifiqueAbusoSexual, () => !string.IsNullOrEmpty(EspecifiqueAbusoSexual), "ESTE CAMPO ES REQUERIDO PARA GRABAR");
                //base.AddRule(() => EdadAbuso, () => EdadAbuso != null, "ESTE CAMPO ES REQUERIDO PARA GRABAR");
                //base.AddRule(() => EdadInicioContactoSexual, () => EdadInicioContactoSexual != null, "ESTE CAMPO ES REQUERIDO PARA GRABAR");
                //base.AddRule(() => AGFParentesco, () => AGFParentesco != null, "ESTE CAMPO ES REQUERIDO PARA GRABAR");
                //base.AddRule(() => AGFAño, () => AGFAño != null, "ESTE CAMPO ES REQUERIDO PARA GRABAR");
                //base.AddRule(() => AGFCereso, () => AGFCereso != null, "ESTE CAMPO ES REQUERIDO PARA GRABAR");
                //base.AddRule(() => AGFDelito, () => AGFDelito != null, "ESTE CAMPO ES REQUERIDO PARA GRABAR");
                //base.AddRule(() => AGFRelacion, () => AGFRelacion != null, "ESTE CAMPO ES REQUERIDO PARA GRABAR");
            }
            catch (Exception ex)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al establecer validaciones en factores socio familiares", ex);
            }
        }
    }
}
