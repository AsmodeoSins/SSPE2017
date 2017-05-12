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
using SSP.Servidor.ModelosExtendidos;
using System.Data.Objects;
using System.Text;


namespace SSP.Controlador.Catalogo.Justicia
{
    public class cIngreso : EntityManagerServer<INGRESO>
    {
        /// <summary>
        /// constructor de la clase
        /// </summary>
        public cIngreso()
        { }

        /// <summary>
        /// metodo que conecta con la base de datos para extraer todos los registros
        /// </summary>
        /// <returns>listado de tipo "INGRESO"</returns>
        public IQueryable<INGRESO> ObtenerTodos()
        {
            try
            {
                return GetData();
            }
            catch (Exception ex)
            {
                throw new ApplicationException(ex.Message);
            }
        }

        /// <summary>
        /// metodo que conecta con la base de datos para extraer todos los registros
        /// </summary>
        /// <returns>listado de tipo "INGRESO"</returns>
        public IQueryable<INGRESO> ObtenerTodosPorFecha(bool BusquedaMensual, int? Mes = null, DateTime? FechaServer = null, DateTime? FechaInicial = null, DateTime? FechaFinal = null)
        {
            try
            {
                if (BusquedaMensual)
                {
                    return GetData(g =>
                        (g.FEC_INGRESO_CERESO.Value.Month == Mes &&
                         g.FEC_INGRESO_CERESO.Value.Year == FechaServer.Value.Year) ||
                        ((g.FEC_INGRESO_CERESO.Value.Month == Mes &&
                        g.FEC_INGRESO_CERESO.Value.Year == FechaServer.Value.Year) &&
                         (g.FEC_SALIDA_CERESO.HasValue &&
                         g.FEC_SALIDA_CERESO.Value.Month == Mes &&
                         g.FEC_SALIDA_CERESO.Value.Year == FechaServer.Value.Year)));
                }
                else
                {
                    return GetData(g =>
                        (g.FEC_INGRESO_CERESO.Value >= FechaInicial.Value &&
                        g.FEC_INGRESO_CERESO.Value <= FechaFinal.Value) ||
                        ((g.FEC_INGRESO_CERESO.Value >= FechaInicial.Value &&
                        g.FEC_INGRESO_CERESO.Value <= FechaFinal.Value) &&
                        (g.FEC_SALIDA_CERESO.HasValue &&
                        g.FEC_INGRESO_CERESO.Value >= FechaInicial.Value &&
                        g.FEC_INGRESO_CERESO.Value <= FechaFinal.Value)));
                }
            }
            catch (Exception ex)
            {
                throw new ApplicationException(ex.Message);
            }
        }

        /// <summary>
        /// Metodo que regresa listado de internos dados de alta o baja
        /// </summary>
        /// <param name="FecInicio"></param>
        /// <param name="FecFin"></param>
        /// <param name="Tipo"></param>
        /// <returns></returns>
        public IQueryable<INGRESO> ObtenerAltasBajas(DateTime? FecInicio = null, DateTime? FecFin = null, int Tipo = 1)
        {
            try
            {
                var predicate = PredicateBuilder.True<INGRESO>();
                if (Tipo == 1)//Altas
                {
                    if (FecInicio.HasValue)
                        predicate = predicate.And(w => EntityFunctions.TruncateTime(w.FEC_INGRESO_CERESO) >= FecInicio);
                    if (FecFin.HasValue)
                        predicate = predicate.And(w => EntityFunctions.TruncateTime(w.FEC_INGRESO_CERESO) <= FecFin);
                    return GetData(predicate.Expand()).OrderBy(w => w.FEC_INGRESO_CERESO);
                }
                else//Bajas
                {
                    predicate = predicate.And(w => w.FEC_SALIDA_CERESO != null);
                    if (FecInicio.HasValue)
                        predicate = predicate.And(w => EntityFunctions.TruncateTime(w.FEC_SALIDA_CERESO) >= FecInicio);
                    if (FecFin.HasValue)
                        predicate = predicate.And(w => EntityFunctions.TruncateTime(w.FEC_SALIDA_CERESO) <= FecFin);
                    return GetData(predicate.Expand()).OrderBy(w => w.FEC_SALIDA_CERESO);
                }
                
            }
            catch (Exception ex)
            {
                throw new ApplicationException(ex.Message);
            }
        }
        /// <summary>
        /// metodo que conecta con la base de datos para extraer todos los registros que coincidan con el nombre que se busca
        /// </summary>
        /// <returns>listado de tipo "INGRESO"</returns>
        public ObservableCollection<INGRESO> ObtenerTodos(short Centro, string Nombre = "", string Paterno = "", string Materno = "", int Estatus = 0)
        {
            ObservableCollection<INGRESO> imputados;
            var Resultado = new List<INGRESO>();
            try
            {
                if (Estatus == 0)
                    Resultado = GetData().Where(w => w.ID_CENTRO == Centro && w.IMPUTADO.NOMBRE.Contains(Nombre) && w.IMPUTADO.PATERNO.Contains(Paterno) && w.IMPUTADO.MATERNO.Contains(Materno)).ToList();
                else
                    Resultado = GetData().Where(w => w.ID_CENTRO == Centro && w.IMPUTADO.NOMBRE.Contains(Nombre) && w.IMPUTADO.PATERNO.Contains(Paterno) && w.IMPUTADO.MATERNO.Contains(Materno) && w.ID_ESTATUS_ADMINISTRATIVO != Estatus).ToList();
                imputados = new ObservableCollection<INGRESO>(Resultado);
            }
            catch (Exception ex)
            {
                throw new ApplicationException(ex.Message);
            }
            return imputados;
        }

        public INGRESO ObtenerPorNIP(string NIP)
        {
            try
            {
                return GetData(g =>
                    g.IMPUTADO.NIP.TrimEnd() == NIP).
                    OrderByDescending(o =>
                    o.ID_INGRESO).
                    FirstOrDefault();
            }
            catch (Exception ex)
            {
                throw new ApplicationException(ex.Message);
            }
        }


        public IQueryable<INGRESO> ObtenerTodos(short Centro, string Nombre = "", string Paterno = "", string Materno = "", bool Estatus = true, int Pagina = 1)
        {
            try
            {
                var predicate = PredicateBuilder.True<INGRESO>();
                if (!string.IsNullOrEmpty(Nombre))
                    predicate = predicate.And(w => w.IMPUTADO.NOMBRE.Contains(Nombre));
                if (!string.IsNullOrEmpty(Paterno))
                    predicate = predicate.And(w => w.IMPUTADO.PATERNO.Contains(Paterno));
                if (!string.IsNullOrEmpty(Materno))
                    predicate = predicate.And(w => w.IMPUTADO.MATERNO.Contains(Materno));
                if (Estatus)
                {
                    int[] estatus = { 1, 2, 3, 8 };
                    predicate = predicate.And(w => estatus.Count(x => x == w.ID_ESTATUS_ADMINISTRATIVO) > 0);
                }
                return GetData(predicate.Expand()).OrderBy(w => new { w.ID_CENTRO, w.ID_ANIO, w.ID_IMPUTADO }).Take((Pagina * 30)).Skip((Pagina == 1 ? 0 : ((Pagina * 30) - 30)));

            }
            catch (Exception ex)
            {
                throw new ApplicationException(ex.Message);
            }
        }


        public IQueryable<INGRESO> ObtenerTodosReporte(short Centro, int Id_Anio = 0, int Id_Imputado = 0, string Nombre = "", string Paterno = "", string Materno = "")
        {
            try
            {
                var predicate = PredicateBuilder.True<INGRESO>();
                predicate = predicate.And(a => a.ID_UB_CENTRO.HasValue && a.ID_UB_CENTRO == Centro);
                if (Id_Anio != 0)
                    predicate = predicate.And(a => a.ID_ANIO == Id_Anio);
                if (Id_Imputado != 0)
                    predicate = predicate.And(a => a.ID_IMPUTADO == Id_Imputado);
                if (!string.IsNullOrEmpty(Nombre))
                    predicate = predicate.And(a => a.IMPUTADO.NOMBRE.Contains(Nombre));
                if (!string.IsNullOrEmpty(Paterno))
                    predicate = predicate.And(a => a.IMPUTADO.NOMBRE.Contains(Paterno));
                if (!string.IsNullOrEmpty(Materno))
                    predicate = predicate.And(a => a.IMPUTADO.NOMBRE.Contains(Materno));
                return GetData(predicate.Expand());
            }
            catch (Exception ex)
            {

                throw new ApplicationException(ex.Message);
            }
        }

        public IQueryable<INGRESO> ObtenerIngresosEstancias(List<CELDA> Estancias)
        {
            var predicate = PredicateBuilder.False<INGRESO>();
            foreach (var Estancia in Estancias)
            {
                var LIBERADO = 4;
                var TRASLADADO = 5;
                var SUJETO_A_PROCESO_EN_LIBERTAD = 6;
                var DISCRECIONAL = 7;
                predicate = predicate.Or(x =>
                    (x.ID_ESTATUS_ADMINISTRATIVO != LIBERADO &&
                    x.ID_ESTATUS_ADMINISTRATIVO != TRASLADADO &&
                    x.ID_ESTATUS_ADMINISTRATIVO != SUJETO_A_PROCESO_EN_LIBERTAD &&
                    x.ID_ESTATUS_ADMINISTRATIVO != DISCRECIONAL) &&
                    x.ID_UB_CENTRO == Estancia.ID_CENTRO &&
                    x.ID_UB_EDIFICIO == Estancia.ID_EDIFICIO &&
                    x.ID_UB_SECTOR == Estancia.ID_SECTOR &&
                    x.ID_UB_CELDA == Estancia.ID_CELDA &&
                    x.CAMA != null);
            }
            return GetData(predicate.Expand());
        }


        public IQueryable<INGRESO> ObtenerTodos(short? Centro = null, short? Anio = null, int? Folio = null, string Nombre = "", string Paterno = "", string Materno = "", int Pagina = 1)
        {
            try
            {
                var predicate = PredicateBuilder.True<INGRESO>();
                predicate = predicate.And(w => w.ID_ESTATUS_ADMINISTRATIVO != 4);
                if (Centro != null)
                    predicate = predicate.And(w => w.ID_UB_CENTRO == Centro);
                if (Anio != null)
                    predicate = predicate.And(w => w.ID_ANIO == Anio);
                if (Folio != null)
                    predicate = predicate.And(w => w.ID_IMPUTADO == Folio);
                if (!string.IsNullOrEmpty(Paterno))
                    predicate = predicate.And(w => w.IMPUTADO.PATERNO.Contains(Paterno));
                if (!string.IsNullOrEmpty(Materno))
                    predicate = predicate.And(w => w.IMPUTADO.MATERNO.Contains(Materno));
                if (!string.IsNullOrEmpty(Nombre))
                    predicate = predicate.And(w => w.IMPUTADO.NOMBRE.Contains(Nombre));
                return GetData(predicate.Expand()).OrderBy(w => new { w.ID_CENTRO, w.ID_ANIO, w.ID_IMPUTADO }).Take((Pagina * 30)).Skip((Pagina == 1 ? 0 : ((Pagina * 30) - 30)));
            }
            catch (Exception ex)
            {
                throw new ApplicationException(ex.Message);
            }

        }

