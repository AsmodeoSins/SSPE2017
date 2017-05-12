using SSP.Servidor;
using SSP.Modelo;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Collections.ObjectModel;
using System.Data.SqlClient;
using System.Data.Objects.SqlClient;
using System.Linq.Expressions;
using LinqKit;
using System.Transactions;
using System.Data;

namespace SSP.Controlador.Catalogo.Justicia.Liberados
{
    public class cEntrevistaInicialTrabajoSocial : EntityManagerServer<PRS_ENTREVISTA_INICIAL>
    {
        public PRS_ENTREVISTA_INICIAL Obtener(short? Centro = null, short? Anio = null,int? Imputado = null,short? Folio = null)
        {
            try
            {
                var predicate = PredicateBuilder.True<PRS_ENTREVISTA_INICIAL>();
                if (Centro.HasValue)
                    predicate = predicate.And(w => w.ID_CENTRO == Centro);
                if (Anio.HasValue)
                    predicate = predicate.And(w => w.ID_ANIO == Anio);
                if (Imputado.HasValue)
                    predicate = predicate.And(w => w.ID_IMPUTADO == Imputado);
                if (Folio.HasValue)
                    predicate = predicate.And(w => w.ID_FOLIO == Folio);
                return GetData(predicate.Expand()).FirstOrDefault();
            }
            catch (Exception ex)
            {
                throw new ApplicationException(ex.Message + " " + (ex.InnerException != null ? ex.InnerException.InnerException.Message : ""));
            }
        }

        public short Insertar(PRS_ENTREVISTA_INICIAL _entidad)
        {
            try
            {
                #region comentado
                //No tiene Secuencia
                ///  _entidad.ID_FOLIO = GetSequence<short>("PRS_VISITA_DOMICILIARIA_SEQ");
                
                //Ya existe un metodo para sacar los id consecutivos 
                //_entidad.ID_FOLIO = GetIDProceso<short>()("TABLA", "COLUMNA", "FILTROS");
                //_entidad.ID_FOLIO =  Obtener_Id_Folio(_entidad.ID_IMPUTADO, _entidad.ID_ANIO, _entidad.ID_CENTRO, _entidad.ID_FOLIO);
                #endregion

                using (TransactionScope transaccion = new TransactionScope(TransactionScopeOption.RequiresNew, new TransactionOptions() { IsolationLevel = System.Transactions.IsolationLevel.ReadCommitted }))
                {
                    _entidad.ID_FOLIO = GetIDProceso<short>("PRS_ENTREVISTA_INICIAL", "ID_FOLIO", string.Format("ID_CENTRO = {0} AND ID_ANIO = {1} AND ID_IMPUTADO = {2}", _entidad.ID_CENTRO, _entidad.ID_ANIO, _entidad.ID_IMPUTADO));
                    Context.PRS_ENTREVISTA_INICIAL.Add(_entidad);
                    Context.SaveChanges();
                    transaccion.Complete();
                    return _entidad.ID_FOLIO;
                }
            }
            catch (Exception ex)
            {
                throw new ApplicationException(ex.Message + " " + (ex.InnerException != null ? ex.InnerException.InnerException.Message : ""));
            }
        }

        public short Obtener_Id_Folio(int Id_imputado, short Id_Anio, short Id_Centro, short? Folio)
        {
            int Id = 1;
            IQueryable<PRS_ENTREVISTA_INICIAL> query;
            if (Folio > 0)
            {
                query = GetData().Where(w => w.ID_IMPUTADO == Id_imputado && w.ID_ANIO == Id_Anio && w.ID_CENTRO == Id_Centro && w.ID_FOLIO == Folio);
            }
            else
            {
                query = GetData().Where(w => w.ID_IMPUTADO == Id_imputado && w.ID_ANIO == Id_Anio && w.ID_CENTRO == Id_Centro);
            }
            //int numcount = query.Count();
            int FolioObt= query.Count() > 0 ? query.ToList().Max(mx=>mx.ID_FOLIO):0;
            FolioObt = FolioObt + 1;
            return short.Parse(FolioObt.ToString());
            //return short.Parse((query.Count() > 0 ? query.ToList().LastOrDefault().ID_FOLIO + Id : Id).ToString());
        }

        public List<PRS_ENTREVISTA_INICIAL> Obtener(short IdImputado, short Folio)
        {
            var Resultado = new List<PRS_ENTREVISTA_INICIAL>();
            try
            {
                Resultado = GetData().Where(w => w.ID_IMPUTADO == IdImputado && w.ID_FOLIO == Folio).ToList();
            }
            catch (Exception ex)
            {
                throw new ApplicationException(ex.Message);
            }
            return Resultado;
        }

