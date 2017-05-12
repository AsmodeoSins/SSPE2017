using System;
using System.Collections.Generic;
using System.Linq;
using SSP.Servidor;
using System.Windows.Controls;
using System.Threading.Tasks;
using System.Collections.ObjectModel;
using SSP.Controlador.Catalogo.Justicia;
using System.Windows;
using ControlPenales.BiometricoServiceReference;

namespace ControlPenales
{
    partial class EstudioPersonalidadViewModel : ValidationViewModelBase
    {
        private eProcesoVentanasEstudioPersonalidad ventana_origen;

        public EstudioPersonalidadViewModel(eProcesoVentanasEstudioPersonalidad _origen)
        {
            ventana_origen = _origen;
            if (_origen == eProcesoVentanasEstudioPersonalidad.CIERRE_ESTUDIOS)
                ((System.Windows.Controls.ContentControl)PopUpsViewModels.MainWindow.FindName("contentControl")).Content = new CierreEstudiosPersonalidadView();
        }

        private void LoadEstudioPersonalidad(EstudioPersonalidadView Window = null)
        {
            ConfiguraPermisos();
        }

        #region Permisos

        private void PermisosCierreEstudiosPersonalidad()
        {
            try
            {
                var permisos = new cProcesoUsuario().ObtenerTodos(StaticSourcesViewModel.UsuarioLogin.Username, null, GlobalVariables.gCentro);
                if (permisos.Any())
                {
                    var PermisosCierreEstudios = permisos.FirstOrDefault(x => x.PROCESO != null && x.PROCESO.VENTANA.Contains(enumProcesos.CIERRE_ESTUDIOS_PERSONALIDAD.ToString()));
                    if (PermisosCierreEstudios != null)
                    {
                        PInsertar = PermisosCierreEstudios.INSERTAR == (short)eSINOProcesos.SI ? true : false;
                        PEditar = PermisosCierreEstudios.EDITAR == (short)eSINOProcesos.SI ? true : false;
                        PConsultar = PermisosCierreEstudios.CONSULTAR == (short)eSINOProcesos.SI ? true : false;
                        PImprimir = MenuReporteEnabled = PermisosCierreEstudios.IMPRIMIR == (short)eSINOProcesos.SI ? true : false;
                    };
                };
            }
            catch (Exception exc)
            {
                throw;
            }
        }
        private void ConfiguraPermisos()
        {
            try
            {
                IdEstudioPersonalidadPdre = null;
                SelectedEstudioPersonalidadDetalle = null;

                ListEstudiosPersonalidad = new ObservableCollection<PERSONALIDAD>();
                ListEstudiosPersonalidadDetalle = new ObservableCollection<PERSONALIDAD_DETALLE>();
                LstCandidatos = new ObservableCollection<INGRESO>();
                LstEstudiosConfirmados = new ObservableCollection<PERSONALIDAD>();

                var permisos = new cProcesoUsuario().ObtenerTodos(StaticSourcesViewModel.UsuarioLogin.Username, null, GlobalVariables.gCentro);
                if (permisos.Any())
                {
                    string EnumeradorProgramacion = "ESTUDIO_PERSONALIDAD";
                    string EnumeradorCierre = "CIERRE_PERSONALIDAD";
                    var PermisosProgramacionEstudios = permisos.FirstOrDefault(x => x.PROCESO != null && x.PROCESO.VENTANA.Contains(EnumeradorProgramacion));
                    var PermisosCierreEstudios = permisos.FirstOrDefault(x => x.PROCESO != null && x.PROCESO.VENTANA.Contains(EnumeradorCierre));
                    if (PermisosProgramacionEstudios != null)
                    {
                        PInsertar = PermisosProgramacionEstudios.INSERTAR == (short)eSINOProcesos.SI ? true : false;
                        PEditar = PermisosProgramacionEstudios.EDITAR == (short)eSINOProcesos.SI ? true : false;
                        PConsultar = PermisosProgramacionEstudios.CONSULTAR == (short)eSINOProcesos.SI ? true : false;
                        PImprimir = MenuReporteEnabled = PermisosProgramacionEstudios.IMPRIMIR == (short)eSINOProcesos.SI ? true : false;
                    };

                    if (PermisosCierreEstudios != null)
                    {
                        PInsertar = PermisosCierreEstudios.INSERTAR == (short)eSINOProcesos.SI ? true : false;
                        PEditar = PermisosCierreEstudios.EDITAR == (short)eSINOProcesos.SI ? true : false;
                        PConsultar = PermisosCierreEstudios.CONSULTAR == (short)eSINOProcesos.SI ? true : false;
                        PImprimir = MenuReporteEnabled = PermisosCierreEstudios.IMPRIMIR == (short)eSINOProcesos.SI ? true : false;
                    };
                };

                MenuGuardarEnabled = true;
                var EstudiosProgramadosHoy = new cEstudioPersonalidad().ObtenerDatosEstudiosProgramados(GlobalVar.gCentro);

                if (EstudiosProgramadosHoy != null && EstudiosProgramadosHoy.Any())
                {
                    if (LstEstudiosConfirmados == null)
                        LstEstudiosConfirmados = new ObservableCollection<PERSONALIDAD>();

                    foreach (var item in EstudiosProgramadosHoy)
                    {
                        if (item.INICIO_FEC.HasValue && item.TERMINO_FEC.HasValue && item.ID_SITUACION == 5)
                        {
                            LstEstudiosConfirmados.Add(new PERSONALIDAD
                            {
                                ID_ANIO = item.ID_ANIO,
                                ID_AREA = item.ID_AREA,
                                ID_CENTRO = item.ID_CENTRO,
                                ID_ESTUDIO = item.ID_ESTUDIO,
                                ID_IMPUTADO = item.ID_IMPUTADO,
                                SOLICITADO = item.SOLICITADO,
                                ID_INGRESO = item.ID_INGRESO,
                                ID_MOTIVO = item.ID_MOTIVO,
                                ID_SITUACION = item.ID_SITUACION,
                                INICIO_FEC = item.INICIO_FEC,
                                NUM_OFICIO = item.NUM_OFICIO,
                                NUM_OFICIO1 = item.NUM_OFICIO1,
                                NUM_OFICIO2 = item.PERSONALIDAD_DETALLE != null ? item.PERSONALIDAD_DETALLE.Any() ? item.PERSONALIDAD_DETALLE.All(x => x.ID_ESTATUS == (short)eEstatusSituacionPersonalidad.TERMINADO || x.ID_ESTATUS == (short)eEstatusSituacionPersonalidad.CANCELADO) ? "S" : "N" : "N" : "N",
                                PLAZO_DIAS = item.PLAZO_DIAS,
                                SOLICITUD_FEC = item.SOLICITUD_FEC,
                                PROG_NOMBRE = item.PROG_NOMBRE,
                                RESULT_ESTUDIO = item.RESULT_ESTUDIO,
                                TERMINO_FEC = item.TERMINO_FEC,
                                INGRESO = item.INGRESO
                            });
                            continue;
                        }

                        if (LstCandidatos.Any(z => z.ID_IMPUTADO == item.ID_IMPUTADO))//Este imputado ya existe en la lista
                            continue;

                        else
                            LstCandidatos.Add(item.INGRESO);
                    };
                };
            }
            catch (Exception ex)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al configurar permisos en la pantalla", ex);
            }
        }

        #endregion

        private void AgregarIncidente(INGRESO Entity)
        {
            var Incidente = new INCIDENTE()
            {
                REGISTRO_FEC = FecIncidencia,
                ID_INCIDENTE_TIPO = SelectedIncidenteTipo.ID_INCIDENTE_TIPO,
                INCIDENTE_TIPO = SelectedIncidenteTipo,
                MOTIVO = Motivo,
                ID_ANIO = Entity.ID_ANIO,
                ID_CENTRO = Entity.ID_CENTRO,
                ID_IMPUTADO = Entity.ID_IMPUTADO,
                ID_INGRESO = Entity.ID_INGRESO
            };

            var _UltimoEstudioPersonalidad = new cEstudioPersonalidad().GetData(x => x.ID_INGRESO == SelectIngreso.ID_INGRESO && x.ID_IMPUTADO == SelectIngreso.ID_IMPUTADO && x.ID_CENTRO == SelectIngreso.ID_CENTRO && x.ID_ANIO == SelectIngreso.ID_ANIO).OrderByDescending(x => x.ID_ESTUDIO).FirstOrDefault();

            if (_UltimoEstudioPersonalidad != null)
            {
                if (new cEstudioPersonalidad().CancelaEstudio(Incidente, (short)enumMensajeTipo.CANCELACION_ESTUDIO_PERSONALIDAD, _UltimoEstudioPersonalidad.ID_ESTUDIO))
                {
                    new Dialogos().ConfirmacionDialogo("Exito", "Se ha cancelado el estudio con exito");
                    ConfiguraPermisos();
                }
                else
                    new Dialogos().ConfirmacionDialogo("Error", "Surgió un error al cancelar el estudio de personalidad");
            }
            else
                new Dialogos().ConfirmacionDialogo("Validación", "Seleccione un estudio de personalidad");
        }

