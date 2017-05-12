using SSP.Modelo;
using SSP.Servidor;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Transactions;

namespace SSP.Controlador.Catalogo.Justicia
{
    public class cNotaEgreso:EntityManagerServer<NOTA_EGRESO>
    {
        public void Insertar(NOTA_EGRESO entidad, DateTime fecha_hospitalizacion, int id_medico_alta)
        {
            try
            {
                using (TransactionScope transaccion = new TransactionScope(TransactionScopeOption.RequiresNew, new TransactionOptions() { IsolationLevel = System.Transactions.IsolationLevel.ReadCommitted }))
                {
                    var id_atencion_medica = GetIDProceso<int>("ATENCION_MEDICA", "ID_ATENCION_MEDICA", string.Format("ID_CENTRO_UBI={0}", entidad.ID_CENTRO_UBI.ToString()));
                    entidad.NOTA_MEDICA.ATENCION_MEDICA.ID_ATENCION_MEDICA = id_atencion_medica;
                    entidad.NOTA_MEDICA.ID_ATENCION_MEDICA = id_atencion_medica;
                    entidad.ID_ATENCION_MEDICA = id_atencion_medica;
                    var _receta_medica = entidad.NOTA_MEDICA.RECETA_MEDICA.FirstOrDefault();
                    if (_receta_medica != null)
                    {
                        _receta_medica.ID_ATENCION_MEDICA = id_atencion_medica;
                        foreach (var item in _receta_medica.RECETA_MEDICA_DETALLE)
                            item.ID_ATENCION_MEDICA = id_atencion_medica;
                    }
                    foreach (var item in entidad.NOTA_MEDICA.NOTA_MEDICA_ENFERMEDAD)
                        item.ID_ATENCION_MEDICA = id_atencion_medica;
                    foreach (var item in entidad.NOTA_MEDICA.NOTA_MEDICA_DIETA)
                        item.ID_ATENCION_MEDICA = id_atencion_medica;
                    int id_atencion_cita = 0;
                    if (entidad.NOTA_MEDICA.ATENCION_MEDICA.ATENCION_CITA1 != null)
                        id_atencion_cita = GetIDProceso<int>("ATENCION_CITA", "ID_CITA", string.Format("ID_CENTRO_UBI={0}", entidad.ID_CENTRO_UBI));
                    if (entidad.NOTA_MEDICA.ATENCION_MEDICA.ATENCION_CITA1 != null)
                    {

                        entidad.NOTA_MEDICA.ATENCION_MEDICA.ATENCION_CITA1.ID_CITA = id_atencion_cita;
                        entidad.NOTA_MEDICA.ATENCION_MEDICA.ID_CITA_SEGUIMIENTO = id_atencion_cita;
                    }
                    short _cont_temp = 1;
                    if ((entidad.NOTA_MEDICA.ATENCION_MEDICA.PROC_ATENCION_MEDICA != null || entidad.NOTA_MEDICA.ATENCION_MEDICA.PROC_ATENCION_MEDICA.Count > 0) && id_atencion_cita == 0)
                        id_atencion_cita = GetIDProceso<int>("ATENCION_CITA", "ID_CITA", string.Format("ID_CENTRO_UBI={0}", entidad.ID_CENTRO_UBI));
                    else if (entidad.NOTA_MEDICA.ATENCION_MEDICA.PROC_ATENCION_MEDICA != null || entidad.NOTA_MEDICA.ATENCION_MEDICA.PROC_ATENCION_MEDICA.Count > 0)
                        id_atencion_cita += 1;
                    foreach (var item in entidad.NOTA_MEDICA.ATENCION_MEDICA.PROC_ATENCION_MEDICA)
                    {

                        item.ID_ATENCION_MEDICA = id_atencion_medica;
                        item.ID_PROCMED = _cont_temp;
                        var _cont_temp2 = 1;
                        foreach (var item_prog in item.PROC_ATENCION_MEDICA_PROG)
                        {
                            if (!item_prog.ID_CITA.HasValue || item_prog.ID_CITA == 0)
                            {
                                item_prog.ATENCION_CITA.ID_CITA = id_atencion_cita;
                                item_prog.ID_CITA = id_atencion_cita;
                                var cita_copia = item_prog.ATENCION_CITA;
                                foreach (var item_empata in entidad.NOTA_MEDICA.ATENCION_MEDICA.PROC_ATENCION_MEDICA.Where(w => w.ID_PROCMED != item.ID_PROCMED && w.PROC_ATENCION_MEDICA_PROG.Any(a => a.ATENCION_CITA.CITA_FECHA_HORA == item_prog.ATENCION_CITA.CITA_FECHA_HORA && (!a.ID_CITA.HasValue || a.ID_CITA == 0))))
                                {
                                    foreach (var item_cita in item_empata.PROC_ATENCION_MEDICA_PROG.Where(w => w.ATENCION_CITA.CITA_FECHA_HORA == item_prog.ATENCION_CITA.CITA_FECHA_HORA))
                                    {
                                        item_cita.ID_CITA = id_atencion_cita;
                                        item_cita.ATENCION_CITA = cita_copia;
                                    }
                                }
                                id_atencion_cita += 1;
                            }
                            item_prog.ID_ATENCION_MEDICA = id_atencion_medica;
                            item_prog.ID_PROCMED = _cont_temp;
                            item_prog.ID_AM_PROG = _cont_temp2;
                            _cont_temp2 += 1;


                        }
                        _cont_temp += 1;
                    }
                    Context.NOTA_EGRESO.Add(entidad);
                    Context.SaveChanges();
                    var _hospitalizacion = Context.HOSPITALIZACION.FirstOrDefault(w => w.NOTA_MEDICA.ATENCION_MEDICA.ID_ANIO == entidad.ID_ANIO && w.NOTA_MEDICA.ATENCION_MEDICA.ID_CENTRO == entidad.ID_CENTRO
                        && w.NOTA_MEDICA.ATENCION_MEDICA.ID_IMPUTADO == entidad.ID_IMPUTADO && w.ID_HOSEST == 1);
                    if (_hospitalizacion == null)
                        throw new Exception("No existe ninguna hospitalizacion activa relacionada con esta alta de hospitalizacion");
                    _hospitalizacion.ID_HOSEST = 2;
                    _hospitalizacion.ALTA_FEC = fecha_hospitalizacion;
                    _hospitalizacion.ALTA_MEDICO = id_medico_alta;
                    _hospitalizacion.CAMA_HOSPITAL.ESTATUS = "S";
                    Context.SaveChanges();
                    transaccion.Complete();

                }
            }
            catch(Exception ex)
            {
                throw new ApplicationException(ex.Message + " " + (ex.InnerException != null ? ex.InnerException.InnerException.Message : ""));
            }
        }
    }
}
