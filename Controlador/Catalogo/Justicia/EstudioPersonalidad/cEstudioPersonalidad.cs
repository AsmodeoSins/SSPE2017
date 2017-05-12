using LinqKit;
using SSP.Servidor;
using System.Linq;
using System.Transactions;

namespace SSP.Controlador.Catalogo.Justicia
{
    public class cEstudioPersonalidad : SSP.Modelo.EntityManagerServer<PERSONALIDAD>
    {
        public cEstudioPersonalidad() { }

        private enum eTiposEstudio
        {//Se define enumerador para alimentar detalle de estudio de personalidad
            CRIMINOLOGICO = 1,
            TRABAJO_SOCIAL = 2,
            SEGURIDAD = 3,
            MEDICO = 4,
            PSICOLOGIA = 5,
            PSIQUIATRIA = 6,
            PEDAGOGIA = 7,
            LABORAL = 8,
            CRIMIN_FEDERAL = 9,
            TRABAJO_SOCIAL_FEDERAL = 10,
            SEGURIDAD_FEDERAL = 11,
            MEDICA_FEDERAL = 12,
            PSIQUIATRICA_FEDERAL = 13,
            PSICOLOGICA_FEDERAL = 14,
            PEDAGOGICA_FEDERAL = 15,
            LABORAL_FEDERAL = 16
        };

        private enum eEstatudDetallePersonalidad
        {
            ACTIVO = 1,
            PENDIENTE = 2,
            TERMINADO = 3,
            CANCELADO = 4,
            ASIGNADO = 5
        };

        public IQueryable<SSP.Servidor.PERSONALIDAD> ObtenerDatosEstudiosProgramados(short IdCentro)
        {
            try
            {
               var predicado = PredicateBuilder.True<PERSONALIDAD>();
               predicado = predicado.And(x => x.ID_SITUACION != 3);
               predicado = predicado.And(x => x.ID_SITUACION != 4);
               predicado = predicado.And(x => x.RESULT_ESTUDIO == null);
               predicado = predicado.And(x => x.INGRESO.ID_UB_CENTRO == IdCentro);
               return GetData(predicado.Expand()).OrderBy(x => x.NUM_OFICIO);
            }
            catch (System.Exception exc)
            {
                throw;
            }
        }

        public bool CancelaEstudio(INCIDENTE Incidente, short TipoMensaje, short IdEstudio)
        {
            try
            {
                if (Incidente == null)
                    return false;

                using (System.Transactions.TransactionScope transaccion = new System.Transactions.TransactionScope(System.Transactions.TransactionScopeOption.RequiresNew, new System.Transactions.TransactionOptions() { IsolationLevel = System.Transactions.IsolationLevel.ReadCommitted }))
                {
                    string NombreImputado = string.Empty;
                    var _consecutivoIncidente = GetIDProceso<short>("INCIDENTE", "ID_INCIDENTE", string.Format("ID_CENTRO = {0} AND ID_ANIO = {1} AND ID_IMPUTADO = {2} AND ID_INGRESO = {3}", Incidente.ID_CENTRO, Incidente.ID_ANIO, Incidente.ID_IMPUTADO, Incidente.ID_INGRESO));

                    var Imputado = Context.IMPUTADO.FirstOrDefault(x => x.ID_IMPUTADO == Incidente.ID_IMPUTADO && x.ID_ANIO == Incidente.ID_ANIO && x.ID_CENTRO == Incidente.ID_CENTRO);
                    var Incident = new INCIDENTE()
                    {
                        AUTORIZACION_FEC = Incidente.AUTORIZACION_FEC,
                        ESTATUS = Incidente.ESTATUS,
                        ID_ANIO = Incidente.ID_ANIO,
                        ID_IMPUTADO = Incidente.ID_IMPUTADO,
                        REGISTRO_FEC = Incidente.REGISTRO_FEC,
                        ID_CENTRO = Incidente.ID_CENTRO,
                        ID_INGRESO = Incidente.ID_INGRESO,
                        MOTIVO = Incidente.MOTIVO,
                        ID_INCIDENTE_TIPO = Incidente.ID_INCIDENTE_TIPO,
                        ID_INCIDENTE = _consecutivoIncidente
                    };

                    var _EstudiosPersonalidadProgramados = Context.PERSONALIDAD.FirstOrDefault(x => x.ID_ESTUDIO == IdEstudio && x.ID_INGRESO == Incidente.ID_INGRESO && x.ID_IMPUTADO == Incidente.ID_IMPUTADO && Incidente.ID_ANIO == Incidente.ID_ANIO && x.ID_CENTRO == Incidente.ID_CENTRO);

                    if (_EstudiosPersonalidadProgramados != null)
                    {
                        _EstudiosPersonalidadProgramados.ID_SITUACION = (short)eEstatudDetallePersonalidad.CANCELADO;
                        if (_EstudiosPersonalidadProgramados.PERSONALIDAD_DETALLE != null)
                            if (_EstudiosPersonalidadProgramados.PERSONALIDAD_DETALLE.Any())
                                foreach (var item in _EstudiosPersonalidadProgramados.PERSONALIDAD_DETALLE)
                                    item.ID_ESTATUS = (short)eEstatudDetallePersonalidad.CANCELADO;

                        Context.Entry(_EstudiosPersonalidadProgramados).State = System.Data.EntityState.Modified;

                        NombreImputado = string.Format("{0} / {1} {2} {3} {4}",
                            Imputado != null ? Imputado.ID_ANIO.ToString() : string.Empty,
                            Imputado != null ? Imputado.ID_IMPUTADO.ToString() : string.Empty,
                            Imputado != null ? !string.IsNullOrEmpty(Imputado.NOMBRE) ? Imputado.NOMBRE.Trim() : string.Empty : string.Empty,
                            Imputado != null ? !string.IsNullOrEmpty(Imputado.PATERNO) ? Imputado.PATERNO.Trim() : string.Empty : string.Empty,
                            Imputado != null ? !string.IsNullOrEmpty(Imputado.MATERNO) ? Imputado.MATERNO.Trim() : string.Empty : string.Empty);

                        System.DateTime _fecha = GetFechaServerDate();
                        var _MensajeConsiderado = Context.MENSAJE_TIPO.FirstOrDefault(x => x.ID_MEN_TIPO == TipoMensaje);
                        string _Nombre = string.Empty;
                        var _id_mensaje = GetIDProceso<int>("MENSAJE", "ID_MENSAJE", "1=1");
                        if (_MensajeConsiderado != null)
                            if (_id_mensaje != decimal.Zero)
                            {
                                var NvoMensaje = new MENSAJE()
                                {
                                    CONTENIDO = string.Format("{0} {1} {2} \n BAJO EL SIGUIENTE MOTIVO: {3}", _MensajeConsiderado.CONTENIDO, " PARA EL IMPUTADO \n ", NombreImputado, Incident.MOTIVO),
                                    ENCABEZADO = _MensajeConsiderado.ENCABEZADO,
                                    ID_MEN_TIPO = _MensajeConsiderado.ID_MEN_TIPO,
                                    ID_MENSAJE = _id_mensaje,
                                    REGISTRO_FEC = _fecha,
                                    ID_CENTRO = GlobalVariables.gCentro
                                };

                                Context.MENSAJE.Add(NvoMensaje);
                            };
                    };

                    Context.SaveChanges();
                    transaccion.Complete();
                    return true;
                }
            }
            catch (System.Exception exc)
            {
                throw new System.ApplicationException(exc.Message);
            }
        }

