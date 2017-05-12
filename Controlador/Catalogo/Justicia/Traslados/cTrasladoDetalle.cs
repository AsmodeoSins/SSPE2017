using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SSP.Servidor;
using SSP.Modelo;
using LinqKit;
using System.Transactions;
using System.Data;
using System.Data.Objects;
using SSP.Controlador.Principales.Compartidos;
using SSP.Controlador.Catalogo.Justicia.Ingreso;
namespace SSP.Controlador.Catalogo.Justicia
{
    public class cTrasladoDetalle : EntityManagerServer<TRASLADO_DETALLE>
    {
        /// <summary>
        /// Obtiene todos los traslados individuales
        /// </summary>        
        /// <param name="id_centro_destino">Centro destino al cual se dirigen los traslados para filtrar los resultados</param>
        /// <param name="estatus">Lista de estatus de traslado individuales para filtrar los resultados</param>
        /// <returns></returns>
        public IQueryable<TRASLADO_DETALLE> ObtenerTodosDestino(short id_centro_destino, List<string> estatus)
        {
            try
            {
                var predicate = PredicateBuilder.True<TRASLADO_DETALLE>();
                var predicateOR = PredicateBuilder.False<TRASLADO_DETALLE>();
                predicate = predicate.And(w => w.TRASLADO.CENTRO_DESTINO == id_centro_destino);
                foreach (var item in estatus)
                    predicateOR = predicateOR.Or(w => w.ID_ESTATUS == item);
                predicate = predicate.And(predicateOR.Expand());
                return GetData(predicate.Expand());

            }
            catch (Exception ex)
            {
                throw new ApplicationException(ex.Message + " " + (ex.InnerException != null ? ex.InnerException.InnerException.Message : ""));
            }
        }


        public TRASLADO_DETALLE Obtener(short Id_Centro, short Id_Anio, int Id_Imputado, short Id_Ingreso, string Estatus)
        {
            try
            {
                return GetData(g =>
                    g.ID_CENTRO == Id_Centro &&
                    g.ID_ANIO == Id_Anio &&
                    g.ID_IMPUTADO == Id_Imputado &&
                    g.ID_INGRESO == Id_Ingreso &&
                    g.ID_ESTATUS == Estatus).FirstOrDefault();
            }
            catch (Exception ex)
            {
                throw new ApplicationException(ex.Message + " " + (ex.InnerException != null ? ex.InnerException.InnerException.Message : ""));
            }
        }

        public TRASLADO_DETALLE ObtenerTraslado(short Id_Centro, short Id_Anio, int Id_Imputado, short Id_Ingreso)
        {
            try
            {
                const string TRASLADO_EN_PROCESO = "EP";
                const string TRASLADO_PROGRAMADO = "PR";
                return GetData(g =>
                    g.ID_CENTRO == Id_Centro &&
                    g.ID_ANIO == Id_Anio &&
                    g.ID_IMPUTADO == Id_Imputado &&
                    g.ID_INGRESO == Id_Ingreso &&
                    (g.ID_ESTATUS == TRASLADO_EN_PROCESO || g.ID_ESTATUS == TRASLADO_PROGRAMADO)).FirstOrDefault();
            }
            catch (Exception ex)
            {
                throw new ApplicationException(ex.Message + " " + (ex.InnerException != null ? ex.InnerException.InnerException.Message : ""));
            }
        }


        public void IniciarTrasladoEnProceso(List<TRASLADO_DETALLE> ListaTraslados)
        {
            try
            {
                using (TransactionScope transaccion = new TransactionScope(TransactionScopeOption.RequiresNew, new TransactionOptions() { IsolationLevel = System.Transactions.IsolationLevel.ReadCommitted }))
                {
                    foreach (var traslado in ListaTraslados)
                    {
                        var traslado_detalle = Context.TRASLADO_DETALLE.Where(w =>
                            w.ID_CENTRO == traslado.ID_CENTRO &&
                            w.ID_ANIO == traslado.ID_ANIO &&
                            w.ID_IMPUTADO == traslado.ID_IMPUTADO &&
                            w.ID_INGRESO == traslado.ID_INGRESO &&
                            w.ID_TRASLADO == traslado.ID_TRASLADO).FirstOrDefault();
                        traslado_detalle.ID_ESTATUS = "EP";
                        Context.Entry(traslado_detalle).Property(x => x.ID_ESTATUS).IsModified = true;
                        Context.SaveChanges();
                    }
                    Context.SaveChanges();
                    transaccion.Complete();
                }
            }
            catch (Exception ex)
            {
                throw new ApplicationException(ex.Message + " " + (ex.InnerException != null ? ex.InnerException.InnerException.Message : ""));
            }
        }

