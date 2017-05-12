using ControlPenales.BiometricoServiceReference;
using SSP.Controlador.Catalogo.Justicia;
using SSP.Servidor;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SSP.Controlador.Catalogo.Justicia.Ingreso;
using System.Collections.ObjectModel;
using System.Windows;

namespace ControlPenales.Clases.ControlInternos
{
    public class InternosAusentes
    {
        #region[VARIABLES]
        public string NIP { get; set; }
        public string Expediente { get; set; }
        public string Paterno { get; set; }
        public string Materno { get; set; }
        public string Nombre { get; set; }
        public string Ubicacion { get; set; }
        public string Estancia { get; set; }
        public string Actividad { get; set; }
        public string Area { get; set; }
        public DateTime? Fecha { get; set; }
        public int IdImputado { get; set; }
        public short IdAnio { get; set; }
        public short IdIngreso { get; set; }
        public short IdCentro { get; set; }
        public short? IdArea { get; set; }
        public int Consecutivo { get; set; }
        public TimeSpan? HoraInicio { get; set; }
        public TimeSpan? HoraFin { get; set; }
        public bool Entrada { get; set; }
        public short? Estatus { get; set; }
        public string Llave { get; set; }
        public INGRESO Ingreso { get; set; }
        #endregion

        #region[PARAMETROS]
        private string ubicacion_estancia = Parametro.UBICACION_ESTANCIA;
        private string ubicacion_transito = Parametro.UBICACION_TRANSITO;
        private int id_hospital_otros = Parametro.ID_HOSPITAL_OTROS;
        private short?[] ParametroEstatusInactivos = Parametro.ESTATUS_ADMINISTRATIVO_INACT;
        #endregion
        private enum enumTipoExcarcelacion
        {
            JURIDICA = 1,
            MEDICA = 2
        }

        //CHECKBOX SELECCIONAR INTERNO
        #region[PROPIEDADES]
        private bool _selectEliminar;
        public bool SELECTELIMINAR
        {
            get { return _selectEliminar; }
            set { _selectEliminar = value; }
        }

        private bool _selectEliminarEnabled;
        public bool SelectEliminarEnabled
        {
            get { return _selectEliminarEnabled; }
            set
            { _selectEliminarEnabled = value; }
        }
        #endregion

        #region[CONSTRUCTOR]
        public InternosAusentes() { }
        #endregion

        #region[METODOS]
        public ObservableCollection<InternosAusentes> ListaActividades(int id_imputado, DateTime? fechaInicio, DateTime? fechaFin, short ID_CENTRO, short? ID_UB_EDIFICIO = null, short? ID_UB_SECTOR = null)
        {
            var grupo_horario_actividad = new cGrupoHorario().ObtenerActivos(fechaInicio.Value, fechaFin.Value, GlobalVar.gCentro);
            var lstActividadInternos = new ObservableCollection<InternosAusentes>();

            foreach (var item in grupo_horario_actividad)
            {
                foreach (var asistencia in item.GRUPO_ASISTENCIA)
                {
                    var consulta_imputado = asistencia.GRUPO_PARTICIPANTE.INGRESO.IMPUTADO;
                    var consulta_interno_ubicacion = asistencia.GRUPO_PARTICIPANTE.INGRESO.INGRESO_UBICACION;
                    var consulta_interno = asistencia.GRUPO_PARTICIPANTE.INGRESO;
                    if (asistencia.ESTATUS == 1)
                    {
                        if (asistencia.GRUPO_PARTICIPANTE.ING_ID_IMPUTADO == id_imputado)
                        {
                            if (item.HORA_INICIO.Value.TimeOfDay > Fechas.GetFechaDateServer.TimeOfDay && item.HORA_TERMINO.Value.TimeOfDay > Fechas.GetFechaDateServer.TimeOfDay)
                            {
                                lstActividadInternos.Add(new InternosAusentes()
                                {
                                    Llave = string.Format("{0}/{1}/{2}/{3}", consulta_imputado.ID_CENTRO, consulta_imputado.ID_ANIO, consulta_imputado.ID_IMPUTADO, consulta_interno.ID_INGRESO),
                                    Expediente = string.Format("{0}/{1}", consulta_imputado.ID_ANIO, consulta_imputado.ID_IMPUTADO).TrimEnd(),
                                    Paterno = string.IsNullOrEmpty(consulta_imputado.PATERNO) ? string.Empty : consulta_imputado.PATERNO.TrimEnd(),
                                    Materno = string.IsNullOrEmpty(consulta_imputado.MATERNO) ? string.Empty : consulta_imputado.MATERNO.TrimEnd(),
                                    Nombre = consulta_imputado.NOMBRE.TrimEnd(),
                                    Actividad = string.IsNullOrEmpty(asistencia.GRUPO_PARTICIPANTE.ACTIVIDAD.DESCR) ? string.Empty : asistencia.GRUPO_PARTICIPANTE.ACTIVIDAD.DESCR.TrimEnd(),
                                    IdArea = item.ID_AREA,
                                    Area = item.AREA.DESCR.TrimEnd(),
                                    Fecha = asistencia.GRUPO_HORARIO.HORA_INICIO,
                                    HoraInicio = new TimeSpan(item.HORA_INICIO.Value.Ticks),
                                    HoraFin = new TimeSpan(item.HORA_TERMINO.Value.Ticks),
                                    Ingreso = consulta_interno
                                });
                            }
                        }
                    }
                }
            }
            return lstActividadInternos;
        }

