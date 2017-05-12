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
using System.Windows.Controls;
using SSP.Servidor;
using SSP.Controlador.Catalogo.Justicia;
using ControlPenales;
using System.Globalization;
using System.IO;
using Novacode;
using ControlPenales.BiometricoServiceReference;

namespace ControlPenales
{
    partial class EntrevistaInicialViewModel : ValidationViewModelBase, IPageViewModel
    {
        #region constructor
        public EntrevistaInicialViewModel() { }
        #endregion

        #region Generales
        void IPageViewModel.inicializa()
        { }

        private void clickSwitch(Object obj)
        {
            switch (obj.ToString())
            {
                case "nueva_busqueda":
                    LimpiarBusqueda();
                    break;

                case "buscar_seleccionar":
                    if (SelectIngreso == null)
                    {
                        new Dialogos().ConfirmacionDialogo("Validacion!", "Favor de seleccionar un ingreso");
                        return;
                    }
                    var EstatusInactivos = Parametro.ESTATUS_ADMINISTRATIVO_INACT;
                    foreach (var item in EstatusInactivos)
                    {
                        if (SelectIngreso.ID_ESTATUS_ADMINISTRATIVO == item)
                        {
                            new Dialogos().ConfirmacionDialogo("Notificación!", "El ingreso seleccionado no esta activo.");
                            return;
                        }
                    }
                    if (SelectIngreso.ID_UB_CENTRO != GlobalVar.gCentro)
                    {
                        new Dialogos().ConfirmacionDialogo("Validación", "El ingreso seleccionado no pertenece a su centro.");
                        return;
                    }
                    var toleranciaTraslado = Parametro.TOLERANCIA_TRASLADO_EDIFICIO;
                    if (SelectIngreso.TRASLADO_DETALLE.Any(a => (a.ID_ESTATUS != "CA" ? a.TRASLADO.ORIGEN_TIPO != "F" : false) && a.TRASLADO.TRASLADO_FEC.AddHours(-toleranciaTraslado) <= Fechas.GetFechaDateServer))
                    {
                        new Dialogos().ConfirmacionDialogo("Notificación!", "El interno [" + SelectIngreso.ID_ANIO.ToString() + "/" +
                            SelectIngreso.ID_IMPUTADO.ToString() + "] tiene un traslado proximo y no puede imprimirse el documento.");
                        return;
                    }
                    SelectedIngreso = SelectIngreso;
                    ReportViewerVisible = Visibility.Visible;
                    PopUpsViewModels.ClosePopUp(PopUpsViewModels.TipoPopUp.BUSQUEDA);
                    Imprimir();
                    break;
                case "buscar_salir":
                    if (SelectedIngreso != null)
                    {
                        SelectIngreso = SelectedIngreso;
                    }
                    else
                        ImagenIngreso = new Imagenes().getImagenPerson();
                    ReportViewerVisible = Visibility.Visible;
                    PopUpsViewModels.ClosePopUp(PopUpsViewModels.TipoPopUp.BUSQUEDA);
                    break;

                case "buscar_interno":
                    FolioBuscar = null;
                    AnioBuscar = null;
                    NombreBuscar = ApellidoMaternoBuscar = ApellidoPaternoBuscar = string.Empty;
                    ListExpediente = null;
                    SelectExpediente = null;
                    ImagenIngreso = new Imagenes().getImagenPerson();
                    ReportViewerVisible = Visibility.Collapsed;
                    PopUpsViewModels.ShowPopUp(this, PopUpsViewModels.TipoPopUp.BUSQUEDA);
                    break;

                case "reporte_menu"://"imprimir":
                    if (SelectIngreso != null)
                        //Reporte.PrintDialog();
                        ImprimirAsesoriaJuridica();
                    else
                        new Dialogos().ConfirmacionDialogo("VALIDACION!", "Favor de seleccionar un ingreso");
                    break;
                case "limpiar_menu":
                    ((System.Windows.Controls.ContentControl)PopUpsViewModels.MainWindow.FindName("contentControl")).Content = new EntrevistaInicialView();
                    ((System.Windows.Controls.ContentControl)PopUpsViewModels.MainWindow.FindName("contentControl")).DataContext = new EntrevistaInicialViewModel();
                    break;
                case "salir_menu":
                    PrincipalViewModel.SalirMenu();
                    break;
            }
        }

