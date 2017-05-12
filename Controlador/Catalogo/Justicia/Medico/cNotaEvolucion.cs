using SSP.Modelo;
using SSP.Servidor;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LinqKit;
using System.Transactions;
using System.Data.Objects;
using System.Data;

namespace SSP.Controlador.Catalogo.Justicia.Medico
{
    public class cNotaEvolucion : EntityManagerServer<NOTA_EVOLUCION>
    {
        public bool Insertar(NOTA_EVOLUCION Hospitalizacion)
        {
            try
            {
                using (TransactionScope transaccion = new TransactionScope(TransactionScopeOption.RequiresNew, new TransactionOptions() { IsolationLevel = System.Transactions.IsolationLevel.ReadCommitted }))
                {
                    //Context.CAMA_HOSPITAL.Attach(Cama);
                    //Context.Entry(Cama).Property(x => x.ESTATUS).IsModified = true;
                    //Context.HOSPITALIZACION.Add();
                    Context.SaveChanges();
                    transaccion.Complete();
                }
                return true;
            }
            catch (Exception ex)
            {

                throw new ApplicationException(ex.Message);
            }
        }



        public IQueryable<NOTA_EVOLUCION> ObtenerTodas()
        {
            try
            {
                return GetData();
            }
            catch (Exception ex)
            {
                throw new ApplicationException(ex.Message);
            }
        }

