using ControlPenales.BiometricoServiceReference;
using ControlPenales.ControlInternosEdificio.ViewModel;
using SSP.Controlador.Catalogo.Justicia;
using SSP.Controlador.Catalogo.Justicia.Ingreso;
using SSP.Servidor;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Media;

namespace ControlPenales.Clases.ControlInternos
{
    public class InternosRequeridos
    {
        public InternosRequeridos() { }

        public InternosRequeridos(TimeSpan? HoraDeInicio = null, TimeSpan? HoraDeFin = null)
        {
            Hora = HoraDeInicio;
            HoraFin = HoraDeFin;
        }

        #region [VARIABLES]
        public string LlaveInterno { get; set; }
        public string Expediente { get; set; }
        public short Centro { get; set; }
        public short Anio { get; set; }
        public int IdImputado { get; set; }
        public short IdIngreso { get; set; }
        public string Nombre { get; set; }
        public string Paterno { get; set; }
        public string Materno { get; set; }
        public string Estancia { get; set; }
        public string NIP { get; set; }
        public short? IdArea { get; set; }
        public int? IdAduana { get; set; }// NECESARIO PARA SABER SI TERMINO SU VISITA
        public string Ubicacion { get; set; }// UBICACION ACTUAL DEL INTERNO
        public DateTime? Fecha { get; set; }
        public DateTime? FechaTermino { get; set; }
        public TimeSpan? Hora { get; set; }
        public TimeSpan? HoraFin { get; set; }
        public string Area { get; set; }
        public string Actividad { get; set; }
        public DateTime? FechaRegistro { get; set; }
        public int Id_Grupo { get; set; }
        public int? Empalme { get; set; } //SECUENCIAL DEL EMPALME!
        public decimal? EmpalmeAprobado { get; set; }
        public decimal? EmpalmeCoordinacion { get; set; }
        public short? Prioridad { get; set; }
        public bool Comparado { get; set; }
        public bool BooleanToRowTraslado { get; set; }// EN ROJO SI INTERNO ES TRUE PARA TRASLADO
        public bool BooleanToRowExcarcelacion { get; set; }// EN AMARILLO SI INTERNO ES TRUE PARA EXCARCELACION
        public bool BooleanToRowCitaMedica { get; set; } // EN CAFE SI INTERNO ES TRUE PARA CITA MEDICA
        public bool BooleanToRowVisitaLegal { get; set; } // EN VERDE SI INTERNO ES TRUE PARA VISITA LEGAL
        public bool BooleanToRowVisitaFamiliar { get; set; } // EN PURPLE SI INTERNO ES TRUE PARA VISITA FAMILIAR
        public bool BooleanToRowVisitaIntima { get; set; } // EN MAGENTA SI INTERNO ES TRUE PARA VISITA INTIMA
        public bool YardaInterno { get; set; }
        public bool CitaInterno { get; set; }
        public INGRESO Ingreso { get; set; }
        public YARDA Yarda { get; set; }
        public ATENCION_CITA AtencionCita { get; set; }
        public bool TrasladoInterno { get; set; }
        public bool ExcarcelacionInterno { get; set; }
        public bool CitaMedica { get; set; }
        public bool Citas { get; set; }
        public bool VisitaLegal { get; set; }
        public bool VisitaIntima { get; set; }
        public bool VisitaFamiliar { get; set; }
        public bool InternoRequerido { get; set; } //NECESARIO PARA NO PERMITIR SELECCIONAR 
        public bool ActividadInterno { get; set; }
        public bool EstudioInterno { get; set; }
        public DateTime? FechaTraslado { get; set; }
        #endregion

        #region[PARAMETROS]
        private string id_excarcelacion_programada = Parametro.ID_EXCARCELACION_PROGRAMADA;
        private string id_excarcelacion_activa = Parametro.ID_EXCARCELACION_ACTIVA;
        private string id_excarcelacion_proceso = Parametro.ID_EXCARCELACION_PROCESO;
        private string id_excarcelacion_autorizado = Parametro.ID_EXCARCELACION_AUTORIZADO;
        private string id_traslado_proceso = Parametro.ID_TRASLADO_PROCESO;
        private string id_traslado_activo = Parametro.ID_TRASLADO_ACTIVO;
        private string id_traslado_programado = Parametro.ID_TRASLADO_PROGRAMADO;
        public string ubicacion_estancia = Parametro.UBICACION_ESTANCIA;
        public string ubicacion_transito = Parametro.UBICACION_TRANSITO;
        public short IdAreaVisitaLegal = Parametro.ID_AREA_VISITA_LEGAL;
        public short Tolerancia = Parametro.TOLERANCIA_ACTIVIDAD_EDIFICIO;
        public short ToleranciaTraslado = Parametro.TOLERANCIA_TRASLADO_EDIFICIO;
        #endregion

        //CHECKBOX SELECCIONAR INTERNO
        #region [PROPIEDADES]
        private bool _selectEnabled;
        public bool SelectEnabled
        {
            get { return _selectEnabled; }
            set { _selectEnabled = value; }
        }

        private bool _seleccionar;
        public bool SELECCIONAR
        {
            get { return _seleccionar; }
            set
            {
                if (BooleanToRowTraslado)
                {
                    FechaTraslado = Fecha.Value.AddHours(-ToleranciaTraslado);

                    if (FechaTraslado.Value.Day == Fechas.GetFechaDateServer.Day && FechaTraslado.Value.Month == Fechas.GetFechaDateServer.Month && FechaTraslado.Value.Year == Fechas.GetFechaDateServer.Year)
                    {
                        if ((FechaTraslado.Value.TimeOfDay.TotalMinutes) <= Fechas.GetFechaDateServer.TimeOfDay.TotalMinutes)
                        {
                            if (InternoRequerido)
                                _seleccionar = false;
                            else
                                _seleccionar = value;
                        }
                    }
                }
                if (Fecha.Value.Day == Fechas.GetFechaDateServer.Day && Fecha.Value.Month == Fechas.GetFechaDateServer.Month && Fecha.Value.Year == Fechas.GetFechaDateServer.Year)
                {
                    if (BooleanToRowVisitaLegal || BooleanToRowVisitaFamiliar || BooleanToRowVisitaIntima)
                    {
                        if ((Fecha.Value.TimeOfDay.TotalMinutes - Tolerancia) <= Fechas.GetFechaDateServer.TimeOfDay.TotalMinutes)
                        {
                            if (InternoRequerido)
                                _seleccionar = false;
                            else
                                _seleccionar = value;
                        }
                    }
                    else
                    {
                        //SELECCIONA INTERNOS CON CITAS MEDICAS Y/O CITAS
                        if (BooleanToRowCitaMedica || Citas)
                        {
                            if ((Fecha.Value.TimeOfDay.TotalMinutes - Tolerancia) <= Fechas.GetFechaDateServer.TimeOfDay.TotalMinutes)
                            {
                                if (InternoRequerido)
                                    _seleccionar = false;
                                else
                                    _seleccionar = value;
                            }
                        }
                        else if (BooleanToRowExcarcelacion)
                        {
                            if (InternoRequerido)
                                _seleccionar = false;
                            else
                                _seleccionar = value;
                        }
                        else if (YardaInterno)
                        {
                            if ((Hora.Value.TotalMinutes - Tolerancia) <= Fechas.GetFechaDateServer.TimeOfDay.TotalMinutes)
                            {
                                if (InternoRequerido)
                                    _seleccionar = false;
                                else
                                    _seleccionar = value;
                            }
                        }
                        else if (ActividadInterno)
                        {
                            if ((Fecha.Value.TimeOfDay.TotalMinutes - Tolerancia) <= Fechas.GetFechaDateServer.TimeOfDay.TotalMinutes && FechaTermino.Value.TimeOfDay > Fechas.GetFechaDateServer.TimeOfDay)
                                if (InternoRequerido)
                                    _seleccionar = false;
                                else
                                    _seleccionar = value;
                        }
                        else if (EstudioInterno)
                        {
                            if ((Hora.Value.TotalMinutes - Tolerancia) <= Fechas.GetFechaDateServer.TimeOfDay.TotalMinutes)
                            {
                                _seleccionar = value;
                            }
                        }
                    }
                }
            }
        }
        #endregion