        public ObservableCollection<InternosAusentes> ListInternosAusentes(short? ID_CENTRO)
        {
            try
            {
                var grupo_ausente = new cIngresoUbicacion().ObtenerTodos(ID_CENTRO);
                var lstAusentes = new ObservableCollection<InternosAusentes>();
                var lstAusentesEntrada = new ObservableCollection<InternosAusentes>();

                foreach (var item in grupo_ausente.OrderBy(o => o.ID_IMPUTADO).ThenBy(t => t.MOVIMIENTO_FEC))
                {
                    var consulta_imputado = item.INGRESO.IMPUTADO;
                    var consulta_interno = item.INGRESO.IMPUTADO.INGRESO.OrderByDescending(o => o.ID_INGRESO).FirstOrDefault();
                    var edificio = consulta_interno.CAMA.CELDA.SECTOR.EDIFICIO;
                    var sector = consulta_interno.CAMA.CELDA.SECTOR;
                    var celda = consulta_interno.CAMA.CELDA;

                    if (item.ESTATUS == 1)
                    {
                        var ia = new InternosAusentes();
                            ia.Llave = string.Format("{0}/{1}/{2}/{3}", consulta_imputado.ID_CENTRO, consulta_imputado.ID_ANIO, consulta_imputado.ID_IMPUTADO, consulta_interno.ID_INGRESO);
                            ia.Expediente = string.Format("{0}/{1}", consulta_imputado.ID_ANIO, consulta_imputado.ID_IMPUTADO).TrimEnd();
                            ia.Paterno = string.IsNullOrEmpty(consulta_imputado.PATERNO) ? string.Empty : consulta_imputado.PATERNO.TrimEnd();
                            ia.Materno = string.IsNullOrEmpty(consulta_imputado.MATERNO) ? string.Empty : consulta_imputado.MATERNO.TrimEnd();
                            ia.Nombre = consulta_imputado.NOMBRE.TrimEnd();
                            ia.Ubicacion = string.IsNullOrEmpty(ubicacion_transito) ? string.Empty : ubicacion_transito.TrimEnd();
                            ia.Estancia = string.Format("{0}-{1}-{2}-{3}", 
                                string.IsNullOrEmpty(edificio.DESCR) ? string.Empty : edificio.DESCR.TrimEnd(), 
                                string.IsNullOrEmpty(sector.DESCR) ? string.Empty : sector.DESCR.TrimEnd(),
                                string.IsNullOrEmpty(celda.ID_CELDA) ? string.Empty : celda.ID_CELDA.TrimEnd(), 
                                consulta_interno.CAMA.ID_CAMA);
                            ia.Actividad = string.IsNullOrEmpty(item.ACTIVIDAD) ? string.Empty : item.ACTIVIDAD.TrimEnd();
                            ia.IdArea = item.ID_AREA;
                            ia.Area = string.IsNullOrEmpty(item.ACTIVIDAD) ? string.Empty : (item.ACTIVIDAD.TrimEnd() == "ESTANCIA" ?
                                string.Format("{0}-{1}-{2}-{3}",
                                edificio != null ? !string.IsNullOrEmpty(edificio.DESCR) ? edificio.DESCR.TrimEnd() : string.Empty : string.Empty,
                                sector != null ? !string.IsNullOrEmpty(sector.DESCR) ? sector.DESCR.TrimEnd() : string.Empty : string.Empty,
                                celda != null ? !string.IsNullOrEmpty(celda.ID_CELDA) ?  celda.ID_CELDA.TrimEnd() : string.Empty : string.Empty,
                                consulta_interno.CAMA.ID_CAMA)
                                : ( item.AREA != null ? item.AREA.DESCR.TrimEnd() : string.Empty));
                            ia.Fecha = item.MOVIMIENTO_FEC.HasValue ? item.MOVIMIENTO_FEC : null;//Fechas.GetFechaDateServer;
                            ia.IdCentro = consulta_imputado.ID_CENTRO;
                            ia.IdAnio = consulta_imputado.ID_ANIO;
                            ia.IdIngreso = consulta_interno.ID_INGRESO;
                            ia.IdImputado = consulta_imputado.ID_IMPUTADO;
                            ia.Consecutivo = item.ID_CONSEC;
                            ia.NIP = !string.IsNullOrEmpty(consulta_imputado.NIP) ? consulta_imputado.NIP.TrimEnd() : string.Empty;
                            ia.Estatus = item.ESTATUS;
                            ia.Ingreso = consulta_interno;
                            lstAusentes.Add(ia);
                        //lstAusentes.Add(new InternosAusentes()
                        //{
                            //Llave = string.Format("{0}/{1}/{2}/{3}", consulta_imputado.ID_CENTRO, consulta_imputado.ID_ANIO, consulta_imputado.ID_IMPUTADO, consulta_interno.ID_INGRESO),
                            //Expediente = string.Format("{0}/{1}", consulta_imputado.ID_ANIO, consulta_imputado.ID_IMPUTADO).TrimEnd(),
                            //Paterno = string.IsNullOrEmpty(consulta_imputado.PATERNO) ? string.Empty : consulta_imputado.PATERNO.TrimEnd(),
                            //Materno = string.IsNullOrEmpty(consulta_imputado.MATERNO) ? string.Empty : consulta_imputado.MATERNO.TrimEnd(),
                            //Nombre = consulta_imputado.NOMBRE.TrimEnd(),
                            //Ubicacion = string.IsNullOrEmpty(ubicacion_transito) ? string.Empty : ubicacion_transito.TrimEnd(),
                            //Estancia = string.Format("{0}-{1}-{2}-{3}", string.IsNullOrEmpty(edificio.DESCR) ? string.Empty : edificio.DESCR.TrimEnd(), string.IsNullOrEmpty(sector.DESCR) ? string.Empty : sector.DESCR.TrimEnd(),
                            //    string.IsNullOrEmpty(celda.ID_CELDA) ? string.Empty : celda.ID_CELDA.TrimEnd(), consulta_interno.CAMA.ID_CAMA),
                            //Actividad = string.IsNullOrEmpty(item.ACTIVIDAD) ? string.Empty : item.ACTIVIDAD.TrimEnd(),
                            //IdArea = item.ID_AREA,
                            //Area = string.IsNullOrEmpty(item.ACTIVIDAD) ?
                            //    string.Empty
                            //: (item.ACTIVIDAD.TrimEnd() == "ESTANCIA" ?
                            //    string.Format("{0}-{1}-{2}-{3}", edificio.DESCR.TrimEnd(), sector.DESCR.TrimEnd(), celda.ID_CELDA.TrimEnd(), consulta_interno.CAMA.ID_CAMA)
                            //: item.AREA.DESCR.TrimEnd()),
                            //Fecha = item.MOVIMIENTO_FEC.HasValue ? item.MOVIMIENTO_FEC.Value : Fechas.GetFechaDateServer,
                            //IdCentro = consulta_imputado.ID_CENTRO,
                            //IdAnio = consulta_imputado.ID_ANIO,
                            //IdIngreso = consulta_interno.ID_INGRESO,
                            //IdImputado = consulta_imputado.ID_IMPUTADO,
                            //Consecutivo = item.ID_CONSEC,
                            //NIP = !string.IsNullOrEmpty(consulta_imputado.NIP) ? consulta_imputado.NIP.TrimEnd() : string.Empty,
                            //Estatus = item.ESTATUS,
                            //Ingreso = consulta_interno
                        //});
                    }
                    else if (item.ESTATUS == 2)
                    {
                        if (!ParametroEstatusInactivos.Contains(item.INGRESO.ID_ESTATUS_ADMINISTRATIVO))
                        {
                            lstAusentes.Add(new InternosAusentes()
                            {
                                Llave = string.Format("{0}/{1}/{2}/{3}", consulta_imputado.ID_CENTRO, consulta_imputado.ID_ANIO, consulta_imputado.ID_IMPUTADO, consulta_interno.ID_INGRESO),
                                Expediente = string.Format("{0}/{1}", consulta_imputado.ID_ANIO, consulta_imputado.ID_IMPUTADO).TrimEnd(),
                                Paterno = string.IsNullOrEmpty(consulta_imputado.PATERNO) ? string.Empty : consulta_imputado.PATERNO.TrimEnd(),
                                Materno = string.IsNullOrEmpty(consulta_imputado.MATERNO) ? string.Empty : consulta_imputado.MATERNO.TrimEnd(),
                                Nombre = consulta_imputado.NOMBRE.TrimEnd(),
                                Ubicacion = item.INGRESO.EXCARCELACION.Any(w => w.ID_ESTATUS == "AC") ? item.INGRESO.EXCARCELACION.First(w => w.ID_ESTATUS == "AC").ID_TIPO_EX == (short)enumTipoExcarcelacion.JURIDICA ?
                                item.INGRESO.EXCARCELACION.First(w => w.ID_ESTATUS == "AC").EXCARCELACION_DESTINO.First().JUZGADO.DESCR
                                : item.INGRESO.EXCARCELACION.First(w => w.ID_ESTATUS == "AC").EXCARCELACION_DESTINO.First().INTERCONSULTA_SOLICITUD.HOJA_REFERENCIA_MEDICA.First().ID_HOSPITAL == id_hospital_otros ?
                                item.INGRESO.EXCARCELACION.First(w => w.ID_ESTATUS == "AC").EXCARCELACION_DESTINO.First().INTERCONSULTA_SOLICITUD.HOJA_REFERENCIA_MEDICA.First().HOSPITAL_OTRO :
                                item.INGRESO.EXCARCELACION.First(w => w.ID_ESTATUS == "AC").EXCARCELACION_DESTINO.First().INTERCONSULTA_SOLICITUD.HOJA_REFERENCIA_MEDICA.First().HOSPITAL.DESCR :
                                item.AREA.DESCR.TrimEnd(),
                                Estancia = string.Format("{0}-{1}-{2}-{3}", edificio.DESCR.TrimEnd(), sector.DESCR.TrimEnd(), celda.ID_CELDA.TrimEnd(), consulta_interno.CAMA.ID_CAMA),
                                Actividad = string.IsNullOrEmpty(item.ACTIVIDAD) ? string.Empty : item.ACTIVIDAD.TrimEnd(),
                                IdArea = item.ID_AREA,
                                Area = item.INGRESO.EXCARCELACION.Any(w => w.ID_ESTATUS == "AC") ? item.INGRESO.EXCARCELACION.First(w => w.ID_ESTATUS == "AC").ID_TIPO_EX == (short)enumTipoExcarcelacion.JURIDICA ?
                                item.INGRESO.EXCARCELACION.First(w => w.ID_ESTATUS == "AC").EXCARCELACION_DESTINO.First().JUZGADO.DESCR
                                : item.INGRESO.EXCARCELACION.First(w => w.ID_ESTATUS == "AC").EXCARCELACION_DESTINO.First().INTERCONSULTA_SOLICITUD.HOJA_REFERENCIA_MEDICA.First().ID_HOSPITAL == id_hospital_otros ?
                                item.INGRESO.EXCARCELACION.First(w => w.ID_ESTATUS == "AC").EXCARCELACION_DESTINO.First().INTERCONSULTA_SOLICITUD.HOJA_REFERENCIA_MEDICA.First().HOSPITAL_OTRO :
                                item.INGRESO.EXCARCELACION.First(w => w.ID_ESTATUS == "AC").EXCARCELACION_DESTINO.First().INTERCONSULTA_SOLICITUD.HOJA_REFERENCIA_MEDICA.First().HOSPITAL.DESCR :
                                item.AREA.DESCR.TrimEnd(),
                                Fecha = item.MOVIMIENTO_FEC.HasValue ? item.MOVIMIENTO_FEC.Value : Fechas.GetFechaDateServer,
                                IdCentro = consulta_imputado.ID_CENTRO,
                                IdAnio = consulta_imputado.ID_ANIO,
                                IdIngreso = consulta_interno.ID_INGRESO,
                                IdImputado = consulta_imputado.ID_IMPUTADO,
                                Consecutivo = item.ID_CONSEC,
                                NIP = !string.IsNullOrEmpty(consulta_imputado.NIP) ? consulta_imputado.NIP.TrimEnd() : string.Empty,
                                Estatus = item.ESTATUS,
                                Ingreso = consulta_interno
                            });
                        }
                    }
                    else if (item.ESTATUS == 0)
                    {
                        lstAusentes.Add(new InternosAusentes()
                        {
                            Llave = string.Format("{0}/{1}/{2}/{3}", consulta_imputado.ID_CENTRO, consulta_imputado.ID_ANIO, consulta_imputado.ID_IMPUTADO, consulta_interno.ID_INGRESO),
                            Expediente = string.Format("{0}/{1}", consulta_imputado.ID_ANIO, consulta_imputado.ID_IMPUTADO).TrimEnd(),
                            Paterno = string.IsNullOrEmpty(consulta_imputado.PATERNO) ? string.Empty : consulta_imputado.PATERNO.TrimEnd(),
                            Materno = string.IsNullOrEmpty(consulta_imputado.MATERNO) ? string.Empty : consulta_imputado.MATERNO.TrimEnd(),
                            Nombre = consulta_imputado.NOMBRE.TrimEnd(),
                            Ubicacion = item.AREA.DESCR.TrimEnd(),
                            Estancia = string.Format("{0}-{1}-{2}-{3}", edificio.DESCR.TrimEnd(), sector.DESCR.TrimEnd(), celda.ID_CELDA.TrimEnd(), consulta_interno.CAMA.ID_CAMA),
                            Actividad = string.IsNullOrEmpty(item.ACTIVIDAD) ? string.Empty : item.ACTIVIDAD.TrimEnd(),
                            IdArea = item.ID_AREA,
                            Area = item.AREA.DESCR.TrimEnd(),
                            Fecha = item.MOVIMIENTO_FEC.HasValue ? item.MOVIMIENTO_FEC.Value : Fechas.GetFechaDateServer,
                            IdCentro = consulta_imputado.ID_CENTRO,
                            IdAnio = consulta_imputado.ID_ANIO,
                            IdIngreso = consulta_interno.ID_INGRESO,
                            IdImputado = consulta_imputado.ID_IMPUTADO,
                            Consecutivo = item.ID_CONSEC,
                            NIP = !string.IsNullOrEmpty(consulta_imputado.NIP) ? consulta_imputado.NIP.TrimEnd() : string.Empty,
                            Estatus = item.ESTATUS,
                            Ingreso = consulta_interno,
                            Entrada = true
                        });
                    }
                }
                //FILTRO QUE TOMA EN CUENTA EL ULTIMO MOVIMIENTO DEL INTERNO
                var query = (from p in lstAusentes
                             where p.Consecutivo ==
                             (from pp in lstAusentes
                              where pp.IdImputado == p.IdImputado
                              select pp.Consecutivo).Max()
                             select p).ToList();

                foreach (var x in query.Where(w => w.Entrada != true))
                {
                    lstAusentesEntrada.Add(x);
                }
                lstAusentesEntrada = new ObservableCollection<InternosAusentes>(lstAusentesEntrada);
                return lstAusentesEntrada;
            }
            catch (Exception ex)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al obtener internos ausentes", ex);
                return null;
            }
        #endregion
        }

