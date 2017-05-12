using System;
namespace ControlPenales
{
    partial class EMILiberadoViewModel
    {
        void setValidacionesEnfermedades()
        {
            try
            {
                base.ClearRules();
                #region [Enfermedades]
                #region [Descripcion de Enfermedades Cronicas, Degenerativas, e Infectocontagiosas]
                #region [Descripcion en Caso de Presentarlas o Tener Antecedentes]
                #endregion
                #region [Apariencia Fisica]
                base.AddRule(() => AparienciaFisicaAlineado, () => !string.IsNullOrEmpty(AparienciaFisicaAlineado), "ESTE CAMPO ES REQUERIDO PARA GRABAR");
                base.AddRule(() => AparienciaFisicaConformado, () => !string.IsNullOrEmpty(AparienciaFisicaConformado), "ESTE CAMPO ES REQUERIDO PARA GRABAR");
                base.AddRule(() => AparienciaFisicaIntegro, () => !string.IsNullOrEmpty(AparienciaFisicaIntegro), "ESTE CAMPO ES REQUERIDO PARA GRABAR");
                base.AddRule(() => AparienciaFisicaLimpio, () => !string.IsNullOrEmpty(AparienciaFisicaLimpio), "ESTE CAMPO ES REQUERIDO PARA GRABAR");
                #endregion
                #region [Especifique]
                base.AddRule(() => Discapacidades, () => !string.IsNullOrEmpty(Discapacidades), "ESTE CAMPO ES REQUERIDO PARA GRABAR");
                if (Discapacidades != null)
                    if (Discapacidades.Equals("S"))
                        base.AddRule(() => DiscapacidadesMotivo, () => !string.IsNullOrEmpty(DiscapacidadesMotivo), "ESTE CAMPO ES REQUERIDO PARA GRABAR");
                base.AddRule(() => EnfermoMental, () => !string.IsNullOrEmpty(EnfermoMental), "ESTE CAMPO ES REQUERIDO PARA GRABAR");
                if (EnfermoMental != null)
                    if (EnfermoMental.Equals("S"))
                        base.AddRule(() => EnfermoMentalMotivo, () => !string.IsNullOrEmpty(EnfermoMentalMotivo), "ESTE CAMPO ES REQUERIDO PARA GRABAR");
                base.AddRule(() => VIHHepatitis, () => !string.IsNullOrEmpty(VIHHepatitis), "ESTE CAMPO ES REQUERIDO PARA GRABAR");
                if (VIHHepatitis != null)
                    if (VIHHepatitis.Equals("S"))
                    {
                        base.AddRule(() => VIHHepatitisTratamientoFarmaco, () => !string.IsNullOrEmpty(VIHHepatitisTratamientoFarmaco), "ESTE CAMPO ES REQUERIDO PARA GRABAR");
                        base.AddRule(() => VIHHepatitisDiagnosticoFormal, () => !string.IsNullOrEmpty(VIHHepatitisDiagnosticoFormal), "ESTE CAMPO ES REQUERIDO PARA GRABAR");
                    }
                #endregion
                #endregion
                #endregion
                OnPropertyChanged();
            }
            catch (Exception ex)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al establecer validaciones en enfermedades", ex);
            }
        }
    }
}
