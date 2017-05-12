using LinqKit;
using SSP.Controlador.Catalogo.Justicia.Ingreso;
using SSP.Modelo;
using SSP.Servidor;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Transactions;

namespace SSP.Controlador.Catalogo.Justicia.Medico.CertificadoMedico
{
    public class cAtencionMedica : EntityManagerServer<ATENCION_MEDICA>
    {
        public ATENCION_MEDICA ObtenerNota(int IdAtencionMedica)
        {
            try
            {
                return GetData().Where(x => x.ID_ATENCION_MEDICA == IdAtencionMedica).SingleOrDefault();
            }

            catch (System.Exception exc)
            {
                throw;
            }
        }

        /// <summary>
        /// Obitene las notas medicas que requieren de hospitalizacion dependiendo de su tipo
        /// </summary>
        /// <param name="FechaMinima">Fecha minima del rango a buscar</param>
        /// <param name="FechaMaxima">Fecha maxima del rango a buscar</param>
        /// <param name="tipo_hospitalizacion">Tipo de Nota Medica de la cual se origina la hospitalizacion</param>
        /// <param name="Anio">Id del año del imputado a buscar</param>
        /// <param name="Folio">Id del folio del imputado a buscar</param>
        /// <param name="Nombre">Nombre del imputado a buscar</param>
        /// <param name="Paterno">Apellido Paterno del imputado a buscar</param>
        /// <param name="Materno">Apellido Materno del imputado a buscar</param>
        /// <returns></returns>
        public IQueryable<ATENCION_MEDICA> ObtenerAtencionesMedicaNotasHospitalizacion(DateTime FechaMinima, DateTime FechaMaxima, short tipo_hospitalizacion, int? Anio=null, int? Folio=null, string Nombre = "", string Paterno = "", string Materno = "")
        {
            try
            {
                var predicate = PredicateBuilder.True<ATENCION_MEDICA>();
                switch (tipo_hospitalizacion)
                {
                    case 1:
                        predicate = predicate.And(a => a.ATENCION_CITA.Any(a2 => !a2.ESPECIALISTA_CITA.Any()));
                        break;
                    case 2:
                        predicate = predicate.And(a => !a.ATENCION_CITA.Any());
                        break;
                    case 3:
                        predicate = predicate.And(a => a.ATENCION_CITA.Any(a2 => a2.ESPECIALISTA_CITA.Any()));
                        break;
                }
                if (Anio.HasValue)
                    predicate = predicate.And(a => a.ID_ANIO == Anio);
                if (Folio.HasValue)
                    predicate = predicate.And(a => a.ID_IMPUTADO == Folio);


                if (!string.IsNullOrWhiteSpace(Nombre))
                    predicate = predicate.And(a =>
                        a.INGRESO.IMPUTADO != null &&
                        a.INGRESO.IMPUTADO.NOMBRE.Contains(Nombre));

                if (!string.IsNullOrWhiteSpace(Paterno))
                    predicate = predicate.And(a =>
                        a.INGRESO.IMPUTADO != null &&
                        a.INGRESO.IMPUTADO.PATERNO.Contains(Paterno));

                if (!string.IsNullOrWhiteSpace(Materno))
                    predicate = predicate.And(a =>
                        a.INGRESO.IMPUTADO != null &&
                        a.INGRESO.IMPUTADO.MATERNO.Contains(Materno));

                predicate = predicate.And(a =>
                    a.NOTA_MEDICA.OCUPA_HOSPITALIZACION == "S" &&
                    a.ATENCION_FEC.HasValue &&
                    a.ATENCION_FEC.Value >= FechaMinima &&
                    a.ATENCION_FEC.Value <= FechaMaxima &&
                    !a.NOTA_MEDICA.HOSPITALIZACION.Any());
                return GetData(predicate.Expand());
            }
            catch (Exception ex)
            {

                throw new ApplicationException(ex.Message);
            }
        }