        public ObservableCollection<InternosAusentes> ListInternosAusentesReporte(short? ID_CENTRO, DateTime? fechaInicio = null, DateTime? fechaFin = null)
        {
            try
            {
                var Resultado = new cIngresoUbicacion().ObtenerTodos(ID_CENTRO, fechaInicio, fechaFin).ToList();

                var lstAusentes = new ObservableCollection<InternosAusentes>();
                var lstAusentesEntrada = new ObservableCollection<InternosAusentes>();

                //var query = (from p in Resultado
                //                where p.ID_CONSEC ==
                //               (from pp in Resultado
                //                  where pp.ID_IMPUTADO == p.ID_IMPUTADO
                //                 select pp.ID_CONSEC).Max()
                //                 select p).ToList();

                //    internos_ausentes = new ObservableCollection<INGRESO_UBICACION>(query);

                foreach (var item in Resultado.OrderBy(o => o.ID_IMPUTADO).ThenBy(t => t.MOVIMIENTO_FEC))
                {
                    var consulta_imputado = item.INGRESO.IMPUTADO;
                    var consulta_interno = item.INGRESO;
                    var edificio = consulta_interno.CAMA.CELDA.SECTOR.EDIFICIO;
                    var sector = consulta_interno.CAMA.CELDA.SECTOR;
                    var celda = consulta_interno.CAMA.CELDA;

                    if (item.ESTATUS == 1)
                    {
                        lstAusentes.Add(new InternosAusentes()
                        {
                            Llave = string.Format("{0}/{1}/{2}/{3}", consulta_imputado.ID_CENTRO, consulta_imputado.ID_ANIO, consulta_imputado.ID_IMPUTADO, consulta_interno.ID_INGRESO),
                            Expediente = string.Format("{0}/{1}", consulta_imputado.ID_ANIO, consulta_imputado.ID_IMPUTADO).TrimEnd(),
                            Paterno = string.IsNullOrEmpty(consulta_imputado.PATERNO) ? string.Empty : consulta_imputado.PATERNO.TrimEnd(),
                            Materno = string.IsNullOrEmpty(consulta_imputado.MATERNO) ? string.Empty : consulta_imputado.MATERNO.TrimEnd(),
                            Nombre = string.IsNullOrEmpty(consulta_imputado.NOMBRE) ? string.Empty : consulta_imputado.NOMBRE.TrimEnd(),
                            Ubicacion = string.IsNullOrEmpty(ubicacion_transito) ? string.Empty : ubicacion_transito.TrimEnd(),
                            Estancia = string.Format("{0}-{1}-{2}-{3}", string.IsNullOrEmpty(edificio.DESCR) ? string.Empty : edificio.DESCR.TrimEnd(), string.IsNullOrEmpty(sector.DESCR) ? string.Empty : sector.DESCR.TrimEnd(),
                            string.IsNullOrEmpty(celda.ID_CELDA) ? string.Empty : celda.ID_CELDA.TrimEnd(), consulta_interno.CAMA.ID_CAMA),
                            Actividad = string.IsNullOrEmpty(item.ACTIVIDAD) ? string.Empty : item.ACTIVIDAD.TrimEnd(),
                            IdArea = item.ID_AREA,
                            Area = string.IsNullOrEmpty(item.ACTIVIDAD) ?
                                string.Empty
                            : (item.ACTIVIDAD.TrimEnd() == "ESTANCIA" ?
                                string.Format("{0}-{1}-{2}-{3}", string.IsNullOrEmpty(edificio.DESCR) ? string.Empty : edificio.DESCR.TrimEnd(), string.IsNullOrEmpty(sector.DESCR) ? string.Empty : sector.DESCR.TrimEnd(),
                                string.IsNullOrEmpty(celda.ID_CELDA) ? string.Empty : celda.ID_CELDA.TrimEnd(), consulta_interno.CAMA.ID_CAMA)
                            : item.AREA.DESCR.TrimEnd()),
                            Fecha = item.MOVIMIENTO_FEC.HasValue ? item.MOVIMIENTO_FEC.Value : Fechas.GetFechaDateServer,
                            IdCentro = consulta_imputado.ID_CENTRO,
                            IdAnio = consulta_imputado.ID_ANIO,
                            IdIngreso = consulta_interno.ID_INGRESO,
                            IdImputado = consulta_imputado.ID_IMPUTADO,
                            Consecutivo = item.ID_CONSEC,
                            NIP = !string.IsNullOrEmpty(consulta_imputado.NIP) ? consulta_imputado.NIP.TrimEnd() : string.Empty,
                            Estatus = item.ESTATUS,
                            Ingreso = consulta_interno
                        });
                    }
                    else if (item.ESTATUS == 2)
                    {
                        lstAusentes.Add(new InternosAusentes()
                        {
                            Llave = string.Format("{0}/{1}/{2}/{3}", consulta_imputado.ID_CENTRO, consulta_imputado.ID_ANIO, consulta_imputado.ID_IMPUTADO, consulta_interno.ID_INGRESO),
                            Expediente = string.Format("{0}/{1}", consulta_imputado.ID_ANIO, consulta_imputado.ID_IMPUTADO).TrimEnd(),
                            Paterno = string.IsNullOrEmpty(consulta_imputado.PATERNO) ? string.Empty : consulta_imputado.PATERNO.TrimEnd(),
                            Materno = string.IsNullOrEmpty(consulta_imputado.MATERNO) ? string.Empty : consulta_imputado.MATERNO.TrimEnd(),
                            Nombre = string.IsNullOrEmpty(consulta_imputado.NOMBRE) ? string.Empty : consulta_imputado.NOMBRE.TrimEnd(),
                            Ubicacion = item.INGRESO.EXCARCELACION.Any(w => w.ID_ESTATUS == "AC") ? item.INGRESO.EXCARCELACION.First(w => w.ID_ESTATUS == "AC").ID_TIPO_EX == (short)enumTipoExcarcelacion.JURIDICA ?
                            item.INGRESO.EXCARCELACION.First(w => w.ID_ESTATUS == "AC").EXCARCELACION_DESTINO.First().JUZGADO.DESCR
                            : item.INGRESO.EXCARCELACION.First(w => w.ID_ESTATUS == "AC").EXCARCELACION_DESTINO.First().INTERCONSULTA_SOLICITUD.HOJA_REFERENCIA_MEDICA.First().ID_HOSPITAL == id_hospital_otros ?
                            item.INGRESO.EXCARCELACION.First(w => w.ID_ESTATUS == "AC").EXCARCELACION_DESTINO.First().INTERCONSULTA_SOLICITUD.HOJA_REFERENCIA_MEDICA.First().HOSPITAL_OTRO :
                            item.INGRESO.EXCARCELACION.First(w => w.ID_ESTATUS == "AC").EXCARCELACION_DESTINO.First().INTERCONSULTA_SOLICITUD.HOJA_REFERENCIA_MEDICA.First().HOSPITAL.DESCR :
                            item.AREA.DESCR.TrimEnd(),
                            Estancia = string.Format("{0}-{1}-{2}-{3}", edificio.DESCR.TrimEnd(), sector.DESCR.TrimEnd(), celda.ID_CELDA.TrimEnd(), consulta_interno.CAMA.ID_CAMA),
                            Actividad = string.IsNullOrEmpty(item.ACTIVIDAD) ? string.Empty : item.ACTIVIDAD.TrimEnd(),
                            IdArea = item.ID_AREA,
                            Area = item.INGRESO.EXCARCELACION.Any(w => w.ID_ESTATUS == "AC") ? item.INGRESO.EXCARCELACION.First(w => w.ID_ESTATUS == "AC").ID_TIPO_EX == (short)enumTipoExcarcelacion.JURIDICA ?
                            item.INGRESO.EXCARCELACION.First(w => w.ID_ESTATUS == "AC").EXCARCELACION_DESTINO.First().JUZGADO.DESCR
                            : item.INGRESO.EXCARCELACION.First(w => w.ID_ESTATUS == "AC").EXCARCELACION_DESTINO.First().INTERCONSULTA_SOLICITUD.HOJA_REFERENCIA_MEDICA.First().ID_HOSPITAL == id_hospital_otros ?
                            item.INGRESO.EXCARCELACION.First(w => w.ID_ESTATUS == "AC").EXCARCELACION_DESTINO.First().INTERCONSULTA_SOLICITUD.HOJA_REFERENCIA_MEDICA.First().HOSPITAL_OTRO :
                            item.INGRESO.EXCARCELACION.First(w => w.ID_ESTATUS == "AC").EXCARCELACION_DESTINO.First().INTERCONSULTA_SOLICITUD.HOJA_REFERENCIA_MEDICA.First().HOSPITAL.DESCR :
                            item.AREA.DESCR.TrimEnd(),
                            Fecha = item.MOVIMIENTO_FEC.HasValue ? item.MOVIMIENTO_FEC.Value : Fechas.GetFechaDateServer,
                            IdCentro = consulta_imputado.ID_CENTRO,
                            IdAnio = consulta_imputado.ID_ANIO,
                            IdIngreso = consulta_interno.ID_INGRESO,
                            IdImputado = consulta_imputado.ID_IMPUTADO,
                            Consecutivo = item.ID_CONSEC,
                            NIP = !string.IsNullOrEmpty(consulta_imputado.NIP) ? consulta_imputado.NIP.TrimEnd() : string.Empty,
                            Estatus = item.ESTATUS,
                            Ingreso = consulta_interno
                        });
                    }
                    else if (item.ESTATUS == 0)
                    {
                        lstAusentes.Add(new InternosAusentes()
                        {
                            Llave = string.Format("{0}/{1}/{2}/{3}", consulta_imputado.ID_CENTRO, consulta_imputado.ID_ANIO, consulta_imputado.ID_IMPUTADO, consulta_interno.ID_INGRESO),
                            Expediente = string.Format("{0}/{1}", consulta_imputado.ID_ANIO, consulta_imputado.ID_IMPUTADO).TrimEnd(),
                            Paterno = string.IsNullOrEmpty(consulta_imputado.PATERNO) ? string.Empty : consulta_imputado.PATERNO.TrimEnd(),
                            Materno = string.IsNullOrEmpty(consulta_imputado.MATERNO) ? string.Empty : consulta_imputado.MATERNO.TrimEnd(),
                            Nombre = string.IsNullOrEmpty(consulta_imputado.NOMBRE) ? string.Empty : consulta_imputado.NOMBRE.TrimEnd(),
                            Ubicacion = string.IsNullOrEmpty(item.AREA.DESCR) ? string.Empty : item.AREA.DESCR.TrimEnd(),
                            Estancia = string.Format("{0}-{1}-{2}-{3}", edificio.DESCR.TrimEnd(), sector.DESCR.TrimEnd(), celda.ID_CELDA.TrimEnd(), consulta_interno.CAMA.ID_CAMA),
                            Actividad = string.IsNullOrEmpty(item.ACTIVIDAD) ? string.Empty : item.ACTIVIDAD.TrimEnd(),
                            IdArea = item.ID_AREA,
                            Area = string.IsNullOrEmpty(item.AREA.DESCR) ? string.Empty : item.AREA.DESCR.TrimEnd(),
                            Fecha = item.MOVIMIENTO_FEC.HasValue ? item.MOVIMIENTO_FEC.Value : Fechas.GetFechaDateServer,
                            IdCentro = consulta_imputado.ID_CENTRO,
                            IdAnio = consulta_imputado.ID_ANIO,
                            IdIngreso = consulta_interno.ID_INGRESO,
                            IdImputado = consulta_imputado.ID_IMPUTADO,
                            Consecutivo = item.ID_CONSEC,
                            NIP = !string.IsNullOrEmpty(consulta_imputado.NIP) ? consulta_imputado.NIP.TrimEnd() : string.Empty,
                            Estatus = item.ESTATUS,
                            Ingreso = consulta_interno,
                            Entrada = true
                        });
                    }
                }
                //FILTRO QUE TOMA EN CUENTA EL ULTIMO MOVIMIENTO DEL INTERNO
                //var query2 = (from p in lstAusentes
                //             where p.Consecutivo ==
                //             (from pp in lstAusentes
                //              where pp.IdImputado == p.IdImputado
                //              select pp.Consecutivo).Max()
                //             select p).ToList();

                //foreach (var x in query2)
                //{
                //    lstAusentesEntrada.Add(x);
                //}
                lstAusentesEntrada = new ObservableCollection<InternosAusentes>(lstAusentes);
                return lstAusentesEntrada;
            }
            catch (Exception ex)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al obtener internos ausentes", ex);
                return null;
            }
        }
    }
}