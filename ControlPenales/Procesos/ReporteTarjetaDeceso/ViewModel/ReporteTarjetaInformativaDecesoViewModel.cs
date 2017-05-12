using SSP.Controlador.Catalogo.Justicia;
using SSP.Servidor;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;

namespace ControlPenales
{
    public partial class ReporteTarjetaInformativaDecesoViewModel:ValidationViewModelBase
    {
        private void OnModelChangedSwitch(object parametro)
        {
            switch (parametro.ToString())
            {
                case "cambio_fecha_inicio_busqueda_deceso":
                    if (!FechaInicialBuscarDeceso.HasValue || !fechaFinalBuscarDeceso.HasValue || fechaFinalBuscarDeceso >= FechaInicialBuscarDeceso)
                        IsFechaIniBusquedaDecesoValida = true;
                    else
                        IsFechaIniBusquedaDecesoValida = false;
                    break;
            }
        }

        private async void ClickSwitch(object parametro)
        {
            if (parametro != null && !string.IsNullOrWhiteSpace(parametro.ToString()))
                switch (parametro.ToString())
                {
                    case "filtro_deceso":
                        if (!IsFechaIniBusquedaDecesoValida)
                        {
                            new Dialogos().ConfirmacionDialogo("Validación", "La fecha de inicio tiene que ser menor a la fecha fin!");
                            return;
                        }
                        await StaticSourcesViewModel.CargarDatosMetodoAsync(() =>
                        {
                            BuscarDefuncion(anioBuscarDeceso,folioBuscarDeceso,nombreBuscarDeceso,apellidoPaternoBuscarDeceso,apellidoMaternoBuscarDeceso,
                                fechaInicialBuscarDeceso,fechaFinalBuscarDeceso,true);
                        });
                        break;
                    case "seleccionar_deceso":
                        if (selectedDecesoBusqueda == null)
                        {
                            new Dialogos().ConfirmacionDialogo("Validación", "Debe de seleccionar un registro de deceso");
                            return;
                        }

                        var tc = new TextControlView();
                        tc.Closed += (s, e) =>
                        {
                            PopUpsViewModels.ClosePopUp(PopUpsViewModels.TipoPopUp.OSCURECER_FONDO);
                        };
                        tc.editor.Loaded += (s, e) =>
                        {

                            //DOCX
                            tc.editor.EditMode = TXTextControl.EditMode.ReadOnly;
                            TXTextControl.LoadSettings _settings = new TXTextControl.LoadSettings();
                            tc.editor.Load(selectedDecesoBusqueda.TARJETA_DECESO, TXTextControl.BinaryStreamType.WordprocessingML, _settings);
                        };
                        PopUpsViewModels.ShowPopUp(this, PopUpsViewModels.TipoPopUp.OSCURECER_FONDO);
                        tc.Owner = PopUpsViewModels.MainWindow;
                        tc.Show();

                        break;
                }
        }

        #region Buscar
        private void BuscarDefuncion(short? anio_imputado = null, int? folio_imputado = null, string nombre = "", string paterno = "", string materno = "",
            DateTime? fecha_inicio = null, DateTime? fecha_final = null, bool isExceptionManaged = false)
        {
            try
            {
                listaDecesoBusqueda = new ObservableCollection<NOTA_DEFUNCION>(new cNota_Defuncion().Buscar(GlobalVar.gCentro,anio_imputado, folio_imputado, paterno, materno,
                    nombre, fecha_inicio,fecha_final));
                RaisePropertyChanged("ListaDecesoBusqueda");
            }
            catch (Exception ex)
            {
                if (isExceptionManaged)
                    throw ex;
                else
                    StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al buscar las solicitudes de interconsulta", ex);
            }
        }


        #endregion
    }
}
