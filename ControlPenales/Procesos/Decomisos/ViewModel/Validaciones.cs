using ControlPenales;

using System;
using System.Threading.Tasks;
using System.Windows;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Windows.Input;
using MahApps.Metro.Controls;
using MahApps.Metro.Controls.Dialogs;

namespace ControlPenales
{
    partial class RegistroDecomisoViewModel
    {
        #region Decomiso
        private void SetValidacionesDecomiso()
        {
            try
            {
                base.ClearRules();
                base.AddRule(() => ResumenD, () => !string.IsNullOrEmpty(ResumenD), "RESUMEN ES REQUERIDO!");
                base.AddRule(() => SelectedGpoTactico, () => SelectedGpoTactico != -1, "GRUPO TÁCTICO ES REQUERIDO!");
                base.AddRule(() => SelectedTurno, () => SelectedTurno != -1, "TURNO ES REQUERIDO!");

                if (SelectedArea == -1)
                    base.AddRule(() => Celda, () => !string.IsNullOrEmpty(Celda), "CELDA ES REQUERIDA!");
                
                int i = 0;
                if (FechaEventoD == null)
                    base.AddRule(() => FechaEventoD, () => FechaEventoD.HasValue ? FechaEventoD.Value <= Fechas.GetFechaDateServer : false, "LA FECHA NO PUEDE SER MAYOR AL DÍA ACTUAL");
                else
                    i++;

                if (FechaInformeD == null)
                    base.AddRule(() => FechaInformeD, () => FechaInformeD.HasValue ? FechaInformeD.Value <= Fechas.GetFechaDateServer : false, "LA FECHA NO PUEDE SER MAYOR AL DÍA ACTUAL");
                else
                    i++;
                if (i == 2)
                {
                    base.AddRule(() => FechaInformeD, () =>  FechaInformeD.Value >= FechaEventoD.Value, "LA FECHA DE INFORME DEBE SER MAYOR O IGUAL A LA FECHA DEL EVENTO");
                }

                #region reporte
                base.AddRule(() => IOficioSeguridad, () => !string.IsNullOrEmpty(IOficioSeguridad), "OFICIO DE SEGURIDAD ES REQUERIDO!");
                base.AddRule(() => IJefeTurno, () => !string.IsNullOrEmpty(IJefeTurno), "JEFE DE TURNO ES REQUERIDO!");
                base.AddRule(() => IComandante, () => !string.IsNullOrEmpty(IComandante), "COMANDANTE ES REQUERIDO!");
                base.AddRule(() => IOficioComandancia1, () => !string.IsNullOrEmpty(IOficioComandancia1), "OFICIO DE COMANDANCIA 1 ES REQUERIDO!");
                base.AddRule(() => IOficioComandancia2, () => !string.IsNullOrEmpty(IOficioComandancia2), "OFICIO DE COMANDANCIA 2 ES REQUERIDO!");
                #endregion

                OnPropertyChanged("FechaInformeD");
                OnPropertyChanged("FechaEventoD");
                OnPropertyChanged("FolioD");
                OnPropertyChanged("SelectedGpoTactico");
                OnPropertyChanged("SelectedTurno");
                OnPropertyChanged("Celda");
                
                OnPropertyChanged("IOficioSeguridad");
                OnPropertyChanged("IJefeTurno");
                OnPropertyChanged("IComandante");
                OnPropertyChanged("IOficioComandancia1");
                OnPropertyChanged("IOficioComandancia2");

                
            }
            catch (Exception ex)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al establecer validaciones en decomiso", ex);
            }
        }

