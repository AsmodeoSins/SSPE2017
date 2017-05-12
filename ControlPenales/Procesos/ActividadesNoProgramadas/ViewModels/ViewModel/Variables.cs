using ControlPenales.BiometricoServiceReference;
using ControlPenales.Clases.ControlActividadesNoProgramadas;
using MahApps.Metro.Controls.Dialogs;
using SSP.Controlador.Catalogo.Justicia;
using SSP.Controlador.Catalogo.Justicia.Ingreso;
using SSP.Servidor;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ControlPenales
{
    public partial class ActividadesNoProgramadasViewModel
    {
        ///===CHECK===
        #region Propiedades Generales
        /// <summary>
        /// Propiedades generales del módulo
        /// 1.- Fecha actual en el servidor
        /// 2.- Foto a mostrar del imputado cuándo se selecciona la pestaña (tab) de TRASLADOS en cuestión al interno seleccionado
        /// 3.- Foto a mostrar del imputado cuándo se selecciona la pestaña (tab) de EXCARCELACIONES en cuestión al interno seleccionado
        /// 4.- ID del área de la salida del centro
        /// </summary>

        private DateTime fechaServer = Fechas.GetFechaDateServer;
        public DateTime FechaServer
        {
            get { return fechaServer; }
            set
            {
                fechaServer = value;
                OnPropertyChanged("FechaServer");
            }
        }

        private byte[] fotoImputadoTraslado;
        public byte[] FotoImputadoTraslado
        {
            get { return fotoImputadoTraslado; }
            set
            {
                fotoImputadoTraslado = value;
                OnPropertyChanged("FotoImputadoTraslado");
            }
        }

        private byte[] fotoImputadoExcarcelacion;
        public byte[] FotoImputadoExcarcelacion
        {
            get { return fotoImputadoExcarcelacion; }
            set
            {
                fotoImputadoExcarcelacion = value;
                OnPropertyChanged("FotoImputadoExcarcelacion");
            }
        }

        const short SALIDA_DE_CENTRO = 111;
        #endregion

        ///===CHECK===
        #region Propiedades Cambio Pestañas

        /// <summary>
        /// Propiedades utilizadas para la lógica y manejo del cambio de pestañas (tabs)
        /// 1.- Habilita/Deshabilita la visibilidad de la selección de estatus para excarcelaciones
        /// 2.- Habilita/Deshabilita la visibilidad de la selección de estatus para traslados
        /// 3.- Índice que indica que pestaña esta seleccionada
        /// </summary>

        private bool estatusExcarcelacionesVisible;
        public bool EstatusExcarcelacionesVisible
        {
            get { return estatusExcarcelacionesVisible; }
            set
            {
                estatusExcarcelacionesVisible = value;
                OnPropertyChanged("EstatusExcarcelacionesVisible");
            }
        }

        private bool estatusTrasladosVisible;
        public bool EstatusTrasladosVisible
        {
            get { return estatusTrasladosVisible; }
            set
            {
                estatusTrasladosVisible = value;
                OnPropertyChanged("EstatusTrasladosVisible");
            }
        }

        private int indexTab = 0;
        public int IndexTab
        {
            get { return indexTab; }
            set
            {
                indexTab = value;
                //Si el valor del índice es 0, entonces...
                if (indexTab == 0)
                {
                    //Se deshabilita la selección de estatus para excarcelaciones
                    EstatusExcarcelacionesVisible = false;
                    //Se habilita la selección de estatus para traslados
                    EstatusTrasladosVisible = true;
                    //Se inicializa el traslado seleccionado en nulo
                    SelectedTrasladoDetalle = null;
                }
                //Si el valor del índice es 1, entonces...
                else if (indexTab == 1)
                {
                    //Se habilita la selección de estatus para excarcelaciones
                    EstatusExcarcelacionesVisible = true;
                    //Se deshabilita la selección de estatus para traslados
                    EstatusTrasladosVisible = false;
                    //Se inicializa la excarcelación jurídica seleccionada en nulo
                    SelectedExcarcelacionJuridica = null;
                    //Se inicializa la excarcelación médica seleccionada en nulo
                    SelectedExcarcelacionMedica = null;
                    //Se inicializa el ingreso seleccionado
                    SelectedIngreso = null;
                }
                //Se obtiene la imagen por defecto a mostrar en las fotos
                var placeholder = new Imagenes().getImagenPerson();
                //Se inicializan ambas fotos con la imgen por defecto
                FotoImputadoTraslado = placeholder;
                FotoImputadoExcarcelacion = placeholder;
                OnPropertyChanged("IndexTab");
            }
        }
        #endregion

        ///===CHECK===
        #region Propiedades Traslados

        /// <summary>
        /// Propiedades que se utilizan en la pestaña de traslados
        /// 1 - 5.-Estatus de los traslados (referirse a las constantes para ver los estatus)
        /// 6.- Lista de traslados existentes
        /// 7.- Lista de internos en el traslado seleccionado
        /// 8.- Traslado seleccionado
        /// 9.- Interno seleccionado de un traslado
        /// </summary>


        const string TRASLADO_PROGRAMADO = "PR";
        const string TRASLADO_CANCELADO = "CA";
        const string TRASLADO_ACTIVO = "AC";
        const string TRASLADO_EN_PROCESO = "EP";
        const string TRASLADO_FINALIZADO = "FI";


        private List<TRASLADO> listaTraslados;
        public List<TRASLADO> ListaTraslados
        {
            get { return listaTraslados; }
            set
            {
                listaTraslados = value;
                OnPropertyChanged("ListaTraslados");
            }
        }

        private List<InternoTrasladoDetalle> listaTrasladosDetalle;
        public List<InternoTrasladoDetalle> ListaTrasladosDetalle
        {
            get { return listaTrasladosDetalle; }
            set
            {
                listaTrasladosDetalle = value;
                OnPropertyChanged("ListaTrasladosDetalle");
            }
        }

        private TRASLADO selectedTraslado;
        public TRASLADO SelectedTraslado
        {
            get { return selectedTraslado; }
            set
            {
                var ingreso_ubicacion = new cIngresoUbicacion();
                selectedTraslado = value;
                try
                {
                    var lista_internos_traslado = new List<InternoTrasladoDetalle>();
                    if (value != null)
                    {
                        foreach (var interno_traslado in value.TRASLADO_DETALLE)
                        {
                            if (interno_traslado.ID_ESTATUS == TRASLADO_ACTIVO ||
                                interno_traslado.ID_ESTATUS == TRASLADO_EN_PROCESO ||
                                interno_traslado.ID_ESTATUS == TRASLADO_PROGRAMADO)
                            {
                                var ultima_ubicacion = ingreso_ubicacion.ObtenerUltimaUbicacion(interno_traslado.ID_ANIO, interno_traslado.ID_CENTRO, interno_traslado.ID_IMPUTADO, interno_traslado.ID_INGRESO);
                                lista_internos_traslado.Add(new InternoTrasladoDetalle()
                                {
                                    Centro = interno_traslado.ID_CENTRO,
                                    Anio = interno_traslado.ID_ANIO,
                                    IdImputado = interno_traslado.ID_IMPUTADO,
                                    Paterno = interno_traslado.INGRESO.IMPUTADO.PATERNO != null ? interno_traslado.INGRESO.IMPUTADO.PATERNO.TrimEnd() : string.Empty,
                                    Materno = interno_traslado.INGRESO.IMPUTADO.MATERNO != null ? interno_traslado.INGRESO.IMPUTADO.MATERNO.TrimEnd() : string.Empty,
                                    Nombre = interno_traslado.INGRESO.IMPUTADO.NOMBRE != null ? interno_traslado.INGRESO.IMPUTADO.NOMBRE.TrimEnd() : string.Empty,
                                    Estatus = new Func<string>(() =>
                                    {
                                        var EstatusInterno = "";
                                        switch (interno_traslado.ID_ESTATUS)
                                        {
                                            case TRASLADO_ACTIVO:
                                                EstatusInterno = "ACTIVO";
                                                break;
                                            case TRASLADO_CANCELADO:
                                                EstatusInterno = "CANCELADO";
                                                break;
                                            case TRASLADO_PROGRAMADO:
                                                EstatusInterno = "PROGRAMADO";
                                                break;
                                            case TRASLADO_EN_PROCESO:
                                                EstatusInterno = "EN PROCESO";
                                                break;
                                        }
                                        return EstatusInterno;
                                    })(),
                                    EnSalidaCentro = ultima_ubicacion != null ?
                                    (ultima_ubicacion.ID_AREA == SALIDA_DE_CENTRO && ultima_ubicacion.ESTATUS == (short)enumUbicacion.ACTIVIDAD && (
                                    ultima_ubicacion.INGRESO.ID_ESTATUS_ADMINISTRATIVO != (short)enumEstatusAdministrativo.LIBERADO &&
                                    ultima_ubicacion.INGRESO.ID_ESTATUS_ADMINISTRATIVO != (short)enumEstatusAdministrativo.TRASLADADO &&
                                    ultima_ubicacion.INGRESO.ID_ESTATUS_ADMINISTRATIVO != (short)enumEstatusAdministrativo.SUJETO_A_PROCESO_EN_LIBERTAD &&
                                    ultima_ubicacion.INGRESO.ID_ESTATUS_ADMINISTRATIVO != (short)enumEstatusAdministrativo.DISCRECIONAL
                                    )) : false

                                });
                            }
                        }
                        ListaTrasladosDetalle = lista_internos_traslado;
                    }
                }
                catch (Exception ex)
                {
                    StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al obtener la lista de los internos pertenecientes al traslado seleccionado", ex);
                }

                OnPropertyChanged("SelectedTraslado");
            }
        }

        private InternoTrasladoDetalle selectedTrasladoDetalle;
        public InternoTrasladoDetalle SelectedTrasladoDetalle
        {
            get { return selectedTrasladoDetalle; }
            set
            {
                selectedTrasladoDetalle = value;
                OnPropertyChanged("SelectedTrasladoDetalle");
            }
        }
        #endregion

        ///===CHECK===
        #region Propiedades Excarcelaciones
        /// <summary>
        /// Propiedades que se utilizan en la pestaña de excarcelaciones
        /// 1 - 6.- Estatus de las excarcelaciones (referirse a las constantes para ver los estatus)
        /// 7.- Lista de ingresos con excarcelaciones
        /// 8.- Interno seleccionado
        /// 9.- Lista de excarcelaciones jurídicas
        /// 10.- Lista de excarcelaciones médicas
        /// 11.- Excarcelación jurídica seleccionada
        /// 12.- Excarcelación médica seleccionada
        /// 13.- Clase para identificar los destinos de una excarcelación
        /// 14.- Lista de destinos de la excarcelación seleccionada
        /// </summary>

        const string EXCARCELACION_PROGRAMADA = "PR";
        const string EXCARCELACION_CANCELADA = "CA";
        const string EXCARCELACION_ACTIVA = "AC";
        const string EXCARCELACION_CONCLUIDA = "CO";
        const string EXCARCELACION_EN_PROCESO = "EP";
        const string EXCARCELACION_AUTORIZADA = "AU";



        private List<InternoIngresoExcarcelacion> listaIngresos;
        public List<InternoIngresoExcarcelacion> ListaIngresos
        {
            get { return listaIngresos; }
            set
            {
                listaIngresos = value;
                OnPropertyChanged("ListaIngresos");
            }
        }

        private InternoIngresoExcarcelacion selectedIngreso;
        public InternoIngresoExcarcelacion SelectedIngreso
        {
            get { return selectedIngreso; }
            set
            {
                selectedIngreso = value;
                if (value != null)
                {
                    List<EXCARCELACION> excarcelaciones_juridicas = new List<EXCARCELACION>();
                    List<EXCARCELACION> excarcelaciones_medicas = new List<EXCARCELACION>();
                    var excarcelaciones_ingreso = new cExcarcelacion().ObtenerImputadoExcarcelaciones(value.Id_Centro, value.Id_Anio, value.Id_Imputado, value.Id_Ingreso);
                    foreach (var excarcelacion in excarcelaciones_ingreso)
                    {
                        if (excarcelacion.ID_ESTATUS == DecisionEstatusExcarcelaciones())
                        {
                            if (excarcelacion.ID_TIPO_EX == (short)enumTipoExcarcelacion.JURIDICA)
                            {
                                excarcelaciones_juridicas.Add(excarcelacion);
                            }
                            else
                            {
                                excarcelaciones_medicas.Add(excarcelacion);
                            }
                        }
                    }
                    ListaExcarcelacionesJuridicas = excarcelaciones_juridicas;
                    ListaExcarcelacionesMedicas = excarcelaciones_medicas;
                }
                OnPropertyChanged("SelectedIngreso");
            }
        }

        private List<EXCARCELACION> listaExcarcelacionesJuridicas;
        public List<EXCARCELACION> ListaExcarcelacionesJuridicas
        {
            get { return listaExcarcelacionesJuridicas; }
            set
            {
                listaExcarcelacionesJuridicas = value;
                OnPropertyChanged("ListaExcarcelacionesJuridicas");
            }
        }

        private List<EXCARCELACION> listaExcarcelacionesMedicas;
        public List<EXCARCELACION> ListaExcarcelacionesMedicas
        {
            get { return listaExcarcelacionesMedicas; }
            set
            {
                listaExcarcelacionesMedicas = value;
                OnPropertyChanged("ListaExcarcelacionesMedicas");
            }
        }

        private EXCARCELACION selectedExcarcelacionJuridica;
        public EXCARCELACION SelectedExcarcelacionJuridica
        {
            get { return selectedExcarcelacionJuridica; }
            set
            {
                if (SelectedExcarcelacionMedica != null)
                    SelectedExcarcelacionMedica = null;

                selectedExcarcelacionJuridica = value;
                var destinos = new List<Destino>();
                if (value != null)
                {
                    foreach (var destino in selectedExcarcelacionJuridica.EXCARCELACION_DESTINO)
                    {
                        destinos.Add(new Destino() { DESTINO_EXCARCELACION = destino.JUZGADO.DESCR });
                    }
                    ListaDestinos = destinos;
                }

                OnPropertyChanged("SelectedExcarcelacionJuridica");
            }
        }

        private EXCARCELACION selectedExcarcelacionMedica;
        public EXCARCELACION SelectedExcarcelacionMedica
        {
            get { return selectedExcarcelacionMedica; }
            set
            {
                if (SelectedExcarcelacionJuridica != null)
                    SelectedExcarcelacionJuridica = null;
                selectedExcarcelacionMedica = value;
                var destinos = new List<Destino>();
                if (value != null)
                {
                    foreach (var destino in selectedExcarcelacionMedica.EXCARCELACION_DESTINO)
                    {
                        destinos.Add(new Destino() { DESTINO_EXCARCELACION = destino.INTERCONSULTA_SOLICITUD.HOJA_REFERENCIA_MEDICA.Any()?
                        destino.INTERCONSULTA_SOLICITUD.HOJA_REFERENCIA_MEDICA.First().ID_HOSPITAL.Value==Parametro.ID_HOSPITAL_OTROS?destino.INTERCONSULTA_SOLICITUD.HOJA_REFERENCIA_MEDICA.First().HOSPITAL_OTRO
                        :destino.INTERCONSULTA_SOLICITUD.HOJA_REFERENCIA_MEDICA.First().HOSPITAL.DESCR:""});
                    }
                    ListaDestinos = destinos;
                }

                OnPropertyChanged("SelectedExcarcelacionMedica");
            }
        }

        public class Destino
        {
            public string DESTINO_EXCARCELACION { get; set; }
        }

        private List<Destino> listaDestinos;
        public List<Destino> ListaDestinos
        {
            get { return listaDestinos; }
            set
            {
                listaDestinos = value;
                OnPropertyChanged("ListaDestinos");
            }
        }
        #endregion

        ///===CHECK===
        #region Propiedades Permisos
        private bool pConsultar = false;
        public bool PConsultar
        {
            get { return pConsultar; }
            set
            {
                pConsultar = value;
                OnPropertyChanged("PConsultar");
                //if (value)
                //    BuscarEnabled = value;
            }
        }

        private bool pEditar = false;
        public bool PEditar
        {
            get { return pEditar; }
            set
            {
                pEditar = value;
                OnPropertyChanged("PEditar");
            }
        }
        #endregion

        ///===CHECK===
        #region Enumerables
        public enum enumUbicacion
        {
            ESTANCIA = 0,
            EN_TRANSITO = 1,
            ACTIVIDAD = 2
        }

        private enumEstatusTraslado selectedEstatusTraslado;
        public enumEstatusTraslado SelectedEstatusTraslado
        {
            get { return selectedEstatusTraslado; }
            set
            {
                selectedEstatusTraslado = value;
                OnPropertyChanged("SelectedEstatusTraslado");
            }
        }

        private enum enumTipoExcarcelacion
        {
            JURIDICA = 1,
            MEDICA = 2
        }

        private enumEstatusExcarcelacion selectedEstatusExcarcelacion;
        public enumEstatusExcarcelacion SelectedEstatusExcarcelacion
        {
            get { return selectedEstatusExcarcelacion; }
            set
            {
                selectedEstatusExcarcelacion = value;
                OnPropertyChanged("SelectedEstatusExcarcelacion");
            }
        }
        #endregion

        /////===CHECK===
        //#region Enableds
        ///// <summary>
        ///// Propiedades que habilitan o deshabilitan controles del módulo
        ///// 1.- Habilita/Deshabilita la búsqueda de traslados/excarcelaciones
        ///// </summary>
        //private bool buscarEnabled;
        //public bool BuscarEnabled
        //{
        //    get { return buscarEnabled; }
        //    set
        //    {
        //        buscarEnabled = value;
        //        OnPropertyChanged("BuscarEnabled");
        //    }
        //}
        //#endregion

        ///===CHECK===
        #region Menu Enabled
        /// <summary>
        /// Propiedades que habilitan o deshabilitan controles del menú
        /// 1.- Habilita/Deshabilita la búsqueda de traslados/excarcelaciones en el menú
        /// </summary>
        private bool menuBuscarEnabled;
        public bool MenuBuscarEnabled
        {
            get { return menuBuscarEnabled; }
            set
            {
                menuBuscarEnabled = value;
                OnPropertyChanged("MenuBuscarEnabled");
            }
        }
        #endregion
    }
}
