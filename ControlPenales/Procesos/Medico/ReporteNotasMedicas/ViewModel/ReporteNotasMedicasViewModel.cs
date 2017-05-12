using ControlPenales.Clases;
using SSP.Controlador.Catalogo.Justicia;
using SSP.Servidor;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
//using System.Windows.Forms;
using System.Windows;
using LinqKit;
using System.Linq;
using SSP.Controlador.Catalogo.Justicia.Medico.CertificadoMedico;
using ControlPenales.BiometricoServiceReference;
using Microsoft.Reporting.WinForms;

namespace ControlPenales
{
    partial class ReporteNotasMedicasViewModel : FingerPrintScanner
    {
        public ReporteNotasMedicasViewModel() { }

        private async void WindowLoad(ReporteNotasMedicasView Window = null)
        {
            try
            {
                await StaticSourcesViewModel.CargarDatosMetodoAsync(() =>
                {
                    Reporte = Window.Report;
                    Reporte.LocalReport.SubreportProcessing += ProcesandoSubreporte;
                    ConfiguraPermisos();
                    ListTipoAtencion = new ObservableCollection<ATENCION_TIPO>(new cAtencionTipo().ObtenerTodo().Where(w => w.ESTATUS == "S"));
                    ListTipoServicioAux = new ObservableCollection<ATENCION_SERVICIO>(new cAtencionServicio().ObtenerTodo().Where(w => w.ESTATUS == "S"));
                });
                ListTipoAtencion.Insert(0, new ATENCION_TIPO()
                {
                    DESCR = "SELECCIONE",
                    ID_TIPO_ATENCION = -1,
                });
                SelectTipoAtencion = ListTipoAtencion.First(f => f.ID_TIPO_ATENCION == -1);
            }
            catch (Exception ex)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al configurar permisos en la pantalla", ex);
            }
        }