        public bool Insertar(ATENCION_MEDICA Entity)
        {
            try
            {
                Entity.ID_ATENCION_MEDICA = GetSequence<short>("ATENCION_MEDICA_SEQ");
                if (Insert(Entity))
                    return true;

                return false;
            }

            catch (System.Exception ex)
            {
                throw new System.ApplicationException(ex.Message);
            }
        }

        public bool Actualizar(ATENCION_MEDICA Entity)
        {
            try
            {
                if (Update(Entity))
                    return true;

                return false;
            }

            catch (System.Exception ex)
            {
                throw new System.ApplicationException(ex.Message);
            }
        }

        #region Comprobacion CertificadoMedico
        private bool InsertarAtencionesMedica(ObservableCollection<TRASLADO_DETALLE> ListaTrasDetalle, ObservableCollection<EXCARCELACION> ListaExcarcel, string Procesos)
        {
            cAtencionServicio AtencionServControlador = new cAtencionServicio();
            var Enums = AtencionServControlador.ObtenerTodo().Where(w => w.ID_TIPO_SERVICIO == 3 && w.ID_TIPO_ATENCION == 1).FirstOrDefault();
            switch (Procesos)
            {
                case "TRASLADO":


                    foreach (var itemTrasDetalle in ListaTrasDetalle)
                    {
                        var ObjAtencionMed = new ATENCION_MEDICA();
                        ObjAtencionMed.ID_ANIO = itemTrasDetalle.ID_ANIO;
                        ObjAtencionMed.ID_CENTRO = itemTrasDetalle.ID_CENTRO;
                        ObjAtencionMed.ID_IMPUTADO = itemTrasDetalle.ID_IMPUTADO;
                        ObjAtencionMed.ID_INGRESO = itemTrasDetalle.ID_INGRESO;
                        ObjAtencionMed.ID_TIPO_ATENCION = Enums.ID_TIPO_ATENCION;
                        ObjAtencionMed.ID_TIPO_SERVICIO = Enums.ID_TIPO_SERVICIO;
                        this.Insertar(ObjAtencionMed);
                    }
                    break;
                case "EXCARCELACIÓN":


                    foreach (var itemExcarcelDetalle in ListaExcarcel)
                    {
                        var ObjAtencionMed = new ATENCION_MEDICA();
                        ObjAtencionMed.ID_ANIO = itemExcarcelDetalle.ID_ANIO;
                        ObjAtencionMed.ID_CENTRO = itemExcarcelDetalle.ID_CENTRO;
                        ObjAtencionMed.ID_IMPUTADO = itemExcarcelDetalle.ID_IMPUTADO;
                        ObjAtencionMed.ID_INGRESO = itemExcarcelDetalle.ID_INGRESO;
                        ObjAtencionMed.ID_TIPO_ATENCION = Enums.ID_TIPO_ATENCION;
                        ObjAtencionMed.ID_TIPO_SERVICIO = Enums.ID_TIPO_SERVICIO;
                        this.Insertar(ObjAtencionMed);
                    }
                    break;
                default:
                    break;
            }
            return false;

        }



