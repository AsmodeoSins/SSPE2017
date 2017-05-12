using System;
using System.Collections.ObjectModel;
using SSP.Servidor;
using System.Collections.Generic;
using ControlPenales.Clases;
using System.Linq;
using System.Windows.Media.Imaging;
using System.IO;
using SSP.Controlador.Catalogo.Justicia;

namespace ControlPenales
{
    partial class CausaPenalViewModel : ValidationViewModelBase
    {
        private bool GuardarCausaPenal()
        {
            try
            {
                if (!base.HasErrors)
                {
                    var cp = new CAUSA_PENAL();
                    
                    #region Averiguacion Previa
                    if (SelectedAgencia.ID_AGENCIA != -1)
                        cp.ID_AGENCIA = SelectedAgencia.ID_AGENCIA;
                    if (SelectedAgencia.ID_ENTIDAD != -1)
                        cp.ID_ENTIDAD = SelectedAgencia.ID_ENTIDAD;
                    if (SelectedAgencia.ID_MUNICIPIO != -1)
                        cp.ID_MUNICIPIO = SelectedAgencia.ID_MUNICIPIO;
                    cp.AP_ANIO = AnioAP;
                    cp.AP_FOLIO = FolioAP;
                    cp.AP_FORANEA = AveriguacionPreviaAP;
                    cp.AP_FEC_INICIO = FecAveriguacionAP;
                    cp.AP_FEC_CONSIGNACION = FecConsignacionAP;
                    #endregion

                    #region Causa Penal
                    cp.CP_ANIO = AnioCP;
                    cp.CP_FOLIO = FolioCP;
                    cp.CP_BIS = BisCP;
                    cp.CP_FORANEO = ForaneoCP;
                    if (TipoOrdenCP != -1)
                        cp.CP_TIPO_ORDEN = TipoOrdenCP;
                    if (PaisJuzgadoCP != -1)
                        cp.CP_PAIS_JUZGADO = PaisJuzgadoCP;
                    if (EstadoJuzgadoCP != -1)
                        cp.CP_ESTADO_JUZGADO = EstadoJuzgadoCP;
                    if (MunicipioJuzgadoCP != -1)
                        cp.CP_MUNICIPIO_JUZGADO = MunicipioJuzgadoCP;
                    if (!string.IsNullOrEmpty(FueroCP))
                        cp.CP_FUERO = FueroCP;
                    if (JuzgadoCP != -1)
                        cp.CP_JUZGADO = JuzgadoCP;
                    cp.CP_FEC_RADICACION = FecRadicacionCP;
                    if (!string.IsNullOrEmpty(AmpliacionCP))
                        cp.CP_AMPLIACION = AmpliacionCP;
                    if (TerminoCP != -1)
                        cp.CP_TERMINO = TerminoCP;
                    if (EstatusCP != -1)
                        cp.ID_ESTATUS_CP = EstatusCP;
                    cp.CP_FEC_VENCIMIENTO_TERMINO = FecVencimientoTerinoCP;
                    cp.OBSERV = ObservacionesCP;
                    #endregion
                    
                    //#region Baja
                    //cp.BAJA_FEC = BFecha;
                    //cp.ID_MOTIVO_BAJA = BMotivoBaja != null ? BMotivoBaja : null;
                    //cp.ID_AUTO_BAJA = BAutoridadBaja != null ? BAutoridadBaja : null;
                    //#endregion

                    if (SelectedCausaPenal == null)//INSERT
                    {
                        cp.ID_CENTRO = SelectedIngreso.ID_CENTRO;
                        cp.ID_ANIO = SelectedIngreso.ID_ANIO;
                        cp.ID_IMPUTADO = SelectedIngreso.ID_IMPUTADO;
                        cp.ID_INGRESO = SelectedIngreso.ID_INGRESO;
                        
                        #region Delitos
                        var delitos = new List<CAUSA_PENAL_DELITO>(LstCausaPenalDelitos == null ? null : LstCausaPenalDelitos.Select((w, i) => new CAUSA_PENAL_DELITO()
                        {
                            ID_CENTRO = SelectedIngreso.ID_CENTRO,
                            ID_ANIO = SelectedIngreso.ID_ANIO,
                            ID_IMPUTADO = SelectedIngreso.ID_IMPUTADO,
                            ID_INGRESO = SelectedIngreso.ID_INGRESO,
                            ID_CAUSA_PENAL = 0,
                            ID_DELITO = w.ID_DELITO,
                            ID_FUERO = w.ID_FUERO,
                            ID_MODALIDAD = w.ID_MODALIDAD,
                            ID_TIPO_DELITO = w.ID_TIPO_DELITO,
                            CANTIDAD = w.CANTIDAD,
                            OBJETO = w.OBJETO,
                            DESCR_DELITO = w.DESCR_DELITO,
                            ID_CONS = Convert.ToInt16(i + 1)
                        }));
                        cp.CAUSA_PENAL_DELITO = delitos;
                        #endregion

                        #region NUC
                        //if (!string.IsNullOrEmpty(NUC))
                        //    cp.NUC = new NUC() { ID_NUC = NUC.Replace("NUC:", string.Empty) };
                        if (SelectedInterconexion != null)
                        {
                            cp.NUC = new NUC() { ID_NUC = SelectedInterconexion.EXPEDIENTEID.ToString(), ID_PERSONA_PG = SelectedInterconexion.PERSONAFISICAID };
                        }
                        #endregion

                        cp.ID_CAUSA_PENAL = new cCausaPenal().Insertar(cp);
                        if (cp.ID_CAUSA_PENAL > 0)
                        {
                            SelectedCausaPenal = cp;
                            //if (GuardarDelitoCausaPenal(cp.ID_CAUSA_PENAL))
                            //{
                                if (LstCausaPenalDelitos != null)
                                {
                                    //CARGAMOS LA LISTA DE DELITOS CON LOS DELITOS DE LA CAUSA PENAL EN LA SENTENCIA
                                    if (LstSentenciaDelitos == null)
                                        LstSentenciaDelitos = new ObservableCollection<SENTENCIA_DELITO>();
                                    foreach (var d in LstCausaPenalDelitos)
                                    {
                                        LstSentenciaDelitos.Add(
                                            new SENTENCIA_DELITO()
                                            {
                                                ID_DELITO = d.ID_DELITO,
                                                ID_FUERO = d.ID_FUERO,
                                                ID_MODALIDAD = d.ID_MODALIDAD,
                                                ID_TIPO_DELITO = d.ID_TIPO_DELITO,
                                                DESCR_DELITO = d.DESCR_DELITO,
                                                CANTIDAD = d.CANTIDAD,
                                                OBJETO = d.OBJETO,
                                                MODALIDAD_DELITO = d.MODALIDAD_DELITO,
                                                TIPO_DELITO = d.TIPO_DELITO
                                            });
                                    }
                                    if (LstSentenciaDelitos.Count > 0)
                                        SentenciaDelitoEmpty = false;
                                    else
                                        SentenciaDelitoEmpty = true;
                                }
                                
                                return true;
                        }
                        else
                            return false;
                    }
                    else//UPDATE
                    {
                        cp.ID_CENTRO = SelectedCausaPenal.ID_CENTRO;
                        cp.ID_ANIO = SelectedCausaPenal.ID_ANIO;
                        cp.ID_IMPUTADO = SelectedCausaPenal.ID_IMPUTADO;
                        cp.ID_INGRESO = SelectedCausaPenal.ID_INGRESO;
                        cp.ID_CAUSA_PENAL = SelectedCausaPenal.ID_CAUSA_PENAL;

                        #region Delitos
                        var delitos = new List<CAUSA_PENAL_DELITO>(LstCausaPenalDelitos == null ? null : LstCausaPenalDelitos.Select((w, i) => new CAUSA_PENAL_DELITO()
                        {
                            ID_CENTRO = SelectedCausaPenal.ID_CENTRO,
                            ID_ANIO = SelectedCausaPenal.ID_ANIO,
                            ID_IMPUTADO = SelectedCausaPenal.ID_IMPUTADO,
                            ID_INGRESO = SelectedCausaPenal.ID_INGRESO,
                            ID_CAUSA_PENAL = SelectedCausaPenal.ID_CAUSA_PENAL,
                            ID_DELITO = w.ID_DELITO,
                            ID_FUERO = w.ID_FUERO,
                            ID_MODALIDAD = w.ID_MODALIDAD,
                            ID_TIPO_DELITO = w.ID_TIPO_DELITO,
                            CANTIDAD = w.CANTIDAD,
                            OBJETO = w.OBJETO,
                            DESCR_DELITO = w.DESCR_DELITO,
                            ID_CONS = Convert.ToInt16(i + 1)
                        }));
                        #endregion 

                        #region NUC
                        //string sNUC = string.Empty;
                        //if (!string.IsNullOrEmpty(NUC))
                        //    sNUC = NUC.Replace("NUC:", string.Empty);
                        NUC nuc = null;
                        if(SelectedInterconexion != null)
                        {
                            nuc = new NUC() { ID_NUC = SelectedInterconexion.EXPEDIENTEID.ToString(), ID_PERSONA_PG = SelectedInterconexion.PERSONAFISICAID };
                        }
                        
                        #endregion

                        if (new cCausaPenal().Actualizar(cp,delitos,nuc))//sNUC))
                        {
                            //if (GuardarDelitoCausaPenal(cp.ID_CAUSA_PENAL))
                            //{
                                if (SelectedCausaPenal != null)
                                    if (SelectedCausaPenal.SENTENCIA != null)
                                    {
                                        if (SelectedCausaPenal.SENTENCIA.Count == 0)
                                        {
                                            if (LstCausaPenalDelitos != null)
                                            {
                                                //CARGAMOS LA LISTA DE DELITOS CON LOS DELITOS DE LA CAUSA PENAL EN LA SENTENCIA
                                                if (LstSentenciaDelitos == null)
                                                    LstSentenciaDelitos = new ObservableCollection<SENTENCIA_DELITO>();
                                                foreach (var d in LstCausaPenalDelitos)
                                                {
                                                    LstSentenciaDelitos.Add(
                                                        new SENTENCIA_DELITO()
                                                        {
                                                            ID_DELITO = d.ID_DELITO,
                                                            ID_FUERO = d.ID_FUERO,
                                                            ID_MODALIDAD = d.ID_MODALIDAD,
                                                            ID_TIPO_DELITO = d.ID_TIPO_DELITO,
                                                            DESCR_DELITO = d.DESCR_DELITO,
                                                            CANTIDAD = d.CANTIDAD,
                                                            OBJETO = d.OBJETO,
                                                            MODALIDAD_DELITO = d.MODALIDAD_DELITO,
                                                            TIPO_DELITO = d.TIPO_DELITO
                                                        });
                                                }
                                                if (LstSentenciaDelitos.Count > 0)
                                                    SentenciaDelitoEmpty = false;
                                                else
                                                    SentenciaDelitoEmpty = true;
                                            }
                                        }
                                    }
                                    else
                                    {
                                        if (LstCausaPenalDelitos != null)
                                        {
                                            //CARGAMOS LA LISTA DE DELITOS CON LOS DELITOS DE LA CAUSA PENAL EN LA SENTENCIA
                                            if (LstSentenciaDelitos == null)
                                                LstSentenciaDelitos = new ObservableCollection<SENTENCIA_DELITO>();
                                            foreach (var d in LstCausaPenalDelitos)
                                            {
                                                LstSentenciaDelitos.Add(
                                                    new SENTENCIA_DELITO()
                                                    {
                                                        ID_DELITO = d.ID_DELITO,
                                                        ID_FUERO = d.ID_FUERO,
                                                        ID_MODALIDAD = d.ID_MODALIDAD,
                                                        ID_TIPO_DELITO = d.ID_TIPO_DELITO,
                                                        DESCR_DELITO = d.DESCR_DELITO,
                                                        CANTIDAD = d.CANTIDAD,
                                                        OBJETO = d.OBJETO,
                                                        MODALIDAD_DELITO = d.MODALIDAD_DELITO,
                                                        TIPO_DELITO = d.TIPO_DELITO
                                                    });
                                            }
                                            if (LstSentenciaDelitos.Count > 0)
                                                SentenciaDelitoEmpty = false;
                                            else
                                                SentenciaDelitoEmpty = true;
                                        }
                                    }
                                //ActualizarNUC();
                                return true;
                            //}
                            //else
                            //    return false;
                        }
                        else
                            return false;
                    }
                }
                else
                    return false;
            }
            catch (Exception ex)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al guardar causa penal", ex);
                return false;
            }
        }

