using SSP.Modelo;
using SSP.Servidor;
using System;
using System.Linq;

namespace SSP.Controlador.Catalogo.Justicia
{
    public class cGrupoHorarioEstatus : EntityManagerServer<GRUPO_HORARIO_ESTATUS>
    {
        ///// <summary>
        ///// metodo que se conecta a la base de datos para insertar un registro nuevo
        ///// </summary>
        ///// <param name="Entity">objeto de tipo "GRUPO" con valores a insertar</param>
        public bool Insertar(GRUPO_HORARIO_ESTATUS Entity)
        {
            try
            {
                Entity.ID_ESTATUS = GetSequence<short>("GRUPO_HORARIO_ESTATUS_SEQ");
                return Insert(Entity);
            }
            catch (Exception ex)
            {
                throw new ApplicationException(ex.Message);
            }
        }
    }
}
