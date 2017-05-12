using SSP.Servidor;
using SSP.Modelo;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Collections.ObjectModel;
using System.Data.SqlClient;
using System.Data.Objects.SqlClient;
using System.Linq.Expressions;
using System.Transactions;
using System.Data;


namespace SSP.Controlador.Catalogo.Justicia
{
    public class cHistoriaClinica : EntityManagerServer<HISTORIA_CLINICA>
    {
        /// <summary>
        /// constructor de la clase
        /// </summary>
        public cHistoriaClinica()
        { }

        /// <summary>
        /// metodo que se conectara a la base de datos para regresar un registro 
        /// </summary>
        /// <param name="Id">valor de la llave primaria para obtener registro</param>
        /// <returns>objeto de tipo "HISTORIA_CLINICA"</returns>
        public List<HISTORIA_CLINICA> ObtenerXImputado(short anio, short centro, int imputado)
        {
            var Resultado = new List<HISTORIA_CLINICA>();
            try
            {
                Resultado = GetData().Where(w => w.ID_ANIO == anio && w.ID_CENTRO == centro && w.ID_IMPUTADO == imputado).ToList();
            }
            catch (Exception ex)
            {
                throw new ApplicationException(ex.Message);
            }
            return Resultado;
        }
        public List<HISTORIA_CLINICA> ObtenerXIngreso(short anio, short centro, int imputado, short ingreso)
        {
            try
            {
                return GetData().Where(w => w.ID_ANIO == anio && w.ID_CENTRO == centro && w.ID_IMPUTADO == imputado && w.ID_INGRESO == ingreso).ToList();
            }
            catch (Exception ex)
            {
                throw new ApplicationException(ex.Message);
            }
        }