        public IQueryable<PERSONALIDAD> ObtenerDatosBrigada(string NombreB, string NombreFolio)
        {
            try
            {
                var predicado = PredicateBuilder.True<PERSONALIDAD>();
                predicado = predicado.And(x => !string.IsNullOrEmpty(x.RESULT_ESTUDIO));

                if (!string.IsNullOrEmpty(NombreB))
                    predicado = predicado.And(c => c.PROG_NOMBRE.Trim() == NombreB);

                if (!string.IsNullOrEmpty(NombreFolio))
                    predicado = predicado.And(c => c.NUM_OFICIO.Trim() == NombreFolio);

                return GetData(predicado.Expand());
            }
            catch (System.Exception exc)
            {
                throw new System.ApplicationException(exc.Message);
            }
        }
        public IQueryable<PERSONALIDAD> ObtenerEstudiosHoy(System.DateTime? Fecha)
        {
            try
            {
                var predicado = PredicateBuilder.True<PERSONALIDAD>();
                if (Fecha.HasValue)
                    predicado = predicado.And(x => x.SOLICITUD_FEC.Value.Year == Fecha.Value.Year && x.SOLICITUD_FEC.Value.Month == Fecha.Value.Month && x.SOLICITUD_FEC.Value.Day == Fecha.Value.Day);
                else
                    predicado = predicado.And(x => x.SOLICITUD_FEC == System.DateTime.Parse(GetFechaServer()));

                return GetData(predicado.Expand());

            }
            catch (System.Exception exc)
            {
                throw new System.ApplicationException(exc.Message);
            }
        }

        public IQueryable<PERSONALIDAD> ObtenerInternosProgramados(System.DateTime? FechaInicio, System.DateTime? FechaFin, short IdCentro)
        {
            try
            {
                var predicado = PredicateBuilder.True<PERSONALIDAD>();

                //definicion de predicado base
                predicado.And(x => x.ID_AREA != null && x.INICIO_FEC != null && x.TERMINO_FEC != null && x.ID_CENTRO == IdCentro);

                //definicion de predicado complementado con paramteros
                if (FechaInicio.HasValue)
                    predicado = predicado.And(w => System.Data.Objects.EntityFunctions.TruncateTime(w.INICIO_FEC) >= FechaInicio);

                if (FechaFin.HasValue)
                    predicado = predicado.And(w => System.Data.Objects.EntityFunctions.TruncateTime(w.TERMINO_FEC) <= FechaFin);

                return GetData(predicado.Expand());
            }
            catch (System.Exception exc)
            {
                throw new System.ApplicationException(exc.Message);
            }
        }

