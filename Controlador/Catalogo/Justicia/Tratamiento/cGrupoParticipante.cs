using SSP.Modelo;
using SSP.Servidor;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SSP.Controlador.Catalogo.Justicia
{
    public class cGrupoParticipante : EntityManagerServer<GRUPO_PARTICIPANTE>
    {
        public T ObtenerConsecutivo<T>(short IdCentro, short IdTipoPrograma, short IdActividad) where T : struct
        {
            var max = GetData().Where(w => w.ID_CENTRO == IdCentro && w.ID_TIPO_PROGRAMA == IdTipoPrograma && w.ID_ACTIVIDAD == IdActividad).Max(m => (short?)(new { m.CENTRO, m.ID_TIPO_PROGRAMA, m.ID_ACTIVIDAD, m.ID_CONSEC }.ID_CONSEC));
            return (T)Convert.ChangeType(max.HasValue ? ++max : 1, typeof(T));
        }

        public bool InsertarParticipante(GRUPO_PARTICIPANTE Entity)
        {
            Entity.ID_CONSEC = ObtenerConsecutivo<short>(Entity.ID_CENTRO, Entity.ID_TIPO_PROGRAMA, Entity.ID_ACTIVIDAD);

            return Insert(Entity);
        }
        

        public GRUPO_PARTICIPANTE ObtenerParticipante(short Id_Anio,short Id_Centro,short Id_Imputado,short Id_Grupo)
        {
            return GetData(w => w.ING_ID_ANIO == Id_Anio &&
                w.ING_ID_CENTRO == Id_Centro &&
                w.ING_ID_IMPUTADO == Id_Imputado &&
                w.ID_GRUPO == Id_Grupo
                ).FirstOrDefault();
        }

        public GRUPO_PARTICIPANTE InsertarParticipanteAsistencia(GRUPO_PARTICIPANTE Entity)
        {
            Entity.ID_CONSEC = ObtenerConsecutivo<short>(Entity.ID_CENTRO, Entity.ID_TIPO_PROGRAMA, Entity.ID_ACTIVIDAD);
            Insert(Entity);
            return Entity;
        }

        public GRUPO_PARTICIPANTE ActualizarParticipanteAsistencia(GRUPO_PARTICIPANTE Entity)
        {
            Update(Entity);
            return Entity;
        }

        public IQueryable<GRUPO_PARTICIPANTE> ObtenerTodos(short Centro,short Edificio,short Sector,DateTime FechaHora)
        {
            try
            {

                return  (from ga in Context.GRUPO_ASISTENCIA
                        from gh in Context.GRUPO_HORARIO.Where(w => ga.ID_CENTRO == w.ID_CENTRO && ga.ID_ACTIVIDAD == w.ID_ACTIVIDAD && ga.ID_TIPO_PROGRAMA == w.ID_TIPO_PROGRAMA && ga.ID_GRUPO == w.ID_GRUPO)
                        from g in Context.GRUPO.Where(w => gh.ID_CENTRO == w.ID_CENTRO && gh.ID_TIPO_PROGRAMA == w.ID_TIPO_PROGRAMA && gh.ID_ACTIVIDAD == w.ID_ACTIVIDAD && gh.ID_GRUPO == w.ID_GRUPO)
                        from gp in Context.GRUPO_PARTICIPANTE.Where(w => g.ID_CENTRO == w.ID_CENTRO && g.ID_ACTIVIDAD == w.ID_ACTIVIDAD && g.ID_TIPO_PROGRAMA == w.ID_TIPO_PROGRAMA && g.ID_GRUPO == w.ID_GRUPO)
                        from i in Context.INGRESO.Where(w => gp.ING_ID_CENTRO == w.ID_CENTRO && gp.ING_ID_ANIO == w.ID_ANIO && gp.ING_ID_IMPUTADO == w.ID_IMPUTADO && gp.ING_ID_INGRESO == w.ID_INGRESO)
                        where
                        ga.ESTATUS == 1 && gh.ESTATUS == 1 && g.ID_ESTATUS_GRUPO == 1 && gp.ESTATUS == 2 && (i.ID_ESTATUS_ADMINISTRATIVO == 1 || i.ID_ESTATUS_ADMINISTRATIVO == 2 || i.ID_ESTATUS_ADMINISTRATIVO == 3 || i.ID_ESTATUS_ADMINISTRATIVO == 8)
                        && i.ID_UB_CENTRO == 4 && i.ID_UB_EDIFICIO == 1 && i.ID_UB_SECTOR == 1
                        && (ga.EMPALME == 0 || (ga.EMPALME > 0 && ga.EMP_COORDINACION == 1) || (ga.EMPALME > 0 && ga.EMP_COORDINACION == 2 && ga.EMP_APROBADO == 1)) 
                        && gh.HORA_INICIO >= FechaHora
                        select gp).Distinct();
                            
                    /*
                     select distinct gp.* from ssp.grupo_asistencia ga 
                    inner join ssp.grupo_horario gh on ga.id_centro = gh.id_centro and ga.id_actividad = gh.id_actividad and ga.id_tipo_programa = gh.id_tipo_programa and ga.id_grupo = gh.id_grupo
                    inner join ssp.grupo g on gh.id_centro = g.id_centro and gh.id_tipo_programa = g.id_tipo_programa and gh.id_actividad = g.id_actividad and gh.id_grupo = g.id_grupo
                    inner join ssp.grupo_participante gp on g.id_centro = gp.id_centro and g.id_actividad = gp.id_actividad and g.id_tipo_programa = gp.id_tipo_programa and g.id_grupo = gp.id_grupo
                    inner join ssp.ingreso i on gp.ing_id_centro = i.id_centro and gp.ing_id_anio = i.id_anio and gp.ing_id_imputado = i.id_imputado and gp.ing_id_ingreso = i.id_ingreso 
                    where ga.estatus = 1 and gh.estatus = 1 and g.id_estatus_grupo = 1 and gp.estatus = 2 and i.id_estatus_administrativo in (1,2,3,8) 
                    and i.id_ub_centro = 4 and i.id_ub_edificio = 1 and i.id_ub_sector = 1 
                    and (ga.empalme = 0 or (ga.empalme > 0 and ga.emp_coordinacion = 1) or (ga.empalme > 0 and ga.emp_coordinacion = 2 and ga.emp_aprobado = 1))
                    and gh.hora_inicio >= sysdate
                    order by ga.fec_registro;
                     */
          
            }
            catch (Exception ex)
            {
                throw new ApplicationException(ex.Message);
            }
        }

        public IEnumerable<cKardexReporte> ObtenerReporteKardex(short Centro, short Anio, int Imputado, short Ingreso)
        {
            try
            {
                string query = "SELECT " +
                "GP.ID_CONSEC,I.ID_CENTRO,I.ID_ANIO,I.ID_IMPUTADO,I.ID_INGRESO, " +
                "D.ID_DEPARTAMENTO,D.DESCR DEPARTAMENTO,GP.ID_TIPO_PROGRAMA,TP.NOMBRE TIPO_PROGRAMA,GPE.DESCR ESTATUS,E.ID_EJE,E.DESCR EJE, " +
                "G.DESCR AS GRUPO,G.FEC_INICIO,G.FEC_FIN,G.RECURRENCIA,A.ID_ACTIVIDAD,A.DESCR ACTIVIDAD,G.ID_GRUPO, "+//,GH.HORA_INICIO,GH.HORA_TERMINO " +
                "GP.FEC_REGISTRO,NTE.DESCR CALIFICACION "+
                "FROM  " +
                "SSP.INGRESO I " +
                "INNER JOIN SSP.GRUPO_PARTICIPANTE GP ON I.ID_CENTRO = GP.ID_CENTRO AND I.ID_ANIO = GP.ING_ID_ANIO AND I.ID_IMPUTADO = GP.ING_ID_IMPUTADO AND I.ID_INGRESO = GP.ING_ID_INGRESO " +
                "INNER JOIN SSP.ACTIVIDAD A ON GP.ID_ACTIVIDAD = A.ID_ACTIVIDAD AND GP.ID_TIPO_PROGRAMA = A.ID_TIPO_PROGRAMA " +
                "INNER JOIN SSP.GRUPO_PARTICIPANTE_ESTATUS GPE ON GP.ESTATUS = GPE.ID_ESTATUS " +
                "INNER JOIN SSP.EJE E ON GP.EJE = E.ID_EJE " +
                "LEFT JOIN SSP.GRUPO G ON GP.ID_CENTRO = G.ID_CENTRO AND GP.ID_ACTIVIDAD = G.ID_ACTIVIDAD AND GP.ID_TIPO_PROGRAMA = G.ID_TIPO_PROGRAMA AND GP.ID_GRUPO = G.ID_GRUPO " +
                //"LEFT JOIN SSP.GRUPO_HORARIO GH ON G.ID_CENTRO = GH.ID_CENTRO AND G.ID_ACTIVIDAD = GH.ID_ACTIVIDAD AND G.ID_TIPO_PROGRAMA = GH.ID_TIPO_PROGRAMA AND G.ID_GRUPO = GH.ID_GRUPO " +
                "LEFT JOIN SSP.TIPO_PROGRAMA TP ON GP.ID_TIPO_PROGRAMA = TP.ID_TIPO_PROGRAMA " +
                "LEFT JOIN SSP.DEPARTAMENTO D ON TP.ID_DEPARTAMENTO = D.ID_DEPARTAMENTO " +
                "LEFT JOIN SSP.NOTA_TECNICA NT ON GP.ID_CENTRO = NT.ID_CENTRO AND GP.ID_TIPO_PROGRAMA = NT.ID_TIPO_PROGRAMA AND GP.ID_ACTIVIDAD = NT.ID_ACTIVIDAD AND GP.ID_GRUPO = NT.ID_GRUPO AND GP.ID_CONSEC = NT.ID_CONSEC "+
                "LEFT JOIN SSP.NOTA_TECNICA_ESTATUS NTE ON NT.ESTATUS = NTE.ID_ESTATUS "+
                "WHERE I.ID_CENTRO = {0} AND I.ID_ANIO = {1} AND I.ID_IMPUTADO = {2} AND I.ID_INGRESO = {3} ORDER BY E.ID_EJE,GP.FEC_REGISTRO";
                query = string.Format(query, Centro, Anio, Imputado, Ingreso);
                return Context.Database.SqlQuery<cKardexReporte>(query);

            }
            catch (Exception ex)
            {
                throw new ApplicationException(ex.Message);
            }
        }

        public IEnumerable<cKardexReporteHorario> ObtenerReporteKardexHorario(short Centro,short Anio,int Imputado,short Ingreso)
        {
            try
            {
                string query = "SELECT T.*,GH.ID_GRUPO_HORARIO,GH.HORA_INICIO,GH.HORA_TERMINO " +
                "FROM  " +
                "( " +
                "SELECT  " +
                "G.ID_CENTRO,G.ID_TIPO_PROGRAMA,G.ID_ACTIVIDAD,G.ID_GRUPO, " +
                "T.ID_DIA,T.DIA " +
                "FROM " +
                "SSP.GRUPO G , " +
                "TABLE(SSP.FUNC_RECURRENCIA(G.RECURRENCIA)) T " +
                ") T " +
                "INNER JOIN SSP.GRUPO_HORARIO GH ON T.ID_CENTRO = GH.ID_CENTRO AND T.ID_TIPO_PROGRAMA = GH.ID_TIPO_PROGRAMA AND T.ID_ACTIVIDAD = GH.ID_ACTIVIDAD AND T.ID_GRUPO = GH.ID_GRUPO AND T.ID_DIA = GH.ID_GRUPO_HORARIO " +
                "INNER JOIN SSP.GRUPO_PARTICIPANTE GP ON GH.ID_CENTRO = GP.ID_CENTRO AND GH.ID_TIPO_PROGRAMA = GP.ID_TIPO_PROGRAMA AND GH.ID_ACTIVIDAD = GP.ID_ACTIVIDAD AND GH.ID_GRUPO = GP.ID_GRUPO " +
                "WHERE GP.ING_ID_CENTRO = {0} AND GP.ING_ID_ANIO = {1} AND GP.ING_ID_IMPUTADO = {2} AND GP.ING_ID_INGRESO = {3} " +
                "ORDER BY T.ID_CENTRO,T.ID_TIPO_PROGRAMA,T.ID_ACTIVIDAD,T.ID_GRUPO,GH.ID_GRUPO_HORARIO ";
                query = string.Format(query, Centro, Anio, Imputado, Ingreso);
                return Context.Database.SqlQuery<cKardexReporteHorario>(query);

            }
            catch (Exception ex)
            {
                throw new ApplicationException(ex.Message);
            }
        }

        public IEnumerable<cReporteGrupoParticipante> ObtenerReporteProgramasActividades(short Centro)
        {
            try
            {
                StringBuilder query = new StringBuilder();
                query.Append("SELECT ");
                query.Append("G.ID_TIPO_PROGRAMA, ");
                query.Append("G.ID_ACTIVIDAD, ");
                query.Append("TP.DESCR TIPO_PROGRAMA, ");
                query.Append("A.DESCR ACTIVIDAD,  ");
                query.Append("COUNT(CASE WHEN IM.SEXO = 'F' THEN 1 END) FEMENINO, ");
                query.Append("COUNT(CASE WHEN IM.SEXO = 'M' THEN 1 END) MASCULINO, ");
                query.Append("COUNT(CASE WHEN IM.SEXO IS NULL OR IM.SEXO NOT IN ('F','M') THEN 1 END) NO_DEFINIDO, ");
                query.Append("COUNT(G.ID_GRUPO) TOTAL ");
                query.Append("FROM  ");
                query.Append("SSP.GRUPO G ");
                query.Append("INNER JOIN SSP.GRUPO_PARTICIPANTE GP ON G.ID_CENTRO = GP.ID_CENTRO AND  G.ID_ACTIVIDAD = GP.ID_ACTIVIDAD AND G.ID_TIPO_PROGRAMA = GP.ID_TIPO_PROGRAMA AND G.ID_GRUPO = GP.ID_GRUPO ");
                query.Append("INNER JOIN SSP.INGRESO I ON GP.ING_ID_CENTRO = I.ID_CENTRO AND GP.ING_ID_ANIO = I.ID_ANIO AND GP.ING_ID_IMPUTADO = I.ID_IMPUTADO AND GP.ING_ID_INGRESO = I.ID_INGRESO ");
                query.Append("INNER JOIN SSP.IMPUTADO IM ON I.ID_CENTRO = IM.ID_CENTRO AND I.ID_ANIO = IM.ID_ANIO AND I.ID_IMPUTADO = IM.ID_IMPUTADO ");
                query.Append("INNER JOIN SSP.ACTIVIDAD A ON G.ID_TIPO_PROGRAMA = A.ID_TIPO_PROGRAMA AND G.ID_ACTIVIDAD = A.ID_ACTIVIDAD ");
                query.Append("INNER JOIN SSP.TIPO_PROGRAMA TP ON A.ID_TIPO_PROGRAMA = TP.ID_TIPO_PROGRAMA ");
                query.AppendFormat("WHERE G.ID_CENTRO = {0} AND G.ID_ESTATUS_GRUPO = 1 AND GP.ESTATUS = 2 AND I.ID_UB_CENTRO = 4 AND I.ID_ESTATUS_ADMINISTRATIVO IN (1,2,3,7) ", Centro);
                query.Append("GROUP BY  ");
                query.Append("G.ID_TIPO_PROGRAMA,G.ID_ACTIVIDAD,TP.DESCR,A.DESCR ");
                query.Append("ORDER BY  ");
                query.Append("G.ID_TIPO_PROGRAMA, ");
                query.Append("G.ID_ACTIVIDAD ");
                return Context.Database.SqlQuery<cReporteGrupoParticipante>(query.ToString());
            }
            catch (Exception ex)
            { 
                throw new ApplicationException(ex.Message); 
            }
        }

    }


    public class cKardexReporte
    { 
        public int ID_CONSEC { get; set; }
        public short ID_CENTRO { get; set; }
        public short ID_ANIO { get; set; }
        public int ID_IMPUTADO { get; set; }
        public short ID_INGRESO { get; set; }
        public int? ID_DEPARTAMENTO { get; set; }
        public string DEPARTAMENTO { get; set; }
        public int? ID_TIPO_PROGRAMA { get; set; }
        public string TIPO_PROGRAMA { get; set; }
        public string ESTATUS { get; set; }
        public int? ID_EJE { get; set; }
        public string EJE { get; set; }
        public string GRUPO { get; set; }
        public DateTime? FEC_INICIO { get; set; }
        public DateTime? FEC_FIN { get; set; }
        public string RECURRENCIA { get; set; }
        //public DateTime? HORA_INICIO { get; set; }
        //public DateTime? HORA_TERMINO { get; set; }
        public int? ID_ACTIVIDAD { get; set; }
        public string ACTIVIDAD { get; set; }
        public int? ID_GRUPO { get; set; }
        public DateTime? FEC_REGISTRO { get; set; }
        public string CALIFICACION { get; set; }
    }

    public class cKardexReporteHorario
    {
        public int ID_CENTRO { get; set; }
        public int ID_TIPO_PROGRAMA { get; set; }
        public int ID_ACTIVIDAD { get; set; }
        public int ID_GRUPO { get; set; }
        public int ID_DIA { get; set; }
        public string DIA { get; set; }
        public int ID_GRUPO_HORARIO { get; set; }
        public DateTime? HORA_INICIO { get; set; }
        public DateTime? HORA_TERMINO { get; set; }
    }

    public class cReporteGrupoParticipante
    {
        public short ID_TIPO_PROGRAMA { get; set; }
        public short ID_ACTIVIDAD { get; set; }
        public string TIPO_PROGRAMA { get; set; }
        public string ACTIVIDAD { get; set; }
        public short FEMENINO { get; set; }
        public short MASCULINO { get; set; }
        public short NO_DEFINIDO { get; set; }
        public short TOTAL { get; set; }
    }
}