        private void PopulateCausaPenal()
        {
            try
            {
                #region Limpiar
                LimpiarCausaPenal();
                LimpiarCoparticipe();
                LimpiarSentencia();
                LimpiarRecusrsos();
                #endregion
                if (SelectedCausaPenal != null)
                {
                    #region Averiguacion Previa
                    AgenciaAP = SelectedCausaPenal.ID_AGENCIA != null ? SelectedCausaPenal.ID_AGENCIA : -1;
                    AnioAP = SelectedCausaPenal.AP_ANIO;
                    FolioAP = SelectedCausaPenal.AP_FOLIO;
                    AveriguacionPreviaAP = SelectedCausaPenal.AP_FORANEA;
                    FecAveriguacionAP = SelectedCausaPenal.AP_FEC_INICIO;
                    FecConsignacionAP = SelectedCausaPenal.AP_FEC_CONSIGNACION;
                    #endregion
                    
                    #region Causa Penal
                    AnioCP = SelectedCausaPenal.CP_ANIO;
                    FolioCP = SelectedCausaPenal.CP_FOLIO;
                    BisCP = SelectedCausaPenal.CP_BIS;
                    ForaneoCP = SelectedCausaPenal.CP_FORANEO;
                    TipoOrdenCP = SelectedCausaPenal.CP_TIPO_ORDEN != null ? SelectedCausaPenal.CP_TIPO_ORDEN : -1;
                    PaisJuzgadoCP = SelectedCausaPenal.CP_PAIS_JUZGADO != null ? SelectedCausaPenal.CP_PAIS_JUZGADO : -1;
                    EstadoJuzgadoCP = SelectedCausaPenal.CP_ESTADO_JUZGADO != null ? SelectedCausaPenal.CP_ESTADO_JUZGADO : -1;
                    MunicipioJuzgadoCP = SelectedCausaPenal.CP_MUNICIPIO_JUZGADO != null ? SelectedCausaPenal.CP_MUNICIPIO_JUZGADO : -1;
                    FueroCP = !string.IsNullOrEmpty(SelectedCausaPenal.CP_FUERO) ? SelectedCausaPenal.CP_FUERO : string.Empty;
                    JuzgadoCP = SelectedCausaPenal.CP_JUZGADO != null ? SelectedCausaPenal.CP_JUZGADO : -1;
                    FecRadicacionCP = SelectedCausaPenal.CP_FEC_RADICACION;
                    AmpliacionCP = !string.IsNullOrEmpty(SelectedCausaPenal.CP_AMPLIACION) ? SelectedCausaPenal.CP_AMPLIACION : string.Empty;
                    TerminoCP = SelectedCausaPenal.CP_TERMINO != null ? SelectedCausaPenal.CP_TERMINO : -1;
                    EstatusCP = SelectedCausaPenal.ID_ESTATUS_CP != null ? SelectedCausaPenal.ID_ESTATUS_CP : -1;
                    FecVencimientoTerinoCP = SelectedCausaPenal.CP_FEC_VENCIMIENTO_TERMINO;
                    ObservacionesCP = SelectedCausaPenal.OBSERV;
                    #endregion

                    #region Baja
                    //BFecha = SelectedCausaPenal.BAJA_FEC;
                    //BMotivoBaja = SelectedCausaPenal.ID_MOTIVO_BAJA != null ? SelectedCausaPenal.ID_MOTIVO_BAJA : -1;
                    //BAutoridadBaja = SelectedCausaPenal.ID_AUTO_BAJA != null ? SelectedCausaPenal.ID_AUTO_BAJA : -1;
                    if (SelectedCausaPenal.LIBERACION != null)
                    {
                        var l = SelectedCausaPenal.LIBERACION.FirstOrDefault();
                        if (l != null)
                        {
                            BFecha = l.LIBERACION_FEC;
                            BLiberacionMotivo = l.ID_LIBERACION_MOTIVO.Value;
                            BLiberacionAutoridad = l.ID_LIBERACION_AUTORIDAD.Value;
                        }
                    }
                    #endregion

                    #region Coparticipes
                    if (SelectedCausaPenal != null)
                        LstCoparticipe = new ObservableCollection<COPARTICIPE>(SelectedCausaPenal.COPARTICIPE);
                    #endregion

                    #region Delitos Causa Penal
                    PopulateDelitoCausaPenal();
                    #endregion

                    #region Sentencia
                    if (SelectedCausaPenal.SENTENCIA != null)
                    {
                        var lst = SelectedCausaPenal.SENTENCIA.Where(w => w.ESTATUS == "A");
                        if (lst.Count() > 0)
                        {
                            SelectedSentencia = lst.FirstOrDefault();
                            PopulateSentencia();
                        }
                        else
                        {
                            SelectedSentencia = null;
                            //CARGAMOS LA LISTA DE DELITOS CON LOS DELITOS DE LA CAUSA PENAL EN LA SENTENCIA
                            if (LstSentenciaDelitos == null)
                                LstSentenciaDelitos = new ObservableCollection<SENTENCIA_DELITO>();
                            foreach (var d in LstCausaPenalDelitos)
                            {
                                LstSentenciaDelitos.Add(
                                    new SENTENCIA_DELITO()
                                    {
                                        ID_DELITO = d.ID_DELITO,
                                        ID_FUERO = d.ID_FUERO,
                                        ID_MODALIDAD = d.ID_MODALIDAD,
                                        ID_TIPO_DELITO = d.ID_TIPO_DELITO,
                                        DESCR_DELITO = d.DESCR_DELITO,
                                        CANTIDAD = d.CANTIDAD,
                                        OBJETO = d.OBJETO,
                                        MODALIDAD_DELITO = d.MODALIDAD_DELITO,
                                        TIPO_DELITO = d.TIPO_DELITO
                                    });
                            }
                            if (LstSentenciaDelitos.Count > 0)
                                SentenciaDelitoEmpty = false;
                            else
                                SentenciaDelitoEmpty = true;
                        }
                    }
                    #endregion

                    #region NUC
                    PopulateNUC();
                    #endregion
                }
            }
            catch (Exception ex)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al establecer causa penal", ex);
            }
        }
    
        private void PopulateNUC(){
            NUC = string.Empty;
            SelectedInterconexion = null;
            if (SelectedCausaPenal != null)
            { 
                if(SelectedCausaPenal.NUC != null)
                {
                    SelectedInterconexion = new VM_IMPUTADOSDATOS() { EXPEDIENTEID = long.Parse(SelectedCausaPenal.NUC.ID_NUC), PERSONAFISICAID = SelectedCausaPenal.NUC.ID_PERSONA_PG != null ? SelectedCausaPenal.NUC.ID_PERSONA_PG.Value : 0 };
                    NUC = string.Format("NUC: {0}", SelectedCausaPenal.NUC.ID_NUC.Trim());
                }
            }
        }

        private void ActualizarNUC()
        {
            if (SelectedCausaPenal != null)
            {
                if (SelectedCausaPenal.NUC != null)//EDITAR
                {
                    if (!string.IsNullOrEmpty(NUC))
                    {
                        var n = new NUC()
                              {
                                  ID_CENTRO = SelectedCausaPenal.ID_CENTRO,
                                  ID_ANIO = SelectedCausaPenal.ID_ANIO,
                                  ID_IMPUTADO = SelectedCausaPenal.ID_IMPUTADO,
                                  ID_INGRESO = SelectedCausaPenal.ID_INGRESO,
                                  ID_CAUSA_PENAL = SelectedCausaPenal.ID_CAUSA_PENAL,
                                  ID_NUC = NUC.Replace("NUC:",string.Empty)
                              };
                        if (new cNuc().Actualizar(n))
                            SelectedCausaPenal.NUC = n;
                            
                              
                    }
                    else//ELIMINAR
                    {
                        if (new cNuc().Eliminar(SelectedCausaPenal.ID_CENTRO, SelectedCausaPenal.ID_ANIO, SelectedCausaPenal.ID_IMPUTADO, SelectedCausaPenal.ID_INGRESO, SelectedCausaPenal.ID_CAUSA_PENAL))
                            SelectedCausaPenal.NUC = null;
                    }
                }
                else //INSERTAR
                {
                    var n = new NUC()
                               {
                                   ID_CENTRO = SelectedCausaPenal.ID_CENTRO,
                                   ID_ANIO = SelectedCausaPenal.ID_ANIO,
                                   ID_IMPUTADO = SelectedCausaPenal.ID_IMPUTADO,
                                   ID_INGRESO = SelectedCausaPenal.ID_INGRESO,
                                   ID_CAUSA_PENAL = SelectedCausaPenal.ID_CAUSA_PENAL,
                                   ID_NUC =  !string.IsNullOrEmpty(NUC) ? NUC.Replace("NUC:", string.Empty) : string.Empty
                               };
                    if (new cNuc().Insertar(n))
                        SelectedCausaPenal.NUC = n;
                        
                }
            }
        }
    }//clase
}//namespac