        public IQueryable<PERSONALIDAD> ObtenerTodos(int Imputado, short Ingreso)
        {
            try
            {
                var predicate = PredicateBuilder.True<PERSONALIDAD>();
                if (Imputado != decimal.Zero)
                    predicate = predicate.And(w => w.ID_IMPUTADO == Imputado);

                if (Ingreso != decimal.Zero)
                    predicate = predicate.And(w => w.ID_INGRESO == Ingreso);

                return GetData(predicate.Expand()).OrderBy(x => x.ID_ESTUDIO);
            }

            catch (System.Exception ex)
            {
                throw new System.ApplicationException(ex.Message);
            }
        }

        public bool Insertar(PERSONALIDAD Entity)
        {
            try
            {
                short _ConsecutivoPersonalidad = GetIDProceso<short>("PERSONALIDAD", "ID_ESTUDIO", "1=1");
                Entity.ID_ESTUDIO = _ConsecutivoPersonalidad;
                if (Insert(Entity))
                    return true;

                return false;
            }
            catch (System.Exception ex)
            {
                throw new System.ApplicationException(ex.Message);
            }
        }

        public bool CierraEstudiosPersonalidad(System.Collections.Generic.List<PERSONALIDAD> lstEstudiosCerrar)
        {
            try
            {
                if (lstEstudiosCerrar == null)
                    return false;

                if (lstEstudiosCerrar.Any())
                {
                    using (System.Transactions.TransactionScope transaccion = new System.Transactions.TransactionScope(System.Transactions.TransactionScopeOption.RequiresNew, new System.Transactions.TransactionOptions() { IsolationLevel = System.Transactions.IsolationLevel.ReadCommitted }))
                    {
                        foreach (var item in lstEstudiosCerrar)
                        {
                            var _EstudioActual = Context.PERSONALIDAD.FirstOrDefault(x => x.ID_ANIO == item.ID_ANIO && x.ID_CENTRO == item.ID_CENTRO && x.ID_INGRESO == item.ID_INGRESO && x.ID_IMPUTADO == item.ID_IMPUTADO && x.ID_ESTUDIO == item.ID_ESTUDIO);

                            if (_EstudioActual != null)
                            {
                                _EstudioActual.ID_ANIO = item.ID_ANIO;
                                _EstudioActual.ID_CENTRO = item.ID_CENTRO;
                                _EstudioActual.ID_ESTUDIO = item.ID_ESTUDIO;
                                _EstudioActual.ID_IMPUTADO = item.ID_IMPUTADO;
                                _EstudioActual.ID_INGRESO = item.ID_INGRESO;
                                _EstudioActual.ID_MOTIVO = item.ID_MOTIVO;
                                _EstudioActual.ID_SITUACION = item.RESULT_ESTUDIO == "True" ? short.Parse((3).ToString()) : short.Parse((5).ToString());
                                _EstudioActual.INICIO_FEC = item.INICIO_FEC;
                                _EstudioActual.SOLICITADO = item.SOLICITADO;
                                _EstudioActual.SOLICITUD_FEC = item.SOLICITUD_FEC;
                                _EstudioActual.TERMINO_FEC = item.TERMINO_FEC;
                                _EstudioActual.PROG_NOMBRE = item.PROG_NOMBRE;
                                _EstudioActual.NUM_OFICIO = item.NUM_OFICIO;
                                _EstudioActual.ID_AREA = item.ID_AREA;
                                Context.Entry(_EstudioActual).State = System.Data.EntityState.Modified;
                            }
                        };

                        Context.SaveChanges();
                        transaccion.Complete();
                        return true;
                    }
                }
            }
            catch (System.Exception)
            {
                return false;
            }

            return false;
        }
        public bool ProgramaEstudiosPersonalidad(System.Collections.Generic.List<PERSONALIDAD> lstPersonalidad, short TipoMensaje)
        {
            try
            {
                if (lstPersonalidad != null && !lstPersonalidad.Any())
                    return false;

                using (System.Transactions.TransactionScope transaccion = new System.Transactions.TransactionScope(System.Transactions.TransactionScopeOption.RequiresNew, new System.Transactions.TransactionOptions() { IsolationLevel = System.Transactions.IsolationLevel.ReadCommitted }))
                {
                    string CuerpoMensaje = string.Empty;
                    foreach (var item in lstPersonalidad)
                    {
                        var _EstudioActual = Context.PERSONALIDAD.FirstOrDefault(x => x.ID_ANIO == item.ID_ANIO && x.ID_CENTRO == item.ID_CENTRO && x.ID_INGRESO == item.ID_INGRESO && x.ID_IMPUTADO == item.ID_IMPUTADO && x.ID_ESTUDIO == item.ID_ESTUDIO);

                        if (_EstudioActual != null)
                        {
                            string _temp = string.Format(" - {0}/{1} {2} {3} {4} CON FECHA: {5} \n", item.INGRESO != null ? item.INGRESO.IMPUTADO != null ? item.INGRESO.IMPUTADO.ID_ANIO.ToString() : string.Empty : string.Empty,//0
                                item.INGRESO != null ? item.INGRESO.IMPUTADO != null ? item.INGRESO.IMPUTADO.ID_IMPUTADO.ToString() : string.Empty : string.Empty,//1
                                item.INGRESO != null ? item.INGRESO.IMPUTADO != null ? !string.IsNullOrEmpty(item.INGRESO.IMPUTADO.NOMBRE) ? item.INGRESO.IMPUTADO.NOMBRE.Trim() : string.Empty : string.Empty : string.Empty,//2
                                item.INGRESO != null ? item.INGRESO.IMPUTADO != null ? !string.IsNullOrEmpty(item.INGRESO.IMPUTADO.PATERNO) ? item.INGRESO.IMPUTADO.PATERNO.Trim() : string.Empty : string.Empty : string.Empty,//3
                                item.INGRESO != null ? item.INGRESO.IMPUTADO != null ? !string.IsNullOrEmpty(item.INGRESO.IMPUTADO.MATERNO) ? item.INGRESO.IMPUTADO.MATERNO.Trim() : string.Empty : string.Empty : string.Empty,//4
                                item.INICIO_FEC.HasValue ? item.INICIO_FEC.Value.ToString("dd/MM/yyyy hh:mm:ss") : string.Empty);//5

                            if ((CuerpoMensaje.Length + _temp.Length) < 1700)
                                CuerpoMensaje += _temp;

                            _EstudioActual.ID_ANIO = item.ID_ANIO;
                            _EstudioActual.ID_CENTRO = item.ID_CENTRO;
                            _EstudioActual.ID_ESTUDIO = item.ID_ESTUDIO;
                            _EstudioActual.ID_IMPUTADO = item.ID_IMPUTADO;
                            _EstudioActual.ID_INGRESO = item.ID_INGRESO;
                            _EstudioActual.ID_MOTIVO = item.ID_MOTIVO;
                            _EstudioActual.ID_SITUACION = item.RESULT_ESTUDIO == "True" ? short.Parse((3).ToString()) : short.Parse((5).ToString());//estatus de asignado a estudio de personalidad
                            _EstudioActual.INICIO_FEC = item.INICIO_FEC;
                            _EstudioActual.SOLICITADO = item.SOLICITADO;
                            _EstudioActual.SOLICITUD_FEC = item.SOLICITUD_FEC;
                            _EstudioActual.TERMINO_FEC = item.TERMINO_FEC;
                            _EstudioActual.PROG_NOMBRE = item.PROG_NOMBRE;
                            _EstudioActual.NUM_OFICIO = item.NUM_OFICIO;
                            _EstudioActual.ID_AREA = item.ID_AREA;
                            Context.Entry(_EstudioActual).State = System.Data.EntityState.Modified;
                        };
                    }

                    var _MensajeConsiderado = Context.MENSAJE_TIPO.FirstOrDefault(x => x.ID_MEN_TIPO == TipoMensaje);
                    var _id_mensaje = GetIDProceso<int>("MENSAJE", "ID_MENSAJE", "1=1");
                    if (_MensajeConsiderado != null)
                        if (_id_mensaje != decimal.Zero)
                        {
                            System.DateTime _fecha = GetFechaServerDate();
                            var NvoMensaje = new MENSAJE()
                            {
                                CONTENIDO = _MensajeConsiderado.CONTENIDO + " PARA LOS IMPUTADOS \n " + CuerpoMensaje,
                                ENCABEZADO = _MensajeConsiderado.ENCABEZADO,
                                ID_MEN_TIPO = _MensajeConsiderado.ID_MEN_TIPO,
                                ID_MENSAJE = _id_mensaje,
                                REGISTRO_FEC = _fecha,
                                ID_CENTRO = GlobalVariables.gCentro
                            };

                            Context.MENSAJE.Add(NvoMensaje);
                        };

                    Context.SaveChanges();
                    transaccion.Complete();
                    return true;
                }

                return false;
            }

            catch (System.Data.Entity.Validation.DbEntityValidationException dbEx)
            {
                foreach (var validationErrors in dbEx.EntityValidationErrors)
                    foreach (var validationError in validationErrors.ValidationErrors)
                        System.Diagnostics.Trace.TraceInformation("Nombre del causante de excepción: {0} Error: {1}", validationError.PropertyName, validationError.ErrorMessage);

                return false;
            }

            catch (System.Exception exc)
            {
                return false;
            }
        }
        public bool Actualizar(PERSONALIDAD Entity, short TipoMensaje)
        {
            try
            {
                using (System.Transactions.TransactionScope transaccion = new System.Transactions.TransactionScope(System.Transactions.TransactionScopeOption.RequiresNew, new System.Transactions.TransactionOptions() { IsolationLevel = System.Transactions.IsolationLevel.ReadCommitted }))
                {
                    var _EstudioActual = Context.PERSONALIDAD.FirstOrDefault(x => x.ID_ESTUDIO == Entity.ID_ESTUDIO && x.ID_CENTRO == Entity.ID_CENTRO && x.ID_IMPUTADO == Entity.ID_IMPUTADO && x.ID_INGRESO == Entity.ID_INGRESO && x.ID_ANIO == Entity.ID_ANIO);
                    if (_EstudioActual != null)
                    {
                        //SE GENERA LA NOTIFICACION 
                        if (_EstudioActual.INICIO_FEC != Entity.INICIO_FEC)//LA FECHA CAMBIO, SE DEBE GENERAR UNA NOTIFICACION
                        {
                            System.DateTime _fecha = GetFechaServerDate();
                            var _MensajeConsiderado = Context.MENSAJE_TIPO.FirstOrDefault(x => x.ID_MEN_TIPO == TipoMensaje);
                            string _Nombre = string.Empty;
                            var _id_mensaje = GetIDProceso<int>("MENSAJE", "ID_MENSAJE", "1=1");
                            var NombreImputado = Context.IMPUTADO.First(w => w.ID_ANIO == _EstudioActual.ID_ANIO && w.ID_CENTRO == _EstudioActual.ID_CENTRO && w.ID_IMPUTADO == _EstudioActual.ID_IMPUTADO);
                            if (NombreImputado != null)
                                _Nombre = string.Format("{0} {1} {2}", !string.IsNullOrEmpty(NombreImputado.NOMBRE) ? NombreImputado.NOMBRE.Trim() : string.Empty,
                                                                       !string.IsNullOrEmpty(NombreImputado.PATERNO) ? NombreImputado.PATERNO.Trim() : string.Empty,
                                                                       !string.IsNullOrEmpty(NombreImputado.MATERNO) ? NombreImputado.MATERNO.Trim() : string.Empty);

                            if (_MensajeConsiderado != null)
                            {
                                if (_id_mensaje != decimal.Zero)
                                {
                                    var NvoMensaje = new MENSAJE()
                                    {
                                        CONTENIDO = _MensajeConsiderado.CONTENIDO + " PARA EL IMPUTADO \n " + " - " + _Nombre + " CON FECHA: " + (Entity.INICIO_FEC.HasValue ? Entity.INICIO_FEC.Value.ToString("dd/MM/yyyy") : string.Empty),
                                        ENCABEZADO = _MensajeConsiderado.ENCABEZADO,
                                        ID_MEN_TIPO = _MensajeConsiderado.ID_MEN_TIPO,
                                        ID_MENSAJE = _id_mensaje,
                                        REGISTRO_FEC = _fecha,
                                        ID_CENTRO = GlobalVariables.gCentro
                                    };

                                    Context.MENSAJE.Add(NvoMensaje);
                                };
                            };
                        };


                        _EstudioActual.ID_ANIO = Entity.ID_ANIO;
                        _EstudioActual.ID_CENTRO = Entity.ID_CENTRO;
                        _EstudioActual.ID_ESTUDIO = Entity.ID_ESTUDIO;
                        _EstudioActual.ID_IMPUTADO = Entity.ID_IMPUTADO;
                        _EstudioActual.ID_INGRESO = Entity.ID_INGRESO;
                        _EstudioActual.ID_MOTIVO = Entity.ID_MOTIVO;
                        _EstudioActual.ID_SITUACION = Entity.ID_SITUACION;
                        _EstudioActual.INICIO_FEC = Entity.INICIO_FEC;
                        _EstudioActual.SOLICITADO = Entity.SOLICITADO;
                        _EstudioActual.SOLICITUD_FEC = Entity.SOLICITUD_FEC;
                        _EstudioActual.TERMINO_FEC = Entity.TERMINO_FEC;
                        _EstudioActual.PROG_NOMBRE = Entity.PROG_NOMBRE;
                        _EstudioActual.NUM_OFICIO = Entity.NUM_OFICIO;
                    };

                    Context.SaveChanges();
                    transaccion.Complete();
                    return true;
                }

                return false;
            }
            catch (System.Exception ex)
            {
                return false;
                //throw new System.ApplicationException(ex.Message);
            }
        }

