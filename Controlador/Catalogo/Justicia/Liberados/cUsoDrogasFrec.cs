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
    public class cUsoDrogasFrec : EntityManagerServer<PRS_DROGA>
    {
        public bool Insertar(PRS_DROGA _entidad)
        {
            try
            {
                //No tiene Secuencia (Llave primaria) Se relaciona con el Folio de PRS_ENTEREVISTA_INICIAL
                
                Insert(_entidad);
                return true;
            }
            catch (Exception ex)
            {
                throw new ApplicationException(ex.Message + " " + (ex.InnerException != null ? ex.InnerException.InnerException.Message : ""));
            }
        }
       

        public List<PRS_DROGA> Obtener(int IdImputado, short Id_Anio, short Id_Centro)
        {
            var Resultado = new List<PRS_DROGA>();
            try
            {
                Resultado = GetData().Where(w => w.ID_IMPUTADO == IdImputado&&w.ID_ANIO==Id_Anio&&w.ID_CENTRO==Id_Centro).ToList();
            }
            catch (Exception ex)
            {
                throw new ApplicationException(ex.Message);
            }
            return Resultado;
        }

        public IQueryable<PRS_DROGA> ObtenerTodos()
        {
            return GetData();
        }
        public PRS_DROGA ObtenerSingle(PRS_DROGA ObjfrecuencDroga)
        {
            return GetData().Where(w => w.ID_IMPUTADO == ObjfrecuencDroga.ID_IMPUTADO && w.ID_CENTRO == ObjfrecuencDroga.ID_CENTRO && w.ID_ANIO == ObjfrecuencDroga.ID_ANIO && w.ID_DROGA == ObjfrecuencDroga.ID_DROGA && w.ID_FRECUENCIA == ObjfrecuencDroga.ID_FRECUENCIA && w.INICIO_CONSUMO == ObjfrecuencDroga.INICIO_CONSUMO).FirstOrDefault();
        }

        public bool Actualizar(PRS_DROGA _entidad)
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

        public bool Eliminar(PRS_DROGA _entidad)
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