        /// <summary>
        /// metodo que autoriza el traslado, genera el nuevo ingreso y agrega informacion pertinente a este nuevo ingreso
        /// <param name="_traslado_detalle">Lista de traslados_detalle a autorizar</param>
        /// <param name="area_traslado">id del area de recepcion de traslados para ubicar al imputado fisicamente para el control de internos</param>
        /// <param name="fecha">la fecha actual del servidor oracle</param>
        /// <param name="id_recepcion_traslado_edificio">id del edificio donde se reciben los traslados y se usa como estancia durante la clasificacion</param>
        /// </summary>
        public void AutorizarTraslado(List<TRASLADO_DETALLE> _traslado_detalle, DateTime fecha, short area_traslado, short id_recepcion_traslado_edificio, short tipo_atencion_medica, short tipo_atencion_dentista, string usuario, string estatus_pendiente_tv_cita_medica="PE")
        {
            try
            {
                using (TransactionScope transaccion = new TransactionScope(TransactionScopeOption.RequiresNew, new TransactionOptions() { IsolationLevel = System.Transactions.IsolationLevel.ReadCommitted }))
                {


                    foreach (var item in _traslado_detalle)
                    {
                        var _item_traslado_detalle = Context.TRASLADO_DETALLE.Where(w => w.ID_CENTRO == item.ID_CENTRO && w.ID_ANIO == item.ID_ANIO && w.ID_IMPUTADO == item.ID_IMPUTADO && w.ID_INGRESO == item.ID_INGRESO && w.ID_TRASLADO == item.ID_TRASLADO).First();
                        _item_traslado_detalle.ID_ESTATUS = "FI";
                        Context.Entry(_item_traslado_detalle).Property(x => x.ID_ESTATUS).IsModified = true;
                        Context.SaveChanges();
                        #region Ingreso
                        //ingreso original
                        var _ingreso = Context.INGRESO.First(w => w.ID_ANIO == item.ID_ANIO && w.ID_CENTRO == item.ID_CENTRO && w.ID_IMPUTADO == item.ID_IMPUTADO && w.ID_INGRESO == item.ID_INGRESO);
                        //traslado paquete
                        var _traslado = Context.TRASLADO.First(w => w.ID_TRASLADO == item.ID_TRASLADO);
                        //cama para clasificacion
                        var _cama=Context.CAMA.FirstOrDefault(w=>w.ID_CENTRO==_traslado.CENTRO_DESTINO && w.ID_EDIFICIO==id_recepcion_traslado_edificio && w.ESTATUS=="S");
                        if (_cama == null)
                            throw new Exception("Falta configurar las camas en salida del centro para traslados/Ya no existen camas disponibles en la salida del centro");
                        //ingreso nuevo
                        var _ingreso_nuevo = new INGRESO
                        {
                            A_DISPOSICION = _ingreso.A_DISPOSICION,
                            ANIO_GOBIERNO = _ingreso.ANIO_GOBIERNO,
                            AV_PREVIA = _ingreso.AV_PREVIA,
                            DOCINTERNACION_NUM_OFICIO = _traslado.OFICIO_AUTORIZACION,
                            DOCUMENTO_INTERNACION = _ingreso.DOCUMENTO_INTERNACION,
                            FEC_INGRESO_CERESO = _ingreso.FEC_INGRESO_CERESO,
                            FEC_REGISTRO = _ingreso.FEC_REGISTRO,
                            FOLIO_GOBIERNO = _ingreso.FOLIO_GOBIERNO,
                            ID_ANIO = item.ID_ANIO,
                            ID_AUTORIDAD_INTERNA = _ingreso.ID_AUTORIDAD_INTERNA,
                            ID_CENTRO = item.ID_CENTRO,
                            ID_CLASIFICACION_JURIDICA = _ingreso.ID_CLASIFICACION_JURIDICA,
                            ID_DISPOSICION = _ingreso.ID_DISPOSICION,
                            ID_ESTATUS_ADMINISTRATIVO = item.ID_ESTATUS_ADMINISTRATIVO,
                            ID_IMPUTADO = item.ID_IMPUTADO,
                            ID_IMPUTADO_EXPEDIENTE = _ingreso.ID_IMPUTADO_EXPEDIENTE,
                            ID_INGRESO = (short)(_ingreso.ID_INGRESO + 1),
                            ID_INGRESO_DELITO = _ingreso.ID_INGRESO_DELITO,
                            ID_TIPO_DOCUMENTO_INTERNACION = _ingreso.ID_TIPO_DOCUMENTO_INTERNACION,
                            ID_TIPO_INGRESO = (short)Context.PARAMETRO.First(w => w.ID_CLAVE == "TRASLADO_TIPO_INGRESO").VALOR_NUM.Value,
                            ID_TIPO_SEGURIDAD = _ingreso.ID_TIPO_SEGURIDAD,
                            ID_UB_CENTRO = _traslado.CENTRO_DESTINO,
                            ID_UB_CAMA=_cama.ID_CAMA,
                            ID_UB_CELDA=_cama.ID_CELDA,
                            ID_UB_EDIFICIO=_cama.ID_EDIFICIO,
                            ID_UB_SECTOR=_cama.ID_SECTOR,
                            NUC = _ingreso.NUC,
                            ID_FUERO=_ingreso.ID_FUERO,
                            ID_DELITO=_ingreso.ID_DELITO

                        };
                        Context.INGRESO.Add(_ingreso_nuevo);
                        _cama.ESTATUS="N";
                        Context.SaveChanges();
                        if (!_traslado.TRASLADO_DETALLE.Any(w => w.ID_ESTATUS == "EP" || w.ID_ESTATUS == "PR"))
                        {
                            _traslado.ID_ESTATUS = "FI";
                            Context.Entry(_traslado).State = EntityState.Modified;
                            Context.SaveChanges();
                        }

                        #endregion


                        #region biometrico
                        foreach (var item_biometrico in Context.INGRESO_BIOMETRICO.Where(w => w.ID_ANIO == item.ID_ANIO && w.ID_CENTRO == item.ID_CENTRO
                            && w.ID_IMPUTADO == item.ID_IMPUTADO && w.ID_INGRESO == item.ID_INGRESO))
                            Context.INGRESO_BIOMETRICO.Add(new INGRESO_BIOMETRICO
                            {
                                BIOMETRICO = item_biometrico.BIOMETRICO,
                                ID_ANIO = item_biometrico.ID_ANIO,
                                ID_CENTRO = item_biometrico.ID_CENTRO,
                                ID_FORMATO = item_biometrico.ID_FORMATO,
                                ID_IMPUTADO = item_biometrico.ID_IMPUTADO,
                                ID_INGRESO = _ingreso_nuevo.ID_INGRESO,
                                ID_TIPO_BIOMETRICO = item_biometrico.ID_TIPO_BIOMETRICO
                            });
                        #endregion
                        #region causas penales
                        Context.SaveChanges();
                        short _id_liberacion = 1;
                        foreach (var _causa_penal in _ingreso.CAUSA_PENAL)
                        {
                            var _id_causa_penal = GetIDProceso<short>("CAUSA_PENAL", "ID_CAUSA_PENAL", string.Format("ID_CENTRO={0} AND ID_ANIO={1} AND ID_IMPUTADO={2} AND ID_INGRESO={3}", _ingreso_nuevo.ID_CENTRO,
                                _ingreso_nuevo.ID_ANIO, _ingreso_nuevo.ID_IMPUTADO, _ingreso_nuevo.ID_INGRESO));

                            //Inicializacion de variables id's de listas en 1
                            short _causa_penal_delito_id_cons = 1;

                            short _id_coparticipe = 1;
                            short _id_coparticipe_apodo = 1;
                            short _id_coparticipe_alias = 1;

                            short _id_recurso = 1;
                            short _recurso_id_docto = 1;

                            short _amparo_incidente_id_docto = 1;
                            short _id_amparo_incidente = 1;

                            short _amparo_directo_id_docto = 1;
                            short _id_amparo_directo = 1;

                            short _amparo_indirecto_id_docto = 1;
                            short _id_amparo_indirecto = 1;

                            short _causa_penal_id_docto = 1;

                            short _id_sentencia = 1;
                            short _sentencia_id_cons = 1;



                            #region causa penal delito
                            var _causa_penal_delitos = new List<CAUSA_PENAL_DELITO>();
                            foreach (var _causa_penal_delito in _causa_penal.CAUSA_PENAL_DELITO)
                            {
                                _causa_penal_delitos.Add(new CAUSA_PENAL_DELITO
                                {
                                    CANTIDAD = _causa_penal_delito.CANTIDAD,
                                    DESCR_DELITO = _causa_penal_delito.DESCR_DELITO,
                                    ID_ANIO = _causa_penal_delito.ID_ANIO,
                                    ID_CAUSA_PENAL = _id_causa_penal,
                                    ID_CENTRO = _causa_penal_delito.ID_CENTRO,
                                    ID_CONS = _causa_penal_delito_id_cons,
                                    ID_DELITO = _causa_penal_delito.ID_DELITO,
                                    ID_FUERO = _causa_penal_delito.ID_FUERO,
                                    ID_IMPUTADO = _causa_penal_delito.ID_IMPUTADO,
                                    ID_INGRESO = _ingreso_nuevo.ID_INGRESO,
                                    ID_MODALIDAD = _causa_penal_delito.ID_MODALIDAD,
                                    ID_TIPO_DELITO = _causa_penal_delito.ID_TIPO_DELITO,
                                    OBJETO = _causa_penal_delito.OBJETO
                                });
                                _causa_penal_delito_id_cons += 1;
                            }

                            #endregion
                            #region coparticipe
                            var _coparticipes = new List<COPARTICIPE>();
                            foreach (var _coparticipe in _causa_penal.COPARTICIPE)
                            {
                                var _coparticipe_apodos = new List<COPARTICIPE_APODO>();
                                foreach (var _coparticipe_apodo in _coparticipe.COPARTICIPE_APODO)
                                {
                                    _coparticipe_apodos.Add(new COPARTICIPE_APODO
                                    {
                                        APODO = _coparticipe_apodo.APODO,
                                        ID_ANIO = _coparticipe_apodo.ID_ANIO,
                                        ID_CAUSA_PENAL = _id_causa_penal,
                                        ID_CENTRO = _ingreso_nuevo.ID_CENTRO,
                                        ID_COPARTICIPE = _id_coparticipe,
                                        ID_COPARTICIPE_APODO = _id_coparticipe_apodo,
                                        ID_IMPUTADO = _coparticipe_apodo.ID_IMPUTADO,
                                        ID_INGRESO = _ingreso_nuevo.ID_INGRESO
                                    });
                                    _id_coparticipe_apodo += 1;
                                }

                                var _coparticipe_aliases = new List<COPARTICIPE_ALIAS>();
                                foreach (var _coparticipe_alias in _coparticipe.COPARTICIPE_ALIAS)
                                {
                                    _coparticipe_aliases.Add(new COPARTICIPE_ALIAS
                                    {
                                        ID_ANIO = _coparticipe_alias.ID_ANIO,
                                        ID_CAUSA_PENAL = _id_causa_penal,
                                        ID_CENTRO = _coparticipe_alias.ID_CENTRO,
                                        ID_COPARTICIPE = _id_coparticipe,
                                        ID_COPARTICIPE_ALIAS = _id_coparticipe_alias,
                                        ID_IMPUTADO = _coparticipe_alias.ID_IMPUTADO,
                                        ID_INGRESO = _ingreso_nuevo.ID_INGRESO,
                                        MATERNO = _coparticipe_alias.MATERNO,
                                        NOMBRE = _coparticipe_alias.NOMBRE,
                                        PATERNO = _coparticipe_alias.PATERNO
                                    });
                                    _id_coparticipe_alias += 1;
                                }

                                _coparticipes.Add(new COPARTICIPE
                                {
                                    COPARTICIPE_ALIAS = _coparticipe_aliases,
                                    COPARTICIPE_APODO = _coparticipe_apodos,
                                    ID_ANIO = _coparticipe.ID_ANIO,
                                    ID_CAUSA_PENAL = _id_causa_penal,
                                    ID_CENTRO = _coparticipe.ID_CENTRO,
                                    ID_COPARTICIPE = _id_coparticipe,
                                    ID_IMPUTADO = _coparticipe.ID_IMPUTADO,
                                    ID_INGRESO = _ingreso_nuevo.ID_INGRESO,
                                    MATERNO = _coparticipe.MATERNO,
                                    NOMBRE = _coparticipe.NOMBRE,
                                    PATERNO = _coparticipe.PATERNO
                                });
                                _id_coparticipe += 1;
                            }
                            #endregion
                            #region recursos
                            var _recursos = new List<RECURSO>();
                            foreach (var _recurso in _causa_penal.RECURSO)
                            {
                                var _recurso_doctos = new List<RECURSO_DOCTO>();
                                foreach (var _recurso_docto in _recurso.RECURSO_DOCTO)
                                {
                                    _recurso_doctos.Add(new RECURSO_DOCTO
                                    {
                                        DESCR = _recurso_docto.DESCR,
                                        DIGITALIZACION_FEC = _recurso_docto.DIGITALIZACION_FEC,
                                        DOCUMENTO = _recurso_docto.DOCUMENTO,
                                        ID_ANIO = _recurso_docto.ID_ANIO,
                                        ID_CAUSA_PENAL = _id_causa_penal,
                                        ID_CENTRO = _recurso_docto.ID_CENTRO,
                                        ID_DOCTO = _recurso_id_docto,
                                        ID_IMPUTADO = _recurso_docto.ID_IMPUTADO,
                                        ID_INGRESO = _ingreso_nuevo.ID_INGRESO,
                                        ID_RECURSO = _id_recurso,
                                    });
                                    _recurso_id_docto += 1;
                                }
                                _recursos.Add(new RECURSO
                                {
                                    FEC_RECURSO = _recurso.FEC_RECURSO,
                                    FEC_RESOLUCION = _recurso.FEC_RESOLUCION,
                                    FUERO = _recurso.FUERO,
                                    ID_ANIO = _recurso.ID_ANIO,
                                    ID_CAUSA_PENAL = _id_causa_penal,
                                    ID_CENTRO = _recurso.ID_CENTRO,
                                    ID_IMPUTADO = _recurso.ID_IMPUTADO,
                                    ID_INGRESO = _ingreso_nuevo.ID_INGRESO,
                                    ID_RECURSO = _id_recurso,
                                    ID_TIPO_RECURSO = _recurso.ID_TIPO_RECURSO,
                                    ID_TRIBUNAL = _recurso.ID_TRIBUNAL,
                                    MULTA = _recurso.MULTA,
                                    MULTA_CONDICIONAL = _recurso.MULTA_CONDICIONAL,
                                    NO_OFICIO = _recurso.NO_OFICIO,
                                    RECURSO_DOCTO = _recurso_doctos,
                                    REPARACION_DANIO = _recurso.REPARACION_DANIO,
                                    RESOLUCION = _recurso.RESOLUCION,
                                    RESULTADO = _recurso.RESULTADO,
                                    SENTENCIA_ANIOS = _recurso.SENTENCIA_ANIOS,
                                    SENTENCIA_DIAS = _recurso.SENTENCIA_DIAS,
                                    SENTENCIA_MESES = _recurso.SENTENCIA_MESES,
                                    SUSTITUCION_PENA = _recurso.SUSTITUCION_PENA,
                                    TOCA_PENAL = _recurso.TOCA_PENAL
                                });
                                _id_recurso += 1;
                            }
                            #endregion
                            #region amparo_incidente
                            var _amparos_incidente = new List<AMPARO_INCIDENTE>();
                            foreach (var _amparo_incidente in _causa_penal.AMPARO_INCIDENTE)
                            {
                                var _amparo_incidente_doctos = new List<AMPARO_INCIDENTE_DOCTO>();
                                foreach (var _amparo_incidente_docto in _amparo_incidente.AMPARO_INCIDENTE_DOCTO)
                                {
                                    _amparo_incidente_doctos.Add(new AMPARO_INCIDENTE_DOCTO
                                    {
                                        DESCR = _amparo_incidente_docto.DESCR,
                                        DIGITALIZACION_FEC = _amparo_incidente_docto.DIGITALIZACION_FEC,
                                        DOCUMENTO = _amparo_incidente_docto.DOCUMENTO,
                                        ID_AMPARO_INCIDENTE = _id_amparo_incidente,
                                        ID_ANIO = _amparo_incidente_docto.ID_ANIO,
                                        ID_CAUSA_PENAL = _id_causa_penal,
                                        ID_CENTRO = _amparo_incidente_docto.ID_CENTRO,
                                        ID_DOCTO = _amparo_incidente_id_docto,
                                        ID_IMPUTADO = _amparo_incidente_docto.ID_IMPUTADO,
                                        ID_INGRESO = _ingreso_nuevo.ID_INGRESO
                                    });
                                    _amparo_incidente_id_docto += 1;
                                }
                                _amparos_incidente.Add(new AMPARO_INCIDENTE
                                {
                                    AMPARO_INCIDENTE_DOCTO = _amparo_incidente_doctos,
                                    AUTORIDAD_NOTIFICA = _amparo_incidente.AUTORIDAD_NOTIFICA,
                                    CAPTURA_FEC = _amparo_incidente.CAPTURA_FEC,
                                    DIAS_REMISION = _amparo_incidente.DIAS_REMISION,
                                    DOCUMENTO_FEC = _amparo_incidente.DOCUMENTO_FEC,
                                    GARANTIA = _amparo_incidente.GARANTIA,
                                    ID_AMP_INC_TIPO = _amparo_incidente.ID_AMP_INC_TIPO,
                                    ID_AMPARO_INCIDENTE = _amparo_incidente.ID_AMPARO_INCIDENTE,
                                    ID_ANIO = _amparo_incidente.ID_ANIO,
                                    ID_CAUSA_PENAL = _id_causa_penal,
                                    ID_CENTRO = _amparo_incidente.ID_CENTRO,
                                    ID_IMPUTADO = _amparo_incidente.ID_IMPUTADO,
                                    ID_INGRESO = _ingreso_nuevo.ID_INGRESO,
                                    OFICIO_NUM = _amparo_incidente.OFICIO_NUM,
                                    RESULTADO = _amparo_incidente.RESULTADO,
                                    MODIFICA_PENA_ANIO = _amparo_incidente.MODIFICA_PENA_ANIO,
                                    MODIFICA_PENA_DIA = _amparo_incidente.MODIFICA_PENA_DIA,
                                    MODIFICA_PENA_MES = _amparo_incidente.MODIFICA_PENA_MES
                                });
                                _id_amparo_incidente += 1;
                            }
                            #endregion
                            #region amparo directo
                            var _amparos_directo = new List<AMPARO_DIRECTO>();
                            foreach (var _amparo_directo in _causa_penal.AMPARO_DIRECTO)
                            {
                                var _amparo_directo_doctos = new List<AMPARO_DIRECTO_DOCTO>();
                                foreach (var _amparo_directo_docto in _amparo_directo.AMPARO_DIRECTO_DOCTO)
                                {
                                    _amparo_directo_doctos.Add(new AMPARO_DIRECTO_DOCTO
                                    {
                                        DESCR = _amparo_directo_docto.DESCR,
                                        DIGITALIZACION_FEC = _amparo_directo_docto.DIGITALIZACION_FEC,
                                        DOCUMENTO = _amparo_directo_docto.DOCUMENTO,
                                        ID_AMPARO_DIRECTO = _id_amparo_directo,
                                        ID_ANIO = _amparo_directo_docto.ID_ANIO,
                                        ID_CAUSA_PENAL = _id_causa_penal,
                                        ID_CENTRO = _amparo_directo_docto.ID_CENTRO,
                                        ID_DOCTO = _amparo_directo_id_docto,
                                        ID_IMPUTADO = _amparo_directo_docto.ID_IMPUTADO,
                                        ID_INGRESO = _ingreso_nuevo.ID_INGRESO
                                    });
                                    _amparo_directo_id_docto += 1;
                                }
                                _amparos_directo.Add(new AMPARO_DIRECTO
                                {
                                    AMPARO_DIRECTO_DOCTO = _amparo_directo_doctos,
                                    AMPARO_NUM = _amparo_directo.AMPARO_NUM,
                                    AUTORIDAD_NOTIFICA = _amparo_directo.AUTORIDAD_NOTIFICA,
                                    AUTORIDAD_PRONUNCIA_SENTENCIA = _amparo_directo.AUTORIDAD_PRONUNCIA_SENTENCIA,
                                    DOCUMENTO_FEC = _amparo_directo.DOCUMENTO_FEC,
                                    ID_AMPARO_DIRECTO = _id_amparo_directo,
                                    ID_ANIO = _amparo_directo.ID_ANIO,
                                    ID_CAUSA_PENAL = _id_causa_penal,
                                    ID_CENTRO = _amparo_directo.ID_CENTRO,
                                    ID_IMPUTADO = _amparo_directo.ID_IMPUTADO,
                                    ID_INGRESO = _ingreso_nuevo.ID_INGRESO,
                                    ID_SEN_AMP_RESULTADO = _amparo_directo.ID_SEN_AMP_RESULTADO,
                                    NOTIFICACION_FEC = _amparo_directo.NOTIFICACION_FEC,
                                    OFICIO_NUM = _amparo_directo.OFICIO_NUM,
                                    RESUELVE_DOCUMENTO_FEC = _amparo_directo.RESUELVE_DOCUMENTO_FEC,
                                    RESUELVE_OFICIO_NUM = _amparo_directo.RESUELVE_OFICIO_NUM,
                                    RESUELVE_SENTENCIA_FEC = _amparo_directo.RESUELVE_SENTENCIA_FEC,
                                    SENTENCIA_AMP_AUTORIDAD = _amparo_directo.SENTENCIA_AMP_AUTORIDAD,
                                    SENTENCIA_FEC = _amparo_directo.SENTENCIA_FEC,
                                    SUSPENSION_AUT_INFORMA = _amparo_directo.SUSPENSION_AUT_INFORMA,
                                    SUSPENSION_FEC = _amparo_directo.SUSPENSION_FEC
                                });
                                _id_amparo_directo += 1;
                            }
                            #endregion
                            #region amparo indirecto
                            var _amparos_indirecto = new List<AMPARO_INDIRECTO>();
                            foreach (var _amparo_indirecto in _causa_penal.AMPARO_INDIRECTO)
                            {
                                var _amparo_indirecto_doctos = new List<AMPARO_INDIRECTO_DOCTO>();
                                foreach (var _amparo_indirecto_docto in _amparo_indirecto.AMPARO_INDIRECTO_DOCTO)
                                {
                                    _amparo_indirecto_doctos.Add(new AMPARO_INDIRECTO_DOCTO
                                    {
                                        DESCR = _amparo_indirecto_docto.DESCR,
                                        DIGITALIZACION_FEC = _amparo_indirecto_docto.DIGITALIZACION_FEC,
                                        DOCUMENTO = _amparo_indirecto_docto.DOCUMENTO,
                                        ID_AMPARO_INDIRECTO = _id_amparo_indirecto,
                                        ID_ANIO = _amparo_indirecto_docto.ID_ANIO,
                                        ID_CENTRO = _amparo_indirecto_docto.ID_CENTRO,
                                        ID_DOCTO = _amparo_indirecto_id_docto,
                                        ID_IMPUTADO = _amparo_indirecto_docto.ID_IMPUTADO,
                                        ID_INGRESO = _ingreso_nuevo.ID_INGRESO
                                    });
                                    _amparo_indirecto_id_docto += 1;
                                }
                                _amparos_indirecto.Add(new AMPARO_INDIRECTO
                                {
                                    ACTO_RECLAMADO = _amparo_indirecto.ACTO_RECLAMADO,
                                    AMPARO_INDIRECTO_DOCTO = _amparo_indirecto_doctos,
                                    AMPARO_NUM = _amparo_indirecto.AMPARO_NUM,
                                    DOCUMENTO_FEC = _amparo_indirecto.DOCUMENTO_FEC,
                                    ID_AMPARO_INDIRECTO = _id_amparo_indirecto,
                                    ID_ANIO = _amparo_indirecto.ID_ANIO,
                                    ID_CAUSA_PENAL = _id_causa_penal,
                                    ID_CENTRO = _amparo_indirecto.ID_CENTRO,
                                    ID_IMPUTADO = _amparo_indirecto.ID_IMPUTADO,
                                    ID_INGRESO = _ingreso_nuevo.ID_INGRESO,
                                    NOTIFICACION_FEC = _amparo_indirecto.NOTIFICACION_FEC,
                                    OFICIO_NUM = _amparo_indirecto.OFICIO_NUM,
                                    RESOLUCION_EJECUTORIA_FEC = _amparo_indirecto.RESOLUCION_EJECUTORIA_FEC,
                                    RESUELVE_AUTORIDAD = _amparo_indirecto.RESUELVE_AUTORIDAD,
                                    RESUELVE_DOCUMENTO_FEC = _amparo_indirecto.RESUELVE_DOCUMENTO_FEC,
                                    RESUELVE_OFICIO_NUM = _amparo_indirecto.RESUELVE_OFICIO_NUM,
                                    REVISION_RECURSO_FEC = _amparo_indirecto.REVISION_RECURSO_FEC,
                                    SENTENCIA_FEC = _amparo_indirecto.SENTENCIA_FEC,
                                    SUSPENSION_AUT_INFORMA = _amparo_indirecto.SUSPENSION_AUT_INFORMA,
                                    SUSPENSION_FEC = _amparo_indirecto.SUSPENSION_FEC
                                });
                                _id_amparo_indirecto += 1;
                            }
                            #endregion
                            #region causa penal docto
                            var _causa_penal_doctos = new List<CAUSA_PENAL_DOCTO>();
                            foreach (var _causa_penal_docto in _causa_penal.CAUSA_PENAL_DOCTO)
                                _causa_penal_doctos.Add(new CAUSA_PENAL_DOCTO
                                {
                                    DESCR = _causa_penal_docto.DESCR,
                                    DIGITALIZACION_FEC = _causa_penal_docto.DIGITALIZACION_FEC,
                                    DOCUMENTO = _causa_penal_docto.DOCUMENTO,
                                    ID_ANIO = _causa_penal_docto.ID_ANIO,
                                    ID_CAUSA_PENAL = _id_causa_penal,
                                    ID_CENTRO = _causa_penal_docto.ID_CENTRO,
                                    ID_DOCTO = _causa_penal_id_docto,
                                    ID_IMPUTADO = _causa_penal_docto.ID_IMPUTADO,
                                    ID_INGRESO = _ingreso_nuevo.ID_INGRESO
                                });
                            _causa_penal_id_docto += 1;
                            #endregion
                            #region sentencia
                            var _sentencias = new List<SENTENCIA>();
                            foreach (var _sentencia in _causa_penal.SENTENCIA)
                            {
                                var _sentencia_delitos = new List<SENTENCIA_DELITO>();
                                {
                                    foreach (var _sentencia_delito in _sentencia.SENTENCIA_DELITO)
                                    {
                                        _sentencia_delitos.Add(new SENTENCIA_DELITO
                                        {
                                            CANTIDAD = _sentencia_delito.CANTIDAD,
                                            DESCR_DELITO = _sentencia_delito.DESCR_DELITO,
                                            ID_ANIO = _sentencia_delito.ID_ANIO,
                                            ID_CAUSA_PENAL = _id_causa_penal,
                                            ID_CENTRO = _sentencia_delito.ID_CENTRO,
                                            ID_CONS = _sentencia_id_cons,
                                            ID_DELITO = _sentencia_delito.ID_DELITO,
                                            ID_FUERO = _sentencia_delito.ID_FUERO,
                                            ID_IMPUTADO = _sentencia_delito.ID_IMPUTADO,
                                            ID_INGRESO = _ingreso_nuevo.ID_INGRESO,
                                            ID_MODALIDAD = _sentencia_delito.ID_MODALIDAD,
                                            ID_SENTENCIA = _id_sentencia,
                                            ID_TIPO_DELITO = _sentencia_delito.ID_TIPO_DELITO,
                                            OBJETO = _sentencia_delito.OBJETO
                                        });
                                        _sentencia_id_cons += 1;
                                    }
                                }
                                _sentencias.Add(new SENTENCIA
                                {
                                    ANIOS = _sentencia.ANIOS,
                                    ANIOS_ABONADOS = _sentencia.ANIOS_ABONADOS,
                                    DIAS = _sentencia.DIAS,
                                    DIAS_ABONADOS = _sentencia.DIAS_ABONADOS,
                                    ESTATUS = _sentencia.ESTATUS,
                                    FEC_EJECUTORIA = _sentencia.FEC_EJECUTORIA,
                                    FEC_INICIO_COMPURGACION = _sentencia.FEC_INICIO_COMPURGACION,
                                    FEC_REAL_COMPURGACION = _sentencia.FEC_REAL_COMPURGACION,
                                    FEC_SENTENCIA = _sentencia.FEC_SENTENCIA,
                                    ID_ANIO = _sentencia.ID_ANIO,
                                    ID_CAUSA_PENAL = _id_causa_penal,
                                    ID_CENTRO = _sentencia.ID_CENTRO,
                                    ID_GRADO_AUTORIA = _sentencia.ID_GRADO_AUTORIA,
                                    ID_GRADO_PARTICIPACION = _sentencia.ID_GRADO_PARTICIPACION,
                                    ID_IMPUTADO = _sentencia.ID_IMPUTADO,
                                    ID_INGRESO = _ingreso_nuevo.ID_INGRESO,
                                    ID_SENTENCIA = _id_sentencia,
                                    MESES = _sentencia.MESES,
                                    MESES_ABONADOS = _sentencia.MESES_ABONADOS,
                                    MOTIVO_CANCELACION_ANTECEDENTE = _sentencia.MOTIVO_CANCELACION_ANTECEDENTE,
                                    MULTA = _sentencia.MULTA,
                                    MULTA_PAGADA = _sentencia.MULTA_PAGADA,
                                    OBSERVACION = _sentencia.OBSERVACION,
                                    REPARACION_DANIO = _sentencia.REPARACION_DANIO,
                                    REPARACION_DANIO_PAGADA = _sentencia.REPARACION_DANIO_PAGADA,
                                    SENTENCIA_DELITO = _sentencia_delitos,
                                    SUSPENSION_CONDICIONAL = _sentencia.SUSPENSION_CONDICIONAL,
                                    SUSTITUCION_PENA = _sentencia.SUSTITUCION_PENA,
                                    SUSTITUCION_PENA_PAGADA = _sentencia.SUSTITUCION_PENA_PAGADA
                                });
                                _id_sentencia += 1;
                            }
                            #endregion
                            #region liberacion
                            var _liberaciones = new List<LIBERACION>();
                            foreach (var _liberacion in _causa_penal.LIBERACION)
                            {
                                _liberaciones.Add(new LIBERACION
                                {
                                    ID_ANIO = _liberacion.ID_ANIO,
                                    ID_CAUSA_PENAL = _id_causa_penal,
                                    ID_CENTRO = _liberacion.ID_CENTRO,
                                    ID_IMPUTADO = _liberacion.ID_IMPUTADO,
                                    ID_INGRESO = _ingreso_nuevo.ID_INGRESO,
                                    ID_LIBERACION = _id_liberacion,
                                    ID_LIBERACION_AUTORIDAD = _liberacion.ID_LIBERACION_AUTORIDAD,
                                    ID_LIBERACION_MOTIVO = _liberacion.ID_LIBERACION_MOTIVO,
                                    INCIDENTE_BIOMETRICO = _liberacion.INCIDENTE_BIOMETRICO,
                                    LIBERACION_FEC = _liberacion.LIBERACION_FEC,
                                    LIBERACION_MOTIVO = _liberacion.LIBERACION_MOTIVO,
                                    LIBERACION_OFICIO = _liberacion.LIBERACION_OFICIO,
                                    LIBERADO = _liberacion.LIBERADO
                                });
                                _id_liberacion += 1;
                            }

                            #endregion
                            #region NUC
                            var _nuc = new NUC();
                            if (_causa_penal.NUC != null)
                            {
                                _nuc = new NUC
                                {
                                    DESCR = _causa_penal.NUC.DESCR,
                                    ID_ANIO = _ingreso_nuevo.ID_ANIO,
                                    ID_CAUSA_PENAL = _id_causa_penal,
                                    ID_CENTRO = _ingreso_nuevo.ID_CENTRO,
                                    ID_IMPUTADO = _ingreso_nuevo.ID_IMPUTADO,
                                    ID_INGRESO = _ingreso_nuevo.ID_INGRESO,
                                    ID_NUC = _causa_penal.NUC.ID_NUC,
                                    ID_PERSONA_PG = _causa_penal.NUC.ID_PERSONA_PG
                                };
                            }
                            else
                                _nuc = null;

                            #endregion
                            Context.CAUSA_PENAL.Add(new CAUSA_PENAL
                            {
                                ID_CAUSA_PENAL = _id_causa_penal,
                                AP_ANIO = _causa_penal.AP_ANIO,
                                AP_FEC_CONSIGNACION = _causa_penal.AP_FEC_CONSIGNACION,
                                AP_FEC_INICIO = _causa_penal.AP_FEC_INICIO,
                                AP_FOLIO = _causa_penal.AP_FOLIO,
                                AP_FORANEA = _causa_penal.AP_FORANEA,
                                CP_AMPLIACION = _causa_penal.CP_AMPLIACION,
                                CP_ANIO = _causa_penal.CP_ANIO,
                                CP_BIS = _causa_penal.CP_BIS,
                                CP_ESTADO_JUZGADO = _causa_penal.CP_ESTADO_JUZGADO,
                                CP_FEC_RADICACION = _causa_penal.CP_FEC_RADICACION,
                                CP_FEC_VENCIMIENTO_TERMINO = _causa_penal.CP_FEC_VENCIMIENTO_TERMINO,
                                CP_FOLIO = _causa_penal.CP_FOLIO,
                                CP_FORANEO = _causa_penal.CP_FORANEO,
                                CP_FUERO = _causa_penal.CP_FUERO,
                                CP_JUZGADO = _causa_penal.CP_JUZGADO,
                                CP_MUNICIPIO_JUZGADO = _causa_penal.CP_MUNICIPIO_JUZGADO,
                                CP_PAIS_JUZGADO = _causa_penal.CP_PAIS_JUZGADO,
                                CP_TERMINO = _causa_penal.CP_TERMINO,
                                CP_TIPO_ORDEN = _causa_penal.CP_TIPO_ORDEN,
                                ID_AGENCIA = _causa_penal.ID_AGENCIA,
                                ID_ANIO = _ingreso_nuevo.ID_ANIO,
                                ID_CENTRO = _ingreso_nuevo.ID_CENTRO,
                                ID_ENTIDAD = _causa_penal.ID_ENTIDAD,
                                ID_ESTATUS_CP = _causa_penal.ID_ESTATUS_CP,
                                ID_IMPUTADO = _ingreso_nuevo.ID_IMPUTADO,
                                ID_INGRESO = _ingreso_nuevo.ID_INGRESO,
                                ID_MUNICIPIO = _causa_penal.ID_MUNICIPIO,
                                OBSERV = _causa_penal.OBSERV,
                                CAUSA_PENAL_DELITO = _causa_penal_delitos,
                                COPARTICIPE = _coparticipes,
                                RECURSO = _recursos,
                                AMPARO_INCIDENTE = _amparos_incidente,
                                AMPARO_DIRECTO = _amparos_directo,
                                AMPARO_INDIRECTO = _amparos_indirecto,
                                CAUSA_PENAL_DOCTO = _causa_penal_doctos,
                                SENTENCIA = _sentencias,
                                LIBERACION = _liberaciones,
                                NUC = _nuc
                            });
                            Context.SaveChanges();
                        }
                        #endregion

                        #region grupo participante
                        //if (_ingreso.GRUPO_PARTICIPANTE != null && _ingreso.GRUPO_PARTICIPANTE.Count > 0)
                        //{
                        //    var _temp = new GRUPO_PARTICIPANTE[_ingreso.GRUPO_PARTICIPANTE.Count];
                        //    _ingreso.GRUPO_PARTICIPANTE.CopyTo(_temp, 0);
                        //    foreach (var _grupo_participante in _temp)
                        //    {
                        //        var total_asistencias = _grupo_participante.GRUPO_ASISTENCIA.Where(w => w.ID_GRUPO == _grupo_participante.ID_GRUPO && w.ASISTENCIA.HasValue && w.ASISTENCIA == 1 && w.ESTATUS == 1).Count() + _grupo_participante.GRUPO_ASISTENCIA.Where(w => w.ID_GRUPO == _grupo_participante.ID_GRUPO && !w.ASISTENCIA.HasValue && w.ESTATUS == 3).Count();
                        //        var total_horarios = _grupo_participante.GRUPO_ASISTENCIA.Where(w => w.ID_GRUPO == _grupo_participante.ID_GRUPO && (w.ASISTENCIA == 1 || w.ASISTENCIA == 3)).Count();
                        //        Context.PROGRAMA_HISTORICO.Add(new PROGRAMA_HISTORICO
                        //        {
                        //            ACTIVIDAD = _grupo_participante.ACTIVIDAD.DESCR,
                        //            APROBADO = _grupo_participante.NOTA_TECNICA.Any(a => a.ID_GRUPO == _grupo_participante.ID_GRUPO) ? _grupo_participante.NOTA_TECNICA.First().NOTA_TECNICA_ESTATUS.DESCR : "",
                        //            EJE = _grupo_participante.EJE1.DESCR,
                        //            ESTATUS = _grupo_participante.GRUPO_PARTICIPANTE_ESTATUS.DESCR,
                        //            NOTA = _grupo_participante.NOTA_TECNICA.Any(a => a.ID_GRUPO == _grupo_participante.ID_GRUPO) ? _grupo_participante.NOTA_TECNICA.First().NOTA : "",
                        //            PROGRAMA = _grupo_participante.ACTIVIDAD.TIPO_PROGRAMA.DESCR,
                        //            ASISTENCIA = String.Format("{0:0.00}", total_asistencias / total_horarios),
                        //            ID_CENTRO=_ingreso_nuevo.ID_CENTRO,
                        //            ID_TRASLADO=item.ID_TRASLADO
                        //        });
                        //        Context.SaveChanges();
                        //    }
                        //}

                        //if (_ingreso.GRUPO_PARTICIPANTE!=null && _ingreso.GRUPO_PARTICIPANTE.Count>0)
                        //{
                        //    var _temp = new GRUPO_PARTICIPANTE[_ingreso.GRUPO_PARTICIPANTE.Count];
                        //    _ingreso.GRUPO_PARTICIPANTE.CopyTo(_temp, 0);
                        //    foreach (var _grupo_participante in _temp)
                        //    {
                        //        var _id_consec = GetIDProceso<short>("GRUPO_PARTICIPANTE", "ID_CONSEC", string.Format("ID_CENTRO={0} AND ID_ACTIVIDAD={1} AND ID_TIPO_PROGRAMA={2}", _ingreso_nuevo.ID_UB_CENTRO,
                        //            _grupo_participante.ID_ACTIVIDAD, _grupo_participante.ID_TIPO_PROGRAMA));
                        //        short? _estatus_grupo = _grupo_participante.ESTATUS == 5 ? _grupo_participante.ESTATUS : 1;

                        //        Context.GRUPO_PARTICIPANTE.Add(new GRUPO_PARTICIPANTE
                        //        {
                        //            EJE = _grupo_participante.EJE,
                        //            EJE1 = _grupo_participante.EJE1,
                        //            ESTATUS = _estatus_grupo,
                        //            FEC_REGISTRO = _grupo_participante.FEC_REGISTRO,
                        //            ID_ACTIVIDAD = _grupo_participante.ID_ACTIVIDAD,
                        //            ID_CENTRO = _ingreso_nuevo.ID_UB_CENTRO.Value,
                        //            ID_CONSEC = _id_consec,
                        //            ID_TIPO_PROGRAMA = _grupo_participante.ID_TIPO_PROGRAMA,
                        //            ING_ID_ANIO = _grupo_participante.ING_ID_ANIO,
                        //            ING_ID_CENTRO = _grupo_participante.ING_ID_CENTRO,
                        //            ING_ID_IMPUTADO = _grupo_participante.ING_ID_IMPUTADO,
                        //            ING_ID_INGRESO = _ingreso_nuevo.ID_INGRESO,
                        //        });
                        //        Context.SaveChanges();
                        //    }
                        //}

                        #endregion
                        #region visitante_ingreso
                        foreach (var _visitante_ingreso in _ingreso.VISITANTE_INGRESO)
                        {
                            var _estatus_visita = (short?)Int16.Parse(Context.PARAMETRO.First(w => w.ID_CLAVE.Trim() == "ID_ESTATUS_VISITA_REGISTRO").VALOR);
                            if (_visitante_ingreso.ID_ESTATUS_VISITA == Int16.Parse(Context.PARAMETRO.First(w => w.ID_CLAVE.Trim() == "ID_ESTATUS_VISITA_CANCELADO").VALOR) ||
                                _visitante_ingreso.ID_ESTATUS_VISITA == Int16.Parse(Context.PARAMETRO.First(w => w.ID_CLAVE.Trim() == "ID_ESTATUS_VISITA_SUSPENDIDO").VALOR))
                                _estatus_visita = _visitante_ingreso.ID_ESTATUS_VISITA;
                            var _acompanantes = new List<ACOMPANANTE>();
                            foreach (var _acompanante in _visitante_ingreso.ACOMPANANTE)
                                _acompanantes.Add(new ACOMPANANTE
                                {
                                    ACO_ID_ANIO = _acompanante.ACO_ID_ANIO,
                                    ACO_ID_CENTRO = _acompanante.ACO_ID_CENTRO,
                                    ACO_ID_IMPUTADO = _acompanante.ACO_ID_IMPUTADO,
                                    ACO_ID_INGRESO = _ingreso_nuevo.ID_INGRESO,
                                    FEC_REGISTRO = _acompanante.FEC_REGISTRO,
                                    ID_ACOMPANANTE = _acompanante.ID_ACOMPANANTE,
                                    ID_ACOMPANANTE_RELACION = _acompanante.ID_ACOMPANANTE_RELACION,
                                    ID_VISITANTE = _acompanante.ID_VISITANTE,
                                    VIS_ID_ANIO = _acompanante.VIS_ID_ANIO,
                                    VIS_ID_CENTRO = _acompanante.VIS_ID_CENTRO,
                                    VIS_ID_IMPUTADO = _acompanante.VIS_ID_IMPUTADO,
                                    VIS_ID_INGRESO = _ingreso_nuevo.ID_INGRESO
                                });
                            Context.VISITANTE_INGRESO.Add(new VISITANTE_INGRESO
                            {
                                ACCESO_UNICO = _visitante_ingreso.ACCESO_UNICO,
                                ACOMPANANTE = _acompanantes,
                                EMISION_GAFETE = "N",
                                ESTATUS_MOTIVO = _visitante_ingreso.ESTATUS_MOTIVO,
                                FEC_ALTA = _visitante_ingreso.FEC_ALTA,
                                FEC_ULTIMA_MOD = _visitante_ingreso.FEC_ULTIMA_MOD,
                                ID_ANIO = _visitante_ingreso.ID_ANIO,
                                ID_CENTRO = _visitante_ingreso.ID_CENTRO,
                                ID_ESTATUS_VISITA = _estatus_visita,
                                ID_IMPUTADO = _visitante_ingreso.ID_IMPUTADO,
                                ID_INGRESO = _ingreso_nuevo.ID_INGRESO,
                                ID_PERSONA = _visitante_ingreso.ID_PERSONA,
                                ID_TIPO_REFERENCIA = _visitante_ingreso.ID_TIPO_REFERENCIA,
                                ID_TIPO_VISITANTE = _visitante_ingreso.ID_TIPO_VISITANTE,
                                OBSERVACION = _visitante_ingreso.OBSERVACION
                            });
                        }
                        #endregion
                        #region emi
                        var _emi = Context.EMI_INGRESO.Where(w => w.ID_CENTRO == _ingreso_nuevo.ID_CENTRO && w.ID_ANIO == _ingreso_nuevo.ID_ANIO && w.ID_IMPUTADO == _ingreso_nuevo.ID_IMPUTADO
                            && w.ID_INGRESO < _ingreso_nuevo.ID_INGRESO).OrderByDescending(w => w.ID_EMI).ThenBy(w => w.ID_EMI_CONS).FirstOrDefault();
                        if (_emi != null)
                        {
                            _emi.EMI.ESTATUS = "C";
                            Context.Entry(_emi.EMI).Property(x => x.ESTATUS).IsModified = true;
                        }
                        #endregion
                        #region estudios
                        #region estudiosocioeconomico
                        if (_ingreso.SOCIOECONOMICO != null)
                        {
                            SOCIOE_GPOFAMPRI _socioe_gpofampri = null;
                            if (_ingreso.SOCIOECONOMICO.SOCIOE_GPOFAMPRI != null)
                            {
                                var _gpofampri_carac = new List<SOCIOE_GPOFAMPRI_CARAC>();
                                foreach (var item_gpofampri_carac in _ingreso.SOCIOECONOMICO.SOCIOE_GPOFAMPRI.SOCIOE_GPOFAMPRI_CARAC)
                                {
                                    _gpofampri_carac.Add(new SOCIOE_GPOFAMPRI_CARAC
                                    {
                                        ID_ANIO = _ingreso_nuevo.ID_ANIO,
                                        ID_CENTRO = _ingreso_nuevo.ID_CENTRO,
                                        ID_CLAVE = item_gpofampri_carac.ID_CLAVE,
                                        ID_IMPUTADO = _ingreso_nuevo.ID_IMPUTADO,
                                        ID_INGRESO = _ingreso_nuevo.ID_INGRESO,
                                        ID_TIPO = item_gpofampri_carac.ID_TIPO,
                                        REGISTRO_FEC = item_gpofampri_carac.REGISTRO_FEC
                                    });
                                }
                                _socioe_gpofampri = new SOCIOE_GPOFAMPRI
                                {
                                    ANTECEDENTE = _ingreso.SOCIOECONOMICO.SOCIOE_GPOFAMPRI.ANTECEDENTE,
                                    EGRESO_MENSUAL = _ingreso.SOCIOECONOMICO.SOCIOE_GPOFAMPRI.EGRESO_MENSUAL,
                                    FAM_ANTECEDENTE = _ingreso.SOCIOECONOMICO.SOCIOE_GPOFAMPRI.FAM_ANTECEDENTE,
                                    GRUPO_FAMILIAR = _ingreso.SOCIOECONOMICO.SOCIOE_GPOFAMPRI.GRUPO_FAMILIAR,
                                    ID_ANIO = _ingreso_nuevo.ID_ANIO,
                                    ID_CENTRO = _ingreso_nuevo.ID_CENTRO,
                                    ID_IMPUTADO = _ingreso_nuevo.ID_IMPUTADO,
                                    ID_INGRESO = _ingreso_nuevo.ID_INGRESO,
                                    INGRESO_MENSUAL = _ingreso.SOCIOECONOMICO.SOCIOE_GPOFAMPRI.INGRESO_MENSUAL,
                                    NIVEL_SOCIO_CULTURAL = _ingreso.SOCIOECONOMICO.SOCIOE_GPOFAMPRI.NIVEL_SOCIO_CULTURAL,
                                    PERSONAS_LABORAN = _ingreso.SOCIOECONOMICO.SOCIOE_GPOFAMPRI.PERSONAS_LABORAN,
                                    PERSONAS_VIVEN_HOGAR = _ingreso.SOCIOECONOMICO.SOCIOE_GPOFAMPRI.PERSONAS_VIVEN_HOGAR,
                                    RELACION_INTRAFAMILIAR = _ingreso.SOCIOECONOMICO.SOCIOE_GPOFAMPRI.RELACION_INTRAFAMILIAR,
                                    SOCIOE_GPOFAMPRI_CARAC = _gpofampri_carac,
                                    VIVIENDA_CONDICIONES = _ingreso.SOCIOECONOMICO.SOCIOE_GPOFAMPRI.VIVIENDA_CONDICIONES,
                                    VIVIENDA_ZONA = _ingreso.SOCIOECONOMICO.SOCIOE_GPOFAMPRI.VIVIENDA_ZONA
                                };
                            }
                            SOCIOE_GPOFAMSEC _socioe_gpofamsec = null;
                            if (_ingreso.SOCIOECONOMICO.SOCIOE_GPOFAMPRI != null)
                            {
                                var _gpofamsec_carac = new List<SOCIOE_GPOFAMSEC_CARAC>();
                                foreach (var item_gpofamsec_carac in _ingreso.SOCIOECONOMICO.SOCIOE_GPOFAMSEC.SOCIOE_GPOFAMSEC_CARAC)
                                {
                                    _gpofamsec_carac.Add(new SOCIOE_GPOFAMSEC_CARAC
                                    {
                                        ID_ANIO = _ingreso_nuevo.ID_ANIO,
                                        ID_CENTRO = _ingreso_nuevo.ID_CENTRO,
                                        ID_CLAVE = item_gpofamsec_carac.ID_CLAVE,
                                        ID_IMPUTADO = _ingreso_nuevo.ID_IMPUTADO,
                                        ID_INGRESO = _ingreso_nuevo.ID_INGRESO,
                                        ID_TIPO = item_gpofamsec_carac.ID_TIPO,
                                        REGISTRO_FEC = item_gpofamsec_carac.REGISTRO_FEC
                                    });
                                }
                                _socioe_gpofamsec = new SOCIOE_GPOFAMSEC
                                {
                                    ANTECEDENTE = _ingreso.SOCIOECONOMICO.SOCIOE_GPOFAMSEC.ANTECEDENTE,
                                    APOYO_ECONOMICO = _ingreso.SOCIOECONOMICO.SOCIOE_GPOFAMSEC.APOYO_ECONOMICO,
                                    EGRESO_MENSUAL = _ingreso.SOCIOECONOMICO.SOCIOE_GPOFAMSEC.EGRESO_MENSUAL,
                                    FAM_ANTECEDENTE = _ingreso.SOCIOECONOMICO.SOCIOE_GPOFAMSEC.FAM_ANTECEDENTE,
                                    FRECUENCIA = _ingreso.SOCIOECONOMICO.SOCIOE_GPOFAMSEC.FRECUENCIA,
                                    GRUPO_FAMILIAR = _ingreso.SOCIOECONOMICO.SOCIOE_GPOFAMSEC.GRUPO_FAMILIAR,
                                    ID_ANIO = _ingreso_nuevo.ID_ANIO,
                                    ID_CENTRO = _ingreso_nuevo.ID_CENTRO,
                                    ID_IMPUTADO = _ingreso_nuevo.ID_IMPUTADO,
                                    ID_INGRESO = _ingreso_nuevo.ID_INGRESO,
                                    INGRESO_MENSUAL = _ingreso.SOCIOECONOMICO.SOCIOE_GPOFAMSEC.INGRESO_MENSUAL,
                                    MOTIVO_NO_VISITA = _ingreso.SOCIOECONOMICO.SOCIOE_GPOFAMSEC.MOTIVO_NO_VISITA,
                                    NIVEL_SOCIO_CULTURAL = _ingreso.SOCIOECONOMICO.SOCIOE_GPOFAMSEC.NIVEL_SOCIO_CULTURAL,
                                    PERSONAS_LABORAN = _ingreso.SOCIOECONOMICO.SOCIOE_GPOFAMSEC.PERSONAS_LABORAN,
                                    RECIBE_VISITA = _ingreso.SOCIOECONOMICO.SOCIOE_GPOFAMSEC.RECIBE_VISITA,
                                    RELACION_INTRAFAMILIAR = _ingreso.SOCIOECONOMICO.SOCIOE_GPOFAMSEC.RELACION_INTRAFAMILIAR,
                                    SOCIOE_GPOFAMSEC_CARAC = _gpofamsec_carac,
                                    VISITA = _ingreso.SOCIOECONOMICO.SOCIOE_GPOFAMSEC.VISITA,
                                    VIVIENDA_CONDICIONES = _ingreso.SOCIOECONOMICO.SOCIOE_GPOFAMSEC.VIVIENDA_CONDICIONES,
                                    VIVIENDA_ZONA = _ingreso.SOCIOECONOMICO.SOCIOE_GPOFAMSEC.VIVIENDA_ZONA
                                };
                            }
                            Context.SOCIOECONOMICO.Add(new SOCIOECONOMICO
                            {
                                DICTAMEN = _ingreso.SOCIOECONOMICO.DICTAMEN,
                                DICTAMEN_FEC = _ingreso.SOCIOECONOMICO.DICTAMEN_FEC,
                                ID_ANIO = _ingreso_nuevo.ID_ANIO,
                                ID_CENTRO = _ingreso_nuevo.ID_CENTRO,
                                ID_IMPUTADO = _ingreso_nuevo.ID_IMPUTADO,
                                ID_INGRESO = _ingreso_nuevo.ID_INGRESO,
                                SALARIO = _ingreso.SOCIOECONOMICO.SALARIO,
                                SOCIOE_GPOFAMPRI = _socioe_gpofampri,
                                SOCIOE_GPOFAMSEC = _socioe_gpofamsec
                            });
                        }
                        #endregion
                        #region ficha identificacion juridica
                        if (_ingreso.FICHA_IDENTIFICACION_JURIDICA != null)
                        {
                            var _ficha_identificacion_juridicaOld = _ingreso.FICHA_IDENTIFICACION_JURIDICA;
                            Context.FICHA_IDENTIFICACION_JURIDICA.Add(new FICHA_IDENTIFICACION_JURIDICA
                            {
                                DEPARTAMENTO_JURIDICO = _ficha_identificacion_juridicaOld.DEPARTAMENTO_JURIDICO,
                                ELABORO = _ficha_identificacion_juridicaOld.ELABORO,
                                FICHA_FEC = _ficha_identificacion_juridicaOld.FICHA_FEC,
                                ID_ANIO = _ingreso_nuevo.ID_ANIO,
                                ID_CENTRO = _ingreso_nuevo.ID_CENTRO,
                                ID_IMPUTADO = _ingreso_nuevo.ID_IMPUTADO,
                                ID_INGRESO = _ingreso_nuevo.ID_INGRESO,
                                JEFE_DEPARTAMENTO = _ficha_identificacion_juridicaOld.JEFE_DEPARTAMENTO,
                                OFICIO_ESTUDIO_SOLICITADO = _ficha_identificacion_juridicaOld.OFICIO_ESTUDIO_SOLICITADO,
                                P2_CLAS_JURID = _ficha_identificacion_juridicaOld.P2_CLAS_JURID,
                                P2_DELITO = _ficha_identificacion_juridicaOld.P2_DELITO,
                                P2_EJECUTORIA = _ficha_identificacion_juridicaOld.P2_EJECUTORIA,
                                P2_FEC_INGRESO = _ficha_identificacion_juridicaOld.P2_FEC_INGRESO,
                                P2_JUZGADOS = _ficha_identificacion_juridicaOld.P2_JUZGADOS,
                                P2_PARTIR = _ficha_identificacion_juridicaOld.P2_PARTIR,
                                P2_PENA_COMPURG = _ficha_identificacion_juridicaOld.P2_PENA_COMPURG,
                                P2_PROCEDENTE = _ficha_identificacion_juridicaOld.P2_PROCEDENTE,
                                P2_PROCESOS = _ficha_identificacion_juridicaOld.P2_PROCESOS,
                                P2_SENTENCIA = _ficha_identificacion_juridicaOld.P2_SENTENCIA,
                                P3_PROCESOS_PENDIENTES = _ficha_identificacion_juridicaOld.P3_PROCESOS_PENDIENTES,
                                P4_ULTIMO_EXAMEN_FEC = _ficha_identificacion_juridicaOld.P4_ULTIMO_EXAMEN_FEC,
                                P5_RESOLUCION_APLAZADO = _ficha_identificacion_juridicaOld.P5_RESOLUCION_APLAZADO,
                                P5_RESOLUCION_APROBADO = _ficha_identificacion_juridicaOld.P5_RESOLUCION_APROBADO,
                                P5_RESOLUCION_MAYORIA = _ficha_identificacion_juridicaOld.P5_RESOLUCION_MAYORIA,
                                P5_RESOLUCION_UNANIMIDAD = _ficha_identificacion_juridicaOld.P5_RESOLUCION_UNANIMIDAD,
                                P6_CRIMINODINAMIA = _ficha_identificacion_juridicaOld.P6_CRIMINODINAMIA,
                                P7_TRAMITE_LIBERTAD = _ficha_identificacion_juridicaOld.P7_TRAMITE_LIBERTAD,
                                P7_TRAMITE_MODIFICACION = _ficha_identificacion_juridicaOld.P7_TRAMITE_MODIFICACION,
                                P7_TRAMITE_TRASLADO = _ficha_identificacion_juridicaOld.P7_TRAMITE_TRASLADO,
                                TRAMITE_DIAGNOSTICO = _ficha_identificacion_juridicaOld.TRAMITE_DIAGNOSTICO,
                                TRAMITE_TRASLADO_VOLUNTARIO = _ficha_identificacion_juridicaOld.TRAMITE_TRASLADO_VOLUNTARIO
                            });
                        }
                        #endregion
                        if (_ingreso.PERSONALIDAD != null && _ingreso.PERSONALIDAD.Count > 0)
                        {
                            var _personalidadOld = _ingreso.PERSONALIDAD.OrderByDescending(w => w.ID_ESTUDIO).FirstOrDefault();
                            #region PFC
                            var _personalidad_fuero_comun = new PERSONALIDAD_FUERO_COMUN();
                            if (_personalidadOld.PERSONALIDAD_FUERO_COMUN != null)
                            {
                                var _pfc_vii_educativo = new PFC_VII_EDUCATIVO();
                                if (_personalidadOld.PERSONALIDAD_FUERO_COMUN.PFC_VII_EDUCATIVO != null)
                                {
                                    var pfc_actividades = new List<PFC_VII_ACTIVIDAD>();
                                    short _temp_consec = 1;
                                    foreach (var item_actividad in _personalidadOld.PERSONALIDAD_FUERO_COMUN.PFC_VII_EDUCATIVO.PFC_VII_ACTIVIDAD)
                                    {
                                        pfc_actividades.Add(new PFC_VII_ACTIVIDAD
                                        {
                                            ACTIVIDAD = item_actividad.ACTIVIDAD,
                                            DURACION = item_actividad.DURACION,
                                            ID_ANIO = _ingreso_nuevo.ID_ANIO,
                                            ID_CENTRO = _ingreso_nuevo.ID_CENTRO,
                                            ID_ESTUDIO = 1,
                                            ID_IMPUTADO = _ingreso_nuevo.ID_IMPUTADO,
                                            ID_INGRESO = _ingreso_nuevo.ID_INGRESO,
                                            ID_PROGRAMA = item_actividad.ID_PROGRAMA,
                                            OBSERVACION = item_actividad.OBSERVACION,
                                            TIPO = item_actividad.TIPO,
                                            ID_CONSEC = _temp_consec,
                                            ID_ACTIVIDAD=item_actividad.ID_ACTIVIDAD
                                        });
                                        _temp_consec += 1;
                                    }
                                        
                                    var _pfc_vii_escolaridad_anteriores = new List<PFC_VII_ESCOLARIDAD_ANTERIOR>();
                                    _temp_consec = 1;
                                    foreach (var item_escolaridad_anterior in _personalidadOld.PERSONALIDAD_FUERO_COMUN.PFC_VII_EDUCATIVO.PFC_VII_ESCOLARIDAD_ANTERIOR)
                                    {
                                        _pfc_vii_escolaridad_anteriores.Add(new PFC_VII_ESCOLARIDAD_ANTERIOR
                                        {
                                            
                                            CONCLUIDA = item_escolaridad_anterior.CONCLUIDA,
                                            ID_ANIO = _ingreso_nuevo.ID_ANIO,
                                            ID_CENTRO = _ingreso_nuevo.ID_CENTRO,
                                            ID_ESTUDIO = 1,
                                            ID_GRADO = item_escolaridad_anterior.ID_GRADO,
                                            ID_IMPUTADO = _ingreso_nuevo.ID_IMPUTADO,
                                            ID_INGRESO = _ingreso_nuevo.ID_INGRESO,
                                            INTERES = item_escolaridad_anterior.INTERES,
                                            OBSERVACION = item_escolaridad_anterior.OBSERVACION,
                                            RENDIMIENTO = item_escolaridad_anterior.RENDIMIENTO,
                                            ID_CONSEC = _temp_consec
                                        });
                                        _temp_consec += 1;
                                    }
                                    _pfc_vii_educativo.COORDINADOR = _personalidadOld.PERSONALIDAD_FUERO_COMUN.PFC_VII_EDUCATIVO.COORDINADOR;
                                    _pfc_vii_educativo.ELABORO = _personalidadOld.PERSONALIDAD_FUERO_COMUN.PFC_VII_EDUCATIVO.ELABORO;
                                    _pfc_vii_educativo.ESTUDIO_FEC = _personalidadOld.PERSONALIDAD_FUERO_COMUN.PFC_VII_EDUCATIVO.ESTUDIO_FEC;
                                    _pfc_vii_educativo.ID_ANIO = _ingreso_nuevo.ID_ANIO;
                                    _pfc_vii_educativo.ID_CENTRO = _ingreso_nuevo.ID_CENTRO;
                                    _pfc_vii_educativo.ID_ESTUDIO = 1;
                                    _pfc_vii_educativo.ID_IMPUTADO = _ingreso_nuevo.ID_IMPUTADO;
                                    _pfc_vii_educativo.ID_INGRESO = _ingreso_nuevo.ID_INGRESO;
                                    _pfc_vii_educativo.P3_DICTAMEN = _personalidadOld.PERSONALIDAD_FUERO_COMUN.PFC_VII_EDUCATIVO.P3_DICTAMEN;
                                    _pfc_vii_educativo.P4_MOTIVACION_DICTAMEN = _personalidadOld.PERSONALIDAD_FUERO_COMUN.PFC_VII_EDUCATIVO.P4_MOTIVACION_DICTAMEN;
                                    _pfc_vii_educativo.PFC_VII_ACTIVIDAD = pfc_actividades;
                                    _pfc_vii_educativo.PFC_VII_ESCOLARIDAD_ANTERIOR = _pfc_vii_escolaridad_anteriores;
                                    _personalidad_fuero_comun.PFC_VII_EDUCATIVO = _pfc_vii_educativo;
                                }
                                var _pfc_ii_medico = new PFC_II_MEDICO();
                                if (_personalidadOld.PERSONALIDAD_FUERO_COMUN.PFC_II_MEDICO != null)
                                {
                                    _pfc_ii_medico.COORDINADOR = _personalidadOld.PERSONALIDAD_FUERO_COMUN.PFC_II_MEDICO.COORDINADOR;
                                    _pfc_ii_medico.ELABORO = _personalidadOld.PERSONALIDAD_FUERO_COMUN.PFC_II_MEDICO.ELABORO;
                                    _pfc_ii_medico.ESTUDIO_FEC = _personalidadOld.PERSONALIDAD_FUERO_COMUN.PFC_II_MEDICO.ESTUDIO_FEC;
                                    _pfc_ii_medico.ID_ANIO = _ingreso_nuevo.ID_ANIO;
                                    _pfc_ii_medico.ID_CENTRO = _ingreso_nuevo.ID_CENTRO;
                                    _pfc_ii_medico.ID_ESTUDIO = 1;
                                    _pfc_ii_medico.ID_IMPUTADO = _ingreso_nuevo.ID_IMPUTADO;
                                    _pfc_ii_medico.ID_INGRESO = _ingreso_nuevo.ID_INGRESO;
                                    _pfc_ii_medico.P2_HEREDO_FAMILIARES = _personalidadOld.PERSONALIDAD_FUERO_COMUN.PFC_II_MEDICO.P2_HEREDO_FAMILIARES;
                                    _pfc_ii_medico.P3_ANTPER_NOPATO = _personalidadOld.PERSONALIDAD_FUERO_COMUN.PFC_II_MEDICO.P3_ANTPER_NOPATO;
                                    _pfc_ii_medico.P31_CONSUMO_TOXICO = _personalidadOld.PERSONALIDAD_FUERO_COMUN.PFC_II_MEDICO.P31_CONSUMO_TOXICO;
                                    _pfc_ii_medico.P32_TATUAJES_CICATRICES = _personalidadOld.PERSONALIDAD_FUERO_COMUN.PFC_II_MEDICO.P32_TATUAJES_CICATRICES;
                                    _pfc_ii_medico.P4_PATOLOGICOS = _personalidadOld.PERSONALIDAD_FUERO_COMUN.PFC_II_MEDICO.P4_PATOLOGICOS;
                                    _pfc_ii_medico.P5_PADECIMIENTOS = _personalidadOld.PERSONALIDAD_FUERO_COMUN.PFC_II_MEDICO.P5_PADECIMIENTOS;
                                    _pfc_ii_medico.P6_EXPLORACION_FISICA = _personalidadOld.PERSONALIDAD_FUERO_COMUN.PFC_II_MEDICO.P6_EXPLORACION_FISICA;
                                    _pfc_ii_medico.P7_IMPRESION_DIAGNOSTICA = _personalidadOld.PERSONALIDAD_FUERO_COMUN.PFC_II_MEDICO.P7_IMPRESION_DIAGNOSTICA;
                                    _pfc_ii_medico.P8_DICTAMEN_MEDICO = _personalidadOld.PERSONALIDAD_FUERO_COMUN.PFC_II_MEDICO.P8_DICTAMEN_MEDICO;
                                    _pfc_ii_medico.SIGNOS_ESTATURA = _personalidadOld.PERSONALIDAD_FUERO_COMUN.PFC_II_MEDICO.SIGNOS_ESTATURA;
                                    _pfc_ii_medico.SIGNOS_PESO = _personalidadOld.PERSONALIDAD_FUERO_COMUN.PFC_II_MEDICO.SIGNOS_PESO;
                                    _pfc_ii_medico.SIGNOS_PULSO = _personalidadOld.PERSONALIDAD_FUERO_COMUN.PFC_II_MEDICO.SIGNOS_PULSO;
                                    _pfc_ii_medico.SIGNOS_RESPIRACION = _personalidadOld.PERSONALIDAD_FUERO_COMUN.PFC_II_MEDICO.SIGNOS_RESPIRACION;
                                    _pfc_ii_medico.SIGNOS_TA = _personalidadOld.PERSONALIDAD_FUERO_COMUN.PFC_II_MEDICO.SIGNOS_TA;
                                    _pfc_ii_medico.SIGNOS_TEMPERATURA = _personalidadOld.PERSONALIDAD_FUERO_COMUN.PFC_II_MEDICO.SIGNOS_TEMPERATURA;
                                    _personalidad_fuero_comun.PFC_II_MEDICO = _pfc_ii_medico;
                                }
                                var _pfc_iii_psiquiatrico = new PFC_III_PSIQUIATRICO();
                                if (_personalidadOld.PERSONALIDAD_FUERO_COMUN.PFC_III_PSIQUIATRICO != null)
                                {
                                    _pfc_iii_psiquiatrico.A1_ASPECTO_FISICO = _personalidadOld.PERSONALIDAD_FUERO_COMUN.PFC_III_PSIQUIATRICO.A1_ASPECTO_FISICO;
                                    _pfc_iii_psiquiatrico.A2_ESTADO_ANIMO = _personalidadOld.PERSONALIDAD_FUERO_COMUN.PFC_III_PSIQUIATRICO.A2_ESTADO_ANIMO;
                                    _pfc_iii_psiquiatrico.A3_ALUCINACIONES = _personalidadOld.PERSONALIDAD_FUERO_COMUN.PFC_III_PSIQUIATRICO.A3_ALUCINACIONES;
                                    _pfc_iii_psiquiatrico.A4_CURSO = _personalidadOld.PERSONALIDAD_FUERO_COMUN.PFC_III_PSIQUIATRICO.A4_CURSO;
                                    _pfc_iii_psiquiatrico.A7_BAJA_TOLERANCIA = _personalidadOld.PERSONALIDAD_FUERO_COMUN.PFC_III_PSIQUIATRICO.A7_BAJA_TOLERANCIA;
                                    _pfc_iii_psiquiatrico.B1_CONDUCTA_MOTORA = _personalidadOld.PERSONALIDAD_FUERO_COMUN.PFC_III_PSIQUIATRICO.B1_CONDUCTA_MOTORA;
                                    _pfc_iii_psiquiatrico.B2_EXPRESION_AFECTIVA = _personalidadOld.PERSONALIDAD_FUERO_COMUN.PFC_III_PSIQUIATRICO.B2_EXPRESION_AFECTIVA;
                                    _pfc_iii_psiquiatrico.B3_ILUSIONES = _personalidadOld.PERSONALIDAD_FUERO_COMUN.PFC_III_PSIQUIATRICO.B3_ILUSIONES;
                                    _pfc_iii_psiquiatrico.B4_CONTINUIDAD = _personalidadOld.PERSONALIDAD_FUERO_COMUN.PFC_III_PSIQUIATRICO.B4_CONTINUIDAD;
                                    _pfc_iii_psiquiatrico.B7_EXPRESION = _personalidadOld.PERSONALIDAD_FUERO_COMUN.PFC_III_PSIQUIATRICO.B7_EXPRESION;
                                    _pfc_iii_psiquiatrico.C1_HABLA = _personalidadOld.PERSONALIDAD_FUERO_COMUN.PFC_III_PSIQUIATRICO.C1_HABLA;
                                    _pfc_iii_psiquiatrico.C2_ADECUACION = _personalidadOld.PERSONALIDAD_FUERO_COMUN.PFC_III_PSIQUIATRICO.C2_ADECUACION;
                                    _pfc_iii_psiquiatrico.C3_DESPERSONALIZACION = _personalidadOld.PERSONALIDAD_FUERO_COMUN.PFC_III_PSIQUIATRICO.C3_DESPERSONALIZACION;
                                    _pfc_iii_psiquiatrico.C4_CONTENIDO = _personalidadOld.PERSONALIDAD_FUERO_COMUN.PFC_III_PSIQUIATRICO.C4_CONTENIDO;
                                    _pfc_iii_psiquiatrico.C7_ADECUADA = _personalidadOld.PERSONALIDAD_FUERO_COMUN.PFC_III_PSIQUIATRICO.C7_ADECUADA;
                                    _pfc_iii_psiquiatrico.COORDINADOR = _personalidadOld.PERSONALIDAD_FUERO_COMUN.PFC_III_PSIQUIATRICO.COORDINADOR;
                                    _pfc_iii_psiquiatrico.D1_ACTITUD = _personalidadOld.PERSONALIDAD_FUERO_COMUN.PFC_III_PSIQUIATRICO.D1_ACTITUD;
                                    _pfc_iii_psiquiatrico.D3_DESREALIZACION = _personalidadOld.PERSONALIDAD_FUERO_COMUN.PFC_III_PSIQUIATRICO.D3_DESREALIZACION;
                                    _pfc_iii_psiquiatrico.D4_ABASTRACTO = _personalidadOld.PERSONALIDAD_FUERO_COMUN.PFC_III_PSIQUIATRICO.D4_ABASTRACTO;
                                    _pfc_iii_psiquiatrico.E4_CONCENTRACION = _personalidadOld.PERSONALIDAD_FUERO_COMUN.PFC_III_PSIQUIATRICO.E4_CONCENTRACION;
                                    _pfc_iii_psiquiatrico.ESTUDIO_FEC = _personalidadOld.PERSONALIDAD_FUERO_COMUN.PFC_III_PSIQUIATRICO.ESTUDIO_FEC;
                                    _pfc_iii_psiquiatrico.ID_ANIO = _ingreso_nuevo.ID_ANIO;
                                    _pfc_iii_psiquiatrico.ID_CENTRO = _ingreso_nuevo.ID_CENTRO;
                                    _pfc_iii_psiquiatrico.ID_IMPUTADO = _ingreso_nuevo.ID_IMPUTADO;
                                    _pfc_iii_psiquiatrico.ID_ESTUDIO = 1;
                                    _pfc_iii_psiquiatrico.ID_INGRESO = _ingreso_nuevo.ID_INGRESO;
                                    _pfc_iii_psiquiatrico.MEDICO_PSIQUIATRA = _personalidadOld.PERSONALIDAD_FUERO_COMUN.PFC_III_PSIQUIATRICO.MEDICO_PSIQUIATRA;
                                    _pfc_iii_psiquiatrico.P10_FIANILIDAD = _personalidadOld.PERSONALIDAD_FUERO_COMUN.PFC_III_PSIQUIATRICO.P10_FIANILIDAD;
                                    _pfc_iii_psiquiatrico.P11_IMPRESION = _personalidadOld.PERSONALIDAD_FUERO_COMUN.PFC_III_PSIQUIATRICO.P11_IMPRESION;
                                    _pfc_iii_psiquiatrico.P12_DICTAMEN_PSIQUIATRICO = _personalidadOld.PERSONALIDAD_FUERO_COMUN.PFC_III_PSIQUIATRICO.P12_DICTAMEN_PSIQUIATRICO;
                                    _pfc_iii_psiquiatrico.P5_ORIENTACION = _personalidadOld.PERSONALIDAD_FUERO_COMUN.PFC_III_PSIQUIATRICO.P5_ORIENTACION;
                                    _pfc_iii_psiquiatrico.P6_MEMORIA = _personalidadOld.PERSONALIDAD_FUERO_COMUN.PFC_III_PSIQUIATRICO.P6_MEMORIA;
                                    _pfc_iii_psiquiatrico.P8_CAPACIDAD_JUICIO = _personalidadOld.PERSONALIDAD_FUERO_COMUN.PFC_III_PSIQUIATRICO.P8_CAPACIDAD_JUICIO;
                                    _pfc_iii_psiquiatrico.P9_INTROSPECCION = _personalidadOld.PERSONALIDAD_FUERO_COMUN.PFC_III_PSIQUIATRICO.P9_INTROSPECCION;
                                    _personalidad_fuero_comun.PFC_III_PSIQUIATRICO = _pfc_iii_psiquiatrico;
                                }
                                var _pfc_iv_psicologico = new PFC_IV_PSICOLOGICO();
                                if (_personalidadOld.PERSONALIDAD_FUERO_COMUN.PFC_IV_PSICOLOGICO != null)
                                {
                                    var _pfc_iv_programas = new List<PFC_IV_PROGRAMA>();
                                    foreach (var item_pfc_iv_programa in _personalidadOld.PERSONALIDAD_FUERO_COMUN.PFC_IV_PSICOLOGICO.PFC_IV_PROGRAMA)
                                        _pfc_iv_programas.Add(new PFC_IV_PROGRAMA
                                        {
                                            CONCLUYO = item_pfc_iv_programa.CONCLUYO,
                                            DURACION = item_pfc_iv_programa.DURACION,
                                            ID_ACTIVIDAD = item_pfc_iv_programa.ID_ACTIVIDAD,
                                            ID_ANIO = _ingreso_nuevo.ID_ANIO,
                                            ID_CENTRO = _ingreso_nuevo.ID_CENTRO,
                                            ID_CONSEC = item_pfc_iv_programa.ID_CONSEC,
                                            ID_ESTUDIO = 1,
                                            ID_IMPUTADO = _ingreso_nuevo.ID_IMPUTADO,
                                            ID_INGRESO = _ingreso_nuevo.ID_INGRESO,
                                            ID_TIPO_PROGRAMA = item_pfc_iv_programa.ID_TIPO_PROGRAMA,
                                            OBSERVACION = item_pfc_iv_programa.OBSERVACION
                                        });
                                    _pfc_iv_psicologico.COORDINADOR = _personalidadOld.PERSONALIDAD_FUERO_COMUN.PFC_IV_PSICOLOGICO.COORDINADOR;
                                    _pfc_iv_psicologico.ELABORO = _personalidadOld.PERSONALIDAD_FUERO_COMUN.PFC_IV_PSICOLOGICO.ELABORO;
                                    _pfc_iv_psicologico.ESTUDIO_FEC = _personalidadOld.PERSONALIDAD_FUERO_COMUN.PFC_IV_PSICOLOGICO.ESTUDIO_FEC;
                                    _pfc_iv_psicologico.ID_ANIO = _ingreso_nuevo.ID_ANIO;
                                    _pfc_iv_psicologico.ID_CENTRO = _ingreso_nuevo.ID_CENTRO;
                                    _pfc_iv_psicologico.ID_ESTUDIO = 1;
                                    _pfc_iv_psicologico.ID_IMPUTADO = _ingreso_nuevo.ID_IMPUTADO;
                                    _pfc_iv_psicologico.ID_INGRESO = _ingreso_nuevo.ID_INGRESO;
                                    _pfc_iv_psicologico.P1_CONDICIONES_GRALES = _personalidadOld.PERSONALIDAD_FUERO_COMUN.PFC_IV_PSICOLOGICO.P1_CONDICIONES_GRALES;
                                    _pfc_iv_psicologico.P10_MOTIVACION_DICTAMEN = _personalidadOld.PERSONALIDAD_FUERO_COMUN.PFC_IV_PSICOLOGICO.P10_MOTIVACION_DICTAMEN;
                                    _pfc_iv_psicologico.P11_CASO_NEGATIVO = _personalidadOld.PERSONALIDAD_FUERO_COMUN.PFC_IV_PSICOLOGICO.P11_CASO_NEGATIVO;
                                    _pfc_iv_psicologico.P12_CUAL = _personalidadOld.PERSONALIDAD_FUERO_COMUN.PFC_IV_PSICOLOGICO.P12_CUAL;
                                    _pfc_iv_psicologico.P12_REQUIERE_TRATAMIENTO = _personalidadOld.PERSONALIDAD_FUERO_COMUN.PFC_IV_PSICOLOGICO.P12_REQUIERE_TRATAMIENTO;
                                    _pfc_iv_psicologico.P2_EXAMEN_MENTAL = _personalidadOld.PERSONALIDAD_FUERO_COMUN.PFC_IV_PSICOLOGICO.P2_EXAMEN_MENTAL;
                                    _pfc_iv_psicologico.P3_PRINCIPALES_RASGOS = _personalidadOld.PERSONALIDAD_FUERO_COMUN.PFC_IV_PSICOLOGICO.P3_PRINCIPALES_RASGOS;
                                    _pfc_iv_psicologico.P4_INVENTARIO_MULTIFASICO = _personalidadOld.PERSONALIDAD_FUERO_COMUN.PFC_IV_PSICOLOGICO.P4_INVENTARIO_MULTIFASICO;
                                    _pfc_iv_psicologico.P4_OTRA_MENCIONAR = _personalidadOld.PERSONALIDAD_FUERO_COMUN.PFC_IV_PSICOLOGICO.P4_OTRA_MENCIONAR;
                                    _pfc_iv_psicologico.P4_OTRAS = _personalidadOld.PERSONALIDAD_FUERO_COMUN.PFC_IV_PSICOLOGICO.P4_OTRAS;
                                    _pfc_iv_psicologico.P4_TEST_GUALTICO = _personalidadOld.PERSONALIDAD_FUERO_COMUN.PFC_IV_PSICOLOGICO.P4_TEST_GUALTICO;
                                    _pfc_iv_psicologico.P4_TEST_HTP = _personalidadOld.PERSONALIDAD_FUERO_COMUN.PFC_IV_PSICOLOGICO.P4_TEST_HTP;
                                    _pfc_iv_psicologico.P4_TEST_MATRICES = _personalidadOld.PERSONALIDAD_FUERO_COMUN.PFC_IV_PSICOLOGICO.P4_TEST_MATRICES;
                                    _pfc_iv_psicologico.P51_NIVEL_INTELECTUAL = _personalidadOld.PERSONALIDAD_FUERO_COMUN.PFC_IV_PSICOLOGICO.P51_NIVEL_INTELECTUAL;
                                    _pfc_iv_psicologico.P52_DISFUNCION_NEUROLOGICA = _personalidadOld.PERSONALIDAD_FUERO_COMUN.PFC_IV_PSICOLOGICO.P52_DISFUNCION_NEUROLOGICA;
                                    _pfc_iv_psicologico.P6_INTEGRACION = _personalidadOld.PERSONALIDAD_FUERO_COMUN.PFC_IV_PSICOLOGICO.P6_INTEGRACION;
                                    _pfc_iv_psicologico.P8_RASGOS_PERSONALIDAD = _personalidadOld.PERSONALIDAD_FUERO_COMUN.PFC_IV_PSICOLOGICO.P8_RASGOS_PERSONALIDAD;
                                    _pfc_iv_psicologico.P9_DICTAMEN_REINSERCION = _personalidadOld.PERSONALIDAD_FUERO_COMUN.PFC_IV_PSICOLOGICO.P9_DICTAMEN_REINSERCION;
                                    _pfc_iv_psicologico.PFC_IV_PROGRAMA = _pfc_iv_programas;
                                    _personalidad_fuero_comun.PFC_IV_PSICOLOGICO = _pfc_iv_psicologico;

                                }
                                var _pfc_ix_seguridad = new PFC_IX_SEGURIDAD();
                                if (_personalidadOld.PERSONALIDAD_FUERO_COMUN.PFC_IX_SEGURIDAD != null)
                                {
                                    var _pfc_ix_correctivos = new List<PFC_IX_CORRECTIVO>();
                                    foreach (var item_pfc_ix_correctivo in _personalidadOld.PERSONALIDAD_FUERO_COMUN.PFC_IX_SEGURIDAD.PFC_IX_CORRECTIVO)
                                        _pfc_ix_correctivos.Add(new PFC_IX_CORRECTIVO
                                        {
                                            CORRECTIVO_FEC = item_pfc_ix_correctivo.CORRECTIVO_FEC,
                                            ID_ANIO = _ingreso_nuevo.ID_ANIO,
                                            ID_CENTRO = _ingreso_nuevo.ID_CENTRO,
                                            ID_CORRECTIVO = item_pfc_ix_correctivo.ID_CORRECTIVO,
                                            ID_ESTUDIO = 1,
                                            ID_IMPUTADO = _ingreso_nuevo.ID_IMPUTADO,
                                            ID_INGRESO = _ingreso_nuevo.ID_INGRESO,
                                            MOTIVO = item_pfc_ix_correctivo.MOTIVO,
                                            SANCION = item_pfc_ix_correctivo.SANCION
                                        });
                                    _pfc_ix_seguridad.COMANDANTE = _personalidadOld.PERSONALIDAD_FUERO_COMUN.PFC_IX_SEGURIDAD.COMANDANTE;
                                    _pfc_ix_seguridad.ELABORO = _personalidadOld.PERSONALIDAD_FUERO_COMUN.PFC_IX_SEGURIDAD.ELABORO;
                                    _pfc_ix_seguridad.ESTUDIO_FEC = _personalidadOld.PERSONALIDAD_FUERO_COMUN.PFC_IX_SEGURIDAD.ESTUDIO_FEC;
                                    _pfc_ix_seguridad.ID_ANIO = _ingreso_nuevo.ID_ANIO;
                                    _pfc_ix_seguridad.ID_CENTRO = _ingreso_nuevo.ID_CENTRO;
                                    _pfc_ix_seguridad.ID_ESTUDIO = 1;
                                    _pfc_ix_seguridad.ID_IMPUTADO = _ingreso_nuevo.ID_IMPUTADO;
                                    _pfc_ix_seguridad.ID_INGRESO = _ingreso_nuevo.ID_INGRESO;
                                    _pfc_ix_seguridad.P1_CONDUCTA_CENTRO = _personalidadOld.PERSONALIDAD_FUERO_COMUN.PFC_IX_SEGURIDAD.P1_CONDUCTA_CENTRO;
                                    _pfc_ix_seguridad.P2_CONDUCTA_AUTORIDAD = _personalidadOld.PERSONALIDAD_FUERO_COMUN.PFC_IX_SEGURIDAD.P2_CONDUCTA_AUTORIDAD;
                                    _pfc_ix_seguridad.P3_CONDUCTA_GENERAL = _personalidadOld.PERSONALIDAD_FUERO_COMUN.PFC_IX_SEGURIDAD.P3_CONDUCTA_GENERAL;
                                    _pfc_ix_seguridad.P4_RELACION_COMPANEROS = _personalidadOld.PERSONALIDAD_FUERO_COMUN.PFC_IX_SEGURIDAD.P4_RELACION_COMPANEROS;
                                    _pfc_ix_seguridad.P5_CORRECTIVOS = _personalidadOld.PERSONALIDAD_FUERO_COMUN.PFC_IX_SEGURIDAD.P5_CORRECTIVOS;
                                    _pfc_ix_seguridad.P6_OPINION_CONDUCTA = _personalidadOld.PERSONALIDAD_FUERO_COMUN.PFC_IX_SEGURIDAD.P6_OPINION_CONDUCTA;
                                    _pfc_ix_seguridad.P7_DICTAMEN = _personalidadOld.PERSONALIDAD_FUERO_COMUN.PFC_IX_SEGURIDAD.P7_DICTAMEN;
                                    _pfc_ix_seguridad.P8_MOTIVACION = _personalidadOld.PERSONALIDAD_FUERO_COMUN.PFC_IX_SEGURIDAD.P8_MOTIVACION;
                                    _personalidad_fuero_comun.PFC_IX_SEGURIDAD = _pfc_ix_seguridad;
                                }
                                var _pfc_v_criminodiagnostico = new PFC_V_CRIMINODIAGNOSTICO();
                                if (_personalidadOld.PERSONALIDAD_FUERO_COMUN.PFC_V_CRIMINODIAGNOSTICO != null)
                                {
                                    _pfc_v_criminodiagnostico.COORDINADOR = _personalidadOld.PERSONALIDAD_FUERO_COMUN.PFC_V_CRIMINODIAGNOSTICO.COORDINADOR;
                                    _pfc_v_criminodiagnostico.ELABORO = _personalidadOld.PERSONALIDAD_FUERO_COMUN.PFC_V_CRIMINODIAGNOSTICO.ELABORO;
                                    _pfc_v_criminodiagnostico.ESTUDIO_FEC = _personalidadOld.PERSONALIDAD_FUERO_COMUN.PFC_V_CRIMINODIAGNOSTICO.ESTUDIO_FEC;
                                    _pfc_v_criminodiagnostico.ID_ANIO = _ingreso_nuevo.ID_INGRESO;
                                    _pfc_v_criminodiagnostico.ID_CENTRO = _ingreso_nuevo.ID_CENTRO;
                                    _pfc_v_criminodiagnostico.ID_ESTUDIO = 1;
                                    _pfc_v_criminodiagnostico.ID_INGRESO = _ingreso_nuevo.ID_INGRESO;
                                    _pfc_v_criminodiagnostico.P1_ALCOHOL = _personalidadOld.PERSONALIDAD_FUERO_COMUN.PFC_V_CRIMINODIAGNOSTICO.P1_ALCOHOL;
                                    _pfc_v_criminodiagnostico.P1_DROGADO = _personalidadOld.PERSONALIDAD_FUERO_COMUN.PFC_V_CRIMINODIAGNOSTICO.P1_DROGADO;
                                    _pfc_v_criminodiagnostico.P1_DROGRA_ILEGAL = _personalidadOld.PERSONALIDAD_FUERO_COMUN.PFC_V_CRIMINODIAGNOSTICO.P1_DROGRA_ILEGAL;
                                    _pfc_v_criminodiagnostico.P1_OTRA = _personalidadOld.PERSONALIDAD_FUERO_COMUN.PFC_V_CRIMINODIAGNOSTICO.P1_OTRA;
                                    _pfc_v_criminodiagnostico.P1_VERSION_DELITO = _personalidadOld.PERSONALIDAD_FUERO_COMUN.PFC_V_CRIMINODIAGNOSTICO.P1_VERSION_DELITO;
                                    _pfc_v_criminodiagnostico.P10_DICTAMEN_REINSERCION = _personalidadOld.PERSONALIDAD_FUERO_COMUN.PFC_V_CRIMINODIAGNOSTICO.P10_DICTAMEN_REINSERCION;
                                    _pfc_v_criminodiagnostico.P10_MOTIVACION_DICTAMEN = _personalidadOld.PERSONALIDAD_FUERO_COMUN.PFC_V_CRIMINODIAGNOSTICO.P10_MOTIVACION_DICTAMEN;
                                    _pfc_v_criminodiagnostico.P11_PROGRAMAS_REMITIRSE = _personalidadOld.PERSONALIDAD_FUERO_COMUN.PFC_V_CRIMINODIAGNOSTICO.P11_PROGRAMAS_REMITIRSE;
                                    _pfc_v_criminodiagnostico.P12_CUAL = _personalidadOld.PERSONALIDAD_FUERO_COMUN.PFC_V_CRIMINODIAGNOSTICO.P12_CUAL;
                                    _pfc_v_criminodiagnostico.P12_TRATAMIENTO_EXTRAMUROS = _personalidadOld.PERSONALIDAD_FUERO_COMUN.PFC_V_CRIMINODIAGNOSTICO.P12_TRATAMIENTO_EXTRAMUROS;
                                    _pfc_v_criminodiagnostico.P2_CRIMINOGENESIS = _personalidadOld.PERSONALIDAD_FUERO_COMUN.PFC_V_CRIMINODIAGNOSTICO.P2_CRIMINOGENESIS;
                                    _pfc_v_criminodiagnostico.P3_CONDUCTA_ANTISOCIAL = _personalidadOld.PERSONALIDAD_FUERO_COMUN.PFC_V_CRIMINODIAGNOSTICO.P3_CONDUCTA_ANTISOCIAL;
                                    _pfc_v_criminodiagnostico.P4_CLASIFICACION_CRIMINOLOGICA = _personalidadOld.PERSONALIDAD_FUERO_COMUN.PFC_V_CRIMINODIAGNOSTICO.P4_CLASIFICACION_CRIMINOLOGICA;
                                    _pfc_v_criminodiagnostico.P5_INTIMIDACION = _personalidadOld.PERSONALIDAD_FUERO_COMUN.PFC_V_CRIMINODIAGNOSTICO.P5_INTIMIDACION;
                                    _pfc_v_criminodiagnostico.P5_PORQUE = _personalidadOld.PERSONALIDAD_FUERO_COMUN.PFC_V_CRIMINODIAGNOSTICO.P5_PORQUE;
                                    _pfc_v_criminodiagnostico.P6_CAPACIDAD_CRIMINAL = _personalidadOld.PERSONALIDAD_FUERO_COMUN.PFC_V_CRIMINODIAGNOSTICO.P6_CAPACIDAD_CRIMINAL;
                                    _pfc_v_criminodiagnostico.P6A_EGOCENTRICO = _personalidadOld.PERSONALIDAD_FUERO_COMUN.PFC_V_CRIMINODIAGNOSTICO.P6A_EGOCENTRICO;
                                    _pfc_v_criminodiagnostico.P6B_LIABILIDAD_EFECTIVA = _personalidadOld.PERSONALIDAD_FUERO_COMUN.PFC_V_CRIMINODIAGNOSTICO.P6B_LIABILIDAD_EFECTIVA;
                                    _pfc_v_criminodiagnostico.P6C_AGRESIVIDAD = _personalidadOld.PERSONALIDAD_FUERO_COMUN.PFC_V_CRIMINODIAGNOSTICO.P6C_AGRESIVIDAD;
                                    _pfc_v_criminodiagnostico.P6D_INDIFERENCIA_AFECTIVA = _personalidadOld.PERSONALIDAD_FUERO_COMUN.PFC_V_CRIMINODIAGNOSTICO.P6D_INDIFERENCIA_AFECTIVA;
                                    _pfc_v_criminodiagnostico.P7_ADAPTACION_SOCIAL = _personalidadOld.PERSONALIDAD_FUERO_COMUN.PFC_V_CRIMINODIAGNOSTICO.P7_ADAPTACION_SOCIAL;
                                    _pfc_v_criminodiagnostico.P8_INDICE_PELIGROSIDAD = _personalidadOld.PERSONALIDAD_FUERO_COMUN.PFC_V_CRIMINODIAGNOSTICO.P8_INDICE_PELIGROSIDAD;
                                    _pfc_v_criminodiagnostico.P9_PRONOSTICO_REINCIDENCIA = _personalidadOld.PERSONALIDAD_FUERO_COMUN.PFC_V_CRIMINODIAGNOSTICO.P9_PRONOSTICO_REINCIDENCIA;
                                    _personalidad_fuero_comun.PFC_V_CRIMINODIAGNOSTICO = _pfc_v_criminodiagnostico;
                                }

                                var _pfc_vi_socio_familiar = new PFC_VI_SOCIO_FAMILIAR();
                                if (_personalidadOld.PERSONALIDAD_FUERO_COMUN.PFC_VI_SOCIO_FAMILIAR != null)
                                {
                                    var _pfc_vi_comunicaciones = new List<PFC_VI_COMUNICACION>();
                                    foreach (var item_pfc_vi_comunicacion in _personalidadOld.PERSONALIDAD_FUERO_COMUN.PFC_VI_SOCIO_FAMILIAR.PFC_VI_COMUNICACION)
                                        _pfc_vi_comunicaciones.Add(new PFC_VI_COMUNICACION
                                        {
                                            FRECUENCIA = item_pfc_vi_comunicacion.FRECUENCIA,
                                            ID_ANIO = _ingreso_nuevo.ID_ANIO,
                                            ID_CENTRO = _ingreso_nuevo.ID_CENTRO,
                                            ID_CONSEC = item_pfc_vi_comunicacion.ID_CONSEC,
                                            ID_ESTUDIO = 1,
                                            ID_IMPUTADO = _ingreso_nuevo.ID_IMPUTADO,
                                            ID_INGRESO = _ingreso_nuevo.ID_INGRESO,
                                            ID_TIPO_REFERENCIA = item_pfc_vi_comunicacion.ID_TIPO_REFERENCIA,
                                            NOMBRE = item_pfc_vi_comunicacion.NOMBRE,
                                            TELEFONO = item_pfc_vi_comunicacion.TELEFONO
                                        });
                                    var _pfc_vi_grupos = new List<PFC_VI_GRUPO>();
                                    foreach (var item_pfc_vi_grupo in _personalidadOld.PERSONALIDAD_FUERO_COMUN.PFC_VI_SOCIO_FAMILIAR.PFC_VI_GRUPO)
                                        _pfc_vi_grupos.Add(new PFC_VI_GRUPO
                                        {
                                            CONGREGACION = item_pfc_vi_grupo.CONGREGACION,
                                            ID_ACTIVIDAD = item_pfc_vi_grupo.ID_ACTIVIDAD,
                                            ID_ANIO = _ingreso_nuevo.ID_ANIO,
                                            ID_CENTRO = _ingreso_nuevo.ID_CENTRO,
                                            ID_ESTUDIO = 1,
                                            ID_IMPUTADO = _ingreso_nuevo.ID_IMPUTADO,
                                            ID_INGRESO = _ingreso_nuevo.ID_INGRESO,
                                            ID_TIPO_PROGRAMA = item_pfc_vi_grupo.ID_TIPO_PROGRAMA,
                                            OBSERVACIONES = item_pfc_vi_grupo.OBSERVACIONES,
                                            PERIODO = item_pfc_vi_grupo.PERIODO
                                        });
                                    _pfc_vi_socio_familiar.COORDINADOR = _personalidadOld.PERSONALIDAD_FUERO_COMUN.PFC_VI_SOCIO_FAMILIAR.COORDINADOR;
                                    _pfc_vi_socio_familiar.ELABORO = _personalidadOld.PERSONALIDAD_FUERO_COMUN.PFC_VI_SOCIO_FAMILIAR.ELABORO;
                                    _pfc_vi_socio_familiar.ESTUDIO_FEC = _personalidadOld.PERSONALIDAD_FUERO_COMUN.PFC_VI_SOCIO_FAMILIAR.ESTUDIO_FEC;
                                    _pfc_vi_socio_familiar.ID_ANIO = _ingreso_nuevo.ID_ANIO;
                                    _pfc_vi_socio_familiar.ID_CENTRO = _ingreso_nuevo.ID_CENTRO;
                                    _pfc_vi_socio_familiar.ID_ESTUDIO = 1;
                                    _pfc_vi_socio_familiar.ID_IMPUTADO = _ingreso_nuevo.ID_IMPUTADO;
                                    _pfc_vi_socio_familiar.ID_INGRESO = _ingreso_nuevo.ID_INGRESO;
                                    _pfc_vi_socio_familiar.P10_DICTAMEN = _personalidadOld.PERSONALIDAD_FUERO_COMUN.PFC_VI_SOCIO_FAMILIAR.P10_DICTAMEN;
                                    _pfc_vi_socio_familiar.P11_MOTIVACION_DICTAMEN = _personalidadOld.PERSONALIDAD_FUERO_COMUN.PFC_VI_SOCIO_FAMILIAR.P11_MOTIVACION_DICTAMEN;
                                    _pfc_vi_socio_familiar.P21_FAMILIA_PRIMARIA = _personalidadOld.PERSONALIDAD_FUERO_COMUN.PFC_VI_SOCIO_FAMILIAR.P21_FAMILIA_PRIMARIA;
                                    _pfc_vi_socio_familiar.P22_FAMILIA_SECUNDARIA = _personalidadOld.PERSONALIDAD_FUERO_COMUN.PFC_VI_SOCIO_FAMILIAR.P22_FAMILIA_SECUNDARIA;
                                    _pfc_vi_socio_familiar.P3_TERCERA_EDAD = _personalidadOld.PERSONALIDAD_FUERO_COMUN.PFC_VI_SOCIO_FAMILIAR.P3_TERCERA_EDAD;
                                    _pfc_vi_socio_familiar.P4_ESPOSOA = _personalidadOld.PERSONALIDAD_FUERO_COMUN.PFC_VI_SOCIO_FAMILIAR.P4_ESPOSOA;
                                    _pfc_vi_socio_familiar.P4_FRECUENCIA = _personalidadOld.PERSONALIDAD_FUERO_COMUN.PFC_VI_SOCIO_FAMILIAR.P4_FRECUENCIA;
                                    _pfc_vi_socio_familiar.P4_HERMANOS = _personalidadOld.PERSONALIDAD_FUERO_COMUN.PFC_VI_SOCIO_FAMILIAR.P4_HERMANOS;
                                    _pfc_vi_socio_familiar.P4_HIJOS = _personalidadOld.PERSONALIDAD_FUERO_COMUN.PFC_VI_SOCIO_FAMILIAR.P4_HIJOS;
                                    _pfc_vi_socio_familiar.P4_MADRE = _personalidadOld.PERSONALIDAD_FUERO_COMUN.PFC_VI_SOCIO_FAMILIAR.P4_MADRE;
                                    _pfc_vi_socio_familiar.P4_MOTIVO_NO_VISITA = _personalidadOld.PERSONALIDAD_FUERO_COMUN.PFC_VI_SOCIO_FAMILIAR.P4_MOTIVO_NO_VISITA;
                                    _pfc_vi_socio_familiar.P4_OTROS = _personalidadOld.PERSONALIDAD_FUERO_COMUN.PFC_VI_SOCIO_FAMILIAR.P4_OTROS;
                                    _pfc_vi_socio_familiar.P4_OTROS_EPECIFICAR = _personalidadOld.PERSONALIDAD_FUERO_COMUN.PFC_VI_SOCIO_FAMILIAR.P4_OTROS_EPECIFICAR;
                                    _pfc_vi_socio_familiar.P4_PADRE = _personalidadOld.PERSONALIDAD_FUERO_COMUN.PFC_VI_SOCIO_FAMILIAR.P4_PADRE;
                                    _pfc_vi_socio_familiar.P4_RECIBE_VISITA = _personalidadOld.PERSONALIDAD_FUERO_COMUN.PFC_VI_SOCIO_FAMILIAR.P4_RECIBE_VISITA;
                                    _pfc_vi_socio_familiar.P5_COMUNICACION_TELEFONICA = _personalidadOld.PERSONALIDAD_FUERO_COMUN.PFC_VI_SOCIO_FAMILIAR.P5_COMUNICACION_TELEFONICA;
                                    _pfc_vi_socio_familiar.P5_NO_POR_QUE = _personalidadOld.PERSONALIDAD_FUERO_COMUN.PFC_VI_SOCIO_FAMILIAR.P5_NO_POR_QUE;
                                    _pfc_vi_socio_familiar.P6_APOYO_EXTERIOR = _personalidadOld.PERSONALIDAD_FUERO_COMUN.PFC_VI_SOCIO_FAMILIAR.P6_APOYO_EXTERIOR;
                                    _pfc_vi_socio_familiar.P7_PLANES_INTERNO = _personalidadOld.PERSONALIDAD_FUERO_COMUN.PFC_VI_SOCIO_FAMILIAR.P7_PLANES_INTERNO;
                                    _pfc_vi_socio_familiar.P7_VIVIRA = _personalidadOld.PERSONALIDAD_FUERO_COMUN.PFC_VI_SOCIO_FAMILIAR.P7_VIVIRA;
                                    _pfc_vi_socio_familiar.P8_OFERTA_ESPECIFICAR = _personalidadOld.PERSONALIDAD_FUERO_COMUN.PFC_VI_SOCIO_FAMILIAR.P8_OFERTA_ESPECIFICAR;
                                    _pfc_vi_socio_familiar.P8_OFERTA_TRABAJO = _personalidadOld.PERSONALIDAD_FUERO_COMUN.PFC_VI_SOCIO_FAMILIAR.P8_OFERTA_TRABAJO;
                                    _pfc_vi_socio_familiar.P9_AVAL_ESPECIFICAR = _personalidadOld.PERSONALIDAD_FUERO_COMUN.PFC_VI_SOCIO_FAMILIAR.P9_AVAL_ESPECIFICAR;
                                    _pfc_vi_socio_familiar.P9_AVAL_MORAL = _personalidadOld.PERSONALIDAD_FUERO_COMUN.PFC_VI_SOCIO_FAMILIAR.P9_AVAL_MORAL;
                                    _pfc_vi_socio_familiar.PFC_VI_COMUNICACION = _pfc_vi_comunicaciones;
                                    _pfc_vi_socio_familiar.PFC_VI_GRUPO = _pfc_vi_grupos;
                                    _personalidad_fuero_comun.PFC_VI_SOCIO_FAMILIAR = _pfc_vi_socio_familiar;
                                }

                                var _pfc_viii_trabajo = new PFC_VIII_TRABAJO();
                                if (_personalidadOld.PERSONALIDAD_FUERO_COMUN.PFC_VIII_TRABAJO != null)
                                {


                                    _pfc_viii_trabajo.COORDINADOR = _personalidadOld.PERSONALIDAD_FUERO_COMUN.PFC_VIII_TRABAJO.COORDINADOR;
                                    _pfc_viii_trabajo.ELABORO = _personalidadOld.PERSONALIDAD_FUERO_COMUN.PFC_VIII_TRABAJO.ELABORO;
                                    _pfc_viii_trabajo.ESTUDIO_FEC = _personalidadOld.PERSONALIDAD_FUERO_COMUN.PFC_VIII_TRABAJO.ESTUDIO_FEC;
                                    _pfc_viii_trabajo.ID_ANIO = _ingreso_nuevo.ID_ANIO;
                                    _pfc_viii_trabajo.ID_CENTRO = _ingreso_nuevo.ID_CENTRO;
                                    _pfc_viii_trabajo.ID_ESTUDIO = 1;
                                    _pfc_viii_trabajo.ID_IMPUTADO = _ingreso_nuevo.ID_IMPUTADO;
                                    _pfc_viii_trabajo.ID_INGRESO = _ingreso_nuevo.ID_INGRESO;
                                    _pfc_viii_trabajo.P1_TRABAJO_ANTES = _personalidadOld.PERSONALIDAD_FUERO_COMUN.PFC_VIII_TRABAJO.P1_TRABAJO_ANTES;
                                    _pfc_viii_trabajo.P3_CALIDAD = _personalidadOld.PERSONALIDAD_FUERO_COMUN.PFC_VIII_TRABAJO.P3_CALIDAD;
                                    _pfc_viii_trabajo.P3_PERSEVERANCIA = _personalidadOld.PERSONALIDAD_FUERO_COMUN.PFC_VIII_TRABAJO.P3_PERSEVERANCIA;
                                    _pfc_viii_trabajo.P3_RESPONSABILIDAD = _personalidadOld.PERSONALIDAD_FUERO_COMUN.PFC_VIII_TRABAJO.P3_RESPONSABILIDAD;
                                    _pfc_viii_trabajo.P4_FONDO_AHORRO = _personalidadOld.PERSONALIDAD_FUERO_COMUN.PFC_VIII_TRABAJO.P4_FONDO_AHORRO;
                                    _pfc_viii_trabajo.P5_DIAS_CENTRO_ACTUAL = _personalidadOld.PERSONALIDAD_FUERO_COMUN.PFC_VIII_TRABAJO.P5_DIAS_CENTRO_ACTUAL;
                                    _pfc_viii_trabajo.P5_DIAS_LABORADOS = _personalidadOld.PERSONALIDAD_FUERO_COMUN.PFC_VIII_TRABAJO.P5_DIAS_LABORADOS;
                                    _pfc_viii_trabajo.P5_DIAS_OTROS_CENTROS = _personalidadOld.PERSONALIDAD_FUERO_COMUN.PFC_VIII_TRABAJO.P5_DIAS_OTROS_CENTROS;
                                    _pfc_viii_trabajo.P5_PERIODO_LABORAL = _personalidadOld.PERSONALIDAD_FUERO_COMUN.PFC_VIII_TRABAJO.P5_PERIODO_LABORAL;
                                    _pfc_viii_trabajo.P6_DICTAMEN = _personalidadOld.PERSONALIDAD_FUERO_COMUN.PFC_VIII_TRABAJO.P6_DICTAMEN;
                                    _pfc_viii_trabajo.P7_MOTIVACION = _personalidadOld.PERSONALIDAD_FUERO_COMUN.PFC_VIII_TRABAJO.P6_DICTAMEN;
                                    //_pfc_viii_trabajo.PFC_VIII_ACTIVIDAD_LABORAL   Queda pendiente de que sergio lo convierta en detalle.
                                    _personalidad_fuero_comun.PFC_VIII_TRABAJO = _pfc_viii_trabajo;

                                }
                                _personalidad_fuero_comun.ID_ANIO = _ingreso_nuevo.ID_ANIO;
                                _personalidad_fuero_comun.ID_CENTRO = _ingreso_nuevo.ID_CENTRO;
                                _personalidad_fuero_comun.ID_ESTUDIO = 1;
                                _personalidad_fuero_comun.ID_IMPUTADO = _ingreso_nuevo.ID_IMPUTADO;
                                _personalidad_fuero_comun.ID_INGRESO = _ingreso_nuevo.ID_INGRESO;
                            }
                            else
                                _personalidad_fuero_comun = null;
                            #endregion

                            #region PFF
                            var _personalidad_fuero_federal = new PERSONALIDAD_FUERO_FEDERAL();
                            if (_personalidadOld.PERSONALIDAD_FUERO_FEDERAL != null)
                            {
                                var _pff_acta_consejo_tecnico = new PFF_ACTA_CONSEJO_TECNICO();
                                if (_personalidadOld.PERSONALIDAD_FUERO_FEDERAL.PFF_ACTA_CONSEJO_TECNICO != null)
                                {
                                    var _pff_acta_determino = new List<PFF_ACTA_DETERMINO>();
                                    foreach (var item_pff_acta_determino in _personalidadOld.PERSONALIDAD_FUERO_FEDERAL.PFF_ACTA_CONSEJO_TECNICO.PFF_ACTA_DETERMINO)
                                        _pff_acta_determino.Add(new PFF_ACTA_DETERMINO
                                        {
                                            ID_ANIO = _ingreso_nuevo.ID_ANIO,
                                            ID_AREA_TECNICA = item_pff_acta_determino.ID_AREA_TECNICA,
                                            ID_CENTRO = _ingreso_nuevo.ID_CENTRO,
                                            ID_ESTUDIO = 1,
                                            ID_IMPUTADO = _ingreso_nuevo.ID_IMPUTADO,
                                            ID_INGRESO = _ingreso_nuevo.ID_INGRESO,
                                            NOMBRE = item_pff_acta_determino.NOMBRE,
                                            OPINION = item_pff_acta_determino.OPINION
                                        });
                                    _pff_acta_consejo_tecnico.APROBADO_APLAZADO = _personalidadOld.PERSONALIDAD_FUERO_FEDERAL.PFF_ACTA_CONSEJO_TECNICO.APROBADO_APLAZADO;
                                    _pff_acta_consejo_tecnico.APROBADO_POR = _personalidadOld.PERSONALIDAD_FUERO_FEDERAL.PFF_ACTA_CONSEJO_TECNICO.APROBADO_POR;
                                    _pff_acta_consejo_tecnico.CEN_ID_CENTRO = _personalidadOld.PERSONALIDAD_FUERO_FEDERAL.PFF_ACTA_CONSEJO_TECNICO.CEN_ID_CENTRO;
                                    _pff_acta_consejo_tecnico.DIRECTOR = _personalidadOld.PERSONALIDAD_FUERO_FEDERAL.PFF_ACTA_CONSEJO_TECNICO.DIRECTOR;
                                    _pff_acta_consejo_tecnico.EXPEDIENTE = _personalidadOld.PERSONALIDAD_FUERO_FEDERAL.PFF_ACTA_CONSEJO_TECNICO.EXPEDIENTE;
                                    _pff_acta_consejo_tecnico.FECHA = _personalidadOld.PERSONALIDAD_FUERO_FEDERAL.PFF_ACTA_CONSEJO_TECNICO.FECHA;
                                    _pff_acta_consejo_tecnico.ID_ANIO = _ingreso_nuevo.ID_ANIO;
                                    _pff_acta_consejo_tecnico.ID_CENTRO = _ingreso_nuevo.ID_CENTRO;
                                    _pff_acta_consejo_tecnico.ID_ESTUDIO = 1;
                                    _pff_acta_consejo_tecnico.ID_IMPUTADO = _ingreso_nuevo.ID_IMPUTADO;
                                    _pff_acta_consejo_tecnico.ID_INGRESO = _ingreso_nuevo.ID_INGRESO;
                                    _pff_acta_consejo_tecnico.LUGAR = _personalidadOld.PERSONALIDAD_FUERO_FEDERAL.PFF_ACTA_CONSEJO_TECNICO.LUGAR;
                                    _pff_acta_consejo_tecnico.PFF_ACTA_DETERMINO = _pff_acta_determino;
                                    _personalidad_fuero_federal.PFF_ACTA_CONSEJO_TECNICO = _pff_acta_consejo_tecnico;
                                }
                                var _pff_actividad = new PFF_ACTIVIDAD();
                                if (_personalidadOld.PERSONALIDAD_FUERO_FEDERAL.PFF_ACTIVIDAD != null)
                                {
                                    var _pff_actividad_participaciones = new List<PFF_ACTIVIDAD_PARTICIPACION>();
                                    foreach (var item_pff_actividad_participacion in _personalidadOld.PERSONALIDAD_FUERO_FEDERAL.PFF_ACTIVIDAD.PFF_ACTIVIDAD_PARTICIPACION)
                                        _pff_actividad_participaciones.Add(new PFF_ACTIVIDAD_PARTICIPACION
                                        {
                                            FECHA_1 = item_pff_actividad_participacion.FECHA_1,
                                            FECHA_2 = item_pff_actividad_participacion.FECHA_2,
                                            ID_ANIO = _ingreso_nuevo.ID_ANIO,
                                            ID_CENTRO = _ingreso_nuevo.ID_CENTRO,
                                            ID_ESTUDIO = 1,
                                            ID_IMPUTADO = _ingreso_nuevo.ID_IMPUTADO,
                                            ID_INGRESO = _ingreso_nuevo.ID_INGRESO,
                                            ID_TIPO_PROGRAMA = item_pff_actividad_participacion.ID_TIPO_PROGRAMA,
                                            OTRO_ESPECIFICAR = item_pff_actividad_participacion.OTRO_ESPECIFICAR,
                                            PARTICIPACION = item_pff_actividad_participacion.PARTICIPACION
                                        });
                                    _pff_actividad.ALFABE_PRIMARIA = _personalidadOld.PERSONALIDAD_FUERO_FEDERAL.PFF_ACTIVIDAD.ALFABE_PRIMARIA;
                                    _pff_actividad.ASISTE_PUNTUAL = _personalidadOld.PERSONALIDAD_FUERO_FEDERAL.PFF_ACTIVIDAD.ASISTE_PUNTUAL;
                                    _pff_actividad.ASISTE_PUNTUAL_NO_POR_QUE = _personalidadOld.PERSONALIDAD_FUERO_FEDERAL.PFF_ACTIVIDAD.ASISTE_PUNTUAL_NO_POR_QUE;
                                    _pff_actividad.AVANCE_RENDIMIENTO_ACADEMINCO = _personalidadOld.PERSONALIDAD_FUERO_FEDERAL.PFF_ACTIVIDAD.AVANCE_RENDIMIENTO_ACADEMINCO;
                                    _pff_actividad.BACHILLER_UNI = _personalidadOld.PERSONALIDAD_FUERO_FEDERAL.PFF_ACTIVIDAD.BACHILLER_UNI;
                                    _pff_actividad.CONCLUSIONES = _personalidadOld.PERSONALIDAD_FUERO_FEDERAL.PFF_ACTIVIDAD.CONCLUSIONES;
                                    _pff_actividad.DIRECTOR_CENTRO = _personalidadOld.PERSONALIDAD_FUERO_FEDERAL.PFF_ACTIVIDAD.DIRECTOR_CENTRO;
                                    _pff_actividad.ESCOLARIDAD_MOMENTO = _personalidadOld.PERSONALIDAD_FUERO_FEDERAL.PFF_ACTIVIDAD.ESCOLARIDAD_MOMENTO;
                                    _pff_actividad.ESPECIFIQUE = _personalidadOld.PERSONALIDAD_FUERO_FEDERAL.PFF_ACTIVIDAD.ESPECIFIQUE;
                                    _pff_actividad.ESTUDIOS_ACTUALES = _personalidadOld.PERSONALIDAD_FUERO_FEDERAL.PFF_ACTIVIDAD.ESTUDIOS_ACTUALES;
                                    _pff_actividad.ESTUDIOS_EN_INTERNAMIENTO = _personalidadOld.PERSONALIDAD_FUERO_FEDERAL.PFF_ACTIVIDAD.ESTUDIOS_EN_INTERNAMIENTO;
                                    _pff_actividad.FECHA = _personalidadOld.PERSONALIDAD_FUERO_FEDERAL.PFF_ACTIVIDAD.FECHA;
                                    _pff_actividad.ID_ANIO = _ingreso_nuevo.ID_ANIO;
                                    _pff_actividad.ID_CENTRO = _ingreso_nuevo.ID_CENTRO;
                                    _pff_actividad.ID_ESTUDIO = 1;
                                    _pff_actividad.ID_IMPUTADO = _ingreso_nuevo.ID_IMPUTADO;
                                    _pff_actividad.ID_INGRESO = _ingreso_nuevo.ID_INGRESO;
                                    _pff_actividad.IMPARTIDO_ENSENANZA = _personalidadOld.PERSONALIDAD_FUERO_FEDERAL.PFF_ACTIVIDAD.IMPARTIDO_ENSENANZA;
                                    _pff_actividad.IMPARTIDO_ENSENANZA_TIEMPO = _personalidadOld.PERSONALIDAD_FUERO_FEDERAL.PFF_ACTIVIDAD.IMPARTIDO_ENSENANZA_TIEMPO;
                                    _pff_actividad.IMPARTIDO_ENSENANZA_TIPO = _personalidadOld.PERSONALIDAD_FUERO_FEDERAL.PFF_ACTIVIDAD.IMPARTIDO_ENSENANZA_TIPO;
                                    _pff_actividad.JEFE_SECC_EDUCATIVA = _personalidadOld.PERSONALIDAD_FUERO_FEDERAL.PFF_ACTIVIDAD.JEFE_SECC_EDUCATIVA;
                                    _pff_actividad.LUGAR = _personalidadOld.PERSONALIDAD_FUERO_FEDERAL.PFF_ACTIVIDAD.LUGAR;
                                    _pff_actividad.NOMBRE = _personalidadOld.PERSONALIDAD_FUERO_FEDERAL.PFF_ACTIVIDAD.NOMBRE;
                                    _pff_actividad.OTRA_ENSENANZA = _personalidadOld.PERSONALIDAD_FUERO_FEDERAL.PFF_ACTIVIDAD.OTRA_ENSENANZA;
                                    _pff_actividad.OTRO = _personalidadOld.PERSONALIDAD_FUERO_FEDERAL.PFF_ACTIVIDAD.OTRO;
                                    _pff_actividad.PFF_ACTIVIDAD_PARTICIPACION = _pff_actividad_participaciones;
                                    _pff_actividad.PRIMARIA_SECU = _personalidadOld.PERSONALIDAD_FUERO_FEDERAL.PFF_ACTIVIDAD.PRIMARIA_SECU;
                                    _pff_actividad.PROMOVIDO = _personalidadOld.PERSONALIDAD_FUERO_FEDERAL.PFF_ACTIVIDAD.PROMOVIDO;
                                    _pff_actividad.SECU_BACHILLER = _personalidadOld.PERSONALIDAD_FUERO_FEDERAL.PFF_ACTIVIDAD.SECU_BACHILLER;
                                    _personalidad_fuero_federal.PFF_ACTIVIDAD = _pff_actividad;
                                }
                                var _pff_capacitacion = new PFF_CAPACITACION();
                                if (_personalidadOld.PERSONALIDAD_FUERO_FEDERAL.PFF_CAPACITACION != null)
                                {
                                    var _pff_capacitacion_cursos = new List<PFF_CAPACITACION_CURSO>();
                                    foreach (var item_pff_capacitacion_curso in _personalidadOld.PERSONALIDAD_FUERO_FEDERAL.PFF_CAPACITACION.PFF_CAPACITACION_CURSO)
                                        _pff_capacitacion_cursos.Add(new PFF_CAPACITACION_CURSO
                                        {
                                            CURSO = item_pff_capacitacion_curso.CURSO,
                                            FECHA_INICIO = item_pff_capacitacion_curso.FECHA_INICIO,
                                            FECHA_TERMINO = item_pff_capacitacion_curso.FECHA_TERMINO,
                                            ID_ANIO = _ingreso_nuevo.ID_ANIO,
                                            ID_CENTRO = _ingreso_nuevo.ID_CENTRO,
                                            ID_CONSEC = item_pff_capacitacion_curso.ID_CONSEC,
                                            ID_ESTUDIO = 1,
                                            ID_IMPUTADO = _ingreso_nuevo.ID_IMPUTADO,
                                            ID_INGRESO = _ingreso_nuevo.ID_INGRESO
                                        });
                                    var _pff_dias_laborado = new List<PFF_DIAS_LABORADO>();
                                    foreach (var item_pff_dia_laborado in _personalidadOld.PERSONALIDAD_FUERO_FEDERAL.PFF_CAPACITACION.PFF_DIAS_LABORADO)
                                        _pff_dias_laborado.Add(new PFF_DIAS_LABORADO
                                        {
                                            ANIO = item_pff_dia_laborado.ANIO,
                                            DIAS_TRABAJADOS = item_pff_dia_laborado.DIAS_TRABAJADOS,
                                            ID_ANIO = _ingreso_nuevo.ID_ANIO,
                                            ID_CENTRO = _ingreso_nuevo.ID_CENTRO,
                                            ID_ESTUDIO = 1,
                                            ID_IMPUTADO = _ingreso_nuevo.ID_IMPUTADO,
                                            ID_INGRESO = _ingreso_nuevo.ID_INGRESO,
                                            MES = item_pff_dia_laborado.MES
                                        });
                                    _pff_capacitacion.A_TOTAL_DIAS_LABORADOS = _personalidadOld.PERSONALIDAD_FUERO_FEDERAL.PFF_CAPACITACION.A_TOTAL_DIAS_LABORADOS;
                                    _pff_capacitacion.ACTITUDES_DESEMPENO_ACT = _personalidadOld.PERSONALIDAD_FUERO_FEDERAL.PFF_CAPACITACION.ACTITUDES_DESEMPENO_ACT;
                                    _pff_capacitacion.ACTIVIDAD_PRODUC_ACTUAL = _personalidadOld.PERSONALIDAD_FUERO_FEDERAL.PFF_CAPACITACION.ACTIVIDAD_PRODUC_ACTUAL;
                                    _pff_capacitacion.ATIENDE_INDICACIONES = _personalidadOld.PERSONALIDAD_FUERO_FEDERAL.PFF_CAPACITACION.ATIENDE_INDICACIONES;
                                    _pff_capacitacion.B_DIAS_LABORADOS_OTROS_CERESOS = _personalidadOld.PERSONALIDAD_FUERO_FEDERAL.PFF_CAPACITACION.B_DIAS_LABORADOS_OTROS_CERESOS;
                                    _pff_capacitacion.CAMBIO_ACTIVIDAD = _personalidadOld.PERSONALIDAD_FUERO_FEDERAL.PFF_CAPACITACION.CAMBIO_ACTIVIDAD;
                                    _pff_capacitacion.CAMBIO_ACTIVIDAD_POR_QUE = _personalidadOld.PERSONALIDAD_FUERO_FEDERAL.PFF_CAPACITACION.CAMBIO_ACTIVIDAD_POR_QUE;
                                    _pff_capacitacion.CONCLUSIONES = _personalidadOld.PERSONALIDAD_FUERO_FEDERAL.PFF_CAPACITACION.CONCLUSIONES;
                                    _pff_capacitacion.DESCUIDADO_LABORES = _personalidadOld.PERSONALIDAD_FUERO_FEDERAL.PFF_CAPACITACION.DESCUIDADO_LABORES;
                                    _pff_capacitacion.DIRECTOR_CENTRO = _personalidadOld.PERSONALIDAD_FUERO_FEDERAL.PFF_CAPACITACION.DIRECTOR_CENTRO;
                                    _pff_capacitacion.FECHA = _personalidadOld.PERSONALIDAD_FUERO_FEDERAL.PFF_CAPACITACION.FECHA;
                                    _pff_capacitacion.FONDO_AHORRO = _personalidadOld.PERSONALIDAD_FUERO_FEDERAL.PFF_CAPACITACION.FONDO_AHORRO;
                                    _pff_capacitacion.FONDO_AHORRO_COMPESACION_ACTUA = _personalidadOld.PERSONALIDAD_FUERO_FEDERAL.PFF_CAPACITACION.FONDO_AHORRO_COMPESACION_ACTUA;
                                    _pff_capacitacion.HA_PROGRESADO_OFICIO = _personalidadOld.PERSONALIDAD_FUERO_FEDERAL.PFF_CAPACITACION.HA_PROGRESADO_OFICIO;
                                    _pff_capacitacion.ID_ANIO = _ingreso_nuevo.ID_ANIO;
                                    _pff_capacitacion.ID_CENTRO = _ingreso_nuevo.ID_CENTRO;
                                    _pff_capacitacion.ID_ESTUDIO = 1;
                                    _pff_capacitacion.ID_IMPUTADO = _ingreso_nuevo.ID_IMPUTADO;
                                    _pff_capacitacion.ID_INGRESO = _ingreso_nuevo.ID_INGRESO;
                                    _pff_capacitacion.JEFE_SECC_INDUSTRIAL = _personalidadOld.PERSONALIDAD_FUERO_FEDERAL.PFF_CAPACITACION.JEFE_SECC_INDUSTRIAL;
                                    _pff_capacitacion.LUGAR = _personalidadOld.PERSONALIDAD_FUERO_FEDERAL.PFF_CAPACITACION.LUGAR;
                                    _pff_capacitacion.MOTIVO_TIEMPO_INTERRUP_ACT = _personalidadOld.PERSONALIDAD_FUERO_FEDERAL.PFF_CAPACITACION.MOTIVO_TIEMPO_INTERRUP_ACT;
                                    _pff_capacitacion.NO_CURSOS_MOTIVO = _personalidadOld.PERSONALIDAD_FUERO_FEDERAL.PFF_CAPACITACION.NO_CURSOS_MOTIVO;
                                    _pff_capacitacion.NOMBRE = _personalidadOld.PERSONALIDAD_FUERO_FEDERAL.PFF_CAPACITACION.NOMBRE;
                                    _pff_capacitacion.OFICIO_ANTES_RECLUSION = _personalidadOld.PERSONALIDAD_FUERO_FEDERAL.PFF_CAPACITACION.OFICIO_ANTES_RECLUSION;
                                    _pff_capacitacion.PFF_CAPACITACION_CURSO = _pff_capacitacion_cursos;
                                    _pff_capacitacion.PFF_DIAS_LABORADO = _pff_dias_laborado;
                                    _pff_capacitacion.RECIBIO_CONSTANCIA = _personalidadOld.PERSONALIDAD_FUERO_FEDERAL.PFF_CAPACITACION.RECIBIO_CONSTANCIA;
                                    _pff_capacitacion.SALARIO_DEVENGABA_DETENCION = _personalidadOld.PERSONALIDAD_FUERO_FEDERAL.PFF_CAPACITACION.SALARIO_DEVENGABA_DETENCION;
                                    _pff_capacitacion.SATISFACE_ACTIVIDAD = _personalidadOld.PERSONALIDAD_FUERO_FEDERAL.PFF_CAPACITACION.SATISFACE_ACTIVIDAD;
                                    _pff_capacitacion.SECCION = _personalidadOld.PERSONALIDAD_FUERO_FEDERAL.PFF_CAPACITACION.SECCION;
                                    _pff_capacitacion.TOTAL_A_B = _personalidadOld.PERSONALIDAD_FUERO_FEDERAL.PFF_CAPACITACION.TOTAL_A_B;
                                    _personalidad_fuero_federal.PFF_CAPACITACION = _pff_capacitacion;
                                }
                                var _pff_criminologico = new PFF_CRIMINOLOGICO();
                                if (_personalidadOld.PERSONALIDAD_FUERO_FEDERAL.PFF_CRIMINOLOGICO != null)
                                {
                                    _pff_criminologico.ANTECEDENTES_PARA_ANTI_SOCIALE = _personalidadOld.PERSONALIDAD_FUERO_FEDERAL.PFF_CRIMINOLOGICO.ANTECEDENTES_PARA_ANTI_SOCIALE;
                                    _pff_criminologico.CRIMINOLOGO = _personalidadOld.PERSONALIDAD_FUERO_FEDERAL.PFF_CRIMINOLOGICO.CRIMINOLOGO;
                                    _pff_criminologico.DIRECTOR_CENTRO = _personalidadOld.PERSONALIDAD_FUERO_FEDERAL.PFF_CRIMINOLOGICO.DIRECTOR_CENTRO;
                                    _pff_criminologico.FECHA = _personalidadOld.PERSONALIDAD_FUERO_FEDERAL.PFF_CRIMINOLOGICO.FECHA;
                                    _pff_criminologico.ID_ANIO = _ingreso_nuevo.ID_ANIO;
                                    _pff_criminologico.ID_CENTRO = _ingreso_nuevo.ID_CENTRO;
                                    _pff_criminologico.ID_ESTUDIO = 1;
                                    _pff_criminologico.ID_IMPUTADO = _ingreso_nuevo.ID_IMPUTADO;
                                    _pff_criminologico.ID_INGRESO = _ingreso_nuevo.ID_INGRESO;
                                    _pff_criminologico.LUGAR = _personalidadOld.PERSONALIDAD_FUERO_FEDERAL.PFF_CRIMINOLOGICO.LUGAR;
                                    _pff_criminologico.NOMBRE = _personalidadOld.PERSONALIDAD_FUERO_FEDERAL.PFF_CRIMINOLOGICO.NOMBRE;
                                    _pff_criminologico.P1_VERSION_INTERNO = _personalidadOld.PERSONALIDAD_FUERO_FEDERAL.PFF_CRIMINOLOGICO.P1_VERSION_INTERNO;
                                    _pff_criminologico.P10_CONTINUAR_NO_ESPECIFICAR = _personalidadOld.PERSONALIDAD_FUERO_FEDERAL.PFF_CRIMINOLOGICO.P10_CONTINUAR_NO_ESPECIFICAR;
                                    _pff_criminologico.P10_CONTINUAR_SI_ESPECIFICAR = _personalidadOld.PERSONALIDAD_FUERO_FEDERAL.PFF_CRIMINOLOGICO.P10_CONTINUAR_SI_ESPECIFICAR;
                                    _pff_criminologico.P10_CONTINUAR_TRATAMIENTO = _personalidadOld.PERSONALIDAD_FUERO_FEDERAL.PFF_CRIMINOLOGICO.P10_CONTINUAR_TRATAMIENTO;
                                    _pff_criminologico.P10_OPINION = _personalidadOld.PERSONALIDAD_FUERO_FEDERAL.PFF_CRIMINOLOGICO.P10_OPINION;
                                    _pff_criminologico.P2_PERSONALIDAD = _personalidadOld.PERSONALIDAD_FUERO_FEDERAL.PFF_CRIMINOLOGICO.P2_PERSONALIDAD;
                                    _pff_criminologico.P3_VALORACION = _personalidadOld.PERSONALIDAD_FUERO_FEDERAL.PFF_CRIMINOLOGICO.P3_VALORACION;
                                    _pff_criminologico.P5_ESPECIFICO = _personalidadOld.PERSONALIDAD_FUERO_FEDERAL.PFF_CRIMINOLOGICO.P5_ESPECIFICO;
                                    _pff_criminologico.P5_GENERICO = _personalidadOld.PERSONALIDAD_FUERO_FEDERAL.PFF_CRIMINOLOGICO.P5_GENERICO;
                                    _pff_criminologico.P5_HABITUAL = _personalidadOld.PERSONALIDAD_FUERO_FEDERAL.PFF_CRIMINOLOGICO.P5_HABITUAL;
                                    _pff_criminologico.P5_PRIMODELINCUENTE = _personalidadOld.PERSONALIDAD_FUERO_FEDERAL.PFF_CRIMINOLOGICO.P5_PRIMODELINCUENTE;
                                    _pff_criminologico.P5_PROFESIONAL = _personalidadOld.PERSONALIDAD_FUERO_FEDERAL.PFF_CRIMINOLOGICO.P5_PROFESIONAL;
                                    _pff_criminologico.P6_CRIMINOGENESIS = _personalidadOld.PERSONALIDAD_FUERO_FEDERAL.PFF_CRIMINOLOGICO.P6_CRIMINOGENESIS;
                                    _pff_criminologico.P7_AGRESIVIDAD = _personalidadOld.PERSONALIDAD_FUERO_FEDERAL.PFF_CRIMINOLOGICO.P7_AGRESIVIDAD;
                                    _pff_criminologico.P7_EGOCENTRISMO = _personalidadOld.PERSONALIDAD_FUERO_FEDERAL.PFF_CRIMINOLOGICO.P7_EGOCENTRISMO;
                                    _pff_criminologico.P7_INDIFERENCIA = _personalidadOld.PERSONALIDAD_FUERO_FEDERAL.PFF_CRIMINOLOGICO.P7_INDIFERENCIA;
                                    _pff_criminologico.P7_LABILIDAD = _personalidadOld.PERSONALIDAD_FUERO_FEDERAL.PFF_CRIMINOLOGICO.P7_LABILIDAD;
                                    _pff_criminologico.P8_ESTADO_PELIGRO = _personalidadOld.PERSONALIDAD_FUERO_FEDERAL.PFF_CRIMINOLOGICO.P8_ESTADO_PELIGRO;
                                    _pff_criminologico.P8_RESULTADO_TRATAMIENTO = _personalidadOld.PERSONALIDAD_FUERO_FEDERAL.PFF_CRIMINOLOGICO.P8_RESULTADO_TRATAMIENTO;
                                    _pff_criminologico.P9_PRONOSTICO = _personalidadOld.PERSONALIDAD_FUERO_FEDERAL.PFF_CRIMINOLOGICO.P9_PRONOSTICO;
                                    _pff_criminologico.SOBRENOMBRE = _personalidadOld.PERSONALIDAD_FUERO_FEDERAL.PFF_CRIMINOLOGICO.SOBRENOMBRE;
                                    _personalidad_fuero_federal.PFF_CRIMINOLOGICO = _pff_criminologico;
                                }
                                var _pff_estudio_medico = new PFF_ESTUDIO_MEDICO();
                                if (_personalidadOld.PERSONALIDAD_FUERO_FEDERAL.PFF_ESTUDIO_MEDICO != null)
                                {
                                    var pff_sustancias_toxicas = new List<PFF_SUSTANCIA_TOXICA>();
                                    foreach (var item_sustancia_toxica in _personalidadOld.PERSONALIDAD_FUERO_FEDERAL.PFF_ESTUDIO_MEDICO.PFF_SUSTANCIA_TOXICA)
                                    {
                                        pff_sustancias_toxicas.Add(new PFF_SUSTANCIA_TOXICA
                                        {
                                            CANTIDAD = item_sustancia_toxica.CANTIDAD,
                                            DROGA = item_sustancia_toxica.DROGA,
                                            EDAD_INICIO = item_sustancia_toxica.EDAD_INICIO,
                                            ID_ANIO = _ingreso_nuevo.ID_ANIO,
                                            ID_CENTRO = _ingreso_nuevo.ID_CENTRO,
                                            ID_DROGA = item_sustancia_toxica.ID_DROGA,
                                            ID_ESTUDIO = 1,
                                            ID_IMPUTADO = _ingreso_nuevo.ID_IMPUTADO,
                                            ID_INGRESO = _ingreso_nuevo.ID_INGRESO,
                                            PERIODICIDAD = item_sustancia_toxica.PERIODICIDAD
                                        });
                                    }
                                    _pff_estudio_medico.ALIAS = _personalidadOld.PERSONALIDAD_FUERO_FEDERAL.PFF_ESTUDIO_MEDICO.ALIAS;
                                    _pff_estudio_medico.ANTE_HEREDO_FAM = _personalidadOld.PERSONALIDAD_FUERO_FEDERAL.PFF_ESTUDIO_MEDICO.ANTE_HEREDO_FAM;
                                    _pff_estudio_medico.ANTE_PATOLOGICOS = _personalidadOld.PERSONALIDAD_FUERO_FEDERAL.PFF_ESTUDIO_MEDICO.ANTE_PATOLOGICOS;
                                    _pff_estudio_medico.ANTE_PERSONAL_NO_PATOLOGICOS = _personalidadOld.PERSONALIDAD_FUERO_FEDERAL.PFF_ESTUDIO_MEDICO.ANTE_PERSONAL_NO_PATOLOGICOS;
                                    _pff_estudio_medico.ASIST_AA = _personalidadOld.PERSONALIDAD_FUERO_FEDERAL.PFF_ESTUDIO_MEDICO.ASIST_AA;
                                    _pff_estudio_medico.ASIST_FARMACODEPENDENCIA = _personalidadOld.PERSONALIDAD_FUERO_FEDERAL.PFF_ESTUDIO_MEDICO.ASIST_FARMACODEPENDENCIA;
                                    _pff_estudio_medico.ASIST_OTROS = _personalidadOld.PERSONALIDAD_FUERO_FEDERAL.PFF_ESTUDIO_MEDICO.ASIST_OTROS;
                                    _pff_estudio_medico.ASIST_OTROS_ESPECIF = _personalidadOld.PERSONALIDAD_FUERO_FEDERAL.PFF_ESTUDIO_MEDICO.ASIST_OTROS_ESPECIF;
                                    _pff_estudio_medico.CONCLUSION = _personalidadOld.PERSONALIDAD_FUERO_FEDERAL.PFF_ESTUDIO_MEDICO.CONCLUSION;
                                    _pff_estudio_medico.DELITO = _personalidadOld.PERSONALIDAD_FUERO_FEDERAL.PFF_ESTUDIO_MEDICO.DELITO;
                                    _pff_estudio_medico.DIAGNOSTICO = _personalidadOld.PERSONALIDAD_FUERO_FEDERAL.PFF_ESTUDIO_MEDICO.DIAGNOSTICO;
                                    _pff_estudio_medico.DIRECTOR_CENTRO = _personalidadOld.PERSONALIDAD_FUERO_FEDERAL.PFF_ESTUDIO_MEDICO.DIRECTOR_CENTRO;
                                    _pff_estudio_medico.EDAD = _personalidadOld.PERSONALIDAD_FUERO_FEDERAL.PFF_ESTUDIO_MEDICO.EDAD;
                                    _pff_estudio_medico.EDO_CIVIL = _personalidadOld.PERSONALIDAD_FUERO_FEDERAL.PFF_ESTUDIO_MEDICO.EDO_CIVIL;
                                    _pff_estudio_medico.ESTATURA = _personalidadOld.PERSONALIDAD_FUERO_FEDERAL.PFF_ESTUDIO_MEDICO.ESTATURA;
                                    _pff_estudio_medico.EXP_FIS_ABDOMEN = _personalidadOld.PERSONALIDAD_FUERO_FEDERAL.PFF_ESTUDIO_MEDICO.EXP_FIS_ABDOMEN;
                                    _pff_estudio_medico.EXP_FIS_CABEZA_CUELLO = _personalidadOld.PERSONALIDAD_FUERO_FEDERAL.PFF_ESTUDIO_MEDICO.EXP_FIS_CABEZA_CUELLO;
                                    _pff_estudio_medico.EXP_FIS_EXTREMIDADES = _personalidadOld.PERSONALIDAD_FUERO_FEDERAL.PFF_ESTUDIO_MEDICO.EXP_FIS_EXTREMIDADES;
                                    _pff_estudio_medico.EXP_FIS_GENITALES = _personalidadOld.PERSONALIDAD_FUERO_FEDERAL.PFF_ESTUDIO_MEDICO.EXP_FIS_GENITALES;
                                    _pff_estudio_medico.EXP_FIS_TORAX = _personalidadOld.PERSONALIDAD_FUERO_FEDERAL.PFF_ESTUDIO_MEDICO.EXP_FIS_TORAX;
                                    _pff_estudio_medico.FECHA = _personalidadOld.PERSONALIDAD_FUERO_FEDERAL.PFF_ESTUDIO_MEDICO.FECHA;
                                    _pff_estudio_medico.ID_ANIO = _ingreso_nuevo.ID_ANIO;
                                    _pff_estudio_medico.ID_CENTRO = _ingreso_nuevo.ID_CENTRO;
                                    _pff_estudio_medico.ID_ESTUDIO = 1;
                                    _pff_estudio_medico.ID_IMPUTADO = _ingreso_nuevo.ID_IMPUTADO;
                                    _pff_estudio_medico.ID_INGRESO = _ingreso_nuevo.ID_INGRESO;
                                    _pff_estudio_medico.INTERROGATORIO_APARATOS = _personalidadOld.PERSONALIDAD_FUERO_FEDERAL.PFF_ESTUDIO_MEDICO.INTERROGATORIO_APARATOS;
                                    _pff_estudio_medico.LUGAR = _personalidadOld.PERSONALIDAD_FUERO_FEDERAL.PFF_ESTUDIO_MEDICO.LUGAR;
                                    _pff_estudio_medico.MEDICO = _personalidadOld.PERSONALIDAD_FUERO_FEDERAL.PFF_ESTUDIO_MEDICO.MEDICO;
                                    _pff_estudio_medico.NOMBRE = _personalidadOld.PERSONALIDAD_FUERO_FEDERAL.PFF_ESTUDIO_MEDICO.NOMBRE;
                                    _pff_estudio_medico.OCUPACION_ACT = _personalidadOld.PERSONALIDAD_FUERO_FEDERAL.PFF_ESTUDIO_MEDICO.OCUPACION_ACT;
                                    _pff_estudio_medico.OCUPACION_ANT = _personalidadOld.PERSONALIDAD_FUERO_FEDERAL.PFF_ESTUDIO_MEDICO.OCUPACION_ANT;
                                    _pff_estudio_medico.ORIGINARIO_DE = _personalidadOld.PERSONALIDAD_FUERO_FEDERAL.PFF_ESTUDIO_MEDICO.ORIGINARIO_DE;
                                    _pff_estudio_medico.PADECIMIENTO_ACTUAL = _personalidadOld.PERSONALIDAD_FUERO_FEDERAL.PFF_ESTUDIO_MEDICO.PADECIMIENTO_ACTUAL;
                                    _pff_estudio_medico.PFF_SUSTANCIA_TOXICA = pff_sustancias_toxicas;
                                    _pff_estudio_medico.PULSO = _personalidadOld.PERSONALIDAD_FUERO_FEDERAL.PFF_ESTUDIO_MEDICO.PULSO;
                                    _pff_estudio_medico.RESPIRACION = _personalidadOld.PERSONALIDAD_FUERO_FEDERAL.PFF_ESTUDIO_MEDICO.RESPIRACION;
                                    _pff_estudio_medico.RESULTADOS_OBTENIDOS = _personalidadOld.PERSONALIDAD_FUERO_FEDERAL.PFF_ESTUDIO_MEDICO.RESULTADOS_OBTENIDOS;
                                    _pff_estudio_medico.SENTENCIA = _personalidadOld.PERSONALIDAD_FUERO_FEDERAL.PFF_ESTUDIO_MEDICO.SENTENCIA;
                                    _pff_estudio_medico.TA = _personalidadOld.PERSONALIDAD_FUERO_FEDERAL.PFF_ESTUDIO_MEDICO.TA;
                                    _pff_estudio_medico.TATUAJES = _personalidadOld.PERSONALIDAD_FUERO_FEDERAL.PFF_ESTUDIO_MEDICO.TATUAJES;
                                    _pff_estudio_medico.TEMPERATURA = _personalidadOld.PERSONALIDAD_FUERO_FEDERAL.PFF_ESTUDIO_MEDICO.TEMPERATURA;
                                    _personalidad_fuero_federal.PFF_ESTUDIO_MEDICO = _pff_estudio_medico;
                                }
                                var _pff_estudio_psicologico = new PFF_ESTUDIO_PSICOLOGICO();
                                if (_personalidadOld.PERSONALIDAD_FUERO_FEDERAL.PFF_ESTUDIO_PSICOLOGICO != null)
                                {
                                    _pff_estudio_psicologico.ACTITUD = _personalidadOld.PERSONALIDAD_FUERO_FEDERAL.PFF_ESTUDIO_PSICOLOGICO.ACTITUD;
                                    _pff_estudio_psicologico.CI = _personalidadOld.PERSONALIDAD_FUERO_FEDERAL.PFF_ESTUDIO_PSICOLOGICO.CI;
                                    _pff_estudio_psicologico.DELITO = _personalidadOld.PERSONALIDAD_FUERO_FEDERAL.PFF_ESTUDIO_PSICOLOGICO.DELITO;
                                    _pff_estudio_psicologico.DINAM_PERSON_ACTUAL = _personalidadOld.PERSONALIDAD_FUERO_FEDERAL.PFF_ESTUDIO_PSICOLOGICO.DINAM_PERSON_ACTUAL;
                                    _pff_estudio_psicologico.DINAM_PERSON_INGRESO = _personalidadOld.PERSONALIDAD_FUERO_FEDERAL.PFF_ESTUDIO_PSICOLOGICO.DINAM_PERSON_INGRESO;
                                    _pff_estudio_psicologico.DIRECTOR_DENTRO = _personalidadOld.PERSONALIDAD_FUERO_FEDERAL.PFF_ESTUDIO_PSICOLOGICO.DIRECTOR_DENTRO;
                                    _pff_estudio_psicologico.EDAD = _personalidadOld.PERSONALIDAD_FUERO_FEDERAL.PFF_ESTUDIO_PSICOLOGICO.EDAD;
                                    _pff_estudio_psicologico.ESPECIFIQUE = _personalidadOld.PERSONALIDAD_FUERO_FEDERAL.PFF_ESTUDIO_PSICOLOGICO.ESPECIFIQUE;
                                    _pff_estudio_psicologico.EXAMEN_MENTAL = _personalidadOld.PERSONALIDAD_FUERO_FEDERAL.PFF_ESTUDIO_PSICOLOGICO.EXAMEN_MENTAL;
                                    _pff_estudio_psicologico.EXTERNO = _personalidadOld.PERSONALIDAD_FUERO_FEDERAL.PFF_ESTUDIO_PSICOLOGICO.EXTERNO;
                                    _pff_estudio_psicologico.FECHA = _personalidadOld.PERSONALIDAD_FUERO_FEDERAL.PFF_ESTUDIO_PSICOLOGICO.FECHA;
                                    _pff_estudio_psicologico.ID_ANIO = _ingreso_nuevo.ID_ANIO;
                                    _pff_estudio_psicologico.ID_CENTRO = _ingreso_nuevo.ID_CENTRO;
                                    _pff_estudio_psicologico.ID_ESTUDIO = 1;
                                    _pff_estudio_psicologico.ID_IMPUTADO = _ingreso_nuevo.ID_IMPUTADO;
                                    _pff_estudio_psicologico.ID_INGRESO = _ingreso_nuevo.ID_INGRESO;
                                    _pff_estudio_psicologico.INDICE_LESION_ORGANICA = _personalidadOld.PERSONALIDAD_FUERO_FEDERAL.PFF_ESTUDIO_PSICOLOGICO.INDICE_LESION_ORGANICA;
                                    _pff_estudio_psicologico.INTERNO = _personalidadOld.PERSONALIDAD_FUERO_FEDERAL.PFF_ESTUDIO_PSICOLOGICO.INTERNO;
                                    _pff_estudio_psicologico.LUGAR = _personalidadOld.PERSONALIDAD_FUERO_FEDERAL.PFF_ESTUDIO_PSICOLOGICO.LUGAR;
                                    _pff_estudio_psicologico.NIVEL_INTELECTUAL = _personalidadOld.PERSONALIDAD_FUERO_FEDERAL.PFF_ESTUDIO_PSICOLOGICO.NIVEL_INTELECTUAL;
                                    _pff_estudio_psicologico.NOMBRE = _personalidadOld.PERSONALIDAD_FUERO_FEDERAL.PFF_ESTUDIO_PSICOLOGICO.NOMBRE;
                                    _pff_estudio_psicologico.OPINION = _personalidadOld.PERSONALIDAD_FUERO_FEDERAL.PFF_ESTUDIO_PSICOLOGICO.OPINION;
                                    _pff_estudio_psicologico.PRONOSTICO_REINTEGRACION = _personalidadOld.PERSONALIDAD_FUERO_FEDERAL.PFF_ESTUDIO_PSICOLOGICO.PRONOSTICO_REINTEGRACION;
                                    _pff_estudio_psicologico.PRUEBAS_APLICADAS = _personalidadOld.PERSONALIDAD_FUERO_FEDERAL.PFF_ESTUDIO_PSICOLOGICO.PRUEBAS_APLICADAS;
                                    _pff_estudio_psicologico.PSICOLOGO = _personalidadOld.PERSONALIDAD_FUERO_FEDERAL.PFF_ESTUDIO_PSICOLOGICO.PSICOLOGO;
                                    _pff_estudio_psicologico.REQ_CONT_TRATAMIENTO = _personalidadOld.PERSONALIDAD_FUERO_FEDERAL.PFF_ESTUDIO_PSICOLOGICO.REQ_CONT_TRATAMIENTO;
                                    _pff_estudio_psicologico.RESULT_TRATAMIENTO = _personalidadOld.PERSONALIDAD_FUERO_FEDERAL.PFF_ESTUDIO_PSICOLOGICO.RESULT_TRATAMIENTO;
                                    _pff_estudio_psicologico.SOBRENOMBRE = _personalidadOld.PERSONALIDAD_FUERO_FEDERAL.PFF_ESTUDIO_PSICOLOGICO.SOBRENOMBRE;
                                    _personalidad_fuero_federal.PFF_ESTUDIO_PSICOLOGICO = _pff_estudio_psicologico;
                                }
                                var _pff_ficha_identificacion = new PFF_FICHA_IDENTIFICACION();
                                if (_personalidadOld.PERSONALIDAD_FUERO_FEDERAL.PFF_FICHA_IDENTIFICACION != null)
                                {
                                    var _pff_filiaciones = new List<PFF_FILIACION>();
                                    foreach (var item_pff_filiacion in _personalidadOld.PERSONALIDAD_FUERO_FEDERAL.PFF_FICHA_IDENTIFICACION.PFF_FILIACION)
                                        _pff_filiaciones.Add(new PFF_FILIACION
                                        {
                                            ESTATUS = item_pff_filiacion.ESTATUS,
                                            ID_ANIO = _ingreso_nuevo.ID_ANIO,
                                            ID_CENTRO = _ingreso_nuevo.ID_CENTRO,
                                            ID_ESTUDIO = 1,
                                            ID_IMPUTADO = _ingreso_nuevo.ID_IMPUTADO,
                                            ID_INGRESO = _ingreso_nuevo.ID_INGRESO,
                                            ID_MEDIA_FILIACION = item_pff_filiacion.ID_MEDIA_FILIACION,
                                            ID_TIPO_FILIACION = item_pff_filiacion.ID_TIPO_FILIACION

                                        });
                                    _pff_ficha_identificacion.A_PARTIR_DE = _personalidadOld.PERSONALIDAD_FUERO_FEDERAL.PFF_FICHA_IDENTIFICACION.A_PARTIR_DE;
                                    _pff_ficha_identificacion.AUTORIDAD_SENTENCIO = _personalidadOld.PERSONALIDAD_FUERO_FEDERAL.PFF_FICHA_IDENTIFICACION.AUTORIDAD_SENTENCIO;
                                    _pff_ficha_identificacion.CAUSA_PENAL = _personalidadOld.PERSONALIDAD_FUERO_FEDERAL.PFF_FICHA_IDENTIFICACION.CAUSA_PENAL;
                                    _pff_ficha_identificacion.COMPLEXION = _personalidadOld.PERSONALIDAD_FUERO_FEDERAL.PFF_FICHA_IDENTIFICACION.COMPLEXION;
                                    _pff_ficha_identificacion.DELITO = _personalidadOld.PERSONALIDAD_FUERO_FEDERAL.PFF_FICHA_IDENTIFICACION.DELITO;
                                    _pff_ficha_identificacion.DELITO_MODALIDAD = _personalidadOld.PERSONALIDAD_FUERO_FEDERAL.PFF_FICHA_IDENTIFICACION.DELITO_MODALIDAD;
                                    _pff_ficha_identificacion.DINAMICA_DELITO = _personalidadOld.PERSONALIDAD_FUERO_FEDERAL.PFF_FICHA_IDENTIFICACION.DINAMICA_DELITO;
                                    _pff_ficha_identificacion.DIRECTOR_CENTRO = _personalidadOld.PERSONALIDAD_FUERO_FEDERAL.PFF_FICHA_IDENTIFICACION.DIRECTOR_CENTRO;
                                    _pff_ficha_identificacion.DORMITORIO_CAMPAMENTO = _personalidadOld.PERSONALIDAD_FUERO_FEDERAL.PFF_FICHA_IDENTIFICACION.DORMITORIO_CAMPAMENTO;
                                    _pff_ficha_identificacion.ESTATURA = _personalidadOld.PERSONALIDAD_FUERO_FEDERAL.PFF_FICHA_IDENTIFICACION.ESTATURA;
                                    _pff_ficha_identificacion.EXP_CENTRO_RECLUSION = _personalidadOld.PERSONALIDAD_FUERO_FEDERAL.PFF_FICHA_IDENTIFICACION.EXP_CENTRO_RECLUSION;
                                    _pff_ficha_identificacion.EXP_PROCESO = _personalidadOld.PERSONALIDAD_FUERO_FEDERAL.PFF_FICHA_IDENTIFICACION.EXP_PROCESO;
                                    _pff_ficha_identificacion.FECHA = _personalidadOld.PERSONALIDAD_FUERO_FEDERAL.PFF_FICHA_IDENTIFICACION.FECHA;
                                    _pff_ficha_identificacion.ID_ANIO = _ingreso_nuevo.ID_ANIO;
                                    _pff_ficha_identificacion.ID_CENTRO = _ingreso_nuevo.ID_CENTRO;
                                    _pff_ficha_identificacion.ID_ESTUDIO = 1;
                                    _pff_ficha_identificacion.ID_IMPUTADO = _ingreso_nuevo.ID_IMPUTADO;
                                    _pff_ficha_identificacion.ID_INGRESO = _ingreso_nuevo.ID_INGRESO;
                                    _pff_ficha_identificacion.INGRESO_FEC = _personalidadOld.PERSONALIDAD_FUERO_FEDERAL.PFF_FICHA_IDENTIFICACION.INGRESO_FEC;
                                    _pff_ficha_identificacion.INGRESOS_ANTERIORES = _personalidadOld.PERSONALIDAD_FUERO_FEDERAL.PFF_FICHA_IDENTIFICACION.INGRESOS_ANTERIORES;
                                    _pff_ficha_identificacion.PESO = _personalidadOld.PERSONALIDAD_FUERO_FEDERAL.PFF_FICHA_IDENTIFICACION.PESO;
                                    _pff_ficha_identificacion.PFF_FILIACION = _pff_filiaciones;
                                    _pff_ficha_identificacion.PROC_PENDIENTE = _personalidadOld.PERSONALIDAD_FUERO_FEDERAL.PFF_FICHA_IDENTIFICACION.PROC_PENDIENTE;
                                    _pff_ficha_identificacion.PROCEDENCIA = _personalidadOld.PERSONALIDAD_FUERO_FEDERAL.PFF_FICHA_IDENTIFICACION.PROCEDENCIA;
                                    _pff_ficha_identificacion.REPARACION_DANIO = _personalidadOld.PERSONALIDAD_FUERO_FEDERAL.PFF_FICHA_IDENTIFICACION.REPARACION_DANIO;
                                    _pff_ficha_identificacion.SENTENCIA = _personalidadOld.PERSONALIDAD_FUERO_FEDERAL.PFF_FICHA_IDENTIFICACION.SENTENCIA;
                                    _pff_ficha_identificacion.SENTENCIA_MULTA = _personalidadOld.PERSONALIDAD_FUERO_FEDERAL.PFF_FICHA_IDENTIFICACION.SENTENCIA_MULTA;
                                    _pff_ficha_identificacion.SEXO = _personalidadOld.PERSONALIDAD_FUERO_FEDERAL.PFF_FICHA_IDENTIFICACION.SEXO;
                                    _personalidad_fuero_federal.PFF_FICHA_IDENTIFICACION = _pff_ficha_identificacion;
                                }
                                var _pff_trabajo_social = new PFF_TRABAJO_SOCIAL();
                                if (_personalidadOld.PERSONALIDAD_FUERO_FEDERAL.PFF_TRABAJO_SOCIAL != null)
                                {
                                    var _pff_grupos_familiares = new List<PFF_GRUPO_FAMILIAR>();
                                    foreach (var item_grupo_familiar in _personalidadOld.PERSONALIDAD_FUERO_FEDERAL.PFF_TRABAJO_SOCIAL.PFF_GRUPO_FAMILIAR)
                                        _pff_grupos_familiares.Add(new PFF_GRUPO_FAMILIAR
                                        {
                                            EDAD = item_grupo_familiar.EDAD,
                                            ESTADO_CIVIL = item_grupo_familiar.ESTADO_CIVIL,
                                            ID_ANIO = _ingreso_nuevo.ID_ANIO,
                                            ID_CENTRO = _ingreso_nuevo.ID_CENTRO,
                                            ID_CONSEC = item_grupo_familiar.ID_CONSEC,
                                            ID_ESTUDIO = 1,
                                            ID_GRUPO_FAMILIAR = item_grupo_familiar.ID_GRUPO_FAMILIAR,
                                            ID_IMPUTADO = _ingreso_nuevo.ID_IMPUTADO,
                                            ID_INGRESO = _ingreso_nuevo.ID_INGRESO,
                                            NOMBRE = item_grupo_familiar.NOMBRE,
                                            OCUPACION = item_grupo_familiar.OCUPACION,
                                            PARENTESCO = item_grupo_familiar.PARENTESCO
                                        });
                                    _pff_trabajo_social.ALIMENTACION_FAM = _personalidadOld.PERSONALIDAD_FUERO_FEDERAL.PFF_TRABAJO_SOCIAL.ALIMENTACION_FAM;
                                    _pff_trabajo_social.APORTACIONES_FAM = _personalidadOld.PERSONALIDAD_FUERO_FEDERAL.PFF_TRABAJO_SOCIAL.APORTACIONES_FAM;
                                    _pff_trabajo_social.APOYO_FAM_OTROS = _personalidadOld.PERSONALIDAD_FUERO_FEDERAL.PFF_TRABAJO_SOCIAL.APOYO_FAM_OTROS;
                                    _pff_trabajo_social.AVAL_MORAL = _personalidadOld.PERSONALIDAD_FUERO_FEDERAL.PFF_TRABAJO_SOCIAL.AVAL_MORAL;
                                    _pff_trabajo_social.AVAL_MORAL_PARENTESCO = _personalidadOld.PERSONALIDAD_FUERO_FEDERAL.PFF_TRABAJO_SOCIAL.AVAL_MORAL_PARENTESCO;
                                    _pff_trabajo_social.CARACT_FP_ANTECE_PENALES_ADIC = _personalidadOld.PERSONALIDAD_FUERO_FEDERAL.PFF_TRABAJO_SOCIAL.CARACT_FP_ANTECE_PENALES_ADIC;
                                    _pff_trabajo_social.CARACT_FP_ANTECEDENTES_PENALES = _personalidadOld.PERSONALIDAD_FUERO_FEDERAL.PFF_TRABAJO_SOCIAL.CARACT_FP_ANTECEDENTES_PENALES;
                                    _pff_trabajo_social.CARACT_FP_CONCEPTO = _personalidadOld.PERSONALIDAD_FUERO_FEDERAL.PFF_TRABAJO_SOCIAL.CARACT_FP_CONCEPTO;
                                    _pff_trabajo_social.CARACT_FP_GRUPO = _personalidadOld.PERSONALIDAD_FUERO_FEDERAL.PFF_TRABAJO_SOCIAL.CARACT_FP_GRUPO;
                                    _pff_trabajo_social.CARACT_FP_NIVEL_SOCIO_CULTURAL = _personalidadOld.PERSONALIDAD_FUERO_FEDERAL.PFF_TRABAJO_SOCIAL.CARACT_FP_NIVEL_SOCIO_CULTURAL;
                                    _pff_trabajo_social.CARACT_FP_RELAC_INTERFAM = _personalidadOld.PERSONALIDAD_FUERO_FEDERAL.PFF_TRABAJO_SOCIAL.CARACT_FP_RELAC_INTERFAM;
                                    _pff_trabajo_social.CARACT_FP_VIOLENCIA_FAM = _personalidadOld.PERSONALIDAD_FUERO_FEDERAL.PFF_TRABAJO_SOCIAL.CARACT_FP_VIOLENCIA_FAM;
                                    _pff_trabajo_social.CARACT_FP_VIOLENCIA_FAM_ESPEFI = _personalidadOld.PERSONALIDAD_FUERO_FEDERAL.PFF_TRABAJO_SOCIAL.CARACT_FP_VIOLENCIA_FAM_ESPEFI;
                                    _pff_trabajo_social.CARACT_FS_GRUPO = _personalidadOld.PERSONALIDAD_FUERO_FEDERAL.PFF_TRABAJO_SOCIAL.CARACT_FS_GRUPO;
                                    _pff_trabajo_social.CARACT_FS_HIJOS_ANT = _personalidadOld.PERSONALIDAD_FUERO_FEDERAL.PFF_TRABAJO_SOCIAL.CARACT_FS_HIJOS_ANT;
                                    _pff_trabajo_social.CARACT_FS_NIVEL_SOCIO_CULTURAL = _personalidadOld.PERSONALIDAD_FUERO_FEDERAL.PFF_TRABAJO_SOCIAL.CARACT_FS_NIVEL_SOCIO_CULTURAL;
                                    _pff_trabajo_social.CARACT_FS_PROBLEMAS_CONDUCTA = _personalidadOld.PERSONALIDAD_FUERO_FEDERAL.PFF_TRABAJO_SOCIAL.CARACT_FS_PROBLEMAS_CONDUCTA;
                                    _pff_trabajo_social.CARACT_FS_PROBLEMAS_CONDUCTA_E = _personalidadOld.PERSONALIDAD_FUERO_FEDERAL.PFF_TRABAJO_SOCIAL.CARACT_FS_PROBLEMAS_CONDUCTA_E;
                                    _pff_trabajo_social.CARACT_FS_RELACION_MEDIO_EXT = _personalidadOld.PERSONALIDAD_FUERO_FEDERAL.PFF_TRABAJO_SOCIAL.CARACT_FS_RELACION_MEDIO_EXT;
                                    _pff_trabajo_social.CARACT_FS_RELACIONES_INTERFAM = _personalidadOld.PERSONALIDAD_FUERO_FEDERAL.PFF_TRABAJO_SOCIAL.CARACT_FS_RELACIONES_INTERFAM;
                                    _pff_trabajo_social.CARACT_FS_VIOLENCIA_INTRAFAM = _personalidadOld.PERSONALIDAD_FUERO_FEDERAL.PFF_TRABAJO_SOCIAL.CARACT_FS_VIOLENCIA_INTRAFAM;
                                    _pff_trabajo_social.CARACT_FS_VIOLENCIA_INTRAFAM_E = _personalidadOld.PERSONALIDAD_FUERO_FEDERAL.PFF_TRABAJO_SOCIAL.CARACT_FS_VIOLENCIA_INTRAFAM_E;
                                    _pff_trabajo_social.CARACT_FS_VIVIEN_DESCRIPCION = _personalidadOld.PERSONALIDAD_FUERO_FEDERAL.PFF_TRABAJO_SOCIAL.CARACT_FS_VIVIEN_DESCRIPCION;
                                    _pff_trabajo_social.CARACT_FS_VIVIEN_MOBILIARIO = _personalidadOld.PERSONALIDAD_FUERO_FEDERAL.PFF_TRABAJO_SOCIAL.CARACT_FS_VIVIEN_MOBILIARIO;
                                    _pff_trabajo_social.CARACT_FS_VIVIEN_NUM_HABITACIO = _personalidadOld.PERSONALIDAD_FUERO_FEDERAL.PFF_TRABAJO_SOCIAL.CARACT_FS_VIVIEN_NUM_HABITACIO;
                                    _pff_trabajo_social.CARACT_FS_VIVIEN_TRANSPORTE = _personalidadOld.PERSONALIDAD_FUERO_FEDERAL.PFF_TRABAJO_SOCIAL.CARACT_FS_VIVIEN_TRANSPORTE;
                                    _pff_trabajo_social.CARACT_FS_ZONA = _personalidadOld.PERSONALIDAD_FUERO_FEDERAL.PFF_TRABAJO_SOCIAL.CARACT_FS_ZONA;
                                    _pff_trabajo_social.DIAG_SOCIAL_PRONOS = _personalidadOld.PERSONALIDAD_FUERO_FEDERAL.PFF_TRABAJO_SOCIAL.DIAG_SOCIAL_PRONOS;
                                    _pff_trabajo_social.DIALECTO = _personalidadOld.PERSONALIDAD_FUERO_FEDERAL.PFF_TRABAJO_SOCIAL.DIALECTO;
                                    _pff_trabajo_social.DIRECTOR_CENTRO = _personalidadOld.PERSONALIDAD_FUERO_FEDERAL.PFF_TRABAJO_SOCIAL.DIRECTOR_CENTRO;
                                    _pff_trabajo_social.DISTRIBUCION_GASTO_FAM = _personalidadOld.PERSONALIDAD_FUERO_FEDERAL.PFF_TRABAJO_SOCIAL.DISTRIBUCION_GASTO_FAM;
                                    _pff_trabajo_social.DOMICILIO = _personalidadOld.PERSONALIDAD_FUERO_FEDERAL.PFF_TRABAJO_SOCIAL.DOMICILIO;
                                    _pff_trabajo_social.ECO_FP_COOPERA_ACTUALMENTE = _personalidadOld.PERSONALIDAD_FUERO_FEDERAL.PFF_TRABAJO_SOCIAL.ECO_FP_COOPERA_ACTUALMENTE;
                                    _pff_trabajo_social.ECO_FP_FONDOS_AHORRO = _personalidadOld.PERSONALIDAD_FUERO_FEDERAL.PFF_TRABAJO_SOCIAL.ECO_FP_FONDOS_AHORRO;
                                    _pff_trabajo_social.ECO_FP_RESPONSABLE = _personalidadOld.PERSONALIDAD_FUERO_FEDERAL.PFF_TRABAJO_SOCIAL.ECO_FP_RESPONSABLE;
                                    _pff_trabajo_social.ECO_FP_TOTAL_EGRESOS_MEN = _personalidadOld.PERSONALIDAD_FUERO_FEDERAL.PFF_TRABAJO_SOCIAL.ECO_FP_TOTAL_EGRESOS_MEN;
                                    _pff_trabajo_social.ECO_FP_TOTAL_INGRESOS_MEN = _personalidadOld.PERSONALIDAD_FUERO_FEDERAL.PFF_TRABAJO_SOCIAL.ECO_FP_TOTAL_INGRESOS_MEN;
                                    _pff_trabajo_social.ECO_FP_ZONA = _personalidadOld.PERSONALIDAD_FUERO_FEDERAL.PFF_TRABAJO_SOCIAL.ECO_FP_ZONA;
                                    _pff_trabajo_social.EDO_CIVIL = _personalidadOld.PERSONALIDAD_FUERO_FEDERAL.PFF_TRABAJO_SOCIAL.EDO_CIVIL;
                                    _pff_trabajo_social.ESCOLARIDAD_ACTUAL = _personalidadOld.PERSONALIDAD_FUERO_FEDERAL.PFF_TRABAJO_SOCIAL.ESCOLARIDAD_ACTUAL;
                                    _pff_trabajo_social.ESCOLARIDAD_CENTRO = _personalidadOld.PERSONALIDAD_FUERO_FEDERAL.PFF_TRABAJO_SOCIAL.ESCOLARIDAD_CENTRO;
                                    _pff_trabajo_social.EXTERNADO_CALLE = _personalidadOld.PERSONALIDAD_FUERO_FEDERAL.PFF_TRABAJO_SOCIAL.EXTERNADO_CALLE;
                                    _pff_trabajo_social.EXTERNADO_CIUDAD = _personalidadOld.PERSONALIDAD_FUERO_FEDERAL.PFF_TRABAJO_SOCIAL.EXTERNADO_CIUDAD;
                                    _pff_trabajo_social.EXTERNADO_COLONIA = _personalidadOld.PERSONALIDAD_FUERO_FEDERAL.PFF_TRABAJO_SOCIAL.EXTERNADO_COLONIA;
                                    _pff_trabajo_social.EXTERNADO_CP = _personalidadOld.PERSONALIDAD_FUERO_FEDERAL.PFF_TRABAJO_SOCIAL.EXTERNADO_CP;
                                    _pff_trabajo_social.EXTERNADO_ENTIDAD = _personalidadOld.PERSONALIDAD_FUERO_FEDERAL.PFF_TRABAJO_SOCIAL.EXTERNADO_ENTIDAD;
                                    _pff_trabajo_social.EXTERNADO_MUNICIPIO = _personalidadOld.PERSONALIDAD_FUERO_FEDERAL.PFF_TRABAJO_SOCIAL.EXTERNADO_MUNICIPIO;
                                    _pff_trabajo_social.EXTERNADO_NUMERO = _personalidadOld.PERSONALIDAD_FUERO_FEDERAL.PFF_TRABAJO_SOCIAL.EXTERNADO_NUMERO;
                                    _pff_trabajo_social.EXTERNADO_PARENTESCO = _personalidadOld.PERSONALIDAD_FUERO_FEDERAL.PFF_TRABAJO_SOCIAL.EXTERNADO_PARENTESCO;
                                    _pff_trabajo_social.EXTERNADO_VIVIR_NOMBRE = _personalidadOld.PERSONALIDAD_FUERO_FEDERAL.PFF_TRABAJO_SOCIAL.EXTERNADO_VIVIR_NOMBRE;
                                    _pff_trabajo_social.FECHA = _personalidadOld.PERSONALIDAD_FUERO_FEDERAL.PFF_TRABAJO_SOCIAL.FECHA;
                                    _pff_trabajo_social.FECHA_NAC = _personalidadOld.PERSONALIDAD_FUERO_FEDERAL.PFF_TRABAJO_SOCIAL.FECHA_NAC;
                                    _pff_trabajo_social.ID_ANIO = _ingreso_nuevo.ID_ANIO;
                                    _pff_trabajo_social.ID_CENTRO = _ingreso_nuevo.ID_CENTRO;
                                    _pff_trabajo_social.ID_ESTUDIO = 1;
                                    _pff_trabajo_social.ID_IMPUTADO = _ingreso_nuevo.ID_IMPUTADO;
                                    _pff_trabajo_social.ID_INGRESO = _ingreso_nuevo.ID_INGRESO;
                                    _pff_trabajo_social.INFLUENCIADO_ESTANCIA_PRISION = _personalidadOld.PERSONALIDAD_FUERO_FEDERAL.PFF_TRABAJO_SOCIAL.INFLUENCIADO_ESTANCIA_PRISION;
                                    _pff_trabajo_social.LUGAR = _personalidadOld.PERSONALIDAD_FUERO_FEDERAL.PFF_TRABAJO_SOCIAL.LUGAR;
                                    _pff_trabajo_social.LUGAR_NAC = _personalidadOld.PERSONALIDAD_FUERO_FEDERAL.PFF_TRABAJO_SOCIAL.LUGAR_NAC;
                                    _pff_trabajo_social.NOMBRE = _personalidadOld.PERSONALIDAD_FUERO_FEDERAL.PFF_TRABAJO_SOCIAL.NOMBRE;
                                    _pff_trabajo_social.NUM_PAREJAS_ESTABLE = _personalidadOld.PERSONALIDAD_FUERO_FEDERAL.PFF_TRABAJO_SOCIAL.NUM_PAREJAS_ESTABLE;
                                    _pff_trabajo_social.OCUPACION_ANT = _personalidadOld.PERSONALIDAD_FUERO_FEDERAL.PFF_TRABAJO_SOCIAL.OCUPACION_ANT;
                                    _pff_trabajo_social.OFERTA_TRABAJO = _personalidadOld.PERSONALIDAD_FUERO_FEDERAL.PFF_TRABAJO_SOCIAL.OFERTA_TRABAJO;
                                    _pff_trabajo_social.OFERTA_TRABAJO_CONSISTE = _personalidadOld.PERSONALIDAD_FUERO_FEDERAL.PFF_TRABAJO_SOCIAL.OFERTA_TRABAJO_CONSISTE;
                                    _pff_trabajo_social.OPINION_CONCESION_BENEFICIOS = _personalidadOld.PERSONALIDAD_FUERO_FEDERAL.PFF_TRABAJO_SOCIAL.OPINION_CONCESION_BENEFICIOS;
                                    _pff_trabajo_social.OPINION_INTERNAMIENTO = _personalidadOld.PERSONALIDAD_FUERO_FEDERAL.PFF_TRABAJO_SOCIAL.OPINION_INTERNAMIENTO;
                                    _pff_trabajo_social.PFF_GRUPO_FAMILIAR = _pff_grupos_familiares;
                                    _pff_trabajo_social.RADICAN_ESTADO = _personalidadOld.PERSONALIDAD_FUERO_FEDERAL.PFF_TRABAJO_SOCIAL.RADICAN_ESTADO;
                                    _pff_trabajo_social.SERVICIOS_PUBLICOS = _personalidadOld.PERSONALIDAD_FUERO_FEDERAL.PFF_TRABAJO_SOCIAL.SERVICIOS_PUBLICOS;
                                    _pff_trabajo_social.SUELDO_PERCIBIDO = _personalidadOld.PERSONALIDAD_FUERO_FEDERAL.PFF_TRABAJO_SOCIAL.SUELDO_PERCIBIDO;
                                    _pff_trabajo_social.TIEMPO_LABORAR = _personalidadOld.PERSONALIDAD_FUERO_FEDERAL.PFF_TRABAJO_SOCIAL.TIEMPO_LABORAR;
                                    _pff_trabajo_social.TRABAJADORA_SOCIAL = _personalidadOld.PERSONALIDAD_FUERO_FEDERAL.PFF_TRABAJO_SOCIAL.TRABAJADORA_SOCIAL;
                                    _pff_trabajo_social.TRABAJO_DESEMP_ANTES = _personalidadOld.PERSONALIDAD_FUERO_FEDERAL.PFF_TRABAJO_SOCIAL.TRABAJO_DESEMP_ANTES;
                                    _pff_trabajo_social.VISITA_FAMILIARES = _personalidadOld.PERSONALIDAD_FUERO_FEDERAL.PFF_TRABAJO_SOCIAL.VISITA_FAMILIARES;
                                    _pff_trabajo_social.VISITA_FRECUENCIA = _personalidadOld.PERSONALIDAD_FUERO_FEDERAL.PFF_TRABAJO_SOCIAL.VISITA_FRECUENCIA;
                                    _pff_trabajo_social.VISITA_OTROS_QUIIEN = _personalidadOld.PERSONALIDAD_FUERO_FEDERAL.PFF_TRABAJO_SOCIAL.VISITA_OTROS_QUIIEN;
                                    _pff_trabajo_social.VISITAS_OTROS = _personalidadOld.PERSONALIDAD_FUERO_FEDERAL.PFF_TRABAJO_SOCIAL.VISITAS_OTROS;
                                    _pff_trabajo_social.VISTA_PARENTESCO = _personalidadOld.PERSONALIDAD_FUERO_FEDERAL.PFF_TRABAJO_SOCIAL.VISTA_PARENTESCO;
                                    _personalidad_fuero_federal.PFF_TRABAJO_SOCIAL = _pff_trabajo_social;
                                }
                                var _pff_vigilancia = new PFF_VIGILANCIA();
                                if (_personalidadOld.PERSONALIDAD_FUERO_FEDERAL.PFF_VIGILANCIA != null)
                                {
                                    var _pff_correctivos = new List<PFF_CORRECTIVO>();
                                    foreach (var item_pff_correctivo in _personalidadOld.PERSONALIDAD_FUERO_FEDERAL.PFF_VIGILANCIA.PFF_CORRECTIVO)
                                        _pff_correctivos.Add(new PFF_CORRECTIVO
                                        {
                                            FECHA = item_pff_correctivo.FECHA,
                                            ID_ANIO = _ingreso_nuevo.ID_ANIO,
                                            ID_CENTRO = _ingreso_nuevo.ID_CENTRO,
                                            ID_CONSEC = item_pff_correctivo.ID_CONSEC,
                                            ID_ESTUDIO = 1,
                                            ID_IMPUTADO = _ingreso_nuevo.ID_IMPUTADO,
                                            ID_INGRESO = _ingreso_nuevo.ID_INGRESO,
                                            MOTIVO = item_pff_correctivo.RESOLUCION
                                        });
                                    _pff_vigilancia.CENTRO_DONDE_PROCEDE = _personalidadOld.PERSONALIDAD_FUERO_FEDERAL.PFF_VIGILANCIA.CENTRO_DONDE_PROCEDE;
                                    _pff_vigilancia.CONCLUSIONES = _personalidadOld.PERSONALIDAD_FUERO_FEDERAL.PFF_VIGILANCIA.CONCLUSIONES;
                                    _pff_vigilancia.CONDUCTA = _personalidadOld.PERSONALIDAD_FUERO_FEDERAL.PFF_VIGILANCIA.CONDUCTA;
                                    _pff_vigilancia.CONDUCTA_FAMILIA = _personalidadOld.PERSONALIDAD_FUERO_FEDERAL.PFF_VIGILANCIA.CONDUCTA_FAMILIA;
                                    _pff_vigilancia.CONDUCTA_GENERAL = _personalidadOld.PERSONALIDAD_FUERO_FEDERAL.PFF_VIGILANCIA.CONDUCTA_GENERAL;
                                    _pff_vigilancia.CONDUCTA_SUPERIORES = _personalidadOld.PERSONALIDAD_FUERO_FEDERAL.PFF_VIGILANCIA.CONDUCTA_SUPERIORES;
                                    _pff_vigilancia.DESCRIPCION_CONDUCTA = _personalidadOld.PERSONALIDAD_FUERO_FEDERAL.PFF_VIGILANCIA.DESCRIPCION_CONDUCTA;
                                    _pff_vigilancia.DIRECTOR_CENTRO = _personalidadOld.PERSONALIDAD_FUERO_FEDERAL.PFF_VIGILANCIA.DIRECTOR_CENTRO;
                                    _pff_vigilancia.ESTIMULOS_BUENA_CONDUCTA = _personalidadOld.PERSONALIDAD_FUERO_FEDERAL.PFF_VIGILANCIA.ESTIMULOS_BUENA_CONDUCTA;
                                    _pff_vigilancia.FECHA = _personalidadOld.PERSONALIDAD_FUERO_FEDERAL.PFF_VIGILANCIA.FECHA;
                                    _pff_vigilancia.FECHA_INGRESO = _personalidadOld.PERSONALIDAD_FUERO_FEDERAL.PFF_VIGILANCIA.FECHA_INGRESO;
                                    _pff_vigilancia.HIGIENE_CELDA = _personalidadOld.PERSONALIDAD_FUERO_FEDERAL.PFF_VIGILANCIA.HIGIENE_CELDA;
                                    _pff_vigilancia.HIGIENE_PERSONAL = _personalidadOld.PERSONALIDAD_FUERO_FEDERAL.PFF_VIGILANCIA.HIGIENE_PERSONAL;
                                    _pff_vigilancia.ID_ANIO = _ingreso_nuevo.ID_ANIO;
                                    _pff_vigilancia.ID_CENTRO = _ingreso_nuevo.ID_CENTRO;
                                    _pff_vigilancia.ID_ESTUDIO = 1;
                                    _pff_vigilancia.ID_IMPUTADO = _ingreso_nuevo.ID_IMPUTADO;
                                    _pff_vigilancia.ID_INGRESO = _ingreso_nuevo.ID_INGRESO;
                                    _pff_vigilancia.JEFE_VIGILANCIA = _personalidadOld.PERSONALIDAD_FUERO_FEDERAL.PFF_VIGILANCIA.JEFE_VIGILANCIA;
                                    _pff_vigilancia.LUGAR = _personalidadOld.PERSONALIDAD_FUERO_FEDERAL.PFF_VIGILANCIA.LUGAR;
                                    _pff_vigilancia.MOTIVO_TRASLADO = _personalidadOld.PERSONALIDAD_FUERO_FEDERAL.PFF_VIGILANCIA.MOTIVO_TRASLADO;
                                    _pff_vigilancia.NOMBRE = _personalidadOld.PERSONALIDAD_FUERO_FEDERAL.PFF_VIGILANCIA.NOMBRE;
                                    _pff_vigilancia.PFF_CORRECTIVO = _pff_correctivos;
                                    _pff_vigilancia.RELACION_COMPANEROS = _personalidadOld.PERSONALIDAD_FUERO_FEDERAL.PFF_VIGILANCIA.RELACION_COMPANEROS;
                                    _pff_vigilancia.VISITA_FRECUENCIA = _personalidadOld.PERSONALIDAD_FUERO_FEDERAL.PFF_VIGILANCIA.VISITA_FRECUENCIA;
                                    _pff_vigilancia.VISITA_QUIENES = _personalidadOld.PERSONALIDAD_FUERO_FEDERAL.PFF_VIGILANCIA.VISITA_QUIENES;
                                    _pff_vigilancia.VISITA_RECIBE = _personalidadOld.PERSONALIDAD_FUERO_FEDERAL.PFF_VIGILANCIA.VISITA_RECIBE;
                                    _personalidad_fuero_federal.PFF_VIGILANCIA = _pff_vigilancia;
                                }
                                _personalidad_fuero_federal.ID_ANIO = _ingreso_nuevo.ID_INGRESO;
                                _personalidad_fuero_federal.ID_CENTRO = _ingreso_nuevo.ID_CENTRO;
                                _personalidad_fuero_federal.ID_ESTUDIO = 1;
                                _personalidad_fuero_federal.ID_IMPUTADO = _ingreso_nuevo.ID_IMPUTADO;
                                _personalidad_fuero_federal.ID_INGRESO = _ingreso_nuevo.ID_INGRESO;
                            }

                            #endregion

                            #region PERSONALIDAD DETALLE
                            var _personalidad_detalle = new List<PERSONALIDAD_DETALLE>();
                            foreach (var item_pd in _personalidadOld.PERSONALIDAD_DETALLE)
                                _personalidad_detalle.Add(new PERSONALIDAD_DETALLE
                                {
                                    DIAS_BONIFICADOS = item_pd.DIAS_BONIFICADOS,
                                    ESTUDIO = item_pd.ESTUDIO,
                                    ID_ANIO = _ingreso_nuevo.ID_ANIO,
                                    ID_CENTRO = _ingreso_nuevo.ID_CENTRO,
                                    ID_DETALLE = item_pd.ID_DETALLE,
                                    ID_ESTATUS = item_pd.ID_ESTATUS,
                                    ID_ESTUDIO = 1,
                                    ID_IMPUTADO = _ingreso_nuevo.ID_IMPUTADO,
                                    ID_INGRESO = _ingreso_nuevo.ID_INGRESO,
                                    ID_TIPO = item_pd.ID_TIPO,
                                    INICIO_FEC = item_pd.INICIO_FEC,
                                    RESULTADO = item_pd.RESULTADO,
                                    SOLICITUD_FEC = item_pd.SOLICITUD_FEC,
                                    TERMINO_FEC = item_pd.TERMINO_FEC,
                                    TIPO_MEDIA = item_pd.TIPO_MEDIA
                                });
                            #endregion

                            var _acta_consejo_tecnico = new ACTA_CONSEJO_TECNICO();
                            if (_personalidadOld.ACTA_CONSEJO_TECNICO!=null)
                            {
                                _acta_consejo_tecnico=new ACTA_CONSEJO_TECNICO{
                                    ACTUACION=_personalidadOld.ACTA_CONSEJO_TECNICO.ACTUACION,
                                    ACUERDO=_personalidadOld.ACTA_CONSEJO_TECNICO.ACUERDO,
                                    AREA_LABORAL=_personalidadOld.ACTA_CONSEJO_TECNICO.AREA_LABORAL,
                                    CRIMINOLOGIA=_personalidadOld.ACTA_CONSEJO_TECNICO.CRIMINOLOGIA,
                                    EDUCATIVO=_personalidadOld.ACTA_CONSEJO_TECNICO.EDUCATIVO,
                                    ID_ANIO=_ingreso_nuevo.ID_ANIO,
                                    ID_CENTRO=_ingreso_nuevo.ID_CENTRO,
                                    ID_ESTUDIO=_personalidadOld.ACTA_CONSEJO_TECNICO.ID_ESTUDIO,
                                    ID_IMPUTADO=_ingreso_nuevo.ID_IMPUTADO,
                                    ID_INGRESO=_ingreso_nuevo.ID_INGRESO,
                                    INTERNO=_personalidadOld.ACTA_CONSEJO_TECNICO.INTERNO,
                                    JURIDICO=_personalidadOld.ACTA_CONSEJO_TECNICO.JURIDICO,
                                    LUGAR=_personalidadOld.ACTA_CONSEJO_TECNICO.LUGAR,
                                    MANIFESTARON=_personalidadOld.ACTA_CONSEJO_TECNICO.MANIFESTARON,
                                    MEDICO=_personalidadOld.ACTA_CONSEJO_TECNICO.MEDICO,
                                    OPINION=_personalidadOld.ACTA_CONSEJO_TECNICO.OPINION,
                                    OPINION_CRIMINOLOGIA=_personalidadOld.ACTA_CONSEJO_TECNICO.OPINION_CRIMINOLOGIA,
                                    OPINION_ESCOLAR=_personalidadOld.ACTA_CONSEJO_TECNICO.OPINION_ESCOLAR,
                                    OPINION_LABORAL=_personalidadOld.ACTA_CONSEJO_TECNICO.OPINION_LABORAL,
                                    OPINION_MEDICO=_personalidadOld.ACTA_CONSEJO_TECNICO.OPINION_MEDICO,
                                    OPINION_PSICOLOGICA=_personalidadOld.ACTA_CONSEJO_TECNICO.OPINION_PSICOLOGICA,
                                    OPINION_SEGURIDAD=_personalidadOld.ACTA_CONSEJO_TECNICO.OPINION_SEGURIDAD,
                                    OPINION_TRABAJO_SOCIAL=_personalidadOld.ACTA_CONSEJO_TECNICO.OPINION_TRABAJO_SOCIAL,
                                    PRESIDENTE=_personalidadOld.ACTA_CONSEJO_TECNICO.PRESIDENTE,
                                    PSICOLOGIA=_personalidadOld.ACTA_CONSEJO_TECNICO.PSICOLOGIA,
                                    SECRETARIO=_personalidadOld.ACTA_CONSEJO_TECNICO.SECRETARIO,
                                    SEGURIDAD_CUSTODIA=_personalidadOld.ACTA_CONSEJO_TECNICO.SEGURIDAD_CUSTODIA,
                                    TRABAJO_SOCIAL=_personalidadOld.ACTA_CONSEJO_TECNICO.TRABAJO_SOCIAL
                                };
                            }

                            Context.PERSONALIDAD.Add(new PERSONALIDAD
                            {

                                ID_ANIO = _ingreso_nuevo.ID_ANIO,
                                ID_CENTRO = _ingreso_nuevo.ID_CENTRO,
                                ID_ESTUDIO = 1,
                                ID_IMPUTADO = _ingreso_nuevo.ID_IMPUTADO,
                                ID_INGRESO = _ingreso_nuevo.ID_INGRESO,
                                ID_MOTIVO = _personalidadOld.ID_MOTIVO,
                                ID_SITUACION = _personalidadOld.ID_SITUACION,
                                INICIO_FEC = _personalidadOld.INICIO_FEC,
                                SOLICITADO = _personalidadOld.SOLICITADO,
                                SOLICITUD_FEC = _personalidadOld.SOLICITUD_FEC,
                                TERMINO_FEC = _personalidadOld.TERMINO_FEC,
                                PERSONALIDAD_DETALLE = _personalidad_detalle,
                                PERSONALIDAD_FUERO_COMUN = _personalidad_fuero_comun,
                                PERSONALIDAD_FUERO_FEDERAL = _personalidad_fuero_federal,
                                PROG_NOMBRE = _personalidadOld.PROG_NOMBRE,
                                NUM_OFICIO = _personalidadOld.NUM_OFICIO,
                                RESULT_ESTUDIO = _personalidadOld.RESULT_ESTUDIO,
                                ACTA_CONSEJO_TECNICO=_acta_consejo_tecnico
                            });
                        }
                        #endregion
                        #region Datos Medicos
                        var _citas_medicas_sin_atender_original = _ingreso.ATENCION_CITA.Where(w=>w.ESTATUS=="N" && w.ID_TIPO_ATENCION.HasValue && (w.ID_TIPO_ATENCION==tipo_atencion_dentista || w.ID_TIPO_ATENCION==tipo_atencion_medica));
                        var _citas_medicas = new List<TV_CITA_MEDICA>();
                        if (_citas_medicas_sin_atender_original != null && _citas_medicas_sin_atender_original.Count() > 0)
                        {
                            
                            foreach (var _item_citas in _citas_medicas_sin_atender_original)
                            {
                                var _id_tv_citas = GetIDProceso<int>("TV_CITA_MEDICA", "ID_TV_CITA", string.Format("ID_CENTRO_UBI={0}", _ingreso_nuevo.ID_UB_CENTRO.Value));
                                Context.TV_CITA_MEDICA.Add(new TV_CITA_MEDICA {
                                    CITA_FECHA_HORA = _item_citas.CITA_FECHA_HORA,
                                    CITA_HORA_TERMINA = _item_citas.CITA_HORA_TERMINA,
                                    ID_ANIO = _ingreso_nuevo.ID_ANIO,
                                    ID_CENTRO = _ingreso_nuevo.ID_CENTRO,
                                    ID_CENTRO_ORIGINAL = _item_citas.ID_CENTRO,
                                    ID_CITA_ORIGINAL = _item_citas.ID_CITA,
                                    ID_IMPUTADO = _ingreso_nuevo.ID_IMPUTADO,
                                    ID_INGRESO=_ingreso_nuevo.ID_INGRESO,
                                    ID_TIPO_ATENCION=_item_citas.ID_TIPO_ATENCION,
                                    ID_TIPO_SERVICIO=_item_citas.ID_TIPO_SERVICIO,
                                    ID_TV_CITA=_id_tv_citas,
                                    ID_TV_MEDICO_ESTATUS=estatus_pendiente_tv_cita_medica,
                                    ID_CENTRO_UBI=_ingreso_nuevo.ID_UB_CENTRO.Value
                                });
                                if (_item_citas.PROC_ATENCION_MEDICA_PROG!=null && _item_citas.PROC_ATENCION_MEDICA_PROG.Count>0)
                                {
                                   foreach(var _item_proc_original in _item_citas.PROC_ATENCION_MEDICA_PROG)
                                   {
                                       Context.TV_PROC_ATENCION_MEDICA_PROG.Add(new TV_PROC_ATENCION_MEDICA_PROG {
                                           ID_CENTRO_UBI=_ingreso_nuevo.ID_UB_CENTRO.Value,
                                           ID_PROCMED=_item_proc_original.ID_PROCMED,
                                           ID_TV_CITA=_id_tv_citas,
                                           OBSERV=_item_proc_original.PROC_ATENCION_MEDICA.OBSERV
                                       });
                                   }
                                }
                                Context.SaveChanges();
                            }
                        }
                        //Estatus validos de solicitud de interconsulta para traslado son "P"=Abierta, "E"=Pendiente de Fecha, "S"="CAPTURADA"
                        var _interconsulta_solicitud_original = _ingreso.ATENCION_MEDICA.Where(w =>w.NOTA_MEDICA!=null && w.NOTA_MEDICA.CANALIZACION!=null && w.NOTA_MEDICA.CANALIZACION.INTERCONSULTA_SOLICITUD != null && w.NOTA_MEDICA.CANALIZACION.INTERCONSULTA_SOLICITUD.Any(a => a.ESTATUS == "P" || a.ESTATUS == "E" || a.ESTATUS == "S")).SelectMany(s =>  
                           s.NOTA_MEDICA.CANALIZACION.INTERCONSULTA_SOLICITUD);
                        if (_interconsulta_solicitud_original!=null && _interconsulta_solicitud_original.Count()>0)
                        {
                            foreach(var _item_interconsulta_original in _interconsulta_solicitud_original)
                            {
                                var _id_tv_intersol=GetIDProceso<int>("TV_INTERCONSULTA_SOLICITUD","ID_TV_INTERSOL",string.Format("ID_CENTRO_UBI={0}",_ingreso_nuevo.ID_UB_CENTRO.Value));
                                Context.TV_INTERCONSULTA_SOLICITUD.Add(new TV_INTERCONSULTA_SOLICITUD {
                                    ESTATUS=_item_interconsulta_original.ESTATUS,
                                    ID_ANIO=_ingreso_nuevo.ID_ANIO,
                                    ID_CENTRO=_ingreso_nuevo.ID_CENTRO,
                                    ID_IMPUTADO=_ingreso_nuevo.ID_IMPUTADO,
                                    ID_INGRESO=_ingreso_nuevo.ID_INGRESO,
                                    ID_ESPECIALIDAD=_item_interconsulta_original.ID_ESPECIALIDAD,
                                    ID_INIVEL=_item_interconsulta_original.ID_INIVEL,
                                    ID_INTER=_item_interconsulta_original.ID_INTER,
                                    ID_INTERAT=_item_interconsulta_original.ID_INTERAT,
                                    ID_TV_INTERSOL=_id_tv_intersol,
                                    ID_TV_MEDICO_ESTATUS=estatus_pendiente_tv_cita_medica,
                                    ID_USUARIO=usuario,
                                    REGISTRO_FEC=fecha,
                                    ID_CENTRO_UBI=_ingreso_nuevo.ID_UB_CENTRO.Value
                                });
                                if (_item_interconsulta_original.HOJA_REFERENCIA_MEDICA!=null && _item_interconsulta_original.HOJA_REFERENCIA_MEDICA.Count==1)
                                {
                                    var _id_hoja = GetIDProceso<int>("TV_HOJA_REFERENCIA_MEDICA", "ID_TV_HOJA", string.Format("ID_CENTRO_UBI={0}",_ingreso_nuevo.ID_UB_CENTRO.Value));
                                    Context.TV_HOJA_REFERENCIA_MEDICA.Add(new TV_HOJA_REFERENCIA_MEDICA { 
                                        EXP_HGT=_item_interconsulta_original.HOJA_REFERENCIA_MEDICA.First().EXP_HGT,
                                        FECHA_CITA=_item_interconsulta_original.HOJA_REFERENCIA_MEDICA.First().FECHA_CITA,
                                        HOSPITAL_OTRO=_item_interconsulta_original.HOJA_REFERENCIA_MEDICA.First().HOSPITAL_OTRO,
                                        ID_CENTRO_UBI=_ingreso_nuevo.ID_UB_CENTRO.Value,
                                        ID_HOSPITAL=_item_interconsulta_original.HOJA_REFERENCIA_MEDICA.First().ID_HOSPITAL,
                                        ID_TIPO_CITA=_item_interconsulta_original.HOJA_REFERENCIA_MEDICA.First().ID_TIPO_CITA,
                                        ID_TV_HOJA=_id_hoja,
                                        ID_TV_INTERSOL = _id_tv_intersol,
                                        MOTIVO=_item_interconsulta_original.HOJA_REFERENCIA_MEDICA.First().MOTIVO,
                                        OBSERV=_item_interconsulta_original.HOJA_REFERENCIA_MEDICA.First().OBSERV
                                    });
                                }
                                if (_item_interconsulta_original.SOL_INTERCONSULTA_INTERNA!=null && _item_interconsulta_original.SOL_INTERCONSULTA_INTERNA.Count==1)
                                {
                                    var _id_tv_int_inter = GetIDProceso<int>("TV_SOL_INTERCONSULTA_INTERNA","ID_TV_INTERSOL", string.Format("ID_CENTRO_UBI={0}", _ingreso_nuevo.ID_UB_CENTRO.Value));
                                    Context.TV_SOL_INTERCONSULTA_INTERNA.Add(new TV_SOL_INTERCONSULTA_INTERNA {
                                        ID_CENTRO= _item_interconsulta_original.SOL_INTERCONSULTA_INTERNA.First().ID_CENTRO,
                                        ID_CENTRO_UBI = _ingreso_nuevo.ID_UB_CENTRO.Value,
                                        ID_TV_INTERSOL = _id_tv_intersol,
                                        ID_TV_SOLICITUD= _id_tv_int_inter,
                                        ID_USUARIO=usuario,
                                        MOTIVO_INTERCONSULTA = _item_interconsulta_original.SOL_INTERCONSULTA_INTERNA.First().MOTIVO_INTERCONSULTA,
                                        REGISTRO_FEC=fecha
                                    });
                                }
                                if (_item_interconsulta_original.SERVICIO_AUX_INTERCONSULTA!=null && _item_interconsulta_original.SERVICIO_AUX_INTERCONSULTA.Count>0)
                                    foreach(var _item_interconsulta_serv_aux_original in _item_interconsulta_original.SERVICIO_AUX_INTERCONSULTA)
                                    {
                                        Context.TV_SERVICIO_AUX_INTERCONSULTA.Add(new TV_SERVICIO_AUX_INTERCONSULTA{
                                            ID_CENTRO_UBI=_ingreso_nuevo.ID_UB_CENTRO.Value,
                                            ID_SERV_AUX=_item_interconsulta_serv_aux_original.ID_SERV_AUX,
                                            ID_TV_INTERSOL=_id_tv_intersol,
                                            REGISTRO_FEC=fecha
                                        });
                                    }
                                Context.SaveChanges();
                            }
                            
                        }
                        //Traslada las canalizaciones pendientes
                        var _AT_canalizaciones_pendientes_original = _ingreso.ATENCION_MEDICA.Where(w =>w.NOTA_MEDICA!=null && w.NOTA_MEDICA.CANALIZACION!=null &&   w.NOTA_MEDICA.CANALIZACION.ID_ESTATUS_CAN == "P");
                        if (_AT_canalizaciones_pendientes_original!=null && _AT_canalizaciones_pendientes_original.Count()>0)
                        {
                            foreach (var _atencion_medica_canalizacion_pend in _AT_canalizaciones_pendientes_original)
                            {
                                var _id_canalizacion = GetIDProceso<int>("TV_CANALIZACION", "ID_TV_CANALIZACION", string.Format("ID_CENTRO_UBI={0}", _ingreso_nuevo.ID_UB_CENTRO));
                                if ((_atencion_medica_canalizacion_pend.NOTA_MEDICA.CANALIZACION.CANALIZACION_ESPECIALIDAD != null && _atencion_medica_canalizacion_pend.NOTA_MEDICA.CANALIZACION.CANALIZACION_ESPECIALIDAD.Any(w => w.ID_ESTATUS == "P")) ||
                                    (_atencion_medica_canalizacion_pend.NOTA_MEDICA.CANALIZACION.CANALIZACION_SERV_AUX != null && _atencion_medica_canalizacion_pend.NOTA_MEDICA.CANALIZACION.CANALIZACION_SERV_AUX.Any(w => w.ID_ESTATUS == "P")))
                                {
                                    Context.TV_CANALIZACION.Add(new TV_CANALIZACION
                                    {
                                        ID_ANIO = _ingreso_nuevo.ID_ANIO,
                                        ID_CENTRO = _ingreso_nuevo.ID_CENTRO,
                                        ID_ESTATUS_CAN = "P",
                                        ID_FECHA = fecha,
                                        ID_IMPUTADO = _ingreso_nuevo.ID_IMPUTADO,
                                        ID_INGRESO = _ingreso_nuevo.ID_INGRESO,
                                        ID_TV_CANALIZACION = _id_canalizacion,
                                        ID_TV_MEDICO_ESTATUS = estatus_pendiente_tv_cita_medica,
                                        ID_USUARIO = usuario,
                                        ID_CENTRO_UBI = _ingreso_nuevo.ID_UB_CENTRO.Value
                                    });
                                    if (_atencion_medica_canalizacion_pend.NOTA_MEDICA.CANALIZACION.CANALIZACION_ESPECIALIDAD != null && _atencion_medica_canalizacion_pend.NOTA_MEDICA.CANALIZACION.CANALIZACION_ESPECIALIDAD.Any(w => w.ID_ESTATUS == "P"))
                                    {
                                        foreach (var _item_canalizacion_especialidad in _atencion_medica_canalizacion_pend.NOTA_MEDICA.CANALIZACION.CANALIZACION_ESPECIALIDAD.Where(w => w.ID_ESTATUS == "P"))
                                            Context.TV_CANALIZACION_ESPECIALIDAD.Add(new TV_CANALIZACION_ESPECIALIDAD
                                            {
                                                ID_CENTRO_UBI = _ingreso_nuevo.ID_UB_CENTRO.Value,
                                                ID_ESPECIALIDAD = _item_canalizacion_especialidad.ID_ESPECIALIDAD,
                                                ID_FECHA = fecha,
                                                ID_INIVEL = _item_canalizacion_especialidad.ID_INIVEL,
                                                ID_INTER = _item_canalizacion_especialidad.ID_INTER,
                                                ID_INTERAT = _item_canalizacion_especialidad.ID_INTERAT,
                                                ID_TV_CANALIZACION = _id_canalizacion,
                                                ID_TV_MEDICO_ESTATUS = estatus_pendiente_tv_cita_medica,
                                            }
                                            );
                                    }

                                    if ((_atencion_medica_canalizacion_pend.NOTA_MEDICA.CANALIZACION.CANALIZACION_SERV_AUX != null && _atencion_medica_canalizacion_pend.NOTA_MEDICA.CANALIZACION.CANALIZACION_SERV_AUX.Any(w => w.ID_ESTATUS == "P")))
                                        foreach (var _item_canalizacion_serv_aux in _atencion_medica_canalizacion_pend.NOTA_MEDICA.CANALIZACION.CANALIZACION_SERV_AUX.Where(w => w.ID_ESTATUS == "P"))
                                            Context.TV_CANALIZACION_SERV_AUX.Add(
                                                new TV_CANALIZACION_SERV_AUX
                                                {
                                                    ID_CENTRO_UBI = _ingreso_nuevo.ID_UB_CENTRO.Value,
                                                    ID_ESTATUS = "P",
                                                    ID_FECHA = fecha,
                                                    ID_INIVEL = _item_canalizacion_serv_aux.ID_INIVEL,
                                                    ID_INTER = _item_canalizacion_serv_aux.ID_INTER,
                                                    ID_SERV_AUX = _item_canalizacion_serv_aux.ID_SERV_AUX,
                                                    ID_TV_CANALIZACION = _id_canalizacion,
                                                    ID_TV_MEDICO_ESTATUS = estatus_pendiente_tv_cita_medica
                                                });
                                    Context.SaveChanges();
                                }
                            }
                        }
                        
                        #endregion
                        #region Control del Internos
                        #region Ubicacion Anterior
                        Context.INGRESO_UBICACION_ANT.Add(new INGRESO_UBICACION_ANT {
                            ID_ANIO=_ingreso_nuevo.ID_ANIO,
                            ID_CENTRO=_ingreso_nuevo.ID_CENTRO,
                            ID_CONSEC=0,
                            ID_IMPUTADO=_ingreso_nuevo.ID_IMPUTADO,
                            ID_INGRESO=_ingreso_nuevo.ID_INGRESO,
                            ID_UB_CAMA=null,
                            ID_UB_CELDA=null,
                            ID_UB_CENTRO=_ingreso_nuevo.ID_UB_CENTRO,
                            ID_UB_EDIFICIO=null,
                            ID_UB_SECTOR=null,
                            MOTIVO_CAMBIO="INGRESO TRASLADO",
                            REGISTRO_FEC=fecha
                        });
                        #endregion
                        var consec = GetIDProceso<int>("INGRESO_UBICACION", "ID_CONSEC", string.Format("ID_CENTRO={0} AND ID_ANIO={1} AND ID_IMPUTADO={2} AND ID_INGRESO={3}", _ingreso_nuevo.ID_CENTRO,
                                _ingreso_nuevo.ID_ANIO, _ingreso_nuevo.ID_IMPUTADO, _ingreso_nuevo.ID_INGRESO));
                        Context.INGRESO_UBICACION.Add(new INGRESO_UBICACION
                        {
                            ESTATUS = 2,
                            ID_ANIO = _ingreso_nuevo.ID_ANIO,
                            ID_AREA = area_traslado,
                            ID_CENTRO = _ingreso_nuevo.ID_CENTRO,
                            ID_CONSEC = consec,
                            ID_IMPUTADO = _ingreso_nuevo.ID_IMPUTADO,
                            ID_INGRESO = _ingreso_nuevo.ID_INGRESO,
                            MOVIMIENTO_FEC = fecha
                        });
                        #endregion
                    }
                    Context.SaveChanges();
                    transaccion.Complete();
                }
            }
            catch (Exception ex)
            {
                throw new ApplicationException(ex.Message + " " + (ex.InnerException != null ? ex.InnerException.InnerException.Message : ""));
            }
        }