        public bool GuardaListaCandidatos(System.Collections.ObjectModel.ObservableCollection<PERSONALIDAD> lista, short? IdMotivoB, string OficioNumero, string NombrePrograma, short? Dias, bool FueroSeleccionadoLista, short TipoMensaje)
        {
            try
            {
                if (lista != null && lista.Any())
                {
                    using (TransactionScope transaccion = new TransactionScope(TransactionScopeOption.RequiresNew, new TransactionOptions() { IsolationLevel = System.Transactions.IsolationLevel.ReadCommitted }))
                    {
                        foreach (var item in lista)
                        {
                            var _EstudiosAnteriores = Context.PERSONALIDAD.Where(c => c.ID_INGRESO == item.ID_INGRESO && c.ID_IMPUTADO == item.ID_IMPUTADO && c.ID_ANIO == item.ID_ANIO && c.ID_CENTRO == item.ID_CENTRO);
                            if (_EstudiosAnteriores.Any())
                                foreach (var item2 in _EstudiosAnteriores)
                                {
                                    item2.ID_SITUACION = item2.ID_SITUACION == (short)eEstatudDetallePersonalidad.CANCELADO ? (short)eEstatudDetallePersonalidad.CANCELADO : (short)eEstatudDetallePersonalidad.TERMINADO;
                                    Context.Entry(item2).Property(x => x.ID_SITUACION).IsModified = true;
                                };

                            var _ValidacionIngreso = Context.INGRESO.FirstOrDefault(x => x.ID_CENTRO == item.ID_CENTRO && x.ID_IMPUTADO == item.ID_IMPUTADO && x.ID_INGRESO == item.ID_INGRESO && x.ID_ANIO == item.ID_ANIO);
                            short _ConsecutivoPersonalidad = GetIDProceso<short>("PERSONALIDAD", "ID_ESTUDIO", "1=1");
                            var NvoPersonalidad = new PERSONALIDAD()
                            {
                                ID_ANIO = item.ID_ANIO,
                                ID_CENTRO = item.ID_CENTRO,
                                ID_IMPUTADO = item.ID_IMPUTADO,
                                ID_INGRESO = item.ID_INGRESO,
                                ID_MOTIVO = IdMotivoB,
                                ID_SITUACION = (short)eEstatudDetallePersonalidad.PENDIENTE,
                                INICIO_FEC = item.INICIO_FEC,
                                SOLICITADO = item.SOLICITADO,
                                SOLICITUD_FEC = GetFechaServerDate(),
                                TERMINO_FEC = item.TERMINO_FEC,
                                PROG_NOMBRE = NombrePrograma,
                                NUM_OFICIO = OficioNumero,
                                PLAZO_DIAS = _ValidacionIngreso != null ? _ValidacionIngreso.CAUSA_PENAL.FirstOrDefault(c => c.CP_FUERO == "F" && c.ID_ESTATUS_CP == 1) != null ? Dias : new short?() : new short?(),
                                ID_ESTUDIO = _ConsecutivoPersonalidad
                            };

                            Context.PERSONALIDAD.Add(NvoPersonalidad);

                            #region se inicializa el control de las pruebas que se le van a realizar al interno
                            if (!FueroSeleccionadoLista) // fuero comun
                            {
                                short _ConsecutivoPersonalidadDetalle = GetIDProceso<short>("PERSONALIDAD_DETALLE", "ID_DETALLE", "1=1");
                                short[] _comunes = new[] { (short)eTiposEstudio.CRIMINOLOGICO, (short)eTiposEstudio.LABORAL, (short)eTiposEstudio.MEDICO, (short)eTiposEstudio.PEDAGOGIA, (short)eTiposEstudio.PSICOLOGIA, (short)eTiposEstudio.PSIQUIATRIA, (short)eTiposEstudio.SEGURIDAD, (short)eTiposEstudio.TRABAJO_SOCIAL };
                                foreach (short _com in _comunes)
                                {
                                    var _detallePersonalidad = new PERSONALIDAD_DETALLE()
                                    {
                                        DIAS_BONIFICADOS = null,
                                        ID_ANIO = item.ID_ANIO,
                                        ID_CENTRO = item.ID_CENTRO,
                                        ID_DETALLE = _ConsecutivoPersonalidadDetalle,
                                        ID_ESTATUS = (short)eEstatudDetallePersonalidad.PENDIENTE,
                                        ID_ESTUDIO = NvoPersonalidad.ID_ESTUDIO,
                                        ID_IMPUTADO = item.ID_IMPUTADO,
                                        ID_INGRESO = item.ID_INGRESO,
                                        ID_TIPO = _com,
                                        INICIO_FEC = null,
                                        RESULTADO = string.Empty,
                                        SOLICITUD_FEC = item.SOLICITUD_FEC,
                                        TERMINO_FEC = null,
                                        TIPO_MEDIA = string.Empty
                                    };

                                    Context.PERSONALIDAD_DETALLE.Add(_detallePersonalidad);
                                    _ConsecutivoPersonalidadDetalle++;
                                };
                            }

                            #region detalle federal
                            else
                            {
                                short _ConsecutivoPersonalidadDetalle = GetIDProceso<short>("PERSONALIDAD_DETALLE", "ID_DETALLE", "1=1");
                                short[] _federales = new[] { (short)eTiposEstudio.CRIMIN_FEDERAL, (short)eTiposEstudio.LABORAL_FEDERAL, (short)eTiposEstudio.MEDICA_FEDERAL, (short)eTiposEstudio.PEDAGOGICA_FEDERAL, (short)eTiposEstudio.PSICOLOGICA_FEDERAL, (short)eTiposEstudio.SEGURIDAD_FEDERAL, (short)eTiposEstudio.TRABAJO_SOCIAL_FEDERAL };
                                foreach (var _fed in _federales)
                                {
                                    var _detallePersonalidad = new PERSONALIDAD_DETALLE()
                                    {
                                        DIAS_BONIFICADOS = null,
                                        ID_ANIO = item.ID_ANIO,
                                        ID_CENTRO = item.ID_CENTRO,
                                        ID_DETALLE = _ConsecutivoPersonalidadDetalle,
                                        ID_ESTATUS = (short)eEstatudDetallePersonalidad.PENDIENTE,
                                        ID_ESTUDIO = NvoPersonalidad.ID_ESTUDIO,
                                        ID_IMPUTADO = item.ID_IMPUTADO,
                                        ID_INGRESO = item.ID_INGRESO,
                                        ID_TIPO = _fed,
                                        INICIO_FEC = null,
                                        RESULTADO = string.Empty,
                                        SOLICITUD_FEC = item.SOLICITUD_FEC,
                                        TERMINO_FEC = null,
                                        TIPO_MEDIA = string.Empty
                                    };

                                    Context.PERSONALIDAD_DETALLE.Add(_detallePersonalidad);
                                    _ConsecutivoPersonalidadDetalle++;
                                };
                            }

                            #endregion

                            #endregion
                        };

                        System.DateTime _fecha = GetFechaServerDate();
                        var _MensajeConsiderado = Context.MENSAJE_TIPO.FirstOrDefault(x => x.ID_MEN_TIPO == TipoMensaje);
                        string _Nombre = string.Empty;
                        var _id_mensaje = GetIDProceso<int>("MENSAJE", "ID_MENSAJE", "1=1");
                        string DetalleProgramados = string.Empty;
                        if (lista.Any() && lista != null)
                            foreach (var item in lista)
                                DetalleProgramados += string.Format("- {0} {1} {2}  ({3} / {4}) \n",
                                    item.INGRESO != null ? item.INGRESO.IMPUTADO != null ? !string.IsNullOrEmpty(item.INGRESO.IMPUTADO.NOMBRE) ? item.INGRESO.IMPUTADO.NOMBRE.Trim() : string.Empty : string.Empty : string.Empty,
                                    item.INGRESO != null ? item.INGRESO.IMPUTADO != null ? !string.IsNullOrEmpty(item.INGRESO.IMPUTADO.PATERNO) ? item.INGRESO.IMPUTADO.PATERNO.Trim() : string.Empty : string.Empty : string.Empty,
                                    item.INGRESO != null ? item.INGRESO.IMPUTADO != null ? !string.IsNullOrEmpty(item.INGRESO.IMPUTADO.MATERNO) ? item.INGRESO.IMPUTADO.MATERNO.Trim() : string.Empty : string.Empty : string.Empty,
                                    item.ID_ANIO.ToString(), item.ID_IMPUTADO.ToString());

                        if (_MensajeConsiderado != null)
                        {
                            if (_id_mensaje != decimal.Zero)
                            {
                                var NvoMensaje = new MENSAJE()
                                {
                                    CONTENIDO = string.Format("SE HAN PROGRAMADO ESTUDIOS DE PERSONALIDAD A LOS SIGUIENTES IMPUTADOS: \n {0}", DetalleProgramados),
                                    ENCABEZADO = _MensajeConsiderado.ENCABEZADO,
                                    ID_MEN_TIPO = _MensajeConsiderado.ID_MEN_TIPO,
                                    ID_MENSAJE = _id_mensaje,
                                    REGISTRO_FEC = _fecha,
                                    ID_CENTRO = GlobalVariables.gCentro
                                };

                                Context.MENSAJE.Add(NvoMensaje);
                            };
                        };

                        Context.SaveChanges();
                        transaccion.Complete();
                        return true;
                    }
                }
            }
            catch (System.Exception exc)
            {
                return false;
            }

            return false;
        }

