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
    public class cNuceloFamiliar : EntityManagerServer<PRS_NUCLEO_FAMILIAR>
    {

        public bool Insertar(PRS_NUCLEO_FAMILIAR _entidad)
        {
            try
            {
                //No tiene Secuencia
                ///  _entidad.ID_FOLIO = GetSequence<short>("PRS_VISITA_DOMICILIARIA_SEQ");
                _entidad.ID_PERS = Obtener_Id_Persona((short)_entidad.ID_IMPUTADO, (short)_entidad.ID_ANIO, (short)_entidad.ID_CENTRO, _entidad.ID_TIPO);
                Insert(_entidad);
                return true;
            }
            catch (Exception ex)
            {
                throw new ApplicationException(ex.Message + " " + (ex.InnerException != null ? ex.InnerException.InnerException.Message : ""));
            }
        }
        public short Obtener_Id_Persona(int Id_imputado, short Id_Anio, short Id_Centro, string Tipo)
        {
            IQueryable<PRS_NUCLEO_FAMILIAR> query;

            query = GetData().Where(w => w.ID_IMPUTADO == Id_imputado && w.ID_ANIO == Id_Anio && w.ID_CENTRO == Id_Centro && w.ID_TIPO == Tipo);
            int PersonaObt = query.Count() > 0 ? query.ToList().Max(mx => mx.ID_PERS) : 0;

            PersonaObt = PersonaObt + 1;

            return short.Parse(PersonaObt.ToString());

        }

        public List<PRS_NUCLEO_FAMILIAR> Obtener(short Id_Imputado, short Id_Anio, short Id_Centro, short? Folio, string Tipo)
        {
            var Resultado = new List<PRS_NUCLEO_FAMILIAR>();
            try
            {
                Resultado = GetData().Where(w => w.ID_IMPUTADO == Id_Imputado && w.ID_ANIO == Id_Anio && w.ID_CENTRO == Id_Centro && w.ID_TIPO == Tipo).ToList();
            }
            catch (Exception ex)
            {
                throw new ApplicationException(ex.Message);
            }
            return Resultado;
        }
        public PRS_NUCLEO_FAMILIAR ObtenerSingle(int IdImputado, short Id_Anio, short Id_Centro, short Id_Pers, string Tipo)
        {
            var Resultado = new PRS_NUCLEO_FAMILIAR();
            try
            {
                Resultado = GetData().Where(w => w.ID_IMPUTADO == IdImputado && w.ID_ANIO == Id_Anio && w.ID_CENTRO == Id_Centro && w.ID_TIPO == Tipo && w.ID_PERS == Id_Pers).ToList().FirstOrDefault();
            }
            catch (Exception ex)
            {
                throw new ApplicationException(ex.Message);
            }
            return Resultado;
        }



        public bool Actualizar(PRS_NUCLEO_FAMILIAR _entidad)
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

        public bool Eliminar(PRS_NUCLEO_FAMILIAR _entidad)
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
