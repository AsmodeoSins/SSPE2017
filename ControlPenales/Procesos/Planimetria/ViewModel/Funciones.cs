using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using SSP.Servidor;
using SSP.Modelo;
using SSP.Controlador;
using SSP.Controlador.Catalogo.Justicia;
namespace ControlPenales
{
    partial class PlanimetriaViewModel
    {
        private void PopulateClasificacionSector() 
        {
            try
            {
                LstSectorClasificacion = new ObservableCollection<SECTOR_CLASIFICACION>(new cSectorClasificacion().ObtenerTodas());
                LstSectorClasificacion.Insert(0, new SECTOR_CLASIFICACION() { ID_SECTOR_CLAS = -1, POBLACION = "SELECCIONE" });
            }
            catch (Exception ex)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al cargar clasificaciones", ex);
            }
        }

        //IMPUTADOS
        private void PopulateImputadoSector() 
        {
            try
            {
                if (SelectedSector != null)
                {
                    if (SelectedObservacion != null)
                        LstImputadoSector = new ObservableCollection<INGRESO>(new cIngreso().ObtenerIngresosPorSector(SelectedSector.ID_CENTRO, SelectedSector.ID_EDIFICIO, SelectedSector.ID_SECTOR, SelectedObservacion.ID_SECTOR_OBS));
                    else
                        LstImputadoSector = new ObservableCollection<INGRESO>();
                    EmptyImputados = LstImputadoSector.Count > 0 ? false : true;
                }
                else
                    EmptyImputados = true;
            }
            catch (Exception ex) 
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al cargar el listado de internos por sector", ex);
            }
        }

        private void PopulateImputadoCelda()
        {
            try
            {
                if (LstImputadoSector != null)
                {
                    LstImputadoCelda = new ObservableCollection<INGRESO>(LstImputadoSector.Where(w => w.ID_UB_CELDA == SelectedSectorObservacionCelda.ID_CELDA));
                }
                else
                    LstImputadoCelda = new ObservableCollection<INGRESO>();
                EmptyImputados = LstImputadoCelda.Count > 0 ? false : true;
            }
            catch (Exception ex)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al cargar el listado de internos por celda", ex);
            }
        }
    }
}