        private async void clickSwitch(Object obj)
        {
            switch (obj.ToString())
            {
                case "cancelar_estudio_personalidad":
                    if (SelectIngreso != null)
                    {
                        var _EstudioParaCancelar = LstCandidatos.FirstOrDefault(x => x.ID_INGRESO == SelectIngreso.ID_INGRESO && x.ID_IMPUTADO == SelectIngreso.ID_IMPUTADO && x.ID_ANIO == SelectIngreso.ID_ANIO);
                        if (_EstudioParaCancelar != null)
                        {
                            TituloPopUp = "Agregar Incidencia";
                            FecIncidencia = Fechas.GetFechaDateServer;
                            InicializaListasCancelacionEstudios();
                            Motivo = string.Empty;
                            IdIncidenteTipo = -1;
                            SetValidacionesIncidente();
                            PopUpsViewModels.ShowPopUp(this, PopUpsViewModels.TipoPopUp.AGREGAR_INCIDENTE);
                        }
                        else
                            new Dialogos().ConfirmacionDialogo("Validación", "Seleccione un estudio para continuar");
                    }
                    break;

                case "cancelar_incidente":
                    //base.ClearRules();
                    PopUpsViewModels.ClosePopUp(PopUpsViewModels.TipoPopUp.AGREGAR_INCIDENTE);
                    break;

                case "agregar_incidente":
                    if (!base.HasErrors)
                    {
                        AgregarIncidente(SelectIngreso);
                        base.ClearRules();
                        PopUpsViewModels.ClosePopUp(PopUpsViewModels.TipoPopUp.AGREGAR_INCIDENTE);
                    }
                    else
                    {
                        Application.Current.Dispatcher.Invoke((Action)(delegate
                        {
                            (new Dialogos()).ConfirmacionDialogo("Advertencia!", base.Error);
                        }));
                    }
                    break;

                #region Cierre de los estudios y las brigadas
                case "cancelar_ampliar_descripcion":
                    TextAmpliarDescripcion = string.Empty;
                    PopUpsViewModels.ClosePopUp(PopUpsViewModels.TipoPopUp.CAPTURA_OFICIO_PERSONALIDAD);
                    break;
                case "guardar_ampliar_descripcion":
                    if (!base.HasErrors)
                    {
                        if (lstTemporalEstudiosCerrar != null)
                            if (lstTemporalEstudiosCerrar.Any())
                                foreach (var item in lstTemporalEstudiosCerrar)
                                    item.NUM_OFICIO1 = TextAmpliarDescripcion;
                    }
                    else
                    {
                        Application.Current.Dispatcher.Invoke((Action)(delegate
                        {
                            (new Dialogos()).ConfirmacionDialogo("Advertencia!", base.Error);
                        }));
                    }

                    if (new cEstudioPersonalidad().InsertarEstudiosCerrados(lstTemporalEstudiosCerrar))
                    {
                        PopUpsViewModels.ClosePopUp(PopUpsViewModels.TipoPopUp.CAPTURA_OFICIO_PERSONALIDAD);

                        if (await new Dialogos().ConfirmarEliminar("Validación", "Se han cerrado los estudios con éxito,  ¿Desea visualizar el formato de remisión?") != 1)
                        {
                            DateTime _f1, _f2;
                            if (FechaInicioBusquedaEstudiosT.HasValue)
                                _f1 = FechaInicioBusquedaEstudiosT.Value;
                            else
                                _f1 = Fechas.GetFechaDateServer;

                            if (FechaFinBusquedaEstudiosT.HasValue)
                                _f2 = FechaFinBusquedaEstudiosT.Value;
                            else
                                _f2 = Fechas.GetFechaDateServer;

                            _f1 = new DateTime(_f1.Year, _f1.Month, _f1.Day);
                            _f2 = new DateTime(_f2.Year, _f2.Month, _f2.Day);

                            LstEstudiosCerrdaos = new ObservableCollection<PERSONALIDAD>();

                            //LstEstudiosCerrdaos = new ObservableCollection<PERSONALIDAD>(new cEstudioPersonalidad().BuscarEstudiosTPersonalidadTerminados(_f1, _f2, NoOficioBusquedaEstudiosT, GlobalVar.gCentro));
                            var _dato = new cEstudioPersonalidad().BuscarEstudiosTPersonalidadTerminados(_f1, _f2, NoOficioBusquedaEstudiosT, GlobalVar.gCentro);
                            if (_dato != null && _dato.Any())
                            {
                                foreach (var item in _dato)
                                {
                                    item.RESULT_ESTUDIO = string.Empty;
                                    LstEstudiosCerrdaos.Add(item);
                                }
                            }
                            break;
                        }

                        else
                            MuestraFormatoCierreEstudiosPersonalidad(lstTemporalEstudiosCerrar.ToList());

                        lstTemporalEstudiosCerrar = new ObservableCollection<PERSONALIDAD>();
                        LstEstudiosCerrdaos.Clear();

                        DateTime f1, f2;
                        if (FechaInicioBusquedaEstudiosT.HasValue)
                            f1 = FechaInicioBusquedaEstudiosT.Value;
                        else
                            f1 = Fechas.GetFechaDateServer;

                        if (FechaFinBusquedaEstudiosT.HasValue)
                            f2 = FechaFinBusquedaEstudiosT.Value;
                        else
                            f2 = Fechas.GetFechaDateServer;

                        f1 = new DateTime(f1.Year, f1.Month, f1.Day);
                        f2 = new DateTime(f2.Year, f2.Month, f2.Day);

                        //LstEstudiosCerrdaos = new ObservableCollection<PERSONALIDAD>(new cEstudioPersonalidad().BuscarEstudiosTPersonalidadTerminados(f1, f2, NoOficioBusquedaEstudiosT, GlobalVar.gCentro));

                        var _dato2 = new cEstudioPersonalidad().BuscarEstudiosTPersonalidadTerminados(f1, f2, NoOficioBusquedaEstudiosT, GlobalVar.gCentro);
                        if (_dato2 != null && _dato2.Any())
                        {
                            foreach (var item in _dato2)
                            {
                                item.RESULT_ESTUDIO = string.Empty;
                                LstEstudiosCerrdaos.Add(item);
                            }
                        }
                    }

                    break;

                case "cerrar_estudios_brigadas":
                    if (SelectedEstudioCerrado != null)
                    {
                        lstTemporalEstudiosCerrar = new ObservableCollection<PERSONALIDAD>(LstEstudiosCerrdaos.Where(x => x.PROG_NOMBRE.Trim() == SelectedEstudioCerrado.PROG_NOMBRE && x.NUM_OFICIO.Trim() == SelectedEstudioCerrado.NUM_OFICIO /*&& x.SOLICITUD_FEC == SelectedEstudioCerrado.SOLICITUD_FEC && x.ID_SITUACION == 3*/));

                        if (lstTemporalEstudiosCerrar.Any())
                        {
                            if (lstTemporalEstudiosCerrar.Any(x => string.IsNullOrEmpty(x.RESULT_ESTUDIO)))
                            {
                                new Dialogos().ConfirmacionDialogo("Validación", "Debe especificar si el dictamen es favorable o aplazado a cada uno de los estudios pertenecientes al oficio " + lstTemporalEstudiosCerrar.FirstOrDefault().NUM_OFICIO);
                                return;
                            }

                            if (await new Dialogos().ConfirmarEliminar("Validación", string.Format("Está por cerrar los estudios correspondientes al oficio {0}, ¿Desea continuar?", lstTemporalEstudiosCerrar.FirstOrDefault().NUM_OFICIO)) != 1)
                                break;

                            if (lstTemporalEstudiosCerrar.Any(x => x.INGRESO.CAUSA_PENAL.FirstOrDefault(y => y.ID_ESTATUS_CP == 1 && y.CP_FUERO == "C") != null))
                            {
                                PopUpsViewModels.ShowPopUp(this, PopUpsViewModels.TipoPopUp.CAPTURA_OFICIO_PERSONALIDAD);
                                TituloHeaderExpandirDescripcion = "Numero de Oficio de remisión de estudios de personalidad";
                                TextAmpliarDescripcion = string.Empty;
                                ValidacionesNoumOficio2();
                            }

                            else
                            {
                                foreach (var item in lstTemporalEstudiosCerrar)
                                    item.RESULT_ESTUDIO = "S";

                                if (new cEstudioPersonalidad().InsertarEstudiosCerrados(lstTemporalEstudiosCerrar))
                                {
                                    //if (_EstudiosDentroBrigada.Any(x => x.INGRESO.CAUSA_PENAL.FirstOrDefault(y => y.ID_ESTATUS_CP == 1 && y.CP_FUERO == "C") != null))//Existe al menos un comun dentro de los que se acaban de guardar, muestra el oficio de remision
                                    //{
                                    //    if (await new Dialogos().ConfirmarEliminar("Validación", "Se han cerrado los estudios con éxito,  ¿Desea visualizar el formato de remisión?") != 1)
                                    //        break;

                                    //    MuestraFormatoCierreEstudiosPersonalidad(_EstudiosDentroBrigada.ToList());
                                    //}
                                    //else
                                    new Dialogos().ConfirmacionDialogo("EXITO!", "Se han cerrado los estudios con éxito");


                                    lstTemporalEstudiosCerrar = new ObservableCollection<PERSONALIDAD>();
                                    LstEstudiosCerrdaos.Clear();

                                    DateTime f1, f2;
                                    if (FechaInicioBusquedaEstudiosT.HasValue)
                                        f1 = FechaInicioBusquedaEstudiosT.Value;
                                    else
                                        f1 = Fechas.GetFechaDateServer;

                                    if (FechaFinBusquedaEstudiosT.HasValue)
                                        f2 = FechaFinBusquedaEstudiosT.Value;
                                    else
                                        f2 = Fechas.GetFechaDateServer;

                                    f1 = new DateTime(f1.Year, f1.Month, f1.Day);
                                    f2 = new DateTime(f2.Year, f2.Month, f2.Day);

                                    //LstEstudiosCerrdaos = new ObservableCollection<PERSONALIDAD>(new cEstudioPersonalidad().BuscarEstudiosTPersonalidadTerminados(f1, f2, NoOficioBusquedaEstudiosT, GlobalVar.gCentro));
                                    var _dato = new cEstudioPersonalidad().BuscarEstudiosTPersonalidadTerminados(f1, f2, NoOficioBusquedaEstudiosT, GlobalVar.gCentro);
                                    if (_dato != null && _dato.Any())
                                    {
                                        foreach (var item in _dato)
                                        {
                                            item.RESULT_ESTUDIO = string.Empty;
                                            LstEstudiosCerrdaos.Add(item);
                                        }
                                    }
                                }
                            }
                        }
                    }
                    break;

                #endregion
                #region Seleccion de candidatos

                case "agregar_candidato":
                    if (SelectIngreso != null)
                    {
                        if (LstEstudiosConfirmados == null)
                            LstEstudiosConfirmados = new ObservableCollection<PERSONALIDAD>();

                        var _EstudioPorHacer = new cEstudioPersonalidad().GetData(x => x.ID_INGRESO == SelectIngreso.ID_INGRESO && x.ID_IMPUTADO == SelectIngreso.ID_IMPUTADO && x.ID_CENTRO == SelectIngreso.ID_CENTRO && x.ID_ANIO == SelectIngreso.ID_ANIO).OrderByDescending(x => x.ID_ESTUDIO).FirstOrDefault();//validacion con respecto al ultimo estudio de personalidad programado al imputado
                        if (_EstudioPorHacer != null)
                            if (_EstudioPorHacer.INGRESO != null)
                            {
                                INGRESO _datoLista = LstCandidatos.FirstOrDefault(x => x.ID_INGRESO == _EstudioPorHacer.ID_INGRESO && x.ID_IMPUTADO == _EstudioPorHacer.ID_IMPUTADO);
                                if (_datoLista != null)
                                    if (LstCandidatos.Remove(_datoLista))
                                    {
                                        LstEstudiosConfirmados.Add(new PERSONALIDAD
                                            {
                                                ID_ANIO = _EstudioPorHacer.ID_ANIO,
                                                ID_CENTRO = _EstudioPorHacer.ID_CENTRO,
                                                ID_ESTUDIO = _EstudioPorHacer.ID_ESTUDIO,
                                                ID_IMPUTADO = _EstudioPorHacer.ID_IMPUTADO,
                                                ID_INGRESO = _EstudioPorHacer.ID_INGRESO,
                                                ID_MOTIVO = _EstudioPorHacer.ID_MOTIVO,
                                                ID_SITUACION = _EstudioPorHacer.ID_SITUACION,
                                                INICIO_FEC = _EstudioPorHacer.INICIO_FEC,
                                                NUM_OFICIO = _EstudioPorHacer.NUM_OFICIO,
                                                PROG_NOMBRE = _EstudioPorHacer.PROG_NOMBRE,
                                                RESULT_ESTUDIO = _EstudioPorHacer.RESULT_ESTUDIO,
                                                SOLICITADO = _EstudioPorHacer.SOLICITADO,
                                                SOLICITUD_FEC = _EstudioPorHacer.SOLICITUD_FEC,
                                                TERMINO_FEC = _EstudioPorHacer.TERMINO_FEC,
                                                INGRESO = _EstudioPorHacer.INGRESO
                                            });

                                        //limpia la slistas subsecuentes
                                        IdEstudioPersonalidadPdre = null;
                                        ListEstudiosPersonalidad = new ObservableCollection<PERSONALIDAD>();
                                        ListEstudiosPersonalidadDetalle = new ObservableCollection<PERSONALIDAD_DETALLE>();
                                        SelectedEstudioPersonalidadDetalle = null;
                                        SelectIngreso = null;
                                    };
                            };
                    };
                    break;

                case "remove_candidato":
                    if (SelectedEstudioConfirmado != null)
                    {
                        PERSONALIDAD _temporal = SelectedEstudioConfirmado;

                        if (LstCandidatos == null)
                            LstCandidatos = new ObservableCollection<INGRESO>();

                        if (SelectedEstudioConfirmado.INGRESO != null)
                            if (LstEstudiosConfirmados.Remove(SelectedEstudioConfirmado))
                                LstCandidatos.Add(_temporal.INGRESO);

                        IdEstudioPersonalidadPdre = null;
                        ListEstudiosPersonalidad = new ObservableCollection<PERSONALIDAD>();
                        ListEstudiosPersonalidadDetalle = new ObservableCollection<PERSONALIDAD_DETALLE>();
                        SelectedEstudioPersonalidadDetalle = null;
                        SelectedEstudioConfirmado = null;
                        _temporal = null;
                    };
                    break;
                #endregion
                case "ficha_menu":
                    ///no action
                    break;

                case "ayuda_menu":
                    //no action
                    break;


                #region Personalidad Principal
                case "guardar_menu_personalidad":
                    if (LstEstudiosConfirmados == null)
                    {
                        new Dialogos().ConfirmacionDialogo("VALIDACIÓN", "ES NECESARIO INGRESE AL MENOS UN IMPUTADO EN LA LISTA DE ESTUDIOS PROGRAMADOS");
                        return;
                    }

                    if (LstEstudiosConfirmados.Count == 0)
                    {
                        new Dialogos().ConfirmacionDialogo("VALIDACIÓN", "ES NECESARIO INGRESE AL MENOS UN IMPUTADO EN LA LISTA DE ESTUDIOS PROGRAMADOS");
                        return;
                    }

                    else
                    {
                        if (LstEstudiosConfirmados.Any(x => x.INICIO_FEC == null))
                        {
                            new Dialogos().ConfirmacionDialogo("VALIDACIÓN", "INGRESE LA FECHA DE INICIO A CADA ESTUDIO PARA PROGRAMARLO");
                            return;
                        }
                        else
                        //if (LstEstudiosConfirmados.Any(x => x.ID_AREA == null))
                        //{
                        //    new Dialogos().ConfirmacionDialogo("VALIDACIÓN", "INGRESE EL ÁREA A CADA ESTUDIO PARA PROGRAMARLO");
                        //    return;
                        //}
                        {
                            if (LstEstudiosConfirmados.Any(x => x.TERMINO_FEC == null))
                            {
                                new Dialogos().ConfirmacionDialogo("VALIDACIÓN", "INGRESE LA FECHA DE FIN A CADA ESTUDIO PARA PROGRAMARLO");
                                return;
                            }

                            if (LstEstudiosConfirmados.Any(x => x.RESULT_ESTUDIO == "True"))
                            {
                                ObservableCollection<PERSONALIDAD> lstEstudiosConclui = new ObservableCollection<PERSONALIDAD>();
                                var agrupar = LstEstudiosConfirmados.Where(x => x.RESULT_ESTUDIO == "True").GroupBy(c => c.PROG_NOMBRE);
                                foreach (var item in agrupar)
                                {
                                    string llave = item.Key;
                                    if (string.IsNullOrEmpty(llave))
                                        return;

                                    var detalleBrigada = new cEstudioPersonalidad().GetData(x => x.PROG_NOMBRE.Trim() == llave.Trim() && x.ID_SITUACION != 4 && x.ID_SITUACION != 3);
                                    if (detalleBrigada.Any())
                                    {
                                        foreach (var item2 in detalleBrigada)
                                        {
                                            var _detalleEstudio = new cPersonalidadDetalle().GetData(x => x.ID_ESTUDIO == item2.ID_ESTUDIO && x.ID_INGRESO == item2.ID_INGRESO && x.ID_IMPUTADO == item2.ID_IMPUTADO && x.ID_CENTRO == item2.ID_CENTRO && x.ID_ANIO == item2.ID_ANIO);
                                            if (_detalleEstudio.Any(x => x.ID_ESTATUS != 3 && x.ID_ESTATUS != 4))//ALGUNO DEL DESARROLLO NO HA SIDO TERMINADO O CANCELADO, SIGUE ASIGNADO
                                            {
                                                new Dialogos().ConfirmacionDialogo("Validación", "ES NECESARIO TERMINAR EL DESARROLLO DE LOS ESTUDIOS PARA PODER CERRAR");
                                                return;
                                            }

                                            else
                                                lstEstudiosConclui.Add(new PERSONALIDAD
                                                {
                                                    ID_ANIO = item2.ID_ANIO,
                                                    ID_AREA = item2.ID_AREA,
                                                    ID_CENTRO = item2.ID_CENTRO,
                                                    ID_ESTUDIO = item2.ID_ESTUDIO,
                                                    ID_IMPUTADO = item2.ID_IMPUTADO,
                                                    SOLICITADO = item2.SOLICITADO,
                                                    ID_INGRESO = item2.ID_INGRESO,
                                                    ID_MOTIVO = item2.ID_MOTIVO,
                                                    ID_SITUACION = item2.ID_SITUACION,
                                                    INICIO_FEC = item2.INICIO_FEC,
                                                    NUM_OFICIO = item2.NUM_OFICIO,
                                                    NUM_OFICIO1 = item2.NUM_OFICIO1,
                                                    NUM_OFICIO2 = item2.NUM_OFICIO2,
                                                    PLAZO_DIAS = item2.PLAZO_DIAS,
                                                    SOLICITUD_FEC = item2.SOLICITUD_FEC,
                                                    PROG_NOMBRE = item2.PROG_NOMBRE,
                                                    RESULT_ESTUDIO = "True",
                                                    TERMINO_FEC = item2.TERMINO_FEC,
                                                    INGRESO = item2.INGRESO
                                                });
                                        };

                                        if (new cEstudioPersonalidad().CierraEstudiosPersonalidad(lstEstudiosConclui.ToList()))
                                        {
                                            new Dialogos().ConfirmacionDialogo("ÉXITO", "LOS ESTUDIOS DE PERSONALIDAD HAN SIDO PROCESADOS CON ÉXITO");
                                            ConfiguraPermisos();
                                        }
                                        else
                                            new Dialogos().ConfirmacionDialogo("ERROR", "SURGIÓ UN ERROR AL PROGRAMAR LOS ESTUDIOS DE PERSONALIDAD");
                                    }
                                };

                                return;
                            }

                            if (new cEstudioPersonalidad().ProgramaEstudiosPersonalidad(LstEstudiosConfirmados.ToList(), (short)enumMensajeTipo.DEFINIDA_FECHA_INICIO_PERSONALIDAD))
                            {
                                new Dialogos().ConfirmacionDialogo("ÉXITO", "LOS ESTUDIOS DE PERSONALIDAD HAN SIDO PROCESADOS CON ÉXITO");
                                SelectedEstudioConfirmado = null;
                                IdEstudioPersonalidadPdre = null;
                                SelectedEstudioPersonalidadDetalle = null;
                                ListEstudiosPersonalidad = new ObservableCollection<PERSONALIDAD>();
                                ListEstudiosPersonalidadDetalle = new ObservableCollection<PERSONALIDAD_DETALLE>();
                            }
                            else
                                new Dialogos().ConfirmacionDialogo("ERROR", "SURGIÓ UN ERROR AL PROGRAMAR LOS ESTUDIOS DE PERSONALIDAD");
                        }
                    }
                    break;

                case "buscar_menu_personalidad":
                    BuscarEstudiosP();
                    break;

                case "limpiar_menu_personalidad":
                    NoOficioBusqueda = string.Empty;
                    FechaInicioBusqueda = FechaFinBusqueda = Fechas.GetFechaDateServer;
                    ((System.Windows.Controls.ContentControl)PopUpsViewModels.MainWindow.FindName("contentControl")).Content = new EstudioPersonalidadView();
                    ((System.Windows.Controls.ContentControl)PopUpsViewModels.MainWindow.FindName("contentControl")).DataContext = new ControlPenales.EstudioPersonalidadViewModel(ventana_origen);
                    break;


                #endregion
                case "mostrar_ficha_identificacion":
                    Reporte();
                    break;
                case "guardar_menu":
                    if (ValidaCierreEstudios())
                    {
                        if (new cEstudioPersonalidad().InsertarEstudiosCerrados(LstEstudiosCerrdaos))
                        {

                            if (await new Dialogos().ConfirmarEliminar("Validación", "Se han cerrado los estudios con éxito,  ¿Desea visualizar el (los) formato (s) de remisión?") != 1)
                                break;

                            MuestraFormatoCierreEstudiosPersonalidad(LstEstudiosCerrdaos.ToList());

                            //re consulta de nuevo una vez que guardo los terminados
                            LstEstudiosCerrdaos.Clear();

                            DateTime f1, f2;
                            if (FechaInicioBusquedaEstudiosT.HasValue)
                                f1 = FechaInicioBusquedaEstudiosT.Value;
                            else
                                f1 = Fechas.GetFechaDateServer;

                            if (FechaFinBusquedaEstudiosT.HasValue)
                                f2 = FechaFinBusquedaEstudiosT.Value;
                            else
                                f2 = Fechas.GetFechaDateServer;

                            f1 = new DateTime(f1.Year, f1.Month, f1.Day);
                            f2 = new DateTime(f2.Year, f2.Month, f2.Day);


                            //LstEstudiosCerrdaos = new ObservableCollection<PERSONALIDAD>(new cEstudioPersonalidad().BuscarEstudiosTPersonalidadTerminados(f1, f2, NoOficioBusquedaEstudiosT, GlobalVar.gCentro));
                            var _dato = new cEstudioPersonalidad().BuscarEstudiosTPersonalidadTerminados(f1, f2, NoOficioBusquedaEstudiosT, GlobalVar.gCentro);
                            if (_dato != null && _dato.Any())
                            {
                                foreach (var item in _dato)
                                {
                                    item.RESULT_ESTUDIO = string.Empty;
                                    LstEstudiosCerrdaos.Add(item);
                                }
                            }
                        }
                        else
                            new Dialogos().ConfirmacionDialogo("Error", "Surgió un error al actualizar loe estudios de personalidad");
                    }
                    else
                        new Dialogos().ConfirmacionDialogo("Validación", "Asegurese de marcar los estudios a cerrar");

                    break;

                case "buscar_menu":
                    PopUpsViewModels.ShowPopUp(this, PopUpsViewModels.TipoPopUp.BUSQUEDA);
                    break;

                case "nueva_busqueda":
                    ListExpediente = new RangeEnabledObservableCollection<IMPUTADO>();
                    SelectExpediente = new IMPUTADO();
                    EmptyExpedienteVisible = true;
                    ApellidoPaternoBuscar = ApellidoMaternoBuscar = NombreBuscar = string.Empty;
                    FolioBuscar = AnioBuscar = null;
                    ImagenImputado = ImagenIngreso = new Imagenes().getImagenPerson();
                    break;

                case "cancel_guardar_estudio_personalidad_detalle":
                    TipoEstudioSelectedDetalle = EstatusEstudioSelectedDetalle = -1;
                    FechaSolicitudEstudioSelectedDetalle = FechaInicioSolicitudEstudioSelectedDetalle = FechaFinSolicitudEstudioSelectedDetalle = null;
                    ResultadoSolicitudEstudioSelectedDetalle = string.Empty;
                    DiasBonificadosSolicitudEstudioSelectedDetalle = null;
                    PopUpsViewModels.ClosePopUp(PopUpsViewModels.TipoPopUp.AGREGAR_ESTUDIO_PERSONALIDAD_DETALLE);
                    break;

                case "guardar_estudio_personalidad_detalle":
                    if (!base.HasErrors)
                        GuardarDetalleEstudioPersonalidad();

                    else
                    {
                        new Dialogos().ConfirmacionDialogo("Validación", "Capture todos los campos requeridos para continuar");
                        break;
                    }

                    break;

                case "insertar_desarrollo_estudio":
                    if (SelectedEstudioPersonalidad == null)
                    {
                        new Dialogos().ConfirmacionDialogo("Validación", "Es necesario seleccionar un estudio de personalidad para continuar");
                        break;
                    }

                    else
                    {
                        if (!PInsertar)
                        {
                            (new Dialogos()).ConfirmacionDialogo("Validación", "No cuenta con suficientes privilegios para realizar esta acción.");
                            return;
                        }

                        FechaSolicitudEstudioSelectedDetalle = FechaInicioSolicitudEstudioSelectedDetalle = FechaFinSolicitudEstudioSelectedDetalle = null;
                        TipoEstudioSelectedDetalle = EstatusEstudioSelectedDetalle = -1;
                        ResultadoSolicitudEstudioSelectedDetalle = string.Empty;
                        DiasBonificadosSolicitudEstudioSelectedDetalle = TipoMedidaSolicitudEstudioSelectedDetalle = null;
                        PrepararListasEstudioPersonalidadDetalle();
                        ValidacionesPopUpEstudioPersonalidadDetalle();
                        PopUpsViewModels.ShowPopUp(this, PopUpsViewModels.TipoPopUp.AGREGAR_ESTUDIO_PERSONALIDAD_DETALLE);
                    }

                    break;

                case "editar_desarrollo_estudio":
                    if (SelectedEstudioPersonalidad == null)
                    {
                        new Dialogos().ConfirmacionDialogo("Validación", "Es necesario seleccionar un estudio de personalidad para continuar");
                        break;
                    }
                    else
                    {
                        if (SelectedEstudioPersonalidadDetalle == null)
                        {
                            new Dialogos().ConfirmacionDialogo("Validación", "Es necesario seleccionar un detalle de estudio de personalidad para continuar");
                            break;
                        }

                        if (!PEditar)
                        {
                            (new Dialogos()).ConfirmacionDialogo("Validación", "No cuenta con suficientes privilegios para realizar esta acción.");
                            return;
                        }

                        else
                        {
                            TipoEstudioSelectedDetalle = SelectedEstudioPersonalidadDetalle.ID_TIPO ?? -1;
                            EstatusEstudioSelectedDetalle = SelectedEstudioPersonalidadDetalle.ID_ESTATUS ?? -1;
                            FechaSolicitudEstudioSelectedDetalle = SelectedEstudioPersonalidadDetalle.SOLICITUD_FEC;
                            FechaInicioSolicitudEstudioSelectedDetalle = SelectedEstudioPersonalidadDetalle.INICIO_FEC;
                            FechaFinSolicitudEstudioSelectedDetalle = SelectedEstudioPersonalidadDetalle.TERMINO_FEC;
                            ResultadoSolicitudEstudioSelectedDetalle = SelectedEstudioPersonalidadDetalle.RESULTADO;
                            DiasBonificadosSolicitudEstudioSelectedDetalle = SelectedEstudioPersonalidadDetalle.DIAS_BONIFICADOS;
                            PrepararListasEstudioPersonalidadDetalle();
                            ValidacionesPopUpEstudioPersonalidadDetalle();
                            PopUpsViewModels.ShowPopUp(this, PopUpsViewModels.TipoPopUp.AGREGAR_ESTUDIO_PERSONALIDAD_DETALLE);
                        }
                    }

                    break;

                case "insertar_estudio_personalidad":
                    if (SelectIngreso == null)
                    {
                        new Dialogos().ConfirmacionDialogo("Validación", "Favor de seleccionar un ingreso");
                        break;
                    }

                    else
                    {
                        if (!PInsertar)
                        {
                            (new Dialogos()).ConfirmacionDialogo("Validación", "No cuenta con suficientes privilegios para realizar esta acción.");
                            return;
                        }

                        Motivo = SolicitadoPor = string.Empty;
                        FechaInicio = FechaFin = FechaSolicitud = null;
                        SelectedEstudioPersonalidad = null;
                        MotivoEstudioPadreSelected = SituacionEstudioPadreSelected = -1;
                        ValidacionesPopUpEstudioPersonalidad();
                        PrepararListasEstudioPersonalidadPadre();
                        PopUpsViewModels.ShowPopUp(this, PopUpsViewModels.TipoPopUp.AGREGAR_ESTUDIO_PERSONALIDAD);
                    }
                    break;

                case "editar_estudio_personalidad":
                    if (IdEstudioPersonalidadPdre == null)
                    {
                        new Dialogos().ConfirmacionDialogo("Validación", "Es necesario seleccionar un estudio de personalidad para continuar");
                        break;
                    }

                    else
                    {
                        if (!PEditar)
                        {
                            (new Dialogos()).ConfirmacionDialogo("Validación", "No cuenta con suficientes privilegios para realizar esta acción.");
                            return;
                        }

                        ValidacionesPopUpEstudioPersonalidad();
                        PrepararListasEstudioPersonalidadPadre();
                        ProcesaEstudioExistente();
                        PopUpsViewModels.ShowPopUp(this, PopUpsViewModels.TipoPopUp.AGREGAR_ESTUDIO_PERSONALIDAD);
                    }

                    break;
                case "cancel_guardar_estudio_personalidad":
                    FechaSolicitud = null;
                    Motivo = SolicitadoPor = string.Empty;
                    SelectedEstudioPersonalidad = null;
                    ListEstudiosPersonalidadDetalle = new ObservableCollection<PERSONALIDAD_DETALLE>();
                    PopUpsViewModels.ClosePopUp(PopUpsViewModels.TipoPopUp.AGREGAR_ESTUDIO_PERSONALIDAD);
                    break;

                case "guardar_estudio_personalidad":
                    if (base.HasErrors)
                    {
                        Application.Current.Dispatcher.Invoke((Action)(delegate
                        {
                            (new Dialogos()).ConfirmacionDialogo("Advertencia!", base.Error);
                        }));
                    }

                    else
                    {
                        Guardar();
                        ConfiguraPermisos();
                        PopUpsViewModels.ClosePopUp(PopUpsViewModels.TipoPopUp.AGREGAR_ESTUDIO_PERSONALIDAD);
                    }

                    break;

                case "buscar_seleccionar":
                    if (SelectIngreso == null)
                    {
                        new Dialogos().ConfirmacionDialogo("Validación", "Favor de seleccionar un ingreso");
                        break;
                    };

                    foreach (var item in Parametro.ESTATUS_ADMINISTRATIVO_INACT)
                    {
                        if (SelectIngreso.ID_ESTATUS_ADMINISTRATIVO == item)
                        {
                            new Dialogos().ConfirmacionDialogo("Notificación!", "El ingreso seleccionado no esta activo.");
                            return;
                        };
                    };

                    if (SelectIngreso.ID_UB_CENTRO != GlobalVar.gCentro)
                    {
                        new Dialogos().ConfirmacionDialogo("Validación", "El ingreso seleccionado no pertenece a su centro.");
                        break;
                    };

                    var toleranciaTraslado = Parametro.TOLERANCIA_TRASLADO_EDIFICIO;
                    if (SelectIngreso.TRASLADO_DETALLE.Any(a => (a.ID_ESTATUS != "CA" ? a.TRASLADO.ORIGEN_TIPO != "F" : false) && a.TRASLADO.TRASLADO_FEC.AddHours(-toleranciaTraslado) <= Fechas.GetFechaDateServer))
                    {
                        new Dialogos().ConfirmacionDialogo("Notificación!", "El interno [" + SelectIngreso.ID_ANIO.ToString() + "/" +
                            SelectIngreso.ID_IMPUTADO.ToString() + "] tiene un traslado proximo y no tiene permitido ningun cambio de informacion.");
                        return;
                    }


                    SelectedIngreso = SelectIngreso;

                    StaticSourcesViewModel.SourceChanged = false;
                    SeleccionaIngreso();
                    PopUpsViewModels.ClosePopUp(PopUpsViewModels.TipoPopUp.BUSQUEDA);
                    break;

                case "limpiar_menu":
                    ((System.Windows.Controls.ContentControl)PopUpsViewModels.MainWindow.FindName("contentControl")).Content = new EstudioPersonalidadView();
                    ((System.Windows.Controls.ContentControl)PopUpsViewModels.MainWindow.FindName("contentControl")).DataContext = new ControlPenales.EstudioPersonalidadViewModel(ventana_origen);
                    break;

                case "buscar_salir":
                    StaticSourcesViewModel.SourceChanged = false;
                    PopUpsViewModels.ClosePopUp(PopUpsViewModels.TipoPopUp.BUSQUEDA);
                    break;

                case "salir_menu":
                    PrincipalViewModel.SalirMenu();
                    break;

                case "reporte_menu":
                    //MuestraFormatoCierreEstudiosPersonalidad();
                    break;

                case "buscarEstudiosPersonalidad":
                    BuscarEstudiosP();
                    break;

                case "buscarEstudiosPersonalidadTerminados":
                    BuscarEstudiosTerminados();
                    break;
                default:
                    ///no action
                    break;

                #region Desarrollo
                case "guardar_desarrollo_estudios_personalidad":
                    if (IdEstudioPersonalidadPdre == null)
                    {
                        new Dialogos().ConfirmacionDialogo("Validación", "Seleccione un estudio para continuar");
                        break;
                    }

                    if (IdEstudioPersonalidadPdre.INICIO_FEC == null)
                    {
                        new Dialogos().ConfirmacionDialogo("Validación", "Ingrese la fecha de inicio al estudio de personalidad antes de programar");
                        break;
                    }

                    if (IdEstudioPersonalidadPdre.TERMINO_FEC == null)
                    {
                        new Dialogos().ConfirmacionDialogo("Validación", "Ingrese la fecha de fin al estudio de personalidad antes de programar");
                        break;
                    }

                    GuardarDesarrolloPersonalidad();
                    break;
                #endregion

                case "cancelar_nueva_fecha_personalidad":
                    StaticSourcesViewModel.SourceChanged = false;
                    PopUpsViewModels.ClosePopUp(PopUpsViewModels.TipoPopUp.DEFINIR_FECHAS_DESARROLLO_PERSONALIDAD);
                    break;
                case "define_fechas_desarrollo_personalidad":
                    if (SelectedEstudioPersonalidadDetalle == null)
                    {
                        new Dialogos().ConfirmacionDialogo("Validación!", "Seleccione un registro para continuar");
                        return;
                    }

                    if (SelectedEstudioPersonalidadDetalle.ID_ESTATUS != (short)eEstatusSituacionPersonalidad.ASIGNADO)
                    {
                        new Dialogos().ConfirmacionDialogo("Validación!", "Verifique el estatus del estudio, debe estar Asignado");
                        return;
                    };

                    if (SelectedEstudioPersonalidadDetalle.PERSONALIDAD != null && SelectedEstudioPersonalidadDetalle.PERSONALIDAD.ID_SITUACION != (short)eEstatusSituacionPersonalidad.ASIGNADO)
                    {
                        new Dialogos().ConfirmacionDialogo("Validación!", "Seleccione un estudio de personalidad activo");
                        return;
                    };
                    if (SelectedEstudioPersonalidadDetalle.INICIO_FEC == null)
                    {
                        new Dialogos().ConfirmacionDialogo("Validación!", "Capture las fechas de inicio y fin");
                        return;
                    };

                    if (SelectedEstudioPersonalidadDetalle.TERMINO_FEC == null)
                    {
                        new Dialogos().ConfirmacionDialogo("Validación!", "Capture las fechas de inicio y fin");
                        return;
                    };

                    if (SelectedEstudioPersonalidadDetalle.TERMINO_FEC > SelectedEstudioPersonalidadDetalle.PERSONALIDAD.TERMINO_FEC)
                    {
                        new Dialogos().ConfirmacionDialogo("Validación!", "Capture las fechas de inicio y fin");
                        return;
                    };

                    if (SelectedEstudioPersonalidadDetalle.INICIO_FEC < SelectedEstudioPersonalidadDetalle.PERSONALIDAD.INICIO_FEC)
                    {
                        new Dialogos().ConfirmacionDialogo("Validación!", "Capture las fechas de inicio y fin");
                        return;
                    };

                    InicializaEntornoDetallePersonalidad();
                    PopUpsViewModels.ShowPopUp(this, PopUpsViewModels.TipoPopUp.DEFINIR_FECHAS_DESARROLLO_PERSONALIDAD);
                    break;

                case "agregar_nueva_fecha_personalidad":
                    if (FechaSeleccionadaInicioDetalleP == null || HorasSeleccionadasFechaInicioDesarrolloP == null)
                    {
                        new Dialogos().ConfirmacionDialogo("Validación!", "Capture la fecha y hora de inicio");
                        return;
                    };

                    //if (FechaSeleccionadaFinDetalleP == null || HorasSeleccionadasFechaFinDesarrolloP == null)
                    //{
                    //    new Dialogos().ConfirmacionDialogo("Validación!", "Capture la fecha y hora de fin");
                    //    return;
                    //};

                    if (SelectedAreaDetalleP == null || SelectedAreaDetalleP == -1)
                    {
                        new Dialogos().ConfirmacionDialogo("Validación!", "Capture el area");
                        return;
                    };

                    AgregaFechaNueva();
                    break;
                case "borrar_fechas_desarrollo_personalidad":
                    if (LstDiasProgramados.Remove(SeledtedDiaProgramado))
                        SeledtedDiaProgramado = null;
                    break;

                case "guardar_nueva_fecha_personalidad":
                    var FechasProgramadasPorEstudio = new SSP.Controlador.Catalogo.Justicia.cPersonalidadDetalleDias().GetData(x => x.ID_ESTUDIO == SelectedEstudioPersonalidadDetalle.ID_ESTUDIO && x.ID_IMPUTADO == SelectedEstudioPersonalidadDetalle.ID_IMPUTADO && x.ID_INGRESO == SelectedEstudioPersonalidadDetalle.ID_INGRESO && x.ID_ANIO == SelectedEstudioPersonalidadDetalle.ID_ANIO);
                    //if (LstDiasProgramados != null && LstDiasProgramados.Any())
                    //{
                    if (FechasProgramadasPorEstudio.Any())
                    {
                        foreach (var item in FechasProgramadasPorEstudio)
                            if (item.FECHA_INICIO.HasValue)
                            {
                                //estas son las fechas que apenas se estan programando, no tienen un id aun
                                var FechasHoyProgramadas = LstDiasProgramados.Where(x => x.ID_CONSEC == decimal.Zero && x.FECHA_INICIO.Value.Year == item.FECHA_INICIO.Value.Year && x.FECHA_INICIO.Value.Month == item.FECHA_INICIO.Value.Month && x.FECHA_INICIO.Value.Day == item.FECHA_INICIO.Value.Day && x.FECHA_INICIO.Value.Hour == item.FECHA_INICIO.Value.Hour && x.FECHA_INICIO.Value.Minute == item.FECHA_INICIO.Value.Minute);
                                if (FechasHoyProgramadas != null && FechasHoyProgramadas.Any())
                                {
                                    new Dialogos().ConfirmacionDialogo("Validación!", "Una de las fechas ingresadas interfiere con otra fecha ya programada");
                                    return;
                                };
                            };
                    }
                    else
                    {
                        var FechasPorEstudioGral = new SSP.Controlador.Catalogo.Justicia.cPersonalidadDetalleDias().GetData(x => x.ID_ESTUDIO == SelectedEstudioPersonalidadDetalle.ID_ESTUDIO && x.ID_CENTRO == SelectedEstudioPersonalidadDetalle.ID_CENTRO && x.ID_IMPUTADO == SelectedEstudioPersonalidadDetalle.ID_IMPUTADO && x.ID_INGRESO == SelectedEstudioPersonalidadDetalle.ID_INGRESO);
                        if (FechasPorEstudioGral.Any())
                            foreach (var item in FechasPorEstudioGral)
                                if (item.FECHA_INICIO.HasValue)
                                {
                                    var FechasHoyProgramadas = LstDiasProgramados.Where(x => x.ID_CONSEC == decimal.Zero && x.FECHA_INICIO.Value.Year == item.FECHA_INICIO.Value.Year && x.FECHA_INICIO.Value.Month == item.FECHA_INICIO.Value.Month && x.FECHA_INICIO.Value.Day == item.FECHA_INICIO.Value.Day && x.FECHA_INICIO.Value.Hour == item.FECHA_INICIO.Value.Hour && x.FECHA_INICIO.Value.Minute == item.FECHA_INICIO.Value.Minute);
                                    if (FechasHoyProgramadas != null && FechasHoyProgramadas.Any())
                                    {
                                        new Dialogos().ConfirmacionDialogo("Validación!", "Una de las fechas ingresadas interfiere con otra fecha ya programada");
                                        return;
                                    };
                                };
                    }

                    if (await StaticSourcesViewModel.OperacionesAsync<bool>("Guardando Fechas de Estudios de Personalidad", () => GuardaFechasEstudiosPersonalidad()))
                    {
                        (new Dialogos()).ConfirmacionDialogo("Exito!", "Se han registrado las fechas exitosamente");
                        PopUpsViewModels.ClosePopUp(PopUpsViewModels.TipoPopUp.DEFINIR_FECHAS_DESARROLLO_PERSONALIDAD);
                        StaticSourcesViewModel.SourceChanged = false;
                    }
                    else
                        (new Dialogos()).ConfirmacionDialogo("Error!", "Surgió un error al ingresar las fechas");
                    //}
                    //else
                    //    (new Dialogos()).ConfirmacionDialogo("Validación!", "Agregue al menos un registro dentro de la lista de fechas");


                    break;
            }
        }