        public bool Insertar_Comprobacion_CertificadoMedico(List<TRASLADO_DETALLE> ListaTrasDetalle, List<EXCARCELACION> ListaExcarcel, string NameProceso, string TipoCertificadoMedico, short? IdAreaTRaslado, DateTime FechaServer)
        {
            using (TransactionScope transaccion = new TransactionScope(TransactionScopeOption.RequiresNew, new TransactionOptions() { IsolationLevel = System.Transactions.IsolationLevel.ReadCommitted }))
            {
                try
                {
                    cExcarcelacion ExcarcelacionActualizar = new cExcarcelacion();
                    cTrasladoDetalle TrasladoDetalleControlador = new cTrasladoDetalle();
                    cAtencionServicio AtencionServControlador = new cAtencionServicio();
                    cIngresoUbicacion IngresoUbicacionControlador = new cIngresoUbicacion();
                    var Enums = AtencionServControlador.ObtenerTodo().Where(w => w.ID_TIPO_SERVICIO == 3 && w.ID_TIPO_ATENCION == 1).FirstOrDefault();
                    bool OCURRIO_ERROR = false;
                    string ACTIVIDAD = string.Empty;
                    switch (NameProceso)
                    {
                        case "Traslados-Salidas":
                            ACTIVIDAD = "TRASLADO";
                            foreach (var itemTrasDetalle in ListaTrasDetalle)
                            {
                                var ObjAtencionMed = new ATENCION_MEDICA();
                                ObjAtencionMed.ID_ANIO = itemTrasDetalle.ID_ANIO;
                                ObjAtencionMed.ID_CENTRO = itemTrasDetalle.ID_CENTRO;
                                ObjAtencionMed.ID_IMPUTADO = itemTrasDetalle.ID_IMPUTADO;
                                ObjAtencionMed.ID_INGRESO = itemTrasDetalle.ID_INGRESO;
                                ObjAtencionMed.ID_TIPO_ATENCION = Enums.ID_TIPO_ATENCION;
                                ObjAtencionMed.ID_TIPO_SERVICIO = Enums.ID_TIPO_SERVICIO;

                                if (this.Insertar(ObjAtencionMed))
                                {

                                    //Se actualiza campo de ID_ATENCION_MEDICA(comprobacion certificado medico)
                                    itemTrasDetalle.ID_ATENCION_MEDICA = ObjAtencionMed.ID_ATENCION_MEDICA;
                                    if (TrasladoDetalleControlador.Actualizar(new TRASLADO_DETALLE()
                                    {

                                        ID_CENTRO = itemTrasDetalle.ID_CENTRO,
                                        ID_ANIO = itemTrasDetalle.ID_ANIO,
                                        ID_IMPUTADO = itemTrasDetalle.ID_IMPUTADO,
                                        ID_INGRESO = itemTrasDetalle.ID_INGRESO,
                                        ID_ESTATUS = itemTrasDetalle.ID_ESTATUS,
                                        AMP_ID_AMPARO_INDIRECTO = itemTrasDetalle.AMP_ID_AMPARO_INDIRECTO,
                                        AMP_ID_ANIO = itemTrasDetalle.AMP_ID_ANIO,
                                        AMP_ID_CENTRO = itemTrasDetalle.AMP_ID_CENTRO,
                                        AMP_ID_IMPUTADO = itemTrasDetalle.AMP_ID_IMPUTADO,
                                        AMP_ID_INGRESO = itemTrasDetalle.AMP_ID_INGRESO,
                                        CANCELADO_OBSERVA = itemTrasDetalle.CANCELADO_OBSERVA,
                                        EGRESO_FEC = itemTrasDetalle.EGRESO_FEC,
                                        ID_ATENCION_MEDICA = itemTrasDetalle.ID_ATENCION_MEDICA,
                                        ID_CENTRO_TRASLADO = itemTrasDetalle.ID_CENTRO_TRASLADO,
                                        ID_ESTATUS_ADMINISTRATIVO = itemTrasDetalle.ID_ESTATUS_ADMINISTRATIVO,
                                        ID_MOTIVO = itemTrasDetalle.ID_MOTIVO,
                                        ID_TRASLADO = itemTrasDetalle.ID_TRASLADO


                                    }))
                                    {
                                        int IdCpnsecutivo = 0;
                                        int? Id_custodio = null;
                                        var IngresoUbicacionDatos = IngresoUbicacionControlador.ObtenerTodos().Where(w => w.ID_ANIO == itemTrasDetalle.ID_ANIO && w.ID_CENTRO == itemTrasDetalle.ID_CENTRO && w.ID_IMPUTADO == itemTrasDetalle.ID_IMPUTADO && w.ID_INGRESO == itemTrasDetalle.ID_INGRESO);
                                        if (IngresoUbicacionDatos.Count() > 0)//si el ingreso ya tiene registros anteriores Actualizara el ultimo Movimiento 
                                        {//SE ACTUALIZA
                                            var ObjetoUbicacionTras = IngresoUbicacionDatos.Where(w => w.ID_CONSEC == IngresoUbicacionDatos.Max(IdMAX => IdMAX.ID_CONSEC)).ToList().FirstOrDefault();

                                            //ACTUALIZACION DATOS DE UBICACION_DETALLE
                                            Id_custodio = ObjetoUbicacionTras.ID_CUSTODIO;//Revisar que regreso si es null declararlo de inicio como null sio es cero declararlo de inicio como cero

                                            ObjetoUbicacionTras.ESTATUS = 2;//:::::ACTUALIZA ESTATUS
                                            if (!IngresoUbicacionControlador.Actualizar(new INGRESO_UBICACION()
                                            {
                                                ID_ANIO = ObjetoUbicacionTras.ID_ANIO,
                                                MOVIMIENTO_FEC = ObjetoUbicacionTras.MOVIMIENTO_FEC,
                                                ACTIVIDAD = ObjetoUbicacionTras.ACTIVIDAD,
                                                ID_CENTRO = ObjetoUbicacionTras.ID_CENTRO,
                                                ID_CONSEC = ObjetoUbicacionTras.ID_CONSEC,
                                                ID_CUSTODIO = ObjetoUbicacionTras.ID_CUSTODIO,
                                                ID_IMPUTADO = ObjetoUbicacionTras.ID_IMPUTADO,
                                                ID_INGRESO = ObjetoUbicacionTras.ID_INGRESO,
                                                INTERNO_UBICACION = ObjetoUbicacionTras.INTERNO_UBICACION,
                                                ID_AREA = 46,//Valor que se leasigna por el momento
                                                ESTATUS = ObjetoUbicacionTras.ESTATUS
                                            }))
                                            {
                                                OCURRIO_ERROR = true;
                                            }
                                        }

                                        //SE AGREGA UNO NUEVO
                                        if (!IngresoUbicacionControlador.Insertar(new INGRESO_UBICACION()
                                        {
                                            ID_ANIO = itemTrasDetalle.ID_ANIO,
                                            ID_CENTRO = itemTrasDetalle.ID_CENTRO,
                                            ID_IMPUTADO = itemTrasDetalle.ID_IMPUTADO,
                                            ID_INGRESO = itemTrasDetalle.ID_INGRESO,
                                            ID_CONSEC = IngresoUbicacionControlador.ObtenerConsecutivo<int>(itemTrasDetalle.ID_CENTRO, itemTrasDetalle.ID_ANIO, itemTrasDetalle.ID_IMPUTADO, itemTrasDetalle.ID_INGRESO),
                                            MOVIMIENTO_FEC = FechaServer,
                                            ACTIVIDAD = ACTIVIDAD,
                                            ID_AREA = IdAreaTRaslado,
                                            ESTATUS = 1,
                                            ID_CUSTODIO = Id_custodio,
                                        }))
                                        {
                                            OCURRIO_ERROR = true;
                                        }

                                    }
                                }
                                else
                                {
                                    OCURRIO_ERROR = true;
                                }

                            }

                            break;

                        case "EXCARCELACIÓN":
                            foreach (var itemExcarcelDetalle in ListaExcarcel)
                            {
                                var ObjAtencionMed = new ATENCION_MEDICA();
                                ObjAtencionMed.ID_ANIO = itemExcarcelDetalle.ID_ANIO;
                                ObjAtencionMed.ID_CENTRO = itemExcarcelDetalle.ID_CENTRO;
                                ObjAtencionMed.ID_IMPUTADO = itemExcarcelDetalle.ID_IMPUTADO;
                                ObjAtencionMed.ID_INGRESO = itemExcarcelDetalle.ID_INGRESO;
                                ObjAtencionMed.ID_TIPO_ATENCION = Enums.ID_TIPO_ATENCION;
                                ObjAtencionMed.ID_TIPO_SERVICIO = Enums.ID_TIPO_SERVICIO;

                                if (this.Insertar(ObjAtencionMed))
                                {
                                    //Se agrega campo de CERTIFICADO_MEDICO_SALIDA  y se actualiza Tabla excarcelacion
                                    switch (TipoCertificadoMedico)
                                    {
                                        case "SALIDA":
                                            itemExcarcelDetalle.CERT_MEDICO_SALIDA = ObjAtencionMed.ID_ATENCION_MEDICA;
                                            ACTIVIDAD = "EXCARCELACIÓN";
                                            break;
                                        case "RETORNO":
                                            itemExcarcelDetalle.CERT_MEDICO_RETORNO = ObjAtencionMed.ID_ATENCION_MEDICA;
                                            ACTIVIDAD = "ESTANCIA";
                                            break;
                                    }

                                    if (ExcarcelacionActualizar.Actualizar(new EXCARCELACION()
                                    {
                                        ID_ANIO = itemExcarcelDetalle.ID_ANIO,
                                        ID_CENTRO = itemExcarcelDetalle.ID_CENTRO,
                                        ID_IMPUTADO = itemExcarcelDetalle.ID_IMPUTADO,
                                        ID_INGRESO = itemExcarcelDetalle.ID_INGRESO,
                                        ID_CONSEC = itemExcarcelDetalle.ID_CONSEC,
                                        ID_TIPO_EX = itemExcarcelDetalle.ID_TIPO_EX,
                                        ID_ESTATUS = itemExcarcelDetalle.ID_ESTATUS,
                                        ID_USUARIO = itemExcarcelDetalle.ID_USUARIO,
                                        OBSERVACION = itemExcarcelDetalle.OBSERVACION,
                                        PROGRAMADO_FEC = itemExcarcelDetalle.PROGRAMADO_FEC,
                                        REGISTRO_FEC = itemExcarcelDetalle.REGISTRO_FEC,
                                        RETORNO_FEC = itemExcarcelDetalle.RETORNO_FEC,
                                        SALIDA_FEC = itemExcarcelDetalle.SALIDA_FEC,
                                        CANCELADO_TIPO = itemExcarcelDetalle.CANCELADO_TIPO,
                                        CERT_MEDICO_RETORNO = itemExcarcelDetalle.CERT_MEDICO_RETORNO,
                                        CERT_MEDICO_SALIDA = itemExcarcelDetalle.CERT_MEDICO_SALIDA,
                                        CERTIFICADO_MEDICO = itemExcarcelDetalle.CERTIFICADO_MEDICO,


                                    }))
                                    {
                                        int IdCpnsecutivo = 0;
                                        int? Id_custodio = null;
                                        var IngresoUbicacionDatos = IngresoUbicacionControlador.ObtenerTodos().Where(w => w.ID_ANIO == itemExcarcelDetalle.ID_ANIO && w.ID_CENTRO == itemExcarcelDetalle.ID_CENTRO && w.ID_IMPUTADO == itemExcarcelDetalle.ID_IMPUTADO && w.ID_INGRESO == itemExcarcelDetalle.ID_INGRESO);
                                        if (IngresoUbicacionDatos.Count() > 0)
                                        {//SE ACTUALIZA
                                            var ObjetoUnicacionTras = IngresoUbicacionDatos.Where(w => w.ID_CONSEC == IngresoUbicacionDatos.Max(IdMAX => IdMAX.ID_CONSEC)).ToList().FirstOrDefault();

                                            Id_custodio = ObjetoUnicacionTras.ID_CUSTODIO;//Revisar que regreso si es null declararlo de inicio como null sio es cero declararlo de inicio como cero
                                            ObjetoUnicacionTras.ESTATUS = 2;//:::::ACTUALIZA ESTATUS
                                            if (!IngresoUbicacionControlador.Actualizar(new INGRESO_UBICACION()
                                            {
                                                ID_ANIO = ObjetoUnicacionTras.ID_ANIO,
                                                MOVIMIENTO_FEC = ObjetoUnicacionTras.MOVIMIENTO_FEC,
                                                ACTIVIDAD = ObjetoUnicacionTras.ACTIVIDAD,
                                                ID_CENTRO = ObjetoUnicacionTras.ID_CENTRO,
                                                ID_CONSEC = ObjetoUnicacionTras.ID_CONSEC,
                                                ID_CUSTODIO = ObjetoUnicacionTras.ID_CUSTODIO,
                                                ID_IMPUTADO = ObjetoUnicacionTras.ID_IMPUTADO,
                                                ID_INGRESO = ObjetoUnicacionTras.ID_INGRESO,
                                                INTERNO_UBICACION = ObjetoUnicacionTras.INTERNO_UBICACION,
                                                ID_AREA = 46,//Valor que se leasigna por el momento
                                                ESTATUS = ObjetoUnicacionTras.ESTATUS

                                            }))
                                            {
                                                OCURRIO_ERROR = true;
                                            }
                                        }

                                        //SE AGREGA UNO NUEVO
                                        if (!IngresoUbicacionControlador.Insertar(new INGRESO_UBICACION()
                                        {
                                            ID_ANIO = itemExcarcelDetalle.ID_ANIO,
                                            ID_CENTRO = itemExcarcelDetalle.ID_CENTRO,
                                            ID_IMPUTADO = itemExcarcelDetalle.ID_IMPUTADO,
                                            ID_INGRESO = itemExcarcelDetalle.ID_INGRESO,
                                            ID_CONSEC = IngresoUbicacionControlador.ObtenerConsecutivo<int>(itemExcarcelDetalle.ID_CENTRO, itemExcarcelDetalle.ID_ANIO, itemExcarcelDetalle.ID_IMPUTADO, itemExcarcelDetalle.ID_INGRESO),
                                            MOVIMIENTO_FEC = FechaServer,
                                            ACTIVIDAD = ACTIVIDAD,
                                            ID_AREA = IdAreaTRaslado,
                                            ESTATUS = 1,
                                            ID_CUSTODIO = Id_custodio,
                                        }))
                                        {
                                            OCURRIO_ERROR = true;
                                        }



                                    }
                                    else
                                    {
                                        OCURRIO_ERROR = true;
                                    }
                                }
                                else
                                {
                                    OCURRIO_ERROR = true;
                                }

                            }


                            break;

                        default:
                            break;
                    }
                    if (OCURRIO_ERROR == false)
                    {
                        transaccion.Complete();
                        return true;
                    }

                }
                catch (Exception)
                {

                    transaccion.Dispose();
                }


            }
            return true;

        }
        #endregion

        /// <summary>
        /// Metodo que obtiene la ultima atencion medica recibida por el interno
        /// </summary>
        /// <param name="Centro"></param>
        /// <param name="Anio"></param>
        /// <param name="Imputado"></param>
        /// <param name="Ingreso"></param>
        /// <param name="CentroUbicacion"></param>
        /// <returns></returns>
        public ATENCION_MEDICA ObtenerUltimaAtencionCertificadoCentro(short Centro,short Anio,int Imputado,short Ingreso,short CentroUbicacion)
        {
            try
            {
                return GetData().Where(w => w.ID_CENTRO == Centro && 
                    w.ID_ANIO == Anio && 
                    w.ID_IMPUTADO == Imputado && 
                    w.ID_INGRESO == Ingreso && 
                    w.ID_CENTRO_UBI == CentroUbicacion && 
                    w.CERTIFICADO_MEDICO.ES_SANCION == "S").OrderByDescending(w => w.ID_ATENCION_MEDICA).FirstOrDefault();
            }

            catch (System.Exception ex)
            {
                throw new System.ApplicationException(ex.Message);
            }
        }


    }
}