        /// <summary>
        /// metodo que se conecta a la base de datos para insertar un registro nuevo
        /// </summary>
        /// <param name="Entity">objeto de tipo "INGRESO_DELITO" con valores a insertar</param>
        public HISTORIA_CLINICA InsertarTransaccion(HISTORIA_CLINICA historia, DateTime hoy)
        {
            try
            {
                using (System.Transactions.TransactionScope transaccion = new System.Transactions.TransactionScope(System.Transactions.TransactionScopeOption.RequiresNew, new System.Transactions.TransactionOptions() { IsolationLevel = System.Transactions.IsolationLevel.ReadCommitted }))
                {
                    var HistoriaAnterior = Context.HISTORIA_CLINICA.Where(x => x.ID_ANIO == historia.ID_ANIO && x.ID_CENTRO == historia.ID_CENTRO && x.ID_IMPUTADO == historia.ID_IMPUTADO && x.ID_INGRESO == historia.ID_INGRESO).FirstOrDefault();
                    if (HistoriaAnterior == null)
                    {
                        var ConsecutivoHC = GetIDProceso<short>("HISTORIA_CLINICA", "ID_CONSEC", string.Format("ID_CENTRO={0} AND ID_ANIO={1} AND ID_IMPUTADO={2} AND ID_INGRESO={3}", historia.ID_CENTRO, historia.ID_ANIO, historia.ID_IMPUTADO, historia.ID_INGRESO));
                        HistoriaAnterior = new HISTORIA_CLINICA()
                        {
                            AHF_HERMANOS_F = historia.AHF_HERMANOS_F,
                            AHF_HERMANOS_M = historia.AHF_HERMANOS_M,
                            AHF_NOMBRE = historia.AHF_NOMBRE,
                            APNP_ALCOHOLISMO = historia.APNP_ALCOHOLISMO,
                            APNP_ALIMENTACION = historia.APNP_ALIMENTACION,
                            APNP_HABITACION = historia.APNP_HABITACION,
                            APNP_NACIMIENTO = historia.APNP_NACIMIENTO,
                            APNP_TABAQUISMO = historia.APNP_TABAQUISMO,
                            APNP_TOXICOMANIAS = historia.APNP_TOXICOMANIAS,
                            APP_MEDICAMENTOS_ACTIVOS = historia.APP_MEDICAMENTOS_ACTIVOS,
                            CARDIOVASCULAR = historia.CARDIOVASCULAR,
                            CEDULA_PROFESIONAL = historia.CEDULA_PROFESIONAL,
                            CONCLUSIONES = historia.CONCLUSIONES,
                            CP_CAPACIDAD_TRATAMIENTO = historia.CP_CAPACIDAD_TRATAMIENTO,
                            CP_ETAPA_EVOLUTIVA = historia.CP_ETAPA_EVOLUTIVA,
                            CP_GRADO_AFECTACION = historia.CP_GRADO_AFECTACION,
                            CP_GRAVEDAD = historia.CP_GRAVEDAD,
                            CP_NIVEL_ATENCION = historia.CP_NIVEL_ATENCION,
                            CP_PRONOSTICO = historia.CP_PRONOSTICO,
                            CP_REMISION = historia.CP_REMISION,
                            DIGESTIVO = historia.DIGESTIVO,
                            DIRECTO = historia.DIRECTO,
                            DOCTOR = historia.DOCTOR,
                            EF_ABDOMEN = historia.EF_ABDOMEN,
                            EF_CABEZA = historia.EF_CABEZA,
                            EF_CUELLO = historia.EF_CUELLO,
                            EF_ESTATURA = historia.EF_ESTATURA,
                            EF_EXTREMIDADES = historia.EF_EXTREMIDADES,
                            EF_GENITALES = historia.EF_GENITALES,
                            EF_IMPRESION_DIAGNOSTICA = historia.EF_IMPRESION_DIAGNOSTICA,
                            EF_PESO = historia.EF_PESO,
                            EF_PRESION_ARTERIAL = historia.EF_PRESION_ARTERIAL,
                            EF_PULSO = historia.EF_PULSO,
                            ESTATUS = historia.ESTATUS,
                            EF_RECTO = historia.EF_RECTO,
                            EF_RESPIRACION = historia.EF_RESPIRACION,
                            EF_RESULTADO_ANALISIS = historia.EF_RESULTADO_ANALISIS,
                            EF_RESULTADO_GABINETE = historia.EF_RESULTADO_GABINETE,
                            EF_TEMPERATURA = historia.EF_TEMPERATURA,
                            EF_TORAX = historia.EF_TORAX,
                            ENDOCRINO = historia.ENDOCRINO,
                            ESTUDIO_FEC = historia.ESTUDIO_FEC,
                            GENITAL_HOMBRES = historia.GENITAL_HOMBRES,
                            GENITAL_MUJERES = historia.GENITAL_MUJERES,
                            HEMATICO_LINFACTICO = historia.HEMATICO_LINFACTICO,
                            ID_ANIO = historia.ID_ANIO,
                            ID_CENTRO = historia.ID_CENTRO,
                            ID_CONSEC = ConsecutivoHC,
                            ID_IMPUTADO = historia.ID_IMPUTADO,
                            ID_INGRESO = historia.ID_INGRESO,
                            ID_RESPONSABLE = historia.ID_RESPONSABLE,
                            M65_ALTERACION_AUDITIVA = historia.M65_ALTERACION_AUDITIVA,
                            M65_ALTERACION_OLFACION = historia.M65_ALTERACION_OLFACION,
                            M65_ALTERACION_VISOMOTRIZ = historia.M65_ALTERACION_VISOMOTRIZ,
                            M65_ALTERACION_VISUAL = historia.M65_ALTERACION_VISUAL,
                            M65_OTROS = historia.M65_OTROS,
                            M65_PARTICIPACION = historia.M65_PARTICIPACION,
                            M65_TRAS_ANIMO = historia.M65_TRAS_ANIMO,
                            M65_TRAS_ATENCION = historia.M65_TRAS_ATENCION,
                            M65_TRAS_COMPRENSION = historia.M65_TRAS_COMPRENSION,
                            M65_TRAS_DEMENCIAL = historia.M65_TRAS_DEMENCIAL,
                            M65_TRAS_MEMORIA = historia.M65_TRAS_MEMORIA,
                            M65_TRAS_ORIENTACION = historia.M65_TRAS_ORIENTACION,
                            MU_MENARQUIA = historia.MU_MENARQUIA,
                            MUSCULO_ESQUELETICO = historia.MUSCULO_ESQUELETICO,
                            NERVIOSO = historia.NERVIOSO,
                            PADECIMIENTO_ACTUAL = historia.PADECIMIENTO_ACTUAL,
                            PAG3_NOMBRE = historia.PAG3_NOMBRE,
                            PAG4_NOMBRE = historia.PAG4_NOMBRE,
                            PAG5_NOMBRE = historia.PAG5_NOMBRE,
                            PIEL_ANEXOS = historia.PIEL_ANEXOS,
                            POR_CONSTANCIAS_DOCUMENTALES = historia.POR_CONSTANCIAS_DOCUMENTALES,
                            RES_NOMBRE = historia.RES_NOMBRE,
                            RESPIRATORIO = historia.RESPIRATORIO,
                            SINTOMAS_GENERALES = historia.SINTOMAS_GENERALES,
                            TERAPEUTICA_PREVIA = historia.TERAPEUTICA_PREVIA,
                            ULTIMA_FEC = historia.ULTIMA_FEC,
                            URINARIO = historia.URINARIO,
                            APNP_ALCOHOLISMO_OBSERV = historia.APNP_ALCOHOLISMO_OBSERV,
                            APNP_TABAQUISMO_OBSERV = historia.APNP_TABAQUISMO_OBSERV,
                            APNP_TOXICOMANIAS_OBSERV = historia.APNP_TOXICOMANIAS_OBSERV,
                            EF_DESCRIPCION = historia.EF_DESCRIPCION
                        };

                        short ConsecutivoPato = GetIDProceso<short>("HISTORIA_CLINICA_PATOLOGICOS", "ID_NOPATOLOGICO", string.Format("ID_CENTRO={0} AND ID_ANIO={1} AND ID_IMPUTADO={2} AND ID_INGRESO={3}", historia.ID_CENTRO, historia.ID_ANIO, historia.ID_IMPUTADO, historia.ID_INGRESO));

                        if (historia.HISTORIA_CLINICA_PATOLOGICOS != null && historia.HISTORIA_CLINICA_PATOLOGICOS.Any())
                            foreach (var item in historia.HISTORIA_CLINICA_PATOLOGICOS)
                            {
                                var NvoPato = new HISTORIA_CLINICA_PATOLOGICOS()
                                {
                                    ID_ANIO = item.ID_ANIO,
                                    ID_CENTRO = item.ID_CENTRO,
                                    ID_CONSEC = HistoriaAnterior.ID_CONSEC,
                                    ID_IMPUTADO = item.ID_IMPUTADO,
                                    ID_INGRESO = item.ID_INGRESO,
                                    ID_PATOLOGICO = item.ID_PATOLOGICO,
                                    MOMENTO_DETECCION = item.MOMENTO_DETECCION,
                                    OTROS_DESCRIPCION = item.OTROS_DESCRIPCION,
                                    RECUPERADO = item.RECUPERADO,
                                    REGISTRO_FEC = item.REGISTRO_FEC,
                                    OBSERVACIONES = item.OBSERVACIONES,
                                    ID_NOPATOLOGICO = ConsecutivoPato
                                };

                                HistoriaAnterior.HISTORIA_CLINICA_PATOLOGICOS.Add(NvoPato);
                                ConsecutivoPato++;
                            };

                        var ConsecutivoDocumentos = GetIDProceso<short>("HISTORIA_CLINICA_DOCUMENTO", "ID_NODOCTO", string.Format("ID_CENTRO={0} AND ID_ANIO={1} AND ID_IMPUTADO={2} AND ID_INGRESO={3}", historia.ID_CENTRO, historia.ID_ANIO, historia.ID_IMPUTADO, historia.ID_INGRESO));
                        if (historia.HISTORIA_CLINICA_DOCUMENTO != null && historia.HISTORIA_CLINICA_DOCUMENTO.Any())
                            foreach (var item in historia.HISTORIA_CLINICA_DOCUMENTO)
                            {
                                var Docto = new HISTORIA_CLINICA_DOCUMENTO();
                                Docto.DOCUMENTO = item.DOCUMENTO;//archivo
                                Docto.FISICO = item.FISICO;//complemento
                                Docto.ID_DOCTO = item.ID_DOCTO;//foranea
                                Docto.ID_ANIO = item.ID_ANIO;//llave primaria
                                Docto.ID_CENTRO = item.ID_CENTRO;//llave primaria
                                Docto.ID_IMPUTADO = item.ID_IMPUTADO;//llave primaria
                                Docto.ID_INGRESO = HistoriaAnterior.ID_INGRESO;//llave primaria
                                Docto.ID_CONSEC = HistoriaAnterior.ID_CONSEC;//llave primaria
                                Docto.ID_NODOCTO = ConsecutivoDocumentos;
                                Docto.ID_FORMATO = item.ID_FORMATO;
                                HistoriaAnterior.HISTORIA_CLINICA_DOCUMENTO.Add(Docto);
                                ConsecutivoDocumentos++;
                            };

                        short _id_consec_familiares = GetIDProceso<short>("HISTORIA_CLINICA_FAMILIAR", "ID_CONSEC", string.Format("ID_CENTRO={0} AND ID_ANIO={1} AND ID_IMPUTADO={2} AND ID_INGRESO={3}", historia.ID_CENTRO, historia.ID_ANIO, historia.ID_IMPUTADO, historia.ID_INGRESO));
                        foreach (var item in historia.HISTORIA_CLINICA_FAMILIAR)
                        {
                            var dato = new HISTORIA_CLINICA_FAMILIAR();
                            dato.AHF_ALERGIAS = item.AHF_ALERGIAS;
                            dato.AHF_CA = item.AHF_CA;
                            dato.AHF_CARDIACOS = item.AHF_CARDIACOS;
                            dato.AHF_DIABETES = item.AHF_DIABETES;
                            dato.AHF_EDAD = item.AHF_EDAD;
                            dato.AHF_EPILEPSIA = item.AHF_EPILEPSIA;
                            dato.AHF_FALLECIMIENTO_CAUSA = item.AHF_FALLECIMIENTO_CAUSA;
                            dato.AHF_FALLECIMIENTO_FEC = item.AHF_FALLECIMIENTO_FEC;
                            dato.AHF_HIPERTENSIVO = item.AHF_HIPERTENSIVO;
                            dato.AHF_MENTALES = item.AHF_MENTALES;
                            dato.AHF_NOMBRE = item.AHF_NOMBRE;
                            dato.AHF_SANO = item.AHF_SANO;
                            dato.AHF_TB = item.AHF_TB;
                            dato.AHF_VIVE = item.AHF_VIVE;
                            dato.ID_TIPO_REFERENCIA = item.ID_TIPO_REFERENCIA;


                            //llave primaria
                            dato.ID_ANIO = item.ID_ANIO;
                            dato.ID_CENTRO = item.ID_CENTRO;
                            dato.ID_CONSEC = HistoriaAnterior.ID_CONSEC;
                            dato.ID_IMPUTADO = item.ID_IMPUTADO;
                            dato.ID_INGRESO = item.ID_INGRESO;
                            dato.ID_FAMILIAR = _id_consec_familiares;
                            HistoriaAnterior.HISTORIA_CLINICA_FAMILIAR.Add(dato);
                            _id_consec_familiares++;
                        };

                        short _id_consec_grupos_vulnerables = GetIDProceso<short>("GRUPO_VULNERABLE", "ID_GRUPO_CONSEC", string.Format("ID_CENTRO={0} AND ID_ANIO={1} AND ID_IMPUTADO={2} AND ID_INGRESO={3}", historia.ID_CENTRO, historia.ID_ANIO, historia.ID_IMPUTADO, historia.ID_INGRESO));
                        string _DetalleGruposVulnerables = string.Empty;
                        if (historia.GRUPO_VULNERABLE != null && historia.GRUPO_VULNERABLE.Any())
                        {
                            //SE GUARDAN LOS NUEVOS GRUPOS VULNERABLES
                            foreach (var item in historia.GRUPO_VULNERABLE)
                            {
                                var NvoGrupoV = new GRUPO_VULNERABLE()
                                {
                                    BAJA_FEC = item.BAJA_FEC,
                                    ID_ANIO = item.ID_ANIO,
                                    REGISTRO_FEC = item.REGISTRO_FEC,
                                    MOMENTO_DETECCION = item.MOMENTO_DETECCION,
                                    ID_SECTOR_CLAS = item.ID_SECTOR_CLAS,
                                    ID_CENTRO = item.ID_CENTRO,
                                    ID_CONSEC = HistoriaAnterior.ID_CONSEC,
                                    ID_GRUPO_CONSEC = _id_consec_grupos_vulnerables,
                                    ID_IMPUTADO = item.ID_IMPUTADO,
                                    ID_INGRESO = item.ID_INGRESO
                                };

                                HistoriaAnterior.GRUPO_VULNERABLE.Add(NvoGrupoV);
                                _id_consec_grupos_vulnerables++;

                                var _DescripcionGrupo = Context.SECTOR_CLASIFICACION.Where(x => x.ID_SECTOR_CLAS == item.ID_SECTOR_CLAS).FirstOrDefault();
                                if (_DescripcionGrupo != null)
                                    _DetalleGruposVulnerables += string.Format("- {0} \n", !string.IsNullOrEmpty(_DescripcionGrupo.POBLACION) ? _DescripcionGrupo.POBLACION.Trim() : string.Empty);
                            };

                            //SE GENERA LA NOTIFICACION 
                            DateTime _fecha = GetFechaServerDate();
                            var _MensajeConsiderado = Context.MENSAJE_TIPO.FirstOrDefault(x => x.ID_MEN_TIPO == 95);
                            string _Nombre = string.Empty;
                            var _id_mensaje = GetIDProceso<int>("MENSAJE", "ID_MENSAJE", "1=1");
                            var NombreImputado = Context.IMPUTADO.First(w => w.ID_ANIO == HistoriaAnterior.ID_ANIO && w.ID_CENTRO == HistoriaAnterior.ID_CENTRO && w.ID_IMPUTADO == HistoriaAnterior.ID_IMPUTADO);
                            if (NombreImputado != null)
                                _Nombre = string.Format("{0} {1} {2}", !string.IsNullOrEmpty(NombreImputado.NOMBRE) ? NombreImputado.NOMBRE.Trim() : string.Empty,
                                    !string.IsNullOrEmpty(NombreImputado.PATERNO) ? NombreImputado.PATERNO.Trim() : string.Empty, !string.IsNullOrEmpty(NombreImputado.MATERNO) ? NombreImputado.MATERNO.Trim() : string.Empty);

                            var NvoMensaje = new MENSAJE()
                            {
                                CONTENIDO = _MensajeConsiderado.CONTENIDO + " " + _Nombre + " HA INGRESADO A LOS SIGUIENTES GRUPOS VULNERABLES: \n" + _DetalleGruposVulnerables,
                                ENCABEZADO = _MensajeConsiderado.ENCABEZADO,
                                ID_MEN_TIPO = _MensajeConsiderado.ID_MEN_TIPO,
                                ID_MENSAJE = _id_mensaje,
                                REGISTRO_FEC = _fecha,
                                ID_CENTRO = GlobalVariables.gCentro
                            };

                            Context.MENSAJE.Add(NvoMensaje);
                        }

                        var _SexoImputado = Context.INGRESO.FirstOrDefault(x => x.ID_INGRESO == historia.ID_INGRESO && x.ID_IMPUTADO == historia.ID_IMPUTADO && x.ID_ANIO == historia.ID_ANIO && x.ID_CENTRO == historia.ID_CENTRO);
                        if (_SexoImputado.IMPUTADO != null)
                            if (_SexoImputado.IMPUTADO.SEXO == "F")
                            {
                                short _id_consec_gineco = GetIDProceso<short>("HISTORIA_CLINICA_GINECO_OBSTRE", "ID_CONSEC", string.Format("ID_CENTRO={0} AND ID_ANIO={1} AND ID_IMPUTADO={2} AND ID_INGRESO={3}", historia.ID_CENTRO, historia.ID_ANIO, historia.ID_IMPUTADO, historia.ID_INGRESO));
                                if (historia.HISTORIA_CLINICA_GINECO_OBSTRE != null && historia.HISTORIA_CLINICA_GINECO_OBSTRE.Any())
                                {
                                    foreach (var item in historia.HISTORIA_CLINICA_GINECO_OBSTRE)
                                    {
                                        var _Gineco = new HISTORIA_CLINICA_GINECO_OBSTRE()
                                        {
                                            ABORTO = item.ABORTO,
                                            ABORTO_MODIFICADO = item.ABORTO_MODIFICADO,
                                            ANIOS_RITMO = item.ANIOS_RITMO,
                                            ANIOS_RITMO_MODIFICADO = item.ANIOS_RITMO_MODIFICADO,
                                            CESAREA = item.CESAREA,
                                            CESAREA_MODIFICADO = item.CESAREA_MODIFICADO,
                                            CONTROL_PRENATAL = item.CONTROL_PRENATAL,
                                            CONTROL_PRENATAL_MODIFICADO = item.CONTROL_PRENATAL_MODIFICADO,
                                            DEFORMACION = item.DEFORMACION,
                                            DEFORMACION_MODIFICADO = item.DEFORMACION_MODIFICADO,
                                            EMBARAZO = item.EMBARAZO,
                                            EMBARAZO_MODIFICADO = item.EMBARAZO_MODIFICADO,
                                            FECHA_PROBABLE_PARTO = item.FECHA_PROBABLE_PARTO,
                                            FECHA_PROBABLE_PARTO_MOD = item.FECHA_PROBABLE_PARTO_MOD,
                                            FUENTE = item.FUENTE,
                                            ID_CONTROL_PRENATAL = item.ID_CONTROL_PRENATAL.HasValue ? item.ID_CONTROL_PRENATAL != -1 ? item.ID_CONTROL_PRENATAL : new short?() : new short?(),
                                            ID_ANIO = historia.ID_ANIO,
                                            ID_CENTRO = historia.ID_CENTRO,
                                            ID_IMPUTADO = historia.ID_IMPUTADO,
                                            ID_INGRESO = historia.ID_INGRESO,
                                            MOMENTO_REGISTRO = item.MOMENTO_REGISTRO,
                                            PARTO = item.PARTO,
                                            PARTO_MODIFICADO = item.PARTO_MODIFICADO,
                                            REGISTRO_FEC = item.REGISTRO_FEC,
                                            ULTIMA_MENS_MODIFICADO = item.ULTIMA_MENS_MODIFICADO,
                                            ULTIMA_MENSTRUACION_FEC = item.ULTIMA_MENSTRUACION_FEC,
                                            ID_CONSEC = _id_consec_gineco,
                                            ID_GINECO = 1
                                        };

                                        HistoriaAnterior.HISTORIA_CLINICA_GINECO_OBSTRE.Add(_Gineco);
                                    }
                                };
                            };


                        var _CitaMedica = Context.ATENCION_CITA.Where(x => x.ID_ANIO == historia.ID_ANIO && x.ID_CENTRO == historia.ID_CENTRO && x.ID_IMPUTADO == historia.ID_IMPUTADO && x.ID_TIPO_ATENCION == 1 && x.ID_TIPO_SERVICIO == 4);
                        if (_CitaMedica.Any())
                            foreach (var item in _CitaMedica)
                            {
                                item.ESTATUS = "S";
                                Context.Entry(item).Property(x => x.ESTATUS).IsModified = true;
                            };

                        Context.HISTORIA_CLINICA.Add(HistoriaAnterior);
                    }


                    else
                    {
                        HistoriaAnterior.ESTUDIO_FEC = GetData().First(f => f.ID_ANIO == historia.ID_ANIO && f.ID_CENTRO == historia.ID_CENTRO && f.ID_IMPUTADO == historia.ID_IMPUTADO && f.ID_INGRESO == historia.ID_INGRESO).ESTUDIO_FEC;
                        HistoriaAnterior.AHF_HERMANOS_F = historia.AHF_HERMANOS_F;
                        HistoriaAnterior.AHF_HERMANOS_M = historia.AHF_HERMANOS_M;
                        HistoriaAnterior.AHF_NOMBRE = historia.AHF_NOMBRE;
                        HistoriaAnterior.APNP_ALCOHOLISMO = historia.APNP_ALCOHOLISMO;
                        HistoriaAnterior.APNP_ALIMENTACION = historia.APNP_ALIMENTACION;
                        HistoriaAnterior.APNP_HABITACION = historia.APNP_HABITACION;
                        HistoriaAnterior.APNP_NACIMIENTO = historia.APNP_NACIMIENTO;
                        HistoriaAnterior.APNP_TABAQUISMO = historia.APNP_TABAQUISMO;
                        HistoriaAnterior.APNP_TOXICOMANIAS = historia.APNP_TOXICOMANIAS;
                        HistoriaAnterior.APP_MEDICAMENTOS_ACTIVOS = historia.APP_MEDICAMENTOS_ACTIVOS;
                        HistoriaAnterior.CARDIOVASCULAR = historia.CARDIOVASCULAR;
                        HistoriaAnterior.CEDULA_PROFESIONAL = historia.CEDULA_PROFESIONAL;
                        HistoriaAnterior.CONCLUSIONES = historia.CONCLUSIONES;
                        HistoriaAnterior.CP_CAPACIDAD_TRATAMIENTO = historia.CP_CAPACIDAD_TRATAMIENTO;
                        HistoriaAnterior.CP_ETAPA_EVOLUTIVA = historia.CP_ETAPA_EVOLUTIVA;
                        HistoriaAnterior.CP_GRADO_AFECTACION = historia.CP_GRADO_AFECTACION;
                        HistoriaAnterior.CP_GRAVEDAD = historia.CP_GRAVEDAD;
                        HistoriaAnterior.CP_NIVEL_ATENCION = historia.CP_NIVEL_ATENCION;
                        HistoriaAnterior.CP_PRONOSTICO = historia.CP_PRONOSTICO;
                        HistoriaAnterior.CP_REMISION = historia.CP_REMISION;
                        HistoriaAnterior.DIGESTIVO = historia.DIGESTIVO;
                        HistoriaAnterior.DIRECTO = historia.DIRECTO;
                        HistoriaAnterior.EF_DESCRIPCION = historia.EF_DESCRIPCION;
                        HistoriaAnterior.DOCTOR = historia.DOCTOR;
                        HistoriaAnterior.EF_ABDOMEN = historia.EF_ABDOMEN;
                        HistoriaAnterior.EF_CABEZA = historia.EF_CABEZA;
                        HistoriaAnterior.EF_CUELLO = historia.EF_CUELLO;
                        HistoriaAnterior.EF_ESTATURA = historia.EF_ESTATURA;
                        HistoriaAnterior.EF_EXTREMIDADES = historia.EF_EXTREMIDADES;
                        HistoriaAnterior.EF_GENITALES = historia.EF_GENITALES;
                        HistoriaAnterior.EF_IMPRESION_DIAGNOSTICA = historia.EF_IMPRESION_DIAGNOSTICA;
                        HistoriaAnterior.EF_PESO = historia.EF_PESO;
                        HistoriaAnterior.EF_PRESION_ARTERIAL = historia.EF_PRESION_ARTERIAL;
                        HistoriaAnterior.EF_PULSO = historia.EF_PULSO;
                        HistoriaAnterior.EF_RECTO = historia.EF_RECTO;
                        HistoriaAnterior.EF_RESPIRACION = historia.EF_RESPIRACION;
                        HistoriaAnterior.EF_RESULTADO_ANALISIS = historia.EF_RESULTADO_ANALISIS;
                        HistoriaAnterior.EF_RESULTADO_GABINETE = historia.EF_RESULTADO_GABINETE;
                        HistoriaAnterior.EF_TEMPERATURA = historia.EF_TEMPERATURA;
                        HistoriaAnterior.EF_TORAX = historia.EF_TORAX;
                        HistoriaAnterior.ENDOCRINO = historia.ENDOCRINO;
                        HistoriaAnterior.ESTATUS = historia.ESTATUS;
                        HistoriaAnterior.GENITAL_HOMBRES = historia.GENITAL_HOMBRES;
                        HistoriaAnterior.GENITAL_MUJERES = historia.GENITAL_MUJERES;
                        HistoriaAnterior.HEMATICO_LINFACTICO = historia.HEMATICO_LINFACTICO;
                        HistoriaAnterior.ID_ANIO = historia.ID_ANIO;
                        HistoriaAnterior.ID_CENTRO = historia.ID_CENTRO;
                        HistoriaAnterior.ID_IMPUTADO = historia.ID_IMPUTADO;
                        HistoriaAnterior.ID_INGRESO = historia.ID_INGRESO;
                        HistoriaAnterior.ID_RESPONSABLE = historia.ID_RESPONSABLE;
                        HistoriaAnterior.M65_ALTERACION_AUDITIVA = historia.M65_ALTERACION_AUDITIVA;
                        HistoriaAnterior.M65_ALTERACION_OLFACION = historia.M65_ALTERACION_OLFACION;
                        HistoriaAnterior.M65_ALTERACION_VISOMOTRIZ = historia.M65_ALTERACION_VISOMOTRIZ;
                        HistoriaAnterior.M65_ALTERACION_VISUAL = historia.M65_ALTERACION_VISUAL;
                        HistoriaAnterior.M65_OTROS = historia.M65_OTROS;
                        HistoriaAnterior.M65_PARTICIPACION = historia.M65_PARTICIPACION;
                        HistoriaAnterior.M65_TRAS_ANIMO = historia.M65_TRAS_ANIMO;
                        HistoriaAnterior.M65_TRAS_ATENCION = historia.M65_TRAS_ATENCION;
                        HistoriaAnterior.M65_TRAS_COMPRENSION = historia.M65_TRAS_COMPRENSION;
                        HistoriaAnterior.M65_TRAS_DEMENCIAL = historia.M65_TRAS_DEMENCIAL;
                        HistoriaAnterior.M65_TRAS_MEMORIA = historia.M65_TRAS_MEMORIA;
                        HistoriaAnterior.M65_TRAS_ORIENTACION = historia.M65_TRAS_ORIENTACION;
                        HistoriaAnterior.MU_MENARQUIA = historia.MU_MENARQUIA;
                        HistoriaAnterior.MUSCULO_ESQUELETICO = historia.MUSCULO_ESQUELETICO;
                        HistoriaAnterior.NERVIOSO = historia.NERVIOSO;
                        HistoriaAnterior.PADECIMIENTO_ACTUAL = historia.PADECIMIENTO_ACTUAL;
                        HistoriaAnterior.PAG3_NOMBRE = historia.PAG3_NOMBRE;
                        HistoriaAnterior.PAG4_NOMBRE = historia.PAG4_NOMBRE;
                        HistoriaAnterior.PAG5_NOMBRE = historia.PAG5_NOMBRE;
                        HistoriaAnterior.PIEL_ANEXOS = historia.PIEL_ANEXOS;
                        HistoriaAnterior.POR_CONSTANCIAS_DOCUMENTALES = historia.POR_CONSTANCIAS_DOCUMENTALES;
                        HistoriaAnterior.RES_NOMBRE = historia.RES_NOMBRE;
                        HistoriaAnterior.RESPIRATORIO = historia.RESPIRATORIO;
                        HistoriaAnterior.SINTOMAS_GENERALES = historia.SINTOMAS_GENERALES;
                        HistoriaAnterior.TERAPEUTICA_PREVIA = historia.TERAPEUTICA_PREVIA;
                        HistoriaAnterior.ULTIMA_FEC = historia.ULTIMA_FEC;
                        HistoriaAnterior.URINARIO = historia.URINARIO;
                        HistoriaAnterior.APNP_ALCOHOLISMO_OBSERV = historia.APNP_ALCOHOLISMO_OBSERV;
                        HistoriaAnterior.APNP_TABAQUISMO_OBSERV = historia.APNP_TABAQUISMO_OBSERV;
                        HistoriaAnterior.APNP_TOXICOMANIAS_OBSERV = historia.APNP_TOXICOMANIAS_OBSERV;
                        HistoriaAnterior.APP_MEDICAMENTOS_ACTIVOS = historia.APP_MEDICAMENTOS_ACTIVOS;
                        Context.Entry(HistoriaAnterior).State = EntityState.Modified;

                        var familiares = Context.HISTORIA_CLINICA_FAMILIAR.Where(x => x.ID_IMPUTADO == historia.ID_IMPUTADO && x.ID_INGRESO == historia.ID_INGRESO);
                        if (familiares != null && familiares.Any())
                            foreach (var item in familiares)
                                Context.Entry(item).State = EntityState.Deleted;

                        short _id_consec_familiares = GetIDProceso<short>("HISTORIA_CLINICA_FAMILIAR", "ID_CONSEC", string.Format("ID_CENTRO={0} AND ID_ANIO={1} AND ID_IMPUTADO={2} AND ID_INGRESO={3}", historia.ID_CENTRO, historia.ID_ANIO, historia.ID_IMPUTADO, historia.ID_INGRESO));
                        foreach (var item in historia.HISTORIA_CLINICA_FAMILIAR)
                        {
                            var dato = new HISTORIA_CLINICA_FAMILIAR();
                            dato.AHF_ALERGIAS = item.AHF_ALERGIAS;
                            dato.AHF_CA = item.AHF_CA;
                            dato.AHF_CARDIACOS = item.AHF_CARDIACOS;
                            dato.AHF_DIABETES = item.AHF_DIABETES;
                            dato.AHF_EDAD = item.AHF_EDAD;
                            dato.AHF_EPILEPSIA = item.AHF_EPILEPSIA;
                            dato.AHF_FALLECIMIENTO_CAUSA = item.AHF_FALLECIMIENTO_CAUSA;
                            dato.AHF_FALLECIMIENTO_FEC = item.AHF_FALLECIMIENTO_FEC;
                            dato.AHF_HIPERTENSIVO = item.AHF_HIPERTENSIVO;
                            dato.AHF_MENTALES = item.AHF_MENTALES;
                            dato.AHF_NOMBRE = item.AHF_NOMBRE;
                            dato.AHF_SANO = item.AHF_SANO;
                            dato.AHF_TB = item.AHF_TB;
                            dato.AHF_VIVE = item.AHF_VIVE;
                            dato.ID_TIPO_REFERENCIA = item.ID_TIPO_REFERENCIA;


                            //llave primaria
                            dato.ID_ANIO = item.ID_ANIO;
                            dato.ID_CENTRO = item.ID_CENTRO;
                            dato.ID_CONSEC = HistoriaAnterior.ID_CONSEC;
                            dato.ID_IMPUTADO = item.ID_IMPUTADO;
                            dato.ID_INGRESO = item.ID_INGRESO;
                            dato.ID_FAMILIAR = _id_consec_familiares;
                            HistoriaAnterior.HISTORIA_CLINICA_FAMILIAR.Add(dato);
                            _id_consec_familiares++;
                        };

                        var ConsecutivoDocumentos = GetIDProceso<short>("HISTORIA_CLINICA_DOCUMENTO", "ID_NODOCTO", string.Format("ID_CENTRO={0} AND ID_ANIO={1} AND ID_IMPUTADO={2} AND ID_INGRESO={3}", historia.ID_CENTRO, historia.ID_ANIO, historia.ID_IMPUTADO, historia.ID_INGRESO));
                        if (historia.HISTORIA_CLINICA_DOCUMENTO != null && historia.HISTORIA_CLINICA_DOCUMENTO.Any())
                            foreach (var item in historia.HISTORIA_CLINICA_DOCUMENTO)
                            {
                                var Docto = new HISTORIA_CLINICA_DOCUMENTO();
                                Docto.DOCUMENTO = item.DOCUMENTO;//archivo
                                Docto.FISICO = item.FISICO;//complemento
                                Docto.ID_DOCTO = item.ID_DOCTO;//foranea
                                Docto.ID_FORMATO = item.ID_FORMATO;
                                Docto.ID_ANIO = item.ID_ANIO;//llave primaria
                                Docto.ID_CENTRO = item.ID_CENTRO;//llave primaria
                                Docto.ID_IMPUTADO = item.ID_IMPUTADO;//llave primaria
                                Docto.ID_INGRESO = HistoriaAnterior.ID_INGRESO;//llave primaria
                                Docto.ID_CONSEC = HistoriaAnterior.ID_CONSEC;//llave primaria
                                Docto.ID_NODOCTO = ConsecutivoDocumentos;
                                HistoriaAnterior.HISTORIA_CLINICA_DOCUMENTO.Add(Docto);
                                ConsecutivoDocumentos++;
                            };


                        ObservableCollection<HISTORIA_CLINICA_PATOLOGICOS> lstTemporales = new ObservableCollection<HISTORIA_CLINICA_PATOLOGICOS>();
                        short ConsecutivoPato = GetIDProceso<short>("HISTORIA_CLINICA_PATOLOGICOS", "ID_NOPATOLOGICO", string.Format("ID_CENTRO={0} AND ID_ANIO={1} AND ID_IMPUTADO={2} AND ID_INGRESO={3}", historia.ID_CENTRO, historia.ID_ANIO, historia.ID_IMPUTADO, historia.ID_INGRESO));
                        if (HistoriaAnterior.HISTORIA_CLINICA_PATOLOGICOS != null && HistoriaAnterior.HISTORIA_CLINICA_PATOLOGICOS.Any())
                        {
                            var Patos = Context.HISTORIA_CLINICA_PATOLOGICOS.Where(x => x.ID_INGRESO == historia.ID_INGRESO && x.ID_IMPUTADO == historia.ID_IMPUTADO);
                            foreach (var item in Patos)
                            {
                                lstTemporales.Add(item);
                                Context.Entry(item).State = EntityState.Deleted;
                            };

                            if (historia.HISTORIA_CLINICA_PATOLOGICOS != null && historia.HISTORIA_CLINICA_PATOLOGICOS.Any())
                            {
                                foreach (var item in historia.HISTORIA_CLINICA_PATOLOGICOS)
                                {
                                    if (lstTemporales.Any(x => x.ID_PATOLOGICO == item.ID_PATOLOGICO && x.RECUPERADO == item.RECUPERADO))
                                        continue;//ya existe y no ha cambiado el estatus de recuperado

                                    else
                                    {
                                        if (lstTemporales.Any(x => x.ID_PATOLOGICO == item.ID_PATOLOGICO && x.RECUPERADO != item.RECUPERADO))
                                        {//es el mismo patologico, pero ya se curo
                                            lstTemporales.Add(new HISTORIA_CLINICA_PATOLOGICOS
                                            {
                                                ID_ANIO = HistoriaAnterior.ID_ANIO,
                                                ID_CENTRO = HistoriaAnterior.ID_CENTRO,
                                                ID_CONSEC = HistoriaAnterior.ID_CONSEC,
                                                ID_IMPUTADO = HistoriaAnterior.ID_IMPUTADO,
                                                ID_INGRESO = HistoriaAnterior.ID_INGRESO,
                                                ID_PATOLOGICO = item.ID_PATOLOGICO,
                                                MOMENTO_DETECCION = item.RECUPERADO == "S" ? "DI" : "EI",
                                                OBSERVACIONES = item.OBSERVACIONES,
                                                OTROS_DESCRIPCION = item.OTROS_DESCRIPCION,
                                                RECUPERADO = item.RECUPERADO,
                                                REGISTRO_FEC = item.REGISTRO_FEC
                                            });
                                        }
                                        else
                                        {
                                            lstTemporales.Add(new HISTORIA_CLINICA_PATOLOGICOS
                                            {
                                                ID_ANIO = HistoriaAnterior.ID_ANIO,
                                                ID_CENTRO = HistoriaAnterior.ID_CENTRO,
                                                ID_CONSEC = HistoriaAnterior.ID_CONSEC,
                                                ID_IMPUTADO = HistoriaAnterior.ID_IMPUTADO,
                                                ID_INGRESO = HistoriaAnterior.ID_INGRESO,
                                                ID_PATOLOGICO = item.ID_PATOLOGICO,
                                                MOMENTO_DETECCION = item.RECUPERADO == "S" ? "DI" : "EI",
                                                OBSERVACIONES = item.OBSERVACIONES,
                                                OTROS_DESCRIPCION = item.OTROS_DESCRIPCION,
                                                RECUPERADO = item.RECUPERADO,
                                                REGISTRO_FEC = item.REGISTRO_FEC
                                            });
                                        };
                                    };
                                };
                            }

                            foreach (var item in lstTemporales)
                            {
                                HistoriaAnterior.HISTORIA_CLINICA_PATOLOGICOS.Add(new HISTORIA_CLINICA_PATOLOGICOS
                                {
                                    ID_ANIO = HistoriaAnterior.ID_ANIO,
                                    ID_CENTRO = HistoriaAnterior.ID_CENTRO,
                                    ID_CONSEC = HistoriaAnterior.ID_CONSEC,
                                    ID_NOPATOLOGICO = ConsecutivoPato,
                                    ID_IMPUTADO = HistoriaAnterior.ID_IMPUTADO,
                                    ID_INGRESO = HistoriaAnterior.ID_INGRESO,
                                    ID_PATOLOGICO = item.ID_PATOLOGICO,
                                    MOMENTO_DETECCION = item.RECUPERADO == "S" ? "DI" : "EI",
                                    OBSERVACIONES = item.OBSERVACIONES,
                                    OTROS_DESCRIPCION = item.OTROS_DESCRIPCION,
                                    RECUPERADO = item.RECUPERADO,
                                    REGISTRO_FEC = item.REGISTRO_FEC
                                });

                                ConsecutivoPato++;
                            }
                        }
                        else
                        {
                            foreach (var item in historia.HISTORIA_CLINICA_PATOLOGICOS)
                            {
                                HistoriaAnterior.HISTORIA_CLINICA_PATOLOGICOS.Add(new HISTORIA_CLINICA_PATOLOGICOS
                                 {
                                     ID_ANIO = HistoriaAnterior.ID_ANIO,
                                     ID_CENTRO = HistoriaAnterior.ID_CENTRO,
                                     ID_CONSEC = HistoriaAnterior.ID_CONSEC,
                                     ID_NOPATOLOGICO = ConsecutivoPato,
                                     ID_IMPUTADO = HistoriaAnterior.ID_IMPUTADO,
                                     ID_INGRESO = HistoriaAnterior.ID_INGRESO,
                                     ID_PATOLOGICO = item.ID_PATOLOGICO,
                                     MOMENTO_DETECCION = item.RECUPERADO == "S" ? "DI" : "EI",
                                     OBSERVACIONES = item.OBSERVACIONES,
                                     OTROS_DESCRIPCION = item.OTROS_DESCRIPCION,
                                     RECUPERADO = item.RECUPERADO,
                                     REGISTRO_FEC = item.REGISTRO_FEC
                                 });

                                ConsecutivoPato++;
                            }
                        }

                        var _SexoImputado = Context.INGRESO.FirstOrDefault(x => x.ID_INGRESO == historia.ID_INGRESO && x.ID_IMPUTADO == historia.ID_IMPUTADO && x.ID_ANIO == historia.ID_ANIO && x.ID_CENTRO == historia.ID_CENTRO);
                        if (_SexoImputado.IMPUTADO != null)
                            if (_SexoImputado.IMPUTADO.SEXO == "F")
                            {
                                short _id_consec_gineco = GetIDProceso<short>("HISTORIA_CLINICA_GINECO_OBSTRE", "ID_CONSEC", string.Format("ID_CENTRO={0} AND ID_ANIO={1} AND ID_IMPUTADO={2} AND ID_INGRESO={3}", historia.ID_CENTRO, historia.ID_ANIO, historia.ID_IMPUTADO, historia.ID_INGRESO));
                                if (historia.HISTORIA_CLINICA_GINECO_OBSTRE != null && historia.HISTORIA_CLINICA_GINECO_OBSTRE.Any())
                                {
                                    foreach (var item in historia.HISTORIA_CLINICA_GINECO_OBSTRE)
                                    {
                                        var _Gineco = new HISTORIA_CLINICA_GINECO_OBSTRE()
                                        {
                                            ABORTO = item.ABORTO,
                                            ABORTO_MODIFICADO = item.ABORTO_MODIFICADO,
                                            ANIOS_RITMO = item.ANIOS_RITMO,
                                            ANIOS_RITMO_MODIFICADO = item.ANIOS_RITMO_MODIFICADO,
                                            CESAREA = item.CESAREA,
                                            CESAREA_MODIFICADO = item.CESAREA_MODIFICADO,
                                            CONTROL_PRENATAL = !string.IsNullOrEmpty(item.CONTROL_PRENATAL) ? item.CONTROL_PRENATAL != "-1" ? item.CONTROL_PRENATAL : string.Empty : string.Empty,
                                            CONTROL_PRENATAL_MODIFICADO = item.CONTROL_PRENATAL_MODIFICADO,
                                            DEFORMACION = item.DEFORMACION,
                                            DEFORMACION_MODIFICADO = item.DEFORMACION_MODIFICADO,
                                            EMBARAZO = item.EMBARAZO,
                                            EMBARAZO_MODIFICADO = item.EMBARAZO_MODIFICADO,
                                            FECHA_PROBABLE_PARTO = item.FECHA_PROBABLE_PARTO,
                                            FECHA_PROBABLE_PARTO_MOD = item.FECHA_PROBABLE_PARTO_MOD,
                                            FUENTE = item.FUENTE,
                                            ID_ANIO = historia.ID_ANIO,
                                            ID_CONTROL_PRENATAL = item.ID_CONTROL_PRENATAL.HasValue ? item.ID_CONTROL_PRENATAL != -1 ? item.ID_CONTROL_PRENATAL : new short?() : new short?(),
                                            ID_CENTRO = historia.ID_CENTRO,
                                            ID_IMPUTADO = historia.ID_IMPUTADO,
                                            ID_INGRESO = historia.ID_INGRESO,
                                            MOMENTO_REGISTRO = item.MOMENTO_REGISTRO,
                                            PARTO = item.PARTO,
                                            PARTO_MODIFICADO = item.PARTO_MODIFICADO,
                                            REGISTRO_FEC = item.REGISTRO_FEC,
                                            ULTIMA_MENS_MODIFICADO = item.ULTIMA_MENS_MODIFICADO,
                                            ULTIMA_MENSTRUACION_FEC = item.ULTIMA_MENSTRUACION_FEC,
                                            ID_CONSEC = _id_consec_gineco,
                                            ID_GINECO = HistoriaAnterior.ID_CONSEC
                                        };

                                        HistoriaAnterior.HISTORIA_CLINICA_GINECO_OBSTRE.Add(_Gineco);
                                    }
                                };
                            };


                        short _id_consec_grupos_vulnerables = GetIDProceso<short>("GRUPO_VULNERABLE", "ID_GRUPO_CONSEC", string.Format("ID_CENTRO={0} AND ID_ANIO={1} AND ID_IMPUTADO={2} AND ID_INGRESO={3}", historia.ID_CENTRO, historia.ID_ANIO, historia.ID_IMPUTADO, historia.ID_INGRESO));
                        string _DetalleGruposVulnerables = string.Empty;
                        var VulnerablesExistentes = Context.GRUPO_VULNERABLE.Where(x => x.ID_INGRESO == HistoriaAnterior.ID_INGRESO && x.ID_IMPUTADO == HistoriaAnterior.ID_IMPUTADO && x.ID_CONSEC == HistoriaAnterior.ID_CONSEC);
                        if (historia.GRUPO_VULNERABLE != null && historia.GRUPO_VULNERABLE.Any())
                        {
                            //SE GUARDAN LOS NUEVOS GRUPOS VULNERABLES
                            foreach (var item in historia.GRUPO_VULNERABLE)
                            {
                                if (VulnerablesExistentes.Any(x => x.ID_SECTOR_CLAS == item.ID_SECTOR_CLAS))
                                    continue;//Ya existe, lo omito para no duplicar

                                var NvoGrupoV = new GRUPO_VULNERABLE()
                                {
                                    BAJA_FEC = item.BAJA_FEC,
                                    ID_ANIO = item.ID_ANIO,
                                    REGISTRO_FEC = item.REGISTRO_FEC,
                                    MOMENTO_DETECCION = item.MOMENTO_DETECCION,
                                    ID_SECTOR_CLAS = item.ID_SECTOR_CLAS,
                                    ID_CENTRO = item.ID_CENTRO,
                                    ID_CONSEC = HistoriaAnterior.ID_CONSEC,
                                    ID_GRUPO_CONSEC = _id_consec_grupos_vulnerables,
                                    ID_IMPUTADO = item.ID_IMPUTADO,
                                    ID_INGRESO = item.ID_INGRESO
                                };

                                HistoriaAnterior.GRUPO_VULNERABLE.Add(NvoGrupoV);
                                _id_consec_grupos_vulnerables++;

                                var _DescripcionGrupo = Context.SECTOR_CLASIFICACION.Where(x => x.ID_SECTOR_CLAS == item.ID_SECTOR_CLAS).FirstOrDefault();
                                if (_DescripcionGrupo != null)
                                    _DetalleGruposVulnerables += string.Format("- {0} \n", !string.IsNullOrEmpty(_DescripcionGrupo.POBLACION) ? _DescripcionGrupo.POBLACION.Trim() : string.Empty);
                            };

                            //SE GENERA LA NOTIFICACION 
                            DateTime _fecha = GetFechaServerDate();
                            var _MensajeConsiderado = Context.MENSAJE_TIPO.FirstOrDefault(x => x.ID_MEN_TIPO == 95);
                            string _Nombre = string.Empty;
                            var _id_mensaje = GetIDProceso<int>("MENSAJE", "ID_MENSAJE", "1=1");
                            var NombreImputado = Context.IMPUTADO.First(w => w.ID_ANIO == HistoriaAnterior.ID_ANIO && w.ID_CENTRO == HistoriaAnterior.ID_CENTRO && w.ID_IMPUTADO == HistoriaAnterior.ID_IMPUTADO);
                            if (NombreImputado != null)
                                _Nombre = string.Format("{0} {1} {2} ({3}/{4})", !string.IsNullOrEmpty(NombreImputado.NOMBRE) ? NombreImputado.NOMBRE.Trim() : string.Empty,
                                    !string.IsNullOrEmpty(NombreImputado.PATERNO) ? NombreImputado.PATERNO.Trim() : string.Empty, !string.IsNullOrEmpty(NombreImputado.MATERNO) ? NombreImputado.MATERNO.Trim() : string.Empty,
                                    NombreImputado.ID_ANIO.ToString(), NombreImputado.ID_IMPUTADO.ToString());

                            var NvoMensaje = new MENSAJE()
                            {
                                CONTENIDO = _MensajeConsiderado.CONTENIDO + " " + _Nombre + " HA INGRESADO A LOS SIGUIENTES GRUPOS VULNERABLES: \n" + _DetalleGruposVulnerables,
                                ENCABEZADO = _MensajeConsiderado.ENCABEZADO,
                                ID_MEN_TIPO = _MensajeConsiderado.ID_MEN_TIPO,
                                ID_MENSAJE = _id_mensaje,
                                REGISTRO_FEC = _fecha,
                                ID_CENTRO = GlobalVariables.gCentro
                            };

                            Context.MENSAJE.Add(NvoMensaje);
                        }



                        var _CitaMedica = Context.ATENCION_CITA.Where(x => x.ID_ANIO == historia.ID_ANIO && x.ID_CENTRO == historia.ID_CENTRO && x.ID_IMPUTADO == historia.ID_IMPUTADO && x.ID_TIPO_ATENCION == 1 && x.ID_TIPO_SERVICIO == 4);
                        if (_CitaMedica.Any())
                            foreach (var item in _CitaMedica)
                            {
                                item.ESTATUS = "S";
                                Context.Entry(item).Property(x => x.ESTATUS).IsModified = true;
                            };
                    }

                    Context.SaveChanges();
                    transaccion.Complete();
                    return historia;
                };
            }
            catch (System.Data.Entity.Validation.DbEntityValidationException dbEx)
            {
                foreach (var validationErrors in dbEx.EntityValidationErrors)
                    foreach (var validationError in validationErrors.ValidationErrors)
                        System.Diagnostics.Trace.TraceInformation("Nombre del causante de excepción: {0} Error: {1}", validationError.PropertyName, validationError.ErrorMessage);

                return null;
            }
            catch
            {
                return null;
                //throw new ApplicationException(ex.Message);
            }
        }

        /// <summary>
        /// metodo que se conecta a la base de datos para actualizar un registro
        /// </summary>
        /// <param name="Entity">objeto de tipo "INGRESO_DELITO" con valores a actualizar</param>
        public bool Actualizar(HISTORIA_CLINICA Entity)
        {
            try
            {
                Update(Entity);
                return true;
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
        public bool Eliminar(short anio, short centro, int imputado, short ingreso)
        {
            try
            {
                var Entity = GetData().Where(w => w.ID_ANIO == anio && w.ID_CENTRO == centro && w.ID_IMPUTADO == imputado && w.ID_INGRESO == ingreso).ToList();
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