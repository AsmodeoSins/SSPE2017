using ControlPenales;
using ControlPenales.BiometricoServiceReference;
using LinqKit;
using SSP.Controlador.Catalogo.Justicia;
using SSP.Servidor;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Configuration;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Input;

namespace ControlPenales
{
    public partial class ReportePadronEmpleadosViewModel : ValidationViewModelBase
    {

        #region Constructor
        public ReportePadronEmpleadosViewModel() { }
        #endregion

        private async void SwitchClick(Object obj)
        {
            if (!pConsultar)
            {
                ReportViewerVisible = false;
                new Dialogos().ConfirmacionDialogo("Validación", "Su usuario no tiene privilegios para realizar esta acción");
                return;
            }
           ReportViewerVisible = false;
            //StaticSourcesViewModel.CargarDatosMetodoAsync(GenerarReporte);
          //  reporte.Visible = false;
            //GenerarReporte();
           await StaticSourcesViewModel.CargarDatosMetodoAsync(GenerarReporte);
           // reporte.Visible = true;
          ReportViewerVisible = true;
           
        }
   
        #region Reporte
        private void GenerarReporte()
        {

            var centro = new cCentro().Obtener(GlobalVar.gCentro).FirstOrDefault();
            var datosReporte = new List<cReporteDatos>();
            datosReporte.Add(new cReporteDatos()
            {
                Encabezado1 = Parametro.ENCABEZADO1.Trim(),
                Encabezado2 = Parametro.ENCABEZADO2.Trim(),
                Encabezado3 = centro != null ? centro.DESCR.Trim() : string.Empty,
                Logo1 = Parametro.REPORTE_LOGO1,
                Logo2 = Parametro.REPORTE_LOGO2,
            });




            var datosReporteEmpleados = new List<PadronEmpleados>();
            var predicate = PredicateBuilder.True<EMPLEADO>();
            //var DatosEmpleados = SelectTipoEmpl == -1 ? new cEmpleado().ObtenerTodos() : new cEmpleado().ObtenerTodosReporte(GlobalVar.gCentro, SelectTipoEmpl != -1 ? SelectTipoEmpl : null).Where(w.ID_EMPLEADO == 2016150402);
            var DatosEmpleados = new cEmpleado().ObtenerTodosReporte(GlobalVar.gCentro, SelectTipoEmpl);
            if (SelectEstatus == 2)
            {
               DatosEmpleados= DatosEmpleados.Where(w => w.ESTATUS == "S");
            }
            else if (SelectEstatus == 3)
            {
                DatosEmpleados= DatosEmpleados.Where(w => w.ESTATUS == "N");
            }
            int Count = 0;
            
            foreach (var itemEmpl in DatosEmpleados)
            {
                var objEmpleado = new PadronEmpleados();
                var ApellidoPat = !string.IsNullOrEmpty(itemEmpl.PERSONA.PATERNO) ? itemEmpl.PERSONA.PATERNO.Trim() : "";
                var Apeliidomat = !string.IsNullOrEmpty(itemEmpl.PERSONA.MATERNO) ? itemEmpl.PERSONA.MATERNO.Trim() : "";
                var Nombre = !string.IsNullOrEmpty(itemEmpl.PERSONA.NOMBRE) ? itemEmpl.PERSONA.NOMBRE.Trim() : "";
                objEmpleado.nombre = ApellidoPat + " " + Apeliidomat + " " + Nombre;
                objEmpleado.domicilio = itemEmpl.PERSONA.DOMICILIO_CALLE+ " " + itemEmpl.PERSONA.DOMICILIO_NUM_EXT+ " " + itemEmpl.PERSONA.DOMICILIO_NUM_INT;

                objEmpleado.estado = itemEmpl.PERSONA.ENTIDAD.DESCR;
                objEmpleado.municipio = itemEmpl.PERSONA.MUNICIPIO.MUNICIPIO1;

                objEmpleado.tel = itemEmpl.PERSONA.TELEFONO == null ? "" : itemEmpl.PERSONA.TELEFONO;
                objEmpleado.sexo = itemEmpl.PERSONA.SEXO == null ? "" : itemEmpl.PERSONA.SEXO;
                objEmpleado.fechanac =itemEmpl.PERSONA.FEC_NACIMIENTO!=null? itemEmpl.PERSONA.FEC_NACIMIENTO.Value.ToShortDateString():"";
                objEmpleado.rfc = itemEmpl.PERSONA.RFC == null ? "" : itemEmpl.PERSONA.RFC;
                objEmpleado.curp = itemEmpl.PERSONA.CURP == null ? "" : itemEmpl.PERSONA.CURP;
                objEmpleado.fecha_reg = itemEmpl.REGISTRO_FEC==null?"":itemEmpl.REGISTRO_FEC.Value.ToShortDateString();
                objEmpleado.estatus_visita = itemEmpl.ESTATUS == "S" ? "PERMITIDO" : "NO PERMITIDO";
                objEmpleado.num_visitante = itemEmpl.PERSONA.ID_PERSONA.ToString();
                objEmpleado.observaciones = itemEmpl.OBSERV == null ? "" : itemEmpl.OBSERV;

                if (itemEmpl.PERSONA.PERSONA_BIOMETRICO != null)
                {
                    if (itemEmpl.PERSONA.PERSONA_BIOMETRICO.Any(w => w.ID_TIPO_BIOMETRICO == (short)enumTipoBiometrico.FOTO_FRENTE_REGISTRO))
                    {
                        var foto = itemEmpl.PERSONA.PERSONA_BIOMETRICO.Where(w => w.ID_TIPO_BIOMETRICO == (short)enumTipoBiometrico.FOTO_FRENTE_REGISTRO).FirstOrDefault().BIOMETRICO;
                        if (foto != null)
                        {
                            objEmpleado.imagen_frente = foto; 
                        }
                        else
                            objEmpleado.imagen_frente = new Imagenes().getImagenPerson();
                    }
                    else
                        objEmpleado.imagen_frente = new Imagenes().getImagenPerson();
                }
                else
                    objEmpleado.imagen_frente = new Imagenes().getImagenPerson();
                //if (itemEmpl.PERSONA.PERSONA_BIOMETRICO.Any(w => w.ID_TIPO_BIOMETRICO == 102 || w.ID_TIPO_BIOMETRICO == 105))
                //{
                //    objEmpleado.imagen_frente = itemEmpl.PERSONA.PERSONA_BIOMETRICO.Where(w => w.ID_TIPO_BIOMETRICO == 102 || w.ID_TIPO_BIOMETRICO == 105).FirstOrDefault().BIOMETRICO;
                  
                //}
                //else
                //{
                //    objEmpleado.imagen_frente = new Imagenes().getImagenPerson();
                //}
                datosReporteEmpleados.Add(objEmpleado);
                Count++;
               // else
                  //  objEmpleado.imagen_frente = new Imagenes().getImagenPerson();
                   // objEmpleado.imagen_frente = null;
                
            }
            if (Count == 0)
            {
                Application.Current.Dispatcher.Invoke((Action)(delegate
                {
               Reporte.Clear();
               Reporte.LocalReport.DataSources.Clear();

                }));
              //  StaticSourcesViewModel.Mensaje("Aviso", "La Busqueda no contiene Información", StaticSourcesViewModel.enumTipoMensaje.MENSAJE_INFORMACION, 5);
            }
            else
            {
                #region Reporte
                Reporte.LocalReport.ReportPath = "Reportes/rPadronEmpleado.rdlc";
                Reporte.LocalReport.DataSources.Clear();

                Microsoft.Reporting.WinForms.ReportDataSource rds1 = new Microsoft.Reporting.WinForms.ReportDataSource();
                rds1.Name = "DataSet1";
                rds1.Value = datosReporteEmpleados;
                Reporte.LocalReport.DataSources.Add(rds1);

                Microsoft.Reporting.WinForms.ReportDataSource rds2 = new Microsoft.Reporting.WinForms.ReportDataSource();
                rds2.Name = "DataSet2";
                rds2.Value = datosReporte;
                Reporte.LocalReport.DataSources.Add(rds2);

                //Microsoft.Reporting.WinForms.ReportDataSource rds2 = new Microsoft.Reporting.WinForms.ReportDataSource();
                //rds2.Name = "DataSet2";
                //rds2.Value = lstInternos;
                //Reporte.LocalReport.DataSources.Add(rds2);

                /* #region Parametros
                 Reporte.LocalReport.SetParameters(new Microsoft.Reporting.WinForms.ReportParameter("MostrarEdad", IncluirEdad ? "N" : "S"));
                 Reporte.LocalReport.SetParameters(new Microsoft.Reporting.WinForms.ReportParameter("MostrarFoto", IncluirFoto ? "N" : "S"));
                 #endregion*/
                
                Application.Current.Dispatcher.Invoke((Action)(delegate
                {
                    Reporte.RefreshReport();
                }));
        #endregion
            }

       
        }

        private void CargarListas()
        {
            try
            {
                ListTipoEmpleados = new ObservableCollection<TIPO_EMPLEADO>(new cTipoEmpleado().ObtenerTodos().OrderBy(o => o.DESCR));
                System.Windows.Application.Current.Dispatcher.Invoke((Action)(delegate
                 {
                     ListTipoEmpleados.Insert(0, new TIPO_EMPLEADO() { ID_TIPO_EMPLEADO = -1, DESCR = "SELECCIONE" });

                 }));
            }
            catch (Exception ex)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al cargar buscar por nuc", ex);
            }
        }
        
        private async void OnLoad(ReportePadronEmpleadosView Window = null)
        {
            try
            {
                ConfiguraPermisos();
                Reporte = Window.Report;
                await StaticSourcesViewModel.CargarDatosMetodoAsync(CargarListas);
                
                //ValidarFiltros();
            }
            catch (Exception ex)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al cargar pantalla", ex);
            }
        }
        #endregion

        #region Permisos
        private void ConfiguraPermisos()
        {
            try
            {
                var permisos = new cProcesoUsuario().Obtener(enumProcesos.REPORTE_PADRON_EMPLEADO.ToString(), StaticSourcesViewModel.UsuarioLogin.Username);
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
}
