using Cogent.Biometrics;
using ControlPenales.BiometricoServiceReference;
using ControlPenales.Clases;
using DPUruNet;
using MahApps.Metro.Controls.Dialogs;
using Microsoft.Reporting.WinForms;
using SSP.Controlador.Catalogo.Justicia;
using SSP.Controlador.Catalogo.Justicia.Ingreso;
using SSP.Servidor;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data.Objects;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;

namespace ControlPenales
{
    partial class EscalaRiesgoViewModel : FingerPrintScanner
    {
        public EscalaRiesgoViewModel() { }

        #region Metodos
        private async void clickSwitch(object op)
        {
            try { 
                switch (op.ToString())
                {
                    case "buscar_menu":
                        if (!pConsultar)
                        {
                            new Dialogos().ConfirmacionDialogo("Validación", "Su usuario no tiene privilegios para realizar esta acción");
                            break;
                        }
                        LimpiarBusqueda();
                        PopUpsViewModels.ShowPopUp(this, PopUpsViewModels.TipoPopUp.BUSCAR_ESCALA_RIESGO);
                        break;
                    case "buscar_escala_riesgo":
                        if (!pConsultar)
                        {
                            new Dialogos().ConfirmacionDialogo("Validación", "Su usuario no tiene privilegios para realizar esta acción");
                            break;
                        }
                        //BuscarEscalaRiesgo();
                        Pagina = 1;
                        SeguirCargando = true;
                        LstEscalaRiesgo = new RangeEnabledObservableCollection<ESCALA_RIESGO>();
                        LstEscalaRiesgo.InsertRange(await SegmentarResultadoBusqueda());
                        if (LstEscalaRiesgo.Count > 0)
                            EmptyBuscarEscalaRiesgo = Visibility.Collapsed;
                        else
                        EmptyBuscarEscalaRiesgo = Visibility.Visible;
                        break;
                    case "nueva_busqueda_escala_riesgo":
                        LimpiarBusqueda();
                        break;
                    case "seleccionar_buscar_escala_riesgo":
                        if (SelectedEscalaRiesgo == null)
                        {
                            new Dialogos().ConfirmacionDialogo("Validación", "Favor de selecconar una escala de riesgo");
                            break;
                        }
                        PopulateEscalaRiesgo();
                        PopUpsViewModels.ClosePopUp(PopUpsViewModels.TipoPopUp.BUSCAR_ESCALA_RIESGO);
                        break;
                    case "guardar_menu":
                        GuardarEscalaRiesgo();
                        break;
                    case "reporte_menu":
                        if (!pImprimir)
                        {
                            new Dialogos().ConfirmacionDialogo("Validación", "Su usuario no tiene privilegios para realizar esta acción");
                            break;
                        }
                        GeneraReporte();
                        break;
                    case "cancelar_buscar_persona":
                        if (SelectedEscalaRiesgo != null)
                        {
                            NUC = SelectedEscalaRiesgo.NUC;
                            Paterno = SelectedEscalaRiesgo.PATERNO;
                            Materno = SelectedEscalaRiesgo.MATERNO;
                            Nombre = SelectedEscalaRiesgo.NOMBRE;
                        }
                        PopUpsViewModels.ClosePopUp(PopUpsViewModels.TipoPopUp.BUSCAR_ESCALA_RIESGO);
                        break;
                    case "limpiar_menu":
                        ((System.Windows.Controls.ContentControl)PopUpsViewModels.MainWindow.FindName("contentControl")).Content = new EscalaRiesgoView();
                        ((System.Windows.Controls.ContentControl)PopUpsViewModels.MainWindow.FindName("contentControl")).DataContext = new ControlPenales.EscalaRiesgoViewModel();
                        break;
                    case "salir_menu":
                        PrincipalViewModel.SalirMenu();
                        break;
                }
            }
            catch (Exception ex)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error...", ex);
            }
        }

        private void Radio(object obj)
        {
            switch (obj.ToString())
            { 
                case "a1":
                    DatosFamiliares = 6;
                    break;
                case "a2":
                    DatosFamiliares = 4;
                    break;
                case "a3":
                    DatosFamiliares = 1;
                    break;
                case "b1":
                    AportacionEconomica = 6;
                    break;
                case "b2":
                    AportacionEconomica = -2;
                    break;
                case "c1":
                    ArraigoLocalidad = 4;
                    break;
                case "c2":
                    ArraigoLocalidad = 3;
                    break;
                case "c3":
                    ArraigoLocalidad = 2;
                    break;
                case "c4":
                    ArraigoLocalidad = -2;
                    break;
                case "c5":
                    ArraigoLocalidad = -4;
                    break;
                case "d1":
                    Residencia = 6;
                    break;
                case "d2":
                    Residencia = 4;
                    break;
                case "d3":
                    Residencia = 2;
                    break;
                case "d4":
                    Residencia = -6;
                    break;
                case "e1":
                    HistoriaLaboral = 6;
                    break;
                case "e2":
                    HistoriaLaboral = 4;
                    break;
                case "e3":
                    HistoriaLaboral = 2;
                    break;
                case "e4":
                    HistoriaLaboral = -2;
                    break;
                case "e5":
                    HistoriaLaboral = -6;
                    break;
                case "f1":
                    Consumosustancias = -4;
                    break;
                case "f2":
                    Consumosustancias = -2;
                    break;
                case "g1":
                    PosiblePena = 4;
                    break;
                case "g2":
                    PosiblePena = -4;
                    break;
                case "i1":
                    AntecedentesPenales = -6;
                    break;
                case "i2":
                    AntecedentesPenales = -5;
                    break;
                case "i3":
                    AntecedentesPenales = -6;
                    break;
            }

            CalculaTotal();
        }

