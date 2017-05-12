using SSP.Servidor;
using SSP.Modelo;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Collections.ObjectModel;
using System.Data.Objects.SqlClient;
using System.Transactions;
using System.Data;

namespace SSP.Controlador.Catalogo.Justicia
{
    public class cEstudioSocioEconomico  : EntityManagerServer<SOCIOECONOMICO>
    {
        public cEstudioSocioEconomico() { }


        public SOCIOECONOMICO Obtener(int IdImputado, int IdIngreso)
        {
            try
            {
                return GetData().Where(w => w.ID_INGRESO == IdIngreso && w.ID_IMPUTADO == IdImputado).SingleOrDefault();
            }

            catch (Exception ex)
            {
                throw new ApplicationException(ex.Message);
            }
        }

        public bool Insertar(SOCIOECONOMICO Estudio, SOCIOE_GPOFAMPRI GrupoPrimario, SOCIOE_GPOFAMSEC GrupoSecundario, List<SOCIOE_GPOFAMPRI_CARAC> CaractGrupoPrimario, List<SOCIOE_GPOFAMSEC_CARAC> CaractGrupoecundario)
        {
            try
            {
                using (TransactionScope transaccion = new TransactionScope(TransactionScopeOption.RequiresNew, new TransactionOptions() { IsolationLevel = System.Transactions.IsolationLevel.ReadCommitted }))
                {
                    if (Estudio != null)
                    {
                        var EstudioS = new SOCIOECONOMICO
                        {
                            DICTAMEN = Estudio.DICTAMEN,
                            DICTAMEN_FEC = Estudio.DICTAMEN_FEC,
                            ID_ANIO = Estudio.ID_ANIO,
                            ID_CENTRO = Estudio.ID_CENTRO,
                            ID_IMPUTADO = Estudio.ID_IMPUTADO,
                            ID_INGRESO = Estudio.ID_INGRESO,
                            SALARIO = Estudio.SALARIO
                        };

                        EstudioS.SOCIOE_GPOFAMPRI = new SOCIOE_GPOFAMPRI
                        {
                            ANTECEDENTE = GrupoPrimario.ANTECEDENTE,
                            EGRESO_MENSUAL = GrupoPrimario.EGRESO_MENSUAL,
                            FAM_ANTECEDENTE = GrupoPrimario.FAM_ANTECEDENTE,
                            GRUPO_FAMILIAR = GrupoPrimario.GRUPO_FAMILIAR,
                            ID_ANIO = GrupoPrimario.ID_ANIO,
                            ID_CENTRO = GrupoPrimario.ID_CENTRO,
                            ID_IMPUTADO = GrupoPrimario.ID_IMPUTADO,
                            ID_INGRESO = GrupoPrimario.ID_INGRESO,
                            INGRESO_MENSUAL = GrupoPrimario.INGRESO_MENSUAL,
                            NIVEL_SOCIO_CULTURAL = GrupoPrimario.NIVEL_SOCIO_CULTURAL,
                            PERSONAS_LABORAN = GrupoPrimario.PERSONAS_LABORAN,
                            PERSONAS_VIVEN_HOGAR = GrupoPrimario.PERSONAS_VIVEN_HOGAR,
                            RELACION_INTRAFAMILIAR = GrupoPrimario.RELACION_INTRAFAMILIAR,
                            VIVIENDA_CONDICIONES = GrupoPrimario.VIVIENDA_CONDICIONES,
                            VIVIENDA_ZONA = GrupoPrimario.VIVIENDA_ZONA
                        };

                        EstudioS.SOCIOE_GPOFAMSEC = new SOCIOE_GPOFAMSEC
                        {
                            ANTECEDENTE = GrupoSecundario.ANTECEDENTE,
                            APOYO_ECONOMICO = GrupoSecundario.APOYO_ECONOMICO,
                            EGRESO_MENSUAL = GrupoSecundario.EGRESO_MENSUAL,
                            FAM_ANTECEDENTE = GrupoSecundario.FAM_ANTECEDENTE,
                            FRECUENCIA = GrupoSecundario.FRECUENCIA,
                            GRUPO_FAMILIAR = GrupoSecundario.GRUPO_FAMILIAR,
                            ID_ANIO = GrupoSecundario.ID_ANIO,
                            ID_CENTRO = GrupoSecundario.ID_CENTRO,
                            ID_IMPUTADO = GrupoSecundario.ID_IMPUTADO,
                            ID_INGRESO = GrupoSecundario.ID_INGRESO,
                            INGRESO_MENSUAL = GrupoSecundario.INGRESO_MENSUAL,
                            NIVEL_SOCIO_CULTURAL = GrupoSecundario.NIVEL_SOCIO_CULTURAL,
                            PERSONAS_LABORAN = GrupoSecundario.PERSONAS_LABORAN,
                            RECIBE_VISITA = GrupoSecundario.RECIBE_VISITA,
                            RELACION_INTRAFAMILIAR = GrupoSecundario.RELACION_INTRAFAMILIAR,
                            VISITA = GrupoSecundario.VISITA,
                            VIVIENDA_ZONA = GrupoSecundario.VIVIENDA_ZONA,
                            VIVIENDA_CONDICIONES = GrupoSecundario.VIVIENDA_CONDICIONES
                        };

                        foreach (var item in CaractGrupoPrimario)
                        {
                            EstudioS.SOCIOE_GPOFAMPRI.SOCIOE_GPOFAMPRI_CARAC.Add(item);
                            Context.SOCIOE_GPOFAMPRI_CARAC.Add(item);
                        }

                        foreach (var item in CaractGrupoecundario)
                        {
                            EstudioS.SOCIOE_GPOFAMSEC.SOCIOE_GPOFAMSEC_CARAC.Add(item);
                            Context.SOCIOE_GPOFAMSEC_CARAC.Add(item);
                        }

                        Context.SOCIOECONOMICO.Add(EstudioS);
                        Context.SOCIOE_GPOFAMPRI.Add(EstudioS.SOCIOE_GPOFAMPRI);
                        Context.SOCIOE_GPOFAMSEC.Add(EstudioS.SOCIOE_GPOFAMSEC);

                        Context.SaveChanges();
                        transaccion.Complete();
                        return true;
                    }
                }
            }

            catch (Exception exc)
            {

            }

            return false;
        }

