using System;
using System.Collections.Generic;
using System.Linq;
using SSP.Controlador.Catalogo.Justicia;
using System.Collections.ObjectModel;
using SSP.Servidor;
using System.Windows;
using ControlPenales.Clases;
using System.Threading;
using System.Windows.Controls;
using Cogent.Biometrics;
using System.Runtime.InteropServices;
using DPUruNet;
using System.IO;
using System.Drawing;
using System.Threading.Tasks;
using ControlPenales.BiometricoServiceReference;

namespace ControlPenales
{
    partial class RecepcionAduanaViewModel : FingerPrintScanner
    {
        public RecepcionAduanaViewModel() { }

        private bool ValidarVisitaIntima(VISITANTE_INGRESO item)
        {
            var HoySemana = Convert.ToInt32(Fechas.GetFechaDateServer.DayOfWeek) + 1;
            var VisitaEdificio = new cVisitaEdificio().ObtenerTodosActivos(item.INGRESO.ID_UB_CENTRO.Value, HoySemana);
            if (!VisitaEdificio.Where(w => (w.CELDA_INICIO == item.INGRESO.ID_UB_CELDA || w.CELDA_FINAL == item.INGRESO.ID_UB_CELDA) &&
                w.ID_SECTOR == item.INGRESO.ID_UB_SECTOR && w.ID_EDIFICIO == item.INGRESO.ID_UB_EDIFICIO && w.ID_TIPO_VISITA == ParametroVisitaIntima).Any())
            {
                var celdas = new cCelda().ObtenerPorSector(item.INGRESO.ID_UB_SECTOR, item.INGRESO.ID_UB_EDIFICIO,
                                item.INGRESO.ID_UB_CENTRO.Value).OrderBy(o => o.ID_CELDA);
                foreach (var itemVisita in VisitaEdificio.Where(w => w.ID_SECTOR == item.INGRESO.ID_UB_SECTOR && w.ID_EDIFICIO == item.INGRESO.ID_UB_EDIFICIO && w.ID_TIPO_VISITA == ParametroVisitaIntima))
                {
                    if (celdas.ToList().IndexOf(celdas.Where(w => w.ID_CELDA == item.INGRESO.ID_UB_CELDA).FirstOrDefault()) >=
                        celdas.ToList().IndexOf(celdas.Where(w => w.ID_CELDA == itemVisita.CELDA_INICIO).FirstOrDefault()) &&
                        celdas.ToList().IndexOf(celdas.Where(w => w.ID_CELDA == item.INGRESO.ID_UB_CELDA).FirstOrDefault()) <=
                        celdas.ToList().IndexOf(celdas.Where(w => w.ID_CELDA == itemVisita.CELDA_FINAL).FirstOrDefault()))
                    {
                        //SI ES DIA DE VISITA INTIMA
                        BanderaIntima = true;
                        AreaFamiliar = itemVisita.ID_AREA.HasValue ? itemVisita.ID_AREA.Value : (short)0;
                        return true;
                    }
                    AreaFamiliar = 0;
                    BanderaIntima = false;
                }
            }
            else return true;

            foreach (var itm in item.INGRESO.VISITA_AGENDA.Where(w => w.ESTATUS == "0" && w.ID_TIPO_VISITA == ParametroVisitaIntima))
            {
                if (itm.ID_DIA == HoySemana)
                {
                    AreaFamiliar = itm.ID_AREA.HasValue ? itm.ID_AREA.Value : (short)0;
                    return true;
                }
            }
            var letra = item.INGRESO.IMPUTADO.PATERNO[0].ToString();
            foreach (var itm in new cVisitaApellido().ObtenerTodosActivos(item.ID_CENTRO, new Nullable<int>()).Where(w => w.ID_TIPO_VISITA == ParametroVisitaIntima))
            {
                if (itm.ID_DIA == HoySemana && ((itm.LETRA_INICIAL == letra || itm.LETRA_FINAL == letra) ||
                    (ListLetras.IndexOf(itm.LETRA_INICIAL) < ListLetras.IndexOf(letra) && (ListLetras.IndexOf(itm.LETRA_FINAL) > ListLetras.IndexOf(letra)))))
                {
                    AreaFamiliar = itm.ID_AREA.HasValue ? itm.ID_AREA.Value : (short)0;
                    return true;
                }
            }
            return false;
        }

        private bool? ValidarVisitaEdificio(INGRESO ingreso, bool PorEdificio, List<VISITA_EDIFICIO> VisitaEdificio, short VisitaIntima, short VisitaAutorizada, short PersonaVisita)
        {
            var ahora = Fechas.GetFechaDateServer;
            if (PorEdificio)
            {
                if (!VisitaEdificio.Where(w => (w.CELDA_INICIO == ingreso.ID_UB_CELDA || w.CELDA_FINAL == ingreso.ID_UB_CELDA) &&
                    w.ID_SECTOR == ingreso.ID_UB_SECTOR && w.ID_EDIFICIO == ingreso.ID_UB_EDIFICIO).Any())
                {
                    var celdas = new cCelda().ObtenerPorSector(ingreso.ID_UB_SECTOR, ingreso.ID_UB_EDIFICIO,
                                    ingreso.ID_UB_CENTRO.Value).OrderBy(o => o.ID_CELDA);
                    foreach (var itemVisita in VisitaEdificio.Where(w => w.ID_SECTOR == ingreso.ID_UB_SECTOR &&
                                    w.ID_EDIFICIO == ingreso.ID_UB_EDIFICIO))
                    {
                        if (celdas.ToList().IndexOf(celdas.First(w => w.ID_CELDA == ingreso.ID_UB_CELDA)) >=
                            celdas.ToList().IndexOf(celdas.First(w => w.ID_CELDA == itemVisita.CELDA_INICIO)) &&
                            celdas.ToList().IndexOf(celdas.First(w => w.ID_CELDA == ingreso.ID_UB_CELDA)) <=
                            celdas.ToList().IndexOf(celdas.First(w => w.ID_CELDA == itemVisita.CELDA_FINAL)))
                        {
                            //SI ES DIA DE VISITA
                            AreaFamiliar = itemVisita.ID_AREA.HasValue ? itemVisita.ID_AREA.Value : (short)0;
                            SelectAreaDestinoAuxiliar = itemVisita.ID_AREA.HasValue ? itemVisita.ID_AREA.Value : (short)0;
                            if (itemVisita.ID_TIPO_VISITA == VisitaIntima)
                            {
                                BanderaIntima = true;
                                //checar que la ultima visita haya sido hace dos semanas
                                if (((TimeSpan)(ahora - new cAduana().ObtenerUltimaEntradaIntima(SelectPersona.ID_PERSONA, PersonaVisita,
                                    VisitaIntima, VisitaAutorizada).ENTRADA_FEC)).TotalDays >= 10)
                                {
                                    /*puede pasar*/
                                    return true;
                                }
                                else
                                {
                                    //no puede pasar
                                    (new Dialogos()).ConfirmacionDialogo("Advertencia!", "No se puede realizar esta visita.");
                                    return null;
                                }
                            }
                            else
                            {
                                BanderaIntima = false;
                                return true;
                            }
                        }
                    }
                }
                else
                    return true;
            }
            else
                return false;
            return false;
        }

        private bool? ValidarDiaVisita(VISITANTE_INGRESO item)
        {
            try
            {
                var ahora = Fechas.GetFechaDateServer;
                var hoy = Convert.ToInt32(ahora.DayOfWeek) + 1;
                var PorAgenda = item.INGRESO.VISITA_AGENDA.Where(w => w.ESTATUS == "0" && w.ID_DIA == hoy);
                if (PorAgenda.Any())
                {
                    AreaFamiliar = PorAgenda.FirstOrDefault().ID_AREA.HasValue ? PorAgenda.FirstOrDefault().ID_AREA.Value : (short)0;
                    SelectAreaDestinoAuxiliar = PorAgenda.FirstOrDefault().ID_AREA.HasValue ? PorAgenda.FirstOrDefault().ID_AREA.Value : (short)0;
                    return true;
                }
                else
                {
                    if (ParametroTipoVisitaPorCentro == (short)enumTipoVisitaPorCentro.APELLIDO)
                    {
                        #region APELLIDO
                        var letra = item.INGRESO.IMPUTADO.PATERNO[0].ToString();
                        var VisitaApellido = new cVisitaApellido().ObtenerTodosActivos(item.ID_CENTRO, new Nullable<int>());
                        foreach (var itm in VisitaApellido)
                        {
                            if (itm.ID_DIA == hoy && ((itm.LETRA_INICIAL == letra || itm.LETRA_FINAL == letra) ||
                                (ListLetras.IndexOf(itm.LETRA_INICIAL) < ListLetras.IndexOf(letra) && (ListLetras.IndexOf(itm.LETRA_FINAL) > ListLetras.IndexOf(letra)))))
                            {
                                AreaFamiliar = itm.ID_AREA.HasValue ? itm.ID_AREA.Value : (short)0;
                                SelectAreaDestinoAuxiliar = itm.ID_AREA.HasValue ? itm.ID_AREA.Value : (short)0;
                                if (item.ID_TIPO_VISITANTE == ParametroTipoVisitanteIntima)
                                {
                                    if (itm.ID_TIPO_VISITA == ParametroVisitaIntima)
                                    {
                                        BanderaIntima = true;
                                        //checar que la ultima visita haya sido hace dos semanas
                                        var UltimaEntrada = new cAduana().ObtenerUltimaEntradaIntima(SelectPersona.ID_PERSONA, ParametroPersonaVisita,
                                            ParametroVisitaIntima, ParametroVisitaAutorizada);
                                        if (UltimaEntrada != null ? ((TimeSpan)(ahora - UltimaEntrada.ENTRADA_FEC)).TotalDays >= 10 : true)
                                        {
                                            return true;
                                        }
                                        else
                                        {
                                            //no puede pasar
                                            AreaFamiliar = 0;
                                            //(new Dialogos()).ConfirmacionDialogo("Advertencia!", "No se puede realizar esta visita.");
                                            return null;
                                        }
                                    }
                                    else
                                    {
                                        BanderaIntima = false;
                                        continue;
                                    }
                                }
                                else
                                {
                                    if (itm.ID_TIPO_VISITA == ParametroVisitaIntima)
                                    {
                                        BanderaIntima = false;
                                        continue;
                                    }
                                    else
                                    {
                                        BanderaIntima = false;
                                        return true;
                                    }
                                }
                            }
                        }
                        return false;
                        #endregion
                    }
                    else if (ParametroTipoVisitaPorCentro == (short)enumTipoVisitaPorCentro.EDIFICIO)
                    {
                        #region EDIFICIO
                        var VisitaEdificio = new cVisitaEdificio().ObtenerTodosActivos(item.INGRESO.ID_UB_CENTRO.Value, hoy);
                        var PorEdificio = VisitaEdificio.Count() > 0;
                        if (PorEdificio)
                        {
                            var bandera = ValidarVisitaEdificio(item.INGRESO, PorEdificio, VisitaEdificio.ToList(), ParametroVisitaIntima, ParametroVisitaAutorizada, ParametroPersonaVisita);
                            if (bandera.HasValue)
                                PorEdificio = bandera.Value;
                            else
                                return null;
                        }
                        return PorEdificio;
                        #endregion
                    }
                }
            }
            catch (Exception ex)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al validar los días de visita.", ex);
            }
            return false;
        }

