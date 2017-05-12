using SSP.Modelo;
using SSP.Servidor;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Transactions;

namespace SSP.Controlador.Catalogo.Justicia
{
    public class cTraslado_Virtual_Medico:EntityManagerServer<INGRESO>
    {

        /// <summary>
        /// Obtiene los ingresos con atenciones medicas/interconsultas/canalizacion pendientes del centro de origen
        /// </summary>
        /// <param name="id_ub_centro">Id del centro sobre el cual se va a realizar la consulta</param>
        /// <param name="id_anio">Id anio del imputado</param>
        /// <param name="id_imputado">Id imputado del imputado</param>
        /// <param name="nombre">Nombre del imputado</param>
        /// <param name="paterno">Apellido paterno del imputado</param>
        /// <param name="materno">Apellido materno del imputado</param>
        /// <param name="centro_origen">Centro de origen del traslado</param>
        /// <param name="fecha_ini">Fecha inicial para el rango de busqueda de la fecha del traslado</param>
        /// <param name="fecha_fin">Fecha final para el rango de busqueda de la fecha del traslado</param>
        /// <returns></returns>
        public IEnumerable<TRASLADO_VIRTUAL_INGRESO> ObtenerTrasladoVirtualMedico(short id_ub_centro,short?[] estatus_administrativos_inactivos, int? id_anio=null, int? id_imputado=null, string nombre="", string paterno="",
            string materno="", int? centro_origen=null, DateTime? fecha_ini=null, DateTime? fecha_fin=null)
        {
            try
            {

                var query = string.Empty;
                if (centro_origen.HasValue || fecha_ini.HasValue || fecha_fin.HasValue)
                {
                    query=string.Format("SELECT Distinct ING.ID_CENTRO, ING.ID_ANIO, ING.ID_IMPUTADO, ING.ID_INGRESO, " + 
                    "IMP.PATERNO,IMP.MATERNO, IMP.NOMBRE, " +
                    "SSP.TRASLADO_VIRTUAL_CENTRO_ORIGEN(ING.ID_CENTRO, ING.ID_ANIO, ING.ID_IMPUTADO, ING.ID_INGRESO) AS CENTRO_ORIGEN, " +
                    "SSP.TRASLADO_VIRTUAL_FECHA_EGRESO(ING.ID_CENTRO, ING.ID_ANIO, ING.ID_IMPUTADO, ING.ID_INGRESO) AS FECHA_EGRESO " +
                    "FROM SSP.INGRESO ING, SSP.IMPUTADO IMP, SSP.TV_CITA_MEDICA TVCM, SSP.TV_CANALIZACION TVC," +
                    "SSP.TV_INTERCONSULTA_SOLICITUD TVIS " +
                    "WHERE ING.ID_UB_CENTRO={0} " +
                    "{1}" +
                    "{2}" +
                    "{3}" +
                    "{4}" +
                    "{5}" +
                    "AND ING.ID_CENTRO=IMP.ID_CENTRO " +
                    "AND ING.ID_ANIO=IMP.ID_ANIO " +
                    "AND ING.ID_IMPUTADO=IMP.ID_IMPUTADO " +
                    "AND ((ING.ID_ANIO=TVCM.ID_ANIO AND ING.ID_CENTRO=TVCM.ID_CENTRO AND ING.ID_IMPUTADO=TVCM.ID_IMPUTADO AND " + 
                    "ING.ID_INGRESO=TVCM.ID_INGRESO AND TVCM.ID_TV_MEDICO_ESTATUS='PE') " +
                    "OR (ING.ID_ANIO=TVC.ID_ANIO AND ING.ID_CENTRO=TVC.ID_CENTRO AND ING.ID_IMPUTADO=TVC.ID_IMPUTADO AND " + 
                    "ING.ID_INGRESO=TVC.ID_INGRESO AND TVC.ID_TV_MEDICO_ESTATUS='PE') " +
                    "OR (ING.ID_ANIO=TVIS.ID_ANIO AND ING.ID_CENTRO=TVIS.ID_CENTRO AND ING.ID_IMPUTADO=TVIS.ID_IMPUTADO AND " +
                    "ING.ID_INGRESO=TVIS.ID_INGRESO AND TVIS.ID_TV_MEDICO_ESTATUS='PE')) " +
                    "AND SSP.TRASLADO_VIRTUAL_VALIDACION(ING.ID_CENTRO, ING.ID_ANIO, ING.ID_IMPUTADO, ING.ID_INGRESO, {6},{7},{8})=1"
                    , id_ub_centro, id_anio.HasValue? "AND ING.ID_ANIO=" +id_anio.Value.ToString() + " " : "" ,
                    id_imputado.HasValue ? "AND ING.ID_IMPUTADO=" + id_imputado.Value.ToString() + " " : "" , 
                    !string.IsNullOrWhiteSpace(nombre)?"AND IMP.NOMBRE like '%'" + nombre +"'%' ": "",
                    !string.IsNullOrWhiteSpace(paterno)?"AND IMP.PATERNO like '%'" + paterno +"'%' ": "",
                    !string.IsNullOrWhiteSpace(materno)?"AND IMP.PATERNO like '%'" + materno +"'%' ": "",
                    centro_origen.HasValue?centro_origen.ToString():"NULL",
                    fecha_ini.HasValue ? "TO_DATE('" + fecha_ini.Value.ToShortDateString() + "','dd/mm/yyyy')" : "NULL",
                    fecha_fin.HasValue ? "TO_DATE('" + fecha_fin.Value.ToShortDateString() + "','dd/mm/yyyy')" : "NULL");
                }
                else
                    query = string.Format("SELECT Distinct ING.ID_CENTRO, ING.ID_ANIO, ING.ID_IMPUTADO, ING.ID_INGRESO, " +
                    "IMP.PATERNO,IMP.MATERNO, IMP.NOMBRE, " +
                    "SSP.TRASLADO_VIRTUAL_CENTRO_ORIGEN(ING.ID_CENTRO, ING.ID_ANIO, ING.ID_IMPUTADO, ING.ID_INGRESO) AS CENTRO_ORIGEN, " +
                    "SSP.TRASLADO_VIRTUAL_FECHA_EGRESO(ING.ID_CENTRO, ING.ID_ANIO, ING.ID_IMPUTADO, ING.ID_INGRESO) AS FECHA_EGRESO " +
                    "FROM SSP.INGRESO ING, SSP.IMPUTADO IMP, SSP.TV_CITA_MEDICA TVCM, SSP.TV_CANALIZACION TVC," +
                    "SSP.TV_INTERCONSULTA_SOLICITUD TVIS " +
                    "WHERE ING.ID_UB_CENTRO={0} " +
                    "{1}" +
                    "{2}" +
                    "{3}" +
                    "{4}" +
                    "{5}" +
                    "AND ING.ID_CENTRO=IMP.ID_CENTRO " +
                    "AND ING.ID_ANIO=IMP.ID_ANIO " +
                    "AND ING.ID_IMPUTADO=IMP.ID_IMPUTADO " +
                    "AND ((ING.ID_ANIO=TVCM.ID_ANIO AND ING.ID_CENTRO=TVCM.ID_CENTRO AND ING.ID_IMPUTADO=TVCM.ID_IMPUTADO AND " +
                    "ING.ID_INGRESO=TVCM.ID_INGRESO AND TVCM.ID_TV_MEDICO_ESTATUS='PE') " +
                    "OR (ING.ID_ANIO=TVC.ID_ANIO AND ING.ID_CENTRO=TVC.ID_CENTRO AND ING.ID_IMPUTADO=TVC.ID_IMPUTADO AND " +
                    "ING.ID_INGRESO=TVC.ID_INGRESO AND TVC.ID_TV_MEDICO_ESTATUS='PE') " +
                    "OR (ING.ID_ANIO=TVIS.ID_ANIO AND ING.ID_CENTRO=TVIS.ID_CENTRO AND ING.ID_IMPUTADO=TVIS.ID_IMPUTADO AND " +
                    "ING.ID_INGRESO=TVIS.ID_INGRESO AND TVIS.ID_TV_MEDICO_ESTATUS='PE')) "
                    , id_ub_centro, id_anio.HasValue ? "AND ING.ID_ANIO=" + id_anio.Value.ToString() + " " : "",
                    id_imputado.HasValue ? "AND ING.ID_IMPUTADO=" + id_imputado.Value.ToString() + " " : "",
                    !string.IsNullOrWhiteSpace(nombre) ? "AND IMP.NOMBRE like '%'" + nombre + "'%' " : "",
                    !string.IsNullOrWhiteSpace(paterno) ? "AND IMP.PATERNO like '%'" + paterno + "'%' " : "",
                    !string.IsNullOrWhiteSpace(materno) ? "AND IMP.PATERNO like '%'" + materno + "'%' " : "");
                foreach (var _item in estatus_administrativos_inactivos)
                {
                    if (_item.HasValue)
                        query += (" AND ING.ID_ESTATUS_ADMINISTRATIVO != " + _item.Value.ToString());
                }
                return Context.Database.SqlQuery<TRASLADO_VIRTUAL_INGRESO>(query);
            }
            catch (Exception ex)
            {
                throw new ApplicationException(ex.Message + " " + (ex.InnerException != null ? ex.InnerException.InnerException.Message : ""));
            }
        }