        public bool RevertirTraslado(TRASLADO_DETALLE Traslado_Detalle, INCIDENTE Incidente, DateTime FechaServer, INGRESO Ingreso = null)
        {
            try
            {
                using (TransactionScope transaccion = new TransactionScope(TransactionScopeOption.RequiresNew, new TransactionOptions() { IsolationLevel = System.Transactions.IsolationLevel.ReadCommitted }))
                {

                    #region Restauración de Citas Médicas
                    var ListaCitasMedicas = Context.ATENCION_CITA.Where(w =>
                        w.ID_CENTRO == Traslado_Detalle.ID_CENTRO &&
                        w.ID_ANIO == Traslado_Detalle.ID_ANIO &&
                        w.ID_IMPUTADO == Traslado_Detalle.ID_IMPUTADO &&
                        w.ID_INGRESO == Traslado_Detalle.ID_INGRESO &&
                        (w.CITA_FECHA_HORA.HasValue ?
                        (w.CITA_FECHA_HORA.Value.Year >= FechaServer.Year &&
                        w.CITA_FECHA_HORA.Value.Month >= FechaServer.Month &&
                        w.CITA_FECHA_HORA.Value.Day >= FechaServer.Day) : false) &&
                        w.ESTATUS == "C").ToList();
                    foreach (var cita in ListaCitasMedicas)
                    {
                        cita.ESTATUS = "N";
                        Context.ATENCION_CITA.Attach(cita);
                        Context.Entry(cita).Property(x => x.ESTATUS).IsModified = true;
                    }
                    #endregion

                    #region RetornoAEdificio - Restauración de ubicación
                    Context.INGRESO_UBICACION.Add(new INGRESO_UBICACION()
                    {
                        ID_CENTRO = Traslado_Detalle.ID_CENTRO,
                        ID_ANIO = Traslado_Detalle.ID_ANIO,
                        ID_IMPUTADO = Traslado_Detalle.ID_IMPUTADO,
                        ID_INGRESO = Traslado_Detalle.ID_INGRESO,
                        ID_CONSEC = new cIngresoUbicacion().ObtenerConsecutivo<int>(Traslado_Detalle.ID_CENTRO, Traslado_Detalle.ID_ANIO, Traslado_Detalle.ID_IMPUTADO, Traslado_Detalle.ID_INGRESO),
                        ID_AREA = 0,
                        MOVIMIENTO_FEC = FechaServer,
                        ACTIVIDAD = "ESTANCIA",
                        ESTATUS = 0
                    });

                    #endregion

                    #region Cambio Estatus Administrativo
                    if (Ingreso != null)
                    {
                        Context.INGRESO.Attach(Ingreso);
                        Context.Entry(Ingreso).Property(x => x.ID_ESTATUS_ADMINISTRATIVO).IsModified = true;
                    }
                    #endregion

                    //Se indica en el context que la propiedad del registro del traslado del interno ha sido modificado
                    Context.TRASLADO_DETALLE.Attach(Traslado_Detalle);
                    Context.Entry(Traslado_Detalle).Property(x => x.ID_ESTATUS_ADMINISTRATIVO).IsModified = true;
                    //Se restaura el registro del traslado del interno
                    Context.Entry(Traslado_Detalle).Property(x => x.ID_ESTATUS).IsModified = true;
                    Context.Entry(Traslado_Detalle).Property(x => x.ID_INCIDENCIA_TRASLADO).IsModified = true;
                    Context.Entry(Traslado_Detalle).Property(x => x.INCIDENCIA_OBSERVACION).IsModified = true;

                    Context.INCIDENTE.Add(new INCIDENTE()
                    {
                        ID_CENTRO = Incidente.ID_CENTRO,
                        ID_ANIO = Incidente.ID_ANIO,
                        ID_IMPUTADO = Incidente.ID_IMPUTADO,
                        ID_INGRESO = Incidente.ID_INGRESO,
                        ID_INCIDENTE = Incidente.ID_INCIDENTE,
                        ID_INCIDENTE_TIPO = Incidente.ID_INCIDENTE_TIPO,
                        REGISTRO_FEC = Incidente.REGISTRO_FEC,
                        MOTIVO = Incidente.MOTIVO
                    });


                    Context.SaveChanges();
                    transaccion.Complete();
                    return true;
                }
            }
            catch (Exception ex)
            {

                throw new ApplicationException(ex.Message);
            }
        }