        public bool InsertarEstudiosCerrados(System.Collections.ObjectModel.ObservableCollection<PERSONALIDAD> _Estudios)
        {
            if (_Estudios != null && _Estudios.Any())
            {
                using (TransactionScope transaccion = new TransactionScope(TransactionScopeOption.RequiresNew, new TransactionOptions() { IsolationLevel = System.Transactions.IsolationLevel.ReadCommitted }))
                {
                    foreach (var item in _Estudios)
                    {
                        var NvoPersonalidad = new PERSONALIDAD();
                        NvoPersonalidad.ID_ANIO = item.ID_ANIO;
                        NvoPersonalidad.ID_CENTRO = item.ID_CENTRO;
                        NvoPersonalidad.ID_IMPUTADO = item.ID_IMPUTADO;
                        NvoPersonalidad.ID_INGRESO = item.ID_INGRESO;
                        NvoPersonalidad.ID_MOTIVO = item.ID_MOTIVO;
                        NvoPersonalidad.ID_SITUACION = item.ID_SITUACION;
                        NvoPersonalidad.INICIO_FEC = item.INICIO_FEC;
                        NvoPersonalidad.RESULT_ESTUDIO = item.RESULT_ESTUDIO;
                        NvoPersonalidad.SOLICITADO = item.SOLICITADO;
                        NvoPersonalidad.SOLICITUD_FEC = item.SOLICITUD_FEC;
                        NvoPersonalidad.TERMINO_FEC = item.TERMINO_FEC;
                        NvoPersonalidad.PROG_NOMBRE = item.PROG_NOMBRE;
                        NvoPersonalidad.NUM_OFICIO = item.NUM_OFICIO;
                        NvoPersonalidad.NUM_OFICIO1 = item.NUM_OFICIO1;
                        NvoPersonalidad.ID_ESTUDIO = item.ID_ESTUDIO;
                        Context.Entry(NvoPersonalidad).State = System.Data.EntityState.Modified;
                    };

                    Context.SaveChanges();
                    transaccion.Complete();
                    return true;
                }
            }

            return false;
        }

