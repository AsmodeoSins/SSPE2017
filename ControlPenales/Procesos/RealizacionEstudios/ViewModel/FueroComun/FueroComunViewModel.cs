using SSP.Controlador.Catalogo.Justicia;
using SSP.Servidor;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace ControlPenales
{
    partial class RealizacionEstudiosViewModel
    {
        #region Validacion vs estudio padre y transacciones individuales
        private bool ValidaEstudioPadre()
        {
            try
            {
                if (SelectIngreso != null)
                {
                    var _EstudioPersonalidadPdre = new cEstudioPersonalidad().GetData(x => x.ID_INGRESO == SelectIngreso.ID_INGRESO && x.ID_IMPUTADO == SelectIngreso.ID_IMPUTADO && x.ID_ANIO == SelectIngreso.ID_ANIO).FirstOrDefault();
                    if (_EstudioPersonalidadPdre != null)
                    {
                        _EstudioPersonalidad = _EstudioPersonalidadPdre;//Independientemente que exista o no un estudio hijo, ocupo este estudio para amarrarle los estudios de personalidad
                        var _EstudioFueroC = new cEstudioPersonalidadFueroComun().GetData(x => x.ID_INGRESO == SelectIngreso.ID_INGRESO && x.ID_IMPUTADO == SelectIngreso.ID_IMPUTADO && x.ID_ESTUDIO == _EstudioPersonalidad.ID_ESTUDIO).FirstOrDefault();
                        if (_EstudioFueroC != null)
                            _IsNuevoEstudioP = false;
                        else
                            _IsNuevoEstudioP = true;

                        return true;//Si tiene un estudio para amarrarle a los estudios
                    }

                    else
                    {
                        new Dialogos().ConfirmacionDialogo("Notificación!", "No se encontró ningún estudio de personalidad programado hacia este imputado.");
                        return false;
                    }
                }
                else
                {
                    new Dialogos().ConfirmacionDialogo("Notificación!", "Seleccione un ingreso para continuar.");
                    return false;
                }
            }
            catch (Exception exc)
            {

                throw;
            }
        } //Inicializa el estudio de personalidad de fuero comun
        private PFC_II_MEDICO EstudioMedicoTransaccionIndividual(PERSONALIDAD Entity)
        {
            try
            {
                var _EstudioAnterior = new cPersonalidadEstudioMedicoComun().GetData(x => x.ID_INGRESO == SelectIngreso.ID_INGRESO && x.ID_IMPUTADO == SelectIngreso.ID_IMPUTADO && x.ID_CENTRO == SelectIngreso.ID_CENTRO && x.ID_ANIO == SelectIngreso.ID_ANIO && x.ID_ANIO == SelectIngreso.ID_ANIO && x.ID_ESTUDIO == Entity.ID_ESTUDIO).FirstOrDefault();
                var NombreUsuario = new cUsuario().Obtener(GlobalVar.gUsr);
                var NombreCoordinadorMedico = new cDepartamentosAcceso().GetData(x => x.ID_DEPARTAMENTO == (short)eRolesDepartamentoAccesoProcesoRealizacionEstudios.COORDINADOR_MEDICO).FirstOrDefault();
                if (_EstudioAnterior == null)
                {
                    var _MedicoComun = new PFC_II_MEDICO()
                    {
                        ELABORO = NombreUsuario != null ? NombreUsuario.EMPLEADO != null ? NombreUsuario.EMPLEADO.PERSONA != null ? string.Format("{0} {1} {2}",
                            !string.IsNullOrEmpty(NombreUsuario.EMPLEADO.PERSONA.NOMBRE) ? NombreUsuario.EMPLEADO.PERSONA.NOMBRE.Trim() : string.Empty,
                            !string.IsNullOrEmpty(NombreUsuario.EMPLEADO.PERSONA.PATERNO) ? NombreUsuario.EMPLEADO.PERSONA.PATERNO.Trim() : string.Empty,
                            !string.IsNullOrEmpty(NombreUsuario.EMPLEADO.PERSONA.MATERNO) ? NombreUsuario.EMPLEADO.PERSONA.MATERNO.Trim() : string.Empty) : string.Empty : string.Empty : string.Empty,
                        COORDINADOR = NombreCoordinadorMedico != null ? NombreCoordinadorMedico.USUARIO != null ? NombreCoordinadorMedico.USUARIO.EMPLEADO != null ? NombreCoordinadorMedico.USUARIO.EMPLEADO.PERSONA != null ?
                            string.Format("{0} {1} {2} ", !string.IsNullOrEmpty(NombreCoordinadorMedico.USUARIO.EMPLEADO.PERSONA.NOMBRE) ? NombreCoordinadorMedico.USUARIO.EMPLEADO.PERSONA.NOMBRE.Trim() : string.Empty,
                            !string.IsNullOrEmpty(NombreCoordinadorMedico.USUARIO.EMPLEADO.PERSONA.PATERNO) ? NombreCoordinadorMedico.USUARIO.EMPLEADO.PERSONA.PATERNO.Trim() : string.Empty,
                            !string.IsNullOrEmpty(NombreCoordinadorMedico.USUARIO.EMPLEADO.PERSONA.MATERNO) ? NombreCoordinadorMedico.USUARIO.EMPLEADO.PERSONA.MATERNO.Trim() : string.Empty) : string.Empty : string.Empty : string.Empty : string.Empty,
                        ID_ANIO = SelectIngreso.ID_ANIO,
                        ID_CENTRO = SelectIngreso.ID_CENTRO,
                        ID_IMPUTADO = SelectIngreso.ID_IMPUTADO,
                        ID_INGRESO = SelectIngreso.ID_INGRESO,
                        P2_HEREDO_FAMILIARES = AntecedentesHeredoFamiliares,
                        P3_ANTPER_NOPATO = AntecedentesPersonalesNoPatologicos,
                        P31_CONSUMO_TOXICO = AntedecentesConsumoToxicosEstadoActual,
                        P32_TATUAJES_CICATRICES = DescripcionTatuajesCicatricesMalformaciones,
                        P4_PATOLOGICOS = AntecedentesPatologicos,
                        P5_PADECIMIENTOS = DescipcionPadecimientoActual,
                        SIGNOS_TA = string.Format("{0} / {1}", Arterial1, Arterial2),
                        SIGNOS_TEMPERATURA = TemperaturaGenerico,
                        SIGNOS_PULSO = PulsoGenerico,
                        SIGNOS_RESPIRACION = RespiracionGenerico,
                        SIGNOS_PESO = PesoGenerico,
                        SIGNOS_ESTATURA = EstaturaGenerico,
                        P8_DICTAMEN_MEDICO = IdDictamenMedicoComun,
                        ESTUDIO_FEC = FechaEstudioMedicoComun,
                        P7_IMPRESION_DIAGNOSTICA = ImpresionDiagnosticaEstudioMedicoComun,
                        P6_EXPLORACION_FISICA = string.Empty//ESTE CAMPO NO SE USA, PROGRAMADO PARA SU ELIMINACION
                    };

                    return _MedicoComun;
                }

                else
                {
                    _EstudioAnterior.ELABORO = NombreUsuario != null ? NombreUsuario.EMPLEADO != null ? NombreUsuario.EMPLEADO.PERSONA != null ? string.Format("{0} {1} {2}",
                        !string.IsNullOrEmpty(NombreUsuario.EMPLEADO.PERSONA.NOMBRE) ? NombreUsuario.EMPLEADO.PERSONA.NOMBRE.Trim() : string.Empty,
                        !string.IsNullOrEmpty(NombreUsuario.EMPLEADO.PERSONA.PATERNO) ? NombreUsuario.EMPLEADO.PERSONA.PATERNO.Trim() : string.Empty,
                        !string.IsNullOrEmpty(NombreUsuario.EMPLEADO.PERSONA.MATERNO) ? NombreUsuario.EMPLEADO.PERSONA.MATERNO.Trim() : string.Empty) : string.Empty : string.Empty : string.Empty;
                    _EstudioAnterior.COORDINADOR = NombreCoordinadorMedico != null ? NombreCoordinadorMedico.USUARIO != null ? NombreCoordinadorMedico.USUARIO.EMPLEADO != null ? NombreCoordinadorMedico.USUARIO.EMPLEADO.PERSONA != null ?
                        string.Format("{0} {1} {2} ", !string.IsNullOrEmpty(NombreCoordinadorMedico.USUARIO.EMPLEADO.PERSONA.NOMBRE) ? NombreCoordinadorMedico.USUARIO.EMPLEADO.PERSONA.NOMBRE.Trim() : string.Empty,
                        !string.IsNullOrEmpty(NombreCoordinadorMedico.USUARIO.EMPLEADO.PERSONA.PATERNO) ? NombreCoordinadorMedico.USUARIO.EMPLEADO.PERSONA.PATERNO.Trim() : string.Empty,
                        !string.IsNullOrEmpty(NombreCoordinadorMedico.USUARIO.EMPLEADO.PERSONA.MATERNO) ? NombreCoordinadorMedico.USUARIO.EMPLEADO.PERSONA.MATERNO.Trim() : string.Empty) : string.Empty : string.Empty : string.Empty : string.Empty;
                    _EstudioAnterior.ID_ANIO = SelectIngreso.ID_ANIO;
                    _EstudioAnterior.ID_CENTRO = SelectIngreso.ID_CENTRO;
                    _EstudioAnterior.ID_IMPUTADO = SelectIngreso.ID_IMPUTADO;
                    _EstudioAnterior.ID_INGRESO = SelectIngreso.ID_INGRESO;
                    _EstudioAnterior.P2_HEREDO_FAMILIARES = AntecedentesHeredoFamiliares;
                    _EstudioAnterior.P3_ANTPER_NOPATO = AntecedentesPersonalesNoPatologicos;
                    _EstudioAnterior.P31_CONSUMO_TOXICO = AntedecentesConsumoToxicosEstadoActual;
                    _EstudioAnterior.P32_TATUAJES_CICATRICES = DescripcionTatuajesCicatricesMalformaciones;
                    _EstudioAnterior.P4_PATOLOGICOS = AntecedentesPatologicos;
                    _EstudioAnterior.P5_PADECIMIENTOS = DescipcionPadecimientoActual;
                    _EstudioAnterior.SIGNOS_TA = string.Format("{0} / {1}", Arterial1, Arterial2);
                    _EstudioAnterior.SIGNOS_TEMPERATURA = TemperaturaGenerico;
                    _EstudioAnterior.SIGNOS_PULSO = PulsoGenerico;
                    _EstudioAnterior.SIGNOS_RESPIRACION = RespiracionGenerico;
                    _EstudioAnterior.SIGNOS_PESO = PesoGenerico;
                    _EstudioAnterior.SIGNOS_ESTATURA = EstaturaGenerico;
                    _EstudioAnterior.P8_DICTAMEN_MEDICO = IdDictamenMedicoComun;
                    _EstudioAnterior.ESTUDIO_FEC = FechaEstudioMedicoComun;
                    _EstudioAnterior.P7_IMPRESION_DIAGNOSTICA = ImpresionDiagnosticaEstudioMedicoComun;
                }

                return _EstudioAnterior;
            }
            catch (Exception exc)
            {
                throw;
            }
        }
        private PFC_III_PSIQUIATRICO EstudioPsiquiatricoTransaccionIndividual(PERSONALIDAD Entity)
        {
            try
            {
                var _EstudioPsiq = new cPersonalidadEstudioPsiquiatricoComun().GetData(x => x.ID_INGRESO == SelectIngreso.ID_INGRESO && x.ID_IMPUTADO == SelectIngreso.ID_IMPUTADO && x.ID_CENTRO == SelectIngreso.ID_CENTRO && x.ID_ANIO == SelectIngreso.ID_ANIO && x.ID_ANIO == SelectIngreso.ID_ANIO && x.ID_ESTUDIO == Entity.ID_ESTUDIO).FirstOrDefault();
                var NombreUsuario = new cUsuario().Obtener(GlobalVar.gUsr);
                var NombreCoordinadorMedico = new cDepartamentosAcceso().GetData(x => x.ID_DEPARTAMENTO == (short)eRolesDepartamentoAccesoProcesoRealizacionEstudios.COORDINADOR_MEDICO).FirstOrDefault();
                if (_EstudioPsiq == null)
                {
                    var _PsiquiatricoComun = new PFC_III_PSIQUIATRICO()
                    {
                        A1_ASPECTO_FISICO = AspectoFisico,
                        B1_CONDUCTA_MOTORA = ConductaMotora,
                        C1_HABLA = Habla,
                        D1_ACTITUD = Actitud,
                        A2_ESTADO_ANIMO = EstadoAnimo,
                        B2_EXPRESION_AFECTIVA = ExpresionAfectiva,
                        C2_ADECUACION = Adecuacion,
                        A3_ALUCINACIONES = Alucinaciones,
                        B3_ILUSIONES = Ilusiones,
                        C3_DESPERSONALIZACION = Despersonalizacion,
                        D3_DESREALIZACION = Desrealizacion,
                        A4_CURSO = CursoPensamiento,
                        B4_CONTINUIDAD = ContinuidadPensamiento,
                        C4_CONTENIDO = ContenidoPensamiento,
                        D4_ABASTRACTO = PensamientoAbstracto,
                        E4_CONCENTRACION = Concentracion,
                        P5_ORIENTACION = Orientacion,
                        P6_MEMORIA = Memoria,
                        A7_BAJA_TOLERANCIA = BajaToleranciaFrustr,
                        B7_EXPRESION = ExpresionDesadaptativa,
                        C7_ADECUADA = Adecuada,
                        P8_CAPACIDAD_JUICIO = CapacidadJuicio,
                        P9_INTROSPECCION = Introspeccion,
                        P10_FIANILIDAD = Fiabilidad,
                        P11_IMPRESION = ImpresionDiagnosticaPsiquiatricoComun,
                        P12_DICTAMEN_PSIQUIATRICO = DictamenPsiqComun,
                        ESTUDIO_FEC = FecDictamenPsiqComun,
                        ID_ANIO = SelectIngreso.ID_ANIO,
                        ID_CENTRO = SelectIngreso.ID_CENTRO,
                        ID_IMPUTADO = SelectIngreso.ID_IMPUTADO,
                        ID_INGRESO = SelectIngreso.ID_INGRESO,
                        MEDICO_PSIQUIATRA = NombreUsuario != null ? NombreUsuario.EMPLEADO != null ? NombreUsuario.EMPLEADO.PERSONA != null ? string.Format("{0} {1} {2}",
                            !string.IsNullOrEmpty(NombreUsuario.EMPLEADO.PERSONA.NOMBRE) ? NombreUsuario.EMPLEADO.PERSONA.NOMBRE.Trim() : string.Empty,
                            !string.IsNullOrEmpty(NombreUsuario.EMPLEADO.PERSONA.PATERNO) ? NombreUsuario.EMPLEADO.PERSONA.PATERNO.Trim() : string.Empty,
                            !string.IsNullOrEmpty(NombreUsuario.EMPLEADO.PERSONA.MATERNO) ? NombreUsuario.EMPLEADO.PERSONA.MATERNO.Trim() : string.Empty) : string.Empty : string.Empty : string.Empty,
                        COORDINADOR = NombreCoordinadorMedico != null ? NombreCoordinadorMedico.USUARIO != null ? NombreCoordinadorMedico.USUARIO.EMPLEADO != null ? NombreCoordinadorMedico.USUARIO.EMPLEADO.PERSONA != null ?
                            string.Format("{0} {1} {2} ", !string.IsNullOrEmpty(NombreCoordinadorMedico.USUARIO.EMPLEADO.PERSONA.NOMBRE) ? NombreCoordinadorMedico.USUARIO.EMPLEADO.PERSONA.NOMBRE.Trim() : string.Empty,
                            !string.IsNullOrEmpty(NombreCoordinadorMedico.USUARIO.EMPLEADO.PERSONA.PATERNO) ? NombreCoordinadorMedico.USUARIO.EMPLEADO.PERSONA.PATERNO.Trim() : string.Empty,
                            !string.IsNullOrEmpty(NombreCoordinadorMedico.USUARIO.EMPLEADO.PERSONA.MATERNO) ? NombreCoordinadorMedico.USUARIO.EMPLEADO.PERSONA.MATERNO.Trim() : string.Empty) : string.Empty : string.Empty : string.Empty : string.Empty
                    };

                    return _PsiquiatricoComun;
                }

                else
                {
                    _EstudioPsiq.A1_ASPECTO_FISICO = AspectoFisico;
                    _EstudioPsiq.B1_CONDUCTA_MOTORA = ConductaMotora;
                    _EstudioPsiq.C1_HABLA = Habla;
                    _EstudioPsiq.D1_ACTITUD = Actitud;
                    _EstudioPsiq.A2_ESTADO_ANIMO = EstadoAnimo;
                    _EstudioPsiq.B2_EXPRESION_AFECTIVA = ExpresionAfectiva;
                    _EstudioPsiq.C2_ADECUACION = Adecuacion;
                    _EstudioPsiq.A3_ALUCINACIONES = Alucinaciones;
                    _EstudioPsiq.B3_ILUSIONES = Ilusiones;
                    _EstudioPsiq.C3_DESPERSONALIZACION = Despersonalizacion;
                    _EstudioPsiq.D3_DESREALIZACION = Desrealizacion;
                    _EstudioPsiq.A4_CURSO = CursoPensamiento;
                    _EstudioPsiq.B4_CONTINUIDAD = ContinuidadPensamiento;
                    _EstudioPsiq.C4_CONTENIDO = ContenidoPensamiento;
                    _EstudioPsiq.D4_ABASTRACTO = PensamientoAbstracto;
                    _EstudioPsiq.E4_CONCENTRACION = Concentracion;
                    _EstudioPsiq.P5_ORIENTACION = Orientacion;
                    _EstudioPsiq.P6_MEMORIA = Memoria;
                    _EstudioPsiq.A7_BAJA_TOLERANCIA = BajaToleranciaFrustr;
                    _EstudioPsiq.B7_EXPRESION = ExpresionDesadaptativa;
                    _EstudioPsiq.C7_ADECUADA = Adecuada;
                    _EstudioPsiq.P8_CAPACIDAD_JUICIO = CapacidadJuicio;
                    _EstudioPsiq.P9_INTROSPECCION = Introspeccion;
                    _EstudioPsiq.P10_FIANILIDAD = Fiabilidad;
                    _EstudioPsiq.P11_IMPRESION = ImpresionDiagnosticaPsiquiatricoComun;
                    _EstudioPsiq.P12_DICTAMEN_PSIQUIATRICO = DictamenPsiqComun;
                    _EstudioPsiq.ESTUDIO_FEC = FecDictamenPsiqComun;
                    _EstudioPsiq.ID_ANIO = SelectIngreso.ID_ANIO;
                    _EstudioPsiq.ID_CENTRO = SelectIngreso.ID_CENTRO;
                    _EstudioPsiq.ID_IMPUTADO = SelectIngreso.ID_IMPUTADO;
                    _EstudioPsiq.ID_INGRESO = SelectIngreso.ID_INGRESO;
                    _EstudioPsiq.MEDICO_PSIQUIATRA = NombreUsuario != null ? NombreUsuario.EMPLEADO != null ? NombreUsuario.EMPLEADO.PERSONA != null ? string.Format("{0} {1} {2}",
                        !string.IsNullOrEmpty(NombreUsuario.EMPLEADO.PERSONA.NOMBRE) ? NombreUsuario.EMPLEADO.PERSONA.NOMBRE.Trim() : string.Empty,
                        !string.IsNullOrEmpty(NombreUsuario.EMPLEADO.PERSONA.PATERNO) ? NombreUsuario.EMPLEADO.PERSONA.PATERNO.Trim() : string.Empty,
                        !string.IsNullOrEmpty(NombreUsuario.EMPLEADO.PERSONA.MATERNO) ? NombreUsuario.EMPLEADO.PERSONA.MATERNO.Trim() : string.Empty) : string.Empty : string.Empty : string.Empty;
                    _EstudioPsiq.COORDINADOR = NombreCoordinadorMedico != null ? NombreCoordinadorMedico.USUARIO != null ? NombreCoordinadorMedico.USUARIO.EMPLEADO != null ? NombreCoordinadorMedico.USUARIO.EMPLEADO.PERSONA != null ?
                        string.Format("{0} {1} {2} ", !string.IsNullOrEmpty(NombreCoordinadorMedico.USUARIO.EMPLEADO.PERSONA.NOMBRE) ? NombreCoordinadorMedico.USUARIO.EMPLEADO.PERSONA.NOMBRE.Trim() : string.Empty,
                        !string.IsNullOrEmpty(NombreCoordinadorMedico.USUARIO.EMPLEADO.PERSONA.PATERNO) ? NombreCoordinadorMedico.USUARIO.EMPLEADO.PERSONA.PATERNO.Trim() : string.Empty,
                        !string.IsNullOrEmpty(NombreCoordinadorMedico.USUARIO.EMPLEADO.PERSONA.MATERNO) ? NombreCoordinadorMedico.USUARIO.EMPLEADO.PERSONA.MATERNO.Trim() : string.Empty) : string.Empty : string.Empty : string.Empty : string.Empty;
                    return _EstudioPsiq;
                }

            }
            catch (Exception exc)
            {
                throw;
            }
        }
        private PFC_IV_PSICOLOGICO EstudioPsicologicoTransaccionIndividual(PERSONALIDAD Entity)
        {
            try
            {
                var NombreUsuario = new cUsuario().Obtener(GlobalVar.gUsr);
                var NombreCoordinadorPsicologico = new cDepartamentosAcceso().GetData(x => x.ID_DEPARTAMENTO == (short)eRolesDepartamentoAccesoProcesoRealizacionEstudios.COORDINADOR_PSICOLOGICO).FirstOrDefault();
                var _EstudioPsico = new cPersonalidadEstudioPsicologicoComun().GetData(x => x.ID_INGRESO == SelectIngreso.ID_INGRESO && x.ID_IMPUTADO == SelectIngreso.ID_IMPUTADO && x.ID_CENTRO == SelectIngreso.ID_CENTRO && x.ID_ANIO == SelectIngreso.ID_ANIO && x.ID_ANIO == SelectIngreso.ID_ANIO && x.ID_ESTUDIO == Entity.ID_ESTUDIO).FirstOrDefault();
                if (_EstudioPsico == null)
                {
                    var _Psicologico = new PFC_IV_PSICOLOGICO()
                    {
                        ESTUDIO_FEC = FechaDictamenPsicologicoComun,
                        ID_ANIO = SelectIngreso.ID_ANIO,
                        ID_CENTRO = SelectIngreso.ID_CENTRO,
                        ID_IMPUTADO = SelectIngreso.ID_IMPUTADO,
                        ID_INGRESO = SelectIngreso.ID_INGRESO,
                        P1_CONDICIONES_GRALES = CondicionesGralesInterno,
                        P10_MOTIVACION_DICTAMEN = MotivacionDictamenPsicologicoComun,
                        P11_CASO_NEGATIVO = CasoNegativoEstudioPsicologicoComun,
                        P12_CUAL = CualTratamientoExtraMurosPsicologicoComun,
                        P12_REQUIERE_TRATAMIENTO = IdReqExtraMurosPsicologicoComun,
                        P2_EXAMEN_MENTAL = ExamenMental,
                        P3_PRINCIPALES_RASGOS = DescripcionPrincipalesRazgosIngreso,
                        P4_INVENTARIO_MULTIFASICO = IsTestMinesotaChecked == true ? (short)eSINO.SI : (short)eSINO.NO,
                        P4_OTRAS = IsTestOtrosChecked == true ? (short)eSINO.SI : (short)eSINO.NO,
                        P4_TEST_GUALTICO = IsTestLaurettaBenderChecked == true ? (short)eSINO.SI : (short)eSINO.NO,
                        P4_TEST_HTP = IsTestCasaArbolPersonaChecked == true ? (short)eSINO.SI : (short)eSINO.NO,
                        P4_TEST_MATRICES = IsTestMatricesProgresivasRavenChecked == true ? (short)eSINO.SI : (short)eSINO.NO,
                        P51_NIVEL_INTELECTUAL = IdNivelIntelectual,
                        P52_DISFUNCION_NEUROLOGICA = IdDisfuncionNeurologica,
                        P6_INTEGRACION = IntegracionDinamicaPersonalidadActual,
                        P8_RASGOS_PERSONALIDAD = RasgosPersonalidadRelaciondosComisionDelito,
                        P9_DICTAMEN_REINSERCION = IdDictamenPsicologicoComun,
                        ELABORO = NombreUsuario != null ? NombreUsuario.EMPLEADO != null ? NombreUsuario.EMPLEADO.PERSONA != null ? string.Format("{0} {1} {2}",
                            !string.IsNullOrEmpty(NombreUsuario.EMPLEADO.PERSONA.NOMBRE) ? NombreUsuario.EMPLEADO.PERSONA.NOMBRE.Trim() : string.Empty,
                            !string.IsNullOrEmpty(NombreUsuario.EMPLEADO.PERSONA.PATERNO) ? NombreUsuario.EMPLEADO.PERSONA.PATERNO.Trim() : string.Empty,
                            !string.IsNullOrEmpty(NombreUsuario.EMPLEADO.PERSONA.MATERNO) ? NombreUsuario.EMPLEADO.PERSONA.MATERNO.Trim() : string.Empty) : string.Empty : string.Empty : string.Empty,
                        P4_OTRA_MENCIONAR = EspecifiqueOtroTest,
                        COORDINADOR = NombreCoordinadorPsicologico != null ? NombreCoordinadorPsicologico.USUARIO != null ? NombreCoordinadorPsicologico.USUARIO.EMPLEADO != null ? NombreCoordinadorPsicologico.USUARIO.EMPLEADO.PERSONA != null ?
                            string.Format("{0} {1} {2} ", !string.IsNullOrEmpty(NombreCoordinadorPsicologico.USUARIO.EMPLEADO.PERSONA.NOMBRE) ? NombreCoordinadorPsicologico.USUARIO.EMPLEADO.PERSONA.NOMBRE.Trim() : string.Empty,
                            !string.IsNullOrEmpty(NombreCoordinadorPsicologico.USUARIO.EMPLEADO.PERSONA.PATERNO) ? NombreCoordinadorPsicologico.USUARIO.EMPLEADO.PERSONA.PATERNO.Trim() : string.Empty,
                            !string.IsNullOrEmpty(NombreCoordinadorPsicologico.USUARIO.EMPLEADO.PERSONA.MATERNO) ? NombreCoordinadorPsicologico.USUARIO.EMPLEADO.PERSONA.MATERNO.Trim() : string.Empty) : string.Empty : string.Empty : string.Empty : string.Empty
                    };

                    if (LstProgramasPsicologico != null && LstProgramasPsicologico.Any())
                        foreach (var item in LstProgramasPsicologico)
                            _Psicologico.PFC_IV_PROGRAMA.Add(item);

                    if (LstProgModifConduc != null && LstProgModifConduc.Any())
                        foreach (var item in LstProgModifConduc)
                            _Psicologico.PFC_IV_PROGRAMA.Add(item);

                    if (LstComplement != null && LstComplement.Any())
                        foreach (var item in LstComplement)
                            _Psicologico.PFC_IV_PROGRAMA.Add(item);

                    if (LstTalleresOrient != null && LstTalleresOrient.Any())
                        foreach (var item in LstTalleresOrient)
                            _Psicologico.PFC_IV_PROGRAMA.Add(item);

                    return _Psicologico;
                }
                else
                {
                    var Progra = new ObservableCollection<PFC_IV_PROGRAMA>();
                    _EstudioPsico.ESTUDIO_FEC = FechaDictamenPsicologicoComun;
                    _EstudioPsico.ID_ANIO = SelectIngreso.ID_ANIO;
                    _EstudioPsico.ID_CENTRO = SelectIngreso.ID_CENTRO;
                    _EstudioPsico.ID_IMPUTADO = SelectIngreso.ID_IMPUTADO;
                    _EstudioPsico.ID_INGRESO = SelectIngreso.ID_INGRESO;
                    _EstudioPsico.P1_CONDICIONES_GRALES = CondicionesGralesInterno;
                    _EstudioPsico.P10_MOTIVACION_DICTAMEN = MotivacionDictamenPsicologicoComun;
                    _EstudioPsico.P11_CASO_NEGATIVO = CasoNegativoEstudioPsicologicoComun;
                    _EstudioPsico.P12_CUAL = CualTratamientoExtraMurosPsicologicoComun;
                    _EstudioPsico.P12_REQUIERE_TRATAMIENTO = IdReqExtraMurosPsicologicoComun;
                    _EstudioPsico.P2_EXAMEN_MENTAL = ExamenMental;
                    _EstudioPsico.P3_PRINCIPALES_RASGOS = DescripcionPrincipalesRazgosIngreso;
                    _EstudioPsico.P4_INVENTARIO_MULTIFASICO = IsTestMinesotaChecked == true ? (short)eSINO.SI : (short)eSINO.NO;
                    _EstudioPsico.P4_OTRAS = IsTestOtrosChecked == true ? (short)eSINO.SI : (short)eSINO.NO;
                    _EstudioPsico.P4_TEST_GUALTICO = IsTestLaurettaBenderChecked == true ? (short)eSINO.SI : (short)eSINO.NO;
                    _EstudioPsico.P4_TEST_HTP = IsTestCasaArbolPersonaChecked == true ? (short)eSINO.SI : (short)eSINO.NO;
                    _EstudioPsico.P4_TEST_MATRICES = IsTestMatricesProgresivasRavenChecked == true ? (short)eSINO.SI : (short)eSINO.NO;
                    _EstudioPsico.P51_NIVEL_INTELECTUAL = IdNivelIntelectual;
                    _EstudioPsico.P52_DISFUNCION_NEUROLOGICA = IdDisfuncionNeurologica;
                    _EstudioPsico.P6_INTEGRACION = IntegracionDinamicaPersonalidadActual;
                    _EstudioPsico.P8_RASGOS_PERSONALIDAD = RasgosPersonalidadRelaciondosComisionDelito;
                    _EstudioPsico.P9_DICTAMEN_REINSERCION = IdDictamenPsicologicoComun;
                    _EstudioPsico.ELABORO = NombreUsuario != null ? NombreUsuario.EMPLEADO != null ? NombreUsuario.EMPLEADO.PERSONA != null ? string.Format("{0} {1} {2}",
                        !string.IsNullOrEmpty(NombreUsuario.EMPLEADO.PERSONA.NOMBRE) ? NombreUsuario.EMPLEADO.PERSONA.NOMBRE.Trim() : string.Empty,
                        !string.IsNullOrEmpty(NombreUsuario.EMPLEADO.PERSONA.PATERNO) ? NombreUsuario.EMPLEADO.PERSONA.PATERNO.Trim() : string.Empty,
                        !string.IsNullOrEmpty(NombreUsuario.EMPLEADO.PERSONA.MATERNO) ? NombreUsuario.EMPLEADO.PERSONA.MATERNO.Trim() : string.Empty) : string.Empty : string.Empty : string.Empty;
                    _EstudioPsico.P4_OTRA_MENCIONAR = EspecifiqueOtroTest;
                    _EstudioPsico.COORDINADOR = NombreCoordinadorPsicologico != null ? NombreCoordinadorPsicologico.USUARIO != null ? NombreCoordinadorPsicologico.USUARIO.EMPLEADO != null ? NombreCoordinadorPsicologico.USUARIO.EMPLEADO.PERSONA != null ?
                        string.Format("{0} {1} {2} ", !string.IsNullOrEmpty(NombreCoordinadorPsicologico.USUARIO.EMPLEADO.PERSONA.NOMBRE) ? NombreCoordinadorPsicologico.USUARIO.EMPLEADO.PERSONA.NOMBRE.Trim() : string.Empty,
                        !string.IsNullOrEmpty(NombreCoordinadorPsicologico.USUARIO.EMPLEADO.PERSONA.PATERNO) ? NombreCoordinadorPsicologico.USUARIO.EMPLEADO.PERSONA.PATERNO.Trim() : string.Empty,
                        !string.IsNullOrEmpty(NombreCoordinadorPsicologico.USUARIO.EMPLEADO.PERSONA.MATERNO) ? NombreCoordinadorPsicologico.USUARIO.EMPLEADO.PERSONA.MATERNO.Trim() : string.Empty) : string.Empty : string.Empty : string.Empty : string.Empty;

                    if (LstProgramasPsicologico != null && LstProgramasPsicologico.Any())
                        foreach (var item in LstProgramasPsicologico)
                            Progra.Add(item);

                    if (LstProgModifConduc != null && LstProgModifConduc.Any())
                        foreach (var item in LstProgModifConduc)
                            Progra.Add(item);

                    if (LstComplement != null && LstComplement.Any())
                        foreach (var item in LstComplement)
                            Progra.Add(item);

                    if (LstTalleresOrient != null && LstTalleresOrient.Any())
                        foreach (var item in LstTalleresOrient)
                            Progra.Add(item);

                    if (_EstudioPsico.PFC_IV_PROGRAMA != null && _EstudioPsico.PFC_IV_PROGRAMA.Any())
                        _EstudioPsico.PFC_IV_PROGRAMA.Clear();

                    _EstudioPsico.PFC_IV_PROGRAMA = Progra;
                    return _EstudioPsico;
                }
            }
            catch (Exception exc)
            {
                throw;
            }
        }

        private PFC_V_CRIMINODIAGNOSTICO EstudioCriminodTransaccionIndividual(PERSONALIDAD Entity)
        {
            var NombreUsuario = new cUsuario().Obtener(GlobalVar.gUsr);
            var _UsuarioCoordinador = new cUsuarioRol().GetData(x => x.ID_ROL == (short)eRolesCoordinadores.COORDINADOR_CRIMINODIAGNOSTICO).FirstOrDefault();
            var _EstudioCrimin = new cPersonalidadEstudioCriminodiagnosticoComun().GetData(x => x.ID_INGRESO == SelectIngreso.ID_INGRESO && x.ID_IMPUTADO == SelectIngreso.ID_IMPUTADO && x.ID_CENTRO == SelectIngreso.ID_CENTRO && x.ID_ANIO == SelectIngreso.ID_ANIO && x.ID_ANIO == SelectIngreso.ID_ANIO && x.ID_ESTUDIO == Entity.ID_ESTUDIO).FirstOrDefault();
            if (_EstudioCrimin == null)
            {
                var _Crimin = new PFC_V_CRIMINODIAGNOSTICO()
                {
                    ESTUDIO_FEC = FechaDictamenCrimino,
                    ID_ANIO = SelectIngreso.ID_ANIO,
                    ID_CENTRO = SelectIngreso.ID_CENTRO,
                    ID_IMPUTADO = SelectIngreso.ID_IMPUTADO,
                    ID_INGRESO = SelectIngreso.ID_INGRESO,
                    P1_ALCOHOL = IsAlcoholChecked == true ? (short)eSINO.SI : (short)eSINO.NO,
                    P1_DROGADO = IdEncontrabaBajoInfluenciaDroga,
                    P1_DROGRA_ILEGAL = IsDrogasIlegalesChecked == true ? (short)eSINO.SI : (short)eSINO.NO,
                    P1_OTRA = IsOtraChecked == true ? (short)eSINO.SI : (short)eSINO.NO,
                    P1_VERSION_DELITO = VersionDelitoSegunInterno,
                    P10_DICTAMEN_REINSERCION = DictamenCriminod,
                    P10_MOTIVACION_DICTAMEN = MotivacionDictamenCriminodiagnosticoComun,
                    P11_PROGRAMAS_REMITIRSE = SenialeProgramasDebeRemitirseInterno,
                    P12_CUAL = CualTratamRemitirCriminodiagnosticoComun,
                    P12_TRATAMIENTO_EXTRAMUROS = ReqTratamExtramurosCriminod,
                    P2_CRIMINOGENESIS = CriminoGenesisEstudioCriminoFC,
                    P3_CONDUCTA_ANTISOCIAL = AntecedentesEvolucionConductasParaSociales,
                    P4_CLASIFICACION_CRIMINOLOGICA = IdClasificacionCriminologica,
                    P5_INTIMIDACION = IntimidacionAntePenaImpuesta,
                    P5_PORQUE = PorqueIntimidacionAntePenaImpuesta,
                    P6_CAPACIDAD_CRIMINAL = IdCapacidadCriminologicaActual,
                    P6A_EGOCENTRICO = IdEgocentrismo,
                    P6B_LIABILIDAD_EFECTIVA = IdLabilidadAfectiva,
                    P6C_AGRESIVIDAD = IdAgresividad,
                    P6D_INDIFERENCIA_AFECTIVA = IdIndiferenciaAfectiva,
                    P7_ADAPTACION_SOCIAL = IdAdaptabilidadSocial,
                    P8_INDICE_PELIGROSIDAD = IdIndicePeligrosidadCriminologicaActual,
                    P9_PRONOSTICO_REINCIDENCIA = IdPronosticoReincidencia,
                    ELABORO = NombreUsuario != null ? NombreUsuario.EMPLEADO != null ? NombreUsuario.EMPLEADO.PERSONA != null ? string.Format("{0} {1} {2}",
                        !string.IsNullOrEmpty(NombreUsuario.EMPLEADO.PERSONA.NOMBRE) ? NombreUsuario.EMPLEADO.PERSONA.NOMBRE.Trim() : string.Empty,
                        !string.IsNullOrEmpty(NombreUsuario.EMPLEADO.PERSONA.PATERNO) ? NombreUsuario.EMPLEADO.PERSONA.PATERNO.Trim() : string.Empty,
                        !string.IsNullOrEmpty(NombreUsuario.EMPLEADO.PERSONA.MATERNO) ? NombreUsuario.EMPLEADO.PERSONA.MATERNO.Trim() : string.Empty) : string.Empty : string.Empty : string.Empty,
                    COORDINADOR = _UsuarioCoordinador != null ? _UsuarioCoordinador.USUARIO != null ? _UsuarioCoordinador.USUARIO.EMPLEADO != null ? _UsuarioCoordinador.USUARIO.EMPLEADO.PERSONA != null ?
                        string.Format("{0} {1} {2}",
                            !string.IsNullOrEmpty(_UsuarioCoordinador.USUARIO.EMPLEADO.PERSONA.NOMBRE) ? _UsuarioCoordinador.USUARIO.EMPLEADO.PERSONA.NOMBRE.Trim() : string.Empty,
                            !string.IsNullOrEmpty(_UsuarioCoordinador.USUARIO.EMPLEADO.PERSONA.PATERNO) ? _UsuarioCoordinador.USUARIO.EMPLEADO.PERSONA.PATERNO.Trim() : string.Empty,
                            !string.IsNullOrEmpty(_UsuarioCoordinador.USUARIO.EMPLEADO.PERSONA.MATERNO) ? _UsuarioCoordinador.USUARIO.EMPLEADO.PERSONA.MATERNO.Trim() : string.Empty) : string.Empty : string.Empty : string.Empty : string.Empty
                };

                return _Crimin;
            }

            else
            {
                _EstudioCrimin.ESTUDIO_FEC = FechaDictamenCrimino;
                _EstudioCrimin.ID_ANIO = SelectIngreso.ID_ANIO;
                _EstudioCrimin.ID_CENTRO = SelectIngreso.ID_CENTRO;
                _EstudioCrimin.ID_IMPUTADO = SelectIngreso.ID_IMPUTADO;
                _EstudioCrimin.ID_INGRESO = SelectIngreso.ID_INGRESO;
                _EstudioCrimin.P1_ALCOHOL = IsAlcoholChecked == true ? (short)eSINO.SI : (short)eSINO.NO;
                _EstudioCrimin.P1_DROGADO = IdEncontrabaBajoInfluenciaDroga;
                _EstudioCrimin.P1_DROGRA_ILEGAL = IsDrogasIlegalesChecked == true ? (short)eSINO.SI : (short)eSINO.NO;
                _EstudioCrimin.P1_OTRA = IsOtraChecked == true ? (short)eSINO.SI : (short)eSINO.NO;
                _EstudioCrimin.P1_VERSION_DELITO = VersionDelitoSegunInterno;
                _EstudioCrimin.P10_DICTAMEN_REINSERCION = DictamenCriminod;
                _EstudioCrimin.P10_MOTIVACION_DICTAMEN = MotivacionDictamenCriminodiagnosticoComun;
                _EstudioCrimin.P11_PROGRAMAS_REMITIRSE = SenialeProgramasDebeRemitirseInterno;
                _EstudioCrimin.P12_CUAL = CualTratamRemitirCriminodiagnosticoComun;
                _EstudioCrimin.P12_TRATAMIENTO_EXTRAMUROS = ReqTratamExtramurosCriminod;
                _EstudioCrimin.P2_CRIMINOGENESIS = CriminoGenesisEstudioCriminoFC;
                _EstudioCrimin.P3_CONDUCTA_ANTISOCIAL = AntecedentesEvolucionConductasParaSociales;
                _EstudioCrimin.P4_CLASIFICACION_CRIMINOLOGICA = IdClasificacionCriminologica;
                _EstudioCrimin.P5_INTIMIDACION = IntimidacionAntePenaImpuesta;
                _EstudioCrimin.P5_PORQUE = PorqueIntimidacionAntePenaImpuesta;
                _EstudioCrimin.P6_CAPACIDAD_CRIMINAL = IdCapacidadCriminologicaActual;
                _EstudioCrimin.P6A_EGOCENTRICO = IdEgocentrismo;
                _EstudioCrimin.P6B_LIABILIDAD_EFECTIVA = IdLabilidadAfectiva;
                _EstudioCrimin.P6C_AGRESIVIDAD = IdAgresividad;
                _EstudioCrimin.P6D_INDIFERENCIA_AFECTIVA = IdIndiferenciaAfectiva;
                _EstudioCrimin.P7_ADAPTACION_SOCIAL = IdAdaptabilidadSocial;
                _EstudioCrimin.P8_INDICE_PELIGROSIDAD = IdIndicePeligrosidadCriminologicaActual;
                _EstudioCrimin.P9_PRONOSTICO_REINCIDENCIA = IdPronosticoReincidencia;
                _EstudioCrimin.ELABORO = NombreUsuario != null ? NombreUsuario.EMPLEADO != null ? NombreUsuario.EMPLEADO.PERSONA != null ? string.Format("{0} {1} {2}",
                        !string.IsNullOrEmpty(NombreUsuario.EMPLEADO.PERSONA.NOMBRE) ? NombreUsuario.EMPLEADO.PERSONA.NOMBRE.Trim() : string.Empty,
                        !string.IsNullOrEmpty(NombreUsuario.EMPLEADO.PERSONA.PATERNO) ? NombreUsuario.EMPLEADO.PERSONA.PATERNO.Trim() : string.Empty,
                        !string.IsNullOrEmpty(NombreUsuario.EMPLEADO.PERSONA.MATERNO) ? NombreUsuario.EMPLEADO.PERSONA.MATERNO.Trim() : string.Empty) : string.Empty : string.Empty : string.Empty;
                _EstudioCrimin.COORDINADOR = _UsuarioCoordinador != null ? _UsuarioCoordinador.USUARIO != null ? _UsuarioCoordinador.USUARIO.EMPLEADO != null ? _UsuarioCoordinador.USUARIO.EMPLEADO.PERSONA != null ?
                        string.Format("{0} {1} {2}",
                            !string.IsNullOrEmpty(_UsuarioCoordinador.USUARIO.EMPLEADO.PERSONA.NOMBRE) ? _UsuarioCoordinador.USUARIO.EMPLEADO.PERSONA.NOMBRE.Trim() : string.Empty,
                            !string.IsNullOrEmpty(_UsuarioCoordinador.USUARIO.EMPLEADO.PERSONA.PATERNO) ? _UsuarioCoordinador.USUARIO.EMPLEADO.PERSONA.PATERNO.Trim() : string.Empty,
                            !string.IsNullOrEmpty(_UsuarioCoordinador.USUARIO.EMPLEADO.PERSONA.MATERNO) ? _UsuarioCoordinador.USUARIO.EMPLEADO.PERSONA.MATERNO.Trim() : string.Empty) : string.Empty : string.Empty : string.Empty : string.Empty;
                return _EstudioCrimin;
            }
        }

        private PFC_VI_SOCIO_FAMILIAR EstudioSocioFamiliarTransaccionIndividual(PERSONALIDAD Entity)
        {
            try
            {
                var NombreUsuario = new cUsuario().Obtener(GlobalVar.gUsr);
                var _UsuarioCoordinador = new cUsuarioRol().GetData(x => x.ID_ROL == (short)eRolesCoordinadores.COORDINADOR_TRABAJO_SOCAL).FirstOrDefault();
                var _EstudioSocioFam = new cPersonalidadEstudioSocioFamiliarComun().GetData(x => x.ID_INGRESO == SelectIngreso.ID_INGRESO && x.ID_IMPUTADO == SelectIngreso.ID_IMPUTADO && x.ID_CENTRO == SelectIngreso.ID_CENTRO && x.ID_ANIO == SelectIngreso.ID_ANIO && x.ID_ANIO == SelectIngreso.ID_ANIO && x.ID_ESTUDIO == Entity.ID_ESTUDIO).FirstOrDefault();
                if (_EstudioSocioFam == null)
                {
                    var _EstudioSoc = new PFC_VI_SOCIO_FAMILIAR()
                    {
                        ID_ANIO = selectIngreso.ID_ANIO,
                        ID_CENTRO = selectIngreso.ID_CENTRO,
                        ID_IMPUTADO = selectIngreso.ID_IMPUTADO,
                        ID_INGRESO = selectIngreso.ID_INGRESO,
                        P21_FAMILIA_PRIMARIA = FamiliaPrimaria,
                        P22_FAMILIA_SECUNDARIA = FamiliaSecundaria,
                        P3_TERCERA_EDAD = IsAdultoMayorParticipoEnProgramaEspecial,
                        P4_ESPOSOA = IsEsposoConcubinaChecked == true ? (short)eSINO.SI : (short)eSINO.NO,
                        P4_FRECUENCIA = FrecuenciaVisita,
                        P4_HERMANOS = IsHermanosChecked == true ? (short)eSINO.SI : (short)eSINO.NO,
                        P4_HIJOS = IsHijosChecked == true ? (short)eSINO.SI : (short)eSINO.NO,
                        P4_MADRE = IsMadreVisitaChecked == true ? (short)eSINO.SI : (short)eSINO.NO,
                        P4_MOTIVO_NO_VISITA = RazonNoRecibeVisitas,
                        P4_OTROS = IsOtrosVisitaChecked == true ? (short)eSINO.SI : (short)eSINO.NO,
                        P4_OTROS_EPECIFICAR = EspecificarQuienVisita,
                        P4_PADRE = IsPadreVisitaCheckd == true ? (short)eSINO.SI : (short)eSINO.NO,
                        P4_RECIBE_VISITA = IdRecibeVisitaSocioFamComun,
                        P10_DICTAMEN = IdDictamenSocioFamComun,
                        P5_COMUNICACION_TELEFONICA = IdComunicacionViaTelChecked,
                        P5_NO_POR_QUE = EspecifiqueViaTelefonica,
                        P6_APOYO_EXTERIOR = ApoyosRecibeExterior,
                        P7_PLANES_INTERNO = PlanesInternoAlSerExternado,
                        P7_VIVIRA = ConQuienVivirSerExternado,
                        P8_OFERTA_ESPECIFICAR = EspecifiqueOfertaTrabajo,
                        P8_OFERTA_TRABAJO = IdOfertaTrabajoChecked,
                        P9_AVAL_ESPECIFICAR = EspecifiqueAvalMoral,
                        P9_AVAL_MORAL = IdAvalMoralChecked,
                        ESTUDIO_FEC = FechaEstudioSocioFamiliarComun,
                        P11_MOTIVACION_DICTAMEN = MotivacionDictamenSocioEconomicoComun,
                        ELABORO = NombreUsuario != null ? NombreUsuario.EMPLEADO != null ? NombreUsuario.EMPLEADO.PERSONA != null ? string.Format("{0} {1} {2}",
                            !string.IsNullOrEmpty(NombreUsuario.EMPLEADO.PERSONA.NOMBRE) ? NombreUsuario.EMPLEADO.PERSONA.NOMBRE.Trim() : string.Empty,
                            !string.IsNullOrEmpty(NombreUsuario.EMPLEADO.PERSONA.PATERNO) ? NombreUsuario.EMPLEADO.PERSONA.PATERNO.Trim() : string.Empty,
                            !string.IsNullOrEmpty(NombreUsuario.EMPLEADO.PERSONA.MATERNO) ? NombreUsuario.EMPLEADO.PERSONA.MATERNO.Trim() : string.Empty) : string.Empty : string.Empty : string.Empty,
                        COORDINADOR = _UsuarioCoordinador != null ? _UsuarioCoordinador.USUARIO != null ? _UsuarioCoordinador.USUARIO.EMPLEADO != null ? _UsuarioCoordinador.USUARIO.EMPLEADO.PERSONA != null ?
                            string.Format("{0} {1} {2}",
                                !string.IsNullOrEmpty(_UsuarioCoordinador.USUARIO.EMPLEADO.PERSONA.NOMBRE) ? _UsuarioCoordinador.USUARIO.EMPLEADO.PERSONA.NOMBRE.Trim() : string.Empty,
                                !string.IsNullOrEmpty(_UsuarioCoordinador.USUARIO.EMPLEADO.PERSONA.PATERNO) ? _UsuarioCoordinador.USUARIO.EMPLEADO.PERSONA.PATERNO.Trim() : string.Empty,
                                !string.IsNullOrEmpty(_UsuarioCoordinador.USUARIO.EMPLEADO.PERSONA.MATERNO) ? _UsuarioCoordinador.USUARIO.EMPLEADO.PERSONA.MATERNO.Trim() : string.Empty) : string.Empty : string.Empty : string.Empty : string.Empty,
                    };

                    var HistoricoVisitas = new ObservableCollection<PFC_VI_COMUNICACION>();
                    var GruposSoc = new ObservableCollection<PFC_VI_GRUPO>();

                    if (LstComunicaciones != null && LstComunicaciones.Any())
                        foreach (var item in LstComunicaciones)
                            HistoricoVisitas.Add(item);

                    if (ListGruposSocioFamComun != null && ListGruposSocioFamComun.Any())
                        foreach (var item in ListGruposSocioFamComun)
                            GruposSoc.Add(item);

                    if (ListFortalecimientoSocioFamComun != null && ListFortalecimientoSocioFamComun.Any())
                        foreach (var item in ListFortalecimientoSocioFamComun)
                            GruposSoc.Add(item);

                    if (_EstudioSoc.PFC_VI_COMUNICACION != null && _EstudioSoc.PFC_VI_COMUNICACION.Any())
                        _EstudioSoc.PFC_VI_COMUNICACION.Clear();

                    if (_EstudioSoc.PFC_VI_GRUPO != null && _EstudioSoc.PFC_VI_GRUPO.Any())
                        _EstudioSoc.PFC_VI_GRUPO.Clear();

                    _EstudioSoc.PFC_VI_COMUNICACION = HistoricoVisitas;
                    _EstudioSoc.PFC_VI_GRUPO = GruposSoc;

                    return _EstudioSoc;
                }
                else
                {
                    _EstudioSocioFam.ID_ANIO = selectIngreso.ID_ANIO;
                    _EstudioSocioFam.ID_CENTRO = selectIngreso.ID_CENTRO;
                    _EstudioSocioFam.ID_IMPUTADO = selectIngreso.ID_IMPUTADO;
                    _EstudioSocioFam.ID_INGRESO = selectIngreso.ID_INGRESO;
                    _EstudioSocioFam.P21_FAMILIA_PRIMARIA = FamiliaPrimaria;
                    _EstudioSocioFam.P22_FAMILIA_SECUNDARIA = FamiliaSecundaria;
                    _EstudioSocioFam.P3_TERCERA_EDAD = IsAdultoMayorParticipoEnProgramaEspecial;
                    _EstudioSocioFam.P4_ESPOSOA = IsEsposoConcubinaChecked == true ? (short)eSINO.SI : (short)eSINO.NO;
                    _EstudioSocioFam.P4_FRECUENCIA = FrecuenciaVisita;
                    _EstudioSocioFam.P4_HERMANOS = IsHermanosChecked == true ? (short)eSINO.SI : (short)eSINO.NO;
                    _EstudioSocioFam.P4_HIJOS = IsHijosChecked == true ? (short)eSINO.SI : (short)eSINO.NO;
                    _EstudioSocioFam.P4_MADRE = IsMadreVisitaChecked == true ? (short)eSINO.SI : (short)eSINO.NO;
                    _EstudioSocioFam.P4_MOTIVO_NO_VISITA = RazonNoRecibeVisitas;
                    _EstudioSocioFam.P4_OTROS = IsOtrosVisitaChecked == true ? (short)eSINO.SI : (short)eSINO.NO;
                    _EstudioSocioFam.P4_OTROS_EPECIFICAR = EspecificarQuienVisita;
                    _EstudioSocioFam.P4_PADRE = IsPadreVisitaCheckd == true ? (short)eSINO.SI : (short)eSINO.NO;
                    _EstudioSocioFam.P4_RECIBE_VISITA = IdRecibeVisitaSocioFamComun;
                    _EstudioSocioFam.P10_DICTAMEN = IdDictamenSocioFamComun;
                    _EstudioSocioFam.P5_COMUNICACION_TELEFONICA = IdComunicacionViaTelChecked;
                    _EstudioSocioFam.P5_NO_POR_QUE = EspecifiqueViaTelefonica;
                    _EstudioSocioFam.P6_APOYO_EXTERIOR = ApoyosRecibeExterior;
                    _EstudioSocioFam.P7_PLANES_INTERNO = PlanesInternoAlSerExternado;
                    _EstudioSocioFam.P7_VIVIRA = ConQuienVivirSerExternado;
                    _EstudioSocioFam.P8_OFERTA_ESPECIFICAR = EspecifiqueOfertaTrabajo;
                    _EstudioSocioFam.P8_OFERTA_TRABAJO = IdOfertaTrabajoChecked;
                    _EstudioSocioFam.P9_AVAL_ESPECIFICAR = EspecifiqueAvalMoral;
                    _EstudioSocioFam.P9_AVAL_MORAL = IdAvalMoralChecked;
                    _EstudioSocioFam.ESTUDIO_FEC = FechaEstudioSocioFamiliarComun;
                    _EstudioSocioFam.P11_MOTIVACION_DICTAMEN = MotivacionDictamenSocioEconomicoComun;
                    _EstudioSocioFam.ELABORO = NombreUsuario != null ? NombreUsuario.EMPLEADO != null ? NombreUsuario.EMPLEADO.PERSONA != null ? string.Format("{0} {1} {2}",
                        !string.IsNullOrEmpty(NombreUsuario.EMPLEADO.PERSONA.NOMBRE) ? NombreUsuario.EMPLEADO.PERSONA.NOMBRE.Trim() : string.Empty,
                        !string.IsNullOrEmpty(NombreUsuario.EMPLEADO.PERSONA.PATERNO) ? NombreUsuario.EMPLEADO.PERSONA.PATERNO.Trim() : string.Empty,
                        !string.IsNullOrEmpty(NombreUsuario.EMPLEADO.PERSONA.MATERNO) ? NombreUsuario.EMPLEADO.PERSONA.MATERNO.Trim() : string.Empty) : string.Empty : string.Empty : string.Empty;
                    _EstudioSocioFam.COORDINADOR = _UsuarioCoordinador != null ? _UsuarioCoordinador.USUARIO != null ? _UsuarioCoordinador.USUARIO.EMPLEADO != null ? _UsuarioCoordinador.USUARIO.EMPLEADO.PERSONA != null ?
                            string.Format("{0} {1} {2}",
                                !string.IsNullOrEmpty(_UsuarioCoordinador.USUARIO.EMPLEADO.PERSONA.NOMBRE) ? _UsuarioCoordinador.USUARIO.EMPLEADO.PERSONA.NOMBRE.Trim() : string.Empty,
                                !string.IsNullOrEmpty(_UsuarioCoordinador.USUARIO.EMPLEADO.PERSONA.PATERNO) ? _UsuarioCoordinador.USUARIO.EMPLEADO.PERSONA.PATERNO.Trim() : string.Empty,
                                !string.IsNullOrEmpty(_UsuarioCoordinador.USUARIO.EMPLEADO.PERSONA.MATERNO) ? _UsuarioCoordinador.USUARIO.EMPLEADO.PERSONA.MATERNO.Trim() : string.Empty) : string.Empty : string.Empty : string.Empty : string.Empty;

                    var HistoricoVisitas = new ObservableCollection<PFC_VI_COMUNICACION>();
                    var GruposSoc = new ObservableCollection<PFC_VI_GRUPO>();

                    if (LstComunicaciones != null && LstComunicaciones.Any())
                        foreach (var item in LstComunicaciones)
                            HistoricoVisitas.Add(item);

                    if (ListGruposSocioFamComun != null && ListGruposSocioFamComun.Any())
                        foreach (var item in ListGruposSocioFamComun)
                            GruposSoc.Add(item);

                    if (ListFortalecimientoSocioFamComun != null && ListFortalecimientoSocioFamComun.Any())
                        foreach (var item in ListFortalecimientoSocioFamComun)
                            GruposSoc.Add(item);

                    if (_EstudioSocioFam.PFC_VI_COMUNICACION != null && _EstudioSocioFam.PFC_VI_COMUNICACION.Any())
                        _EstudioSocioFam.PFC_VI_COMUNICACION.Clear();

                    if (_EstudioSocioFam.PFC_VI_GRUPO != null && _EstudioSocioFam.PFC_VI_GRUPO.Any())
                        _EstudioSocioFam.PFC_VI_GRUPO.Clear();

                    _EstudioSocioFam.PFC_VI_COMUNICACION = HistoricoVisitas;
                    _EstudioSocioFam.PFC_VI_GRUPO = GruposSoc;

                    return _EstudioSocioFam;
                }
            }
            catch (Exception exc)
            {
                throw exc;
            }
        }
        private PFC_VII_EDUCATIVO EstudioEducativoTransaccionIndividual(PERSONALIDAD Entity)
        {
            try
            {
                var NombreUsuario = new cUsuario().Obtener(GlobalVar.gUsr);
                var _UsuarioCoordinador = new cUsuarioRol().GetData(x => x.ID_ROL == (short)eRolesCoordinadores.COORDINADOR_EDUCATIVO).FirstOrDefault();
                var _EstudioEduc = new cPersonalidadEstudioEducativoCultDepComun().GetData(x => x.ID_INGRESO == SelectIngreso.ID_INGRESO && x.ID_IMPUTADO == SelectIngreso.ID_IMPUTADO && x.ID_CENTRO == SelectIngreso.ID_CENTRO && x.ID_ANIO == SelectIngreso.ID_ANIO && x.ID_ANIO == SelectIngreso.ID_ANIO && x.ID_ESTUDIO == Entity.ID_ESTUDIO).FirstOrDefault();
                if (_EstudioEduc == null)
                {
                    var _Educ = new PFC_VII_EDUCATIVO()
                    {
                        ID_ANIO = SelectIngreso.ID_ANIO,
                        ID_CENTRO = SelectIngreso.ID_CENTRO,
                        ID_IMPUTADO = SelectIngreso.ID_IMPUTADO,
                        ID_INGRESO = SelectIngreso.ID_INGRESO,
                        P3_DICTAMEN = IdDictamenEducativoComun,
                        P4_MOTIVACION_DICTAMEN = MotivacionDictamenEducativoComun,
                        ESTUDIO_FEC = FechaEstudioEducativoComun,
                        ELABORO = NombreUsuario != null ? NombreUsuario.EMPLEADO != null ? NombreUsuario.EMPLEADO.PERSONA != null ? string.Format("{0} {1} {2}",
                            !string.IsNullOrEmpty(NombreUsuario.EMPLEADO.PERSONA.NOMBRE) ? NombreUsuario.EMPLEADO.PERSONA.NOMBRE.Trim() : string.Empty,
                            !string.IsNullOrEmpty(NombreUsuario.EMPLEADO.PERSONA.PATERNO) ? NombreUsuario.EMPLEADO.PERSONA.PATERNO.Trim() : string.Empty,
                            !string.IsNullOrEmpty(NombreUsuario.EMPLEADO.PERSONA.MATERNO) ? NombreUsuario.EMPLEADO.PERSONA.MATERNO.Trim() : string.Empty) : string.Empty : string.Empty : string.Empty,
                        COORDINADOR = _UsuarioCoordinador != null ? _UsuarioCoordinador.USUARIO != null ? _UsuarioCoordinador.USUARIO.EMPLEADO != null ? _UsuarioCoordinador.USUARIO.EMPLEADO.PERSONA != null ?
                            string.Format("{0} {1} {2}",
                                !string.IsNullOrEmpty(_UsuarioCoordinador.USUARIO.EMPLEADO.PERSONA.NOMBRE) ? _UsuarioCoordinador.USUARIO.EMPLEADO.PERSONA.NOMBRE.Trim() : string.Empty,
                                !string.IsNullOrEmpty(_UsuarioCoordinador.USUARIO.EMPLEADO.PERSONA.PATERNO) ? _UsuarioCoordinador.USUARIO.EMPLEADO.PERSONA.PATERNO.Trim() : string.Empty,
                                !string.IsNullOrEmpty(_UsuarioCoordinador.USUARIO.EMPLEADO.PERSONA.MATERNO) ? _UsuarioCoordinador.USUARIO.EMPLEADO.PERSONA.MATERNO.Trim() : string.Empty) : string.Empty : string.Empty : string.Empty : string.Empty
                    };

                    if (LstEscolaridadesEducativo != null && LstEscolaridadesEducativo.Any())
                        foreach (var item in LstEscolaridadesEducativo)
                            _Educ.PFC_VII_ESCOLARIDAD_ANTERIOR.Add(item);

                    if (LstAcitividadesCulturales != null && LstAcitividadesCulturales.Any())
                    {
                        LstAcitividadesCulturales.ToList().ForEach(x => x.TIPO = "C");
                        foreach (var item in LstAcitividadesCulturales)
                            _Educ.PFC_VII_ACTIVIDAD.Add(item);
                    }

                    if (LstActividadesEducativas != null && LstActividadesEducativas.Any())
                        foreach (var item in LstActividadesEducativas)
                            _Educ.PFC_VII_ESCOLARIDAD_ANTERIOR.Add(item);

                    if (LstActividadesDeportivas != null && LstActividadesDeportivas.Any())
                    {
                        LstActividadesDeportivas.ToList().ForEach(x => x.TIPO = "D");
                        foreach (var item in LstActividadesDeportivas)
                            _Educ.PFC_VII_ACTIVIDAD.Add(item);
                    };

                    return _Educ;
                }

                else
                {
                    _EstudioEduc.ID_ANIO = SelectIngreso.ID_ANIO;
                    _EstudioEduc.ID_CENTRO = SelectIngreso.ID_CENTRO;
                    _EstudioEduc.ID_IMPUTADO = SelectIngreso.ID_IMPUTADO;
                    _EstudioEduc.ID_INGRESO = SelectIngreso.ID_INGRESO;
                    _EstudioEduc.P3_DICTAMEN = IdDictamenEducativoComun;
                    _EstudioEduc.P4_MOTIVACION_DICTAMEN = MotivacionDictamenEducativoComun;
                    _EstudioEduc.ESTUDIO_FEC = FechaEstudioEducativoComun;
                    _EstudioEduc.ELABORO = NombreUsuario != null ? NombreUsuario.EMPLEADO != null ? NombreUsuario.EMPLEADO.PERSONA != null ? string.Format("{0} {1} {2}",
                        !string.IsNullOrEmpty(NombreUsuario.EMPLEADO.PERSONA.NOMBRE) ? NombreUsuario.EMPLEADO.PERSONA.NOMBRE.Trim() : string.Empty,
                        !string.IsNullOrEmpty(NombreUsuario.EMPLEADO.PERSONA.PATERNO) ? NombreUsuario.EMPLEADO.PERSONA.PATERNO.Trim() : string.Empty,
                        !string.IsNullOrEmpty(NombreUsuario.EMPLEADO.PERSONA.MATERNO) ? NombreUsuario.EMPLEADO.PERSONA.MATERNO.Trim() : string.Empty) : string.Empty : string.Empty : string.Empty;
                    _EstudioEduc.COORDINADOR = _UsuarioCoordinador != null ? _UsuarioCoordinador.USUARIO != null ? _UsuarioCoordinador.USUARIO.EMPLEADO != null ? _UsuarioCoordinador.USUARIO.EMPLEADO.PERSONA != null ?
                            string.Format("{0} {1} {2}",
                                !string.IsNullOrEmpty(_UsuarioCoordinador.USUARIO.EMPLEADO.PERSONA.NOMBRE) ? _UsuarioCoordinador.USUARIO.EMPLEADO.PERSONA.NOMBRE.Trim() : string.Empty,
                                !string.IsNullOrEmpty(_UsuarioCoordinador.USUARIO.EMPLEADO.PERSONA.PATERNO) ? _UsuarioCoordinador.USUARIO.EMPLEADO.PERSONA.PATERNO.Trim() : string.Empty,
                                !string.IsNullOrEmpty(_UsuarioCoordinador.USUARIO.EMPLEADO.PERSONA.MATERNO) ? _UsuarioCoordinador.USUARIO.EMPLEADO.PERSONA.MATERNO.Trim() : string.Empty) : string.Empty : string.Empty : string.Empty : string.Empty;

                    var DatosListaUno = new ObservableCollection<PFC_VII_ESCOLARIDAD_ANTERIOR>();
                    var DatosListaDos = new ObservableCollection<PFC_VII_ACTIVIDAD>();

                    if (LstEscolaridadesEducativo != null && LstEscolaridadesEducativo.Any())
                        foreach (var item in LstEscolaridadesEducativo)
                            DatosListaUno.Add(item);

                    if (LstAcitividadesCulturales != null && LstAcitividadesCulturales.Any())
                    {
                        LstAcitividadesCulturales.ToList().ForEach(x => x.TIPO = "C");
                        foreach (var item in LstAcitividadesCulturales)
                            DatosListaDos.Add(item);
                    };

                    if (LstActividadesEducativas != null && LstActividadesEducativas.Any())
                        foreach (var item in LstActividadesEducativas)
                            DatosListaUno.Add(item);

                    if (LstActividadesDeportivas != null && LstActividadesDeportivas.Any())
                    {
                        LstActividadesDeportivas.ToList().ForEach(x => x.TIPO = "D");
                        foreach (var item in LstActividadesDeportivas)
                            DatosListaDos.Add(item);
                    }

                    if (_EstudioEduc.PFC_VII_ESCOLARIDAD_ANTERIOR != null && _EstudioEduc.PFC_VII_ESCOLARIDAD_ANTERIOR.Any())
                        _EstudioEduc.PFC_VII_ESCOLARIDAD_ANTERIOR.Clear();

                    if (_EstudioEduc.PFC_VII_ACTIVIDAD != null && _EstudioEduc.PFC_VII_ACTIVIDAD.Any())
                        _EstudioEduc.PFC_VII_ACTIVIDAD.Clear();

                    _EstudioEduc.PFC_VII_ACTIVIDAD = DatosListaDos;
                    _EstudioEduc.PFC_VII_ESCOLARIDAD_ANTERIOR = DatosListaUno;
                    return _EstudioEduc;
                }
            }
            catch (Exception exc)
            {
                throw exc;
            }
        }

        private PFC_VIII_TRABAJO EstudioTrabajoTransaccionIndividual(PERSONALIDAD Entity)
        {
            try
            {
                var NombreUsuario = new cUsuario().Obtener(GlobalVar.gUsr);
                var _UsuarioCoordinador = new cUsuarioRol().GetData(x => x.ID_ROL == (short)eRolesCoordinadores.COORDINADOR_PROGRAMAS).FirstOrDefault();//PROGRAMAS PRODUCTIVOS
                var _EstudioTr = new cEstudioCapacitTrabajoPenitenciarioComun().GetData(x => x.ID_INGRESO == SelectIngreso.ID_INGRESO && x.ID_IMPUTADO == SelectIngreso.ID_IMPUTADO && x.ID_CENTRO == SelectIngreso.ID_CENTRO && x.ID_ANIO == SelectIngreso.ID_ANIO && x.ID_ANIO == SelectIngreso.ID_ANIO && x.ID_ESTUDIO == Entity.ID_ESTUDIO).FirstOrDefault();
                if (_EstudioTr == null)
                {
                    var _NvoTrabajo = new PFC_VIII_TRABAJO()
                    {
                        ID_ANIO = SelectIngreso.ID_ANIO,
                        ID_CENTRO = SelectIngreso.ID_CENTRO,
                        ID_IMPUTADO = SelectIngreso.ID_IMPUTADO,
                        ID_INGRESO = SelectIngreso.ID_INGRESO,
                        ESTUDIO_FEC = FechaSeguridadCustodiaDictamen,
                        P1_TRABAJO_ANTES = ActividadOficioAntesReclusion,
                        P3_CALIDAD = IdCalidadTrabajo,
                        P3_PERSEVERANCIA = IdPerseverancia,
                        P3_RESPONSABILIDAD = IdResponsabilidad,
                        P4_FONDO_AHORRO = IdCuentaConFondoAhorro,
                        P5_DIAS_CENTRO_ACTUAL = DiasEfectivosLaboradosEnCentroActual,
                        P5_DIAS_LABORADOS = DiasEfectivosLaboradosTotalDiasLaborados,
                        P5_DIAS_OTROS_CENTROS = DiasEfectivosLaboradosEnOtrosCentros,
                        P5_PERIODO_LABORAL = PeriodoDondeRealizoActividadLaboral,
                        P6_DICTAMEN = IdDicatmenSeguridadCustodiaDict,
                        P7_MOTIVACION = MotivacionDictamenSeguridadCustodia,
                        ELABORO = NombreUsuario != null ? NombreUsuario.EMPLEADO != null ? NombreUsuario.EMPLEADO.PERSONA != null ? string.Format("{0} {1} {2}",
                            !string.IsNullOrEmpty(NombreUsuario.EMPLEADO.PERSONA.NOMBRE) ? NombreUsuario.EMPLEADO.PERSONA.NOMBRE.Trim() : string.Empty,
                            !string.IsNullOrEmpty(NombreUsuario.EMPLEADO.PERSONA.PATERNO) ? NombreUsuario.EMPLEADO.PERSONA.PATERNO.Trim() : string.Empty,
                            !string.IsNullOrEmpty(NombreUsuario.EMPLEADO.PERSONA.MATERNO) ? NombreUsuario.EMPLEADO.PERSONA.MATERNO.Trim() : string.Empty) : string.Empty : string.Empty : string.Empty,
                        COORDINADOR = _UsuarioCoordinador != null ? _UsuarioCoordinador.USUARIO != null ? _UsuarioCoordinador.USUARIO.EMPLEADO != null ? _UsuarioCoordinador.USUARIO.EMPLEADO.PERSONA != null ?
                            string.Format("{0} {1} {2}",
                                !string.IsNullOrEmpty(_UsuarioCoordinador.USUARIO.EMPLEADO.PERSONA.NOMBRE) ? _UsuarioCoordinador.USUARIO.EMPLEADO.PERSONA.NOMBRE.Trim() : string.Empty,
                                !string.IsNullOrEmpty(_UsuarioCoordinador.USUARIO.EMPLEADO.PERSONA.PATERNO) ? _UsuarioCoordinador.USUARIO.EMPLEADO.PERSONA.PATERNO.Trim() : string.Empty,
                                !string.IsNullOrEmpty(_UsuarioCoordinador.USUARIO.EMPLEADO.PERSONA.MATERNO) ? _UsuarioCoordinador.USUARIO.EMPLEADO.PERSONA.MATERNO.Trim() : string.Empty) : string.Empty : string.Empty : string.Empty : string.Empty
                    };

                    if (LstCapacitacionLaboral != null && LstCapacitacionLaboral.Any())
                        foreach (var item in LstCapacitacionLaboral)
                        {
                            var _NuevaCapacitacion = new PFC_VIII_ACTIVIDAD_LABORAL()
                            {
                                CONCLUYO = item.CONCLUYO,
                                ID_ANIO = SelectIngreso.ID_ANIO,
                                ID_CAPACITACION = item.ID_CAPACITACION,
                                ID_CENTRO = SelectIngreso.ID_CENTRO,
                                ID_IMPUTADO = SelectIngreso.ID_IMPUTADO,
                                ID_INGRESO = SelectIngreso.ID_INGRESO,
                                OBSERVACION = item.OBSERVACION,
                                PERIODO = item.PERIODO,
                                ID_CONSEC = item.ID_CONSEC,
                                ID_ESTUDIO = item.ID_ESTUDIO,
                                ID_ACTIVIDAD = item.ID_ACTIVIDAD,
                                ID_TIPO_PROGRAMA = item.ID_TIPO_PROGRAMA
                            };

                            _NvoTrabajo.PFC_VIII_ACTIVIDAD_LABORAL.Add(_NuevaCapacitacion);
                        };

                    if (LstActivNoGratificadas != null && LstActivNoGratificadas.Any())
                        foreach (var item in LstActivNoGratificadas)
                        {
                            var _NvaActividadNoGratificada = new PFC_VIII_ACTIVIDAD_LABORAL()
                            {
                                CONCLUYO = item.CONCLUYO,
                                ID_ANIO = SelectIngreso.ID_ANIO,
                                ID_CAPACITACION = item.ID_CAPACITACION,
                                ID_CENTRO = SelectIngreso.ID_CENTRO,
                                ID_IMPUTADO = SelectIngreso.ID_IMPUTADO,
                                ID_INGRESO = SelectIngreso.ID_INGRESO,
                                OBSERVACION = item.OBSERVACION,
                                PERIODO = item.PERIODO,
                                ID_CONSEC = item.ID_CONSEC,
                                ID_ESTUDIO = item.ID_ESTUDIO,
                                ID_ACTIVIDAD = item.ID_ACTIVIDAD,
                                ID_TIPO_PROGRAMA = item.ID_TIPO_PROGRAMA
                            };

                            _NvoTrabajo.PFC_VIII_ACTIVIDAD_LABORAL.Add(_NvaActividadNoGratificada);
                        };

                    if (LstActivGratificadas != null && LstActivGratificadas.Any())
                        foreach (var item in LstActivGratificadas)
                        {
                            var _NuevaActividadGratificada = new PFC_VIII_ACTIVIDAD_LABORAL()
                            {
                                CONCLUYO = item.CONCLUYO,
                                ID_ANIO = SelectIngreso.ID_ANIO,
                                ID_CAPACITACION = item.ID_CAPACITACION,
                                ID_CENTRO = SelectIngreso.ID_CENTRO,
                                ID_IMPUTADO = SelectIngreso.ID_IMPUTADO,
                                ID_INGRESO = SelectIngreso.ID_INGRESO,
                                OBSERVACION = item.OBSERVACION,
                                PERIODO = item.PERIODO,
                                ID_CONSEC = item.ID_CONSEC,
                                ID_ESTUDIO = item.ID_ESTUDIO,
                                ID_ACTIVIDAD = item.ID_ACTIVIDAD,
                                ID_TIPO_PROGRAMA = item.ID_TIPO_PROGRAMA
                            };

                            _NvoTrabajo.PFC_VIII_ACTIVIDAD_LABORAL.Add(_NuevaActividadGratificada);
                        };

                    return _NvoTrabajo;
                }
                else
                {
                    _EstudioTr.ID_ANIO = SelectIngreso.ID_ANIO;
                    _EstudioTr.ID_CENTRO = SelectIngreso.ID_CENTRO;
                    _EstudioTr.ID_IMPUTADO = SelectIngreso.ID_IMPUTADO;
                    _EstudioTr.ID_INGRESO = SelectIngreso.ID_INGRESO;
                    _EstudioTr.ESTUDIO_FEC = FechaSeguridadCustodiaDictamen;
                    _EstudioTr.P1_TRABAJO_ANTES = ActividadOficioAntesReclusion;
                    _EstudioTr.P3_CALIDAD = IdCalidadTrabajo;
                    _EstudioTr.P3_PERSEVERANCIA = IdPerseverancia;
                    _EstudioTr.P3_RESPONSABILIDAD = IdResponsabilidad;
                    _EstudioTr.P4_FONDO_AHORRO = IdCuentaConFondoAhorro;
                    _EstudioTr.P5_DIAS_CENTRO_ACTUAL = DiasEfectivosLaboradosEnCentroActual;
                    _EstudioTr.P5_DIAS_LABORADOS = DiasEfectivosLaboradosTotalDiasLaborados;
                    _EstudioTr.P5_DIAS_OTROS_CENTROS = DiasEfectivosLaboradosEnOtrosCentros;
                    _EstudioTr.P5_PERIODO_LABORAL = PeriodoDondeRealizoActividadLaboral;
                    _EstudioTr.P6_DICTAMEN = IdDicatmenSeguridadCustodiaDict;
                    _EstudioTr.P7_MOTIVACION = MotivacionDictamenSeguridadCustodia;
                    _EstudioTr.ELABORO = NombreUsuario != null ? NombreUsuario.EMPLEADO != null ? NombreUsuario.EMPLEADO.PERSONA != null ? string.Format("{0} {1} {2}",
                        !string.IsNullOrEmpty(NombreUsuario.EMPLEADO.PERSONA.NOMBRE) ? NombreUsuario.EMPLEADO.PERSONA.NOMBRE.Trim() : string.Empty,
                        !string.IsNullOrEmpty(NombreUsuario.EMPLEADO.PERSONA.PATERNO) ? NombreUsuario.EMPLEADO.PERSONA.PATERNO.Trim() : string.Empty,
                        !string.IsNullOrEmpty(NombreUsuario.EMPLEADO.PERSONA.MATERNO) ? NombreUsuario.EMPLEADO.PERSONA.MATERNO.Trim() : string.Empty) : string.Empty : string.Empty : string.Empty;
                    _EstudioTr.COORDINADOR = _UsuarioCoordinador != null ? _UsuarioCoordinador.USUARIO != null ? _UsuarioCoordinador.USUARIO.EMPLEADO != null ? _UsuarioCoordinador.USUARIO.EMPLEADO.PERSONA != null ?
                            string.Format("{0} {1} {2}",
                                !string.IsNullOrEmpty(_UsuarioCoordinador.USUARIO.EMPLEADO.PERSONA.NOMBRE) ? _UsuarioCoordinador.USUARIO.EMPLEADO.PERSONA.NOMBRE.Trim() : string.Empty,
                                !string.IsNullOrEmpty(_UsuarioCoordinador.USUARIO.EMPLEADO.PERSONA.PATERNO) ? _UsuarioCoordinador.USUARIO.EMPLEADO.PERSONA.PATERNO.Trim() : string.Empty,
                                !string.IsNullOrEmpty(_UsuarioCoordinador.USUARIO.EMPLEADO.PERSONA.MATERNO) ? _UsuarioCoordinador.USUARIO.EMPLEADO.PERSONA.MATERNO.Trim() : string.Empty) : string.Empty : string.Empty : string.Empty : string.Empty;
                    _EstudioTr.PFC_VIII_ACTIVIDAD_LABORAL.Clear();
                    if (LstCapacitacionLaboral != null)
                        if (LstCapacitacionLaboral.Any())
                            foreach (var item in LstCapacitacionLaboral)
                            {
                                var _NuevaCapacitacion = new PFC_VIII_ACTIVIDAD_LABORAL()
                                {
                                    CONCLUYO = item.CONCLUYO,
                                    ID_ANIO = SelectIngreso.ID_ANIO,
                                    ID_CAPACITACION = item.ID_CAPACITACION,
                                    ID_CENTRO = SelectIngreso.ID_CENTRO,
                                    ID_IMPUTADO = SelectIngreso.ID_IMPUTADO,
                                    ID_INGRESO = SelectIngreso.ID_INGRESO,
                                    OBSERVACION = item.OBSERVACION,
                                    ID_CONSEC = item.ID_CONSEC,
                                    ID_ESTUDIO = item.ID_ESTUDIO,
                                    PERIODO = item.PERIODO,
                                    ID_ACTIVIDAD = item.ID_ACTIVIDAD,
                                    ID_TIPO_PROGRAMA = item.ID_TIPO_PROGRAMA
                                };

                                _EstudioTr.PFC_VIII_ACTIVIDAD_LABORAL.Add(_NuevaCapacitacion);
                            };

                    if (LstActivNoGratificadas != null && LstActivNoGratificadas.Any())
                        foreach (var item in LstActivNoGratificadas)
                        {
                            var _NvaActividadNoGratificada = new PFC_VIII_ACTIVIDAD_LABORAL()
                            {
                                CONCLUYO = item.CONCLUYO,
                                ID_ANIO = SelectIngreso.ID_ANIO,
                                ID_CAPACITACION = item.ID_CAPACITACION,
                                ID_CENTRO = SelectIngreso.ID_CENTRO,
                                ID_IMPUTADO = SelectIngreso.ID_IMPUTADO,
                                ID_INGRESO = SelectIngreso.ID_INGRESO,
                                OBSERVACION = item.OBSERVACION,
                                PERIODO = item.PERIODO,
                                ID_CONSEC = item.ID_CONSEC,
                                ID_ESTUDIO = item.ID_ESTUDIO,
                                ID_ACTIVIDAD = item.ID_ACTIVIDAD,
                                ID_TIPO_PROGRAMA = item.ID_TIPO_PROGRAMA
                            };

                            _EstudioTr.PFC_VIII_ACTIVIDAD_LABORAL.Add(_NvaActividadNoGratificada);
                        };

                    if (LstActivGratificadas != null && LstActivGratificadas.Any())
                        foreach (var item in LstActivGratificadas)
                        {
                            var _NuevaActividadGratificada = new PFC_VIII_ACTIVIDAD_LABORAL()
                            {
                                CONCLUYO = item.CONCLUYO,
                                ID_ANIO = SelectIngreso.ID_ANIO,
                                ID_CAPACITACION = item.ID_CAPACITACION,
                                ID_CENTRO = SelectIngreso.ID_CENTRO,
                                ID_IMPUTADO = SelectIngreso.ID_IMPUTADO,
                                ID_INGRESO = SelectIngreso.ID_INGRESO,
                                OBSERVACION = item.OBSERVACION,
                                ID_CONSEC = item.ID_CONSEC,
                                ID_ESTUDIO = item.ID_ESTUDIO,
                                PERIODO = item.PERIODO,
                                ID_ACTIVIDAD = item.ID_ACTIVIDAD,
                                ID_TIPO_PROGRAMA = item.ID_TIPO_PROGRAMA
                            };

                            _EstudioTr.PFC_VIII_ACTIVIDAD_LABORAL.Add(_NuevaActividadGratificada);
                        };

                    return _EstudioTr;
                }
            }
            catch (Exception exc)
            {
                throw;
            }
        }
        private PFC_IX_SEGURIDAD EstudioSeguridadTransaccionIndividual(PERSONALIDAD Entity)
        {
            try
            {
                string _NombreSeg = string.Empty;
                string _NombreCoordSeg = string.Empty;
                var NombreUsuario = new cUsuario().Obtener(GlobalVar.gUsr);
                if (NombreUsuario != null)
                    _NombreSeg = NombreUsuario.EMPLEADO != null ? NombreUsuario.EMPLEADO.PERSONA != null ? string.Format("{0} {1} {2}",
                        !string.IsNullOrEmpty(NombreUsuario.EMPLEADO.PERSONA.NOMBRE) ? NombreUsuario.EMPLEADO.PERSONA.NOMBRE.Trim() : string.Empty,
                        !string.IsNullOrEmpty(NombreUsuario.EMPLEADO.PERSONA.PATERNO) ? NombreUsuario.EMPLEADO.PERSONA.PATERNO.Trim() : string.Empty,
                        !string.IsNullOrEmpty(NombreUsuario.EMPLEADO.PERSONA.MATERNO) ? NombreUsuario.EMPLEADO.PERSONA.MATERNO.Trim() : string.Empty) : string.Empty : string.Empty;

                var _UsuarioCoordinador = new cUsuarioRol().GetData(x => x.ID_ROL == (short)eRolesCoordinadores.COORDINADOR_COMANDANCIA).FirstOrDefault();
                if (_UsuarioCoordinador != null)
                    _NombreCoordSeg = _UsuarioCoordinador.USUARIO != null ? _UsuarioCoordinador.USUARIO.EMPLEADO != null ? _UsuarioCoordinador.USUARIO.EMPLEADO.PERSONA != null ?
                            string.Format("{0} {1} {2}",
                                !string.IsNullOrEmpty(_UsuarioCoordinador.USUARIO.EMPLEADO.PERSONA.NOMBRE) ? _UsuarioCoordinador.USUARIO.EMPLEADO.PERSONA.NOMBRE.Trim() : string.Empty,
                                !string.IsNullOrEmpty(_UsuarioCoordinador.USUARIO.EMPLEADO.PERSONA.PATERNO) ? _UsuarioCoordinador.USUARIO.EMPLEADO.PERSONA.PATERNO.Trim() : string.Empty,
                                !string.IsNullOrEmpty(_UsuarioCoordinador.USUARIO.EMPLEADO.PERSONA.MATERNO) ? _UsuarioCoordinador.USUARIO.EMPLEADO.PERSONA.MATERNO.Trim() : string.Empty) : string.Empty : string.Empty : string.Empty;

                var _EstudioCusto = new cInformeAreaSeguridadCustodiaComun().GetData(x => x.ID_INGRESO == SelectIngreso.ID_INGRESO && x.ID_IMPUTADO == SelectIngreso.ID_IMPUTADO && x.ID_CENTRO == SelectIngreso.ID_CENTRO && x.ID_ANIO == SelectIngreso.ID_ANIO && x.ID_ANIO == SelectIngreso.ID_ANIO && x.ID_ESTUDIO == Entity.ID_ESTUDIO).FirstOrDefault();
                if (_EstudioCusto == null)
                {
                    var _EstudioSeg = new PFC_IX_SEGURIDAD()
                    {
                        ID_ANIO = SelectIngreso.ID_ANIO,
                        ID_CENTRO = SelectIngreso.ID_CENTRO,
                        ID_IMPUTADO = SelectIngreso.ID_IMPUTADO,
                        ID_INGRESO = SelectIngreso.ID_INGRESO,
                        ELABORO = _NombreSeg,
                        COMANDANTE = _NombreCoordSeg,
                        ESTUDIO_FEC = FechaDictamenInformeSeguridadCustodia,
                        P1_CONDUCTA_CENTRO = IdConductaObservadaCentro,
                        P2_CONDUCTA_AUTORIDAD = IdConductaAutoridad,
                        P3_CONDUCTA_GENERAL = IdConductaGral,
                        P4_RELACION_COMPANEROS = IdRelacionCompanieros,
                        P5_CORRECTIVOS = IdRegistraCorrectivosDisciplinarios,
                        P6_OPINION_CONDUCTA = IdOpinionGralConductaInterno,
                        P7_DICTAMEN = IdDictamenInformeSeguridadCustodia,
                        P8_MOTIVACION = MotivacionDictamenInformeSeguridadCustodia
                    };

                    _EstudioSeg.PFC_IX_CORRECTIVO.Clear();

                    var ListaCorre = new List<PFC_IX_CORRECTIVO>();
                    if (LstCorrectivosSeguridad != null && LstCorrectivosSeguridad.Any())
                        foreach (var item in LstCorrectivosSeguridad)
                            ListaCorre.Add(item);

                    if (ListaCorre.Any() && ListaCorre != null)
                        foreach (var item in ListaCorre)
                            _EstudioSeg.PFC_IX_CORRECTIVO.Add(item);

                    return _EstudioSeg;
                }

                else
                {
                    _EstudioCusto.ID_ANIO = SelectIngreso.ID_ANIO;
                    _EstudioCusto.ID_CENTRO = SelectIngreso.ID_CENTRO;
                    _EstudioCusto.ID_IMPUTADO = SelectIngreso.ID_IMPUTADO;
                    _EstudioCusto.ID_INGRESO = SelectIngreso.ID_INGRESO;
                    _EstudioCusto.ELABORO = _NombreSeg;
                    _EstudioCusto.COMANDANTE = _NombreCoordSeg;
                    _EstudioCusto.ESTUDIO_FEC = FechaDictamenInformeSeguridadCustodia;
                    _EstudioCusto.P1_CONDUCTA_CENTRO = IdConductaObservadaCentro;
                    _EstudioCusto.P2_CONDUCTA_AUTORIDAD = IdConductaAutoridad;
                    _EstudioCusto.P3_CONDUCTA_GENERAL = IdConductaGral;
                    _EstudioCusto.P4_RELACION_COMPANEROS = IdRelacionCompanieros;
                    _EstudioCusto.P5_CORRECTIVOS = IdRegistraCorrectivosDisciplinarios;
                    _EstudioCusto.P6_OPINION_CONDUCTA = IdOpinionGralConductaInterno;
                    _EstudioCusto.P7_DICTAMEN = IdDictamenInformeSeguridadCustodia;
                    _EstudioCusto.P8_MOTIVACION = MotivacionDictamenInformeSeguridadCustodia;
                    _EstudioCusto.PFC_IX_CORRECTIVO.Clear();

                    if (LstCorrectivosSeguridad != null && LstCorrectivosSeguridad.Any())
                        foreach (var item in LstCorrectivosSeguridad)
                            _EstudioCusto.PFC_IX_CORRECTIVO.Add(item);

                    return _EstudioCusto;
                }
            }
            catch (Exception exc)
            {
                throw;
            }
        }
        private PERSONALIDAD_FUERO_COMUN EstudioPersonalidadPadre()
        {
            try
            {
                if (SelectIngreso != null)
                    return new cEstudioPersonalidadFueroComun().GetData(x => x.ID_INGRESO == SelectIngreso.ID_INGRESO && x.ID_IMPUTADO == SelectIngreso.ID_IMPUTADO).FirstOrDefault();
                else
                    return null;
            }
            catch (Exception exc)
            {
                throw;
            }
        }
        #endregion


        #region Llenado de datos de reportes de fuero comun
        private Microsoft.Reporting.WinForms.ReportDataSource DatosCapacitacionLaboral(PERSONALIDAD Entity)
        {
            try
            {
                var _Datos = new List<cCapacitacionLaboral>();
                var _DatosCapacitacion = new cActrividadesEstudioCapacitacionTrabajoFueroComun().GetData(x => x.ID_INGRESO == Entity.ID_INGRESO && x.ID_IMPUTADO == Entity.ID_IMPUTADO && x.ID_CENTRO == Entity.ID_CENTRO && x.ID_ESTUDIO == Entity.ID_ESTUDIO && x.ID_ANIO == Entity.ID_ANIO && x.ID_TIPO_PROGRAMA == 147);
                Microsoft.Reporting.WinForms.ReportDataSource _respuesta = new Microsoft.Reporting.WinForms.ReportDataSource();
                cCapacitacionLaboral CamposBase = new cCapacitacionLaboral();
                if (_DatosCapacitacion != null)
                    if (_DatosCapacitacion.Any())
                        foreach (var item in _DatosCapacitacion)
                        {
                            CamposBase = new cCapacitacionLaboral()
                            {
                                Concluyo = item.CONCLUYO == "S" ? "SI" : "NO",
                                Observaciones = item.OBSERVACION,
                                Periodo = item.PERIODO,
                                Taller = item.ACTIVIDAD != null ? !string.IsNullOrEmpty(item.ACTIVIDAD.DESCR) ? item.ACTIVIDAD.DESCR.Trim() : string.Empty : string.Empty
                            };

                            _Datos.Add(CamposBase);
                        };

                _respuesta.Value = _Datos;
                _respuesta.Name = "DataSet3";
                return _respuesta;

            }
            catch (Exception exc)
            {
                throw exc;
            }
        }

        private Microsoft.Reporting.WinForms.ReportDataSource DatosActivNoGratificadas(PERSONALIDAD Entity)
        {
            try
            {
                var _Datos = new List<cCapacitacionLaboral>();
                var _DatosCapacitacion = new cActrividadesEstudioCapacitacionTrabajoFueroComun().GetData(x => x.ID_INGRESO == Entity.ID_INGRESO && x.ID_IMPUTADO == Entity.ID_IMPUTADO && x.ID_CENTRO == Entity.ID_CENTRO && x.ID_ESTUDIO == Entity.ID_ESTUDIO && x.ID_ANIO == Entity.ID_ANIO && x.ID_TIPO_PROGRAMA == 148);
                Microsoft.Reporting.WinForms.ReportDataSource _respuesta = new Microsoft.Reporting.WinForms.ReportDataSource();
                cCapacitacionLaboral CamposBase = new cCapacitacionLaboral();
                if (_DatosCapacitacion != null)
                    if (_DatosCapacitacion.Any())
                        foreach (var item in _DatosCapacitacion)
                        {
                            CamposBase = new cCapacitacionLaboral()
                            {
                                Concluyo = item.CONCLUYO,
                                Observaciones = item.OBSERVACION,
                                Periodo = item.PERIODO,
                                Taller = item.ACTIVIDAD != null ? !string.IsNullOrEmpty(item.ACTIVIDAD.DESCR) ? item.ACTIVIDAD.DESCR.Trim() : string.Empty : string.Empty
                            };

                            _Datos.Add(CamposBase);
                        };

                _respuesta.Value = _Datos;
                _respuesta.Name = "DataSet4";
                return _respuesta;

            }
            catch (Exception exc)
            {
                throw exc;
            }
        }

        private Microsoft.Reporting.WinForms.ReportDataSource DatosActivGratificadas(PERSONALIDAD Entity)
        {
            try
            {
                var _Datos = new List<cCapacitacionLaboral>();
                var _DatosCapacitacion = new cActrividadesEstudioCapacitacionTrabajoFueroComun().GetData(x => x.ID_INGRESO == Entity.ID_INGRESO && x.ID_IMPUTADO == Entity.ID_IMPUTADO && x.ID_CENTRO == Entity.ID_CENTRO && x.ID_ESTUDIO == Entity.ID_ESTUDIO && x.ID_ANIO == Entity.ID_ANIO && x.ID_TIPO_PROGRAMA == 149);
                Microsoft.Reporting.WinForms.ReportDataSource _respuesta = new Microsoft.Reporting.WinForms.ReportDataSource();
                cCapacitacionLaboral CamposBase = new cCapacitacionLaboral();
                if (_DatosCapacitacion != null)
                    if (_DatosCapacitacion.Any())
                        foreach (var item in _DatosCapacitacion)
                        {
                            CamposBase = new cCapacitacionLaboral()
                            {
                                Concluyo = item.CONCLUYO,
                                Observaciones = item.OBSERVACION,
                                Periodo = item.PERIODO,
                                Taller = item.ACTIVIDAD != null ? !string.IsNullOrEmpty(item.ACTIVIDAD.DESCR) ? item.ACTIVIDAD.DESCR.Trim() : string.Empty : string.Empty
                            };

                            _Datos.Add(CamposBase);
                        };

                _respuesta.Value = _Datos;
                _respuesta.Name = "DataSet5";
                return _respuesta;

            }
            catch (Exception exc)
            {
                throw exc;
            }
        }


        private Microsoft.Reporting.WinForms.ReportDataSource DatosProgUno(PERSONALIDAD Entity)
        {
            try
            {
                var _Datos = new List<cProgramasFueroComun>();
                var _DatosCapacitacion = new cProgramaRealizacionEstudios().GetData(x => x.ID_INGRESO == Entity.ID_INGRESO && x.ID_IMPUTADO == Entity.ID_IMPUTADO && x.ID_CENTRO == Entity.ID_CENTRO && x.ID_ESTUDIO == Entity.ID_ESTUDIO && x.ID_ANIO == Entity.ID_ANIO);
                Microsoft.Reporting.WinForms.ReportDataSource _respuesta = new Microsoft.Reporting.WinForms.ReportDataSource();
                cProgramasFueroComun CamposBase = new cProgramasFueroComun();
                if (_DatosCapacitacion != null && _DatosCapacitacion.Any())
                {
                    short _con = 1;
                    foreach (var item in _DatosCapacitacion)
                    {
                        if (item.ID_TIPO_PROGRAMA == (short)eGrupos.PROGRAMAS_DESHABITUAMIENTO)
                        {
                            CamposBase = new cProgramasFueroComun();
                            CamposBase.Duraci = item.DURACION;
                            CamposBase.Observaciones = item.OBSERVACION;
                            CamposBase._Consecutivo = _con;
                            CamposBase.Programa = item.ACTIVIDAD != null ? !string.IsNullOrEmpty(item.ACTIVIDAD.DESCR) ? item.ACTIVIDAD.DESCR.Trim() : string.Empty : string.Empty;
                            _Datos.Add(CamposBase);
                            _con++;
                        };
                    };
                }

                _respuesta.Value = _Datos;
                _respuesta.Name = "DataSet3";
                return _respuesta;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private Microsoft.Reporting.WinForms.ReportDataSource DatosProgDos(PERSONALIDAD Entity)
        {
            try
            {
                var _Datos = new List<cProgramasFueroComun>();
                var _DatosCapacitacion = new cProgramaRealizacionEstudios().GetData(x => x.ID_INGRESO == Entity.ID_INGRESO && x.ID_IMPUTADO == Entity.ID_IMPUTADO && x.ID_CENTRO == Entity.ID_CENTRO && x.ID_ESTUDIO == Entity.ID_ESTUDIO && x.ID_ANIO == Entity.ID_ANIO);
                Microsoft.Reporting.WinForms.ReportDataSource _respuesta = new Microsoft.Reporting.WinForms.ReportDataSource();
                cProgramasFueroComun CamposBase = new cProgramasFueroComun();
                if (_DatosCapacitacion != null && _DatosCapacitacion.Any())
                {
                    short _con = 1;
                    foreach (var item in _DatosCapacitacion)
                    {
                        if (item.ID_TIPO_PROGRAMA == (short)eGrupos.PROGRAMAS_MODIFIC_CONDUCTA)
                        {
                            CamposBase = new cProgramasFueroComun();
                            CamposBase.Duraci = item.CONCLUYO == "S" ? "SI" : "NO";
                            CamposBase.Observaciones = item.OBSERVACION;
                            CamposBase._Consecutivo = _con;
                            CamposBase.Programa = item.ACTIVIDAD != null ? !string.IsNullOrEmpty(item.ACTIVIDAD.DESCR) ? item.ACTIVIDAD.DESCR.Trim() : string.Empty : string.Empty;
                            _Datos.Add(CamposBase);
                            _con++;
                        };
                    };
                }

                _respuesta.Value = _Datos;
                _respuesta.Name = "DataSet4";
                return _respuesta;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private Microsoft.Reporting.WinForms.ReportDataSource DatosProgTres(PERSONALIDAD Entity)
        {
            try
            {

                var _Datos = new List<cProgramasFueroComun>();
                var _DatosCapacitacion = new cProgramaRealizacionEstudios().GetData(x => x.ID_INGRESO == Entity.ID_INGRESO && x.ID_IMPUTADO == Entity.ID_IMPUTADO && x.ID_CENTRO == Entity.ID_CENTRO && x.ID_ESTUDIO == Entity.ID_ESTUDIO && x.ID_ANIO == Entity.ID_ANIO);
                Microsoft.Reporting.WinForms.ReportDataSource _respuesta = new Microsoft.Reporting.WinForms.ReportDataSource();
                cProgramasFueroComun CamposBase = new cProgramasFueroComun();
                if (_DatosCapacitacion != null && _DatosCapacitacion.Any())
                {
                    short _con = 1;
                    foreach (var item in _DatosCapacitacion)
                    {
                        if (item.ID_TIPO_PROGRAMA == (short)eGrupos.COMPLEMENTARIO)
                        {
                            CamposBase = new cProgramasFueroComun();
                            CamposBase.Duraci = item.CONCLUYO == "S" ? "SI" : "NO";
                            CamposBase.Observaciones = item.OBSERVACION;
                            CamposBase._Consecutivo = _con;
                            CamposBase.Programa = item.ACTIVIDAD != null ? !string.IsNullOrEmpty(item.ACTIVIDAD.DESCR) ? item.ACTIVIDAD.DESCR.Trim() : string.Empty : string.Empty;
                            _Datos.Add(CamposBase);
                            _con++;
                        };
                    };
                }

                _respuesta.Value = _Datos;
                _respuesta.Name = "DataSet5";
                return _respuesta;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private Microsoft.Reporting.WinForms.ReportDataSource DatosProgCuatro(PERSONALIDAD Entity)
        {
            try
            {
                var _Datos = new List<cProgramasFueroComun>();
                var _DatosCapacitacion = new cProgramaRealizacionEstudios().GetData(x => x.ID_INGRESO == Entity.ID_INGRESO && x.ID_IMPUTADO == Entity.ID_IMPUTADO && x.ID_CENTRO == Entity.ID_CENTRO && x.ID_ESTUDIO == Entity.ID_ESTUDIO && x.ID_ANIO == Entity.ID_ANIO);
                Microsoft.Reporting.WinForms.ReportDataSource _respuesta = new Microsoft.Reporting.WinForms.ReportDataSource();
                cProgramasFueroComun CamposBase = new cProgramasFueroComun();
                if (_DatosCapacitacion != null && _DatosCapacitacion.Any())
                {
                    short _con = 1;
                    foreach (var item in _DatosCapacitacion)
                    {
                        if (item.ID_TIPO_PROGRAMA != (short)eGrupos.COMPLEMENTARIO && item.ID_TIPO_PROGRAMA != (short)eGrupos.PROGRAMAS_DESHABITUAMIENTO && item.ID_TIPO_PROGRAMA != (short)eGrupos.PROGRAMAS_MODIFIC_CONDUCTA)
                        {
                            CamposBase = new cProgramasFueroComun();
                            CamposBase.Duraci = item.CONCLUYO == "S" ? "SI" : "NO";
                            CamposBase.Observaciones = item.OBSERVACION;
                            CamposBase._Consecutivo = _con;
                            CamposBase.Programa = item.ACTIVIDAD != null ? !string.IsNullOrEmpty(item.ACTIVIDAD.DESCR) ? item.ACTIVIDAD.DESCR.Trim() : string.Empty : string.Empty;
                            _Datos.Add(CamposBase);
                            _con++;
                        };
                    };
                }

                _respuesta.Value = _Datos;
                _respuesta.Name = "DataSet6";
                return _respuesta;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private Microsoft.Reporting.WinForms.ReportDataSource DatosEstudioMedicoFueroComun(PERSONALIDAD Entity)
        {
            try
            {
                var _Datos = new List<cRealizacionEstudios>();
                var _DatosEstudioMedicoFrueroComun = new cPersonalidadEstudioMedicoComun().GetData(x => x.ID_INGRESO == Entity.ID_INGRESO && x.ID_IMPUTADO == Entity.ID_IMPUTADO && x.ID_CENTRO == Entity.ID_CENTRO && x.ID_ESTUDIO == Entity.ID_ESTUDIO && x.ID_ANIO == Entity.ID_ANIO).FirstOrDefault();
                Microsoft.Reporting.WinForms.ReportDataSource _respuesta = new Microsoft.Reporting.WinForms.ReportDataSource();
                cRealizacionEstudios CamposBase = new cRealizacionEstudios();
                CamposBase.EdadInterno = SelectIngreso.IMPUTADO != null ? new Fechas().CalculaEdad(SelectIngreso.IMPUTADO.NACIMIENTO_FECHA).ToString() : string.Empty;
                CamposBase.SexoInterno = SelectIngreso.IMPUTADO != null ? SelectIngreso.IMPUTADO.SEXO == "M" ? "MASCULINO" : "FEMENINO" : string.Empty;
                if (_DatosEstudioMedicoFrueroComun != null)
                {
                    CamposBase.AntecedentesHeredoFamiliares = string.Format("{0}", _DatosEstudioMedicoFrueroComun.P2_HEREDO_FAMILIARES);
                    CamposBase.AntecedentesPersonalesNoPatologicos = _DatosEstudioMedicoFrueroComun.P3_ANTPER_NOPATO;
                    CamposBase.AntecedentesConsumoToxicosEstadoActual = _DatosEstudioMedicoFrueroComun.P31_CONSUMO_TOXICO;
                    CamposBase.DescrTatuajesCicatricesRecAntiguasMalformaciones = _DatosEstudioMedicoFrueroComun.P32_TATUAJES_CICATRICES;
                    CamposBase.AntecedentesPatologicos = _DatosEstudioMedicoFrueroComun.P4_PATOLOGICOS;
                    CamposBase.PadecimientoActual = _DatosEstudioMedicoFrueroComun.P5_PADECIMIENTOS;
                    CamposBase.TensionArterial = _DatosEstudioMedicoFrueroComun.SIGNOS_TA;
                    CamposBase.Teperatura = _DatosEstudioMedicoFrueroComun.SIGNOS_TEMPERATURA;
                    CamposBase.Pulso = _DatosEstudioMedicoFrueroComun.SIGNOS_PULSO;
                    CamposBase.Respiracion = _DatosEstudioMedicoFrueroComun.SIGNOS_RESPIRACION;
                    CamposBase.Peso = _DatosEstudioMedicoFrueroComun.SIGNOS_PESO;
                    CamposBase.Estatura = _DatosEstudioMedicoFrueroComun.SIGNOS_ESTATURA;
                    CamposBase.Abdomen = _DatosEstudioMedicoFrueroComun.COORDINADOR;
                    CamposBase.Actitud = _DatosEstudioMedicoFrueroComun.ELABORO;
                    CamposBase.ImpresionDiagnostica = _DatosEstudioMedicoFrueroComun.P7_IMPRESION_DIAGNOSTICA;
                    CamposBase.Dictamen = string.Format(" ( {0} ) FAVORABLE \n ( {1} ) DESFAVORABLE", _DatosEstudioMedicoFrueroComun.P8_DICTAMEN_MEDICO == (short)eDiagnosticoDictamen.FAVORABLE ? "X" : string.Empty,
                        _DatosEstudioMedicoFrueroComun.P8_DICTAMEN_MEDICO == (short)eDiagnosticoDictamen.DESFAVORABLE ? "X" : string.Empty);//estructura provisional del dictamen
                    CamposBase.FechaRealizacionEstudio = _DatosEstudioMedicoFrueroComun.ESTUDIO_FEC.HasValue ? _DatosEstudioMedicoFrueroComun.ESTUDIO_FEC.Value.ToString("dd/MM/yyyy") : string.Empty;
                }

                _Datos.Add(CamposBase);
                _respuesta.Value = _Datos;
                _respuesta.Name = "DataSet2";
                return _respuesta;
            }

            catch (Exception exc)
            {
                throw exc;
            }
        }

        private Microsoft.Reporting.WinForms.ReportDataSource DatosEstudioPsiquiatricoFueroComun(PERSONALIDAD Entity)
        {
            try
            {
                var _Datos = new List<cRealizacionEstudios>();
                var _DatosEstudioPsiquatricoComun = new cPersonalidadEstudioPsiquiatricoComun().GetData(x => x.ID_INGRESO == Entity.ID_INGRESO && x.ID_IMPUTADO == Entity.ID_IMPUTADO && x.ID_CENTRO == Entity.ID_CENTRO && x.ID_ESTUDIO == Entity.ID_ESTUDIO && x.ID_ANIO == Entity.ID_ANIO).FirstOrDefault();
                Microsoft.Reporting.WinForms.ReportDataSource _respuesta = new Microsoft.Reporting.WinForms.ReportDataSource();
                cRealizacionEstudios CamposBase = new cRealizacionEstudios();
                if (_DatosEstudioPsiquatricoComun != null)
                {
                    CamposBase.AspectoFisico = _DatosEstudioPsiquatricoComun.A1_ASPECTO_FISICO;
                    CamposBase.ConductaMotora = _DatosEstudioPsiquatricoComun.B1_CONDUCTA_MOTORA;
                    CamposBase.Habla = _DatosEstudioPsiquatricoComun.C1_HABLA;
                    CamposBase.Actitud = _DatosEstudioPsiquatricoComun.D1_ACTITUD;
                    CamposBase.EstadoAnimo = _DatosEstudioPsiquatricoComun.A2_ESTADO_ANIMO;
                    CamposBase.ExpresAfectiva = _DatosEstudioPsiquatricoComun.B2_EXPRESION_AFECTIVA;
                    CamposBase.Adecuacion = _DatosEstudioPsiquatricoComun.C2_ADECUACION;
                    CamposBase.Alucinaciones = _DatosEstudioPsiquatricoComun.A3_ALUCINACIONES;
                    CamposBase.Ilusiones = _DatosEstudioPsiquatricoComun.B3_ILUSIONES;
                    CamposBase.Despersonalizacion = _DatosEstudioPsiquatricoComun.C3_DESPERSONALIZACION;
                    CamposBase.Desrealizacion = _DatosEstudioPsiquatricoComun.D3_DESREALIZACION;
                    CamposBase.CursoPensamiento = _DatosEstudioPsiquatricoComun.A4_CURSO;
                    CamposBase.ContinuidadPensamiento = _DatosEstudioPsiquatricoComun.B4_CONTINUIDAD;
                    CamposBase.ContenidoPensamiento = _DatosEstudioPsiquatricoComun.C4_CONTENIDO;
                    CamposBase.PensamientoAbstracto = _DatosEstudioPsiquatricoComun.D4_ABASTRACTO;
                    CamposBase.Concentracion = _DatosEstudioPsiquatricoComun.E4_CONCENTRACION;
                    CamposBase.Orientacion = _DatosEstudioPsiquatricoComun.P5_ORIENTACION;
                    CamposBase.Memoria = _DatosEstudioPsiquatricoComun.P6_MEMORIA;
                    CamposBase.BajaToleranciaFrust = _DatosEstudioPsiquatricoComun.A7_BAJA_TOLERANCIA;
                    CamposBase.ExpresionDesadapt = _DatosEstudioPsiquatricoComun.B7_EXPRESION;
                    CamposBase.Adecuada = _DatosEstudioPsiquatricoComun.C7_ADECUADA;
                    CamposBase.CapacJuicio = _DatosEstudioPsiquatricoComun.P8_CAPACIDAD_JUICIO;
                    CamposBase.Introspeccion = _DatosEstudioPsiquatricoComun.P9_INTROSPECCION;
                    CamposBase.Fiabilidad = _DatosEstudioPsiquatricoComun.P10_FIANILIDAD;
                    CamposBase.ImpresionDiagnostica = _DatosEstudioPsiquatricoComun.P11_IMPRESION;
                    CamposBase.Abdomen = _DatosEstudioPsiquatricoComun.COORDINADOR;
                    CamposBase.AliasInterno = _DatosEstudioPsiquatricoComun.MEDICO_PSIQUIATRA;
                    CamposBase.FechaRealizacionEstudio = _DatosEstudioPsiquatricoComun.ESTUDIO_FEC.HasValue ? _DatosEstudioPsiquatricoComun.ESTUDIO_FEC.Value.ToString("dd/MM/yyyy") : string.Empty;
                    CamposBase.Dictamen = string.Format("FAVORABLE ( {0} ) \t DESFAVORABLE ( {1} )", _DatosEstudioPsiquatricoComun.P12_DICTAMEN_PSIQUIATRICO == (short)eDiagnosticoDictamen.FAVORABLE ? "X" : string.Empty,
                        _DatosEstudioPsiquatricoComun.P12_DICTAMEN_PSIQUIATRICO == (short)eDiagnosticoDictamen.DESFAVORABLE ? "X" : string.Empty);
                }

                _Datos.Add(CamposBase);
                _respuesta.Value = _Datos;
                _respuesta.Name = "DataSet2";
                return _respuesta;
            }

            catch (Exception exc)
            {
                throw exc;
            }
        }

        private Microsoft.Reporting.WinForms.ReportDataSource DatosEstudioPsicologicoFueroComun(PERSONALIDAD Entity)
        {
            try
            {
                var _Datos = new List<cRealizacionEstudios>();
                var _DatosPsicologicoComun = new cPersonalidadEstudioPsicologicoComun().GetData(x => x.ID_INGRESO == Entity.ID_INGRESO && x.ID_IMPUTADO == Entity.ID_IMPUTADO && x.ID_CENTRO == Entity.ID_CENTRO && x.ID_ESTUDIO == Entity.ID_ESTUDIO && x.ID_ANIO == Entity.ID_ANIO).FirstOrDefault();
                Microsoft.Reporting.WinForms.ReportDataSource _respuesta = new Microsoft.Reporting.WinForms.ReportDataSource();
                cRealizacionEstudios CamposBase = new cRealizacionEstudios();
                if (_DatosPsicologicoComun != null)
                {
                    CamposBase.CondicionesGralesInterno = _DatosPsicologicoComun.P1_CONDICIONES_GRALES;
                    CamposBase.ExamenMental = _DatosPsicologicoComun.P2_EXAMEN_MENTAL;
                    CamposBase.DescripcionPrincRasgosIngresoRelComDelito = _DatosPsicologicoComun.P3_PRINCIPALES_RASGOS;
                    CamposBase.LauretaBenderTexto = string.Format("( {0} ) TEST GUESTALTICO VISOMOTOR DE LAURETTA BENDER", _DatosPsicologicoComun.P4_TEST_GUALTICO == (short)eSINO.SI ? "X" : string.Empty);
                    CamposBase.MatricesRavenTexto = string.Format("( {0} ) TEST DE MATRICES PROGRESIVAS DE RAVEN", _DatosPsicologicoComun.P4_TEST_MATRICES == (short)eSINO.SI ? "X" : string.Empty);
                    CamposBase.HTPTexto = string.Format("( {0} ) TEST (HTP) CASA, ARBOL, PERSONA", _DatosPsicologicoComun.P4_TEST_HTP == (short)eSINO.SI ? "X" : string.Empty);
                    CamposBase.MinnessotaTexto = string.Format("( {0} ) INVENTARIO MULTIFÁSICO DE LA PERSONALIDAD MINESOTA (MMPI 1 o 2).", _DatosPsicologicoComun.P4_INVENTARIO_MULTIFASICO == (short)eSINO.SI ? "X" : string.Empty);
                    CamposBase.OtroTestTexto = string.Format("( {0} ) OTRA (S) {1}", _DatosPsicologicoComun.P4_OTRAS == (short)eSINO.SI ? "X" : string.Empty, _DatosPsicologicoComun.P4_OTRA_MENCIONAR);
                    CamposBase.NivelIntelectualTextro = string.Format(
                        " ( {0} ) SUPERIOR \n ( {1} ) SUPERIOR AL TÉRMINO MEDIO \n ( {2} ) MEDIO \n ( {3} ) INFERIOR AL TÉRMINO MEDIO \n ( {4} ) INFERIOR \n ( {5} ) DEFICIENTE",
                        _DatosPsicologicoComun.P51_NIVEL_INTELECTUAL == (short)eNivelIntelectual.SUPERIOR ? "X" : string.Empty, _DatosPsicologicoComun.P51_NIVEL_INTELECTUAL == (short)eNivelIntelectual.SUPERIOR_TERMINO_MEDIO ? "X" : string.Empty,
                        _DatosPsicologicoComun.P51_NIVEL_INTELECTUAL == (short)eNivelIntelectual.MEDIO ? "X" : string.Empty, _DatosPsicologicoComun.P51_NIVEL_INTELECTUAL == (short)eNivelIntelectual.INFERIOR_TERMINO_MEDIO ? "X" : string.Empty,
                        _DatosPsicologicoComun.P51_NIVEL_INTELECTUAL == (short)eNivelIntelectual.INFERIOR ? "X" : string.Empty, _DatosPsicologicoComun.P51_NIVEL_INTELECTUAL == (short)eNivelIntelectual.DEFICIENTE ? "X" : string.Empty);
                    CamposBase.DatosDisfuncionNeuroTexto = string.Format(" ( {0} ) NO PRESENTA \n ( {1} ) SE SOSPECHA \n ( {2} ) CON DATOS CLÍNICOS EVIDENTES",
                        _DatosPsicologicoComun.P52_DISFUNCION_NEUROLOGICA == (short)eDisfuncionNeurologica.NO_PRESENTA ? "X" : string.Empty, _DatosPsicologicoComun.P52_DISFUNCION_NEUROLOGICA == (short)eDisfuncionNeurologica.SE_SOSPECHA ? "X" : string.Empty
                        , _DatosPsicologicoComun.P52_DISFUNCION_NEUROLOGICA == (short)eDisfuncionNeurologica.DATOS_CLINICOS_EVIDENTES ? "X" : string.Empty);
                    CamposBase.IntegracionDinamica = _DatosPsicologicoComun.P6_INTEGRACION;
                    CamposBase.RasgosPersonalidadRelComisionDelitoLogradoModificarInternamiento = _DatosPsicologicoComun.P8_RASGOS_PERSONALIDAD;
                    CamposBase.Dictamen = string.Format(" ( {0} ) FAVORABLE \n ( {1} ) DESFAVORABLE ", _DatosPsicologicoComun.P9_DICTAMEN_REINSERCION == (short)eDiagnosticoDictamen.FAVORABLE ? "X" : string.Empty, _DatosPsicologicoComun.P9_DICTAMEN_REINSERCION == (short)eDiagnosticoDictamen.DESFAVORABLE ? "X" : string.Empty);
                    CamposBase.MotivacionDictamen = _DatosPsicologicoComun.P10_MOTIVACION_DICTAMEN;
                    CamposBase.CasoNegativoSenialeProgramasARemitir = _DatosPsicologicoComun.P11_CASO_NEGATIVO;
                    CamposBase.RequiereTratExtraMurosTexto = string.Format("SI ( {0} ) NO ( {1} )", _DatosPsicologicoComun.P12_REQUIERE_TRATAMIENTO == "S" ? "X" : string.Empty, _DatosPsicologicoComun.P12_REQUIERE_TRATAMIENTO == "N" ? "X" : string.Empty);
                    CamposBase.CualExtramuros = _DatosPsicologicoComun.P12_CUAL;
                    CamposBase.FechaRealizacionEstudio = _DatosPsicologicoComun.ESTUDIO_FEC.HasValue ? _DatosPsicologicoComun.ESTUDIO_FEC.Value.ToString("dd/MM/yyyy") : string.Empty;
                    CamposBase.Abdomen = _DatosPsicologicoComun.COORDINADOR;
                    CamposBase.Actitud = _DatosPsicologicoComun.ELABORO;
                }

                _Datos.Add(CamposBase);
                _respuesta.Value = _Datos;
                _respuesta.Name = "DataSet2";
                return _respuesta;
            }
            catch (Exception exc)
            {
                throw exc;
            }
        }

        private Microsoft.Reporting.WinForms.ReportDataSource DatosEstudioCriminodiagnosticoFueroComun(PERSONALIDAD Entity)
        {
            try
            {
                var _Datos = new List<cRealizacionEstudios>();
                var _DatosCriminodiagnosticoComun = new cPersonalidadEstudioCriminodiagnosticoComun().GetData(x => x.ID_INGRESO == Entity.ID_INGRESO && x.ID_IMPUTADO == Entity.ID_IMPUTADO && x.ID_CENTRO == Entity.ID_CENTRO && x.ID_ESTUDIO == Entity.ID_ESTUDIO && x.ID_ANIO == Entity.ID_ANIO).FirstOrDefault();
                Microsoft.Reporting.WinForms.ReportDataSource _respuesta = new Microsoft.Reporting.WinForms.ReportDataSource();
                cRealizacionEstudios CamposBase = new cRealizacionEstudios();
                if (_DatosCriminodiagnosticoComun != null)
                {
                    CamposBase.VersionDelitoSegunInterno = _DatosCriminodiagnosticoComun.P1_VERSION_DELITO;
                    CamposBase.MomentoCometerDelitoEncontrabaInfluenciaDrogaTexto = string.Format(" ( {0} ) NO \n ( {1} ) SI", _DatosCriminodiagnosticoComun.P1_DROGADO == "N" ? "X" : string.Empty, _DatosCriminodiagnosticoComun.P1_DROGADO == "S" ? "X" : string.Empty);
                    CamposBase.DescripcionDrogasTexto = string.Format(" ( {0} ) ALCOHOL \n ( {1} ) DROGAS ILEGALES \n ( {2} ) OTRA ", _DatosCriminodiagnosticoComun.P1_ALCOHOL == (short)eSINO.SI ? "X" : string.Empty, _DatosCriminodiagnosticoComun.P1_DROGRA_ILEGAL == (short)eSINO.SI ? "X" : string.Empty,
                        _DatosCriminodiagnosticoComun.P1_OTRA == (short)eSINO.SI ? "X" : string.Empty);
                    CamposBase.Criminogenesis = _DatosCriminodiagnosticoComun.P2_CRIMINOGENESIS;
                    CamposBase.AntecedentesEvolucionConductasParaAntiSociales = _DatosCriminodiagnosticoComun.P3_CONDUCTA_ANTISOCIAL;
                    CamposBase.ClasificCriminologTexto = string.Format(" ( {0} ) PRIMO DELINCUENTE \n ( {1} ) REINCIDENTE ESPECIFICO \n ( {2} ) REINCIDENTE GENÉRICO \n ( {3} ) HABITUAL \n ( {4} ) PROFESIONAL ", _DatosCriminodiagnosticoComun.P4_CLASIFICACION_CRIMINOLOGICA == (short)eCapacidadCriminal.PRIMO_DELINCUENTE ? "X" : string.Empty,
                        _DatosCriminodiagnosticoComun.P4_CLASIFICACION_CRIMINOLOGICA == (short)eCapacidadCriminal.REINCIDENTE_ESPECIFICO ? "X" : string.Empty, _DatosCriminodiagnosticoComun.P4_CLASIFICACION_CRIMINOLOGICA == (short)eCapacidadCriminal.REINCIDENTE_GENERICO ? "X" : string.Empty,
                        _DatosCriminodiagnosticoComun.P4_CLASIFICACION_CRIMINOLOGICA == (short)eCapacidadCriminal.HABITUAL ? "X" : string.Empty, _DatosCriminodiagnosticoComun.P4_CLASIFICACION_CRIMINOLOGICA == (short)eCapacidadCriminal.PROFESIONAL ? "X" : string.Empty);
                    CamposBase.IntimidacionPenaImpuestaTexto = string.Format("SI ( {0} ) NO ( {1} )", _DatosCriminodiagnosticoComun.P5_INTIMIDACION == "S" ? "X" : string.Empty, _DatosCriminodiagnosticoComun.P5_INTIMIDACION == "N" ? "X" : string.Empty);
                    CamposBase.PorqueIntimidacion = _DatosCriminodiagnosticoComun.P5_PORQUE;
                    CamposBase.CapacidadCriminalActualTexto = string.Format("ALTA ( {0} ) MEDIA ( {1} ) MEDIA BAJA ( {2} ) BAJA ( {3} )", _DatosCriminodiagnosticoComun.P6_CAPACIDAD_CRIMINAL == (short)eCapacidad.ALTA ? "X" : string.Empty, _DatosCriminodiagnosticoComun.P6_CAPACIDAD_CRIMINAL == (short)eCapacidad.MEDIA ? "X" : string.Empty,
                        _DatosCriminodiagnosticoComun.P6_CAPACIDAD_CRIMINAL == (short)eCapacidad.MEDIA_BAJA ? "X" : string.Empty, _DatosCriminodiagnosticoComun.P6_CAPACIDAD_CRIMINAL == (short)eCapacidad.BAJA ? "X" : string.Empty);
                    CamposBase.EgocentrismoTexto = string.Format("ALTO ( {0} ) MEDIO ( {1} ) MEDIO BAJO ( {2} ) BAJO ( {3} )", _DatosCriminodiagnosticoComun.P6A_EGOCENTRICO == (short)eCapacidad.ALTA ? "X" : string.Empty, _DatosCriminodiagnosticoComun.P6A_EGOCENTRICO == (short)eCapacidad.MEDIA ? "X" : string.Empty,
                        _DatosCriminodiagnosticoComun.P6A_EGOCENTRICO == (short)eCapacidad.MEDIA_BAJA ? "X" : string.Empty, _DatosCriminodiagnosticoComun.P6A_EGOCENTRICO == (short)eCapacidad.BAJA ? "X" : string.Empty);
                    CamposBase.LabilidadAfectivaTexto = string.Format("ALTA ( {0} ) MEDIA ( {1} ) MEDIA BAJA ( {2} ) BAJA ( {3} )", _DatosCriminodiagnosticoComun.P6B_LIABILIDAD_EFECTIVA == (short)eCapacidad.ALTA ? "X" : string.Empty, _DatosCriminodiagnosticoComun.P6B_LIABILIDAD_EFECTIVA == (short)eCapacidad.MEDIA ? "X" : string.Empty,
                        _DatosCriminodiagnosticoComun.P6B_LIABILIDAD_EFECTIVA == (short)eCapacidad.MEDIA_BAJA ? "X" : string.Empty, _DatosCriminodiagnosticoComun.P6B_LIABILIDAD_EFECTIVA == (short)eCapacidad.BAJA ? "X" : string.Empty);
                    CamposBase.AgresividadTexto = string.Format("ALTA ( {0} ) MEDIA ( {1} ) MEDIA BAJA ( {2} ) BAJA ( {3} )", _DatosCriminodiagnosticoComun.P6C_AGRESIVIDAD == (short)eCapacidad.ALTA ? "X" : string.Empty, _DatosCriminodiagnosticoComun.P6C_AGRESIVIDAD == (short)eCapacidad.MEDIA ? "X" : string.Empty,
                        _DatosCriminodiagnosticoComun.P6C_AGRESIVIDAD == (short)eCapacidad.MEDIA_BAJA ? "X" : string.Empty, _DatosCriminodiagnosticoComun.P6C_AGRESIVIDAD == (short)eCapacidad.BAJA ? "X" : string.Empty);
                    CamposBase.IndiferenciaAfectTexto = string.Format("ALTA ( {0} ) MEDIA ( {1} ) MEDIA BAJA ( {2} ) BAJA ( {3} )", _DatosCriminodiagnosticoComun.P6D_INDIFERENCIA_AFECTIVA == (short)eCapacidad.ALTA ? "X" : string.Empty, _DatosCriminodiagnosticoComun.P6D_INDIFERENCIA_AFECTIVA == (short)eCapacidad.MEDIA ? "X" : string.Empty,
                        _DatosCriminodiagnosticoComun.P6D_INDIFERENCIA_AFECTIVA == (short)eCapacidad.MEDIA_BAJA ? "X" : string.Empty, _DatosCriminodiagnosticoComun.P6D_INDIFERENCIA_AFECTIVA == (short)eCapacidad.BAJA ? "X" : string.Empty);
                    CamposBase.AdaptabSocialTexto = string.Format("ALTA ( {0} ) MEDIA ( {1} ) MEDIA BAJA ( {2} ) BAJA ( {3} )", _DatosCriminodiagnosticoComun.P7_ADAPTACION_SOCIAL == (short)eCapacidad.ALTA ? "X" : string.Empty, _DatosCriminodiagnosticoComun.P7_ADAPTACION_SOCIAL == (short)eCapacidad.MEDIA ? "X" : string.Empty,
                        _DatosCriminodiagnosticoComun.P7_ADAPTACION_SOCIAL == (short)eCapacidad.MEDIA_BAJA ? "X" : string.Empty, _DatosCriminodiagnosticoComun.P7_ADAPTACION_SOCIAL == (short)eCapacidad.BAJA ? "X" : string.Empty);
                    CamposBase.IndicePeligrosidadCriminActualTexto = string.Format("MÁXIMA ( {0} ) MEDIA-MÁXIMA ( {1} ) MEDIA ( {2} ) MEDIA-MÍNIMA ( {3} ) MÍNIMA ( {4} )", _DatosCriminodiagnosticoComun.P8_INDICE_PELIGROSIDAD == (short)ePeligrosidad.MAXIMA ? "X" : string.Empty, _DatosCriminodiagnosticoComun.P8_INDICE_PELIGROSIDAD == (short)ePeligrosidad.MEDIA_MAXIMA ? "X" : string.Empty,
                        _DatosCriminodiagnosticoComun.P8_INDICE_PELIGROSIDAD == (short)ePeligrosidad.MEDIA ? "X" : string.Empty, _DatosCriminodiagnosticoComun.P8_INDICE_PELIGROSIDAD == (short)ePeligrosidad.MEDIA_MINIMA ? "X" : string.Empty, _DatosCriminodiagnosticoComun.P8_INDICE_PELIGROSIDAD == (short)ePeligrosidad.MINIMA ? "X" : string.Empty);
                    CamposBase.PronosticoReincidenciaTexto = string.Format("ALTA ( {0} ) MEDIA ( {1} ) MEDIA BAJA ( {2} ) BAJA ( {3} )", _DatosCriminodiagnosticoComun.P9_PRONOSTICO_REINCIDENCIA == (short)eCapacidad.ALTA ? "X" : string.Empty, _DatosCriminodiagnosticoComun.P9_PRONOSTICO_REINCIDENCIA == (short)eCapacidad.MEDIA ? "X" : string.Empty,
                        _DatosCriminodiagnosticoComun.P9_PRONOSTICO_REINCIDENCIA == (short)eCapacidad.MEDIA_BAJA ? "X" : string.Empty, _DatosCriminodiagnosticoComun.P9_PRONOSTICO_REINCIDENCIA == (short)eCapacidad.BAJA ? "X" : string.Empty);
                    CamposBase.Dictamen = string.Format(" ( {0} ) FAVORABLE \n ( {1} ) DESFAVORABLE ", _DatosCriminodiagnosticoComun.P10_DICTAMEN_REINSERCION == (short)eDiagnosticoDictamen.FAVORABLE ? "X" : string.Empty, _DatosCriminodiagnosticoComun.P10_DICTAMEN_REINSERCION == (short)eDiagnosticoDictamen.DESFAVORABLE ? "X" : string.Empty);
                    CamposBase.MotivacionDictamen = _DatosCriminodiagnosticoComun.P10_MOTIVACION_DICTAMEN;
                    CamposBase.CasoNegativoSenialeProgramasARemitir = _DatosCriminodiagnosticoComun.P11_PROGRAMAS_REMITIRSE;
                    CamposBase.RequiereTratExtraMurosTexto = string.Format("Si ( {0} ) NO ( {1} )", _DatosCriminodiagnosticoComun.P12_TRATAMIENTO_EXTRAMUROS == "S" ? "X" : string.Empty, _DatosCriminodiagnosticoComun.P12_TRATAMIENTO_EXTRAMUROS == "N" ? "X" : string.Empty);
                    CamposBase.CualExtramuros = _DatosCriminodiagnosticoComun.P12_CUAL;
                    CamposBase.FechaRealizacionEstudio = _DatosCriminodiagnosticoComun.ESTUDIO_FEC.HasValue ? _DatosCriminodiagnosticoComun.ESTUDIO_FEC.Value.ToString("dd/MM/yyyy") : string.Empty;
                    CamposBase.Actitud = _DatosCriminodiagnosticoComun.ELABORO;
                    CamposBase.Abdomen = _DatosCriminodiagnosticoComun.COORDINADOR;
                }

                _Datos.Add(CamposBase);
                _respuesta.Value = _Datos;
                _respuesta.Name = "DataSet2";
                return _respuesta;
            }
            catch (Exception exc)
            {
                throw exc;
            }
        }

        private Microsoft.Reporting.WinForms.ReportDataSource DatosEstudioSocioFamiliarFueroComun(PERSONALIDAD Entity)
        {
            try
            {
                var _Datos = new List<cRealizacionEstudios>();
                var _DatosEstudioSocioEconomicoComun = new cPersonalidadEstudioSocioFamiliarComun().GetData(x => x.ID_INGRESO == Entity.ID_INGRESO && x.ID_IMPUTADO == Entity.ID_IMPUTADO && x.ID_CENTRO == Entity.ID_CENTRO && x.ID_ESTUDIO == Entity.ID_ESTUDIO && x.ID_ANIO == Entity.ID_ANIO).FirstOrDefault();
                Microsoft.Reporting.WinForms.ReportDataSource _respuesta = new Microsoft.Reporting.WinForms.ReportDataSource();
                cRealizacionEstudios CamposBase = new cRealizacionEstudios();
                CamposBase.NombreInterno = string.Format("{0} {1} {2} ", SelectIngreso.IMPUTADO != null ? !string.IsNullOrEmpty(SelectIngreso.IMPUTADO.NOMBRE) ? SelectIngreso.IMPUTADO.NOMBRE.Trim() : string.Empty : string.Empty,
                    SelectIngreso.IMPUTADO != null ? !string.IsNullOrEmpty(SelectIngreso.IMPUTADO.PATERNO) ? SelectIngreso.IMPUTADO.PATERNO.Trim() : string.Empty : string.Empty,
                    SelectIngreso.IMPUTADO != null ? !string.IsNullOrEmpty(SelectIngreso.IMPUTADO.MATERNO) ? SelectIngreso.IMPUTADO.MATERNO.Trim() : string.Empty : string.Empty);
                var municipio = new cMunicipio().Obtener(SelectIngreso.IMPUTADO.NACIMIENTO_ESTADO.Value, SelectIngreso.IMPUTADO.NACIMIENTO_MUNICIPIO.Value).FirstOrDefault();
                CamposBase.LugarFecNacInterno = string.Format("{0} {1}", SelectIngreso.IMPUTADO.NACIMIENTO_MUNICIPIO.HasValue ? !string.IsNullOrEmpty(municipio.MUNICIPIO1) ? municipio.MUNICIPIO1.Trim() : string.Empty : string.Empty,
                    SelectIngreso.IMPUTADO.NACIMIENTO_FECHA.HasValue ? SelectIngreso.IMPUTADO.NACIMIENTO_FECHA.Value.ToString("dd/MM/yyyy") : string.Empty);
                //CamposBase.EstadoCivilInterno = SelectIngreso.IMPUTADO != null ? SelectIngreso.IMPUTADO.ID_ESTADO_CIVIL.HasValue ? !string.IsNullOrEmpty(SelectIngreso.IMPUTADO.ESTADO_CIVIL.DESCR) ? SelectIngreso.IMPUTADO.ESTADO_CIVIL.DESCR.Trim() : string.Empty : string.Empty : string.Empty;
                CamposBase.EstadoCivilInterno = SelectIngreso != null ? SelectIngreso.ID_ESTADO_CIVIL.HasValue ? !string.IsNullOrEmpty(SelectIngreso.ESTADO_CIVIL.DESCR) ? SelectIngreso.ESTADO_CIVIL.DESCR.Trim() : string.Empty : string.Empty : string.Empty;
                //CamposBase.DomicilioInterno = string.Format("{0} {1}", SelectIngreso.IMPUTADO != null ? !string.IsNullOrEmpty(SelectIngreso.IMPUTADO.DOMICILIO_CALLE) ? SelectIngreso.IMPUTADO.DOMICILIO_CALLE.Trim() : string.Empty : string.Empty,
                //    SelectIngreso.IMPUTADO != null ? SelectIngreso.IMPUTADO.DOMICILIO_NUM_EXT.HasValue ? SelectIngreso.IMPUTADO.DOMICILIO_NUM_EXT.Value.ToString() : string.Empty : string.Empty);
                CamposBase.DomicilioInterno = string.Format("{0} {1}", SelectIngreso != null ? !string.IsNullOrEmpty(SelectIngreso.DOMICILIO_CALLE) ? SelectIngreso.DOMICILIO_CALLE.Trim() : string.Empty : string.Empty,
                    SelectIngreso != null ? SelectIngreso.DOMICILIO_NUM_EXT.HasValue ? SelectIngreso.DOMICILIO_NUM_EXT.Value.ToString() : string.Empty : string.Empty);
                //CamposBase.TelefonoInterno = SelectIngreso.IMPUTADO != null ? SelectIngreso.IMPUTADO.TELEFONO.HasValue ? SelectIngreso.IMPUTADO.TELEFONO.Value.ToString() : string.Empty : string.Empty;
                CamposBase.TelefonoInterno = SelectIngreso != null ? SelectIngreso.TELEFONO.HasValue ? SelectIngreso.TELEFONO.Value.ToString() : string.Empty : string.Empty;

                if (_DatosEstudioSocioEconomicoComun != null)
                {
                    CamposBase.FamiliaPrimaria = _DatosEstudioSocioEconomicoComun.P21_FAMILIA_PRIMARIA;
                    CamposBase.FamiliaSecundaria = _DatosEstudioSocioEconomicoComun.P22_FAMILIA_SECUNDARIA;
                    CamposBase.AdultoMayorProgramaEspecial = string.Format(" ( {0} ) SI \n ( {1} ) NO", _DatosEstudioSocioEconomicoComun.P3_TERCERA_EDAD == "S" ? "X" : string.Empty, _DatosEstudioSocioEconomicoComun.P3_TERCERA_EDAD == "N" ? "X" : string.Empty);
                    CamposBase.RecibeVisText = string.Format(" ( {0} ) NO \n ( {1} ) SI", _DatosEstudioSocioEconomicoComun.P4_RECIBE_VISITA == "N" ? "X" : string.Empty, _DatosEstudioSocioEconomicoComun.P4_RECIBE_VISITA == "S" ? "X" : string.Empty);
                    CamposBase.QuienesVisitaG = string.Format(" PADRE ( {0} ) MADRE ( {1} ) ESPOSA(O)/CONCUBINA(O) ( {2} ) HERMANOS ( {3} ) HIJOS ( {4} ) OTROS FAMILIARES ( {5} ) ESPECIFICAR QUIEN {6}",
                        _DatosEstudioSocioEconomicoComun.P4_PADRE == (short)eSINO.SI ? "X" : string.Empty, _DatosEstudioSocioEconomicoComun.P4_MADRE == (short)eSINO.SI ? "X" : string.Empty, _DatosEstudioSocioEconomicoComun.P4_ESPOSOA == (short)eSINO.SI ? "X" : string.Empty,
                        _DatosEstudioSocioEconomicoComun.P4_HERMANOS == (short)eSINO.SI ? "X" : string.Empty, _DatosEstudioSocioEconomicoComun.P4_HIJOS == (short)eSINO.SI ? "X" : string.Empty, _DatosEstudioSocioEconomicoComun.P4_OTROS == (short)eSINO.SI ? "X" : string.Empty,
                        _DatosEstudioSocioEconomicoComun.P4_OTROS_EPECIFICAR);
                    CamposBase.FrecuenciaV = _DatosEstudioSocioEconomicoComun.P4_FRECUENCIA;
                    CamposBase.RazonNoRecibeVisitasTexto = _DatosEstudioSocioEconomicoComun.P4_MOTIVO_NO_VISITA;
                    CamposBase.MantieneComunicTelefonicaTexto = string.Format(" ( {0} ) NO (ESPECIFICAR POR QUE) {1} \n ( {2} ) SI (ESPECIFICAR QUIEN) {3}", _DatosEstudioSocioEconomicoComun.P5_COMUNICACION_TELEFONICA == "N" ? "X" : string.Empty,
                        _DatosEstudioSocioEconomicoComun.P5_COMUNICACION_TELEFONICA == "N" ? _DatosEstudioSocioEconomicoComun.P5_NO_POR_QUE : string.Empty,
                        _DatosEstudioSocioEconomicoComun.P5_COMUNICACION_TELEFONICA == "S" ? "X" : string.Empty,
                        _DatosEstudioSocioEconomicoComun.P5_COMUNICACION_TELEFONICA == "S" ? _DatosEstudioSocioEconomicoComun.P5_NO_POR_QUE : string.Empty);
                    CamposBase.CuentaApoyoFamiliaAlgunaPersona = _DatosEstudioSocioEconomicoComun.P6_APOYO_EXTERIOR;
                    CamposBase.PlanesSerExternado = _DatosEstudioSocioEconomicoComun.P7_PLANES_INTERNO;
                    CamposBase.QuienViviraSerExternado = _DatosEstudioSocioEconomicoComun.P7_VIVIRA;
                    CamposBase.CuentaOfertaTrabajoTexto = string.Format(" ( {0} ) SI (ESPECIFICAR) {1} \n ( {2} ) NO", _DatosEstudioSocioEconomicoComun.P8_OFERTA_TRABAJO == "S" ? "X" : string.Empty, _DatosEstudioSocioEconomicoComun.P8_OFERTA_ESPECIFICAR, _DatosEstudioSocioEconomicoComun.P8_OFERTA_TRABAJO == "N" ? "X" : string.Empty);
                    CamposBase.CuentaAvalMoralTexto = string.Format(" ( {0} ) SI (ESPECIFICAR) {1} \n ( {2} ) NO", _DatosEstudioSocioEconomicoComun.P9_AVAL_MORAL == "S" ? "X" : string.Empty, _DatosEstudioSocioEconomicoComun.P9_AVAL_ESPECIFICAR, _DatosEstudioSocioEconomicoComun.P9_AVAL_MORAL == "N" ? "X" : string.Empty);
                    CamposBase.Dictamen = string.Format(" ( {0} ) FAVORABLE \n ( {1} ) DESFAVORABLE", _DatosEstudioSocioEconomicoComun.P10_DICTAMEN == (short)eSINO.SI ? "X" : string.Empty, _DatosEstudioSocioEconomicoComun.P10_DICTAMEN == (short)eSINO.NO ? "X" : string.Empty);
                    CamposBase.MotivacionDictamen = _DatosEstudioSocioEconomicoComun.P11_MOTIVACION_DICTAMEN;
                    CamposBase.Abdomen = _DatosEstudioSocioEconomicoComun.ELABORO;
                    CamposBase.Actitud = _DatosEstudioSocioEconomicoComun.COORDINADOR;
                    CamposBase.FechaRealizacionEstudio = _DatosEstudioSocioEconomicoComun.ESTUDIO_FEC.HasValue ? _DatosEstudioSocioEconomicoComun.ESTUDIO_FEC.Value.ToString("dd/MM/yyyy") : string.Empty;
                }

                _Datos.Add(CamposBase);
                _respuesta.Value = _Datos;
                _respuesta.Name = "DataSet2";
                return _respuesta;
            }

            catch (Exception exc)
            {
                throw exc;
            }
        }

        private Microsoft.Reporting.WinForms.ReportDataSource DatosVisitas(PERSONALIDAD Entity)
        {
            try
            {
                Microsoft.Reporting.WinForms.ReportDataSource _respuesta = new Microsoft.Reporting.WinForms.ReportDataSource();
                var _DetalleVisitas = new List<cPadronVisitantesRealizacionEstudios>();
                var Datos = new cComunicacionComun().GetData(x => x.ID_INGRESO == Entity.ID_INGRESO && x.ID_IMPUTADO == Entity.ID_IMPUTADO && x.ID_CENTRO == Entity.ID_CENTRO && x.ID_ESTUDIO == Entity.ID_ESTUDIO && x.ID_ANIO == Entity.ID_ANIO);
                if (Datos.Any())
                {
                    foreach (var item in Datos)
                    {
                        _DetalleVisitas.Add(new cPadronVisitantesRealizacionEstudios
                        {
                            Frecuencia = !string.IsNullOrEmpty(item.FRECUENCIA) ? item.FRECUENCIA.Trim() : string.Empty,
                            NombreTelefono = !string.IsNullOrEmpty(item.NOMBRE) ? item.NOMBRE.Trim() : string.Concat(string.Empty, " / ", !string.IsNullOrEmpty(item.TELEFONO) ? item.TELEFONO.Trim() : string.Empty),
                            Parentesco = item.ID_TIPO_REFERENCIA.HasValue ? !string.IsNullOrEmpty(item.TIPO_REFERENCIA.DESCR) ? item.TIPO_REFERENCIA.DESCR.Trim() : string.Empty : string.Empty
                        });
                    };
                }

                else
                {
                    var _DatoVacio = new cPadronVisitantesRealizacionEstudios()
                    {
                        Frecuencia = string.Empty,
                        NombreTelefono = string.Empty,
                        Parentesco = string.Empty
                    };//se crea un registro sin nada, el report data source espera un valor
                    _DetalleVisitas.Add(_DatoVacio);
                }

                _respuesta.Value = _DetalleVisitas;
                _respuesta.Name = "DataSet3";
                return _respuesta;
            }

            catch (System.Exception exc)
            {
                throw exc;
            }
        }

        private Microsoft.Reporting.WinForms.ReportDataSource DatosProgramasFortalecimientoComun(PERSONALIDAD Entity)
        {
            try
            {
                Microsoft.Reporting.WinForms.ReportDataSource _respuesta = new Microsoft.Reporting.WinForms.ReportDataSource();
                var _DetalleVisitas = new List<cProgramasSocioEconomicoComunReporte>();
                var Datos = new cGruposSocioEconomicoComun().GetData(x => x.ID_INGRESO == Entity.ID_INGRESO && x.ID_IMPUTADO == Entity.ID_IMPUTADO && x.ID_CENTRO == Entity.ID_CENTRO && x.ID_ESTUDIO == Entity.ID_ESTUDIO && x.ID_ANIO == Entity.ID_ANIO && x.ID_TIPO_PROGRAMA == (short)eTipoActividad.NUCLEO_FAM);
                if (Datos.Any())
                {
                    foreach (var item in Datos)
                    {
                        _DetalleVisitas.Add(new cProgramasSocioEconomicoComunReporte
                        {
                            Generico = item.ACTIVIDAD != null ? !string.IsNullOrEmpty(item.ACTIVIDAD.DESCR) ? item.ACTIVIDAD.DESCR.Trim() : string.Empty : string.Empty,
                            Generico2 = item.CONGREGACION,
                            Generico3 = item.PERIODO,
                            Generico4 = item.OBSERVACIONES
                        });
                    };
                }

                else
                {
                    var _DatoVacio = new cProgramasSocioEconomicoComunReporte()
                    {
                        Generico = string.Empty,
                        Generico2 = string.Empty,
                        Generico3 = string.Empty,
                        Generico4 = string.Empty
                    };//se crea un registro sin nada, el report data source espera un valor
                    _DetalleVisitas.Add(_DatoVacio);
                }

                _respuesta.Value = _DetalleVisitas;
                _respuesta.Name = "DataSet5";
                return _respuesta;
            }
            catch (Exception exc)
            {
                throw exc;
            }
        }

        private Microsoft.Reporting.WinForms.ReportDataSource DatosProgramasReligiososComun(PERSONALIDAD Entity)
        {
            try
            {
                Microsoft.Reporting.WinForms.ReportDataSource _respuesta = new Microsoft.Reporting.WinForms.ReportDataSource();
                var _DetalleVisitas = new List<cProgramasSocioEconomicoComunReporte>();
                var Datos = new cGruposSocioEconomicoComun().GetData(x => x.ID_INGRESO == Entity.ID_INGRESO && x.ID_IMPUTADO == Entity.ID_IMPUTADO && x.ID_CENTRO == Entity.ID_CENTRO && x.ID_ESTUDIO == Entity.ID_ESTUDIO && x.ID_ANIO == Entity.ID_ANIO && x.ID_TIPO_PROGRAMA == (short)eTipoActividad.APOYO_ESPIRITUAL_Y_RELIGIOSO);
                if (Datos.Any())
                {
                    foreach (var item in Datos)
                    {
                        _DetalleVisitas.Add(new cProgramasSocioEconomicoComunReporte
                        {
                            Generico = item.ACTIVIDAD != null ? !string.IsNullOrEmpty(item.ACTIVIDAD.DESCR) ? item.ACTIVIDAD.DESCR.Trim() : string.Empty : string.Empty,//item.TIPO_PROGRAMA != null ? !string.IsNullOrEmpty(item.TIPO_PROGRAMA.NOMBRE) ? item.TIPO_PROGRAMA.NOMBRE.Trim() : string.Empty : string.Empty,
                            Generico2 = item.CONGREGACION,
                            Generico3 = item.PERIODO,
                            Generico4 = item.OBSERVACIONES
                        });
                    };
                }

                else
                {
                    var _DatoVacio = new cProgramasSocioEconomicoComunReporte()
                    {
                        Generico = string.Empty,
                        Generico2 = string.Empty,
                        Generico3 = string.Empty,
                        Generico4 = string.Empty
                    };//se crea un registro sin nada, el report data source espera un valor
                    _DetalleVisitas.Add(_DatoVacio);
                }

                _respuesta.Value = _DetalleVisitas;
                _respuesta.Name = "DataSet4";
                return _respuesta;
            }
            catch (Exception exc)
            {
                throw exc;
            }
        }

        private Microsoft.Reporting.WinForms.ReportDataSource DatosEstudioEducativoCulturalDeportivoFueroComun(PERSONALIDAD Entity)
        {
            try
            {
                var _Datos = new List<cRealizacionEstudios>();
                var dato = new cPersonalidadEstudioEducativoCultDepComun().GetData(x => x.ID_INGRESO == Entity.ID_INGRESO && x.ID_IMPUTADO == Entity.ID_IMPUTADO && x.ID_CENTRO == Entity.ID_CENTRO && x.ID_ESTUDIO == Entity.ID_ESTUDIO && x.ID_ANIO == Entity.ID_ANIO).FirstOrDefault();
                Microsoft.Reporting.WinForms.ReportDataSource _respuesta = new Microsoft.Reporting.WinForms.ReportDataSource();
                cRealizacionEstudios CamposBase = new cRealizacionEstudios();

                if (dato != null)
                {
                    CamposBase.Dictamen = string.Format(" ( {0} ) FAVORABLE \n ( {1} ) DESFAVORABLE", dato.P3_DICTAMEN == (short)eDiagnosticoDictamen.FAVORABLE ? "X" : string.Empty, dato.P3_DICTAMEN == (short)eDiagnosticoDictamen.DESFAVORABLE ? "X" : string.Empty);
                    CamposBase.MotivacionDictamen = dato.P4_MOTIVACION_DICTAMEN;
                    CamposBase.FechaRealizacionEstudio = dato.ESTUDIO_FEC.HasValue ? dato.ESTUDIO_FEC.Value.ToString("dd/MM/yyyy") : string.Empty;
                    CamposBase.Abdomen = dato.COORDINADOR;
                    CamposBase.Actitud = dato.ELABORO;
                }

                else
                {
                    CamposBase.Dictamen = string.Format(" ( {0} ) FAVORABLE \n ( {1} ) DESFAVORABLE", string.Empty, string.Empty);
                    CamposBase.MotivacionDictamen = string.Empty;
                    CamposBase.FechaRealizacionEstudio = string.Empty;
                    CamposBase.Abdomen = string.Empty;
                    CamposBase.Actitud = string.Empty;
                }


                _Datos.Add(CamposBase);
                _respuesta.Value = _Datos;
                _respuesta.Name = "DataSet2";
                return _respuesta;
            }
            catch (Exception exc)
            {
                throw exc;
            }
        }

        private Microsoft.Reporting.WinForms.ReportDataSource DatosEscolaridadAnterior(PERSONALIDAD Entity)
        {
            try
            {
                Microsoft.Reporting.WinForms.ReportDataSource _respuesta = new Microsoft.Reporting.WinForms.ReportDataSource();
                var _DetalleEscolaridad = new List<cEscolaridadAnterior>();
                var Datos = new cEscolaridadesAnterioresIngreso().GetData(x => x.ID_INGRESO == Entity.ID_INGRESO && x.ID_IMPUTADO == Entity.ID_IMPUTADO && x.ID_CENTRO == Entity.ID_CENTRO && x.ID_ESTUDIO == Entity.ID_ESTUDIO && x.ID_ANIO == Entity.ID_ANIO && x.RENDIMIENTO == null && x.INTERES == null);
                if (Datos != null && Datos.Any())
                {
                    foreach (var item in Datos)
                    {
                        _DetalleEscolaridad.Add(new cEscolaridadAnterior
                        {
                            DescripcionConcluida = item.CONCLUIDA == "S" ? "SI" : "NO",
                            NivelEducativo = item.EDUCACION_GRADO != null ? !string.IsNullOrEmpty(item.EDUCACION_GRADO.DESCR) ? item.EDUCACION_GRADO.DESCR.Trim() : string.Empty : string.Empty,
                            ObservacionesEscolaridadAnterior = !string.IsNullOrEmpty(item.OBSERVACION) ? item.OBSERVACION.Trim() : string.Empty
                        });
                    };
                }

                else
                {
                    var _DatoVacio = new cEscolaridadAnterior()
                    {
                        DescripcionConcluida = string.Empty,
                        NivelEducativo = string.Empty,
                        ObservacionesEscolaridadAnterior = string.Empty
                    };//se crea un registro sin nada, el report data source espera un valor
                    _DetalleEscolaridad.Add(_DatoVacio);
                }

                _respuesta.Value = _DetalleEscolaridad;
                _respuesta.Name = "DataSet3";
                return _respuesta;
            }
            catch (Exception exc)
            {
                throw exc;
            }
        }

        private Microsoft.Reporting.WinForms.ReportDataSource DatosActividadesEscolares(PERSONALIDAD Entity)
        {
            try
            {
                Microsoft.Reporting.WinForms.ReportDataSource _respuesta = new Microsoft.Reporting.WinForms.ReportDataSource();
                var _DetalleEscolaridad = new List<cActividadesEducativas>();
                var Datos = new cEscolaridadesAnterioresIngreso().GetData(x => x.ID_INGRESO == Entity.ID_INGRESO && x.ID_IMPUTADO == Entity.ID_IMPUTADO && x.ID_CENTRO == Entity.ID_CENTRO && x.ID_ESTUDIO == Entity.ID_ESTUDIO && x.ID_ANIO == Entity.ID_ANIO && !string.IsNullOrEmpty(x.RENDIMIENTO) && !string.IsNullOrEmpty(x.INTERES));
                if (Datos != null && Datos.Any())
                {
                    foreach (var item in Datos)
                    {
                        _DetalleEscolaridad.Add(new cActividadesEducativas
                        {
                            Concluida = item.CONCLUIDA == "S" ? "SI" : "NO",
                            Nivel = item.EDUCACION_GRADO != null ? !string.IsNullOrEmpty(item.EDUCACION_GRADO.DESCR) ? item.EDUCACION_GRADO.DESCR.Trim() : string.Empty : string.Empty,
                            Observaciones = !string.IsNullOrEmpty(item.OBSERVACION) ? item.OBSERVACION.Trim() : string.Empty,
                            Interes = item.INTERES,
                            RendimientoEscolar = item.RENDIMIENTO
                        });
                    };
                }

                else
                {
                    var _DatoVacio = new cActividadesEducativas()
                    {
                        Concluida = string.Empty,
                        Interes = string.Empty,
                        Nivel = string.Empty,
                        Observaciones = string.Empty,
                        RendimientoEscolar = string.Empty
                    };//se crea un registro sin nada, el report data source espera un valor

                    _DetalleEscolaridad.Add(_DatoVacio);
                }

                _respuesta.Value = _DetalleEscolaridad;
                _respuesta.Name = "DataSet4";
                return _respuesta;
            }
            catch (Exception exc)
            {
                throw exc;
            }
        }

        private Microsoft.Reporting.WinForms.ReportDataSource DatosActividadesDeportivas(PERSONALIDAD Entity)
        {
            try
            {
                Microsoft.Reporting.WinForms.ReportDataSource _respuesta = new Microsoft.Reporting.WinForms.ReportDataSource();
                var _DetalleEscolaridad = new List<cActividadesCulturales>();
                var Padre = new cPersonalidadEstudioEducativoCultDepComun().GetData(x => x.ID_ESTUDIO == Entity.ID_ESTUDIO && x.ID_INGRESO == Entity.ID_INGRESO && x.ID_IMPUTADO == Entity.ID_IMPUTADO).FirstOrDefault();
                if (Padre != null)
                {
                    var Datos = new cActividadesEstudioEducativoFueroComun().GetData(x => x.ID_ESTUDIO == Entity.ID_ESTUDIO && x.ID_INGRESO == Entity.ID_INGRESO && x.ID_IMPUTADO == Entity.ID_IMPUTADO && x.ID_TIPO_PROGRAMA == (short)eTiposP.DEPORTIVAS);
                    if (Datos != null && Datos.Any())
                    {
                        foreach (var item in Datos)
                        {
                            _DetalleEscolaridad.Add(new cActividadesCulturales
                            {
                                Actividad = item.ACTIVIDAD1 != null ? !string.IsNullOrEmpty(item.ACTIVIDAD1.DESCR) ? item.ACTIVIDAD1.DESCR.Trim() : string.Empty : string.Empty,
                                Duracion = item.DURACION,
                                Observaciones = item.OBSERVACION,
                                Programa = item.TIPO_PROGRAMA != null ? !string.IsNullOrEmpty(item.TIPO_PROGRAMA.NOMBRE) ? item.TIPO_PROGRAMA.NOMBRE.Trim() : string.Empty : string.Empty
                            });
                        };
                    };
                }
                else
                {
                    var _DatoVacio = new cActividadesCulturales()
                    {
                        Actividad = string.Empty,
                        Duracion = string.Empty,
                        Observaciones = string.Empty,
                        Programa = string.Empty
                    };//se crea un registro sin nada, el report data source espera un valor

                    _DetalleEscolaridad.Add(_DatoVacio);
                }

                _respuesta.Value = _DetalleEscolaridad;
                _respuesta.Name = "DataSet6";
                return _respuesta;
            }
            catch (Exception exc)
            {
                throw exc;
            }
        }

        private Microsoft.Reporting.WinForms.ReportDataSource DatosActividadesCulturales(PERSONALIDAD Entity)
        {
            try
            {
                Microsoft.Reporting.WinForms.ReportDataSource _respuesta = new Microsoft.Reporting.WinForms.ReportDataSource();
                var _DetalleEscolaridad = new List<cActividadesCulturales>();
                var Datos = new cActividadesEstudioEducativoFueroComun().GetData(x => x.ID_INGRESO == Entity.ID_INGRESO && x.ID_IMPUTADO == Entity.ID_IMPUTADO && x.ID_CENTRO == Entity.ID_CENTRO && x.ID_ESTUDIO == Entity.ID_ESTUDIO && x.ID_ANIO == Entity.ID_ANIO && x.ID_TIPO_PROGRAMA == (short)eTiposP.CULTURALES);
                if (Datos != null && Datos.Any())
                {
                    foreach (var item in Datos)
                    {
                        _DetalleEscolaridad.Add(new cActividadesCulturales
                        {
                            Actividad = item.ACTIVIDAD1 != null ? !string.IsNullOrEmpty(item.ACTIVIDAD1.DESCR) ? item.ACTIVIDAD1.DESCR.Trim() : string.Empty : string.Empty,
                            Duracion = item.DURACION,
                            Observaciones = item.OBSERVACION,
                            Programa = item.TIPO_PROGRAMA != null ? !string.IsNullOrEmpty(item.TIPO_PROGRAMA.NOMBRE) ? item.TIPO_PROGRAMA.NOMBRE.Trim() : string.Empty : string.Empty
                        });
                    };
                }

                else
                {
                    var _DatoVacio = new cActividadesCulturales()
                    {
                        Actividad = string.Empty,
                        Duracion = string.Empty,
                        Observaciones = string.Empty,
                        Programa = string.Empty
                    };//se crea un registro sin nada, el report data source espera un valor
                    _DetalleEscolaridad.Add(_DatoVacio);
                }

                _respuesta.Value = _DetalleEscolaridad;
                _respuesta.Name = "DataSet5";
                return _respuesta;
            }
            catch (Exception exc)
            {
                throw exc;
            }
        }

        private Microsoft.Reporting.WinForms.ReportDataSource DatosEstudioCapacitacionTrabajoPenitenciarioFueroComun(PERSONALIDAD Entity)
        {
            try
            {
                var _Datos = new List<cRealizacionEstudios>();
                var _DatosTrabajo = new cEstudioCapacitTrabajoPenitenciarioComun().GetData(x => x.ID_INGRESO == Entity.ID_INGRESO && x.ID_IMPUTADO == Entity.ID_IMPUTADO && x.ID_CENTRO == Entity.ID_CENTRO && x.ID_ESTUDIO == Entity.ID_ESTUDIO && x.ID_ANIO == Entity.ID_ANIO).FirstOrDefault();
                Microsoft.Reporting.WinForms.ReportDataSource _respuesta = new Microsoft.Reporting.WinForms.ReportDataSource();
                cRealizacionEstudios CamposBase = new cRealizacionEstudios();
                if (_DatosTrabajo != null)
                {
                    CamposBase.OficioActivDesempenadaAntesReclucion = _DatosTrabajo.P1_TRABAJO_ANTES;
                    CamposBase.ResponsabilidadTexto = string.Format("BUENA ( {0} ) \t REGULAR ( {1} ) \t MALA ( {2} ) ", _DatosTrabajo.P3_RESPONSABILIDAD == "B" ? "X" : string.Empty, _DatosTrabajo.P3_RESPONSABILIDAD == "R" ? "X" : string.Empty, _DatosTrabajo.P3_RESPONSABILIDAD == "M" ? "X" : string.Empty);
                    CamposBase.CalidadTrabajoTexto = string.Format("BUENA ( {0} ) \t REGULAR ( {1} ) \t MALA ( {2} ) ", _DatosTrabajo.P3_CALIDAD == "B" ? "X" : string.Empty, _DatosTrabajo.P3_CALIDAD == "R" ? "X" : string.Empty, _DatosTrabajo.P3_CALIDAD == "M" ? "X" : string.Empty);
                    CamposBase.PerseveranciaTexto = string.Format("BUENA ( {0} ) \t REGULAR ( {1} ) \t MALA ( {2} ) ", _DatosTrabajo.P3_PERSEVERANCIA == "B" ? "X" : string.Empty, _DatosTrabajo.P3_PERSEVERANCIA == "R" ? "X" : string.Empty, _DatosTrabajo.P3_PERSEVERANCIA == "M" ? "X" : string.Empty);
                    CamposBase.CuentaFondoAhorroTexto = string.Format(" SI ( {0} ) \n NO ( {1} )", _DatosTrabajo.P4_FONDO_AHORRO == "S" ? "X" : string.Empty, _DatosTrabajo.P4_FONDO_AHORRO == "N" ? "X" : string.Empty);
                    CamposBase.DiasOtrosCentros = _DatosTrabajo.P5_DIAS_OTROS_CENTROS.HasValue ? _DatosTrabajo.P5_DIAS_OTROS_CENTROS.Value.ToString() : string.Empty;
                    CamposBase.DiasCentroActual = _DatosTrabajo.P5_DIAS_CENTRO_ACTUAL.HasValue ? _DatosTrabajo.P5_DIAS_CENTRO_ACTUAL.Value.ToString() : string.Empty;
                    CamposBase.DiasTotalLaborados = _DatosTrabajo.P5_DIAS_LABORADOS.HasValue ? _DatosTrabajo.P5_DIAS_LABORADOS.Value.ToString() : string.Empty;
                    CamposBase.Dictamen = string.Format(" ( {0} ) FAVORABLE \n ( {1} ) DESFAVORABLE", _DatosTrabajo.P6_DICTAMEN == "F" ? "X" : string.Empty, _DatosTrabajo.P6_DICTAMEN == "D" ? "X" : string.Empty);
                    CamposBase.MotivacionDictamen = _DatosTrabajo.P7_MOTIVACION;
                    CamposBase.FechaRealizacionEstudio = _DatosTrabajo.ESTUDIO_FEC.HasValue ? _DatosTrabajo.ESTUDIO_FEC.Value.ToString("dd/MM/yyyy") : string.Empty;
                    CamposBase.Abdomen = _DatosTrabajo.COORDINADOR;
                    CamposBase.Actitud = _DatosTrabajo.ELABORO;
                }

                _Datos.Add(CamposBase);
                _respuesta.Value = _Datos;
                _respuesta.Name = "DataSet2";
                return _respuesta;
            }
            catch (Exception exc)
            {
                throw exc;
            }
        }

        private Microsoft.Reporting.WinForms.ReportDataSource DatosInformeAreaSeguridadCustodiaFueroComun(PERSONALIDAD Entity)
        {
            try
            {
                var _Datos = new List<cRealizacionEstudios>();
                Microsoft.Reporting.WinForms.ReportDataSource _respuesta = new Microsoft.Reporting.WinForms.ReportDataSource();
                cRealizacionEstudios CamposBase = new cRealizacionEstudios();
                var datosReporte = new cInformeAreaSeguridadCustodiaComun().GetData(x => x.ID_INGRESO == Entity.ID_INGRESO && x.ID_IMPUTADO == Entity.ID_IMPUTADO && x.ID_CENTRO == Entity.ID_CENTRO && x.ID_ESTUDIO == Entity.ID_ESTUDIO && x.ID_ANIO == Entity.ID_ANIO).FirstOrDefault();
                if (datosReporte != null)
                {
                    CamposBase.ConductaObservadaCentroTexto = string.Format(" ( {0} ) BUENA \t ( {1} ) REGULAR \t ( {2} ) MALA ", datosReporte.P1_CONDUCTA_CENTRO == "B" ? "X" : string.Empty, datosReporte.P1_CONDUCTA_CENTRO == "R" ? "X" : string.Empty, datosReporte.P1_CONDUCTA_CENTRO == "M" ? "X" : string.Empty);
                    CamposBase.ConductaAutoridadTexto = string.Format(" ( {0} ) BUENA \t ( {1} ) REGULAR \t ( {2} ) MALA ", datosReporte.P2_CONDUCTA_AUTORIDAD == "B" ? "X" : string.Empty, datosReporte.P2_CONDUCTA_AUTORIDAD == "R" ? "X" : string.Empty, datosReporte.P2_CONDUCTA_AUTORIDAD == "M" ? "X" : string.Empty);
                    CamposBase.ConductaGralTexto = string.Format(" ( {0} ) REBELDE ( {1} ) AGRESIVO ( {2} ) DISCIPLINADO ( {3} ) SE ADAPTA SIN CONFLICTOS ", datosReporte.P3_CONDUCTA_GENERAL == 1 ? "X" : string.Empty, datosReporte.P3_CONDUCTA_GENERAL == 2 ? "X" : string.Empty, datosReporte.P3_CONDUCTA_GENERAL == 3 ? "X" : string.Empty, datosReporte.P3_CONDUCTA_GENERAL == 4 ? "X" : string.Empty);
                    CamposBase.RelacionCompanerosTexto = string.Format(" ( {0} ) AISLAMIENTO ( {1} ) AGRESIVIDAD ( {2} ) CAMARADERIA ( {3} ) DOMINANTE ( {4} ) INDIFERENTE ", datosReporte.P4_RELACION_COMPANEROS == 1 ? "X" : string.Empty, datosReporte.P4_RELACION_COMPANEROS == 2 ? "X" : string.Empty, datosReporte.P4_RELACION_COMPANEROS == 3 ? "X" : string.Empty, datosReporte.P4_RELACION_COMPANEROS == 4 ? "X" : string.Empty, datosReporte.P4_RELACION_COMPANEROS == 5 ? "X" : string.Empty);
                    CamposBase.RegistraCorrectivosDiscTexto = string.Format(" SI ( {0} ) NO ( {1} )", datosReporte.P5_CORRECTIVOS == "S" ? "X" : string.Empty, datosReporte.P5_CORRECTIVOS == "N" ? "X" : string.Empty);
                    CamposBase.OpinionConductaGralInterno = string.Format(" ( {0} ) BUENA ( {1} ) REGULAR ( {2} ) MALA ", datosReporte.P6_OPINION_CONDUCTA == "B" ? "X" : string.Empty, datosReporte.P6_OPINION_CONDUCTA == "R" ? "X" : string.Empty, datosReporte.P6_OPINION_CONDUCTA == "M" ? "X" : string.Empty);
                    CamposBase.Dictamen = string.Format(" ( {0} ) FAVORABLE \n ( {1} ) DESFAVORABLE", datosReporte.P7_DICTAMEN == "F" ? "X" : string.Empty, datosReporte.P7_DICTAMEN == "D" ? "X" : string.Empty);
                    CamposBase.MotivacionDictamen = datosReporte.P8_MOTIVACION;
                    CamposBase.FechaRealizacionEstudio = datosReporte.ESTUDIO_FEC.HasValue ? datosReporte.ESTUDIO_FEC.Value.ToString("dd/MM/yyyy") : string.Empty;
                    CamposBase.Abdomen = datosReporte.COMANDANTE;
                    CamposBase.Actitud = datosReporte.ELABORO;
                }

                _Datos.Add(CamposBase);
                _respuesta.Value = _Datos;
                _respuesta.Name = "DataSet2";
                return _respuesta;
            }
            catch (System.Exception exc)
            {
                throw exc;
            }
        }

        private Microsoft.Reporting.WinForms.ReportDataSource SancionesInformeAreaSeguridadCustodiaFueroComun(PERSONALIDAD Entity)
        {
            Microsoft.Reporting.WinForms.ReportDataSource _respuesta = new Microsoft.Reporting.WinForms.ReportDataSource();

            try
            {
                var _Datos = new List<cCorrectivosDisc>();
                cCorrectivosDisc CamposBase = new cCorrectivosDisc();
                var datosReporte = new cCorrectivosSeguridadFueroComun().GetData(x => x.ID_INGRESO == Entity.ID_INGRESO && x.ID_IMPUTADO == Entity.ID_IMPUTADO && x.ID_CENTRO == Entity.ID_CENTRO && x.ID_ESTUDIO == Entity.ID_ESTUDIO && x.ID_ANIO == Entity.ID_ANIO);
                if (datosReporte != null)
                {
                    if (datosReporte.Any())
                        foreach (var item in datosReporte)
                        {
                            CamposBase = new cCorrectivosDisc();
                            CamposBase.Fecha = item.CORRECTIVO_FEC.HasValue ? item.CORRECTIVO_FEC.Value.ToString("dd/MM/yyyy") : string.Empty;
                            CamposBase.Motivo = item.MOTIVO;
                            CamposBase.Sancion = item.SANCION;
                            _Datos.Add(CamposBase);
                        };
                };

                _respuesta.Value = _Datos;
                _respuesta.Name = "DataSet3";
            }

            catch (System.Exception ex)
            {
                throw ex;
            }

            return _respuesta;
        }

        #endregion


        #region Informe de Seguridad
        private void InfoSeguridad(PERSONALIDAD Entity)
        {
            try
            {
                var _seguridadActual = new cInformeAreaSeguridadCustodiaComun().GetData(x => x.ID_INGRESO == Entity.ID_INGRESO && x.ID_IMPUTADO == Entity.ID_IMPUTADO && x.ID_CENTRO == Entity.ID_CENTRO && x.ID_ANIO == Entity.ID_ANIO && x.ID_ESTUDIO == Entity.ID_ESTUDIO).FirstOrDefault();
                if (_seguridadActual == null)
                {
                    LstSanciones = new ObservableCollection<SANCION>();
                    LstCorrectivosSeguridad = new ObservableCollection<PFC_IX_CORRECTIVO>();
                    IdConductaGral = IdRelacionCompanieros = -1;
                    short _consecutivo = 1;
                    FechaDictamenInformeSeguridadCustodia = Fechas.GetFechaDateServer;
                    MotivacionDictamenInformeSeguridadCustodia = IdDictamenInformeSeguridadCustodia = IdConductaObservadaCentro = IdConductaAutoridad = IdRegistraCorrectivosDisciplinarios = IdOpinionGralConductaInterno = string.Empty;
                    var DetalleSanciones = new cSancion().GetData(x => x.ID_INGRESO == SelectIngreso.ID_INGRESO && x.ID_IMPUTADO == SelectIngreso.ID_IMPUTADO);
                    if (DetalleSanciones != null && DetalleSanciones.Any())
                    {
                        IdRegistraCorrectivosDisciplinarios = "S";//SI TIENE CORRECTIVOS REGISTRADOS
                        foreach (var item in DetalleSanciones)
                        {
                            var _NuevoCorrectivo = new PFC_IX_CORRECTIVO()
                            {
                                CORRECTIVO_FEC = item.INICIA_FEC,
                                ID_ANIO = item.ID_ANIO,
                                ID_CENTRO = item.ID_CENTRO,
                                ID_CORRECTIVO = _consecutivo,
                                ID_IMPUTADO = item.ID_IMPUTADO,
                                ID_INGRESO = item.ID_INGRESO,
                                MOTIVO = item.INCIDENTE != null ? !string.IsNullOrEmpty(item.INCIDENTE.MOTIVO) ? item.INCIDENTE.MOTIVO.Trim() : string.Empty : string.Empty,
                                SANCION = item.SANCION_TIPO != null ? !string.IsNullOrEmpty(item.SANCION_TIPO.DESCR) ? item.SANCION_TIPO.DESCR.Trim() : string.Empty : string.Empty
                            };

                            LstCorrectivosSeguridad.Add(_NuevoCorrectivo);
                            _consecutivo++;
                        };
                    }
                }
                else
                {
                    LstCorrectivosSeguridad = new ObservableCollection<PFC_IX_CORRECTIVO>();
                    if (_seguridadActual.PFC_IX_CORRECTIVO != null && _seguridadActual.PFC_IX_CORRECTIVO.Any())
                        foreach (var item in _seguridadActual.PFC_IX_CORRECTIVO)
                            LstCorrectivosSeguridad.Add(item);

                    IdConductaGral = _seguridadActual.P3_CONDUCTA_GENERAL ?? -1;
                    IdRelacionCompanieros = _seguridadActual.P4_RELACION_COMPANEROS ?? -1;
                    FechaDictamenInformeSeguridadCustodia = _seguridadActual.ESTUDIO_FEC;
                    MotivacionDictamenInformeSeguridadCustodia = _seguridadActual.P8_MOTIVACION;
                    IdDictamenInformeSeguridadCustodia = _seguridadActual.P7_DICTAMEN ?? string.Empty;
                    IdConductaObservadaCentro = _seguridadActual.P1_CONDUCTA_CENTRO ?? string.Empty;
                    IdConductaAutoridad = _seguridadActual.P2_CONDUCTA_AUTORIDAD ?? string.Empty;
                    IdRegistraCorrectivosDisciplinarios = _seguridadActual.P5_CORRECTIVOS ?? string.Empty;
                    IdOpinionGralConductaInterno = _seguridadActual.P6_OPINION_CONDUCTA ?? string.Empty;
                }
            }
            catch (Exception exc)
            {
                throw exc;
            }
        }
        #endregion


        private string FrecuenciaPorAduana(PERSONA Entity)
        {
            try
            {
                if (Entity == null)
                    return "SIN DATO";

                var _VisitasPorPersona = new cAduanaIngreso().GetData(x => x.ADUANA != null && x.ADUANA.ID_PERSONA == Entity.ID_PERSONA);
                if (_VisitasPorPersona.Any())
                {
                    var _VisitasPorAnio = _VisitasPorPersona.GroupBy(x => x.ENTRADA_FEC.Value.Year);
                    if (_VisitasPorAnio != null && _VisitasPorAnio.Any())
                    {
                        var _VisitasMes = _VisitasPorAnio.OrderByDescending(x => x.Count()).FirstOrDefault();//OBTIENE EL AñO CON MAS VISITAS
                        if (_VisitasMes != null)
                        {
                            var _MesesPorAnio = _VisitasMes.GroupBy(x => x.ENTRADA_FEC.Value.Month);//OBTIENE EL MES CON MAS VISITAS
                            var _MesConMasVisitas = _MesesPorAnio.OrderByDescending(x => x.Count()).FirstOrDefault();
                            if (_MesConMasVisitas != null)
                            {
                                if (_MesConMasVisitas.Count() >= (int)eFrecuencia.SEMANAL)
                                    return "SEMANAL";
                                if (_MesConMasVisitas.Count() == (int)eFrecuencia.QUINCENAL)
                                    return "QUINCENAL";
                                if (_MesConMasVisitas.Count() == (int)eFrecuencia.MENSUAL)
                                    return "MENSUAL";
                                if (_MesConMasVisitas.Count() == (int)eFrecuencia.ANUAL)
                                    return "ANUAL";
                            };
                        };
                    };
                };

                return "SIN DATO";
            }
            catch (Exception exc)
            {
                throw exc;
            }
        }
        #region Estudio Medico Info
        private void InfoEducativoComun(PERSONALIDAD Entity)
        {
            try
            {
                var _EstudioEd = new cPersonalidadEstudioEducativoCultDepComun().GetData(x => x.ID_INGRESO == Entity.ID_INGRESO && x.ID_IMPUTADO == Entity.ID_IMPUTADO && x.ID_CENTRO == Entity.ID_CENTRO && x.ID_ANIO == Entity.ID_ANIO && x.ID_ESTUDIO == Entity.ID_ESTUDIO).FirstOrDefault();
                LstEscolaridadesEducativo = new ObservableCollection<PFC_VII_ESCOLARIDAD_ANTERIOR>();
                LstActividadesEducativas = new ObservableCollection<PFC_VII_ESCOLARIDAD_ANTERIOR>();
                LstAcitividadesCulturales = new ObservableCollection<PFC_VII_ACTIVIDAD>();
                LstActividadesDeportivas = new ObservableCollection<PFC_VII_ACTIVIDAD>();

                if (_EstudioEd == null)
                {
                    IdDictamenEducativoComun = 0;
                    MotivacionDictamenEducativoComun = string.Empty;
                    FechaEstudioEducativoComun = Fechas.GetFechaDateServer;
                    LstEscolaridadesEducativo = new ObservableCollection<PFC_VII_ESCOLARIDAD_ANTERIOR>();
                    var GruposIngreso = SelectIngreso.GRUPO_PARTICIPANTE;
                    if (GruposIngreso != null && GruposIngreso.Any())
                    {
                        foreach (var item in GruposIngreso)
                        {
                            //DESCOMENTAR ESTA PARTE CUANDO MNDEN LA MODIFICACION A LA TABLA PFC_VII_ESCOLARIDAD_ANTERIOR
                            //if (item.ID_TIPO_PROGRAMA == 1 || item.ID_TIPO_PROGRAMA == 2 || item.ID_TIPO_PROGRAMA == 3)
                            //{
                            //    LstActividadesEducativas.Add(new PFC_VII_ESCOLARIDAD_ANTERIOR
                            //        {
                            //            CONCLUIDA = item.NOTA_TECNICA != null ? item.NOTA_TECNICA.Any() ? !string.IsNullOrEmpty(item.NOTA_TECNICA.FirstOrDefault().NOTA) ? item.NOTA_TECNICA.FirstOrDefault().NOTA.Trim() : string.Empty : string.Empty : string.Empty,
                            //            ID_ANIO = Entity.ID_ANIO,
                            //            ID_CENTRO = Entity.ID_CENTRO,
                            //            ID_ESTUDIO = Entity.ID_ESTUDIO,
                            //            ID_GRADO = -1,
                            //            ID_IMPUTADO = Entity.ID_IMPUTADO,
                            //            ID_INGRESO = Entity.ID_INGRESO,
                            //            INTERES = string.Empty,
                            //            OBSERVACION = string.Empty,
                            //            RENDIMIENTO = string.Empty,
                            //            EDUCACION_GRADO = item.ACTIVIDAD.es
                            //        });

                            //    continue;
                            //}

                            if (item.ID_TIPO_PROGRAMA == 168)
                            {
                                LstAcitividadesCulturales.Add(new PFC_VII_ACTIVIDAD
                                {
                                    ACTIVIDAD = item.ACTIVIDAD != null ? !string.IsNullOrEmpty(item.ACTIVIDAD.DESCR) ? item.ACTIVIDAD.DESCR.Trim() : string.Empty : string.Empty,
                                    DURACION = item.GRUPO != null ? string.Format("{0} - {1}",
                                                    item.GRUPO.FEC_INICIO.HasValue ? item.GRUPO.FEC_INICIO.Value.ToString("dd/MM/yyyy") : string.Empty,
                                                    item.GRUPO.FEC_FIN.HasValue ? item.GRUPO.FEC_FIN.Value.ToString("dd/MM/yyyy") : string.Empty) : string.Empty,
                                    ID_ACTIVIDAD = item.ID_ACTIVIDAD,
                                    ID_ANIO = Entity.ID_ANIO,
                                    ID_CENTRO = Entity.ID_CENTRO,
                                    ID_ESTUDIO = Entity.ID_ESTUDIO,
                                    ID_INGRESO = Entity.ID_INGRESO,
                                    ID_IMPUTADO = Entity.ID_IMPUTADO,
                                    ID_PROGRAMA = item.ID_TIPO_PROGRAMA,
                                    OBSERVACION = item.NOTA_TECNICA != null ? item.NOTA_TECNICA.Any() ? !string.IsNullOrEmpty(item.NOTA_TECNICA.FirstOrDefault().NOTA) ? item.NOTA_TECNICA.FirstOrDefault().NOTA.Trim() : string.Empty : string.Empty : string.Empty,
                                    ID_TIPO_PROGRAMA = item.ID_TIPO_PROGRAMA,
                                    TIPO = string.Empty,
                                    ACTIVIDAD1 = item.ACTIVIDAD,
                                    TIPO_PROGRAMA = item.ACTIVIDAD != null ? item.ACTIVIDAD.TIPO_PROGRAMA : null
                                });

                                continue;
                            }

                            if (item.ID_TIPO_PROGRAMA == 169)
                            {
                                LstActividadesDeportivas.Add(new PFC_VII_ACTIVIDAD
                                {
                                    ACTIVIDAD = item.ACTIVIDAD != null ? !string.IsNullOrEmpty(item.ACTIVIDAD.DESCR) ? item.ACTIVIDAD.DESCR.Trim() : string.Empty : string.Empty,
                                    DURACION = item.GRUPO != null ? string.Format("{0} - {1}",
                                    item.GRUPO.FEC_INICIO.HasValue ? item.GRUPO.FEC_INICIO.Value.ToString("dd/MM/yyyy") : string.Empty,
                                    item.GRUPO.FEC_FIN.HasValue ? item.GRUPO.FEC_FIN.Value.ToString("dd/MM/yyyy") : string.Empty) : string.Empty,
                                    ID_ACTIVIDAD = item.ID_ACTIVIDAD,
                                    ID_ANIO = Entity.ID_ANIO,
                                    ID_CENTRO = Entity.ID_CENTRO,
                                    ID_ESTUDIO = Entity.ID_ESTUDIO,
                                    ID_INGRESO = Entity.ID_INGRESO,
                                    ID_IMPUTADO = Entity.ID_IMPUTADO,
                                    ID_PROGRAMA = item.ID_TIPO_PROGRAMA,
                                    OBSERVACION = item.NOTA_TECNICA != null ? item.NOTA_TECNICA.Any() ? !string.IsNullOrEmpty(item.NOTA_TECNICA.FirstOrDefault().NOTA) ? item.NOTA_TECNICA.FirstOrDefault().NOTA.Trim() : string.Empty : string.Empty : string.Empty,
                                    ID_TIPO_PROGRAMA = item.ID_TIPO_PROGRAMA,
                                    TIPO = string.Empty,
                                    ACTIVIDAD1 = item.ACTIVIDAD,
                                    TIPO_PROGRAMA = item.ACTIVIDAD != null ? item.ACTIVIDAD.TIPO_PROGRAMA : null
                                });

                                continue;
                            }
                        }
                    }
                }

                else
                {
                    //IsEnabledEducativoD = false;
                    IdDictamenEducativoComun = _EstudioEd.P3_DICTAMEN ?? 0;
                    MotivacionDictamenEducativoComun = _EstudioEd.P4_MOTIVACION_DICTAMEN;
                    FechaEstudioEducativoComun = _EstudioEd.ESTUDIO_FEC;
                    if (_EstudioEd.PFC_VII_ESCOLARIDAD_ANTERIOR != null)
                    {
                        LstEscolaridadesEducativo = new ObservableCollection<PFC_VII_ESCOLARIDAD_ANTERIOR>();
                        LstActividadesEducativas = new ObservableCollection<PFC_VII_ESCOLARIDAD_ANTERIOR>();
                        LstAcitividadesCulturales = new ObservableCollection<PFC_VII_ACTIVIDAD>();
                        LstActividadesDeportivas = new ObservableCollection<PFC_VII_ACTIVIDAD>();
                        var ActividadesEducativas = _EstudioEd.PFC_VII_ESCOLARIDAD_ANTERIOR.Where(x => !string.IsNullOrEmpty(x.INTERES) && !string.IsNullOrEmpty(x.RENDIMIENTO));
                        var EscolaridadAnterior = _EstudioEd.PFC_VII_ESCOLARIDAD_ANTERIOR.Where(x => string.IsNullOrEmpty(x.INTERES) && string.IsNullOrEmpty(x.RENDIMIENTO));
                        if (EscolaridadAnterior != null && EscolaridadAnterior.Any())
                            foreach (var item in EscolaridadAnterior)
                                LstEscolaridadesEducativo.Add(item);

                        if (ActividadesEducativas != null && ActividadesEducativas.Any())
                            foreach (var item in ActividadesEducativas)
                                LstActividadesEducativas.Add(item);
                    };

                    if (_EstudioEd.PFC_VII_ACTIVIDAD != null && _EstudioEd.PFC_VII_ACTIVIDAD.Any())
                        foreach (var item in _EstudioEd.PFC_VII_ACTIVIDAD)
                        {
                            if (item.ID_TIPO_PROGRAMA == 168)
                            {
                                LstAcitividadesCulturales.Add(item);
                                continue;
                            }

                            if (item.ID_TIPO_PROGRAMA == 169)
                            {
                                LstActividadesDeportivas.Add(item);
                                continue;
                            }
                        };
                }
            }
            catch (Exception exc)
            {
                throw exc;
            }
        }
        #endregion
        #region Capacitacion Comun
        private void InfoCapacitacionComun(PERSONALIDAD Entity)
        {
            try
            {
                var _Trabajo = new cEstudioCapacitTrabajoPenitenciarioComun().GetData(x => x.ID_INGRESO == Entity.ID_INGRESO && x.ID_IMPUTADO == Entity.ID_IMPUTADO && x.ID_CENTRO == Entity.ID_CENTRO && x.ID_ANIO == Entity.ID_ANIO && x.ID_ESTUDIO == Entity.ID_ESTUDIO).FirstOrDefault();
                if (_Trabajo == null)
                {
                    //ValidacionesEstudioCapacitacionTrabajoFueroComun();
                    #region EMI Ingreso
                    var _EmiByIngreso = SelectIngreso.EMI_INGRESO.Any() ? SelectIngreso.EMI_INGRESO.FirstOrDefault().EMI != null ? SelectIngreso.EMI_INGRESO.FirstOrDefault().EMI.EMI_ULTIMOS_EMPLEOS : null : null;
                    if (_EmiByIngreso != null && _EmiByIngreso.Any())
                    {
                        var _UltimoEmpleo = _EmiByIngreso.FirstOrDefault(x => x.ULTIMO_EMPLEO_ANTES_DETENCION == "S");
                        if (_UltimoEmpleo != null)
                            ActividadOficioAntesReclusion += string.Format("OCUPACIÓN: {0},  DURACIÓN: {1},  EMPRESA: {2}",
                                _UltimoEmpleo.ID_OCUPACION.HasValue ? !string.IsNullOrEmpty(_UltimoEmpleo.OCUPACION.DESCR) ? _UltimoEmpleo.OCUPACION.DESCR.Trim() : string.Empty : string.Empty,
                                !string.IsNullOrEmpty(_UltimoEmpleo.DURACION) ? _UltimoEmpleo.DURACION.Trim() : string.Empty, !string.IsNullOrEmpty(_UltimoEmpleo.EMPRESA) ? _UltimoEmpleo.EMPRESA.Trim() : string.Empty);
                    };

                    #endregion

                    LstCapacitacionLaboral = new ObservableCollection<PFC_VIII_ACTIVIDAD_LABORAL>();
                    LstActivNoGratificadas = new ObservableCollection<PFC_VIII_ACTIVIDAD_LABORAL>();
                    LstActivGratificadas = new ObservableCollection<PFC_VIII_ACTIVIDAD_LABORAL>();

                    var GruposIngreso = SelectIngreso.GRUPO_PARTICIPANTE;
                    if (GruposIngreso != null && GruposIngreso.Any())
                    {
                        foreach (var item in GruposIngreso)
                        {
                            if (item.ID_TIPO_PROGRAMA == 147)
                            {
                                LstCapacitacionLaboral.Add(new PFC_VIII_ACTIVIDAD_LABORAL
                                    {
                                        CONCLUYO = item.NOTA_TECNICA != null ? item.NOTA_TECNICA.Any() ? "S" : "N" : "N",
                                        ID_ACTIVIDAD = item.ID_ACTIVIDAD,
                                        ID_ANIO = Entity.ID_ANIO,
                                        ID_CENTRO = Entity.ID_CENTRO,
                                        ID_ESTUDIO = Entity.ID_ESTUDIO,
                                        ID_IMPUTADO = Entity.ID_IMPUTADO,
                                        ID_INGRESO = Entity.ID_INGRESO,
                                        ID_TIPO_PROGRAMA = item.ID_TIPO_PROGRAMA,
                                        OBSERVACION = item.NOTA_TECNICA != null ? item.NOTA_TECNICA.Any() ? !string.IsNullOrEmpty(item.NOTA_TECNICA.FirstOrDefault().NOTA) ? item.NOTA_TECNICA.FirstOrDefault().NOTA.Trim() : string.Empty : string.Empty : string.Empty,
                                        PERIODO = item.GRUPO != null ? string.Format("{0} - {1}",
                                            item.GRUPO.FEC_INICIO.HasValue ? item.GRUPO.FEC_INICIO.Value.ToString("dd/MM/yyyy") : string.Empty,
                                            item.GRUPO.FEC_FIN.HasValue ? item.GRUPO.FEC_FIN.Value.ToString("dd/MM/yyyy") : string.Empty) : string.Empty,
                                        ACTIVIDAD = item.ACTIVIDAD,
                                        TIPO_PROGRAMA = item.ACTIVIDAD != null ? item.ACTIVIDAD.TIPO_PROGRAMA : null
                                    });

                                continue;
                            }

                            if (item.ID_TIPO_PROGRAMA == 148)
                            {
                                LstActivNoGratificadas.Add(new PFC_VIII_ACTIVIDAD_LABORAL
                                {
                                    CONCLUYO = item.NOTA_TECNICA != null ? item.NOTA_TECNICA.Any() ? "S" : "N" : "N",
                                    ID_ACTIVIDAD = item.ID_ACTIVIDAD,
                                    ID_ANIO = Entity.ID_ANIO,
                                    ID_CENTRO = Entity.ID_CENTRO,
                                    ID_ESTUDIO = Entity.ID_ESTUDIO,
                                    ID_IMPUTADO = Entity.ID_IMPUTADO,
                                    ID_INGRESO = Entity.ID_INGRESO,
                                    ID_TIPO_PROGRAMA = item.ID_TIPO_PROGRAMA,
                                    OBSERVACION = item.NOTA_TECNICA != null ? item.NOTA_TECNICA.Any() ? !string.IsNullOrEmpty(item.NOTA_TECNICA.FirstOrDefault().NOTA) ? item.NOTA_TECNICA.FirstOrDefault().NOTA.Trim() : string.Empty : string.Empty : string.Empty,
                                    PERIODO = item.GRUPO != null ? string.Format("{0} - {1}",
                                        item.GRUPO.FEC_INICIO.HasValue ? item.GRUPO.FEC_INICIO.Value.ToString("dd/MM/yyyy") : string.Empty,
                                        item.GRUPO.FEC_FIN.HasValue ? item.GRUPO.FEC_FIN.Value.ToString("dd/MM/yyyy") : string.Empty) : string.Empty,
                                    ACTIVIDAD = item.ACTIVIDAD,
                                    TIPO_PROGRAMA = item.ACTIVIDAD != null ? item.ACTIVIDAD.TIPO_PROGRAMA : null
                                });

                                continue;
                            }

                            if (item.ID_TIPO_PROGRAMA == 149)
                            {
                                LstActivGratificadas.Add(new PFC_VIII_ACTIVIDAD_LABORAL
                                {
                                    CONCLUYO = item.NOTA_TECNICA != null ? item.NOTA_TECNICA.Any() ? "S" : "N" : "N",
                                    ID_ACTIVIDAD = item.ID_ACTIVIDAD,
                                    ID_ANIO = Entity.ID_ANIO,
                                    ID_CENTRO = Entity.ID_CENTRO,
                                    ID_ESTUDIO = Entity.ID_ESTUDIO,
                                    ID_IMPUTADO = Entity.ID_IMPUTADO,
                                    ID_INGRESO = Entity.ID_INGRESO,
                                    ID_TIPO_PROGRAMA = item.ID_TIPO_PROGRAMA,
                                    OBSERVACION = item.NOTA_TECNICA != null ? item.NOTA_TECNICA.Any() ? !string.IsNullOrEmpty(item.NOTA_TECNICA.FirstOrDefault().NOTA) ? item.NOTA_TECNICA.FirstOrDefault().NOTA.Trim() : string.Empty : string.Empty : string.Empty,
                                    PERIODO = item.GRUPO != null ? string.Format("{0} - {1}",
                                        item.GRUPO.FEC_INICIO.HasValue ? item.GRUPO.FEC_INICIO.Value.ToString("dd/MM/yyyy") : string.Empty,
                                        item.GRUPO.FEC_FIN.HasValue ? item.GRUPO.FEC_FIN.Value.ToString("dd/MM/yyyy") : string.Empty) : string.Empty,
                                    ACTIVIDAD = item.ACTIVIDAD,
                                    TIPO_PROGRAMA = item.ACTIVIDAD != null ? item.ACTIVIDAD.TIPO_PROGRAMA : null
                                });

                                continue;
                            }
                        }
                    }

                    IdResponsabilidad = IdCalidadTrabajo = IdPerseverancia = IdCuentaConFondoAhorro = IdDicatmenSeguridadCustodiaDict = MotivacionDictamenSeguridadCustodia = string.Empty;
                    DiasEfectivosLaboradosEnOtrosCentros = DiasEfectivosLaboradosEnCentroActual = DiasEfectivosLaboradosTotalDiasLaborados = 0;
                    FechaSeguridadCustodiaDictamen = PeriodoDondeRealizoActividadLaboral = Fechas.GetFechaDateServer;
                }

                else
                {
                    //IsEnabledTrabajoDatos = false;
                    ActividadOficioAntesReclusion = _Trabajo.P1_TRABAJO_ANTES;
                    LstCapacitacionLaboral = new ObservableCollection<PFC_VIII_ACTIVIDAD_LABORAL>();
                    LstActivNoGratificadas = new ObservableCollection<PFC_VIII_ACTIVIDAD_LABORAL>();
                    LstActivGratificadas = new ObservableCollection<PFC_VIII_ACTIVIDAD_LABORAL>();
                    IdResponsabilidad = _Trabajo.P3_RESPONSABILIDAD ?? string.Empty;
                    IdCalidadTrabajo = _Trabajo.P3_CALIDAD ?? string.Empty;
                    IdPerseverancia = _Trabajo.P3_PERSEVERANCIA ?? string.Empty;
                    IdCuentaConFondoAhorro = _Trabajo.P4_FONDO_AHORRO ?? string.Empty;
                    IdDicatmenSeguridadCustodiaDict = _Trabajo.P6_DICTAMEN ?? string.Empty;
                    DiasEfectivosLaboradosEnOtrosCentros = _Trabajo.P5_DIAS_OTROS_CENTROS;
                    DiasEfectivosLaboradosEnCentroActual = _Trabajo.P5_DIAS_CENTRO_ACTUAL;
                    DiasEfectivosLaboradosTotalDiasLaborados = _Trabajo.P5_DIAS_LABORADOS;
                    PeriodoDondeRealizoActividadLaboral = _Trabajo.P5_PERIODO_LABORAL;
                    MotivacionDictamenSeguridadCustodia = _Trabajo.P7_MOTIVACION;
                    FechaSeguridadCustodiaDictamen = _Trabajo.ESTUDIO_FEC;

                    if (_Trabajo.PFC_VIII_ACTIVIDAD_LABORAL.Any())
                    {
                        foreach (var item in _Trabajo.PFC_VIII_ACTIVIDAD_LABORAL)
                        {
                            if (item.ID_TIPO_PROGRAMA == 147)
                                LstCapacitacionLaboral.Add(item);

                            if (item.ID_TIPO_PROGRAMA == 148)
                                LstActivNoGratificadas.Add(item);

                            if (item.ID_TIPO_PROGRAMA == 149)
                                LstActivGratificadas.Add(item);
                        };
                    };
                }
            }
            catch (Exception exc)
            {
                throw exc;
            }
        }

        private void ValidacionDiasLaborados(short? DiasUno, short? DiasDos)
        {
            try
            {
                if (DiasUno.HasValue && DiasDos.HasValue)
                {
                    short _absoluto1 = Math.Abs(DiasUno.Value);
                    short _absoluto2 = Math.Abs(DiasDos.Value);
                    DiasEfectivosLaboradosTotalDiasLaborados = short.Parse((_absoluto1 + _absoluto2).ToString());
                    return;
                }

                DiasEfectivosLaboradosTotalDiasLaborados = short.Parse((DiasUno != null ? DiasUno : 0 + DiasDos != null ? DiasDos : 0).ToString());
            }
            catch (Exception exc)
            {
                throw exc;
            }
        }
        #endregion
        #region Socio Familiar Comun
        private void EstudioSocioEComun(PERSONALIDAD Entity)
        {
            try
            {
                var _SocioFam = new cPersonalidadEstudioSocioFamiliarComun().GetData(x => x.ID_INGRESO == Entity.ID_INGRESO && x.ID_IMPUTADO == Entity.ID_IMPUTADO && x.ID_CENTRO == Entity.ID_CENTRO && x.ID_ANIO == Entity.ID_ANIO && x.ID_ESTUDIO == Entity.ID_ESTUDIO).FirstOrDefault();
                if (_SocioFam == null)
                {
                    #region Estudio Socio Economico por Ingreso
                    var _EstudioSocioe = SelectIngreso.SOCIOECONOMICO;
                    if (_EstudioSocioe != null)
                    {
                        if (_EstudioSocioe.SOCIOE_GPOFAMPRI != null)
                            FamiliaPrimaria = string.Format("GRUPO FAMILIAR: {0},  RELACIÓN INTRAFAMILIAR: {1},  CUANTAS PERSONAS VIVEN EN EL HOGAR: {2},  CUANTAS PERSONAS LABORAN: {3},  INGRESO MENSUAL FAM. : {4},  EGRESO MENSUAL FAM. : {5}, ",
                                _EstudioSocioe.SOCIOE_GPOFAMPRI.GRUPO_FAMILIAR == "F" ? "FUNCIONAL" : "DISFUNCIONAL", _EstudioSocioe.SOCIOE_GPOFAMPRI.RELACION_INTRAFAMILIAR == "A" ? "ADECUADA" : "INADECUADA",
                                _EstudioSocioe.SOCIOE_GPOFAMPRI.PERSONAS_VIVEN_HOGAR.HasValue ? _EstudioSocioe.SOCIOE_GPOFAMPRI.PERSONAS_VIVEN_HOGAR.Value.ToString() : string.Empty,
                                _EstudioSocioe.SOCIOE_GPOFAMPRI.PERSONAS_LABORAN.HasValue ? _EstudioSocioe.SOCIOE_GPOFAMPRI.PERSONAS_LABORAN.Value.ToString() : string.Empty,
                                _EstudioSocioe.SOCIOE_GPOFAMPRI.INGRESO_MENSUAL.HasValue ? _EstudioSocioe.SOCIOE_GPOFAMPRI.INGRESO_MENSUAL.Value.ToString("c") : string.Empty,
                                _EstudioSocioe.SOCIOE_GPOFAMPRI.EGRESO_MENSUAL.HasValue ? _EstudioSocioe.SOCIOE_GPOFAMPRI.EGRESO_MENSUAL.Value.ToString("c") : string.Empty);

                        if (_EstudioSocioe.SOCIOE_GPOFAMSEC != null)
                            FamiliaSecundaria = string.Format("GRUPO FAMILIAR: {0},  RELACIÓN INTRAFAMILIAR: {1},  CUANTAS PERSONAS LABORAN: {2},  INGRESO MENSUAL FAM. : {3},  EGRESO MENSUAL FAM. : {4}",
                                _EstudioSocioe.SOCIOE_GPOFAMSEC.GRUPO_FAMILIAR == "F" ? "FUNCIONAL" : "DISFUNCIONAL",
                                _EstudioSocioe.SOCIOE_GPOFAMSEC.RELACION_INTRAFAMILIAR == "A" ? "ADECUADA" : "INADECUADA",
                                _EstudioSocioe.SOCIOE_GPOFAMSEC.PERSONAS_LABORAN.HasValue ? _EstudioSocioe.SOCIOE_GPOFAMSEC.PERSONAS_LABORAN.Value.ToString() : string.Empty,
                                _EstudioSocioe.SOCIOE_GPOFAMSEC.INGRESO_MENSUAL.HasValue ? _EstudioSocioe.SOCIOE_GPOFAMSEC.INGRESO_MENSUAL.Value.ToString("c") : string.Empty,
                                _EstudioSocioe.SOCIOE_GPOFAMSEC.EGRESO_MENSUAL.HasValue ? _EstudioSocioe.SOCIOE_GPOFAMSEC.EGRESO_MENSUAL.Value.ToString("c") : string.Empty).ToUpper();
                    };

                    #endregion
                    #region Visitas
                    var _VisitasIngreso = SelectIngreso.VISITA_AUTORIZADA;
                    if (_VisitasIngreso != null && _VisitasIngreso.Any())
                    {
                        if (new cAduanaIngreso().GetData(x => x.ID_ANIO == Entity.ID_ANIO && x.ID_CENTRO == Entity.ID_ANIO && x.ID_IMPUTADO == Entity.ID_IMPUTADO && x.ID_INGRESO == Entity.ID_INGRESO && x.ADUANA != null && x.ADUANA.ID_TIPO_PERSONA == (short)eTiposAduana.VISITA).Any())
                            IdRecibeVisitaSocioFamComun = "S";//si recibe visitas
                        else
                            IdRecibeVisitaSocioFamComun = "N";


                        short _Consecutivo = 0;
                        LstComunicaciones = new ObservableCollection<PFC_VI_COMUNICACION>();
                        foreach (var item in _VisitasIngreso)
                        {
                            if (item.ID_PARENTESCO.HasValue)
                            {
                                if (item.TIPO_REFERENCIA.DESCR.Contains("PADRE")) IsPadreVisitaCheckd = true;
                                if (item.TIPO_REFERENCIA.DESCR.Contains("MADRE")) IsMadreVisitaChecked = true;
                                if (item.TIPO_REFERENCIA.DESCR.Contains("ESPOS")) IsEsposoConcubinaChecked = true;
                                if (item.TIPO_REFERENCIA.DESCR.Contains("CONCUBIN")) IsEsposoConcubinaChecked = true;
                                if (item.TIPO_REFERENCIA.DESCR.Contains("HERMAN")) IsHermanosChecked = true;
                                if (item.TIPO_REFERENCIA.DESCR.Contains("HIJ")) IsHijosChecked = true;
                                if (item.TIPO_REFERENCIA.DESCR.Contains("OTR")) IsOtrosVisitaChecked = true;
                            };

                            LstComunicaciones.Add(new PFC_VI_COMUNICACION
                            {
                                ID_CONSEC = _Consecutivo++,
                                FRECUENCIA = item.PERSONA != null ? item.PERSONA.ADUANA != null ? FrecuenciaPorAduana(item.PERSONA) : "SIN DATO" : "SIN DATO",//item.TIPO_REFERENCIA != null ? item.TIPO_REFERENCIA.EMI_FACTORES_SOCIO_FAMILIARES.Any() ? item.TIPO_REFERENCIA.EMI_FACTORES_SOCIO_FAMILIARES.Where(x => x.id).FirstOrDefault().FRECUENCIA1.DESCR.Trim() : string.Empty : string.Empty,
                                ID_ANIO = SelectIngreso.ID_ANIO,
                                ID_CENTRO = SelectIngreso.ID_CENTRO,
                                ID_IMPUTADO = SelectIngreso.ID_IMPUTADO,
                                ID_INGRESO = SelectIngreso.ID_INGRESO,
                                NOMBRE = string.Format("{0} {1} {2}",
                                    item.PERSONA != null ? !string.IsNullOrEmpty(item.PERSONA.NOMBRE) ? item.PERSONA.NOMBRE.Trim() : string.Empty : string.Empty,
                                    item.PERSONA != null ? !string.IsNullOrEmpty(item.PERSONA.PATERNO) ? item.PERSONA.PATERNO.Trim() : string.Empty : string.Empty,
                                    item.PERSONA != null ? !string.IsNullOrEmpty(item.PERSONA.MATERNO) ? item.PERSONA.MATERNO.Trim() : string.Empty : string.Empty),
                                ID_TIPO_REFERENCIA = item.ID_PARENTESCO,
                                TELEFONO = item.PERSONA != null ? !string.IsNullOrEmpty(item.PERSONA.TELEFONO) ? item.PERSONA.TELEFONO.Trim() : string.Empty : string.Empty,
                                TIPO_REFERENCIA = item.TIPO_REFERENCIA
                            });
                        };
                    }

                    else
                        IdRecibeVisitaSocioFamComun = "N";//no recibe visitas
                    #endregion

                    FrecuenciaVisita = ProcesaFrecuencia(SelectIngreso);
                    EspecificarQuienVisita = EspecifiqueViaTelefonica = RazonNoRecibeVisitas = ApoyosRecibeExterior = MotivacionDictamenSocioEconomicoComun = IdComunicacionViaTelChecked = IdAvalMoralChecked =
                    PlanesInternoAlSerExternado = ConQuienVivirSerExternado = IdOfertaTrabajoChecked = EspecifiqueOfertaTrabajo = EspecifiqueAvalMoral = MotivacionDictamem = IsAdultoMayorParticipoEnProgramaEspecial = string.Empty;
                    IdDictamenSocioSocioFamComun = -1;
                    IdDictamenSocioFamComun = 0;
                    FechaEstudioSocioFamiliarComun = Fechas.GetFechaDateServer;

                    var municipio = new cMunicipio().Obtener(SelectIngreso.IMPUTADO.NACIMIENTO_ESTADO.Value, SelectIngreso.IMPUTADO.NACIMIENTO_MUNICIPIO.Value).FirstOrDefault();
                    FechaYLugarNacimiento = SelectIngreso.IMPUTADO != null ? string.Format("{0} {1}",
                            SelectIngreso.IMPUTADO.NACIMIENTO_MUNICIPIO.HasValue ? !string.IsNullOrEmpty(municipio.MUNICIPIO1) ? municipio.MUNICIPIO1.Trim() : string.Empty : string.Empty,
                            SelectIngreso.IMPUTADO.NACIMIENTO_FECHA.HasValue ? SelectIngreso.IMPUTADO.NACIMIENTO_FECHA.Value.ToString("dd/MM/yyyy") : string.Empty) : string.Empty;

                    NombreImputadoSocioComun = SelectIngreso.IMPUTADO != null ? string.Format("{0} {1} {2}",
                        !string.IsNullOrEmpty(SelectIngreso.IMPUTADO.NOMBRE) ? SelectIngreso.IMPUTADO.NOMBRE.Trim() : string.Empty,
                        !string.IsNullOrEmpty(SelectIngreso.IMPUTADO.PATERNO) ? SelectIngreso.IMPUTADO.PATERNO.Trim() : string.Empty,
                        !string.IsNullOrEmpty(SelectIngreso.IMPUTADO.MATERNO) ? SelectIngreso.IMPUTADO.MATERNO.Trim() : string.Empty) : string.Empty;

                    DomicilioAntesIngresarCentro = SelectIngreso != null ? string.Format("{0} {1} ",
                        !string.IsNullOrEmpty(SelectIngreso.DOMICILIO_CALLE) ? SelectIngreso.DOMICILIO_CALLE.Trim() : string.Empty,
                        SelectIngreso.DOMICILIO_NUM_EXT.HasValue ? SelectIngreso.DOMICILIO_NUM_EXT.Value.ToString() : string.Empty) : string.Empty;

                    EstadoCivilSocioComun = SelectIngreso != null ? SelectIngreso.ID_ESTADO_CIVIL.HasValue ? !string.IsNullOrEmpty(SelectIngreso.ESTADO_CIVIL.DESCR) ? SelectIngreso.ESTADO_CIVIL.DESCR.Trim() : string.Empty : string.Empty : string.Empty;
                    Telefono = SelectIngreso != null ? SelectIngreso.TELEFONO.HasValue ? new Converters().MascaraTelefono(SelectIngreso.TELEFONO.Value.ToString()) : string.Empty : string.Empty;

                    #region Programas
                    var GruposIngreso = SelectIngreso.GRUPO_PARTICIPANTE;
                    if (GruposIngreso != null && GruposIngreso.Any())
                    {
                        ListGruposSocioFamComun = new ObservableCollection<PFC_VI_GRUPO>();
                        ListFortalecimientoSocioFamComun = new ObservableCollection<PFC_VI_GRUPO>();

                        var _Refinados = GruposIngreso.Where(c => c.ESTATUS == (short)eEstatusGrupos.CANCELADO || c.ESTATUS == (short)eEstatusGrupos.CONCLUIDO);
                        foreach (var item in _Refinados)
                        {
                            if (item.ID_TIPO_PROGRAMA == (short)eTipoActividad.APOYO_ESPIRITUAL_Y_RELIGIOSO)
                            {
                                ListGruposSocioFamComun.Add(new PFC_VI_GRUPO
                                {
                                    CONGREGACION = string.Empty,
                                    ID_ACTIVIDAD = item.ID_ACTIVIDAD,
                                    ID_ANIO = SelectIngreso.ID_ANIO,
                                    ID_CENTRO = SelectIngreso.ID_CENTRO,
                                    ID_IMPUTADO = SelectIngreso.ID_IMPUTADO,
                                    ID_INGRESO = SelectIngreso.ID_INGRESO,
                                    ID_TIPO_PROGRAMA = item.ID_TIPO_PROGRAMA,
                                    TIPO_PROGRAMA = item.ACTIVIDAD != null ? item.ACTIVIDAD.TIPO_PROGRAMA : null,
                                    ACTIVIDAD = item.ACTIVIDAD,
                                    OBSERVACIONES = item.NOTA_TECNICA.Any() ? item.NOTA_TECNICA.FirstOrDefault().NOTA : string.Empty,
                                    PERIODO = item.GRUPO != null ? string.Format("{0} - {1}",
                                            item.GRUPO.FEC_INICIO.HasValue ? item.GRUPO.FEC_INICIO.Value.ToString("dd/MM/yyyy") : string.Empty,
                                            item.GRUPO.FEC_FIN.HasValue ? item.GRUPO.FEC_FIN.Value.ToString("dd/MM/yyyy") : string.Empty) : string.Empty
                                });
                            };

                            if (item.ID_TIPO_PROGRAMA == (short)eTipoActividad.NUCLEO_FAM)
                            {
                                ListFortalecimientoSocioFamComun.Add(new PFC_VI_GRUPO
                                {
                                    CONGREGACION = string.Empty,
                                    ID_ACTIVIDAD = item.ID_ACTIVIDAD,
                                    ID_ANIO = SelectIngreso.ID_ANIO,
                                    ID_CENTRO = SelectIngreso.ID_CENTRO,
                                    ID_IMPUTADO = SelectIngreso.ID_IMPUTADO,
                                    ID_INGRESO = SelectIngreso.ID_INGRESO,
                                    ID_TIPO_PROGRAMA = item.ID_TIPO_PROGRAMA,
                                    OBSERVACIONES = item.NOTA_TECNICA.Any() ? item.NOTA_TECNICA.FirstOrDefault().NOTA : string.Empty,
                                    PERIODO = item.GRUPO != null ? string.Format("{0} - {1}",
                                            item.GRUPO.FEC_INICIO.HasValue ? item.GRUPO.FEC_INICIO.Value.ToString("dd/MM/yyyy") : string.Empty,
                                            item.GRUPO.FEC_FIN.HasValue ? item.GRUPO.FEC_FIN.Value.ToString("dd/MM/yyyy") : string.Empty) : string.Empty,
                                    ACTIVIDAD = item.ACTIVIDAD,
                                    TIPO_PROGRAMA = item.ACTIVIDAD != null ? item.ACTIVIDAD.TIPO_PROGRAMA : null
                                });
                            };
                        };
                    };

                    #endregion
                }

                else
                {
                    LstComunicaciones = new ObservableCollection<PFC_VI_COMUNICACION>();
                    FamiliaPrimaria = _SocioFam.P21_FAMILIA_PRIMARIA;
                    FamiliaSecundaria = _SocioFam.P22_FAMILIA_SECUNDARIA;
                    EspecificarQuienVisita = _SocioFam.P4_OTROS_EPECIFICAR;
                    EspecifiqueViaTelefonica = _SocioFam.P5_NO_POR_QUE;
                    RazonNoRecibeVisitas = _SocioFam.P4_MOTIVO_NO_VISITA;
                    FrecuenciaVisita = _SocioFam.P4_FRECUENCIA;
                    ApoyosRecibeExterior = _SocioFam.P6_APOYO_EXTERIOR;
                    PlanesInternoAlSerExternado = _SocioFam.P7_PLANES_INTERNO;
                    ConQuienVivirSerExternado = _SocioFam.P7_VIVIRA;
                    EspecifiqueOfertaTrabajo = _SocioFam.P8_OFERTA_ESPECIFICAR;
                    EspecifiqueAvalMoral = _SocioFam.P9_AVAL_ESPECIFICAR;
                    MotivacionDictamenSocioEconomicoComun = _SocioFam.P11_MOTIVACION_DICTAMEN;
                    IsAdultoMayorParticipoEnProgramaEspecial = _SocioFam.P3_TERCERA_EDAD ?? string.Empty;
                    IdRecibeVisitaSocioFamComun = _SocioFam.P4_RECIBE_VISITA ?? string.Empty;
                    IdDictamenSocioFamComun = _SocioFam.P10_DICTAMEN ?? 0;
                    IdComunicacionViaTelChecked = _SocioFam.P5_COMUNICACION_TELEFONICA ?? string.Empty;
                    IdOfertaTrabajoChecked = _SocioFam.P8_OFERTA_TRABAJO ?? string.Empty;
                    IdAvalMoralChecked = _SocioFam.P9_AVAL_MORAL ?? string.Empty;
                    FechaEstudioSocioFamiliarComun = _SocioFam.ESTUDIO_FEC;
                    #region Checado de condensado de visitas
                    if (_SocioFam.P4_ESPOSOA == (short)eSINO.SI)
                        IsEsposoConcubinaChecked = true;
                    if (_SocioFam.P4_HERMANOS == (short)eSINO.SI)
                        IsHermanosChecked = true;
                    if (_SocioFam.P4_HIJOS == (short)eSINO.SI)
                        IsHijosChecked = true;
                    if (_SocioFam.P4_MADRE == (short)eSINO.SI)
                        IsMadreVisitaChecked = true;
                    if (_SocioFam.P4_OTROS == (short)eSINO.SI)
                        IsOtrosVisitaChecked = true;

                    if (_SocioFam.PFC_VI_COMUNICACION.Any())
                        foreach (var item in _SocioFam.PFC_VI_COMUNICACION)
                            LstComunicaciones.Add(item);

                    if (_SocioFam.PFC_VI_GRUPO != null && _SocioFam.PFC_VI_GRUPO.Any())
                    {
                        ListGruposSocioFamComun = new ObservableCollection<PFC_VI_GRUPO>();
                        ListFortalecimientoSocioFamComun = new ObservableCollection<PFC_VI_GRUPO>();
                        foreach (var item in _SocioFam.PFC_VI_GRUPO)
                        {
                            var _periodo = item.ACTIVIDAD != null ? item.ACTIVIDAD.GRUPO != null ? item.ACTIVIDAD.GRUPO.Any() ? item.ACTIVIDAD.GRUPO.FirstOrDefault(x => x.ID_TIPO_PROGRAMA == (short)eTipoActividad.APOYO_ESPIRITUAL_Y_RELIGIOSO && x.ID_ACTIVIDAD == item.ID_ACTIVIDAD && x.ID_TIPO_PROGRAMA == item.ID_TIPO_PROGRAMA && x.ID_CENTRO == item.ID_CENTRO) : null : null : null;

                            if (item.ID_TIPO_PROGRAMA == (short)eTipoActividad.APOYO_ESPIRITUAL_Y_RELIGIOSO)
                            {
                                ListGruposSocioFamComun.Add(new PFC_VI_GRUPO
                                {
                                    CONGREGACION = item.CONGREGACION,
                                    ID_ACTIVIDAD = item.ID_ACTIVIDAD,
                                    ID_ANIO = SelectIngreso.ID_ANIO,
                                    ID_CENTRO = SelectIngreso.ID_CENTRO,
                                    ID_IMPUTADO = SelectIngreso.ID_IMPUTADO,
                                    ID_INGRESO = SelectIngreso.ID_INGRESO,
                                    ID_TIPO_PROGRAMA = item.ID_TIPO_PROGRAMA,
                                    TIPO_PROGRAMA = item.ACTIVIDAD != null ? item.ACTIVIDAD.TIPO_PROGRAMA : null,
                                    ACTIVIDAD = item.ACTIVIDAD,
                                    OBSERVACIONES = item.OBSERVACIONES,
                                    PERIODO = _periodo != null ? string.Format("{0} - {1}",
                                    _periodo.FEC_INICIO.HasValue ? _periodo.FEC_INICIO.Value.ToString("dd/MM/yyyy") : string.Empty,
                                    _periodo.FEC_FIN.HasValue ? _periodo.FEC_FIN.Value.ToString("dd/MM/yyyy") : string.Empty) : string.Empty
                                });

                                continue;
                            };

                            if (item.ID_TIPO_PROGRAMA == (short)eTipoActividad.NUCLEO_FAM)
                            {
                                ListFortalecimientoSocioFamComun.Add(new PFC_VI_GRUPO
                                {
                                    CONGREGACION = string.Empty,
                                    ID_ACTIVIDAD = item.ID_ACTIVIDAD,
                                    ID_ANIO = SelectIngreso.ID_ANIO,
                                    ID_CENTRO = SelectIngreso.ID_CENTRO,
                                    ID_IMPUTADO = SelectIngreso.ID_IMPUTADO,
                                    ID_INGRESO = SelectIngreso.ID_INGRESO,
                                    ID_TIPO_PROGRAMA = item.ID_TIPO_PROGRAMA,
                                    OBSERVACIONES = item.OBSERVACIONES,
                                    PERIODO = _periodo != null ? string.Format("{0} - {1}",
                                    _periodo.FEC_INICIO.HasValue ? _periodo.FEC_INICIO.Value.ToString("dd/MM/yyyy") : string.Empty,
                                    _periodo.FEC_FIN.HasValue ? _periodo.FEC_FIN.Value.ToString("dd/MM/yyyy") : string.Empty) : string.Empty,
                                    ACTIVIDAD = item.ACTIVIDAD,
                                    TIPO_PROGRAMA = item.ACTIVIDAD != null ? item.ACTIVIDAD.TIPO_PROGRAMA : null
                                });

                                continue;
                            };
                        };
                    };

                    #endregion
                    var municipio = new cMunicipio().Obtener(SelectIngreso.IMPUTADO.NACIMIENTO_ESTADO.Value, SelectIngreso.IMPUTADO.NACIMIENTO_MUNICIPIO.Value).FirstOrDefault();
                    FechaYLugarNacimiento = SelectIngreso.IMPUTADO != null ? string.Format("{0} {1}",
                        SelectIngreso.IMPUTADO.NACIMIENTO_MUNICIPIO.HasValue ? !string.IsNullOrEmpty(municipio.MUNICIPIO1) ? municipio.MUNICIPIO1.Trim() : string.Empty : string.Empty,
                        SelectIngreso.IMPUTADO.NACIMIENTO_FECHA.HasValue ? SelectIngreso.IMPUTADO.NACIMIENTO_FECHA.Value.ToString("dd/MM/yyyy") : string.Empty) : string.Empty;

                    NombreImputadoSocioComun = SelectIngreso.IMPUTADO != null ? string.Format("{0} {1} {2}",
                        !string.IsNullOrEmpty(SelectIngreso.IMPUTADO.NOMBRE) ? SelectIngreso.IMPUTADO.NOMBRE.Trim() : string.Empty,
                        !string.IsNullOrEmpty(SelectIngreso.IMPUTADO.PATERNO) ? SelectIngreso.IMPUTADO.PATERNO.Trim() : string.Empty,
                        !string.IsNullOrEmpty(SelectIngreso.IMPUTADO.MATERNO) ? SelectIngreso.IMPUTADO.MATERNO.Trim() : string.Empty) : string.Empty;

                    DomicilioAntesIngresarCentro = SelectIngreso != null ? string.Format("{0} {1} ",
                        !string.IsNullOrEmpty(SelectIngreso.DOMICILIO_CALLE) ? SelectIngreso.DOMICILIO_CALLE.Trim() : string.Empty,
                        SelectIngreso.DOMICILIO_NUM_EXT.HasValue ? SelectIngreso.DOMICILIO_NUM_EXT.Value.ToString() : string.Empty) : string.Empty;

                    EstadoCivilSocioComun = SelectIngreso != null ? SelectIngreso.ID_ESTADO_CIVIL.HasValue ? !string.IsNullOrEmpty(SelectIngreso.ESTADO_CIVIL.DESCR) ? SelectIngreso.ESTADO_CIVIL.DESCR.Trim() : string.Empty : string.Empty : string.Empty;
                    Telefono = SelectIngreso != null ? SelectIngreso.TELEFONO.HasValue ? new Converters().MascaraTelefono(SelectIngreso.TELEFONO.Value.ToString()) : string.Empty : string.Empty;
                }
            }
            catch (Exception exc)
            {
                throw exc;
            }
        }
        #endregion
        #region Criminod Comun
        private void InfoCriminodComun(PERSONALIDAD Entity)
        {
            try
            {
                var _Crimin = new cPersonalidadEstudioCriminodiagnosticoComun().GetData(x => x.ID_INGRESO == Entity.ID_INGRESO && x.ID_IMPUTADO == Entity.ID_IMPUTADO && x.ID_CENTRO == Entity.ID_CENTRO && x.ID_ANIO == Entity.ID_ANIO && x.ID_ESTUDIO == Entity.ID_ESTUDIO).FirstOrDefault();
                if (_Crimin == null)
                {
                    IdPronosticoReincidencia = IdEgocentrismo = IdLabilidadAfectiva = IdAgresividad = IdIndiferenciaAfectiva = IdCapacidadCriminologicaActual = IdAdaptabilidadSocial = IdIndicePeligrosidadCriminologicaActual = -1;

                    VersionDelitoSegunInterno = AntecedentesEvolucionConductasParaSociales = string.Empty;
                    var _EmiIngreso = SelectIngreso.EMI_INGRESO;
                    if (_EmiIngreso != null && _EmiIngreso.Any())
                    {
                        var _UltimoEmi = _EmiIngreso.OrderByDescending(x => x.ID_EMI_CONS).FirstOrDefault();//ESTE ES EL ULTIMO EMI QUE SE LE HA PRACTICADO
                        if (_UltimoEmi != null)
                        {
                            foreach (var item in _EmiIngreso)
                            {
                                string dato = string.Empty;
                                dato = item.EMI != null ? item.EMI.EMI_SITUACION_JURIDICA != null ? !string.IsNullOrEmpty(item.EMI.EMI_SITUACION_JURIDICA.VERSION_DELITO_INTERNO) ? item.EMI.EMI_SITUACION_JURIDICA.VERSION_DELITO_INTERNO : string.Empty : string.Empty : string.Empty;

                                if (!string.IsNullOrEmpty(dato))
                                    VersionDelitoSegunInterno += dato.Length > 4000 ? dato.Substring(0, 4000) : dato;

                                AntecedentesEvolucionConductasParaSociales += string.Format(" {0} PANDILLERISMO , {1} VAGANCIA , {2} TATUAJES , {3} HOMOSEXUALIDAD , {4} PROSTITUCIÓN, {5} CONSUMO DE DROGAS , {6} CONDUCTAS DELICTIVAS ",
                                    item.EMI != null ? item.EMI.EMI_HPS != null ? item.EMI.EMI_HPS.ID_PANDILLA.HasValue ? "SI" : "NO" : string.Empty : string.Empty,
                                    item.EMI != null ? item.EMI.EMI_HPS != null ? !string.IsNullOrEmpty(item.EMI.EMI_HPS.VAGANCIA) ? item.EMI.EMI_HPS.VAGANCIA == "S" ? "SI" : "NO" : string.Empty : string.Empty : string.Empty,
                                    item.EMI != null ? item.EMI.EMI_TATUAJE != null ? item.EMI.EMI_TATUAJE.TOTAL_TATUAJES.HasValue ? item.EMI.EMI_TATUAJE.TOTAL_TATUAJES != decimal.Zero ? "SI" : "NO" : string.Empty : string.Empty : string.Empty,
                                    item.EMI != null ? item.EMI.EMI_HPS != null ? item.EMI.EMI_HPS.COMPORTAMIENTO_HOMO != null ? "SI" : "NO" : string.Empty : string.Empty,
                                    string.Format("{0}", item.EMI != null ? item.EMI.EMI_HPS.PROSTITUIA_HOMBRES == "S" ? "SI" : item.EMI.EMI_HPS.PROSTITUIA_MUJERES == "S" ? "SI" : "NO" : "NO"),
                                    item.EMI != null ? item.EMI.EMI_USO_DROGA != null ? item.EMI.EMI_USO_DROGA.Any() ? "SI" : "NO" : "NO" : "NO",
                                    item.EMI != null ? item.EMI.EMI_ANTECEDENTE_FAM_CON_DEL != null ? item.EMI.EMI_ANTECEDENTE_FAM_CON_DEL.Any() ? "SI" : "NO" : "NO" : "NO");
                            };


                            #region Informacion de los combobox en base al emi aplicado
                            var _EmiRealizado = _UltimoEmi.EMI;
                            if (_EmiRealizado != null)//Ya se le ha practicado un emi, es posible consultar los datos de los combobox
                            {
                                var _FactoresEMIAplicado = _EmiRealizado.EMI_FACTOR_CRIMINODIAGNOSTICO;
                                if (_FactoresEMIAplicado != null)
                                {
                                    IdEgocentrismo = ProcesaEMICapacidad(_FactoresEMIAplicado.EGOCENTRISMO);
                                    IdLabilidadAfectiva = ProcesaEMICapacidad(_FactoresEMIAplicado.LABILIDAD_AFECTIVA);//
                                    IdAgresividad = ProcesaEMICapacidad(_FactoresEMIAplicado.AGRESIVIDAD);//
                                    IdIndiferenciaAfectiva = ProcesaEMICapacidad(_FactoresEMIAplicado.INDIFERENCIA_AFECTIVA);//
                                    IdCapacidadCriminologicaActual = ProcesaEMICapacidad(_FactoresEMIAplicado.CAPACIDAD_CRIMINAL);//
                                    IdAdaptabilidadSocial = ProcesaEMICapacidad(_FactoresEMIAplicado.ADAPTABILIDAD_SOCIAL);
                                };
                            };
                            #endregion
                        }
                    };

                    CriminoGenesisEstudioCriminoFC = PorqueIntimidacionAntePenaImpuesta = MotivacionDictamenCriminodiagnosticoComun = SenialeProgramasDebeRemitirseInterno =
                    IdEncontrabaBajoInfluenciaDroga = IntimidacionAntePenaImpuesta = ReqTratamExtramurosCriminod = CualTratamRemitirCriminodiagnosticoComun = string.Empty;
                    IdClasificacionCriminologica = IdPronosticoReincidencia = IdIndicePeligrosidadCriminologicaActual = -1;
                    DictamenCriminod = 0;
                    FechaDictamenCrimino = Fechas.GetFechaDateServer;
                }

                else
                {
                    //IsEnabledDatosCriminod = false;
                    VersionDelitoSegunInterno = _Crimin.P1_VERSION_DELITO;
                    CriminoGenesisEstudioCriminoFC = _Crimin.P2_CRIMINOGENESIS;
                    AntecedentesEvolucionConductasParaSociales = _Crimin.P3_CONDUCTA_ANTISOCIAL;
                    PorqueIntimidacionAntePenaImpuesta = _Crimin.P5_PORQUE;
                    MotivacionDictamenCriminodiagnosticoComun = _Crimin.P10_MOTIVACION_DICTAMEN;
                    SenialeProgramasDebeRemitirseInterno = _Crimin.P11_PROGRAMAS_REMITIRSE;
                    IdEncontrabaBajoInfluenciaDroga = _Crimin.P1_DROGADO ?? string.Empty;
                    IdClasificacionCriminologica = _Crimin.P4_CLASIFICACION_CRIMINOLOGICA ?? -1;
                    IntimidacionAntePenaImpuesta = _Crimin.P5_INTIMIDACION ?? string.Empty;
                    IdEgocentrismo = _Crimin.P6A_EGOCENTRICO ?? -1;
                    IdLabilidadAfectiva = _Crimin.P6B_LIABILIDAD_EFECTIVA ?? -1;
                    IdAgresividad = _Crimin.P6C_AGRESIVIDAD ?? -1;
                    IdIndiferenciaAfectiva = _Crimin.P6D_INDIFERENCIA_AFECTIVA ?? -1;
                    IdCapacidadCriminologicaActual = _Crimin.P6_CAPACIDAD_CRIMINAL ?? -1;
                    IdAdaptabilidadSocial = _Crimin.P7_ADAPTACION_SOCIAL ?? -1;
                    IdIndicePeligrosidadCriminologicaActual = _Crimin.P8_INDICE_PELIGROSIDAD ?? -1;
                    IdPronosticoReincidencia = _Crimin.P9_PRONOSTICO_REINCIDENCIA ?? -1;
                    DictamenCriminod = _Crimin.P10_DICTAMEN_REINSERCION ?? 0;
                    ReqTratamExtramurosCriminod = _Crimin.P12_TRATAMIENTO_EXTRAMUROS ?? string.Empty;
                    FechaDictamenCrimino = _Crimin.ESTUDIO_FEC;
                    CualTratamRemitirCriminodiagnosticoComun = _Crimin.P12_CUAL;
                    if (_Crimin.P1_ALCOHOL == (short)eSINO.SI)
                        IsAlcoholChecked = true;
                    if (_Crimin.P1_DROGRA_ILEGAL == (short)eSINO.SI)
                        IsDrogasIlegalesChecked = true;
                    if (_Crimin.P1_OTRA == (short)eSINO.SI)
                        IsOtraChecked = true;
                }
            }
            catch (Exception exc)
            {
                throw exc;
            }
        }
        #endregion
        #region Psicologico Comun
        private void PsicoInfoComun(PERSONALIDAD Entity)
        {
            try
            {
                var EstudioPsico = new cPersonalidadEstudioPsicologicoComun().GetData(x => x.ID_INGRESO == Entity.ID_INGRESO && x.ID_IMPUTADO == Entity.ID_IMPUTADO && x.ID_CENTRO == Entity.ID_CENTRO && x.ID_ANIO == Entity.ID_ANIO && x.ID_ESTUDIO == Entity.ID_ESTUDIO).FirstOrDefault();
                if (EstudioPsico == null)
                {
                    //ValidacionesEstudioPsicologicoFueroComun();
                    #region Grupos
                    LstComplement = new ObservableCollection<PFC_IV_PROGRAMA>();
                    LstProgramasPsicologico = new ObservableCollection<PFC_IV_PROGRAMA>();
                    LstProgModifConduc = new ObservableCollection<PFC_IV_PROGRAMA>();
                    LstTalleresOrient = new ObservableCollection<PFC_IV_PROGRAMA>();
                    var _GrupoParticipante = SelectIngreso.GRUPO_PARTICIPANTE;
                    if (_GrupoParticipante != null && _GrupoParticipante.Any())//grupos concluidos o registrados del imputado
                    {
                        var _ColeccionRefinada = _GrupoParticipante.Where(x => x.ESTATUS == (short)eEstatusGrupos.CANCELADO || x.ESTATUS == (short)eEstatusGrupos.CONCLUIDO).AsQueryable();
                        foreach (var item in _ColeccionRefinada)
                        {//AGREGAR EL GUION CUANDO EL CAMPO SEA MAS AMPLIO
                            if (item.ID_TIPO_PROGRAMA == (short)eGrupos.PROGRAMAS_DESHABITUAMIENTO)
                            {
                                LstProgramasPsicologico.Add(new PFC_IV_PROGRAMA
                                {
                                    CONCLUYO = item.NOTA_TECNICA != null ? item.NOTA_TECNICA.Any() ? item.NOTA_TECNICA.FirstOrDefault().ESTATUS == (short)eAprobado.APROBADO ? "S" : "N" : "N" : "N",
                                    DURACION = string.Format("{0} - {1}",
                                        item.GRUPO != null ? item.GRUPO.FEC_INICIO.HasValue ? item.GRUPO.FEC_INICIO.Value.ToString("dd/MM/yyyy") : string.Empty : string.Empty,
                                        item.GRUPO != null ? item.GRUPO.FEC_FIN.HasValue ? item.GRUPO.FEC_FIN.Value.ToString("dd/MM/yyyy") : string.Empty : string.Empty),
                                    ID_ACTIVIDAD = item.ID_ACTIVIDAD,
                                    ID_ANIO = SelectIngreso.ID_ANIO,
                                    ID_CENTRO = SelectIngreso.ID_CENTRO,
                                    ID_IMPUTADO = SelectIngreso.ID_IMPUTADO,
                                    ID_INGRESO = SelectIngreso.ID_INGRESO,
                                    ID_TIPO_PROGRAMA = item.ID_TIPO_PROGRAMA,
                                    OBSERVACION = item.NOTA_TECNICA.Any() ? item.NOTA_TECNICA.FirstOrDefault().NOTA : string.Empty,
                                    TIPO_PROGRAMA = item.ACTIVIDAD != null ? item.ACTIVIDAD.TIPO_PROGRAMA : null,
                                    ACTIVIDAD = item.ACTIVIDAD,
                                    ID_CONSEC = item.ID_CONSEC
                                });

                                continue;//NO es necesario continuar recorriendo el arreglo, termina en cuanto lo encuentro
                            };

                            if (item.ID_TIPO_PROGRAMA == (short)eGrupos.PROGRAMAS_MODIFIC_CONDUCTA)
                            {
                                LstProgModifConduc.Add(new PFC_IV_PROGRAMA
                                {
                                    CONCLUYO = item.NOTA_TECNICA != null ? item.NOTA_TECNICA.Any() ? item.NOTA_TECNICA.FirstOrDefault().ESTATUS == (short)eAprobado.APROBADO ? "S" : "N" : "N" : "N",
                                    DURACION = string.Format("{0} - {1}",
                                        item.GRUPO != null ? item.GRUPO.FEC_INICIO.HasValue ? item.GRUPO.FEC_INICIO.Value.ToString("dd/MM/yyyy") : string.Empty : string.Empty,
                                        item.GRUPO != null ? item.GRUPO.FEC_FIN.HasValue ? item.GRUPO.FEC_FIN.Value.ToString("dd/MM/yyyy") : string.Empty : string.Empty),
                                    ID_ACTIVIDAD = item.ID_ACTIVIDAD,
                                    ID_ANIO = SelectIngreso.ID_ANIO,
                                    ID_CENTRO = SelectIngreso.ID_CENTRO,
                                    ID_IMPUTADO = SelectIngreso.ID_IMPUTADO,
                                    ID_INGRESO = SelectIngreso.ID_INGRESO,
                                    ID_TIPO_PROGRAMA = item.ID_TIPO_PROGRAMA,
                                    OBSERVACION = item.NOTA_TECNICA.Any() ? item.NOTA_TECNICA.FirstOrDefault().NOTA : string.Empty,
                                    TIPO_PROGRAMA = item.ACTIVIDAD != null ? item.ACTIVIDAD.TIPO_PROGRAMA : null,
                                    ACTIVIDAD = item.ACTIVIDAD,
                                    ID_CONSEC = item.ID_CONSEC
                                });

                                continue;//NO es necesario continuar recorriendo el arreglo, termina en cuanto lo encuentro
                            };

                            if (item.ID_TIPO_PROGRAMA == (short)eGrupos.COMPLEMENTARIO)
                            {
                                LstComplement.Add(new PFC_IV_PROGRAMA
                                {
                                    CONCLUYO = item.NOTA_TECNICA != null ? item.NOTA_TECNICA.Any() ? item.NOTA_TECNICA.FirstOrDefault().ESTATUS == (short)eAprobado.APROBADO ? "S" : "N" : "N" : "N",
                                    DURACION = string.Format("{0} - {1}",
                                        item.GRUPO != null ? item.GRUPO.FEC_INICIO.HasValue ? item.GRUPO.FEC_INICIO.Value.ToString("dd/MM/yyyy") : string.Empty : string.Empty,
                                        item.GRUPO != null ? item.GRUPO.FEC_FIN.HasValue ? item.GRUPO.FEC_FIN.Value.ToString("dd/MM/yyyy") : string.Empty : string.Empty),
                                    ID_ACTIVIDAD = item.ID_ACTIVIDAD,
                                    ID_ANIO = SelectIngreso.ID_ANIO,
                                    ID_CENTRO = SelectIngreso.ID_CENTRO,
                                    ID_IMPUTADO = SelectIngreso.ID_IMPUTADO,
                                    ID_INGRESO = SelectIngreso.ID_INGRESO,
                                    ID_TIPO_PROGRAMA = item.ID_TIPO_PROGRAMA,
                                    OBSERVACION = item.NOTA_TECNICA.Any() ? item.NOTA_TECNICA.FirstOrDefault().NOTA : string.Empty,
                                    TIPO_PROGRAMA = item.ACTIVIDAD != null ? item.ACTIVIDAD.TIPO_PROGRAMA : null,
                                    ACTIVIDAD = item.ACTIVIDAD,
                                    ID_CONSEC = item.ID_CONSEC //Se amarra un campo caracteristico para poder editar este registro
                                });

                                continue;//NO es necesario continuar recorriendo el arreglo, termina en cuanto lo encuentro
                            };

                            if (item.ID_TIPO_PROGRAMA == (short)eGrupos.TALLERES_ORIENTACION)
                            {
                                LstTalleresOrient.Add(new PFC_IV_PROGRAMA
                                     {
                                         CONCLUYO = item.NOTA_TECNICA != null ? item.NOTA_TECNICA.Any() ? item.NOTA_TECNICA.FirstOrDefault().ESTATUS == (short)eAprobado.APROBADO ? "S" : "N" : "N" : "N",
                                         DURACION = string.Format("{0} - {1}",
                                        item.GRUPO != null ? item.GRUPO.FEC_INICIO.HasValue ? item.GRUPO.FEC_INICIO.Value.ToString("dd/MM/yyyy") : string.Empty : string.Empty,
                                        item.GRUPO != null ? item.GRUPO.FEC_FIN.HasValue ? item.GRUPO.FEC_FIN.Value.ToString("dd/MM/yyyy") : string.Empty : string.Empty),
                                         ID_ACTIVIDAD = item.ID_ACTIVIDAD,
                                         ID_ANIO = SelectIngreso.ID_ANIO,
                                         ID_CENTRO = SelectIngreso.ID_CENTRO,
                                         ID_IMPUTADO = SelectIngreso.ID_IMPUTADO,
                                         ID_INGRESO = SelectIngreso.ID_INGRESO,
                                         ID_TIPO_PROGRAMA = item.ID_TIPO_PROGRAMA,
                                         OBSERVACION = item.NOTA_TECNICA.Any() ? item.NOTA_TECNICA.FirstOrDefault().NOTA : string.Empty,
                                         TIPO_PROGRAMA = item.ACTIVIDAD != null ? item.ACTIVIDAD.TIPO_PROGRAMA : null,
                                         ACTIVIDAD = item.ACTIVIDAD,
                                         ID_CONSEC = item.ID_CONSEC //Se amarra un campo caracteristico para poder editar este registro
                                     });

                                continue;
                            }
                        };
                    };
                    #endregion

                    DescripcionPrincipalesRazgosIngreso = ExamenMental = CondicionesGralesInterno = IntegracionDinamicaPersonalidadActual = RasgosPersonalidadRelaciondosComisionDelito =
                    MotivacionDictamenPsicologicoComun = CasoNegativoEstudioPsicologicoComun = EspecifiqueOtroTest = IdReqExtraMurosPsicologicoComun = CualTratamientoExtraMurosPsicologicoComun = string.Empty;
                    IdNivelIntelectual = IdDisfuncionNeurologica = -1;
                    IdDictamenPsicologicoComun = 0;
                    FechaDictamenPsicologicoComun = Fechas.GetFechaDateServer;
                }
                else
                {
                    PrepararListas();
                    LstProgramasPsicologico = new ObservableCollection<PFC_IV_PROGRAMA>();
                    LstProgModifConduc = new ObservableCollection<PFC_IV_PROGRAMA>();
                    LstComplement = new ObservableCollection<PFC_IV_PROGRAMA>();
                    LstTalleresOrient = new ObservableCollection<PFC_IV_PROGRAMA>();
                    CondicionesGralesInterno = EstudioPsico.P1_CONDICIONES_GRALES;
                    ExamenMental = EstudioPsico.P2_EXAMEN_MENTAL;
                    DescripcionPrincipalesRazgosIngreso = EstudioPsico.P3_PRINCIPALES_RASGOS;
                    EspecifiqueOtroTest = EstudioPsico.P4_OTRA_MENCIONAR;
                    IntegracionDinamicaPersonalidadActual = EstudioPsico.P6_INTEGRACION;
                    IdNivelIntelectual = EstudioPsico.P51_NIVEL_INTELECTUAL ?? -1;
                    IdDisfuncionNeurologica = EstudioPsico.P52_DISFUNCION_NEUROLOGICA ?? -1;
                    RasgosPersonalidadRelaciondosComisionDelito = EstudioPsico.P8_RASGOS_PERSONALIDAD;
                    IdDictamenPsicologicoComun = EstudioPsico.P9_DICTAMEN_REINSERCION ?? 0;
                    MotivacionDictamenPsicologicoComun = EstudioPsico.P10_MOTIVACION_DICTAMEN;
                    CasoNegativoEstudioPsicologicoComun = EstudioPsico.P11_CASO_NEGATIVO;
                    IdReqExtraMurosPsicologicoComun = EstudioPsico.P12_REQUIERE_TRATAMIENTO ?? string.Empty;
                    CualTratamientoExtraMurosPsicologicoComun = EstudioPsico.P12_CUAL;
                    FechaDictamenPsicologicoComun = EstudioPsico.ESTUDIO_FEC;
                    if (EstudioPsico.PFC_IV_PROGRAMA.Any())
                    {
                        foreach (var item in EstudioPsico.PFC_IV_PROGRAMA)
                        {
                            if (item.ID_TIPO_PROGRAMA == (short)eGrupos.PROGRAMAS_DESHABITUAMIENTO)
                            {
                                LstProgramasPsicologico.Add(item);
                                continue;
                            };

                            if (item.ID_TIPO_PROGRAMA == (short)eGrupos.PROGRAMAS_MODIFIC_CONDUCTA)
                            {
                                LstProgModifConduc.Add(item);
                                continue;
                            };

                            if (item.ID_TIPO_PROGRAMA == (short)eGrupos.COMPLEMENTARIO)
                            {
                                LstComplement.Add(item);
                                continue;
                            };

                            if (item.ID_TIPO_PROGRAMA == (short)eGrupos.TALLERES_ORIENTACION)
                            {
                                LstTalleresOrient.Add(item);
                                continue;
                            }
                        };
                    };
                    #region Checado de Pruebas Aplicadas
                    if (EstudioPsico.P4_TEST_GUALTICO == (short)eSINO.SI)
                        IsTestLaurettaBenderChecked = true;
                    if (EstudioPsico.P4_TEST_MATRICES == (short)eSINO.SI)
                        IsTestMatricesProgresivasRavenChecked = true;
                    if (EstudioPsico.P4_TEST_HTP == (short)eSINO.SI)
                        IsTestCasaArbolPersonaChecked = true;
                    if (EstudioPsico.P4_INVENTARIO_MULTIFASICO == (short)eSINO.SI)
                        IsTestMinesotaChecked = true;
                    if (EstudioPsico.P4_OTRAS == (short)eSINO.SI)
                        IsTestOtrosChecked = true;
                    #endregion
                }
            }
            catch (Exception exc)
            {
                throw exc;
            }
        }
        #endregion
        #region Psiq Comun
        private void PsiquiatricoComunInfo(PERSONALIDAD Entity)
        {
            try
            {
                var EstudioPsiq = new cPersonalidadEstudioPsiquiatricoComun().GetData(x => x.ID_INGRESO == Entity.ID_INGRESO && x.ID_IMPUTADO == Entity.ID_IMPUTADO && x.ID_CENTRO == Entity.ID_CENTRO && x.ID_ANIO == Entity.ID_ANIO && x.ID_ESTUDIO == Entity.ID_ESTUDIO).FirstOrDefault();
                if (EstudioPsiq == null)
                {
                    AspectoFisico = ConductaMotora = Habla = Actitud = EstadoAnimo = ExpresionAfectiva = Adecuacion = Alucinaciones = Ilusiones = Despersonalizacion = Desrealizacion = CursoPensamiento =
                    ContinuidadPensamiento = ContenidoPensamiento = PensamientoAbstracto = Concentracion = BajaToleranciaFrustr = ExpresionDesadaptativa = Adecuada = Orientacion = Memoria = CapacidadJuicio =
                    Introspeccion = Fiabilidad = ImpresionDiagnosticaPsiquiatricoComun = string.Empty;
                    DictamenPsiqComun = 0;
                    FecDictamenPsiqComun = Fechas.GetFechaDateServer;
                }
                else
                {
                    AspectoFisico = EstudioPsiq.A1_ASPECTO_FISICO;
                    ConductaMotora = EstudioPsiq.B1_CONDUCTA_MOTORA;
                    Habla = EstudioPsiq.C1_HABLA;
                    Actitud = EstudioPsiq.D1_ACTITUD;
                    EstadoAnimo = EstudioPsiq.A2_ESTADO_ANIMO;
                    ExpresionAfectiva = EstudioPsiq.B2_EXPRESION_AFECTIVA;
                    Adecuacion = EstudioPsiq.C2_ADECUACION;
                    Alucinaciones = EstudioPsiq.A3_ALUCINACIONES;
                    Ilusiones = EstudioPsiq.B3_ILUSIONES;
                    Despersonalizacion = EstudioPsiq.C3_DESPERSONALIZACION;
                    Desrealizacion = EstudioPsiq.D3_DESREALIZACION;
                    CursoPensamiento = EstudioPsiq.A4_CURSO;
                    ContinuidadPensamiento = EstudioPsiq.B4_CONTINUIDAD;
                    ContenidoPensamiento = EstudioPsiq.C4_CONTENIDO;
                    PensamientoAbstracto = EstudioPsiq.D4_ABASTRACTO;
                    Concentracion = EstudioPsiq.E4_CONCENTRACION;
                    BajaToleranciaFrustr = EstudioPsiq.A7_BAJA_TOLERANCIA;
                    ExpresionDesadaptativa = EstudioPsiq.B7_EXPRESION;
                    Adecuada = EstudioPsiq.C7_ADECUADA;
                    Orientacion = EstudioPsiq.P5_ORIENTACION;
                    Memoria = EstudioPsiq.P6_MEMORIA;
                    CapacidadJuicio = EstudioPsiq.P8_CAPACIDAD_JUICIO;
                    Introspeccion = EstudioPsiq.P9_INTROSPECCION;
                    Fiabilidad = EstudioPsiq.P10_FIANILIDAD;
                    ImpresionDiagnosticaPsiquiatricoComun = EstudioPsiq.P11_IMPRESION;
                    DictamenPsiqComun = EstudioPsiq.P12_DICTAMEN_PSIQUIATRICO;
                    FecDictamenPsiqComun = EstudioPsiq.ESTUDIO_FEC;
                }
            }
            catch (Exception exc)
            {
                throw exc;
            }
        }
        #endregion
        #region Medico Comun
        private void MedicoInfoComun(PERSONALIDAD Entity)
        {
            try
            {
                var _EstudioMedicoComun = new cPersonalidadEstudioMedicoComun().GetData(x => x.ID_INGRESO == Entity.ID_INGRESO && x.ID_IMPUTADO == Entity.ID_IMPUTADO && x.ID_CENTRO == Entity.ID_CENTRO && x.ID_ANIO == Entity.ID_ANIO && x.ID_ESTUDIO == Entity.ID_ESTUDIO).FirstOrDefault();
                if (_EstudioMedicoComun == null)
                {
                    FechaEstudioMedicoComun = Fechas.GetFechaDateServer;
                    #region Historial Clinico
                    AntecedentesHeredoFamiliares = AntecedentesPersonalesNoPatologicos = AntedecentesConsumoToxicosEstadoActual = DescripcionTatuajesCicatricesMalformaciones = string.Empty;
                    var _HistoriaClinicaByImputado = new cHistoriaClinica().GetData(x => x.ID_ANIO == SelectIngreso.ID_ANIO && x.ID_CENTRO == GlobalVar.gCentro && x.ID_IMPUTADO == SelectIngreso.ID_IMPUTADO && x.ID_INGRESO == SelectIngreso.ID_INGRESO);
                    if (_HistoriaClinicaByImputado.Any())
                    {
                        var _Historia = _HistoriaClinicaByImputado.FirstOrDefault();
                        if (_Historia.HISTORIA_CLINICA_FAMILIAR != null && _Historia.HISTORIA_CLINICA_FAMILIAR.Any())
                        {
                            foreach (var item in _Historia.HISTORIA_CLINICA_FAMILIAR)
                            {
                                string _Caracteres = string.Empty;
                                _Caracteres = string.Format("{0} , {1} ALERGIAS,  {2} CA,  {3} CARDIACOS, {4} DIABETES,  {5} EPILEPSIA,  {6} HIPERTENSO,  {7} MENTALES,  {8} TB. \n",
                                    item.ID_TIPO_REFERENCIA.HasValue ? !string.IsNullOrEmpty(item.TIPO_REFERENCIA.DESCR) ? item.TIPO_REFERENCIA.DESCR.Trim() : string.Empty : string.Empty,
                                    item.AHF_ALERGIAS == "S" ? "SI" : "NO", item.AHF_CA == "S" ? "SI" : "NO", item.AHF_CARDIACOS == "S" ? "SI" : "NO", item.AHF_DIABETES == "S" ? "SI" : "NO",
                                    item.AHF_EPILEPSIA == "S" ? "SI" : "NO", item.AHF_HIPERTENSIVO == "S" ? "SI" : "NO", item.AHF_MENTALES == "S" ? "SI" : "NO", item.AHF_TB == "S" ? "SI" : "NO"
                                    );

                                if (AntecedentesHeredoFamiliares.Length + _Caracteres.Length < 500)
                                    AntecedentesHeredoFamiliares += _Caracteres;
                            };

                            string NoPatos = string.Format("{0} ALCOHOLISMO, ALIMENTACIÓN: {1}, HABITACIÓN: {2}, NACIMIENTO: {3},  {4} TABAQUISMO,  {5} TOXICOMANÍAS",
                                _Historia.APNP_ALCOHOLISMO == "S" ? "SI" : "NO",
                                _Historia.APNP_ALIMENTACION,
                                _Historia.APNP_HABITACION,
                                _Historia.APNP_NACIMIENTO,
                                _Historia.APNP_TABAQUISMO == "S" ? "SI" : "NO",
                                _Historia.APNP_TOXICOMANIAS == "S" ? "SI" : "NO");

                            if (AntecedentesPersonalesNoPatologicos.Length + NoPatos.Length < 500)
                                AntecedentesPersonalesNoPatologicos += NoPatos;
                        };
                    };

                    #endregion
                    #region EMI INGRESO
                    var _EmiIngreso = SelectIngreso.EMI_INGRESO;
                    if (_EmiIngreso != null && _EmiIngreso.Any())
                    {
                        var _UltimoEMI = _EmiIngreso.OrderByDescending(x => x.ID_EMI_CONS).FirstOrDefault();
                        if (_UltimoEMI != null)
                            if (_UltimoEMI.EMI != null)
                                if (_UltimoEMI.EMI.EMI_USO_DROGA.Any())
                                    foreach (var item in _UltimoEMI.EMI.EMI_USO_DROGA)
                                        AntedecentesConsumoToxicosEstadoActual += string.Format("{0} , ", item.ID_DROGA != decimal.Zero ? !string.IsNullOrEmpty(item.DROGA.DESCR) ? item.DROGA.DESCR.Trim() : string.Empty : string.Empty);
                    };

                    #endregion
                    #region Senias Particulares
                    var _SeniasParticulares = SelectIngreso.IMPUTADO.SENAS_PARTICULARES;//new cSenasParticulares().GetData(x => x.ID_IMPUTADO == SelectIngreso.ID_IMPUTADO && x.ID_INGRESO == SelectIngreso.ID_INGRESO && x.ID_ANIO == SelectIngreso.ID_ANIO);
                    if (_SeniasParticulares != null && _SeniasParticulares.Any())
                        foreach (var item in _SeniasParticulares)
                            DescripcionTatuajesCicatricesMalformaciones += string.Format("{0} \n", !string.IsNullOrEmpty(item.SIGNIFICADO) ? item.SIGNIFICADO.Trim() : string.Empty);

                    #endregion
                    EdadInterno = SelectIngreso.IMPUTADO != null ? new Fechas().CalculaEdad(SelectIngreso.IMPUTADO.NACIMIENTO_FECHA).ToString() : string.Empty;
                    SexoInterno = SelectIngreso.IMPUTADO != null ? SelectIngreso.IMPUTADO.SEXO == "M" ? "MASCULINO" : "FEMENINO" : string.Empty;
                    AntecedentesPatologicos = DescipcionPadecimientoActual = Arterial1 = Arterial2 = TensionArterialGenerico = TemperaturaGenerico = PulsoGenerico = RespiracionGenerico = PesoGenerico = EstaturaGenerico = ImpresionDiagnosticaEstudioMedicoComun = string.Empty;
                    IdDictamenMedicoComun = 0;
                }
                else
                {
                    AntecedentesHeredoFamiliares = _EstudioMedicoComun.P2_HEREDO_FAMILIARES;
                    AntecedentesPersonalesNoPatologicos = _EstudioMedicoComun.P3_ANTPER_NOPATO;
                    AntedecentesConsumoToxicosEstadoActual = _EstudioMedicoComun.P31_CONSUMO_TOXICO;
                    DescripcionTatuajesCicatricesMalformaciones = _EstudioMedicoComun.P32_TATUAJES_CICATRICES;
                    AntecedentesPatologicos = _EstudioMedicoComun.P4_PATOLOGICOS;
                    DescipcionPadecimientoActual = _EstudioMedicoComun.P5_PADECIMIENTOS;
                    if (!string.IsNullOrEmpty(_EstudioMedicoComun.SIGNOS_TA) ? _EstudioMedicoComun.SIGNOS_TA.Trim().Contains("/") : false)
                    {
                        string dato1 = _EstudioMedicoComun.SIGNOS_TA.Split('/')[0];
                        string dato2 = _EstudioMedicoComun.SIGNOS_TA.Split('/')[1];
                        Arterial1 = dato1.Trim();
                        Arterial2 = dato2.Trim();
                    }
                    TemperaturaGenerico = _EstudioMedicoComun.SIGNOS_TEMPERATURA;
                    PulsoGenerico = _EstudioMedicoComun.SIGNOS_PULSO;
                    RespiracionGenerico = _EstudioMedicoComun.SIGNOS_RESPIRACION;
                    PesoGenerico = _EstudioMedicoComun.SIGNOS_PESO;
                    EstaturaGenerico = _EstudioMedicoComun.SIGNOS_ESTATURA;
                    ImpresionDiagnosticaEstudioMedicoComun = _EstudioMedicoComun.P7_IMPRESION_DIAGNOSTICA;
                    IdDictamenMedicoComun = _EstudioMedicoComun.P8_DICTAMEN_MEDICO ?? 0;
                    FechaEstudioMedicoComun = _EstudioMedicoComun.ESTUDIO_FEC;
                    EdadInterno = SelectIngreso.IMPUTADO != null ? new Fechas().CalculaEdad(SelectIngreso.IMPUTADO.NACIMIENTO_FECHA).ToString() : string.Empty;
                    SexoInterno = SelectIngreso.IMPUTADO != null ? SelectIngreso.IMPUTADO.SEXO == "M" ? "MASCULINO" : "FEMENINO" : string.Empty;
                }
            }
            catch (Exception exc)
            {
                throw exc;
            }
        }
        #endregion

        #region TRANSACCION AISLADA

        private void Mensaje(string resultado, string name, StaticSourcesViewModel.enumTipoMensaje _tipo)
        {
            try
            {
                StaticSourcesViewModel.Mensaje(name, resultado, _tipo);
            }
            catch (Exception ex)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al dar mensaje", ex);
            }
        }

        private string ProcesaFrecuencia(INGRESO Entity)
        {
            try
            {
                var _VisitanteIngreso = new cAduanaIngreso().GetData(x => x.ID_ANIO == Entity.ID_ANIO && x.ID_CENTRO == Entity.ID_CENTRO && x.ID_IMPUTADO == Entity.ID_IMPUTADO && x.ID_INGRESO == Entity.ID_INGRESO && x.ADUANA != null && x.ADUANA.ID_TIPO_PERSONA == (short)eTiposAduana.VISITA); //new cVisitanteIngreso().ObtenerXImputado(Entity.ID_CENTRO, Entity.ID_ANIO, Entity.ID_IMPUTADO, Entity.ID_INGRESO);
                if (_VisitanteIngreso.Any())//SOLO VISITAS NORMALES, NO LEGALES NI ACTUARIOS NI DEMAS
                {
                    var _VisitasPorAnio = _VisitanteIngreso.GroupBy(x => x.ENTRADA_FEC.Value.Year);//VISITAS POR AñO
                    if (_VisitasPorAnio.Any())
                    {
                        var _DetalleMeses = _VisitasPorAnio.OrderByDescending(x => x.Count()).FirstOrDefault();// TOMA EL MES CON VAS VISITAS RECIBIDAS
                        if (_DetalleMeses != null)
                        {
                            if (_DetalleMeses.Count() >= (int)eFrecuencia.SEMANAL)
                                return "SEMANAL";
                            if (_DetalleMeses.Count() == (int)eFrecuencia.QUINCENAL)
                                return "QUINCENAL";
                            if (_DetalleMeses.Count() == (int)eFrecuencia.MENSUAL)
                                return "MENSUAL";
                            if (_DetalleMeses.Count() == (int)eFrecuencia.ANUAL)
                                return "ANUAL";
                        };
                    };
                };

                return "SIN DATO";
            }
            catch (Exception exc)
            {
                throw exc;
            }
        }

        private void GuardaAislado(string _Actual)
        {
            try
            {
                short _dato = -1;//SIRVE COMO INDICADOR DEL PROCESO
                //if (IsOtraTarea)//NO NECESARIAMENTE QUIERE GUARDAR
                //    return;

                var _EstudioPdre = new cEstudioPersonalidad().GetData(x => x.ID_INGRESO == SelectIngreso.ID_INGRESO && x.ID_IMPUTADO == SelectIngreso.ID_IMPUTADO && x.ID_CENTRO == SelectIngreso.ID_CENTRO && x.ID_ANIO == SelectIngreso.ID_ANIO).OrderByDescending(x => x.ID_ESTUDIO).FirstOrDefault();
                if (_EstudioPdre == null)
                {
                    (new Dialogos()).ConfirmacionDialogo("Validacion", "Verifique el estado del estudio de personalidad");
                    return;
                };

                if (string.IsNullOrEmpty(_Actual))
                    return;

                if (_Actual == "TabEstudioMedicoFC")
                    _dato = new cRealizacionEstudiosPersonalidad().GuardaMedicoComun(EstudioMedicoTransaccionIndividual(_EstudioPdre));
                if (_Actual == "TabEstudioPsiqFC")
                    _dato = new cRealizacionEstudiosPersonalidad().GuardaPsiquiatricoComun(EstudioPsiquiatricoTransaccionIndividual(_EstudioPdre));
                if (_Actual == "TabEstudioPsicFC")
                    _dato = new cRealizacionEstudiosPersonalidad().GuardaPsicologicoComun(EstudioPsicologicoTransaccionIndividual(_EstudioPdre));
                if (_Actual == "TabCriminoDFC")
                    _dato = new cRealizacionEstudiosPersonalidad().GuardaCriminoDiagnoticoComun(EstudioCriminodTransaccionIndividual(_EstudioPdre));
                if (_Actual == "TabEstudioSocioFamFC")
                    _dato = new cRealizacionEstudiosPersonalidad().GuardaSocioiFamiliarComun(EstudioSocioFamiliarTransaccionIndividual(_EstudioPdre));
                if (_Actual == "TabEstudioEducCultDepFC")
                    _dato = new cRealizacionEstudiosPersonalidad().GuardarEducativoComun(EstudioEducativoTransaccionIndividual(_EstudioPdre));
                if (_Actual == "TabEstudioCapacitacionTrabajoPenitFC")
                    _dato = new cRealizacionEstudiosPersonalidad().GuardarCapacitacionComun(EstudioTrabajoTransaccionIndividual(_EstudioPdre));
                if (_Actual == "TabSeguriddCustodiaFC")
                    _dato = new cRealizacionEstudiosPersonalidad().GuardaSeguridadComun(EstudioSeguridadTransaccionIndividual(_EstudioPdre));
                if (_Actual == "TabEstudioMedicoFF")
                    _dato = new cRealizacionEstudiosPersonalidad().GuardaMedicoFederalAislado(GuardadoEstudioMedicoFederal());
                if (_Actual == "TabEstudioPsicoFF")
                    _dato = new cRealizacionEstudiosPersonalidad().GuardaPsicologicoFederalAislado(GuardadoPsicologicoFueroFederal());
                if (_Actual == "TabEstudioTrabajoSocialFF")
                    _dato = new cRealizacionEstudiosPersonalidad().GuardarEstudioTrabajoSocialAislado(GuardadoTrabajoSocialFueroFederal());
                if (_Actual == "TabActivProductCapacitFF")
                    _dato = new cRealizacionEstudiosPersonalidad().GuardarEducativoFederalAislado(GuardadoCapacitacionFueroFederal());
                if (_Actual == "TabActEducCultDepRecCivFF")
                    _dato = new cRealizacionEstudiosPersonalidad().GuardarEstudioActividadesFederalAislado(GuardadoActividadesEducFueroFederal());
                if (_Actual == "TabVigilanciaFF")
                    _dato = new cRealizacionEstudiosPersonalidad().GuardaVigilanciaFederalAislado(GuardadoVigilanciaFueroFederal());
                if (_Actual == "TabEstudioCriminologico")
                    _dato = new cRealizacionEstudiosPersonalidad().GuardaCriminologicoFederaAislado(GuardadoCriminologicoFueroFederal());

                if (_dato == 0)
                    Mensaje("Error", "Surgió un error al guardar la información", StaticSourcesViewModel.enumTipoMensaje.MENSAJE_ERROR);
                else if (_dato == 1)
                    Mensaje("Exito", "Se registró la información con exito", StaticSourcesViewModel.enumTipoMensaje.MENSAJE_CORRECTO);
                else if (_dato == 2)
                    Mensaje("Validación", "Uno de los campos ingresados excede el tamaño maximo permitido", StaticSourcesViewModel.enumTipoMensaje.MESNAJE_ADVERTENCIA);
            }
            catch (Exception exc)
            {
                throw exc;
            }
        }

        #endregion
    }
}