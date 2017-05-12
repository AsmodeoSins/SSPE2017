using System.Linq;
using System.Transactions;
using SSP.Servidor;
using SSP.Modelo;
using System.Data;
using System.Collections.Generic;
using System;
using LinqKit;
using System.Data.Objects;

namespace SSP.Controlador.Catalogo.Justicia
{
    public class cNotaMedica : EntityManagerServer<NOTA_MEDICA>
    {
        public cNotaMedica() { }


        public NOTA_MEDICA ObtenerNota(int IdAtencionMedica)
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

        public bool Insertar(NOTA_MEDICA Entity)
        {
            try
            {
                if (Insert(Entity))
                    return true;

                return false;
            }

            catch (System.Exception ex)
            {
                throw new System.ApplicationException(ex.Message);
            }
        }

        public bool Actualizar(NOTA_MEDICA Entity)
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

        /*private enum eTipoAtencionMedica ///SE CREA ENUMERADOR TEMPORAL PARA SEGUIR CON EL PROCESO, LOS DATOS AQUI LISTADOS SON PRODUCTO DEL DOCUMENTO DE ESPECIFICACION, EN LA PAG 219
        {
            CONSULTA_MEDICA = 1,
            SERVICIOS_DE_URGENCIAS = 2,
            CONSULTA_DENTAL = 3,
            ESTUDIOS_LABORATORIO_O_RAYOS_X = 4
        }*/

        public bool ActualizarNotas(NOTA_SIGNOS_VITALES Entity, NOTA_MEDICA EntityMedico, int IdAtencionMedica)
        {
            try
            {
                using (TransactionScope transaccion = new TransactionScope(TransactionScopeOption.RequiresNew, new TransactionOptions() { IsolationLevel = System.Transactions.IsolationLevel.ReadCommitted }))
                {
                    /*var NotaSignosVitales = Context.NOTA_SIGNOS_VITALES.Where(x => x.ID_ATENCION_MEDICA == IdAtencionMedica && x.ATENCION_MEDICA.ID_TIPO_ATENCION == (short)eTipoAtencionMedica.CONSULTA_MEDICA).SingleOrDefault();
                    var NotaMedica = Context.NOTA_MEDICA.Where(x => x.ID_ATENCION_MEDICA == IdAtencionMedica).SingleOrDefault();

                    var NotaVital = NotaSignosVitales;
                    if (NotaVital != null)
                    {
                        NotaVital.FRECUENCIA_CARDIAC = Entity.FRECUENCIA_CARDIAC;
                        NotaVital.FRECUENCIA_RESPIRA = Entity.FRECUENCIA_RESPIRA;
                        NotaVital.ID_ATENCION_MEDICA = IdAtencionMedica;
                        NotaVital.ID_RESPONSABLE = Entity.ID_RESPONSABLE;
                        NotaVital.OBSERVACIONES = Entity.OBSERVACIONES;
                        NotaVital.PESO = Entity.PESO;
                        NotaVital.TALLA = Entity.TALLA;
                        NotaVital.TEMPERATURA = Entity.TEMPERATURA;
                        NotaVital.TENSION_ARTERIAL = Entity.TENSION_ARTERIAL;
                        Context.Entry(NotaSignosVitales).State = EntityState.Modified;
                    };

                    var SignoVital = NotaMedica;
                    if (SignoVital != null)
                    {
                        SignoVital.DIAGNOSTICO = EntityMedico.DIAGNOSTICO;
                        SignoVital.EXPLORACION_FISICA = EntityMedico.EXPLORACION_FISICA;
                        SignoVital.ID_ATENCION_MEDICA = IdAtencionMedica;
                        SignoVital.ID_RESPONSABLE = EntityMedico.ID_RESPONSABLE;
                        SignoVital.PLAN_ESTUDIO_TRATA = EntityMedico.PLAN_ESTUDIO_TRATA;
                        SignoVital.PRONOSTICO = EntityMedico.PRONOSTICO;
                        SignoVital.RESULTADO_SERV_AUX = EntityMedico.RESULTADO_SERV_AUX;
                        SignoVital.RESULTADO_SERV_TRA = EntityMedico.RESULTADO_SERV_TRA;
                        SignoVital.RESUMEN_INTERROGAT = EntityMedico.RESUMEN_INTERROGAT;
                       Context.Entry(NotaMedica).State = EntityState.Modified;
                    };

                    Context.SaveChanges();
                    transaccion.Complete();*/
                    return true;
                }
            }

            catch (System.Exception)
            {
                throw;
            }

            return false;
        }


        #region Transaccion que contempla la falta de una atncion medica y ambos roles para el mismo usuario

        public bool GuardarNotas(NOTA_MEDICA NotaMedica, NOTA_SIGNOS_VITALES NotaSignosVitales, INGRESO IngresoNotas)
        {
            try
            {
                using (TransactionScope transaccion = new TransactionScope(TransactionScopeOption.RequiresNew, new TransactionOptions() { IsolationLevel = System.Transactions.IsolationLevel.ReadCommitted }))
                {
                    #region Atencion Medica
                    var AtencionMedica = new ATENCION_MEDICA();
                    AtencionMedica.ID_ANIO = IngresoNotas.ID_ANIO;
                    AtencionMedica.ID_CENTRO = IngresoNotas.ID_UB_CENTRO;
                    AtencionMedica.ID_IMPUTADO = IngresoNotas.ID_IMPUTADO;
                    AtencionMedica.ID_INGRESO = IngresoNotas.ID_INGRESO;
                    //AtencionMedica.ID_TIPO_ATENCION = (short)eTipoAtencionMedica.CONSULTA_MEDICA;
                    AtencionMedica.ID_ATENCION_MEDICA = GetSequence<short>("ATENCION_MEDICA_SEQ");
                    #endregion
                    #region Nota Medica
                    var NotaM = new NOTA_MEDICA();
                    NotaM.DIAGNOSTICO = NotaMedica.DIAGNOSTICO;
                    NotaM.EXPLORACION_FISICA = NotaMedica.EXPLORACION_FISICA;
                    NotaM.ID_ATENCION_MEDICA = AtencionMedica.ID_ATENCION_MEDICA;
                    NotaM.ID_RESPONSABLE = NotaMedica.ID_RESPONSABLE;
                    NotaM.PLAN_ESTUDIO_TRATA = NotaMedica.PLAN_ESTUDIO_TRATA;
                    NotaM.PRONOSTICO = NotaMedica.PRONOSTICO;
                    NotaM.RESULTADO_SERV_AUX = NotaMedica.RESULTADO_SERV_AUX;
                    NotaM.RESULTADO_SERV_TRA = NotaMedica.RESULTADO_SERV_TRA;
                    NotaM.RESUMEN_INTERROGAT = NotaMedica.RESUMEN_INTERROGAT;
                    #endregion
                    #region Nota Signos Vitales
                    var NotaSV = new NOTA_SIGNOS_VITALES();
                    NotaSV.FRECUENCIA_CARDIAC = NotaSignosVitales.FRECUENCIA_CARDIAC;
                    NotaSV.FRECUENCIA_RESPIRA = NotaSignosVitales.FRECUENCIA_RESPIRA;
                    NotaSV.ID_ATENCION_MEDICA = AtencionMedica.ID_ATENCION_MEDICA;
                    NotaSV.ID_RESPONSABLE = NotaSignosVitales.ID_RESPONSABLE;
                    NotaSV.OBSERVACIONES = NotaSignosVitales.OBSERVACIONES;
                    NotaSV.PESO = NotaSignosVitales.PESO;
                    NotaSV.TALLA = NotaSignosVitales.TALLA;
                    NotaSV.TEMPERATURA = NotaSignosVitales.TEMPERATURA;
                    NotaSV.TENSION_ARTERIAL = NotaSignosVitales.TENSION_ARTERIAL;
                    #endregion

                    Context.ATENCION_MEDICA.Add(AtencionMedica);
                    Context.NOTA_SIGNOS_VITALES.Add(NotaSV);
                    Context.NOTA_MEDICA.Add(NotaM);

                    Context.SaveChanges();
                    transaccion.Complete();
                    return true;
                }
            }

            catch (System.Exception exc)
            {
                throw exc;
            }
        }
        #endregion