        private async void ClickEnter(Object obj)
        {
            try
            {
                if (obj != null)
                {
                    //cuando es boton no se hace nada porque solamente existe el de buscar, si hay otro habra que castearlos a button y hacer la comparacion
                    var textbox = obj as TextBox;
                    if (textbox != null)
                    {
                        switch (textbox.Name)
                        {
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
                                    AnioBuscar = int.Parse(textbox.Text);
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
                }
                //TabVisible = false;
                ListExpediente = new RangeEnabledObservableCollection<IMPUTADO>();
                ListExpediente.InsertRange(await SegmentarResultadoBusqueda());
                if (ListExpediente.Count > 0)//Empty row
                    EmptyExpedienteVisible = false;
                else
                    EmptyExpedienteVisible = true;
                ImagenImputado = ImagenIngreso = new Imagenes().getImagenPerson();
            }
            catch (Exception ex)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al ingresar búsqueda", ex);
            }
        }

        private async void ModelEnter(Object obj)
        {
            try
            {
                //if (obj != null)
                //{
                //    if (!obj.GetType().Name.Equals("String"))
                //    {

                //        var textbox = (TextBox)obj;
                //        switch (textbox.Name)
                //        {
                //            case "NombreBuscar":
                //                NombreBuscar = textbox.Text;
                //                NombreD = NombreBuscar;
                //                FolioBuscar = FolioD;
                //                AnioBuscar = AnioD;
                //                break;
                //            case "ApellidoPaternoBuscar":
                //                ApellidoPaternoBuscar = textbox.Text;
                //                PaternoD = ApellidoPaternoBuscar;
                //                FolioBuscar = FolioD;
                //                AnioBuscar = AnioD;
                //                break;
                //            case "ApellidoMaternoBuscar":
                //                ApellidoMaternoBuscar = textbox.Text;
                //                MaternoD = ApellidoMaternoBuscar;
                //                FolioBuscar = FolioD;
                //                AnioBuscar = AnioD;
                //                break;
                //            case "FolioBuscar":
                //                if (!string.IsNullOrEmpty(textbox.Text))
                //                    FolioBuscar = int.Parse(textbox.Text);
                //                else
                //                    FolioBuscar = null;
                //                AnioBuscar = AnioD;
                //                break;
                //            case "AnioBuscar":
                //                if (!string.IsNullOrEmpty(textbox.Text))
                //                    AnioBuscar = int.Parse(textbox.Text);
                //                else
                //                    AnioBuscar = null;
                //                FolioBuscar = FolioD;
                //                break;
                //        }
                //    }
                //}
                //ListExpediente = new RangeEnabledObservableCollection<IMPUTADO>();

                //if (string.IsNullOrEmpty(NombreD))
                //    NombreBuscar = string.Empty;
                //else
                //    NombreBuscar = NombreD;

                //if (string.IsNullOrEmpty(PaternoD))
                //    ApellidoPaternoBuscar = string.Empty;
                //else
                //    ApellidoPaternoBuscar = PaternoD;

                //if (string.IsNullOrEmpty(MaternoD))
                //    ApellidoMaternoBuscar = string.Empty;
                //else
                //    ApellidoMaternoBuscar = MaternoD;

                //if (AnioBuscar != null && FolioBuscar != null)
                //{
                //    ListExpediente = new RangeEnabledObservableCollection<IMPUTADO>();
                //    ListExpediente.InsertRange(await SegmentarResultadoBusqueda());
                //    if (ListExpediente.Count == 1)
                //    {
                //        if (ListExpediente[0].INGRESO.Count > 0)
                //        {
                //            foreach (var item in ListExpediente[0].INGRESO)
                //            {
                //                if (item.ID_ESTATUS_ADMINISTRATIVO != Parametro.ID_ESTATUS_ADMVO_LIBERADO)
                //                {
                //                    SelectExpediente = ListExpediente[0];
                //                    SelectIngreso = item;
                //                    //TabVisible = true;
                //                    //this.SeleccionaIngreso();
                //                    //this.ViewModelArbol();
                //                    //EdificioI = SelectIngreso.ID_UB_EDIFICIO;
                //                    ObtenerIngreso();
                //                    PopUpsViewModels.ClosePopUp(PopUpsViewModels.TipoPopUp.BUSQUEDA);
                //                    break;
                //                }
                //                else
                //                {
                //                    SelectExpediente = null;
                //                    SelectIngreso = null;
                //                    ImagenImputado = ImagenIngreso = new Imagenes().getImagenPerson();
                //                    PopUpsViewModels.ShowPopUp(this, PopUpsViewModels.TipoPopUp.BUSQUEDA);
                //                    //TabVisible = false;
                //                }
                //            }
                //        }
                //        else
                //        {
                //            SelectExpediente = null;
                //            SelectIngreso = null;
                //            ImagenImputado = ImagenIngreso = new Imagenes().getImagenPerson();
                //            PopUpsViewModels.ShowPopUp(this, PopUpsViewModels.TipoPopUp.BUSQUEDA);
                //            //TabVisible = false;
                //        }
                //    }
                //    else
                //    {
                //        ImagenImputado = ImagenIngreso = new Imagenes().getImagenPerson();
                //        PopUpsViewModels.ShowPopUp(this, PopUpsViewModels.TipoPopUp.BUSQUEDA);
                //        //TabVisible = false;
                //    }
                //}
                //else
                //{
                //    //ListExpediente = (new cImputado()).ObtenerTodos(ApellidoPaternoBuscar, ApellidoMaternoBuscar, NombreBuscar, AnioBuscar,FolioBuscar);
                //    ListExpediente = new RangeEnabledObservableCollection<IMPUTADO>();
                //    ListExpediente.InsertRange(await SegmentarResultadoBusqueda());
                //    if (ListExpediente.Count > 0)//Empty row
                //        EmptyExpedienteVisible = false;
                //    else
                //        EmptyExpedienteVisible = true;
                //    ImagenImputado = ImagenIngreso = new Imagenes().getImagenPerson();
                //    PopUpsViewModels.ShowPopUp(this, PopUpsViewModels.TipoPopUp.BUSQUEDA);
                //    //TabVisible = false;
                //}
            }
            catch (Exception ex)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al ingresar búsqueda", ex);
            }
        }

        private async Task<List<IMPUTADO>> SegmentarResultadoBusqueda(int _Pag = 1)
        {
            try
            {
                if (string.IsNullOrEmpty(ApellidoPaternoBuscar) && string.IsNullOrEmpty(ApellidoMaternoBuscar) && string.IsNullOrEmpty(NombreBuscar) && !AnioBuscar.HasValue && !FolioBuscar.HasValue)
                    return new List<IMPUTADO>();

                Pagina = _Pag;
                var result = await StaticSourcesViewModel.CargarDatosAsync<ObservableCollection<IMPUTADO>>(() => new cImputado().ObtenerTodos(ApellidoPaternoBuscar, ApellidoMaternoBuscar, NombreBuscar, AnioBuscar, FolioBuscar, _Pag));
                if (result.Any())
                {
                    Pagina++;
                    SeguirCargando = true;
                }
                else
                    SeguirCargando = false;

                return result.ToList();
            }
            catch (Exception ex)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al segmentar resultados de búsqueda", ex);
                return new List<IMPUTADO>();
            }
        }

