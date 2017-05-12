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
        
        private void PopulateAmparoDirecto() {
            try
            {
                if (SelectedAmparoDirecto != null)
                {
                    FecDocumentoAD = SelectedAmparoDirecto.DOCUMENTO_FEC;
                    FecNotificacionAD = SelectedAmparoDirecto.NOTIFICACION_FEC;
                    FecSuspencionSentenciaAD = SelectedAmparoDirecto.SUSPENSION_FEC;
                    NoOficioAD = SelectedAmparoDirecto.OFICIO_NUM;
                    NoAD = SelectedAmparoDirecto.AMPARO_NUM;
                    AutoridadInformaAD = SelectedAmparoDirecto.SUSPENSION_AUT_INFORMA;
                    FecSentenciaAmparoAD = SelectedAmparoDirecto.SENTENCIA_FEC;
                    ResultadoSentenciaAD = SelectedAmparoDirecto.ID_SEN_AMP_RESULTADO;
                    AutoridadPronunciaSentenciaAmparo = SelectedAmparoDirecto.SENTENCIA_AMP_AUTORIDAD;
                }
            }
            catch (Exception ex)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al establecer amparo directo", ex);
            }
        }

        private void PopulateAmparoDirectoListado()
        {
            try
            {
                if (SelectedCausaPenal != null)
                {
                    LstAmparoDirecto = new ObservableCollection<AMPARO_DIRECTO>(new cAmparoDirecto().ObtenerTodos(SelectedCausaPenal.ID_CENTRO, SelectedCausaPenal.ID_ANIO, SelectedCausaPenal.ID_IMPUTADO, SelectedCausaPenal.ID_INGRESO, SelectedCausaPenal.ID_CAUSA_PENAL));
                    AmparoDirectoEmpty = LstAmparoDirecto.Count > 0 ? false : true;
                }
            }
            catch (Exception ex)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al establecer el listado de amparos directos", ex);
            }
        }


    

    }
}