        public IQueryable<PRS_ENTREVISTA_INICIAL> ObtenerTodos()
        {
            return GetData();
        }
     
        public PRS_ENTREVISTA_INICIAL ObtenerUltimaEntrevista(short Centro,short Anio,int Imputado)
        {
            try
            {
                return GetData().Where(w => w.ID_CENTRO == Centro && w.ID_ANIO == Anio && w.ID_IMPUTADO == Imputado).OrderByDescending(w => w.ID_FOLIO).FirstOrDefault();
            }
            catch (Exception ex)
            {
                throw new ApplicationException(ex.Message);
            }
        }

        public bool Actualizar(PRS_ENTREVISTA_INICIAL _entidad,List<PRS_APOYO_ECONOMICO> LstApoyoEconomico,List<PRS_NUCLEO_FAMILIAR> LstNucleoFamiliar,List<PRS_DROGA> LstDroga)
        {
            try
            {
                using (TransactionScope transaccion = new TransactionScope(TransactionScopeOption.RequiresNew, new TransactionOptions() { IsolationLevel = System.Transactions.IsolationLevel.ReadCommitted }))
                {
                    Context.Entry(_entidad).State = EntityState.Modified;

                    #region Apoyo Economico
                    var lstAE = Context.PRS_APOYO_ECONOMICO.Where(w =>
                        w.ID_CENTRO == _entidad.ID_CENTRO &&
                        w.ID_ANIO == _entidad.ID_ANIO &&
                        w.ID_IMPUTADO == _entidad.ID_IMPUTADO &&
                        w.ID_FOLIO == _entidad.ID_FOLIO);
                    if (lstAE != null)
                        foreach (var l in lstAE)
                            Context.PRS_APOYO_ECONOMICO.Remove(l);

                    if (LstApoyoEconomico != null)
                        foreach (var l in LstApoyoEconomico)
                        {
                            l.ID_FOLIO = _entidad.ID_FOLIO;
                            Context.PRS_APOYO_ECONOMICO.Add(l);
                        }
                    #endregion

                    #region Nucleo Familiar
                    var lstNF = Context.PRS_NUCLEO_FAMILIAR.Where(w =>
                        w.ID_CENTRO == _entidad.ID_CENTRO &&
                        w.ID_ANIO == _entidad.ID_ANIO &&
                        w.ID_IMPUTADO == _entidad.ID_IMPUTADO &&
                        w.ID_FOLIO == _entidad.ID_FOLIO);
                    if (lstNF != null)
                        foreach (var l in lstNF)
                            Context.PRS_NUCLEO_FAMILIAR.Remove(l);

                    if (LstNucleoFamiliar != null)
                        foreach (var l in LstNucleoFamiliar)
                        {
                            l.ID_FOLIO = _entidad.ID_FOLIO;
                            Context.PRS_NUCLEO_FAMILIAR.Add(l);
                        }
                    #endregion

                    #region Drogas
                    var lstD = Context.PRS_DROGA.Where(w =>
                        w.ID_CENTRO == _entidad.ID_CENTRO &&
                        w.ID_ANIO == _entidad.ID_ANIO &&
                        w.ID_IMPUTADO == _entidad.ID_IMPUTADO &&
                        w.ID_FOLIO == _entidad.ID_FOLIO);
                    if (lstD != null)
                        foreach (var l in lstD)
                            Context.PRS_DROGA.Remove(l);

                    if (LstDroga != null)
                        foreach (var l in LstDroga)
                        {
                            l.ID_FOLIO = _entidad.ID_FOLIO;
                            Context.PRS_DROGA.Add(l);
                        }
                    #endregion

                    Context.SaveChanges();
                    transaccion.Complete();
                    return true;
                    //if (Insert(Entity))
                }

                //Update(_entidad);
                //return true;
            }
            catch (Exception ex)
            {
                throw new ApplicationException(ex.Message + " " + (ex.InnerException != null ? ex.InnerException.InnerException.Message : ""));
            }
        }

        public bool Eliminar(PRS_ENTREVISTA_INICIAL _entidad)
        {
            try
            {
                Delete(_entidad);
                return true;
            }
            catch (Exception ex)
            {
                throw new ApplicationException(ex.Message + " " + (ex.InnerException != null ? ex.InnerException.InnerException.Message : ""));
            }
        }
    }
}