        private void OnLoad(EntrevistaInicialView Window = null)
        {
            try
            {
                ReporteHeight = (Window.ActualHeight / 1.5);
                Reporte = Window.Report;
                ReportViewerVisible = Visibility.Hidden;
                PopUpsViewModels.ShowPopUp(this, PopUpsViewModels.TipoPopUp.BUSQUEDA);
            }
            catch (Exception ex)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al cargar la entrevista inicial", ex);
            }
        }

        #endregion

        #region Metodos Buscar
        private void LimpiarBusqueda()
        {
            AnioBuscar = FolioBuscar = null;
            NombreBuscar = ApellidoPaternoBuscar = ApellidoMaternoBuscar = string.Empty;
            ListExpediente = null;
            SelectExpediente = null;
            ImagenIngreso = new Imagenes().getImagenPerson();
        }

        private void ObtenerIngreso()
        {

        }
        #endregion

        #region Metodos Reporte
        private void Imprimir()
        {
            try
            {
                if (SelectIngreso != null)
                {
                    var emi = new EMI_INGRESO();
                    var datos = new List<cEntrevistaInicialTS>();
                    var familia = new List<cEstructuraFamiliar>();
                    var alias = string.Empty;
                    var lista = SelectIngreso.EMI_INGRESO;

                    if (SelectIngreso.IMPUTADO.ALIAS != null)
                    {
                        foreach (var a in SelectIngreso.IMPUTADO.ALIAS)
                        {
                            if (!string.IsNullOrEmpty(alias))
                                alias = string.Format("{0},", alias);
                            alias = alias + string.Format("{0} {1} {2}", !string.IsNullOrEmpty(a.PATERNO) ? a.PATERNO.Trim() : string.Empty, !string.IsNullOrEmpty(a.MATERNO) ? a.MATERNO.Trim() : string.Empty, !string.IsNullOrEmpty(a.NOMBRE) ? a.NOMBRE.Trim() : string.Empty);
                        }
                    }

                    if (lista != null)
                        emi = lista.FirstOrDefault();

                    datos.Add(new cEntrevistaInicialTS()
                    {
                        Centro = SelectIngreso.CAMA.CELDA.SECTOR.EDIFICIO.CENTRO.DESCR,
                        FechaIngreso = string.Format("{0:dd/MM/yyyy}", SelectIngreso.FEC_REGISTRO),
                        Expediente = string.Format("{0}/{1}", SelectIngreso.ID_ANIO, SelectIngreso.ID_IMPUTADO),
                        Nombre = string.Format("{0} {1} {2}", !string.IsNullOrEmpty(SelectIngreso.IMPUTADO.PATERNO) ? SelectIngreso.IMPUTADO.PATERNO.Trim() : string.Empty, !string.IsNullOrEmpty(SelectIngreso.IMPUTADO.MATERNO) ? SelectIngreso.IMPUTADO.MATERNO.Trim() : string.Empty, !string.IsNullOrEmpty(SelectIngreso.IMPUTADO.NOMBRE) ? SelectIngreso.IMPUTADO.NOMBRE.Trim() : string.Empty),
                        Edad = string.Format("{0}", new Fechas().CalculaEdad(SelectIngreso.IMPUTADO.NACIMIENTO_FECHA)),
                        Sexo = SelectIngreso.IMPUTADO.SEXO == "M" ? "MASCULINO" : "FEMENINO",
                        Alias = alias,
                        //EstadoCivil = SelectIngreso.IMPUTADO.ESTADO_CIVIL.DESCR,
                        EstadoCivil = SelectIngreso.ESTADO_CIVIL != null ? SelectIngreso.ESTADO_CIVIL.DESCR : string.Empty,
                        Nacionalidad = SelectIngreso.IMPUTADO.PAIS_NACIONALIDAD.NACIONALIDAD,
                        //Escolaridad = SelectIngreso.IMPUTADO.ESCOLARIDAD.DESCR,
                        Escolaridad = SelectIngreso.ESCOLARIDAD != null ?  SelectIngreso.ESCOLARIDAD.DESCR :  string.Empty,
                        //Religion = SelectIngreso.IMPUTADO.RELIGION.DESCR,
                        Religion = SelectIngreso.RELIGION != null ? SelectIngreso.RELIGION.DESCR : string.Empty,
                        Etnia = SelectIngreso.IMPUTADO.ETNIA.DESCR,
                        //Telefono = string.Format("{0}", SelectIngreso.IMPUTADO.TELEFONO),
                        Telefono = string.Format("{0}", SelectIngreso.TELEFONO),
                        //Domicilio = string.Format("{0} {1}", SelectIngreso.IMPUTADO.DOMICILIO_CALLE, SelectIngreso.IMPUTADO.DOMICILIO_NUM_EXT),
                        Domicilio = string.Format("{0} {1}", SelectIngreso.DOMICILIO_CALLE, SelectIngreso.DOMICILIO_NUM_EXT),
                        //Colonia = SelectIngreso.IMPUTADO.COLONIA.DESCR,
                        Colonia = SelectIngreso.COLONIA != null ? SelectIngreso.COLONIA.DESCR : string.Empty,
                        //TiempoEstado = string.Format("{0} {1}", SelectIngreso.IMPUTADO.RESIDENCIA_ANIOS != null ? SelectIngreso.IMPUTADO.RESIDENCIA_ANIOS + " Año(s)" : string.Empty, SelectIngreso.IMPUTADO.RESIDENCIA_ANIOS != null ? SelectIngreso.IMPUTADO.RESIDENCIA_MESES + " Mes(es)" : string.Empty),
                        TiempoEstado = string.Format("{0} {1}", SelectIngreso.RESIDENCIA_ANIOS != null ? SelectIngreso.RESIDENCIA_ANIOS + " Año(s)" : string.Empty, SelectIngreso.RESIDENCIA_ANIOS != null ? SelectIngreso.RESIDENCIAS_MESES + " Mes(es)" : string.Empty),
                        Delito = SelectIngreso.INGRESO_DELITO != null ? SelectIngreso.INGRESO_DELITO.DESCR : string.Empty,
                        ActaNacimiento = emi != null ? emi.EMI != null ? emi.EMI.EMI_FICHA_IDENTIFICACION != null ? emi.EMI.EMI_FICHA_IDENTIFICACION.ACTA_NACIMIENTO == "S" ? "X" : string.Empty : string.Empty : string.Empty : string.Empty,
                        Pasaporte = emi != null ? emi.EMI != null ? emi.EMI.EMI_FICHA_IDENTIFICACION != null ? emi.EMI.EMI_FICHA_IDENTIFICACION.PASAPORTE == "S" ? "X" : string.Empty : string.Empty : string.Empty : string.Empty,
                        CredencialElector = emi != null ? emi.EMI != null ? emi.EMI.EMI_FICHA_IDENTIFICACION != null ? emi.EMI.EMI_FICHA_IDENTIFICACION.CREDENCIAL_ELECTOR == "S" ? "X" : string.Empty : string.Empty : string.Empty : string.Empty,
                        LicenciaManejo = emi != null ? emi.EMI != null ? emi.EMI.EMI_FICHA_IDENTIFICACION != null ? emi.EMI.EMI_FICHA_IDENTIFICACION.LICENCIA_MANEJO == "S" ? "X" : string.Empty : string.Empty : string.Empty : string.Empty,
                        CartillaMilitar = emi != null ? emi.EMI != null ? emi.EMI.EMI_FICHA_IDENTIFICACION != null ? emi.EMI.EMI_FICHA_IDENTIFICACION.CARTILLA_MILITAR == "S" ? "X" : string.Empty : string.Empty : string.Empty : string.Empty,
                        CertificadoEducacion = emi != null ? emi.EMI != null ? emi.EMI.EMI_FICHA_IDENTIFICACION != null ? emi.EMI.EMI_FICHA_IDENTIFICACION.EDUCACION_CERTIFICADO.DESCR : string.Empty : string.Empty : string.Empty,
                        Otros = emi != null ? emi.EMI != null ? emi.EMI.EMI_FICHA_IDENTIFICACION != null ? emi.EMI.EMI_FICHA_IDENTIFICACION.OFICIOS_HABILIDADES : string.Empty : string.Empty : string.Empty,/*revisar*/
                        ViviaAntesDetencion = emi != null ? emi.EMI != null ? emi.EMI.EMI_FICHA_IDENTIFICACION != null ? emi.EMI.EMI_FICHA_IDENTIFICACION.PERSONA_CONVIVENCIA_ANTERIOR : string.Empty : string.Empty : string.Empty,
                        NombreResponsable = emi != null ? emi.EMI != null ? emi.EMI.EMI_FACTORES_SOCIO_FAMILIARES != null ? emi.EMI.EMI_FACTORES_SOCIO_FAMILIARES.CONTACTO_NOMBRE : string.Empty : string.Empty : string.Empty,
                        TelefonoResponsable = emi != null ? emi.EMI != null ? emi.EMI.EMI_FACTORES_SOCIO_FAMILIARES != null ? string.Format("{0}", emi.EMI.EMI_FACTORES_SOCIO_FAMILIARES.CONTACTO_TELEFONO) : string.Empty : string.Empty : string.Empty,
                        ParentescoResponsable = emi != null ? emi.EMI != null ? emi.EMI.EMI_FACTORES_SOCIO_FAMILIARES != null ? emi.EMI.EMI_FACTORES_SOCIO_FAMILIARES.TIPO_REFERENCIA.DESCR : string.Empty : string.Empty : string.Empty,
                    });

                    if (emi != null)
                        if (emi.EMI.EMI_GRUPO_FAMILIAR != null)
                            foreach (var x in emi.EMI.EMI_GRUPO_FAMILIAR)
                            {
                                familia.Add(new cEstructuraFamiliar()
                                {
                                    Nombre = string.Format("{0} {1} {2}", !string.IsNullOrEmpty(x.NOMBRE) ? x.NOMBRE.Trim() : string.Empty, !string.IsNullOrEmpty(x.PATERNO) ? x.PATERNO.Trim() : string.Empty, !string.IsNullOrEmpty(x.MATERNO) ? x.MATERNO.Trim() : string.Empty),
                                    Edad = string.Format("{0}", x.EDAD),
                                    Parentesco = x.TIPO_REFERENCIA.DESCR
                                });
                            }


                    var reporte = new List<cReporte>();
                    reporte.Add(new cReporte()
                    {
                        Logo1 = Parametro.LOGO_ESTADO,
                        Encabezado1 = Parametro.ENCABEZADO1,
                        Encabezado2 = Parametro.ENCABEZADO2,
                        Encabezado3 = SelectIngreso.CENTRO.DESCR.ToUpper(),
                        Encabezado4 = "ENTREVISTA INICIAL TRABAJO SOCIAL",
                    });
                    //ARMAMOS EL REPORTE
                    Reporte.LocalReport.ReportPath = "Reportes/rEntrevistaInicialTS.rdlc";
                    Reporte.LocalReport.DataSources.Clear();

                    Microsoft.Reporting.WinForms.ReportDataSource rds1 = new Microsoft.Reporting.WinForms.ReportDataSource();
                    rds1.Name = "DataSet1";
                    rds1.Value = datos;
                    Reporte.LocalReport.DataSources.Add(rds1);

                    Microsoft.Reporting.WinForms.ReportDataSource rds2 = new Microsoft.Reporting.WinForms.ReportDataSource();
                    rds2.Name = "DataSet2";
                    rds2.Value = familia;
                    Reporte.LocalReport.DataSources.Add(rds2);

                    Microsoft.Reporting.WinForms.ReportDataSource rds3 = new Microsoft.Reporting.WinForms.ReportDataSource();
                    rds3.Name = "DataSet3";
                    rds3.Value = reporte;
                    Reporte.LocalReport.DataSources.Add(rds3);

                    Reporte.RefreshReport();
                }
            }
            catch (Exception ex)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al generar la entrevista inicial de trabajo social", ex);
            }
        }
        #endregion

        #region AsesoriaJuridica
        private int SentenciaAnio = 0;
        private int SentenciaMes = 0;
        private int SentenciaDia = 0;
        private int CumplidoAnio = 0;
        private int CumplidoMes = 0;
        private int CumplidoDia = 0;

        private void ReporteAsesoriaJuridica() 
        {
            try
            {
                if (SelectedIngreso != null)
                {
                    CultureInfo cultura = new CultureInfo("es-MX");
                    var obj = new cAsesoriaJuridica();
                    var hoy = Fechas.GetFechaDateServer;
                    var centro = new cCentro().Obtener(GlobalVar.gCentro).FirstOrDefault();
                    if (centro != null)
                    {
                        obj.LugarFecha = string.Format("{0},{1};A {2} DE {3} DE {4}", centro.MUNICIPIO.MUNICIPIO1, centro.MUNICIPIO.ENTIDAD.DESCR, hoy.Day, cultura.DateTimeFormat.GetMonthName(hoy.Month), hoy.Year).ToString();
                        obj.Interno = string.Format("{0} {1} {2}", SelectedIngreso.IMPUTADO.NOMBRE, SelectedIngreso.IMPUTADO.PATERNO, SelectedIngreso.IMPUTADO.MATERNO);
                        obj.Familiar = string.Empty;
                        obj.Parentesco = string.Empty;
                        if (SelectedIngreso.ID_UB_CAMA != null)
                        {
                            obj.Ubicacion = string.Format("{0}-{1}-{2}-{3}",
                                                  SelectedIngreso.CAMA.CELDA.SECTOR.EDIFICIO.DESCR.Trim(),
                                                  SelectedIngreso.CAMA.CELDA.SECTOR.DESCR.Trim(),
                                                  SelectedIngreso.CAMA.ID_CELDA.Trim(),
                                                  SelectedIngreso.CAMA.ID_CAMA);
                        }
                        obj.DefensorSI = string.Empty;
                        obj.DefensorNO = string.Empty;
                        obj.DefensorPublico = string.Empty;
                        obj.DefensorParticular = string.Empty;

                        obj.Expediente = string.Format("{0}/{1}", SelectedIngreso.ID_ANIO, SelectedIngreso.ID_IMPUTADO);

                        obj.CausasPenales = string.Empty;
                        obj.Juzgados = string.Empty;
                        bool federal = false, comun = false;
                        obj.Delitos = string.Empty;
                        if (SelectedIngreso.CAUSA_PENAL != null)
                        {
                            foreach (var cp in SelectedIngreso.CAUSA_PENAL)//Mostrar todas las CP?
                            {
                                if (!string.IsNullOrEmpty(obj.CausasPenales))
                                { 
                                    obj.CausasPenales = obj.CausasPenales + ", ";
                                    obj.Juzgados = obj.Juzgados + ", ";
                                }
                                obj.CausasPenales = obj.CausasPenales + string.Format("{0}/{1}", cp.CP_ANIO, cp.CP_FOLIO);
                                obj.Juzgados = obj.Juzgados + cp.JUZGADO.DESCR;

                                if (cp.CP_FUERO == "C")
                                    comun = true;
                                else
                                    if (cp.CP_FUERO == "F")
                                        federal = true;
                            
                                //Delitos
                                if (cp.CAUSA_PENAL_DELITO != null)
                                {
                                    foreach (var d in cp.CAUSA_PENAL_DELITO)
                                    { 
                                        //if(!string.IsNullOrEmpty(obj.Delitos))
                                        //    obj.Delitos = obj.Delitos + ", "
                                    }
                                }
                            }

                        }
                        if (comun && federal)
                            obj.FueroAmbos = "X";
                        else
                            if (comun)
                                obj.FueroComun = "X";
                            else
                                if (federal)
                                    obj.FueroFederal = "X";


                    }

                    obj.Indiciado = string.Empty; 
                    obj.Imputado =  string.Empty;
                    obj.Procesado =string.Empty;
                    obj.Sentenciado =string.Empty;
                    obj.Apelacion =   string.Empty;
                    obj.Amparo = string.Empty;


                }
                else
                    new Dialogos().ConfirmacionDialogo("Validación","Favor de seleccionar un ingreso");

            }
            catch (Exception ex)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al ingresar búsqueda", ex);
            }
        }

        private void ImprimirAsesoriaJuridica() 
        {
            try
            {
                if (SelectedIngreso != null)
                {

                    var documento = new cImputadoTipoDocumento().Obtener((short)enumTipoDocumentoImputado.ASESORIA_JURIDICA);
                    if (documento == null)
                    {
                        new Dialogos().ConfirmacionDialogo("Validación", "No se encontro la plantilla del documento");
                        return;
                    }
                    using (var ms = new MemoryStream())
                    {
                        ms.Write(documento.DOCUMENTO, 0, documento.DOCUMENTO.Length);
                        using (DocX doc = DocX.Load(ms))
                        {
                            var hoy = Fechas.GetFechaDateServer;
                            var centro = new cCentro().Obtener(GlobalVar.gCentro).FirstOrDefault();
                            doc.ReplaceText("{centro}", centro.DESCR);
                            doc.ReplaceText("{municipio}", centro.MUNICIPIO.MUNICIPIO1.Trim());
                            doc.ReplaceText("{estado}", centro.MUNICIPIO.ENTIDAD.DESCR.Trim());
                            doc.ReplaceText("{dia}", hoy.Day.ToString());
                            CultureInfo cultura = new CultureInfo("es-MX");
                            doc.ReplaceText("{mes}", cultura.DateTimeFormat.GetMonthName(hoy.Month));
                            doc.ReplaceText("{anio}", hoy.Year.ToString());
                            doc.ReplaceText("{interno}", string.Format("{0} {1} {2}", SelectedIngreso.IMPUTADO.NOMBRE.Trim(),
                                !string.IsNullOrEmpty(SelectedIngreso.IMPUTADO.PATERNO) ? SelectedIngreso.IMPUTADO.PATERNO.Trim() : string.Empty,
                                !string.IsNullOrEmpty(SelectedIngreso.IMPUTADO.MATERNO) ? SelectedIngreso.IMPUTADO.MATERNO.Trim() : string.Empty));
                            doc.ReplaceText("{familiar}", "  ");
                            doc.ReplaceText("{parentesco}", "  ");
                            var cama = SelectedIngreso.CAMA;
                            doc.ReplaceText("{ubicacion}", string.Format("{0}-{1}-{2}-{3}",
                                                  cama.CELDA.SECTOR.EDIFICIO.DESCR.Trim(),
                                                  cama.CELDA.SECTOR.DESCR.Trim(),
                                                  cama.ID_CELDA.Trim(),
                                                  cama.ID_CAMA));
                            doc.ReplaceText("{1}", " ");
                            doc.ReplaceText("{2}", " ");
                            doc.ReplaceText("{3}", " ");
                            doc.ReplaceText("{4}", " ");
                            doc.ReplaceText("{expediente}", string.Format("{0}/{1}", SelectedIngreso.ID_ANIO, SelectedIngreso.ID_IMPUTADO));

                            if (SelectedIngreso.CAUSA_PENAL != null)
                            {
                                CalcularSentencia();
                                string CP = string.Empty, Juzgados = string.Empty, Delitos = string.Empty, Fuero = string.Empty;
                                int Comun = 0, Federal = 0;
                                foreach (var cp in SelectedIngreso.CAUSA_PENAL)
                                {
                                    if (cp.CP_FUERO == "C")
                                        Comun = 1;
                                    else
                                        if (cp.CP_FUERO == "F")
                                            Federal = 1;

                                    if (!string.IsNullOrEmpty(CP))
                                        CP = CP + ",";
                                    CP = CP + string.Format("{0}/{1}", cp.CP_ANIO, cp.CP_FOLIO);
                                    if (!string.IsNullOrEmpty(Juzgados))
                                        Juzgados = Juzgados + ",";
                                    Juzgados = Juzgados + cp.JUZGADO.DESCR.Trim();
                                    if (cp.CAUSA_PENAL_DELITO != null)
                                    {
                                        foreach (var d in cp.CAUSA_PENAL_DELITO)
                                        {
                                            if (!string.IsNullOrEmpty(Delitos))
                                                Delitos = Delitos + ",";
                                            Delitos = Delitos + d.DESCR_DELITO.Trim();

                                        }
                                    }
                                }
                                doc.ReplaceText("{causaspenales}", CP);
                                doc.ReplaceText("{juzgado}", Juzgados);
                                doc.ReplaceText("{delitos}", Delitos);
                                if (Comun == 1 && Federal == 1)
                                    doc.ReplaceText("{7}", "X");
                                else
                                    if (Comun == 1)
                                        doc.ReplaceText("{6}", "X");
                                    else
                                        if (Federal == 1)
                                            doc.ReplaceText("{5}", "X");
                                doc.ReplaceText("{5}", " ");
                                doc.ReplaceText("{6}", " ");
                                doc.ReplaceText("{7}", " ");

                                if (SelectedIngreso.ID_CLASIFICACION_JURIDICA == "1")
                                {
                                    if (!string.IsNullOrEmpty(SelectedIngreso.NUC))
                                        doc.ReplaceText("{9}", "X");
                                    else
                                        doc.ReplaceText("{8}", "X");
                                }
                                else
                                    if (SelectedIngreso.ID_CLASIFICACION_JURIDICA == "2")
                                        doc.ReplaceText("{10}", "X");
                                    else
                                        if (SelectedIngreso.ID_CLASIFICACION_JURIDICA == "3")
                                            doc.ReplaceText("{11}", "X");
                                doc.ReplaceText("{8}", " ");
                                doc.ReplaceText("{9}", " ");
                                doc.ReplaceText("{10}", " ");
                                doc.ReplaceText("{11}", " ");
                                doc.ReplaceText("{12}", " ");
                                doc.ReplaceText("{13}", " ");

                                if (SentenciaAnio > 0 || SentenciaMes > 0 || SentenciaDia > 0)
                                {
                                    doc.ReplaceText("{s}", "X");
                                    doc.ReplaceText("{sa}", SentenciaAnio.ToString());
                                    doc.ReplaceText("{sm}", SentenciaMes.ToString());
                                    doc.ReplaceText("{sd}", SentenciaDia.ToString());
                                }
                                else
                                {
                                    doc.ReplaceText("{s}", " ");
                                    doc.ReplaceText("{sa}", " ");
                                    doc.ReplaceText("{sm}", " ");
                                    doc.ReplaceText("{sd}", " ");
                                }

                                if (CumplidoAnio > 0 || CumplidoMes > 0 || CumplidoDia > 0)
                                {
                                    doc.ReplaceText("{ra}", CumplidoAnio.ToString());
                                    doc.ReplaceText("{rm}", CumplidoMes.ToString());
                                    doc.ReplaceText("{rd}", CumplidoDia.ToString());
                                }
                                else
                                {
                                    doc.ReplaceText("{ra}", " ");
                                    doc.ReplaceText("{rm}", " ");
                                    doc.ReplaceText("{rd}", " ");
                                }

                                doc.ReplaceText("{ets}", " ");
                                doc.ReplaceText("{etn}", " ");
                                doc.ReplaceText("{d}", " ");
                                doc.ReplaceText("{fec_estudio}", " ");
                                doc.ReplaceText("{ma}", " ");
                            }

                            doc.Save();

                            var tc = new TextControlView();
                            PopUpsViewModels.ClosePopUp(PopUpsViewModels.TipoPopUp.OSCURECER_FONDO);
                            tc.editor.Loaded += (s, e) =>
                            {
                                try
                                {
                                    tc.editor.Load(ms.ToArray(), TXTextControl.BinaryStreamType.WordprocessingML);
                                }
                                catch (Exception ex)
                                {
                                    StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al mostrar el documento", ex);
                                }
                            };
                            tc.Owner = PopUpsViewModels.MainWindow;
                            tc.Closed += (s, e) => { PopUpsViewModels.ClosePopUp(PopUpsViewModels.TipoPopUp.OSCURECER_FONDO); };
                            PopUpsViewModels.ShowPopUp(this, PopUpsViewModels.TipoPopUp.OSCURECER_FONDO);
                            tc.Show();
                        }// Release this document from memory.
                    }
                }
                else
                    new Dialogos().ConfirmacionDialogo("Validación", "Favor de seleccionar a un interno");
            }
            catch (Exception ex)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al cerrar búsqueda", ex);
            }
        }

        private void CalcularSentencia()
        {
            try
            {
                if (SelectedIngreso != null)
                {
                    int anios = 0, meses = 0, dias = 0, anios_abono = 0, meses_abono = 0, dias_abono = 0;
                    DateTime? FechaInicioCompurgacion = null, FechaFinCompurgacion = null;
                    if (SelectedIngreso.CAUSA_PENAL != null)
                    {
                        foreach (var cp in SelectedIngreso.CAUSA_PENAL)
                        {
                            bool segundaInstancia = false, Incidente = false;
                            if (cp.SENTENCIA != null)
                            {
                                if (cp.SENTENCIA.Count > 0)
                                {
                                    #region BUSCAMOS SI TIENE 2DA INSTANCIA
                                    if (cp.RECURSO.Count > 0)
                                    {
                                        var r = cp.RECURSO.Where(w => w.SENTENCIA_ANIOS > 0 || w.SENTENCIA_MESES > 0 || w.SENTENCIA_DIAS > 0);
                                        if (r != null)
                                        {
                                            var res = r.OrderByDescending(w => w.ID_RECURSO).FirstOrDefault();
                                            if (res != null)
                                            {
                                                //SENTENCIA
                                                anios = anios + (res.SENTENCIA_ANIOS != null ? res.SENTENCIA_ANIOS.Value : 0);
                                                meses = meses + (res.SENTENCIA_MESES != null ? res.SENTENCIA_MESES.Value : 0);
                                                dias = dias + (res.SENTENCIA_DIAS != null ? res.SENTENCIA_DIAS.Value : 0);
                                                segundaInstancia = true;
                                            }
                                        }
                                    }
                                    #endregion

                                    #region Incidente
                                    if (cp.AMPARO_INCIDENTE != null)
                                    {
                                        var i = cp.AMPARO_INCIDENTE.Where(w => w.MODIFICA_PENA_ANIO != null && w.MODIFICA_PENA_MES != null && w.MODIFICA_PENA_DIA != null);
                                        if (i != null && segundaInstancia == false)
                                        {
                                            var res = i.OrderByDescending(w => w.ID_AMPARO_INCIDENTE).FirstOrDefault();// SingleOrDefault();
                                            if (res != null)
                                            {

                                                anios = anios + (res.MODIFICA_PENA_ANIO != null ? res.MODIFICA_PENA_ANIO.Value : 0);
                                                meses = meses + (res.MODIFICA_PENA_MES != null ? res.MODIFICA_PENA_MES.Value : 0);
                                                dias = dias + (res.MODIFICA_PENA_DIA != null ? res.MODIFICA_PENA_DIA.Value : 0);
                                                Incidente = true;
                                            }
                                        }

                                        //ABONOS
                                        var dr = cp.AMPARO_INCIDENTE.Where(w => w.DIAS_REMISION != null);
                                        if (i != null)
                                        {
                                            foreach (var x in dr)
                                            {
                                                //ABONO
                                                dias_abono = dias_abono + (x.DIAS_REMISION != null ? (int)x.DIAS_REMISION : 0);
                                            }
                                        }
                                    }
                                    #endregion
                                    var s = cp.SENTENCIA.Where(w => w.ESTATUS == "A").FirstOrDefault();
                                    if (s != null)
                                    {
                                        if (FechaInicioCompurgacion == null)
                                        {
                                            FechaInicioCompurgacion = s.FEC_INICIO_COMPURGACION;
                                            //FechaInicioCompurgacion = s.FEC_REAL_COMPURGACION;
                                        }
                                        else
                                        {
                                            if (FechaInicioCompurgacion > s.FEC_INICIO_COMPURGACION)
                                                FechaInicioCompurgacion = s.FEC_INICIO_COMPURGACION;
                                            //if (FechaInicioCompurgacion > s.FEC_REAL_COMPURGACION)
                                            //    FechaInicioCompurgacion = s.FEC_REAL_COMPURGACION;
                                        }

                                        //SENTENCIA
                                        if (!segundaInstancia && !Incidente)
                                        {
                                            anios = anios + (s.ANIOS != null ? s.ANIOS.Value : 0);
                                            meses = meses + (s.MESES != null ? s.MESES.Value : 0);
                                            dias = dias + (s.DIAS != null ? s.DIAS.Value : 0);
                                        }

                                        //ABONO
                                        anios_abono = anios_abono + (s.ANIOS_ABONADOS != null ? s.ANIOS_ABONADOS.Value : 0);
                                        meses_abono = meses_abono + (s.MESES_ABONADOS != null ? s.MESES_ABONADOS.Value : 0);
                                        dias_abono = dias_abono + (s.DIAS_ABONADOS != null ? s.DIAS_ABONADOS.Value : 0);
                                    }
                                }
                            }
                        }
                    }

                    while (dias > 29)
                    {
                        meses++;
                        dias = dias - 30;
                    }
                    while (meses > 11)
                    {
                        anios++;
                        meses = meses - 12;
                    }

                    SentenciaAnio = anios;
                    SentenciaMes = meses;
                    SentenciaDia= dias;

                    //AniosAbonosI = anios_abono;
                    //MesesAbonosI = meses_abono;
                    //DiasAbonosI = dias_abono;

                    if (FechaInicioCompurgacion != null)
                    {
                        FechaFinCompurgacion = FechaInicioCompurgacion;
                        FechaFinCompurgacion = FechaFinCompurgacion.Value.AddYears(anios);
                        FechaFinCompurgacion = FechaFinCompurgacion.Value.AddMonths(meses);
                        FechaFinCompurgacion = FechaFinCompurgacion.Value.AddDays(dias);
                        //
                        FechaFinCompurgacion = FechaFinCompurgacion.Value.AddYears(-anios_abono);
                        FechaFinCompurgacion = FechaFinCompurgacion.Value.AddMonths(-meses_abono);
                        FechaFinCompurgacion = FechaFinCompurgacion.Value.AddDays(-dias_abono);

                        int a = 0, m = 0, d = 0;
                        //new Fechas().DiferenciaFechas(Fechas.GetFechaDateServer.Date, FechaInicioCompurgacion.Value.Date, out a, out  m, out d);
                        new Fechas().DiferenciaFechas(Fechas.GetFechaDateServer.Date, FechaInicioCompurgacion.Value.Date, out a, out  m, out d);
                        CumplidoAnio = a;
                        CumplidoMes = m;
                        CumplidoDia = d;
                        //a = m = d = 0;
                        //new Fechas().DiferenciaFechas(FechaFinCompurgacion.Value.Date, Fechas.GetFechaDateServer.Date, out a, out  m, out d);
                        //AniosRestanteI = a;
                        //MesesRestanteI = m;
                        //DiasRestanteI = d;
                    }

                }
            }
            catch (Exception ex)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al calcular sentencia", ex);
            }
        }
        #endregion

        #region Cambio SelectedItem de Busqueda de Expediente
        private async void OnModelChangedSwitch(object parametro)
        {
            if (parametro != null)
            {
                switch (parametro.ToString())
                {
                    case "cambio_expediente":
                        if (SelectExpediente != null && (SelectExpediente.INGRESO == null || SelectExpediente.INGRESO.Count == 0))
                        {
                            await StaticSourcesViewModel.CargarDatosMetodoAsync(() =>
                            {
                                selectExpediente = new cImputado().Obtener(selectExpediente.ID_IMPUTADO, selectExpediente.ID_ANIO, selectExpediente.ID_CENTRO).First();
                                RaisePropertyChanged("SelectExpediente");
                            });
                            //MUESTRA LOS INGRESOS
                            if (SelectExpediente.INGRESO != null && SelectExpediente.INGRESO.Count > 0)
                            {
                                EmptyIngresoVisible = false;
                                SelectIngreso = SelectExpediente.INGRESO.OrderByDescending(o => o.ID_INGRESO).FirstOrDefault();
                            }
                            else
                                EmptyIngresoVisible = true;

                            //OBTENEMOS FOTO DE FRENTE
                            if (SelectIngreso != null)
                            {
                                if (SelectIngreso.INGRESO_BIOMETRICO.Where(w => w.ID_TIPO_BIOMETRICO == (short)enumTipoBiometrico.FOTO_FRENTE_REGISTRO && w.ID_FORMATO == (short)enumTipoFormato.FMTO_JPG).Any())
                                    ImagenImputado = SelectIngreso.INGRESO_BIOMETRICO.Where(w => w.ID_TIPO_BIOMETRICO == (short)enumTipoBiometrico.FOTO_FRENTE_REGISTRO && w.ID_FORMATO == (short)enumTipoFormato.FMTO_JPG).FirstOrDefault().BIOMETRICO;
                                else
                                    ImagenImputado = new Imagenes().getImagenPerson();
                            }
                            else
                                ImagenImputado = new Imagenes().getImagenPerson();
                        }
                        break;
                }
            }
        }
        #endregion
    }
}