        public void ReagendarTVCitaMedica(int id_tv_cita,ATENCION_CITA cita, short id_centro, ATENCION_MEDICA atencion_medica=null)
        {
            try
            {
                using (TransactionScope transaccion = new TransactionScope(TransactionScopeOption.RequiresNew, new TransactionOptions() { IsolationLevel = System.Transactions.IsolationLevel.ReadCommitted }))
                {
                    var id_cita = GetIDProceso<int>("ATENCION_CITA","ID_CITA",string.Format("ID_CENTRO_UBI={0}",id_centro));
                    cita.ID_CITA = id_cita;
                    Context.ATENCION_CITA.Add(cita);
                    Context.SaveChanges();
                    if (atencion_medica!=null)
                    {
                        var id_atencion_medica = GetIDProceso<int>("ATENCION_MEDICA","ID_ATENCION_MEDICA",string.Format("ID_CENTRO_UBI={0}",id_centro));
                        atencion_medica.ID_ATENCION_MEDICA = id_atencion_medica;
                        foreach(var proc_at_med in atencion_medica.PROC_ATENCION_MEDICA)
                        {
                            var id_proc_prog = 1;
                            proc_at_med.ID_ATENCION_MEDICA = id_atencion_medica;
                            foreach (var proc_prog in proc_at_med.PROC_ATENCION_MEDICA_PROG)
                            {
                                proc_prog.ID_ATENCION_MEDICA = id_atencion_medica;
                                proc_prog.ID_AM_PROG = id_proc_prog;
                                proc_prog.ID_CITA = id_cita;
                                foreach (var detalle in proc_prog.PROC_MEDICO_PROG_DET)
                                {
                                    detalle.ID_AM_PROG = id_proc_prog;
                                    detalle.ID_ATENCION_MEDICA = id_atencion_medica;
                                }
                                id_proc_prog += 1;
                            }
                        }
                        Context.ATENCION_MEDICA.Add(atencion_medica);
                    }
                    Context.SaveChanges();
                    var _tv_cita_medica = Context.TV_CITA_MEDICA.Where(w => w.ID_TV_CITA == id_tv_cita && w.ID_CENTRO_UBI == id_centro).FirstOrDefault();
                    if (_tv_cita_medica == null)
                        throw new Exception("No se pudo encontrar el registro en la tabla TV_CITA_MEDICA");
                    _tv_cita_medica.ID_TV_MEDICO_ESTATUS = "RP";
                    Context.SaveChanges();
                    transaccion.Complete();
                }
            }
            catch (Exception ex)
            {
                throw new ApplicationException(ex.Message + " " + (ex.InnerException != null ? ex.InnerException.InnerException.Message : ""));
            }
        }

