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
        private void LimpiarRecusrsos()
        {
            try
            {
                TipoR = -3;
                TribunalR = -1;
                //SelectedRecursoResultado = LstRecursoResultado.Where(w => w.ID_TIPO_RECURSO == -1 && string.IsNullOrEmpty(w.RESULTADO)).FirstOrDefault();
                IdRecusrsoResultado = -1;
                RecursoResultado = string.Empty;
                if(LstRecursoResultado != null)
                    SelectedRecursoResultado = LstRecursoResultado.Where(w => w.ID_TIPO_RECURSO == IdRecusrsoResultado && string.IsNullOrEmpty(w.RESULTADO)).FirstOrDefault();
                FecR = FecResolucionR = null;
                FueroR = TocaPenalR = NoOficioR = ResolucionR = MultaR = MultaCondicionalR = ReparacionDanioR = SustitucionPenaR = MultaCondicionalR = string.Empty;
                AniosR = MesesR = DiasR = 0;
                RTipoRecurso = -1;
                RResultadoRecurso = string.Empty;
                LstRecursoDelitos = null;

            }
            catch (Exception ex)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al limpiar recursos", ex);
            }
        }

        private void PopulateRecurso() {
            try
            {
                if (SelectedRecurso != null)
                {
                    TipoR = SelectedRecurso.ID_TIPO_RECURSO;
                    TribunalR = SelectedRecurso.ID_TRIBUNAL;
                    //SelectedRecursoResultado = LstRecursoResultado.Where(w => w.ID_TIPO_RECURSO == SelectedRecurso.ID_TIPO_RECURSO && w.RESULTADO == SelectedRecurso.RESULTADO).FirstOrDefault();
                    RTipoRecurso = SelectedRecurso.ID_TIPO_RECURSO;
                    RResultadoRecurso = SelectedRecurso.RESULTADO;
                    FecR = SelectedRecurso.FEC_RECURSO;
                    FecResolucionR = SelectedRecurso.FEC_RESOLUCION;
                    FueroR = SelectedRecurso.FUERO;
                    TocaPenalR = SelectedRecurso.TOCA_PENAL;
                    NoOficioR = SelectedRecurso.NO_OFICIO;
                    ResolucionR = SelectedRecurso.RESOLUCION;
                    MultaR = SelectedRecurso.MULTA;
                    MultaCondicionalR = SelectedRecurso.MULTA_CONDICIONAL;
                    ReparacionDanioR = SelectedRecurso.REPARACION_DANIO;
                    SustitucionPenaR = SelectedRecurso.SUSTITUCION_PENA;
                    AniosR = SelectedRecurso.SENTENCIA_ANIOS;
                    MesesR = SelectedRecurso.SENTENCIA_MESES;
                    DiasR = SelectedRecurso.SENTENCIA_DIAS;

                    //DELITOS
                    PopulateDelitoRecusrso();
                }
            }
            catch (Exception ex)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al establecer recurso", ex);
            }
        }

        private void PopulateRecursoListado()
        {
            try
            {
                if (SelectedCausaPenal != null)
                {
                    LstRecursos = new ObservableCollection<RECURSO>(new cRecurso().ObtenerTodos(SelectedCausaPenal.ID_CENTRO, SelectedCausaPenal.ID_ANIO, SelectedCausaPenal.ID_IMPUTADO, SelectedCausaPenal.ID_INGRESO, SelectedCausaPenal.ID_CAUSA_PENAL));
                }
            }
            catch (Exception ex)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al establecer recurso listado", ex);
            }
        }

        private void LimpiarSentenciaRecurso()
        {
            try
            {
                RAnio = RMeses = RDias = null;
            }
            catch (Exception ex)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al limpiar sentencia recurso", ex);
            }
        }

        private void LimpiarMultasRecurso()
        {
            try
            {
                RMulta = RReparacionDanio = RSustitucionPena = RMultaCondicional = string.Empty;
            }
            catch (Exception ex)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al limpiar multas recurso", ex);
            }
        }

        private bool ValidarGuardarRecurso()
        {
            try
            {
                if (SelectedRecurso != null)
                    return true;
                if (SelectedCausaPenal != null)
                {
                    //PRIMERA INSTANCIA
                    if (SelectedTipoRecurso.ID_TIPO_RECURSO == 1)//(int)TipoRecurso.SentenciaPrimeraInstancia)
                    {
                        if (SelectedCausaPenal.RECURSO != null)
                        {
                            var rec = SelectedCausaPenal.RECURSO.Where(w => w.ID_TIPO_RECURSO == (int)TipoRecurso.SentenciaPrimeraInstancia).Count();
                            if (rec != 0)
                            {
                                new Dialogos().ConfirmacionDialogo("NOTIFICACIÓN", "Ya cuenta con un recurso de primera instancia");
                                return false;
                            }
                        }
                    }
                    //SEGUNDA INSTANCIA
                    if (SelectedTipoRecurso.ID_TIPO_RECURSO == 2)//(int)TipoRecurso.DeterminacionSegundaInstancia)
                    {
                        if (SelectedCausaPenal.RECURSO != null)
                        {
                            var rec = SelectedCausaPenal.RECURSO.Where(w => w.ID_TIPO_RECURSO == (int)TipoRecurso.SentenciaPrimeraInstancia).Count();
                            if (rec == 0)
                            {
                                new Dialogos().ConfirmacionDialogo("NOTIFICACIÓN", "No cuenta con un recurso de primera instancia");
                                return false;
                            }

                            rec = SelectedCausaPenal.RECURSO.Where(w => w.ID_TIPO_RECURSO == (int)TipoRecurso.DeterminacionSegundaInstancia).Count();
                            if (rec != 0)
                            {
                                new Dialogos().ConfirmacionDialogo("NOTIFICACIÓN", "Ya cuenta con un recurso de primera instancia");
                                return false;
                            }
                        }
                    }

                    //AMPARO
                    if (SelectedTipoRecurso.ID_TIPO_RECURSO == 3)//(int)TipoRecurso.AmparoApelacionSegundaInstancia)
                    {
                        if (SelectedCausaPenal.RECURSO != null)
                        {
                            var rec = SelectedCausaPenal.RECURSO.Where(w => w.ID_TIPO_RECURSO == (int)TipoRecurso.DeterminacionSegundaInstancia).Count();
                            if (rec == 0)
                            {
                                new Dialogos().ConfirmacionDialogo("NOTIFICACIÓN", "No cuenta con un recurso de segunda instancia");
                                return false;
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al validar guardar recurso", ex);
            }
            return true;
        }
    }
}
