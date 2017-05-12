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
    public class cFotografias : EntityManagerServer<PRS_VISITA_FOTOGRAFIA>
    {
        public bool Insertar(PRS_VISITA_FOTOGRAFIA _entidad)
        {
            try
            {
                //No tiene Secuencia
                ///  _entidad.ID_FOLIO = GetSequence<short>("PRS_VISITA_DOMICILIARIA_SEQ");
                _entidad.ID_FOTO = Obtener_Id_Fotografia(_entidad.ID_IMPUTADO, (short)_entidad.ID_ANIO, (short)_entidad.ID_CENTRO);
                Insert(_entidad);
                return true;
            }
            catch (Exception ex)
            {
                throw new ApplicationException(ex.Message + " " + (ex.InnerException != null ? ex.InnerException.InnerException.Message : ""));
            }
        }
        public short Obtener_Id_Fotografia(int Id_imputado, short Id_Anio, short Id_Centro)
      {
              IQueryable<PRS_VISITA_FOTOGRAFIA> query;
              query = GetData().Where(w => w.ID_IMPUTADO == Id_imputado&& w.ID_ANIO == Id_Anio&& w.ID_CENTRO == Id_Centro);
          
          
          int FolioObt = query.Count() > 0 ? query.ToList().Max(mx => mx.ID_FOTO) : 0;
          FolioObt = FolioObt + 1;
          return short.Parse(FolioObt.ToString());
      }

        public List<PRS_VISITA_FOTOGRAFIA> Obtener(int Id_Imputado, short Id_Anio, short Id_Centro, short? Folio)
        {
            var Resultado = new List<PRS_VISITA_FOTOGRAFIA>();
            try
            {
                Resultado = GetData().Where(w => w.ID_IMPUTADO == Id_Imputado && w.ID_ANIO == Id_Anio && w.ID_CENTRO == Id_Centro).ToList();
            }
            catch (Exception ex)
            {
                throw new ApplicationException(ex.Message);
            }
            return Resultado;
        }
        public PRS_VISITA_FOTOGRAFIA ObtenerSingle(short IdImputado, short Id_Anio, short Id_Centro, short? Folio, short Id_Foto)
      {
          var Resultado = new PRS_VISITA_FOTOGRAFIA();
          try
          {
              Resultado = GetData().Where(w => w.ID_IMPUTADO == IdImputado && w.ID_ANIO == Id_Anio && w.ID_CENTRO == Id_Centro && w.ID_FOLIO == Folio && w.ID_FOTO == Id_Foto).FirstOrDefault();
          }
          catch (Exception ex)
          {
              throw new ApplicationException(ex.Message);
          }
          return Resultado;
      }



        public bool Actualizar(PRS_VISITA_FOTOGRAFIA _entidad)
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

        public bool Eliminar(PRS_VISITA_FOTOGRAFIA _entidad)
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