        private void SetValidacionesObjetos(int op)
        {
            base.ClearRules();
                if (op == 1)
                {
                    base.AddRule(() => OCantidad, () => OCantidad.HasValue, "CANTIDAD ES REQUERIDA!");
                    base.AddRule(() => ODescripcion, () => !string.IsNullOrEmpty(ODescripcion), "DESCRIPCIÓN ES REQUERIDA!");
                    OnPropertyChanged("OCantidad");
                    OnPropertyChanged("ODescripcion");
                }
                else
                    if (op == 2)
                    {
                        base.AddRule(() => OSerie, () => !string.IsNullOrEmpty(OSerie), "SERIE ES REQUERIDA!");
                        base.AddRule(() => ODescripcion, () => !string.IsNullOrEmpty(ODescripcion), "DESCRIPCIÓN ES REQUERIDA!");
                        OnPropertyChanged("OSerie");
                        OnPropertyChanged("ODescripcion");
                    }
                    else
                        if (op == 3)
                        {
                            base.AddRule(() => OTipoDroga, () => OTipoDroga != -1, "TIPO DE DROGA ES REQUERIDA!");
                            base.AddRule(() => OCantidad, () => OCantidad.HasValue, "CANTIDAD ES REQUERIDA!");
                            base.AddRule(() => OUnidadMedida, () => OUnidadMedida != 0, "UNIDAD DE MEDIDA ES REQUERIDA!");
                            base.AddRule(() => ODosis, () => ODosis.HasValue, "DOSIS ES REQUERIDA!");
                            base.AddRule(() => OEnvoltorios, () => OEnvoltorios.HasValue, "ENVOLTORIOS ES REQUERIDO!");
                            OnPropertyChanged("OTipoDroga");
                            OnPropertyChanged("OCantidad");
                            OnPropertyChanged("OUnidadMedida");
                            OnPropertyChanged("ODosis");
                            OnPropertyChanged("OEnvoltorios");
                        }
                        else
                            if (op == 4)
                            {
                                base.AddRule(() => OTelefono, () => OTelefono != null ? OTelefono.Length >= 14 : false, "TELÉFONO ES REQUERIDO Y DEBE CONTENER 10 DÍGITOS!");
                                base.AddRule(() => OIMEI, () => !string.IsNullOrEmpty(OIMEI), "IMEI ES REQUERIDO!");
                                base.AddRule(() => OSIMSerie, () => !string.IsNullOrEmpty(OSIMSerie), "SIM SERIE ES REQUERIDO!");
                                base.AddRule(() => OCapacidad, () => !string.IsNullOrEmpty(OCapacidad), "CAPACIDAD ES REQUERIDA!");
                                base.AddRule(() => ODescripcion, () => !string.IsNullOrEmpty(ODescripcion), "DESCRIPCIÓN ES REQUERIDA!");
                                OnPropertyChanged("OTelefono");
                                OnPropertyChanged("OIMEI");
                                OnPropertyChanged("OSIMSerie");
                                OnPropertyChanged("OCapacidad");
                                OnPropertyChanged("ODescripcion");
                            }
                            else
                                if (op == 5)
                                {
                                    base.AddRule(() => OSerie, () => !string.IsNullOrEmpty(OSerie), "SERIE ES REQUERIDA!");
                                    base.AddRule(() => OCapacidad, () => !string.IsNullOrEmpty(OCapacidad), "CAPACIDAD ES REQUERIDA!");
                                    base.AddRule(() => ODescripcion, () => !string.IsNullOrEmpty(ODescripcion), "DESCRIPCIÓN ES REQUERIDA!");
                                    OnPropertyChanged("OSerie");
                                    OnPropertyChanged("OCapacidad");
                                    OnPropertyChanged("ODescripcion");
                                }
                                else
                                    if (op == 6)
                                    {
                                        base.AddRule(() => OCompania, () => OCompania != -1, "COMPAÑIA ES REQUERIDA!");
                                        base.AddRule(() => OTelefono, () => OTelefono != null ? OTelefono.Length >= 14 : false, "TELÉFONO ES REQUERIDO Y DEBE CONTENER 10 DÍGITOS!");
                                        base.AddRule(() => OSIMSerie, () => !string.IsNullOrEmpty(OSIMSerie), "SIM SERIE ES REQUERIDO!");
                                        base.AddRule(() => ODescripcion, () => !string.IsNullOrEmpty(ODescripcion), "DESCRIPCIÓN ES REQUERIDA!");
                                        OnPropertyChanged("OCompania");
                                        OnPropertyChanged("OTelefono");
                                        OnPropertyChanged("OSIMSerie");
                                        OnPropertyChanged("ODescripcion");
                                    }
                                    else
                                        if (op == 7)
                                        {
                                            base.AddRule(() => OCompania, () => OCompania != -1, "COMPAÑIA ES REQUERIDA!");
                                            base.AddRule(() => OSerie, () => !string.IsNullOrEmpty(OSerie), "SERIE ES REQUERIDA!");
                                            base.AddRule(() => OCantidad, () => OCantidad.HasValue, "CANTIDAD ES REQUERIDA!");
                                            base.AddRule(() => ODescripcion, () => !string.IsNullOrEmpty(ODescripcion), "DESCRIPCIÓN ES REQUERIDA!");
                                            OnPropertyChanged("OCompania");
                                            OnPropertyChanged("OSerie");
                                            OnPropertyChanged("OCantidad");
                                            OnPropertyChanged("ODescripcion");
                                        }
                                        else
                                            if (op == 8)
                                            {
                                                base.AddRule(() => OCantidad, () => OCantidad.HasValue, "CANTIDAD ES REQUERIDA!");
                                                base.AddRule(() => OSerie, () => !string.IsNullOrEmpty(OSerie), "SERIE ES REQUERIDA!");
                                                base.AddRule(() => ODescripcion, () => !string.IsNullOrEmpty(ODescripcion), "DESCRIPCIÓN ES REQUERIDA!");
                                                OnPropertyChanged("OCantidad");
                                                OnPropertyChanged("OSerie");
                                                OnPropertyChanged("ODescripcion");
                                            }
        }