        public bool RevertirUltimoMovimiento(INGRESO Ingreso, TRASLADO_DETALLE Traslado_Detalle, INCIDENTE Incidente, DateTime FechaServer)
        {
            try
            {
                //Se inicia una transacción
                using (TransactionScope transaccion = new TransactionScope(TransactionScopeOption.RequiresNew, new TransactionOptions() { IsolationLevel = System.Transactions.IsolationLevel.ReadCommitted }))
                {
                    //Se consultan todas las citas médicas registradas a las que debe acudir el interno, donde:
                    //1.-La cita pertenezca a dicho interno (ID_CENTRO,ID_ANIO,ID_IMPUTADO y ID_INGRESO coincidan con la cita)
                    //2.-La cita tenga una fecha igual o mayor al dia actual
                    //3.-El estatus de la cita
                    var ListaCitasMedicas = Context.ATENCION_CITA.Where(w =>
                        w.ID_CENTRO == Traslado_Detalle.ID_CENTRO &&
                        w.ID_ANIO == Traslado_Detalle.ID_ANIO &&
                        w.ID_IMPUTADO == Traslado_Detalle.ID_IMPUTADO &&
                        w.ID_INGRESO == Traslado_Detalle.ID_INGRESO &&
                        (w.CITA_FECHA_HORA.HasValue ?
                        (w.CITA_FECHA_HORA.Value.Year >= FechaServer.Year &&
                        w.CITA_FECHA_HORA.Value.Month >= FechaServer.Month &&
                        w.CITA_FECHA_HORA.Value.Day >= FechaServer.Day) : false) &&
                        w.ESTATUS == "C").ToList();

                    //Se itera sobre las citas encontradas si es que las hubo
                    foreach (var cita in ListaCitasMedicas)
                    {
                        //Se cambia el estatus de la cita actual en la iteración
                        cita.ESTATUS = "N";
                        //Se indica en el contexto que la propiedad del registro de la cita actual ha sido modificado
                        Context.ATENCION_CITA.Attach(cita);
                        Context.Entry(cita).Property(x => x.ESTATUS).IsModified = true;
                    }

                    //Se indica en el context que la propiedad del registro del ingreso ha sido modificado
                    Context.INGRESO.Attach(Ingreso);
                    Context.Entry(Ingreso).Property(x => x.ID_ESTATUS_ADMINISTRATIVO).IsModified = true;

                    //Se indica en el context que la propiedad del registro del traslado del interno ha sido modificado
                    Context.TRASLADO_DETALLE.Attach(Traslado_Detalle);
                    Context.Entry(Traslado_Detalle).Property(x => x.ID_ESTATUS_ADMINISTRATIVO).IsModified = true;
                    //Se restaura el registro del traslado del interno
                    Context.Entry(Traslado_Detalle).Property(x => x.ID_ESTATUS).IsModified = true;
                    Context.Entry(Traslado_Detalle).Property(x => x.ID_INCIDENCIA_TRASLADO).IsModified = true;
                    Context.Entry(Traslado_Detalle).Property(x => x.INCIDENCIA_OBSERVACION).IsModified = true;
                    Context.INCIDENTE.Add(new INCIDENTE()
                    {
                        ID_CENTRO = Incidente.ID_CENTRO,
                        ID_ANIO = Incidente.ID_ANIO,
                        ID_IMPUTADO = Incidente.ID_IMPUTADO,
                        ID_INGRESO = Incidente.ID_INGRESO,
                        ID_INCIDENTE = Incidente.ID_INCIDENTE,
                        ID_INCIDENTE_TIPO = Incidente.ID_INCIDENTE_TIPO,
                        REGISTRO_FEC = Incidente.REGISTRO_FEC,
                        MOTIVO = Incidente.MOTIVO
                    });


                    //Se guardan los cambios realizados
                    Context.SaveChanges();

                    //Se completa la transacción
                    transaccion.Complete();
                    return true;
                }
            }
            catch (Exception ex)
            {

                throw new ApplicationException(ex.Message);
            }
        }

