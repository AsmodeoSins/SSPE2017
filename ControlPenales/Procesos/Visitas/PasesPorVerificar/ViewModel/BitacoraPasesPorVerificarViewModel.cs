using ControlPenales.Clases;
using MahApps.Metro.Controls;
using SSP.Controlador.Catalogo.Justicia;
using SSP.Servidor;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace ControlPenales
{
    partial class BitacoraPasesPorVerificarViewModel : FingerPrintScanner
    {
        public BitacoraPasesPorVerificarViewModel() { }

        private async void clickSwitch(Object obj)
        {
            switch (obj.ToString())
            {
                case "guardar_menu":
                    try
                    {
                        if (ListPases.Any(a => a.AUTORIZA == null))
                        {
                            (new Dialogos()).ConfirmacionDialogo("Advertencia!", "Tienes pases sin AUTORIZAR/CANCELAR.");
                            break;
                        }
                        var lista = ListPases.Where(w => w.OBJETO_VISITANTE_INGRESO_PASE != null).Select(s => new VISITANTE_INGRESO_PASE
                            {
                                AUTORIZADO = s.AUTORIZA.HasValue ? s.AUTORIZA.Value ? "S" : "N" : null,
                                FECHA_ALTA = s.OBJETO_VISITANTE_INGRESO_PASE.FECHA_ALTA,
                                ID_ANIO = s.OBJETO_VISITANTE_INGRESO_PASE.ID_ANIO,
                                ID_CENTRO = s.OBJETO_VISITANTE_INGRESO_PASE.ID_CENTRO,
                                ID_CONSEC = s.OBJETO_VISITANTE_INGRESO_PASE.ID_CONSEC,
                                ID_IMPUTADO = s.OBJETO_VISITANTE_INGRESO_PASE.ID_IMPUTADO,
                                ID_INGRESO = s.OBJETO_VISITANTE_INGRESO_PASE.ID_INGRESO,
                                ID_PASE = s.OBJETO_VISITANTE_INGRESO_PASE.ID_PASE,
                                ID_PERSONA = s.OBJETO_VISITANTE_INGRESO_PASE.ID_PERSONA
                            }).ToList();
                        if (new cPasesVisitanteIngreso().ActualizarLista(lista))
                        {
                            var listaCancelados = lista.Where(w => w.AUTORIZADO == "N").Select(s => new VISITANTE_INGRESO
                            {
                                EMISION_GAFETE = s.VISITANTE_INGRESO.EMISION_GAFETE,
                                ESTATUS_MOTIVO = s.VISITANTE_INGRESO.ESTATUS_MOTIVO,
                                FEC_ALTA = s.VISITANTE_INGRESO.FEC_ALTA,
                                FEC_ULTIMA_MOD = Fechas.GetFechaDateServer,
                                ID_ANIO = s.VISITANTE_INGRESO.ID_ANIO,
                                ID_CENTRO = s.VISITANTE_INGRESO.ID_CENTRO,
                                ID_ESTATUS_VISITA = Parametro.ID_ESTATUS_VISITA_CANCELADO,
                                ID_IMPUTADO = s.VISITANTE_INGRESO.ID_IMPUTADO,
                                ID_INGRESO = s.VISITANTE_INGRESO.ID_INGRESO,
                                ID_PERSONA = s.VISITANTE_INGRESO.ID_PERSONA,
                                ID_TIPO_REFERENCIA = s.VISITANTE_INGRESO.ID_TIPO_REFERENCIA,
                                OBSERVACION = s.VISITANTE_INGRESO.OBSERVACION
                            });
                            if (!new cPasesVisitanteIngreso().ActualizarLista(lista))
                            {
                                (new Dialogos()).ConfirmacionDialogo("Advertencia!", "Ocurrió un error al guardar!");
                                break;
                            }

                            (new Dialogos()).ConfirmacionDialogo("Exito!", "GUARDADO CON EXITO!");
                            await StaticSourcesViewModel.CargarDatosMetodoAsync(GetListas);
                        }
                    }
                    catch (Exception ex)
                    {
                        StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al guardar.", ex);
                    }
                    break;
                case "limpiar_menu":
                    ((System.Windows.Controls.ContentControl)PopUpsViewModels.MainWindow.FindName("contentControl")).Content = new BitacoraPasesPorVerificar();
                    ((System.Windows.Controls.ContentControl)PopUpsViewModels.MainWindow.FindName("contentControl")).DataContext = new BitacoraPasesPorVerificarViewModel();
                    break;
                case "filtrar":
                    FiltrarBusquedaPases();
                    break;
                case "salir_menu":
                    PrincipalViewModel.SalirMenu();
                    break;
            }
        }

        private async void FiltrarBusquedaPases()
        {
            try
            {
                var filtro = TextNombreFiltro;
                ListPases = ListPasesTotales = new RangeEnabledObservableCollection<VERIFICACION_PASES>();
                ListPasesTotales.InsertRange(await SegmentarResultadoBusqueda());
                TextNombreFiltro = filtro;
            }
            catch (Exception ex)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al filtrar la lista de pases.", ex);
            }
        }

        private async Task<List<VERIFICACION_PASES>> SegmentarResultadoBusqueda(int _Pag = 1)
        {
            try
            {
                Pagina = _Pag;
                var result = await StaticSourcesViewModel.CargarDatosAsync<ObservableCollection<VERIFICACION_PASES>>(() => new ObservableCollection<VERIFICACION_PASES>((
                    new cPasesVisitanteIngreso().ObtenerPasesXAutorizarXCentroDocumentoYFecha(SelectTipoPase, GlobalVar.gCentro, FechaFiltroInicial, FechaFiltroFinal, SelectAutorizaFiltro, Pagina)).Select(s => new VERIFICACION_PASES
                    {
                        NOMBRE_VISITANTE = s.VISITANTE_INGRESO.VISITANTE.PERSONA.PATERNO.Trim() + " " +
                        (string.IsNullOrEmpty(s.VISITANTE_INGRESO.VISITANTE.PERSONA.MATERNO) ? string.Empty : s.VISITANTE_INGRESO.VISITANTE.PERSONA.MATERNO.Trim()) + " " +
                        s.VISITANTE_INGRESO.VISITANTE.PERSONA.NOMBRE.Trim(),
                        NOMBRE_IMPUTADO = s.VISITANTE_INGRESO.INGRESO.IMPUTADO.PATERNO.Trim() + " " +
                        (string.IsNullOrEmpty(s.VISITANTE_INGRESO.INGRESO.IMPUTADO.MATERNO) ? string.Empty : s.VISITANTE_INGRESO.INGRESO.IMPUTADO.MATERNO.Trim()) + " " +
                        s.VISITANTE_INGRESO.INGRESO.IMPUTADO.NOMBRE.Trim(),
                        CENTRO = s.VISITANTE_INGRESO.INGRESO.CENTRO.DESCR.Trim(),
                        EDAD = (short)new Fechas().CalculaEdad(s.VISITANTE_INGRESO.VISITANTE.PERSONA.FEC_NACIMIENTO),
                        PARENTESCO = s.VISITANTE_INGRESO.TIPO_REFERENCIA.DESCR,
                        FECHA = s.FECHA_ALTA,
                        ID_ANIO = s.ID_ANIO,
                        ID_CENTRO = s.ID_CENTRO,
                        ID_IMPUTADO = s.ID_IMPUTADO,
                        ID_INGRESO = s.ID_INGRESO,
                        ID_PERSONA = s.ID_PERSONA,
                        ID_TIPO_PASE = s.ID_PASE.Value,
                        TIPO_PASE = s.PASE_CATALOGO.DESCR,
                        OBJETO_VISITANTE_INGRESO_PASE = s,
                        AUTORIZA = s.AUTORIZADO == null ? new Nullable<bool>() : s.AUTORIZADO == "S"
                    })));
                if (result.Any())
                {
                    Pagina++;
                    SeguirCargando = true;
                }
                else
                    SeguirCargando = false;
                ListPases = new RangeEnabledObservableCollection<VERIFICACION_PASES>();
                ListPases.InsertRange(result);
                return result.ToList();
            }
            catch (Exception ex)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al realizar la busqueda de pases.", ex);
                return new List<VERIFICACION_PASES>();
            }
        }

        private async void Load_Window(BitacoraPasesPorVerificar Window)
        {
            await StaticSourcesViewModel.CargarDatosMetodoAsync(GetListas);
        }

        private void Filtrar(Object obj)
        {
            if (obj != null)
            {
                if (obj is TextBox)
                {
                    var textbox = (TextBox)obj;
                    TextNombreFiltro = textbox.Text;
                }
                FiltrarBusquedaPases();
            }
        }

        private void GetListas()
        {
            try
            {
                ListPases = ListPasesTotales = ListPasesTotales ?? new RangeEnabledObservableCollection<VERIFICACION_PASES>();
                Application.Current.Dispatcher.Invoke((Action)(async delegate { ListPasesTotales.InsertRange(await SegmentarResultadoBusqueda()); }));

                ListTipoPase = ListTipoPase ?? new ObservableCollection<PASE_CATALOGO>((new cPases()).ObtenerTodos());
                Application.Current.Dispatcher.Invoke((Action)(delegate
                {
                    ListTipoPase.Insert(0, new PASE_CATALOGO() { ID_PASE = 0, DESCR = "TODOS" });
                    SelectTipoPase = 0;
                }));
            }
            catch (Exception ex)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al cargar la lista.", ex);
            }
        }
    }
    public class VERIFICACION_PASES
    {
        public string NOMBRE_IMPUTADO { get; set; }
        public string NOMBRE_VISITANTE { get; set; }
        public string CENTRO { get; set; }
        public DateTime? FECHA { get; set; }
        public string TIPO_PASE { get; set; }
        public string PARENTESCO { get; set; }
        public short EDAD { get; set; }
        public short ID_TIPO_PASE { get; set; }
        public short ID_ANIO { get; set; }
        public short ID_CENTRO { get; set; }
        public short ID_INGRESO { get; set; }
        public int ID_IMPUTADO { get; set; }
        public int ID_PERSONA { get; set; }
        public bool? AUTORIZA { get; set; }
        public VISITANTE_INGRESO_PASE OBJETO_VISITANTE_INGRESO_PASE { get; set; }
    }
}