        public IQueryable<PERSONALIDAD> BusquedaConParametros(System.DateTime? FechaInicio, System.DateTime? FechaFin, string Folio, short Centro)
        {
            try
            {
                var predicate = PredicateBuilder.True<PERSONALIDAD>();

                ///definicion de parametros base, con respecto a los estatus
                predicate = predicate.And(c => c.ID_SITUACION == (short)eEstatudDetallePersonalidad.PENDIENTE || c.ID_SITUACION == (short)eEstatudDetallePersonalidad.CANCELADO);//estudios que aun no han sido cerrados o cancelados
                predicate = predicate.And(z => z.RESULT_ESTUDIO == null);
                predicate = predicate.And(x => x.INGRESO.ID_UB_CENTRO == Centro);

                if (FechaInicio.HasValue)
                    predicate = predicate.And(w => System.Data.Objects.EntityFunctions.TruncateTime(w.SOLICITUD_FEC) >= FechaInicio);

                if (FechaFin.HasValue)
                    predicate = predicate.And(w => System.Data.Objects.EntityFunctions.TruncateTime(w.SOLICITUD_FEC) <= FechaFin);

                if (!string.IsNullOrEmpty(Folio))
                    predicate = predicate.And(w => w.NUM_OFICIO.Trim() == Folio);

                return GetData(predicate.Expand()).OrderBy(x => x.ID_ANIO);
            }
            catch (System.Exception)
            {

                throw;
            }
        }