        public IQueryable<TRASLADO_DETALLE> ObtenerTrasladoInternos(int ID_TRASLADO, short ID_CENTRO_TRASLADO)
        {
            try
            {
                return GetData(g =>
                    g.ID_CENTRO_TRASLADO == ID_CENTRO_TRASLADO &&
                    g.ID_TRASLADO == ID_TRASLADO);
            }
            catch (Exception ex)
            {

                throw new ApplicationException(ex.Message);
            }
        }

        /// <summary>
        /// Activa el registro de traslado de un interno en cuanto a su ingreso actual y de acuerdo a la fecha dada.
        /// </summary>
        /// <param name="Traslado_Detalle">Registro de TRASLADO_DETALLE a modificar</param>
        /// <param name="Ingreso">Registro de INGRESO a modificar</param>
        /// <param name="FechaServer">Fecha del traslado</param>
        /// <returns></returns>
        public bool ProcesarTraslado(TRASLADO_DETALLE Traslado_Detalle, INGRESO Ingreso, DateTime FechaServer) ///===CHECK===
        {
            try
            {
                //Se inicia una transacción
                using (TransactionScope transaccion = new TransactionScope(TransactionScopeOption.RequiresNew, new TransactionOptions() { IsolationLevel = System.Transactions.IsolationLevel.ReadCommitted }))
                {
                    //Se consultan todas las citas médicas registradas a las que debe acudir el interno, donde:
                    //1.-La cita pertenezca a dicho interno (ID_CENTRO,ID_ANIO,ID_IMPUTADO y ID_INGRESO coincidan con la cita)
                    //2.-La cita tenga una fecha igual o mayor al dia actual
                    //3.-El estatus de la cita indique que aun no ha sido atendida
                    var ListaCitasMedicas = Context.ATENCION_CITA.Where(w =>
                        w.ID_CENTRO == Traslado_Detalle.ID_CENTRO &&
                        w.ID_ANIO == Traslado_Detalle.ID_ANIO &&
                        w.ID_IMPUTADO == Traslado_Detalle.ID_IMPUTADO &&
                        w.ID_INGRESO == Traslado_Detalle.ID_INGRESO &&
                        (w.CITA_FECHA_HORA.HasValue ?
                        (w.CITA_FECHA_HORA.Value.Year >= FechaServer.Year &&
                        w.CITA_FECHA_HORA.Value.Month >= FechaServer.Month &&
                        w.CITA_FECHA_HORA.Value.Day >= FechaServer.Day) : false) &&
                        w.ESTATUS == "N").ToList();

                    //Se itera sobre las citas encontradas si es que las hubo
                    foreach (var cita in ListaCitasMedicas)
                    {
                        //Se cambia el estatus de la cita actual en la iteración
                        cita.ESTATUS = "C";
                        //Se indica en el contexto que la propiedad del registro de la cita actual ha sido modificado
                        Context.ATENCION_CITA.Attach(cita);
                        Context.Entry(cita).Property(x => x.ESTATUS).IsModified = true;
                    }

                    //Se indica en el context que la propiedad del registro del ingreso ha sido modificado
                    Context.INGRESO.Attach(Ingreso);
                    Context.Entry(Ingreso).Property(x => x.ID_ESTATUS_ADMINISTRATIVO).IsModified = true;



                    //Se indica en el context que la propiedad del registro del traslado del interno ha sido modificado
                    Context.TRASLADO_DETALLE.Attach(Traslado_Detalle);
                    Context.Entry(Traslado_Detalle).Property(x => x.ID_ESTATUS_ADMINISTRATIVO).IsModified = true;
                    //Se activa el registro del traslado del interno
                    Context.Entry(Traslado_Detalle).Property(x => x.ID_ESTATUS).IsModified = true;
                    Context.Entry(Traslado_Detalle).Property(x => x.EGRESO_FEC).IsModified = true;
                    Context.Entry(Traslado_Detalle).Property(x => x.ID_INCIDENCIA_TRASLADO).IsModified = Traslado_Detalle.ID_INCIDENCIA_TRASLADO.HasValue;
                    Context.Entry(Traslado_Detalle).Property(x => x.INCIDENCIA_OBSERVACION).IsModified = Traslado_Detalle.ID_INCIDENCIA_TRASLADO.HasValue;

                    //Se guardan los cambios realizados
                    Context.SaveChanges();

                    //Se completa la transacción
                    transaccion.Complete();
                    return true;
                }
            }
            catch (Exception ex)
            {
                throw new ApplicationException(ex.Message);
            }
        }

