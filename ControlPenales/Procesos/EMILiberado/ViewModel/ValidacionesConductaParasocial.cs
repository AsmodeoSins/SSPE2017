using System;
namespace ControlPenales
{
    partial class EMILiberadoViewModel
    {
        void setValidacionesConductaParasocial()
        {
            base.ClearRules();
            //if (SelectIngreso != null)
            //{
            //#region [Uso de Drogas]
            //#region [Uso de Drogas]
            //base.AddRule(() => Droga, () => Droga != null, "ESTE CAMPO ES REQUERIDO PARA GRABAR");
            //base.AddRule(() => EdadIncio, () => EdadIncio != null, "ESTE CAMPO ES REQUERIDO PARA GRABAR");
            //base.AddRule(() => FechaUltimaDosis, () => FechaUltimaDosis != null, "ESTE CAMPO ES REQUERIDO PARA GRABAR");
            //base.AddRule(() => FrecuenciaUso, () => FrecuenciaUso != null, "ESTE CAMPO ES REQUERIDO PARA GRABAR");
            //base.AddRule(() => ConsumoActual, () => !string.IsNullOrEmpty(ConsumoActual), "ESTE CAMPO ES REQUERIDO PARA GRABAR");
            //base.AddRule(() => TiempoConsumo, () => TiempoConsumo != null, "ESTE CAMPO ES REQUERIDO PARA GRABAR");
            //#endregion
            //#endregion
            //#region [Homosexualidad, Pandillas, Sexualidad]
            //#region [Conducta Parasocial]
            //base.AddRule(() => VivioCalleOrfanato, () => !string.IsNullOrEmpty(VivioCalleOrfanato), "ESTE CAMPO ES REQUERIDO PARA GRABAR");
            //base.AddRule(() => PertenecePandilla, () => !string.IsNullOrEmpty(PertenecePandilla), "ESTE CAMPO ES REQUERIDO PARA GRABAR");
            //base.AddRule(() => PandillaNombre, () => PandillaNombre != null, "ESTE CAMPO ES REQUERIDO PARA GRABAR");
            //#region [Conducta]
            //base.AddRule(() => Homosexual, () => !string.IsNullOrEmpty(Homosexual), "ESTE CAMPO ES REQUERIDO PARA GRABAR");
            //base.AddRule(() => HomosexualEdadIncial, () => HomosexualEdadIncial != null, "ESTE CAMPO ES REQUERIDO PARA GRABAR");
            //base.AddRule(() => HomosexualRol, () => !string.IsNullOrEmpty(HomosexualRol), "ESTE CAMPO ES REQUERIDO PARA GRABAR");
            //base.AddRule(() => PertenecioPandillaExterior, () => !string.IsNullOrEmpty(PertenecioPandillaExterior), "ESTE CAMPO ES REQUERIDO PARA GRABAR");
            //base.AddRule(() => PandillaExteriorEdadInicial, () => PandillaExteriorEdadInicial != null, "ESTE CAMPO ES REQUERIDO PARA GRABAR");
            //base.AddRule(() => PandillaExteriorMotivo, () => !string.IsNullOrEmpty(PandillaExteriorMotivo), "ESTE CAMPO ES REQUERIDO PARA GRABAR");
            //base.AddRule(() => Vagancia, () => !string.IsNullOrEmpty(Vagancia), "ESTE CAMPO ES REQUERIDO PARA GRABAR");
            //base.AddRule(() => VaganciaEdadIncial, () => VaganciaEdadIncial != null, "ESTE CAMPO ES REQUERIDO PARA GRABAR");
            //base.AddRule(() => VaganciaMotivos, () => !string.IsNullOrEmpty(VaganciaMotivos), "ESTE CAMPO ES REQUERIDO PARA GRABAR");
            //base.AddRule(() => Cicatrices, () => !string.IsNullOrEmpty(Cicatrices), "ESTE CAMPO ES REQUERIDO PARA GRABAR");
            //base.AddRule(() => CicatricesEdadIncial, () => CicatricesEdadIncial != null, "ESTE CAMPO ES REQUERIDO PARA GRABAR");
            //base.AddRule(() => CicatricesMotivo, () => !string.IsNullOrEmpty(CicatricesMotivo), "ESTE CAMPO ES REQUERIDO PARA GRABAR");
            //base.AddRule(() => CicatricesRina, () => !string.IsNullOrEmpty(CicatricesRina.ToString()), "ESTE CAMPO ES REQUERIDO PARA GRABAR");
            //base.AddRule(() => DesercionEscolar, () => !string.IsNullOrEmpty(DesercionEscolar), "ESTE CAMPO ES REQUERIDO PARA GRABAR");
            //base.AddRule(() => DesercionMotivo, () => !string.IsNullOrEmpty(DesercionMotivo), "ESTE CAMPO ES REQUERIDO PARA GRABAR");
            //base.AddRule(() => ReprobacionEscolar, () => !string.IsNullOrEmpty(ReprobacionEscolar), "ESTE CAMPO ES REQUERIDO PARA GRABAR");
            //base.AddRule(() => ReprobacionEscolarMotivo, () => !string.IsNullOrEmpty(ReprobacionEscolarMotivo), "ESTE CAMPO ES REQUERIDO PARA GRABAR");
            //base.AddRule(() => ExplusionEscolar, () => !string.IsNullOrEmpty(ExplusionEscolar), "ESTE CAMPO ES REQUERIDO PARA GRABAR");
            //base.AddRule(() => ExplusionEscolarMotivo, () => !string.IsNullOrEmpty(ExplusionEscolarMotivo), "ESTE CAMPO ES REQUERIDO PARA GRABAR");
            //#endregion
            //#region [Pagaba por Servicio Sexual]
            //base.AddRule(() => ConHombres, () => !string.IsNullOrEmpty(ConHombres.ToString()), "ESTE CAMPO ES REQUERIDO PARA GRABAR");
            //base.AddRule(() => ConMujeres, () => !string.IsNullOrEmpty(ConMujeres.ToString()), "ESTE CAMPO ES REQUERIDO PARA GRABAR");
            //#endregion
            //#endregion
            //#endregion
            //#region [Tatuajes]
            //#region [Tatuajes]
            //base.AddRule(() => CantidadAntesIngresoAntisocial, () => CantidadAntesIngresoAntisocial != null, "ESTE CAMPO ES REQUERIDO PARA GRABAR");
            //base.AddRule(() => CantidadIntramurosAntisocial, () => CantidadIntramurosAntisocial != null, "ESTE CAMPO ES REQUERIDO PARA GRABAR");
            //base.AddRule(() => CantidadAntesIngresoErotico, () => CantidadAntesIngresoErotico != null, "ESTE CAMPO ES REQUERIDO PARA GRABAR");
            //base.AddRule(() => CantidadIntramurosErotico, () => CantidadIntramurosErotico != null, "ESTE CAMPO ES REQUERIDO PARA GRABAR");
            //base.AddRule(() => CantidadAntesIngresoReligioso, () => (CantidadAntesIngresoReligioso) != null, "ESTE CAMPO ES REQUERIDO PARA GRABAR");
            //base.AddRule(() => CantidadIntramurosReligioso, () => CantidadIntramurosReligioso != null, "ESTE CAMPO ES REQUERIDO PARA GRABAR");
            //base.AddRule(() => CantidadAntesIngresoIdentificacion, () => CantidadAntesIngresoIdentificacion != null, "ESTE CAMPO ES REQUERIDO PARA GRABAR");
            //base.AddRule(() => CantidadIntramurosIdentificacion, () => CantidadIntramurosIdentificacion != null, "ESTE CAMPO ES REQUERIDO PARA GRABAR");
            //base.AddRule(() => CantidadAntesIngresoDecorativo, () => CantidadAntesIngresoDecorativo != null, "ESTE CAMPO ES REQUERIDO PARA GRABAR");
            //base.AddRule(() => CantidadIntramurosDecorativo, () => CantidadIntramurosDecorativo != null, "ESTE CAMPO ES REQUERIDO PARA GRABAR");
            //base.AddRule(() => CantidadAntesIngresoSentimental, () => CantidadAntesIngresoSentimental != null, "ESTE CAMPO ES REQUERIDO PARA GRABAR");
            //base.AddRule(() => CantidadIntramurosSentimental, () => CantidadIntramurosSentimental != null, "ESTE CAMPO ES REQUERIDO PARA GRABAR");
            //base.AddRule(() => TatuajesTotal, () => TatuajesTotal != null, "ESTE CAMPO ES REQUERIDO PARA GRABAR");
            //base.AddRule(() => TatuajesDescripcion, () => !string.IsNullOrEmpty(TatuajesDescripcion), "ESTE CAMPO ES REQUERIDO PARA GRABAR");
            //#endregion
            //#endregion
            //#region [Enfermedades]
            /*#region [Descripcion de Enfermedades Cronicas, Degenerativas, e Infectocontagiosas]
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
             base.AddRule(() => DiscapacidadesMotivo, () => !string.IsNullOrEmpty(DiscapacidadesMotivo), "ESTE CAMPO ES REQUERIDO PARA GRABAR");
             base.AddRule(() => EnfermoMental, () => !string.IsNullOrEmpty(EnfermoMental), "ESTE CAMPO ES REQUERIDO PARA GRABAR");
             base.AddRule(() => EnfermoMentalMotivo, () => !string.IsNullOrEmpty(EnfermoMentalMotivo), "ESTE CAMPO ES REQUERIDO PARA GRABAR");
             base.AddRule(() => VIHHepatitis, () => !string.IsNullOrEmpty(VIHHepatitis), "ESTE CAMPO ES REQUERIDO PARA GRABAR");
             base.AddRule(() => VIHHepatitisTratamientoFarmaco, () => !string.IsNullOrEmpty(VIHHepatitisTratamientoFarmaco), "ESTE CAMPO ES REQUERIDO PARA GRABAR");
             base.AddRule(() => VIHHepatitisDiagnosticoFormal, () => !string.IsNullOrEmpty(VIHHepatitisDiagnosticoFormal), "ESTE CAMPO ES REQUERIDO PARA GRABAR");
             #endregion
             #endregion*/
            // #endregion
            // }
        }