        public IQueryable<INGRESO> ObtenerTodosActivos(short? Centro = null, short? Anio = null, int? Folio = null, string Nombre = "", string Paterno = "", string Materno = "", int Pagina = 1)
        {
            try
            {
                var predicate = PredicateBuilder.True<INGRESO>();
                predicate = predicate.And(w => w.ID_ESTATUS_ADMINISTRATIVO != 4 && w.ID_ESTATUS_ADMINISTRATIVO != 5 && w.ID_ESTATUS_ADMINISTRATIVO != 6 && w.ID_ESTATUS_ADMINISTRATIVO != 7);
                if (Centro != null)
                    predicate = predicate.And(w => w.ID_UB_CENTRO == Centro);
                if (Anio != null)
                    predicate = predicate.And(w => w.ID_ANIO == Anio);
                if (Folio != null)
                    predicate = predicate.And(w => w.ID_IMPUTADO == Folio);
                if (!string.IsNullOrEmpty(Paterno))
                    predicate = predicate.And(w => w.IMPUTADO.PATERNO.Contains(Paterno));
                if (!string.IsNullOrEmpty(Materno))
                    predicate = predicate.And(w => w.IMPUTADO.MATERNO.Contains(Materno));
                if (!string.IsNullOrEmpty(Nombre))
                    predicate = predicate.And(w => w.IMPUTADO.NOMBRE.Contains(Nombre));
                return GetData(predicate.Expand()).OrderBy(w => new { w.ID_CENTRO, w.ID_ANIO, w.ID_IMPUTADO }).Take((Pagina * 30)).Skip((Pagina == 1 ? 0 : ((Pagina * 30) - 30)));
            }
            catch (Exception ex)
            {
                throw new ApplicationException(ex.Message);
            }

        }
        /// <summary>
        /// metodo que se conectara a la base de datos para regresar un registro 
        /// </summary>
        /// <param name="Id">valor de la llave primaria para obtener registro</param>
        /// <returns>objeto de tipo "INGRESO"</returns>
        public IQueryable<INGRESO> ObtenerIngresosAnteriores(short Id_centro, short Id_anio, int Id_imputado, short Id_ingreso)
        {
            try
            {
                return GetData().Where(w => w.ID_CENTRO == Id_centro && w.ID_ANIO == Id_anio && w.ID_IMPUTADO == Id_imputado && w.ID_INGRESO < Id_ingreso);
            }
            catch (Exception ex)
            {
                throw new ApplicationException(ex.Message);
            }
        }

        /// <summary>
        /// metodo que se conectara a la base de datos para regresar un registro 
        /// </summary>
        /// <param name="Id">valor de la llave primaria para obtener registro</param>
        /// <returns>objeto de tipo "INGRESO"</returns>
        public IQueryable<INGRESO> ObtenerIngresosActivos(short? Centro = null, short? Edificio = null, short? Sector = null, bool ConCausaPenal = false)
        {
            try
            {
                var predicate = PredicateBuilder.True<INGRESO>();
                if (Centro.HasValue)
                    predicate = predicate.And(w => w.ID_UB_CENTRO == Centro);
                if (Edificio.HasValue)
                    predicate = predicate.And(w => w.ID_UB_EDIFICIO == Edificio);
                if (Sector.HasValue)
                    predicate = predicate.And(w => w.ID_UB_SECTOR == Sector);
                /*  1	EN AREA TEMPORAL
                    2	EN CLASIFICACION
                    3	ASIGNADO A CELDA
                    4	LIBERADO
                    5	TRASLADADO
                    6	SUJETO A PROCESO EN LIBERTAD
                    7	DISCRECIONAL
                    8	INDICIADOS*/
                predicate = predicate.And(w => w.CAMA != null);
                int[] estatus = { 1, 2, 3, 8 };
                predicate = predicate.And(w => estatus.Contains(w.ID_ESTATUS_ADMINISTRATIVO.Value));
                if (ConCausaPenal)
                    predicate = predicate.And(w => w.CAUSA_PENAL.Count > 0);
                return GetData(predicate.Expand()).OrderBy(w => new { w.ID_CENTRO, w.ID_ANIO, w.ID_IMPUTADO });
            }
            catch (Exception ex)
            {
                throw new ApplicationException(ex.Message);
            }
        }


        public IQueryable<INGRESO> ObtenerIngresosActivosCausaPenal(short? Centro = null,string Fuero = "")
        {
            try
            {
                var predicate = PredicateBuilder.True<INGRESO>();
                if (Centro.HasValue)
                    predicate = predicate.And(w => w.ID_UB_CENTRO == Centro);
                /*  1	EN AREA TEMPORAL
                    2	EN CLASIFICACION
                    3	ASIGNADO A CELDA
                    4	LIBERADO
                    5	TRASLADADO
                    6	SUJETO A PROCESO EN LIBERTAD
                    7	DISCRECIONAL
                    8	INDICIADOS*/
                int[] estatus = { 1, 2, 3, 8 };
                predicate = predicate.And(w => estatus.Contains(w.ID_ESTATUS_ADMINISTRATIVO.Value));
                int[] estatusCP = { 0,1,6 };
                if(!string.IsNullOrEmpty(Fuero))
                    predicate = predicate.And(w => w.CAUSA_PENAL.Any(x => estatusCP.Contains(x.ID_ESTATUS_CP.Value) && x.CP_FUERO == Fuero));
                else
                    predicate = predicate.And(w => w.CAUSA_PENAL.Any(x => estatusCP.Contains(x.ID_ESTATUS_CP.Value)));
                return GetData(predicate.Expand()).OrderBy(w => new { w.ID_CENTRO, w.ID_ANIO, w.ID_IMPUTADO });
            }
            catch (Exception ex)
            {
                throw new ApplicationException(ex.Message);
            }
        }

        public IQueryable<INGRESO> ObtenerIngresosActivosPorFecha(short? Centro = null, DateTime? FechaInicial = null, DateTime? FechaFinal = null)
        {
            try
            {
                var predicate = PredicateBuilder.True<INGRESO>();
                if (Centro.HasValue)

                    /*  1	EN AREA TEMPORAL
                        2	EN CLASIFICACION
                        3	ASIGNADO A CELDA
                        4	LIBERADO
                        5	TRASLADADO
                        6	SUJETO A PROCESO EN LIBERTAD
                        7	DISCRECIONAL
                        8	INDICIADOS*/
                    predicate = predicate.And(w => w.CAMA != null);
                int[] estatus = { 1, 2, 3, 8 };
                predicate = predicate.And(w => estatus.Contains(w.ID_ESTATUS_ADMINISTRATIVO.Value));
                if (FechaFinal.HasValue && FechaFinal.HasValue)
                    predicate = predicate.And(w =>
                        w.FEC_INGRESO_CERESO.HasValue &&
                        w.FEC_INGRESO_CERESO.Value >= FechaInicial.Value &&
                        w.FEC_INGRESO_CERESO.Value <= FechaFinal.Value);
                return GetData(predicate.Expand()).OrderBy(w => new { w.ID_CENTRO, w.ID_ANIO, w.ID_IMPUTADO });
            }
            catch (Exception ex)
            {
                throw new ApplicationException(ex.Message);
            }
        }


        public IQueryable<INGRESO> ObtenerIngresoActivosBusqueda(string busqueda, short? Centro = null)
        {
            try
            {
                var predicate = PredicateBuilder.True<INGRESO>();
                if (Centro != null)
                    predicate = predicate.And(w => w.ID_UB_CENTRO == Centro);
                predicate = predicate.And(w => w.ID_ESTATUS_ADMINISTRATIVO != 4 &&
                    (w.IMPUTADO.NOMBRE.Contains(busqueda) || w.IMPUTADO.PATERNO.Contains(busqueda) || w.IMPUTADO.MATERNO.Contains(busqueda)) &&
                    w.GRUPO_PARTICIPANTE.Count > 0
                    );
                return GetData(predicate.Expand());
            }
            catch (Exception ex)
            {
                throw new ApplicationException(ex.Message);
            }
        }


        public IQueryable<INGRESO> ObtenerIngresosActivosListadoGeneral(short[] ESTATUS_CAUSA_PENAL, string CLASIFICACION_JURIDICA = "", string FUERO = "", short? ID_CENTRO = null)
        {
            try
            {
                var predicate = PredicateBuilder.True<INGRESO>();
                /*  1	EN AREA TEMPORAL
                       2	EN CLASIFICACION
                       3	ASIGNADO A CELDA
                       4	LIBERADO
                       5	TRASLADADO
                       6	SUJETO A PROCESO EN LIBERTAD
                       7	DISCRECIONAL
                       8	INDICIADOS*/
                int[] estatus = { 1, 2, 3, 8 };
                predicate = predicate.And(w => estatus.Contains(w.ID_ESTATUS_ADMINISTRATIVO.Value));
                if (ID_CENTRO.HasValue)
                    predicate = predicate.And(a =>
                        a.ID_UB_CENTRO.Value == ID_CENTRO);

                if (!string.IsNullOrEmpty(CLASIFICACION_JURIDICA))
                    predicate = predicate.And(a =>
                        a.ID_CLASIFICACION_JURIDICA != null &&
                        a.ID_CLASIFICACION_JURIDICA != string.Empty &&
                        a.ID_CLASIFICACION_JURIDICA.TrimEnd() == CLASIFICACION_JURIDICA);

                if (!string.IsNullOrEmpty(FUERO))
                {

                    predicate = predicate.And(a =>

                        a.CAUSA_PENAL.Any(aCP =>
                            aCP.ID_ESTATUS_CP.HasValue &&
                            ESTATUS_CAUSA_PENAL.Contains(aCP.ID_ESTATUS_CP.Value)) != null &&

                            a.CAUSA_PENAL.Where(aF =>
                            aF.ID_ESTATUS_CP.HasValue &&
                            ESTATUS_CAUSA_PENAL.Contains(aF.ID_ESTATUS_CP.Value) &&
                            aF.CP_FUERO == FUERO).Any());
                }
                return GetData(predicate.Expand());


            }
            catch (Exception ex)
            {

                throw new ApplicationException(ex.Message);
            }
        }


        public IQueryable<INGRESO> ObtenerIngresosActivosConAdicciones(short? ID_CENTRO = null, string SEXO = "")
        {
            try
            {
                var predicate = PredicateBuilder.True<INGRESO>();
                /*  1	EN AREA TEMPORAL
                       2	EN CLASIFICACION
                       3	ASIGNADO A CELDA
                       4	LIBERADO
                       5	TRASLADADO
                       6	SUJETO A PROCESO EN LIBERTAD
                       7	DISCRECIONAL
                       8	INDICIADOS*/
                int[] estatus = { 1, 2, 3, 8 };
                predicate = predicate.And(w => estatus.Contains(w.ID_ESTATUS_ADMINISTRATIVO.Value));
                if (ID_CENTRO.HasValue)
                    predicate = predicate.And(a =>
                        a.ID_UB_CENTRO.Value == ID_CENTRO);

                if (!string.IsNullOrEmpty(SEXO))
                    predicate = predicate.And(a =>
                        a.IMPUTADO != null &&
                        a.IMPUTADO.SEXO == SEXO);

                predicate.And(a =>
                    a.EMI_INGRESO != null);


                return GetData(predicate.Expand());
            }
            catch (Exception ex)
            {

                throw new ApplicationException(ex.Message);
            }
        }



        public IQueryable<INGRESO> ObtenerIngresosConDiscapacidad(int TIPO_DISCAPACIDAD, short? ID_CENTRO = null, string SEXO = "")
        {
            try
            {
                var predicate = PredicateBuilder.True<INGRESO>();
                /*  1	EN AREA TEMPORAL
                       2	EN CLASIFICACION
                       3	ASIGNADO A CELDA
                       4	LIBERADO
                       5	TRASLADADO
                       6	SUJETO A PROCESO EN LIBERTAD
                       7	DISCRECIONAL
                       8	INDICIADOS*/
                int[] estatus = { 1, 2, 3, 8 };
                predicate = predicate.And(w => estatus.Contains(w.ID_ESTATUS_ADMINISTRATIVO.Value));
                if (ID_CENTRO.HasValue)
                    predicate = predicate.And(a =>
                        a.ID_UB_CENTRO.Value == ID_CENTRO);

                if (!string.IsNullOrEmpty(SEXO))
                    predicate = predicate.And(a =>
                        a.IMPUTADO != null &&
                        a.IMPUTADO.SEXO == SEXO);

                predicate.And(a =>
                    a.EMI_INGRESO != null &&
                    a.EMI_INGRESO.Any(w =>
                    w.EMI.EMI_ENFERMEDAD != null &&
                    w.EMI.EMI_ENFERMEDAD.DISCAPACIDAD == "S" &&
                    w.EMI.EMI_ENFERMEDAD.ID_TIPO_DISCAPACIDAD.HasValue &&
                    w.EMI.EMI_ENFERMEDAD.ID_TIPO_DISCAPACIDAD.Value == TIPO_DISCAPACIDAD));


                return GetData(predicate.Expand());
            }
            catch (Exception ex)
            {

                throw new ApplicationException(ex.Message);
            }
        }

