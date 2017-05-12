using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace WCF_BiometricoService.Modelo.Rule
{
    public class MainRule<TEntity>
        where TEntity : class
    {
        HuellasEntities ctx;

        public MainRule()
        {
            ctx = new HuellasEntities();
        }

        public bool Conexion()
        {
            return ctx.Database.Exists();
        }

        public virtual IQueryable<TEntity> Listado()
        {
            try
            {
                return ctx.Set<TEntity>();
            }
            catch (Exception ex)
            {
                throw new ApplicationException(ex.Message);
            }
        }

        public virtual void Insertar(TEntity entity)
        {
            try
            {
                ctx.Set<TEntity>().Add(entity);
                ctx.SaveChanges();
            }
            catch (Exception ex)
            {
                throw new ApplicationException(ex.Message);
            }
            finally
            {
                ctx = null;
            }
        }

        public virtual void Insertar(TEntity[] entity)
        {
            try
            {
                foreach (var item in entity)
                    ctx.Set<TEntity>().Add(item);
                ctx.SaveChanges();
            }
            catch (Exception ex)
            {
                throw new ApplicationException(ex.Message);
            }
            finally
            {
                ctx = null;
            }
        }

        public virtual void Actualizar(TEntity entity)
        {
            try
            {
                ctx.Set<TEntity>().Attach(entity);
                ctx.Entry(entity).State = EntityState.Modified;
                ctx.SaveChanges();
            }
            catch (Exception ex)
            {
                throw new ApplicationException(ex.Message);
            }
            finally
            {
                ctx = null;
            }
        }

        public virtual void Actualizar(TEntity[] entity)
        {
            try
            {
                foreach (var item in entity)
                {
                    ctx.Set<TEntity>().Attach(item);
                    ctx.Entry(item).State = EntityState.Modified;
                }
                ctx.SaveChanges();
            }
            catch (Exception ex)
            {
                throw new ApplicationException(ex.Message);
            }
            finally
            {
                ctx = null;
            }
        }

        public virtual void Eliminar(TEntity entity)
        {
            try
            {
                ctx.Set<TEntity>().Attach(entity);
                ctx.Entry(entity).State = EntityState.Deleted;
                ctx.SaveChanges();
            }
            catch (Exception ex)
            {
                throw new ApplicationException(ex.Message);
            }
            finally
            {
                ctx = null;
            }
        }

        public virtual void Eliminar(List<TEntity> entity)
        {
            try
            {
                foreach (var item in entity)
                {
                    ctx.Set<TEntity>().Attach(item);
                    ctx.Entry(item).State = EntityState.Deleted;
                }
                ctx.SaveChanges();
            }
            catch (Exception ex)
            {
                throw new ApplicationException(ex.Message);
            }
            finally
            {
                ctx = null;
            }
        }
    }
}