        private bool GuardaFechasEstudiosPersonalidad()
        {
            try
            {
                System.Collections.Generic.List<SSP.Servidor.PERSONALIDAD_DETALLE_DIAS> listD = new List<PERSONALIDAD_DETALLE_DIAS>();
                if (LstDiasProgramados != null && LstDiasProgramados.Any())
                    foreach (var item in LstDiasProgramados)
                        listD.Add(new SSP.Servidor.PERSONALIDAD_DETALLE_DIAS
                            {
                                FECHA_FINAL = SelectedEstudioPersonalidadDetalle != null ? SelectedEstudioPersonalidadDetalle.TERMINO_FEC : new DateTime?(),
                                FECHA_INICIO = item.FECHA_INICIO,
                                ID_ANIO = item.ID_ANIO,
                                ID_AREA = item.ID_AREA,
                                ID_CENTRO = item.ID_CENTRO,
                                ID_ESTUDIO = SelectedEstudioPersonalidadDetalle != null ? SelectedEstudioPersonalidadDetalle.ID_ESTUDIO : new short(),
                                ID_DETALLE = SelectedEstudioPersonalidadDetalle != null ? SelectedEstudioPersonalidadDetalle.ID_DETALLE : new short(),
                                ID_IMPUTADO = item.ID_IMPUTADO,
                                ID_INGRESO = item.ID_INGRESO
                            });

                SSP.Servidor.PERSONALIDAD_DETALLE _Detalle = new PERSONALIDAD_DETALLE()
                {
                    DIAS_BONIFICADOS = SelectedEstudioPersonalidadDetalle.DIAS_BONIFICADOS,
                    ID_ANIO = SelectedEstudioPersonalidadDetalle.ID_ANIO,
                    ID_AREA = SelectedEstudioPersonalidadDetalle.ID_AREA,
                    ID_CENTRO = SelectedEstudioPersonalidadDetalle.ID_CENTRO,
                    ID_DETALLE = SelectedEstudioPersonalidadDetalle.ID_DETALLE,
                    ID_ESTATUS = SelectedEstudioPersonalidadDetalle.ID_ESTATUS,
                    ID_ESTUDIO = SelectedEstudioPersonalidadDetalle.ID_ESTUDIO,
                    ID_IMPUTADO = SelectedEstudioPersonalidadDetalle.ID_IMPUTADO,
                    ID_INGRESO = SelectedEstudioPersonalidadDetalle.ID_INGRESO,
                    ID_TIPO = SelectedEstudioPersonalidadDetalle.ID_TIPO,
                    INICIO_FEC = SelectedEstudioPersonalidadDetalle.INICIO_FEC,
                    RESULTADO = SelectedEstudioPersonalidadDetalle.RESULTADO,
                    SOLICITUD_FEC = SelectedEstudioPersonalidadDetalle.SOLICITUD_FEC,
                    TERMINO_FEC = SelectedEstudioPersonalidadDetalle.TERMINO_FEC,
                    TIPO_MEDIA = SelectedEstudioPersonalidadDetalle.TIPO_MEDIA,
                    ESTUDIO = SelectedEstudioPersonalidadDetalle.ESTUDIO
                };

                return new cPersonalidadDetalleDias().GuardaFechasProgramacionEstudiosPersonalidad(listD, _Detalle);
            }
            catch (Exception exc)
            {
                return false;
            }
        }