        public bool InsertarNotaEvolucion(ATENCION_MEDICA AtencionMedica, NOTA_EVOLUCION NotaEvolucion, NOTA_SIGNOS_VITALES NotaSignosVitales, NOTA_MEDICA NotaMedica, List<NOTA_MEDICA_ENFERMEDAD> Enfermedades = null,
            RECETA_MEDICA Receta = null, List<RECETA_MEDICA_DETALLE> RecetasCanceladas = null, List<RECETA_MEDICA_DETALLE> RecetasAgregadas = null, List<NOTA_MEDICA_DIETA> DietasCanceladas = null,
                List<NOTA_MEDICA_DIETA> DietasPorAgregar = null, List<ATENCION_CITA> ProcedimientosCitas = null, List<HISTORIA_CLINICA_PATOLOGICOS> Patologicos = null, HISTORIA_CLINICA_GINECO_OBSTRE Mujeres = null,
                    short? MensajeEnfermedad = new Nullable<short>())
        {//List<PROC_ATENCION_MEDICA> ProcedimientosPorEditar = null,
            var citatruena = new ATENCION_CITA();
            var citasAgregadas = new List<ATENCION_CITA>();
            var aux = new Nullable<DateTime>();
            var hoy = new DateTime();
            var idCita = 0;
            var responsable = 0;
            try
            {
                using (TransactionScope transaccion = new TransactionScope(TransactionScopeOption.RequiresNew, new TransactionOptions() { IsolationLevel = System.Transactions.IsolationLevel.ReadCommitted }))
                {
                    AtencionMedica.ID_ATENCION_MEDICA = GetIDProceso<short>("ATENCION_MEDICA", "ID_ATENCION_MEDICA", "ID_CENTRO_UBI = " + AtencionMedica.ID_CENTRO_UBI);
                    Context.ATENCION_MEDICA.Add(AtencionMedica);
                    Context.SaveChanges();

                    NotaMedica.ID_ATENCION_MEDICA = AtencionMedica.ID_ATENCION_MEDICA;
                    Context.NOTA_MEDICA.Add(NotaMedica);
                    Context.SaveChanges();

                    NotaSignosVitales.ID_ATENCION_MEDICA = AtencionMedica.ID_ATENCION_MEDICA;
                    Context.NOTA_SIGNOS_VITALES.Add(NotaSignosVitales);
                    Context.SaveChanges();

                    NotaEvolucion.ID_ATENCION_MEDICA = AtencionMedica.ID_ATENCION_MEDICA;
                    Context.NOTA_EVOLUCION.Add(NotaEvolucion);
                    Context.SaveChanges();

                    #region ENFERMEDADES
                    var consec = Context.HISTORIA_CLINICA.First(f => f.ID_CENTRO == AtencionMedica.ID_CENTRO && f.ID_ANIO == AtencionMedica.ID_ANIO && f.ID_IMPUTADO == AtencionMedica.ID_IMPUTADO && f.ID_INGRESO == AtencionMedica.ID_INGRESO);
                    hoy = GetFechaServerDate();
                    var banderaEnfermedades = false;
                    var gruposString = string.Empty;
                    var gruposIDs = new List<short>();
                    if (Enfermedades != null ? Enfermedades.Any() : false)
                    {
                        foreach (var item in Enfermedades)
                        {
                            Context.NOTA_MEDICA_ENFERMEDAD.Add(new NOTA_MEDICA_ENFERMEDAD
                            {
                                ID_ATENCION_MEDICA = AtencionMedica.ID_ATENCION_MEDICA,
                                ID_CENTRO_UBI = AtencionMedica.ID_CENTRO_UBI,
                                ID_ENFERMEDAD = item.ID_ENFERMEDAD,
                                REGISTRO_FEC = item.REGISTRO_FEC
                            });
                            banderaEnfermedades = banderaEnfermedades ? banderaEnfermedades : item.ENFERMEDAD != null ? item.ENFERMEDAD.SECTOR_CLASIFICACION.Any() : false;
                        }
                        if (banderaEnfermedades)
                        {
                            var i = 1;
                            foreach (var item in Enfermedades.SelectMany(s => s.ENFERMEDAD.SECTOR_CLASIFICACION))
                            {
                                //grupos = grupos + item.POBLACION + (i == Enfermedades.SelectMany(s => s.ENFERMEDAD.SECTOR_CLASIFICACION).Count() ? "" : ", ");
                                i++;
                                if (consec != null)
                                {
                                    if (!Context.GRUPO_VULNERABLE.Any(w => w.ID_CENTRO == AtencionMedica.ID_CENTRO && w.ID_ANIO == AtencionMedica.ID_ANIO && w.ID_IMPUTADO == AtencionMedica.ID_IMPUTADO && w.ID_INGRESO == AtencionMedica.ID_INGRESO &&
                                        w.ID_CONSEC == consec.ID_CONSEC && w.ID_SECTOR_CLAS == item.ID_SECTOR_CLAS))
                                    {
                                        Context.GRUPO_VULNERABLE.Add(new GRUPO_VULNERABLE
                                        {
                                            REGISTRO_FEC = hoy,
                                            MOMENTO_DETECCION = "DI",
                                            ID_CONSEC = consec.ID_CONSEC,
                                            ID_SECTOR_CLAS = item.ID_SECTOR_CLAS,
                                            ID_ANIO = AtencionMedica.ID_ANIO.Value,
                                            ID_CENTRO = AtencionMedica.ID_CENTRO.Value,
                                            ID_INGRESO = AtencionMedica.ID_INGRESO.Value,
                                            ID_IMPUTADO = AtencionMedica.ID_IMPUTADO.Value,
                                            ID_GRUPO_CONSEC = GetIDProceso<short>("GRUPO_VULNERABLE", "ID_CONSEC", "ID_CENTRO = " + AtencionMedica.ID_CENTRO + " AND ID_ANIO = " + AtencionMedica.ID_ANIO + " AND ID_IMPUTADO = " +
                                                AtencionMedica.ID_IMPUTADO + " AND ID_INGRESO = " + AtencionMedica.ID_INGRESO + " AND ID_CONSEC = " + consec.ID_CONSEC),
                                        });
                                        Context.SaveChanges();
                                        if (!gruposIDs.Any(a => a == item.ID_SECTOR_CLAS))
                                        {
                                            gruposIDs.Add(item.ID_SECTOR_CLAS);
                                            gruposString = gruposString + (string.IsNullOrEmpty(gruposString) ? "" : ", ") + item.POBLACION;//+(i == item.PATOLOGICO_CAT.SECTOR_CLASIFICACION.Count() ? "" : ", ");
                                            i++;
                                        }
                                    }
                                }
                            }
                            /*var _mensaje_tipo = Context.MENSAJE_TIPO.FirstOrDefault(w => w.ID_MEN_TIPO == MensajeEnfermedad);
                            var _imputado = Context.IMPUTADO.First(w => w.ID_ANIO == AtencionMedica.ID_ANIO && w.ID_CENTRO == AtencionMedica.ID_CENTRO && w.ID_IMPUTADO == AtencionMedica.ID_IMPUTADO);
                            var _nombre_completo = string.Format("{0} {1} {2}", (!string.IsNullOrEmpty(_imputado.NOMBRE) ? _imputado.NOMBRE.Trim() : string.Empty),
                                (!string.IsNullOrEmpty(_imputado.PATERNO) ? _imputado.PATERNO.Trim() : string.Empty),
                                (!string.IsNullOrEmpty(_imputado.MATERNO) ? _imputado.MATERNO.Trim() : string.Empty));
                            var _id_mensaje = GetIDProceso<int>("MENSAJE", "ID_MENSAJE", "1=1");
                            var _contenido = _mensaje_tipo.CONTENIDO + " " + _nombre_completo + " CON FOLIO " +
                                string.Format("{0}/{1}", AtencionMedica.ID_ANIO, AtencionMedica.ID_IMPUTADO) +
                                    " QUE PERTENE A LOS SIGUIENTES GRUPOS VULNERABLES: \n" + grupos;
                            hoy = GetFechaServerDate();
                            var _mensaje = new MENSAJE
                            {
                                CONTENIDO = _contenido,
                                ENCABEZADO = _mensaje_tipo.ENCABEZADO,
                                ID_MEN_TIPO = _mensaje_tipo.ID_MEN_TIPO,
                                ID_MENSAJE = _id_mensaje,
                                REGISTRO_FEC = hoy,
                                ID_CENTRO = AtencionMedica.ID_CENTRO_UBI
                            };
                            Context.MENSAJE.Add(_mensaje);*/
                        }
                        Context.SaveChanges();
                    }
                    #endregion

                    #region PATOLOGICOS
                    if (Patologicos != null ? Patologicos.Any() : false)
                        foreach (var item in Patologicos)
                        {
                            if (item.ID_CONSEC == 0)
                            {
                                item.ID_CONSEC = Context.HISTORIA_CLINICA.First(w => w.ID_CENTRO == item.ID_CENTRO && w.ID_ANIO == item.ID_ANIO && w.ID_IMPUTADO == item.ID_IMPUTADO && w.ID_INGRESO == item.ID_INGRESO).ID_CONSEC;
                                Context.HISTORIA_CLINICA_PATOLOGICOS.Add(item);
                                Context.SaveChanges();
                                if (consec != null)
                                {
                                    item.PATOLOGICO_CAT = Context.PATOLOGICO_CAT.Any(a => a.ID_PATOLOGICO == item.ID_PATOLOGICO) ? Context.PATOLOGICO_CAT.First(f => f.ID_PATOLOGICO == item.ID_PATOLOGICO) : null;
                                    if (item.PATOLOGICO_CAT != null)
                                    {
                                        var i = 0;
                                        foreach (var itm in item.PATOLOGICO_CAT.SECTOR_CLASIFICACION)
                                        {
                                            if (!Context.GRUPO_VULNERABLE.Any(w => w.ID_CENTRO == item.ID_CENTRO && w.ID_ANIO == item.ID_ANIO && w.ID_IMPUTADO == item.ID_IMPUTADO && w.ID_INGRESO == item.ID_INGRESO &&
                                                w.ID_CONSEC == consec.ID_CONSEC && w.ID_SECTOR_CLAS == itm.ID_SECTOR_CLAS))
                                            {
                                                Context.GRUPO_VULNERABLE.Add(new GRUPO_VULNERABLE
                                                {
                                                    ID_ANIO = AtencionMedica.ID_ANIO.Value,
                                                    ID_CENTRO = AtencionMedica.ID_CENTRO.Value,
                                                    ID_IMPUTADO = AtencionMedica.ID_IMPUTADO.Value,
                                                    ID_INGRESO = AtencionMedica.ID_INGRESO.Value,
                                                    ID_SECTOR_CLAS = itm.ID_SECTOR_CLAS,
                                                    MOMENTO_DETECCION = "DI",
                                                    REGISTRO_FEC = hoy,
                                                    ID_GRUPO_CONSEC = GetIDProceso<short>("GRUPO_VULNERABLE", "ID_CONSEC", "ID_CENTRO = " + AtencionMedica.ID_CENTRO + " AND ID_ANIO = " + AtencionMedica.ID_ANIO + " AND ID_IMPUTADO = " +
                                                              AtencionMedica.ID_IMPUTADO + " AND ID_INGRESO = " + AtencionMedica.ID_INGRESO + " AND ID_CONSEC = " + consec.ID_CONSEC),
                                                    ID_CONSEC = consec.ID_CONSEC,
                                                });
                                                Context.SaveChanges();
                                                if (!gruposIDs.Any(a => a == itm.ID_SECTOR_CLAS))
                                                {
                                                    gruposIDs.Add(itm.ID_SECTOR_CLAS);
                                                    gruposString = gruposString + (string.IsNullOrEmpty(gruposString) ? "" : ", ") + itm.POBLACION;//+(i == item.PATOLOGICO_CAT.SECTOR_CLASIFICACION.Count() ? "" : ", ");
                                                    i++;
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                            else
                            {
                                var historia = Context.HISTORIA_CLINICA_PATOLOGICOS.First(w => w.ID_CENTRO == item.ID_CENTRO && w.ID_ANIO == item.ID_ANIO && w.ID_IMPUTADO == item.ID_IMPUTADO && w.ID_INGRESO == item.ID_INGRESO &&
                                    w.ID_CONSEC == item.ID_CONSEC && w.ID_PATOLOGICO == item.ID_PATOLOGICO && w.ID_NOPATOLOGICO == item.ID_NOPATOLOGICO);
                                if (historia != null ? ((historia.RECUPERADO != "S" && item.RECUPERADO == "S") || historia.OBSERVACIONES != item.OBSERVACIONES) : false)
                                {
                                    //item.RECUPERACION_FEC = GetFechaServerDate();
                                    //Context.Entry(item).State = EntityState.Modified;
                                    Context.Entry(historia).CurrentValues.SetValues(item);
                                    Context.SaveChanges();
                                }
                            }
                        }
                    #endregion

                    #region MENSAJE
                    if (string.IsNullOrEmpty(gruposString) ? false : !string.IsNullOrEmpty(gruposString.Trim()))
                    {
                        var _mensaje_tipo = Context.MENSAJE_TIPO.FirstOrDefault(w => w.ID_MEN_TIPO == MensajeEnfermedad);
                        var _imputado = Context.IMPUTADO.First(w => w.ID_ANIO == AtencionMedica.ID_ANIO && w.ID_CENTRO == AtencionMedica.ID_CENTRO && w.ID_IMPUTADO == AtencionMedica.ID_IMPUTADO);
                        var _nombre_completo = string.Format("{0} {1} {2}", (!string.IsNullOrEmpty(_imputado.NOMBRE) ? _imputado.NOMBRE.Trim() : string.Empty),
                            (!string.IsNullOrEmpty(_imputado.PATERNO) ? _imputado.PATERNO.Trim() : string.Empty),
                            (!string.IsNullOrEmpty(_imputado.MATERNO) ? _imputado.MATERNO.Trim() : string.Empty));
                        var _id_mensaje = GetIDProceso<int>("MENSAJE", "ID_MENSAJE", "1=1");
                        var _contenido = _mensaje_tipo.CONTENIDO + " " + _nombre_completo + " CON FOLIO " +
                            string.Format("{0}/{1}", AtencionMedica.ID_ANIO, AtencionMedica.ID_IMPUTADO) +
                                " QUE PERTENE A LOS SIGUIENTES GRUPOS VULNERABLES: \n" + gruposString;
                        hoy = GetFechaServerDate();
                        var _mensaje = new MENSAJE
                        {
                            CONTENIDO = _contenido,
                            ENCABEZADO = _mensaje_tipo.ENCABEZADO,
                            ID_MEN_TIPO = _mensaje_tipo.ID_MEN_TIPO,
                            ID_MENSAJE = _id_mensaje,
                            REGISTRO_FEC = hoy,
                            ID_CENTRO = AtencionMedica.ID_CENTRO_UBI
                        };
                        Context.MENSAJE.Add(_mensaje);
                        Context.SaveChanges();
                    }
                    #endregion

                    #region RECETAS
                    if (RecetasCanceladas != null ? RecetasCanceladas.Any() : false)
                    {
                        foreach (var item in RecetasCanceladas)
                        {
                            item.CANCELA_ID_ATENCION_MEDICA = AtencionMedica.ID_ATENCION_MEDICA;
                            item.CANCELA_ID_CENTRO_UBI = AtencionMedica.ID_CENTRO_UBI;
                            Context.Entry(item).State = EntityState.Modified;
                        }
                        Context.SaveChanges();
                    }

                    if (RecetasAgregadas != null ? RecetasAgregadas.Any() : false)
                    {
                        Receta.ID_FOLIO = GetIDProceso<short>("RECETA_MEDICA", "ID_FOLIO", "ID_CENTRO_UBI = " + AtencionMedica.ID_CENTRO_UBI);
                        Receta.ID_ATENCION_MEDICA = AtencionMedica.ID_ATENCION_MEDICA;
                        Context.RECETA_MEDICA.Add(Receta);
                        Context.SaveChanges();
                        foreach (var item in RecetasAgregadas)
                        {
                            item.ID_ATENCION_MEDICA = AtencionMedica.ID_ATENCION_MEDICA;
                            item.ID_FOLIO = Receta.ID_FOLIO;
                            Context.RECETA_MEDICA_DETALLE.Add(item);
                        }
                        Context.SaveChanges();
                    }
                    #endregion

                    #region DIETAS
                    if (DietasCanceladas != null ? DietasCanceladas.Any() : false)
                    {
                        foreach (var item in DietasCanceladas)
                        {
                            Context.Entry(item).State = EntityState.Modified;
                        }
                        Context.SaveChanges();
                    }
                    if (DietasPorAgregar != null ? DietasPorAgregar.Any() : false)
                    {
                        foreach (var item in DietasPorAgregar)
                        {
                            item.ID_ATENCION_MEDICA = AtencionMedica.ID_ATENCION_MEDICA;
                            Context.NOTA_MEDICA_DIETA.Add(item);
                        }
                        Context.SaveChanges();
                    }
                    #endregion

                    #region PROCEDIMIENTOS MEDICOS
                    var auxiliarProcs = new List<PROC_ATENCION_MEDICA>();
                    if (ProcedimientosCitas != null ? ProcedimientosCitas.Any() : false)
                    {
                        foreach (var item in ProcedimientosCitas)
                        {
                            if (item.ID_CENTRO_AT_SOL == null)
                                item.ID_CENTRO_AT_SOL = item.ID_CENTRO_UBI;
                            if (!aux.HasValue)
                            {
                                citatruena = new ATENCION_CITA
                                {
                                    ID_USUARIO = item.ID_USUARIO,
                                    ID_TIPO_SERVICIO = item.ID_TIPO_SERVICIO,
                                    ID_TIPO_ATENCION = item.ID_TIPO_ATENCION,
                                    ID_RESPONSABLE = item.ID_RESPONSABLE,
                                    ID_INGRESO = item.ID_INGRESO,
                                    ID_IMPUTADO = item.ID_IMPUTADO,
                                    ID_CENTRO = item.ID_CENTRO,
                                    ID_AREA = item.ID_AREA,
                                    ID_ANIO = item.ID_ANIO,
                                    CITA_HORA_TERMINA = item.CITA_HORA_TERMINA,
                                    CITA_FECHA_HORA = item.CITA_FECHA_HORA,
                                    ESTATUS = item.ESTATUS,
                                    ID_CITA = item.ID_CITA,
                                    ID_CENTRO_UBI = item.ID_CENTRO_UBI,
                                    ID_ATENCION_MEDICA = item.ID_ATENCION_MEDICA,
                                    ID_ATENCION = item.ID_ATENCION,
                                    ID_CENTRO_AT_SOL = item.ID_CENTRO_AT_SOL,
                                };
                                citatruena.ID_CITA = GetIDProceso<short>("ATENCION_CITA", "ID_CITA", "ID_CENTRO_UBI = " + AtencionMedica.ID_CENTRO_UBI);
                                Context.ATENCION_CITA.Add(citatruena);
                                Context.SaveChanges();
                                citasAgregadas.Add(citatruena);
                                aux = citatruena.CITA_FECHA_HORA;
                                idCita = citatruena.ID_CITA;
                                responsable = item.ID_RESPONSABLE.HasValue ? item.ID_RESPONSABLE.Value : 0;
                            }
                            else if (!(item.CITA_FECHA_HORA.Value.Year == aux.Value.Year && item.CITA_FECHA_HORA.Value.Month == aux.Value.Month && item.CITA_FECHA_HORA.Value.Day == aux.Value.Day &&
                            item.CITA_FECHA_HORA.Value.Hour == aux.Value.Hour && item.CITA_FECHA_HORA.Value.Minute == aux.Value.Minute) ?
                                true
                            : !citasAgregadas.Any(a => (a.ID_RESPONSABLE.HasValue ? a.ID_RESPONSABLE.Value == item.ID_RESPONSABLE : false) ?
                                (a.CITA_FECHA_HORA.Value.Year == aux.Value.Year && a.CITA_FECHA_HORA.Value.Month == aux.Value.Month && a.CITA_FECHA_HORA.Value.Day == aux.Value.Day &&
                                a.CITA_FECHA_HORA.Value.Hour == aux.Value.Hour && a.CITA_FECHA_HORA.Value.Minute == aux.Value.Minute)
                            : false))
                            {
                                citatruena = new ATENCION_CITA
                                {
                                    ID_USUARIO = item.ID_USUARIO,
                                    ID_TIPO_SERVICIO = item.ID_TIPO_SERVICIO,
                                    ID_TIPO_ATENCION = item.ID_TIPO_ATENCION,
                                    ID_RESPONSABLE = item.ID_RESPONSABLE,
                                    ID_INGRESO = item.ID_INGRESO,
                                    ID_IMPUTADO = item.ID_IMPUTADO,
                                    ID_CENTRO = item.ID_CENTRO,
                                    ID_AREA = item.ID_AREA,
                                    ID_ANIO = item.ID_ANIO,
                                    CITA_HORA_TERMINA = item.CITA_HORA_TERMINA,
                                    CITA_FECHA_HORA = item.CITA_FECHA_HORA,
                                    ESTATUS = item.ESTATUS,
                                    ID_CITA = item.ID_CITA,
                                    ID_CENTRO_UBI = item.ID_CENTRO_UBI,
                                    ID_ATENCION_MEDICA = item.ID_ATENCION_MEDICA,
                                    ID_ATENCION = item.ID_ATENCION,
                                    ID_CENTRO_AT_SOL = item.ID_CENTRO_AT_SOL,
                                };
                                citatruena.ID_CITA = GetIDProceso<short>("ATENCION_CITA", "ID_CITA", "ID_CENTRO_UBI = " + AtencionMedica.ID_CENTRO_UBI);
                                Context.ATENCION_CITA.Add(citatruena);
                                Context.SaveChanges();
                                citasAgregadas.Add(citatruena);
                                aux = citatruena.CITA_FECHA_HORA;
                                idCita = citatruena.ID_CITA;
                                responsable = item.ID_RESPONSABLE.HasValue ? item.ID_RESPONSABLE.Value : 0;
                            }
                            if (item.PROC_ATENCION_MEDICA_PROG != null ? item.PROC_ATENCION_MEDICA_PROG.Any() : false)
                            {
                                foreach (var itm in item.PROC_ATENCION_MEDICA_PROG)
                                {
                                    if (!auxiliarProcs.Any(a => a.ID_PROCMED == itm.PROC_ATENCION_MEDICA.ID_PROCMED && a.ID_USUARIO == itm.PROC_ATENCION_MEDICA.ID_USUARIO && a.REGISTRO_FEC == itm.PROC_ATENCION_MEDICA.REGISTRO_FEC && a.OBSERV == itm.PROC_ATENCION_MEDICA.OBSERV))
                                    {
                                        itm.PROC_ATENCION_MEDICA.ID_ATENCION_MEDICA = AtencionMedica.ID_ATENCION_MEDICA;
                                        itm.PROC_ATENCION_MEDICA.ID_CENTRO_UBI = AtencionMedica.ID_CENTRO_UBI;
                                        auxiliarProcs.Add(itm.PROC_ATENCION_MEDICA);
                                        Context.PROC_ATENCION_MEDICA.Add(new PROC_ATENCION_MEDICA
                                        {
                                            ID_PROCMED = itm.PROC_ATENCION_MEDICA.ID_PROCMED,
                                            ID_USUARIO = itm.PROC_ATENCION_MEDICA.ID_USUARIO,
                                            OBSERV = itm.PROC_ATENCION_MEDICA.OBSERV,
                                            REGISTRO_FEC = itm.PROC_ATENCION_MEDICA.REGISTRO_FEC,
                                            ID_ATENCION_MEDICA = AtencionMedica.ID_ATENCION_MEDICA,
                                            ID_CENTRO_UBI = AtencionMedica.ID_CENTRO_UBI,
                                        });
                                        Context.SaveChanges();
                                    }
                                    Context.PROC_ATENCION_MEDICA_PROG.Add(new PROC_ATENCION_MEDICA_PROG
                                    {
                                        ID_AM_PROG = GetIDProceso<short>("PROC_ATENCION_MEDICA_PROG", "ID_AM_PROG", "ID_CENTRO_UBI = " + AtencionMedica.ID_CENTRO_UBI),
                                        ESTATUS = itm.ESTATUS,
                                        ID_ATENCION_MEDICA = AtencionMedica.ID_ATENCION_MEDICA,
                                        ID_CENTRO_UBI = AtencionMedica.ID_CENTRO_UBI,
                                        ID_PROCMED = itm.ID_PROCMED,
                                        ID_USUARIO_ASIGNADO = itm.ID_USUARIO_ASIGNADO,
                                        ID_CITA = idCita,
                                        ID_CENTRO_CITA = itm.ID_CENTRO_CITA,
                                    });
                                    Context.SaveChanges();
                                }
                            }
                        }
                    }
                    #endregion

                    #region MUJERES
                    if (Mujeres != null)
                    {
                        if (Context.HISTORIA_CLINICA.Any(a => a.ID_CENTRO == AtencionMedica.ID_CENTRO && a.ID_ANIO == AtencionMedica.ID_ANIO && a.ID_IMPUTADO == AtencionMedica.ID_IMPUTADO && a.ID_INGRESO == AtencionMedica.ID_INGRESO))
                        {
                            Mujeres.ID_CONSEC = Context.HISTORIA_CLINICA.First(a => a.ID_CENTRO == AtencionMedica.ID_CENTRO && a.ID_ANIO == AtencionMedica.ID_ANIO && a.ID_IMPUTADO == AtencionMedica.ID_IMPUTADO && a.ID_INGRESO == AtencionMedica.ID_INGRESO).ID_CONSEC;
                            Mujeres.ID_GINECO = GetIDProceso<short>("HISTORIA_CLINICA_GINECO_OBSTRE", "ID_GINECO", "ID_CENTRO = " + AtencionMedica.ID_CENTRO + " AND ID_ANIO = " + AtencionMedica.ID_ANIO + " AND ID_IMPUTADO = " +
                                AtencionMedica.ID_IMPUTADO + " AND ID_INGRESO = " + AtencionMedica.ID_INGRESO + " AND ID_CONSEC = " + Mujeres.ID_CONSEC);
                            Context.HISTORIA_CLINICA_GINECO_OBSTRE.Add(Mujeres);
                            Context.SaveChanges();
                        }
                    }
                    #endregion

                    Context.SaveChanges();
                    transaccion.Complete();
                }
                return true;
            }
            catch (Exception ex)
            {

                throw new ApplicationException(ex.Message);
            }
        }
        public bool ActualizarNotaEvolucion(ATENCION_MEDICA AtencionMedica, NOTA_EVOLUCION NotaEvolucion, NOTA_SIGNOS_VITALES NotaSignosVitales, NOTA_MEDICA NotaMedica, List<NOTA_MEDICA_ENFERMEDAD> Enfermedades = null,
            RECETA_MEDICA Receta = null, List<RECETA_MEDICA_DETALLE> RecetasCanceladas = null, List<RECETA_MEDICA_DETALLE> RecetasAgregadas = null, List<NOTA_MEDICA_DIETA> DietasCanceladas = null,
                List<NOTA_MEDICA_DIETA> DietasPorAgregar = null, List<ATENCION_CITA> ProcedimientosCitas = null, short? MensajeEnfermedad = new Nullable<short>(), List<HISTORIA_CLINICA_PATOLOGICOS> Patologicos = null)
        {//List<PROC_ATENCION_MEDICA> ProcedimientosPorEditar = null,
            var citatruena = new ATENCION_CITA();
            var citasAgregadas = new List<ATENCION_CITA>();
            var aux = new Nullable<DateTime>();
            var hoy = new DateTime();
            var idCita = 0;
            var responsable = 0;
            try
            {
                using (TransactionScope transaccion = new TransactionScope(TransactionScopeOption.RequiresNew, new TransactionOptions() { IsolationLevel = System.Transactions.IsolationLevel.ReadCommitted }))
                {
                    //Context.Entry(AtencionMedica).State = EntityState.Modified;
                    //Context.SaveChanges();

                    NotaMedica.ID_ATENCION_MEDICA = AtencionMedica.ID_ATENCION_MEDICA;
                    //Context.NOTA_MEDICA.Add(NotaMedica);
                    Context.Entry(NotaMedica).State = EntityState.Modified;
                    Context.SaveChanges();

                    //NotaSignosVitales.ID_ATENCION_MEDICA = AtencionMedica.ID_ATENCION_MEDICA;
                    //Context.NOTA_SIGNOS_VITALES.Add(NotaSignosVitales);
                    Context.Entry(NotaSignosVitales).State = EntityState.Modified;
                    Context.SaveChanges();

                    NotaEvolucion.ID_ATENCION_MEDICA = AtencionMedica.ID_ATENCION_MEDICA;
                    //Context.NOTA_EVOLUCION.Add(NotaEvolucion);
                    Context.Entry(NotaEvolucion).State = EntityState.Modified;
                    Context.SaveChanges();

                    #region ENFERMEDADES
                    var consec = Context.HISTORIA_CLINICA.First(f => f.ID_CENTRO == AtencionMedica.ID_CENTRO && f.ID_ANIO == AtencionMedica.ID_ANIO && f.ID_IMPUTADO == AtencionMedica.ID_IMPUTADO && f.ID_INGRESO == AtencionMedica.ID_INGRESO);
                    hoy = GetFechaServerDate();
                    var bandera = false;
                    var gruposString = string.Empty;
                    var gruposIDs = new List<short>();
                    if (Enfermedades != null ? Enfermedades.Any() : false)
                    {
                        foreach (var item in Enfermedades)
                        {
                            Context.NOTA_MEDICA_ENFERMEDAD.Add(new NOTA_MEDICA_ENFERMEDAD
                            {
                                ID_ATENCION_MEDICA = AtencionMedica.ID_ATENCION_MEDICA,
                                ID_CENTRO_UBI = AtencionMedica.ID_CENTRO_UBI,
                                ID_ENFERMEDAD = item.ID_ENFERMEDAD,
                                REGISTRO_FEC = item.REGISTRO_FEC
                            });
                            bandera = bandera ? bandera : item.ENFERMEDAD != null ? item.ENFERMEDAD.SECTOR_CLASIFICACION.Any() : false;
                        }
                        if (bandera)
                        {
                            var i = 1;
                            foreach (var item in Enfermedades.SelectMany(s => s.ENFERMEDAD.SECTOR_CLASIFICACION))
                            {
                                if (consec != null)
                                {
                                    if (!Context.GRUPO_VULNERABLE.Any(w => w.ID_CENTRO == AtencionMedica.ID_CENTRO && w.ID_ANIO == AtencionMedica.ID_ANIO && w.ID_IMPUTADO == AtencionMedica.ID_IMPUTADO && w.ID_INGRESO == AtencionMedica.ID_INGRESO &&
                                        w.ID_CONSEC == consec.ID_CONSEC && w.ID_SECTOR_CLAS == item.ID_SECTOR_CLAS))
                                    {
                                        Context.GRUPO_VULNERABLE.Add(new GRUPO_VULNERABLE
                                        {
                                            ID_ANIO = AtencionMedica.ID_ANIO.Value,
                                            ID_CENTRO = AtencionMedica.ID_CENTRO.Value,
                                            ID_IMPUTADO = AtencionMedica.ID_IMPUTADO.Value,
                                            ID_INGRESO = AtencionMedica.ID_INGRESO.Value,
                                            ID_SECTOR_CLAS = item.ID_SECTOR_CLAS,
                                            MOMENTO_DETECCION = "DI",
                                            REGISTRO_FEC = hoy,
                                            ID_GRUPO_CONSEC = GetIDProceso<short>("GRUPO_VULNERABLE", "ID_CONSEC", "ID_CENTRO = " + AtencionMedica.ID_CENTRO + " AND ID_ANIO = " + AtencionMedica.ID_ANIO + " AND ID_IMPUTADO = " +
                                                AtencionMedica.ID_IMPUTADO + " AND ID_INGRESO = " + AtencionMedica.ID_INGRESO + " AND ID_CONSEC = " + consec.ID_CONSEC),
                                            ID_CONSEC = consec.ID_CONSEC,
                                        });
                                        Context.SaveChanges();
                                        if (!gruposIDs.Any(a => a == item.ID_SECTOR_CLAS))
                                        {
                                            gruposIDs.Add(item.ID_SECTOR_CLAS);
                                            gruposString = gruposString + (string.IsNullOrEmpty(gruposString) ? "" : ", ") + item.POBLACION;//+(i == item.PATOLOGICO_CAT.SECTOR_CLASIFICACION.Count() ? "" : ", ");
                                            i++;
                                        }
                                    }
                                }
                            }
                        }
                        Context.SaveChanges();
                    }
                    #endregion

                    #region PATOLOGICOS
                    if (Patologicos != null ? Patologicos.Any() : false)
                        foreach (var item in Patologicos)
                        {
                            if (item.ID_CONSEC == 0)
                            {
                                item.ID_CONSEC = Context.HISTORIA_CLINICA.First(w => w.ID_CENTRO == item.ID_CENTRO && w.ID_ANIO == item.ID_ANIO && w.ID_IMPUTADO == item.ID_IMPUTADO && w.ID_INGRESO == item.ID_INGRESO).ID_CONSEC;
                                Context.HISTORIA_CLINICA_PATOLOGICOS.Add(item);
                                Context.SaveChanges();
                                if (consec != null)
                                {
                                    item.PATOLOGICO_CAT = Context.PATOLOGICO_CAT.Any(a => a.ID_PATOLOGICO == item.ID_PATOLOGICO) ? Context.PATOLOGICO_CAT.First(f => f.ID_PATOLOGICO == item.ID_PATOLOGICO) : null;
                                    if (item.PATOLOGICO_CAT != null)
                                    {
                                        var i = 0;
                                        foreach (var itm in item.PATOLOGICO_CAT.SECTOR_CLASIFICACION)
                                        {
                                            if (!Context.GRUPO_VULNERABLE.Any(w => w.ID_CENTRO == item.ID_CENTRO && w.ID_ANIO == item.ID_ANIO && w.ID_IMPUTADO == item.ID_IMPUTADO && w.ID_INGRESO == item.ID_INGRESO &&
                                                w.ID_CONSEC == consec.ID_CONSEC && w.ID_SECTOR_CLAS == itm.ID_SECTOR_CLAS))
                                            {
                                                Context.GRUPO_VULNERABLE.Add(new GRUPO_VULNERABLE
                                                {
                                                    ID_ANIO = AtencionMedica.ID_ANIO.Value,
                                                    ID_CENTRO = AtencionMedica.ID_CENTRO.Value,
                                                    ID_IMPUTADO = AtencionMedica.ID_IMPUTADO.Value,
                                                    ID_INGRESO = AtencionMedica.ID_INGRESO.Value,
                                                    ID_SECTOR_CLAS = itm.ID_SECTOR_CLAS,
                                                    MOMENTO_DETECCION = "DI",
                                                    REGISTRO_FEC = hoy,
                                                    ID_GRUPO_CONSEC = GetIDProceso<short>("GRUPO_VULNERABLE", "ID_CONSEC", "ID_CENTRO = " + AtencionMedica.ID_CENTRO + " AND ID_ANIO = " + AtencionMedica.ID_ANIO + " AND ID_IMPUTADO = " +
                                                              AtencionMedica.ID_IMPUTADO + " AND ID_INGRESO = " + AtencionMedica.ID_INGRESO + " AND ID_CONSEC = " + consec.ID_CONSEC),
                                                    ID_CONSEC = consec.ID_CONSEC,
                                                });
                                                Context.SaveChanges();
                                                if (!gruposIDs.Any(a => a == itm.ID_SECTOR_CLAS))
                                                {
                                                    gruposIDs.Add(itm.ID_SECTOR_CLAS);
                                                    gruposString = gruposString + (string.IsNullOrEmpty(gruposString) ? "" : ", ") + itm.POBLACION;//+(i == item.PATOLOGICO_CAT.SECTOR_CLASIFICACION.Count() ? "" : ", ");
                                                    i++;
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                            else
                            {
                                var historia = Context.HISTORIA_CLINICA_PATOLOGICOS.First(w => w.ID_CENTRO == item.ID_CENTRO && w.ID_ANIO == item.ID_ANIO && w.ID_IMPUTADO == item.ID_IMPUTADO && w.ID_INGRESO == item.ID_INGRESO &&
                                    w.ID_CONSEC == item.ID_CONSEC && w.ID_PATOLOGICO == item.ID_PATOLOGICO && w.ID_NOPATOLOGICO == item.ID_NOPATOLOGICO);
                                if (historia != null ? ((historia.RECUPERADO != "S" && item.RECUPERADO == "S") || historia.OBSERVACIONES != item.OBSERVACIONES) : false)
                                {
                                    //item.RECUPERACION_FEC = GetFechaServerDate();
                                    //Context.Entry(item).State = EntityState.Modified;
                                    Context.Entry(historia).CurrentValues.SetValues(item);
                                    Context.SaveChanges();
                                }
                            }
                        }
                    #endregion

                    #region MENSAJE
                    if (string.IsNullOrEmpty(gruposString) ? false : !string.IsNullOrEmpty(gruposString.Trim()))
                    {
                        var _mensaje_tipo = Context.MENSAJE_TIPO.FirstOrDefault(w => w.ID_MEN_TIPO == MensajeEnfermedad);
                        var _imputado = Context.IMPUTADO.First(w => w.ID_ANIO == AtencionMedica.ID_ANIO && w.ID_CENTRO == AtencionMedica.ID_CENTRO && w.ID_IMPUTADO == AtencionMedica.ID_IMPUTADO);
                        var _nombre_completo = string.Format("{0} {1} {2}", (!string.IsNullOrEmpty(_imputado.NOMBRE) ? _imputado.NOMBRE.Trim() : string.Empty),
                            (!string.IsNullOrEmpty(_imputado.PATERNO) ? _imputado.PATERNO.Trim() : string.Empty),
                            (!string.IsNullOrEmpty(_imputado.MATERNO) ? _imputado.MATERNO.Trim() : string.Empty));
                        var _id_mensaje = GetIDProceso<int>("MENSAJE", "ID_MENSAJE", "1=1");
                        var _contenido = _mensaje_tipo.CONTENIDO + " " + _nombre_completo + " CON FOLIO " +
                            string.Format("{0}/{1}", AtencionMedica.ID_ANIO, AtencionMedica.ID_IMPUTADO) +
                                " QUE PERTENE A LOS SIGUIENTES GRUPOS VULNERABLES: \n" + gruposString;
                        hoy = GetFechaServerDate();
                        var _mensaje = new MENSAJE
                        {
                            CONTENIDO = _contenido,
                            ENCABEZADO = _mensaje_tipo.ENCABEZADO,
                            ID_MEN_TIPO = _mensaje_tipo.ID_MEN_TIPO,
                            ID_MENSAJE = _id_mensaje,
                            REGISTRO_FEC = hoy,
                            ID_CENTRO = AtencionMedica.ID_CENTRO_UBI
                        };
                        Context.MENSAJE.Add(_mensaje);
                        Context.SaveChanges();
                    }
                    #endregion

                    #region RECETAS
                    if (RecetasCanceladas != null ? RecetasCanceladas.Any() : false)
                    {
                        foreach (var item in RecetasCanceladas)
                        {
                            item.CANCELA_ID_ATENCION_MEDICA = AtencionMedica.ID_ATENCION_MEDICA;
                            item.CANCELA_ID_CENTRO_UBI = AtencionMedica.ID_CENTRO_UBI;
                            Context.Entry(item).State = EntityState.Modified;
                        }
                        Context.SaveChanges();
                    }

                    if (RecetasAgregadas != null ? RecetasAgregadas.Any() : false)
                    {
                        Receta.ID_ATENCION_MEDICA = AtencionMedica.ID_ATENCION_MEDICA;
                        if (!Context.RECETA_MEDICA.Any(a => a.ID_ATENCION_MEDICA == AtencionMedica.ID_ATENCION_MEDICA))
                        {
                            Receta.ID_FOLIO = GetIDProceso<short>("RECETA_MEDICA", "ID_FOLIO", "ID_CENTRO_UBI = " + AtencionMedica.ID_CENTRO_UBI);
                            Context.RECETA_MEDICA.Add(Receta);
                        }
                        else
                        {
                            Receta.ID_FOLIO = Context.RECETA_MEDICA.First(a => a.ID_ATENCION_MEDICA == AtencionMedica.ID_ATENCION_MEDICA).ID_FOLIO;
                        }
                        Context.SaveChanges();
                        foreach (var item in RecetasAgregadas)
                        {
                            if (item.ID_PRESENTACION_MEDICAMENTO <= 0)
                                item.ID_PRESENTACION_MEDICAMENTO = new Nullable<short>();
                            item.ID_ATENCION_MEDICA = AtencionMedica.ID_ATENCION_MEDICA;
                            item.ID_FOLIO = Receta.ID_FOLIO;
                            Context.RECETA_MEDICA_DETALLE.Add(item);
                        }
                        Context.SaveChanges();
                    }
                    #endregion

                    #region DIETAS
                    if (DietasCanceladas != null ? DietasCanceladas.Any() : false)
                    {
                        foreach (var item in DietasCanceladas)
                        {
                            item.ESTATUS = "N";
                            Context.Entry(item).State = EntityState.Modified;
                        }
                        Context.SaveChanges();
                    }
                    if (DietasPorAgregar != null ? DietasPorAgregar.Any() : false)
                    {
                        // checar cuando solo es solo una por agregar
                        foreach (var item in DietasPorAgregar)
                        {
                            item.ID_ATENCION_MEDICA = AtencionMedica.ID_ATENCION_MEDICA;
                            if (Context.NOTA_MEDICA_DIETA.Any(a => a.ID_ATENCION_MEDICA == AtencionMedica.ID_ATENCION_MEDICA ? a.ID_CENTRO_UBI == AtencionMedica.ID_CENTRO_UBI ? a.ID_DIETA == item.ID_DIETA : false : false))
                                Context.Entry(item).State = EntityState.Modified;
                            else
                                Context.NOTA_MEDICA_DIETA.Add(item);
                        }
                        Context.SaveChanges();
                    }
                    #endregion

                    #region PROCEDIMIENTOS MEDICOS
                    var auxiliarProcs = new List<PROC_ATENCION_MEDICA>();
                    if (ProcedimientosCitas != null ? ProcedimientosCitas.Any() : false)
                    {
                        foreach (var item in ProcedimientosCitas)
                        {
                            if (item.ID_CENTRO_AT_SOL == null)
                                item.ID_CENTRO_AT_SOL = item.ID_CENTRO_UBI;
                            if (!aux.HasValue)
                            {
                                citatruena = new ATENCION_CITA
                                {
                                    ID_USUARIO = item.ID_USUARIO,
                                    ID_TIPO_SERVICIO = item.ID_TIPO_SERVICIO,
                                    ID_TIPO_ATENCION = item.ID_TIPO_ATENCION,
                                    ID_RESPONSABLE = item.ID_RESPONSABLE,
                                    ID_INGRESO = item.ID_INGRESO,
                                    ID_IMPUTADO = item.ID_IMPUTADO,
                                    ID_CENTRO = item.ID_CENTRO,
                                    ID_AREA = item.ID_AREA,
                                    ID_ANIO = item.ID_ANIO,
                                    CITA_HORA_TERMINA = item.CITA_HORA_TERMINA,
                                    CITA_FECHA_HORA = item.CITA_FECHA_HORA,
                                    ESTATUS = item.ESTATUS,
                                    ID_CITA = item.ID_CITA,
                                    ID_CENTRO_UBI = item.ID_CENTRO_UBI,
                                    ID_ATENCION_MEDICA = item.ID_ATENCION_MEDICA,
                                    ID_ATENCION = item.ID_ATENCION,
                                    ID_CENTRO_AT_SOL = item.ID_CENTRO_AT_SOL,
                                };
                                citatruena.ID_CITA = GetIDProceso<short>("ATENCION_CITA", "ID_CITA", "ID_CENTRO_UBI = " + AtencionMedica.ID_CENTRO_UBI);
                                Context.ATENCION_CITA.Add(citatruena);
                                Context.SaveChanges();
                                citasAgregadas.Add(citatruena);
                                aux = citatruena.CITA_FECHA_HORA;
                                idCita = citatruena.ID_CITA;
                                responsable = item.ID_RESPONSABLE.HasValue ? item.ID_RESPONSABLE.Value : 0;
                            }
                            else if (!(item.CITA_FECHA_HORA.Value.Year == aux.Value.Year && item.CITA_FECHA_HORA.Value.Month == aux.Value.Month && item.CITA_FECHA_HORA.Value.Day == aux.Value.Day &&
                            item.CITA_FECHA_HORA.Value.Hour == aux.Value.Hour && item.CITA_FECHA_HORA.Value.Minute == aux.Value.Minute) ?
                                true
                            : !citasAgregadas.Any(a => (a.ID_RESPONSABLE.HasValue ? a.ID_RESPONSABLE.Value == item.ID_RESPONSABLE : false) ?
                                (a.CITA_FECHA_HORA.Value.Year == aux.Value.Year && a.CITA_FECHA_HORA.Value.Month == aux.Value.Month && a.CITA_FECHA_HORA.Value.Day == aux.Value.Day &&
                                a.CITA_FECHA_HORA.Value.Hour == aux.Value.Hour && a.CITA_FECHA_HORA.Value.Minute == aux.Value.Minute)
                            : false))
                            {
                                citatruena = new ATENCION_CITA
                                {
                                    ID_USUARIO = item.ID_USUARIO,
                                    ID_TIPO_SERVICIO = item.ID_TIPO_SERVICIO,
                                    ID_TIPO_ATENCION = item.ID_TIPO_ATENCION,
                                    ID_RESPONSABLE = item.ID_RESPONSABLE,
                                    ID_INGRESO = item.ID_INGRESO,
                                    ID_IMPUTADO = item.ID_IMPUTADO,
                                    ID_CENTRO = item.ID_CENTRO,
                                    ID_AREA = item.ID_AREA,
                                    ID_ANIO = item.ID_ANIO,
                                    CITA_HORA_TERMINA = item.CITA_HORA_TERMINA,
                                    CITA_FECHA_HORA = item.CITA_FECHA_HORA,
                                    ESTATUS = item.ESTATUS,
                                    ID_CITA = item.ID_CITA,
                                    ID_CENTRO_UBI = item.ID_CENTRO_UBI,
                                    ID_ATENCION_MEDICA = item.ID_ATENCION_MEDICA,
                                    ID_ATENCION = item.ID_ATENCION,
                                    ID_CENTRO_AT_SOL = item.ID_CENTRO_AT_SOL,
                                };
                                citatruena.ID_CITA = GetIDProceso<short>("ATENCION_CITA", "ID_CITA", "ID_CENTRO_UBI = " + AtencionMedica.ID_CENTRO_UBI);
                                Context.ATENCION_CITA.Add(citatruena);
                                Context.SaveChanges();
                                citasAgregadas.Add(citatruena);
                                aux = citatruena.CITA_FECHA_HORA;
                                idCita = citatruena.ID_CITA;
                                responsable = item.ID_RESPONSABLE.HasValue ? item.ID_RESPONSABLE.Value : 0;
                            }
                            if (item.PROC_ATENCION_MEDICA_PROG != null ? item.PROC_ATENCION_MEDICA_PROG.Any() : false)
                            {
                                foreach (var itm in item.PROC_ATENCION_MEDICA_PROG)
                                {
                                    if (!auxiliarProcs.Any(a => a.ID_PROCMED == itm.PROC_ATENCION_MEDICA.ID_PROCMED && a.ID_USUARIO == itm.PROC_ATENCION_MEDICA.ID_USUARIO && a.REGISTRO_FEC == itm.PROC_ATENCION_MEDICA.REGISTRO_FEC && a.OBSERV == itm.PROC_ATENCION_MEDICA.OBSERV))
                                    {
                                        itm.PROC_ATENCION_MEDICA.ID_ATENCION_MEDICA = AtencionMedica.ID_ATENCION_MEDICA;
                                        itm.PROC_ATENCION_MEDICA.ID_CENTRO_UBI = AtencionMedica.ID_CENTRO_UBI;
                                        auxiliarProcs.Add(itm.PROC_ATENCION_MEDICA);
                                        Context.PROC_ATENCION_MEDICA.Add(new PROC_ATENCION_MEDICA
                                        {
                                            ID_PROCMED = itm.PROC_ATENCION_MEDICA.ID_PROCMED,
                                            ID_USUARIO = itm.PROC_ATENCION_MEDICA.ID_USUARIO,
                                            OBSERV = itm.PROC_ATENCION_MEDICA.OBSERV,
                                            REGISTRO_FEC = itm.PROC_ATENCION_MEDICA.REGISTRO_FEC,
                                            ID_ATENCION_MEDICA = AtencionMedica.ID_ATENCION_MEDICA,
                                            ID_CENTRO_UBI = AtencionMedica.ID_CENTRO_UBI,
                                        });
                                        Context.SaveChanges();
                                    }
                                    Context.PROC_ATENCION_MEDICA_PROG.Add(new PROC_ATENCION_MEDICA_PROG
                                    {
                                        ID_AM_PROG = GetIDProceso<short>("PROC_ATENCION_MEDICA_PROG", "ID_AM_PROG", "ID_CENTRO_UBI = " + AtencionMedica.ID_CENTRO_UBI),
                                        ESTATUS = itm.ESTATUS,
                                        ID_ATENCION_MEDICA = AtencionMedica.ID_ATENCION_MEDICA,
                                        ID_CENTRO_UBI = AtencionMedica.ID_CENTRO_UBI,
                                        ID_PROCMED = itm.ID_PROCMED,
                                        ID_USUARIO_ASIGNADO = itm.ID_USUARIO_ASIGNADO,
                                        ID_CITA = idCita,
                                        ID_CENTRO_CITA = itm.ID_CENTRO_CITA,
                                    });
                                    Context.SaveChanges();
                                }
                            }
                        }
                    }
                    #endregion

                    Context.SaveChanges();
                    transaccion.Complete();
                }
                return true;
            }
            catch (Exception ex)
            {

                throw new ApplicationException(ex.Message);
            }
        }
    }
}