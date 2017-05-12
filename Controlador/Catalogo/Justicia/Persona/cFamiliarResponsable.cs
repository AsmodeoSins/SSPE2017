using SSP.Servidor;
using SSP.Modelo;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Collections.ObjectModel;
using System.Data.Objects.SqlClient;

namespace SSP.Controlador.Catalogo.Justicia
{
    public class cFamiliarResponsable: EntityManagerServer<FAMILIAR_RESPONSABLE>
    {
        /// <summary>
        /// constructor de la clase
        /// </summary>
        public cFamiliarResponsable()
        { }

        /// <summary>
        /// metodo que conecta con la base de datos para extraer todos los registros
        /// </summary>
        /// <returns>listado de tipo "RELACION_PERSONAL_INTERNO"</returns>
        public ObservableCollection<FAMILIAR_RESPONSABLE> ObtenerTodos(int Centro, int Anio, int Imputado)
        {
            ObservableCollection<FAMILIAR_RESPONSABLE> familiar_responsable;
            var Resultado = new List<FAMILIAR_RESPONSABLE>();
            try
            {
                if (Centro == 0)
                    Resultado = GetData().ToList();
                else
                    Resultado = GetData().Where(w => w.ID_CENTRO == Centro && w.ID_ANIO == Anio && w.ID_IMPUTADO == Imputado).ToList();

                familiar_responsable = new ObservableCollection<FAMILIAR_RESPONSABLE>(Resultado);
            }
            catch(Exception ex)
            {
                throw new ApplicationException(ex.Message);
            }
            return familiar_responsable;
        }

        /// <summary>
        /// metodo que se conectara a la base de datos para regresar un registro 
        /// </summary>
        /// <param name="Id">valor de la llave primaria para obtener registro</param>
        /// <returns>objeto de tipo "FAMILIAR_RESPONSABLE"</returns>
        public List<FAMILIAR_RESPONSABLE> Obtener(int Persona, int Centro, int Anio, int Imputado)
        {
            var Resultado = new List<FAMILIAR_RESPONSABLE>();
            try
            {
                Resultado = GetData().Where(w => w.ID_CENTRO == Centro && w.ID_ANIO == Anio && w.ID_IMPUTADO == Imputado && w.ID_PERSONA == Persona).ToList();
            }
            catch (Exception ex)
            {
                throw new ApplicationException(ex.Message);
            }
            return Resultado;
        }

        /// <summary>
        /// metodo que se conecta a la base de datos para insertar un registro nuevo
        /// </summary>
        /// <param name="Entity">objeto de tipo "FAMILIAR_RESPONSABLE" con valores a insertar</param>
        public bool Insertar(short Centro, short Anio, int Imputado, List<FAMILIAR_RESPONSABLE> list)
        {
            try
            {
                
                Insert(list);
                return true;
            }
            catch (Exception ex)
            {
                throw new ApplicationException(ex.Message);
            }
            return false;
        }

        /// <summary>
        /// metodo que se conecta a la base de datos para actualizar un registro
        /// </summary>
        /// <param name="Entity">objeto de tipo "FAMILIAR_RESPONSABLE" con valores a actualizar</param>
        public void Actualizar(FAMILIAR_RESPONSABLE Entity)
        {
            try
            {
                Update(Entity);
            }
            catch (Exception ex)
            {
                if (ex.Message.Contains("part of the object's key information"))
                    throw new ApplicationException("La llave principal no se puede cambiar");
                else
                    throw new ApplicationException(ex.Message);
            }
        }

        /// <summary>
        /// metodo que se conecta a la base de datos para eliminar un registro
        /// </summary>
        /// <param name="Id">valor de la llave primaria para obtener registro</param>
        /// <returns>"True" para eliminado, "False" para no encontrado</returns>
        public bool Eliminar(int Centro, int Anio, int Imputado)
        {
            try
            {
                var ListEntity = GetData().Where(w => w.ID_CENTRO == Centro && w.ID_ANIO == Anio && w.ID_IMPUTADO == Imputado);//.SingleOrDefault();
                foreach (var entity in ListEntity)
                {
                    Delete(entity);
                }
                return true;
              
            }
            catch (Exception ex)
            {
                if (ex.InnerException != null)
                {
                    if (ex.InnerException.InnerException.Message.Contains("child record found") || ex.InnerException.InnerException.Message.Contains("registro secundario encontrado"))
                        throw new ApplicationException("Este registro se encuentra ligado a otro registro, por lo tanto no se puede eliminar");
                }
                throw new ApplicationException(ex.Message + " " + (ex.InnerException != null ? ex.InnerException.InnerException.Message : ""));
            }
            return false;
        }
    }
}