        public bool ActualizarNotaMedicaEspecialista(DateTime FechaServidor, NOTA_SIGNOS_VITALES SignosVitales, NOTA_MEDICA NotaMedica, ATENCION_MEDICA AtencionMedica, ATENCION_CITA AtencionCita = null,
            List<NOTA_MEDICA_ENFERMEDAD> Enfermedades = null, List<LESION> Lesiones = null, List<DIETA> dietas = null, List<HISTORIA_CLINICA_PATOLOGICOS> patologicos = null, ATENCION_CITA Seguimiento = null,
            RECETA_MEDICA RecetaMedica = null, List<RECETA_MEDICA_DETALLE> RecetaMedicaDetalle = null, List<ATENCION_CITA> ProcedimientosCitas = null, ESPECIALISTA Especialista = null, string Usuario = "",
            short? MensajeEnfermedad = new Nullable<short>(), HISTORIA_CLINICA_GINECO_OBSTRE Mujeres = null, List<ODONTOGRAMA_SEGUIMIENTO2> Odontograma = null)
        {
            var citatruena = new ATENCION_CITA();
            var citasAgregadas = new List<ATENCION_CITA>();
            var responsable = 0;
            var aux = new Nullable<DateTime>();
            var hoy = new DateTime();
            var idCita = 0;
            var citaEliminada = new List<int>();
            var pampEliminada = new List<int>();
            var pamEliminada = new List<int>();
            var pamExistencia = new List<int>();
            var consec = new HISTORIA_CLINICA();
            try
            {

                using (TransactionScope transaccion = new TransactionScope(TransactionScopeOption.RequiresNew, new TransactionOptions() { IsolationLevel = System.Transactions.IsolationLevel.ReadCommitted }))
                {
                    #region SEGUIMIENTO
                    if (Seguimiento != null)
                    {
                        Seguimiento.ID_CENTRO_UBI = AtencionMedica.ID_CENTRO_UBI;
                        if (Seguimiento.ID_CENTRO_AT_SOL == null)
                            Seguimiento.ID_CENTRO_AT_SOL = Seguimiento.ID_CENTRO_UBI;
                        if (Context.ATENCION_CITA.Any(a => a.ID_CITA == Seguimiento.ID_CITA && a.ID_CENTRO_UBI == AtencionMedica.ID_CENTRO_UBI))
                        {
                            Context.Entry(new ATENCION_CITA
                            {
                                CITA_FECHA_HORA = Seguimiento.CITA_FECHA_HORA,
                                CITA_HORA_TERMINA = Seguimiento.CITA_HORA_TERMINA,
                                ID_ANIO = Seguimiento.ID_ANIO,
                                ID_AREA = Seguimiento.ID_AREA,
                                ID_ATENCION = Seguimiento.ID_ATENCION,
                                ID_ATENCION_MEDICA = Seguimiento.ID_ATENCION_MEDICA,
                                ID_CENTRO = Seguimiento.ID_CENTRO,
                                ID_CITA = Seguimiento.ID_CITA,
                                ID_CENTRO_UBI = Seguimiento.ID_CENTRO_UBI,
                                ID_IMPUTADO = Seguimiento.ID_IMPUTADO,
                                ID_INGRESO = Seguimiento.ID_INGRESO,
                                ID_RESPONSABLE = Seguimiento.ID_RESPONSABLE,
                                ID_TIPO_ATENCION = Seguimiento.ID_TIPO_ATENCION,
                                ID_TIPO_SERVICIO = Seguimiento.ID_TIPO_SERVICIO,
                                ID_USUARIO = Seguimiento.ID_USUARIO,
                                ESTATUS = Seguimiento.ESTATUS,
                                ID_CENTRO_AT_SOL = Seguimiento.ID_CENTRO_AT_SOL,
                            }).State = EntityState.Modified;
                        }
                        else
                        {
                            Seguimiento.ID_CITA = GetIDProceso<short>("ATENCION_CITA", "ID_CITA", "ID_CENTRO_UBI = " + AtencionMedica.ID_CENTRO_UBI);
                            Context.ATENCION_CITA.Add(new ATENCION_CITA
                            {
                                CITA_FECHA_HORA = Seguimiento.CITA_FECHA_HORA,
                                CITA_HORA_TERMINA = Seguimiento.CITA_HORA_TERMINA,
                                ID_ANIO = Seguimiento.ID_ANIO,
                                ID_AREA = Seguimiento.ID_AREA,
                                ID_ATENCION = Seguimiento.ID_ATENCION,
                                ID_ATENCION_MEDICA = Seguimiento.ID_ATENCION_MEDICA,
                                ID_CENTRO = Seguimiento.ID_CENTRO,
                                ID_CITA = Seguimiento.ID_CITA,
                                ID_CENTRO_UBI = Seguimiento.ID_CENTRO_UBI,
                                ID_IMPUTADO = Seguimiento.ID_IMPUTADO,
                                ID_INGRESO = Seguimiento.ID_INGRESO,
                                ID_RESPONSABLE = Seguimiento.ID_RESPONSABLE,
                                ID_TIPO_ATENCION = Seguimiento.ID_TIPO_ATENCION,
                                ID_TIPO_SERVICIO = Seguimiento.ID_TIPO_SERVICIO,
                                ID_USUARIO = Seguimiento.ID_USUARIO,
                                ESTATUS = Seguimiento.ESTATUS,
                                ID_CENTRO_AT_SOL = Seguimiento.ID_CENTRO_AT_SOL,
                            });
                        }
                        AtencionMedica.ID_CITA_SEGUIMIENTO = Seguimiento.ID_CITA;
                        Context.SaveChanges();
                        if (Especialista != null)
                        {
                            if (Context.ESPECIALISTA_CITA.Any(a => a.ID_CITA == Seguimiento.ID_CITA && a.ID_ESPECIALIDAD == Especialista.ID_ESPECIALIDAD && a.ID_ESPECIALISTA == Especialista.ID_ESPECIALISTA && a.ID_CENTRO_UBI == AtencionMedica.ID_CENTRO_UBI))
                                Context.Entry(new ESPECIALISTA_CITA
                                {
                                    ID_CENTRO_UBI = Seguimiento.ID_CENTRO_UBI,
                                    ID_CITA = Seguimiento.ID_CITA,
                                    ID_ESPECIALIDAD = Especialista.ID_ESPECIALIDAD,
                                    ID_ESPECIALISTA = Especialista.ID_ESPECIALISTA,
                                    ID_SOLICITUD = new Nullable<int>(),
                                    ID_USUARIO = Usuario,
                                    REGISTRO_FEC = GetFechaServerDate(),
                                }).State = EntityState.Modified;
                            else
                                Context.ESPECIALISTA_CITA.Add(new ESPECIALISTA_CITA
                                {
                                    ID_CENTRO_UBI = Seguimiento.ID_CENTRO_UBI,
                                    ID_CITA = Seguimiento.ID_CITA,
                                    ID_ESPECIALIDAD = Especialista.ID_ESPECIALIDAD,
                                    ID_ESPECIALISTA = Especialista.ID_ESPECIALISTA,
                                    ID_SOLICITUD = new Nullable<int>(),
                                    ID_USUARIO = Usuario,
                                    REGISTRO_FEC = GetFechaServerDate(),
                                });
                        }
                    }
                    #endregion

                    #region ATENCION MEDICA
                    if (Context.ATENCION_MEDICA.Any(a => a.ID_ATENCION_MEDICA == AtencionMedica.ID_ATENCION_MEDICA && a.ID_CENTRO_UBI == AtencionMedica.ID_CENTRO_UBI))
                        Context.Entry(AtencionMedica).State = EntityState.Modified;
                    else
                    {
                        AtencionMedica.ID_ATENCION_MEDICA = GetIDProceso<short>("ATENCION_MEDICA", "ID_ATENCION_MEDICA", "ID_CENTRO_UBI = " + AtencionMedica.ID_CENTRO_UBI);
                        Context.ATENCION_MEDICA.Add(AtencionMedica);
                    }
                    Context.SaveChanges();
                    #endregion

                    #region SIGNOS VITALES
                    SignosVitales.ID_ATENCION_MEDICA = AtencionMedica.ID_ATENCION_MEDICA;
                    SignosVitales.ID_CENTRO_UBI = AtencionMedica.ID_CENTRO_UBI;
                    if (Context.NOTA_SIGNOS_VITALES.Any(a => a.ID_ATENCION_MEDICA == AtencionMedica.ID_ATENCION_MEDICA && a.ID_CENTRO_UBI == AtencionMedica.ID_CENTRO_UBI))
                        Context.Entry(SignosVitales).State = EntityState.Modified;
                    else
                        Context.NOTA_SIGNOS_VITALES.Add(SignosVitales);
                    Context.SaveChanges();
                    #endregion

                    #region NOTA MEDICA
                    NotaMedica.ID_ATENCION_MEDICA = AtencionMedica.ID_ATENCION_MEDICA;
                    NotaMedica.ID_CENTRO_UBI = AtencionMedica.ID_CENTRO_UBI;
                    if (Context.NOTA_MEDICA.Any(a => a.ID_ATENCION_MEDICA == AtencionMedica.ID_ATENCION_MEDICA && a.ID_CENTRO_UBI == AtencionMedica.ID_CENTRO_UBI))
                        Context.Entry(NotaMedica).State = EntityState.Modified;
                    else
                        Context.NOTA_MEDICA.Add(NotaMedica);
                    Context.SaveChanges();
                    #endregion

                    #region ATENCION CITA
                    if (AtencionCita != null)
                    {
                        AtencionCita.ID_ATENCION_MEDICA = AtencionMedica.ID_ATENCION_MEDICA;
                        Context.Entry(AtencionCita).State = EntityState.Modified;
                        Context.SaveChanges();
                    }
                    #endregion

                    #region RECETA
                    if (RecetaMedicaDetalle != null ? RecetaMedicaDetalle.Any() : false)
                    {
                        RecetaMedica.ID_ATENCION_MEDICA = AtencionMedica.ID_ATENCION_MEDICA;
                        RecetaMedica.ID_CENTRO_UBI = AtencionMedica.ID_CENTRO_UBI;
                        if (Context.RECETA_MEDICA.Any(a => a.ID_ATENCION_MEDICA == AtencionMedica.ID_ATENCION_MEDICA && a.ID_CENTRO_UBI == AtencionMedica.ID_CENTRO_UBI))
                        {
                            //var receta = Context.RECETA_MEDICA.First(a => a.ID_ATENCION_MEDICA == AtencionMedica.ID_ATENCION_MEDICA && a.ID_CENTRO_UBI == AtencionMedica.ID_CENTRO_UBI);
                            //RecetaMedica.ID_FOLIO = receta.ID_FOLIO;
                            Context.Entry(RecetaMedica).State = EntityState.Modified;
                            Context.Database.ExecuteSqlCommand("DELETE FROM RECETA_MEDICA_DETALLE WHERE ID_FOLIO = " + RecetaMedica.ID_FOLIO);
                            Context.SaveChanges();
                        }
                        else
                        {
                            RecetaMedica.ID_FOLIO = GetIDProceso<short>("RECETA_MEDICA", "ID_FOLIO", "ID_CENTRO_UBI = " + AtencionMedica.ID_CENTRO_UBI);
                            Context.RECETA_MEDICA.Add(RecetaMedica);
                        }
                        Context.SaveChanges();
                        foreach (var s in RecetaMedicaDetalle)
                        {
                            /*if (Context.RECETA_MEDICA_DETALLE.Any(a => a.ID_FOLIO == RecetaMedica.ID_FOLIO && a.ID_PRODUCTO == s.ID_PRODUCTO && a.ID_ATENCION_MEDICA == AtencionMedica.ID_ATENCION_MEDICA &&
                                a.ID_CENTRO_UBI == AtencionMedica.ID_CENTRO_UBI))
                            {
                                Context.Entry(new RECETA_MEDICA_DETALLE
                                {
                                    CENA = s.CENA,
                                    COMIDA = s.COMIDA,
                                    DESAYUNO = s.DESAYUNO,
                                    DOSIS = s.DOSIS,
                                    DURACION = s.DURACION,
                                    ID_FOLIO = RecetaMedica.ID_FOLIO,
                                    ID_PRODUCTO = s.ID_PRODUCTO,
                                    OBSERV = s.OBSERV,
                                    ID_CENTRO_UBI = AtencionMedica.ID_CENTRO_UBI,
                                    ID_ATENCION_MEDICA = AtencionMedica.ID_ATENCION_MEDICA,
                                    ID_PRESENTACION_MEDICAMENTO = s.ID_PRESENTACION_MEDICAMENTO == 0 ? new Nullable<short>() : s.ID_PRESENTACION_MEDICAMENTO,
                                }).State = EntityState.Modified;
                            }
                            else
                            {*/
                            Context.RECETA_MEDICA_DETALLE.Add(new RECETA_MEDICA_DETALLE
                            {
                                CENA = s.CENA,
                                COMIDA = s.COMIDA,
                                DESAYUNO = s.DESAYUNO,
                                DOSIS = s.DOSIS,
                                DURACION = s.DURACION,
                                ID_FOLIO = RecetaMedica.ID_FOLIO,
                                ID_PRODUCTO = s.ID_PRODUCTO,
                                OBSERV = s.OBSERV,
                                ID_CENTRO_UBI = AtencionMedica.ID_CENTRO_UBI,
                                ID_ATENCION_MEDICA = AtencionMedica.ID_ATENCION_MEDICA,
                                ID_PRESENTACION_MEDICAMENTO = s.ID_PRESENTACION_MEDICAMENTO == 0 ? new Nullable<short>() : s.ID_PRESENTACION_MEDICAMENTO,
                            });
                            /*}*/
                        }
                        Context.SaveChanges();
                    }
                    #endregion

                    consec = Context.HISTORIA_CLINICA.First(f => f.ID_CENTRO == AtencionMedica.ID_CENTRO && f.ID_ANIO == AtencionMedica.ID_ANIO && f.ID_IMPUTADO == AtencionMedica.ID_IMPUTADO && f.ID_INGRESO == AtencionMedica.ID_INGRESO);
                    hoy = GetFechaServerDate();

                    #region PATOLOGICOS
                    var gruposString = string.Empty;
                    var gruposIDs = new List<short>();
                    if (patologicos != null ? patologicos.Any() : false)
                    {
                        foreach (var item in patologicos)
                        {
                            if (item.ID_CONSEC == 0)
                            {
                                item.ID_CONSEC = Context.HISTORIA_CLINICA.OrderByDescending(o => o.ID_CONSEC)
                                    .First(w => w.ID_CENTRO == item.ID_CENTRO && w.ID_ANIO == item.ID_ANIO && w.ID_IMPUTADO == item.ID_IMPUTADO && w.ID_INGRESO == item.ID_INGRESO).ID_CONSEC;
                                if (Context.HISTORIA_CLINICA_PATOLOGICOS.Any(w => w.ID_CENTRO == item.ID_CENTRO && w.ID_ANIO == item.ID_ANIO && w.ID_IMPUTADO == item.ID_IMPUTADO && w.ID_INGRESO == item.ID_INGRESO && w.ID_CONSEC == item.ID_CONSEC
                                    && w.ID_PATOLOGICO == item.ID_PATOLOGICO && w.ID_NOPATOLOGICO == item.ID_NOPATOLOGICO))
                                {
                                    Context.Entry(item).State = EntityState.Modified;
                                    Context.SaveChanges();
                                }
                                else
                                {
                                    Context.HISTORIA_CLINICA_PATOLOGICOS.Add(item);
                                    Context.SaveChanges();
                                    if (consec != null)
                                    {
                                        item.PATOLOGICO_CAT = Context.PATOLOGICO_CAT.Any(a => a.ID_PATOLOGICO == item.ID_PATOLOGICO) ? Context.PATOLOGICO_CAT.First(f => f.ID_PATOLOGICO == item.ID_PATOLOGICO) : null;
                                        if (item.PATOLOGICO_CAT != null)
                                            foreach (var itm in item.PATOLOGICO_CAT.SECTOR_CLASIFICACION)
                                            {
                                                var grupoConsec = GetIDProceso<short>("GRUPO_VULNERABLE", "ID_CONSEC", "ID_CENTRO = " + AtencionMedica.ID_CENTRO + " AND ID_ANIO = " + AtencionMedica.ID_ANIO + " AND ID_IMPUTADO = " +
                                                                  AtencionMedica.ID_IMPUTADO + " AND ID_INGRESO = " + AtencionMedica.ID_INGRESO + " AND ID_CONSEC = " + consec.ID_CONSEC);
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
                                                        ID_GRUPO_CONSEC = grupoConsec,
                                                        ID_CONSEC = consec.ID_CONSEC,
                                                    });
                                                    Context.SaveChanges();
                                                    if (!gruposIDs.Any(a => a == itm.ID_SECTOR_CLAS))
                                                    {
                                                        gruposIDs.Add(itm.ID_SECTOR_CLAS);
                                                        gruposString = gruposString + (string.IsNullOrEmpty(gruposString) ? "" : ", ") + itm.POBLACION;//+(i == item.PATOLOGICO_CAT.SECTOR_CLASIFICACION.Count() ? "" : ", ");
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
                        Context.SaveChanges();
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

                    #region ENFERMEDADES
                    if (Enfermedades != null ? Enfermedades.Any() : false)
                    {
                        var bandera = false;
                        //var grupos = string.Empty;
                        foreach (var item in Enfermedades)
                        {
                            Context.NOTA_MEDICA_ENFERMEDAD.Add(new NOTA_MEDICA_ENFERMEDAD
                            {
                                ID_ATENCION_MEDICA = AtencionMedica.ID_ATENCION_MEDICA,
                                ID_CENTRO_UBI = AtencionMedica.ID_CENTRO_UBI,
                                ID_ENFERMEDAD = item.ID_ENFERMEDAD,
                                REGISTRO_FEC = item.REGISTRO_FEC
                            });
                            Context.SaveChanges();
                            bandera = bandera ? bandera : item.ENFERMEDAD != null ? item.ENFERMEDAD.SECTOR_CLASIFICACION.Any() : false;
                        }
                        if (bandera)
                        {
                            var i = 1;
                            foreach (var item in Enfermedades.SelectMany(s => s.ENFERMEDAD.SECTOR_CLASIFICACION))
                            {
                                gruposString = gruposString + item.POBLACION + (i == Enfermedades.SelectMany(s => s.ENFERMEDAD.SECTOR_CLASIFICACION).Count() ? "" : ", ");
                                i++;
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
                                        }
                                    }
                                }
                            }
                        }
                        Context.SaveChanges();
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

                    #region DIETAS
                    if (dietas != null ? dietas.Any() : false)
                    {
                        Context.Database.ExecuteSqlCommand("DELETE FROM NOTA_MEDICA_DIETA WHERE ID_ATENCION_MEDICA = " + AtencionMedica.ID_ATENCION_MEDICA + " AND ID_CENTRO_UBI = " + AtencionMedica.ID_CENTRO_UBI);
                        Context.SaveChanges();
                        foreach (var item in dietas)
                        {
                            //if (!Context.NOTA_MEDICA_DIETA.Any(a => a.ID_DIETA == item.ID_DIETA && a.ID_ATENCION_MEDICA == AtencionMedica.ID_ATENCION_MEDICA && a.ID_CENTRO_UBI == AtencionMedica.ID_CENTRO_UBI))
                            Context.NOTA_MEDICA_DIETA.Add(new NOTA_MEDICA_DIETA
                            {
                                ID_ATENCION_MEDICA = AtencionMedica.ID_ATENCION_MEDICA,
                                ID_CENTRO_UBI = AtencionMedica.ID_CENTRO_UBI,
                                DIETA_FEC = FechaServidor,
                                ID_DIETA = item.ID_DIETA,
                                ESTATUS = item.ESTATUS,
                            });
                        }
                        Context.SaveChanges();
                    }
                    #endregion

                    #region PROCEDIMIENTOS MEDICOS
                    var auxiliarProcs = new List<PROC_ATENCION_MEDICA>();
                    if (ProcedimientosCitas != null ? ProcedimientosCitas.Any() : false)
                    {
                        #region DELETE ALL
                        var atcitas = Context.ATENCION_CITA.Any(w => w.ATENCION_MEDICA != null ?
                            (w.ATENCION_MEDICA.ID_ATENCION_MEDICA == AtencionMedica.ID_ATENCION_MEDICA && w.ATENCION_MEDICA.ID_CENTRO_UBI == AtencionMedica.ID_CENTRO_UBI) ?
                                w.ATENCION_MEDICA.NOTA_MEDICA != null ?
                                    w.ATENCION_MEDICA.PROC_ATENCION_MEDICA.Any()
                                : false
                            : false
                        : false) ?
                            Context.ATENCION_CITA.Where(w => w.ATENCION_MEDICA != null ?
                                (w.ATENCION_MEDICA.ID_ATENCION_MEDICA == AtencionMedica.ID_ATENCION_MEDICA && w.ATENCION_MEDICA.ID_CENTRO_UBI == AtencionMedica.ID_CENTRO_UBI) ?
                                    w.ATENCION_MEDICA.NOTA_MEDICA != null ?
                                        w.ATENCION_MEDICA.PROC_ATENCION_MEDICA.Any()
                                    : false
                                : false
                            : false)
                        : null;
                        if (atcitas != null)
                        {
                            var pam = atcitas.SelectMany(s => s.ATENCION_MEDICA.PROC_ATENCION_MEDICA).Any() ? atcitas.SelectMany(s => s.ATENCION_MEDICA.PROC_ATENCION_MEDICA).ToList() : new List<PROC_ATENCION_MEDICA>();
                            var pamp = pam.Any() ? pam.SelectMany(s => s.PROC_ATENCION_MEDICA_PROG) : new List<PROC_ATENCION_MEDICA_PROG>();
                            var citasprog = pamp.Any() ? pamp.Select(s => s.ATENCION_CITA) : new List<ATENCION_CITA>();
                            foreach (var item in pamp)
                            {
                                Context.Database.ExecuteSqlCommand("DELETE FROM PROC_ATENCION_MEDICA_PROG WHERE ID_ATENCION_MEDICA = " + item.ID_ATENCION_MEDICA +
                                    " AND ID_CENTRO_UBI = " + item.ID_CENTRO_UBI + " AND ID_AM_PROG = " + item.ID_AM_PROG + " AND ID_CITA = " + item.ID_CITA);
                                pampEliminada.Add(item.ID_AM_PROG);
                                Context.SaveChanges();
                            }
                            foreach (var item in pam)
                            {
                                if (!ProcedimientosCitas.Any(a => a.PROC_ATENCION_MEDICA_PROG.Any(an => an.PROC_ATENCION_MEDICA.ID_ATENCION_MEDICA == item.ID_ATENCION_MEDICA &&
                                    an.PROC_ATENCION_MEDICA.ID_CENTRO_UBI == item.ID_CENTRO_UBI && an.PROC_ATENCION_MEDICA.ID_PROCMED == item.ID_PROCMED)))
                                {
                                    Context.Database.ExecuteSqlCommand("DELETE FROM PROC_ATENCION_MEDICA WHERE ID_ATENCION_MEDICA = " + item.ID_ATENCION_MEDICA +
                                       " AND ID_CENTRO_UBI = " + item.ID_CENTRO_UBI + " AND ID_PROCMED = " + item.ID_PROCMED);
                                    pamEliminada.Add(item.ID_PROCMED);
                                    Context.SaveChanges();
                                }
                                pamExistencia.Add(item.ID_PROCMED);
                            }
                            foreach (var item in citasprog)
                            {
                                citaEliminada.Add(item.ID_CITA);
                                Context.Database.ExecuteSqlCommand("DELETE FROM ATENCION_CITA WHERE ID_CITA = " + item.ID_CITA);
                                Context.SaveChanges();
                            }
                        }
                        #endregion
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
                                if (citaEliminada.Any(a => a.Equals(citatruena.ID_CITA)))
                                    citatruena.ID_CITA = citatruena.ID_CITA + citaEliminada.Count;
                                Context.ATENCION_CITA.Add(citatruena);
                                Context.SaveChanges();
                                citasAgregadas.Add(citatruena);
                                aux = item.CITA_FECHA_HORA;
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
                                if (citaEliminada.Any(a => a.Equals(citatruena.ID_CITA)))
                                    citatruena.ID_CITA = citatruena.ID_CITA + 1;
                                Context.ATENCION_CITA.Add(citatruena);
                                Context.SaveChanges();
                                citasAgregadas.Add(citatruena);
                                aux = item.CITA_FECHA_HORA;
                                idCita = citatruena.ID_CITA;
                                responsable = item.ID_RESPONSABLE.HasValue ? item.ID_RESPONSABLE.Value : 0;
                            }
                            if (item.PROC_ATENCION_MEDICA_PROG != null ? item.PROC_ATENCION_MEDICA_PROG.Any() : false)
                            {
                                var test = false;
                                foreach (var itm in item.PROC_ATENCION_MEDICA_PROG)
                                {
                                    var idAmProg = GetIDProceso<int>("PROC_ATENCION_MEDICA_PROG", "ID_AM_PROG", "ID_CENTRO_UBI = " + AtencionMedica.ID_CENTRO_UBI);
                                    if (pampEliminada.Any(a => a.Equals(idAmProg)))
                                    {
                                        idAmProg = idAmProg + (test ? 1 : pampEliminada.Count + 1);
                                        test = true;
                                    }
                                    if (!auxiliarProcs.Any(a => a.ID_PROCMED == itm.PROC_ATENCION_MEDICA.ID_PROCMED && a.ID_USUARIO == itm.PROC_ATENCION_MEDICA.ID_USUARIO && a.REGISTRO_FEC == itm.PROC_ATENCION_MEDICA.REGISTRO_FEC &&
                                        a.OBSERV == itm.PROC_ATENCION_MEDICA.OBSERV))
                                    {
                                        itm.PROC_ATENCION_MEDICA.ID_ATENCION_MEDICA = AtencionMedica.ID_ATENCION_MEDICA;
                                        itm.PROC_ATENCION_MEDICA.ID_CENTRO_UBI = AtencionMedica.ID_CENTRO_UBI;
                                        auxiliarProcs.Add(itm.PROC_ATENCION_MEDICA);
                                        if (pamEliminada.Any(a => a.Equals(itm.PROC_ATENCION_MEDICA.ID_PROCMED)))
                                        {
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
                                        else if (!pamExistencia.Any(a => a.Equals(itm.PROC_ATENCION_MEDICA.ID_PROCMED)))
                                        {
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
                                    }
                                    Context.PROC_ATENCION_MEDICA_PROG.Add(new PROC_ATENCION_MEDICA_PROG
                                    {
                                        ID_AM_PROG = idAmProg,
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

                    #region DENTAL
                    if (Odontograma != null ? Odontograma.Any() : false)
                    {
                        foreach (var item in Odontograma)
                        {
                            if (!Context.ODONTOGRAMA_SEGUIMIENTO2.Any(a => a.ID_ATENCION_MEDICA == AtencionMedica.ID_ATENCION_MEDICA && a.ID_CENTRO_UBI == AtencionMedica.ID_CENTRO_UBI &&
                                (a.ID_TRATA == item.ID_TRATA || a.ID_ENFERMEDAD == item.ID_ENFERMEDAD)))
                                Context.ODONTOGRAMA_SEGUIMIENTO2.Add(new ODONTOGRAMA_SEGUIMIENTO2
                                {
                                    ESTATUS = item.ESTATUS,
                                    //ID_TRATA = item.ID_TRATA,
                                    ID_TRATA = item.ID_TRATA.HasValue ? item.ID_TRATA != -1 ? item.ID_TRATA : null : null,
                                    ID_ENFERMEDAD = item.ID_ENFERMEDAD.HasValue ? item.ID_ENFERMEDAD != -1 ? item.ID_ENFERMEDAD : null : null,
                                    //ID_ENFERMEDAD = item.ID_ENFERMEDAD,
                                    ID_TIPO_ODO = item.ID_TIPO_ODO,
                                    ID_POSICION = item.ID_POSICION,
                                    ID_ATENCION_MEDICA = AtencionMedica.ID_ATENCION_MEDICA,
                                    ID_CENTRO_UBI = AtencionMedica.ID_CENTRO_UBI,
                                    REGISTRO_FEC = hoy,
                                    ID_CONSECUTIVO = GetIDProceso<int>("ODONTOGRAMA_SEGUIMIENTO2", "ID_CONSECUTIVO", "ID_CENTRO_UBI = " + AtencionMedica.ID_CENTRO_UBI),
                                });
                            Context.SaveChanges();
                        }
                    }
                    #endregion

                    Context.SaveChanges();
                    transaccion.Complete();
                    return true;
                }
            }
            catch (System.Exception exc)
            {
                throw exc;
            }
        }

        public bool InsertarNotaMedicaTransaccion(DateTime FechaServidor, NOTA_SIGNOS_VITALES SignosVitales, NOTA_MEDICA NotaMedica, ATENCION_MEDICA AtencionMedica, ATENCION_CITA AtencionCita = null,
            NOTA_URGENCIA NotaUrgencia = null, NOTA_INTERCONSULTA NotaInterconsulta = null, NOTA_POST_OPERATOR NotaPostop = null, NOTA_PRE_ANESTECIC NotaPreanestecica = null, NOTA_PRE_OPERATORI NotaPreop = null,
            NOTA_REFERENCIA_TR NotaTraslado = null, NOTA_EVOLUCION NotaEvolucion = null, List<NOTA_MEDICA_ENFERMEDAD> Enfermedades = null, CERTIFICADO_MEDICO Certificado = null, List<LESION> Lesiones = null,
            List<DIETA> dietas = null, EXCARCELACION Excarcelacion = null, TRASLADO_DETALLE TrasladoDetalle = null, List<HISTORIA_CLINICA_PATOLOGICOS> patologicos = null, HISTORIA_CLINICA_GINECO_OBSTRE Mujeres = null,
            ATENCION_CITA Seguimiento = null, RECETA_MEDICA RecetaMedica = null, List<RECETA_MEDICA_DETALLE> RecetaMedicaDetalle = null, List<ATENCION_CITA> ProcedimientosCitas = null, ESPECIALISTA Especialista = null,
            string Usuario = "", List<PROC_ATENCION_MEDICA_PROG> ProcedimientosConCitaExistente = null, short? MensajeEnfermedad = new Nullable<short>(), List<ODONTOGRAMA_SEGUIMIENTO2> Odontograma = null)
        {
            var citatruena = new ATENCION_CITA();
            var citasAgregadas = new List<ATENCION_CITA>();
            var aux = new Nullable<DateTime>();
            var hoy = new DateTime();
            var idCita = 0;
            var responsable = 0;
            var consec = new HISTORIA_CLINICA();
            try
            {
                using (TransactionScope transaccion = new TransactionScope(TransactionScopeOption.RequiresNew, new TransactionOptions() { IsolationLevel = System.Transactions.IsolationLevel.ReadCommitted }))
                {
                    if (Seguimiento != null)
                    {
                        //Seguimiento.ID_CITA = GetSequence<short>("ATENCION_CITA_SEQ");
                        Seguimiento.ID_CITA = GetIDProceso<short>("ATENCION_CITA", "ID_CITA", "ID_CENTRO_UBI = " + AtencionMedica.ID_CENTRO_UBI);
                        Seguimiento.ID_CENTRO_UBI = AtencionMedica.ID_CENTRO_UBI;
                        if (Seguimiento.ID_CENTRO_AT_SOL == null)
                            Seguimiento.ID_CENTRO_AT_SOL = Seguimiento.ID_CENTRO_UBI;
                        AtencionMedica.ID_CITA_SEGUIMIENTO = Seguimiento.ID_CITA;
                        Context.ATENCION_CITA.Add(new ATENCION_CITA
                        {
                            CITA_FECHA_HORA = Seguimiento.CITA_FECHA_HORA,
                            CITA_HORA_TERMINA = Seguimiento.CITA_HORA_TERMINA,
                            ID_ANIO = Seguimiento.ID_ANIO,
                            ID_AREA = Seguimiento.ID_AREA,
                            ID_ATENCION = Seguimiento.ID_ATENCION,
                            ID_ATENCION_MEDICA = Seguimiento.ID_ATENCION_MEDICA,
                            ID_CENTRO = Seguimiento.ID_CENTRO,
                            ID_CITA = Seguimiento.ID_CITA,
                            ID_CENTRO_UBI = Seguimiento.ID_CENTRO_UBI,
                            ID_IMPUTADO = Seguimiento.ID_IMPUTADO,
                            ID_INGRESO = Seguimiento.ID_INGRESO,
                            ID_RESPONSABLE = Seguimiento.ID_RESPONSABLE,
                            ID_TIPO_ATENCION = Seguimiento.ID_TIPO_ATENCION,
                            ID_TIPO_SERVICIO = Seguimiento.ID_TIPO_SERVICIO,
                            ID_USUARIO = Seguimiento.ID_USUARIO,
                            ESTATUS = Seguimiento.ESTATUS,
                            ID_CENTRO_AT_SOL = Seguimiento.ID_CENTRO_AT_SOL,
                        });
                        Context.SaveChanges();
                        if (Especialista != null)
                        {
                            Context.ESPECIALISTA_CITA.Add(new ESPECIALISTA_CITA
                            {
                                ID_CENTRO_UBI = Seguimiento.ID_CENTRO_UBI,
                                ID_CITA = Seguimiento.ID_CITA,
                                ID_ESPECIALIDAD = Especialista.ID_ESPECIALIDAD,
                                ID_ESPECIALISTA = Especialista.ID_ESPECIALISTA,
                                ID_SOLICITUD = new Nullable<int>(),
                                ID_USUARIO = Usuario,
                                REGISTRO_FEC = GetFechaServerDate(),
                            });
                            Context.SaveChanges();
                        }
                    }
                    AtencionMedica.ID_ATENCION_MEDICA = GetIDProceso<short>("ATENCION_MEDICA", "ID_ATENCION_MEDICA", "ID_CENTRO_UBI = " + AtencionMedica.ID_CENTRO_UBI);
                    Context.ATENCION_MEDICA.Add(AtencionMedica);
                    Context.SaveChanges();
                    SignosVitales.ID_ATENCION_MEDICA = AtencionMedica.ID_ATENCION_MEDICA;
                    SignosVitales.ID_CENTRO_UBI = AtencionMedica.ID_CENTRO_UBI;
                    Context.NOTA_SIGNOS_VITALES.Add(SignosVitales);
                    Context.SaveChanges();
                    NotaMedica.ID_ATENCION_MEDICA = AtencionMedica.ID_ATENCION_MEDICA;
                    NotaMedica.ID_CENTRO_UBI = AtencionMedica.ID_CENTRO_UBI;
                    Context.NOTA_MEDICA.Add(NotaMedica);
                    Context.SaveChanges();
                    if (AtencionCita != null)
                    {
                        AtencionCita.ID_ATENCION_MEDICA = AtencionMedica.ID_ATENCION_MEDICA;
                        AtencionCita.ID_CENTRO_UBI = AtencionMedica.ID_CENTRO_UBI;
                        if (AtencionCita.ID_CENTRO_AT_SOL == null)
                            AtencionCita.ID_CENTRO_AT_SOL = AtencionCita.ID_CENTRO_UBI;
                        Context.Entry(AtencionCita).State = EntityState.Modified;
                        Context.SaveChanges();
                    }

                    #region EXCARCELACION Y TRASLADO
                    if (Excarcelacion != null)
                    {
                        if (!Excarcelacion.ID_INCIDENCIA_TRASLADO_RETORNO.HasValue)
                            Excarcelacion.CERT_MEDICO_RETORNO = Excarcelacion.CERT_MEDICO_SALIDA.HasValue ?
                                Excarcelacion.CERT_MEDICO_SALIDA.Value == 0 ?
                                    new Nullable<int>()
                                : Excarcelacion.CERT_MEDICO_RETORNO.HasValue ?
                                    Excarcelacion.CERT_MEDICO_RETORNO.Value == 0 ?
                                        AtencionMedica.ID_ATENCION_MEDICA
                                    : Excarcelacion.CERT_MEDICO_RETORNO.Value
                                : AtencionMedica.ID_ATENCION_MEDICA
                            : new Nullable<int>();

                        if (!Excarcelacion.ID_INCIDENCIA_TRASLADO.HasValue)
                            Excarcelacion.CERT_MEDICO_SALIDA = Excarcelacion.CERT_MEDICO_SALIDA.HasValue ?
                                Excarcelacion.CERT_MEDICO_SALIDA.Value == 0 ?
                                    AtencionMedica.ID_ATENCION_MEDICA
                                : Excarcelacion.CERT_MEDICO_SALIDA
                            : AtencionMedica.ID_ATENCION_MEDICA;
                        //Excarcelacion.CERT_MEDICO_SALIDA = Excarcelacion.CERT_MEDICO_SALIDA.HasValue ? Excarcelacion.CERT_MEDICO_SALIDA.Value == 0 ? new Nullable<int>() : Excarcelacion.CERT_MEDICO_SALIDA.Value : AtencionMedica.ID_ATENCION_MEDICA;
                        //Excarcelacion.ID_ATENCION_MEDICA = AtencionMedica.ID_ATENCION_MEDICA;
                        Excarcelacion.ID_CENTRO_UBI = AtencionMedica.ID_CENTRO_UBI;
                        Context.Entry(Excarcelacion).State = EntityState.Modified;
                    }
                    if (TrasladoDetalle != null)
                    {
                        TrasladoDetalle.ID_ATENCION_MEDICA = AtencionMedica.ID_ATENCION_MEDICA;
                        Context.Entry(TrasladoDetalle).State = EntityState.Modified;
                    }
                    Context.SaveChanges();
                    #endregion

                    #region RECETA
                    if (RecetaMedicaDetalle != null ? RecetaMedicaDetalle.Any() : false)
                    {
                        RecetaMedica.ID_ATENCION_MEDICA = AtencionMedica.ID_ATENCION_MEDICA;
                        RecetaMedica.ID_CENTRO_UBI = AtencionMedica.ID_CENTRO_UBI;
                        RecetaMedica.ID_FOLIO = GetIDProceso<short>("RECETA_MEDICA", "ID_FOLIO", "ID_CENTRO_UBI = " + AtencionMedica.ID_CENTRO_UBI);
                        Context.RECETA_MEDICA.Add(RecetaMedica);
                        foreach (var s in RecetaMedicaDetalle)
                        {
                            Context.RECETA_MEDICA_DETALLE.Add(new RECETA_MEDICA_DETALLE
                            {
                                CENA = s.CENA,
                                COMIDA = s.COMIDA,
                                DESAYUNO = s.DESAYUNO,
                                DOSIS = s.DOSIS,
                                DURACION = s.DURACION,
                                ID_FOLIO = RecetaMedica.ID_FOLIO,
                                ID_PRODUCTO = s.ID_PRODUCTO,
                                OBSERV = s.OBSERV,
                                ID_CENTRO_UBI = AtencionMedica.ID_CENTRO_UBI,
                                ID_ATENCION_MEDICA = AtencionMedica.ID_ATENCION_MEDICA,
                                ID_PRESENTACION_MEDICAMENTO = s.ID_PRESENTACION_MEDICAMENTO == 0 ? new Nullable<short>() : s.ID_PRESENTACION_MEDICAMENTO,
                            });
                        }
                        Context.SaveChanges();
                    }
                    #endregion

                    #region NOTAS
                    if (NotaUrgencia != null)
                    {
                        NotaUrgencia.ID_ATENCION_MEDICA = AtencionMedica.ID_ATENCION_MEDICA;
                        NotaUrgencia.ID_CENTRO_UBI = AtencionMedica.ID_CENTRO_UBI;
                        Context.NOTA_URGENCIA.Add(NotaUrgencia);
                    }
                    if (NotaInterconsulta != null)
                    {
                        NotaInterconsulta.ID_ATENCION_MEDICA = AtencionMedica.ID_ATENCION_MEDICA;
                        NotaInterconsulta.ID_CENTRO_UBI = AtencionMedica.ID_CENTRO_UBI;
                        Context.NOTA_INTERCONSULTA.Add(NotaInterconsulta);
                    }
                    if (NotaTraslado != null)
                    {
                        NotaTraslado.ID_ATENCION_MEDICA = AtencionMedica.ID_ATENCION_MEDICA;
                        NotaTraslado.ID_CENTRO_UBI = AtencionMedica.ID_CENTRO_UBI;
                        Context.NOTA_REFERENCIA_TR.Add(NotaTraslado);
                    }
                    if (NotaPreop != null)
                    {
                        NotaPreop.ID_ATENCION_MEDICA = AtencionMedica.ID_ATENCION_MEDICA;
                        NotaPreop.ID_CENTRO_UBI = AtencionMedica.ID_CENTRO_UBI;
                        Context.NOTA_PRE_OPERATORI.Add(NotaPreop);
                    }
                    if (NotaPreanestecica != null)
                    {
                        NotaPreanestecica.ID_ATENCION_MEDICA = AtencionMedica.ID_ATENCION_MEDICA;
                        NotaPreanestecica.ID_CENTRO_UBI = AtencionMedica.ID_CENTRO_UBI;
                        Context.NOTA_PRE_ANESTECIC.Add(NotaPreanestecica);
                    }
                    if (NotaPostop != null)
                    {
                        NotaPostop.ID_ATENCION_MEDICA = AtencionMedica.ID_ATENCION_MEDICA;
                        NotaPostop.ID_CENTRO_UBI = AtencionMedica.ID_CENTRO_UBI;
                        Context.NOTA_POST_OPERATOR.Add(NotaPostop);
                    }
                    if (NotaEvolucion != null)
                    {
                        NotaEvolucion.ID_ATENCION_MEDICA = AtencionMedica.ID_ATENCION_MEDICA;
                        NotaEvolucion.ID_CENTRO_UBI = AtencionMedica.ID_CENTRO_UBI;
                        Context.NOTA_EVOLUCION.Add(NotaEvolucion);
                    }
                    #endregion

                    consec = Context.HISTORIA_CLINICA.Any(f => f.ID_CENTRO == AtencionMedica.ID_CENTRO && f.ID_ANIO == AtencionMedica.ID_ANIO && f.ID_IMPUTADO == AtencionMedica.ID_IMPUTADO && f.ID_INGRESO == AtencionMedica.ID_INGRESO) ?
                        Context.HISTORIA_CLINICA.First(f => f.ID_CENTRO == AtencionMedica.ID_CENTRO && f.ID_ANIO == AtencionMedica.ID_ANIO && f.ID_IMPUTADO == AtencionMedica.ID_IMPUTADO && f.ID_INGRESO == AtencionMedica.ID_INGRESO)
                    : null;
                    hoy = GetFechaServerDate();

                    #region PATOLOGICOS
                    var gruposString = string.Empty;
                    var gruposIDs = new List<short>();
                    if (patologicos != null ? patologicos.Any() : false)
                        foreach (var item in patologicos)
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
                                        foreach (var itm in item.PATOLOGICO_CAT.SECTOR_CLASIFICACION)
                                        {
                                            var grupoConsec = GetIDProceso<short>("GRUPO_VULNERABLE", "ID_CONSEC", "ID_CENTRO = " + AtencionMedica.ID_CENTRO + " AND ID_ANIO = " + AtencionMedica.ID_ANIO + " AND ID_IMPUTADO = " +
                                                              AtencionMedica.ID_IMPUTADO + " AND ID_INGRESO = " + AtencionMedica.ID_INGRESO + " AND ID_CONSEC = " + consec.ID_CONSEC);
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
                                                    ID_GRUPO_CONSEC = grupoConsec,
                                                    ID_CONSEC = consec.ID_CONSEC,
                                                });
                                                Context.SaveChanges();
                                                if (!gruposIDs.Any(a => a == itm.ID_SECTOR_CLAS))
                                                {
                                                    gruposIDs.Add(itm.ID_SECTOR_CLAS);
                                                    gruposString = gruposString + (string.IsNullOrEmpty(gruposString) ? "" : ", ") + itm.POBLACION;//+(i == item.PATOLOGICO_CAT.SECTOR_CLASIFICACION.Count() ? "" : ", ");
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

                    #region ENFERMEDADES
                    if (Enfermedades != null ? Enfermedades.Any() : false)
                    {
                        var bandera = false;
                        //var grupos = string.Empty;
                        foreach (var item in Enfermedades)
                        {
                            Context.NOTA_MEDICA_ENFERMEDAD.Add(new NOTA_MEDICA_ENFERMEDAD
                            {
                                ID_ATENCION_MEDICA = AtencionMedica.ID_ATENCION_MEDICA,
                                ID_CENTRO_UBI = AtencionMedica.ID_CENTRO_UBI,
                                ID_ENFERMEDAD = item.ID_ENFERMEDAD,
                                REGISTRO_FEC = item.REGISTRO_FEC
                            });
                            Context.SaveChanges();
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
                                        }
                                    }
                                }
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

                    #region DIETAS
                    if (dietas != null ? dietas.Any() : false)
                    {
                        foreach (var item in dietas)
                        {
                            Context.NOTA_MEDICA_DIETA.Add(new NOTA_MEDICA_DIETA
                            {
                                ID_ATENCION_MEDICA = AtencionMedica.ID_ATENCION_MEDICA,
                                ID_CENTRO_UBI = AtencionMedica.ID_CENTRO_UBI,
                                DIETA_FEC = FechaServidor,
                                ID_DIETA = item.ID_DIETA,
                                ESTATUS = "S",
                            });
                        }
                        Context.SaveChanges();
                    }
                    #endregion

                    #region CERTIFICADO
                    if (Certificado != null)
                    {
                        Certificado.ID_ATENCION_MEDICA = AtencionMedica.ID_ATENCION_MEDICA;
                        Certificado.ID_CENTRO_UBI = AtencionMedica.ID_CENTRO_UBI;
                        Context.CERTIFICADO_MEDICO.Add(Certificado);
                        Context.SaveChanges();
                        if (Lesiones != null)
                        {
                            short i = 1;
                            foreach (var item in Lesiones)
                            {
                                Context.LESION.Add(new LESION
                                {
                                    ID_ATENCION_MEDICA = AtencionMedica.ID_ATENCION_MEDICA,
                                    ID_CENTRO_UBI = AtencionMedica.ID_CENTRO_UBI,
                                    ID_IMPUTADO = item.ID_IMPUTADO,
                                    ID_INGRESO = item.ID_INGRESO,
                                    ID_REGION = item.ID_REGION,
                                    ID_CENTRO = item.ID_CENTRO,
                                    ID_ANIO = item.ID_ANIO,
                                    DESCR = item.DESCR,
                                    ID_LESION = i,
                                });
                                i++;
                            }
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
                                foreach (var itm in item.PROC_ATENCION_MEDICA_PROG.Where(w => ProcedimientosConCitaExistente != null ? !ProcedimientosConCitaExistente.Any(a => a.ID_CITA.HasValue ? a.ID_PROCMED == w.ID_PROCMED : false) : true))
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
                    if (ProcedimientosConCitaExistente != null)
                        foreach (var item in ProcedimientosConCitaExistente)
                        {
                            if (!Context.PROC_ATENCION_MEDICA.Any(a => a.ID_ATENCION_MEDICA == AtencionMedica.ID_ATENCION_MEDICA && a.ID_CENTRO_UBI == AtencionMedica.ID_CENTRO_UBI))
                            {
                                Context.PROC_ATENCION_MEDICA.Add(new PROC_ATENCION_MEDICA
                                  {
                                      ID_PROCMED = item.ID_PROCMED,
                                      ID_USUARIO = item.PROC_ATENCION_MEDICA.ID_USUARIO,
                                      OBSERV = item.PROC_ATENCION_MEDICA.OBSERV,
                                      REGISTRO_FEC = item.PROC_ATENCION_MEDICA.REGISTRO_FEC,
                                      ID_ATENCION_MEDICA = AtencionMedica.ID_ATENCION_MEDICA,
                                      ID_CENTRO_UBI = AtencionMedica.ID_CENTRO_UBI,
                                  });
                                Context.SaveChanges();
                            }
                            Context.PROC_ATENCION_MEDICA_PROG.Add(new PROC_ATENCION_MEDICA_PROG
                            {
                                ID_AM_PROG = GetIDProceso<short>("PROC_ATENCION_MEDICA_PROG", "ID_AM_PROG", "ID_CENTRO_UBI = " + AtencionMedica.ID_CENTRO_UBI),
                                ESTATUS = item.ESTATUS,
                                ID_ATENCION_MEDICA = AtencionMedica.ID_ATENCION_MEDICA,
                                ID_CENTRO_UBI = AtencionMedica.ID_CENTRO_UBI,
                                ID_PROCMED = item.ID_PROCMED,
                                ID_USUARIO_ASIGNADO = item.ID_USUARIO_ASIGNADO,
                                ID_CITA = item.ID_CITA,
                                ID_CENTRO_CITA = item.ID_CENTRO_CITA,
                            });
                            Context.SaveChanges();

                        }
                    #endregion

                    #region DENTAL
                    if (Odontograma != null ? Odontograma.Any() : false)
                    {
                        foreach (var item in Odontograma)
                        {
                            Context.ODONTOGRAMA_SEGUIMIENTO2.Add(new ODONTOGRAMA_SEGUIMIENTO2
                            {
                                ESTATUS = item.ESTATUS,
                                //ID_TRATA = item.ID_TRATA,
                                ID_TRATA = item.ID_TRATA.HasValue ? item.ID_TRATA != -1 ? item.ID_TRATA : null : null,
                                ID_ENFERMEDAD = item.ID_ENFERMEDAD.HasValue ? item.ID_ENFERMEDAD != -1 ? item.ID_ENFERMEDAD : null : null,
                                //ID_ENFERMEDAD = item.ID_ENFERMEDAD,
                                ID_TIPO_ODO = item.ID_TIPO_ODO,
                                ID_POSICION = item.ID_POSICION,
                                ID_ATENCION_MEDICA = AtencionMedica.ID_ATENCION_MEDICA,
                                ID_CENTRO_UBI = AtencionMedica.ID_CENTRO_UBI,
                                REGISTRO_FEC = hoy,
                                ID_CONSECUTIVO = GetIDProceso<int>("ODONTOGRAMA_SEGUIMIENTO2", "ID_CONSECUTIVO", "ID_CENTRO_UBI = " + AtencionMedica.ID_CENTRO_UBI),
                            });
                            Context.SaveChanges();
                        }
                    }
                    #endregion

                    Context.SaveChanges();
                    transaccion.Complete();
                    return true;
                }
            }
            catch (System.Exception exc)
            {
                throw exc;
            }
        }

        public bool ActualizarNotaMedicaTransaccion(DateTime FechaServidor, NOTA_MEDICA NotaMedica, ATENCION_MEDICA AtencionMedica, ATENCION_CITA AtencionCita = null, NOTA_URGENCIA NotaUrgencia = null,
            NOTA_INTERCONSULTA NotaInterconsulta = null, NOTA_POST_OPERATOR NotaPostop = null, NOTA_PRE_ANESTECIC NotaPreanestecica = null, NOTA_PRE_OPERATORI NotaPreop = null, NOTA_REFERENCIA_TR NotaTraslado = null,
            NOTA_EVOLUCION NotaEvolucion = null, List<NOTA_MEDICA_ENFERMEDAD> Enfermedades = null, CERTIFICADO_MEDICO Certificado = null, List<LESION> Lesiones = null, List<DIETA> dietas = null,
            EXCARCELACION Excarcelacion = null, TRASLADO_DETALLE TrasladoDetalle = null, List<HISTORIA_CLINICA_PATOLOGICOS> patologicos = null, HISTORIA_CLINICA_GINECO_OBSTRE Mujeres = null,
            ATENCION_CITA Seguimiento = null, RECETA_MEDICA RecetaMedica = null, List<RECETA_MEDICA_DETALLE> RecetaMedicaDetalle = null, List<ATENCION_CITA> ProcedimientosCitas = null, ESPECIALISTA Especialista = null,
            string Usuario = "", List<PROC_ATENCION_MEDICA_PROG> ProcedimientosConCitaExistente = null, short? MensajeEnfermedad = new Nullable<short>(), List<ODONTOGRAMA_SEGUIMIENTO2> Odontograma = null)
        {
            var citatruena = new ATENCION_CITA();
            var citasAgregadas = new List<ATENCION_CITA>();
            var aux = new Nullable<DateTime>();
            var hoy = new DateTime();
            var idCita = 0;
            var responsable = 0;
            var consec = new HISTORIA_CLINICA();
            try
            {
                using (TransactionScope transaccion = new TransactionScope(TransactionScopeOption.RequiresNew, new TransactionOptions() { IsolationLevel = System.Transactions.IsolationLevel.ReadCommitted }))
                {
                    if (Seguimiento != null)
                    {
                        Seguimiento.ID_CITA = GetIDProceso<short>("ATENCION_CITA", "ID_CITA", "ID_CENTRO_UBI = " + AtencionMedica.ID_CENTRO_UBI);
                        AtencionMedica.ID_CITA_SEGUIMIENTO = Seguimiento.ID_CITA;
                        if (Seguimiento.ID_CENTRO_AT_SOL == null)
                            Seguimiento.ID_CENTRO_AT_SOL = Seguimiento.ID_CENTRO_UBI;
                        Context.ATENCION_CITA.Add(new ATENCION_CITA
                        {
                            CITA_FECHA_HORA = Seguimiento.CITA_FECHA_HORA,
                            CITA_HORA_TERMINA = Seguimiento.CITA_HORA_TERMINA,
                            ID_ANIO = Seguimiento.ID_ANIO,
                            ID_AREA = Seguimiento.ID_AREA,
                            ID_ATENCION = Seguimiento.ID_ATENCION,
                            ID_ATENCION_MEDICA = Seguimiento.ID_ATENCION_MEDICA,
                            ID_CENTRO = Seguimiento.ID_CENTRO,
                            ID_CITA = Seguimiento.ID_CITA,
                            ID_CENTRO_UBI = Seguimiento.ID_CENTRO_UBI,
                            ID_IMPUTADO = Seguimiento.ID_IMPUTADO,
                            ID_INGRESO = Seguimiento.ID_INGRESO,
                            ID_RESPONSABLE = Seguimiento.ID_RESPONSABLE,
                            ID_TIPO_ATENCION = Seguimiento.ID_TIPO_ATENCION,
                            ID_TIPO_SERVICIO = Seguimiento.ID_TIPO_SERVICIO,
                            ID_USUARIO = Seguimiento.ID_USUARIO,
                            ESTATUS = Seguimiento.ESTATUS,
                            ID_CENTRO_AT_SOL = Seguimiento.ID_CENTRO_AT_SOL,
                        });
                        Context.SaveChanges();
                        if (Especialista != null)
                        {
                            Context.ESPECIALISTA_CITA.Add(new ESPECIALISTA_CITA
                            {
                                ID_CENTRO_UBI = Seguimiento.ID_CENTRO_UBI,
                                ID_CITA = Seguimiento.ID_CITA,
                                ID_ESPECIALIDAD = Especialista.ID_ESPECIALIDAD,
                                ID_ESPECIALISTA = Especialista.ID_ESPECIALISTA,
                                ID_SOLICITUD = new Nullable<int>(),
                                ID_USUARIO = Usuario,
                                REGISTRO_FEC = GetFechaServerDate(),
                            });
                        }
                    }
                    if (AtencionMedica.ID_ATENCION_MEDICA == 0)
                    {
                        AtencionMedica.ID_ATENCION_MEDICA = GetIDProceso<short>("ATENCION_MEDICA", "ID_ATENCION_MEDICA", "ID_CENTRO_UBI = " + AtencionMedica.ID_CENTRO_UBI);
                        Context.ATENCION_MEDICA.Add(AtencionMedica);
                        Context.SaveChanges();
                    }
                    NotaMedica.ID_ATENCION_MEDICA = AtencionMedica.ID_ATENCION_MEDICA;
                    NotaMedica.ID_CENTRO_UBI = AtencionMedica.ID_CENTRO_UBI;
                    Context.NOTA_MEDICA.Add(NotaMedica);
                    Context.SaveChanges();
                    if (AtencionCita != null)
                    {
                        AtencionCita.ID_ATENCION_MEDICA = AtencionMedica.ID_ATENCION_MEDICA;
                        AtencionCita.ID_CENTRO_UBI = AtencionMedica.ID_CENTRO_UBI;
                        Context.Entry(AtencionCita).State = EntityState.Modified;
                        Context.SaveChanges();
                    }

                    #region EXCARCELACION Y TRASLADO
                    if (Excarcelacion != null)
                    {
                        Excarcelacion.CERT_MEDICO_RETORNO = Excarcelacion.CERT_MEDICO_SALIDA.HasValue ?
                            Excarcelacion.CERT_MEDICO_SALIDA.Value == 0 ?
                                new Nullable<int>()
                            : Excarcelacion.CERT_MEDICO_RETORNO.HasValue ?
                                Excarcelacion.CERT_MEDICO_RETORNO.Value == 0 ?
                                    AtencionMedica.ID_ATENCION_MEDICA
                                : Excarcelacion.CERT_MEDICO_RETORNO.Value
                            : AtencionMedica.ID_ATENCION_MEDICA
                        : new Nullable<int>();
                        Excarcelacion.CERT_MEDICO_SALIDA = Excarcelacion.CERT_MEDICO_SALIDA.HasValue ?
                            Excarcelacion.CERT_MEDICO_SALIDA.Value == 0 ?
                                AtencionMedica.ID_ATENCION_MEDICA
                            : Excarcelacion.CERT_MEDICO_SALIDA
                        : AtencionMedica.ID_ATENCION_MEDICA;
                        //Excarcelacion.CERT_MEDICO_SALIDA = Excarcelacion.CERT_MEDICO_SALIDA.HasValue ? Excarcelacion.CERT_MEDICO_SALIDA.Value == 0 ? new Nullable<int>() : Excarcelacion.CERT_MEDICO_SALIDA.Value : AtencionMedica.ID_ATENCION_MEDICA;
                        //Excarcelacion.ID_ATENCION_MEDICA = AtencionMedica.ID_ATENCION_MEDICA;
                        Excarcelacion.ID_CENTRO_UBI = AtencionMedica.ID_CENTRO_UBI;
                        Context.Entry(Excarcelacion).State = EntityState.Modified;
                    }
                    if (TrasladoDetalle != null)
                    {
                        TrasladoDetalle.ID_ATENCION_MEDICA = AtencionMedica.ID_ATENCION_MEDICA;
                        Context.Entry(TrasladoDetalle).State = EntityState.Modified;
                    }
                    Context.SaveChanges();
                    #endregion

                    #region RECETAS
                    if (RecetaMedicaDetalle != null ? RecetaMedicaDetalle.Any() : false)
                    {
                        RecetaMedica.ID_ATENCION_MEDICA = AtencionMedica.ID_ATENCION_MEDICA;
                        RecetaMedica.ID_CENTRO_UBI = AtencionMedica.ID_CENTRO_UBI;
                        RecetaMedica.ID_FOLIO = GetIDProceso<short>("RECETA_MEDICA", "ID_FOLIO", "ID_CENTRO_UBI = " + AtencionMedica.ID_CENTRO_UBI);
                        Context.RECETA_MEDICA.Add(RecetaMedica);
                        Context.SaveChanges();
                        foreach (var s in RecetaMedicaDetalle)
                        {
                            Context.RECETA_MEDICA_DETALLE.Add(new RECETA_MEDICA_DETALLE
                            {
                                CENA = s.CENA,
                                COMIDA = s.COMIDA,
                                DESAYUNO = s.DESAYUNO,
                                DOSIS = s.DOSIS,
                                DURACION = s.DURACION,
                                ID_FOLIO = RecetaMedica.ID_FOLIO,
                                ID_PRODUCTO = s.ID_PRODUCTO,
                                ID_ATENCION_MEDICA = AtencionMedica.ID_ATENCION_MEDICA,
                                ID_CENTRO_UBI = AtencionMedica.ID_CENTRO_UBI,
                                OBSERV = s.OBSERV,
                                ID_PRESENTACION_MEDICAMENTO = s.ID_PRESENTACION_MEDICAMENTO == 0 ? new Nullable<short>() : s.ID_PRESENTACION_MEDICAMENTO,
                            });
                        }
                        Context.SaveChanges();
                    }
                    #endregion

                    #region NOTAS
                    if (NotaUrgencia != null)
                    {
                        NotaUrgencia.ID_ATENCION_MEDICA = AtencionMedica.ID_ATENCION_MEDICA;
                        NotaUrgencia.ID_CENTRO_UBI = AtencionMedica.ID_CENTRO_UBI;
                        Context.NOTA_URGENCIA.Add(NotaUrgencia);
                    }
                    if (NotaInterconsulta != null)
                    {
                        NotaInterconsulta.ID_CENTRO_UBI = AtencionMedica.ID_CENTRO_UBI;
                        NotaInterconsulta.ID_ATENCION_MEDICA = AtencionMedica.ID_ATENCION_MEDICA;
                        Context.NOTA_INTERCONSULTA.Add(NotaInterconsulta);
                    }
                    if (NotaTraslado != null)
                    {
                        NotaTraslado.ID_CENTRO_UBI = AtencionMedica.ID_CENTRO_UBI;
                        NotaTraslado.ID_ATENCION_MEDICA = AtencionMedica.ID_ATENCION_MEDICA;
                        Context.NOTA_REFERENCIA_TR.Add(NotaTraslado);
                    }
                    if (NotaPreop != null)
                    {
                        NotaPreop.ID_CENTRO_UBI = AtencionMedica.ID_CENTRO_UBI;
                        NotaPreop.ID_ATENCION_MEDICA = AtencionMedica.ID_ATENCION_MEDICA;
                        Context.NOTA_PRE_OPERATORI.Add(NotaPreop);
                    }
                    if (NotaPreanestecica != null)
                    {
                        NotaPreanestecica.ID_CENTRO_UBI = AtencionMedica.ID_CENTRO_UBI;
                        NotaPreanestecica.ID_ATENCION_MEDICA = AtencionMedica.ID_ATENCION_MEDICA;
                        Context.NOTA_PRE_ANESTECIC.Add(NotaPreanestecica);
                    }
                    if (NotaPostop != null)
                    {
                        NotaPostop.ID_CENTRO_UBI = AtencionMedica.ID_CENTRO_UBI;
                        NotaPostop.ID_ATENCION_MEDICA = AtencionMedica.ID_ATENCION_MEDICA;
                        Context.NOTA_POST_OPERATOR.Add(NotaPostop);
                    }
                    if (NotaEvolucion != null)
                    {
                        NotaEvolucion.ID_CENTRO_UBI = AtencionMedica.ID_CENTRO_UBI;
                        NotaEvolucion.ID_ATENCION_MEDICA = AtencionMedica.ID_ATENCION_MEDICA;
                        Context.NOTA_EVOLUCION.Add(NotaEvolucion);
                    }
                    #endregion

                    consec = Context.HISTORIA_CLINICA.First(f => f.ID_CENTRO == AtencionMedica.ID_CENTRO && f.ID_ANIO == AtencionMedica.ID_ANIO && f.ID_IMPUTADO == AtencionMedica.ID_IMPUTADO && f.ID_INGRESO == AtencionMedica.ID_INGRESO);
                    hoy = GetFechaServerDate();

                    #region ENFERMEDADES
                    var gruposString = string.Empty;
                    var gruposIDs = new List<short>();
                    if (Enfermedades != null ? Enfermedades.Any() : false)
                    {
                        var bandera = false;
                        //var grupos = string.Empty;
                        foreach (var item in Enfermedades)
                        {
                            Context.NOTA_MEDICA_ENFERMEDAD.Add(new NOTA_MEDICA_ENFERMEDAD
                            {
                                ID_ATENCION_MEDICA = AtencionMedica.ID_ATENCION_MEDICA,
                                ID_CENTRO_UBI = AtencionMedica.ID_CENTRO_UBI,
                                ID_ENFERMEDAD = item.ID_ENFERMEDAD,
                                REGISTRO_FEC = item.REGISTRO_FEC
                            });
                            Context.SaveChanges();
                            bandera = bandera ? bandera : item.ENFERMEDAD != null ? item.ENFERMEDAD.SECTOR_CLASIFICACION.Any() : false;
                        }
                        if (bandera)
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
                                        }
                                    }
                                }
                            }
                        }
                    }
                    #endregion

                    #region PATOLOGICOS
                    if (patologicos != null ? patologicos.Any() : false)
                    {
                        foreach (var item in patologicos)
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
                                                }
                                            }
                                        }
                                }
                            }
                            else
                            {
                                /*var historia = Context.HISTORIA_CLINICA_PATOLOGICOS.First(w => w.ID_CENTRO == item.ID_CENTRO && w.ID_ANIO == item.ID_ANIO && w.ID_IMPUTADO == item.ID_IMPUTADO && w.ID_INGRESO == item.ID_INGRESO &&
                                    w.ID_CONSEC == item.ID_CONSEC && w.ID_PATOLOGICO == item.ID_PATOLOGICO && w.ID_NOPATOLOGICO == item.ID_NOPATOLOGICO);
                                if (historia != null ? historia.RECUPERADO == "N" : false)
                                {
                                    item.RECUPERADO = "S";
                                    Context.Entry(historia).CurrentValues.SetValues(item);
                                }*/
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
                        Context.SaveChanges();
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

                    #region DIETAS
                    if (dietas != null ? dietas.Any() : false)
                    {
                        foreach (var item in dietas)
                        {
                            Context.NOTA_MEDICA_DIETA.Add(new NOTA_MEDICA_DIETA
                            {
                                DIETA_FEC = FechaServidor,
                                ID_ATENCION_MEDICA = AtencionMedica.ID_ATENCION_MEDICA,
                                ID_CENTRO_UBI = AtencionMedica.ID_CENTRO_UBI,
                                ID_DIETA = item.ID_DIETA,
                                ESTATUS = "S",
                            });
                        }
                        Context.SaveChanges();
                    }
                    #endregion

                    #region CERTIFICADO
                    if (Certificado != null)
                    {
                        Certificado.ID_CENTRO_UBI = AtencionMedica.ID_CENTRO_UBI;
                        Certificado.ID_ATENCION_MEDICA = AtencionMedica.ID_ATENCION_MEDICA;
                        if (Context.CERTIFICADO_MEDICO.Any(a => a.ID_ATENCION_MEDICA == AtencionMedica.ID_ATENCION_MEDICA && a.ID_CENTRO_UBI == AtencionMedica.ID_CENTRO_UBI))
                            Context.Entry(Certificado).State = EntityState.Modified;
                        else
                            Context.CERTIFICADO_MEDICO.Add(Certificado);

                        Context.SaveChanges();
                        if (Lesiones != null)
                        {
                            short i = 1;
                            foreach (var item in Lesiones)
                            {
                                Context.LESION.Add(new LESION
                                {
                                    DESCR = item.DESCR,
                                    ID_ANIO = item.ID_ANIO,
                                    ID_CENTRO = item.ID_CENTRO,
                                    ID_IMPUTADO = item.ID_IMPUTADO,
                                    ID_INGRESO = item.ID_INGRESO,
                                    ID_REGION = item.ID_REGION,
                                    ID_ATENCION_MEDICA = AtencionMedica.ID_ATENCION_MEDICA,
                                    ID_CENTRO_UBI = AtencionMedica.ID_CENTRO_UBI,
                                    ID_LESION = i,
                                });
                                i++;
                            }
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

                    #region DENTAL
                    if (Odontograma != null ? Odontograma.Any() : false)
                    {
                        foreach (var item in Odontograma)
                        {
                            Context.ODONTOGRAMA_SEGUIMIENTO2.Add(new ODONTOGRAMA_SEGUIMIENTO2
                            {
                                ESTATUS = item.ESTATUS,
                                //ID_TRATA = item.ID_TRATA,
                                ID_TRATA = item.ID_TRATA.HasValue ? item.ID_TRATA != -1 ? item.ID_TRATA : null : null,
                                ID_ENFERMEDAD = item.ID_ENFERMEDAD.HasValue ? item.ID_ENFERMEDAD != -1 ? item.ID_ENFERMEDAD : null : null,
                                //ID_ENFERMEDAD = item.ID_ENFERMEDAD,
                                ID_TIPO_ODO = item.ID_TIPO_ODO,
                                ID_POSICION = item.ID_POSICION,
                                ID_ATENCION_MEDICA = AtencionMedica.ID_ATENCION_MEDICA,
                                ID_CENTRO_UBI = AtencionMedica.ID_CENTRO_UBI,
                                REGISTRO_FEC = hoy,
                                ID_CONSECUTIVO = GetIDProceso<int>("ODONTOGRAMA_SEGUIMIENTO2", "ID_CONSECUTIVO", "ID_CENTRO_UBI = " + AtencionMedica.ID_CENTRO_UBI),
                            });
                            Context.SaveChanges();
                        }
                    }
                    #endregion

                    Context.SaveChanges();
                    transaccion.Complete();
                    return true;
                }
            }

            catch (System.Exception exc)
            {
                throw exc;
            }
        }

        public bool InsertarHistoriaClinicaXEnfermero(HISTORIA_CLINICA HistoriaClinica = null, ATENCION_CITA AtencionCita = null, HISTORIA_CLINICA_DENTAL HistoriaClinicaDental = null)
        {
            try
            {
                using (TransactionScope transaccion = new TransactionScope(TransactionScopeOption.RequiresNew, new TransactionOptions() { IsolationLevel = System.Transactions.IsolationLevel.ReadCommitted }))
                {
                    if (HistoriaClinica != null)
                    {
                        var historia = Context.HISTORIA_CLINICA.Any(a => a.ID_CENTRO == HistoriaClinica.ID_CENTRO && a.ID_ANIO == HistoriaClinica.ID_ANIO && a.ID_IMPUTADO == HistoriaClinica.ID_IMPUTADO && a.ID_INGRESO == HistoriaClinica.ID_INGRESO) ?
                            Context.HISTORIA_CLINICA.First(a => a.ID_CENTRO == HistoriaClinica.ID_CENTRO && a.ID_ANIO == HistoriaClinica.ID_ANIO && a.ID_IMPUTADO == HistoriaClinica.ID_IMPUTADO && a.ID_INGRESO == HistoriaClinica.ID_INGRESO)
                        : null;
                        if (historia != null)
                        {
                            historia.EF_PESO = HistoriaClinica.EF_PESO;
                            historia.EF_PRESION_ARTERIAL = HistoriaClinica.EF_PRESION_ARTERIAL;
                            historia.EF_RESPIRACION = HistoriaClinica.EF_RESPIRACION;
                            historia.EF_PULSO = HistoriaClinica.EF_PULSO;
                            historia.EF_TEMPERATURA = HistoriaClinica.EF_TEMPERATURA;
                            historia.ESTUDIO_FEC = HistoriaClinica.ESTUDIO_FEC;
                            historia.ESTATUS = HistoriaClinica.ESTATUS;
                            historia.EF_ESTATURA = HistoriaClinica.EF_ESTATURA;
                            Context.Entry(historia).State = EntityState.Modified;
                        }
                        else
                        {
                            HistoriaClinica.ID_CONSEC = 1;
                            Context.HISTORIA_CLINICA.Add(HistoriaClinica);
                        }
                    }
                    if (HistoriaClinicaDental != null)
                    {
                        var historia = Context.HISTORIA_CLINICA_DENTAL.Any(a => a.ID_CENTRO == HistoriaClinicaDental.ID_CENTRO && a.ID_ANIO == HistoriaClinicaDental.ID_ANIO && a.ID_IMPUTADO == HistoriaClinicaDental.ID_IMPUTADO && a.ID_INGRESO == HistoriaClinicaDental.ID_INGRESO) ?
                            Context.HISTORIA_CLINICA_DENTAL.First(a => a.ID_CENTRO == HistoriaClinicaDental.ID_CENTRO && a.ID_ANIO == HistoriaClinicaDental.ID_ANIO && a.ID_IMPUTADO == HistoriaClinicaDental.ID_IMPUTADO && a.ID_INGRESO == HistoriaClinicaDental.ID_INGRESO)
                        : null;
                        if (historia != null)
                        {
                            historia.PESO = HistoriaClinicaDental.PESO;
                            historia.TENSION_ARTERIAL = HistoriaClinicaDental.TENSION_ARTERIAL;
                            historia.FRECUENCIA_RESPIRA = HistoriaClinicaDental.FRECUENCIA_RESPIRA;
                            historia.FRECUENCIA_CARDIAC = HistoriaClinicaDental.FRECUENCIA_CARDIAC;
                            historia.TEMPERATURA = HistoriaClinicaDental.TEMPERATURA;
                            historia.REGISTRO_FEC = HistoriaClinicaDental.REGISTRO_FEC;
                            historia.ESTATUS = HistoriaClinicaDental.ESTATUS;
                            historia.GLICEMIA = HistoriaClinicaDental.GLICEMIA;
                            historia.ESTATURA = HistoriaClinicaDental.ESTATURA;
                            Context.Entry(historia).State = EntityState.Modified;
                        }
                        else
                        {
                            HistoriaClinicaDental.ID_CONSEC = 1;
                            Context.HISTORIA_CLINICA_DENTAL.Add(HistoriaClinicaDental);
                        }
                    }
                    if (AtencionCita != null)
                        Context.Entry(AtencionCita).State = EntityState.Modified;
                    Context.SaveChanges();
                    transaccion.Complete();
                    return true;
                }
            }

            catch (System.Exception exc)
            {
                throw exc;
            }
        }

        public bool InsertarNotaMedicaXDentista(ATENCION_CITA AtencionCita = null, HISTORIA_CLINICA_DENTAL HistoriaClinicaDental = null, ATENCION_MEDICA AtencionMedica = null)
        {
            try
            {
                using (TransactionScope transaccion = new TransactionScope(TransactionScopeOption.RequiresNew, new TransactionOptions() { IsolationLevel = System.Transactions.IsolationLevel.ReadCommitted }))
                {
                    if (HistoriaClinicaDental != null)
                    {
                        var historia = Context.HISTORIA_CLINICA_DENTAL.Any(a => a.ID_CENTRO == HistoriaClinicaDental.ID_CENTRO && a.ID_ANIO == HistoriaClinicaDental.ID_ANIO && a.ID_IMPUTADO == HistoriaClinicaDental.ID_IMPUTADO &&
                            a.ID_INGRESO == HistoriaClinicaDental.ID_INGRESO) ?
                                Context.HISTORIA_CLINICA_DENTAL.First(a => a.ID_CENTRO == HistoriaClinicaDental.ID_CENTRO && a.ID_ANIO == HistoriaClinicaDental.ID_ANIO && a.ID_IMPUTADO == HistoriaClinicaDental.ID_IMPUTADO &&
                                    a.ID_INGRESO == HistoriaClinicaDental.ID_INGRESO) : null;
                        if (historia != null)
                        {
                            historia.PESO = HistoriaClinicaDental.PESO;
                            historia.TENSION_ARTERIAL = HistoriaClinicaDental.TENSION_ARTERIAL;
                            historia.FRECUENCIA_RESPIRA = HistoriaClinicaDental.FRECUENCIA_RESPIRA;
                            historia.FRECUENCIA_CARDIAC = HistoriaClinicaDental.FRECUENCIA_CARDIAC;
                            historia.TEMPERATURA = HistoriaClinicaDental.TEMPERATURA;
                            historia.REGISTRO_FEC = HistoriaClinicaDental.REGISTRO_FEC;
                            historia.ESTATUS = HistoriaClinicaDental.ESTATUS;
                            historia.GLICEMIA = HistoriaClinicaDental.GLICEMIA;
                            Context.Entry(historia).State = EntityState.Modified;
                        }
                        else
                        {
                            HistoriaClinicaDental.ID_CONSEC = 1;
                            Context.HISTORIA_CLINICA_DENTAL.Add(HistoriaClinicaDental);
                        }
                    }
                    if (AtencionMedica != null)
                    {
                        var atencion = Context.ATENCION_MEDICA.Any(a => a.ID_CENTRO == AtencionMedica.ID_CENTRO && a.ID_ANIO == AtencionMedica.ID_ANIO && a.ID_IMPUTADO == AtencionMedica.ID_IMPUTADO &&
                            a.ID_INGRESO == AtencionMedica.ID_INGRESO && a.ID_ATENCION_MEDICA == AtencionMedica.ID_ATENCION_MEDICA) ?
                                Context.ATENCION_MEDICA.First(a => a.ID_CENTRO == AtencionMedica.ID_CENTRO && a.ID_ANIO == AtencionMedica.ID_ANIO && a.ID_IMPUTADO == AtencionMedica.ID_IMPUTADO &&
                                    a.ID_INGRESO == AtencionMedica.ID_INGRESO && a.ID_ATENCION_MEDICA == AtencionMedica.ID_ATENCION_MEDICA) : null;
                        if (atencion != null)
                            Context.Entry(AtencionMedica).State = EntityState.Modified;
                        else
                        {
                            AtencionMedica.ID_ATENCION_MEDICA = GetIDProceso<short>("ATENCION_MEDICA", "ID_ATENCION_MEDICA", "ID_CENTRO_UBI = " + AtencionMedica.ID_CENTRO_UBI);
                            Context.ATENCION_MEDICA.Add(AtencionMedica);
                            if (AtencionCita != null)
                            {
                                AtencionCita.ID_ATENCION_MEDICA = AtencionMedica.ID_ATENCION_MEDICA;
                                Context.Entry(AtencionCita).State = EntityState.Modified;
                            }
                        }
                    }
                    if (AtencionCita != null)
                        Context.Entry(AtencionCita).State = EntityState.Modified;
                    Context.SaveChanges();
                    transaccion.Complete();
                    return true;
                }
            }

            catch (System.Exception exc)
            {
                throw exc;
            }
        }

        public bool InsertarNotaMedicaEnfermeroTransaccion(NOTA_SIGNOS_VITALES SignosVitales, ATENCION_MEDICA AtencionMedica, ATENCION_CITA AtencionCita = null, EXCARCELACION Excarcelacion = null, TRASLADO_DETALLE TrasladoDetalle = null,
            bool EsCertificado = false, string EsSancion = null)
        {
            try
            {
                using (TransactionScope transaccion = new TransactionScope(TransactionScopeOption.RequiresNew, new TransactionOptions() { IsolationLevel = System.Transactions.IsolationLevel.ReadCommitted }))
                {
                    if (AtencionMedica.ID_ATENCION_MEDICA != 0) return true;
                    AtencionMedica.ID_ATENCION_MEDICA = GetIDProceso<short>("ATENCION_MEDICA", "ID_ATENCION_MEDICA", "ID_CENTRO_UBI = " + AtencionMedica.ID_CENTRO_UBI);
                    Context.ATENCION_MEDICA.Add(AtencionMedica);
                    Context.SaveChanges();
                    SignosVitales.ID_ATENCION_MEDICA = AtencionMedica.ID_ATENCION_MEDICA;
                    Context.NOTA_SIGNOS_VITALES.Add(SignosVitales);
                    Context.SaveChanges();
                    if (EsCertificado)
                    {
                        Context.CERTIFICADO_MEDICO.Add(new CERTIFICADO_MEDICO
                        {
                            ID_ATENCION_MEDICA = AtencionMedica.ID_ATENCION_MEDICA,
                            ID_CENTRO_UBI = AtencionMedica.ID_CENTRO_UBI,
                            ES_SANCION = EsSancion,
                            ESTATUS = "1",
                        });
                        Context.SaveChanges();
                    }
                    if (AtencionCita != null)
                    {
                        AtencionCita.ID_ATENCION_MEDICA = AtencionMedica.ID_ATENCION_MEDICA;
                        Context.Entry(AtencionCita).State = EntityState.Modified;
                        Context.SaveChanges();
                    }
                    if (Excarcelacion != null)
                    {
                        Excarcelacion.CERT_MEDICO_RETORNO = Excarcelacion.CERT_MEDICO_SALIDA.HasValue && !Excarcelacion.CERT_MEDICO_RETORNO.HasValue ? AtencionMedica.ID_ATENCION_MEDICA : Excarcelacion.CERT_MEDICO_RETORNO;
                        Excarcelacion.CERT_MEDICO_SALIDA = Excarcelacion.CERT_MEDICO_SALIDA.HasValue ? Excarcelacion.CERT_MEDICO_SALIDA.Value : AtencionMedica.ID_ATENCION_MEDICA;
                        Context.Entry(Excarcelacion).State = EntityState.Modified;
                        Context.SaveChanges();
                    }
                    if (TrasladoDetalle != null)
                    {
                        TrasladoDetalle.ID_ATENCION_MEDICA = AtencionMedica.ID_ATENCION_MEDICA;
                        Context.Entry(TrasladoDetalle).State = EntityState.Modified;
                        Context.SaveChanges();
                    }
                    Context.SaveChanges();
                    transaccion.Complete();
                    return true;
                }
            }

            catch (System.Exception exc)
            {
                throw exc;
            }
        }

        public bool InsertarProcedimientoMedico(ATENCION_CITA AtencionCita = null, List<PROC_MEDICO_PROG_DET> ProcMedProgDet = null, ATENCION_MEDICA AtencionMedica = null, NOTA_SIGNOS_VITALES NotaSignosVitales = null,
            PROC_ATENCION_MEDICA_PROG ProcAtencionMedicaProg = null, DateTime? hoy = new Nullable<DateTime>(), List<PROC_ATENCION_MEDICA_PROG_INCI> ProcedimientosIncidencias = null, List<ATENCION_CITA> ProcedimientosCitas = null,
            List<PROC_ATENCION_MEDICA_PROG> ProcedimientoSinMateriales = null, List<ODONTOGRAMA_SEGUIMIENTO2> Odontograma = null)
        {
            try
            {
                using (TransactionScope transaccion = new TransactionScope(TransactionScopeOption.RequiresNew, new TransactionOptions() { IsolationLevel = System.Transactions.IsolationLevel.ReadCommitted }))
                {
                    if (AtencionMedica != null)
                    {
                        AtencionMedica.ID_ATENCION_MEDICA = GetIDProceso<short>("ATENCION_MEDICA", "ID_ATENCION_MEDICA", "ID_CENTRO_UBI = " + AtencionMedica.ID_CENTRO_UBI);
                        Context.ATENCION_MEDICA.Add(AtencionMedica);
                        Context.SaveChanges();
                        if (NotaSignosVitales != null)
                        {
                            NotaSignosVitales.ID_ATENCION_MEDICA = AtencionMedica.ID_ATENCION_MEDICA;
                            NotaSignosVitales.ID_CENTRO_UBI = AtencionMedica.ID_CENTRO_UBI;
                            Context.NOTA_SIGNOS_VITALES.Add(NotaSignosVitales);
                            Context.SaveChanges();
                        }
                    }
                    if (AtencionCita != null)
                    {
                        if (AtencionCita.ID_CENTRO_AT_SOL == null)
                            AtencionCita.ID_CENTRO_AT_SOL = AtencionCita.ID_CENTRO_UBI;
                        Context.Entry(new ATENCION_CITA
                               {
                                   CITA_FECHA_HORA = AtencionCita.CITA_FECHA_HORA,
                                   CITA_HORA_TERMINA = AtencionCita.CITA_HORA_TERMINA,
                                   ESTATUS = AtencionCita.ESTATUS,
                                   ID_ANIO = AtencionCita.ID_ANIO,
                                   ID_AREA = AtencionCita.ID_AREA,
                                   ID_ATENCION = AtencionCita.ID_ATENCION,
                                   ID_ATENCION_MEDICA = AtencionMedica.ID_ATENCION_MEDICA,
                                   ID_CENTRO = AtencionCita.ID_CENTRO,
                                   ID_CITA = AtencionCita.ID_CITA,
                                   ID_CENTRO_UBI = AtencionCita.ID_CENTRO_UBI,
                                   ID_IMPUTADO = AtencionCita.ID_IMPUTADO,
                                   ID_INGRESO = AtencionCita.ID_INGRESO,
                                   ID_RESPONSABLE = AtencionCita.ID_RESPONSABLE,
                                   ID_TIPO_ATENCION = AtencionCita.ID_TIPO_ATENCION,
                                   ID_TIPO_SERVICIO = AtencionCita.ID_TIPO_SERVICIO,
                                   ID_USUARIO = AtencionCita.ID_USUARIO,
                                   ID_CENTRO_AT_SOL = AtencionCita.ID_CENTRO_AT_SOL,
                               }).State = EntityState.Modified;
                        Context.SaveChanges();
                    }

                    #region PROCEDIMIENTOS MEDICOS
                    var auxiliarProcs = new List<PROC_ATENCION_MEDICA>();
                    if (ProcedimientosCitas != null ? ProcedimientosCitas.Any() : false)
                    {
                        var aux = new Nullable<DateTime>();
                        var idCita = 0;
                        foreach (var item in ProcedimientosCitas)
                        {
                            if (!aux.HasValue)
                            {
                                if (item.ID_CENTRO_AT_SOL == null)
                                    item.ID_CENTRO_AT_SOL = item.ID_CENTRO_UBI;
                                Context.ATENCION_CITA.Add(new ATENCION_CITA
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
                                    ID_CENTRO_AT_SOL = AtencionCita.ID_CENTRO_AT_SOL,
                                });
                                Context.SaveChanges();
                                aux = item.CITA_FECHA_HORA;
                                idCita = item.ID_CITA;
                            }
                            else if (!(item.CITA_FECHA_HORA.Value.Year == aux.Value.Year && item.CITA_FECHA_HORA.Value.Month == aux.Value.Month && item.CITA_FECHA_HORA.Value.Day == aux.Value.Day))
                            {
                                if (item.ID_CENTRO_AT_SOL == null)
                                    item.ID_CENTRO_AT_SOL = item.ID_CENTRO_UBI;
                                Context.ATENCION_CITA.Add(new ATENCION_CITA
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
                                    ID_CENTRO_AT_SOL = AtencionCita.ID_CENTRO_AT_SOL,
                                });
                                Context.SaveChanges();
                                aux = item.CITA_FECHA_HORA;
                                idCita = item.ID_CITA;
                            }
                            if (item.PROC_ATENCION_MEDICA_PROG != null ? item.PROC_ATENCION_MEDICA_PROG.Any() : false)
                            {
                                var ProcAtMedProg = new PROC_ATENCION_MEDICA_PROG();
                                foreach (var itm in item.PROC_ATENCION_MEDICA_PROG)
                                {
                                    if (!auxiliarProcs.Any(a => a.ID_PROCMED == itm.PROC_ATENCION_MEDICA.ID_PROCMED && a.ID_USUARIO == itm.PROC_ATENCION_MEDICA.ID_USUARIO && a.REGISTRO_FEC == itm.PROC_ATENCION_MEDICA.REGISTRO_FEC && a.OBSERV == itm.PROC_ATENCION_MEDICA.OBSERV))
                                    {
                                        itm.PROC_ATENCION_MEDICA.ID_ATENCION_MEDICA = AtencionMedica.ID_ATENCION_MEDICA;
                                        auxiliarProcs.Add(itm.PROC_ATENCION_MEDICA);
                                        Context.PROC_ATENCION_MEDICA.Add(itm.PROC_ATENCION_MEDICA);
                                    }
                                    Context.SaveChanges();
                                    ProcAtMedProg = new PROC_ATENCION_MEDICA_PROG
                                    {
                                        ID_AM_PROG = GetIDProceso<short>("PROC_ATENCION_MEDICA_PROG", "ID_AM_PROG", "ID_ATENCION_MEDICA = " + AtencionMedica.ID_ATENCION_MEDICA + " AND ID_CENTRO_UBI = " + AtencionMedica.ID_CENTRO_UBI +
                                            " AND ID_PROCMED = " + itm.ID_PROCMED + ""),
                                        ESTATUS = itm.ESTATUS,
                                        ID_ATENCION_MEDICA = AtencionMedica.ID_ATENCION_MEDICA,
                                        ID_CENTRO_CITA = AtencionMedica.ID_CENTRO_UBI,
                                        ID_CENTRO_UBI = AtencionMedica.ID_CENTRO_UBI,
                                        ID_PROCMED = itm.ID_PROCMED,
                                        ID_USUARIO_ASIGNADO = itm.ID_USUARIO_ASIGNADO,
                                        //REALIZACION_FEC = itm.REALIZACION_FEC,
                                        ID_CITA = idCita,
                                    };
                                    if (ProcedimientosIncidencias != null ?
                                        ProcAtencionMedicaProg != null ?
                                            !ProcedimientosIncidencias.Any(a => a.ID_PROCMED == itm.ID_PROCMED)
                                        : true
                                    : false)
                                        Context.PROC_ATENCION_MEDICA_PROG.Add(ProcAtMedProg);
                                    Context.SaveChanges();
                                }
                            }
                        }
                    }
                    #endregion

                    #region INCIDENCIAS
                    if (ProcAtencionMedicaProg != null)
                    {
                        if (ProcedimientosIncidencias != null ? ProcedimientosIncidencias.Any() : false)
                        {
                            foreach (var item in ProcedimientosIncidencias)
                            {
                                var procProg = Context.PROC_ATENCION_MEDICA_PROG.Any(a => a.ID_ATENCION_MEDICA == ProcAtencionMedicaProg.ID_ATENCION_MEDICA && a.ID_CENTRO_UBI == ProcAtencionMedicaProg.ID_CENTRO_UBI && a.ID_PROCMED == item.ID_PROCMED) ?
                                    Context.PROC_ATENCION_MEDICA_PROG.First(a => a.ID_ATENCION_MEDICA == ProcAtencionMedicaProg.ID_ATENCION_MEDICA && a.ID_CENTRO_UBI == ProcAtencionMedicaProg.ID_CENTRO_UBI && a.ID_PROCMED == item.ID_PROCMED)
                                : null;
                                if (procProg == null) continue;
                                procProg.ESTATUS = "C";
                                procProg.REALIZACION_FEC = new Nullable<DateTime>();
                                Context.Entry(procProg).State = EntityState.Modified;
                                Context.SaveChanges();
                                var incidencia = new PROC_ATENCION_MEDICA_PROG_INCI
                                {
                                    ID_AM_PROG = procProg.ID_AM_PROG,
                                    ID_ATENCION_MEDICA = procProg.ID_ATENCION_MEDICA,
                                    ID_CENTRO_UBI = procProg.ID_CENTRO_UBI,
                                    ID_PROCMED = item.ID_PROCMED,
                                    ID_ACMOTIVO = item.ID_ACMOTIVO,
                                    ID_AM_PROG_IN = GetIDProceso<short>("PROC_ATENCION_MEDICA_PROG_INCI", "ID_AM_PROG_IN", "ID_AM_PROG = " + procProg.ID_AM_PROG + " AND ID_ATENCION_MEDICA = " + procProg.ID_ATENCION_MEDICA +
                                        " AND ID_CENTRO_UBI = " + procProg.ID_CENTRO_UBI + " AND ID_PROCMED = " + procProg.ID_PROCMED),
                                    ID_USUARIO = item.ID_USUARIO,
                                    REGISTRO_FEC = item.REGISTRO_FEC,
                                    OBSERVACIONES = item.OBSERVACIONES,
                                };
                                Context.PROC_ATENCION_MEDICA_PROG_INCI.Add(incidencia);
                                Context.SaveChanges();
                                var newProg = new PROC_ATENCION_MEDICA_PROG
                                {
                                    ESTATUS = "N",
                                    ID_PROCMED = item.ID_PROCMED,
                                    ID_CENTRO_UBI = procProg.ID_CENTRO_UBI,
                                    REALIZACION_FEC = new Nullable<DateTime>(),
                                    ID_ATENCION_MEDICA = procProg.ID_ATENCION_MEDICA,
                                    ID_CITA = item.PROC_ATENCION_MEDICA_PROG.ID_CITA,
                                    ID_USUARIO_ASIGNADO = procProg.ID_USUARIO_ASIGNADO,
                                    ID_CENTRO_CITA = item.PROC_ATENCION_MEDICA_PROG.ID_CENTRO_CITA,
                                    ID_AM_PROG = GetIDProceso<short>("PROC_ATENCION_MEDICA_PROG", "ID_AM_PROG", "ID_ATENCION_MEDICA = " + procProg.ID_ATENCION_MEDICA + " AND ID_CENTRO_UBI = " + procProg.ID_CENTRO_UBI +
                                      " AND ID_PROCMED = " + item.ID_PROCMED),
                                };
                                Context.PROC_ATENCION_MEDICA_PROG.Add(newProg);
                                Context.SaveChanges();
                            }
                        }
                    }
                    #endregion

                    #region DETALLE
                    var listoProg = new List<PROC_ATENCION_MEDICA_PROG>();
                    var listo = new List<PROC_ATENCION_MEDICA>();
                    var prog = new PROC_ATENCION_MEDICA_PROG();
                    var procatmed = new PROC_ATENCION_MEDICA();
                    foreach (var item in ProcMedProgDet)
                    {
                        if (!Context.PROC_ATENCION_MEDICA.Any(a => a.ID_CENTRO_UBI == item.ID_CENTRO_UBI && a.ID_PROCMED == item.ID_PROCMED && a.ID_ATENCION_MEDICA == item.ID_ATENCION_MEDICA))
                            if (!listo.Any(a => a.ID_CENTRO_UBI == item.ID_CENTRO_UBI && a.ID_PROCMED == item.ID_PROCMED && a.ID_ATENCION_MEDICA == item.ID_ATENCION_MEDICA))
                            {
                                procatmed = new PROC_ATENCION_MEDICA
                                {
                                    ID_ATENCION_MEDICA = item.ID_ATENCION_MEDICA,
                                    ID_CENTRO_UBI = item.ID_CENTRO_UBI,
                                    ID_PROCMED = item.ID_PROCMED,
                                    ID_USUARIO = ProcAtencionMedicaProg.ID_USUARIO_ASIGNADO,
                                    REGISTRO_FEC = item.REGISTRO_FEC,
                                };
                                Context.PROC_ATENCION_MEDICA.Add(procatmed);
                                Context.SaveChanges();
                                listo.Add(procatmed);
                            }
                        if (!Context.PROC_ATENCION_MEDICA_PROG.Any(a => a.ID_AM_PROG == item.ID_AM_PROG && a.ID_ATENCION_MEDICA == item.ID_ATENCION_MEDICA && a.ID_CENTRO_UBI == item.ID_CENTRO_UBI && a.ID_PROCMED == item.ID_PROCMED))
                        {
                            if (!listoProg.Any(a => a.ID_AM_PROG == item.ID_AM_PROG && a.ID_ATENCION_MEDICA == item.ID_ATENCION_MEDICA && a.ID_CENTRO_UBI == item.ID_CENTRO_UBI && a.ID_PROCMED == item.ID_PROCMED))
                            {
                                prog = new PROC_ATENCION_MEDICA_PROG
                                {
                                    ID_AM_PROG = item.ID_AM_PROG,
                                    ESTATUS = "S",
                                    ID_ATENCION_MEDICA = item.ID_ATENCION_MEDICA,
                                    ID_CENTRO_UBI = item.ID_CENTRO_UBI,
                                    ID_PROCMED = item.ID_PROCMED,
                                    ID_USUARIO_ASIGNADO = ProcAtencionMedicaProg.ID_USUARIO_ASIGNADO,
                                    REALIZACION_FEC = item.REGISTRO_FEC,
                                    ID_CITA = ProcAtencionMedicaProg.ID_CITA,
                                    ID_CENTRO_CITA = ProcAtencionMedicaProg.ID_CENTRO_CITA,
                                };
                                Context.PROC_ATENCION_MEDICA_PROG.Add(prog);
                                Context.SaveChanges();
                                listoProg.Add(prog);
                            }
                        }
                        else
                        {
                            if (!Context.PROC_ATENCION_MEDICA_PROG.Local.Any(a => a.ID_AM_PROG == item.ID_AM_PROG && a.ID_ATENCION_MEDICA == item.ID_ATENCION_MEDICA && a.ID_CENTRO_UBI == item.ID_CENTRO_UBI && a.ID_PROCMED == item.ID_PROCMED && a.ESTATUS == "S"))
                            {
                                prog = new PROC_ATENCION_MEDICA_PROG
                                    {
                                        ID_AM_PROG = item.ID_AM_PROG,
                                        ESTATUS = "S",
                                        ID_ATENCION_MEDICA = item.ID_ATENCION_MEDICA,
                                        ID_CENTRO_UBI = item.ID_CENTRO_UBI,
                                        ID_PROCMED = item.ID_PROCMED,
                                        ID_USUARIO_ASIGNADO = ProcAtencionMedicaProg.ID_USUARIO_ASIGNADO,
                                        REALIZACION_FEC = item.REGISTRO_FEC,
                                        ID_CITA = ProcAtencionMedicaProg.ID_CITA,
                                        ID_CENTRO_CITA = ProcAtencionMedicaProg.ID_CENTRO_CITA,
                                    };
                                Context.Entry(prog).State = EntityState.Modified;
                                Context.SaveChanges();
                                //listoProg.Add(prog);
                            }
                        }
                        Context.PROC_MEDICO_PROG_DET.Add(new PROC_MEDICO_PROG_DET
                        {
                            CANTIDAD_UTILIZADA = item.CANTIDAD_UTILIZADA,
                            ID_AM_PROG = item.ID_AM_PROG,
                            ID_ATENCION_MEDICA = item.ID_ATENCION_MEDICA,
                            ID_CENTRO_UBI = item.ID_CENTRO_UBI,
                            ID_PROCMED = item.ID_PROCMED,
                            ID_PRODUCTO = item.ID_PRODUCTO,
                            REGISTRO_FEC = item.REGISTRO_FEC
                        });
                        Context.SaveChanges();
                    }
                    #endregion

                    if (ProcedimientoSinMateriales != null ? ProcedimientoSinMateriales.Any() : false)
                        foreach (var item in ProcedimientoSinMateriales)
                            if (Context.PROC_ATENCION_MEDICA_PROG.Any(a => a.ID_AM_PROG == item.ID_AM_PROG && a.ID_ATENCION_MEDICA == item.ID_ATENCION_MEDICA && a.ID_CENTRO_UBI == item.ID_CENTRO_UBI && a.ID_PROCMED == item.ID_PROCMED && a.ESTATUS == "N"))
                            {
                                var progSM = Context.PROC_ATENCION_MEDICA_PROG.First(a => a.ID_AM_PROG == item.ID_AM_PROG && a.ID_ATENCION_MEDICA == item.ID_ATENCION_MEDICA && a.ID_CENTRO_UBI == item.ID_CENTRO_UBI && a.ID_PROCMED == item.ID_PROCMED && a.ESTATUS == "N");
                                progSM.ESTATUS = "S";
                                progSM.REALIZACION_FEC = hoy;
                                Context.Entry(prog).State = EntityState.Modified;
                                Context.SaveChanges();
                            }

                    #region DENTAL
                    if (Odontograma != null ? Odontograma.Any() : false)
                    {
                        foreach (var item in Odontograma)
                        {
                            Context.ODONTOGRAMA_SEGUIMIENTO2.Add(new ODONTOGRAMA_SEGUIMIENTO2
                            {
                                ESTATUS = item.ESTATUS,
                                //ID_TRATA = item.ID_TRATA,
                                ID_TRATA = item.ID_TRATA.HasValue ? item.ID_TRATA != -1 ? item.ID_TRATA : null : null,
                                ID_ENFERMEDAD = item.ID_ENFERMEDAD.HasValue ? item.ID_ENFERMEDAD != -1 ? item.ID_ENFERMEDAD : null : null,
                                //ID_ENFERMEDAD = item.ID_ENFERMEDAD,
                                ID_TIPO_ODO = item.ID_TIPO_ODO,
                                ID_POSICION = item.ID_POSICION,
                                ID_ATENCION_MEDICA = AtencionMedica.ID_ATENCION_MEDICA,
                                ID_CENTRO_UBI = AtencionMedica.ID_CENTRO_UBI,
                                REGISTRO_FEC = hoy,
                                ID_CONSECUTIVO = GetIDProceso<int>("ODONTOGRAMA_SEGUIMIENTO2", "ID_CONSECUTIVO", "ID_CENTRO_UBI = " + AtencionMedica.ID_CENTRO_UBI),
                            });
                            Context.SaveChanges();
                        }
                    }
                    #endregion

                    Context.SaveChanges();
                    transaccion.Complete();
                    return true;
                }
            }

            catch (System.Exception exc)
            {
                throw exc;
            }
        }
        /// <summary>
        /// Obtiene las notas médicas que ocupan interconsulta
        /// </summary>
        /// <param name="centro">Id del centro a consultar</param>
        /// <param name="anio_imputado">Año en id del Imputado</param>
        /// <param name="folio_imputado">Folio en id del Imputado</param>
        /// <param name="nombre">Nombre del imputado</param>
        /// <param name="paterno">Apellido paterno del imputado</param>
        /// <param name="materno">Apellido materno del nombre</param>
        /// <param name="fecha_inicio">Fecha Incial del Rango de Busqueda</param>
        /// <param name="fecha_fin">Fecha Final del Rango de Busqueda</param>
        /// <param name="estatus_administrativos_inactivos">Listado que contiene los estatus administrativos que vuelven inactivo un ingreso</param>
        /// <param name="tipo_atencion">Llave del tipo de atencion medica a buscar</param>
        /// <returns></returns>
        public IQueryable<NOTA_MEDICA> ObtenerNotasConInterconsulta(short centro, short?[] estatus_administrativos_inactivos, short? tipo_atencion = null, short? anio_imputado = null, int? folio_imputado = null, string nombre = "", string paterno = "", string materno = "", DateTime? fecha_inicio = null, DateTime? fecha_fin = null)
        {
            try
            {
                var predicate = PredicateBuilder.True<NOTA_MEDICA>();
                if (tipo_atencion.HasValue)
                    predicate = predicate.And(w => w.ATENCION_MEDICA.ID_TIPO_ATENCION == tipo_atencion);
                if (anio_imputado.HasValue && folio_imputado.HasValue)
                {
                    predicate = predicate.And(w => w.ATENCION_MEDICA.INGRESO.ID_ANIO == anio_imputado.Value && w.ATENCION_MEDICA.INGRESO.ID_IMPUTADO == folio_imputado.Value);
                }
                if (!string.IsNullOrWhiteSpace(nombre))
                    predicate = predicate.And(w => w.ATENCION_MEDICA.INGRESO.IMPUTADO.NOMBRE.Contains(nombre));
                if (!string.IsNullOrWhiteSpace(paterno))
                    predicate = predicate.And(w => w.ATENCION_MEDICA.INGRESO.IMPUTADO.PATERNO.Contains(paterno));
                if (!string.IsNullOrWhiteSpace(materno))
                    predicate = predicate.And(w => w.ATENCION_MEDICA.INGRESO.IMPUTADO.PATERNO.Contains(materno));
                if (fecha_inicio.HasValue)
                    predicate = predicate.And(w => EntityFunctions.TruncateTime(w.ATENCION_MEDICA.ATENCION_FEC) >= EntityFunctions.TruncateTime(fecha_inicio));
                if (fecha_fin.HasValue)
                    predicate = predicate.And(w => EntityFunctions.TruncateTime(w.ATENCION_MEDICA.ATENCION_FEC) <= EntityFunctions.TruncateTime(fecha_fin));
                foreach (var item in estatus_administrativos_inactivos)
                    if (item.HasValue)
                        predicate = predicate.And(w => w.ATENCION_MEDICA.INGRESO.ID_ESTATUS_ADMINISTRATIVO != item.Value);
                predicate = predicate.And(w => w.OCUPA_INTERCONSULTA == "S" && w.CANALIZACION == null && w.ATENCION_MEDICA.ID_CENTRO_UBI == centro);
                return GetData(predicate.Expand());
            }
            catch (Exception ex)
            {
                throw new ApplicationException(ex.Message + " " + (ex.InnerException != null ? ex.InnerException.InnerException.Message : ""));
            }

        }

        public IQueryable<NOTA_MEDICA> ObtenerNotasMedicasRecetaMedica(short Centro, short?[] EstatusInac, short? TipoAtencion, short? TipoServicio, int? Anio, int? Folio, string Nombre, string Paterno, string Materno, System.DateTime? Fecha)
        {
            try
            {
                var predicate = PredicateBuilder.True<NOTA_MEDICA>();
                if (TipoAtencion.HasValue && TipoAtencion != -1)
                    predicate = predicate.And(x => x.ATENCION_MEDICA.ID_TIPO_ATENCION == TipoAtencion);

                if (EstatusInac != null && EstatusInac.Any())
                    foreach (var item in EstatusInac)
                        predicate = predicate.And(x => x.ATENCION_MEDICA.INGRESO.ID_ESTATUS_ADMINISTRATIVO != item);

                if (TipoServicio.HasValue && TipoServicio != -1)
                    predicate = predicate.And(x => x.ATENCION_MEDICA.ID_TIPO_SERVICIO == TipoServicio);

                if (Anio.HasValue)
                    predicate = predicate.And(x => x.ATENCION_MEDICA.INGRESO.IMPUTADO.ID_ANIO == Anio);

                if (Folio.HasValue)
                    predicate = predicate.And(x => x.ATENCION_MEDICA.INGRESO.IMPUTADO.ID_IMPUTADO == Folio);

                if (!string.IsNullOrEmpty(Nombre))
                    predicate = predicate.And(x => x.ATENCION_MEDICA.INGRESO.IMPUTADO.NOMBRE.Trim() == Nombre);

                if (!string.IsNullOrEmpty(Paterno))
                    predicate = predicate.And(x => x.ATENCION_MEDICA.INGRESO.IMPUTADO.PATERNO.Trim() == Paterno);

                if (!string.IsNullOrEmpty(Materno))
                    predicate = predicate.And(x => x.ATENCION_MEDICA.INGRESO.IMPUTADO.MATERNO.Trim() == Materno);

                if (Fecha.HasValue)
                    predicate = predicate.And(x => x.ATENCION_MEDICA.ATENCION_FEC.Value.Year == Fecha.Value.Year && x.ATENCION_MEDICA.ATENCION_FEC.Value.Month == Fecha.Value.Month && x.ATENCION_MEDICA.ATENCION_FEC.Value.Day == Fecha.Value.Day);

                predicate = predicate.And(x => x.ID_CENTRO_UBI == Centro);
                return GetData(predicate.Expand()).OrderByDescending(x => x.ATENCION_MEDICA.ATENCION_FEC);
            }
            catch (System.Exception exc)
            {
                throw new ApplicationException(exc.Message + " " + (exc.InnerException != null ? exc.InnerException.InnerException.Message : ""));
            }
        }
    }
}