        public void CancelarTV_Cita_Medica(int id_tv_cita, short id_centro)
        {
            try
            {
                var _tv_cita_medica = Context.TV_CITA_MEDICA.Where(w => w.ID_TV_CITA == id_tv_cita && w.ID_CENTRO_UBI == id_centro).FirstOrDefault();
                if (_tv_cita_medica == null)
                    throw new Exception("No se pudo encontrar el registro en la tabla TV_CITA_MEDICA");
                _tv_cita_medica.ID_TV_MEDICO_ESTATUS = "CA";
                Context.SaveChanges();
            }
            catch (Exception ex)
            {
                throw new ApplicationException(ex.Message + " " + (ex.InnerException != null ? ex.InnerException.InnerException.Message : ""));
            }
        }

        public void RecrearTVCanalizaciones(ATENCION_MEDICA atencion_medica, short id_centro, int id_tv_canalizacion)
        {
            try
            {
                using (TransactionScope transaccion = new TransactionScope(TransactionScopeOption.RequiresNew, new TransactionOptions() { IsolationLevel = System.Transactions.IsolationLevel.ReadCommitted }))
                {
                    var id_atencion_medica = GetIDProceso<int>("ATENCION_MEDICA", "ID_ATENCION_MEDICA", string.Format("ID_CENTRO_UBI={0}", id_centro));
                    atencion_medica.ID_ATENCION_MEDICA = id_atencion_medica;
                    atencion_medica.NOTA_MEDICA.ID_ATENCION_MEDICA = id_atencion_medica;
                    atencion_medica.NOTA_MEDICA.CANALIZACION.ID_ATENCION_MEDICA = id_atencion_medica;
                    if (atencion_medica.NOTA_MEDICA.CANALIZACION.CANALIZACION_ESPECIALIDAD != null && atencion_medica.NOTA_MEDICA.CANALIZACION.CANALIZACION_ESPECIALIDAD.Count() > 0)
                        foreach (var can_esp in atencion_medica.NOTA_MEDICA.CANALIZACION.CANALIZACION_ESPECIALIDAD)
                            can_esp.ID_ATENCION_MEDICA = id_atencion_medica;
                    if (atencion_medica.NOTA_MEDICA.CANALIZACION.CANALIZACION_SERV_AUX != null && atencion_medica.NOTA_MEDICA.CANALIZACION.CANALIZACION_SERV_AUX.Count() > 0)
                        foreach (var can_serv_aux in atencion_medica.NOTA_MEDICA.CANALIZACION.CANALIZACION_SERV_AUX)
                            can_serv_aux.ID_ATENCION_MEDICA = id_atencion_medica;
                    Context.ATENCION_MEDICA.Add(atencion_medica);
                    var _tv_canalizacion = Context.TV_CANALIZACION.Where(w => w.ID_TV_CANALIZACION == id_tv_canalizacion && w.ID_CENTRO_UBI == id_centro).FirstOrDefault();
                    if (_tv_canalizacion == null)
                        throw new Exception("No se pudo encontrar el registro en la tabla TV_CANALIZACION");
                    _tv_canalizacion.ID_TV_MEDICO_ESTATUS = "RP";
                    Context.SaveChanges();
                    transaccion.Complete();
                }
            }
            catch (Exception ex)
            {
                throw new ApplicationException(ex.Message + " " + (ex.InnerException != null ? ex.InnerException.InnerException.Message : ""));
            }
        }

