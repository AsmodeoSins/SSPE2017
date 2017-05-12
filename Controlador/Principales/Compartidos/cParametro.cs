using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LinqKit;
using SSP.Modelo;
using SSP.Servidor;
using SSP.Controlador.Catalogo.Justicia;

namespace SSP.Controlador.Principales.Compartidos
{
    public class cParametro : EntityManagerServer<PARAMETRO>
    {
        public IQueryable<PARAMETRO> Seleccionar(string id_parametro, int id_centro)
        {
            try
            {
                if (Context.PARAMETRO.Any(w => w.ID_CLAVE.Trim() == id_parametro && w.ID_CENTRO == id_centro))
                    return GetData(w => w.ID_CLAVE.Trim() == id_parametro && w.ID_CENTRO == id_centro); //obtiene la regla especifica del parametro para ese centro, es de mayor prioridad que la regla general
                else
                    return GetData(w => w.ID_CLAVE.Trim() == id_parametro && w.ID_CENTRO == 0); //obtiene la regla general de ese parametro
            }
            catch (Exception ex)
            {
                throw new ApplicationException(ex.Message + " " + (ex.InnerException != null ? ex.InnerException.InnerException.Message : ""));
            }
        }

        public PARAMETRO ObtenerParametro(short? Centro = null, string Clave = "")
        { 
            try
            {
                var predicate = PredicateBuilder.True<PARAMETRO>();
                if (Centro.HasValue)
                    predicate = predicate.And(w => w.ID_CENTRO == Centro);
                if(!string.IsNullOrEmpty(Clave))
                    predicate = predicate.And(w => w.ID_CLAVE == Clave);
                return GetData(predicate.Expand()).FirstOrDefault();
            }
            catch (Exception ex)
            {
                throw new ApplicationException(ex.Message + " " + (ex.InnerException != null ? ex.InnerException.InnerException.Message : ""));
            }
        }

        public IQueryable<PARAMETRO> Obtener(short? centro = null, string clave = "", string valor = "", int? valor_num = null, string descripcion = "")
        {
            try
            {
               
                IQueryable<PARAMETRO> queryParametro = null; //< -------------Obtener si se retorna esta variable es porque el centro esta inactivo(Regresa un Null)
                var predicate = PredicateBuilder.True<PARAMETRO>();
                var predicateCentro = PredicateBuilder.True<CENTRO>();

                predicateCentro=predicateCentro.And(wCentro => wCentro.ID_CENTRO == centro&&wCentro.ESTATUS.Equals("S"));
                var EstatusCentro = new cCentro().GetData(predicateCentro.Expand());
                if (EstatusCentro.Count()>0)
                {
                    
                
                if (centro.HasValue)
                {
                    predicate = predicate.And(w => w.ID_CENTRO == centro);
                }
                if (!string.IsNullOrEmpty(clave))
                {
                    predicate = predicate.And(w => w.ID_CLAVE.Trim().ToUpper().Contains(clave.Trim().ToUpper()));
                }
                if (!string.IsNullOrEmpty(valor))
                {
                    predicate = predicate.And(w => w.VALOR.Trim().ToUpper().Contains(valor.Trim().ToUpper()));
                }
                if (valor_num.HasValue)
                {
                    predicate = predicate.And(w => w.VALOR_NUM == valor_num);
                }
                if (!string.IsNullOrEmpty(descripcion))
                {
                    predicate = predicate.And(w => w.PARAMETRO_CLAVE.DESCR.Trim().ToUpper().Contains(descripcion.Trim().ToUpper()));
                }
                }       
                else
                {
                    return queryParametro;//<-regresa null en la consulta porque el centro que busca esta inactivo
                }
                return GetData(predicate.Expand());
            }
            catch (Exception ex)
            {
                throw new ApplicationException(ex.Message);
            }
        }

    

        public bool Insertar(PARAMETRO entity)
        {
            try
            {
                return Insert(entity);
            }
            catch (Exception ex)
            {
                throw new ApplicationException(ex.Message + " " + (ex.InnerException != null ? ex.InnerException.InnerException.Message : ""));
            }
        }
        public bool Editar(PARAMETRO entity)
        {
            try
            {
                return Update(entity);

            }
            catch (Exception ex)
            {
                throw new ApplicationException(ex.Message + " " + (ex.InnerException != null ? ex.InnerException.InnerException.Message : ""));
            }
        }
    }
}
