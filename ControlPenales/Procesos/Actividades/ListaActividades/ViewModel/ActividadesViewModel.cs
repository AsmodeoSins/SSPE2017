using ControlPenales.BiometricoServiceReference;
using ControlPenales.Clases.ControlInternos;
using ControlPenales.Clases.ControlProgramas;
using ControlPenales.Procesos.Actividades.ListaActividades;
using DPUruNet;
using MahApps.Metro.Controls;
using MahApps.Metro.Controls.Dialogs;
using SSP.Controlador.Catalogo.Justicia;
using SSP.Controlador.Catalogo.Justicia.Actividades;
using SSP.Servidor;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

namespace ControlPenales
{
    public partial class ActividadesViewModel : ValidationViewModelBase, IPageViewModel
    {
        #region Variables
        public string Name
        {
            get
            {
                return "actividades";
            }
        }

        #endregion

        #region Constructor
        public ActividadesViewModel() { }
        #endregion

        #region Metodos Eventos
        public async void DoubleClick(object obj)
        {
            await StaticSourcesViewModel.CargarDatosMetodoAsync(() =>
            {
                ObtenerInternosActividad();
            });
            PopUpsViewModels.ShowPopUp(this, PopUpsViewModels.TipoPopUp.LISTA_ASISTENCIA);
        }


        public async void ClickSwitch(object obj)
        {

            switch (obj.ToString())
            {
                case "obtenerFotoInternoSeleccionado":
                    try
                    {
                        var foto_seguimiento = new cIngresoBiometrico().Obtener(SelectedInterno.Anio, SelectedInterno.Centro, SelectedInterno.IdImputado, (short)enumTipoBiometrico.FOTO_FRENTE_SEGUIMIENTO).Any() ?
                                                            new cIngresoBiometrico().Obtener(SelectedInterno.Anio, SelectedInterno.Centro, SelectedInterno.IdImputado, (short)enumTipoBiometrico.FOTO_FRENTE_SEGUIMIENTO).FirstOrDefault().BIOMETRICO : null;

                        var foto_registro = new cIngresoBiometrico().Obtener(SelectedInterno.Anio, SelectedInterno.Centro, SelectedInterno.IdImputado, (short)enumTipoBiometrico.FOTO_FRENTE_REGISTRO).Any() ?
                            new cIngresoBiometrico().Obtener(SelectedInterno.Anio, SelectedInterno.Centro, SelectedInterno.IdImputado, (short)enumTipoBiometrico.FOTO_FRENTE_REGISTRO).FirstOrDefault().BIOMETRICO : null;

                        FotoInternoSeleccionado = foto_seguimiento == null ? (foto_registro == null ? new Imagenes().getImagenPerson() : foto_registro) : foto_seguimiento;
                    }
                    catch (Exception)
                    {
                        FotoInternoSeleccionado = new Imagenes().getImagenPerson();
                    }

                    break;
                case "cerrarActividadSeleccionada":
                    PopUpsViewModels.ClosePopUp(PopUpsViewModels.TipoPopUp.LISTA_ASISTENCIA);
                    PopUpsViewModels.ClosePopUp(PopUpsViewModels.TipoPopUp.OSCURECER_FONDO);
                    break;
            }

        }


        public async void ObtenerActividadesPorFecha(object obj)
        {
            await StaticSourcesViewModel.CargarDatosMetodoAsync(() => {
                try
                {
                    var lista_actividades = new List<GRUPO_HORARIO>();


                    if (SelectedFecha.Year == Fechas.GetFechaDateServer.Year &&
                       SelectedFecha.Month == Fechas.GetFechaDateServer.Month &&
                           SelectedFecha.Day == Fechas.GetFechaDateServer.Day)
                        lista_actividades.AddRange(new cGrupoHorario().GetData(
                        g =>
                            g.HORA_INICIO.Value.Year == SelectedFecha.Year &&
                            g.HORA_INICIO.Value.Month == SelectedFecha.Month &&
                            g.HORA_INICIO.Value.Day == SelectedFecha.Day &&
                            g.HORA_INICIO.Value.Hour == Fechas.GetFechaDateServer.Hour).ToList());
                    else
                        lista_actividades.AddRange(new cGrupoHorario().GetData(
                        g =>
                            g.HORA_INICIO.Value.Year == SelectedFecha.Year &&
                            g.HORA_INICIO.Value.Month == SelectedFecha.Month &&
                            g.HORA_INICIO.Value.Day == SelectedFecha.Day
                            ).ToList());



                    List<GRUPO_HORARIO> actividades = new List<GRUPO_HORARIO>();
                    foreach (var Area in Areas)
                    {
                        actividades.AddRange(lista_actividades.Where(
                            w =>
                                w.ID_AREA == Area.ID_AREA
                                ).ToList());
                    }
                    ListaActividades = actividades;


                }
                catch (Exception ex)
                {
                    StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al obtener las actividades contempladas por el equipo.", ex);

                }
            });
            

        }

        #endregion

        #region Metodos
        void IPageViewModel.inicializa() { }

        public void ObtenerActividades()
        {
            try
            {
                var lista_actividades = new cGrupoHorario().GetData(
                    g =>
                        g.HORA_INICIO.Value.Year == Fechas.GetFechaDateServer.Year &&
                        g.HORA_INICIO.Value.Month == Fechas.GetFechaDateServer.Month &&
                        g.HORA_INICIO.Value.Day == Fechas.GetFechaDateServer.Day &&
                        g.HORA_INICIO.Value.Hour == Fechas.GetFechaDateServer.Hour).ToList();

                List<GRUPO_HORARIO> actividades = new List<GRUPO_HORARIO>();
                foreach (var Area in Areas)
                {
                    actividades.AddRange(lista_actividades.Where(
                        w =>
                            w.ID_AREA == Area.ID_AREA
                            ).ToList());
                }
                ListaActividades = actividades;


            }
            catch (Exception ex)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al obtener las actividades contempladas por el equipo.", ex);

            }

        }