        private async Task<bool> ValidarYGuardarEntrada()
        {
            try
            {
                if (!PInsertar)
                {
                    Application.Current.Dispatcher.Invoke((Action)(delegate
                    {
                        (new Dialogos()).ConfirmacionDialogo("Advertencia!", "No tienes permiso para guardar un ingreso.");
                    }));
                    return false;
                }

                if (!BotonMenuGuardado) return false;

                #region VALIDACIONES
                if (SelectPersona == null || (SelectTipoPersona.HasValue ? SelectTipoPersona.Value <= 0 : true))
                {
                    Application.Current.Dispatcher.Invoke((Action)(delegate
                    {
                        (new Dialogos()).ConfirmacionDialogo("Advertencia!", "Debes seleccionar a una persona.");
                    }));
                    return false;
                }
                /*if (new cAduana().ObtenerUltimaEntradaSinSalida(SelectPersona.ID_PERSONA) != null)
                {
                    Application.Current.Dispatcher.Invoke((Action)(delegate
                    {
                        (new Dialogos()).ConfirmacionDialogo("Advertencia!", "No puede registrar la entrada porque tiene pendiente una salida.");
                    }));
                    return false;
                }*/

                var AdminCP = "";
                var regresa = false;
                var UtilizaPase = 0;
                var SoloDeposito = false;
                var regresaString = string.Empty;
                var listaRegresa = new List<string>();
                #region EXTERNA
                //if (SelectTipoPersona == int.Parse(ParametroPersonaExterna) ? SelectPersona.PERSONA_EXTERNO.Where(w => w.ID_CENTRO == GlobalVar.gCentro && w.ESTATUS != "S").Any() : false)
                if (SelectTipoPersona == int.Parse(Parametro.ID_TIPO_PERSONA_EXTERNA) ? SelectPersona.PERSONA_EXTERNO != null ? SelectPersona.PERSONA_EXTERNO.ESTATUS == "S" : false : false)
                {
                    Application.Current.Dispatcher.Invoke((Action)(delegate
                    {
                        (new Dialogos()).ConfirmacionDialogo("Advertencia!", "La persona seleccionada no está autorizada para ingresar al centro.");
                    }));
                    return false;
                    /*regresa = true;
                    regresaString = "La persona seleccionada no esta autorizada para ingresar al centro";*/
                }
                #endregion

                var hoy = Fechas.GetFechaDateServer;
                var PersonaVisita = ParametroPersonaVisita;

                #region VISITA
                if (SelectTipoPersona == PersonaVisita)
                {
                    if (!ListadoInternos.Where(w => w.ELEGIDO).Any())
                    {
                        Application.Current.Dispatcher.Invoke((Action)(delegate
                        {
                            (new Dialogos()).ConfirmacionDialogo("Advertencia!", "Debes seleccionar un interno a visitar.");
                        }));
                        return false;
                    }
                    if (ListadoInternos.Where(w => w.ELEGIDO).Count() > ParametroInternosPermitidosPorDia)
                    {
                        Application.Current.Dispatcher.Invoke((Action)(delegate
                        {
                            (new Dialogos()).ConfirmacionDialogo("Advertencia!", "El limite de internos permitidos en visita por día es de [ " + ParametroInternosPermitidosPorDia + " ] y lo ha sobrepasado.");
                        }));
                        return false;
                    }
                    if (SelectPersona.VISITANTE.ID_ESTATUS_VISITA == ParametroVisitaCancelada || SelectPersona.VISITANTE.ID_ESTATUS_VISITA == ParametroVisitaSuspendido)
                    {
                        Application.Current.Dispatcher.Invoke((Action)(delegate
                        {
                            (new Dialogos()).ConfirmacionDialogo("Advertencia!", "El visitante seleccionado no está autorizado para ingresar al centro.");
                        }));
                        return false;
                    }
                    if (ListadoInternos.Where(w => w.ELEGIDO && (w.VISITANTE_INGRESO.ID_ESTATUS_VISITA == ParametroVisitaCancelada || w.VISITANTE_INGRESO.ID_ESTATUS_VISITA == ParametroVisitaSuspendido)).Any())
                    {
                        Application.Current.Dispatcher.Invoke((Action)(delegate
                        {
                            (new Dialogos()).ConfirmacionDialogo("Advertencia!", "La persona seleccionada no está autorizada para ingresar a ver a uno de los internos seleccionados.");
                        }));
                        return false;
                    }
                    foreach (var item in ListadoInternos.Where(w => w.ELEGIDO))
                    {
                        SoloDeposito = item.VISITANTE_INGRESO.ID_TIPO_VISITANTE == ParametroTipoVisitanteDepositante;
                        if (SoloDeposito) continue;
                        if (ListBitacoraAcceso.Count(w => w.ADUANA.ENTRADA_FEC.HasValue ?
                            ((w.ADUANA.ENTRADA_FEC.Value.Day == hoy.Day && w.ADUANA.ENTRADA_FEC.Value.Month == hoy.Month && w.ADUANA.ENTRADA_FEC.Value.Year == hoy.Year) ?
                                (w.ADUANA.ID_TIPO_PERSONA == PersonaVisita && !w.ADUANA.SALIDA_FEC.HasValue) ?
                                    w.ADUANA.ADUANA_INGRESO != null ?
                                        w.ADUANA.ADUANA_INGRESO.Any() ?
                                            w.ADUANA.ADUANA_INGRESO.Any(a => a.ID_CENTRO == item.VISITANTE_INGRESO.ID_CENTRO && a.ID_ANIO == item.VISITANTE_INGRESO.ID_ANIO && a.ID_IMPUTADO == item.VISITANTE_INGRESO.ID_IMPUTADO &&
                                            a.ID_INGRESO == item.VISITANTE_INGRESO.ID_INGRESO)
                                        : false
                                    : false
                                : false
                            : false)
                        : false) >= ParametroVisitaAlaVes)
                        {
                            /*Application.Current.Dispatcher.Invoke((Action)(delegate
                            {
                                new Dialogos().ConfirmacionDialogo("Notificación!", "El interno [" + item.VISITANTE_INGRESO.ID_ANIO.ToString() + "/" +
                                        item.VISITANTE_INGRESO.ID_IMPUTADO.ToString() + "] alcanzo el limite de visitantes por dia.");
                            }));
                            return false;*/
                            listaRegresa.Add(item.VISITANTE_INGRESO.ID_ANIO.ToString() + "/" + item.VISITANTE_INGRESO.ID_IMPUTADO.ToString());
                            regresa = true;
                            regresaString = regresaString + "El interno [" + item.VISITANTE_INGRESO.ID_ANIO.ToString() + "/" +
                                        item.VISITANTE_INGRESO.ID_IMPUTADO.ToString() + "] alcanzó el limite de visitantes por día. \n";
                        }
                        if (item.VISITANTE_INGRESO.INGRESO.TRASLADO_DETALLE.Any(w => (w.ID_ESTATUS != "CA" ? w.TRASLADO.ORIGEN_TIPO != "F" : false) && w.TRASLADO.TRASLADO_FEC.AddHours(-ParametroToleranciaTraslado) <= Fechas.GetFechaDateServer))
                        {
                            /*Application.Current.Dispatcher.Invoke((Action)(delegate
                            {
                                new Dialogos().ConfirmacionDialogo("Notificación!", "El interno [" + item.VISITANTE_INGRESO.ID_ANIO.ToString() + "/" +
                                        item.VISITANTE_INGRESO.ID_IMPUTADO.ToString() + "] tiene un traslado proximo y no puede recibir visitas.");
                            }));
                            return false;*/
                            listaRegresa.Add(item.VISITANTE_INGRESO.ID_ANIO.ToString() + "/" + item.VISITANTE_INGRESO.ID_IMPUTADO.ToString());
                            regresa = true;
                            regresaString = regresaString + "El interno [" + item.VISITANTE_INGRESO.ID_ANIO.ToString() + "/" +
                                        item.VISITANTE_INGRESO.ID_IMPUTADO.ToString() + "] tiene un traslado próximo y no puede recibir visitas. \n";
                        }
                        /*if (item.VISITANTE_INGRESO.ID_ESTATUS_VISITA == ParametroVisitaRegistro ? item.VISITANTE_INGRESO.INGRESO.CAUSA_PENAL != null ? !item.VISITANTE_INGRESO.INGRESO.CAUSA_PENAL.Any() : true : false)
                        {
                            var visitas = new cAduana().ObtenerTodos("").Where(w => w.ADUANA_INGRESO.Any(a => a.ID_CENTRO == item.VISITANTE_INGRESO.ID_CENTRO && a.ID_ANIO == item.VISITANTE_INGRESO.ID_ANIO && a.ID_IMPUTADO == item.VISITANTE_INGRESO.ID_IMPUTADO && a.ID_INGRESO == item.VISITANTE_INGRESO.ID_INGRESO) && w.ID_TIPO_PERSONA == (short)enumTipoPersona.PERSONA_VISITA);
                            if (visitas != null ? !visitas.Any() : true)
                            {
                                continue;
                            }
                        }*/
                        if (item.VISITANTE_INGRESO.EMISION_GAFETE == "S")
                        {
                            if (item.VISITANTE_INGRESO.INGRESO.INCIDENTE.Where(w => w.ESTATUS == "A" ? w.SANCION.Where(wh => wh.ID_SANCION == 9).Any() : false).Any())
                            {
                                /*Application.Current.Dispatcher.Invoke((Action)(delegate
                                {
                                    (new Dialogos()).ConfirmacionDialogo("Advertencia!", "El imputado [" + item.VISITANTE_INGRESO.ID_ANIO.ToString() + "/" + item.VISITANTE_INGRESO.ID_IMPUTADO.ToString() + "] esta sancionado y no puede recibir visitas.");
                                }));
                                return false;*/
                                listaRegresa.Add(item.VISITANTE_INGRESO.ID_ANIO.ToString() + "/" + item.VISITANTE_INGRESO.ID_IMPUTADO.ToString());
                                regresa = true;
                                regresaString = regresaString + "El imputado [" + item.VISITANTE_INGRESO.ID_ANIO.ToString() + "/" + item.VISITANTE_INGRESO.ID_IMPUTADO.ToString() + "] está sancionado y no puede recibir visitas. \n";
                            }
                            var validacion = ValidarDiaVisita(item.VISITANTE_INGRESO);
                            if (validacion.HasValue)
                            {
                                if (validacion.Value)
                                {
                                    /*if (BanderaIntima)
                                    {
                                        //VISITA INTIMA TRUE
                                    }
                                    else
                                    {
                                        listaRegresa.Add(item.VISITANTE_INGRESO.ID_ANIO.ToString() + "/" + item.VISITANTE_INGRESO.ID_IMPUTADO.ToString());
                                        regresa = true;
                                        regresaString = regresaString + "El imputado [" + item.VISITANTE_INGRESO.ID_ANIO.ToString() + "/" + item.VISITANTE_INGRESO.ID_IMPUTADO.ToString() + "] no tiene permitido recibir visitas en este día. \n";
                                    }*/
                                }
                                else
                                {
                                    /*Application.Current.Dispatcher.Invoke((Action)(delegate
                                    {
                                        (new Dialogos()).ConfirmacionDialogo("Advertencia!", "Un imputado no tiene perimitido recibir visitas en este dia.");
                                    }));
                                    return false;
                                    if (BanderaIntima)
                                    {
                                        //VISITA INTIMA TRUE
                                    }
                                    else
                                    {*/
                                    listaRegresa.Add(item.VISITANTE_INGRESO.ID_ANIO.ToString() + "/" + item.VISITANTE_INGRESO.ID_IMPUTADO.ToString());
                                    regresa = true;
                                    regresaString = regresaString + "El imputado [" + item.VISITANTE_INGRESO.ID_ANIO.ToString() + "/" + item.VISITANTE_INGRESO.ID_IMPUTADO.ToString() + "] no tiene permitido recibir visitas en este día. \n";
                                    //}
                                }
                            }
                        }
                        else
                        {
                            if (item.VISITANTE_INGRESO.ACCESO_UNICO == "S")
                            {
                                var visitas = new cAduana().ObtenerTodos("").Where(w => w.ADUANA_INGRESO.Any(a => a.ID_CENTRO == item.VISITANTE_INGRESO.ID_CENTRO && a.ID_ANIO == item.VISITANTE_INGRESO.ID_ANIO && a.ID_IMPUTADO == item.VISITANTE_INGRESO.ID_IMPUTADO && a.ID_INGRESO == item.VISITANTE_INGRESO.ID_INGRESO) && w.ID_TIPO_PERSONA == (short)enumTipoPersona.PERSONA_VISITA);
                                if (visitas != null ? !visitas.Any() : true) continue;
                                else
                                {
                                    listaRegresa.Add(item.VISITANTE_INGRESO.ID_ANIO.ToString() + "/" + item.VISITANTE_INGRESO.ID_IMPUTADO.ToString());
                                    regresa = true;
                                    regresaString = regresaString + "El visitante seleccionado esta registrado como solo de unico acceso con el imputado [" + item.VISITANTE_INGRESO.ID_ANIO.ToString() + "/" + item.VISITANTE_INGRESO.ID_IMPUTADO.ToString() + "] y ya tiene un ingreso registrado. \n";
                                }
                            }
                            var hoymenos = hoy.AddDays(-45);
                            var pase = new cVisitanteIngresoPase().ObtenerTodos().Where(w => (w.ID_CENTRO == item.VISITANTE_INGRESO.ID_CENTRO && w.ID_ANIO == item.VISITANTE_INGRESO.ID_ANIO &&
                            w.ID_IMPUTADO == item.VISITANTE_INGRESO.ID_IMPUTADO && w.ID_INGRESO == item.VISITANTE_INGRESO.ID_INGRESO && w.ID_PERSONA == item.VISITANTE_INGRESO.ID_PERSONA) ?
                                w.AUTORIZADO == "S" ?
                                    w.FECHA_ALTA.HasValue ?
                                        w.FECHA_ALTA.Value > hoymenos
                                    : false
                                : false
                            : false).OrderByDescending(o => o.FECHA_ALTA);
                            if (pase != null ? !pase.Any() : true)
                            {
                                listaRegresa.Add(item.VISITANTE_INGRESO.ID_ANIO.ToString() + "/" + item.VISITANTE_INGRESO.ID_IMPUTADO.ToString());
                                regresa = true;
                                regresaString = regresaString + "La persona seleccionada no está credencializada y no tiene ningún pase por utilizar. \n";
                            }
                            var entradas = new cAduanaIngreso().ObtenerXAbogadoEIngreso(SelectPersona.ID_PERSONA, item.VISITANTE_INGRESO.ID_CENTRO,
                                    item.VISITANTE_INGRESO.ID_ANIO, item.VISITANTE_INGRESO.ID_IMPUTADO, item.VISITANTE_INGRESO.ID_INGRESO);
                            foreach (var pass in pase)
                            {
                                if (pass.ID_PASE == (short)enumTiposPases.INICIAL_UNICO)
                                {
                                    if (pass.FECHA_ALTA.HasValue ? new Fechas().AgregarDiasHabiles(pass.FECHA_ALTA.Value, 10) >= hoy ?
                                            true
                                        : pass.ADUANA_INGRESO != null ?
                                            pass.ADUANA_INGRESO.Count <= 1
                                        : false
                                    : false)
                                    {
                                        //ACCESO UNICO POR 10 DIAS HABILES
                                        if (entradas.Count(c => c.VISITANTE_INGRESO_PASE != null ? c.VISITANTE_INGRESO_PASE.ID_PASE == (short)enumTiposPases.INICIAL_UNICO : false) <= 1)
                                        {
                                            /*aun no cumple con su pase unico*/
                                            var validacion = ValidarDiaVisita(item.VISITANTE_INGRESO);
                                            if (validacion.HasValue)
                                            {
                                                if (validacion.Value)
                                                {
                                                    UtilizaPase = pass.ID_CONSEC;
                                                    regresa = false;
                                                    break;
                                                }
                                                else
                                                {
                                                    UtilizaPase = 0;
                                                    listaRegresa.Add(item.VISITANTE_INGRESO.ID_ANIO.ToString() + "/" + item.VISITANTE_INGRESO.ID_IMPUTADO.ToString());
                                                    regresa = true;
                                                    regresaString = regresaString + "El imputado [" + item.VISITANTE_INGRESO.ID_ANIO.ToString() + "/" + item.VISITANTE_INGRESO.ID_IMPUTADO.ToString() + "] no tiene permitido recibir visitas en este día. \n";
                                                    break;
                                                }
                                            }
                                            else
                                            {
                                                UtilizaPase = 0;
                                                listaRegresa.Add(item.VISITANTE_INGRESO.ID_ANIO.ToString() + "/" + item.VISITANTE_INGRESO.ID_IMPUTADO.ToString());
                                                regresa = true;
                                                regresaString = regresaString + "El imputado [" + item.VISITANTE_INGRESO.ID_ANIO.ToString() + "/" + item.VISITANTE_INGRESO.ID_IMPUTADO.ToString() + "] no tiene permitido recibir visitas en este día. \n";
                                                break;
                                            }
                                        }
                                        else
                                        {
                                            UtilizaPase = 0;
                                            listaRegresa.Add(item.VISITANTE_INGRESO.ID_ANIO.ToString() + "/" + item.VISITANTE_INGRESO.ID_IMPUTADO.ToString());
                                            regresa = true;
                                            regresaString = regresaString + "La persona seleccionada ya utilizó su pase inicial. \n";
                                            break;
                                        }
                                    }
                                    else
                                    {
                                        UtilizaPase = 0;
                                        listaRegresa.Add(item.VISITANTE_INGRESO.ID_ANIO.ToString() + "/" + item.VISITANTE_INGRESO.ID_IMPUTADO.ToString());
                                        regresa = true;
                                        regresaString = regresaString + "La persona seleccionada ya utilizó su pase inicial. \n";
                                        break;
                                    }
                                }
                                else if (pass.ID_PASE == (short)enumTiposPases.POR_10_DIAS)
                                {
                                    if (pass.FECHA_ALTA.HasValue ?
                                        new Fechas().AgregarDiasHabiles(pass.FECHA_ALTA.Value, 10) >= hoy ?
                                            true
                                        : pass.ADUANA_INGRESO != null ?
                                            pass.ADUANA_INGRESO.Count <= 1
                                        : false
                                    : false)
                                    {
                                        //10 DIAS EXTRAS
                                        if (entradas.Count(c => c.VISITANTE_INGRESO_PASE != null ? c.VISITANTE_INGRESO_PASE.ID_PASE == (short)enumTiposPases.POR_10_DIAS : false) <= 1)
                                        {
                                            //aun no cumple con su pase por 10 dias
                                            var validacion = ValidarDiaVisita(item.VISITANTE_INGRESO);
                                            if (validacion.HasValue)
                                            {
                                                if (validacion.Value)
                                                {
                                                    UtilizaPase = pass.ID_CONSEC;
                                                    regresa = false;
                                                    break;
                                                }
                                                else
                                                {
                                                    UtilizaPase = 0;
                                                    listaRegresa.Add(item.VISITANTE_INGRESO.ID_ANIO.ToString() + "/" + item.VISITANTE_INGRESO.ID_IMPUTADO.ToString());
                                                    regresa = true;
                                                    regresaString = regresaString + "El imputado [" + item.VISITANTE_INGRESO.ID_ANIO.ToString() + "/" + item.VISITANTE_INGRESO.ID_IMPUTADO.ToString() + "] no tiene permitido recibir visitas en este día. \n";
                                                    break;
                                                }
                                            }
                                            else
                                            {
                                                UtilizaPase = 0;
                                                listaRegresa.Add(item.VISITANTE_INGRESO.ID_ANIO.ToString() + "/" + item.VISITANTE_INGRESO.ID_IMPUTADO.ToString());
                                                regresa = true;
                                                regresaString = regresaString + "El imputado [" + item.VISITANTE_INGRESO.ID_ANIO.ToString() + "/" + item.VISITANTE_INGRESO.ID_IMPUTADO.ToString() + "] no tiene permitido recibir visitas en este día. \n";
                                                break;
                                            }
                                        }
                                        else
                                        {
                                            UtilizaPase = 0;
                                            listaRegresa.Add(item.VISITANTE_INGRESO.ID_ANIO.ToString() + "/" + item.VISITANTE_INGRESO.ID_IMPUTADO.ToString());
                                            regresa = true;
                                            regresaString = regresaString + "La persona seleccionada ya utilizó su pase por 10 dias o ya pasó el dia de vigencia. \n";
                                            break;
                                        }
                                    }
                                    else
                                    {
                                        UtilizaPase = 0;
                                        listaRegresa.Add(item.VISITANTE_INGRESO.ID_ANIO.ToString() + "/" + item.VISITANTE_INGRESO.ID_IMPUTADO.ToString());
                                        regresa = true;
                                        regresaString = regresaString + "La persona seleccionada ya utilizó su pase por 10 dias o ya pasó el dia de vigencia. \n";
                                        break;
                                    }
                                }
                                else if (pass.ID_PASE == (short)enumTiposPases.EXTRAORDINARIO)
                                {
                                    if (pass.ADUANA_INGRESO != null ? !pass.ADUANA_INGRESO.Any() : false)
                                    {
                                        //EXTRAORDINARIO DE HOY
                                        //DEJARLO PASAR ***
                                        if (pass.FECHA_ALTA.Value.Day == hoy.Day && pass.FECHA_ALTA.Value.Month == hoy.Month && pass.FECHA_ALTA.Value.Year == hoy.Year)
                                        {
                                            UtilizaPase = pass.ID_CONSEC;
                                            regresa = false;
                                            break;
                                        }
                                        else
                                        {
                                            UtilizaPase = 0;
                                            listaRegresa.Add(item.VISITANTE_INGRESO.ID_ANIO.ToString() + "/" + item.VISITANTE_INGRESO.ID_IMPUTADO.ToString());
                                            regresa = true;
                                            regresaString = regresaString + "La persona seleccionada no tiene un pase para el dia de hoy. \n";
                                            break;
                                        }
                                    }
                                    else
                                    {
                                        UtilizaPase = 0;
                                        listaRegresa.Add(item.VISITANTE_INGRESO.ID_ANIO.ToString() + "/" + item.VISITANTE_INGRESO.ID_IMPUTADO.ToString());
                                        regresa = true;
                                        regresaString = regresaString + "La persona seleccionada no tiene un pase para el dia de hoy o ya fue utilizado. \n";
                                        continue;
                                    }
                                }
                                else if (pass.ID_PASE == (short)enumTiposPases.DE_TERMINO)
                                {
                                    if (pass.ADUANA_INGRESO != null ? !pass.ADUANA_INGRESO.Any() : false)
                                    {
                                        //DE TERMINO
                                        //DEJARLO PASAR ***
                                        if (pass.FECHA_ALTA.Value.Day == hoy.Day && pass.FECHA_ALTA.Value.Month == hoy.Month && pass.FECHA_ALTA.Value.Year == hoy.Year)
                                        {
                                            UtilizaPase = pass.ID_CONSEC;
                                            regresa = false;
                                            break;
                                        }
                                        else
                                        {
                                            UtilizaPase = 0;
                                            listaRegresa.Add(item.VISITANTE_INGRESO.ID_ANIO.ToString() + "/" + item.VISITANTE_INGRESO.ID_IMPUTADO.ToString());
                                            regresa = true;
                                            regresaString = regresaString + "La persona seleccionada no tiene un pase para el dia de hoy. \n";
                                            break;
                                        }
                                    }
                                    else
                                    {
                                        UtilizaPase = 0;
                                        listaRegresa.Add(item.VISITANTE_INGRESO.ID_ANIO.ToString() + "/" + item.VISITANTE_INGRESO.ID_IMPUTADO.ToString());
                                        regresa = true;
                                        regresaString = regresaString + "La persona seleccionada no tiene un pase para el dia de hoy o ya fue utilizado. \n";
                                        continue;
                                    }
                                }
                                else
                                {
                                    UtilizaPase = 0;
                                    listaRegresa.Add(item.VISITANTE_INGRESO.ID_ANIO.ToString() + "/" + item.VISITANTE_INGRESO.ID_IMPUTADO.ToString());
                                    regresa = true;
                                    regresaString = regresaString + "La persona seleccionada no está credencializada y no tiene ningún pase por utilizar. \n";
                                    break;
                                }
                            }
                        }
                    }
                }
                #endregion

                #region ABOGADOS
                if (SelectTipoPersona == ParametroPersonaLegal)
                {
                    if (SelectPersona.ABOGADO.ID_ESTATUS_VISITA == ParametroVisitaCancelada || SelectPersona.ABOGADO.ID_ESTATUS_VISITA == ParametroVisitaSuspendido)
                    {
                        (new Dialogos()).ConfirmacionDialogo("Advertencia!", "El " + (SelectPersona.ABOGADO.ID_ABOGADO_TITULO == ParametroAbogadoTitular ? "abogado" :
                            SelectPersona.ABOGADO.ID_ABOGADO_TITULO == ParametroAbogadoColaborador ? "colaborador" :
                            SelectPersona.ABOGADO.ID_ABOGADO_TITULO == ParametroAbogadoActuario ? "actuario" : "visitante") + " seleccionado no está autorizado para ingresar al centro.");
                        //listaRegresa.Add(item.ABOGADO_INGRESO.ID_ANIO.ToString() + "/" + item.ABOGADO_INGRESO.ID_IMPUTADO.ToString());
                        //regresa = true;
                        //regresaString = regresaString + "El " + (SelectPersona.ABOGADO.ID_ABOGADO_TITULO == AbogadoTitular ? "abogado" :
                        //    SelectPersona.ABOGADO.ID_ABOGADO_TITULO == AbogadoColaborador ? "colaborador" :
                        //        SelectPersona.ABOGADO.ID_ABOGADO_TITULO == AbogadoActuario ? "actuario" : "visitante") + " seleccionado no está autorizado para ingresar al centro.\n";
                        return false;
                    }
                    if (SelectPersona.ABOGADO.ID_ABOGADO_TITULO == ParametroAbogadoTitular || SelectPersona.ABOGADO.ID_ABOGADO_TITULO == ParametroAbogadoColaborador)
                    {
                        if (!ListIngresosAsignados.Where(w => w.ELIGE).Any())
                        {
                            (new Dialogos()).ConfirmacionDialogo("Advertencia!", "Debes seleccionar cuando menos un interno para tener acceso.");
                            return false;
                        }
                        var visita = short.Parse(ParametroDoctoJuez[0]);
                        var doctoJuez = short.Parse(ParametroDoctoJuez[1]);
                        var doctoInterno = short.Parse(ParametroDoctoInterno[1]);
                        #region ABOGADO_INGRESO
                        //var AdminCP = "";
                        var CPs = "";
                        foreach (var item in ListIngresosAsignados.Where(w => w.ELIGE))
                        {
                            //Abogado Titular
                            var titular = item.ABOGADO_INGRESO.ABOGADO.ABOGADO_TITULAR1.Where(w => w.ESTATUS == "S").FirstOrDefault();

                            if (item.ABOGADO_INGRESO.INGRESO.TRASLADO_DETALLE.Any(w => (w.ID_ESTATUS != "CA" ? w.TRASLADO.ORIGEN_TIPO != "F" : false) && w.TRASLADO.TRASLADO_FEC.AddHours(-ParametroToleranciaTraslado) <= Fechas.GetFechaDateServer))
                            {
                                /*new Dialogos().ConfirmacionDialogo("Notificación!", "El interno [" + item.ABOGADO_INGRESO.ID_ANIO.ToString() + "/" +
                                        item.ABOGADO_INGRESO.ID_IMPUTADO.ToString() + "] tiene un traslado proximo y no puede recibir visitas.");
                                return false;*/
                                listaRegresa.Add(item.ABOGADO_INGRESO.ID_ANIO.ToString() + "/" + item.ABOGADO_INGRESO.ID_IMPUTADO.ToString());
                                regresa = true;
                                regresaString = regresaString + "El interno [" + item.ABOGADO_INGRESO.ID_ANIO.ToString() + "/" +
                                        item.ABOGADO_INGRESO.ID_IMPUTADO.ToString() + "] tiene un traslado próximo y no puede recibir visitas. \n";
                            }
                            if (item.ABOGADO_INGRESO.ID_ESTATUS_VISITA == ParametroVisitaCancelada || item.ABOGADO_INGRESO.ID_ESTATUS_VISITA == ParametroVisitaSuspendido)
                            {
                                /*(new Dialogos()).ConfirmacionDialogo("Advertencia!", "El abogado seleccionado no esta autorizado para ingresar a ver al imputado [" +
                                    item.ABOGADO_INGRESO.ID_ANIO.ToString() + "/" + item.ABOGADO_INGRESO.ID_IMPUTADO.ToString() + "]");
                                return false;*/
                                listaRegresa.Add(item.ABOGADO_INGRESO.ID_ANIO.ToString() + "/" + item.ABOGADO_INGRESO.ID_IMPUTADO.ToString());
                                regresa = true;
                                regresaString = regresaString + "El abogado seleccionado no está autorizado para ingresar a ver al imputado [" +
                                    item.ABOGADO_INGRESO.ID_ANIO.ToString() + "/" + item.ABOGADO_INGRESO.ID_IMPUTADO.ToString() + "]. \n";
                            }
                            if (item.ABOGADO_INGRESO.INGRESO.CAUSA_PENAL == null ? true : item.ABOGADO_INGRESO.INGRESO.CAUSA_PENAL.Count <= 0)
                            {
                                //no trae causas penales [DE TERMINO]
                                if (new cAduanaIngreso().ObtenerXAbogadoEIngreso(SelectPersona.ID_PERSONA, item.ABOGADO_INGRESO.ID_CENTRO, item.ABOGADO_INGRESO.ID_ANIO, item.ABOGADO_INGRESO.ID_IMPUTADO, item.ABOGADO_INGRESO.ID_INGRESO)
                                    .Where(w => w.ADMINISTRATIVO == "S").Count() >= pases_admin)
                                {
                                    /*(new Dialogos()).ConfirmacionDialogo("Advertencia!", "El abogado seleccionado no esta autorizado para ingresar, ya utilizó su pase provisional con el imputado [" +
                                        item.ABOGADO_INGRESO.ID_ANIO.ToString() + "/" + item.ABOGADO_INGRESO.ID_IMPUTADO.ToString() + "]");
                                    return false;*/
                                    listaRegresa.Add(item.ABOGADO_INGRESO.ID_ANIO.ToString() + "/" + item.ABOGADO_INGRESO.ID_IMPUTADO.ToString());
                                    regresa = true;
                                    regresaString = regresaString + "El abogado seleccionado no está autorizado para ingresar, ya utilizó su pase provisional con el imputado [" +
                                        item.ABOGADO_INGRESO.ID_ANIO.ToString() + "/" + item.ABOGADO_INGRESO.ID_IMPUTADO.ToString() + "]. \n";
                                }
                                AdminCP = AdminCP + "Está entrando con este imputado [" + item.ABOGADO_INGRESO.ID_ANIO.ToString() + "/" + item.ABOGADO_INGRESO.ID_IMPUTADO.ToString() + "] como un abogado administrativo.\n";
                                Administrativos.Add(item.ABOGADO_INGRESO);
                            }
                            else
                            {
                                //if (item.ABOGADO_INGRESO.ABOGADO_ING_DOCTO.Any(wh => wh.ID_TIPO_VISITA == visita && (wh.ID_TIPO_DOCUMENTO == doctoJuez || wh.ID_TIPO_DOCUMENTO == doctoInterno)) ||
                                //    (item.ABOGADO_INGRESO.ABOGADO.ABOGADO2 != null ? item.ABOGADO_INGRESO.ABOGADO.ABOGADO2.ABOGADO_INGRESO.Any(a => a.ID_CENTRO == item.ABOGADO_INGRESO.ID_CENTRO && a.ID_ANIO == item.ABOGADO_INGRESO.ID_ANIO &&
                                //        a.ID_IMPUTADO == item.ABOGADO_INGRESO.ID_IMPUTADO && a.ID_INGRESO == item.ABOGADO_INGRESO.ID_INGRESO &&
                                //        a.ABOGADO_ING_DOCTO.Any(an => an.ID_TIPO_VISITA == visita && (an.ID_TIPO_DOCUMENTO == doctoJuez || an.ID_TIPO_DOCUMENTO == doctoInterno))) : false))

                                if (item.ABOGADO_INGRESO.ABOGADO_ING_DOCTO.Any(wh => wh.ID_TIPO_VISITA == visita && (wh.ID_TIPO_DOCUMENTO == doctoJuez || wh.ID_TIPO_DOCUMENTO == doctoInterno)) ||
                                    (titular != null ? titular.ABOGADO1.ABOGADO_INGRESO.Any(a => a.ID_CENTRO == item.ABOGADO_INGRESO.ID_CENTRO && a.ID_ANIO == item.ABOGADO_INGRESO.ID_ANIO &&
                                        a.ID_IMPUTADO == item.ABOGADO_INGRESO.ID_IMPUTADO && a.ID_INGRESO == item.ABOGADO_INGRESO.ID_INGRESO &&
                                        a.ABOGADO_ING_DOCTO.Any(an => an.ID_TIPO_VISITA == visita && (an.ID_TIPO_DOCUMENTO == doctoJuez || an.ID_TIPO_DOCUMENTO == doctoInterno))) : false))
                                {
                                    //tiene documento de asignacion

                                    if (item.ABOGADO_INGRESO.ABOGADO_CAUSA_PENAL.Count > 0 ? !ListCausasSeleccionadas.Where(w => w.ID_CENTRO == item.ABOGADO_INGRESO.ID_CENTRO &&
                                        w.ID_ANIO == item.ABOGADO_INGRESO.ID_ANIO && w.ID_IMPUTADO == item.ABOGADO_INGRESO.ID_IMPUTADO && w.ID_INGRESO == item.ABOGADO_INGRESO.ID_INGRESO &&
                                        w.ID_ESTATUS_CP != 4).Any() : false)
                                    {
                                        //if (await new Dialogos().ConfirmarEliminar("ADVERTENCIA!", "Esta seguro que desea ingresar como administrativo para este imputado? [" +
                                        //    item.ABOGADO_INGRESO.ID_ANIO.ToString() + "/" + item.ABOGADO_INGRESO.ID_IMPUTADO.ToString() + "]") != 1)
                                        //    return false;
                                        if (item.ABOGADO_INGRESO.ADMINISTRATIVO == "S")
                                        {
                                            AdminCP = AdminCP + "Está entrando con este imputado [" + item.ABOGADO_INGRESO.ID_ANIO.ToString() + "/" + item.ABOGADO_INGRESO.ID_IMPUTADO.ToString() + "] como un abogado administrativo.\n";
                                            Administrativos.Add(item.ABOGADO_INGRESO);
                                        }
                                        else
                                        {
                                            listaRegresa.Add(item.ABOGADO_INGRESO.ID_ANIO.ToString() + "/" + item.ABOGADO_INGRESO.ID_IMPUTADO.ToString());
                                            regresa = true;
                                            regresaString = regresaString + "El abogado seleccionado no está autorizado para ingresar a ver al imputado [" +
                                                item.ABOGADO_INGRESO.ID_ANIO.ToString() + "/" + item.ABOGADO_INGRESO.ID_IMPUTADO.ToString() + "] como un administrativo. \n";
                                            break;
                                        }
                                    }
                                    else
                                    {
                                        var cant = ListCausasSeleccionadas.Where(w => w.ID_CENTRO == item.ABOGADO_INGRESO.ID_CENTRO && w.ID_ANIO == item.ABOGADO_INGRESO.ID_ANIO &&
                                              w.ID_IMPUTADO == item.ABOGADO_INGRESO.ID_IMPUTADO && w.ID_INGRESO == item.ABOGADO_INGRESO.ID_INGRESO && w.ID_ESTATUS_CP != 4);
                                        var i = 1;
                                        foreach (var cp in cant)
                                        {
                                            if (cant.Count() > 1)
                                            {
                                                CPs = CPs + cp.CP_ANIO + "/" + cp.CP_FOLIO + ((i == cant.Count()) ? "," : "");
                                            }
                                            else
                                            {
                                                CPs = cp.CP_ANIO + "/" + cp.CP_FOLIO;
                                            }
                                            i++;
                                        }
                                        i = 0;
                                        AdminCP = AdminCP + "Está entrando con este imputado [" + item.ABOGADO_INGRESO.ID_ANIO.ToString() + "/" + item.ABOGADO_INGRESO.ID_IMPUTADO.ToString() + "] por causa penal (";
                                        foreach (var cp in cant)
                                        {
                                            AdminCP = AdminCP + cp.CP_ANIO + "/" + cp.CP_FOLIO;
                                            if (cant.Count() > 1) AdminCP = AdminCP + ", ";
                                            else if (cant.Count() == i) { }
                                            i++;
                                        }
                                        if (item.ABOGADO_INGRESO.ADMINISTRATIVO == "S")
                                        {
                                            AdminCP = AdminCP + ") y como un abogado administrativo.\n";
                                            Administrativos.Add(item.ABOGADO_INGRESO);
                                            //AdminCP = AdminCP + "Está entrando con este imputado [" + item.ABOGADO_INGRESO.ID_ANIO.ToString() + "/" + item.ABOGADO_INGRESO.ID_IMPUTADO.ToString() + "] por causa penal ("
                                            //       + CPs + ") y como un abogado administrativo.\n";
                                        }
                                        else
                                        {
                                            AdminCP = AdminCP + ")\n";
                                            //AdminCP = AdminCP + "Está entrando con este imputado [" + item.ABOGADO_INGRESO.ID_ANIO.ToString() + "/" + item.ABOGADO_INGRESO.ID_IMPUTADO.ToString() + "] por causa penal ("
                                            //       + CPs + ").\n";
                                        }
                                    }
                                }
                                else
                                {
                                    if (new cAduanaIngreso().ObtenerXAbogadoEIngreso(SelectPersona.ID_PERSONA, item.ABOGADO_INGRESO.ID_CENTRO, item.ABOGADO_INGRESO.ID_ANIO, item.ABOGADO_INGRESO.ID_IMPUTADO, item.ABOGADO_INGRESO.ID_INGRESO)
                                        .Where(w => w.ADMINISTRATIVO == "S").Count() >= pases_cp)
                                    {
                                        /*(new Dialogos()).ConfirmacionDialogo("Advertencia!", "El abogado seleccionado no esta autorizado para ingresar, ya utilizó su pase provisional con el imputado [" +
                                            item.ABOGADO_INGRESO.ID_ANIO.ToString() + "/" + item.ABOGADO_INGRESO.ID_IMPUTADO.ToString() + "]");
                                        return false;*/
                                        listaRegresa.Add(item.ABOGADO_INGRESO.ID_ANIO.ToString() + "/" + item.ABOGADO_INGRESO.ID_IMPUTADO.ToString());
                                        regresa = true;
                                        regresaString = regresaString + "El abogado seleccionado no está autorizado para ingresar, ya utilizó su pase provisional con el imputado [" +
                                            item.ABOGADO_INGRESO.ID_ANIO.ToString() + "/" + item.ABOGADO_INGRESO.ID_IMPUTADO.ToString() + "]. \n";
                                    }
                                    if (item.ABOGADO_INGRESO.ABOGADO_CAUSA_PENAL.Any(w => w.ABOGADO_CP_DOCTO.Any(wh => wh.ID_TIPO_VISITA == visita && wh.ID_TIPO_DOCUMENTO == doctoInterno
                                        && w.ID_ESTATUS_VISITA == ParametroVisitaAutorizada)) && item.ABOGADO_INGRESO.ID_ESTATUS_VISITA == ParametroVisitaAutorizada)
                                    {
                                        //es titular
                                        if (new cAduanaIngreso().ObtenerXAbogadoEIngreso(SelectPersona.ID_PERSONA, item.ABOGADO_INGRESO.ID_CENTRO,
                                            item.ABOGADO_INGRESO.ID_ANIO, item.ABOGADO_INGRESO.ID_IMPUTADO, item.ABOGADO_INGRESO.ID_INGRESO).Count() <= 3)
                                        {
                                            //aun no cumple con sus tres pases provisionales
                                            if (item.ABOGADO_INGRESO.ADMINISTRATIVO == "S")
                                                AdminCP = AdminCP + "Está entrando con este imputado [" + item.ABOGADO_INGRESO.ID_ANIO.ToString() + "/" + item.ABOGADO_INGRESO.ID_IMPUTADO.ToString() + "] como un abogado administrativo.\n";
                                        }
                                        else
                                        {
                                            /*(new Dialogos()).ConfirmacionDialogo("Advertencia!", "El abogado seleccionado no esta autorizado para ingresar, ya utilizó sus 3 pases provisionales con el imputado [" +
                                                item.ABOGADO_INGRESO.ID_ANIO.ToString() + "/" + item.ABOGADO_INGRESO.ID_IMPUTADO.ToString() + "]");
                                            return false;*/
                                            listaRegresa.Add(item.ABOGADO_INGRESO.ID_ANIO.ToString() + "/" + item.ABOGADO_INGRESO.ID_IMPUTADO.ToString());
                                            regresa = true;
                                            regresaString = regresaString + "El abogado seleccionado no está autorizado para ingresar, ya utilizó sus 3 pases provisionales con el imputado [" +
                                                item.ABOGADO_INGRESO.ID_ANIO.ToString() + "/" + item.ABOGADO_INGRESO.ID_IMPUTADO.ToString() + "]. \n";
                                        }
                                    }
                                    else
                                    {
                                        if (item.ABOGADO_INGRESO.ABOGADO_CAUSA_PENAL.Count > 0)
                                        {
                                            if (!ListCausasSeleccionadas.Any(w => w.ID_CENTRO == item.ABOGADO_INGRESO.ID_CENTRO && w.ID_ANIO == item.ABOGADO_INGRESO.ID_ANIO && w.ID_IMPUTADO == item.ABOGADO_INGRESO.ID_IMPUTADO &&
                                                   w.ID_INGRESO == item.ABOGADO_INGRESO.ID_INGRESO && w.ID_ESTATUS_CP != 4))
                                            {
                                                if (item.ABOGADO_INGRESO.ADMINISTRATIVO == "S")
                                                {
                                                    AdminCP = AdminCP + "Está entrando con este imputado [" + item.ABOGADO_INGRESO.ID_ANIO.ToString() + "/" + item.ABOGADO_INGRESO.ID_IMPUTADO.ToString() + "] como un abogado administrativo.\n";
                                                    Administrativos.Add(item.ABOGADO_INGRESO);
                                                }
                                            }
                                            else
                                            {
                                                if (ListCausasSeleccionadas.Any(w => w.ID_CENTRO == item.ABOGADO_INGRESO.ID_CENTRO && w.ID_ANIO == item.ABOGADO_INGRESO.ID_ANIO && w.ID_IMPUTADO == item.ABOGADO_INGRESO.ID_IMPUTADO &&
                                                   w.ID_INGRESO == item.ABOGADO_INGRESO.ID_INGRESO && w.ID_ESTATUS_CP != 4))
                                                {
                                                    var cant = ListCausasSeleccionadas.Where(w => w.ID_CENTRO == item.ABOGADO_INGRESO.ID_CENTRO && w.ID_ANIO == item.ABOGADO_INGRESO.ID_ANIO &&
                                                        w.ID_IMPUTADO == item.ABOGADO_INGRESO.ID_IMPUTADO && w.ID_INGRESO == item.ABOGADO_INGRESO.ID_INGRESO && w.ID_ESTATUS_CP != 4);
                                                    var i = 1;
                                                    foreach (var cp in cant)
                                                    {
                                                        CPs = cant.Count() > 1 ? (CPs + cp.CP_ANIO + "/" + cp.CP_FOLIO + ((i == cant.Count()) ? "," : "")) : (CPs = cp.CP_ANIO + "/" + cp.CP_FOLIO);
                                                        i++;
                                                    }
                                                    i = 0;
                                                    AdminCP = AdminCP + "Está entrando con este imputado [" + item.ABOGADO_INGRESO.ID_ANIO.ToString() + "/" + item.ABOGADO_INGRESO.ID_IMPUTADO.ToString() + "] por causa penal (";
                                                    foreach (var cp in cant)
                                                    {
                                                        AdminCP = AdminCP + cp.CP_ANIO + "/" + cp.CP_FOLIO;
                                                        if (cant.Count() > 1)
                                                        {
                                                            AdminCP = AdminCP + ", ";
                                                        }
                                                        else if (cant.Count() == i) { }
                                                        i++;
                                                    }
                                                    if (item.ABOGADO_INGRESO.ADMINISTRATIVO == "S")
                                                    {
                                                        AdminCP = AdminCP + ") y como un abogado administrativo.\n";
                                                        Administrativos.Add(item.ABOGADO_INGRESO);
                                                    }
                                                    else AdminCP = AdminCP + ")\n";
                                                }
                                                else
                                                {
                                                    if (item.ABOGADO_INGRESO.ADMINISTRATIVO == "S")
                                                    {
                                                        AdminCP = AdminCP + "Está entrando con este imputado [" + item.ABOGADO_INGRESO.ID_ANIO.ToString() + "/" + item.ABOGADO_INGRESO.ID_IMPUTADO.ToString() + "] como un abogado administrativo.\n";
                                                        Administrativos.Add(item.ABOGADO_INGRESO);
                                                    }
                                                    else
                                                    {
                                                        AdminCP = AdminCP + "Está entrando con este imputado [" + item.ABOGADO_INGRESO.ID_ANIO.ToString() + "/" + item.ABOGADO_INGRESO.ID_IMPUTADO.ToString() + "] por causa penal.\n";
                                                        Administrativos.Add(item.ABOGADO_INGRESO);
                                                    }
                                                }
                                            }
                                        }
                                        else
                                        {
                                            if (item.ABOGADO_INGRESO.ADMINISTRATIVO == "S")
                                            {
                                                AdminCP = AdminCP + "Está entrando con este imputado [" + item.ABOGADO_INGRESO.ID_ANIO.ToString() + "/" + item.ABOGADO_INGRESO.ID_IMPUTADO.ToString() + "] como un abogado administrativo.\n";
                                                Administrativos.Add(item.ABOGADO_INGRESO);
                                            }
                                        }
                                        //no es titular
                                        if (item.ABOGADO_INGRESO.ABOGADO_ING_DOCTO.Where(w => w.ID_TIPO_VISITA == visita && w.ID_TIPO_DOCUMENTO == doctoInterno).Any())
                                        {
                                            if (new cAduanaIngreso().ObtenerXAbogadoEIngreso(SelectPersona.ID_PERSONA, item.ABOGADO_INGRESO.ID_CENTRO,
                                                item.ABOGADO_INGRESO.ID_ANIO, item.ABOGADO_INGRESO.ID_IMPUTADO, item.ABOGADO_INGRESO.ID_INGRESO).Count() <= 1)
                                            {
                                                //aun no cumple con su pase provisional
                                            }
                                            else
                                            {
                                                /*(new Dialogos()).ConfirmacionDialogo("Advertencia!", "El abogado seleccionado no esta autorizado para ingresar, ya utilizó su pase provisional con el imputado [" +
                                                    item.ABOGADO_INGRESO.ID_ANIO.ToString() + "/" + item.ABOGADO_INGRESO.ID_IMPUTADO.ToString() + "]");
                                                return false;*/
                                                listaRegresa.Add(item.ABOGADO_INGRESO.ID_ANIO.ToString() + "/" + item.ABOGADO_INGRESO.ID_IMPUTADO.ToString());
                                                regresa = true;
                                                regresaString = regresaString + "El abogado seleccionado no está autorizado para ingresar, ya utilizó su pase provisional con el imputado [" +
                                                    item.ABOGADO_INGRESO.ID_ANIO.ToString() + "/" + item.ABOGADO_INGRESO.ID_IMPUTADO.ToString() + "]. \n";
                                            }
                                        }
                                        else
                                        {
                                            if (new cAduanaIngreso().ObtenerXAbogadoEIngreso(SelectPersona.ID_PERSONA, item.ABOGADO_INGRESO.ID_CENTRO,
                                                item.ABOGADO_INGRESO.ID_ANIO, item.ABOGADO_INGRESO.ID_IMPUTADO, item.ABOGADO_INGRESO.ID_INGRESO).Count() <= 2) { /*aun no cumple con sus dos pases provisionales*/ }
                                            else
                                            {
                                                /*(new Dialogos()).ConfirmacionDialogo("Advertencia!", "El abogado seleccionado no esta autorizado para ingresar, ya utilizó sus 2 pases provisionales con el imputado [" +
                                                    item.ABOGADO_INGRESO.ID_ANIO.ToString() + "/" + item.ABOGADO_INGRESO.ID_IMPUTADO.ToString() + "]");
                                                return false;*/
                                                listaRegresa.Add(item.ABOGADO_INGRESO.ID_ANIO.ToString() + "/" + item.ABOGADO_INGRESO.ID_IMPUTADO.ToString());
                                                regresa = true;
                                                regresaString = regresaString + "El abogado seleccionado no está autorizado para ingresar, ya utilizó sus 2 pases provisionales con el imputado [" +
                                                    item.ABOGADO_INGRESO.ID_ANIO.ToString() + "/" + item.ABOGADO_INGRESO.ID_IMPUTADO.ToString() + "]. \n";
                                            }
                                        }
                                    }
                                }
                            }

                            //if (SelectPersona.ABOGADO.ID_ABOGADO_TITULO == AbogadoColaborador ? SelectPersona.ABOGADO.ABOGADO2.ABOGADO_INGRESO.Where(w => w.ID_CENTRO == item.ABOGADO_INGRESO.ID_CENTRO && w.ID_ANIO == item.ABOGADO_INGRESO.ID_ANIO &&
                            //        w.ID_IMPUTADO == item.ABOGADO_INGRESO.ID_IMPUTADO && w.ID_INGRESO == item.ABOGADO_INGRESO.ID_INGRESO).Any() ?
                            //        SelectPersona.ABOGADO.ABOGADO2.ABOGADO_INGRESO.Where(w => w.ID_CENTRO == item.ABOGADO_INGRESO.ID_CENTRO && w.ID_ANIO == item.ABOGADO_INGRESO.ID_ANIO &&
                            //        w.ID_IMPUTADO == item.ABOGADO_INGRESO.ID_IMPUTADO && w.ID_INGRESO == item.ABOGADO_INGRESO.ID_INGRESO).FirstOrDefault().ID_ESTATUS_VISITA == VisitaCancelado ||
                            //        SelectPersona.ABOGADO.ABOGADO2.ABOGADO_INGRESO.Where(w => w.ID_CENTRO == item.ABOGADO_INGRESO.ID_CENTRO && w.ID_ANIO == item.ABOGADO_INGRESO.ID_ANIO &&
                            //        w.ID_IMPUTADO == item.ABOGADO_INGRESO.ID_IMPUTADO && w.ID_INGRESO == item.ABOGADO_INGRESO.ID_INGRESO).FirstOrDefault().ID_ESTATUS_VISITA == VisitaSuspendido : false : false)
                            if (SelectPersona.ABOGADO.ID_ABOGADO_TITULO == ParametroAbogadoColaborador ? titular.ABOGADO1.ABOGADO_INGRESO.Where(w => w.ID_CENTRO == item.ABOGADO_INGRESO.ID_CENTRO && w.ID_ANIO == item.ABOGADO_INGRESO.ID_ANIO &&
                                    w.ID_IMPUTADO == item.ABOGADO_INGRESO.ID_IMPUTADO && w.ID_INGRESO == item.ABOGADO_INGRESO.ID_INGRESO).Any() ?
                                    titular.ABOGADO1.ABOGADO_INGRESO.Where(w => w.ID_CENTRO == item.ABOGADO_INGRESO.ID_CENTRO && w.ID_ANIO == item.ABOGADO_INGRESO.ID_ANIO &&
                                    w.ID_IMPUTADO == item.ABOGADO_INGRESO.ID_IMPUTADO && w.ID_INGRESO == item.ABOGADO_INGRESO.ID_INGRESO).FirstOrDefault().ID_ESTATUS_VISITA == ParametroVisitaCancelada ||
                                    titular.ABOGADO1.ABOGADO_INGRESO.Where(w => w.ID_CENTRO == item.ABOGADO_INGRESO.ID_CENTRO && w.ID_ANIO == item.ABOGADO_INGRESO.ID_ANIO &&
                                    w.ID_IMPUTADO == item.ABOGADO_INGRESO.ID_IMPUTADO && w.ID_INGRESO == item.ABOGADO_INGRESO.ID_INGRESO).FirstOrDefault().ID_ESTATUS_VISITA == ParametroVisitaSuspendido : false : false)
                            {
                                (new Dialogos()).ConfirmacionDialogo("Advertencia!", "El abogado titular de este colaborador no tiene permitido el acceso con el imputado [" +
                                    item.ABOGADO_INGRESO.ID_ANIO.ToString() + "/" + item.ABOGADO_INGRESO.ID_IMPUTADO.ToString() + "].");
                                return false;
                                /*regresa = true;
                                regresaString = regresaString + "El abogado titular de este colaborador no tiene permitido el acceso con el imputado [" +
                                    item.ABOGADO_INGRESO.ID_ANIO.ToString() + "/" + item.ABOGADO_INGRESO.ID_IMPUTADO.ToString() + "]. \n";*/

                            }
                        }
                        #endregion

                        //if (SelectPersona.ABOGADO.ID_ABOGADO_TITULO == AbogadoColaborador ? SelectPersona.ABOGADO.ABOGADO2.ID_ESTATUS_VISITA == VisitaCancelado ||
                        //    SelectPersona.ABOGADO.ABOGADO2.ID_ESTATUS_VISITA == VisitaSuspendido : false)
                        var titular2 = SelectPersona.ABOGADO.ABOGADO_TITULAR1.Where(w => w.ESTATUS == "S").FirstOrDefault();
                        if (SelectPersona.ABOGADO.ID_ABOGADO_TITULO == ParametroAbogadoColaborador ? titular2.ABOGADO1.ID_ESTATUS_VISITA == ParametroVisitaCancelada ||
                            titular2.ABOGADO1.ID_ESTATUS_VISITA == ParametroVisitaSuspendido : false)
                        {
                            (new Dialogos()).ConfirmacionDialogo("Advertencia!", "El abogado titular de este colaborador no tiene permitido el acceso.");
                            return false;
                            /*regresa = true;
                            regresaString = regresaString + "El abogado titular de este colaborador no tiene permitido el acceso. \n";*/
                        }
                    }
                    if (SelectPersona.ABOGADO.ID_ABOGADO_TITULO == ParametroAbogadoActuario)
                    {
                        if (!ListInternosSeleccionadosPorNotificar.Any(w => w.ELIGE))
                        {
                            (new Dialogos()).ConfirmacionDialogo("Advertencia!", "Debes seleccionar cuando menos un interno para tener acceso.");
                            return false;
                            /*regresa = true;
                            regresaString = regresaString + "Debes seleccionar cuando menos un interno para tener acceso. \n";*/
                        }
                        foreach (var item in ListInternosSeleccionadosPorNotificar.Where(w => w.ELIGE))
                        {
                            if (item.ACTUARIO_INGRESO.INGRESO.TRASLADO_DETALLE.Any(w => (w.ID_ESTATUS != "CA" ? w.TRASLADO.ORIGEN_TIPO != "F" : false) && w.TRASLADO.TRASLADO_FEC.AddHours(-ParametroToleranciaTraslado) <= Fechas.GetFechaDateServer))
                            {
                                /*(new Dialogos()).ConfirmacionDialogo("Advertencia!", "El imputado [" + item.ACTUARIO_INGRESO.INGRESO.IMPUTADO.NOMBRE.Trim() + " " +
                                    item.ACTUARIO_INGRESO.INGRESO.IMPUTADO.PATERNO.Trim() + " " + item.ACTUARIO_INGRESO.INGRESO.IMPUTADO.MATERNO.Trim() + " - " + item.ACTUARIO_INGRESO.ID_ANIO.ToString() + "/" +
                                        item.ACTUARIO_INGRESO.ID_IMPUTADO.ToString() + "] tiene un traslado próximo y no puede recibir visitas.");
                                return false;*/
                                listaRegresa.Add(item.ACTUARIO_INGRESO.ID_ANIO.ToString() + "/" + item.ACTUARIO_INGRESO.ID_IMPUTADO.ToString());
                                regresa = true;
                                regresaString = regresaString + "El imputado [" + item.ACTUARIO_INGRESO.INGRESO.IMPUTADO.NOMBRE.Trim() + " " +
                                    item.ACTUARIO_INGRESO.INGRESO.IMPUTADO.PATERNO.Trim() + " " + item.ACTUARIO_INGRESO.INGRESO.IMPUTADO.MATERNO.Trim() + " - " + item.ACTUARIO_INGRESO.ID_ANIO.ToString() + "/" +
                                        item.ACTUARIO_INGRESO.ID_IMPUTADO.ToString() + "] tiene un traslado próximo y no puede recibir visitas. \n";
                            }
                            if (item.ACTUARIO_INGRESO.INGRESO.INGRESO_UBICACION.Any(a => (a.ESTATUS == 1 || a.ESTATUS == 2) ? a.ID_AREA == (short)enumAreas.SALA_ABOGADOS || a.ID_AREA == (short)enumAreas.VISITA_LEGAL : false))
                            {
                                /*Application.Current.Dispatcher.Invoke((Action)(delegate
                                {
                                    (new Dialogos()).ConfirmacionDialogo("Advertencia!", "El imputado [" + item.ACTUARIO_INGRESO.INGRESO.IMPUTADO.NOMBRE.Trim() + " " +
                                        item.ACTUARIO_INGRESO.INGRESO.IMPUTADO.PATERNO.Trim() + " " + item.ACTUARIO_INGRESO.INGRESO.IMPUTADO.MATERNO.Trim() + " - " + item.ACTUARIO_INGRESO.ID_ANIO.ToString() + "/" +
                                            item.ACTUARIO_INGRESO.ID_IMPUTADO.ToString() + "] no está disponible para ser notificado, porque se encuentra en una visita legal.");
                                }));
                                return false;*/
                                listaRegresa.Add(item.ACTUARIO_INGRESO.ID_ANIO.ToString() + "/" + item.ACTUARIO_INGRESO.ID_IMPUTADO.ToString());
                                regresa = true;
                                regresaString = regresaString + "El imputado [" + item.ACTUARIO_INGRESO.INGRESO.IMPUTADO.NOMBRE.Trim() + " " +
                                        item.ACTUARIO_INGRESO.INGRESO.IMPUTADO.PATERNO.Trim() + " " + item.ACTUARIO_INGRESO.INGRESO.IMPUTADO.MATERNO.Trim() + " - " + item.ACTUARIO_INGRESO.ID_ANIO.ToString() + "/" +
                                            item.ACTUARIO_INGRESO.ID_IMPUTADO.ToString() + "] no está disponible para ser notificado, porque se encuentra en una visita legal. \n";
                            }
                            //URGENCIA MEDICA
                            if (item.ACTUARIO_INGRESO.INGRESO.INGRESO_UBICACION.Any(a => (a.ESTATUS == 1 || a.ESTATUS == 2) ? a.ID_AREA == (short)enumAreas.MEDICA_PA || a.ID_AREA == (short)enumAreas.MEDICA_PB : false))
                            {
                                /*Application.Current.Dispatcher.Invoke((Action)(delegate
                                {
                                    (new Dialogos()).ConfirmacionDialogo("Advertencia!", "El imputado [" + item.ACTUARIO_INGRESO.INGRESO.IMPUTADO.NOMBRE.Trim() + " " +
                                        item.ACTUARIO_INGRESO.INGRESO.IMPUTADO.PATERNO.Trim() + " " + item.ACTUARIO_INGRESO.INGRESO.IMPUTADO.MATERNO.Trim() + " - " + item.ACTUARIO_INGRESO.ID_ANIO.ToString() + "/" +
                                            item.ACTUARIO_INGRESO.ID_IMPUTADO.ToString() + "] no está disponible para ser notificado, porque se encuentra en el área de salida.");
                                }));
                                return false;*/
                                listaRegresa.Add(item.ACTUARIO_INGRESO.ID_ANIO.ToString() + "/" + item.ACTUARIO_INGRESO.ID_IMPUTADO.ToString());
                                regresa = true;
                                regresaString = regresaString + "El imputado [" + item.ACTUARIO_INGRESO.INGRESO.IMPUTADO.NOMBRE.Trim() + " " +
                                        item.ACTUARIO_INGRESO.INGRESO.IMPUTADO.PATERNO.Trim() + " " + item.ACTUARIO_INGRESO.INGRESO.IMPUTADO.MATERNO.Trim() + " - " + item.ACTUARIO_INGRESO.ID_ANIO.ToString() + "/" +
                                            item.ACTUARIO_INGRESO.ID_IMPUTADO.ToString() + "] no está disponible para ser notificado, porque se encuentra en el área de salida. \n";
                            }
                            if (item.ACTUARIO_INGRESO.INGRESO.INGRESO_UBICACION.Any(a => (a.ESTATUS == 1 || a.ESTATUS == 2) ? a.ID_AREA == (short)enumAreas.EXCARCELACION_TRASLADOS : false))
                            {
                                /*Application.Current.Dispatcher.Invoke((Action)(delegate
                                {
                                    (new Dialogos()).ConfirmacionDialogo("Advertencia!", "El imputado [" + item.ACTUARIO_INGRESO.INGRESO.IMPUTADO.NOMBRE.Trim() + " " +
                                        item.ACTUARIO_INGRESO.INGRESO.IMPUTADO.PATERNO.Trim() + " " + item.ACTUARIO_INGRESO.INGRESO.IMPUTADO.MATERNO.Trim() + " - " + item.ACTUARIO_INGRESO.ID_ANIO.ToString() + "/" +
                                            item.ACTUARIO_INGRESO.ID_IMPUTADO.ToString() + "] no está disponible para ser notificado, porque se encuentra en una urgencia medica.");
                                }));
                                return false;*/
                                listaRegresa.Add(item.ACTUARIO_INGRESO.ID_ANIO.ToString() + "/" + item.ACTUARIO_INGRESO.ID_IMPUTADO.ToString());
                                regresa = true;
                                regresaString = regresaString + "El imputado [" + item.ACTUARIO_INGRESO.INGRESO.IMPUTADO.NOMBRE.Trim() + " " +
                                        item.ACTUARIO_INGRESO.INGRESO.IMPUTADO.PATERNO.Trim() + " " + item.ACTUARIO_INGRESO.INGRESO.IMPUTADO.MATERNO.Trim() + " - " + item.ACTUARIO_INGRESO.ID_ANIO.ToString() + "/" +
                                            item.ACTUARIO_INGRESO.ID_IMPUTADO.ToString() + "] no está disponible para ser notificado, porque se encuentra en una urgencia medica. \n";
                            }
                            /*
                            if (item.ACTUARIO_INGRESO.INGRESO.INCIDENTE.Any(w => w.ESTATUS == "A"))
                            {
                                Application.Current.Dispatcher.Invoke((Action)(delegate
                                {
                                    (new Dialogos()).ConfirmacionDialogo("Advertencia!", "El imputado [" + item.ACTUARIO_INGRESO.INGRESO.IMPUTADO.NOMBRE.Trim() + " " +
                                        item.ACTUARIO_INGRESO.INGRESO.IMPUTADO.PATERNO.Trim() + " " + item.ACTUARIO_INGRESO.INGRESO.IMPUTADO.MATERNO.Trim() + " - " + item.ACTUARIO_INGRESO.ID_ANIO.ToString() + "/" +
                                            item.ACTUARIO_INGRESO.ID_IMPUTADO.ToString() + "] está sancionado y no puede ser notificado.");
                                }));
                                return false;*/
                            /*listaRegresa.Add(item.ACTUARIO_INGRESO.ID_ANIO.ToString() + "/" + item.ACTUARIO_INGRESO.ID_IMPUTADO.ToString());
                            regresa = true;
                            regresaString = regresaString + "El imputado [" + item.ACTUARIO_INGRESO.INGRESO.IMPUTADO.NOMBRE.Trim() + " " +
                                    item.ACTUARIO_INGRESO.INGRESO.IMPUTADO.PATERNO.Trim() + " " + item.ACTUARIO_INGRESO.INGRESO.IMPUTADO.MATERNO.Trim() + " - " + item.ACTUARIO_INGRESO.ID_ANIO.ToString() + "/" +
                                        item.ACTUARIO_INGRESO.ID_IMPUTADO.ToString() + "] está sancionado y no puede ser notificado. \n";
                        }
                        */
                        }
                    }
                }
                #endregion

                #endregion

                if (regresa)
                {
                    if (listaRegresa.Count > 1)
                    {
                        regresaString = regresaString + "\nDesea quitar los imputados seleccionados?";
                        Application.Current.Dispatcher.Invoke((Action)(async delegate
                        {
                            if (await (new Dialogos()).ConfirmacionDosBotonesCustom("Advertencia!", regresaString, "Aceptar", 1, "Cancelar", 0) == 1)
                            {
                                if (ListIngresosAsignados != null)
                                    ListIngresosAsignados = new ObservableCollection<AbogadoIngresoAsignacion>(ListIngresosAsignados.Select(s =>
                                        new AbogadoIngresoAsignacion
                                        {
                                            ELIGE = listaRegresa.Any(a => a == s.ABOGADO_INGRESO.ID_ANIO.ToString() + "/" + s.ABOGADO_INGRESO.ID_IMPUTADO.ToString()) ? false : s.ELIGE,
                                            ABOGADO_INGRESO = s.ABOGADO_INGRESO
                                        }));
                                if (ListInternosSeleccionadosPorNotificar != null)
                                    ListInternosSeleccionadosPorNotificar = new ObservableCollection<ActuarioIngresoAsignacion>(ListInternosSeleccionadosPorNotificar.Select(s =>
                                        new ActuarioIngresoAsignacion
                                        {
                                            ELIGE = listaRegresa.Any(a => a == s.ACTUARIO_INGRESO.ID_ANIO.ToString() + "/" + s.ACTUARIO_INGRESO.ID_IMPUTADO.ToString()) ? false : s.ELIGE,
                                            ACTUARIO_INGRESO = s.ACTUARIO_INGRESO
                                        }));
                            }
                        }));
                    }
                    else
                    {
                        Application.Current.Dispatcher.Invoke((Action)(delegate
                        {
                            (new Dialogos()).ConfirmacionDialogo("Advertencia!", regresaString);
                        }));
                    }
                    return false;
                }
                else
                {
                    if (!string.IsNullOrEmpty(AdminCP)) 
                    {
                        var message1 = await (new Dialogos()).ConfirmacionDialogoReturn("Advertencia!", AdminCP); 
                    }
                }

                #region GUARDAR
                if (SelectBitacoraAcceso != null ? !SelectBitacoraAcceso.ADUANA.NUM_LOCKER.HasValue : true)
                {
                    if (SelectTipoPersona.Value == PersonaVisita)
                    {
                        var UltimaEntradaConLocker = new cAduana().ObtenerIDLocker(hoy, (short)SelectTipoPersona.Value);
                        TextIDLocker = UltimaEntradaConLocker != null ? (UltimaEntradaConLocker.NUM_LOCKER.Value + 1).ToString() : "1";
                        LockerEnabled = false;
                    }
                    else
                    {
                        TextIDLocker = SelectBitacoraAcceso != null ? SelectBitacoraAcceso.ADUANA.NUM_LOCKER.HasValue ? SelectBitacoraAcceso.ADUANA.NUM_LOCKER.Value.ToString() : string.Empty : string.Empty;
                        LockerEnabled = true;
                    }
                }
                else
                {
                    if (SelectTipoPersona.Value == PersonaVisita)
                    {
                        TextIDLocker = SelectBitacoraAcceso != null ? SelectBitacoraAcceso.ADUANA.NUM_LOCKER.HasValue ? SelectBitacoraAcceso.ADUANA.NUM_LOCKER.Value.ToString() : string.Empty : string.Empty;
                        LockerEnabled = true;
                    }
                    else
                        LockerEnabled = false;
                }

                var u = new cUsuario().Obtener(GlobalVar.gUsr);
                var aduana = new ADUANA
                {
                    ID_CENTRO = GlobalVar.gCentro,
                    ID_PERSONA = SelectPersona.ID_PERSONA,
                    ENTRADA_FEC = hoy,
                    ID_TIPO_PERSONA = (short)SelectTipoPersona,
                    NUM_LOCKER = string.IsNullOrEmpty(TextIDLocker) ? new Nullable<short>() : short.Parse(TextIDLocker),
                    ID_USUARIO = u.ID_USUARIO,
                };
                var aduanaIngresos = new List<ADUANA_INGRESO>();

                #region EXTERNA
                if (SelectTipoPersona == ParametroPersonaExterna)
                {
                    aduana.ASUNTO = TextAsuntoExterno;
                    aduana.OBSERV = TextObservacionExterno;
                    aduana.ID_AREA = SelectAreaExterno;
                    aduana.INSTITUCION = TextInstitucionExterno;
                    aduana.DEPARTAMENTO = TextDepartamentoExterno;
                    aduana.ID_PUESTO = SelectPuestoExterno;
                    aduana.DEPARTAMENTO = TextDepartamentoExterno;
                }
                #endregion

                #region EMPLEADO
                if (SelectTipoPersona == ParametroPersonaEmpleado)
                {
                    aduana.ID_PUESTO = SelectPuestoExterno;
                    aduana.DEPARTAMENTO = TextDepartamentoExterno;
                }
                #endregion

                #region ABOGADOS
                if (SelectTipoPersona == ParametroPersonaLegal)
                {
                    var visita = short.Parse(ParametroDoctoInterno[0]);
                    var doctoInterno = short.Parse(ParametroDoctoInterno[1]);
                    var doctoJuez = short.Parse(ParametroDoctoJuez[1]);

                    #region TITULAR
                    if (SelectPersona.ABOGADO.ID_ABOGADO_TITULO == ParametroAbogadoTitular)
                    {
                        aduana.ID_AREA = AreaAbogados;
                        foreach (var item in ListIngresosAsignados.Where(w => w.ELIGE))
                        {
                            aduanaIngresos.Add(new ADUANA_INGRESO
                            {
                                ENTRADA_FEC = hoy,
                                ID_ADUANA = aduana.ID_ADUANA,
                                ID_ANIO = item.ABOGADO_INGRESO.ID_ANIO,
                                ID_CENTRO = item.ABOGADO_INGRESO.ID_CENTRO,
                                ID_IMPUTADO = item.ABOGADO_INGRESO.ID_IMPUTADO,
                                ID_INGRESO = item.ABOGADO_INGRESO.ID_INGRESO,
                                ADMINISTRATIVO = Administrativos.Any(w => w.ID_CENTRO == item.ABOGADO_INGRESO.ID_CENTRO && w.ID_ANIO == item.ABOGADO_INGRESO.ID_ANIO &&
                                    w.ID_IMPUTADO == item.ABOGADO_INGRESO.ID_IMPUTADO && w.ID_INGRESO == item.ABOGADO_INGRESO.ID_INGRESO) ? "S" : "N",
                                CAUSA_PENAL = ListCausasSeleccionadas.Where(w => w.ID_CENTRO == item.ABOGADO_INGRESO.ID_CENTRO && w.ID_ANIO == item.ABOGADO_INGRESO.ID_ANIO &&
                                    w.ID_IMPUTADO == item.ABOGADO_INGRESO.ID_IMPUTADO && w.ID_INGRESO == item.ABOGADO_INGRESO.ID_INGRESO && w.ID_ESTATUS_CP != 4).Any() ? "S" : "N",
                            });
                        }
                    }
                    #endregion

                    #region ACTUARIO
                    if (SelectPersona.ABOGADO.ID_ABOGADO_TITULO == ParametroAbogadoActuario)
                    {
                        aduana.ID_AREA = (short)enumAreas.LOCUTORIOS;
                        foreach (var item in ListInternosSeleccionadosPorNotificar.Where(w => w.ELIGE))
                        {
                            aduanaIngresos.Add(new ADUANA_INGRESO
                            {
                                ENTRADA_FEC = hoy,
                                ID_ANIO = item.ACTUARIO_INGRESO.ID_ANIO,
                                ID_CENTRO = item.ACTUARIO_INGRESO.ID_CENTRO,
                                ID_IMPUTADO = item.ACTUARIO_INGRESO.ID_IMPUTADO,
                                ID_INGRESO = item.ACTUARIO_INGRESO.ID_INGRESO
                            });
                        }
                    }
                    #endregion

                    #region COLABORADOR
                    if (SelectPersona.ABOGADO.ID_ABOGADO_TITULO == ParametroAbogadoColaborador)
                    {
                        aduana.ID_AREA = AreaAbogados;
                        foreach (var item in ListIngresosAsignados.Where(w => w.ELIGE))
                        {
                            aduanaIngresos.Add(new ADUANA_INGRESO
                            {
                                ENTRADA_FEC = hoy,
                                ID_ADUANA = aduana.ID_ADUANA,
                                ID_ANIO = item.ABOGADO_INGRESO.ID_ANIO,
                                ID_CENTRO = item.ABOGADO_INGRESO.ID_CENTRO,
                                ID_IMPUTADO = item.ABOGADO_INGRESO.ID_IMPUTADO,
                                ID_INGRESO = item.ABOGADO_INGRESO.ID_INGRESO,
                                ADMINISTRATIVO = Administrativos.Any(w => w.ID_CENTRO == item.ABOGADO_INGRESO.ID_CENTRO && w.ID_ANIO == item.ABOGADO_INGRESO.ID_ANIO &&
                                    w.ID_IMPUTADO == item.ABOGADO_INGRESO.ID_IMPUTADO && w.ID_INGRESO == item.ABOGADO_INGRESO.ID_INGRESO) ? "S" : "N",
                                CAUSA_PENAL = ListCausasSeleccionadas.Where(w => w.ID_CENTRO == item.ABOGADO_INGRESO.ID_CENTRO && w.ID_ANIO == item.ABOGADO_INGRESO.ID_ANIO &&
                                    w.ID_IMPUTADO == item.ABOGADO_INGRESO.ID_IMPUTADO && w.ID_INGRESO == item.ABOGADO_INGRESO.ID_INGRESO && w.ID_ESTATUS_CP != 4).Any() ? "S" : "N"
                            });
                        }
                    }
                    #endregion
                }
                #endregion

                #region VISITA
                if (SelectTipoPersona == PersonaVisita ? !SoloDeposito : false)
                {
                    if (SelectAreaDestino != null)
                    aduana.ID_AREA = SelectAreaDestino.ID_AREA;
                    foreach (var item in ListadoInternos.Where(w => w.ELEGIDO))
                    {
                        aduanaIngresos.Add(new ADUANA_INGRESO
                        {
                            ENTRADA_FEC = hoy,
                            ID_ADUANA = aduana.ID_ADUANA,
                            ID_ANIO = item.VISITANTE_INGRESO.ID_ANIO,
                            ID_CENTRO = item.VISITANTE_INGRESO.ID_CENTRO,
                            ID_IMPUTADO = item.VISITANTE_INGRESO.ID_IMPUTADO,
                            ID_INGRESO = item.VISITANTE_INGRESO.ID_INGRESO,
                            ID_PERSONA = UtilizaPase > 0 ? item.VISITANTE_INGRESO.ID_PERSONA : new Nullable<int>(),
                            ID_CONSEC = UtilizaPase > 0 ? (short)UtilizaPase : new Nullable<short>(),
                        });
                    }
                }
                #endregion

                aduana.ID_ADUANA = int.Parse(hoy.Year.ToString() + "" + new cAduana().GetSequence<short>("ADUANA_SEQ").ToString("D6"));
                var aduanaAcompanantes = new List<ADUANA_ACOMPANANTE>();
                var aduanaCausaPenal = new List<ADUANA_INGRESO_CP>();
                if (SelectTipoPersona == ParametroPersonaLegal || SelectTipoPersona == PersonaVisita)
                {
                    if (!SoloDeposito)
                    {
                        aduanaIngresos = aduanaIngresos.Select(s => new ADUANA_INGRESO
                        {
                            ENTRADA_FEC = s.ENTRADA_FEC,
                            ID_ANIO = s.ID_ANIO,
                            ID_ADUANA = aduana.ID_ADUANA,
                            ID_CENTRO = s.ID_CENTRO,
                            ID_IMPUTADO = s.ID_IMPUTADO,
                            ID_INGRESO = s.ID_INGRESO,
                            INTIMA = BanderaIntima ? "S" : "N",
                            ADMINISTRATIVO = s.ADMINISTRATIVO,
                            CAUSA_PENAL = s.CAUSA_PENAL,
                            ID_PERSONA = s.ID_PERSONA,
                            ID_CONSEC = s.ID_CONSEC,
                        }).ToList();
                        if (!IsVisitaIntima ? (AcompanantesVisibles == Visibility.Visible ? (ListAcompanantes != null ? (ListAcompanantes.Count > 0) : false) : false) : false)
                            foreach (var item in ListAcompanantes.Where(w => w.ELEGIDO))
                                aduanaAcompanantes.Add(new ADUANA_ACOMPANANTE
                                {
                                    CAPTURA_FEC = hoy,
                                    ID_ADUANA = aduana.ID_ADUANA,
                                    ID_PERSONA = item.ACOMPANANTE.ID_VISITANTE
                                });
                        if (ListCausasSeleccionadas != null ? ListCausasSeleccionadas.Count > 0 : false)
                            foreach (var item in ListCausasSeleccionadas)
                                aduanaCausaPenal.Add(new ADUANA_INGRESO_CP
                                {
                                    ENTRADA_FEC = hoy,
                                    ID_ADUANA = aduana.ID_ADUANA,
                                    ID_ANIO = item.ID_ANIO,
                                    ID_CENTRO = item.ID_CENTRO,
                                    ID_IMPUTADO = item.ID_IMPUTADO,
                                    ID_INGRESO = item.ID_INGRESO,
                                    ID_CAUSA_PENAL = item.ID_CAUSA_PENAL
                                });
                    }
                }

                if (new cAduana().InsertarTransaccion(aduana, aduanaIngresos, aduanaAcompanantes, aduanaCausaPenal))
                {
                    TextIDLockerAuxiliar = TextIDLocker;

                    HideAll();
                    LimpiarCampos();
                    LimpiarCamposAbogados();
                    LimpiarCamposDatosExpediente();
                    LimpiarCamposExternos();
                    LimpiarCamposEmpleados();
                    BotonMenuGuardado = false;
                    (new Dialogos()).ConfirmacionDialogo("Exito!", "Entrada generada satisfactoriamente." + (string.IsNullOrEmpty(TextIDLockerAuxiliar) ? string.Empty : "\n\nLocker: \b" + TextIDLockerAuxiliar));
                }
                else
                {
                    (new Dialogos()).ConfirmacionDialogo("Advertencia!", "No se pudo generar la entrada, favor de ponerse en contacto con el administrador.");
                    return false;
                }
                #endregion
            }
            catch (Exception ex)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al validar y guardar la entrada.", ex);
            }
            return true;
        }

        private async void clickSwitch(Object obj)
        {
            switch (obj.ToString())
            {
                #region HUELLAS
                case "tomar_huellas":
                    PopUpsViewModels.ShowPopUp(this, PopUpsViewModels.TipoPopUp.HUELLAS);
                    this.ShowIdentification();
                    break;
                #endregion

                #region BUSCAR_PERSONAS
                case "nueva_busqueda_visitante":
                    TextPaterno = TextMaterno = TextNombre = string.Empty;
                    ListPersonasAuxiliar = new RangeEnabledObservableCollection<SSP.Servidor.PERSONA>();
                    ListPersonas = new RangeEnabledObservableCollection<SSP.Servidor.PERSONA>();
                    break;
                case "buscar_visita":
                    {
                        var pers1 = SelectPersona;
                        SelectPersonaAuxiliar = pers1;
                        TextCodigo = TextPaterno = TextMaterno = TextNombre = string.Empty;
                        ImagenPersona = new Imagenes().getImagenPerson();
                        PopUpsViewModels.ShowPopUp(this, PopUpsViewModels.TipoPopUp.BUSCAR_PERSONAS_EXISTENTES);
                    }
                    break;
                case "buscar_visitante":
                    if (TextPaterno == null)
                        TextPaterno = string.Empty;
                    if (TextMaterno == null)
                        TextMaterno = string.Empty;
                    if (TextNombre == null)
                        TextNombre = string.Empty;
                    if (TextCodigo == null)
                        TextCodigo = string.Empty;
                    var pers2 = SelectPersona;
                    SelectPersonaAuxiliar = pers2;
                    BuscarPersonasSinCodigo();
                    break;
                case "seleccionar_buscar_persona":
                    try
                    {
                        if (SelectPersona == null)
                        {
                            SelectPersona = SelectPersonaAuxiliar;
                            (new Dialogos()).ConfirmacionDialogo("Advertencia!", "Debes seleccionar a una persona.");
                            break;
                        }
                        else
                        {
                            if (SelectPersona.ID_TIPO_DISCAPACIDAD != null ? SelectPersona.ID_TIPO_DISCAPACIDAD > 0 : false)
                            {
                                if (SelectPersona.TIPO_DISCAPACIDAD.HUELLA == "S")
                                {
                                    PopUpsViewModels.ClosePopUp(PopUpsViewModels.TipoPopUp.BUSCAR_PERSONAS_EXISTENTES);
                                    await new Dialogos().ConfirmacionDialogoReturn("Validación", "Favor de validar la identificación a través la huella digital");
                                    PopUpsViewModels.ShowPopUp(this, PopUpsViewModels.TipoPopUp.BUSCAR_PERSONAS_EXISTENTES);
                                    break;
                                }
                                else
                                {
                                    PopUpsViewModels.ClosePopUp(PopUpsViewModels.TipoPopUp.BUSCAR_PERSONAS_EXISTENTES);
                                    await SeleccionarPersona(SelectPersona);
                                    StaticSourcesViewModel.SourceChanged = false;
                                }
                            }
                            else
                            {
                                PopUpsViewModels.ClosePopUp(PopUpsViewModels.TipoPopUp.BUSCAR_PERSONAS_EXISTENTES);
                                await new Dialogos().ConfirmacionDialogoReturn("Validación", "Favor de validar la identificación a través la huella digital");
                                PopUpsViewModels.ShowPopUp(this, PopUpsViewModels.TipoPopUp.BUSCAR_PERSONAS_EXISTENTES);
                                break;
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al realizar la búsqueda.", ex);
                    }
                    break;
                case "cancelar_buscar_persona":
                    var pers = SelectPersonaAuxiliar;
                    SelectPersona = pers;
                    if (SelectPersona != null)
                    {
                        TextPaterno = SelectPersona.PATERNO.Trim();
                        TextMaterno = string.IsNullOrEmpty(SelectPersona.MATERNO) ? string.Empty : SelectPersona.MATERNO.Trim();
                        TextNombre = SelectPersona.NOMBRE.Trim();
                        TextCodigo = SelectPersona.ID_PERSONA.ToString();
                    }
                    PopUpsViewModels.ClosePopUp(PopUpsViewModels.TipoPopUp.BUSCAR_PERSONAS_EXISTENTES);
                    break;
                case "buscar_por_huella":
                    break;
                #endregion

                #region MENU
                case "limpiar_campos":
                    ((System.Windows.Controls.ContentControl)PopUpsViewModels.MainWindow.FindName("contentControl")).Content = new RecepcionAduanaView();
                    ((System.Windows.Controls.ContentControl)PopUpsViewModels.MainWindow.FindName("contentControl")).DataContext = new ControlPenales.RecepcionAduanaViewModel();
                    /*await StaticSourcesViewModel.CargarDatosMetodoAsync(() =>
                    {
                        base.ClearRules();
                        SalidaEnabled = EntradaEnabled = false;
                        LimpiarCampos();
                        LimpiarCamposAbogados();
                        LimpiarCamposDatosExpediente();
                        LimpiarCamposExternos();
                        LimpiarCamposEmpleados();
                        HideAll();
                        TextIDLockerAuxiliar = string.Empty;
                    });*/
                    break;
                case "salida_ingreso":
                    var _bitacora_acceso = SelectBitacoraAcceso != null ? SelectBitacoraAcceso : SelectPersona.ADUANA.Any(a => !a.SALIDA_FEC.HasValue) ? new BitacoraAduana { ADUANA = SelectPersona.ADUANA.FirstOrDefault(w => !w.SALIDA_FEC.HasValue), PARAMETRO = false } : null;
                    if (_bitacora_acceso == null)
                    {
                        (new Dialogos()).ConfirmacionDialogo("Advertencia!", "Debes seleccionar a un visitante que haya entrado el dia de hoy.");
                        return;
                    }
                    if (SelectPersona == null)
                    {
                        (new Dialogos()).ConfirmacionDialogo("Advertencia!", "Debes seleccionar a un visitante.");
                        return;
                    }
                    if (new cAduana().Actualizar(new ADUANA
                    {
                        ID_CENTRO = _bitacora_acceso.ADUANA.ID_CENTRO,
                        ID_PERSONA = _bitacora_acceso.ADUANA.ID_PERSONA,
                        ENTRADA_FEC = _bitacora_acceso.ADUANA.ENTRADA_FEC,
                        ID_TIPO_PERSONA = _bitacora_acceso.ADUANA.ID_TIPO_PERSONA,
                        ID_ADUANA = _bitacora_acceso.ADUANA.ID_ADUANA,
                        ASUNTO = _bitacora_acceso.ADUANA.ASUNTO,
                        ID_AREA = _bitacora_acceso.ADUANA.ID_AREA,
                        INSTITUCION = _bitacora_acceso.ADUANA.INSTITUCION,
                        NUM_LOCKER = _bitacora_acceso.ADUANA.NUM_LOCKER,
                        OBSERV = _bitacora_acceso.ADUANA.OBSERV,
                        SALIDA_FEC = Fechas.GetFechaDateServer,
                        DEPARTAMENTO = _bitacora_acceso.ADUANA.DEPARTAMENTO,
                        ID_PUESTO = _bitacora_acceso.ADUANA.ID_PUESTO,
                        ID_USUARIO = _bitacora_acceso.ADUANA.ID_USUARIO,
                    }))
                    {
                        var locker = _bitacora_acceso.ADUANA.NUM_LOCKER.HasValue ? _bitacora_acceso.ADUANA.NUM_LOCKER.Value.ToString() : string.Empty;
                        HideAll();
                        LimpiarCampos();
                        LimpiarCamposAbogados();
                        LimpiarCamposDatosExpediente();
                        LimpiarCamposExternos();
                        LimpiarCamposEmpleados();
                        StaticSourcesViewModel.SourceChanged = false;
                        (new Dialogos()).ConfirmacionDialogo("Exito!", "Salida satisfactoria." + (string.IsNullOrEmpty(locker) ? string.Empty : " \n\nLocker: " + locker));
                    }

                    break;
                case "entrada_ingreso":
                    try
                    {
                        if (base.HasErrors)
                        {
                            new Dialogos().ConfirmacionDialogo("Validación", string.Format("Faltan datos por capturar: {0}.", base.Error));
                            return;
                        }
                        BotonMenuGuardado = true;
                        if (await ValidarYGuardarEntrada())
                            TextIDLockerAuxiliar = string.Empty;
                    }
                    catch (Exception ex)
                    {
                        StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al generar la entrada.", ex);
                    }
                    break;
                case "salir_menu":
                    PrincipalViewModel.SalirMenu();
                    break;
                #endregion

                case "actualizar_bitacora":
                    await StaticSourcesViewModel.CargarDatosMetodoAsync(() => { CargarBitacoraAccesos(); });
                    break;
            }
        }

        private bool HasError() { return base.HasErrors; }

        private void ClickEnter(Object obj)
        {
            try
            {
                if (!PConsultar)
                {
                    (new Dialogos()).ConfirmacionDialogo("Advertencia!", "No tienes permisos para hacer consultas.");
                    return;
                }
                base.ClearRules();
                if (obj != null)
                {
                    //cuando es boton no se hace nada porque solamente existe el de buscar, si hay otro habra que castearlos a button y hacer la comparacion
                    var textbox = obj as System.Windows.Controls.TextBox;
                    if (textbox != null)
                    {
                        switch (textbox.Name)
                        {
                            case "NombreBuscar":
                                TextNombre = textbox.Text;
                                break;
                            case "ApellidoPaternoBuscar":
                                TextPaterno = textbox.Text;
                                break;
                            case "ApellidoMaternoBuscar":
                                TextMaterno = textbox.Text;
                                break;
                            case "CodigoBuscar":
                                TextCodigo = textbox.Text;
                                break;
                        }
                    }
                }
                BuscarPersonasSinCodigo();
            }
            catch (Exception ex)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al realizar la búsqueda.", ex);
            }
        }

        private void AduanaEnter(Object obj)
        {
            try
            {
                if (!PConsultar)
                {
                    (new Dialogos()).ConfirmacionDialogo("Advertencia!", "No tienes permisos para hacer consultas.");
                    return;
                }
                base.ClearRules();
                if (obj is TextBox)
                {
                    //cuando es boton no se hace nada porque solamente existe el de buscar, si hay otro habra que castearlos a button y hacer la comparacion
                    var textbox = obj as System.Windows.Controls.TextBox;
                    if (textbox != null)
                    {
                        switch (textbox.Name)
                        {
                            case "NombreBuscar":
                                TextNombre = textbox.Text;
                                break;
                            case "ApellidoPaternoBuscar":
                                TextPaterno = textbox.Text;
                                break;
                            case "ApellidoMaternoBuscar":
                                TextMaterno = textbox.Text;
                                break;
                            case "CodigoBuscar":
                                TextCodigo = textbox.Text;
                                break;
                        }
                    }
                }
                if (string.IsNullOrEmpty(TextCodigo))
                    BuscarPersonasSinCodigo();
                else
                    BuscarPersonas();
            }
            catch (Exception ex)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al realizar la búsqueda.", ex);
            }
        }

        private async void Load_Window(RecepcionAduanaView Window)
        {
            try
            {
                await StaticSourcesViewModel.CargarDatosMetodoAsync(() =>
                {
                    try
                    {
                        #region Parametros
                        /* TIPO VISITA */
                        ParametroVisitaIntima = Parametro.ID_TIPO_VISITA_INTIMA;
                        /* ABOGADOS */
                        ParametroAbogadoTitular = Parametro.ID_ABOGADO_TITULAR_ABOGADO;
                        ParametroAbogadoColaborador = Parametro.ID_ABOGADO_TITULAR_COLABORADOR;
                        ParametroAbogadoActuario = Parametro.ID_ABOGADO_TITULAR_ACTUARIO;
                        /* ESTATUS VISITA */
                        ParametroVisitaAutorizada = Parametro.ID_ESTATUS_VISITA_AUTORIZADO;
                        ParametroVisitaCancelada = Parametro.ID_ESTATUS_VISITA_CANCELADO;
                        ParametroVisitaSuspendido = Parametro.ID_ESTATUS_VISITA_SUSPENDIDO;
                        ParametroVisitaRegistro = Parametro.ID_ESTATUS_VISITA_REGISTRO;
                        /* TIPO PERSONA */
                        ParametroPersonaLegal = short.Parse(Parametro.ID_TIPO_PERSONA_LEGAL);
                        ParametroPersonaVisita = short.Parse(Parametro.ID_TIPO_PERSONA_VISITA);
                        ParametroPersonaExterna = short.Parse(Parametro.ID_TIPO_PERSONA_EXTERNA);
                        ParametroPersonaEmpleado = short.Parse(Parametro.ID_TIPO_PERSONA_EMPLEADO);
                        /* TIPO VISITANTE */
                        ParametroTipoVisitanteIntima = Parametro.ID_TIPO_VISITANTE_INTIMA;
                        ParametroTipoVisitanteDepositante = Parametro.ID_TIPO_VISITANTE_DEPOSITANTE;
                        /* OTROS */
                        ParametroTipoVisitaPorCentro = Parametro.TIPO_VISITA_POR_CENTRO;
                        ParametroInternosPermitidosPorDia = Parametro.INTERNOS_PERMITIDOS_POR_DIA;
                        ParametroVisitaAlaVes = Parametro.VISITA_POR_IMPUTADO;
                        ParametroToleranciaTraslado = Parametro.TOLERANCIA_TRASLADO_EDIFICIO;
                        ParametroTiempoDentroCentro = Parametro.TIEMPO_DENTRO_CENTRO;
                        ParametroDoctoJuez = Parametro.DOCUMENTO_ASIGNACION_JUEZ;
                        ParametroDoctoInterno = Parametro.DOCUMENTO_ASIGNACION_INTERNO;
                        ParametroEstatusInactivos = Parametro.ESTATUS_ADMINISTRATIVO_INACT;
                        ParametroRequiereGuardarHuellas = Parametro.GuardarHuellaEnBusquedaPadronVisita;
                        #endregion
                        ListAreasDestinos = new ObservableCollection<AREA>(new cArea().ObtenerTodos());
                        ListEntidadesAuxiliares = new cEntidad().ObtenerTodos(string.Empty, "S").ToList();
                        ListMunicipiosAuxiliares = new cMunicipio().ObtenerTodos(string.Empty).Where(w => w.ESTATUS == "S").ToList();
                        ListColoniasAuxiliares = new cColonia().ObtenerTodos(string.Empty, new Nullable<short>(), new Nullable<short>()).ToList();
                        CargarDatos();
                        ConfiguraPermisos();
                        StaticSourcesViewModel.SourceChanged = false;
                    }
                    catch (Exception ex)
                    {
                        StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al cargar los datos iniciales.", ex);
                    }
                });
                ListAreasDestinos.Insert(0, new AREA { ID_AREA = -1, DESCR = "SELECCIONE" });
            }
            catch (Exception ex)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al cargar los datos iniciales.", ex);
            }
        }

        private void CargarBitacoraAccesos()
        {
            try
            {
                var hoy = Fechas.GetFechaDateServer;
                var query = new cAduana().ObtenerBitacoraXDia(GlobalVar.gCentro, Fechas.GetFechaDateServer, ParametroTiempoDentroCentro + 5);
                var list = query.ToList();
                ListBitacoraAcceso = new ObservableCollection<BitacoraAduana>(list.Select(s =>
                    new BitacoraAduana
                    {
                        ADUANA = s,
                        PARAMETRO = s.ENTRADA_FEC.HasValue ? s.ENTRADA_FEC.Value < hoy.AddHours(-ParametroTiempoDentroCentro) : false,
                        CAMA = s.ADUANA_INGRESO.Any() ? s.ADUANA_INGRESO.FirstOrDefault().INGRESO.CAMA : new CAMA()
                    }));
            }
            catch (Exception ex)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al cargar la bitacora de accesos.", ex);
            }
        }

        private void CargarDatos()
        {
            try
            {
                ListPais = new ObservableCollection<PAIS_NACIONALIDAD>(new cPaises().ObtenerTodos());
                ListDiscapacidades = new ObservableCollection<TIPO_DISCAPACIDAD>(new cTipoDiscapacidad().ObtenerTodos());
                ListTipoPersona = new cTipoPersona().ObtenerTodos();
                ListSituacion = ListEstatusVisita = new ObservableCollection<ESTATUS_VISITA>(new cEstatusVisita().ObtenerTodos());
                ListAreaExterno = new ObservableCollection<AREA>(new cArea().ObtenerTodos());
                ListPuestos = new ObservableCollection<PUESTO>(new cPuesto().ObtenerTodos());
                //ListDeptos = new ObservableCollection<AREA_TRABAJO>(new cAreaTrabajo().ObtenerTodos());
                ListLetras = new List<string>(new string[] { "A", "B", "C", "D", "E", "F", "G", "H", "I", "J", "K", "L", "M", "N", "O", "P", "Q", "R", "S", "T", "U", "V", "W", "X", "Y", "Z" });
                CargarBitacoraAccesos();
                Application.Current.Dispatcher.Invoke((Action)(delegate
                {
                    try
                    {
                        ListEstatusVisita.Insert(0, new ESTATUS_VISITA { ID_ESTATUS_VISITA = -1, DESCR = "SELECCIONE" });
                        ListPais.Insert(0, new PAIS_NACIONALIDAD { ID_PAIS_NAC = -1, PAIS = "SELECCIONE" });
                        ListAreaExterno.Insert(0, new AREA { ID_AREA = -1, DESCR = "SELECCIONE" });
                        ListPuestos.Insert(0, new PUESTO { ID_PUESTO = -1, DESCR = "SELECCIONE" });
                        SelectPuestoExterno = -1;
                        SelectSituacion = -1;
                        SelectDiscapacitado = "N";
                        SelectPais = Parametro.PAIS; //82;
                        CodigoEnabled = NombreReadOnly = true;
                        StaticSourcesViewModel.SourceChanged = false;
                    }
                    catch (Exception ex)
                    {
                        StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al cargar los datos iniciales.", ex);
                    }
                }));
            }
            catch (Exception ex)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al cargar los datos iniciales.", ex);
            }
        }

        private async void BuscarPersonas()
        {
            try
            {
                var persona = SelectPersona;
                ListPersonasAuxiliar = new RangeEnabledObservableCollection<SSP.Servidor.PERSONA>();
                ListPersonas = new RangeEnabledObservableCollection<SSP.Servidor.PERSONA>();
                ListPersonas.InsertRange(await SegmentarPersonasBusqueda());
                ListPersonasAuxiliar.InsertRange(ListPersonas);
                SelectPersonaAuxiliar = persona;
                if (ListPersonas.Count == 1)
                {
                    #region Validaciones
                    var x = ListPersonas.FirstOrDefault();
                    if (x != null)
                    {
                        if (x.ID_TIPO_DISCAPACIDAD != null)
                        {
                            if (x.ID_TIPO_DISCAPACIDAD == 0)
                            {
                                new Dialogos().ConfirmacionDialogo("Validación", "Favor de validar la entrada con la huella digital");
                                //EmptyBuscarRelacionInternoVisible = ListPersonas != null ? ListPersonas.Count <= 0 : false;
                                //PopUpsViewModels.ShowPopUp(this, PopUpsViewModels.TipoPopUp.BUSCAR_PERSONAS_EXISTENTES);
                                return;
                            }
                            else
                                if (x.TIPO_DISCAPACIDAD.HUELLA == "S")
                                {
                                    new Dialogos().ConfirmacionDialogo("Validación", "Favor de validar la entrada con la huella digital");
                                    //EmptyBuscarRelacionInternoVisible = ListPersonas != null ? ListPersonas.Count <= 0 : false;
                                    //PopUpsViewModels.ShowPopUp(this, PopUpsViewModels.TipoPopUp.BUSCAR_PERSONAS_EXISTENTES);
                                    return;
                                }
                        }
                        else
                        {
                            new Dialogos().ConfirmacionDialogo("Validación", "Favor de validar la entrada con la huella digital");
                            //EmptyBuscarRelacionInternoVisible = ListPersonas != null ? ListPersonas.Count <= 0 : false;
                            //PopUpsViewModels.ShowPopUp(this, PopUpsViewModels.TipoPopUp.BUSCAR_PERSONAS_EXISTENTES);
                            return;
                        }
                    }
                    #endregion

                    await SeleccionarPersona(ListPersonas.FirstOrDefault());
                    StaticSourcesViewModel.SourceChanged = false;
                }
                else
                {
                }
            }
            catch (Exception ex)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al traer datos del visitante.", ex);
            }
        }

        private async void BuscarPersonasSinCodigo()
        {
            try
            {
                var person = SelectPersona;
                ListPersonasAuxiliar = new RangeEnabledObservableCollection<SSP.Servidor.PERSONA>();
                ListPersonas = new RangeEnabledObservableCollection<SSP.Servidor.PERSONA>();
                ListPersonas.InsertRange(await SegmentarPersonasBusqueda());
                ListPersonasAuxiliar.InsertRange(ListPersonas);
                if (PopUpsViewModels.VisibleBuscarPersonasExistentes == Visibility.Collapsed)
                {
                    SelectPersonaAuxiliar = person;
                    PopUpsViewModels.ShowPopUp(this, PopUpsViewModels.TipoPopUp.BUSCAR_PERSONAS_EXISTENTES);
                }
                EmptyBuscarRelacionInternoVisible = !(ListPersonas.Count > 0);
            }
            catch (Exception ex)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al traer datos del visitante.", ex);
            }
        }

        private async Task<List<SSP.Servidor.PERSONA>> SegmentarPersonasBusqueda(int _Pag = 1)
        {
            try
            {
                if (string.IsNullOrEmpty(TextPaterno) && string.IsNullOrEmpty(TextMaterno) && string.IsNullOrEmpty(TextNombre) && string.IsNullOrEmpty(TextCodigo))
                    return new List<SSP.Servidor.PERSONA>();
                Pagina = _Pag;
                var legal = ParametroPersonaLegal;
                var result = await StaticSourcesViewModel.CargarDatosAsync<ObservableCollection<SSP.Servidor.PERSONA>>(() =>
                        new ObservableCollection<SSP.Servidor.PERSONA>(new cPersona().ObtenerTodos(TextNombre, TextPaterno, TextMaterno, string.IsNullOrEmpty(TextCodigo) ? new Nullable<int>() : int.Parse(TextCodigo), _Pag)
                            .OrderByDescending(o => o.VISITANTE != null).ThenByDescending(t => t.ABOGADO != null).ThenByDescending(t => t.EMPLEADO != null)));
                Pagina = result.Any() ? Pagina + 1 : Pagina;
                SeguirCargandoPersonas = result.Any();
                return result.ToList();
            }
            catch (Exception ex)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al querer guardar.", ex);
                return new List<SSP.Servidor.PERSONA>();
            }
        }

        private void TipoVisitaKeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            try
            {
                if (e.Key == System.Windows.Input.Key.Escape)
                    SeleccionarTipoVisitaAduana.Close();
            }
            catch (Exception ex)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al buscar por huella.", ex);
            }
        }

        private async Task SeleccionarPersona(SSP.Servidor.PERSONA persona)
        {
            try
            {
                var i = SelectTipoPersona = 0;
                await StaticSourcesViewModel.CargarDatosMetodoAsync(() =>
                {
                    #region GetTiposPersona
                    VisitaLegalEnabled = VisitaEmpleadoEnabled = VisitaExternaEnabled = VisitaFamiliarEnabled = false;
                    if (persona.ABOGADO != null)
                    {
                        i++;
                        VisitaLegalEnabled = true;
                    }
                    if (persona.EMPLEADO != null)
                    {
                        i++;
                        VisitaEmpleadoEnabled = true;
                    }
                    //Modificacion de modelo, PENDIENTE
                    //if (persona.PERSONA_EXTERNO != null ? persona.PERSONA_EXTERNO.Count > 0 : false)
                    if (persona.PERSONA_EXTERNO != null)
                    {
                        i++;
                        VisitaExternaEnabled = true;
                    }
                    //////////////////////////////////////////////////
                    if (persona.VISITANTE != null)
                    {
                        i++;
                        VisitaFamiliarEnabled = true;
                    }
                    if (i == 0)
                    {
                        (new Dialogos()).ConfirmacionDialogo("Advertencia!", "La persona seleccionada no cuenta con un registro correcto.");
                        return;
                    }
                    #endregion
                    var aduana = new cAduana().ObtenerUltimaEntradaSinSalida(persona.ID_PERSONA);
                    SelectBitacoraAcceso = aduana != null ? new BitacoraAduana { ADUANA = aduana, PARAMETRO = aduana.SALIDA_FEC.HasValue } : null;
                    SalidaEnabled = SelectBitacoraAcceso != null;
                    EntradaEnabled = !SalidaEnabled;
                });

                #region DefineTipoPersona
                if (EntradaEnabled)
                {
                    if (i == 1)
                    {
                        SelectTipoPersona = persona.ID_TIPO_PERSONA;
                    }
                    else
                    {
                        SelectTipoPersona = new Nullable<int>();
                        SeleccionarTipoVisitaAduana = new SeleccionarTipoVisitaAduanaView();
                        SeleccionarTipoVisitaAduana.DataContext = this;
                        SeleccionarTipoVisitaAduana.btnEmpleado.Click += SelectTipoVisita;
                        SeleccionarTipoVisitaAduana.btnExterna.Click += SelectTipoVisita;
                        SeleccionarTipoVisitaAduana.btnFamiliar.Click += SelectTipoVisita;
                        SeleccionarTipoVisitaAduana.btnLegal.Click += SelectTipoVisita;
                        SeleccionarTipoVisitaAduana.KeyDown += TipoVisitaKeyDown;
                        SeleccionarTipoVisitaAduana.Owner = PopUpsViewModels.MainWindow;
                        SeleccionarTipoVisitaAduana.WindowStartupLocation = WindowStartupLocation.CenterOwner;
                        PopUpsViewModels.ShowPopUp(this, PopUpsViewModels.TipoPopUp.OSCURECER_FONDO);
                        SeleccionarTipoVisitaAduana.ShowDialog();
                        SeleccionarTipoVisitaAduana.btnEmpleado.Click -= SelectTipoVisita;
                        SeleccionarTipoVisitaAduana.btnExterna.Click -= SelectTipoVisita;
                        SeleccionarTipoVisitaAduana.btnFamiliar.Click -= SelectTipoVisita;
                        SeleccionarTipoVisitaAduana.btnLegal.Click -= SelectTipoVisita;
                        SeleccionarTipoVisitaAduana.KeyDown -= TipoVisitaKeyDown;
                        SeleccionarTipoVisitaAduana = null;
                        PopUpsViewModels.ClosePopUp(PopUpsViewModels.TipoPopUp.OSCURECER_FONDO);
                    }
                }
                else
                {
                    SelectTipoPersona = SelectBitacoraAcceso.ADUANA.ID_TIPO_PERSONA;
                }

                #endregion

                HideAll();
                var tipoP = SelectTipoPersona.HasValue ? SelectTipoPersona.Value : new Nullable<int>();
                if (tipoP > 0)
                {
                    LimpiarCampos();
                    SelectTipoPersona = tipoP;
                    SelectPersona = persona;
                    CodigoEnabled = NombreReadOnly = false;
                    GeneralEnabled = ValidarEnabled = DiscapacitadoEnabled = true;
                    TextIDLocker = SelectBitacoraAcceso != null ? (SelectBitacoraAcceso.ADUANA.NUM_LOCKER.HasValue ? SelectBitacoraAcceso.ADUANA.NUM_LOCKER.Value.ToString() : string.Empty) : string.Empty;

                    SelectPersona = persona;
                    SalidaEnabled = SelectBitacoraAcceso != null;
                    EntradaEnabled = !SalidaEnabled;
                    if (SalidaEnabled)
                    {
                        SalidaEnabled = false;
                        if (await new Dialogos().ConfirmarEliminar("Advertencia!", "La persona seleccionada tiene un ingreso sin salida registrada. Está seguro que desea darle entrada?") == 0)
                        {
                            return;
                        }
                        SelectBitacoraAcceso = null;
                        EntradaEnabled = true;
                    }
                    await StaticSourcesViewModel.CargarDatosMetodoAsync(() =>
                    {
                        GetDatosPersonaSeleccionada(EntradaEnabled);
                        StaticSourcesViewModel.SourceChanged = false;
                        PopUpsViewModels.ClosePopUp(PopUpsViewModels.TipoPopUp.BUSCAR_PERSONAS_EXISTENTES);
                    });

                    if (SelectTipoPersona == ParametroPersonaEmpleado)
                    {
                        setValidacionesEmpleado();
                        SetValidacionesVisitaFamiliar();
                        foreach (var item in SelectPersona.VISITANTE.VISITANTE_INGRESO.Where(w => !ParametroEstatusInactivos.Contains(w.INGRESO.ID_ESTATUS_ADMINISTRATIVO)))
                        {
                            var diaVisita = ValidarDiaVisita(item);
                            SelectAreaDestino = (diaVisita.HasValue ? diaVisita.Value : false) ?
                                ListAreasDestinos.First(w => w.ID_AREA == (SelectAreaDestinoAuxiliar > 0 ? SelectAreaDestinoAuxiliar : (short)-1))
                            : ListAreasDestinos.First(w => w.ID_AREA == -1);
                            break;
                        }
                    }
                }
                else
                {
                    (new Dialogos()).ConfirmacionDialogo("Advertencia!", "La persona seleccionada no cuenta con un registro correcto.");
                    return;
                }
            }
            catch (Exception ex)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al buscar por huella.", ex);
            }
        }

        private void SelectTipoVisita(object s, RoutedEventArgs e)
        {
            try
            {
                if (s is Button)
                {
                    switch (((Button)s).CommandParameter.ToString())
                    {
                        case "visita_familiar":
                            LockerEnabled = false;
                            SelectTipoPersona = ParametroPersonaVisita;
                            //setValidacionesEmpleado();
                            break;
                        case "visita_empleado":
                            LockerEnabled = true;
                            SelectTipoPersona = ParametroPersonaEmpleado;
                            break;
                        case "visita_externa":
                            LockerEnabled = true;
                            SelectTipoPersona = ParametroPersonaExterna;
                            break;
                        case "visita_legal":
                            LockerEnabled = true;
                            SelectTipoPersona = ParametroPersonaLegal;
                            break;
                    }
                    SeleccionarTipoVisitaAduana.Close();
                }
            }
            catch (Exception ex)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al seleccionar el tipo de visita.", ex);
            }
        }

        private void SeleccionarVisitante(Object obj)
        {
            try
            {
                if (obj != null ? obj is DataGrid : false)
                {
                    if (((DataGrid)obj).SelectedItem is BitacoraAduana)
                    {
                        if (SelectBitacoraAcceso == null)
                        {
                            (new Dialogos()).ConfirmacionDialogo("Advertencia!", "Debes seleccionar una entrada de un visitante.");
                            return;
                        }

                        HideAll();
                        var rowSelected = (BitacoraAduana)((DataGrid)obj).SelectedItem;
                        var tipoP = SelectTipoPersona.HasValue ? SelectTipoPersona.Value : new Nullable<int>();
                        LimpiarCampos();
                        SelectTipoPersona = rowSelected.ADUANA.ID_TIPO_PERSONA;
                        SelectPersona = rowSelected.ADUANA.PERSONA;
                        GeneralEnabled = ValidarEnabled = DiscapacitadoEnabled = true;
                        //Application.Current.Dispatcher.Invoke((Action)(delegate { 
                        GetDatosPersonaSeleccionada(false);// }));
                        SalidaEnabled = true;
                        CodigoEnabled = NombreReadOnly = EntradaEnabled = StaticSourcesViewModel.SourceChanged = false;
                        var UltimaEntradaConLocker = new ADUANA();
                        if (SelectBitacoraAcceso != null ? SelectBitacoraAcceso.ADUANA.NUM_LOCKER.HasValue : false)
                            TextIDLocker = SelectBitacoraAcceso.ADUANA.NUM_LOCKER.Value.ToString();
                    }
                    else
                        (new Dialogos()).ConfirmacionDialogo("Advertencia!", "Debes seleccionar una entrada de un visitante.");
                }
            }
            catch (Exception ex)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al cargar datos del visitante.", ex);
            }
        }

        private async void LimpiarCampos()
        {
            TextCodigo = TextPaterno = TextMaterno = TextNombre = TextCurp = TextRfc = TextTelefono = TextCorreo =
                TextCedulaCJF = TextCalle = TextNumInt = SelectDiscapacitado = TextIDLocker = string.Empty;
            TextNip = new Nullable<short>();
            Administrativos = new List<ABOGADO_INGRESO>();
            FechaNacimiento = Fechas.GetFechaDateServer;
            TextFechaUltimaModificacion = Fechas.GetFechaDateServer.ToString("dd/MM/yyyy");
            SelectSexo = "S";
            SelectPais = Parametro.PAIS; //82;
            SelectSituacion = -1;
            ImagenPersona = FotoVisita = new Imagenes().getImagenPerson();
            SelectPersona = null;
            HuellasCapturadas = null;
            ListPersonasAuxiliar = null;
            ListPersonas = null;
            SeleccionarTodoInternos = false;
            SeleccionarTodosEnable = false;
            BanderaSelectAll = false;
            LockerEnabled = false;
            SalidaEnabled = false;
            BanderaIntima = false;
            EntradaEnabled = false;
            AcompanantesVisibles = Visibility.Visible;
            TextNumExt = TextCodigoPostal = SelectTipoPersona = new Nullable<int>();
            CodigoEnabled = true;
            NombreReadOnly = true;
            IsVisitaIntima = false;
            await StaticSourcesViewModel.CargarDatosMetodoAsync(() => { CargarBitacoraAccesos(); });
        }

        private void LimpiarCamposDatosExpediente()
        {
            AnioD = FolioD = NombreD = MaternoD = PaternoD = IngresosD = NoControlD = UbicacionD = FecIngresoD =
                ClasificacionJuridicaD = TipoSeguridadD = EstatusD = string.Empty;
        }

        private void LimpiarCamposAbogados()
        {
            TextObservacionesColaborador = TextNumeroAbogadoTitular = TextNombreAbogadoTitular = TextNombreImputado = TextPaternoImputado = TextMaternoImputado =
                TextFolioImputado = TextAnioImputado = TextJuzgadoActuario = TextObservacionesAbogado = CedulaCJF = TextCedulaCJF = string.Empty;
            CredencializadoColaborador = CredencializadoAbogado = false;
            SelectEstatusColaborador = SelectEstatusAbogado = new Nullable<short>();
            ListIngresosAsignados = new ObservableCollection<AbogadoIngresoAsignacion>();
            SelectAbogadoIngreso = null;
            ImagenInterno = new Imagenes().getImagenPerson();
            ListCausasAsignadas = null;
            ListCausasSeleccionadas = new ObservableCollection<CAUSA_PENAL>();
            SelectCausaAsignada = null;
        }

        private void LimpiarCamposExternos()
        {
            TextInstitucionExterno = TextFechaExterno = TextHoraExterno = TextAsuntoExterno =
                TextObservacionExterno = TextDepartamentoExterno = string.Empty;
            SelectPuestoExterno = -1;
        }

        private void LimpiarCamposEmpleados()
        {
            TextPuestoEmpleado = string.Empty;
            SelectDeptoEmpleado = (short)0;
        }

        private void GetDatosAbogadoTitular()
        {
            CedulaCJF = "Cedula";
            TextCedulaCJF = SelectPersona.ABOGADO.CEDULA;
            CredencializadoAbogado = SelectPersona.ABOGADO.CREDENCIALIZADO == "S";
            SelectSituacion = SelectPersona.ABOGADO.ID_ESTATUS_VISITA.HasValue ? SelectPersona.ABOGADO.ID_ESTATUS_VISITA.Value : (short)-1;
            TextObservacionesAbogado = SelectPersona.ABOGADO.OBSERV;
            ListIngresosAsignados = new ObservableCollection<AbogadoIngresoAsignacion>(SelectPersona.ABOGADO.ABOGADO_INGRESO.Where(w =>
                w.ID_ESTATUS_VISITA == ParametroVisitaAutorizada && w.INGRESO.ID_UB_CENTRO == GlobalVar.gCentro &&
                !ParametroEstatusInactivos.Any(a => a.HasValue ? a.Value == w.INGRESO.ID_ESTATUS_ADMINISTRATIVO : false) &&
                !w.INGRESO.TRASLADO_DETALLE.Any(wh => (wh.ID_ESTATUS != "CA" ? wh.TRASLADO.ORIGEN_TIPO != "F" : false) &&
                    wh.TRASLADO.TRASLADO_FEC.AddHours(-ParametroToleranciaTraslado) <= Fechas.GetFechaDateServer))
                .Select(s => new AbogadoIngresoAsignacion { ABOGADO_INGRESO = s, ELIGE = false }));
            SelectAbogadoIngreso = null;
            ImagenInterno = new Imagenes().getImagenPerson();
            ListCausasAsignadas = null;
            SelectCausaAsignada = null;
            SeleccionarTodoAbogados = false;
        }

        private void GetDatosAbogadoColaborador()
        {
            SelectSituacion = SelectPersona.ABOGADO.ID_ESTATUS_VISITA.HasValue ? SelectPersona.ABOGADO.ID_ESTATUS_VISITA.Value : (short)-1;
            CredencializadoColaborador = SelectPersona.ABOGADO.CREDENCIALIZADO == "S";
            TextObservacionesColaborador = SelectPersona.ABOGADO.OBSERV;
            TextNumeroAbogadoTitular = SelectPersona.ABOGADO.ABOGADO_TITULAR.HasValue ? SelectPersona.ABOGADO.ABOGADO_TITULAR.Value.ToString() : "";
            TextNombreAbogadoTitular = SelectPersona.PATERNO.Trim() + " " + SelectPersona.MATERNO.Trim() + " " + SelectPersona.NOMBRE.Trim();
            ListIngresosAsignados = new ObservableCollection<AbogadoIngresoAsignacion>(SelectPersona.ABOGADO.ABOGADO_INGRESO.Where(w =>
                w.ID_ESTATUS_VISITA == ParametroVisitaAutorizada && w.INGRESO.ID_UB_CENTRO == GlobalVar.gCentro &&
                !ParametroEstatusInactivos.Any(a => a.HasValue ? a.Value == w.INGRESO.ID_ESTATUS_ADMINISTRATIVO : false) &&
                !w.INGRESO.TRASLADO_DETALLE.Any(wh => (wh.ID_ESTATUS != "CA" ? wh.TRASLADO.ORIGEN_TIPO != "F" : false) && wh.TRASLADO.TRASLADO_FEC.AddHours(-ParametroToleranciaTraslado) <= Fechas.GetFechaDateServer))
                .Select(s => new AbogadoIngresoAsignacion { ABOGADO_INGRESO = s, ELIGE = false }));
            SelectAbogadoIngreso = null;
            ImagenInterno = new Imagenes().getImagenPerson();
            ListCausasAsignadas = null;
            SelectCausaAsignada = null;
            SeleccionarTodoAbogados = false;
        }

        private void GetDatosAbogadoActuario()
        {
            var hoy = Fechas.GetFechaDateServer;
            CedulaCJF = "CJF";
            TextCedulaCJF = SelectPersona.ABOGADO.CJF;
            TextJuzgadoActuario = SelectPersona.ABOGADO.JUZGADO != null ? SelectPersona.ABOGADO.JUZGADO.DESCR : "";
            CredencializadoAbogado = SelectPersona.ABOGADO.CREDENCIALIZADO == "S";
            SelectSituacion = SelectPersona.ABOGADO.ID_ESTATUS_VISITA.HasValue ? SelectPersona.ABOGADO.ID_ESTATUS_VISITA.Value : (short)-1;
            TextObservacionesAbogado = SelectPersona.ABOGADO.OBSERV;
            var Internos = new cActuarioIngreso().ObtenerXActuarioYFecha(SelectPersona.ID_PERSONA, Fechas.GetFechaDateServer).Where(w => w.INGRESO.ID_UB_CENTRO == GlobalVar.gCentro &&
                !ParametroEstatusInactivos.Any(a => a.HasValue ? a.Value == w.INGRESO.ID_ESTATUS_ADMINISTRATIVO : false)).AsEnumerable().Where(w =>
                    !w.INGRESO.TRASLADO_DETALLE.Any(wh => (wh.ID_ESTATUS != "CA" ? wh.TRASLADO.ORIGEN_TIPO != "F" : false) && wh.TRASLADO.TRASLADO_FEC.AddHours(-ParametroToleranciaTraslado) <= Fechas.GetFechaDateServer))
                .Select(s => new ActuarioIngresoAsignacion { ACTUARIO_INGRESO = s, ELIGE = true });
            ListInternosSeleccionadosPorNotificarAuxiliar = new ObservableCollection<ActuarioIngresoAsignacion>(Internos.Select(s =>
                new ActuarioIngresoAsignacion { ACTUARIO_INGRESO = s.ACTUARIO_INGRESO, ELIGE = true }));
            ListInternosSeleccionadosPorNotificar = new ObservableCollection<ActuarioIngresoAsignacion>(ListInternosSeleccionadosPorNotificarAuxiliar);
            SeleccionarTodoActuarios = ListInternosSeleccionadosPorNotificar.Count > 0;
        }

        private async Task<bool> GetDatosVisitaFamiliar(bool hideAll)
        {
            try
            {
                LockerEnabled = false;
                //var estatusInactivos = Parametro.ESTATUS_ADMINISTRATIVO_INACT;
                ListadoInternos = new ObservableCollection<VisitanteIngresoAsignacion>(SelectPersona.VISITANTE.VISITANTE_INGRESO.Where(w => w.INGRESO.ID_UB_CENTRO == GlobalVar.gCentro ?
                    !ParametroEstatusInactivos.Contains(w.INGRESO.ID_ESTATUS_ADMINISTRATIVO)
                : false)
                    .Select(s => new VisitanteIngresoAsignacion { VISITANTE_INGRESO = s, ELEGIDO = false }));
                if (ListadoInternos.Count == 1)
                {
                    if (ParametroEstatusInactivos.Any(a => a.HasValue ? ListadoInternos[0].VISITANTE_INGRESO.INGRESO.ID_ESTATUS_ADMINISTRATIVO == a.Value : a.HasValue))
                    {
                        new Dialogos().ConfirmacionDialogo("Notificación!", "El ingreso seleccionado no está activo.");
                        return hideAll;
                    }
                    if (ListadoInternos[0].VISITANTE_INGRESO.INGRESO.TRASLADO_DETALLE.Any(a => (a.ID_ESTATUS != "CA" ? a.TRASLADO.ORIGEN_TIPO != "F" : false) && a.TRASLADO.TRASLADO_FEC.AddHours(-ParametroToleranciaTraslado) <= Fechas.GetFechaDateServer))
                    {
                        new Dialogos().ConfirmacionDialogo("Notificación!", "El ingreso seleccionado tiene un traslado pendiente y no puede recibir visitas.");
                        return hideAll;
                    }
                    if (ListadoInternos[0].VISITANTE_INGRESO.INGRESO.ID_UB_CENTRO != GlobalVar.gCentro)
                    {
                        new Dialogos().ConfirmacionDialogo("Notificación!", "El ingreso seleccionado no pertenece a este centro.");
                        return hideAll;
                    }
                    SelectInternoFamiliar = ListadoInternos.FirstOrDefault();
                    ListadoInternos = new ObservableCollection<VisitanteIngresoAsignacion>(ListadoInternos.Select(s => new VisitanteIngresoAsignacion
                    {
                        ELEGIDO = true,
                        VISITANTE_INGRESO = s.VISITANTE_INGRESO
                    }));
                    SeleccionarTodoInternos = true;
                    ListAcompanantes = new ObservableCollection<AcompananteAsignacion>(ListadoInternos.FirstOrDefault().VISITANTE_INGRESO.ACOMPANANTE.Select(s =>
                        new AcompananteAsignacion { ACOMPANANTE = s, ELEGIDO = true }));
                    SeleccionarTodoAcompanantes = BanderaSelectAll = ListAcompanantes.Count > 0;
                    SelectInternoFamiliar = ListadoInternos.FirstOrDefault();
                    if (ListadoInternos.FirstOrDefault().VISITANTE_INGRESO.ID_TIPO_VISITANTE == ParametroTipoVisitanteIntima ?
                        ValidarVisitaIntima(ListadoInternos.FirstOrDefault().VISITANTE_INGRESO) : false)
                    {
                        IsVisitaIntima = true;
                        if (await ValidarYGuardarEntrada())
                        {
                            // validar si cumple con las validaciones y generar entrada
                            Application.Current.Dispatcher.Invoke((Action)(delegate
                            {
                                HideAll();
                                hideAll = false;
                                LimpiarCampos();
                                LimpiarCamposAbogados();
                                LimpiarCamposDatosExpediente();
                                LimpiarCamposExternos();
                                LimpiarCamposEmpleados();
                                (new Dialogos()).ConfirmacionDialogo("Exito!", "Entrada generada satisfactoriamente." + (string.IsNullOrEmpty(TextIDLockerAuxiliar) ? string.Empty : "\n\nLocker: \b" + TextIDLockerAuxiliar));
                                TextIDLockerAuxiliar = string.Empty;
                            }));
                            return hideAll;
                        }
                    }
                    else
                    {
                        IsVisitaIntima = false;
                        if (ListAcompanantes != null ? ListAcompanantes.Count <= 0 ? await ValidarYGuardarEntrada() : false : false)
                        {
                            // validar si cumple con las validaciones y generar entrada
                            Application.Current.Dispatcher.Invoke((Action)(delegate
                            {
                                HideAll();
                                hideAll = false;
                                LimpiarCampos();
                                LimpiarCamposAbogados();
                                LimpiarCamposDatosExpediente();
                                LimpiarCamposExternos();
                                LimpiarCamposEmpleados();
                                (new Dialogos()).ConfirmacionDialogo("Exito!", "Entrada generada satisfactoriamente." + (string.IsNullOrEmpty(TextIDLockerAuxiliar) ? string.Empty : "\n\nLocker: \b" + TextIDLockerAuxiliar));
                                TextIDLockerAuxiliar = string.Empty;
                            }));
                            return hideAll;
                        }
                    }
                }
                else
                {
                    SelectInternoFamiliar = null;
                    SeleccionarTodoInternos = false;
                    SeleccionarTodoAcompanantes = SeleccionarTodoAcompanantesEnabled = BanderaSelectAll = false;
                }
                SelectAcompanante = ListAcompanantes != null ? ListAcompanantes.Count > 0 ? ListAcompanantes.FirstOrDefault() : null : null;
                SelectSituacion = SelectPersona.VISITANTE.ID_ESTATUS_VISITA.HasValue ? SelectPersona.VISITANTE.ID_ESTATUS_VISITA.Value : (short)-1;
                var hoy = Convert.ToInt32(Fechas.GetFechaDateServer.DayOfWeek) + 1;
                foreach (var item in ListadoInternos)
                {
                    if (ValidarVisitaIntima(item.VISITANTE_INGRESO))
                    {
                        Application.Current.Dispatcher.Invoke((Action)(delegate
                        {
                            AcompanantesVisibles = Visibility.Collapsed;
                        }));
                        break;
                    }
                    else
                        Application.Current.Dispatcher.Invoke((Action)(delegate
                        {
                            AcompanantesVisibles = Visibility.Visible;
                        }));
                }
            }
            catch (Exception ex)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al traer datos del visitante.", ex);
            }
            //return TaskEx.FromResult(true);
            return hideAll;
        }

        private async void GetDatosPersonaSeleccionada(bool hideAll)
        {
            try
            {
                TextCodigo = SelectPersona.ID_PERSONA.ToString();
                TextPaterno = SelectPersona.PATERNO.Trim();
                TextMaterno = SelectPersona.MATERNO.Trim();
                TextNombre = SelectPersona.NOMBRE.Trim();
                SelectSexo = SelectPersona.SEXO;
                FechaNacimiento = SelectPersona.FEC_NACIMIENTO.HasValue ? SelectPersona.FEC_NACIMIENTO.Value : Fechas.GetFechaDateServer;
                TextCurp = SelectPersona.CURP;
                TextRfc = SelectPersona.RFC;
                TextTelefono = SelectPersona.TELEFONO;
                TextCorreo = SelectPersona.CORREO_ELECTRONICO;
                SelectDiscapacitado = SelectPersona.ID_TIPO_DISCAPACIDAD.HasValue ? SelectPersona.ID_TIPO_DISCAPACIDAD.Value > 0 ? "S" : "N" : "N";
                SelectTipoDiscapacidad = (short)(SelectPersona.ID_TIPO_DISCAPACIDAD.HasValue ? SelectPersona.ID_TIPO_DISCAPACIDAD.Value : 0);
                TextNip = SelectPersona.PERSONA_NIP.Where(w => w.ID_CENTRO == GlobalVar.gCentro).Any() ?
                    SelectPersona.PERSONA_NIP.Where(w => w.ID_CENTRO == GlobalVar.gCentro).FirstOrDefault().NIP : new Nullable<int>();
                TextCalle = SelectPersona.DOMICILIO_CALLE;
                TextNumInt = SelectPersona.DOMICILIO_NUM_INT;
                TextNumExt = SelectPersona.DOMICILIO_NUM_EXT;
                TextCodigoPostal = SelectPersona.DOMICILIO_CODIGO_POSTAL;
                FotoVisita = SelectPersona == null ?
                    new Imagenes().getImagenPerson() :
                        SelectPersona.PERSONA_BIOMETRICO.Where(w => w.ID_TIPO_BIOMETRICO == (short)enumTipoBiometrico.FOTO_FRENTE_REGISTRO && w.ID_FORMATO == (short)enumTipoFormato.FMTO_JPG).Any() ?
                            SelectPersona.PERSONA_BIOMETRICO.Where(w => w.ID_TIPO_BIOMETRICO == (short)enumTipoBiometrico.FOTO_FRENTE_REGISTRO && w.ID_FORMATO == (short)enumTipoFormato.FMTO_JPG).FirstOrDefault().BIOMETRICO :
                                new Imagenes().getImagenPerson();
                SelectPais = SelectPersona.ID_PAIS;
                ListEntidad = new ObservableCollection<ENTIDAD>(ListEntidadesAuxiliares.Where(w => w.ID_PAIS_NAC == SelectPersona.ID_PAIS).OrderBy(o => o.DESCR)); //(new cEntidad()).ObtenerTodos();
                ListEntidad.Insert(0, new ENTIDAD() { ID_ENTIDAD = -1, DESCR = "SELECCIONE" });
                //await TaskEx.Delay(100);
                SelectEntidad = SelectPersona.ID_ENTIDAD;
                ListMunicipio = new ObservableCollection<MUNICIPIO>(ListMunicipiosAuxiliares.Where(w => w.ID_ENTIDAD == SelectPersona.ID_ENTIDAD).OrderBy(o => o.MUNICIPIO1));//(new cMunicipio()).ObtenerTodos(string.Empty, value));
                ListMunicipio.Insert(0, new MUNICIPIO() { ID_MUNICIPIO = -1, MUNICIPIO1 = "SELECCIONE" });
                //await TaskEx.Delay(100);
                SelectMunicipio = SelectPersona.ID_MUNICIPIO;
                ListColonia = new ObservableCollection<COLONIA>(ListColoniasAuxiliares.Where(w => w.ID_ENTIDAD == SelectPersona.ID_ENTIDAD && w.ID_MUNICIPIO == SelectPersona.ID_MUNICIPIO).OrderBy(o => o.DESCR));//(new cColonia()).ObtenerTodos(string.Empty, value, SelectEntidad));
                ListColonia.Insert(0, new COLONIA() { ID_COLONIA = -1, DESCR = "SELECCIONE" });
                //await TaskEx.Delay(100);
                if (SelectPersona.ID_COLONIA.HasValue ? SelectPersona.ID_COLONIA.Value > 0 : false)
                    SelectColonia = SelectPersona.ID_COLONIA;
                SelectBitacoraAcceso = SalidaEnabled ? ListBitacoraAcceso.Where(w => w.ADUANA.ID_PERSONA == SelectPersona.ID_PERSONA).FirstOrDefault() : null;
                if (SelectBitacoraAcceso != null)
                {
                    SelectSituacion = SelectTipoPersona == ParametroPersonaExterna ?
                        //(SelectPersona.PERSONA_EXTERNO.Where(w => w.ID_CENTRO == GlobalVar.gCentro).FirstOrDefault().ESTATUS == "S" ?
                        (SelectPersona.PERSONA_EXTERNO.ESTATUS == "S" ?
                        ParametroVisitaAutorizada : ParametroVisitaCancelada) :
                            SelectTipoPersona == ParametroPersonaVisita ?
                            (SelectPersona.VISITANTE.ID_ESTATUS_VISITA.HasValue ? SelectPersona.VISITANTE.ID_ESTATUS_VISITA.Value : (short)-1) :
                                SelectTipoPersona == ParametroPersonaEmpleado ?
                                (SelectPersona.EMPLEADO.ESTATUS == "S" ? ParametroVisitaAutorizada : ParametroVisitaCancelada) :
                                    SelectTipoPersona == ParametroPersonaLegal ?
                                    (SelectPersona.ABOGADO.ID_ESTATUS_VISITA.HasValue ? SelectPersona.ABOGADO.ID_ESTATUS_VISITA.Value : (short)-1) :
                                        ParametroVisitaCancelada;
                    return;
                }

                #region VISITA
                if (SelectTipoPersona == ParametroPersonaVisita)
                {
                    hideAll = await GetDatosVisitaFamiliar(hideAll);
                    VisitaFamiliarVisible = hideAll ? Visibility.Visible : Visibility.Collapsed;
                    BanderaSelectAll = true;
                    SeleccionarTodosEnable = ListadoInternos.Count > 0;
                    SeleccionarTodoAcompanantesEnabled = ListAcompanantes != null ? ListAcompanantes.Count > 0 : false;
                    //SeleccionarTodoInternos = true; solo cuando encuentre solo un imputado
                }
                #endregion

                #region EXTERNA
                if (SelectTipoPersona == ParametroPersonaExterna)
                {
                    SetValidacionesExternos();
                    var ahora = Fechas.GetFechaDateServer;
                    TextFechaExterno = ahora.ToString("dd/MM/yyyy");
                    TextHoraExterno = ahora.ToString("HH:mm");
                    ExternosVisible = hideAll ? Visibility.Visible : Visibility.Collapsed;
                    //Modificacion de modelo, PENDIENTE
                    if (SelectPersona.PERSONA_EXTERNO != null)
                    {
                        TextObservacionExterno = SelectPersona.PERSONA_EXTERNO.OBSERV;
                        SelectSituacion = SelectPersona.PERSONA_EXTERNO.ESTATUS == "S" ? ParametroVisitaAutorizada : ParametroVisitaCancelada;
                    }
                    else
                    {
                        TextObservacionExterno = string.Empty;
                        SelectSituacion = ParametroVisitaCancelada;
                    }
                    //TextObservacionExterno = SelectPersona.PERSONA_EXTERNO.AsQueryable().Any(w => w.ID_CENTRO == GlobalVar.gCentro) ?
                    //  SelectPersona.PERSONA_EXTERNO.AsQueryable().FirstOrDefault().OBSERV : string.Empty;
                    //SelectSituacion = SelectPersona.PERSONA_EXTERNO.AsQueryable().Any(w => w.ID_CENTRO == GlobalVar.gCentro) ?
                    //    SelectPersona.PERSONA_EXTERNO.AsQueryable().FirstOrDefault().ESTATUS == "S" ?
                    //    EstatusAutorizado : EstatusCancelado : EstatusCancelado;
                    ////////////////////////////////////////////////////////
                }
                #endregion

                #region EMPLEADO
                if (SelectTipoPersona == ParametroPersonaEmpleado)
                {
                    LimpiarCamposEmpleados();
                    EmpleadosVisible = hideAll ? Visibility.Visible : Visibility.Collapsed;
                    SelectDeptoEmpleado = SelectPersona.EMPLEADO != null ? SelectPersona.EMPLEADO.ID_DEPARTAMENTO.HasValue ? SelectPersona.EMPLEADO.ID_DEPARTAMENTO.Value : (short)0 : (short)0;
                }
                #endregion

                #region ABOGADOS
                if (SelectTipoPersona == ParametroPersonaLegal)
                {
                    LimpiarCamposAbogados();
                    LockerEnabled = true;
                    ListCausasSeleccionadas = new ObservableCollection<CAUSA_PENAL>();
                    DatosAbogadosVisible = hideAll ? Visibility.Visible : Visibility.Collapsed;
                    if (SelectPersona.ABOGADO != null && SelectPersona.ABOGADO.ID_ABOGADO_TITULO == ParametroAbogadoTitular)
                    {
                        SetValidacionesAbogados();
                        HeaderTextDatosAbogados = "Datos del Abogado Titular";
                        //Application.Current.Dispatcher.Invoke((Action)(delegate { GetDatosPersonaSeleccionada(); }));
                        GetDatosAbogadoTitular();
                        BanderaSelectAll = true;
                        SeleccionarTodosEnable = ListIngresosAsignados.Count > 0;
                        GridAbogadoVisible = hideAll ? Visibility.Visible : Visibility.Collapsed;
                        InternosAsignadosVisible = hideAll ? Visibility.Visible : Visibility.Collapsed;
                    }
                    if (SelectPersona.ABOGADO != null && SelectPersona.ABOGADO.ID_ABOGADO_TITULO == ParametroAbogadoColaborador)
                    {
                        SetValidacionesColaboradores();
                        HeaderTextDatosAbogados = "Datos del Colaborador";
                        //Application.Current.Dispatcher.Invoke((Action)(delegate { GetDatosPersonaSeleccionada(); }));
                        GetDatosAbogadoTitular();
                        GetDatosAbogadoColaborador();
                        BanderaSelectAll = true;
                        SeleccionarTodosEnable = ListIngresosAsignados.Count > 0;
                        ColaboradorVisible = hideAll ? Visibility.Visible : Visibility.Collapsed;
                        InternosAsignadosVisible = hideAll ? Visibility.Visible : Visibility.Collapsed;
                    }
                    if (SelectPersona.ABOGADO != null && SelectPersona.ABOGADO.ID_ABOGADO_TITULO == ParametroAbogadoActuario)
                    {
                        SetValidacionesActuarios();
                        HeaderTextDatosAbogados = "Datos del Actuario";
                        //Application.Current.Dispatcher.Invoke((Action)(delegate { GetDatosPersonaSeleccionada(); }));
                        GetDatosAbogadoTitular();
                        GetDatosAbogadoActuario();
                        BanderaSelectAll = true;
                        GridAbogadoVisible = hideAll ? Visibility.Visible : Visibility.Collapsed;
                        JuzgadoActuarioVisible = hideAll ? Visibility.Visible : Visibility.Collapsed;
                    }
                }
                #endregion
            }
            catch (Exception ex)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al traer datos del visitante.", ex);
            }
        }

        private void HideAll()
        {
            DatosAbogadosVisible = Visibility.Collapsed;
            GridAbogadoVisible = Visibility.Collapsed;
            ColaboradorVisible = Visibility.Collapsed;
            JuzgadoActuarioVisible = Visibility.Collapsed;
            InternosAsignadosVisible = Visibility.Collapsed;
            VisitaFamiliarVisible = Visibility.Collapsed;
            ExternosVisible = Visibility.Collapsed;
            EmpleadosVisible = Visibility.Collapsed;
        }

        private async void CheckBoxSelectedOnGrid(object SelectedItem)
        {
            try
            {
                var checkBox = ((object[])(SelectedItem)).Count() > 1 ? ((CheckBox)(((object[])(SelectedItem))[1])) : ((CheckBox)(((object[])(SelectedItem))[0]));
                if (SelectTipoPersona == ParametroPersonaLegal)
                {
                    if ((((object[])(SelectedItem))[0]) is CheckBox ? BanderaSelectAll : false)
                    {
                        if (checkBox.Name == "CKB_ALL_INTERNOS_ABOGADOS" ? ListIngresosAsignados.Any() : false)
                            ListIngresosAsignados = new ObservableCollection<AbogadoIngresoAsignacion>(ListIngresosAsignados.Select(s => new AbogadoIngresoAsignacion
                            {
                                ABOGADO_INGRESO = s.ABOGADO_INGRESO,
                                ELIGE = checkBox.IsChecked.HasValue ? checkBox.IsChecked.Value : false
                            }));
                        else if (checkBox.Name == "CKB_ALL_INTERNOS_ACTUARIOS" ? ListInternosSeleccionadosPorNotificar.Any() : false)
                        {
                            await StaticSourcesViewModel.CargarDatosMetodoAsync(() =>
                            {
                                Application.Current.Dispatcher.Invoke((Action)(delegate
                                {
                                    ListInternosSeleccionadosPorNotificar = new ObservableCollection<ActuarioIngresoAsignacion>(ListInternosSeleccionadosPorNotificar.Select(s =>
                                        new ActuarioIngresoAsignacion
                                        {
                                            ACTUARIO_INGRESO = s.ACTUARIO_INGRESO,
                                            ELIGE = checkBox.IsChecked.HasValue ? checkBox.IsChecked.Value : false
                                        }));
                                }));
                            });
                        }
                    }
                    if ((((object[])(SelectedItem))[0]) is AbogadoIngresoAsignacion)
                    {
                        checkBox = ((CheckBox)(((object[])(SelectedItem))[1]));
                        var asigna = ((AbogadoIngresoAsignacion)(((object[])(SelectedItem))[0]));
                        asigna.ELIGE = checkBox.IsChecked.HasValue ? checkBox.IsChecked.Value : false;
                        if (checkBox.IsChecked.HasValue)
                        {
                            StaticSourcesViewModel.SourceChanged = true;
                            if (SeleccionarTodoAbogados ? !checkBox.IsChecked.Value : false)
                            {
                                BanderaSelectAll = false;
                                SeleccionarTodoAbogados = false;
                                BanderaSelectAll = true;

                            }
                            else
                            {
                                if (checkBox.IsChecked.Value)
                                {
                                    BanderaSelectAll = false;
                                    SeleccionarTodoAbogados = ListIngresosAsignados.Where(w => w.ELIGE).Count() == ListIngresosAsignados.Count;
                                    BanderaSelectAll = true;
                                }
                                else
                                {
                                    var causas = ListCausasSeleccionadas.Where(w => w.ID_CENTRO == asigna.ABOGADO_INGRESO.ID_CENTRO && w.ID_ANIO == asigna.ABOGADO_INGRESO.ID_ANIO &&
                                        w.ID_IMPUTADO == asigna.ABOGADO_INGRESO.ID_IMPUTADO && w.ID_INGRESO == asigna.ABOGADO_INGRESO.ID_INGRESO).ToList();
                                    if (causas.Count() > 0)
                                    {
                                        foreach (var item in causas)
                                        {
                                            ListCausasSeleccionadas.Remove(item);
                                        }
                                    }
                                    ListCausasAsignadas = new ObservableCollection<AbogadoCausaPenalAsignacion>(ListCausasAsignadas.Select(s => new AbogadoCausaPenalAsignacion
                                    {
                                        ABOGADO_CAUSA_PENAL = s.ABOGADO_CAUSA_PENAL,
                                        DESCR = s.DESCR,
                                        DESHABILITA = s.DESHABILITA,
                                        ELEGIDO = false,
                                        ID_ESTATUS_VISITA = s.ID_ESTATUS_VISITA
                                    }));
                                }
                            }
                        }
                    }
                    if ((((object[])(SelectedItem))[0]) is AbogadoCausaPenalAsignacion)
                    {
                        checkBox = ((CheckBox)(((object[])(SelectedItem))[1]));
                        var causa = ((AbogadoCausaPenalAsignacion)(((object[])(SelectedItem))[0]));
                        causa.ELEGIDO = checkBox.IsChecked.HasValue ? checkBox.IsChecked.Value : false;
                        if (causa.ELEGIDO)
                        {
                            ListCausasSeleccionadas.Add(causa.ABOGADO_CAUSA_PENAL.CAUSA_PENAL);
                            ListIngresosAsignados.Where(w => w.ABOGADO_INGRESO.ID_CENTRO == causa.ABOGADO_CAUSA_PENAL.CAUSA_PENAL.ID_CENTRO &&
                             w.ABOGADO_INGRESO.ID_ANIO == causa.ABOGADO_CAUSA_PENAL.CAUSA_PENAL.ID_ANIO &&
                              w.ABOGADO_INGRESO.ID_IMPUTADO == causa.ABOGADO_CAUSA_PENAL.CAUSA_PENAL.ID_IMPUTADO &&
                               w.ABOGADO_INGRESO.ID_INGRESO == causa.ABOGADO_CAUSA_PENAL.CAUSA_PENAL.ID_INGRESO).FirstOrDefault().ELIGE = true;
                            ListIngresosAsignados = new ObservableCollection<AbogadoIngresoAsignacion>(ListIngresosAsignados);
                        }
                        else
                        {
                            if (ListCausasSeleccionadas.Where(w => w.ID_CENTRO == causa.ABOGADO_CAUSA_PENAL.CAUSA_PENAL.ID_CENTRO &&
                                w.ID_ANIO == causa.ABOGADO_CAUSA_PENAL.CAUSA_PENAL.ID_ANIO &&
                                 w.ID_IMPUTADO == causa.ABOGADO_CAUSA_PENAL.CAUSA_PENAL.ID_IMPUTADO &&
                                  w.ID_INGRESO == causa.ABOGADO_CAUSA_PENAL.CAUSA_PENAL.ID_INGRESO &&
                                   w.ID_CAUSA_PENAL == causa.ABOGADO_CAUSA_PENAL.CAUSA_PENAL.ID_CAUSA_PENAL).Any())
                            {
                                ListCausasSeleccionadas.Remove(ListCausasSeleccionadas.Where(w => w.ID_CENTRO == causa.ABOGADO_CAUSA_PENAL.CAUSA_PENAL.ID_CENTRO &&
                                    w.ID_ANIO == causa.ABOGADO_CAUSA_PENAL.CAUSA_PENAL.ID_ANIO &&
                                     w.ID_IMPUTADO == causa.ABOGADO_CAUSA_PENAL.CAUSA_PENAL.ID_IMPUTADO &&
                                      w.ID_INGRESO == causa.ABOGADO_CAUSA_PENAL.CAUSA_PENAL.ID_INGRESO &&
                                       w.ID_CAUSA_PENAL == causa.ABOGADO_CAUSA_PENAL.CAUSA_PENAL.ID_CAUSA_PENAL).FirstOrDefault());
                            }
                        }
                    }
                    if ((((object[])(SelectedItem))[0]) is ActuarioIngresoAsignacion)
                    {
                        checkBox = ((CheckBox)(((object[])(SelectedItem))[1]));
                        var asigna = ((ActuarioIngresoAsignacion)(((object[])(SelectedItem))[0]));
                        asigna.ELIGE = checkBox.IsChecked.HasValue ? checkBox.IsChecked.Value : false;
                        if (checkBox.IsChecked.HasValue)
                        {
                            StaticSourcesViewModel.SourceChanged = true;
                            if (SeleccionarTodoActuarios ? !checkBox.IsChecked.Value : false)
                            {
                                BanderaSelectAll = false;
                                SeleccionarTodoActuarios = false;
                                BanderaSelectAll = true;
                            }
                            else
                                if (checkBox.IsChecked.Value)
                                {
                                    BanderaSelectAll = false;
                                    SeleccionarTodoActuarios = ListInternosSeleccionadosPorNotificar.Where(w => w.ELIGE).Count() == ListInternosSeleccionadosPorNotificar.Count;
                                    BanderaSelectAll = true;
                                }
                        }
                    }
                }
                if (SelectTipoPersona == ParametroPersonaVisita)
                {
                    if ((((object[])(SelectedItem))[0]) is CheckBox ? BanderaSelectAll : false)
                    {
                        if (checkBox.Name == "CKB_ALL_ACOMPANANTES" ? ListAcompanantes.Any() : false)
                            ListAcompanantes = new ObservableCollection<AcompananteAsignacion>(ListAcompanantes.Select(s => new AcompananteAsignacion
                            {
                                ACOMPANANTE = s.ACOMPANANTE,
                                ELEGIDO = checkBox.IsChecked.HasValue ? checkBox.IsChecked.Value : false
                            }));

                        else if (checkBox.Name == "CKB_ALL_INTERNOS_VISITA" ? ListadoInternos.Any() : false)
                            ListadoInternos = new ObservableCollection<VisitanteIngresoAsignacion>(ListadoInternos.Select(s => new VisitanteIngresoAsignacion
                            {
                                VISITANTE_INGRESO = s.VISITANTE_INGRESO,
                                ELEGIDO = checkBox.IsChecked.HasValue ? checkBox.IsChecked.Value : false
                            }));
                    }
                    if ((((object[])(SelectedItem))[0]) is AcompananteAsignacion)
                    {
                        checkBox = ((CheckBox)(((object[])(SelectedItem))[1]));
                        var acompanante = ((AcompananteAsignacion)(((object[])(SelectedItem))[0]));
                        acompanante.ELEGIDO = checkBox.IsChecked.HasValue ? checkBox.IsChecked.Value : false;
                        if (checkBox.IsChecked.HasValue)
                        {
                            StaticSourcesViewModel.SourceChanged = true;
                            if (SeleccionarTodoAcompanantes ? !checkBox.IsChecked.Value : false)
                            {
                                BanderaSelectAll = false;
                                SeleccionarTodoAcompanantes = false;
                                BanderaSelectAll = true;
                            }
                            else
                                if (checkBox.IsChecked.Value)
                                {
                                    BanderaSelectAll = false;
                                    SeleccionarTodoAcompanantes = ListAcompanantes.Where(w => w.ELEGIDO).Count() == ListAcompanantes.Count;
                                    BanderaSelectAll = true;
                                }
                        }
                    }
                    if ((((object[])(SelectedItem))[0]) is VisitanteIngresoAsignacion)
                    {
                        checkBox = ((CheckBox)(((object[])(SelectedItem))[1]));
                        var VisitanteIngreso = ((VisitanteIngresoAsignacion)(((object[])(SelectedItem))[0]));
                        VisitanteIngreso.ELEGIDO = checkBox.IsChecked.HasValue ? checkBox.IsChecked.Value : false;
                        foreach (var item in ((VisitanteIngresoAsignacion)(((object[])(SelectedItem))[0])).VISITANTE_INGRESO.ACOMPANANTE)
                        {
                            if (ListAcompanantes != null)
                            {
                                if (VisitanteIngreso.ELEGIDO)
                                    ListAcompanantes.Add(new AcompananteAsignacion { ACOMPANANTE = item, ELEGIDO = true });
                                else
                                    if (ListAcompanantes.Where(w => w.ACOMPANANTE.ID_ACOMPANANTE == item.ID_ACOMPANANTE).Any())
                                        ListAcompanantes.Remove(ListAcompanantes.Where(w => w.ACOMPANANTE.ID_ACOMPANANTE == item.ID_ACOMPANANTE).FirstOrDefault());
                            }
                        }
                        if (checkBox.IsChecked.HasValue)
                        {
                            StaticSourcesViewModel.SourceChanged = true;
                            if (SeleccionarTodoInternos ? !checkBox.IsChecked.Value : false)
                            {
                                BanderaSelectAll = false;
                                SeleccionarTodoInternos = false;
                                BanderaSelectAll = true;
                            }
                            else
                                if (checkBox.IsChecked.Value)
                                {
                                    BanderaSelectAll = false;
                                    SeleccionarTodoInternos = ListadoInternos.Where(w => w.ELEGIDO).Count() == ListadoInternos.Count;
                                    BanderaSelectAll = true;
                                }
                        }
                    }
                }
                StaticSourcesViewModel.SourceChanged = checkBox.IsChecked.HasValue;
            }
            catch (Exception ex)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al seleccionar", ex);
            }
        }

        private async void HeaderSort(Object obj)
        {
            try
            {
                await StaticSourcesViewModel.CargarDatosMetodoAsync(() =>
                {
                    Application.Current.Dispatcher.Invoke((Action)(async delegate
                    {
                        StaticSourcesViewModel.ShowMensajeProgreso("Cargando...");
                        if (obj != null ? obj.ToString() == "Tipo Visitante" : false)
                        {
                            if ((HeaderSortin % 2) == 0)
                            {
                                ListBitacoraAcceso = new ObservableCollection<BitacoraAduana>(ListBitacoraAcceso
                                    .OrderBy(o => o.ADUANA.ID_TIPO_PERSONA)
                                    .ThenBy(t => t.ADUANA.ID_TIPO_PERSONA == ParametroPersonaLegal ? t.ADUANA.PERSONA.ABOGADO.ID_ABOGADO_TITULO :
                                        t.ADUANA.ID_TIPO_PERSONA == ParametroPersonaVisita ? t.ADUANA.ADUANA_INGRESO.Any(w => w.INTIMA == "S") ? "A" : string.Empty : string.Empty));
                            }
                            else
                            {
                                ListBitacoraAcceso = new ObservableCollection<BitacoraAduana>(ListBitacoraAcceso
                                    .OrderByDescending(o => o.ADUANA.ID_TIPO_PERSONA)
                                    .ThenByDescending(t => t.ADUANA.ID_TIPO_PERSONA == ParametroPersonaLegal ? t.ADUANA.PERSONA.ABOGADO.ID_ABOGADO_TITULO :
                                        t.ADUANA.ID_TIPO_PERSONA == ParametroPersonaVisita ? t.ADUANA.ADUANA_INGRESO.Any(w => w.INTIMA == "S") ? "A" : string.Empty : string.Empty));
                            }
                        }
                        if (obj != null ? obj.ToString() == "Tipo visita" : false)
                        {
                            ListPersonas = new RangeEnabledObservableCollection<SSP.Servidor.PERSONA>();
                            //Modificacion de modelo, PENDIENTE
                            ListPersonas.InsertRange(await StaticSourcesViewModel.CargarDatosAsync<ObservableCollection<SSP.Servidor.PERSONA>>(() =>
                                new ObservableCollection<SSP.Servidor.PERSONA>(ListPersonasAuxiliar
                                    .OrderByDescending(o => HeaderSortin == 0 ? o.VISITANTE != null : HeaderSortin == 1 ? o.EMPLEADO != null : HeaderSortin == 2 ?
                                        //(o.PERSONA_EXTERNO != null ? o.PERSONA_EXTERNO.Count > 0 : o.PERSONA_EXTERNO != null) : HeaderSortin == 3 ? o.ABOGADO != null : o.ABOGADO != null)
                                        (o.PERSONA_EXTERNO != null) : HeaderSortin == 3 ? o.ABOGADO != null : o.ABOGADO != null)
                                    .ThenByDescending(o => HeaderSortin == 3 ? o.VISITANTE != null : HeaderSortin == 0 ? o.EMPLEADO != null : HeaderSortin == 1 ?
                                        //(o.PERSONA_EXTERNO != null ? o.PERSONA_EXTERNO.Count > 0 : o.PERSONA_EXTERNO != null) : HeaderSortin == 2 ? o.ABOGADO != null : o.ABOGADO != null)
                                        (o.PERSONA_EXTERNO != null) : HeaderSortin == 2 ? o.ABOGADO != null : o.ABOGADO != null)
                                    .ThenByDescending(o => HeaderSortin == 2 ? o.VISITANTE != null : HeaderSortin == 3 ? o.EMPLEADO != null : HeaderSortin == 0 ?
                                        //(o.PERSONA_EXTERNO != null ? o.PERSONA_EXTERNO.Count > 0 : o.PERSONA_EXTERNO != null) : HeaderSortin == 1 ? o.ABOGADO != null : o.ABOGADO != null)
                                        (o.PERSONA_EXTERNO != null) : HeaderSortin == 1 ? o.ABOGADO != null : o.ABOGADO != null)
                                    .ThenByDescending(o => HeaderSortin == 1 ? o.VISITANTE != null : HeaderSortin == 2 ? o.EMPLEADO != null : HeaderSortin == 3 ?
                                        //(o.PERSONA_EXTERNO != null ? o.PERSONA_EXTERNO.Count > 0 : o.PERSONA_EXTERNO != null) : HeaderSortin == 0 ? o.ABOGADO != null : o.ABOGADO != null))));
                                        (o.PERSONA_EXTERNO != null) : HeaderSortin == 0 ? o.ABOGADO != null : o.ABOGADO != null))));
                            ///////////////////////////////////////////////////////////////////
                        }
                        HeaderSortin = (short)(HeaderSortin == 3 ? 0 : HeaderSortin + 1);
                        StaticSourcesViewModel.CloseMensajeProgreso();
                    }));
                });
            }
            catch (Exception ex)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al ordenar la busqueda.", ex);
            }
        }

        private void ConfiguraPermisos()
        {
            try
            {
                var permisos = new cProcesoUsuario().Obtener(enumProcesos.RECEPCION_ADUANA.ToString(), StaticSourcesViewModel.UsuarioLogin.Username);
                foreach (var p in permisos)
                {
                    if (p.INSERTAR == 1)
                        PInsertar = true;
                    if (p.EDITAR == 1)
                        PEditar = true;
                    if (p.CONSULTAR == 1)
                        PConsultar = true;
                    if (p.IMPRIMIR == 1)
                        PImprimir = true;
                }
            }
            catch (Exception ex)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al configurar permisos en la pantalla", ex);
            }
        }

        #region HUELLAS DIGITALES

        private void ShowIdentification(object obj = null)
        {
            ShowPopUp = Visibility.Visible;
            ShowFingerPrint = Visibility.Hidden;
            var Initial442 = new Thread((Init) =>
            {
                try
                {
                    var nRet = 0;
                    CLSFPCaptureDllWrapper.CLS_Initialize();
                    CLSFPCaptureDllWrapper.CLS_SetLanguage(CLSFPCaptureDllWrapper.CLS_FP_CAPTURE_RESOURCE.ENGLISH);
                    nRet = CLSFPCaptureDllWrapper.CLS_CaptureFP(CLSFPCaptureDllWrapper.CLS_FP_CAPTURE_TYPE.IDFLATS);
                    if (nRet == 0)
                    {
                        ScannerMessage = "Procesando...";
                        ShowFingerPrint = Visibility.Visible;
                        ShowLine = Visibility.Visible;
                        ShowOk = Visibility.Hidden;
                        Thread.Sleep(300);
                        HuellasCapturadas = new List<PlantillaBiometrico>();
                        var SaveFingerPrints = new Thread((saver) =>
                        {
                            try
                            {
                                #region [Huellas]
                                for (short i = 1; i <= 10; i++)
                                {
                                    var pBuffer = IntPtr.Zero;
                                    var nBufferLength = 0;
                                    var nNFIQ = 0;
                                    CLSFPCaptureDllWrapper.CLS_GetImage(CLSFPCaptureDllWrapper.CLS_FP_CAPTURE_IMPRESSION_TYPE.PLAIN, (CLSFPCaptureDllWrapper.CLS_FP_CAPTURE_FINGER)i, CLSFPCaptureDllWrapper.IMG_TYPE.BMP, ref pBuffer, ref nBufferLength);
                                    var bufferBMP = new byte[nBufferLength];
                                    if (pBuffer != IntPtr.Zero)
                                        Marshal.Copy(pBuffer, bufferBMP, 0, nBufferLength);
                                    CLSFPCaptureDllWrapper.CLS_GetImage(CLSFPCaptureDllWrapper.CLS_FP_CAPTURE_IMPRESSION_TYPE.PLAIN, (CLSFPCaptureDllWrapper.CLS_FP_CAPTURE_FINGER)i, CLSFPCaptureDllWrapper.IMG_TYPE.WSQ, ref pBuffer, ref nBufferLength);
                                    var bufferWSQ = new byte[nBufferLength];
                                    if (pBuffer != IntPtr.Zero)
                                        Marshal.Copy(pBuffer, bufferWSQ, 0, nBufferLength);
                                    CLSFPCaptureDllWrapper.CLS_GetImageNFIQ(((CLSFPCaptureDllWrapper.CLS_FP_CAPTURE_FINGER)i), ref nNFIQ, CLSFPCaptureDllWrapper.CLS_FP_CAPTURE_IMPRESSION_TYPE.PLAIN);
                                    Fmd FMD = null;
                                    if (bufferBMP.Length != 0)
                                    {
                                        GuardaHuella = CreateBitmapSourceFromBitmap(new MemoryStream(bufferBMP));
                                        FMD = ExtractFmdfromBmp(new Bitmap(new MemoryStream(bufferBMP)).Clone(new Rectangle(0, 0, 357, 392), System.Drawing.Imaging.PixelFormat.Format8bppIndexed)).Data;
                                    }
                                    Thread.Sleep(200);
                                    switch ((CLSFPCaptureDllWrapper.CLS_FP_CAPTURE_FINGER)i)
                                    {
                                        #region [Pulgar Derecho]
                                        case CLSFPCaptureDllWrapper.CLS_FP_CAPTURE_FINGER.RIGHT_THUMB:
                                            HuellasCapturadas.Add(new PlantillaBiometrico { ID_TIPO_BIOMETRICO = enumTipoBiometrico.PULGAR_DERECHO, ID_TIPO_FORMATO = enumTipoFormato.FMTO_DP, BIOMETRICO = FMD != null ? FMD.Bytes : bufferBMP, CALIDAD = (short)nNFIQ });
                                            HuellasCapturadas.Add(new PlantillaBiometrico { ID_TIPO_BIOMETRICO = enumTipoBiometrico.PULGAR_DERECHO, ID_TIPO_FORMATO = enumTipoFormato.FMTO_WSQ, BIOMETRICO = bufferWSQ });
                                            break;
                                        #endregion
                                        #region [Indice Derecho]
                                        case CLSFPCaptureDllWrapper.CLS_FP_CAPTURE_FINGER.RIGHT_INDEX:
                                            HuellasCapturadas.Add(new PlantillaBiometrico { ID_TIPO_BIOMETRICO = enumTipoBiometrico.INDICE_DERECHO, ID_TIPO_FORMATO = enumTipoFormato.FMTO_DP, BIOMETRICO = FMD != null ? FMD.Bytes : bufferBMP, CALIDAD = (short)nNFIQ });
                                            HuellasCapturadas.Add(new PlantillaBiometrico { ID_TIPO_BIOMETRICO = enumTipoBiometrico.INDICE_DERECHO, ID_TIPO_FORMATO = enumTipoFormato.FMTO_WSQ, BIOMETRICO = bufferWSQ });
                                            break;
                                        #endregion
                                        #region [Medio Derecho]
                                        case CLSFPCaptureDllWrapper.CLS_FP_CAPTURE_FINGER.RIGHT_MIDDLE:
                                            HuellasCapturadas.Add(new PlantillaBiometrico { ID_TIPO_BIOMETRICO = enumTipoBiometrico.MEDIO_DERECHO, ID_TIPO_FORMATO = enumTipoFormato.FMTO_DP, BIOMETRICO = FMD != null ? FMD.Bytes : bufferBMP, CALIDAD = (short)nNFIQ });
                                            HuellasCapturadas.Add(new PlantillaBiometrico { ID_TIPO_BIOMETRICO = enumTipoBiometrico.MEDIO_DERECHO, ID_TIPO_FORMATO = enumTipoFormato.FMTO_WSQ, BIOMETRICO = bufferWSQ });
                                            break;
                                        #endregion
                                        #region [Anular Derecho]
                                        case CLSFPCaptureDllWrapper.CLS_FP_CAPTURE_FINGER.RIGHT_RING:
                                            HuellasCapturadas.Add(new PlantillaBiometrico { ID_TIPO_BIOMETRICO = enumTipoBiometrico.ANULAR_DERECHO, ID_TIPO_FORMATO = enumTipoFormato.FMTO_DP, BIOMETRICO = FMD != null ? FMD.Bytes : bufferBMP, CALIDAD = (short)nNFIQ });
                                            HuellasCapturadas.Add(new PlantillaBiometrico { ID_TIPO_BIOMETRICO = enumTipoBiometrico.ANULAR_DERECHO, ID_TIPO_FORMATO = enumTipoFormato.FMTO_WSQ, BIOMETRICO = bufferWSQ });
                                            break;
                                        #endregion
                                        #region [Meñique Derecho]
                                        case CLSFPCaptureDllWrapper.CLS_FP_CAPTURE_FINGER.RIGHT_LITTLE:
                                            HuellasCapturadas.Add(new PlantillaBiometrico { ID_TIPO_BIOMETRICO = enumTipoBiometrico.MENIQUE_DERECHO, ID_TIPO_FORMATO = enumTipoFormato.FMTO_DP, BIOMETRICO = FMD != null ? FMD.Bytes : bufferBMP, CALIDAD = (short)nNFIQ });
                                            HuellasCapturadas.Add(new PlantillaBiometrico { ID_TIPO_BIOMETRICO = enumTipoBiometrico.MENIQUE_DERECHO, ID_TIPO_FORMATO = enumTipoFormato.FMTO_WSQ, BIOMETRICO = bufferWSQ });
                                            break;
                                        #endregion
                                        #region [Pulgar Izquierdo]
                                        case CLSFPCaptureDllWrapper.CLS_FP_CAPTURE_FINGER.LEFT_THUMB:
                                            HuellasCapturadas.Add(new PlantillaBiometrico { ID_TIPO_BIOMETRICO = enumTipoBiometrico.PULGAR_IZQUIERDO, ID_TIPO_FORMATO = enumTipoFormato.FMTO_DP, BIOMETRICO = FMD != null ? FMD.Bytes : bufferBMP, CALIDAD = (short)nNFIQ });
                                            HuellasCapturadas.Add(new PlantillaBiometrico { ID_TIPO_BIOMETRICO = enumTipoBiometrico.PULGAR_IZQUIERDO, ID_TIPO_FORMATO = enumTipoFormato.FMTO_WSQ, BIOMETRICO = bufferWSQ });
                                            break;
                                        #endregion
                                        #region [Indice Izquierdo]
                                        case CLSFPCaptureDllWrapper.CLS_FP_CAPTURE_FINGER.LEFT_INDEX:
                                            HuellasCapturadas.Add(new PlantillaBiometrico { ID_TIPO_BIOMETRICO = enumTipoBiometrico.INDICE_IZQUIERDO, ID_TIPO_FORMATO = enumTipoFormato.FMTO_DP, BIOMETRICO = FMD != null ? FMD.Bytes : bufferBMP, CALIDAD = (short)nNFIQ });
                                            HuellasCapturadas.Add(new PlantillaBiometrico { ID_TIPO_BIOMETRICO = enumTipoBiometrico.INDICE_IZQUIERDO, ID_TIPO_FORMATO = enumTipoFormato.FMTO_WSQ, BIOMETRICO = bufferWSQ });
                                            break;
                                        #endregion
                                        #region [Medio Izquierdo]
                                        case CLSFPCaptureDllWrapper.CLS_FP_CAPTURE_FINGER.LEFT_MIDDLE:
                                            HuellasCapturadas.Add(new PlantillaBiometrico { ID_TIPO_BIOMETRICO = enumTipoBiometrico.MEDIO_IZQUIERDO, ID_TIPO_FORMATO = enumTipoFormato.FMTO_DP, BIOMETRICO = FMD != null ? FMD.Bytes : bufferBMP, CALIDAD = (short)nNFIQ });
                                            HuellasCapturadas.Add(new PlantillaBiometrico { ID_TIPO_BIOMETRICO = enumTipoBiometrico.MEDIO_IZQUIERDO, ID_TIPO_FORMATO = enumTipoFormato.FMTO_WSQ, BIOMETRICO = bufferWSQ });
                                            break;
                                        #endregion
                                        #region [Anular Izquierdo]
                                        case CLSFPCaptureDllWrapper.CLS_FP_CAPTURE_FINGER.LEFT_RING:
                                            HuellasCapturadas.Add(new PlantillaBiometrico { ID_TIPO_BIOMETRICO = enumTipoBiometrico.ANULAR_IZQUIERDO, ID_TIPO_FORMATO = enumTipoFormato.FMTO_DP, BIOMETRICO = FMD != null ? FMD.Bytes : bufferBMP, CALIDAD = (short)nNFIQ });
                                            HuellasCapturadas.Add(new PlantillaBiometrico { ID_TIPO_BIOMETRICO = enumTipoBiometrico.ANULAR_IZQUIERDO, ID_TIPO_FORMATO = enumTipoFormato.FMTO_WSQ, BIOMETRICO = bufferWSQ });
                                            break;
                                        #endregion
                                        #region [Meñique Izquierdo]
                                        case CLSFPCaptureDllWrapper.CLS_FP_CAPTURE_FINGER.LEFT_LITTLE:
                                            HuellasCapturadas.Add(new PlantillaBiometrico { ID_TIPO_BIOMETRICO = enumTipoBiometrico.MENIQUE_IZQUIERDO, ID_TIPO_FORMATO = enumTipoFormato.FMTO_DP, BIOMETRICO = FMD != null ? FMD.Bytes : bufferBMP, CALIDAD = (short)nNFIQ });
                                            HuellasCapturadas.Add(new PlantillaBiometrico { ID_TIPO_BIOMETRICO = enumTipoBiometrico.MENIQUE_IZQUIERDO, ID_TIPO_FORMATO = enumTipoFormato.FMTO_WSQ, BIOMETRICO = bufferWSQ });
                                            break;
                                        #endregion
                                        default:
                                            break;
                                    }
                                }
                                #endregion
                                ScannerMessage = "Huellas Capturadas Correctamente";
                            }
                            catch (Exception ex)
                            {
                                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al procesar huellas.", ex);
                            }
                        });
                        SaveFingerPrints.Start();
                        SaveFingerPrints.Join();
                        ShowLine = Visibility.Hidden;
                        Thread.Sleep(1500);
                    }
                    ShowPopUp = Visibility.Hidden;
                    CLSFPCaptureDllWrapper.CLS_Terminate();
                    PopUpsViewModels.ClosePopUp(PopUpsViewModels.TipoPopUp.HUELLAS);
                }
                catch
                {
                    Application.Current.Dispatcher.Invoke((Action)(delegate
                    {
                        ShowPopUp = Visibility.Hidden;
                        (new Dialogos()).ConfirmacionDialogo("Error", "Revise que el escanner este bien configurado.");
                        PopUpsViewModels.ClosePopUp(PopUpsViewModels.TipoPopUp.HUELLAS);
                    }));
                }
            });

            Initial442.Start();
        }
        private async void OnBuscarPorHuella(string obj = "")
        {
            try
            {
                await Task.Factory.StartNew(() => PopUpsViewModels.ShowPopUp(this, PopUpsViewModels.TipoPopUp.OSCURECER_FONDO));
                await TaskEx.Delay(400);
                var nRet = -1;
                BanderaHuella = true;
                if (ParametroRequiereGuardarHuellas)
                    try
                    {
                        nRet = CLSFPCaptureDllWrapper.CLS_Initialize();
                    }
                    catch
                    {
                        BanderaHuella = false;
                    }
                else
                    BanderaHuella = false;

                WindowBusqueda = new BusquedaHuella();
                var dataContext = new BusquedaHuellaViewModel(enumTipoPersona.PERSONA_TODOS, nRet == 0, ParametroRequiereGuardarHuellas);
                WindowBusqueda.DataContext = dataContext;
                WindowBusqueda.dgHuella.Columns.Insert(WindowBusqueda.dgHuella.Columns.Count, new DataGridTextColumn()
                {
                    Binding = new System.Windows.Data.Binding("Persona")
                    {
                        Converter = new GetTipoPersona()
                    },
                    Header = "TIPO VISITA"
                });
                dataContext.CabeceraBusqueda = string.Empty;
                dataContext.CabeceraFoto = string.Empty;
                if (nRet != 0 ? ((ControlPenales.Clases.FingerPrintScanner)(WindowBusqueda.DataContext)).Readers.Count == 0 : false)
                {
                    PopUpsViewModels.ClosePopUp(PopUpsViewModels.TipoPopUp.OSCURECER_FONDO);
                    PopUpsViewModels.ClosePopUp(PopUpsViewModels.TipoPopUp.HUELLAS);
                    StaticSourcesViewModel.Mensaje("ADVERTENCIA", "ASEGURESE DE CONECTAR SU LECTOR DE HUELLA DIGITAL", StaticSourcesViewModel.enumTipoMensaje.MENSAJE_INFORMACION, 5);
                    return;
                }
                WindowBusqueda.Owner = PopUpsViewModels.MainWindow;
                WindowBusqueda.KeyDown -= HuellaKeyDown;
                WindowBusqueda.KeyDown += HuellaKeyDown;
                WindowBusqueda.Closed -= HuellaClose;
                WindowBusqueda.Closed += HuellaClose;
                WindowBusqueda.ShowDialog();
            }
            catch (Exception ex)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al buscar por huella.", ex);
            }
        }
        private void HuellaKeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            try
            {
                if (e.Key == System.Windows.Input.Key.Escape) WindowBusqueda.Close();
            }
            catch (Exception ex)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al buscar por huella.", ex);
            }
        }
        private async void HuellaClose(object s, EventArgs e)
        {
            try
            {
                HuellasCapturadas = ((BusquedaHuellaViewModel)WindowBusqueda.DataContext).HuellasCapturadas;
                if (BanderaHuella == true) CLSFPCaptureDllWrapper.CLS_Terminate();
                CodigoEnabled = NombreReadOnly = false;
                PopUpsViewModels.ClosePopUp(PopUpsViewModels.TipoPopUp.OSCURECER_FONDO);
                var huella = ((BusquedaHuellaViewModel)WindowBusqueda.DataContext);
                if (!huella.IsSucceed) return;
                if (huella.ScannerMessage == "HUELLA NO ENCONTRADA")
                {
                    var persn = SelectPersona;
                    ListPersonasAuxiliar = new RangeEnabledObservableCollection<SSP.Servidor.PERSONA>();
                    ListPersonas = new RangeEnabledObservableCollection<SSP.Servidor.PERSONA>();
                    PopUpsViewModels.ShowPopUp(this, PopUpsViewModels.TipoPopUp.BUSCAR_PERSONAS_EXISTENTES);
                    SelectPersonaAuxiliar = persn;
                    return;
                }
                if (huella.SelectRegistro != null ? huella.SelectRegistro.Persona == null : null == null) return;
                await SeleccionarPersona(huella.SelectRegistro.Persona);
                StaticSourcesViewModel.SourceChanged = false;
            }
            catch (Exception ex)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al buscar por huella.", ex);
            }
        }

        #endregion
    }
    public class BitacoraAduana
    {
        public ADUANA ADUANA { get; set; }
        public CAMA CAMA { get; set; }
        public bool PARAMETRO { get; set; }
    }
    public enum enumTipoVisitaPorCentro
    {
        APELLIDO = 1,
        EDIFICIO = 2
    }
    public enum enumAreas
    {
        MEDICA_PA = 45,
        MEDICA_PB = 46,
        DENTAL_MATUTINO = 8,
        DENTAL_VESPERTINO = 13,
        SALA_ABOGADOS = 110,
        VISITA_LEGAL = 7,
        EXCARCELACION_TRASLADOS = 111,
        LOCUTORIOS = 85
    }
}