        public void CancelarTV_CANALIZACION(int id_tv_canalizacion, short id_centro)
        {
            try
            {
                var _tv_canalizacion = Context.TV_CANALIZACION.Where(w => w.ID_TV_CANALIZACION == id_tv_canalizacion && w.ID_CENTRO_UBI == id_centro).FirstOrDefault();
                if (_tv_canalizacion == null)
                    throw new Exception("No se pudo encontrar el registro en la tabla TV_CITA_MEDICA");
                _tv_canalizacion.ID_TV_MEDICO_ESTATUS = "CA";
                Context.SaveChanges();
            }
            catch (Exception ex)
            {
                throw new ApplicationException(ex.Message + " " + (ex.InnerException != null ? ex.InnerException.InnerException.Message : ""));
            }
        }

        public void RecrearTVInterconsulta(ATENCION_MEDICA atencion_medica, short id_centro, int id_tv_interconsulta)
        {
            try
            {
                using (TransactionScope transaccion = new TransactionScope(TransactionScopeOption.RequiresNew, new TransactionOptions() { IsolationLevel = System.Transactions.IsolationLevel.Serializable }))
                {
                    var id_atencion_medica = GetIDProceso<int>("ATENCION_MEDICA", "ID_ATENCION_MEDICA", string.Format("ID_CENTRO_UBI={0}", id_centro));
                    atencion_medica.ID_ATENCION_MEDICA = id_atencion_medica;
                    atencion_medica.NOTA_MEDICA.ID_ATENCION_MEDICA = id_atencion_medica;
                    atencion_medica.NOTA_MEDICA.CANALIZACION.ID_ATENCION_MEDICA = id_atencion_medica;
                    if (atencion_medica.NOTA_MEDICA.CANALIZACION.CANALIZACION_ESPECIALIDAD != null && atencion_medica.NOTA_MEDICA.CANALIZACION.CANALIZACION_ESPECIALIDAD.Count() > 0)
                        foreach (var can_esp in atencion_medica.NOTA_MEDICA.CANALIZACION.CANALIZACION_ESPECIALIDAD)
                            can_esp.ID_ATENCION_MEDICA = id_atencion_medica;
                    if (atencion_medica.NOTA_MEDICA.CANALIZACION.CANALIZACION_SERV_AUX != null && atencion_medica.NOTA_MEDICA.CANALIZACION.CANALIZACION_SERV_AUX.Count() > 0)
                        foreach (var can_serv_aux in atencion_medica.NOTA_MEDICA.CANALIZACION.CANALIZACION_SERV_AUX)
                            can_serv_aux.ID_ATENCION_MEDICA = id_atencion_medica;
                    if (atencion_medica.NOTA_MEDICA.CANALIZACION.INTERCONSULTA_SOLICITUD!=null)
                    {
                        var _inter_sol = atencion_medica.NOTA_MEDICA.CANALIZACION.INTERCONSULTA_SOLICITUD.First();
                        var id_inter_sol = GetIDProceso<int>("INTERCONSULTA_SOLICITUD", "ID_INTERSOL", string.Format("ID_CENTRO_UBI={0}", id_centro));
                        _inter_sol.ID_INTERSOL = id_inter_sol;
                        _inter_sol.ID_NOTA_MEDICA = id_atencion_medica;
                        if (_inter_sol.SERVICIO_AUX_INTERCONSULTA != null && _inter_sol.SERVICIO_AUX_INTERCONSULTA.Count() > 0)
                            foreach (var item_serv_aux in _inter_sol.SERVICIO_AUX_INTERCONSULTA)
                                _inter_sol.ID_INTERSOL = id_inter_sol;
                        if (_inter_sol.HOJA_REFERENCIA_MEDICA != null && _inter_sol.HOJA_REFERENCIA_MEDICA.Count() > 0)
                        {
                            var _hoja_ref = _inter_sol.HOJA_REFERENCIA_MEDICA.First();
                            var id_hoja = GetIDProceso<int>("HOJA_REFERENCIA_MEDICA", "ID_HOJA", string.Format("ID_CENTRO_UBI={0}", id_centro));
                            _hoja_ref.ID_HOJA = id_hoja;
                            _hoja_ref.ID_INTERSOL = id_inter_sol;
                        }
                        if (_inter_sol.SOL_INTERCONSULTA_INTERNA != null && _inter_sol.SOL_INTERCONSULTA_INTERNA.Count() > 0)
                        {
                            var _sol_inter = _inter_sol.SOL_INTERCONSULTA_INTERNA.First();
                            var id_solicitud = GetIDProceso<int>("SOL_INTERCONSULTA_INTERNA", "ID_SOLICITUD", string.Format("ID_CENTRO_UBI={0}", id_centro));
                            _sol_inter.ID_SOLICITUD = id_solicitud;
                            _sol_inter.ID_INTERSOL = id_inter_sol;
                        }
                    }
                    Context.ATENCION_MEDICA.Add(atencion_medica);
                    var _tv_interconsulta = Context.TV_INTERCONSULTA_SOLICITUD.Where(w => w.ID_TV_INTERSOL == id_tv_interconsulta && w.ID_CENTRO_UBI == id_centro).FirstOrDefault();
                    if (_tv_interconsulta == null)
                        throw new Exception("No se pudo encontrar el registro en la tabla TV_INTERCONSULTA");
                    _tv_interconsulta.ID_TV_MEDICO_ESTATUS = "RP";
                    Context.SaveChanges();
                    transaccion.Complete();
                }
            }
            catch (Exception ex)
            {
                throw new ApplicationException(ex.Message + " " + (ex.InnerException != null ? ex.InnerException.InnerException.Message : ""));
            }
        }