        //TODO pendiente traslados de SALVADOR RUIZ GUEVARA
        #region [METODOS]
        public List<InternosRequeridos> ListaInternos(DateTime? fechaInicio, DateTime? fechaFin, short ID_CENTRO, short? ID_UB_EDIFICIO = null, short? ID_UB_SECTOR = null, bool filtrarBusqueda = false, string busqueda = "")
        {
            try
            {
                List<EMPLEADO> lista_medicos = new List<EMPLEADO>();
                var result_interno_yarda = InternosYarda(ID_CENTRO, ID_UB_EDIFICIO, ID_UB_SECTOR);
                var result_interno_cita_medica = InternosCitaMedica(fechaInicio, fechaFin, ID_CENTRO);
                var usuario = new cUsuarioRol().ObtenerTodosMedicos();
                var aduana = new List<ADUANA>(new cAduana().ObtenerXDia(ID_CENTRO, Fechas.GetFechaDateServer));
                var imputado = new List<InternosRequeridos>();
                var ingreso = new cIngreso();
                var interno_ubicacion = new InternosAusentes().ListInternosAusentes(ID_CENTRO);

                //SE DETERMINA SI HAY ALGUN MEDICO ACTIVO DENTRO DEL CERESO
                if (usuario != null)
                    foreach (var item in usuario)
                    {
                        var x = item.USUARIO;
                        var empleado_medico = x.EMPLEADO;
                        var medicos_activos = aduana.Find(f => f.ID_PERSONA == empleado_medico.ID_EMPLEADO && empleado_medico.ID_CENTRO == ID_CENTRO && empleado_medico.ID_DEPARTAMENTO == 12 && empleado_medico.ESTATUS == "S");
                        if (medicos_activos != null)
                        {
                            lista_medicos.Add(empleado_medico);
                        }
                    }

                #region [INTERNOS CITA-MEDICA]
                var interno_requerido_cita = false; //VARIABLE PARA DETERMINAR SI EL INTERNO ES REQUERIDO Y SE ENCUENTRA FUERA DE SU CELDA
                if (result_interno_cita_medica != null)
                    foreach (var row in result_interno_cita_medica.Where(w => w.AtencionCita.ESTATUS == "N" && (w.AtencionCita.ID_AREA == 45 || w.AtencionCita.ID_AREA == 46)))
                    {
                        var consulta_interno_cita = row.Ingreso;
                        //FILTRO PARA SABER SI EL INTERNO CON CITA SE ENCUENTRA FUERA DEL EDIFICIO
                        var citas_medicas = interno_ubicacion.Where(w => w.Expediente == row.Expediente).FirstOrDefault();
                        //SI EL INTERNO SE ENCUENTRA FUERA DE SU CELDA PERO ES REQUERIDO...
                        if (citas_medicas != null)
                        {
                            interno_requerido_cita = true;
                        }
                        else
                        {
                            interno_requerido_cita = false;
                        }
                        //FILTRADO POR TODOS LOS EDIFICIOS Y SECTORES
                        if (ID_UB_EDIFICIO == -1 && ID_UB_SECTOR == -1)
                        {
                            if ((row.AtencionCita.CITA_FECHA_HORA.Value.Day == Fechas.GetFechaDateServer.Day && row.AtencionCita.CITA_FECHA_HORA.Value.Month == Fechas.GetFechaDateServer.Month && row.AtencionCita.CITA_FECHA_HORA.Value.Year == Fechas.GetFechaDateServer.Year &&
                               (row.AtencionCita.CITA_FECHA_HORA.Value.TimeOfDay <= Fechas.GetFechaDateServer.TimeOfDay && row.AtencionCita.CITA_HORA_TERMINA.Value.TimeOfDay > Fechas.GetFechaDateServer.TimeOfDay)) || row.AtencionCita.CITA_FECHA_HORA.Value >= Fechas.GetFechaDateServer)
                            {
                                if (ingreso.Activo(consulta_interno_cita))
                                    if (row.AtencionCita.ID_AREA == 45 || row.AtencionCita.ID_AREA == 46)
                                    {
                                        row.CitaMedica = true;
                                        row.InternoRequerido = interno_requerido_cita == true ? true : false;
                                    }
                                imputado.Add(row);
                            }
                        }
                        //FILTRADO POR EDIFICIO-SECTOR
                        else
                        {
                            if ((row.AtencionCita.CITA_FECHA_HORA.Value.Day == Fechas.GetFechaDateServer.Day && row.AtencionCita.CITA_FECHA_HORA.Value.Month == Fechas.GetFechaDateServer.Month && row.AtencionCita.CITA_FECHA_HORA.Value.Year == Fechas.GetFechaDateServer.Year &&
                               (row.AtencionCita.CITA_FECHA_HORA.Value.TimeOfDay <= Fechas.GetFechaDateServer.TimeOfDay && row.AtencionCita.CITA_HORA_TERMINA.Value.TimeOfDay > Fechas.GetFechaDateServer.TimeOfDay)) || row.AtencionCita.CITA_FECHA_HORA.Value >= Fechas.GetFechaDateServer)
                            {
                                if (ingreso.Activo(consulta_interno_cita) && EstaUbicacion(consulta_interno_cita, ID_UB_EDIFICIO, ID_UB_SECTOR))
                                {
                                    if (row.AtencionCita.ID_AREA == 45 || row.AtencionCita.ID_AREA == 46)
                                    {
                                        row.CitaMedica = true;
                                        InternoRequerido = interno_requerido_cita == true ? true : false;
                                    }
                                    imputado.Add(row);
                                }
                            }
                        }
                    }
                #endregion

                #region [INTERNOS CON EXCARCELACION]
                var interno_requerido_excarcelacion = false; //VARIABLE PARA DETERMINAR SI EL INTERNO ES REQUERIDO Y SE ENCUENTRA FUERA DE SU CELDA
                var interno_excarcelacion = new cExcarcelacion().ObtenerTodosExc(fechaInicio.Value, fechaFin.Value, ID_CENTRO);
                //NECESARIO PARA SABER ESTATUS EXCARCELACION
                var temp_excarcelacion = 0;

                if (id_excarcelacion_autorizado == "AU")
                {
                    temp_excarcelacion = 1;
                }
                //QUERY QUE OBTIENE INTERNOS CON TRASLADO POR ID_IMPUTADO Y POR EL ULTIMO CONSECUTIVO DEL MISMO
                var filtro_excarcelacion = (from p in interno_excarcelacion
                                            where p.ID_CONSEC ==
                                            (from pp in interno_excarcelacion
                                             where pp.ID_IMPUTADO == p.ID_IMPUTADO
                                             select pp.ID_CONSEC).Max()
                                            select p).ToList();
                if (filtro_excarcelacion != null)
                    foreach (var x in filtro_excarcelacion)
                    {
                        var consulta_imputado_visita = x.INGRESO.IMPUTADO;
                        var consulta_interno_excarcelacion = x.INGRESO;
                        var edificioInterno = consulta_interno_excarcelacion.CAMA.CELDA.SECTOR.EDIFICIO;
                        var sectorInterno = consulta_interno_excarcelacion.CAMA.CELDA.SECTOR;
                        var celdaInterno = consulta_interno_excarcelacion.CAMA.CELDA;
                        var ingresovisita = new cIngreso();
                        var excarcelacion_interno = filtro_excarcelacion.Where(w => w.ID_IMPUTADO == x.ID_IMPUTADO && w.ID_ESTATUS == "AU" && w.ID_CONSEC == x.ID_CONSEC).Count();
                        //SI LA EXCARCELACION SE EMPALMA CON CITA MEDICA...
                        var interno_exc_con_cita_medica = imputado.Find(f => f.IdImputado == consulta_interno_excarcelacion.ID_IMPUTADO && (f.FechaTermino.Value.TimeOfDay > x.PROGRAMADO_FEC.Value.TimeOfDay &&
                            f.Fecha.Value.TimeOfDay <= x.PROGRAMADO_FEC.Value.TimeOfDay));
                        //FILTRO PARA SABER SI EL INTERNO CON EXCARCELACION SE ENCUENTRA FUERA DEL EDIFICIO
                        var interno_exc_fuera_edificio = interno_ubicacion.Where(w => w.IdCentro == x.ID_CENTRO && w.IdAnio == x.ID_ANIO && w.IdImputado == x.ID_IMPUTADO && w.IdIngreso == x.ID_INGRESO).FirstOrDefault();
                        //SI EL INTERNO SE ENCUENTRA FUERA DE SU CELDA PERO ES REQUERIDO...
                        if (interno_exc_fuera_edificio != null)
                        {
                            interno_requerido_excarcelacion = true;
                        }
                        else
                        {
                            interno_requerido_excarcelacion = false;
                        }
                        //FILTRADO POR TODOS LOS EDIFICIOS
                        if (ID_UB_EDIFICIO == -1 && ID_UB_SECTOR == -1)
                        {
                            if (interno_exc_con_cita_medica == null)
                            {
                                if (ingreso.Activo(consulta_interno_excarcelacion))
                                {
                                    if (x.PROGRAMADO_FEC.Value.Day == Fechas.GetFechaDateServer.Day && x.PROGRAMADO_FEC.Value.Month == Fechas.GetFechaDateServer.Month && x.PROGRAMADO_FEC.Value.Year == Fechas.GetFechaDateServer.Year)
                                    {
                                        if (x.CERTIFICADO_MEDICO == 0)
                                        {
                                            if (excarcelacion_interno > 0)
                                            {
                                                imputado.Add(new InternosRequeridos()
                                                {
                                                    LlaveInterno = string.Format("{0}/{1}/{2}/{3}", x.ID_CENTRO, x.ID_ANIO, x.ID_IMPUTADO, consulta_interno_excarcelacion.ID_INGRESO),
                                                    Centro = consulta_interno_excarcelacion.ID_CENTRO,
                                                    Anio = consulta_interno_excarcelacion.ID_ANIO,
                                                    IdImputado = consulta_interno_excarcelacion.ID_IMPUTADO,
                                                    IdArea = 111,
                                                    IdIngreso = consulta_interno_excarcelacion.ID_INGRESO,
                                                    Expediente = string.Format("{0}/{1}", x.ID_ANIO, x.ID_IMPUTADO),
                                                    Paterno = string.IsNullOrEmpty(consulta_imputado_visita.PATERNO) ? string.Empty : consulta_imputado_visita.PATERNO.TrimEnd(),
                                                    Materno = string.IsNullOrEmpty(consulta_imputado_visita.MATERNO) ? string.Empty : consulta_imputado_visita.MATERNO.TrimEnd(),
                                                    Nombre = string.IsNullOrEmpty(consulta_imputado_visita.NOMBRE) ? string.Empty : consulta_imputado_visita.NOMBRE.TrimEnd(),
                                                    Ubicacion = string.IsNullOrEmpty(ObtenerUbicacion()) ? string.Empty : ObtenerUbicacion().TrimEnd(),
                                                    Estancia = string.Format("{0}-{1}-{2}-{3}", string.IsNullOrEmpty(edificioInterno.DESCR) ? string.Empty : edificioInterno.DESCR.TrimEnd(), string.IsNullOrEmpty(sectorInterno.DESCR) ? string.Empty : sectorInterno.DESCR.TrimEnd(),
                                                    string.IsNullOrEmpty(celdaInterno.ID_CELDA) ? string.Empty : celdaInterno.ID_CELDA.TrimEnd(), consulta_interno_excarcelacion.CAMA.ID_CAMA),
                                                    Actividad = "EXCARCELACIÓN",
                                                    Area = "SALIDA DEL CENTRO",
                                                    Fecha = x.PROGRAMADO_FEC,
                                                    FechaTermino = x.PROGRAMADO_FEC,
                                                    Hora = new TimeSpan(x.PROGRAMADO_FEC.Value.Ticks),
                                                    HoraFin = new TimeSpan(x.PROGRAMADO_FEC.Value.Ticks),
                                                    ExcarcelacionInterno = excarcelacion_interno == temp_excarcelacion,
                                                    Ingreso = consulta_interno_excarcelacion,
                                                    InternoRequerido = interno_requerido_excarcelacion == true ? true : false
                                                });
                                            }
                                        }
                                        else
                                        {
                                            if (excarcelacion_interno > 0 && lista_medicos.Any())
                                            {
                                                imputado.Add(new InternosRequeridos()
                                                {
                                                    LlaveInterno = string.Format("{0}/{1}/{2}/{3}", x.ID_CENTRO, x.ID_ANIO, x.ID_IMPUTADO, consulta_interno_excarcelacion.ID_INGRESO),
                                                    Centro = consulta_interno_excarcelacion.ID_CENTRO,
                                                    Anio = consulta_interno_excarcelacion.ID_ANIO,
                                                    IdImputado = consulta_interno_excarcelacion.ID_IMPUTADO,
                                                    IdArea = 46,
                                                    IdIngreso = consulta_interno_excarcelacion.ID_INGRESO,
                                                    Expediente = string.Format("{0}/{1}", x.ID_ANIO, x.ID_IMPUTADO),
                                                    Paterno = string.IsNullOrEmpty(consulta_imputado_visita.PATERNO) ? string.Empty : consulta_imputado_visita.PATERNO.TrimEnd(),
                                                    Materno = string.IsNullOrEmpty(consulta_imputado_visita.MATERNO) ? string.Empty : consulta_imputado_visita.MATERNO.TrimEnd(),
                                                    Nombre = string.IsNullOrEmpty(consulta_imputado_visita.NOMBRE) ? string.Empty : consulta_imputado_visita.NOMBRE.TrimEnd(),
                                                    Ubicacion = string.IsNullOrEmpty(ObtenerUbicacion()) ? string.Empty : ObtenerUbicacion().TrimEnd(),
                                                    Estancia = string.Format("{0}-{1}-{2}-{3}", string.IsNullOrEmpty(edificioInterno.DESCR) ? string.Empty : edificioInterno.DESCR.TrimEnd(), string.IsNullOrEmpty(sectorInterno.DESCR) ? string.Empty : sectorInterno.DESCR.TrimEnd(),
                                                    string.IsNullOrEmpty(celdaInterno.ID_CELDA) ? string.Empty : celdaInterno.ID_CELDA.TrimEnd(), consulta_interno_excarcelacion.CAMA.ID_CAMA),
                                                    Actividad = "EXCARCELACIÓN",
                                                    Area = "ÁREA MÉDICA PB",
                                                    Fecha = x.PROGRAMADO_FEC,
                                                    FechaTermino = x.PROGRAMADO_FEC,
                                                    Hora = new TimeSpan(x.PROGRAMADO_FEC.Value.Ticks),
                                                    HoraFin = new TimeSpan(x.PROGRAMADO_FEC.Value.Ticks),
                                                    CitaInterno = true,
                                                    ExcarcelacionInterno = excarcelacion_interno == temp_excarcelacion,
                                                    Ingreso = consulta_interno_excarcelacion,
                                                    InternoRequerido = interno_requerido_excarcelacion == true ? true : false
                                                });
                                            }
                                            else if (excarcelacion_interno > 0)
                                            {
                                                imputado.Add(new InternosRequeridos()
                                                {
                                                    LlaveInterno = string.Format("{0}/{1}/{2}/{3}", x.ID_CENTRO, x.ID_ANIO, x.ID_IMPUTADO, consulta_interno_excarcelacion.ID_INGRESO),
                                                    Centro = consulta_interno_excarcelacion.ID_CENTRO,
                                                    Anio = consulta_interno_excarcelacion.ID_ANIO,
                                                    IdImputado = consulta_interno_excarcelacion.ID_IMPUTADO,
                                                    IdArea = 111,
                                                    IdIngreso = consulta_interno_excarcelacion.ID_INGRESO,
                                                    Expediente = string.Format("{0}/{1}", x.ID_ANIO, x.ID_IMPUTADO),
                                                    Paterno = string.IsNullOrEmpty(consulta_imputado_visita.PATERNO) ? string.Empty : consulta_imputado_visita.PATERNO.TrimEnd(),
                                                    Materno = string.IsNullOrEmpty(consulta_imputado_visita.MATERNO) ? string.Empty : consulta_imputado_visita.MATERNO.TrimEnd(),
                                                    Nombre = string.IsNullOrEmpty(consulta_imputado_visita.NOMBRE) ? string.Empty : consulta_imputado_visita.NOMBRE.TrimEnd(),
                                                    Ubicacion = string.IsNullOrEmpty(ObtenerUbicacion()) ? string.Empty : ObtenerUbicacion().TrimEnd(),
                                                    Estancia = string.Format("{0}-{1}-{2}-{3}", string.IsNullOrEmpty(edificioInterno.DESCR) ? string.Empty : edificioInterno.DESCR.TrimEnd(), string.IsNullOrEmpty(sectorInterno.DESCR) ? string.Empty : sectorInterno.DESCR.TrimEnd(),
                                                    string.IsNullOrEmpty(celdaInterno.ID_CELDA) ? string.Empty : celdaInterno.ID_CELDA.TrimEnd(), consulta_interno_excarcelacion.CAMA.ID_CAMA),
                                                    Actividad = "EXCARCELACIÓN",
                                                    Area = "SALIDA DEL CENTRO",
                                                    Fecha = x.PROGRAMADO_FEC,
                                                    FechaTermino = x.PROGRAMADO_FEC,
                                                    Hora = new TimeSpan(x.PROGRAMADO_FEC.Value.Ticks),
                                                    HoraFin = new TimeSpan(x.PROGRAMADO_FEC.Value.Ticks),
                                                    ExcarcelacionInterno = excarcelacion_interno == temp_excarcelacion,
                                                    Ingreso = consulta_interno_excarcelacion,
                                                    InternoRequerido = interno_requerido_excarcelacion == true ? true : false
                                                });
                                            }
                                        }
                                    }
                                }
                            }
                        }
                        //FILTRADO POR EDIFICIO
                        else
                        {
                            if (interno_exc_con_cita_medica == null)
                            {
                                if (ingreso.Activo(consulta_interno_excarcelacion))
                                {
                                    if (x.PROGRAMADO_FEC.Value.Day == Fechas.GetFechaDateServer.Day && x.PROGRAMADO_FEC.Value.Month == Fechas.GetFechaDateServer.Month && x.PROGRAMADO_FEC.Value.Year == Fechas.GetFechaDateServer.Year)
                                    {
                                        if (ingresovisita.Activo(consulta_interno_excarcelacion) && EstaUbicacion(consulta_interno_excarcelacion, ID_UB_EDIFICIO, ID_UB_SECTOR))
                                        {
                                            if (x.CERTIFICADO_MEDICO == 0)
                                            {
                                                if (excarcelacion_interno > 0)
                                                {
                                                    imputado.Add(new InternosRequeridos()
                                                    {
                                                        LlaveInterno = string.Format("{0}/{1}/{2}/{3}", x.ID_CENTRO, x.ID_ANIO, x.ID_IMPUTADO, consulta_interno_excarcelacion.ID_INGRESO),
                                                        Centro = consulta_interno_excarcelacion.ID_CENTRO,
                                                        Anio = consulta_interno_excarcelacion.ID_ANIO,
                                                        IdImputado = consulta_interno_excarcelacion.ID_IMPUTADO,
                                                        IdArea = 111,
                                                        IdIngreso = consulta_interno_excarcelacion.ID_INGRESO,
                                                        Expediente = string.Format("{0}/{1}", x.ID_ANIO, x.ID_IMPUTADO),
                                                        Paterno = string.IsNullOrEmpty(consulta_imputado_visita.PATERNO) ? string.Empty : consulta_imputado_visita.PATERNO.TrimEnd(),
                                                        Materno = string.IsNullOrEmpty(consulta_imputado_visita.MATERNO) ? string.Empty : consulta_imputado_visita.MATERNO.TrimEnd(),
                                                        Nombre = string.IsNullOrEmpty(consulta_imputado_visita.NOMBRE) ? string.Empty : consulta_imputado_visita.NOMBRE.TrimEnd(),
                                                        Ubicacion = string.IsNullOrEmpty(ObtenerUbicacion()) ? string.Empty : ObtenerUbicacion().TrimEnd(),
                                                        Estancia = string.Format("{0}-{1}-{2}-{3}", string.IsNullOrEmpty(edificioInterno.DESCR) ? string.Empty : edificioInterno.DESCR.TrimEnd(), string.IsNullOrEmpty(sectorInterno.DESCR) ? string.Empty : sectorInterno.DESCR.TrimEnd(),
                                                        string.IsNullOrEmpty(celdaInterno.ID_CELDA) ? string.Empty : celdaInterno.ID_CELDA.TrimEnd(), consulta_interno_excarcelacion.CAMA.ID_CAMA),
                                                        Actividad = "EXCARCELACIÓN",
                                                        Area = "SALIDA DEL CENTRO",
                                                        Fecha = x.PROGRAMADO_FEC,
                                                        FechaTermino = x.PROGRAMADO_FEC,
                                                        Hora = new TimeSpan(x.PROGRAMADO_FEC.Value.Ticks),
                                                        HoraFin = new TimeSpan(x.PROGRAMADO_FEC.Value.Ticks),
                                                        ExcarcelacionInterno = excarcelacion_interno == temp_excarcelacion,
                                                        Ingreso = consulta_interno_excarcelacion,
                                                        InternoRequerido = interno_requerido_excarcelacion == true ? true : false
                                                    });
                                                }
                                            }
                                            else
                                            {
                                                if (excarcelacion_interno > 0 && lista_medicos.Any())
                                                {
                                                    imputado.Add(new InternosRequeridos()
                                                    {
                                                        LlaveInterno = string.Format("{0}/{1}/{2}/{3}", x.ID_CENTRO, x.ID_ANIO, x.ID_IMPUTADO, consulta_interno_excarcelacion.ID_INGRESO),
                                                        Centro = consulta_interno_excarcelacion.ID_CENTRO,
                                                        Anio = consulta_interno_excarcelacion.ID_ANIO,
                                                        IdImputado = consulta_interno_excarcelacion.ID_IMPUTADO,
                                                        IdArea = 46,
                                                        IdIngreso = consulta_interno_excarcelacion.ID_INGRESO,
                                                        Expediente = string.Format("{0}/{1}", x.ID_ANIO, x.ID_IMPUTADO),
                                                        Paterno = string.IsNullOrEmpty(consulta_imputado_visita.PATERNO) ? string.Empty : consulta_imputado_visita.PATERNO.TrimEnd(),
                                                        Materno = string.IsNullOrEmpty(consulta_imputado_visita.MATERNO) ? string.Empty : consulta_imputado_visita.MATERNO.TrimEnd(),
                                                        Nombre = string.IsNullOrEmpty(consulta_imputado_visita.NOMBRE) ? string.Empty : consulta_imputado_visita.NOMBRE.TrimEnd(),
                                                        Ubicacion = string.IsNullOrEmpty(ObtenerUbicacion()) ? string.Empty : ObtenerUbicacion().TrimEnd(),
                                                        Estancia = string.Format("{0}-{1}-{2}-{3}", string.IsNullOrEmpty(edificioInterno.DESCR) ? string.Empty : edificioInterno.DESCR.TrimEnd(), string.IsNullOrEmpty(sectorInterno.DESCR) ? string.Empty : sectorInterno.DESCR.TrimEnd(),
                                                        string.IsNullOrEmpty(celdaInterno.ID_CELDA) ? string.Empty : celdaInterno.ID_CELDA.TrimEnd(), consulta_interno_excarcelacion.CAMA.ID_CAMA),
                                                        Actividad = "EXCARCELACIÓN",
                                                        Area = "ÁREA MÉDICA PB",
                                                        Fecha = x.PROGRAMADO_FEC,
                                                        FechaTermino = x.PROGRAMADO_FEC,
                                                        Hora = new TimeSpan(x.PROGRAMADO_FEC.Value.Ticks),
                                                        HoraFin = new TimeSpan(x.PROGRAMADO_FEC.Value.Ticks),
                                                        CitaInterno = true,
                                                        ExcarcelacionInterno = excarcelacion_interno == temp_excarcelacion,
                                                        Ingreso = consulta_interno_excarcelacion,
                                                        InternoRequerido = interno_requerido_excarcelacion == true ? true : false
                                                    });
                                                }
                                                else if (excarcelacion_interno > 0)
                                                {
                                                    imputado.Add(new InternosRequeridos()
                                                    {
                                                        LlaveInterno = string.Format("{0}/{1}/{2}/{3}", x.ID_CENTRO, x.ID_ANIO, x.ID_IMPUTADO, consulta_interno_excarcelacion.ID_INGRESO),
                                                        Centro = consulta_interno_excarcelacion.ID_CENTRO,
                                                        Anio = consulta_interno_excarcelacion.ID_ANIO,
                                                        IdImputado = consulta_interno_excarcelacion.ID_IMPUTADO,
                                                        IdArea = 111,
                                                        IdIngreso = consulta_interno_excarcelacion.ID_INGRESO,
                                                        Expediente = string.Format("{0}/{1}", x.ID_ANIO, x.ID_IMPUTADO),
                                                        Paterno = string.IsNullOrEmpty(consulta_imputado_visita.PATERNO) ? string.Empty : consulta_imputado_visita.PATERNO.TrimEnd(),
                                                        Materno = string.IsNullOrEmpty(consulta_imputado_visita.MATERNO) ? string.Empty : consulta_imputado_visita.MATERNO.TrimEnd(),
                                                        Nombre = string.IsNullOrEmpty(consulta_imputado_visita.NOMBRE) ? string.Empty : consulta_imputado_visita.NOMBRE.TrimEnd(),
                                                        Ubicacion = string.IsNullOrEmpty(ObtenerUbicacion()) ? string.Empty : ObtenerUbicacion().TrimEnd(),
                                                        Estancia = string.Format("{0}-{1}-{2}-{3}", string.IsNullOrEmpty(edificioInterno.DESCR) ? string.Empty : edificioInterno.DESCR.TrimEnd(), string.IsNullOrEmpty(sectorInterno.DESCR) ? string.Empty : sectorInterno.DESCR.TrimEnd(),
                                                        string.IsNullOrEmpty(celdaInterno.ID_CELDA) ? string.Empty : celdaInterno.ID_CELDA.TrimEnd(), consulta_interno_excarcelacion.CAMA.ID_CAMA),
                                                        Actividad = "EXCARCELACIÓN",
                                                        Area = "SALIDA DEL CENTRO",
                                                        Fecha = x.PROGRAMADO_FEC,
                                                        FechaTermino = x.PROGRAMADO_FEC,
                                                        Hora = new TimeSpan(x.PROGRAMADO_FEC.Value.Ticks),
                                                        HoraFin = new TimeSpan(x.PROGRAMADO_FEC.Value.Ticks),
                                                        ExcarcelacionInterno = excarcelacion_interno == temp_excarcelacion,
                                                        Ingreso = consulta_interno_excarcelacion,
                                                        InternoRequerido = interno_requerido_excarcelacion == true ? true : false
                                                    });
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                #endregion

                #region [INTERNOS CON TRASLADO]
                var interno_requerido_traslado = false; //VARIABLE PARA DETERMINAR SI EL INTERNO ES REQUERIDO Y SE ENCUENTRA FUERA DE SU CELDA
                var interno_traslado = new cTrasladoDetalle().ObtenerTrasladoTodos(fechaInicio, fechaFin, ID_CENTRO);
                //NECESARIO PARA SABER ESTATUS TRASLADO
                var temp_traslado = 0;

                if (id_traslado_programado == "PR")
                {
                    temp_traslado = 1;
                }
                //QUERY PARA OBTENER INTERNOS CON TRASLADO POR ID_IMPUTADO Y POR EL ULTIMO TRASLADO PROGRAMADO
                var filtro_traslado = (from p in interno_traslado
                                       where p.ID_TRASLADO ==
                                       (from pp in interno_traslado
                                        where pp.ID_IMPUTADO == p.ID_IMPUTADO && pp.ID_INGRESO == p.ID_INGRESO
                                        select pp.ID_TRASLADO).Max()
                                       select p).ToList();
                if (filtro_traslado != null)
                    foreach (var x in filtro_traslado)
                    {
                        var consulta_imputado_traslado = x.INGRESO.IMPUTADO;
                        var consulta_interno_traslado = x.INGRESO;
                        var ingreso_traslado = new cIngreso();
                        var v = x.TRASLADO;
                        var edificioInterno = consulta_interno_traslado.CAMA.CELDA.SECTOR.EDIFICIO;
                        var sectorInterno = consulta_interno_traslado.CAMA.CELDA.SECTOR;
                        var celdaInterno = consulta_interno_traslado.CAMA.CELDA;
                        var traslado_interno = filtro_traslado.Where(w => w.ID_ESTATUS == "PR" && w.ID_IMPUTADO == x.ID_IMPUTADO && w.ID_TRASLADO == x.ID_TRASLADO).Count();
                        //SI EL TRASLADO SE EMPALMA TOMA DESICION BASADO EN JERARQUIAS...
                        var interno_con_traslado = imputado.Find(f => f.IdImputado == consulta_interno_traslado.ID_IMPUTADO && (f.CitaMedica == true || f.ExcarcelacionInterno == true) && (f.FechaTermino.Value.TimeOfDay > x.TRASLADO.TRASLADO_FEC.TimeOfDay && f.Fecha.Value.TimeOfDay <= x.TRASLADO.TRASLADO_FEC.TimeOfDay));
                        //SI EL INTERNO SE EMPALMA BASADO EN JERARQUIAS...
                        var interno_con_traslado_exc = imputado.Find(f => f.IdImputado == consulta_interno_traslado.ID_IMPUTADO && (f.ExcarcelacionInterno == true) && (f.FechaTermino.Value.TimeOfDay >= x.TRASLADO.TRASLADO_FEC.TimeOfDay &&
                            f.Fecha.Value.TimeOfDay <= x.TRASLADO.TRASLADO_FEC.TimeOfDay));
                        //FILTRO PARA ELIMINAR PROGRAMAS DESPUES DE UN TRASLADO EN HORAS...
                        var interno_con_traslado_horas = imputado.Where(w => w.IdImputado == consulta_interno_traslado.ID_IMPUTADO && (w.CitaMedica == true || w.ExcarcelacionInterno == true) &&
                            (w.Fecha.Value.TimeOfDay > x.TRASLADO.TRASLADO_FEC.TimeOfDay)).ToList();
                        //FILTRO PARA SABER SI EL INTERNO CON TRASLADO ESTA FUERA DEL EDIFICIO
                        var interno_traslado_fuera_edificio = interno_ubicacion.Where(w => w.IdCentro == x.ID_CENTRO && w.IdAnio == x.ID_ANIO && w.IdImputado == x.ID_IMPUTADO && w.IdIngreso == x.ID_INGRESO).FirstOrDefault();
                        //SI SE ENCUENTRA FUERA DE SU CELDA PERO ES REQUERIDO...
                        if (interno_traslado_fuera_edificio != null)
                        {
                            interno_requerido_traslado = true;
                        }
                        else
                        {
                            interno_requerido_traslado = false;
                        }
                        if (interno_con_traslado_horas.Count > 0)
                        {
                            foreach (var item in interno_con_traslado_horas)
                            {
                                imputado.Remove(item);
                            }
                        }
                        imputado = new List<InternosRequeridos>(imputado); //SE ACTUALIZA LISTA
                        //FILTRADO POR TODOS LOS EDIFICIOS
                        if (ID_UB_EDIFICIO == -1 && ID_UB_SECTOR == -1)
                        {
                            if (interno_con_traslado == null)
                            {
                                if (interno_con_traslado_exc == null)
                                {
                                    if (ingreso.Activo(consulta_interno_traslado))
                                    {
                                        if (v.TRASLADO_FEC.Day == Fechas.GetFechaDateServer.Day && v.TRASLADO_FEC.Month == Fechas.GetFechaDateServer.Month && v.TRASLADO_FEC.Year == Fechas.GetFechaDateServer.Year
                                            || (v.TRASLADO_FEC.Date >= Fechas.GetFechaDateServer.Date))
                                        {
                                            if (v.TRASLADO_FEC.AddHours(-ToleranciaTraslado).TimeOfDay <= Fechas.GetFechaDateServer.TimeOfDay && v.TRASLADO_FEC > Fechas.GetFechaDateServer)
                                            {
                                                if (lista_medicos.Any())
                                                {
                                                    if (traslado_interno > 0)
                                                    {
                                                        imputado.Add(new InternosRequeridos()
                                                        {
                                                            LlaveInterno = string.Format("{0}/{1}/{2}/{3}", x.ID_CENTRO, x.ID_ANIO, x.ID_IMPUTADO, consulta_interno_traslado.ID_INGRESO),
                                                            Centro = consulta_interno_traslado.ID_CENTRO,
                                                            Anio = consulta_interno_traslado.ID_ANIO,
                                                            IdImputado = consulta_interno_traslado.ID_IMPUTADO,
                                                            IdArea = 46,
                                                            IdIngreso = consulta_interno_traslado.ID_INGRESO,
                                                            Expediente = string.Format("{0}/{1}", x.ID_ANIO, x.ID_IMPUTADO),
                                                            Paterno = string.IsNullOrEmpty(consulta_imputado_traslado.PATERNO) ? string.Empty : consulta_imputado_traslado.PATERNO.TrimEnd(),
                                                            Materno = string.IsNullOrEmpty(consulta_imputado_traslado.MATERNO) ? string.Empty : consulta_imputado_traslado.MATERNO.TrimEnd(),
                                                            Nombre = string.IsNullOrEmpty(consulta_imputado_traslado.NOMBRE) ? string.Empty : consulta_imputado_traslado.NOMBRE.TrimEnd(),
                                                            Ubicacion = string.IsNullOrEmpty(ObtenerUbicacion()) ? string.Empty : ObtenerUbicacion().TrimEnd(),
                                                            Estancia = string.Format("{0}-{1}-{2}-{3}", string.IsNullOrEmpty(edificioInterno.DESCR) ? string.Empty : edificioInterno.DESCR.TrimEnd(), string.IsNullOrEmpty(sectorInterno.DESCR) ? string.Empty : sectorInterno.DESCR.TrimEnd(),
                                                            string.IsNullOrEmpty(celdaInterno.ID_CELDA) ? string.Empty : celdaInterno.ID_CELDA.TrimEnd(), consulta_interno_traslado.CAMA.ID_CAMA),
                                                            Actividad = "TRASLADO",
                                                            Area = "ÁREA MÉDICA PB",
                                                            Fecha = v.TRASLADO_FEC,
                                                            FechaTermino = v.TRASLADO_FEC,
                                                            Hora = new TimeSpan(v.TRASLADO_FEC.TimeOfDay.Ticks),
                                                            HoraFin = new TimeSpan(v.TRASLADO_FEC.TimeOfDay.Ticks),
                                                            TrasladoInterno = traslado_interno == temp_traslado,
                                                            CitaInterno = true,
                                                            Ingreso = consulta_interno_traslado,
                                                            InternoRequerido = interno_requerido_traslado == true ? true : false
                                                        });
                                                    }
                                                }
                                                else
                                                {
                                                    if (traslado_interno > 0)
                                                    {
                                                        imputado.Add(new InternosRequeridos()
                                                        {
                                                            LlaveInterno = string.Format("{0}/{1}/{2}/{3}", x.ID_CENTRO, x.ID_ANIO, x.ID_IMPUTADO, consulta_interno_traslado.ID_INGRESO),
                                                            Centro = consulta_interno_traslado.ID_CENTRO,
                                                            Anio = consulta_interno_traslado.ID_ANIO,
                                                            IdImputado = consulta_interno_traslado.ID_IMPUTADO,
                                                            IdArea = 111,
                                                            IdIngreso = consulta_interno_traslado.ID_INGRESO,
                                                            Expediente = string.Format("{0}/{1}", x.ID_ANIO, x.ID_IMPUTADO),
                                                            Paterno = string.IsNullOrEmpty(consulta_imputado_traslado.PATERNO) ? string.Empty : consulta_imputado_traslado.PATERNO.TrimEnd(),
                                                            Materno = string.IsNullOrEmpty(consulta_imputado_traslado.MATERNO) ? string.Empty : consulta_imputado_traslado.MATERNO.TrimEnd(),
                                                            Nombre = string.IsNullOrEmpty(consulta_imputado_traslado.NOMBRE) ? string.Empty : consulta_imputado_traslado.NOMBRE.TrimEnd(),
                                                            Ubicacion = string.IsNullOrEmpty(ObtenerUbicacion()) ? string.Empty : ObtenerUbicacion().TrimEnd(),
                                                            Estancia = string.Format("{0}-{1}-{2}-{3}", string.IsNullOrEmpty(edificioInterno.DESCR) ? string.Empty : edificioInterno.DESCR.TrimEnd(), string.IsNullOrEmpty(sectorInterno.DESCR) ? string.Empty : sectorInterno.DESCR.TrimEnd(),
                                                            string.IsNullOrEmpty(celdaInterno.ID_CELDA) ? string.Empty : celdaInterno.ID_CELDA.TrimEnd(), consulta_interno_traslado.CAMA.ID_CAMA),
                                                            Actividad = "TRASLADO",
                                                            Area = "SALIDA DEL CENTRO",
                                                            Fecha = v.TRASLADO_FEC,
                                                            FechaTermino = v.TRASLADO_FEC,
                                                            Hora = new TimeSpan(v.TRASLADO_FEC.TimeOfDay.Ticks),
                                                            HoraFin = new TimeSpan(v.TRASLADO_FEC.TimeOfDay.Ticks),
                                                            TrasladoInterno = traslado_interno == temp_traslado,
                                                            Ingreso = consulta_interno_traslado,
                                                            InternoRequerido = interno_requerido_traslado == true ? true : false
                                                        });
                                                    }
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        }
                        //FILTRADO POR EDIFICIO
                        else
                        {
                            if (interno_con_traslado == null)
                            {
                                if (interno_con_traslado_exc == null)
                                {
                                    if (ingreso.Activo(consulta_interno_traslado))
                                    {
                                        if (v.TRASLADO_FEC.Day == Fechas.GetFechaDateServer.Day && v.TRASLADO_FEC.Month == Fechas.GetFechaDateServer.Month && v.TRASLADO_FEC.Year == Fechas.GetFechaDateServer.Year
                                            || (v.TRASLADO_FEC.Date >= Fechas.GetFechaDateServer.Date))
                                        {
                                            if (v.TRASLADO_FEC.AddHours(-ToleranciaTraslado).TimeOfDay <= Fechas.GetFechaDateServer.TimeOfDay && v.TRASLADO_FEC > Fechas.GetFechaDateServer)
                                            {
                                                if (ingreso_traslado.Activo(consulta_interno_traslado) && EstaUbicacion(consulta_interno_traslado, ID_UB_EDIFICIO, ID_UB_SECTOR))
                                                {
                                                    if (lista_medicos.Any())
                                                    {
                                                        if (traslado_interno > 0)
                                                        {
                                                            imputado.Add(new InternosRequeridos()
                                                            {
                                                                LlaveInterno = string.Format("{0}/{1}/{2}/{3}", x.ID_CENTRO, x.ID_ANIO, x.ID_IMPUTADO, consulta_interno_traslado.ID_INGRESO),
                                                                Centro = consulta_interno_traslado.ID_CENTRO,
                                                                Anio = consulta_interno_traslado.ID_ANIO,
                                                                IdImputado = consulta_interno_traslado.ID_IMPUTADO,
                                                                IdArea = 46,
                                                                IdIngreso = consulta_interno_traslado.ID_INGRESO,
                                                                Expediente = string.Format("{0}/{1}", x.ID_ANIO, x.ID_IMPUTADO),
                                                                Paterno = string.IsNullOrEmpty(consulta_imputado_traslado.PATERNO) ? string.Empty : consulta_imputado_traslado.PATERNO.TrimEnd(),
                                                                Materno = string.IsNullOrEmpty(consulta_imputado_traslado.MATERNO) ? string.Empty : consulta_imputado_traslado.MATERNO.TrimEnd(),
                                                                Nombre = string.IsNullOrEmpty(consulta_imputado_traslado.NOMBRE) ? string.Empty : consulta_imputado_traslado.NOMBRE.TrimEnd(),
                                                                Ubicacion = string.IsNullOrEmpty(ObtenerUbicacion()) ? string.Empty : ObtenerUbicacion().TrimEnd(),
                                                                Estancia = string.Format("{0}-{1}-{2}-{3}", string.IsNullOrEmpty(edificioInterno.DESCR) ? string.Empty : edificioInterno.DESCR.TrimEnd(), string.IsNullOrEmpty(sectorInterno.DESCR) ? string.Empty : sectorInterno.DESCR.TrimEnd(),
                                                                string.IsNullOrEmpty(celdaInterno.ID_CELDA) ? string.Empty : celdaInterno.ID_CELDA.TrimEnd(), consulta_interno_traslado.CAMA.ID_CAMA),
                                                                Actividad = "TRASLADO",
                                                                Area = "ÁREA MÉDICA PB",
                                                                Fecha = v.TRASLADO_FEC,
                                                                FechaTermino = v.TRASLADO_FEC,
                                                                Hora = new TimeSpan(v.TRASLADO_FEC.TimeOfDay.Ticks),
                                                                HoraFin = new TimeSpan(v.TRASLADO_FEC.TimeOfDay.Ticks),
                                                                TrasladoInterno = traslado_interno == temp_traslado,
                                                                CitaInterno = true,
                                                                Ingreso = consulta_interno_traslado,
                                                                InternoRequerido = interno_requerido_traslado == true ? true : false
                                                            });
                                                        }
                                                    }
                                                    else
                                                    {
                                                        if (traslado_interno > 0)
                                                        {
                                                            imputado.Add(new InternosRequeridos()
                                                            {
                                                                LlaveInterno = string.Format("{0}/{1}/{2}/{3}", x.ID_CENTRO, x.ID_ANIO, x.ID_IMPUTADO, consulta_interno_traslado.ID_INGRESO),
                                                                Centro = consulta_interno_traslado.ID_CENTRO,
                                                                Anio = consulta_interno_traslado.ID_ANIO,
                                                                IdImputado = consulta_interno_traslado.ID_IMPUTADO,
                                                                IdArea = 111,
                                                                IdIngreso = consulta_interno_traslado.ID_INGRESO,
                                                                Expediente = string.Format("{0}/{1}", x.ID_ANIO, x.ID_IMPUTADO),
                                                                Paterno = string.IsNullOrEmpty(consulta_imputado_traslado.PATERNO) ? string.Empty : consulta_imputado_traslado.PATERNO.TrimEnd(),
                                                                Materno = string.IsNullOrEmpty(consulta_imputado_traslado.MATERNO) ? string.Empty : consulta_imputado_traslado.MATERNO.TrimEnd(),
                                                                Nombre = string.IsNullOrEmpty(consulta_imputado_traslado.NOMBRE) ? string.Empty : consulta_imputado_traslado.NOMBRE.TrimEnd(),
                                                                Ubicacion = string.IsNullOrEmpty(ObtenerUbicacion()) ? string.Empty : ObtenerUbicacion().TrimEnd(),
                                                                Estancia = string.Format("{0}-{1}-{2}-{3}", string.IsNullOrEmpty(edificioInterno.DESCR) ? string.Empty : edificioInterno.DESCR.TrimEnd(), string.IsNullOrEmpty(sectorInterno.DESCR) ? string.Empty : sectorInterno.DESCR.TrimEnd(),
                                                                string.IsNullOrEmpty(celdaInterno.ID_CELDA) ? string.Empty : celdaInterno.ID_CELDA.TrimEnd(), consulta_interno_traslado.CAMA.ID_CAMA),
                                                                Actividad = "TRASLADO",
                                                                Area = "SALIDA DEL CENTRO",
                                                                Fecha = v.TRASLADO_FEC,
                                                                FechaTermino = v.TRASLADO_FEC,
                                                                Hora = new TimeSpan(v.TRASLADO_FEC.TimeOfDay.Ticks),
                                                                HoraFin = new TimeSpan(v.TRASLADO_FEC.TimeOfDay.Ticks),
                                                                TrasladoInterno = traslado_interno == temp_traslado,
                                                                Ingreso = consulta_interno_traslado,
                                                                InternoRequerido = interno_requerido_traslado == true ? true : false
                                                            });
                                                        }
                                                    }
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                #endregion

                #region [INTERNOS CON VISITAS]
                var interno_requerido_visitas = false; //VARIABLE PARA DETERMINAR SI EL INTERNO ES REQUERIDO Y SE ENCUENTRA FUERA DE SU CELDA
                var aduana_ingreso = new cAduanaIngreso().ObtenerTodos(fechaInicio.Value, fechaFin.Value, ID_CENTRO);
                //var nombre_area = new cArea().Obtener(IdAreaVisitaLegal);
                if (aduana_ingreso != null)
                    foreach (var x in aduana_ingreso.Where(w => (!string.IsNullOrEmpty(w.INTERNO_NOTIFICADO) ? w.INTERNO_NOTIFICADO : "N") != "S"))
                    {
                        var tipo_visita = x.ADUANA.TIPO_PERSONA;
                        var consulta_imputado_visita = x.INGRESO.IMPUTADO;
                        var consulta_interno_visita = x.INGRESO;
                        var ingresovisita = new cIngreso();
                        var v = x.ADUANA;
                        var edificioInterno = consulta_interno_visita.CAMA.CELDA.SECTOR.EDIFICIO;
                        var sectorInterno = consulta_interno_visita.CAMA.CELDA.SECTOR;
                        var celdaInterno = consulta_interno_visita.CAMA.CELDA;
                        //FILTRO PARA SABER SI EL INTERNO CON VISITA ESTA FUERA DEL EDIFICIO
                        var visitas = interno_ubicacion.Where(w => w.IdCentro == x.ID_CENTRO && w.IdAnio == x.ID_ANIO && w.IdImputado == x.ID_IMPUTADO && w.IdIngreso == x.ID_INGRESO).FirstOrDefault();
                        //SI EL INTERNO SE ENCUENTRA FUERA DE SU CELDA PERO ES REQUERIDO...
                        if (visitas != null)
                        {
                            interno_requerido_visitas = true;
                        }
                        else
                        {
                            interno_requerido_visitas = false;
                        }
                        //FILTRADO POR TODOS LOS EDIFICIOS
                        if (ID_UB_EDIFICIO == -1 && ID_UB_SECTOR == -1)
                        {
                            if (ingreso.Activo(consulta_interno_visita))
                            {
                                if (v.ENTRADA_FEC.Value.Day == Fechas.GetFechaDateServer.Day && v.ENTRADA_FEC.Value.Month == Fechas.GetFechaDateServer.Month && v.ENTRADA_FEC.Value.Year ==
                                    Fechas.GetFechaDateServer.Year && (v.SALIDA_FEC == null))
                                {
                                    //SI HAY UNA CITA MEDICA QUE EMPALME...
                                    var visit_inter = imputado.Find(f => (f.IdImputado == consulta_interno_visita.ID_IMPUTADO) && (f.FechaTermino.Value.TimeOfDay > x.ENTRADA_FEC.Value.TimeOfDay && f.Fecha.Value.TimeOfDay <= x.ENTRADA_FEC.Value.TimeOfDay));
                                    //SI HAY UNA EXCARCELACION QUE EMPALME...
                                    var visit_inter_exc = imputado.Find(f => (f.IdImputado == consulta_interno_visita.ID_IMPUTADO) && f.ExcarcelacionInterno == true && (f.FechaTermino.Value.TimeOfDay >= x.ENTRADA_FEC.Value.TimeOfDay && f.Fecha.Value.TimeOfDay <= x.ENTRADA_FEC.Value.TimeOfDay));
                                    //SI HAY TRASLADO...
                                    var interno_con_traslado = imputado.Find(f => f.IdImputado == consulta_interno_visita.ID_IMPUTADO && f.TrasladoInterno == true && (f.Fecha.Value.TimeOfDay <= x.ENTRADA_FEC.Value.TimeOfDay));
                                    if (interno_con_traslado == null)
                                    {
                                        if (visit_inter == null)
                                        {
                                            if (visit_inter_exc == null)
                                            {
                                                if (tipo_visita.ID_TIPO_PERSONA == 2)
                                                {
                                                    imputado.Add(new InternosRequeridos()
                                                    {
                                                        LlaveInterno = string.Format("{0}/{1}/{2}/{3}", x.ID_CENTRO, x.ID_ANIO, x.ID_IMPUTADO, consulta_interno_visita.ID_INGRESO),
                                                        Centro = consulta_interno_visita.ID_CENTRO,
                                                        Anio = consulta_interno_visita.ID_ANIO,
                                                        IdImputado = consulta_interno_visita.ID_IMPUTADO,
                                                        IdArea = v.ID_AREA,
                                                        IdIngreso = consulta_interno_visita.ID_INGRESO,
                                                        IdAduana = x.ID_ADUANA,
                                                        Expediente = string.Format("{0}/{1}", x.ID_ANIO, x.ID_IMPUTADO),
                                                        Paterno = string.IsNullOrEmpty(consulta_imputado_visita.PATERNO) ? string.Empty : consulta_imputado_visita.PATERNO.TrimEnd(),
                                                        Materno = string.IsNullOrEmpty(consulta_imputado_visita.MATERNO) ? string.Empty : consulta_imputado_visita.MATERNO.TrimEnd(),
                                                        Nombre = string.IsNullOrEmpty(consulta_imputado_visita.NOMBRE) ? string.Empty : consulta_imputado_visita.NOMBRE.TrimEnd(),
                                                        Ubicacion = string.IsNullOrEmpty(ObtenerUbicacion()) ? string.Empty : ObtenerUbicacion().TrimEnd(),
                                                        Estancia = string.Format("{0}-{1}-{2}-{3}", string.IsNullOrEmpty(edificioInterno.DESCR) ? string.Empty : edificioInterno.DESCR.TrimEnd(), string.IsNullOrEmpty(sectorInterno.DESCR) ? string.Empty : sectorInterno.DESCR.TrimEnd(),
                                                        string.IsNullOrEmpty(celdaInterno.ID_CELDA) ? string.Empty : celdaInterno.ID_CELDA.TrimEnd(), consulta_interno_visita.CAMA.ID_CAMA),
                                                        Actividad = "VISITA LEGAL",
                                                        Area = v.AREA == null ? string.Empty : v.AREA.DESCR,
                                                        Fecha = v.ENTRADA_FEC,
                                                        FechaTermino = v.ENTRADA_FEC,
                                                        Hora = new TimeSpan(v.ENTRADA_FEC.Value.Ticks),
                                                        HoraFin = new TimeSpan(v.ENTRADA_FEC.Value.Ticks),
                                                        BooleanToRowVisitaLegal = true,
                                                        Ingreso = consulta_interno_visita,
                                                        VisitaLegal = true,
                                                        InternoRequerido = interno_requerido_visitas == true ? true : false
                                                    });
                                                }
                                                else if (tipo_visita.ID_TIPO_PERSONA == 3)
                                                {
                                                    if (x.INTIMA == "S")
                                                    {
                                                        imputado.Add(new InternosRequeridos()
                                                        {
                                                            LlaveInterno = string.Format("{0}/{1}/{2}/{3}", x.ID_CENTRO, x.ID_ANIO, x.ID_IMPUTADO, consulta_interno_visita.ID_INGRESO),
                                                            Centro = consulta_interno_visita.ID_CENTRO,
                                                            Anio = consulta_interno_visita.ID_ANIO,
                                                            IdImputado = consulta_interno_visita.ID_IMPUTADO,
                                                            IdArea = v.ID_AREA,
                                                            IdIngreso = consulta_interno_visita.ID_INGRESO,
                                                            IdAduana = x.ID_ADUANA,
                                                            Expediente = string.Format("{0}/{1}", x.ID_ANIO, x.ID_IMPUTADO),
                                                            Paterno = string.IsNullOrEmpty(consulta_imputado_visita.PATERNO) ? string.Empty : consulta_imputado_visita.PATERNO.TrimEnd(),
                                                            Materno = string.IsNullOrEmpty(consulta_imputado_visita.MATERNO) ? string.Empty : consulta_imputado_visita.MATERNO.TrimEnd(),
                                                            Nombre = string.IsNullOrEmpty(consulta_imputado_visita.NOMBRE) ? string.Empty : consulta_imputado_visita.NOMBRE.TrimEnd(),
                                                            Ubicacion = string.IsNullOrEmpty(ObtenerUbicacion()) ? string.Empty : ObtenerUbicacion().TrimEnd(),
                                                            Estancia = string.Format("{0}-{1}-{2}-{3}", string.IsNullOrEmpty(edificioInterno.DESCR) ? string.Empty : edificioInterno.DESCR.TrimEnd(), string.IsNullOrEmpty(sectorInterno.DESCR) ? string.Empty : sectorInterno.DESCR.TrimEnd(),
                                                            string.IsNullOrEmpty(celdaInterno.ID_CELDA) ? string.Empty : celdaInterno.ID_CELDA.TrimEnd(), consulta_interno_visita.CAMA.ID_CAMA),
                                                            Actividad = "VISITA INTIMA",
                                                            Area = v.AREA == null ? string.Empty : v.AREA.DESCR,
                                                            Fecha = v.ENTRADA_FEC,
                                                            FechaTermino = v.ENTRADA_FEC,
                                                            Hora = new TimeSpan(v.ENTRADA_FEC.Value.Ticks),
                                                            HoraFin = new TimeSpan(v.ENTRADA_FEC.Value.Ticks),
                                                            BooleanToRowVisitaIntima = true,
                                                            Ingreso = consulta_interno_visita,
                                                            VisitaIntima = true,
                                                            InternoRequerido = interno_requerido_visitas == true ? true : false
                                                        });
                                                    }
                                                    else
                                                    {
                                                        imputado.Add(new InternosRequeridos()
                                                        {
                                                            LlaveInterno = string.Format("{0}/{1}/{2}/{3}", x.ID_CENTRO, x.ID_ANIO, x.ID_IMPUTADO, consulta_interno_visita.ID_INGRESO),
                                                            Centro = consulta_interno_visita.ID_CENTRO,
                                                            Anio = consulta_interno_visita.ID_ANIO,
                                                            IdImputado = consulta_interno_visita.ID_IMPUTADO,
                                                            IdArea = v.ID_AREA,
                                                            IdIngreso = consulta_interno_visita.ID_INGRESO,
                                                            IdAduana = x.ID_ADUANA,
                                                            Expediente = string.Format("{0}/{1}", x.ID_ANIO, x.ID_IMPUTADO),
                                                            Paterno = string.IsNullOrEmpty(consulta_imputado_visita.PATERNO) ? string.Empty : consulta_imputado_visita.PATERNO.TrimEnd(),
                                                            Materno = string.IsNullOrEmpty(consulta_imputado_visita.MATERNO) ? string.Empty : consulta_imputado_visita.MATERNO.TrimEnd(),
                                                            Nombre = string.IsNullOrEmpty(consulta_imputado_visita.NOMBRE) ? string.Empty : consulta_imputado_visita.NOMBRE.TrimEnd(),
                                                            Ubicacion = string.IsNullOrEmpty(ObtenerUbicacion()) ? string.Empty : ObtenerUbicacion().TrimEnd(),
                                                            Estancia = string.Format("{0}-{1}-{2}-{3}", string.IsNullOrEmpty(edificioInterno.DESCR) ? string.Empty : edificioInterno.DESCR.TrimEnd(), string.IsNullOrEmpty(sectorInterno.DESCR) ? string.Empty : sectorInterno.DESCR.TrimEnd(),
                                                            string.IsNullOrEmpty(celdaInterno.ID_CELDA) ? string.Empty : celdaInterno.ID_CELDA.TrimEnd(), consulta_interno_visita.CAMA.ID_CAMA),
                                                            Actividad = "VISITA FAMILIAR",
                                                            Area = v.AREA == null ? string.Empty : v.AREA.DESCR,
                                                            Fecha = v.ENTRADA_FEC,
                                                            FechaTermino = v.ENTRADA_FEC,
                                                            Hora = new TimeSpan(v.ENTRADA_FEC.Value.Ticks),
                                                            HoraFin = new TimeSpan(v.ENTRADA_FEC.Value.Ticks),
                                                            BooleanToRowVisitaFamiliar = true,
                                                            Ingreso = consulta_interno_visita,
                                                            VisitaFamiliar = true,
                                                            InternoRequerido = interno_requerido_visitas == true ? true : false
                                                        });
                                                    }
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        }
                        //FILTRADO POR EDIFICIO
                        else
                        {
                            if (ingreso.Activo(consulta_interno_visita))
                            {
                                if ((v.ENTRADA_FEC.Value.Day == Fechas.GetFechaDateServer.Day && v.ENTRADA_FEC.Value.Month == Fechas.GetFechaDateServer.Month && v.ENTRADA_FEC.Value.Year == Fechas.GetFechaDateServer.Year && (v.SALIDA_FEC == null)))
                                {
                                    if (ingresovisita.Activo(consulta_interno_visita) && EstaUbicacion(consulta_interno_visita, ID_UB_EDIFICIO, ID_UB_SECTOR))
                                    {
                                        var visit_inter = imputado.Find(f => (f.IdImputado == consulta_interno_visita.ID_IMPUTADO) && (f.FechaTermino.Value.TimeOfDay > x.ENTRADA_FEC.Value.TimeOfDay && f.Fecha.Value.TimeOfDay <= x.ENTRADA_FEC.Value.TimeOfDay));
                                        //SI HAY UNA EXCARCELACION QUE EMPALME...
                                        var visit_inter_exc = imputado.Find(f => (f.IdImputado == consulta_interno_visita.ID_IMPUTADO) && f.ExcarcelacionInterno == true && (f.FechaTermino.Value.TimeOfDay >= x.ENTRADA_FEC.Value.TimeOfDay && f.Fecha.Value.TimeOfDay <= x.ENTRADA_FEC.Value.TimeOfDay));
                                        //SI HAY TRASLADO...
                                        var interno_con_traslado = imputado.Find(f => f.IdImputado == consulta_interno_visita.ID_IMPUTADO && f.TrasladoInterno == true && (f.Fecha.Value.TimeOfDay <= x.ENTRADA_FEC.Value.TimeOfDay));

                                        if (interno_con_traslado == null)
                                        {
                                            if (visit_inter == null)
                                            {
                                                if (visit_inter_exc == null)
                                                {
                                                    if (tipo_visita.ID_TIPO_PERSONA == 2)
                                                    {
                                                        imputado.Add(new InternosRequeridos()
                                                        {
                                                            LlaveInterno = string.Format("{0}/{1}/{2}/{3}", x.ID_CENTRO, x.ID_ANIO, x.ID_IMPUTADO, consulta_interno_visita.ID_INGRESO),
                                                            Centro = consulta_interno_visita.ID_CENTRO,
                                                            Anio = consulta_interno_visita.ID_ANIO,
                                                            IdImputado = consulta_interno_visita.ID_IMPUTADO,
                                                            IdArea = v.ID_AREA,
                                                            IdIngreso = consulta_interno_visita.ID_INGRESO,
                                                            IdAduana = x.ID_ADUANA,
                                                            Expediente = string.Format("{0}/{1}", x.ID_ANIO, x.ID_IMPUTADO),
                                                            Paterno = string.IsNullOrEmpty(consulta_imputado_visita.PATERNO) ? string.Empty : consulta_imputado_visita.PATERNO.TrimEnd(),
                                                            Materno = string.IsNullOrEmpty(consulta_imputado_visita.MATERNO) ? string.Empty : consulta_imputado_visita.MATERNO.TrimEnd(),
                                                            Nombre = string.IsNullOrEmpty(consulta_imputado_visita.NOMBRE) ? string.Empty : consulta_imputado_visita.NOMBRE.TrimEnd(),
                                                            Ubicacion = string.IsNullOrEmpty(ObtenerUbicacion()) ? string.Empty : ObtenerUbicacion().TrimEnd(),
                                                            Estancia = string.Format("{0}-{1}-{2}-{3}", string.IsNullOrEmpty(edificioInterno.DESCR) ? string.Empty : edificioInterno.DESCR.TrimEnd(), string.IsNullOrEmpty(sectorInterno.DESCR) ? string.Empty : sectorInterno.DESCR.TrimEnd(),
                                                            string.IsNullOrEmpty(celdaInterno.ID_CELDA) ? string.Empty : celdaInterno.ID_CELDA.TrimEnd(), consulta_interno_visita.CAMA.ID_CAMA),
                                                            Actividad = "VISITA LEGAL",
                                                            Area = v.AREA == null ? string.Empty : v.AREA.DESCR,
                                                            Fecha = v.ENTRADA_FEC,
                                                            FechaTermino = v.ENTRADA_FEC,
                                                            Hora = new TimeSpan(v.ENTRADA_FEC.Value.Ticks),
                                                            HoraFin = new TimeSpan(v.ENTRADA_FEC.Value.Ticks),
                                                            BooleanToRowVisitaLegal = true,
                                                            Ingreso = consulta_interno_visita,
                                                            VisitaLegal = true,
                                                            InternoRequerido = interno_requerido_visitas == true ? true : false
                                                        });
                                                    }
                                                    else if (tipo_visita.ID_TIPO_PERSONA == 3)
                                                    {
                                                        if (x.INTIMA == "S")
                                                        {
                                                            imputado.Add(new InternosRequeridos()
                                                            {
                                                                LlaveInterno = string.Format("{0}/{1}/{2}/{3}", x.ID_CENTRO, x.ID_ANIO, x.ID_IMPUTADO, consulta_interno_visita.ID_INGRESO),
                                                                Centro = consulta_interno_visita.ID_CENTRO,
                                                                Anio = consulta_interno_visita.ID_ANIO,
                                                                IdImputado = consulta_interno_visita.ID_IMPUTADO,
                                                                IdArea = v.ID_AREA,
                                                                IdIngreso = consulta_interno_visita.ID_INGRESO,
                                                                IdAduana = x.ID_ADUANA,
                                                                Expediente = string.Format("{0}/{1}", x.ID_ANIO, x.ID_IMPUTADO),
                                                                Paterno = string.IsNullOrEmpty(consulta_imputado_visita.PATERNO) ? string.Empty : consulta_imputado_visita.PATERNO.TrimEnd(),
                                                                Materno = string.IsNullOrEmpty(consulta_imputado_visita.MATERNO) ? string.Empty : consulta_imputado_visita.MATERNO.TrimEnd(),
                                                                Nombre = string.IsNullOrEmpty(consulta_imputado_visita.NOMBRE) ? string.Empty : consulta_imputado_visita.NOMBRE.TrimEnd(),
                                                                Ubicacion = string.IsNullOrEmpty(ObtenerUbicacion()) ? string.Empty : ObtenerUbicacion().TrimEnd(),
                                                                Estancia = string.Format("{0}-{1}-{2}-{3}", string.IsNullOrEmpty(edificioInterno.DESCR) ? string.Empty : edificioInterno.DESCR.TrimEnd(), string.IsNullOrEmpty(sectorInterno.DESCR) ? string.Empty : sectorInterno.DESCR.TrimEnd(),
                                                                string.IsNullOrEmpty(celdaInterno.ID_CELDA) ? string.Empty : celdaInterno.ID_CELDA.TrimEnd(), consulta_interno_visita.CAMA.ID_CAMA),
                                                                Actividad = "VISITA INTIMA",
                                                                Area = v.AREA == null ? string.Empty : v.AREA.DESCR,
                                                                Fecha = v.ENTRADA_FEC,
                                                                FechaTermino = v.ENTRADA_FEC,
                                                                Hora = new TimeSpan(v.ENTRADA_FEC.Value.Ticks),
                                                                HoraFin = new TimeSpan(v.ENTRADA_FEC.Value.Ticks),
                                                                BooleanToRowVisitaIntima = true,
                                                                Ingreso = consulta_interno_visita,
                                                                VisitaIntima = true,
                                                                InternoRequerido = interno_requerido_visitas == true ? true : false
                                                            });
                                                        }
                                                        else
                                                        {
                                                            imputado.Add(new InternosRequeridos()
                                                            {
                                                                LlaveInterno = string.Format("{0}/{1}/{2}/{3}", x.ID_CENTRO, x.ID_ANIO, x.ID_IMPUTADO, consulta_interno_visita.ID_INGRESO),
                                                                Centro = consulta_interno_visita.ID_CENTRO,
                                                                Anio = consulta_interno_visita.ID_ANIO,
                                                                IdImputado = consulta_interno_visita.ID_IMPUTADO,
                                                                IdArea = v.ID_AREA,
                                                                IdIngreso = consulta_interno_visita.ID_INGRESO,
                                                                IdAduana = x.ID_ADUANA,
                                                                Expediente = string.Format("{0}/{1}", x.ID_ANIO, x.ID_IMPUTADO),
                                                                Paterno = string.IsNullOrEmpty(consulta_imputado_visita.PATERNO) ? string.Empty : consulta_imputado_visita.PATERNO.TrimEnd(),
                                                                Materno = string.IsNullOrEmpty(consulta_imputado_visita.MATERNO) ? string.Empty : consulta_imputado_visita.MATERNO.TrimEnd(),
                                                                Nombre = string.IsNullOrEmpty(consulta_imputado_visita.NOMBRE) ? string.Empty : consulta_imputado_visita.NOMBRE.TrimEnd(),
                                                                Ubicacion = string.IsNullOrEmpty(ObtenerUbicacion()) ? string.Empty : ObtenerUbicacion().TrimEnd(),
                                                                Estancia = string.Format("{0}-{1}-{2}-{3}", string.IsNullOrEmpty(edificioInterno.DESCR) ? string.Empty : edificioInterno.DESCR.TrimEnd(), string.IsNullOrEmpty(sectorInterno.DESCR) ? string.Empty : sectorInterno.DESCR.TrimEnd(),
                                                                string.IsNullOrEmpty(celdaInterno.ID_CELDA) ? string.Empty : celdaInterno.ID_CELDA.TrimEnd(), consulta_interno_visita.CAMA.ID_CAMA),
                                                                Actividad = "VISITA FAMILIAR",
                                                                Area = v.AREA == null ? string.Empty : v.AREA.DESCR,
                                                                Fecha = v.ENTRADA_FEC,
                                                                FechaTermino = v.ENTRADA_FEC,
                                                                Hora = new TimeSpan(v.ENTRADA_FEC.Value.Ticks),
                                                                HoraFin = new TimeSpan(v.ENTRADA_FEC.Value.Ticks),
                                                                BooleanToRowVisitaFamiliar = true,
                                                                Ingreso = consulta_interno_visita,
                                                                VisitaFamiliar = true,
                                                                InternoRequerido = interno_requerido_visitas == true ? true : false
                                                            });
                                                        }
                                                    }
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                #endregion

                #region [INTERNOS CITAS TECNICAS]
                var interno_requerido_cita_tecnica = false; //VARIABLE PARA DETERMINAR SI EL INTERNO ES REQUERIDO Y SE ENCUENTRA FUERA DE SU CELDA
                if (result_interno_cita_medica != null)
                    foreach (var row in result_interno_cita_medica.Where(w => w.AtencionCita.ESTATUS == "N" && (w.AtencionCita.ID_AREA != 45 && w.AtencionCita.ID_AREA != 46)))
                    {
                        var consulta_interno_cita = row.Ingreso;
                        //SI EMPALMA CON CITAS MEDICAS...
                        var cita_inter = imputado.Find(f => (f.LlaveInterno == row.LlaveInterno) && f.CitaMedica == true && (f.FechaTermino.Value.TimeOfDay > row.AtencionCita.CITA_FECHA_HORA.Value.TimeOfDay && f.Fecha.Value.TimeOfDay < row.AtencionCita.CITA_HORA_TERMINA.Value.TimeOfDay));
                        //SI HAY UNA EXCARCELACION QUE EMPALME...
                        var cita_inter_exc = imputado.Find(f => (f.IdImputado == consulta_interno_cita.ID_IMPUTADO) && (f.ExcarcelacionInterno == true || f.VisitaLegal == true) && (f.FechaTermino.Value.TimeOfDay >= row.AtencionCita.CITA_FECHA_HORA.Value.TimeOfDay && f.Fecha.Value.TimeOfDay < row.AtencionCita.CITA_HORA_TERMINA.Value.TimeOfDay));
                        //SI TIENE TRASLADO...
                        var interno_con_traslado = imputado.Find(f => f.IdImputado == consulta_interno_cita.ID_IMPUTADO && f.TrasladoInterno == true && ((f.Fecha.Value.TimeOfDay >= row.AtencionCita.CITA_FECHA_HORA.Value.TimeOfDay &&
                            f.FechaTermino < row.AtencionCita.CITA_HORA_TERMINA) || f.FechaTermino.Value.TimeOfDay < row.AtencionCita.CITA_HORA_TERMINA.Value.TimeOfDay));
                        //SI HAY EMPALME CON VISITA INTIMA O FAMILIAR...
                        var cita_inter_vist = imputado.Where(f => (f.IdImputado == consulta_interno_cita.ID_IMPUTADO) && (f.VisitaIntima == true || f.VisitaFamiliar == true) && (f.FechaTermino.Value.TimeOfDay >= row.AtencionCita.CITA_FECHA_HORA.Value.TimeOfDay && f.Fecha.Value.TimeOfDay < row.AtencionCita.CITA_HORA_TERMINA.Value.TimeOfDay)).ToList();
                        //FILTRO PARA SABER SI EL INTERNO CON CITA SE ENCUENTRA FUERA DEL EDIFICIO
                        var citas_medicas = interno_ubicacion.Where(w => w.Expediente == row.Expediente).FirstOrDefault();
                        //SI EL INTERNO SE ENCUENTRA FUERA DE SU CELDA PERO ES REQUERIDO...
                        if (citas_medicas != null)
                        {
                            interno_requerido_cita_tecnica = true;
                        }
                        else
                        {
                            interno_requerido_cita_tecnica = false;
                        }
                        if (cita_inter_vist.Count > 0)
                        {
                            foreach (var item in cita_inter_vist)
                            {
                                imputado.Remove(item);
                            }
                        }
                        imputado = new List<InternosRequeridos>(imputado); //SE ACTUALIZA LISTA
                        //FILTRADO POR TODOS LOS EDIFICIOS Y SECTORES
                        if (ID_UB_EDIFICIO == -1 && ID_UB_SECTOR == -1)
                        {
                            if (interno_con_traslado == null)
                            {
                                if (cita_inter == null)
                                {
                                    if (cita_inter_exc == null)
                                    {
                                        if ((row.AtencionCita.CITA_FECHA_HORA.Value.Day == Fechas.GetFechaDateServer.Day && row.AtencionCita.CITA_FECHA_HORA.Value.Month == Fechas.GetFechaDateServer.Month && row.AtencionCita.CITA_FECHA_HORA.Value.Year == Fechas.GetFechaDateServer.Year &&
                                           (row.AtencionCita.CITA_FECHA_HORA.Value.TimeOfDay <= Fechas.GetFechaDateServer.TimeOfDay && row.AtencionCita.CITA_HORA_TERMINA.Value.TimeOfDay > Fechas.GetFechaDateServer.TimeOfDay)) || row.AtencionCita.CITA_FECHA_HORA.Value >= Fechas.GetFechaDateServer)
                                        {
                                            if (ingreso.Activo(consulta_interno_cita))
                                            {
                                                row.Citas = true;
                                                row.InternoRequerido = interno_requerido_cita_tecnica == true ? true : false;
                                            }
                                            imputado.Add(row);
                                        }
                                    }
                                }
                                else
                                {
                                    if ((row.AtencionCita.CITA_FECHA_HORA.Value.Day == Fechas.GetFechaDateServer.Day && row.AtencionCita.CITA_FECHA_HORA.Value.Month == Fechas.GetFechaDateServer.Month && row.AtencionCita.CITA_FECHA_HORA.Value.Year == Fechas.GetFechaDateServer.Year &&
                                       (row.AtencionCita.CITA_FECHA_HORA.Value.TimeOfDay <= Fechas.GetFechaDateServer.TimeOfDay && row.AtencionCita.CITA_HORA_TERMINA.Value.TimeOfDay > Fechas.GetFechaDateServer.TimeOfDay)))
                                    {
                                        if (!cita_inter.ExcarcelacionInterno && !cita_inter.VisitaFamiliar && !cita_inter.VisitaIntima && !cita_inter.VisitaLegal)
                                        {
                                            if (ingreso.Activo(consulta_interno_cita))
                                            {
                                                row.Citas = true;
                                                InternoRequerido = interno_requerido_cita_tecnica == true ? true : false;
                                            }
                                            imputado.Add(row);
                                        }
                                    }
                                }
                            }
                        }
                        //FILTRADO POR EDIFICIO-SECTOR
                        else
                        {
                            if (interno_con_traslado == null)
                            {
                                if (cita_inter == null)
                                {
                                    if (cita_inter_exc == null)
                                    {
                                        if ((row.AtencionCita.CITA_FECHA_HORA.Value.Day == Fechas.GetFechaDateServer.Day && row.AtencionCita.CITA_FECHA_HORA.Value.Month == Fechas.GetFechaDateServer.Month && row.AtencionCita.CITA_FECHA_HORA.Value.Year == Fechas.GetFechaDateServer.Year &&
                                           (row.AtencionCita.CITA_FECHA_HORA.Value.TimeOfDay <= Fechas.GetFechaDateServer.TimeOfDay && row.AtencionCita.CITA_HORA_TERMINA.Value.TimeOfDay > Fechas.GetFechaDateServer.TimeOfDay)) || row.AtencionCita.CITA_FECHA_HORA.Value >= Fechas.GetFechaDateServer)
                                        {
                                            if (ingreso.Activo(consulta_interno_cita) && EstaUbicacion(consulta_interno_cita, ID_UB_EDIFICIO, ID_UB_SECTOR))
                                            {
                                                row.Citas = true;
                                                InternoRequerido = interno_requerido_cita_tecnica == true ? true : false;
                                            }
                                            imputado.Add(row);
                                        }
                                    }
                                }
                            }
                            else
                            {
                                if ((row.AtencionCita.CITA_FECHA_HORA.Value.Day == Fechas.GetFechaDateServer.Day && row.AtencionCita.CITA_FECHA_HORA.Value.Month == Fechas.GetFechaDateServer.Month && row.AtencionCita.CITA_FECHA_HORA.Value.Year == Fechas.GetFechaDateServer.Year && (row.AtencionCita.CITA_FECHA_HORA.Value.TimeOfDay <= Fechas.GetFechaDateServer.TimeOfDay && row.AtencionCita.CITA_HORA_TERMINA.Value.TimeOfDay > Fechas.GetFechaDateServer.TimeOfDay)) || row.AtencionCita.CITA_FECHA_HORA.Value >= Fechas.GetFechaDateServer)
                                {
                                    if (!cita_inter.ExcarcelacionInterno)// && !cita_inter.VisitaFamiliar && !cita_inter.VisitaIntima && !cita_inter.VisitaLegal)
                                    {
                                        if (ingreso.Activo(consulta_interno_cita) && EstaUbicacion(consulta_interno_cita, ID_UB_EDIFICIO, ID_UB_SECTOR))
                                        {
                                            row.Citas = true;
                                            InternoRequerido = interno_requerido_cita_tecnica == true ? true : false;
                                        }
                                        imputado.Add(row);
                                    }
                                }
                            }
                        }
                    }
                #endregion

                #region [INTERNOS CON ESTUDIOS DE PERSONALIDAD PROGRAMADOS]
                var interno_requerido_estudios = false;
                var personalidad = InternosEstudiosPersonalidadProgramado(fechaInicio, fechaFin, ID_CENTRO);
                if (personalidad != null)
                    foreach (var row in personalidad)
                    {
                        var consulta_interno_cita = row.Ingreso;
                        //SI EMPALMA CON CITAS MEDICAS...
                        var cita_inter = imputado.Find(f => (f.LlaveInterno == row.LlaveInterno) && f.CitaMedica == true && (f.FechaTermino.Value.TimeOfDay > row.Fecha.Value.TimeOfDay && f.Fecha.Value.TimeOfDay < row.FechaTermino.Value.TimeOfDay));
                        //SI HAY UNA EXCARCELACION QUE EMPALME...
                        var cita_inter_exc = imputado.Find(f => (f.IdImputado == consulta_interno_cita.ID_IMPUTADO) && (f.ExcarcelacionInterno == true) && (f.FechaTermino.Value.TimeOfDay >= row.Fecha.Value.TimeOfDay && f.Fecha.Value.TimeOfDay < row.FechaTermino.Value.TimeOfDay));
                        //SI TIENE TRASLADO...
                        var interno_con_traslado = imputado.Find(f => f.IdImputado == consulta_interno_cita.ID_IMPUTADO && f.TrasladoInterno == true && ((f.Fecha.Value.TimeOfDay >= row.Fecha.Value.TimeOfDay &&
                            f.FechaTermino < row.FechaTermino) || f.FechaTermino.Value.TimeOfDay < row.FechaTermino.Value.TimeOfDay));
                        //SI HAY EMPALME CON VISITA INTIMA O FAMILIAR...
                        var cita_inter_vist = imputado.Where(f => (f.IdImputado == consulta_interno_cita.ID_IMPUTADO) && (f.VisitaIntima == true || f.VisitaFamiliar == true || f.VisitaLegal) && (f.FechaTermino.Value.TimeOfDay >= row.Fecha.Value.TimeOfDay && f.Fecha.Value.TimeOfDay < row.FechaTermino.Value.TimeOfDay)).ToList();
                        //SI HAY EMPALME CON CITAS TECNICAS
                        var cita_inter_tecnica = imputado.Where(f => (f.IdImputado == consulta_interno_cita.ID_IMPUTADO) && (f.Citas == true) && (f.FechaTermino.Value.TimeOfDay > row.Fecha.Value.TimeOfDay && f.Fecha.Value.TimeOfDay < row.FechaTermino.Value.TimeOfDay)).ToList();
                        //FILTRO PARA SABER SI EL INTERNO CON CITA SE ENCUENTRA FUERA DEL EDIFICIO
                        var citas_medicas = interno_ubicacion.Where(w => w.Expediente == row.Expediente).FirstOrDefault();
                        //SI EL INTERNO SE ENCUENTRA FUERA DE SU CELDA PERO ES REQUERIDO...
                        if (citas_medicas != null)
                        {
                            interno_requerido_estudios = true;
                        }
                        else
                        {
                            interno_requerido_estudios = false;
                        }
                        if (cita_inter == null)
                        {
                            if (cita_inter_exc == null)
                            {
                                if (interno_con_traslado == null)
                                {
                                    if (cita_inter_vist == null || cita_inter_vist.Count == 0)
                                    {
                                        if (cita_inter_tecnica == null || cita_inter_tecnica.Count == 0)
                                        {
                                            if (ingreso.Activo(row.Ingreso))//el ingreso esta activo
                                            {
                                                imputado.Add(new InternosRequeridos()
                                                {
                                                    LlaveInterno = string.Format("{0}/{1}/{2}/{3}", row.Centro, row.Anio, row.IdImputado, row.IdIngreso),
                                                    Centro = row.Centro,
                                                    Anio = row.Anio,
                                                    IdImputado = row.IdImputado,
                                                    IdArea = row.IdArea,
                                                    IdIngreso = row.IdIngreso,
                                                    Expediente = string.Format("{0}/{1}", row.Anio, row.IdImputado),
                                                    Paterno = row.Ingreso != null ? row.Ingreso.IMPUTADO != null ? !string.IsNullOrEmpty(row.Ingreso.IMPUTADO.PATERNO) ? row.Ingreso.IMPUTADO.PATERNO.Trim() : string.Empty : string.Empty : string.Empty,
                                                    Materno = row.Ingreso != null ? row.Ingreso.IMPUTADO != null ? !string.IsNullOrEmpty(row.Ingreso.IMPUTADO.MATERNO) ? row.Ingreso.IMPUTADO.MATERNO.Trim() : string.Empty : string.Empty : string.Empty,
                                                    Nombre = row.Ingreso != null ? row.Ingreso.IMPUTADO != null ? !string.IsNullOrEmpty(row.Ingreso.IMPUTADO.NOMBRE) ? row.Ingreso.IMPUTADO.NOMBRE.Trim() : string.Empty : string.Empty : string.Empty,
                                                    Ubicacion = ObtenerUbicacion().TrimEnd(),
                                                    Estancia = string.Format("{0}-{1}-{2}-{3}", row.Ingreso != null ? row.Ingreso.CAMA != null ? row.Ingreso.CAMA.CELDA != null ? row.Ingreso.CAMA.CELDA.SECTOR != null ? row.Ingreso.CAMA.CELDA.SECTOR.EDIFICIO != null ? !string.IsNullOrEmpty(row.Ingreso.CAMA.CELDA.SECTOR.EDIFICIO.DESCR) ? row.Ingreso.CAMA.CELDA.SECTOR.EDIFICIO.DESCR.Trim() : string.Empty : string.Empty :
                                                    string.Empty : string.Empty : string.Empty : string.Empty,
                                                    row.Ingreso != null ? row.Ingreso.CAMA != null ? row.Ingreso.CAMA.CELDA != null ? row.Ingreso.CAMA.CELDA.SECTOR != null ? !string.IsNullOrEmpty(row.Ingreso.CAMA.CELDA.SECTOR.DESCR) ? row.Ingreso.CAMA.CELDA.SECTOR.DESCR.Trim() : string.Empty : string.Empty : string.Empty : string.Empty : string.Empty,
                                                    row.Ingreso != null ? row.Ingreso.CAMA != null ? row.Ingreso.CAMA.CELDA != null ? !string.IsNullOrEmpty(row.Ingreso.CAMA.CELDA.ID_CELDA) ? row.Ingreso.CAMA.CELDA.ID_CELDA.Trim() : string.Empty : string.Empty : string.Empty : string.Empty,
                                                    row.Ingreso != null ? row.Ingreso.CAMA != null ? row.Ingreso.CAMA.ID_CAMA : 0 : 0),
                                                    Actividad = "ESTUDIO DE PERSONALIDAD",
                                                    Area = !string.IsNullOrEmpty(row.Area) ? row.Area.Trim() : string.Empty,
                                                    Fecha = row.Fecha,
                                                    FechaTermino = row.FechaTermino,
                                                    Hora = new TimeSpan(row.Fecha.Value.TimeOfDay.Ticks),
                                                    HoraFin = new TimeSpan(row.FechaTermino.Value.TimeOfDay.Ticks),
                                                    EstudioInterno = true,
                                                    Ingreso = row.Ingreso,
                                                    InternoRequerido = interno_requerido_estudios == true ? true : false
                                                });
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                #endregion

                #region [INTERNOS CON ACTIVIDADES]
                var interno_requerido_actividades = false; //VARIABLE PARA DETERMINAR SI EL INTERNO ES REQUERIDO Y SE ENCUENTRA FUERA DE SU CELDA
                //NECESARIO PARA INTERNOS EN GRUPO-ACTIVIDAD
                var grupo_horario = new cGrupoHorario().ObtenerActivos(fechaInicio.Value, fechaFin.Value, ID_CENTRO);
                var lstEmpalmados = new List<InternosRequeridos>();
                if (grupo_horario != null)
                    foreach (var item in grupo_horario)
                    {
                        // TEMPORAL
                        foreach (var asistencia in item.GRUPO_ASISTENCIA.Where(w => w.GRUPO_PARTICIPANTE.INGRESO.ID_UB_CENTRO == ID_CENTRO))
                        {
                            //el interno participa
                            if (asistencia.ESTATUS == 1)
                            {
                                if (asistencia.ASISTENCIA != 1)
                                {
                                    var consulta_interno = asistencia.GRUPO_PARTICIPANTE.INGRESO;
                                    var edificio = consulta_interno.CAMA.CELDA.SECTOR.EDIFICIO;
                                    var sector = consulta_interno.CAMA.CELDA.SECTOR;
                                    var celda = consulta_interno.CAMA.CELDA;
                                    var consulta_imputado = asistencia.GRUPO_PARTICIPANTE.INGRESO.IMPUTADO;

                                    if (filtrarBusqueda)
                                    {
                                        if (!(consulta_interno.IMPUTADO.NOMBRE.Contains(busqueda) || consulta_interno.IMPUTADO.PATERNO.Contains(busqueda) || consulta_interno.IMPUTADO.MATERNO.Contains(busqueda)))
                                        {
                                            continue;
                                        }
                                    }
                                    //FILTRO PARA SABER SI EL INTERNO CON ACTIVIDAD SE ENCUENTRA FUERA DEL EDIFICIO
                                    var actividades = interno_ubicacion.Where(w => w.IdCentro == consulta_interno.ID_CENTRO && w.IdAnio == consulta_interno.ID_ANIO && w.IdImputado == consulta_interno.ID_IMPUTADO && w.IdIngreso == consulta_interno.ID_INGRESO).FirstOrDefault();
                                    //SI EL INTERNO SE ENCUENTRA FUERA DE SU CELDA PERO ES REQUERIDO...
                                    if (actividades != null)
                                    {
                                        interno_requerido_actividades = true;
                                    }
                                    else
                                    {
                                        interno_requerido_actividades = false;
                                    }
                                    //OBTIENE TODOS LOS INTERNOS DE TODOS LOS EDIFICIOS
                                    if (ID_UB_EDIFICIO == -1 && ID_UB_SECTOR == -1)
                                    {
                                        //revisar grupo horario la fecha y hora en que existe el empalme del consecutivo
                                        if (ingreso.Activo(consulta_interno))
                                        {
                                            if (filtrarBusqueda)
                                            {
                                                if (!(consulta_imputado.NOMBRE.Contains(busqueda) || consulta_imputado.PATERNO.Contains(busqueda) || consulta_imputado.MATERNO.Contains(busqueda)))
                                                {
                                                    continue;
                                                }
                                            }
                                            if ((item.HORA_INICIO.Value.Day == Fechas.GetFechaDateServer.Day && item.HORA_INICIO.Value.Month == Fechas.GetFechaDateServer.Month && item.HORA_INICIO.Value.Year == Fechas.GetFechaDateServer.Year &&
                                                (item.HORA_INICIO.Value.TimeOfDay <= Fechas.GetFechaDateServer.TimeOfDay && item.HORA_TERMINO.Value.TimeOfDay > Fechas.GetFechaDateServer.TimeOfDay)) || item.HORA_INICIO.Value >= Fechas.GetFechaDateServer)
                                            {
                                                ////SI TIENE TRASLADO...
                                                var interno_con_traslado = imputado.Find(f => f.IdImputado == consulta_imputado.ID_IMPUTADO && f.TrasladoInterno == true && (f.Fecha.Value.TimeOfDay >= item.HORA_INICIO.Value.TimeOfDay &&
                                                    f.FechaTermino.Value.TimeOfDay < item.HORA_TERMINO.Value.TimeOfDay));
                                                //SI EMPALMA CON CITAS MEDICAS...
                                                var cita_inter = imputado.Find(f => (f.IdImputado == consulta_imputado.ID_IMPUTADO) && f.CitaMedica == true && (f.FechaTermino.Value.TimeOfDay > item.HORA_INICIO.Value.TimeOfDay && f.Fecha.Value.TimeOfDay < item.HORA_TERMINO.Value.TimeOfDay));
                                                //SI HAY UNA EXCARCELACION QUE EMPALME...
                                                var cita_inter_exc = imputado.Find(f => (f.IdImputado == consulta_imputado.ID_IMPUTADO) && (f.ExcarcelacionInterno == true) && (f.FechaTermino.Value.TimeOfDay >= item.HORA_INICIO.Value.TimeOfDay && f.Fecha.Value.TimeOfDay < item.HORA_TERMINO.Value.TimeOfDay));
                                                //SI HAY EMPALME CON VISITA INTIMA O FAMILIAR...
                                                var cita_inter_vist = imputado.Where(f => (f.IdImputado == consulta_imputado.ID_IMPUTADO) && (f.VisitaIntima == true || f.VisitaFamiliar == true || f.VisitaLegal) && (f.FechaTermino.Value.TimeOfDay >= item.HORA_INICIO.Value.TimeOfDay && f.Fecha.Value.TimeOfDay < item.HORA_TERMINO.Value.TimeOfDay)).ToList();
                                                //SI HAY EMPALME CON CITAS TECNICAS
                                                var cita_inter_tecnica = imputado.Where(f => (f.IdImputado == consulta_imputado.ID_IMPUTADO) && (f.Citas == true) && (f.FechaTermino.Value.TimeOfDay > item.HORA_INICIO.Value.TimeOfDay && f.Fecha.Value.TimeOfDay < item.HORA_TERMINO.Value.TimeOfDay)).ToList();
                                                //SI HAY EMPALME CON ESTUDIOS
                                                var cita_inter_estudios = imputado.Where(f => (f.IdImputado == consulta_imputado.ID_IMPUTADO) && (f.EstudioInterno == true) && (f.FechaTermino.Value.TimeOfDay > item.HORA_INICIO.Value.TimeOfDay && f.Fecha.Value.TimeOfDay < item.HORA_TERMINO.Value.TimeOfDay)).ToList();

                                                if (cita_inter == null)
                                                {
                                                    if (cita_inter_exc == null)
                                                    {
                                                        if (cita_inter_vist == null || cita_inter_vist.Count == 0)
                                                        {
                                                            if (cita_inter_tecnica == null || cita_inter_tecnica.Count == 0)
                                                            {
                                                                if (cita_inter_estudios == null || cita_inter_estudios.Count == 0)
                                                                {
                                                                    if (interno_con_traslado == null)
                                                                    {
                                                                        if (asistencia.EMPALME != 0 && asistencia.EMPALME != null)
                                                                        {
                                                                            lstEmpalmados.Add(new InternosRequeridos()
                                                                            {
                                                                                LlaveInterno = string.Format("{0}/{1}/{2}/{3}", consulta_imputado.ID_CENTRO, consulta_imputado.ID_ANIO, consulta_imputado.ID_IMPUTADO, consulta_interno.ID_INGRESO),
                                                                                IdArea = item.ID_AREA,
                                                                                IdIngreso = consulta_interno.ID_INGRESO,
                                                                                Expediente = string.Format("{0}/{1}", consulta_imputado.ID_ANIO, consulta_imputado.ID_IMPUTADO).TrimEnd(),
                                                                                Paterno = string.IsNullOrEmpty(consulta_imputado.PATERNO) ? string.Empty : consulta_imputado.PATERNO.TrimEnd(),
                                                                                Materno = string.IsNullOrEmpty(consulta_imputado.MATERNO) ? string.Empty : consulta_imputado.MATERNO.TrimEnd(),
                                                                                Nombre = string.IsNullOrEmpty(consulta_imputado.NOMBRE) ? string.Empty : consulta_imputado.NOMBRE.TrimEnd(),
                                                                                Ubicacion = string.IsNullOrEmpty(ObtenerUbicacion()) ? string.Empty : ObtenerUbicacion().TrimEnd(),
                                                                                Estancia = string.Format("{0}-{1}-{2}-{3}", string.IsNullOrEmpty(edificio.DESCR) ? string.Empty : edificio.DESCR.TrimEnd(), string.IsNullOrEmpty(sector.DESCR) ? string.Empty : sector.DESCR.TrimEnd(), 
                                                                                string.IsNullOrEmpty(celda.ID_CELDA) ? string.Empty : celda.ID_CELDA.TrimEnd(), consulta_interno.CAMA.ID_CAMA),
                                                                                Actividad = string.IsNullOrEmpty(asistencia.GRUPO_PARTICIPANTE.ACTIVIDAD.DESCR) ? string.Empty : asistencia.GRUPO_PARTICIPANTE.ACTIVIDAD.DESCR.TrimEnd(),
                                                                                Area = string.IsNullOrEmpty(item.AREA.DESCR) ? string.Empty : item.AREA.DESCR.TrimEnd(),
                                                                                Fecha = item.HORA_INICIO,
                                                                                FechaTermino = item.HORA_TERMINO,
                                                                                Hora = new TimeSpan(item.HORA_INICIO.Value.Ticks),
                                                                                HoraFin = new TimeSpan(item.HORA_TERMINO.Value.Ticks),
                                                                                FechaRegistro = asistencia.FEC_REGISTRO,
                                                                                IdImputado = consulta_imputado.ID_IMPUTADO,
                                                                                Anio = consulta_imputado.ID_ANIO,
                                                                                Centro = consulta_imputado.ID_CENTRO,
                                                                                Id_Grupo = (short)asistencia.GRUPO_PARTICIPANTE.ID_GRUPO,
                                                                                Empalme = asistencia.EMPALME,
                                                                                EmpalmeAprobado = asistencia.EMP_APROBADO.HasValue ? asistencia.EMP_APROBADO.Value : 0,
                                                                                Prioridad = item.GRUPO.ACTIVIDAD.PRIORIDAD,
                                                                                EmpalmeCoordinacion = asistencia.EMP_COORDINACION,
                                                                                NIP = string.IsNullOrEmpty(consulta_imputado.NIP) ? string.Empty : consulta_imputado.NIP.TrimEnd(),
                                                                                ActividadInterno = true,
                                                                                Ingreso = consulta_interno,
                                                                                InternoRequerido = interno_requerido_actividades == true ? true : false
                                                                            });
                                                                        }
                                                                        else
                                                                        {
                                                                            imputado.Add(new InternosRequeridos()
                                                                            {
                                                                                LlaveInterno = string.Format("{0}/{1}/{2}/{3}", consulta_imputado.ID_CENTRO, consulta_imputado.ID_ANIO, consulta_imputado.ID_IMPUTADO, consulta_interno.ID_INGRESO),
                                                                                IdArea = item.ID_AREA,
                                                                                IdIngreso = consulta_interno.ID_INGRESO,
                                                                                Expediente = string.Format("{0}/{1}", consulta_imputado.ID_ANIO, consulta_imputado.ID_IMPUTADO).TrimEnd(),
                                                                                Paterno = string.IsNullOrEmpty(consulta_imputado.PATERNO) ? string.Empty : consulta_imputado.PATERNO.TrimEnd(),
                                                                                Materno = string.IsNullOrEmpty(consulta_imputado.MATERNO) ? string.Empty : consulta_imputado.MATERNO.TrimEnd(),
                                                                                Nombre = string.IsNullOrEmpty(consulta_imputado.NOMBRE) ? string.Empty : consulta_imputado.NOMBRE.TrimEnd(),
                                                                                Ubicacion = string.IsNullOrEmpty(ObtenerUbicacion()) ? string.Empty : ObtenerUbicacion().TrimEnd(),
                                                                                Estancia = string.Format("{0}-{1}-{2}-{3}", string.IsNullOrEmpty(edificio.DESCR) ? string.Empty : edificio.DESCR.TrimEnd(), string.IsNullOrEmpty(sector.DESCR) ? string.Empty : sector.DESCR.TrimEnd(),
                                                                                string.IsNullOrEmpty(celda.ID_CELDA) ? string.Empty : celda.ID_CELDA.TrimEnd(), consulta_interno.CAMA.ID_CAMA),
                                                                                Actividad = string.IsNullOrEmpty(asistencia.GRUPO_PARTICIPANTE.ACTIVIDAD.DESCR) ? string.Empty : asistencia.GRUPO_PARTICIPANTE.ACTIVIDAD.DESCR.TrimEnd(),
                                                                                Area = string.IsNullOrEmpty(item.AREA.DESCR) ? string.Empty : item.AREA.DESCR.TrimEnd(),
                                                                                Fecha = item.HORA_INICIO,
                                                                                FechaTermino = item.HORA_TERMINO,
                                                                                Hora = new TimeSpan(item.HORA_INICIO.Value.Ticks),
                                                                                HoraFin = new TimeSpan(item.HORA_TERMINO.Value.Ticks),
                                                                                FechaRegistro = asistencia.FEC_REGISTRO,
                                                                                IdImputado = consulta_imputado.ID_IMPUTADO,
                                                                                Anio = consulta_imputado.ID_ANIO,
                                                                                Centro = consulta_imputado.ID_CENTRO,
                                                                                Id_Grupo = (short)asistencia.GRUPO_PARTICIPANTE.ID_GRUPO,
                                                                                Empalme = asistencia.EMPALME,
                                                                                EmpalmeAprobado = asistencia.EMP_APROBADO.HasValue ? asistencia.EMP_APROBADO.Value : 0,
                                                                                Prioridad = item.GRUPO.ACTIVIDAD.PRIORIDAD,
                                                                                EmpalmeCoordinacion = asistencia.EMP_COORDINACION,
                                                                                NIP = string.IsNullOrEmpty(consulta_imputado.NIP) ? string.Empty : consulta_imputado.NIP.TrimEnd(),
                                                                                ActividadInterno = true,
                                                                                Ingreso = consulta_interno,
                                                                                InternoRequerido = interno_requerido_actividades == true ? true : false
                                                                            });
                                                                        }
                                                                    }
                                                                }
                                                            }
                                                        }
                                                    }
                                                }
                                            }
                                        }
                                    }
                                    // OBTIENE INTERNOS POR EDIFICIO Y POR SECTOR
                                    else
                                    {
                                        //revisar grupo horario la fecha y hora en que existe el empalme del consecutivo
                                        if (ingreso.Activo(consulta_interno) && EstaUbicacion(consulta_interno, ID_UB_EDIFICIO, ID_UB_SECTOR))
                                        {
                                            if (filtrarBusqueda)
                                            {
                                                if (!(consulta_imputado.NOMBRE.Contains(busqueda) || consulta_imputado.PATERNO.Contains(busqueda) || consulta_imputado.MATERNO.Contains(busqueda)))
                                                {
                                                    continue;
                                                }
                                            }
                                            if ((item.HORA_INICIO.Value.Day == Fechas.GetFechaDateServer.Day && item.HORA_INICIO.Value.Month == Fechas.GetFechaDateServer.Month && item.HORA_INICIO.Value.Year == Fechas.GetFechaDateServer.Year &&
                                            (item.HORA_INICIO.Value.TimeOfDay <= Fechas.GetFechaDateServer.TimeOfDay && item.HORA_TERMINO.Value.TimeOfDay > Fechas.GetFechaDateServer.TimeOfDay)) || item.HORA_INICIO.Value >= Fechas.GetFechaDateServer)
                                            {
                                                // var interno_con_cita = imputado.Find(f => (f.IdImputado == consulta_imputado.ID_IMPUTADO) && (f.FechaTermino.Value.TimeOfDay >= item.HORA_INICIO.Value.TimeOfDay && f.Fecha.Value.TimeOfDay < item.HORA_TERMINO.Value.TimeOfDay));
                                                // var interno_con_traslado = imputado.Find(f => f.IdImputado == consulta_imputado.ID_IMPUTADO && f.TrasladoInterno == true && (f.Fecha.Value.TimeOfDay <= item.HORA_INICIO.Value.TimeOfDay));
                                                ////SI TIENE TRASLADO...
                                                var interno_con_traslado = imputado.Find(f => f.IdImputado == consulta_imputado.ID_IMPUTADO && f.TrasladoInterno == true && (f.Fecha.Value.TimeOfDay >= item.HORA_INICIO.Value.TimeOfDay &&
                                                    f.FechaTermino.Value.TimeOfDay < item.HORA_TERMINO.Value.TimeOfDay));
                                                //SI EMPALMA CON CITAS MEDICAS...
                                                var cita_inter = imputado.Find(f => (f.IdImputado == consulta_imputado.ID_IMPUTADO) && f.CitaMedica == true && (f.FechaTermino.Value.TimeOfDay > item.HORA_INICIO.Value.TimeOfDay && f.Fecha.Value.TimeOfDay < item.HORA_TERMINO.Value.TimeOfDay));
                                                //SI HAY UNA EXCARCELACION QUE EMPALME...
                                                var cita_inter_exc = imputado.Find(f => (f.IdImputado == consulta_imputado.ID_IMPUTADO) && (f.ExcarcelacionInterno == true) && (f.FechaTermino.Value.TimeOfDay >= item.HORA_INICIO.Value.TimeOfDay && f.Fecha.Value.TimeOfDay < item.HORA_TERMINO.Value.TimeOfDay));
                                                //SI HAY EMPALME CON VISITA INTIMA O FAMILIAR...
                                                var cita_inter_vist = imputado.Where(f => (f.IdImputado == consulta_imputado.ID_IMPUTADO) && (f.VisitaIntima == true || f.VisitaFamiliar == true || f.VisitaLegal) && (f.FechaTermino.Value.TimeOfDay >= item.HORA_INICIO.Value.TimeOfDay && f.Fecha.Value.TimeOfDay < item.HORA_TERMINO.Value.TimeOfDay)).ToList();
                                                //SI HAY EMPALME CON CITAS TECNICAS
                                                var cita_inter_tecnica = imputado.Where(f => (f.IdImputado == consulta_imputado.ID_IMPUTADO) && (f.Citas == true) && (f.FechaTermino.Value.TimeOfDay > item.HORA_INICIO.Value.TimeOfDay && f.Fecha.Value.TimeOfDay < item.HORA_TERMINO.Value.TimeOfDay)).ToList();
                                                //SI HAY EMPALME CON ESTUDIOS
                                                var cita_inter_estudios = imputado.Where(f => (f.IdImputado == consulta_imputado.ID_IMPUTADO) && (f.EstudioInterno == true) && (f.FechaTermino.Value.TimeOfDay > item.HORA_INICIO.Value.TimeOfDay && f.Fecha.Value.TimeOfDay < item.HORA_TERMINO.Value.TimeOfDay)).ToList();

                                                if (cita_inter == null)
                                                {
                                                    if (cita_inter_exc == null)
                                                    {
                                                        if (cita_inter_vist == null || cita_inter_vist.Count == 0)
                                                        {
                                                            if (cita_inter_tecnica == null || cita_inter_tecnica.Count == 0)
                                                            {
                                                                if (cita_inter_estudios == null || cita_inter_estudios.Count == 0)
                                                                {
                                                                    if (interno_con_traslado == null)
                                                                    {
                                                                        if (asistencia.EMPALME != 0 && asistencia.EMPALME != null)
                                                                        {
                                                                            lstEmpalmados.Add(new InternosRequeridos()
                                                                            {
                                                                                LlaveInterno = string.Format("{0}/{1}/{2}/{3}", consulta_imputado.ID_CENTRO, consulta_imputado.ID_ANIO, consulta_imputado.ID_IMPUTADO, consulta_interno.ID_INGRESO),
                                                                                IdArea = item.ID_AREA,
                                                                                IdIngreso = consulta_interno.ID_INGRESO,
                                                                                Expediente = string.Format("{0}/{1}", consulta_imputado.ID_ANIO, consulta_imputado.ID_IMPUTADO).TrimEnd(),
                                                                                Paterno = string.IsNullOrEmpty(consulta_imputado.PATERNO) ? string.Empty : consulta_imputado.PATERNO.TrimEnd(),
                                                                                Materno = string.IsNullOrEmpty(consulta_imputado.MATERNO) ? string.Empty : consulta_imputado.MATERNO.TrimEnd(),
                                                                                Nombre = string.IsNullOrEmpty(consulta_imputado.NOMBRE) ? string.Empty : consulta_imputado.NOMBRE.TrimEnd(),
                                                                                Ubicacion = string.IsNullOrEmpty(ObtenerUbicacion()) ? string.Empty : ObtenerUbicacion().TrimEnd(),
                                                                                Estancia = string.Format("{0}-{1}-{2}-{3}", string.IsNullOrEmpty(edificio.DESCR) ? string.Empty : edificio.DESCR.TrimEnd(), string.IsNullOrEmpty(sector.DESCR) ? string.Empty : sector.DESCR.TrimEnd(),
                                                                                string.IsNullOrEmpty(celda.ID_CELDA) ? string.Empty : celda.ID_CELDA.TrimEnd(), consulta_interno.CAMA.ID_CAMA),
                                                                                Actividad = string.IsNullOrEmpty(asistencia.GRUPO_PARTICIPANTE.ACTIVIDAD.DESCR) ? string.Empty : asistencia.GRUPO_PARTICIPANTE.ACTIVIDAD.DESCR.TrimEnd(),
                                                                                Area = string.IsNullOrEmpty(item.AREA.DESCR) ? string.Empty : item.AREA.DESCR.TrimEnd(),
                                                                                Fecha = item.HORA_INICIO,
                                                                                FechaTermino = item.HORA_TERMINO,
                                                                                Hora = new TimeSpan(item.HORA_INICIO.Value.Ticks),
                                                                                HoraFin = new TimeSpan(item.HORA_TERMINO.Value.Ticks),
                                                                                FechaRegistro = asistencia.FEC_REGISTRO,
                                                                                IdImputado = consulta_imputado.ID_IMPUTADO,
                                                                                Anio = consulta_imputado.ID_ANIO,
                                                                                Centro = consulta_imputado.ID_CENTRO,
                                                                                Id_Grupo = (short)asistencia.GRUPO_PARTICIPANTE.ID_GRUPO,
                                                                                Empalme = asistencia.EMPALME,
                                                                                EmpalmeAprobado = asistencia.EMP_APROBADO.HasValue ? asistencia.EMP_APROBADO.Value : 0,
                                                                                Prioridad = item.GRUPO.ACTIVIDAD.PRIORIDAD,
                                                                                EmpalmeCoordinacion = asistencia.EMP_COORDINACION,
                                                                                NIP = string.IsNullOrEmpty(consulta_imputado.NIP) ? string.Empty : consulta_imputado.NIP.TrimEnd(),
                                                                                ActividadInterno = true,
                                                                                Ingreso = consulta_interno,
                                                                                InternoRequerido = interno_requerido_actividades == true ? true : false
                                                                            });
                                                                        }
                                                                        else
                                                                        {
                                                                            imputado.Add(new InternosRequeridos()
                                                                            {
                                                                                LlaveInterno = string.Format("{0}/{1}/{2}/{3}", consulta_imputado.ID_CENTRO, consulta_imputado.ID_ANIO, consulta_imputado.ID_IMPUTADO, consulta_interno.ID_INGRESO),
                                                                                IdArea = item.ID_AREA,
                                                                                IdIngreso = consulta_interno.ID_INGRESO,
                                                                                Expediente = string.Format("{0}/{1}", consulta_imputado.ID_ANIO, consulta_imputado.ID_IMPUTADO).TrimEnd(),
                                                                                Paterno = string.IsNullOrEmpty(consulta_imputado.PATERNO) ? string.Empty : consulta_imputado.PATERNO.TrimEnd(),
                                                                                Materno = string.IsNullOrEmpty(consulta_imputado.MATERNO) ? string.Empty : consulta_imputado.MATERNO.TrimEnd(),
                                                                                Nombre = string.IsNullOrEmpty(consulta_imputado.NOMBRE) ? string.Empty : consulta_imputado.NOMBRE.TrimEnd(),
                                                                                Ubicacion = string.IsNullOrEmpty(ObtenerUbicacion()) ? string.Empty : ObtenerUbicacion().TrimEnd(),
                                                                                Estancia = string.Format("{0}-{1}-{2}-{3}", string.IsNullOrEmpty(edificio.DESCR) ? string.Empty : edificio.DESCR.TrimEnd(), string.IsNullOrEmpty(sector.DESCR) ? string.Empty : sector.DESCR.TrimEnd(),
                                                                                string.IsNullOrEmpty(celda.ID_CELDA) ? string.Empty : celda.ID_CELDA.TrimEnd(), consulta_interno.CAMA.ID_CAMA),
                                                                                Actividad = string.IsNullOrEmpty(asistencia.GRUPO_PARTICIPANTE.ACTIVIDAD.DESCR) ? string.Empty : asistencia.GRUPO_PARTICIPANTE.ACTIVIDAD.DESCR.TrimEnd(),
                                                                                Area = string.IsNullOrEmpty(item.AREA.DESCR) ? string.Empty : item.AREA.DESCR.TrimEnd(),
                                                                                Fecha = item.HORA_INICIO,
                                                                                FechaTermino = item.HORA_TERMINO,
                                                                                Hora = new TimeSpan(item.HORA_INICIO.Value.Ticks),
                                                                                HoraFin = new TimeSpan(item.HORA_TERMINO.Value.Ticks),
                                                                                FechaRegistro = asistencia.FEC_REGISTRO,
                                                                                IdImputado = consulta_imputado.ID_IMPUTADO,
                                                                                Anio = consulta_imputado.ID_ANIO,
                                                                                Centro = consulta_imputado.ID_CENTRO,
                                                                                Id_Grupo = (short)asistencia.GRUPO_PARTICIPANTE.ID_GRUPO,
                                                                                Empalme = asistencia.EMPALME,
                                                                                EmpalmeAprobado = asistencia.EMP_APROBADO.HasValue ? asistencia.EMP_APROBADO.Value : 0,
                                                                                Prioridad = item.GRUPO.ACTIVIDAD.PRIORIDAD,
                                                                                EmpalmeCoordinacion = asistencia.EMP_COORDINACION,
                                                                                NIP = string.IsNullOrEmpty(consulta_imputado.NIP) ? string.Empty : consulta_imputado.NIP.TrimEnd(),
                                                                                ActividadInterno = true,
                                                                                Ingreso = consulta_interno,
                                                                                InternoRequerido = interno_requerido_actividades == true ? true : false
                                                                            });
                                                                        }
                                                                    }
                                                                }
                                                            }
                                                        }
                                                    }
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                foreach (var test in result_interno_yarda)
                {
                    var interno_fuera = false;
                    //FILTRO PARA SABER SI EL INTERNO CON ACTIVIDAD SE ENCUENTRA FUERA DEL EDIFICIO
                    var actividades = interno_ubicacion.Where(w => w.IdCentro == test.Centro && w.IdAnio == test.Anio && w.IdImputado == test.IdImputado && w.IdIngreso == test.IdIngreso).FirstOrDefault();
                    //SI EL INTERNO SE ENCUENTRA FUERA DE SU CELDA PERO ES REQUERIDO...
                    if (actividades != null)
                    {
                        interno_fuera = true;
                    }
                    else
                    {
                        interno_fuera = false;
                    }
                    var query = imputado.Where(w => (w.LlaveInterno == test.LlaveInterno) && (w.FechaTermino.Value.TimeOfDay >= test.Hora.Value && w.Fecha.Value.TimeOfDay < test.HoraFin.Value)).FirstOrDefault();
                    var interno_con_traslado = imputado.Find(f => f.IdImputado == test.IdImputado && f.TrasladoInterno == true && (f.Fecha.Value.TimeOfDay <= test.Hora.Value));

                    if (interno_con_traslado == null)
                    {
                        if (query == null)
                        {
                            test.InternoRequerido = interno_fuera == true ? true : false;
                            imputado.Add(test);
                        }
                        else
                        {
                            if (query.ExcarcelacionInterno || query.TrasladoInterno || query.VisitaLegal || query.VisitaIntima || query.VisitaFamiliar)
                            {
                                if ((query.FechaTermino.Value.TimeOfDay <= test.Hora.Value && query.Fecha.Value.TimeOfDay < test.Hora.Value) || query.Fecha.Value.TimeOfDay >= test.HoraFin.Value)
                                {
                                    test.InternoRequerido = interno_fuera == true ? true : false;
                                    imputado.Add(test);
                                }
                            }
                            else if (query.CitaMedica || query.Citas || query.EstudioInterno || query.ActividadInterno)
                            {
                                if (query.FechaTermino.Value.TimeOfDay <= test.Hora.Value || query.Fecha.Value.TimeOfDay >= test.HoraFin.Value)
                                {
                                    test.InternoRequerido = interno_fuera == true ? true : false;
                                    imputado.Add(test);
                                }
                            }
                        }
                    }
                }
                foreach (var item in ObtenerSinEmpalmes(lstEmpalmados))
                {
                    imputado.Add(item);
                }

                return imputado.OrderBy(o => o.BooleanToRowCitaMedica && o.BooleanToRowExcarcelacion && o.BooleanToRowTraslado && o.BooleanToRowVisitaLegal && o.Citas && o.BooleanToRowVisitaIntima && o.BooleanToRowVisitaFamiliar && o.EstudioInterno).ToList();
                #endregion
            }
            catch (Exception ex)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error...", ex);
                return null;
            }
        }

        /// <summary>
        /// Metodo que regresa lista con internos que tienen citas medicas
        /// </summary>
        /// <param name="FechaInicio"></param>
        /// <param name="FechaFin"></param>
        /// <param name="IdCentro"></param>
        /// <returns></returns>
        public List<InternosRequeridos> InternosCitaMedica(DateTime? FechaInicio, DateTime? FechaFin, short IdCentro)
        {
            //TODO: CITA MEDICA
            try
            {
                var nuevos = new cAtencionCita().ObtenerActivos(IdCentro, FechaInicio, FechaFin);
                var internos = nuevos.Any() ? nuevos.ToList().Select(s => new InternosRequeridos()
                {
                    Centro = s.ID_CENTRO.HasValue ? s.ID_CENTRO.Value : (short)0,
                    Anio = s.ID_ANIO.HasValue ? s.ID_ANIO.Value : (short)0,
                    IdImputado = s.ID_IMPUTADO.HasValue ? s.ID_IMPUTADO.Value : 0,
                    IdIngreso = s.ID_INGRESO.HasValue ? s.ID_INGRESO.Value : (short)0,
                    Paterno = string.IsNullOrEmpty(s.INGRESO.IMPUTADO.PATERNO) ? string.Empty : s.INGRESO.IMPUTADO.PATERNO.Trim(),
                    Materno = string.IsNullOrEmpty(s.INGRESO.IMPUTADO.MATERNO) ? string.Empty : s.INGRESO.IMPUTADO.MATERNO.Trim(),
                    Nombre = string.IsNullOrEmpty(s.INGRESO.IMPUTADO.NOMBRE) ? string.Empty : s.INGRESO.IMPUTADO.NOMBRE.Trim(),
                    NIP = s.INGRESO.IMPUTADO.NIP,
                    IdArea = s.ID_AREA,
                    Ubicacion = "ESTANCIA",
                    Actividad = s.ATENCION_SERVICIO != null ? s.ATENCION_SERVICIO.DESCR : "",
                    Area = s.AREA != null ? s.AREA.DESCR : "",
                    Fecha = s.CITA_FECHA_HORA,
                    FechaTermino = s.CITA_HORA_TERMINA,
                    Ingreso = s.INGRESO,
                    AtencionCita = s
                }).ToList() : null;

                if (internos != null)
                {
                    foreach (var x in internos)
                    {
                        x.Hora = new TimeSpan(x.AtencionCita.CITA_FECHA_HORA.Value.Ticks);
                        x.HoraFin = new TimeSpan(x.AtencionCita.CITA_HORA_TERMINA.Value.Ticks);
                        x.Expediente = string.Format("{0}/{1}", x.Ingreso.IMPUTADO.ID_ANIO, x.Ingreso.IMPUTADO.ID_IMPUTADO);
                        x.LlaveInterno = string.Format("{0}/{1}/{2}/{3}", x.Ingreso.ID_CENTRO, x.Ingreso.ID_ANIO, x.Ingreso.ID_IMPUTADO, x.Ingreso.ID_INGRESO);
                    }
                }
                return internos;
            }
            catch (Exception ex)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error...", ex);
                return null;
            }
        }

        /// <summary>
        /// Metodo que regresa lista con internos que tienen estudios de personalidad
        /// </summary>
        /// <param name="FechaInicio"></param>
        /// <param name="FechaFin"></param>
        /// <param name="IdCentro"></param>
        /// <returns></returns>
        public List<InternosRequeridos> InternosEstudiosPersonalidadProgramado(DateTime? FechaInicio, DateTime? FechaFin, short IdCentro)
        {
            try
            {
                var _EstudiosProgramados = new cPersonalidadDetalle().ObtenerInternosProgramados(FechaInicio, FechaFin, IdCentro);
                List<InternosRequeridos> req = new List<InternosRequeridos>();
                if (_EstudiosProgramados.Any())
                    foreach (var item in _EstudiosProgramados)
                    {
                        req.Add(new InternosRequeridos
                            {
                                LlaveInterno = string.Format("{0}/{1}/{2}/{3}", item.ID_CENTRO, item.ID_ANIO, item.ID_IMPUTADO, item.ID_INGRESO),
                                Centro = item.ID_CENTRO,
                                Anio = item.ID_ANIO,
                                IdImputado = item.ID_IMPUTADO,
                                IdIngreso = item.ID_INGRESO,
                                Paterno = item.PERSONALIDAD != null ? item.PERSONALIDAD.INGRESO != null ? item.PERSONALIDAD.INGRESO.IMPUTADO != null ? !string.IsNullOrEmpty(item.PERSONALIDAD.INGRESO.IMPUTADO.PATERNO) ? item.PERSONALIDAD.INGRESO.IMPUTADO.PATERNO.Trim() : string.Empty : string.Empty : string.Empty : string.Empty,
                                Materno = item.PERSONALIDAD != null ? item.PERSONALIDAD.INGRESO != null ? item.PERSONALIDAD.INGRESO.IMPUTADO != null ? !string.IsNullOrEmpty(item.PERSONALIDAD.INGRESO.IMPUTADO.MATERNO) ? item.PERSONALIDAD.INGRESO.IMPUTADO.MATERNO.Trim() : string.Empty : string.Empty : string.Empty : string.Empty,
                                Nombre = item.PERSONALIDAD != null ? item.PERSONALIDAD.INGRESO != null ? item.PERSONALIDAD.INGRESO.IMPUTADO != null ? !string.IsNullOrEmpty(item.PERSONALIDAD.INGRESO.IMPUTADO.NOMBRE) ? item.PERSONALIDAD.INGRESO.IMPUTADO.NOMBRE.Trim() : string.Empty : string.Empty : string.Empty : string.Empty,
                                NIP = item.PERSONALIDAD != null ? item.PERSONALIDAD.INGRESO != null ? item.PERSONALIDAD.INGRESO.IMPUTADO != null ? !string.IsNullOrEmpty(item.PERSONALIDAD.INGRESO.IMPUTADO.NIP) ? item.PERSONALIDAD.INGRESO.IMPUTADO.NIP.Trim() : string.Empty : string.Empty : string.Empty : string.Empty,
                                IdArea = item.ID_AREA,
                                Ubicacion = "ESTANCIA",
                                Actividad = "ESTUDIO DE PERSONALIDAD",
                                Area = item.ID_AREA.HasValue ? !string.IsNullOrEmpty(item.AREA.DESCR) ? item.AREA.DESCR.Trim() : string.Empty : string.Empty,
                                Fecha = item.INICIO_FEC,
                                FechaTermino = item.TERMINO_FEC,
                                Ingreso = item.PERSONALIDAD.INGRESO
                            });
                    };
                return req;
            }
            catch (Exception exc)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error...", exc);
                return null;
            }
        }

        /// <summary>
        /// Metodo que regresa una lista con internos que existen en un rango de celdas por edificio-sector
        /// </summary>
        /// <param name="IdCentro"></param>
        /// <param name="IdEdificio"></param>
        /// <param name="IdSector"></param>
        /// <returns></returns>
        public List<InternosRequeridos> InternosYarda(short? IdCentro = null, short? IdEdificio = null, short? IdSector = null)
        {
            try
            {
                var lista_internos = new List<InternosRequeridos>();
                var Hoy = Fechas.GetFechaDateServer;
                var DiaSemana = (short)Hoy.DayOfWeek;
                var HoraInicio = Hoy.Hour;
                var Minuto = Hoy.Minute;
                int minuto = HoraInicio * 60 + Minuto;
                var Edificio = IdEdificio;
                var Sector = IdSector;
                var lista = new cYarda().ObtenerTodos(IdCentro, Edificio, Sector, DiaSemana, minuto).FirstOrDefault();
                short? CeldaInicio = null;
                short? CeldaFin = null;
                TimeSpan? HoraInic = null;
                TimeSpan? HoraFinal = null;
                if (lista != null)
                {
                    CeldaInicio = lista.CELDA_INICIO;
                    CeldaFin = lista.CELDA_FINAL;
                    HoraInic = new TimeSpan(lista.HORA_INICIO.Value, lista.MINUTO_INICIO.Value, 0);
                    HoraFinal = new TimeSpan(lista.HORA_FIN.Value, lista.MINUTO_FIN.Value, 0);
                }
                if (lista == null)
                {
                    return lista_internos;
                }

                var X = new cIngreso().ObtenerIngresosPorSectorYarda(GlobalVar.gCentro, IdEdificio, IdSector, CeldaInicio, CeldaFin);
                //var countError = 0;
                var internos = new List<InternosRequeridos>();
                
                foreach (var s in X)
                {
                    internos.Add(
                        new InternosRequeridos()
                             {
                                 Centro = s.ID_CENTRO,
                                 Anio = s.ID_ANIO,
                                 IdImputado = s.ID_IMPUTADO,
                                 IdIngreso = s.ID_INGRESO,
                                 Paterno = string.IsNullOrEmpty(s.IMPUTADO.PATERNO) ? string.Empty : s.IMPUTADO.PATERNO,
                                 Materno = string.IsNullOrEmpty(s.IMPUTADO.MATERNO) ? string.Empty : s.IMPUTADO.MATERNO,
                                 Nombre = string.IsNullOrEmpty(s.IMPUTADO.NOMBRE) ? string.Empty : s.IMPUTADO.NOMBRE,
                                 NIP = string.IsNullOrEmpty(s.IMPUTADO.NIP) ? string.Empty : s.IMPUTADO.NIP.TrimEnd(),
                                 IdArea = lista.ID_AREA,
                                 Ubicacion = "ESTANCIA",
                                 Actividad = "YARDA",
                                 Area = string.IsNullOrEmpty(lista.AREA.DESCR) ? string.Empty : lista.AREA.DESCR,
                                 Fecha = Hoy,
                                 YardaInterno = true,
                                 Ingreso = s
                             });
                }


                //var internos = new cIngreso().ObtenerIngresosPorSectorYarda(GlobalVar.gCentro, IdEdificio, IdSector, CeldaInicio, CeldaFin).Select(s => new InternosRequeridos()
                //     {
                //         Centro = s.ID_CENTRO,
                //         Anio = s.ID_ANIO,
                //         IdImputado = s.ID_IMPUTADO,
                //         IdIngreso = s.ID_INGRESO,
                //         Paterno = string.IsNullOrEmpty(s.IMPUTADO.PATERNO) ? string.Empty : s.IMPUTADO.PATERNO,
                //         Materno = string.IsNullOrEmpty(s.IMPUTADO.MATERNO) ? string.Empty : s.IMPUTADO.MATERNO,
                //         Nombre = string.IsNullOrEmpty(s.IMPUTADO.NOMBRE) ? string.Empty : s.IMPUTADO.NOMBRE,
                //         NIP = string.IsNullOrEmpty(s.IMPUTADO.NIP) ? string.Empty : s.IMPUTADO.NIP.TrimEnd(),
                //         IdArea = lista.ID_AREA,
                //         Ubicacion = "ESTANCIA",
                //         Actividad = "YARDA",
                //         Area = string.IsNullOrEmpty(lista.AREA.DESCR) ? string.Empty : lista.AREA.DESCR,
                //         Fecha = Hoy,
                //         YardaInterno = true,
                //         Ingreso = s
                //     }).ToList();

                if (internos != null)
                {
                    foreach (var x in internos)
                    {
                        x.Expediente = string.Format("{0}/{1}", x.Ingreso.ID_ANIO, x.Ingreso.ID_IMPUTADO);
                        x.Hora = HoraInic;
                        x.HoraFin = HoraFinal;
                        x.LlaveInterno = string.Format("{0}/{1}/{2}/{3}", x.Ingreso.ID_CENTRO, x.Ingreso.ID_ANIO, x.Ingreso.ID_IMPUTADO, x.Ingreso.ID_INGRESO);
                    }
                }
                return internos;
            }
            catch (Exception ex)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error...", ex);
                return null;
            }
        }

        public string ObtenerUbicacion()
        {
            var interno_ubicacion = new cIngresoUbicacion().ObtenerTodos(GlobalVar.gCentro);
            Ubicacion = ubicacion_estancia;

            if (interno_ubicacion.Count > 0)
            {
                foreach (var item in interno_ubicacion)
                {
                    var interno_requerido = item.INGRESO;
                    var interno_ausente = item.INGRESO.INGRESO_UBICACION;

                    if (interno_requerido == interno_ausente)
                    {
                        if (item.ESTATUS == 1)
                        {
                            Ubicacion = ubicacion_transito;
                        }
                    }
                }
            }
            else
            {
                Ubicacion = ubicacion_estancia;
            }
            return Ubicacion;
        }

        private string ObtenerUbicacionInterno(INGRESO Ingreso)
        {
            var x = Ingreso.INGRESO_UBICACION.OrderByDescending(o => o.ID_CONSEC).FirstOrDefault();
            if (x != null)
            {
                if (x.ESTATUS == 0)
                {
                    return ubicacion_estancia;
                }
                else if (x.ESTATUS == 1)
                {
                    return string.Empty;
                }
            }
            return ubicacion_estancia;
        }

        /// <summary>
        /// Metodo que obtienes horarios de interno sin empalmes
        /// </summary>
        /// <param name="empalmados"></param>
        /// <returns></returns>
        public List<InternosRequeridos> ObtenerSinEmpalmes(List<InternosRequeridos> empalmados)
        {
            var final = new List<InternosRequeridos>();
            var horas_empalmadas = new List<InternosRequeridos>();
            var horas_finales = new List<InternosRequeridos>();
            var horas_comparar = new List<InternosRequeridos>();

            //agrupamos por los internos 
            var consulta = empalmados.GroupBy(g => new { g.Empalme, g.IdImputado });

            //vamos a filtrar por horas si se empalman
            foreach (var item in consulta)
            {
                var filtrar = empalmados.Where(w => w.IdImputado == item.Key.IdImputado && w.Empalme == item.Key.Empalme);

                //se obtiene la lista de filtrados y tomamos las fechas para empezar a comparar
                horas_empalmadas = filtrar.ToList();

                while (horas_empalmadas.Count > 0)
                {
                    //tomamos el primero
                    horas_finales.Add(horas_empalmadas.FirstOrDefault());
                    foreach (var empalmeitem in horas_empalmadas)
                    {
                        foreach (var hora in horas_finales.Where(w => w.Comparado == false))
                        {
                            //comparamos si se empalma
                            if (empalmeitem.Fecha >= hora.Fecha.Value && empalmeitem.Fecha <= hora.FechaTermino.Value)
                            {
                                //existe empalme por lo tanto lo agregamos a la lista para tomar la decision
                                horas_comparar.Add(empalmeitem);
                            }
                        }
                    }
                    //removemos
                    horas_comparar.ForEach((r) =>
                    {
                        horas_empalmadas.Remove(r);
                    });

                    horas_finales.ForEach((r) =>
                    {
                        r.Comparado = true; //ya se ha comparado
                    });

                    var agregar = Decision(horas_comparar);

                    if (agregar != null)
                        final.Add(agregar);

                    //limpiamos la lista
                    horas_comparar.Clear();
                }
            }
            return final;
        }

        /// <summary>
        /// Metodo que decide cual actividad dejar tomando enc uenta las condiciones
        /// </summary>
        /// <param name="lstDecidir"></param>
        /// <returns></returns>
        private InternosRequeridos Decision(List<InternosRequeridos> lstDecidir)
        {
            var final = new List<InternosRequeridos>();
            var coordinacion = lstDecidir.Where(w => w.EmpalmeCoordinacion == 2);//se tomo la decision

            if (coordinacion.Count() > 0)
            {
                var decision = coordinacion.Where(w => w.EmpalmeAprobado == 1).FirstOrDefault();//se selecciona el aprobado
                if (decision != null)
                {
                    return decision;
                }
                else return null;
            }
            else
            {
                var interno = lstDecidir.OrderBy(o => o.FechaRegistro).FirstOrDefault();
                if (interno != null)
                {
                    return interno;
                }
                else return null;
            }
        }

        /// <summary>
        /// Metodo que regresa si el ingreso del imputado coincide con el edificio y el sector o solamente edificio
        /// </summary>
        /// <param name="ingreso"></param>
        /// <param name="ID_UB_EDIFICIO"></param>
        /// <param name="ID_UB_SECTOR"></param>
        /// <returns></returns>
        private bool EstaUbicacion(INGRESO ingreso, short? ID_UB_EDIFICIO = null, short? ID_UB_SECTOR = null)
        {
            if (ID_UB_EDIFICIO == null && ID_UB_SECTOR == null)
            {
                return true;
            }
            else if (ID_UB_EDIFICIO != null && ID_UB_SECTOR != null)
            {
                return ingreso.ID_UB_EDIFICIO == ID_UB_EDIFICIO && ingreso.ID_UB_SECTOR == ID_UB_SECTOR;
            }
            else if (ID_UB_EDIFICIO != null && ID_UB_SECTOR == null)
            {
                return ingreso.ID_UB_EDIFICIO == ID_UB_EDIFICIO;
            }
            else throw new Exception("No puede buscar solo por sector sin recibir parámetro de edificio");
        }
        #endregion
    }
}