        private void CalculaTotal() 
        {
            LstCalificaciones = new ObservableCollection<cEscalaRiesgoCalificacion>();
            short a = (DatosFamiliares.HasValue ? DatosFamiliares.Value : (short)0);
            short b = (AportacionEconomica.HasValue ? AportacionEconomica.Value : (short)0);
            short c = (ArraigoLocalidad.HasValue ? ArraigoLocalidad.Value : (short)0);
            short d = (Residencia.HasValue ? Residencia.Value : (short)0);
            short e = (HistoriaLaboral.HasValue ? HistoriaLaboral.Value : (short)0);
            short f = (Consumosustancias.HasValue ? Consumosustancias.Value : (short)0);
            short g = (PosiblePena.HasValue ? PosiblePena.Value : (short)0);
            short h = (CumplimientoCondiciones.HasValue ? CumplimientoCondiciones.Value : (short)0);
            short i = (AntecedentesPenales.HasValue ? AntecedentesPenales.Value : (short)0);
            short j = (DatosFalsos.HasValue ? DatosFalsos.Value : (short)0);
            short t = (short)(a + b + c + d + e + f + g + h + i + j);
            LstCalificaciones.Add(new cEscalaRiesgoCalificacion()
            {
                A1 = a,
                B1 = b,
                C1 = c,
                D1 = d,
                E1 = e,
                F1 = f,
                G1 = g,
                H1 = h,
                I1 = i,
                J1 = j,
                TOTAL = t
            });

            LstRangos = new ObservableCollection<cEscalaRiesgoRangos>();
            LstRangos.Add(new cEscalaRiesgoRangos()
            {
                ID_RAGO = 1,
                DESCR = "NULO[26 AL 32]",
                INICIO = 26,
                FIN = 32,
                SELECCION = (short)((t >= 26 && t <= 32) ? 1 : 0) 
            });
            LstRangos.Add(new cEscalaRiesgoRangos()
            {
                ID_RAGO = 2,
                DESCR = "BAJO[11 AL 24]",
                INICIO = 11,
                FIN = 24,
                SELECCION = (short)((t >= 11 && t <= 24) ? 1 : 0) 
            });
            LstRangos.Add(new cEscalaRiesgoRangos()
            {
                ID_RAGO = 3,
                DESCR = "MEDIO[10 AL -10]",
                INICIO = -10,
                FIN = 10,
                SELECCION = (short)((t >= -10 && t <= 10) ? 1 : 0) 
            });
            LstRangos.Add(new cEscalaRiesgoRangos()
            {
                ID_RAGO = 4,
                DESCR = "ALTO[-11 AL -47]",
                INICIO = -47,
                FIN = -11,
                SELECCION = (short)((t >= -47 && t <= -11) ? 1 : 0) 
            });
        }

