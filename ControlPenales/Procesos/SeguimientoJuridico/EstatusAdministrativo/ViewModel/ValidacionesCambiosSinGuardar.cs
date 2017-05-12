using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ControlPenales
{
    partial class EstatusAdministrativoViewModel
    {
        private async Task<bool> CambiosSinGuardar()
        {
            await this.getDatosGenerales();
            StaticSourcesViewModel.CargarDatosMetodo(this.getDatosTraslados);
            this.getDatosGeneralesIdentificacion();
            this.getDatosMediaFiliacion();
            if (Imputado != null && Ingreso != null)
            {
                if (CambiosSinGuardarIngreso()) return true;
                if (CambiosSinGuardarIdentificacion()) return true;
                if (CambiosSinGuardarApodosAlias()) return true;
                if (CambiosSinGuardarFotosHuellas()) return true;
                if (CambiosSinGuardarMediaFiliacion()) return true;
                if (CambiosSinGuardarTraslado()) return true;
            }
            return false;
        }
        private bool CambiosSinGuardarIngreso()
        {
            #region Datos Ingreso
            if (SelectTipoIngreso != Ingreso.ID_TIPO_INGRESO)
                return true;
            if (IngresoDelito != Ingreso.ID_INGRESO_DELITO)
                return true;
            if (SelectTipoDisposicion != Ingreso.ID_DISPOSICION)
                return true;
            if (SelectTipoAutoridadInterna != Ingreso.ID_AUTORIDAD_INTERNA)
                return true;
            if (SelectTipoSeguridad != Ingreso.ID_TIPO_SEGURIDAD)
                return true;
            if (TextNumeroOficio != Ingreso.DOCINTERNACION_NUM_OFICIO)
                return true;
            if (SelectedCama.ID_CAMA != Ingreso.ID_UB_CAMA)
                return true;
            #endregion
            return false;
        }
        private bool CambiosSinGuardarIdentificacion()
        {
            var ingreso = Imputado.INGRESO.OrderByDescending(w => w.ID_INGRESO).FirstOrDefault();
            #region Identificacion
            if (SelectEtnia != Imputado.ID_ETNIA)
                return true;
            if (SelectEscolaridad != ingreso.ID_ESCOLARIDAD)
                return true;
            if (SelectOcupacion != ingreso.ID_OCUPACION)
                return true;
            if (SelectEstadoCivil != ingreso.ID_ESTADO_CIVIL)
                return true;
            if (SelectNacionalidad != Imputado.ID_NACIONALIDAD)
                return true;
            if (SelectReligion != ingreso.ID_RELIGION)
                return true;
            if (SelectSexo != Imputado.SEXO)
                return true;
            if (SelectPais != ingreso.ID_PAIS)
                return true;
            if (SelectEntidad != ingreso.ID_ENTIDAD)
                return true;
            if (SelectMunicipio != ingreso.ID_MUNICIPIO)
                return true;
            if (SelectColoniaItem.ID_COLONIA != ingreso.ID_COLONIA)
                return true;
            if (TextCalle != ingreso.DOMICILIO_CALLE)
                return true;
            if (TextNumeroExterior != ingreso.DOMICILIO_NUM_EXT)
                return true;
            if (TextNumeroInterior != ingreso.DOMICILIO_NUM_INT)
                return true;
            if (ingreso.RESIDENCIA_ANIOS != null && AniosEstado != ingreso.RESIDENCIA_ANIOS.ToString())
                return true;
            if (ingreso.RESIDENCIAS_MESES != null && MesesEstado != ingreso.RESIDENCIAS_MESES.ToString())
                return true;
            if (TextTelefono != (ingreso.TELEFONO.HasValue ? ingreso.TELEFONO.ToString() : null))
                return true;
            if (TextCodigoPostal != ingreso.DOMICILIO_CP)
                return true;
            if (TextDomicilioTrabajo != ingreso.DOMICILIO_TRABAJO)
                return true;
            if (Imputado.IMPUTADO_PADRES.Count > 0)
            {
                if (!CheckPadreFinado)
                {
                    #region DomicilioPadre
                    foreach (var item in Imputado.IMPUTADO_PADRES)
                    {
                        if (item.ID_PADRE == "P")
                        {
                            if (TextCalleDomicilioPadre != item.CALLE)
                                return true;
                            if (TextNumeroExteriorDomicilioPadre != item.NUM_EXT)
                                return true;
                            if (TextNumeroInteriorDomicilioPadre != item.NUM_INT)
                                return true;
                            if (TextCodigoPostalDomicilioPadre != item.CP)
                                return true;
                            if (SelectPaisDomicilioPadre != item.ID_PAIS)
                                return true;
                            if (SelectEntidadDomicilioPadre != item.ID_ENTIDAD)
                                return true;
                            if (SelectMunicipioDomicilioPadre != item.ID_MUNICIPIO)
                                return true;
                            if (SelectColoniaDomicilioPadre != item.ID_COLONIA)
                                return true;
                        }
                    }
                    #endregion
                }
                if (!CheckMadreFinado)
                {
                    #region DomicilioMadre
                    foreach (var item in Imputado.IMPUTADO_PADRES)
                    {
                        if (item.ID_PADRE == "M")
                        {
                            if (TextCalleDomicilioMadre != item.CALLE)
                                return true;
                            if (TextNumeroExteriorDomicilioMadre != item.NUM_EXT)
                                return true;
                            if (TextNumeroInteriorDomicilioMadre != item.NUM_INT)
                                return true;
                            if (TextCodigoPostalDomicilioMadre != item.CP)
                                return true;
                            if (SelectPaisDomicilioMadre != item.ID_PAIS)
                                return true;
                            if (SelectEntidadDomicilioMadre != item.ID_ENTIDAD)
                                return true;
                            if (SelectMunicipioDomicilioMadre != item.ID_MUNICIPIO)
                                return true;
                            if (SelectColoniaDomicilioMadre != item.ID_COLONIA)
                                return true;
                        }
                    }
                    #endregion
                }
            }
            #endregion
            return false;
        }
        private bool CambiosSinGuardarApodosAlias()
        {
            /*ListAlias = new System.Collections.ObjectModel.ObservableCollection<SSP.Servidor.ALIAS>(Imputado.Aliases);
            ListApodo = new System.Collections.ObjectModel.ObservableCollection<SSP.Servidor.APODO>(Imputado.APODOes);
            ListRelacionPersonalInterno = new System.Collections.ObjectModel.ObservableCollection<SSP.Servidor.RELACION_PERSONAL_INTERNO>(Imputado.RELACION_PERSONAL_INTERNO);*/

            #region ApodosAlias
            foreach (var item in Imputado.APODO)
            {
                if (!ListApodo.Where(w => w.ID_APODO == item.ID_APODO).Any())
                    return true;
            }
            foreach (var item in Imputado.ALIAS)
            {
                if (!ListAlias.Where(w => w.ID_ALIAS == item.ID_ALIAS).Any())
                    return true;
            }
            foreach (var item in Imputado.RELACION_PERSONAL_INTERNO)
            {
                if (!ListRelacionPersonalInterno.Where(w => w.ID_REL_ANIO == item.ID_REL_ANIO && w.ID_REL_CENTRO == item.ID_REL_CENTRO && w.ID_REL_IMPUTADO == item.ID_REL_IMPUTADO && w.ID_REL_INGRESO == item.ID_REL_INGRESO).Any())
                    return true;
            }
            #endregion
            return false;
        }
        private bool CambiosSinGuardarFotosHuellas()
        {
            #region FotosHuellas
            //CHECAR SI LAS IMAGENES SON IGUALES
            #endregion
            return false;
        }
        private bool CambiosSinGuardarMediaFiliacion()
        {
            #region MediaFiliacion
            if (SelectComplexion != Imputado.IMPUTADO_FILIACION.Where(w => w.ID_MEDIA_FILIACION == 39).FirstOrDefault().ID_TIPO_FILIACION) return true;
            if (SelectColorPiel != Imputado.IMPUTADO_FILIACION.Where(w => w.ID_MEDIA_FILIACION == 30).FirstOrDefault().ID_TIPO_FILIACION) return true;
            if (SelectCara != Imputado.IMPUTADO_FILIACION.Where(w => w.ID_MEDIA_FILIACION == 7).FirstOrDefault().ID_TIPO_FILIACION) return true;

            if (SelectTipoSangre != Imputado.IMPUTADO_FILIACION.Where(w => w.ID_MEDIA_FILIACION == 22).FirstOrDefault().ID_TIPO_FILIACION) return true;
            if (SelectFactorSangre != Imputado.IMPUTADO_FILIACION.Where(w => w.ID_MEDIA_FILIACION == 23).FirstOrDefault().ID_TIPO_FILIACION) return true;

            if (SelectCantidadCabello != Imputado.IMPUTADO_FILIACION.Where(w => w.ID_MEDIA_FILIACION == 8).FirstOrDefault().ID_TIPO_FILIACION) return true;
            if (SelectColorCabello != Imputado.IMPUTADO_FILIACION.Where(w => w.ID_MEDIA_FILIACION == 9).FirstOrDefault().ID_TIPO_FILIACION) return true;
            if (SelectCalvicieCabello != Imputado.IMPUTADO_FILIACION.Where(w => w.ID_MEDIA_FILIACION == 10).FirstOrDefault().ID_TIPO_FILIACION) return true;
            if (SelectFormaCabello != Imputado.IMPUTADO_FILIACION.Where(w => w.ID_MEDIA_FILIACION == 11).FirstOrDefault().ID_TIPO_FILIACION) return true;
            if (SelectImplantacionCabello != Imputado.IMPUTADO_FILIACION.Where(w => w.ID_MEDIA_FILIACION == 31).FirstOrDefault().ID_TIPO_FILIACION) return true;

            if (SelectDireccionCeja != Imputado.IMPUTADO_FILIACION.Where(w => w.ID_MEDIA_FILIACION == 12).FirstOrDefault().ID_TIPO_FILIACION) return true;
            if (SelectImplantacionCeja != Imputado.IMPUTADO_FILIACION.Where(w => w.ID_MEDIA_FILIACION == 13).FirstOrDefault().ID_TIPO_FILIACION) return true;
            if (SelectFormaCeja != Imputado.IMPUTADO_FILIACION.Where(w => w.ID_MEDIA_FILIACION == 14).FirstOrDefault().ID_TIPO_FILIACION) return true;
            if (SelectTamanioCeja != Imputado.IMPUTADO_FILIACION.Where(w => w.ID_MEDIA_FILIACION == 15).FirstOrDefault().ID_TIPO_FILIACION) return true;

            if (SelectAlturaFrente != Imputado.IMPUTADO_FILIACION.Where(w => w.ID_MEDIA_FILIACION == 27).FirstOrDefault().ID_TIPO_FILIACION) return true;
            if (SelectInclinacionFrente != Imputado.IMPUTADO_FILIACION.Where(w => w.ID_MEDIA_FILIACION == 28).FirstOrDefault().ID_TIPO_FILIACION) return true;
            if (SelectAnchoFrente != Imputado.IMPUTADO_FILIACION.Where(w => w.ID_MEDIA_FILIACION == 29).FirstOrDefault().ID_TIPO_FILIACION) return true;

            if (SelectColorOjos != Imputado.IMPUTADO_FILIACION.Where(w => w.ID_MEDIA_FILIACION == 16).FirstOrDefault().ID_TIPO_FILIACION) return true;
            if (SelectFormaOjos != Imputado.IMPUTADO_FILIACION.Where(w => w.ID_MEDIA_FILIACION == 17).FirstOrDefault().ID_TIPO_FILIACION) return true;
            if (SelectTamanioOjos != Imputado.IMPUTADO_FILIACION.Where(w => w.ID_MEDIA_FILIACION == 18).FirstOrDefault().ID_TIPO_FILIACION) return true;

            if (SelectRaizNariz != Imputado.IMPUTADO_FILIACION.Where(w => w.ID_MEDIA_FILIACION == 1).FirstOrDefault().ID_TIPO_FILIACION) return true;
            if (SelectDorsoNariz != Imputado.IMPUTADO_FILIACION.Where(w => w.ID_MEDIA_FILIACION == 3).FirstOrDefault().ID_TIPO_FILIACION) return true;
            if (SelectAnchoNariz != Imputado.IMPUTADO_FILIACION.Where(w => w.ID_MEDIA_FILIACION == 4).FirstOrDefault().ID_TIPO_FILIACION) return true;
            if (SelectBaseNariz != Imputado.IMPUTADO_FILIACION.Where(w => w.ID_MEDIA_FILIACION == 5).FirstOrDefault().ID_TIPO_FILIACION) return true;
            if (SelectAlturaNariz != Imputado.IMPUTADO_FILIACION.Where(w => w.ID_MEDIA_FILIACION == 6).FirstOrDefault().ID_TIPO_FILIACION) return true;

            if (SelectAlturaLabio != Imputado.IMPUTADO_FILIACION.Where(w => w.ID_MEDIA_FILIACION == 32).FirstOrDefault().ID_TIPO_FILIACION) return true;
            if (SelectProminenciaLabio != Imputado.IMPUTADO_FILIACION.Where(w => w.ID_MEDIA_FILIACION == 33).FirstOrDefault().ID_TIPO_FILIACION) return true;
            if (SelectEspesorLabio != Imputado.IMPUTADO_FILIACION.Where(w => w.ID_MEDIA_FILIACION == 21).FirstOrDefault().ID_TIPO_FILIACION) return true;

            if (SelectTamanioBoca != Imputado.IMPUTADO_FILIACION.Where(w => w.ID_MEDIA_FILIACION == 19).FirstOrDefault().ID_TIPO_FILIACION) return true;
            if (SelectComisuraBoca != Imputado.IMPUTADO_FILIACION.Where(w => w.ID_MEDIA_FILIACION == 20).FirstOrDefault().ID_TIPO_FILIACION) return true;

            if (SelectTipoMenton != Imputado.IMPUTADO_FILIACION.Where(w => w.ID_MEDIA_FILIACION == 24).FirstOrDefault().ID_TIPO_FILIACION) return true;
            if (SelectFormaMenton != Imputado.IMPUTADO_FILIACION.Where(w => w.ID_MEDIA_FILIACION == 25).FirstOrDefault().ID_TIPO_FILIACION) return true;
            if (SelectInclinacionMenton != Imputado.IMPUTADO_FILIACION.Where(w => w.ID_MEDIA_FILIACION == 26).FirstOrDefault().ID_TIPO_FILIACION) return true;

            if (SelectFormaOrejaDerecha != Imputado.IMPUTADO_FILIACION.Where(w => w.ID_MEDIA_FILIACION == 34).FirstOrDefault().ID_TIPO_FILIACION) return true;
            if (SelectHelixOriginalOrejaDerecha != Imputado.IMPUTADO_FILIACION.Where(w => w.ID_MEDIA_FILIACION == 40).FirstOrDefault().ID_TIPO_FILIACION) return true;
            if (SelectHelixSuperiorOrejaDerecha != Imputado.IMPUTADO_FILIACION.Where(w => w.ID_MEDIA_FILIACION == 41).FirstOrDefault().ID_TIPO_FILIACION) return true;
            if (SelectHelixPosteriorOrejaDerecha != Imputado.IMPUTADO_FILIACION.Where(w => w.ID_MEDIA_FILIACION == 42).FirstOrDefault().ID_TIPO_FILIACION) return true;
            if (SelectHelixAdherenciaOrejaDerecha != Imputado.IMPUTADO_FILIACION.Where(w => w.ID_MEDIA_FILIACION == 43).FirstOrDefault().ID_TIPO_FILIACION) return true;
            if (SelectLobuloContornoOrejaDerecha != Imputado.IMPUTADO_FILIACION.Where(w => w.ID_MEDIA_FILIACION == 44).FirstOrDefault().ID_TIPO_FILIACION) return true;
            if (SelectLobuloAdherenciaOrejaDerecha != Imputado.IMPUTADO_FILIACION.Where(w => w.ID_MEDIA_FILIACION == 45).FirstOrDefault().ID_TIPO_FILIACION) return true;
            if (SelectLobuloParticularidadOrejaDerecha != Imputado.IMPUTADO_FILIACION.Where(w => w.ID_MEDIA_FILIACION == 46).FirstOrDefault().ID_TIPO_FILIACION) return true;
            if (SelectLobuloDimensionOrejaDerecha != Imputado.IMPUTADO_FILIACION.Where(w => w.ID_MEDIA_FILIACION == 47).FirstOrDefault().ID_TIPO_FILIACION) return true;
            #endregion
            return false;
        }
        private bool CambiosSinGuardarTraslado()
        {
            #region Traslado
            //if (TextFechaTraslado != Fechas.GetFechaDateServer)
            //    return true;
            //if (!string.IsNullOrEmpty(TextAnioAnterior))
            //    return true;
            //if (!string.IsNullOrEmpty(TextFolioAnterior))
            //    return true;
            //if (!string.IsNullOrEmpty(TextOrigenForaneo))
            //    return true;
            //if (!string.IsNullOrEmpty(TextUbicacionForaneo))
            //    return true;
            //if (!string.IsNullOrEmpty(TextOficioAutorizacion))
            //    return true;
            //if (!string.IsNullOrEmpty(TextCustodioTraslado))
            //    return true;
            //if (SelectMotivoTraslado != -1)
            //    return true;
            //if (SelectCentroOrigen != -1)
            //    return true;
            //if (SelectedAutorizado == null || SelectedAutorizado.ID_AUTORIDAD_INTERNA != -1)
            //    return true;
            #endregion
            return false;
        }
    }
}
