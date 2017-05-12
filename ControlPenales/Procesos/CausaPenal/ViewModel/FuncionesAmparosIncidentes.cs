using System;
using System.Collections.ObjectModel;
using SSP.Servidor;
using System.Collections.Generic;
using ControlPenales.Clases;
using System.Linq;
using System.Windows.Media.Imaging;
using System.IO;
using SSP.Controlador.Catalogo.Justicia;

namespace ControlPenales
{
    partial class CausaPenalViewModel : ValidationViewModelBase
    {
        private void LimpiarAmparoIncidente()
        {
            try
            {
                FecAIn = null;
                DiasRemisionAIn = null;
                IncidenteAIn = -1;
                IncidenteResultadoAIn = string.Empty;
            }
            catch (Exception ex)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al limpiar incidentes", ex);
            }
        }

        private void PopulateAmparoIncidente() {
            try
            {
                if (SelectedAmparoIncidente != null)
                {
                    FecAIn = SelectedAmparoIncidente.CAPTURA_FEC;
                    DiasRemisionAIn = SelectedAmparoIncidente.DIAS_REMISION;
                    //IncidenteAIn = SelectedAmparoIncidente.ID_TIPO_RECURSO;
                    IncidenteResultadoAIn = SelectedAmparoIncidente.RESULTADO != null ? SelectedAmparoIncidente.RESULTADO : string.Empty ;
                }
            }
            catch (Exception ex)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al establecer incidentes", ex);
            }
        }

        private void PopulateAmparoIncidenteListado()
        {
            try
            {
                if (SelectedCausaPenal != null)
                {
                    LstAmparoIncidente = new ObservableCollection<AMPARO_INCIDENTE>(new cAmparoIncidente().ObtenerTodos(SelectedCausaPenal.ID_CENTRO, SelectedCausaPenal.ID_ANIO, SelectedCausaPenal.ID_IMPUTADO, SelectedCausaPenal.ID_INGRESO, SelectedCausaPenal.ID_CAUSA_PENAL));
                    AmparoIncidenteEmpty = LstAmparoIncidente.Count > 0 ? false : true;
                }
            }
            catch (Exception ex)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al establecer el listado de incidentes", ex);
            }
        }


        private bool GuardarAmparoIncidente() {
            try
            {
                if (SelectedCausaPenal != null)
                {
                    if (!base.HasErrors)
                    {
                        var obj = new AMPARO_INCIDENTE();
                        obj.ID_CENTRO = SelectedCausaPenal.ID_CENTRO;
                        obj.ID_ANIO = SelectedCausaPenal.ID_ANIO;
                        obj.ID_IMPUTADO = SelectedCausaPenal.ID_IMPUTADO;
                        obj.ID_INGRESO = SelectedCausaPenal.ID_INGRESO;
                        obj.ID_CAUSA_PENAL = SelectedCausaPenal.ID_CAUSA_PENAL;

                        obj.CAPTURA_FEC = FecAIn;
                        obj.DIAS_REMISION = DiasRemisionAIn;
                        //obj.ID_TIPO_RECURSO = IncidenteAIn;
                        if (!string.IsNullOrEmpty(IncidenteResultadoAIn))
                            obj.RESULTADO = IncidenteResultadoAIn;
                
                        if (SelectedAmparoIncidente == null) //INSERT
                        {
                            obj.ID_AMPARO_INCIDENTE = new cAmparoIncidente().Insertar(obj);
                            if (obj.ID_AMPARO_INCIDENTE > 0)
                            {
                                return true;
                            }
                        }
                        else //UPDATE
                        {
                            obj.ID_AMPARO_INCIDENTE = SelectedAmparoIncidente.ID_AMPARO_INCIDENTE;
                            if ((new cAmparoIncidente()).Actualizar(obj))
                            {
                                return true;
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al guardar incidente", ex);
            }
            return false; 
        }

      

        private bool ValidaGuardarIncidencia() {
            if (IncidenteAIn != 4)
            {
                if (SelectedCausaPenal.SENTENCIA != null)
                {
                    var x = SelectedCausaPenal.SENTENCIA.FirstOrDefault();
                    if (x != null)
                    {
                        if ((x.ANIOS > 0 || x.MESES > 0 || x.DIAS > 0) && x.FEC_EJECUTORIA != null)
                        {
                            return false;
                        }
                    }
                }
            }
            return true;
        }

    }
}