        private async void WindowLoad(EscalaRiesgoView obj = null)
        {
            try
            {
                ConfiguraPermisos();
                pantalla = obj;
                LstCalificaciones = new ObservableCollection<cEscalaRiesgoCalificacion>();
                LstRangos = new ObservableCollection<cEscalaRiesgoRangos>();
                LstRangos.Add(new cEscalaRiesgoRangos() {
                 ID_RAGO = 1,
                 DESCR = "NULO[26 AL 32]",
                 INICIO = 26,
                 FIN = 32
                });
                LstRangos.Add(new cEscalaRiesgoRangos()
                {
                    ID_RAGO = 2,
                    DESCR = "BAJO[11 AL 24]",
                    INICIO = 11,
                    FIN = 24
                });
                LstRangos.Add(new cEscalaRiesgoRangos()
                {
                    ID_RAGO = 3,
                    DESCR = "MEDIO[10 AL -10]",
                    INICIO = -10,
                    FIN = 10
                });
                LstRangos.Add(new cEscalaRiesgoRangos()
                {
                    ID_RAGO = 4,
                    DESCR = "ALTO[-11 AL -47]",
                    INICIO = -47,
                    FIN = -11
                });
                Validaciones();
                StaticSourcesViewModel.SourceChanged = false;

            }
            catch (Exception ex)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error...", ex);
            }
        }

        private async void ClickEnter(Object obj = null)
        {
            try
            {
                if (!pConsultar)
                {
                    (new Dialogos()).ConfirmacionDialogo("Validación", "No cuenta con suficientes privilegios para realizar esta acción.");
                    return;
                }
                if (obj != null)
                {
                    //cuando es boton no se hace nada porque solamente existe el de buscar, si hay otro habra que castearlos a button y hacer la comparacion
                    var textbox = obj as TextBox;
                    if (textbox != null)
                    {
                        switch (textbox.Name)
                        {
                            case "NUCBuscar":
                                NUC = textbox.Text;
                                break;
                            case "NombreBuscar":
                                Nombre = textbox.Text;
                                break;
                            case "ApellidoPaternoBuscar":
                                Paterno = textbox.Text;
                                break;
                            case "ApellidoMaternoBuscar":
                                Materno = textbox.Text;
                                break;
                        }
                    }
                }
                //BuscarEscalaRiesgo();
                Pagina = 1;
                SeguirCargando = true;
                LstEscalaRiesgo = new RangeEnabledObservableCollection<ESCALA_RIESGO>();
                LstEscalaRiesgo.InsertRange(await SegmentarResultadoBusqueda());
            }
            catch (Exception ex)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al buscar al imputado.", ex);
            }
        }
      
        #endregion

        #region Escala Riesgo
        private void GuardarEscalaRiesgo() 
        {
            try
            {
                if (base.HasErrors)
                {
                    new Dialogos().ConfirmacionDialogo("Validación", "Favor de capturar lod datos obligatorios " + base.Error);
                    return;
                }
                var er = new ESCALA_RIESGO();
                er.LUGAR = Lugar;
                er.FECHA = Fecha;
                er.PATERNO = Paterno;
                er.MATERNO = Materno;
                er.NOMBRE = Nombre;
                er.NUC = NUC;
                er.DATOS_FAMILIARES = DatosFamiliares;
                er.APORTACION_ECONOMICA = AportacionEconomica;
                er.ARRAIGO_LOCALIDAD = ArraigoLocalidad;
                er.RESIDENCIA = Residencia;
                er.HISTORIA_LAB_ESC = HistoriaLaboral;
                er.CONSUMO_SUSTANCIAS = Consumosustancias;
                er.POSIBLE_PENA_IMPONER = PosiblePena;
                er.CUMPLIMIENTO_CONDICIONES = CumplimientoCondiciones;;
                er.ANTECEDENTES_PENALES = AntecedentesPenales;
                er.DATOS_FALSOS = DatosFalsos;
                er.TOTAL = (short)(
                    (er.DATOS_FAMILIARES.HasValue ? er.DATOS_FAMILIARES : 0) + 
                    (er.APORTACION_ECONOMICA.HasValue ? er.APORTACION_ECONOMICA : 0) + 
                    (er.ARRAIGO_LOCALIDAD.HasValue ? er.ARRAIGO_LOCALIDAD : 0) +
                    (er.RESIDENCIA.HasValue ? er.RESIDENCIA : 0) +
                    (er.HISTORIA_LAB_ESC.HasValue ? er.HISTORIA_LAB_ESC : 0) +
                    (er.CONSUMO_SUSTANCIAS.HasValue ? er.CONSUMO_SUSTANCIAS : 0) +
                    (er.POSIBLE_PENA_IMPONER.HasValue ? er.POSIBLE_PENA_IMPONER : 0) +
                    (er.CUMPLIMIENTO_CONDICIONES.HasValue ? er.CUMPLIMIENTO_CONDICIONES : 0) +
                    (er.ANTECEDENTES_PENALES.HasValue ? er.ANTECEDENTES_PENALES : 0) +
                    (er.DATOS_FALSOS.HasValue ? er.DATOS_FALSOS : 0));
                er.EVALUADOR = GlobalVar.gUsr;

                if (SelectedEscalaRiesgo == null)
                {
                    if (!pInsertar)
                    {
                        new Dialogos().ConfirmacionDialogo("Validación", "Su usuario no tiene privilegios para realizar esta acción");
                        return;
                    }
                    er.ID_ESCALA_RIESGO = new cEscalaRiesgo().Insertar(er);
                    if (er.ID_ESCALA_RIESGO > 0)
                    {
                        SelectedEscalaRiesgo = new cEscalaRiesgo().Obtener((int)er.ID_ESCALA_RIESGO);
                       new Dialogos().ConfirmacionDialogo("Éxito", "La información se guardo correctamente");
                       StaticSourcesViewModel.SourceChanged = false;
                    }
                }
                else
                {
                    if (!pEditar)
                    {
                        new Dialogos().ConfirmacionDialogo("Validación", "Su usuario no tiene privilegios para realizar esta acción");
                        return;
                    }
                    er.ID_ESCALA_RIESGO = SelectedEscalaRiesgo.ID_ESCALA_RIESGO;
                    if (new cEscalaRiesgo().Actualizar(er))
                    {
                        new Dialogos().ConfirmacionDialogo("Éxito", "La información se guardo correctamente");
                        StaticSourcesViewModel.SourceChanged = false;
                    }
                }
                

            }
            catch (Exception ex)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error...", ex);
            }
        }

        //private void BuscarEscalaRiesgo() 
        //{
        //    try {
        //        //LstEscalaRiesgo = new ObservableCollection<ESCALA_RIESGO>(new cEscalaRiesgo().ObtenerTodos(NUC,Paterno,Materno,Nombre));
        //        //EmptyBuscarEscalaRiesgo = LstEscalaRiesgo.Count > 0 ? Visibility.Collapsed : Visibility.Visible;
        //    }
        //    catch(Exception ex)
        //    {
        //        StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error...", ex);
        //    }
        //}
        
        private void PopulateEscalaRiesgo() 
        {
            try
            {
                if (SelectedEscalaRiesgo != null)
                {
                    Lugar = SelectedEscalaRiesgo.LUGAR;
                    Fecha = SelectedEscalaRiesgo.FECHA;
                    Paterno = SelectedEscalaRiesgo.PATERNO;
                    Materno = SelectedEscalaRiesgo.MATERNO;
                    Nombre = SelectedEscalaRiesgo.NOMBRE;
                    NUC = SelectedEscalaRiesgo.NUC;

                    DatosFamiliares = SelectedEscalaRiesgo.DATOS_FAMILIARES;
                    switch(SelectedEscalaRiesgo.DATOS_FAMILIARES)
                    {
                         case 6:
                            pantalla.a1.IsChecked = true;
                        break;
                        case 4:
                            pantalla.a2.IsChecked = true;
                            break;
                        case 1:
                            pantalla.a3.IsChecked = true;
                        break;
                    }

                    AportacionEconomica = SelectedEscalaRiesgo.APORTACION_ECONOMICA;
                    switch(SelectedEscalaRiesgo.APORTACION_ECONOMICA)
                    {
                        case 6:
                              pantalla.b1.IsChecked = true;
                            break;
                        case -2:
                              pantalla.b2.IsChecked = true;
                            break;
                    }

                    ArraigoLocalidad = SelectedEscalaRiesgo.ARRAIGO_LOCALIDAD;
                    switch(SelectedEscalaRiesgo.ARRAIGO_LOCALIDAD)
                    {
                        case 4:
                            pantalla.c1.IsChecked = true;
                            break;
                        case 3:
                            pantalla.c2.IsChecked = true;
                            break;
                        case 2:
                            pantalla.c3.IsChecked = true;
                            break;
                        case -2:
                            pantalla.c4.IsChecked = true;
                            break;
                        case -4:
                            pantalla.c5.IsChecked = true;
                            break;
                    }

                    Residencia = SelectedEscalaRiesgo.RESIDENCIA;
                    switch(SelectedEscalaRiesgo.RESIDENCIA)
                    {
                        case 6:
                            pantalla.d1.IsChecked = true;
                            break;
                        case 4:
                            pantalla.d2.IsChecked = true;
                            break;
                        case 2:
                            pantalla.d3.IsChecked = true;
                            break;
                        case -6:
                            pantalla.d4.IsChecked = true;
                            break;
                    }

                    HistoriaLaboral = SelectedEscalaRiesgo.HISTORIA_LAB_ESC;
                    switch(SelectedEscalaRiesgo.HISTORIA_LAB_ESC)
                    {
                        case 6:
                             pantalla.e1.IsChecked = true;
                            break;
                        case 4:
                             pantalla.e2.IsChecked = true;
                            break;
                        case 2:
                              pantalla.e3.IsChecked = true;
                            break;
                        case -2:
                              pantalla.e4.IsChecked = true;
                            break;
                        case -6:
                            pantalla.e5.IsChecked = true;
                            break;
                    }

                    Consumosustancias = SelectedEscalaRiesgo.CONSUMO_SUSTANCIAS;
                    switch(SelectedEscalaRiesgo.CONSUMO_SUSTANCIAS)
                    {
                        case -4:
                            pantalla.f1.IsChecked = true;
                            break;
                        case -2:
                            pantalla.f2.IsChecked = true;
                            break;
                    }

                    PosiblePena = SelectedEscalaRiesgo.POSIBLE_PENA_IMPONER;
                    switch(SelectedEscalaRiesgo.POSIBLE_PENA_IMPONER)
                    {
                        case 4:
                              pantalla.g1.IsChecked = true;
                            break;
                        case -4:
                              pantalla.g2.IsChecked = true;
                            break;
                    }

                    CumplimientoCondiciones = SelectedEscalaRiesgo.CUMPLIMIENTO_CONDICIONES;
                    if(SelectedEscalaRiesgo.CUMPLIMIENTO_CONDICIONES != null)
                        pantalla.h1.IsChecked = true;

                    AntecedentesPenales = SelectedEscalaRiesgo.ANTECEDENTES_PENALES;
                    switch (SelectedEscalaRiesgo.ANTECEDENTES_PENALES)
                    {
                        case -6:
                            pantalla.i1.IsChecked = true;
                            //pantalla.i3.IsChecked = true;
                            break;
                        case -11:
                            pantalla.i1.IsChecked = true;
                            pantalla.i2.IsChecked = true;
                            break;
                        case -12:
                            pantalla.i1.IsChecked = true;
                            pantalla.i3.IsChecked = true;
                            break;
                    }

                    DatosFalsos = SelectedEscalaRiesgo.DATOS_FALSOS;
                    if (SelectedEscalaRiesgo.DATOS_FALSOS != null)
                        pantalla.j1.IsChecked = true;

                    CalculaTotal();
                    StaticSourcesViewModel.SourceChanged = false;
                }
                else
                    new Dialogos().ConfirmacionDialogo("Validación", "Favor de guardar la escala de riesgo antes de imprimir");
            }
            catch (Exception ex)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error...", ex);
            }
        }
        #endregion

        #region Buscar
        private void LimpiarBusqueda()
        {
            NUC = Paterno = Materno = Nombre = string.Empty;
            EmptyBuscarEscalaRiesgo = Visibility.Collapsed;
            LstEscalaRiesgo = null;
        }

        private async Task<List<ESCALA_RIESGO>> SegmentarResultadoBusqueda(int _Pag = 1)
        {
            if (string.IsNullOrEmpty(Paterno) && string.IsNullOrEmpty(Materno) && string.IsNullOrEmpty(Nombre) && string.IsNullOrEmpty(NUC))
                return new List<ESCALA_RIESGO>();

            Pagina = _Pag;
            //.Take((Pagina * 30)).Skip((Pagina == 1 ? 0 : ((Pagina * 30) - 30)));
            var result = await StaticSourcesViewModel.CargarDatosAsync<IEnumerable<ESCALA_RIESGO>>(() => new cEscalaRiesgo().ObtenerTodos(NUC,Paterno,Materno,Nombre, _Pag));
            if (result.Any())
            {
                Pagina++;
                SeguirCargando = true;
            }
            else
                SeguirCargando = false;

            return result.ToList();
        }
        #endregion

        #region Reporte
        private void GeneraReporte()
        {
            try
            {
                if (SelectedEscalaRiesgo == null)
                {
                    new Dialogos().ConfirmacionDialogo("Validación", "Favor de seleccionar una escala de riesgo");
                    return;
                }
                //Header
                var datosReporte = new List<cReporteDatos>();
                datosReporte.Add(new cReporteDatos()
                {
                    Encabezado1 = Parametro.ENCABEZADO1,
                    Encabezado2 = "ESCALA DE RIESGO",
                    Logo1 = Parametro.REPORTE_LOGO1,
                    Logo2 = Parametro.REPORTE_LOGO2
                });

                //Generales
                var er = new List<cReporteEscalaRiesgo>();
                string nombre_evaluador = string.Empty;
                var evaluador = new cUsuario().Obtener(SelectedEscalaRiesgo.EVALUADOR.Trim());
                if (evaluador != null)
                {
                    if (evaluador.EMPLEADO != null)
                    {
                        if (evaluador.EMPLEADO.PERSONA != null)
                        { 
                            nombre_evaluador = string.Format("{0} {1} {2}",
                                !string.IsNullOrEmpty(evaluador.EMPLEADO.PERSONA.NOMBRE) ? evaluador.EMPLEADO.PERSONA.NOMBRE.Trim() : string.Empty,
                                !string.IsNullOrEmpty(evaluador.EMPLEADO.PERSONA.PATERNO) ? evaluador.EMPLEADO.PERSONA.PATERNO.Trim() : string.Empty,
                                !string.IsNullOrEmpty(evaluador.EMPLEADO.PERSONA.MATERNO) ? evaluador.EMPLEADO.PERSONA.MATERNO.Trim() : string.Empty);
                        }
                    }
                }
                er.Add(new cReporteEscalaRiesgo() {
                    FECHA_LUGAR = string.Format("{0:dd/MM/yyyy} {1}", SelectedEscalaRiesgo.FECHA, SelectedEscalaRiesgo.LUGAR),
                    NOMBRE =  string.Format("{0} {1} {2}",
                    SelectedEscalaRiesgo.NOMBRE,
                    SelectedEscalaRiesgo.PATERNO,
                    SelectedEscalaRiesgo.MATERNO),
                    NUC = SelectedEscalaRiesgo.NUC,
                    EVALUADOR = nombre_evaluador,
                    FECHA = SelectedEscalaRiesgo.FECHA.Value.ToString("dd/MM/yyyy")
                });
               
                //Detalle
                var erd = new List<cReporteEscalaRiesgoDetalle>();
                //Datos Familiares
                erd.Add(new cReporteEscalaRiesgoDetalle()
                {
                    GRUPO = "A. DATOS FAMILIARES",
                    DESCRIPCION = "Vive con su familia consanguínea en linea recta(padres o hijos) y/o pareja",
                    VALOR = 6,
                    SELECCION = (short)(SelectedEscalaRiesgo.DATOS_FAMILIARES == 6 ? 1 : 0)
                });
                erd.Add(new cReporteEscalaRiesgoDetalle()
                {
                    GRUPO = "A. DATOS FAMILIARES",
                    DESCRIPCION = "Vive con otro familiar o amigo/a",
                    VALOR = 4,
                    SELECCION = (short)(SelectedEscalaRiesgo.DATOS_FAMILIARES == 4 ? 1 : 0)
                });
                erd.Add(new cReporteEscalaRiesgoDetalle()
                {
                    GRUPO = "A. DATOS FAMILIARES",
                    DESCRIPCION = "Vive solo/a",
                    VALOR = 1,
                    SELECCION = (short)(SelectedEscalaRiesgo.DATOS_FAMILIARES == 1 ? 1 : 0)
                });
                //Aportacion Familiar
                erd.Add(new cReporteEscalaRiesgoDetalle()
                {
                    GRUPO = "B. APORTACION ECONOMICA FAMILIAR",
                    DESCRIPCION = "Tiene dependientes económicos",
                    VALOR = 6,
                    SELECCION = (short)(SelectedEscalaRiesgo.APORTACION_ECONOMICA == 6 ? 1 : 0)
                });
                erd.Add(new cReporteEscalaRiesgoDetalle()
                {
                    GRUPO = "B. APORTACION ECONOMICA FAMILIAR",
                    DESCRIPCION = "No tiene dependientes económicos",
                    VALOR = -2,
                    SELECCION = (short)(SelectedEscalaRiesgo.APORTACION_ECONOMICA == -2 ? 1 : 0)
                });
                //Arraigo en la localidad
                erd.Add(new cReporteEscalaRiesgoDetalle()
                {
                    GRUPO = "C. ARRAIGO EN LA LOCALIDAD",
                    DESCRIPCION = "Tiene por lomenos dos años viviendo dentro dela localidad",
                    VALOR = 4,
                    SELECCION = (short)(SelectedEscalaRiesgo.ARRAIGO_LOCALIDAD == 4 ? 1 : 0)
                });
                erd.Add(new cReporteEscalaRiesgoDetalle()
                {
                    GRUPO = "C. ARRAIGO EN LA LOCALIDAD",
                    DESCRIPCION = "Tiene mas de un año pero menos de dos viviendo dentrode la localidad",
                    VALOR = 3,
                    SELECCION = (short)(SelectedEscalaRiesgo.ARRAIGO_LOCALIDAD == 3 ? 1 : 0)
                });
                erd.Add(new cReporteEscalaRiesgoDetalle()
                {
                    GRUPO = "C. ARRAIGO EN LA LOCALIDAD",
                    DESCRIPCION = "Tiene menos de un año,pero por lomenos seis meses viviendo en la localidad",
                    VALOR = 2,
                    SELECCION = (short)(SelectedEscalaRiesgo.ARRAIGO_LOCALIDAD == 2 ? 1 : 0)
                });
                erd.Add(new cReporteEscalaRiesgoDetalle()
                {
                    GRUPO = "C. ARRAIGO EN LA LOCALIDAD",
                    DESCRIPCION = "Tiene menos de seis meses viviendo en la localidad",
                    VALOR = -2,
                    SELECCION = (short)(SelectedEscalaRiesgo.ARRAIGO_LOCALIDAD == -2 ? 1 : 0)
                });
                erd.Add(new cReporteEscalaRiesgoDetalle()
                {
                    GRUPO = "C. ARRAIGO EN LA LOCALIDAD",
                    DESCRIPCION = "No vive en la localidad",
                    VALOR = -4,
                    SELECCION = (short)(SelectedEscalaRiesgo.ARRAIGO_LOCALIDAD == -4 ? 1 : 0)
                });
                //Residencia
                erd.Add(new cReporteEscalaRiesgoDetalle()
                {
                    GRUPO = "D. RESIDENCIA",
                    DESCRIPCION = "Es propietario de la vivienda en la que habita",
                    VALOR = 6,
                    SELECCION = (short)(SelectedEscalaRiesgo.RESIDENCIA == 6 ? 1 : 0)
                });
                erd.Add(new cReporteEscalaRiesgoDetalle()
                {
                    GRUPO = "D. RESIDENCIA",
                    DESCRIPCION = "Tiene mas de un año ocupando la vivienda en renta o préstamo",
                    VALOR = 4,
                    SELECCION = (short)(SelectedEscalaRiesgo.RESIDENCIA == 4 ? 1 : 0)
                });
                erd.Add(new cReporteEscalaRiesgoDetalle()
                {
                    GRUPO = "D. RESIDENCIA",
                    DESCRIPCION = "Tiene menos de un año viviendo en renta o préstamo",
                    VALOR = 2,
                    SELECCION = (short)(SelectedEscalaRiesgo.RESIDENCIA == 2 ? 1 : 0)
                });
                erd.Add(new cReporteEscalaRiesgoDetalle()
                {
                    GRUPO = "D. RESIDENCIA",
                    DESCRIPCION = "Vive en situación de calle",
                    VALOR = -6,
                    SELECCION = (short)(SelectedEscalaRiesgo.RESIDENCIA == -6 ? 1 : 0)
                });
                //Historia Laboral / Escolar
                erd.Add(new cReporteEscalaRiesgoDetalle()
                {
                    GRUPO = "E. HISTORIA LABORAL/ESCOLAR",
                    DESCRIPCION = "Tiene mas de un año con la misma actividad laboral o escolar",
                    VALOR = 6,
                    SELECCION = (short)(SelectedEscalaRiesgo.HISTORIA_LAB_ESC == 6 ? 1 : 0)
                });
                erd.Add(new cReporteEscalaRiesgoDetalle()
                {
                    GRUPO = "E. HISTORIA LABORAL/ESCOLAR",
                    DESCRIPCION = "Tiene mas de un año con actividad laboral, en diversas ocupaciones pero  continuas, o escolar",
                    VALOR = 4,
                    SELECCION = (short)(SelectedEscalaRiesgo.HISTORIA_LAB_ESC == 4 ? 1 : 0)
                });
                erd.Add(new cReporteEscalaRiesgoDetalle()
                {
                    GRUPO = "E. HISTORIA LABORAL/ESCOLAR",
                    DESCRIPCION = "Tiene de 6 a 12 meses con actividad escolar o laboral continua(un o varios trabajos)",
                    VALOR = 2,
                    SELECCION = (short)(SelectedEscalaRiesgo.HISTORIA_LAB_ESC == 2 ? 1 : 0)
                });
                erd.Add(new cReporteEscalaRiesgoDetalle()
                {
                    GRUPO = "E. HISTORIA LABORAL/ESCOLAR",
                    DESCRIPCION = "Tiene menos de seis meses con actividad laboral o escolar",
                    VALOR = -2,
                    SELECCION = (short)(SelectedEscalaRiesgo.HISTORIA_LAB_ESC == -2 ? 1 : 0)
                });
                erd.Add(new cReporteEscalaRiesgoDetalle()
                {
                    GRUPO = "E. HISTORIA LABORAL/ESCOLAR",
                    DESCRIPCION = "Se encuentra desempleado y no estudia",
                    VALOR = -6,
                    SELECCION = (short)(SelectedEscalaRiesgo.HISTORIA_LAB_ESC == -6 ? 1 : 0)
                });
                //Consumo de Sustancias
                erd.Add(new cReporteEscalaRiesgoDetalle()
                {
                    GRUPO = "F. CONSUMO DE SUSTANCIAS",
                    DESCRIPCION = "Consume drogas y/o alcohol mas de tres veces a la semana",
                    VALOR = -4,
                    SELECCION = (short)(SelectedEscalaRiesgo.CONSUMO_SUSTANCIAS == -4 ? 1 : 0)
                });
                erd.Add(new cReporteEscalaRiesgoDetalle()
                {
                    GRUPO = "F. CONSUMO DE SUSTANCIAS",
                    DESCRIPCION = "Consume drogas y/o alcohol",
                    VALOR = -2,
                    SELECCION = (short)(SelectedEscalaRiesgo.CONSUMO_SUSTANCIAS == -2 ? 1 : 0)
                });
                //Posible pena a imponer
                erd.Add(new cReporteEscalaRiesgoDetalle()
                {
                    GRUPO = "G. POSIBLE PENA A IMPONER",
                    DESCRIPCION = "No excede los 5 años  de pena privativa de libertad",
                    VALOR = 4,
                    SELECCION = (short)(SelectedEscalaRiesgo.POSIBLE_PENA_IMPONER == 4 ? 1 : 0)
                });
                erd.Add(new cReporteEscalaRiesgoDetalle()
                {
                    GRUPO = "G. POSIBLE PENA A IMPONER",
                    DESCRIPCION = "Excede los 5 años de pena privativa de libertad",
                    VALOR = -4,
                    SELECCION = (short)(SelectedEscalaRiesgo.POSIBLE_PENA_IMPONER == -4 ? 1 : 0)
                });
                //Cumplimiento de Condicions Judiciales
                erd.Add(new cReporteEscalaRiesgoDetalle()
                {
                    GRUPO = "H. CUMPLIMIENTO  DE CONDICIONES JUDICIALES",
                    DESCRIPCION = "Incumplio condiciones judiciales en procesos anteriores",
                    VALOR = -6,
                    SELECCION = (short)(SelectedEscalaRiesgo.CUMPLIMIENTO_CONDICIONES == -6 ? 1 : 0)
                });
                //Antecedentes Penales
                erd.Add(new cReporteEscalaRiesgoDetalle()
                {
                    GRUPO = "I. ANTECEDENTES PENALES",
                    DESCRIPCION = "Tiene antecedentes penales",
                    VALOR = -6,
                    SELECCION = (short)(SelectedEscalaRiesgo.ANTECEDENTES_PENALES == -6 ? 1 : 0)
                });
                erd.Add(new cReporteEscalaRiesgoDetalle()
                {
                    GRUPO = "I. ANTECEDENTES PENALES",
                    DESCRIPCION = "Tiene de 2 a 3 antecedentes penales",
                    VALOR = -5,
                    SELECCION = (short)(SelectedEscalaRiesgo.ANTECEDENTES_PENALES == -11 ? 1 : 0)
                });
                erd.Add(new cReporteEscalaRiesgoDetalle()
                {
                    GRUPO = "I. ANTECEDENTES PENALES",
                    DESCRIPCION = "Tiene masde3 antecedentes penales",
                    VALOR = -6,
                    SELECCION = (short)(SelectedEscalaRiesgo.ANTECEDENTES_PENALES == -12 ? 1 : 0)
                });
                //Datos Falsos
                erd.Add(new cReporteEscalaRiesgoDetalle()
                {
                    GRUPO = "J. DATOS FALSOS",
                    DESCRIPCION = "Mintió en la información proporcionadaen la entrevista",
                    VALOR = -6,
                    SELECCION = (short)(SelectedEscalaRiesgo.DATOS_FALSOS == -6 ? 1 : 0)
                });

                //Resultados
                var err = new List<cReporteEscalaRiesgoResultado>();
                err.Add(new cReporteEscalaRiesgoResultado() {
                    A1 = SelectedEscalaRiesgo.DATOS_FAMILIARES,
                    B1 = SelectedEscalaRiesgo.APORTACION_ECONOMICA,
                    C1 = SelectedEscalaRiesgo.ARRAIGO_LOCALIDAD,
                    D1 = SelectedEscalaRiesgo.RESIDENCIA,
                    E1 = SelectedEscalaRiesgo.HISTORIA_LAB_ESC,
                    F1 = SelectedEscalaRiesgo.CONSUMO_SUSTANCIAS,
                    G1 = SelectedEscalaRiesgo.POSIBLE_PENA_IMPONER,
                    H1 = SelectedEscalaRiesgo.CUMPLIMIENTO_CONDICIONES,
                    I1 = SelectedEscalaRiesgo.ANTECEDENTES_PENALES,
                    J1 = SelectedEscalaRiesgo.DATOS_FALSOS,
                    TOTAL = SelectedEscalaRiesgo.TOTAL
                });

                //Nivel
                var ern = new List<cReporteEscalaRiesgoNivel>();
                ern.Add(new cReporteEscalaRiesgoNivel() { NIVEL = "NULO[26 AL 32]", EN_RANGO = (SelectedEscalaRiesgo.TOTAL >= 26 && SelectedEscalaRiesgo.TOTAL <= 32) ? 1 : 0 });
                ern.Add(new cReporteEscalaRiesgoNivel() { NIVEL = "BAJO[11 AL 24]" ,EN_RANGO = (SelectedEscalaRiesgo.TOTAL >= 11 && SelectedEscalaRiesgo.TOTAL <= 24) ? 1 : 0 });
                ern.Add(new cReporteEscalaRiesgoNivel() { NIVEL = "MEDIO[10 AL -10]", EN_RANGO = (SelectedEscalaRiesgo.TOTAL >= -10 && SelectedEscalaRiesgo.TOTAL <= 10) ? 1 : 0 });
                ern.Add(new cReporteEscalaRiesgoNivel() { NIVEL = "ALTO[-11 AL -47]", EN_RANGO = (SelectedEscalaRiesgo.TOTAL >= -47 && SelectedEscalaRiesgo.TOTAL <= -11) ? 1 : 0 });

                var View = new ReporteView();
                PopUpsViewModels.ShowPopUp(this, PopUpsViewModels.TipoPopUp.OSCURECER_FONDO);
                View.Owner = PopUpsViewModels.MainWindow;
                View.Closed += (s, e) => { PopUpsViewModels.ClosePopUp(PopUpsViewModels.TipoPopUp.OSCURECER_FONDO); };

                View.ReporteViewer.LocalReport.ReportPath = "Reportes/rEscalaRiesgo.rdlc";
                View.ReporteViewer.LocalReport.DataSources.Clear();

                ReportDataSource rds1 = new ReportDataSource();
                rds1.Name = "DataSet1";
                rds1.Value = er;
                View.ReporteViewer.LocalReport.DataSources.Add(rds1);

                ReportDataSource rds2 = new ReportDataSource();
                rds2.Name = "DataSet2";
                rds2.Value = datosReporte;
                View.ReporteViewer.LocalReport.DataSources.Add(rds2);

                ReportDataSource rds3 = new ReportDataSource();
                rds3.Name = "DataSet3";
                rds3.Value = erd;
                View.ReporteViewer.LocalReport.DataSources.Add(rds3);

                ReportDataSource rds4 = new ReportDataSource();
                rds4.Name = "DataSet4";
                rds4.Value = ern;
                View.ReporteViewer.LocalReport.DataSources.Add(rds4);

                ReportDataSource rds5 = new ReportDataSource();
                rds5.Name = "DataSet5";
                rds5.Value = err;
                View.ReporteViewer.LocalReport.DataSources.Add(rds5);


                View.ReporteViewer.Refresh();
                View.ReporteViewer.RefreshReport();
                View.Show();
            }
            catch (Exception ex)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error...", ex);
            }
        }
        #endregion

        #region Privilegios
        private void ConfiguraPermisos()
        {
            try
            {
                var permisos = new cProcesoUsuario().Obtener(enumProcesos.ESCALA_RIESGO.ToString(), StaticSourcesViewModel.UsuarioLogin.Username);
                foreach (var p in permisos)
                {
                    if (p.INSERTAR == 1)
                        pInsertar = true;
                    if (p.EDITAR == 1)
                        pEditar = true;
                    if (p.CONSULTAR == 1)
                        pConsultar = true;
                    if (p.IMPRIMIR == 1)
                        pImprimir = true;
                }
            }
            catch (Exception ex)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al configurar permisos en la pantalla", ex);
            }
        }
        #endregion
    }

    public class cEscalaRiesgoCalificacion
    {
        public short? A1 { set; get; }
        public short? B1 { set; get; }
        public short? C1 { set; get; }
        public short? D1 { set; get; }
        public short? E1 { set; get; }
        public short? F1 { set; get; }
        public short? G1 { set; get; }
        public short? H1 { set; get; }
        public short? I1 { set; get; }
        public short? J1 { set; get; }
        public short? TOTAL { set; get; }
    }

    public class cEscalaRiesgoRangos
    {
        public short? ID_RAGO { set; get; }
        public string DESCR { set; get; }
        public string OBSERVACION { set; get; }
        public short? INICIO { set; get; }
        public short? FIN { set; get; }
        public short? SELECCION { set; get; }
        
    }
}
