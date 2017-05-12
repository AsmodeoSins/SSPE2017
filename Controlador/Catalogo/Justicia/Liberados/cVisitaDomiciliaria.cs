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
    public class cVisitaDomiciliaria : EntityManagerServer<PRS_VISITA_DOMICILIARIA>
    {

        public PRS_VISITA_DOMICILIARIA Obtener(short? Centro = null, short? Anio = null, int? Imputado = null, short? Folio = null)
        {
            try
            {
                var predicate = PredicateBuilder.True<PRS_VISITA_DOMICILIARIA>();
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
        public short Insertar(PRS_VISITA_DOMICILIARIA _entidad)
        {
            try
            {
                using (TransactionScope transaccion = new TransactionScope(TransactionScopeOption.RequiresNew, new TransactionOptions() { IsolationLevel = System.Transactions.IsolationLevel.ReadCommitted }))
                {
                    _entidad.ID_FOLIO = GetIDProceso<short>("PRS_VISITA_DOMICILIARIA", "ID_FOLIO", string.Format("ID_CENTRO = {0} AND ID_ANIO = {1} AND ID_IMPUTADO = {2}",
                        _entidad.ID_CENTRO,
                        _entidad.ID_ANIO,
                        _entidad.ID_IMPUTADO));
                    Context.PRS_VISITA_DOMICILIARIA.Add(_entidad);
                    Context.SaveChanges();
                    transaccion.Complete();
                    return _entidad.ID_FOLIO;
                }
                #region comentado
                ////No tiene Secuencia
                //_entidad.ID_FOLIO = Obtener_Id_Folio(_entidad.ID_IMPUTADO, _entidad.ID_ANIO, _entidad.ID_CENTRO, _entidad.ID_FOLIO);
                //Insert(_entidad);
                //return true;
                #endregion
            }
            catch (Exception ex)
            {
                throw new ApplicationException(ex.Message + " " + (ex.InnerException != null ? ex.InnerException.InnerException.Message : ""));
            }
        }

        public short Obtener_Id_Folio(int Id_imputado, short Id_Anio, short Id_Centro, short? Folio)
        {

            IQueryable<PRS_VISITA_DOMICILIARIA> query;
            if (Folio > 0)
            {
                query = GetData().Where(w => w.ID_IMPUTADO == Id_imputado && w.ID_ANIO == Id_Anio && w.ID_CENTRO == Id_Centro && w.ID_FOLIO == Folio);
            }
            else
            {
                query = GetData().Where(w => w.ID_IMPUTADO == Id_imputado && w.ID_ANIO == Id_Anio && w.ID_CENTRO == Id_Centro);
            }
            //int numcount = query.Count();
            int FolioObt = query.Count() > 0 ? query.ToList().Max(mx => mx.ID_FOLIO) : 0;
            FolioObt = FolioObt + 1;
            return short.Parse(FolioObt.ToString());

        }

        public List<PRS_VISITA_DOMICILIARIA> Obtener(long Id)
        {
            var Resultado = new List<PRS_VISITA_DOMICILIARIA>();
            try
            {
                Resultado = GetData().Where(w => w.ID_FOLIO == Id).ToList();
            }
            catch (Exception ex)
            {
                throw new ApplicationException(ex.Message);
            }
            return Resultado;
        }

        public bool Actualizar(PRS_VISITA_DOMICILIARIA _entidad,List<PRS_VISITA_FOTOGRAFIA> LstFoto)
        {
            try
            {
                using (TransactionScope transaccion = new TransactionScope(TransactionScopeOption.RequiresNew, new TransactionOptions() { IsolationLevel = System.Transactions.IsolationLevel.ReadCommitted }))
                {
                    Context.Entry(_entidad).State = EntityState.Modified;

                    #region Fotografia
                    var lista = Context.PRS_VISITA_FOTOGRAFIA.Where(w => w.ID_CENTRO == _entidad.ID_CENTRO && w.ID_ANIO == _entidad.ID_ANIO && w.ID_IMPUTADO == _entidad.ID_IMPUTADO && w.ID_FOLIO == _entidad.ID_FOLIO);
                    if (lista != null)
                    {
                        foreach (var l in lista)
                            Context.PRS_VISITA_FOTOGRAFIA.Remove(l);
                    }
                    if (LstFoto != null)
                    {
                        foreach (var l in LstFoto)
                        {
                            l.ID_FOLIO = _entidad.ID_FOLIO;
                            Context.PRS_VISITA_FOTOGRAFIA.Add(l); 
                        }
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

        public bool Eliminar(PRS_VISITA_DOMICILIARIA _entidad)
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

        public PRS_VISITA_DOMICILIARIA ObtenerUltimoFolio(short ID_ANIO, short ID_CENTRO, int ID_IMPUTADO)
        {
            try
            {
                return GetData(g =>
                    g.ID_ANIO == ID_ANIO &&
                    g.ID_CENTRO == ID_CENTRO &&
                    g.ID_IMPUTADO == ID_IMPUTADO).
                    OrderByDescending(o => o.ID_FOLIO).
                    FirstOrDefault();

            }
            catch (Exception ex)
            {

                throw new ApplicationException(ex.Message);
            }
        }


    }
}