        private void AgregaFechaNueva()
        {
            try
            {
                if (LstDiasProgramados == null)
                    LstDiasProgramados = new ObservableCollection<PERSONALIDAD_DETALLE_DIAS>();

                DateTime _FechaInicio = new DateTime(FechaSeleccionadaInicioDetalleP.Value.Year, FechaSeleccionadaInicioDetalleP.Value.Month, FechaSeleccionadaInicioDetalleP.Value.Day, HorasSeleccionadasFechaInicioDesarrolloP.Value.Hour, HorasSeleccionadasFechaInicioDesarrolloP.Value.Minute, HorasSeleccionadasFechaInicioDesarrolloP.Value.Second);
                if (_FechaInicio < FechaInicioEstudioDetalle)
                {
                    new Dialogos().ConfirmacionDialogo("Validación!", "Verifique la hora de inicio especificada");
                    return;
                };

                var FechasProgramadasPorEstudio = new SSP.Controlador.Catalogo.Justicia.cPersonalidadDetalleDias().GetData(x => x.ID_ESTUDIO == SelectedEstudioPersonalidadDetalle.ID_ESTUDIO && x.ID_IMPUTADO == SelectedEstudioPersonalidadDetalle.ID_IMPUTADO && x.ID_INGRESO == SelectedEstudioPersonalidadDetalle.ID_INGRESO && x.ID_ANIO == SelectedEstudioPersonalidadDetalle.ID_ANIO);
                if (FechasProgramadasPorEstudio.Any())
                {
                    //valida con respecto ala fecha que recien se esta programando
                    if (FechasProgramadasPorEstudio.Any(x => x.FECHA_INICIO.HasValue && x.FECHA_INICIO.Value.Year == _FechaInicio.Year && x.FECHA_INICIO.Value.Month == _FechaInicio.Month && x.FECHA_INICIO.Value.Day == _FechaInicio.Day && x.FECHA_INICIO.Value.Hour == _FechaInicio.Hour && x.FECHA_INICIO.Value.Minute == _FechaInicio.Minute))
                    {
                        new Dialogos().ConfirmacionDialogo("Validación!", "Una de las fechas ingresadas interfiere con otra fecha ya programada");
                        return;
                    }
                }
                else
                {
                    if (LstDiasProgramados.Any(x => x.FECHA_INICIO.HasValue && x.FECHA_INICIO.Value.Year == _FechaInicio.Year && x.FECHA_INICIO.Value.Month == _FechaInicio.Month && x.FECHA_INICIO.Value.Day == _FechaInicio.Day && x.FECHA_INICIO.Value.Hour == _FechaInicio.Hour && x.FECHA_INICIO.Value.Minute == _FechaInicio.Minute))
                    {
                        new Dialogos().ConfirmacionDialogo("Validación!", "Una de las fechas ingresadas interfiere con otra fecha ya programada");
                        return;
                    };
                }

                //DateTime _FechaFin = new DateTime(FechaSeleccionadaFinDetalleP.Value.Year,
                //    FechaSeleccionadaFinDetalleP.Value.Month,
                //    FechaSeleccionadaFinDetalleP.Value.Day,
                //    HorasSeleccionadasFechaFinDesarrolloP.Value.Hour,
                //    HorasSeleccionadasFechaFinDesarrolloP.Value.Minute,
                //    HorasSeleccionadasFechaFinDesarrolloP.Value.Second
                //    );

                //int Horas = FechaFinEstudioDetalle.Value.Hour == 0 ? 23 : FechaFinEstudioDetalle.Value.Hour;
                //int Minutos = FechaFinEstudioDetalle.Value.Hour == 0 ? 59 : FechaFinEstudioDetalle.Value.Minute;
                //int Segundos = FechaFinEstudioDetalle.Value.Hour == 0 ? 59 : FechaFinEstudioDetalle.Value.Second;
                //DateTime _fecFin = new DateTime(FechaFinEstudioDetalle.Value.Year, FechaFinEstudioDetalle.Value.Month, FechaFinEstudioDetalle.Value.Day, Horas, Minutos, Segundos);

                //if (_FechaFin > _fecFin)
                //{
                //    new Dialogos().ConfirmacionDialogo("Validación!", "Verifique la hora de fin especificada");
                //    return;
                //};

                //if (LstDiasProgramados.Any())
                //    if (LstDiasProgramados.Any(x => _FechaInicio < x.FECHA_FINAL))
                //    {
                //        new Dialogos().ConfirmacionDialogo("Validación!", "Verifique que la fecha de inicio seleccionada no se traslape con otra ya programada");
                //        return;
                //    };

                LstDiasProgramados.Add(new SSP.Servidor.PERSONALIDAD_DETALLE_DIAS()
                {
                    FECHA_FINAL = null,
                    FECHA_INICIO = _FechaInicio,
                    ID_ANIO = SelectedEstudioPersonalidadDetalle.ID_ANIO,
                    ID_CENTRO = SelectedEstudioPersonalidadDetalle.ID_CENTRO,
                    ID_ESTUDIO = SelectedEstudioPersonalidadDetalle.ID_ESTUDIO,
                    ID_INGRESO = SelectedEstudioPersonalidadDetalle.ID_INGRESO,
                    ID_IMPUTADO = SelectedEstudioPersonalidadDetalle.ID_IMPUTADO,
                    ID_DETALLE = SelectedEstudioPersonalidadDetalle.ID_DETALLE,
                    ID_AREA = SelectedAreaDetalleP,
                    AREA = SelectedAreaEdicionFechas
                });

                SelectedAreaDetalleP = -1;
                FechaInicioEstudioDetalle = SelectedEstudioPersonalidadDetalle != null ? SelectedEstudioPersonalidadDetalle.INICIO_FEC : new DateTime?();
                FechaFinEstudioDetalle = SelectedEstudioPersonalidadDetalle != null ? SelectedEstudioPersonalidadDetalle.TERMINO_FEC : new DateTime?();
                FechaSeleccionadaInicioDetalleP = HorasSeleccionadasFechaInicioDesarrolloP = HorasSeleccionadasFechaFinDesarrolloP = FechaSeleccionadaFinDetalleP = new DateTime?();
            }
            catch (Exception exc)
            {
                throw;
            }
        }

        private void InicializaEntornoDetallePersonalidad()
        {
            try
            {
                LstAreasD = new ObservableCollection<AREA>(new cArea().GetData());
                LstAreasD.Insert(0, new AREA { DESCR = "SELECCIONE", ID_AREA = -1 });
                OnPropertyChanged("LstAreasD");

                SelectedAreaDetalleP = -1;

                NombreTipoEstudio = SelectedEstudioPersonalidadDetalle != null ? SelectedEstudioPersonalidadDetalle.ID_TIPO.HasValue ? !string.IsNullOrEmpty(SelectedEstudioPersonalidadDetalle.PERSONALIDAD_TIPO_ESTUDIO.DESCR) ? SelectedEstudioPersonalidadDetalle.PERSONALIDAD_TIPO_ESTUDIO.DESCR.Trim() : string.Empty : string.Empty : string.Empty;
                FechaInicioEstudioDetalle = SelectedEstudioPersonalidadDetalle != null ? SelectedEstudioPersonalidadDetalle.INICIO_FEC.HasValue ? SelectedEstudioPersonalidadDetalle.INICIO_FEC.Value : new DateTime?() : new DateTime?();
                FechaFinEstudioDetalle = SelectedEstudioPersonalidadDetalle != null ? SelectedEstudioPersonalidadDetalle.TERMINO_FEC.HasValue ? SelectedEstudioPersonalidadDetalle.TERMINO_FEC.Value : new DateTime?() : new DateTime?();
                FechaSeleccionadaInicioDetalleP = HorasSeleccionadasFechaInicioDesarrolloP = HorasSeleccionadasFechaFinDesarrolloP = FechaSeleccionadaFinDetalleP = new DateTime?();
                LstDiasProgramados = new ObservableCollection<PERSONALIDAD_DETALLE_DIAS>();
                LstDiasProgramados = new ObservableCollection<PERSONALIDAD_DETALLE_DIAS>(new SSP.Controlador.Catalogo.Justicia.cPersonalidadDetalleDias().GetData(x => x.ID_ESTUDIO == SelectedEstudioPersonalidadDetalle.ID_ESTUDIO && x.ID_IMPUTADO == SelectedEstudioPersonalidadDetalle.ID_IMPUTADO && x.ID_INGRESO == SelectedEstudioPersonalidadDetalle.ID_INGRESO && x.PERSONALIDAD_DETALLE.ID_TIPO == SelectedEstudioPersonalidadDetalle.ID_TIPO));
                OnPropertyChanged("LstDiasProgramados");
            }
            catch (Exception exc)
            {
                throw exc;
            }
        }

        private void GuardarDesarrolloPersonalidad()
        {
            try
            {
                var toleranciaTraslado = Parametro.TOLERANCIA_TRASLADO_EDIFICIO;
                if (IdEstudioPersonalidadPdre.INGRESO != null)
                    if (IdEstudioPersonalidadPdre.INGRESO.TRASLADO_DETALLE.Any(a => (a.ID_ESTATUS != "CA" ? a.TRASLADO.ORIGEN_TIPO != "F" : false) && a.TRASLADO.TRASLADO_FEC.AddHours(-toleranciaTraslado) <= Fechas.GetFechaDateServer))
                    {
                        new Dialogos().ConfirmacionDialogo("Notificación!", "El interno [" + SelectIngreso.ID_ANIO.ToString() + "/" +
                            SelectIngreso.ID_IMPUTADO.ToString() + "] tiene un traslado proximo y no tiene permitido ningun cambio de informacion.");
                        return;
                    };

                if (IdEstudioPersonalidadPdre.ID_SITUACION != (short)eEstatusSituacionPersonalidad.ASIGNADO)
                {
                    new Dialogos().ConfirmacionDialogo("Validación", "Seleccione el estudio de personalidad con estatus asignado");
                    return;
                }

                if (ListEstudiosPersonalidadDetalle != null && ListEstudiosPersonalidadDetalle.Any())
                    if (IdEstudioPersonalidadPdre.INICIO_FEC.HasValue)
                        if (IdEstudioPersonalidadPdre.TERMINO_FEC.HasValue)
                        {
                            if (!new cPersonalidadDetalle().ValidaDisponibleImputadoArea(IdEstudioPersonalidadPdre.INGRESO, null, IdEstudioPersonalidadPdre.INICIO_FEC, IdEstudioPersonalidadPdre.TERMINO_FEC))
                            {
                                new Dialogos().ConfirmacionDialogo("Validación", "Esta definida una atencion para este imputado");
                                return;
                            };

                            if (ListEstudiosPersonalidadDetalle.ToList().TrueForAll(x => !x.INICIO_FEC.HasValue))
                            {
                                new Dialogos().ConfirmacionDialogo("Validación", "Verifique las fechas de inicio especificadas");
                                return;
                            };

                            if (ListEstudiosPersonalidadDetalle.ToList().TrueForAll(x => !x.TERMINO_FEC.HasValue))
                            {
                                new Dialogos().ConfirmacionDialogo("Validación", "Verifique las fechas de fin especificadas");
                                return;
                            };

                            if (ListEstudiosPersonalidadDetalle.Any(x => x.INICIO_FEC.HasValue ? x.INICIO_FEC < IdEstudioPersonalidadPdre.INICIO_FEC : false) == true)
                            {
                                new Dialogos().ConfirmacionDialogo("Validación", "Verifique las fechas de inicio especificadas");
                                return;
                            };

                            if (ListEstudiosPersonalidadDetalle.Any(x => x.INICIO_FEC.HasValue ? x.INICIO_FEC > IdEstudioPersonalidadPdre.TERMINO_FEC : false) == true)
                            {
                                new Dialogos().ConfirmacionDialogo("Validación", "Verifique las fechas de inicio especificadas");
                                return;
                            };

                            if (ListEstudiosPersonalidadDetalle.Any(x => x.TERMINO_FEC.HasValue ? x.TERMINO_FEC > IdEstudioPersonalidadPdre.TERMINO_FEC : false) == true)
                            {
                                new Dialogos().ConfirmacionDialogo("Validación", "Verifique las fechas de fin especificadas");
                                return;
                            };

                            if (ListEstudiosPersonalidadDetalle.Any(x => x.TERMINO_FEC.HasValue ? x.TERMINO_FEC < IdEstudioPersonalidadPdre.INICIO_FEC : false) == true)
                            {
                                new Dialogos().ConfirmacionDialogo("Validación", "Verifique las fechas de fin especificadas");
                                return;
                            };

                            #region Validacion con respecto a las fechas
                            var _validacionesListas = ListEstudiosPersonalidadDetalle.Where(x => x.INICIO_FEC.HasValue || x.TERMINO_FEC.HasValue);//elementos a los que se les ha capturado al menos una fecha de inicio o fin

                            if (_validacionesListas != null && _validacionesListas.Any())
                                if (!_validacionesListas.ToList().TrueForAll(x => x.INICIO_FEC.HasValue && x.TERMINO_FEC.HasValue))
                                {
                                    new Dialogos().ConfirmacionDialogo("Validación", "Es necesario que proporcione fecha de inicio y fin");
                                    return;
                                };
                            #endregion
                            ///AHORA EL AREA ES EN EL DESARROLLO DE LOS DIAS
                            //if (ListEstudiosPersonalidadDetalle.ToList().TrueForAll(x => !x.ID_AREA.HasValue))
                            //{
                            //    new Dialogos().ConfirmacionDialogo("Validación", "Verifique el área a considerar");
                            //    return;
                            //};

                            List<SSP.Servidor.PERSONALIDAD_DETALLE> detalles = new List<PERSONALIDAD_DETALLE>();
                            foreach (var item in ListEstudiosPersonalidadDetalle)
                            {
                                detalles.Add(new SSP.Servidor.PERSONALIDAD_DETALLE
                                    {
                                        DIAS_BONIFICADOS = item.DIAS_BONIFICADOS,
                                        ID_AREA = item.ID_AREA,
                                        ID_ANIO = item.ID_ANIO,
                                        ID_CENTRO = item.ID_CENTRO,
                                        ID_DETALLE = item.ID_DETALLE,
                                        ID_ESTATUS = item.ID_ESTATUS,
                                        ID_ESTUDIO = item.ID_ESTUDIO,
                                        ID_IMPUTADO = item.ID_IMPUTADO,
                                        ID_INGRESO = item.ID_INGRESO,
                                        ID_TIPO = item.ID_TIPO,
                                        INICIO_FEC = item.INICIO_FEC,
                                        RESULTADO = item.RESULTADO,
                                        TIPO_MEDIA = item.TIPO_MEDIA,
                                        TERMINO_FEC = item.TERMINO_FEC,
                                        SOLICITUD_FEC = item.SOLICITUD_FEC
                                    });
                            };

                            if (new cPersonalidadDetalle().ActualizaFechasEstudiosHijos(detalles))
                            {
                                ConfiguraPermisos();//recarga para actualizar si alguno fue terminado
                                new Dialogos().ConfirmacionDialogo("Exito", "Se ha guardado el desarrollo del estudio de personalidad con exito");
                            }
                            else
                                new Dialogos().ConfirmacionDialogo("Error", "Surgió un error al guardar el desarrollo del estudio de personalidad");
                        }
            }
            catch (Exception exc)
            {
                throw exc;
            }
        }

