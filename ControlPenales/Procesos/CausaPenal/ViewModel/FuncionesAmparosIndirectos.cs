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
        private void PopulateAmparoIndirecto() {
            try
            {
                if (SelectedAmparoIndirecto != null)
                {
                    FecDocumentoAI = SelectedAmparoIndirecto.DOCUMENTO_FEC;
                    FecNotificacionAI = SelectedAmparoIndirecto.NOTIFICACION_FEC;
                    FecSuspencionAccionAI = SelectedAmparoIndirecto.SUSPENSION_FEC;
                    NoOficioAI = SelectedAmparoIndirecto.OFICIO_NUM;
                    NoAI = SelectedAmparoIndirecto.AMPARO_NUM;
                    SelectedJuzgadoAI = SelectedAmparoIndirecto.JUZGADO.ID_JUZGADO;
                    FecSentenciaAI = SelectedAmparoIndirecto.SENTENCIA_FEC;
                    //SelectedSentenciaAI = SelectedAmparoIndirecto.ID_SEN_AMP_RESULTADO;
                    FecEjecutoriaAI = SelectedAmparoIndirecto.RESOLUCION_EJECUTORIA_FEC;
                    FecRevisionSentenciaAI = SelectedAmparoIndirecto.REVISION_RECURSO_FEC;
                }
            }
            catch (Exception ex)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al establecer amparo indirecto", ex);
            }
        }

        private void PopulateAmparoIndirectoListado()
        {
            //try
            //{
            //    if (SelectedIngreso != null)
            //    {
            //        //LstAmparoIndirecto = new ObservableCollection<AMPARO_INDIRECTO>(new cAmparoIndirecto().ObtenerTodos(SelectedIngreso.ID_CENTRO, SelectedIngreso.ID_ANIO, SelectedIngreso.ID_IMPUTADO, SelectedIngreso.ID_INGRESO));
            //        AmparoIndirectoEmpty = LstAmparoIndirecto.Count > 0 ? false : true;
            //    }
            //}
            //catch (Exception ex)
            //{
            //    StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al establecer el listado de amparos indirectos", ex);
            //}
        }


        
       
    }
}