        public IQueryable<INGRESO> ObtenerEtniasReporte(short? Centro, string Fuero = "", string Clasificacion = "", string Sexo = "")
        {
            try
            {
                var predicate = PredicateBuilder.True<INGRESO>();
                if (Centro != null)
                    predicate = predicate.And(w => w.ID_UB_CENTRO == Centro);

                if (!string.IsNullOrEmpty(Fuero))
                    predicate = predicate.And(x => x.CAUSA_PENAL.Any(y => y.CP_FUERO.Trim() == Fuero));

                int[] estatus = { 1, 2, 3, 8 };
                predicate = predicate.And(w => estatus.Contains(w.ID_ESTATUS_ADMINISTRATIVO.Value));

                if (!string.IsNullOrEmpty(Clasificacion))
                    predicate = predicate.And(x => x.ID_CLASIFICACION_JURIDICA.Trim() == Clasificacion.Trim());

                if (!string.IsNullOrEmpty(Sexo))
                    predicate = predicate.And(x => x.IMPUTADO.SEXO == Sexo);

                return GetData(predicate.Expand());
            }

            catch (Exception exc)
            {
                throw exc;
            }
        }

        public IQueryable<INGRESO> ObtenerIngresosTerceraEdad(string _Clasificacion = "", string Fuero = "", string Sexo = "", short? Centro = null)
        {
            try
            {
                var predicate = PredicateBuilder.True<INGRESO>();
                if (Centro != null)
                    predicate = predicate.And(w => w.ID_UB_CENTRO == Centro);

                int[] estatus = { 1, 2, 3, 8 };
                predicate = predicate.And(w => estatus.Contains(w.ID_ESTATUS_ADMINISTRATIVO.Value));
                if (!string.IsNullOrEmpty(_Clasificacion))
                    predicate = predicate.And(x => x.ID_CLASIFICACION_JURIDICA.Trim() == _Clasificacion.Trim());
                if (!string.IsNullOrEmpty(Sexo))
                    predicate = predicate.And(x => x.IMPUTADO.SEXO == Sexo);

                return GetData(predicate.Expand());

            }
            catch (Exception exc)
            {
                throw new ApplicationException(exc.Message);
            }
        }

