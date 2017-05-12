using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ControlPenales
{
    partial class RegistroIngresoViewModel
    {
        private bool CambiosSinGuardar()
        {
            if (SelectExpediente != null)
            {
                #region Datos Ingreso
                if (SelectTipoIngreso != null && SelectTipoIngreso != -1)
                    return true;
                if (IngresoDelito != -1)
                    return true;
                if (SelectTipoDisposicion != null && SelectTipoDisposicion != -1)
                    return true;
                if (SelectTipoAutoridadInterna != null && SelectTipoAutoridadInterna != -1)
                    return true;
                if (!string.IsNullOrEmpty(SelectTipoSeguridad))
                    return true;
                if (!string.IsNullOrEmpty(TextNumeroOficio))
                    return true;
                if (SelectedCama != null && SelectedCama.ID_CAMA > 0)
                    return true;
                #endregion

                #region Identificacion
                var ingreso = SelectExpediente.INGRESO.OrderByDescending(w => w.ID_INGRESO).FirstOrDefault();

                if (SelectEtnia != SelectExpediente.ID_ETNIA)
                    return true;
                if (SelectEscolaridad != ingreso.ID_ESCOLARIDAD)
                    return true;
                if (SelectOcupacion != ingreso.ID_OCUPACION)
                    return true;
                if (SelectEstadoCivil != ingreso.ID_ESTADO_CIVIL)
                    return true;
                if (SelectNacionalidad != SelectExpediente.ID_NACIONALIDAD)
                    return true;
                if (SelectReligion != ingreso.ID_RELIGION)
                    return true;
                if (SelectSexo != SelectExpediente.SEXO)
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
                if (long.Parse(TextTelefono.Replace(" ", "").Replace("-", "").Replace("(", "").Replace(")", "")) != ingreso.TELEFONO)
                    return true;
                if (TextCodigoPostal != ingreso.DOMICILIO_CP)
                    return true;
                if (TextDomicilioTrabajo != ingreso.DOMICILIO_TRABAJO)
                    return true;
                if (SelectExpediente.IMPUTADO_PADRES.Count > 0)
                {
                    if (!CheckPadreFinado)
                    {
                        #region DomicilioPadre
                        foreach (var item in SelectExpediente.IMPUTADO_PADRES)
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
                        foreach (var item in SelectExpediente.IMPUTADO_PADRES)
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

                #region ApodosAlias
                foreach (var item in SelectExpediente.APODO)
                {
                    if (!ListApodo.Where(w => w.ID_APODO == item.ID_APODO).Any())
                        return true;
                }
                foreach (var item in SelectExpediente.ALIAS)
                {
                    if (!ListAlias.Where(w => w.ID_ALIAS == item.ID_ALIAS).Any())
                        return true;
                }
                foreach (var item in SelectExpediente.RELACION_PERSONAL_INTERNO)
                {
                    if (ListRelacionPersonalInterno.Where(w => w.ID_REL_ANIO == item.ID_REL_ANIO && w.ID_REL_CENTRO == item.ID_REL_CENTRO && w.ID_REL_IMPUTADO == item.ID_REL_IMPUTADO && w.ID_REL_INGRESO == item.ID_REL_INGRESO).Any())
                        return true;
                }
                #endregion

                #region FotosHuellas
                //CHECAR SI LAS IMAGENES SON IGUALES
                #endregion

                #region Traslado
                //if (TextFechaTraslado != null && TextFechaTraslado != Fechas.GetFechaDateServer)
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
                //if (SelectedAutorizado.ID_AUTORIDAD_INTERNA != -1)
                //    return true;
                #endregion
            }
            return false;
        }
    }
}