        private void BuscarEstudiosP()
        {
            try
            {
                //nivel tres
                ListEstudiosPersonalidadDetalle = new ObservableCollection<PERSONALIDAD_DETALLE>();
                SelectedEstudioPersonalidadDetalle = null;

                //nivel dos
                ListEstudiosPersonalidad = new ObservableCollection<PERSONALIDAD>();
                IdEstudioPersonalidadPdre = null;

                //nivel uno
                LstCandidatos = new ObservableCollection<INGRESO>();//Lista principal con el nombre de los imputados con estudios por hacer
                SelectIngreso = null;

                DateTime f1, f2;
                if (FechaInicioBusqueda.HasValue)
                    f1 = FechaInicioBusqueda.Value;
                else
                    f1 = Fechas.GetFechaDateServer;

                if (FechaFinBusqueda.HasValue)
                    f2 = FechaFinBusqueda.Value;
                else
                    f2 = Fechas.GetFechaDateServer;

                f1 = new DateTime(f1.Year, f1.Month, f1.Day);
                f2 = new DateTime(f2.Year, f2.Month, f2.Day);

                var _dato = new cEstudioPersonalidad().BusquedaConParametros(f1, f2, NoOficioBusqueda, GlobalVar.gCentro);
                if (_dato != null && _dato.Any(x => x.ID_SITUACION != 4))
                    foreach (var item in _dato)
                        if (!LstCandidatos.Any(c => c.ID_IMPUTADO == item.ID_IMPUTADO && c.ID_INGRESO == item.ID_INGRESO && c.ID_ANIO == item.ID_ANIO && c.ID_CENTRO == item.ID_CENTRO))
                            LstCandidatos.Add(item.INGRESO);
            }
            catch (Exception exc)
            {
                throw;
            }
        }

        private void ProcesaFechasDesarrollo(PERSONALIDAD_DETALLE Entity)
        {
            try
            {
                if (Entity.PERSONALIDAD != null)
                {
                    if (Entity.PERSONALIDAD.ID_SITUACION.HasValue)
                    {
                        if (Entity.ID_ESTATUS == (short)eTipoEstudioDetalle.TERMINADO)
                        {
                            Mensaje("Este estudio está terminado", "Validación");
                            return;//NO SE MUEVE UN DETALLE TERMINADO
                        };

                        if (Entity.PERSONALIDAD.ID_SITUACION == (short)eTipoEstudioDetalle.ACTIVO || Entity.PERSONALIDAD.ID_SITUACION == (short)eTipoEstudioDetalle.ASIGNADO || Entity.PERSONALIDAD.ID_SITUACION == (short)eTipoEstudioDetalle.PENDIENTE)
                            if (Entity.ID_ESTATUS.HasValue)
                                if (Entity.ID_ESTATUS == (short)eTipoEstudioDetalle.ACTIVO || Entity.ID_ESTATUS == (short)eTipoEstudioDetalle.ASIGNADO || Entity.ID_ESTATUS == (short)eTipoEstudioDetalle.PENDIENTE)//considera los que aun estan pendientes, no los cancelados ni los terminados
                                {
                                    if (Entity.ID_AREA == null)
                                    {
                                        Mensaje("Seleccione el área", "Validación");
                                        return;
                                    }

                                    if (Entity.INICIO_FEC.HasValue && Entity.TERMINO_FEC.HasValue)
                                    {
                                        FechaInicioProgramacionEstudios = DateTime.Parse(Entity.INICIO_FEC.ToString());

                                        if (Entity.PERSONALIDAD.INICIO_FEC.HasValue && Entity.PERSONALIDAD.TERMINO_FEC.HasValue)
                                        {
                                            if (Entity.INICIO_FEC.Value.Year >= Entity.PERSONALIDAD.INICIO_FEC.Value.Year && Entity.INICIO_FEC.Value.Month >= Entity.PERSONALIDAD.INICIO_FEC.Value.Month && Entity.INICIO_FEC.Value.DayOfYear >= Entity.PERSONALIDAD.INICIO_FEC.Value.DayOfYear)
                                            {
                                                FechaFinProgramacionEstudios = DateTime.Parse(Entity.TERMINO_FEC.ToString());
                                                if (Entity.INICIO_FEC.Value.TimeOfDay < Entity.PERSONALIDAD.INICIO_FEC.Value.TimeOfDay)
                                                {
                                                    if (Entity.INICIO_FEC.Value.DayOfYear == Entity.PERSONALIDAD.INICIO_FEC.Value.DayOfYear)
                                                    {
                                                        Mensaje("La hora de inicio del desarrollo no puede ser mayor ala hora de inicio del estudio", "Validación");
                                                        return;
                                                    }
                                                };

                                                if (Entity.INICIO_FEC.Value <= Entity.PERSONALIDAD.TERMINO_FEC.Value)//la fecha de inicio es menor ala fecha de fin del estudio principal
                                                {//la fecha de inicio esta dentro del rango de fechas de inicio y fin del estudio principal
                                                    if (Entity.TERMINO_FEC.Value < Entity.PERSONALIDAD.TERMINO_FEC.Value && Entity.TERMINO_FEC.Value > Entity.PERSONALIDAD.INICIO_FEC.Value)
                                                    {
                                                        if (!new cPersonalidadDetalle().ValidaDisponibleImputadoArea(Entity.PERSONALIDAD.INGRESO, Entity.ID_AREA, Entity.INICIO_FEC, Entity.TERMINO_FEC))
                                                        {
                                                            Mensaje("Esta definida una atencion para este imputado", "Validación");
                                                            return;
                                                        };

                                                        if (Entity.PERSONALIDAD.INGRESO != null)
                                                            if (Entity.PERSONALIDAD.INGRESO.TRASLADO_DETALLE.Any(a => a.ID_ESTATUS != "CA" ? a.TRASLADO.ORIGEN_TIPO != "F" : false))
                                                            {
                                                                Mensaje("Esta definido un traslado para este imputado", "Validación");
                                                                return;
                                                            };

                                                        var _detalle = new PERSONALIDAD_DETALLE()
                                                        {
                                                            DIAS_BONIFICADOS = null,
                                                            ID_ANIO = Entity.ID_ANIO,
                                                            ID_DETALLE = Entity.ID_DETALLE,
                                                            ID_ESTATUS = Entity.ID_ESTATUS,
                                                            ID_CENTRO = Entity.ID_CENTRO,
                                                            ID_ESTUDIO = Entity.ID_ESTUDIO,
                                                            ID_IMPUTADO = Entity.ID_IMPUTADO,
                                                            ID_INGRESO = Entity.ID_INGRESO,
                                                            INICIO_FEC = Entity.INICIO_FEC,
                                                            TERMINO_FEC = Entity.TERMINO_FEC,
                                                            ID_AREA = Entity.ID_AREA
                                                        };

                                                        if (new cPersonalidadDetalle().ActualizaDetalleEstudioPersonalidad(_detalle))
                                                        {
                                                            Mensaje("Se han definido las fechas de inicio y fin correctamente", "Éxito");
                                                            return;
                                                        }
                                                    }
                                                    else
                                                    {
                                                        Mensaje("Las fechas de inicio y fin no estan dentro del rango especificado", "Validación");
                                                        return;
                                                    }
                                                }
                                            }
                                            else
                                            {
                                                Mensaje("Las fechas de inicio y fin no estan dentro del rango especificado", "Validación");
                                                return;
                                            }
                                        }
                                    };
                                };
                    };
                };
            }
            catch (Exception exc)
            {
                throw;
            }
        }
        private string ValidaFechasDesarrolloEstudiosPersonalidad(PERSONALIDAD Entity)
        {
            try
            {
                string _respuesta = string.Empty;
                if (Entity != null)
                {
                    return _respuesta;
                }
            }
            catch (Exception exc)
            {
                throw;
            }

            return string.Empty;
        }

        private void BuscarEstudiosTerminados()
        {
            try
            {
                //nivel uno
                LstEstudiosCerrdaos = new ObservableCollection<PERSONALIDAD>();
                SelectedEstudioCerrado = null;

                DateTime f1, f2;
                if (FechaInicioBusquedaEstudiosT.HasValue)
                    f1 = FechaInicioBusquedaEstudiosT.Value;
                else
                    f1 = Fechas.GetFechaDateServer;

                if (FechaFinBusquedaEstudiosT.HasValue)
                    f2 = FechaFinBusquedaEstudiosT.Value;
                else
                    f2 = Fechas.GetFechaDateServer;

                f1 = new DateTime(f1.Year, f1.Month, f1.Day);
                f2 = new DateTime(f2.Year, f2.Month, f2.Day);

                var _dato = new cEstudioPersonalidad().BuscarEstudiosTPersonalidadTerminados(f1, f2, NoOficioBusquedaEstudiosT, GlobalVar.gCentro);
                if (_dato != null && _dato.Any())
                {
                    foreach (var item in _dato)
                    {
                        item.RESULT_ESTUDIO = string.Empty;
                        LstEstudiosCerrdaos.Add(item);
                    }
                }
            }
            catch (Exception exc)
            {
                throw;
            }
        }