        public IQueryable<PERSONALIDAD> BuscarEstudiosDictamenFinal(System.DateTime? FechaInicio, System.DateTime? FechaFin, string Folio, short Centro)
        {
            try
            {
                var predicate = PredicateBuilder.True<PERSONALIDAD>();

                ///definicion de parametros base, con respecto al estatus
                predicate = predicate.And(x => x.INGRESO.ID_UB_CENTRO == Centro);
                predicate = predicate.And(x => x.ID_SITUACION == (short)eEstatudDetallePersonalidad.TERMINADO);//estudios que ya fueron cerrados
                //predicate = predicate.And(x => !string.IsNullOrEmpty(x.RESULT_ESTUDIO));

                if (FechaInicio.HasValue)
                    predicate = predicate.And(w => System.Data.Objects.EntityFunctions.TruncateTime(w.TERMINO_FEC) >= FechaInicio);

                if (FechaFin.HasValue)
                    predicate = predicate.And(w => System.Data.Objects.EntityFunctions.TruncateTime(w.TERMINO_FEC) <= FechaFin);

                if (!string.IsNullOrEmpty(Folio))
                    predicate = predicate.And(w => w.NUM_OFICIO.Trim() == Folio);

                return GetData(predicate.Expand()).OrderBy(x => x.ID_ANIO);
            }
            catch (System.Exception)
            {

                throw;
            }
        }

