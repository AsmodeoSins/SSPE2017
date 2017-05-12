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
    public class cReportePsicologico : EntityManagerServer<PRS_REPORTE_PSICOLOGICO>
    {

        public bool Insertar(PRS_REPORTE_PSICOLOGICO _entidad)
        {
            try
            {
                //No tiene Secuencia
                _entidad.ID_FOLIO = ObtenerId(_entidad.ID_IMPUTADO, _entidad.ID_ANIO, _entidad.ID_CENTRO);
                ///  _entidad.ID_FOLIO = GetSequence<short>("PRS_VISITA_DOMICILIARIA_SEQ");
                Insert(_entidad);
                return true;
            }
            catch (Exception ex)
            {
                throw new ApplicationException(ex.Message + " " + (ex.InnerException != null ? ex.InnerException.InnerException.Message : ""));
            }
        }

        private short ObtenerId(int Id_imputado, short Id_Anio, short Id_Centro)    
        {
            var query = GetData().Where(w => w.ID_IMPUTADO == Id_imputado && w.ID_ANIO == Id_Anio && w.ID_CENTRO == Id_Centro);
            int FolioObt = query.Count() > 0 ? query.ToList().Max(mx => mx.ID_FOLIO) : 0;
            FolioObt = FolioObt + 1;
            return short.Parse(FolioObt.ToString());
        }

        public List<PRS_REPORTE_PSICOLOGICO> Obtener(long Id)
        {
            var Resultado = new List<PRS_REPORTE_PSICOLOGICO>();
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

        public PRS_REPORTE_PSICOLOGICO Obtener(int? Centro = null,int? Anio = null,int? Imputado = null,int? ProcesoLibertad = null)
        {
            try
            {
                var predicate = PredicateBuilder.True<PRS_REPORTE_PSICOLOGICO>();
                if (Centro.HasValue)
                    predicate = predicate.And(w => w.ID_CENTRO == Centro);
                if(Anio.HasValue)
                    predicate = predicate.And(w => w.ID_ANIO == Anio);
                if(Imputado.HasValue)
                    predicate = predicate.And(w => w.ID_IMPUTADO == Imputado);
                if(ProcesoLibertad.HasValue)
                    predicate = predicate.And(w => w.ID_PROCESO_LIBERTAD == ProcesoLibertad);
                return GetData(predicate.Expand()).FirstOrDefault();
            }
            catch (Exception ex)
            {
                throw new ApplicationException(ex.Message);
            }
        }
        
        public PRS_REPORTE_PSICOLOGICO ObtenerUltimaEntrevista(IMPUTADO ObjImputado)
        {
            var query=GetData().Where(w => w.ID_IMPUTADO == ObjImputado.ID_IMPUTADO && w.ID_ANIO == ObjImputado.ID_ANIO && w.ID_CENTRO == ObjImputado.ID_CENTRO);
            if (query.Count()>0)
            {
                short UltimoFolio = query.Max(m => m.ID_FOLIO);
                return   query.Where(w => w.ID_FOLIO == UltimoFolio).FirstOrDefault();
            }
            return null;
            
        }

        public bool Actualizar(PRS_REPORTE_PSICOLOGICO _entidad)
        {
            try
            {
                Update(_entidad);
                return true;
            }
            catch (Exception ex)
            {
                throw new ApplicationException(ex.Message + " " + (ex.InnerException != null ? ex.InnerException.InnerException.Message : ""));
            }
        }

        public bool Eliminar(PRS_REPORTE_PSICOLOGICO _entidad)
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