        public void ObtenerAreas()
        {
            try
            {
                Areas = new cEquipo_Area().GetData(g =>
                    g.MAC_ADDRESS == GlobalVar.gMAC_ADDRESS &&
                    g.IP == GlobalVar.gIP).ToList();
            }
            catch (Exception ex)
            {

                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al obtener las áreas asignadas al equipo.", ex);
            }
        }

        public void ObtenerInternosActividad()
        {
            try
            {
                FotoInternoSeleccionado = new Imagenes().getImagenPerson();

                var lista_internos_actividad = new cGrupoAsistencia().GetData(
                     g =>
                         g.GRUPO_HORARIO.ID_GRUPO_HORARIO == SelectedActividad.ID_GRUPO_HORARIO &&
                         g.ID_GRUPO == SelectedActividad.ID_GRUPO
                     ).ToList();

                List<InternosActividad> internos = new List<InternosActividad>();
                foreach (var interno in lista_internos_actividad)
                {
                    var ingreso = interno.GRUPO_PARTICIPANTE.INGRESO;
                    internos.Add(new InternosActividad()
                    {
                        Anio = (short)interno.GRUPO_PARTICIPANTE.ING_ID_ANIO,
                        Centro = (short)interno.GRUPO_PARTICIPANTE.ING_ID_CENTRO,
                        IdImputado = (int)interno.GRUPO_PARTICIPANTE.ING_ID_IMPUTADO,
                        Expediente = string.Format("{0}/{1}", interno.GRUPO_PARTICIPANTE.ING_ID_ANIO, interno.GRUPO_PARTICIPANTE.ING_ID_IMPUTADO),
                        Nombre = ingreso.IMPUTADO.NOMBRE.TrimEnd(),
                        Paterno = ingreso.IMPUTADO.PATERNO.TrimEnd(),
                        Materno = ingreso.IMPUTADO.MATERNO.TrimEnd(),
                        Asistencia = interno.ASISTENCIA != null ? true : false
                    });
                }

                Responsable = string.Format("{1} {2} {0}",
                    SelectedActividad.GRUPO.PERSONA.NOMBRE.TrimEnd(),
                    SelectedActividad.GRUPO.PERSONA.PATERNO.TrimEnd(),
                    SelectedActividad.GRUPO.PERSONA.MATERNO.TrimEnd());

                ListaInternosActividad = internos;

            }
            catch (Exception ex)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al obtener los internos de la actividad.", ex);

            }
        }
        #endregion

        #region Inicializar
        private async void ActividadesLoad(ActividadesView obj)
        {
            try
            {
                await StaticSourcesViewModel.CargarDatosMetodoAsync(() =>
                {
                    MenuGuardarEnabled = false;
                    MenuBuscarEnabled = true;
                    MenuLimpiarEnabled = true;
                    MenuReporteEnabled = false;
                    MenuAyudaEnabled = true;
                    MenuSalirEnabled = true;
                    ObtenerAreas();
                    ObtenerActividades();
                    ///DESCOMENTAR EN CASO DE OCUPAR CONFIGURAR PRIVILEGIOS
                    //ConfiguraPermisos(); 
                });
                StaticSourcesViewModel.Mensaje("NOTA", "Para marcar asistencia de los internos, haga clic en 'Toma de asistencia'.", StaticSourcesViewModel.enumTipoMensaje.MENSAJE_INFORMACION, 5);
            }
            catch (Exception ex)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al cargar las actividades.", ex);
            }
        }
        #endregion

        #region Permisos
        ///DADO QUE NO SE VISUALIZA EN INTERFAZ EL COMO PRESENTAR AL USUARIO LA PRESENTACION VISUAL DE LOS PRIVILEGIOS, SE REALIZA CONFIGURACION BASE A ESPERA DE DETERMINAR (POR PARTE DEL DESARROLLADOR DEL MODULO) LA CONFIGURACION.

        //private void ConfiguraPermisos()
        //{
        //    try
        //    {
        //        var permisos = new cProcesoUsuario().Obtener(enumProcesos.CONTROL_DE_ACTIVIDADES.ToString(), StaticSourcesViewModel.UsuarioLogin.Username); //CONSULTA LA TABLA DE LOS PERMISOS, BUSCANDO POR EL NOMBRE DEL MODULO QUE SE DECLARA EN EL ENUMERADOR
        //        if (permisos.Any())//SI EXISTE ALGUNA CONFIGURACION DE PERMISOS PARA ESTE MODULO
        //        {
        //            foreach (var p in permisos)//POR LO REGULAR ES SOLO UN CAMPO, SE RECORRE Y SE AJUSTAN LAS VARIABLES PARA QUE LOS ENABLED DE LA BARRA DE MENU QUE SE USA(VARIA EN ALGUNAS VISTAS, CAMBIAR POR LAS VARIABLES QUE USA LA VISTA Y ADECUAR SEGUN LA NECESIDAD).
        //            {
        //                if (p.INSERTAR == 1)//VALOR QUE VIENE DESDE LA TABLA
        //                    PInsertar = true;//VARIABLE QUE DEBEN DECLARAR SI ES QUE USAN UNA VISTA DIFERENTE A LAS CONVENCIONALES.
        //                if (p.EDITAR == 1)
        //                    PEditar = true;
        //                if (p.CONSULTAR == 1)
        //                    PConsultar = true;
        //                if (p.IMPRIMIR == 1)
        //                    PImprimir = true;
        //            }
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al configurar permisos en la pantalla", ex);
        //    }
        //}
        #endregion
    }
}