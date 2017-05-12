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
        #region Visita
        private void LimpiarVisitante()
        {
            NoV = null;
            NombreV = PaternoV = MaternoV = string.Empty;
            LstVisitantePop = null;//new ObservableCollection<VISITANTE>();
            SelectedVisitantePop = null;
            VisitanteEmpty = true;
            ImagenVisitantePop = new Imagenes().getImagenPerson();
        }
        
        //private void PopulateBuscarVisita() 
        //{
        //    try
        //    {
        //        System.Windows.Application.Current.Dispatcher.Invoke((Action)(delegate
        //        {
        //            LstVisitantePop = new ObservableCollection<VISITANTE>(new cVisitante().ObtenerTodos(NoV, NombreV, PaternoV, MaternoV));
        //            VisitanteEmpty = LstVisitantePop.Count > 0 ? false : true;
        //        }));
        //    }
        //    catch (Exception ex)
        //    {
        //        StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al buscar visitante", ex);
        //    }
        //}

        private async void PopulateBuscarVisita()
        {
            try
            {
                PaginaVisitante = 1;
                SeguirCargandoVisitante = true;
                SelectedVisitantePop = null;
                LstVisitantePop = new RangeEnabledObservableCollection<VISITANTE>();
                LstVisitantePop.InsertRange(await SegmentarVisitanteBusqueda());
                VisitanteEmpty = LstVisitantePop.Count > 0 ? false : true;
            }
            catch (Exception ex)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al buscar visitante", ex);
            }
        }
 

        private async Task<List<VISITANTE>> SegmentarVisitanteBusqueda(int _Pag = 1)
        {
            try
            {
                if (string.IsNullOrEmpty(PaternoV) && string.IsNullOrEmpty(MaternoV) && string.IsNullOrEmpty(NombreV) && !NoV.HasValue)
                    return new List<VISITANTE>();
                PaginaVisitante = _Pag;
                var result = await StaticSourcesViewModel.CargarDatosAsync<ObservableCollection<VISITANTE>>(() => 
                     new ObservableCollection<VISITANTE>(new cVisitante().ObtenerTodos(NoV, NombreV, PaternoV, MaternoV,_Pag)));
                if (result.Any())
                {
                    PaginaVisitante++;
                    SeguirCargandoVisitante = true;
                }
                else
                    SeguirCargandoVisitante = false;
                return result.ToList();
            }
            catch (Exception ex)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al consultar.", ex);
                return new List<VISITANTE>();
            }
        }





        private void AgregarVisitante() 
        {
            if (SelectedVisitantePop != null)
            {
                if (LstVisitante == null)
                    LstVisitante = new ObservableCollection<DECOMISO_PERSONA>();
                LstVisitante.Add(new DECOMISO_PERSONA() {
                    ID_PERSONA = SelectedVisitantePop.ID_PERSONA,
                    ID_TIPO_PERSONA = 3,//visita
                    PERSONA = SelectedVisitantePop.PERSONA
                });
                VisitaInvolucradoVisible = LstVisitaInvolucrada.Any() ? Visibility.Collapsed : Visibility.Visible;
            }
        }

        private async void EliminarVisitante() {
            if (SelectedVisitaInvolucrada != null)
            {
                if (await new Dialogos().ConfirmarEliminar("Confirmacion!", "¿Desea eliminar el visitante seleccionado?") == 1)
                {
                    LstVisitaInvolucrada.Remove(SelectedVisitaInvolucrada);
                    VisitaInvolucradoVisible = LstVisitaInvolucrada.Any() ? Visibility.Collapsed : Visibility.Visible;
                }
            }
            else
                new Dialogos().ConfirmacionDialogo("Notificacion!", "Favor de seleccionar un visitante");
        }
        #endregion

        #region Abogado
        private void AgregarAbogado()
        {
            if (SelectedVisitantePop != null)
            {
                if (LstAbogadoInvolucrada == null)
                    LstAbogadoInvolucrada = new ObservableCollection<DECOMISO_PERSONA>();
                LstAbogadoInvolucrada.Add(new DECOMISO_PERSONA()
                {
                    ID_PERSONA = SelectedVisitantePop.ID_PERSONA,
                    ID_TIPO_PERSONA = 3,//visita
                    PERSONA = SelectedVisitantePop.PERSONA
                });
                AbogadoInvolucradoVisible = LstAbogadoInvolucrada.Any() ? Visibility.Collapsed : Visibility.Visible;
            }
        }

        private async void EliminarAbogado()
        {
            if (SelectedAbogadoInvolucrada != null)
            {
                if (await new Dialogos().ConfirmarEliminar("Confirmacion!", "¿Desea eliminar el abogado seleccionado?") == 1)
                {
                    LstAbogadoInvolucrada.Remove(SelectedAbogadoInvolucrada);
                    AbogadoInvolucradoVisible = LstAbogadoInvolucrada.Any() ? Visibility.Collapsed : Visibility.Visible;
                }
            }
            else
                new Dialogos().ConfirmacionDialogo("Notificacion!", "Favor de seleccionar un abogado");
        }
        #endregion
    }
}