        public void Editar(SOCIOECONOMICO Entity)
        {
            try
            {
                if (Entity != null)
                {
                    using (TransactionScope transaccion = new TransactionScope(TransactionScopeOption.RequiresNew, new TransactionOptions() { IsolationLevel = System.Transactions.IsolationLevel.ReadCommitted }))
                    {
                        Context.SOCIOECONOMICO.Attach(Entity);
                        Context.Entry(Entity).Property(x => x.SOCIOE_GPOFAMPRI).IsModified = true;
                        Context.Entry(Entity).Property(x => x.SOCIOE_GPOFAMSEC).IsModified = true;
                        Context.SaveChanges();
                        transaccion.Complete();
                    }
                }
            }
            catch (Exception ex)
            {
                if (ex.Message.Contains("part of the object's key information"))
                    throw new ApplicationException("La llave principal no se puede cambiar");
                else
                    throw new ApplicationException(ex.Message);
            }
        }

        public bool Actualizar(SOCIOECONOMICO Entity)
        {
            try
            {
                if (Update(Entity))
                    return true;
            }
            catch (Exception ex)
            {
                throw new ApplicationException(ex.Message);
            }
            return false;
        }

        public bool ActualizarEstudioTransaccion(SOCIOECONOMICO EstudioSoc, SOCIOE_GPOFAMPRI EstudioPrimario, SOCIOE_GPOFAMSEC EstudioSec, List<SOCIOE_GPOFAMPRI_CARAC> ListCaractPrimarias, List<SOCIOE_GPOFAMSEC_CARAC> ListCaracSec)
        {
            try
            {
                using (TransactionScope transaccion = new TransactionScope(TransactionScopeOption.RequiresNew, new TransactionOptions() { IsolationLevel = System.Transactions.IsolationLevel.ReadCommitted }))
                {
                    Context.Entry(EstudioSoc).State = EntityState.Modified;
                    var CaracteristicasPrimarias = Context.SOCIOE_GPOFAMPRI_CARAC.Where(x => x.ID_IMPUTADO == EstudioSoc.ID_IMPUTADO && x.ID_INGRESO == EstudioSoc.ID_INGRESO);
                    var CaracteristicasSecundarias = Context.SOCIOE_GPOFAMSEC_CARAC.Where(x => x.ID_IMPUTADO == EstudioSoc.ID_IMPUTADO && x.ID_INGRESO == EstudioSoc.ID_INGRESO);
                    var EstudioGrupPrimario = Context.SOCIOE_GPOFAMPRI.Where(x => x.ID_IMPUTADO == EstudioSoc.ID_IMPUTADO && x.ID_INGRESO == EstudioSoc.ID_INGRESO).SingleOrDefault();
                    var EstudioSecundario = Context.SOCIOE_GPOFAMSEC.Where(x => x.ID_IMPUTADO == EstudioSoc.ID_IMPUTADO && x.ID_INGRESO == EstudioSoc.ID_INGRESO).SingleOrDefault();

                    var EstudioHecho = Context.SOCIOECONOMICO.Where(x => x.ID_INGRESO == EstudioSoc.ID_INGRESO && x.ID_IMPUTADO == EstudioSoc.ID_IMPUTADO).SingleOrDefault();
                    if (EstudioHecho != null)
                    {
                        //Actualiza el estudio principal
                        var EstudioPrincipal = EstudioHecho;
                        EstudioPrincipal.DICTAMEN = EstudioSoc.DICTAMEN;
                        EstudioPrincipal.DICTAMEN_FEC = EstudioSoc.DICTAMEN_FEC;
                        EstudioPrincipal.ID_ANIO = EstudioSoc.ID_ANIO;
                        EstudioPrincipal.ID_CENTRO = EstudioSoc.ID_CENTRO;
                        EstudioPrincipal.ID_IMPUTADO = EstudioSoc.ID_IMPUTADO;
                        EstudioPrincipal.ID_INGRESO = EstudioSoc.ID_INGRESO;
                        EstudioPrincipal.SALARIO = EstudioSoc.SALARIO;
                        Context.Entry(EstudioPrincipal).State = EntityState.Modified;

                        //Actualiza grupo primario
                        var GrupoPrimarioEstudio = EstudioGrupPrimario;
                        if (GrupoPrimarioEstudio != null)
                        {
                            GrupoPrimarioEstudio.ANTECEDENTE = EstudioPrimario.ANTECEDENTE;
                            GrupoPrimarioEstudio.EGRESO_MENSUAL = EstudioPrimario.EGRESO_MENSUAL;
                            GrupoPrimarioEstudio.FAM_ANTECEDENTE = EstudioPrimario.FAM_ANTECEDENTE;
                            GrupoPrimarioEstudio.GRUPO_FAMILIAR = EstudioPrimario.GRUPO_FAMILIAR;
                            GrupoPrimarioEstudio.ID_ANIO = EstudioPrimario.ID_ANIO;
                            GrupoPrimarioEstudio.ID_CENTRO = EstudioPrimario.ID_CENTRO;
                            GrupoPrimarioEstudio.ID_IMPUTADO = EstudioPrimario.ID_IMPUTADO;
                            GrupoPrimarioEstudio.ID_INGRESO = EstudioPrimario.ID_INGRESO;
                            GrupoPrimarioEstudio.INGRESO_MENSUAL = EstudioPrimario.INGRESO_MENSUAL;
                            GrupoPrimarioEstudio.NIVEL_SOCIO_CULTURAL = EstudioPrimario.NIVEL_SOCIO_CULTURAL;
                            GrupoPrimarioEstudio.PERSONAS_LABORAN = EstudioPrimario.PERSONAS_LABORAN;
                            GrupoPrimarioEstudio.PERSONAS_VIVEN_HOGAR = EstudioPrimario.PERSONAS_VIVEN_HOGAR;
                            GrupoPrimarioEstudio.RELACION_INTRAFAMILIAR = EstudioPrimario.RELACION_INTRAFAMILIAR;
                            GrupoPrimarioEstudio.VIVIENDA_CONDICIONES = EstudioPrimario.VIVIENDA_CONDICIONES;
                            GrupoPrimarioEstudio.VIVIENDA_ZONA = EstudioPrimario.VIVIENDA_ZONA;
                            Context.Entry(GrupoPrimarioEstudio).State = EntityState.Modified;
                        };

                        var GrupoSecundario = EstudioSecundario;
                        if (GrupoSecundario != null)
                        {
                            GrupoSecundario.ANTECEDENTE = EstudioSec.ANTECEDENTE;
                            GrupoSecundario.APOYO_ECONOMICO = EstudioSec.APOYO_ECONOMICO;
                            GrupoSecundario.EGRESO_MENSUAL = EstudioSec.EGRESO_MENSUAL;
                            GrupoSecundario.FAM_ANTECEDENTE = EstudioSec.FAM_ANTECEDENTE;
                            GrupoSecundario.FRECUENCIA = EstudioSec.FRECUENCIA;
                            GrupoSecundario.GRUPO_FAMILIAR = EstudioSec.GRUPO_FAMILIAR;
                            GrupoSecundario.ID_ANIO = EstudioSec.ID_ANIO;
                            GrupoSecundario.ID_CENTRO = EstudioSec.ID_CENTRO;
                            GrupoSecundario.ID_IMPUTADO = EstudioSec.ID_IMPUTADO;
                            GrupoSecundario.MOTIVO_NO_VISITA = EstudioSec.MOTIVO_NO_VISITA; 
                            GrupoSecundario.ID_INGRESO = EstudioSec.ID_INGRESO;
                            GrupoSecundario.INGRESO_MENSUAL = EstudioSec.INGRESO_MENSUAL;
                            GrupoSecundario.NIVEL_SOCIO_CULTURAL = EstudioSec.NIVEL_SOCIO_CULTURAL;
                            GrupoSecundario.PERSONAS_LABORAN = EstudioSec.PERSONAS_LABORAN;
                            GrupoSecundario.RECIBE_VISITA = EstudioSec.RECIBE_VISITA;
                            GrupoSecundario.RELACION_INTRAFAMILIAR = EstudioSec.RELACION_INTRAFAMILIAR;
                            GrupoSecundario.VISITA = EstudioSec.VISITA;
                            GrupoSecundario.VIVIENDA_CONDICIONES = EstudioSec.VIVIENDA_CONDICIONES;
                            GrupoSecundario.VIVIENDA_ZONA = EstudioSec.VIVIENDA_ZONA;
                            Context.Entry(GrupoSecundario).State = EntityState.Modified;
                        };


                        #region Proceso Caract. Primarias
                        if (CaracteristicasPrimarias != null)
                            foreach (var item in CaracteristicasPrimarias)
                                Context.Entry(item).State = EntityState.Deleted;

                        foreach (var item in ListCaractPrimarias)
                        {
                            item.ID_ANIO = EstudioSoc.ID_ANIO;
                            item.ID_CENTRO = EstudioSoc.ID_CENTRO;
                            item.ID_IMPUTADO = EstudioSoc.ID_IMPUTADO;
                            item.ID_INGRESO = EstudioSoc.ID_INGRESO;
                            Context.SOCIOE_GPOFAMPRI_CARAC.Add(item);
                        };

                        #endregion

                        #region Proceso Caract. Secundarias
                        if (CaracteristicasSecundarias != null)
                            foreach (var item in CaracteristicasSecundarias)
                                Context.Entry(item).State = EntityState.Deleted;

                        foreach (var item in ListCaracSec)
                        {
                            item.ID_ANIO = EstudioSoc.ID_ANIO;
                            item.ID_CENTRO = EstudioSoc.ID_CENTRO;
                            item.ID_IMPUTADO = EstudioSoc.ID_IMPUTADO;
                            item.ID_INGRESO = EstudioSoc.ID_INGRESO;
                            Context.SOCIOE_GPOFAMSEC_CARAC.Add(item);
                        };

                        #endregion

                    };


                    Context.SaveChanges();
                    transaccion.Complete();
                    return true;
                }

            }

            catch (Exception exc)
            {

            }

            return false;
        }
        
    }
}