        public bool Actualizar(TRASLADO_DETALLE Entity)
        {
            try
            {
                return Update(Entity);
            }
            catch (Exception ex)
            {
                throw new ApplicationException(ex.Message);
            }
        }

        public IQueryable<TRASLADO_DETALLE> ObtenerTodosTraslado(short? Centro = null)
        {
            try
            {
                var predicate = PredicateBuilder.True<TRASLADO_DETALLE>();
                //if (FechaEntrada != null)
                //    predicate = predicate.And(w => EntityFunctions.TruncateTime(w.ENTRADA_FEC) >= FechaEntrada);
                //if (FechaSalida != null)
                //    predicate = predicate.And(w => EntityFunctions.TruncateTime(w.ENTRADA_FEC) <= FechaSalida);
                if (Centro != null)
                    predicate = predicate.And(w => w.ID_CENTRO == Centro);
                predicate = predicate.And(w => w.INGRESO.ID_ESTATUS_ADMINISTRATIVO != 4 && w.INGRESO.ID_ESTATUS_ADMINISTRATIVO != 5 && w.INGRESO.ID_ESTATUS_ADMINISTRATIVO != 6 && w.INGRESO.ID_ESTATUS_ADMINISTRATIVO != 7);
                //predicate = predicate.And(w => w.ID_ESTATUS == "PR");
                return GetData(predicate.Expand());
            }
            catch (Exception ex)
            {
                throw new ApplicationException(ex.Message);
            }
        }