        public enum eEstausCausaPenal
        {
            ACTIVA = 1
        };

        public IQueryable<PERSONALIDAD> BuscarEstudiosTPersonalidadTerminados(System.DateTime? FechaInicio, System.DateTime? FechaFin, string Folio, short Centro)
        {
            try
            {
                var predicate = PredicateBuilder.True<PERSONALIDAD>();

                ///definicion de parametros base, con respecto al estatus
                predicate = predicate.And(x => x.INGRESO.ID_UB_CENTRO == Centro);
                predicate = predicate.And(x => x.ID_SITUACION == (short)eEstatudDetallePersonalidad.TERMINADO);//estudios que ya fueron cerrados
                predicate = predicate.And(x => string.IsNullOrEmpty(x.RESULT_ESTUDIO));
                predicate = predicate.And(x => x.INGRESO != null);
                predicate = predicate.And(x => x.INGRESO.CAUSA_PENAL.FirstOrDefault(y => y.ID_ESTATUS_CP == (short)eEstausCausaPenal.ACTIVA && y.CP_FUERO == "C") != null);

                if (FechaInicio.HasValue)
                    predicate = predicate.And(w => System.Data.Objects.EntityFunctions.TruncateTime(w.SOLICITUD_FEC) >= FechaInicio);

                if (FechaFin.HasValue)
                    predicate = predicate.And(w => System.Data.Objects.EntityFunctions.TruncateTime(w.SOLICITUD_FEC) <= FechaFin);

                if (!string.IsNullOrEmpty(Folio))
                    predicate = predicate.And(w => w.NUM_OFICIO.Trim() == Folio);

                return GetData(predicate.Expand()).OrderBy(x => x.NUM_OFICIO);
            }
            catch (System.Exception)
            {

                throw;
            }
        }
    }
}