        public async void clickSwitch(Object obj)
        {
            if (obj != null ? obj is ATENCION_MEDICA : false)
            {
                var AtencionSeleccionada = (ATENCION_MEDICA)obj;

                if (AtencionSeleccionada.ID_TIPO_SERVICIO == (short)enumAtencionServicio.CERTIFICADO_NUEVO_INGRESO || AtencionSeleccionada.ID_TIPO_SERVICIO == (short)enumAtencionServicio.CERTIFICADO_INTEGRIDAD_FISICA ||
                    AtencionSeleccionada.ID_TIPO_SERVICIO == (short)enumAtencionServicio.CERTIFICADO_DENTAL_NUEVO_INGRESO || AtencionSeleccionada.ID_TIPO_SERVICIO == (short)enumAtencionServicio.CERTIFICADO_DENTISTA_INTEGRIDAD_FISICA)
                    CargarReporteCertificado();
                else if (AtencionSeleccionada.ID_TIPO_SERVICIO == (short)enumAtencionServicio.PROCEDIMIENTOS_MEDICOS)
                    CargarReporteProdecimientoMedico();
                else
                    CargarReporteConsulta();

                return;
            }
            switch (obj.ToString())
            {
                #region MENU
                case "buscar_menu":
                    if (SelectIngreso != null ? SelectIngreso.ID_INGRESO > 0 : false)
                    {
                        var x = SelectIngreso;
                        SelectedIngresoAuxiliar = x;
                    }
                    NombreBuscar = ApellidoPaternoBuscar = ApellidoMaternoBuscar = string.Empty;
                    FolioBuscar = AnioBuscar = new Nullable<int>();
                    SelectExpediente = null;
                    SelectIngreso = null;
                    ListExpediente = new RangeEnabledObservableCollection<IMPUTADO>();
                    ImagenImputado = ImagenIngreso = new Imagenes().getImagenPerson();
                    PopUpsViewModels.ShowPopUp(this, PopUpsViewModels.TipoPopUp.BUSQUEDA);
                    break;
                case "limpiar_menu":
                    ((System.Windows.Controls.ContentControl)PopUpsViewModels.MainWindow.FindName("contentControl")).Content = new ReporteNotasMedicasView();
                    ((System.Windows.Controls.ContentControl)PopUpsViewModels.MainWindow.FindName("contentControl")).DataContext = new ReporteNotasMedicasViewModel();
                    break;
                case "salir_menu":
                    PrincipalViewModel.SalirMenu();
                    break;
                case "reporte_menu":
                    #region COMMENT
                    /*try
                    {
                        if (SelectIngreso == null)
                        {
                            new Dialogos().ConfirmacionDialogo("Validación", "Debes seleccionar un imputado.");
                            return;
                        }
                        if (!SelectIngreso.ATENCION_MEDICA.Any(a => a.CERTIFICADO_MEDICO != null))
                        {
                            new Dialogos().ConfirmacionDialogo("Validación", "Debes seleccionar un imputado con un certificado de nuevo ingreso registrado.");
                            return;
                        }
                        if (Reporte == null)
                        {
                            new Dialogos().ConfirmacionDialogo("Validación", "Error al generar el reporte.");
                            return;
                        }
                        //SelectAtencionMedica = SelectIngreso.ATENCION_MEDICA.First(f => f.CERTIFICADO_MEDICO != null);
                        if (SelectAtencionMedica == null)
                        {
                            new Dialogos().ConfirmacionDialogo("Validación", "El imputado seleccionado no cuenta con un certificado medico de ingreso.");
                            return;
                        }
                        await StaticSourcesViewModel.CargarDatosMetodoAsync(() =>
                        {
                            try
                            {
                                ListLesiones = SelectAtencionMedica.CERTIFICADO_MEDICO.LESION.Select(s => new LesionesCustom { DESCR = s.DESCR, REGION = s.ANATOMIA_TOPOGRAFICA }).ToList();
                                var medico = new cUsuario().ObtenerTodos(GlobalVar.gUsr).FirstOrDefault().EMPLEADO;
                                var hoy = Fechas.GetFechaDateServer;
                                var hora = hoy.ToShortTimeString() + " hrs. Del dia ";
                                var dia = hoy.Day + " de " + hoy.ToString("MMMM").ToUpper() + " del " + hoy.Year + ".";
                                var rds1 = new Microsoft.Reporting.WinForms.ReportDataSource();
                                rds1.Name = "DataSet1";
                                var lesiones = string.Empty;
                                var i = 0;
                                foreach (var item in SelectAtencionMedica.CERTIFICADO_MEDICO.LESION)
                                {
                                    lesiones = lesiones + item.DESCR.Trim() + " EN " + item.ANATOMIA_TOPOGRAFICA.DESCR + (i == SelectAtencionMedica.CERTIFICADO_MEDICO.LESION.Count() ? string.Empty : ", ");
                                    i++;
                                }
                                var delitos = "";
                                if (SelectIngreso.CAUSA_PENAL.Any(a => a.ID_ESTATUS_CP == (short)enumEstatusCausaPenal.ACTIVO))
                                {
                                    var count = 1;
                                    foreach (var item in SelectIngreso.CAUSA_PENAL.First(a => a.ID_ESTATUS_CP == (short)enumEstatusCausaPenal.ACTIVO).CAUSA_PENAL_DELITO)
                                    {
                                        delitos = delitos + item.DESCR_DELITO + (count == SelectIngreso.CAUSA_PENAL.First(a => a.ID_ESTATUS_CP == (short)enumEstatusCausaPenal.ACTIVO).CAUSA_PENAL_DELITO.Count ? "" : ",");
                                        count++;
                                    }
                                }
                                #region OBJETO
                                rds1.Value = new List<cReporteCertificadoNuevoIngreso>()
                                {
                                    new cReporteCertificadoNuevoIngreso
                                    { 
                                        Titulo = "CERTIFICADO MÉDICO DE NUEVO INGRESO",
                                        Centro = SelectIngreso.CENTRO1 != null ? string.IsNullOrEmpty(SelectIngreso.CENTRO1.DESCR) ? string.Empty:SelectIngreso.CENTRO1.DESCR.Trim() : "",
                                        Ciudad = SelectIngreso.CENTRO1 != null ? SelectIngreso.CENTRO1.MUNICIPIO != null ? (string.IsNullOrEmpty(SelectIngreso.CENTRO1.MUNICIPIO.MUNICIPIO1) ? string.Empty : SelectIngreso.CENTRO1.MUNICIPIO.MUNICIPIO1.Trim()) : string.Empty : string.Empty,
                                        Director = SelectIngreso.CENTRO1 != null ? string.IsNullOrEmpty(SelectIngreso.CENTRO1.DIRECTOR) ? string.Empty : SelectIngreso.CENTRO1.DIRECTOR.Trim() : "",
                                        Cedula = medico.CEDULA != null ? medico.CEDULA.Trim() : "",
                                        Edad = new Fechas().CalculaEdad(SelectIngreso.IMPUTADO.NACIMIENTO_FECHA.HasValue ? SelectIngreso.IMPUTADO.NACIMIENTO_FECHA.Value : new DateTime()).ToString(),
                                        FechaIngreso = SelectIngreso.FEC_INGRESO_CERESO.HasValue ? SelectIngreso.FEC_INGRESO_CERESO.Value.ToShortDateString() : new DateTime().ToShortDateString(),
                                        FechaNacimiento = SelectIngreso.IMPUTADO.NACIMIENTO_FECHA.HasValue ? SelectIngreso.IMPUTADO.NACIMIENTO_FECHA.Value.ToShortDateString() : new DateTime().ToShortDateString(),
                                        Fecha = hora + dia,
                                        Escolaridad = SelectIngreso.ESCOLARIDAD != null ? (string.IsNullOrEmpty(SelectIngreso.ESCOLARIDAD.DESCR) ? string.Empty : SelectIngreso.ESCOLARIDAD.DESCR.Trim()) : "",
                                        Originario = SelectIngreso.IMPUTADO.NACIMIENTO_MUNICIPIO.HasValue ? 
                                            (string.IsNullOrEmpty(SelectIngreso.IMPUTADO.MUNICIPIO.MUNICIPIO1) ? 
                                                string.Empty 
                                            : SelectIngreso.IMPUTADO.MUNICIPIO.MUNICIPIO1.Trim()) + ", " +
                                            (SelectIngreso.IMPUTADO.MUNICIPIO != null ? 
                                                SelectIngreso.IMPUTADO.MUNICIPIO.ENTIDAD != null ? 
                                                    (string.IsNullOrEmpty(SelectIngreso.IMPUTADO.MUNICIPIO.ENTIDAD.DESCR) ? string.Empty : SelectIngreso.IMPUTADO.MUNICIPIO.ENTIDAD.DESCR.Trim())
                                                : string.Empty
                                            : string.Empty)
                                        : SelectIngreso.IMPUTADO.NACIMIENTO_LUGAR,
                                        NombreImputado = SelectIngreso.IMPUTADO.NOMBRE.Trim() + " " + SelectIngreso.IMPUTADO.PATERNO.Trim() + " " + (string.IsNullOrEmpty(SelectIngreso.IMPUTADO.MATERNO) ? string.Empty : SelectIngreso.IMPUTADO.MATERNO.Trim()),
                                        TipoDelito = string.IsNullOrEmpty(delitos) ? "N/A" : delitos,
                                        NombreMedico = medico.PERSONA.NOMBRE.Trim() + " " + medico.PERSONA.PATERNO.Trim() + " " + (string.IsNullOrEmpty(medico.PERSONA.MATERNO) ? string.Empty : medico.PERSONA.MATERNO.Trim()),
                                        Sexo = medico.PERSONA.SEXO == "M" ? "MASCULINO" : medico.PERSONA.SEXO == "F" ? "FEMENINO" : string.Empty, 
                                        SignosVitales_FC = SelectAtencionMedica != null ? SelectAtencionMedica.NOTA_SIGNOS_VITALES != null ? SelectAtencionMedica.NOTA_SIGNOS_VITALES.FRECUENCIA_CARDIAC : string.Empty : string.Empty,
                                        SignosVitales_FR = SelectAtencionMedica != null ? SelectAtencionMedica.NOTA_SIGNOS_VITALES != null ? SelectAtencionMedica.NOTA_SIGNOS_VITALES.FRECUENCIA_RESPIRA : string.Empty : string.Empty,
                                        SignosVitales_T = SelectAtencionMedica != null ? SelectAtencionMedica.NOTA_SIGNOS_VITALES != null ? SelectAtencionMedica.NOTA_SIGNOS_VITALES.TEMPERATURA : string.Empty : string.Empty,
                                        SignosVitales_TA = SelectAtencionMedica != null ? SelectAtencionMedica.NOTA_SIGNOS_VITALES != null ? SelectAtencionMedica.NOTA_SIGNOS_VITALES.TENSION_ARTERIAL : string.Empty : string.Empty,
                                        Logo1 = Parametro.LOGO_ESTADO,
                                        Logo2 = Parametro.REPORTE_LOGO2,
                                        Dorso = Parametro.CUERPO_DORSO,
                                        Frente = Parametro.CUERPO_FRENTE,
                                        Check = Parametro.IMAGEN_ZONA_CORPORAL,
                                        AntecedentesPatologicos = SelectAtencionMedica != null ? SelectAtencionMedica.CERTIFICADO_MEDICO != null ? SelectAtencionMedica.CERTIFICADO_MEDICO.ANTECEDENTES_PATOLOGICOS : "" : "",
                                        Interrogatorio = lesiones,
                                        PadecimientoTratamiento = SelectAtencionMedica != null ? SelectAtencionMedica.CERTIFICADO_MEDICO != null ? SelectAtencionMedica.CERTIFICADO_MEDICO.PADECIMIENTO : "" : "",
                                        Toxicomanias = SelectAtencionMedica != null ? SelectAtencionMedica.CERTIFICADO_MEDICO != null ? SelectAtencionMedica.CERTIFICADO_MEDICO.TOXICOMANIAS : "" : "",
                                        AmeritaHospitalizacion = SelectAtencionMedica != null ? SelectAtencionMedica.CERTIFICADO_MEDICO != null ? SelectAtencionMedica.CERTIFICADO_MEDICO.AMERITA_HOSPITALIZACION == "S" ? "SI" : "NO" : "NO" : "NO",
                                        Diagnostico = SelectAtencionMedica != null ? SelectAtencionMedica.CERTIFICADO_MEDICO != null ? SelectAtencionMedica.CERTIFICADO_MEDICO.DIAGNOSTICO : string.Empty : string.Empty,
                                        DiasEnSanar = SelectAtencionMedica != null?SelectAtencionMedica.CERTIFICADO_MEDICO != null ? SelectAtencionMedica.CERTIFICADO_MEDICO.TARDA_SANAR_15 == "S" ? "SI" : "NO" : "NO" : "NO",
                                        Observaciones = SelectAtencionMedica != null ? SelectAtencionMedica.CERTIFICADO_MEDICO != null ? SelectAtencionMedica.CERTIFICADO_MEDICO.OBSERVACIONES : string.Empty : string.Empty,
                                        PeligraVida = SelectAtencionMedica !=null ? SelectAtencionMedica.CERTIFICADO_MEDICO != null ? SelectAtencionMedica.CERTIFICADO_MEDICO.PELIGRA_VIDA == "S" ? "SI" : "NO" : "NO" : "NO",
                                        PlanTerapeutico = SelectAtencionMedica !=null ? SelectAtencionMedica.CERTIFICADO_MEDICO != null ? SelectAtencionMedica.CERTIFICADO_MEDICO.PLAN_TERAPEUTICO : string.Empty : string.Empty,
                                        Seguimiento = SelectAtencionMedica != null ? SelectAtencionMedica.ID_CITA_SEGUIMIENTO.HasValue ? "SI" : "NO" : "NO",
                                        F_FRONTAL = ListLesiones!=null ? !ListLesiones.Any(a => a.REGION.ID_REGION == (short)enumAnatomiaTopografica.F_FRONTAL) : true,
                                        D_ANTEBRAZO_DERECHO = ListLesiones!=null ? !ListLesiones.Any(a => a.REGION.ID_REGION == (short)enumAnatomiaTopografica.D_ANTEBRAZO_DERECHO) : true,
                                        D_ANTEBRAZO_IZQUIERDO = ListLesiones!=null ? !ListLesiones.Any(a => a.REGION.ID_REGION == (short)enumAnatomiaTopografica.D_ANTEBRAZO_IZQUIERDO) : true,
                                        D_BRAZO_POSTERIOR_DERECHO = ListLesiones!=null ? !ListLesiones.Any(a => a.REGION.ID_REGION == (short)enumAnatomiaTopografica.D_BRAZO_POSTERIOR_DERECHO) : true,
                                        D_BRAZO_POSTERIOR_IZQUIERDO = ListLesiones!=null ? !ListLesiones.Any(a => a.REGION.ID_REGION == (short)enumAnatomiaTopografica.D_BRAZO_POSTERIOR_IZQUIERDO) : true,
                                        D_CALCANEO_DERECHO = ListLesiones!=null ? !ListLesiones.Any(a => a.REGION.ID_REGION == (short)enumAnatomiaTopografica.D_CALCANEO_DERECHO) : true,
                                        D_CALCANEO_IZQUIERDA = ListLesiones!=null ? !ListLesiones.Any(a => a.REGION.ID_REGION == (short)enumAnatomiaTopografica.D_CALCANEO_IZQUIERDA) : true,
                                        D_CODO_DERECHO = ListLesiones!=null ? !ListLesiones.Any(a => a.REGION.ID_REGION == (short)enumAnatomiaTopografica.D_CODO_DERECHO) : true,
                                        D_CODO_IZQUIERDO = ListLesiones!=null ? !ListLesiones.Any(a => a.REGION.ID_REGION == (short)enumAnatomiaTopografica.D_CODO_IZQUIERDO) : true,
                                        D_COSTILLAS_COSTADO_DERECHO = ListLesiones!=null ? !ListLesiones.Any(a => a.REGION.ID_REGION == (short)enumAnatomiaTopografica.D_COSTILLAS_COSTADO_DERECHO) : true,
                                        D_COSTILLAS_COSTADO_IZQUIERDO = ListLesiones!=null ? !ListLesiones.Any(a => a.REGION.ID_REGION == (short)enumAnatomiaTopografica.D_COSTILLAS_COSTADO_IZQUIERDO) : true,
                                        D_DORSAL_DERECHA = ListLesiones != null ? !ListLesiones.Any(a => a.REGION.ID_REGION == (short)enumAnatomiaTopografica.D_DORSAL_DERECHA) : true,
                                        D_DORSAL_IZQUIERDA = ListLesiones != null ? !ListLesiones.Any(a => a.REGION.ID_REGION == (short)enumAnatomiaTopografica.D_DORSAL_IZQUIERDA) : true,
                                        D_ESCAPULAR_DERECHA = ListLesiones != null ? !ListLesiones.Any(a => a.REGION.ID_REGION == (short)enumAnatomiaTopografica.D_ESCAPULAR_DERECHA) : true,
                                        D_ESCAPULAR_IZQUIERDA = ListLesiones != null ? !ListLesiones.Any(a => a.REGION.ID_REGION == (short)enumAnatomiaTopografica.D_ESCAPULAR_IZQUIERDA) : true,
                                        D_FALANGES_POSTERIORES_DERECHA = ListLesiones != null ? !ListLesiones.Any(a => a.REGION.ID_REGION == (short)enumAnatomiaTopografica.D_FALANGES_POSTERIORES_DERECHA) : true,
                                        D_FALANGES_POSTERIORES_IZQUIERDA = ListLesiones != null ? !ListLesiones.Any(a => a.REGION.ID_REGION == (short)enumAnatomiaTopografica.D_FALANGES_POSTERIORES_IZQUIERDA) : true,
                                        D_GLUTEA_DERECHA = ListLesiones != null ? !ListLesiones.Any(a => a.REGION.ID_REGION == (short)enumAnatomiaTopografica.D_GLUTEA_DERECHA) : true,
                                        D_GLUTEA_IZQUIERDA = ListLesiones != null ? !ListLesiones.Any(a => a.REGION.ID_REGION == (short)enumAnatomiaTopografica.D_GLUTEA_IZQUIERDA) : true,
                                        D_LUMBAR_RENAL_DERECHA = ListLesiones != null ? !ListLesiones.Any(a => a.REGION.ID_REGION == (short)enumAnatomiaTopografica.D_LUMBAR_RENAL_DERECHA) : true,
                                        D_LUMBAR_RENAL_IZQUIERDA = ListLesiones != null ? !ListLesiones.Any(a => a.REGION.ID_REGION == (short)enumAnatomiaTopografica.D_METATARZIANA_PLANTA_DERECHA) : true,
                                        D_METATARZIANA_PLANTA_DERECHA = ListLesiones != null ? !ListLesiones.Any(a => a.REGION.ID_REGION == (short)enumAnatomiaTopografica.D_METATARZIANA_PLANTA_DERECHA) : true,
                                        D_METATARZIANA_PLANTA_IZQUIERDA = ListLesiones != null ? !ListLesiones.Any(a => a.REGION.ID_REGION == (short)enumAnatomiaTopografica.D_METATARZIANA_PLANTA_IZQUIERDA) : true,
                                        D_MUÑECA_POSTERIOR_DERECHA = ListLesiones != null ? !ListLesiones.Any(a => a.REGION.ID_REGION == (short)enumAnatomiaTopografica.D_MUÑECA_POSTERIOR_DERECHA) : true,
                                        D_MUÑECA_POSTERIOR_IZQUIERDA = ListLesiones != null ? !ListLesiones.Any(a => a.REGION.ID_REGION == (short)enumAnatomiaTopografica.D_MUÑECA_POSTERIOR_IZQUIERDA) : true,
                                        D_MUSLO_POSTERIOR_DERECHO = ListLesiones != null ? !ListLesiones.Any(a => a.REGION.ID_REGION == (short)enumAnatomiaTopografica.D_MUSLO_POSTERIOR_DERECHO) : true,
                                        D_MUSLO_POSTERIOR_IZQUIERDO = ListLesiones != null ? !ListLesiones.Any(a => a.REGION.ID_REGION == (short)enumAnatomiaTopografica.D_MUSLO_POSTERIOR_IZQUIERDO) : true,
                                        D_OCCIPITAL_NUCA = ListLesiones != null ? !ListLesiones.Any(a => a.REGION.ID_REGION == (short)enumAnatomiaTopografica.D_OCCIPITAL_NUCA) : true,
                                        D_OREJA_POSTERIOR_DERECHA = ListLesiones != null ? !ListLesiones.Any(a => a.REGION.ID_REGION == (short)enumAnatomiaTopografica.D_OREJA_POSTERIOR_DERECHA) : true,
                                        D_OREJA_POSTERIOR_IZQUIERDA = ListLesiones != null ? !ListLesiones.Any(a => a.REGION.ID_REGION == (short)enumAnatomiaTopografica.D_OREJA_POSTERIOR_IZQUIERDA) : true,
                                        D_PANTORRILLA_DERECHA = ListLesiones!=null ? !ListLesiones.Any(a => a.REGION.ID_REGION == (short)enumAnatomiaTopografica.D_PANTORRILLA_DERECHA) : true,
                                        D_PANTORRILLA_IZQUIERDA = ListLesiones!=null ? !ListLesiones.Any(a => a.REGION.ID_REGION == (short)enumAnatomiaTopografica.D_PANTORRILLA_IZQUIERDA) : true,
                                        D_POPLITEA_DERECHA = ListLesiones!=null ? !ListLesiones.Any(a => a.REGION.ID_REGION == (short)enumAnatomiaTopografica.D_POPLITEA_DERECHA) : true,
                                        D_POPLITEA_IZQUIERDA = ListLesiones!=null ? !ListLesiones.Any(a => a.REGION.ID_REGION == (short)enumAnatomiaTopografica.D_POPLITEA_IZQUIERDA) : true,
                                        D_POSTERIOR_CABEZA = ListLesiones!=null ? !ListLesiones.Any(a => a.REGION.ID_REGION == (short)enumAnatomiaTopografica.D_POSTERIOR_CABEZA) : true,
                                        D_POSTERIOR_CUELLO = ListLesiones!=null ? !ListLesiones.Any(a => a.REGION.ID_REGION == (short)enumAnatomiaTopografica.D_POSTERIOR_CUELLO) : true,
                                        D_POSTERIOR_ENTREPIERNA_DERECHA = ListLesiones!=null ? !ListLesiones.Any(a => a.REGION.ID_REGION == (short)enumAnatomiaTopografica.D_POSTERIOR_ENTREPIERNA_DERECHA) : true,
                                        D_POSTERIOR_ENTREPIERNA_IZQUIERDA = ListLesiones!=null ? !ListLesiones.Any(a => a.REGION.ID_REGION == (short)enumAnatomiaTopografica.D_POSTERIOR_ENTREPIERNA_IZQUIERDA) : true,
                                        D_POSTERIOR_HOMBRO_DERECHO = ListLesiones!=null ? !ListLesiones.Any(a => a.REGION.ID_REGION == (short)enumAnatomiaTopografica.D_POSTERIOR_HOMBRO_DERECHO) : true,
                                        D_POSTERIOR_HOMBRO_IZQUIERDO = ListLesiones!=null ? !ListLesiones.Any(a => a.REGION.ID_REGION == (short)enumAnatomiaTopografica.D_POSTERIOR_HOMBRO_IZQUIERDO) : true,
                                        D_SACRA = ListLesiones != null ? !ListLesiones.Any(a => a.REGION.ID_REGION == (short)enumAnatomiaTopografica.D_SACRA) : true,
                                        D_TOBILLO_DERECHO = ListLesiones!=null ? !ListLesiones.Any(a => a.REGION.ID_REGION == (short)enumAnatomiaTopografica.D_TOBILLO_DERECHO) : true,
                                        D_TOBILLO_IZQUIERDO = ListLesiones!=null ? !ListLesiones.Any(a => a.REGION.ID_REGION == (short)enumAnatomiaTopografica.D_TOBILLO_IZQUIERDO) : true,
                                        D_TORACICA_DORSAL = ListLesiones!=null ? !ListLesiones.Any(a => a.REGION.ID_REGION == (short)enumAnatomiaTopografica.D_TORACICA_DORSAL) : true,
                                        D_VERTEBRAL_CERVICAL = ListLesiones!=null ? !ListLesiones.Any(a => a.REGION.ID_REGION == (short)enumAnatomiaTopografica.D_VERTEBRAL_CERVICAL) : true,
                                        D_VERTEBRAL_LUMBAR = ListLesiones!=null ? !ListLesiones.Any(a => a.REGION.ID_REGION == (short)enumAnatomiaTopografica.D_VERTEBRAL_LUMBAR) : true,
                                        D_VERTEBRAL_TORACICA = ListLesiones!=null ? !ListLesiones.Any(a => a.REGION.ID_REGION == (short)enumAnatomiaTopografica.D_VERTEBRAL_TORACICA) : true,
                                        F_ANTEBRAZO_DERECHO = ListLesiones!=null ? !ListLesiones.Any(a => a.REGION.ID_REGION == (short)enumAnatomiaTopografica.F_ANTEBRAZO_DERECHO) : true,
                                        F_ANTEBRAZO_IZQUIERDO = ListLesiones!=null ? !ListLesiones.Any(a => a.REGION.ID_REGION == (short)enumAnatomiaTopografica.F_ANTEBRAZO_IZQUIERDO) : true,
                                        F_ANTERIOR_CUELLO = ListLesiones!=null ? !ListLesiones.Any(a => a.REGION.ID_REGION == (short)enumAnatomiaTopografica.F_ANTERIOR_CUELLO) : true,
                                        F_MUÑECA_ANTERIOR_DERECHA = ListLesiones!=null ? !ListLesiones.Any(a => a.REGION.ID_REGION == (short)enumAnatomiaTopografica.F_MUÑECA_ANTERIOR_DERECHA) : true,
                                        F_MUÑECA_ANTERIOR_IZQUIERDA = ListLesiones!=null ? !ListLesiones.Any(a => a.REGION.ID_REGION == (short)enumAnatomiaTopografica.F_MUÑECA_ANTERIOR_IZQUIERDA) : true,
                                        F_AXILA_DERECHA = ListLesiones!=null ? !ListLesiones.Any(a => a.REGION.ID_REGION == (short)enumAnatomiaTopografica.F_AXILA_DERECHA) : true,
                                        F_AXILA_IZQUIERDA = ListLesiones!=null ? !ListLesiones.Any(a => a.REGION.ID_REGION == (short)enumAnatomiaTopografica.F_AXILA_IZQUIERDA) : true,
                                        F_BAJO_VIENTRE_HIPOGASTRIO = ListLesiones!=null ? !ListLesiones.Any(a => a.REGION.ID_REGION == (short)enumAnatomiaTopografica.F_BAJO_VIENTRE_HIPOGASTRIO) : true,
                                        F_BRAZO_DERECHO = ListLesiones!=null ? !ListLesiones.Any(a => a.REGION.ID_REGION == (short)enumAnatomiaTopografica.F_BRAZO_DERECHO) : true,
                                        F_BRAZO_IZQUIERDO = ListLesiones!=null ? !ListLesiones.Any(a => a.REGION.ID_REGION == (short)enumAnatomiaTopografica.F_BRAZO_IZQUIERDO) : true,
                                        F_CARA_DERECHA = ListLesiones!=null ? !ListLesiones.Any(a => a.REGION.ID_REGION == (short)enumAnatomiaTopografica.F_CLAVICULAR_DERECHA) : true,
                                        F_CARA_IZQUIERDA = ListLesiones!=null ? !ListLesiones.Any(a => a.REGION.ID_REGION == (short)enumAnatomiaTopografica.F_CARA_IZQUIERDA) : true,
                                        F_CLAVICULAR_DERECHA = ListLesiones!=null ? !ListLesiones.Any(a => a.REGION.ID_REGION == (short)enumAnatomiaTopografica.F_CLAVICULAR_DERECHA) : true,
                                        F_CLAVICULAR_IZQUIERDA = ListLesiones!=null ? !ListLesiones.Any(a => a.REGION.ID_REGION == (short)enumAnatomiaTopografica.F_CLAVICULAR_IZQUIERDA) : true,
                                        F_CODO_DERECHO = ListLesiones!=null ? !ListLesiones.Any(a => a.REGION.ID_REGION == (short)enumAnatomiaTopografica.F_CODO_DERECHO) : true,
                                        F_CODO_IZQUIERDO = ListLesiones!=null ? !ListLesiones.Any(a => a.REGION.ID_REGION == (short)enumAnatomiaTopografica.F_CODO_IZQUIERDO) : true,
                                        F_ENTREPIERNA_ANTERIOR_DERECHA = ListLesiones!=null ? !ListLesiones.Any(a => a.REGION.ID_REGION == (short)enumAnatomiaTopografica.F_ENTREPIERNA_ANTERIOR_DERECHA) : true,
                                        F_ENTREPIERNA_ANTERIOR_IZQUIERDA = ListLesiones!=null ? !ListLesiones.Any(a => a.REGION.ID_REGION == (short)enumAnatomiaTopografica.F_ENTREPIERNA_ANTERIOR_IZQUIERDA) : true,
                                        F_EPIGASTRIO = ListLesiones != null ? !ListLesiones.Any(a => a.REGION.ID_REGION == (short)enumAnatomiaTopografica.F_EPIGASTRIO) : true,
                                        F_ESCROTO = ListLesiones != null ? !ListLesiones.Any(a => a.REGION.ID_REGION == (short)enumAnatomiaTopografica.F_ESCROTO) : true,
                                        F_ESPINILLA_DERECHA = ListLesiones != null ? !ListLesiones.Any(a => a.REGION.ID_REGION == (short)enumAnatomiaTopografica.F_ESPINILLA_DERECHA) : true,
                                        F_ESPINILLA_IZQUIERDA = ListLesiones != null ? !ListLesiones.Any(a => a.REGION.ID_REGION == (short)enumAnatomiaTopografica.F_ESPINILLA_IZQUIERDA) : true,
                                        F_FALANGES_MANO_DERECHA = ListLesiones != null ? !ListLesiones.Any(a => a.REGION.ID_REGION == (short)enumAnatomiaTopografica.F_FALANGES_MANO_DERECHA) : true,
                                        F_FALANGES_MANO_IZQUIERDA = ListLesiones != null ? !ListLesiones.Any(a => a.REGION.ID_REGION == (short)enumAnatomiaTopografica.F_FALANGES_MANO_IZQUIERDA) : true,
                                        F_FALANGES_PIE_DERECHO = ListLesiones != null ? !ListLesiones.Any(a => a.REGION.ID_REGION == (short)enumAnatomiaTopografica.F_FALANGES_PIE_DERECHO) : true,
                                        F_FALANGES_PIE_IZQUIERDO = ListLesiones != null ? !ListLesiones.Any(a => a.REGION.ID_REGION == (short)enumAnatomiaTopografica.F_FALANGES_PIE_IZQUIERDO) : true,
                                        F_HIPOCONDRIA_DERECHA = ListLesiones != null ? !ListLesiones.Any(a => a.REGION.ID_REGION == (short)enumAnatomiaTopografica.F_HIPOCONDRIA_DERECHA) : true,
                                        F_HIPOCONDRIA_IZQUIERDA = ListLesiones != null ? !ListLesiones.Any(a => a.REGION.ID_REGION == (short)enumAnatomiaTopografica.F_HIPOCONDRIA_IZQUIERDA) : true,
                                        F_HOMBRO_DERECHO = ListLesiones != null ? !ListLesiones.Any(a => a.REGION.ID_REGION == (short)enumAnatomiaTopografica.F_HOMBRO_DERECHO) : true,
                                        F_HOMBRO_IZQUIERDO = ListLesiones != null ? !ListLesiones.Any(a => a.REGION.ID_REGION == (short)enumAnatomiaTopografica.F_HOMBRO_IZQUIERDO) : true,
                                        F_INGUINAL_DERECHA = ListLesiones != null ? !ListLesiones.Any(a => a.REGION.ID_REGION == (short)enumAnatomiaTopografica.F_INGUINAL_DERECHA) : true,
                                        F_INGUINAL_IZQUIERDA = ListLesiones != null ? !ListLesiones.Any(a => a.REGION.ID_REGION == (short)enumAnatomiaTopografica.F_INGUINAL_IZQUIERDA) : true,
                                        F_LATERAL_CABEZA_DERECHO = ListLesiones != null ? !ListLesiones.Any(a => a.REGION.ID_REGION == (short)enumAnatomiaTopografica.F_LATERAL_CABEZA_DERECHO) : true,
                                        F_LATERAL_CABEZA_IZQUIERDO = ListLesiones != null ? !ListLesiones.Any(a => a.REGION.ID_REGION == (short)enumAnatomiaTopografica.F_LATERAL_CABEZA_IZQUIERDO) : true,
                                        F_MANDIBULA = ListLesiones != null ? !ListLesiones.Any(a => a.REGION.ID_REGION == (short)enumAnatomiaTopografica.F_MANDIBULA) : true,
                                        F_METACARPIANOS_DERECHA = ListLesiones != null ? !ListLesiones.Any(a => a.REGION.ID_REGION == (short)enumAnatomiaTopografica.F_METACARPIANOS_DERECHA) : true,
                                        F_METACARPIANOS_IZQUIERDA = ListLesiones != null ? !ListLesiones.Any(a => a.REGION.ID_REGION == (short)enumAnatomiaTopografica.F_METACARPIANOS_IZQUIERDA) : true,
                                        F_METATARZIANA_DORSAL_DERECHO = ListLesiones != null ? !ListLesiones.Any(a => a.REGION.ID_REGION == (short)enumAnatomiaTopografica.F_METATARZIANA_DORSAL_DERECHO) : true,
                                        F_METATARZIANA_DORSAL_IZQUIERDO = ListLesiones != null ? !ListLesiones.Any(a => a.REGION.ID_REGION == (short)enumAnatomiaTopografica.F_METATARZIANA_DORSAL_IZQUIERDO) : true,
                                        F_MUSLO_ANTERIOR_DERECHO = ListLesiones!=null ? !ListLesiones.Any(a => a.REGION.ID_REGION == (short)enumAnatomiaTopografica.F_MUSLO_ANTERIOR_DERECHO) : true,
                                        F_MUSLO_ANTERIOR_IZQUIERDO = ListLesiones!=null ? !ListLesiones.Any(a => a.REGION.ID_REGION == (short)enumAnatomiaTopografica.F_MUSLO_ANTERIOR_IZQUIERDO) : true,
                                        F_NARIZ = ListLesiones != null ? !ListLesiones.Any(a => a.REGION.ID_REGION == (short)enumAnatomiaTopografica.F_NARIZ) : true,
                                        F_ORBITAL_DERECHO = ListLesiones != null ? !ListLesiones.Any(a => a.REGION.ID_REGION == (short)enumAnatomiaTopografica.F_ORBITAL_DERECHO) : true,
                                        F_ORBITAL_IZQUIERDO = ListLesiones != null ? !ListLesiones.Any(a => a.REGION.ID_REGION == (short)enumAnatomiaTopografica.F_ORBITAL_IZQUIERDO) : true,
                                        F_OREJA_DERECHA = ListLesiones != null ? !ListLesiones.Any(a => a.REGION.ID_REGION == (short)enumAnatomiaTopografica.F_OREJA_DERECHA) : true,
                                        F_OREJA_IZQUIERDA = ListLesiones != null ? !ListLesiones.Any(a => a.REGION.ID_REGION == (short)enumAnatomiaTopografica.F_OREJA_IZQUIERDA) : true,
                                        F_PENE_VAGINA = ListLesiones != null ? !ListLesiones.Any(a => a.REGION.ID_REGION == (short)enumAnatomiaTopografica.F_PENE_VAGINA) : true,
                                        F_PEZON_DERECHO = ListLesiones != null ? !ListLesiones.Any(a => a.REGION.ID_REGION == (short)enumAnatomiaTopografica.F_PEZON_DERECHO) : true,
                                        F_PEZON_IZQUIERDO = ListLesiones != null ? !ListLesiones.Any(a => a.REGION.ID_REGION == (short)enumAnatomiaTopografica.F_PEZON_IZQUIERDO) : true,
                                        F_RODILLA_DERECHA = ListLesiones != null ? !ListLesiones.Any(a => a.REGION.ID_REGION == (short)enumAnatomiaTopografica.F_RODILLA_DERECHA) : true,
                                        F_RODILLA_IZQUIERDA = ListLesiones != null ? !ListLesiones.Any(a => a.REGION.ID_REGION == (short)enumAnatomiaTopografica.F_RODILLA_IZQUIERDA) : true,
                                        F_TOBILLO_DERECHO = ListLesiones != null ? !ListLesiones.Any(a => a.REGION.ID_REGION == (short)enumAnatomiaTopografica.F_TOBILLO_DERECHO) : true,
                                        F_TOBILLO_IZQUIERDO = ListLesiones != null ? !ListLesiones.Any(a => a.REGION.ID_REGION == (short)enumAnatomiaTopografica.F_TOBILLO_IZQUIERDO) : true,
                                        F_TORAX_CENTRAL = ListLesiones != null ? !ListLesiones.Any(a => a.REGION.ID_REGION == (short)enumAnatomiaTopografica.F_TORAX_CENTRAL) : true,
                                        F_TORAX_DERECHO=ListLesiones!=null ? !ListLesiones.Any(a => a.REGION.ID_REGION == (short)enumAnatomiaTopografica.F_TORAX_DERECHO) : true,
                                        F_TORAX_IZQUIERDO=ListLesiones!=null ? !ListLesiones.Any(a => a.REGION.ID_REGION == (short)enumAnatomiaTopografica.F_TORAX_IZQUIERDO) : true,
                                        F_UMBILICAL_MESOGASTIO = ListLesiones!=null ? !ListLesiones.Any(a => a.REGION.ID_REGION == (short)enumAnatomiaTopografica.F_UMBILICAL_MESOGASTIO) : true,
                                        F_VACIO_DERECHA = ListLesiones!=null ? !ListLesiones.Any(a => a.REGION.ID_REGION == (short)enumAnatomiaTopografica.F_VACIO_DERECHA) : true,
                                        F_VACIO_IZQUIERDA = ListLesiones!=null ? !ListLesiones.Any(a => a.REGION.ID_REGION == (short)enumAnatomiaTopografica.F_VACIO_IZQUIERDA) : true,
                                        F_VERTICE_CABEZA = ListLesiones!=null ? !ListLesiones.Any(a => a.REGION.ID_REGION == (short)enumAnatomiaTopografica.F_VERTICE_CABEZA) : true
                                    }
                                };
                                #endregion
                                Application.Current.Dispatcher.Invoke((Action)(delegate
                                {
                                    Reporte.LocalReport.ReportPath = "Reportes/rCertificadoNuevoIngreso.rdlc";
                                    Reporte.LocalReport.DataSources.Clear();
                                    Reporte.LocalReport.DataSources.Add(rds1);
                                    Reporte.ShowExportButton = false;
                                    Reporte.SetDisplayMode(Microsoft.Reporting.WinForms.DisplayMode.PrintLayout);
                                    Reporte.ZoomMode = Microsoft.Reporting.WinForms.ZoomMode.Percent;
                                    Reporte.ZoomPercent = 100;
                                    Reporte.RefreshReport();
                                }));
                            }
                            catch (Exception ex)
                            {
                                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al cargar la información del certificado seleccionado.", ex);
                            }
                        });
                    }
                    catch (Exception ex)
                    {
                        StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al cargar la información del certificado seleccionado.", ex);
                    }*/
                    #endregion
                    break;
                #endregion

                #region BUSCAR IMPUTADOS
                case "nueva_busqueda":
                    NombreBuscar = ApellidoPaternoBuscar = ApellidoMaternoBuscar = string.Empty;
                    FolioBuscar = AnioBuscar = new int?();
                    SelectExpediente = null;
                    //FiltrarEnabled = false;
                    SelectIngreso = null;
                    ListExpediente = new RangeEnabledObservableCollection<IMPUTADO>();
                    ImagenImputado = ImagenIngreso = new Imagenes().getImagenPerson();
                    break;
                case "buscar_visible":
                    if (SelectIngreso != null ? SelectIngreso.ID_INGRESO > 0 : false)
                    {
                        var x = SelectIngreso;
                        SelectedIngresoAuxiliar = x;
                    }

                    PopUpsViewModels.ShowPopUp(this, PopUpsViewModels.TipoPopUp.BUSQUEDA);
                    break;
                case "buscar_salir":
                    if (SelectedIngresoAuxiliar != null ? SelectedIngresoAuxiliar != SelectIngreso : false)
                    {
                        var x = SelectedIngresoAuxiliar;
                        SelectIngreso = x;
                    }
                    else
                        ImagenIngreso = new Imagenes().getImagenPerson();
                    PopUpsViewModels.ClosePopUp(PopUpsViewModels.TipoPopUp.BUSQUEDA);
                    break;
                case "buscar_seleccionar":
                    try
                    {
                        if (SelectIngreso == null)
                        {
                            new Dialogos().ConfirmacionDialogo("Validación", "Favor de seleccionar un ingreso");
                            return;
                        };
                        if (SelectIngreso.ID_UB_CENTRO != GlobalVar.gCentro)
                        {
                            new Dialogos().ConfirmacionDialogo("Validación", "El ingreso seleccionado no pertenece a su centro.");
                            return;
                        };
                        if (Parametro.ESTATUS_ADMINISTRATIVO_INACT.Any(a => a.HasValue ? a.Value == SelectIngreso.ID_ESTATUS_ADMINISTRATIVO : false))
                        {
                            new Dialogos().ConfirmacionDialogo("Notificación!", "Debes seleccionar un ingreso activo.");
                            return;
                        }
                        if (!SelectIngreso.ATENCION_MEDICA.Any())
                        {
                            new Dialogos().ConfirmacionDialogo("Notificación!", "El imputado seleccionado no tiene ninguna nota médica registrada.");
                            return;
                        }
                        //FiltrarEnabled = true;
                        await StaticSourcesViewModel.CargarDatosMetodoAsync(GetExpediente);
                        SelectTipoAtencion = ListTipoAtencion.First(f => f.ID_TIPO_ATENCION == (short)enumAtencionTipo.CONSULTA_MEDICA);
                        clickSwitch("cargar_listado");
                        PopUpsViewModels.ClosePopUp(PopUpsViewModels.TipoPopUp.BUSQUEDA);
                        //PopUpsViewModels.ShowPopUp(this, PopUpsViewModels.TipoPopUp.OSCURECER_FONDO);
                        PopUpsViewModels.ClosePopUp(PopUpsViewModels.TipoPopUp.OSCURECER_FONDO);
                    }
                    catch (Exception ex)
                    {
                        StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al cargar los datos del imputado seleccionado.", ex);
                    }
                    break;
                #endregion

                case "cargar_listado":
                    if (SelectTipoAtencion != null ? SelectTipoAtencion.ID_TIPO_ATENCION <= 0 : true)
                    {
                        new Dialogos().ConfirmacionDialogo("Notificación!", "Debe seleccionar el tipo de atención recibida.");
                        return;
                    }
                    var servicio = SelectTipoServicio != null ? SelectTipoServicio.ID_TIPO_SERVICIO > 0 ? SelectTipoServicio.ID_TIPO_SERVICIO : 0 : 0;
                    await StaticSourcesViewModel.CargarDatosMetodoAsync(() =>
                    {
                        var query = new cAtencionMedica().GetData(g => g.ATENCION_FEC.HasValue ?
                            SelectTipoAtencion.ID_TIPO_ATENCION == g.ID_TIPO_ATENCION ?
                                (g.ID_ANIO == SelectIngreso.ID_ANIO && g.ID_CENTRO == SelectIngreso.ID_CENTRO && g.ID_IMPUTADO == SelectIngreso.ID_IMPUTADO && g.ID_INGRESO == SelectIngreso.ID_INGRESO) ?
                                    servicio > 0 ?
                                        g.ID_TIPO_SERVICIO == servicio ?
                                            servicio == (short)enumAtencionServicio.PROCEDIMIENTOS_MEDICOS ?
                                                true
                                            : g.NOTA_MEDICA != null
                                        : false
                                    : true
                                : false
                            : false
                        : false);
                        if (query.Any())
                            ListNotasMedicas = new ObservableCollection<ATENCION_MEDICA>(query.OrderBy(o => o.ATENCION_FEC).ToList().Select(s => new ATENCION_MEDICA
                            {
                                ATENCION_CITA = s.ATENCION_CITA,
                                ATENCION_CITA1 = s.ATENCION_CITA1,
                                ATENCION_FEC = s.ATENCION_FEC,
                                ATENCION_SERVICIO = s.ATENCION_SERVICIO,
                                CERTIFICADO_MEDICO = s.CERTIFICADO_MEDICO,
                                EXCARCELACION = s.EXCARCELACION,
                                EXCARCELACION1 = s.EXCARCELACION1,
                                HOSPITALIZACION = s.HOSPITALIZACION,
                                ID_ANIO = s.ID_ANIO,
                                ID_ATENCION_MEDICA = s.ID_ATENCION_MEDICA,
                                ID_CENTRO = s.ID_CENTRO,
                                ID_CENTRO_UBI = s.ID_CENTRO_UBI,
                                ID_CITA_SEGUIMIENTO = s.ID_CITA_SEGUIMIENTO,
                                ID_HOSPITA = s.ID_HOSPITA,
                                ID_IMPUTADO = s.ID_IMPUTADO,
                                ID_INGRESO = s.ID_INGRESO,
                                ID_TIPO_ATENCION = s.ID_TIPO_ATENCION,
                                ID_TIPO_SERVICIO = s.ID_TIPO_SERVICIO,
                                INCIDENTE = s.INCIDENTE,
                                INGRESO = s.INGRESO,
                                NOTA_INTERCONSULTA = s.NOTA_INTERCONSULTA,
                                NOTA_MEDICA = s.NOTA_MEDICA != null ?
                                    s.NOTA_MEDICA
                                : (servicio == (short)enumAtencionServicio.PROCEDIMIENTOS_MEDICOS ?
                                    s.ATENCION_CITA.Any() ?
                                        new NOTA_MEDICA { PERSONA = s.ATENCION_CITA.FirstOrDefault().PERSONA }
                                    : null
                                : null),
                                NOTA_POST_OPERATOR = s.NOTA_POST_OPERATOR,
                                NOTA_PRE_ANESTECIC = s.NOTA_PRE_ANESTECIC,
                                NOTA_PRE_OPERATORI = s.NOTA_PRE_OPERATORI,
                                NOTA_REFERENCIA_TR = s.NOTA_REFERENCIA_TR,
                                NOTA_SIGNOS_VITALES = s.NOTA_SIGNOS_VITALES,
                                NOTA_URGENCIA = s.NOTA_URGENCIA,
                                PROC_ATENCION_MEDICA = s.PROC_ATENCION_MEDICA,
                                RECETA_MEDICA = s.RECETA_MEDICA,
                                TRASLADO_DETALLE = s.TRASLADO_DETALLE,
                            }));
                        EmptyVisible = ListNotasMedicas != null ? ListNotasMedicas.Any() ? Visibility.Collapsed : Visibility.Visible : Visibility.Visible;
                    });
                    break;
            }
        }