        public void CancelarTV_Interconsulta(int id_tv_interconsulta, short id_centro)
        {
            try
            {
                var _tv_interconsulta = Context.TV_INTERCONSULTA_SOLICITUD.Where(w => w.ID_TV_INTERSOL == id_tv_interconsulta && w.ID_CENTRO_UBI == id_centro).FirstOrDefault();
                if (_tv_interconsulta == null)
                    throw new Exception("No se pudo encontrar el registro en la tabla TV_INTERCONSULTA");
                _tv_interconsulta.ID_TV_MEDICO_ESTATUS = "CA";
                Context.SaveChanges();
            }
            catch (Exception ex)
            {
                throw new ApplicationException(ex.Message + " " + (ex.InnerException != null ? ex.InnerException.InnerException.Message : ""));
            }
        }
    
    }

    public class TRASLADO_VIRTUAL_INGRESO
    {
        public int ID_CENTRO { get; set; }
        public int ID_ANIO { get; set; }
        public int ID_IMPUTADO { get; set; }
        public int ID_INGRESO { get; set; }
        private string nombre;
        public string NOMBRE
        {
            get { return nombre; }
            set { nombre = value; }
        }
        private string paterno;
        public string PATERNO
        {
            get { return paterno; }
            set { paterno = value; }
        }
        private string materno;
        public string MATERNO
        {
            get { return materno; }
            set { materno = value; }
        }
        public string CENTRO_ORIGEN { get; set; }
        public string FECHA_EGRESO { get; set; }
        public string NOMBRE_COMPLETO
        {
            get 
            {
                StringBuilder _strbuilder = new StringBuilder();
                if (!string.IsNullOrWhiteSpace(PATERNO))
                    _strbuilder.Append(PATERNO.Trim()).Append(" ");
                if (!string.IsNullOrWhiteSpace(MATERNO))
                    _strbuilder.Append(MATERNO.Trim()).Append(" ");
                _strbuilder.Append(NOMBRE.Trim());
                return _strbuilder.ToString();
            }
        }
    }
}