        private bool ValidarGuardarDecomiso() 
        {
            if (string.IsNullOrEmpty(ResumenD))
            {
                new Dialogos().ConfirmacionDialogo("Validación", "El resumen debe de tener un mínimo de 100 caracteres.");
                return false;
            }
            else
            if (ResumenD.Length < 100)
            {
                new Dialogos().ConfirmacionDialogo("Validación", "El resumen debe de tener un minimo de 100 caracteres.");
                return false;
            }
            else
            if (LstOficialesACargo != null)
            {
                if (LstOficialesACargo.Count == 0)
                {
                    new Dialogos().ConfirmacionDialogo("Validación", "Debe seleccionar a un oficial a cargo.");
                    return false;
                }
                else
                {
                    //INTERNOS
                    if (LstInternoInvolucrado != null)
                    {
                        if (LstInternoInvolucrado.Count > 0)
                            return true;
                    }
                    //VISITANTE
                    if (LstVisitaInvolucrada != null)
                    {
                        if (LstVisitaInvolucrada.Count > 0)
                            return true;
                    }
                    //EMPLEADO
                    if (LstEmpleadoInvolucrado != null)
                    {
                        if (LstEmpleadoInvolucrado.Count > 0)
                            return true;
                    }
                    //VISITA EXTERNA
                    if (LstProveedoresInvolucrados != null)
                    {
                        if (LstProveedoresInvolucrados.Count > 0)
                            return true;
                    }
                    new Dialogos().ConfirmacionDialogo("Validación","Debe seleccionar un imputado, visitante, empleado o visita externa involucrados en el decomiso.");
                    return false;
                }
            }
            else
            {
                new Dialogos().ConfirmacionDialogo("Validación", "Debe seleccionar a un oficial a cargo.");
                return false;
            }
        }

        private bool ValidarResumen() {
            if (string.IsNullOrEmpty(ResumenD))
            {
                new Dialogos().ConfirmacionDialogo("Validación", "Favor de capturar resumen.");
                return false;
            }
            return true;
        }
        #endregion

        #region Impresion Decomiso
        //private void ValidacionImpresionDocumento()
        //{
        //    base.ClearRules();
        //    base.AddRule(() => IOficioSeguridad, () => !string.IsNullOrEmpty(IOficioSeguridad), "OFICIO DE SEGURIDAD ES REQUERIDO!");
        //    base.AddRule(() => IJefeTurno , () => !string.IsNullOrEmpty(IJefeTurno), "JEFE DE TURNO ES REQUERIDO!");
        //    base.AddRule(() => IComandante, () => !string.IsNullOrEmpty(IComandante), "COMANDANTE ES REQUERIDO!");
        //    base.AddRule(() => IOficioComandancia1, () => !string.IsNullOrEmpty(IOficioComandancia1), "OFICIO DE COMANDANCIA 1 ES REQUERIDO!");
        //    base.AddRule(() => IOficioComandancia2, () => !string.IsNullOrEmpty(IOficioComandancia2), "OFICIO DE COMANDANCIA 2 ES REQUERIDO!");
        //    OnPropertyChanged("IOficioSeguridad");
        //    OnPropertyChanged("IJefeTurno");
        //    OnPropertyChanged("IComandante");
        //    OnPropertyChanged("IOficioComandancia1");
        //    OnPropertyChanged("IOficioComandancia2");
        //}
        #endregion
    }
}
