using SSP.Modelo;
using SSP.Servidor;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace SSP.Controlador.Catalogo.Justicia
{
    public class cAbogadoCausaPenalDocumento : EntityManagerServer<ABOGADO_CP_DOCTO>
    {
        /// <summary>
        /// constructor de la clase
        /// </summary>
        public cAbogadoCausaPenalDocumento()
        { }

        /// <summary>
        /// metodo que conecta con la base de datos para extraer todos los registros
        /// </summary>
        /// <returns>listado de tipo "VisitaDocumento"</returns>
        public ObservableCollection<ABOGADO_CP_DOCTO> ObtenerXAbogadoYCausaPenal(short centro, short anio, int imputado, short ingreso, int causa_penal, int abogado, string titulo, short tipo_docto = 0)
        {
            ObservableCollection<ABOGADO_CP_DOCTO> VisitaDocumentos;
            var Resultado = new List<ABOGADO_CP_DOCTO>();
            try
            {
                Resultado = GetData().Where(w => w.ID_CENTRO == centro && w.ID_ANIO == anio && w.ID_IMPUTADO == imputado && w.ID_INGRESO == ingreso
                    && w.ID_CAUSA_PENAL == causa_penal && w.ID_ABOGADO == abogado && (tipo_docto == 0 ? true : w.ID_TIPO_DOCUMENTO == tipo_docto) &&
                    w.ID_ABOGADO_TITULO == titulo).ToList();
                VisitaDocumentos = new ObservableCollection<ABOGADO_CP_DOCTO>(Resultado);
            }
            catch (Exception ex)
            {
                throw new ApplicationException(ex.Message);
            }
            return VisitaDocumentos;
        }

        /// <summary>
        /// metodo que se conectara a la base de datos para regresar un registro 
        /// </summary>
        /// <param name="Id">valor de la llave primaria para obtener registro</param>
        /// <returns>objeto de tipo "ETNIA"</returns>
        public ABOGADO_CP_DOCTO ObtenerXAbogado(int Id)
        {
            var Resultado = new ABOGADO_CP_DOCTO();
            try
            {
                Resultado = GetData().Where(w => w.ID_ABOGADO == Id).FirstOrDefault();
            }
            catch (Exception ex)
            {
                throw new ApplicationException(ex.Message);
            }
            return Resultado;
        }

        ///// <summary>
        ///// metodo que se conecta a la base de datos para insertar un registro nuevo
        ///// </summary>
        ///// <param name="Entity">objeto de tipo "ESTADO" con valores a insertar</param>
        public bool Insertar(ABOGADO_CP_DOCTO Entity)
        {
            try
            {
                //Entity.ID_VisitaDocumento = GetSequence<short>("VisitaDocumento_SEQ");
                return Insert(Entity);
            }
            catch (Exception ex)
            {
                throw new ApplicationException(ex.Message);
            }
        }

        /// <summary>
        /// metodo que se conecta a la base de datos para actualizar un registro
        /// </summary>
        /// <param name="Entity">objeto de tipo "ESTADO" con valores a actualizar</param>
        public bool Actualizar(ABOGADO_CP_DOCTO Entity)
        {
            try
            {
                return Update(Entity);
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
        public bool Eliminar(ABOGADO_CP_DOCTO Id)
        {
            try
            {
                var Entity = GetData().Where(w => w == Id).SingleOrDefault();
                if (Entity != null)
                    return Delete(Entity);
                else
                    return false;
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
        }
    }
}