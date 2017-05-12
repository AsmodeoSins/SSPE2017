using SSP.Modelo;
using SSP.Servidor;
using System.Linq;

namespace SSP.Controlador.Catalogo.Justicia
{
    public class cHistoriaClinicaDental : EntityManagerServer<HISTORIA_CLINICA_DENTAL>
    {
        public bool InsertarHistoriaClinicaDental(HISTORIA_CLINICA_DENTAL Entity)
        {
            try
            {
                using (System.Transactions.TransactionScope transaccion = new System.Transactions.TransactionScope(System.Transactions.TransactionScopeOption.RequiresNew, new System.Transactions.TransactionOptions() { IsolationLevel = System.Transactions.IsolationLevel.ReadCommitted }))
                {
                    var _HistoriaDental = Context.HISTORIA_CLINICA_DENTAL.FirstOrDefault(x => x.ID_INGRESO == Entity.ID_INGRESO && x.ID_IMPUTADO == Entity.ID_IMPUTADO && x.ID_ANIO == Entity.ID_ANIO && x.ID_CENTRO == Entity.ID_CENTRO);

                    if (_HistoriaDental == null)
                    {
                        var ConsecutivoHCD = GetIDProceso<short>("HISTORIA_CLINICA_DENTAL", "ID_CONSEC", string.Format("ID_CENTRO={0} AND ID_ANIO={1} AND ID_IMPUTADO={2} AND ID_INGRESO={3}", Entity.ID_CENTRO, Entity.ID_ANIO, Entity.ID_IMPUTADO, Entity.ID_INGRESO));

                        _HistoriaDental = new HISTORIA_CLINICA_DENTAL()
                        {
                            ALERGICO_MEDICAMENTO = Entity.ALERGICO_MEDICAMENTO,
                            AMENAZA_ABORTO = Entity.AMENAZA_ABORTO,
                            ALERGICO_MEDICAMENTO_CUAL = Entity.ALERGICO_MEDICAMENTO_CUAL,
                            LACTANDO = Entity.LACTANDO,
                            ART_TEMP_CANSANCIO = Entity.ART_TEMP_CANSANCIO,
                            ART_TEMP_CANSANCIO_OBS = Entity.ART_TEMP_CANSANCIO_OBS,
                            ART_TEMP_CHASQUIDOS = Entity.ART_TEMP_CHASQUIDOS,
                            ART_TEMP_CHASQUIDOS_OBS = Entity.ART_TEMP_CHASQUIDOS_OBS,
                            ART_TEMP_DOLOR = Entity.ART_TEMP_DOLOR,
                            ART_TEMP_DOLOR_OBS = Entity.ART_TEMP_DOLOR_OBS,
                            ART_TEMP_RIGIDEZ = Entity.ART_TEMP_RIGIDEZ,
                            ART_TEMP_RIGIDEZ_OBS = Entity.ART_TEMP_RIGIDEZ_OBS,
                            ENCIAS_COLORACION = Entity.ENCIAS_COLORACION,
                            ENCIAS_FORMA = Entity.ENCIAS_FORMA,
                            ENCIAS_TEXTURA = Entity.ENCIAS_TEXTURA,
                            EXP_BUC_AMIGDALAS = Entity.EXP_BUC_AMIGDALAS,
                            EXP_BUC_CARRILLOS = Entity.EXP_BUC_CARRILLOS,
                            EXP_BUC_FRENILLOS = Entity.EXP_BUC_FRENILLOS,
                            EXP_BUC_LABIOS = Entity.EXP_BUC_LABIOS,
                            EXP_BUC_LENGUA = Entity.EXP_BUC_LENGUA,
                            EXP_BUC_MUCOSA_NASAL = Entity.EXP_BUC_MUCOSA_NASAL,
                            EXP_BUC_OTROS = Entity.EXP_BUC_OTROS,
                            EXP_BUC_PALADAR_BLANCO = Entity.EXP_BUC_PALADAR_BLANCO,
                            EXP_BUC_PALADAR_DURO = Entity.EXP_BUC_PALADAR_DURO,
                            EXP_BUC_PISO_BOCA = Entity.EXP_BUC_PISO_BOCA,
                            COMPLICACIONES = Entity.COMPLICACIONES,
                            DIENTES_ANOM_FORMA = Entity.DIENTES_ANOM_FORMA,
                            DIENTES_ANOM_FORMA_OBS = Entity.DIENTES_ANOM_FORMA_OBS,
                            DIENTES_ANOM_TAMANO = Entity.DIENTES_ANOM_TAMANO,
                            DIENTES_ANOM_TAMANO_OBS = Entity.DIENTES_ANOM_TAMANO_OBS,
                            DIENTES_CARIES = Entity.DIENTES_CARIES,
                            DIENTES_FLUOROSIS = Entity.DIENTES_FLUOROSIS,
                            DIENTES_HIPOPLASIA = Entity.DIENTES_HIPOPLASIA,
                            DIENTES_HIPOPLASIA_OBS = Entity.DIENTES_HIPOPLASIA_OBS,
                            DIENTES_OTROS = Entity.DIENTES_OTROS,
                            FRECUENCIA_CARDIAC = Entity.FRECUENCIA_CARDIAC,
                            FRECUENCIA_RESPIRA = Entity.FRECUENCIA_RESPIRA,
                            GLICEMIA = Entity.GLICEMIA,
                            HEMORRAGIA = Entity.HEMORRAGIA,
                            ID_ANIO = Entity.ID_ANIO,
                            ID_IMPUTADO = Entity.ID_IMPUTADO,
                            ESTATURA = Entity.ESTATURA,
                            ESTATUS = "T",
                            USUARIO_ENFERMERA = string.Empty,//ESTE CAMPO NO SE USA, SE MANEJA DENTRO DEL FORMATO IMPRESO, PERO SE DESCARTO EN UNA JUNTA CON EL AREA DENTAL
                            ID_USUARIO = Entity.ID_USUARIO,
                            REACCION_ANESTESICO = Entity.REACCION_ANESTESICO,
                            TEMPERATURA = Entity.TEMPERATURA,
                            ID_CENTRO = Entity.ID_CENTRO,
                            ID_CONSEC = ConsecutivoHCD,
                            ID_INGRESO = Entity.ID_INGRESO,
                            BRUXISMO = Entity.BRUXISMO,
                            BRUXISMO_DOLOR = Entity.BRUXISMO_DOLOR,
                            PESO = Entity.PESO,
                            REGISTRO_FEC = Entity.REGISTRO_FEC,
                            TENSION_ARTERIAL = Entity.TENSION_ARTERIAL
                        };

                        var _consecutivoDientesOdontogramaInicial = GetIDProceso<int>("ODONTOGRAMA_INICIAL", "ID_CONSECUTIVO", "1=1");
                        if (Entity.ODONTOGRAMA_INICIAL != null && Entity.ODONTOGRAMA_INICIAL.Any())
                            foreach (var item in Entity.ODONTOGRAMA_INICIAL)
                            {
                                var _NuevoDiente = new ODONTOGRAMA_INICIAL
                                {
                                    ID_ANIO = Entity.ID_ANIO,
                                    ID_CENTRO = Entity.ID_CENTRO,
                                    ID_CONSEC = ConsecutivoHCD,
                                    ID_CONSECUTIVO = _consecutivoDientesOdontogramaInicial,
                                    ID_ENFERMEDAD = item.ID_ENFERMEDAD.HasValue ? item.ID_ENFERMEDAD.Value != -1 ? item.ID_ENFERMEDAD : null : null,
                                    ID_POSICION = item.ID_POSICION,
                                    ID_IMPUTADO = Entity.ID_IMPUTADO,
                                    ID_INGRESO = Entity.ID_INGRESO,
                                    ID_SIMBOLO = item.ID_SIMBOLO,
                                    ID_NOMENCLATURA = item.ID_NOMENCLATURA.HasValue ? item.ID_NOMENCLATURA != -1 ? item.ID_NOMENCLATURA : null : null,
                                    ID_TIPO_ODO = item.ID_TIPO_ODO,
                                    REGISTRO_FEC = item.REGISTRO_FEC
                                };

                                _HistoriaDental.ODONTOGRAMA_INICIAL.Add(_NuevoDiente);
                                _consecutivoDientesOdontogramaInicial++;
                            };

                        var _ConsecutivoImagenesDentales = GetIDProceso<int>("HISTORIA_CLINICA_DENTAL_DOCUME", "ID_HCDDOCTO", "1=1");
                        if (Entity.HISTORIA_CLINICA_DENTAL_DOCUME != null && Entity.HISTORIA_CLINICA_DENTAL_DOCUME.Any())
                            foreach (var item in Entity.HISTORIA_CLINICA_DENTAL_DOCUME)
                            {
                                var NvaImagen = new HISTORIA_CLINICA_DENTAL_DOCUME()
                                {
                                    DOCUMENTO = item.DOCUMENTO,
                                    ID_ANIO = Entity.ID_ANIO,
                                    ID_CENTRO = Entity.ID_CENTRO,
                                    ID_CONSEC = _HistoriaDental.ID_CONSEC,
                                    ID_DOCTO = item.ID_DOCTO,
                                    ID_HCDDOCTO = _ConsecutivoImagenesDentales,
                                    ID_IMPUTADO = Entity.ID_IMPUTADO,
                                    ID_INGRESO = Entity.ID_INGRESO
                                };

                                _HistoriaDental.HISTORIA_CLINICA_DENTAL_DOCUME.Add(NvaImagen);
                                _ConsecutivoImagenesDentales++;
                            };

                        var _CitaMedica = Context.ATENCION_CITA.Where(x => x.ID_ANIO == Entity.ID_ANIO && x.ID_CENTRO == Entity.ID_CENTRO && x.ID_IMPUTADO == Entity.ID_IMPUTADO && x.ID_TIPO_ATENCION == 2 && x.ID_TIPO_SERVICIO == 4);
                        if (_CitaMedica.Any())
                            foreach (var item in _CitaMedica)
                            {
                                item.ESTATUS = "S";
                                Context.Entry(item).Property(x => x.ESTATUS).IsModified = true;
                            };


                        Context.HISTORIA_CLINICA_DENTAL.Add(_HistoriaDental);
                    }
                    else
                    {
                        _HistoriaDental.ALERGICO_MEDICAMENTO = Entity.ALERGICO_MEDICAMENTO;
                        _HistoriaDental.AMENAZA_ABORTO = Entity.AMENAZA_ABORTO;
                        _HistoriaDental.ALERGICO_MEDICAMENTO_CUAL = Entity.ALERGICO_MEDICAMENTO_CUAL;
                        _HistoriaDental.LACTANDO = Entity.LACTANDO;
                        _HistoriaDental.ART_TEMP_CANSANCIO = Entity.ART_TEMP_CANSANCIO;
                        _HistoriaDental.ART_TEMP_CANSANCIO_OBS = Entity.ART_TEMP_CANSANCIO_OBS;
                        _HistoriaDental.ART_TEMP_CHASQUIDOS = Entity.ART_TEMP_CHASQUIDOS;
                        _HistoriaDental.ART_TEMP_CHASQUIDOS_OBS = Entity.ART_TEMP_CHASQUIDOS_OBS;
                        _HistoriaDental.ART_TEMP_DOLOR = Entity.ART_TEMP_DOLOR;
                        _HistoriaDental.ART_TEMP_DOLOR_OBS = Entity.ART_TEMP_DOLOR_OBS;
                        _HistoriaDental.ART_TEMP_RIGIDEZ = Entity.ART_TEMP_RIGIDEZ;
                        _HistoriaDental.ART_TEMP_RIGIDEZ_OBS = Entity.ART_TEMP_RIGIDEZ_OBS;
                        _HistoriaDental.BRUXISMO = Entity.BRUXISMO;
                        _HistoriaDental.BRUXISMO_DOLOR = Entity.BRUXISMO_DOLOR;
                        _HistoriaDental.COMPLICACIONES = Entity.COMPLICACIONES;
                        _HistoriaDental.DIENTES_ANOM_FORMA = Entity.DIENTES_ANOM_FORMA;
                        _HistoriaDental.DIENTES_ANOM_FORMA_OBS = Entity.DIENTES_ANOM_FORMA_OBS;
                        _HistoriaDental.DIENTES_ANOM_TAMANO = Entity.DIENTES_ANOM_TAMANO;
                        _HistoriaDental.DIENTES_ANOM_TAMANO_OBS = Entity.DIENTES_ANOM_TAMANO_OBS;
                        _HistoriaDental.DIENTES_CARIES = Entity.DIENTES_CARIES;
                        _HistoriaDental.DIENTES_FLUOROSIS = Entity.DIENTES_FLUOROSIS;
                        _HistoriaDental.DIENTES_HIPOPLASIA = Entity.DIENTES_HIPOPLASIA;
                        _HistoriaDental.DIENTES_HIPOPLASIA_OBS = Entity.DIENTES_HIPOPLASIA_OBS;
                        _HistoriaDental.DIENTES_OTROS = Entity.DIENTES_OTROS;
                        _HistoriaDental.ENCIAS_COLORACION = Entity.ENCIAS_COLORACION;
                        _HistoriaDental.ENCIAS_FORMA = Entity.ENCIAS_FORMA;
                        _HistoriaDental.ENCIAS_TEXTURA = Entity.ENCIAS_TEXTURA;
                        _HistoriaDental.EXP_BUC_AMIGDALAS = Entity.EXP_BUC_AMIGDALAS;
                        _HistoriaDental.EXP_BUC_CARRILLOS = Entity.EXP_BUC_CARRILLOS;
                        _HistoriaDental.ESTATURA = Entity.ESTATURA;
                        _HistoriaDental.EXP_BUC_FRENILLOS = Entity.EXP_BUC_FRENILLOS;
                        _HistoriaDental.EXP_BUC_LABIOS = Entity.EXP_BUC_LABIOS;
                        _HistoriaDental.EXP_BUC_LENGUA = Entity.EXP_BUC_LENGUA;
                        _HistoriaDental.EXP_BUC_MUCOSA_NASAL = Entity.EXP_BUC_MUCOSA_NASAL;
                        _HistoriaDental.EXP_BUC_OTROS = Entity.EXP_BUC_OTROS;
                        _HistoriaDental.EXP_BUC_PALADAR_BLANCO = Entity.EXP_BUC_PALADAR_BLANCO;
                        _HistoriaDental.EXP_BUC_PALADAR_DURO = Entity.EXP_BUC_PALADAR_DURO;
                        _HistoriaDental.EXP_BUC_PISO_BOCA = Entity.EXP_BUC_PISO_BOCA;
                        _HistoriaDental.FRECUENCIA_CARDIAC = Entity.FRECUENCIA_CARDIAC;
                        _HistoriaDental.FRECUENCIA_RESPIRA = Entity.FRECUENCIA_RESPIRA;
                        _HistoriaDental.GLICEMIA = Entity.GLICEMIA;
                        _HistoriaDental.HEMORRAGIA = Entity.HEMORRAGIA;
                        _HistoriaDental.PESO = Entity.PESO;
                        _HistoriaDental.ESTATUS = "T";
                        _HistoriaDental.REACCION_ANESTESICO = Entity.REACCION_ANESTESICO;
                        //_HistoriaDental.REGISTRO_FEC = Entity.REGISTRO_FEC; YA TIENE UNA FECHA DE REGISTRO, NO ES CONVENIENTE MODIFICARLA
                        _HistoriaDental.TEMPERATURA = Entity.TEMPERATURA;
                        _HistoriaDental.TENSION_ARTERIAL = Entity.TENSION_ARTERIAL;

                        var _dientesActuales = Context.ODONTOGRAMA_INICIAL.Where(x => x.ID_ANIO == Entity.ID_ANIO && x.ID_CENTRO == Entity.ID_CENTRO && x.ID_IMPUTADO == Entity.ID_IMPUTADO && x.ID_INGRESO == Entity.ID_INGRESO && x.ID_CONSEC == _HistoriaDental.ID_CONSEC);
                        if (_dientesActuales.Any())
                            foreach (var item in _dientesActuales)
                                Context.Entry(item).State = System.Data.EntityState.Deleted;

                        var _ImagenesAscuales = Context.HISTORIA_CLINICA_DENTAL_DOCUME.Where(x => x.ID_ANIO == Entity.ID_ANIO && x.ID_CENTRO == Entity.ID_CENTRO && x.ID_IMPUTADO == Entity.ID_IMPUTADO && x.ID_INGRESO == Entity.ID_INGRESO && Entity.ID_CONSEC == _HistoriaDental.ID_CONSEC);
                        if (_ImagenesAscuales.Any())
                            foreach (var item in _ImagenesAscuales)
                                Context.Entry(item).State = System.Data.EntityState.Deleted;

                        var _ConsecutivoImagenesDentales = GetIDProceso<int>("HISTORIA_CLINICA_DENTAL_DOCUME", "ID_HCDDOCTO", "1=1");

                        Context.Entry(_HistoriaDental).State = System.Data.EntityState.Modified;

                        var _consecutivoDientesOdontogramaInicial = GetIDProceso<int>("ODONTOGRAMA_INICIAL", "ID_CONSECUTIVO", "1=1");
                        if (Entity.ODONTOGRAMA_INICIAL != null && Entity.ODONTOGRAMA_INICIAL.Any())
                            foreach (var item in Entity.ODONTOGRAMA_INICIAL)
                            {
                                var _NuevoDiente = new ODONTOGRAMA_INICIAL
                                {
                                    ID_ANIO = Entity.ID_ANIO,
                                    ID_CENTRO = Entity.ID_CENTRO,
                                    ID_CONSEC = _HistoriaDental.ID_CONSEC,
                                    ID_CONSECUTIVO = _consecutivoDientesOdontogramaInicial,
                                    ID_ENFERMEDAD = item.ID_ENFERMEDAD.HasValue ? item.ID_ENFERMEDAD.Value != -1 ? item.ID_ENFERMEDAD : null : null,
                                    ID_POSICION = item.ID_POSICION,
                                    ID_IMPUTADO = Entity.ID_IMPUTADO,
                                    ID_SIMBOLO = item.ID_SIMBOLO,
                                    ID_INGRESO = Entity.ID_INGRESO,
                                    ID_NOMENCLATURA = item.ID_NOMENCLATURA.HasValue ? item.ID_NOMENCLATURA.Value != -1 ? item.ID_NOMENCLATURA : null : null,
                                    ID_TIPO_ODO = item.ID_TIPO_ODO,
                                    REGISTRO_FEC = item.REGISTRO_FEC
                                };

                                _HistoriaDental.ODONTOGRAMA_INICIAL.Add(_NuevoDiente);
                                _consecutivoDientesOdontogramaInicial++;
                            };


                        if (Entity.HISTORIA_CLINICA_DENTAL_DOCUME != null && Entity.HISTORIA_CLINICA_DENTAL_DOCUME.Any())
                            foreach (var item in Entity.HISTORIA_CLINICA_DENTAL_DOCUME)
                            {
                                var NvaImagen = new HISTORIA_CLINICA_DENTAL_DOCUME()
                                {
                                    DOCUMENTO = item.DOCUMENTO,
                                    ID_ANIO = Entity.ID_ANIO,
                                    ID_CENTRO = Entity.ID_CENTRO,
                                    ID_CONSEC = _HistoriaDental.ID_CONSEC,
                                    ID_DOCTO = item.ID_DOCTO,
                                    ID_HCDDOCTO = _ConsecutivoImagenesDentales,
                                    ID_IMPUTADO = Entity.ID_IMPUTADO,
                                    ID_INGRESO = Entity.ID_INGRESO
                                };

                                _HistoriaDental.HISTORIA_CLINICA_DENTAL_DOCUME.Add(NvaImagen);
                                _ConsecutivoImagenesDentales++;
                            };


                        var _CitaMedica = Context.ATENCION_CITA.Where(x => x.ID_ANIO == Entity.ID_ANIO && x.ID_CENTRO == Entity.ID_CENTRO && x.ID_IMPUTADO == Entity.ID_IMPUTADO && x.ID_TIPO_ATENCION == 2 && x.ID_TIPO_SERVICIO == 4);
                        if (_CitaMedica.Any())
                            foreach (var item in _CitaMedica)
                            {
                                item.ESTATUS = "S";
                                Context.Entry(item).Property(x => x.ESTATUS).IsModified = true;
                            };
                    }

                    Context.SaveChanges();
                    transaccion.Complete();
                    return true;
                }
            }

            catch (System.Data.Entity.Validation.DbEntityValidationException dbEx)
            {
                foreach (var validationErrors in dbEx.EntityValidationErrors)
                    foreach (var validationError in validationErrors.ValidationErrors)
                        System.Diagnostics.Trace.TraceInformation("Nombre del causante de excepción: {0} Error: {1}", validationError.PropertyName, validationError.ErrorMessage);

                return false;
            }

            catch (System.Exception exc)
            {
                return false;
            }
        }
    }
}