        private async void ClickEnter(Object obj)
        {
            try
            {
                if (obj != null)
                {
                    var textbox = obj as TextBox;
                    if (textbox != null)
                    {
                        switch (textbox.Name)
                        {
                            case "NombreBuscar":
                                NombreBuscar = textbox.Text;
                                break;
                            case "ApellidoPaternoBuscar":
                                ApellidoPaternoBuscar = textbox.Text;
                                break;
                            case "ApellidoMaternoBuscar":
                                ApellidoMaternoBuscar = textbox.Text;
                                break;
                            case "AnioBuscar":
                                if (!string.IsNullOrEmpty(textbox.Text))
                                    AnioBuscar = int.Parse(textbox.Text);
                                else
                                    AnioBuscar = null;
                                break;
                            case "FolioBuscar":
                                if (!string.IsNullOrEmpty(textbox.Text))
                                    FolioBuscar = int.Parse(textbox.Text);
                                else
                                    FolioBuscar = null;
                                break;
                        }
                    }
                }

                ListExpediente = new RangeEnabledObservableCollection<IMPUTADO>();
                ListExpediente.InsertRange(await SegmentarResultadoBusqueda());
                if (ListExpediente.Count > 0)//Empty row
                    EmptyExpedienteVisible = false;
                else
                    EmptyExpedienteVisible = true;

                ImagenImputado = ImagenIngreso = new Imagenes().getImagenPerson();
            }
            catch (Exception ex)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al ingresar búsqueda", ex);
            }
        }



        private void ProcesaEstudiosByPersonalidad(PERSONALIDAD Entity)
        {
            try
            {
                ListEstudiosPersonalidad = new ObservableCollection<PERSONALIDAD>();
                //ListEstudiosPersonalidadDetalle = new ObservableCollection<PERSONALIDAD_DETALLE>();

                if (Entity == null)
                    return;

                ListEstudiosPersonalidad = new ObservableCollection<PERSONALIDAD>(new cEstudioPersonalidad().ObtenerTodos(Entity.ID_IMPUTADO, Entity.ID_INGRESO));
                ListEstudiosPersonalidadDetalle = new ObservableCollection<PERSONALIDAD_DETALLE>();
                // ListEstudiosPersonalidadDetalle = new ObservableCollection<PERSONALIDAD_DETALLE>(new cPersonalidadDetalle().ObtenerTodosDetalle(Entity.ID_IMPUTADO, Entity.ID_INGRESO, Entity.ID_ESTUDIO));
            }
            catch (Exception exc)
            {
                throw;
            }
        }
        private async Task<List<IMPUTADO>> SegmentarResultadoBusqueda(int _Pag = 1)
        {
            try
            {
                if (string.IsNullOrEmpty(ApellidoPaternoBuscar) && string.IsNullOrEmpty(ApellidoMaternoBuscar) && string.IsNullOrEmpty(NombreBuscar) && !AnioBuscar.HasValue && !FolioBuscar.HasValue)
                    return new List<IMPUTADO>();

                Pagina = _Pag;
                var result = await StaticSourcesViewModel.CargarDatosAsync<ObservableCollection<IMPUTADO>>(() => new cImputado().ObtenerTodos( ApellidoPaternoBuscar, ApellidoMaternoBuscar, NombreBuscar, AnioBuscar, FolioBuscar, _Pag));
                if (result.Any())
                {
                    Pagina++;
                    SeguirCargando = true;
                }
                else
                    SeguirCargando = false;

                return result.ToList();
            }

            catch (Exception ex)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al segmentar resultados de búsqueda", ex);
                return new List<IMPUTADO>();
            }
        }

        private void SeleccionaIngreso()
        {
            try
            {
                if (SelectIngreso != null)
                {
                    AnioD = SelectIngreso.ID_ANIO;
                    FolioD = SelectIngreso.ID_IMPUTADO;
                    PaternoD = SelectIngreso.IMPUTADO != null ? !string.IsNullOrEmpty(SelectIngreso.IMPUTADO.PATERNO) ? SelectIngreso.IMPUTADO.PATERNO.Trim() : string.Empty : string.Empty;
                    MaternoD = SelectIngreso.IMPUTADO != null ? !string.IsNullOrEmpty(SelectIngreso.IMPUTADO.MATERNO) ? SelectIngreso.IMPUTADO.MATERNO.Trim() : string.Empty : string.Empty;
                    NombreD = SelectIngreso.IMPUTADO != null ? !string.IsNullOrEmpty(SelectIngreso.IMPUTADO.NOMBRE) ? SelectIngreso.IMPUTADO.NOMBRE.Trim() : string.Empty : string.Empty;
                    IngresosD = SelectIngreso.ID_INGRESO;

                    if (SelectIngreso.CAMA != null)
                    {
                        UbicacionD = string.Format("{0}-{1}{2}-{3}",
                            SelectIngreso.CAMA.CELDA != null ? SelectIngreso.CAMA.CELDA.SECTOR != null ? SelectIngreso.CAMA.CELDA.SECTOR.EDIFICIO != null ? !string.IsNullOrEmpty(SelectIngreso.CAMA.CELDA.SECTOR.EDIFICIO.DESCR) ? SelectIngreso.CAMA.CELDA.SECTOR.EDIFICIO.DESCR.Trim() : string.Empty : string.Empty : string.Empty : string.Empty,
                             SelectIngreso.CAMA.CELDA != null ? SelectIngreso.CAMA.CELDA.SECTOR != null ? !string.IsNullOrEmpty(SelectIngreso.CAMA.CELDA.SECTOR.DESCR) ? SelectIngreso.CAMA.CELDA.SECTOR.DESCR.Trim() : string.Empty : string.Empty : string.Empty,
                            SelectIngreso.CAMA.CELDA != null ? !string.IsNullOrEmpty(SelectIngreso.CAMA.CELDA.ID_CELDA) ? SelectIngreso.CAMA.CELDA.ID_CELDA.Trim() : string.Empty : string.Empty,
                                                   SelectIngreso.ID_UB_CAMA);
                    }
                    else
                        UbicacionD = string.Empty;

                    TipoSeguridadD = SelectIngreso.TIPO_SEGURIDAD != null ? !string.IsNullOrEmpty(SelectIngreso.TIPO_SEGURIDAD.DESCR) ? SelectIngreso.TIPO_SEGURIDAD.DESCR.Trim() : string.Empty : string.Empty;
                    FecIngresoD = SelectIngreso.FEC_INGRESO_CERESO;
                    ClasificacionJuridicaD = SelectIngreso.CLASIFICACION_JURIDICA != null ? !string.IsNullOrEmpty(SelectIngreso.CLASIFICACION_JURIDICA.DESCR) ? SelectIngreso.CLASIFICACION_JURIDICA.DESCR.Trim() : string.Empty : string.Empty;
                    EstatusD = SelectIngreso.ESTATUS_ADMINISTRATIVO != null ? !string.IsNullOrEmpty(SelectIngreso.ESTATUS_ADMINISTRATIVO.DESCR) ? SelectIngreso.ESTATUS_ADMINISTRATIVO.DESCR.Trim() : string.Empty : string.Empty;
                    ListEstudiosPersonalidad = new ObservableCollection<PERSONALIDAD>(new cEstudioPersonalidad().ObtenerTodos(SelectIngreso.ID_IMPUTADO, SelectIngreso.ID_INGRESO));
                    ListEstudiosPersonalidadDetalle = new ObservableCollection<PERSONALIDAD_DETALLE>();
                }

                else
                    return;
            }
            catch (Exception ex)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al seleccionar ingreso", ex);
            }
        }

        private void ConsultaDetalle()
        {
            try
            {
                if (SelectIngreso != null)
                    if (SelectedEstudioPersonalidad != null)
                        ListEstudiosPersonalidadDetalle = new ObservableCollection<PERSONALIDAD_DETALLE>(new cPersonalidadDetalle().ObtenerTodosDetalle(SelectIngreso.ID_IMPUTADO, SelectIngreso.ID_INGRESO, SelectedEstudioPersonalidad.ID_ESTUDIO));
                    else
                        return;
                else
                    return;
            }

            catch (Exception exc)
            {
                throw;
            }
        }

        private async void ModelEnter(Object obj)
        {
            try
            {
                if (obj != null)
                {
                    if (!PConsultar)
                    {
                        (new Dialogos()).ConfirmacionDialogo("Validación", "No cuenta con suficientes privilegios para realizar esta acción.");
                        return;
                    }
                    if (!obj.GetType().Name.Equals("String"))
                    {
                        var textbox = obj as TextBox;
                        if (textbox != null)
                        {
                            switch (textbox.Name)
                            {
                                case "NombreBuscar":
                                    NombreBuscar = textbox.Text;
                                    NombreD = NombreBuscar;
                                    FolioBuscar = FolioD;
                                    AnioBuscar = AnioD;
                                    break;
                                case "ApellidoPaternoBuscar":
                                    ApellidoPaternoBuscar = textbox.Text;
                                    PaternoD = ApellidoPaternoBuscar;
                                    FolioBuscar = FolioD;
                                    AnioBuscar = AnioD;
                                    break;
                                case "ApellidoMaternoBuscar":
                                    ApellidoMaternoBuscar = textbox.Text;
                                    MaternoD = ApellidoMaternoBuscar;
                                    FolioBuscar = FolioD;
                                    AnioBuscar = AnioD;
                                    break;
                                case "FolioBuscar":
                                    if (!string.IsNullOrEmpty(textbox.Text))
                                        FolioBuscar = int.Parse(textbox.Text);
                                    else
                                        FolioBuscar = null;
                                    AnioBuscar = AnioD;
                                    break;
                                case "AnioBuscar":
                                    if (!string.IsNullOrEmpty(textbox.Text))
                                        AnioBuscar = int.Parse(textbox.Text);
                                    else
                                        AnioBuscar = null;
                                    FolioBuscar = FolioD;
                                    break;
                            }
                        }
                    }
                }

                ListExpediente = new RangeEnabledObservableCollection<IMPUTADO>();

                if (string.IsNullOrEmpty(NombreD))
                    NombreBuscar = string.Empty;
                else
                    NombreBuscar = NombreD;

                if (string.IsNullOrEmpty(PaternoD))
                    ApellidoPaternoBuscar = string.Empty;
                else
                    ApellidoPaternoBuscar = PaternoD;

                if (string.IsNullOrEmpty(MaternoD))
                    ApellidoMaternoBuscar = string.Empty;
                else
                    ApellidoMaternoBuscar = MaternoD;

                if (AnioBuscar != null && FolioBuscar != null)
                {
                    ListExpediente = new RangeEnabledObservableCollection<IMPUTADO>();
                    ListExpediente.InsertRange(await SegmentarResultadoBusqueda());
                    if (ListExpediente.Count == 1)
                    {
                        foreach (var item in Parametro.ESTATUS_ADMINISTRATIVO_INACT)
                        {
                            if (ListExpediente[0].INGRESO.OrderByDescending(o => o.ID_INGRESO).FirstOrDefault().ID_ESTATUS_ADMINISTRATIVO == item)
                            {
                                SelectExpediente = null;
                                SelectIngreso = null;
                                ImagenImputado = ImagenIngreso = new Imagenes().getImagenPerson();
                                PopUpsViewModels.ShowPopUp(this, PopUpsViewModels.TipoPopUp.BUSQUEDA);
                                new Dialogos().ConfirmacionDialogo("Notificación!", "No se encontró ningún ingreso activo en este imputado.");
                                return;
                            }
                        }

                        if (ListExpediente[0].INGRESO.OrderByDescending(o => o.ID_INGRESO).FirstOrDefault().ID_UB_CENTRO != GlobalVar.gCentro)
                        {
                            SelectExpediente = null;
                            SelectIngreso = null;
                            ImagenImputado = ImagenIngreso = new Imagenes().getImagenPerson();
                            PopUpsViewModels.ShowPopUp(this, PopUpsViewModels.TipoPopUp.BUSQUEDA);
                            new Dialogos().ConfirmacionDialogo("Validación", "El ingreso seleccionado no pertenece a su centro.");
                            return;
                        }

                        SelectExpediente = ListExpediente[0];
                        SelectIngreso = ListExpediente[0].INGRESO.OrderByDescending(o => o.ID_INGRESO).FirstOrDefault();
                        SelectedInterno = SelectExpediente;
                        var toleranciaTraslado = Parametro.TOLERANCIA_TRASLADO_EDIFICIO;
                        if (SelectIngreso.TRASLADO_DETALLE.Any(a => (a.ID_ESTATUS != "CA" ? a.TRASLADO.ORIGEN_TIPO != "F" : false) && a.TRASLADO.TRASLADO_FEC.AddHours(-toleranciaTraslado) <= Fechas.GetFechaDateServer))
                        {
                            new Dialogos().ConfirmacionDialogo("Notificación!", "El interno [" + SelectIngreso.ID_ANIO.ToString() + "/" +
                                SelectIngreso.ID_IMPUTADO.ToString() + "] tiene un traslado proximo y no tiene permitido ningun cambio de informacion.");
                            return;
                        }
                        SeleccionaIngreso();
                        StaticSourcesViewModel.SourceChanged = false;
                        PopUpsViewModels.ClosePopUp(PopUpsViewModels.TipoPopUp.BUSQUEDA);
                        return;
                    }
                    else
                    {
                        SelectExpediente = null;
                        SelectIngreso = null;
                        ListEstudiosPersonalidad = new ObservableCollection<PERSONALIDAD>();
                        ListEstudiosPersonalidadDetalle = new ObservableCollection<PERSONALIDAD_DETALLE>();
                        ImagenImputado = ImagenIngreso = new Imagenes().getImagenPerson();
                        PopUpsViewModels.ShowPopUp(this, PopUpsViewModels.TipoPopUp.BUSQUEDA);
                    }
                }
                else
                {
                    ListExpediente = new RangeEnabledObservableCollection<IMPUTADO>();
                    ListExpediente.InsertRange(await SegmentarResultadoBusqueda());
                    if (ListExpediente.Count > 0)//Empty row
                        EmptyExpedienteVisible = false;
                    else
                        EmptyExpedienteVisible = true;

                    ImagenImputado = ImagenIngreso = new Imagenes().getImagenPerson();
                    PopUpsViewModels.ShowPopUp(this, PopUpsViewModels.TipoPopUp.BUSQUEDA);
                }
            }
            catch (Exception ex)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al ingresar búsqueda", ex);
            }
        }

        private void GuardarDetalleEstudioPersonalidad()
        {
            try
            {
                if (SelectedEstudioPersonalidad == null)
                    return;

                if (SelectIngreso == null)
                    return;

                if (SelectedEstudioPersonalidadDetalle == null)
                {
                    PERSONALIDAD_DETALLE Detalle = new PERSONALIDAD_DETALLE();
                    Detalle.DIAS_BONIFICADOS = DiasBonificadosSolicitudEstudioSelectedDetalle;
                    Detalle.ID_ANIO = SelectIngreso.ID_ANIO;
                    Detalle.ID_CENTRO = SelectIngreso.ID_CENTRO;
                    Detalle.ID_ESTATUS = EstatusEstudioSelectedDetalle;
                    Detalle.ID_ESTUDIO = SelectedEstudioPersonalidad.ID_ESTUDIO;
                    Detalle.ID_IMPUTADO = SelectIngreso.ID_IMPUTADO;
                    Detalle.ID_INGRESO = SelectIngreso.ID_INGRESO;
                    Detalle.ID_TIPO = TipoEstudioSelectedDetalle;
                    Detalle.INICIO_FEC = FechaInicioSolicitudEstudioSelectedDetalle;
                    Detalle.RESULTADO = ResultadoSolicitudEstudioSelectedDetalle;
                    Detalle.SOLICITUD_FEC = FechaSolicitudEstudioSelectedDetalle;
                    Detalle.TERMINO_FEC = FechaFinSolicitudEstudioSelectedDetalle;
                    if (new cPersonalidadDetalle().Insertar(Detalle))
                    {
                        PopUpsViewModels.ClosePopUp(PopUpsViewModels.TipoPopUp.AGREGAR_ESTUDIO_PERSONALIDAD_DETALLE);
                        SelectedEstudioPersonalidadDetalle = null;
                        new Dialogos().ConfirmacionDialogo("EXITO!", "Se ha registrado el desarrollo del estudio de personalidad con éxito.");
                        ListEstudiosPersonalidadDetalle = new ObservableCollection<PERSONALIDAD_DETALLE>(new cPersonalidadDetalle().GetData(x => x.ID_ESTUDIO == SelectedEstudioPersonalidad.ID_ESTUDIO && x.ID_IMPUTADO == SelectIngreso.ID_IMPUTADO).OrderBy(y => y.ID_DETALLE));
                        return;
                    }
                    else
                    {
                        new Dialogos().ConfirmacionDialogo("ERROR!", "Surgió un error al guardar el desarrollo del estudio de personalidad.");
                        return;
                    }
                }

                else
                {
                    PERSONALIDAD_DETALLE DetalleModificado = new PERSONALIDAD_DETALLE();
                    DetalleModificado.DIAS_BONIFICADOS = DiasBonificadosSolicitudEstudioSelectedDetalle;
                    DetalleModificado.ID_ANIO = SelectIngreso.ID_ANIO;
                    DetalleModificado.ID_CENTRO = SelectIngreso.ID_CENTRO;
                    DetalleModificado.ID_ESTATUS = EstatusEstudioSelectedDetalle;
                    DetalleModificado.ID_ESTUDIO = SelectedEstudioPersonalidad.ID_ESTUDIO;
                    DetalleModificado.ID_IMPUTADO = SelectIngreso.ID_IMPUTADO;
                    DetalleModificado.ID_INGRESO = SelectIngreso.ID_INGRESO;
                    DetalleModificado.ID_TIPO = TipoEstudioSelectedDetalle;
                    DetalleModificado.INICIO_FEC = FechaInicioSolicitudEstudioSelectedDetalle;
                    DetalleModificado.RESULTADO = ResultadoSolicitudEstudioSelectedDetalle;
                    DetalleModificado.SOLICITUD_FEC = FechaSolicitudEstudioSelectedDetalle;
                    DetalleModificado.TERMINO_FEC = FechaFinSolicitudEstudioSelectedDetalle;
                    DetalleModificado.TIPO_MEDIA = null;//este campo no existe en la vista que esta en el sistema d Justicia
                    DetalleModificado.ID_DETALLE = SelectedEstudioPersonalidadDetalle.ID_DETALLE;
                    if (new cPersonalidadDetalle().Actualizar(DetalleModificado))
                    {
                        PopUpsViewModels.ClosePopUp(PopUpsViewModels.TipoPopUp.AGREGAR_ESTUDIO_PERSONALIDAD_DETALLE);
                        SelectedEstudioPersonalidadDetalle = null;//No se manajaran por el momento los archivos que se suben
                        new Dialogos().ConfirmacionDialogo("EXITO!", "Se ha actualizado el desarrollo del estudio de personalidad con éxito.");
                        ListEstudiosPersonalidadDetalle = new ObservableCollection<PERSONALIDAD_DETALLE>(new cPersonalidadDetalle().GetData(x => x.ID_ESTUDIO == SelectedEstudioPersonalidad.ID_ESTUDIO && x.ID_IMPUTADO == SelectIngreso.ID_IMPUTADO).OrderBy(y => y.ID_DETALLE));
                        return;
                    }

                    else
                    {
                        new Dialogos().ConfirmacionDialogo("ERROR!", "Surgió un error al actualizar el desarrollo del estudio de personalidad.");
                        return;
                    }
                }
            }

            catch (Exception exc)
            {
                throw exc;
            }
        }

        public void Guardar()
        {
            try
            {
                if (SelectIngreso != null)
                {
                    if (IdEstudioPersonalidadPdre == null)
                    {
                        PERSONALIDAD NuevoEstudioPersonalidad = new PERSONALIDAD();
                        NuevoEstudioPersonalidad.ID_ANIO = SelectIngreso.ID_ANIO;
                        NuevoEstudioPersonalidad.ID_CENTRO = SelectIngreso.ID_CENTRO;
                        NuevoEstudioPersonalidad.ID_IMPUTADO = SelectIngreso.ID_IMPUTADO;
                        NuevoEstudioPersonalidad.ID_INGRESO = SelectIngreso.ID_INGRESO;
                        NuevoEstudioPersonalidad.ID_MOTIVO = MotivoEstudioPadreSelected;
                        NuevoEstudioPersonalidad.ID_SITUACION = SituacionEstudioPadreSelected;
                        NuevoEstudioPersonalidad.INICIO_FEC = FechaInicio;
                        NuevoEstudioPersonalidad.SOLICITADO = SolicitadoPor;
                        NuevoEstudioPersonalidad.SOLICITUD_FEC = FechaSolicitud;
                        NuevoEstudioPersonalidad.TERMINO_FEC = FechaFin;
                        if (new cEstudioPersonalidad().Insertar(NuevoEstudioPersonalidad))
                        {
                            new Dialogos().ConfirmacionDialogo("EXITO!", "Se ha registrado el estudio de personalidad con exito.");
                            ListEstudiosPersonalidad = new ObservableCollection<PERSONALIDAD>(new cEstudioPersonalidad().ObtenerTodos(SelectIngreso.ID_IMPUTADO, SelectIngreso.ID_INGRESO));//recarga los estudios de personalidad una vez ingresado el nuevo estudio.
                        }
                        else
                            new Dialogos().ConfirmacionDialogo("ERROR!", "Surgió un error al guardar el estudio de personalidad.");
                    }

                    else
                    {
                        PERSONALIDAD EstudioPersonalidadRecienEditado = new PERSONALIDAD()
                        {
                            ID_ANIO = SelectIngreso.ID_ANIO,
                            ID_IMPUTADO = SelectIngreso.ID_IMPUTADO,
                            ID_SITUACION = SituacionEstudioPadreSelected,
                            ID_CENTRO = SelectIngreso.ID_CENTRO,
                            ID_ESTUDIO = IdEstudioPersonalidadPdre.ID_ESTUDIO,
                            ID_INGRESO = SelectIngreso.ID_INGRESO,
                            ID_MOTIVO = MotivoEstudioPadreSelected,
                            INICIO_FEC = FechaInicio,
                            NUM_OFICIO = IdEstudioPersonalidadPdre.NUM_OFICIO,
                            PROG_NOMBRE = IdEstudioPersonalidadPdre.PROG_NOMBRE,
                            RESULT_ESTUDIO = IdEstudioPersonalidadPdre.RESULT_ESTUDIO,
                            SOLICITADO = SolicitadoPor,
                            SOLICITUD_FEC = FechaSolicitud,
                            TERMINO_FEC = FechaFin
                        };

                        if (new cEstudioPersonalidad().Actualizar(EstudioPersonalidadRecienEditado, (short)enumMensajeTipo.DEFINIDA_FECHA_INICIO_PERSONALIDAD))
                        {
                            new Dialogos().ConfirmacionDialogo("EXITO!", "Se ha actualizado el estudio de personalidad con exito.");
                            SelectedEstudioPersonalidad = null;//se limpia el estudio que se seleccion antes para que no se traslape
                            ListEstudiosPersonalidad = new ObservableCollection<PERSONALIDAD>(new cEstudioPersonalidad().ObtenerTodos(SelectIngreso.ID_IMPUTADO, SelectIngreso.ID_INGRESO));//recarga los estudios de personalidad una vez ingresado el nuevo estudio.
                            return;
                        }
                        else
                        {
                            new Dialogos().ConfirmacionDialogo("ERROR!", "Surgió un error al actualizar el estudio de personalidad.");
                            return;
                        }
                    }
                }

                else
                    return;
            }

            catch (Exception exc)
            {
                new Dialogos().ConfirmacionDialogo("ERROR!", "Surgió un error al actualizar el estudio de personalidad.");
                return;
            }
        }

        private void ProcesaEstudioExistente()
        {
            try
            {
                if (IdEstudioPersonalidadPdre != null)
                {
                    MotivoEstudioPadreSelected = IdEstudioPersonalidadPdre.ID_MOTIVO;
                    SituacionEstudioPadreSelected = IdEstudioPersonalidadPdre.ID_SITUACION ?? -1;
                    FechaInicio = IdEstudioPersonalidadPdre.INICIO_FEC;
                    SolicitadoPor = IdEstudioPersonalidadPdre.SOLICITADO;
                    FechaSolicitud = IdEstudioPersonalidadPdre.SOLICITUD_FEC;
                    FechaFin = IdEstudioPersonalidadPdre.TERMINO_FEC;
                }

                else
                    return;
            }

            catch (Exception exc)
            {
                throw exc;
            }
        }

        private void PrepararListasEstudioPersonalidadPadre()
        {
            if (ListPersonalidadSituaciones == null)
            {
                ListPersonalidadSituaciones = new ObservableCollection<PERSONALIDAD_SITUACION>(new cPersonalidadSituacion().GetData().OrderBy(x => x.DESCR));
                Application.Current.Dispatcher.Invoke((Action)(delegate
                    {
                        ListPersonalidadSituaciones.Insert(0, (new PERSONALIDAD_SITUACION() { ID_SITUACION = -1, DESCR = "SELECCIONE" }));
                    }));
            };

            if (ListPersonalidadMotivo == null)
            {
                ListPersonalidadMotivo = new ObservableCollection<PERSONALIDAD_MOTIVO>(new cPersonalidadMotivo().GetData().OrderBy(x => x.DESCR));
                Application.Current.Dispatcher.Invoke((Action)(delegate
                {
                    ListPersonalidadMotivo.Insert(0, (new PERSONALIDAD_MOTIVO() { ID_MOTIVO = -1, DESCR = "SELECCIONE" }));
                }));
            };

            return;
        }

        private void PrepararListasEstudioPersonalidadDetalle()
        {
            if (ListTipoEstudio == null)
            {
                ListTipoEstudio = new ObservableCollection<PERSONALIDAD_TIPO_ESTUDIO>(new cPersonalidadTipoEstudio().GetData().OrderBy(x => x.ID_TIPO));
                Application.Current.Dispatcher.Invoke((Action)(delegate
                {
                    ListTipoEstudio.Insert(0, (new PERSONALIDAD_TIPO_ESTUDIO() { ID_TIPO = -1, DESCR = "SELECCIONE" }));
                }));
            };

            if (ListEstatusEstudio == null)
            {
                ListEstatusEstudio = new ObservableCollection<PERSONALIDAD_ESTATUS>(new cPersonalidadEstatus().GetData().OrderBy(x => x.ID_ESTATUS));
                Application.Current.Dispatcher.Invoke((Action)(delegate
                {
                    ListEstatusEstudio.Insert(0, (new PERSONALIDAD_ESTATUS() { ID_ESTATUS = -1, DESCR = "SELECCIONE" }));
                }));
            };

            return;
        }

        private void InicializaListasCancelacionEstudios()
        {
            try
            {
                LstIncidenteTipo = new ObservableCollection<INCIDENTE_TIPO>(new cIncidenteTipo().ObtenerTodas().OrderBy(x => x.DESCR));
                Application.Current.Dispatcher.Invoke((Action)(delegate
                {
                    LstIncidenteTipo.Insert(0, (new INCIDENTE_TIPO() { ID_INCIDENTE_TIPO = -1, DESCR = "SELECCIONE" }));
                }));
            }
            catch (Exception exc)
            {
                throw;
            }
        }

        private void ProcesaEstudiosByImputado(int IdImputado, short IdIngreso, short IdCentro, short IdAnio)
        {
            try
            {
                if (ListEstudiosPersonalidad == null)
                    ListEstudiosPersonalidad = new ObservableCollection<PERSONALIDAD>();

                if (ListEstudiosPersonalidadDetalle == null)
                    ListEstudiosPersonalidadDetalle = new ObservableCollection<PERSONALIDAD_DETALLE>();

                ListEstudiosPersonalidadDetalle.Clear();
                ListEstudiosPersonalidad.Clear();
                ListEstudiosPersonalidad = new ObservableCollection<PERSONALIDAD>(new cEstudioPersonalidad().GetData(x => x.ID_IMPUTADO == IdImputado && x.ID_INGRESO == IdIngreso && x.ID_CENTRO == IdCentro && x.ID_ANIO == IdAnio));
            }
            catch (Exception exc)
            {
                throw;
            }
        }

        private void ProcesaDetalleEstudios(short IdEstudio, int IdImp, short IdCentro, short IdIngreso, short IdAnio)
        {
            try
            {
                if (ListEstudiosPersonalidadDetalle == null)
                    ListEstudiosPersonalidadDetalle = new ObservableCollection<PERSONALIDAD_DETALLE>();

                ListEstudiosPersonalidadDetalle.Clear();
                ListEstudiosPersonalidadDetalle = new ObservableCollection<PERSONALIDAD_DETALLE>(new cPersonalidadDetalle().GetData(x => x.ID_ESTUDIO == IdEstudio && x.ID_IMPUTADO == IdImp && x.ID_CENTRO == IdCentro &&
                    x.ID_INGRESO == IdIngreso && x.ID_ANIO == IdAnio));
            }
            catch (Exception exc)
            {
                throw;
            }
        }


        private void Mensaje(string resultado, string name)
        {
            try
            {
                StaticSourcesViewModel.Mensaje(name, resultado, resultado.ToLower().Contains("correctamente") ? StaticSourcesViewModel.enumTipoMensaje.MENSAJE_CORRECTO : StaticSourcesViewModel.enumTipoMensaje.MESNAJE_ADVERTENCIA);
            }
            catch (Exception ex)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al dar mensaje", ex);
            }
        }

        #region Cierre de Estudios de Personalidad
        private void InicializaCierreEstudiosPersonalidad(CierreEstudiosPersonalidadView obj = null)
        {
            try
            {
                PermisosCierreEstudiosPersonalidad();
                LstEstudiosCerrdaos = new ObservableCollection<PERSONALIDAD>();
                CargaEstudiosCerrados();
                DateTime f1, f2;
                if (FechaInicioBusquedaEstudiosT.HasValue)
                    f1 = FechaInicioBusquedaEstudiosT.Value;
                else
                    f1 = Fechas.GetFechaDateServer;

                if (FechaFinBusquedaEstudiosT.HasValue)
                    f2 = FechaFinBusquedaEstudiosT.Value;
                else
                    f2 = Fechas.GetFechaDateServer;

                f1 = new DateTime(f1.Year, f1.Month, f1.Day);
                f2 = new DateTime(f2.Year, f2.Month, f2.Day);

                //var _dato = new cEstudioPersonalidad().BuscarEstudiosTPersonalidadTerminados(f1, f2, NoOficioBusquedaEstudiosT, GlobalVar.gCentro);
                //if (_dato != null && _dato.Any())
                //    foreach (var item in _dato)
                //        LstEstudiosCerrdaos.Add(item);

                var _dato = new cEstudioPersonalidad().BuscarEstudiosTPersonalidadTerminados(f1, f2, NoOficioBusquedaEstudiosT, GlobalVar.gCentro);
                if (_dato != null && _dato.Any())
                {
                    foreach (var item in _dato)
                    {
                        item.RESULT_ESTUDIO = string.Empty;
                        LstEstudiosCerrdaos.Add(item);
                    }
                }
            }
            catch (Exception exc)
            {
                throw;
            }
        }

        private void InicializaListaEstudiosPersonalidad(EstudioPersonalidadView obj = null)
        {
            try
            {
                CargaEstudiosPersonalidad();
            }
            catch (Exception exc)
            {
                throw;
            }
        }

        private async void CargaEstudiosCerrados()
        {
            try
            {
                MenuGuardarEnabled = true;
                if (StaticSourcesViewModel.SourceChanged)
                {
                    var respuesta = await new Dialogos().ConfirmarEliminar("Advertencia", "Hay cambios sin guardar,¿Seguro que desea salir sin guardar?");
                    if (respuesta == 1)
                    {
                        ((System.Windows.Controls.ContentControl)PopUpsViewModels.MainWindow.FindName("contentControl")).Content = new CierreEstudiosPersonalidadView();
                        StaticSourcesViewModel.SourceChanged = false;
                    }
                }
                else
                {
                    ((System.Windows.Controls.ContentControl)PopUpsViewModels.MainWindow.FindName("contentControl")).Content = new CierreEstudiosPersonalidadView();
                    StaticSourcesViewModel.SourceChanged = false;
                }
            }
            catch (Exception exc)
            {
                throw;
            }
        }

        private async void CargaEstudiosPersonalidad()
        {
            try
            {
                if (StaticSourcesViewModel.SourceChanged)
                {
                    var respuesta = await new Dialogos().ConfirmarEliminar("Advertencia", "Hay cambios sin guardar,¿Seguro que desea salir sin guardar?");
                    if (respuesta == 1)
                    {
                        ((System.Windows.Controls.ContentControl)PopUpsViewModels.MainWindow.FindName("contentControl")).Content = new EstudioPersonalidadView();
                        StaticSourcesViewModel.SourceChanged = false;
                    }
                }
                else
                {
                    ((System.Windows.Controls.ContentControl)PopUpsViewModels.MainWindow.FindName("contentControl")).Content = new EstudioPersonalidadView();
                    StaticSourcesViewModel.SourceChanged = false;
                }
            }
            catch (Exception exc)
            {
                throw;
            }
        }
        #endregion

        #region Cierre Estudios
        private bool ValidaCierreEstudios()
        {
            try
            {
                if (LstEstudiosCerrdaos == null)
                {
                    new Dialogos().ConfirmacionDialogo("Validación!", "No hay estudios de personalidad por cerrar.");
                    return false;
                }

                if (!LstEstudiosCerrdaos.Any())
                {
                    new Dialogos().ConfirmacionDialogo("Validación!", "No hay estudios de personalidad por cerrar.");
                    return false;
                }

                return true;
            }
            catch (Exception exc)
            {
                throw;
            }
        }

        private void MuestraFormatoCierreEstudiosPersonalidad(List<PERSONALIDAD> Entity)
        {
            try
            {
                if (Entity != null && Entity.Any())
                {
                    var View = new ReportesView();
                    #region Iniciliza el entorno para mostrar el reporte al usuario
                    PopUpsViewModels.ShowPopUp(this, PopUpsViewModels.TipoPopUp.OSCURECER_FONDO);
                    View.Owner = PopUpsViewModels.MainWindow;
                    View.Closed += (s, e) => { PopUpsViewModels.ClosePopUp(PopUpsViewModels.TipoPopUp.OSCURECER_FONDO); };

                    #endregion



                    string NombreCentro = string.Empty;
                    string NombreMunicipio = string.Empty;
                    var Centro = new cCentro().GetData(c => c.ID_CENTRO == GlobalVar.gCentro).FirstOrDefault();
                    if (Centro != null)
                    {
                        NombreCentro = !string.IsNullOrEmpty(Centro.DESCR) ? Centro.DESCR.Trim() : string.Empty;
                        NombreMunicipio = string.Format("{0} {1}", Centro.ID_MUNICIPIO.HasValue ? !string.IsNullOrEmpty(Centro.MUNICIPIO.MUNICIPIO1) ? Centro.MUNICIPIO.MUNICIPIO1.Trim() : string.Empty : string.Empty,
                            Centro.ID_MUNICIPIO.HasValue ? Centro.MUNICIPIO.ENTIDAD != null ? !string.IsNullOrEmpty(Centro.MUNICIPIO.ENTIDAD.DESCR) ? Centro.MUNICIPIO.ENTIDAD.DESCR.Trim() : string.Empty : string.Empty : string.Empty);
                    };

                    string ListaFavorable = string.Empty;
                    string ListaAplazados = string.Empty;
                    short _dummy = 0;
                    short _dummy2 = 0;
                    string _alia = string.Empty;
                    foreach (var item in Entity)
                    {
                        _alia = string.Empty;
                        if (item.RESULT_ESTUDIO == "S")
                        {
                            _dummy++;
                            var _aliasImputado = new cApodo().ObtenerTodosXImputado(item.ID_CENTRO, item.ID_ANIO, item.ID_IMPUTADO);
                            if (_aliasImputado != null && _aliasImputado.Any())
                                foreach (var item2 in _aliasImputado)
                                    _alia += string.Format("(a) {0}", !string.IsNullOrEmpty(item2.APODO1) ? item2.APODO1.Trim() : string.Empty);

                            ListaFavorable += _dummy + ". " + string.Format("{0} {1} {2} {3}", item.INGRESO != null ? item.INGRESO.IMPUTADO != null ? !string.IsNullOrEmpty(item.INGRESO.IMPUTADO.NOMBRE) ? item.INGRESO.IMPUTADO.NOMBRE.Trim() : string.Empty : string.Empty : string.Empty,
                                                            item.INGRESO != null ? item.INGRESO.IMPUTADO != null ? !string.IsNullOrEmpty(item.INGRESO.IMPUTADO.PATERNO) ? item.INGRESO.IMPUTADO.PATERNO.Trim() : string.Empty : string.Empty : string.Empty,
                                                            item.INGRESO != null ? item.INGRESO.IMPUTADO != null ? !string.IsNullOrEmpty(item.INGRESO.IMPUTADO.MATERNO) ? item.INGRESO.IMPUTADO.MATERNO.Trim() : string.Empty : string.Empty : string.Empty,
                                                            !string.IsNullOrEmpty(_alia) ? _alia.Trim() : string.Empty) + "\n";
                        }
                        else
                        {
                            _dummy2++;
                            var _aliasImputado = new cApodo().ObtenerTodosXImputado(item.ID_CENTRO, item.ID_ANIO, item.ID_IMPUTADO);
                            if (_aliasImputado != null && _aliasImputado.Any())
                                foreach (var item2 in _aliasImputado)
                                    _alia += string.Format("(a) {0}", !string.IsNullOrEmpty(item2.APODO1) ? item2.APODO1.Trim() : string.Empty);

                            ListaAplazados += _dummy2 + ". " + string.Format("{0} {1} {2} {3}", item.INGRESO != null ? item.INGRESO.IMPUTADO != null ? !string.IsNullOrEmpty(item.INGRESO.IMPUTADO.NOMBRE) ? item.INGRESO.IMPUTADO.NOMBRE.Trim() : string.Empty : string.Empty : string.Empty,
                                                            item.INGRESO != null ? item.INGRESO.IMPUTADO != null ? !string.IsNullOrEmpty(item.INGRESO.IMPUTADO.PATERNO) ? item.INGRESO.IMPUTADO.PATERNO.Trim() : string.Empty : string.Empty : string.Empty,
                                                            item.INGRESO != null ? item.INGRESO.IMPUTADO != null ? !string.IsNullOrEmpty(item.INGRESO.IMPUTADO.MATERNO) ? item.INGRESO.IMPUTADO.MATERNO.Trim() : string.Empty : string.Empty : string.Empty,
                                                            !string.IsNullOrEmpty(_alia) ? _alia.Trim() : string.Empty) + "\n";

                        };
                    };

                    string NombreJ = string.Empty;
                    var NombreJuridico = new cDepartamentosAcceso().GetData(x => x.ID_DEPARTAMENTO == 30).FirstOrDefault();
                    if (NombreJuridico != null)
                        NombreJ = NombreJuridico.USUARIO != null ? NombreJuridico.USUARIO.EMPLEADO != null ? NombreJuridico.USUARIO.EMPLEADO.PERSONA != null ?
                            string.Format("{0} {1} {2} ",
                            !string.IsNullOrEmpty(NombreJuridico.USUARIO.EMPLEADO.PERSONA.NOMBRE) ? NombreJuridico.USUARIO.EMPLEADO.PERSONA.NOMBRE.Trim() : string.Empty,
                            !string.IsNullOrEmpty(NombreJuridico.USUARIO.EMPLEADO.PERSONA.PATERNO) ? NombreJuridico.USUARIO.EMPLEADO.PERSONA.PATERNO.Trim() : string.Empty,
                            !string.IsNullOrEmpty(NombreJuridico.USUARIO.EMPLEADO.PERSONA.MATERNO) ? NombreJuridico.USUARIO.EMPLEADO.PERSONA.MATERNO.Trim() : string.Empty) : string.Empty : string.Empty : string.Empty;

                    string _NombreAreasTecnicas = string.Empty;
                    var NombreAreasTecnicas = new cDepartamentosAcceso().GetData(x => x.ID_DEPARTAMENTO == 1).FirstOrDefault();
                    if (NombreAreasTecnicas != null)
                        _NombreAreasTecnicas = NombreAreasTecnicas.USUARIO != null ? NombreAreasTecnicas.USUARIO.EMPLEADO != null ? NombreAreasTecnicas.USUARIO.EMPLEADO.PERSONA != null ?
                            string.Format("{0} {1} {2} ",
                            !string.IsNullOrEmpty(NombreAreasTecnicas.USUARIO.EMPLEADO.PERSONA.NOMBRE) ? NombreAreasTecnicas.USUARIO.EMPLEADO.PERSONA.NOMBRE.Trim() : string.Empty,
                            !string.IsNullOrEmpty(NombreAreasTecnicas.USUARIO.EMPLEADO.PERSONA.PATERNO) ? NombreAreasTecnicas.USUARIO.EMPLEADO.PERSONA.PATERNO.Trim() : string.Empty,
                            !string.IsNullOrEmpty(NombreAreasTecnicas.USUARIO.EMPLEADO.PERSONA.MATERNO) ? NombreAreasTecnicas.USUARIO.EMPLEADO.PERSONA.MATERNO.Trim() : string.Empty) : string.Empty : string.Empty : string.Empty;


                    var _referencia = Entity.FirstOrDefault();
                    string _folioReferencia = _referencia.NUM_OFICIO1;
                    string CuerpoDescripcion = string.Concat("  En atención al oficio " + _referencia.NUM_OFICIO + " de fecha " + (_referencia.SOLICITUD_FEC.HasValue ? Fechas.fechaLetra(_referencia.SOLICITUD_FEC.Value, false) : string.Empty) + " remito a usted " + Entity.Count + " estudios de personalidad del fuero común con su respectivo dictamen, para el trámite de beneficio que corresponda, los cuales comprenden de las semanas ");
                    string Fecha = string.Format("{0} a {1} ", NombreMunicipio, Fechas.fechaLetra(Fechas.GetFechaDateServer, false));
                    var _dato = new cFormatoRemisionEstudiosPersonalidad()
                    {
                        Descripcion = CuerpoDescripcion,
                        Fecha = Fecha,
                        InternosAplazados = ListaAplazados,
                        InternosFavorables = ListaFavorable,
                        NombreCereso = NombreCentro,
                        NombreEntidad = NombreMunicipio,
                        NombreJefeAreasTecnicas = _NombreAreasTecnicas,
                        NombreJefeJuridico = NombreJ,
                        Generico1 = _folioReferencia
                    };


                    cEncabezado Encabezado = new cEncabezado();
                    Encabezado.TituloUno = "SECRETARÍA DE SEGURIDAD PÚBLICA";
                    Encabezado.TituloDos = Parametro.ENCABEZADO2;
                    Encabezado.ImagenIzquierda = Parametro.REPORTE_LOGO2;
                    Encabezado.NombreReporte = NombreCentro;
                    Encabezado.ImagenFondo = Parametro.REPORTE_LOGO_ISO;
                    Encabezado.ImagenDerecha = Parametro.LOGO_ESTADO_BC;
                    Encabezado.PieUno = Parametro.DESCR_ISO_1.Replace(";", ";\n");

                    #region Inicializacion de reporte
                    View.Report.LocalReport.ReportPath = "Reportes/rFormatoRemisionEstudiosPersonalidad.rdlc";
                    View.Report.LocalReport.DataSources.Clear();
                    #endregion


                    #region Definicion d origenes de datos

                    var ds2 = new List<cEncabezado>();
                    ds2.Add(Encabezado);
                    Microsoft.Reporting.WinForms.ReportDataSource rds2 = new Microsoft.Reporting.WinForms.ReportDataSource();
                    rds2.Name = "DataSet1";
                    rds2.Value = ds2;
                    View.Report.LocalReport.DataSources.Add(rds2);


                    var ds1 = new List<cFormatoRemisionEstudiosPersonalidad>();
                    Microsoft.Reporting.WinForms.ReportDataSource rds1 = new Microsoft.Reporting.WinForms.ReportDataSource();
                    ds1.Add(_dato);
                    rds1.Name = "DataSet2";
                    rds1.Value = ds1;
                    View.Report.LocalReport.DataSources.Add(rds1);

                    #endregion

                    View.Report.RefreshReport();
                    byte[] renderedBytes;

                    Microsoft.Reporting.WinForms.Warning[] warnings;
                    string[] streamids;
                    string mimeType;
                    string encoding;
                    string extension;

                    renderedBytes = View.Report.LocalReport.Render("WORD", null, out mimeType, out encoding, out extension, out streamids, out warnings);
                    //var disponibles = View.Report.LocalReport.ListRenderingExtensions(); ME INDICA CUALES SON LAS EXTENSIONES QUE TENGO DISPONIBLES PARA RENDERIZAR LOS REPORTES
                    var fileNamepdf = System.IO.Path.GetTempPath() + System.IO.Path.GetRandomFileName().Split('.')[0] + ".doc";
                    System.IO.File.WriteAllBytes(fileNamepdf, renderedBytes);
                    renderedBytes = System.IO.File.ReadAllBytes(fileNamepdf);
                    var tc = new TextControlView();
                    PopUpsViewModels.ClosePopUp(PopUpsViewModels.TipoPopUp.OSCURECER_FONDO);
                    tc.editor.Loaded += (s, e) =>
                    {
                        try
                        {
                            tc.editor.ViewMode = TXTextControl.ViewMode.PageView;
                            tc.editor.EditMode = TXTextControl.EditMode.ReadAndSelect;
                            tc.editor.Load(renderedBytes, TXTextControl.BinaryStreamType.MSWord);//ESTE ES EL FORMATO CON MAYOR COMPATIBILIDAD CON RESPECTO AL MANEJO DE TEXTO 
                        }
                        catch (Exception ex)
                        {
                            StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al mostrar el documento", ex);
                        }
                    };

                    PopUpsViewModels.ClosePopUp(PopUpsViewModels.TipoPopUp.OSCURECER_FONDO);
                    tc.Owner = PopUpsViewModels.MainWindow;
                    tc.Closed += (s, e) => { PopUpsViewModels.ClosePopUp(PopUpsViewModels.TipoPopUp.OSCURECER_FONDO); };
                    PopUpsViewModels.ShowPopUp(this, PopUpsViewModels.TipoPopUp.OSCURECER_FONDO);
                    tc.Show();
                }
            }
            catch (Exception exc)
            {
                throw;
            }
        }

        #endregion

        #region [REPORTE]
        private void Reporte()
        {
            try
            {
                if (SelectedEstudioCerrado != null && SelectedEstudioCerrado.INGRESO != null && SelectedEstudioCerrado.INGRESO.IMPUTADO != null)
                {
                    ReporteIngreso reporte = new ReporteIngreso();
                    reporte.Nombre = string.Format("{0} {1} {2}",
                        !string.IsNullOrEmpty(SelectedEstudioCerrado.INGRESO.IMPUTADO.NOMBRE) ? SelectedEstudioCerrado.INGRESO.IMPUTADO.NOMBRE.Trim() : string.Empty,
                        !string.IsNullOrEmpty(SelectedEstudioCerrado.INGRESO.IMPUTADO.PATERNO) ? SelectedEstudioCerrado.INGRESO.IMPUTADO.PATERNO.Trim() : string.Empty,
                        !string.IsNullOrEmpty(SelectedEstudioCerrado.INGRESO.IMPUTADO.MATERNO) ? SelectedEstudioCerrado.INGRESO.IMPUTADO.MATERNO.Trim() : string.Empty);

                    reporte.Alias = " ";
                    if (SelectedEstudioCerrado.INGRESO.IMPUTADO.ALIAS != null && SelectedEstudioCerrado.INGRESO.IMPUTADO.ALIAS.Any())
                    {
                        string alias = string.Empty;
                        foreach (var a in SelectedEstudioCerrado.INGRESO.IMPUTADO.ALIAS)
                        {
                            if (!string.IsNullOrEmpty(alias))
                                alias = string.Format("{0},", alias);
                            alias = alias + string.Format("{0} {1} {2}", !string.IsNullOrEmpty(a.NOMBRE) ? a.NOMBRE.Trim() : string.Empty, !string.IsNullOrEmpty(a.PATERNO) ? a.PATERNO.Trim() : string.Empty, !string.IsNullOrEmpty(a.MATERNO) ? a.MATERNO.Trim() : string.Empty);
                        }
                    }

                    reporte.Apodo = " ";
                    if (SelectedEstudioCerrado.INGRESO.IMPUTADO.APODO != null && SelectedEstudioCerrado.INGRESO.IMPUTADO.APODO.Any())
                    {
                        string apodos = string.Empty;
                        foreach (var a in SelectedEstudioCerrado.INGRESO.IMPUTADO.APODO)
                        {
                            if (!string.IsNullOrEmpty(apodos))
                                apodos = string.Format("{0}, ", apodos);
                            apodos = apodos + (!string.IsNullOrEmpty(a.APODO1) ? a.APODO1.Trim() : " ");
                        }
                    }

                    reporte.EstadoCivil = SelectedEstudioCerrado.INGRESO.ESTADO_CIVIL != null ? !string.IsNullOrEmpty(SelectedEstudioCerrado.INGRESO.ESTADO_CIVIL.DESCR) ? SelectedEstudioCerrado.INGRESO.ESTADO_CIVIL.DESCR.Trim() : " " : " ";

                    reporte.Conyugue = " ";
                    reporte.Originario = " ";
                    reporte.FecNacimiento = SelectedEstudioCerrado.INGRESO != null ? SelectedEstudioCerrado.INGRESO.IMPUTADO != null ? new Fechas().CalculaEdad(SelectedEstudioCerrado.INGRESO.IMPUTADO.NACIMIENTO_FECHA).ToString() : " " : " ";
                    reporte.Escolaridad = SelectedEstudioCerrado.INGRESO.ESCOLARIDAD != null ? !string.IsNullOrEmpty(SelectedEstudioCerrado.INGRESO.ESCOLARIDAD.DESCR) ? SelectedEstudioCerrado.INGRESO.ESCOLARIDAD.DESCR.Trim() : " " : " ";
                    reporte.DomicilioActual = " ";//string.Format("{0} {1},{2},{3},{4}", Imputado.DOMICILIO_CALLE, Imputado.DOMICILIO_NUM_EXT, Imputado.COLONIA.DESCR, Imputado.COLONIA.MUNICIPIO.MUNICIPIO1, Imputado.COLONIA.MUNICIPIO.ENTIDAD.DESCR);
                    reporte.Edad = " ";
                    reporte.TiempoBC = " ";
                    reporte.Telefono = SelectedEstudioCerrado.INGRESO.TELEFONO.HasValue ? SelectedEstudioCerrado.INGRESO.TELEFONO.Value.ToString() : " ";
                    reporte.Ocupacion = SelectedEstudioCerrado.INGRESO.OCUPACION != null ? !string.IsNullOrEmpty(SelectedEstudioCerrado.INGRESO.OCUPACION.DESCR) ? SelectedEstudioCerrado.INGRESO.OCUPACION.DESCR.Trim() : " " : " ";
                    reporte.NombreMadre = string.Format("{0} {1} {2} {3}",
                        !string.IsNullOrEmpty(SelectedEstudioCerrado.INGRESO.IMPUTADO.NOMBRE_MADRE) ? SelectedEstudioCerrado.INGRESO.IMPUTADO.NOMBRE_MADRE.Trim() : string.Empty,
                        !string.IsNullOrEmpty(SelectedEstudioCerrado.INGRESO.IMPUTADO.PATERNO_MADRE) ? SelectedEstudioCerrado.INGRESO.IMPUTADO.PATERNO_MADRE.Trim() : string.Empty,
                        !string.IsNullOrEmpty(SelectedEstudioCerrado.INGRESO.IMPUTADO.MATERNO_MADRE) ? SelectedEstudioCerrado.INGRESO.IMPUTADO.MATERNO_MADRE.Trim() : string.Empty,
                        !string.IsNullOrEmpty(SelectedEstudioCerrado.INGRESO.MADRE_FINADO) ? SelectedEstudioCerrado.INGRESO.MADRE_FINADO == "S" ? "FINADO" : string.Empty : string.Empty);

                    reporte.NombrePadre = string.Format("{0} {1} {2} {3}",
                        !string.IsNullOrEmpty(SelectedEstudioCerrado.INGRESO.IMPUTADO.NOMBRE_PADRE) ? SelectedEstudioCerrado.INGRESO.IMPUTADO.NOMBRE_PADRE.Trim() : string.Empty,
                        !string.IsNullOrEmpty(SelectedEstudioCerrado.INGRESO.IMPUTADO.PATERNO_PADRE) ? SelectedEstudioCerrado.INGRESO.IMPUTADO.PATERNO_PADRE.Trim() : string.Empty,
                        !string.IsNullOrEmpty(SelectedEstudioCerrado.INGRESO.IMPUTADO.MATERNO_PADRE) ? SelectedEstudioCerrado.INGRESO.IMPUTADO.MATERNO_PADRE.Trim() : string.Empty,
                        !string.IsNullOrEmpty(SelectedEstudioCerrado.INGRESO.PADRE_FINADO) ? SelectedEstudioCerrado.INGRESO.PADRE_FINADO == "S" ? "FINADO" : string.Empty : string.Empty);

                    reporte.DomicilioPadres = " ";

                    var v = new EditorView(reporte);
                    PopUpsViewModels.ShowPopUp(this, PopUpsViewModels.TipoPopUp.OSCURECER_FONDO);
                    v.Owner = PopUpsViewModels.MainWindow;
                    v.Closed += (s, e) => { PopUpsViewModels.ClosePopUp(PopUpsViewModels.TipoPopUp.OSCURECER_FONDO); };
                    v.Show();
                }
            }
            catch (Exception ex)
            {
                StaticSourcesViewModel.ShowMessageError("Algo paso...", "Ocurrió un error al generar reporte", ex);
            }
        }
        #endregion

        #region Cambio SelectedItem de Busqueda de Expediente
        private async void OnModelChangedSwitch(object parametro)
        {
            if (parametro != null)
            {
                switch (parametro.ToString())
                {
                    case "cambio_expediente":
                        if (SelectExpediente != null && (SelectExpediente.INGRESO == null || SelectExpediente.INGRESO.Count == 0))
                        {
                            await StaticSourcesViewModel.CargarDatosMetodoAsync(() =>
                            {
                                selectExpediente = new cImputado().Obtener(selectExpediente.ID_IMPUTADO, selectExpediente.ID_ANIO, selectExpediente.ID_CENTRO).First();
                                RaisePropertyChanged("SelectExpediente");
                            });
                            //MUESTRA LOS INGRESOS
                            if (SelectExpediente.INGRESO != null && SelectExpediente.INGRESO.Count > 0)
                            {
                                EmptyIngresoVisible = false;
                                SelectIngreso = SelectExpediente.INGRESO.OrderByDescending(o => o.ID_INGRESO).FirstOrDefault();
                            }
                            else
                                EmptyIngresoVisible = true;

                            //OBTENEMOS FOTO DE FRENTE
                            if (SelectIngreso != null)
                            {
                                if (SelectIngreso.INGRESO_BIOMETRICO.Where(w => w.ID_TIPO_BIOMETRICO == (short)enumTipoBiometrico.FOTO_FRENTE_REGISTRO && w.ID_FORMATO == (short)enumTipoFormato.FMTO_JPG).Any())
                                    ImagenImputado = SelectIngreso.INGRESO_BIOMETRICO.Where(w => w.ID_TIPO_BIOMETRICO == (short)enumTipoBiometrico.FOTO_FRENTE_REGISTRO && w.ID_FORMATO == (short)enumTipoFormato.FMTO_JPG).FirstOrDefault().BIOMETRICO;
                                else
                                    ImagenImputado = new Imagenes().getImagenPerson();
                            }
                            else
                                ImagenImputado = new Imagenes().getImagenPerson();

                        }
                        break;
                }
            }
        }
        #endregion
    }
}