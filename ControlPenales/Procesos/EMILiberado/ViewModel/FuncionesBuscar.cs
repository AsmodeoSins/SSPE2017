using SSP.Controlador.Catalogo.Justicia;
using SSP.Servidor;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
namespace ControlPenales
{
    partial class EMILiberadoViewModel
    {
        private async void BuscarImputado()
        {
            try
            {
                //ListExpediente = (new cImputado()).ObtenerTodos(ApellidoPaternoBuscar, ApellidoMaternoBuscar, NombreBuscar,AnioBuscar, FolioBuscar);
                // ListExpediente = await StaticSourcesViewModel.CargarDatosAsync<ObservableCollection<IMPUTADO>>(() =>
                //new cImputado().ObtenerTodos(ApellidoPaternoBuscar, ApellidoMaternoBuscar, NombreBuscar, AnioBuscar, FolioBuscar));
                ListExpediente = new RangeEnabledObservableCollection<IMPUTADO>();
                ListExpediente.InsertRange(await SegmentarResultadoBusqueda());
                if (ListExpediente != null)
                {
                    if (ListExpediente.Count == 0)
                        EmptyExpedienteVisible = true;
                    else
                        emptyExpedienteVisible = false;
                }
            }
            catch (Exception ex)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al buscar imputado", ex);
            }
        }


        private async Task<List<IMPUTADO>> SegmentarResultadoBusqueda(int _Pag = 1)
        {
            var result = new ObservableCollection<IMPUTADO>();
            try
            {
                if (string.IsNullOrEmpty(ApellidoPaternoBuscar) && string.IsNullOrEmpty(ApellidoMaternoBuscar) && string.IsNullOrEmpty(NombreBuscar) && !AnioBuscar.HasValue && !FolioBuscar.HasValue)
                    return new List<IMPUTADO>();

                Pagina = _Pag;
                result = await StaticSourcesViewModel.CargarDatosAsync<ObservableCollection<IMPUTADO>>(() => new cImputado().ObtenerTodos(ApellidoPaternoBuscar, ApellidoMaternoBuscar, NombreBuscar, AnioBuscar, FolioBuscar, _Pag));
                if (result.Any())
                {
                    Pagina++;
                    SeguirCargando = true;
                }
                else
                    SeguirCargando = false;

            }
            catch (Exception ex)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al segmentar la búsqueda", ex);
            }
            return result.ToList();
        }

        private async Task<List<cLiberados>> SegmentarResultadoBusquedaLiberados(int _Pag = 1)
        {
            if (string.IsNullOrEmpty(ApellidoPaternoBuscar) && string.IsNullOrEmpty(ApellidoMaternoBuscar) && string.IsNullOrEmpty(NombreBuscar) && string.IsNullOrEmpty(NUCBuscar) && !AnioBuscar.HasValue && !FolioBuscar.HasValue)
                return new List<cLiberados>();

            Pagina = _Pag;
            var result = await StaticSourcesViewModel.CargarDatosAsync<IEnumerable<cLiberados>>(() => new cImputado().ObtenerLiberados(NUCBuscar,AnioBuscar,FolioBuscar, NombreBuscar, ApellidoPaternoBuscar, ApellidoMaternoBuscar, _Pag).Select(w => new cLiberados()
            {
                ID_CENTRO = w.ID_CENTRO,
                ID_ANIO = w.ID_ANIO,
                ID_IMPUTADO = w.ID_IMPUTADO,
                CENTRO = w.CENTRO,
                NOMBRE = string.Format("{0}{1}", w.NOMBRE, !string.IsNullOrEmpty(w.APODO_NOMBRE) ? "(" + w.APODO_NOMBRE + ")" : string.Empty),
                PATERNO = string.Format("{0}{1}", w.PATERNO, !string.IsNullOrEmpty(w.PATERNO_A) ? "(" + w.PATERNO_A + ")" : string.Empty),
                MATERNO = string.Format("{0}{1}", w.MATERNO, !string.IsNullOrEmpty(w.MATERNO_A) ? "(" + w.MATERNO_A + ")" : string.Empty)
            }));
            if (result.Any())
            {
                Pagina++;
                SeguirCargando = true;
            }
            else
                SeguirCargando = false;

            return result.ToList();
        }

        /// <summary>
        //Metodo que recive el evento click de una busqueda
        /// </summary>
        /// <param name="obj"></param>
        private async void ClickEnter(Object obj)
        {
            try
            {
                if (obj != null)
                {
                    //cuando es boton no se hace nada porque solamente existe el de buscar, si hay otro habra que castearlos a button y hacer la comparacion
                    var textbox = obj as System.Windows.Controls.TextBox;
                    if (textbox != null)
                    {
                        switch (textbox.Name)
                        {
                            case "NUCBuscar":
                                NUCBuscar = textbox.Text;
                                break;
                            case "NombreBuscar":
                                NombreBuscar = textbox.Text;
                                break;
                            case "ApellidoPaternoBuscar":
                                ApellidoPaternoBuscar = textbox.Text;
                                break;
                            case "ApellidoMaternoBuscar":
                                ApellidoMaternoBuscar = textbox.Text;
                                break;

                            case "AnioBuscar":
                                if (!string.IsNullOrEmpty(textbox.Text))
                                    AnioBuscar = short.Parse(textbox.Text);
                                else
                                    AnioBuscar = null;
                                break;
                            case "FolioBuscar":
                                if (!string.IsNullOrEmpty(textbox.Text))
                                    FolioBuscar = int.Parse(textbox.Text);
                                else
                                    FolioBuscar = null;
                                break;
                        }
                    }

                    #region nuevo
                    LstLiberados = new RangeEnabledObservableCollection<cLiberados>();
                    LstLiberados.InsertRange(await SegmentarResultadoBusquedaLiberados());
                    EmptyExpedienteVisible = LstLiberados.Count > 0 ? false : true;
                    PopUpsViewModels.ShowPopUp(this, PopUpsViewModels.TipoPopUp.BUSCAR_LIBERADO);
                    #endregion
                    //PopUpsViewModels.ShowPopUp(this, PopUpsViewModels.TipoPopUp.BUSCAR_LIBERADO);
                    //ListExpediente = new RangeEnabledObservableCollection<IMPUTADO>();
                    //ListExpediente.InsertRange(await SegmentarResultadoBusqueda());
                    //if (ListExpediente.Count > 0)//Empty row
                    //    EmptyExpedienteVisible = false;
                    //else
                    //    EmptyExpedienteVisible = true;
                }
            }
            catch (Exception ex)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error en el flujo del modulo", ex);
            }
        }
    }
}
