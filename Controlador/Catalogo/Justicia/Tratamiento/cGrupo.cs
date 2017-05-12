using SSP.Modelo;
using SSP.Servidor;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace SSP.Controlador.Catalogo.Justicia
{
    public class cGrupo : EntityManagerServer<GRUPO>
    {
        public List<GRUPO> ObtenerTodos(string buscar = "")
        {
            getDbSet();
            if (buscar.Equals(string.Empty))
            {
                return GetData().ToList();
            }
            else
            {
                return GetData().Where(w => w.DESCR.Contains(buscar)).ToList();
            }
        }

        ///// <summary>
        ///// metodo que se conecta a la base de datos para insertar un registro nuevo
        ///// </summary>
        ///// <param name="Entity">objeto de tipo "GRUPO" con valores a insertar</param>
        public short Insertar(GRUPO Entity)
        {
            try
            {
                Entity.ID_GRUPO = GetSequence<short>("GRUPO_SEQ");
                Insert(Entity);
                return Entity.ID_GRUPO;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public void CrearGrupoRollback(short idgrupo)
        {
            try
            {
                var context = new SSPEntidades();
                context.Conexion();

                if (context.GRUPO_ASISTENCIA.Where(w => w.ID_GRUPO == idgrupo).Any())
                {
                    foreach (var entity in context.GRUPO_ASISTENCIA.Where(w => w.ID_GRUPO == idgrupo))
                    {
                        context.Set<GRUPO_ASISTENCIA>().Attach(entity);
                        context.Entry(entity).State = EntityState.Deleted;
                    }
                    context.SaveChanges();
                }
                if (context.GRUPO_HORARIO.Where(w => w.ID_GRUPO == idgrupo).Any())
                {
                    foreach (var entity in context.GRUPO_HORARIO.Where(w => w.ID_GRUPO == idgrupo))
                    {
                        context.Set<GRUPO_HORARIO>().Attach(entity);
                        context.Entry(entity).State = EntityState.Deleted;
                    }
                    context.SaveChanges();
                }
                if (context.GRUPO_PARTICIPANTE.Where(w => w.ID_GRUPO == idgrupo).Any())
                {
                    foreach (var entity in context.GRUPO_PARTICIPANTE.Where(w => w.ID_GRUPO == idgrupo))
                    {
                        entity.ID_GRUPO = new Nullable<short>();
                        entity.ESTATUS = 1;

                        context.Set<GRUPO_PARTICIPANTE>().Attach(entity);
                        context.Entry(entity).State = EntityState.Modified;
                    }
                    context.SaveChanges();
                }
                if (context.GRUPO.Where(w => w.ID_GRUPO == idgrupo).Any())
                {
                    var entity = context.GRUPO.Where(w => w.ID_GRUPO == idgrupo).FirstOrDefault();
                    context.Set<GRUPO>().Attach(entity);
                    context.Entry(entity).State = EntityState.Deleted;
                    context.SaveChanges();
                }

                /*
                delete from ssp.grupo_asistencia;
                delete from ssp.grupo_horario;
                update ssp.grupo_participante set id_grupo = null, estatus = 1;
                delete from ssp.grupo;*/
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}