        void setValidacionesUsoDrogasPop()
        {
            try
            {
                base.ClearRules();
                base.AddRule(() => popUpDrogaId, () => popUpDrogaId > 0, "ESTE CAMPO ES REQUERIDO PARA GRABAR");
                base.AddRule(() => popUpEdadInicio, () => popUpEdadInicio <= EdadInterno, "LA EDAD ES MAYOR A LA ACTUAL");
                if (_popUpFechaUltDosis.HasValue)
                {
                    base.AddRule(() => popUpFechaUltDosis, () => popUpFechaUltDosis.HasValue ? popUpFechaUltDosis.Value <= Fechas.GetFechaDateServer : false, "LA FECHA SELECCIONADA NO PUEDE SER MAYOR AL DIA ACTUAL");
                    OnPropertyChanged("popUpFechaUltDosis");
                }
                base.AddRule(() => popUpFrecuenciaUso, () => popUpFrecuenciaUso > 0, "ESTE CAMPO ES REQUERIDO PARA GRABAR");
                base.AddRule(() => popUpConsumoActual, () => !string.IsNullOrEmpty(popUpConsumoActual), "ESTE CAMPO ES REQUERIDO PARA GRABAR");
                base.AddRule(() => popUpTiempoConsumo, () => popUpTiempoConsumo > 0, "ESTE CAMPO ES REQUERIDO PARA GRABAR");

                OnPropertyChanged("popUpDrogaId");
                OnPropertyChanged("popUpEdadInicio");
                OnPropertyChanged("popUpFrecuenciaUso");
                OnPropertyChanged("popUpConsumoActual");
                OnPropertyChanged("popUpTiempoConsumo");
            }
            catch (Exception ex)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al establecer validaciones en uso dorga", ex);
            }
        }
    }
}