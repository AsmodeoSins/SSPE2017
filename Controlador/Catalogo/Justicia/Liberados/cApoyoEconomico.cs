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
  public  class cApoyoEconomico : EntityManagerServer<PRS_APOYO_ECONOMICO>
    {

      public bool Insertar(PRS_APOYO_ECONOMICO _entidad)
        {
            try
            {
                //No tiene Secuencia se creo un metodo simulacion secuencial
                ///  _entidad.ID_FOLIO = GetSequence<short>("PRS_VISITA_DOMICILIARIA_SEQ");
                short Id_Inicial = 1;
                _entidad.ID_PERS = Obtener_Id_Persona(_entidad.ID_IMPUTADO, _entidad.ID_ANIO, _entidad.ID_CENTRO);
                Insert(_entidad);
                
                return true;
            }
            catch (Exception ex)
            {
                throw new ApplicationException(ex.Message + " " + (ex.InnerException != null ? ex.InnerException.InnerException.Message : ""));
            }
        }

      public short Obtener_Id_Persona(int Id_imputado, short Id_Anio, short Id_Centro)
      {
          var query = GetData().Where(w => w.ID_IMPUTADO == Id_imputado && w.ID_ANIO == Id_Anio && w.ID_CENTRO == Id_Centro);

          int PersonaObt = query.Count() > 0 ? query.ToList().Max(mx => mx.ID_PERS) : 0;

          PersonaObt = PersonaObt + 1;

          return short.Parse(PersonaObt.ToString());
      }
      public IQueryable<PRS_APOYO_ECONOMICO> ObtenerTodos(short Id_Imputado,short Id_Anio, short Id_Centro)
      {
          return GetData().Where(w => w.ID_IMPUTADO == Id_Imputado && w.ID_ANIO == Id_Anio &&w.ID_CENTRO==Id_Centro);
          
      }
      public IQueryable<PRS_APOYO_ECONOMICO> ObtenerTodosImputado(short Id_Imputado)
      {
          return GetData().Where(w=>w.ID_IMPUTADO==Id_Imputado);
      }


      public List<PRS_APOYO_ECONOMICO> Obtener(int Id_Imputado,short Id_Anio, short Id_Centro,long Id_Persona)
        {
            var Resultado = new List<PRS_APOYO_ECONOMICO>();
            try
            {
                Resultado = GetData().Where(w => w.ID_IMPUTADO==Id_Imputado&&w.ID_ANIO==Id_Anio&&w.ID_CENTRO==Id_Centro&&w.ID_PERS == Id_Persona).ToList();
            }
            catch (Exception ex)
            {
                throw new ApplicationException(ex.Message);
            }
            return Resultado;
        }

        public bool Actualizar(PRS_APOYO_ECONOMICO _entidad)
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

        public bool Eliminar(PRS_APOYO_ECONOMICO _entidad)
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