        private async void EnterExpediente(Object obj)
        {
            try
            {
                if (!PConsultar)
                {
                    (new Dialogos()).ConfirmacionDialogo("Advertencia!", "No tienes permisos para hacer consultas.");
                    return;
                }
                if (obj == null)
                {
                    (new Dialogos()).ConfirmacionDialogo("Advertencia!", "Ocurrió un error al realizar la búsqueda de imputados.");
                    return;
                }
                NombreBuscar = NombreD;
                ApellidoPaternoBuscar = PaternoD;
                ApellidoMaternoBuscar = MaternoD;
                AnioBuscar = AnioD;
                FolioBuscar = FolioD;
                if (!AnioD.HasValue || !FolioD.HasValue)
                {
                    ListExpediente = new RangeEnabledObservableCollection<IMPUTADO>();
                    ListExpediente.InsertRange(await SegmentarResultadoBusqueda());
                    PopUpsViewModels.ShowPopUp(this, PopUpsViewModels.TipoPopUp.BUSQUEDA);
                    return;
                }
                ListExpediente = new RangeEnabledObservableCollection<IMPUTADO>();
                ListExpediente.InsertRange(await SegmentarResultadoBusqueda());
                if (ListExpediente.Count == 1)
                {
                    if (string.IsNullOrEmpty(NombreBuscar) && string.IsNullOrEmpty(ApellidoPaternoBuscar) && string.IsNullOrEmpty(ApellidoMaternoBuscar))
                    {
                        SelectExpediente = ListExpediente.OrderBy(o => o.ID_ANIO).ThenBy(t => t.ID_IMPUTADO).FirstOrDefault();
                        if (SelectIngreso == null)
                        {
                            new Dialogos().ConfirmacionDialogo("Validación", "Favor de seleccionar un ingreso");
                            return;
                        };
                        if (SelectIngreso.ID_UB_CENTRO != GlobalVar.gCentro)
                        {
                            new Dialogos().ConfirmacionDialogo("Validación", "El ingreso seleccionado no pertenece a su centro.");
                            SelectIngreso = null;
                            selectExpediente = null;
                            ImagenIngreso = new Imagenes().getImagenPerson();
                            return;
                        };
                        if (Parametro.ESTATUS_ADMINISTRATIVO_INACT.Any(a => a.HasValue ? a.Value == SelectIngreso.ID_ESTATUS_ADMINISTRATIVO : false))
                        {
                            new Dialogos().ConfirmacionDialogo("Notificación!", "Debes seleccionar un ingreso activo.");
                            SelectIngreso = null;
                            selectExpediente = null;
                            ImagenIngreso = new Imagenes().getImagenPerson();
                            return;
                        }
                        if (!SelectIngreso.ATENCION_MEDICA.Any())
                        {
                            new Dialogos().ConfirmacionDialogo("Notificación!", "El imputado seleccionado no tiene ninguna nota médica registrada.");
                            SelectIngreso = null;
                            selectExpediente = null;
                            ImagenIngreso = new Imagenes().getImagenPerson();
                            return;
                        }
                        await StaticSourcesViewModel.CargarDatosMetodoAsync(GetExpediente);
                        SelectTipoAtencion = ListTipoAtencion.First(f => f.ID_TIPO_ATENCION == (short)enumAtencionTipo.CONSULTA_MEDICA);
                        clickSwitch("cargar_listado");
                    }
                }
                else if (ListExpediente.Count == 0 && (!string.IsNullOrEmpty(NombreBuscar) || !string.IsNullOrEmpty(ApellidoPaternoBuscar) || !string.IsNullOrEmpty(ApellidoMaternoBuscar)))
                    new Dialogos().ConfirmacionDialogo("Notificación!", "No se encontró ningun imputado con esos datos.");
                EmptyExpedienteVisible = !(ListExpediente.Count > 0);
            }
            catch (Exception ex)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al realizar la búsqueda de imputados.", ex);
            }
        }

        private async void EnterBuscar(Object obj)
        {
            try
            {
                if (!PConsultar)
                {
                    (new Dialogos()).ConfirmacionDialogo("Advertencia!", "No tienes permisos para hacer consultas.");
                    return;
                }
                if (obj == null)
                {
                    (new Dialogos()).ConfirmacionDialogo("Advertencia!", "Ocurrió un error al realizar la búsqueda de imputados.");
                    return;
                }
                ListExpediente = new RangeEnabledObservableCollection<IMPUTADO>();
                ListExpediente.InsertRange(await SegmentarResultadoBusqueda());
                PopUpsViewModels.ShowPopUp(this, PopUpsViewModels.TipoPopUp.BUSQUEDA);

            }
            catch (Exception ex)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al realizar la búsqueda de imputados.", ex);
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
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al querer guardar.", ex);
                return new List<IMPUTADO>();
            }
        }

        private void GetExpediente()
        {
            try
            {
                AnioD = SelectIngreso.ID_ANIO;
                FolioD = SelectIngreso.ID_IMPUTADO;
                NombreD = string.IsNullOrEmpty(SelectIngreso.IMPUTADO.NOMBRE) ? string.Empty : SelectIngreso.IMPUTADO.NOMBRE.Trim();
                PaternoD = string.IsNullOrEmpty(SelectIngreso.IMPUTADO.PATERNO) ? string.Empty : SelectIngreso.IMPUTADO.PATERNO.Trim();
                MaternoD = string.IsNullOrEmpty(SelectIngreso.IMPUTADO.MATERNO) ? string.Empty : SelectIngreso.IMPUTADO.MATERNO.Trim();
                IngresosD = (short)SelectIngreso.IMPUTADO.INGRESO.Count;
                //NoControlD = SelectImputadoIngreso.IMPUTADO.
                UbicacionD = SelectIngreso.CAMA != null ? SelectIngreso.CAMA.CELDA.SECTOR.EDIFICIO.DESCR.Trim() + "-" + SelectIngreso.CAMA.CELDA.SECTOR.DESCR.Trim() + "" +
                    SelectIngreso.CAMA.CELDA.ID_CELDA.ToString().Trim() + "-" + SelectIngreso.CAMA.ID_CAMA : string.Empty;
                TipoSeguridadD = SelectIngreso.TIPO_SEGURIDAD.DESCR;
                FecIngresoD = SelectIngreso.FEC_INGRESO_CERESO == null ? null : SelectIngreso.FEC_INGRESO_CERESO;
                ClasificacionJuridicaD = SelectIngreso.CLASIFICACION_JURIDICA.DESCR;
                EstatusD = SelectIngreso.ESTATUS_ADMINISTRATIVO.DESCR;
                ImagenIngreso = SelectIngreso.INGRESO_BIOMETRICO.Where(w => w.ID_TIPO_BIOMETRICO == (short)enumTipoBiometrico.FOTO_FRENTE_SEGUIMIENTO && w.ID_FORMATO == (short)enumTipoFormato.FMTO_JPG).Any() ?
                    SelectIngreso.INGRESO_BIOMETRICO.Where(w => w.ID_TIPO_BIOMETRICO == (short)enumTipoBiometrico.FOTO_FRENTE_SEGUIMIENTO && w.ID_FORMATO == (short)enumTipoFormato.FMTO_JPG).FirstOrDefault().BIOMETRICO :
                        SelectIngreso.INGRESO_BIOMETRICO.Where(w => w.ID_TIPO_BIOMETRICO == (short)enumTipoBiometrico.FOTO_FRENTE_REGISTRO && w.ID_FORMATO == (short)enumTipoFormato.FMTO_JPG).Any() ?
                            SelectIngreso.INGRESO_BIOMETRICO.Where(w => w.ID_TIPO_BIOMETRICO == (short)enumTipoBiometrico.FOTO_FRENTE_REGISTRO && w.ID_FORMATO == (short)enumTipoFormato.FMTO_JPG).FirstOrDefault().BIOMETRICO :
                                new Imagenes().getImagenPerson();
                FiltrarEnabled = true;
            }
            catch (Exception ex)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al cargar los datos del imputado.", ex);
            }
        }

        private void CargarReporteConsulta()
        {
            try
            {
                if (SelectNotaMedica == null)
                {
                    new Dialogos().ConfirmacionDialogo("Validación", "Debe seleccionar una nota medica para cargar el reporte.");
                    return;
                }
                SelectNotaMedicaAuxiliar = new cAtencionMedica().GetData().First(g => g.ID_ATENCION_MEDICA == SelectNotaMedica.ID_ATENCION_MEDICA);
                var medico = SelectNotaMedicaAuxiliar.NOTA_MEDICA != null ? SelectNotaMedicaAuxiliar.NOTA_MEDICA.PERSONA != null ? SelectNotaMedicaAuxiliar.NOTA_MEDICA.PERSONA : null : null; //new cUsuario().ObtenerTodos(GlobalVar.gUsr).FirstOrDefault().EMPLEADO;
                var hoy = Fechas.GetFechaDateServer;
                var hora = hoy.ToShortTimeString() + " hrs. Del dia ";
                var dia = hoy.Day + " de " + hoy.ToString("MMMM").ToUpper() + " del " + hoy.Year + ".";
                var rds1 = new Microsoft.Reporting.WinForms.ReportDataSource();
                rds1.Name = "DataSet1";

                #region DELITOS
                var delitos = string.Empty;
                var i = 1;
                if (SelectIngreso.CAUSA_PENAL.Any(a => a.ID_ESTATUS_CP == (short)enumEstatusCausaPenal.ACTIVO))
                {
                    foreach (var item in SelectIngreso.CAUSA_PENAL.First(a => a.ID_ESTATUS_CP == (short)enumEstatusCausaPenal.ACTIVO).CAUSA_PENAL_DELITO)
                    {
                        delitos = delitos + item.DESCR_DELITO + (i == SelectIngreso.CAUSA_PENAL.First(a => a.ID_ESTATUS_CP == (short)enumEstatusCausaPenal.ACTIVO).CAUSA_PENAL_DELITO.Count ? "" : ",");
                        i++;
                    }
                }
                #endregion

                #region DIETAS
                var dietas = string.Empty;
                i = 0;
                if (SelectNotaMedicaAuxiliar.NOTA_MEDICA != null ? SelectNotaMedicaAuxiliar.NOTA_MEDICA.NOTA_MEDICA_DIETA.Any(a => a.ESTATUS == "S") : false)
                    foreach (var item in SelectNotaMedicaAuxiliar.NOTA_MEDICA.NOTA_MEDICA_DIETA.Where(w => w.ESTATUS == "S"))
                    {
                        dietas = dietas + item.DIETA.DESCR + (i == SelectNotaMedicaAuxiliar.NOTA_MEDICA.NOTA_MEDICA_DIETA.Count(c => c.ESTATUS == "S") ? string.Empty : ", ");
                        i++;
                    }
                #endregion

                #region MEDICAMENTOS
                var medicamentos = string.Empty;
                i = 0;
                if (SelectNotaMedicaAuxiliar.RECETA_MEDICA.Any(a => a.RECETA_MEDICA_DETALLE.Any()))
                    foreach (var item in SelectNotaMedicaAuxiliar.RECETA_MEDICA.SelectMany(s => s.RECETA_MEDICA_DETALLE))
                    {
                        medicamentos = medicamentos + item.DOSIS + " " + item.PRODUCTO.PRODUCTO_UNIDAD_MEDIDA.NOMBRE.Trim() + " DE " + item.PRODUCTO.NOMBRE.Trim() + " " +
                            (item.DESAYUNO == 1 ? (item.COMIDA == 1 ? (item.CENA == 1 ? "EN LA MAÑANA, TARDE Y NOCHE" : "EN LA MAÑANA Y TARDE") :
                            (item.CENA == 1 ? "EN LA MAÑANA Y NOCHE" : "EN LA MAÑANA")) :
                            (item.COMIDA == 1 ? (item.CENA == 1 ? "EN LA TARDE Y NOCHE" : "EN LA TARDE") :
                            (item.CENA == 1 ? "EN LA NOCHE" : ""))) +
                            " POR " + (item.DURACION) + " " + "DIA" + (item.DURACION > 1 ? "S" : "") + (i == SelectNotaMedicaAuxiliar.RECETA_MEDICA.SelectMany(s => s.RECETA_MEDICA_DETALLE).Count() ? ", " : " ");
                        i++;
                    }
                #endregion

                #region ENFERMEDADES
                var enfermedades = string.Empty;
                i = 0;
                if (SelectNotaMedicaAuxiliar.NOTA_MEDICA != null ? SelectNotaMedicaAuxiliar.NOTA_MEDICA.NOTA_MEDICA_ENFERMEDAD.Any() : false)
                    foreach (var item in SelectNotaMedicaAuxiliar.NOTA_MEDICA.NOTA_MEDICA_ENFERMEDAD)
                    {
                        enfermedades = enfermedades + item.ENFERMEDAD.NOMBRE.Trim() + (i == SelectNotaMedicaAuxiliar.NOTA_MEDICA.NOTA_MEDICA_ENFERMEDAD.Count() ? ", " : " ");
                        i++;
                    }
                #endregion

                var dientes = Convert.ToBase64String(new Imagenes().getImagen("miniDiente.png"));
                var _htmlDiente = "<html><table><tr><td>";
                var j = 0;
                for (j = 0; j < 16; j++)
                    _htmlDiente += "<image id='ImagenDiente' style='width:20px;height:50px;' src = \" data:image/png;base64, " + dientes + " alt=\"diente" + j + "\" /></td><td>";
                _htmlDiente += "</td></tr></table></html>";
                var odonto = new HistoriaClinicaViewModel(new Nullable<short>(), new Nullable<short>(), new Nullable<short>(), new Nullable<short>()).CaptureWebPageBytesP(_htmlDiente, 400, 100);

                #region ANTECEDENTES PAOTOLOGICOS
                var antecedentesPatologicos = string.Empty;
                if (SelectNotaMedicaAuxiliar != null ? SelectNotaMedicaAuxiliar.CERTIFICADO_MEDICO != null : false)
                {
                    var k = 1;
                    foreach (var item in SelectNotaMedicaAuxiliar.INGRESO.HISTORIA_CLINICA.SelectMany(s => s.HISTORIA_CLINICA_PATOLOGICOS))
                    {
                        antecedentesPatologicos = antecedentesPatologicos + item.PATOLOGICO_CAT.DESCR + (k <= SelectNotaMedicaAuxiliar.INGRESO.HISTORIA_CLINICA.SelectMany(s => s.HISTORIA_CLINICA_PATOLOGICOS).Count() ? ", " : "");
                        k++;
                    }
                }
                #endregion

                #region TOXICOMANIAS
                var toxicomanias = SelectNotaMedicaAuxiliar != null ?
                    SelectNotaMedicaAuxiliar.CERTIFICADO_MEDICO != null ?
                        string.IsNullOrEmpty(SelectNotaMedicaAuxiliar.CERTIFICADO_MEDICO.TOXICOMANIAS) ? "" : "SI, " + SelectNotaMedicaAuxiliar.CERTIFICADO_MEDICO.TOXICOMANIAS
                    : ""
                : "";
                #endregion

                #region OBJETO
                rds1.Value = new List<cReporteCertificadoNuevoIngreso>()
                {
                    new cReporteCertificadoNuevoIngreso
                    { 
                        OdontogramaImagen = SelectNotaMedicaAuxiliar.ID_TIPO_ATENCION == (short)enumAtencionTipo.CONSULTA_DENTAL ? odonto : null,
                        DENTAL = SelectNotaMedicaAuxiliar.ID_TIPO_ATENCION == (short)enumAtencionTipo.CONSULTA_DENTAL ? "S" : "N",
                        ID_ATENCION_MEDICA = SelectNotaMedicaAuxiliar.ID_ATENCION_MEDICA,
                        Titulo = SelectNotaMedicaAuxiliar.ATENCION_SERVICIO != null ? SelectNotaMedicaAuxiliar.ATENCION_SERVICIO.DESCR : string.Empty,
                        EstudiosRealizados=SelectNotaMedicaAuxiliar.ID_TIPO_SERVICIO == (short)enumAtencionServicio.NOTA_EVOLUCION ?
                            SelectNotaMedicaAuxiliar.NOTA_MEDICA != null ?
                                SelectNotaMedicaAuxiliar.NOTA_MEDICA.NOTA_EVOLUCION != null ?
                                    SelectNotaMedicaAuxiliar.NOTA_MEDICA.NOTA_EVOLUCION.ESTUDIO_RESULTADO
                                : string.Empty
                            : string.Empty
                        : string.Empty,
                        Pronostico = SelectNotaMedicaAuxiliar.NOTA_MEDICA != null ?
                            SelectNotaMedicaAuxiliar.NOTA_MEDICA.PRONOSTICO != null ?
                                SelectNotaMedicaAuxiliar.NOTA_MEDICA.PRONOSTICO.DESCR
                            : string.Empty
                        : string.Empty,
                        SignosVitales_Observaciones = SelectNotaMedicaAuxiliar != null ? SelectNotaMedicaAuxiliar.NOTA_SIGNOS_VITALES != null ? SelectNotaMedicaAuxiliar.NOTA_SIGNOS_VITALES.OBSERVACIONES: string.Empty : string.Empty,
                        SignosVitales_Glucemia = SelectNotaMedicaAuxiliar != null ? SelectNotaMedicaAuxiliar.NOTA_SIGNOS_VITALES != null ? SelectNotaMedicaAuxiliar.NOTA_SIGNOS_VITALES.GLUCEMIA : string.Empty : string.Empty,
                        SignosVitales_Peso = SelectNotaMedicaAuxiliar != null ? SelectNotaMedicaAuxiliar.NOTA_SIGNOS_VITALES != null ? SelectNotaMedicaAuxiliar.NOTA_SIGNOS_VITALES.PESO : string.Empty : string.Empty,
                        SignosVitales_Talla = SelectNotaMedicaAuxiliar != null ? SelectNotaMedicaAuxiliar.NOTA_SIGNOS_VITALES != null ? SelectNotaMedicaAuxiliar.NOTA_SIGNOS_VITALES.TALLA : string.Empty : string.Empty,
                        SignosVitales_FC = SelectNotaMedicaAuxiliar != null ? SelectNotaMedicaAuxiliar.NOTA_SIGNOS_VITALES != null ? SelectNotaMedicaAuxiliar.NOTA_SIGNOS_VITALES.FRECUENCIA_CARDIAC : string.Empty : string.Empty,
                        SignosVitales_FR = SelectNotaMedicaAuxiliar != null ? SelectNotaMedicaAuxiliar.NOTA_SIGNOS_VITALES != null ? SelectNotaMedicaAuxiliar.NOTA_SIGNOS_VITALES.FRECUENCIA_RESPIRA : string.Empty : string.Empty,
                        SignosVitales_T = SelectNotaMedicaAuxiliar != null ? SelectNotaMedicaAuxiliar.NOTA_SIGNOS_VITALES != null ? SelectNotaMedicaAuxiliar.NOTA_SIGNOS_VITALES.TEMPERATURA : string.Empty : string.Empty,
                        SignosVitales_TA = SelectNotaMedicaAuxiliar != null ? SelectNotaMedicaAuxiliar.NOTA_SIGNOS_VITALES != null ? SelectNotaMedicaAuxiliar.NOTA_SIGNOS_VITALES.TENSION_ARTERIAL : string.Empty : string.Empty,
                        Centro = SelectIngreso.CENTRO1 != null ?
                            string.IsNullOrEmpty(SelectIngreso.CENTRO1.DESCR) ?
                                string.Empty
                            : SelectIngreso.CENTRO1.DESCR.Trim()
                        : "",
                        Ciudad = SelectIngreso.CENTRO1 != null ?
                            SelectIngreso.CENTRO1.MUNICIPIO != null ?
                                (string.IsNullOrEmpty(SelectIngreso.CENTRO1.MUNICIPIO.MUNICIPIO1) ?
                                    string.Empty
                                : SelectIngreso.CENTRO1.MUNICIPIO.MUNICIPIO1.Trim())
                            : string.Empty
                        : string.Empty,
                        Director = SelectIngreso.CENTRO1 != null ? string.IsNullOrEmpty(SelectIngreso.CENTRO1.DIRECTOR) ? string.Empty : SelectIngreso.CENTRO1.DIRECTOR.Trim() : "",
                        Cedula = medico != null ? medico.EMPLEADO != null ? medico.EMPLEADO.CEDULA != null ? medico.EMPLEADO.CEDULA.Trim() : "" : "" : "",
                        Edad = new Fechas().CalculaEdad(SelectIngreso.IMPUTADO.NACIMIENTO_FECHA.HasValue ? SelectIngreso.IMPUTADO.NACIMIENTO_FECHA.Value : new DateTime()).ToString(),
                        FechaIngreso = SelectIngreso.FEC_INGRESO_CERESO.HasValue ? SelectIngreso.FEC_INGRESO_CERESO.Value.ToShortDateString() : new DateTime().ToShortDateString(),
                        FechaNacimiento = SelectIngreso.IMPUTADO.NACIMIENTO_FECHA.HasValue ? SelectIngreso.IMPUTADO.NACIMIENTO_FECHA.Value.ToShortDateString() : new DateTime().ToShortDateString(),
                        Fecha = hora + dia,
                        Folio = SelectIngreso.ID_ANIO + "/" + SelectIngreso.ID_IMPUTADO,
                        Escolaridad = SelectIngreso.ESCOLARIDAD != null ? (string.IsNullOrEmpty(SelectIngreso.ESCOLARIDAD.DESCR) ? string.Empty : SelectIngreso.ESCOLARIDAD.DESCR.Trim()) : string.Empty,
                        Originario = SelectIngreso.IMPUTADO.NACIMIENTO_MUNICIPIO.HasValue ?
                            (string.IsNullOrEmpty(SelectIngreso.IMPUTADO.MUNICIPIO.MUNICIPIO1) ?
                                string.Empty
                            : SelectIngreso.IMPUTADO.MUNICIPIO.MUNICIPIO1.Trim()) + ", " +
                            (SelectIngreso.IMPUTADO.MUNICIPIO != null ?
                                SelectIngreso.IMPUTADO.MUNICIPIO.ENTIDAD != null ?
                                    (string.IsNullOrEmpty(SelectIngreso.IMPUTADO.MUNICIPIO.ENTIDAD.DESCR) ? string.Empty : SelectIngreso.IMPUTADO.MUNICIPIO.ENTIDAD.DESCR.Trim())
                                : string.Empty
                            : string.Empty)
                        : SelectIngreso.IMPUTADO.NACIMIENTO_LUGAR,
                        NombreImputado = SelectIngreso.IMPUTADO.NOMBRE.Trim() + " " + SelectIngreso.IMPUTADO.PATERNO.Trim() + " " + (string.IsNullOrEmpty(SelectIngreso.IMPUTADO.MATERNO) ? string.Empty : SelectIngreso.IMPUTADO.MATERNO.Trim()),
                        TipoDelito = string.IsNullOrEmpty(delitos) ? "N/A" : delitos,
                        NombreMedico = medico != null ? (medico.NOMBRE.Trim() + " " + medico.PATERNO.Trim() + " " + (string.IsNullOrEmpty(medico.MATERNO) ? string.Empty : medico.MATERNO.Trim())) : string.Empty,
                        Sexo = SelectIngreso.IMPUTADO != null ? SelectIngreso.IMPUTADO.SEXO == "M" ? "MASCULINO" : SelectIngreso.IMPUTADO.SEXO == "F" ? "FEMENINO" : string.Empty : string.Empty,
                        Logo1 = Parametro.LOGO_ESTADO,
                        Logo2 = Parametro.REPORTE_LOGO2,
                        Dorso = Parametro.CUERPO_DORSO,
                        Frente = Parametro.CUERPO_FRENTE,
                        Check = Parametro.IMAGEN_ZONA_CORPORAL,
                        AntecedentesPatologicos = antecedentesPatologicos,
                        Toxicomanias = toxicomanias,
                        ExploracionFisica = SelectNotaMedicaAuxiliar != null ? SelectNotaMedicaAuxiliar.NOTA_MEDICA != null ? SelectNotaMedicaAuxiliar.NOTA_MEDICA.EXPLORACION_FISICA : string.Empty : string.Empty,
                        PadecimientoTratamiento = SelectNotaMedicaAuxiliar != null ? SelectNotaMedicaAuxiliar.CERTIFICADO_MEDICO != null ? SelectNotaMedicaAuxiliar.CERTIFICADO_MEDICO.PADECIMIENTO : "" : "",
                        AmeritaHospitalizacion = SelectNotaMedicaAuxiliar != null ? SelectNotaMedicaAuxiliar.CERTIFICADO_MEDICO != null ? SelectNotaMedicaAuxiliar.CERTIFICADO_MEDICO.AMERITA_HOSPITALIZACION == "S" ? "SI" : "NO" : "NO" : "NO",
                        Diagnostico = SelectNotaMedicaAuxiliar != null ? SelectNotaMedicaAuxiliar.CERTIFICADO_MEDICO != null ? SelectNotaMedicaAuxiliar.CERTIFICADO_MEDICO.DIAGNOSTICO : string.Empty : string.Empty,
                        DiasEnSanar = SelectNotaMedicaAuxiliar != null ? SelectNotaMedicaAuxiliar.CERTIFICADO_MEDICO != null ? SelectNotaMedicaAuxiliar.CERTIFICADO_MEDICO.TARDA_SANAR_15 == "S" ? "SI" : "NO" : "NO" : "NO",
                        Observaciones = SelectNotaMedicaAuxiliar != null ? SelectNotaMedicaAuxiliar.CERTIFICADO_MEDICO != null ? SelectNotaMedicaAuxiliar.CERTIFICADO_MEDICO.OBSERVACIONES : string.Empty : string.Empty,
                        PeligraVida = SelectNotaMedicaAuxiliar !=null ? SelectNotaMedicaAuxiliar.CERTIFICADO_MEDICO != null ? SelectNotaMedicaAuxiliar.CERTIFICADO_MEDICO.PELIGRA_VIDA == "S" ? "SI" : "NO" : "NO" : "NO",
                        PlanTerapeutico = SelectNotaMedicaAuxiliar !=null ? SelectNotaMedicaAuxiliar.CERTIFICADO_MEDICO != null ? SelectNotaMedicaAuxiliar.CERTIFICADO_MEDICO.PLAN_TERAPEUTICO : string.Empty : string.Empty,
                        Seguimiento = SelectNotaMedicaAuxiliar != null ?
                            SelectNotaMedicaAuxiliar.ID_CITA_SEGUIMIENTO.HasValue ?
                                SelectNotaMedicaAuxiliar.ATENCION_CITA1 != null ?
                                    SelectNotaMedicaAuxiliar.ATENCION_CITA1.CITA_FECHA_HORA.HasValue ?
                                        "SI, " + SelectNotaMedicaAuxiliar.ATENCION_CITA1.CITA_FECHA_HORA.Value.ToString()
                                    : "SI"
                                : "SI"
                            : "NO"
                        : "NO",
                        SolicitaInterconsulta = SelectNotaMedicaAuxiliar.NOTA_INTERCONSULTA != null ? "SI" : "NO",
                        Dietas = dietas, 
                        Medicamentos = medicamentos, 
                        Enfermedades = enfermedades,
                        Interrogatorio = SelectNotaMedicaAuxiliar != null ? SelectNotaMedicaAuxiliar.NOTA_MEDICA != null ? SelectNotaMedicaAuxiliar.NOTA_MEDICA.RESUMEN_INTERROGAT : string.Empty : string.Empty,
                    }
                };
                #endregion

                #region DENTAL
                ListaOdontogramaSeguimiento = new List<cReporteOdontogramaSeguimiento>();
                foreach (var item in SelectNotaMedicaAuxiliar.ODONTOGRAMA_SEGUIMIENTO2)
                {
                    ListaOdontogramaSeguimiento.Add(new cReporteOdontogramaSeguimiento
                    {
                        DESCR = string.Format("{0} EN {1} DEL {2}. POSICIÓN {3}", item.DENTAL_TRATAMIENTO == null ? item.ENFERMEDAD == null ? "" : item.ENFERMEDAD.NOMBRE.Trim() : item.DENTAL_TRATAMIENTO.DESCR.Trim(),
                            item.ODONTOGRAMA_TIPO.DESCR.Trim(), item.ODONTOGRAMA.DESCR.Trim(), item.ID_POSICION.ToString()),
                        ID = item.ID_CONSECUTIVO,
                    });
                }
                #endregion

                Application.Current.Dispatcher.Invoke((Action)(delegate
                {
                    ReporteVisible = Visibility.Visible;
                    if (SelectNotaMedicaAuxiliar.ID_TIPO_SERVICIO == (short)enumAtencionServicio.NOTA_EVOLUCION)
                        Reporte.LocalReport.ReportPath = "Reportes/rNotaEvolucion.rdlc";
                    else
                        Reporte.LocalReport.ReportPath = "Reportes/rConsultaMedica.rdlc";
                    Reporte.LocalReport.DataSources.Clear();
                    Reporte.LocalReport.DataSources.Add(rds1);
                    //Reporte.LocalReport.DataSources.Add(rds2);
                    Reporte.ShowExportButton = false;
                    Reporte.SetDisplayMode(Microsoft.Reporting.WinForms.DisplayMode.PrintLayout);
                    Reporte.ZoomMode = Microsoft.Reporting.WinForms.ZoomMode.Percent;
                    Reporte.ZoomPercent = 100;
                    Reporte.Margin = new System.Windows.Forms.Padding(5);
                    #region Subreporte
                    /*Reporte.LocalReport.SubreportProcessing += (s, e) =>
                    {
                        if (e.ReportPath.Equals("cSubreporteProcedimientos", StringComparison.InvariantCultureIgnoreCase))
                            e.DataSources.Add(new ReportDataSource("DataSet1", ListSubreporteProcedimientos));
                        else if (e.ReportPath.Equals("cSubreporteInsumoDetalle", StringComparison.InvariantCultureIgnoreCase))
                            e.DataSources.Add(new ReportDataSource("DataSet1", ListSubreporteInsumos));
                        else if (e.ReportPath.Equals("cSubreporteOdontograma", StringComparison.InvariantCultureIgnoreCase) && SelectNotaMedicaAuxiliar.ID_TIPO_ATENCION == (short)enumAtencionTipo.CONSULTA_DENTAL)
                        {
                            e.DataSources.Clear();
                            e.DataSources.Add(new ReportDataSource("DataSet1", ListaOdontogramaSeguimiento));
                        }
                    };*/
                    Reporte.LocalReport.SubreportProcessing += ProcesandoSubreporte;
                    #endregion
                    Reporte.Refresh();
                    Reporte.RefreshReport();
                }));
            }
            catch (Exception ex)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al cargar los datos del reporte.", ex);
            }
        }

        private void CargarReporteProdecimientoMedico()
        {
            try
            {
                if (SelectNotaMedica == null)
                {
                    new Dialogos().ConfirmacionDialogo("Validación", "Debe seleccionar una nota medica para cargar el reporte.");
                    return;
                }
                SelectNotaMedicaAuxiliar = new cAtencionMedica().GetData().First(g => g.ID_ATENCION_MEDICA == SelectNotaMedica.ID_ATENCION_MEDICA);
                var medico = new SSP.Servidor.PERSONA();
                medico = null;
                if (SelectNotaMedicaAuxiliar.NOTA_MEDICA != null ? SelectNotaMedicaAuxiliar.NOTA_MEDICA.PERSONA != null : false)
                    medico = SelectNotaMedicaAuxiliar.NOTA_MEDICA.PERSONA;
                else if (SelectNotaMedicaAuxiliar.ATENCION_CITA.Any() ? SelectNotaMedicaAuxiliar.ATENCION_CITA.Any(a => a.PERSONA != null) : false)
                    medico = SelectNotaMedicaAuxiliar.ATENCION_CITA.First(f => f.PERSONA != null).PERSONA;
                var hoy = Fechas.GetFechaDateServer;
                var hora = hoy.ToShortTimeString() + " hrs. Del dia ";
                var dia = hoy.Day + " de " + hoy.ToString("MMMM").ToUpper() + " del " + hoy.Year + ".";
                var rds1 = new Microsoft.Reporting.WinForms.ReportDataSource();
                rds1.Name = "DataSet1";
                var delitos = string.Empty;
                var i = 1;
                if (SelectIngreso.CAUSA_PENAL.Any(a => a.ID_ESTATUS_CP == (short)enumEstatusCausaPenal.ACTIVO))
                {
                    foreach (var item in SelectIngreso.CAUSA_PENAL.First(a => a.ID_ESTATUS_CP == (short)enumEstatusCausaPenal.ACTIVO).CAUSA_PENAL_DELITO)
                    {
                        delitos = delitos + item.DESCR_DELITO + (i == SelectIngreso.CAUSA_PENAL.First(a => a.ID_ESTATUS_CP == (short)enumEstatusCausaPenal.ACTIVO).CAUSA_PENAL_DELITO.Count ? "" : ",");
                        i++;
                    }
                }
                var dientes = Convert.ToBase64String(new Imagenes().getImagen("miniDiente.png"));
                var _htmlDiente = "<html><table><tr><td>";
                var j = 0;
                for (j = 0; j < 16; j++)
                    _htmlDiente += "<image id='ImagenDiente' style='width:20px;height:50px;' src = \" data:image/png;base64, " + dientes + " alt=\"diente" + j + "\" /></td><td>";

                _htmlDiente += "</td></tr></table></html>";

                var odonto = new HistoriaClinicaViewModel(new Nullable<short>(), new Nullable<short>(), new Nullable<short>(), new Nullable<short>()).CaptureWebPageBytesP(_htmlDiente, 400, 100);

                #region PROC MED
                var procedimientosmedicos = new List<string>();
                var insumos = new List<string>();
                //ListaSubreporte = new List<cReporteProcedimientosMedicos>();
                ListSubreporteInsumos = new List<cReporteInsumos>();
                i = 0;
                ListSubreporteProcedimientos = new List<cReporteProcsMeds>();
                if (SelectNotaMedicaAuxiliar.PROC_ATENCION_MEDICA.Any(a => a.PROC_ATENCION_MEDICA_PROG.Any(an => an.PROC_MEDICO_PROG_DET.Any())))
                    foreach (var item in SelectNotaMedicaAuxiliar.PROC_ATENCION_MEDICA.Where(a => a.PROC_ATENCION_MEDICA_PROG.Any(an => an.PROC_MEDICO_PROG_DET.Any())))
                        if (item.PROC_ATENCION_MEDICA_PROG.Any())
                        {
                            ListSubreporteProcedimientos.Add(new cReporteProcsMeds
                            {
                                ID_ATENCION_MEDICA = SelectNotaMedicaAuxiliar.ID_ATENCION_MEDICA,
                                ID_PROC_MED = item.ID_PROCMED,
                                ProcedimientoMedico = item.PROC_MED.DESCR,
                            });
                            foreach (var itm in item.PROC_ATENCION_MEDICA_PROG.SelectMany(s => s.PROC_MEDICO_PROG_DET))
                            {
                                ListSubreporteInsumos.Add(new cReporteInsumos
                                {
                                    Insumo = itm.CANTIDAD_UTILIZADA.HasValue ? itm.CANTIDAD_UTILIZADA.Value.ToString() : "0",
                                    Producto = itm.PRODUCTO.NOMBRE,
                                    ID_ATENCION_MEDICA = SelectNotaMedicaAuxiliar.ID_ATENCION_MEDICA,
                                    ID_PROC_MED = item.ID_PROCMED,
                                    Estatus = itm.PROC_ATENCION_MEDICA_PROG.REALIZACION_FEC.HasValue ? itm.PROC_ATENCION_MEDICA_PROG.REALIZACION_FEC.Value.ToString() : "N/A"
                                });
                            }
                        }
                if (SelectNotaMedicaAuxiliar.ATENCION_CITA.Any(a => a.PROC_ATENCION_MEDICA_PROG.Any(b => b.PROC_MEDICO_PROG_DET.Any())))
                {
                    foreach (var item in SelectNotaMedicaAuxiliar.ATENCION_CITA.SelectMany(s => s.PROC_ATENCION_MEDICA_PROG))
                    {
                        ListSubreporteProcedimientos.Add(new cReporteProcsMeds
                        {
                            ID_ATENCION_MEDICA = item.ID_ATENCION_MEDICA,
                            ID_PROC_MED = item.ID_PROCMED,
                            ProcedimientoMedico = item.PROC_ATENCION_MEDICA.PROC_MED.DESCR,
                        });
                        foreach (var itm in item.PROC_MEDICO_PROG_DET)
                        {
                            ListSubreporteInsumos.Add(new cReporteInsumos
                            {
                                Insumo = itm.CANTIDAD_UTILIZADA.HasValue ? itm.CANTIDAD_UTILIZADA.Value.ToString() : string.Empty,
                                Producto = itm.PRODUCTO.NOMBRE,
                                ID_ATENCION_MEDICA = item.ID_ATENCION_MEDICA,
                                ID_PROC_MED = itm.ID_PROCMED,
                                Estatus = itm.PROC_ATENCION_MEDICA_PROG.REALIZACION_FEC.HasValue ? itm.PROC_ATENCION_MEDICA_PROG.REALIZACION_FEC.Value.ToString() : "N/A"
                            });
                        }
                    }
                }
                #endregion

                #region OBJETO
                rds1.Value = new List<cReporteCertificadoNuevoIngreso>()
                {
                    new cReporteCertificadoNuevoIngreso
                    { 
                        OdontogramaImagen = SelectNotaMedicaAuxiliar.ID_TIPO_ATENCION == (short)enumAtencionTipo.CONSULTA_DENTAL ? odonto : null,
                        DENTAL = SelectNotaMedicaAuxiliar.ID_TIPO_ATENCION == (short)enumAtencionTipo.CONSULTA_DENTAL ? "S" : "N",
                        ID_ATENCION_MEDICA = SelectNotaMedicaAuxiliar.ID_ATENCION_MEDICA,
                        Titulo = SelectNotaMedicaAuxiliar.ATENCION_SERVICIO != null ? SelectNotaMedicaAuxiliar.ATENCION_SERVICIO.DESCR : string.Empty,
                        Centro = SelectIngreso.CENTRO1 != null ? string.IsNullOrEmpty(SelectIngreso.CENTRO1.DESCR) ? string.Empty : SelectIngreso.CENTRO1.DESCR.Trim() : "",
                        Ciudad = SelectIngreso.CENTRO1 != null ? SelectIngreso.CENTRO1.MUNICIPIO != null ? (string.IsNullOrEmpty(SelectIngreso.CENTRO1.MUNICIPIO.MUNICIPIO1) ? string.Empty : SelectIngreso.CENTRO1.MUNICIPIO.MUNICIPIO1.Trim()) : string.Empty : string.Empty,
                        Director = SelectIngreso.CENTRO1 != null ? string.IsNullOrEmpty(SelectIngreso.CENTRO1.DIRECTOR) ? string.Empty : SelectIngreso.CENTRO1.DIRECTOR.Trim() : "",
                        Cedula = medico != null ? medico.EMPLEADO != null ? medico.EMPLEADO.CEDULA != null ? medico.EMPLEADO.CEDULA.Trim() : "N/A" : "N/A" : "N/A",
                        Edad = new Fechas().CalculaEdad(SelectIngreso.IMPUTADO.NACIMIENTO_FECHA.HasValue ? SelectIngreso.IMPUTADO.NACIMIENTO_FECHA.Value : new DateTime()).ToString(),
                        FechaIngreso = SelectIngreso.FEC_INGRESO_CERESO.HasValue ? SelectIngreso.FEC_INGRESO_CERESO.Value.ToShortDateString() : new DateTime().ToShortDateString(),
                        FechaNacimiento = SelectIngreso.IMPUTADO.NACIMIENTO_FECHA.HasValue ? SelectIngreso.IMPUTADO.NACIMIENTO_FECHA.Value.ToShortDateString() : new DateTime().ToShortDateString(),
                        Fecha = hora + dia,
                        Folio = SelectIngreso.ID_ANIO + "/" + SelectIngreso.ID_IMPUTADO,
                        Escolaridad = SelectIngreso.ESCOLARIDAD != null ? (string.IsNullOrEmpty(SelectIngreso.ESCOLARIDAD.DESCR) ? string.Empty : SelectIngreso.ESCOLARIDAD.DESCR.Trim()) : "",
                        Originario = SelectIngreso.IMPUTADO.NACIMIENTO_MUNICIPIO.HasValue ? 
                            (string.IsNullOrEmpty(SelectIngreso.IMPUTADO.MUNICIPIO.MUNICIPIO1) ? 
                                string.Empty 
                            : SelectIngreso.IMPUTADO.MUNICIPIO.MUNICIPIO1.Trim()) + ", " +
                            (SelectIngreso.IMPUTADO.MUNICIPIO != null ? 
                                SelectIngreso.IMPUTADO.MUNICIPIO.ENTIDAD != null ? 
                                    (string.IsNullOrEmpty(SelectIngreso.IMPUTADO.MUNICIPIO.ENTIDAD.DESCR) ? string.Empty : SelectIngreso.IMPUTADO.MUNICIPIO.ENTIDAD.DESCR.Trim())
                                : string.Empty
                            : string.Empty)
                        : SelectIngreso.IMPUTADO.NACIMIENTO_LUGAR,
                        SignosVitales_Observaciones = SelectNotaMedicaAuxiliar != null ? SelectNotaMedicaAuxiliar.NOTA_SIGNOS_VITALES != null ? SelectNotaMedicaAuxiliar.NOTA_SIGNOS_VITALES.OBSERVACIONES: string.Empty : string.Empty,
                        SignosVitales_Glucemia = SelectNotaMedicaAuxiliar != null ? SelectNotaMedicaAuxiliar.NOTA_SIGNOS_VITALES != null ? SelectNotaMedicaAuxiliar.NOTA_SIGNOS_VITALES.GLUCEMIA : string.Empty : string.Empty,
                        SignosVitales_Peso = SelectNotaMedicaAuxiliar != null ? SelectNotaMedicaAuxiliar.NOTA_SIGNOS_VITALES != null ? SelectNotaMedicaAuxiliar.NOTA_SIGNOS_VITALES.PESO : string.Empty : string.Empty,
                        SignosVitales_Talla = SelectNotaMedicaAuxiliar != null ? SelectNotaMedicaAuxiliar.NOTA_SIGNOS_VITALES != null ? SelectNotaMedicaAuxiliar.NOTA_SIGNOS_VITALES.TALLA : string.Empty : string.Empty,
                        SignosVitales_FC = SelectNotaMedicaAuxiliar != null ? SelectNotaMedicaAuxiliar.NOTA_SIGNOS_VITALES != null ? SelectNotaMedicaAuxiliar.NOTA_SIGNOS_VITALES.FRECUENCIA_CARDIAC : string.Empty : string.Empty,
                        SignosVitales_FR = SelectNotaMedicaAuxiliar != null ? SelectNotaMedicaAuxiliar.NOTA_SIGNOS_VITALES != null ? SelectNotaMedicaAuxiliar.NOTA_SIGNOS_VITALES.FRECUENCIA_RESPIRA : string.Empty : string.Empty,
                        SignosVitales_T = SelectNotaMedicaAuxiliar != null ? SelectNotaMedicaAuxiliar.NOTA_SIGNOS_VITALES != null ? SelectNotaMedicaAuxiliar.NOTA_SIGNOS_VITALES.TEMPERATURA : string.Empty : string.Empty,
                        SignosVitales_TA = SelectNotaMedicaAuxiliar != null ? SelectNotaMedicaAuxiliar.NOTA_SIGNOS_VITALES != null ? SelectNotaMedicaAuxiliar.NOTA_SIGNOS_VITALES.TENSION_ARTERIAL : string.Empty : string.Empty,
                        NombreImputado = SelectIngreso.IMPUTADO.NOMBRE.Trim() + " " + SelectIngreso.IMPUTADO.PATERNO.Trim() + " " + (string.IsNullOrEmpty(SelectIngreso.IMPUTADO.MATERNO) ? string.Empty : SelectIngreso.IMPUTADO.MATERNO.Trim()),
                        TipoDelito = string.IsNullOrEmpty(delitos) ? "N/A" : delitos,
                        NombreMedico = medico != null ? (medico.NOMBRE.Trim() + " " + medico.PATERNO.Trim() + " " + (string.IsNullOrEmpty(medico.MATERNO) ? string.Empty : medico.MATERNO.Trim())) : string.Empty,
                        Sexo = SelectIngreso.IMPUTADO != null ? SelectIngreso.IMPUTADO.SEXO == "M" ? "MASCULINO" : SelectIngreso.IMPUTADO.SEXO == "F" ? "FEMENINO" : string.Empty : string.Empty,
                        Logo1 = Parametro.LOGO_ESTADO,
                        Logo2 = Parametro.REPORTE_LOGO2,
                        Dorso = Parametro.CUERPO_DORSO,
                        Frente = Parametro.CUERPO_FRENTE, 
                        Check = Parametro.IMAGEN_ZONA_CORPORAL,
                        AntecedentesPatologicos = SelectNotaMedicaAuxiliar != null ? SelectNotaMedicaAuxiliar.CERTIFICADO_MEDICO != null ? SelectNotaMedicaAuxiliar.CERTIFICADO_MEDICO.ANTECEDENTES_PATOLOGICOS : "" : "",
                        PadecimientoTratamiento = SelectNotaMedicaAuxiliar != null ? SelectNotaMedicaAuxiliar.CERTIFICADO_MEDICO != null ? SelectNotaMedicaAuxiliar.CERTIFICADO_MEDICO.PADECIMIENTO : "" : "",
                        Toxicomanias = SelectNotaMedicaAuxiliar != null ? SelectNotaMedicaAuxiliar.CERTIFICADO_MEDICO != null ? SelectNotaMedicaAuxiliar.CERTIFICADO_MEDICO.TOXICOMANIAS : "" : "",
                        AmeritaHospitalizacion = SelectNotaMedicaAuxiliar != null ? SelectNotaMedicaAuxiliar.CERTIFICADO_MEDICO != null ? SelectNotaMedicaAuxiliar.CERTIFICADO_MEDICO.AMERITA_HOSPITALIZACION == "S" ? "SI" : "NO" : "NO" : "NO",
                        Diagnostico = SelectNotaMedicaAuxiliar != null ? SelectNotaMedicaAuxiliar.CERTIFICADO_MEDICO != null ? SelectNotaMedicaAuxiliar.CERTIFICADO_MEDICO.DIAGNOSTICO : string.Empty : string.Empty,
                        DiasEnSanar = SelectNotaMedicaAuxiliar != null?SelectNotaMedicaAuxiliar.CERTIFICADO_MEDICO != null ? SelectNotaMedicaAuxiliar.CERTIFICADO_MEDICO.TARDA_SANAR_15 == "S" ? "SI" : "NO" : "NO" : "NO",
                        Observaciones = SelectNotaMedicaAuxiliar != null ? SelectNotaMedicaAuxiliar.CERTIFICADO_MEDICO != null ? SelectNotaMedicaAuxiliar.CERTIFICADO_MEDICO.OBSERVACIONES : string.Empty : string.Empty,
                        PeligraVida = SelectNotaMedicaAuxiliar !=null ? SelectNotaMedicaAuxiliar.CERTIFICADO_MEDICO != null ? SelectNotaMedicaAuxiliar.CERTIFICADO_MEDICO.PELIGRA_VIDA == "S" ? "SI" : "NO" : "NO" : "NO",
                        PlanTerapeutico = SelectNotaMedicaAuxiliar !=null ? SelectNotaMedicaAuxiliar.CERTIFICADO_MEDICO != null ? SelectNotaMedicaAuxiliar.CERTIFICADO_MEDICO.PLAN_TERAPEUTICO : string.Empty : string.Empty,
                        Seguimiento = SelectNotaMedicaAuxiliar != null ? 
                            SelectNotaMedicaAuxiliar.ID_CITA_SEGUIMIENTO.HasValue ? 
                                SelectNotaMedicaAuxiliar.ATENCION_CITA1 != null ? 
                                    SelectNotaMedicaAuxiliar.ATENCION_CITA1.CITA_FECHA_HORA.HasValue ?
                                        "SI, " + SelectNotaMedicaAuxiliar.ATENCION_CITA1.CITA_FECHA_HORA.Value.ToString()
                                    : "SI" 
                                : "SI" 
                            : "NO" 
                        : "NO",
                        SolicitaInterconsulta = SelectNotaMedicaAuxiliar.NOTA_INTERCONSULTA != null ? "SI" : "NO", 
                    }
                };
                #endregion

                #region DENTAL
                ListaOdontogramaSeguimiento = new List<cReporteOdontogramaSeguimiento>();
                foreach (var item in SelectNotaMedicaAuxiliar.ODONTOGRAMA_SEGUIMIENTO2)
                {
                    ListaOdontogramaSeguimiento.Add(new cReporteOdontogramaSeguimiento
                    {
                        DESCR = string.Format("{0} EN {1} DEL {2}. POSICIÓN {3}", item.DENTAL_TRATAMIENTO == null ? item.ENFERMEDAD == null ? "" : item.ENFERMEDAD.NOMBRE.Trim() : item.DENTAL_TRATAMIENTO.DESCR.Trim(),
                            item.ODONTOGRAMA_TIPO.DESCR.Trim(), item.ODONTOGRAMA.DESCR.Trim(), item.ID_POSICION.ToString()),
                        ID = item.ID_CONSECUTIVO,
                    });
                }
                #endregion

                Application.Current.Dispatcher.Invoke((Action)(delegate
                {
                    ReporteVisible = Visibility.Visible;
                    Reporte.LocalReport.ReportPath = "Reportes/rProcedimientosMedicos.rdlc";
                    Reporte.LocalReport.DataSources.Clear();
                    Reporte.LocalReport.DataSources.Add(rds1);
                    Reporte.ShowExportButton = false;
                    Reporte.SetDisplayMode(Microsoft.Reporting.WinForms.DisplayMode.PrintLayout);
                    Reporte.ZoomMode = Microsoft.Reporting.WinForms.ZoomMode.Percent;
                    Reporte.ZoomPercent = 100;
                    Reporte.Margin = new System.Windows.Forms.Padding(5);
                    #region Subreporte
                    /*Reporte.LocalReport.SubreportProcessing += (s, e) =>
                    {
                        if (e.ReportPath.Equals("cSubreporteProcedimientos", StringComparison.InvariantCultureIgnoreCase))
                        {
                            e.DataSources.Clear();
                            e.DataSources.Add(new ReportDataSource("DataSet1", ListSubreporteProcedimientos));
                        }
                        else if (e.ReportPath.Equals("cSubreporteInsumoDetalle", StringComparison.InvariantCultureIgnoreCase))
                        {
                            e.DataSources.Clear();
                            e.DataSources.Add(new ReportDataSource("DataSet1", ListSubreporteInsumos));
                        }
                        else if (e.ReportPath.Equals("cSubreporteOdontograma", StringComparison.InvariantCultureIgnoreCase) && SelectNotaMedicaAuxiliar.ID_TIPO_ATENCION == (short)enumAtencionTipo.CONSULTA_DENTAL)
                        {
                            e.DataSources.Clear();
                            e.DataSources.Add(new ReportDataSource("DataSet1", ListaOdontogramaSeguimiento));
                        }
                    };*/
                    Reporte.LocalReport.SubreportProcessing += ProcesandoSubreporte;
                    #endregion
                    Reporte.Refresh();
                    Reporte.RefreshReport();
                }));
            }
            catch (Exception ex)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al cargar los datos del reporte.", ex);
            }
        }

        private void CargarReporteCertificado()
        {
            try
            {
                if (SelectNotaMedica == null)
                {
                    new Dialogos().ConfirmacionDialogo("Validación", "Debe seleccionar una nota medica para cargar el reporte.");
                    return;
                }
                SelectNotaMedicaAuxiliar = new cAtencionMedica().GetData().First(g => g.ID_ATENCION_MEDICA == SelectNotaMedica.ID_ATENCION_MEDICA);
                ListLesiones = SelectNotaMedicaAuxiliar.CERTIFICADO_MEDICO.LESION.Select(s => new LesionesCustom { DESCR = s.DESCR, REGION = s.ANATOMIA_TOPOGRAFICA }).ToList();
                var medico = new cUsuario().ObtenerTodos(GlobalVar.gUsr).FirstOrDefault().EMPLEADO;
                var hoy = Fechas.GetFechaDateServer;
                var hora = hoy.ToShortTimeString() + " hrs. Del dia ";
                var dia = hoy.Day + " de " + hoy.ToString("MMMM").ToUpper() + " del " + hoy.Year + ".";
                var rds1 = new Microsoft.Reporting.WinForms.ReportDataSource();
                rds1.Name = "DataSet1";
                var lesiones = string.Empty;
                var i = 0;
                foreach (var item in SelectNotaMedicaAuxiliar.CERTIFICADO_MEDICO.LESION)
                {
                    lesiones = lesiones + item.DESCR.Trim() + " EN " + item.ANATOMIA_TOPOGRAFICA.DESCR + (i == SelectNotaMedicaAuxiliar.CERTIFICADO_MEDICO.LESION.Count() ? string.Empty : ", \n");
                    i++;
                }

                var delitos = "";
                if (SelectIngreso.CAUSA_PENAL.Any(a => a.ID_ESTATUS_CP == (short)enumEstatusCausaPenal.ACTIVO))
                {
                    var count = 1;
                    foreach (var item in SelectIngreso.CAUSA_PENAL.First(a => a.ID_ESTATUS_CP == (short)enumEstatusCausaPenal.ACTIVO).CAUSA_PENAL_DELITO)
                    {
                        delitos = delitos + item.DESCR_DELITO + (count == SelectIngreso.CAUSA_PENAL.First(a => a.ID_ESTATUS_CP == (short)enumEstatusCausaPenal.ACTIVO).CAUSA_PENAL_DELITO.Count ? "" : ",");
                        count++;
                    }
                }

                var dientes = Convert.ToBase64String(new Imagenes().getImagen("miniDiente.png"));
                var _htmlDiente = "<html><table><tr><td>";
                var j = 0;
                for (j = 0; j < 16; j++)
                    _htmlDiente += "<image id='ImagenDiente' style='width:20px;height:50px;' src = \" data:image/png;base64, " + dientes + " alt=\"diente" + j + "\" /></td><td>";

                _htmlDiente += "</td></tr></table></html>";

                var odonto = new HistoriaClinicaViewModel(new Nullable<short>(), new Nullable<short>(), new Nullable<short>(), new Nullable<short>()).CaptureWebPageBytesP(_htmlDiente, 400, 100);

                #region ANTECEDENTES PAOTOLOGICOS
                var antecedentesPatologicos = string.Empty;
                if (SelectNotaMedicaAuxiliar != null ? SelectNotaMedicaAuxiliar.CERTIFICADO_MEDICO != null : false)
                {
                    var k = 1;
                    foreach (var item in SelectNotaMedicaAuxiliar.INGRESO.HISTORIA_CLINICA.SelectMany(s => s.HISTORIA_CLINICA_PATOLOGICOS))
                    {
                        antecedentesPatologicos = antecedentesPatologicos + item.PATOLOGICO_CAT.DESCR + (k <= SelectNotaMedicaAuxiliar.INGRESO.HISTORIA_CLINICA.SelectMany(s => s.HISTORIA_CLINICA_PATOLOGICOS).Count() ? ", " : "");
                        k++;
                    }
                }
                #endregion

                #region TOXICOMANIAS
                var toxicomanias = SelectNotaMedicaAuxiliar != null ?
                    SelectNotaMedicaAuxiliar.CERTIFICADO_MEDICO != null ?
                        string.IsNullOrEmpty(SelectNotaMedicaAuxiliar.CERTIFICADO_MEDICO.TOXICOMANIAS) ? "" : "SI, " + SelectNotaMedicaAuxiliar.CERTIFICADO_MEDICO.TOXICOMANIAS
                    : ""
                : "";
                #endregion

                #region OBJETO
                rds1.Value = new List<cReporteCertificadoNuevoIngreso>()
                {
                    new cReporteCertificadoNuevoIngreso
                    { 
                        OdontogramaImagen = SelectNotaMedicaAuxiliar.ID_TIPO_ATENCION == (short)enumAtencionTipo.CONSULTA_DENTAL ? odonto : null, 
                        DENTAL = SelectNotaMedicaAuxiliar.ID_TIPO_ATENCION == (short)enumAtencionTipo.CONSULTA_DENTAL ? "S" : "N",
                        Titulo = SelectNotaMedicaAuxiliar.ATENCION_SERVICIO != null ? SelectNotaMedicaAuxiliar.ATENCION_SERVICIO.DESCR : string.Empty,
                        Centro = SelectIngreso.CENTRO1 != null ? string.IsNullOrEmpty(SelectIngreso.CENTRO1.DESCR) ? string.Empty:SelectIngreso.CENTRO1.DESCR.Trim() : "",
                        Ciudad = SelectIngreso.CENTRO1 != null ? SelectIngreso.CENTRO1.MUNICIPIO != null ? (string.IsNullOrEmpty(SelectIngreso.CENTRO1.MUNICIPIO.MUNICIPIO1) ? string.Empty : SelectIngreso.CENTRO1.MUNICIPIO.MUNICIPIO1.Trim()) : string.Empty : string.Empty,
                        Director = SelectIngreso.CENTRO1 != null ? string.IsNullOrEmpty(SelectIngreso.CENTRO1.DIRECTOR) ? string.Empty : SelectIngreso.CENTRO1.DIRECTOR.Trim() : "",
                        Cedula = medico.CEDULA != null ? medico.CEDULA.Trim() : "",
                        Edad = new Fechas().CalculaEdad(SelectIngreso.IMPUTADO.NACIMIENTO_FECHA.HasValue ? SelectIngreso.IMPUTADO.NACIMIENTO_FECHA.Value : new DateTime()).ToString(),
                        FechaIngreso = SelectIngreso.FEC_INGRESO_CERESO.HasValue ? SelectIngreso.FEC_INGRESO_CERESO.Value.ToShortDateString() : new DateTime().ToShortDateString(),
                        FechaNacimiento = SelectIngreso.IMPUTADO.NACIMIENTO_FECHA.HasValue ? SelectIngreso.IMPUTADO.NACIMIENTO_FECHA.Value.ToShortDateString() : new DateTime().ToShortDateString(),
                        Fecha = hora + dia,
                        Escolaridad = SelectIngreso.ESCOLARIDAD != null ? (string.IsNullOrEmpty(SelectIngreso.ESCOLARIDAD.DESCR) ? string.Empty : SelectIngreso.ESCOLARIDAD.DESCR.Trim()) : "",
                        Originario = SelectIngreso.IMPUTADO.NACIMIENTO_MUNICIPIO.HasValue ? 
                            (string.IsNullOrEmpty(SelectIngreso.IMPUTADO.MUNICIPIO.MUNICIPIO1) ? 
                                string.Empty 
                            : SelectIngreso.IMPUTADO.MUNICIPIO.MUNICIPIO1.Trim()) + ", " + (SelectIngreso.IMPUTADO.MUNICIPIO != null ? 
                                SelectIngreso.IMPUTADO.MUNICIPIO.ENTIDAD != null ? 
                                    (string.IsNullOrEmpty(SelectIngreso.IMPUTADO.MUNICIPIO.ENTIDAD.DESCR) ? string.Empty : SelectIngreso.IMPUTADO.MUNICIPIO.ENTIDAD.DESCR.Trim())
                                : string.Empty
                            : string.Empty)
                        : SelectIngreso.IMPUTADO.NACIMIENTO_LUGAR,
                        NombreImputado = SelectIngreso.IMPUTADO.NOMBRE.Trim() + " " + SelectIngreso.IMPUTADO.PATERNO.Trim() + " " + (string.IsNullOrEmpty(SelectIngreso.IMPUTADO.MATERNO) ? string.Empty : SelectIngreso.IMPUTADO.MATERNO.Trim()),
                        TipoDelito = string.IsNullOrEmpty(delitos) ? "N/A" : delitos,
                        NombreMedico = medico.PERSONA.NOMBRE.Trim() + " " + medico.PERSONA.PATERNO.Trim() + " " + (string.IsNullOrEmpty(medico.PERSONA.MATERNO) ? string.Empty : medico.PERSONA.MATERNO.Trim()),
                        Sexo = medico.PERSONA.SEXO == "M" ? "MASCULINO" : medico.PERSONA.SEXO == "F" ? "FEMENINO" : string.Empty,
                        SignosVitales_FC = SelectNotaMedicaAuxiliar != null ? SelectNotaMedicaAuxiliar.NOTA_SIGNOS_VITALES != null ? SelectNotaMedicaAuxiliar.NOTA_SIGNOS_VITALES.FRECUENCIA_CARDIAC : string.Empty : string.Empty,
                        SignosVitales_FR = SelectNotaMedicaAuxiliar != null ? SelectNotaMedicaAuxiliar.NOTA_SIGNOS_VITALES != null ? SelectNotaMedicaAuxiliar.NOTA_SIGNOS_VITALES.FRECUENCIA_RESPIRA : string.Empty : string.Empty,
                        SignosVitales_T = SelectNotaMedicaAuxiliar != null ? SelectNotaMedicaAuxiliar.NOTA_SIGNOS_VITALES != null ? SelectNotaMedicaAuxiliar.NOTA_SIGNOS_VITALES.TEMPERATURA : string.Empty : string.Empty,
                        SignosVitales_TA = SelectNotaMedicaAuxiliar != null ? SelectNotaMedicaAuxiliar.NOTA_SIGNOS_VITALES != null ? SelectNotaMedicaAuxiliar.NOTA_SIGNOS_VITALES.TENSION_ARTERIAL : string.Empty : string.Empty,
                        Logo1 = Parametro.LOGO_ESTADO,
                        Logo2 = Parametro.REPORTE_LOGO2,
                        Dorso = Parametro.CUERPO_DORSO,
                        Frente = Parametro.CUERPO_FRENTE,
                        Check = Parametro.IMAGEN_ZONA_CORPORAL,
                        Interrogatorio = string.IsNullOrEmpty(lesiones) ? "NO" : lesiones,
                        AntecedentesPatologicos = string.IsNullOrEmpty(antecedentesPatologicos) ? "NO" : antecedentesPatologicos,
                        Toxicomanias = string.IsNullOrEmpty(toxicomanias) ? "NO" : string.IsNullOrEmpty(toxicomanias.Trim()) ? "NO" : "SI, " + toxicomanias,
                        PadecimientoTratamiento = SelectNotaMedicaAuxiliar != null ? SelectNotaMedicaAuxiliar.CERTIFICADO_MEDICO != null ? SelectNotaMedicaAuxiliar.CERTIFICADO_MEDICO.PADECIMIENTO : "" : "",
                        AmeritaHospitalizacion = SelectNotaMedicaAuxiliar != null ? SelectNotaMedicaAuxiliar.CERTIFICADO_MEDICO != null ? SelectNotaMedicaAuxiliar.CERTIFICADO_MEDICO.AMERITA_HOSPITALIZACION == "S" ? "SI" : "NO" : "NO" : "NO",
                        Diagnostico = SelectNotaMedicaAuxiliar != null ? SelectNotaMedicaAuxiliar.CERTIFICADO_MEDICO != null ? SelectNotaMedicaAuxiliar.CERTIFICADO_MEDICO.DIAGNOSTICO : string.Empty : string.Empty,
                        DiasEnSanar = SelectNotaMedicaAuxiliar != null?SelectNotaMedicaAuxiliar.CERTIFICADO_MEDICO != null ? SelectNotaMedicaAuxiliar.CERTIFICADO_MEDICO.TARDA_SANAR_15 == "S" ? "SI" : "NO" : "NO" : "NO",
                        Observaciones = SelectNotaMedicaAuxiliar != null ? SelectNotaMedicaAuxiliar.CERTIFICADO_MEDICO != null ? SelectNotaMedicaAuxiliar.CERTIFICADO_MEDICO.OBSERVACIONES : string.Empty : string.Empty,
                        PeligraVida = SelectNotaMedicaAuxiliar !=null ? SelectNotaMedicaAuxiliar.CERTIFICADO_MEDICO != null ? SelectNotaMedicaAuxiliar.CERTIFICADO_MEDICO.PELIGRA_VIDA == "S" ? "SI" : "NO" : "NO" : "NO",
                        PlanTerapeutico = SelectNotaMedicaAuxiliar !=null ? SelectNotaMedicaAuxiliar.CERTIFICADO_MEDICO != null ? SelectNotaMedicaAuxiliar.CERTIFICADO_MEDICO.PLAN_TERAPEUTICO : string.Empty : string.Empty,
                        Seguimiento = SelectNotaMedicaAuxiliar != null ? SelectNotaMedicaAuxiliar.ID_CITA_SEGUIMIENTO.HasValue ? "SI" : "NO" : "NO",
                        F_FRONTAL = ListLesiones!=null ? !ListLesiones.Any(a => a.REGION.ID_REGION == (short)enumAnatomiaTopografica.F_FRONTAL) : true,
                        D_ANTEBRAZO_DERECHO = ListLesiones!=null ? !ListLesiones.Any(a => a.REGION.ID_REGION == (short)enumAnatomiaTopografica.D_ANTEBRAZO_DERECHO) : true,
                        D_ANTEBRAZO_IZQUIERDO = ListLesiones!=null ? !ListLesiones.Any(a => a.REGION.ID_REGION == (short)enumAnatomiaTopografica.D_ANTEBRAZO_IZQUIERDO) : true,
                        D_BRAZO_POSTERIOR_DERECHO = ListLesiones!=null ? !ListLesiones.Any(a => a.REGION.ID_REGION == (short)enumAnatomiaTopografica.D_BRAZO_POSTERIOR_DERECHO) : true,
                        D_BRAZO_POSTERIOR_IZQUIERDO = ListLesiones!=null ? !ListLesiones.Any(a => a.REGION.ID_REGION == (short)enumAnatomiaTopografica.D_BRAZO_POSTERIOR_IZQUIERDO) : true,
                        D_CALCANEO_DERECHO = ListLesiones!=null ? !ListLesiones.Any(a => a.REGION.ID_REGION == (short)enumAnatomiaTopografica.D_CALCANEO_DERECHO) : true,
                        D_CALCANEO_IZQUIERDA = ListLesiones!=null ? !ListLesiones.Any(a => a.REGION.ID_REGION == (short)enumAnatomiaTopografica.D_CALCANEO_IZQUIERDA) : true,
                        D_CODO_DERECHO = ListLesiones!=null ? !ListLesiones.Any(a => a.REGION.ID_REGION == (short)enumAnatomiaTopografica.D_CODO_DERECHO) : true,
                        D_CODO_IZQUIERDO = ListLesiones!=null ? !ListLesiones.Any(a => a.REGION.ID_REGION == (short)enumAnatomiaTopografica.D_CODO_IZQUIERDO) : true,
                        D_COSTILLAS_COSTADO_DERECHO = ListLesiones!=null ? !ListLesiones.Any(a => a.REGION.ID_REGION == (short)enumAnatomiaTopografica.D_COSTILLAS_COSTADO_DERECHO) : true,
                        D_COSTILLAS_COSTADO_IZQUIERDO = ListLesiones!=null ? !ListLesiones.Any(a => a.REGION.ID_REGION == (short)enumAnatomiaTopografica.D_COSTILLAS_COSTADO_IZQUIERDO) : true,
                        D_DORSAL_DERECHA = ListLesiones != null ? !ListLesiones.Any(a => a.REGION.ID_REGION == (short)enumAnatomiaTopografica.D_DORSAL_DERECHA) : true,
                        D_DORSAL_IZQUIERDA = ListLesiones != null ? !ListLesiones.Any(a => a.REGION.ID_REGION == (short)enumAnatomiaTopografica.D_DORSAL_IZQUIERDA) : true,
                        D_ESCAPULAR_DERECHA = ListLesiones != null ? !ListLesiones.Any(a => a.REGION.ID_REGION == (short)enumAnatomiaTopografica.D_ESCAPULAR_DERECHA) : true,
                        D_ESCAPULAR_IZQUIERDA = ListLesiones != null ? !ListLesiones.Any(a => a.REGION.ID_REGION == (short)enumAnatomiaTopografica.D_ESCAPULAR_IZQUIERDA) : true,
                        D_FALANGES_POSTERIORES_DERECHA = ListLesiones != null ? !ListLesiones.Any(a => a.REGION.ID_REGION == (short)enumAnatomiaTopografica.D_FALANGES_POSTERIORES_DERECHA) : true,
                        D_FALANGES_POSTERIORES_IZQUIERDA = ListLesiones != null ? !ListLesiones.Any(a => a.REGION.ID_REGION == (short)enumAnatomiaTopografica.D_FALANGES_POSTERIORES_IZQUIERDA) : true,
                        D_GLUTEA_DERECHA = ListLesiones != null ? !ListLesiones.Any(a => a.REGION.ID_REGION == (short)enumAnatomiaTopografica.D_GLUTEA_DERECHA) : true,
                        D_GLUTEA_IZQUIERDA = ListLesiones != null ? !ListLesiones.Any(a => a.REGION.ID_REGION == (short)enumAnatomiaTopografica.D_GLUTEA_IZQUIERDA) : true,
                        D_LUMBAR_RENAL_DERECHA = ListLesiones != null ? !ListLesiones.Any(a => a.REGION.ID_REGION == (short)enumAnatomiaTopografica.D_LUMBAR_RENAL_DERECHA) : true,
                        D_LUMBAR_RENAL_IZQUIERDA = ListLesiones != null ? !ListLesiones.Any(a => a.REGION.ID_REGION == (short)enumAnatomiaTopografica.D_METATARZIANA_PLANTA_DERECHA) : true,
                        D_METATARZIANA_PLANTA_DERECHA = ListLesiones != null ? !ListLesiones.Any(a => a.REGION.ID_REGION == (short)enumAnatomiaTopografica.D_METATARZIANA_PLANTA_DERECHA) : true,
                        D_METATARZIANA_PLANTA_IZQUIERDA = ListLesiones != null ? !ListLesiones.Any(a => a.REGION.ID_REGION == (short)enumAnatomiaTopografica.D_METATARZIANA_PLANTA_IZQUIERDA) : true,
                        D_MUÑECA_POSTERIOR_DERECHA = ListLesiones != null ? !ListLesiones.Any(a => a.REGION.ID_REGION == (short)enumAnatomiaTopografica.D_MUÑECA_POSTERIOR_DERECHA) : true,
                        D_MUÑECA_POSTERIOR_IZQUIERDA = ListLesiones != null ? !ListLesiones.Any(a => a.REGION.ID_REGION == (short)enumAnatomiaTopografica.D_MUÑECA_POSTERIOR_IZQUIERDA) : true,
                        D_MUSLO_POSTERIOR_DERECHO = ListLesiones != null ? !ListLesiones.Any(a => a.REGION.ID_REGION == (short)enumAnatomiaTopografica.D_MUSLO_POSTERIOR_DERECHO) : true,
                        D_MUSLO_POSTERIOR_IZQUIERDO = ListLesiones != null ? !ListLesiones.Any(a => a.REGION.ID_REGION == (short)enumAnatomiaTopografica.D_MUSLO_POSTERIOR_IZQUIERDO) : true,
                        D_OCCIPITAL_NUCA = ListLesiones != null ? !ListLesiones.Any(a => a.REGION.ID_REGION == (short)enumAnatomiaTopografica.D_OCCIPITAL_NUCA) : true,
                        D_OREJA_POSTERIOR_DERECHA = ListLesiones != null ? !ListLesiones.Any(a => a.REGION.ID_REGION == (short)enumAnatomiaTopografica.D_OREJA_POSTERIOR_DERECHA) : true,
                        D_OREJA_POSTERIOR_IZQUIERDA = ListLesiones != null ? !ListLesiones.Any(a => a.REGION.ID_REGION == (short)enumAnatomiaTopografica.D_OREJA_POSTERIOR_IZQUIERDA) : true,
                        D_PANTORRILLA_DERECHA = ListLesiones!=null ? !ListLesiones.Any(a => a.REGION.ID_REGION == (short)enumAnatomiaTopografica.D_PANTORRILLA_DERECHA) : true,
                        D_PANTORRILLA_IZQUIERDA = ListLesiones!=null ? !ListLesiones.Any(a => a.REGION.ID_REGION == (short)enumAnatomiaTopografica.D_PANTORRILLA_IZQUIERDA) : true,
                        D_POPLITEA_DERECHA = ListLesiones!=null ? !ListLesiones.Any(a => a.REGION.ID_REGION == (short)enumAnatomiaTopografica.D_POPLITEA_DERECHA) : true,
                        D_POPLITEA_IZQUIERDA = ListLesiones!=null ? !ListLesiones.Any(a => a.REGION.ID_REGION == (short)enumAnatomiaTopografica.D_POPLITEA_IZQUIERDA) : true,
                        D_POSTERIOR_CABEZA = ListLesiones!=null ? !ListLesiones.Any(a => a.REGION.ID_REGION == (short)enumAnatomiaTopografica.D_POSTERIOR_CABEZA) : true,
                        D_POSTERIOR_CUELLO = ListLesiones!=null ? !ListLesiones.Any(a => a.REGION.ID_REGION == (short)enumAnatomiaTopografica.D_POSTERIOR_CUELLO) : true,
                        D_POSTERIOR_ENTREPIERNA_DERECHA = ListLesiones!=null ? !ListLesiones.Any(a => a.REGION.ID_REGION == (short)enumAnatomiaTopografica.D_POSTERIOR_ENTREPIERNA_DERECHA) : true,
                        D_POSTERIOR_ENTREPIERNA_IZQUIERDA = ListLesiones!=null ? !ListLesiones.Any(a => a.REGION.ID_REGION == (short)enumAnatomiaTopografica.D_POSTERIOR_ENTREPIERNA_IZQUIERDA) : true,
                        D_POSTERIOR_HOMBRO_DERECHO = ListLesiones!=null ? !ListLesiones.Any(a => a.REGION.ID_REGION == (short)enumAnatomiaTopografica.D_POSTERIOR_HOMBRO_DERECHO) : true,
                        D_POSTERIOR_HOMBRO_IZQUIERDO = ListLesiones!=null ? !ListLesiones.Any(a => a.REGION.ID_REGION == (short)enumAnatomiaTopografica.D_POSTERIOR_HOMBRO_IZQUIERDO) : true,
                        D_SACRA = ListLesiones != null ? !ListLesiones.Any(a => a.REGION.ID_REGION == (short)enumAnatomiaTopografica.D_SACRA) : true,
                        D_TOBILLO_DERECHO = ListLesiones!=null ? !ListLesiones.Any(a => a.REGION.ID_REGION == (short)enumAnatomiaTopografica.D_TOBILLO_DERECHO) : true,
                        D_TOBILLO_IZQUIERDO = ListLesiones!=null ? !ListLesiones.Any(a => a.REGION.ID_REGION == (short)enumAnatomiaTopografica.D_TOBILLO_IZQUIERDO) : true,
                        D_TORACICA_DORSAL = ListLesiones!=null ? !ListLesiones.Any(a => a.REGION.ID_REGION == (short)enumAnatomiaTopografica.D_TORACICA_DORSAL) : true,
                        D_VERTEBRAL_CERVICAL = ListLesiones!=null ? !ListLesiones.Any(a => a.REGION.ID_REGION == (short)enumAnatomiaTopografica.D_VERTEBRAL_CERVICAL) : true,
                        D_VERTEBRAL_LUMBAR = ListLesiones!=null ? !ListLesiones.Any(a => a.REGION.ID_REGION == (short)enumAnatomiaTopografica.D_VERTEBRAL_LUMBAR) : true,
                        D_VERTEBRAL_TORACICA = ListLesiones!=null ? !ListLesiones.Any(a => a.REGION.ID_REGION == (short)enumAnatomiaTopografica.D_VERTEBRAL_TORACICA) : true,
                        F_ANTEBRAZO_DERECHO = ListLesiones!=null ? !ListLesiones.Any(a => a.REGION.ID_REGION == (short)enumAnatomiaTopografica.F_ANTEBRAZO_DERECHO) : true,
                        F_ANTEBRAZO_IZQUIERDO = ListLesiones!=null ? !ListLesiones.Any(a => a.REGION.ID_REGION == (short)enumAnatomiaTopografica.F_ANTEBRAZO_IZQUIERDO) : true,
                        F_ANTERIOR_CUELLO = ListLesiones!=null ? !ListLesiones.Any(a => a.REGION.ID_REGION == (short)enumAnatomiaTopografica.F_ANTERIOR_CUELLO) : true,
                        F_MUÑECA_ANTERIOR_DERECHA = ListLesiones!=null ? !ListLesiones.Any(a => a.REGION.ID_REGION == (short)enumAnatomiaTopografica.F_MUÑECA_ANTERIOR_DERECHA) : true,
                        F_MUÑECA_ANTERIOR_IZQUIERDA = ListLesiones!=null ? !ListLesiones.Any(a => a.REGION.ID_REGION == (short)enumAnatomiaTopografica.F_MUÑECA_ANTERIOR_IZQUIERDA) : true,
                        F_AXILA_DERECHA = ListLesiones!=null ? !ListLesiones.Any(a => a.REGION.ID_REGION == (short)enumAnatomiaTopografica.F_AXILA_DERECHA) : true,
                        F_AXILA_IZQUIERDA = ListLesiones!=null ? !ListLesiones.Any(a => a.REGION.ID_REGION == (short)enumAnatomiaTopografica.F_AXILA_IZQUIERDA) : true,
                        F_BAJO_VIENTRE_HIPOGASTRIO = ListLesiones!=null ? !ListLesiones.Any(a => a.REGION.ID_REGION == (short)enumAnatomiaTopografica.F_BAJO_VIENTRE_HIPOGASTRIO) : true,
                        F_BRAZO_DERECHO = ListLesiones!=null ? !ListLesiones.Any(a => a.REGION.ID_REGION == (short)enumAnatomiaTopografica.F_BRAZO_DERECHO) : true,
                        F_BRAZO_IZQUIERDO = ListLesiones!=null ? !ListLesiones.Any(a => a.REGION.ID_REGION == (short)enumAnatomiaTopografica.F_BRAZO_IZQUIERDO) : true,
                        F_CARA_DERECHA = ListLesiones!=null ? !ListLesiones.Any(a => a.REGION.ID_REGION == (short)enumAnatomiaTopografica.F_CLAVICULAR_DERECHA) : true,
                        F_CARA_IZQUIERDA = ListLesiones!=null ? !ListLesiones.Any(a => a.REGION.ID_REGION == (short)enumAnatomiaTopografica.F_CARA_IZQUIERDA) : true,
                        F_CLAVICULAR_DERECHA = ListLesiones!=null ? !ListLesiones.Any(a => a.REGION.ID_REGION == (short)enumAnatomiaTopografica.F_CLAVICULAR_DERECHA) : true,
                        F_CLAVICULAR_IZQUIERDA = ListLesiones!=null ? !ListLesiones.Any(a => a.REGION.ID_REGION == (short)enumAnatomiaTopografica.F_CLAVICULAR_IZQUIERDA) : true,
                        F_CODO_DERECHO = ListLesiones!=null ? !ListLesiones.Any(a => a.REGION.ID_REGION == (short)enumAnatomiaTopografica.F_CODO_DERECHO) : true,
                        F_CODO_IZQUIERDO = ListLesiones!=null ? !ListLesiones.Any(a => a.REGION.ID_REGION == (short)enumAnatomiaTopografica.F_CODO_IZQUIERDO) : true,
                        F_ENTREPIERNA_ANTERIOR_DERECHA = ListLesiones!=null ? !ListLesiones.Any(a => a.REGION.ID_REGION == (short)enumAnatomiaTopografica.F_ENTREPIERNA_ANTERIOR_DERECHA) : true,
                        F_ENTREPIERNA_ANTERIOR_IZQUIERDA = ListLesiones!=null ? !ListLesiones.Any(a => a.REGION.ID_REGION == (short)enumAnatomiaTopografica.F_ENTREPIERNA_ANTERIOR_IZQUIERDA) : true,
                        F_EPIGASTRIO = ListLesiones != null ? !ListLesiones.Any(a => a.REGION.ID_REGION == (short)enumAnatomiaTopografica.F_EPIGASTRIO) : true,
                        F_ESCROTO = ListLesiones != null ? !ListLesiones.Any(a => a.REGION.ID_REGION == (short)enumAnatomiaTopografica.F_ESCROTO) : true,
                        F_ESPINILLA_DERECHA = ListLesiones != null ? !ListLesiones.Any(a => a.REGION.ID_REGION == (short)enumAnatomiaTopografica.F_ESPINILLA_DERECHA) : true,
                        F_ESPINILLA_IZQUIERDA = ListLesiones != null ? !ListLesiones.Any(a => a.REGION.ID_REGION == (short)enumAnatomiaTopografica.F_ESPINILLA_IZQUIERDA) : true,
                        F_FALANGES_MANO_DERECHA = ListLesiones != null ? !ListLesiones.Any(a => a.REGION.ID_REGION == (short)enumAnatomiaTopografica.F_FALANGES_MANO_DERECHA) : true,
                        F_FALANGES_MANO_IZQUIERDA = ListLesiones != null ? !ListLesiones.Any(a => a.REGION.ID_REGION == (short)enumAnatomiaTopografica.F_FALANGES_MANO_IZQUIERDA) : true,
                        F_FALANGES_PIE_DERECHO = ListLesiones != null ? !ListLesiones.Any(a => a.REGION.ID_REGION == (short)enumAnatomiaTopografica.F_FALANGES_PIE_DERECHO) : true,
                        F_FALANGES_PIE_IZQUIERDO = ListLesiones != null ? !ListLesiones.Any(a => a.REGION.ID_REGION == (short)enumAnatomiaTopografica.F_FALANGES_PIE_IZQUIERDO) : true,
                        F_HIPOCONDRIA_DERECHA = ListLesiones != null ? !ListLesiones.Any(a => a.REGION.ID_REGION == (short)enumAnatomiaTopografica.F_HIPOCONDRIA_DERECHA) : true,
                        F_HIPOCONDRIA_IZQUIERDA = ListLesiones != null ? !ListLesiones.Any(a => a.REGION.ID_REGION == (short)enumAnatomiaTopografica.F_HIPOCONDRIA_IZQUIERDA) : true,
                        F_HOMBRO_DERECHO = ListLesiones != null ? !ListLesiones.Any(a => a.REGION.ID_REGION == (short)enumAnatomiaTopografica.F_HOMBRO_DERECHO) : true,
                        F_HOMBRO_IZQUIERDO = ListLesiones != null ? !ListLesiones.Any(a => a.REGION.ID_REGION == (short)enumAnatomiaTopografica.F_HOMBRO_IZQUIERDO) : true,
                        F_INGUINAL_DERECHA = ListLesiones != null ? !ListLesiones.Any(a => a.REGION.ID_REGION == (short)enumAnatomiaTopografica.F_INGUINAL_DERECHA) : true,
                        F_INGUINAL_IZQUIERDA = ListLesiones != null ? !ListLesiones.Any(a => a.REGION.ID_REGION == (short)enumAnatomiaTopografica.F_INGUINAL_IZQUIERDA) : true,
                        F_LATERAL_CABEZA_DERECHO = ListLesiones != null ? !ListLesiones.Any(a => a.REGION.ID_REGION == (short)enumAnatomiaTopografica.F_LATERAL_CABEZA_DERECHO) : true,
                        F_LATERAL_CABEZA_IZQUIERDO = ListLesiones != null ? !ListLesiones.Any(a => a.REGION.ID_REGION == (short)enumAnatomiaTopografica.F_LATERAL_CABEZA_IZQUIERDO) : true,
                        F_MANDIBULA = ListLesiones != null ? !ListLesiones.Any(a => a.REGION.ID_REGION == (short)enumAnatomiaTopografica.F_MANDIBULA) : true,
                        F_METACARPIANOS_DERECHA = ListLesiones != null ? !ListLesiones.Any(a => a.REGION.ID_REGION == (short)enumAnatomiaTopografica.F_METACARPIANOS_DERECHA) : true,
                        F_METACARPIANOS_IZQUIERDA = ListLesiones != null ? !ListLesiones.Any(a => a.REGION.ID_REGION == (short)enumAnatomiaTopografica.F_METACARPIANOS_IZQUIERDA) : true,
                        F_METATARZIANA_DORSAL_DERECHO = ListLesiones != null ? !ListLesiones.Any(a => a.REGION.ID_REGION == (short)enumAnatomiaTopografica.F_METATARZIANA_DORSAL_DERECHO) : true,
                        F_METATARZIANA_DORSAL_IZQUIERDO = ListLesiones != null ? !ListLesiones.Any(a => a.REGION.ID_REGION == (short)enumAnatomiaTopografica.F_METATARZIANA_DORSAL_IZQUIERDO) : true,
                        F_MUSLO_ANTERIOR_DERECHO = ListLesiones!=null ? !ListLesiones.Any(a => a.REGION.ID_REGION == (short)enumAnatomiaTopografica.F_MUSLO_ANTERIOR_DERECHO) : true,
                        F_MUSLO_ANTERIOR_IZQUIERDO = ListLesiones!=null ? !ListLesiones.Any(a => a.REGION.ID_REGION == (short)enumAnatomiaTopografica.F_MUSLO_ANTERIOR_IZQUIERDO) : true,
                        F_NARIZ = ListLesiones != null ? !ListLesiones.Any(a => a.REGION.ID_REGION == (short)enumAnatomiaTopografica.F_NARIZ) : true,
                        F_ORBITAL_DERECHO = ListLesiones != null ? !ListLesiones.Any(a => a.REGION.ID_REGION == (short)enumAnatomiaTopografica.F_ORBITAL_DERECHO) : true,
                        F_ORBITAL_IZQUIERDO = ListLesiones != null ? !ListLesiones.Any(a => a.REGION.ID_REGION == (short)enumAnatomiaTopografica.F_ORBITAL_IZQUIERDO) : true,
                        F_OREJA_DERECHA = ListLesiones != null ? !ListLesiones.Any(a => a.REGION.ID_REGION == (short)enumAnatomiaTopografica.F_OREJA_DERECHA) : true,
                        F_OREJA_IZQUIERDA = ListLesiones != null ? !ListLesiones.Any(a => a.REGION.ID_REGION == (short)enumAnatomiaTopografica.F_OREJA_IZQUIERDA) : true,
                        F_PENE_VAGINA = ListLesiones != null ? !ListLesiones.Any(a => a.REGION.ID_REGION == (short)enumAnatomiaTopografica.F_PENE_VAGINA) : true,
                        F_PEZON_DERECHO = ListLesiones != null ? !ListLesiones.Any(a => a.REGION.ID_REGION == (short)enumAnatomiaTopografica.F_PEZON_DERECHO) : true,
                        F_PEZON_IZQUIERDO = ListLesiones != null ? !ListLesiones.Any(a => a.REGION.ID_REGION == (short)enumAnatomiaTopografica.F_PEZON_IZQUIERDO) : true,
                        F_RODILLA_DERECHA = ListLesiones != null ? !ListLesiones.Any(a => a.REGION.ID_REGION == (short)enumAnatomiaTopografica.F_RODILLA_DERECHA) : true,
                        F_RODILLA_IZQUIERDA = ListLesiones != null ? !ListLesiones.Any(a => a.REGION.ID_REGION == (short)enumAnatomiaTopografica.F_RODILLA_IZQUIERDA) : true,
                        F_TOBILLO_DERECHO = ListLesiones != null ? !ListLesiones.Any(a => a.REGION.ID_REGION == (short)enumAnatomiaTopografica.F_TOBILLO_DERECHO) : true,
                        F_TOBILLO_IZQUIERDO = ListLesiones != null ? !ListLesiones.Any(a => a.REGION.ID_REGION == (short)enumAnatomiaTopografica.F_TOBILLO_IZQUIERDO) : true,
                        F_TORAX_CENTRAL = ListLesiones != null ? !ListLesiones.Any(a => a.REGION.ID_REGION == (short)enumAnatomiaTopografica.F_TORAX_CENTRAL) : true,
                        F_TORAX_DERECHO=ListLesiones!=null ? !ListLesiones.Any(a => a.REGION.ID_REGION == (short)enumAnatomiaTopografica.F_TORAX_DERECHO) : true,
                        F_TORAX_IZQUIERDO=ListLesiones!=null ? !ListLesiones.Any(a => a.REGION.ID_REGION == (short)enumAnatomiaTopografica.F_TORAX_IZQUIERDO) : true,
                        F_UMBILICAL_MESOGASTIO = ListLesiones!=null ? !ListLesiones.Any(a => a.REGION.ID_REGION == (short)enumAnatomiaTopografica.F_UMBILICAL_MESOGASTIO) : true,
                        F_VACIO_DERECHA = ListLesiones!=null ? !ListLesiones.Any(a => a.REGION.ID_REGION == (short)enumAnatomiaTopografica.F_VACIO_DERECHA) : true,
                        F_VACIO_IZQUIERDA = ListLesiones!=null ? !ListLesiones.Any(a => a.REGION.ID_REGION == (short)enumAnatomiaTopografica.F_VACIO_IZQUIERDA) : true,
                        F_VERTICE_CABEZA = ListLesiones!=null ? !ListLesiones.Any(a => a.REGION.ID_REGION == (short)enumAnatomiaTopografica.F_VERTICE_CABEZA) : true
                    }
                };
                #endregion

                #region DENTAL
                ListaOdontogramaSeguimiento = new List<cReporteOdontogramaSeguimiento>();
                foreach (var item in SelectNotaMedicaAuxiliar.ODONTOGRAMA_SEGUIMIENTO2)
                {
                    ListaOdontogramaSeguimiento.Add(new cReporteOdontogramaSeguimiento
                    {
                        DESCR = string.Format("{0} EN {1} DEL {2}. POSICIÓN {3}", item.DENTAL_TRATAMIENTO == null ? item.ENFERMEDAD == null ? "" : item.ENFERMEDAD.NOMBRE.Trim() : item.DENTAL_TRATAMIENTO.DESCR.Trim(),
                            item.ODONTOGRAMA_TIPO.DESCR.Trim(), item.ODONTOGRAMA.DESCR.Trim(), item.ID_POSICION.ToString()),
                        ID = item.ID_CONSECUTIVO,
                    });
                }
                #endregion

                Application.Current.Dispatcher.Invoke((Action)(delegate
                {
                    ReporteVisible = Visibility.Visible;
                    Reporte.LocalReport.ReportPath = "Reportes/rCertificadoNuevoIngreso.rdlc";
                    Reporte.LocalReport.DataSources.Clear();
                    Reporte.LocalReport.DataSources.Add(rds1);
                    Reporte.ShowExportButton = false;
                    Reporte.SetDisplayMode(Microsoft.Reporting.WinForms.DisplayMode.PrintLayout);
                    Reporte.ZoomMode = Microsoft.Reporting.WinForms.ZoomMode.Percent;
                    Reporte.ZoomPercent = 100;
                    Reporte.Margin = new System.Windows.Forms.Padding(5);
                    #region Subreporte
                    //Reporte.LocalReport.SubreportProcessing += ProcesandoSubreporte;
                    #endregion
                    Reporte.RefreshReport();
                }));
            }
            catch (Exception ex)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al cargar la informacion del reporte.", ex);
                ReporteVisible = Visibility.Collapsed;
            }
        }

        private void ProcesandoSubreporte(object s, SubreportProcessingEventArgs e)
        {
            if (e.ReportPath.Equals("cSubreporteProcedimientos", StringComparison.InvariantCultureIgnoreCase))
                e.DataSources.Add(new ReportDataSource("DataSet1", ListSubreporteProcedimientos));
            else if (e.ReportPath.Equals("cSubreporteInsumoDetalle", StringComparison.InvariantCultureIgnoreCase))
                e.DataSources.Add(new ReportDataSource("DataSet1", ListSubreporteInsumos));
            else if (e.ReportPath.Equals("cSubreporteOdontograma", StringComparison.InvariantCultureIgnoreCase) && SelectNotaMedicaAuxiliar.ID_TIPO_ATENCION == (short)enumAtencionTipo.CONSULTA_DENTAL)
            {
                e.DataSources.Clear();
                e.DataSources.Add(new ReportDataSource("DataSet1", ListaOdontogramaSeguimiento));
            }
        }

        private void ConfiguraPermisos()
        {
            try
            {
                var permisos = new cProcesoUsuario().Obtener(enumProcesos.REPORTE_CERTIFICADO_MEDICO_NUEVO_INGRESO.ToString(), StaticSourcesViewModel.UsuarioLogin.Username);
                if (permisos.Any())
                {
                    foreach (var p in permisos)
                    {
                        if (p.INSERTAR == 1)
                            PInsertar = true;
                        if (p.EDITAR == 1)
                            PEditar = true;
                        if (p.IMPRIMIR == 1)
                            PImprimir = true;
                        if (p.CONSULTAR == 1)
                            PConsultar = true;
                    }
                }
            }
            catch (Exception ex)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al configurar permisos en la pantalla", ex);
            }
        }

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