using SSP.Servidor;
using SSP.Modelo;
using System.Linq;
using System.Data;

namespace SSP.Controlador.Catalogo.Justicia
{
    public class cRealizacionEstudiosPersonalidad : EntityManagerServer<PERSONALIDAD_FUERO_COMUN>
    {
        private enum eTiposEstudio
        {//Se define enumerador para alimentar detalle de estudio de personalidad
            CRIMINOLOGICO = 1,
            TRABAJO_SOCIAL = 2,
            SEGURIDAD = 3,
            MEDICO = 4,
            PSICOLOGIA = 5,
            PSIQUIATRIA = 6,
            PEDAGOGIA = 7,
            LABORAL = 8,
            CRIMIN_FEDERAL = 9,
            TRABAJO_SOCIAL_FEDERAL = 10,
            SEGURIDAD_FEDERAL = 11,
            MEDICA_FEDERAL = 12,
            PSIQUIATRICA_FEDERAL = 13,
            PSICOLOGICA_FEDERAL = 14,
            PEDAGOGICA_FEDERAL = 15,
            LABORAL_FEDERAL = 16
        };
        private enum eEstatudDetallePersonalidad
        {
            ACTIVO = 1,
            PENDIENTE = 2,
            TERMINADO = 3,
            CANCELADO = 4,
            ASIGNADO = 5
        };
        private enum eResultado
        {
            FAVORABLE = 1,
            DESFAVORABLE = 2
        }

        //Esta clase sirve para guardar los estudios de personalidad, incluidos los formatos de fuero comun y federal
        public bool IngresaEstudioConHijos(PERSONALIDAD_FUERO_COMUN Entity, PERSONALIDAD_FUERO_FEDERAL EntityFederal)
        {
            try
            {
                using (System.Transactions.TransactionScope transaccion = new System.Transactions.TransactionScope(System.Transactions.TransactionScopeOption.RequiresNew, new System.Transactions.TransactionOptions() { IsolationLevel = System.Transactions.IsolationLevel.ReadCommitted }))
                {
                    # region Fuero Comun
                    if (Entity != null)
                    {
                        var ValidaEstudio = Context.PERSONALIDAD_FUERO_COMUN.FirstOrDefault(c => c.ID_IMPUTADO == Entity.ID_IMPUTADO && c.ID_INGRESO == Entity.ID_INGRESO && c.ID_ESTUDIO == Entity.ID_ESTUDIO && c.ID_ANIO == Entity.ID_ANIO && c.ID_CENTRO == Entity.ID_CENTRO);
                        var _EstudioPadre = Context.PERSONALIDAD.FirstOrDefault(x => x.ID_IMPUTADO == Entity.ID_IMPUTADO && x.ID_INGRESO == Entity.ID_INGRESO && x.ID_ESTUDIO == Entity.ID_ESTUDIO && x.ID_ANIO == Entity.ID_ANIO && x.ID_CENTRO == Entity.ID_CENTRO);

                        #region Nuevo
                        if (ValidaEstudio == null)
                        {
                            var _EstudioFueroComun = new PERSONALIDAD_FUERO_COMUN()
                            {
                                ID_ANIO = Entity.ID_ANIO,
                                ID_CENTRO = Entity.ID_CENTRO,
                                ID_ESTUDIO = Entity.ID_ESTUDIO,
                                ID_IMPUTADO = Entity.ID_IMPUTADO,
                                ID_INGRESO = Entity.ID_INGRESO
                            };

                            Context.PERSONALIDAD_FUERO_COMUN.Add(_EstudioFueroComun);

                            if (Entity.PFC_II_MEDICO != null)
                            {
                                #region Medico
                                _EstudioFueroComun.PFC_II_MEDICO = new PFC_II_MEDICO()
                                {
                                    ESTUDIO_FEC = Entity.PFC_II_MEDICO.ESTUDIO_FEC,
                                    ID_ANIO = Entity.ID_ANIO,
                                    ID_CENTRO = Entity.ID_CENTRO,
                                    ID_ESTUDIO = Entity.ID_ESTUDIO,
                                    ID_IMPUTADO = Entity.ID_IMPUTADO,
                                    ID_INGRESO = Entity.ID_INGRESO,
                                    P2_HEREDO_FAMILIARES = Entity.PFC_II_MEDICO.P2_HEREDO_FAMILIARES,
                                    P3_ANTPER_NOPATO = Entity.PFC_II_MEDICO.P3_ANTPER_NOPATO,
                                    P31_CONSUMO_TOXICO = Entity.PFC_II_MEDICO.P31_CONSUMO_TOXICO,
                                    P32_TATUAJES_CICATRICES = Entity.PFC_II_MEDICO.P32_TATUAJES_CICATRICES,
                                    P4_PATOLOGICOS = Entity.PFC_II_MEDICO.P4_PATOLOGICOS,
                                    P5_PADECIMIENTOS = Entity.PFC_II_MEDICO.P5_PADECIMIENTOS,
                                    P6_EXPLORACION_FISICA = Entity.PFC_II_MEDICO.P6_EXPLORACION_FISICA,
                                    P7_IMPRESION_DIAGNOSTICA = Entity.PFC_II_MEDICO.P7_IMPRESION_DIAGNOSTICA,
                                    P8_DICTAMEN_MEDICO = Entity.PFC_II_MEDICO.P8_DICTAMEN_MEDICO.HasValue ? Entity.PFC_II_MEDICO.P8_DICTAMEN_MEDICO != decimal.Zero ? Entity.PFC_II_MEDICO.P8_DICTAMEN_MEDICO : null : null,
                                    SIGNOS_ESTATURA = Entity.PFC_II_MEDICO.SIGNOS_ESTATURA,
                                    SIGNOS_PESO = Entity.PFC_II_MEDICO.SIGNOS_PESO,
                                    SIGNOS_PULSO = Entity.PFC_II_MEDICO.SIGNOS_PULSO,
                                    SIGNOS_RESPIRACION = Entity.PFC_II_MEDICO.SIGNOS_RESPIRACION,
                                    SIGNOS_TA = Entity.PFC_II_MEDICO.SIGNOS_TA,
                                    ELABORO = Entity.PFC_II_MEDICO.ELABORO,
                                    COORDINADOR = Entity.PFC_II_MEDICO.COORDINADOR,
                                    SIGNOS_TEMPERATURA = Entity.PFC_II_MEDICO.SIGNOS_TEMPERATURA
                                };

                                #endregion
                                Context.PFC_II_MEDICO.Add(_EstudioFueroComun.PFC_II_MEDICO);
                                #region Detalle Estudio
                                if (_EstudioPadre != null)
                                {
                                    short? _Estatus = new short?();
                                    var _DetalleMedico = Context.PERSONALIDAD.FirstOrDefault(x => x.ID_ESTUDIO == _EstudioPadre.ID_ESTUDIO && x.ID_ANIO == _EstudioPadre.ID_ANIO && x.ID_CENTRO == _EstudioPadre.ID_CENTRO && x.ID_IMPUTADO == _EstudioPadre.ID_IMPUTADO && x.ID_INGRESO == _EstudioPadre.ID_INGRESO);
                                    if (string.IsNullOrEmpty(Entity.PFC_II_MEDICO.COORDINADOR) || string.IsNullOrEmpty(Entity.PFC_II_MEDICO.ELABORO) || string.IsNullOrEmpty(Entity.PFC_II_MEDICO.P2_HEREDO_FAMILIARES) || string.IsNullOrEmpty(Entity.PFC_II_MEDICO.P3_ANTPER_NOPATO) ||
                                        string.IsNullOrEmpty(Entity.PFC_II_MEDICO.P31_CONSUMO_TOXICO) || string.IsNullOrEmpty(Entity.PFC_II_MEDICO.P32_TATUAJES_CICATRICES) || string.IsNullOrEmpty(Entity.PFC_II_MEDICO.P4_PATOLOGICOS) || string.IsNullOrEmpty(Entity.PFC_II_MEDICO.P5_PADECIMIENTOS) ||
                                        string.IsNullOrEmpty(Entity.PFC_II_MEDICO.P7_IMPRESION_DIAGNOSTICA) || string.IsNullOrEmpty(Entity.PFC_II_MEDICO.SIGNOS_ESTATURA) || string.IsNullOrEmpty(Entity.PFC_II_MEDICO.SIGNOS_PESO) || string.IsNullOrEmpty(Entity.PFC_II_MEDICO.SIGNOS_PULSO) ||
                                        string.IsNullOrEmpty(Entity.PFC_II_MEDICO.SIGNOS_RESPIRACION) || string.IsNullOrEmpty(Entity.PFC_II_MEDICO.SIGNOS_TA) || string.IsNullOrEmpty(Entity.PFC_II_MEDICO.SIGNOS_TEMPERATURA) || Entity.PFC_II_MEDICO.ESTUDIO_FEC == null || Entity.PFC_II_MEDICO.P8_DICTAMEN_MEDICO == null)
                                        _Estatus = (short)eEstatudDetallePersonalidad.ASIGNADO;
                                    else
                                        _Estatus = (short)eEstatudDetallePersonalidad.TERMINADO;

                                    if (_DetalleMedico != null)
                                    {
                                        var _DesarrolloEstudioMedico = Context.PERSONALIDAD_DETALLE.FirstOrDefault(c => c.ID_ESTUDIO == _DetalleMedico.ID_ESTUDIO && c.ID_TIPO == (short)eTiposEstudio.MEDICO && c.ID_ANIO == _DetalleMedico.ID_ANIO && c.ID_IMPUTADO == _EstudioFueroComun.ID_IMPUTADO && c.ID_INGRESO == _EstudioFueroComun.ID_INGRESO && c.ID_CENTRO == _EstudioFueroComun.ID_CENTRO);
                                        if (_DesarrolloEstudioMedico == null)
                                        {//No se le habia generado aun el detalle del estudio medico
                                            short _ConsecutivoPersonalidadDetalle = GetIDProceso<short>("PERSONALIDAD_DETALLE", "ID_DETALLE", "1=1");
                                            var _NvoDetalle = new PERSONALIDAD_DETALLE()
                                            {
                                                DIAS_BONIFICADOS = null,
                                                ID_ANIO = Entity.ID_ANIO,
                                                ID_CENTRO = Entity.ID_CENTRO,
                                                ID_DETALLE = _ConsecutivoPersonalidadDetalle,
                                                ID_ESTUDIO = _EstudioPadre.ID_ESTUDIO,
                                                ID_IMPUTADO = Entity.ID_IMPUTADO,
                                                ID_INGRESO = Entity.ID_INGRESO,
                                                ID_TIPO = (short)eTiposEstudio.MEDICO,
                                                INICIO_FEC = GetFechaServerDate(),
                                                RESULTADO = _EstudioFueroComun.PFC_II_MEDICO != null ? _EstudioFueroComun.PFC_II_MEDICO.P8_DICTAMEN_MEDICO.HasValue ? _EstudioFueroComun.PFC_II_MEDICO.P8_DICTAMEN_MEDICO == (short)eResultado.FAVORABLE ? "S" : _EstudioFueroComun.PFC_II_MEDICO.P8_DICTAMEN_MEDICO == (short)eResultado.DESFAVORABLE ? "N" : string.Empty : string.Empty : string.Empty,
                                                SOLICITUD_FEC = _EstudioPadre.SOLICITUD_FEC,
                                                ID_ESTATUS = _Estatus,
                                                TERMINO_FEC = _Estatus == (short)eEstatudDetallePersonalidad.TERMINADO ? GetFechaServerDate() : new System.DateTime?(),
                                                TIPO_MEDIA = string.Empty
                                            };

                                            Context.PERSONALIDAD_DETALLE.Add(_NvoDetalle);
                                        }

                                        else
                                        {
                                            _DesarrolloEstudioMedico.RESULTADO = _EstudioFueroComun.PFC_II_MEDICO != null ? _EstudioFueroComun.PFC_II_MEDICO.P8_DICTAMEN_MEDICO.HasValue ? _EstudioFueroComun.PFC_II_MEDICO.P8_DICTAMEN_MEDICO == (short)eResultado.FAVORABLE ? "N" : _EstudioFueroComun.PFC_II_MEDICO.P8_DICTAMEN_MEDICO == (short)eResultado.DESFAVORABLE ? "S" : string.Empty : string.Empty : string.Empty;
                                            _DesarrolloEstudioMedico.ID_ESTATUS = _Estatus;
                                            Context.Entry(_DesarrolloEstudioMedico).State = System.Data.EntityState.Modified;
                                        }
                                    }
                                }
                                #endregion
                            };

                            if (Entity.PFC_III_PSIQUIATRICO != null)
                            {
                                #region Psiqui
                                _EstudioFueroComun.PFC_III_PSIQUIATRICO = new PFC_III_PSIQUIATRICO()
                                {
                                    A1_ASPECTO_FISICO = Entity.PFC_III_PSIQUIATRICO.A1_ASPECTO_FISICO,
                                    A2_ESTADO_ANIMO = Entity.PFC_III_PSIQUIATRICO.A2_ESTADO_ANIMO,
                                    A3_ALUCINACIONES = Entity.PFC_III_PSIQUIATRICO.A3_ALUCINACIONES,
                                    A4_CURSO = Entity.PFC_III_PSIQUIATRICO.A4_CURSO,
                                    A7_BAJA_TOLERANCIA = Entity.PFC_III_PSIQUIATRICO.A7_BAJA_TOLERANCIA,
                                    B1_CONDUCTA_MOTORA = Entity.PFC_III_PSIQUIATRICO.B1_CONDUCTA_MOTORA,
                                    B2_EXPRESION_AFECTIVA = Entity.PFC_III_PSIQUIATRICO.B2_EXPRESION_AFECTIVA,
                                    B3_ILUSIONES = Entity.PFC_III_PSIQUIATRICO.B3_ILUSIONES,
                                    B4_CONTINUIDAD = Entity.PFC_III_PSIQUIATRICO.B4_CONTINUIDAD,
                                    B7_EXPRESION = Entity.PFC_III_PSIQUIATRICO.B7_EXPRESION,
                                    C1_HABLA = Entity.PFC_III_PSIQUIATRICO.C1_HABLA,
                                    C2_ADECUACION = Entity.PFC_III_PSIQUIATRICO.C2_ADECUACION,
                                    C3_DESPERSONALIZACION = Entity.PFC_III_PSIQUIATRICO.C3_DESPERSONALIZACION,
                                    C4_CONTENIDO = Entity.PFC_III_PSIQUIATRICO.C4_CONTENIDO,
                                    C7_ADECUADA = Entity.PFC_III_PSIQUIATRICO.C7_ADECUADA,
                                    D1_ACTITUD = Entity.PFC_III_PSIQUIATRICO.D1_ACTITUD,
                                    D3_DESREALIZACION = Entity.PFC_III_PSIQUIATRICO.D3_DESREALIZACION,
                                    D4_ABASTRACTO = Entity.PFC_III_PSIQUIATRICO.D4_ABASTRACTO,
                                    E4_CONCENTRACION = Entity.PFC_III_PSIQUIATRICO.E4_CONCENTRACION,
                                    ESTUDIO_FEC = Entity.PFC_III_PSIQUIATRICO.ESTUDIO_FEC,
                                    ID_ANIO = Entity.ID_ANIO,
                                    ID_CENTRO = Entity.ID_CENTRO,
                                    ID_ESTUDIO = Entity.ID_ESTUDIO,
                                    ID_IMPUTADO = Entity.ID_IMPUTADO,
                                    ID_INGRESO = Entity.ID_INGRESO,
                                    P10_FIANILIDAD = Entity.PFC_III_PSIQUIATRICO.P10_FIANILIDAD,
                                    P11_IMPRESION = Entity.PFC_III_PSIQUIATRICO.P11_IMPRESION,
                                    P12_DICTAMEN_PSIQUIATRICO = Entity.PFC_III_PSIQUIATRICO.P12_DICTAMEN_PSIQUIATRICO,
                                    P5_ORIENTACION = Entity.PFC_III_PSIQUIATRICO.P5_ORIENTACION,
                                    P6_MEMORIA = Entity.PFC_III_PSIQUIATRICO.P6_MEMORIA,
                                    P8_CAPACIDAD_JUICIO = Entity.PFC_III_PSIQUIATRICO.P8_CAPACIDAD_JUICIO,
                                    P9_INTROSPECCION = Entity.PFC_III_PSIQUIATRICO.P9_INTROSPECCION,
                                    MEDICO_PSIQUIATRA = Entity.PFC_III_PSIQUIATRICO.MEDICO_PSIQUIATRA,
                                    COORDINADOR = Entity.PFC_III_PSIQUIATRICO.COORDINADOR
                                };

                                #endregion

                                Context.PFC_III_PSIQUIATRICO.Add(_EstudioFueroComun.PFC_III_PSIQUIATRICO);
                                #region Detalle Psiq
                                if (_EstudioPadre != null)
                                {
                                    short? _Estatus = new short?();
                                    var _DetallePsiq = Context.PERSONALIDAD.FirstOrDefault(x => x.ID_ESTUDIO == _EstudioPadre.ID_ESTUDIO && x.ID_ANIO == _EstudioPadre.ID_ANIO && x.ID_CENTRO == _EstudioPadre.ID_CENTRO && x.ID_IMPUTADO == _EstudioPadre.ID_IMPUTADO && x.ID_INGRESO == _EstudioPadre.ID_INGRESO);
                                    if (string.IsNullOrEmpty(Entity.PFC_III_PSIQUIATRICO.A1_ASPECTO_FISICO) || string.IsNullOrEmpty(Entity.PFC_III_PSIQUIATRICO.A2_ESTADO_ANIMO) || string.IsNullOrEmpty(Entity.PFC_III_PSIQUIATRICO.A3_ALUCINACIONES) || string.IsNullOrEmpty(Entity.PFC_III_PSIQUIATRICO.A4_CURSO) || string.IsNullOrEmpty(Entity.PFC_III_PSIQUIATRICO.A7_BAJA_TOLERANCIA) || string.IsNullOrEmpty(Entity.PFC_III_PSIQUIATRICO.B1_CONDUCTA_MOTORA) || string.IsNullOrEmpty(Entity.PFC_III_PSIQUIATRICO.B2_EXPRESION_AFECTIVA) || string.IsNullOrEmpty(Entity.PFC_III_PSIQUIATRICO.B3_ILUSIONES) || string.IsNullOrEmpty(Entity.PFC_III_PSIQUIATRICO.B4_CONTINUIDAD) || string.IsNullOrEmpty(Entity.PFC_III_PSIQUIATRICO.B7_EXPRESION) || string.IsNullOrEmpty(Entity.PFC_III_PSIQUIATRICO.C1_HABLA) || string.IsNullOrEmpty(Entity.PFC_III_PSIQUIATRICO.C2_ADECUACION) || string.IsNullOrEmpty(Entity.PFC_III_PSIQUIATRICO.C3_DESPERSONALIZACION) || string.IsNullOrEmpty(Entity.PFC_III_PSIQUIATRICO.C4_CONTENIDO) || string.IsNullOrEmpty(Entity.PFC_III_PSIQUIATRICO.C7_ADECUADA) || string.IsNullOrEmpty(Entity.PFC_III_PSIQUIATRICO.COORDINADOR) || string.IsNullOrEmpty(Entity.PFC_III_PSIQUIATRICO.D1_ACTITUD) ||
                                        string.IsNullOrEmpty(Entity.PFC_III_PSIQUIATRICO.D3_DESREALIZACION) || string.IsNullOrEmpty(Entity.PFC_III_PSIQUIATRICO.D4_ABASTRACTO) || string.IsNullOrEmpty(Entity.PFC_III_PSIQUIATRICO.E4_CONCENTRACION) || string.IsNullOrEmpty(Entity.PFC_III_PSIQUIATRICO.MEDICO_PSIQUIATRA) || string.IsNullOrEmpty(Entity.PFC_III_PSIQUIATRICO.P10_FIANILIDAD) || string.IsNullOrEmpty(Entity.PFC_III_PSIQUIATRICO.P11_IMPRESION) || string.IsNullOrEmpty(Entity.PFC_III_PSIQUIATRICO.P5_ORIENTACION) || string.IsNullOrEmpty(Entity.PFC_III_PSIQUIATRICO.P6_MEMORIA) || string.IsNullOrEmpty(Entity.PFC_III_PSIQUIATRICO.P8_CAPACIDAD_JUICIO) ||
                                        string.IsNullOrEmpty(Entity.PFC_III_PSIQUIATRICO.P9_INTROSPECCION) || Entity.PFC_III_PSIQUIATRICO.ESTUDIO_FEC == null || Entity.PFC_III_PSIQUIATRICO.P12_DICTAMEN_PSIQUIATRICO == null)
                                        _Estatus = (short)eEstatudDetallePersonalidad.ASIGNADO;
                                    else
                                        _Estatus = (short)eEstatudDetallePersonalidad.TERMINADO;

                                    if (_DetallePsiq != null)
                                    {
                                        var _DesarrolloEstudioPsiq = Context.PERSONALIDAD_DETALLE.FirstOrDefault(c => c.ID_ESTUDIO == _DetallePsiq.ID_ESTUDIO && c.ID_TIPO == (short)eTiposEstudio.PSIQUIATRIA && c.ID_IMPUTADO == _DetallePsiq.ID_IMPUTADO && c.ID_INGRESO == _DetallePsiq.ID_INGRESO && c.ID_CENTRO == _DetallePsiq.ID_CENTRO && c.ID_ANIO == _DetallePsiq.ID_ANIO);
                                        if (_DesarrolloEstudioPsiq == null)
                                        {
                                            short _ConsecutivoPersonalidadDetalle = GetIDProceso<short>("PERSONALIDAD_DETALLE", "ID_DETALLE", "1=1");

                                            var _NvoDetalle = new PERSONALIDAD_DETALLE()
                                            {
                                                DIAS_BONIFICADOS = null,
                                                ID_ANIO = Entity.ID_ANIO,
                                                ID_CENTRO = Entity.ID_CENTRO,
                                                ID_DETALLE = _ConsecutivoPersonalidadDetalle,
                                                ID_ESTUDIO = _EstudioPadre.ID_ESTUDIO,
                                                ID_IMPUTADO = Entity.ID_IMPUTADO,
                                                ID_INGRESO = Entity.ID_INGRESO,
                                                ID_TIPO = (short)eTiposEstudio.PSIQUIATRIA,
                                                INICIO_FEC = GetFechaServerDate(),
                                                RESULTADO = _EstudioFueroComun.PFC_III_PSIQUIATRICO != null ? _EstudioFueroComun.PFC_III_PSIQUIATRICO.P12_DICTAMEN_PSIQUIATRICO.HasValue ? _EstudioFueroComun.PFC_III_PSIQUIATRICO.P12_DICTAMEN_PSIQUIATRICO == (short)eResultado.FAVORABLE ? "S" : _EstudioFueroComun.PFC_III_PSIQUIATRICO.P12_DICTAMEN_PSIQUIATRICO == (short)eResultado.DESFAVORABLE ? "N" : string.Empty : string.Empty : string.Empty,
                                                SOLICITUD_FEC = _EstudioPadre.SOLICITUD_FEC,
                                                ID_ESTATUS = _Estatus,
                                                TERMINO_FEC = _Estatus == (short)eEstatudDetallePersonalidad.TERMINADO ? GetFechaServerDate() : new System.DateTime?(),
                                                TIPO_MEDIA = string.Empty
                                            };

                                            Context.PERSONALIDAD_DETALLE.Add(_NvoDetalle);
                                        }

                                        else
                                        {
                                            _DesarrolloEstudioPsiq.RESULTADO = _EstudioFueroComun.PFC_III_PSIQUIATRICO != null ? _EstudioFueroComun.PFC_III_PSIQUIATRICO.P12_DICTAMEN_PSIQUIATRICO.HasValue ? _EstudioFueroComun.PFC_III_PSIQUIATRICO.P12_DICTAMEN_PSIQUIATRICO == (short)eResultado.FAVORABLE ? "S" : _EstudioFueroComun.PFC_III_PSIQUIATRICO.P12_DICTAMEN_PSIQUIATRICO == (short)eResultado.DESFAVORABLE ? "N" : string.Empty : string.Empty : string.Empty;
                                            _DesarrolloEstudioPsiq.ID_ESTATUS = _Estatus;
                                            Context.Entry(_DesarrolloEstudioPsiq).State = System.Data.EntityState.Modified;
                                        }
                                    }
                                }
                                #endregion
                            };

                            if (Entity.PFC_IV_PSICOLOGICO != null)
                            {
                                #region Psicolog
                                _EstudioFueroComun.PFC_IV_PSICOLOGICO = new PFC_IV_PSICOLOGICO()
                                {
                                    ESTUDIO_FEC = Entity.PFC_IV_PSICOLOGICO.ESTUDIO_FEC,
                                    ID_ANIO = Entity.PFC_IV_PSICOLOGICO.ID_ANIO,
                                    ID_CENTRO = Entity.PFC_IV_PSICOLOGICO.ID_CENTRO,
                                    ID_ESTUDIO = Entity.ID_ESTUDIO,
                                    ID_IMPUTADO = Entity.PFC_IV_PSICOLOGICO.ID_IMPUTADO,
                                    ID_INGRESO = Entity.PFC_IV_PSICOLOGICO.ID_INGRESO,
                                    P1_CONDICIONES_GRALES = Entity.PFC_IV_PSICOLOGICO.P1_CONDICIONES_GRALES,
                                    P10_MOTIVACION_DICTAMEN = Entity.PFC_IV_PSICOLOGICO.P10_MOTIVACION_DICTAMEN,
                                    P11_CASO_NEGATIVO = Entity.PFC_IV_PSICOLOGICO.P11_CASO_NEGATIVO,
                                    P12_CUAL = Entity.PFC_IV_PSICOLOGICO.P12_CUAL,
                                    P12_REQUIERE_TRATAMIENTO = Entity.PFC_IV_PSICOLOGICO.P12_REQUIERE_TRATAMIENTO,
                                    P2_EXAMEN_MENTAL = Entity.PFC_IV_PSICOLOGICO.P2_EXAMEN_MENTAL,
                                    P3_PRINCIPALES_RASGOS = Entity.PFC_IV_PSICOLOGICO.P3_PRINCIPALES_RASGOS,
                                    P4_INVENTARIO_MULTIFASICO = Entity.PFC_IV_PSICOLOGICO.P4_INVENTARIO_MULTIFASICO,
                                    P4_OTRAS = Entity.PFC_IV_PSICOLOGICO.P4_OTRAS,
                                    P4_TEST_GUALTICO = Entity.PFC_IV_PSICOLOGICO.P4_TEST_GUALTICO,
                                    P4_TEST_HTP = Entity.PFC_IV_PSICOLOGICO.P4_TEST_HTP,
                                    P4_TEST_MATRICES = Entity.PFC_IV_PSICOLOGICO.P4_TEST_MATRICES,
                                    P51_NIVEL_INTELECTUAL = Entity.PFC_IV_PSICOLOGICO.P51_NIVEL_INTELECTUAL.HasValue ? Entity.PFC_IV_PSICOLOGICO.P51_NIVEL_INTELECTUAL.Value != -1 ? Entity.PFC_IV_PSICOLOGICO.P51_NIVEL_INTELECTUAL : null : null,
                                    P52_DISFUNCION_NEUROLOGICA = Entity.PFC_IV_PSICOLOGICO.P52_DISFUNCION_NEUROLOGICA.HasValue ? Entity.PFC_IV_PSICOLOGICO.P52_DISFUNCION_NEUROLOGICA.Value != -1 ? Entity.PFC_IV_PSICOLOGICO.P52_DISFUNCION_NEUROLOGICA : null : null,
                                    P9_DICTAMEN_REINSERCION = Entity.PFC_IV_PSICOLOGICO.P9_DICTAMEN_REINSERCION.HasValue ? Entity.PFC_IV_PSICOLOGICO.P9_DICTAMEN_REINSERCION.Value != decimal.Zero ? Entity.PFC_IV_PSICOLOGICO.P9_DICTAMEN_REINSERCION : null : null,
                                    P8_RASGOS_PERSONALIDAD = Entity.PFC_IV_PSICOLOGICO.P8_RASGOS_PERSONALIDAD,
                                    COORDINADOR = Entity.PFC_IV_PSICOLOGICO.COORDINADOR,
                                    P6_INTEGRACION = Entity.PFC_IV_PSICOLOGICO.P6_INTEGRACION,
                                    ELABORO = Entity.PFC_IV_PSICOLOGICO.ELABORO,
                                    P4_OTRA_MENCIONAR = Entity.PFC_IV_PSICOLOGICO.P4_OTRA_MENCIONAR
                                };

                                var _consecutivoPsicologicoComun = GetIDProceso<short>("PFC_IV_PROGRAMA", "ID_CONSEC", string.Format("ID_CENTRO = {0} AND ID_ANIO = {1} AND ID_IMPUTADO = {2} AND ID_INGRESO = {3} AND ID_ESTUDIO = {4}", Entity.ID_CENTRO, Entity.ID_ANIO, Entity.ID_IMPUTADO, Entity.ID_INGRESO, Entity.ID_ESTUDIO));
                                if (Entity.PFC_IV_PSICOLOGICO.PFC_IV_PROGRAMA != null)
                                    if (Entity.PFC_IV_PSICOLOGICO.PFC_IV_PROGRAMA.Any())
                                        foreach (var item in Entity.PFC_IV_PSICOLOGICO.PFC_IV_PROGRAMA)
                                        {
                                            var _NuevoPrograma = new PFC_IV_PROGRAMA()
                                            {
                                                CONCLUYO = item.CONCLUYO,
                                                DURACION = item.DURACION,
                                                ID_ACTIVIDAD = item.ID_ACTIVIDAD,
                                                ID_ANIO = Entity.ID_ANIO,
                                                ID_CENTRO = Entity.ID_CENTRO,
                                                ID_ESTUDIO = Entity.ID_ESTUDIO,
                                                ID_IMPUTADO = Entity.ID_IMPUTADO,
                                                ID_INGRESO = Entity.ID_INGRESO,
                                                ID_TIPO_PROGRAMA = item.ID_TIPO_PROGRAMA,
                                                ID_CONSEC = _consecutivoPsicologicoComun,
                                                OBSERVACION = item.OBSERVACION
                                            };

                                            Context.PFC_IV_PROGRAMA.Add(_NuevoPrograma);
                                            _consecutivoPsicologicoComun++;
                                        };

                                #endregion
                                Context.PFC_IV_PSICOLOGICO.Add(_EstudioFueroComun.PFC_IV_PSICOLOGICO);
                                #region Detalle Psico

                                if (_EstudioPadre != null)
                                {
                                    short? _Estatus = new short?();
                                    var _DetallePsic = Context.PERSONALIDAD.FirstOrDefault(x => x.ID_ESTUDIO == _EstudioPadre.ID_ESTUDIO && x.ID_ANIO == _EstudioPadre.ID_ANIO && x.ID_CENTRO == _EstudioPadre.ID_CENTRO && x.ID_IMPUTADO == _EstudioPadre.ID_IMPUTADO && x.ID_INGRESO == _EstudioPadre.ID_INGRESO);
                                    if (string.IsNullOrEmpty(Entity.PFC_IV_PSICOLOGICO.COORDINADOR) || string.IsNullOrEmpty(Entity.PFC_IV_PSICOLOGICO.ELABORO) || string.IsNullOrEmpty(Entity.PFC_IV_PSICOLOGICO.P1_CONDICIONES_GRALES) || string.IsNullOrEmpty(Entity.PFC_IV_PSICOLOGICO.P10_MOTIVACION_DICTAMEN) || string.IsNullOrEmpty(Entity.PFC_IV_PSICOLOGICO.P2_EXAMEN_MENTAL) || string.IsNullOrEmpty(Entity.PFC_IV_PSICOLOGICO.P3_PRINCIPALES_RASGOS) || string.IsNullOrEmpty(Entity.PFC_IV_PSICOLOGICO.P6_INTEGRACION) || string.IsNullOrEmpty(Entity.PFC_IV_PSICOLOGICO.P8_RASGOS_PERSONALIDAD) ||
                                        Entity.PFC_IV_PSICOLOGICO.ESTUDIO_FEC == null || Entity.PFC_IV_PSICOLOGICO.P51_NIVEL_INTELECTUAL == null || Entity.PFC_IV_PSICOLOGICO.P52_DISFUNCION_NEUROLOGICA == null || Entity.PFC_IV_PSICOLOGICO.P9_DICTAMEN_REINSERCION == null)
                                        _Estatus = (short)eEstatudDetallePersonalidad.ASIGNADO;
                                    else
                                        _Estatus = (short)eEstatudDetallePersonalidad.TERMINADO;


                                    if (_DetallePsic != null)
                                    {
                                        var _DesarrolloEstudioPsiq = Context.PERSONALIDAD_DETALLE.FirstOrDefault(c => c.ID_ESTUDIO == _DetallePsic.ID_ESTUDIO && c.ID_TIPO == (short)eTiposEstudio.PSICOLOGIA && c.ID_IMPUTADO == _DetallePsic.ID_IMPUTADO && c.ID_INGRESO == _DetallePsic.ID_INGRESO && c.ID_CENTRO == _DetallePsic.ID_CENTRO && c.ID_ANIO == _DetallePsic.ID_ANIO);
                                        if (_DesarrolloEstudioPsiq == null)
                                        {
                                            short _ConsecutivoPersonalidadDetalle = GetIDProceso<short>("PERSONALIDAD_DETALLE", "ID_DETALLE", "1=1");

                                            var _NvoDetalle = new PERSONALIDAD_DETALLE()
                                            {
                                                DIAS_BONIFICADOS = null,
                                                ID_ANIO = Entity.ID_ANIO,
                                                ID_CENTRO = Entity.ID_CENTRO,
                                                ID_DETALLE = _ConsecutivoPersonalidadDetalle,
                                                ID_ESTUDIO = _EstudioPadre.ID_ESTUDIO,
                                                ID_IMPUTADO = Entity.ID_IMPUTADO,
                                                ID_INGRESO = Entity.ID_INGRESO,
                                                ID_TIPO = (short)eTiposEstudio.PSICOLOGIA,
                                                INICIO_FEC = GetFechaServerDate(),
                                                RESULTADO = _EstudioFueroComun.PFC_IV_PSICOLOGICO != null ? _EstudioFueroComun.PFC_IV_PSICOLOGICO.P9_DICTAMEN_REINSERCION.HasValue ? _EstudioFueroComun.PFC_IV_PSICOLOGICO.P9_DICTAMEN_REINSERCION == (short)eResultado.FAVORABLE ? "N" : _EstudioFueroComun.PFC_IV_PSICOLOGICO.P9_DICTAMEN_REINSERCION == (short)eResultado.DESFAVORABLE ? "N" : string.Empty : string.Empty : string.Empty,
                                                SOLICITUD_FEC = _EstudioPadre.SOLICITUD_FEC,
                                                ID_ESTATUS = _Estatus,
                                                TERMINO_FEC = _Estatus == (short)eEstatudDetallePersonalidad.TERMINADO ? GetFechaServerDate() : new System.DateTime?(),
                                                TIPO_MEDIA = string.Empty
                                            };

                                            Context.PERSONALIDAD_DETALLE.Add(_NvoDetalle);
                                        }

                                        else
                                        {
                                            _DesarrolloEstudioPsiq.RESULTADO = _EstudioFueroComun.PFC_IV_PSICOLOGICO != null ? _EstudioFueroComun.PFC_IV_PSICOLOGICO.P9_DICTAMEN_REINSERCION.HasValue ? _EstudioFueroComun.PFC_IV_PSICOLOGICO.P9_DICTAMEN_REINSERCION == (short)eResultado.FAVORABLE ? "N" : _EstudioFueroComun.PFC_IV_PSICOLOGICO.P9_DICTAMEN_REINSERCION == (short)eResultado.DESFAVORABLE ? "N" : string.Empty : string.Empty : string.Empty;
                                            _DesarrolloEstudioPsiq.ID_ESTATUS = _Estatus;
                                            Context.Entry(_DesarrolloEstudioPsiq).State = System.Data.EntityState.Modified;
                                        }
                                    }
                                }

                                #endregion

                            };

                            if (Entity.PFC_V_CRIMINODIAGNOSTICO != null)
                            {
                                #region Estudio Crimin
                                _EstudioFueroComun.PFC_V_CRIMINODIAGNOSTICO = new PFC_V_CRIMINODIAGNOSTICO()
                                {
                                    ESTUDIO_FEC = Entity.PFC_V_CRIMINODIAGNOSTICO.ESTUDIO_FEC,
                                    ID_ANIO = Entity.PFC_V_CRIMINODIAGNOSTICO.ID_ANIO,
                                    ID_CENTRO = Entity.PFC_V_CRIMINODIAGNOSTICO.ID_CENTRO,
                                    ID_ESTUDIO = Entity.ID_ESTUDIO,
                                    ID_IMPUTADO = Entity.PFC_V_CRIMINODIAGNOSTICO.ID_IMPUTADO,
                                    ID_INGRESO = Entity.PFC_V_CRIMINODIAGNOSTICO.ID_INGRESO,
                                    P1_ALCOHOL = Entity.PFC_V_CRIMINODIAGNOSTICO.P1_ALCOHOL,
                                    P1_DROGADO = string.IsNullOrEmpty(Entity.PFC_V_CRIMINODIAGNOSTICO.P1_DROGADO) ? Entity.PFC_V_CRIMINODIAGNOSTICO.P1_DROGADO : string.Empty,
                                    P1_DROGRA_ILEGAL = Entity.PFC_V_CRIMINODIAGNOSTICO.P1_DROGRA_ILEGAL,
                                    P1_OTRA = Entity.PFC_V_CRIMINODIAGNOSTICO.P1_OTRA,
                                    P1_VERSION_DELITO = Entity.PFC_V_CRIMINODIAGNOSTICO.P1_VERSION_DELITO,
                                    P10_DICTAMEN_REINSERCION = Entity.PFC_V_CRIMINODIAGNOSTICO.P10_DICTAMEN_REINSERCION.HasValue ? Entity.PFC_V_CRIMINODIAGNOSTICO.P10_DICTAMEN_REINSERCION.Value != decimal.Zero ? Entity.PFC_V_CRIMINODIAGNOSTICO.P10_DICTAMEN_REINSERCION : null : null,
                                    P10_MOTIVACION_DICTAMEN = Entity.PFC_V_CRIMINODIAGNOSTICO.P10_MOTIVACION_DICTAMEN,
                                    P11_PROGRAMAS_REMITIRSE = Entity.PFC_V_CRIMINODIAGNOSTICO.P11_PROGRAMAS_REMITIRSE,
                                    P12_CUAL = Entity.PFC_V_CRIMINODIAGNOSTICO.P12_CUAL,
                                    P12_TRATAMIENTO_EXTRAMUROS = string.IsNullOrEmpty(Entity.PFC_V_CRIMINODIAGNOSTICO.P12_TRATAMIENTO_EXTRAMUROS) ? Entity.PFC_V_CRIMINODIAGNOSTICO.P12_TRATAMIENTO_EXTRAMUROS : string.Empty,
                                    P2_CRIMINOGENESIS = Entity.PFC_V_CRIMINODIAGNOSTICO.P2_CRIMINOGENESIS,
                                    P3_CONDUCTA_ANTISOCIAL = Entity.PFC_V_CRIMINODIAGNOSTICO.P3_CONDUCTA_ANTISOCIAL,
                                    P4_CLASIFICACION_CRIMINOLOGICA = Entity.PFC_V_CRIMINODIAGNOSTICO.P4_CLASIFICACION_CRIMINOLOGICA.HasValue ? Entity.PFC_V_CRIMINODIAGNOSTICO.P4_CLASIFICACION_CRIMINOLOGICA.Value != -1 ? Entity.PFC_V_CRIMINODIAGNOSTICO.P4_CLASIFICACION_CRIMINOLOGICA : null : null,
                                    P5_INTIMIDACION = string.IsNullOrEmpty(Entity.PFC_V_CRIMINODIAGNOSTICO.P5_INTIMIDACION) ? Entity.PFC_V_CRIMINODIAGNOSTICO.P5_INTIMIDACION : string.Empty,
                                    P5_PORQUE = Entity.PFC_V_CRIMINODIAGNOSTICO.P5_PORQUE,
                                    P6_CAPACIDAD_CRIMINAL = Entity.PFC_V_CRIMINODIAGNOSTICO.P6_CAPACIDAD_CRIMINAL.HasValue ? Entity.PFC_V_CRIMINODIAGNOSTICO.P6_CAPACIDAD_CRIMINAL.Value != -1 ? Entity.PFC_V_CRIMINODIAGNOSTICO.P6_CAPACIDAD_CRIMINAL : null : null,
                                    P6A_EGOCENTRICO = Entity.PFC_V_CRIMINODIAGNOSTICO.P6A_EGOCENTRICO.HasValue ? Entity.PFC_V_CRIMINODIAGNOSTICO.P6A_EGOCENTRICO.Value != -1 ? Entity.PFC_V_CRIMINODIAGNOSTICO.P6A_EGOCENTRICO : null : null,
                                    P6B_LIABILIDAD_EFECTIVA = Entity.PFC_V_CRIMINODIAGNOSTICO.P6B_LIABILIDAD_EFECTIVA.HasValue ? Entity.PFC_V_CRIMINODIAGNOSTICO.P6B_LIABILIDAD_EFECTIVA.Value != -1 ? Entity.PFC_V_CRIMINODIAGNOSTICO.P6B_LIABILIDAD_EFECTIVA : null : null,
                                    P6C_AGRESIVIDAD = Entity.PFC_V_CRIMINODIAGNOSTICO.P6C_AGRESIVIDAD.HasValue ? Entity.PFC_V_CRIMINODIAGNOSTICO.P6C_AGRESIVIDAD.Value != -1 ? Entity.PFC_V_CRIMINODIAGNOSTICO.P6C_AGRESIVIDAD : null : null,
                                    P6D_INDIFERENCIA_AFECTIVA = Entity.PFC_V_CRIMINODIAGNOSTICO.P6D_INDIFERENCIA_AFECTIVA.HasValue ? Entity.PFC_V_CRIMINODIAGNOSTICO.P6D_INDIFERENCIA_AFECTIVA.Value != -1 ? Entity.PFC_V_CRIMINODIAGNOSTICO.P6D_INDIFERENCIA_AFECTIVA : null : null,
                                    P7_ADAPTACION_SOCIAL = Entity.PFC_V_CRIMINODIAGNOSTICO.P7_ADAPTACION_SOCIAL.HasValue ? Entity.PFC_V_CRIMINODIAGNOSTICO.P7_ADAPTACION_SOCIAL.Value != -1 ? Entity.PFC_V_CRIMINODIAGNOSTICO.P7_ADAPTACION_SOCIAL : null : null,
                                    P8_INDICE_PELIGROSIDAD = Entity.PFC_V_CRIMINODIAGNOSTICO.P8_INDICE_PELIGROSIDAD.HasValue ? Entity.PFC_V_CRIMINODIAGNOSTICO.P8_INDICE_PELIGROSIDAD.Value != -1 ? Entity.PFC_V_CRIMINODIAGNOSTICO.P8_INDICE_PELIGROSIDAD : null : null,
                                    P9_PRONOSTICO_REINCIDENCIA = Entity.PFC_V_CRIMINODIAGNOSTICO.P9_PRONOSTICO_REINCIDENCIA.HasValue ? Entity.PFC_V_CRIMINODIAGNOSTICO.P9_PRONOSTICO_REINCIDENCIA.Value != -1 ? Entity.PFC_V_CRIMINODIAGNOSTICO.P9_PRONOSTICO_REINCIDENCIA : null : null,
                                    COORDINADOR = Entity.PFC_V_CRIMINODIAGNOSTICO.COORDINADOR,
                                    ELABORO = Entity.PFC_V_CRIMINODIAGNOSTICO.ELABORO
                                };

                                #endregion

                                Context.PFC_V_CRIMINODIAGNOSTICO.Add(_EstudioFueroComun.PFC_V_CRIMINODIAGNOSTICO);

                                #region Detalle Crimin

                                if (_EstudioPadre != null)
                                {
                                    short? _Estatus = new short?();
                                    var _DetallePsic = Context.PERSONALIDAD.FirstOrDefault(x => x.ID_ESTUDIO == _EstudioPadre.ID_ESTUDIO && x.ID_ANIO == _EstudioPadre.ID_ANIO && x.ID_CENTRO == _EstudioPadre.ID_CENTRO && x.ID_IMPUTADO == _EstudioPadre.ID_IMPUTADO && x.ID_INGRESO == _EstudioPadre.ID_INGRESO);
                                    if (string.IsNullOrEmpty(Entity.PFC_V_CRIMINODIAGNOSTICO.COORDINADOR) || string.IsNullOrEmpty(Entity.PFC_V_CRIMINODIAGNOSTICO.ELABORO) || string.IsNullOrEmpty(Entity.PFC_V_CRIMINODIAGNOSTICO.P1_VERSION_DELITO) || string.IsNullOrEmpty(Entity.PFC_V_CRIMINODIAGNOSTICO.P10_MOTIVACION_DICTAMEN) || string.IsNullOrEmpty(Entity.PFC_V_CRIMINODIAGNOSTICO.P2_CRIMINOGENESIS) || string.IsNullOrEmpty(Entity.PFC_V_CRIMINODIAGNOSTICO.P3_CONDUCTA_ANTISOCIAL) || string.IsNullOrEmpty(Entity.PFC_V_CRIMINODIAGNOSTICO.P5_INTIMIDACION) || string.IsNullOrEmpty(Entity.PFC_V_CRIMINODIAGNOSTICO.P5_PORQUE) ||
                                        Entity.PFC_V_CRIMINODIAGNOSTICO.ESTUDIO_FEC == null || Entity.PFC_V_CRIMINODIAGNOSTICO.P10_DICTAMEN_REINSERCION == null || Entity.PFC_V_CRIMINODIAGNOSTICO.P4_CLASIFICACION_CRIMINOLOGICA == null || Entity.PFC_V_CRIMINODIAGNOSTICO.P5_INTIMIDACION == null || Entity.PFC_V_CRIMINODIAGNOSTICO.P6_CAPACIDAD_CRIMINAL == null || Entity.PFC_V_CRIMINODIAGNOSTICO.P6A_EGOCENTRICO == null || Entity.PFC_V_CRIMINODIAGNOSTICO.P6B_LIABILIDAD_EFECTIVA == null
                                         || Entity.PFC_V_CRIMINODIAGNOSTICO.P6C_AGRESIVIDAD == null || Entity.PFC_V_CRIMINODIAGNOSTICO.P6D_INDIFERENCIA_AFECTIVA == null || Entity.PFC_V_CRIMINODIAGNOSTICO.P7_ADAPTACION_SOCIAL == null || Entity.PFC_V_CRIMINODIAGNOSTICO.P8_INDICE_PELIGROSIDAD == null || Entity.PFC_V_CRIMINODIAGNOSTICO.P9_PRONOSTICO_REINCIDENCIA == null)
                                        _Estatus = (short)eEstatudDetallePersonalidad.ASIGNADO;
                                    else
                                        _Estatus = (short)eEstatudDetallePersonalidad.TERMINADO;

                                    if (_DetallePsic != null)
                                    {
                                        var _DesarrolloEstudioCrimi = Context.PERSONALIDAD_DETALLE.FirstOrDefault(c => c.ID_ESTUDIO == _DetallePsic.ID_ESTUDIO && c.ID_TIPO == (short)eTiposEstudio.CRIMINOLOGICO && c.ID_IMPUTADO == _DetallePsic.ID_IMPUTADO && c.ID_INGRESO == _DetallePsic.ID_INGRESO && c.ID_CENTRO == _DetallePsic.ID_CENTRO && c.ID_ANIO == _DetallePsic.ID_ANIO);
                                        if (_DesarrolloEstudioCrimi == null)
                                        {
                                            short _ConsecutivoPersonalidadDetalle = GetIDProceso<short>("PERSONALIDAD_DETALLE", "ID_DETALLE", "1=1");

                                            var _NvoDetalle = new PERSONALIDAD_DETALLE()
                                            {
                                                DIAS_BONIFICADOS = null,
                                                ID_ANIO = Entity.ID_ANIO,
                                                ID_CENTRO = Entity.ID_CENTRO,
                                                ID_DETALLE = _ConsecutivoPersonalidadDetalle,
                                                ID_ESTUDIO = _EstudioPadre.ID_ESTUDIO,
                                                ID_IMPUTADO = Entity.ID_IMPUTADO,
                                                ID_INGRESO = Entity.ID_INGRESO,
                                                ID_TIPO = (short)eTiposEstudio.CRIMINOLOGICO,
                                                INICIO_FEC = GetFechaServerDate(),
                                                RESULTADO = _EstudioFueroComun.PFC_V_CRIMINODIAGNOSTICO != null ? _EstudioFueroComun.PFC_V_CRIMINODIAGNOSTICO.P10_DICTAMEN_REINSERCION.HasValue ? _EstudioFueroComun.PFC_V_CRIMINODIAGNOSTICO.P10_DICTAMEN_REINSERCION == (short)eResultado.FAVORABLE ? "S" : _EstudioFueroComun.PFC_V_CRIMINODIAGNOSTICO.P10_DICTAMEN_REINSERCION == (short)eResultado.DESFAVORABLE ? "N" : string.Empty : string.Empty : string.Empty,
                                                SOLICITUD_FEC = _EstudioPadre.SOLICITUD_FEC,
                                                ID_ESTATUS = _Estatus,
                                                TERMINO_FEC = _Estatus == (short)eEstatudDetallePersonalidad.TERMINADO ? GetFechaServerDate() : new System.DateTime?(),
                                                TIPO_MEDIA = string.Empty
                                            };

                                            Context.PERSONALIDAD_DETALLE.Add(_NvoDetalle);
                                        }

                                        else
                                        {
                                            _DesarrolloEstudioCrimi.RESULTADO = _EstudioFueroComun.PFC_V_CRIMINODIAGNOSTICO != null ? _EstudioFueroComun.PFC_V_CRIMINODIAGNOSTICO.P10_DICTAMEN_REINSERCION.HasValue ? _EstudioFueroComun.PFC_V_CRIMINODIAGNOSTICO.P10_DICTAMEN_REINSERCION == (short)eResultado.FAVORABLE ? "S" : _EstudioFueroComun.PFC_V_CRIMINODIAGNOSTICO.P10_DICTAMEN_REINSERCION == (short)eResultado.DESFAVORABLE ? "N" : string.Empty : string.Empty : string.Empty;
                                            _DesarrolloEstudioCrimi.ID_ESTATUS = _Estatus;
                                            Context.Entry(_DesarrolloEstudioCrimi).State = System.Data.EntityState.Modified;
                                        }
                                    }
                                }

                                #endregion

                            };

                            if (Entity.PFC_VI_SOCIO_FAMILIAR != null)
                            {
                                #region Estudio SocioFam
                                _EstudioFueroComun.PFC_VI_SOCIO_FAMILIAR = new PFC_VI_SOCIO_FAMILIAR()
                                {
                                    ID_ANIO = Entity.PFC_VI_SOCIO_FAMILIAR.ID_ANIO,
                                    ID_CENTRO = Entity.PFC_VI_SOCIO_FAMILIAR.ID_CENTRO,
                                    ID_ESTUDIO = Entity.ID_ESTUDIO,
                                    ID_IMPUTADO = Entity.PFC_VI_SOCIO_FAMILIAR.ID_IMPUTADO,
                                    ID_INGRESO = Entity.PFC_VI_SOCIO_FAMILIAR.ID_INGRESO,
                                    P21_FAMILIA_PRIMARIA = Entity.PFC_VI_SOCIO_FAMILIAR.P21_FAMILIA_PRIMARIA,
                                    P22_FAMILIA_SECUNDARIA = Entity.PFC_VI_SOCIO_FAMILIAR.P22_FAMILIA_SECUNDARIA,
                                    P3_TERCERA_EDAD = Entity.PFC_VI_SOCIO_FAMILIAR.P3_TERCERA_EDAD,
                                    P4_ESPOSOA = Entity.PFC_VI_SOCIO_FAMILIAR.P4_ESPOSOA,
                                    P4_FRECUENCIA = Entity.PFC_VI_SOCIO_FAMILIAR.P4_FRECUENCIA,
                                    P4_HERMANOS = Entity.PFC_VI_SOCIO_FAMILIAR.P4_HERMANOS,
                                    P4_HIJOS = Entity.PFC_VI_SOCIO_FAMILIAR.P4_HIJOS,
                                    P4_MADRE = Entity.PFC_VI_SOCIO_FAMILIAR.P4_MADRE,
                                    P4_MOTIVO_NO_VISITA = Entity.PFC_VI_SOCIO_FAMILIAR.P4_MOTIVO_NO_VISITA,
                                    P4_OTROS = Entity.PFC_VI_SOCIO_FAMILIAR.P4_OTROS,
                                    P4_OTROS_EPECIFICAR = Entity.PFC_VI_SOCIO_FAMILIAR.P4_OTROS_EPECIFICAR,
                                    P4_PADRE = Entity.PFC_VI_SOCIO_FAMILIAR.P4_PADRE,
                                    P4_RECIBE_VISITA = Entity.PFC_VI_SOCIO_FAMILIAR.P4_RECIBE_VISITA,
                                    ESTUDIO_FEC = Entity.PFC_VI_SOCIO_FAMILIAR.ESTUDIO_FEC,
                                    P10_DICTAMEN = Entity.PFC_VI_SOCIO_FAMILIAR.P10_DICTAMEN.HasValue ? Entity.PFC_VI_SOCIO_FAMILIAR.P10_DICTAMEN.Value != decimal.Zero ? Entity.PFC_VI_SOCIO_FAMILIAR.P10_DICTAMEN : null : null,
                                    P11_MOTIVACION_DICTAMEN = Entity.PFC_VI_SOCIO_FAMILIAR.P11_MOTIVACION_DICTAMEN,
                                    P5_COMUNICACION_TELEFONICA = Entity.PFC_VI_SOCIO_FAMILIAR.P5_COMUNICACION_TELEFONICA,
                                    P5_NO_POR_QUE = Entity.PFC_VI_SOCIO_FAMILIAR.P5_NO_POR_QUE,
                                    P6_APOYO_EXTERIOR = Entity.PFC_VI_SOCIO_FAMILIAR.P6_APOYO_EXTERIOR,
                                    P7_PLANES_INTERNO = Entity.PFC_VI_SOCIO_FAMILIAR.P7_PLANES_INTERNO,
                                    P7_VIVIRA = Entity.PFC_VI_SOCIO_FAMILIAR.P7_VIVIRA,
                                    P8_OFERTA_ESPECIFICAR = Entity.PFC_VI_SOCIO_FAMILIAR.P8_OFERTA_ESPECIFICAR,
                                    P8_OFERTA_TRABAJO = Entity.PFC_VI_SOCIO_FAMILIAR.P8_OFERTA_TRABAJO,
                                    P9_AVAL_ESPECIFICAR = Entity.PFC_VI_SOCIO_FAMILIAR.P9_AVAL_ESPECIFICAR,
                                    P9_AVAL_MORAL = Entity.PFC_VI_SOCIO_FAMILIAR.P9_AVAL_MORAL,
                                    COORDINADOR = Entity.PFC_VI_SOCIO_FAMILIAR.COORDINADOR,
                                    ELABORO = Entity.PFC_VI_SOCIO_FAMILIAR.ELABORO
                                };

                                var _consecutivoFamiliares = GetIDProceso<short>("PFC_VI_COMUNICACION", "ID_CONSEC", string.Format("ID_CENTRO = {0} AND ID_ANIO = {1} AND ID_IMPUTADO = {2} AND ID_INGRESO = {3} AND ID_ESTUDIO = {4}", Entity.ID_CENTRO, Entity.ID_ANIO, Entity.ID_IMPUTADO, Entity.ID_INGRESO, Entity.ID_ESTUDIO));
                                if (Entity.PFC_VI_SOCIO_FAMILIAR.PFC_VI_COMUNICACION != null)
                                {
                                    if (Entity.PFC_VI_SOCIO_FAMILIAR.PFC_VI_COMUNICACION.Any())
                                        foreach (var item in Entity.PFC_VI_SOCIO_FAMILIAR.PFC_VI_COMUNICACION)
                                        {
                                            var _NuevaVisita = new PFC_VI_COMUNICACION()
                                            {
                                                ID_ANIO = Entity.ID_ANIO,
                                                FRECUENCIA = item.FRECUENCIA,
                                                ID_CENTRO = Entity.ID_CENTRO,
                                                ID_CONSEC = _consecutivoFamiliares,
                                                ID_ESTUDIO = Entity.ID_ESTUDIO,
                                                ID_IMPUTADO = Entity.ID_IMPUTADO,
                                                ID_INGRESO = Entity.ID_INGRESO,
                                                ID_TIPO_REFERENCIA = item.ID_TIPO_REFERENCIA,
                                                NOMBRE = item.NOMBRE,
                                                TELEFONO = item.TELEFONO
                                            };

                                            Context.PFC_VI_COMUNICACION.Add(_NuevaVisita);
                                            _consecutivoFamiliares++;
                                        };
                                };

                                if (Entity.PFC_VI_SOCIO_FAMILIAR.PFC_VI_GRUPO != null)
                                    if (Entity.PFC_VI_SOCIO_FAMILIAR.PFC_VI_GRUPO.Any())
                                    {
                                        foreach (var item in Entity.PFC_VI_SOCIO_FAMILIAR.PFC_VI_GRUPO)
                                        {
                                            var _NuevoGrupo = new PFC_VI_GRUPO()
                                                {
                                                    CONGREGACION = item.CONGREGACION,
                                                    ID_ACTIVIDAD = item.ID_ACTIVIDAD,
                                                    ID_ANIO = Entity.ID_ANIO,
                                                    ID_CENTRO = Entity.ID_CENTRO,
                                                    ID_ESTUDIO = Entity.ID_ESTUDIO,
                                                    ID_IMPUTADO = Entity.ID_IMPUTADO,
                                                    ID_INGRESO = Entity.ID_INGRESO,
                                                    ID_TIPO_PROGRAMA = item.ID_TIPO_PROGRAMA,
                                                    OBSERVACIONES = item.OBSERVACIONES,
                                                    PERIODO = item.PERIODO
                                                };

                                            Context.PFC_VI_GRUPO.Add(_NuevoGrupo);
                                        };
                                    };

                                #endregion

                                Context.PFC_VI_SOCIO_FAMILIAR.Add(_EstudioFueroComun.PFC_VI_SOCIO_FAMILIAR);

                                #region Detalle Socio
                                if (_EstudioPadre != null)
                                {
                                    short? _Estatus = new short?();
                                    var _DetallePsic = Context.PERSONALIDAD.FirstOrDefault(x => x.ID_ESTUDIO == _EstudioPadre.ID_ESTUDIO && x.ID_ANIO == _EstudioPadre.ID_ANIO && x.ID_CENTRO == _EstudioPadre.ID_CENTRO && x.ID_IMPUTADO == _EstudioPadre.ID_IMPUTADO && x.ID_INGRESO == _EstudioPadre.ID_INGRESO);
                                    if (string.IsNullOrEmpty(Entity.PFC_VI_SOCIO_FAMILIAR.COORDINADOR) || string.IsNullOrEmpty(Entity.PFC_VI_SOCIO_FAMILIAR.ELABORO) || string.IsNullOrEmpty(Entity.PFC_VI_SOCIO_FAMILIAR.P11_MOTIVACION_DICTAMEN) || string.IsNullOrEmpty(Entity.PFC_VI_SOCIO_FAMILIAR.P21_FAMILIA_PRIMARIA) || string.IsNullOrEmpty(Entity.PFC_VI_SOCIO_FAMILIAR.P22_FAMILIA_SECUNDARIA) || string.IsNullOrEmpty(Entity.PFC_VI_SOCIO_FAMILIAR.P3_TERCERA_EDAD) || string.IsNullOrEmpty(Entity.PFC_VI_SOCIO_FAMILIAR.P4_RECIBE_VISITA) || string.IsNullOrEmpty(Entity.PFC_VI_SOCIO_FAMILIAR.P5_COMUNICACION_TELEFONICA)
                                         || string.IsNullOrEmpty(Entity.PFC_VI_SOCIO_FAMILIAR.P6_APOYO_EXTERIOR) || string.IsNullOrEmpty(Entity.PFC_VI_SOCIO_FAMILIAR.P7_PLANES_INTERNO) || string.IsNullOrEmpty(Entity.PFC_VI_SOCIO_FAMILIAR.P7_VIVIRA) || string.IsNullOrEmpty(Entity.PFC_VI_SOCIO_FAMILIAR.P8_OFERTA_TRABAJO) || Entity.PFC_VI_SOCIO_FAMILIAR.ESTUDIO_FEC == null || Entity.PFC_VI_SOCIO_FAMILIAR.P10_DICTAMEN == null)
                                        _Estatus = (short)eEstatudDetallePersonalidad.ASIGNADO;
                                    else
                                        _Estatus = (short)eEstatudDetallePersonalidad.TERMINADO;

                                    if (_DetallePsic != null)
                                    {
                                        var _DesarrolloEstudioCrimi = Context.PERSONALIDAD_DETALLE.FirstOrDefault(c => c.ID_ESTUDIO == _DetallePsic.ID_ESTUDIO && c.ID_TIPO == (short)eTiposEstudio.TRABAJO_SOCIAL && c.ID_IMPUTADO == _DetallePsic.ID_IMPUTADO && c.ID_INGRESO == _DetallePsic.ID_INGRESO && c.ID_CENTRO == _DetallePsic.ID_CENTRO && c.ID_ANIO == _DetallePsic.ID_ANIO);
                                        if (_DesarrolloEstudioCrimi == null)
                                        {
                                            short _ConsecutivoPersonalidadDetalle = GetIDProceso<short>("PERSONALIDAD_DETALLE", "ID_DETALLE", "1=1");

                                            var _NvoDetalle = new PERSONALIDAD_DETALLE()
                                            {
                                                DIAS_BONIFICADOS = null,
                                                ID_ANIO = Entity.ID_ANIO,
                                                ID_CENTRO = Entity.ID_CENTRO,
                                                ID_DETALLE = _ConsecutivoPersonalidadDetalle,
                                                ID_ESTUDIO = _EstudioPadre.ID_ESTUDIO,
                                                ID_IMPUTADO = Entity.ID_IMPUTADO,
                                                ID_INGRESO = Entity.ID_INGRESO,
                                                ID_TIPO = (short)eTiposEstudio.TRABAJO_SOCIAL,
                                                INICIO_FEC = GetFechaServerDate(),
                                                RESULTADO = _EstudioFueroComun.PFC_VI_SOCIO_FAMILIAR != null ? _EstudioFueroComun.PFC_VI_SOCIO_FAMILIAR.P10_DICTAMEN.HasValue ? _EstudioFueroComun.PFC_VI_SOCIO_FAMILIAR.P10_DICTAMEN == (short)eResultado.FAVORABLE ? "S" : _EstudioFueroComun.PFC_VI_SOCIO_FAMILIAR.P10_DICTAMEN == (short)eResultado.DESFAVORABLE ? "N" : string.Empty : string.Empty : string.Empty,
                                                SOLICITUD_FEC = _EstudioPadre.SOLICITUD_FEC,
                                                ID_ESTATUS = _Estatus,
                                                TERMINO_FEC = _Estatus == (short)eEstatudDetallePersonalidad.TERMINADO ? GetFechaServerDate() : new System.DateTime?(),
                                                TIPO_MEDIA = string.Empty,
                                            };

                                            Context.PERSONALIDAD_DETALLE.Add(_NvoDetalle);
                                        }

                                        else
                                        {
                                            _DesarrolloEstudioCrimi.RESULTADO = _EstudioFueroComun.PFC_VI_SOCIO_FAMILIAR != null ? _EstudioFueroComun.PFC_VI_SOCIO_FAMILIAR.P10_DICTAMEN.HasValue ? _EstudioFueroComun.PFC_VI_SOCIO_FAMILIAR.P10_DICTAMEN == (short)eResultado.FAVORABLE ? "S" : _EstudioFueroComun.PFC_VI_SOCIO_FAMILIAR.P10_DICTAMEN == (short)eResultado.DESFAVORABLE ? "N" : string.Empty : string.Empty : string.Empty;
                                            _DesarrolloEstudioCrimi.ID_ESTATUS = _Estatus;
                                            Context.Entry(_DesarrolloEstudioCrimi).State = System.Data.EntityState.Modified;
                                        }
                                    }
                                }

                                #endregion

                            };

                            if (Entity.PFC_VII_EDUCATIVO != null)
                            {
                                #region Estudio Educ
                                _EstudioFueroComun.PFC_VII_EDUCATIVO = new PFC_VII_EDUCATIVO()
                                {
                                    ID_ANIO = Entity.PFC_VII_EDUCATIVO.ID_ANIO,
                                    ID_CENTRO = Entity.PFC_VII_EDUCATIVO.ID_CENTRO,
                                    ID_ESTUDIO = Entity.ID_ESTUDIO,
                                    ID_IMPUTADO = Entity.PFC_VII_EDUCATIVO.ID_IMPUTADO,
                                    ID_INGRESO = Entity.PFC_VII_EDUCATIVO.ID_INGRESO,
                                    COORDINADOR = Entity.PFC_VII_EDUCATIVO.COORDINADOR,
                                    ELABORO = Entity.PFC_VII_EDUCATIVO.ELABORO,
                                    ESTUDIO_FEC = Entity.PFC_VII_EDUCATIVO.ESTUDIO_FEC,
                                    P3_DICTAMEN = Entity.PFC_VII_EDUCATIVO.P3_DICTAMEN.HasValue ? Entity.PFC_VII_EDUCATIVO.P3_DICTAMEN.Value != decimal.Zero ? Entity.PFC_VII_EDUCATIVO.P3_DICTAMEN : null : null,
                                    P4_MOTIVACION_DICTAMEN = Entity.PFC_VII_EDUCATIVO.P4_MOTIVACION_DICTAMEN
                                };

                                #endregion

                                #region Escolaridad y Actividades
                                var _consecutivoTrabajo = GetIDProceso<short>("PFC_VII_ACTIVIDAD", "ID_CONSEC", string.Format("ID_CENTRO = {0} AND ID_ANIO = {1} AND ID_IMPUTADO = {2} AND ID_INGRESO = {3} AND ID_ESTUDIO = {4}", _EstudioPadre.ID_CENTRO, _EstudioPadre.ID_ANIO, _EstudioPadre.ID_IMPUTADO, _EstudioPadre.ID_INGRESO, _EstudioPadre.ID_ESTUDIO));
                                if (Entity.PFC_VII_EDUCATIVO.PFC_VII_ACTIVIDAD != null && Entity.PFC_VII_EDUCATIVO.PFC_VII_ACTIVIDAD.Any())
                                    foreach (var item in Entity.PFC_VII_EDUCATIVO.PFC_VII_ACTIVIDAD)
                                    {
                                        var _NuevaActividad = new PFC_VII_ACTIVIDAD()
                                        {
                                            ACTIVIDAD = item.ACTIVIDAD,
                                            DURACION = item.DURACION,
                                            ID_ANIO = Entity.ID_ANIO,
                                            ID_IMPUTADO = Entity.ID_IMPUTADO,
                                            ID_ESTUDIO = Entity.ID_ESTUDIO,
                                            ID_CENTRO = Entity.ID_CENTRO,
                                            ID_INGRESO = Entity.ID_INGRESO,
                                            ID_PROGRAMA = item.ID_PROGRAMA,
                                            OBSERVACION = item.OBSERVACION,
                                            TIPO = item.TIPO,
                                            ID_ACTIVIDAD = item.ID_ACTIVIDAD,
                                            ID_CONSEC = _consecutivoTrabajo,
                                            ID_TIPO_PROGRAMA = item.ID_TIPO_PROGRAMA
                                        };

                                        Context.PFC_VII_ACTIVIDAD.Add(_NuevaActividad);
                                        _consecutivoTrabajo++;
                                    };


                                var _consecutivoEduca = GetIDProceso<short>("PFC_VII_ESCOLARIDAD_ANTERIOR", "ID_CONSEC", string.Format("ID_CENTRO = {0} AND ID_ANIO = {1} AND ID_IMPUTADO = {2} AND ID_INGRESO = {3} AND ID_ESTUDIO = {4}", Entity.ID_CENTRO, Entity.ID_ANIO, Entity.ID_IMPUTADO, Entity.ID_INGRESO, Entity.ID_ESTUDIO));

                                if (Entity.PFC_VII_EDUCATIVO.PFC_VII_ESCOLARIDAD_ANTERIOR != null && Entity.PFC_VII_EDUCATIVO.PFC_VII_ESCOLARIDAD_ANTERIOR.Any())
                                    foreach (var item in Entity.PFC_VII_EDUCATIVO.PFC_VII_ESCOLARIDAD_ANTERIOR)
                                    {
                                        var _NuevaEscolaridad = new PFC_VII_ESCOLARIDAD_ANTERIOR()
                                        {
                                            CONCLUIDA = item.CONCLUIDA,
                                            ID_ANIO = Entity.ID_ANIO,
                                            ID_CENTRO = Entity.ID_CENTRO,
                                            ID_ESTUDIO = Entity.ID_ESTUDIO,
                                            ID_GRADO = item.ID_GRADO,
                                            ID_IMPUTADO = Entity.ID_IMPUTADO,
                                            ID_INGRESO = Entity.ID_INGRESO,
                                            INTERES = item.INTERES,
                                            OBSERVACION = item.OBSERVACION,
                                            RENDIMIENTO = item.RENDIMIENTO,
                                            ID_CONSEC = _consecutivoEduca
                                        };

                                        Context.PFC_VII_ESCOLARIDAD_ANTERIOR.Add(_NuevaEscolaridad);
                                        _consecutivoEduca++;
                                    };

                                #endregion

                                Context.PFC_VII_EDUCATIVO.Add(_EstudioFueroComun.PFC_VII_EDUCATIVO);

                                #region Detalle Estudio Educ
                                if (_EstudioPadre != null)
                                {
                                    short? _Estatus = new short?();
                                    var _DetalleEduc = Context.PERSONALIDAD.FirstOrDefault(x => x.ID_ESTUDIO == _EstudioPadre.ID_ESTUDIO && x.ID_ANIO == _EstudioPadre.ID_ANIO && x.ID_CENTRO == _EstudioPadre.ID_CENTRO && x.ID_IMPUTADO == _EstudioPadre.ID_IMPUTADO && x.ID_INGRESO == _EstudioPadre.ID_INGRESO);
                                    if (string.IsNullOrEmpty(Entity.PFC_VII_EDUCATIVO.COORDINADOR) || string.IsNullOrEmpty(Entity.PFC_VII_EDUCATIVO.ELABORO) || string.IsNullOrEmpty(Entity.PFC_VII_EDUCATIVO.P4_MOTIVACION_DICTAMEN) || Entity.PFC_VII_EDUCATIVO.ESTUDIO_FEC == null || Entity.PFC_VII_EDUCATIVO.P3_DICTAMEN == null)
                                        _Estatus = (short)eEstatudDetallePersonalidad.ASIGNADO;
                                    else
                                        _Estatus = (short)eEstatudDetallePersonalidad.TERMINADO;

                                    if (_DetalleEduc != null)
                                    {
                                        var _DesarrolloEstudioCrimi = Context.PERSONALIDAD_DETALLE.FirstOrDefault(c => c.ID_ESTUDIO == _DetalleEduc.ID_ESTUDIO && c.ID_TIPO == (short)eTiposEstudio.PEDAGOGIA && c.ID_IMPUTADO == _DetalleEduc.ID_IMPUTADO && c.ID_INGRESO == _DetalleEduc.ID_INGRESO && c.ID_CENTRO == _DetalleEduc.ID_CENTRO && c.ID_ANIO == _DetalleEduc.ID_ANIO);
                                        if (_DesarrolloEstudioCrimi == null)
                                        {
                                            short _ConsecutivoPersonalidadDetalle = GetIDProceso<short>("PERSONALIDAD_DETALLE", "ID_DETALLE", "1=1");
                                            var _NvoDetalle = new PERSONALIDAD_DETALLE()
                                            {
                                                DIAS_BONIFICADOS = null,
                                                ID_ANIO = Entity.ID_ANIO,
                                                ID_CENTRO = Entity.ID_CENTRO,
                                                ID_DETALLE = _ConsecutivoPersonalidadDetalle,
                                                ID_ESTUDIO = _EstudioPadre.ID_ESTUDIO,
                                                ID_IMPUTADO = Entity.ID_IMPUTADO,
                                                ID_INGRESO = Entity.ID_INGRESO,
                                                ID_TIPO = (short)eTiposEstudio.PEDAGOGIA,
                                                INICIO_FEC = GetFechaServerDate(),
                                                RESULTADO = _EstudioFueroComun.PFC_VII_EDUCATIVO != null ? _EstudioFueroComun.PFC_VII_EDUCATIVO.P3_DICTAMEN.HasValue ? _EstudioFueroComun.PFC_VII_EDUCATIVO.P3_DICTAMEN == (short)eResultado.FAVORABLE ? "S" : _EstudioFueroComun.PFC_VII_EDUCATIVO.P3_DICTAMEN == (short)eResultado.DESFAVORABLE ? "N" : string.Empty : string.Empty : string.Empty,
                                                SOLICITUD_FEC = _EstudioPadre.SOLICITUD_FEC,
                                                ID_ESTATUS = _Estatus,
                                                TERMINO_FEC = _Estatus == (short)eEstatudDetallePersonalidad.TERMINADO ? GetFechaServerDate() : new System.DateTime?(),
                                                TIPO_MEDIA = string.Empty
                                            };

                                            Context.PERSONALIDAD_DETALLE.Add(_NvoDetalle);
                                        }

                                        else
                                        {
                                            _DesarrolloEstudioCrimi.RESULTADO = _EstudioFueroComun.PFC_VII_EDUCATIVO != null ? _EstudioFueroComun.PFC_VII_EDUCATIVO.P3_DICTAMEN.HasValue ? _EstudioFueroComun.PFC_VII_EDUCATIVO.P3_DICTAMEN == (short)eResultado.FAVORABLE ? "S" : _EstudioFueroComun.PFC_VII_EDUCATIVO.P3_DICTAMEN == (short)eResultado.DESFAVORABLE ? "N" : string.Empty : string.Empty : string.Empty;
                                            _DesarrolloEstudioCrimi.ID_ESTATUS = _Estatus;
                                            Context.Entry(_DesarrolloEstudioCrimi).State = System.Data.EntityState.Modified;
                                        }
                                    }
                                }
                                #endregion
                            };

                            if (Entity.PFC_VIII_TRABAJO != null)//actividad laboral
                            {
                                #region Estudio Trabajo
                                _EstudioFueroComun.PFC_VIII_TRABAJO = new PFC_VIII_TRABAJO()
                                {
                                    COORDINADOR = Entity.PFC_VIII_TRABAJO.COORDINADOR,
                                    ELABORO = Entity.PFC_VIII_TRABAJO.ELABORO,
                                    ESTUDIO_FEC = Entity.PFC_VIII_TRABAJO.ESTUDIO_FEC,
                                    ID_ANIO = Entity.PFC_VIII_TRABAJO.ID_ANIO,
                                    ID_CENTRO = Entity.PFC_VIII_TRABAJO.ID_CENTRO,
                                    ID_ESTUDIO = Entity.ID_ESTUDIO,
                                    ID_IMPUTADO = Entity.PFC_VIII_TRABAJO.ID_IMPUTADO,
                                    ID_INGRESO = Entity.PFC_VIII_TRABAJO.ID_INGRESO,
                                    P1_TRABAJO_ANTES = Entity.PFC_VIII_TRABAJO.P1_TRABAJO_ANTES,
                                    P3_CALIDAD = Entity.PFC_VIII_TRABAJO.P3_CALIDAD,
                                    P3_PERSEVERANCIA = Entity.PFC_VIII_TRABAJO.P3_PERSEVERANCIA,
                                    P3_RESPONSABILIDAD = Entity.PFC_VIII_TRABAJO.P3_RESPONSABILIDAD,
                                    P4_FONDO_AHORRO = Entity.PFC_VIII_TRABAJO.P4_FONDO_AHORRO,
                                    P5_DIAS_CENTRO_ACTUAL = Entity.PFC_VIII_TRABAJO.P5_DIAS_CENTRO_ACTUAL,
                                    P5_DIAS_LABORADOS = Entity.PFC_VIII_TRABAJO.P5_DIAS_LABORADOS,
                                    P5_DIAS_OTROS_CENTROS = Entity.PFC_VIII_TRABAJO.P5_DIAS_OTROS_CENTROS,
                                    P5_PERIODO_LABORAL = Entity.PFC_VIII_TRABAJO.P5_PERIODO_LABORAL,
                                    P6_DICTAMEN = Entity.PFC_VIII_TRABAJO.P6_DICTAMEN,
                                    P7_MOTIVACION = Entity.PFC_VIII_TRABAJO.P7_MOTIVACION
                                };

                                #endregion

                                var _consecutivoLaboral = GetIDProceso<short>("PFC_VIII_ACTIVIDAD_LABORAL", "ID_CONSEC", string.Format("ID_CENTRO = {0} AND ID_ANIO = {1} AND ID_IMPUTADO = {2} AND ID_INGRESO = {3} AND ID_ESTUDIO = {4}", Entity.ID_CENTRO, Entity.ID_ANIO, Entity.ID_IMPUTADO, Entity.ID_INGRESO, Entity.ID_ESTUDIO));

                                if (Entity.PFC_VIII_TRABAJO.PFC_VIII_ACTIVIDAD_LABORAL != null && Entity.PFC_VIII_TRABAJO.PFC_VIII_ACTIVIDAD_LABORAL.Any())
                                    foreach (var item in Entity.PFC_VIII_TRABAJO.PFC_VIII_ACTIVIDAD_LABORAL)
                                    {
                                        var NvaActividadLaboral = new PFC_VIII_ACTIVIDAD_LABORAL()
                                        {
                                            CONCLUYO = item.CONCLUYO,
                                            ID_ANIO = Entity.ID_ANIO,
                                            ID_CAPACITACION = null,
                                            ID_CENTRO = Entity.ID_CENTRO,
                                            ID_CONSEC = _consecutivoLaboral,
                                            ID_ACTIVIDAD = item.ID_ACTIVIDAD,
                                            ID_TIPO_PROGRAMA = item.ID_TIPO_PROGRAMA,
                                            ID_ESTUDIO = Entity.ID_ESTUDIO,
                                            ID_IMPUTADO = Entity.ID_IMPUTADO,
                                            ID_INGRESO = Entity.ID_INGRESO,
                                            OBSERVACION = item.OBSERVACION,
                                            PERIODO = item.PERIODO
                                        };

                                        Context.PFC_VIII_ACTIVIDAD_LABORAL.Add(NvaActividadLaboral);
                                        _consecutivoLaboral++;
                                    };

                                Context.PFC_VIII_TRABAJO.Add(_EstudioFueroComun.PFC_VIII_TRABAJO);

                                #region Detalle Estudio Trabajo
                                if (_EstudioPadre != null)
                                {
                                    short? _Estatus = new short?();
                                    var _DetalleEduc = Context.PERSONALIDAD.FirstOrDefault(x => x.ID_ESTUDIO == _EstudioPadre.ID_ESTUDIO && x.ID_ANIO == _EstudioPadre.ID_ANIO && x.ID_CENTRO == _EstudioPadre.ID_CENTRO && x.ID_IMPUTADO == _EstudioPadre.ID_IMPUTADO && x.ID_INGRESO == _EstudioPadre.ID_INGRESO);
                                    if (string.IsNullOrEmpty(Entity.PFC_VIII_TRABAJO.COORDINADOR) || string.IsNullOrEmpty(Entity.PFC_VIII_TRABAJO.ELABORO) || string.IsNullOrEmpty(Entity.PFC_VIII_TRABAJO.P1_TRABAJO_ANTES) || string.IsNullOrEmpty(Entity.PFC_VIII_TRABAJO.P3_CALIDAD)
                                        || string.IsNullOrEmpty(Entity.PFC_VIII_TRABAJO.P3_PERSEVERANCIA) || string.IsNullOrEmpty(Entity.PFC_VIII_TRABAJO.P3_RESPONSABILIDAD) || string.IsNullOrEmpty(Entity.PFC_VIII_TRABAJO.P4_FONDO_AHORRO) || string.IsNullOrEmpty(Entity.PFC_VIII_TRABAJO.P6_DICTAMEN)
                                        || string.IsNullOrEmpty(Entity.PFC_VIII_TRABAJO.P7_MOTIVACION) || Entity.PFC_VIII_TRABAJO.ESTUDIO_FEC == null || Entity.PFC_VIII_TRABAJO.P5_DIAS_CENTRO_ACTUAL == null || Entity.PFC_VIII_TRABAJO.P5_DIAS_LABORADOS == null || Entity.PFC_VIII_TRABAJO.P5_DIAS_OTROS_CENTROS == null
                                        || Entity.PFC_VIII_TRABAJO.P5_PERIODO_LABORAL == null)
                                        _Estatus = (short)eEstatudDetallePersonalidad.ASIGNADO;
                                    else
                                        _Estatus = (short)eEstatudDetallePersonalidad.TERMINADO;

                                    if (_DetalleEduc != null)
                                    {
                                        var _DesarrolloEstudioCrimi = Context.PERSONALIDAD_DETALLE.FirstOrDefault(c => c.ID_ESTUDIO == _DetalleEduc.ID_ESTUDIO && c.ID_TIPO == (short)eTiposEstudio.LABORAL && c.ID_IMPUTADO == _DetalleEduc.ID_IMPUTADO && c.ID_INGRESO == _DetalleEduc.ID_INGRESO && c.ID_CENTRO == _DetalleEduc.ID_CENTRO && c.ID_ANIO == _DetalleEduc.ID_ANIO);
                                        if (_DesarrolloEstudioCrimi == null)
                                        {
                                            short _ConsecutivoPersonalidadDetalle = GetIDProceso<short>("PERSONALIDAD_DETALLE", "ID_DETALLE", "1=1");
                                            var _NvoDetalle = new PERSONALIDAD_DETALLE()
                                            {
                                                DIAS_BONIFICADOS = null,
                                                ID_ANIO = Entity.ID_ANIO,
                                                ID_CENTRO = Entity.ID_CENTRO,
                                                ID_DETALLE = _ConsecutivoPersonalidadDetalle,
                                                ID_ESTUDIO = _EstudioPadre.ID_ESTUDIO,
                                                ID_IMPUTADO = Entity.ID_IMPUTADO,
                                                ID_INGRESO = Entity.ID_INGRESO,
                                                ID_TIPO = (short)eTiposEstudio.LABORAL,
                                                INICIO_FEC = GetFechaServerDate(),
                                                RESULTADO = _EstudioFueroComun.PFC_VIII_TRABAJO.P6_DICTAMEN == "S" ? "S" : _EstudioFueroComun.PFC_VIII_TRABAJO.P6_DICTAMEN == "D" ? "N" : string.Empty,
                                                SOLICITUD_FEC = _EstudioPadre.SOLICITUD_FEC,
                                                ID_ESTATUS = _Estatus,
                                                TERMINO_FEC = _Estatus == (short)eEstatudDetallePersonalidad.TERMINADO ? GetFechaServerDate() : new System.DateTime?(),
                                                TIPO_MEDIA = string.Empty
                                            };

                                            Context.PERSONALIDAD_DETALLE.Add(_NvoDetalle);
                                        }

                                        else
                                        {
                                            _DesarrolloEstudioCrimi.RESULTADO = _EstudioFueroComun.PFC_VIII_TRABAJO.P6_DICTAMEN == "S" ? "S" : _EstudioFueroComun.PFC_VIII_TRABAJO.P6_DICTAMEN == "D" ? "N" : string.Empty;
                                            _DesarrolloEstudioCrimi.ID_ESTATUS = _Estatus;
                                            Context.Entry(_DesarrolloEstudioCrimi).State = System.Data.EntityState.Modified;
                                        }
                                    }
                                }

                                #endregion
                            };

                            if (Entity.PFC_IX_SEGURIDAD != null)
                            {
                                #region Estudio Seguridad
                                _EstudioFueroComun.PFC_IX_SEGURIDAD = new PFC_IX_SEGURIDAD()
                                {
                                    COMANDANTE = Entity.PFC_IX_SEGURIDAD.COMANDANTE,
                                    ELABORO = Entity.PFC_IX_SEGURIDAD.ELABORO,
                                    ESTUDIO_FEC = Entity.PFC_IX_SEGURIDAD.ESTUDIO_FEC,
                                    ID_ANIO = Entity.PFC_IX_SEGURIDAD.ID_ANIO,
                                    ID_CENTRO = Entity.PFC_IX_SEGURIDAD.ID_CENTRO,
                                    ID_ESTUDIO = Entity.ID_ESTUDIO,
                                    ID_IMPUTADO = Entity.PFC_IX_SEGURIDAD.ID_IMPUTADO,
                                    ID_INGRESO = Entity.PFC_IX_SEGURIDAD.ID_INGRESO,
                                    P1_CONDUCTA_CENTRO = Entity.PFC_IX_SEGURIDAD.P1_CONDUCTA_CENTRO,
                                    P2_CONDUCTA_AUTORIDAD = Entity.PFC_IX_SEGURIDAD.P2_CONDUCTA_AUTORIDAD,
                                    P3_CONDUCTA_GENERAL = Entity.PFC_IX_SEGURIDAD.P3_CONDUCTA_GENERAL.HasValue ? Entity.PFC_IX_SEGURIDAD.P3_CONDUCTA_GENERAL.Value != -1 ? Entity.PFC_IX_SEGURIDAD.P3_CONDUCTA_GENERAL : null : null,
                                    P4_RELACION_COMPANEROS = Entity.PFC_IX_SEGURIDAD.P4_RELACION_COMPANEROS.HasValue ? Entity.PFC_IX_SEGURIDAD.P4_RELACION_COMPANEROS.Value != -1 ? Entity.PFC_IX_SEGURIDAD.P4_RELACION_COMPANEROS : null : null,
                                    P5_CORRECTIVOS = Entity.PFC_IX_SEGURIDAD.P5_CORRECTIVOS,
                                    P6_OPINION_CONDUCTA = Entity.PFC_IX_SEGURIDAD.P6_OPINION_CONDUCTA,
                                    P7_DICTAMEN = Entity.PFC_IX_SEGURIDAD.P7_DICTAMEN,
                                    P8_MOTIVACION = Entity.PFC_IX_SEGURIDAD.P8_MOTIVACION
                                };

                                var _consecutivoCorrectivos = GetIDProceso<short>("PFC_IX_CORRECTIVO", "ID_CORRECTIVO", string.Format("ID_CENTRO = {0} AND ID_ANIO = {1} AND ID_IMPUTADO = {2} AND ID_INGRESO = {3} AND ID_ESTUDIO = {4}", Entity.ID_CENTRO, Entity.ID_ANIO, Entity.ID_IMPUTADO, Entity.ID_INGRESO, Entity.ID_ESTUDIO));
                                if (Entity.PFC_IX_SEGURIDAD.PFC_IX_CORRECTIVO != null && Entity.PFC_IX_SEGURIDAD.PFC_IX_CORRECTIVO.Any())
                                    foreach (var item in Entity.PFC_IX_SEGURIDAD.PFC_IX_CORRECTIVO)
                                    {
                                        var _correctivo = new PFC_IX_CORRECTIVO()
                                        {
                                            CORRECTIVO_FEC = item.CORRECTIVO_FEC,
                                            SANCION = item.SANCION,
                                            ID_ANIO = Entity.ID_ANIO,
                                            ID_ESTUDIO = Entity.ID_ESTUDIO,
                                            ID_INGRESO = Entity.ID_INGRESO,
                                            MOTIVO = item.MOTIVO,
                                            ID_IMPUTADO = Entity.ID_IMPUTADO,
                                            ID_CENTRO = Entity.ID_CENTRO,
                                            ID_CORRECTIVO = _consecutivoCorrectivos
                                        };

                                        Context.PFC_IX_CORRECTIVO.Add(_correctivo);
                                        _consecutivoCorrectivos++;
                                    }
                                #endregion
                                Context.PFC_IX_SEGURIDAD.Add(_EstudioFueroComun.PFC_IX_SEGURIDAD);

                                #region Detalle Seguridad

                                if (_EstudioPadre != null)
                                {
                                    short? _Estatus = new short?();
                                    var _DetalleSegu = Context.PERSONALIDAD.FirstOrDefault(x => x.ID_ESTUDIO == _EstudioPadre.ID_ESTUDIO && x.ID_ANIO == _EstudioPadre.ID_ANIO && x.ID_CENTRO == _EstudioPadre.ID_CENTRO && x.ID_IMPUTADO == _EstudioPadre.ID_IMPUTADO && x.ID_INGRESO == _EstudioPadre.ID_INGRESO);
                                    if (string.IsNullOrEmpty(Entity.PFC_IX_SEGURIDAD.COMANDANTE) || string.IsNullOrEmpty(Entity.PFC_IX_SEGURIDAD.ELABORO) || string.IsNullOrEmpty(Entity.PFC_IX_SEGURIDAD.P1_CONDUCTA_CENTRO) || string.IsNullOrEmpty(Entity.PFC_IX_SEGURIDAD.P2_CONDUCTA_AUTORIDAD)
                                        || string.IsNullOrEmpty(Entity.PFC_IX_SEGURIDAD.P5_CORRECTIVOS) || string.IsNullOrEmpty(Entity.PFC_IX_SEGURIDAD.P6_OPINION_CONDUCTA) || string.IsNullOrEmpty(Entity.PFC_IX_SEGURIDAD.P7_DICTAMEN) || string.IsNullOrEmpty(Entity.PFC_IX_SEGURIDAD.P8_MOTIVACION)
                                        || Entity.PFC_IX_SEGURIDAD.ESTUDIO_FEC == null || Entity.PFC_IX_SEGURIDAD.P3_CONDUCTA_GENERAL == null || Entity.PFC_IX_SEGURIDAD.P4_RELACION_COMPANEROS == null)
                                        _Estatus = (short)eEstatudDetallePersonalidad.ASIGNADO;
                                    else
                                        _Estatus = (short)eEstatudDetallePersonalidad.TERMINADO;

                                    if (_DetalleSegu != null)
                                    {
                                        var _DesarrolloEstudioSegur = Context.PERSONALIDAD_DETALLE.FirstOrDefault(c => c.ID_ESTUDIO == _DetalleSegu.ID_ESTUDIO && c.ID_TIPO == (short)eTiposEstudio.SEGURIDAD && c.ID_IMPUTADO == _DetalleSegu.ID_IMPUTADO && c.ID_INGRESO == _DetalleSegu.ID_INGRESO && c.ID_CENTRO == _DetalleSegu.ID_CENTRO && c.ID_ANIO == _DetalleSegu.ID_ANIO);
                                        if (_DesarrolloEstudioSegur == null)
                                        {
                                            short _ConsecutivoPersonalidadDetalle = GetIDProceso<short>("PERSONALIDAD_DETALLE", "ID_DETALLE", "1=1");
                                            var _NvoDetalle = new PERSONALIDAD_DETALLE()
                                            {
                                                DIAS_BONIFICADOS = null,
                                                ID_ANIO = Entity.ID_ANIO,
                                                ID_CENTRO = Entity.ID_CENTRO,
                                                ID_DETALLE = _ConsecutivoPersonalidadDetalle,
                                                ID_ESTUDIO = _EstudioPadre.ID_ESTUDIO,
                                                ID_IMPUTADO = Entity.ID_IMPUTADO,
                                                ID_INGRESO = Entity.ID_INGRESO,
                                                ID_TIPO = (short)eTiposEstudio.SEGURIDAD,
                                                INICIO_FEC = GetFechaServerDate(),
                                                RESULTADO = _EstudioFueroComun.PFC_IX_SEGURIDAD.P7_DICTAMEN == "S" ? "S" : _EstudioFueroComun.PFC_IX_SEGURIDAD.P7_DICTAMEN == "D" ? "N" : string.Empty,
                                                SOLICITUD_FEC = _EstudioPadre.SOLICITUD_FEC,
                                                ID_ESTATUS = _Estatus,
                                                TERMINO_FEC = _Estatus == (short)eEstatudDetallePersonalidad.TERMINADO ? GetFechaServerDate() : new System.DateTime?(),
                                                TIPO_MEDIA = string.Empty
                                            };

                                            Context.PERSONALIDAD_DETALLE.Add(_NvoDetalle);
                                        }

                                        else
                                        {
                                            _DesarrolloEstudioSegur.RESULTADO = _EstudioFueroComun.PFC_IX_SEGURIDAD.P7_DICTAMEN == "S" ? "S" : _EstudioFueroComun.PFC_IX_SEGURIDAD.P7_DICTAMEN == "D" ? "N" : string.Empty;
                                            _DesarrolloEstudioSegur.ID_ESTATUS = _Estatus;
                                            Context.Entry(_DesarrolloEstudioSegur).State = System.Data.EntityState.Modified;
                                        }
                                    }
                                }
                                #endregion

                            };

                            Context.SaveChanges();
                            transaccion.Complete();
                            return true;
                        }
                        #endregion
                        #region Modificar
                        else
                        {

                            #region Edicion Medico Comun
                            if (Entity.PFC_II_MEDICO != null)
                            {
                                var _Medico = Context.PFC_II_MEDICO.FirstOrDefault(x => x.ID_ESTUDIO == ValidaEstudio.ID_ESTUDIO && x.ID_INGRESO == Entity.ID_INGRESO && x.ID_IMPUTADO == Entity.ID_IMPUTADO && x.ID_CENTRO == Entity.ID_CENTRO);
                                if (_Medico != null)
                                {
                                    _Medico.COORDINADOR = Entity.PFC_II_MEDICO.COORDINADOR;
                                    _Medico.ELABORO = Entity.PFC_II_MEDICO.ELABORO;
                                    _Medico.ESTUDIO_FEC = Entity.PFC_II_MEDICO.ESTUDIO_FEC;
                                    _Medico.ID_ANIO = Entity.PFC_II_MEDICO.ID_ANIO;
                                    _Medico.ID_CENTRO = Entity.PFC_II_MEDICO.ID_CENTRO;
                                    _Medico.ID_ESTUDIO = ValidaEstudio.ID_ESTUDIO;
                                    _Medico.ID_IMPUTADO = Entity.PFC_II_MEDICO.ID_IMPUTADO;
                                    _Medico.ID_INGRESO = Entity.PFC_II_MEDICO.ID_INGRESO;
                                    _Medico.P2_HEREDO_FAMILIARES = Entity.PFC_II_MEDICO.P2_HEREDO_FAMILIARES;
                                    _Medico.P3_ANTPER_NOPATO = Entity.PFC_II_MEDICO.P3_ANTPER_NOPATO;
                                    _Medico.P31_CONSUMO_TOXICO = Entity.PFC_II_MEDICO.P31_CONSUMO_TOXICO;
                                    _Medico.P32_TATUAJES_CICATRICES = Entity.PFC_II_MEDICO.P32_TATUAJES_CICATRICES;
                                    _Medico.P4_PATOLOGICOS = Entity.PFC_II_MEDICO.P4_PATOLOGICOS;
                                    _Medico.P5_PADECIMIENTOS = Entity.PFC_II_MEDICO.P5_PADECIMIENTOS;
                                    _Medico.P6_EXPLORACION_FISICA = Entity.PFC_II_MEDICO.P6_EXPLORACION_FISICA;
                                    _Medico.P7_IMPRESION_DIAGNOSTICA = Entity.PFC_II_MEDICO.P7_IMPRESION_DIAGNOSTICA;
                                    _Medico.P8_DICTAMEN_MEDICO = Entity.PFC_II_MEDICO.P8_DICTAMEN_MEDICO.HasValue ? Entity.PFC_II_MEDICO.P8_DICTAMEN_MEDICO != decimal.Zero ? Entity.PFC_II_MEDICO.P8_DICTAMEN_MEDICO : null : null;
                                    _Medico.SIGNOS_ESTATURA = Entity.PFC_II_MEDICO.SIGNOS_ESTATURA;
                                    _Medico.SIGNOS_PESO = Entity.PFC_II_MEDICO.SIGNOS_PESO;
                                    _Medico.SIGNOS_PULSO = Entity.PFC_II_MEDICO.SIGNOS_PULSO;
                                    _Medico.SIGNOS_RESPIRACION = Entity.PFC_II_MEDICO.SIGNOS_RESPIRACION;
                                    _Medico.SIGNOS_TA = Entity.PFC_II_MEDICO.SIGNOS_TA;
                                    _Medico.SIGNOS_TEMPERATURA = Entity.PFC_II_MEDICO.SIGNOS_TEMPERATURA;
                                    Context.Entry(_Medico).State = System.Data.EntityState.Modified;
                                }

                                else
                                {
                                    ValidaEstudio.PFC_II_MEDICO = new PFC_II_MEDICO()
                                    {
                                        COORDINADOR = Entity.PFC_II_MEDICO.COORDINADOR,
                                        ELABORO = Entity.PFC_II_MEDICO.ELABORO,
                                        ESTUDIO_FEC = Entity.PFC_II_MEDICO.ESTUDIO_FEC,
                                        ID_ANIO = Entity.ID_ANIO,
                                        ID_CENTRO = Entity.ID_CENTRO,
                                        ID_ESTUDIO = Entity.ID_ESTUDIO,
                                        ID_IMPUTADO = Entity.ID_IMPUTADO,
                                        ID_INGRESO = Entity.ID_INGRESO,
                                        P2_HEREDO_FAMILIARES = Entity.PFC_II_MEDICO.P2_HEREDO_FAMILIARES,
                                        P3_ANTPER_NOPATO = Entity.PFC_II_MEDICO.P3_ANTPER_NOPATO,
                                        P31_CONSUMO_TOXICO = Entity.PFC_II_MEDICO.P31_CONSUMO_TOXICO,
                                        P32_TATUAJES_CICATRICES = Entity.PFC_II_MEDICO.P32_TATUAJES_CICATRICES,
                                        P4_PATOLOGICOS = Entity.PFC_II_MEDICO.P4_PATOLOGICOS,
                                        P5_PADECIMIENTOS = Entity.PFC_II_MEDICO.P5_PADECIMIENTOS,
                                        P6_EXPLORACION_FISICA = Entity.PFC_II_MEDICO.P6_EXPLORACION_FISICA,
                                        P7_IMPRESION_DIAGNOSTICA = Entity.PFC_II_MEDICO.P7_IMPRESION_DIAGNOSTICA,
                                        P8_DICTAMEN_MEDICO = Entity.PFC_II_MEDICO.P8_DICTAMEN_MEDICO != decimal.Zero ? Entity.PFC_II_MEDICO.P8_DICTAMEN_MEDICO : null,
                                        SIGNOS_ESTATURA = Entity.PFC_II_MEDICO.SIGNOS_ESTATURA,
                                        SIGNOS_PESO = Entity.PFC_II_MEDICO.SIGNOS_PESO,
                                        SIGNOS_PULSO = Entity.PFC_II_MEDICO.SIGNOS_PULSO,
                                        SIGNOS_RESPIRACION = Entity.PFC_II_MEDICO.SIGNOS_RESPIRACION,
                                        SIGNOS_TA = Entity.PFC_II_MEDICO.SIGNOS_TA,
                                        SIGNOS_TEMPERATURA = Entity.PFC_II_MEDICO.SIGNOS_TEMPERATURA
                                    };

                                    Context.PFC_II_MEDICO.Add(ValidaEstudio.PFC_II_MEDICO);
                                }

                                #region Detalle Medico
                                if (_EstudioPadre != null && Entity.PFC_II_MEDICO != null)
                                {
                                    short? _Estatus = new short?();
                                    var _DetalleMedico = Context.PERSONALIDAD.FirstOrDefault(x => x.ID_ESTUDIO == _EstudioPadre.ID_ESTUDIO && x.ID_IMPUTADO == Entity.ID_IMPUTADO && x.ID_INGRESO == Entity.ID_INGRESO && x.ID_CENTRO == Entity.ID_CENTRO);
                                    if (string.IsNullOrEmpty(Entity.PFC_II_MEDICO.COORDINADOR) || string.IsNullOrEmpty(Entity.PFC_II_MEDICO.ELABORO) || string.IsNullOrEmpty(Entity.PFC_II_MEDICO.P2_HEREDO_FAMILIARES)
                                        || string.IsNullOrEmpty(Entity.PFC_II_MEDICO.P3_ANTPER_NOPATO) || string.IsNullOrEmpty(Entity.PFC_II_MEDICO.P31_CONSUMO_TOXICO) || string.IsNullOrEmpty(Entity.PFC_II_MEDICO.P32_TATUAJES_CICATRICES)
                                        || string.IsNullOrEmpty(Entity.PFC_II_MEDICO.P4_PATOLOGICOS) || string.IsNullOrEmpty(Entity.PFC_II_MEDICO.P5_PADECIMIENTOS)
                                        || string.IsNullOrEmpty(Entity.PFC_II_MEDICO.P7_IMPRESION_DIAGNOSTICA) || string.IsNullOrEmpty(Entity.PFC_II_MEDICO.SIGNOS_ESTATURA)
                                        || string.IsNullOrEmpty(Entity.PFC_II_MEDICO.SIGNOS_PESO) || string.IsNullOrEmpty(Entity.PFC_II_MEDICO.SIGNOS_PULSO)
                                        || string.IsNullOrEmpty(Entity.PFC_II_MEDICO.SIGNOS_RESPIRACION) || string.IsNullOrEmpty(Entity.PFC_II_MEDICO.SIGNOS_TA)
                                        || string.IsNullOrEmpty(Entity.PFC_II_MEDICO.SIGNOS_TEMPERATURA) || Entity.PFC_II_MEDICO.ESTUDIO_FEC == null
                                        || Entity.PFC_II_MEDICO.P8_DICTAMEN_MEDICO == null)
                                        _Estatus = (short)eEstatudDetallePersonalidad.ASIGNADO;
                                    else
                                        _Estatus = (short)eEstatudDetallePersonalidad.TERMINADO;

                                    if (_DetalleMedico != null)
                                    {
                                        var _DesarrolloEstudioMedico = Context.PERSONALIDAD_DETALLE.FirstOrDefault(c => c.ID_ESTUDIO == _DetalleMedico.ID_ESTUDIO && c.ID_TIPO == (short)eTiposEstudio.MEDICO && c.ID_ANIO == _DetalleMedico.ID_ANIO && c.ID_IMPUTADO == _DetalleMedico.ID_IMPUTADO && c.ID_INGRESO == _DetalleMedico.ID_INGRESO && c.ID_CENTRO == _DetalleMedico.ID_CENTRO);
                                        if (_DesarrolloEstudioMedico == null)
                                        {//No se le habia generado aun el detalle del estudio medico
                                            short _ConsecutivoPersonalidadDetalle = GetIDProceso<short>("PERSONALIDAD_DETALLE", "ID_DETALLE", "1=1");
                                            var _NvoDetalle = new PERSONALIDAD_DETALLE()
                                            {
                                                DIAS_BONIFICADOS = null,
                                                ID_ANIO = Entity.ID_ANIO,
                                                ID_CENTRO = Entity.ID_CENTRO,
                                                ID_DETALLE = _ConsecutivoPersonalidadDetalle,
                                                ID_ESTUDIO = _EstudioPadre.ID_ESTUDIO,
                                                ID_IMPUTADO = Entity.ID_IMPUTADO,
                                                ID_INGRESO = Entity.ID_INGRESO,
                                                ID_TIPO = (short)eTiposEstudio.MEDICO,
                                                INICIO_FEC = GetFechaServerDate(),
                                                RESULTADO = ValidaEstudio.PFC_II_MEDICO != null ? ValidaEstudio.PFC_II_MEDICO.P8_DICTAMEN_MEDICO.HasValue ? ValidaEstudio.PFC_II_MEDICO.P8_DICTAMEN_MEDICO == (short)eResultado.FAVORABLE ? "S" : ValidaEstudio.PFC_II_MEDICO.P8_DICTAMEN_MEDICO == (short)eResultado.DESFAVORABLE ? "N" : string.Empty : string.Empty : string.Empty,
                                                SOLICITUD_FEC = _EstudioPadre.SOLICITUD_FEC,
                                                ID_ESTATUS = _Estatus,
                                                TERMINO_FEC = _Estatus == (short)eEstatudDetallePersonalidad.TERMINADO ? GetFechaServerDate() : new System.DateTime?(),
                                                TIPO_MEDIA = string.Empty,
                                            };

                                            Context.PERSONALIDAD_DETALLE.Add(_NvoDetalle);
                                        }

                                        else
                                        {
                                            _DesarrolloEstudioMedico.RESULTADO = ValidaEstudio.PFC_II_MEDICO != null ? ValidaEstudio.PFC_II_MEDICO.P8_DICTAMEN_MEDICO.HasValue ? ValidaEstudio.PFC_II_MEDICO.P8_DICTAMEN_MEDICO == (short)eResultado.FAVORABLE ? "S" : ValidaEstudio.PFC_II_MEDICO.P8_DICTAMEN_MEDICO == (short)eResultado.DESFAVORABLE ? "N" : string.Empty : string.Empty : string.Empty;
                                            _DesarrolloEstudioMedico.ID_ESTATUS = _Estatus;
                                            Context.Entry(_DesarrolloEstudioMedico).State = System.Data.EntityState.Modified;
                                        }
                                    }
                                };
                                #endregion

                            }

                            #endregion


                            #region Psiquiatrico Edicion Comun
                            if (Entity.PFC_III_PSIQUIATRICO != null)
                            {
                                var _Psiquiatrico = Context.PFC_III_PSIQUIATRICO.FirstOrDefault(x => x.ID_ESTUDIO == ValidaEstudio.ID_ESTUDIO && x.ID_INGRESO == Entity.ID_INGRESO && x.ID_IMPUTADO == Entity.ID_IMPUTADO && x.ID_CENTRO == Entity.ID_CENTRO);
                                if (_Psiquiatrico != null)
                                {
                                    _Psiquiatrico.A1_ASPECTO_FISICO = Entity.PFC_III_PSIQUIATRICO.A1_ASPECTO_FISICO;
                                    _Psiquiatrico.A2_ESTADO_ANIMO = Entity.PFC_III_PSIQUIATRICO.A2_ESTADO_ANIMO;
                                    _Psiquiatrico.A3_ALUCINACIONES = Entity.PFC_III_PSIQUIATRICO.A3_ALUCINACIONES;
                                    _Psiquiatrico.A4_CURSO = Entity.PFC_III_PSIQUIATRICO.A4_CURSO;
                                    _Psiquiatrico.A7_BAJA_TOLERANCIA = Entity.PFC_III_PSIQUIATRICO.A7_BAJA_TOLERANCIA;
                                    _Psiquiatrico.B1_CONDUCTA_MOTORA = Entity.PFC_III_PSIQUIATRICO.B1_CONDUCTA_MOTORA;
                                    _Psiquiatrico.B2_EXPRESION_AFECTIVA = Entity.PFC_III_PSIQUIATRICO.B2_EXPRESION_AFECTIVA;
                                    _Psiquiatrico.B3_ILUSIONES = Entity.PFC_III_PSIQUIATRICO.B3_ILUSIONES;
                                    _Psiquiatrico.B4_CONTINUIDAD = Entity.PFC_III_PSIQUIATRICO.B4_CONTINUIDAD;
                                    _Psiquiatrico.B7_EXPRESION = Entity.PFC_III_PSIQUIATRICO.B7_EXPRESION;
                                    _Psiquiatrico.C1_HABLA = Entity.PFC_III_PSIQUIATRICO.C1_HABLA;
                                    _Psiquiatrico.C2_ADECUACION = Entity.PFC_III_PSIQUIATRICO.C2_ADECUACION;
                                    _Psiquiatrico.C3_DESPERSONALIZACION = Entity.PFC_III_PSIQUIATRICO.C3_DESPERSONALIZACION;
                                    _Psiquiatrico.C4_CONTENIDO = Entity.PFC_III_PSIQUIATRICO.C4_CONTENIDO;
                                    _Psiquiatrico.C7_ADECUADA = Entity.PFC_III_PSIQUIATRICO.C7_ADECUADA;
                                    _Psiquiatrico.COORDINADOR = Entity.PFC_III_PSIQUIATRICO.COORDINADOR;
                                    _Psiquiatrico.D1_ACTITUD = Entity.PFC_III_PSIQUIATRICO.D1_ACTITUD;
                                    _Psiquiatrico.D3_DESREALIZACION = Entity.PFC_III_PSIQUIATRICO.D3_DESREALIZACION;
                                    _Psiquiatrico.D4_ABASTRACTO = Entity.PFC_III_PSIQUIATRICO.D4_ABASTRACTO;
                                    _Psiquiatrico.E4_CONCENTRACION = Entity.PFC_III_PSIQUIATRICO.E4_CONCENTRACION;
                                    _Psiquiatrico.ESTUDIO_FEC = Entity.PFC_III_PSIQUIATRICO.ESTUDIO_FEC;
                                    _Psiquiatrico.ID_ANIO = Entity.PFC_III_PSIQUIATRICO.ID_ANIO;
                                    _Psiquiatrico.ID_CENTRO = Entity.PFC_III_PSIQUIATRICO.ID_CENTRO;
                                    _Psiquiatrico.ID_ESTUDIO = ValidaEstudio.ID_ESTUDIO;
                                    _Psiquiatrico.ID_IMPUTADO = Entity.PFC_III_PSIQUIATRICO.ID_IMPUTADO;
                                    _Psiquiatrico.ID_INGRESO = Entity.PFC_III_PSIQUIATRICO.ID_INGRESO;
                                    _Psiquiatrico.MEDICO_PSIQUIATRA = Entity.PFC_III_PSIQUIATRICO.MEDICO_PSIQUIATRA;
                                    _Psiquiatrico.P10_FIANILIDAD = Entity.PFC_III_PSIQUIATRICO.P10_FIANILIDAD;
                                    _Psiquiatrico.P11_IMPRESION = Entity.PFC_III_PSIQUIATRICO.P11_IMPRESION;
                                    _Psiquiatrico.P12_DICTAMEN_PSIQUIATRICO = Entity.PFC_III_PSIQUIATRICO.P12_DICTAMEN_PSIQUIATRICO;
                                    _Psiquiatrico.P5_ORIENTACION = Entity.PFC_III_PSIQUIATRICO.P5_ORIENTACION;
                                    _Psiquiatrico.P6_MEMORIA = Entity.PFC_III_PSIQUIATRICO.P6_MEMORIA;
                                    _Psiquiatrico.P8_CAPACIDAD_JUICIO = Entity.PFC_III_PSIQUIATRICO.P8_CAPACIDAD_JUICIO;
                                    _Psiquiatrico.P9_INTROSPECCION = Entity.PFC_III_PSIQUIATRICO.P9_INTROSPECCION;
                                    Context.Entry(_Psiquiatrico).State = System.Data.EntityState.Modified;
                                }

                                else
                                {
                                    ValidaEstudio.PFC_III_PSIQUIATRICO = new PFC_III_PSIQUIATRICO()
                                    {
                                        A1_ASPECTO_FISICO = Entity.PFC_III_PSIQUIATRICO.A1_ASPECTO_FISICO,
                                        A2_ESTADO_ANIMO = Entity.PFC_III_PSIQUIATRICO.A2_ESTADO_ANIMO,
                                        A3_ALUCINACIONES = Entity.PFC_III_PSIQUIATRICO.A3_ALUCINACIONES,
                                        A4_CURSO = Entity.PFC_III_PSIQUIATRICO.A4_CURSO,
                                        A7_BAJA_TOLERANCIA = Entity.PFC_III_PSIQUIATRICO.A7_BAJA_TOLERANCIA,
                                        B1_CONDUCTA_MOTORA = Entity.PFC_III_PSIQUIATRICO.B1_CONDUCTA_MOTORA,
                                        B2_EXPRESION_AFECTIVA = Entity.PFC_III_PSIQUIATRICO.B2_EXPRESION_AFECTIVA,
                                        B3_ILUSIONES = Entity.PFC_III_PSIQUIATRICO.B3_ILUSIONES,
                                        B4_CONTINUIDAD = Entity.PFC_III_PSIQUIATRICO.B4_CONTINUIDAD,
                                        B7_EXPRESION = Entity.PFC_III_PSIQUIATRICO.B7_EXPRESION,
                                        C1_HABLA = Entity.PFC_III_PSIQUIATRICO.C1_HABLA,
                                        C2_ADECUACION = Entity.PFC_III_PSIQUIATRICO.C2_ADECUACION,
                                        C3_DESPERSONALIZACION = Entity.PFC_III_PSIQUIATRICO.C3_DESPERSONALIZACION,
                                        C4_CONTENIDO = Entity.PFC_III_PSIQUIATRICO.C4_CONTENIDO,
                                        C7_ADECUADA = Entity.PFC_III_PSIQUIATRICO.C7_ADECUADA,
                                        COORDINADOR = Entity.PFC_III_PSIQUIATRICO.COORDINADOR,
                                        D1_ACTITUD = Entity.PFC_III_PSIQUIATRICO.D1_ACTITUD,
                                        D3_DESREALIZACION = Entity.PFC_III_PSIQUIATRICO.D3_DESREALIZACION,
                                        D4_ABASTRACTO = Entity.PFC_III_PSIQUIATRICO.D4_ABASTRACTO,
                                        E4_CONCENTRACION = Entity.PFC_III_PSIQUIATRICO.E4_CONCENTRACION,
                                        ESTUDIO_FEC = Entity.PFC_III_PSIQUIATRICO.ESTUDIO_FEC,
                                        ID_ANIO = Entity.PFC_III_PSIQUIATRICO.ID_ANIO,
                                        ID_CENTRO = Entity.PFC_III_PSIQUIATRICO.ID_CENTRO,
                                        ID_ESTUDIO = ValidaEstudio.ID_ESTUDIO,
                                        ID_IMPUTADO = Entity.PFC_III_PSIQUIATRICO.ID_IMPUTADO,
                                        ID_INGRESO = Entity.PFC_III_PSIQUIATRICO.ID_INGRESO,
                                        MEDICO_PSIQUIATRA = Entity.PFC_III_PSIQUIATRICO.MEDICO_PSIQUIATRA,
                                        P10_FIANILIDAD = Entity.PFC_III_PSIQUIATRICO.P10_FIANILIDAD,
                                        P11_IMPRESION = Entity.PFC_III_PSIQUIATRICO.P11_IMPRESION,
                                        P12_DICTAMEN_PSIQUIATRICO = Entity.PFC_III_PSIQUIATRICO.P12_DICTAMEN_PSIQUIATRICO,
                                        P5_ORIENTACION = Entity.PFC_III_PSIQUIATRICO.P5_ORIENTACION,
                                        P6_MEMORIA = Entity.PFC_III_PSIQUIATRICO.P6_MEMORIA,
                                        P8_CAPACIDAD_JUICIO = Entity.PFC_III_PSIQUIATRICO.P8_CAPACIDAD_JUICIO,
                                        P9_INTROSPECCION = Entity.PFC_III_PSIQUIATRICO.P9_INTROSPECCION
                                    };

                                    Context.PFC_III_PSIQUIATRICO.Add(ValidaEstudio.PFC_III_PSIQUIATRICO);
                                }

                                #region Detalle
                                if (_EstudioPadre != null && Entity.PFC_III_PSIQUIATRICO != null)
                                {
                                    short? _Estatus = new short?();
                                    var _DetallePsiq = Context.PERSONALIDAD.FirstOrDefault(x => x.ID_ESTUDIO == _EstudioPadre.ID_ESTUDIO && x.ID_ANIO == _EstudioPadre.ID_ANIO && x.ID_CENTRO == _EstudioPadre.ID_CENTRO && x.ID_IMPUTADO == _EstudioPadre.ID_IMPUTADO && x.ID_INGRESO == _EstudioPadre.ID_INGRESO);
                                    if (string.IsNullOrEmpty(Entity.PFC_III_PSIQUIATRICO.A1_ASPECTO_FISICO) || string.IsNullOrEmpty(Entity.PFC_III_PSIQUIATRICO.A2_ESTADO_ANIMO) || string.IsNullOrEmpty(Entity.PFC_III_PSIQUIATRICO.A3_ALUCINACIONES) || string.IsNullOrEmpty(Entity.PFC_III_PSIQUIATRICO.A4_CURSO) || string.IsNullOrEmpty(Entity.PFC_III_PSIQUIATRICO.A7_BAJA_TOLERANCIA) || string.IsNullOrEmpty(Entity.PFC_III_PSIQUIATRICO.B1_CONDUCTA_MOTORA) || string.IsNullOrEmpty(Entity.PFC_III_PSIQUIATRICO.B2_EXPRESION_AFECTIVA) || string.IsNullOrEmpty(Entity.PFC_III_PSIQUIATRICO.B3_ILUSIONES) || string.IsNullOrEmpty(Entity.PFC_III_PSIQUIATRICO.B4_CONTINUIDAD) || string.IsNullOrEmpty(Entity.PFC_III_PSIQUIATRICO.B7_EXPRESION) || string.IsNullOrEmpty(Entity.PFC_III_PSIQUIATRICO.C1_HABLA) || string.IsNullOrEmpty(Entity.PFC_III_PSIQUIATRICO.C2_ADECUACION) || string.IsNullOrEmpty(Entity.PFC_III_PSIQUIATRICO.C3_DESPERSONALIZACION) || string.IsNullOrEmpty(Entity.PFC_III_PSIQUIATRICO.C4_CONTENIDO) || string.IsNullOrEmpty(Entity.PFC_III_PSIQUIATRICO.C7_ADECUADA) || string.IsNullOrEmpty(Entity.PFC_III_PSIQUIATRICO.COORDINADOR) || string.IsNullOrEmpty(Entity.PFC_III_PSIQUIATRICO.D1_ACTITUD) ||
                                        string.IsNullOrEmpty(Entity.PFC_III_PSIQUIATRICO.D3_DESREALIZACION) || string.IsNullOrEmpty(Entity.PFC_III_PSIQUIATRICO.D4_ABASTRACTO) || string.IsNullOrEmpty(Entity.PFC_III_PSIQUIATRICO.E4_CONCENTRACION) || string.IsNullOrEmpty(Entity.PFC_III_PSIQUIATRICO.MEDICO_PSIQUIATRA) || string.IsNullOrEmpty(Entity.PFC_III_PSIQUIATRICO.P10_FIANILIDAD) || string.IsNullOrEmpty(Entity.PFC_III_PSIQUIATRICO.P11_IMPRESION) || string.IsNullOrEmpty(Entity.PFC_III_PSIQUIATRICO.P5_ORIENTACION) || string.IsNullOrEmpty(Entity.PFC_III_PSIQUIATRICO.P6_MEMORIA) || string.IsNullOrEmpty(Entity.PFC_III_PSIQUIATRICO.P8_CAPACIDAD_JUICIO) ||
                                        string.IsNullOrEmpty(Entity.PFC_III_PSIQUIATRICO.P9_INTROSPECCION) || Entity.PFC_III_PSIQUIATRICO.ESTUDIO_FEC == null || Entity.PFC_III_PSIQUIATRICO.P12_DICTAMEN_PSIQUIATRICO == null)
                                        _Estatus = (short)eEstatudDetallePersonalidad.ASIGNADO;
                                    else
                                        _Estatus = (short)eEstatudDetallePersonalidad.TERMINADO;

                                    if (_DetallePsiq != null)
                                    {
                                        var _DesarrolloEstudioPsiq = Context.PERSONALIDAD_DETALLE.FirstOrDefault(c => c.ID_ESTUDIO == _DetallePsiq.ID_ESTUDIO && c.ID_TIPO == (short)eTiposEstudio.PSIQUIATRIA && c.ID_IMPUTADO == _DetallePsiq.ID_IMPUTADO && c.ID_INGRESO == _DetallePsiq.ID_INGRESO && c.ID_CENTRO == _DetallePsiq.ID_CENTRO && c.ID_ANIO == _DetallePsiq.ID_ANIO);
                                        if (_DesarrolloEstudioPsiq == null)
                                        {
                                            short _ConsecutivoPersonalidadDetalle = GetIDProceso<short>("PERSONALIDAD_DETALLE", "ID_DETALLE", "1=1");
                                            var _NvoDetalle = new PERSONALIDAD_DETALLE()
                                            {
                                                DIAS_BONIFICADOS = null,
                                                ID_ANIO = Entity.ID_ANIO,
                                                ID_CENTRO = Entity.ID_CENTRO,
                                                ID_DETALLE = _ConsecutivoPersonalidadDetalle,
                                                ID_ESTUDIO = _EstudioPadre.ID_ESTUDIO,
                                                ID_IMPUTADO = Entity.ID_IMPUTADO,
                                                ID_INGRESO = Entity.ID_INGRESO,
                                                ID_TIPO = (short)eTiposEstudio.PSIQUIATRIA,
                                                INICIO_FEC = GetFechaServerDate(),
                                                RESULTADO = ValidaEstudio.PFC_III_PSIQUIATRICO != null ? ValidaEstudio.PFC_III_PSIQUIATRICO.P12_DICTAMEN_PSIQUIATRICO.HasValue ? ValidaEstudio.PFC_III_PSIQUIATRICO.P12_DICTAMEN_PSIQUIATRICO == (short)eResultado.FAVORABLE ? "S" : ValidaEstudio.PFC_III_PSIQUIATRICO.P12_DICTAMEN_PSIQUIATRICO == (short)eResultado.DESFAVORABLE ? "N" : string.Empty : string.Empty : string.Empty,
                                                SOLICITUD_FEC = _EstudioPadre.SOLICITUD_FEC,
                                                ID_ESTATUS = _Estatus,
                                                TERMINO_FEC = _Estatus == (short)eEstatudDetallePersonalidad.TERMINADO ? GetFechaServerDate() : new System.DateTime?(),
                                                TIPO_MEDIA = string.Empty,
                                            };

                                            Context.PERSONALIDAD_DETALLE.Add(_NvoDetalle);
                                        }

                                        else
                                        {
                                            _DesarrolloEstudioPsiq.RESULTADO = ValidaEstudio.PFC_III_PSIQUIATRICO != null ? ValidaEstudio.PFC_III_PSIQUIATRICO.P12_DICTAMEN_PSIQUIATRICO.HasValue ? ValidaEstudio.PFC_III_PSIQUIATRICO.P12_DICTAMEN_PSIQUIATRICO == (short)eResultado.FAVORABLE ? "S" : ValidaEstudio.PFC_III_PSIQUIATRICO.P12_DICTAMEN_PSIQUIATRICO == (short)eResultado.DESFAVORABLE ? "N" : string.Empty : string.Empty : string.Empty;
                                            _DesarrolloEstudioPsiq.ID_ESTATUS = _Estatus;
                                            Context.Entry(_DesarrolloEstudioPsiq).State = System.Data.EntityState.Modified;
                                        }
                                    }
                                }
                                #endregion
                            }

                            #endregion




                            #region Psicologico Comun Edicion
                            if (Entity.PFC_IV_PSICOLOGICO != null)
                            {
                                var _Psicologico = Context.PFC_IV_PSICOLOGICO.FirstOrDefault(x => x.ID_ESTUDIO == ValidaEstudio.ID_ESTUDIO && x.ID_IMPUTADO == Entity.ID_IMPUTADO && x.ID_INGRESO == Entity.ID_INGRESO && x.ID_CENTRO == Entity.ID_CENTRO);
                                var _ProgramasPsicologico = Context.PFC_IV_PROGRAMA.Where(x => x.ID_IMPUTADO == Entity.ID_IMPUTADO && x.ID_INGRESO == Entity.ID_INGRESO && x.ID_ESTUDIO == ValidaEstudio.ID_ESTUDIO && x.ID_CENTRO == Entity.ID_CENTRO);
                                if (_Psicologico != null)
                                {
                                    _Psicologico.COORDINADOR = Entity.PFC_IV_PSICOLOGICO.COORDINADOR;
                                    _Psicologico.ELABORO = Entity.PFC_IV_PSICOLOGICO.ELABORO;
                                    _Psicologico.ESTUDIO_FEC = Entity.PFC_IV_PSICOLOGICO.ESTUDIO_FEC;
                                    _Psicologico.ID_ANIO = Entity.PFC_IV_PSICOLOGICO.ID_ANIO;
                                    _Psicologico.ID_CENTRO = Entity.PFC_IV_PSICOLOGICO.ID_CENTRO;
                                    _Psicologico.ID_ESTUDIO = ValidaEstudio.ID_ESTUDIO;
                                    _Psicologico.ID_IMPUTADO = Entity.PFC_IV_PSICOLOGICO.ID_IMPUTADO;
                                    _Psicologico.ID_INGRESO = Entity.PFC_IV_PSICOLOGICO.ID_INGRESO;
                                    _Psicologico.P1_CONDICIONES_GRALES = Entity.PFC_IV_PSICOLOGICO.P1_CONDICIONES_GRALES;
                                    _Psicologico.P10_MOTIVACION_DICTAMEN = Entity.PFC_IV_PSICOLOGICO.P10_MOTIVACION_DICTAMEN;
                                    _Psicologico.P11_CASO_NEGATIVO = Entity.PFC_IV_PSICOLOGICO.P11_CASO_NEGATIVO;
                                    _Psicologico.P12_CUAL = Entity.PFC_IV_PSICOLOGICO.P12_CUAL;
                                    _Psicologico.P12_REQUIERE_TRATAMIENTO = string.IsNullOrEmpty(Entity.PFC_IV_PSICOLOGICO.P12_REQUIERE_TRATAMIENTO) ? Entity.PFC_IV_PSICOLOGICO.P12_REQUIERE_TRATAMIENTO : string.Empty;
                                    _Psicologico.P2_EXAMEN_MENTAL = Entity.PFC_IV_PSICOLOGICO.P2_EXAMEN_MENTAL;
                                    _Psicologico.P3_PRINCIPALES_RASGOS = Entity.PFC_IV_PSICOLOGICO.P3_PRINCIPALES_RASGOS;
                                    _Psicologico.P4_INVENTARIO_MULTIFASICO = Entity.PFC_IV_PSICOLOGICO.P4_INVENTARIO_MULTIFASICO;
                                    _Psicologico.P4_OTRAS = Entity.PFC_IV_PSICOLOGICO.P4_OTRAS;
                                    _Psicologico.P4_OTRA_MENCIONAR = Entity.PFC_IV_PSICOLOGICO.P4_OTRA_MENCIONAR;
                                    _Psicologico.P4_TEST_GUALTICO = Entity.PFC_IV_PSICOLOGICO.P4_TEST_GUALTICO;
                                    _Psicologico.P4_TEST_HTP = Entity.PFC_IV_PSICOLOGICO.P4_TEST_HTP;
                                    _Psicologico.P4_TEST_MATRICES = Entity.PFC_IV_PSICOLOGICO.P4_TEST_MATRICES;
                                    _Psicologico.P51_NIVEL_INTELECTUAL = Entity.PFC_IV_PSICOLOGICO.P51_NIVEL_INTELECTUAL.HasValue ? Entity.PFC_IV_PSICOLOGICO.P51_NIVEL_INTELECTUAL != -1 ? Entity.PFC_IV_PSICOLOGICO.P51_NIVEL_INTELECTUAL : null : null;
                                    _Psicologico.P52_DISFUNCION_NEUROLOGICA = Entity.PFC_IV_PSICOLOGICO.P52_DISFUNCION_NEUROLOGICA.HasValue ? Entity.PFC_IV_PSICOLOGICO.P52_DISFUNCION_NEUROLOGICA != -1 ? Entity.PFC_IV_PSICOLOGICO.P52_DISFUNCION_NEUROLOGICA : null : null;
                                    _Psicologico.P6_INTEGRACION = Entity.PFC_IV_PSICOLOGICO.P6_INTEGRACION;
                                    _Psicologico.P8_RASGOS_PERSONALIDAD = Entity.PFC_IV_PSICOLOGICO.P8_RASGOS_PERSONALIDAD;
                                    _Psicologico.P9_DICTAMEN_REINSERCION = Entity.PFC_IV_PSICOLOGICO.P9_DICTAMEN_REINSERCION.HasValue ? Entity.PFC_IV_PSICOLOGICO.P9_DICTAMEN_REINSERCION != decimal.Zero ? Entity.PFC_IV_PSICOLOGICO.P9_DICTAMEN_REINSERCION : null : null;
                                    Context.Entry(_Psicologico).State = System.Data.EntityState.Modified;

                                    #region Programas IV

                                    if (_ProgramasPsicologico != null && _ProgramasPsicologico.Any())
                                        foreach (var item in _ProgramasPsicologico)
                                            Context.Entry(item).State = System.Data.EntityState.Deleted;

                                    var _consecutivoPsicologicoComun = GetIDProceso<short>("PFC_IV_PROGRAMA", "ID_CONSEC", string.Format("ID_CENTRO = {0} AND ID_ANIO = {1} AND ID_IMPUTADO = {2} AND ID_INGRESO = {3} AND ID_ESTUDIO = {4}", Entity.ID_CENTRO, Entity.ID_ANIO, Entity.ID_IMPUTADO, Entity.ID_INGRESO, Entity.ID_ESTUDIO));
                                    if (Entity.PFC_IV_PSICOLOGICO.PFC_IV_PROGRAMA != null && Entity.PFC_IV_PSICOLOGICO.PFC_IV_PROGRAMA.Any())
                                    {
                                        foreach (var item in Entity.PFC_IV_PSICOLOGICO.PFC_IV_PROGRAMA)
                                        {
                                            var _NuevoProgramaPsicologico = new PFC_IV_PROGRAMA()
                                            {
                                                CONCLUYO = item.CONCLUYO,
                                                DURACION = item.DURACION,
                                                ID_ACTIVIDAD = item.ID_ACTIVIDAD,
                                                ID_ANIO = item.ID_ANIO,
                                                ID_CENTRO = item.ID_CENTRO,
                                                ID_ESTUDIO = Entity.ID_ESTUDIO,
                                                ID_IMPUTADO = item.ID_IMPUTADO,
                                                ID_INGRESO = item.ID_INGRESO,
                                                ID_TIPO_PROGRAMA = item.ID_TIPO_PROGRAMA,
                                                OBSERVACION = item.OBSERVACION,
                                                ID_CONSEC = _consecutivoPsicologicoComun
                                            };

                                            ValidaEstudio.PFC_IV_PSICOLOGICO.PFC_IV_PROGRAMA.Add(_NuevoProgramaPsicologico);
                                            _consecutivoPsicologicoComun++;
                                        };
                                    };

                                    #endregion
                                }

                                else
                                {
                                    if (Entity.PFC_IV_PSICOLOGICO != null)
                                    {
                                        ValidaEstudio.PFC_IV_PSICOLOGICO = new PFC_IV_PSICOLOGICO()
                                        {
                                            COORDINADOR = Entity.PFC_IV_PSICOLOGICO.COORDINADOR,
                                            ELABORO = Entity.PFC_IV_PSICOLOGICO.ELABORO,
                                            ESTUDIO_FEC = Entity.PFC_IV_PSICOLOGICO.ESTUDIO_FEC,
                                            ID_ANIO = Entity.PFC_IV_PSICOLOGICO.ID_ANIO,
                                            ID_CENTRO = Entity.PFC_IV_PSICOLOGICO.ID_CENTRO,
                                            ID_ESTUDIO = ValidaEstudio.ID_ESTUDIO,
                                            ID_IMPUTADO = Entity.PFC_IV_PSICOLOGICO.ID_IMPUTADO,
                                            ID_INGRESO = Entity.PFC_IV_PSICOLOGICO.ID_INGRESO,
                                            P1_CONDICIONES_GRALES = Entity.PFC_IV_PSICOLOGICO.P1_CONDICIONES_GRALES,
                                            P10_MOTIVACION_DICTAMEN = Entity.PFC_IV_PSICOLOGICO.P10_MOTIVACION_DICTAMEN,
                                            P11_CASO_NEGATIVO = Entity.PFC_IV_PSICOLOGICO.P11_CASO_NEGATIVO,
                                            P12_CUAL = Entity.PFC_IV_PSICOLOGICO.P12_CUAL,
                                            P12_REQUIERE_TRATAMIENTO = string.IsNullOrEmpty(Entity.PFC_IV_PSICOLOGICO.P12_REQUIERE_TRATAMIENTO) ? Entity.PFC_IV_PSICOLOGICO.P12_REQUIERE_TRATAMIENTO : string.Empty,
                                            P2_EXAMEN_MENTAL = Entity.PFC_IV_PSICOLOGICO.P2_EXAMEN_MENTAL,
                                            P3_PRINCIPALES_RASGOS = Entity.PFC_IV_PSICOLOGICO.P3_PRINCIPALES_RASGOS,
                                            P4_INVENTARIO_MULTIFASICO = Entity.PFC_IV_PSICOLOGICO.P4_INVENTARIO_MULTIFASICO,
                                            P4_OTRAS = Entity.PFC_IV_PSICOLOGICO.P4_OTRAS,
                                            P4_OTRA_MENCIONAR = Entity.PFC_IV_PSICOLOGICO.P4_OTRA_MENCIONAR,
                                            P4_TEST_GUALTICO = Entity.PFC_IV_PSICOLOGICO.P4_TEST_GUALTICO,
                                            P4_TEST_HTP = Entity.PFC_IV_PSICOLOGICO.P4_TEST_HTP,
                                            P4_TEST_MATRICES = Entity.PFC_IV_PSICOLOGICO.P4_TEST_MATRICES,
                                            P51_NIVEL_INTELECTUAL = Entity.PFC_IV_PSICOLOGICO.P51_NIVEL_INTELECTUAL.HasValue ? Entity.PFC_IV_PSICOLOGICO.P51_NIVEL_INTELECTUAL != -1 ? Entity.PFC_IV_PSICOLOGICO.P51_NIVEL_INTELECTUAL : null : null,
                                            P52_DISFUNCION_NEUROLOGICA = Entity.PFC_IV_PSICOLOGICO.P52_DISFUNCION_NEUROLOGICA.HasValue ? Entity.PFC_IV_PSICOLOGICO.P52_DISFUNCION_NEUROLOGICA != -1 ? Entity.PFC_IV_PSICOLOGICO.P52_DISFUNCION_NEUROLOGICA : null : null,
                                            P6_INTEGRACION = Entity.PFC_IV_PSICOLOGICO.P6_INTEGRACION,
                                            P8_RASGOS_PERSONALIDAD = Entity.PFC_IV_PSICOLOGICO.P8_RASGOS_PERSONALIDAD,
                                            P9_DICTAMEN_REINSERCION = Entity.PFC_IV_PSICOLOGICO.P9_DICTAMEN_REINSERCION.HasValue ? Entity.PFC_IV_PSICOLOGICO.P9_DICTAMEN_REINSERCION != decimal.Zero ? Entity.PFC_IV_PSICOLOGICO.P9_DICTAMEN_REINSERCION : null : null,
                                        };

                                        #region Programas IV
                                        if (_ProgramasPsicologico != null && _ProgramasPsicologico.Any())
                                            foreach (var item in _ProgramasPsicologico)
                                                Context.Entry(item).State = System.Data.EntityState.Deleted;

                                        var _consecutivoPsicologicoComun = GetIDProceso<short>("PFC_IV_PROGRAMA", "ID_CONSEC", string.Format("ID_CENTRO = {0} AND ID_ANIO = {1} AND ID_IMPUTADO = {2} AND ID_INGRESO = {3} AND ID_ESTUDIO = {4}", Entity.ID_CENTRO, Entity.ID_ANIO, Entity.ID_IMPUTADO, Entity.ID_INGRESO, Entity.ID_ESTUDIO));

                                        if (Entity.PFC_IV_PSICOLOGICO.PFC_IV_PROGRAMA != null && Entity.PFC_IV_PSICOLOGICO.PFC_IV_PROGRAMA.Any())
                                        {
                                            foreach (var item in Entity.PFC_IV_PSICOLOGICO.PFC_IV_PROGRAMA)
                                            {
                                                var _NuevoProgramaPsicologico = new PFC_IV_PROGRAMA()
                                                {
                                                    CONCLUYO = item.CONCLUYO,
                                                    DURACION = item.DURACION,
                                                    ID_ACTIVIDAD = item.ID_ACTIVIDAD,
                                                    ID_ANIO = item.ID_ANIO,
                                                    ID_CENTRO = item.ID_CENTRO,
                                                    ID_ESTUDIO = Entity.ID_ESTUDIO,
                                                    ID_IMPUTADO = item.ID_IMPUTADO,
                                                    ID_INGRESO = item.ID_INGRESO,
                                                    ID_TIPO_PROGRAMA = item.ID_TIPO_PROGRAMA,
                                                    OBSERVACION = item.OBSERVACION,
                                                    ID_CONSEC = _consecutivoPsicologicoComun
                                                };

                                                ValidaEstudio.PFC_IV_PSICOLOGICO.PFC_IV_PROGRAMA.Add(_NuevoProgramaPsicologico);
                                                _consecutivoPsicologicoComun++;
                                            };
                                        };

                                        #endregion

                                        Context.PFC_IV_PSICOLOGICO.Add(ValidaEstudio.PFC_IV_PSICOLOGICO);
                                    }

                            #endregion
                                }

                                #region Detalle
                                if (_EstudioPadre != null && Entity.PFC_IV_PSICOLOGICO != null)
                                {
                                    short? _Estatus = new short?();
                                    var _DetallePsic = Context.PERSONALIDAD.FirstOrDefault(x => x.ID_ESTUDIO == _EstudioPadre.ID_ESTUDIO && x.ID_ANIO == _EstudioPadre.ID_ANIO && x.ID_CENTRO == _EstudioPadre.ID_CENTRO && x.ID_IMPUTADO == _EstudioPadre.ID_IMPUTADO && x.ID_INGRESO == _EstudioPadre.ID_INGRESO);
                                    if (string.IsNullOrEmpty(Entity.PFC_IV_PSICOLOGICO.COORDINADOR) || string.IsNullOrEmpty(Entity.PFC_IV_PSICOLOGICO.ELABORO) || string.IsNullOrEmpty(Entity.PFC_IV_PSICOLOGICO.P1_CONDICIONES_GRALES) || string.IsNullOrEmpty(Entity.PFC_IV_PSICOLOGICO.P10_MOTIVACION_DICTAMEN) || string.IsNullOrEmpty(Entity.PFC_IV_PSICOLOGICO.P2_EXAMEN_MENTAL) || string.IsNullOrEmpty(Entity.PFC_IV_PSICOLOGICO.P3_PRINCIPALES_RASGOS) || string.IsNullOrEmpty(Entity.PFC_IV_PSICOLOGICO.P6_INTEGRACION) || string.IsNullOrEmpty(Entity.PFC_IV_PSICOLOGICO.P8_RASGOS_PERSONALIDAD) ||
                                        Entity.PFC_IV_PSICOLOGICO.ESTUDIO_FEC == null || Entity.PFC_IV_PSICOLOGICO.P51_NIVEL_INTELECTUAL == null || Entity.PFC_IV_PSICOLOGICO.P52_DISFUNCION_NEUROLOGICA == null || Entity.PFC_IV_PSICOLOGICO.P9_DICTAMEN_REINSERCION == null)
                                        _Estatus = (short)eEstatudDetallePersonalidad.ASIGNADO;
                                    else
                                        _Estatus = (short)eEstatudDetallePersonalidad.TERMINADO;

                                    if (_DetallePsic != null)
                                    {
                                        var _DesarrolloEstudioPsiq = Context.PERSONALIDAD_DETALLE.FirstOrDefault(c => c.ID_ESTUDIO == _DetallePsic.ID_ESTUDIO && c.ID_TIPO == (short)eTiposEstudio.PSICOLOGIA && c.ID_IMPUTADO == _DetallePsic.ID_IMPUTADO && c.ID_INGRESO == _DetallePsic.ID_INGRESO && c.ID_CENTRO == _DetallePsic.ID_CENTRO && c.ID_ANIO == _DetallePsic.ID_ANIO);
                                        if (_DesarrolloEstudioPsiq == null)
                                        {
                                            short _ConsecutivoPersonalidadDetalle = GetIDProceso<short>("PERSONALIDAD_DETALLE", "ID_DETALLE", "1=1");
                                            var _NvoDetalle = new PERSONALIDAD_DETALLE()
                                            {
                                                DIAS_BONIFICADOS = null,
                                                ID_ANIO = Entity.ID_ANIO,
                                                ID_CENTRO = Entity.ID_CENTRO,
                                                ID_DETALLE = _ConsecutivoPersonalidadDetalle,
                                                ID_ESTUDIO = _EstudioPadre.ID_ESTUDIO,
                                                ID_IMPUTADO = Entity.ID_IMPUTADO,
                                                ID_INGRESO = Entity.ID_INGRESO,
                                                ID_TIPO = (short)eTiposEstudio.PSICOLOGIA,
                                                INICIO_FEC = GetFechaServerDate(),
                                                RESULTADO = ValidaEstudio.PFC_II_MEDICO != null ? ValidaEstudio.PFC_II_MEDICO.P8_DICTAMEN_MEDICO.HasValue ? ValidaEstudio.PFC_II_MEDICO.P8_DICTAMEN_MEDICO == (short)eResultado.FAVORABLE ? "S" : ValidaEstudio.PFC_II_MEDICO.P8_DICTAMEN_MEDICO == (short)eResultado.DESFAVORABLE ? "N" : string.Empty : string.Empty : string.Empty,
                                                SOLICITUD_FEC = _EstudioPadre.SOLICITUD_FEC,
                                                ID_ESTATUS = _Estatus,
                                                TERMINO_FEC = _Estatus == (short)eEstatudDetallePersonalidad.TERMINADO ? GetFechaServerDate() : new System.DateTime?(),
                                                TIPO_MEDIA = string.Empty
                                            };

                                            Context.PERSONALIDAD_DETALLE.Add(_NvoDetalle);
                                        }

                                        else
                                        {
                                            _DesarrolloEstudioPsiq.RESULTADO = ValidaEstudio.PFC_II_MEDICO != null ? ValidaEstudio.PFC_II_MEDICO.P8_DICTAMEN_MEDICO.HasValue ? ValidaEstudio.PFC_II_MEDICO.P8_DICTAMEN_MEDICO == (short)eResultado.FAVORABLE ? "S" : ValidaEstudio.PFC_II_MEDICO.P8_DICTAMEN_MEDICO == (short)eResultado.DESFAVORABLE ? "N" : string.Empty : string.Empty : string.Empty;
                                            _DesarrolloEstudioPsiq.ID_ESTATUS = _Estatus;
                                            Context.Entry(_DesarrolloEstudioPsiq).State = System.Data.EntityState.Modified;
                                        }
                                    }
                                }
                            }

                                #endregion


                            #region Criminodiagnostico Edicion Comun
                            if (Entity.PFC_V_CRIMINODIAGNOSTICO != null)
                            {
                                var _CriminoDiagnostico = Context.PFC_V_CRIMINODIAGNOSTICO.FirstOrDefault(x => x.ID_IMPUTADO == Entity.ID_IMPUTADO && x.ID_INGRESO == Entity.ID_INGRESO && x.ID_ESTUDIO == ValidaEstudio.ID_ESTUDIO && x.ID_CENTRO == Entity.ID_CENTRO);
                                if (_CriminoDiagnostico != null)
                                {
                                    _CriminoDiagnostico.COORDINADOR = Entity.PFC_V_CRIMINODIAGNOSTICO.COORDINADOR;
                                    _CriminoDiagnostico.ELABORO = Entity.PFC_V_CRIMINODIAGNOSTICO.ELABORO;
                                    _CriminoDiagnostico.ESTUDIO_FEC = Entity.PFC_V_CRIMINODIAGNOSTICO.ESTUDIO_FEC;
                                    _CriminoDiagnostico.ID_ANIO = Entity.PFC_V_CRIMINODIAGNOSTICO.ID_ANIO;
                                    _CriminoDiagnostico.ID_CENTRO = Entity.PFC_V_CRIMINODIAGNOSTICO.ID_CENTRO;
                                    _CriminoDiagnostico.ID_ESTUDIO = ValidaEstudio.ID_ESTUDIO;
                                    _CriminoDiagnostico.ID_IMPUTADO = Entity.PFC_V_CRIMINODIAGNOSTICO.ID_IMPUTADO;
                                    _CriminoDiagnostico.ID_INGRESO = Entity.PFC_V_CRIMINODIAGNOSTICO.ID_INGRESO;
                                    _CriminoDiagnostico.P1_ALCOHOL = Entity.PFC_V_CRIMINODIAGNOSTICO.P1_ALCOHOL;
                                    _CriminoDiagnostico.P1_DROGADO = Entity.PFC_V_CRIMINODIAGNOSTICO.P1_DROGADO;
                                    _CriminoDiagnostico.P1_DROGRA_ILEGAL = Entity.PFC_V_CRIMINODIAGNOSTICO.P1_DROGRA_ILEGAL;
                                    _CriminoDiagnostico.P1_OTRA = Entity.PFC_V_CRIMINODIAGNOSTICO.P1_OTRA;
                                    _CriminoDiagnostico.P1_VERSION_DELITO = Entity.PFC_V_CRIMINODIAGNOSTICO.P1_VERSION_DELITO;
                                    _CriminoDiagnostico.P10_DICTAMEN_REINSERCION = Entity.PFC_V_CRIMINODIAGNOSTICO.P10_DICTAMEN_REINSERCION.HasValue ? Entity.PFC_V_CRIMINODIAGNOSTICO.P10_DICTAMEN_REINSERCION != decimal.Zero ? Entity.PFC_V_CRIMINODIAGNOSTICO.P10_DICTAMEN_REINSERCION : null : null;
                                    _CriminoDiagnostico.P10_MOTIVACION_DICTAMEN = Entity.PFC_V_CRIMINODIAGNOSTICO.P10_MOTIVACION_DICTAMEN;
                                    _CriminoDiagnostico.P11_PROGRAMAS_REMITIRSE = Entity.PFC_V_CRIMINODIAGNOSTICO.P11_PROGRAMAS_REMITIRSE;
                                    _CriminoDiagnostico.P12_CUAL = Entity.PFC_V_CRIMINODIAGNOSTICO.P12_CUAL;
                                    _CriminoDiagnostico.P12_TRATAMIENTO_EXTRAMUROS = Entity.PFC_V_CRIMINODIAGNOSTICO.P12_TRATAMIENTO_EXTRAMUROS;
                                    _CriminoDiagnostico.P2_CRIMINOGENESIS = Entity.PFC_V_CRIMINODIAGNOSTICO.P2_CRIMINOGENESIS;
                                    _CriminoDiagnostico.P3_CONDUCTA_ANTISOCIAL = Entity.PFC_V_CRIMINODIAGNOSTICO.P3_CONDUCTA_ANTISOCIAL;
                                    _CriminoDiagnostico.P4_CLASIFICACION_CRIMINOLOGICA = Entity.PFC_V_CRIMINODIAGNOSTICO.P4_CLASIFICACION_CRIMINOLOGICA.HasValue ? Entity.PFC_V_CRIMINODIAGNOSTICO.P4_CLASIFICACION_CRIMINOLOGICA != -1 ? Entity.PFC_V_CRIMINODIAGNOSTICO.P4_CLASIFICACION_CRIMINOLOGICA : null : null;
                                    _CriminoDiagnostico.P5_INTIMIDACION = Entity.PFC_V_CRIMINODIAGNOSTICO.P5_INTIMIDACION;
                                    _CriminoDiagnostico.P5_PORQUE = Entity.PFC_V_CRIMINODIAGNOSTICO.P5_PORQUE;
                                    _CriminoDiagnostico.P6_CAPACIDAD_CRIMINAL = Entity.PFC_V_CRIMINODIAGNOSTICO.P6_CAPACIDAD_CRIMINAL.HasValue ? Entity.PFC_V_CRIMINODIAGNOSTICO.P6_CAPACIDAD_CRIMINAL != -1 ? Entity.PFC_V_CRIMINODIAGNOSTICO.P6_CAPACIDAD_CRIMINAL : null : null;
                                    _CriminoDiagnostico.P6A_EGOCENTRICO = Entity.PFC_V_CRIMINODIAGNOSTICO.P6A_EGOCENTRICO.HasValue ? Entity.PFC_V_CRIMINODIAGNOSTICO.P6A_EGOCENTRICO != -1 ? Entity.PFC_V_CRIMINODIAGNOSTICO.P6A_EGOCENTRICO : null : null;
                                    _CriminoDiagnostico.P6B_LIABILIDAD_EFECTIVA = Entity.PFC_V_CRIMINODIAGNOSTICO.P6B_LIABILIDAD_EFECTIVA.HasValue ? Entity.PFC_V_CRIMINODIAGNOSTICO.P6B_LIABILIDAD_EFECTIVA != -1 ? Entity.PFC_V_CRIMINODIAGNOSTICO.P6B_LIABILIDAD_EFECTIVA : null : null;
                                    _CriminoDiagnostico.P6C_AGRESIVIDAD = Entity.PFC_V_CRIMINODIAGNOSTICO.P6C_AGRESIVIDAD.HasValue ? Entity.PFC_V_CRIMINODIAGNOSTICO.P6C_AGRESIVIDAD != -1 ? Entity.PFC_V_CRIMINODIAGNOSTICO.P6C_AGRESIVIDAD : null : null;
                                    _CriminoDiagnostico.P6D_INDIFERENCIA_AFECTIVA = Entity.PFC_V_CRIMINODIAGNOSTICO.P6D_INDIFERENCIA_AFECTIVA.HasValue ? Entity.PFC_V_CRIMINODIAGNOSTICO.P6D_INDIFERENCIA_AFECTIVA != -1 ? Entity.PFC_V_CRIMINODIAGNOSTICO.P6D_INDIFERENCIA_AFECTIVA : null : null;
                                    _CriminoDiagnostico.P7_ADAPTACION_SOCIAL = Entity.PFC_V_CRIMINODIAGNOSTICO.P7_ADAPTACION_SOCIAL.HasValue ? Entity.PFC_V_CRIMINODIAGNOSTICO.P7_ADAPTACION_SOCIAL != -1 ? Entity.PFC_V_CRIMINODIAGNOSTICO.P7_ADAPTACION_SOCIAL : null : null;
                                    _CriminoDiagnostico.P8_INDICE_PELIGROSIDAD = Entity.PFC_V_CRIMINODIAGNOSTICO.P8_INDICE_PELIGROSIDAD.HasValue ? Entity.PFC_V_CRIMINODIAGNOSTICO.P8_INDICE_PELIGROSIDAD != -1 ? Entity.PFC_V_CRIMINODIAGNOSTICO.P8_INDICE_PELIGROSIDAD : null : null;
                                    _CriminoDiagnostico.P9_PRONOSTICO_REINCIDENCIA = Entity.PFC_V_CRIMINODIAGNOSTICO.P9_PRONOSTICO_REINCIDENCIA.HasValue ? Entity.PFC_V_CRIMINODIAGNOSTICO.P9_PRONOSTICO_REINCIDENCIA != -1 ? Entity.PFC_V_CRIMINODIAGNOSTICO.P9_PRONOSTICO_REINCIDENCIA : null : null;
                                    Context.Entry(_CriminoDiagnostico).State = System.Data.EntityState.Modified;
                                }

                                else
                                {
                                    Entity.PFC_V_CRIMINODIAGNOSTICO = new PFC_V_CRIMINODIAGNOSTICO()
                                    {
                                        COORDINADOR = Entity.PFC_V_CRIMINODIAGNOSTICO.COORDINADOR,
                                        ELABORO = Entity.PFC_V_CRIMINODIAGNOSTICO.ELABORO,
                                        ESTUDIO_FEC = Entity.PFC_V_CRIMINODIAGNOSTICO.ESTUDIO_FEC,
                                        ID_ANIO = Entity.PFC_V_CRIMINODIAGNOSTICO.ID_ANIO,
                                        ID_CENTRO = Entity.PFC_V_CRIMINODIAGNOSTICO.ID_CENTRO,
                                        ID_ESTUDIO = ValidaEstudio.ID_ESTUDIO,
                                        ID_IMPUTADO = Entity.PFC_V_CRIMINODIAGNOSTICO.ID_IMPUTADO,
                                        ID_INGRESO = Entity.PFC_V_CRIMINODIAGNOSTICO.ID_INGRESO,
                                        P1_ALCOHOL = Entity.PFC_V_CRIMINODIAGNOSTICO.P1_ALCOHOL,
                                        P1_DROGADO = Entity.PFC_V_CRIMINODIAGNOSTICO.P1_DROGADO,
                                        P1_DROGRA_ILEGAL = Entity.PFC_V_CRIMINODIAGNOSTICO.P1_DROGRA_ILEGAL,
                                        P1_OTRA = Entity.PFC_V_CRIMINODIAGNOSTICO.P1_OTRA,
                                        P1_VERSION_DELITO = Entity.PFC_V_CRIMINODIAGNOSTICO.P1_VERSION_DELITO,
                                        P10_DICTAMEN_REINSERCION = Entity.PFC_V_CRIMINODIAGNOSTICO.P10_DICTAMEN_REINSERCION.HasValue ? Entity.PFC_V_CRIMINODIAGNOSTICO.P10_DICTAMEN_REINSERCION != decimal.Zero ? Entity.PFC_V_CRIMINODIAGNOSTICO.P10_DICTAMEN_REINSERCION : null : null,
                                        P10_MOTIVACION_DICTAMEN = Entity.PFC_V_CRIMINODIAGNOSTICO.P10_MOTIVACION_DICTAMEN,
                                        P11_PROGRAMAS_REMITIRSE = Entity.PFC_V_CRIMINODIAGNOSTICO.P11_PROGRAMAS_REMITIRSE,
                                        P12_CUAL = Entity.PFC_V_CRIMINODIAGNOSTICO.P12_CUAL,
                                        P12_TRATAMIENTO_EXTRAMUROS = Entity.PFC_V_CRIMINODIAGNOSTICO.P12_TRATAMIENTO_EXTRAMUROS,
                                        P2_CRIMINOGENESIS = Entity.PFC_V_CRIMINODIAGNOSTICO.P2_CRIMINOGENESIS,
                                        P3_CONDUCTA_ANTISOCIAL = Entity.PFC_V_CRIMINODIAGNOSTICO.P3_CONDUCTA_ANTISOCIAL,
                                        P4_CLASIFICACION_CRIMINOLOGICA = Entity.PFC_V_CRIMINODIAGNOSTICO.P4_CLASIFICACION_CRIMINOLOGICA.HasValue ? Entity.PFC_V_CRIMINODIAGNOSTICO.P4_CLASIFICACION_CRIMINOLOGICA != -1 ? Entity.PFC_V_CRIMINODIAGNOSTICO.P4_CLASIFICACION_CRIMINOLOGICA : null : null,
                                        P5_INTIMIDACION = Entity.PFC_V_CRIMINODIAGNOSTICO.P5_INTIMIDACION,
                                        P5_PORQUE = Entity.PFC_V_CRIMINODIAGNOSTICO.P5_PORQUE,
                                        P6_CAPACIDAD_CRIMINAL = Entity.PFC_V_CRIMINODIAGNOSTICO.P6_CAPACIDAD_CRIMINAL.HasValue ? Entity.PFC_V_CRIMINODIAGNOSTICO.P6_CAPACIDAD_CRIMINAL != -1 ? Entity.PFC_V_CRIMINODIAGNOSTICO.P6_CAPACIDAD_CRIMINAL : null : null,
                                        P6A_EGOCENTRICO = Entity.PFC_V_CRIMINODIAGNOSTICO.P6A_EGOCENTRICO.HasValue ? Entity.PFC_V_CRIMINODIAGNOSTICO.P6A_EGOCENTRICO != -1 ? Entity.PFC_V_CRIMINODIAGNOSTICO.P6A_EGOCENTRICO : null : null,
                                        P6B_LIABILIDAD_EFECTIVA = Entity.PFC_V_CRIMINODIAGNOSTICO.P6B_LIABILIDAD_EFECTIVA.HasValue ? Entity.PFC_V_CRIMINODIAGNOSTICO.P6B_LIABILIDAD_EFECTIVA != -1 ? Entity.PFC_V_CRIMINODIAGNOSTICO.P6B_LIABILIDAD_EFECTIVA : null : null,
                                        P6C_AGRESIVIDAD = Entity.PFC_V_CRIMINODIAGNOSTICO.P6C_AGRESIVIDAD.HasValue ? Entity.PFC_V_CRIMINODIAGNOSTICO.P6C_AGRESIVIDAD != -1 ? Entity.PFC_V_CRIMINODIAGNOSTICO.P6C_AGRESIVIDAD : null : null,
                                        P6D_INDIFERENCIA_AFECTIVA = Entity.PFC_V_CRIMINODIAGNOSTICO.P6D_INDIFERENCIA_AFECTIVA.HasValue ? Entity.PFC_V_CRIMINODIAGNOSTICO.P6D_INDIFERENCIA_AFECTIVA != -1 ? Entity.PFC_V_CRIMINODIAGNOSTICO.P6D_INDIFERENCIA_AFECTIVA : null : null,
                                        P7_ADAPTACION_SOCIAL = Entity.PFC_V_CRIMINODIAGNOSTICO.P7_ADAPTACION_SOCIAL.HasValue ? Entity.PFC_V_CRIMINODIAGNOSTICO.P7_ADAPTACION_SOCIAL != -1 ? Entity.PFC_V_CRIMINODIAGNOSTICO.P7_ADAPTACION_SOCIAL : null : null,
                                        P8_INDICE_PELIGROSIDAD = Entity.PFC_V_CRIMINODIAGNOSTICO.P8_INDICE_PELIGROSIDAD.HasValue ? Entity.PFC_V_CRIMINODIAGNOSTICO.P8_INDICE_PELIGROSIDAD != -1 ? Entity.PFC_V_CRIMINODIAGNOSTICO.P8_INDICE_PELIGROSIDAD : null : null,
                                        P9_PRONOSTICO_REINCIDENCIA = Entity.PFC_V_CRIMINODIAGNOSTICO.P9_PRONOSTICO_REINCIDENCIA.HasValue ? Entity.PFC_V_CRIMINODIAGNOSTICO.P9_PRONOSTICO_REINCIDENCIA != -1 ? Entity.PFC_V_CRIMINODIAGNOSTICO.P9_PRONOSTICO_REINCIDENCIA : null : null,
                                    };

                                    Context.PFC_V_CRIMINODIAGNOSTICO.Add(Entity.PFC_V_CRIMINODIAGNOSTICO);
                                }

                                #region  detalle
                                if (_EstudioPadre != null && Entity.PFC_V_CRIMINODIAGNOSTICO != null)
                                {
                                    short? _Estatus = new short?();
                                    var _DetallePsic = Context.PERSONALIDAD.FirstOrDefault(x => x.ID_ESTUDIO == _EstudioPadre.ID_ESTUDIO && x.ID_ANIO == _EstudioPadre.ID_ANIO && x.ID_CENTRO == _EstudioPadre.ID_CENTRO && x.ID_IMPUTADO == _EstudioPadre.ID_IMPUTADO && x.ID_INGRESO == _EstudioPadre.ID_INGRESO);
                                    if (string.IsNullOrEmpty(Entity.PFC_V_CRIMINODIAGNOSTICO.COORDINADOR) || string.IsNullOrEmpty(Entity.PFC_V_CRIMINODIAGNOSTICO.ELABORO) || string.IsNullOrEmpty(Entity.PFC_V_CRIMINODIAGNOSTICO.P1_VERSION_DELITO) || string.IsNullOrEmpty(Entity.PFC_V_CRIMINODIAGNOSTICO.P10_MOTIVACION_DICTAMEN) || string.IsNullOrEmpty(Entity.PFC_V_CRIMINODIAGNOSTICO.P2_CRIMINOGENESIS) || string.IsNullOrEmpty(Entity.PFC_V_CRIMINODIAGNOSTICO.P3_CONDUCTA_ANTISOCIAL) || string.IsNullOrEmpty(Entity.PFC_V_CRIMINODIAGNOSTICO.P5_INTIMIDACION) || string.IsNullOrEmpty(Entity.PFC_V_CRIMINODIAGNOSTICO.P5_PORQUE) ||
                                        Entity.PFC_V_CRIMINODIAGNOSTICO.ESTUDIO_FEC == null || Entity.PFC_V_CRIMINODIAGNOSTICO.P10_DICTAMEN_REINSERCION == null || Entity.PFC_V_CRIMINODIAGNOSTICO.P4_CLASIFICACION_CRIMINOLOGICA == null || Entity.PFC_V_CRIMINODIAGNOSTICO.P5_INTIMIDACION == null || Entity.PFC_V_CRIMINODIAGNOSTICO.P6_CAPACIDAD_CRIMINAL == null || Entity.PFC_V_CRIMINODIAGNOSTICO.P6A_EGOCENTRICO == null || Entity.PFC_V_CRIMINODIAGNOSTICO.P6B_LIABILIDAD_EFECTIVA == null
                                         || Entity.PFC_V_CRIMINODIAGNOSTICO.P6C_AGRESIVIDAD == null || Entity.PFC_V_CRIMINODIAGNOSTICO.P6D_INDIFERENCIA_AFECTIVA == null || Entity.PFC_V_CRIMINODIAGNOSTICO.P7_ADAPTACION_SOCIAL == null || Entity.PFC_V_CRIMINODIAGNOSTICO.P8_INDICE_PELIGROSIDAD == null || Entity.PFC_V_CRIMINODIAGNOSTICO.P9_PRONOSTICO_REINCIDENCIA == null)
                                        _Estatus = (short)eEstatudDetallePersonalidad.ASIGNADO;
                                    else
                                        _Estatus = (short)eEstatudDetallePersonalidad.TERMINADO;

                                    if (_DetallePsic != null)
                                    {
                                        var _DesarrolloEstudioCrimi = Context.PERSONALIDAD_DETALLE.FirstOrDefault(c => c.ID_ESTUDIO == _DetallePsic.ID_ESTUDIO && c.ID_TIPO == (short)eTiposEstudio.CRIMINOLOGICO && c.ID_IMPUTADO == _DetallePsic.ID_IMPUTADO && c.ID_INGRESO == _DetallePsic.ID_INGRESO && c.ID_CENTRO == _DetallePsic.ID_CENTRO && c.ID_ANIO == _DetallePsic.ID_ANIO);
                                        if (_DesarrolloEstudioCrimi == null)
                                        {
                                            short _ConsecutivoPersonalidadDetalle = GetIDProceso<short>("PERSONALIDAD_DETALLE", "ID_DETALLE", "1=1");
                                            var _NvoDetalle = new PERSONALIDAD_DETALLE()
                                            {
                                                DIAS_BONIFICADOS = null,
                                                ID_ANIO = Entity.ID_ANIO,
                                                ID_CENTRO = Entity.ID_CENTRO,
                                                ID_DETALLE = _ConsecutivoPersonalidadDetalle,
                                                ID_ESTUDIO = _EstudioPadre.ID_ESTUDIO,
                                                ID_IMPUTADO = Entity.ID_IMPUTADO,
                                                ID_INGRESO = Entity.ID_INGRESO,
                                                ID_TIPO = (short)eTiposEstudio.CRIMINOLOGICO,
                                                INICIO_FEC = GetFechaServerDate(),
                                                RESULTADO = ValidaEstudio.PFC_V_CRIMINODIAGNOSTICO != null ? ValidaEstudio.PFC_V_CRIMINODIAGNOSTICO.P10_DICTAMEN_REINSERCION.HasValue ? ValidaEstudio.PFC_V_CRIMINODIAGNOSTICO.P10_DICTAMEN_REINSERCION == (short)eResultado.FAVORABLE ? "S" : ValidaEstudio.PFC_V_CRIMINODIAGNOSTICO.P10_DICTAMEN_REINSERCION == (short)eResultado.DESFAVORABLE ? "N" : string.Empty : string.Empty : string.Empty,
                                                SOLICITUD_FEC = _EstudioPadre.SOLICITUD_FEC,
                                                ID_ESTATUS = _Estatus,
                                                TERMINO_FEC = _Estatus == (short)eEstatudDetallePersonalidad.TERMINADO ? GetFechaServerDate() : new System.DateTime?(),
                                                TIPO_MEDIA = string.Empty
                                            };

                                            Context.PERSONALIDAD_DETALLE.Add(_NvoDetalle);
                                        }

                                        else
                                        {
                                            _DesarrolloEstudioCrimi.RESULTADO = ValidaEstudio.PFC_V_CRIMINODIAGNOSTICO != null ? ValidaEstudio.PFC_V_CRIMINODIAGNOSTICO.P10_DICTAMEN_REINSERCION.HasValue ? ValidaEstudio.PFC_V_CRIMINODIAGNOSTICO.P10_DICTAMEN_REINSERCION == (short)eResultado.FAVORABLE ? "S" : ValidaEstudio.PFC_V_CRIMINODIAGNOSTICO.P10_DICTAMEN_REINSERCION == (short)eResultado.DESFAVORABLE ? "N" : string.Empty : string.Empty : string.Empty;
                                            _DesarrolloEstudioCrimi.ID_ESTATUS = _Estatus;
                                            Context.Entry(_DesarrolloEstudioCrimi).State = System.Data.EntityState.Modified;
                                        }
                                    }
                                }

                                #endregion
                            }
                            #endregion

                            #region Edicion Socio familiar Comun
                            if (Entity.PFC_VI_SOCIO_FAMILIAR != null)
                            {
                                var _SocioFamiliar = Context.PFC_VI_SOCIO_FAMILIAR.FirstOrDefault(x => x.ID_IMPUTADO == Entity.ID_IMPUTADO && x.ID_INGRESO == Entity.ID_INGRESO && x.ID_ESTUDIO == ValidaEstudio.ID_ESTUDIO && x.ID_CENTRO == Entity.ID_CENTRO);
                                var _PadronVisitas = Context.PFC_VI_COMUNICACION.Where(x => x.ID_IMPUTADO == Entity.ID_IMPUTADO && x.ID_INGRESO == Entity.ID_INGRESO && x.ID_ESTUDIO == ValidaEstudio.ID_ESTUDIO && x.ID_CENTRO == Entity.ID_CENTRO);
                                var _GruposSocioFamiliar = Context.PFC_VI_GRUPO.Where(x => x.ID_IMPUTADO == Entity.ID_IMPUTADO && x.ID_INGRESO == Entity.ID_INGRESO && x.ID_ESTUDIO == ValidaEstudio.ID_ESTUDIO && x.ID_CENTRO == Entity.ID_CENTRO);
                                if (_SocioFamiliar != null)
                                {
                                    _SocioFamiliar.COORDINADOR = Entity.PFC_VI_SOCIO_FAMILIAR.COORDINADOR;
                                    _SocioFamiliar.ELABORO = Entity.PFC_VI_SOCIO_FAMILIAR.ELABORO;
                                    _SocioFamiliar.ESTUDIO_FEC = Entity.PFC_VI_SOCIO_FAMILIAR.ESTUDIO_FEC;
                                    _SocioFamiliar.ID_ANIO = Entity.PFC_VI_SOCIO_FAMILIAR.ID_ANIO;
                                    _SocioFamiliar.ID_CENTRO = Entity.PFC_VI_SOCIO_FAMILIAR.ID_CENTRO;
                                    _SocioFamiliar.ID_ESTUDIO = ValidaEstudio.ID_ESTUDIO;
                                    _SocioFamiliar.ID_IMPUTADO = Entity.PFC_VI_SOCIO_FAMILIAR.ID_IMPUTADO;
                                    _SocioFamiliar.ID_INGRESO = Entity.PFC_VI_SOCIO_FAMILIAR.ID_INGRESO;
                                    _SocioFamiliar.P10_DICTAMEN = Entity.PFC_VI_SOCIO_FAMILIAR.P10_DICTAMEN.HasValue ? Entity.PFC_VI_SOCIO_FAMILIAR.P10_DICTAMEN != -1 ? Entity.PFC_VI_SOCIO_FAMILIAR.P10_DICTAMEN != decimal.Zero ? Entity.PFC_VI_SOCIO_FAMILIAR.P10_DICTAMEN : null : null : null;
                                    _SocioFamiliar.P11_MOTIVACION_DICTAMEN = Entity.PFC_VI_SOCIO_FAMILIAR.P11_MOTIVACION_DICTAMEN;
                                    _SocioFamiliar.P21_FAMILIA_PRIMARIA = Entity.PFC_VI_SOCIO_FAMILIAR.P21_FAMILIA_PRIMARIA;
                                    _SocioFamiliar.P22_FAMILIA_SECUNDARIA = Entity.PFC_VI_SOCIO_FAMILIAR.P22_FAMILIA_SECUNDARIA;
                                    _SocioFamiliar.P3_TERCERA_EDAD = Entity.PFC_VI_SOCIO_FAMILIAR.P3_TERCERA_EDAD;
                                    _SocioFamiliar.P4_ESPOSOA = Entity.PFC_VI_SOCIO_FAMILIAR.P4_ESPOSOA;
                                    _SocioFamiliar.P4_FRECUENCIA = Entity.PFC_VI_SOCIO_FAMILIAR.P4_FRECUENCIA;
                                    _SocioFamiliar.P4_HERMANOS = Entity.PFC_VI_SOCIO_FAMILIAR.P4_HERMANOS;
                                    _SocioFamiliar.P4_HIJOS = Entity.PFC_VI_SOCIO_FAMILIAR.P4_HIJOS;
                                    _SocioFamiliar.P4_MADRE = Entity.PFC_VI_SOCIO_FAMILIAR.P4_MADRE;
                                    _SocioFamiliar.P4_MOTIVO_NO_VISITA = Entity.PFC_VI_SOCIO_FAMILIAR.P4_MOTIVO_NO_VISITA;
                                    _SocioFamiliar.P4_OTROS = Entity.PFC_VI_SOCIO_FAMILIAR.P4_OTROS;
                                    _SocioFamiliar.P4_OTROS_EPECIFICAR = Entity.PFC_VI_SOCIO_FAMILIAR.P4_OTROS_EPECIFICAR;
                                    _SocioFamiliar.P4_PADRE = Entity.PFC_VI_SOCIO_FAMILIAR.P4_PADRE;
                                    _SocioFamiliar.P4_RECIBE_VISITA = Entity.PFC_VI_SOCIO_FAMILIAR.P4_RECIBE_VISITA;
                                    _SocioFamiliar.P5_COMUNICACION_TELEFONICA = Entity.PFC_VI_SOCIO_FAMILIAR.P5_COMUNICACION_TELEFONICA;
                                    _SocioFamiliar.P5_NO_POR_QUE = Entity.PFC_VI_SOCIO_FAMILIAR.P5_NO_POR_QUE;
                                    _SocioFamiliar.P6_APOYO_EXTERIOR = Entity.PFC_VI_SOCIO_FAMILIAR.P6_APOYO_EXTERIOR;
                                    _SocioFamiliar.P7_PLANES_INTERNO = Entity.PFC_VI_SOCIO_FAMILIAR.P7_PLANES_INTERNO;
                                    _SocioFamiliar.P7_VIVIRA = Entity.PFC_VI_SOCIO_FAMILIAR.P7_VIVIRA;
                                    _SocioFamiliar.P8_OFERTA_ESPECIFICAR = Entity.PFC_VI_SOCIO_FAMILIAR.P8_OFERTA_ESPECIFICAR;
                                    _SocioFamiliar.P8_OFERTA_TRABAJO = Entity.PFC_VI_SOCIO_FAMILIAR.P8_OFERTA_TRABAJO;
                                    _SocioFamiliar.P9_AVAL_ESPECIFICAR = Entity.PFC_VI_SOCIO_FAMILIAR.P9_AVAL_ESPECIFICAR;
                                    _SocioFamiliar.P9_AVAL_MORAL = Entity.PFC_VI_SOCIO_FAMILIAR.P9_AVAL_MORAL;
                                    Context.Entry(_SocioFamiliar).State = System.Data.EntityState.Modified;

                                    #region Visitas y Grupos Edicion Socio Familiar Comun
                                    if (_PadronVisitas != null && _PadronVisitas.Any())
                                        foreach (var item in _PadronVisitas)
                                            Context.Entry(item).State = System.Data.EntityState.Deleted;

                                    if (_GruposSocioFamiliar != null && _GruposSocioFamiliar.Any())
                                        foreach (var item in _GruposSocioFamiliar)
                                            Context.Entry(item).State = System.Data.EntityState.Deleted;

                                    var _consecutivoFamiliares = GetIDProceso<short>("PFC_VI_COMUNICACION", "ID_CONSEC", string.Format("ID_CENTRO = {0} AND ID_ANIO = {1} AND ID_IMPUTADO = {2} AND ID_INGRESO = {3} AND ID_ESTUDIO = {4}", Entity.ID_CENTRO, Entity.ID_ANIO, Entity.ID_IMPUTADO, Entity.ID_INGRESO, Entity.ID_ESTUDIO));

                                    if (Entity.PFC_VI_SOCIO_FAMILIAR.PFC_VI_COMUNICACION != null && Entity.PFC_VI_SOCIO_FAMILIAR.PFC_VI_COMUNICACION.Any())
                                    {
                                        foreach (var item in Entity.PFC_VI_SOCIO_FAMILIAR.PFC_VI_COMUNICACION)
                                        {
                                            var _NuevaVisita = new PFC_VI_COMUNICACION()
                                            {
                                                FRECUENCIA = item.FRECUENCIA,
                                                ID_ANIO = item.ID_ANIO,
                                                ID_CENTRO = item.ID_CENTRO,
                                                ID_CONSEC = _consecutivoFamiliares,
                                                ID_ESTUDIO = ValidaEstudio.ID_ESTUDIO,
                                                ID_IMPUTADO = item.ID_IMPUTADO,
                                                ID_INGRESO = item.ID_INGRESO,
                                                ID_TIPO_REFERENCIA = item.ID_TIPO_REFERENCIA,
                                                NOMBRE = item.NOMBRE,
                                                TELEFONO = item.TELEFONO
                                            };

                                            _SocioFamiliar.PFC_VI_COMUNICACION.Add(_NuevaVisita);
                                            _consecutivoFamiliares++;
                                        };
                                    }

                                    if (Entity.PFC_VI_SOCIO_FAMILIAR.PFC_VI_GRUPO != null && Entity.PFC_VI_SOCIO_FAMILIAR.PFC_VI_GRUPO.Any())
                                        foreach (var item in Entity.PFC_VI_SOCIO_FAMILIAR.PFC_VI_GRUPO)
                                        {
                                            var _NuevoGrupoSocioFamiliar = new PFC_VI_GRUPO()
                                            {
                                                CONGREGACION = item.CONGREGACION,
                                                ID_ACTIVIDAD = item.ID_ACTIVIDAD,
                                                ID_ANIO = item.ID_ANIO,
                                                ID_CENTRO = item.ID_CENTRO,
                                                ID_ESTUDIO = ValidaEstudio.ID_ESTUDIO,
                                                ID_IMPUTADO = item.ID_IMPUTADO,
                                                ID_INGRESO = item.ID_INGRESO,
                                                ID_TIPO_PROGRAMA = item.ID_TIPO_PROGRAMA,
                                                OBSERVACIONES = item.OBSERVACIONES,
                                                PERIODO = item.PERIODO
                                            };

                                            _SocioFamiliar.PFC_VI_GRUPO.Add(_NuevoGrupoSocioFamiliar);
                                        };

                                    #endregion
                                }

                                else
                                {
                                    ValidaEstudio.PFC_VI_SOCIO_FAMILIAR = new PFC_VI_SOCIO_FAMILIAR()
                                    {
                                        COORDINADOR = Entity.PFC_VI_SOCIO_FAMILIAR.COORDINADOR,
                                        ELABORO = Entity.PFC_VI_SOCIO_FAMILIAR.ELABORO,
                                        ESTUDIO_FEC = Entity.PFC_VI_SOCIO_FAMILIAR.ESTUDIO_FEC,
                                        ID_ANIO = Entity.PFC_VI_SOCIO_FAMILIAR.ID_ANIO,
                                        ID_CENTRO = Entity.PFC_VI_SOCIO_FAMILIAR.ID_CENTRO,
                                        ID_ESTUDIO = ValidaEstudio.ID_ESTUDIO,
                                        ID_IMPUTADO = Entity.PFC_VI_SOCIO_FAMILIAR.ID_IMPUTADO,
                                        ID_INGRESO = Entity.PFC_VI_SOCIO_FAMILIAR.ID_INGRESO,
                                        P10_DICTAMEN = Entity.PFC_VI_SOCIO_FAMILIAR.P10_DICTAMEN.HasValue ? Entity.PFC_VI_SOCIO_FAMILIAR.P10_DICTAMEN != -1 ? Entity.PFC_VI_SOCIO_FAMILIAR.P10_DICTAMEN != decimal.Zero ? Entity.PFC_VI_SOCIO_FAMILIAR.P10_DICTAMEN : null : null : null,
                                        P11_MOTIVACION_DICTAMEN = Entity.PFC_VI_SOCIO_FAMILIAR.P11_MOTIVACION_DICTAMEN,
                                        P21_FAMILIA_PRIMARIA = Entity.PFC_VI_SOCIO_FAMILIAR.P21_FAMILIA_PRIMARIA,
                                        P22_FAMILIA_SECUNDARIA = Entity.PFC_VI_SOCIO_FAMILIAR.P22_FAMILIA_SECUNDARIA,
                                        P3_TERCERA_EDAD = Entity.PFC_VI_SOCIO_FAMILIAR.P3_TERCERA_EDAD,
                                        P4_ESPOSOA = Entity.PFC_VI_SOCIO_FAMILIAR.P4_ESPOSOA,
                                        P4_FRECUENCIA = Entity.PFC_VI_SOCIO_FAMILIAR.P4_FRECUENCIA,
                                        P4_HERMANOS = Entity.PFC_VI_SOCIO_FAMILIAR.P4_HERMANOS,
                                        P4_HIJOS = Entity.PFC_VI_SOCIO_FAMILIAR.P4_HIJOS,
                                        P4_MADRE = Entity.PFC_VI_SOCIO_FAMILIAR.P4_MADRE,
                                        P4_MOTIVO_NO_VISITA = Entity.PFC_VI_SOCIO_FAMILIAR.P4_MOTIVO_NO_VISITA,
                                        P4_OTROS = Entity.PFC_VI_SOCIO_FAMILIAR.P4_OTROS,
                                        P4_OTROS_EPECIFICAR = Entity.PFC_VI_SOCIO_FAMILIAR.P4_OTROS_EPECIFICAR,
                                        P4_PADRE = Entity.PFC_VI_SOCIO_FAMILIAR.P4_PADRE,
                                        P4_RECIBE_VISITA = Entity.PFC_VI_SOCIO_FAMILIAR.P4_RECIBE_VISITA,
                                        P5_COMUNICACION_TELEFONICA = Entity.PFC_VI_SOCIO_FAMILIAR.P5_COMUNICACION_TELEFONICA,
                                        P5_NO_POR_QUE = Entity.PFC_VI_SOCIO_FAMILIAR.P5_NO_POR_QUE,
                                        P6_APOYO_EXTERIOR = Entity.PFC_VI_SOCIO_FAMILIAR.P6_APOYO_EXTERIOR,
                                        P7_PLANES_INTERNO = Entity.PFC_VI_SOCIO_FAMILIAR.P7_PLANES_INTERNO,
                                        P7_VIVIRA = Entity.PFC_VI_SOCIO_FAMILIAR.P7_VIVIRA,
                                        P8_OFERTA_ESPECIFICAR = Entity.PFC_VI_SOCIO_FAMILIAR.P8_OFERTA_ESPECIFICAR,
                                        P8_OFERTA_TRABAJO = Entity.PFC_VI_SOCIO_FAMILIAR.P8_OFERTA_TRABAJO,
                                        P9_AVAL_ESPECIFICAR = Entity.PFC_VI_SOCIO_FAMILIAR.P9_AVAL_ESPECIFICAR,
                                        P9_AVAL_MORAL = Entity.PFC_VI_SOCIO_FAMILIAR.P9_AVAL_MORAL
                                    };

                                    #region Listas Edicion SocioFamiliar Edicion comun
                                    if (_PadronVisitas != null && _PadronVisitas.Any())
                                        foreach (var item in _PadronVisitas)
                                            Context.Entry(item).State = System.Data.EntityState.Deleted;

                                    var _consecutivoFamiliares = GetIDProceso<short>("PFC_VI_COMUNICACION", "ID_CONSEC", string.Format("ID_CENTRO = {0} AND ID_ANIO = {1} AND ID_IMPUTADO = {2} AND ID_INGRESO = {3} AND ID_ESTUDIO = {4}", Entity.ID_CENTRO, Entity.ID_ANIO, Entity.ID_IMPUTADO, Entity.ID_INGRESO, Entity.ID_ESTUDIO));
                                    if (Entity.PFC_VI_SOCIO_FAMILIAR.PFC_VI_COMUNICACION != null && Entity.PFC_VI_SOCIO_FAMILIAR.PFC_VI_COMUNICACION.Any())
                                    {
                                        foreach (var item in Entity.PFC_VI_SOCIO_FAMILIAR.PFC_VI_COMUNICACION)
                                        {
                                            var _NuevaVisita = new PFC_VI_COMUNICACION()
                                            {
                                                FRECUENCIA = item.FRECUENCIA,
                                                ID_ANIO = item.ID_ANIO,
                                                ID_CENTRO = item.ID_CENTRO,
                                                ID_CONSEC = _consecutivoFamiliares,
                                                ID_ESTUDIO = ValidaEstudio.ID_ESTUDIO,
                                                ID_IMPUTADO = item.ID_IMPUTADO,
                                                ID_INGRESO = item.ID_INGRESO,
                                                ID_TIPO_REFERENCIA = item.ID_TIPO_REFERENCIA,
                                                NOMBRE = item.NOMBRE,
                                                TELEFONO = item.TELEFONO
                                            };

                                            ValidaEstudio.PFC_VI_SOCIO_FAMILIAR.PFC_VI_COMUNICACION.Add(_NuevaVisita);
                                            _consecutivoFamiliares++;
                                        };
                                    };

                                    if (_GruposSocioFamiliar != null && _GruposSocioFamiliar.Any())
                                        foreach (var item in _GruposSocioFamiliar)
                                            Context.Entry(item).State = System.Data.EntityState.Deleted;

                                    if (Entity.PFC_VI_SOCIO_FAMILIAR.PFC_VI_GRUPO != null && Entity.PFC_VI_SOCIO_FAMILIAR.PFC_VI_GRUPO.Any())
                                        foreach (var item in Entity.PFC_VI_SOCIO_FAMILIAR.PFC_VI_GRUPO)
                                        {
                                            var _NuevoGrupoSocioFamiliar = new PFC_VI_GRUPO()
                                            {
                                                CONGREGACION = item.CONGREGACION,
                                                ID_ACTIVIDAD = item.ID_ACTIVIDAD,
                                                ID_ANIO = item.ID_ANIO,
                                                ID_CENTRO = item.ID_CENTRO,
                                                ID_ESTUDIO = ValidaEstudio.ID_ESTUDIO,
                                                ID_IMPUTADO = item.ID_IMPUTADO,
                                                ID_INGRESO = item.ID_INGRESO,
                                                ID_TIPO_PROGRAMA = item.ID_TIPO_PROGRAMA,
                                                OBSERVACIONES = item.OBSERVACIONES,
                                                PERIODO = item.PERIODO
                                            };

                                            ValidaEstudio.PFC_VI_SOCIO_FAMILIAR.PFC_VI_GRUPO.Add(_NuevoGrupoSocioFamiliar);
                                        };

                                    #endregion
                                    Context.PFC_VI_SOCIO_FAMILIAR.Add(ValidaEstudio.PFC_VI_SOCIO_FAMILIAR);
                                }

                                #region Detalle
                                if (_EstudioPadre != null)
                                {
                                    short? _Estatus = new short?();
                                    var _DetallePsic = Context.PERSONALIDAD.FirstOrDefault(x => x.ID_ESTUDIO == _EstudioPadre.ID_ESTUDIO && x.ID_ANIO == _EstudioPadre.ID_ANIO && x.ID_CENTRO == _EstudioPadre.ID_CENTRO && x.ID_IMPUTADO == _EstudioPadre.ID_IMPUTADO && x.ID_INGRESO == _EstudioPadre.ID_INGRESO);
                                    if (string.IsNullOrEmpty(Entity.PFC_VI_SOCIO_FAMILIAR.COORDINADOR) || string.IsNullOrEmpty(Entity.PFC_VI_SOCIO_FAMILIAR.ELABORO) || string.IsNullOrEmpty(Entity.PFC_VI_SOCIO_FAMILIAR.P11_MOTIVACION_DICTAMEN) || string.IsNullOrEmpty(Entity.PFC_VI_SOCIO_FAMILIAR.P21_FAMILIA_PRIMARIA) || string.IsNullOrEmpty(Entity.PFC_VI_SOCIO_FAMILIAR.P22_FAMILIA_SECUNDARIA) || string.IsNullOrEmpty(Entity.PFC_VI_SOCIO_FAMILIAR.P3_TERCERA_EDAD) || string.IsNullOrEmpty(Entity.PFC_VI_SOCIO_FAMILIAR.P4_RECIBE_VISITA) || string.IsNullOrEmpty(Entity.PFC_VI_SOCIO_FAMILIAR.P5_COMUNICACION_TELEFONICA)
                                         || string.IsNullOrEmpty(Entity.PFC_VI_SOCIO_FAMILIAR.P6_APOYO_EXTERIOR) || string.IsNullOrEmpty(Entity.PFC_VI_SOCIO_FAMILIAR.P7_PLANES_INTERNO) || string.IsNullOrEmpty(Entity.PFC_VI_SOCIO_FAMILIAR.P7_VIVIRA) || string.IsNullOrEmpty(Entity.PFC_VI_SOCIO_FAMILIAR.P8_OFERTA_TRABAJO) || Entity.PFC_VI_SOCIO_FAMILIAR.ESTUDIO_FEC == null || Entity.PFC_VI_SOCIO_FAMILIAR.P10_DICTAMEN == null)
                                        _Estatus = (short)eEstatudDetallePersonalidad.ASIGNADO;
                                    else
                                        _Estatus = (short)eEstatudDetallePersonalidad.TERMINADO;

                                    if (_DetallePsic != null)
                                    {
                                        var _DesarrolloEstudioCrimi = Context.PERSONALIDAD_DETALLE.FirstOrDefault(c => c.ID_ESTUDIO == _DetallePsic.ID_ESTUDIO && c.ID_TIPO == (short)eTiposEstudio.TRABAJO_SOCIAL && c.ID_IMPUTADO == _DetallePsic.ID_IMPUTADO && c.ID_INGRESO == _DetallePsic.ID_INGRESO && c.ID_CENTRO == _DetallePsic.ID_CENTRO && c.ID_ANIO == _DetallePsic.ID_ANIO);
                                        if (_DesarrolloEstudioCrimi == null)
                                        {
                                            short _ConsecutivoPersonalidadDetalle = GetIDProceso<short>("PERSONALIDAD_DETALLE", "ID_DETALLE", "1=1");
                                            var _NvoDetalle = new PERSONALIDAD_DETALLE()
                                            {
                                                DIAS_BONIFICADOS = null,
                                                ID_ANIO = Entity.ID_ANIO,
                                                ID_CENTRO = Entity.ID_CENTRO,
                                                ID_DETALLE = _ConsecutivoPersonalidadDetalle,
                                                ID_ESTUDIO = _EstudioPadre.ID_ESTUDIO,
                                                ID_IMPUTADO = Entity.ID_IMPUTADO,
                                                ID_INGRESO = Entity.ID_INGRESO,
                                                ID_TIPO = (short)eTiposEstudio.TRABAJO_SOCIAL,
                                                INICIO_FEC = GetFechaServerDate(),
                                                RESULTADO = ValidaEstudio.PFC_VI_SOCIO_FAMILIAR != null ? ValidaEstudio.PFC_VI_SOCIO_FAMILIAR.P10_DICTAMEN.HasValue ? ValidaEstudio.PFC_VI_SOCIO_FAMILIAR.P10_DICTAMEN == (short)eResultado.FAVORABLE ? "S" : ValidaEstudio.PFC_V_CRIMINODIAGNOSTICO.P10_DICTAMEN_REINSERCION == (short)eResultado.DESFAVORABLE ? "N" : string.Empty : string.Empty : string.Empty,
                                                SOLICITUD_FEC = _EstudioPadre.SOLICITUD_FEC,
                                                ID_ESTATUS = _Estatus,
                                                TERMINO_FEC = _Estatus == (short)eEstatudDetallePersonalidad.TERMINADO ? GetFechaServerDate() : new System.DateTime?(),
                                                TIPO_MEDIA = string.Empty
                                            };

                                            Context.PERSONALIDAD_DETALLE.Add(_NvoDetalle);
                                        }

                                        else
                                        {
                                            _DesarrolloEstudioCrimi.RESULTADO = ValidaEstudio.PFC_VI_SOCIO_FAMILIAR != null ? ValidaEstudio.PFC_VI_SOCIO_FAMILIAR.P10_DICTAMEN.HasValue ? ValidaEstudio.PFC_VI_SOCIO_FAMILIAR.P10_DICTAMEN == (short)eResultado.FAVORABLE ? "S" : ValidaEstudio.PFC_V_CRIMINODIAGNOSTICO.P10_DICTAMEN_REINSERCION == (short)eResultado.DESFAVORABLE ? "N" : string.Empty : string.Empty : string.Empty;
                                            _DesarrolloEstudioCrimi.ID_ESTATUS = _Estatus;
                                            Context.Entry(_DesarrolloEstudioCrimi).State = System.Data.EntityState.Modified;
                                        }
                                    }
                                }
                                #endregion
                            }

                            #endregion

                            if (Entity.PFC_VII_EDUCATIVO != null)
                            {
                                var _EstudioEducativo = Context.PFC_VII_EDUCATIVO.FirstOrDefault(x => x.ID_IMPUTADO == Entity.ID_IMPUTADO && x.ID_INGRESO == Entity.ID_INGRESO && x.ID_ESTUDIO == Entity.ID_ESTUDIO && x.ID_CENTRO == Entity.ID_CENTRO);
                                var _Actividades = Context.PFC_VII_ACTIVIDAD.Where(c => c.ID_INGRESO == Entity.ID_INGRESO && c.ID_IMPUTADO == Entity.ID_IMPUTADO && c.ID_ESTUDIO == Entity.ID_ESTUDIO && c.ID_CENTRO == Entity.ID_CENTRO);
                                var _EscolaridadesAnteriores = Context.PFC_VII_ESCOLARIDAD_ANTERIOR.Where(x => x.ID_IMPUTADO == Entity.ID_IMPUTADO && x.ID_INGRESO == Entity.ID_INGRESO && x.ID_ESTUDIO == Entity.ID_ESTUDIO && x.ID_CENTRO == Entity.ID_CENTRO);
                                if (_EstudioEducativo != null)
                                {
                                    _EstudioEducativo.COORDINADOR = Entity.PFC_VII_EDUCATIVO.COORDINADOR;
                                    _EstudioEducativo.ELABORO = Entity.PFC_VII_EDUCATIVO.ELABORO;
                                    _EstudioEducativo.ESTUDIO_FEC = Entity.PFC_VII_EDUCATIVO.ESTUDIO_FEC;
                                    _EstudioEducativo.ID_ANIO = Entity.PFC_VII_EDUCATIVO.ID_ANIO;
                                    _EstudioEducativo.ID_CENTRO = Entity.PFC_VII_EDUCATIVO.ID_CENTRO;
                                    _EstudioEducativo.ID_ESTUDIO = ValidaEstudio.ID_ESTUDIO;
                                    _EstudioEducativo.ID_IMPUTADO = Entity.PFC_VII_EDUCATIVO.ID_IMPUTADO;
                                    _EstudioEducativo.ID_INGRESO = Entity.PFC_VII_EDUCATIVO.ID_INGRESO;
                                    _EstudioEducativo.P3_DICTAMEN = Entity.PFC_VII_EDUCATIVO.P3_DICTAMEN.HasValue ? Entity.PFC_VII_EDUCATIVO.P3_DICTAMEN.Value != decimal.Zero ? Entity.PFC_VII_EDUCATIVO.P3_DICTAMEN : null : null;
                                    _EstudioEducativo.P4_MOTIVACION_DICTAMEN = Entity.PFC_VII_EDUCATIVO.P4_MOTIVACION_DICTAMEN;
                                    Context.Entry(_EstudioEducativo).State = System.Data.EntityState.Modified;

                                    if (_Actividades != null && _Actividades.Any())
                                        foreach (var item in _Actividades)
                                            Context.Entry(item).State = System.Data.EntityState.Deleted;

                                    var _consecutivoTrabajo = GetIDProceso<short>("PFC_VII_ACTIVIDAD", "ID_CONSEC", string.Format("ID_CENTRO = {0} AND ID_ANIO = {1} AND ID_IMPUTADO = {2} AND ID_INGRESO = {3} AND ID_ESTUDIO = {4}", _EstudioPadre.ID_CENTRO, _EstudioPadre.ID_ANIO, _EstudioPadre.ID_IMPUTADO, _EstudioPadre.ID_INGRESO, _EstudioPadre.ID_ESTUDIO));
                                    if (Entity.PFC_VII_EDUCATIVO.PFC_VII_ACTIVIDAD != null && Entity.PFC_VII_EDUCATIVO.PFC_VII_ACTIVIDAD.Any())
                                        foreach (var item in Entity.PFC_VII_EDUCATIVO.PFC_VII_ACTIVIDAD)
                                        {
                                            var _NuevaActividad = new PFC_VII_ACTIVIDAD()
                                            {
                                                ACTIVIDAD = item.ACTIVIDAD,
                                                DURACION = item.DURACION,
                                                ID_ANIO = item.ID_ANIO,
                                                ID_CENTRO = item.ID_CENTRO,
                                                ID_ESTUDIO = ValidaEstudio.ID_ESTUDIO,
                                                ID_IMPUTADO = item.ID_IMPUTADO,
                                                ID_INGRESO = item.ID_INGRESO,
                                                ID_CONSEC = _consecutivoTrabajo,
                                                ID_PROGRAMA = null,//QUITAR DE LA LLAVE Y AGREGAR UN CONSECUTIVO
                                                OBSERVACION = item.OBSERVACION,
                                                ID_ACTIVIDAD = item.ID_ACTIVIDAD,
                                                ID_TIPO_PROGRAMA = item.ID_TIPO_PROGRAMA,
                                                TIPO = item.TIPO
                                            };

                                            _EstudioEducativo.PFC_VII_ACTIVIDAD.Add(_NuevaActividad);
                                            _consecutivoTrabajo++;
                                        };

                                    if (_EscolaridadesAnteriores != null && _EscolaridadesAnteriores.Any())
                                        foreach (var item in _EscolaridadesAnteriores)
                                            Context.Entry(item).State = System.Data.EntityState.Deleted;

                                    var _consecutivoEduca = GetIDProceso<short>("PFC_VII_ESCOLARIDAD_ANTERIOR", "ID_CONSEC", string.Format("ID_CENTRO = {0} AND ID_ANIO = {1} AND ID_IMPUTADO = {2} AND ID_INGRESO = {3} AND ID_ESTUDIO = {4}", Entity.ID_CENTRO, Entity.ID_ANIO, Entity.ID_IMPUTADO, Entity.ID_INGRESO, Entity.ID_ESTUDIO));

                                    if (Entity.PFC_VII_EDUCATIVO.PFC_VII_ESCOLARIDAD_ANTERIOR != null && Entity.PFC_VII_EDUCATIVO.PFC_VII_ESCOLARIDAD_ANTERIOR.Any())
                                        foreach (var item in Entity.PFC_VII_EDUCATIVO.PFC_VII_ESCOLARIDAD_ANTERIOR)
                                        {
                                            var _NuevaEscolaridad = new PFC_VII_ESCOLARIDAD_ANTERIOR()
                                            {
                                                CONCLUIDA = item.CONCLUIDA,
                                                ID_ANIO = item.ID_ANIO,
                                                ID_CENTRO = item.ID_CENTRO,
                                                ID_ESTUDIO = ValidaEstudio.ID_ESTUDIO,
                                                ID_GRADO = item.ID_GRADO,
                                                ID_IMPUTADO = item.ID_IMPUTADO,
                                                ID_INGRESO = item.ID_INGRESO,
                                                INTERES = item.INTERES,
                                                OBSERVACION = item.OBSERVACION,
                                                RENDIMIENTO = item.RENDIMIENTO,
                                                ID_CONSEC = _consecutivoEduca
                                            };

                                            _EstudioEducativo.PFC_VII_ESCOLARIDAD_ANTERIOR.Add(_NuevaEscolaridad);
                                            _consecutivoEduca++;
                                        };
                                }

                                else
                                {
                                    ValidaEstudio.PFC_VII_EDUCATIVO = new PFC_VII_EDUCATIVO()
                                    {
                                        COORDINADOR = Entity.PFC_VII_EDUCATIVO.COORDINADOR,
                                        ELABORO = Entity.PFC_VII_EDUCATIVO.ELABORO,
                                        ESTUDIO_FEC = Entity.PFC_VII_EDUCATIVO.ESTUDIO_FEC,
                                        ID_ANIO = Entity.ID_ANIO,
                                        ID_CENTRO = Entity.ID_CENTRO,
                                        ID_ESTUDIO = Entity.ID_ESTUDIO,
                                        ID_IMPUTADO = Entity.ID_IMPUTADO,
                                        ID_INGRESO = Entity.ID_INGRESO,
                                        P3_DICTAMEN = Entity.PFC_VII_EDUCATIVO.P3_DICTAMEN.HasValue ? Entity.PFC_VII_EDUCATIVO.P3_DICTAMEN.Value != decimal.Zero ? Entity.PFC_VII_EDUCATIVO.P3_DICTAMEN : null : null,
                                        P4_MOTIVACION_DICTAMEN = Entity.PFC_VII_EDUCATIVO.P4_MOTIVACION_DICTAMEN
                                    };

                                    if (_Actividades != null && _Actividades.Any())
                                        foreach (var item in _Actividades)
                                            Context.Entry(item).State = System.Data.EntityState.Deleted;

                                    var _consecutivoTrabajo = GetIDProceso<short>("PFC_VII_ACTIVIDAD", "ID_CONSEC", string.Format("ID_CENTRO = {0} AND ID_ANIO = {1} AND ID_IMPUTADO = {2} AND ID_INGRESO = {3} AND ID_ESTUDIO = {4}", _EstudioPadre.ID_CENTRO, _EstudioPadre.ID_ANIO, _EstudioPadre.ID_IMPUTADO, _EstudioPadre.ID_INGRESO, _EstudioPadre.ID_ESTUDIO));
                                    if (Entity.PFC_VII_EDUCATIVO.PFC_VII_ACTIVIDAD != null && Entity.PFC_VII_EDUCATIVO.PFC_VII_ACTIVIDAD.Any())
                                        foreach (var item in Entity.PFC_VII_EDUCATIVO.PFC_VII_ACTIVIDAD)
                                        {
                                            var _NuevaActividad = new PFC_VII_ACTIVIDAD()
                                            {
                                                ACTIVIDAD = item.ACTIVIDAD,
                                                DURACION = item.DURACION,
                                                ID_ANIO = _EstudioPadre.ID_ANIO,
                                                ID_CENTRO = _EstudioPadre.ID_CENTRO,
                                                ID_ESTUDIO = _EstudioPadre.ID_ESTUDIO,
                                                ID_IMPUTADO = _EstudioPadre.ID_IMPUTADO,
                                                ID_INGRESO = _EstudioPadre.ID_INGRESO,
                                                ID_PROGRAMA = null,
                                                ID_CONSEC = _consecutivoTrabajo,
                                                OBSERVACION = item.OBSERVACION,
                                                TIPO = item.TIPO,
                                                ID_ACTIVIDAD = item.ID_ACTIVIDAD,
                                                ID_TIPO_PROGRAMA = item.ID_TIPO_PROGRAMA
                                            };

                                            ValidaEstudio.PFC_VII_EDUCATIVO.PFC_VII_ACTIVIDAD.Add(_NuevaActividad);
                                            _consecutivoTrabajo++;
                                        };

                                    if (_EscolaridadesAnteriores != null && _EscolaridadesAnteriores.Any())
                                        foreach (var item in _EscolaridadesAnteriores)
                                            Context.Entry(item).State = System.Data.EntityState.Deleted;

                                    var _consecutivoEduca = GetIDProceso<short>("PFC_VII_ESCOLARIDAD_ANTERIOR", "ID_CONSEC", string.Format("ID_CENTRO = {0} AND ID_ANIO = {1} AND ID_IMPUTADO = {2} AND ID_INGRESO = {3} AND ID_ESTUDIO = {4}", Entity.ID_CENTRO, Entity.ID_ANIO, Entity.ID_IMPUTADO, Entity.ID_INGRESO, Entity.ID_ESTUDIO));
                                    if (Entity.PFC_VII_EDUCATIVO.PFC_VII_ESCOLARIDAD_ANTERIOR != null && Entity.PFC_VII_EDUCATIVO.PFC_VII_ESCOLARIDAD_ANTERIOR.Any())
                                        foreach (var item in Entity.PFC_VII_EDUCATIVO.PFC_VII_ESCOLARIDAD_ANTERIOR)
                                        {
                                            var _NuevaEscolaridad = new PFC_VII_ESCOLARIDAD_ANTERIOR()
                                            {
                                                CONCLUIDA = item.CONCLUIDA,
                                                ID_ANIO = item.ID_ANIO,
                                                ID_CENTRO = item.ID_CENTRO,
                                                ID_ESTUDIO = ValidaEstudio.ID_ESTUDIO,
                                                ID_GRADO = item.ID_GRADO,
                                                ID_IMPUTADO = item.ID_IMPUTADO,
                                                ID_INGRESO = item.ID_INGRESO,
                                                INTERES = item.INTERES,
                                                OBSERVACION = item.OBSERVACION,
                                                RENDIMIENTO = item.RENDIMIENTO,
                                                ID_CONSEC = _consecutivoEduca,
                                            };

                                            ValidaEstudio.PFC_VII_EDUCATIVO.PFC_VII_ESCOLARIDAD_ANTERIOR.Add(_NuevaEscolaridad);
                                            _consecutivoEduca++;
                                        };

                                    Context.PFC_VII_EDUCATIVO.Add(ValidaEstudio.PFC_VII_EDUCATIVO);

                                }

                                #region Detalle VII
                                if (_EstudioPadre != null)
                                {
                                    short? _Estatus = new short?();
                                    var _DetalleEduc = Context.PERSONALIDAD.FirstOrDefault(x => x.ID_ESTUDIO == _EstudioPadre.ID_ESTUDIO && x.ID_ANIO == _EstudioPadre.ID_ANIO && x.ID_CENTRO == _EstudioPadre.ID_CENTRO && x.ID_IMPUTADO == _EstudioPadre.ID_IMPUTADO && x.ID_INGRESO == _EstudioPadre.ID_INGRESO);
                                    if (string.IsNullOrEmpty(Entity.PFC_VII_EDUCATIVO.COORDINADOR) || string.IsNullOrEmpty(Entity.PFC_VII_EDUCATIVO.ELABORO) || string.IsNullOrEmpty(Entity.PFC_VII_EDUCATIVO.P4_MOTIVACION_DICTAMEN) || Entity.PFC_VII_EDUCATIVO.ESTUDIO_FEC == null || Entity.PFC_VII_EDUCATIVO.P3_DICTAMEN == null)
                                        _Estatus = (short)eEstatudDetallePersonalidad.ASIGNADO;
                                    else
                                        _Estatus = (short)eEstatudDetallePersonalidad.TERMINADO;

                                    if (_DetalleEduc != null)
                                    {
                                        var _DesarrolloEstudioCrimi = Context.PERSONALIDAD_DETALLE.FirstOrDefault(c => c.ID_ESTUDIO == _DetalleEduc.ID_ESTUDIO && c.ID_TIPO == (short)eTiposEstudio.PEDAGOGIA && c.ID_IMPUTADO == _DetalleEduc.ID_IMPUTADO && c.ID_INGRESO == _DetalleEduc.ID_INGRESO && c.ID_CENTRO == _DetalleEduc.ID_CENTRO && c.ID_ANIO == _DetalleEduc.ID_ANIO);
                                        if (_DesarrolloEstudioCrimi == null)
                                        {
                                            short _ConsecutivoPersonalidadDetalle = GetIDProceso<short>("PERSONALIDAD_DETALLE", "ID_DETALLE", "1=1");
                                            var _NvoDetalle = new PERSONALIDAD_DETALLE()
                                            {
                                                DIAS_BONIFICADOS = null,
                                                ID_ANIO = Entity.ID_ANIO,
                                                ID_CENTRO = Entity.ID_CENTRO,
                                                ID_DETALLE = _ConsecutivoPersonalidadDetalle,
                                                ID_ESTUDIO = _EstudioPadre.ID_ESTUDIO,
                                                ID_IMPUTADO = Entity.ID_IMPUTADO,
                                                ID_INGRESO = Entity.ID_INGRESO,
                                                ID_TIPO = (short)eTiposEstudio.PEDAGOGIA,
                                                INICIO_FEC = GetFechaServerDate(),
                                                RESULTADO = ValidaEstudio.PFC_VII_EDUCATIVO != null ? ValidaEstudio.PFC_VII_EDUCATIVO.P3_DICTAMEN.HasValue ? ValidaEstudio.PFC_VII_EDUCATIVO.P3_DICTAMEN == (short)eResultado.FAVORABLE ? "S" : ValidaEstudio.PFC_VII_EDUCATIVO.P3_DICTAMEN == (short)eResultado.DESFAVORABLE ? "N" : string.Empty : string.Empty : string.Empty,
                                                SOLICITUD_FEC = _EstudioPadre.SOLICITUD_FEC,
                                                ID_ESTATUS = _Estatus,
                                                TERMINO_FEC = _Estatus == (short)eEstatudDetallePersonalidad.TERMINADO ? GetFechaServerDate() : new System.DateTime?(),
                                                TIPO_MEDIA = string.Empty
                                            };

                                            Context.PERSONALIDAD_DETALLE.Add(_NvoDetalle);
                                        }

                                        else
                                        {
                                            _DesarrolloEstudioCrimi.RESULTADO = ValidaEstudio.PFC_VII_EDUCATIVO != null ? ValidaEstudio.PFC_VII_EDUCATIVO.P3_DICTAMEN.HasValue ? ValidaEstudio.PFC_VII_EDUCATIVO.P3_DICTAMEN == (short)eResultado.FAVORABLE ? "S" : ValidaEstudio.PFC_VII_EDUCATIVO.P3_DICTAMEN == (short)eResultado.DESFAVORABLE ? "N" : string.Empty : string.Empty : string.Empty;
                                            _DesarrolloEstudioCrimi.ID_ESTATUS = _Estatus;
                                            Context.Entry(_DesarrolloEstudioCrimi).State = System.Data.EntityState.Modified;
                                        }
                                    }
                                }
                                #endregion

                            }

                            if (Entity.PFC_VIII_TRABAJO != null)
                            {
                                var _EstudioTrabajo = Context.PFC_VIII_TRABAJO.FirstOrDefault(x => x.ID_INGRESO == Entity.ID_INGRESO && x.ID_IMPUTADO == Entity.ID_IMPUTADO && x.ID_ESTUDIO == Entity.ID_ESTUDIO && x.ID_CENTRO == Entity.ID_CENTRO);
                                var ListaDatosAnteriores = Context.PFC_VIII_ACTIVIDAD_LABORAL.Where(x => x.ID_INGRESO == Entity.ID_INGRESO && x.ID_IMPUTADO == Entity.ID_IMPUTADO && x.ID_ESTUDIO == Entity.ID_ESTUDIO && x.ID_CENTRO == Entity.ID_CENTRO);
                                if (_EstudioTrabajo != null)
                                {
                                    if (Entity.PFC_VIII_TRABAJO != null)
                                    {
                                        _EstudioTrabajo.ID_ANIO = Entity.PFC_VIII_TRABAJO.ID_ANIO;
                                        _EstudioTrabajo.ID_CENTRO = Entity.PFC_VIII_TRABAJO.ID_CENTRO;
                                        _EstudioTrabajo.ID_ESTUDIO = ValidaEstudio.ID_ESTUDIO;
                                        _EstudioTrabajo.ID_IMPUTADO = Entity.PFC_VIII_TRABAJO.ID_IMPUTADO;
                                        _EstudioTrabajo.ID_INGRESO = Entity.PFC_VIII_TRABAJO.ID_INGRESO;
                                        _EstudioTrabajo.COORDINADOR = Entity.PFC_VIII_TRABAJO.COORDINADOR;
                                        _EstudioTrabajo.ELABORO = Entity.PFC_VIII_TRABAJO.ELABORO;
                                        _EstudioTrabajo.ESTUDIO_FEC = Entity.PFC_VIII_TRABAJO.ESTUDIO_FEC;
                                        _EstudioTrabajo.P1_TRABAJO_ANTES = Entity.PFC_VIII_TRABAJO.P1_TRABAJO_ANTES;
                                        _EstudioTrabajo.P3_CALIDAD = Entity.PFC_VIII_TRABAJO.P3_CALIDAD;
                                        _EstudioTrabajo.P3_PERSEVERANCIA = Entity.PFC_VIII_TRABAJO.P3_PERSEVERANCIA;
                                        _EstudioTrabajo.P3_RESPONSABILIDAD = Entity.PFC_VIII_TRABAJO.P3_RESPONSABILIDAD;
                                        _EstudioTrabajo.P4_FONDO_AHORRO = Entity.PFC_VIII_TRABAJO.P4_FONDO_AHORRO;
                                        _EstudioTrabajo.P5_DIAS_CENTRO_ACTUAL = Entity.PFC_VIII_TRABAJO.P5_DIAS_CENTRO_ACTUAL;
                                        _EstudioTrabajo.P5_DIAS_LABORADOS = Entity.PFC_VIII_TRABAJO.P5_DIAS_LABORADOS;
                                        _EstudioTrabajo.P5_DIAS_OTROS_CENTROS = Entity.PFC_VIII_TRABAJO.P5_DIAS_OTROS_CENTROS;
                                        _EstudioTrabajo.P5_PERIODO_LABORAL = Entity.PFC_VIII_TRABAJO.P5_PERIODO_LABORAL;
                                        _EstudioTrabajo.P6_DICTAMEN = Entity.PFC_VIII_TRABAJO.P6_DICTAMEN;
                                        _EstudioTrabajo.P7_MOTIVACION = Entity.PFC_VIII_TRABAJO.P7_MOTIVACION;
                                        Context.Entry(_EstudioTrabajo).State = System.Data.EntityState.Modified;

                                        if (ListaDatosAnteriores != null && ListaDatosAnteriores.Any())
                                            foreach (var item in ListaDatosAnteriores)
                                                Context.Entry(item).State = System.Data.EntityState.Deleted;

                                        var _consecutivoLaboral = GetIDProceso<short>("PFC_VIII_ACTIVIDAD_LABORAL", "ID_CONSEC", string.Format("ID_CENTRO = {0} AND ID_ANIO = {1} AND ID_IMPUTADO = {2} AND ID_INGRESO = {3} AND ID_ESTUDIO = {4}", Entity.ID_CENTRO, Entity.ID_ANIO, Entity.ID_IMPUTADO, Entity.ID_INGRESO, Entity.ID_ESTUDIO));

                                        if (Entity.PFC_VIII_TRABAJO.PFC_VIII_ACTIVIDAD_LABORAL != null && Entity.PFC_VIII_TRABAJO.PFC_VIII_ACTIVIDAD_LABORAL.Any())
                                            foreach (var item in Entity.PFC_VIII_TRABAJO.PFC_VIII_ACTIVIDAD_LABORAL)
                                            {
                                                var NvaActividadLaboral = new PFC_VIII_ACTIVIDAD_LABORAL()
                                                {
                                                    CONCLUYO = item.CONCLUYO,
                                                    ID_ANIO = Entity.ID_ANIO,
                                                    ID_CAPACITACION = null,
                                                    ID_CENTRO = Entity.ID_CENTRO,
                                                    ID_CONSEC = _consecutivoLaboral,
                                                    ID_ESTUDIO = Entity.ID_ESTUDIO,
                                                    ID_IMPUTADO = Entity.ID_IMPUTADO,
                                                    ID_INGRESO = Entity.ID_INGRESO,
                                                    OBSERVACION = item.OBSERVACION,
                                                    PERIODO = item.PERIODO,
                                                    ID_ACTIVIDAD = item.ID_ACTIVIDAD,
                                                    ID_TIPO_PROGRAMA = item.ID_TIPO_PROGRAMA
                                                };

                                                Context.PFC_VIII_ACTIVIDAD_LABORAL.Add(NvaActividadLaboral);
                                                _consecutivoLaboral++;
                                            };
                                    }
                                }

                                else
                                {
                                    ValidaEstudio.PFC_VIII_TRABAJO = new PFC_VIII_TRABAJO()
                                    {
                                        COORDINADOR = Entity.PFC_VIII_TRABAJO.COORDINADOR,
                                        ELABORO = Entity.PFC_VIII_TRABAJO.ELABORO,
                                        ESTUDIO_FEC = Entity.PFC_VIII_TRABAJO.ESTUDIO_FEC,
                                        ID_ANIO = Entity.ID_ANIO,
                                        ID_CENTRO = Entity.ID_CENTRO,
                                        ID_ESTUDIO = Entity.ID_ESTUDIO,
                                        ID_IMPUTADO = Entity.ID_IMPUTADO,
                                        ID_INGRESO = Entity.ID_INGRESO,
                                        P1_TRABAJO_ANTES = Entity.PFC_VIII_TRABAJO.P1_TRABAJO_ANTES,
                                        P3_CALIDAD = Entity.PFC_VIII_TRABAJO.P3_CALIDAD,
                                        P3_PERSEVERANCIA = Entity.PFC_VIII_TRABAJO.P3_PERSEVERANCIA,
                                        P3_RESPONSABILIDAD = Entity.PFC_VIII_TRABAJO.P3_RESPONSABILIDAD,
                                        P4_FONDO_AHORRO = Entity.PFC_VIII_TRABAJO.P4_FONDO_AHORRO,
                                        P5_DIAS_CENTRO_ACTUAL = Entity.PFC_VIII_TRABAJO.P5_DIAS_CENTRO_ACTUAL,
                                        P5_DIAS_LABORADOS = Entity.PFC_VIII_TRABAJO.P5_DIAS_LABORADOS,
                                        P5_DIAS_OTROS_CENTROS = Entity.PFC_VIII_TRABAJO.P5_DIAS_OTROS_CENTROS,
                                        P5_PERIODO_LABORAL = Entity.PFC_VIII_TRABAJO.P5_PERIODO_LABORAL,
                                        P6_DICTAMEN = Entity.PFC_VIII_TRABAJO.P6_DICTAMEN,
                                        P7_MOTIVACION = Entity.PFC_VIII_TRABAJO.P7_MOTIVACION
                                    };

                                    if (ListaDatosAnteriores != null && ListaDatosAnteriores.Any())
                                        foreach (var item in ListaDatosAnteriores)
                                            Context.Entry(item).State = System.Data.EntityState.Deleted;

                                    var _consecutivoLaboral = GetIDProceso<short>("PFC_VIII_ACTIVIDAD_LABORAL", "ID_CONSEC", string.Format("ID_CENTRO = {0} AND ID_ANIO = {1} AND ID_IMPUTADO = {2} AND ID_INGRESO = {3} AND ID_ESTUDIO = {4}", Entity.ID_CENTRO, Entity.ID_ANIO, Entity.ID_IMPUTADO, Entity.ID_INGRESO, Entity.ID_ESTUDIO));

                                    if (Entity.PFC_VIII_TRABAJO.PFC_VIII_ACTIVIDAD_LABORAL != null && Entity.PFC_VIII_TRABAJO.PFC_VIII_ACTIVIDAD_LABORAL.Any())
                                        foreach (var item in Entity.PFC_VIII_TRABAJO.PFC_VIII_ACTIVIDAD_LABORAL)
                                        {
                                            var NvaActividadLaboral = new PFC_VIII_ACTIVIDAD_LABORAL()
                                            {
                                                CONCLUYO = item.CONCLUYO,
                                                ID_ANIO = Entity.ID_ANIO,
                                                ID_CAPACITACION = null,
                                                ID_CENTRO = Entity.ID_CENTRO,
                                                ID_CONSEC = _consecutivoLaboral,
                                                ID_ESTUDIO = Entity.ID_ESTUDIO,
                                                ID_IMPUTADO = Entity.ID_IMPUTADO,
                                                ID_INGRESO = Entity.ID_INGRESO,
                                                OBSERVACION = item.OBSERVACION,
                                                PERIODO = item.PERIODO,
                                                ID_ACTIVIDAD = item.ID_ACTIVIDAD,
                                                ID_TIPO_PROGRAMA = item.ID_TIPO_PROGRAMA
                                            };

                                            Context.PFC_VIII_ACTIVIDAD_LABORAL.Add(NvaActividadLaboral);
                                            _consecutivoLaboral++;
                                        };

                                    Context.PFC_VIII_TRABAJO.Add(ValidaEstudio.PFC_VIII_TRABAJO);
                                }


                                #region Detalle
                                if (_EstudioPadre != null)
                                {
                                    short? _Estatus = new short?();
                                    var _DetalleEduc = Context.PERSONALIDAD.FirstOrDefault(x => x.ID_ESTUDIO == _EstudioPadre.ID_ESTUDIO && x.ID_ANIO == _EstudioPadre.ID_ANIO && x.ID_CENTRO == _EstudioPadre.ID_CENTRO && x.ID_IMPUTADO == _EstudioPadre.ID_IMPUTADO && x.ID_INGRESO == _EstudioPadre.ID_INGRESO);
                                    if (string.IsNullOrEmpty(Entity.PFC_VIII_TRABAJO.COORDINADOR) || string.IsNullOrEmpty(Entity.PFC_VIII_TRABAJO.ELABORO) || string.IsNullOrEmpty(Entity.PFC_VIII_TRABAJO.P1_TRABAJO_ANTES) || string.IsNullOrEmpty(Entity.PFC_VIII_TRABAJO.P3_CALIDAD)
                                        || string.IsNullOrEmpty(Entity.PFC_VIII_TRABAJO.P3_PERSEVERANCIA) || string.IsNullOrEmpty(Entity.PFC_VIII_TRABAJO.P3_RESPONSABILIDAD) || string.IsNullOrEmpty(Entity.PFC_VIII_TRABAJO.P4_FONDO_AHORRO) || string.IsNullOrEmpty(Entity.PFC_VIII_TRABAJO.P6_DICTAMEN)
                                        || string.IsNullOrEmpty(Entity.PFC_VIII_TRABAJO.P7_MOTIVACION) || Entity.PFC_VIII_TRABAJO.ESTUDIO_FEC == null || Entity.PFC_VIII_TRABAJO.P5_DIAS_CENTRO_ACTUAL == null || Entity.PFC_VIII_TRABAJO.P5_DIAS_LABORADOS == null || Entity.PFC_VIII_TRABAJO.P5_DIAS_OTROS_CENTROS == null
                                        || Entity.PFC_VIII_TRABAJO.P5_PERIODO_LABORAL == null)
                                        _Estatus = (short)eEstatudDetallePersonalidad.ASIGNADO;
                                    else
                                        _Estatus = (short)eEstatudDetallePersonalidad.TERMINADO;

                                    if (_DetalleEduc != null)
                                    {
                                        var _DesarrolloEstudioCrimi = Context.PERSONALIDAD_DETALLE.FirstOrDefault(c => c.ID_ESTUDIO == _DetalleEduc.ID_ESTUDIO && c.ID_TIPO == (short)eTiposEstudio.LABORAL && c.ID_IMPUTADO == _DetalleEduc.ID_IMPUTADO && c.ID_INGRESO == _DetalleEduc.ID_INGRESO && c.ID_CENTRO == _DetalleEduc.ID_CENTRO && c.ID_ANIO == _DetalleEduc.ID_ANIO);
                                        if (_DesarrolloEstudioCrimi == null)
                                        {
                                            short _ConsecutivoPersonalidadDetalle = GetIDProceso<short>("PERSONALIDAD_DETALLE", "ID_DETALLE", "1=1");
                                            var _NvoDetalle = new PERSONALIDAD_DETALLE()
                                            {
                                                DIAS_BONIFICADOS = null,
                                                ID_ANIO = Entity.ID_ANIO,
                                                ID_CENTRO = Entity.ID_CENTRO,
                                                ID_DETALLE = _ConsecutivoPersonalidadDetalle,
                                                ID_ESTUDIO = _EstudioPadre.ID_ESTUDIO,
                                                ID_IMPUTADO = Entity.ID_IMPUTADO,
                                                ID_INGRESO = Entity.ID_INGRESO,
                                                ID_TIPO = (short)eTiposEstudio.LABORAL,
                                                INICIO_FEC = GetFechaServerDate(),
                                                RESULTADO = ValidaEstudio.PFC_VIII_TRABAJO.P6_DICTAMEN == "S" ? "S" : ValidaEstudio.PFC_VIII_TRABAJO.P6_DICTAMEN == "D" ? "N" : string.Empty,
                                                SOLICITUD_FEC = _EstudioPadre.SOLICITUD_FEC,
                                                ID_ESTATUS = _Estatus,
                                                TERMINO_FEC = _Estatus == (short)eEstatudDetallePersonalidad.TERMINADO ? GetFechaServerDate() : new System.DateTime?(),
                                                TIPO_MEDIA = string.Empty
                                            };

                                            Context.PERSONALIDAD_DETALLE.Add(_NvoDetalle);
                                        }

                                        else
                                        {
                                            _DesarrolloEstudioCrimi.RESULTADO = ValidaEstudio.PFC_VIII_TRABAJO.P6_DICTAMEN == "S" ? "S" : ValidaEstudio.PFC_VIII_TRABAJO.P6_DICTAMEN == "D" ? "N" : string.Empty;
                                            _DesarrolloEstudioCrimi.ID_ESTATUS = _Estatus;
                                            Context.Entry(_DesarrolloEstudioCrimi).State = System.Data.EntityState.Modified;
                                        }
                                    }
                                }

                                #endregion

                            }

                            if (Entity.PFC_IX_SEGURIDAD != null)
                            {
                                var _EstudioSeguridadCustodia = Context.PFC_IX_SEGURIDAD.FirstOrDefault(x => x.ID_IMPUTADO == Entity.ID_IMPUTADO && x.ID_INGRESO == Entity.ID_INGRESO && x.ID_ESTUDIO == Entity.ID_ESTUDIO && x.ID_CENTRO == Entity.ID_CENTRO);
                                var CorrectivosSeguridadComun = Context.PFC_IX_CORRECTIVO.Where(x => x.ID_IMPUTADO == Entity.ID_IMPUTADO && x.ID_INGRESO == Entity.ID_INGRESO && x.ID_ESTUDIO == Entity.ID_ESTUDIO && x.ID_CENTRO == Entity.ID_CENTRO);
                                if (_EstudioSeguridadCustodia != null)
                                {
                                    _EstudioSeguridadCustodia.COMANDANTE = Entity.PFC_IX_SEGURIDAD.COMANDANTE;
                                    _EstudioSeguridadCustodia.ELABORO = Entity.PFC_IX_SEGURIDAD.ELABORO;
                                    _EstudioSeguridadCustodia.ESTUDIO_FEC = Entity.PFC_IX_SEGURIDAD.ESTUDIO_FEC;
                                    _EstudioSeguridadCustodia.ID_ANIO = Entity.PFC_IX_SEGURIDAD.ID_ANIO;
                                    _EstudioSeguridadCustodia.ID_CENTRO = Entity.PFC_IX_SEGURIDAD.ID_CENTRO;
                                    _EstudioSeguridadCustodia.ID_ESTUDIO = ValidaEstudio.ID_ESTUDIO;
                                    _EstudioSeguridadCustodia.ID_IMPUTADO = Entity.PFC_IX_SEGURIDAD.ID_IMPUTADO;
                                    _EstudioSeguridadCustodia.ID_INGRESO = Entity.PFC_IX_SEGURIDAD.ID_INGRESO;
                                    _EstudioSeguridadCustodia.P1_CONDUCTA_CENTRO = Entity.PFC_IX_SEGURIDAD.P1_CONDUCTA_CENTRO;
                                    _EstudioSeguridadCustodia.P2_CONDUCTA_AUTORIDAD = Entity.PFC_IX_SEGURIDAD.P2_CONDUCTA_AUTORIDAD;
                                    _EstudioSeguridadCustodia.P3_CONDUCTA_GENERAL = Entity.PFC_IX_SEGURIDAD.P3_CONDUCTA_GENERAL.HasValue ? Entity.PFC_IX_SEGURIDAD.P3_CONDUCTA_GENERAL.Value != -1 ? Entity.PFC_IX_SEGURIDAD.P3_CONDUCTA_GENERAL : null : null;
                                    _EstudioSeguridadCustodia.P4_RELACION_COMPANEROS = Entity.PFC_IX_SEGURIDAD.P4_RELACION_COMPANEROS.HasValue ? Entity.PFC_IX_SEGURIDAD.P4_RELACION_COMPANEROS.Value != -1 ? Entity.PFC_IX_SEGURIDAD.P4_RELACION_COMPANEROS : null : null;
                                    _EstudioSeguridadCustodia.P5_CORRECTIVOS = Entity.PFC_IX_SEGURIDAD.P5_CORRECTIVOS;
                                    _EstudioSeguridadCustodia.P6_OPINION_CONDUCTA = Entity.PFC_IX_SEGURIDAD.P6_OPINION_CONDUCTA;
                                    _EstudioSeguridadCustodia.P7_DICTAMEN = Entity.PFC_IX_SEGURIDAD.P7_DICTAMEN;
                                    _EstudioSeguridadCustodia.P8_MOTIVACION = Entity.PFC_IX_SEGURIDAD.P8_MOTIVACION;
                                    Context.Entry(_EstudioSeguridadCustodia).State = System.Data.EntityState.Modified;

                                    #region Correctivos Seguridad Comun
                                    if (CorrectivosSeguridadComun != null && CorrectivosSeguridadComun.Any())
                                        foreach (var item in CorrectivosSeguridadComun)
                                            Context.Entry(item).State = System.Data.EntityState.Deleted;

                                    var _consecutivoCorrectivos = GetIDProceso<short>("PFC_IX_CORRECTIVO", "ID_CORRECTIVO", string.Format("ID_CENTRO = {0} AND ID_ANIO = {1} AND ID_IMPUTADO = {2} AND ID_INGRESO = {3} AND ID_ESTUDIO = {4}", Entity.ID_CENTRO, Entity.ID_ANIO, Entity.ID_IMPUTADO, Entity.ID_INGRESO, Entity.ID_ESTUDIO));
                                    if (Entity.PFC_IX_SEGURIDAD.PFC_IX_CORRECTIVO != null)
                                        if (Entity.PFC_IX_SEGURIDAD.PFC_IX_CORRECTIVO.Any())
                                            foreach (var item in Entity.PFC_IX_SEGURIDAD.PFC_IX_CORRECTIVO)
                                            {
                                                var _NuevoCorrectivo = new PFC_IX_CORRECTIVO()
                                                {
                                                    CORRECTIVO_FEC = item.CORRECTIVO_FEC,
                                                    ID_ANIO = Entity.ID_ANIO,
                                                    ID_CENTRO = Entity.ID_CENTRO,
                                                    ID_CORRECTIVO = _consecutivoCorrectivos,
                                                    ID_ESTUDIO = item.ID_ESTUDIO,
                                                    ID_IMPUTADO = Entity.ID_IMPUTADO,
                                                    ID_INGRESO = Entity.ID_INGRESO,
                                                    MOTIVO = item.MOTIVO,
                                                    SANCION = item.SANCION
                                                };

                                                _EstudioSeguridadCustodia.PFC_IX_CORRECTIVO.Add(_NuevoCorrectivo);
                                                _consecutivoCorrectivos++;
                                            };

                                    #endregion
                                }
                                else
                                {
                                    ValidaEstudio.PFC_IX_SEGURIDAD = new PFC_IX_SEGURIDAD()
                                    {
                                        COMANDANTE = Entity.PFC_IX_SEGURIDAD.COMANDANTE,
                                        ELABORO = Entity.PFC_IX_SEGURIDAD.ELABORO,
                                        ESTUDIO_FEC = Entity.PFC_IX_SEGURIDAD.ESTUDIO_FEC,
                                        ID_ANIO = Entity.ID_ANIO,
                                        ID_CENTRO = Entity.ID_CENTRO,
                                        ID_ESTUDIO = Entity.ID_ESTUDIO,
                                        ID_IMPUTADO = Entity.ID_IMPUTADO,
                                        ID_INGRESO = Entity.ID_INGRESO,
                                        P1_CONDUCTA_CENTRO = Entity.PFC_IX_SEGURIDAD.P1_CONDUCTA_CENTRO,
                                        P2_CONDUCTA_AUTORIDAD = Entity.PFC_IX_SEGURIDAD.P2_CONDUCTA_AUTORIDAD,
                                        P3_CONDUCTA_GENERAL = Entity.PFC_IX_SEGURIDAD.P3_CONDUCTA_GENERAL.HasValue ? Entity.PFC_IX_SEGURIDAD.P3_CONDUCTA_GENERAL.Value != -1 ? Entity.PFC_IX_SEGURIDAD.P3_CONDUCTA_GENERAL : null : null,
                                        P4_RELACION_COMPANEROS = Entity.PFC_IX_SEGURIDAD.P4_RELACION_COMPANEROS.HasValue ? Entity.PFC_IX_SEGURIDAD.P4_RELACION_COMPANEROS.Value != -1 ? Entity.PFC_IX_SEGURIDAD.P4_RELACION_COMPANEROS : null : null,
                                        P5_CORRECTIVOS = Entity.PFC_IX_SEGURIDAD.P5_CORRECTIVOS,
                                        P6_OPINION_CONDUCTA = Entity.PFC_IX_SEGURIDAD.P6_OPINION_CONDUCTA,
                                        P7_DICTAMEN = Entity.PFC_IX_SEGURIDAD.P7_DICTAMEN,
                                        P8_MOTIVACION = Entity.PFC_IX_SEGURIDAD.P8_MOTIVACION
                                    };

                                    #region Correctivos Seguridad Comun
                                    if (CorrectivosSeguridadComun != null && CorrectivosSeguridadComun.Any())
                                        foreach (var item in CorrectivosSeguridadComun)
                                            Context.Entry(item).State = System.Data.EntityState.Deleted;

                                    var _consecutivoCorrectivos = GetIDProceso<short>("PFC_IX_CORRECTIVO", "ID_CORRECTIVO", string.Format("ID_CENTRO = {0} AND ID_ANIO = {1} AND ID_IMPUTADO = {2} AND ID_INGRESO = {3} AND ID_ESTUDIO = {4}", Entity.ID_CENTRO, Entity.ID_ANIO, Entity.ID_IMPUTADO, Entity.ID_INGRESO, Entity.ID_ESTUDIO));
                                    if (Entity.PFC_IX_SEGURIDAD.PFC_IX_CORRECTIVO != null)
                                        if (Entity.PFC_IX_SEGURIDAD.PFC_IX_CORRECTIVO.Any())
                                            foreach (var item in Entity.PFC_IX_SEGURIDAD.PFC_IX_CORRECTIVO)
                                            {
                                                var _NuevoCorrectivo = new PFC_IX_CORRECTIVO()
                                                {
                                                    CORRECTIVO_FEC = item.CORRECTIVO_FEC,
                                                    ID_ANIO = item.ID_ANIO,
                                                    ID_CENTRO = item.ID_CENTRO,
                                                    ID_ESTUDIO = item.ID_ESTUDIO,
                                                    ID_IMPUTADO = item.ID_IMPUTADO,
                                                    ID_INGRESO = item.ID_INGRESO,
                                                    MOTIVO = item.MOTIVO,
                                                    SANCION = item.SANCION,
                                                    ID_CORRECTIVO = _consecutivoCorrectivos
                                                };

                                                ValidaEstudio.PFC_IX_SEGURIDAD.PFC_IX_CORRECTIVO.Add(_NuevoCorrectivo);
                                                _consecutivoCorrectivos++;
                                            };

                                    #endregion

                                    Context.PFC_IX_SEGURIDAD.Add(ValidaEstudio.PFC_IX_SEGURIDAD);
                                }

                                #region Detalle de Seguridad Edicion Comun
                                if (_EstudioPadre != null && Entity.PFC_IX_SEGURIDAD != null)
                                {
                                    short? _Estatus = new short?();
                                    var _DetalleSegu = Context.PERSONALIDAD.FirstOrDefault(x => x.ID_ESTUDIO == _EstudioPadre.ID_ESTUDIO && x.ID_ANIO == _EstudioPadre.ID_ANIO && x.ID_CENTRO == _EstudioPadre.ID_CENTRO && x.ID_IMPUTADO == _EstudioPadre.ID_IMPUTADO && x.ID_INGRESO == _EstudioPadre.ID_INGRESO);
                                    if (string.IsNullOrEmpty(Entity.PFC_IX_SEGURIDAD.COMANDANTE) || string.IsNullOrEmpty(Entity.PFC_IX_SEGURIDAD.ELABORO) || string.IsNullOrEmpty(Entity.PFC_IX_SEGURIDAD.P1_CONDUCTA_CENTRO) || string.IsNullOrEmpty(Entity.PFC_IX_SEGURIDAD.P2_CONDUCTA_AUTORIDAD)
                                        || string.IsNullOrEmpty(Entity.PFC_IX_SEGURIDAD.P5_CORRECTIVOS) || string.IsNullOrEmpty(Entity.PFC_IX_SEGURIDAD.P6_OPINION_CONDUCTA) || string.IsNullOrEmpty(Entity.PFC_IX_SEGURIDAD.P7_DICTAMEN) || string.IsNullOrEmpty(Entity.PFC_IX_SEGURIDAD.P8_MOTIVACION)
                                        || Entity.PFC_IX_SEGURIDAD.ESTUDIO_FEC == null || Entity.PFC_IX_SEGURIDAD.P3_CONDUCTA_GENERAL == null || Entity.PFC_IX_SEGURIDAD.P4_RELACION_COMPANEROS == null)
                                        _Estatus = (short)eEstatudDetallePersonalidad.ASIGNADO;
                                    else
                                        _Estatus = (short)eEstatudDetallePersonalidad.TERMINADO;

                                    if (_DetalleSegu != null)
                                    {
                                        var _DesarrolloEstudioSegur = Context.PERSONALIDAD_DETALLE.FirstOrDefault(c => c.ID_ESTUDIO == _DetalleSegu.ID_ESTUDIO && c.ID_TIPO == (short)eTiposEstudio.SEGURIDAD && c.ID_IMPUTADO == _DetalleSegu.ID_IMPUTADO && c.ID_INGRESO == _DetalleSegu.ID_INGRESO && c.ID_CENTRO == _DetalleSegu.ID_CENTRO && c.ID_ANIO == _DetalleSegu.ID_ANIO);
                                        if (_DesarrolloEstudioSegur == null)
                                        {
                                            short _ConsecutivoPersonalidadDetalle = GetIDProceso<short>("PERSONALIDAD_DETALLE", "ID_DETALLE", "1=1");
                                            var _NvoDetalle = new PERSONALIDAD_DETALLE()
                                            {
                                                DIAS_BONIFICADOS = null,
                                                ID_ANIO = Entity.ID_ANIO,
                                                ID_CENTRO = Entity.ID_CENTRO,
                                                ID_DETALLE = _ConsecutivoPersonalidadDetalle,
                                                ID_ESTUDIO = _EstudioPadre.ID_ESTUDIO,
                                                ID_IMPUTADO = Entity.ID_IMPUTADO,
                                                ID_INGRESO = Entity.ID_INGRESO,
                                                ID_TIPO = (short)eTiposEstudio.SEGURIDAD,
                                                INICIO_FEC = GetFechaServerDate(),
                                                RESULTADO = ValidaEstudio.PFC_IX_SEGURIDAD.P7_DICTAMEN == "S" ? "S" : ValidaEstudio.PFC_IX_SEGURIDAD.P7_DICTAMEN == "D" ? "N" : string.Empty,
                                                SOLICITUD_FEC = _EstudioPadre.SOLICITUD_FEC,
                                                ID_ESTATUS = _Estatus,
                                                TERMINO_FEC = _Estatus == (short)eEstatudDetallePersonalidad.TERMINADO ? GetFechaServerDate() : new System.DateTime?(),
                                                TIPO_MEDIA = string.Empty
                                            };

                                            Context.PERSONALIDAD_DETALLE.Add(_NvoDetalle);
                                        }

                                        else
                                        {
                                            _DesarrolloEstudioSegur.RESULTADO = ValidaEstudio.PFC_IX_SEGURIDAD.P7_DICTAMEN == "S" ? "S" : ValidaEstudio.PFC_IX_SEGURIDAD.P7_DICTAMEN == "D" ? "N" : string.Empty;
                                            _DesarrolloEstudioSegur.ID_ESTATUS = _Estatus;
                                            Context.Entry(_DesarrolloEstudioSegur).State = System.Data.EntityState.Modified;
                                        }
                                    }
                                }

                                #endregion
                            }


                            Context.Entry(ValidaEstudio).State = System.Data.EntityState.Modified;
                            Context.SaveChanges();
                            transaccion.Complete();

                        #endregion
                        }
                    }

                    #endregion

                    #region Fuero Federal
                    if (EntityFederal != null)
                    {
                        var ValidaEstudio = Context.PERSONALIDAD_FUERO_FEDERAL.FirstOrDefault(c => c.ID_IMPUTADO == EntityFederal.ID_IMPUTADO && c.ID_INGRESO == EntityFederal.ID_INGRESO && c.ID_ESTUDIO == EntityFederal.ID_ESTUDIO && c.ID_ANIO == EntityFederal.ID_ANIO);
                        var _EstudioPadre = Context.PERSONALIDAD.FirstOrDefault(x => x.ID_CENTRO == EntityFederal.ID_CENTRO && x.ID_IMPUTADO == EntityFederal.ID_IMPUTADO && x.ID_INGRESO == EntityFederal.ID_INGRESO && x.ID_ESTUDIO == EntityFederal.ID_ESTUDIO && x.ID_ANIO == EntityFederal.ID_ANIO);

                        if (ValidaEstudio == null)
                        {
                            ValidaEstudio = new PERSONALIDAD_FUERO_FEDERAL()
                            {
                                ID_ANIO = EntityFederal.ID_ANIO,
                                ID_CENTRO = EntityFederal.ID_CENTRO,
                                ID_ESTUDIO = EntityFederal.ID_ESTUDIO,
                                ID_IMPUTADO = EntityFederal.ID_IMPUTADO,
                                ID_INGRESO = EntityFederal.ID_INGRESO
                            };

                            Context.PERSONALIDAD_FUERO_FEDERAL.Add(ValidaEstudio);

                            if (EntityFederal.PFF_ACTA_CONSEJO_TECNICO != null)
                            {
                                var ValidaActaFederal = Context.PFF_ACTA_CONSEJO_TECNICO.FirstOrDefault(x => x.ID_IMPUTADO == EntityFederal.ID_IMPUTADO && x.ID_INGRESO == EntityFederal.ID_INGRESO && x.ID_CENTRO == EntityFederal.ID_CENTRO && x.ID_ANIO == EntityFederal.ID_ANIO && x.ID_ESTUDIO == EntityFederal.ID_ESTUDIO);
                                var DetalleAct = Context.PFF_ACTA_DETERMINO.Where(x => x.ID_IMPUTADO == EntityFederal.ID_IMPUTADO && x.ID_INGRESO == EntityFederal.ID_INGRESO && x.ID_CENTRO == EntityFederal.ID_CENTRO && x.ID_ANIO == EntityFederal.ID_ANIO && x.ID_ESTUDIO == EntityFederal.ID_ESTUDIO);
                                if (ValidaActaFederal == null)
                                {
                                    ValidaEstudio.PFF_ACTA_CONSEJO_TECNICO = new PFF_ACTA_CONSEJO_TECNICO()
                                    {
                                        APROBADO_APLAZADO = EntityFederal.PFF_ACTA_CONSEJO_TECNICO.APROBADO_APLAZADO,
                                        APROBADO_POR = EntityFederal.PFF_ACTA_CONSEJO_TECNICO.APROBADO_POR,
                                        CEN_ID_CENTRO = EntityFederal.PFF_ACTA_CONSEJO_TECNICO.CEN_ID_CENTRO,
                                        DIRECTOR = EntityFederal.PFF_ACTA_CONSEJO_TECNICO.DIRECTOR,
                                        EXPEDIENTE = EntityFederal.PFF_ACTA_CONSEJO_TECNICO.EXPEDIENTE,
                                        FECHA = EntityFederal.PFF_ACTA_CONSEJO_TECNICO.FECHA,
                                        ID_ANIO = EntityFederal.ID_ANIO,
                                        ID_CENTRO = EntityFederal.ID_CENTRO,
                                        ID_ESTUDIO = EntityFederal.ID_ESTUDIO,
                                        ID_IMPUTADO = EntityFederal.ID_IMPUTADO,
                                        ID_INGRESO = EntityFederal.ID_INGRESO,
                                        LUGAR = EntityFederal.PFF_ACTA_CONSEJO_TECNICO.LUGAR,
                                        PRESENTE_ACTUACION = EntityFederal.PFF_ACTA_CONSEJO_TECNICO.PRESENTE_ACTUACION,
                                        SESION_FEC = EntityFederal.PFF_ACTA_CONSEJO_TECNICO.SESION_FEC,
                                        SUSCRITO_DIRECTOR_CRS = EntityFederal.PFF_ACTA_CONSEJO_TECNICO.SUSCRITO_DIRECTOR_CRS,
                                        TRAMITE = EntityFederal.PFF_ACTA_CONSEJO_TECNICO.TRAMITE
                                    };

                                    #region Detalle de Actividades
                                    if (DetalleAct != null && DetalleAct.Any())
                                        foreach (var item in DetalleAct)
                                            Context.Entry(item).State = System.Data.EntityState.Deleted;

                                    if (EntityFederal.PFF_ACTA_CONSEJO_TECNICO.PFF_ACTA_DETERMINO != null && EntityFederal.PFF_ACTA_CONSEJO_TECNICO.PFF_ACTA_DETERMINO.Any())
                                        foreach (var item in EntityFederal.PFF_ACTA_CONSEJO_TECNICO.PFF_ACTA_DETERMINO)
                                        {
                                            var _Acta = new PFF_ACTA_DETERMINO()
                                            {
                                                ID_ANIO = EntityFederal.ID_ANIO,
                                                ID_AREA_TECNICA = item.ID_AREA_TECNICA,
                                                ID_CENTRO = EntityFederal.ID_CENTRO,
                                                ID_ESTUDIO = EntityFederal.ID_ESTUDIO,
                                                ID_IMPUTADO = EntityFederal.ID_IMPUTADO,
                                                ID_INGRESO = EntityFederal.ID_INGRESO,
                                                OPINION = item.OPINION,
                                                NOMBRE = item.NOMBRE
                                            };

                                            ValidaEstudio.PFF_ACTA_CONSEJO_TECNICO.PFF_ACTA_DETERMINO.Add(_Acta);
                                        };
                                    #endregion

                                    Context.PFF_ACTA_CONSEJO_TECNICO.Add(ValidaEstudio.PFF_ACTA_CONSEJO_TECNICO);
                                }

                                else
                                {
                                    ValidaActaFederal.APROBADO_APLAZADO = EntityFederal.PFF_ACTA_CONSEJO_TECNICO.APROBADO_APLAZADO;
                                    ValidaActaFederal.APROBADO_POR = EntityFederal.PFF_ACTA_CONSEJO_TECNICO.APROBADO_POR;
                                    ValidaActaFederal.CEN_ID_CENTRO = EntityFederal.PFF_ACTA_CONSEJO_TECNICO.CEN_ID_CENTRO;
                                    ValidaActaFederal.DIRECTOR = EntityFederal.PFF_ACTA_CONSEJO_TECNICO.DIRECTOR;
                                    ValidaActaFederal.EXPEDIENTE = EntityFederal.PFF_ACTA_CONSEJO_TECNICO.EXPEDIENTE;
                                    ValidaActaFederal.FECHA = EntityFederal.PFF_ACTA_CONSEJO_TECNICO.FECHA;
                                    ValidaActaFederal.ID_ANIO = EntityFederal.ID_ANIO;
                                    ValidaActaFederal.ID_CENTRO = EntityFederal.ID_CENTRO;
                                    ValidaActaFederal.ID_ESTUDIO = EntityFederal.ID_ESTUDIO;
                                    ValidaActaFederal.ID_IMPUTADO = EntityFederal.ID_IMPUTADO;
                                    ValidaActaFederal.ID_INGRESO = EntityFederal.ID_INGRESO;
                                    ValidaActaFederal.LUGAR = EntityFederal.PFF_ACTA_CONSEJO_TECNICO.LUGAR;
                                    ValidaActaFederal.PRESENTE_ACTUACION = EntityFederal.PFF_ACTA_CONSEJO_TECNICO.PRESENTE_ACTUACION;
                                    ValidaActaFederal.SESION_FEC = EntityFederal.PFF_ACTA_CONSEJO_TECNICO.SESION_FEC;
                                    ValidaActaFederal.SUSCRITO_DIRECTOR_CRS = EntityFederal.PFF_ACTA_CONSEJO_TECNICO.SUSCRITO_DIRECTOR_CRS;
                                    ValidaActaFederal.TRAMITE = EntityFederal.PFF_ACTA_CONSEJO_TECNICO.TRAMITE;
                                    Context.Entry(ValidaActaFederal).State = System.Data.EntityState.Modified;

                                    #region Detalle Activ.
                                    if (DetalleAct != null && DetalleAct.Any())
                                        foreach (var item in DetalleAct)
                                            Context.Entry(item).State = System.Data.EntityState.Deleted;

                                    if (EntityFederal.PFF_ACTA_CONSEJO_TECNICO.PFF_ACTA_DETERMINO != null && EntityFederal.PFF_ACTA_CONSEJO_TECNICO.PFF_ACTA_DETERMINO.Any())
                                        foreach (var item in EntityFederal.PFF_ACTA_CONSEJO_TECNICO.PFF_ACTA_DETERMINO)
                                        {
                                            var _Acta = new PFF_ACTA_DETERMINO()
                                            {
                                                ID_ANIO = EntityFederal.ID_ANIO,
                                                ID_AREA_TECNICA = item.ID_AREA_TECNICA,
                                                ID_CENTRO = EntityFederal.ID_CENTRO,
                                                ID_ESTUDIO = EntityFederal.ID_ESTUDIO,
                                                ID_IMPUTADO = EntityFederal.ID_IMPUTADO,
                                                ID_INGRESO = EntityFederal.ID_INGRESO,
                                                NOMBRE = item.NOMBRE,
                                                OPINION = item.OPINION,
                                            };

                                            ValidaActaFederal.PFF_ACTA_DETERMINO.Add(item);
                                        };

                                    #endregion

                                }
                            };

                            if (EntityFederal.PFF_ESTUDIO_MEDICO != null)
                            {
                                var ValidaMedicoFederal = Context.PFF_ESTUDIO_MEDICO.FirstOrDefault(x => x.ID_IMPUTADO == EntityFederal.ID_IMPUTADO && x.ID_INGRESO == EntityFederal.ID_INGRESO && x.ID_ESTUDIO == EntityFederal.ID_ESTUDIO && x.ID_ANIO == EntityFederal.ID_ANIO);
                                var SustTox = Context.PFF_SUSTANCIA_TOXICA.Where(x => x.ID_IMPUTADO == EntityFederal.ID_IMPUTADO && x.ID_INGRESO == EntityFederal.ID_INGRESO && x.ID_ESTUDIO == EntityFederal.ID_ESTUDIO && x.ID_ANIO == EntityFederal.ID_ANIO);
                                if (ValidaMedicoFederal == null)
                                {
                                    ValidaEstudio.PFF_ESTUDIO_MEDICO = new PFF_ESTUDIO_MEDICO()
                                    {
                                        ALIAS = EntityFederal.PFF_ESTUDIO_MEDICO.ALIAS,
                                        ANTE_HEREDO_FAM = EntityFederal.PFF_ESTUDIO_MEDICO.ANTE_HEREDO_FAM,
                                        ANTE_PATOLOGICOS = EntityFederal.PFF_ESTUDIO_MEDICO.ANTE_PATOLOGICOS,
                                        ANTE_PERSONAL_NO_PATOLOGICOS = EntityFederal.PFF_ESTUDIO_MEDICO.ANTE_PERSONAL_NO_PATOLOGICOS,
                                        ASIST_AA = EntityFederal.PFF_ESTUDIO_MEDICO.ASIST_AA,
                                        ASIST_FARMACODEPENDENCIA = EntityFederal.PFF_ESTUDIO_MEDICO.ASIST_FARMACODEPENDENCIA,
                                        ASIST_OTROS = EntityFederal.PFF_ESTUDIO_MEDICO.ASIST_OTROS,
                                        ASIST_OTROS_ESPECIF = EntityFederal.PFF_ESTUDIO_MEDICO.ASIST_OTROS_ESPECIF,
                                        CONCLUSION = EntityFederal.PFF_ESTUDIO_MEDICO.CONCLUSION,
                                        DELITO = EntityFederal.PFF_ESTUDIO_MEDICO.DELITO,
                                        DIAGNOSTICO = EntityFederal.PFF_ESTUDIO_MEDICO.DIAGNOSTICO,
                                        DIRECTOR_CENTRO = EntityFederal.PFF_ESTUDIO_MEDICO.DIRECTOR_CENTRO,
                                        EDAD = EntityFederal.PFF_ESTUDIO_MEDICO.EDAD,
                                        EDO_CIVIL = EntityFederal.PFF_ESTUDIO_MEDICO.EDO_CIVIL != null ? !EntityFederal.PFF_ESTUDIO_MEDICO.EDO_CIVIL.Equals(-1) ? EntityFederal.PFF_ESTUDIO_MEDICO.EDO_CIVIL : string.Empty : string.Empty,
                                        ESTATURA = EntityFederal.PFF_ESTUDIO_MEDICO.ESTATURA,
                                        EXP_FIS_ABDOMEN = EntityFederal.PFF_ESTUDIO_MEDICO.EXP_FIS_ABDOMEN,
                                        EXP_FIS_CABEZA_CUELLO = EntityFederal.PFF_ESTUDIO_MEDICO.EXP_FIS_CABEZA_CUELLO,
                                        EXP_FIS_EXTREMIDADES = EntityFederal.PFF_ESTUDIO_MEDICO.EXP_FIS_EXTREMIDADES,
                                        EXP_FIS_GENITALES = EntityFederal.PFF_ESTUDIO_MEDICO.EXP_FIS_GENITALES,
                                        EXP_FIS_TORAX = EntityFederal.PFF_ESTUDIO_MEDICO.EXP_FIS_TORAX,
                                        FECHA = EntityFederal.PFF_ESTUDIO_MEDICO.FECHA,
                                        ID_ANIO = EntityFederal.ID_ANIO,
                                        ID_CENTRO = EntityFederal.ID_CENTRO,
                                        ID_ESTUDIO = EntityFederal.ID_ESTUDIO,
                                        ID_IMPUTADO = EntityFederal.ID_IMPUTADO,
                                        ID_INGRESO = EntityFederal.ID_INGRESO,
                                        INTERROGATORIO_APARATOS = EntityFederal.PFF_ESTUDIO_MEDICO.INTERROGATORIO_APARATOS,
                                        LUGAR = EntityFederal.PFF_ESTUDIO_MEDICO.LUGAR,
                                        MEDICO = EntityFederal.PFF_ESTUDIO_MEDICO.MEDICO,
                                        NOMBRE = EntityFederal.PFF_ESTUDIO_MEDICO.NOMBRE,
                                        OCUPACION_ACT = EntityFederal.PFF_ESTUDIO_MEDICO.OCUPACION_ACT != -1 ? EntityFederal.PFF_ESTUDIO_MEDICO.OCUPACION_ACT : new short?(),
                                        OCUPACION_ANT = EntityFederal.PFF_ESTUDIO_MEDICO.OCUPACION_ANT != -1 ? EntityFederal.PFF_ESTUDIO_MEDICO.OCUPACION_ANT : new short?(),
                                        ORIGINARIO_DE = EntityFederal.PFF_ESTUDIO_MEDICO.ORIGINARIO_DE,
                                        PADECIMIENTO_ACTUAL = EntityFederal.PFF_ESTUDIO_MEDICO.PADECIMIENTO_ACTUAL,
                                        PULSO = EntityFederal.PFF_ESTUDIO_MEDICO.PULSO,
                                        RESPIRACION = EntityFederal.PFF_ESTUDIO_MEDICO.RESPIRACION,
                                        RESULTADOS_OBTENIDOS = EntityFederal.PFF_ESTUDIO_MEDICO.RESULTADOS_OBTENIDOS,
                                        SENTENCIA = EntityFederal.PFF_ESTUDIO_MEDICO.SENTENCIA,
                                        TA = EntityFederal.PFF_ESTUDIO_MEDICO.TA,
                                        TATUAJES = EntityFederal.PFF_ESTUDIO_MEDICO.TATUAJES,
                                        TEMPERATURA = EntityFederal.PFF_ESTUDIO_MEDICO.TEMPERATURA
                                    };

                                    #region Detalle de Sustancias toxicas
                                    if (SustTox != null && SustTox.Any())
                                        foreach (var item in SustTox)
                                            Context.Entry(item).State = System.Data.EntityState.Deleted;

                                    if (EntityFederal.PFF_ESTUDIO_MEDICO.PFF_SUSTANCIA_TOXICA != null && EntityFederal.PFF_ESTUDIO_MEDICO.PFF_SUSTANCIA_TOXICA.Any())
                                        foreach (var item in EntityFederal.PFF_ESTUDIO_MEDICO.PFF_SUSTANCIA_TOXICA)
                                        {
                                            var _NuevaToxica = new PFF_SUSTANCIA_TOXICA()
                                            {
                                                CANTIDAD = item.CANTIDAD,
                                                EDAD_INICIO = item.EDAD_INICIO,
                                                ID_ANIO = EntityFederal.ID_ANIO,
                                                ID_ESTUDIO = EntityFederal.ID_ESTUDIO,
                                                ID_INGRESO = EntityFederal.ID_INGRESO,
                                                PERIODICIDAD = item.PERIODICIDAD,
                                                ID_IMPUTADO = EntityFederal.ID_IMPUTADO,
                                                ID_CENTRO = EntityFederal.ID_CENTRO,
                                                ID_DROGA = item.ID_DROGA
                                            };

                                            ValidaEstudio.PFF_ESTUDIO_MEDICO.PFF_SUSTANCIA_TOXICA.Add(_NuevaToxica);
                                        };

                                    #endregion

                                    Context.PFF_ESTUDIO_MEDICO.Add(ValidaEstudio.PFF_ESTUDIO_MEDICO);
                                }

                                else
                                {
                                    ValidaMedicoFederal.ALIAS = EntityFederal.PFF_ESTUDIO_MEDICO.ALIAS;
                                    ValidaMedicoFederal.ANTE_HEREDO_FAM = EntityFederal.PFF_ESTUDIO_MEDICO.ANTE_HEREDO_FAM;
                                    ValidaMedicoFederal.ANTE_PATOLOGICOS = EntityFederal.PFF_ESTUDIO_MEDICO.ANTE_PATOLOGICOS;
                                    ValidaMedicoFederal.ANTE_PERSONAL_NO_PATOLOGICOS = EntityFederal.PFF_ESTUDIO_MEDICO.ANTE_PERSONAL_NO_PATOLOGICOS;
                                    ValidaMedicoFederal.ASIST_AA = EntityFederal.PFF_ESTUDIO_MEDICO.ASIST_AA;
                                    ValidaMedicoFederal.ASIST_FARMACODEPENDENCIA = EntityFederal.PFF_ESTUDIO_MEDICO.ASIST_FARMACODEPENDENCIA;
                                    ValidaMedicoFederal.ASIST_OTROS = EntityFederal.PFF_ESTUDIO_MEDICO.ASIST_OTROS;
                                    ValidaMedicoFederal.ASIST_OTROS_ESPECIF = EntityFederal.PFF_ESTUDIO_MEDICO.ASIST_OTROS_ESPECIF;
                                    ValidaMedicoFederal.CONCLUSION = EntityFederal.PFF_ESTUDIO_MEDICO.CONCLUSION;
                                    ValidaMedicoFederal.DELITO = EntityFederal.PFF_ESTUDIO_MEDICO.DELITO;
                                    ValidaMedicoFederal.DIAGNOSTICO = EntityFederal.PFF_ESTUDIO_MEDICO.DIAGNOSTICO;
                                    ValidaMedicoFederal.DIRECTOR_CENTRO = EntityFederal.PFF_ESTUDIO_MEDICO.DIRECTOR_CENTRO;
                                    ValidaMedicoFederal.EDAD = EntityFederal.PFF_ESTUDIO_MEDICO.EDAD;
                                    ValidaMedicoFederal.EDO_CIVIL = EntityFederal.PFF_ESTUDIO_MEDICO.EDO_CIVIL != null ? !EntityFederal.PFF_ESTUDIO_MEDICO.EDO_CIVIL.Equals(-1) ? EntityFederal.PFF_ESTUDIO_MEDICO.EDO_CIVIL : string.Empty : string.Empty;
                                    ValidaMedicoFederal.ESTATURA = EntityFederal.PFF_ESTUDIO_MEDICO.ESTATURA;
                                    ValidaMedicoFederal.EXP_FIS_ABDOMEN = EntityFederal.PFF_ESTUDIO_MEDICO.EXP_FIS_ABDOMEN;
                                    ValidaMedicoFederal.EXP_FIS_CABEZA_CUELLO = EntityFederal.PFF_ESTUDIO_MEDICO.EXP_FIS_CABEZA_CUELLO;
                                    ValidaMedicoFederal.EXP_FIS_EXTREMIDADES = EntityFederal.PFF_ESTUDIO_MEDICO.EXP_FIS_EXTREMIDADES;
                                    ValidaMedicoFederal.EXP_FIS_GENITALES = EntityFederal.PFF_ESTUDIO_MEDICO.EXP_FIS_GENITALES;
                                    ValidaMedicoFederal.EXP_FIS_TORAX = EntityFederal.PFF_ESTUDIO_MEDICO.EXP_FIS_TORAX;
                                    ValidaMedicoFederal.FECHA = EntityFederal.PFF_ESTUDIO_MEDICO.FECHA;
                                    ValidaMedicoFederal.ID_ANIO = EntityFederal.ID_ANIO;
                                    ValidaMedicoFederal.ID_CENTRO = EntityFederal.ID_CENTRO;
                                    ValidaMedicoFederal.ID_ESTUDIO = EntityFederal.ID_ESTUDIO;
                                    ValidaMedicoFederal.ID_IMPUTADO = EntityFederal.ID_IMPUTADO;
                                    ValidaMedicoFederal.ID_INGRESO = EntityFederal.ID_INGRESO;
                                    ValidaMedicoFederal.INTERROGATORIO_APARATOS = EntityFederal.PFF_ESTUDIO_MEDICO.INTERROGATORIO_APARATOS;
                                    ValidaMedicoFederal.LUGAR = EntityFederal.PFF_ESTUDIO_MEDICO.LUGAR;
                                    ValidaMedicoFederal.MEDICO = EntityFederal.PFF_ESTUDIO_MEDICO.MEDICO;
                                    ValidaMedicoFederal.NOMBRE = EntityFederal.PFF_ESTUDIO_MEDICO.NOMBRE;
                                    ValidaMedicoFederal.OCUPACION_ACT = EntityFederal.PFF_ESTUDIO_MEDICO.OCUPACION_ACT != -1 ? EntityFederal.PFF_ESTUDIO_MEDICO.OCUPACION_ACT : new short?();
                                    ValidaMedicoFederal.OCUPACION_ANT = EntityFederal.PFF_ESTUDIO_MEDICO.OCUPACION_ANT != -1 ? EntityFederal.PFF_ESTUDIO_MEDICO.OCUPACION_ANT : new short?();
                                    ValidaMedicoFederal.ORIGINARIO_DE = EntityFederal.PFF_ESTUDIO_MEDICO.ORIGINARIO_DE;
                                    ValidaMedicoFederal.PADECIMIENTO_ACTUAL = EntityFederal.PFF_ESTUDIO_MEDICO.PADECIMIENTO_ACTUAL;
                                    ValidaMedicoFederal.PULSO = EntityFederal.PFF_ESTUDIO_MEDICO.PULSO;
                                    ValidaMedicoFederal.RESPIRACION = EntityFederal.PFF_ESTUDIO_MEDICO.RESPIRACION;
                                    ValidaMedicoFederal.RESULTADOS_OBTENIDOS = EntityFederal.PFF_ESTUDIO_MEDICO.RESULTADOS_OBTENIDOS;
                                    ValidaMedicoFederal.SENTENCIA = EntityFederal.PFF_ESTUDIO_MEDICO.SENTENCIA;
                                    ValidaMedicoFederal.TA = EntityFederal.PFF_ESTUDIO_MEDICO.TA;
                                    ValidaMedicoFederal.TATUAJES = EntityFederal.PFF_ESTUDIO_MEDICO.TATUAJES;
                                    ValidaMedicoFederal.TEMPERATURA = EntityFederal.PFF_ESTUDIO_MEDICO.TEMPERATURA;
                                    Context.Entry(ValidaMedicoFederal).State = System.Data.EntityState.Modified;

                                    #region Detalle Sust. Tox
                                    if (SustTox != null && SustTox.Any())
                                        foreach (var item in SustTox)
                                            Context.Entry(item).State = System.Data.EntityState.Deleted;

                                    if (EntityFederal.PFF_ESTUDIO_MEDICO.PFF_SUSTANCIA_TOXICA != null && EntityFederal.PFF_ESTUDIO_MEDICO.PFF_SUSTANCIA_TOXICA.Any())
                                        foreach (var item in EntityFederal.PFF_ESTUDIO_MEDICO.PFF_SUSTANCIA_TOXICA)
                                        {
                                            var _NuevaToxica = new PFF_SUSTANCIA_TOXICA()
                                            {
                                                CANTIDAD = item.CANTIDAD,
                                                EDAD_INICIO = item.EDAD_INICIO,
                                                ID_ANIO = EntityFederal.ID_ANIO,
                                                ID_CENTRO = EntityFederal.ID_CENTRO,
                                                ID_DROGA = item.ID_DROGA,
                                                ID_ESTUDIO = EntityFederal.ID_ESTUDIO,
                                                ID_IMPUTADO = EntityFederal.ID_IMPUTADO,
                                                ID_INGRESO = EntityFederal.ID_INGRESO,
                                                PERIODICIDAD = item.PERIODICIDAD
                                            };

                                            ValidaMedicoFederal.PFF_SUSTANCIA_TOXICA.Add(_NuevaToxica);
                                        };

                                    #endregion
                                }

                                #region Definicion de detalle medico de fuero federal

                                if (_EstudioPadre != null)
                                {
                                    short? _Estatus = new short?();
                                    if (string.IsNullOrEmpty(EntityFederal.PFF_ESTUDIO_MEDICO.ALIAS) || string.IsNullOrEmpty(EntityFederal.PFF_ESTUDIO_MEDICO.ANTE_HEREDO_FAM) || string.IsNullOrEmpty(EntityFederal.PFF_ESTUDIO_MEDICO.ANTE_PATOLOGICOS) ||
                                        string.IsNullOrEmpty(EntityFederal.PFF_ESTUDIO_MEDICO.ANTE_PERSONAL_NO_PATOLOGICOS) || string.IsNullOrEmpty(EntityFederal.PFF_ESTUDIO_MEDICO.CONCLUSION) || string.IsNullOrEmpty(EntityFederal.PFF_ESTUDIO_MEDICO.DELITO) ||
                                        string.IsNullOrEmpty(EntityFederal.PFF_ESTUDIO_MEDICO.DIAGNOSTICO) || string.IsNullOrEmpty(EntityFederal.PFF_ESTUDIO_MEDICO.EDO_CIVIL) || string.IsNullOrEmpty(EntityFederal.PFF_ESTUDIO_MEDICO.ESTATURA) ||
                                        string.IsNullOrEmpty(EntityFederal.PFF_ESTUDIO_MEDICO.EXP_FIS_ABDOMEN) || string.IsNullOrEmpty(EntityFederal.PFF_ESTUDIO_MEDICO.EXP_FIS_CABEZA_CUELLO) || string.IsNullOrEmpty(EntityFederal.PFF_ESTUDIO_MEDICO.EXP_FIS_EXTREMIDADES) ||
                                        string.IsNullOrEmpty(EntityFederal.PFF_ESTUDIO_MEDICO.EXP_FIS_GENITALES) || string.IsNullOrEmpty(EntityFederal.PFF_ESTUDIO_MEDICO.EXP_FIS_TORAX) || string.IsNullOrEmpty(EntityFederal.PFF_ESTUDIO_MEDICO.INTERROGATORIO_APARATOS) ||
                                        string.IsNullOrEmpty(EntityFederal.PFF_ESTUDIO_MEDICO.NOMBRE) || EntityFederal.PFF_ESTUDIO_MEDICO.OCUPACION_ACT == null || EntityFederal.PFF_ESTUDIO_MEDICO.OCUPACION_ANT == null ||
                                         string.IsNullOrEmpty(EntityFederal.PFF_ESTUDIO_MEDICO.ORIGINARIO_DE) || string.IsNullOrEmpty(EntityFederal.PFF_ESTUDIO_MEDICO.PADECIMIENTO_ACTUAL) || string.IsNullOrEmpty(EntityFederal.PFF_ESTUDIO_MEDICO.PULSO) ||
                                        string.IsNullOrEmpty(EntityFederal.PFF_ESTUDIO_MEDICO.RESPIRACION) || string.IsNullOrEmpty(EntityFederal.PFF_ESTUDIO_MEDICO.RESULTADOS_OBTENIDOS) || string.IsNullOrEmpty(EntityFederal.PFF_ESTUDIO_MEDICO.SENTENCIA) || string.IsNullOrEmpty(EntityFederal.PFF_ESTUDIO_MEDICO.TA) ||
                                         string.IsNullOrEmpty(EntityFederal.PFF_ESTUDIO_MEDICO.TATUAJES) || string.IsNullOrEmpty(EntityFederal.PFF_ESTUDIO_MEDICO.TEMPERATURA) || !EntityFederal.PFF_ESTUDIO_MEDICO.EDAD.HasValue || !EntityFederal.PFF_ESTUDIO_MEDICO.FECHA.HasValue)
                                        _Estatus = (short)eEstatudDetallePersonalidad.ASIGNADO;

                                    else
                                        _Estatus = (short)eEstatudDetallePersonalidad.TERMINADO;


                                    var DesarrolloMedicoFederal = Context.PERSONALIDAD_DETALLE.FirstOrDefault(x => x.ID_ESTUDIO == _EstudioPadre.ID_ESTUDIO && x.ID_TIPO == (short)eTiposEstudio.MEDICA_FEDERAL && x.ID_INGRESO == EntityFederal.ID_INGRESO && x.ID_IMPUTADO == EntityFederal.ID_IMPUTADO && x.ID_ANIO == EntityFederal.ID_ANIO && x.ID_CENTRO == _EstudioPadre.ID_CENTRO);
                                    if (DesarrolloMedicoFederal == null)
                                    {
                                        short _ConsecutivoPersonalidadDetalle = GetIDProceso<short>("PERSONALIDAD_DETALLE", "ID_DETALLE", "1=1");
                                        var NvoDetalle = new PERSONALIDAD_DETALLE()
                                        {
                                            ID_ANIO = EntityFederal.ID_ANIO,
                                            ID_CENTRO = EntityFederal.ID_CENTRO,
                                            ID_DETALLE = _ConsecutivoPersonalidadDetalle,
                                            ID_ESTUDIO = _EstudioPadre.ID_ESTUDIO,
                                            ID_IMPUTADO = EntityFederal.ID_IMPUTADO,
                                            ID_ESTATUS = _Estatus,
                                            ID_INGRESO = EntityFederal.ID_INGRESO,
                                            ID_TIPO = (short)eTiposEstudio.MEDICA_FEDERAL,
                                            INICIO_FEC = GetFechaServerDate(),
                                            RESULTADO = _Estatus == (short)eEstatudDetallePersonalidad.TERMINADO ? "S" : "N",
                                            SOLICITUD_FEC = null,
                                            TERMINO_FEC = _Estatus == (short)eEstatudDetallePersonalidad.TERMINADO ? GetFechaServerDate() : new System.DateTime?(),
                                            TIPO_MEDIA = string.Empty
                                        };

                                        Context.PERSONALIDAD_DETALLE.Add(NvoDetalle);
                                    }

                                    else
                                    {
                                        DesarrolloMedicoFederal.RESULTADO = _Estatus.HasValue ? _Estatus.Value == (short)eEstatudDetallePersonalidad.TERMINADO ? "S" : "N" : "N";
                                        DesarrolloMedicoFederal.ID_ESTATUS = _Estatus;
                                        Context.Entry(DesarrolloMedicoFederal).State = System.Data.EntityState.Modified;
                                    }
                                };
                                #endregion
                            }

                            if (EntityFederal.PFF_ESTUDIO_PSICOLOGICO != null)
                            {
                                var _EstudioPsico = Context.PFF_ESTUDIO_PSICOLOGICO.FirstOrDefault(c => c.ID_IMPUTADO == EntityFederal.ID_IMPUTADO && c.ID_INGRESO == EntityFederal.ID_INGRESO && c.ID_ESTUDIO == EntityFederal.ID_ESTUDIO && c.ID_ANIO == EntityFederal.ID_ANIO && c.ID_CENTRO == EntityFederal.ID_CENTRO);
                                if (_EstudioPsico == null)
                                {
                                    ValidaEstudio.PFF_ESTUDIO_PSICOLOGICO = new PFF_ESTUDIO_PSICOLOGICO()
                                    {
                                        ACTITUD = EntityFederal.PFF_ESTUDIO_PSICOLOGICO.ACTITUD,
                                        CI = EntityFederal.PFF_ESTUDIO_PSICOLOGICO.CI,
                                        DELITO = EntityFederal.PFF_ESTUDIO_PSICOLOGICO.DELITO,
                                        DINAM_PERSON_ACTUAL = EntityFederal.PFF_ESTUDIO_PSICOLOGICO.DINAM_PERSON_ACTUAL,
                                        DINAM_PERSON_INGRESO = EntityFederal.PFF_ESTUDIO_PSICOLOGICO.DINAM_PERSON_INGRESO,
                                        EDAD = EntityFederal.PFF_ESTUDIO_PSICOLOGICO.EDAD,
                                        DIRECTOR_DENTRO = EntityFederal.PFF_ESTUDIO_PSICOLOGICO.DIRECTOR_DENTRO,
                                        ESPECIFIQUE = EntityFederal.PFF_ESTUDIO_PSICOLOGICO.ESPECIFIQUE,
                                        EXAMEN_MENTAL = EntityFederal.PFF_ESTUDIO_PSICOLOGICO.EXAMEN_MENTAL,
                                        EXTERNO = EntityFederal.PFF_ESTUDIO_PSICOLOGICO.EXTERNO,
                                        FECHA = EntityFederal.PFF_ESTUDIO_PSICOLOGICO.FECHA,
                                        ID_ANIO = EntityFederal.ID_ANIO,
                                        ID_CENTRO = EntityFederal.ID_CENTRO,
                                        ID_ESTUDIO = EntityFederal.ID_ESTUDIO,
                                        ID_IMPUTADO = EntityFederal.ID_IMPUTADO,
                                        ID_INGRESO = EntityFederal.ID_INGRESO,
                                        INDICE_LESION_ORGANICA = EntityFederal.PFF_ESTUDIO_PSICOLOGICO.INDICE_LESION_ORGANICA,
                                        INTERNO = EntityFederal.PFF_ESTUDIO_PSICOLOGICO.INTERNO,
                                        LUGAR = EntityFederal.PFF_ESTUDIO_PSICOLOGICO.LUGAR,
                                        NIVEL_INTELECTUAL = EntityFederal.PFF_ESTUDIO_PSICOLOGICO.NIVEL_INTELECTUAL,
                                        NOMBRE = EntityFederal.PFF_ESTUDIO_PSICOLOGICO.NOMBRE,
                                        OPINION = EntityFederal.PFF_ESTUDIO_PSICOLOGICO.OPINION,
                                        PRONOSTICO_REINTEGRACION = EntityFederal.PFF_ESTUDIO_PSICOLOGICO.PRONOSTICO_REINTEGRACION,
                                        PRUEBAS_APLICADAS = EntityFederal.PFF_ESTUDIO_PSICOLOGICO.PRUEBAS_APLICADAS,
                                        PSICOLOGO = EntityFederal.PFF_ESTUDIO_PSICOLOGICO.PSICOLOGO,
                                        REQ_CONT_TRATAMIENTO = EntityFederal.PFF_ESTUDIO_PSICOLOGICO.REQ_CONT_TRATAMIENTO,
                                        RESULT_TRATAMIENTO = EntityFederal.PFF_ESTUDIO_PSICOLOGICO.RESULT_TRATAMIENTO,
                                        SOBRENOMBRE = EntityFederal.PFF_ESTUDIO_PSICOLOGICO.SOBRENOMBRE
                                    };

                                    Context.PFF_ESTUDIO_PSICOLOGICO.Add(ValidaEstudio.PFF_ESTUDIO_PSICOLOGICO);
                                }

                                else
                                {
                                    _EstudioPsico.ACTITUD = EntityFederal.PFF_ESTUDIO_PSICOLOGICO.ACTITUD;
                                    _EstudioPsico.CI = EntityFederal.PFF_ESTUDIO_PSICOLOGICO.CI;
                                    _EstudioPsico.DELITO = EntityFederal.PFF_ESTUDIO_PSICOLOGICO.DELITO;
                                    _EstudioPsico.DINAM_PERSON_ACTUAL = EntityFederal.PFF_ESTUDIO_PSICOLOGICO.DINAM_PERSON_ACTUAL;
                                    _EstudioPsico.DINAM_PERSON_INGRESO = EntityFederal.PFF_ESTUDIO_PSICOLOGICO.DINAM_PERSON_INGRESO;
                                    _EstudioPsico.DIRECTOR_DENTRO = EntityFederal.PFF_ESTUDIO_PSICOLOGICO.DIRECTOR_DENTRO;
                                    _EstudioPsico.EDAD = EntityFederal.PFF_ESTUDIO_PSICOLOGICO.EDAD;
                                    _EstudioPsico.ESPECIFIQUE = EntityFederal.PFF_ESTUDIO_PSICOLOGICO.ESPECIFIQUE;
                                    _EstudioPsico.EXAMEN_MENTAL = EntityFederal.PFF_ESTUDIO_PSICOLOGICO.EXAMEN_MENTAL;
                                    _EstudioPsico.EXTERNO = EntityFederal.PFF_ESTUDIO_PSICOLOGICO.EXTERNO;
                                    _EstudioPsico.FECHA = EntityFederal.PFF_ESTUDIO_PSICOLOGICO.FECHA;
                                    _EstudioPsico.ID_ANIO = EntityFederal.ID_ANIO;
                                    _EstudioPsico.ID_CENTRO = EntityFederal.ID_CENTRO;
                                    _EstudioPsico.ID_ESTUDIO = EntityFederal.ID_ESTUDIO;
                                    _EstudioPsico.ID_IMPUTADO = EntityFederal.ID_IMPUTADO;
                                    _EstudioPsico.ID_INGRESO = _EstudioPsico.ID_INGRESO;
                                    _EstudioPsico.INDICE_LESION_ORGANICA = EntityFederal.PFF_ESTUDIO_PSICOLOGICO.INDICE_LESION_ORGANICA;
                                    _EstudioPsico.INTERNO = EntityFederal.PFF_ESTUDIO_PSICOLOGICO.INTERNO;
                                    _EstudioPsico.LUGAR = EntityFederal.PFF_ESTUDIO_PSICOLOGICO.LUGAR;
                                    _EstudioPsico.NIVEL_INTELECTUAL = EntityFederal.PFF_ESTUDIO_PSICOLOGICO.NIVEL_INTELECTUAL;
                                    _EstudioPsico.NOMBRE = EntityFederal.PFF_ESTUDIO_PSICOLOGICO.NOMBRE;
                                    _EstudioPsico.OPINION = EntityFederal.PFF_ESTUDIO_PSICOLOGICO.OPINION;
                                    _EstudioPsico.PRONOSTICO_REINTEGRACION = EntityFederal.PFF_ESTUDIO_PSICOLOGICO.PRONOSTICO_REINTEGRACION;
                                    _EstudioPsico.PRUEBAS_APLICADAS = EntityFederal.PFF_ESTUDIO_PSICOLOGICO.PRUEBAS_APLICADAS;
                                    _EstudioPsico.PSICOLOGO = EntityFederal.PFF_ESTUDIO_PSICOLOGICO.PSICOLOGO;
                                    _EstudioPsico.REQ_CONT_TRATAMIENTO = EntityFederal.PFF_ESTUDIO_PSICOLOGICO.REQ_CONT_TRATAMIENTO;
                                    _EstudioPsico.RESULT_TRATAMIENTO = EntityFederal.PFF_ESTUDIO_PSICOLOGICO.RESULT_TRATAMIENTO;
                                    _EstudioPsico.SOBRENOMBRE = EntityFederal.PFF_ESTUDIO_PSICOLOGICO.SOBRENOMBRE;
                                    Context.Entry(_EstudioPsico).State = System.Data.EntityState.Modified;
                                }

                                if (_EstudioPadre != null)
                                {
                                    short? _Estatus = new short?();
                                    if (string.IsNullOrEmpty(EntityFederal.PFF_ESTUDIO_PSICOLOGICO.ACTITUD) || string.IsNullOrEmpty(EntityFederal.PFF_ESTUDIO_PSICOLOGICO.CI) || string.IsNullOrEmpty(EntityFederal.PFF_ESTUDIO_PSICOLOGICO.DELITO) || string.IsNullOrEmpty(EntityFederal.PFF_ESTUDIO_PSICOLOGICO.DINAM_PERSON_ACTUAL)
                                        || string.IsNullOrEmpty(EntityFederal.PFF_ESTUDIO_PSICOLOGICO.DINAM_PERSON_INGRESO) || string.IsNullOrEmpty(EntityFederal.PFF_ESTUDIO_PSICOLOGICO.EXAMEN_MENTAL) || string.IsNullOrEmpty(EntityFederal.PFF_ESTUDIO_PSICOLOGICO.EXTERNO) || string.IsNullOrEmpty(EntityFederal.PFF_ESTUDIO_PSICOLOGICO.INDICE_LESION_ORGANICA)
                                        || string.IsNullOrEmpty(EntityFederal.PFF_ESTUDIO_PSICOLOGICO.INTERNO) || string.IsNullOrEmpty(EntityFederal.PFF_ESTUDIO_PSICOLOGICO.NIVEL_INTELECTUAL) || string.IsNullOrEmpty(EntityFederal.PFF_ESTUDIO_PSICOLOGICO.NOMBRE) || string.IsNullOrEmpty(EntityFederal.PFF_ESTUDIO_PSICOLOGICO.OPINION) || string.IsNullOrEmpty(EntityFederal.PFF_ESTUDIO_PSICOLOGICO.PRONOSTICO_REINTEGRACION)
                                        || string.IsNullOrEmpty(EntityFederal.PFF_ESTUDIO_PSICOLOGICO.PRUEBAS_APLICADAS) || string.IsNullOrEmpty(EntityFederal.PFF_ESTUDIO_PSICOLOGICO.REQ_CONT_TRATAMIENTO) || string.IsNullOrEmpty(EntityFederal.PFF_ESTUDIO_PSICOLOGICO.RESULT_TRATAMIENTO) || string.IsNullOrEmpty(EntityFederal.PFF_ESTUDIO_PSICOLOGICO.SOBRENOMBRE) || !EntityFederal.PFF_ESTUDIO_PSICOLOGICO.EDAD.HasValue ||
                                        !EntityFederal.PFF_ESTUDIO_PSICOLOGICO.FECHA.HasValue)
                                        _Estatus = (short)eEstatudDetallePersonalidad.ASIGNADO;
                                    else
                                        _Estatus = (short)eEstatudDetallePersonalidad.TERMINADO;

                                    var DesarrolloPsicoFederal = Context.PERSONALIDAD_DETALLE.FirstOrDefault(x => x.ID_IMPUTADO == EntityFederal.ID_IMPUTADO && x.ID_ESTUDIO == _EstudioPadre.ID_ESTUDIO && x.ID_INGRESO == EntityFederal.ID_INGRESO && x.ID_TIPO == (short)eTiposEstudio.PSICOLOGICA_FEDERAL && x.ID_CENTRO == EntityFederal.ID_CENTRO && x.ID_ANIO == EntityFederal.ID_ANIO);
                                    if (DesarrolloPsicoFederal == null)
                                    {
                                        short _ConsecutivoPersonalidadDetalle = GetIDProceso<short>("PERSONALIDAD_DETALLE", "ID_DETALLE", "1=1");
                                        var NvoDetalle = new PERSONALIDAD_DETALLE()
                                        {
                                            ID_ANIO = EntityFederal.ID_ANIO,
                                            ID_CENTRO = EntityFederal.ID_CENTRO,
                                            ID_DETALLE = _ConsecutivoPersonalidadDetalle,
                                            ID_ESTUDIO = _EstudioPadre.ID_ESTUDIO,
                                            ID_IMPUTADO = EntityFederal.ID_IMPUTADO,
                                            ID_INGRESO = EntityFederal.ID_INGRESO,
                                            ID_TIPO = (short)eTiposEstudio.PSICOLOGICA_FEDERAL,
                                            INICIO_FEC = GetFechaServerDate(),
                                            RESULTADO = _Estatus.HasValue ? _Estatus.Value == (short)eEstatudDetallePersonalidad.TERMINADO ? "S" : "N" : "N",
                                            SOLICITUD_FEC = null,
                                            ID_ESTATUS = _Estatus,
                                            TERMINO_FEC = _Estatus.HasValue ? _Estatus.Value == (short)eEstatudDetallePersonalidad.TERMINADO ? GetFechaServerDate() : new System.DateTime?() : new System.DateTime?(),
                                            TIPO_MEDIA = string.Empty
                                        };

                                        Context.PERSONALIDAD_DETALLE.Add(NvoDetalle);
                                    }

                                    else
                                    {
                                        DesarrolloPsicoFederal.RESULTADO = _Estatus.HasValue ? _Estatus.Value == (short)eEstatudDetallePersonalidad.TERMINADO ? "S" : "N" : "N";
                                        DesarrolloPsicoFederal.ID_ESTATUS = _Estatus;
                                        Context.Entry(DesarrolloPsicoFederal).State = System.Data.EntityState.Modified;
                                    };
                                };
                            }

                            if (EntityFederal.PFF_TRABAJO_SOCIAL != null)
                            {
                                var _EstudioTrabajoSocHecho = Context.PFF_TRABAJO_SOCIAL.FirstOrDefault(c => c.ID_INGRESO == EntityFederal.ID_INGRESO && c.ID_IMPUTADO == EntityFederal.ID_IMPUTADO && c.ID_ESTUDIO == EntityFederal.ID_ESTUDIO && c.ID_ANIO == EntityFederal.ID_ANIO && c.ID_CENTRO == EntityFederal.ID_CENTRO);
                                var GruposFam = Context.PFF_GRUPO_FAMILIAR.Where(x => x.ID_INGRESO == EntityFederal.ID_INGRESO && x.ID_IMPUTADO == EntityFederal.ID_IMPUTADO && x.ID_ESTUDIO == EntityFederal.ID_ESTUDIO && x.ID_ANIO == EntityFederal.ID_ANIO && x.ID_CENTRO == EntityFederal.ID_CENTRO);
                                if (_EstudioTrabajoSocHecho == null)
                                {
                                    ValidaEstudio.PFF_TRABAJO_SOCIAL = new PFF_TRABAJO_SOCIAL()
                                    {
                                        ALIMENTACION_FAM = EntityFederal.PFF_TRABAJO_SOCIAL.ALIMENTACION_FAM,
                                        APORTACIONES_FAM = EntityFederal.PFF_TRABAJO_SOCIAL.APORTACIONES_FAM,
                                        APOYO_FAM_OTROS = EntityFederal.PFF_TRABAJO_SOCIAL.APOYO_FAM_OTROS,
                                        CARACT_FP_ANTECE_PENALES_ADIC = EntityFederal.PFF_TRABAJO_SOCIAL.CARACT_FP_ANTECE_PENALES_ADIC,
                                        CARACT_FP_ANTECEDENTES_PENALES = EntityFederal.PFF_TRABAJO_SOCIAL.CARACT_FP_ANTECEDENTES_PENALES,
                                        CARACT_FP_CONCEPTO = EntityFederal.PFF_TRABAJO_SOCIAL.CARACT_FP_CONCEPTO,
                                        CARACT_FP_GRUPO = EntityFederal.PFF_TRABAJO_SOCIAL.CARACT_FP_GRUPO,
                                        CARACT_FP_NIVEL_SOCIO_CULTURAL = EntityFederal.PFF_TRABAJO_SOCIAL.CARACT_FP_NIVEL_SOCIO_CULTURAL,
                                        CARACT_FP_RELAC_INTERFAM = EntityFederal.PFF_TRABAJO_SOCIAL.CARACT_FP_RELAC_INTERFAM,
                                        CARACT_FP_VIOLENCIA_FAM = EntityFederal.PFF_TRABAJO_SOCIAL.CARACT_FP_VIOLENCIA_FAM,
                                        CARACT_FP_VIOLENCIA_FAM_ESPEFI = EntityFederal.PFF_TRABAJO_SOCIAL.CARACT_FP_VIOLENCIA_FAM_ESPEFI,
                                        CARACT_FS_GRUPO = EntityFederal.PFF_TRABAJO_SOCIAL.CARACT_FS_GRUPO,
                                        CARACT_FS_HIJOS_ANT = EntityFederal.PFF_TRABAJO_SOCIAL.CARACT_FS_HIJOS_ANT,
                                        CARACT_FS_NIVEL_SOCIO_CULTURAL = EntityFederal.PFF_TRABAJO_SOCIAL.CARACT_FS_NIVEL_SOCIO_CULTURAL,
                                        CARACT_FS_PROBLEMAS_CONDUCTA = EntityFederal.PFF_TRABAJO_SOCIAL.CARACT_FS_PROBLEMAS_CONDUCTA,
                                        CARACT_FS_PROBLEMAS_CONDUCTA_E = EntityFederal.PFF_TRABAJO_SOCIAL.CARACT_FS_PROBLEMAS_CONDUCTA_E,
                                        CARACT_FS_RELACION_MEDIO_EXT = EntityFederal.PFF_TRABAJO_SOCIAL.CARACT_FS_RELACION_MEDIO_EXT,
                                        CARACT_FS_RELACIONES_INTERFAM = EntityFederal.PFF_TRABAJO_SOCIAL.CARACT_FS_RELACIONES_INTERFAM,
                                        CARACT_FS_VIOLENCIA_INTRAFAM = EntityFederal.PFF_TRABAJO_SOCIAL.CARACT_FS_VIOLENCIA_INTRAFAM,
                                        CARACT_FS_VIOLENCIA_INTRAFAM_E = EntityFederal.PFF_TRABAJO_SOCIAL.CARACT_FS_VIOLENCIA_INTRAFAM_E,
                                        CARACT_FS_VIVIEN_DESCRIPCION = EntityFederal.PFF_TRABAJO_SOCIAL.CARACT_FS_VIVIEN_DESCRIPCION,
                                        CARACT_FS_VIVIEN_MOBILIARIO = EntityFederal.PFF_TRABAJO_SOCIAL.CARACT_FS_VIVIEN_MOBILIARIO,
                                        CARACT_FS_VIVIEN_NUM_HABITACIO = EntityFederal.PFF_TRABAJO_SOCIAL.CARACT_FS_VIVIEN_NUM_HABITACIO,
                                        CARACT_FS_VIVIEN_TRANSPORTE = EntityFederal.PFF_TRABAJO_SOCIAL.CARACT_FS_VIVIEN_TRANSPORTE,
                                        CARACT_FS_ZONA = EntityFederal.PFF_TRABAJO_SOCIAL.CARACT_FS_ZONA,
                                        DIAG_SOCIAL_PRONOS = EntityFederal.PFF_TRABAJO_SOCIAL.DIAG_SOCIAL_PRONOS,
                                        DIALECTO = EntityFederal.PFF_TRABAJO_SOCIAL.DIALECTO.HasValue ? EntityFederal.PFF_TRABAJO_SOCIAL.DIALECTO.Value != -1 ? EntityFederal.PFF_TRABAJO_SOCIAL.DIALECTO : new short?() : new short?(),
                                        DIRECTOR_CENTRO = EntityFederal.PFF_TRABAJO_SOCIAL.DIRECTOR_CENTRO,
                                        DISTRIBUCION_GASTO_FAM = EntityFederal.PFF_TRABAJO_SOCIAL.DISTRIBUCION_GASTO_FAM,
                                        DOMICILIO = EntityFederal.PFF_TRABAJO_SOCIAL.DOMICILIO,
                                        ECO_FP_COOPERA_ACTUALMENTE = EntityFederal.PFF_TRABAJO_SOCIAL.ECO_FP_COOPERA_ACTUALMENTE,
                                        ECO_FP_FONDOS_AHORRO = EntityFederal.PFF_TRABAJO_SOCIAL.ECO_FP_FONDOS_AHORRO,
                                        ECO_FP_RESPONSABLE = EntityFederal.PFF_TRABAJO_SOCIAL.ECO_FP_RESPONSABLE,
                                        ECO_FP_TOTAL_EGRESOS_MEN = EntityFederal.PFF_TRABAJO_SOCIAL.ECO_FP_TOTAL_EGRESOS_MEN,
                                        ECO_FP_TOTAL_INGRESOS_MEN = EntityFederal.PFF_TRABAJO_SOCIAL.ECO_FP_TOTAL_INGRESOS_MEN,
                                        ECO_FP_ZONA = EntityFederal.PFF_TRABAJO_SOCIAL.ECO_FP_ZONA,
                                        EDO_CIVIL = EntityFederal.PFF_TRABAJO_SOCIAL.EDO_CIVIL.HasValue ? EntityFederal.PFF_TRABAJO_SOCIAL.EDO_CIVIL.Value != -1 ? EntityFederal.PFF_TRABAJO_SOCIAL.EDO_CIVIL : new short?() : new short?(),
                                        ESCOLARIDAD_ACTUAL = EntityFederal.PFF_TRABAJO_SOCIAL.ESCOLARIDAD_ACTUAL.HasValue ? EntityFederal.PFF_TRABAJO_SOCIAL.ESCOLARIDAD_ACTUAL.Value != -1 ? EntityFederal.PFF_TRABAJO_SOCIAL.ESCOLARIDAD_ACTUAL : new short?() : new short?(),
                                        ESCOLARIDAD_CENTRO = EntityFederal.PFF_TRABAJO_SOCIAL.ESCOLARIDAD_CENTRO.HasValue ? EntityFederal.PFF_TRABAJO_SOCIAL.ESCOLARIDAD_CENTRO.Value != -1 ? EntityFederal.PFF_TRABAJO_SOCIAL.ESCOLARIDAD_CENTRO : new short?() : new short?(),
                                        AVAL_MORAL = EntityFederal.PFF_TRABAJO_SOCIAL.AVAL_MORAL,
                                        AVAL_MORAL_PARENTESCO = EntityFederal.PFF_TRABAJO_SOCIAL.AVAL_MORAL_PARENTESCO,
                                        EXTERNADO_CALLE = EntityFederal.PFF_TRABAJO_SOCIAL.EXTERNADO_CALLE,
                                        EXTERNADO_CIUDAD = EntityFederal.PFF_TRABAJO_SOCIAL.EXTERNADO_CIUDAD.HasValue ? EntityFederal.PFF_TRABAJO_SOCIAL.EXTERNADO_CIUDAD.Value != -1 ? EntityFederal.PFF_TRABAJO_SOCIAL.EXTERNADO_CIUDAD.Value != decimal.Zero ? EntityFederal.PFF_TRABAJO_SOCIAL.EXTERNADO_CIUDAD : new short?() : new short?() : new short?(),
                                        EXTERNADO_COLONIA = EntityFederal.PFF_TRABAJO_SOCIAL.EXTERNADO_COLONIA.HasValue ? EntityFederal.PFF_TRABAJO_SOCIAL.EXTERNADO_COLONIA.Value != -1 ? EntityFederal.PFF_TRABAJO_SOCIAL.EXTERNADO_COLONIA.Value != decimal.Zero ? EntityFederal.PFF_TRABAJO_SOCIAL.EXTERNADO_COLONIA : new int?() : new int?() : new int?(),
                                        EXTERNADO_CP = EntityFederal.PFF_TRABAJO_SOCIAL.EXTERNADO_CP,
                                        EXTERNADO_ENTIDAD = EntityFederal.PFF_TRABAJO_SOCIAL.EXTERNADO_ENTIDAD.HasValue ? EntityFederal.PFF_TRABAJO_SOCIAL.EXTERNADO_ENTIDAD.Value != -1 ? EntityFederal.PFF_TRABAJO_SOCIAL.EXTERNADO_ENTIDAD.Value != decimal.Zero ? EntityFederal.PFF_TRABAJO_SOCIAL.EXTERNADO_ENTIDAD : new short?() : new short?() : new short?(),
                                        EXTERNADO_MUNICIPIO = EntityFederal.PFF_TRABAJO_SOCIAL.EXTERNADO_MUNICIPIO.HasValue ? EntityFederal.PFF_TRABAJO_SOCIAL.EXTERNADO_MUNICIPIO.Value != -1 ? EntityFederal.PFF_TRABAJO_SOCIAL.EXTERNADO_MUNICIPIO.Value != decimal.Zero ? EntityFederal.PFF_TRABAJO_SOCIAL.EXTERNADO_MUNICIPIO : new short?() : new short?() : new short?(),
                                        EXTERNADO_NUMERO = EntityFederal.PFF_TRABAJO_SOCIAL.EXTERNADO_NUMERO,
                                        EXTERNADO_PARENTESCO = EntityFederal.PFF_TRABAJO_SOCIAL.EXTERNADO_PARENTESCO.HasValue ? EntityFederal.PFF_TRABAJO_SOCIAL.EXTERNADO_PARENTESCO.Value != -1 ? EntityFederal.PFF_TRABAJO_SOCIAL.EXTERNADO_PARENTESCO : null : null,
                                        EXTERNADO_VIVIR_NOMBRE = EntityFederal.PFF_TRABAJO_SOCIAL.EXTERNADO_VIVIR_NOMBRE,
                                        RADICAN_ESTADO = EntityFederal.PFF_TRABAJO_SOCIAL.RADICAN_ESTADO,
                                        VISITA_FRECUENCIA = EntityFederal.PFF_TRABAJO_SOCIAL.VISITA_FRECUENCIA,
                                        FECHA = EntityFederal.PFF_TRABAJO_SOCIAL.FECHA,
                                        FECHA_NAC = EntityFederal.PFF_TRABAJO_SOCIAL.FECHA_NAC,
                                        ID_ANIO = EntityFederal.ID_ANIO,
                                        ID_CENTRO = EntityFederal.ID_CENTRO,
                                        ID_IMPUTADO = EntityFederal.ID_IMPUTADO,
                                        ID_ESTUDIO = EntityFederal.ID_ESTUDIO,
                                        ID_INGRESO = EntityFederal.ID_INGRESO,
                                        INFLUENCIADO_ESTANCIA_PRISION = EntityFederal.PFF_TRABAJO_SOCIAL.INFLUENCIADO_ESTANCIA_PRISION,
                                        LUGAR = EntityFederal.PFF_TRABAJO_SOCIAL.LUGAR,
                                        LUGAR_NAC = EntityFederal.PFF_TRABAJO_SOCIAL.LUGAR_NAC,
                                        NOMBRE = EntityFederal.PFF_TRABAJO_SOCIAL.NOMBRE,
                                        NUM_PAREJAS_ESTABLE = EntityFederal.PFF_TRABAJO_SOCIAL.NUM_PAREJAS_ESTABLE,
                                        OCUPACION_ANT = EntityFederal.PFF_TRABAJO_SOCIAL.OCUPACION_ANT.HasValue ? EntityFederal.PFF_TRABAJO_SOCIAL.OCUPACION_ANT.Value != -1 ? EntityFederal.PFF_TRABAJO_SOCIAL.OCUPACION_ANT : new short?() : new short?(),
                                        OFERTA_TRABAJO = EntityFederal.PFF_TRABAJO_SOCIAL.OFERTA_TRABAJO,
                                        OFERTA_TRABAJO_CONSISTE = EntityFederal.PFF_TRABAJO_SOCIAL.OFERTA_TRABAJO_CONSISTE,
                                        OPINION_CONCESION_BENEFICIOS = EntityFederal.PFF_TRABAJO_SOCIAL.OPINION_CONCESION_BENEFICIOS,
                                        OPINION_INTERNAMIENTO = EntityFederal.PFF_TRABAJO_SOCIAL.OPINION_INTERNAMIENTO,
                                        SERVICIOS_PUBLICOS = EntityFederal.PFF_TRABAJO_SOCIAL.SERVICIOS_PUBLICOS,
                                        SUELDO_PERCIBIDO = EntityFederal.PFF_TRABAJO_SOCIAL.SUELDO_PERCIBIDO,
                                        TIEMPO_LABORAR = EntityFederal.PFF_TRABAJO_SOCIAL.TIEMPO_LABORAR,
                                        TRABAJADORA_SOCIAL = EntityFederal.PFF_TRABAJO_SOCIAL.TRABAJADORA_SOCIAL,
                                        TRABAJO_DESEMP_ANTES = EntityFederal.PFF_TRABAJO_SOCIAL.TRABAJO_DESEMP_ANTES,
                                        VISITA_FAMILIARES = EntityFederal.PFF_TRABAJO_SOCIAL.VISITA_FAMILIARES,
                                        VISITA_OTROS_QUIIEN = EntityFederal.PFF_TRABAJO_SOCIAL.VISITA_OTROS_QUIIEN,
                                        VISITAS_OTROS = EntityFederal.PFF_TRABAJO_SOCIAL.VISITAS_OTROS,
                                        VISTA_PARENTESCO = EntityFederal.PFF_TRABAJO_SOCIAL.VISTA_PARENTESCO.HasValue ? EntityFederal.PFF_TRABAJO_SOCIAL.VISTA_PARENTESCO.Value != -1 ? EntityFederal.PFF_TRABAJO_SOCIAL.VISTA_PARENTESCO : null : null
                                    };

                                    #region Gru
                                    if (GruposFam != null && GruposFam.Any())
                                        foreach (var item in GruposFam)
                                            Context.Entry(item).State = System.Data.EntityState.Deleted;

                                    var _consecutivoSocialFederal = GetIDProceso<short>("PFF_GRUPO_FAMILIAR", "ID_CONSEC", string.Format("ID_CENTRO = {0} AND ID_ANIO = {1} AND ID_IMPUTADO = {2} AND ID_INGRESO = {3} AND ID_ESTUDIO = {4}", EntityFederal.ID_CENTRO, EntityFederal.ID_ANIO, EntityFederal.ID_IMPUTADO, EntityFederal.ID_INGRESO, EntityFederal.ID_ESTUDIO));

                                    if (EntityFederal.PFF_TRABAJO_SOCIAL.PFF_GRUPO_FAMILIAR != null && EntityFederal.PFF_TRABAJO_SOCIAL.PFF_GRUPO_FAMILIAR.Any())
                                        foreach (var item in EntityFederal.PFF_TRABAJO_SOCIAL.PFF_GRUPO_FAMILIAR)
                                        {
                                            var _NvoGrupo = new PFF_GRUPO_FAMILIAR()
                                            {
                                                EDAD = item.EDAD,
                                                ESTADO_CIVIL = item.ESTADO_CIVIL,
                                                ID_ANIO = EntityFederal.ID_ANIO,
                                                ID_CENTRO = EntityFederal.ID_CENTRO,
                                                ID_ESTUDIO = item.ID_ESTUDIO,
                                                ID_GRUPO_FAMILIAR = item.ID_GRUPO_FAMILIAR,
                                                ID_IMPUTADO = EntityFederal.ID_IMPUTADO,
                                                ID_INGRESO = item.ID_INGRESO,
                                                NOMBRE = item.NOMBRE,
                                                OCUPACION = item.OCUPACION,
                                                ID_GRADO = item.ID_GRADO,
                                                PARENTESCO = item.PARENTESCO,
                                                ID_CONSEC = _consecutivoSocialFederal,
                                            };

                                            ValidaEstudio.PFF_TRABAJO_SOCIAL.PFF_GRUPO_FAMILIAR.Add(_NvoGrupo);
                                            _consecutivoSocialFederal++;
                                        };

                                    #endregion

                                    Context.PFF_TRABAJO_SOCIAL.Add(ValidaEstudio.PFF_TRABAJO_SOCIAL);
                                }

                                else
                                {
                                    _EstudioTrabajoSocHecho.ALIMENTACION_FAM = EntityFederal.PFF_TRABAJO_SOCIAL.ALIMENTACION_FAM;
                                    _EstudioTrabajoSocHecho.APORTACIONES_FAM = EntityFederal.PFF_TRABAJO_SOCIAL.APORTACIONES_FAM;
                                    _EstudioTrabajoSocHecho.APOYO_FAM_OTROS = EntityFederal.PFF_TRABAJO_SOCIAL.APOYO_FAM_OTROS;
                                    _EstudioTrabajoSocHecho.AVAL_MORAL = EntityFederal.PFF_TRABAJO_SOCIAL.AVAL_MORAL;
                                    _EstudioTrabajoSocHecho.AVAL_MORAL_PARENTESCO = EntityFederal.PFF_TRABAJO_SOCIAL.AVAL_MORAL_PARENTESCO;
                                    _EstudioTrabajoSocHecho.CARACT_FP_ANTECE_PENALES_ADIC = EntityFederal.PFF_TRABAJO_SOCIAL.CARACT_FP_ANTECE_PENALES_ADIC;
                                    _EstudioTrabajoSocHecho.CARACT_FP_ANTECEDENTES_PENALES = EntityFederal.PFF_TRABAJO_SOCIAL.CARACT_FP_ANTECEDENTES_PENALES;
                                    _EstudioTrabajoSocHecho.CARACT_FP_CONCEPTO = EntityFederal.PFF_TRABAJO_SOCIAL.CARACT_FP_CONCEPTO;
                                    _EstudioTrabajoSocHecho.CARACT_FP_GRUPO = EntityFederal.PFF_TRABAJO_SOCIAL.CARACT_FP_GRUPO;
                                    _EstudioTrabajoSocHecho.CARACT_FP_NIVEL_SOCIO_CULTURAL = EntityFederal.PFF_TRABAJO_SOCIAL.CARACT_FP_NIVEL_SOCIO_CULTURAL;
                                    _EstudioTrabajoSocHecho.CARACT_FP_RELAC_INTERFAM = EntityFederal.PFF_TRABAJO_SOCIAL.CARACT_FP_RELAC_INTERFAM;
                                    _EstudioTrabajoSocHecho.CARACT_FP_VIOLENCIA_FAM = EntityFederal.PFF_TRABAJO_SOCIAL.CARACT_FP_VIOLENCIA_FAM;
                                    _EstudioTrabajoSocHecho.CARACT_FP_VIOLENCIA_FAM_ESPEFI = EntityFederal.PFF_TRABAJO_SOCIAL.CARACT_FP_VIOLENCIA_FAM_ESPEFI;
                                    _EstudioTrabajoSocHecho.CARACT_FS_GRUPO = EntityFederal.PFF_TRABAJO_SOCIAL.CARACT_FS_GRUPO;
                                    _EstudioTrabajoSocHecho.CARACT_FS_HIJOS_ANT = EntityFederal.PFF_TRABAJO_SOCIAL.CARACT_FS_HIJOS_ANT;
                                    _EstudioTrabajoSocHecho.CARACT_FS_NIVEL_SOCIO_CULTURAL = EntityFederal.PFF_TRABAJO_SOCIAL.CARACT_FS_NIVEL_SOCIO_CULTURAL;
                                    _EstudioTrabajoSocHecho.CARACT_FS_PROBLEMAS_CONDUCTA = EntityFederal.PFF_TRABAJO_SOCIAL.CARACT_FS_PROBLEMAS_CONDUCTA;
                                    _EstudioTrabajoSocHecho.CARACT_FS_PROBLEMAS_CONDUCTA_E = EntityFederal.PFF_TRABAJO_SOCIAL.CARACT_FS_PROBLEMAS_CONDUCTA_E;
                                    _EstudioTrabajoSocHecho.CARACT_FS_RELACION_MEDIO_EXT = EntityFederal.PFF_TRABAJO_SOCIAL.CARACT_FS_RELACION_MEDIO_EXT;
                                    _EstudioTrabajoSocHecho.CARACT_FS_RELACIONES_INTERFAM = EntityFederal.PFF_TRABAJO_SOCIAL.CARACT_FS_RELACIONES_INTERFAM;
                                    _EstudioTrabajoSocHecho.CARACT_FS_VIOLENCIA_INTRAFAM = EntityFederal.PFF_TRABAJO_SOCIAL.CARACT_FS_VIOLENCIA_INTRAFAM;
                                    _EstudioTrabajoSocHecho.CARACT_FS_VIOLENCIA_INTRAFAM_E = EntityFederal.PFF_TRABAJO_SOCIAL.CARACT_FS_VIOLENCIA_INTRAFAM_E;
                                    _EstudioTrabajoSocHecho.CARACT_FS_VIVIEN_DESCRIPCION = EntityFederal.PFF_TRABAJO_SOCIAL.CARACT_FS_VIVIEN_DESCRIPCION;
                                    _EstudioTrabajoSocHecho.CARACT_FS_VIVIEN_MOBILIARIO = EntityFederal.PFF_TRABAJO_SOCIAL.CARACT_FS_VIVIEN_MOBILIARIO;
                                    _EstudioTrabajoSocHecho.CARACT_FS_VIVIEN_NUM_HABITACIO = EntityFederal.PFF_TRABAJO_SOCIAL.CARACT_FS_VIVIEN_NUM_HABITACIO;
                                    _EstudioTrabajoSocHecho.CARACT_FS_VIVIEN_TRANSPORTE = EntityFederal.PFF_TRABAJO_SOCIAL.CARACT_FS_VIVIEN_TRANSPORTE;
                                    _EstudioTrabajoSocHecho.CARACT_FS_ZONA = EntityFederal.PFF_TRABAJO_SOCIAL.CARACT_FS_ZONA;
                                    _EstudioTrabajoSocHecho.DIAG_SOCIAL_PRONOS = EntityFederal.PFF_TRABAJO_SOCIAL.DIAG_SOCIAL_PRONOS;
                                    _EstudioTrabajoSocHecho.DIALECTO = EntityFederal.PFF_TRABAJO_SOCIAL.DIALECTO.HasValue ? EntityFederal.PFF_TRABAJO_SOCIAL.DIALECTO.Value != -1 ? EntityFederal.PFF_TRABAJO_SOCIAL.DIALECTO : new short?() : new short?();
                                    _EstudioTrabajoSocHecho.DIRECTOR_CENTRO = EntityFederal.PFF_TRABAJO_SOCIAL.DIRECTOR_CENTRO;
                                    _EstudioTrabajoSocHecho.DISTRIBUCION_GASTO_FAM = EntityFederal.PFF_TRABAJO_SOCIAL.DISTRIBUCION_GASTO_FAM;
                                    _EstudioTrabajoSocHecho.DOMICILIO = EntityFederal.PFF_TRABAJO_SOCIAL.DOMICILIO;
                                    _EstudioTrabajoSocHecho.ECO_FP_COOPERA_ACTUALMENTE = EntityFederal.PFF_TRABAJO_SOCIAL.ECO_FP_COOPERA_ACTUALMENTE;
                                    _EstudioTrabajoSocHecho.ECO_FP_FONDOS_AHORRO = EntityFederal.PFF_TRABAJO_SOCIAL.ECO_FP_FONDOS_AHORRO;
                                    _EstudioTrabajoSocHecho.ECO_FP_RESPONSABLE = EntityFederal.PFF_TRABAJO_SOCIAL.ECO_FP_RESPONSABLE;
                                    _EstudioTrabajoSocHecho.ECO_FP_TOTAL_EGRESOS_MEN = EntityFederal.PFF_TRABAJO_SOCIAL.ECO_FP_TOTAL_EGRESOS_MEN;
                                    _EstudioTrabajoSocHecho.ECO_FP_TOTAL_INGRESOS_MEN = EntityFederal.PFF_TRABAJO_SOCIAL.ECO_FP_TOTAL_INGRESOS_MEN;
                                    _EstudioTrabajoSocHecho.ECO_FP_ZONA = EntityFederal.PFF_TRABAJO_SOCIAL.ECO_FP_ZONA;
                                    _EstudioTrabajoSocHecho.EDO_CIVIL = EntityFederal.PFF_TRABAJO_SOCIAL.EDO_CIVIL.HasValue ? EntityFederal.PFF_TRABAJO_SOCIAL.EDO_CIVIL.Value != -1 ? EntityFederal.PFF_TRABAJO_SOCIAL.EDO_CIVIL : new short?() : new short?();
                                    _EstudioTrabajoSocHecho.ESCOLARIDAD_ACTUAL = EntityFederal.PFF_TRABAJO_SOCIAL.ESCOLARIDAD_ACTUAL.HasValue ? EntityFederal.PFF_TRABAJO_SOCIAL.ESCOLARIDAD_ACTUAL.Value != -1 ? EntityFederal.PFF_TRABAJO_SOCIAL.ESCOLARIDAD_ACTUAL : new short?() : new short?();
                                    _EstudioTrabajoSocHecho.ESCOLARIDAD_CENTRO = EntityFederal.PFF_TRABAJO_SOCIAL.ESCOLARIDAD_CENTRO.HasValue ? EntityFederal.PFF_TRABAJO_SOCIAL.ESCOLARIDAD_CENTRO.Value != -1 ? EntityFederal.PFF_TRABAJO_SOCIAL.ESCOLARIDAD_CENTRO : new short?() : new short?();
                                    _EstudioTrabajoSocHecho.EXTERNADO_CALLE = EntityFederal.PFF_TRABAJO_SOCIAL.EXTERNADO_CALLE;
                                    _EstudioTrabajoSocHecho.EXTERNADO_CIUDAD = EntityFederal.PFF_TRABAJO_SOCIAL.EXTERNADO_CIUDAD.HasValue ? EntityFederal.PFF_TRABAJO_SOCIAL.EXTERNADO_CIUDAD.Value != -1 ? EntityFederal.PFF_TRABAJO_SOCIAL.EXTERNADO_CIUDAD.Value != decimal.Zero ? EntityFederal.PFF_TRABAJO_SOCIAL.EXTERNADO_CIUDAD : new short?() : new short?() : new short?();
                                    _EstudioTrabajoSocHecho.EXTERNADO_COLONIA = EntityFederal.PFF_TRABAJO_SOCIAL.EXTERNADO_COLONIA.HasValue ? EntityFederal.PFF_TRABAJO_SOCIAL.EXTERNADO_COLONIA.Value != -1 ? EntityFederal.PFF_TRABAJO_SOCIAL.EXTERNADO_COLONIA.Value != decimal.Zero ? EntityFederal.PFF_TRABAJO_SOCIAL.EXTERNADO_COLONIA : new int?() : new int?() : new int?();
                                    _EstudioTrabajoSocHecho.EXTERNADO_CP = EntityFederal.PFF_TRABAJO_SOCIAL.EXTERNADO_CP;
                                    _EstudioTrabajoSocHecho.EXTERNADO_ENTIDAD = EntityFederal.PFF_TRABAJO_SOCIAL.EXTERNADO_ENTIDAD.HasValue ? EntityFederal.PFF_TRABAJO_SOCIAL.EXTERNADO_ENTIDAD.Value != -1 ? EntityFederal.PFF_TRABAJO_SOCIAL.EXTERNADO_ENTIDAD.Value != decimal.Zero ? EntityFederal.PFF_TRABAJO_SOCIAL.EXTERNADO_ENTIDAD : new short?() : new short?() : new short?();
                                    _EstudioTrabajoSocHecho.EXTERNADO_MUNICIPIO = EntityFederal.PFF_TRABAJO_SOCIAL.EXTERNADO_MUNICIPIO.HasValue ? EntityFederal.PFF_TRABAJO_SOCIAL.EXTERNADO_MUNICIPIO.Value != -1 ? EntityFederal.PFF_TRABAJO_SOCIAL.EXTERNADO_MUNICIPIO.Value != decimal.Zero ? EntityFederal.PFF_TRABAJO_SOCIAL.EXTERNADO_MUNICIPIO : new short?() : new short?() : new short?();
                                    _EstudioTrabajoSocHecho.EXTERNADO_NUMERO = EntityFederal.PFF_TRABAJO_SOCIAL.EXTERNADO_NUMERO;
                                    _EstudioTrabajoSocHecho.EXTERNADO_PARENTESCO = EntityFederal.PFF_TRABAJO_SOCIAL.EXTERNADO_PARENTESCO.HasValue ? EntityFederal.PFF_TRABAJO_SOCIAL.EXTERNADO_PARENTESCO.Value != -1 ? EntityFederal.PFF_TRABAJO_SOCIAL.EXTERNADO_PARENTESCO : null : null;//visita parentesco
                                    _EstudioTrabajoSocHecho.EXTERNADO_VIVIR_NOMBRE = EntityFederal.PFF_TRABAJO_SOCIAL.EXTERNADO_VIVIR_NOMBRE;
                                    _EstudioTrabajoSocHecho.FECHA_NAC = EntityFederal.PFF_TRABAJO_SOCIAL.FECHA_NAC;
                                    _EstudioTrabajoSocHecho.ID_ANIO = EntityFederal.ID_ANIO;
                                    _EstudioTrabajoSocHecho.ID_CENTRO = EntityFederal.ID_CENTRO;
                                    _EstudioTrabajoSocHecho.ID_IMPUTADO = EntityFederal.ID_IMPUTADO;
                                    _EstudioTrabajoSocHecho.ID_INGRESO = EntityFederal.ID_INGRESO;
                                    _EstudioTrabajoSocHecho.ID_ESTUDIO = EntityFederal.ID_ESTUDIO;
                                    _EstudioTrabajoSocHecho.INFLUENCIADO_ESTANCIA_PRISION = EntityFederal.PFF_TRABAJO_SOCIAL.INFLUENCIADO_ESTANCIA_PRISION;
                                    _EstudioTrabajoSocHecho.LUGAR = EntityFederal.PFF_TRABAJO_SOCIAL.LUGAR;
                                    _EstudioTrabajoSocHecho.LUGAR_NAC = EntityFederal.PFF_TRABAJO_SOCIAL.LUGAR_NAC;
                                    _EstudioTrabajoSocHecho.NOMBRE = EntityFederal.PFF_TRABAJO_SOCIAL.NOMBRE;
                                    _EstudioTrabajoSocHecho.NUM_PAREJAS_ESTABLE = EntityFederal.PFF_TRABAJO_SOCIAL.NUM_PAREJAS_ESTABLE;
                                    _EstudioTrabajoSocHecho.OCUPACION_ANT = EntityFederal.PFF_TRABAJO_SOCIAL.OCUPACION_ANT.HasValue ? EntityFederal.PFF_TRABAJO_SOCIAL.OCUPACION_ANT.Value != -1 ? EntityFederal.PFF_TRABAJO_SOCIAL.OCUPACION_ANT : new short?() : new short?();
                                    _EstudioTrabajoSocHecho.OFERTA_TRABAJO = EntityFederal.PFF_TRABAJO_SOCIAL.OFERTA_TRABAJO;
                                    _EstudioTrabajoSocHecho.OFERTA_TRABAJO_CONSISTE = EntityFederal.PFF_TRABAJO_SOCIAL.OFERTA_TRABAJO_CONSISTE;
                                    _EstudioTrabajoSocHecho.OPINION_CONCESION_BENEFICIOS = EntityFederal.PFF_TRABAJO_SOCIAL.OPINION_CONCESION_BENEFICIOS;
                                    _EstudioTrabajoSocHecho.OPINION_INTERNAMIENTO = EntityFederal.PFF_TRABAJO_SOCIAL.OPINION_INTERNAMIENTO;
                                    _EstudioTrabajoSocHecho.RADICAN_ESTADO = EntityFederal.PFF_TRABAJO_SOCIAL.RADICAN_ESTADO;
                                    _EstudioTrabajoSocHecho.SERVICIOS_PUBLICOS = EntityFederal.PFF_TRABAJO_SOCIAL.SERVICIOS_PUBLICOS;
                                    _EstudioTrabajoSocHecho.SUELDO_PERCIBIDO = EntityFederal.PFF_TRABAJO_SOCIAL.SUELDO_PERCIBIDO;
                                    _EstudioTrabajoSocHecho.TIEMPO_LABORAR = EntityFederal.PFF_TRABAJO_SOCIAL.TIEMPO_LABORAR;
                                    _EstudioTrabajoSocHecho.TRABAJADORA_SOCIAL = EntityFederal.PFF_TRABAJO_SOCIAL.TRABAJADORA_SOCIAL;
                                    _EstudioTrabajoSocHecho.TRABAJO_DESEMP_ANTES = EntityFederal.PFF_TRABAJO_SOCIAL.TRABAJO_DESEMP_ANTES;
                                    _EstudioTrabajoSocHecho.VISITA_FAMILIARES = EntityFederal.PFF_TRABAJO_SOCIAL.VISITA_FAMILIARES;
                                    _EstudioTrabajoSocHecho.VISITA_OTROS_QUIIEN = EntityFederal.PFF_TRABAJO_SOCIAL.VISITA_OTROS_QUIIEN;
                                    _EstudioTrabajoSocHecho.VISITAS_OTROS = EntityFederal.PFF_TRABAJO_SOCIAL.VISITAS_OTROS;
                                    _EstudioTrabajoSocHecho.VISTA_PARENTESCO = EntityFederal.PFF_TRABAJO_SOCIAL.VISTA_PARENTESCO.HasValue ? EntityFederal.PFF_TRABAJO_SOCIAL.VISTA_PARENTESCO.Value != -1 ? EntityFederal.PFF_TRABAJO_SOCIAL.VISTA_PARENTESCO : null : null;
                                    Context.Entry(_EstudioTrabajoSocHecho).State = System.Data.EntityState.Modified;

                                    #region Grup
                                    if (GruposFam != null && GruposFam.Any())
                                        foreach (var item in GruposFam)
                                            Context.Entry(item).State = System.Data.EntityState.Deleted;

                                    var _consecutivoSocialFederal = GetIDProceso<short>("PFF_GRUPO_FAMILIAR", "ID_CONSEC", string.Format("ID_CENTRO = {0} AND ID_ANIO = {1} AND ID_IMPUTADO = {2} AND ID_INGRESO = {3} AND ID_ESTUDIO = {4}", EntityFederal.ID_CENTRO, EntityFederal.ID_ANIO, EntityFederal.ID_IMPUTADO, EntityFederal.ID_INGRESO, EntityFederal.ID_ESTUDIO));
                                    if (EntityFederal.PFF_TRABAJO_SOCIAL.PFF_GRUPO_FAMILIAR != null && EntityFederal.PFF_TRABAJO_SOCIAL.PFF_GRUPO_FAMILIAR.Any())
                                        foreach (var item in EntityFederal.PFF_TRABAJO_SOCIAL.PFF_GRUPO_FAMILIAR)
                                        {
                                            var _NvoGrupo = new PFF_GRUPO_FAMILIAR()
                                            {
                                                EDAD = item.EDAD,
                                                ESTADO_CIVIL = item.ESTADO_CIVIL,
                                                ID_ANIO = EntityFederal.ID_ANIO,
                                                ID_CENTRO = EntityFederal.ID_CENTRO,
                                                ID_ESTUDIO = item.ID_ESTUDIO,
                                                ID_GRUPO_FAMILIAR = item.ID_GRUPO_FAMILIAR,
                                                ID_IMPUTADO = EntityFederal.ID_IMPUTADO,
                                                ID_INGRESO = item.ID_INGRESO,
                                                NOMBRE = item.NOMBRE,
                                                OCUPACION = item.OCUPACION,
                                                PARENTESCO = item.PARENTESCO,
                                                ID_CONSEC = _consecutivoSocialFederal,
                                                ID_GRADO = item.ID_GRADO
                                            };

                                            ValidaEstudio.PFF_TRABAJO_SOCIAL.PFF_GRUPO_FAMILIAR.Add(_NvoGrupo);
                                            _consecutivoSocialFederal++;
                                        };
                                    #endregion
                                }


                                #region Definicion de detalle de estudio de trabajo social de fuero federal
                                if (_EstudioPadre != null)
                                {
                                    short? _Estatus = new short?();
                                    if (string.IsNullOrEmpty(EntityFederal.PFF_TRABAJO_SOCIAL.ALIMENTACION_FAM) || string.IsNullOrEmpty(EntityFederal.PFF_TRABAJO_SOCIAL.APORTACIONES_FAM) || string.IsNullOrEmpty(EntityFederal.PFF_TRABAJO_SOCIAL.AVAL_MORAL) || string.IsNullOrEmpty(EntityFederal.PFF_TRABAJO_SOCIAL.CARACT_FP_CONCEPTO) || string.IsNullOrEmpty(EntityFederal.PFF_TRABAJO_SOCIAL.CARACT_FP_GRUPO) ||
                                        string.IsNullOrEmpty(EntityFederal.PFF_TRABAJO_SOCIAL.CARACT_FP_NIVEL_SOCIO_CULTURAL) || string.IsNullOrEmpty(EntityFederal.PFF_TRABAJO_SOCIAL.CARACT_FP_RELAC_INTERFAM) ||
                                        string.IsNullOrEmpty(EntityFederal.PFF_TRABAJO_SOCIAL.CARACT_FP_VIOLENCIA_FAM) || string.IsNullOrEmpty(EntityFederal.PFF_TRABAJO_SOCIAL.CARACT_FS_GRUPO) ||
                                        string.IsNullOrEmpty(EntityFederal.PFF_TRABAJO_SOCIAL.CARACT_FS_HIJOS_ANT) || string.IsNullOrEmpty(EntityFederal.PFF_TRABAJO_SOCIAL.CARACT_FS_NIVEL_SOCIO_CULTURAL) ||
                                        string.IsNullOrEmpty(EntityFederal.PFF_TRABAJO_SOCIAL.CARACT_FS_PROBLEMAS_CONDUCTA) || string.IsNullOrEmpty(EntityFederal.PFF_TRABAJO_SOCIAL.CARACT_FS_RELACION_MEDIO_EXT) ||
                                        string.IsNullOrEmpty(EntityFederal.PFF_TRABAJO_SOCIAL.CARACT_FS_RELACIONES_INTERFAM) || string.IsNullOrEmpty(EntityFederal.PFF_TRABAJO_SOCIAL.CARACT_FS_VIOLENCIA_INTRAFAM) ||
                                        string.IsNullOrEmpty(EntityFederal.PFF_TRABAJO_SOCIAL.CARACT_FS_VIVIEN_DESCRIPCION) || string.IsNullOrEmpty(EntityFederal.PFF_TRABAJO_SOCIAL.CARACT_FS_VIVIEN_MOBILIARIO) ||
                                        string.IsNullOrEmpty(EntityFederal.PFF_TRABAJO_SOCIAL.CARACT_FS_VIVIEN_TRANSPORTE) || string.IsNullOrEmpty(EntityFederal.PFF_TRABAJO_SOCIAL.CARACT_FS_ZONA) ||
                                        string.IsNullOrEmpty(EntityFederal.PFF_TRABAJO_SOCIAL.DIAG_SOCIAL_PRONOS) || EntityFederal.PFF_TRABAJO_SOCIAL.DIALECTO == null || string.IsNullOrEmpty(EntityFederal.PFF_TRABAJO_SOCIAL.DISTRIBUCION_GASTO_FAM) ||
                                        string.IsNullOrEmpty(EntityFederal.PFF_TRABAJO_SOCIAL.DOMICILIO) || string.IsNullOrEmpty(EntityFederal.PFF_TRABAJO_SOCIAL.ECO_FP_COOPERA_ACTUALMENTE) ||
                                        string.IsNullOrEmpty(EntityFederal.PFF_TRABAJO_SOCIAL.ECO_FP_FONDOS_AHORRO) || string.IsNullOrEmpty(EntityFederal.PFF_TRABAJO_SOCIAL.ECO_FP_RESPONSABLE) ||
                                        string.IsNullOrEmpty(EntityFederal.PFF_TRABAJO_SOCIAL.ECO_FP_ZONA) || EntityFederal.PFF_TRABAJO_SOCIAL.ESCOLARIDAD_ACTUAL == null || EntityFederal.PFF_TRABAJO_SOCIAL.ESCOLARIDAD_CENTRO == null ||
                                        string.IsNullOrEmpty(EntityFederal.PFF_TRABAJO_SOCIAL.EXTERNADO_CALLE) || string.IsNullOrEmpty(EntityFederal.PFF_TRABAJO_SOCIAL.EXTERNADO_CP) ||
                                        string.IsNullOrEmpty(EntityFederal.PFF_TRABAJO_SOCIAL.EXTERNADO_NUMERO) || string.IsNullOrEmpty(EntityFederal.PFF_TRABAJO_SOCIAL.EXTERNADO_VIVIR_NOMBRE) ||
                                        string.IsNullOrEmpty(EntityFederal.PFF_TRABAJO_SOCIAL.INFLUENCIADO_ESTANCIA_PRISION) || string.IsNullOrEmpty(EntityFederal.PFF_TRABAJO_SOCIAL.LUGAR_NAC) ||
                                        string.IsNullOrEmpty(EntityFederal.PFF_TRABAJO_SOCIAL.NOMBRE) || string.IsNullOrEmpty(EntityFederal.PFF_TRABAJO_SOCIAL.NUM_PAREJAS_ESTABLE) ||
                                        EntityFederal.PFF_TRABAJO_SOCIAL.OCUPACION_ANT == null || string.IsNullOrEmpty(EntityFederal.PFF_TRABAJO_SOCIAL.OFERTA_TRABAJO) ||
                                        string.IsNullOrEmpty(EntityFederal.PFF_TRABAJO_SOCIAL.OPINION_CONCESION_BENEFICIOS) || string.IsNullOrEmpty(EntityFederal.PFF_TRABAJO_SOCIAL.OPINION_INTERNAMIENTO) ||
                                        string.IsNullOrEmpty(EntityFederal.PFF_TRABAJO_SOCIAL.SERVICIOS_PUBLICOS) || string.IsNullOrEmpty(EntityFederal.PFF_TRABAJO_SOCIAL.TIEMPO_LABORAR) ||
                                        string.IsNullOrEmpty(EntityFederal.PFF_TRABAJO_SOCIAL.TRABAJO_DESEMP_ANTES) || string.IsNullOrEmpty(EntityFederal.PFF_TRABAJO_SOCIAL.VISITA_FAMILIARES) ||
                                        EntityFederal.PFF_TRABAJO_SOCIAL.CARACT_FS_VIVIEN_NUM_HABITACIO == null || EntityFederal.PFF_TRABAJO_SOCIAL.ECO_FP_TOTAL_EGRESOS_MEN == null || EntityFederal.PFF_TRABAJO_SOCIAL.ECO_FP_TOTAL_INGRESOS_MEN == null ||
                                        EntityFederal.PFF_TRABAJO_SOCIAL.EDO_CIVIL == null || EntityFederal.PFF_TRABAJO_SOCIAL.EXTERNADO_CIUDAD == null ||
                                        EntityFederal.PFF_TRABAJO_SOCIAL.EXTERNADO_COLONIA == null || EntityFederal.PFF_TRABAJO_SOCIAL.EXTERNADO_ENTIDAD == null || EntityFederal.PFF_TRABAJO_SOCIAL.EXTERNADO_MUNICIPIO == null ||
                                        EntityFederal.PFF_TRABAJO_SOCIAL.EXTERNADO_PARENTESCO == null || EntityFederal.PFF_TRABAJO_SOCIAL.FECHA == null || EntityFederal.PFF_TRABAJO_SOCIAL.FECHA_NAC == null ||
                                        EntityFederal.PFF_TRABAJO_SOCIAL.SUELDO_PERCIBIDO == null)
                                        _Estatus = (short)eEstatudDetallePersonalidad.ASIGNADO;
                                    else
                                        _Estatus = (short)eEstatudDetallePersonalidad.TERMINADO;

                                    var _DesarrolloEstudioTSFederal = Context.PERSONALIDAD_DETALLE.FirstOrDefault(x => x.ID_IMPUTADO == EntityFederal.ID_IMPUTADO && x.ID_ESTUDIO == _EstudioPadre.ID_ESTUDIO && x.ID_INGRESO == EntityFederal.ID_INGRESO && x.ID_TIPO == (short)eTiposEstudio.TRABAJO_SOCIAL_FEDERAL && x.ID_CENTRO == EntityFederal.ID_CENTRO && x.ID_ANIO == EntityFederal.ID_ANIO);
                                    if (_DesarrolloEstudioTSFederal == null)
                                    {
                                        short _ConsecutivoPersonalidadDetalle = GetIDProceso<short>("PERSONALIDAD_DETALLE", "ID_DETALLE", "1=1");
                                        var NvoDetalle = new PERSONALIDAD_DETALLE()
                                        {
                                            ID_ANIO = EntityFederal.ID_ANIO,
                                            ID_CENTRO = EntityFederal.ID_CENTRO,
                                            ID_DETALLE = _ConsecutivoPersonalidadDetalle,
                                            ID_ESTUDIO = _EstudioPadre.ID_ESTUDIO,
                                            ID_IMPUTADO = EntityFederal.ID_IMPUTADO,
                                            ID_INGRESO = EntityFederal.ID_INGRESO,
                                            ID_TIPO = (short)eTiposEstudio.TRABAJO_SOCIAL_FEDERAL,
                                            INICIO_FEC = GetFechaServerDate(),
                                            RESULTADO = _Estatus == (short)eEstatudDetallePersonalidad.TERMINADO ? "S" : "N",
                                            SOLICITUD_FEC = null,
                                            TERMINO_FEC = _Estatus == (short)eEstatudDetallePersonalidad.TERMINADO ? GetFechaServerDate() : new System.DateTime?(),
                                            TIPO_MEDIA = string.Empty,
                                            ID_ESTATUS = _Estatus
                                        };

                                        Context.PERSONALIDAD_DETALLE.Add(NvoDetalle);
                                    }
                                    else
                                    {
                                        _DesarrolloEstudioTSFederal.RESULTADO = _Estatus.HasValue ? _Estatus.Value == (short)eEstatudDetallePersonalidad.TERMINADO ? "S" : "N" : "N";
                                        _DesarrolloEstudioTSFederal.ID_ESTATUS = _Estatus;
                                        Context.Entry(_DesarrolloEstudioTSFederal).State = System.Data.EntityState.Modified;
                                    };
                                };
                                #endregion
                            }

                            if (EntityFederal.PFF_CAPACITACION != null)
                            {
                                var _EstudioCapacitacionFederal = Context.PFF_CAPACITACION.FirstOrDefault(x => x.ID_INGRESO == EntityFederal.ID_INGRESO && x.ID_IMPUTADO == EntityFederal.ID_IMPUTADO && x.ID_ESTUDIO == EntityFederal.ID_ESTUDIO && x.ID_ANIO == EntityFederal.ID_ANIO && x.ID_CENTRO == EntityFederal.ID_CENTRO && x.ID_ANIO == EntityFederal.ID_ANIO);
                                var _DiasGuardados = Context.PFF_DIAS_LABORADO.Where(x => x.ID_INGRESO == EntityFederal.ID_INGRESO && x.ID_IMPUTADO == EntityFederal.ID_IMPUTADO && x.ID_ESTUDIO == EntityFederal.ID_ESTUDIO && x.ID_ANIO == EntityFederal.ID_ANIO && x.ID_CENTRO == EntityFederal.ID_CENTRO && x.ID_ANIO == EntityFederal.ID_ANIO);
                                var _GruposCapac = Context.PFF_CAPACITACION_CURSO.Where(x => x.ID_INGRESO == EntityFederal.ID_INGRESO && x.ID_IMPUTADO == EntityFederal.ID_IMPUTADO && x.ID_ESTUDIO == EntityFederal.ID_ESTUDIO && x.ID_ANIO == EntityFederal.ID_ANIO && x.ID_CENTRO == EntityFederal.ID_CENTRO && x.ID_ANIO == EntityFederal.ID_ANIO);
                                if (_EstudioCapacitacionFederal == null)
                                {
                                    ValidaEstudio.PFF_CAPACITACION = new PFF_CAPACITACION()
                                    {
                                        A_TOTAL_DIAS_LABORADOS = EntityFederal.PFF_CAPACITACION.A_TOTAL_DIAS_LABORADOS,
                                        ACTITUDES_DESEMPENO_ACT = EntityFederal.PFF_CAPACITACION.ACTITUDES_DESEMPENO_ACT,
                                        ACTIVIDAD_PRODUC_ACTUAL = EntityFederal.PFF_CAPACITACION.ACTIVIDAD_PRODUC_ACTUAL,
                                        ATIENDE_INDICACIONES = EntityFederal.PFF_CAPACITACION.ATIENDE_INDICACIONES,
                                        B_DIAS_LABORADOS_OTROS_CERESOS = EntityFederal.PFF_CAPACITACION.B_DIAS_LABORADOS_OTROS_CERESOS,
                                        CAMBIO_ACTIVIDAD = EntityFederal.PFF_CAPACITACION.CAMBIO_ACTIVIDAD,
                                        CAMBIO_ACTIVIDAD_POR_QUE = EntityFederal.PFF_CAPACITACION.CAMBIO_ACTIVIDAD_POR_QUE,
                                        CONCLUSIONES = EntityFederal.PFF_CAPACITACION.CONCLUSIONES,
                                        DESCUIDADO_LABORES = EntityFederal.PFF_CAPACITACION.DESCUIDADO_LABORES,
                                        DIRECTOR_CENTRO = EntityFederal.PFF_CAPACITACION.DIRECTOR_CENTRO,
                                        FECHA = EntityFederal.PFF_CAPACITACION.FECHA,
                                        FONDO_AHORRO = EntityFederal.PFF_CAPACITACION.FONDO_AHORRO,
                                        FONDO_AHORRO_COMPESACION_ACTUA = EntityFederal.PFF_CAPACITACION.FONDO_AHORRO_COMPESACION_ACTUA,
                                        HA_PROGRESADO_OFICIO = EntityFederal.PFF_CAPACITACION.HA_PROGRESADO_OFICIO,
                                        ID_ANIO = EntityFederal.ID_ANIO,
                                        ID_CENTRO = EntityFederal.ID_CENTRO,
                                        ID_ESTUDIO = EntityFederal.ID_ESTUDIO,
                                        ID_IMPUTADO = EntityFederal.ID_IMPUTADO,
                                        ID_INGRESO = EntityFederal.ID_INGRESO,
                                        JEFE_SECC_INDUSTRIAL = EntityFederal.PFF_CAPACITACION.JEFE_SECC_INDUSTRIAL,
                                        LUGAR = EntityFederal.PFF_CAPACITACION.LUGAR,
                                        MOTIVO_TIEMPO_INTERRUP_ACT = EntityFederal.PFF_CAPACITACION.MOTIVO_TIEMPO_INTERRUP_ACT,
                                        NO_CURSOS_MOTIVO = EntityFederal.PFF_CAPACITACION.NO_CURSOS_MOTIVO,
                                        NOMBRE = EntityFederal.PFF_CAPACITACION.NOMBRE,
                                        OFICIO_ANTES_RECLUSION = EntityFederal.PFF_CAPACITACION.OFICIO_ANTES_RECLUSION.HasValue ? EntityFederal.PFF_CAPACITACION.OFICIO_ANTES_RECLUSION != -1 ? EntityFederal.PFF_CAPACITACION.OFICIO_ANTES_RECLUSION : null : null,
                                        RECIBIO_CONSTANCIA = EntityFederal.PFF_CAPACITACION.RECIBIO_CONSTANCIA,
                                        SALARIO_DEVENGABA_DETENCION = EntityFederal.PFF_CAPACITACION.SALARIO_DEVENGABA_DETENCION,
                                        SATISFACE_ACTIVIDAD = EntityFederal.PFF_CAPACITACION.SATISFACE_ACTIVIDAD,
                                        SECCION = EntityFederal.PFF_CAPACITACION.SECCION,
                                        TOTAL_A_B = EntityFederal.PFF_CAPACITACION.TOTAL_A_B
                                    };

                                    #region Capac
                                    if (_DiasGuardados != null && _DiasGuardados.Any())
                                        foreach (var item in _DiasGuardados)
                                            Context.Entry(item).State = System.Data.EntityState.Deleted;

                                    if (_GruposCapac != null && _GruposCapac.Any())
                                        foreach (var item in _GruposCapac)
                                            Context.Entry(item).State = System.Data.EntityState.Deleted;

                                    if (EntityFederal.PFF_CAPACITACION.PFF_DIAS_LABORADO != null && EntityFederal.PFF_CAPACITACION.PFF_DIAS_LABORADO.Any())
                                        foreach (var item in EntityFederal.PFF_CAPACITACION.PFF_DIAS_LABORADO)
                                        {
                                            var _NvoCapac = new PFF_DIAS_LABORADO()
                                            {
                                                ANIO = item.ANIO,
                                                DIAS_TRABAJADOS = item.DIAS_TRABAJADOS,
                                                ID_ANIO = EntityFederal.ID_ANIO,
                                                ID_IMPUTADO = EntityFederal.ID_IMPUTADO,
                                                ID_CENTRO = EntityFederal.ID_CENTRO,
                                                MES = item.MES,
                                                ID_ESTUDIO = EntityFederal.ID_ESTUDIO,
                                                ID_INGRESO = EntityFederal.ID_INGRESO
                                            };

                                            ValidaEstudio.PFF_CAPACITACION.PFF_DIAS_LABORADO.Add(_NvoCapac);
                                        };

                                    var _consecutivoCapacitacionFederal = GetIDProceso<short>("PFF_CAPACITACION_CURSO", "ID_CONSEC", string.Format("ID_CENTRO = {0} AND ID_ANIO = {1} AND ID_IMPUTADO = {2} AND ID_INGRESO = {3} AND ID_ESTUDIO = {4}", EntityFederal.ID_CENTRO, EntityFederal.ID_ANIO, EntityFederal.ID_IMPUTADO, EntityFederal.ID_INGRESO, EntityFederal.ID_ESTUDIO));

                                    if (EntityFederal.PFF_CAPACITACION.PFF_CAPACITACION_CURSO != null && EntityFederal.PFF_CAPACITACION.PFF_CAPACITACION_CURSO.Any())
                                        foreach (var item in EntityFederal.PFF_CAPACITACION.PFF_CAPACITACION_CURSO)
                                        {
                                            PFF_CAPACITACION_CURSO _NvaCurso = new PFF_CAPACITACION_CURSO()
                                            {
                                                CURSO = item.CURSO,
                                                FECHA_INICIO = item.FECHA_INICIO,
                                                FECHA_TERMINO = item.FECHA_TERMINO,
                                                ID_ANIO = EntityFederal.ID_ANIO,
                                                ID_IMPUTADO = EntityFederal.ID_IMPUTADO,
                                                ID_CENTRO = EntityFederal.ID_CENTRO,
                                                ID_ESTUDIO = EntityFederal.ID_ESTUDIO,
                                                ID_INGRESO = EntityFederal.ID_INGRESO,
                                                ID_CONSEC = _consecutivoCapacitacionFederal
                                            };

                                            ValidaEstudio.PFF_CAPACITACION.PFF_CAPACITACION_CURSO.Add(_NvaCurso);
                                            _consecutivoCapacitacionFederal++;
                                        };

                                    #endregion
                                }

                                else
                                {
                                    _EstudioCapacitacionFederal.A_TOTAL_DIAS_LABORADOS = EntityFederal.PFF_CAPACITACION.A_TOTAL_DIAS_LABORADOS;
                                    _EstudioCapacitacionFederal.ACTITUDES_DESEMPENO_ACT = EntityFederal.PFF_CAPACITACION.ACTITUDES_DESEMPENO_ACT;
                                    _EstudioCapacitacionFederal.ACTIVIDAD_PRODUC_ACTUAL = EntityFederal.PFF_CAPACITACION.ACTIVIDAD_PRODUC_ACTUAL;
                                    _EstudioCapacitacionFederal.ATIENDE_INDICACIONES = EntityFederal.PFF_CAPACITACION.ATIENDE_INDICACIONES;
                                    _EstudioCapacitacionFederal.B_DIAS_LABORADOS_OTROS_CERESOS = EntityFederal.PFF_CAPACITACION.B_DIAS_LABORADOS_OTROS_CERESOS;
                                    _EstudioCapacitacionFederal.CAMBIO_ACTIVIDAD = EntityFederal.PFF_CAPACITACION.CAMBIO_ACTIVIDAD;
                                    _EstudioCapacitacionFederal.CAMBIO_ACTIVIDAD_POR_QUE = EntityFederal.PFF_CAPACITACION.CAMBIO_ACTIVIDAD_POR_QUE;
                                    _EstudioCapacitacionFederal.CONCLUSIONES = EntityFederal.PFF_CAPACITACION.CONCLUSIONES;
                                    _EstudioCapacitacionFederal.DESCUIDADO_LABORES = EntityFederal.PFF_CAPACITACION.DESCUIDADO_LABORES;
                                    _EstudioCapacitacionFederal.DIRECTOR_CENTRO = EntityFederal.PFF_CAPACITACION.DIRECTOR_CENTRO;
                                    _EstudioCapacitacionFederal.FECHA = EntityFederal.PFF_CAPACITACION.FECHA;
                                    _EstudioCapacitacionFederal.FONDO_AHORRO = EntityFederal.PFF_CAPACITACION.FONDO_AHORRO;
                                    _EstudioCapacitacionFederal.FONDO_AHORRO_COMPESACION_ACTUA = EntityFederal.PFF_CAPACITACION.FONDO_AHORRO_COMPESACION_ACTUA;
                                    _EstudioCapacitacionFederal.HA_PROGRESADO_OFICIO = EntityFederal.PFF_CAPACITACION.HA_PROGRESADO_OFICIO;
                                    _EstudioCapacitacionFederal.ID_ANIO = EntityFederal.ID_ANIO;
                                    _EstudioCapacitacionFederal.ID_CENTRO = EntityFederal.ID_CENTRO;
                                    _EstudioCapacitacionFederal.ID_ESTUDIO = EntityFederal.ID_ESTUDIO;
                                    _EstudioCapacitacionFederal.ID_IMPUTADO = EntityFederal.ID_IMPUTADO;
                                    _EstudioCapacitacionFederal.ID_INGRESO = EntityFederal.ID_INGRESO;
                                    _EstudioCapacitacionFederal.JEFE_SECC_INDUSTRIAL = EntityFederal.PFF_CAPACITACION.JEFE_SECC_INDUSTRIAL;
                                    _EstudioCapacitacionFederal.LUGAR = EntityFederal.PFF_CAPACITACION.LUGAR;
                                    _EstudioCapacitacionFederal.MOTIVO_TIEMPO_INTERRUP_ACT = EntityFederal.PFF_CAPACITACION.MOTIVO_TIEMPO_INTERRUP_ACT;
                                    _EstudioCapacitacionFederal.NO_CURSOS_MOTIVO = EntityFederal.PFF_CAPACITACION.NO_CURSOS_MOTIVO;
                                    _EstudioCapacitacionFederal.NOMBRE = EntityFederal.PFF_CAPACITACION.NOMBRE;
                                    _EstudioCapacitacionFederal.OFICIO_ANTES_RECLUSION = EntityFederal.PFF_CAPACITACION.OFICIO_ANTES_RECLUSION.HasValue ? EntityFederal.PFF_CAPACITACION.OFICIO_ANTES_RECLUSION != -1 ? EntityFederal.PFF_CAPACITACION.OFICIO_ANTES_RECLUSION : null : null;
                                    _EstudioCapacitacionFederal.RECIBIO_CONSTANCIA = EntityFederal.PFF_CAPACITACION.RECIBIO_CONSTANCIA;
                                    _EstudioCapacitacionFederal.SALARIO_DEVENGABA_DETENCION = EntityFederal.PFF_CAPACITACION.SALARIO_DEVENGABA_DETENCION;
                                    _EstudioCapacitacionFederal.SATISFACE_ACTIVIDAD = EntityFederal.PFF_CAPACITACION.SATISFACE_ACTIVIDAD;
                                    _EstudioCapacitacionFederal.SECCION = EntityFederal.PFF_CAPACITACION.SECCION;
                                    _EstudioCapacitacionFederal.TOTAL_A_B = EntityFederal.PFF_CAPACITACION.TOTAL_A_B;
                                    Context.Entry(_EstudioCapacitacionFederal).State = System.Data.EntityState.Modified;

                                    #region Capa
                                    var _consecutivoCapacitacionFederal = GetIDProceso<short>("PFF_CAPACITACION_CURSO", "ID_CONSEC", string.Format("ID_CENTRO = {0} AND ID_ANIO = {1} AND ID_IMPUTADO = {2} AND ID_INGRESO = {3} AND ID_ESTUDIO = {4}", EntityFederal.ID_CENTRO, EntityFederal.ID_ANIO, EntityFederal.ID_IMPUTADO, EntityFederal.ID_INGRESO, EntityFederal.ID_ESTUDIO));
                                    if (_DiasGuardados != null && _DiasGuardados.Any())
                                        foreach (var item in _DiasGuardados)
                                            Context.Entry(item).State = System.Data.EntityState.Deleted;

                                    if (_GruposCapac != null && _GruposCapac.Any())
                                        foreach (var item in _GruposCapac)
                                            Context.Entry(item).State = System.Data.EntityState.Deleted;

                                    if (EntityFederal.PFF_CAPACITACION.PFF_DIAS_LABORADO != null && EntityFederal.PFF_CAPACITACION.PFF_DIAS_LABORADO.Any())
                                        foreach (var item in EntityFederal.PFF_CAPACITACION.PFF_DIAS_LABORADO)
                                        {
                                            var _NvoCapac = new PFF_DIAS_LABORADO()
                                            {
                                                ANIO = item.ANIO,
                                                DIAS_TRABAJADOS = item.DIAS_TRABAJADOS,
                                                ID_ANIO = EntityFederal.ID_ANIO,
                                                ID_CENTRO = EntityFederal.ID_CENTRO,
                                                ID_ESTUDIO = EntityFederal.ID_ESTUDIO,
                                                ID_IMPUTADO = EntityFederal.ID_IMPUTADO,
                                                ID_INGRESO = EntityFederal.ID_INGRESO,
                                                MES = item.MES
                                            };

                                            ValidaEstudio.PFF_CAPACITACION.PFF_DIAS_LABORADO.Add(_NvoCapac);
                                        };

                                    if (EntityFederal.PFF_CAPACITACION.PFF_CAPACITACION_CURSO != null && EntityFederal.PFF_CAPACITACION.PFF_CAPACITACION_CURSO.Any())
                                        foreach (var item in EntityFederal.PFF_CAPACITACION.PFF_CAPACITACION_CURSO)
                                        {
                                            var _NvaCurso = new PFF_CAPACITACION_CURSO()
                                            {
                                                CURSO = item.CURSO,
                                                FECHA_INICIO = item.FECHA_INICIO,
                                                FECHA_TERMINO = item.FECHA_TERMINO,
                                                ID_ANIO = EntityFederal.ID_ANIO,
                                                ID_CENTRO = EntityFederal.ID_CENTRO,
                                                ID_CONSEC = _consecutivoCapacitacionFederal,
                                                ID_ESTUDIO = EntityFederal.ID_ESTUDIO,
                                                ID_IMPUTADO = EntityFederal.ID_IMPUTADO,
                                                ID_INGRESO = EntityFederal.ID_INGRESO
                                            };

                                            ValidaEstudio.PFF_CAPACITACION.PFF_CAPACITACION_CURSO.Add(_NvaCurso);
                                            _consecutivoCapacitacionFederal++;
                                        };

                                    #endregion
                                };


                                #region Definicion de detalle de estudio de capacitacion de fuero federal
                                if (_EstudioPadre != null)
                                {
                                    short? _Estatus = new short?();
                                    if (string.IsNullOrEmpty(EntityFederal.PFF_CAPACITACION.ACTITUDES_DESEMPENO_ACT) || string.IsNullOrEmpty(EntityFederal.PFF_CAPACITACION.ACTIVIDAD_PRODUC_ACTUAL) ||
                                        string.IsNullOrEmpty(EntityFederal.PFF_CAPACITACION.ATIENDE_INDICACIONES) || string.IsNullOrEmpty(EntityFederal.PFF_CAPACITACION.CAMBIO_ACTIVIDAD) ||
                                       string.IsNullOrEmpty(EntityFederal.PFF_CAPACITACION.CONCLUSIONES) || string.IsNullOrEmpty(EntityFederal.PFF_CAPACITACION.DESCUIDADO_LABORES) ||
                                       string.IsNullOrEmpty(EntityFederal.PFF_CAPACITACION.FONDO_AHORRO) || string.IsNullOrEmpty(EntityFederal.PFF_CAPACITACION.HA_PROGRESADO_OFICIO) ||
                                       string.IsNullOrEmpty(EntityFederal.PFF_CAPACITACION.NOMBRE) || EntityFederal.PFF_CAPACITACION.OFICIO_ANTES_RECLUSION == null ||
                                       string.IsNullOrEmpty(EntityFederal.PFF_CAPACITACION.SATISFACE_ACTIVIDAD) || string.IsNullOrEmpty(EntityFederal.PFF_CAPACITACION.SECCION) ||
                                       EntityFederal.PFF_CAPACITACION.A_TOTAL_DIAS_LABORADOS == null || EntityFederal.PFF_CAPACITACION.B_DIAS_LABORADOS_OTROS_CERESOS == null ||
                                       EntityFederal.PFF_CAPACITACION.FECHA == null || EntityFederal.PFF_CAPACITACION.SALARIO_DEVENGABA_DETENCION == null ||
                                       EntityFederal.PFF_CAPACITACION.TOTAL_A_B == null)
                                        _Estatus = (short)eEstatudDetallePersonalidad.ASIGNADO;
                                    else
                                        _Estatus = (short)eEstatudDetallePersonalidad.TERMINADO;

                                    var _DesarrolloCapacitacionFederal = Context.PERSONALIDAD_DETALLE.FirstOrDefault(x => x.ID_IMPUTADO == EntityFederal.ID_IMPUTADO && x.ID_ESTUDIO == _EstudioPadre.ID_ESTUDIO && x.ID_INGRESO == EntityFederal.ID_INGRESO && x.ID_TIPO == (short)eTiposEstudio.LABORAL_FEDERAL && x.ID_CENTRO == EntityFederal.ID_CENTRO && x.ID_ANIO == EntityFederal.ID_ANIO);
                                    if (_DesarrolloCapacitacionFederal == null)
                                    {
                                        short _ConsecutivoPersonalidadDetalle = GetIDProceso<short>("PERSONALIDAD_DETALLE", "ID_DETALLE", "1=1");
                                        var NvoDetalle = new PERSONALIDAD_DETALLE()
                                        {
                                            ID_ANIO = EntityFederal.ID_ANIO,
                                            ID_CENTRO = EntityFederal.ID_CENTRO,
                                            ID_DETALLE = _ConsecutivoPersonalidadDetalle,
                                            ID_ESTUDIO = _EstudioPadre.ID_ESTUDIO,
                                            ID_IMPUTADO = EntityFederal.ID_IMPUTADO,
                                            ID_INGRESO = EntityFederal.ID_INGRESO,
                                            ID_TIPO = (short)eTiposEstudio.LABORAL_FEDERAL,
                                            INICIO_FEC = GetFechaServerDate(),
                                            RESULTADO = _Estatus == (short)eEstatudDetallePersonalidad.TERMINADO ? "S" : "N",
                                            SOLICITUD_FEC = null,
                                            ID_ESTATUS = _Estatus,
                                            TERMINO_FEC = _Estatus == (short)eEstatudDetallePersonalidad.TERMINADO ? GetFechaServerDate() : new System.DateTime?(),
                                            TIPO_MEDIA = string.Empty
                                        };

                                        Context.PERSONALIDAD_DETALLE.Add(NvoDetalle);
                                    }

                                    else
                                    {
                                        _DesarrolloCapacitacionFederal.RESULTADO = _Estatus.HasValue ? _Estatus.Value == (short)eEstatudDetallePersonalidad.TERMINADO ? "S" : "N" : "N";
                                        _DesarrolloCapacitacionFederal.ID_ESTATUS = _Estatus;
                                        Context.Entry(_DesarrolloCapacitacionFederal).State = System.Data.EntityState.Modified;
                                    }
                                }

                                #endregion
                            }

                            if (EntityFederal.PFF_ACTIVIDAD != null)
                            {
                                var _ACtivFed = Context.PFF_ACTIVIDAD.FirstOrDefault(x => x.ID_INGRESO == EntityFederal.ID_INGRESO && x.ID_IMPUTADO == EntityFederal.ID_IMPUTADO && x.ID_ESTUDIO == EntityFederal.ID_ESTUDIO && x.ID_ANIO == EntityFederal.ID_ANIO && x.ID_CENTRO == EntityFederal.ID_CENTRO && x.ID_ANIO == EntityFederal.ID_ANIO);
                                var _DetalleActivParticipacion = Context.PFF_ACTIVIDAD_PARTICIPACION.Where(x => x.ID_INGRESO == EntityFederal.ID_INGRESO && x.ID_IMPUTADO == EntityFederal.ID_IMPUTADO && x.ID_ESTUDIO == EntityFederal.ID_ESTUDIO && x.ID_ANIO == EntityFederal.ID_ANIO && x.ID_CENTRO == EntityFederal.ID_CENTRO && x.ID_ANIO == EntityFederal.ID_ANIO);
                                if (_ACtivFed == null)
                                {
                                    ValidaEstudio.PFF_ACTIVIDAD = new PFF_ACTIVIDAD()
                                    {
                                        ALFABE_PRIMARIA = EntityFederal.PFF_ACTIVIDAD.ALFABE_PRIMARIA,
                                        ASISTE_PUNTUAL = EntityFederal.PFF_ACTIVIDAD.ASISTE_PUNTUAL,
                                        ASISTE_PUNTUAL_NO_POR_QUE = EntityFederal.PFF_ACTIVIDAD.ASISTE_PUNTUAL_NO_POR_QUE,
                                        AVANCE_RENDIMIENTO_ACADEMINCO = EntityFederal.PFF_ACTIVIDAD.AVANCE_RENDIMIENTO_ACADEMINCO,
                                        BACHILLER_UNI = EntityFederal.PFF_ACTIVIDAD.BACHILLER_UNI,
                                        CONCLUSIONES = EntityFederal.PFF_ACTIVIDAD.CONCLUSIONES,
                                        DIRECTOR_CENTRO = EntityFederal.PFF_ACTIVIDAD.DIRECTOR_CENTRO,
                                        ESCOLARIDAD_MOMENTO = EntityFederal.PFF_ACTIVIDAD.ESCOLARIDAD_MOMENTO.HasValue ? EntityFederal.PFF_ACTIVIDAD.ESCOLARIDAD_MOMENTO.Value != -1 ? EntityFederal.PFF_ACTIVIDAD.ESCOLARIDAD_MOMENTO : new short?() : new short?(),
                                        ESPECIFIQUE = EntityFederal.PFF_ACTIVIDAD.ESPECIFIQUE,
                                        ESTUDIOS_ACTUALES = EntityFederal.PFF_ACTIVIDAD.ESTUDIOS_ACTUALES,
                                        ESTUDIOS_EN_INTERNAMIENTO = EntityFederal.PFF_ACTIVIDAD.ESTUDIOS_EN_INTERNAMIENTO,
                                        FECHA = EntityFederal.PFF_ACTIVIDAD.FECHA,
                                        ID_ANIO = EntityFederal.ID_ANIO,
                                        ID_CENTRO = EntityFederal.ID_CENTRO,
                                        ID_INGRESO = EntityFederal.ID_INGRESO,
                                        ID_ESTUDIO = EntityFederal.ID_ESTUDIO,
                                        ID_IMPUTADO = EntityFederal.ID_IMPUTADO,
                                        IMPARTIDO_ENSENANZA = EntityFederal.PFF_ACTIVIDAD.IMPARTIDO_ENSENANZA,
                                        IMPARTIDO_ENSENANZA_TIEMPO = EntityFederal.PFF_ACTIVIDAD.IMPARTIDO_ENSENANZA_TIEMPO,
                                        IMPARTIDO_ENSENANZA_TIPO = EntityFederal.PFF_ACTIVIDAD.IMPARTIDO_ENSENANZA_TIPO,
                                        JEFE_SECC_EDUCATIVA = EntityFederal.PFF_ACTIVIDAD.JEFE_SECC_EDUCATIVA,
                                        LUGAR = EntityFederal.PFF_ACTIVIDAD.LUGAR,
                                        NOMBRE = EntityFederal.PFF_ACTIVIDAD.NOMBRE,
                                        OTRA_ENSENANZA = EntityFederal.PFF_ACTIVIDAD.OTRA_ENSENANZA,
                                        OTRO = EntityFederal.PFF_ACTIVIDAD.OTRO,
                                        PRIMARIA_SECU = EntityFederal.PFF_ACTIVIDAD.PRIMARIA_SECU,
                                        PROMOVIDO = EntityFederal.PFF_ACTIVIDAD.PROMOVIDO,
                                        OTROS_PROGRAMAS = EntityFederal.PFF_ACTIVIDAD.OTROS_PROGRAMAS,
                                        SECU_BACHILLER = EntityFederal.PFF_ACTIVIDAD.SECU_BACHILLER
                                    };

                                    #region Actividades
                                    if (_DetalleActivParticipacion != null && _DetalleActivParticipacion.Any())
                                        foreach (var item in _DetalleActivParticipacion)
                                            Context.Entry(item).State = System.Data.EntityState.Deleted;

                                    if (EntityFederal.PFF_ACTIVIDAD.PFF_ACTIVIDAD_PARTICIPACION != null && EntityFederal.PFF_ACTIVIDAD.PFF_ACTIVIDAD_PARTICIPACION.Any())
                                        foreach (var item in EntityFederal.PFF_ACTIVIDAD.PFF_ACTIVIDAD_PARTICIPACION)
                                        {
                                            var _Detalle = new PFF_ACTIVIDAD_PARTICIPACION()
                                            {
                                                FECHA_1 = item.FECHA_1,
                                                FECHA_2 = item.FECHA_2,
                                                ID_ANIO = EntityFederal.ID_ANIO,
                                                ID_CENTRO = EntityFederal.ID_CENTRO,
                                                ID_ESTUDIO = EntityFederal.ID_ESTUDIO,
                                                ID_IMPUTADO = EntityFederal.ID_IMPUTADO,
                                                ID_INGRESO = EntityFederal.ID_INGRESO,
                                                ID_TIPO_PROGRAMA = item.ID_TIPO_PROGRAMA,
                                                OTRO_ESPECIFICAR = item.OTRO_ESPECIFICAR,
                                                PARTICIPACION = item.PARTICIPACION
                                            };

                                            ValidaEstudio.PFF_ACTIVIDAD.PFF_ACTIVIDAD_PARTICIPACION.Add(_Detalle);
                                        };

                                    #endregion
                                }

                                else
                                {
                                    _ACtivFed.ALFABE_PRIMARIA = EntityFederal.PFF_ACTIVIDAD.ALFABE_PRIMARIA;
                                    _ACtivFed.ASISTE_PUNTUAL = EntityFederal.PFF_ACTIVIDAD.ASISTE_PUNTUAL;
                                    _ACtivFed.ASISTE_PUNTUAL_NO_POR_QUE = EntityFederal.PFF_ACTIVIDAD.ASISTE_PUNTUAL_NO_POR_QUE;
                                    _ACtivFed.AVANCE_RENDIMIENTO_ACADEMINCO = EntityFederal.PFF_ACTIVIDAD.AVANCE_RENDIMIENTO_ACADEMINCO;
                                    _ACtivFed.BACHILLER_UNI = EntityFederal.PFF_ACTIVIDAD.BACHILLER_UNI;
                                    _ACtivFed.CONCLUSIONES = EntityFederal.PFF_ACTIVIDAD.CONCLUSIONES;
                                    _ACtivFed.DIRECTOR_CENTRO = EntityFederal.PFF_ACTIVIDAD.DIRECTOR_CENTRO;
                                    _ACtivFed.ESCOLARIDAD_MOMENTO = EntityFederal.PFF_ACTIVIDAD.ESCOLARIDAD_MOMENTO.HasValue ? EntityFederal.PFF_ACTIVIDAD.ESCOLARIDAD_MOMENTO.Value != -1 ? EntityFederal.PFF_ACTIVIDAD.ESCOLARIDAD_MOMENTO : new short?() : new short?();
                                    _ACtivFed.ESPECIFIQUE = EntityFederal.PFF_ACTIVIDAD.ESPECIFIQUE;
                                    _ACtivFed.ESTUDIOS_ACTUALES = EntityFederal.PFF_ACTIVIDAD.ESTUDIOS_ACTUALES;
                                    _ACtivFed.ESTUDIOS_EN_INTERNAMIENTO = EntityFederal.PFF_ACTIVIDAD.ESTUDIOS_EN_INTERNAMIENTO;
                                    _ACtivFed.FECHA = EntityFederal.PFF_ACTIVIDAD.FECHA;
                                    _ACtivFed.ID_ANIO = EntityFederal.ID_ANIO;
                                    _ACtivFed.ID_CENTRO = EntityFederal.ID_CENTRO;
                                    _ACtivFed.ID_ESTUDIO = EntityFederal.ID_ESTUDIO;
                                    _ACtivFed.ID_IMPUTADO = EntityFederal.ID_IMPUTADO;
                                    _ACtivFed.ID_INGRESO = EntityFederal.ID_INGRESO;
                                    _ACtivFed.IMPARTIDO_ENSENANZA = EntityFederal.PFF_ACTIVIDAD.IMPARTIDO_ENSENANZA;
                                    _ACtivFed.IMPARTIDO_ENSENANZA_TIEMPO = EntityFederal.PFF_ACTIVIDAD.IMPARTIDO_ENSENANZA_TIEMPO;
                                    _ACtivFed.IMPARTIDO_ENSENANZA_TIPO = EntityFederal.PFF_ACTIVIDAD.IMPARTIDO_ENSENANZA_TIPO;
                                    _ACtivFed.JEFE_SECC_EDUCATIVA = EntityFederal.PFF_ACTIVIDAD.JEFE_SECC_EDUCATIVA;
                                    _ACtivFed.LUGAR = EntityFederal.PFF_ACTIVIDAD.LUGAR;
                                    _ACtivFed.NOMBRE = EntityFederal.PFF_ACTIVIDAD.NOMBRE;
                                    _ACtivFed.OTRA_ENSENANZA = EntityFederal.PFF_ACTIVIDAD.OTRA_ENSENANZA;
                                    _ACtivFed.OTRO = EntityFederal.PFF_ACTIVIDAD.OTRO;
                                    _ACtivFed.PRIMARIA_SECU = EntityFederal.PFF_ACTIVIDAD.PRIMARIA_SECU;
                                    _ACtivFed.PROMOVIDO = EntityFederal.PFF_ACTIVIDAD.PROMOVIDO;
                                    _ACtivFed.SECU_BACHILLER = EntityFederal.PFF_ACTIVIDAD.SECU_BACHILLER;
                                    _ACtivFed.OTROS_PROGRAMAS = EntityFederal.PFF_ACTIVIDAD.OTROS_PROGRAMAS;
                                    Context.Entry(_ACtivFed).State = System.Data.EntityState.Modified;

                                    #region Activ
                                    if (_DetalleActivParticipacion != null && _DetalleActivParticipacion.Any())
                                        foreach (var item in _DetalleActivParticipacion)
                                            Context.Entry(item).State = System.Data.EntityState.Deleted;

                                    if (EntityFederal.PFF_ACTIVIDAD.PFF_ACTIVIDAD_PARTICIPACION != null && EntityFederal.PFF_ACTIVIDAD.PFF_ACTIVIDAD_PARTICIPACION.Any())
                                        foreach (var item in EntityFederal.PFF_ACTIVIDAD.PFF_ACTIVIDAD_PARTICIPACION)
                                        {
                                            var _Detalle = new PFF_ACTIVIDAD_PARTICIPACION()
                                            {
                                                FECHA_1 = item.FECHA_1,
                                                FECHA_2 = item.FECHA_2,
                                                ID_ANIO = EntityFederal.ID_ANIO,
                                                ID_CENTRO = EntityFederal.ID_CENTRO,
                                                ID_ESTUDIO = EntityFederal.ID_ESTUDIO,
                                                ID_IMPUTADO = EntityFederal.ID_IMPUTADO,
                                                ID_INGRESO = EntityFederal.ID_INGRESO,
                                                ID_TIPO_PROGRAMA = item.ID_TIPO_PROGRAMA,
                                                OTRO_ESPECIFICAR = item.OTRO_ESPECIFICAR,
                                                PARTICIPACION = item.PARTICIPACION
                                            };

                                            ValidaEstudio.PFF_ACTIVIDAD.PFF_ACTIVIDAD_PARTICIPACION.Add(_Detalle);
                                        };
                                    #endregion
                                };


                                #region Detalle
                                if (_EstudioPadre != null)
                                {
                                    short? _Estatus = new short?();
                                    if (string.IsNullOrEmpty(EntityFederal.PFF_ACTIVIDAD.ASISTE_PUNTUAL) || string.IsNullOrEmpty(EntityFederal.PFF_ACTIVIDAD.AVANCE_RENDIMIENTO_ACADEMINCO) || EntityFederal.PFF_ACTIVIDAD.ESCOLARIDAD_MOMENTO == null ||
                                        string.IsNullOrEmpty(EntityFederal.PFF_ACTIVIDAD.ESTUDIOS_ACTUALES) || string.IsNullOrEmpty(EntityFederal.PFF_ACTIVIDAD.ESTUDIOS_EN_INTERNAMIENTO) ||
                                        string.IsNullOrEmpty(EntityFederal.PFF_ACTIVIDAD.IMPARTIDO_ENSENANZA) || string.IsNullOrEmpty(EntityFederal.PFF_ACTIVIDAD.NOMBRE) || string.IsNullOrEmpty(EntityFederal.PFF_ACTIVIDAD.PROMOVIDO)
                                        || EntityFederal.PFF_ACTIVIDAD.FECHA == null)
                                        _Estatus = (short)eEstatudDetallePersonalidad.ASIGNADO;
                                    else
                                        _Estatus = (short)eEstatudDetallePersonalidad.TERMINADO;

                                    var DesarrolloTrabajoFederal = Context.PERSONALIDAD_DETALLE.FirstOrDefault(x => x.ID_IMPUTADO == EntityFederal.ID_IMPUTADO && x.ID_ESTUDIO == _EstudioPadre.ID_ESTUDIO && x.ID_INGRESO == EntityFederal.ID_INGRESO && x.ID_TIPO == (short)eTiposEstudio.PEDAGOGICA_FEDERAL && x.ID_CENTRO == EntityFederal.ID_CENTRO && x.ID_ANIO == EntityFederal.ID_ANIO);
                                    if (DesarrolloTrabajoFederal == null)
                                    {
                                        short _ConsecutivoPersonalidadDetalle = GetIDProceso<short>("PERSONALIDAD_DETALLE", "ID_DETALLE", "1=1");
                                        var NvoDetalle = new PERSONALIDAD_DETALLE()
                                        {
                                            ID_ANIO = EntityFederal.ID_ANIO,
                                            ID_CENTRO = EntityFederal.ID_CENTRO,
                                            ID_DETALLE = _ConsecutivoPersonalidadDetalle,
                                            ID_ESTUDIO = _EstudioPadre.ID_ESTUDIO,
                                            ID_IMPUTADO = EntityFederal.ID_IMPUTADO,
                                            ID_INGRESO = EntityFederal.ID_INGRESO,
                                            ID_TIPO = (short)eTiposEstudio.PEDAGOGICA_FEDERAL,
                                            INICIO_FEC = GetFechaServerDate(),
                                            RESULTADO = _Estatus == (short)eEstatudDetallePersonalidad.TERMINADO ? "S" : "N",
                                            SOLICITUD_FEC = null,
                                            ID_ESTATUS = _Estatus,
                                            TERMINO_FEC = _Estatus == (short)eEstatudDetallePersonalidad.TERMINADO ? GetFechaServerDate() : new System.DateTime?(),
                                            TIPO_MEDIA = string.Empty
                                        };

                                        Context.PERSONALIDAD_DETALLE.Add(NvoDetalle);
                                    }

                                    else
                                    {
                                        DesarrolloTrabajoFederal.RESULTADO = _Estatus.HasValue ? _Estatus.Value == (short)eEstatudDetallePersonalidad.TERMINADO ? "S" : "N" : "N";
                                        DesarrolloTrabajoFederal.ID_ESTATUS = _Estatus;
                                        Context.Entry(DesarrolloTrabajoFederal).State = System.Data.EntityState.Modified;
                                    }
                                };

                                #endregion
                            }

                            if (EntityFederal.PFF_VIGILANCIA != null)
                            {
                                var _InformeVigilanciaFF = Context.PFF_VIGILANCIA.FirstOrDefault(x => x.ID_INGRESO == EntityFederal.ID_INGRESO && x.ID_IMPUTADO == EntityFederal.ID_IMPUTADO && x.ID_ESTUDIO == EntityFederal.ID_ESTUDIO && x.ID_ANIO == EntityFederal.ID_ANIO && x.ID_CENTRO == EntityFederal.ID_CENTRO);
                                var _DetalleSentencia = Context.PFF_CORRECTIVO.Where(x => x.ID_INGRESO == EntityFederal.ID_INGRESO && x.ID_IMPUTADO == EntityFederal.ID_IMPUTADO && x.ID_ESTUDIO == EntityFederal.ID_ESTUDIO && x.ID_ANIO == EntityFederal.ID_ANIO && x.ID_CENTRO == EntityFederal.ID_CENTRO);
                                if (_InformeVigilanciaFF == null)
                                {
                                    ValidaEstudio.PFF_VIGILANCIA = new PFF_VIGILANCIA()
                                    {
                                        CENTRO_DONDE_PROCEDE = EntityFederal.PFF_VIGILANCIA.CENTRO_DONDE_PROCEDE,
                                        CONCLUSIONES = EntityFederal.PFF_VIGILANCIA.CONCLUSIONES,
                                        CONDUCTA = EntityFederal.PFF_VIGILANCIA.CONDUCTA,
                                        CONDUCTA_FAMILIA = EntityFederal.PFF_VIGILANCIA.CONDUCTA_FAMILIA,
                                        CONDUCTA_GENERAL = EntityFederal.PFF_VIGILANCIA.CONDUCTA_GENERAL,
                                        CONDUCTA_SUPERIORES = EntityFederal.PFF_VIGILANCIA.CONDUCTA_SUPERIORES,
                                        DESCRIPCION_CONDUCTA = EntityFederal.PFF_VIGILANCIA.DESCRIPCION_CONDUCTA,
                                        DIRECTOR_CENTRO = EntityFederal.PFF_VIGILANCIA.DIRECTOR_CENTRO,
                                        ESTIMULOS_BUENA_CONDUCTA = EntityFederal.PFF_VIGILANCIA.ESTIMULOS_BUENA_CONDUCTA,
                                        FECHA = EntityFederal.PFF_VIGILANCIA.FECHA,
                                        FECHA_INGRESO = EntityFederal.PFF_VIGILANCIA.FECHA_INGRESO,
                                        HIGIENE_CELDA = EntityFederal.PFF_VIGILANCIA.HIGIENE_CELDA,
                                        HIGIENE_PERSONAL = EntityFederal.PFF_VIGILANCIA.HIGIENE_PERSONAL,
                                        ID_ANIO = EntityFederal.ID_ANIO,
                                        ID_CENTRO = EntityFederal.ID_CENTRO,
                                        ID_ESTUDIO = EntityFederal.ID_ESTUDIO,
                                        ID_IMPUTADO = EntityFederal.ID_IMPUTADO,
                                        ID_INGRESO = EntityFederal.ID_INGRESO,
                                        JEFE_VIGILANCIA = EntityFederal.PFF_VIGILANCIA.JEFE_VIGILANCIA,
                                        LUGAR = EntityFederal.PFF_VIGILANCIA.LUGAR,
                                        MOTIVO_TRASLADO = EntityFederal.PFF_VIGILANCIA.MOTIVO_TRASLADO,
                                        NOMBRE = EntityFederal.PFF_VIGILANCIA.NOMBRE,
                                        RELACION_COMPANEROS = EntityFederal.PFF_VIGILANCIA.RELACION_COMPANEROS,
                                        VISITA_FRECUENCIA = EntityFederal.PFF_VIGILANCIA.VISITA_FRECUENCIA,
                                        VISITA_QUIENES = EntityFederal.PFF_VIGILANCIA.VISITA_QUIENES,
                                        VISITA_RECIBE = EntityFederal.PFF_VIGILANCIA.VISITA_RECIBE
                                    };

                                    #region Sentencias
                                    if (_DetalleSentencia != null && _DetalleSentencia.Any())
                                        foreach (var item in _DetalleSentencia)
                                            Context.Entry(item).State = System.Data.EntityState.Deleted;

                                    var _consecutivoCapacitacionFederal = GetIDProceso<short>("PFF_CORRECTIVO", "ID_CONSEC", string.Format("ID_CENTRO = {0} AND ID_ANIO = {1} AND ID_IMPUTADO = {2} AND ID_INGRESO = {3} AND ID_ESTUDIO = {4}", EntityFederal.ID_CENTRO, EntityFederal.ID_ANIO, EntityFederal.ID_IMPUTADO, EntityFederal.ID_INGRESO, EntityFederal.ID_ESTUDIO));

                                    if (EntityFederal.PFF_VIGILANCIA.PFF_CORRECTIVO != null && EntityFederal.PFF_VIGILANCIA.PFF_CORRECTIVO.Any())
                                        foreach (var item in EntityFederal.PFF_VIGILANCIA.PFF_CORRECTIVO)
                                        {
                                            var NvaSancion = new PFF_CORRECTIVO()
                                            {
                                                FECHA = item.FECHA,
                                                ID_ANIO = EntityFederal.ID_ANIO,
                                                ID_IMPUTADO = EntityFederal.ID_IMPUTADO,
                                                ID_CONSEC = _consecutivoCapacitacionFederal,
                                                ID_ESTUDIO = EntityFederal.ID_ESTUDIO,
                                                ID_INGRESO = EntityFederal.ID_INGRESO,
                                                RESOLUCION = item.RESOLUCION,
                                                ID_CENTRO = EntityFederal.ID_CENTRO,
                                                MOTIVO = item.MOTIVO
                                            };

                                            ValidaEstudio.PFF_VIGILANCIA.PFF_CORRECTIVO.Add(NvaSancion);
                                            _consecutivoCapacitacionFederal++;
                                        };
                                    #endregion
                                }
                                else
                                {
                                    _InformeVigilanciaFF.CENTRO_DONDE_PROCEDE = EntityFederal.PFF_VIGILANCIA.CENTRO_DONDE_PROCEDE;
                                    _InformeVigilanciaFF.CONCLUSIONES = EntityFederal.PFF_VIGILANCIA.CONCLUSIONES;
                                    _InformeVigilanciaFF.CONDUCTA = EntityFederal.PFF_VIGILANCIA.CONDUCTA;
                                    _InformeVigilanciaFF.CONDUCTA_FAMILIA = EntityFederal.PFF_VIGILANCIA.CONDUCTA_FAMILIA;
                                    _InformeVigilanciaFF.CONDUCTA_GENERAL = EntityFederal.PFF_VIGILANCIA.CONDUCTA_GENERAL;
                                    _InformeVigilanciaFF.CONDUCTA_SUPERIORES = EntityFederal.PFF_VIGILANCIA.CONDUCTA_SUPERIORES;
                                    _InformeVigilanciaFF.DESCRIPCION_CONDUCTA = EntityFederal.PFF_VIGILANCIA.DESCRIPCION_CONDUCTA;
                                    _InformeVigilanciaFF.DIRECTOR_CENTRO = EntityFederal.PFF_VIGILANCIA.DIRECTOR_CENTRO;
                                    _InformeVigilanciaFF.ESTIMULOS_BUENA_CONDUCTA = EntityFederal.PFF_VIGILANCIA.ESTIMULOS_BUENA_CONDUCTA;
                                    _InformeVigilanciaFF.FECHA = EntityFederal.PFF_VIGILANCIA.FECHA;
                                    _InformeVigilanciaFF.FECHA_INGRESO = EntityFederal.PFF_VIGILANCIA.FECHA_INGRESO;
                                    _InformeVigilanciaFF.HIGIENE_CELDA = EntityFederal.PFF_VIGILANCIA.HIGIENE_CELDA;
                                    _InformeVigilanciaFF.HIGIENE_PERSONAL = EntityFederal.PFF_VIGILANCIA.HIGIENE_PERSONAL;
                                    _InformeVigilanciaFF.ID_ANIO = EntityFederal.ID_ANIO;
                                    _InformeVigilanciaFF.ID_CENTRO = EntityFederal.ID_CENTRO;
                                    _InformeVigilanciaFF.ID_ESTUDIO = EntityFederal.ID_ESTUDIO;
                                    _InformeVigilanciaFF.ID_IMPUTADO = EntityFederal.ID_IMPUTADO;
                                    _InformeVigilanciaFF.ID_INGRESO = EntityFederal.ID_INGRESO;
                                    _InformeVigilanciaFF.JEFE_VIGILANCIA = EntityFederal.PFF_VIGILANCIA.JEFE_VIGILANCIA;
                                    _InformeVigilanciaFF.LUGAR = EntityFederal.PFF_VIGILANCIA.LUGAR;
                                    _InformeVigilanciaFF.MOTIVO_TRASLADO = EntityFederal.PFF_VIGILANCIA.MOTIVO_TRASLADO;
                                    _InformeVigilanciaFF.NOMBRE = EntityFederal.PFF_VIGILANCIA.NOMBRE;
                                    _InformeVigilanciaFF.RELACION_COMPANEROS = EntityFederal.PFF_VIGILANCIA.RELACION_COMPANEROS;
                                    _InformeVigilanciaFF.VISITA_FRECUENCIA = EntityFederal.PFF_VIGILANCIA.VISITA_FRECUENCIA;
                                    _InformeVigilanciaFF.VISITA_QUIENES = EntityFederal.PFF_VIGILANCIA.VISITA_QUIENES;
                                    _InformeVigilanciaFF.VISITA_RECIBE = EntityFederal.PFF_VIGILANCIA.VISITA_RECIBE;
                                    Context.Entry(_InformeVigilanciaFF).State = System.Data.EntityState.Modified;

                                    #region Sentencia
                                    if (_DetalleSentencia != null && _DetalleSentencia.Any())
                                        foreach (var item in _DetalleSentencia)
                                            Context.Entry(item).State = System.Data.EntityState.Deleted;

                                    var _consecutivoCapacitacionFederal = GetIDProceso<short>("PFF_CORRECTIVO", "ID_CONSEC", string.Format("ID_CENTRO = {0} AND ID_ANIO = {1} AND ID_IMPUTADO = {2} AND ID_INGRESO = {3} AND ID_ESTUDIO = {4}", EntityFederal.ID_CENTRO, EntityFederal.ID_ANIO, EntityFederal.ID_IMPUTADO, EntityFederal.ID_INGRESO, EntityFederal.ID_ESTUDIO));
                                    if (EntityFederal.PFF_VIGILANCIA.PFF_CORRECTIVO != null && EntityFederal.PFF_VIGILANCIA.PFF_CORRECTIVO.Any())
                                        foreach (var item in EntityFederal.PFF_VIGILANCIA.PFF_CORRECTIVO)
                                        {
                                            var NvaSancion = new PFF_CORRECTIVO()
                                            {
                                                FECHA = item.FECHA,
                                                ID_ANIO = EntityFederal.ID_ANIO,
                                                ID_CENTRO = EntityFederal.ID_CENTRO,
                                                ID_ESTUDIO = EntityFederal.ID_ESTUDIO,
                                                ID_IMPUTADO = EntityFederal.ID_IMPUTADO,
                                                ID_INGRESO = EntityFederal.ID_INGRESO,
                                                MOTIVO = item.MOTIVO,
                                                RESOLUCION = item.RESOLUCION,
                                                ID_CONSEC = _consecutivoCapacitacionFederal
                                            };

                                            ValidaEstudio.PFF_VIGILANCIA.PFF_CORRECTIVO.Add(NvaSancion);
                                            _consecutivoCapacitacionFederal++;
                                        };

                                    #endregion
                                };


                                if (_EstudioPadre != null)
                                {
                                    short? _Estatus = new short?();
                                    if (string.IsNullOrEmpty(EntityFederal.PFF_VIGILANCIA.CENTRO_DONDE_PROCEDE) || string.IsNullOrEmpty(EntityFederal.PFF_VIGILANCIA.CONCLUSIONES) ||
                                        string.IsNullOrEmpty(EntityFederal.PFF_VIGILANCIA.CONDUCTA) || string.IsNullOrEmpty(EntityFederal.PFF_VIGILANCIA.CONDUCTA_FAMILIA) ||
                                        string.IsNullOrEmpty(EntityFederal.PFF_VIGILANCIA.CONDUCTA_GENERAL) || string.IsNullOrEmpty(EntityFederal.PFF_VIGILANCIA.CONDUCTA_SUPERIORES) ||
                                        string.IsNullOrEmpty(EntityFederal.PFF_VIGILANCIA.DESCRIPCION_CONDUCTA) || string.IsNullOrEmpty(EntityFederal.PFF_VIGILANCIA.ESTIMULOS_BUENA_CONDUCTA) ||
                                        string.IsNullOrEmpty(EntityFederal.PFF_VIGILANCIA.HIGIENE_CELDA) || string.IsNullOrEmpty(EntityFederal.PFF_VIGILANCIA.HIGIENE_PERSONAL) ||
                                        string.IsNullOrEmpty(EntityFederal.PFF_VIGILANCIA.MOTIVO_TRASLADO) || string.IsNullOrEmpty(EntityFederal.PFF_VIGILANCIA.NOMBRE) ||
                                        string.IsNullOrEmpty(EntityFederal.PFF_VIGILANCIA.RELACION_COMPANEROS) || EntityFederal.PFF_VIGILANCIA.FECHA == null || EntityFederal.PFF_VIGILANCIA.FECHA_INGRESO == null ||
                                        string.IsNullOrEmpty(EntityFederal.PFF_VIGILANCIA.VISITA_RECIBE))
                                        _Estatus = (short)eEstatudDetallePersonalidad.ASIGNADO;
                                    else
                                        _Estatus = (short)eEstatudDetallePersonalidad.TERMINADO;

                                    var DesarrolloVigilanciaFederal = Context.PERSONALIDAD_DETALLE.FirstOrDefault(x => x.ID_IMPUTADO == EntityFederal.ID_IMPUTADO && x.ID_ESTUDIO == _EstudioPadre.ID_ESTUDIO && x.ID_INGRESO == EntityFederal.ID_INGRESO && x.ID_TIPO == (short)eTiposEstudio.SEGURIDAD_FEDERAL && x.ID_CENTRO == EntityFederal.ID_CENTRO && x.ID_ANIO == EntityFederal.ID_ANIO);
                                    if (DesarrolloVigilanciaFederal == null)
                                    {
                                        short _ConsecutivoPersonalidadDetalle = GetIDProceso<short>("PERSONALIDAD_DETALLE", "ID_DETALLE", "1=1");
                                        var NvoDetalle = new PERSONALIDAD_DETALLE()
                                        {
                                            ID_ANIO = EntityFederal.ID_ANIO,
                                            ID_CENTRO = EntityFederal.ID_CENTRO,
                                            ID_DETALLE = GetSequence<short>("PERSONALIDAD_DETALLE_SEQ"),
                                            ID_ESTUDIO = _EstudioPadre.ID_ESTUDIO,
                                            ID_IMPUTADO = EntityFederal.ID_IMPUTADO,
                                            ID_INGRESO = EntityFederal.ID_INGRESO,
                                            ID_TIPO = (short)eTiposEstudio.SEGURIDAD_FEDERAL,
                                            INICIO_FEC = GetFechaServerDate(),
                                            RESULTADO = _Estatus.HasValue ? _Estatus.Value == (short)eEstatudDetallePersonalidad.TERMINADO ? "S" : "N" : "N",
                                            SOLICITUD_FEC = null,
                                            TERMINO_FEC = _Estatus.HasValue ? _Estatus.Value == (short)eEstatudDetallePersonalidad.TERMINADO ? GetFechaServerDate() : new System.DateTime?() : new System.DateTime?(),
                                            TIPO_MEDIA = string.Empty
                                        };

                                        Context.PERSONALIDAD_DETALLE.Add(NvoDetalle);
                                    }

                                    else
                                    {
                                        DesarrolloVigilanciaFederal.RESULTADO = _Estatus.HasValue ? _Estatus.Value == (short)eEstatudDetallePersonalidad.TERMINADO ? "S" : "N" : "N";
                                        DesarrolloVigilanciaFederal.ID_ESTATUS = _Estatus;
                                        Context.Entry(DesarrolloVigilanciaFederal).State = System.Data.EntityState.Modified;
                                    }
                                };
                            }

                            if (EntityFederal.PFF_CRIMINOLOGICO != null)
                            {
                                var _EstudioCriminodFueroFederal = Context.PFF_CRIMINOLOGICO.FirstOrDefault(x => x.ID_INGRESO == EntityFederal.ID_INGRESO && x.ID_IMPUTADO == EntityFederal.ID_IMPUTADO && x.ID_ESTUDIO == EntityFederal.ID_ESTUDIO && x.ID_ANIO == EntityFederal.ID_ANIO && x.ID_CENTRO == EntityFederal.ID_CENTRO);
                                if (_EstudioCriminodFueroFederal == null)
                                {
                                    ValidaEstudio.PFF_CRIMINOLOGICO = new PFF_CRIMINOLOGICO()
                                    {
                                        ANTECEDENTES_PARA_ANTI_SOCIALE = EntityFederal.PFF_CRIMINOLOGICO.ANTECEDENTES_PARA_ANTI_SOCIALE,
                                        CRIMINOLOGO = EntityFederal.PFF_CRIMINOLOGICO.CRIMINOLOGO,
                                        DIRECTOR_CENTRO = EntityFederal.PFF_CRIMINOLOGICO.DIRECTOR_CENTRO,
                                        FECHA = EntityFederal.PFF_CRIMINOLOGICO.FECHA,
                                        ID_ANIO = EntityFederal.ID_ANIO,
                                        ID_CENTRO = EntityFederal.ID_CENTRO,
                                        ID_ESTUDIO = EntityFederal.ID_ESTUDIO,
                                        ID_IMPUTADO = EntityFederal.ID_IMPUTADO,
                                        ID_INGRESO = EntityFederal.ID_INGRESO,
                                        LUGAR = EntityFederal.PFF_CRIMINOLOGICO.LUGAR,
                                        NOMBRE = EntityFederal.PFF_CRIMINOLOGICO.NOMBRE,
                                        P1_VERSION_INTERNO = EntityFederal.PFF_CRIMINOLOGICO.P1_VERSION_INTERNO,
                                        P10_CONTINUAR_NO_ESPECIFICAR = EntityFederal.PFF_CRIMINOLOGICO.P10_CONTINUAR_NO_ESPECIFICAR,
                                        P10_CONTINUAR_SI_ESPECIFICAR = EntityFederal.PFF_CRIMINOLOGICO.P10_CONTINUAR_SI_ESPECIFICAR,
                                        P10_CONTINUAR_TRATAMIENTO = EntityFederal.PFF_CRIMINOLOGICO.P10_CONTINUAR_TRATAMIENTO,
                                        P10_OPINION = EntityFederal.PFF_CRIMINOLOGICO.P10_OPINION,
                                        P2_PERSONALIDAD = EntityFederal.PFF_CRIMINOLOGICO.P2_PERSONALIDAD,
                                        P3_VALORACION = EntityFederal.PFF_CRIMINOLOGICO.P3_VALORACION,
                                        P5_ESPECIFICO = EntityFederal.PFF_CRIMINOLOGICO.P5_ESPECIFICO,
                                        P5_GENERICO = EntityFederal.PFF_CRIMINOLOGICO.P5_GENERICO,
                                        P5_HABITUAL = EntityFederal.PFF_CRIMINOLOGICO.P5_HABITUAL,
                                        P5_PRIMODELINCUENTE = EntityFederal.PFF_CRIMINOLOGICO.P5_PRIMODELINCUENTE,
                                        P5_PROFESIONAL = EntityFederal.PFF_CRIMINOLOGICO.P5_PROFESIONAL,
                                        P6_CRIMINOGENESIS = EntityFederal.PFF_CRIMINOLOGICO.P6_CRIMINOGENESIS,
                                        P7_AGRESIVIDAD = EntityFederal.PFF_CRIMINOLOGICO.P7_AGRESIVIDAD,
                                        P7_EGOCENTRISMO = EntityFederal.PFF_CRIMINOLOGICO.P7_EGOCENTRISMO,
                                        P7_INDIFERENCIA = EntityFederal.PFF_CRIMINOLOGICO.P7_INDIFERENCIA,
                                        P7_LABILIDAD = EntityFederal.PFF_CRIMINOLOGICO.P7_LABILIDAD,
                                        P8_ESTADO_PELIGRO = EntityFederal.PFF_CRIMINOLOGICO.P8_ESTADO_PELIGRO,
                                        P8_RESULTADO_TRATAMIENTO = EntityFederal.PFF_CRIMINOLOGICO.P8_RESULTADO_TRATAMIENTO,
                                        P9_PRONOSTICO = EntityFederal.PFF_CRIMINOLOGICO.P9_PRONOSTICO,
                                        SOBRENOMBRE = EntityFederal.PFF_CRIMINOLOGICO.SOBRENOMBRE
                                    };
                                }

                                else
                                {
                                    _EstudioCriminodFueroFederal.ANTECEDENTES_PARA_ANTI_SOCIALE = EntityFederal.PFF_CRIMINOLOGICO.ANTECEDENTES_PARA_ANTI_SOCIALE;
                                    _EstudioCriminodFueroFederal.CRIMINOLOGO = EntityFederal.PFF_CRIMINOLOGICO.CRIMINOLOGO;
                                    _EstudioCriminodFueroFederal.DIRECTOR_CENTRO = EntityFederal.PFF_CRIMINOLOGICO.DIRECTOR_CENTRO;
                                    _EstudioCriminodFueroFederal.FECHA = EntityFederal.PFF_CRIMINOLOGICO.FECHA;
                                    _EstudioCriminodFueroFederal.ID_ANIO = EntityFederal.ID_ANIO;
                                    _EstudioCriminodFueroFederal.ID_CENTRO = EntityFederal.ID_CENTRO;
                                    _EstudioCriminodFueroFederal.ID_ESTUDIO = EntityFederal.ID_ESTUDIO;
                                    _EstudioCriminodFueroFederal.ID_IMPUTADO = EntityFederal.ID_IMPUTADO;
                                    _EstudioCriminodFueroFederal.ID_INGRESO = EntityFederal.ID_INGRESO;
                                    _EstudioCriminodFueroFederal.LUGAR = EntityFederal.PFF_CRIMINOLOGICO.LUGAR;
                                    _EstudioCriminodFueroFederal.NOMBRE = EntityFederal.PFF_CRIMINOLOGICO.NOMBRE;
                                    _EstudioCriminodFueroFederal.P1_VERSION_INTERNO = EntityFederal.PFF_CRIMINOLOGICO.P1_VERSION_INTERNO;
                                    _EstudioCriminodFueroFederal.P10_CONTINUAR_NO_ESPECIFICAR = EntityFederal.PFF_CRIMINOLOGICO.P10_CONTINUAR_NO_ESPECIFICAR;
                                    _EstudioCriminodFueroFederal.P10_CONTINUAR_SI_ESPECIFICAR = EntityFederal.PFF_CRIMINOLOGICO.P10_CONTINUAR_SI_ESPECIFICAR;
                                    _EstudioCriminodFueroFederal.P10_CONTINUAR_TRATAMIENTO = EntityFederal.PFF_CRIMINOLOGICO.P10_CONTINUAR_TRATAMIENTO;
                                    _EstudioCriminodFueroFederal.P10_OPINION = EntityFederal.PFF_CRIMINOLOGICO.P10_OPINION;
                                    _EstudioCriminodFueroFederal.P2_PERSONALIDAD = EntityFederal.PFF_CRIMINOLOGICO.P2_PERSONALIDAD;
                                    _EstudioCriminodFueroFederal.P3_VALORACION = EntityFederal.PFF_CRIMINOLOGICO.P3_VALORACION;
                                    _EstudioCriminodFueroFederal.P5_ESPECIFICO = EntityFederal.PFF_CRIMINOLOGICO.P5_ESPECIFICO;
                                    _EstudioCriminodFueroFederal.P5_GENERICO = EntityFederal.PFF_CRIMINOLOGICO.P5_GENERICO;
                                    _EstudioCriminodFueroFederal.P5_HABITUAL = EntityFederal.PFF_CRIMINOLOGICO.P5_HABITUAL;
                                    _EstudioCriminodFueroFederal.P5_PRIMODELINCUENTE = EntityFederal.PFF_CRIMINOLOGICO.P5_PRIMODELINCUENTE;
                                    _EstudioCriminodFueroFederal.P5_PROFESIONAL = EntityFederal.PFF_CRIMINOLOGICO.P5_PROFESIONAL;
                                    _EstudioCriminodFueroFederal.P6_CRIMINOGENESIS = EntityFederal.PFF_CRIMINOLOGICO.P6_CRIMINOGENESIS;
                                    _EstudioCriminodFueroFederal.P7_AGRESIVIDAD = EntityFederal.PFF_CRIMINOLOGICO.P7_AGRESIVIDAD;
                                    _EstudioCriminodFueroFederal.P7_EGOCENTRISMO = EntityFederal.PFF_CRIMINOLOGICO.P7_EGOCENTRISMO;
                                    _EstudioCriminodFueroFederal.P7_INDIFERENCIA = EntityFederal.PFF_CRIMINOLOGICO.P7_INDIFERENCIA;
                                    _EstudioCriminodFueroFederal.P7_LABILIDAD = EntityFederal.PFF_CRIMINOLOGICO.P7_LABILIDAD;
                                    _EstudioCriminodFueroFederal.P8_ESTADO_PELIGRO = EntityFederal.PFF_CRIMINOLOGICO.P8_ESTADO_PELIGRO;
                                    _EstudioCriminodFueroFederal.P8_RESULTADO_TRATAMIENTO = EntityFederal.PFF_CRIMINOLOGICO.P8_RESULTADO_TRATAMIENTO;
                                    _EstudioCriminodFueroFederal.P9_PRONOSTICO = EntityFederal.PFF_CRIMINOLOGICO.P9_PRONOSTICO;
                                    _EstudioCriminodFueroFederal.SOBRENOMBRE = EntityFederal.PFF_CRIMINOLOGICO.SOBRENOMBRE;
                                    Context.Entry(_EstudioCriminodFueroFederal).State = System.Data.EntityState.Modified;
                                };


                                if (_EstudioPadre != null)
                                {
                                    short? _Estatus = new short?();
                                    if (string.IsNullOrEmpty(EntityFederal.PFF_CRIMINOLOGICO.ANTECEDENTES_PARA_ANTI_SOCIALE) || string.IsNullOrEmpty(EntityFederal.PFF_CRIMINOLOGICO.NOMBRE) ||
                                        string.IsNullOrEmpty(EntityFederal.PFF_CRIMINOLOGICO.P1_VERSION_INTERNO) || string.IsNullOrEmpty(EntityFederal.PFF_CRIMINOLOGICO.P10_CONTINUAR_TRATAMIENTO) ||
                                        string.IsNullOrEmpty(EntityFederal.PFF_CRIMINOLOGICO.P10_OPINION) || string.IsNullOrEmpty(EntityFederal.PFF_CRIMINOLOGICO.P2_PERSONALIDAD) ||
                                        string.IsNullOrEmpty(EntityFederal.PFF_CRIMINOLOGICO.P3_VALORACION) || string.IsNullOrEmpty(EntityFederal.PFF_CRIMINOLOGICO.P6_CRIMINOGENESIS) ||
                                        string.IsNullOrEmpty(EntityFederal.PFF_CRIMINOLOGICO.P7_AGRESIVIDAD) || string.IsNullOrEmpty(EntityFederal.PFF_CRIMINOLOGICO.P7_EGOCENTRISMO) ||
                                        string.IsNullOrEmpty(EntityFederal.PFF_CRIMINOLOGICO.P7_INDIFERENCIA) || string.IsNullOrEmpty(EntityFederal.PFF_CRIMINOLOGICO.P7_LABILIDAD) ||
                                        string.IsNullOrEmpty(EntityFederal.PFF_CRIMINOLOGICO.P8_ESTADO_PELIGRO) ||
                                        string.IsNullOrEmpty(EntityFederal.PFF_CRIMINOLOGICO.P8_RESULTADO_TRATAMIENTO) || string.IsNullOrEmpty(EntityFederal.PFF_CRIMINOLOGICO.P9_PRONOSTICO) ||
                                        string.IsNullOrEmpty(EntityFederal.PFF_CRIMINOLOGICO.SOBRENOMBRE) || EntityFederal.PFF_CRIMINOLOGICO.FECHA == null)
                                        _Estatus = (short)eEstatudDetallePersonalidad.ASIGNADO;
                                    else
                                        _Estatus = (short)eEstatudDetallePersonalidad.TERMINADO;

                                    var DesarrolloCriminoFederal = Context.PERSONALIDAD_DETALLE.FirstOrDefault(x => x.ID_IMPUTADO == EntityFederal.ID_IMPUTADO && x.ID_ESTUDIO == _EstudioPadre.ID_ESTUDIO && x.ID_INGRESO == EntityFederal.ID_INGRESO && x.ID_TIPO == (short)eTiposEstudio.CRIMIN_FEDERAL && x.ID_CENTRO == EntityFederal.ID_CENTRO && x.ID_ANIO == EntityFederal.ID_ANIO);
                                    if (DesarrolloCriminoFederal == null)
                                    {
                                        short _ConsecutivoPersonalidadDetalle = GetIDProceso<short>("PERSONALIDAD_DETALLE", "ID_DETALLE", "1=1");
                                        var NvoDetalle = new PERSONALIDAD_DETALLE()
                                        {
                                            ID_ANIO = EntityFederal.ID_ANIO,
                                            ID_CENTRO = EntityFederal.ID_CENTRO,
                                            ID_DETALLE = _ConsecutivoPersonalidadDetalle,
                                            ID_ESTUDIO = _EstudioPadre.ID_ESTUDIO,
                                            ID_IMPUTADO = EntityFederal.ID_IMPUTADO,
                                            ID_INGRESO = EntityFederal.ID_INGRESO,
                                            ID_TIPO = (short)eTiposEstudio.CRIMIN_FEDERAL,
                                            INICIO_FEC = GetFechaServerDate(),
                                            RESULTADO = _Estatus == (short)eEstatudDetallePersonalidad.TERMINADO ? "S" : "N",
                                            SOLICITUD_FEC = null,
                                            TERMINO_FEC = _Estatus == (short)eEstatudDetallePersonalidad.TERMINADO ? GetFechaServerDate() : new System.DateTime?(),
                                            TIPO_MEDIA = string.Empty,
                                        };

                                        Context.PERSONALIDAD_DETALLE.Add(NvoDetalle);
                                    }

                                    else
                                    {
                                        DesarrolloCriminoFederal.RESULTADO = _Estatus.HasValue ? _Estatus.Value == (short)eEstatudDetallePersonalidad.TERMINADO ? "S" : "N" : "N";
                                        DesarrolloCriminoFederal.ID_ESTATUS = _Estatus;
                                        Context.Entry(DesarrolloCriminoFederal).State = System.Data.EntityState.Modified;
                                    }
                                };
                            }

                            Context.SaveChanges();
                            transaccion.Complete();
                        }

                        else
                        {

                            if (EntityFederal.PFF_ACTA_CONSEJO_TECNICO != null)
                            {
                                var _ActaHecha = Context.PFF_ACTA_CONSEJO_TECNICO.FirstOrDefault(x => x.ID_INGRESO == EntityFederal.ID_INGRESO && x.ID_IMPUTADO == EntityFederal.ID_IMPUTADO && x.ID_ESTUDIO == EntityFederal.ID_ESTUDIO && x.ID_ANIO == EntityFederal.ID_ANIO);
                                var DetalleActas = Context.PFF_ACTA_DETERMINO.Where(x => x.ID_INGRESO == EntityFederal.ID_INGRESO && x.ID_IMPUTADO == EntityFederal.ID_IMPUTADO && x.ID_ESTUDIO == EntityFederal.ID_ESTUDIO && x.ID_ANIO == EntityFederal.ID_ANIO);
                                if (_ActaHecha != null)
                                {
                                    _ActaHecha.APROBADO_APLAZADO = EntityFederal.PFF_ACTA_CONSEJO_TECNICO.APROBADO_APLAZADO;
                                    _ActaHecha.APROBADO_POR = EntityFederal.PFF_ACTA_CONSEJO_TECNICO.APROBADO_POR;
                                    _ActaHecha.CEN_ID_CENTRO = EntityFederal.PFF_ACTA_CONSEJO_TECNICO.CEN_ID_CENTRO;
                                    _ActaHecha.DIRECTOR = EntityFederal.PFF_ACTA_CONSEJO_TECNICO.DIRECTOR;
                                    _ActaHecha.EXPEDIENTE = EntityFederal.PFF_ACTA_CONSEJO_TECNICO.EXPEDIENTE;
                                    _ActaHecha.FECHA = EntityFederal.PFF_ACTA_CONSEJO_TECNICO.FECHA;
                                    _ActaHecha.LUGAR = EntityFederal.PFF_ACTA_CONSEJO_TECNICO.LUGAR;
                                    _ActaHecha.PRESENTE_ACTUACION = EntityFederal.PFF_ACTA_CONSEJO_TECNICO.PRESENTE_ACTUACION;
                                    _ActaHecha.SESION_FEC = EntityFederal.PFF_ACTA_CONSEJO_TECNICO.SESION_FEC;//
                                    _ActaHecha.SUSCRITO_DIRECTOR_CRS = EntityFederal.PFF_ACTA_CONSEJO_TECNICO.SUSCRITO_DIRECTOR_CRS;
                                    _ActaHecha.TRAMITE = EntityFederal.PFF_ACTA_CONSEJO_TECNICO.TRAMITE;//

                                    Context.Entry(_ActaHecha).State = System.Data.EntityState.Modified;

                                    if (DetalleActas != null && DetalleActas.Any())
                                        foreach (var item in DetalleActas)
                                            Context.Entry(item).State = System.Data.EntityState.Deleted;

                                    if (EntityFederal.PFF_ACTA_CONSEJO_TECNICO.PFF_ACTA_DETERMINO != null && EntityFederal.PFF_ACTA_CONSEJO_TECNICO.PFF_ACTA_DETERMINO.Any())
                                        foreach (var item in EntityFederal.PFF_ACTA_CONSEJO_TECNICO.PFF_ACTA_DETERMINO)
                                        {
                                            var _Acta = new PFF_ACTA_DETERMINO()
                                            {
                                                ID_ANIO = EntityFederal.ID_ANIO,
                                                ID_AREA_TECNICA = item.ID_AREA_TECNICA,
                                                ID_CENTRO = EntityFederal.ID_CENTRO,
                                                ID_ESTUDIO = EntityFederal.ID_ESTUDIO,
                                                ID_IMPUTADO = EntityFederal.ID_IMPUTADO,
                                                ID_INGRESO = EntityFederal.ID_INGRESO,
                                                NOMBRE = item.NOMBRE,
                                                OPINION = item.OPINION
                                            };

                                            _ActaHecha.PFF_ACTA_DETERMINO.Add(_Acta);
                                        };
                                }

                                else
                                {
                                    ValidaEstudio.PFF_ACTA_CONSEJO_TECNICO = new PFF_ACTA_CONSEJO_TECNICO()
                                    {
                                        APROBADO_APLAZADO = EntityFederal.PFF_ACTA_CONSEJO_TECNICO.APROBADO_APLAZADO,
                                        APROBADO_POR = EntityFederal.PFF_ACTA_CONSEJO_TECNICO.APROBADO_POR,
                                        CEN_ID_CENTRO = EntityFederal.PFF_ACTA_CONSEJO_TECNICO.CEN_ID_CENTRO,
                                        DIRECTOR = EntityFederal.PFF_ACTA_CONSEJO_TECNICO.DIRECTOR,
                                        EXPEDIENTE = EntityFederal.PFF_ACTA_CONSEJO_TECNICO.EXPEDIENTE,
                                        FECHA = EntityFederal.PFF_ACTA_CONSEJO_TECNICO.FECHA,
                                        ID_ANIO = EntityFederal.ID_ANIO,
                                        ID_CENTRO = EntityFederal.ID_CENTRO,
                                        ID_ESTUDIO = EntityFederal.ID_ESTUDIO,
                                        ID_IMPUTADO = EntityFederal.ID_IMPUTADO,
                                        ID_INGRESO = EntityFederal.ID_INGRESO,
                                        LUGAR = EntityFederal.PFF_ACTA_CONSEJO_TECNICO.LUGAR,
                                        PRESENTE_ACTUACION = EntityFederal.PFF_ACTA_CONSEJO_TECNICO.PRESENTE_ACTUACION,
                                        SESION_FEC = EntityFederal.PFF_ACTA_CONSEJO_TECNICO.SESION_FEC,//
                                        SUSCRITO_DIRECTOR_CRS = EntityFederal.PFF_ACTA_CONSEJO_TECNICO.SUSCRITO_DIRECTOR_CRS,
                                        TRAMITE = EntityFederal.PFF_ACTA_CONSEJO_TECNICO.TRAMITE
                                    };

                                    Context.PFF_ACTA_CONSEJO_TECNICO.Add(ValidaEstudio.PFF_ACTA_CONSEJO_TECNICO);

                                    if (DetalleActas != null && DetalleActas.Any())
                                        foreach (var item in DetalleActas)
                                            Context.Entry(item).State = System.Data.EntityState.Deleted;

                                    if (EntityFederal.PFF_ACTA_CONSEJO_TECNICO.PFF_ACTA_DETERMINO != null && EntityFederal.PFF_ACTA_CONSEJO_TECNICO.PFF_ACTA_DETERMINO.Any())
                                        foreach (var item in EntityFederal.PFF_ACTA_CONSEJO_TECNICO.PFF_ACTA_DETERMINO)
                                        {
                                            var _Acta = new PFF_ACTA_DETERMINO()
                                            {
                                                ID_ANIO = EntityFederal.ID_ANIO,
                                                ID_AREA_TECNICA = item.ID_AREA_TECNICA,
                                                ID_CENTRO = EntityFederal.ID_CENTRO,
                                                ID_ESTUDIO = EntityFederal.ID_ESTUDIO,
                                                ID_IMPUTADO = EntityFederal.ID_IMPUTADO,
                                                ID_INGRESO = EntityFederal.ID_INGRESO,
                                                NOMBRE = item.NOMBRE,
                                                OPINION = item.OPINION
                                            };

                                            _ActaHecha.PFF_ACTA_DETERMINO.Add(_Acta);
                                        };
                                };
                            };

                            if (EntityFederal.PFF_ESTUDIO_MEDICO != null)
                            {
                                var MedicoFederal = Context.PFF_ESTUDIO_MEDICO.FirstOrDefault(x => x.ID_IMPUTADO == EntityFederal.ID_IMPUTADO && x.ID_INGRESO == EntityFederal.ID_INGRESO && x.ID_ESTUDIO == EntityFederal.ID_ESTUDIO && x.ID_ANIO == EntityFederal.ID_ANIO && x.ID_CENTRO == EntityFederal.ID_CENTRO);
                                var DetalleSustToxicas = Context.PFF_SUSTANCIA_TOXICA.Where(x => x.ID_IMPUTADO == EntityFederal.ID_IMPUTADO && x.ID_INGRESO == EntityFederal.ID_INGRESO && x.ID_ESTUDIO == EntityFederal.ID_ESTUDIO && x.ID_ANIO == EntityFederal.ID_ANIO && x.ID_CENTRO == EntityFederal.ID_CENTRO);
                                if (MedicoFederal != null)//SI EXISTE
                                {
                                    MedicoFederal.ALIAS = EntityFederal.PFF_ESTUDIO_MEDICO.ALIAS;
                                    MedicoFederal.ANTE_HEREDO_FAM = EntityFederal.PFF_ESTUDIO_MEDICO.ANTE_HEREDO_FAM;
                                    MedicoFederal.ANTE_PATOLOGICOS = EntityFederal.PFF_ESTUDIO_MEDICO.ANTE_PATOLOGICOS;
                                    MedicoFederal.ANTE_PERSONAL_NO_PATOLOGICOS = EntityFederal.PFF_ESTUDIO_MEDICO.ANTE_PERSONAL_NO_PATOLOGICOS;
                                    MedicoFederal.ASIST_AA = EntityFederal.PFF_ESTUDIO_MEDICO.ASIST_AA;
                                    MedicoFederal.ASIST_FARMACODEPENDENCIA = EntityFederal.PFF_ESTUDIO_MEDICO.ASIST_FARMACODEPENDENCIA;
                                    MedicoFederal.ASIST_OTROS = EntityFederal.PFF_ESTUDIO_MEDICO.ASIST_OTROS;
                                    MedicoFederal.ASIST_OTROS_ESPECIF = EntityFederal.PFF_ESTUDIO_MEDICO.ASIST_OTROS_ESPECIF;
                                    MedicoFederal.CONCLUSION = EntityFederal.PFF_ESTUDIO_MEDICO.CONCLUSION;
                                    MedicoFederal.DELITO = EntityFederal.PFF_ESTUDIO_MEDICO.DELITO;
                                    MedicoFederal.DIAGNOSTICO = EntityFederal.PFF_ESTUDIO_MEDICO.DIAGNOSTICO;
                                    MedicoFederal.DIRECTOR_CENTRO = EntityFederal.PFF_ESTUDIO_MEDICO.DIRECTOR_CENTRO;
                                    MedicoFederal.EDAD = EntityFederal.PFF_ESTUDIO_MEDICO.EDAD;
                                    MedicoFederal.EDO_CIVIL = EntityFederal.PFF_ESTUDIO_MEDICO.EDO_CIVIL != null ? EntityFederal.PFF_ESTUDIO_MEDICO.EDO_CIVIL != "-1" ? EntityFederal.PFF_ESTUDIO_MEDICO.EDO_CIVIL : string.Empty : string.Empty;
                                    MedicoFederal.ESTATURA = EntityFederal.PFF_ESTUDIO_MEDICO.ESTATURA;
                                    MedicoFederal.EXP_FIS_ABDOMEN = EntityFederal.PFF_ESTUDIO_MEDICO.EXP_FIS_ABDOMEN;
                                    MedicoFederal.EXP_FIS_CABEZA_CUELLO = EntityFederal.PFF_ESTUDIO_MEDICO.EXP_FIS_CABEZA_CUELLO;
                                    MedicoFederal.EXP_FIS_EXTREMIDADES = EntityFederal.PFF_ESTUDIO_MEDICO.EXP_FIS_EXTREMIDADES;
                                    MedicoFederal.EXP_FIS_GENITALES = EntityFederal.PFF_ESTUDIO_MEDICO.EXP_FIS_GENITALES;
                                    MedicoFederal.EXP_FIS_TORAX = EntityFederal.PFF_ESTUDIO_MEDICO.EXP_FIS_TORAX;
                                    MedicoFederal.FECHA = EntityFederal.PFF_ESTUDIO_MEDICO.FECHA;
                                    MedicoFederal.ID_ANIO = EntityFederal.ID_ANIO;
                                    MedicoFederal.ID_CENTRO = EntityFederal.ID_CENTRO;
                                    MedicoFederal.ID_ESTUDIO = EntityFederal.ID_ESTUDIO;
                                    MedicoFederal.ID_IMPUTADO = EntityFederal.ID_IMPUTADO;
                                    MedicoFederal.ID_INGRESO = EntityFederal.ID_INGRESO;
                                    MedicoFederal.INTERROGATORIO_APARATOS = EntityFederal.PFF_ESTUDIO_MEDICO.INTERROGATORIO_APARATOS;
                                    MedicoFederal.LUGAR = EntityFederal.PFF_ESTUDIO_MEDICO.LUGAR;
                                    MedicoFederal.MEDICO = EntityFederal.PFF_ESTUDIO_MEDICO.MEDICO;
                                    MedicoFederal.NOMBRE = EntityFederal.PFF_ESTUDIO_MEDICO.NOMBRE;
                                    MedicoFederal.OCUPACION_ACT = EntityFederal.PFF_ESTUDIO_MEDICO.OCUPACION_ACT != -1 ? EntityFederal.PFF_ESTUDIO_MEDICO.OCUPACION_ACT : null;
                                    MedicoFederal.OCUPACION_ANT = EntityFederal.PFF_ESTUDIO_MEDICO.OCUPACION_ANT != -1 ? EntityFederal.PFF_ESTUDIO_MEDICO.OCUPACION_ANT : null;
                                    MedicoFederal.ORIGINARIO_DE = EntityFederal.PFF_ESTUDIO_MEDICO.ORIGINARIO_DE;
                                    MedicoFederal.PADECIMIENTO_ACTUAL = EntityFederal.PFF_ESTUDIO_MEDICO.PADECIMIENTO_ACTUAL;
                                    MedicoFederal.PULSO = EntityFederal.PFF_ESTUDIO_MEDICO.PULSO;
                                    MedicoFederal.RESPIRACION = EntityFederal.PFF_ESTUDIO_MEDICO.RESPIRACION;
                                    MedicoFederal.RESULTADOS_OBTENIDOS = EntityFederal.PFF_ESTUDIO_MEDICO.RESULTADOS_OBTENIDOS;
                                    MedicoFederal.SENTENCIA = EntityFederal.PFF_ESTUDIO_MEDICO.SENTENCIA;
                                    MedicoFederal.TA = EntityFederal.PFF_ESTUDIO_MEDICO.TA;
                                    MedicoFederal.TEMPERATURA = EntityFederal.PFF_ESTUDIO_MEDICO.TEMPERATURA;
                                    Context.Entry(MedicoFederal).State = System.Data.EntityState.Modified;

                                    if (DetalleSustToxicas != null && DetalleSustToxicas.Any())
                                        foreach (var item in DetalleSustToxicas)
                                            Context.Entry(item).State = System.Data.EntityState.Deleted;

                                    if (EntityFederal.PFF_ESTUDIO_MEDICO.PFF_SUSTANCIA_TOXICA != null && EntityFederal.PFF_ESTUDIO_MEDICO.PFF_SUSTANCIA_TOXICA.Any())
                                        foreach (var item in EntityFederal.PFF_ESTUDIO_MEDICO.PFF_SUSTANCIA_TOXICA)
                                        {
                                            var _NuevaToxica = new PFF_SUSTANCIA_TOXICA()
                                            {
                                                CANTIDAD = item.CANTIDAD,
                                                EDAD_INICIO = item.EDAD_INICIO,
                                                ID_ANIO = EntityFederal.ID_ANIO,
                                                ID_CENTRO = EntityFederal.ID_CENTRO,
                                                ID_DROGA = item.ID_DROGA,
                                                ID_ESTUDIO = EntityFederal.ID_ESTUDIO,
                                                ID_IMPUTADO = EntityFederal.ID_IMPUTADO,
                                                ID_INGRESO = EntityFederal.ID_INGRESO,
                                                PERIODICIDAD = item.PERIODICIDAD
                                            };

                                            MedicoFederal.PFF_SUSTANCIA_TOXICA.Add(_NuevaToxica);
                                        };
                                }

                                else
                                {//SI NO EXISTE
                                    ValidaEstudio.PFF_ESTUDIO_MEDICO = new PFF_ESTUDIO_MEDICO()
                                    {
                                        ALIAS = EntityFederal.PFF_ESTUDIO_MEDICO.ALIAS,
                                        ANTE_HEREDO_FAM = EntityFederal.PFF_ESTUDIO_MEDICO.ANTE_HEREDO_FAM,
                                        ANTE_PATOLOGICOS = EntityFederal.PFF_ESTUDIO_MEDICO.ANTE_PATOLOGICOS,
                                        ANTE_PERSONAL_NO_PATOLOGICOS = EntityFederal.PFF_ESTUDIO_MEDICO.ANTE_PERSONAL_NO_PATOLOGICOS,
                                        ASIST_AA = EntityFederal.PFF_ESTUDIO_MEDICO.ASIST_AA,
                                        ASIST_FARMACODEPENDENCIA = EntityFederal.PFF_ESTUDIO_MEDICO.ASIST_FARMACODEPENDENCIA,
                                        ASIST_OTROS = EntityFederal.PFF_ESTUDIO_MEDICO.ASIST_OTROS,
                                        ASIST_OTROS_ESPECIF = EntityFederal.PFF_ESTUDIO_MEDICO.ASIST_OTROS_ESPECIF,
                                        CONCLUSION = EntityFederal.PFF_ESTUDIO_MEDICO.CONCLUSION,
                                        DELITO = EntityFederal.PFF_ESTUDIO_MEDICO.DELITO,
                                        DIAGNOSTICO = EntityFederal.PFF_ESTUDIO_MEDICO.DIAGNOSTICO,
                                        DIRECTOR_CENTRO = EntityFederal.PFF_ESTUDIO_MEDICO.DIRECTOR_CENTRO,
                                        EDAD = EntityFederal.PFF_ESTUDIO_MEDICO.EDAD,
                                        EDO_CIVIL = EntityFederal.PFF_ESTUDIO_MEDICO.EDO_CIVIL != null ? EntityFederal.PFF_ESTUDIO_MEDICO.EDO_CIVIL != "-1" ? EntityFederal.PFF_ESTUDIO_MEDICO.EDO_CIVIL : string.Empty : string.Empty,
                                        ESTATURA = EntityFederal.PFF_ESTUDIO_MEDICO.ESTATURA,
                                        EXP_FIS_ABDOMEN = EntityFederal.PFF_ESTUDIO_MEDICO.EXP_FIS_ABDOMEN,
                                        EXP_FIS_CABEZA_CUELLO = EntityFederal.PFF_ESTUDIO_MEDICO.EXP_FIS_CABEZA_CUELLO,
                                        EXP_FIS_EXTREMIDADES = EntityFederal.PFF_ESTUDIO_MEDICO.EXP_FIS_EXTREMIDADES,
                                        EXP_FIS_GENITALES = EntityFederal.PFF_ESTUDIO_MEDICO.EXP_FIS_GENITALES,
                                        EXP_FIS_TORAX = EntityFederal.PFF_ESTUDIO_MEDICO.EXP_FIS_TORAX,
                                        FECHA = EntityFederal.PFF_ESTUDIO_MEDICO.FECHA,
                                        ID_ANIO = EntityFederal.ID_ANIO,
                                        ID_CENTRO = EntityFederal.ID_CENTRO,
                                        ID_ESTUDIO = EntityFederal.ID_ESTUDIO,
                                        TATUAJES = EntityFederal.PFF_ESTUDIO_MEDICO.TATUAJES,
                                        ID_IMPUTADO = EntityFederal.ID_IMPUTADO,
                                        ID_INGRESO = EntityFederal.ID_INGRESO,
                                        INTERROGATORIO_APARATOS = EntityFederal.PFF_ESTUDIO_MEDICO.INTERROGATORIO_APARATOS,
                                        LUGAR = EntityFederal.PFF_ESTUDIO_MEDICO.LUGAR,
                                        MEDICO = EntityFederal.PFF_ESTUDIO_MEDICO.MEDICO,
                                        NOMBRE = EntityFederal.PFF_ESTUDIO_MEDICO.NOMBRE,
                                        OCUPACION_ACT = EntityFederal.PFF_ESTUDIO_MEDICO.OCUPACION_ACT != -1 ? EntityFederal.PFF_ESTUDIO_MEDICO.OCUPACION_ACT : null,
                                        OCUPACION_ANT = EntityFederal.PFF_ESTUDIO_MEDICO.OCUPACION_ANT != -1 ? EntityFederal.PFF_ESTUDIO_MEDICO.OCUPACION_ANT : null,
                                        ORIGINARIO_DE = EntityFederal.PFF_ESTUDIO_MEDICO.ORIGINARIO_DE,
                                        PADECIMIENTO_ACTUAL = EntityFederal.PFF_ESTUDIO_MEDICO.PADECIMIENTO_ACTUAL,
                                        PULSO = EntityFederal.PFF_ESTUDIO_MEDICO.PULSO,
                                        RESPIRACION = EntityFederal.PFF_ESTUDIO_MEDICO.RESPIRACION,
                                        RESULTADOS_OBTENIDOS = EntityFederal.PFF_ESTUDIO_MEDICO.RESULTADOS_OBTENIDOS,
                                        SENTENCIA = EntityFederal.PFF_ESTUDIO_MEDICO.SENTENCIA,
                                        TA = EntityFederal.PFF_ESTUDIO_MEDICO.TA,
                                        TEMPERATURA = EntityFederal.PFF_ESTUDIO_MEDICO.TEMPERATURA
                                    };

                                    if (DetalleSustToxicas != null && DetalleSustToxicas.Any())
                                        foreach (var item in DetalleSustToxicas)
                                            Context.Entry(item).State = System.Data.EntityState.Deleted;

                                    if (EntityFederal.PFF_ESTUDIO_MEDICO.PFF_SUSTANCIA_TOXICA != null && EntityFederal.PFF_ESTUDIO_MEDICO.PFF_SUSTANCIA_TOXICA.Any())
                                        foreach (var item in EntityFederal.PFF_ESTUDIO_MEDICO.PFF_SUSTANCIA_TOXICA)
                                        {
                                            var _NuevaToxica = new PFF_SUSTANCIA_TOXICA()
                                            {
                                                CANTIDAD = item.CANTIDAD,
                                                EDAD_INICIO = item.EDAD_INICIO,
                                                ID_ANIO = EntityFederal.ID_ANIO,
                                                ID_CENTRO = EntityFederal.ID_CENTRO,
                                                ID_DROGA = item.ID_DROGA,
                                                ID_ESTUDIO = EntityFederal.ID_ESTUDIO,
                                                ID_IMPUTADO = EntityFederal.ID_IMPUTADO,
                                                ID_INGRESO = EntityFederal.ID_INGRESO,
                                                PERIODICIDAD = item.PERIODICIDAD
                                            };

                                            ValidaEstudio.PFF_ESTUDIO_MEDICO.PFF_SUSTANCIA_TOXICA.Add(_NuevaToxica);
                                        };

                                    Context.PFF_ESTUDIO_MEDICO.Add(ValidaEstudio.PFF_ESTUDIO_MEDICO);
                                };


                                #region Definicion de detalle medico de fuero federal

                                if (_EstudioPadre != null)
                                {
                                    short? _Estatus = new short?();
                                    if (string.IsNullOrEmpty(EntityFederal.PFF_ESTUDIO_MEDICO.ALIAS) || string.IsNullOrEmpty(EntityFederal.PFF_ESTUDIO_MEDICO.ANTE_HEREDO_FAM) || string.IsNullOrEmpty(EntityFederal.PFF_ESTUDIO_MEDICO.ANTE_PATOLOGICOS) ||
                                        string.IsNullOrEmpty(EntityFederal.PFF_ESTUDIO_MEDICO.ANTE_PERSONAL_NO_PATOLOGICOS) || string.IsNullOrEmpty(EntityFederal.PFF_ESTUDIO_MEDICO.CONCLUSION) || string.IsNullOrEmpty(EntityFederal.PFF_ESTUDIO_MEDICO.DELITO) ||
                                        string.IsNullOrEmpty(EntityFederal.PFF_ESTUDIO_MEDICO.DIAGNOSTICO) || string.IsNullOrEmpty(EntityFederal.PFF_ESTUDIO_MEDICO.EDO_CIVIL) || string.IsNullOrEmpty(EntityFederal.PFF_ESTUDIO_MEDICO.ESTATURA) ||
                                        string.IsNullOrEmpty(EntityFederal.PFF_ESTUDIO_MEDICO.EXP_FIS_ABDOMEN) || string.IsNullOrEmpty(EntityFederal.PFF_ESTUDIO_MEDICO.EXP_FIS_CABEZA_CUELLO) || string.IsNullOrEmpty(EntityFederal.PFF_ESTUDIO_MEDICO.EXP_FIS_EXTREMIDADES) ||
                                        string.IsNullOrEmpty(EntityFederal.PFF_ESTUDIO_MEDICO.EXP_FIS_GENITALES) || string.IsNullOrEmpty(EntityFederal.PFF_ESTUDIO_MEDICO.EXP_FIS_TORAX) || string.IsNullOrEmpty(EntityFederal.PFF_ESTUDIO_MEDICO.INTERROGATORIO_APARATOS) ||
                                        //string.IsNullOrEmpty(EntityFederal.PFF_ESTUDIO_MEDICO.NOMBRE) || string.IsNullOrEmpty(EntityFederal.PFF_ESTUDIO_MEDICO.OCUPACION_ACT) || string.IsNullOrEmpty(EntityFederal.PFF_ESTUDIO_MEDICO.OCUPACION_ANT) ||
                                         string.IsNullOrEmpty(EntityFederal.PFF_ESTUDIO_MEDICO.ORIGINARIO_DE) || string.IsNullOrEmpty(EntityFederal.PFF_ESTUDIO_MEDICO.PADECIMIENTO_ACTUAL) || string.IsNullOrEmpty(EntityFederal.PFF_ESTUDIO_MEDICO.PULSO) ||
                                        string.IsNullOrEmpty(EntityFederal.PFF_ESTUDIO_MEDICO.RESPIRACION) || string.IsNullOrEmpty(EntityFederal.PFF_ESTUDIO_MEDICO.RESULTADOS_OBTENIDOS) || string.IsNullOrEmpty(EntityFederal.PFF_ESTUDIO_MEDICO.SENTENCIA) || string.IsNullOrEmpty(EntityFederal.PFF_ESTUDIO_MEDICO.TA) ||
                                         string.IsNullOrEmpty(EntityFederal.PFF_ESTUDIO_MEDICO.TATUAJES) || string.IsNullOrEmpty(EntityFederal.PFF_ESTUDIO_MEDICO.TEMPERATURA) || !EntityFederal.PFF_ESTUDIO_MEDICO.EDAD.HasValue || !EntityFederal.PFF_ESTUDIO_MEDICO.FECHA.HasValue)
                                        _Estatus = (short)eEstatudDetallePersonalidad.ASIGNADO;

                                    else
                                        _Estatus = (short)eEstatudDetallePersonalidad.TERMINADO;


                                    var DesarrolloMedicoFederal = Context.PERSONALIDAD_DETALLE.FirstOrDefault(x => x.ID_ESTUDIO == _EstudioPadre.ID_ESTUDIO && x.ID_TIPO == (short)eTiposEstudio.MEDICA_FEDERAL && x.ID_INGRESO == EntityFederal.ID_INGRESO && x.ID_IMPUTADO == EntityFederal.ID_IMPUTADO && x.ID_CENTRO == EntityFederal.ID_CENTRO && x.ID_ANIO == EntityFederal.ID_ANIO);
                                    if (DesarrolloMedicoFederal == null)
                                    {
                                        short _ConsecutivoPersonalidadDetalle = GetIDProceso<short>("PERSONALIDAD_DETALLE", "ID_DETALLE", "1=1");
                                        var NvoDetalle = new PERSONALIDAD_DETALLE()
                                        {
                                            ID_ANIO = EntityFederal.ID_ANIO,
                                            ID_CENTRO = EntityFederal.ID_CENTRO,
                                            ID_DETALLE = _ConsecutivoPersonalidadDetalle,
                                            ID_ESTUDIO = _EstudioPadre.ID_ESTUDIO,
                                            ID_IMPUTADO = EntityFederal.ID_IMPUTADO,
                                            ID_INGRESO = EntityFederal.ID_INGRESO,
                                            ID_TIPO = (short)eTiposEstudio.MEDICA_FEDERAL,
                                            ID_ESTATUS = _Estatus,
                                            INICIO_FEC = GetFechaServerDate(),
                                            RESULTADO = _Estatus == (short)eEstatudDetallePersonalidad.TERMINADO ? "S" : "N",
                                            SOLICITUD_FEC = null,
                                            TERMINO_FEC = _Estatus == (short)eEstatudDetallePersonalidad.TERMINADO ? GetFechaServerDate() : new System.DateTime?(),
                                            TIPO_MEDIA = string.Empty
                                        };

                                        Context.PERSONALIDAD_DETALLE.Add(NvoDetalle);
                                    }

                                    else
                                    {
                                        DesarrolloMedicoFederal.RESULTADO = _Estatus.HasValue ? _Estatus.Value == (short)eEstatudDetallePersonalidad.TERMINADO ? "S" : "N" : "N";
                                        DesarrolloMedicoFederal.ID_ESTATUS = _Estatus;
                                        Context.Entry(DesarrolloMedicoFederal).State = System.Data.EntityState.Modified;
                                    }
                                };
                                #endregion
                            };



                            if (EntityFederal.PFF_ESTUDIO_PSICOLOGICO != null)
                            {
                                var _EstudioPsico = Context.PFF_ESTUDIO_PSICOLOGICO.FirstOrDefault(c => c.ID_IMPUTADO == EntityFederal.ID_IMPUTADO && c.ID_INGRESO == EntityFederal.ID_INGRESO && c.ID_ESTUDIO == EntityFederal.ID_ESTUDIO && c.ID_ANIO == EntityFederal.ID_ANIO && c.ID_CENTRO == EntityFederal.ID_CENTRO);
                                if (_EstudioPsico == null)
                                {
                                    ValidaEstudio.PFF_ESTUDIO_PSICOLOGICO = new PFF_ESTUDIO_PSICOLOGICO()
                                    {
                                        ACTITUD = EntityFederal.PFF_ESTUDIO_PSICOLOGICO.ACTITUD,
                                        CI = EntityFederal.PFF_ESTUDIO_PSICOLOGICO.CI,
                                        DELITO = EntityFederal.PFF_ESTUDIO_PSICOLOGICO.DELITO,
                                        DINAM_PERSON_ACTUAL = EntityFederal.PFF_ESTUDIO_PSICOLOGICO.DINAM_PERSON_ACTUAL,
                                        DINAM_PERSON_INGRESO = EntityFederal.PFF_ESTUDIO_PSICOLOGICO.DINAM_PERSON_INGRESO,
                                        EDAD = EntityFederal.PFF_ESTUDIO_PSICOLOGICO.EDAD,
                                        DIRECTOR_DENTRO = EntityFederal.PFF_ESTUDIO_PSICOLOGICO.DIRECTOR_DENTRO,
                                        ESPECIFIQUE = EntityFederal.PFF_ESTUDIO_PSICOLOGICO.ESPECIFIQUE,
                                        EXAMEN_MENTAL = EntityFederal.PFF_ESTUDIO_PSICOLOGICO.EXAMEN_MENTAL,
                                        EXTERNO = EntityFederal.PFF_ESTUDIO_PSICOLOGICO.EXTERNO,
                                        FECHA = EntityFederal.PFF_ESTUDIO_PSICOLOGICO.FECHA,
                                        ID_ANIO = EntityFederal.ID_ANIO,
                                        ID_CENTRO = EntityFederal.ID_CENTRO,
                                        ID_ESTUDIO = EntityFederal.ID_ESTUDIO,
                                        ID_IMPUTADO = EntityFederal.ID_IMPUTADO,
                                        ID_INGRESO = EntityFederal.ID_INGRESO,
                                        INDICE_LESION_ORGANICA = EntityFederal.PFF_ESTUDIO_PSICOLOGICO.INDICE_LESION_ORGANICA,
                                        INTERNO = EntityFederal.PFF_ESTUDIO_PSICOLOGICO.INTERNO,
                                        LUGAR = EntityFederal.PFF_ESTUDIO_PSICOLOGICO.LUGAR,
                                        NIVEL_INTELECTUAL = EntityFederal.PFF_ESTUDIO_PSICOLOGICO.NIVEL_INTELECTUAL,
                                        NOMBRE = EntityFederal.PFF_ESTUDIO_PSICOLOGICO.NOMBRE,
                                        OPINION = EntityFederal.PFF_ESTUDIO_PSICOLOGICO.OPINION,
                                        PRONOSTICO_REINTEGRACION = EntityFederal.PFF_ESTUDIO_PSICOLOGICO.PRONOSTICO_REINTEGRACION,
                                        PRUEBAS_APLICADAS = EntityFederal.PFF_ESTUDIO_PSICOLOGICO.PRUEBAS_APLICADAS,
                                        PSICOLOGO = EntityFederal.PFF_ESTUDIO_PSICOLOGICO.PSICOLOGO,
                                        REQ_CONT_TRATAMIENTO = EntityFederal.PFF_ESTUDIO_PSICOLOGICO.REQ_CONT_TRATAMIENTO,
                                        RESULT_TRATAMIENTO = EntityFederal.PFF_ESTUDIO_PSICOLOGICO.RESULT_TRATAMIENTO,
                                        SOBRENOMBRE = EntityFederal.PFF_ESTUDIO_PSICOLOGICO.SOBRENOMBRE
                                    };

                                    Context.PFF_ESTUDIO_PSICOLOGICO.Add(ValidaEstudio.PFF_ESTUDIO_PSICOLOGICO);
                                }

                                else
                                {
                                    _EstudioPsico.ACTITUD = EntityFederal.PFF_ESTUDIO_PSICOLOGICO.ACTITUD;
                                    _EstudioPsico.CI = EntityFederal.PFF_ESTUDIO_PSICOLOGICO.CI;
                                    _EstudioPsico.DELITO = EntityFederal.PFF_ESTUDIO_PSICOLOGICO.DELITO;
                                    _EstudioPsico.DINAM_PERSON_ACTUAL = EntityFederal.PFF_ESTUDIO_PSICOLOGICO.DINAM_PERSON_ACTUAL;
                                    _EstudioPsico.DINAM_PERSON_INGRESO = EntityFederal.PFF_ESTUDIO_PSICOLOGICO.DINAM_PERSON_INGRESO;
                                    _EstudioPsico.DIRECTOR_DENTRO = EntityFederal.PFF_ESTUDIO_PSICOLOGICO.DIRECTOR_DENTRO;
                                    _EstudioPsico.EDAD = EntityFederal.PFF_ESTUDIO_PSICOLOGICO.EDAD;
                                    _EstudioPsico.ESPECIFIQUE = EntityFederal.PFF_ESTUDIO_PSICOLOGICO.ESPECIFIQUE;
                                    _EstudioPsico.EXAMEN_MENTAL = EntityFederal.PFF_ESTUDIO_PSICOLOGICO.EXAMEN_MENTAL;
                                    _EstudioPsico.EXTERNO = EntityFederal.PFF_ESTUDIO_PSICOLOGICO.EXTERNO;
                                    _EstudioPsico.FECHA = EntityFederal.PFF_ESTUDIO_PSICOLOGICO.FECHA;
                                    _EstudioPsico.ID_ANIO = EntityFederal.ID_ANIO;
                                    _EstudioPsico.ID_CENTRO = EntityFederal.ID_CENTRO;
                                    _EstudioPsico.ID_ESTUDIO = EntityFederal.ID_ESTUDIO;
                                    _EstudioPsico.ID_IMPUTADO = EntityFederal.ID_IMPUTADO;
                                    _EstudioPsico.ID_INGRESO = EntityFederal.ID_INGRESO;
                                    _EstudioPsico.INDICE_LESION_ORGANICA = EntityFederal.PFF_ESTUDIO_PSICOLOGICO.INDICE_LESION_ORGANICA;
                                    _EstudioPsico.INTERNO = EntityFederal.PFF_ESTUDIO_PSICOLOGICO.INTERNO;
                                    _EstudioPsico.LUGAR = EntityFederal.PFF_ESTUDIO_PSICOLOGICO.LUGAR;
                                    _EstudioPsico.NIVEL_INTELECTUAL = EntityFederal.PFF_ESTUDIO_PSICOLOGICO.NIVEL_INTELECTUAL;
                                    _EstudioPsico.NOMBRE = EntityFederal.PFF_ESTUDIO_PSICOLOGICO.NOMBRE;
                                    _EstudioPsico.OPINION = EntityFederal.PFF_ESTUDIO_PSICOLOGICO.OPINION;
                                    _EstudioPsico.PRONOSTICO_REINTEGRACION = EntityFederal.PFF_ESTUDIO_PSICOLOGICO.PRONOSTICO_REINTEGRACION;
                                    _EstudioPsico.PRUEBAS_APLICADAS = EntityFederal.PFF_ESTUDIO_PSICOLOGICO.PRUEBAS_APLICADAS;
                                    _EstudioPsico.PSICOLOGO = EntityFederal.PFF_ESTUDIO_PSICOLOGICO.PSICOLOGO;
                                    _EstudioPsico.REQ_CONT_TRATAMIENTO = EntityFederal.PFF_ESTUDIO_PSICOLOGICO.REQ_CONT_TRATAMIENTO;
                                    _EstudioPsico.RESULT_TRATAMIENTO = EntityFederal.PFF_ESTUDIO_PSICOLOGICO.RESULT_TRATAMIENTO;
                                    _EstudioPsico.SOBRENOMBRE = EntityFederal.PFF_ESTUDIO_PSICOLOGICO.SOBRENOMBRE;
                                    Context.Entry(_EstudioPsico).State = System.Data.EntityState.Modified;
                                };


                                if (_EstudioPadre != null)
                                {
                                    short? _Estatus = new short?();
                                    if (string.IsNullOrEmpty(EntityFederal.PFF_ESTUDIO_PSICOLOGICO.ACTITUD) || string.IsNullOrEmpty(EntityFederal.PFF_ESTUDIO_PSICOLOGICO.CI) || string.IsNullOrEmpty(EntityFederal.PFF_ESTUDIO_PSICOLOGICO.DELITO) || string.IsNullOrEmpty(EntityFederal.PFF_ESTUDIO_PSICOLOGICO.DINAM_PERSON_ACTUAL)
                                        || string.IsNullOrEmpty(EntityFederal.PFF_ESTUDIO_PSICOLOGICO.DINAM_PERSON_INGRESO) || string.IsNullOrEmpty(EntityFederal.PFF_ESTUDIO_PSICOLOGICO.EXAMEN_MENTAL) || string.IsNullOrEmpty(EntityFederal.PFF_ESTUDIO_PSICOLOGICO.EXTERNO) || string.IsNullOrEmpty(EntityFederal.PFF_ESTUDIO_PSICOLOGICO.INDICE_LESION_ORGANICA)
                                        || string.IsNullOrEmpty(EntityFederal.PFF_ESTUDIO_PSICOLOGICO.INTERNO) || string.IsNullOrEmpty(EntityFederal.PFF_ESTUDIO_PSICOLOGICO.NIVEL_INTELECTUAL) || string.IsNullOrEmpty(EntityFederal.PFF_ESTUDIO_PSICOLOGICO.NOMBRE) || string.IsNullOrEmpty(EntityFederal.PFF_ESTUDIO_PSICOLOGICO.OPINION) || string.IsNullOrEmpty(EntityFederal.PFF_ESTUDIO_PSICOLOGICO.PRONOSTICO_REINTEGRACION)
                                        || string.IsNullOrEmpty(EntityFederal.PFF_ESTUDIO_PSICOLOGICO.PRUEBAS_APLICADAS) || string.IsNullOrEmpty(EntityFederal.PFF_ESTUDIO_PSICOLOGICO.REQ_CONT_TRATAMIENTO) || string.IsNullOrEmpty(EntityFederal.PFF_ESTUDIO_PSICOLOGICO.RESULT_TRATAMIENTO) || string.IsNullOrEmpty(EntityFederal.PFF_ESTUDIO_PSICOLOGICO.SOBRENOMBRE) || !EntityFederal.PFF_ESTUDIO_PSICOLOGICO.EDAD.HasValue ||
                                        !EntityFederal.PFF_ESTUDIO_PSICOLOGICO.FECHA.HasValue)
                                        _Estatus = (short)eEstatudDetallePersonalidad.ASIGNADO;
                                    else
                                        _Estatus = (short)eEstatudDetallePersonalidad.TERMINADO;

                                    var DesarrolloPsicoFederal = Context.PERSONALIDAD_DETALLE.FirstOrDefault(x => x.ID_IMPUTADO == EntityFederal.ID_IMPUTADO && x.ID_ESTUDIO == _EstudioPadre.ID_ESTUDIO && x.ID_INGRESO == EntityFederal.ID_INGRESO && x.ID_TIPO == (short)eTiposEstudio.PSICOLOGICA_FEDERAL && x.ID_CENTRO == EntityFederal.ID_CENTRO && x.ID_ANIO == EntityFederal.ID_ANIO);
                                    if (DesarrolloPsicoFederal == null)
                                    {
                                        short _ConsecutivoPersonalidadDetalle = GetIDProceso<short>("PERSONALIDAD_DETALLE", "ID_DETALLE", "1=1");
                                        var NvoDetalle = new PERSONALIDAD_DETALLE()
                                        {
                                            ID_ANIO = EntityFederal.ID_ANIO,
                                            ID_CENTRO = EntityFederal.ID_CENTRO,
                                            ID_DETALLE = _ConsecutivoPersonalidadDetalle,
                                            ID_ESTUDIO = _EstudioPadre.ID_ESTUDIO,
                                            ID_IMPUTADO = EntityFederal.ID_IMPUTADO,
                                            ID_INGRESO = EntityFederal.ID_INGRESO,
                                            ID_ESTATUS = _Estatus,
                                            ID_TIPO = (short)eTiposEstudio.PSICOLOGICA_FEDERAL,
                                            INICIO_FEC = GetFechaServerDate(),
                                            RESULTADO = _Estatus == (short)eEstatudDetallePersonalidad.TERMINADO ? "S" : "N",
                                            SOLICITUD_FEC = null,
                                            TERMINO_FEC = _Estatus == (short)eEstatudDetallePersonalidad.TERMINADO ? GetFechaServerDate() : new System.DateTime?(),
                                            TIPO_MEDIA = string.Empty
                                        };

                                        Context.PERSONALIDAD_DETALLE.Add(NvoDetalle);
                                    }

                                    else
                                    {
                                        DesarrolloPsicoFederal.RESULTADO = _Estatus.HasValue ? _Estatus.Value == (short)eEstatudDetallePersonalidad.TERMINADO ? "S" : "N" : "N";
                                        DesarrolloPsicoFederal.ID_ESTATUS = _Estatus;
                                        Context.Entry(DesarrolloPsicoFederal).State = System.Data.EntityState.Modified;
                                    };
                                };
                            };

                            if (EntityFederal.PFF_TRABAJO_SOCIAL != null)
                            {
                                var _EstudioTrabajoSocialHecho = Context.PFF_TRABAJO_SOCIAL.FirstOrDefault(x => x.ID_INGRESO == EntityFederal.ID_INGRESO && x.ID_IMPUTADO == EntityFederal.ID_IMPUTADO && x.ID_ESTUDIO == EntityFederal.ID_ESTUDIO && x.ID_ANIO == EntityFederal.ID_ANIO && x.ID_CENTRO == EntityFederal.ID_CENTRO);
                                var _GruposFam = Context.PFF_GRUPO_FAMILIAR.Where(x => x.ID_INGRESO == EntityFederal.ID_INGRESO && x.ID_IMPUTADO == EntityFederal.ID_IMPUTADO && x.ID_ESTUDIO == EntityFederal.ID_ESTUDIO && x.ID_ANIO == EntityFederal.ID_ANIO && x.ID_CENTRO == EntityFederal.ID_CENTRO);
                                if (_EstudioTrabajoSocialHecho == null)
                                {
                                    ValidaEstudio.PFF_TRABAJO_SOCIAL = new PFF_TRABAJO_SOCIAL()
                                    {
                                        ALIMENTACION_FAM = EntityFederal.PFF_TRABAJO_SOCIAL.ALIMENTACION_FAM,
                                        APORTACIONES_FAM = EntityFederal.PFF_TRABAJO_SOCIAL.APORTACIONES_FAM,
                                        APOYO_FAM_OTROS = EntityFederal.PFF_TRABAJO_SOCIAL.APOYO_FAM_OTROS,
                                        CARACT_FP_ANTECE_PENALES_ADIC = EntityFederal.PFF_TRABAJO_SOCIAL.CARACT_FP_ANTECE_PENALES_ADIC,
                                        CARACT_FP_ANTECEDENTES_PENALES = EntityFederal.PFF_TRABAJO_SOCIAL.CARACT_FP_ANTECEDENTES_PENALES,
                                        CARACT_FP_CONCEPTO = EntityFederal.PFF_TRABAJO_SOCIAL.CARACT_FP_CONCEPTO,
                                        CARACT_FP_GRUPO = EntityFederal.PFF_TRABAJO_SOCIAL.CARACT_FP_GRUPO,
                                        CARACT_FP_NIVEL_SOCIO_CULTURAL = EntityFederal.PFF_TRABAJO_SOCIAL.CARACT_FP_NIVEL_SOCIO_CULTURAL,
                                        CARACT_FP_RELAC_INTERFAM = EntityFederal.PFF_TRABAJO_SOCIAL.CARACT_FP_RELAC_INTERFAM,
                                        CARACT_FP_VIOLENCIA_FAM = EntityFederal.PFF_TRABAJO_SOCIAL.CARACT_FP_VIOLENCIA_FAM,
                                        CARACT_FP_VIOLENCIA_FAM_ESPEFI = EntityFederal.PFF_TRABAJO_SOCIAL.CARACT_FP_VIOLENCIA_FAM_ESPEFI,
                                        CARACT_FS_GRUPO = EntityFederal.PFF_TRABAJO_SOCIAL.CARACT_FS_GRUPO,
                                        CARACT_FS_HIJOS_ANT = EntityFederal.PFF_TRABAJO_SOCIAL.CARACT_FS_HIJOS_ANT,
                                        CARACT_FS_NIVEL_SOCIO_CULTURAL = EntityFederal.PFF_TRABAJO_SOCIAL.CARACT_FS_NIVEL_SOCIO_CULTURAL,
                                        CARACT_FS_PROBLEMAS_CONDUCTA = EntityFederal.PFF_TRABAJO_SOCIAL.CARACT_FS_PROBLEMAS_CONDUCTA,
                                        CARACT_FS_PROBLEMAS_CONDUCTA_E = EntityFederal.PFF_TRABAJO_SOCIAL.CARACT_FS_PROBLEMAS_CONDUCTA_E,
                                        CARACT_FS_RELACION_MEDIO_EXT = EntityFederal.PFF_TRABAJO_SOCIAL.CARACT_FS_RELACION_MEDIO_EXT,
                                        CARACT_FS_RELACIONES_INTERFAM = EntityFederal.PFF_TRABAJO_SOCIAL.CARACT_FS_RELACIONES_INTERFAM,
                                        CARACT_FS_VIOLENCIA_INTRAFAM = EntityFederal.PFF_TRABAJO_SOCIAL.CARACT_FS_VIOLENCIA_INTRAFAM,
                                        CARACT_FS_VIOLENCIA_INTRAFAM_E = EntityFederal.PFF_TRABAJO_SOCIAL.CARACT_FS_VIOLENCIA_INTRAFAM_E,
                                        CARACT_FS_VIVIEN_DESCRIPCION = EntityFederal.PFF_TRABAJO_SOCIAL.CARACT_FS_VIVIEN_DESCRIPCION,
                                        CARACT_FS_VIVIEN_MOBILIARIO = EntityFederal.PFF_TRABAJO_SOCIAL.CARACT_FS_VIVIEN_MOBILIARIO,
                                        CARACT_FS_VIVIEN_NUM_HABITACIO = EntityFederal.PFF_TRABAJO_SOCIAL.CARACT_FS_VIVIEN_NUM_HABITACIO,
                                        CARACT_FS_VIVIEN_TRANSPORTE = EntityFederal.PFF_TRABAJO_SOCIAL.CARACT_FS_VIVIEN_TRANSPORTE,
                                        CARACT_FS_ZONA = EntityFederal.PFF_TRABAJO_SOCIAL.CARACT_FS_ZONA,
                                        DIAG_SOCIAL_PRONOS = EntityFederal.PFF_TRABAJO_SOCIAL.DIAG_SOCIAL_PRONOS,
                                        DIALECTO = EntityFederal.PFF_TRABAJO_SOCIAL.DIALECTO != -1 ? EntityFederal.PFF_TRABAJO_SOCIAL.DIALECTO : null,
                                        DIRECTOR_CENTRO = EntityFederal.PFF_TRABAJO_SOCIAL.DIRECTOR_CENTRO,
                                        DISTRIBUCION_GASTO_FAM = EntityFederal.PFF_TRABAJO_SOCIAL.DISTRIBUCION_GASTO_FAM,
                                        DOMICILIO = EntityFederal.PFF_TRABAJO_SOCIAL.DOMICILIO,
                                        ECO_FP_COOPERA_ACTUALMENTE = EntityFederal.PFF_TRABAJO_SOCIAL.ECO_FP_COOPERA_ACTUALMENTE,
                                        ECO_FP_FONDOS_AHORRO = EntityFederal.PFF_TRABAJO_SOCIAL.ECO_FP_FONDOS_AHORRO,
                                        ECO_FP_RESPONSABLE = EntityFederal.PFF_TRABAJO_SOCIAL.ECO_FP_RESPONSABLE,
                                        ECO_FP_TOTAL_EGRESOS_MEN = EntityFederal.PFF_TRABAJO_SOCIAL.ECO_FP_TOTAL_EGRESOS_MEN,
                                        ECO_FP_TOTAL_INGRESOS_MEN = EntityFederal.PFF_TRABAJO_SOCIAL.ECO_FP_TOTAL_INGRESOS_MEN,
                                        ECO_FP_ZONA = EntityFederal.PFF_TRABAJO_SOCIAL.ECO_FP_ZONA,
                                        EDO_CIVIL = EntityFederal.PFF_TRABAJO_SOCIAL.EDO_CIVIL != -1 ? EntityFederal.PFF_TRABAJO_SOCIAL.EDO_CIVIL : null,
                                        ESCOLARIDAD_ACTUAL = EntityFederal.PFF_TRABAJO_SOCIAL.ESCOLARIDAD_ACTUAL != -1 ? EntityFederal.PFF_TRABAJO_SOCIAL.ESCOLARIDAD_ACTUAL : null,
                                        ESCOLARIDAD_CENTRO = EntityFederal.PFF_TRABAJO_SOCIAL.ESCOLARIDAD_CENTRO != -1 ? EntityFederal.PFF_TRABAJO_SOCIAL.ESCOLARIDAD_CENTRO : null,
                                        AVAL_MORAL = EntityFederal.PFF_TRABAJO_SOCIAL.AVAL_MORAL,
                                        AVAL_MORAL_PARENTESCO = EntityFederal.PFF_TRABAJO_SOCIAL.AVAL_MORAL_PARENTESCO,
                                        EXTERNADO_CALLE = EntityFederal.PFF_TRABAJO_SOCIAL.EXTERNADO_CALLE,
                                        EXTERNADO_CIUDAD = EntityFederal.PFF_TRABAJO_SOCIAL.EXTERNADO_CIUDAD.HasValue ? EntityFederal.PFF_TRABAJO_SOCIAL.EXTERNADO_CIUDAD.Value != -1 ? EntityFederal.PFF_TRABAJO_SOCIAL.EXTERNADO_CIUDAD.Value != decimal.Zero ? EntityFederal.PFF_TRABAJO_SOCIAL.EXTERNADO_CIUDAD : new short?() : new short?() : new short?(),
                                        EXTERNADO_COLONIA = EntityFederal.PFF_TRABAJO_SOCIAL.EXTERNADO_COLONIA.HasValue ? EntityFederal.PFF_TRABAJO_SOCIAL.EXTERNADO_COLONIA.Value != -1 ? EntityFederal.PFF_TRABAJO_SOCIAL.EXTERNADO_COLONIA.Value != decimal.Zero ? EntityFederal.PFF_TRABAJO_SOCIAL.EXTERNADO_COLONIA : new int?() : new int?() : new int?(),
                                        EXTERNADO_CP = EntityFederal.PFF_TRABAJO_SOCIAL.EXTERNADO_CP,
                                        EXTERNADO_ENTIDAD = EntityFederal.PFF_TRABAJO_SOCIAL.EXTERNADO_ENTIDAD.HasValue ? EntityFederal.PFF_TRABAJO_SOCIAL.EXTERNADO_ENTIDAD.Value != -1 ? EntityFederal.PFF_TRABAJO_SOCIAL.EXTERNADO_ENTIDAD.Value != decimal.Zero ? EntityFederal.PFF_TRABAJO_SOCIAL.EXTERNADO_ENTIDAD : new short?() : new short?() : new short?(),
                                        EXTERNADO_MUNICIPIO = EntityFederal.PFF_TRABAJO_SOCIAL.EXTERNADO_MUNICIPIO.HasValue ? EntityFederal.PFF_TRABAJO_SOCIAL.EXTERNADO_MUNICIPIO.Value != -1 ? EntityFederal.PFF_TRABAJO_SOCIAL.EXTERNADO_MUNICIPIO.Value != decimal.Zero ? EntityFederal.PFF_TRABAJO_SOCIAL.EXTERNADO_MUNICIPIO : new short?() : new short?() : new short?(),
                                        EXTERNADO_NUMERO = EntityFederal.PFF_TRABAJO_SOCIAL.EXTERNADO_NUMERO,
                                        EXTERNADO_PARENTESCO = EntityFederal.PFF_TRABAJO_SOCIAL.EXTERNADO_PARENTESCO.HasValue ? EntityFederal.PFF_TRABAJO_SOCIAL.EXTERNADO_PARENTESCO.Value != -1 ? EntityFederal.PFF_TRABAJO_SOCIAL.EXTERNADO_PARENTESCO : null : null,
                                        EXTERNADO_VIVIR_NOMBRE = EntityFederal.PFF_TRABAJO_SOCIAL.EXTERNADO_VIVIR_NOMBRE,
                                        RADICAN_ESTADO = EntityFederal.PFF_TRABAJO_SOCIAL.RADICAN_ESTADO,
                                        VISITA_FRECUENCIA = EntityFederal.PFF_TRABAJO_SOCIAL.VISITA_FRECUENCIA,
                                        FECHA = EntityFederal.PFF_TRABAJO_SOCIAL.FECHA,
                                        FECHA_NAC = EntityFederal.PFF_TRABAJO_SOCIAL.FECHA_NAC,
                                        ID_ANIO = EntityFederal.ID_ANIO,
                                        ID_ESTUDIO = EntityFederal.ID_ESTUDIO,
                                        ID_CENTRO = EntityFederal.ID_CENTRO,
                                        ID_IMPUTADO = EntityFederal.ID_IMPUTADO,
                                        ID_INGRESO = EntityFederal.ID_INGRESO,
                                        INFLUENCIADO_ESTANCIA_PRISION = EntityFederal.PFF_TRABAJO_SOCIAL.INFLUENCIADO_ESTANCIA_PRISION,
                                        LUGAR = EntityFederal.PFF_TRABAJO_SOCIAL.LUGAR,
                                        LUGAR_NAC = EntityFederal.PFF_TRABAJO_SOCIAL.LUGAR_NAC,
                                        NOMBRE = EntityFederal.PFF_TRABAJO_SOCIAL.NOMBRE,
                                        NUM_PAREJAS_ESTABLE = EntityFederal.PFF_TRABAJO_SOCIAL.NUM_PAREJAS_ESTABLE,
                                        OCUPACION_ANT = EntityFederal.PFF_TRABAJO_SOCIAL.OCUPACION_ANT != -1 ? EntityFederal.PFF_TRABAJO_SOCIAL.OCUPACION_ANT : null,
                                        OFERTA_TRABAJO = EntityFederal.PFF_TRABAJO_SOCIAL.OFERTA_TRABAJO,
                                        OFERTA_TRABAJO_CONSISTE = EntityFederal.PFF_TRABAJO_SOCIAL.OFERTA_TRABAJO_CONSISTE,
                                        OPINION_CONCESION_BENEFICIOS = EntityFederal.PFF_TRABAJO_SOCIAL.OPINION_CONCESION_BENEFICIOS,
                                        OPINION_INTERNAMIENTO = EntityFederal.PFF_TRABAJO_SOCIAL.OPINION_INTERNAMIENTO,
                                        SERVICIOS_PUBLICOS = EntityFederal.PFF_TRABAJO_SOCIAL.SERVICIOS_PUBLICOS,
                                        SUELDO_PERCIBIDO = EntityFederal.PFF_TRABAJO_SOCIAL.SUELDO_PERCIBIDO,
                                        TIEMPO_LABORAR = EntityFederal.PFF_TRABAJO_SOCIAL.TIEMPO_LABORAR,
                                        TRABAJADORA_SOCIAL = EntityFederal.PFF_TRABAJO_SOCIAL.TRABAJADORA_SOCIAL,
                                        TRABAJO_DESEMP_ANTES = EntityFederal.PFF_TRABAJO_SOCIAL.TRABAJO_DESEMP_ANTES,
                                        VISITA_FAMILIARES = EntityFederal.PFF_TRABAJO_SOCIAL.VISITA_FAMILIARES,
                                        VISITA_OTROS_QUIIEN = EntityFederal.PFF_TRABAJO_SOCIAL.VISITA_OTROS_QUIIEN,
                                        VISITAS_OTROS = EntityFederal.PFF_TRABAJO_SOCIAL.VISITAS_OTROS,
                                        VISTA_PARENTESCO = EntityFederal.PFF_TRABAJO_SOCIAL.VISTA_PARENTESCO.HasValue ? EntityFederal.PFF_TRABAJO_SOCIAL.VISTA_PARENTESCO.Value != -1 ? EntityFederal.PFF_TRABAJO_SOCIAL.VISTA_PARENTESCO : null : null
                                    };

                                    #region Grupos TS
                                    if (_GruposFam != null && _GruposFam.Any())
                                        foreach (var item in _GruposFam)
                                            Context.Entry(item).State = System.Data.EntityState.Deleted;

                                    var _consecutivoSocialFederal = GetIDProceso<short>("PFF_GRUPO_FAMILIAR", "ID_CONSEC", string.Format("ID_CENTRO = {0} AND ID_ANIO = {1} AND ID_IMPUTADO = {2} AND ID_INGRESO = {3} AND ID_ESTUDIO = {4}", EntityFederal.ID_CENTRO, EntityFederal.ID_ANIO, EntityFederal.ID_IMPUTADO, EntityFederal.ID_INGRESO, EntityFederal.ID_ESTUDIO));
                                    if (EntityFederal.PFF_TRABAJO_SOCIAL.PFF_GRUPO_FAMILIAR != null && EntityFederal.PFF_TRABAJO_SOCIAL.PFF_GRUPO_FAMILIAR.Any())
                                        foreach (var item in EntityFederal.PFF_TRABAJO_SOCIAL.PFF_GRUPO_FAMILIAR)
                                        {
                                            var _NvoGrupo = new PFF_GRUPO_FAMILIAR()
                                            {
                                                EDAD = item.EDAD,
                                                ESTADO_CIVIL = item.ESTADO_CIVIL,
                                                ID_ANIO = EntityFederal.ID_ANIO,
                                                ID_CENTRO = EntityFederal.ID_CENTRO,
                                                ID_ESTUDIO = item.ID_ESTUDIO,
                                                ID_GRUPO_FAMILIAR = item.ID_GRUPO_FAMILIAR,
                                                ID_IMPUTADO = EntityFederal.ID_IMPUTADO,
                                                ID_INGRESO = item.ID_INGRESO,
                                                NOMBRE = item.NOMBRE,
                                                OCUPACION = item.OCUPACION,
                                                PARENTESCO = item.PARENTESCO,
                                                ID_CONSEC = _consecutivoSocialFederal,
                                                ID_GRADO = item.ID_GRADO
                                            };

                                            ValidaEstudio.PFF_TRABAJO_SOCIAL.PFF_GRUPO_FAMILIAR.Add(_NvoGrupo);
                                            _consecutivoSocialFederal++;
                                        };

                                    #endregion

                                    Context.PFF_TRABAJO_SOCIAL.Add(ValidaEstudio.PFF_TRABAJO_SOCIAL);
                                }

                                else
                                {
                                    _EstudioTrabajoSocialHecho.ALIMENTACION_FAM = EntityFederal.PFF_TRABAJO_SOCIAL.ALIMENTACION_FAM;
                                    _EstudioTrabajoSocialHecho.APORTACIONES_FAM = EntityFederal.PFF_TRABAJO_SOCIAL.APORTACIONES_FAM;
                                    _EstudioTrabajoSocialHecho.APOYO_FAM_OTROS = EntityFederal.PFF_TRABAJO_SOCIAL.APOYO_FAM_OTROS;
                                    _EstudioTrabajoSocialHecho.CARACT_FP_ANTECE_PENALES_ADIC = EntityFederal.PFF_TRABAJO_SOCIAL.CARACT_FP_ANTECE_PENALES_ADIC;
                                    _EstudioTrabajoSocialHecho.CARACT_FP_ANTECEDENTES_PENALES = EntityFederal.PFF_TRABAJO_SOCIAL.CARACT_FP_ANTECEDENTES_PENALES;
                                    _EstudioTrabajoSocialHecho.CARACT_FP_CONCEPTO = EntityFederal.PFF_TRABAJO_SOCIAL.CARACT_FP_CONCEPTO;
                                    _EstudioTrabajoSocialHecho.CARACT_FP_GRUPO = EntityFederal.PFF_TRABAJO_SOCIAL.CARACT_FP_GRUPO;
                                    _EstudioTrabajoSocialHecho.CARACT_FP_NIVEL_SOCIO_CULTURAL = EntityFederal.PFF_TRABAJO_SOCIAL.CARACT_FP_NIVEL_SOCIO_CULTURAL;
                                    _EstudioTrabajoSocialHecho.CARACT_FP_RELAC_INTERFAM = EntityFederal.PFF_TRABAJO_SOCIAL.CARACT_FP_RELAC_INTERFAM;
                                    _EstudioTrabajoSocialHecho.CARACT_FP_VIOLENCIA_FAM = EntityFederal.PFF_TRABAJO_SOCIAL.CARACT_FP_VIOLENCIA_FAM;
                                    _EstudioTrabajoSocialHecho.CARACT_FP_VIOLENCIA_FAM_ESPEFI = EntityFederal.PFF_TRABAJO_SOCIAL.CARACT_FP_VIOLENCIA_FAM_ESPEFI;
                                    _EstudioTrabajoSocialHecho.CARACT_FS_GRUPO = EntityFederal.PFF_TRABAJO_SOCIAL.CARACT_FS_GRUPO;
                                    _EstudioTrabajoSocialHecho.CARACT_FS_HIJOS_ANT = EntityFederal.PFF_TRABAJO_SOCIAL.CARACT_FS_HIJOS_ANT;
                                    _EstudioTrabajoSocialHecho.CARACT_FS_NIVEL_SOCIO_CULTURAL = EntityFederal.PFF_TRABAJO_SOCIAL.CARACT_FS_NIVEL_SOCIO_CULTURAL;
                                    _EstudioTrabajoSocialHecho.CARACT_FS_PROBLEMAS_CONDUCTA = EntityFederal.PFF_TRABAJO_SOCIAL.CARACT_FS_PROBLEMAS_CONDUCTA;
                                    _EstudioTrabajoSocialHecho.CARACT_FS_PROBLEMAS_CONDUCTA_E = EntityFederal.PFF_TRABAJO_SOCIAL.CARACT_FS_PROBLEMAS_CONDUCTA_E;
                                    _EstudioTrabajoSocialHecho.CARACT_FS_RELACION_MEDIO_EXT = EntityFederal.PFF_TRABAJO_SOCIAL.CARACT_FS_RELACION_MEDIO_EXT;
                                    _EstudioTrabajoSocialHecho.CARACT_FS_RELACIONES_INTERFAM = EntityFederal.PFF_TRABAJO_SOCIAL.CARACT_FS_RELACIONES_INTERFAM;
                                    _EstudioTrabajoSocialHecho.CARACT_FS_VIOLENCIA_INTRAFAM = EntityFederal.PFF_TRABAJO_SOCIAL.CARACT_FS_VIOLENCIA_INTRAFAM;
                                    _EstudioTrabajoSocialHecho.CARACT_FS_VIOLENCIA_INTRAFAM_E = EntityFederal.PFF_TRABAJO_SOCIAL.CARACT_FS_VIOLENCIA_INTRAFAM_E;
                                    _EstudioTrabajoSocialHecho.CARACT_FS_VIVIEN_DESCRIPCION = EntityFederal.PFF_TRABAJO_SOCIAL.CARACT_FS_VIVIEN_DESCRIPCION;
                                    _EstudioTrabajoSocialHecho.CARACT_FS_VIVIEN_MOBILIARIO = EntityFederal.PFF_TRABAJO_SOCIAL.CARACT_FS_VIVIEN_MOBILIARIO;
                                    _EstudioTrabajoSocialHecho.CARACT_FS_VIVIEN_NUM_HABITACIO = EntityFederal.PFF_TRABAJO_SOCIAL.CARACT_FS_VIVIEN_NUM_HABITACIO;
                                    _EstudioTrabajoSocialHecho.CARACT_FS_VIVIEN_TRANSPORTE = EntityFederal.PFF_TRABAJO_SOCIAL.CARACT_FS_VIVIEN_TRANSPORTE;
                                    _EstudioTrabajoSocialHecho.CARACT_FS_ZONA = EntityFederal.PFF_TRABAJO_SOCIAL.CARACT_FS_ZONA;
                                    _EstudioTrabajoSocialHecho.DIAG_SOCIAL_PRONOS = EntityFederal.PFF_TRABAJO_SOCIAL.DIAG_SOCIAL_PRONOS;
                                    _EstudioTrabajoSocialHecho.DIALECTO = EntityFederal.PFF_TRABAJO_SOCIAL.DIALECTO != -1 ? EntityFederal.PFF_TRABAJO_SOCIAL.DIALECTO : null;
                                    _EstudioTrabajoSocialHecho.DIRECTOR_CENTRO = EntityFederal.PFF_TRABAJO_SOCIAL.DIRECTOR_CENTRO;
                                    _EstudioTrabajoSocialHecho.DISTRIBUCION_GASTO_FAM = EntityFederal.PFF_TRABAJO_SOCIAL.DISTRIBUCION_GASTO_FAM;
                                    _EstudioTrabajoSocialHecho.DOMICILIO = EntityFederal.PFF_TRABAJO_SOCIAL.DOMICILIO;
                                    _EstudioTrabajoSocialHecho.ECO_FP_COOPERA_ACTUALMENTE = EntityFederal.PFF_TRABAJO_SOCIAL.ECO_FP_COOPERA_ACTUALMENTE;
                                    _EstudioTrabajoSocialHecho.ECO_FP_FONDOS_AHORRO = EntityFederal.PFF_TRABAJO_SOCIAL.ECO_FP_FONDOS_AHORRO;
                                    _EstudioTrabajoSocialHecho.ECO_FP_RESPONSABLE = EntityFederal.PFF_TRABAJO_SOCIAL.ECO_FP_RESPONSABLE;
                                    _EstudioTrabajoSocialHecho.ECO_FP_TOTAL_EGRESOS_MEN = EntityFederal.PFF_TRABAJO_SOCIAL.ECO_FP_TOTAL_EGRESOS_MEN;
                                    _EstudioTrabajoSocialHecho.ECO_FP_TOTAL_INGRESOS_MEN = EntityFederal.PFF_TRABAJO_SOCIAL.ECO_FP_TOTAL_INGRESOS_MEN;
                                    _EstudioTrabajoSocialHecho.ECO_FP_ZONA = EntityFederal.PFF_TRABAJO_SOCIAL.ECO_FP_ZONA;
                                    _EstudioTrabajoSocialHecho.EDO_CIVIL = EntityFederal.PFF_TRABAJO_SOCIAL.EDO_CIVIL != -1 ? EntityFederal.PFF_TRABAJO_SOCIAL.EDO_CIVIL : null;
                                    _EstudioTrabajoSocialHecho.ESCOLARIDAD_ACTUAL = EntityFederal.PFF_TRABAJO_SOCIAL.ESCOLARIDAD_ACTUAL != -1 ? EntityFederal.PFF_TRABAJO_SOCIAL.ESCOLARIDAD_ACTUAL : null;
                                    _EstudioTrabajoSocialHecho.ESCOLARIDAD_CENTRO = EntityFederal.PFF_TRABAJO_SOCIAL.ESCOLARIDAD_CENTRO != -1 ? EntityFederal.PFF_TRABAJO_SOCIAL.ESCOLARIDAD_CENTRO : null;
                                    _EstudioTrabajoSocialHecho.AVAL_MORAL = EntityFederal.PFF_TRABAJO_SOCIAL.AVAL_MORAL;
                                    _EstudioTrabajoSocialHecho.AVAL_MORAL_PARENTESCO = EntityFederal.PFF_TRABAJO_SOCIAL.AVAL_MORAL_PARENTESCO;
                                    _EstudioTrabajoSocialHecho.EXTERNADO_CALLE = EntityFederal.PFF_TRABAJO_SOCIAL.EXTERNADO_CALLE;
                                    _EstudioTrabajoSocialHecho.EXTERNADO_CIUDAD = EntityFederal.PFF_TRABAJO_SOCIAL.EXTERNADO_CIUDAD.HasValue ? EntityFederal.PFF_TRABAJO_SOCIAL.EXTERNADO_CIUDAD.Value != -1 ? EntityFederal.PFF_TRABAJO_SOCIAL.EXTERNADO_CIUDAD.Value != decimal.Zero ? EntityFederal.PFF_TRABAJO_SOCIAL.EXTERNADO_CIUDAD : new short?() : new short?() : new short?();
                                    _EstudioTrabajoSocialHecho.EXTERNADO_COLONIA = EntityFederal.PFF_TRABAJO_SOCIAL.EXTERNADO_COLONIA.HasValue ? EntityFederal.PFF_TRABAJO_SOCIAL.EXTERNADO_COLONIA.Value != -1 ? EntityFederal.PFF_TRABAJO_SOCIAL.EXTERNADO_COLONIA.Value != decimal.Zero ? EntityFederal.PFF_TRABAJO_SOCIAL.EXTERNADO_COLONIA : new int?() : new int?() : new int?();
                                    _EstudioTrabajoSocialHecho.EXTERNADO_CP = EntityFederal.PFF_TRABAJO_SOCIAL.EXTERNADO_CP;
                                    _EstudioTrabajoSocialHecho.EXTERNADO_ENTIDAD = EntityFederal.PFF_TRABAJO_SOCIAL.EXTERNADO_ENTIDAD.HasValue ? EntityFederal.PFF_TRABAJO_SOCIAL.EXTERNADO_ENTIDAD.Value != -1 ? EntityFederal.PFF_TRABAJO_SOCIAL.EXTERNADO_ENTIDAD.Value != decimal.Zero ? EntityFederal.PFF_TRABAJO_SOCIAL.EXTERNADO_ENTIDAD : new short?() : new short?() : new short?();
                                    _EstudioTrabajoSocialHecho.EXTERNADO_MUNICIPIO = EntityFederal.PFF_TRABAJO_SOCIAL.EXTERNADO_MUNICIPIO.HasValue ? EntityFederal.PFF_TRABAJO_SOCIAL.EXTERNADO_MUNICIPIO.Value != -1 ? EntityFederal.PFF_TRABAJO_SOCIAL.EXTERNADO_MUNICIPIO.Value != decimal.Zero ? EntityFederal.PFF_TRABAJO_SOCIAL.EXTERNADO_MUNICIPIO : new short?() : new short?() : new short?();
                                    _EstudioTrabajoSocialHecho.EXTERNADO_NUMERO = EntityFederal.PFF_TRABAJO_SOCIAL.EXTERNADO_NUMERO;
                                    _EstudioTrabajoSocialHecho.EXTERNADO_PARENTESCO = EntityFederal.PFF_TRABAJO_SOCIAL.EXTERNADO_PARENTESCO.HasValue ? EntityFederal.PFF_TRABAJO_SOCIAL.EXTERNADO_PARENTESCO.Value != -1 ? EntityFederal.PFF_TRABAJO_SOCIAL.EXTERNADO_PARENTESCO : null : null;
                                    _EstudioTrabajoSocialHecho.EXTERNADO_VIVIR_NOMBRE = EntityFederal.PFF_TRABAJO_SOCIAL.EXTERNADO_VIVIR_NOMBRE;
                                    _EstudioTrabajoSocialHecho.RADICAN_ESTADO = EntityFederal.PFF_TRABAJO_SOCIAL.RADICAN_ESTADO;
                                    _EstudioTrabajoSocialHecho.VISITA_FRECUENCIA = EntityFederal.PFF_TRABAJO_SOCIAL.VISITA_FRECUENCIA;
                                    _EstudioTrabajoSocialHecho.FECHA = EntityFederal.PFF_TRABAJO_SOCIAL.FECHA;
                                    _EstudioTrabajoSocialHecho.FECHA_NAC = EntityFederal.PFF_TRABAJO_SOCIAL.FECHA_NAC;
                                    _EstudioTrabajoSocialHecho.ID_ANIO = EntityFederal.ID_ANIO;
                                    _EstudioTrabajoSocialHecho.ID_CENTRO = EntityFederal.ID_CENTRO;
                                    _EstudioTrabajoSocialHecho.ID_IMPUTADO = EntityFederal.ID_IMPUTADO;
                                    _EstudioTrabajoSocialHecho.ID_INGRESO = EntityFederal.ID_INGRESO;
                                    _EstudioTrabajoSocialHecho.ID_ESTUDIO = EntityFederal.ID_ESTUDIO;
                                    _EstudioTrabajoSocialHecho.INFLUENCIADO_ESTANCIA_PRISION = EntityFederal.PFF_TRABAJO_SOCIAL.INFLUENCIADO_ESTANCIA_PRISION;
                                    _EstudioTrabajoSocialHecho.LUGAR = EntityFederal.PFF_TRABAJO_SOCIAL.LUGAR;
                                    _EstudioTrabajoSocialHecho.LUGAR_NAC = EntityFederal.PFF_TRABAJO_SOCIAL.LUGAR_NAC;
                                    _EstudioTrabajoSocialHecho.NOMBRE = EntityFederal.PFF_TRABAJO_SOCIAL.NOMBRE;
                                    _EstudioTrabajoSocialHecho.NUM_PAREJAS_ESTABLE = EntityFederal.PFF_TRABAJO_SOCIAL.NUM_PAREJAS_ESTABLE;
                                    _EstudioTrabajoSocialHecho.OCUPACION_ANT = EntityFederal.PFF_TRABAJO_SOCIAL.OCUPACION_ANT != -1 ? EntityFederal.PFF_TRABAJO_SOCIAL.OCUPACION_ANT : null;
                                    _EstudioTrabajoSocialHecho.OFERTA_TRABAJO = EntityFederal.PFF_TRABAJO_SOCIAL.OFERTA_TRABAJO;
                                    _EstudioTrabajoSocialHecho.OFERTA_TRABAJO_CONSISTE = EntityFederal.PFF_TRABAJO_SOCIAL.OFERTA_TRABAJO_CONSISTE;
                                    _EstudioTrabajoSocialHecho.OPINION_CONCESION_BENEFICIOS = EntityFederal.PFF_TRABAJO_SOCIAL.OPINION_CONCESION_BENEFICIOS;
                                    _EstudioTrabajoSocialHecho.OPINION_INTERNAMIENTO = EntityFederal.PFF_TRABAJO_SOCIAL.OPINION_INTERNAMIENTO;
                                    _EstudioTrabajoSocialHecho.SERVICIOS_PUBLICOS = EntityFederal.PFF_TRABAJO_SOCIAL.SERVICIOS_PUBLICOS;
                                    _EstudioTrabajoSocialHecho.SUELDO_PERCIBIDO = EntityFederal.PFF_TRABAJO_SOCIAL.SUELDO_PERCIBIDO;
                                    _EstudioTrabajoSocialHecho.TIEMPO_LABORAR = EntityFederal.PFF_TRABAJO_SOCIAL.TIEMPO_LABORAR;
                                    _EstudioTrabajoSocialHecho.TRABAJADORA_SOCIAL = EntityFederal.PFF_TRABAJO_SOCIAL.TRABAJADORA_SOCIAL;
                                    _EstudioTrabajoSocialHecho.TRABAJO_DESEMP_ANTES = EntityFederal.PFF_TRABAJO_SOCIAL.TRABAJO_DESEMP_ANTES;
                                    _EstudioTrabajoSocialHecho.VISITA_FAMILIARES = EntityFederal.PFF_TRABAJO_SOCIAL.VISITA_FAMILIARES;
                                    _EstudioTrabajoSocialHecho.VISITA_OTROS_QUIIEN = EntityFederal.PFF_TRABAJO_SOCIAL.VISITA_OTROS_QUIIEN;
                                    _EstudioTrabajoSocialHecho.VISITAS_OTROS = EntityFederal.PFF_TRABAJO_SOCIAL.VISITAS_OTROS;
                                    _EstudioTrabajoSocialHecho.VISTA_PARENTESCO = EntityFederal.PFF_TRABAJO_SOCIAL.VISTA_PARENTESCO.HasValue ? EntityFederal.PFF_TRABAJO_SOCIAL.VISTA_PARENTESCO.Value != -1 ? EntityFederal.PFF_TRABAJO_SOCIAL.VISTA_PARENTESCO : null : null;
                                    Context.Entry(_EstudioTrabajoSocialHecho).State = System.Data.EntityState.Modified;

                                    #region Gru TS
                                    if (_GruposFam != null && _GruposFam.Any())
                                        foreach (var item in _GruposFam)
                                            Context.Entry(item).State = System.Data.EntityState.Deleted;

                                    var _consecutivoSocialFederal = GetIDProceso<short>("PFF_GRUPO_FAMILIAR", "ID_CONSEC", string.Format("ID_CENTRO = {0} AND ID_ANIO = {1} AND ID_IMPUTADO = {2} AND ID_INGRESO = {3} AND ID_ESTUDIO = {4}", EntityFederal.ID_CENTRO, EntityFederal.ID_ANIO, EntityFederal.ID_IMPUTADO, EntityFederal.ID_INGRESO, EntityFederal.ID_ESTUDIO));
                                    if (EntityFederal.PFF_TRABAJO_SOCIAL.PFF_GRUPO_FAMILIAR != null && EntityFederal.PFF_TRABAJO_SOCIAL.PFF_GRUPO_FAMILIAR.Any())
                                        foreach (var item in EntityFederal.PFF_TRABAJO_SOCIAL.PFF_GRUPO_FAMILIAR)
                                        {
                                            var _NvoGrupo = new PFF_GRUPO_FAMILIAR()
                                            {
                                                EDAD = item.EDAD,
                                                ESTADO_CIVIL = item.ESTADO_CIVIL,
                                                ID_ANIO = EntityFederal.ID_ANIO,
                                                ID_CENTRO = EntityFederal.ID_CENTRO,
                                                ID_ESTUDIO = item.ID_ESTUDIO,
                                                ID_GRUPO_FAMILIAR = item.ID_GRUPO_FAMILIAR,
                                                ID_IMPUTADO = EntityFederal.ID_IMPUTADO,
                                                ID_INGRESO = item.ID_INGRESO,
                                                NOMBRE = item.NOMBRE,
                                                OCUPACION = item.OCUPACION,
                                                ID_GRADO = item.ID_GRADO,
                                                PARENTESCO = item.PARENTESCO,
                                                ID_CONSEC = _consecutivoSocialFederal
                                            };

                                            ValidaEstudio.PFF_TRABAJO_SOCIAL.PFF_GRUPO_FAMILIAR.Add(_NvoGrupo);
                                            _consecutivoSocialFederal++;
                                        };
                                    #endregion

                                };


                                #region Definicion de detalle de estudio de trabajo social de fuero federal
                                if (_EstudioPadre != null)
                                {
                                    short? _Estatus = new short?();
                                    if (string.IsNullOrEmpty(EntityFederal.PFF_TRABAJO_SOCIAL.ALIMENTACION_FAM) || string.IsNullOrEmpty(EntityFederal.PFF_TRABAJO_SOCIAL.APORTACIONES_FAM) || string.IsNullOrEmpty(EntityFederal.PFF_TRABAJO_SOCIAL.AVAL_MORAL) || string.IsNullOrEmpty(EntityFederal.PFF_TRABAJO_SOCIAL.CARACT_FP_CONCEPTO) || string.IsNullOrEmpty(EntityFederal.PFF_TRABAJO_SOCIAL.CARACT_FP_GRUPO) ||
                                        string.IsNullOrEmpty(EntityFederal.PFF_TRABAJO_SOCIAL.CARACT_FP_NIVEL_SOCIO_CULTURAL) || string.IsNullOrEmpty(EntityFederal.PFF_TRABAJO_SOCIAL.CARACT_FP_RELAC_INTERFAM) ||
                                        string.IsNullOrEmpty(EntityFederal.PFF_TRABAJO_SOCIAL.CARACT_FP_VIOLENCIA_FAM) || string.IsNullOrEmpty(EntityFederal.PFF_TRABAJO_SOCIAL.CARACT_FS_GRUPO) ||
                                        string.IsNullOrEmpty(EntityFederal.PFF_TRABAJO_SOCIAL.CARACT_FS_HIJOS_ANT) || string.IsNullOrEmpty(EntityFederal.PFF_TRABAJO_SOCIAL.CARACT_FS_NIVEL_SOCIO_CULTURAL) ||
                                        string.IsNullOrEmpty(EntityFederal.PFF_TRABAJO_SOCIAL.CARACT_FS_PROBLEMAS_CONDUCTA) || string.IsNullOrEmpty(EntityFederal.PFF_TRABAJO_SOCIAL.CARACT_FS_RELACION_MEDIO_EXT) ||
                                        string.IsNullOrEmpty(EntityFederal.PFF_TRABAJO_SOCIAL.CARACT_FS_RELACIONES_INTERFAM) || string.IsNullOrEmpty(EntityFederal.PFF_TRABAJO_SOCIAL.CARACT_FS_VIOLENCIA_INTRAFAM) ||
                                        string.IsNullOrEmpty(EntityFederal.PFF_TRABAJO_SOCIAL.CARACT_FS_VIVIEN_DESCRIPCION) || string.IsNullOrEmpty(EntityFederal.PFF_TRABAJO_SOCIAL.CARACT_FS_VIVIEN_MOBILIARIO) ||
                                        string.IsNullOrEmpty(EntityFederal.PFF_TRABAJO_SOCIAL.CARACT_FS_VIVIEN_TRANSPORTE) || string.IsNullOrEmpty(EntityFederal.PFF_TRABAJO_SOCIAL.CARACT_FS_ZONA) ||
                                        string.IsNullOrEmpty(EntityFederal.PFF_TRABAJO_SOCIAL.DIAG_SOCIAL_PRONOS) || EntityFederal.PFF_TRABAJO_SOCIAL.DIALECTO == null || string.IsNullOrEmpty(EntityFederal.PFF_TRABAJO_SOCIAL.DISTRIBUCION_GASTO_FAM) ||
                                        string.IsNullOrEmpty(EntityFederal.PFF_TRABAJO_SOCIAL.DOMICILIO) || string.IsNullOrEmpty(EntityFederal.PFF_TRABAJO_SOCIAL.ECO_FP_COOPERA_ACTUALMENTE) ||
                                        string.IsNullOrEmpty(EntityFederal.PFF_TRABAJO_SOCIAL.ECO_FP_FONDOS_AHORRO) || string.IsNullOrEmpty(EntityFederal.PFF_TRABAJO_SOCIAL.ECO_FP_RESPONSABLE) ||
                                        string.IsNullOrEmpty(EntityFederal.PFF_TRABAJO_SOCIAL.ECO_FP_ZONA) || EntityFederal.PFF_TRABAJO_SOCIAL.ESCOLARIDAD_ACTUAL == null || EntityFederal.PFF_TRABAJO_SOCIAL.ESCOLARIDAD_CENTRO == null ||
                                        string.IsNullOrEmpty(EntityFederal.PFF_TRABAJO_SOCIAL.EXTERNADO_CALLE) || string.IsNullOrEmpty(EntityFederal.PFF_TRABAJO_SOCIAL.EXTERNADO_CP) ||
                                        string.IsNullOrEmpty(EntityFederal.PFF_TRABAJO_SOCIAL.EXTERNADO_NUMERO) || string.IsNullOrEmpty(EntityFederal.PFF_TRABAJO_SOCIAL.EXTERNADO_VIVIR_NOMBRE) ||
                                        string.IsNullOrEmpty(EntityFederal.PFF_TRABAJO_SOCIAL.INFLUENCIADO_ESTANCIA_PRISION) || string.IsNullOrEmpty(EntityFederal.PFF_TRABAJO_SOCIAL.LUGAR_NAC) ||
                                        string.IsNullOrEmpty(EntityFederal.PFF_TRABAJO_SOCIAL.NOMBRE) || string.IsNullOrEmpty(EntityFederal.PFF_TRABAJO_SOCIAL.NUM_PAREJAS_ESTABLE) ||
                                        EntityFederal.PFF_TRABAJO_SOCIAL.OCUPACION_ANT == null || string.IsNullOrEmpty(EntityFederal.PFF_TRABAJO_SOCIAL.OFERTA_TRABAJO) ||
                                        string.IsNullOrEmpty(EntityFederal.PFF_TRABAJO_SOCIAL.OPINION_CONCESION_BENEFICIOS) || string.IsNullOrEmpty(EntityFederal.PFF_TRABAJO_SOCIAL.OPINION_INTERNAMIENTO) ||
                                        string.IsNullOrEmpty(EntityFederal.PFF_TRABAJO_SOCIAL.SERVICIOS_PUBLICOS) || string.IsNullOrEmpty(EntityFederal.PFF_TRABAJO_SOCIAL.TIEMPO_LABORAR) ||
                                        string.IsNullOrEmpty(EntityFederal.PFF_TRABAJO_SOCIAL.TRABAJO_DESEMP_ANTES) || string.IsNullOrEmpty(EntityFederal.PFF_TRABAJO_SOCIAL.VISITA_FAMILIARES) ||
                                        EntityFederal.PFF_TRABAJO_SOCIAL.CARACT_FS_VIVIEN_NUM_HABITACIO == null || EntityFederal.PFF_TRABAJO_SOCIAL.ECO_FP_TOTAL_EGRESOS_MEN == null || EntityFederal.PFF_TRABAJO_SOCIAL.ECO_FP_TOTAL_INGRESOS_MEN == null ||
                                        EntityFederal.PFF_TRABAJO_SOCIAL.EDO_CIVIL == null || EntityFederal.PFF_TRABAJO_SOCIAL.EXTERNADO_CIUDAD == null ||
                                        EntityFederal.PFF_TRABAJO_SOCIAL.EXTERNADO_COLONIA == null || EntityFederal.PFF_TRABAJO_SOCIAL.EXTERNADO_ENTIDAD == null || EntityFederal.PFF_TRABAJO_SOCIAL.EXTERNADO_MUNICIPIO == null ||
                                        EntityFederal.PFF_TRABAJO_SOCIAL.EXTERNADO_PARENTESCO == null || EntityFederal.PFF_TRABAJO_SOCIAL.FECHA == null || EntityFederal.PFF_TRABAJO_SOCIAL.FECHA_NAC == null ||
                                        EntityFederal.PFF_TRABAJO_SOCIAL.SUELDO_PERCIBIDO == null)
                                        _Estatus = (short)eEstatudDetallePersonalidad.ASIGNADO;
                                    else
                                        _Estatus = (short)eEstatudDetallePersonalidad.TERMINADO;

                                    var _DesarrolloEstudioTSFederal = Context.PERSONALIDAD_DETALLE.FirstOrDefault(x => x.ID_IMPUTADO == EntityFederal.ID_IMPUTADO && x.ID_ESTUDIO == _EstudioPadre.ID_ESTUDIO && x.ID_INGRESO == EntityFederal.ID_INGRESO && x.ID_TIPO == (short)eTiposEstudio.TRABAJO_SOCIAL_FEDERAL && x.ID_CENTRO == EntityFederal.ID_CENTRO && x.ID_ANIO == EntityFederal.ID_ANIO);
                                    if (_DesarrolloEstudioTSFederal == null)
                                    {
                                        short _ConsecutivoPersonalidadDetalle = GetIDProceso<short>("PERSONALIDAD_DETALLE", "ID_DETALLE", "1=1");
                                        var NvoDetalle = new PERSONALIDAD_DETALLE()
                                        {
                                            ID_ANIO = EntityFederal.ID_ANIO,
                                            ID_CENTRO = EntityFederal.ID_CENTRO,
                                            ID_DETALLE = _ConsecutivoPersonalidadDetalle,
                                            ID_ESTUDIO = _EstudioPadre.ID_ESTUDIO,
                                            ID_IMPUTADO = EntityFederal.ID_IMPUTADO,
                                            ID_ESTATUS = _Estatus,
                                            ID_INGRESO = EntityFederal.ID_INGRESO,
                                            ID_TIPO = (short)eTiposEstudio.TRABAJO_SOCIAL_FEDERAL,
                                            INICIO_FEC = GetFechaServerDate(),
                                            RESULTADO = _Estatus == (short)eEstatudDetallePersonalidad.TERMINADO ? "S" : "N",
                                            SOLICITUD_FEC = null,
                                            TERMINO_FEC = _Estatus == (short)eEstatudDetallePersonalidad.TERMINADO ? GetFechaServerDate() : new System.DateTime?(),
                                            TIPO_MEDIA = string.Empty
                                        };

                                        Context.PERSONALIDAD_DETALLE.Add(NvoDetalle);
                                    }
                                    else
                                    {
                                        _DesarrolloEstudioTSFederal.RESULTADO = _Estatus.HasValue ? _Estatus.Value == (short)eEstatudDetallePersonalidad.TERMINADO ? "S" : "N" : "N";
                                        _DesarrolloEstudioTSFederal.ID_ESTATUS = _Estatus;
                                        Context.Entry(_DesarrolloEstudioTSFederal).State = System.Data.EntityState.Modified;
                                    };
                                };
                                #endregion

                            };

                            if (EntityFederal.PFF_CAPACITACION != null)
                            {
                                var _EstudioCapacitacionFederal = Context.PFF_CAPACITACION.FirstOrDefault(x => x.ID_INGRESO == EntityFederal.ID_INGRESO && x.ID_IMPUTADO == EntityFederal.ID_IMPUTADO && x.ID_ESTUDIO == EntityFederal.ID_ESTUDIO && x.ID_ANIO == EntityFederal.ID_ANIO && x.ID_CENTRO == EntityFederal.ID_CENTRO);
                                var _DiasGuardados = Context.PFF_DIAS_LABORADO.Where(x => x.ID_INGRESO == EntityFederal.ID_INGRESO && x.ID_IMPUTADO == EntityFederal.ID_IMPUTADO && x.ID_ESTUDIO == EntityFederal.ID_ESTUDIO && x.ID_ANIO == EntityFederal.ID_ANIO && x.ID_CENTRO == EntityFederal.ID_CENTRO);
                                var _GruposCapac = Context.PFF_CAPACITACION_CURSO.Where(x => x.ID_INGRESO == EntityFederal.ID_INGRESO && x.ID_IMPUTADO == EntityFederal.ID_IMPUTADO && x.ID_ESTUDIO == EntityFederal.ID_ESTUDIO && x.ID_ANIO == EntityFederal.ID_ANIO && x.ID_CENTRO == EntityFederal.ID_CENTRO);
                                if (_EstudioCapacitacionFederal == null)
                                {
                                    var _consecutivoCapacitacionFederal = GetIDProceso<short>("PFF_CAPACITACION_CURSO", "ID_CONSEC", string.Format("ID_CENTRO = {0} AND ID_ANIO = {1} AND ID_IMPUTADO = {2} AND ID_INGRESO = {3} AND ID_ESTUDIO = {4}", EntityFederal.ID_CENTRO, EntityFederal.ID_ANIO, EntityFederal.ID_IMPUTADO, EntityFederal.ID_INGRESO, EntityFederal.ID_ESTUDIO));

                                    ValidaEstudio.PFF_CAPACITACION = new PFF_CAPACITACION()
                                    {
                                        A_TOTAL_DIAS_LABORADOS = EntityFederal.PFF_CAPACITACION.A_TOTAL_DIAS_LABORADOS,
                                        ACTITUDES_DESEMPENO_ACT = EntityFederal.PFF_CAPACITACION.ACTITUDES_DESEMPENO_ACT,
                                        ACTIVIDAD_PRODUC_ACTUAL = EntityFederal.PFF_CAPACITACION.ACTIVIDAD_PRODUC_ACTUAL,
                                        ATIENDE_INDICACIONES = EntityFederal.PFF_CAPACITACION.ATIENDE_INDICACIONES,
                                        B_DIAS_LABORADOS_OTROS_CERESOS = EntityFederal.PFF_CAPACITACION.B_DIAS_LABORADOS_OTROS_CERESOS,
                                        CAMBIO_ACTIVIDAD = EntityFederal.PFF_CAPACITACION.CAMBIO_ACTIVIDAD,
                                        CAMBIO_ACTIVIDAD_POR_QUE = EntityFederal.PFF_CAPACITACION.CAMBIO_ACTIVIDAD_POR_QUE,
                                        CONCLUSIONES = EntityFederal.PFF_CAPACITACION.CONCLUSIONES,
                                        DESCUIDADO_LABORES = EntityFederal.PFF_CAPACITACION.DESCUIDADO_LABORES,
                                        DIRECTOR_CENTRO = EntityFederal.PFF_CAPACITACION.DIRECTOR_CENTRO,
                                        FECHA = EntityFederal.PFF_CAPACITACION.FECHA,
                                        FONDO_AHORRO = EntityFederal.PFF_CAPACITACION.FONDO_AHORRO,
                                        FONDO_AHORRO_COMPESACION_ACTUA = EntityFederal.PFF_CAPACITACION.FONDO_AHORRO_COMPESACION_ACTUA,
                                        HA_PROGRESADO_OFICIO = EntityFederal.PFF_CAPACITACION.HA_PROGRESADO_OFICIO,
                                        ID_ANIO = EntityFederal.ID_ANIO,
                                        ID_CENTRO = EntityFederal.ID_CENTRO,
                                        ID_ESTUDIO = EntityFederal.ID_ESTUDIO,
                                        ID_IMPUTADO = EntityFederal.ID_IMPUTADO,
                                        ID_INGRESO = EntityFederal.ID_INGRESO,
                                        JEFE_SECC_INDUSTRIAL = EntityFederal.PFF_CAPACITACION.JEFE_SECC_INDUSTRIAL,
                                        LUGAR = EntityFederal.PFF_CAPACITACION.LUGAR,
                                        MOTIVO_TIEMPO_INTERRUP_ACT = EntityFederal.PFF_CAPACITACION.MOTIVO_TIEMPO_INTERRUP_ACT,
                                        NO_CURSOS_MOTIVO = EntityFederal.PFF_CAPACITACION.NO_CURSOS_MOTIVO,
                                        NOMBRE = EntityFederal.PFF_CAPACITACION.NOMBRE,
                                        OFICIO_ANTES_RECLUSION = EntityFederal.PFF_CAPACITACION.OFICIO_ANTES_RECLUSION.HasValue ? EntityFederal.PFF_CAPACITACION.OFICIO_ANTES_RECLUSION != -1 ? EntityFederal.PFF_CAPACITACION.OFICIO_ANTES_RECLUSION : null : null,
                                        RECIBIO_CONSTANCIA = EntityFederal.PFF_CAPACITACION.RECIBIO_CONSTANCIA,
                                        SALARIO_DEVENGABA_DETENCION = EntityFederal.PFF_CAPACITACION.SALARIO_DEVENGABA_DETENCION,
                                        SATISFACE_ACTIVIDAD = EntityFederal.PFF_CAPACITACION.SATISFACE_ACTIVIDAD,
                                        SECCION = EntityFederal.PFF_CAPACITACION.SECCION,
                                        TOTAL_A_B = EntityFederal.PFF_CAPACITACION.TOTAL_A_B
                                    };

                                    if (_DiasGuardados != null && _DiasGuardados.Any())
                                        foreach (var item in _DiasGuardados)
                                            Context.Entry(item).State = System.Data.EntityState.Deleted;

                                    if (_GruposCapac != null && _GruposCapac.Any())
                                        foreach (var item in _GruposCapac)
                                            Context.Entry(item).State = System.Data.EntityState.Deleted;

                                    if (EntityFederal.PFF_CAPACITACION.PFF_DIAS_LABORADO != null && EntityFederal.PFF_CAPACITACION.PFF_DIAS_LABORADO.Any())
                                        foreach (var item in EntityFederal.PFF_CAPACITACION.PFF_DIAS_LABORADO)
                                        {
                                            var _NvoCapac = new PFF_DIAS_LABORADO()
                                            {
                                                ANIO = item.ANIO,
                                                DIAS_TRABAJADOS = item.DIAS_TRABAJADOS,
                                                ID_ANIO = EntityFederal.ID_ANIO,
                                                ID_CENTRO = EntityFederal.ID_CENTRO,
                                                ID_ESTUDIO = EntityFederal.ID_ESTUDIO,
                                                ID_IMPUTADO = EntityFederal.ID_IMPUTADO,
                                                ID_INGRESO = EntityFederal.ID_INGRESO,
                                                MES = item.MES
                                            };

                                            ValidaEstudio.PFF_CAPACITACION.PFF_DIAS_LABORADO.Add(_NvoCapac);
                                        };

                                    if (EntityFederal.PFF_CAPACITACION.PFF_CAPACITACION_CURSO != null && EntityFederal.PFF_CAPACITACION.PFF_CAPACITACION_CURSO.Any())
                                        foreach (var item in EntityFederal.PFF_CAPACITACION.PFF_CAPACITACION_CURSO)
                                        {
                                            var _NvaCurso = new PFF_CAPACITACION_CURSO()
                                            {
                                                CURSO = item.CURSO,
                                                FECHA_INICIO = item.FECHA_INICIO,
                                                FECHA_TERMINO = item.FECHA_TERMINO,
                                                ID_ANIO = EntityFederal.ID_ANIO,
                                                ID_CENTRO = EntityFederal.ID_CENTRO,
                                                ID_CONSEC = _consecutivoCapacitacionFederal,
                                                ID_ESTUDIO = EntityFederal.ID_ESTUDIO,
                                                ID_IMPUTADO = EntityFederal.ID_IMPUTADO,
                                                ID_INGRESO = EntityFederal.ID_INGRESO
                                            };

                                            ValidaEstudio.PFF_CAPACITACION.PFF_CAPACITACION_CURSO.Add(_NvaCurso);
                                            _consecutivoCapacitacionFederal++;
                                        };
                                }

                                else
                                {
                                    if (EntityFederal.PFF_CAPACITACION != null)
                                    {
                                        _EstudioCapacitacionFederal.A_TOTAL_DIAS_LABORADOS = EntityFederal.PFF_CAPACITACION.A_TOTAL_DIAS_LABORADOS;
                                        _EstudioCapacitacionFederal.ACTITUDES_DESEMPENO_ACT = EntityFederal.PFF_CAPACITACION.ACTITUDES_DESEMPENO_ACT;
                                        _EstudioCapacitacionFederal.ACTIVIDAD_PRODUC_ACTUAL = EntityFederal.PFF_CAPACITACION.ACTIVIDAD_PRODUC_ACTUAL;
                                        _EstudioCapacitacionFederal.ATIENDE_INDICACIONES = EntityFederal.PFF_CAPACITACION.ATIENDE_INDICACIONES;
                                        _EstudioCapacitacionFederal.B_DIAS_LABORADOS_OTROS_CERESOS = EntityFederal.PFF_CAPACITACION.B_DIAS_LABORADOS_OTROS_CERESOS;
                                        _EstudioCapacitacionFederal.CAMBIO_ACTIVIDAD = EntityFederal.PFF_CAPACITACION.CAMBIO_ACTIVIDAD;
                                        _EstudioCapacitacionFederal.CAMBIO_ACTIVIDAD_POR_QUE = EntityFederal.PFF_CAPACITACION.CAMBIO_ACTIVIDAD_POR_QUE;
                                        _EstudioCapacitacionFederal.CONCLUSIONES = EntityFederal.PFF_CAPACITACION.CONCLUSIONES;
                                        _EstudioCapacitacionFederal.DESCUIDADO_LABORES = EntityFederal.PFF_CAPACITACION.DESCUIDADO_LABORES;
                                        _EstudioCapacitacionFederal.DIRECTOR_CENTRO = EntityFederal.PFF_CAPACITACION.DIRECTOR_CENTRO;
                                        _EstudioCapacitacionFederal.FECHA = EntityFederal.PFF_CAPACITACION.FECHA;
                                        _EstudioCapacitacionFederal.FONDO_AHORRO = EntityFederal.PFF_CAPACITACION.FONDO_AHORRO;
                                        _EstudioCapacitacionFederal.FONDO_AHORRO_COMPESACION_ACTUA = EntityFederal.PFF_CAPACITACION.FONDO_AHORRO_COMPESACION_ACTUA;
                                        _EstudioCapacitacionFederal.HA_PROGRESADO_OFICIO = EntityFederal.PFF_CAPACITACION.HA_PROGRESADO_OFICIO;
                                        _EstudioCapacitacionFederal.ID_ANIO = EntityFederal.ID_ANIO;
                                        _EstudioCapacitacionFederal.ID_CENTRO = EntityFederal.ID_CENTRO;
                                        _EstudioCapacitacionFederal.ID_ESTUDIO = EntityFederal.ID_ESTUDIO;
                                        _EstudioCapacitacionFederal.ID_IMPUTADO = EntityFederal.ID_IMPUTADO;
                                        _EstudioCapacitacionFederal.ID_INGRESO = EntityFederal.ID_INGRESO;
                                        _EstudioCapacitacionFederal.JEFE_SECC_INDUSTRIAL = EntityFederal.PFF_CAPACITACION.JEFE_SECC_INDUSTRIAL;
                                        _EstudioCapacitacionFederal.LUGAR = EntityFederal.PFF_CAPACITACION.LUGAR;
                                        _EstudioCapacitacionFederal.MOTIVO_TIEMPO_INTERRUP_ACT = EntityFederal.PFF_CAPACITACION.MOTIVO_TIEMPO_INTERRUP_ACT;
                                        _EstudioCapacitacionFederal.NO_CURSOS_MOTIVO = EntityFederal.PFF_CAPACITACION.NO_CURSOS_MOTIVO;
                                        _EstudioCapacitacionFederal.NOMBRE = EntityFederal.PFF_CAPACITACION.NOMBRE;
                                        _EstudioCapacitacionFederal.OFICIO_ANTES_RECLUSION = EntityFederal.PFF_CAPACITACION.OFICIO_ANTES_RECLUSION.HasValue ? EntityFederal.PFF_CAPACITACION.OFICIO_ANTES_RECLUSION != -1 ? EntityFederal.PFF_CAPACITACION.OFICIO_ANTES_RECLUSION : null : null;
                                        _EstudioCapacitacionFederal.RECIBIO_CONSTANCIA = EntityFederal.PFF_CAPACITACION.RECIBIO_CONSTANCIA;
                                        _EstudioCapacitacionFederal.SALARIO_DEVENGABA_DETENCION = EntityFederal.PFF_CAPACITACION.SALARIO_DEVENGABA_DETENCION;
                                        _EstudioCapacitacionFederal.SATISFACE_ACTIVIDAD = EntityFederal.PFF_CAPACITACION.SATISFACE_ACTIVIDAD;
                                        _EstudioCapacitacionFederal.SECCION = EntityFederal.PFF_CAPACITACION.SECCION;
                                        _EstudioCapacitacionFederal.TOTAL_A_B = EntityFederal.PFF_CAPACITACION.TOTAL_A_B;
                                        Context.Entry(_EstudioCapacitacionFederal).State = System.Data.EntityState.Modified;

                                        if (_DiasGuardados != null && _DiasGuardados.Any())
                                            foreach (var item in _DiasGuardados)
                                                Context.Entry(item).State = System.Data.EntityState.Deleted;

                                        if (_GruposCapac != null && _GruposCapac.Any())
                                            foreach (var item in _GruposCapac)
                                                Context.Entry(item).State = System.Data.EntityState.Deleted;

                                        if (EntityFederal.PFF_CAPACITACION.PFF_DIAS_LABORADO != null && EntityFederal.PFF_CAPACITACION.PFF_DIAS_LABORADO.Any())
                                            foreach (var item in EntityFederal.PFF_CAPACITACION.PFF_DIAS_LABORADO)
                                            {
                                                var _NvoCapac = new PFF_DIAS_LABORADO()
                                                {
                                                    ANIO = item.ANIO,
                                                    DIAS_TRABAJADOS = item.DIAS_TRABAJADOS,
                                                    ID_ANIO = EntityFederal.ID_ANIO,
                                                    ID_CENTRO = EntityFederal.ID_CENTRO,
                                                    ID_ESTUDIO = EntityFederal.ID_ESTUDIO,
                                                    ID_IMPUTADO = EntityFederal.ID_IMPUTADO,
                                                    ID_INGRESO = EntityFederal.ID_INGRESO,
                                                    MES = item.MES
                                                };

                                                ValidaEstudio.PFF_CAPACITACION.PFF_DIAS_LABORADO.Add(_NvoCapac);
                                            };

                                        var _consecutivoCapacitacionFederal = GetIDProceso<short>("PFF_CAPACITACION_CURSO", "ID_CONSEC", string.Format("ID_CENTRO = {0} AND ID_ANIO = {1} AND ID_IMPUTADO = {2} AND ID_INGRESO = {3} AND ID_ESTUDIO = {4}", EntityFederal.ID_CENTRO, EntityFederal.ID_ANIO, EntityFederal.ID_IMPUTADO, EntityFederal.ID_INGRESO, EntityFederal.ID_ESTUDIO));
                                        if (EntityFederal.PFF_CAPACITACION.PFF_CAPACITACION_CURSO != null && EntityFederal.PFF_CAPACITACION.PFF_CAPACITACION_CURSO.Any())
                                            foreach (var item in EntityFederal.PFF_CAPACITACION.PFF_CAPACITACION_CURSO)
                                            {
                                                var _NvaCurso = new PFF_CAPACITACION_CURSO()
                                                {
                                                    CURSO = item.CURSO,
                                                    FECHA_INICIO = item.FECHA_INICIO,
                                                    FECHA_TERMINO = item.FECHA_TERMINO,
                                                    ID_ANIO = EntityFederal.ID_ANIO,
                                                    ID_CENTRO = EntityFederal.ID_CENTRO,
                                                    ID_CONSEC = _consecutivoCapacitacionFederal,
                                                    ID_ESTUDIO = EntityFederal.ID_ESTUDIO,
                                                    ID_IMPUTADO = EntityFederal.ID_IMPUTADO,
                                                    ID_INGRESO = EntityFederal.ID_INGRESO
                                                };

                                                ValidaEstudio.PFF_CAPACITACION.PFF_CAPACITACION_CURSO.Add(_NvaCurso);
                                                _consecutivoCapacitacionFederal++;
                                            };
                                    };
                                };


                                #region Definicion de detalle de estudio de capacitacion de fuero federal
                                if (_EstudioPadre != null)
                                {
                                    short? _Estatus = new short?();
                                    if (string.IsNullOrEmpty(EntityFederal.PFF_CAPACITACION.ACTITUDES_DESEMPENO_ACT) || string.IsNullOrEmpty(EntityFederal.PFF_CAPACITACION.ACTIVIDAD_PRODUC_ACTUAL) ||
                                        string.IsNullOrEmpty(EntityFederal.PFF_CAPACITACION.ATIENDE_INDICACIONES) || string.IsNullOrEmpty(EntityFederal.PFF_CAPACITACION.CAMBIO_ACTIVIDAD) ||
                                       string.IsNullOrEmpty(EntityFederal.PFF_CAPACITACION.CONCLUSIONES) || string.IsNullOrEmpty(EntityFederal.PFF_CAPACITACION.DESCUIDADO_LABORES) ||
                                       string.IsNullOrEmpty(EntityFederal.PFF_CAPACITACION.FONDO_AHORRO) || string.IsNullOrEmpty(EntityFederal.PFF_CAPACITACION.HA_PROGRESADO_OFICIO) ||
                                       string.IsNullOrEmpty(EntityFederal.PFF_CAPACITACION.NOMBRE) || EntityFederal.PFF_CAPACITACION.OFICIO_ANTES_RECLUSION == null ||
                                       string.IsNullOrEmpty(EntityFederal.PFF_CAPACITACION.SATISFACE_ACTIVIDAD) || string.IsNullOrEmpty(EntityFederal.PFF_CAPACITACION.SECCION) ||
                                       EntityFederal.PFF_CAPACITACION.A_TOTAL_DIAS_LABORADOS == null || EntityFederal.PFF_CAPACITACION.B_DIAS_LABORADOS_OTROS_CERESOS == null ||
                                       EntityFederal.PFF_CAPACITACION.FECHA == null || EntityFederal.PFF_CAPACITACION.SALARIO_DEVENGABA_DETENCION == null ||
                                       EntityFederal.PFF_CAPACITACION.TOTAL_A_B == null)
                                        _Estatus = (short)eEstatudDetallePersonalidad.ASIGNADO;
                                    else
                                        _Estatus = (short)eEstatudDetallePersonalidad.TERMINADO;

                                    var _DesarrolloCapacitacionFederal = Context.PERSONALIDAD_DETALLE.FirstOrDefault(x => x.ID_IMPUTADO == EntityFederal.ID_IMPUTADO && x.ID_ESTUDIO == _EstudioPadre.ID_ESTUDIO && x.ID_INGRESO == EntityFederal.ID_INGRESO && x.ID_TIPO == (short)eTiposEstudio.LABORAL_FEDERAL && x.ID_CENTRO == EntityFederal.ID_CENTRO && x.ID_ANIO == EntityFederal.ID_ANIO);
                                    if (_DesarrolloCapacitacionFederal == null)
                                    {
                                        short _ConsecutivoPersonalidadDetalle = GetIDProceso<short>("PERSONALIDAD_DETALLE", "ID_DETALLE", "1=1");
                                        var NvoDetalle = new PERSONALIDAD_DETALLE()
                                        {
                                            ID_ANIO = EntityFederal.ID_ANIO,
                                            ID_CENTRO = EntityFederal.ID_CENTRO,
                                            ID_DETALLE = _ConsecutivoPersonalidadDetalle,
                                            ID_ESTUDIO = _EstudioPadre.ID_ESTUDIO,
                                            ID_ESTATUS = _Estatus,
                                            ID_IMPUTADO = EntityFederal.ID_IMPUTADO,
                                            ID_INGRESO = EntityFederal.ID_INGRESO,
                                            ID_TIPO = (short)eTiposEstudio.LABORAL_FEDERAL,
                                            INICIO_FEC = GetFechaServerDate(),
                                            RESULTADO = _Estatus == (short)eEstatudDetallePersonalidad.TERMINADO ? "S" : "N",
                                            SOLICITUD_FEC = null,
                                            TERMINO_FEC = _Estatus == (short)eEstatudDetallePersonalidad.TERMINADO ? GetFechaServerDate() : new System.DateTime?(),
                                            TIPO_MEDIA = string.Empty
                                        };

                                        Context.PERSONALIDAD_DETALLE.Add(NvoDetalle);
                                    }

                                    else
                                    {
                                        _DesarrolloCapacitacionFederal.RESULTADO = _Estatus.HasValue ? _Estatus.Value == (short)eEstatudDetallePersonalidad.TERMINADO ? "S" : "N" : "N";
                                        _DesarrolloCapacitacionFederal.ID_ESTATUS = _Estatus;
                                        Context.Entry(_DesarrolloCapacitacionFederal).State = System.Data.EntityState.Modified;
                                    }
                                }

                                #endregion
                            };


                            if (EntityFederal.PFF_ACTIVIDAD != null)
                            {
                                var _ACtivFed = Context.PFF_ACTIVIDAD.FirstOrDefault(x => x.ID_INGRESO == EntityFederal.ID_INGRESO && x.ID_IMPUTADO == EntityFederal.ID_IMPUTADO && x.ID_ESTUDIO == EntityFederal.ID_ESTUDIO && x.ID_CENTRO == EntityFederal.ID_CENTRO && x.ID_ANIO == EntityFederal.ID_ANIO);
                                var _DetalleActivParticipacion = Context.PFF_ACTIVIDAD_PARTICIPACION.Where(x => x.ID_INGRESO == EntityFederal.ID_INGRESO && x.ID_IMPUTADO == EntityFederal.ID_IMPUTADO && x.ID_ESTUDIO == EntityFederal.ID_ESTUDIO && x.ID_CENTRO == EntityFederal.ID_CENTRO && x.ID_ANIO == EntityFederal.ID_ANIO);
                                if (_ACtivFed == null)
                                {
                                    ValidaEstudio.PFF_ACTIVIDAD = new PFF_ACTIVIDAD()
                                    {
                                        ALFABE_PRIMARIA = EntityFederal.PFF_ACTIVIDAD.ALFABE_PRIMARIA,
                                        ASISTE_PUNTUAL = EntityFederal.PFF_ACTIVIDAD.ASISTE_PUNTUAL,
                                        ASISTE_PUNTUAL_NO_POR_QUE = EntityFederal.PFF_ACTIVIDAD.ASISTE_PUNTUAL_NO_POR_QUE,
                                        AVANCE_RENDIMIENTO_ACADEMINCO = EntityFederal.PFF_ACTIVIDAD.AVANCE_RENDIMIENTO_ACADEMINCO,
                                        BACHILLER_UNI = EntityFederal.PFF_ACTIVIDAD.BACHILLER_UNI,
                                        CONCLUSIONES = EntityFederal.PFF_ACTIVIDAD.CONCLUSIONES,
                                        DIRECTOR_CENTRO = EntityFederal.PFF_ACTIVIDAD.DIRECTOR_CENTRO,
                                        ESCOLARIDAD_MOMENTO = EntityFederal.PFF_ACTIVIDAD.ESCOLARIDAD_MOMENTO != -1 ? EntityFederal.PFF_ACTIVIDAD.ESCOLARIDAD_MOMENTO : null,
                                        ESPECIFIQUE = EntityFederal.PFF_ACTIVIDAD.ESPECIFIQUE,
                                        ESTUDIOS_ACTUALES = EntityFederal.PFF_ACTIVIDAD.ESTUDIOS_ACTUALES,
                                        ESTUDIOS_EN_INTERNAMIENTO = EntityFederal.PFF_ACTIVIDAD.ESTUDIOS_EN_INTERNAMIENTO,
                                        FECHA = EntityFederal.PFF_ACTIVIDAD.FECHA,
                                        ID_ANIO = EntityFederal.ID_ANIO,
                                        ID_CENTRO = EntityFederal.ID_CENTRO,
                                        ID_INGRESO = EntityFederal.ID_INGRESO,
                                        ID_ESTUDIO = EntityFederal.ID_ESTUDIO,
                                        ID_IMPUTADO = EntityFederal.ID_IMPUTADO,
                                        IMPARTIDO_ENSENANZA = EntityFederal.PFF_ACTIVIDAD.IMPARTIDO_ENSENANZA,
                                        IMPARTIDO_ENSENANZA_TIEMPO = EntityFederal.PFF_ACTIVIDAD.IMPARTIDO_ENSENANZA_TIEMPO,
                                        IMPARTIDO_ENSENANZA_TIPO = EntityFederal.PFF_ACTIVIDAD.IMPARTIDO_ENSENANZA_TIPO,
                                        JEFE_SECC_EDUCATIVA = EntityFederal.PFF_ACTIVIDAD.JEFE_SECC_EDUCATIVA,
                                        LUGAR = EntityFederal.PFF_ACTIVIDAD.LUGAR,
                                        NOMBRE = EntityFederal.PFF_ACTIVIDAD.NOMBRE,
                                        OTRA_ENSENANZA = EntityFederal.PFF_ACTIVIDAD.OTRA_ENSENANZA,
                                        OTRO = EntityFederal.PFF_ACTIVIDAD.OTRO,
                                        PRIMARIA_SECU = EntityFederal.PFF_ACTIVIDAD.PRIMARIA_SECU,
                                        PROMOVIDO = EntityFederal.PFF_ACTIVIDAD.PROMOVIDO,
                                        OTROS_PROGRAMAS = EntityFederal.PFF_ACTIVIDAD.OTROS_PROGRAMAS,
                                        SECU_BACHILLER = EntityFederal.PFF_ACTIVIDAD.SECU_BACHILLER
                                    };

                                    if (_DetalleActivParticipacion != null && _DetalleActivParticipacion.Any())
                                        foreach (var item in _DetalleActivParticipacion)
                                            Context.Entry(item).State = System.Data.EntityState.Deleted;

                                    if (EntityFederal.PFF_ACTIVIDAD.PFF_ACTIVIDAD_PARTICIPACION != null && EntityFederal.PFF_ACTIVIDAD.PFF_ACTIVIDAD_PARTICIPACION.Any())
                                        foreach (var item in EntityFederal.PFF_ACTIVIDAD.PFF_ACTIVIDAD_PARTICIPACION)
                                        {
                                            var _Detalle = new PFF_ACTIVIDAD_PARTICIPACION()
                                            {
                                                FECHA_1 = item.FECHA_1,
                                                FECHA_2 = item.FECHA_2,
                                                ID_ANIO = EntityFederal.ID_ANIO,
                                                ID_CENTRO = EntityFederal.ID_CENTRO,
                                                ID_ESTUDIO = EntityFederal.ID_ESTUDIO,
                                                ID_IMPUTADO = EntityFederal.ID_IMPUTADO,
                                                ID_INGRESO = EntityFederal.ID_INGRESO,
                                                ID_TIPO_PROGRAMA = item.ID_TIPO_PROGRAMA,
                                                OTRO_ESPECIFICAR = item.OTRO_ESPECIFICAR,
                                                PARTICIPACION = item.PARTICIPACION
                                            };

                                            ValidaEstudio.PFF_ACTIVIDAD.PFF_ACTIVIDAD_PARTICIPACION.Add(_Detalle);
                                        };
                                }

                                else
                                {
                                    _ACtivFed.ALFABE_PRIMARIA = EntityFederal.PFF_ACTIVIDAD.ALFABE_PRIMARIA;
                                    _ACtivFed.ASISTE_PUNTUAL = EntityFederal.PFF_ACTIVIDAD.ASISTE_PUNTUAL;
                                    _ACtivFed.ASISTE_PUNTUAL_NO_POR_QUE = EntityFederal.PFF_ACTIVIDAD.ASISTE_PUNTUAL_NO_POR_QUE;
                                    _ACtivFed.AVANCE_RENDIMIENTO_ACADEMINCO = EntityFederal.PFF_ACTIVIDAD.AVANCE_RENDIMIENTO_ACADEMINCO;
                                    _ACtivFed.BACHILLER_UNI = EntityFederal.PFF_ACTIVIDAD.BACHILLER_UNI;
                                    _ACtivFed.CONCLUSIONES = EntityFederal.PFF_ACTIVIDAD.CONCLUSIONES;
                                    _ACtivFed.DIRECTOR_CENTRO = EntityFederal.PFF_ACTIVIDAD.DIRECTOR_CENTRO;
                                    _ACtivFed.ESCOLARIDAD_MOMENTO = EntityFederal.PFF_ACTIVIDAD.ESCOLARIDAD_MOMENTO != -1 ? EntityFederal.PFF_ACTIVIDAD.ESCOLARIDAD_MOMENTO : null;
                                    _ACtivFed.ESPECIFIQUE = EntityFederal.PFF_ACTIVIDAD.ESPECIFIQUE;
                                    _ACtivFed.ESTUDIOS_ACTUALES = EntityFederal.PFF_ACTIVIDAD.ESTUDIOS_ACTUALES;
                                    _ACtivFed.ESTUDIOS_EN_INTERNAMIENTO = EntityFederal.PFF_ACTIVIDAD.ESTUDIOS_EN_INTERNAMIENTO;
                                    _ACtivFed.FECHA = EntityFederal.PFF_ACTIVIDAD.FECHA;
                                    _ACtivFed.ID_ANIO = EntityFederal.ID_ANIO;
                                    _ACtivFed.ID_CENTRO = EntityFederal.ID_CENTRO;
                                    _ACtivFed.ID_ESTUDIO = EntityFederal.ID_ESTUDIO;
                                    _ACtivFed.ID_IMPUTADO = EntityFederal.ID_IMPUTADO;
                                    _ACtivFed.ID_INGRESO = EntityFederal.ID_INGRESO;
                                    _ACtivFed.IMPARTIDO_ENSENANZA = EntityFederal.PFF_ACTIVIDAD.IMPARTIDO_ENSENANZA;
                                    _ACtivFed.IMPARTIDO_ENSENANZA_TIEMPO = EntityFederal.PFF_ACTIVIDAD.IMPARTIDO_ENSENANZA_TIEMPO;
                                    _ACtivFed.IMPARTIDO_ENSENANZA_TIPO = EntityFederal.PFF_ACTIVIDAD.IMPARTIDO_ENSENANZA_TIPO;
                                    _ACtivFed.JEFE_SECC_EDUCATIVA = EntityFederal.PFF_ACTIVIDAD.JEFE_SECC_EDUCATIVA;
                                    _ACtivFed.LUGAR = EntityFederal.PFF_ACTIVIDAD.LUGAR;
                                    _ACtivFed.NOMBRE = EntityFederal.PFF_ACTIVIDAD.NOMBRE;
                                    _ACtivFed.OTRA_ENSENANZA = EntityFederal.PFF_ACTIVIDAD.OTRA_ENSENANZA;
                                    _ACtivFed.OTROS_PROGRAMAS = EntityFederal.PFF_ACTIVIDAD.OTROS_PROGRAMAS;
                                    _ACtivFed.OTRO = EntityFederal.PFF_ACTIVIDAD.OTRO;
                                    _ACtivFed.PRIMARIA_SECU = EntityFederal.PFF_ACTIVIDAD.PRIMARIA_SECU;
                                    _ACtivFed.PROMOVIDO = EntityFederal.PFF_ACTIVIDAD.PROMOVIDO;
                                    _ACtivFed.SECU_BACHILLER = EntityFederal.PFF_ACTIVIDAD.SECU_BACHILLER;
                                    _ACtivFed.OTROS_PROGRAMAS = EntityFederal.PFF_ACTIVIDAD.OTROS_PROGRAMAS;
                                    Context.Entry(_ACtivFed).State = System.Data.EntityState.Modified;

                                    if (_DetalleActivParticipacion != null && _DetalleActivParticipacion.Any())
                                        foreach (var item in _DetalleActivParticipacion)
                                            Context.Entry(item).State = System.Data.EntityState.Deleted;

                                    if (EntityFederal.PFF_ACTIVIDAD.PFF_ACTIVIDAD_PARTICIPACION != null && EntityFederal.PFF_ACTIVIDAD.PFF_ACTIVIDAD_PARTICIPACION.Any())
                                        foreach (var item in EntityFederal.PFF_ACTIVIDAD.PFF_ACTIVIDAD_PARTICIPACION)
                                        {
                                            var _Detalle = new PFF_ACTIVIDAD_PARTICIPACION()
                                            {
                                                FECHA_1 = item.FECHA_1,
                                                FECHA_2 = item.FECHA_2,
                                                ID_ANIO = EntityFederal.ID_ANIO,
                                                ID_CENTRO = EntityFederal.ID_CENTRO,
                                                ID_ESTUDIO = EntityFederal.ID_ESTUDIO,
                                                ID_IMPUTADO = EntityFederal.ID_IMPUTADO,
                                                ID_INGRESO = EntityFederal.ID_INGRESO,
                                                ID_TIPO_PROGRAMA = item.ID_TIPO_PROGRAMA,
                                                OTRO_ESPECIFICAR = item.OTRO_ESPECIFICAR,
                                                PARTICIPACION = item.PARTICIPACION
                                            };

                                            ValidaEstudio.PFF_ACTIVIDAD.PFF_ACTIVIDAD_PARTICIPACION.Add(_Detalle);
                                        };
                                };

                                #region Detalle
                                if (_EstudioPadre != null)
                                {
                                    short? _Estatus = new short?();
                                    if (string.IsNullOrEmpty(EntityFederal.PFF_ACTIVIDAD.ASISTE_PUNTUAL) || string.IsNullOrEmpty(EntityFederal.PFF_ACTIVIDAD.AVANCE_RENDIMIENTO_ACADEMINCO) || EntityFederal.PFF_ACTIVIDAD.ESCOLARIDAD_MOMENTO == null ||
                                        string.IsNullOrEmpty(EntityFederal.PFF_ACTIVIDAD.ESTUDIOS_ACTUALES) || string.IsNullOrEmpty(EntityFederal.PFF_ACTIVIDAD.ESTUDIOS_EN_INTERNAMIENTO) ||
                                        string.IsNullOrEmpty(EntityFederal.PFF_ACTIVIDAD.IMPARTIDO_ENSENANZA) || string.IsNullOrEmpty(EntityFederal.PFF_ACTIVIDAD.NOMBRE) || string.IsNullOrEmpty(EntityFederal.PFF_ACTIVIDAD.PROMOVIDO)
                                        || EntityFederal.PFF_ACTIVIDAD.FECHA == null)
                                        _Estatus = (short)eEstatudDetallePersonalidad.ASIGNADO;
                                    else
                                        _Estatus = (short)eEstatudDetallePersonalidad.TERMINADO;

                                    var DesarrolloTrabajoFederal = Context.PERSONALIDAD_DETALLE.FirstOrDefault(x => x.ID_IMPUTADO == EntityFederal.ID_IMPUTADO && x.ID_ESTUDIO == _EstudioPadre.ID_ESTUDIO && x.ID_INGRESO == EntityFederal.ID_INGRESO && x.ID_TIPO == (short)eTiposEstudio.PEDAGOGICA_FEDERAL && x.ID_CENTRO == EntityFederal.ID_CENTRO && x.ID_ANIO == EntityFederal.ID_ANIO);
                                    if (DesarrolloTrabajoFederal == null)
                                    {
                                        short _ConsecutivoPersonalidadDetalle = GetIDProceso<short>("PERSONALIDAD_DETALLE", "ID_DETALLE", "1=1");
                                        var NvoDetalle = new PERSONALIDAD_DETALLE()
                                        {
                                            ID_ANIO = EntityFederal.ID_ANIO,
                                            ID_CENTRO = EntityFederal.ID_CENTRO,
                                            ID_DETALLE = _ConsecutivoPersonalidadDetalle,
                                            ID_ESTUDIO = _EstudioPadre.ID_ESTUDIO,
                                            ID_IMPUTADO = EntityFederal.ID_IMPUTADO,
                                            ID_ESTATUS = _Estatus,
                                            ID_INGRESO = EntityFederal.ID_INGRESO,
                                            ID_TIPO = (short)eTiposEstudio.PEDAGOGICA_FEDERAL,
                                            INICIO_FEC = GetFechaServerDate(),
                                            RESULTADO = _Estatus == (short)eEstatudDetallePersonalidad.TERMINADO ? "S" : "N",
                                            SOLICITUD_FEC = null,
                                            TERMINO_FEC = _Estatus == (short)eEstatudDetallePersonalidad.TERMINADO ? GetFechaServerDate() : new System.DateTime?(),
                                            TIPO_MEDIA = string.Empty
                                        };

                                        Context.PERSONALIDAD_DETALLE.Add(NvoDetalle);
                                    }

                                    else
                                    {
                                        DesarrolloTrabajoFederal.RESULTADO = _Estatus.HasValue ? _Estatus.Value == (short)eEstatudDetallePersonalidad.TERMINADO ? "S" : "N" : "N";
                                        DesarrolloTrabajoFederal.ID_ESTATUS = _Estatus;
                                        Context.Entry(DesarrolloTrabajoFederal).State = System.Data.EntityState.Modified;
                                    }
                                };

                                #endregion
                            }


                            if (EntityFederal.PFF_VIGILANCIA != null)
                            {
                                var _InformeVigilanciaFF = Context.PFF_VIGILANCIA.FirstOrDefault(x => x.ID_INGRESO == EntityFederal.ID_INGRESO && x.ID_IMPUTADO == EntityFederal.ID_IMPUTADO && x.ID_ESTUDIO == EntityFederal.ID_ESTUDIO && x.ID_ANIO == EntityFederal.ID_ANIO);
                                var _DetalleSentencia = Context.PFF_CORRECTIVO.Where(x => x.ID_INGRESO == EntityFederal.ID_INGRESO && x.ID_IMPUTADO == EntityFederal.ID_IMPUTADO && x.ID_ESTUDIO == EntityFederal.ID_ESTUDIO && x.ID_ANIO == EntityFederal.ID_ANIO);
                                if (_InformeVigilanciaFF == null)
                                {
                                    ValidaEstudio.PFF_VIGILANCIA = new PFF_VIGILANCIA()
                                    {
                                        CENTRO_DONDE_PROCEDE = EntityFederal.PFF_VIGILANCIA.CENTRO_DONDE_PROCEDE,
                                        CONCLUSIONES = EntityFederal.PFF_VIGILANCIA.CONCLUSIONES,
                                        CONDUCTA = EntityFederal.PFF_VIGILANCIA.CONDUCTA,
                                        CONDUCTA_FAMILIA = EntityFederal.PFF_VIGILANCIA.CONDUCTA_FAMILIA,
                                        CONDUCTA_GENERAL = EntityFederal.PFF_VIGILANCIA.CONDUCTA_GENERAL,
                                        CONDUCTA_SUPERIORES = EntityFederal.PFF_VIGILANCIA.CONDUCTA_SUPERIORES,
                                        DESCRIPCION_CONDUCTA = EntityFederal.PFF_VIGILANCIA.DESCRIPCION_CONDUCTA,
                                        DIRECTOR_CENTRO = EntityFederal.PFF_VIGILANCIA.DIRECTOR_CENTRO,
                                        ESTIMULOS_BUENA_CONDUCTA = EntityFederal.PFF_VIGILANCIA.ESTIMULOS_BUENA_CONDUCTA,
                                        FECHA = EntityFederal.PFF_VIGILANCIA.FECHA,
                                        FECHA_INGRESO = EntityFederal.PFF_VIGILANCIA.FECHA_INGRESO,
                                        HIGIENE_CELDA = EntityFederal.PFF_VIGILANCIA.HIGIENE_CELDA,
                                        HIGIENE_PERSONAL = EntityFederal.PFF_VIGILANCIA.HIGIENE_PERSONAL,
                                        ID_ANIO = EntityFederal.ID_ANIO,
                                        ID_CENTRO = EntityFederal.ID_CENTRO,
                                        ID_ESTUDIO = EntityFederal.ID_ESTUDIO,
                                        ID_IMPUTADO = EntityFederal.ID_IMPUTADO,
                                        ID_INGRESO = EntityFederal.ID_INGRESO,
                                        JEFE_VIGILANCIA = EntityFederal.PFF_VIGILANCIA.JEFE_VIGILANCIA,
                                        LUGAR = EntityFederal.PFF_VIGILANCIA.LUGAR,
                                        MOTIVO_TRASLADO = EntityFederal.PFF_VIGILANCIA.MOTIVO_TRASLADO,
                                        NOMBRE = EntityFederal.PFF_VIGILANCIA.NOMBRE,
                                        RELACION_COMPANEROS = EntityFederal.PFF_VIGILANCIA.RELACION_COMPANEROS,
                                        VISITA_FRECUENCIA = EntityFederal.PFF_VIGILANCIA.VISITA_FRECUENCIA,
                                        VISITA_QUIENES = EntityFederal.PFF_VIGILANCIA.VISITA_QUIENES,
                                        VISITA_RECIBE = EntityFederal.PFF_VIGILANCIA.VISITA_RECIBE
                                    };

                                    if (_DetalleSentencia != null && _DetalleSentencia.Any())
                                        foreach (var item in _DetalleSentencia)
                                            Context.Entry(item).State = System.Data.EntityState.Deleted;

                                    var _consecutivoCapacitacionFederal = GetIDProceso<short>("PFF_CORRECTIVO", "ID_CONSEC", string.Format("ID_CENTRO = {0} AND ID_ANIO = {1} AND ID_IMPUTADO = {2} AND ID_INGRESO = {3} AND ID_ESTUDIO = {4}", EntityFederal.ID_CENTRO, EntityFederal.ID_ANIO, EntityFederal.ID_IMPUTADO, EntityFederal.ID_INGRESO, EntityFederal.ID_ESTUDIO));
                                    if (EntityFederal.PFF_VIGILANCIA.PFF_CORRECTIVO != null && EntityFederal.PFF_VIGILANCIA.PFF_CORRECTIVO.Any())
                                        foreach (var item in EntityFederal.PFF_VIGILANCIA.PFF_CORRECTIVO)
                                        {
                                            var NvaSancion = new PFF_CORRECTIVO()
                                            {
                                                FECHA = item.FECHA,
                                                ID_ANIO = EntityFederal.ID_ANIO,
                                                ID_CENTRO = EntityFederal.ID_CENTRO,
                                                ID_ESTUDIO = EntityFederal.ID_ESTUDIO,
                                                ID_IMPUTADO = EntityFederal.ID_IMPUTADO,
                                                ID_INGRESO = EntityFederal.ID_INGRESO,
                                                MOTIVO = item.MOTIVO,
                                                RESOLUCION = item.RESOLUCION,
                                                ID_CONSEC = _consecutivoCapacitacionFederal
                                            };

                                            ValidaEstudio.PFF_VIGILANCIA.PFF_CORRECTIVO.Add(NvaSancion);
                                            _consecutivoCapacitacionFederal++;
                                        };
                                }
                                else
                                {
                                    _InformeVigilanciaFF.CENTRO_DONDE_PROCEDE = EntityFederal.PFF_VIGILANCIA.CENTRO_DONDE_PROCEDE;
                                    _InformeVigilanciaFF.CONCLUSIONES = EntityFederal.PFF_VIGILANCIA.CONCLUSIONES;
                                    _InformeVigilanciaFF.CONDUCTA = EntityFederal.PFF_VIGILANCIA.CONDUCTA;
                                    _InformeVigilanciaFF.CONDUCTA_FAMILIA = EntityFederal.PFF_VIGILANCIA.CONDUCTA_FAMILIA;
                                    _InformeVigilanciaFF.CONDUCTA_GENERAL = EntityFederal.PFF_VIGILANCIA.CONDUCTA_GENERAL;
                                    _InformeVigilanciaFF.CONDUCTA_SUPERIORES = EntityFederal.PFF_VIGILANCIA.CONDUCTA_SUPERIORES;
                                    _InformeVigilanciaFF.DESCRIPCION_CONDUCTA = EntityFederal.PFF_VIGILANCIA.DESCRIPCION_CONDUCTA;
                                    _InformeVigilanciaFF.DIRECTOR_CENTRO = EntityFederal.PFF_VIGILANCIA.DIRECTOR_CENTRO;
                                    _InformeVigilanciaFF.ESTIMULOS_BUENA_CONDUCTA = EntityFederal.PFF_VIGILANCIA.ESTIMULOS_BUENA_CONDUCTA;
                                    _InformeVigilanciaFF.FECHA = EntityFederal.PFF_VIGILANCIA.FECHA;
                                    _InformeVigilanciaFF.FECHA_INGRESO = EntityFederal.PFF_VIGILANCIA.FECHA_INGRESO;
                                    _InformeVigilanciaFF.HIGIENE_CELDA = EntityFederal.PFF_VIGILANCIA.HIGIENE_CELDA;
                                    _InformeVigilanciaFF.HIGIENE_PERSONAL = EntityFederal.PFF_VIGILANCIA.HIGIENE_PERSONAL;
                                    _InformeVigilanciaFF.ID_ANIO = EntityFederal.ID_ANIO;
                                    _InformeVigilanciaFF.ID_CENTRO = EntityFederal.ID_CENTRO;
                                    _InformeVigilanciaFF.ID_ESTUDIO = EntityFederal.ID_ESTUDIO;
                                    _InformeVigilanciaFF.ID_IMPUTADO = EntityFederal.ID_IMPUTADO;
                                    _InformeVigilanciaFF.ID_INGRESO = EntityFederal.ID_INGRESO;
                                    _InformeVigilanciaFF.JEFE_VIGILANCIA = EntityFederal.PFF_VIGILANCIA.JEFE_VIGILANCIA;
                                    _InformeVigilanciaFF.LUGAR = EntityFederal.PFF_VIGILANCIA.LUGAR;
                                    _InformeVigilanciaFF.MOTIVO_TRASLADO = EntityFederal.PFF_VIGILANCIA.MOTIVO_TRASLADO;
                                    _InformeVigilanciaFF.NOMBRE = EntityFederal.PFF_VIGILANCIA.NOMBRE;
                                    _InformeVigilanciaFF.RELACION_COMPANEROS = EntityFederal.PFF_VIGILANCIA.RELACION_COMPANEROS;
                                    _InformeVigilanciaFF.VISITA_FRECUENCIA = EntityFederal.PFF_VIGILANCIA.VISITA_FRECUENCIA;
                                    _InformeVigilanciaFF.VISITA_QUIENES = EntityFederal.PFF_VIGILANCIA.VISITA_QUIENES;
                                    _InformeVigilanciaFF.VISITA_RECIBE = EntityFederal.PFF_VIGILANCIA.VISITA_RECIBE;
                                    Context.Entry(_InformeVigilanciaFF).State = System.Data.EntityState.Modified;

                                    if (_DetalleSentencia != null && _DetalleSentencia.Any())
                                        foreach (var item in _DetalleSentencia)
                                            Context.Entry(item).State = System.Data.EntityState.Deleted;

                                    var _consecutivoCapacitacionFederal = GetIDProceso<short>("PFF_CORRECTIVO", "ID_CONSEC", string.Format("ID_CENTRO = {0} AND ID_ANIO = {1} AND ID_IMPUTADO = {2} AND ID_INGRESO = {3} AND ID_ESTUDIO = {4}", EntityFederal.ID_CENTRO, EntityFederal.ID_ANIO, EntityFederal.ID_IMPUTADO, EntityFederal.ID_INGRESO, EntityFederal.ID_ESTUDIO));
                                    if (EntityFederal.PFF_VIGILANCIA.PFF_CORRECTIVO != null && EntityFederal.PFF_VIGILANCIA.PFF_CORRECTIVO.Any())
                                        foreach (var item in EntityFederal.PFF_VIGILANCIA.PFF_CORRECTIVO)
                                        {
                                            var NvaSancion = new PFF_CORRECTIVO()
                                            {
                                                FECHA = item.FECHA,
                                                ID_ANIO = EntityFederal.ID_ANIO,
                                                ID_CENTRO = EntityFederal.ID_CENTRO,
                                                ID_ESTUDIO = EntityFederal.ID_ESTUDIO,
                                                ID_IMPUTADO = EntityFederal.ID_IMPUTADO,
                                                ID_INGRESO = EntityFederal.ID_INGRESO,
                                                MOTIVO = item.MOTIVO,
                                                RESOLUCION = item.RESOLUCION,
                                                ID_CONSEC = _consecutivoCapacitacionFederal
                                            };

                                            ValidaEstudio.PFF_VIGILANCIA.PFF_CORRECTIVO.Add(NvaSancion);
                                            _consecutivoCapacitacionFederal++;
                                        };
                                };


                                if (_EstudioPadre != null)
                                {
                                    short? _Estatus = new short?();
                                    if (string.IsNullOrEmpty(EntityFederal.PFF_VIGILANCIA.CENTRO_DONDE_PROCEDE) || string.IsNullOrEmpty(EntityFederal.PFF_VIGILANCIA.CONCLUSIONES) ||
                                        string.IsNullOrEmpty(EntityFederal.PFF_VIGILANCIA.CONDUCTA) || string.IsNullOrEmpty(EntityFederal.PFF_VIGILANCIA.CONDUCTA_FAMILIA) ||
                                        string.IsNullOrEmpty(EntityFederal.PFF_VIGILANCIA.CONDUCTA_GENERAL) || string.IsNullOrEmpty(EntityFederal.PFF_VIGILANCIA.CONDUCTA_SUPERIORES) ||
                                        string.IsNullOrEmpty(EntityFederal.PFF_VIGILANCIA.DESCRIPCION_CONDUCTA) || string.IsNullOrEmpty(EntityFederal.PFF_VIGILANCIA.ESTIMULOS_BUENA_CONDUCTA) ||
                                        string.IsNullOrEmpty(EntityFederal.PFF_VIGILANCIA.HIGIENE_CELDA) || string.IsNullOrEmpty(EntityFederal.PFF_VIGILANCIA.HIGIENE_PERSONAL) ||
                                        string.IsNullOrEmpty(EntityFederal.PFF_VIGILANCIA.MOTIVO_TRASLADO) || string.IsNullOrEmpty(EntityFederal.PFF_VIGILANCIA.NOMBRE) ||
                                        string.IsNullOrEmpty(EntityFederal.PFF_VIGILANCIA.RELACION_COMPANEROS) || EntityFederal.PFF_VIGILANCIA.FECHA == null || EntityFederal.PFF_VIGILANCIA.FECHA_INGRESO == null ||
                                        string.IsNullOrEmpty(EntityFederal.PFF_VIGILANCIA.VISITA_RECIBE))
                                        _Estatus = (short)eEstatudDetallePersonalidad.ASIGNADO;
                                    else
                                        _Estatus = (short)eEstatudDetallePersonalidad.TERMINADO;

                                    var DesarrolloVigilanciaFederal = Context.PERSONALIDAD_DETALLE.FirstOrDefault(x => x.ID_IMPUTADO == EntityFederal.ID_IMPUTADO && x.ID_ESTUDIO == _EstudioPadre.ID_ESTUDIO && x.ID_INGRESO == EntityFederal.ID_INGRESO && x.ID_TIPO == (short)eTiposEstudio.SEGURIDAD_FEDERAL && x.ID_CENTRO == EntityFederal.ID_CENTRO && x.ID_ANIO == EntityFederal.ID_ANIO);
                                    if (DesarrolloVigilanciaFederal == null)
                                    {
                                        short _ConsecutivoPersonalidadDetalle = GetIDProceso<short>("PERSONALIDAD_DETALLE", "ID_DETALLE", "1=1");
                                        var NvoDetalle = new PERSONALIDAD_DETALLE()
                                        {
                                            ID_ANIO = EntityFederal.ID_ANIO,
                                            ID_CENTRO = EntityFederal.ID_CENTRO,
                                            ID_DETALLE = _ConsecutivoPersonalidadDetalle,
                                            ID_ESTUDIO = _EstudioPadre.ID_ESTUDIO,
                                            ID_IMPUTADO = EntityFederal.ID_IMPUTADO,
                                            ID_INGRESO = EntityFederal.ID_INGRESO,
                                            ID_ESTATUS = _Estatus,
                                            ID_TIPO = (short)eTiposEstudio.SEGURIDAD_FEDERAL,
                                            INICIO_FEC = GetFechaServerDate(),
                                            RESULTADO = _Estatus == (short)eEstatudDetallePersonalidad.TERMINADO ? "S" : "N",
                                            SOLICITUD_FEC = null,
                                            TERMINO_FEC = _Estatus == (short)eEstatudDetallePersonalidad.TERMINADO ? GetFechaServerDate() : new System.DateTime?(),
                                            TIPO_MEDIA = string.Empty
                                        };

                                        Context.PERSONALIDAD_DETALLE.Add(NvoDetalle);
                                    }

                                    else
                                    {
                                        DesarrolloVigilanciaFederal.RESULTADO = _Estatus.HasValue ? _Estatus.Value == (short)eEstatudDetallePersonalidad.TERMINADO ? "S" : "N" : "N";
                                        DesarrolloVigilanciaFederal.ID_ESTATUS = _Estatus;
                                        Context.Entry(DesarrolloVigilanciaFederal).State = System.Data.EntityState.Modified;
                                    }
                                };
                            };


                            if (EntityFederal.PFF_CRIMINOLOGICO != null)
                            {
                                var _EstudioCriminodFueroFederal = Context.PFF_CRIMINOLOGICO.FirstOrDefault(x => x.ID_INGRESO == EntityFederal.ID_INGRESO && x.ID_IMPUTADO == EntityFederal.ID_IMPUTADO && x.ID_ESTUDIO == EntityFederal.ID_ESTUDIO && x.ID_ANIO == EntityFederal.ID_ANIO);
                                if (_EstudioCriminodFueroFederal == null)
                                {
                                    ValidaEstudio.PFF_CRIMINOLOGICO = new PFF_CRIMINOLOGICO()
                                    {
                                        ANTECEDENTES_PARA_ANTI_SOCIALE = EntityFederal.PFF_CRIMINOLOGICO.ANTECEDENTES_PARA_ANTI_SOCIALE,
                                        CRIMINOLOGO = EntityFederal.PFF_CRIMINOLOGICO.CRIMINOLOGO,
                                        DIRECTOR_CENTRO = EntityFederal.PFF_CRIMINOLOGICO.DIRECTOR_CENTRO,
                                        FECHA = EntityFederal.PFF_CRIMINOLOGICO.FECHA,
                                        ID_ANIO = EntityFederal.ID_ANIO,
                                        ID_CENTRO = EntityFederal.ID_CENTRO,
                                        ID_ESTUDIO = EntityFederal.ID_ESTUDIO,
                                        ID_IMPUTADO = EntityFederal.ID_IMPUTADO,
                                        ID_INGRESO = EntityFederal.ID_INGRESO,
                                        LUGAR = EntityFederal.PFF_CRIMINOLOGICO.LUGAR,
                                        NOMBRE = EntityFederal.PFF_CRIMINOLOGICO.NOMBRE,
                                        P1_VERSION_INTERNO = EntityFederal.PFF_CRIMINOLOGICO.P1_VERSION_INTERNO,
                                        P10_CONTINUAR_NO_ESPECIFICAR = EntityFederal.PFF_CRIMINOLOGICO.P10_CONTINUAR_NO_ESPECIFICAR,
                                        P10_CONTINUAR_SI_ESPECIFICAR = EntityFederal.PFF_CRIMINOLOGICO.P10_CONTINUAR_SI_ESPECIFICAR,
                                        P10_CONTINUAR_TRATAMIENTO = EntityFederal.PFF_CRIMINOLOGICO.P10_CONTINUAR_TRATAMIENTO,
                                        P10_OPINION = EntityFederal.PFF_CRIMINOLOGICO.P10_OPINION,
                                        P2_PERSONALIDAD = EntityFederal.PFF_CRIMINOLOGICO.P2_PERSONALIDAD,
                                        P3_VALORACION = EntityFederal.PFF_CRIMINOLOGICO.P3_VALORACION,
                                        P5_ESPECIFICO = EntityFederal.PFF_CRIMINOLOGICO.P5_ESPECIFICO,
                                        P5_GENERICO = EntityFederal.PFF_CRIMINOLOGICO.P5_GENERICO,
                                        P5_HABITUAL = EntityFederal.PFF_CRIMINOLOGICO.P5_HABITUAL,
                                        P5_PRIMODELINCUENTE = EntityFederal.PFF_CRIMINOLOGICO.P5_PRIMODELINCUENTE,
                                        P5_PROFESIONAL = EntityFederal.PFF_CRIMINOLOGICO.P5_PROFESIONAL,
                                        P6_CRIMINOGENESIS = EntityFederal.PFF_CRIMINOLOGICO.P6_CRIMINOGENESIS,
                                        P7_AGRESIVIDAD = EntityFederal.PFF_CRIMINOLOGICO.P7_AGRESIVIDAD,
                                        P7_EGOCENTRISMO = EntityFederal.PFF_CRIMINOLOGICO.P7_EGOCENTRISMO,
                                        P7_INDIFERENCIA = EntityFederal.PFF_CRIMINOLOGICO.P7_INDIFERENCIA,
                                        P7_LABILIDAD = EntityFederal.PFF_CRIMINOLOGICO.P7_LABILIDAD,
                                        P8_ESTADO_PELIGRO = EntityFederal.PFF_CRIMINOLOGICO.P8_ESTADO_PELIGRO,
                                        P8_RESULTADO_TRATAMIENTO = EntityFederal.PFF_CRIMINOLOGICO.P8_RESULTADO_TRATAMIENTO,
                                        P9_PRONOSTICO = EntityFederal.PFF_CRIMINOLOGICO.P9_PRONOSTICO,
                                        SOBRENOMBRE = EntityFederal.PFF_CRIMINOLOGICO.SOBRENOMBRE
                                    };
                                }

                                else
                                {
                                    _EstudioCriminodFueroFederal.ANTECEDENTES_PARA_ANTI_SOCIALE = EntityFederal.PFF_CRIMINOLOGICO.ANTECEDENTES_PARA_ANTI_SOCIALE;
                                    _EstudioCriminodFueroFederal.CRIMINOLOGO = EntityFederal.PFF_CRIMINOLOGICO.CRIMINOLOGO;
                                    _EstudioCriminodFueroFederal.DIRECTOR_CENTRO = EntityFederal.PFF_CRIMINOLOGICO.DIRECTOR_CENTRO;
                                    _EstudioCriminodFueroFederal.FECHA = EntityFederal.PFF_CRIMINOLOGICO.FECHA;
                                    _EstudioCriminodFueroFederal.ID_ANIO = EntityFederal.ID_ANIO;
                                    _EstudioCriminodFueroFederal.ID_CENTRO = EntityFederal.ID_CENTRO;
                                    _EstudioCriminodFueroFederal.ID_ESTUDIO = EntityFederal.ID_ESTUDIO;
                                    _EstudioCriminodFueroFederal.ID_IMPUTADO = EntityFederal.ID_IMPUTADO;
                                    _EstudioCriminodFueroFederal.ID_INGRESO = EntityFederal.ID_INGRESO;
                                    _EstudioCriminodFueroFederal.LUGAR = EntityFederal.PFF_CRIMINOLOGICO.LUGAR;
                                    _EstudioCriminodFueroFederal.NOMBRE = EntityFederal.PFF_CRIMINOLOGICO.NOMBRE;
                                    _EstudioCriminodFueroFederal.P1_VERSION_INTERNO = EntityFederal.PFF_CRIMINOLOGICO.P1_VERSION_INTERNO;
                                    _EstudioCriminodFueroFederal.P10_CONTINUAR_NO_ESPECIFICAR = EntityFederal.PFF_CRIMINOLOGICO.P10_CONTINUAR_NO_ESPECIFICAR;
                                    _EstudioCriminodFueroFederal.P10_CONTINUAR_SI_ESPECIFICAR = EntityFederal.PFF_CRIMINOLOGICO.P10_CONTINUAR_SI_ESPECIFICAR;
                                    _EstudioCriminodFueroFederal.P10_CONTINUAR_TRATAMIENTO = EntityFederal.PFF_CRIMINOLOGICO.P10_CONTINUAR_TRATAMIENTO;
                                    _EstudioCriminodFueroFederal.P10_OPINION = EntityFederal.PFF_CRIMINOLOGICO.P10_OPINION;
                                    _EstudioCriminodFueroFederal.P2_PERSONALIDAD = EntityFederal.PFF_CRIMINOLOGICO.P2_PERSONALIDAD;
                                    _EstudioCriminodFueroFederal.P3_VALORACION = EntityFederal.PFF_CRIMINOLOGICO.P3_VALORACION;
                                    _EstudioCriminodFueroFederal.P5_ESPECIFICO = EntityFederal.PFF_CRIMINOLOGICO.P5_ESPECIFICO;
                                    _EstudioCriminodFueroFederal.P5_GENERICO = EntityFederal.PFF_CRIMINOLOGICO.P5_GENERICO;
                                    _EstudioCriminodFueroFederal.P5_HABITUAL = EntityFederal.PFF_CRIMINOLOGICO.P5_HABITUAL;
                                    _EstudioCriminodFueroFederal.P5_PRIMODELINCUENTE = EntityFederal.PFF_CRIMINOLOGICO.P5_PRIMODELINCUENTE;
                                    _EstudioCriminodFueroFederal.P5_PROFESIONAL = EntityFederal.PFF_CRIMINOLOGICO.P5_PROFESIONAL;
                                    _EstudioCriminodFueroFederal.P6_CRIMINOGENESIS = EntityFederal.PFF_CRIMINOLOGICO.P6_CRIMINOGENESIS;
                                    _EstudioCriminodFueroFederal.P7_AGRESIVIDAD = EntityFederal.PFF_CRIMINOLOGICO.P7_AGRESIVIDAD;
                                    _EstudioCriminodFueroFederal.P7_EGOCENTRISMO = EntityFederal.PFF_CRIMINOLOGICO.P7_EGOCENTRISMO;
                                    _EstudioCriminodFueroFederal.P7_INDIFERENCIA = EntityFederal.PFF_CRIMINOLOGICO.P7_INDIFERENCIA;
                                    _EstudioCriminodFueroFederal.P7_LABILIDAD = EntityFederal.PFF_CRIMINOLOGICO.P7_LABILIDAD;
                                    _EstudioCriminodFueroFederal.P8_ESTADO_PELIGRO = EntityFederal.PFF_CRIMINOLOGICO.P8_ESTADO_PELIGRO;
                                    _EstudioCriminodFueroFederal.P8_RESULTADO_TRATAMIENTO = EntityFederal.PFF_CRIMINOLOGICO.P8_RESULTADO_TRATAMIENTO;
                                    _EstudioCriminodFueroFederal.P9_PRONOSTICO = EntityFederal.PFF_CRIMINOLOGICO.P9_PRONOSTICO;
                                    _EstudioCriminodFueroFederal.SOBRENOMBRE = EntityFederal.PFF_CRIMINOLOGICO.SOBRENOMBRE;
                                    Context.Entry(_EstudioCriminodFueroFederal).State = System.Data.EntityState.Modified;
                                };


                                if (_EstudioPadre != null)
                                {
                                    short? _Estatus = new short?();
                                    if (string.IsNullOrEmpty(EntityFederal.PFF_CRIMINOLOGICO.ANTECEDENTES_PARA_ANTI_SOCIALE) || string.IsNullOrEmpty(EntityFederal.PFF_CRIMINOLOGICO.NOMBRE) ||
                                        string.IsNullOrEmpty(EntityFederal.PFF_CRIMINOLOGICO.P1_VERSION_INTERNO) || string.IsNullOrEmpty(EntityFederal.PFF_CRIMINOLOGICO.P10_CONTINUAR_TRATAMIENTO) ||
                                        string.IsNullOrEmpty(EntityFederal.PFF_CRIMINOLOGICO.P10_OPINION) || string.IsNullOrEmpty(EntityFederal.PFF_CRIMINOLOGICO.P2_PERSONALIDAD) ||
                                        string.IsNullOrEmpty(EntityFederal.PFF_CRIMINOLOGICO.P3_VALORACION) || string.IsNullOrEmpty(EntityFederal.PFF_CRIMINOLOGICO.P6_CRIMINOGENESIS) ||
                                        string.IsNullOrEmpty(EntityFederal.PFF_CRIMINOLOGICO.P7_AGRESIVIDAD) || string.IsNullOrEmpty(EntityFederal.PFF_CRIMINOLOGICO.P7_EGOCENTRISMO) ||
                                        string.IsNullOrEmpty(EntityFederal.PFF_CRIMINOLOGICO.P7_INDIFERENCIA) || string.IsNullOrEmpty(EntityFederal.PFF_CRIMINOLOGICO.P7_LABILIDAD) ||
                                        string.IsNullOrEmpty(EntityFederal.PFF_CRIMINOLOGICO.P8_ESTADO_PELIGRO) ||
                                        string.IsNullOrEmpty(EntityFederal.PFF_CRIMINOLOGICO.P8_RESULTADO_TRATAMIENTO) || string.IsNullOrEmpty(EntityFederal.PFF_CRIMINOLOGICO.P9_PRONOSTICO) ||
                                        string.IsNullOrEmpty(EntityFederal.PFF_CRIMINOLOGICO.SOBRENOMBRE) || EntityFederal.PFF_CRIMINOLOGICO.FECHA == null)
                                        _Estatus = (short)eEstatudDetallePersonalidad.ASIGNADO;
                                    else
                                        _Estatus = (short)eEstatudDetallePersonalidad.TERMINADO;

                                    var DesarrolloCriminoFederal = Context.PERSONALIDAD_DETALLE.FirstOrDefault(x => x.ID_IMPUTADO == EntityFederal.ID_IMPUTADO && x.ID_ESTUDIO == _EstudioPadre.ID_ESTUDIO && x.ID_INGRESO == EntityFederal.ID_INGRESO && x.ID_TIPO == (short)eTiposEstudio.CRIMIN_FEDERAL && x.ID_CENTRO == EntityFederal.ID_CENTRO && x.ID_ANIO == EntityFederal.ID_ANIO);
                                    if (DesarrolloCriminoFederal == null)
                                    {
                                        short _ConsecutivoPersonalidadDetalle = GetIDProceso<short>("PERSONALIDAD_DETALLE", "ID_DETALLE", "1=1");
                                        var NvoDetalle = new PERSONALIDAD_DETALLE()
                                        {
                                            ID_ANIO = EntityFederal.ID_ANIO,
                                            ID_CENTRO = EntityFederal.ID_CENTRO,
                                            ID_DETALLE = _ConsecutivoPersonalidadDetalle,
                                            ID_ESTUDIO = _EstudioPadre.ID_ESTUDIO,
                                            ID_ESTATUS = _Estatus,
                                            ID_IMPUTADO = EntityFederal.ID_IMPUTADO,
                                            ID_INGRESO = EntityFederal.ID_INGRESO,
                                            ID_TIPO = (short)eTiposEstudio.CRIMIN_FEDERAL,
                                            INICIO_FEC = GetFechaServerDate(),
                                            RESULTADO = _Estatus == (short)eEstatudDetallePersonalidad.TERMINADO ? "S" : "N",
                                            SOLICITUD_FEC = null,
                                            TERMINO_FEC = _Estatus == (short)eEstatudDetallePersonalidad.TERMINADO ? GetFechaServerDate() : new System.DateTime?(),
                                            TIPO_MEDIA = string.Empty
                                        };

                                        Context.PERSONALIDAD_DETALLE.Add(NvoDetalle);
                                    }

                                    else
                                    {
                                        DesarrolloCriminoFederal.RESULTADO = _Estatus.HasValue ? _Estatus.Value == (short)eEstatudDetallePersonalidad.TERMINADO ? "S" : "N" : "N";
                                        DesarrolloCriminoFederal.ID_ESTATUS = _Estatus;
                                        Context.Entry(DesarrolloCriminoFederal).State = System.Data.EntityState.Modified;
                                    }
                                };
                            };

                            Context.Entry(ValidaEstudio).State = System.Data.EntityState.Modified;
                            Context.SaveChanges();
                            transaccion.Complete();
                        }
                    }

                    #endregion
                    return true;
                }
            }
            catch (System.Data.Entity.Validation.DbEntityValidationException dbEx)
            {
                foreach (var validationErrors in dbEx.EntityValidationErrors)
                    foreach (var validationError in validationErrors.ValidationErrors)
                        System.Diagnostics.Trace.TraceInformation("Nombre del causante de excepción: {0} Error: {1}", validationError.PropertyName, validationError.ErrorMessage);
            }

            catch (System.Exception exc)
            {
                return false;
                //throw new System.ApplicationException(exc.Message + " " + (exc.InnerException != null ? exc.InnerException.InnerException.Message : ""));
            }

            return false;
        }

        #region se definen guardados aislados
        public short GuardaMedicoComun(PFC_II_MEDICO Entity)
        {
            try
            {
                using (System.Transactions.TransactionScope transaccion = new System.Transactions.TransactionScope(System.Transactions.TransactionScopeOption.RequiresNew, new System.Transactions.TransactionOptions() { IsolationLevel = System.Transactions.IsolationLevel.ReadCommitted }))
                {
                    var _EstudioPadre = Context.PERSONALIDAD.Where(x => x.ID_INGRESO == Entity.ID_INGRESO && x.ID_IMPUTADO == Entity.ID_IMPUTADO && x.ID_CENTRO == Entity.ID_CENTRO && x.ID_ANIO == Entity.ID_ANIO).OrderByDescending(x => x.ID_ESTUDIO).FirstOrDefault();

                    if (_EstudioPadre == null)
                        return 0;

                    var _Actual = Context.PERSONALIDAD_FUERO_COMUN.FirstOrDefault(x => x.ID_ANIO == _EstudioPadre.ID_ANIO && x.ID_CENTRO == _EstudioPadre.ID_CENTRO && x.ID_IMPUTADO == _EstudioPadre.ID_IMPUTADO && x.ID_INGRESO == _EstudioPadre.ID_INGRESO && x.ID_ESTUDIO == _EstudioPadre.ID_ESTUDIO);
                    if (_Actual == null)
                    {
                        var personalidadComun = new PERSONALIDAD_FUERO_COMUN()
                        {
                            ID_ANIO = _EstudioPadre.ID_ANIO,
                            ID_IMPUTADO = _EstudioPadre.ID_IMPUTADO,
                            ID_CENTRO = _EstudioPadre.ID_CENTRO,
                            ID_ESTUDIO = _EstudioPadre.ID_ESTUDIO,
                            ID_INGRESO = _EstudioPadre.ID_INGRESO
                        };

                        personalidadComun.PFC_II_MEDICO = new PFC_II_MEDICO()
                        {
                            ESTUDIO_FEC = Entity.ESTUDIO_FEC,
                            ID_ANIO = Entity.ID_ANIO,
                            ID_CENTRO = Entity.ID_CENTRO,
                            ID_ESTUDIO = Entity.ID_ESTUDIO,
                            ID_IMPUTADO = Entity.ID_IMPUTADO,
                            ID_INGRESO = Entity.ID_INGRESO,
                            P2_HEREDO_FAMILIARES = Entity.P2_HEREDO_FAMILIARES,
                            P3_ANTPER_NOPATO = Entity.P3_ANTPER_NOPATO,
                            P31_CONSUMO_TOXICO = Entity.P31_CONSUMO_TOXICO,
                            P32_TATUAJES_CICATRICES = Entity.P32_TATUAJES_CICATRICES,
                            P4_PATOLOGICOS = Entity.P4_PATOLOGICOS,
                            P5_PADECIMIENTOS = Entity.P5_PADECIMIENTOS,
                            P6_EXPLORACION_FISICA = Entity.P6_EXPLORACION_FISICA,
                            P7_IMPRESION_DIAGNOSTICA = Entity.P7_IMPRESION_DIAGNOSTICA,
                            P8_DICTAMEN_MEDICO = Entity.P8_DICTAMEN_MEDICO.HasValue ? Entity.P8_DICTAMEN_MEDICO != decimal.Zero ? Entity.P8_DICTAMEN_MEDICO : null : null,
                            SIGNOS_ESTATURA = Entity.SIGNOS_ESTATURA,
                            SIGNOS_PESO = Entity.SIGNOS_PESO,
                            SIGNOS_PULSO = Entity.SIGNOS_PULSO,
                            SIGNOS_RESPIRACION = Entity.SIGNOS_RESPIRACION,
                            SIGNOS_TA = Entity.SIGNOS_TA,
                            ELABORO = Entity.ELABORO,
                            COORDINADOR = Entity.COORDINADOR,
                            SIGNOS_TEMPERATURA = Entity.SIGNOS_TEMPERATURA
                        };

                        Context.PERSONALIDAD_FUERO_COMUN.Add(personalidadComun);
                        Context.PFC_II_MEDICO.Add(personalidadComun.PFC_II_MEDICO);
                    }
                    else
                    {
                        var _MedicoActual = _Actual.PFC_II_MEDICO;
                        if (_MedicoActual != null)
                        {
                            _MedicoActual.COORDINADOR = Entity.COORDINADOR;
                            _MedicoActual.ELABORO = Entity.ELABORO;
                            _MedicoActual.ESTUDIO_FEC = Entity.ESTUDIO_FEC;
                            _MedicoActual.P2_HEREDO_FAMILIARES = Entity.P2_HEREDO_FAMILIARES;
                            _MedicoActual.P3_ANTPER_NOPATO = Entity.P3_ANTPER_NOPATO;
                            _MedicoActual.P31_CONSUMO_TOXICO = Entity.P31_CONSUMO_TOXICO;
                            _MedicoActual.P32_TATUAJES_CICATRICES = Entity.P32_TATUAJES_CICATRICES;
                            _MedicoActual.P4_PATOLOGICOS = Entity.P4_PATOLOGICOS;
                            _MedicoActual.P5_PADECIMIENTOS = Entity.P5_PADECIMIENTOS;
                            _MedicoActual.P6_EXPLORACION_FISICA = Entity.P6_EXPLORACION_FISICA;
                            _MedicoActual.P7_IMPRESION_DIAGNOSTICA = Entity.P7_IMPRESION_DIAGNOSTICA;
                            _MedicoActual.P8_DICTAMEN_MEDICO = Entity.P8_DICTAMEN_MEDICO.HasValue ? Entity.P8_DICTAMEN_MEDICO != decimal.Zero ? Entity.P8_DICTAMEN_MEDICO : null : null;
                            _MedicoActual.SIGNOS_ESTATURA = Entity.SIGNOS_ESTATURA;
                            _MedicoActual.SIGNOS_PESO = Entity.SIGNOS_PESO;
                            _MedicoActual.SIGNOS_PULSO = Entity.SIGNOS_PULSO;
                            _MedicoActual.SIGNOS_RESPIRACION = Entity.SIGNOS_RESPIRACION;
                            _MedicoActual.SIGNOS_TA = Entity.SIGNOS_TA;
                            _MedicoActual.SIGNOS_TEMPERATURA = Entity.SIGNOS_TEMPERATURA;
                            Context.Entry(_MedicoActual).State = System.Data.EntityState.Modified;
                        }
                        else
                        {
                            var _NvoMed = new PFC_II_MEDICO()
                            {
                                ESTUDIO_FEC = Entity.ESTUDIO_FEC,
                                ID_ANIO = _EstudioPadre.ID_ANIO,
                                ID_CENTRO = _EstudioPadre.ID_CENTRO,
                                ID_ESTUDIO = _EstudioPadre.ID_ESTUDIO,
                                ID_IMPUTADO = _EstudioPadre.ID_IMPUTADO,
                                ID_INGRESO = _EstudioPadre.ID_INGRESO,
                                P2_HEREDO_FAMILIARES = Entity.P2_HEREDO_FAMILIARES,
                                P3_ANTPER_NOPATO = Entity.P3_ANTPER_NOPATO,
                                P31_CONSUMO_TOXICO = Entity.P31_CONSUMO_TOXICO,
                                P32_TATUAJES_CICATRICES = Entity.P32_TATUAJES_CICATRICES,
                                P4_PATOLOGICOS = Entity.P4_PATOLOGICOS,
                                P5_PADECIMIENTOS = Entity.P5_PADECIMIENTOS,
                                P6_EXPLORACION_FISICA = Entity.P6_EXPLORACION_FISICA,
                                P7_IMPRESION_DIAGNOSTICA = Entity.P7_IMPRESION_DIAGNOSTICA,
                                P8_DICTAMEN_MEDICO = Entity.P8_DICTAMEN_MEDICO.HasValue ? Entity.P8_DICTAMEN_MEDICO != decimal.Zero ? Entity.P8_DICTAMEN_MEDICO : null : null,
                                SIGNOS_ESTATURA = Entity.SIGNOS_ESTATURA,
                                SIGNOS_PESO = Entity.SIGNOS_PESO,
                                SIGNOS_PULSO = Entity.SIGNOS_PULSO,
                                SIGNOS_RESPIRACION = Entity.SIGNOS_RESPIRACION,
                                SIGNOS_TA = Entity.SIGNOS_TA,
                                ELABORO = Entity.ELABORO,
                                COORDINADOR = Entity.COORDINADOR,
                                SIGNOS_TEMPERATURA = Entity.SIGNOS_TEMPERATURA
                            };

                            Context.PFC_II_MEDICO.Add(_NvoMed);
                        };
                    }

                    #region Detalle Estudio
                    if (_EstudioPadre != null)
                    {
                        short? _Estatus = new short?();
                        var _DetalleMedico = Context.PERSONALIDAD.FirstOrDefault(x => x.ID_ESTUDIO == _EstudioPadre.ID_ESTUDIO && x.ID_ANIO == _EstudioPadre.ID_ANIO && x.ID_CENTRO == _EstudioPadre.ID_CENTRO && x.ID_IMPUTADO == _EstudioPadre.ID_IMPUTADO && x.ID_INGRESO == _EstudioPadre.ID_INGRESO);
                        if (string.IsNullOrEmpty(Entity.COORDINADOR) || string.IsNullOrEmpty(Entity.ELABORO) || string.IsNullOrEmpty(Entity.P2_HEREDO_FAMILIARES) || string.IsNullOrEmpty(Entity.P3_ANTPER_NOPATO) ||
                            string.IsNullOrEmpty(Entity.P31_CONSUMO_TOXICO) || string.IsNullOrEmpty(Entity.P32_TATUAJES_CICATRICES) || string.IsNullOrEmpty(Entity.P4_PATOLOGICOS) || string.IsNullOrEmpty(Entity.P5_PADECIMIENTOS) ||
                            string.IsNullOrEmpty(Entity.P7_IMPRESION_DIAGNOSTICA) || string.IsNullOrEmpty(Entity.SIGNOS_ESTATURA) || string.IsNullOrEmpty(Entity.SIGNOS_PESO) || string.IsNullOrEmpty(Entity.SIGNOS_PULSO) ||
                            string.IsNullOrEmpty(Entity.SIGNOS_RESPIRACION) || string.IsNullOrEmpty(Entity.SIGNOS_TA) || string.IsNullOrEmpty(Entity.SIGNOS_TEMPERATURA) || Entity.ESTUDIO_FEC == null || Entity.P8_DICTAMEN_MEDICO == null)
                            _Estatus = (short)eEstatudDetallePersonalidad.ASIGNADO;
                        else
                            _Estatus = (short)eEstatudDetallePersonalidad.TERMINADO;

                        if (_DetalleMedico != null)
                        {
                            System.DateTime? _FechaS = GetFechaServerDate();
                            var _DesarrolloEstudioMedico = Context.PERSONALIDAD_DETALLE.FirstOrDefault(c => c.ID_ESTUDIO == _DetalleMedico.ID_ESTUDIO && c.ID_TIPO == (short)eTiposEstudio.MEDICO && c.ID_IMPUTADO == _DetalleMedico.ID_IMPUTADO && c.ID_INGRESO == _DetalleMedico.ID_INGRESO && c.ID_CENTRO == _DetalleMedico.ID_CENTRO && c.ID_ANIO == _DetalleMedico.ID_ANIO);
                            if (_DesarrolloEstudioMedico == null)
                            {//No se le habia generado aun el detalle del estudio medico
                                short _ConsecutivoPersonalidadDetalle = GetIDProceso<short>("PERSONALIDAD_DETALLE", "ID_DETALLE", "1=1");
                                var _NvoDetalle = new PERSONALIDAD_DETALLE()
                                {
                                    DIAS_BONIFICADOS = null,
                                    ID_ANIO = Entity.ID_ANIO,
                                    ID_CENTRO = Entity.ID_CENTRO,
                                    ID_DETALLE = _ConsecutivoPersonalidadDetalle,
                                    ID_ESTUDIO = Entity.ID_ESTUDIO,
                                    ID_IMPUTADO = Entity.ID_IMPUTADO,
                                    ID_INGRESO = Entity.ID_INGRESO,
                                    ID_TIPO = (short)eTiposEstudio.MEDICO,
                                    INICIO_FEC = _FechaS,
                                    RESULTADO = Entity.P8_DICTAMEN_MEDICO.HasValue ? Entity.P8_DICTAMEN_MEDICO == (short)eResultado.FAVORABLE ? "S" : Entity.P8_DICTAMEN_MEDICO == (short)eResultado.DESFAVORABLE ? "N" : string.Empty : string.Empty,
                                    SOLICITUD_FEC = _EstudioPadre.SOLICITUD_FEC,
                                    ID_ESTATUS = _Estatus,
                                    TERMINO_FEC = _Estatus == (short)eEstatudDetallePersonalidad.TERMINADO ? _FechaS : new System.DateTime?(),
                                    TIPO_MEDIA = string.Empty
                                };

                                Context.PERSONALIDAD_DETALLE.Add(_NvoDetalle);
                            }

                            else
                            {
                                _DesarrolloEstudioMedico.RESULTADO = Entity.P8_DICTAMEN_MEDICO.HasValue ? Entity.P8_DICTAMEN_MEDICO == (short)eResultado.FAVORABLE ? "N" : Entity.P8_DICTAMEN_MEDICO == (short)eResultado.DESFAVORABLE ? "S" : string.Empty : string.Empty;
                                _DesarrolloEstudioMedico.ID_ESTATUS = _Estatus;
                                Context.Entry(_DesarrolloEstudioMedico).State = System.Data.EntityState.Modified;
                            }
                        };
                    }


                    Context.SaveChanges();
                    transaccion.Complete();
                    return 1;
                }
                    #endregion

            }
            catch (System.Data.Entity.Validation.DbEntityValidationException dbEx)
            {
                foreach (var validationErrors in dbEx.EntityValidationErrors)
                    foreach (var validationError in validationErrors.ValidationErrors)
                        System.Diagnostics.Trace.TraceInformation("Nombre del causante de excepción: {0} Error: {1}", validationError.PropertyName, validationError.ErrorMessage);

                return 2;
            }

            catch (System.Exception exc)
            {
                return 0;
            }
        }
        public short GuardaPsiquiatricoComun(PFC_III_PSIQUIATRICO Entity)
        {
            try
            {
                using (System.Transactions.TransactionScope transaccion = new System.Transactions.TransactionScope(System.Transactions.TransactionScopeOption.RequiresNew, new System.Transactions.TransactionOptions() { IsolationLevel = System.Transactions.IsolationLevel.ReadCommitted }))
                {
                    var _EstudioPadre = Context.PERSONALIDAD.Where(x => x.ID_INGRESO == Entity.ID_INGRESO && x.ID_IMPUTADO == Entity.ID_IMPUTADO && x.ID_CENTRO == Entity.ID_CENTRO && x.ID_ANIO == Entity.ID_ANIO).OrderByDescending(x => x.ID_ESTUDIO).FirstOrDefault();

                    if (_EstudioPadre == null)
                        return 0;

                    var _Actual = Context.PERSONALIDAD_FUERO_COMUN.FirstOrDefault(x => x.ID_ANIO == _EstudioPadre.ID_ANIO && x.ID_CENTRO == _EstudioPadre.ID_CENTRO && x.ID_IMPUTADO == _EstudioPadre.ID_IMPUTADO && x.ID_INGRESO == _EstudioPadre.ID_INGRESO && x.ID_ESTUDIO == _EstudioPadre.ID_ESTUDIO);

                    if (_Actual == null)
                    {
                        var personalidadComun = new PERSONALIDAD_FUERO_COMUN()
                        {
                            ID_ANIO = _EstudioPadre.ID_ANIO,
                            ID_IMPUTADO = _EstudioPadre.ID_IMPUTADO,
                            ID_CENTRO = _EstudioPadre.ID_CENTRO,
                            ID_ESTUDIO = _EstudioPadre.ID_ESTUDIO,
                            ID_INGRESO = _EstudioPadre.ID_INGRESO
                        };

                        var Pisq = new PFC_III_PSIQUIATRICO()
                        {
                            A1_ASPECTO_FISICO = Entity.A1_ASPECTO_FISICO,
                            A2_ESTADO_ANIMO = Entity.A2_ESTADO_ANIMO,
                            A3_ALUCINACIONES = Entity.A3_ALUCINACIONES,
                            A4_CURSO = Entity.A4_CURSO,
                            A7_BAJA_TOLERANCIA = Entity.A7_BAJA_TOLERANCIA,
                            B1_CONDUCTA_MOTORA = Entity.B1_CONDUCTA_MOTORA,
                            B2_EXPRESION_AFECTIVA = Entity.B2_EXPRESION_AFECTIVA,
                            B3_ILUSIONES = Entity.B3_ILUSIONES,
                            B4_CONTINUIDAD = Entity.B4_CONTINUIDAD,
                            B7_EXPRESION = Entity.B7_EXPRESION,
                            C1_HABLA = Entity.C1_HABLA,
                            C2_ADECUACION = Entity.C2_ADECUACION,
                            C3_DESPERSONALIZACION = Entity.C3_DESPERSONALIZACION,
                            C4_CONTENIDO = Entity.C4_CONTENIDO,
                            C7_ADECUADA = Entity.C7_ADECUADA,
                            D1_ACTITUD = Entity.D1_ACTITUD,
                            D3_DESREALIZACION = Entity.D3_DESREALIZACION,
                            D4_ABASTRACTO = Entity.D4_ABASTRACTO,
                            E4_CONCENTRACION = Entity.E4_CONCENTRACION,
                            ESTUDIO_FEC = Entity.ESTUDIO_FEC,
                            ID_ANIO = _EstudioPadre.ID_ANIO,
                            ID_CENTRO = _EstudioPadre.ID_CENTRO,
                            ID_ESTUDIO = _EstudioPadre.ID_ESTUDIO,
                            ID_IMPUTADO = _EstudioPadre.ID_IMPUTADO,
                            ID_INGRESO = _EstudioPadre.ID_INGRESO,
                            P10_FIANILIDAD = Entity.P10_FIANILIDAD,
                            P11_IMPRESION = Entity.P11_IMPRESION,
                            P12_DICTAMEN_PSIQUIATRICO = Entity.P12_DICTAMEN_PSIQUIATRICO,
                            P5_ORIENTACION = Entity.P5_ORIENTACION,
                            P6_MEMORIA = Entity.P6_MEMORIA,
                            P8_CAPACIDAD_JUICIO = Entity.P8_CAPACIDAD_JUICIO,
                            P9_INTROSPECCION = Entity.P9_INTROSPECCION,
                            MEDICO_PSIQUIATRA = Entity.MEDICO_PSIQUIATRA,
                            COORDINADOR = Entity.COORDINADOR
                        };

                        Context.PFC_III_PSIQUIATRICO.Add(Pisq);
                        Context.PERSONALIDAD_FUERO_COMUN.Add(personalidadComun);
                    }
                    else
                    {
                        var _PsiqAct = _Actual.PFC_III_PSIQUIATRICO;
                        if (_PsiqAct != null)
                        {
                            _PsiqAct.A1_ASPECTO_FISICO = Entity.A1_ASPECTO_FISICO;
                            _PsiqAct.A2_ESTADO_ANIMO = Entity.A2_ESTADO_ANIMO;
                            _PsiqAct.A3_ALUCINACIONES = Entity.A3_ALUCINACIONES;
                            _PsiqAct.A4_CURSO = Entity.A4_CURSO;
                            _PsiqAct.A7_BAJA_TOLERANCIA = Entity.A7_BAJA_TOLERANCIA;
                            _PsiqAct.B1_CONDUCTA_MOTORA = Entity.B1_CONDUCTA_MOTORA;
                            _PsiqAct.B2_EXPRESION_AFECTIVA = Entity.B2_EXPRESION_AFECTIVA;
                            _PsiqAct.B3_ILUSIONES = Entity.B3_ILUSIONES;
                            _PsiqAct.B4_CONTINUIDAD = Entity.B4_CONTINUIDAD;
                            _PsiqAct.B7_EXPRESION = Entity.B7_EXPRESION;
                            _PsiqAct.C1_HABLA = Entity.C1_HABLA;
                            _PsiqAct.C2_ADECUACION = Entity.C2_ADECUACION;
                            _PsiqAct.C3_DESPERSONALIZACION = Entity.C3_DESPERSONALIZACION;
                            _PsiqAct.C4_CONTENIDO = Entity.C4_CONTENIDO;
                            _PsiqAct.C7_ADECUADA = Entity.C7_ADECUADA;
                            _PsiqAct.COORDINADOR = Entity.COORDINADOR;
                            _PsiqAct.D1_ACTITUD = Entity.D1_ACTITUD;
                            _PsiqAct.D3_DESREALIZACION = Entity.D3_DESREALIZACION;
                            _PsiqAct.D4_ABASTRACTO = Entity.D4_ABASTRACTO;
                            _PsiqAct.E4_CONCENTRACION = Entity.E4_CONCENTRACION;
                            _PsiqAct.ESTUDIO_FEC = Entity.ESTUDIO_FEC;
                            _PsiqAct.MEDICO_PSIQUIATRA = Entity.MEDICO_PSIQUIATRA;
                            _PsiqAct.P10_FIANILIDAD = Entity.P10_FIANILIDAD;
                            _PsiqAct.P11_IMPRESION = Entity.P11_IMPRESION;
                            _PsiqAct.P12_DICTAMEN_PSIQUIATRICO = Entity.P12_DICTAMEN_PSIQUIATRICO;
                            _PsiqAct.P5_ORIENTACION = Entity.P5_ORIENTACION;
                            _PsiqAct.P6_MEMORIA = Entity.P6_MEMORIA;
                            _PsiqAct.P8_CAPACIDAD_JUICIO = Entity.P8_CAPACIDAD_JUICIO;
                            _PsiqAct.P9_INTROSPECCION = Entity.P9_INTROSPECCION;
                            Context.Entry(_PsiqAct).State = System.Data.EntityState.Modified;
                        }
                        else
                        {
                            _PsiqAct = new PFC_III_PSIQUIATRICO()
                            {
                                A1_ASPECTO_FISICO = Entity.A1_ASPECTO_FISICO,
                                A2_ESTADO_ANIMO = Entity.A2_ESTADO_ANIMO,
                                A3_ALUCINACIONES = Entity.A3_ALUCINACIONES,
                                A4_CURSO = Entity.A4_CURSO,
                                A7_BAJA_TOLERANCIA = Entity.A7_BAJA_TOLERANCIA,
                                B1_CONDUCTA_MOTORA = Entity.B1_CONDUCTA_MOTORA,
                                B2_EXPRESION_AFECTIVA = Entity.B2_EXPRESION_AFECTIVA,
                                B3_ILUSIONES = Entity.B3_ILUSIONES,
                                B4_CONTINUIDAD = Entity.B4_CONTINUIDAD,
                                B7_EXPRESION = Entity.B7_EXPRESION,
                                C1_HABLA = Entity.C1_HABLA,
                                C2_ADECUACION = Entity.C2_ADECUACION,
                                C3_DESPERSONALIZACION = Entity.C3_DESPERSONALIZACION,
                                C4_CONTENIDO = Entity.C4_CONTENIDO,
                                C7_ADECUADA = Entity.C7_ADECUADA,
                                D1_ACTITUD = Entity.D1_ACTITUD,
                                D3_DESREALIZACION = Entity.D3_DESREALIZACION,
                                D4_ABASTRACTO = Entity.D4_ABASTRACTO,
                                E4_CONCENTRACION = Entity.E4_CONCENTRACION,
                                ESTUDIO_FEC = Entity.ESTUDIO_FEC,
                                ID_ANIO = _EstudioPadre.ID_ANIO,
                                ID_CENTRO = _EstudioPadre.ID_CENTRO,
                                ID_ESTUDIO = _EstudioPadre.ID_ESTUDIO,
                                ID_IMPUTADO = _EstudioPadre.ID_IMPUTADO,
                                ID_INGRESO = _EstudioPadre.ID_INGRESO,
                                P10_FIANILIDAD = Entity.P10_FIANILIDAD,
                                P11_IMPRESION = Entity.P11_IMPRESION,
                                P12_DICTAMEN_PSIQUIATRICO = Entity.P12_DICTAMEN_PSIQUIATRICO,
                                P5_ORIENTACION = Entity.P5_ORIENTACION,
                                P6_MEMORIA = Entity.P6_MEMORIA,
                                P8_CAPACIDAD_JUICIO = Entity.P8_CAPACIDAD_JUICIO,
                                P9_INTROSPECCION = Entity.P9_INTROSPECCION,
                                MEDICO_PSIQUIATRA = Entity.MEDICO_PSIQUIATRA,
                                COORDINADOR = Entity.COORDINADOR
                            };

                            Context.PFC_III_PSIQUIATRICO.Add(_PsiqAct);
                        };
                    }

                    #region Detalle Psiq
                    if (_EstudioPadre != null)
                    {
                        short? _Estatus = new short?();
                        var _DetallePsiq = Context.PERSONALIDAD.FirstOrDefault(x => x.ID_ESTUDIO == _EstudioPadre.ID_ESTUDIO && x.ID_ANIO == _EstudioPadre.ID_ANIO && x.ID_CENTRO == _EstudioPadre.ID_CENTRO && x.ID_IMPUTADO == _EstudioPadre.ID_IMPUTADO && x.ID_INGRESO == _EstudioPadre.ID_INGRESO);
                        if (string.IsNullOrEmpty(Entity.A1_ASPECTO_FISICO) || string.IsNullOrEmpty(Entity.A2_ESTADO_ANIMO) || string.IsNullOrEmpty(Entity.A3_ALUCINACIONES) || string.IsNullOrEmpty(Entity.A4_CURSO) || string.IsNullOrEmpty(Entity.A7_BAJA_TOLERANCIA) || string.IsNullOrEmpty(Entity.B1_CONDUCTA_MOTORA) || string.IsNullOrEmpty(Entity.B2_EXPRESION_AFECTIVA) || string.IsNullOrEmpty(Entity.B3_ILUSIONES) || string.IsNullOrEmpty(Entity.B4_CONTINUIDAD) || string.IsNullOrEmpty(Entity.B7_EXPRESION) || string.IsNullOrEmpty(Entity.C1_HABLA) || string.IsNullOrEmpty(Entity.C2_ADECUACION) || string.IsNullOrEmpty(Entity.C3_DESPERSONALIZACION) || string.IsNullOrEmpty(Entity.C4_CONTENIDO) || string.IsNullOrEmpty(Entity.C7_ADECUADA) || string.IsNullOrEmpty(Entity.COORDINADOR) || string.IsNullOrEmpty(Entity.D1_ACTITUD) ||
                            string.IsNullOrEmpty(Entity.D3_DESREALIZACION) || string.IsNullOrEmpty(Entity.D4_ABASTRACTO) || string.IsNullOrEmpty(Entity.E4_CONCENTRACION) || string.IsNullOrEmpty(Entity.MEDICO_PSIQUIATRA) || string.IsNullOrEmpty(Entity.P10_FIANILIDAD) || string.IsNullOrEmpty(Entity.P11_IMPRESION) || string.IsNullOrEmpty(Entity.P5_ORIENTACION) || string.IsNullOrEmpty(Entity.P6_MEMORIA) || string.IsNullOrEmpty(Entity.P8_CAPACIDAD_JUICIO) ||
                            string.IsNullOrEmpty(Entity.P9_INTROSPECCION) || Entity.ESTUDIO_FEC == null || Entity.P12_DICTAMEN_PSIQUIATRICO == null)
                            _Estatus = (short)eEstatudDetallePersonalidad.ASIGNADO;
                        else
                            _Estatus = (short)eEstatudDetallePersonalidad.TERMINADO;

                        if (_DetallePsiq != null)
                        {
                            var _DesarrolloEstudioPsiq = Context.PERSONALIDAD_DETALLE.FirstOrDefault(c => c.ID_ESTUDIO == _DetallePsiq.ID_ESTUDIO && c.ID_TIPO == (short)eTiposEstudio.PSIQUIATRIA && c.ID_IMPUTADO == _DetallePsiq.ID_IMPUTADO && c.ID_INGRESO == _DetallePsiq.ID_INGRESO && c.ID_CENTRO == _DetallePsiq.ID_CENTRO && c.ID_ANIO == _DetallePsiq.ID_ANIO);
                            if (_DesarrolloEstudioPsiq == null)
                            {
                                short _ConsecutivoPersonalidadDetalle = GetIDProceso<short>("PERSONALIDAD_DETALLE", "ID_DETALLE", "1=1");
                                var _NvoDetalle = new PERSONALIDAD_DETALLE()
                                {
                                    DIAS_BONIFICADOS = null,
                                    ID_ANIO = Entity.ID_ANIO,
                                    ID_CENTRO = Entity.ID_CENTRO,
                                    ID_DETALLE = _ConsecutivoPersonalidadDetalle,
                                    ID_ESTUDIO = _EstudioPadre.ID_ESTUDIO,
                                    ID_IMPUTADO = Entity.ID_IMPUTADO,
                                    ID_INGRESO = Entity.ID_INGRESO,
                                    ID_TIPO = (short)eTiposEstudio.PSIQUIATRIA,
                                    INICIO_FEC = GetFechaServerDate(),
                                    RESULTADO = Entity.P12_DICTAMEN_PSIQUIATRICO.HasValue ? Entity.P12_DICTAMEN_PSIQUIATRICO == (short)eResultado.FAVORABLE ? "S" : Entity.P12_DICTAMEN_PSIQUIATRICO == (short)eResultado.DESFAVORABLE ? "N" : string.Empty : string.Empty,
                                    SOLICITUD_FEC = _EstudioPadre.SOLICITUD_FEC,
                                    ID_ESTATUS = _Estatus,
                                    TERMINO_FEC = _Estatus == (short)eEstatudDetallePersonalidad.TERMINADO ? GetFechaServerDate() : new System.DateTime?(),
                                    TIPO_MEDIA = string.Empty
                                };

                                Context.PERSONALIDAD_DETALLE.Add(_NvoDetalle);
                            }

                            else
                            {
                                _DesarrolloEstudioPsiq.RESULTADO = Entity.P12_DICTAMEN_PSIQUIATRICO.HasValue ? Entity.P12_DICTAMEN_PSIQUIATRICO == (short)eResultado.FAVORABLE ? "S" : Entity.P12_DICTAMEN_PSIQUIATRICO == (short)eResultado.DESFAVORABLE ? "N" : string.Empty : string.Empty;
                                _DesarrolloEstudioPsiq.ID_ESTATUS = _Estatus;
                                Context.Entry(_DesarrolloEstudioPsiq).State = System.Data.EntityState.Modified;
                            }
                        }
                    }


                    #endregion

                    Context.SaveChanges();
                    transaccion.Complete();
                    return 1;
                }
        #endregion

            }
            catch (System.Data.Entity.Validation.DbEntityValidationException dbEx)
            {
                foreach (var validationErrors in dbEx.EntityValidationErrors)
                    foreach (var validationError in validationErrors.ValidationErrors)
                        System.Diagnostics.Trace.TraceInformation("Nombre del causante de excepción: {0} Error: {1}", validationError.PropertyName, validationError.ErrorMessage);

                return 2;
            }

            catch (System.Exception exc)
            {
                return 0;
            }
        }

        public short GuardaPsicologicoComun(PFC_IV_PSICOLOGICO Entity)
        {
            try
            {
                using (System.Transactions.TransactionScope transaccion = new System.Transactions.TransactionScope(System.Transactions.TransactionScopeOption.RequiresNew, new System.Transactions.TransactionOptions() { IsolationLevel = System.Transactions.IsolationLevel.ReadCommitted }))
                {
                    var _EstudioPadre = Context.PERSONALIDAD.Where(x => x.ID_INGRESO == Entity.ID_INGRESO && x.ID_IMPUTADO == Entity.ID_IMPUTADO && x.ID_CENTRO == Entity.ID_CENTRO && x.ID_ANIO == Entity.ID_ANIO).OrderByDescending(x => x.ID_ESTUDIO).FirstOrDefault();

                    if (_EstudioPadre == null)
                        return 0;

                    var _Actual = Context.PERSONALIDAD_FUERO_COMUN.FirstOrDefault(x => x.ID_ANIO == _EstudioPadre.ID_ANIO && x.ID_CENTRO == _EstudioPadre.ID_CENTRO && x.ID_IMPUTADO == _EstudioPadre.ID_IMPUTADO && x.ID_INGRESO == _EstudioPadre.ID_INGRESO && x.ID_ESTUDIO == _EstudioPadre.ID_ESTUDIO);

                    if (_Actual == null)
                    {
                        var personalidadComun = new PERSONALIDAD_FUERO_COMUN()
                        {
                            ID_ANIO = _EstudioPadre.ID_ANIO,
                            ID_IMPUTADO = _EstudioPadre.ID_IMPUTADO,
                            ID_CENTRO = _EstudioPadre.ID_CENTRO,
                            ID_ESTUDIO = _EstudioPadre.ID_ESTUDIO,
                            ID_INGRESO = _EstudioPadre.ID_INGRESO
                        };


                        #region Psicolog
                        PFC_IV_PSICOLOGICO _Psico = new PFC_IV_PSICOLOGICO()
                        {
                            ESTUDIO_FEC = Entity.ESTUDIO_FEC,
                            ID_ANIO = _EstudioPadre.ID_ANIO,
                            ID_CENTRO = _EstudioPadre.ID_CENTRO,
                            ID_ESTUDIO = _EstudioPadre.ID_ESTUDIO,
                            ID_IMPUTADO = _EstudioPadre.ID_IMPUTADO,
                            ID_INGRESO = _EstudioPadre.ID_INGRESO,
                            P1_CONDICIONES_GRALES = Entity.P1_CONDICIONES_GRALES,
                            P10_MOTIVACION_DICTAMEN = Entity.P10_MOTIVACION_DICTAMEN,
                            P11_CASO_NEGATIVO = Entity.P11_CASO_NEGATIVO,
                            P12_CUAL = Entity.P12_CUAL,
                            P12_REQUIERE_TRATAMIENTO = Entity.P12_REQUIERE_TRATAMIENTO,
                            P2_EXAMEN_MENTAL = Entity.P2_EXAMEN_MENTAL,
                            P3_PRINCIPALES_RASGOS = Entity.P3_PRINCIPALES_RASGOS,
                            P4_INVENTARIO_MULTIFASICO = Entity.P4_INVENTARIO_MULTIFASICO,
                            P4_OTRAS = Entity.P4_OTRAS,
                            P4_TEST_GUALTICO = Entity.P4_TEST_GUALTICO,
                            P4_TEST_HTP = Entity.P4_TEST_HTP,
                            P4_TEST_MATRICES = Entity.P4_TEST_MATRICES,
                            P51_NIVEL_INTELECTUAL = Entity.P51_NIVEL_INTELECTUAL.HasValue ? Entity.P51_NIVEL_INTELECTUAL.Value != -1 ? Entity.P51_NIVEL_INTELECTUAL : null : null,
                            P52_DISFUNCION_NEUROLOGICA = Entity.P52_DISFUNCION_NEUROLOGICA.HasValue ? Entity.P52_DISFUNCION_NEUROLOGICA.Value != -1 ? Entity.P52_DISFUNCION_NEUROLOGICA : null : null,
                            P9_DICTAMEN_REINSERCION = Entity.P9_DICTAMEN_REINSERCION.HasValue ? Entity.P9_DICTAMEN_REINSERCION.Value != decimal.Zero ? Entity.P9_DICTAMEN_REINSERCION : null : null,
                            P8_RASGOS_PERSONALIDAD = Entity.P8_RASGOS_PERSONALIDAD,
                            COORDINADOR = Entity.COORDINADOR,
                            P6_INTEGRACION = Entity.P6_INTEGRACION,
                            ELABORO = Entity.ELABORO,
                            P4_OTRA_MENCIONAR = Entity.P4_OTRA_MENCIONAR
                        };

                        var _consecutivoPsicologicoComun = GetIDProceso<short>("PFC_IV_PROGRAMA", "ID_CONSEC", string.Format("ID_CENTRO = {0} AND ID_ANIO = {1} AND ID_IMPUTADO = {2} AND ID_INGRESO = {3} AND ID_ESTUDIO = {4}", Entity.ID_CENTRO, Entity.ID_ANIO, Entity.ID_IMPUTADO, Entity.ID_INGRESO, Entity.ID_ESTUDIO));
                        if (Entity.PFC_IV_PROGRAMA != null)
                            if (Entity.PFC_IV_PROGRAMA.Any())
                                foreach (var item in Entity.PFC_IV_PROGRAMA)
                                {
                                    var _NuevoPrograma = new PFC_IV_PROGRAMA()
                                    {
                                        CONCLUYO = item.CONCLUYO,
                                        DURACION = item.DURACION,
                                        ID_ACTIVIDAD = item.ID_ACTIVIDAD,
                                        ID_ANIO = _EstudioPadre.ID_ANIO,
                                        ID_CENTRO = _EstudioPadre.ID_CENTRO,
                                        ID_ESTUDIO = _EstudioPadre.ID_ESTUDIO,
                                        ID_IMPUTADO = _EstudioPadre.ID_IMPUTADO,
                                        ID_INGRESO = _EstudioPadre.ID_INGRESO,
                                        ID_TIPO_PROGRAMA = item.ID_TIPO_PROGRAMA,
                                        ID_CONSEC = _consecutivoPsicologicoComun,
                                        OBSERVACION = item.OBSERVACION
                                    };

                                    Context.PFC_IV_PROGRAMA.Add(_NuevoPrograma);
                                    _consecutivoPsicologicoComun++;
                                };

                        Context.PFC_IV_PSICOLOGICO.Add(_Psico);
                        Context.PERSONALIDAD_FUERO_COMUN.Add(personalidadComun);
                        #endregion

                    }
                    else
                    {
                        var _PsicoAct = _Actual.PFC_IV_PSICOLOGICO;

                        if (_PsicoAct != null)
                        {
                            var _ProgramasPsicologico = Context.PFC_IV_PROGRAMA.Where(x => x.ID_IMPUTADO == _EstudioPadre.ID_IMPUTADO && x.ID_INGRESO == _EstudioPadre.ID_INGRESO && x.ID_ESTUDIO == _Actual.ID_ESTUDIO && x.ID_CENTRO == _EstudioPadre.ID_CENTRO);
                            _PsicoAct.COORDINADOR = Entity.COORDINADOR;
                            _PsicoAct.ELABORO = Entity.ELABORO;
                            _PsicoAct.ESTUDIO_FEC = Entity.ESTUDIO_FEC;
                            _PsicoAct.P1_CONDICIONES_GRALES = Entity.P1_CONDICIONES_GRALES;
                            _PsicoAct.P10_MOTIVACION_DICTAMEN = Entity.P10_MOTIVACION_DICTAMEN;
                            _PsicoAct.P11_CASO_NEGATIVO = Entity.P11_CASO_NEGATIVO;
                            _PsicoAct.P12_CUAL = Entity.P12_CUAL;
                            _PsicoAct.P12_REQUIERE_TRATAMIENTO = string.IsNullOrEmpty(Entity.P12_REQUIERE_TRATAMIENTO) ? Entity.P12_REQUIERE_TRATAMIENTO : string.Empty;
                            _PsicoAct.P2_EXAMEN_MENTAL = Entity.P2_EXAMEN_MENTAL;
                            _PsicoAct.P3_PRINCIPALES_RASGOS = Entity.P3_PRINCIPALES_RASGOS;
                            _PsicoAct.P4_INVENTARIO_MULTIFASICO = Entity.P4_INVENTARIO_MULTIFASICO;
                            _PsicoAct.P4_OTRAS = Entity.P4_OTRAS;
                            _PsicoAct.P4_OTRA_MENCIONAR = Entity.P4_OTRA_MENCIONAR;
                            _PsicoAct.P4_TEST_GUALTICO = Entity.P4_TEST_GUALTICO;
                            _PsicoAct.P4_TEST_HTP = Entity.P4_TEST_HTP;
                            _PsicoAct.P4_TEST_MATRICES = Entity.P4_TEST_MATRICES;
                            _PsicoAct.P51_NIVEL_INTELECTUAL = Entity.P51_NIVEL_INTELECTUAL.HasValue ? Entity.P51_NIVEL_INTELECTUAL != -1 ? Entity.P51_NIVEL_INTELECTUAL : null : null;
                            _PsicoAct.P52_DISFUNCION_NEUROLOGICA = Entity.P52_DISFUNCION_NEUROLOGICA.HasValue ? Entity.P52_DISFUNCION_NEUROLOGICA != -1 ? Entity.P52_DISFUNCION_NEUROLOGICA : null : null;
                            _PsicoAct.P6_INTEGRACION = Entity.P6_INTEGRACION;
                            _PsicoAct.P8_RASGOS_PERSONALIDAD = Entity.P8_RASGOS_PERSONALIDAD;
                            _PsicoAct.P9_DICTAMEN_REINSERCION = Entity.P9_DICTAMEN_REINSERCION.HasValue ? Entity.P9_DICTAMEN_REINSERCION != decimal.Zero ? Entity.P9_DICTAMEN_REINSERCION : null : null;
                            Context.Entry(_PsicoAct).State = System.Data.EntityState.Modified;

                            #region Programas IV

                            if (_ProgramasPsicologico != null && _ProgramasPsicologico.Any())
                                foreach (var item in _ProgramasPsicologico)
                                    Context.Entry(item).State = System.Data.EntityState.Deleted;

                            var _consecutivoPsicologicoComun = GetIDProceso<short>("PFC_IV_PROGRAMA", "ID_CONSEC", string.Format("ID_CENTRO = {0} AND ID_ANIO = {1} AND ID_IMPUTADO = {2} AND ID_INGRESO = {3} AND ID_ESTUDIO = {4}", Entity.ID_CENTRO, Entity.ID_ANIO, Entity.ID_IMPUTADO, Entity.ID_INGRESO, Entity.ID_ESTUDIO));
                            if (Entity.PFC_IV_PROGRAMA != null && Entity.PFC_IV_PROGRAMA.Any())
                            {
                                foreach (var item in Entity.PFC_IV_PROGRAMA)
                                {
                                    var _NuevoProgramaPsicologico = new PFC_IV_PROGRAMA()
                                    {
                                        CONCLUYO = item.CONCLUYO,
                                        DURACION = item.DURACION,
                                        ID_ACTIVIDAD = item.ID_ACTIVIDAD,
                                        ID_ANIO = _EstudioPadre.ID_ANIO,
                                        ID_CENTRO = _EstudioPadre.ID_CENTRO,
                                        ID_ESTUDIO = _EstudioPadre.ID_ESTUDIO,
                                        ID_IMPUTADO = _EstudioPadre.ID_IMPUTADO,
                                        ID_INGRESO = _EstudioPadre.ID_INGRESO,
                                        ID_TIPO_PROGRAMA = item.ID_TIPO_PROGRAMA,
                                        OBSERVACION = item.OBSERVACION,
                                        ID_CONSEC = _consecutivoPsicologicoComun
                                    };

                                    _PsicoAct.PFC_IV_PROGRAMA.Add(_NuevoProgramaPsicologico);
                                    _consecutivoPsicologicoComun++;
                                };
                            };
                        }
                        else
                        {
                            _PsicoAct = new PFC_IV_PSICOLOGICO()
                            {
                                ESTUDIO_FEC = Entity.ESTUDIO_FEC,
                                ID_ANIO = _EstudioPadre.ID_ANIO,
                                ID_CENTRO = _EstudioPadre.ID_CENTRO,
                                ID_ESTUDIO = _EstudioPadre.ID_ESTUDIO,
                                ID_IMPUTADO = _EstudioPadre.ID_IMPUTADO,
                                ID_INGRESO = _EstudioPadre.ID_INGRESO,
                                P1_CONDICIONES_GRALES = Entity.P1_CONDICIONES_GRALES,
                                P10_MOTIVACION_DICTAMEN = Entity.P10_MOTIVACION_DICTAMEN,
                                P11_CASO_NEGATIVO = Entity.P11_CASO_NEGATIVO,
                                P12_CUAL = Entity.P12_CUAL,
                                P12_REQUIERE_TRATAMIENTO = Entity.P12_REQUIERE_TRATAMIENTO,
                                P2_EXAMEN_MENTAL = Entity.P2_EXAMEN_MENTAL,
                                P3_PRINCIPALES_RASGOS = Entity.P3_PRINCIPALES_RASGOS,
                                P4_INVENTARIO_MULTIFASICO = Entity.P4_INVENTARIO_MULTIFASICO,
                                P4_OTRAS = Entity.P4_OTRAS,
                                P4_TEST_GUALTICO = Entity.P4_TEST_GUALTICO,
                                P4_TEST_HTP = Entity.P4_TEST_HTP,
                                P4_TEST_MATRICES = Entity.P4_TEST_MATRICES,
                                P51_NIVEL_INTELECTUAL = Entity.P51_NIVEL_INTELECTUAL.HasValue ? Entity.P51_NIVEL_INTELECTUAL.Value != -1 ? Entity.P51_NIVEL_INTELECTUAL : null : null,
                                P52_DISFUNCION_NEUROLOGICA = Entity.P52_DISFUNCION_NEUROLOGICA.HasValue ? Entity.P52_DISFUNCION_NEUROLOGICA.Value != -1 ? Entity.P52_DISFUNCION_NEUROLOGICA : null : null,
                                P9_DICTAMEN_REINSERCION = Entity.P9_DICTAMEN_REINSERCION.HasValue ? Entity.P9_DICTAMEN_REINSERCION.Value != decimal.Zero ? Entity.P9_DICTAMEN_REINSERCION : null : null,
                                P8_RASGOS_PERSONALIDAD = Entity.P8_RASGOS_PERSONALIDAD,
                                COORDINADOR = Entity.COORDINADOR,
                                P6_INTEGRACION = Entity.P6_INTEGRACION,
                                ELABORO = Entity.ELABORO,
                                P4_OTRA_MENCIONAR = Entity.P4_OTRA_MENCIONAR
                            };

                            var _consecutivoPsicologicoComun = GetIDProceso<short>("PFC_IV_PROGRAMA", "ID_CONSEC", string.Format("ID_CENTRO = {0} AND ID_ANIO = {1} AND ID_IMPUTADO = {2} AND ID_INGRESO = {3} AND ID_ESTUDIO = {4}", Entity.ID_CENTRO, Entity.ID_ANIO, Entity.ID_IMPUTADO, Entity.ID_INGRESO, Entity.ID_ESTUDIO));
                            if (Entity.PFC_IV_PROGRAMA != null)
                                if (Entity.PFC_IV_PROGRAMA.Any())
                                    foreach (var item in Entity.PFC_IV_PROGRAMA)
                                    {
                                        var _NuevoPrograma = new PFC_IV_PROGRAMA()
                                        {
                                            CONCLUYO = item.CONCLUYO,
                                            DURACION = item.DURACION,
                                            ID_ACTIVIDAD = item.ID_ACTIVIDAD,
                                            ID_ANIO = _EstudioPadre.ID_ANIO,
                                            ID_CENTRO = _EstudioPadre.ID_CENTRO,
                                            ID_ESTUDIO = _EstudioPadre.ID_ESTUDIO,
                                            ID_IMPUTADO = _EstudioPadre.ID_IMPUTADO,
                                            ID_INGRESO = _EstudioPadre.ID_INGRESO,
                                            ID_TIPO_PROGRAMA = item.ID_TIPO_PROGRAMA,
                                            ID_CONSEC = _consecutivoPsicologicoComun,
                                            OBSERVACION = item.OBSERVACION
                                        };

                                        Context.PFC_IV_PROGRAMA.Add(_NuevoPrograma);
                                        _consecutivoPsicologicoComun++;
                                    };

                            Context.PFC_IV_PSICOLOGICO.Add(_PsicoAct);
                        };
                    }

                            #endregion


                    #region Detalle Psico

                    if (_EstudioPadre != null)
                    {
                        short? _Estatus = new short?();
                        var _DetallePsic = Context.PERSONALIDAD.FirstOrDefault(x => x.ID_ESTUDIO == _EstudioPadre.ID_ESTUDIO && x.ID_ANIO == _EstudioPadre.ID_ANIO && x.ID_CENTRO == _EstudioPadre.ID_CENTRO && x.ID_IMPUTADO == _EstudioPadre.ID_IMPUTADO && x.ID_INGRESO == _EstudioPadre.ID_INGRESO);
                        if (string.IsNullOrEmpty(Entity.COORDINADOR) || string.IsNullOrEmpty(Entity.ELABORO) || string.IsNullOrEmpty(Entity.P1_CONDICIONES_GRALES) || string.IsNullOrEmpty(Entity.P10_MOTIVACION_DICTAMEN) || string.IsNullOrEmpty(Entity.P2_EXAMEN_MENTAL) || string.IsNullOrEmpty(Entity.P3_PRINCIPALES_RASGOS) || string.IsNullOrEmpty(Entity.P6_INTEGRACION) || string.IsNullOrEmpty(Entity.P8_RASGOS_PERSONALIDAD) ||
                            Entity.ESTUDIO_FEC == null || Entity.P51_NIVEL_INTELECTUAL == null || Entity.P52_DISFUNCION_NEUROLOGICA == null || Entity.P9_DICTAMEN_REINSERCION == null)
                            _Estatus = (short)eEstatudDetallePersonalidad.ASIGNADO;
                        else
                            _Estatus = (short)eEstatudDetallePersonalidad.TERMINADO;


                        if (_DetallePsic != null)
                        {
                            var _DesarrolloEstudioPsiq = Context.PERSONALIDAD_DETALLE.FirstOrDefault(c => c.ID_ESTUDIO == _DetallePsic.ID_ESTUDIO && c.ID_TIPO == (short)eTiposEstudio.PSICOLOGIA && c.ID_IMPUTADO == _DetallePsic.ID_IMPUTADO && c.ID_INGRESO == _DetallePsic.ID_INGRESO && c.ID_CENTRO == _DetallePsic.ID_CENTRO && c.ID_ANIO == _DetallePsic.ID_ANIO);
                            if (_DesarrolloEstudioPsiq == null)
                            {
                                short _ConsecutivoPersonalidadDetalle = GetIDProceso<short>("PERSONALIDAD_DETALLE", "ID_DETALLE", "1=1");
                                var _NvoDetalle = new PERSONALIDAD_DETALLE()
                                {
                                    DIAS_BONIFICADOS = null,
                                    ID_ANIO = Entity.ID_ANIO,
                                    ID_CENTRO = Entity.ID_CENTRO,
                                    ID_DETALLE = _ConsecutivoPersonalidadDetalle,
                                    ID_ESTUDIO = _EstudioPadre.ID_ESTUDIO,
                                    ID_IMPUTADO = Entity.ID_IMPUTADO,
                                    ID_INGRESO = Entity.ID_INGRESO,
                                    ID_TIPO = (short)eTiposEstudio.PSICOLOGIA,
                                    INICIO_FEC = GetFechaServerDate(),
                                    RESULTADO = Entity.P9_DICTAMEN_REINSERCION.HasValue ? Entity.P9_DICTAMEN_REINSERCION == (short)eResultado.FAVORABLE ? "N" : Entity.P9_DICTAMEN_REINSERCION == (short)eResultado.DESFAVORABLE ? "N" : string.Empty : string.Empty,
                                    SOLICITUD_FEC = _EstudioPadre.SOLICITUD_FEC,
                                    ID_ESTATUS = _Estatus,
                                    TERMINO_FEC = _Estatus == (short)eEstatudDetallePersonalidad.TERMINADO ? GetFechaServerDate() : new System.DateTime?(),
                                    TIPO_MEDIA = string.Empty
                                };

                                Context.PERSONALIDAD_DETALLE.Add(_NvoDetalle);
                            }

                            else
                            {
                                _DesarrolloEstudioPsiq.RESULTADO = Entity.P9_DICTAMEN_REINSERCION.HasValue ? Entity.P9_DICTAMEN_REINSERCION == (short)eResultado.FAVORABLE ? "N" : Entity.P9_DICTAMEN_REINSERCION == (short)eResultado.DESFAVORABLE ? "N" : string.Empty : string.Empty;
                                _DesarrolloEstudioPsiq.ID_ESTATUS = _Estatus;
                                Context.Entry(_DesarrolloEstudioPsiq).State = System.Data.EntityState.Modified;
                            };
                        };
                    };

                    Context.SaveChanges();
                    transaccion.Complete();
                    return 1;

                    #endregion

                }
            }
            catch (System.Data.Entity.Validation.DbEntityValidationException dbEx)
            {
                foreach (var validationErrors in dbEx.EntityValidationErrors)
                    foreach (var validationError in validationErrors.ValidationErrors)
                        System.Diagnostics.Trace.TraceInformation("Nombre del causante de excepción: {0} Error: {1}", validationError.PropertyName, validationError.ErrorMessage);

                return 2;
            }

            catch (System.Exception exc)
            {
                return 0;
            }
        }

        public short GuardaCriminoDiagnoticoComun(PFC_V_CRIMINODIAGNOSTICO Entity)
        {
            try
            {
                using (System.Transactions.TransactionScope transaccion = new System.Transactions.TransactionScope(System.Transactions.TransactionScopeOption.RequiresNew, new System.Transactions.TransactionOptions() { IsolationLevel = System.Transactions.IsolationLevel.ReadCommitted }))
                {
                    var _EstudioPadre = Context.PERSONALIDAD.Where(x => x.ID_INGRESO == Entity.ID_INGRESO && x.ID_IMPUTADO == Entity.ID_IMPUTADO && x.ID_CENTRO == Entity.ID_CENTRO && x.ID_ANIO == Entity.ID_ANIO).OrderByDescending(x => x.ID_ESTUDIO).FirstOrDefault();

                    if (_EstudioPadre == null)
                        return 0;

                    var _Actual = Context.PERSONALIDAD_FUERO_COMUN.FirstOrDefault(x => x.ID_ANIO == _EstudioPadre.ID_ANIO && x.ID_CENTRO == _EstudioPadre.ID_CENTRO && x.ID_IMPUTADO == _EstudioPadre.ID_IMPUTADO && x.ID_INGRESO == _EstudioPadre.ID_INGRESO && x.ID_ESTUDIO == _EstudioPadre.ID_ESTUDIO);

                    if (_Actual == null)
                    {
                        var personalidadComun = new PERSONALIDAD_FUERO_COMUN()
                        {
                            ID_ANIO = _EstudioPadre.ID_ANIO,
                            ID_IMPUTADO = _EstudioPadre.ID_IMPUTADO,
                            ID_CENTRO = _EstudioPadre.ID_CENTRO,
                            ID_ESTUDIO = _EstudioPadre.ID_ESTUDIO,
                            ID_INGRESO = _EstudioPadre.ID_INGRESO
                        };

                        var _Crimin = new PFC_V_CRIMINODIAGNOSTICO()
                        {
                            ESTUDIO_FEC = Entity.ESTUDIO_FEC,
                            ID_ANIO = _EstudioPadre.ID_ANIO,
                            ID_IMPUTADO = _EstudioPadre.ID_IMPUTADO,
                            ID_CENTRO = _EstudioPadre.ID_CENTRO,
                            ID_ESTUDIO = _EstudioPadre.ID_ESTUDIO,
                            ID_INGRESO = _EstudioPadre.ID_INGRESO,
                            P1_ALCOHOL = Entity.P1_ALCOHOL,
                            P1_DROGADO = string.IsNullOrEmpty(Entity.P1_DROGADO) ? Entity.P1_DROGADO : string.Empty,
                            P1_DROGRA_ILEGAL = Entity.P1_DROGRA_ILEGAL,
                            P1_OTRA = Entity.P1_OTRA,
                            P1_VERSION_DELITO = Entity.P1_VERSION_DELITO,
                            P10_DICTAMEN_REINSERCION = Entity.P10_DICTAMEN_REINSERCION.HasValue ? Entity.P10_DICTAMEN_REINSERCION.Value != decimal.Zero ? Entity.P10_DICTAMEN_REINSERCION : null : null,
                            P10_MOTIVACION_DICTAMEN = Entity.P10_MOTIVACION_DICTAMEN,
                            P11_PROGRAMAS_REMITIRSE = Entity.P11_PROGRAMAS_REMITIRSE,
                            P12_CUAL = Entity.P12_CUAL,
                            P12_TRATAMIENTO_EXTRAMUROS = string.IsNullOrEmpty(Entity.P12_TRATAMIENTO_EXTRAMUROS) ? Entity.P12_TRATAMIENTO_EXTRAMUROS : string.Empty,
                            P2_CRIMINOGENESIS = Entity.P2_CRIMINOGENESIS,
                            P3_CONDUCTA_ANTISOCIAL = Entity.P3_CONDUCTA_ANTISOCIAL,
                            P4_CLASIFICACION_CRIMINOLOGICA = Entity.P4_CLASIFICACION_CRIMINOLOGICA.HasValue ? Entity.P4_CLASIFICACION_CRIMINOLOGICA.Value != -1 ? Entity.P4_CLASIFICACION_CRIMINOLOGICA : null : null,
                            P5_INTIMIDACION = string.IsNullOrEmpty(Entity.P5_INTIMIDACION) ? Entity.P5_INTIMIDACION : string.Empty,
                            P5_PORQUE = Entity.P5_PORQUE,
                            P6_CAPACIDAD_CRIMINAL = Entity.P6_CAPACIDAD_CRIMINAL.HasValue ? Entity.P6_CAPACIDAD_CRIMINAL.Value != -1 ? Entity.P6_CAPACIDAD_CRIMINAL : null : null,
                            P6A_EGOCENTRICO = Entity.P6A_EGOCENTRICO.HasValue ? Entity.P6A_EGOCENTRICO.Value != -1 ? Entity.P6A_EGOCENTRICO : null : null,
                            P6B_LIABILIDAD_EFECTIVA = Entity.P6B_LIABILIDAD_EFECTIVA.HasValue ? Entity.P6B_LIABILIDAD_EFECTIVA.Value != -1 ? Entity.P6B_LIABILIDAD_EFECTIVA : null : null,
                            P6C_AGRESIVIDAD = Entity.P6C_AGRESIVIDAD.HasValue ? Entity.P6C_AGRESIVIDAD.Value != -1 ? Entity.P6C_AGRESIVIDAD : null : null,
                            P6D_INDIFERENCIA_AFECTIVA = Entity.P6D_INDIFERENCIA_AFECTIVA.HasValue ? Entity.P6D_INDIFERENCIA_AFECTIVA.Value != -1 ? Entity.P6D_INDIFERENCIA_AFECTIVA : null : null,
                            P7_ADAPTACION_SOCIAL = Entity.P7_ADAPTACION_SOCIAL.HasValue ? Entity.P7_ADAPTACION_SOCIAL.Value != -1 ? Entity.P7_ADAPTACION_SOCIAL : null : null,
                            P8_INDICE_PELIGROSIDAD = Entity.P8_INDICE_PELIGROSIDAD.HasValue ? Entity.P8_INDICE_PELIGROSIDAD.Value != -1 ? Entity.P8_INDICE_PELIGROSIDAD : null : null,
                            P9_PRONOSTICO_REINCIDENCIA = Entity.P9_PRONOSTICO_REINCIDENCIA.HasValue ? Entity.P9_PRONOSTICO_REINCIDENCIA.Value != -1 ? Entity.P9_PRONOSTICO_REINCIDENCIA : null : null,
                            COORDINADOR = Entity.COORDINADOR,
                            ELABORO = Entity.ELABORO
                        };

                        Context.PERSONALIDAD_FUERO_COMUN.Add(personalidadComun);
                        Context.PFC_V_CRIMINODIAGNOSTICO.Add(_Crimin);
                    }
                    else
                    {
                        var _ActualCrim = _Actual.PFC_V_CRIMINODIAGNOSTICO;
                        if (_ActualCrim == null)
                        {
                            var _Crimin = new PFC_V_CRIMINODIAGNOSTICO()
                            {
                                ESTUDIO_FEC = Entity.ESTUDIO_FEC,
                                ID_ANIO = _EstudioPadre.ID_ANIO,
                                ID_IMPUTADO = _EstudioPadre.ID_IMPUTADO,
                                ID_CENTRO = _EstudioPadre.ID_CENTRO,
                                ID_ESTUDIO = _EstudioPadre.ID_ESTUDIO,
                                ID_INGRESO = _EstudioPadre.ID_INGRESO,
                                P1_ALCOHOL = Entity.P1_ALCOHOL,
                                P1_DROGADO = string.IsNullOrEmpty(Entity.P1_DROGADO) ? Entity.P1_DROGADO : string.Empty,
                                P1_DROGRA_ILEGAL = Entity.P1_DROGRA_ILEGAL,
                                P1_OTRA = Entity.P1_OTRA,
                                P1_VERSION_DELITO = Entity.P1_VERSION_DELITO,
                                P10_DICTAMEN_REINSERCION = Entity.P10_DICTAMEN_REINSERCION.HasValue ? Entity.P10_DICTAMEN_REINSERCION.Value != decimal.Zero ? Entity.P10_DICTAMEN_REINSERCION : null : null,
                                P10_MOTIVACION_DICTAMEN = Entity.P10_MOTIVACION_DICTAMEN,
                                P11_PROGRAMAS_REMITIRSE = Entity.P11_PROGRAMAS_REMITIRSE,
                                P12_CUAL = Entity.P12_CUAL,
                                P12_TRATAMIENTO_EXTRAMUROS = string.IsNullOrEmpty(Entity.P12_TRATAMIENTO_EXTRAMUROS) ? Entity.P12_TRATAMIENTO_EXTRAMUROS : string.Empty,
                                P2_CRIMINOGENESIS = Entity.P2_CRIMINOGENESIS,
                                P3_CONDUCTA_ANTISOCIAL = Entity.P3_CONDUCTA_ANTISOCIAL,
                                P4_CLASIFICACION_CRIMINOLOGICA = Entity.P4_CLASIFICACION_CRIMINOLOGICA.HasValue ? Entity.P4_CLASIFICACION_CRIMINOLOGICA.Value != -1 ? Entity.P4_CLASIFICACION_CRIMINOLOGICA : null : null,
                                P5_INTIMIDACION = string.IsNullOrEmpty(Entity.P5_INTIMIDACION) ? Entity.P5_INTIMIDACION : string.Empty,
                                P5_PORQUE = Entity.P5_PORQUE,
                                P6_CAPACIDAD_CRIMINAL = Entity.P6_CAPACIDAD_CRIMINAL.HasValue ? Entity.P6_CAPACIDAD_CRIMINAL.Value != -1 ? Entity.P6_CAPACIDAD_CRIMINAL : null : null,
                                P6A_EGOCENTRICO = Entity.P6A_EGOCENTRICO.HasValue ? Entity.P6A_EGOCENTRICO.Value != -1 ? Entity.P6A_EGOCENTRICO : null : null,
                                P6B_LIABILIDAD_EFECTIVA = Entity.P6B_LIABILIDAD_EFECTIVA.HasValue ? Entity.P6B_LIABILIDAD_EFECTIVA.Value != -1 ? Entity.P6B_LIABILIDAD_EFECTIVA : null : null,
                                P6C_AGRESIVIDAD = Entity.P6C_AGRESIVIDAD.HasValue ? Entity.P6C_AGRESIVIDAD.Value != -1 ? Entity.P6C_AGRESIVIDAD : null : null,
                                P6D_INDIFERENCIA_AFECTIVA = Entity.P6D_INDIFERENCIA_AFECTIVA.HasValue ? Entity.P6D_INDIFERENCIA_AFECTIVA.Value != -1 ? Entity.P6D_INDIFERENCIA_AFECTIVA : null : null,
                                P7_ADAPTACION_SOCIAL = Entity.P7_ADAPTACION_SOCIAL.HasValue ? Entity.P7_ADAPTACION_SOCIAL.Value != -1 ? Entity.P7_ADAPTACION_SOCIAL : null : null,
                                P8_INDICE_PELIGROSIDAD = Entity.P8_INDICE_PELIGROSIDAD.HasValue ? Entity.P8_INDICE_PELIGROSIDAD.Value != -1 ? Entity.P8_INDICE_PELIGROSIDAD : null : null,
                                P9_PRONOSTICO_REINCIDENCIA = Entity.P9_PRONOSTICO_REINCIDENCIA.HasValue ? Entity.P9_PRONOSTICO_REINCIDENCIA.Value != -1 ? Entity.P9_PRONOSTICO_REINCIDENCIA : null : null,
                                COORDINADOR = Entity.COORDINADOR,
                                ELABORO = Entity.ELABORO
                            };

                            Context.PFC_V_CRIMINODIAGNOSTICO.Add(_Crimin);
                        }
                        else
                        {
                            _ActualCrim.COORDINADOR = Entity.COORDINADOR;
                            _ActualCrim.ELABORO = Entity.ELABORO;
                            _ActualCrim.ESTUDIO_FEC = Entity.ESTUDIO_FEC;
                            _ActualCrim.ID_ANIO = _EstudioPadre.ID_ANIO;
                            _ActualCrim.ID_CENTRO = _EstudioPadre.ID_CENTRO;
                            _ActualCrim.ID_ESTUDIO = _EstudioPadre.ID_ESTUDIO;
                            _ActualCrim.ID_IMPUTADO = _EstudioPadre.ID_IMPUTADO;
                            _ActualCrim.ID_INGRESO = _EstudioPadre.ID_INGRESO;
                            _ActualCrim.P1_ALCOHOL = Entity.P1_ALCOHOL;
                            _ActualCrim.P1_DROGADO = Entity.P1_DROGADO;
                            _ActualCrim.P1_DROGRA_ILEGAL = Entity.P1_DROGRA_ILEGAL;
                            _ActualCrim.P1_OTRA = Entity.P1_OTRA;
                            _ActualCrim.P1_VERSION_DELITO = Entity.P1_VERSION_DELITO;
                            _ActualCrim.P10_DICTAMEN_REINSERCION = Entity.P10_DICTAMEN_REINSERCION.HasValue ? Entity.P10_DICTAMEN_REINSERCION != decimal.Zero ? Entity.P10_DICTAMEN_REINSERCION : null : null;
                            _ActualCrim.P10_MOTIVACION_DICTAMEN = Entity.P10_MOTIVACION_DICTAMEN;
                            _ActualCrim.P11_PROGRAMAS_REMITIRSE = Entity.P11_PROGRAMAS_REMITIRSE;
                            _ActualCrim.P12_CUAL = Entity.P12_CUAL;
                            _ActualCrim.P12_TRATAMIENTO_EXTRAMUROS = Entity.P12_TRATAMIENTO_EXTRAMUROS;
                            _ActualCrim.P2_CRIMINOGENESIS = Entity.P2_CRIMINOGENESIS;
                            _ActualCrim.P3_CONDUCTA_ANTISOCIAL = Entity.P3_CONDUCTA_ANTISOCIAL;
                            _ActualCrim.P4_CLASIFICACION_CRIMINOLOGICA = Entity.P4_CLASIFICACION_CRIMINOLOGICA.HasValue ? Entity.P4_CLASIFICACION_CRIMINOLOGICA != -1 ? Entity.P4_CLASIFICACION_CRIMINOLOGICA : null : null;
                            _ActualCrim.P5_INTIMIDACION = Entity.P5_INTIMIDACION;
                            _ActualCrim.P5_PORQUE = Entity.P5_PORQUE;
                            _ActualCrim.P6_CAPACIDAD_CRIMINAL = Entity.P6_CAPACIDAD_CRIMINAL.HasValue ? Entity.P6_CAPACIDAD_CRIMINAL != -1 ? Entity.P6_CAPACIDAD_CRIMINAL : null : null;
                            _ActualCrim.P6A_EGOCENTRICO = Entity.P6A_EGOCENTRICO.HasValue ? Entity.P6A_EGOCENTRICO != -1 ? Entity.P6A_EGOCENTRICO : null : null;
                            _ActualCrim.P6B_LIABILIDAD_EFECTIVA = Entity.P6B_LIABILIDAD_EFECTIVA.HasValue ? Entity.P6B_LIABILIDAD_EFECTIVA != -1 ? Entity.P6B_LIABILIDAD_EFECTIVA : null : null;
                            _ActualCrim.P6C_AGRESIVIDAD = Entity.P6C_AGRESIVIDAD.HasValue ? Entity.P6C_AGRESIVIDAD != -1 ? Entity.P6C_AGRESIVIDAD : null : null;
                            _ActualCrim.P6D_INDIFERENCIA_AFECTIVA = Entity.P6D_INDIFERENCIA_AFECTIVA.HasValue ? Entity.P6D_INDIFERENCIA_AFECTIVA != -1 ? Entity.P6D_INDIFERENCIA_AFECTIVA : null : null;
                            _ActualCrim.P7_ADAPTACION_SOCIAL = Entity.P7_ADAPTACION_SOCIAL.HasValue ? Entity.P7_ADAPTACION_SOCIAL != -1 ? Entity.P7_ADAPTACION_SOCIAL : null : null;
                            _ActualCrim.P8_INDICE_PELIGROSIDAD = Entity.P8_INDICE_PELIGROSIDAD.HasValue ? Entity.P8_INDICE_PELIGROSIDAD != -1 ? Entity.P8_INDICE_PELIGROSIDAD : null : null;
                            _ActualCrim.P9_PRONOSTICO_REINCIDENCIA = Entity.P9_PRONOSTICO_REINCIDENCIA.HasValue ? Entity.P9_PRONOSTICO_REINCIDENCIA != -1 ? Entity.P9_PRONOSTICO_REINCIDENCIA : null : null;
                            Context.Entry(_ActualCrim).State = System.Data.EntityState.Modified;
                        };
                    }
                    #region Detalle Crimin

                    if (_EstudioPadre != null)
                    {
                        short? _Estatus = new short?();
                        var _DetallePsic = Context.PERSONALIDAD.FirstOrDefault(x => x.ID_ESTUDIO == _EstudioPadre.ID_ESTUDIO && x.ID_ANIO == _EstudioPadre.ID_ANIO && x.ID_CENTRO == _EstudioPadre.ID_CENTRO && x.ID_IMPUTADO == _EstudioPadre.ID_IMPUTADO && x.ID_INGRESO == _EstudioPadre.ID_INGRESO);
                        if (string.IsNullOrEmpty(Entity.COORDINADOR) || string.IsNullOrEmpty(Entity.ELABORO) || string.IsNullOrEmpty(Entity.P1_VERSION_DELITO) || string.IsNullOrEmpty(Entity.P10_MOTIVACION_DICTAMEN) || string.IsNullOrEmpty(Entity.P2_CRIMINOGENESIS) || string.IsNullOrEmpty(Entity.P3_CONDUCTA_ANTISOCIAL) || string.IsNullOrEmpty(Entity.P5_INTIMIDACION) || string.IsNullOrEmpty(Entity.P5_PORQUE) ||
                            Entity.ESTUDIO_FEC == null || Entity.P10_DICTAMEN_REINSERCION == null || Entity.P4_CLASIFICACION_CRIMINOLOGICA == null || Entity.P5_INTIMIDACION == null || Entity.P6_CAPACIDAD_CRIMINAL == null || Entity.P6A_EGOCENTRICO == null || Entity.P6B_LIABILIDAD_EFECTIVA == null
                             || Entity.P6C_AGRESIVIDAD == null || Entity.P6D_INDIFERENCIA_AFECTIVA == null || Entity.P7_ADAPTACION_SOCIAL == null || Entity.P8_INDICE_PELIGROSIDAD == null || Entity.P9_PRONOSTICO_REINCIDENCIA == null)
                            _Estatus = (short)eEstatudDetallePersonalidad.ASIGNADO;
                        else
                            _Estatus = (short)eEstatudDetallePersonalidad.TERMINADO;

                        if (_DetallePsic != null)
                        {
                            var _DesarrolloEstudioCrimi = Context.PERSONALIDAD_DETALLE.FirstOrDefault(c => c.ID_ESTUDIO == _DetallePsic.ID_ESTUDIO && c.ID_TIPO == (short)eTiposEstudio.CRIMINOLOGICO && c.ID_IMPUTADO == _DetallePsic.ID_IMPUTADO && c.ID_INGRESO == _DetallePsic.ID_INGRESO && c.ID_CENTRO == _DetallePsic.ID_CENTRO && c.ID_ANIO == _DetallePsic.ID_ANIO);
                            if (_DesarrolloEstudioCrimi == null)
                            {
                                short _ConsecutivoPersonalidadDetalle = GetIDProceso<short>("PERSONALIDAD_DETALLE", "ID_DETALLE", "1=1");
                                var _NvoDetalle = new PERSONALIDAD_DETALLE()
                                {
                                    DIAS_BONIFICADOS = null,
                                    ID_ANIO = _EstudioPadre.ID_ANIO,
                                    ID_CENTRO = _EstudioPadre.ID_CENTRO,
                                    ID_DETALLE = _ConsecutivoPersonalidadDetalle,
                                    ID_ESTUDIO = _EstudioPadre.ID_ESTUDIO,
                                    ID_IMPUTADO = _EstudioPadre.ID_IMPUTADO,
                                    ID_INGRESO = _EstudioPadre.ID_INGRESO,
                                    ID_TIPO = (short)eTiposEstudio.CRIMINOLOGICO,
                                    INICIO_FEC = GetFechaServerDate(),
                                    RESULTADO = Entity.P10_DICTAMEN_REINSERCION.HasValue ? Entity.P10_DICTAMEN_REINSERCION == (short)eResultado.FAVORABLE ? "S" : Entity.P10_DICTAMEN_REINSERCION == (short)eResultado.DESFAVORABLE ? "N" : string.Empty : string.Empty,
                                    SOLICITUD_FEC = _EstudioPadre.SOLICITUD_FEC,
                                    ID_ESTATUS = _Estatus,
                                    TERMINO_FEC = _Estatus == (short)eEstatudDetallePersonalidad.TERMINADO ? GetFechaServerDate() : new System.DateTime?(),
                                    TIPO_MEDIA = string.Empty
                                };

                                Context.PERSONALIDAD_DETALLE.Add(_NvoDetalle);
                            }

                            else
                            {
                                _DesarrolloEstudioCrimi.RESULTADO = Entity.P10_DICTAMEN_REINSERCION.HasValue ? Entity.P10_DICTAMEN_REINSERCION == (short)eResultado.FAVORABLE ? "S" : Entity.P10_DICTAMEN_REINSERCION == (short)eResultado.DESFAVORABLE ? "N" : string.Empty : string.Empty;
                                _DesarrolloEstudioCrimi.ID_ESTATUS = _Estatus;
                                Context.Entry(_DesarrolloEstudioCrimi).State = System.Data.EntityState.Modified;
                            }
                        }
                    };

                    #endregion



                    Context.SaveChanges();
                    transaccion.Complete();
                    return 1;
                }
            }
            catch (System.Data.Entity.Validation.DbEntityValidationException dbEx)
            {
                foreach (var validationErrors in dbEx.EntityValidationErrors)
                    foreach (var validationError in validationErrors.ValidationErrors)
                        System.Diagnostics.Trace.TraceInformation("Nombre del causante de excepción: {0} Error: {1}", validationError.PropertyName, validationError.ErrorMessage);

                return 2;
            }

            catch (System.Exception exc)
            {
                return 0;
            }
        }

        public short GuardaSocioiFamiliarComun(PFC_VI_SOCIO_FAMILIAR Entity)
        {
            try
            {
                using (System.Transactions.TransactionScope transaccion = new System.Transactions.TransactionScope(System.Transactions.TransactionScopeOption.RequiresNew, new System.Transactions.TransactionOptions() { IsolationLevel = System.Transactions.IsolationLevel.ReadCommitted }))
                {
                    var _EstudioPadre = Context.PERSONALIDAD.Where(x => x.ID_INGRESO == Entity.ID_INGRESO && x.ID_IMPUTADO == Entity.ID_IMPUTADO && x.ID_CENTRO == Entity.ID_CENTRO && x.ID_ANIO == Entity.ID_ANIO).OrderByDescending(x => x.ID_ESTUDIO).FirstOrDefault();

                    if (_EstudioPadre == null)
                        return 0;

                    var _Actual = Context.PERSONALIDAD_FUERO_COMUN.FirstOrDefault(x => x.ID_ANIO == _EstudioPadre.ID_ANIO && x.ID_CENTRO == _EstudioPadre.ID_CENTRO && x.ID_IMPUTADO == _EstudioPadre.ID_IMPUTADO && x.ID_INGRESO == _EstudioPadre.ID_INGRESO && x.ID_ESTUDIO == _EstudioPadre.ID_ESTUDIO);

                    if (_Actual == null)
                    {
                        var personalidadComun = new PERSONALIDAD_FUERO_COMUN()
                        {
                            ID_ANIO = _EstudioPadre.ID_ANIO,
                            ID_IMPUTADO = _EstudioPadre.ID_IMPUTADO,
                            ID_CENTRO = _EstudioPadre.ID_CENTRO,
                            ID_ESTUDIO = _EstudioPadre.ID_ESTUDIO,
                            ID_INGRESO = _EstudioPadre.ID_INGRESO
                        };

                        var _SocioFamiliar = Context.PFC_VI_SOCIO_FAMILIAR.FirstOrDefault(x => x.ID_IMPUTADO == _EstudioPadre.ID_IMPUTADO && x.ID_INGRESO == _EstudioPadre.ID_INGRESO && x.ID_ESTUDIO == _EstudioPadre.ID_ESTUDIO && x.ID_CENTRO == _EstudioPadre.ID_CENTRO);
                        var _PadronVisitas = Context.PFC_VI_COMUNICACION.Where(x => x.ID_IMPUTADO == _EstudioPadre.ID_IMPUTADO && x.ID_INGRESO == _EstudioPadre.ID_INGRESO && x.ID_ESTUDIO == _EstudioPadre.ID_ESTUDIO && x.ID_CENTRO == _EstudioPadre.ID_CENTRO);
                        var _GruposSocioFamiliar = Context.PFC_VI_GRUPO.Where(x => x.ID_IMPUTADO == _EstudioPadre.ID_IMPUTADO && x.ID_INGRESO == _EstudioPadre.ID_INGRESO && x.ID_ESTUDIO == _EstudioPadre.ID_ESTUDIO && x.ID_CENTRO == _EstudioPadre.ID_CENTRO);

                        var _SocioComun = new PFC_VI_SOCIO_FAMILIAR()
                                    {
                                        COORDINADOR = Entity.COORDINADOR,
                                        ELABORO = Entity.ELABORO,
                                        ESTUDIO_FEC = Entity.ESTUDIO_FEC,
                                        ID_ANIO = _EstudioPadre.ID_ANIO,
                                        ID_CENTRO = _EstudioPadre.ID_CENTRO,
                                        ID_ESTUDIO = _EstudioPadre.ID_ESTUDIO,
                                        ID_IMPUTADO = _EstudioPadre.ID_IMPUTADO,
                                        ID_INGRESO = _EstudioPadre.ID_INGRESO,
                                        P10_DICTAMEN = Entity.P10_DICTAMEN.HasValue ? Entity.P10_DICTAMEN != -1 ? Entity.P10_DICTAMEN != decimal.Zero ? Entity.P10_DICTAMEN : null : null : null,
                                        P11_MOTIVACION_DICTAMEN = Entity.P11_MOTIVACION_DICTAMEN,
                                        P21_FAMILIA_PRIMARIA = Entity.P21_FAMILIA_PRIMARIA,
                                        P22_FAMILIA_SECUNDARIA = Entity.P22_FAMILIA_SECUNDARIA,
                                        P3_TERCERA_EDAD = Entity.P3_TERCERA_EDAD,
                                        P4_ESPOSOA = Entity.P4_ESPOSOA,
                                        P4_FRECUENCIA = Entity.P4_FRECUENCIA,
                                        P4_HERMANOS = Entity.P4_HERMANOS,
                                        P4_HIJOS = Entity.P4_HIJOS,
                                        P4_MADRE = Entity.P4_MADRE,
                                        P4_MOTIVO_NO_VISITA = Entity.P4_MOTIVO_NO_VISITA,
                                        P4_OTROS = Entity.P4_OTROS,
                                        P4_OTROS_EPECIFICAR = Entity.P4_OTROS_EPECIFICAR,
                                        P4_PADRE = Entity.P4_PADRE,
                                        P4_RECIBE_VISITA = Entity.P4_RECIBE_VISITA,
                                        P5_COMUNICACION_TELEFONICA = Entity.P5_COMUNICACION_TELEFONICA,
                                        P5_NO_POR_QUE = Entity.P5_NO_POR_QUE,
                                        P6_APOYO_EXTERIOR = Entity.P6_APOYO_EXTERIOR,
                                        P7_PLANES_INTERNO = Entity.P7_PLANES_INTERNO,
                                        P7_VIVIRA = Entity.P7_VIVIRA,
                                        P8_OFERTA_ESPECIFICAR = Entity.P8_OFERTA_ESPECIFICAR,
                                        P8_OFERTA_TRABAJO = Entity.P8_OFERTA_TRABAJO,
                                        P9_AVAL_ESPECIFICAR = Entity.P9_AVAL_ESPECIFICAR,
                                        P9_AVAL_MORAL = Entity.P9_AVAL_MORAL
                                    };

                        #region Listas Edicion SocioFamiliar Edicion comun
                        if (_PadronVisitas != null && _PadronVisitas.Any())
                            foreach (var item in _PadronVisitas)
                                Context.Entry(item).State = System.Data.EntityState.Deleted;

                        var _consecutivoFamiliares = GetIDProceso<short>("PFC_VI_COMUNICACION", "ID_CONSEC", string.Format("ID_CENTRO = {0} AND ID_ANIO = {1} AND ID_IMPUTADO = {2} AND ID_INGRESO = {3} AND ID_ESTUDIO = {4}", Entity.ID_CENTRO, Entity.ID_ANIO, Entity.ID_IMPUTADO, Entity.ID_INGRESO, Entity.ID_ESTUDIO));
                        if (Entity.PFC_VI_COMUNICACION != null && Entity.PFC_VI_COMUNICACION.Any())
                        {
                            foreach (var item in Entity.PFC_VI_COMUNICACION)
                            {
                                var _NuevaVisita = new PFC_VI_COMUNICACION()
                                {
                                    FRECUENCIA = item.FRECUENCIA,
                                    ID_ANIO = _EstudioPadre.ID_ANIO,
                                    ID_IMPUTADO = _EstudioPadre.ID_IMPUTADO,
                                    ID_CENTRO = _EstudioPadre.ID_CENTRO,
                                    ID_ESTUDIO = _EstudioPadre.ID_ESTUDIO,
                                    ID_CONSEC = _consecutivoFamiliares,
                                    ID_INGRESO = _EstudioPadre.ID_INGRESO,
                                    ID_TIPO_REFERENCIA = item.ID_TIPO_REFERENCIA,
                                    NOMBRE = item.NOMBRE,
                                    TELEFONO = item.TELEFONO
                                };

                                Context.PFC_VI_COMUNICACION.Add(_NuevaVisita);
                                _consecutivoFamiliares++;
                            };
                        };

                        if (_GruposSocioFamiliar != null && _GruposSocioFamiliar.Any())
                            foreach (var item in _GruposSocioFamiliar)
                                Context.Entry(item).State = System.Data.EntityState.Deleted;

                        if (Entity.PFC_VI_GRUPO != null && Entity.PFC_VI_GRUPO.Any())
                            foreach (var item in Entity.PFC_VI_GRUPO)
                            {
                                var _NuevoGrupoSocioFamiliar = new PFC_VI_GRUPO()
                                {
                                    CONGREGACION = item.CONGREGACION,
                                    ID_ACTIVIDAD = item.ID_ACTIVIDAD,
                                    ID_ANIO = _EstudioPadre.ID_ANIO,
                                    ID_CENTRO = _EstudioPadre.ID_CENTRO,
                                    ID_ESTUDIO = _EstudioPadre.ID_ESTUDIO,
                                    ID_IMPUTADO = _EstudioPadre.ID_IMPUTADO,
                                    ID_INGRESO = _EstudioPadre.ID_INGRESO,
                                    ID_TIPO_PROGRAMA = item.ID_TIPO_PROGRAMA,
                                    OBSERVACIONES = item.OBSERVACIONES,
                                    PERIODO = item.PERIODO
                                };

                                Context.PFC_VI_GRUPO.Add(_NuevoGrupoSocioFamiliar);
                            };

                        #endregion
                        Context.PERSONALIDAD_FUERO_COMUN.Add(personalidadComun);
                        Context.PFC_VI_SOCIO_FAMILIAR.Add(_SocioComun);
                    }
                    else
                    {
                        var _SocCom = _Actual.PFC_VI_SOCIO_FAMILIAR;
                        if (_SocCom == null)
                        {
                            var _SocioFamiliar = Context.PFC_VI_SOCIO_FAMILIAR.FirstOrDefault(x => x.ID_IMPUTADO == _EstudioPadre.ID_IMPUTADO && x.ID_INGRESO == _EstudioPadre.ID_INGRESO && x.ID_ESTUDIO == _EstudioPadre.ID_ESTUDIO && x.ID_CENTRO == _EstudioPadre.ID_CENTRO);
                            var _PadronVisitas = Context.PFC_VI_COMUNICACION.Where(x => x.ID_IMPUTADO == _EstudioPadre.ID_IMPUTADO && x.ID_INGRESO == _EstudioPadre.ID_INGRESO && x.ID_ESTUDIO == _EstudioPadre.ID_ESTUDIO && x.ID_CENTRO == _EstudioPadre.ID_CENTRO);
                            var _GruposSocioFamiliar = Context.PFC_VI_GRUPO.Where(x => x.ID_IMPUTADO == _EstudioPadre.ID_IMPUTADO && x.ID_INGRESO == _EstudioPadre.ID_INGRESO && x.ID_ESTUDIO == _EstudioPadre.ID_ESTUDIO && x.ID_CENTRO == _EstudioPadre.ID_CENTRO);

                            var _SocioComun = new PFC_VI_SOCIO_FAMILIAR()
                            {
                                COORDINADOR = Entity.COORDINADOR,
                                ELABORO = Entity.ELABORO,
                                ESTUDIO_FEC = Entity.ESTUDIO_FEC,
                                ID_ANIO = _EstudioPadre.ID_ANIO,
                                ID_CENTRO = _EstudioPadre.ID_CENTRO,
                                ID_ESTUDIO = _EstudioPadre.ID_ESTUDIO,
                                ID_IMPUTADO = _EstudioPadre.ID_IMPUTADO,
                                ID_INGRESO = _EstudioPadre.ID_INGRESO,
                                P10_DICTAMEN = Entity.P10_DICTAMEN.HasValue ? Entity.P10_DICTAMEN != -1 ? Entity.P10_DICTAMEN != decimal.Zero ? Entity.P10_DICTAMEN : null : null : null,
                                P11_MOTIVACION_DICTAMEN = Entity.P11_MOTIVACION_DICTAMEN,
                                P21_FAMILIA_PRIMARIA = Entity.P21_FAMILIA_PRIMARIA,
                                P22_FAMILIA_SECUNDARIA = Entity.P22_FAMILIA_SECUNDARIA,
                                P3_TERCERA_EDAD = Entity.P3_TERCERA_EDAD,
                                P4_ESPOSOA = Entity.P4_ESPOSOA,
                                P4_FRECUENCIA = Entity.P4_FRECUENCIA,
                                P4_HERMANOS = Entity.P4_HERMANOS,
                                P4_HIJOS = Entity.P4_HIJOS,
                                P4_MADRE = Entity.P4_MADRE,
                                P4_MOTIVO_NO_VISITA = Entity.P4_MOTIVO_NO_VISITA,
                                P4_OTROS = Entity.P4_OTROS,
                                P4_OTROS_EPECIFICAR = Entity.P4_OTROS_EPECIFICAR,
                                P4_PADRE = Entity.P4_PADRE,
                                P4_RECIBE_VISITA = Entity.P4_RECIBE_VISITA,
                                P5_COMUNICACION_TELEFONICA = Entity.P5_COMUNICACION_TELEFONICA,
                                P5_NO_POR_QUE = Entity.P5_NO_POR_QUE,
                                P6_APOYO_EXTERIOR = Entity.P6_APOYO_EXTERIOR,
                                P7_PLANES_INTERNO = Entity.P7_PLANES_INTERNO,
                                P7_VIVIRA = Entity.P7_VIVIRA,
                                P8_OFERTA_ESPECIFICAR = Entity.P8_OFERTA_ESPECIFICAR,
                                P8_OFERTA_TRABAJO = Entity.P8_OFERTA_TRABAJO,
                                P9_AVAL_ESPECIFICAR = Entity.P9_AVAL_ESPECIFICAR,
                                P9_AVAL_MORAL = Entity.P9_AVAL_MORAL
                            };

                            #region Listas Edicion SocioFamiliar Edicion comun
                            if (_PadronVisitas != null && _PadronVisitas.Any())
                                foreach (var item in _PadronVisitas)
                                    Context.Entry(item).State = System.Data.EntityState.Deleted;

                            var _consecutivoFamiliares = GetIDProceso<short>("PFC_VI_COMUNICACION", "ID_CONSEC", string.Format("ID_CENTRO = {0} AND ID_ANIO = {1} AND ID_IMPUTADO = {2} AND ID_INGRESO = {3} AND ID_ESTUDIO = {4}", Entity.ID_CENTRO, Entity.ID_ANIO, Entity.ID_IMPUTADO, Entity.ID_INGRESO, Entity.ID_ESTUDIO));
                            if (Entity.PFC_VI_COMUNICACION != null && Entity.PFC_VI_COMUNICACION.Any())
                            {
                                foreach (var item in Entity.PFC_VI_COMUNICACION)
                                {
                                    var _NuevaVisita = new PFC_VI_COMUNICACION()
                                    {
                                        FRECUENCIA = item.FRECUENCIA,
                                        ID_ANIO = _EstudioPadre.ID_ANIO,
                                        ID_CENTRO = _EstudioPadre.ID_CENTRO,
                                        ID_CONSEC = _consecutivoFamiliares,
                                        ID_ESTUDIO = _EstudioPadre.ID_ESTUDIO,
                                        ID_IMPUTADO = _EstudioPadre.ID_IMPUTADO,
                                        ID_INGRESO = _EstudioPadre.ID_INGRESO,
                                        ID_TIPO_REFERENCIA = item.ID_TIPO_REFERENCIA,
                                        NOMBRE = item.NOMBRE,
                                        TELEFONO = item.TELEFONO
                                    };

                                    Context.PFC_VI_COMUNICACION.Add(_NuevaVisita);
                                    _consecutivoFamiliares++;
                                };
                            };

                            if (_GruposSocioFamiliar != null && _GruposSocioFamiliar.Any())
                                foreach (var item in _GruposSocioFamiliar)
                                    Context.Entry(item).State = System.Data.EntityState.Deleted;

                            if (Entity.PFC_VI_GRUPO != null && Entity.PFC_VI_GRUPO.Any())
                                foreach (var item in Entity.PFC_VI_GRUPO)
                                {
                                    var _NuevoGrupoSocioFamiliar = new PFC_VI_GRUPO()
                                    {
                                        CONGREGACION = item.CONGREGACION,
                                        ID_ACTIVIDAD = item.ID_ACTIVIDAD,
                                        ID_ANIO = _EstudioPadre.ID_ANIO,
                                        ID_CENTRO = _EstudioPadre.ID_CENTRO,
                                        ID_ESTUDIO = _EstudioPadre.ID_ESTUDIO,
                                        ID_IMPUTADO = _EstudioPadre.ID_IMPUTADO,
                                        ID_INGRESO = _EstudioPadre.ID_INGRESO,
                                        ID_TIPO_PROGRAMA = item.ID_TIPO_PROGRAMA,
                                        OBSERVACIONES = item.OBSERVACIONES,
                                        PERIODO = item.PERIODO
                                    };

                                    Context.PFC_VI_GRUPO.Add(_NuevoGrupoSocioFamiliar);
                                };

                            #endregion
                            Context.PFC_VI_SOCIO_FAMILIAR.Add(_SocioComun);
                        }
                        else
                        {
                            var _PadronVisitas = Context.PFC_VI_COMUNICACION.Where(x => x.ID_IMPUTADO == _EstudioPadre.ID_IMPUTADO && x.ID_INGRESO == _EstudioPadre.ID_INGRESO && x.ID_ESTUDIO == _EstudioPadre.ID_ESTUDIO && x.ID_CENTRO == _EstudioPadre.ID_CENTRO);
                            var _GruposSocioFamiliar = Context.PFC_VI_GRUPO.Where(x => x.ID_IMPUTADO == _EstudioPadre.ID_IMPUTADO && x.ID_INGRESO == _EstudioPadre.ID_INGRESO && x.ID_ESTUDIO == _EstudioPadre.ID_ESTUDIO && x.ID_CENTRO == _EstudioPadre.ID_CENTRO);

                            _SocCom.COORDINADOR = Entity.COORDINADOR;
                            _SocCom.ELABORO = Entity.ELABORO;
                            _SocCom.ESTUDIO_FEC = Entity.ESTUDIO_FEC;
                            _SocCom.ID_ANIO = _EstudioPadre.ID_ANIO;
                            _SocCom.ID_CENTRO = _EstudioPadre.ID_CENTRO;
                            _SocCom.ID_ESTUDIO = _EstudioPadre.ID_ESTUDIO;
                            _SocCom.ID_IMPUTADO = _EstudioPadre.ID_IMPUTADO;
                            _SocCom.ID_INGRESO = _EstudioPadre.ID_INGRESO;
                            _SocCom.P10_DICTAMEN = Entity.P10_DICTAMEN.HasValue ? Entity.P10_DICTAMEN != -1 ? Entity.P10_DICTAMEN != decimal.Zero ? Entity.P10_DICTAMEN : null : null : null;
                            _SocCom.P11_MOTIVACION_DICTAMEN = Entity.P11_MOTIVACION_DICTAMEN;
                            _SocCom.P21_FAMILIA_PRIMARIA = Entity.P21_FAMILIA_PRIMARIA;
                            _SocCom.P22_FAMILIA_SECUNDARIA = Entity.P22_FAMILIA_SECUNDARIA;
                            _SocCom.P3_TERCERA_EDAD = Entity.P3_TERCERA_EDAD;
                            _SocCom.P4_ESPOSOA = Entity.P4_ESPOSOA;
                            _SocCom.P4_FRECUENCIA = Entity.P4_FRECUENCIA;
                            _SocCom.P4_HERMANOS = Entity.P4_HERMANOS;
                            _SocCom.P4_HIJOS = Entity.P4_HIJOS;
                            _SocCom.P4_MADRE = Entity.P4_MADRE;
                            _SocCom.P4_MOTIVO_NO_VISITA = Entity.P4_MOTIVO_NO_VISITA;
                            _SocCom.P4_OTROS = Entity.P4_OTROS;
                            _SocCom.P4_OTROS_EPECIFICAR = Entity.P4_OTROS_EPECIFICAR;
                            _SocCom.P4_PADRE = Entity.P4_PADRE;
                            _SocCom.P4_RECIBE_VISITA = Entity.P4_RECIBE_VISITA;
                            _SocCom.P5_COMUNICACION_TELEFONICA = Entity.P5_COMUNICACION_TELEFONICA;
                            _SocCom.P5_NO_POR_QUE = Entity.P5_NO_POR_QUE;
                            _SocCom.P6_APOYO_EXTERIOR = Entity.P6_APOYO_EXTERIOR;
                            _SocCom.P7_PLANES_INTERNO = Entity.P7_PLANES_INTERNO;
                            _SocCom.P7_VIVIRA = Entity.P7_VIVIRA;
                            _SocCom.P8_OFERTA_ESPECIFICAR = Entity.P8_OFERTA_ESPECIFICAR;
                            _SocCom.P8_OFERTA_TRABAJO = Entity.P8_OFERTA_TRABAJO;
                            _SocCom.P9_AVAL_ESPECIFICAR = Entity.P9_AVAL_ESPECIFICAR;
                            _SocCom.P9_AVAL_MORAL = Entity.P9_AVAL_MORAL;
                            Context.Entry(_SocCom).State = System.Data.EntityState.Modified;

                            #region Visitas y Grupos Edicion Socio Familiar Comun
                            if (_PadronVisitas != null && _PadronVisitas.Any())
                                foreach (var item in _PadronVisitas)
                                    Context.Entry(item).State = System.Data.EntityState.Deleted;

                            if (_GruposSocioFamiliar != null && _GruposSocioFamiliar.Any())
                                foreach (var item in _GruposSocioFamiliar)
                                    Context.Entry(item).State = System.Data.EntityState.Deleted;

                            var _consecutivoFamiliares = GetIDProceso<short>("PFC_VI_COMUNICACION", "ID_CONSEC", string.Format("ID_CENTRO = {0} AND ID_ANIO = {1} AND ID_IMPUTADO = {2} AND ID_INGRESO = {3} AND ID_ESTUDIO = {4}", Entity.ID_CENTRO, Entity.ID_ANIO, Entity.ID_IMPUTADO, Entity.ID_INGRESO, Entity.ID_ESTUDIO));

                            if (Entity.PFC_VI_COMUNICACION != null && Entity.PFC_VI_COMUNICACION.Any())
                            {
                                foreach (var item in Entity.PFC_VI_COMUNICACION)
                                {
                                    var _NuevaVisita = new PFC_VI_COMUNICACION()
                                    {
                                        FRECUENCIA = item.FRECUENCIA,
                                        ID_ANIO = _EstudioPadre.ID_ANIO,
                                        ID_CENTRO = _EstudioPadre.ID_CENTRO,
                                        ID_CONSEC = _consecutivoFamiliares,
                                        ID_ESTUDIO = _EstudioPadre.ID_ESTUDIO,
                                        ID_IMPUTADO = _EstudioPadre.ID_IMPUTADO,
                                        ID_INGRESO = _EstudioPadre.ID_INGRESO,
                                        ID_TIPO_REFERENCIA = item.ID_TIPO_REFERENCIA,
                                        NOMBRE = item.NOMBRE,
                                        TELEFONO = item.TELEFONO
                                    };

                                    _SocCom.PFC_VI_COMUNICACION.Add(_NuevaVisita);
                                    _consecutivoFamiliares++;
                                };
                            }

                            if (Entity.PFC_VI_GRUPO != null && Entity.PFC_VI_GRUPO.Any())
                                foreach (var item in Entity.PFC_VI_GRUPO)
                                {
                                    var _NuevoGrupoSocioFamiliar = new PFC_VI_GRUPO()
                                    {
                                        CONGREGACION = item.CONGREGACION,
                                        ID_ACTIVIDAD = item.ID_ACTIVIDAD,
                                        ID_ANIO = _EstudioPadre.ID_ANIO,
                                        ID_CENTRO = _EstudioPadre.ID_CENTRO,
                                        ID_ESTUDIO = _EstudioPadre.ID_ESTUDIO,
                                        ID_IMPUTADO = _EstudioPadre.ID_IMPUTADO,
                                        ID_INGRESO = _EstudioPadre.ID_INGRESO,
                                        ID_TIPO_PROGRAMA = item.ID_TIPO_PROGRAMA,
                                        OBSERVACIONES = item.OBSERVACIONES,
                                        PERIODO = item.PERIODO
                                    };

                                    _SocCom.PFC_VI_GRUPO.Add(_NuevoGrupoSocioFamiliar);
                                };

                            #endregion
                        }
                    }

                    #region Detalle
                    if (_EstudioPadre != null)
                    {
                        short? _Estatus = new short?();
                        var _DetallePsic = Context.PERSONALIDAD.FirstOrDefault(x => x.ID_ESTUDIO == _EstudioPadre.ID_ESTUDIO && x.ID_ANIO == _EstudioPadre.ID_ANIO && x.ID_CENTRO == _EstudioPadre.ID_CENTRO && x.ID_IMPUTADO == _EstudioPadre.ID_IMPUTADO && x.ID_INGRESO == _EstudioPadre.ID_INGRESO);
                        if (string.IsNullOrEmpty(Entity.COORDINADOR) || string.IsNullOrEmpty(Entity.ELABORO) || string.IsNullOrEmpty(Entity.P11_MOTIVACION_DICTAMEN) || string.IsNullOrEmpty(Entity.P21_FAMILIA_PRIMARIA) || string.IsNullOrEmpty(Entity.P22_FAMILIA_SECUNDARIA) || string.IsNullOrEmpty(Entity.P3_TERCERA_EDAD) || string.IsNullOrEmpty(Entity.P4_RECIBE_VISITA) || string.IsNullOrEmpty(Entity.P5_COMUNICACION_TELEFONICA)
                             || string.IsNullOrEmpty(Entity.P6_APOYO_EXTERIOR) || string.IsNullOrEmpty(Entity.P7_PLANES_INTERNO) || string.IsNullOrEmpty(Entity.P7_VIVIRA) || string.IsNullOrEmpty(Entity.P8_OFERTA_TRABAJO) || Entity.ESTUDIO_FEC == null || Entity.P10_DICTAMEN == null)
                            _Estatus = (short)eEstatudDetallePersonalidad.ASIGNADO;
                        else
                            _Estatus = (short)eEstatudDetallePersonalidad.TERMINADO;

                        if (_DetallePsic != null)
                        {
                            var _DesarrolloEstudioCrimi = Context.PERSONALIDAD_DETALLE.FirstOrDefault(c => c.ID_ESTUDIO == _DetallePsic.ID_ESTUDIO && c.ID_TIPO == (short)eTiposEstudio.TRABAJO_SOCIAL && c.ID_IMPUTADO == _DetallePsic.ID_IMPUTADO && c.ID_INGRESO == _DetallePsic.ID_INGRESO && c.ID_CENTRO == _DetallePsic.ID_CENTRO && c.ID_ANIO == _DetallePsic.ID_ANIO);
                            if (_DesarrolloEstudioCrimi == null)
                            {
                                short _ConsecutivoPersonalidadDetalle = GetIDProceso<short>("PERSONALIDAD_DETALLE", "ID_DETALLE", "1=1");
                                var _NvoDetalle = new PERSONALIDAD_DETALLE()
                                {
                                    DIAS_BONIFICADOS = null,
                                    ID_ANIO = Entity.ID_ANIO,
                                    ID_CENTRO = Entity.ID_CENTRO,
                                    ID_DETALLE = _ConsecutivoPersonalidadDetalle,
                                    ID_ESTUDIO = _EstudioPadre.ID_ESTUDIO,
                                    ID_IMPUTADO = Entity.ID_IMPUTADO,
                                    ID_INGRESO = Entity.ID_INGRESO,
                                    ID_TIPO = (short)eTiposEstudio.TRABAJO_SOCIAL,
                                    INICIO_FEC = GetFechaServerDate(),
                                    RESULTADO = Entity.P10_DICTAMEN.HasValue ? Entity.P10_DICTAMEN == (short)eResultado.FAVORABLE ? "S" : Entity.P10_DICTAMEN == (short)eResultado.DESFAVORABLE ? "N" : string.Empty : string.Empty,
                                    SOLICITUD_FEC = _EstudioPadre.SOLICITUD_FEC,
                                    ID_ESTATUS = _Estatus,
                                    TERMINO_FEC = _Estatus == (short)eEstatudDetallePersonalidad.TERMINADO ? GetFechaServerDate() : new System.DateTime?(),
                                    TIPO_MEDIA = string.Empty
                                };

                                Context.PERSONALIDAD_DETALLE.Add(_NvoDetalle);
                            }

                            else
                            {
                                _DesarrolloEstudioCrimi.RESULTADO = Entity.P10_DICTAMEN.HasValue ? Entity.P10_DICTAMEN == (short)eResultado.FAVORABLE ? "S" : Entity.P10_DICTAMEN == (short)eResultado.DESFAVORABLE ? "N" : string.Empty : string.Empty;
                                _DesarrolloEstudioCrimi.ID_ESTATUS = _Estatus;
                                Context.Entry(_DesarrolloEstudioCrimi).State = System.Data.EntityState.Modified;
                            };
                        }
                    }
                    #endregion


                    Context.SaveChanges();
                    transaccion.Complete();
                    return 1;
                }
            }

            catch (System.Data.Entity.Validation.DbEntityValidationException dbEx)
            {
                foreach (var validationErrors in dbEx.EntityValidationErrors)
                    foreach (var validationError in validationErrors.ValidationErrors)
                        System.Diagnostics.Trace.TraceInformation("Nombre del causante de excepción: {0} Error: {1}", validationError.PropertyName, validationError.ErrorMessage);

                return 2;
            }

            catch (System.Exception exc)
            {
                return 0;
            }
        }

        public short GuardarEducativoComun(PFC_VII_EDUCATIVO Entity)
        {
            try
            {
                using (System.Transactions.TransactionScope transaccion = new System.Transactions.TransactionScope(System.Transactions.TransactionScopeOption.RequiresNew, new System.Transactions.TransactionOptions() { IsolationLevel = System.Transactions.IsolationLevel.ReadCommitted }))
                {
                    var _EstudioPadre = Context.PERSONALIDAD.Where(x => x.ID_INGRESO == Entity.ID_INGRESO && x.ID_IMPUTADO == Entity.ID_IMPUTADO && x.ID_CENTRO == Entity.ID_CENTRO && x.ID_ANIO == Entity.ID_ANIO).OrderByDescending(x => x.ID_ESTUDIO).FirstOrDefault();

                    if (_EstudioPadre == null)
                        return 0;

                    var _Actual = Context.PERSONALIDAD_FUERO_COMUN.FirstOrDefault(x => x.ID_ANIO == _EstudioPadre.ID_ANIO && x.ID_CENTRO == _EstudioPadre.ID_CENTRO && x.ID_IMPUTADO == _EstudioPadre.ID_IMPUTADO && x.ID_INGRESO == _EstudioPadre.ID_INGRESO && x.ID_ESTUDIO == _EstudioPadre.ID_ESTUDIO);

                    if (_Actual == null)
                    {
                        var _EstudioEducativo = Context.PFC_VII_EDUCATIVO.FirstOrDefault(x => x.ID_IMPUTADO == _EstudioPadre.ID_IMPUTADO && x.ID_INGRESO == _EstudioPadre.ID_INGRESO && x.ID_ESTUDIO == _EstudioPadre.ID_ESTUDIO && x.ID_CENTRO == _EstudioPadre.ID_CENTRO);
                        var _Actividades = Context.PFC_VII_ACTIVIDAD.Where(c => c.ID_INGRESO == _EstudioPadre.ID_INGRESO && c.ID_IMPUTADO == _EstudioPadre.ID_IMPUTADO && c.ID_ESTUDIO == _EstudioPadre.ID_ESTUDIO && c.ID_CENTRO == _EstudioPadre.ID_CENTRO);
                        var _EscolaridadesAnteriores = Context.PFC_VII_ESCOLARIDAD_ANTERIOR.Where(x => x.ID_IMPUTADO == _EstudioPadre.ID_IMPUTADO && x.ID_INGRESO == _EstudioPadre.ID_INGRESO && x.ID_ESTUDIO == _EstudioPadre.ID_ESTUDIO && x.ID_CENTRO == _EstudioPadre.ID_CENTRO);

                        var personalidadComun = new PERSONALIDAD_FUERO_COMUN()
                        {
                            ID_ANIO = _EstudioPadre.ID_ANIO,
                            ID_IMPUTADO = _EstudioPadre.ID_IMPUTADO,
                            ID_CENTRO = _EstudioPadre.ID_CENTRO,
                            ID_ESTUDIO = _EstudioPadre.ID_ESTUDIO,
                            ID_INGRESO = _EstudioPadre.ID_INGRESO
                        };

                        var _edu = new PFC_VII_EDUCATIVO()
                        {
                            COORDINADOR = Entity.COORDINADOR,
                            ELABORO = Entity.ELABORO,
                            ESTUDIO_FEC = Entity.ESTUDIO_FEC,
                            ID_ANIO = _EstudioPadre.ID_ANIO,
                            ID_CENTRO = _EstudioPadre.ID_CENTRO,
                            ID_ESTUDIO = _EstudioPadre.ID_ESTUDIO,
                            ID_IMPUTADO = _EstudioPadre.ID_IMPUTADO,
                            ID_INGRESO = _EstudioPadre.ID_INGRESO,
                            P3_DICTAMEN = Entity.P3_DICTAMEN.HasValue ? Entity.P3_DICTAMEN.Value != decimal.Zero ? Entity.P3_DICTAMEN : null : null,
                            P4_MOTIVACION_DICTAMEN = Entity.P4_MOTIVACION_DICTAMEN
                        };

                        if (_Actividades != null && _Actividades.Any())
                            foreach (var item in _Actividades)
                                Context.Entry(item).State = System.Data.EntityState.Deleted;

                        var _consecutivoTrabajo = GetIDProceso<short>("PFC_VII_ACTIVIDAD", "ID_CONSEC", string.Format("ID_CENTRO = {0} AND ID_ANIO = {1} AND ID_IMPUTADO = {2} AND ID_INGRESO = {3} AND ID_ESTUDIO = {4}", _EstudioPadre.ID_CENTRO, _EstudioPadre.ID_ANIO, _EstudioPadre.ID_IMPUTADO, _EstudioPadre.ID_INGRESO, _EstudioPadre.ID_ESTUDIO));
                        if (Entity.PFC_VII_ACTIVIDAD != null && Entity.PFC_VII_ACTIVIDAD.Any())
                            foreach (var item in Entity.PFC_VII_ACTIVIDAD)
                            {
                                var _NuevaActividad = new PFC_VII_ACTIVIDAD()
                                {
                                    ACTIVIDAD = item.ACTIVIDAD,
                                    DURACION = item.DURACION,
                                    ID_ANIO = _EstudioPadre.ID_ANIO,
                                    ID_CENTRO = _EstudioPadre.ID_CENTRO,
                                    ID_ESTUDIO = _EstudioPadre.ID_ESTUDIO,
                                    ID_CONSEC = _consecutivoTrabajo,
                                    ID_IMPUTADO = _EstudioPadre.ID_IMPUTADO,
                                    ID_INGRESO = _EstudioPadre.ID_INGRESO,
                                    ID_PROGRAMA = null,
                                    OBSERVACION = item.OBSERVACION,
                                    TIPO = item.TIPO,
                                    ID_ACTIVIDAD = item.ID_ACTIVIDAD,
                                    ID_TIPO_PROGRAMA = item.ID_TIPO_PROGRAMA
                                };

                                Context.PFC_VII_ACTIVIDAD.Add(_NuevaActividad);
                                _consecutivoTrabajo++;
                            };

                        if (_EscolaridadesAnteriores != null && _EscolaridadesAnteriores.Any())
                            foreach (var item in _EscolaridadesAnteriores)
                                Context.Entry(item).State = System.Data.EntityState.Deleted;

                        var _consecutivoEduca = GetIDProceso<short>("PFC_VII_ESCOLARIDAD_ANTERIOR", "ID_CONSEC", string.Format("ID_CENTRO = {0} AND ID_ANIO = {1} AND ID_IMPUTADO = {2} AND ID_INGRESO = {3} AND ID_ESTUDIO = {4}", Entity.ID_CENTRO, Entity.ID_ANIO, Entity.ID_IMPUTADO, Entity.ID_INGRESO, Entity.ID_ESTUDIO));

                        if (Entity.PFC_VII_ESCOLARIDAD_ANTERIOR != null && Entity.PFC_VII_ESCOLARIDAD_ANTERIOR.Any())
                            foreach (var item in Entity.PFC_VII_ESCOLARIDAD_ANTERIOR)
                            {
                                var _NuevaEscolaridad = new PFC_VII_ESCOLARIDAD_ANTERIOR()
                                {
                                    CONCLUIDA = item.CONCLUIDA,
                                    ID_ANIO = _EstudioPadre.ID_ANIO,
                                    ID_CENTRO = _EstudioPadre.ID_CENTRO,
                                    ID_ESTUDIO = _EstudioPadre.ID_ESTUDIO,
                                    ID_GRADO = item.ID_GRADO,
                                    ID_IMPUTADO = _EstudioPadre.ID_IMPUTADO,
                                    ID_INGRESO = _EstudioPadre.ID_INGRESO,
                                    INTERES = item.INTERES,
                                    OBSERVACION = item.OBSERVACION,
                                    RENDIMIENTO = item.RENDIMIENTO,
                                    ID_CONSEC = _consecutivoEduca
                                };

                                Context.PFC_VII_ESCOLARIDAD_ANTERIOR.Add(_NuevaEscolaridad);
                                _consecutivoEduca++;
                            };

                        Context.PFC_VII_EDUCATIVO.Add(_edu);
                        Context.PERSONALIDAD_FUERO_COMUN.Add(personalidadComun);
                    }
                    else
                    {
                        var _eduActual = _Actual.PFC_VII_EDUCATIVO;
                        if (_eduActual == null)
                        {
                            var _EstudioEducativo = Context.PFC_VII_EDUCATIVO.FirstOrDefault(x => x.ID_IMPUTADO == _EstudioPadre.ID_IMPUTADO && x.ID_INGRESO == _EstudioPadre.ID_INGRESO && x.ID_ESTUDIO == _EstudioPadre.ID_ESTUDIO && x.ID_CENTRO == _EstudioPadre.ID_CENTRO);
                            var _Actividades = Context.PFC_VII_ACTIVIDAD.Where(c => c.ID_INGRESO == _EstudioPadre.ID_INGRESO && c.ID_IMPUTADO == _EstudioPadre.ID_IMPUTADO && c.ID_ESTUDIO == _EstudioPadre.ID_ESTUDIO && c.ID_CENTRO == _EstudioPadre.ID_CENTRO);
                            var _EscolaridadesAnteriores = Context.PFC_VII_ESCOLARIDAD_ANTERIOR.Where(x => x.ID_IMPUTADO == _EstudioPadre.ID_IMPUTADO && x.ID_INGRESO == _EstudioPadre.ID_INGRESO && x.ID_ESTUDIO == _EstudioPadre.ID_ESTUDIO && x.ID_CENTRO == _EstudioPadre.ID_CENTRO);

                            var _edu = new PFC_VII_EDUCATIVO()
                            {
                                COORDINADOR = Entity.COORDINADOR,
                                ELABORO = Entity.ELABORO,
                                ESTUDIO_FEC = Entity.ESTUDIO_FEC,
                                ID_ANIO = _EstudioPadre.ID_ANIO,
                                ID_CENTRO = _EstudioPadre.ID_CENTRO,
                                ID_ESTUDIO = _EstudioPadre.ID_ESTUDIO,
                                ID_IMPUTADO = _EstudioPadre.ID_IMPUTADO,
                                ID_INGRESO = _EstudioPadre.ID_INGRESO,
                                P3_DICTAMEN = Entity.P3_DICTAMEN.HasValue ? Entity.P3_DICTAMEN.Value != decimal.Zero ? Entity.P3_DICTAMEN : null : null,
                                P4_MOTIVACION_DICTAMEN = Entity.P4_MOTIVACION_DICTAMEN
                            };

                            if (_Actividades != null && _Actividades.Any())
                                foreach (var item in _Actividades)
                                    Context.Entry(item).State = System.Data.EntityState.Deleted;

                            var _consecutivoTrabajo = GetIDProceso<short>("PFC_VII_ACTIVIDAD", "ID_CONSEC", string.Format("ID_CENTRO = {0} AND ID_ANIO = {1} AND ID_IMPUTADO = {2} AND ID_INGRESO = {3} AND ID_ESTUDIO = {4}", _EstudioPadre.ID_CENTRO, _EstudioPadre.ID_ANIO, _EstudioPadre.ID_IMPUTADO, _EstudioPadre.ID_INGRESO, _EstudioPadre.ID_ESTUDIO));
                            if (Entity.PFC_VII_ACTIVIDAD != null && Entity.PFC_VII_ACTIVIDAD.Any())
                                foreach (var item in Entity.PFC_VII_ACTIVIDAD)
                                {
                                    var _NuevaActividad = new PFC_VII_ACTIVIDAD()
                                    {
                                        ACTIVIDAD = item.ACTIVIDAD,
                                        DURACION = item.DURACION,
                                        ID_ANIO = _EstudioPadre.ID_ANIO,
                                        ID_CENTRO = _EstudioPadre.ID_CENTRO,
                                        ID_ESTUDIO = _EstudioPadre.ID_ESTUDIO,
                                        ID_IMPUTADO = _EstudioPadre.ID_IMPUTADO,
                                        ID_INGRESO = _EstudioPadre.ID_INGRESO,
                                        ID_PROGRAMA = null,
                                        ID_CONSEC = _consecutivoTrabajo,
                                        OBSERVACION = item.OBSERVACION,
                                        TIPO = item.TIPO,
                                        ID_ACTIVIDAD = item.ID_ACTIVIDAD,
                                        ID_TIPO_PROGRAMA = item.ID_TIPO_PROGRAMA
                                    };

                                    Context.PFC_VII_ACTIVIDAD.Add(_NuevaActividad);
                                    _consecutivoTrabajo++;
                                };

                            if (_EscolaridadesAnteriores != null && _EscolaridadesAnteriores.Any())
                                foreach (var item in _EscolaridadesAnteriores)
                                    Context.Entry(item).State = System.Data.EntityState.Deleted;

                            var _consecutivoEduca = GetIDProceso<short>("PFC_VII_ESCOLARIDAD_ANTERIOR", "ID_CONSEC", string.Format("ID_CENTRO = {0} AND ID_ANIO = {1} AND ID_IMPUTADO = {2} AND ID_INGRESO = {3} AND ID_ESTUDIO = {4}", Entity.ID_CENTRO, Entity.ID_ANIO, Entity.ID_IMPUTADO, Entity.ID_INGRESO, Entity.ID_ESTUDIO));

                            if (Entity.PFC_VII_ESCOLARIDAD_ANTERIOR != null && Entity.PFC_VII_ESCOLARIDAD_ANTERIOR.Any())
                                foreach (var item in Entity.PFC_VII_ESCOLARIDAD_ANTERIOR)
                                {
                                    var _NuevaEscolaridad = new PFC_VII_ESCOLARIDAD_ANTERIOR()
                                    {
                                        CONCLUIDA = item.CONCLUIDA,
                                        ID_ANIO = _EstudioPadre.ID_ANIO,
                                        ID_CENTRO = _EstudioPadre.ID_CENTRO,
                                        ID_ESTUDIO = _EstudioPadre.ID_ESTUDIO,
                                        ID_GRADO = item.ID_GRADO,
                                        ID_IMPUTADO = _EstudioPadre.ID_IMPUTADO,
                                        ID_INGRESO = _EstudioPadre.ID_INGRESO,
                                        INTERES = item.INTERES,
                                        OBSERVACION = item.OBSERVACION,
                                        RENDIMIENTO = item.RENDIMIENTO,
                                        ID_CONSEC = _consecutivoEduca
                                    };

                                    Context.PFC_VII_ESCOLARIDAD_ANTERIOR.Add(_NuevaEscolaridad);
                                    _consecutivoEduca++;
                                };

                            Context.PFC_VII_EDUCATIVO.Add(_edu);
                        }
                        else
                        {
                            var _EstudioEducativo = Context.PFC_VII_EDUCATIVO.FirstOrDefault(x => x.ID_IMPUTADO == _EstudioPadre.ID_IMPUTADO && x.ID_INGRESO == _EstudioPadre.ID_INGRESO && x.ID_ESTUDIO == _EstudioPadre.ID_ESTUDIO && x.ID_CENTRO == _EstudioPadre.ID_CENTRO);
                            var _Actividades = Context.PFC_VII_ACTIVIDAD.Where(c => c.ID_INGRESO == _EstudioPadre.ID_INGRESO && c.ID_IMPUTADO == _EstudioPadre.ID_IMPUTADO && c.ID_ESTUDIO == _EstudioPadre.ID_ESTUDIO && c.ID_CENTRO == _EstudioPadre.ID_CENTRO);
                            var _EscolaridadesAnteriores = Context.PFC_VII_ESCOLARIDAD_ANTERIOR.Where(x => x.ID_IMPUTADO == _EstudioPadre.ID_IMPUTADO && x.ID_INGRESO == _EstudioPadre.ID_INGRESO && x.ID_ESTUDIO == _EstudioPadre.ID_ESTUDIO && x.ID_CENTRO == _EstudioPadre.ID_CENTRO);

                            _eduActual.COORDINADOR = Entity.COORDINADOR;
                            _eduActual.ELABORO = Entity.ELABORO;
                            _eduActual.ESTUDIO_FEC = Entity.ESTUDIO_FEC;
                            _eduActual.ID_ANIO = _EstudioPadre.ID_ANIO;
                            _eduActual.ID_CENTRO = _EstudioPadre.ID_CENTRO;
                            _eduActual.ID_ESTUDIO = _EstudioPadre.ID_ESTUDIO;
                            _eduActual.ID_IMPUTADO = _EstudioPadre.ID_IMPUTADO;
                            _eduActual.ID_INGRESO = _EstudioPadre.ID_INGRESO;
                            _eduActual.P3_DICTAMEN = Entity.P3_DICTAMEN.HasValue ? Entity.P3_DICTAMEN.Value != decimal.Zero ? Entity.P3_DICTAMEN : null : null;
                            _eduActual.P4_MOTIVACION_DICTAMEN = Entity.P4_MOTIVACION_DICTAMEN;
                            Context.Entry(_eduActual).State = System.Data.EntityState.Modified;

                            if (_Actividades != null && _Actividades.Any())
                                foreach (var item in _Actividades)
                                    Context.Entry(item).State = System.Data.EntityState.Deleted;

                            var _consecutivoTrabajo = GetIDProceso<short>("PFC_VII_ACTIVIDAD", "ID_CONSEC", string.Format("ID_CENTRO = {0} AND ID_ANIO = {1} AND ID_IMPUTADO = {2} AND ID_INGRESO = {3} AND ID_ESTUDIO = {4}", _EstudioPadre.ID_CENTRO, _EstudioPadre.ID_ANIO, _EstudioPadre.ID_IMPUTADO, _EstudioPadre.ID_INGRESO, _EstudioPadre.ID_ESTUDIO));
                            if (Entity.PFC_VII_ACTIVIDAD != null && Entity.PFC_VII_ACTIVIDAD.Any())
                                foreach (var item in Entity.PFC_VII_ACTIVIDAD)
                                {
                                    var _NuevaActividad = new PFC_VII_ACTIVIDAD()
                                    {
                                        ACTIVIDAD = item.ACTIVIDAD,
                                        DURACION = item.DURACION,
                                        ID_ANIO = _EstudioPadre.ID_ANIO,
                                        ID_CENTRO = _EstudioPadre.ID_CENTRO,
                                        ID_ESTUDIO = _EstudioPadre.ID_ESTUDIO,
                                        ID_IMPUTADO = _EstudioPadre.ID_IMPUTADO,
                                        ID_CONSEC = _consecutivoTrabajo,
                                        ID_INGRESO = _EstudioPadre.ID_INGRESO,
                                        ID_PROGRAMA = null,
                                        OBSERVACION = item.OBSERVACION,
                                        TIPO = item.TIPO,
                                        ID_ACTIVIDAD = item.ID_ACTIVIDAD,
                                        ID_TIPO_PROGRAMA = item.ID_TIPO_PROGRAMA
                                    };

                                    _eduActual.PFC_VII_ACTIVIDAD.Add(_NuevaActividad);
                                    _consecutivoTrabajo++;
                                };

                            if (_EscolaridadesAnteriores != null && _EscolaridadesAnteriores.Any())
                                foreach (var item in _EscolaridadesAnteriores)
                                    Context.Entry(item).State = System.Data.EntityState.Deleted;

                            var _consecutivoEduca = GetIDProceso<short>("PFC_VII_ESCOLARIDAD_ANTERIOR", "ID_CONSEC", string.Format("ID_CENTRO = {0} AND ID_ANIO = {1} AND ID_IMPUTADO = {2} AND ID_INGRESO = {3} AND ID_ESTUDIO = {4}", _EstudioPadre.ID_CENTRO, _EstudioPadre.ID_ANIO, _EstudioPadre.ID_IMPUTADO, _EstudioPadre.ID_INGRESO, _EstudioPadre.ID_ESTUDIO));

                            if (Entity.PFC_VII_ESCOLARIDAD_ANTERIOR != null && Entity.PFC_VII_ESCOLARIDAD_ANTERIOR.Any())
                                foreach (var item in Entity.PFC_VII_ESCOLARIDAD_ANTERIOR)
                                {
                                    var _NuevaEscolaridad = new PFC_VII_ESCOLARIDAD_ANTERIOR()
                                    {
                                        CONCLUIDA = item.CONCLUIDA,
                                        ID_ANIO = _EstudioPadre.ID_ANIO,
                                        ID_CENTRO = _EstudioPadre.ID_CENTRO,
                                        ID_ESTUDIO = _EstudioPadre.ID_ESTUDIO,
                                        ID_GRADO = item.ID_GRADO,
                                        ID_IMPUTADO = _EstudioPadre.ID_IMPUTADO,
                                        ID_INGRESO = _EstudioPadre.ID_INGRESO,
                                        INTERES = item.INTERES,
                                        OBSERVACION = item.OBSERVACION,
                                        RENDIMIENTO = item.RENDIMIENTO,
                                        ID_CONSEC = _consecutivoEduca
                                    };

                                    Context.PFC_VII_ESCOLARIDAD_ANTERIOR.Add(_NuevaEscolaridad);
                                    _consecutivoEduca++;
                                };
                        }
                    }

                    #region Detalle VII
                    if (_EstudioPadre != null)
                    {
                        short? _Estatus = new short?();
                        var _DetalleEduc = Context.PERSONALIDAD.FirstOrDefault(x => x.ID_ESTUDIO == _EstudioPadre.ID_ESTUDIO && x.ID_ANIO == _EstudioPadre.ID_ANIO && x.ID_CENTRO == _EstudioPadre.ID_CENTRO && x.ID_IMPUTADO == _EstudioPadre.ID_IMPUTADO && x.ID_INGRESO == _EstudioPadre.ID_INGRESO);
                        if (string.IsNullOrEmpty(Entity.COORDINADOR) || string.IsNullOrEmpty(Entity.ELABORO) || string.IsNullOrEmpty(Entity.P4_MOTIVACION_DICTAMEN) || Entity.ESTUDIO_FEC == null || Entity.P3_DICTAMEN == null)
                            _Estatus = (short)eEstatudDetallePersonalidad.ASIGNADO;
                        else
                            _Estatus = (short)eEstatudDetallePersonalidad.TERMINADO;

                        if (_DetalleEduc != null)
                        {
                            var _DesarrolloEstudioCrimi = Context.PERSONALIDAD_DETALLE.FirstOrDefault(c => c.ID_ESTUDIO == _DetalleEduc.ID_ESTUDIO && c.ID_TIPO == (short)eTiposEstudio.PEDAGOGIA && c.ID_IMPUTADO == _DetalleEduc.ID_IMPUTADO && c.ID_INGRESO == _DetalleEduc.ID_INGRESO && c.ID_CENTRO == _DetalleEduc.ID_CENTRO && c.ID_ANIO == _DetalleEduc.ID_ANIO);
                            if (_DesarrolloEstudioCrimi == null)
                            {
                                short _ConsecutivoPersonalidadDetalle = GetIDProceso<short>("PERSONALIDAD_DETALLE", "ID_DETALLE", "1=1");
                                var _NvoDetalle = new PERSONALIDAD_DETALLE()
                                {
                                    DIAS_BONIFICADOS = null,
                                    ID_ANIO = Entity.ID_ANIO,
                                    ID_CENTRO = Entity.ID_CENTRO,
                                    ID_DETALLE = _ConsecutivoPersonalidadDetalle,
                                    ID_ESTUDIO = _EstudioPadre.ID_ESTUDIO,
                                    ID_IMPUTADO = Entity.ID_IMPUTADO,
                                    ID_INGRESO = Entity.ID_INGRESO,
                                    ID_TIPO = (short)eTiposEstudio.PEDAGOGIA,
                                    INICIO_FEC = GetFechaServerDate(),
                                    RESULTADO = Entity.P3_DICTAMEN.HasValue ? Entity.P3_DICTAMEN == (short)eResultado.FAVORABLE ? "S" : Entity.P3_DICTAMEN == (short)eResultado.DESFAVORABLE ? "N" : string.Empty : string.Empty,
                                    SOLICITUD_FEC = _EstudioPadre.SOLICITUD_FEC,
                                    ID_ESTATUS = _Estatus,
                                    TERMINO_FEC = _Estatus == (short)eEstatudDetallePersonalidad.TERMINADO ? GetFechaServerDate() : new System.DateTime?(),
                                    TIPO_MEDIA = string.Empty
                                };

                                Context.PERSONALIDAD_DETALLE.Add(_NvoDetalle);
                            }

                            else
                            {
                                _DesarrolloEstudioCrimi.RESULTADO = Entity.P3_DICTAMEN.HasValue ? Entity.P3_DICTAMEN == (short)eResultado.FAVORABLE ? "S" : Entity.P3_DICTAMEN == (short)eResultado.DESFAVORABLE ? "N" : string.Empty : string.Empty;
                                _DesarrolloEstudioCrimi.ID_ESTATUS = _Estatus;
                                Context.Entry(_DesarrolloEstudioCrimi).State = System.Data.EntityState.Modified;
                            }
                        }
                    }
                    #endregion

                    Context.SaveChanges();
                    transaccion.Complete();
                    return 1;

                }
            }
            catch (System.Data.Entity.Validation.DbEntityValidationException dbEx)
            {
                foreach (var validationErrors in dbEx.EntityValidationErrors)
                    foreach (var validationError in validationErrors.ValidationErrors)
                        System.Diagnostics.Trace.TraceInformation("Nombre del causante de excepción: {0} Error: {1}", validationError.PropertyName, validationError.ErrorMessage);

                return 2;
            }

            catch (System.Exception exc)
            {
                return 0;
            }
        }

        public short GuardarCapacitacionComun(PFC_VIII_TRABAJO Entity)
        {
            try
            {
                using (System.Transactions.TransactionScope transaccion = new System.Transactions.TransactionScope(System.Transactions.TransactionScopeOption.RequiresNew, new System.Transactions.TransactionOptions() { IsolationLevel = System.Transactions.IsolationLevel.ReadCommitted }))
                {
                    var _EstudioPadre = Context.PERSONALIDAD.Where(x => x.ID_INGRESO == Entity.ID_INGRESO && x.ID_IMPUTADO == Entity.ID_IMPUTADO && x.ID_CENTRO == Entity.ID_CENTRO && x.ID_ANIO == Entity.ID_ANIO).OrderByDescending(x => x.ID_ESTUDIO).FirstOrDefault();

                    if (_EstudioPadre == null)
                        return 0;

                    var _Actual = Context.PERSONALIDAD_FUERO_COMUN.FirstOrDefault(x => x.ID_ANIO == _EstudioPadre.ID_ANIO && x.ID_CENTRO == _EstudioPadre.ID_CENTRO && x.ID_IMPUTADO == _EstudioPadre.ID_IMPUTADO && x.ID_INGRESO == _EstudioPadre.ID_INGRESO && x.ID_ESTUDIO == _EstudioPadre.ID_ESTUDIO);

                    if (_Actual == null)
                    {
                        var personalidadComun = new PERSONALIDAD_FUERO_COMUN()
                        {
                            ID_ANIO = _EstudioPadre.ID_ANIO,
                            ID_IMPUTADO = _EstudioPadre.ID_IMPUTADO,
                            ID_CENTRO = _EstudioPadre.ID_CENTRO,
                            ID_ESTUDIO = _EstudioPadre.ID_ESTUDIO,
                            ID_INGRESO = _EstudioPadre.ID_INGRESO
                        };

                        var ListaDatosAnteriores = Context.PFC_VIII_ACTIVIDAD_LABORAL.Where(x => x.ID_INGRESO == _EstudioPadre.ID_INGRESO && x.ID_IMPUTADO == _EstudioPadre.ID_IMPUTADO && x.ID_ESTUDIO == _EstudioPadre.ID_ESTUDIO && x.ID_CENTRO == _EstudioPadre.ID_CENTRO);

                        var _trabajoComun = new PFC_VIII_TRABAJO()
                                    {
                                        COORDINADOR = Entity.COORDINADOR,
                                        ELABORO = Entity.ELABORO,
                                        ESTUDIO_FEC = Entity.ESTUDIO_FEC,
                                        ID_ANIO = _EstudioPadre.ID_ANIO,
                                        ID_CENTRO = _EstudioPadre.ID_CENTRO,
                                        ID_ESTUDIO = _EstudioPadre.ID_ESTUDIO,
                                        ID_IMPUTADO = _EstudioPadre.ID_IMPUTADO,
                                        ID_INGRESO = _EstudioPadre.ID_INGRESO,
                                        P1_TRABAJO_ANTES = Entity.P1_TRABAJO_ANTES,
                                        P3_CALIDAD = Entity.P3_CALIDAD,
                                        P3_PERSEVERANCIA = Entity.P3_PERSEVERANCIA,
                                        P3_RESPONSABILIDAD = Entity.P3_RESPONSABILIDAD,
                                        P4_FONDO_AHORRO = Entity.P4_FONDO_AHORRO,
                                        P5_DIAS_CENTRO_ACTUAL = Entity.P5_DIAS_CENTRO_ACTUAL,
                                        P5_DIAS_LABORADOS = Entity.P5_DIAS_LABORADOS,
                                        P5_DIAS_OTROS_CENTROS = Entity.P5_DIAS_OTROS_CENTROS,
                                        P5_PERIODO_LABORAL = Entity.P5_PERIODO_LABORAL,
                                        P6_DICTAMEN = Entity.P6_DICTAMEN,
                                        P7_MOTIVACION = Entity.P7_MOTIVACION
                                    };

                        if (ListaDatosAnteriores.Any())
                            foreach (var item in ListaDatosAnteriores)
                                Context.Entry(item).State = System.Data.EntityState.Deleted;

                        var _consecutivoLaboral = GetIDProceso<short>("PFC_VIII_ACTIVIDAD_LABORAL", "ID_CONSEC", string.Format("ID_CENTRO = {0} AND ID_ANIO = {1} AND ID_IMPUTADO = {2} AND ID_INGRESO = {3} AND ID_ESTUDIO = {4}", _EstudioPadre.ID_CENTRO, _EstudioPadre.ID_ANIO, _EstudioPadre.ID_IMPUTADO, _EstudioPadre.ID_INGRESO, _EstudioPadre.ID_ESTUDIO));

                        if (Entity.PFC_VIII_ACTIVIDAD_LABORAL != null && Entity.PFC_VIII_ACTIVIDAD_LABORAL.Any())
                            foreach (var item in Entity.PFC_VIII_ACTIVIDAD_LABORAL)
                            {
                                var NvaActividadLaboral = new PFC_VIII_ACTIVIDAD_LABORAL()
                                {
                                    CONCLUYO = item.CONCLUYO,
                                    ID_ANIO = Entity.ID_ANIO,
                                    ID_CAPACITACION = item.ID_CAPACITACION,
                                    ID_CENTRO = _EstudioPadre.ID_CENTRO,
                                    ID_CONSEC = _consecutivoLaboral,
                                    ID_ESTUDIO = _EstudioPadre.ID_ESTUDIO,
                                    ID_IMPUTADO = _EstudioPadre.ID_IMPUTADO,
                                    ID_ACTIVIDAD = item.ID_ACTIVIDAD,
                                    ID_TIPO_PROGRAMA = item.ID_TIPO_PROGRAMA,
                                    ID_INGRESO = _EstudioPadre.ID_INGRESO,
                                    OBSERVACION = item.OBSERVACION,
                                    PERIODO = item.PERIODO
                                };

                                Context.PFC_VIII_ACTIVIDAD_LABORAL.Add(NvaActividadLaboral);
                                _consecutivoLaboral++;
                            };

                        Context.PERSONALIDAD_FUERO_COMUN.Add(personalidadComun);
                        Context.PFC_VIII_TRABAJO.Add(_trabajoComun);
                    }
                    else
                    {
                        var _trabActua = _Actual.PFC_VIII_TRABAJO;
                        if (_trabActua == null)
                        {
                            var ListaDatosAnteriores = Context.PFC_VIII_ACTIVIDAD_LABORAL.Where(x => x.ID_INGRESO == _EstudioPadre.ID_INGRESO && x.ID_IMPUTADO == _EstudioPadre.ID_IMPUTADO && x.ID_ESTUDIO == _EstudioPadre.ID_ESTUDIO && x.ID_CENTRO == _EstudioPadre.ID_CENTRO);

                            _trabActua = new PFC_VIII_TRABAJO()
                            {
                                COORDINADOR = Entity.COORDINADOR,
                                ELABORO = Entity.ELABORO,
                                ESTUDIO_FEC = Entity.ESTUDIO_FEC,
                                ID_ANIO = _EstudioPadre.ID_ANIO,
                                ID_CENTRO = _EstudioPadre.ID_CENTRO,
                                ID_ESTUDIO = _EstudioPadre.ID_ESTUDIO,
                                ID_IMPUTADO = _EstudioPadre.ID_IMPUTADO,
                                ID_INGRESO = _EstudioPadre.ID_INGRESO,
                                P1_TRABAJO_ANTES = Entity.P1_TRABAJO_ANTES,
                                P3_CALIDAD = Entity.P3_CALIDAD,
                                P3_PERSEVERANCIA = Entity.P3_PERSEVERANCIA,
                                P3_RESPONSABILIDAD = Entity.P3_RESPONSABILIDAD,
                                P4_FONDO_AHORRO = Entity.P4_FONDO_AHORRO,
                                P5_DIAS_CENTRO_ACTUAL = Entity.P5_DIAS_CENTRO_ACTUAL,
                                P5_DIAS_LABORADOS = Entity.P5_DIAS_LABORADOS,
                                P5_DIAS_OTROS_CENTROS = Entity.P5_DIAS_OTROS_CENTROS,
                                P5_PERIODO_LABORAL = Entity.P5_PERIODO_LABORAL,
                                P6_DICTAMEN = Entity.P6_DICTAMEN,
                                P7_MOTIVACION = Entity.P7_MOTIVACION
                            };

                            if (ListaDatosAnteriores != null && ListaDatosAnteriores.Any())
                                foreach (var item in ListaDatosAnteriores)
                                    Context.Entry(item).State = System.Data.EntityState.Deleted;

                            var _consecutivoLaboral = GetIDProceso<short>("PFC_VIII_ACTIVIDAD_LABORAL", "ID_CONSEC", string.Format("ID_CENTRO = {0} AND ID_ANIO = {1} AND ID_IMPUTADO = {2} AND ID_INGRESO = {3} AND ID_ESTUDIO = {4}", _EstudioPadre.ID_CENTRO, _EstudioPadre.ID_ANIO, _EstudioPadre.ID_IMPUTADO, _EstudioPadre.ID_INGRESO, _EstudioPadre.ID_ESTUDIO));

                            if (Entity.PFC_VIII_ACTIVIDAD_LABORAL != null && Entity.PFC_VIII_ACTIVIDAD_LABORAL.Any())
                                foreach (var item in Entity.PFC_VIII_ACTIVIDAD_LABORAL)
                                {
                                    var NvaActividadLaboral = new PFC_VIII_ACTIVIDAD_LABORAL()
                                    {
                                        CONCLUYO = item.CONCLUYO,
                                        ID_ANIO = Entity.ID_ANIO,
                                        ID_CAPACITACION = null,//campo basura
                                        ID_CENTRO = _EstudioPadre.ID_CENTRO,
                                        ID_CONSEC = _consecutivoLaboral,
                                        ID_ACTIVIDAD = item.ID_ACTIVIDAD,
                                        ID_TIPO_PROGRAMA = item.ID_TIPO_PROGRAMA,
                                        ID_ESTUDIO = _EstudioPadre.ID_ESTUDIO,
                                        ID_IMPUTADO = _EstudioPadre.ID_IMPUTADO,
                                        ID_INGRESO = _EstudioPadre.ID_INGRESO,
                                        OBSERVACION = item.OBSERVACION,
                                        PERIODO = item.PERIODO
                                    };

                                    Context.PFC_VIII_ACTIVIDAD_LABORAL.Add(NvaActividadLaboral);
                                    _consecutivoLaboral++;
                                };

                            Context.PFC_VIII_TRABAJO.Add(_trabActua);
                        }
                        else
                        {
                            var ListaDatosAnteriores = Context.PFC_VIII_ACTIVIDAD_LABORAL.Where(x => x.ID_INGRESO == _EstudioPadre.ID_INGRESO && x.ID_IMPUTADO == _EstudioPadre.ID_IMPUTADO && x.ID_ESTUDIO == _EstudioPadre.ID_ESTUDIO && x.ID_CENTRO == _EstudioPadre.ID_CENTRO);

                            _trabActua.ID_ANIO = _EstudioPadre.ID_ANIO;
                            _trabActua.ID_CENTRO = _EstudioPadre.ID_CENTRO;
                            _trabActua.ID_ESTUDIO = _EstudioPadre.ID_ESTUDIO;
                            _trabActua.ID_IMPUTADO = _EstudioPadre.ID_IMPUTADO;
                            _trabActua.ID_INGRESO = _EstudioPadre.ID_INGRESO;
                            _trabActua.COORDINADOR = Entity.COORDINADOR;
                            _trabActua.ELABORO = Entity.ELABORO;
                            _trabActua.ESTUDIO_FEC = Entity.ESTUDIO_FEC;
                            _trabActua.P1_TRABAJO_ANTES = Entity.P1_TRABAJO_ANTES;
                            _trabActua.P3_CALIDAD = Entity.P3_CALIDAD;
                            _trabActua.P3_PERSEVERANCIA = Entity.P3_PERSEVERANCIA;
                            _trabActua.P3_RESPONSABILIDAD = Entity.P3_RESPONSABILIDAD;
                            _trabActua.P4_FONDO_AHORRO = Entity.P4_FONDO_AHORRO;
                            _trabActua.P5_DIAS_CENTRO_ACTUAL = Entity.P5_DIAS_CENTRO_ACTUAL;
                            _trabActua.P5_DIAS_LABORADOS = Entity.P5_DIAS_LABORADOS;
                            _trabActua.P5_DIAS_OTROS_CENTROS = Entity.P5_DIAS_OTROS_CENTROS;
                            _trabActua.P5_PERIODO_LABORAL = Entity.P5_PERIODO_LABORAL;
                            _trabActua.P6_DICTAMEN = Entity.P6_DICTAMEN;
                            _trabActua.P7_MOTIVACION = Entity.P7_MOTIVACION;
                            Context.Entry(_trabActua).State = System.Data.EntityState.Modified;

                            if (ListaDatosAnteriores != null && ListaDatosAnteriores.Any())
                                foreach (var item in ListaDatosAnteriores)
                                    Context.Entry(item).State = System.Data.EntityState.Deleted;

                            var _consecutivoLaboral = GetIDProceso<short>("PFC_VIII_ACTIVIDAD_LABORAL", "ID_CONSEC", string.Format("ID_CENTRO = {0} AND ID_ANIO = {1} AND ID_IMPUTADO = {2} AND ID_INGRESO = {3} AND ID_ESTUDIO = {4}", _EstudioPadre.ID_CENTRO, _EstudioPadre.ID_ANIO, _EstudioPadre.ID_IMPUTADO, _EstudioPadre.ID_INGRESO, _EstudioPadre.ID_ESTUDIO));

                            if (Entity.PFC_VIII_ACTIVIDAD_LABORAL != null && Entity.PFC_VIII_ACTIVIDAD_LABORAL.Any())
                                foreach (var item in Entity.PFC_VIII_ACTIVIDAD_LABORAL)
                                {
                                    var NvaActividadLaboral = new PFC_VIII_ACTIVIDAD_LABORAL()
                                    {
                                        CONCLUYO = item.CONCLUYO,
                                        ID_ANIO = Entity.ID_ANIO,
                                        ID_CAPACITACION = null,
                                        ID_CENTRO = Entity.ID_CENTRO,
                                        ID_CONSEC = _consecutivoLaboral,
                                        ID_ESTUDIO = Entity.ID_ESTUDIO,
                                        ID_IMPUTADO = Entity.ID_IMPUTADO,
                                        ID_INGRESO = Entity.ID_INGRESO,
                                        OBSERVACION = item.OBSERVACION,
                                        PERIODO = item.PERIODO,
                                        ID_ACTIVIDAD = item.ID_ACTIVIDAD,
                                        ID_TIPO_PROGRAMA = item.ID_TIPO_PROGRAMA
                                    };

                                    Context.PFC_VIII_ACTIVIDAD_LABORAL.Add(NvaActividadLaboral);
                                    _consecutivoLaboral++;
                                };
                        }
                    }

                    #region Detalle
                    if (_EstudioPadre != null)
                    {
                        short? _Estatus = new short?();
                        var _DetalleEduc = Context.PERSONALIDAD.FirstOrDefault(x => x.ID_ESTUDIO == _EstudioPadre.ID_ESTUDIO && x.ID_ANIO == _EstudioPadre.ID_ANIO && x.ID_CENTRO == _EstudioPadre.ID_CENTRO && x.ID_IMPUTADO == _EstudioPadre.ID_IMPUTADO && x.ID_INGRESO == _EstudioPadre.ID_INGRESO);
                        if (string.IsNullOrEmpty(Entity.COORDINADOR) || string.IsNullOrEmpty(Entity.ELABORO) || string.IsNullOrEmpty(Entity.P1_TRABAJO_ANTES) || string.IsNullOrEmpty(Entity.P3_CALIDAD)
                            || string.IsNullOrEmpty(Entity.P3_PERSEVERANCIA) || string.IsNullOrEmpty(Entity.P3_RESPONSABILIDAD) || string.IsNullOrEmpty(Entity.P4_FONDO_AHORRO) || string.IsNullOrEmpty(Entity.P6_DICTAMEN)
                            || string.IsNullOrEmpty(Entity.P7_MOTIVACION) || Entity.ESTUDIO_FEC == null || Entity.P5_DIAS_CENTRO_ACTUAL == null || Entity.P5_DIAS_LABORADOS == null || Entity.P5_DIAS_OTROS_CENTROS == null
                            || Entity.P5_PERIODO_LABORAL == null)
                            _Estatus = (short)eEstatudDetallePersonalidad.ASIGNADO;
                        else
                            _Estatus = (short)eEstatudDetallePersonalidad.TERMINADO;

                        if (_DetalleEduc != null)
                        {
                            var _DesarrolloEstudioCrimi = Context.PERSONALIDAD_DETALLE.FirstOrDefault(c => c.ID_ESTUDIO == _DetalleEduc.ID_ESTUDIO && c.ID_TIPO == (short)eTiposEstudio.LABORAL && c.ID_IMPUTADO == _DetalleEduc.ID_IMPUTADO && c.ID_INGRESO == _DetalleEduc.ID_INGRESO && c.ID_CENTRO == _DetalleEduc.ID_CENTRO && c.ID_ANIO == _DetalleEduc.ID_ANIO);
                            if (_DesarrolloEstudioCrimi == null)
                            {
                                short _ConsecutivoPersonalidadDetalle = GetIDProceso<short>("PERSONALIDAD_DETALLE", "ID_DETALLE", "1=1");
                                var _NvoDetalle = new PERSONALIDAD_DETALLE()
                                {
                                    DIAS_BONIFICADOS = null,
                                    ID_ANIO = Entity.ID_ANIO,
                                    ID_CENTRO = Entity.ID_CENTRO,
                                    ID_DETALLE = _ConsecutivoPersonalidadDetalle,
                                    ID_ESTUDIO = _EstudioPadre.ID_ESTUDIO,
                                    ID_IMPUTADO = Entity.ID_IMPUTADO,
                                    ID_INGRESO = Entity.ID_INGRESO,
                                    ID_TIPO = (short)eTiposEstudio.LABORAL,
                                    INICIO_FEC = GetFechaServerDate(),
                                    RESULTADO = Entity.P6_DICTAMEN == "S" ? "S" : Entity.P6_DICTAMEN == "D" ? "N" : string.Empty,
                                    SOLICITUD_FEC = _EstudioPadre.SOLICITUD_FEC,
                                    ID_ESTATUS = _Estatus,
                                    TERMINO_FEC = _Estatus == (short)eEstatudDetallePersonalidad.TERMINADO ? GetFechaServerDate() : new System.DateTime?(),
                                    TIPO_MEDIA = string.Empty
                                };

                                Context.PERSONALIDAD_DETALLE.Add(_NvoDetalle);
                            }

                            else
                            {
                                _DesarrolloEstudioCrimi.RESULTADO = Entity.P6_DICTAMEN == "S" ? "S" : Entity.P6_DICTAMEN == "D" ? "N" : string.Empty;
                                _DesarrolloEstudioCrimi.ID_ESTATUS = _Estatus;
                                Context.Entry(_DesarrolloEstudioCrimi).State = System.Data.EntityState.Modified;
                            }
                        }
                    }

                    #endregion

                    Context.SaveChanges();
                    transaccion.Complete();
                    return 1;
                }
            }
            catch (System.Data.Entity.Validation.DbEntityValidationException dbEx)
            {
                foreach (var validationErrors in dbEx.EntityValidationErrors)
                    foreach (var validationError in validationErrors.ValidationErrors)
                        System.Diagnostics.Trace.TraceInformation("Nombre del causante de excepción: {0} Error: {1}", validationError.PropertyName, validationError.ErrorMessage);

                return 2;
            }

            catch (System.Exception exc)
            {
                return 0;
            }
        }

        public short GuardaSeguridadComun(PFC_IX_SEGURIDAD Entity)
        {
            try
            {
                using (System.Transactions.TransactionScope transaccion = new System.Transactions.TransactionScope(System.Transactions.TransactionScopeOption.RequiresNew, new System.Transactions.TransactionOptions() { IsolationLevel = System.Transactions.IsolationLevel.ReadCommitted }))
                {
                    var _EstudioPadre = Context.PERSONALIDAD.Where(x => x.ID_INGRESO == Entity.ID_INGRESO && x.ID_IMPUTADO == Entity.ID_IMPUTADO && x.ID_CENTRO == Entity.ID_CENTRO && x.ID_ANIO == Entity.ID_ANIO).OrderByDescending(x => x.ID_ESTUDIO).FirstOrDefault();

                    if (_EstudioPadre == null)
                        return 0;

                    var _Actual = Context.PERSONALIDAD_FUERO_COMUN.FirstOrDefault(x => x.ID_ANIO == _EstudioPadre.ID_ANIO && x.ID_CENTRO == _EstudioPadre.ID_CENTRO && x.ID_IMPUTADO == _EstudioPadre.ID_IMPUTADO && x.ID_INGRESO == _EstudioPadre.ID_INGRESO && x.ID_ESTUDIO == _EstudioPadre.ID_ESTUDIO);

                    if (_Actual == null)
                    {
                        var personalidadComun = new PERSONALIDAD_FUERO_COMUN()
                        {
                            ID_ANIO = _EstudioPadre.ID_ANIO,
                            ID_IMPUTADO = _EstudioPadre.ID_IMPUTADO,
                            ID_CENTRO = _EstudioPadre.ID_CENTRO,
                            ID_ESTUDIO = _EstudioPadre.ID_ESTUDIO,
                            ID_INGRESO = _EstudioPadre.ID_INGRESO
                        };

                        var CorrectivosSeguridadComun = Context.PFC_IX_CORRECTIVO.Where(x => x.ID_IMPUTADO == _EstudioPadre.ID_IMPUTADO && x.ID_INGRESO == _EstudioPadre.ID_INGRESO && x.ID_ESTUDIO == _EstudioPadre.ID_ESTUDIO && x.ID_CENTRO == _EstudioPadre.ID_CENTRO);

                        var _custoria = new PFC_IX_SEGURIDAD()
                        {
                            COMANDANTE = Entity.COMANDANTE,
                            ELABORO = Entity.ELABORO,
                            ESTUDIO_FEC = Entity.ESTUDIO_FEC,
                            ID_ANIO = _EstudioPadre.ID_ANIO,
                            ID_CENTRO = _EstudioPadre.ID_CENTRO,
                            ID_ESTUDIO = _EstudioPadre.ID_ESTUDIO,
                            ID_IMPUTADO = _EstudioPadre.ID_IMPUTADO,
                            ID_INGRESO = _EstudioPadre.ID_INGRESO,
                            P1_CONDUCTA_CENTRO = Entity.P1_CONDUCTA_CENTRO,
                            P2_CONDUCTA_AUTORIDAD = Entity.P2_CONDUCTA_AUTORIDAD,
                            P3_CONDUCTA_GENERAL = Entity.P3_CONDUCTA_GENERAL.HasValue ? Entity.P3_CONDUCTA_GENERAL.Value != -1 ? Entity.P3_CONDUCTA_GENERAL : null : null,
                            P4_RELACION_COMPANEROS = Entity.P4_RELACION_COMPANEROS.HasValue ? Entity.P4_RELACION_COMPANEROS.Value != -1 ? Entity.P4_RELACION_COMPANEROS : null : null,
                            P5_CORRECTIVOS = Entity.P5_CORRECTIVOS,
                            P6_OPINION_CONDUCTA = Entity.P6_OPINION_CONDUCTA,
                            P7_DICTAMEN = Entity.P7_DICTAMEN,
                            P8_MOTIVACION = Entity.P8_MOTIVACION
                        };

                        #region Correctivos Seguridad Comun

                        if (CorrectivosSeguridadComun != null && CorrectivosSeguridadComun.Any())
                            foreach (var pfcIxCorrectivo in CorrectivosSeguridadComun)
                                Context.Entry(pfcIxCorrectivo).State = EntityState.Deleted;

                        var _consecutivoCorrectivos = GetIDProceso<short>("PFC_IX_CORRECTIVO", "ID_CORRECTIVO", string.Format("ID_CENTRO = {0} AND ID_ANIO = {1} AND ID_IMPUTADO = {2} AND ID_INGRESO = {3} AND ID_ESTUDIO = {4}", _EstudioPadre.ID_CENTRO, _EstudioPadre.ID_ANIO, _EstudioPadre.ID_IMPUTADO, _EstudioPadre.ID_INGRESO, _EstudioPadre.ID_ESTUDIO));
                        if (Entity.PFC_IX_CORRECTIVO != null)
                            if (Entity.PFC_IX_CORRECTIVO.Any())
                                foreach (var item in Entity.PFC_IX_CORRECTIVO)
                                {
                                    var _NuevoCorrectivo = new PFC_IX_CORRECTIVO()
                                    {
                                        CORRECTIVO_FEC = item.CORRECTIVO_FEC,
                                        ID_ANIO = _EstudioPadre.ID_ANIO,
                                        ID_CENTRO = _EstudioPadre.ID_CENTRO,
                                        ID_ESTUDIO = _EstudioPadre.ID_ESTUDIO,
                                        ID_IMPUTADO = _EstudioPadre.ID_IMPUTADO,
                                        ID_INGRESO = _EstudioPadre.ID_INGRESO,
                                        MOTIVO = item.MOTIVO,
                                        SANCION = item.SANCION,
                                        ID_CORRECTIVO = _consecutivoCorrectivos
                                    };

                                    Context.PFC_IX_CORRECTIVO.Add(_NuevoCorrectivo);
                                    _consecutivoCorrectivos++;
                                };

                        #endregion

                        Context.PERSONALIDAD_FUERO_COMUN.Add(personalidadComun);
                        Context.PFC_IX_SEGURIDAD.Add(_custoria);
                    }
                    else
                    {
                        var _ActualS = _Actual.PFC_IX_SEGURIDAD;
                        if (_ActualS != null)
                        {

                            var _consecutivoCorrectivos = GetIDProceso<short>("PFC_IX_CORRECTIVO", "ID_CORRECTIVO", string.Format("ID_CENTRO = {0} AND ID_ANIO = {1} AND ID_IMPUTADO = {2} AND ID_INGRESO = {3} AND ID_ESTUDIO = {4}", _EstudioPadre.ID_CENTRO, _EstudioPadre.ID_ANIO, _EstudioPadre.ID_IMPUTADO, _EstudioPadre.ID_INGRESO, _EstudioPadre.ID_ESTUDIO));

                            var CorrectivosSeguridadComun = Context.PFC_IX_CORRECTIVO.Where(x => x.ID_IMPUTADO == _EstudioPadre.ID_IMPUTADO && x.ID_INGRESO == _EstudioPadre.ID_INGRESO && x.ID_ESTUDIO == _EstudioPadre.ID_ESTUDIO && x.ID_CENTRO == _EstudioPadre.ID_CENTRO);

                            _ActualS.COMANDANTE = Entity.COMANDANTE;
                            _ActualS.ELABORO = Entity.ELABORO;
                            _ActualS.ESTUDIO_FEC = Entity.ESTUDIO_FEC;
                            _ActualS.ID_ANIO = _EstudioPadre.ID_ANIO;
                            _ActualS.ID_CENTRO = _EstudioPadre.ID_CENTRO;
                            _ActualS.ID_ESTUDIO = _EstudioPadre.ID_ESTUDIO;
                            _ActualS.ID_IMPUTADO = _EstudioPadre.ID_IMPUTADO;
                            _ActualS.ID_INGRESO = _EstudioPadre.ID_INGRESO;
                            _ActualS.P1_CONDUCTA_CENTRO = Entity.P1_CONDUCTA_CENTRO;
                            _ActualS.P2_CONDUCTA_AUTORIDAD = Entity.P2_CONDUCTA_AUTORIDAD;
                            _ActualS.P3_CONDUCTA_GENERAL = Entity.P3_CONDUCTA_GENERAL.HasValue ? Entity.P3_CONDUCTA_GENERAL.Value != -1 ? Entity.P3_CONDUCTA_GENERAL : null : null;
                            _ActualS.P4_RELACION_COMPANEROS = Entity.P4_RELACION_COMPANEROS.HasValue ? Entity.P4_RELACION_COMPANEROS.Value != -1 ? Entity.P4_RELACION_COMPANEROS : null : null;
                            _ActualS.P5_CORRECTIVOS = Entity.P5_CORRECTIVOS;
                            _ActualS.P6_OPINION_CONDUCTA = Entity.P6_OPINION_CONDUCTA;
                            _ActualS.P7_DICTAMEN = Entity.P7_DICTAMEN;
                            _ActualS.P8_MOTIVACION = Entity.P8_MOTIVACION;
                            Context.Entry(_ActualS).State = System.Data.EntityState.Modified;

                            #region Correctivos Seguridad Comun
                            if (CorrectivosSeguridadComun != null && CorrectivosSeguridadComun.Any())
                                foreach (var item in CorrectivosSeguridadComun)
                                    Context.Entry(item).State = System.Data.EntityState.Deleted;

                            if (Entity.PFC_IX_CORRECTIVO != null)
                                if (Entity.PFC_IX_CORRECTIVO.Any())
                                    foreach (var item in Entity.PFC_IX_CORRECTIVO)
                                    {
                                        var _NuevoCorrectivo = new PFC_IX_CORRECTIVO()
                                        {
                                            CORRECTIVO_FEC = item.CORRECTIVO_FEC,
                                            ID_ANIO = Entity.ID_ANIO,
                                            ID_CENTRO = Entity.ID_CENTRO,
                                            ID_CORRECTIVO = _consecutivoCorrectivos,
                                            ID_ESTUDIO = item.ID_ESTUDIO,
                                            ID_IMPUTADO = Entity.ID_IMPUTADO,
                                            ID_INGRESO = Entity.ID_INGRESO,
                                            MOTIVO = item.MOTIVO,
                                            SANCION = item.SANCION
                                        };

                                        _ActualS.PFC_IX_CORRECTIVO.Add(_NuevoCorrectivo);
                                        _consecutivoCorrectivos++;
                                    };

                            #endregion
                        }
                        else
                        {
                            var CorrectivosSeguridadComun = Context.PFC_IX_CORRECTIVO.Where(x => x.ID_IMPUTADO == _EstudioPadre.ID_IMPUTADO && x.ID_INGRESO == _EstudioPadre.ID_INGRESO && x.ID_ESTUDIO == _EstudioPadre.ID_ESTUDIO && x.ID_CENTRO == _EstudioPadre.ID_CENTRO);
                            _ActualS = new PFC_IX_SEGURIDAD()
                            {
                                COMANDANTE = Entity.COMANDANTE,
                                ELABORO = Entity.ELABORO,
                                ESTUDIO_FEC = Entity.ESTUDIO_FEC,
                                ID_ANIO = _EstudioPadre.ID_ANIO,
                                ID_CENTRO = _EstudioPadre.ID_CENTRO,
                                ID_ESTUDIO = _EstudioPadre.ID_ESTUDIO,
                                ID_IMPUTADO = _EstudioPadre.ID_IMPUTADO,
                                ID_INGRESO = _EstudioPadre.ID_INGRESO,
                                P1_CONDUCTA_CENTRO = Entity.P1_CONDUCTA_CENTRO,
                                P2_CONDUCTA_AUTORIDAD = Entity.P2_CONDUCTA_AUTORIDAD,
                                P3_CONDUCTA_GENERAL = Entity.P3_CONDUCTA_GENERAL.HasValue ? Entity.P3_CONDUCTA_GENERAL.Value != -1 ? Entity.P3_CONDUCTA_GENERAL : null : null,
                                P4_RELACION_COMPANEROS = Entity.P4_RELACION_COMPANEROS.HasValue ? Entity.P4_RELACION_COMPANEROS.Value != -1 ? Entity.P4_RELACION_COMPANEROS : null : null,
                                P5_CORRECTIVOS = Entity.P5_CORRECTIVOS,
                                P6_OPINION_CONDUCTA = Entity.P6_OPINION_CONDUCTA,
                                P7_DICTAMEN = Entity.P7_DICTAMEN,
                                P8_MOTIVACION = Entity.P8_MOTIVACION
                            };

                            #region Correctivos Seguridad Comun
                            if (CorrectivosSeguridadComun != null && CorrectivosSeguridadComun.Any())
                                foreach (var item in CorrectivosSeguridadComun)
                                    Context.Entry(item).State = System.Data.EntityState.Deleted;

                            var _consecutivoCorrectivos = GetIDProceso<short>("PFC_IX_CORRECTIVO", "ID_CORRECTIVO", string.Format("ID_CENTRO = {0} AND ID_ANIO = {1} AND ID_IMPUTADO = {2} AND ID_INGRESO = {3} AND ID_ESTUDIO = {4}", _EstudioPadre.ID_CENTRO, _EstudioPadre.ID_ANIO, _EstudioPadre.ID_IMPUTADO, _EstudioPadre.ID_INGRESO, _EstudioPadre.ID_ESTUDIO));
                            if (Entity.PFC_IX_CORRECTIVO != null)
                                if (Entity.PFC_IX_CORRECTIVO.Any())
                                    foreach (var item in Entity.PFC_IX_CORRECTIVO)
                                    {
                                        var _NuevoCorrectivo = new PFC_IX_CORRECTIVO()
                                        {
                                            CORRECTIVO_FEC = item.CORRECTIVO_FEC,
                                            ID_ANIO = _EstudioPadre.ID_ANIO,
                                            ID_CENTRO = _EstudioPadre.ID_CENTRO,
                                            ID_ESTUDIO = _EstudioPadre.ID_ESTUDIO,
                                            ID_IMPUTADO = _EstudioPadre.ID_IMPUTADO,
                                            ID_INGRESO = _EstudioPadre.ID_INGRESO,
                                            MOTIVO = item.MOTIVO,
                                            SANCION = item.SANCION,
                                            ID_CORRECTIVO = _consecutivoCorrectivos
                                        };

                                        Context.PFC_IX_CORRECTIVO.Add(_NuevoCorrectivo);
                                        _consecutivoCorrectivos++;
                                    };

                            #endregion

                            Context.PFC_IX_SEGURIDAD.Add(_ActualS);
                        }
                    }


                    #region Detalle Seguridad

                    if (_EstudioPadre != null)
                    {
                        short? _Estatus = new short?();
                        var _DetalleSegu = Context.PERSONALIDAD.FirstOrDefault(x => x.ID_ESTUDIO == _EstudioPadre.ID_ESTUDIO && x.ID_ANIO == _EstudioPadre.ID_ANIO && x.ID_CENTRO == _EstudioPadre.ID_CENTRO && x.ID_IMPUTADO == _EstudioPadre.ID_IMPUTADO && x.ID_INGRESO == _EstudioPadre.ID_INGRESO);
                        if (string.IsNullOrEmpty(Entity.COMANDANTE) || string.IsNullOrEmpty(Entity.ELABORO) || string.IsNullOrEmpty(Entity.P1_CONDUCTA_CENTRO) || string.IsNullOrEmpty(Entity.P2_CONDUCTA_AUTORIDAD)
                            || string.IsNullOrEmpty(Entity.P5_CORRECTIVOS) || string.IsNullOrEmpty(Entity.P6_OPINION_CONDUCTA) || string.IsNullOrEmpty(Entity.P7_DICTAMEN) || string.IsNullOrEmpty(Entity.P8_MOTIVACION)
                            || Entity.ESTUDIO_FEC == null || Entity.P3_CONDUCTA_GENERAL == null || Entity.P4_RELACION_COMPANEROS == null)
                            _Estatus = (short)eEstatudDetallePersonalidad.ASIGNADO;
                        else
                            _Estatus = (short)eEstatudDetallePersonalidad.TERMINADO;

                        if (_DetalleSegu != null)
                        {
                            var _DesarrolloEstudioSegur = Context.PERSONALIDAD_DETALLE.FirstOrDefault(c => c.ID_ESTUDIO == _DetalleSegu.ID_ESTUDIO && c.ID_TIPO == (short)eTiposEstudio.SEGURIDAD && c.ID_IMPUTADO == _DetalleSegu.ID_IMPUTADO && c.ID_INGRESO == _DetalleSegu.ID_INGRESO && c.ID_CENTRO == _DetalleSegu.ID_CENTRO && c.ID_ANIO == _DetalleSegu.ID_ANIO);
                            if (_DesarrolloEstudioSegur == null)
                            {
                                short _ConsecutivoPersonalidadDetalle = GetIDProceso<short>("PERSONALIDAD_DETALLE", "ID_DETALLE", "1=1");
                                var _NvoDetalle = new PERSONALIDAD_DETALLE()
                                {
                                    DIAS_BONIFICADOS = null,
                                    ID_ANIO = Entity.ID_ANIO,
                                    ID_CENTRO = Entity.ID_CENTRO,
                                    ID_DETALLE = _ConsecutivoPersonalidadDetalle,
                                    ID_ESTUDIO = _EstudioPadre.ID_ESTUDIO,
                                    ID_IMPUTADO = Entity.ID_IMPUTADO,
                                    ID_INGRESO = Entity.ID_INGRESO,
                                    ID_TIPO = (short)eTiposEstudio.SEGURIDAD,
                                    INICIO_FEC = GetFechaServerDate(),
                                    RESULTADO = Entity.P7_DICTAMEN == "S" ? "S" : Entity.P7_DICTAMEN == "D" ? "N" : string.Empty,
                                    SOLICITUD_FEC = _EstudioPadre.SOLICITUD_FEC,
                                    ID_ESTATUS = _Estatus,
                                    TERMINO_FEC = _Estatus == (short)eEstatudDetallePersonalidad.TERMINADO ? GetFechaServerDate() : new System.DateTime?(),
                                    TIPO_MEDIA = string.Empty
                                };

                                Context.PERSONALIDAD_DETALLE.Add(_NvoDetalle);
                            }

                            else
                            {
                                _DesarrolloEstudioSegur.RESULTADO = Entity.P7_DICTAMEN == "S" ? "S" : Entity.P7_DICTAMEN == "D" ? "N" : string.Empty;
                                _DesarrolloEstudioSegur.ID_ESTATUS = _Estatus;
                                Context.Entry(_DesarrolloEstudioSegur).State = System.Data.EntityState.Modified;
                            }
                        }
                    }
                    #endregion

                    Context.SaveChanges();
                    transaccion.Complete();
                    return 1;
                }
            }
            catch (System.Data.Entity.Validation.DbEntityValidationException dbEx)
            {
                foreach (var validationErrors in dbEx.EntityValidationErrors)
                    foreach (var validationError in validationErrors.ValidationErrors)
                        System.Diagnostics.Trace.TraceInformation("Nombre del causante de excepción: {0} Error: {1}", validationError.PropertyName, validationError.ErrorMessage);

                return 2;
            }

            catch (System.Exception exc)
            {
                return 0;
            }
        }

        public short GuardaMedicoFederalAislado(PFF_ESTUDIO_MEDICO Entity)
        {
            try
            {
                using (System.Transactions.TransactionScope transaccion = new System.Transactions.TransactionScope(System.Transactions.TransactionScopeOption.RequiresNew, new System.Transactions.TransactionOptions() { IsolationLevel = System.Transactions.IsolationLevel.ReadCommitted }))
                {
                    var _EstudioPadre = Context.PERSONALIDAD.Where(x => x.ID_INGRESO == Entity.ID_INGRESO && x.ID_IMPUTADO == Entity.ID_IMPUTADO && x.ID_CENTRO == Entity.ID_CENTRO && x.ID_ANIO == Entity.ID_ANIO).OrderByDescending(x => x.ID_ESTUDIO).FirstOrDefault();

                    if (_EstudioPadre == null)
                        return 0;

                    var _Actual = Context.PERSONALIDAD_FUERO_FEDERAL.FirstOrDefault(x => x.ID_ANIO == _EstudioPadre.ID_ANIO && x.ID_CENTRO == _EstudioPadre.ID_CENTRO && x.ID_IMPUTADO == _EstudioPadre.ID_IMPUTADO && x.ID_INGRESO == _EstudioPadre.ID_INGRESO && x.ID_ESTUDIO == _EstudioPadre.ID_ESTUDIO);

                    if (_Actual == null)
                    {
                        var _EstudioMedico = Context.PFF_ESTUDIO_MEDICO.FirstOrDefault(x => x.ID_IMPUTADO == _EstudioPadre.ID_IMPUTADO && x.ID_INGRESO == _EstudioPadre.ID_INGRESO && x.ID_ESTUDIO == _EstudioPadre.ID_ESTUDIO && x.ID_CENTRO == _EstudioPadre.ID_CENTRO);
                        var personalidadFederal = new PERSONALIDAD_FUERO_FEDERAL()
                        {
                            ID_ANIO = _EstudioPadre.ID_ANIO,
                            ID_IMPUTADO = _EstudioPadre.ID_IMPUTADO,
                            ID_CENTRO = _EstudioPadre.ID_CENTRO,
                            ID_ESTUDIO = _EstudioPadre.ID_ESTUDIO,
                            ID_INGRESO = _EstudioPadre.ID_INGRESO
                        };

                        var _MedicoFederal = new PFF_ESTUDIO_MEDICO()
                        {
                            ALIAS = Entity.ALIAS,
                            ANTE_HEREDO_FAM = Entity.ANTE_HEREDO_FAM,
                            ANTE_PATOLOGICOS = Entity.ANTE_PATOLOGICOS,
                            ANTE_PERSONAL_NO_PATOLOGICOS = Entity.ANTE_PERSONAL_NO_PATOLOGICOS,
                            ASIST_AA = Entity.ASIST_AA,
                            ASIST_FARMACODEPENDENCIA = Entity.ASIST_FARMACODEPENDENCIA,
                            ASIST_OTROS = Entity.ASIST_OTROS,
                            ASIST_OTROS_ESPECIF = Entity.ASIST_OTROS_ESPECIF,
                            CONCLUSION = Entity.CONCLUSION,
                            DELITO = Entity.DELITO,
                            DIAGNOSTICO = Entity.DIAGNOSTICO,
                            DIRECTOR_CENTRO = Entity.DIRECTOR_CENTRO,
                            EDAD = Entity.EDAD,
                            EDO_CIVIL = Entity.EDO_CIVIL != null ? Entity.EDO_CIVIL != "-1" ? Entity.EDO_CIVIL : string.Empty : string.Empty,
                            ESTATURA = Entity.ESTATURA,
                            EXP_FIS_ABDOMEN = Entity.EXP_FIS_ABDOMEN,
                            EXP_FIS_CABEZA_CUELLO = Entity.EXP_FIS_CABEZA_CUELLO,
                            EXP_FIS_EXTREMIDADES = Entity.EXP_FIS_EXTREMIDADES,
                            EXP_FIS_GENITALES = Entity.EXP_FIS_GENITALES,
                            EXP_FIS_TORAX = Entity.EXP_FIS_TORAX,
                            FECHA = Entity.FECHA,
                            ID_ANIO = _EstudioPadre.ID_ANIO,
                            ID_CENTRO = _EstudioPadre.ID_CENTRO,
                            ID_ESTUDIO = _EstudioPadre.ID_ESTUDIO,
                            ID_IMPUTADO = _EstudioPadre.ID_IMPUTADO,
                            TATUAJES = Entity.TATUAJES,
                            ID_INGRESO = _EstudioPadre.ID_INGRESO,
                            INTERROGATORIO_APARATOS = Entity.INTERROGATORIO_APARATOS,
                            LUGAR = Entity.LUGAR,
                            MEDICO = Entity.MEDICO,
                            NOMBRE = Entity.NOMBRE,
                            OCUPACION_ACT = Entity.OCUPACION_ACT != -1 ? Entity.OCUPACION_ACT : null,
                            OCUPACION_ANT = Entity.OCUPACION_ANT != -1 ? Entity.OCUPACION_ANT : null,
                            ORIGINARIO_DE = Entity.ORIGINARIO_DE,
                            PADECIMIENTO_ACTUAL = Entity.PADECIMIENTO_ACTUAL,
                            PULSO = Entity.PULSO,
                            RESPIRACION = Entity.RESPIRACION,
                            RESULTADOS_OBTENIDOS = Entity.RESULTADOS_OBTENIDOS,
                            SENTENCIA = Entity.SENTENCIA,
                            TA = Entity.TA,
                            TEMPERATURA = Entity.TEMPERATURA
                        };

                        if (Entity.PFF_SUSTANCIA_TOXICA != null && Entity.PFF_SUSTANCIA_TOXICA.Any())
                        {
                            foreach (var item in Entity.PFF_SUSTANCIA_TOXICA)
                            {
                                var _NuevaToxica = new PFF_SUSTANCIA_TOXICA()
                                {
                                    CANTIDAD = item.CANTIDAD,
                                    EDAD_INICIO = item.EDAD_INICIO,
                                    ID_ANIO = _EstudioPadre.ID_ANIO,
                                    ID_CENTRO = _EstudioPadre.ID_CENTRO,
                                    ID_DROGA = item.ID_DROGA,
                                    ID_ESTUDIO = _EstudioPadre.ID_ESTUDIO,
                                    ID_IMPUTADO = _EstudioPadre.ID_IMPUTADO,
                                    ID_INGRESO = _EstudioPadre.ID_INGRESO,
                                    PERIODICIDAD = item.PERIODICIDAD
                                };

                                Context.PFF_SUSTANCIA_TOXICA.Add(_NuevaToxica);
                            }
                        }

                        Context.PERSONALIDAD_FUERO_FEDERAL.Add(personalidadFederal);
                        Context.PFF_ESTUDIO_MEDICO.Add(_MedicoFederal);
                    }
                    else
                    {
                        var _MedFed = _Actual.PFF_ESTUDIO_MEDICO;
                        if (_MedFed == null)
                        {
                            _MedFed = new PFF_ESTUDIO_MEDICO()
                            {
                                ALIAS = Entity.ALIAS,
                                ANTE_HEREDO_FAM = Entity.ANTE_HEREDO_FAM,
                                ANTE_PATOLOGICOS = Entity.ANTE_PATOLOGICOS,
                                ANTE_PERSONAL_NO_PATOLOGICOS = Entity.ANTE_PERSONAL_NO_PATOLOGICOS,
                                ASIST_AA = Entity.ASIST_AA,
                                ASIST_FARMACODEPENDENCIA = Entity.ASIST_FARMACODEPENDENCIA,
                                ASIST_OTROS = Entity.ASIST_OTROS,
                                ASIST_OTROS_ESPECIF = Entity.ASIST_OTROS_ESPECIF,
                                CONCLUSION = Entity.CONCLUSION,
                                DELITO = Entity.DELITO,
                                DIAGNOSTICO = Entity.DIAGNOSTICO,
                                DIRECTOR_CENTRO = Entity.DIRECTOR_CENTRO,
                                EDAD = Entity.EDAD,
                                EDO_CIVIL = Entity.EDO_CIVIL != null ? Entity.EDO_CIVIL != "-1" ? Entity.EDO_CIVIL : string.Empty : string.Empty,
                                ESTATURA = Entity.ESTATURA,
                                EXP_FIS_ABDOMEN = Entity.EXP_FIS_ABDOMEN,
                                EXP_FIS_CABEZA_CUELLO = Entity.EXP_FIS_CABEZA_CUELLO,
                                EXP_FIS_EXTREMIDADES = Entity.EXP_FIS_EXTREMIDADES,
                                EXP_FIS_GENITALES = Entity.EXP_FIS_GENITALES,
                                EXP_FIS_TORAX = Entity.EXP_FIS_TORAX,
                                TATUAJES = Entity.TATUAJES,
                                FECHA = Entity.FECHA,
                                ID_ANIO = _EstudioPadre.ID_ANIO,
                                ID_CENTRO = _EstudioPadre.ID_CENTRO,
                                ID_ESTUDIO = _EstudioPadre.ID_ESTUDIO,
                                ID_IMPUTADO = _EstudioPadre.ID_IMPUTADO,
                                ID_INGRESO = _EstudioPadre.ID_INGRESO,
                                INTERROGATORIO_APARATOS = Entity.INTERROGATORIO_APARATOS,
                                LUGAR = Entity.LUGAR,
                                MEDICO = Entity.MEDICO,
                                NOMBRE = Entity.NOMBRE,
                                OCUPACION_ACT = Entity.OCUPACION_ACT != -1 ? Entity.OCUPACION_ACT : null,
                                OCUPACION_ANT = Entity.OCUPACION_ANT != -1 ? Entity.OCUPACION_ANT : null,
                                ORIGINARIO_DE = Entity.ORIGINARIO_DE,
                                PADECIMIENTO_ACTUAL = Entity.PADECIMIENTO_ACTUAL,
                                PULSO = Entity.PULSO,
                                RESPIRACION = Entity.RESPIRACION,
                                RESULTADOS_OBTENIDOS = Entity.RESULTADOS_OBTENIDOS,
                                SENTENCIA = Entity.SENTENCIA,
                                TA = Entity.TA,
                                TEMPERATURA = Entity.TEMPERATURA
                            };

                            if (Entity.PFF_SUSTANCIA_TOXICA != null && Entity.PFF_SUSTANCIA_TOXICA.Any())
                            {
                                foreach (var item in Entity.PFF_SUSTANCIA_TOXICA)
                                {
                                    var _NuevaToxica = new PFF_SUSTANCIA_TOXICA()
                                    {
                                        CANTIDAD = item.CANTIDAD,
                                        EDAD_INICIO = item.EDAD_INICIO,
                                        ID_ANIO = _EstudioPadre.ID_ANIO,
                                        ID_CENTRO = _EstudioPadre.ID_CENTRO,
                                        ID_DROGA = item.ID_DROGA,
                                        ID_ESTUDIO = _EstudioPadre.ID_ESTUDIO,
                                        ID_IMPUTADO = _EstudioPadre.ID_IMPUTADO,
                                        ID_INGRESO = _EstudioPadre.ID_INGRESO,
                                        PERIODICIDAD = item.PERIODICIDAD
                                    };

                                    Context.PFF_SUSTANCIA_TOXICA.Add(_NuevaToxica);
                                }
                            };

                            Context.PFF_ESTUDIO_MEDICO.Add(_MedFed);
                        }
                        else
                        {
                            var _Toxicos = Context.PFF_SUSTANCIA_TOXICA.Where(c => c.ID_INGRESO == _EstudioPadre.ID_INGRESO && c.ID_IMPUTADO == _EstudioPadre.ID_IMPUTADO && c.ID_ESTUDIO == _EstudioPadre.ID_ESTUDIO && c.ID_CENTRO == _EstudioPadre.ID_CENTRO);
                            if (_Toxicos.Any())
                                foreach (var item in _Toxicos)
                                    Context.Entry(item).State = System.Data.EntityState.Deleted;

                            _MedFed.ALIAS = Entity.ALIAS;
                            _MedFed.ANTE_HEREDO_FAM = Entity.ANTE_HEREDO_FAM;
                            _MedFed.ANTE_PATOLOGICOS = Entity.ANTE_PATOLOGICOS;
                            _MedFed.ANTE_PERSONAL_NO_PATOLOGICOS = Entity.ANTE_PERSONAL_NO_PATOLOGICOS;
                            _MedFed.ASIST_AA = Entity.ASIST_AA;
                            _MedFed.ASIST_FARMACODEPENDENCIA = Entity.ASIST_FARMACODEPENDENCIA;
                            _MedFed.ASIST_OTROS = Entity.ASIST_OTROS;
                            _MedFed.ASIST_OTROS_ESPECIF = Entity.ASIST_OTROS_ESPECIF;
                            _MedFed.CONCLUSION = Entity.CONCLUSION;
                            _MedFed.DELITO = Entity.DELITO;
                            _MedFed.DIAGNOSTICO = Entity.DIAGNOSTICO;
                            _MedFed.DIRECTOR_CENTRO = Entity.DIRECTOR_CENTRO;
                            _MedFed.EDAD = Entity.EDAD;
                            _MedFed.EDO_CIVIL = Entity.EDO_CIVIL != null ? Entity.EDO_CIVIL != "-1" ? Entity.EDO_CIVIL : string.Empty : string.Empty;
                            _MedFed.ESTATURA = Entity.ESTATURA;
                            _MedFed.EXP_FIS_ABDOMEN = Entity.EXP_FIS_ABDOMEN;
                            _MedFed.EXP_FIS_CABEZA_CUELLO = Entity.EXP_FIS_CABEZA_CUELLO;
                            _MedFed.EXP_FIS_EXTREMIDADES = Entity.EXP_FIS_EXTREMIDADES;
                            _MedFed.EXP_FIS_GENITALES = Entity.EXP_FIS_GENITALES;
                            _MedFed.EXP_FIS_TORAX = Entity.EXP_FIS_TORAX;
                            _MedFed.TATUAJES = Entity.TATUAJES;
                            _MedFed.FECHA = Entity.FECHA;
                            _MedFed.ID_ANIO = _EstudioPadre.ID_ANIO;
                            _MedFed.ID_CENTRO = _EstudioPadre.ID_CENTRO;
                            _MedFed.ID_ESTUDIO = _EstudioPadre.ID_ESTUDIO;
                            _MedFed.ID_IMPUTADO = _EstudioPadre.ID_IMPUTADO;
                            _MedFed.ID_INGRESO = _EstudioPadre.ID_INGRESO;
                            _MedFed.INTERROGATORIO_APARATOS = Entity.INTERROGATORIO_APARATOS;
                            _MedFed.LUGAR = Entity.LUGAR;
                            _MedFed.MEDICO = Entity.MEDICO;
                            _MedFed.NOMBRE = Entity.NOMBRE;
                            _MedFed.OCUPACION_ACT = Entity.OCUPACION_ACT != -1 ? Entity.OCUPACION_ACT : null;
                            _MedFed.OCUPACION_ANT = Entity.OCUPACION_ANT != -1 ? Entity.OCUPACION_ANT : null;
                            _MedFed.ORIGINARIO_DE = Entity.ORIGINARIO_DE;
                            _MedFed.PADECIMIENTO_ACTUAL = Entity.PADECIMIENTO_ACTUAL;
                            _MedFed.PULSO = Entity.PULSO;
                            _MedFed.RESPIRACION = Entity.RESPIRACION;
                            _MedFed.RESULTADOS_OBTENIDOS = Entity.RESULTADOS_OBTENIDOS;
                            _MedFed.SENTENCIA = Entity.SENTENCIA;
                            _MedFed.TA = Entity.TA;
                            _MedFed.TEMPERATURA = Entity.TEMPERATURA;
                            Context.Entry(_MedFed).State = System.Data.EntityState.Modified;

                            if (Entity.PFF_SUSTANCIA_TOXICA != null && Entity.PFF_SUSTANCIA_TOXICA.Any())
                                foreach (var item in Entity.PFF_SUSTANCIA_TOXICA)
                                {
                                    var _NuevaToxica = new PFF_SUSTANCIA_TOXICA()
                                    {
                                        CANTIDAD = item.CANTIDAD,
                                        EDAD_INICIO = item.EDAD_INICIO,
                                        ID_ANIO = _EstudioPadre.ID_ANIO,
                                        ID_CENTRO = _EstudioPadre.ID_CENTRO,
                                        ID_DROGA = item.ID_DROGA,
                                        ID_ESTUDIO = _EstudioPadre.ID_ESTUDIO,
                                        ID_IMPUTADO = _EstudioPadre.ID_IMPUTADO,
                                        ID_INGRESO = _EstudioPadre.ID_INGRESO,
                                        PERIODICIDAD = item.PERIODICIDAD
                                    };

                                    _MedFed.PFF_SUSTANCIA_TOXICA.Add(_NuevaToxica);
                                };
                        }
                    }

                    #region DEFINICION DE DETALLE MEDICO FEDERAL

                    if (_EstudioPadre != null)
                    {
                        short? _Estatus = new short?();
                        if (string.IsNullOrEmpty(Entity.ALIAS) || string.IsNullOrEmpty(Entity.ANTE_HEREDO_FAM) || string.IsNullOrEmpty(Entity.ANTE_PATOLOGICOS) ||
                            string.IsNullOrEmpty(Entity.ANTE_PERSONAL_NO_PATOLOGICOS) || string.IsNullOrEmpty(Entity.CONCLUSION) || string.IsNullOrEmpty(Entity.DELITO) ||
                            string.IsNullOrEmpty(Entity.DIAGNOSTICO) || string.IsNullOrEmpty(Entity.EDO_CIVIL) || string.IsNullOrEmpty(Entity.ESTATURA) ||
                            string.IsNullOrEmpty(Entity.EXP_FIS_ABDOMEN) || string.IsNullOrEmpty(Entity.EXP_FIS_CABEZA_CUELLO) || string.IsNullOrEmpty(Entity.EXP_FIS_EXTREMIDADES) ||
                            string.IsNullOrEmpty(Entity.EXP_FIS_GENITALES) || string.IsNullOrEmpty(Entity.EXP_FIS_TORAX) || string.IsNullOrEmpty(Entity.INTERROGATORIO_APARATOS) ||
                            string.IsNullOrEmpty(Entity.NOMBRE) || Entity.OCUPACION_ACT != -1 || Entity.OCUPACION_ANT == null ||
                            string.IsNullOrEmpty(Entity.ORIGINARIO_DE) || string.IsNullOrEmpty(Entity.PADECIMIENTO_ACTUAL) || string.IsNullOrEmpty(Entity.PULSO) ||
                            string.IsNullOrEmpty(Entity.RESPIRACION) || string.IsNullOrEmpty(Entity.RESULTADOS_OBTENIDOS) || string.IsNullOrEmpty(Entity.SENTENCIA) || string.IsNullOrEmpty(Entity.TA) ||
                            string.IsNullOrEmpty(Entity.TATUAJES) || string.IsNullOrEmpty(Entity.TEMPERATURA) || !Entity.EDAD.HasValue || !Entity.FECHA.HasValue)
                            _Estatus = (short)eEstatudDetallePersonalidad.ASIGNADO;

                        else
                            _Estatus = (short)eEstatudDetallePersonalidad.TERMINADO;


                        var DesarrolloMedicoFederal = Context.PERSONALIDAD_DETALLE.FirstOrDefault(x => x.ID_ESTUDIO == _EstudioPadre.ID_ESTUDIO && x.ID_TIPO == (short)eTiposEstudio.MEDICA_FEDERAL && x.ID_INGRESO == _EstudioPadre.ID_INGRESO && x.ID_IMPUTADO == _EstudioPadre.ID_IMPUTADO && x.ID_ANIO == _EstudioPadre.ID_ANIO && x.ID_CENTRO == _EstudioPadre.ID_CENTRO);
                        if (DesarrolloMedicoFederal == null)
                        {
                            ///METODO COMPLEMENTARIO QUE CREA UN DETALLE (NO ES PROBABLE QUE SEA NECESARIO, PERO EN CASOS EXTRAORDINARIOS SI SE USA <usualmente con informacion vieja>)
                            var NvoDetalle = new PERSONALIDAD_DETALLE()
                            {
                                ID_ANIO = _EstudioPadre.ID_ANIO,
                                ID_CENTRO = _EstudioPadre.ID_CENTRO,
                                ID_DETALLE = GetSequence<short>("PERSONALIDAD_DETALLE_SEQ"),
                                ID_ESTUDIO = _EstudioPadre.ID_ESTUDIO,
                                ID_IMPUTADO = _EstudioPadre.ID_IMPUTADO,
                                ID_ESTATUS = _Estatus,
                                ID_INGRESO = _EstudioPadre.ID_INGRESO,
                                ID_TIPO = (short)eTiposEstudio.MEDICA_FEDERAL,
                                INICIO_FEC = GetFechaServerDate(),
                                RESULTADO = _Estatus.HasValue ? _Estatus.Value == (short)eEstatudDetallePersonalidad.TERMINADO ? "S" : "N" : "N",
                                SOLICITUD_FEC = null,
                                TERMINO_FEC = _Estatus.HasValue ? _Estatus.Value == (short)eEstatudDetallePersonalidad.TERMINADO ? GetFechaServerDate() : new System.DateTime?() : new System.DateTime?(),
                                TIPO_MEDIA = string.Empty
                            };

                            Context.PERSONALIDAD_DETALLE.Add(NvoDetalle);
                        }

                        else
                        {
                            DesarrolloMedicoFederal.RESULTADO = _Estatus.HasValue ? _Estatus.Value == (short)eEstatudDetallePersonalidad.TERMINADO ? "S" : "N" : "N";
                            DesarrolloMedicoFederal.ID_ESTATUS = _Estatus;
                            Context.Entry(DesarrolloMedicoFederal).State = System.Data.EntityState.Modified;
                        }
                    };

                    #endregion


                    Context.SaveChanges();
                    transaccion.Complete();
                    return 1;
                }
            }
            catch (System.Data.Entity.Validation.DbEntityValidationException dbEx)
            {
                foreach (var validationErrors in dbEx.EntityValidationErrors)
                    foreach (var validationError in validationErrors.ValidationErrors)
                        System.Diagnostics.Trace.TraceInformation("Nombre del causante de excepción: {0} Error: {1}", validationError.PropertyName, validationError.ErrorMessage);

                return 2;
            }

            catch (System.Exception exc)
            {
                return 0;
            }
        }

        public short GuardaPsicologicoFederalAislado(PFF_ESTUDIO_PSICOLOGICO Entity)
        {
            try
            {
                using (System.Transactions.TransactionScope transaccion = new System.Transactions.TransactionScope(System.Transactions.TransactionScopeOption.RequiresNew, new System.Transactions.TransactionOptions() { IsolationLevel = System.Transactions.IsolationLevel.ReadCommitted }))
                {
                    var _EstudioPadre = Context.PERSONALIDAD.Where(x => x.ID_INGRESO == Entity.ID_INGRESO && x.ID_IMPUTADO == Entity.ID_IMPUTADO && x.ID_CENTRO == Entity.ID_CENTRO && x.ID_ANIO == Entity.ID_ANIO).OrderByDescending(x => x.ID_ESTUDIO).FirstOrDefault();

                    if (_EstudioPadre == null)
                        return 0;

                    var _Actual = Context.PERSONALIDAD_FUERO_FEDERAL.FirstOrDefault(x => x.ID_ANIO == _EstudioPadre.ID_ANIO && x.ID_CENTRO == _EstudioPadre.ID_CENTRO && x.ID_IMPUTADO == _EstudioPadre.ID_IMPUTADO && x.ID_INGRESO == _EstudioPadre.ID_INGRESO && x.ID_ESTUDIO == _EstudioPadre.ID_ESTUDIO);

                    if (_Actual == null)
                    {
                        var _EstudioPsicologico = Context.PFF_ESTUDIO_PSICOLOGICO.FirstOrDefault(x => x.ID_IMPUTADO == _EstudioPadre.ID_IMPUTADO && x.ID_INGRESO == _EstudioPadre.ID_INGRESO && x.ID_ESTUDIO == _EstudioPadre.ID_ESTUDIO && x.ID_CENTRO == _EstudioPadre.ID_CENTRO);
                        var personalidadFederal = new PERSONALIDAD_FUERO_FEDERAL()
                        {
                            ID_ANIO = _EstudioPadre.ID_ANIO,
                            ID_IMPUTADO = _EstudioPadre.ID_IMPUTADO,
                            ID_CENTRO = _EstudioPadre.ID_CENTRO,
                            ID_ESTUDIO = _EstudioPadre.ID_ESTUDIO,
                            ID_INGRESO = _EstudioPadre.ID_INGRESO
                        };

                        var _PsicoFede = new PFF_ESTUDIO_PSICOLOGICO()
                        {
                            ACTITUD = Entity.ACTITUD,
                            CI = Entity.CI,
                            DELITO = Entity.DELITO,
                            DINAM_PERSON_ACTUAL = Entity.DINAM_PERSON_ACTUAL,
                            DINAM_PERSON_INGRESO = Entity.DINAM_PERSON_INGRESO,
                            EDAD = Entity.EDAD,
                            DIRECTOR_DENTRO = Entity.DIRECTOR_DENTRO,
                            ESPECIFIQUE = Entity.ESPECIFIQUE,
                            EXAMEN_MENTAL = Entity.EXAMEN_MENTAL,
                            EXTERNO = Entity.EXTERNO,
                            FECHA = Entity.FECHA,
                            ID_ANIO = _EstudioPadre.ID_ANIO,
                            ID_CENTRO = _EstudioPadre.ID_CENTRO,
                            ID_ESTUDIO = _EstudioPadre.ID_ESTUDIO,
                            ID_IMPUTADO = _EstudioPadre.ID_IMPUTADO,
                            ID_INGRESO = _EstudioPadre.ID_INGRESO,
                            INDICE_LESION_ORGANICA = Entity.INDICE_LESION_ORGANICA,
                            INTERNO = Entity.INTERNO,
                            LUGAR = Entity.LUGAR,
                            NIVEL_INTELECTUAL = Entity.NIVEL_INTELECTUAL,
                            NOMBRE = Entity.NOMBRE,
                            OPINION = Entity.OPINION,
                            PRONOSTICO_REINTEGRACION = Entity.PRONOSTICO_REINTEGRACION,
                            PRUEBAS_APLICADAS = Entity.PRUEBAS_APLICADAS,
                            PSICOLOGO = Entity.PSICOLOGO,
                            REQ_CONT_TRATAMIENTO = Entity.REQ_CONT_TRATAMIENTO,
                            RESULT_TRATAMIENTO = Entity.RESULT_TRATAMIENTO,
                            SOBRENOMBRE = Entity.SOBRENOMBRE
                        };

                        Context.PERSONALIDAD_FUERO_FEDERAL.Add(personalidadFederal);
                        Context.PFF_ESTUDIO_PSICOLOGICO.Add(_PsicoFede);
                    }
                    else
                    {
                        var _Psic = _Actual.PFF_ESTUDIO_PSICOLOGICO;
                        if (_Psic == null)
                        {
                            _Psic = new PFF_ESTUDIO_PSICOLOGICO()
                            {
                                ACTITUD = Entity.ACTITUD,
                                CI = Entity.CI,
                                DELITO = Entity.DELITO,
                                DINAM_PERSON_ACTUAL = Entity.DINAM_PERSON_ACTUAL,
                                DINAM_PERSON_INGRESO = Entity.DINAM_PERSON_INGRESO,
                                EDAD = Entity.EDAD,
                                DIRECTOR_DENTRO = Entity.DIRECTOR_DENTRO,
                                ESPECIFIQUE = Entity.ESPECIFIQUE,
                                EXAMEN_MENTAL = Entity.EXAMEN_MENTAL,
                                EXTERNO = Entity.EXTERNO,
                                FECHA = Entity.FECHA,
                                ID_ANIO = _EstudioPadre.ID_ANIO,
                                ID_CENTRO = _EstudioPadre.ID_CENTRO,
                                ID_ESTUDIO = _EstudioPadre.ID_ESTUDIO,
                                ID_IMPUTADO = _EstudioPadre.ID_IMPUTADO,
                                ID_INGRESO = _EstudioPadre.ID_INGRESO,
                                INDICE_LESION_ORGANICA = Entity.INDICE_LESION_ORGANICA,
                                INTERNO = Entity.INTERNO,
                                LUGAR = Entity.LUGAR,
                                NIVEL_INTELECTUAL = Entity.NIVEL_INTELECTUAL,
                                NOMBRE = Entity.NOMBRE,
                                OPINION = Entity.OPINION,
                                PRONOSTICO_REINTEGRACION = Entity.PRONOSTICO_REINTEGRACION,
                                PRUEBAS_APLICADAS = Entity.PRUEBAS_APLICADAS,
                                PSICOLOGO = Entity.PSICOLOGO,
                                REQ_CONT_TRATAMIENTO = Entity.REQ_CONT_TRATAMIENTO,
                                RESULT_TRATAMIENTO = Entity.RESULT_TRATAMIENTO,
                                SOBRENOMBRE = Entity.SOBRENOMBRE
                            };

                            Context.PFF_ESTUDIO_PSICOLOGICO.Add(_Psic);
                        }
                        else
                        {
                            _Psic.ACTITUD = Entity.ACTITUD;
                            _Psic.CI = Entity.CI;
                            _Psic.DELITO = Entity.DELITO;
                            _Psic.DINAM_PERSON_ACTUAL = Entity.DINAM_PERSON_ACTUAL;
                            _Psic.DINAM_PERSON_INGRESO = Entity.DINAM_PERSON_INGRESO;
                            _Psic.EDAD = Entity.EDAD;
                            _Psic.DIRECTOR_DENTRO = Entity.DIRECTOR_DENTRO;
                            _Psic.ESPECIFIQUE = Entity.ESPECIFIQUE;
                            _Psic.EXAMEN_MENTAL = Entity.EXAMEN_MENTAL;
                            _Psic.EXTERNO = Entity.EXTERNO;
                            _Psic.FECHA = Entity.FECHA;
                            _Psic.ID_ANIO = _EstudioPadre.ID_ANIO;
                            _Psic.ID_CENTRO = _EstudioPadre.ID_CENTRO;
                            _Psic.ID_ESTUDIO = _EstudioPadre.ID_ESTUDIO;
                            _Psic.ID_IMPUTADO = _EstudioPadre.ID_IMPUTADO;
                            _Psic.ID_INGRESO = _EstudioPadre.ID_INGRESO;
                            _Psic.INDICE_LESION_ORGANICA = Entity.INDICE_LESION_ORGANICA;
                            _Psic.INTERNO = Entity.INTERNO;
                            _Psic.LUGAR = Entity.LUGAR;
                            _Psic.NIVEL_INTELECTUAL = Entity.NIVEL_INTELECTUAL;
                            _Psic.NOMBRE = Entity.NOMBRE;
                            _Psic.OPINION = Entity.OPINION;
                            _Psic.PRONOSTICO_REINTEGRACION = Entity.PRONOSTICO_REINTEGRACION;
                            _Psic.PRUEBAS_APLICADAS = Entity.PRUEBAS_APLICADAS;
                            _Psic.PSICOLOGO = Entity.PSICOLOGO;
                            _Psic.REQ_CONT_TRATAMIENTO = Entity.REQ_CONT_TRATAMIENTO;
                            _Psic.RESULT_TRATAMIENTO = Entity.RESULT_TRATAMIENTO;
                            _Psic.SOBRENOMBRE = Entity.SOBRENOMBRE;
                            Context.Entry(_Psic).State = System.Data.EntityState.Modified;
                        }
                    }

                    //INICIA DETALLE PSICOLOGICO FEDERAL
                    #region DEFINIION DE DETALLE PSICOLOGICO FEDRAL
                    if (_EstudioPadre != null)
                    {
                        short? _Estatus = new short?();
                        if (string.IsNullOrEmpty(Entity.ACTITUD) || string.IsNullOrEmpty(Entity.CI) || string.IsNullOrEmpty(Entity.DELITO) || string.IsNullOrEmpty(Entity.DINAM_PERSON_ACTUAL)
                            || string.IsNullOrEmpty(Entity.DINAM_PERSON_INGRESO) || string.IsNullOrEmpty(Entity.EXAMEN_MENTAL) || string.IsNullOrEmpty(Entity.EXTERNO) || string.IsNullOrEmpty(Entity.INDICE_LESION_ORGANICA)
                            || string.IsNullOrEmpty(Entity.INTERNO) || string.IsNullOrEmpty(Entity.NIVEL_INTELECTUAL) || string.IsNullOrEmpty(Entity.NOMBRE) || string.IsNullOrEmpty(Entity.OPINION) || string.IsNullOrEmpty(Entity.PRONOSTICO_REINTEGRACION)
                            || string.IsNullOrEmpty(Entity.PRUEBAS_APLICADAS) || string.IsNullOrEmpty(Entity.REQ_CONT_TRATAMIENTO) || string.IsNullOrEmpty(Entity.RESULT_TRATAMIENTO) || string.IsNullOrEmpty(Entity.SOBRENOMBRE) || !Entity.EDAD.HasValue ||
                            !Entity.FECHA.HasValue)
                            _Estatus = (short)eEstatudDetallePersonalidad.ASIGNADO;
                        else
                            _Estatus = (short)eEstatudDetallePersonalidad.TERMINADO;

                        var DesarrolloPsicoFederal = Context.PERSONALIDAD_DETALLE.FirstOrDefault(x => x.ID_IMPUTADO == _EstudioPadre.ID_IMPUTADO && x.ID_ESTUDIO == _EstudioPadre.ID_ESTUDIO && x.ID_INGRESO == _EstudioPadre.ID_INGRESO && x.ID_TIPO == (short)eTiposEstudio.PSICOLOGICA_FEDERAL && x.ID_CENTRO == _EstudioPadre.ID_CENTRO && x.ID_ANIO == _EstudioPadre.ID_ANIO);
                        if (DesarrolloPsicoFederal == null)
                        {
                            var NvoDetalle = new PERSONALIDAD_DETALLE()
                            {
                                ID_ANIO = _EstudioPadre.ID_ANIO,
                                ID_CENTRO = _EstudioPadre.ID_CENTRO,
                                ID_DETALLE = GetSequence<short>("PERSONALIDAD_DETALLE_SEQ"),
                                ID_ESTUDIO = _EstudioPadre.ID_ESTUDIO,
                                ID_IMPUTADO = _EstudioPadre.ID_IMPUTADO,
                                ID_INGRESO = _EstudioPadre.ID_INGRESO,
                                ID_ESTATUS = _Estatus,
                                ID_TIPO = (short)eTiposEstudio.PSICOLOGICA_FEDERAL,
                                INICIO_FEC = GetFechaServerDate(),
                                RESULTADO = _Estatus == (short)eEstatudDetallePersonalidad.TERMINADO ? "S" : "N",
                                SOLICITUD_FEC = null,
                                TERMINO_FEC = _Estatus == (short)eEstatudDetallePersonalidad.TERMINADO ? GetFechaServerDate() : new System.DateTime?(),
                                TIPO_MEDIA = string.Empty
                            };

                            Context.PERSONALIDAD_DETALLE.Add(NvoDetalle);
                        }

                        else
                        {
                            DesarrolloPsicoFederal.RESULTADO = _Estatus == (short)eEstatudDetallePersonalidad.TERMINADO ? "S" : "N";
                            DesarrolloPsicoFederal.ID_ESTATUS = _Estatus;
                            Context.Entry(DesarrolloPsicoFederal).State = System.Data.EntityState.Modified;
                        };
                    };
                    #endregion

                    Context.SaveChanges();
                    transaccion.Complete();
                    return 1;
                }
            }
            catch (System.Data.Entity.Validation.DbEntityValidationException dbEx)
            {
                foreach (var validationErrors in dbEx.EntityValidationErrors)
                    foreach (var validationError in validationErrors.ValidationErrors)
                        System.Diagnostics.Trace.TraceInformation("Nombre del causante de excepción: {0} Error: {1}", validationError.PropertyName, validationError.ErrorMessage);

                return 2;
            }

            catch (System.Exception exc)
            {
                return 0;
            }
        }

        public short GuardarEstudioTrabajoSocialAislado(PFF_TRABAJO_SOCIAL Entity)
        {
            try
            {
                using (System.Transactions.TransactionScope transaccion = new System.Transactions.TransactionScope(System.Transactions.TransactionScopeOption.RequiresNew, new System.Transactions.TransactionOptions() { IsolationLevel = System.Transactions.IsolationLevel.ReadCommitted }))
                {
                    var _EstudioPadre = Context.PERSONALIDAD.Where(x => x.ID_INGRESO == Entity.ID_INGRESO && x.ID_IMPUTADO == Entity.ID_IMPUTADO && x.ID_CENTRO == Entity.ID_CENTRO && x.ID_ANIO == Entity.ID_ANIO).OrderByDescending(x => x.ID_ESTUDIO).FirstOrDefault();

                    if (_EstudioPadre == null)
                        return 0;

                    var _Actual = Context.PERSONALIDAD_FUERO_FEDERAL.FirstOrDefault(x => x.ID_ANIO == _EstudioPadre.ID_ANIO && x.ID_CENTRO == _EstudioPadre.ID_CENTRO && x.ID_IMPUTADO == _EstudioPadre.ID_IMPUTADO && x.ID_INGRESO == _EstudioPadre.ID_INGRESO && x.ID_ESTUDIO == _EstudioPadre.ID_ESTUDIO);

                    if (_Actual == null)
                    {
                        var _EstudioTS = Context.PFF_TRABAJO_SOCIAL.FirstOrDefault(x => x.ID_IMPUTADO == _EstudioPadre.ID_IMPUTADO && x.ID_INGRESO == _EstudioPadre.ID_INGRESO && x.ID_ESTUDIO == _EstudioPadre.ID_ESTUDIO && x.ID_CENTRO == _EstudioPadre.ID_CENTRO);
                        var personalidadFederal = new PERSONALIDAD_FUERO_FEDERAL()
                        {
                            ID_ANIO = _EstudioPadre.ID_ANIO,
                            ID_IMPUTADO = _EstudioPadre.ID_IMPUTADO,
                            ID_CENTRO = _EstudioPadre.ID_CENTRO,
                            ID_ESTUDIO = _EstudioPadre.ID_ESTUDIO,
                            ID_INGRESO = _EstudioPadre.ID_INGRESO
                        };

                        var _SocialFamiliar = new PFF_TRABAJO_SOCIAL()
                        {
                            ALIMENTACION_FAM = Entity.ALIMENTACION_FAM,
                            APORTACIONES_FAM = Entity.APORTACIONES_FAM,
                            APOYO_FAM_OTROS = Entity.APOYO_FAM_OTROS,
                            CARACT_FP_ANTECE_PENALES_ADIC = Entity.CARACT_FP_ANTECE_PENALES_ADIC,
                            CARACT_FP_ANTECEDENTES_PENALES = Entity.CARACT_FP_ANTECEDENTES_PENALES,
                            CARACT_FP_CONCEPTO = Entity.CARACT_FP_CONCEPTO,
                            CARACT_FP_GRUPO = Entity.CARACT_FP_GRUPO,
                            CARACT_FP_NIVEL_SOCIO_CULTURAL = Entity.CARACT_FP_NIVEL_SOCIO_CULTURAL,
                            CARACT_FP_RELAC_INTERFAM = Entity.CARACT_FP_RELAC_INTERFAM,
                            CARACT_FP_VIOLENCIA_FAM = Entity.CARACT_FP_VIOLENCIA_FAM,
                            CARACT_FP_VIOLENCIA_FAM_ESPEFI = Entity.CARACT_FP_VIOLENCIA_FAM_ESPEFI,
                            CARACT_FS_GRUPO = Entity.CARACT_FS_GRUPO,
                            CARACT_FS_HIJOS_ANT = Entity.CARACT_FS_HIJOS_ANT,
                            CARACT_FS_NIVEL_SOCIO_CULTURAL = Entity.CARACT_FS_NIVEL_SOCIO_CULTURAL,
                            CARACT_FS_PROBLEMAS_CONDUCTA = Entity.CARACT_FS_PROBLEMAS_CONDUCTA,
                            CARACT_FS_PROBLEMAS_CONDUCTA_E = Entity.CARACT_FS_PROBLEMAS_CONDUCTA_E,
                            CARACT_FS_RELACION_MEDIO_EXT = Entity.CARACT_FS_RELACION_MEDIO_EXT,
                            CARACT_FS_RELACIONES_INTERFAM = Entity.CARACT_FS_RELACIONES_INTERFAM,
                            CARACT_FS_VIOLENCIA_INTRAFAM = Entity.CARACT_FS_VIOLENCIA_INTRAFAM,
                            CARACT_FS_VIOLENCIA_INTRAFAM_E = Entity.CARACT_FS_VIOLENCIA_INTRAFAM_E,
                            CARACT_FS_VIVIEN_DESCRIPCION = Entity.CARACT_FS_VIVIEN_DESCRIPCION,
                            CARACT_FS_VIVIEN_MOBILIARIO = Entity.CARACT_FS_VIVIEN_MOBILIARIO,
                            CARACT_FS_VIVIEN_NUM_HABITACIO = Entity.CARACT_FS_VIVIEN_NUM_HABITACIO,
                            CARACT_FS_VIVIEN_TRANSPORTE = Entity.CARACT_FS_VIVIEN_TRANSPORTE,
                            CARACT_FS_ZONA = Entity.CARACT_FS_ZONA,
                            DIAG_SOCIAL_PRONOS = Entity.DIAG_SOCIAL_PRONOS,
                            DIALECTO = Entity.DIALECTO != -1 ? Entity.DIALECTO : null,
                            DIRECTOR_CENTRO = Entity.DIRECTOR_CENTRO,
                            DISTRIBUCION_GASTO_FAM = Entity.DISTRIBUCION_GASTO_FAM,
                            DOMICILIO = Entity.DOMICILIO,
                            ECO_FP_COOPERA_ACTUALMENTE = Entity.ECO_FP_COOPERA_ACTUALMENTE,
                            ECO_FP_FONDOS_AHORRO = Entity.ECO_FP_FONDOS_AHORRO,
                            ECO_FP_RESPONSABLE = Entity.ECO_FP_RESPONSABLE,
                            ECO_FP_TOTAL_EGRESOS_MEN = Entity.ECO_FP_TOTAL_EGRESOS_MEN,
                            ECO_FP_TOTAL_INGRESOS_MEN = Entity.ECO_FP_TOTAL_INGRESOS_MEN,
                            ECO_FP_ZONA = Entity.ECO_FP_ZONA,
                            EDO_CIVIL = Entity.EDO_CIVIL != -1 ? Entity.EDO_CIVIL : null,
                            ESCOLARIDAD_ACTUAL = Entity.ESCOLARIDAD_ACTUAL != -1 ? Entity.ESCOLARIDAD_ACTUAL : null,
                            ESCOLARIDAD_CENTRO = Entity.ESCOLARIDAD_CENTRO != -1 ? Entity.ESCOLARIDAD_CENTRO : null,
                            AVAL_MORAL = Entity.AVAL_MORAL,
                            AVAL_MORAL_PARENTESCO = Entity.AVAL_MORAL_PARENTESCO,
                            EXTERNADO_CALLE = Entity.EXTERNADO_CALLE,
                            EXTERNADO_CIUDAD = Entity.EXTERNADO_CIUDAD.HasValue ? Entity.EXTERNADO_CIUDAD.Value != -1 ? Entity.EXTERNADO_CIUDAD.Value != decimal.Zero ? Entity.EXTERNADO_CIUDAD : new short?() : new short?() : new short?(),
                            EXTERNADO_COLONIA = Entity.EXTERNADO_COLONIA.HasValue ? Entity.EXTERNADO_COLONIA.Value != -1 ? Entity.EXTERNADO_COLONIA.Value != decimal.Zero ? Entity.EXTERNADO_COLONIA : new int?() : new int?() : new int?(),
                            EXTERNADO_CP = Entity.EXTERNADO_CP,
                            EXTERNADO_ENTIDAD = Entity.EXTERNADO_ENTIDAD.HasValue ? Entity.EXTERNADO_ENTIDAD.Value != -1 ? Entity.EXTERNADO_ENTIDAD.Value != decimal.Zero ? Entity.EXTERNADO_ENTIDAD : new short?() : new short?() : new short?(),
                            EXTERNADO_MUNICIPIO = Entity.EXTERNADO_MUNICIPIO.HasValue ? Entity.EXTERNADO_MUNICIPIO.Value != -1 ? Entity.EXTERNADO_MUNICIPIO.Value != decimal.Zero ? Entity.EXTERNADO_MUNICIPIO : new short?() : new short?() : new short?(),
                            EXTERNADO_NUMERO = Entity.EXTERNADO_NUMERO,
                            EXTERNADO_PARENTESCO = Entity.EXTERNADO_PARENTESCO.HasValue ? Entity.EXTERNADO_PARENTESCO.Value != -1 ? Entity.EXTERNADO_PARENTESCO : null : null,
                            EXTERNADO_VIVIR_NOMBRE = Entity.EXTERNADO_VIVIR_NOMBRE,
                            RADICAN_ESTADO = Entity.RADICAN_ESTADO,
                            VISITA_FRECUENCIA = Entity.VISITA_FRECUENCIA,
                            FECHA = Entity.FECHA,
                            FECHA_NAC = Entity.FECHA_NAC,
                            ID_ANIO = _Actual.ID_ANIO,
                            ID_ESTUDIO = _Actual.ID_ESTUDIO,
                            ID_CENTRO = _Actual.ID_CENTRO,
                            ID_IMPUTADO = _Actual.ID_IMPUTADO,
                            ID_INGRESO = _Actual.ID_INGRESO,
                            INFLUENCIADO_ESTANCIA_PRISION = Entity.INFLUENCIADO_ESTANCIA_PRISION,
                            LUGAR = Entity.LUGAR,
                            LUGAR_NAC = Entity.LUGAR_NAC,
                            NOMBRE = Entity.NOMBRE,
                            NUM_PAREJAS_ESTABLE = Entity.NUM_PAREJAS_ESTABLE,
                            OCUPACION_ANT = Entity.OCUPACION_ANT != -1 ? Entity.OCUPACION_ANT : null,
                            OFERTA_TRABAJO = Entity.OFERTA_TRABAJO,
                            OFERTA_TRABAJO_CONSISTE = Entity.OFERTA_TRABAJO_CONSISTE,
                            OPINION_CONCESION_BENEFICIOS = Entity.OPINION_CONCESION_BENEFICIOS,
                            OPINION_INTERNAMIENTO = Entity.OPINION_INTERNAMIENTO,
                            SERVICIOS_PUBLICOS = Entity.SERVICIOS_PUBLICOS,
                            SUELDO_PERCIBIDO = Entity.SUELDO_PERCIBIDO,
                            TIEMPO_LABORAR = Entity.TIEMPO_LABORAR,
                            TRABAJADORA_SOCIAL = Entity.TRABAJADORA_SOCIAL,
                            TRABAJO_DESEMP_ANTES = Entity.TRABAJO_DESEMP_ANTES,
                            VISITA_FAMILIARES = Entity.VISITA_FAMILIARES,
                            VISITA_OTROS_QUIIEN = Entity.VISITA_OTROS_QUIIEN,
                            VISITAS_OTROS = Entity.VISITAS_OTROS,
                            VISTA_PARENTESCO = Entity.VISTA_PARENTESCO.HasValue ? Entity.VISTA_PARENTESCO.Value != -1 ? Entity.VISTA_PARENTESCO : null : null
                        };

                        #region Grupos TS

                        var _consecutivoSocialFederal = GetIDProceso<short>("PFF_GRUPO_FAMILIAR", "ID_CONSEC", string.Format("ID_CENTRO = {0} AND ID_ANIO = {1} AND ID_IMPUTADO = {2} AND ID_INGRESO = {3} AND ID_ESTUDIO = {4}", _Actual.ID_CENTRO, _Actual.ID_ANIO, _Actual.ID_IMPUTADO, _Actual.ID_INGRESO, _Actual.ID_ESTUDIO));
                        if (Entity.PFF_GRUPO_FAMILIAR != null && Entity.PFF_GRUPO_FAMILIAR.Any())
                            foreach (var item in Entity.PFF_GRUPO_FAMILIAR)
                            {
                                var _NvoGrupo = new PFF_GRUPO_FAMILIAR()
                                {
                                    EDAD = item.EDAD,
                                    ESTADO_CIVIL = item.ESTADO_CIVIL,
                                    ID_ANIO = _Actual.ID_ANIO,
                                    ID_CENTRO = _Actual.ID_CENTRO,
                                    ID_CONSEC = _consecutivoSocialFederal,
                                    ID_ESTUDIO = _Actual.ID_ESTUDIO,
                                    ID_GRADO = item.ID_GRADO,
                                    ID_GRUPO_FAMILIAR = item.ID_GRUPO_FAMILIAR,
                                    ID_IMPUTADO = _Actual.ID_IMPUTADO,
                                    ID_INGRESO = _Actual.ID_INGRESO,
                                    NOMBRE = item.NOMBRE,
                                    OCUPACION = item.OCUPACION,
                                    PARENTESCO = item.PARENTESCO
                                };

                                Context.PFF_GRUPO_FAMILIAR.Add(_NvoGrupo);
                                _consecutivoSocialFederal++;
                            };

                        #endregion

                        Context.PERSONALIDAD_FUERO_FEDERAL.Add(personalidadFederal);
                        Context.PFF_TRABAJO_SOCIAL.Add(_SocialFamiliar);
                    }
                    else
                    {
                        var _SocFamA = _Actual.PFF_TRABAJO_SOCIAL;
                        if (_SocFamA == null)
                        {
                            _SocFamA = new PFF_TRABAJO_SOCIAL()
                            {
                                ALIMENTACION_FAM = Entity.ALIMENTACION_FAM,
                                APORTACIONES_FAM = Entity.APORTACIONES_FAM,
                                APOYO_FAM_OTROS = Entity.APOYO_FAM_OTROS,
                                CARACT_FP_ANTECE_PENALES_ADIC = Entity.CARACT_FP_ANTECE_PENALES_ADIC,
                                CARACT_FP_ANTECEDENTES_PENALES = Entity.CARACT_FP_ANTECEDENTES_PENALES,
                                CARACT_FP_CONCEPTO = Entity.CARACT_FP_CONCEPTO,
                                CARACT_FP_GRUPO = Entity.CARACT_FP_GRUPO,
                                CARACT_FP_NIVEL_SOCIO_CULTURAL = Entity.CARACT_FP_NIVEL_SOCIO_CULTURAL,
                                CARACT_FP_RELAC_INTERFAM = Entity.CARACT_FP_RELAC_INTERFAM,
                                CARACT_FP_VIOLENCIA_FAM = Entity.CARACT_FP_VIOLENCIA_FAM,
                                CARACT_FP_VIOLENCIA_FAM_ESPEFI = Entity.CARACT_FP_VIOLENCIA_FAM_ESPEFI,
                                CARACT_FS_GRUPO = Entity.CARACT_FS_GRUPO,
                                CARACT_FS_HIJOS_ANT = Entity.CARACT_FS_HIJOS_ANT,
                                CARACT_FS_NIVEL_SOCIO_CULTURAL = Entity.CARACT_FS_NIVEL_SOCIO_CULTURAL,
                                CARACT_FS_PROBLEMAS_CONDUCTA = Entity.CARACT_FS_PROBLEMAS_CONDUCTA,
                                CARACT_FS_PROBLEMAS_CONDUCTA_E = Entity.CARACT_FS_PROBLEMAS_CONDUCTA_E,
                                CARACT_FS_RELACION_MEDIO_EXT = Entity.CARACT_FS_RELACION_MEDIO_EXT,
                                CARACT_FS_RELACIONES_INTERFAM = Entity.CARACT_FS_RELACIONES_INTERFAM,
                                CARACT_FS_VIOLENCIA_INTRAFAM = Entity.CARACT_FS_VIOLENCIA_INTRAFAM,
                                CARACT_FS_VIOLENCIA_INTRAFAM_E = Entity.CARACT_FS_VIOLENCIA_INTRAFAM_E,
                                CARACT_FS_VIVIEN_DESCRIPCION = Entity.CARACT_FS_VIVIEN_DESCRIPCION,
                                CARACT_FS_VIVIEN_MOBILIARIO = Entity.CARACT_FS_VIVIEN_MOBILIARIO,
                                CARACT_FS_VIVIEN_NUM_HABITACIO = Entity.CARACT_FS_VIVIEN_NUM_HABITACIO,
                                CARACT_FS_VIVIEN_TRANSPORTE = Entity.CARACT_FS_VIVIEN_TRANSPORTE,
                                CARACT_FS_ZONA = Entity.CARACT_FS_ZONA,
                                DIAG_SOCIAL_PRONOS = Entity.DIAG_SOCIAL_PRONOS,
                                DIALECTO = Entity.DIALECTO != -1 ? Entity.DIALECTO : null,
                                DIRECTOR_CENTRO = Entity.DIRECTOR_CENTRO,
                                DISTRIBUCION_GASTO_FAM = Entity.DISTRIBUCION_GASTO_FAM,
                                DOMICILIO = Entity.DOMICILIO,
                                ECO_FP_COOPERA_ACTUALMENTE = Entity.ECO_FP_COOPERA_ACTUALMENTE,
                                ECO_FP_FONDOS_AHORRO = Entity.ECO_FP_FONDOS_AHORRO,
                                ECO_FP_RESPONSABLE = Entity.ECO_FP_RESPONSABLE,
                                ECO_FP_TOTAL_EGRESOS_MEN = Entity.ECO_FP_TOTAL_EGRESOS_MEN,
                                ECO_FP_TOTAL_INGRESOS_MEN = Entity.ECO_FP_TOTAL_INGRESOS_MEN,
                                ECO_FP_ZONA = Entity.ECO_FP_ZONA,
                                EDO_CIVIL = Entity.EDO_CIVIL != -1 ? Entity.EDO_CIVIL : null,
                                ESCOLARIDAD_ACTUAL = Entity.ESCOLARIDAD_ACTUAL != -1 ? Entity.ESCOLARIDAD_ACTUAL : null,
                                ESCOLARIDAD_CENTRO = Entity.ESCOLARIDAD_CENTRO != -1 ? Entity.ESCOLARIDAD_CENTRO : null,
                                AVAL_MORAL = Entity.AVAL_MORAL,
                                AVAL_MORAL_PARENTESCO = Entity.AVAL_MORAL_PARENTESCO,
                                EXTERNADO_CALLE = Entity.EXTERNADO_CALLE,
                                EXTERNADO_CIUDAD = Entity.EXTERNADO_CIUDAD.HasValue ? Entity.EXTERNADO_CIUDAD.Value != -1 ? Entity.EXTERNADO_CIUDAD.Value != decimal.Zero ? Entity.EXTERNADO_CIUDAD : new short?() : new short?() : new short?(),
                                EXTERNADO_COLONIA = Entity.EXTERNADO_COLONIA.HasValue ? Entity.EXTERNADO_COLONIA.Value != -1 ? Entity.EXTERNADO_COLONIA.Value != decimal.Zero ? Entity.EXTERNADO_COLONIA : new int?() : new int?() : new int?(),
                                EXTERNADO_CP = Entity.EXTERNADO_CP,
                                EXTERNADO_ENTIDAD = Entity.EXTERNADO_ENTIDAD.HasValue ? Entity.EXTERNADO_ENTIDAD.Value != -1 ? Entity.EXTERNADO_ENTIDAD.Value != decimal.Zero ? Entity.EXTERNADO_ENTIDAD : new short?() : new short?() : new short?(),
                                EXTERNADO_MUNICIPIO = Entity.EXTERNADO_MUNICIPIO.HasValue ? Entity.EXTERNADO_MUNICIPIO.Value != -1 ? Entity.EXTERNADO_MUNICIPIO.Value != decimal.Zero ? Entity.EXTERNADO_MUNICIPIO : new short?() : new short?() : new short?(),
                                EXTERNADO_NUMERO = Entity.EXTERNADO_NUMERO,
                                EXTERNADO_PARENTESCO = Entity.EXTERNADO_PARENTESCO.HasValue ? Entity.EXTERNADO_PARENTESCO.Value != -1 ? Entity.EXTERNADO_PARENTESCO : null : null,
                                EXTERNADO_VIVIR_NOMBRE = Entity.EXTERNADO_VIVIR_NOMBRE,
                                RADICAN_ESTADO = Entity.RADICAN_ESTADO,
                                VISITA_FRECUENCIA = Entity.VISITA_FRECUENCIA,
                                FECHA = Entity.FECHA,
                                FECHA_NAC = Entity.FECHA_NAC,
                                ID_ANIO = _Actual.ID_ANIO,
                                ID_ESTUDIO = _Actual.ID_ESTUDIO,
                                ID_CENTRO = _Actual.ID_CENTRO,
                                ID_IMPUTADO = _Actual.ID_IMPUTADO,
                                ID_INGRESO = _Actual.ID_INGRESO,
                                INFLUENCIADO_ESTANCIA_PRISION = Entity.INFLUENCIADO_ESTANCIA_PRISION,
                                LUGAR = Entity.LUGAR,
                                LUGAR_NAC = Entity.LUGAR_NAC,
                                NOMBRE = Entity.NOMBRE,
                                NUM_PAREJAS_ESTABLE = Entity.NUM_PAREJAS_ESTABLE,
                                OCUPACION_ANT = Entity.OCUPACION_ANT != -1 ? Entity.OCUPACION_ANT : null,
                                OFERTA_TRABAJO = Entity.OFERTA_TRABAJO,
                                OFERTA_TRABAJO_CONSISTE = Entity.OFERTA_TRABAJO_CONSISTE,
                                OPINION_CONCESION_BENEFICIOS = Entity.OPINION_CONCESION_BENEFICIOS,
                                OPINION_INTERNAMIENTO = Entity.OPINION_INTERNAMIENTO,
                                SERVICIOS_PUBLICOS = Entity.SERVICIOS_PUBLICOS,
                                SUELDO_PERCIBIDO = Entity.SUELDO_PERCIBIDO,
                                TIEMPO_LABORAR = Entity.TIEMPO_LABORAR,
                                TRABAJADORA_SOCIAL = Entity.TRABAJADORA_SOCIAL,
                                TRABAJO_DESEMP_ANTES = Entity.TRABAJO_DESEMP_ANTES,
                                VISITA_FAMILIARES = Entity.VISITA_FAMILIARES,
                                VISITA_OTROS_QUIIEN = Entity.VISITA_OTROS_QUIIEN,
                                VISITAS_OTROS = Entity.VISITAS_OTROS,
                                VISTA_PARENTESCO = Entity.VISTA_PARENTESCO.HasValue ? Entity.VISTA_PARENTESCO.Value != -1 ? Entity.VISTA_PARENTESCO : null : null
                            };

                            var _consecutivoSocialFederal = GetIDProceso<short>("PFF_GRUPO_FAMILIAR", "ID_CONSEC", string.Format("ID_CENTRO = {0} AND ID_ANIO = {1} AND ID_IMPUTADO = {2} AND ID_INGRESO = {3} AND ID_ESTUDIO = {4}", _Actual.ID_CENTRO, _Actual.ID_ANIO, _Actual.ID_IMPUTADO, _Actual.ID_INGRESO, _Actual.ID_ESTUDIO));
                            if (Entity.PFF_GRUPO_FAMILIAR != null && Entity.PFF_GRUPO_FAMILIAR.Any())
                                foreach (var item in Entity.PFF_GRUPO_FAMILIAR)
                                {
                                    var _NvoGrupo = new PFF_GRUPO_FAMILIAR()
                                    {
                                        EDAD = item.EDAD,
                                        ESTADO_CIVIL = item.ESTADO_CIVIL,
                                        ID_ANIO = _Actual.ID_ANIO,
                                        ID_CENTRO = _Actual.ID_CENTRO,
                                        ID_CONSEC = _consecutivoSocialFederal,
                                        ID_ESTUDIO = _Actual.ID_ESTUDIO,
                                        ID_GRADO = item.ID_GRADO,
                                        ID_GRUPO_FAMILIAR = item.ID_GRUPO_FAMILIAR,
                                        ID_IMPUTADO = _Actual.ID_IMPUTADO,
                                        ID_INGRESO = _Actual.ID_INGRESO,
                                        NOMBRE = item.NOMBRE,
                                        OCUPACION = item.OCUPACION,
                                        PARENTESCO = item.PARENTESCO
                                    };

                                    Context.PFF_GRUPO_FAMILIAR.Add(_NvoGrupo);
                                    _consecutivoSocialFederal++;
                                };

                            Context.PFF_TRABAJO_SOCIAL.Add(_SocFamA);
                        }
                        else
                        {
                            var _GruposFam = Context.PFF_GRUPO_FAMILIAR.Where(x => x.ID_INGRESO == _EstudioPadre.ID_INGRESO && x.ID_IMPUTADO == _EstudioPadre.ID_IMPUTADO && x.ID_ESTUDIO == _EstudioPadre.ID_ESTUDIO && x.ID_ANIO == _EstudioPadre.ID_ANIO && x.ID_CENTRO == _EstudioPadre.ID_CENTRO);

                            _SocFamA.ALIMENTACION_FAM = Entity.ALIMENTACION_FAM;
                            _SocFamA.APORTACIONES_FAM = Entity.APORTACIONES_FAM;
                            _SocFamA.APOYO_FAM_OTROS = Entity.APOYO_FAM_OTROS;
                            _SocFamA.CARACT_FP_ANTECE_PENALES_ADIC = Entity.CARACT_FP_ANTECE_PENALES_ADIC;
                            _SocFamA.CARACT_FP_ANTECEDENTES_PENALES = Entity.CARACT_FP_ANTECEDENTES_PENALES;
                            _SocFamA.CARACT_FP_CONCEPTO = Entity.CARACT_FP_CONCEPTO;
                            _SocFamA.CARACT_FP_GRUPO = Entity.CARACT_FP_GRUPO;
                            _SocFamA.CARACT_FP_NIVEL_SOCIO_CULTURAL = Entity.CARACT_FP_NIVEL_SOCIO_CULTURAL;
                            _SocFamA.CARACT_FP_RELAC_INTERFAM = Entity.CARACT_FP_RELAC_INTERFAM;
                            _SocFamA.CARACT_FP_VIOLENCIA_FAM = Entity.CARACT_FP_VIOLENCIA_FAM;
                            _SocFamA.CARACT_FP_VIOLENCIA_FAM_ESPEFI = Entity.CARACT_FP_VIOLENCIA_FAM_ESPEFI;
                            _SocFamA.CARACT_FS_GRUPO = Entity.CARACT_FS_GRUPO;
                            _SocFamA.CARACT_FS_HIJOS_ANT = Entity.CARACT_FS_HIJOS_ANT;
                            _SocFamA.CARACT_FS_NIVEL_SOCIO_CULTURAL = Entity.CARACT_FS_NIVEL_SOCIO_CULTURAL;
                            _SocFamA.CARACT_FS_PROBLEMAS_CONDUCTA = Entity.CARACT_FS_PROBLEMAS_CONDUCTA;
                            _SocFamA.CARACT_FS_PROBLEMAS_CONDUCTA_E = Entity.CARACT_FS_PROBLEMAS_CONDUCTA_E;
                            _SocFamA.CARACT_FS_RELACION_MEDIO_EXT = Entity.CARACT_FS_RELACION_MEDIO_EXT;
                            _SocFamA.CARACT_FS_RELACIONES_INTERFAM = Entity.CARACT_FS_RELACIONES_INTERFAM;
                            _SocFamA.CARACT_FS_VIOLENCIA_INTRAFAM = Entity.CARACT_FS_VIOLENCIA_INTRAFAM;
                            _SocFamA.CARACT_FS_VIOLENCIA_INTRAFAM_E = Entity.CARACT_FS_VIOLENCIA_INTRAFAM_E;
                            _SocFamA.CARACT_FS_VIVIEN_DESCRIPCION = Entity.CARACT_FS_VIVIEN_DESCRIPCION;
                            _SocFamA.CARACT_FS_VIVIEN_MOBILIARIO = Entity.CARACT_FS_VIVIEN_MOBILIARIO;
                            _SocFamA.CARACT_FS_VIVIEN_NUM_HABITACIO = Entity.CARACT_FS_VIVIEN_NUM_HABITACIO;
                            _SocFamA.CARACT_FS_VIVIEN_TRANSPORTE = Entity.CARACT_FS_VIVIEN_TRANSPORTE;
                            _SocFamA.CARACT_FS_ZONA = Entity.CARACT_FS_ZONA;
                            _SocFamA.DIAG_SOCIAL_PRONOS = Entity.DIAG_SOCIAL_PRONOS;
                            _SocFamA.DIALECTO = Entity.DIALECTO != -1 ? Entity.DIALECTO : null;
                            _SocFamA.DIRECTOR_CENTRO = Entity.DIRECTOR_CENTRO;
                            _SocFamA.DISTRIBUCION_GASTO_FAM = Entity.DISTRIBUCION_GASTO_FAM;
                            _SocFamA.DOMICILIO = Entity.DOMICILIO;
                            _SocFamA.ECO_FP_COOPERA_ACTUALMENTE = Entity.ECO_FP_COOPERA_ACTUALMENTE;
                            _SocFamA.ECO_FP_FONDOS_AHORRO = Entity.ECO_FP_FONDOS_AHORRO;
                            _SocFamA.ECO_FP_RESPONSABLE = Entity.ECO_FP_RESPONSABLE;
                            _SocFamA.ECO_FP_TOTAL_EGRESOS_MEN = Entity.ECO_FP_TOTAL_EGRESOS_MEN;
                            _SocFamA.ECO_FP_TOTAL_INGRESOS_MEN = Entity.ECO_FP_TOTAL_INGRESOS_MEN;
                            _SocFamA.ECO_FP_ZONA = Entity.ECO_FP_ZONA;
                            _SocFamA.EDO_CIVIL = Entity.EDO_CIVIL != -1 ? Entity.EDO_CIVIL : null;
                            _SocFamA.ESCOLARIDAD_ACTUAL = Entity.ESCOLARIDAD_ACTUAL != -1 ? Entity.ESCOLARIDAD_ACTUAL : null;
                            _SocFamA.ESCOLARIDAD_CENTRO = Entity.ESCOLARIDAD_CENTRO != -1 ? Entity.ESCOLARIDAD_CENTRO : null;
                            _SocFamA.AVAL_MORAL = Entity.AVAL_MORAL;
                            _SocFamA.AVAL_MORAL_PARENTESCO = Entity.AVAL_MORAL_PARENTESCO;
                            _SocFamA.EXTERNADO_CALLE = Entity.EXTERNADO_CALLE;
                            _SocFamA.EXTERNADO_CIUDAD = Entity.EXTERNADO_CIUDAD.HasValue ? Entity.EXTERNADO_CIUDAD.Value != -1 ? Entity.EXTERNADO_CIUDAD.Value != decimal.Zero ? Entity.EXTERNADO_CIUDAD : new short?() : new short?() : new short?();
                            _SocFamA.EXTERNADO_COLONIA = Entity.EXTERNADO_COLONIA.HasValue ? Entity.EXTERNADO_COLONIA.Value != -1 ? Entity.EXTERNADO_COLONIA.Value != decimal.Zero ? Entity.EXTERNADO_COLONIA : new int?() : new int?() : new int?();
                            _SocFamA.EXTERNADO_CP = Entity.EXTERNADO_CP;
                            _SocFamA.EXTERNADO_ENTIDAD = Entity.EXTERNADO_ENTIDAD.HasValue ? Entity.EXTERNADO_ENTIDAD.Value != -1 ? Entity.EXTERNADO_ENTIDAD.Value != decimal.Zero ? Entity.EXTERNADO_ENTIDAD : new short?() : new short?() : new short?();
                            _SocFamA.EXTERNADO_MUNICIPIO = Entity.EXTERNADO_MUNICIPIO.HasValue ? Entity.EXTERNADO_MUNICIPIO.Value != -1 ? Entity.EXTERNADO_MUNICIPIO.Value != decimal.Zero ? Entity.EXTERNADO_MUNICIPIO : new short?() : new short?() : new short?();
                            _SocFamA.EXTERNADO_NUMERO = Entity.EXTERNADO_NUMERO;
                            _SocFamA.EXTERNADO_PARENTESCO = Entity.EXTERNADO_PARENTESCO.HasValue ? Entity.EXTERNADO_PARENTESCO.Value != -1 ? Entity.EXTERNADO_PARENTESCO : null : null;
                            _SocFamA.EXTERNADO_VIVIR_NOMBRE = Entity.EXTERNADO_VIVIR_NOMBRE;
                            _SocFamA.RADICAN_ESTADO = Entity.RADICAN_ESTADO;
                            _SocFamA.VISITA_FRECUENCIA = Entity.VISITA_FRECUENCIA;
                            _SocFamA.FECHA = Entity.FECHA;
                            _SocFamA.FECHA_NAC = Entity.FECHA_NAC;
                            _SocFamA.ID_ANIO = _Actual.ID_ANIO;
                            _SocFamA.ID_CENTRO = _Actual.ID_CENTRO;
                            _SocFamA.ID_IMPUTADO = _Actual.ID_IMPUTADO;
                            _SocFamA.ID_INGRESO = _Actual.ID_INGRESO;
                            _SocFamA.ID_ESTUDIO = _Actual.ID_ESTUDIO;
                            _SocFamA.INFLUENCIADO_ESTANCIA_PRISION = Entity.INFLUENCIADO_ESTANCIA_PRISION;
                            _SocFamA.LUGAR = Entity.LUGAR;
                            _SocFamA.LUGAR_NAC = Entity.LUGAR_NAC;
                            _SocFamA.NOMBRE = Entity.NOMBRE;
                            _SocFamA.NUM_PAREJAS_ESTABLE = Entity.NUM_PAREJAS_ESTABLE;
                            _SocFamA.OCUPACION_ANT = Entity.OCUPACION_ANT != -1 ? Entity.OCUPACION_ANT : null;
                            _SocFamA.OFERTA_TRABAJO = Entity.OFERTA_TRABAJO;
                            _SocFamA.OFERTA_TRABAJO_CONSISTE = Entity.OFERTA_TRABAJO_CONSISTE;
                            _SocFamA.OPINION_CONCESION_BENEFICIOS = Entity.OPINION_CONCESION_BENEFICIOS;
                            _SocFamA.OPINION_INTERNAMIENTO = Entity.OPINION_INTERNAMIENTO;
                            _SocFamA.SERVICIOS_PUBLICOS = Entity.SERVICIOS_PUBLICOS;
                            _SocFamA.SUELDO_PERCIBIDO = Entity.SUELDO_PERCIBIDO;
                            _SocFamA.TIEMPO_LABORAR = Entity.TIEMPO_LABORAR;
                            _SocFamA.TRABAJADORA_SOCIAL = Entity.TRABAJADORA_SOCIAL;
                            _SocFamA.TRABAJO_DESEMP_ANTES = Entity.TRABAJO_DESEMP_ANTES;
                            _SocFamA.VISITA_FAMILIARES = Entity.VISITA_FAMILIARES;
                            _SocFamA.VISITA_OTROS_QUIIEN = Entity.VISITA_OTROS_QUIIEN;
                            _SocFamA.VISITAS_OTROS = Entity.VISITAS_OTROS;
                            _SocFamA.VISTA_PARENTESCO = Entity.VISTA_PARENTESCO.HasValue ? Entity.VISTA_PARENTESCO.Value != -1 ? Entity.VISTA_PARENTESCO : null : null;
                            Context.Entry(_SocFamA).State = System.Data.EntityState.Modified;

                            #region Gru TS
                            if (_GruposFam != null && _GruposFam.Any())
                                foreach (var item in _GruposFam)
                                    Context.Entry(item).State = System.Data.EntityState.Deleted;

                            var _consecutivoSocialFederal = GetIDProceso<short>("PFF_GRUPO_FAMILIAR", "ID_CONSEC", string.Format("ID_CENTRO = {0} AND ID_ANIO = {1} AND ID_IMPUTADO = {2} AND ID_INGRESO = {3} AND ID_ESTUDIO = {4}", _EstudioPadre.ID_CENTRO, _EstudioPadre.ID_ANIO, _EstudioPadre.ID_IMPUTADO, _EstudioPadre.ID_INGRESO, _EstudioPadre.ID_ESTUDIO));
                            if (Entity.PFF_GRUPO_FAMILIAR != null && Entity.PFF_GRUPO_FAMILIAR.Any())
                                foreach (var item in Entity.PFF_GRUPO_FAMILIAR)
                                {
                                    var _NvoGrupo = new PFF_GRUPO_FAMILIAR()
                                    {
                                        EDAD = item.EDAD,
                                        ESTADO_CIVIL = item.ESTADO_CIVIL,
                                        ID_ANIO = _Actual.ID_ANIO,
                                        ID_CENTRO = _Actual.ID_CENTRO,
                                        ID_CONSEC = _consecutivoSocialFederal,
                                        ID_ESTUDIO = _Actual.ID_ESTUDIO,
                                        ID_GRADO = item.ID_GRADO,
                                        ID_GRUPO_FAMILIAR = item.ID_GRUPO_FAMILIAR,
                                        ID_IMPUTADO = _Actual.ID_IMPUTADO,
                                        ID_INGRESO = _Actual.ID_INGRESO,
                                        NOMBRE = item.NOMBRE,
                                        OCUPACION = item.OCUPACION,
                                        PARENTESCO = item.PARENTESCO
                                    };

                                    Context.PFF_GRUPO_FAMILIAR.Add(_NvoGrupo);
                                    _consecutivoSocialFederal++;
                                };
                            #endregion
                        }
                    }


                    #region Definicion de detalle de estudio de trabajo social de fuero federal
                    if (_EstudioPadre != null)
                    {
                        short? _Estatus = new short?();
                        if (string.IsNullOrEmpty(Entity.ALIMENTACION_FAM) || string.IsNullOrEmpty(Entity.APORTACIONES_FAM) || string.IsNullOrEmpty(Entity.AVAL_MORAL) || string.IsNullOrEmpty(Entity.CARACT_FP_CONCEPTO) || string.IsNullOrEmpty(Entity.CARACT_FP_GRUPO) ||
                            string.IsNullOrEmpty(Entity.CARACT_FP_NIVEL_SOCIO_CULTURAL) || string.IsNullOrEmpty(Entity.CARACT_FP_RELAC_INTERFAM) ||
                            string.IsNullOrEmpty(Entity.CARACT_FP_VIOLENCIA_FAM) || string.IsNullOrEmpty(Entity.CARACT_FS_GRUPO) ||
                            string.IsNullOrEmpty(Entity.CARACT_FS_HIJOS_ANT) || string.IsNullOrEmpty(Entity.CARACT_FS_NIVEL_SOCIO_CULTURAL) ||
                            string.IsNullOrEmpty(Entity.CARACT_FS_PROBLEMAS_CONDUCTA) || string.IsNullOrEmpty(Entity.CARACT_FS_RELACION_MEDIO_EXT) ||
                            string.IsNullOrEmpty(Entity.CARACT_FS_RELACIONES_INTERFAM) || string.IsNullOrEmpty(Entity.CARACT_FS_VIOLENCIA_INTRAFAM) ||
                            string.IsNullOrEmpty(Entity.CARACT_FS_VIVIEN_DESCRIPCION) || string.IsNullOrEmpty(Entity.CARACT_FS_VIVIEN_MOBILIARIO) ||
                            string.IsNullOrEmpty(Entity.CARACT_FS_VIVIEN_TRANSPORTE) || string.IsNullOrEmpty(Entity.CARACT_FS_ZONA) ||
                            string.IsNullOrEmpty(Entity.DIAG_SOCIAL_PRONOS) || Entity.DIALECTO == null || string.IsNullOrEmpty(Entity.DISTRIBUCION_GASTO_FAM) ||
                            string.IsNullOrEmpty(Entity.DOMICILIO) || string.IsNullOrEmpty(Entity.ECO_FP_COOPERA_ACTUALMENTE) ||
                            string.IsNullOrEmpty(Entity.ECO_FP_FONDOS_AHORRO) || string.IsNullOrEmpty(Entity.ECO_FP_RESPONSABLE) ||
                            string.IsNullOrEmpty(Entity.ECO_FP_ZONA) || Entity.ESCOLARIDAD_ACTUAL == null || Entity.ESCOLARIDAD_CENTRO == null ||
                            string.IsNullOrEmpty(Entity.EXTERNADO_CALLE) || string.IsNullOrEmpty(Entity.EXTERNADO_CP) ||
                            string.IsNullOrEmpty(Entity.EXTERNADO_NUMERO) || string.IsNullOrEmpty(Entity.EXTERNADO_VIVIR_NOMBRE) ||
                            string.IsNullOrEmpty(Entity.INFLUENCIADO_ESTANCIA_PRISION) || string.IsNullOrEmpty(Entity.LUGAR_NAC) ||
                            string.IsNullOrEmpty(Entity.NOMBRE) || string.IsNullOrEmpty(Entity.NUM_PAREJAS_ESTABLE) ||
                            Entity.OCUPACION_ANT == null || string.IsNullOrEmpty(Entity.OFERTA_TRABAJO) ||
                            string.IsNullOrEmpty(Entity.OPINION_CONCESION_BENEFICIOS) || string.IsNullOrEmpty(Entity.OPINION_INTERNAMIENTO) ||
                            string.IsNullOrEmpty(Entity.SERVICIOS_PUBLICOS) || string.IsNullOrEmpty(Entity.TIEMPO_LABORAR) ||
                            string.IsNullOrEmpty(Entity.TRABAJO_DESEMP_ANTES) || string.IsNullOrEmpty(Entity.VISITA_FAMILIARES) ||
                            Entity.CARACT_FS_VIVIEN_NUM_HABITACIO == null || Entity.ECO_FP_TOTAL_EGRESOS_MEN == null || Entity.ECO_FP_TOTAL_INGRESOS_MEN == null ||
                            Entity.EDO_CIVIL == null || Entity.EXTERNADO_CIUDAD == null ||
                            Entity.EXTERNADO_COLONIA == null || Entity.EXTERNADO_ENTIDAD == null || Entity.EXTERNADO_MUNICIPIO == null ||
                            Entity.EXTERNADO_PARENTESCO == null || Entity.FECHA == null || Entity.FECHA_NAC == null ||
                            Entity.SUELDO_PERCIBIDO == null)
                            _Estatus = (short)eEstatudDetallePersonalidad.ASIGNADO;
                        else
                            _Estatus = (short)eEstatudDetallePersonalidad.TERMINADO;

                        var _DesarrolloEstudioTSFederal = Context.PERSONALIDAD_DETALLE.FirstOrDefault(x => x.ID_IMPUTADO == _EstudioPadre.ID_IMPUTADO && x.ID_ESTUDIO == _EstudioPadre.ID_ESTUDIO && x.ID_INGRESO == _EstudioPadre.ID_INGRESO && x.ID_TIPO == (short)eTiposEstudio.TRABAJO_SOCIAL_FEDERAL && x.ID_CENTRO == _EstudioPadre.ID_CENTRO && x.ID_ANIO == _EstudioPadre.ID_ANIO);
                        if (_DesarrolloEstudioTSFederal == null)
                        {
                            short _ConsecutivoPersonalidadDetalle = GetIDProceso<short>("PERSONALIDAD_DETALLE", "ID_DETALLE", "1=1");
                            var NvoDetalle = new PERSONALIDAD_DETALLE()
                            {
                                ID_ANIO = _EstudioPadre.ID_ANIO,
                                ID_CENTRO = _EstudioPadre.ID_CENTRO,
                                ID_DETALLE = _ConsecutivoPersonalidadDetalle,
                                ID_ESTUDIO = _EstudioPadre.ID_ESTUDIO,
                                ID_IMPUTADO = _EstudioPadre.ID_IMPUTADO,
                                ID_ESTATUS = _Estatus,
                                ID_INGRESO = _EstudioPadre.ID_INGRESO,
                                ID_TIPO = (short)eTiposEstudio.TRABAJO_SOCIAL_FEDERAL,
                                INICIO_FEC = GetFechaServerDate(),
                                RESULTADO = _Estatus == (short)eEstatudDetallePersonalidad.TERMINADO ? "S" : "N",
                                SOLICITUD_FEC = null,
                                TERMINO_FEC = _Estatus == (short)eEstatudDetallePersonalidad.TERMINADO ? GetFechaServerDate() : new System.DateTime?(),
                                TIPO_MEDIA = string.Empty
                            };

                            Context.PERSONALIDAD_DETALLE.Add(NvoDetalle);
                        }
                        else
                        {
                            _DesarrolloEstudioTSFederal.RESULTADO = _Estatus.HasValue ? _Estatus.Value == (short)eEstatudDetallePersonalidad.TERMINADO ? "S" : "N" : "N";
                            _DesarrolloEstudioTSFederal.ID_ESTATUS = _Estatus;
                            Context.Entry(_DesarrolloEstudioTSFederal).State = System.Data.EntityState.Modified;
                        };
                    };
                    #endregion

                    Context.SaveChanges();
                    transaccion.Complete();
                    return 1;
                }
            }
            catch (System.Data.Entity.Validation.DbEntityValidationException dbEx)
            {
                foreach (var validationErrors in dbEx.EntityValidationErrors)
                    foreach (var validationError in validationErrors.ValidationErrors)
                        System.Diagnostics.Trace.TraceInformation("Nombre del causante de excepción: {0} Error: {1}", validationError.PropertyName, validationError.ErrorMessage);

                return 2;
            }

            catch (System.Exception exc)
            {
                return 0;
            }
        }

        public short GuardarEstudioActividadesFederalAislado(PFF_ACTIVIDAD Entity)
        {
            try
            {
                using (System.Transactions.TransactionScope transaccion = new System.Transactions.TransactionScope(System.Transactions.TransactionScopeOption.RequiresNew, new System.Transactions.TransactionOptions() { IsolationLevel = System.Transactions.IsolationLevel.ReadCommitted }))
                {
                    var _EstudioPadre = Context.PERSONALIDAD.Where(x => x.ID_INGRESO == Entity.ID_INGRESO && x.ID_IMPUTADO == Entity.ID_IMPUTADO && x.ID_CENTRO == Entity.ID_CENTRO && x.ID_ANIO == Entity.ID_ANIO).OrderByDescending(x => x.ID_ESTUDIO).FirstOrDefault();

                    if (_EstudioPadre == null)
                        return 0;

                    var _Actual = Context.PERSONALIDAD_FUERO_FEDERAL.FirstOrDefault(x => x.ID_ANIO == _EstudioPadre.ID_ANIO && x.ID_CENTRO == _EstudioPadre.ID_CENTRO && x.ID_IMPUTADO == _EstudioPadre.ID_IMPUTADO && x.ID_INGRESO == _EstudioPadre.ID_INGRESO && x.ID_ESTUDIO == _EstudioPadre.ID_ESTUDIO);

                    if (_Actual == null)
                    {
                        var _EstudioTS = Context.PFF_ACTIVIDAD.FirstOrDefault(x => x.ID_IMPUTADO == _EstudioPadre.ID_IMPUTADO && x.ID_INGRESO == _EstudioPadre.ID_INGRESO && x.ID_ESTUDIO == _EstudioPadre.ID_ESTUDIO && x.ID_CENTRO == _EstudioPadre.ID_CENTRO);
                        var personalidadFederal = new PERSONALIDAD_FUERO_FEDERAL()
                        {
                            ID_ANIO = _EstudioPadre.ID_ANIO,
                            ID_IMPUTADO = _EstudioPadre.ID_IMPUTADO,
                            ID_CENTRO = _EstudioPadre.ID_CENTRO,
                            ID_ESTUDIO = _EstudioPadre.ID_ESTUDIO,
                            ID_INGRESO = _EstudioPadre.ID_INGRESO
                        };

                        var _Activida = new PFF_ACTIVIDAD()
                        {
                            ALFABE_PRIMARIA = Entity.ALFABE_PRIMARIA,
                            ASISTE_PUNTUAL = Entity.ASISTE_PUNTUAL,
                            ASISTE_PUNTUAL_NO_POR_QUE = Entity.ASISTE_PUNTUAL_NO_POR_QUE,
                            AVANCE_RENDIMIENTO_ACADEMINCO = Entity.AVANCE_RENDIMIENTO_ACADEMINCO,
                            BACHILLER_UNI = Entity.BACHILLER_UNI,
                            CONCLUSIONES = Entity.CONCLUSIONES,
                            DIRECTOR_CENTRO = Entity.DIRECTOR_CENTRO,
                            ESCOLARIDAD_MOMENTO = Entity.ESCOLARIDAD_MOMENTO != -1 ? Entity.ESCOLARIDAD_MOMENTO : null,
                            ESPECIFIQUE = Entity.ESPECIFIQUE,
                            ESTUDIOS_ACTUALES = Entity.ESTUDIOS_ACTUALES,
                            ESTUDIOS_EN_INTERNAMIENTO = Entity.ESTUDIOS_EN_INTERNAMIENTO,
                            FECHA = Entity.FECHA,
                            ID_ANIO = _Actual.ID_ANIO,
                            ID_CENTRO = _Actual.ID_CENTRO,
                            ID_INGRESO = _Actual.ID_INGRESO,
                            ID_ESTUDIO = _Actual.ID_ESTUDIO,
                            OTROS_PROGRAMAS = Entity.OTROS_PROGRAMAS,
                            ID_IMPUTADO = _Actual.ID_IMPUTADO,
                            IMPARTIDO_ENSENANZA = Entity.IMPARTIDO_ENSENANZA,
                            IMPARTIDO_ENSENANZA_TIEMPO = Entity.IMPARTIDO_ENSENANZA_TIEMPO,
                            IMPARTIDO_ENSENANZA_TIPO = Entity.IMPARTIDO_ENSENANZA_TIPO,
                            JEFE_SECC_EDUCATIVA = Entity.JEFE_SECC_EDUCATIVA,
                            LUGAR = Entity.LUGAR,
                            NOMBRE = Entity.NOMBRE,
                            OTRA_ENSENANZA = Entity.OTRA_ENSENANZA,
                            OTRO = Entity.OTRO,
                            PRIMARIA_SECU = Entity.PRIMARIA_SECU,
                            PROMOVIDO = Entity.PROMOVIDO,
                            SECU_BACHILLER = Entity.SECU_BACHILLER
                        };

                        if (Entity.PFF_ACTIVIDAD_PARTICIPACION != null && Entity.PFF_ACTIVIDAD_PARTICIPACION.Any())
                            foreach (var item in Entity.PFF_ACTIVIDAD_PARTICIPACION)
                            {
                                var _Detalle = new PFF_ACTIVIDAD_PARTICIPACION()
                                {
                                    FECHA_1 = item.FECHA_1,
                                    FECHA_2 = item.FECHA_2,
                                    ID_ANIO = _Actual.ID_ANIO,
                                    ID_IMPUTADO = _Actual.ID_IMPUTADO,
                                    ID_CENTRO = _Actual.ID_CENTRO,
                                    ID_ESTUDIO = _Actual.ID_ESTUDIO,
                                    ID_INGRESO = _Actual.ID_INGRESO,
                                    ID_TIPO_PROGRAMA = item.ID_TIPO_PROGRAMA,
                                    OTRO_ESPECIFICAR = item.OTRO_ESPECIFICAR,
                                    PARTICIPACION = item.PARTICIPACION
                                };

                                Context.PFF_ACTIVIDAD_PARTICIPACION.Add(_Detalle);
                            };

                        Context.PERSONALIDAD_FUERO_FEDERAL.Add(personalidadFederal);
                        Context.PFF_ACTIVIDAD.Add(_Activida);
                    }
                    else
                    {
                        var _ACtivFed = _Actual.PFF_ACTIVIDAD;
                        if (_ACtivFed == null)
                        {
                            _ACtivFed = new PFF_ACTIVIDAD()
                            {
                                ALFABE_PRIMARIA = Entity.ALFABE_PRIMARIA,
                                ASISTE_PUNTUAL = Entity.ASISTE_PUNTUAL,
                                ASISTE_PUNTUAL_NO_POR_QUE = Entity.ASISTE_PUNTUAL_NO_POR_QUE,
                                AVANCE_RENDIMIENTO_ACADEMINCO = Entity.AVANCE_RENDIMIENTO_ACADEMINCO,
                                BACHILLER_UNI = Entity.BACHILLER_UNI,
                                CONCLUSIONES = Entity.CONCLUSIONES,
                                DIRECTOR_CENTRO = Entity.DIRECTOR_CENTRO,
                                ESCOLARIDAD_MOMENTO = Entity.ESCOLARIDAD_MOMENTO != -1 ? Entity.ESCOLARIDAD_MOMENTO : null,
                                ESPECIFIQUE = Entity.ESPECIFIQUE,
                                ESTUDIOS_ACTUALES = Entity.ESTUDIOS_ACTUALES,
                                ESTUDIOS_EN_INTERNAMIENTO = Entity.ESTUDIOS_EN_INTERNAMIENTO,
                                FECHA = Entity.FECHA,
                                ID_ANIO = _Actual.ID_ANIO,
                                ID_CENTRO = _Actual.ID_CENTRO,
                                ID_INGRESO = _Actual.ID_INGRESO,
                                ID_ESTUDIO = _Actual.ID_ESTUDIO,
                                ID_IMPUTADO = _Actual.ID_IMPUTADO,
                                IMPARTIDO_ENSENANZA = Entity.IMPARTIDO_ENSENANZA,
                                IMPARTIDO_ENSENANZA_TIEMPO = Entity.IMPARTIDO_ENSENANZA_TIEMPO,
                                IMPARTIDO_ENSENANZA_TIPO = Entity.IMPARTIDO_ENSENANZA_TIPO,
                                JEFE_SECC_EDUCATIVA = Entity.JEFE_SECC_EDUCATIVA,
                                LUGAR = Entity.LUGAR,
                                NOMBRE = Entity.NOMBRE,
                                OTRA_ENSENANZA = Entity.OTRA_ENSENANZA,
                                OTRO = Entity.OTRO,
                                PRIMARIA_SECU = Entity.PRIMARIA_SECU,
                                PROMOVIDO = Entity.PROMOVIDO,
                                OTROS_PROGRAMAS = Entity.OTROS_PROGRAMAS,
                                SECU_BACHILLER = Entity.SECU_BACHILLER
                            };

                            if (Entity.PFF_ACTIVIDAD_PARTICIPACION != null && Entity.PFF_ACTIVIDAD_PARTICIPACION.Any())
                                foreach (var item in Entity.PFF_ACTIVIDAD_PARTICIPACION)
                                {
                                    var _Detalle = new PFF_ACTIVIDAD_PARTICIPACION()
                                    {
                                        FECHA_1 = item.FECHA_1,
                                        FECHA_2 = item.FECHA_2,
                                        ID_ANIO = _Actual.ID_ANIO,
                                        ID_IMPUTADO = _Actual.ID_IMPUTADO,
                                        ID_CENTRO = _Actual.ID_CENTRO,
                                        ID_ESTUDIO = _Actual.ID_ESTUDIO,
                                        ID_INGRESO = _Actual.ID_INGRESO,
                                        ID_TIPO_PROGRAMA = item.ID_TIPO_PROGRAMA,
                                        OTRO_ESPECIFICAR = item.OTRO_ESPECIFICAR,
                                        PARTICIPACION = item.PARTICIPACION
                                    };

                                    Context.PFF_ACTIVIDAD_PARTICIPACION.Add(_Detalle);
                                };

                            Context.PFF_ACTIVIDAD.Add(_ACtivFed);
                        }
                        else
                        {
                            _ACtivFed.ALFABE_PRIMARIA = Entity.ALFABE_PRIMARIA;
                            _ACtivFed.ASISTE_PUNTUAL = Entity.ASISTE_PUNTUAL;
                            _ACtivFed.ASISTE_PUNTUAL_NO_POR_QUE = Entity.ASISTE_PUNTUAL_NO_POR_QUE;
                            _ACtivFed.AVANCE_RENDIMIENTO_ACADEMINCO = Entity.AVANCE_RENDIMIENTO_ACADEMINCO;
                            _ACtivFed.BACHILLER_UNI = Entity.BACHILLER_UNI;
                            _ACtivFed.CONCLUSIONES = Entity.CONCLUSIONES;
                            _ACtivFed.DIRECTOR_CENTRO = Entity.DIRECTOR_CENTRO;
                            _ACtivFed.ESCOLARIDAD_MOMENTO = Entity.ESCOLARIDAD_MOMENTO != -1 ? Entity.ESCOLARIDAD_MOMENTO : null;
                            _ACtivFed.ESPECIFIQUE = Entity.ESPECIFIQUE;
                            _ACtivFed.ESTUDIOS_ACTUALES = Entity.ESTUDIOS_ACTUALES;
                            _ACtivFed.ESTUDIOS_EN_INTERNAMIENTO = Entity.ESTUDIOS_EN_INTERNAMIENTO;
                            _ACtivFed.FECHA = Entity.FECHA;
                            _ACtivFed.ID_ANIO = _Actual.ID_ANIO;
                            _ACtivFed.ID_CENTRO = _Actual.ID_CENTRO;
                            _ACtivFed.ID_ESTUDIO = _Actual.ID_ESTUDIO;
                            _ACtivFed.ID_IMPUTADO = _Actual.ID_IMPUTADO;
                            _ACtivFed.ID_INGRESO = _Actual.ID_INGRESO;
                            _ACtivFed.IMPARTIDO_ENSENANZA = Entity.IMPARTIDO_ENSENANZA;
                            _ACtivFed.IMPARTIDO_ENSENANZA_TIEMPO = Entity.IMPARTIDO_ENSENANZA_TIEMPO;
                            _ACtivFed.IMPARTIDO_ENSENANZA_TIPO = Entity.IMPARTIDO_ENSENANZA_TIPO;
                            _ACtivFed.JEFE_SECC_EDUCATIVA = Entity.JEFE_SECC_EDUCATIVA;
                            _ACtivFed.LUGAR = Entity.LUGAR;
                            _ACtivFed.NOMBRE = Entity.NOMBRE;
                            _ACtivFed.OTRA_ENSENANZA = Entity.OTRA_ENSENANZA;
                            _ACtivFed.OTRO = Entity.OTRO;
                            _ACtivFed.PRIMARIA_SECU = Entity.PRIMARIA_SECU;
                            _ACtivFed.PROMOVIDO = Entity.PROMOVIDO;
                            _ACtivFed.OTROS_PROGRAMAS = Entity.OTROS_PROGRAMAS;
                            _ACtivFed.SECU_BACHILLER = Entity.SECU_BACHILLER;
                            Context.Entry(_ACtivFed).State = System.Data.EntityState.Modified;

                            var _DetalleActivParticipacion = Context.PFF_ACTIVIDAD_PARTICIPACION.Where(x => x.ID_INGRESO == _EstudioPadre.ID_INGRESO && x.ID_IMPUTADO == _EstudioPadre.ID_IMPUTADO && x.ID_ESTUDIO == _EstudioPadre.ID_ESTUDIO && x.ID_CENTRO == _EstudioPadre.ID_CENTRO && x.ID_ANIO == _EstudioPadre.ID_ANIO);

                            if (_DetalleActivParticipacion != null && _DetalleActivParticipacion.Any())
                                foreach (var item in _DetalleActivParticipacion)
                                    Context.Entry(item).State = System.Data.EntityState.Deleted;

                            if (Entity.PFF_ACTIVIDAD_PARTICIPACION != null && Entity.PFF_ACTIVIDAD_PARTICIPACION.Any())
                                foreach (var item in Entity.PFF_ACTIVIDAD_PARTICIPACION)
                                {
                                    var _Detalle = new PFF_ACTIVIDAD_PARTICIPACION()
                                    {
                                        FECHA_1 = item.FECHA_1,
                                        FECHA_2 = item.FECHA_2,
                                        ID_ANIO = _Actual.ID_ANIO,
                                        ID_IMPUTADO = _Actual.ID_IMPUTADO,
                                        ID_CENTRO = _Actual.ID_CENTRO,
                                        ID_ESTUDIO = _Actual.ID_ESTUDIO,
                                        ID_INGRESO = _Actual.ID_INGRESO,
                                        ID_TIPO_PROGRAMA = item.ID_TIPO_PROGRAMA,
                                        OTRO_ESPECIFICAR = item.OTRO_ESPECIFICAR,
                                        PARTICIPACION = item.PARTICIPACION
                                    };

                                    Context.PFF_ACTIVIDAD_PARTICIPACION.Add(_Detalle);
                                };
                        }
                    }


                    #region Detalle
                    if (_EstudioPadre != null)
                    {
                        short? _Estatus = new short?();
                        if (string.IsNullOrEmpty(Entity.ASISTE_PUNTUAL) || string.IsNullOrEmpty(Entity.AVANCE_RENDIMIENTO_ACADEMINCO) || Entity.ESCOLARIDAD_MOMENTO == null ||
                            string.IsNullOrEmpty(Entity.ESTUDIOS_ACTUALES) || string.IsNullOrEmpty(Entity.ESTUDIOS_EN_INTERNAMIENTO) ||
                            string.IsNullOrEmpty(Entity.IMPARTIDO_ENSENANZA) || string.IsNullOrEmpty(Entity.NOMBRE) || string.IsNullOrEmpty(Entity.PROMOVIDO) || Entity.FECHA == null)
                            _Estatus = (short)eEstatudDetallePersonalidad.ASIGNADO;
                        else
                            _Estatus = (short)eEstatudDetallePersonalidad.TERMINADO;

                        var DesarrolloTrabajoFederal = Context.PERSONALIDAD_DETALLE.FirstOrDefault(x => x.ID_IMPUTADO == _EstudioPadre.ID_IMPUTADO && x.ID_ESTUDIO == _EstudioPadre.ID_ESTUDIO && x.ID_INGRESO == _EstudioPadre.ID_INGRESO && x.ID_TIPO == (short)eTiposEstudio.PEDAGOGICA_FEDERAL && x.ID_CENTRO == _EstudioPadre.ID_CENTRO && x.ID_ANIO == _EstudioPadre.ID_ANIO);
                        if (DesarrolloTrabajoFederal == null)
                        {
                            short _ConsecutivoPersonalidadDetalle = GetIDProceso<short>("PERSONALIDAD_DETALLE", "ID_DETALLE", "1=1");
                            var NvoDetalle = new PERSONALIDAD_DETALLE()
                            {
                                ID_ANIO = _EstudioPadre.ID_ANIO,
                                ID_CENTRO = _EstudioPadre.ID_CENTRO,
                                ID_DETALLE = _ConsecutivoPersonalidadDetalle,
                                ID_ESTUDIO = _EstudioPadre.ID_ESTUDIO,
                                ID_IMPUTADO = _EstudioPadre.ID_IMPUTADO,
                                ID_ESTATUS = _Estatus,
                                ID_INGRESO = _EstudioPadre.ID_INGRESO,
                                ID_TIPO = (short)eTiposEstudio.PEDAGOGICA_FEDERAL,
                                INICIO_FEC = GetFechaServerDate(),
                                RESULTADO = _Estatus == (short)eEstatudDetallePersonalidad.TERMINADO ? "S" : "N",
                                SOLICITUD_FEC = null,
                                TERMINO_FEC = _Estatus == (short)eEstatudDetallePersonalidad.TERMINADO ? GetFechaServerDate() : new System.DateTime?(),
                                TIPO_MEDIA = string.Empty
                            };

                            Context.PERSONALIDAD_DETALLE.Add(NvoDetalle);
                        }
                        else
                        {
                            DesarrolloTrabajoFederal.RESULTADO = _Estatus == (short)eEstatudDetallePersonalidad.TERMINADO ? "S" : "N";
                            DesarrolloTrabajoFederal.ID_ESTATUS = _Estatus;
                            Context.Entry(DesarrolloTrabajoFederal).State = System.Data.EntityState.Modified;
                        }
                    };

                    #endregion

                    Context.SaveChanges();
                    transaccion.Complete();
                    return 1;
                }
            }
            catch (System.Data.Entity.Validation.DbEntityValidationException dbEx)
            {
                foreach (var validationErrors in dbEx.EntityValidationErrors)
                    foreach (var validationError in validationErrors.ValidationErrors)
                        System.Diagnostics.Trace.TraceInformation("Nombre del causante de excepción: {0} Error: {1}", validationError.PropertyName, validationError.ErrorMessage);

                return 2;
            }

            catch (System.Exception exc)
            {
                return 0;
            }
        }

        public short GuardarEducativoFederalAislado(PFF_CAPACITACION Entity)
        {
            try
            {
                using (System.Transactions.TransactionScope transaccion = new System.Transactions.TransactionScope(System.Transactions.TransactionScopeOption.RequiresNew, new System.Transactions.TransactionOptions() { IsolationLevel = System.Transactions.IsolationLevel.ReadCommitted }))
                {
                    var _EstudioPadre = Context.PERSONALIDAD.Where(x => x.ID_INGRESO == Entity.ID_INGRESO && x.ID_IMPUTADO == Entity.ID_IMPUTADO && x.ID_CENTRO == Entity.ID_CENTRO && x.ID_ANIO == Entity.ID_ANIO).OrderByDescending(x => x.ID_ESTUDIO).FirstOrDefault();

                    if (_EstudioPadre == null)
                        return 0;

                    var _Actual = Context.PERSONALIDAD_FUERO_FEDERAL.FirstOrDefault(x => x.ID_ANIO == _EstudioPadre.ID_ANIO && x.ID_CENTRO == _EstudioPadre.ID_CENTRO && x.ID_IMPUTADO == _EstudioPadre.ID_IMPUTADO && x.ID_INGRESO == _EstudioPadre.ID_INGRESO && x.ID_ESTUDIO == _EstudioPadre.ID_ESTUDIO);

                    if (_Actual == null)
                    {
                        var _EstudioTS = Context.PFF_CAPACITACION.FirstOrDefault(x => x.ID_IMPUTADO == _EstudioPadre.ID_IMPUTADO && x.ID_INGRESO == _EstudioPadre.ID_INGRESO && x.ID_ESTUDIO == _EstudioPadre.ID_ESTUDIO && x.ID_CENTRO == _EstudioPadre.ID_CENTRO);
                        var personalidadFederal = new PERSONALIDAD_FUERO_FEDERAL()
                        {
                            ID_ANIO = _EstudioPadre.ID_ANIO,
                            ID_IMPUTADO = _EstudioPadre.ID_IMPUTADO,
                            ID_CENTRO = _EstudioPadre.ID_CENTRO,
                            ID_ESTUDIO = _EstudioPadre.ID_ESTUDIO,
                            ID_INGRESO = _EstudioPadre.ID_INGRESO
                        };

                        var _capac = new PFF_CAPACITACION()
                        {
                            A_TOTAL_DIAS_LABORADOS = Entity.A_TOTAL_DIAS_LABORADOS,
                            ACTITUDES_DESEMPENO_ACT = Entity.ACTITUDES_DESEMPENO_ACT,
                            ACTIVIDAD_PRODUC_ACTUAL = Entity.ACTIVIDAD_PRODUC_ACTUAL,
                            ATIENDE_INDICACIONES = Entity.ATIENDE_INDICACIONES,
                            B_DIAS_LABORADOS_OTROS_CERESOS = Entity.B_DIAS_LABORADOS_OTROS_CERESOS,
                            CAMBIO_ACTIVIDAD = Entity.CAMBIO_ACTIVIDAD,
                            CAMBIO_ACTIVIDAD_POR_QUE = Entity.CAMBIO_ACTIVIDAD_POR_QUE,
                            CONCLUSIONES = Entity.CONCLUSIONES,
                            DESCUIDADO_LABORES = Entity.DESCUIDADO_LABORES,
                            DIRECTOR_CENTRO = Entity.DIRECTOR_CENTRO,
                            FECHA = Entity.FECHA,
                            FONDO_AHORRO = Entity.FONDO_AHORRO,
                            FONDO_AHORRO_COMPESACION_ACTUA = Entity.FONDO_AHORRO_COMPESACION_ACTUA,
                            HA_PROGRESADO_OFICIO = Entity.HA_PROGRESADO_OFICIO,
                            ID_ANIO = _Actual.ID_ANIO,
                            ID_CENTRO = _Actual.ID_CENTRO,
                            ID_ESTUDIO = _Actual.ID_ESTUDIO,
                            ID_IMPUTADO = _Actual.ID_IMPUTADO,
                            ID_INGRESO = _Actual.ID_INGRESO,
                            JEFE_SECC_INDUSTRIAL = Entity.JEFE_SECC_INDUSTRIAL,
                            LUGAR = Entity.LUGAR,
                            MOTIVO_TIEMPO_INTERRUP_ACT = Entity.MOTIVO_TIEMPO_INTERRUP_ACT,
                            NO_CURSOS_MOTIVO = Entity.NO_CURSOS_MOTIVO,
                            NOMBRE = Entity.NOMBRE,
                            OFICIO_ANTES_RECLUSION = Entity.OFICIO_ANTES_RECLUSION.HasValue ? Entity.OFICIO_ANTES_RECLUSION != -1 ? Entity.OFICIO_ANTES_RECLUSION : null : null,
                            RECIBIO_CONSTANCIA = Entity.RECIBIO_CONSTANCIA,
                            SALARIO_DEVENGABA_DETENCION = Entity.SALARIO_DEVENGABA_DETENCION,
                            SATISFACE_ACTIVIDAD = Entity.SATISFACE_ACTIVIDAD,
                            SECCION = Entity.SECCION,
                            TOTAL_A_B = Entity.TOTAL_A_B
                        };

                        if (Entity.PFF_DIAS_LABORADO != null && Entity.PFF_DIAS_LABORADO.Any())
                            foreach (var item in Entity.PFF_DIAS_LABORADO)
                            {
                                var _NvoCapac = new PFF_DIAS_LABORADO()
                                {
                                    ANIO = item.ANIO,
                                    DIAS_TRABAJADOS = item.DIAS_TRABAJADOS,
                                    ID_ANIO = _Actual.ID_ANIO,
                                    ID_IMPUTADO = _Actual.ID_IMPUTADO,
                                    ID_CENTRO = _Actual.ID_CENTRO,
                                    ID_ESTUDIO = _Actual.ID_ESTUDIO,
                                    ID_INGRESO = _Actual.ID_INGRESO,
                                    MES = item.MES
                                };

                                Context.PFF_DIAS_LABORADO.Add(_NvoCapac);
                            };

                        var _consecutivoCapacitacionFederal = GetIDProceso<short>("PFF_CAPACITACION_CURSO", "ID_CONSEC", string.Format("ID_CENTRO = {0} AND ID_ANIO = {1} AND ID_IMPUTADO = {2} AND ID_INGRESO = {3} AND ID_ESTUDIO = {4}", _EstudioPadre.ID_CENTRO, _EstudioPadre.ID_ANIO, _EstudioPadre.ID_IMPUTADO, _EstudioPadre.ID_INGRESO, _EstudioPadre.ID_ESTUDIO));
                        if (Entity.PFF_CAPACITACION_CURSO != null && Entity.PFF_CAPACITACION_CURSO.Any())
                            foreach (var item in Entity.PFF_CAPACITACION_CURSO)
                            {
                                var _NvaCurso = new PFF_CAPACITACION_CURSO()
                                {
                                    CURSO = item.CURSO,
                                    FECHA_INICIO = item.FECHA_INICIO,
                                    ID_CONSEC = _consecutivoCapacitacionFederal,
                                    FECHA_TERMINO = item.FECHA_TERMINO,
                                    ID_ESTUDIO = _Actual.ID_ESTUDIO,
                                    ID_ANIO = _Actual.ID_ANIO,
                                    ID_IMPUTADO = _Actual.ID_IMPUTADO,
                                    ID_CENTRO = _Actual.ID_CENTRO,
                                    ID_INGRESO = _Actual.ID_INGRESO
                                };

                                Context.PFF_CAPACITACION_CURSO.Add(_NvaCurso);
                                _consecutivoCapacitacionFederal++;
                            };

                        Context.PERSONALIDAD_FUERO_FEDERAL.Add(personalidadFederal);
                        Context.PFF_CAPACITACION.Add(_capac);
                    }

                    else
                    {
                        var _CapaFed = _Actual.PFF_CAPACITACION;
                        if (_CapaFed == null)
                        {
                            _CapaFed = new PFF_CAPACITACION()
                            {
                                A_TOTAL_DIAS_LABORADOS = Entity.A_TOTAL_DIAS_LABORADOS,
                                ACTITUDES_DESEMPENO_ACT = Entity.ACTITUDES_DESEMPENO_ACT,
                                ACTIVIDAD_PRODUC_ACTUAL = Entity.ACTIVIDAD_PRODUC_ACTUAL,
                                ATIENDE_INDICACIONES = Entity.ATIENDE_INDICACIONES,
                                B_DIAS_LABORADOS_OTROS_CERESOS = Entity.B_DIAS_LABORADOS_OTROS_CERESOS,
                                CAMBIO_ACTIVIDAD = Entity.CAMBIO_ACTIVIDAD,
                                CAMBIO_ACTIVIDAD_POR_QUE = Entity.CAMBIO_ACTIVIDAD_POR_QUE,
                                CONCLUSIONES = Entity.CONCLUSIONES,
                                DESCUIDADO_LABORES = Entity.DESCUIDADO_LABORES,
                                DIRECTOR_CENTRO = Entity.DIRECTOR_CENTRO,
                                FECHA = Entity.FECHA,
                                FONDO_AHORRO = Entity.FONDO_AHORRO,
                                FONDO_AHORRO_COMPESACION_ACTUA = Entity.FONDO_AHORRO_COMPESACION_ACTUA,
                                HA_PROGRESADO_OFICIO = Entity.HA_PROGRESADO_OFICIO,
                                ID_ANIO = _Actual.ID_ANIO,
                                ID_CENTRO = _Actual.ID_CENTRO,
                                ID_ESTUDIO = _Actual.ID_ESTUDIO,
                                ID_IMPUTADO = _Actual.ID_IMPUTADO,
                                ID_INGRESO = _Actual.ID_INGRESO,
                                JEFE_SECC_INDUSTRIAL = Entity.JEFE_SECC_INDUSTRIAL,
                                LUGAR = Entity.LUGAR,
                                MOTIVO_TIEMPO_INTERRUP_ACT = Entity.MOTIVO_TIEMPO_INTERRUP_ACT,
                                NO_CURSOS_MOTIVO = Entity.NO_CURSOS_MOTIVO,
                                NOMBRE = Entity.NOMBRE,
                                OFICIO_ANTES_RECLUSION = Entity.OFICIO_ANTES_RECLUSION.HasValue ? Entity.OFICIO_ANTES_RECLUSION != -1 ? Entity.OFICIO_ANTES_RECLUSION : null : null,
                                RECIBIO_CONSTANCIA = Entity.RECIBIO_CONSTANCIA,
                                SALARIO_DEVENGABA_DETENCION = Entity.SALARIO_DEVENGABA_DETENCION,
                                SATISFACE_ACTIVIDAD = Entity.SATISFACE_ACTIVIDAD,
                                SECCION = Entity.SECCION,
                                TOTAL_A_B = Entity.TOTAL_A_B
                            };

                            if (Entity.PFF_DIAS_LABORADO != null && Entity.PFF_DIAS_LABORADO.Any())
                                foreach (var item in Entity.PFF_DIAS_LABORADO)
                                {
                                    var _NvoCapac = new PFF_DIAS_LABORADO()
                                    {
                                        ANIO = item.ANIO,
                                        DIAS_TRABAJADOS = item.DIAS_TRABAJADOS,
                                        ID_ANIO = _Actual.ID_ANIO,
                                        ID_IMPUTADO = _Actual.ID_IMPUTADO,
                                        ID_CENTRO = _Actual.ID_CENTRO,
                                        ID_ESTUDIO = _Actual.ID_ESTUDIO,
                                        ID_INGRESO = _Actual.ID_INGRESO,
                                        MES = item.MES
                                    };

                                    _CapaFed.PFF_DIAS_LABORADO.Add(_NvoCapac);
                                };

                            var _consecutivoCapacitacionFederal = GetIDProceso<short>("PFF_CAPACITACION_CURSO", "ID_CONSEC", string.Format("ID_CENTRO = {0} AND ID_ANIO = {1} AND ID_IMPUTADO = {2} AND ID_INGRESO = {3} AND ID_ESTUDIO = {4}", _EstudioPadre.ID_CENTRO, _EstudioPadre.ID_ANIO, _EstudioPadre.ID_IMPUTADO, _EstudioPadre.ID_INGRESO, _EstudioPadre.ID_ESTUDIO));
                            if (Entity.PFF_CAPACITACION_CURSO != null && Entity.PFF_CAPACITACION_CURSO.Any())
                                foreach (var item in Entity.PFF_CAPACITACION_CURSO)
                                {
                                    var _NvaCurso = new PFF_CAPACITACION_CURSO()
                                    {
                                        CURSO = item.CURSO,
                                        FECHA_INICIO = item.FECHA_INICIO,
                                        ID_CONSEC = _consecutivoCapacitacionFederal,
                                        FECHA_TERMINO = item.FECHA_TERMINO,
                                        ID_ESTUDIO = _Actual.ID_ESTUDIO,
                                        ID_ANIO = _Actual.ID_ANIO,
                                        ID_IMPUTADO = _Actual.ID_IMPUTADO,
                                        ID_CENTRO = _Actual.ID_CENTRO,
                                        ID_INGRESO = _Actual.ID_INGRESO
                                    };

                                    _CapaFed.PFF_CAPACITACION_CURSO.Add(_NvaCurso);
                                    _consecutivoCapacitacionFederal++;
                                };

                            Context.PFF_CAPACITACION.Add(_CapaFed);
                        }
                        else
                        {

                            var _DiasGuardados = Context.PFF_DIAS_LABORADO.Where(x => x.ID_INGRESO == _EstudioPadre.ID_INGRESO && x.ID_IMPUTADO == _EstudioPadre.ID_IMPUTADO && x.ID_ESTUDIO == _EstudioPadre.ID_ESTUDIO && x.ID_ANIO == _EstudioPadre.ID_ANIO && x.ID_CENTRO == _EstudioPadre.ID_CENTRO);
                            var _GruposCapac = Context.PFF_CAPACITACION_CURSO.Where(x => x.ID_INGRESO == _EstudioPadre.ID_INGRESO && x.ID_IMPUTADO == _EstudioPadre.ID_IMPUTADO && x.ID_ESTUDIO == _EstudioPadre.ID_ESTUDIO && x.ID_ANIO == _EstudioPadre.ID_ANIO && x.ID_CENTRO == _EstudioPadre.ID_CENTRO);

                            _CapaFed.A_TOTAL_DIAS_LABORADOS = Entity.A_TOTAL_DIAS_LABORADOS;
                            _CapaFed.ACTITUDES_DESEMPENO_ACT = Entity.ACTITUDES_DESEMPENO_ACT;
                            _CapaFed.ACTIVIDAD_PRODUC_ACTUAL = Entity.ACTIVIDAD_PRODUC_ACTUAL;
                            _CapaFed.ATIENDE_INDICACIONES = Entity.ATIENDE_INDICACIONES;
                            _CapaFed.B_DIAS_LABORADOS_OTROS_CERESOS = Entity.B_DIAS_LABORADOS_OTROS_CERESOS;
                            _CapaFed.CAMBIO_ACTIVIDAD = Entity.CAMBIO_ACTIVIDAD;
                            _CapaFed.CAMBIO_ACTIVIDAD_POR_QUE = Entity.CAMBIO_ACTIVIDAD_POR_QUE;
                            _CapaFed.CONCLUSIONES = Entity.CONCLUSIONES;
                            _CapaFed.DESCUIDADO_LABORES = Entity.DESCUIDADO_LABORES;
                            _CapaFed.DIRECTOR_CENTRO = Entity.DIRECTOR_CENTRO;
                            _CapaFed.FECHA = Entity.FECHA;
                            _CapaFed.FONDO_AHORRO = Entity.FONDO_AHORRO;
                            _CapaFed.FONDO_AHORRO_COMPESACION_ACTUA = Entity.FONDO_AHORRO_COMPESACION_ACTUA;
                            _CapaFed.HA_PROGRESADO_OFICIO = Entity.HA_PROGRESADO_OFICIO;
                            _CapaFed.ID_ANIO = _Actual.ID_ANIO;
                            _CapaFed.ID_CENTRO = _Actual.ID_CENTRO;
                            _CapaFed.ID_ESTUDIO = _Actual.ID_ESTUDIO;
                            _CapaFed.ID_IMPUTADO = _Actual.ID_IMPUTADO;
                            _CapaFed.ID_INGRESO = _Actual.ID_INGRESO;
                            _CapaFed.JEFE_SECC_INDUSTRIAL = Entity.JEFE_SECC_INDUSTRIAL;
                            _CapaFed.LUGAR = Entity.LUGAR;
                            _CapaFed.MOTIVO_TIEMPO_INTERRUP_ACT = Entity.MOTIVO_TIEMPO_INTERRUP_ACT;
                            _CapaFed.NO_CURSOS_MOTIVO = Entity.NO_CURSOS_MOTIVO;
                            _CapaFed.NOMBRE = Entity.NOMBRE;
                            _CapaFed.OFICIO_ANTES_RECLUSION = Entity.OFICIO_ANTES_RECLUSION.HasValue ? Entity.OFICIO_ANTES_RECLUSION != -1 ? Entity.OFICIO_ANTES_RECLUSION : null : null;
                            _CapaFed.RECIBIO_CONSTANCIA = Entity.RECIBIO_CONSTANCIA;
                            _CapaFed.SALARIO_DEVENGABA_DETENCION = Entity.SALARIO_DEVENGABA_DETENCION;
                            _CapaFed.SATISFACE_ACTIVIDAD = Entity.SATISFACE_ACTIVIDAD;
                            _CapaFed.SECCION = Entity.SECCION;
                            _CapaFed.TOTAL_A_B = Entity.TOTAL_A_B;
                            Context.Entry(_CapaFed).State = System.Data.EntityState.Modified;

                            if (_DiasGuardados != null && _DiasGuardados.Any())
                                foreach (var item in _DiasGuardados)
                                    Context.Entry(item).State = System.Data.EntityState.Deleted;

                            if (_GruposCapac != null && _GruposCapac.Any())
                                foreach (var item in _GruposCapac)
                                    Context.Entry(item).State = System.Data.EntityState.Deleted;

                            if (Entity.PFF_DIAS_LABORADO != null && Entity.PFF_DIAS_LABORADO.Any())
                                foreach (var item in Entity.PFF_DIAS_LABORADO)
                                {
                                    var _NvoCapac = new PFF_DIAS_LABORADO()
                                    {
                                        ANIO = item.ANIO,
                                        DIAS_TRABAJADOS = item.DIAS_TRABAJADOS,
                                        ID_ANIO = _Actual.ID_ANIO,
                                        ID_CENTRO = _Actual.ID_CENTRO,
                                        ID_ESTUDIO = _Actual.ID_ESTUDIO,
                                        ID_IMPUTADO = _Actual.ID_IMPUTADO,
                                        ID_INGRESO = _Actual.ID_INGRESO,
                                        MES = item.MES
                                    };

                                    Context.PFF_DIAS_LABORADO.Add(_NvoCapac);
                                };

                            var _consecutivoCapacitacionFederal = GetIDProceso<short>("PFF_CAPACITACION_CURSO", "ID_CONSEC", string.Format("ID_CENTRO = {0} AND ID_ANIO = {1} AND ID_IMPUTADO = {2} AND ID_INGRESO = {3} AND ID_ESTUDIO = {4}", _EstudioPadre.ID_CENTRO, _EstudioPadre.ID_ANIO, _EstudioPadre.ID_IMPUTADO, _EstudioPadre.ID_INGRESO, _EstudioPadre.ID_ESTUDIO));
                            if (Entity.PFF_CAPACITACION_CURSO != null && Entity.PFF_CAPACITACION_CURSO.Any())
                                foreach (var item in Entity.PFF_CAPACITACION_CURSO)
                                {
                                    var _NvaCurso = new PFF_CAPACITACION_CURSO()
                                    {
                                        CURSO = item.CURSO,
                                        FECHA_INICIO = item.FECHA_INICIO,
                                        FECHA_TERMINO = item.FECHA_TERMINO,
                                        ID_ANIO = _Actual.ID_ANIO,
                                        ID_CENTRO = _Actual.ID_CENTRO,
                                        ID_CONSEC = _consecutivoCapacitacionFederal,
                                        ID_ESTUDIO = _Actual.ID_ESTUDIO,
                                        ID_IMPUTADO = _Actual.ID_IMPUTADO,
                                        ID_INGRESO = _Actual.ID_INGRESO
                                    };

                                    Context.PFF_CAPACITACION_CURSO.Add(_NvaCurso);
                                    _consecutivoCapacitacionFederal++;
                                };
                        }
                    }

                    #region Definicion de detalle de estudio de capacitacion de fuero federal
                    if (_EstudioPadre != null)
                    {
                        short? _Estatus = new short?();
                        if (string.IsNullOrEmpty(Entity.ACTITUDES_DESEMPENO_ACT) || string.IsNullOrEmpty(Entity.ACTIVIDAD_PRODUC_ACTUAL) || string.IsNullOrEmpty(Entity.ATIENDE_INDICACIONES) || string.IsNullOrEmpty(Entity.CAMBIO_ACTIVIDAD) ||
                           string.IsNullOrEmpty(Entity.CONCLUSIONES) || string.IsNullOrEmpty(Entity.DESCUIDADO_LABORES) || string.IsNullOrEmpty(Entity.FONDO_AHORRO) || string.IsNullOrEmpty(Entity.HA_PROGRESADO_OFICIO) ||
                           string.IsNullOrEmpty(Entity.NOMBRE) || Entity.OFICIO_ANTES_RECLUSION == null || string.IsNullOrEmpty(Entity.SATISFACE_ACTIVIDAD) || string.IsNullOrEmpty(Entity.SECCION) ||
                           Entity.A_TOTAL_DIAS_LABORADOS == null || Entity.B_DIAS_LABORADOS_OTROS_CERESOS == null || Entity.FECHA == null || Entity.SALARIO_DEVENGABA_DETENCION == null || Entity.TOTAL_A_B == null)
                            _Estatus = (short)eEstatudDetallePersonalidad.ASIGNADO;
                        else
                            _Estatus = (short)eEstatudDetallePersonalidad.TERMINADO;

                        var _DesarrolloCapacitacionFederal = Context.PERSONALIDAD_DETALLE.FirstOrDefault(x => x.ID_IMPUTADO == _EstudioPadre.ID_IMPUTADO && x.ID_ESTUDIO == _EstudioPadre.ID_ESTUDIO && x.ID_INGRESO == _EstudioPadre.ID_INGRESO && x.ID_TIPO == (short)eTiposEstudio.LABORAL_FEDERAL && x.ID_CENTRO == _EstudioPadre.ID_CENTRO && x.ID_ANIO == _EstudioPadre.ID_ANIO);
                        if (_DesarrolloCapacitacionFederal == null)
                        {
                            short _ConsecutivoPersonalidadDetalle = GetIDProceso<short>("PERSONALIDAD_DETALLE", "ID_DETALLE", "1=1");
                            var NvoDetalle = new PERSONALIDAD_DETALLE()
                            {
                                ID_ANIO = _EstudioPadre.ID_ANIO,
                                ID_CENTRO = _EstudioPadre.ID_CENTRO,
                                ID_DETALLE = _ConsecutivoPersonalidadDetalle,
                                ID_ESTUDIO = _EstudioPadre.ID_ESTUDIO,
                                ID_ESTATUS = _Estatus,
                                ID_IMPUTADO = _EstudioPadre.ID_IMPUTADO,
                                ID_INGRESO = _EstudioPadre.ID_INGRESO,
                                ID_TIPO = (short)eTiposEstudio.LABORAL_FEDERAL,
                                INICIO_FEC = GetFechaServerDate(),
                                RESULTADO = _Estatus == (short)eEstatudDetallePersonalidad.TERMINADO ? "S" : "N",
                                SOLICITUD_FEC = null,
                                TERMINO_FEC = _Estatus == (short)eEstatudDetallePersonalidad.TERMINADO ? GetFechaServerDate() : new System.DateTime?(),
                                TIPO_MEDIA = string.Empty
                            };

                            Context.PERSONALIDAD_DETALLE.Add(NvoDetalle);
                        }
                        else
                        {
                            _DesarrolloCapacitacionFederal.RESULTADO = _Estatus == (short)eEstatudDetallePersonalidad.TERMINADO ? "S" : "N";
                            _DesarrolloCapacitacionFederal.ID_ESTATUS = _Estatus;
                            Context.Entry(_DesarrolloCapacitacionFederal).State = System.Data.EntityState.Modified;
                        }
                    }

                    #endregion

                    Context.SaveChanges();
                    transaccion.Complete();
                    return 1;
                }
            }
            catch (System.Data.Entity.Validation.DbEntityValidationException dbEx)
            {
                foreach (var validationErrors in dbEx.EntityValidationErrors)
                    foreach (var validationError in validationErrors.ValidationErrors)
                        System.Diagnostics.Trace.TraceInformation("Nombre del causante de excepción: {0} Error: {1}", validationError.PropertyName, validationError.ErrorMessage);

                return 2;
            }

            catch (System.Exception exc)
            {
                return 0;
            }
        }

        public short GuardaVigilanciaFederalAislado(PFF_VIGILANCIA Entity)
        {
            try
            {
                using (System.Transactions.TransactionScope transaccion = new System.Transactions.TransactionScope(System.Transactions.TransactionScopeOption.RequiresNew, new System.Transactions.TransactionOptions() { IsolationLevel = System.Transactions.IsolationLevel.ReadCommitted }))
                {
                    var _EstudioPadre = Context.PERSONALIDAD.Where(x => x.ID_INGRESO == Entity.ID_INGRESO && x.ID_IMPUTADO == Entity.ID_IMPUTADO && x.ID_CENTRO == Entity.ID_CENTRO && x.ID_ANIO == Entity.ID_ANIO).OrderByDescending(x => x.ID_ESTUDIO).FirstOrDefault();

                    if (_EstudioPadre == null)
                        return 0;

                    var _Actual = Context.PERSONALIDAD_FUERO_FEDERAL.FirstOrDefault(x => x.ID_ANIO == _EstudioPadre.ID_ANIO && x.ID_CENTRO == _EstudioPadre.ID_CENTRO && x.ID_IMPUTADO == _EstudioPadre.ID_IMPUTADO && x.ID_INGRESO == _EstudioPadre.ID_INGRESO && x.ID_ESTUDIO == _EstudioPadre.ID_ESTUDIO);

                    if (_Actual == null)
                    {
                        var _EstudioVigilanciaFederal = Context.PFF_VIGILANCIA.FirstOrDefault(x => x.ID_IMPUTADO == _EstudioPadre.ID_IMPUTADO && x.ID_INGRESO == _EstudioPadre.ID_INGRESO && x.ID_ESTUDIO == _EstudioPadre.ID_ESTUDIO && x.ID_CENTRO == _EstudioPadre.ID_CENTRO);
                        var personalidadFederal = new PERSONALIDAD_FUERO_FEDERAL()
                        {
                            ID_ANIO = _EstudioPadre.ID_ANIO,
                            ID_IMPUTADO = _EstudioPadre.ID_IMPUTADO,
                            ID_CENTRO = _EstudioPadre.ID_CENTRO,
                            ID_ESTUDIO = _EstudioPadre.ID_ESTUDIO,
                            ID_INGRESO = _EstudioPadre.ID_INGRESO
                        };

                        var _Vigilan = new PFF_VIGILANCIA()
                        {
                            CENTRO_DONDE_PROCEDE = Entity.CENTRO_DONDE_PROCEDE,
                            CONCLUSIONES = Entity.CONCLUSIONES,
                            CONDUCTA = Entity.CONDUCTA,
                            CONDUCTA_FAMILIA = Entity.CONDUCTA_FAMILIA,
                            CONDUCTA_GENERAL = Entity.CONDUCTA_GENERAL,
                            CONDUCTA_SUPERIORES = Entity.CONDUCTA_SUPERIORES,
                            DESCRIPCION_CONDUCTA = Entity.DESCRIPCION_CONDUCTA,
                            DIRECTOR_CENTRO = Entity.DIRECTOR_CENTRO,
                            ESTIMULOS_BUENA_CONDUCTA = Entity.ESTIMULOS_BUENA_CONDUCTA,
                            FECHA = Entity.FECHA,
                            FECHA_INGRESO = Entity.FECHA_INGRESO,
                            HIGIENE_CELDA = Entity.HIGIENE_CELDA,
                            HIGIENE_PERSONAL = Entity.HIGIENE_PERSONAL,
                            ID_ANIO = _EstudioPadre.ID_ANIO,
                            ID_CENTRO = _EstudioPadre.ID_CENTRO,
                            ID_ESTUDIO = _EstudioPadre.ID_ESTUDIO,
                            ID_IMPUTADO = _EstudioPadre.ID_IMPUTADO,
                            ID_INGRESO = _EstudioPadre.ID_INGRESO,
                            JEFE_VIGILANCIA = Entity.JEFE_VIGILANCIA,
                            LUGAR = Entity.LUGAR,
                            MOTIVO_TRASLADO = Entity.MOTIVO_TRASLADO,
                            NOMBRE = Entity.NOMBRE,
                            RELACION_COMPANEROS = Entity.RELACION_COMPANEROS,
                            VISITA_FRECUENCIA = Entity.VISITA_FRECUENCIA,
                            VISITA_QUIENES = Entity.VISITA_QUIENES,
                            VISITA_RECIBE = Entity.VISITA_RECIBE
                        };

                        var _consecutivoCapacitacionFederal = GetIDProceso<short>("PFF_CORRECTIVO", "ID_CONSEC", string.Format("ID_CENTRO = {0} AND ID_ANIO = {1} AND ID_IMPUTADO = {2} AND ID_INGRESO = {3} AND ID_ESTUDIO = {4}", _EstudioPadre.ID_CENTRO, _EstudioPadre.ID_ANIO, _EstudioPadre.ID_IMPUTADO, _EstudioPadre.ID_INGRESO, _EstudioPadre.ID_ESTUDIO));
                        if (Entity.PFF_CORRECTIVO != null && Entity.PFF_CORRECTIVO.Any())
                            foreach (var item in Entity.PFF_CORRECTIVO)
                            {
                                var NvaSancion = new PFF_CORRECTIVO()
                                {
                                    FECHA = item.FECHA,
                                    ID_ANIO = _Actual.ID_ANIO,
                                    ID_CENTRO = _Actual.ID_CENTRO,
                                    ID_CONSEC = _consecutivoCapacitacionFederal,
                                    ID_ESTUDIO = _Actual.ID_ESTUDIO,
                                    ID_IMPUTADO = _Actual.ID_IMPUTADO,
                                    ID_INGRESO = _Actual.ID_INGRESO,
                                    MOTIVO = item.MOTIVO,
                                    RESOLUCION = item.RESOLUCION
                                };

                                Context.PFF_CORRECTIVO.Add(NvaSancion);
                                _consecutivoCapacitacionFederal++;
                            };

                        Context.PFF_VIGILANCIA.Add(_Vigilan);
                        Context.PERSONALIDAD_FUERO_FEDERAL.Add(personalidadFederal);
                    }
                    else
                    {
                        var _vigilFedera = _Actual.PFF_VIGILANCIA;
                        if (_vigilFedera == null)
                        {
                            _vigilFedera = new PFF_VIGILANCIA()
                            {
                                CENTRO_DONDE_PROCEDE = Entity.CENTRO_DONDE_PROCEDE,
                                CONCLUSIONES = Entity.CONCLUSIONES,
                                CONDUCTA = Entity.CONDUCTA,
                                CONDUCTA_FAMILIA = Entity.CONDUCTA_FAMILIA,
                                CONDUCTA_GENERAL = Entity.CONDUCTA_GENERAL,
                                CONDUCTA_SUPERIORES = Entity.CONDUCTA_SUPERIORES,
                                DESCRIPCION_CONDUCTA = Entity.DESCRIPCION_CONDUCTA,
                                DIRECTOR_CENTRO = Entity.DIRECTOR_CENTRO,
                                ESTIMULOS_BUENA_CONDUCTA = Entity.ESTIMULOS_BUENA_CONDUCTA,
                                FECHA = Entity.FECHA,
                                FECHA_INGRESO = Entity.FECHA_INGRESO,
                                HIGIENE_CELDA = Entity.HIGIENE_CELDA,
                                HIGIENE_PERSONAL = Entity.HIGIENE_PERSONAL,
                                ID_ANIO = _EstudioPadre.ID_ANIO,
                                ID_CENTRO = _EstudioPadre.ID_CENTRO,
                                ID_ESTUDIO = _EstudioPadre.ID_ESTUDIO,
                                ID_IMPUTADO = _EstudioPadre.ID_IMPUTADO,
                                ID_INGRESO = _EstudioPadre.ID_INGRESO,
                                JEFE_VIGILANCIA = Entity.JEFE_VIGILANCIA,
                                LUGAR = Entity.LUGAR,
                                MOTIVO_TRASLADO = Entity.MOTIVO_TRASLADO,
                                NOMBRE = Entity.NOMBRE,
                                RELACION_COMPANEROS = Entity.RELACION_COMPANEROS,
                                VISITA_FRECUENCIA = Entity.VISITA_FRECUENCIA,
                                VISITA_QUIENES = Entity.VISITA_QUIENES,
                                VISITA_RECIBE = Entity.VISITA_RECIBE
                            };

                            var _consecutivoCapacitacionFederal = GetIDProceso<short>("PFF_CORRECTIVO", "ID_CONSEC", string.Format("ID_CENTRO = {0} AND ID_ANIO = {1} AND ID_IMPUTADO = {2} AND ID_INGRESO = {3} AND ID_ESTUDIO = {4}", _EstudioPadre.ID_CENTRO, _EstudioPadre.ID_ANIO, _EstudioPadre.ID_IMPUTADO, _EstudioPadre.ID_INGRESO, _EstudioPadre.ID_ESTUDIO));
                            if (Entity.PFF_CORRECTIVO != null && Entity.PFF_CORRECTIVO.Any())
                                foreach (var item in Entity.PFF_CORRECTIVO)
                                {
                                    var NvaSancion = new PFF_CORRECTIVO()
                                    {
                                        FECHA = item.FECHA,
                                        ID_ANIO = _Actual.ID_ANIO,
                                        ID_CENTRO = _Actual.ID_CENTRO,
                                        ID_CONSEC = _consecutivoCapacitacionFederal,
                                        ID_ESTUDIO = _Actual.ID_ESTUDIO,
                                        ID_IMPUTADO = _Actual.ID_IMPUTADO,
                                        ID_INGRESO = _Actual.ID_INGRESO,
                                        MOTIVO = item.MOTIVO,
                                        RESOLUCION = item.RESOLUCION
                                    };

                                    Context.PFF_CORRECTIVO.Add(NvaSancion);
                                    _consecutivoCapacitacionFederal++;
                                };

                            Context.PFF_VIGILANCIA.Add(_vigilFedera);
                        }
                        else
                        {
                            _vigilFedera.CENTRO_DONDE_PROCEDE = Entity.CENTRO_DONDE_PROCEDE;
                            _vigilFedera.CONCLUSIONES = Entity.CONCLUSIONES;
                            _vigilFedera.CONDUCTA = Entity.CONDUCTA;
                            _vigilFedera.CONDUCTA_FAMILIA = Entity.CONDUCTA_FAMILIA;
                            _vigilFedera.CONDUCTA_GENERAL = Entity.CONDUCTA_GENERAL;
                            _vigilFedera.CONDUCTA_SUPERIORES = Entity.CONDUCTA_SUPERIORES;
                            _vigilFedera.DESCRIPCION_CONDUCTA = Entity.DESCRIPCION_CONDUCTA;
                            _vigilFedera.DIRECTOR_CENTRO = Entity.DIRECTOR_CENTRO;
                            _vigilFedera.ESTIMULOS_BUENA_CONDUCTA = Entity.ESTIMULOS_BUENA_CONDUCTA;
                            _vigilFedera.FECHA = Entity.FECHA;
                            _vigilFedera.FECHA_INGRESO = Entity.FECHA_INGRESO;
                            _vigilFedera.HIGIENE_CELDA = Entity.HIGIENE_CELDA;
                            _vigilFedera.HIGIENE_PERSONAL = Entity.HIGIENE_PERSONAL;
                            _vigilFedera.ID_ANIO = _EstudioPadre.ID_ANIO;
                            _vigilFedera.ID_CENTRO = _EstudioPadre.ID_CENTRO;
                            _vigilFedera.ID_ESTUDIO = _EstudioPadre.ID_ESTUDIO;
                            _vigilFedera.ID_IMPUTADO = _EstudioPadre.ID_IMPUTADO;
                            _vigilFedera.ID_INGRESO = _EstudioPadre.ID_INGRESO;
                            _vigilFedera.JEFE_VIGILANCIA = Entity.JEFE_VIGILANCIA;
                            _vigilFedera.LUGAR = Entity.LUGAR;
                            _vigilFedera.MOTIVO_TRASLADO = Entity.MOTIVO_TRASLADO;
                            _vigilFedera.NOMBRE = Entity.NOMBRE;
                            _vigilFedera.RELACION_COMPANEROS = Entity.RELACION_COMPANEROS;
                            _vigilFedera.VISITA_FRECUENCIA = Entity.VISITA_FRECUENCIA;
                            _vigilFedera.VISITA_QUIENES = Entity.VISITA_QUIENES;
                            _vigilFedera.VISITA_RECIBE = Entity.VISITA_RECIBE;
                            Context.Entry(_vigilFedera).State = System.Data.EntityState.Modified;

                            var _DetalleSentencia = Context.PFF_CORRECTIVO.Where(x => x.ID_INGRESO == _EstudioPadre.ID_INGRESO && x.ID_IMPUTADO == _EstudioPadre.ID_IMPUTADO && x.ID_ESTUDIO == _EstudioPadre.ID_ESTUDIO && x.ID_ANIO == _EstudioPadre.ID_ANIO);
                            if (_DetalleSentencia != null && _DetalleSentencia.Any())
                                foreach (var item in _DetalleSentencia)
                                    Context.Entry(item).State = System.Data.EntityState.Deleted;

                            var _consecutivoCapacitacionFederal = GetIDProceso<short>("PFF_CORRECTIVO", "ID_CONSEC", string.Format("ID_CENTRO = {0} AND ID_ANIO = {1} AND ID_IMPUTADO = {2} AND ID_INGRESO = {3} AND ID_ESTUDIO = {4}", _EstudioPadre.ID_CENTRO, _EstudioPadre.ID_ANIO, _EstudioPadre.ID_IMPUTADO, _EstudioPadre.ID_INGRESO, _EstudioPadre.ID_ESTUDIO));
                            if (Entity.PFF_CORRECTIVO != null && Entity.PFF_CORRECTIVO.Any())
                                foreach (var item in Entity.PFF_CORRECTIVO)
                                {
                                    var NvaSancion = new PFF_CORRECTIVO()
                                    {
                                        FECHA = item.FECHA,
                                        ID_ANIO = _Actual.ID_ANIO,
                                        ID_CENTRO = _Actual.ID_CENTRO,
                                        ID_CONSEC = _consecutivoCapacitacionFederal,
                                        ID_ESTUDIO = _Actual.ID_ESTUDIO,
                                        ID_IMPUTADO = _Actual.ID_IMPUTADO,
                                        ID_INGRESO = _Actual.ID_INGRESO,
                                        MOTIVO = item.MOTIVO,
                                        RESOLUCION = item.RESOLUCION
                                    };

                                    Context.PFF_CORRECTIVO.Add(NvaSancion);
                                    _consecutivoCapacitacionFederal++;
                                };
                        }
                    }

                    if (_EstudioPadre != null)
                    {
                        short? _Estatus = new short?();
                        if (string.IsNullOrEmpty(Entity.CENTRO_DONDE_PROCEDE) || string.IsNullOrEmpty(Entity.CONCLUSIONES) || string.IsNullOrEmpty(Entity.CONDUCTA) || string.IsNullOrEmpty(Entity.CONDUCTA_FAMILIA) ||
                            string.IsNullOrEmpty(Entity.CONDUCTA_GENERAL) || string.IsNullOrEmpty(Entity.CONDUCTA_SUPERIORES) || string.IsNullOrEmpty(Entity.DESCRIPCION_CONDUCTA) || string.IsNullOrEmpty(Entity.ESTIMULOS_BUENA_CONDUCTA) ||
                            string.IsNullOrEmpty(Entity.HIGIENE_CELDA) || string.IsNullOrEmpty(Entity.HIGIENE_PERSONAL) || string.IsNullOrEmpty(Entity.MOTIVO_TRASLADO) || string.IsNullOrEmpty(Entity.NOMBRE) ||
                            string.IsNullOrEmpty(Entity.RELACION_COMPANEROS) || Entity.FECHA == null || Entity.FECHA_INGRESO == null || string.IsNullOrEmpty(Entity.VISITA_RECIBE))
                            _Estatus = (short)eEstatudDetallePersonalidad.ASIGNADO;
                        else
                            _Estatus = (short)eEstatudDetallePersonalidad.TERMINADO;

                        var DesarrolloVigilanciaFederal = Context.PERSONALIDAD_DETALLE.FirstOrDefault(x => x.ID_IMPUTADO == _EstudioPadre.ID_IMPUTADO && x.ID_ESTUDIO == _EstudioPadre.ID_ESTUDIO && x.ID_INGRESO == _EstudioPadre.ID_INGRESO && x.ID_TIPO == (short)eTiposEstudio.SEGURIDAD_FEDERAL && x.ID_CENTRO == _EstudioPadre.ID_CENTRO && x.ID_ANIO == _EstudioPadre.ID_ANIO);
                        if (DesarrolloVigilanciaFederal == null)
                        {
                            short _ConsecutivoPersonalidadDetalle = GetIDProceso<short>("PERSONALIDAD_DETALLE", "ID_DETALLE", "1=1");
                            var NvoDetalle = new PERSONALIDAD_DETALLE()
                            {
                                ID_ANIO = _EstudioPadre.ID_ANIO,
                                ID_CENTRO = _EstudioPadre.ID_CENTRO,
                                ID_DETALLE = _ConsecutivoPersonalidadDetalle,
                                ID_ESTUDIO = _EstudioPadre.ID_ESTUDIO,
                                ID_IMPUTADO = _EstudioPadre.ID_IMPUTADO,
                                ID_INGRESO = _EstudioPadre.ID_INGRESO,
                                ID_ESTATUS = _Estatus,
                                ID_TIPO = (short)eTiposEstudio.SEGURIDAD_FEDERAL,
                                INICIO_FEC = GetFechaServerDate(),
                                RESULTADO = _Estatus == (short)eEstatudDetallePersonalidad.TERMINADO ? "S" : "N",
                                SOLICITUD_FEC = null,
                                TERMINO_FEC = _Estatus == (short)eEstatudDetallePersonalidad.TERMINADO ? GetFechaServerDate() : new System.DateTime?(),
                                TIPO_MEDIA = string.Empty
                            };

                            Context.PERSONALIDAD_DETALLE.Add(NvoDetalle);
                        }

                        else
                        {
                            DesarrolloVigilanciaFederal.RESULTADO = _Estatus == (short)eEstatudDetallePersonalidad.TERMINADO ? "S" : "N";
                            DesarrolloVigilanciaFederal.ID_ESTATUS = _Estatus;
                            Context.Entry(DesarrolloVigilanciaFederal).State = System.Data.EntityState.Modified;
                        }
                    };

                    Context.SaveChanges();
                    transaccion.Complete();
                    return 1;
                }
            }
            catch (System.Data.Entity.Validation.DbEntityValidationException dbEx)
            {
                foreach (var validationErrors in dbEx.EntityValidationErrors)
                    foreach (var validationError in validationErrors.ValidationErrors)
                        System.Diagnostics.Trace.TraceInformation("Nombre del causante de excepción: {0} Error: {1}", validationError.PropertyName, validationError.ErrorMessage);

                return 2;
            }

            catch (System.Exception exc)
            {
                return 0;
            }
        }

        public short GuardaCriminologicoFederaAislado(PFF_CRIMINOLOGICO Entity)
        {
            try
            {
                using (System.Transactions.TransactionScope transaccion = new System.Transactions.TransactionScope(System.Transactions.TransactionScopeOption.RequiresNew, new System.Transactions.TransactionOptions() { IsolationLevel = System.Transactions.IsolationLevel.ReadCommitted }))
                {
                    var _EstudioPadre = Context.PERSONALIDAD.Where(x => x.ID_INGRESO == Entity.ID_INGRESO && x.ID_IMPUTADO == Entity.ID_IMPUTADO && x.ID_CENTRO == Entity.ID_CENTRO && x.ID_ANIO == Entity.ID_ANIO).OrderByDescending(x => x.ID_ESTUDIO).FirstOrDefault();

                    if (_EstudioPadre == null)
                        return 0;

                    var _Actual = Context.PERSONALIDAD_FUERO_FEDERAL.FirstOrDefault(x => x.ID_ANIO == _EstudioPadre.ID_ANIO && x.ID_CENTRO == _EstudioPadre.ID_CENTRO && x.ID_IMPUTADO == _EstudioPadre.ID_IMPUTADO && x.ID_INGRESO == _EstudioPadre.ID_INGRESO && x.ID_ESTUDIO == _EstudioPadre.ID_ESTUDIO);

                    if (_Actual == null)
                    {
                        var _EstudioTS = Context.PFF_CAPACITACION.FirstOrDefault(x => x.ID_IMPUTADO == _EstudioPadre.ID_IMPUTADO && x.ID_INGRESO == _EstudioPadre.ID_INGRESO && x.ID_ESTUDIO == _EstudioPadre.ID_ESTUDIO && x.ID_CENTRO == _EstudioPadre.ID_CENTRO);
                        var personalidadFederal = new PERSONALIDAD_FUERO_FEDERAL()
                        {
                            ID_ANIO = _EstudioPadre.ID_ANIO,
                            ID_IMPUTADO = _EstudioPadre.ID_IMPUTADO,
                            ID_CENTRO = _EstudioPadre.ID_CENTRO,
                            ID_ESTUDIO = _EstudioPadre.ID_ESTUDIO,
                            ID_INGRESO = _EstudioPadre.ID_INGRESO
                        };

                        var _Crimi = new PFF_CRIMINOLOGICO()
                        {
                            ANTECEDENTES_PARA_ANTI_SOCIALE = Entity.ANTECEDENTES_PARA_ANTI_SOCIALE,
                            CRIMINOLOGO = Entity.CRIMINOLOGO,
                            DIRECTOR_CENTRO = Entity.DIRECTOR_CENTRO,
                            FECHA = Entity.FECHA,
                            ID_ANIO = _EstudioPadre.ID_ANIO,
                            ID_CENTRO = _EstudioPadre.ID_CENTRO,
                            ID_ESTUDIO = _EstudioPadre.ID_ESTUDIO,
                            ID_IMPUTADO = _EstudioPadre.ID_IMPUTADO,
                            ID_INGRESO = _EstudioPadre.ID_INGRESO,
                            LUGAR = Entity.LUGAR,
                            NOMBRE = Entity.NOMBRE,
                            P1_VERSION_INTERNO = Entity.P1_VERSION_INTERNO,
                            P10_CONTINUAR_NO_ESPECIFICAR = Entity.P10_CONTINUAR_NO_ESPECIFICAR,
                            P10_CONTINUAR_SI_ESPECIFICAR = Entity.P10_CONTINUAR_SI_ESPECIFICAR,
                            P10_CONTINUAR_TRATAMIENTO = Entity.P10_CONTINUAR_TRATAMIENTO,
                            P10_OPINION = Entity.P10_OPINION,
                            P2_PERSONALIDAD = Entity.P2_PERSONALIDAD,
                            P3_VALORACION = Entity.P3_VALORACION,
                            P5_ESPECIFICO = Entity.P5_ESPECIFICO,
                            P5_GENERICO = Entity.P5_GENERICO,
                            P5_HABITUAL = Entity.P5_HABITUAL,
                            P5_PRIMODELINCUENTE = Entity.P5_PRIMODELINCUENTE,
                            P5_PROFESIONAL = Entity.P5_PROFESIONAL,
                            P6_CRIMINOGENESIS = Entity.P6_CRIMINOGENESIS,
                            P7_AGRESIVIDAD = Entity.P7_AGRESIVIDAD,
                            P7_EGOCENTRISMO = Entity.P7_EGOCENTRISMO,
                            P7_INDIFERENCIA = Entity.P7_INDIFERENCIA,
                            P7_LABILIDAD = Entity.P7_LABILIDAD,
                            P8_ESTADO_PELIGRO = Entity.P8_ESTADO_PELIGRO,
                            P8_RESULTADO_TRATAMIENTO = Entity.P8_RESULTADO_TRATAMIENTO,
                            P9_PRONOSTICO = Entity.P9_PRONOSTICO,
                            SOBRENOMBRE = Entity.SOBRENOMBRE
                        };

                        Context.PERSONALIDAD_FUERO_FEDERAL.Add(personalidadFederal);
                        Context.PFF_CRIMINOLOGICO.Add(_Crimi);
                    }
                    else
                    {
                        var _crimiAct = _Actual.PFF_CRIMINOLOGICO;
                        if (_crimiAct == null)
                        {
                            _crimiAct = new PFF_CRIMINOLOGICO()
                            {
                                ANTECEDENTES_PARA_ANTI_SOCIALE = Entity.ANTECEDENTES_PARA_ANTI_SOCIALE,
                                CRIMINOLOGO = Entity.CRIMINOLOGO,
                                DIRECTOR_CENTRO = Entity.DIRECTOR_CENTRO,
                                FECHA = Entity.FECHA,
                                ID_ANIO = _EstudioPadre.ID_ANIO,
                                ID_CENTRO = _EstudioPadre.ID_CENTRO,
                                ID_ESTUDIO = _EstudioPadre.ID_ESTUDIO,
                                ID_IMPUTADO = _EstudioPadre.ID_IMPUTADO,
                                ID_INGRESO = _EstudioPadre.ID_INGRESO,
                                LUGAR = Entity.LUGAR,
                                NOMBRE = Entity.NOMBRE,
                                P1_VERSION_INTERNO = Entity.P1_VERSION_INTERNO,
                                P10_CONTINUAR_NO_ESPECIFICAR = Entity.P10_CONTINUAR_NO_ESPECIFICAR,
                                P10_CONTINUAR_SI_ESPECIFICAR = Entity.P10_CONTINUAR_SI_ESPECIFICAR,
                                P10_CONTINUAR_TRATAMIENTO = Entity.P10_CONTINUAR_TRATAMIENTO,
                                P10_OPINION = Entity.P10_OPINION,
                                P2_PERSONALIDAD = Entity.P2_PERSONALIDAD,
                                P3_VALORACION = Entity.P3_VALORACION,
                                P5_ESPECIFICO = Entity.P5_ESPECIFICO,
                                P5_GENERICO = Entity.P5_GENERICO,
                                P5_HABITUAL = Entity.P5_HABITUAL,
                                P5_PRIMODELINCUENTE = Entity.P5_PRIMODELINCUENTE,
                                P5_PROFESIONAL = Entity.P5_PROFESIONAL,
                                P6_CRIMINOGENESIS = Entity.P6_CRIMINOGENESIS,
                                P7_AGRESIVIDAD = Entity.P7_AGRESIVIDAD,
                                P7_EGOCENTRISMO = Entity.P7_EGOCENTRISMO,
                                P7_INDIFERENCIA = Entity.P7_INDIFERENCIA,
                                P7_LABILIDAD = Entity.P7_LABILIDAD,
                                P8_ESTADO_PELIGRO = Entity.P8_ESTADO_PELIGRO,
                                P8_RESULTADO_TRATAMIENTO = Entity.P8_RESULTADO_TRATAMIENTO,
                                P9_PRONOSTICO = Entity.P9_PRONOSTICO,
                                SOBRENOMBRE = Entity.SOBRENOMBRE
                            };

                            Context.PFF_CRIMINOLOGICO.Add(_crimiAct);
                        }
                        else
                        {
                            _crimiAct.ANTECEDENTES_PARA_ANTI_SOCIALE = Entity.ANTECEDENTES_PARA_ANTI_SOCIALE;
                            _crimiAct.CRIMINOLOGO = Entity.CRIMINOLOGO;
                            _crimiAct.DIRECTOR_CENTRO = Entity.DIRECTOR_CENTRO;
                            _crimiAct.FECHA = Entity.FECHA;
                            _crimiAct.ID_ANIO = _EstudioPadre.ID_ANIO;
                            _crimiAct.ID_CENTRO = _EstudioPadre.ID_CENTRO;
                            _crimiAct.ID_ESTUDIO = _EstudioPadre.ID_ESTUDIO;
                            _crimiAct.ID_IMPUTADO = _EstudioPadre.ID_IMPUTADO;
                            _crimiAct.ID_INGRESO = _EstudioPadre.ID_INGRESO;
                            _crimiAct.LUGAR = Entity.LUGAR;
                            _crimiAct.NOMBRE = Entity.NOMBRE;
                            _crimiAct.P1_VERSION_INTERNO = Entity.P1_VERSION_INTERNO;
                            _crimiAct.P10_CONTINUAR_NO_ESPECIFICAR = Entity.P10_CONTINUAR_NO_ESPECIFICAR;
                            _crimiAct.P10_CONTINUAR_SI_ESPECIFICAR = Entity.P10_CONTINUAR_SI_ESPECIFICAR;
                            _crimiAct.P10_CONTINUAR_TRATAMIENTO = Entity.P10_CONTINUAR_TRATAMIENTO;
                            _crimiAct.P10_OPINION = Entity.P10_OPINION;
                            _crimiAct.P2_PERSONALIDAD = Entity.P2_PERSONALIDAD;
                            _crimiAct.P3_VALORACION = Entity.P3_VALORACION;
                            _crimiAct.P5_ESPECIFICO = Entity.P5_ESPECIFICO;
                            _crimiAct.P5_GENERICO = Entity.P5_GENERICO;
                            _crimiAct.P5_HABITUAL = Entity.P5_HABITUAL;
                            _crimiAct.P5_PRIMODELINCUENTE = Entity.P5_PRIMODELINCUENTE;
                            _crimiAct.P5_PROFESIONAL = Entity.P5_PROFESIONAL;
                            _crimiAct.P6_CRIMINOGENESIS = Entity.P6_CRIMINOGENESIS;
                            _crimiAct.P7_AGRESIVIDAD = Entity.P7_AGRESIVIDAD;
                            _crimiAct.P7_EGOCENTRISMO = Entity.P7_EGOCENTRISMO;
                            _crimiAct.P7_INDIFERENCIA = Entity.P7_INDIFERENCIA;
                            _crimiAct.P7_LABILIDAD = Entity.P7_LABILIDAD;
                            _crimiAct.P8_ESTADO_PELIGRO = Entity.P8_ESTADO_PELIGRO;
                            _crimiAct.P8_RESULTADO_TRATAMIENTO = Entity.P8_RESULTADO_TRATAMIENTO;
                            _crimiAct.P9_PRONOSTICO = Entity.P9_PRONOSTICO;
                            _crimiAct.SOBRENOMBRE = Entity.SOBRENOMBRE;
                            Context.Entry(_crimiAct).State = System.Data.EntityState.Modified;
                        }
                    }

                    if (_EstudioPadre != null)
                    {
                        short? _Estatus = new short?();
                        if (string.IsNullOrEmpty(Entity.ANTECEDENTES_PARA_ANTI_SOCIALE) || string.IsNullOrEmpty(Entity.NOMBRE) || string.IsNullOrEmpty(Entity.P1_VERSION_INTERNO) || string.IsNullOrEmpty(Entity.P10_CONTINUAR_TRATAMIENTO) ||
                            string.IsNullOrEmpty(Entity.P10_OPINION) || string.IsNullOrEmpty(Entity.P2_PERSONALIDAD) || string.IsNullOrEmpty(Entity.P3_VALORACION) || string.IsNullOrEmpty(Entity.P6_CRIMINOGENESIS) ||
                            string.IsNullOrEmpty(Entity.P7_AGRESIVIDAD) || string.IsNullOrEmpty(Entity.P7_EGOCENTRISMO) || string.IsNullOrEmpty(Entity.P7_INDIFERENCIA) || string.IsNullOrEmpty(Entity.P7_LABILIDAD) ||
                            string.IsNullOrEmpty(Entity.P8_ESTADO_PELIGRO) || string.IsNullOrEmpty(Entity.P8_RESULTADO_TRATAMIENTO) || string.IsNullOrEmpty(Entity.P9_PRONOSTICO) || string.IsNullOrEmpty(Entity.SOBRENOMBRE) || Entity.FECHA == null)
                            _Estatus = (short)eEstatudDetallePersonalidad.ASIGNADO;
                        else
                            _Estatus = (short)eEstatudDetallePersonalidad.TERMINADO;

                        var DesarrolloCriminoFederal = Context.PERSONALIDAD_DETALLE.FirstOrDefault(x => x.ID_IMPUTADO == _EstudioPadre.ID_IMPUTADO && x.ID_ESTUDIO == _EstudioPadre.ID_ESTUDIO && x.ID_INGRESO == _EstudioPadre.ID_INGRESO && x.ID_TIPO == (short)eTiposEstudio.CRIMIN_FEDERAL && x.ID_CENTRO == _EstudioPadre.ID_CENTRO && x.ID_ANIO == _EstudioPadre.ID_ANIO);
                        if (DesarrolloCriminoFederal == null)
                        {
                            short _ConsecutivoPersonalidadDetalle = GetIDProceso<short>("PERSONALIDAD_DETALLE", "ID_DETALLE", "1=1");
                            var NvoDetalle = new PERSONALIDAD_DETALLE()
                            {
                                ID_ANIO = _EstudioPadre.ID_ANIO,
                                ID_CENTRO = _EstudioPadre.ID_CENTRO,
                                ID_DETALLE = _ConsecutivoPersonalidadDetalle,
                                ID_ESTUDIO = _EstudioPadre.ID_ESTUDIO,
                                ID_IMPUTADO = _EstudioPadre.ID_IMPUTADO,
                                ID_INGRESO = _EstudioPadre.ID_INGRESO,
                                ID_TIPO = (short)eTiposEstudio.CRIMIN_FEDERAL,
                                INICIO_FEC = GetFechaServerDate(),
                                RESULTADO = _Estatus == (short)eEstatudDetallePersonalidad.TERMINADO ? "S" : "N",
                                SOLICITUD_FEC = null,
                                TERMINO_FEC = _Estatus == (short)eEstatudDetallePersonalidad.TERMINADO ? GetFechaServerDate() : new System.DateTime?(),
                                TIPO_MEDIA = string.Empty
                            };

                            Context.PERSONALIDAD_DETALLE.Add(NvoDetalle);
                        }

                        else
                        {
                            DesarrolloCriminoFederal.RESULTADO = _Estatus == (short)eEstatudDetallePersonalidad.TERMINADO ? "S" : "N";
                            DesarrolloCriminoFederal.ID_ESTATUS = _Estatus;
                            Context.Entry(DesarrolloCriminoFederal).State = EntityState.Modified;
                        }
                    };

                    Context.SaveChanges();
                    transaccion.Complete();
                    return 1;
                }
            }
            catch (System.Data.Entity.Validation.DbEntityValidationException dbEx)
            {
                foreach (var validationErrors in dbEx.EntityValidationErrors)
                    foreach (var validationError in validationErrors.ValidationErrors)
                        System.Diagnostics.Trace.TraceInformation("Nombre del causante de excepción: {0} Error: {1}", validationError.PropertyName, validationError.ErrorMessage);

                return 2;
            }

            catch (System.Exception)
            {
                return 0;
            }
        }
    }
}