        public IQueryable<TRASLADO_DETALLE> ObtenerTrasladoTodos(DateTime? FechaEntrada, DateTime? FechaSalida, short? Centro = null)
        {
            try
            {
                var predicate = PredicateBuilder.True<TRASLADO_DETALLE>();
                if (FechaEntrada != null)
                    predicate = predicate.And(w => EntityFunctions.TruncateTime(w.TRASLADO.TRASLADO_FEC) >= FechaEntrada);
                if (FechaSalida != null)
                    predicate = predicate.And(w => EntityFunctions.TruncateTime(w.TRASLADO.TRASLADO_FEC) <= FechaSalida);
                if (Centro != null)
                    predicate = predicate.And(w => w.INGRESO.ID_UB_CENTRO == Centro);
                predicate = predicate.And(w => w.ID_ESTATUS == "PR");
                return GetData(predicate.Expand());
            }
            catch (Exception ex)
            {
                throw new ApplicationException(ex.Message);
            }
        }

        /// <summary>
        /// Obtiene todos los traslados por tipo de movimiento
        /// </summary>
        /// <param name="id_centro">id del centro sobre el cual se realiza la consulta</param>
        /// <param name="tipo_movimiento">I=Ingreso, E=Egreso</param>
        /// <param name="fecha_inicio">fecha de inicial del rango de la consulta</param>
        /// <param name="fecha_fin">fecha final del rango de la consulta</param>
        /// <param name="tipo_traslado">L=Local (entre centros), F=Foraneo (Es hacia o proviene un centro foraneo al estado)</param>
        /// <returns></returns>
        public IQueryable<TRASLADO_DETALLE> ObtenerTodosPorTipo(string tipo_movimiento, short id_centro, DateTime? fecha_inicio = null, DateTime? fecha_fin = null, string tipo_traslado = "")
        {
            try
            {
                var predicate = PredicateBuilder.True<TRASLADO_DETALLE>();
                if (string.IsNullOrWhiteSpace(tipo_movimiento) || (tipo_movimiento != "I" && tipo_movimiento != "E"))
                    throw new Exception("El tipo de movimiento es invalido");
                if (!string.IsNullOrWhiteSpace(tipo_traslado) && tipo_traslado != "L" && tipo_traslado != "F" && tipo_traslado != "T")
                    throw new Exception("El tipo de traslado es invalido");
                predicate = predicate.And(w => w.ID_ESTATUS == "FI");
                if (tipo_movimiento == "E")
                {
                    predicate = predicate.And(w => w.TRASLADO.ID_CENTRO == id_centro);
                    if (fecha_inicio.HasValue)
                        predicate=predicate.And(w => EntityFunctions.TruncateTime(w.TRASLADO.TRASLADO_FEC) >= EntityFunctions.TruncateTime(fecha_inicio));
                    if (fecha_fin.HasValue)
                        predicate=predicate.And(w => EntityFunctions.TruncateTime(w.TRASLADO.TRASLADO_FEC) <= EntityFunctions.TruncateTime(fecha_fin));
                    if (!string.IsNullOrWhiteSpace(tipo_traslado))
                    {
                        if (tipo_traslado == "L")
                            predicate = predicate.And(w => w.TRASLADO.CENTRO_DESTINO != null);
                        else if (tipo_traslado == "F")
                            predicate = predicate.And(w => w.TRASLADO.ID_CENTRO_DESTINO_FORANEO != null);
                        else
                            predicate = predicate.And(w => w.TRASLADO.ID_CENTRO_DESTINO_FORANEO != null || w.TRASLADO.CENTRO_DESTINO != null);
                    }
                }
                else if (tipo_movimiento == "I")
                {
                    if (tipo_traslado == "L")
                        predicate = predicate.And(w => w.TRASLADO.CENTRO_DESTINO == id_centro && w.TRASLADO.ORIGEN_TIPO == "L");
                    else if (tipo_traslado=="F")
                        predicate = predicate.And(w=>w.TRASLADO.ID_CENTRO==id_centro && w.TRASLADO.ORIGEN_TIPO=="F");
                    else if (tipo_traslado=="T")
                        predicate = predicate.And(w => (w.TRASLADO.ID_CENTRO == id_centro && w.TRASLADO.ORIGEN_TIPO == "F") || (w.TRASLADO.CENTRO_DESTINO == id_centro && w.TRASLADO.ORIGEN_TIPO == "L"));
                    if (fecha_inicio.HasValue)
                        predicate=predicate.And(w => EntityFunctions.TruncateTime(w.TRASLADO.TRASLADO_FEC) >= EntityFunctions.TruncateTime(fecha_inicio));
                    if (fecha_fin.HasValue)
                        predicate=predicate.And(w => EntityFunctions.TruncateTime(w.TRASLADO.TRASLADO_FEC) <= EntityFunctions.TruncateTime(fecha_fin));
                }
                return GetData(predicate.Expand());
            }
            catch (Exception ex)
            {
                throw new ApplicationException(ex.Message + " " + (ex.InnerException != null ? ex.InnerException.InnerException.Message : ""));
            }
        }
    }
}
