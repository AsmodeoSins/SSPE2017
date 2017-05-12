using ControlPenales;

using System;
using System.Threading.Tasks;
using System.Windows;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Windows.Input;
using MahApps.Metro.Controls;
using MahApps.Metro.Controls.Dialogs;
using SSP.Servidor;
using SSP.Controlador.Catalogo.Justicia;
using ControlPenales.Clases;
using System.Windows.Forms;
using System.Windows.Controls;

namespace ControlPenales
{
    partial class RegistroDecomisoViewModel
    {

        #region Visita Externa
        private void LimpiarExterno()
        {
            NombreEx = PaternoEx = MaternoEx = NipEx = string.Empty;
            LstExternoPop = null;// new ObservableCollection<PERSONA_EXTERNO>();
            SelectedExternoPop = null;
            ExternoEmpty = false;
            ImagenExternoPop = new Imagenes().getImagenPerson();
        }
        
        //private void PopulateBuscarExterno() 
        //{
        //    try
        //    {
        //        System.Windows.Application.Current.Dispatcher.Invoke((Action)(delegate
        //        {
        //            int? x = !string.IsNullOrEmpty(NipEx) ? int.Parse(NipEx) : 0;
        //            LstExternoPop = new ObservableCollection<PERSONA_EXTERNO>(new cPersonaExterna().ObtenerTodos(/*4*/GlobalVar.gCentro, x > 0 ? x : null, PaternoEx, MaternoEx, NombreEx));
        //            ExternoEmpty = LstExternoPop.Count > 0 ? false : true;
        //        }));
        //    }
        //    catch (Exception ex)
        //    {
        //        StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al buscar visita externa", ex);
        //    }
        //}
        private async void PopulateBuscarExterno()
        {
            try
            {
                PaginaExterno = 1;
                SeguirCargandoExterno = true;
                SelectedExternoPop = null;
                LstExternoPop = new RangeEnabledObservableCollection<PERSONA_EXTERNO>();
                LstExternoPop.InsertRange(await SegmentarExternoBusqueda());
                ExternoEmpty = LstExternoPop.Count > 0 ? false : true;
            }
            catch (Exception ex)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al buscar visitante", ex);
            }
        }
 

        private async Task<List<PERSONA_EXTERNO>> SegmentarExternoBusqueda(int _Pag = 1)
        {
            try
            {
                if (string.IsNullOrEmpty(PaternoEx) && string.IsNullOrEmpty(MaternoEx) && string.IsNullOrEmpty(NombreEx) && string.IsNullOrEmpty(NipEx))
                    return new List<PERSONA_EXTERNO>();
                PaginaExterno = _Pag;
                var result = await StaticSourcesViewModel.CargarDatosAsync<ObservableCollection<PERSONA_EXTERNO>>(() =>
                        new ObservableCollection<PERSONA_EXTERNO>(new cPersonaExterna().ObtenerTodos(GlobalVar.gCentro,!string.IsNullOrEmpty(NipEx) ? (int?)int.Parse(NipEx) : null, PaternoEx, MaternoEx, NombreEx,_Pag)));
                     
                if (result.Any())
                {
                    PaginaExterno++;
                    SeguirCargandoExterno = true;
                }
                else
                    SeguirCargandoExterno = false;
                return result.ToList();
            }
            catch (Exception ex)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al consultar.", ex);
                return new List<PERSONA_EXTERNO>();
            }
        }

        private void AgregarExterno() 
        {
            if (SelectedExternoPop != null)
            {
                if (LstExterno == null)
                    LstExterno = new ObservableCollection<DECOMISO_PERSONA>();
                LstExterno.Add(new DECOMISO_PERSONA() {
                    ID_PERSONA = SelectedExternoPop.ID_PERSONA,
                    ID_TIPO_PERSONA = 4, //EXTERNO
                    PERSONA = SelectedExternoPop.PERSONA
                });
            }
        }

        private async void EliminarExterno() {
            if (SelectedProveedoresInvolucrados != null)
            {
                if (await new Dialogos().ConfirmarEliminar("Confirmacion!", "¿Desea eliminar el visitante externo seleccionado?") == 1)
                { 
                    LstProveedoresInvolucrados.Remove(SelectedProveedoresInvolucrados);
                    ProveedorInvolucradoVisible = LstProveedoresInvolucrados.Any() ? Visibility.Collapsed : Visibility.Visible;
                }
            }
            else
                new Dialogos().ConfirmacionDialogo("Notificacion!", "Favor de seleccionar un visitante externo");
        }
        #endregion

        #region Metodos
        private void ClickExternoEnter(Object obj)
        {
            if (obj != null)
            {
                var textbox = (System.Windows.Controls.TextBox)obj;
                switch (textbox.Name)
                {
                    case "NIPExterno":
                        NipEx = textbox.Text;
                        break;
                    case "PaternoExterno":
                        PaternoEx = textbox.Text;
                        break;
                    case "MaternoExterno":
                        MaternoEx = textbox.Text;
                        break;
                    case "NombreExterno":
                        NombreEx = textbox.Text;
                        break;
                }
                PopulateBuscarExterno();
            }
        }
        #endregion

    }
}
