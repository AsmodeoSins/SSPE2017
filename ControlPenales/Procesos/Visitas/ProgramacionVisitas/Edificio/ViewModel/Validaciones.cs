using System;
using System.Windows.Input;

namespace ControlPenales
{
    partial class ProgramacionVisitaEdificioViewModel
    {
        void SetValidaciones()
        {
            try
            {
                base.ClearRules();
                //var fecha = Fechas.GetFechaDateServer;
                base.AddRule(() => SelectDiaVisita, () => SelectDiaVisita > 0, "FECHA ES REQUERIDA!");
                base.AddRule(() => SelectTipoVisita, () => SelectTipoVisita.HasValue ? SelectTipoVisita.Value > 0 : SelectTipoVisita.HasValue, "TIPO DE VISITA ES REQUERIDA!");
                base.AddRule(() => SelectEdificio, () => SelectEdificio.HasValue ? SelectEdificio.Value > 0 : SelectEdificio.HasValue, "EDIFICO ES REQUERIDO!");
                base.AddRule(() => SelectSector, () => SelectSector.HasValue ? SelectSector.Value > 0 : SelectSector.HasValue, "SECTOR ES REQUERIDO!");
                base.AddRule(() => SelectCeldaInicio, () => !SelectCeldaInicio.Equals("SELECCIONE"), "CELDA DE INICIO ES REQUERIDA!");
                base.AddRule(() => SelectCeldaFin, () => !SelectCeldaFin.Equals("SELECCIONE"), "CELDA FINAL ES REQUERIDA!");
                base.AddRule(() => SelectArea, () => SelectArea > 0, "AREA ES REQUERIDA!");

                OnPropertyChanged("SelectDiaVisita");
                OnPropertyChanged("SelectTipoVisita");
                OnPropertyChanged("SelectEdificio");
                OnPropertyChanged("SelectSector");
                OnPropertyChanged("SelectCeldaInicio");
                OnPropertyChanged("SelectCeldaFin");
            }
            catch (Exception ex)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al cargar validaciones.", ex);
            }
        }
    }
}