        public IQueryable<INGRESO> ObtenerIngresosActivos(int Centro, short?[] EstatusAdmvo, short programacompl, short actividadcompl, short? delitocompl, short? planimetriacompl, int? edadinicio, int? edadfinal, short? SelectAnio, short? GrupoSelect, int Pagina = 1)
        {
            IQueryable<INGRESO> imputados = null;
            try
            {
                var context = new SSPEntidades();
                var predicate = PredicateBuilder.True<INGRESO>();
                context.Conexion();

                predicate = predicate.And(w => w.ID_UB_CENTRO == Centro);
                if (delitocompl.HasValue)
                    if (delitocompl.Value > 0)
                        predicate = predicate.And(w => (w.ID_INGRESO_DELITO == delitocompl && w.ID_INGRESO_DELITO != null));
                if (planimetriacompl.HasValue)
                    if (planimetriacompl > 0)
                        predicate = predicate.And(w => w.CAMA.SECTOR_OBSERVACION_CELDA.SECTOR_OBSERVACION.ID_SECTOR_CLAS == planimetriacompl);
                if (edadinicio.HasValue && edadfinal.HasValue)
                    predicate = predicate.And(w => w.IMPUTADO.NACIMIENTO_FECHA != null ? (w.IMPUTADO.NACIMIENTO_FECHA.Value.Year >= edadfinal.Value && w.IMPUTADO.NACIMIENTO_FECHA.Value.Year < edadinicio.Value) : w.IMPUTADO.NACIMIENTO_FECHA != null);
                if (SelectAnio.HasValue)
                    predicate = predicate.And(w => w.ID_ANIO == SelectAnio);

                if (!GrupoSelect.HasValue)
                {
                    #region [Nuevo Grupo]

                    predicate = predicate.And(w => !EstatusAdmvo.Contains(w.ID_ESTATUS_ADMINISTRATIVO) && !context.GRUPO_PARTICIPANTE.Where(wh => wh.ID_CENTRO == w.ID_CENTRO && wh.ID_TIPO_PROGRAMA == programacompl && wh.ID_ACTIVIDAD == actividadcompl && wh.ID_GRUPO != null).Select(s =>
                        new ingresoPrograma
                        {
                            ING_ID_CENTRO = s.ING_ID_CENTRO,
                            ING_ID_ANIO = s.ING_ID_ANIO,
                            ING_ID_IMPUTADO = s.ING_ID_IMPUTADO,
                            ING_ID_INGRESO = s.ING_ID_INGRESO
                        }).Contains(new ingresoPrograma
                        {
                            ING_ID_CENTRO = w.ID_CENTRO,
                            ING_ID_ANIO = w.ID_ANIO,
                            ING_ID_IMPUTADO = w.ID_IMPUTADO,
                            ING_ID_INGRESO = w.ID_INGRESO
                        }));

                    imputados = context.INGRESO.AsExpandable().Where(predicate)
                        .OrderByDescending(o => o.GRUPO_PARTICIPANTE.Where(w => w.ID_ACTIVIDAD == actividadcompl && w.ID_TIPO_PROGRAMA == programacompl).Select(s => s.ID_ACTIVIDAD).FirstOrDefault())
                        .ThenByDescending(o => o.CAUSA_PENAL.Where(w => (w.ID_ESTATUS_CP == 1 || w.ID_ESTATUS_CP == 0 || w.ID_ESTATUS_CP == 6)).Any())
                        .ThenBy(o => o.ID_ANIO)
                        .ThenBy(o => o.ID_IMPUTADO)
                        .Take((Pagina * 30))
                        .Skip((Pagina == 1 ? 0 : ((Pagina * 30) - 30))); ;
                    #endregion
                }
                else
                {
                    #region [editar Grupo]
                    predicate = predicate.And(w => !EstatusAdmvo.Contains(w.ID_ESTATUS_ADMINISTRATIVO) && !context.GRUPO_PARTICIPANTE.Where(wh => wh.ID_CENTRO == w.ID_CENTRO && wh.ID_TIPO_PROGRAMA == programacompl && wh.ID_ACTIVIDAD == actividadcompl && !(wh.ID_GRUPO != GrupoSelect) && (wh.GRUPO_PARTICIPANTE_CANCELADO.Where(whe => (whe.ID_CENTRO == wh.ID_CENTRO && whe.ID_TIPO_PROGRAMA == wh.ID_TIPO_PROGRAMA && whe.ID_ACTIVIDAD == wh.ID_ACTIVIDAD && whe.ID_CONSEC == wh.ID_CONSEC && whe.ID_GRUPO == wh.ID_GRUPO)).Any() ?

                            !wh.GRUPO_PARTICIPANTE_CANCELADO.Where(whe => (whe.ID_CENTRO == wh.ID_CENTRO && whe.ID_TIPO_PROGRAMA == wh.ID_TIPO_PROGRAMA && whe.ID_ACTIVIDAD == wh.ID_ACTIVIDAD && whe.ID_CONSEC == wh.ID_CONSEC) &&
                                (whe.RESPUESTA_FEC == null ?

                                (whe.RESPUESTA_FEC == null && whe.ESTATUS == 0) :

                                (wh.GRUPO_PARTICIPANTE_CANCELADO.Where(wher => (wher.ID_CENTRO == wh.ID_CENTRO && wher.ID_TIPO_PROGRAMA == wh.ID_TIPO_PROGRAMA && wher.ID_ACTIVIDAD == wh.ID_ACTIVIDAD && wher.ID_CONSEC == wh.ID_CONSEC) && wher.RESPUESTA_FEC != null && (wher.ESTATUS == 0 || wher.ESTATUS == 2)).Count() == (wh.GRUPO_PARTICIPANTE_CANCELADO.Where(wher => (wher.ID_CENTRO == wh.ID_CENTRO && wher.ID_TIPO_PROGRAMA == wh.ID_TIPO_PROGRAMA && wher.ID_ACTIVIDAD == wh.ID_ACTIVIDAD && wher.ID_CONSEC == wh.ID_CONSEC)).Count() - wh.GRUPO_PARTICIPANTE_CANCELADO.Where(wher => (wher.ID_CENTRO == wh.ID_CENTRO && wher.ID_TIPO_PROGRAMA == wh.ID_TIPO_PROGRAMA && wher.ID_ACTIVIDAD == wh.ID_ACTIVIDAD && wher.ID_CONSEC == wh.ID_CONSEC) && wher.RESPUESTA_FEC != null && wher.ESTATUS == 1 && wher.ID_ESTATUS == 3).Count())))).Any()


                                : false)).Select(s =>
                         new ingresoPrograma
                         {
                             ING_ID_CENTRO = s.ING_ID_CENTRO,
                             ING_ID_ANIO = s.ING_ID_ANIO,
                             ING_ID_IMPUTADO = s.ING_ID_IMPUTADO,
                             ING_ID_INGRESO = s.ING_ID_INGRESO
                         }).Contains(new ingresoPrograma
                         {
                             ING_ID_CENTRO = w.ID_CENTRO,
                             ING_ID_ANIO = w.ID_ANIO,
                             ING_ID_IMPUTADO = w.ID_IMPUTADO,
                             ING_ID_INGRESO = w.ID_INGRESO
                         }));

                    imputados = context.INGRESO
                    .AsExpandable().Where(predicate)
                    .OrderByDescending(o => o.GRUPO_PARTICIPANTE.Where(w => w.ID_ACTIVIDAD == actividadcompl && w.ID_TIPO_PROGRAMA == programacompl).Select(s => s.ID_ACTIVIDAD).FirstOrDefault())
                    .ThenByDescending(o => o.CAUSA_PENAL.Where(w => (w.ID_ESTATUS_CP == 1 || w.ID_ESTATUS_CP == 0 || w.ID_ESTATUS_CP == 6)).Any())
                    .ThenBy(o => o.ID_ANIO)
                    .ThenBy(o => o.ID_IMPUTADO)
                    .Take((Pagina * 30))
                    .Skip((Pagina == 1 ? 0 : ((Pagina * 30) - 30)));
                    #endregion
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return imputados;
        }

        /// <summary>
        /// metodo que se conectara a la base de datos para regresar los ingresos por sector.
        /// </summary>
        /// <param name="Id">valor de la llave primaria para obtener registro</param>
        /// <returns>objeto de tipo "INGRESO"</returns>
        public IQueryable<INGRESO> ObtenerIngresosPorSector(short Centro = 0, short Edificio = 0, short Sector = 0, int? SectorObservacion = null)
        {
            try
            {
                var predicate = PredicateBuilder.True<INGRESO>();
                int[] estatus = { 1, 2, 3, 8 };
                predicate = predicate.And(w => estatus.Contains(w.ID_ESTATUS_ADMINISTRATIVO.Value) && w.ID_UB_CENTRO == Centro && w.ID_UB_EDIFICIO == Edificio && w.ID_UB_SECTOR == Sector);
                if (SectorObservacion.HasValue)
                    predicate = predicate.And(w => w.CAMA.SECTOR_OBSERVACION_CELDA.ID_SECTOR_OBS == SectorObservacion);
                return GetData(predicate.Expand()).OrderBy(w => w.CAMA.CELDA.ID_CELDA).ThenBy(w => w.CAMA.ID_CAMA);
            }
            catch (Exception ex)
            {
                throw new ApplicationException(ex.Message);
            }
        }

        /// <summary>
        /// metodo que se conectara a la base de datos para regresar los ingresos por sector 
        /// </summary>
        /// <param name="Id">valor de la llave primaria para obtener registro</param>
        /// <returns>objeto de tipo "INGRESO"</returns>
        public IQueryable<INGRESO> ObtenerIngresosPorSectorYarda(short Centro = 0, short? Edificio = 0, short? Sector = 0, short? CeldaInicio = 0, short? CeldaFin = 0)
        {
            try
            {
                return GetData().Where(w => w.ID_UB_CENTRO == Centro && w.ID_UB_EDIFICIO == Edificio && w.ID_UB_SECTOR == Sector && SSPEntidades.ParseDouble(w.ID_UB_CELDA) >= CeldaInicio && SSPEntidades.ParseDouble(w.ID_UB_CELDA) <= CeldaFin && (w.ID_ESTATUS_ADMINISTRATIVO == 1 || w.ID_ESTATUS_ADMINISTRATIVO == 2 || w.ID_ESTATUS_ADMINISTRATIVO == 3 || w.ID_ESTATUS_ADMINISTRATIVO == 8)).OrderBy(w => w.CAMA.CELDA.ID_CELDA).ThenBy(w => w.CAMA.ID_CAMA);
            }
            catch (Exception ex)
            {
                throw new ApplicationException(ex.Message);
            }
        }
        public IQueryable<INGRESO> ObtenerIngresosPorJuzgado(short? Pais, short? Estado, short? Municipio, short? juzgado, string Fuero)
        {
            try
            {
                var res = GetData().Where(w => w.CAUSA_PENAL.Where(wh => wh.CP_PAIS_JUZGADO == Pais && wh.CP_ESTADO_JUZGADO == Estado && wh.CP_MUNICIPIO_JUZGADO == Municipio &&
                        wh.CP_JUZGADO == juzgado && wh.CP_FUERO == Fuero).Any());
                return res;
            }
            catch (Exception ex)
            {
                throw new ApplicationException(ex.Message);
            }
        }

        public IQueryable<INGRESO> ObtenerIngresosActivosFiltrados(short?[] estatus_inactivos, int? anio = null, int? folio = null, string nombre = "", string paterno = "", string materno = "", short? Centro = null, int Pagina = 1)
        {
            try
            {
                var predicate = PredicateBuilder.True<INGRESO>();
                if (Centro != null)
                    predicate = predicate.And(w => w.ID_UB_CENTRO == Centro);
                if (anio.HasValue)
                    predicate = predicate.And(w => w.ID_ANIO == anio);
                if (folio.HasValue)
                    predicate = predicate.And(w => w.ID_IMPUTADO == folio);
                if (!string.IsNullOrWhiteSpace(nombre))
                    predicate = predicate.And(w => w.IMPUTADO.NOMBRE.Contains(nombre));
                if (!string.IsNullOrWhiteSpace(paterno))
                    predicate = predicate.And(w => w.IMPUTADO.PATERNO.Trim().Contains(paterno));
                if (!string.IsNullOrWhiteSpace(materno))
                    predicate = predicate.And(w => w.IMPUTADO.MATERNO.Trim().Contains(materno));
                predicate = predicate.And(w => !estatus_inactivos.Contains(w.ID_ESTATUS_ADMINISTRATIVO));
                predicate = predicate.And(w => !w.TRASLADO_DETALLE.Any(a => a.ID_ANIO == w.ID_ANIO && a.ID_CENTRO == w.ID_CENTRO && a.ID_IMPUTADO == w.ID_IMPUTADO
                    && a.ID_INGRESO == w.ID_INGRESO && a.ID_ESTATUS == "PR" && a.TRASLADO.ID_ESTATUS == "PR"));
                return GetData(predicate.Expand())
                .OrderBy(o => o.ID_ANIO)
                .ThenBy(t => t.ID_IMPUTADO)
                .Take((Pagina * 30))
                .Skip((Pagina == 1 ? 0 : ((Pagina * 30) - 30)));
            }
            catch (Exception ex)
            {
                throw new ApplicationException(ex.Message);
            }
        }

        public IQueryable<INGRESO> ObtenerIngresosActivosFiltradosPorEdificio(short? Centro = null, short? Edificio = null, short? Sector = null, int Pagina = 1, int Anio = 0, int Folio = 0, string Nombre = "", string ApellidoPaterno = "", string ApellidoMaterno = "")
        {
            var LIBERADO = 4;
            var TRASLADADO = 5;
            var SUJETO_A_PROCESO_EN_LIBERTAD = 6;
            var DISCRECIONAL = 7;
            try
            {
                var predicate = PredicateBuilder.True<INGRESO>();
                if (Centro != null)
                    predicate = predicate.And(a => a.ID_CENTRO == Centro);
                if (Edificio != null)
                {
                    if (Sector != null)
                        predicate = predicate.And(a => a.ID_UB_EDIFICIO == Edificio && a.ID_UB_SECTOR == Sector);
                    else
                        predicate = predicate.And(a => a.ID_UB_EDIFICIO == Edificio);
                }

                if (Anio != 0)
                    predicate = predicate.And(a => a.ID_ANIO == Anio);
                if (Folio != 0)
                    predicate = predicate.And(a => a.ID_IMPUTADO == Folio);

                if (!string.IsNullOrEmpty(Nombre))
                    predicate = predicate.And(a =>
                        a.IMPUTADO != null &&
                        a.IMPUTADO.NOMBRE.Contains(Nombre));

                if (!string.IsNullOrEmpty(ApellidoPaterno))
                    predicate = predicate.And(a =>
                        a.IMPUTADO != null &&
                        a.IMPUTADO.PATERNO.Contains(ApellidoPaterno));

                if (!string.IsNullOrEmpty(ApellidoMaterno))
                    predicate = predicate.And(a =>
                        a.IMPUTADO != null &&
                        a.IMPUTADO.MATERNO.Contains(ApellidoMaterno));

                predicate = predicate.And(a =>
                    a.ID_ESTATUS_ADMINISTRATIVO != LIBERADO &&
                    a.ID_ESTATUS_ADMINISTRATIVO != TRASLADADO &&
                    a.ID_ESTATUS_ADMINISTRATIVO != SUJETO_A_PROCESO_EN_LIBERTAD &&
                    a.ID_ESTATUS_ADMINISTRATIVO != DISCRECIONAL &&
                    !string.IsNullOrEmpty(a.IMPUTADO.NIP) &&
                    a.CAMA != null &&
                    (a.ID_UB_CENTRO.HasValue &&
                    a.ID_UB_EDIFICIO.HasValue &&
                    a.ID_UB_SECTOR.HasValue &&
                    !string.IsNullOrEmpty(a.ID_UB_CELDA) &&
                    a.ID_UB_CAMA.HasValue));
                return GetData(predicate.Expand())
                .OrderByDescending(oD => oD.ID_ANIO)
                .ThenByDescending(tD => tD.ID_IMPUTADO)
                .Take((Pagina * 30))
                .Skip((Pagina == 1 ? 0 : ((Pagina * 30) - 30)));
            }
            catch (Exception ex)
            {
                throw new ApplicationException(ex.Message);
            }
        }

        /// <summary>
        /// metodo que se conectara a la base de datos para regresar un registro 
        /// </summary>
        /// <param name="Id">valor de la llave primaria para obtener registro</param>
        /// <returns>objeto de tipo "INGRESO"</returns>
        public INGRESO Obtener(short Id_centro, short Id_anio, int Id_imputado, short Id_ingreso)
        {
            try
            {
                return GetData().Where(w => w.ID_CENTRO == Id_centro && w.ID_ANIO == Id_anio && w.ID_IMPUTADO == Id_imputado && w.ID_INGRESO == Id_ingreso).FirstOrDefault();
            }
            catch (Exception ex)
            {
                throw new ApplicationException(ex.Message);
            }
        }

        /// <summary>
        /// metodo que se conectara a la base de datos para regresar un registro 
        /// </summary>
        /// <param name="Id">valor de la llave primaria para obtener el último ingreso del imputado</param>
        /// <returns>objeto de tipo "INGRESO"</returns>
        public INGRESO ObtenerUltimoIngreso(short Id_centro, short Id_anio, int Id_imputado)
        {
            INGRESO Resultado = null;
            try
            {
                Resultado = GetData(g =>
                     g.ID_CENTRO == Id_centro &&
                     g.ID_ANIO == Id_anio &&
                     g.ID_IMPUTADO == Id_imputado &&
                     !string.IsNullOrEmpty(g.IMPUTADO.NIP)).
                     OrderByDescending(o => o.ID_INGRESO).
                     FirstOrDefault();
            }
            catch (Exception ex)
            {

                throw new ApplicationException(ex.Message);
            }
            return Resultado;

        }

        public INGRESO ObtenerUltimoIngresoReporte(short Id_centro, short Id_anio, int Id_imputado)
        {
            INGRESO Resultado = null;
            try
            {
                Resultado = GetData(g =>
                     g.ID_CENTRO == Id_centro &&
                     g.ID_ANIO == Id_anio &&
                     g.ID_IMPUTADO == Id_imputado).
                     OrderByDescending(o => o.ID_INGRESO).
                     FirstOrDefault();
            }
            catch (Exception ex)
            {

                throw new ApplicationException(ex.Message);
            }
            return Resultado;

        }

        /// <summary>
        /// metodo que se conectara a la base de datos para regresar una lista
        /// </summary>
        /// <param name="nombre">Nombre a filtrar en los imputados de acuerdo a los ingresos ligados</param>
        /// <param name="paterno">Apellido Paterno a filtrar en los imputados de acuerdo a los ingresos ligados</param>
        /// <param name="materno">Apellido Materno a filtrar en los imputados de acuerdo a los ingresos ligados</param>
        /// <param name="Centro">Centro al que pertenecen</param>
        /// <returns>Lista de objetos de tipo "INGRESO"</returns>
        public IQueryable<INGRESO> ObtenerIngresoActivosBusqueda(string nombre = "", string paterno = "", string materno = "", short id_anio = 0, int id_imputado = 0, short? Centro = null)
        {
            try
            {
                var predicate = PredicateBuilder.True<INGRESO>();
                if (Centro != null)
                    predicate = predicate.And(w => w.ID_UB_CENTRO == Centro);

                predicate = predicate.And(w => w.ID_ESTATUS_ADMINISTRATIVO != 4 &&
                    (w.IMPUTADO.NOMBRE.Contains(nombre) && w.IMPUTADO.PATERNO.Contains(paterno) && w.IMPUTADO.MATERNO.Contains(materno)) &&
                    w.GRUPO_PARTICIPANTE.Count > 0);

                if (id_anio != 0)
                    predicate = predicate.And(w => w.ID_ANIO == id_anio);

                if (id_imputado != 0)
                    predicate = predicate.And(w => w.ID_IMPUTADO == id_imputado);

                return GetData(predicate.Expand());
            }
            catch (Exception ex)
            {
                throw new ApplicationException(ex.Message);
            }
        }



        /// <summary>
        /// metodo que se conectara a la base de datos para regresar una lista
        /// </summary>
        /// <param name="Id_Grupo">Id del grupo de la actividad a la que pertenecen</param>
        /// <param name="Inicio">Fecha de inicio del grupo de la actividad a la que pertenecen</param>
        /// <param name="Final">Fecha de termino del grupo de la actividad a la que pertenecen</param>
        /// <param name="Centro">Centro al que pertenecen</param>
        /// <returns>Lista de objetos de tipo "INGRESO"</returns>
        public IQueryable<INGRESO> ObtenerIngresoActivosActividad(int Id_Grupo, DateTime? Inicio, DateTime? Final, short? Centro = null)
        {
            try
            {

                var predicate = PredicateBuilder.True<INGRESO>();
                if (Centro != null)
                    predicate = predicate.And(w => w.ID_UB_CENTRO == Centro);
                predicate = predicate.And(w => w.ID_ESTATUS_ADMINISTRATIVO != 4 &&
                w.GRUPO_PARTICIPANTE.Where(w2 => w2.ID_GRUPO == Id_Grupo).FirstOrDefault().GRUPO.GRUPO_HORARIO.Where(w3 => w3.HORA_INICIO == Inicio && w3.HORA_TERMINO == Final && w3.GRUPO_HORARIO_ESTATUS.ID_ESTATUS == 1).FirstOrDefault().HORA_INICIO == Inicio &&
                w.GRUPO_PARTICIPANTE.Where(w2 => w2.ID_GRUPO == Id_Grupo).FirstOrDefault().GRUPO.GRUPO_HORARIO.Where(w3 => w3.HORA_INICIO == Inicio && w3.HORA_TERMINO == Final && w3.GRUPO_HORARIO_ESTATUS.ID_ESTATUS == 1).FirstOrDefault().HORA_TERMINO == Final);

                return GetData(predicate.Expand());
            }
            catch (Exception ex)
            {
                throw new ApplicationException(ex.Message);
            }
        }


        public INGRESO ObtenerIngresoPorCama(short ID_CENTRO, short? ID_UB_EDIFICIO = 0, short? ID_UB_SECTOR = 0, string ID_UB_CELDA = null, short? ID_UB_CAMA = 0)
        {
            try
            {
                var LIBERADO = 4;
                var TRASLADADO = 5;
                var SUJETO_A_PROCESO_EN_LIBERTAD = 6;
                var DISCRECIONAL = 7;
                var predicate = PredicateBuilder.True<INGRESO>();
                predicate = predicate.And(x => x.ID_CENTRO == ID_CENTRO &&
                    (x.ID_ESTATUS_ADMINISTRATIVO != LIBERADO &&
                    x.ID_ESTATUS_ADMINISTRATIVO != TRASLADADO &&
                    x.ID_ESTATUS_ADMINISTRATIVO != SUJETO_A_PROCESO_EN_LIBERTAD &&
                    x.ID_ESTATUS_ADMINISTRATIVO != DISCRECIONAL));
                if (ID_UB_EDIFICIO != 0)
                    predicate = predicate.And(x => x.ID_UB_EDIFICIO == ID_UB_EDIFICIO);
                if (ID_UB_SECTOR != 0)
                    predicate = predicate.And(x => x.ID_UB_EDIFICIO == ID_UB_EDIFICIO);
                if (!string.IsNullOrEmpty(ID_UB_CELDA))
                    predicate = predicate.And(x => x.ID_UB_CELDA == ID_UB_CELDA);
                if (ID_UB_CAMA != 0)
                    predicate = predicate.And(x => x.ID_UB_CAMA == ID_UB_CAMA);
                return GetData(predicate.Expand()).FirstOrDefault();
            }
            catch (Exception ex)
            {

                throw new ApplicationException(ex.Message);
            }
        }

        /// <summary>
        /// Devuelve los ingresos de uninterno
        /// </summary>
        /// <param name="Id_centro">Centro</param>
        /// <param name="Id_anio">Anio</param>
        /// <param name="Id_imputado">Imputado</param>
        /// <returns>IQuaryable de tipo "INGRESO"</returns>
        public IQueryable<INGRESO> ObtenerIngresos(short? Id_centro = null, short? Id_anio = null, int? Id_imputado = null)
        {
            try
            {
                var predicate = PredicateBuilder.True<INGRESO>();
                if (Id_centro.HasValue)
                    predicate = predicate.And(w => w.ID_CENTRO == Id_centro);
                if (Id_anio.HasValue)
                    predicate = predicate.And(w => w.ID_ANIO == Id_anio);
                if (Id_imputado.HasValue)
                    predicate = predicate.And(w => w.ID_IMPUTADO == Id_imputado);

                return GetData(predicate.Expand()).OrderBy(w => w.ID_INGRESO);
                //return GetData().Where(w => w.ID_CENTRO == Id_centro && w.ID_ANIO == Id_anio && w.ID_IMPUTADO == Id_imputado && w.ID_INGRESO == Id_ingreso).FirstOrDefault();
            }
            catch (Exception ex)
            {
                throw new ApplicationException(ex.Message);
            }
        }

        /// <summary>
        /// Devuelve los ingresos (Alta peligrosidad)
        /// </summary>
        /// <param name="Id_centro">Centro</param>
        /// <param name="FechaInicial">Fecha Inicial</param>
        /// <param name="FechaFinal">Fecha Final</param>
        /// <returns>IQuaryable de tipo "INGRESO"</returns>
        public IQueryable<INGRESO> ObtenerIngresosAltaSeguridad(short? Id_centro = null)
        {
            try
            {
                var predicate = PredicateBuilder.True<INGRESO>();
                predicate = predicate.And(w => w.ID_TIPO_SEGURIDAD == "A" && w.FEC_INGRESO_CERESO.HasValue);
                if (Id_centro.HasValue)
                    predicate = predicate.And(w => w.ID_CENTRO == Id_centro);
                return GetData(predicate.Expand()).OrderBy(w => w.ID_INGRESO);
                //return GetData().Where(w => w.ID_CENTRO == Id_centro && w.ID_ANIO == Id_anio && w.ID_IMPUTADO == Id_imputado && w.ID_INGRESO == Id_ingreso).FirstOrDefault();
            }
            catch (Exception ex)
            {
                throw new ApplicationException(ex.Message);
            }
        }


        /// <summary>
        /// metodo que se conecta a la base de datos para insertar un registro nuevo
        /// </summary>
        /// <param name="Entity">objeto de tipo "INGRESO" con valores a insertar</param>
        public short Insertar(INGRESO Entity)
        {
            try
            {
                Entity.ID_INGRESO = GetIDProceso<short>("INGRESO", "ID_INGRESO", string.Format("ID_ANIO = {0} AND ID_IMPUTADO = {1} AND ID_CENTRO = {2}", Entity.ID_ANIO, Entity.ID_IMPUTADO, Entity.ID_CENTRO));
                    //(short)(ObtenerSecuenciaIngreso(Entity.ID_ANIO, Entity.ID_IMPUTADO, Entity.ID_CENTRO) + 1);
                Insert(Entity);
                return Entity.ID_INGRESO;
            }
            catch (Exception ex)
            {
                throw new ApplicationException(ex.Message);
            }
        }
        private short ObtenerSecuenciaIngreso(short anio, int imputado, short centro)
        {
            short Resultado;
            try
            {
                var ingres = GetData().Where(w => w.ID_CENTRO == centro && w.ID_ANIO == anio && w.ID_IMPUTADO == imputado).OrderByDescending(m => m.ID_INGRESO).FirstOrDefault();
                if (ingres != null)
                    Resultado = ingres.ID_INGRESO;
                else
                    Resultado = 0;
            }
            catch (Exception ex)
            {
                throw new ApplicationException(ex.Message);
            }
            return Resultado;
        }
        /// <summary>
        /// metodo que se conecta a la base de datos para actualizar un registro
        /// </summary>
        /// <param name="Entity">objeto de tipo "INGRESO" con valores a actualizar</param>
        public bool Actualizar(INGRESO Entity)
        {
            try
            {
                if (Update(Entity))
                    return true;
            }
            catch (Exception ex)
            {
                if (ex.Message.Contains("part of the object's key information"))
                    throw new ApplicationException("La llave principal no se puede cambiar");
                else
                    throw new ApplicationException(ex.Message);
            }
            return false;
        }


        public bool ActualizarUbicacion(INGRESO Entity)
        {
            try
            {
                Context.INGRESO.Attach(Entity);
                Context.Entry(Entity).Property(x => x.ID_UB_EDIFICIO).IsModified = true;
                Context.Entry(Entity).Property(x => x.ID_UB_SECTOR).IsModified = true;
                Context.Entry(Entity).Property(x => x.ID_UB_CELDA).IsModified = true;
                Context.Entry(Entity).Property(x => x.ID_UB_CAMA).IsModified = true;
                Context.SaveChanges();
                //if (Update(Entity))
                return true;
            }
            catch (Exception ex)
            {
                throw new ApplicationException(ex.Message);
            }
            return false;
        }

        /// <summary>
        /// Metodo usado para dar libertad a un interno
        /// </summary>
        /// <param name="ingreso">Objeto tipo ingreso</param>
        /// <param name="incidente">Objeto tipo incidente</param>
        /// <param name="biometria">Objeto tipo biometria</param>
        /// <returns>true o false</returns>
        public bool LibertadBiometrica(INGRESO ingreso, INCIDENTE incidente, INGRESO_BIOMETRICO biometria)
        {
            try
            {
                using (TransactionScope transaccion = new TransactionScope(TransactionScopeOption.RequiresNew, new TransactionOptions() { IsolationLevel = System.Transactions.IsolationLevel.ReadCommitted }))
                {
                    //Libertad
                    Context.INGRESO.Attach(ingreso);
                    Context.Entry(ingreso).Property(x => x.ID_ESTATUS_ADMINISTRATIVO).IsModified = true;
                    Context.Entry(ingreso).Property(x => x.FEC_SALIDA_CERESO).IsModified = true;

                    //Incidente
                    if (incidente != null)
                    {
                        incidente.ID_INCIDENTE = GetIDProceso<short>("INCIDENTE", "ID_INCIDENTE", string.Format("ID_CENTRO = {0} AND ID_ANIO = {1} AND ID_IMPUTADO = {2} AND ID_INGRESO = {3}",
                            incidente.ID_CENTRO, incidente.ID_ANIO, incidente.ID_IMPUTADO, incidente.ID_INGRESO));
                        Context.INCIDENTE.Add(incidente);
                    }

                    //Biometria
                    Context.INGRESO_BIOMETRICO.Add(biometria);

                    //Libera Cama
                    var cama = Context.CAMA.Where(w => w.ID_CENTRO == ingreso.ID_UB_CENTRO && w.ID_EDIFICIO == ingreso.ID_UB_EDIFICIO && w.ID_SECTOR == ingreso.ID_UB_SECTOR && w.ID_CELDA == ingreso.ID_UB_CELDA && w.ID_CAMA == ingreso.ID_UB_CAMA).FirstOrDefault();
                    if (cama != null)
                    {
                        cama.ESTATUS = "S";
                        Context.Entry(cama).State = EntityState.Modified;
                    }
                    Context.SaveChanges();
                    transaccion.Complete();
                    return true;
                }
            }
            catch (Exception ex)
            {
                throw new ApplicationException(ex.Message);
            }

        }

        /// <summary>
        /// metodo que se conecta a la base de datos para eliminar un registro
        /// </summary>
        /// <param name="Id">valor de la llave primaria para obtener registro</param>
        /// <returns>"True" para eliminado, "False" para no encontrado</returns>
        public bool Eliminar(short Id_centro, short Id_anio, int Id_imputado, short Id_ingreso)
        {
            try
            {
                var Entity = GetData().Where(w => w.ID_CENTRO == Id_centro && w.ID_ANIO == Id_anio && w.ID_IMPUTADO == Id_imputado && w.ID_INGRESO == Id_ingreso).SingleOrDefault();
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

        public void ActualizarIngresoYCama(INGRESO ingreso, CAMA camaNueva, CAMA camaVieja)
        {
            try
            {
                using (TransactionScope transaccion = new TransactionScope(TransactionScopeOption.RequiresNew, new TransactionOptions() { IsolationLevel = System.Transactions.IsolationLevel.ReadCommitted }))
                {
                    var traslado_detalle = ingreso.TRASLADO_DETALLE.FirstOrDefault();
                    if (traslado_detalle != null)
                    {
                        if (traslado_detalle.TRASLADO.ID_TRASLADO == 0)
                        {
                            var id_traslado = GetSequence<short>("TRASLADO_SEQ");
                            traslado_detalle.TRASLADO.ID_TRASLADO = id_traslado;
                            traslado_detalle.ID_TRASLADO = id_traslado;
                            Context.TRASLADO.Add(traslado_detalle.TRASLADO);
                            Context.TRASLADO_DETALLE.Add(traslado_detalle);
                        }
                        else
                        {
                            Context.Entry(traslado_detalle.TRASLADO).State = EntityState.Modified;
                            Context.Entry(traslado_detalle).State = EntityState.Modified;
                        }
                    }
                    Context.Entry(ingreso).State = EntityState.Modified;
                    Context.Entry(camaNueva).State = EntityState.Modified;
                    if (camaVieja != null)
                        Context.Entry(camaVieja).State = EntityState.Modified;
                    Context.SaveChanges();
                    transaccion.Complete();
                }
            }
            catch (Exception ex)
            {
                throw new ApplicationException(ex.Message + " " + (ex.InnerException != null ? ex.InnerException.InnerException.Message : ""));
            }
        }

        public bool Activo(INGRESO ingreso)
        {
            return (ingreso.ID_ESTATUS_ADMINISTRATIVO == 1 || ingreso.ID_ESTATUS_ADMINISTRATIVO == 2 || ingreso.ID_ESTATUS_ADMINISTRATIVO == 3 || ingreso.ID_ESTATUS_ADMINISTRATIVO == 8);
        }


        /// <summary>
        /// Obtiene los internos que no cuentan con EMI (Paginado).
        /// </summary>
        /// <returns>Listado de los ingresos que no tienen EMI </returns>
        public IQueryable<INGRESO> ObtenerSinEMI(short Centro, int Pagina = 1)
        {
            try
            {
                return GetData().Where(w => w.ID_UB_CENTRO == Centro && w.EMI_INGRESO.Count == 0 &&
                    (w.ID_ESTATUS_ADMINISTRATIVO == 1 || w.ID_ESTATUS_ADMINISTRATIVO == 2 || w.ID_ESTATUS_ADMINISTRATIVO == 3 || w.ID_ESTATUS_ADMINISTRATIVO == 8)
                    ).OrderBy(o => o.ID_ANIO)
                .ThenBy(t => t.ID_IMPUTADO)
                .Take((Pagina * 30))
                .Skip((Pagina == 1 ? 0 : ((Pagina * 30) - 30))); ;
            }
            catch (Exception ex)
            {
                throw new ApplicationException(ex.Message);
            }
        }

        /// <summary>
        /// Obtiene los internos que no cuentan con EMI (Todos).
        /// </summary>
        /// <returns>Listado de los ingresos que no tienen EMI</returns>
        public IQueryable<INGRESO> ObtenerSinEMI(short Centro)
        {
            try
            {
                //{1 = EN AREA TEMPORAL,2 = EN CLASIFICACION,3 = ASIGNADO A CELDA,8	= INDICIADOS}
                return GetData().Where(w => w.ID_UB_CENTRO == Centro && w.EMI_INGRESO.Count == 0 && (w.ID_ESTATUS_ADMINISTRATIVO == 1 || w.ID_ESTATUS_ADMINISTRATIVO == 2 || w.ID_ESTATUS_ADMINISTRATIVO == 3 || w.ID_ESTATUS_ADMINISTRATIVO == 8)).OrderBy(o => o.ID_ANIO).ThenBy(t => t.ID_IMPUTADO);
            }
            catch (Exception ex)
            {
                throw new ApplicationException(ex.Message);
            }
        }

        /// <summary>
        /// Obtiene el total de los internos que no cuentan con EMI.
        /// </summary>
        /// <returns>Numero total de internos sin emi</returns>
        public int ObtenerSinEMITotal(short Centro)
        {
            try
            {
                //{1 = EN AREA TEMPORAL,2 = EN CLASIFICACION,3 = ASIGNADO A CELDA,8	= INDICIADOS}
                return GetData().Count(w => w.ID_UB_CENTRO == Centro && w.EMI_INGRESO.Count == 0 && (w.ID_ESTATUS_ADMINISTRATIVO == 1 || w.ID_ESTATUS_ADMINISTRATIVO == 2 || w.ID_ESTATUS_ADMINISTRATIVO == 3 || w.ID_ESTATUS_ADMINISTRATIVO == 8));
            }
            catch (Exception ex)
            {
                throw new ApplicationException(ex.Message);
            }
        }

        /// <summary>
        /// Obtiene el total de los internos en un centro.
        /// </summary>
        /// <returns>Numero total de internos</returns>
        public int ObtenerTotalInternos(short Centro)
        {
            try
            {
                //{1 = EN AREA TEMPORAL,2 = EN CLASIFICACION,3 = ASIGNADO A CELDA,8	= INDICIADOS}
                return GetData().Count(w => w.ID_UB_CENTRO == Centro && (w.ID_ESTATUS_ADMINISTRATIVO == 1 || w.ID_ESTATUS_ADMINISTRATIVO == 2 || w.ID_ESTATUS_ADMINISTRATIVO == 3 || w.ID_ESTATUS_ADMINISTRATIVO == 8));
            }
            catch (Exception ex)
            {
                throw new ApplicationException(ex.Message);
            }
        }

        /// <summary>
        /// Obtiene los internos con EMI incompleto (Paginado).
        /// </summary>
        /// <returns>Listado de los ingresos que no han completado la captura de su EMI</returns>
        public IQueryable<INGRESO> ObtenerEMIIncompleto(short Centro, string Estatus, int Pagina = 1)
        {
            try
            {
                //{1 = EN AREA TEMPORAL,2 = EN CLASIFICACION,3 = ASIGNADO A CELDA,8	= INDICIADOS}
                return (from i in this.Context.INGRESO
                        from ei in this.Context.EMI_INGRESO
                        from e in this.Context.EMI
                        where
                        i.ID_CENTRO == ei.ID_CENTRO && i.ID_ANIO == ei.ID_ANIO && i.ID_IMPUTADO == ei.ID_IMPUTADO &&
                        ei.ID_EMI == e.ID_EMI && ei.ID_EMI_CONS == e.ID_EMI_CONS &&
                        i.ID_UB_CENTRO == Centro && e.ESTATUS == Estatus && new[] { 1, 2, 3, 8 }.Contains(i.ID_ESTATUS_ADMINISTRATIVO.Value)
                        select i).OrderBy(o => o.ID_ANIO)
                        .ThenBy(t => t.ID_IMPUTADO)
                        .Take((Pagina * 30))
                        .Skip((Pagina == 1 ? 0 : ((Pagina * 30) - 30)));

                #region Query SQL
                /*
                     select i.* from ssp.ingreso i
                    inner join ssp.emi_ingreso ei on ei.id_centro = i.id_centro and ei.id_anio = i.id_anio and ei.id_imputado = i.id_imputado
                    inner join ssp.emi e on e.id_emi = ei.id_emi and e.id_emi_cons = ei.id_emi_cons
                    where i.id_ub_centro = 4 and i.id_estatus_administrativo in (8,1,2,3) and e.estatus = 'P';
                     */
                #endregion
            }
            catch (Exception ex)
            {
                throw new ApplicationException(ex.Message);
            }
        }

        /// <summary>
        /// Obtiene los internos con EMI incompleto (Todos).
        /// </summary>
        /// <returns>Listado de los ingresos que no han completado la captura de su EMI</returns>
        public IQueryable<INGRESO> ObtenerEMIIncompleto(short Centro, string Estatus)
        {
            try
            {
                //{1 = EN AREA TEMPORAL,2 = EN CLASIFICACION,3 = ASIGNADO A CELDA,8	= INDICIADOS}
                return (from i in this.Context.INGRESO
                        from ei in this.Context.EMI_INGRESO
                        from e in this.Context.EMI
                        where
                        i.ID_CENTRO == ei.ID_CENTRO && i.ID_ANIO == ei.ID_ANIO && i.ID_IMPUTADO == ei.ID_IMPUTADO &&
                        ei.ID_EMI == e.ID_EMI && ei.ID_EMI_CONS == e.ID_EMI_CONS &&
                        i.ID_UB_CENTRO == Centro && e.ESTATUS == Estatus && new[] { 1, 2, 3, 8 }.Contains(i.ID_ESTATUS_ADMINISTRATIVO.Value)
                        select i).OrderBy(o => o.ID_ANIO)
                        .ThenBy(t => t.ID_IMPUTADO);

            }
            catch (Exception ex)
            {
                throw new ApplicationException(ex.Message);
            }
        }
        /// <summary>
        /// Obtiene el total de los internos con EMI incompleto.
        /// </summary>
        /// <returns>Numero total de internos que no han completado la captura de su EMI</returns>
        public int ObtenerEMIIncompletoTotal(short Centro, string Estatus)
        {
            try
            {
                //{1 = EN AREA TEMPORAL,2 = EN CLASIFICACION,3 = ASIGNADO A CELDA,8	= INDICIADOS}
                return (from i in this.Context.INGRESO
                        from ei in this.Context.EMI_INGRESO
                        from e in this.Context.EMI
                        where
                        i.ID_CENTRO == ei.ID_CENTRO && i.ID_ANIO == ei.ID_ANIO && i.ID_IMPUTADO == ei.ID_IMPUTADO &&
                        ei.ID_EMI == e.ID_EMI && ei.ID_EMI_CONS == e.ID_EMI_CONS &&
                        i.ID_UB_CENTRO == Centro && e.ESTATUS == Estatus && new[] { 1, 2, 3, 8 }.Contains(i.ID_ESTATUS_ADMINISTRATIVO.Value)
                        select i).Count();

                #region Query SQL
                /*
                     select i.* from ssp.ingreso i
                    inner join ssp.emi_ingreso ei on ei.id_centro = i.id_centro and ei.id_anio = i.id_anio and ei.id_imputado = i.id_imputado
                    inner join ssp.emi e on e.id_emi = ei.id_emi and e.id_emi_cons = ei.id_emi_cons
                    where i.id_ub_centro = 4 and i.id_estatus_administrativo in (8,1,2,3) and e.estatus = 'P';
                     */
                #endregion
            }
            catch (Exception ex)
            {
                throw new ApplicationException(ex.Message);
            }
        }

        /// <summary>
        /// Obtiene el total de los internos con EMI completo.
        /// </summary>
        /// <returns>Numero total de internos que han completado la captura de su EMI</returns>
        public int ObtenerEMICompletoTotal(short Centro)
        {
            try
            {
                //{1 = EN AREA TEMPORAL,2 = EN CLASIFICACION,3 = ASIGNADO A CELDA,8	= INDICIADOS}
                return (from i in this.Context.INGRESO
                        from ei in this.Context.EMI_INGRESO
                        from e in this.Context.EMI
                        from efc in this.Context.EMI_FACTOR_CRIMINODIAGNOSTICO
                        where
                        i.ID_CENTRO == ei.ID_CENTRO && i.ID_ANIO == ei.ID_ANIO && i.ID_IMPUTADO == ei.ID_IMPUTADO &&
                        ei.ID_EMI == e.ID_EMI && ei.ID_EMI_CONS == e.ID_EMI_CONS &&
                        i.ID_UB_CENTRO == Centro && new[] { 1, 2, 3, 8 }.Contains(i.ID_ESTATUS_ADMINISTRATIVO.Value) &&
                        efc.ID_EMI == e.ID_EMI && efc.ID_EMI_CONS == e.ID_EMI_CONS
                        select i).Count();

                #region Query SQL
                /*
                     select i.* from ssp.ingreso i
                    inner join ssp.emi_ingreso ei on ei.id_centro = i.id_centro and ei.id_anio = i.id_anio and ei.id_imputado = i.id_imputado
                    inner join ssp.emi e on e.id_emi = ei.id_emi and e.id_emi_cons = ei.id_emi_cons
                    where i.id_ub_centro = 4 and i.id_estatus_administrativo in (8,1,2,3) and e.estatus = 'P';
                     */
                #endregion
            }
            catch (Exception ex)
            {
                throw new ApplicationException(ex.Message);
            }
        }

        public IQueryable<CAUSA_PENAL_INGRESOS> SeleccionarPorNUC(short centro, string nuc)
        {
            try
            {
                return (from n in this.Context.NUC
                        from i in this.Context.INGRESO
                        from cp in this.Context.CAUSA_PENAL
                        where
                        i.ID_UB_CENTRO == centro && i.ID_ESTATUS_ADMINISTRATIVO != 4 && i.ID_ESTATUS_ADMINISTRATIVO != 5 && i.ID_ESTATUS_ADMINISTRATIVO != 7 &&
                        i.ID_ANIO == cp.ID_ANIO && i.ID_CENTRO == cp.ID_CENTRO && i.ID_IMPUTADO == cp.ID_IMPUTADO && i.ID_INGRESO == cp.ID_INGRESO &&
                        n.ID_ANIO == cp.ID_ANIO && n.ID_CENTRO == cp.ID_CENTRO && n.ID_IMPUTADO == cp.ID_IMPUTADO && n.ID_INGRESO == cp.ID_INGRESO && n.ID_CAUSA_PENAL == cp.ID_CAUSA_PENAL &&
                        n.ID_NUC.Trim() == nuc.Trim()
                        select new CAUSA_PENAL_INGRESOS
                        {
                            INGRESO = i,
                            CAUSA_PENAL = cp
                        });
            }
            catch (Exception ex)
            {
                throw new ApplicationException(ex.Message + " " + (ex.InnerException != null ? ex.InnerException.InnerException.Message : ""));
            }
        }

        public bool HasCertificadoIngreso(short id_centro, short id_anio, int id_imputado, short id_ingreso, short id_tipo_atencion)
        {
            try
            {
                return Context.INGRESO.Any(w => w.ID_CENTRO == id_centro && w.ID_ANIO == id_anio && w.ID_IMPUTADO == id_imputado && w.ID_INGRESO == id_ingreso
                    && (w.ATENCION_CITA.Any(a2 => a2.ID_TIPO_SERVICIO == 2 && a2.ID_TIPO_ATENCION == id_tipo_atencion) || w.ATENCION_MEDICA.Any(a2 => a2.ID_TIPO_SERVICIO == 2 && a2.ID_TIPO_ATENCION == id_tipo_atencion)));

            }
            catch (Exception ex)
            {
                throw new ApplicationException(ex.Message + " " + (ex.InnerException != null ? ex.InnerException.InnerException.Message : ""));
            }
        }

        /// <summary>
        ///  Obtiene los ingresos que se consideran para su liberacion
        /// </summary>
        /// <param name="Centro">Centro del usuario logeado</param>
        /// <returns>Listade ingresos</returns>
        public IQueryable<INGRESO> GetIngresoParaLiberacion(short Centro)
        {
            try
            {
                short[] ea = new short[] { 1, 2, 3, 8 };
                short[] ecp = new short[] { 0, 1, 6 };
                var predicate = PredicateBuilder.True<INGRESO>();
                predicate = predicate.And(w => ea.Contains(w.ID_ESTATUS_ADMINISTRATIVO.Value) && w.ID_UB_CENTRO == Centro && w.LIBERACION.Count(x => x.LIBERADO != "S" && x.CAUSA_PENAL == null) > 0);
                predicate = predicate.Or(w => ea.Contains(w.ID_ESTATUS_ADMINISTRATIVO.Value) && w.ID_UB_CENTRO == Centro && w.LIBERACION.Count(x => x.LIBERADO != "S" && w.CAUSA_PENAL.Count(y => ecp.Contains(y.ID_ESTATUS_CP.Value)) == 0) > 0);
                return GetData(predicate.Expand()).Distinct().OrderBy(w => new { w.ID_CENTRO, w.ID_ANIO, w.ID_IMPUTADO });//.Take((Pagina * 30)).Skip((Pagina == 1 ? 0 : ((Pagina * 30) - 30)));     
            }
            catch (Exception ex)
            {
                throw new ApplicationException(ex.Message);
            }
        }

        /// <summary>
        /// Metodo ultilizado para liberar internos
        /// </summary>
        /// <param name="Centro">Centro</param>
        /// <param name="Anio">Anio</param>
        /// <param name="Imputado">Imputado</param>
        /// <param name="Ingreso">Ingreso</param>
        /// <param name="Incidente">Hay incidente al liberar Si = S / No = N</param>
        /// <param name="FotoSalida">byte[]</param>
        /// <returns>true = se realizao la liberacion, false = ocurro un error</returns>
        public bool LiberarIngreso(short? Centro = null, short? Anio = null, int? Imputado = null, short? Ingreso = null, string Incidente = "N", byte[] FotoSalida = null)
        {
            try
            {
                using (TransactionScope transaccion = new TransactionScope(TransactionScopeOption.RequiresNew, new TransactionOptions() { IsolationLevel = System.Transactions.IsolationLevel.ReadCommitted }))
                {
                    var ingreso = Context.INGRESO.Where(w => w.ID_CENTRO == Centro && w.ID_ANIO == Anio && w.ID_IMPUTADO == Imputado && w.ID_INGRESO == Ingreso).FirstOrDefault();
                    if (ingreso != null)
                    {
                        if (ingreso.LIBERACION != null)
                        {
                            foreach (var l in ingreso.LIBERACION.Where(w => w.LIBERADO != "S"))
                            {
                                l.LIBERADO = "S";
                                l.INCIDENTE_BIOMETRICO = Incidente;
                                Context.Entry(l).State = EntityState.Modified;
                            }
                        }

                        if (FotoSalida != null)
                        {

                            var fb = Context.INGRESO_BIOMETRICO.Where(w => w.ID_CENTRO == Centro.Value && w.ID_ANIO == Anio.Value && w.ID_IMPUTADO == Imputado.Value && w.ID_INGRESO == Ingreso.Value && w.ID_TIPO_BIOMETRICO == 101).FirstOrDefault();
                            if (fb != null)
                            {
                                Context.Entry(fb).State = EntityState.Deleted;
                            }
                            var ib = new INGRESO_BIOMETRICO();
                            ib.ID_CENTRO = Centro.Value;
                            ib.ID_ANIO = Anio.Value;
                            ib.ID_IMPUTADO = Imputado.Value;
                            ib.ID_INGRESO = Ingreso.Value;
                            ib.ID_TIPO_BIOMETRICO = 101;
                            ib.BIOMETRICO = FotoSalida;
                            ib.ID_FORMATO = 5;
                            Context.INGRESO_BIOMETRICO.Add(ib);
                        }

                        //actualizamos el estatus
                        ingreso.ID_ESTATUS_ADMINISTRATIVO = 4;
                        Context.Entry(ingreso).State = EntityState.Modified;
                    }

                    Context.SaveChanges();
                    transaccion.Complete();
                    return true;
                }
            }
            catch (Exception ex)
            {
                throw new ApplicationException(ex.Message);
            }
            return false;
        }



        public IEnumerable<cReporteRelacionInterno> ReporteRelacionInterno(short Centro, short OrdenarPor)
        {
            try
            {

                var query = string.Format("SELECT * FROM SSP.V_RELACION_INTERNOS WHERE ID_UB_CENTRO = {0}", Centro);
                switch (OrdenarPor)
                {
                    case 1://expediente
                        query = query + " ORDER BY ID_ANIO,ID_IMPUTADO";
                        break;
                    case 2://nombre
                        query = query + " ORDER BY NOMBRE,PATERNO,MATERNO";
                        break;
                    case 3://Fecha de ingreso
                        query = query + " ORDER BY FEC_INGRESO_CERESO";
                        break;
                    case 4://Ubicacion
                        query = query + " ORDER BY ID_UB_CENTRO,ID_UB_EDIFICIO,ID_UB_SECTOR,ID_UB_CELDA,ID_UB_CAMA ";
                        break;
                    case 5://Autoridad que interna
                        query = query + " ORDER BY ID_AUTORIDAD_INTERNA";
                        break;
                    case 6://Clasificacion juridica
                        query = query + " ORDER BY ID_CLASIFICACION_JURIDICA";
                        break;
                }
                return Context.Database.SqlQuery<cReporteRelacionInterno>(query);
            }
            catch (Exception ex)
            {
                throw new ApplicationException(ex.Message);
            }
        }

        public IEnumerable<cReporteRelacionInterno> ReporteGeneralDelito(short Centro, short OrdenarPor,bool Ordenamiento = true)
        {
            try
            {

                var query = string.Format("SELECT V.*,SSP.FUNC_ALIAS(ID_CENTRO,ID_ANIO,ID_IMPUTADO) AS ALIAS FROM SSP.V_RELACION_INTERNOS V WHERE ID_UB_CENTRO = {0}", Centro);
                switch (OrdenarPor)
                {
                    case 1://expediente
                        if(Ordenamiento)
                            query = query + " ORDER BY ID_ANIO,ID_IMPUTADO";
                        else
                            query = query + " ORDER BY ID_ANIO DESC,ID_IMPUTADO DESC";
                        break;
                    case 2://nombre
                        if(Ordenamiento)
                            query = query + " ORDER BY PATERNO,MATERNO,NOMBRE";
                        else
                            query = query + " ORDER BY PATERNO DESC,MATERNO DESC,NOMBRE DESC";
                        break;
                    case 3://Fecha de ingreso
                        if(Ordenamiento)
                            query = query + " ORDER BY FEC_INGRESO_CERESO";
                        else
                            query = query + " ORDER BY FEC_INGRESO_CERESO DESC";
                        break;
                    case 4://Ubicacion
                        if(Ordenamiento)
                            query = query + " ORDER BY ID_UB_CENTRO,ID_UB_EDIFICIO,ID_UB_SECTOR,ID_UB_CELDA,ID_UB_CAMA ";
                        else
                            query = query + " ORDER BY ID_UB_CENTRO DESC,ID_UB_EDIFICIO DESC,ID_UB_SECTOR DESC,ID_UB_CELDA DESC,ID_UB_CAMA DESC";
                        break;
                    case 6://Clasificacion juridica
                        if(Ordenamiento)
                            query = query + " ORDER BY ID_CLASIFICACION_JURIDICA";
                        else
                            query = query + " ORDER BY ID_CLASIFICACION_JURIDICA DESC";
                        break;
                }
                return Context.Database.SqlQuery<cReporteRelacionInterno>(query);
            }
            catch (Exception ex)
            {
                throw new ApplicationException(ex.Message);
            }
        }

        public IEnumerable<cReporteRelacionInterno> ReporteRelacionInternoUbicacion(short Centro, short? Edificio = null,short? Sector = null)
        {
            try
            {

                var query = new StringBuilder();
                query.AppendFormat("SELECT * FROM SSP.V_RELACION_INTERNOS WHERE ID_UB_CENTRO = {0}", Centro);
                if (Edificio.HasValue)
                {
                    query.AppendFormat(" AND ID_UB_EDIFICIO = {0} ",Edificio);
                }
                if (Sector.HasValue)
                {
                    query.AppendFormat(" AND ID_UB_SECTOR = {0} ", Sector);
                }
                query.Append("ORDER BY EDIFICIO,SECTOR,CELDA,CAMA ");
                return Context.Database.SqlQuery<cReporteRelacionInterno>(query.ToString());
            }
            catch (Exception ex)
            {
                throw new ApplicationException(ex.Message);
            }
        }

        public IEnumerable<cReporteRelacionInterno> ReporteSituacionJuridica(short Centro)
        {
            try
            {

                var query = string.Format("SELECT V.*,ssp.FUNC_ALIAS(ID_CENTRO,ID_ANIO,ID_IMPUTADO) AS ALIAS,ssp.FUNC_APODOS(ID_CENTRO,ID_ANIO,ID_IMPUTADO) AS APODOS FROM SSP.V_RELACION_INTERNOS V WHERE ID_UB_CENTRO = {0}  ORDER BY V.PATERNO,V.MATERNO,V.NOMBRE", Centro);
                return Context.Database.SqlQuery<cReporteRelacionInterno>(query);
            }
            catch (Exception ex)
            {
                throw new ApplicationException(ex.Message);
            }
        }

        public IEnumerable<cIngresoUbicacionEdificio> ReporteUbicacionIngresoEdificio(int Centro,DateTime FechaInicio,DateTime FechaFin)
        {
            try
            {
                string query = "SELECT "+
                "ING.ID_CENTRO,ING.ID_ANIO,ING.ID_IMPUTADO, "+
                "TRIM(I.NOMBRE) AS NOMBRE,TRIM(I.PATERNO) AS PATERNO,TRIM(I.MATERNO) AS MATERNO, "+
                "TRIM(E.DESCR) AS EDIFICIO,TRIM(S.DESCR) AS SECTOR,TRIM(ING.ID_UB_CELDA) AS CELDA,ING.ID_UB_CAMA AS CAMA, "+
                "IU.ID_AREA,A.DESCR AREA,IU.MOVIMIENTO_FEC,IU.ACTIVIDAD,IU.ESTATUS,IU.ID_CONSEC "+
                "FROM SSP.INGRESO ING  "+
                "INNER JOIN SSP.IMPUTADO I ON ING.ID_CENTRO = I.ID_CENTRO AND ING.ID_ANIO = I.ID_ANIO AND ING.ID_IMPUTADO = I.ID_IMPUTADO  "+
                "INNER JOIN SSP.EDIFICIO E ON ING.ID_UB_CENTRO = E.ID_CENTRO AND ING.ID_UB_EDIFICIO = E.ID_EDIFICIO "+
                "INNER JOIN SSP.SECTOR S ON ING.ID_UB_CENTRO = S.ID_CENTRO AND ING.ID_UB_EDIFICIO = S.ID_EDIFICIO AND ING.ID_UB_SECTOR = S.ID_SECTOR "+
                "INNER JOIN SSP.INGRESO_UBICACION IU ON ING.ID_CENTRO = IU.ID_CENTRO AND ING.ID_ANIO = IU.ID_ANIO AND ING.ID_IMPUTADO = IU.ID_IMPUTADO AND ING.ID_INGRESO = IU.ID_INGRESO "+
                "INNER JOIN SSP.AREA A ON IU.ID_AREA = A.ID_AREA "+
                "WHERE ING.ID_UB_CENTRO = {0}  AND ING.ID_ESTATUS_ADMINISTRATIVO IN (1,2,3,8) AND TRUNC(IU.MOVIMIENTO_FEC) >= '{1}' AND TRUNC(IU.MOVIMIENTO_FEC) <= '{2}' " +
                "ORDER BY IU.ID_CENTRO,IU.ID_ANIO,IU.ID_IMPUTADO,IU.ID_INGRESO,IU.ID_CONSEC";
                query = string.Format(query, Centro,FechaInicio.ToShortDateString(),FechaFin.ToShortDateString());
                return Context.Database.SqlQuery<cIngresoUbicacionEdificio>(query);
            }
            catch (Exception ex)
            {
                throw new ApplicationException(ex.Message);
            }
        }


        public IEnumerable<cReporteReingresos>ReporteReingresos(short Centro, DateTime? FechaInicio = null, DateTime? FechaFinal = null)
        {
            try
            {
                string query = "SELECT " +
                "IMP.SEXO,D.ID_FUERO,D.ID_DELITO,D.DESCR DELITO, "+
                "COUNT(CASE WHEN IMP.SEXO = 'M' THEN 1 END) AS TOTAL_M, "+
                "COUNT(CASE WHEN IMP.SEXO = 'F' THEN 1 END) AS TOTAL_F, " +
                "COUNT(D.ID_DELITO) AS TOTAL " +
                "FROM  " +
                "SSP.INGRESO ING " +
                "INNER JOIN SSP.IMPUTADO IMP ON ING.ID_CENTRO = IMP.ID_CENTRO AND ING.ID_ANIO = IMP.ID_ANIO AND ING.ID_IMPUTADO = IMP.ID_IMPUTADO " +
                "INNER JOIN SSP.CAUSA_PENAL CP ON ING.ID_CENTRO = CP.ID_CENTRO AND ING.ID_ANIO = CP.ID_ANIO AND ING.ID_IMPUTADO = CP.ID_IMPUTADO AND ING.ID_INGRESO = CP.ID_INGRESO " +
                "INNER JOIN SSP.CAUSA_PENAL_DELITO CPD ON CP.ID_CENTRO = CPD.ID_CENTRO AND CP.ID_ANIO = CPD.ID_ANIO AND CP.ID_IMPUTADO = CPD.ID_IMPUTADO AND CP.ID_INGRESO = CPD.ID_INGRESO AND CP.ID_CAUSA_PENAL = CPD.ID_CAUSA_PENAL " +
                "INNER JOIN SSP.DELITO D ON D.ID_FUERO = CPD.ID_FUERO AND D.ID_DELITO = CPD.ID_DELITO " +
                "WHERE ING.ID_UB_CENTRO = {0} AND ING.ID_INGRESO > 1 AND ING.ID_ESTATUS_ADMINISTRATIVO IN (1,2,3,8) " +
                "GROUP BY IMP.SEXO,D.ID_FUERO,D.ID_DELITO,D.DESCR ";
                query = string.Format(query, Centro);
                return Context.Database.SqlQuery<cReporteReingresos>(query);
            }
            catch (Exception ex)
            { 
               throw new ApplicationException(ex.Message);
            }
        }

        public IEnumerable<cReporteReingresos> ReporteReincidencias(short Centro, DateTime? FechaInicio = null, DateTime? FechaFinal = null)
        {
            try
            {
                string query = "SELECT " +
                "D.ID_FUERO,D.ID_DELITO,D.DESCR DELITO, " +
                "COUNT(CASE WHEN IMP.SEXO = 'M' THEN 1 END) AS TOTAL_M, " +
                "COUNT(CASE WHEN IMP.SEXO = 'F' THEN 1 END) AS TOTAL_F, " +
                "COUNT(D.ID_DELITO) AS TOTAL " +
                "FROM  " +
                "SSP.INGRESO ING " +
                "INNER JOIN SSP.IMPUTADO IMP ON ING.ID_CENTRO = IMP.ID_CENTRO AND ING.ID_ANIO = IMP.ID_ANIO AND ING.ID_IMPUTADO = IMP.ID_IMPUTADO " +
                "INNER JOIN SSP.CAUSA_PENAL CP ON ING.ID_CENTRO = CP.ID_CENTRO AND ING.ID_ANIO = CP.ID_ANIO AND ING.ID_IMPUTADO = CP.ID_IMPUTADO AND ING.ID_INGRESO = CP.ID_INGRESO  " +
                "INNER JOIN SSP.SENTENCIA S ON CP.ID_CENTRO = S.ID_CENTRO AND CP.ID_ANIO = S.ID_ANIO AND CP.ID_IMPUTADO = S.ID_IMPUTADO AND CP.ID_INGRESO = S.ID_INGRESO AND CP.ID_CAUSA_PENAL = S.ID_CAUSA_PENAL AND S.ESTATUS = 'A' " +
                "INNER JOIN SSP.CAUSA_PENAL_DELITO CPD ON CP.ID_CENTRO = CPD.ID_CENTRO AND CP.ID_ANIO = CPD.ID_ANIO AND CP.ID_IMPUTADO = CPD.ID_IMPUTADO AND CP.ID_INGRESO = CPD.ID_INGRESO AND CP.ID_CAUSA_PENAL = CPD.ID_CAUSA_PENAL " +
                "INNER JOIN SSP.DELITO D ON D.ID_FUERO = CPD.ID_FUERO AND D.ID_DELITO = CPD.ID_DELITO " +
                "WHERE ING.ID_UB_CENTRO = 4 AND ING.ID_INGRESO > 1 AND ING.ID_ESTATUS_ADMINISTRATIVO IN (1,2,3,8) AND FEC_EJECUTORIA IS NOT NULL " +
                "GROUP BY IMP.SEXO,D.ID_FUERO,D.ID_DELITO,D.DESCR";
                query = string.Format(query, Centro);
                return Context.Database.SqlQuery<cReporteReingresos>(query);

            }
            catch (Exception ex)
            {
                throw new ApplicationException(ex.Message);
            }
        }

        public IEnumerable<cInternoPapeletas> ObtenerInternosPapeletas(int Centro, short? Edificio = null, short? Sector = null)
        {
            try
            {
                var query = new StringBuilder();
                query.Append("SELECT ");
                query.Append("I.ID_CENTRO,I.ID_ANIO,I.ID_IMPUTADO,I.ID_INGRESO,TRIM(IM.NOMBRE) NOMBRE,TRIM(IM.PATERNO) PATERNO,TRIM(IM.MATERNO) MATERNO,IB.BIOMETRICO FOTO, ");
                query.Append("TRIM(E.DESCR) EDIFICIO,TRIM(S.DESCR) SECTOR,TRIM(I.ID_UB_CELDA) CELDA,I.ID_UB_CAMA CAMA ");
                query.Append("FROM SSP.INGRESO I ");
                query.Append("INNER JOIN SSP.IMPUTADO IM ON I.ID_CENTRO = IM.ID_CENTRO AND I.ID_ANIO = IM.ID_ANIO AND I.ID_IMPUTADO = IM.ID_IMPUTADO ");
                query.Append("LEFT JOIN SSP.INGRESO_BIOMETRICO IB ON I.ID_CENTRO = IB.ID_CENTRO AND I.ID_ANIO = IB.ID_ANIO AND I.ID_IMPUTADO = IB.ID_IMPUTADO AND I.ID_INGRESO = IB.ID_INGRESO AND ID_TIPO_BIOMETRICO = 105 ");
                query.Append("INNER JOIN SSP.EDIFICIO E ON I.ID_UB_CENTRO = E.ID_CENTRO AND I.ID_UB_EDIFICIO = E.ID_EDIFICIO ");
                query.Append("INNER JOIN SSP.SECTOR S ON I.ID_UB_CENTRO = S.ID_CENTRO AND I.ID_UB_EDIFICIO = S.ID_EDIFICIO AND I.ID_UB_SECTOR = S.ID_SECTOR ");
                query.AppendFormat("WHERE I.ID_UB_CENTRO = {0} AND I.ID_ESTATUS_ADMINISTRATIVO IN (1,2,3,8) ",Centro);
                if (Edificio.HasValue)
                    query.AppendFormat("AND I.ID_UB_EDIFICIO = {0} ", Edificio);
                if (Sector.HasValue)
                    query.AppendFormat("AND I.ID_UB_SECTOR = {0} ", Sector);
                return Context.Database.SqlQuery<cInternoPapeletas>(query.ToString());
            }
            catch (Exception ex)
            {
                throw new ApplicationException(ex.Message);
            }
        }

        #region Reportes CNDH
        public IQueryable<INGRESO> ListadoGeneralCNDH(string Sexo = "", string Fuero = "", string Clasificacion = "", short? Centro = null)
        {
            try
            {
                var predicate = PredicateBuilder.True<INGRESO>();
                if (!string.IsNullOrEmpty(Sexo))
                    predicate = predicate.And(w => w.IMPUTADO.SEXO == Sexo);
                if (!string.IsNullOrEmpty(Fuero))
                    predicate = predicate.And(w => w.CAUSA_PENAL.FirstOrDefault(x => x.ID_ESTATUS_CP == 1 && x.CP_FUERO == Fuero) != null);
                if (!string.IsNullOrEmpty(Clasificacion))
                    predicate = predicate.And(w => w.ID_CLASIFICACION_JURIDICA == Clasificacion);
                if(Centro.HasValue)
                    predicate = predicate.And(w => w.ID_UB_CENTRO == Centro);
                return GetData(predicate.Expand());
            }
            catch (Exception ex)
            {
                throw new ApplicationException(ex.Message);
            }
        }
        #endregion
    }


    public class cReporteRelacionInterno
    {
        public int ID_CENTRO { get; set; }
        public int ID_ANIO { get; set; }
        public int ID_IMPUTADO { get; set; }
        public string NOMBRE { get; set; }
        public string PATERNO { get; set; }
        public string MATERNO { get; set; }
        public DateTime? NACIMIENTO_FECHA { get; set; }
        public string CLASIFICACION_JURIDICA { get; set; }
        public string FUERO { get; set; }
        public string EDIFICIO { get; set; }
        public string SECTOR { get; set; }
        public string CELDA { get; set; }
        public int? CAMA { get; set; }
        public int? ID_CAUSA_PENAL { get; set; }
        public byte[] FOTO { get; set; }
        public int? ID_UB_CENTRO { get; set; }
        public int? ID_UB_EDIFICIO { get; set; }
        public int? ID_UB_SECTOR { get; set; }
        public string ID_UB_CELDA { get; set; }
        public int? ID_UB_CAMA { get; set; }
        public DateTime? FEC_INGRESO_CERESO { get; set; }
        public int? ID_AUTORIDAD_INTERNA { get; set; }
        public string ID_CLASIFICACION_JURIDICA { get; set; }
        public int ID_INGRESO { get; set; }
        public string INGRESO_DELITO { get; set; }
        public string ALIAS { get; set; }
        public string APODOS { get; set; }
    }

    public class ingresoPrograma
    {
        public short? ING_ID_CENTRO { get; set; }
        public short? ING_ID_ANIO { get; set; }
        public int? ING_ID_IMPUTADO { get; set; }
        public short? ING_ID_INGRESO { get; set; }
    }

    public class cIngresoUbicacionEdificio
    {
        public int ID_CENTRO { get; set; }
        public int ID_ANIO { get; set; }
        public int ID_IMPUTADO { get; set; }
        public string NOMBRE { get; set; }
        public string PATERNO { get; set; }
        public string MATERNO { get; set; }
        public string EDIFICIO { get; set; }
        public string SECTOR { get; set; }
        public string CELDA { get; set; }
        public int CAMA { get; set; }
        public int ID_AREA { get; set; }
        public string AREA { get; set; }
        public DateTime MOVIMIENTO_FEC { get; set; }
        public string ACTIVIDAD { get; set; }
        public int ESTATUS { get; set; }
        public int ID_CONSEC { get; set; }
    }

    public class cReporteReingresos
    {
        public string ID_FUERO{ get; set; }
        public long ID_DELITO{ get; set; }
        public string DELITO { get; set; }
        public int TOTAL_M { get; set; }
        public int TOTAL_F { get; set; }
        public int TOTAL { get; set; }
    }

    public class cInternoPapeletas
    {
        public short ID_CENTRO { get; set; }
        public short ID_ANIO { get; set; }
        public int ID_IMPUTADO { get; set; }
        public short ID_INGRESO { get; set; }
        public string NOMBRE { get; set; }
        public string PATERNO { get; set; }
        public string MATERNO { get; set; }
        public byte[] FOTO { get; set; }
        public string EDIFICIO { get; set; }
        public string SECTOR { get; set; }
        public string CELDA { get; set; }
        public int CAMA { get; set; }
    }
}