using System;
namespace ControlPenales
{
    partial class EMILiberadoViewModel
    {
        void setValidacionesSituacionJuridica()
        {
            try
            {
                base.ClearRules();
                //base.AddRule(() => VersionDelito, () => !string.IsNullOrEmpty(VersionDelito), "ESTE CAMPO ES REQUERIDO PARA GRABAR");
                //base.AddRule(() => MenorPeriodoLibreReingreso, () => !string.IsNullOrEmpty(MenorPeriodoLibreReingreso), "ESTE CAMPO ES REQUERIDO PARA GRABAR");
                //base.AddRule(() => MayorPeriodoLibreReingreso, () => !string.IsNullOrEmpty(MayorPeriodoLibreReingreso), "ESTE CAMPO ES REQUERIDO PARA GRABAR");
                //base.AddRule(() => PracticadoEstudios, () => !string.IsNullOrEmpty(PracticadoEstudios.ToString()), "ESTE CAMPO ES REQUERIDO PARA GRABAR");
                //base.AddRule(() => Cuando, () => !string.IsNullOrEmpty(Cuando), "ESTE CAMPO ES REQUERIDO PARA GRABAR");
                //base.AddRule(() => Traslado, () => !string.IsNullOrEmpty(Traslado.ToString()), "ESTE CAMPO ES REQUERIDO PARA GRABAR");
                //base.AddRule(() => Donde, () => !string.IsNullOrEmpty(Donde), "ESTE CAMPO ES REQUERIDO PARA GRABAR");
                //base.AddRule(() => PorqueMotivo, () => !string.IsNullOrEmpty(PorqueMotivo), "ESTE CAMPO ES REQUERIDO PARA GRABAR");
            }
            catch (Exception ex)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al establecer validaciones en situación jurídica", ex);
            }
        }
        void setValidacionesEstudiosTraslado()
        {
            try
            {
                base.ClearRules();
                //base.AddRule(() => VersionDelito, () => !string.IsNullOrEmpty(VersionDelito), "ESTE CAMPO ES REQUERIDO PARA GRABAR");
                //if (PracticadoEstudios)
                //{
                //    base.AddRule(() => Cuando, () => !string.IsNullOrEmpty(Cuando), "ESTE CAMPO ES REQUERIDO PARA GRABAR");
                //}
                //if (Traslado)
                //{
                //    base.AddRule(() => Donde, () => !string.IsNullOrEmpty(Donde), "ESTE CAMPO ES REQUERIDO PARA GRABAR");
                //    base.AddRule(() => PorqueMotivo, () => !string.IsNullOrEmpty(PorqueMotivo), "ESTE CAMPO ES REQUERIDO PARA GRABAR");
                //}
                //OnPropertyChanged("VersionDelito");
                //OnPropertyChanged("Cuando");
                //OnPropertyChanged("Donde");
                //OnPropertyChanged("PorqueMotivo");
            }
            catch (Exception ex)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al establecer validaciones en estudio traslado", ex);
            }
        }

        void setValidacionesIngresosAnterioresPop()
        {
            try
            {
                base.ClearRules();
                base.AddRule(() => SelectedEmisorIngreso, () => SelectedEmisorIngreso != null ? SelectedEmisorIngreso.ID_EMISOR != -1 : false, "EMISOR ES REQUERIDO");
                //base.AddRule(() => SelectedDelitoIngreso, () => SelectedDelitoIngreso != null ? SelectedDelitoIngreso.ID_DELITO != -1 : false, "ESTE CAMPO ES REQUERIDO PARA GRABAR");
                base.AddRule(() => PeriodoReclusionIngreso, () => string.IsNullOrEmpty(PeriodoReclusionIngreso) == false, "PERIODO DE RECLUSION ES REQUERIDO");
                base.AddRule(() => DelitoDescripcion, () => string.IsNullOrEmpty(DelitoDescripcion) == false, "DELITO ES REQUERIDO");
                base.AddRule(() => SancionesIngreso, () => string.IsNullOrEmpty(SancionesIngreso) == false, "SANCION ES REQUERIDA");
                OnPropertyChanged("SelectedEmisorIngreso");
                //OnPropertyChanged("SelectedDelitoIngreso");
                OnPropertyChanged("PeriodoReclusionIngreso");
                OnPropertyChanged("SancionesIngreso");
                OnPropertyChanged("DelitoDescripcion");
            }
            catch (Exception ex)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al establecer validaciones en ingreso anterior", ex);
            }
        }

        void setValidacionesIngresosAnteriores()
        {
            base.ClearRules();
        }

        /////
        void setValidacionesFamiliarDelitoPop()
        {
            try
            {//Se comenta el campo de anio en base a requerimiento
                base.ClearRules();
                base.AddRule(() => SelectedParentescoFDel, () => SelectedParentescoFDel != null ? SelectedParentescoFDel.ID_TIPO_REFERENCIA != -1 : false, "PARENTEZCO ES REQUERIDO");
                //base.AddRule(() => AnioFDel, () => AnioFDel.HasValue ? AnioFDel.Value <= Fechas.GetFechaDateServer.Year : false, "EL AÑO NO PUEDE SER MAYOR AL ACTUAL");
                base.AddRule(() => SelectedEmisorFDel, () => SelectedEmisorFDel != null ? SelectedEmisorFDel.ID_EMISOR != -1 : false, "EMISOR ES REQUERIDO");
                //base.AddRule(() => SelectedDelitoFDel, () => SelectedDelitoFDel != null ? SelectedDelitoFDel.ID_INGRESO_DELITO != -1 : false, "ESTE CAMPO ES REQUERIDO PARA GRABAR");
                base.AddRule(() => SelectedRelacionFDel, () => SelectedRelacionFDel != null ? SelectedRelacionFDel.ID_RELACION != -1 : false, "RELACIÓN ES REQUERIDA");
                base.AddRule(() => DelitoFDel, () => string.IsNullOrEmpty(DelitoFDel) == false, "DELITO ES REQUERIDO");
                OnPropertyChanged("SelectedParentescoFDel");
                OnPropertyChanged("AnioFDel");
                OnPropertyChanged("SelectedEmisorFDel");
                //OnPropertyChanged("SelectedDelitoFDel");
                OnPropertyChanged("DelitoFDel");
                OnPropertyChanged("SelectedRelacionFDel");
            }
            catch (Exception ex)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al establecer validaciones familiar delito", ex);
            }
        }

        void setValidacionesFamiliarDrogaPop()
        {
            try
            {//Se comenta el campo de anio en base a requerimiento
                base.ClearRules();
                base.AddRule(() => SelectedParentescoFDroga, () => SelectedParentescoFDroga != null ? SelectedParentescoFDroga.ID_TIPO_REFERENCIA != -1 : false, "PARENTEZCO ES REQUERIDO");
                //base.AddRule(() => AnioFDroga, () => AnioFDroga.HasValue ? AnioFDroga.Value <= Fechas.GetFechaDateServer.Year : false, "EL AÑO NO PUEDE SER MAYOR AL ACTUAL");
                base.AddRule(() => SelectedDrogaFDroga, () => SelectedDrogaFDroga != null ? SelectedDrogaFDroga.ID_DROGA != -1 : false, "DROGA ES REQUERIDO");
                base.AddRule(() => SelectedRelacionFDroga, () => SelectedRelacionFDroga != null ? SelectedRelacionFDroga.ID_RELACION != -1 : false, "RELACIÓN ES REQUERIDA");
                OnPropertyChanged("SelectedParentescoFDroga");
                OnPropertyChanged("AnioFDroga");
                OnPropertyChanged("SelectedDrogaFDroga");
                OnPropertyChanged("SelectedRelacionFDroga");
            }
            catch (Exception ex)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al establecer validaciones en familiar droga", ex);
            }
        }
    }
}
