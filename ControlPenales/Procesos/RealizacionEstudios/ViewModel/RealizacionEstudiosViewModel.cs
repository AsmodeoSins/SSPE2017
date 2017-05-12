using ControlPenales.BiometricoServiceReference;
using ControlPenales.Clases;
using SSP.Controlador.Catalogo.Justicia;
using SSP.Controlador.Catalogo.Justicia.Ingreso;
using SSP.Servidor;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
namespace ControlPenales
{
    partial class RealizacionEstudiosViewModel : FingerPrintScanner
    {

        #region Validacion de Roles
        private async void RealizacionEstudiosPLoad(RealizacionEstudiosPersonalidadView obj = null)
        {
            try
            {
                await StaticSourcesViewModel.CargarDatosMetodoAsync(PrepararListas);
                ConfiguraPermisos();
            }
            catch (System.Exception ex)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al cargar estudios de personalidad", ex);
            }
        }

        #region Permisos
        private void ConfiguraPermisos()
        {
            try
            {
                var permisos = new SSP.Controlador.Catalogo.Justicia.cProcesoUsuario().Obtener(enumProcesos.REALIZACION_ESTUDIOS_PERSONALIDAD.ToString(), StaticSourcesViewModel.UsuarioLogin.Username);
                foreach (var p in permisos)
                {
                    if (p.INSERTAR == 1)
                        pInsertar = true;
                    if (p.EDITAR == 1)
                        pEditar = true;
                    if (p.CONSULTAR == 1)
                        pConsultar = true;
                    if (p.IMPRIMIR == 1)
                        pImprimir = true;
                }
            }
            catch (System.Exception ex)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al configurar permisos en la pantalla", ex);
            }
        }
        #endregion

        #region EVENTOS ENTRE PESTANAS
        private void DescargaMedico(EstudioMedicoFCView View)
        {
            try
            {
                //if (EnabledMedicoFueroComun && !TabEstudioMedicoFC && IdVentanaAcual == (short)eVentanasProceso.ESTUDIO_MEDICO_FUERO_COMUN)
                //    GuardaAislado((short)eVentanasProceso.ESTUDIO_MEDICO_FUERO_COMUN);
            }
            catch (System.Exception ex)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al cargar estudios de personalidad", ex);
            }
        }

        private async void DescargaPsicologico(EstudioPsicologicoFCView View)
        {
            try
            {
                if (EnabledPsicologicoFueroComun)
                {
                    await StaticSourcesViewModel.CargarDatosMetodoAsync(PrepararListas);

                }
            }
            catch (System.Exception ex)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al cargar estudios de personalidad", ex);
            }
        }

        private async void DescargaPsiquiatrico(EstudioPsiquiatricoFCView View)
        {
            try
            {
                if (EnabledPsiquiatricoFueroComun)
                {
                    await StaticSourcesViewModel.CargarDatosMetodoAsync(PrepararListas);

                }
            }
            catch (System.Exception ex)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al cargar estudios de personalidad", ex);
            }
        }

        private async void DescargaCriminologico(EstudioCriminodiagnosticoFCView View)
        {
            try
            {
                if (EnabledCriminodiagnosticoFueroComun)
                {
                    await StaticSourcesViewModel.CargarDatosMetodoAsync(PrepararListas);

                }
            }
            catch (System.Exception ex)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al cargar estudios de personalidad", ex);
            }
        }

        private async void DescargaSocioFamiliar(EstudioSocioFamiliarFCView View)
        {
            try
            {
                if (EnabledSocioFamiliarFueroComun)
                {
                    await StaticSourcesViewModel.CargarDatosMetodoAsync(PrepararListas);

                }
            }
            catch (System.Exception ex)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al cargar estudios de personalidad", ex);
            }
        }

        private async void DescargaEducativo(EstudioEducativoCulturalDepFCView View)
        {
            try
            {
                if (EnabledEducativoFueroComun)
                {
                    await StaticSourcesViewModel.CargarDatosMetodoAsync(PrepararListas);

                }
            }
            catch (System.Exception ex)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al cargar estudios de personalidad", ex);
            }
        }

        private async void DescargaCapacitacion(EstudioCapacitacionYTrabajoPenitFCView View)
        {
            try
            {
                if (EnabledCapacitacionFueroComun)
                {
                    await StaticSourcesViewModel.CargarDatosMetodoAsync(PrepararListas);

                }
            }
            catch (System.Exception ex)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al cargar estudios de personalidad", ex);
            }
        }

        private async void DescargaSeguridad(InformeSeguridadCustodiaFCView View)
        {
            try
            {
                if (EnabledSeguridadFueroComun)
                {
                    await StaticSourcesViewModel.CargarDatosMetodoAsync(PrepararListas);

                }
            }
            catch (System.Exception ex)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al cargar estudios de personalidad", ex);
            }
        }
        #endregion

        private void InicializaPerfilUsuario()
        {
            try
            {
                if (!string.IsNullOrEmpty(FueroImputado))
                {
                    var UsuarioActual = new cUsuarioRol().GetData(x => x.ID_USUARIO.Trim() == GlobalVar.gUsr);
                    var _UltimoMovimiento = new cIngresoUbicacion().ObtenerUltimaUbicacion(SelectIngreso.ID_ANIO, SelectIngreso.ID_CENTRO, SelectIngreso.ID_IMPUTADO, SelectIngreso.ID_INGRESO);
                    var _EstudioPorValidar = new cEstudioPersonalidad().GetData(x => x.ID_INGRESO == SelectIngreso.ID_INGRESO && x.ID_IMPUTADO == SelectIngreso.ID_IMPUTADO && x.ID_CENTRO == SelectIngreso.ID_CENTRO && x.ID_ANIO == SelectIngreso.ID_ANIO).OrderByDescending(x => x.ID_ESTUDIO).FirstOrDefault();
                    System.DateTime _fechaH = Fechas.GetFechaDateServer;
                    if (UsuarioActual != null)
                    {
                        lstRoles = new List<short>();
                        if (FueroImputado == "C")
                        {
                            foreach (var item in UsuarioActual)
                            {
                                lstRoles.Add(item.ID_ROL);//se crea la configuracion de los roles que auxiliara a las transacciones individuales
                                switch (item.ID_ROL)
                                {
                                    case (short)eRolesContemplados.COMANDANCIA:
                                        if (_UltimoMovimiento.ID_AREA.HasValue)
                                        {
                                            var _detallePersonalidadDetalleVigilanciaComun = _EstudioPorValidar != null ? _EstudioPorValidar.PERSONALIDAD_DETALLE != null ? _EstudioPorValidar.PERSONALIDAD_DETALLE.Any() ? _EstudioPorValidar.PERSONALIDAD_DETALLE.FirstOrDefault(x => x.ID_TIPO == 3) : null : null : null;
                                            if (_detallePersonalidadDetalleVigilanciaComun != null)//EXISTE EL DETALLE PARA ESTA AREA TECNICA PARA ESTE ESTUDIO DE PERSONALIDAD
                                                if (_detallePersonalidadDetalleVigilanciaComun.PERSONALIDAD_DETALLE_DIAS != null && _detallePersonalidadDetalleVigilanciaComun.PERSONALIDAD_DETALLE_DIAS.Any())
                                                    if (_detallePersonalidadDetalleVigilanciaComun.PERSONALIDAD_DETALLE_DIAS.Any(x => x.FECHA_INICIO.Value.Year == _fechaH.Year && x.FECHA_INICIO.Value.Month == _fechaH.Month && x.FECHA_INICIO.Value.Day == _fechaH.Day /*&& x.FECHA_INICIO.Value.Hour == _fechaH.Hour*/))///VIGILANCIA COMUN
                                                        if (_UltimoMovimiento.ESTATUS == 1 || _UltimoMovimiento.ESTATUS == 2)
                                                            EnabledComun8 = true;
                                        };

                                        EnabledSeguridadFueroComun = true;
                                        break;

                                    case (short)eRolesContemplados.CRIMINOLOGO:
                                        if (_UltimoMovimiento.ID_AREA.HasValue)
                                        {
                                            var _detallePersonalidadDetalleCriminComun = _EstudioPorValidar != null ? _EstudioPorValidar.PERSONALIDAD_DETALLE != null ? _EstudioPorValidar.PERSONALIDAD_DETALLE.Any() ? _EstudioPorValidar.PERSONALIDAD_DETALLE.FirstOrDefault(x => x.ID_TIPO == 1) : null : null : null;
                                            if (_detallePersonalidadDetalleCriminComun != null)
                                                if (_detallePersonalidadDetalleCriminComun.PERSONALIDAD_DETALLE_DIAS != null && _detallePersonalidadDetalleCriminComun.PERSONALIDAD_DETALLE_DIAS.Any())
                                                    if (_detallePersonalidadDetalleCriminComun.PERSONALIDAD_DETALLE_DIAS.Any(x => x.FECHA_INICIO.Value.Year == _fechaH.Year && x.FECHA_INICIO.Value.Month == _fechaH.Month && x.FECHA_INICIO.Value.Day == _fechaH.Day /*&& x.FECHA_INICIO.Value.Hour == _fechaH.Hour*/))///CRIMINOLOGICO COMUN
                                                        if (_UltimoMovimiento.ESTATUS == 1 || _UltimoMovimiento.ESTATUS == 2)
                                                            EnabledComun4 = true;
                                        };

                                        EnabledCriminodiagnosticoFueroComun = true;
                                        break;

                                    case (short)eRolesContemplados.EDUCATIVO:
                                        if (_UltimoMovimiento.ID_AREA.HasValue)
                                        {
                                            var _detallePersonalidadDetalleEducativoComun = _EstudioPorValidar != null ? _EstudioPorValidar.PERSONALIDAD_DETALLE != null ? _EstudioPorValidar.PERSONALIDAD_DETALLE.Any() ? _EstudioPorValidar.PERSONALIDAD_DETALLE.FirstOrDefault(x => x.ID_TIPO == 7) : null : null : null;
                                            if (_detallePersonalidadDetalleEducativoComun != null)
                                                if (_detallePersonalidadDetalleEducativoComun.PERSONALIDAD_DETALLE_DIAS != null && _detallePersonalidadDetalleEducativoComun.PERSONALIDAD_DETALLE_DIAS.Any())
                                                    if (_detallePersonalidadDetalleEducativoComun.PERSONALIDAD_DETALLE_DIAS.Any(x => x.FECHA_INICIO.Value.Year == _fechaH.Year && x.FECHA_INICIO.Value.Month == _fechaH.Month && x.FECHA_INICIO.Value.Day == _fechaH.Day /*&& x.FECHA_INICIO.Value.Hour == _fechaH.Hour*/))///EDUCATIVO COMUN
                                                        if (_UltimoMovimiento.ESTATUS == 1 || _UltimoMovimiento.ESTATUS == 2)
                                                            EnabledComun6 = true;
                                        };

                                        EnabledEducativoFueroComun = true;
                                        break;

                                    case (short)eRolesContemplados.MEDICO:
                                        if (_UltimoMovimiento.ID_AREA.HasValue)
                                        {
                                            var _detallePersonalidadDetalleMedicoComun = _EstudioPorValidar != null ? _EstudioPorValidar.PERSONALIDAD_DETALLE != null ? _EstudioPorValidar.PERSONALIDAD_DETALLE.Any() ? _EstudioPorValidar.PERSONALIDAD_DETALLE.FirstOrDefault(x => x.ID_TIPO == 4) : null : null : null;
                                            if (_detallePersonalidadDetalleMedicoComun != null)
                                                if (_detallePersonalidadDetalleMedicoComun.PERSONALIDAD_DETALLE_DIAS != null && _detallePersonalidadDetalleMedicoComun.PERSONALIDAD_DETALLE_DIAS.Any())
                                                    if (_detallePersonalidadDetalleMedicoComun.PERSONALIDAD_DETALLE_DIAS.Any(x => x.FECHA_INICIO.Value.Year == _fechaH.Year && x.FECHA_INICIO.Value.Month == _fechaH.Month && x.FECHA_INICIO.Value.Day == _fechaH.Day /*&& x.FECHA_INICIO.Value.Hour == _fechaH.Hour*/))
                                                        if (_UltimoMovimiento.ESTATUS == 1 || _UltimoMovimiento.ESTATUS == 2)
                                                            EnabledComun1 = true;///MEDICO COMUN
                                        };

                                        EnabledMedicoFueroComun = true;
                                        break;

                                    case (short)eRolesContemplados.PROGRAMAS:
                                        if (_UltimoMovimiento.ID_AREA.HasValue)
                                        {
                                            var _detallePersonalidadDetalleProgramasComun = _EstudioPorValidar != null ? _EstudioPorValidar.PERSONALIDAD_DETALLE != null ? _EstudioPorValidar.PERSONALIDAD_DETALLE.Any() ? _EstudioPorValidar.PERSONALIDAD_DETALLE.FirstOrDefault(x => x.ID_TIPO == 8) : null : null : null;
                                            if (_detallePersonalidadDetalleProgramasComun != null)
                                                if (_detallePersonalidadDetalleProgramasComun.PERSONALIDAD_DETALLE_DIAS != null && _detallePersonalidadDetalleProgramasComun.PERSONALIDAD_DETALLE_DIAS.Any())
                                                    if (_detallePersonalidadDetalleProgramasComun.PERSONALIDAD_DETALLE_DIAS.Any(x => x.FECHA_INICIO.Value.Year == _fechaH.Year && x.FECHA_INICIO.Value.Month == _fechaH.Month && x.FECHA_INICIO.Value.Day == _fechaH.Day /*&& x.FECHA_INICIO.Value.Hour == _fechaH.Hour*/))///PROGRAMAS COMUN
                                                        if (_UltimoMovimiento.ESTATUS == 1 || _UltimoMovimiento.ESTATUS == 2)
                                                            EnabledComun7 = true;
                                        };

                                        EnabledCapacitacionFueroComun = true;
                                        break;

                                    case (short)eRolesContemplados.PSICOLOGO:
                                        if (_UltimoMovimiento.ID_AREA.HasValue)
                                        {
                                            var _detallePersonalidadDetallePsicologicoComun = _EstudioPorValidar != null ? _EstudioPorValidar.PERSONALIDAD_DETALLE != null ? _EstudioPorValidar.PERSONALIDAD_DETALLE.Any() ? _EstudioPorValidar.PERSONALIDAD_DETALLE.FirstOrDefault(x => x.ID_TIPO == 5) : null : null : null;
                                            if (_detallePersonalidadDetallePsicologicoComun != null)
                                                if (_detallePersonalidadDetallePsicologicoComun.PERSONALIDAD_DETALLE_DIAS != null && _detallePersonalidadDetallePsicologicoComun.PERSONALIDAD_DETALLE_DIAS.Any())
                                                    if (_detallePersonalidadDetallePsicologicoComun.PERSONALIDAD_DETALLE_DIAS.Any(x => x.FECHA_INICIO.Value.Year == _fechaH.Year && x.FECHA_INICIO.Value.Month == _fechaH.Month && x.FECHA_INICIO.Value.Day == _fechaH.Day /*&& x.FECHA_INICIO.Value.Hour == _fechaH.Hour*/))///PSICOLOGICO COMUN
                                                        if (_UltimoMovimiento.ESTATUS == 1 || _UltimoMovimiento.ESTATUS == 2)
                                                            EnabledComun3 = true;
                                        };

                                        EnabledPsicologicoFueroComun = true;
                                        break;

                                    case (short)eRolesContemplados.PSIQUIATRA:
                                        if (_UltimoMovimiento.ID_AREA.HasValue)
                                        {
                                            var _detallePersonalidadDetallePsiquiatraComun = _EstudioPorValidar != null ? _EstudioPorValidar.PERSONALIDAD_DETALLE != null ? _EstudioPorValidar.PERSONALIDAD_DETALLE.Any() ? _EstudioPorValidar.PERSONALIDAD_DETALLE.FirstOrDefault(x => x.ID_TIPO == 6) : null : null : null;
                                            if (_detallePersonalidadDetallePsiquiatraComun != null)
                                                if (_detallePersonalidadDetallePsiquiatraComun.PERSONALIDAD_DETALLE_DIAS != null && _detallePersonalidadDetallePsiquiatraComun.PERSONALIDAD_DETALLE_DIAS.Any())
                                                    if (_detallePersonalidadDetallePsiquiatraComun.PERSONALIDAD_DETALLE_DIAS.Any(x => x.FECHA_INICIO.Value.Year == _fechaH.Year && x.FECHA_INICIO.Value.Month == _fechaH.Month && x.FECHA_INICIO.Value.Day == _fechaH.Day /*&& x.FECHA_INICIO.Value.Hour == _fechaH.Hour*/))
                                                        if (_UltimoMovimiento.ESTATUS == 1 || _UltimoMovimiento.ESTATUS == 2)
                                                            EnabledComun2 = true;///PSIQUIATRICO COMUN
                                        };

                                        EnabledPsiquiatricoFueroComun = true;
                                        break;

                                    case (short)eRolesContemplados.TRABAJO_SOCIAL:
                                        if (_UltimoMovimiento.ID_AREA.HasValue)
                                        {
                                            var _detallePersonalidadDetalleTrabajoSocialComun = _EstudioPorValidar != null ? _EstudioPorValidar.PERSONALIDAD_DETALLE != null ? _EstudioPorValidar.PERSONALIDAD_DETALLE.Any() ? _EstudioPorValidar.PERSONALIDAD_DETALLE.FirstOrDefault(x => x.ID_TIPO == 2) : null : null : null;
                                            if (_detallePersonalidadDetalleTrabajoSocialComun != null)
                                                if (_detallePersonalidadDetalleTrabajoSocialComun.PERSONALIDAD_DETALLE_DIAS != null && _detallePersonalidadDetalleTrabajoSocialComun.PERSONALIDAD_DETALLE_DIAS.Any())
                                                    if (_detallePersonalidadDetalleTrabajoSocialComun.PERSONALIDAD_DETALLE_DIAS.Any(x => x.FECHA_INICIO.Value.Year == _fechaH.Year && x.FECHA_INICIO.Value.Month == _fechaH.Month && x.FECHA_INICIO.Value.Day == _fechaH.Day /*&& x.FECHA_INICIO.Value.Hour == _fechaH.Hour*/))///TRABAJO SOCIAL COMUN
                                                        if (_UltimoMovimiento.ESTATUS == 1 || _UltimoMovimiento.ESTATUS == 2)
                                                            EnabledComun5 = true;
                                        };

                                        EnabledSocioFamiliarFueroComun = true;
                                        break;
                                    default:
                                        break;
                                }//switch
                            }//foreach

                            if (!EnabledComun1 && !EnabledComun2 && !EnabledComun3 && !EnabledComun4 && !EnabledComun5 && !EnabledComun6 && !EnabledComun7 && !EnabledComun8)
                            {
                                new Dialogos().ConfirmacionDialogo("Validación", "Verifique la programación de fechas de estudios de personalidad");
                                return;
                            }
                        }

                        if (FueroImputado == "F")
                        {
                            //IsEnabledActaConsejoT = true; EL ACTA DE CONSEJO TECNICO INTERDISCIPLINARIO SE CONTEMPLA EN JURIDICO
                            foreach (var item in UsuarioActual)
                            {
                                lstRoles.Add(item.ID_ROL);
                                switch (item.ID_ROL)
                                {
                                    case (short)eRolesContemplados.COMANDANCIA:
                                        if (_UltimoMovimiento.ID_AREA.HasValue)
                                        {
                                            var _detallePersonalidadDetalleSeguridadFederal = _EstudioPorValidar != null ? _EstudioPorValidar.PERSONALIDAD_DETALLE != null ? _EstudioPorValidar.PERSONALIDAD_DETALLE.Any() ? _EstudioPorValidar.PERSONALIDAD_DETALLE.FirstOrDefault(x => x.ID_TIPO == 11) : null : null : null;
                                            if (_detallePersonalidadDetalleSeguridadFederal != null)
                                                if (_detallePersonalidadDetalleSeguridadFederal.PERSONALIDAD_DETALLE_DIAS != null && _detallePersonalidadDetalleSeguridadFederal.PERSONALIDAD_DETALLE_DIAS.Any())
                                                    if (_detallePersonalidadDetalleSeguridadFederal.PERSONALIDAD_DETALLE_DIAS.Any(x => x.FECHA_INICIO.Value.Year == _fechaH.Year && x.FECHA_INICIO.Value.Month == _fechaH.Month && x.FECHA_INICIO.Value.Day == _fechaH.Day))
                                                        if (_UltimoMovimiento.ESTATUS == 1 || _UltimoMovimiento.ESTATUS == 2)
                                                            EnabledFederal6 = true;
                                        };

                                        IsEnabledVigilanciaFederal = true;
                                        break;

                                    case (short)eRolesContemplados.CRIMINOLOGO:
                                        if (_UltimoMovimiento.ID_AREA.HasValue)
                                        {
                                            var _detallePersonalidadDetalleCriminologicoFederal = _EstudioPorValidar != null ? _EstudioPorValidar.PERSONALIDAD_DETALLE != null ? _EstudioPorValidar.PERSONALIDAD_DETALLE.Any() ? _EstudioPorValidar.PERSONALIDAD_DETALLE.FirstOrDefault(x => x.ID_TIPO == 9) : null : null : null;
                                            if (_detallePersonalidadDetalleCriminologicoFederal != null)
                                                if (_detallePersonalidadDetalleCriminologicoFederal.PERSONALIDAD_DETALLE_DIAS != null && _detallePersonalidadDetalleCriminologicoFederal.PERSONALIDAD_DETALLE_DIAS.Any())
                                                    if (_detallePersonalidadDetalleCriminologicoFederal.PERSONALIDAD_DETALLE_DIAS.Any(x => x.FECHA_INICIO.Value.Year == _fechaH.Year && x.FECHA_INICIO.Value.Month == _fechaH.Month && x.FECHA_INICIO.Value.Day == _fechaH.Day))
                                                        if (_UltimoMovimiento.ESTATUS == 1 || _UltimoMovimiento.ESTATUS == 2)
                                                            EnabledFederal7 = true;
                                        };

                                        IsEnabledCriminologicoFederal = true;
                                        break;
                                    case (short)eRolesContemplados.EDUCATIVO:
                                        if (_UltimoMovimiento.ID_AREA.HasValue)
                                        {
                                            var _detallePersonalidadDetalleEducativoFederal = _EstudioPorValidar != null ? _EstudioPorValidar.PERSONALIDAD_DETALLE != null ? _EstudioPorValidar.PERSONALIDAD_DETALLE.Any() ? _EstudioPorValidar.PERSONALIDAD_DETALLE.FirstOrDefault(x => x.ID_TIPO == 15) : null : null : null;
                                            if (_detallePersonalidadDetalleEducativoFederal != null)
                                                if (_detallePersonalidadDetalleEducativoFederal.PERSONALIDAD_DETALLE_DIAS != null && _detallePersonalidadDetalleEducativoFederal.PERSONALIDAD_DETALLE_DIAS.Any())
                                                    if (_detallePersonalidadDetalleEducativoFederal.PERSONALIDAD_DETALLE_DIAS.Any(x => x.FECHA_INICIO.Value.Year == _fechaH.Year && x.FECHA_INICIO.Value.Month == _fechaH.Month && x.FECHA_INICIO.Value.Day == _fechaH.Day))
                                                        if (_UltimoMovimiento.ESTATUS == 1 || _UltimoMovimiento.ESTATUS == 2)
                                                            EnabledFederal5 = true;
                                        };

                                        IsEnabledActivEducFederal = true;
                                        break;

                                    case (short)eRolesContemplados.MEDICO:
                                        if (_UltimoMovimiento.ID_AREA.HasValue)
                                        {
                                            var _detallePersonalidadDetalleMedicoFederal = _EstudioPorValidar != null ? _EstudioPorValidar.PERSONALIDAD_DETALLE != null ? _EstudioPorValidar.PERSONALIDAD_DETALLE.Any() ? _EstudioPorValidar.PERSONALIDAD_DETALLE.FirstOrDefault(x => x.ID_TIPO == 12) : null : null : null;
                                            if (_detallePersonalidadDetalleMedicoFederal != null)
                                                if (_detallePersonalidadDetalleMedicoFederal.PERSONALIDAD_DETALLE_DIAS != null && _detallePersonalidadDetalleMedicoFederal.PERSONALIDAD_DETALLE_DIAS.Any())
                                                    if (_detallePersonalidadDetalleMedicoFederal.PERSONALIDAD_DETALLE_DIAS.Any(x => x.FECHA_INICIO.Value.Year == _fechaH.Year && x.FECHA_INICIO.Value.Month == _fechaH.Month && x.FECHA_INICIO.Value.Day == _fechaH.Day))
                                                        if (_UltimoMovimiento.ESTATUS == 1 || _UltimoMovimiento.ESTATUS == 2)
                                                            EnabledFederal1 = true;
                                        };

                                        IsEnabledMedicoFederal = true;
                                        break;

                                    case (short)eRolesContemplados.PROGRAMAS:
                                        if (_UltimoMovimiento.ID_AREA.HasValue)
                                        {
                                            var _detallePersonalidadDetalleLaboralFederal = _EstudioPorValidar != null ? _EstudioPorValidar.PERSONALIDAD_DETALLE != null ? _EstudioPorValidar.PERSONALIDAD_DETALLE.Any() ? _EstudioPorValidar.PERSONALIDAD_DETALLE.FirstOrDefault(x => x.ID_TIPO == 16) : null : null : null;
                                            if (_detallePersonalidadDetalleLaboralFederal != null)
                                                if (_detallePersonalidadDetalleLaboralFederal.PERSONALIDAD_DETALLE_DIAS != null && _detallePersonalidadDetalleLaboralFederal.PERSONALIDAD_DETALLE_DIAS.Any())
                                                    if (_detallePersonalidadDetalleLaboralFederal.PERSONALIDAD_DETALLE_DIAS.Any(x => x.FECHA_INICIO.Value.Year == _fechaH.Year && x.FECHA_INICIO.Value.Month == _fechaH.Month && x.FECHA_INICIO.Value.Day == _fechaH.Day))
                                                        if (_UltimoMovimiento.ESTATUS == 1 || _UltimoMovimiento.ESTATUS == 2)
                                                            EnabledFederal4 = true;
                                        };

                                        IsEnabledActivProd = true;
                                        break;

                                    case (short)eRolesContemplados.PSICOLOGO:
                                        if (_UltimoMovimiento.ID_AREA.HasValue)
                                        {
                                            var _detallePersonalidadDetallePsicologicoFederal = _EstudioPorValidar != null ? _EstudioPorValidar.PERSONALIDAD_DETALLE != null ? _EstudioPorValidar.PERSONALIDAD_DETALLE.Any() ? _EstudioPorValidar.PERSONALIDAD_DETALLE.FirstOrDefault(x => x.ID_TIPO == 14) : null : null : null;
                                            if (_detallePersonalidadDetallePsicologicoFederal != null)
                                                if (_detallePersonalidadDetallePsicologicoFederal.PERSONALIDAD_DETALLE_DIAS != null && _detallePersonalidadDetallePsicologicoFederal.PERSONALIDAD_DETALLE_DIAS.Any())
                                                    if (_detallePersonalidadDetallePsicologicoFederal.PERSONALIDAD_DETALLE_DIAS.Any(x => x.FECHA_INICIO.Value.Year == _fechaH.Year && x.FECHA_INICIO.Value.Month == _fechaH.Month && x.FECHA_INICIO.Value.Day == _fechaH.Day))
                                                        if (_UltimoMovimiento.ESTATUS == 1 || _UltimoMovimiento.ESTATUS == 2)
                                                            EnabledFederal2 = true;
                                        };

                                        IsEnabledPsicoFederal = true;
                                        break;

                                    case (short)eRolesContemplados.TRABAJO_SOCIAL:
                                        if (_UltimoMovimiento.ID_AREA.HasValue)
                                        {
                                            var _detallePersonalidadDetalleSocialFederal = _EstudioPorValidar != null ? _EstudioPorValidar.PERSONALIDAD_DETALLE != null ? _EstudioPorValidar.PERSONALIDAD_DETALLE.Any() ? _EstudioPorValidar.PERSONALIDAD_DETALLE.FirstOrDefault(x => x.ID_TIPO == 10) : null : null : null;
                                            if (_detallePersonalidadDetalleSocialFederal != null)
                                                if (_detallePersonalidadDetalleSocialFederal.PERSONALIDAD_DETALLE_DIAS != null && _detallePersonalidadDetalleSocialFederal.PERSONALIDAD_DETALLE_DIAS.Any())
                                                    if (_detallePersonalidadDetalleSocialFederal.PERSONALIDAD_DETALLE_DIAS.Any(x => x.FECHA_INICIO.Value.Year == _fechaH.Year && x.FECHA_INICIO.Value.Month == _fechaH.Month && x.FECHA_INICIO.Value.Day == _fechaH.Day))
                                                        if (_UltimoMovimiento.ESTATUS == 1 || _UltimoMovimiento.ESTATUS == 2)
                                                            EnabledFederal3 = true;
                                        };

                                        IsTrabajoSocialEnabled = true;
                                        break;

                                    default:
                                        //no case
                                        break;
                                }//switch
                            }//foreach

                            if (!EnabledFederal1 && !EnabledFederal2 && !EnabledFederal3 && !EnabledFederal4 && !EnabledFederal5 && !EnabledFederal6 && !EnabledFederal7)
                            {
                                new Dialogos().ConfirmacionDialogo("Validación", "Verifique la programación de fechas de estudios de personalidad");
                                return;
                            }
                        }
                    }
                }
                else
                {
                    new Dialogos().ConfirmacionDialogo("Validación", "El imputado seleccionado no cuenta con fuero");
                    return;
                }
            }

            catch (System.Exception exc)
            {
                throw exc;
            }
        }
        #endregion


        private void InicializaVisitantesPorInterno()
        {
            try
            {
                LstVisitantesPorInterno = new ObservableCollection<VISITA_AUTORIZADA>();
                if (SelectIngreso != null)
                    if (SelectIngreso.VISITA_AUTORIZADA != null && SelectIngreso.VISITA_AUTORIZADA.Any())
                        foreach (var item in SelectIngreso.VISITA_AUTORIZADA)
                            LstVisitantesPorInterno.Add(item);
            }
            catch (System.Exception exc)
            {
                throw;
            }
        }


        private void RealizacionEstudiosUnLoad(RealizacionEstudiosPersonalidadView Window = null)
        {
            try
            {
                //IsOtraTarea = true;
                //PrincipalViewModel.CambiarVentanaSelecccionado += TimerStop;
            }
            catch (System.Exception ex)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al cargar la configuración principal", ex);
            }
        }

        private async void clickSwitch(System.Object obj)
        {
            switch (obj.ToString())
            {
                case "eliminar_sancion_comun":
                    if (SelectedSancionComun != null)
                        if (LstCorrectivosSeguridad != null && LstCorrectivosSeguridad.Any())
                            if (LstCorrectivosSeguridad.Remove(SelectedSancionComun))
                                SelectedSancionComun = null;

                    break;

                case "eliminar_prog_activ_edu":
                    if (SelectedActivPart != null)
                        if (LstActividadPart != null && LstActividadPart.Any())
                            if (LstActividadPart.Remove(SelectedActivPart))
                                SelectedActivPart = null;

                    break;
                #region drogas
                case "eliminar_droga_fed":
                    if (DrogaElegida != null)
                        if (LstSustToxicas != null && LstSustToxicas.Any())
                            if (LstSustToxicas.Remove(DrogaElegida))
                                DrogaElegida = null;
                    break;
                #endregion
                #region SELECCION DINAMICA DE NOMBRES
                case "seleccionar_quienes_visitan_socio_comun":
                    SelectedVisitanteInterno = null;
                    InicializaVisitantesPorInterno();
                    _ActualProcesoSeleccionVisitante = 1;
                    PopUpsViewModels.ShowPopUp(this, PopUpsViewModels.TipoPopUp.SELECCIONAR_VISITANTES);
                    break;

                case "seleccionar_quienes_visitan_socio_comun_dos":
                    SelectedVisitanteInterno = null;
                    InicializaVisitantesPorInterno();
                    _ActualProcesoSeleccionVisitante = 2;
                    PopUpsViewModels.ShowPopUp(this, PopUpsViewModels.TipoPopUp.SELECCIONAR_VISITANTES);
                    break;

                case "seleccionar_quienes_visitan_vigilancia_federal":
                    //SelectedVisitanteInterno = null;
                    //InicializaVisitantesPorInterno();
                    _ActualProcesoSeleccionVisitante = 3;
                    //PopUpsViewModels.ShowPopUp(this, PopUpsViewModels.TipoPopUp.SELECCIONAR_VISITANTES);

                    if (SelectIngreso == null)
                        return;

                    ConsultaVisitasPadron();
                    PopUpsViewModels.ShowPopUp(this, PopUpsViewModels.TipoPopUp.SELECCIONAR_VISITANTES_PERMITIDOS_MULTIPLE);
                    break;

                case "seleccionar_quienes_visitan_socio_federal_uno":
                    //SelectedVisitanteInterno = null;
                    //InicializaVisitantesPorInterno();
                    _ActualProcesoSeleccionVisitante = 4;
                    if (SelectIngreso == null)
                        return;

                    ConsultaVisitasPadron();
                    PopUpsViewModels.ShowPopUp(this, PopUpsViewModels.TipoPopUp.SELECCIONAR_VISITANTES_PERMITIDOS_MULTIPLE);
                    break;

                case "eliminar_curso_cap":
                    if (SelectedCursoCapacitacionFederal != null)
                        if (LstCursosCapacitacionFederal != null && LstCursosCapacitacionFederal.Any())
                            if (LstCursosCapacitacionFederal.Remove(SelectedCursoCapacitacionFederal))
                                SelectedCursoCapacitacionFederal = null;

                    break;

                case "agregar_curso_cap":
                    ValidacionesCursosFederales();
                    DescripcionCursoFederal = string.Empty;
                    SelectedFechaInicioCursoFederal = SelectedFechaFinCursoFederal = null;
                    PopUpsViewModels.ShowPopUp(this, PopUpsViewModels.TipoPopUp.CURSOS_CAPACITACION_FEDERALES);
                    break;

                case "editar_curso_cap":
                    if (SelectedCursoCapacitacionFederal != null)
                    {
                        ValidacionesCursosFederales();
                        DescripcionCursoFederal = SelectedCursoCapacitacionFederal.CURSO;
                        SelectedFechaInicioCursoFederal = SelectedCursoCapacitacionFederal.FECHA_INICIO;
                        SelectedFechaFinCursoFederal = SelectedCursoCapacitacionFederal.FECHA_TERMINO;
                        PopUpsViewModels.ShowPopUp(this, PopUpsViewModels.TipoPopUp.CURSOS_CAPACITACION_FEDERALES);
                    }
                    else
                        new Dialogos().ConfirmacionDialogo("Validación", "Seleccione un curso");
                    break;

                case "cancelar_curso_federal":
                    base.ClearRules();
                    DescripcionCursoFederal = string.Empty;
                    SelectedFechaInicioCursoFederal = SelectedFechaFinCursoFederal = null;
                    SelectedCursoCapacitacionFederal = null;
                    PopUpsViewModels.ClosePopUp(PopUpsViewModels.TipoPopUp.CURSOS_CAPACITACION_FEDERALES);
                    break;

                case "guardar_curso_federal":
                    if (string.IsNullOrEmpty(DescripcionCursoFederal))
                    {
                        new Dialogos().ConfirmacionDialogo("Validación", "Capture el curso");
                        return;
                    };

                    if (SelectedFechaInicioCursoFederal == null)
                    {
                        new Dialogos().ConfirmacionDialogo("Validación", "Capture la fecha de inicio del curso");
                        return;
                    };

                    if (SelectedFechaFinCursoFederal == null)
                    {
                        new Dialogos().ConfirmacionDialogo("Validación", "Capture la fecha de fin del curso");
                        return;
                    };

                    if (LstCursosCapacitacionFederal == null)
                        LstCursosCapacitacionFederal = new ObservableCollection<PFF_CAPACITACION_CURSO>();


                    if (LstCursosCapacitacionFederal.Remove(SelectedCursoCapacitacionFederal))
                        LstCursosCapacitacionFederal.Add(new SSP.Servidor.PFF_CAPACITACION_CURSO
                            {
                                CURSO = DescripcionCursoFederal,
                                FECHA_INICIO = SelectedFechaInicioCursoFederal,
                                FECHA_TERMINO = SelectedFechaFinCursoFederal,
                                ID_ANIO = SelectIngreso.ID_ANIO,
                                ID_CENTRO = SelectIngreso.ID_CENTRO,
                                ID_IMPUTADO = SelectIngreso.ID_IMPUTADO,
                                ID_INGRESO = SelectIngreso.ID_INGRESO
                            });
                    else
                        LstCursosCapacitacionFederal.Add(new SSP.Servidor.PFF_CAPACITACION_CURSO
                        {
                            CURSO = DescripcionCursoFederal,
                            FECHA_INICIO = SelectedFechaInicioCursoFederal,
                            FECHA_TERMINO = SelectedFechaFinCursoFederal,
                            ID_ANIO = SelectIngreso.ID_ANIO,
                            ID_CENTRO = SelectIngreso.ID_CENTRO,
                            ID_IMPUTADO = SelectIngreso.ID_IMPUTADO,
                            ID_INGRESO = SelectIngreso.ID_INGRESO
                        });

                    SelectedCursoCapacitacionFederal = null;
                    PopUpsViewModels.ClosePopUp(PopUpsViewModels.TipoPopUp.CURSOS_CAPACITACION_FEDERALES);
                    break;
                case "seleccionar_quienes_visitan_socio_federal_dos":
                    SelectedVisitanteInterno = null;
                    InicializaVisitantesPorInterno();
                    _ActualProcesoSeleccionVisitante = 5;

                    //definicion de dinamica con respecto ala persona en socio familiar federal
                    ConQuienViviraSerExternado = string.Empty;
                    NombreCalleExternado = string.Empty;
                    NoQuienViviraSerExternado = string.Empty;
                    CodPosQuienViviraSerExternado = string.Empty;
                    EPais = -1;
                    EEstado = -1;
                    EMunicipio = -1;
                    EColonia = -1;
                    PopUpsViewModels.ShowPopUp(this, PopUpsViewModels.TipoPopUp.SELECCIONAR_VISITANTES);
                    break;
                case "cancelar_visitante_seleccionado":
                    SelectedVisitanteInterno = null;
                    _ActualProcesoSeleccionVisitante = 0;
                    PopUpsViewModels.ClosePopUp(PopUpsViewModels.TipoPopUp.SELECCIONAR_VISITANTES);
                    break;

                case "agregar_visitas_seleccionadas":
                    if (LstPV != null && LstPV.Any())
                    {
                        if (_ActualProcesoSeleccionVisitante == 4)
                        {
                            QuienesVisitanOtrasPersonas = string.Empty;
                            var _Seleccionados = LstPV.Where(x => x.Seleccionado);
                            if (_Seleccionados.Any())
                                foreach (var item in _Seleccionados)
                                {
                                    if ((QuienesVisitanOtrasPersonas.Length + +(item.Nombre).Length + (item.Paterno).Length + (item.Materno).Length) < 500)
                                        QuienesVisitanOtrasPersonas += string.Format("{0} {1} {2}, ",
                                            !string.IsNullOrEmpty(item.Nombre) ? item.Nombre.Trim() : string.Empty,
                                            !string.IsNullOrEmpty(item.Paterno) ? item.Paterno.Trim() : string.Empty,
                                            !string.IsNullOrEmpty(item.Materno) ? item.Materno.Trim() : string.Empty);
                                };
                        };

                        if (_ActualProcesoSeleccionVisitante == 3)
                        {
                            QuienesRecibeVisita = string.Empty;
                            var _Seleccionados = LstPV.Where(x => x.Seleccionado == true);
                            if (_Seleccionados.Any())
                                foreach (var item in _Seleccionados)
                                {
                                    if ((QuienesRecibeVisita.Length + (item.Nombre).Length + (item.Paterno).Length + (item.Materno).Length) < 500)
                                        QuienesRecibeVisita += string.Format("{0} {1} {2}, ",
                                            !string.IsNullOrEmpty(item.Nombre) ? item.Nombre.Trim() : string.Empty,
                                            !string.IsNullOrEmpty(item.Paterno) ? item.Paterno.Trim() : string.Empty,
                                            !string.IsNullOrEmpty(item.Materno) ? item.Materno.Trim() : string.Empty);
                                };
                        };
                    };

                    PopUpsViewModels.ClosePopUp(PopUpsViewModels.TipoPopUp.SELECCIONAR_VISITANTES_PERMITIDOS_MULTIPLE);
                    break;

                case "cancelar_visitas_seleccionadas":
                    PopUpsViewModels.ClosePopUp(PopUpsViewModels.TipoPopUp.SELECCIONAR_VISITANTES_PERMITIDOS_MULTIPLE);
                    break;
                case "guardar_visitante_seleccionado":
                    if (SelectedVisitanteInterno != null)
                        switch (_ActualProcesoSeleccionVisitante)
                        {
                            case 1:
                                EspecificarQuienVisita = string.Empty;
                                EspecificarQuienVisita = string.Format("{0} {1} {2}",
                                    SelectedVisitanteInterno.PERSONA != null ? !string.IsNullOrEmpty(SelectedVisitanteInterno.PERSONA.NOMBRE) ? SelectedVisitanteInterno.PERSONA.NOMBRE.Trim() : string.Empty : string.Empty,
                                    SelectedVisitanteInterno.PERSONA != null ? !string.IsNullOrEmpty(SelectedVisitanteInterno.PERSONA.PATERNO) ? SelectedVisitanteInterno.PERSONA.PATERNO.Trim() : string.Empty : string.Empty,
                                    SelectedVisitanteInterno.PERSONA != null ? !string.IsNullOrEmpty(SelectedVisitanteInterno.PERSONA.MATERNO) ? SelectedVisitanteInterno.PERSONA.MATERNO.Trim() : string.Empty : string.Empty);
                                PopUpsViewModels.ClosePopUp(PopUpsViewModels.TipoPopUp.SELECCIONAR_VISITANTES);
                                break;
                            case 2:
                                ConQuienVivirSerExternado = string.Empty;
                                ConQuienVivirSerExternado = string.Format("{0} {1} {2} , {3} , {4}",
                                    SelectedVisitanteInterno.PERSONA != null ? !string.IsNullOrEmpty(SelectedVisitanteInterno.PERSONA.NOMBRE) ? SelectedVisitanteInterno.PERSONA.NOMBRE.Trim() : string.Empty : string.Empty,
                                    SelectedVisitanteInterno.PERSONA != null ? !string.IsNullOrEmpty(SelectedVisitanteInterno.PERSONA.PATERNO) ? SelectedVisitanteInterno.PERSONA.PATERNO.Trim() : string.Empty : string.Empty,
                                    SelectedVisitanteInterno.PERSONA != null ? !string.IsNullOrEmpty(SelectedVisitanteInterno.PERSONA.MATERNO) ? SelectedVisitanteInterno.PERSONA.MATERNO.Trim() : string.Empty : string.Empty,
                                    SelectedVisitanteInterno.TIPO_REFERENCIA != null ? !string.IsNullOrEmpty(SelectedVisitanteInterno.TIPO_REFERENCIA.DESCR) ? SelectedVisitanteInterno.TIPO_REFERENCIA.DESCR.Trim() : string.Empty : string.Empty,
                                    SelectedVisitanteInterno.PERSONA != null ? string.Concat(
                                                                                                !string.IsNullOrEmpty(SelectedVisitanteInterno.PERSONA.DOMICILIO_CALLE) ? SelectedVisitanteInterno.PERSONA.DOMICILIO_CALLE.Trim() : string.Empty, " ",
                                                                                                SelectedVisitanteInterno.PERSONA.DOMICILIO_NUM_EXT) : string.Empty
                                    );
                                PopUpsViewModels.ClosePopUp(PopUpsViewModels.TipoPopUp.SELECCIONAR_VISITANTES);
                                break;
                            case 3:
                                QuienesRecibeVisita = string.Empty;
                                QuienesRecibeVisita = string.Format("{0} {1} {2}",
                                    SelectedVisitanteInterno.PERSONA != null ? !string.IsNullOrEmpty(SelectedVisitanteInterno.PERSONA.NOMBRE) ? SelectedVisitanteInterno.PERSONA.NOMBRE.Trim() : string.Empty : string.Empty,
                                    SelectedVisitanteInterno.PERSONA != null ? !string.IsNullOrEmpty(SelectedVisitanteInterno.PERSONA.PATERNO) ? SelectedVisitanteInterno.PERSONA.PATERNO.Trim() : string.Empty : string.Empty,
                                    SelectedVisitanteInterno.PERSONA != null ? !string.IsNullOrEmpty(SelectedVisitanteInterno.PERSONA.MATERNO) ? SelectedVisitanteInterno.PERSONA.MATERNO.Trim() : string.Empty : string.Empty);
                                PopUpsViewModels.ClosePopUp(PopUpsViewModels.TipoPopUp.SELECCIONAR_VISITANTES);
                                break;
                            case 4:
                                QuienesVisitanOtrasPersonas = string.Empty;
                                QuienesVisitanOtrasPersonas = string.Format("{0} {1} {2}",
                                    SelectedVisitanteInterno.PERSONA != null ? !string.IsNullOrEmpty(SelectedVisitanteInterno.PERSONA.NOMBRE) ? SelectedVisitanteInterno.PERSONA.NOMBRE.Trim() : string.Empty : string.Empty,
                                    SelectedVisitanteInterno.PERSONA != null ? !string.IsNullOrEmpty(SelectedVisitanteInterno.PERSONA.PATERNO) ? SelectedVisitanteInterno.PERSONA.PATERNO.Trim() : string.Empty : string.Empty,
                                    SelectedVisitanteInterno.PERSONA != null ? !string.IsNullOrEmpty(SelectedVisitanteInterno.PERSONA.MATERNO) ? SelectedVisitanteInterno.PERSONA.MATERNO.Trim() : string.Empty : string.Empty);
                                PopUpsViewModels.ClosePopUp(PopUpsViewModels.TipoPopUp.SELECCIONAR_VISITANTES);
                                break;
                            case 5:
                                ConQuienViviraSerExternado = string.Empty;
                                ConQuienViviraSerExternado = string.Format("{0} {1} {2}",
                                    SelectedVisitanteInterno.PERSONA != null ? !string.IsNullOrEmpty(SelectedVisitanteInterno.PERSONA.NOMBRE) ? SelectedVisitanteInterno.PERSONA.NOMBRE.Trim() : string.Empty : string.Empty,
                                    SelectedVisitanteInterno.PERSONA != null ? !string.IsNullOrEmpty(SelectedVisitanteInterno.PERSONA.PATERNO) ? SelectedVisitanteInterno.PERSONA.PATERNO.Trim() : string.Empty : string.Empty,
                                    SelectedVisitanteInterno.PERSONA != null ? !string.IsNullOrEmpty(SelectedVisitanteInterno.PERSONA.MATERNO) ? SelectedVisitanteInterno.PERSONA.MATERNO.Trim() : string.Empty : string.Empty);
                                NombreCalleExternado = SelectedVisitanteInterno.PERSONA != null ? !string.IsNullOrEmpty(SelectedVisitanteInterno.PERSONA.DOMICILIO_CALLE) ? SelectedVisitanteInterno.DOMICILIO_CALLE.Trim() : string.Empty : string.Empty;
                                NoQuienViviraSerExternado = SelectedVisitanteInterno.PERSONA != null ? SelectedVisitanteInterno.PERSONA.DOMICILIO_NUM_EXT.HasValue ? SelectedVisitanteInterno.PERSONA.DOMICILIO_NUM_EXT.Value.ToString() : string.Empty : string.Empty;
                                CodPosQuienViviraSerExternado = SelectedVisitanteInterno.PERSONA != null ? SelectedVisitanteInterno.PERSONA.DOMICILIO_CODIGO_POSTAL.HasValue ? SelectedVisitanteInterno.DOMICILIO_CODIGO_POSTAL.Value.ToString() : string.Empty : string.Empty;
                                EPais = SelectedVisitanteInterno.PERSONA != null ? SelectedVisitanteInterno.PERSONA.VISITANTE.PERSONA.ID_PAIS : -1;
                                EEstado = SelectedVisitanteInterno.PERSONA != null ? SelectedVisitanteInterno.PERSONA.ID_ENTIDAD : -1;
                                EMunicipio = SelectedVisitanteInterno.PERSONA != null ? SelectedVisitanteInterno.PERSONA.ID_MUNICIPIO : -1;
                                EColonia = SelectedVisitanteInterno.PERSONA != null ? SelectedVisitanteInterno.PERSONA.ID_COLONIA : -1;
                                IdParentescoQuienViviraSerExternado = SelectedVisitanteInterno.ID_PARENTESCO.HasValue ? SelectedVisitanteInterno.ID_PARENTESCO : -1;
                                PopUpsViewModels.ClosePopUp(PopUpsViewModels.TipoPopUp.SELECCIONAR_VISITANTES);
                                break;
                            default:
                                break;
                        }

                    else
                        new Dialogos().ConfirmacionDialogo("Validación", "Seleccione un visitante");
                    break;
                #endregion
                #region Huellas
                case "buscar_nip":
                    ListResultado = ListResultado ?? new List<ResultadoBusquedaBiometrico>();
                    if (string.IsNullOrEmpty(TextNipBusqueda)) return;
                    HuellaWindow.Hide();
                    PopUpsViewModels.ClosePopUp(PopUpsViewModels.TipoPopUp.OSCURECER_FONDO);
                    await StaticSourcesViewModel.CargarDatosMetodoAsync(() =>
                    {
                        try
                        {
                            //var tipo = (short)enumTipoBiometrico.FOTO_FRENTE_SEGUIMIENTO;
                            var auxiliar = new List<ResultadoBusquedaBiometrico>();
                            ListResultado = ListResultado ?? new List<ResultadoBusquedaBiometrico>();

                            if (SelectExpediente.NIP.Trim() == TextNipBusqueda.Trim())
                            {
                                var ingresobiometrico = SelectExpediente.INGRESO.OrderByDescending(o => o.ID_INGRESO).FirstOrDefault();
                                var FotoBusquedaHuella = new Imagenes().ConvertByteToBitmap(new Imagenes().getImagenPerson());
                                if (ingresobiometrico != null)
                                    if (ingresobiometrico.INGRESO_BIOMETRICO.Any())
                                        if (ingresobiometrico.INGRESO_BIOMETRICO.Where(w => w.ID_TIPO_BIOMETRICO == (short)enumTipoBiometrico.FOTO_FRENTE_SEGUIMIENTO && w.ID_FORMATO == (short)enumTipoFormato.FMTO_JPG).Any())
                                            FotoBusquedaHuella = new Imagenes().ConvertByteToBitmap(ingresobiometrico.INGRESO_BIOMETRICO.Where(w => w.ID_TIPO_BIOMETRICO == (short)enumTipoBiometrico.FOTO_FRENTE_SEGUIMIENTO && w.ID_FORMATO == (short)enumTipoFormato.FMTO_JPG).SingleOrDefault().BIOMETRICO);
                                        else
                                            if (ingresobiometrico.INGRESO_BIOMETRICO.Where(w => w.ID_TIPO_BIOMETRICO == (short)enumTipoBiometrico.FOTO_FRENTE_REGISTRO && w.ID_FORMATO == (short)enumTipoFormato.FMTO_JPG).Any())
                                                FotoBusquedaHuella = new Imagenes().ConvertByteToBitmap(ingresobiometrico.INGRESO_BIOMETRICO.Where(w => w.ID_TIPO_BIOMETRICO == (short)enumTipoBiometrico.FOTO_FRENTE_REGISTRO && w.ID_FORMATO == (short)enumTipoFormato.FMTO_JPG).SingleOrDefault().BIOMETRICO);
                                auxiliar.Add(new ResultadoBusquedaBiometrico
                                {
                                    AMaterno = SelectExpediente.MATERNO.Trim(),
                                    APaterno = SelectExpediente.PATERNO.Trim(),
                                    Expediente = SelectExpediente.ID_ANIO + "/" + SelectExpediente.ID_IMPUTADO,
                                    Foto = FotoBusquedaHuella,
                                    Imputado = SelectExpediente,
                                    NIP = !string.IsNullOrEmpty(SelectExpediente.NIP) ? SelectExpediente.NIP : string.Empty,
                                    Nombre = SelectExpediente.NOMBRE.Trim(),
                                    Persona = null
                                });
                            }
                            ListResultado = auxiliar.Any() ? auxiliar.ToList() : new List<ResultadoBusquedaBiometrico>();
                        }
                        catch (System.Exception ex)
                        {
                            StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al cargar realizar la busqueda por nip.", ex);
                        }
                    });
                    HuellaWindow.Show();
                    PopUpsViewModels.ShowPopUp(this, PopUpsViewModels.TipoPopUp.OSCURECER_FONDO);
                    break;
                case "aceptar_huella":
                    try
                    {
                        HuellaWindow.Hide();
                        PopUpsViewModels.ClosePopUp(PopUpsViewModels.TipoPopUp.OSCURECER_FONDO);
                        if (ScannerMessage.Contains("Procesando..."))
                            return;

                        CancelKeepSearching = true;
                        isKeepSearching = true;
                        _IsSucceed = true;
                        var _error_tipo = 0;
                        await StaticSourcesViewModel.CargarDatosMetodoAsync(() =>
                        {
                            try
                            {
                                if (SelectRegistro == null)
                                {
                                    _error_tipo = 1;
                                    return;
                                }

                                SelectExpediente = SelectRegistro.Imputado;
                                if (SelectExpediente.INGRESO.Count == 0)
                                {
                                    SelectExpediente = null;
                                    SelectIngreso = null;
                                    ImagenImputado = ImagenIngreso = new Imagenes().getImagenPerson();
                                }
                                if (Parametro.ESTATUS_ADMINISTRATIVO_INACT.Any(a => a.HasValue ? a.Value == SelectExpediente.INGRESO.OrderByDescending(o => o.ID_INGRESO).FirstOrDefault().ID_ESTATUS_ADMINISTRATIVO : false))
                                {
                                    _error_tipo = 2;
                                    return;
                                }
                                if (SelectExpediente.INGRESO.OrderByDescending(o => o.ID_INGRESO).FirstOrDefault().ID_UB_CENTRO.HasValue ?
                                    SelectExpediente.INGRESO.OrderByDescending(o => o.ID_INGRESO).FirstOrDefault().ID_UB_CENTRO != GlobalVar.gCentro : false)
                                {
                                    SelectExpediente = null;
                                    SelectIngreso = null;
                                    ImagenImputado = ImagenIngreso = new Imagenes().getImagenPerson();
                                    _error_tipo = 3;
                                    return;
                                }

                                SelectIngreso = SelectExpediente.INGRESO.OrderByDescending(o => o.ID_INGRESO).FirstOrDefault();
                                Application.Current.Dispatcher.Invoke((System.Action)(delegate
                                {
                                    HuellaWindow.Close();
                                    if (EstaNavegandoTabs)
                                    {
                                        if (SelectRegistro == null)
                                            RegresaTabAnterior();
                                        else
                                            TabSiguiente();
                                    };
                                    //MenuBuscarEnabled = false;
                                    //MenuGuardarEnabled = true;
                                    PopUpsViewModels.ClosePopUp(PopUpsViewModels.TipoPopUp.OSCURECER_FONDO);
                                }));
                            }
                            catch (System.Exception ex)
                            {
                                Application.Current.Dispatcher.Invoke((System.Action)(delegate
                                {
                                    //HuellaWindow.Show();
                                    PopUpsViewModels.ShowPopUp(this, PopUpsViewModels.TipoPopUp.OSCURECER_FONDO);
                                    StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al cargar datos del imputado seleccionado", ex);
                                }));
                            }
                        });

                        switch (_error_tipo)
                        {
                            case 1:
                                if (EstaNavegandoTabs)
                                    RegresaTabAnterior();

                                await new Dialogos().ConfirmacionDialogoReturn("Notificación!", "Debes seleccionar un imputado.");
                                break;
                            case 2:
                                await new Dialogos().ConfirmacionDialogoReturn("Notificación!", "No se encontró ningun ingreso activo en este imputado.");
                                break;
                            case 3:
                                await new Dialogos().ConfirmacionDialogoReturn("Notificación!", "El ingreso seleccionado no pertenece a su centro.");
                                break;
                        }
                        if (_error_tipo != 0)
                        {
                            HuellaWindow.Show();
                            PopUpsViewModels.ShowPopUp(this, PopUpsViewModels.TipoPopUp.OSCURECER_FONDO);
                            return;
                        }

                        //AQUI SE VALIDA SI PROCEDE O NO CON LA NEVEGACION ENTRE TABS
                        if (!EstaNavegandoTabs)
                            if (SelectIngreso != null)
                                InicializaEntornoRealizacionEstudiosPersonalidad();
                    }
                    catch (System.Exception ex)
                    {
                        HuellaWindow.Show();
                        PopUpsViewModels.ShowPopUp(this, PopUpsViewModels.TipoPopUp.OSCURECER_FONDO);
                        StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al cargar datos del imputado seleccionado", ex);
                    }
                    break;
                #endregion
                #region Escolaridad
                case "insertar_escolaridad":
                    if (SelectIngreso != null)
                    {
                        ObservacionesEducacion = Concluida = string.Empty;
                        IdEducativo = -1;
                        ValidacionesEscolaridad();
                        PopUpsViewModels.ShowPopUp(this, PopUpsViewModels.TipoPopUp.ESCOLARIDAD_ESTUDIO_EDUCATIVO_FUERO_COMUN);
                    }
                    break;

                case "cancelar_escolaridad":
                    ObservacionesEducacion = Concluida = string.Empty;
                    IdEducativo = -1;
                    SelectedComunicacion = null;
                    LimpiaValidacionesEscolaridad();
                    PopUpsViewModels.ClosePopUp(PopUpsViewModels.TipoPopUp.ESCOLARIDAD_ESTUDIO_EDUCATIVO_FUERO_COMUN);
                    break;

                case "editar_escolaridad":
                    if (SelectIngreso != null)
                    {
                        if (SelectedComunicacion != null)
                        {
                            ObservacionesEducacion = SelectedComunicacion.OBSERVACION;
                            Concluida = SelectedComunicacion.CONCLUIDA;
                            IdEducativo = SelectedComunicacion.ID_GRADO;
                            ValidacionesEscolaridad();
                            PopUpsViewModels.ShowPopUp(this, PopUpsViewModels.TipoPopUp.ESCOLARIDAD_ESTUDIO_EDUCATIVO_FUERO_COMUN);
                        };
                    };

                    break;
                case "eliminar_escolaridad":
                    if (SelectIngreso != null)
                        if (SelectedComunicacion != null)
                            if (LstEscolaridadesEducativo.Remove(SelectedComunicacion))
                                SelectedComunicacion = null;
                    break;
                case "guardar_escolaridad":
                    if (base.HasErrors)
                    {
                        Application.Current.Dispatcher.Invoke((System.Action)(delegate
                        {
                            (new Dialogos()).ConfirmacionDialogo("Advertencia!", base.Error);
                        }));
                        return;
                    }

                    if (SelectedComunicacion == null)
                    {
                        LstEscolaridadesEducativo.Add(new PFC_VII_ESCOLARIDAD_ANTERIOR
                            {
                                CONCLUIDA = Concluida,
                                ID_GRADO = IdEducativo,
                                OBSERVACION = ObservacionesEducacion,
                                ID_ANIO = SelectIngreso.ID_ANIO,
                                ID_CENTRO = SelectIngreso.ID_CENTRO,
                                ID_IMPUTADO = SelectIngreso.ID_IMPUTADO,
                                ID_INGRESO = SelectIngreso.ID_INGRESO,
                                INTERES = string.Empty,//No aplica en este caso
                                RENDIMIENTO = string.Empty//No aplica en este caso
                            });
                    }
                    else
                    {
                        if (LstEscolaridadesEducativo.Remove(SelectedComunicacion))
                        {
                            var _NuevoDato = new PFC_VII_ESCOLARIDAD_ANTERIOR()
                            {
                                CONCLUIDA = Concluida,
                                ID_ANIO = SelectIngreso.ID_ANIO,
                                ID_CENTRO = SelectIngreso.ID_CENTRO,
                                ID_GRADO = IdEducativo,
                                ID_IMPUTADO = SelectIngreso.ID_IMPUTADO,
                                ID_INGRESO = SelectIngreso.ID_INGRESO,
                                OBSERVACION = ObservacionesEducacion
                            };

                            LstEscolaridadesEducativo.Add(_NuevoDato);
                        };
                    }

                    SelectedComunicacion = null;
                    StaticSourcesViewModel.SourceChanged = true;//AUN NO SE HA GUARDADO, VALIDA AL USUARIO SI DESEA SALIR
                    PopUpsViewModels.ClosePopUp(PopUpsViewModels.TipoPopUp.ESCOLARIDAD_ESTUDIO_EDUCATIVO_FUERO_COMUN);
                    break;

                #endregion
                #region Actividades Educativas
                case "insertar_activ_educ":
                    if (SelectIngreso != null)
                    {
                        NombreDina2 = "Agregar Actividades Educativas";
                        IdRendimientoActivEducativo = IdInteresActivEducativo = ObservacionesActivEducativa = IdConcluidaActivEducativa = string.Empty;
                        IdNivelEducativoActiv = -1;
                        ValidacionesActividadesEducativas();
                        PopUpsViewModels.ShowPopUp(this, PopUpsViewModels.TipoPopUp.ACTIVIDADES_EDUCATIVAS_EDUCATIVO_FUERO_COMUN);
                    };
                    break;

                case "editar_activ_educ":
                    if (SelectIngreso != null)
                    {
                        NombreDina2 = "Edición Actividades Educativas";
                        if (SelectedActividadEducativa != null)
                        {
                            ObservacionesActivEducativa = SelectedActividadEducativa.OBSERVACION;
                            IdConcluidaActivEducativa = SelectedActividadEducativa.CONCLUIDA;
                            IdNivelEducativoActiv = SelectedActividadEducativa.ID_GRADO;
                            IdRendimientoActivEducativo = SelectedActividadEducativa.RENDIMIENTO;
                            IdInteresActivEducativo = SelectedActividadEducativa.INTERES;
                            ValidacionesActividadesEducativas();
                            PopUpsViewModels.ShowPopUp(this, PopUpsViewModels.TipoPopUp.ACTIVIDADES_EDUCATIVAS_EDUCATIVO_FUERO_COMUN);
                        };
                    };

                    break;

                case "eliminar_activ_educ":
                    if (SelectIngreso != null)
                        if (SelectedActividadEducativa != null)
                            if (LstActividadesEducativas != null && LstActividadesEducativas.Any())
                                if (LstActividadesEducativas.Remove(SelectedActividadEducativa))
                                    SelectedActividadEducativa = null;

                    break;

                case "cancelar_activ_educ":
                    LimpiarValidacionesActiviEducativas();
                    IdRendimientoActivEducativo = IdInteresActivEducativo = ObservacionesActivEducativa = IdConcluidaActivEducativa = string.Empty;
                    IdNivelEducativoActiv = -1;
                    PopUpsViewModels.ClosePopUp(PopUpsViewModels.TipoPopUp.ACTIVIDADES_EDUCATIVAS_EDUCATIVO_FUERO_COMUN);
                    break;

                case "guardar_activ_educ":
                    if (base.HasErrors)
                    {
                        Application.Current.Dispatcher.Invoke((System.Action)(delegate
                        {
                            (new Dialogos()).ConfirmacionDialogo("Advertencia!", base.Error);
                        }));
                        return;
                    }

                    if (SelectedActividadEducativa == null)
                    {
                        LstActividadesEducativas.Add(new PFC_VII_ESCOLARIDAD_ANTERIOR
                            {
                                CONCLUIDA = IdConcluidaActivEducativa,
                                ID_GRADO = IdNivelEducativoActiv,
                                OBSERVACION = ObservacionesActivEducativa,
                                ID_ANIO = SelectIngreso.ID_ANIO,
                                ID_CENTRO = SelectIngreso.ID_CENTRO,
                                ID_IMPUTADO = SelectIngreso.ID_IMPUTADO,
                                ID_INGRESO = SelectIngreso.ID_INGRESO,
                                INTERES = IdInteresActivEducativo,
                                RENDIMIENTO = IdRendimientoActivEducativo
                            });
                    }
                    else
                    {
                        if (LstActividadesEducativas.Remove(SelectedActividadEducativa))
                        {
                            var _NuevoDato = new PFC_VII_ESCOLARIDAD_ANTERIOR()
                            {
                                CONCLUIDA = IdConcluidaActivEducativa,
                                ID_ANIO = SelectIngreso.ID_ANIO,
                                ID_CENTRO = SelectIngreso.ID_CENTRO,
                                ID_GRADO = IdNivelEducativoActiv,
                                ID_IMPUTADO = SelectIngreso.ID_IMPUTADO,
                                ID_INGRESO = SelectIngreso.ID_INGRESO,
                                OBSERVACION = ObservacionesActivEducativa,
                                INTERES = IdInteresActivEducativo,
                                RENDIMIENTO = IdRendimientoActivEducativo
                            };

                            LstActividadesEducativas.Add(_NuevoDato);
                        };
                    }

                    StaticSourcesViewModel.SourceChanged = true;//AUN NO SE HA GUARDADO, VALIDA AL USUARIO SI DESEA SALIR
                    PopUpsViewModels.ClosePopUp(PopUpsViewModels.TipoPopUp.ACTIVIDADES_EDUCATIVAS_EDUCATIVO_FUERO_COMUN);
                    break;

                #endregion
                #region Actividades Culturales y Deportivas
                case "insertar_activ_cult":
                    NombreDina = "Agregar Actividades Culturales";
                    DescripcionActividad = DescripcionDuracion = DescripcionObservacionesActiv = string.Empty;
                    ValidacionesActividadesCulturalesDeportivas();
                    _Selected = 1;
                    LstProgramasActiv = new ObservableCollection<ACTIVIDAD>(new cActividad().GetData(x => x.ID_TIPO_PROGRAMA == (short)eTiposP.CULTURALES));
                    LstProgramasActiv.Insert(0, new ACTIVIDAD { ID_ACTIVIDAD = -1, DESCR = "SELECCIONE" });
                    SelectedPrograma = -1;
                    PopUpsViewModels.ShowPopUp(this, PopUpsViewModels.TipoPopUp.ACTIVIDADES_CULTURALES_DEPORTIVAS_EDUCATIVO_FUERO_COMUN);
                    break;

                case "editar_activ_cult":
                    if (SelectIngreso != null)
                    {
                        EnabledActivCultComun = false;
                        NombreDina = "Edición Actividades Culturales y Deportivas";
                        _Selected = 1;
                        if (SelectedActivCultural != null)
                        {
                            LstProgramasActiv = new ObservableCollection<ACTIVIDAD>(new cActividad().GetData(x => x.ID_TIPO_PROGRAMA == (short)eTiposP.CULTURALES));
                            LstProgramasActiv.Insert(0, new ACTIVIDAD { ID_ACTIVIDAD = -1, DESCR = "SELECCIONE" });
                            DescripcionActividad = SelectedActivCultural.ACTIVIDAD;
                            DescripcionDuracion = SelectedActivCultural.DURACION;
                            DescripcionObservacionesActiv = SelectedActivCultural.OBSERVACION;
                            SelectedPrograma = SelectedActivCultural.ID_ACTIVIDAD;
                        };
                    };

                    PopUpsViewModels.ShowPopUp(this, PopUpsViewModels.TipoPopUp.ACTIVIDADES_CULTURALES_DEPORTIVAS_EDUCATIVO_FUERO_COMUN);
                    break;

                case "eliminar_activ_cult":
                    if (SelectIngreso != null)
                        if (SelectedActivCultural != null)
                            if (LstAcitividadesCulturales != null && LstAcitividadesCulturales.Any())
                                if (LstAcitividadesCulturales.Remove(SelectedActivCultural))
                                    SelectedActivCultural = null;

                    _Selected = 1;
                    EnabledActivCultComun = true;
                    break;

                case "insertar_activ_dep":
                    _Selected = 2;
                    NombreDina = "Agregar Actividades  Deportivas";
                    DescripcionActividad = DescripcionDuracion = DescripcionObservacionesActiv = string.Empty;
                    ValidacionesActividadesCulturalesDeportivas();
                    LstProgramasActiv = new ObservableCollection<ACTIVIDAD>(new cActividad().GetData(x => x.ID_TIPO_PROGRAMA == (short)eTiposP.DEPORTIVAS));
                    LstProgramasActiv.Insert(0, new ACTIVIDAD { ID_ACTIVIDAD = -1, DESCR = "SELECCIONE" });
                    SelectedPrograma = -1;
                    PopUpsViewModels.ShowPopUp(this, PopUpsViewModels.TipoPopUp.ACTIVIDADES_CULTURALES_DEPORTIVAS_EDUCATIVO_FUERO_COMUN);
                    break;

                case "editar_activ_dep":
                    if (SelectIngreso != null)
                    {
                        _Selected = 2;
                        EnabledActivCultComun = false;
                        NombreDina = "Edición Actividades Culturales y Deportivas";
                        if (SelectedActividadDeportiva != null)
                        {
                            LstProgramasActiv = new ObservableCollection<ACTIVIDAD>(new cActividad().GetData(x => x.ID_TIPO_PROGRAMA == (short)eTiposP.DEPORTIVAS));
                            LstProgramasActiv.Insert(0, new ACTIVIDAD { ID_ACTIVIDAD = -1, DESCR = "SELECCIONE" });
                            DescripcionActividad = SelectedActividadDeportiva.ACTIVIDAD;
                            DescripcionDuracion = SelectedActividadDeportiva.DURACION;
                            DescripcionObservacionesActiv = SelectedActividadDeportiva.OBSERVACION;
                            SelectedPrograma = SelectedActividadDeportiva.ID_ACTIVIDAD;
                        };
                    };

                    PopUpsViewModels.ShowPopUp(this, PopUpsViewModels.TipoPopUp.ACTIVIDADES_CULTURALES_DEPORTIVAS_EDUCATIVO_FUERO_COMUN);
                    break;

                case "eliminar_activ_dep":
                    if (SelectIngreso != null)
                        if (SelectedActividadDeportiva != null)
                            if (LstActividadesDeportivas != null && LstActividadesDeportivas.Any())
                                if (LstActividadesDeportivas.Remove(SelectedActividadDeportiva))
                                    SelectedActividadDeportiva = null;

                    _Selected = 2;
                    EnabledActivCultComun = true;
                    break;

                case "guardar_activ_cult":
                    if (base.HasErrors)
                    {
                        Application.Current.Dispatcher.Invoke((System.Action)(delegate
                        {
                            (new Dialogos()).ConfirmacionDialogo("Advertencia!", base.Error);
                        }));
                        return;
                    }

                    if (_Selected == 1)
                    {
                        if (LstAcitividadesCulturales == null)
                            LstAcitividadesCulturales = new ObservableCollection<PFC_VII_ACTIVIDAD>();

                        if (SelectedActivCultural == null)
                        {
                            if (SelectedPrograma == null)
                            {
                                new Dialogos().ConfirmacionDialogo("Validación", "Es necesario seleccionar un grupo");
                                return;
                            };

                            if (SelectedPrograma == -1)
                            {
                                new Dialogos().ConfirmacionDialogo("Validación", "Es necesario seleccionar un grupo");
                                return;
                            };

                            if (LstAcitividadesCulturales.Any(x => x.ID_ACTIVIDAD == SelectedPrograma))
                            {
                                new Dialogos().ConfirmacionDialogo("Validación", "El grupo seleccionado ya existe");
                                return;
                            };

                            var _DetallesActividad = new cActividad().GetData(x => x.ID_TIPO_PROGRAMA == (short)eTiposP.CULTURALES && x.ID_ACTIVIDAD == SelectedPrograma).FirstOrDefault();
                            LstAcitividadesCulturales.Add(new PFC_VII_ACTIVIDAD
                                {
                                    ACTIVIDAD = DescripcionActividad,
                                    DURACION = DescripcionDuracion,
                                    ID_ANIO = SelectIngreso.ID_ANIO,
                                    ID_CENTRO = SelectIngreso.ID_CENTRO,
                                    ID_IMPUTADO = SelectIngreso.ID_IMPUTADO,
                                    ID_TIPO_PROGRAMA = (short)eTiposP.CULTURALES,
                                    ID_INGRESO = SelectIngreso.ID_INGRESO,
                                    OBSERVACION = DescripcionObservacionesActiv,
                                    ID_ACTIVIDAD = SelectedPrograma,
                                    ACTIVIDAD1 = _DetallesActividad,
                                    TIPO_PROGRAMA = _DetallesActividad != null ? _DetallesActividad.TIPO_PROGRAMA : null
                                });
                        }

                        else
                        {
                            PFC_VII_ACTIVIDAD _temp = SelectedActivCultural;
                            if (LstAcitividadesCulturales.Remove(SelectedActivCultural))
                            {
                                var _detalleActividad = new cActividad().GetData(x => x.ID_ACTIVIDAD == _temp.ID_ACTIVIDAD && x.ID_TIPO_PROGRAMA == (short)eTiposP.CULTURALES).FirstOrDefault();
                                var _DatoNuevo = new PFC_VII_ACTIVIDAD()
                                {
                                    ACTIVIDAD = DescripcionActividad,
                                    DURACION = DescripcionDuracion,
                                    OBSERVACION = DescripcionObservacionesActiv,
                                    ID_ACTIVIDAD = SelectedPrograma,
                                    ID_TIPO_PROGRAMA = (short)eTiposP.CULTURALES,
                                    ID_CENTRO = SelectIngreso.ID_CENTRO,
                                    ID_ANIO = SelectIngreso.ID_ANIO,
                                    ID_IMPUTADO = SelectIngreso.ID_IMPUTADO,
                                    ID_INGRESO = SelectIngreso.ID_INGRESO,
                                    ACTIVIDAD1 = _detalleActividad,
                                    TIPO_PROGRAMA = _detalleActividad != null ? _detalleActividad.TIPO_PROGRAMA : null
                                };

                                LstAcitividadesCulturales.Add(_DatoNuevo);
                                _temp = null;
                            };
                        }
                    }

                    else
                    {
                        if (_Selected == 2)
                        {
                            if (SelectedActividadDeportiva == null)
                            {
                                if (SelectedPrograma == null)
                                {
                                    new Dialogos().ConfirmacionDialogo("Validación", "Es necesario seleccionar un grupo");
                                    return;
                                };

                                if (SelectedPrograma == -1)
                                {
                                    new Dialogos().ConfirmacionDialogo("Validación", "Es necesario seleccionar un grupo");
                                    return;
                                };

                                if (LstActividadesDeportivas.Any(x => x.ID_ACTIVIDAD == SelectedPrograma))
                                {
                                    new Dialogos().ConfirmacionDialogo("Validación", "El grupo seleccionado ya existe");
                                    return;
                                };

                                var _DetallesActividad = new cActividad().GetData(x => x.ID_TIPO_PROGRAMA == (short)eTiposP.DEPORTIVAS && x.ID_ACTIVIDAD == SelectedPrograma).FirstOrDefault();
                                LstActividadesDeportivas.Add(new PFC_VII_ACTIVIDAD
                                {
                                    ACTIVIDAD = DescripcionActividad,
                                    DURACION = DescripcionDuracion,
                                    ID_ANIO = SelectIngreso.ID_ANIO,
                                    ID_CENTRO = SelectIngreso.ID_CENTRO,
                                    ID_IMPUTADO = SelectIngreso.ID_IMPUTADO,
                                    ID_INGRESO = SelectIngreso.ID_INGRESO,
                                    OBSERVACION = DescripcionObservacionesActiv,
                                    ID_TIPO_PROGRAMA = (short)eTiposP.DEPORTIVAS,
                                    ID_ACTIVIDAD = SelectedPrograma,
                                    ACTIVIDAD1 = _DetallesActividad,
                                    TIPO_PROGRAMA = _DetallesActividad != null ? _DetallesActividad.TIPO_PROGRAMA : null
                                });
                            }

                            else
                            {
                                PFC_VII_ACTIVIDAD _temp = SelectedActividadDeportiva;
                                if (LstActividadesDeportivas.Remove(SelectedActividadDeportiva))
                                {
                                    var _detalleActividad = new cActividad().GetData(x => x.ID_ACTIVIDAD == _temp.ID_ACTIVIDAD).FirstOrDefault();
                                    var _DatoNuevo = new PFC_VII_ACTIVIDAD()
                                    {
                                        ACTIVIDAD = DescripcionActividad,
                                        DURACION = DescripcionDuracion,
                                        OBSERVACION = DescripcionObservacionesActiv,
                                        ID_TIPO_PROGRAMA = (short)eTiposP.DEPORTIVAS,
                                        ID_ANIO = SelectIngreso.ID_ANIO,
                                        ID_CENTRO = SelectIngreso.ID_CENTRO,
                                        ID_IMPUTADO = SelectIngreso.ID_IMPUTADO,
                                        ID_INGRESO = SelectIngreso.ID_INGRESO,
                                        ID_ACTIVIDAD = SelectedPrograma,
                                        ACTIVIDAD1 = _detalleActividad,
                                        TIPO_PROGRAMA = _detalleActividad != null ? _detalleActividad.TIPO_PROGRAMA : null
                                    };

                                    LstActividadesDeportivas.Add(_DatoNuevo);
                                    _temp = null;
                                };
                            }
                        }
                    }

                    EnabledActivCultComun = true;
                    PopUpsViewModels.ClosePopUp(PopUpsViewModels.TipoPopUp.ACTIVIDADES_CULTURALES_DEPORTIVAS_EDUCATIVO_FUERO_COMUN);
                    break;

                case "cancelar_activ_cult":
                    SelectedActivCultural = null;
                    LimpiaValidacionesActividadesCulturalesDeportivas();
                    PopUpsViewModels.ClosePopUp(PopUpsViewModels.TipoPopUp.ACTIVIDADES_CULTURALES_DEPORTIVAS_EDUCATIVO_FUERO_COMUN);
                    break;

                #endregion
                #region Actividades Laborales Gratificadas y no gratificdas, Capacitacion
                case "agregar_actividad_lab"://Actividad gratificada
                    EnablededitarCampoCapacitacionComun = true;
                    SelectedActivNoGratificada = SelectedCapacitacionLaboral = null;
                    NombreDinamico2 = "Agregar Actividades Gratificadas";
                    ListaActual = (short)eTipoLista.ACTIV_GRATIFICADAS;
                    NombreA = "Taller";
                    IsEnabledConcluyo = Visibility.Hidden;
                    //PreparaListasCapacitacion("G");
                    LstCapacitacion = new ObservableCollection<ACTIVIDAD>(new cActividad().GetData(x => x.ID_TIPO_PROGRAMA == (short)eTiposP.ACTIVIDADES_GRATIFICADAS));
                    LstCapacitacion.Insert(0, new ACTIVIDAD { DESCR = "SELECCIONE", ID_ACTIVIDAD = -1 });
                    DescripcionPeriodo = ObservacionesActiv = string.Empty;
                    IdCapac = -1;
                    PopUpsViewModels.ShowPopUp(this, PopUpsViewModels.TipoPopUp.ACTIVIDADES_LABORALES_CAPACITACION_TRABAJO_COMUN);
                    break;

                case "agregar_capa_lab": //1
                    EnablededitarCampoCapacitacionComun = true;
                    SelectedActivNoGratificada = SelectedActivGratificada = null;
                    NombreDinamico2 = "Agregar Capacitación Laboral";
                    ListaActual = (short)eTipoLista.CAPACITACION_LABORAL;
                    NombreA = "Taller";
                    IsEnabledConcluyo = Visibility.Visible;
                    //PreparaListasCapacitacion("L");
                    LstCapacitacion = new ObservableCollection<ACTIVIDAD>(new cActividad().GetData(x => x.ID_TIPO_PROGRAMA == (short)eTiposP.CAPACITACION_LABORAL));
                    LstCapacitacion.Insert(0, new ACTIVIDAD { DESCR = "SELECCIONE", ID_ACTIVIDAD = -1 });
                    DescripcionPeriodo = ObservacionesActiv = ConcluyoActiv = string.Empty;
                    IdCapac = -1;
                    ValidacionCapacLaboral();
                    PopUpsViewModels.ShowPopUp(this, PopUpsViewModels.TipoPopUp.ACTIVIDADES_LABORALES_CAPACITACION_TRABAJO_COMUN);
                    break;

                case "editar_cap_lab":
                    EnablededitarCampoCapacitacionComun = false;
                    if (SelectedCapacitacionLaboral != null)
                    {
                        ListaActual = (short)eTipoLista.CAPACITACION_LABORAL;
                        SelectedActivNoGratificada = SelectedActivGratificada = null;
                        NombreDinamico2 = "Edición Capacitación Laboral";
                        NombreA = "Taller";
                        IsEnabledConcluyo = Visibility.Visible;
                        //PreparaListasCapacitacion("L");
                        LstCapacitacion = new ObservableCollection<ACTIVIDAD>(new cActividad().GetData(x => x.ID_TIPO_PROGRAMA == (short)eTiposP.CAPACITACION_LABORAL));
                        LstCapacitacion.Insert(0, new ACTIVIDAD { DESCR = "SELECCIONE", ID_ACTIVIDAD = -1 });
                        ConcluyoActiv = SelectedCapacitacionLaboral.CONCLUYO;
                        DescripcionPeriodo = SelectedCapacitacionLaboral.PERIODO;
                        ObservacionesActiv = SelectedCapacitacionLaboral.OBSERVACION;
                        IdCapac = SelectedCapacitacionLaboral.ID_ACTIVIDAD;
                        ValidacionCapacLaboral();
                        PopUpsViewModels.ShowPopUp(this, PopUpsViewModels.TipoPopUp.ACTIVIDADES_LABORALES_CAPACITACION_TRABAJO_COMUN);
                    }
                    else
                        new Dialogos().ConfirmacionDialogo("Validación", "Es necesario seleccionar una capacitación");

                    break;

                case "eliminar_cap_lab":
                    if (SelectedCapacitacionLaboral != null)
                        if (LstCapacitacionLaboral != null && LstCapacitacionLaboral.Any())
                            if (LstCapacitacionLaboral.Remove(SelectedCapacitacionLaboral))
                                SelectedCapacitacionLaboral = null;

                    ListaActual = (short)eTipoLista.CAPACITACION_LABORAL;
                    break;

                case "agregar_actividad_lab_no_grat":
                    EnablededitarCampoCapacitacionComun = true;
                    SelectedCapacitacionLaboral = SelectedActivGratificada = null;
                    ListaActual = (short)eTipoLista.ACTIV_NO_GRATIFICADAS;
                    NombreDinamico2 = "Agregar Actividades Laborales No Gratificadas";
                    NombreA = "Actividad";
                    IsEnabledConcluyo = Visibility.Hidden;
                    //PreparaListasCapacitacion("N");
                    LstCapacitacion = new ObservableCollection<ACTIVIDAD>(new cActividad().GetData(x => x.ID_TIPO_PROGRAMA == (short)eTiposP.ACTIVIDADES_NO_GRATIFICADAS));
                    LstCapacitacion.Insert(0, new ACTIVIDAD { DESCR = "SELECCIONE", ID_ACTIVIDAD = -1 });
                    DescripcionPeriodo = ObservacionesActiv = string.Empty;
                    IdCapac = -1;
                    PopUpsViewModels.ShowPopUp(this, PopUpsViewModels.TipoPopUp.ACTIVIDADES_LABORALES_CAPACITACION_TRABAJO_COMUN);
                    break;

                case "editar_actividad_lab_no_grat":
                    EnablededitarCampoCapacitacionComun = false;
                    SelectedCapacitacionLaboral = SelectedActivGratificada = null;
                    ListaActual = (short)eTipoLista.ACTIV_NO_GRATIFICADAS;
                    if (SelectedActivNoGratificada != null)
                    {
                        NombreDinamico2 = "Edición Actividades Laborales No Gratificadas";
                        NombreA = "Taller";
                        IsEnabledConcluyo = Visibility.Hidden;
                        //PreparaListasCapacitacion("G");
                        LstCapacitacion = new ObservableCollection<ACTIVIDAD>(new cActividad().GetData(x => x.ID_TIPO_PROGRAMA == (short)eTiposP.ACTIVIDADES_NO_GRATIFICADAS));
                        LstCapacitacion.Insert(0, new ACTIVIDAD { DESCR = "SELECCIONE", ID_ACTIVIDAD = -1 });
                        DescripcionPeriodo = SelectedActivNoGratificada.PERIODO;
                        ObservacionesActiv = SelectedActivNoGratificada.OBSERVACION;
                        IdCapac = SelectedActivNoGratificada.ID_ACTIVIDAD;
                        ValidacionesActivLaborales();
                        PopUpsViewModels.ShowPopUp(this, PopUpsViewModels.TipoPopUp.ACTIVIDADES_LABORALES_CAPACITACION_TRABAJO_COMUN);
                    }
                    else
                        new Dialogos().ConfirmacionDialogo("Validación", "Es necesario seleccionar una actividad");

                    break;

                case "eliminar_actividad_lab_no_grat":
                    if (SelectedActivNoGratificada != null)
                        if (LstActivNoGratificadas != null && LstActivNoGratificadas.Any())
                            if (LstActivNoGratificadas.Remove(SelectedActivNoGratificada))
                                SelectedActivNoGratificada = null;

                    ListaActual = (short)eTipoLista.ACTIV_NO_GRATIFICADAS;
                    break;

                case "editar_actividad_lab":
                    EnablededitarCampoCapacitacionComun = false;
                    if (SelectedActivGratificada != null)
                    {
                        ListaActual = (short)eTipoLista.ACTIV_GRATIFICADAS;
                        NombreDinamico2 = "Edición Actividades Gratificadas";
                        NombreA = "Taller";
                        IsEnabledConcluyo = Visibility.Hidden;
                        //PreparaListasCapacitacion("G");
                        LstCapacitacion = new ObservableCollection<ACTIVIDAD>(new cActividad().GetData(x => x.ID_TIPO_PROGRAMA == (short)eTiposP.ACTIVIDADES_GRATIFICADAS));
                        LstCapacitacion.Insert(0, new ACTIVIDAD { DESCR = "SELECCIONE", ID_ACTIVIDAD = -1 });
                        DescripcionPeriodo = SelectedActivGratificada.PERIODO;
                        ObservacionesActiv = SelectedActivGratificada.OBSERVACION;
                        IdCapac = SelectedActivGratificada.ID_ACTIVIDAD;
                        ValidacionesActivLaborales();
                        PopUpsViewModels.ShowPopUp(this, PopUpsViewModels.TipoPopUp.ACTIVIDADES_LABORALES_CAPACITACION_TRABAJO_COMUN);
                    }
                    else
                        new Dialogos().ConfirmacionDialogo("Validación", "Es necesario seleccionar una actividad");

                    break;

                case "eliminar_actividad_lab":
                    if (SelectedActivGratificada != null)
                        if (LstActivGratificadas != null && LstActivGratificadas.Any())
                            if (LstActivGratificadas.Remove(SelectedActivGratificada))
                                SelectedActivGratificada = null;

                    ListaActual = (short)eTipoLista.ACTIV_GRATIFICADAS;
                    break;

                case "guardar_actividad_lab":
                    EnablededitarCampoCapacitacionComun = true;
                    if (ListaActual == (short)eTipoLista.ACTIV_GRATIFICADAS)
                    {
                        ValidacionesActivLaborales();
                        if (base.HasErrors)
                        {
                            Application.Current.Dispatcher.Invoke((System.Action)(delegate
                            {
                                (new Dialogos()).ConfirmacionDialogo("Advertencia!", base.Error);
                            }));
                            return;
                        }

                        if (LstActivGratificadas == null)
                            LstActivGratificadas = new ObservableCollection<PFC_VIII_ACTIVIDAD_LABORAL>();

                        if (IdCapac == null)
                        {
                            new Dialogos().ConfirmacionDialogo("Validación", "Es necesario seleccionar un grupo");
                            return;
                        };

                        if (IdCapac == -1)
                        {
                            new Dialogos().ConfirmacionDialogo("Validación", "Es necesario seleccionar un grupo");
                            return;
                        };

                        var detallesActividad = new cActividad().GetData(x => x.ID_ACTIVIDAD == IdCapac && x.ID_TIPO_PROGRAMA == (short)eTiposP.ACTIVIDADES_GRATIFICADAS).FirstOrDefault();
                        if (SelectedActivGratificada == null)
                        {
                            if (LstActivGratificadas.Any(x => x.ID_ACTIVIDAD == IdCapac))
                            {
                                new Dialogos().ConfirmacionDialogo("Validación", "El grupo seleccionado ya existe");
                                return;
                            };

                            LstActivGratificadas.Add(new PFC_VIII_ACTIVIDAD_LABORAL
                                {
                                    CONCLUYO = string.Empty,//NO APLICA EN ESTE TIPO DE ACTIVIDAD
                                    ID_ANIO = SelectIngreso.ID_ANIO,
                                    ID_CAPACITACION = IdCapac,
                                    ID_CENTRO = SelectIngreso.ID_CENTRO,
                                    ID_IMPUTADO = SelectIngreso.ID_IMPUTADO,
                                    ID_INGRESO = SelectIngreso.ID_INGRESO,
                                    OBSERVACION = ObservacionesActiv,
                                    ACTIVIDAD = detallesActividad,
                                    ID_ACTIVIDAD = detallesActividad != null ? detallesActividad.ID_ACTIVIDAD : new short(),
                                    ID_TIPO_PROGRAMA = detallesActividad != null ? detallesActividad.ID_TIPO_PROGRAMA : new short(),
                                    TIPO_PROGRAMA = detallesActividad != null ? detallesActividad.TIPO_PROGRAMA : null,
                                    PERIODO = DescripcionPeriodo,
                                    PFC_VIII_CAPACITACION = SelectedCapac,
                                });
                        }
                        else
                        {
                            var _temp = SelectedActivGratificada;
                            var _actividd = _temp != null ? new cActividad().GetData(x => x.ID_ACTIVIDAD == _temp.ID_ACTIVIDAD && x.ID_TIPO_PROGRAMA == (short)eTiposP.ACTIVIDADES_GRATIFICADAS).FirstOrDefault() : null;
                            if (LstActivGratificadas.Remove(SelectedActivGratificada))
                            {
                                LstActivGratificadas.Add(new PFC_VIII_ACTIVIDAD_LABORAL
                                    {
                                        CONCLUYO = string.Empty,//NO APLICA EN ESTE TIPO DE ACTIVIDAD
                                        ID_ANIO = SelectIngreso.ID_ANIO,
                                        ID_CAPACITACION = IdCapac,
                                        ID_CENTRO = SelectIngreso.ID_CENTRO,
                                        ID_IMPUTADO = SelectIngreso.ID_IMPUTADO,
                                        ID_INGRESO = SelectIngreso.ID_INGRESO,
                                        OBSERVACION = ObservacionesActiv,
                                        PERIODO = DescripcionPeriodo,
                                        PFC_VIII_CAPACITACION = SelectedCapac,
                                        ACTIVIDAD = _actividd,
                                        ID_TIPO_PROGRAMA = _actividd != null ? _actividd.ID_TIPO_PROGRAMA : new short(),
                                        ID_ACTIVIDAD = _actividd != null ? _actividd.ID_ACTIVIDAD : new short(),
                                        TIPO_PROGRAMA = _actividd != null ? _actividd.TIPO_PROGRAMA : null
                                    });
                            };
                        }

                        SelectedActivGratificada = null;
                        PopUpsViewModels.ClosePopUp(PopUpsViewModels.TipoPopUp.ACTIVIDADES_LABORALES_CAPACITACION_TRABAJO_COMUN);
                    };

                    if (ListaActual == (short)eTipoLista.ACTIV_NO_GRATIFICADAS)
                    {
                        ValidacionesActivLaborales();
                        if (base.HasErrors)
                        {
                            Application.Current.Dispatcher.Invoke((System.Action)(delegate
                            {
                                (new Dialogos()).ConfirmacionDialogo("Advertencia!", base.Error);
                            }));
                            return;
                        }

                        if (IdCapac == null)
                        {
                            new Dialogos().ConfirmacionDialogo("Validación", "Es necesario seleccionar un grupo");
                            return;
                        };

                        if (IdCapac == -1)
                        {
                            new Dialogos().ConfirmacionDialogo("Validación", "Es necesario seleccionar un grupo");
                            return;
                        };

                        if (LstActivNoGratificadas == null)//Inicializa la lista en caso de que no se halla hecho antes
                            LstActivNoGratificadas = new ObservableCollection<PFC_VIII_ACTIVIDAD_LABORAL>();

                        var detallesActividad = new cActividad().GetData(x => x.ID_ACTIVIDAD == IdCapac && x.ID_TIPO_PROGRAMA == (short)eTiposP.ACTIVIDADES_NO_GRATIFICADAS).FirstOrDefault();

                        var temp = SelectedActivNoGratificada;
                        if (SelectedActivNoGratificada == null)
                        {
                            if (LstActivNoGratificadas.Any(x => x.ID_ACTIVIDAD == IdCapac))
                            {
                                new Dialogos().ConfirmacionDialogo("Validación", "El grupo seleccionado ya existe");
                                return;
                            };

                            LstActivNoGratificadas.Add(new PFC_VIII_ACTIVIDAD_LABORAL
                                {
                                    CONCLUYO = string.Empty,//NO APLICA EN ESTE TIPO DE ACTIVIDAD
                                    ID_ANIO = SelectIngreso.ID_ANIO,
                                    ID_CAPACITACION = IdCapac,
                                    ID_CENTRO = SelectIngreso.ID_CENTRO,
                                    ID_IMPUTADO = SelectIngreso.ID_IMPUTADO,
                                    ID_INGRESO = SelectIngreso.ID_INGRESO,
                                    OBSERVACION = ObservacionesActiv,
                                    PERIODO = DescripcionPeriodo,
                                    PFC_VIII_CAPACITACION = SelectedCapac,
                                    ID_ACTIVIDAD = detallesActividad != null ? detallesActividad.ID_ACTIVIDAD : new short(),
                                    ACTIVIDAD = detallesActividad,
                                    TIPO_PROGRAMA = detallesActividad != null ? detallesActividad.TIPO_PROGRAMA : null,
                                    ID_TIPO_PROGRAMA = detallesActividad != null ? detallesActividad.ID_TIPO_PROGRAMA : new short()
                                });
                        }
                        else
                            if (LstActivNoGratificadas.Remove(SelectedActivNoGratificada))
                            {
                                var _Actividad = temp != null ? new cActividad().GetData(x => x.ID_ACTIVIDAD == temp.ID_ACTIVIDAD).FirstOrDefault() : null;
                                LstActivNoGratificadas.Add(new PFC_VIII_ACTIVIDAD_LABORAL
                                {
                                    CONCLUYO = string.Empty,//NO APLICA EN ESTE TIPO DE ACTIVIDAD
                                    ID_ANIO = SelectIngreso.ID_ANIO,
                                    ID_CAPACITACION = IdCapac,
                                    ID_CENTRO = SelectIngreso.ID_CENTRO,
                                    ID_IMPUTADO = SelectIngreso.ID_IMPUTADO,
                                    ID_INGRESO = SelectIngreso.ID_INGRESO,
                                    OBSERVACION = ObservacionesActiv,
                                    PERIODO = DescripcionPeriodo,
                                    PFC_VIII_CAPACITACION = SelectedCapac,
                                    ID_ACTIVIDAD = detallesActividad != null ? detallesActividad.ID_ACTIVIDAD : new short(),
                                    ID_TIPO_PROGRAMA = detallesActividad != null ? detallesActividad.ID_TIPO_PROGRAMA : new short(),
                                    ACTIVIDAD = _Actividad,
                                    TIPO_PROGRAMA = _Actividad != null ? _Actividad.TIPO_PROGRAMA : null
                                });
                            };

                        SelectedActivNoGratificada = null;
                        PopUpsViewModels.ClosePopUp(PopUpsViewModels.TipoPopUp.ACTIVIDADES_LABORALES_CAPACITACION_TRABAJO_COMUN);
                    };

                    if (ListaActual == (short)eTipoLista.CAPACITACION_LABORAL)
                    {
                        ValidacionesActivLaborales();
                        if (base.HasErrors)
                        {
                            Application.Current.Dispatcher.Invoke((System.Action)(delegate
                            {
                                (new Dialogos()).ConfirmacionDialogo("Advertencia!", base.Error);
                            }));
                            return;
                        }

                        if (LstCapacitacionLaboral == null)
                            LstCapacitacionLaboral = new ObservableCollection<PFC_VIII_ACTIVIDAD_LABORAL>();

                        if (IdCapac == null)
                        {
                            new Dialogos().ConfirmacionDialogo("Validación", "Es necesario seleccionar un grupo");
                            return;
                        };

                        if (IdCapac == -1)
                        {
                            new Dialogos().ConfirmacionDialogo("Validación", "Es necesario seleccionar un grupo");
                            return;
                        };

                        if (SelectedCapacitacionLaboral == null)
                        {
                            if (LstCapacitacionLaboral.Any(x => x.ID_ACTIVIDAD == IdCapac))
                            {
                                new Dialogos().ConfirmacionDialogo("Validación", "El grupo seleccionado ya existe");
                                return;
                            };

                            var detallesActividad = new cActividad().GetData(x => x.ID_ACTIVIDAD == IdCapac && x.ID_TIPO_PROGRAMA == (short)eTiposP.CAPACITACION_LABORAL).FirstOrDefault();
                            LstCapacitacionLaboral.Add(new PFC_VIII_ACTIVIDAD_LABORAL
                                {
                                    CONCLUYO = ConcluyoActiv,
                                    ID_ANIO = SelectIngreso.ID_ANIO,
                                    ID_CAPACITACION = IdCapac,
                                    ID_CENTRO = SelectIngreso.ID_CENTRO,
                                    ID_IMPUTADO = SelectIngreso.ID_IMPUTADO,
                                    OBSERVACION = ObservacionesActiv,
                                    PERIODO = DescripcionPeriodo,
                                    ID_INGRESO = SelectIngreso.ID_INGRESO,
                                    PFC_VIII_CAPACITACION = SelectedCapac,
                                    ACTIVIDAD = detallesActividad,
                                    ID_ACTIVIDAD = detallesActividad != null ? detallesActividad.ID_ACTIVIDAD : new short(),
                                    ID_TIPO_PROGRAMA = detallesActividad != null ? detallesActividad.ID_TIPO_PROGRAMA : new short(),
                                    TIPO_PROGRAMA = detallesActividad != null ? detallesActividad.TIPO_PROGRAMA : null
                                });
                        }
                        else
                        {
                            var _temp = SelectedCapacitacionLaboral;
                            var _actividd = _temp != null ? new cActividad().GetData(x => x.ID_ACTIVIDAD == _temp.ID_ACTIVIDAD && x.ID_TIPO_PROGRAMA == (short)eTiposP.CAPACITACION_LABORAL).FirstOrDefault() : null;

                            if (LstCapacitacionLaboral.Remove(SelectedCapacitacionLaboral))
                            {
                                LstCapacitacionLaboral.Add(new PFC_VIII_ACTIVIDAD_LABORAL
                                {
                                    CONCLUYO = ConcluyoActiv,
                                    ID_ANIO = SelectIngreso.ID_ANIO,
                                    ID_CAPACITACION = IdCapac,
                                    ID_CENTRO = SelectIngreso.ID_CENTRO,
                                    ID_IMPUTADO = SelectIngreso.ID_IMPUTADO,
                                    OBSERVACION = ObservacionesActiv,
                                    PERIODO = DescripcionPeriodo,
                                    ID_INGRESO = SelectIngreso.ID_INGRESO,
                                    PFC_VIII_CAPACITACION = SelectedCapac,
                                    ACTIVIDAD = _actividd,
                                    ID_ACTIVIDAD = _actividd != null ? _actividd.ID_ACTIVIDAD : new short(),
                                    ID_TIPO_PROGRAMA = _actividd != null ? _actividd.ID_TIPO_PROGRAMA : new short(),
                                    TIPO_PROGRAMA = _actividd != null ? _actividd.TIPO_PROGRAMA : null
                                });
                            };
                        };

                        SelectedCapacitacionLaboral = null;
                        PopUpsViewModels.ClosePopUp(PopUpsViewModels.TipoPopUp.ACTIVIDADES_LABORALES_CAPACITACION_TRABAJO_COMUN);
                    };
                    break;

                case "cancelar_actividad_lab":
                    SelectedCapacitacionLaboral = SelectedActivNoGratificada = SelectedActivGratificada = null;
                    LimpiarValidacionesLaborales();
                    PopUpsViewModels.ClosePopUp(PopUpsViewModels.TipoPopUp.ACTIVIDADES_LABORALES_CAPACITACION_TRABAJO_COMUN);
                    break;
                #endregion
                #region SocioFamiliar Comun Grupos
                case "edit_grupos_soc":///Grupos religiosos
                    if (SelectedGrupoSocioEconomico != null)
                    {
                        AgregarCongregacion = EnabledCongregacionSocComun = false;
                        TituloModal = "Editar Apoyo Espiritual";
                        IsEnabledC = Visibility.Visible;
                        CongregSocFC = PeriodoSocFC = ObservacionesSocFC = PeriodoSocFC = CongregSocFC = ObservacionesSocFC = string.Empty;
                        LstAcvidadesCongregacionesTSComun = new ObservableCollection<ACTIVIDAD>(new cActividad().GetData(x => x.ID_TIPO_PROGRAMA == (short)eTiposP.APOYO_ESPIRITUAL));
                        LstAcvidadesCongregacionesTSComun.Insert(0, new ACTIVIDAD() { DESCR = "SELECCIONE", ID_ACTIVIDAD = -1 });
                        IsCongre = (short)eSINO.SI;
                        CongregSocFC = PeriodoSocFC = ObservacionesSocFC = string.Empty;
                        PeriodoSocFC = SelectedGrupoSocioEconomico.PERIODO;
                        CongregSocFC = SelectedGrupoSocioEconomico.CONGREGACION;
                        ObservacionesSocFC = SelectedGrupoSocioEconomico.OBSERVACIONES;
                        SelectedCongActiv = SelectedGrupoSocioEconomico.ID_ACTIVIDAD;
                        ValidacionesCongreg();
                        PopUpsViewModels.ShowPopUp(this, PopUpsViewModels.TipoPopUp.AGREGAR_CONGREGACIONES_SOCIFAM_FC);
                    }
                    else
                        new Dialogos().ConfirmacionDialogo("Validación", "Es necesario seleccionar un grupo");

                    break;

                case "agregar_grupos_soc":
                    EnabledCongregacionSocComun = true;
                    TituloModal = "Agregar Apoyo Espiritual";
                    IsEnabledC = Visibility.Visible;
                    IsCongre = (short)eSINO.SI;
                    AgregarCongregacion = true;
                    CongregSocFC = PeriodoSocFC = ObservacionesSocFC = PeriodoSocFC = CongregSocFC = ObservacionesSocFC = string.Empty;
                    LstAcvidadesCongregacionesTSComun = new ObservableCollection<ACTIVIDAD>(new cActividad().GetData(x => x.ID_TIPO_PROGRAMA == (short)eTiposP.APOYO_ESPIRITUAL));
                    LstAcvidadesCongregacionesTSComun.Insert(0, new ACTIVIDAD() { DESCR = "SELECCIONE", ID_ACTIVIDAD = -1 });
                    SelectedCongActiv = -1;
                    ValidacionesCongreg();
                    PopUpsViewModels.ShowPopUp(this, PopUpsViewModels.TipoPopUp.AGREGAR_CONGREGACIONES_SOCIFAM_FC);
                    break;

                case "eliminar_miembro_fam_comun":
                    if (SelectedComunicacionComun != null)
                        if (LstComunicaciones != null && LstComunicaciones.Any())
                            if (LstComunicaciones.Remove(SelectedComunicacionComun))
                                SelectedComunicacionComun = null;

                    break;
                case "eliminar_grupos_soc_uno":
                    if (SelectedGrupoSocioEconomico != null)
                    {
                        if (ListGruposSocioFamComun != null && ListGruposSocioFamComun.Any())
                            if (ListGruposSocioFamComun.Remove(SelectedGrupoSocioEconomico))
                                SelectedGrupoSocioEconomico = null;
                    }
                    else
                        new Dialogos().ConfirmacionDialogo("Validación", "Es necesario seleccionar un grupo");

                    break;

                case "eliminar_grupos_soc_dos":
                    if (SelectedFortGrupo != null)
                    {
                        if (ListFortalecimientoSocioFamComun != null && ListFortalecimientoSocioFamComun.Any())
                            if (ListFortalecimientoSocioFamComun.Remove(SelectedFortGrupo))
                                SelectedFortGrupo = null;
                    }
                    else
                        new Dialogos().ConfirmacionDialogo("Validación", "Es necesario seleccionar un grupo");

                    break;
                case "editar_grupo_fort":
                    if (SelectedFortGrupo != null)
                    {
                        EnabledCongregacionSocComun = false;
                        TituloModal = "Editar Programa de Fortalecimiento Familiar";
                        LstAcvidadesCongregacionesTSComun = new ObservableCollection<ACTIVIDAD>(new cActividad().GetData(x => x.ID_TIPO_PROGRAMA == (short)eTiposP.FORTALECIMIENTO_NUCLEO_FAMILIAR));
                        LstAcvidadesCongregacionesTSComun.Insert(0, new ACTIVIDAD() { DESCR = "SELECCIONE", ID_ACTIVIDAD = -1 });
                        IsCongre = (short)eSINO.NO;
                        IsEnabledC = Visibility.Collapsed;
                        PeriodoSocFC = ObservacionesSocFC = string.Empty;
                        PeriodoSocFC = SelectedFortGrupo.PERIODO;
                        ObservacionesSocFC = SelectedFortGrupo.OBSERVACIONES;
                        SelectedCongActiv = SelectedFortGrupo.ID_ACTIVIDAD;
                        ValidacionesFortFam();
                        PopUpsViewModels.ShowPopUp(this, PopUpsViewModels.TipoPopUp.AGREGAR_CONGREGACIONES_SOCIFAM_FC);
                    }
                    else
                        new Dialogos().ConfirmacionDialogo("Validación", "Es necesario seleccionar un grupo");

                    break;

                case "agregar_grupo_fort":
                    TituloModal = "Agregar Programa de Fortalecimiento Familiar";
                    EnabledCongregacionSocComun = true;
                    AgregarFortNucleoFamiliar = true;
                    IsCongre = (short)eSINO.NO;
                    IsEnabledC = Visibility.Collapsed;
                    PeriodoSocFC = ObservacionesSocFC = string.Empty;
                    LstAcvidadesCongregacionesTSComun = new ObservableCollection<ACTIVIDAD>(new cActividad().GetData(x => x.ID_TIPO_PROGRAMA == (short)eTiposP.FORTALECIMIENTO_NUCLEO_FAMILIAR));
                    LstAcvidadesCongregacionesTSComun.Insert(0, new ACTIVIDAD() { DESCR = "SELECCIONE", ID_ACTIVIDAD = -1 });
                    SelectedCongActiv = -1;
                    ValidacionesFortFam();
                    PopUpsViewModels.ShowPopUp(this, PopUpsViewModels.TipoPopUp.AGREGAR_CONGREGACIONES_SOCIFAM_FC);
                    break;
                case "save_grupos_soc":
                    if (IsCongre == (short)eSINO.SI)
                    {
                        if (base.HasErrors)
                        {
                            Application.Current.Dispatcher.Invoke((System.Action)(delegate
                            {
                                (new Dialogos()).ConfirmacionDialogo("Advertencia!", base.Error);
                            }));
                            return;
                        }

                        if (ListGruposSocioFamComun == null)
                            ListGruposSocioFamComun = new ObservableCollection<PFC_VI_GRUPO>();

                        if (AgregarCongregacion)
                        {
                            if (SelectedCongActiv == null)
                            {
                                new Dialogos().ConfirmacionDialogo("Validación", "Es necesario seleccionar un grupo");
                                return;
                            };

                            if (SelectedCongActiv == -1)
                            {
                                new Dialogos().ConfirmacionDialogo("Validación", "Es necesario seleccionar un grupo");
                                return;
                            };

                            if (ListGruposSocioFamComun.Any(x => x.ID_ACTIVIDAD == SelectedCongActiv))
                            {
                                new Dialogos().ConfirmacionDialogo("Validación", "El grupo seleccionado ya existe");
                                return;
                            };

                            var DetalleActividad = new cActividad().GetData(x => x.ID_ACTIVIDAD == SelectedCongActiv && x.ID_TIPO_PROGRAMA == (short)eTiposP.APOYO_ESPIRITUAL).FirstOrDefault();
                            ListGruposSocioFamComun.Add(new PFC_VI_GRUPO
                            {
                                CONGREGACION = CongregSocFC,
                                ID_ACTIVIDAD = SelectedCongActiv.HasValue ? SelectedCongActiv.Value : new short(),
                                ID_ANIO = SelectIngreso.ID_ANIO,
                                ID_CENTRO = SelectIngreso.ID_CENTRO,
                                ID_IMPUTADO = SelectIngreso.ID_IMPUTADO,
                                ID_INGRESO = SelectIngreso.ID_INGRESO,
                                ID_TIPO_PROGRAMA = DetalleActividad != null ? DetalleActividad.ID_TIPO_PROGRAMA : new short(),
                                OBSERVACIONES = ObservacionesSocFC,
                                PERIODO = PeriodoSocFC,
                                ACTIVIDAD = DetalleActividad,
                                TIPO_PROGRAMA = DetalleActividad != null ? DetalleActividad.TIPO_PROGRAMA : null
                            });

                            AgregarCongregacion = false;
                            SelectedGrupoSocioEconomico = SelectedFortGrupo = null;
                            PopUpsViewModels.ClosePopUp(PopUpsViewModels.TipoPopUp.AGREGAR_CONGREGACIONES_SOCIFAM_FC);
                            return;
                        }

                        var Antigui = SelectedGrupoSocioEconomico;
                        if (ListGruposSocioFamComun.Remove(SelectedGrupoSocioEconomico))
                        {
                            ListGruposSocioFamComun.Add(new PFC_VI_GRUPO
                            {
                                CONGREGACION = CongregSocFC,
                                ID_ACTIVIDAD = Antigui.ID_ACTIVIDAD,
                                ID_ANIO = SelectIngreso.ID_ANIO,
                                ID_CENTRO = SelectIngreso.ID_CENTRO,
                                ID_IMPUTADO = SelectIngreso.ID_IMPUTADO,
                                ID_INGRESO = SelectIngreso.ID_INGRESO,
                                ID_TIPO_PROGRAMA = Antigui.ID_TIPO_PROGRAMA,
                                OBSERVACIONES = ObservacionesSocFC,
                                PERIODO = PeriodoSocFC,
                                ACTIVIDAD = Antigui.ACTIVIDAD,
                                TIPO_PROGRAMA = Antigui.TIPO_PROGRAMA
                            });
                        }

                        Antigui = null;
                    }

                    if (IsCongre == (short)eSINO.NO)
                    {
                        if (base.HasErrors)
                        {
                            Application.Current.Dispatcher.Invoke((System.Action)(delegate
                            {
                                (new Dialogos()).ConfirmacionDialogo("Advertencia!", base.Error);
                            }));
                            return;
                        }

                        if (ListFortalecimientoSocioFamComun == null)
                            ListFortalecimientoSocioFamComun = new ObservableCollection<PFC_VI_GRUPO>();

                        if (AgregarFortNucleoFamiliar)
                        {
                            if (SelectedCongActiv == null)
                            {
                                new Dialogos().ConfirmacionDialogo("Validación", "Es necesario seleccionar un grupo");
                                return;
                            };

                            if (SelectedCongActiv == -1)
                            {
                                new Dialogos().ConfirmacionDialogo("Validación", "Es necesario seleccionar un grupo");
                                return;
                            };

                            if (ListFortalecimientoSocioFamComun.Any(z => z.ID_ACTIVIDAD == SelectedCongActiv))
                            {
                                new Dialogos().ConfirmacionDialogo("Validación", "El grupo seleccionado ya existe");
                                return;
                            };

                            var DetalleActividad = new cActividad().GetData(x => x.ID_ACTIVIDAD == SelectedCongActiv && x.ID_TIPO_PROGRAMA == (short)eTiposP.FORTALECIMIENTO_NUCLEO_FAMILIAR).FirstOrDefault();
                            ListFortalecimientoSocioFamComun.Add(new PFC_VI_GRUPO
                            {
                                ID_ACTIVIDAD = SelectedCongActiv.HasValue ? SelectedCongActiv.Value : new short(),
                                ID_ANIO = SelectIngreso.ID_ANIO,
                                ID_CENTRO = SelectIngreso.ID_CENTRO,
                                ID_IMPUTADO = SelectIngreso.ID_IMPUTADO,
                                ID_INGRESO = SelectIngreso.ID_INGRESO,
                                ID_TIPO_PROGRAMA = DetalleActividad != null ? DetalleActividad.ID_TIPO_PROGRAMA : new short(),
                                OBSERVACIONES = ObservacionesSocFC,
                                PERIODO = PeriodoSocFC,
                                ACTIVIDAD = DetalleActividad,
                                TIPO_PROGRAMA = DetalleActividad != null ? DetalleActividad.TIPO_PROGRAMA : null
                            });

                            AgregarFortNucleoFamiliar = false;
                        }

                        var Antiguo = SelectedFortGrupo;
                        if (ListFortalecimientoSocioFamComun.Remove(SelectedFortGrupo))
                        {
                            ListFortalecimientoSocioFamComun.Add(new PFC_VI_GRUPO
                            {
                                ID_ACTIVIDAD = Antiguo.ID_ACTIVIDAD,
                                ID_ANIO = SelectIngreso.ID_ANIO,
                                ID_CENTRO = SelectIngreso.ID_CENTRO,
                                ID_IMPUTADO = SelectIngreso.ID_IMPUTADO,
                                ID_INGRESO = SelectIngreso.ID_INGRESO,
                                ID_TIPO_PROGRAMA = Antiguo.ID_TIPO_PROGRAMA,
                                OBSERVACIONES = ObservacionesSocFC,
                                PERIODO = PeriodoSocFC,
                                ACTIVIDAD = Antiguo.ACTIVIDAD,
                                TIPO_PROGRAMA = Antiguo.TIPO_PROGRAMA
                            });
                        };

                        Antiguo = null;
                    }

                    SelectedGrupoSocioEconomico = SelectedFortGrupo = null;
                    PopUpsViewModels.ClosePopUp(PopUpsViewModels.TipoPopUp.AGREGAR_CONGREGACIONES_SOCIFAM_FC);

                    break;

                case "cancel_grupos_soc":
                    SelectedGrupoSocioEconomico = SelectedFortGrupo = null;
                    AgregarFortNucleoFamiliar = AgregarCongregacion = false;
                    LimpiaValidacionesCongreg();
                    PopUpsViewModels.ClosePopUp(PopUpsViewModels.TipoPopUp.AGREGAR_CONGREGACIONES_SOCIFAM_FC);
                    break;

                #endregion
                #region Area Tecnica Opiniones
                case "guardar_area_tec":
                    if (base.HasErrors)
                    {
                        Application.Current.Dispatcher.Invoke((System.Action)(delegate
                        {
                            (new Dialogos()).ConfirmacionDialogo("Advertencia!", base.Error);
                        }));
                        return;
                    }

                    if (SelectedAreTec == null)
                    {
                        if (LstAreasTec == null)
                            LstAreasTec = new ObservableCollection<PFF_ACTA_DETERMINO>();

                        LstAreasTec.Add(
                            new PFF_ACTA_DETERMINO
                            {
                                ID_ANIO = SelectIngreso.ID_ANIO,
                                ID_AREA_TECNICA = IdAreaT,
                                ID_CENTRO = SelectIngreso.ID_CENTRO,
                                ID_IMPUTADO = SelectIngreso.ID_IMPUTADO,
                                NOMBRE = NombreAreaMedica,
                                OPINION = OpinionAreaMedica,
                                AREA_TECNICA = SelArea
                            });
                    }

                    else
                    {
                        if (LstAreasTec.Remove(SelectedAreTec))
                        {
                            LstAreasTec.Add(
                            new PFF_ACTA_DETERMINO
                            {
                                ID_ANIO = SelectIngreso.ID_ANIO,
                                ID_AREA_TECNICA = IdAreaT,
                                ID_CENTRO = SelectIngreso.ID_CENTRO,
                                ID_IMPUTADO = SelectIngreso.ID_IMPUTADO,
                                NOMBRE = NombreAreaMedica,
                                OPINION = OpinionAreaMedica,
                                AREA_TECNICA = SelArea
                            });
                        };
                    }

                    SelectedAreTec = null;
                    PopUpsViewModels.ClosePopUp(PopUpsViewModels.TipoPopUp.AGREGAR_OPINION_AREA_TECNICA);
                    break;

                case "agregar_area_tec":
                    NombreAreaMedica = OpinionAreaMedica = string.Empty;
                    IdAreaT = -1;
                    ValidacionesOpinionArea();
                    PopUpsViewModels.ShowPopUp(this, PopUpsViewModels.TipoPopUp.AGREGAR_OPINION_AREA_TECNICA);
                    break;

                case "editar_area_tec":
                    if (SelectedAreTec != null)
                    {
                        NombreAreaMedica = SelectedAreTec.NOMBRE;
                        OpinionAreaMedica = SelectedAreTec.OPINION;
                        IdAreaT = SelectedAreTec.ID_AREA_TECNICA;
                        ValidacionesOpinionArea();
                        PopUpsViewModels.ShowPopUp(this, PopUpsViewModels.TipoPopUp.AGREGAR_OPINION_AREA_TECNICA);
                    }

                    else
                    {
                        new Dialogos().ConfirmacionDialogo("Validación", "Es necesario seleccionar una opinion");
                        return;
                    }
                    break;

                case "cancelar_area_tec":
                    NombreAreaMedica = OpinionAreaMedica = string.Empty;
                    IdAreaT = -1;
                    SelectedAreTec = null;
                    PopUpsViewModels.ClosePopUp(PopUpsViewModels.TipoPopUp.AGREGAR_OPINION_AREA_TECNICA);
                    break;
                #endregion

                case "editar_grupo":
                    EnabledEdicionProgPsicoComun = false;
                    if (SelectedPsicologicoGrupo != null)
                    {
                        LstActividadesNuevasEdicion = new ObservableCollection<ACTIVIDAD>(new cActividad().GetData(x => x.ID_TIPO_PROGRAMA == (short)eTiposP.PROGRAMA_DESHABITUAMIENTO));
                        LstActividadesNuevasEdicion.Insert(0, new ACTIVIDAD { DESCR = "SELECCIONE", ID_ACTIVIDAD = -1 });
                        DuracionrupoIV = SelectedPsicologicoGrupo.DURACION;
                        ObservacionesGrupoIV = SelectedPsicologicoGrupo.OBSERVACION;
                        ConcluidoGrupoIV = SelectedPsicologicoGrupo.CONCLUYO;
                        SelectedTipoPNuev = SelectedPsicologicoGrupo.ID_ACTIVIDAD;
                        ValidacionesGrupoIV();
                        PopUpsViewModels.ShowPopUp(this, PopUpsViewModels.TipoPopUp.AGREGAR_EDITAR_PROGRAMAS_ESTUDIO_PSICOLOGICO_FUERO_COMUN);
                    }
                    else
                        new Dialogos().ConfirmacionDialogo("Validación", "Es necesario seleccionar un grupo");

                    break;

                case "eliminar_grupo_uno":
                    if (SelectedPsicologicoGrupo == null)
                        new Dialogos().ConfirmacionDialogo("Validación", "Seleccione un grupo");
                    else
                        if (LstProgramasPsicologico != null && LstProgramasPsicologico.Any())
                            if (LstProgramasPsicologico.Remove(SelectedPsicologicoGrupo))
                                SelectedPsicologicoGrupo = null;

                    break;
                case "agregar_grupo_uno":
                    IdEdicionProgramasPsicologicosComunes = 1;
                    EnabledEdicionProgPsicoComun = true;
                    DuracionrupoIV = ObservacionesGrupoIV = ConcluidoGrupoIV = string.Empty;
                    ValidacionesGrupoIV();
                    LstActividadesNuevasEdicion = new ObservableCollection<ACTIVIDAD>(new cActividad().GetData(x => x.ID_TIPO_PROGRAMA == (short)eTiposP.PROGRAMA_DESHABITUAMIENTO));
                    LstActividadesNuevasEdicion.Insert(0, new ACTIVIDAD { DESCR = "SELECCIONE", ID_ACTIVIDAD = -1 });
                    SelectedTipoPNuev = -1;
                    PopUpsViewModels.ShowPopUp(this, PopUpsViewModels.TipoPopUp.AGREGAR_EDITAR_PROGRAMAS_ESTUDIO_PSICOLOGICO_FUERO_COMUN);
                    break;
                case "eliminar_grupo_dos":
                    if (SelectedPsicologicoGrupo == null)
                        new Dialogos().ConfirmacionDialogo("Validación", "Seleccione un grupo");
                    else
                        if (LstProgModifConduc != null && LstProgModifConduc.Any())
                            if (LstProgModifConduc.Remove(SelectedPsicologicoGrupo))
                                SelectedPsicologicoGrupo = null;

                    break;

                case "agregar_grupo_dos":
                    IdEdicionProgramasPsicologicosComunes = 2;
                    EnabledEdicionProgPsicoComun = true;
                    DuracionrupoIV = ObservacionesGrupoIV = ConcluidoGrupoIV = string.Empty;
                    ValidacionesGrupoIV();
                    LstActividadesNuevasEdicion = new ObservableCollection<ACTIVIDAD>(new cActividad().GetData(x => x.ID_TIPO_PROGRAMA == (short)eTiposP.MODIFICACION_CONDUCTA));
                    LstActividadesNuevasEdicion.Insert(0, new ACTIVIDAD { DESCR = "SELECCIONE", ID_ACTIVIDAD = -1 });
                    SelectedTipoPNuev = -1;
                    PopUpsViewModels.ShowPopUp(this, PopUpsViewModels.TipoPopUp.AGREGAR_EDITAR_PROGRAMAS_ESTUDIO_PSICOLOGICO_FUERO_COMUN);
                    break;

                case "editar_grupo_dos":
                    EnabledEdicionProgPsicoComun = false;
                    if (SelectedPsicologicoGrupo != null)
                    {
                        LstActividadesNuevasEdicion = new ObservableCollection<ACTIVIDAD>(new cActividad().GetData(x => x.ID_TIPO_PROGRAMA == (short)eTiposP.MODIFICACION_CONDUCTA));
                        LstActividadesNuevasEdicion.Insert(0, new ACTIVIDAD { DESCR = "SELECCIONE", ID_ACTIVIDAD = -1 });
                        DuracionrupoIV = SelectedPsicologicoGrupo.DURACION;
                        ObservacionesGrupoIV = SelectedPsicologicoGrupo.OBSERVACION;
                        ConcluidoGrupoIV = SelectedPsicologicoGrupo.CONCLUYO;
                        SelectedTipoPNuev = SelectedPsicologicoGrupo.ID_ACTIVIDAD;
                        ValidacionesGrupoIV();
                        PopUpsViewModels.ShowPopUp(this, PopUpsViewModels.TipoPopUp.AGREGAR_EDITAR_PROGRAMAS_ESTUDIO_PSICOLOGICO_FUERO_COMUN);
                    }
                    else
                        new Dialogos().ConfirmacionDialogo("Validación", "Es necesario seleccionar un grupo");

                    break;

                case "agregar_grupo_tres":
                    IdEdicionProgramasPsicologicosComunes = 3;
                    EnabledEdicionProgPsicoComun = true;
                    DuracionrupoIV = ObservacionesGrupoIV = ConcluidoGrupoIV = string.Empty;
                    ValidacionesGrupoIV();
                    LstActividadesNuevasEdicion = new ObservableCollection<ACTIVIDAD>(new cActividad().GetData(x => x.ID_TIPO_PROGRAMA == (short)eTiposP.COMPLEMENTARIO));
                    LstActividadesNuevasEdicion.Insert(0, new ACTIVIDAD { DESCR = "SELECCIONE", ID_ACTIVIDAD = -1 });
                    SelectedTipoPNuev = -1;
                    PopUpsViewModels.ShowPopUp(this, PopUpsViewModels.TipoPopUp.AGREGAR_EDITAR_PROGRAMAS_ESTUDIO_PSICOLOGICO_FUERO_COMUN);
                    break;

                case "editar_grupo_tres":
                    ValidacionesGrupoIV();
                    LstActividadesNuevasEdicion = new ObservableCollection<ACTIVIDAD>(new cActividad().GetData(x => x.ID_TIPO_PROGRAMA == (short)eTiposP.COMPLEMENTARIO));
                    LstActividadesNuevasEdicion.Insert(0, new ACTIVIDAD { DESCR = "SELECCIONE", ID_ACTIVIDAD = -1 });
                    DuracionrupoIV = SelectedPsicologicoGrupo.DURACION;
                    ObservacionesGrupoIV = SelectedPsicologicoGrupo.OBSERVACION;
                    ConcluidoGrupoIV = SelectedPsicologicoGrupo.CONCLUYO;
                    SelectedTipoPNuev = SelectedPsicologicoGrupo.ID_ACTIVIDAD;
                    PopUpsViewModels.ShowPopUp(this, PopUpsViewModels.TipoPopUp.AGREGAR_EDITAR_PROGRAMAS_ESTUDIO_PSICOLOGICO_FUERO_COMUN);
                    break;

                case "agregar_grupo_cuatro":
                    IdEdicionProgramasPsicologicosComunes = 4;
                    EnabledEdicionProgPsicoComun = true;
                    DuracionrupoIV = ObservacionesGrupoIV = ConcluidoGrupoIV = string.Empty;
                    ValidacionesGrupoIV();
                    LstActividadesNuevasEdicion = new ObservableCollection<ACTIVIDAD>(new cActividad().GetData(x => x.ID_TIPO_PROGRAMA == (short)eTiposP.TALLERES_ORIENTACION));
                    LstActividadesNuevasEdicion.Insert(0, new ACTIVIDAD { DESCR = "SELECCIONE", ID_ACTIVIDAD = -1 });
                    SelectedTipoPNuev = -1;
                    PopUpsViewModels.ShowPopUp(this, PopUpsViewModels.TipoPopUp.AGREGAR_EDITAR_PROGRAMAS_ESTUDIO_PSICOLOGICO_FUERO_COMUN);
                    break;

                case "editar_grupo_cuatro":
                    ValidacionesGrupoIV();
                    LstActividadesNuevasEdicion = new ObservableCollection<ACTIVIDAD>(new cActividad().GetData(x => x.ID_TIPO_PROGRAMA == (short)eTiposP.TALLERES_ORIENTACION));
                    LstActividadesNuevasEdicion.Insert(0, new ACTIVIDAD { DESCR = "SELECCIONE", ID_ACTIVIDAD = -1 });
                    DuracionrupoIV = SelectedPsicologicoGrupo.DURACION;
                    ObservacionesGrupoIV = SelectedPsicologicoGrupo.OBSERVACION;
                    ConcluidoGrupoIV = SelectedPsicologicoGrupo.CONCLUYO;
                    SelectedTipoPNuev = SelectedPsicologicoGrupo.ID_ACTIVIDAD;
                    PopUpsViewModels.ShowPopUp(this, PopUpsViewModels.TipoPopUp.AGREGAR_EDITAR_PROGRAMAS_ESTUDIO_PSICOLOGICO_FUERO_COMUN);
                    break;

                case "eliminar_grupo_tres":
                    if (SelectedPsicologicoGrupo == null)
                    {
                        new Dialogos().ConfirmacionDialogo("Validación", "Seleccione un grupo");
                        return;
                    }

                    else
                        if (LstComplement != null && LstComplement.Any())
                            if (LstComplement.Remove(SelectedPsicologicoGrupo))
                                SelectedPsicologicoGrupo = null;

                    break;

                case "eliminar_grupo_cuatro":
                    if (SelectedPsicologicoGrupo == null)
                    {
                        new Dialogos().ConfirmacionDialogo("Validación", "Seleccione un grupo");
                        return;
                    }

                    else
                        if (LstTalleresOrient != null && LstTalleresOrient.Any())
                            if (LstTalleresOrient.Remove(SelectedPsicologicoGrupo))
                                SelectedPsicologicoGrupo = null;

                    break;
                case "guardar_grupo_comun":
                    if (base.HasErrors)
                    {
                        Application.Current.Dispatcher.Invoke((System.Action)(delegate
                        {
                            (new Dialogos()).ConfirmacionDialogo("Advertencia!", base.Error);
                        }));
                        return;
                    }

                    if (IdEdicionProgramasPsicologicosComunes == 1)
                    {
                        if (LstProgramasPsicologico == null)
                            LstProgramasPsicologico = new ObservableCollection<PFC_IV_PROGRAMA>();

                        if (SelectedTipoPNuev == null)
                        {
                            new Dialogos().ConfirmacionDialogo("Validación", "Seleccione un programa o taller");
                            return;
                        };

                        if (SelectedTipoPNuev == -1)
                        {
                            new Dialogos().ConfirmacionDialogo("Validación", "Seleccione un programa o taller");
                            return;
                        };

                        if (LstProgramasPsicologico.Any(z => z.ID_ACTIVIDAD == SelectedTipoPNuev))
                        {
                            new Dialogos().ConfirmacionDialogo("Validación", "El grupo seleccionado ya existe");
                            return;
                        };

                        var DetalleActividad = new cActividad().GetData(x => x.ID_ACTIVIDAD == SelectedTipoPNuev && x.ID_TIPO_PROGRAMA == (short)eTiposP.PROGRAMA_DESHABITUAMIENTO).FirstOrDefault();

                        PFC_IV_PROGRAMA pro = new PFC_IV_PROGRAMA()
                        {
                            OBSERVACION = ObservacionesGrupoIV,
                            DURACION = DuracionrupoIV,
                            CONCLUYO = ConcluidoGrupoIV,
                            ID_ACTIVIDAD = SelectedTipoPNuev.HasValue ? SelectedTipoPNuev.Value : new short(),
                            ID_ANIO = selectIngreso.ID_ANIO,
                            ID_CENTRO = SelectIngreso.ID_CENTRO,
                            ID_IMPUTADO = SelectIngreso.ID_IMPUTADO,
                            ID_INGRESO = SelectIngreso.ID_INGRESO,
                            ID_TIPO_PROGRAMA = DetalleActividad != null ? DetalleActividad.ID_TIPO_PROGRAMA : new short(),
                            ACTIVIDAD = DetalleActividad,
                            TIPO_PROGRAMA = DetalleActividad != null ? DetalleActividad.TIPO_PROGRAMA : null
                            //ID_ESTUDIO = SelectedPsicologicoGrupo.ID_ESTUDIO
                        };

                        LstProgramasPsicologico.Add(pro);
                        SelectedPsicologicoGrupo = null;
                        IdEdicionProgramasPsicologicosComunes = new short();
                        PopUpsViewModels.ClosePopUp(PopUpsViewModels.TipoPopUp.AGREGAR_EDITAR_PROGRAMAS_ESTUDIO_PSICOLOGICO_FUERO_COMUN);
                        return;
                    };

                    if (IdEdicionProgramasPsicologicosComunes == 2)
                    {
                        if (LstProgModifConduc == null)
                            LstProgModifConduc = new ObservableCollection<PFC_IV_PROGRAMA>();

                        if (SelectedTipoPNuev == null)
                        {
                            new Dialogos().ConfirmacionDialogo("Validación", "Seleccione un programa o taller");
                            return;
                        };

                        if (SelectedTipoPNuev == -1)
                        {
                            new Dialogos().ConfirmacionDialogo("Validación", "Seleccione un programa o taller");
                            return;
                        };

                        if (LstProgModifConduc.Any(x => x.ID_ACTIVIDAD == SelectedTipoPNuev))
                        {
                            new Dialogos().ConfirmacionDialogo("Validación", "El grupo seleccionado ya existe");
                            return;
                        };

                        var DetalleActividad = new cActividad().GetData(x => x.ID_ACTIVIDAD == SelectedTipoPNuev && x.ID_TIPO_PROGRAMA == (short)eTiposP.MODIFICACION_CONDUCTA).FirstOrDefault();

                        PFC_IV_PROGRAMA pro = new PFC_IV_PROGRAMA()
                        {
                            OBSERVACION = ObservacionesGrupoIV,
                            DURACION = DuracionrupoIV,
                            CONCLUYO = ConcluidoGrupoIV,
                            ID_ACTIVIDAD = SelectedTipoPNuev.HasValue ? SelectedTipoPNuev.Value : new short(),
                            ID_ANIO = selectIngreso.ID_ANIO,
                            ID_CENTRO = SelectIngreso.ID_CENTRO,
                            ID_IMPUTADO = SelectIngreso.ID_IMPUTADO,
                            ID_INGRESO = SelectIngreso.ID_INGRESO,
                            ID_TIPO_PROGRAMA = DetalleActividad != null ? DetalleActividad.ID_TIPO_PROGRAMA : new short(),
                            ACTIVIDAD = DetalleActividad,
                            TIPO_PROGRAMA = DetalleActividad != null ? DetalleActividad.TIPO_PROGRAMA : null
                            //ID_ESTUDIO = SelectedPsicologicoGrupo.ID_ESTUDIO
                        };

                        LstProgModifConduc.Add(pro);
                        SelectedPsicologicoGrupo = null;
                        IdEdicionProgramasPsicologicosComunes = new short();
                        PopUpsViewModels.ClosePopUp(PopUpsViewModels.TipoPopUp.AGREGAR_EDITAR_PROGRAMAS_ESTUDIO_PSICOLOGICO_FUERO_COMUN);
                        return;
                    };

                    if (IdEdicionProgramasPsicologicosComunes == 3)
                    {
                        if (LstComplement == null)
                            LstComplement = new ObservableCollection<PFC_IV_PROGRAMA>();

                        if (SelectedTipoPNuev == null)
                        {
                            new Dialogos().ConfirmacionDialogo("Validación", "Seleccione un programa o taller");
                            return;
                        };

                        if (SelectedTipoPNuev == -1)
                        {
                            new Dialogos().ConfirmacionDialogo("Validación", "Seleccione un programa o taller");
                            return;
                        };

                        if (LstComplement.Any(x => x.ID_ACTIVIDAD == SelectedTipoPNuev))
                        {
                            new Dialogos().ConfirmacionDialogo("Validación", "El grupo seleccionado ya existe");
                            return;
                        };

                        var DetalleActividad = new cActividad().GetData(x => x.ID_ACTIVIDAD == SelectedTipoPNuev && x.ID_TIPO_PROGRAMA == (short)eTiposP.COMPLEMENTARIO).FirstOrDefault();

                        PFC_IV_PROGRAMA pro = new PFC_IV_PROGRAMA()
                        {
                            OBSERVACION = ObservacionesGrupoIV,
                            DURACION = DuracionrupoIV,
                            CONCLUYO = ConcluidoGrupoIV,
                            ID_ACTIVIDAD = SelectedTipoPNuev.HasValue ? SelectedTipoPNuev.Value : new short(),
                            ID_ANIO = selectIngreso.ID_ANIO,
                            ID_CENTRO = SelectIngreso.ID_CENTRO,
                            ID_IMPUTADO = SelectIngreso.ID_IMPUTADO,
                            ID_INGRESO = SelectIngreso.ID_INGRESO,
                            ID_TIPO_PROGRAMA = DetalleActividad != null ? DetalleActividad.ID_TIPO_PROGRAMA : new short(),
                            ACTIVIDAD = DetalleActividad,
                            TIPO_PROGRAMA = DetalleActividad != null ? DetalleActividad.TIPO_PROGRAMA : null
                            //ID_ESTUDIO = SelectedPsicologicoGrupo.ID_ESTUDIO
                        };

                        LstComplement.Add(pro);
                        SelectedPsicologicoGrupo = null;
                        IdEdicionProgramasPsicologicosComunes = new short();
                        PopUpsViewModels.ClosePopUp(PopUpsViewModels.TipoPopUp.AGREGAR_EDITAR_PROGRAMAS_ESTUDIO_PSICOLOGICO_FUERO_COMUN);
                        return;
                    };

                    if (IdEdicionProgramasPsicologicosComunes == 4)
                    {
                        if (LstTalleresOrient == null)
                            LstTalleresOrient = new ObservableCollection<PFC_IV_PROGRAMA>();

                        if (SelectedTipoPNuev == null)
                        {
                            new Dialogos().ConfirmacionDialogo("Validación", "Seleccione un programa o taller");
                            return;
                        };

                        if (SelectedTipoPNuev == -1)
                        {
                            new Dialogos().ConfirmacionDialogo("Validación", "Seleccione un programa o taller");
                            return;
                        };

                        if (LstTalleresOrient.Any(x => x.ID_ACTIVIDAD == SelectedTipoPNuev))
                        {
                            new Dialogos().ConfirmacionDialogo("Validación", "El grupo seleccionado ya existe");
                            return;
                        };

                        var DetalleActividad = new cActividad().GetData(x => x.ID_ACTIVIDAD == SelectedTipoPNuev && x.ID_TIPO_PROGRAMA == (short)eTiposP.TALLERES_ORIENTACION).FirstOrDefault();

                        PFC_IV_PROGRAMA pro = new PFC_IV_PROGRAMA()
                        {
                            OBSERVACION = ObservacionesGrupoIV,
                            DURACION = DuracionrupoIV,
                            CONCLUYO = ConcluidoGrupoIV,
                            ID_ACTIVIDAD = SelectedTipoPNuev.HasValue ? SelectedTipoPNuev.Value : new short(),
                            ID_ANIO = selectIngreso.ID_ANIO,
                            ID_CENTRO = SelectIngreso.ID_CENTRO,
                            ID_IMPUTADO = SelectIngreso.ID_IMPUTADO,
                            ID_INGRESO = SelectIngreso.ID_INGRESO,
                            ID_TIPO_PROGRAMA = DetalleActividad != null ? DetalleActividad.ID_TIPO_PROGRAMA : new short(),
                            ACTIVIDAD = DetalleActividad,
                            TIPO_PROGRAMA = DetalleActividad != null ? DetalleActividad.TIPO_PROGRAMA : null
                            //ID_ESTUDIO = SelectedPsicologicoGrupo.ID_ESTUDIO
                        };

                        LstTalleresOrient.Add(pro);
                        SelectedPsicologicoGrupo = null;
                        IdEdicionProgramasPsicologicosComunes = new short();
                        PopUpsViewModels.ClosePopUp(PopUpsViewModels.TipoPopUp.AGREGAR_EDITAR_PROGRAMAS_ESTUDIO_PSICOLOGICO_FUERO_COMUN);
                        return;
                    };

                    #region Edicion de grupos antes de ser guardados
                    if (LstProgramasPsicologico != null && LstProgramasPsicologico.Any())
                    {
                        var ListaUno = LstProgramasPsicologico.FirstOrDefault(x => x.ID_CONSEC == SelectedPsicologicoGrupo.ID_CONSEC && x.ID_TIPO_PROGRAMA == SelectedPsicologicoGrupo.ID_TIPO_PROGRAMA);
                        if (ListaUno != null)
                        {
                            var pro = new PFC_IV_PROGRAMA()
                            {
                                OBSERVACION = ObservacionesGrupoIV,
                                DURACION = DuracionrupoIV,
                                CONCLUYO = ConcluidoGrupoIV,
                                ID_ACTIVIDAD = SelectedPsicologicoGrupo.ID_ACTIVIDAD,
                                ID_ANIO = selectIngreso.ID_ANIO,
                                ID_CENTRO = SelectIngreso.ID_CENTRO,
                                ID_IMPUTADO = SelectIngreso.ID_IMPUTADO,
                                ID_INGRESO = SelectIngreso.ID_INGRESO,
                                ID_TIPO_PROGRAMA = SelectedPsicologicoGrupo.ID_TIPO_PROGRAMA,
                                ACTIVIDAD = SelectedPsicologicoGrupo.ACTIVIDAD,
                                TIPO_PROGRAMA = SelectedPsicologicoGrupo.TIPO_PROGRAMA,
                                ID_ESTUDIO = SelectedPsicologicoGrupo.ID_ESTUDIO
                            };

                            if (LstProgramasPsicologico.Remove(SelectedPsicologicoGrupo))
                                LstProgramasPsicologico.Add(pro);

                            SelectedPsicologicoGrupo = null;
                            PopUpsViewModels.ClosePopUp(PopUpsViewModels.TipoPopUp.AGREGAR_EDITAR_PROGRAMAS_ESTUDIO_PSICOLOGICO_FUERO_COMUN);
                            return;
                        }
                    }

                    if (LstProgModifConduc != null && LstProgModifConduc.Any())
                    {
                        var ListaDos = LstProgModifConduc.FirstOrDefault(x => x.ID_CONSEC == SelectedPsicologicoGrupo.ID_CONSEC && x.ID_TIPO_PROGRAMA == SelectedPsicologicoGrupo.ID_TIPO_PROGRAMA);
                        if (ListaDos != null)
                        {
                            var pro = new PFC_IV_PROGRAMA()
                            {
                                OBSERVACION = ObservacionesGrupoIV,
                                DURACION = DuracionrupoIV,
                                CONCLUYO = ConcluidoGrupoIV,
                                ID_ACTIVIDAD = SelectedPsicologicoGrupo.ID_ACTIVIDAD,
                                ID_ANIO = selectIngreso.ID_ANIO,
                                ID_CENTRO = SelectIngreso.ID_CENTRO,
                                ID_IMPUTADO = SelectIngreso.ID_IMPUTADO,
                                ID_INGRESO = SelectIngreso.ID_INGRESO,
                                ID_TIPO_PROGRAMA = SelectedPsicologicoGrupo.ID_TIPO_PROGRAMA,
                                ACTIVIDAD = SelectedPsicologicoGrupo.ACTIVIDAD,
                                TIPO_PROGRAMA = SelectedPsicologicoGrupo.TIPO_PROGRAMA,
                                ID_ESTUDIO = SelectedPsicologicoGrupo.ID_ESTUDIO
                            };

                            if (LstProgModifConduc.Remove(SelectedPsicologicoGrupo))
                                LstProgModifConduc.Add(pro);

                            SelectedPsicologicoGrupo = null;
                            PopUpsViewModels.ClosePopUp(PopUpsViewModels.TipoPopUp.AGREGAR_EDITAR_PROGRAMAS_ESTUDIO_PSICOLOGICO_FUERO_COMUN);
                            return;
                        }
                    }

                    if (LstComplement != null && LstComplement.Any())
                    {
                        var ListaTres = LstComplement.FirstOrDefault(x => x.ID_CONSEC == SelectedPsicologicoGrupo.ID_CONSEC && x.ID_TIPO_PROGRAMA == SelectedPsicologicoGrupo.ID_TIPO_PROGRAMA);
                        if (ListaTres != null)
                        {
                            var pro = new PFC_IV_PROGRAMA();
                            pro.OBSERVACION = ObservacionesGrupoIV;
                            pro.DURACION = DuracionrupoIV;
                            pro.CONCLUYO = ConcluidoGrupoIV;
                            pro.ID_ACTIVIDAD = SelectedPsicologicoGrupo.ID_ACTIVIDAD;
                            pro.ID_ANIO = selectIngreso.ID_ANIO;
                            pro.ID_CENTRO = SelectIngreso.ID_CENTRO;
                            pro.ID_IMPUTADO = SelectIngreso.ID_IMPUTADO;
                            pro.ID_INGRESO = SelectIngreso.ID_INGRESO;
                            pro.ID_TIPO_PROGRAMA = SelectedPsicologicoGrupo.ID_TIPO_PROGRAMA;
                            pro.ACTIVIDAD = SelectedPsicologicoGrupo.ACTIVIDAD;
                            pro.TIPO_PROGRAMA = SelectedPsicologicoGrupo.TIPO_PROGRAMA;
                            pro.ID_ESTUDIO = SelectedPsicologicoGrupo.ID_ESTUDIO;
                            if (LstComplement.Remove(SelectedPsicologicoGrupo))
                                LstComplement.Add(pro);

                            SelectedPsicologicoGrupo = null;
                            PopUpsViewModels.ClosePopUp(PopUpsViewModels.TipoPopUp.AGREGAR_EDITAR_PROGRAMAS_ESTUDIO_PSICOLOGICO_FUERO_COMUN);
                            return;
                        }
                    }

                    if (LstTalleresOrient != null && LstTalleresOrient.Any())
                    {
                        var ListaCuatro = LstTalleresOrient.FirstOrDefault(x => x.ID_CONSEC == SelectedPsicologicoGrupo.ID_CONSEC && x.ID_TIPO_PROGRAMA == SelectedPsicologicoGrupo.ID_TIPO_PROGRAMA);
                        if (ListaCuatro != null)
                        {
                            var pro = new PFC_IV_PROGRAMA();
                            pro.OBSERVACION = ObservacionesGrupoIV;
                            pro.DURACION = DuracionrupoIV;
                            pro.CONCLUYO = ConcluidoGrupoIV;
                            pro.ID_ACTIVIDAD = SelectedPsicologicoGrupo.ID_ACTIVIDAD;
                            pro.ID_ANIO = selectIngreso.ID_ANIO;
                            pro.ID_CENTRO = SelectIngreso.ID_CENTRO;
                            pro.ID_IMPUTADO = SelectIngreso.ID_IMPUTADO;
                            pro.ID_INGRESO = SelectIngreso.ID_INGRESO;
                            pro.ID_TIPO_PROGRAMA = SelectedPsicologicoGrupo.ID_TIPO_PROGRAMA;
                            pro.ACTIVIDAD = SelectedPsicologicoGrupo.ACTIVIDAD;
                            pro.TIPO_PROGRAMA = SelectedPsicologicoGrupo.TIPO_PROGRAMA;
                            pro.ID_ESTUDIO = SelectedPsicologicoGrupo.ID_ESTUDIO;
                            if (LstTalleresOrient.Remove(SelectedPsicologicoGrupo))
                                LstTalleresOrient.Add(pro);

                            SelectedPsicologicoGrupo = null;
                            PopUpsViewModels.ClosePopUp(PopUpsViewModels.TipoPopUp.AGREGAR_EDITAR_PROGRAMAS_ESTUDIO_PSICOLOGICO_FUERO_COMUN);
                            return;
                        }
                    }
                    #endregion
                    SelectedPsicologicoGrupo = null;///Limpia antes de salir para que sea necesario volver a seleccionar
                    PopUpsViewModels.ClosePopUp(PopUpsViewModels.TipoPopUp.AGREGAR_EDITAR_PROGRAMAS_ESTUDIO_PSICOLOGICO_FUERO_COMUN);
                    break;

                case "cancelar_grupo_comun":
                    LimpiaValidacionesGrupoIV();
                    SelectedPsicologicoGrupo = null;
                    PopUpsViewModels.ClosePopUp(PopUpsViewModels.TipoPopUp.AGREGAR_EDITAR_PROGRAMAS_ESTUDIO_PSICOLOGICO_FUERO_COMUN);
                    break;

                case "buscar_menu":
                    //IsOtraTarea = true;
                    //IsOtraTarea = true;
                    ListExpediente = new RangeEnabledObservableCollection<IMPUTADO>();
                    SelectExpediente = new IMPUTADO();
                    EmptyExpedienteVisible = true;
                    ApellidoPaternoBuscar = ApellidoMaternoBuscar = NombreBuscar = string.Empty;
                    FolioBuscar = AnioBuscar = null;
                    ImagenImputado = ImagenIngreso = new Imagenes().getImagenPerson();
                    PopUpsViewModels.ShowPopUp(this, PopUpsViewModels.TipoPopUp.BUSQUEDA);
                    break;

                case "limpiar_menu":
                    //IsOtraTarea = true;
                    ((System.Windows.Controls.ContentControl)PopUpsViewModels.MainWindow.FindName("contentControl")).Content = new RealizacionEstudiosPersonalidadView();
                    ((System.Windows.Controls.ContentControl)PopUpsViewModels.MainWindow.FindName("contentControl")).DataContext = new ControlPenales.RealizacionEstudiosViewModel();
                    break;

                case "nueva_busqueda":
                    ListExpediente = new RangeEnabledObservableCollection<IMPUTADO>();
                    SelectExpediente = new IMPUTADO();
                    EmptyExpedienteVisible = true;
                    ApellidoPaternoBuscar = ApellidoMaternoBuscar = NombreBuscar = string.Empty;
                    FolioBuscar = AnioBuscar = null;
                    ImagenImputado = ImagenIngreso = new Imagenes().getImagenPerson();
                    break;

                #region Integrante Fam Trabajo Social Federal
                case "agregar_int_gpo_tec":
                    NombreIntegranteTSFF = EdadIntegranteTSFF = ParentescoIntegranteTSFF = EdoCivilIntegranteTSFF = OcupacionIntegranteTSFF = string.Empty;
                    IdEscIntegranteTSFF = -1;
                    ValidacionesGrupoFamFF();
                    //LoadListasFederales();
                    _IdTipoGrupo = (short)eTipopGrupoTrabajoSocial.PRIMARIO;
                    PopUpsViewModels.ShowPopUp(this, PopUpsViewModels.TipoPopUp.AGREGAR_INTEGRANTE_GRUPO_FAMILIAR_TRABAJO_SOCIAL_FUERO_FEDERAL);
                    break;

                case "guardar_int_grupo_fam":
                    if (base.HasErrors)
                    {
                        Application.Current.Dispatcher.Invoke((System.Action)(delegate
                        {
                            (new Dialogos()).ConfirmacionDialogo("Advertencia!", base.Error);
                        }));
                        return;
                    }

                    if (_IdTipoGrupo == (short)eTipopGrupoTrabajoSocial.PRIMARIO)
                    {
                        if (LstGrupoFam == null)
                            LstGrupoFam = new ObservableCollection<PFF_GRUPO_FAMILIAR>();

                        if (SelectedGrupoFamTSFF == null)
                        {
                            LstGrupoFam.Add(new PFF_GRUPO_FAMILIAR
                            {
                                EDAD = EdadIntegranteTSFF,
                                ESTADO_CIVIL = EdoCivilIntegranteTSFF,
                                ID_ANIO = SelectIngreso.ID_ANIO,
                                ID_CENTRO = SelectIngreso.ID_CENTRO,
                                ID_GRUPO_FAMILIAR = (short)eTipopGrupoTrabajoSocial.PRIMARIO,
                                ID_IMPUTADO = SelectIngreso.ID_IMPUTADO,
                                OCUPACION = OcupacionIntegranteTSFF,
                                PARENTESCO = ParentescoIntegranteTSFF,
                                NOMBRE = NombreIntegranteTSFF,
                                ID_ESTUDIO = IdEscIntegranteTSFF,
                                ID_INGRESO = SelectIngreso.ID_INGRESO
                            });
                        }

                        else
                        {
                            if (LstGrupoFam.Remove(SelectedGrupoFamTSFF))
                            {
                                LstGrupoFam.Add(new PFF_GRUPO_FAMILIAR
                                    {
                                        EDAD = EdadIntegranteTSFF,
                                        ESTADO_CIVIL = EdoCivilIntegranteTSFF,
                                        ID_ANIO = SelectIngreso.ID_ANIO,
                                        ID_CENTRO = SelectIngreso.ID_CENTRO,
                                        ID_GRUPO_FAMILIAR = (short)eTipopGrupoTrabajoSocial.PRIMARIO,
                                        ID_IMPUTADO = SelectIngreso.ID_IMPUTADO,
                                        OCUPACION = OcupacionIntegranteTSFF,
                                        PARENTESCO = ParentescoIntegranteTSFF,
                                        NOMBRE = NombreIntegranteTSFF,
                                        ID_ESTUDIO = IdEscIntegranteTSFF,
                                        ID_INGRESO = SelectIngreso.ID_INGRESO
                                    });
                            };
                        };
                    };

                    if (_IdTipoGrupo == (short)eTipopGrupoTrabajoSocial.SECUNDARIO)
                    {
                        if (LstGrupoFamSecu == null)
                            LstGrupoFamSecu = new ObservableCollection<PFF_GRUPO_FAMILIAR>();

                        if (SelectedGrupoFamFFSec == null)
                        {
                            LstGrupoFamSecu.Add(new PFF_GRUPO_FAMILIAR
                            {
                                EDAD = EdadIntegranteTSFF,
                                ESTADO_CIVIL = EdoCivilIntegranteTSFF,
                                ID_ANIO = SelectIngreso.ID_ANIO,
                                ID_CENTRO = SelectIngreso.ID_CENTRO,
                                ID_GRUPO_FAMILIAR = (short)eTipopGrupoTrabajoSocial.SECUNDARIO,
                                ID_IMPUTADO = SelectIngreso.ID_IMPUTADO,
                                OCUPACION = OcupacionIntegranteTSFF,
                                PARENTESCO = ParentescoIntegranteTSFF,
                                NOMBRE = NombreIntegranteTSFF,
                                ID_ESTUDIO = IdEscIntegranteTSFF,
                                ID_INGRESO = SelectIngreso.ID_INGRESO
                            });
                        }
                        else
                        {
                            if (LstGrupoFamSecu.Remove(SelectedGrupoFamFFSec))
                            {
                                LstGrupoFamSecu.Add(new PFF_GRUPO_FAMILIAR
                                    {
                                        EDAD = EdadIntegranteTSFF,
                                        ESTADO_CIVIL = EdoCivilIntegranteTSFF,
                                        ID_ANIO = SelectIngreso.ID_ANIO,
                                        ID_CENTRO = SelectIngreso.ID_CENTRO,
                                        ID_GRUPO_FAMILIAR = (short)eTipopGrupoTrabajoSocial.SECUNDARIO,
                                        ID_IMPUTADO = SelectIngreso.ID_IMPUTADO,
                                        OCUPACION = OcupacionIntegranteTSFF,
                                        PARENTESCO = ParentescoIntegranteTSFF,
                                        NOMBRE = NombreIntegranteTSFF,
                                        ID_ESTUDIO = IdEscIntegranteTSFF,
                                        ID_INGRESO = SelectIngreso.ID_INGRESO
                                    });
                            };
                        };
                    };

                    SelectedGrupoFamTSFF = SelectedGrupoFamFFSec = null;
                    PopUpsViewModels.ClosePopUp(PopUpsViewModels.TipoPopUp.AGREGAR_INTEGRANTE_GRUPO_FAMILIAR_TRABAJO_SOCIAL_FUERO_FEDERAL);
                    break;

                case "cancelar_int_grupo_fam":
                    SelectedGrupoFamTSFF = SelectedGrupoFamFFSec = null;
                    PopUpsViewModels.ClosePopUp(PopUpsViewModels.TipoPopUp.AGREGAR_INTEGRANTE_GRUPO_FAMILIAR_TRABAJO_SOCIAL_FUERO_FEDERAL);
                    break;

                case "agregar_int_gpo_tec_sec":
                    NombreIntegranteTSFF = EdadIntegranteTSFF = ParentescoIntegranteTSFF = EdoCivilIntegranteTSFF = OcupacionIntegranteTSFF = string.Empty;
                    IdEscIntegranteTSFF = -1;
                    ValidacionesGrupoFamFF();
                    LoadListasFederales();
                    _IdTipoGrupo = (short)eTipopGrupoTrabajoSocial.SECUNDARIO;
                    PopUpsViewModels.ShowPopUp(this, PopUpsViewModels.TipoPopUp.AGREGAR_INTEGRANTE_GRUPO_FAMILIAR_TRABAJO_SOCIAL_FUERO_FEDERAL);
                    break;

                case "editar_int_gpo_tec_sec":
                    if (SelectedGrupoFamFFSec != null)
                    {
                        NombreIntegranteTSFF = SelectedGrupoFamFFSec.NOMBRE;
                        EdadIntegranteTSFF = SelectedGrupoFamFFSec.EDAD;
                        ParentescoIntegranteTSFF = SelectedGrupoFamFFSec.PARENTESCO;
                        EdoCivilIntegranteTSFF = SelectedGrupoFamFFSec.ESTADO_CIVIL;
                        OcupacionIntegranteTSFF = SelectedGrupoFamFFSec.OCUPACION;
                        IdEscIntegranteTSFF = SelectedGrupoFamFFSec.ID_ESTUDIO;
                        ValidacionesGrupoFamFF();
                        LoadListasFederales();
                        _IdTipoGrupo = (short)eTipopGrupoTrabajoSocial.SECUNDARIO;
                        PopUpsViewModels.ShowPopUp(this, PopUpsViewModels.TipoPopUp.AGREGAR_INTEGRANTE_GRUPO_FAMILIAR_TRABAJO_SOCIAL_FUERO_FEDERAL);
                    }
                    break;

                case "eliminar_int_gpo_tec_sec":
                    if (SelectedGrupoFamFFSec != null)
                        if (LstGrupoFamSecu != null && LstGrupoFamSecu.Any())
                            if (LstGrupoFamSecu.Remove(SelectedGrupoFamFFSec))
                                SelectedGrupoFamFFSec = null;

                    _IdTipoGrupo = (short)eTipopGrupoTrabajoSocial.SECUNDARIO;
                    break;

                case "editar_int_gpo_tec":
                    if (SelectedGrupoFamTSFF != null)
                    {
                        NombreIntegranteTSFF = SelectedGrupoFamTSFF.NOMBRE;
                        EdadIntegranteTSFF = SelectedGrupoFamTSFF.EDAD;
                        ParentescoIntegranteTSFF = SelectedGrupoFamTSFF.PARENTESCO;
                        EdoCivilIntegranteTSFF = SelectedGrupoFamTSFF.ESTADO_CIVIL;
                        OcupacionIntegranteTSFF = SelectedGrupoFamTSFF.OCUPACION;
                        IdEscIntegranteTSFF = SelectedGrupoFamTSFF.ID_ESTUDIO;
                        ValidacionesGrupoFamFF();
                        LoadListasFederales();
                        _IdTipoGrupo = (short)eTipopGrupoTrabajoSocial.PRIMARIO;
                        PopUpsViewModels.ShowPopUp(this, PopUpsViewModels.TipoPopUp.AGREGAR_INTEGRANTE_GRUPO_FAMILIAR_TRABAJO_SOCIAL_FUERO_FEDERAL);
                    }
                    break;

                case "eliminar_int_gpo_tec":
                    if (SelectedGrupoFamTSFF != null)
                        if (LstGrupoFam != null && LstGrupoFam.Any())
                            if (LstGrupoFam.Remove(SelectedGrupoFamTSFF))
                                SelectedGrupoFamTSFF = null;

                    _IdTipoGrupo = (short)eTipopGrupoTrabajoSocial.PRIMARIO;
                    break;

                #endregion

                case "salir_menu":
                    //IsOtraTarea = true;
                    PrincipalViewModel.SalirMenu();
                    break;

                case "ficha_menu":
                    //IsOtraTarea = true;
                    break;

                case "buscar_salir":
                    StaticSourcesViewModel.SourceChanged = false;
                    ImagenImputado = ImagenIngreso = new Imagenes().getImagenPerson();
                    PopUpsViewModels.ClosePopUp(PopUpsViewModels.TipoPopUp.BUSQUEDA);
                    break;

                case "buscar_seleccionar":
                    if (SelectIngreso == null)
                    {
                        new Dialogos().ConfirmacionDialogo("Validación", "Favor de seleccionar un ingreso vigente");
                        return;
                    }

                    foreach (var item in Parametro.ESTATUS_ADMINISTRATIVO_INACT)
                    {
                        if (SelectIngreso.ID_ESTATUS_ADMINISTRATIVO == item)
                        {
                            new Dialogos().ConfirmacionDialogo("Notificación!", "No se encontró ningún ingreso activo en este imputado.");
                            return;
                        }
                    }

                    if (SelectIngreso.ID_UB_CENTRO.HasValue && SelectIngreso.ID_UB_CENTRO != GlobalVar.gCentro)
                    {
                        new Dialogos().ConfirmacionDialogo("Validación", "El ingreso seleccionado no pertenece a su centro.");
                        return;
                    }
                    var toleranciaTraslado = Parametro.TOLERANCIA_TRASLADO_EDIFICIO;
                    if (SelectIngreso.TRASLADO_DETALLE.Any(a => (a.ID_ESTATUS != "CA" ? a.TRASLADO.ORIGEN_TIPO != "F" : false) && a.TRASLADO.TRASLADO_FEC.AddHours(-toleranciaTraslado) <= Fechas.GetFechaDateServer))
                    {
                        new Dialogos().ConfirmacionDialogo("Notificación!", "El interno [" + SelectIngreso.ID_ANIO.ToString() + "/" +
                            SelectIngreso.ID_IMPUTADO.ToString() + "] tiene un traslado proximo y no tiene permitido ningun cambio de informacion.");
                        return;
                    }

                    SelectedInterno = SelectIngreso.IMPUTADO;
                    if (SelectIngreso != null)
                        SelectIngreso = SelectIngreso;

                    PopUpsViewModels.ClosePopUp(PopUpsViewModels.TipoPopUp.BUSQUEDA);
                    SeleccionaIngreso();
                    StaticSourcesViewModel.SourceChanged = false;
                    break;

                case "guardar_menu":
                    //IsOtraTarea = true;
                    if (SelectIngreso != null)
                    {
                        if (ValidaEstudioPadre())
                        {
                            if (!base.HasErrors)
                            {
                                if (lstRoles != null)
                                {
                                    var _EstudioPdre = new cEstudioPersonalidad().GetData(x => x.ID_INGRESO == SelectIngreso.ID_INGRESO && x.ID_IMPUTADO == SelectIngreso.ID_IMPUTADO && x.ID_CENTRO == SelectIngreso.ID_CENTRO && x.ID_ANIO == SelectIngreso.ID_ANIO).OrderByDescending(x => x.ID_ESTUDIO).FirstOrDefault();
                                    if (_EstudioPdre == null)
                                    {
                                        (new Dialogos()).ConfirmacionDialogo("Validación", "Verifique el estado del estudio de personalidad");
                                        //complemento
                                        StaticSourcesViewModel.SourceChanged = false;

                                        return;
                                    };

                                    var _nuevo = new PERSONALIDAD_FUERO_COMUN()
                                    {
                                        ID_ANIO = SelectIngreso.ID_ANIO,
                                        ID_CENTRO = SelectIngreso.ID_CENTRO,
                                        ID_IMPUTADO = SelectIngreso.ID_IMPUTADO,
                                        ID_INGRESO = SelectIngreso.ID_INGRESO,
                                        ID_ESTUDIO = _EstudioPdre.ID_ESTUDIO
                                    };

                                    var _NuevoFederal = new PERSONALIDAD_FUERO_FEDERAL()
                                    {
                                        ID_ANIO = SelectIngreso.ID_ANIO,
                                        ID_CENTRO = SelectIngreso.ID_CENTRO,
                                        ID_IMPUTADO = SelectIngreso.ID_IMPUTADO,
                                        ID_INGRESO = SelectIngreso.ID_INGRESO,
                                        ID_ESTUDIO = _EstudioPdre.ID_ESTUDIO
                                    };

                                    if (!string.IsNullOrEmpty(FueroImputado))
                                    {
                                        if (FueroImputado == "C")
                                        {
                                            _NuevoFederal = null;
                                            foreach (var item in lstRoles)
                                            {
                                                switch (item)
                                                {
                                                    case (short)eRolesContemplados.COMANDANCIA:
                                                        if (EnabledComun8)
                                                            _nuevo.PFC_IX_SEGURIDAD = EstudioSeguridadTransaccionIndividual(_EstudioPdre);
                                                        break;
                                                    case (short)eRolesContemplados.CRIMINOLOGO:
                                                        if (EnabledComun4)
                                                            _nuevo.PFC_V_CRIMINODIAGNOSTICO = EstudioCriminodTransaccionIndividual(_EstudioPdre);
                                                        break;
                                                    case (short)eRolesContemplados.EDUCATIVO:
                                                        if (EnabledComun6)
                                                            _nuevo.PFC_VII_EDUCATIVO = EstudioEducativoTransaccionIndividual(_EstudioPdre);
                                                        break;
                                                    case (short)eRolesContemplados.MEDICO:
                                                        if (EnabledComun1)
                                                            _nuevo.PFC_II_MEDICO = EstudioMedicoTransaccionIndividual(_EstudioPdre);
                                                        break;
                                                    case (short)eRolesContemplados.PROGRAMAS:
                                                        if (EnabledComun7)
                                                            _nuevo.PFC_VIII_TRABAJO = EstudioTrabajoTransaccionIndividual(_EstudioPdre);
                                                        break;
                                                    case (short)eRolesContemplados.PSICOLOGO:
                                                        if (EnabledComun3)
                                                            _nuevo.PFC_IV_PSICOLOGICO = EstudioPsicologicoTransaccionIndividual(_EstudioPdre);
                                                        break;
                                                    case (short)eRolesContemplados.PSIQUIATRA:
                                                        if (EnabledComun2)
                                                            _nuevo.PFC_III_PSIQUIATRICO = EstudioPsiquiatricoTransaccionIndividual(_EstudioPdre);
                                                        break;
                                                    case (short)eRolesContemplados.TRABAJO_SOCIAL:
                                                        if (EnabledComun5)
                                                            _nuevo.PFC_VI_SOCIO_FAMILIAR = EstudioSocioFamiliarTransaccionIndividual(_EstudioPdre);
                                                        break;
                                                    default:
                                                        //no action
                                                        break;
                                                }
                                            }
                                        }

                                        if (FueroImputado == "F")
                                        {
                                            _nuevo = null;
                                            foreach (var item in lstRoles)
                                            {
                                                switch (item)
                                                {
                                                    case (short)eRolesContemplados.COMANDANCIA:
                                                        if (EnabledFederal6)
                                                            _NuevoFederal.PFF_VIGILANCIA = GuardadoVigilanciaFueroFederal();
                                                        break;
                                                    case (short)eRolesContemplados.CRIMINOLOGO:
                                                        if (EnabledFederal7)
                                                            _NuevoFederal.PFF_CRIMINOLOGICO = GuardadoCriminologicoFueroFederal();
                                                        break;
                                                    case (short)eRolesContemplados.EDUCATIVO:
                                                        if (EnabledFederal5)
                                                            _NuevoFederal.PFF_ACTIVIDAD = GuardadoActividadesEducFueroFederal();
                                                        break;
                                                    case (short)eRolesContemplados.MEDICO:
                                                        if (EnabledFederal1)
                                                            _NuevoFederal.PFF_ESTUDIO_MEDICO = GuardadoEstudioMedicoFederal();
                                                        break;
                                                    case (short)eRolesContemplados.PROGRAMAS:
                                                        if (EnabledFederal4)
                                                            _NuevoFederal.PFF_CAPACITACION = GuardadoCapacitacionFueroFederal();
                                                        break;
                                                    case (short)eRolesContemplados.PSICOLOGO:
                                                        if (EnabledFederal2)
                                                            _NuevoFederal.PFF_ESTUDIO_PSICOLOGICO = GuardadoPsicologicoFueroFederal();
                                                        break;
                                                    case (short)eRolesContemplados.TRABAJO_SOCIAL:
                                                        if (EnabledFederal3)
                                                            _NuevoFederal.PFF_TRABAJO_SOCIAL = GuardadoTrabajoSocialFueroFederal();
                                                        break;
                                                    default:
                                                        //no action
                                                        break;
                                                }
                                            }
                                        }

                                        if (await StaticSourcesViewModel.OperacionesAsync<bool>("Ingresando Estudio de Personalidad", () => new cRealizacionEstudiosPersonalidad().IngresaEstudioConHijos(_nuevo, _NuevoFederal)))
                                        {
                                            (new Dialogos()).ConfirmacionDialogo("EXITO", "Se ha registrado el estudio de personalidad con exito");
                                        }

                                        else
                                        {
                                            (new Dialogos()).ConfirmacionDialogo("Error", "Surgió un error al ingresar el estudio de personalidad");
                                        }
                                    }

                                    else
                                        (new Dialogos()).ConfirmacionDialogo("Validación", "Verifique el fuero del imputado seleccionado");
                                }
                                else
                                    (new Dialogos()).ConfirmacionDialogo("Validación", "Verifique el rol del usuario actual");
                            }

                            else
                            {
                                Application.Current.Dispatcher.Invoke((System.Action)(delegate
                                {
                                    (new Dialogos()).ConfirmacionDialogo("Advertencia!", base.Error);
                                }));
                                return;
                            }
                        }
                    }
                    else
                    {
                        new Dialogos().ConfirmacionDialogo("Validación", "Favor de seleccionar un ingreso vigente");
                        return;
                    }

                    break;

                case "reporte_menu":
                    //IsOtraTarea = true;
                    if (SelectIngreso != null)
                    {
                        var _EstudioPorHacer = new cEstudioPersonalidad().GetData(x => x.ID_INGRESO == SelectIngreso.ID_INGRESO && x.ID_IMPUTADO == SelectIngreso.ID_IMPUTADO && x.ID_CENTRO == SelectIngreso.ID_CENTRO && x.ID_ANIO == SelectIngreso.ID_ANIO).OrderByDescending(x => x.ID_ESTUDIO).FirstOrDefault();
                        if (_EstudioPorHacer != null)
                            Imprimir(_EstudioPorHacer);
                    }

                    else
                        new Dialogos().ConfirmacionDialogo("Validación", "Favor de seleccionar un ingreso vigente");

                    break;

                #region Dias Laborados de fuero federal
                case "guardar_dias_lab":
                    if (base.HasErrors)
                    {
                        Application.Current.Dispatcher.Invoke((System.Action)(delegate
                        {
                            (new Dialogos()).ConfirmacionDialogo("Advertencia!", base.Error);
                        }));
                        return;
                    }

                    if (LstDiasLaborados == null)
                        LstDiasLaborados = new ObservableCollection<PFF_DIAS_LABORADO>();

                    ProcesaDias(DiasL, TotalPreliminar);
                    if (SelectedDiaLab == null)
                    {
                        LstDiasLaborados.Add(new PFF_DIAS_LABORADO
                            {
                                ANIO = AnioDias,
                                DIAS_TRABAJADOS = DiasL,
                                ID_ANIO = SelectIngreso.ID_ANIO,
                                ID_CENTRO = SelectIngreso.ID_CENTRO,
                                ID_IMPUTADO = SelectIngreso.ID_IMPUTADO,
                                ID_INGRESO = SelectIngreso.ID_INGRESO,
                                MES = MesDias
                            });
                    }

                    else
                    {
                        if (LstDiasLaborados.Any(c => c.ANIO == AnioDias && c.DIAS_TRABAJADOS == DiasL && c.MES == MesDias))
                        {
                            if (LstDiasLaborados.Remove(SelectedDiaLab))
                            {
                                LstDiasLaborados.Add(new PFF_DIAS_LABORADO
                                {
                                    ANIO = AnioDias,
                                    DIAS_TRABAJADOS = DiasL,
                                    ID_ANIO = SelectIngreso.ID_ANIO,
                                    ID_CENTRO = SelectIngreso.ID_CENTRO,
                                    ID_IMPUTADO = SelectIngreso.ID_IMPUTADO,
                                    ID_INGRESO = SelectIngreso.ID_INGRESO,
                                    MES = MesDias
                                });
                            };
                        }
                        else
                        {
                            LstDiasLaborados.Add(new PFF_DIAS_LABORADO
                            {
                                ANIO = AnioDias,
                                DIAS_TRABAJADOS = DiasL,
                                ID_ANIO = SelectIngreso.ID_ANIO,
                                ID_CENTRO = SelectIngreso.ID_CENTRO,
                                ID_IMPUTADO = SelectIngreso.ID_IMPUTADO,
                                ID_INGRESO = SelectIngreso.ID_INGRESO,
                                MES = MesDias
                            });
                        }

                    };

                    SelectedDiaLab = null;
                    PopUpsViewModels.ClosePopUp(PopUpsViewModels.TipoPopUp.AGREGAR_DIAS_LABORADOS_FUERO_FEDERAL);
                    break;

                case "cancelar_dias_lab":
                    SelectedDiaLab = null;
                    TotalPreliminar = 0;
                    PopUpsViewModels.ClosePopUp(PopUpsViewModels.TipoPopUp.AGREGAR_DIAS_LABORADOS_FUERO_FEDERAL);
                    break;

                case "agregar_dias_lab":
                    AnioDias = 0;
                    MesDias = -1;
                    DiasL = 0;
                    TotalPreliminar = 0;
                    ValidacionDiasLab();
                    PopUpsViewModels.ShowPopUp(this, PopUpsViewModels.TipoPopUp.AGREGAR_DIAS_LABORADOS_FUERO_FEDERAL);
                    break;

                case "editar_dias_lab":
                    if (SelectedDiaLab != null)
                    {
                        TotalPreliminar = 0;
                        MesDias = SelectedDiaLab.MES;
                        AnioDias = SelectedDiaLab.ANIO;
                        DiasL = SelectedDiaLab.DIAS_TRABAJADOS;
                        TotalPreliminar = SelectedDiaLab.DIAS_TRABAJADOS;//cantidad de dias antes de procesar
                        ValidacionDiasLab();
                        PopUpsViewModels.ShowPopUp(this, PopUpsViewModels.TipoPopUp.AGREGAR_DIAS_LABORADOS_FUERO_FEDERAL);
                    }
                    break;

                case "eliminar_dias_lab":
                    var _t = SelectedDiaLab;
                    if (SelectedDiaLab != null)
                        if (LstDiasLaborados != null && LstDiasLaborados.Any())
                            if (LstDiasLaborados.Remove(SelectedDiaLab))
                                DiasLaboradosEfectivos = ((short)(DiasLaboradosEfectivos - _t.DIAS_TRABAJADOS));
                    break;
                #endregion
                #region Actividades Educativas Fuero Fedral

                case "insertar_prog_activ_edu":
                    EnabledActividadesTipoPFederal = true;
                    ListProgramas = new ObservableCollection<TIPO_PROGRAMA>(new cTipoPrograma().GetData(x => x.ESTATUS == "S"));
                    ListProgramas.Insert(0, new TIPO_PROGRAMA { NOMBRE = "SELECCIONE", ID_TIPO_PROGRAMA = -1 });
                    FecInicioProg = FecFinProg = Fechas.GetFechaDateServer;
                    Participo = string.Empty;
                    ValidacionesEducFF();
                    IdTipoP = -1;
                    PopUpsViewModels.ShowPopUp(this, PopUpsViewModels.TipoPopUp.AGREGAR_EDUC_CUL_FUERO_FEDERAL);
                    break;

                case "cancelar_prog_activ_edu":
                    EnabledActividadesTipoPFederal = true;
                    LimpiarValidacionesEducFF();
                    SelectedActivPart = null;
                    PopUpsViewModels.ClosePopUp(PopUpsViewModels.TipoPopUp.AGREGAR_EDUC_CUL_FUERO_FEDERAL);
                    break;

                case "editar_prog_activ_edu":
                    EnabledActividadesTipoPFederal = false;
                    if (SelectedActivPart != null)
                    {
                        IdTipoP = SelectedActivPart.ID_TIPO_PROGRAMA;
                        FecInicioProg = SelectedActivPart.FECHA_1;
                        FecFinProg = SelectedActivPart.FECHA_2;
                        Participo = SelectedActivPart.PARTICIPACION;
                        ValidacionesEducFF();
                        PopUpsViewModels.ShowPopUp(this, PopUpsViewModels.TipoPopUp.AGREGAR_EDUC_CUL_FUERO_FEDERAL);
                    }
                    break;

                case "guardar_prog_activ_edu":
                    EnabledActividadesTipoPFederal = true;
                    if (base.HasErrors)
                    {
                        Application.Current.Dispatcher.Invoke((System.Action)(delegate
                        {
                            (new Dialogos()).ConfirmacionDialogo("Advertencia!", base.Error);
                        }));
                        return;
                    }

                    if (LstActividadPart == null)
                        LstActividadPart = new ObservableCollection<PFF_ACTIVIDAD_PARTICIPACION>();

                    if (SelectedActivPart == null)
                    {
                        if (LstActividadPart.Any(x => x.ID_TIPO_PROGRAMA == IdTipoP))
                        {
                            new Dialogos().ConfirmacionDialogo("Validación", "La participacion seleccionada ya existe");
                            return;
                        };

                        var _detallesTipoPrograma = new cTipoPrograma().GetData(x => x.ID_TIPO_PROGRAMA == IdTipoP).FirstOrDefault();
                        LstActividadPart.Add(new PFF_ACTIVIDAD_PARTICIPACION
                        {
                            FECHA_1 = FecInicioProg,
                            FECHA_2 = FecFinProg,
                            ID_ANIO = SelectIngreso.ID_ANIO,
                            ID_CENTRO = SelectIngreso.ID_CENTRO,
                            ID_IMPUTADO = SelectIngreso.ID_IMPUTADO,
                            ID_INGRESO = SelectIngreso.ID_INGRESO,
                            ID_TIPO_PROGRAMA = IdTipoP,
                            TIPO_PROGRAMA = _detallesTipoPrograma,
                            OTRO_ESPECIFICAR = "_",
                            PARTICIPACION = Participo
                        });
                    }

                    else
                    {
                        var _detallesTipoPrograma = new cTipoPrograma().GetData(x => x.ID_TIPO_PROGRAMA == IdTipoP).FirstOrDefault();
                        if (LstActividadPart.Remove(SelectedActivPart))
                        {
                            LstActividadPart.Add(new PFF_ACTIVIDAD_PARTICIPACION
                            {
                                FECHA_1 = FecInicioProg,
                                FECHA_2 = FecFinProg,
                                TIPO_PROGRAMA = _detallesTipoPrograma,
                                ID_ANIO = SelectIngreso.ID_ANIO,
                                ID_CENTRO = SelectIngreso.ID_CENTRO,
                                ID_IMPUTADO = SelectIngreso.ID_IMPUTADO,
                                ID_INGRESO = SelectIngreso.ID_INGRESO,
                                ID_TIPO_PROGRAMA = IdTipoP,
                                OTRO_ESPECIFICAR = "_",
                                PARTICIPACION = Participo
                            });
                        };
                    };

                    SelectedActivPart = null;
                    PopUpsViewModels.ClosePopUp(PopUpsViewModels.TipoPopUp.AGREGAR_EDUC_CUL_FUERO_FEDERAL);
                    break;

                #endregion

                #region Sanciones Vigilancia Federal

                case "agregar_sanc_ff":
                    FecVigiSancFF = Fechas.GetFechaDateServer;
                    MotivoSancFF = ResolucionSancFF = string.Empty;
                    ValidacionesSancionInformeFederal();
                    PopUpsViewModels.ShowPopUp(this, PopUpsViewModels.TipoPopUp.SANCIONES_INFORME_VIGILANCIA_FUERO_FEDERAL);
                    break;

                case "editar_sanc_ff":
                    if (SeleCorrecFF != null)
                    {
                        FecVigiSancFF = SeleCorrecFF.FECHA;
                        MotivoSancFF = SeleCorrecFF.MOTIVO;
                        ResolucionSancFF = SeleCorrecFF.RESOLUCION;
                        ValidacionesSancionInformeFederal();
                        PopUpsViewModels.ShowPopUp(this, PopUpsViewModels.TipoPopUp.SANCIONES_INFORME_VIGILANCIA_FUERO_FEDERAL);
                    }
                    break;

                case "eliminar_sanc_ff":
                    if (SeleCorrecFF != null)
                        if (LstCorrectivosFF != null && LstCorrectivosFF.Any())
                            if (LstCorrectivosFF.Remove(SeleCorrecFF))
                                SeleCorrecFF = null;

                    break;
                case "guardar_sanc_ff":
                    if (base.HasErrors)
                    {
                        Application.Current.Dispatcher.Invoke((System.Action)(delegate
                        {
                            (new Dialogos()).ConfirmacionDialogo("Advertencia!", base.Error);
                        }));
                        return;
                    }

                    if (LstCorrectivosFF == null)
                        LstCorrectivosFF = new ObservableCollection<PFF_CORRECTIVO>();

                    if (SeleCorrecFF == null)
                    {
                        LstCorrectivosFF.Add(new PFF_CORRECTIVO
                            {
                                FECHA = FecVigiSancFF,
                                ID_ANIO = SelectIngreso.ID_ANIO,
                                ID_CENTRO = SelectIngreso.ID_CENTRO,
                                ID_IMPUTADO = SelectIngreso.ID_IMPUTADO,
                                ID_INGRESO = SelectIngreso.ID_INGRESO,
                                RESOLUCION = ResolucionSancFF,
                                MOTIVO = MotivoSancFF
                            });
                    }

                    else
                    {
                        if (LstCorrectivosFF.Remove(SeleCorrecFF))
                        {
                            LstCorrectivosFF.Add(new PFF_CORRECTIVO
                            {
                                FECHA = FecVigiSancFF,
                                ID_ANIO = SelectIngreso.ID_ANIO,
                                ID_CENTRO = SelectIngreso.ID_CENTRO,
                                ID_IMPUTADO = SelectIngreso.ID_IMPUTADO,
                                ID_INGRESO = SelectIngreso.ID_INGRESO,
                                RESOLUCION = ResolucionSancFF,
                                MOTIVO = MotivoSancFF
                            });
                        };
                    };

                    SeleCorrecFF = null;
                    PopUpsViewModels.ClosePopUp(PopUpsViewModels.TipoPopUp.SANCIONES_INFORME_VIGILANCIA_FUERO_FEDERAL);
                    break;

                case "cancelar_sanc_ff":
                    SeleCorrecFF = null;
                    LimpiaValidacionesSancionesFederales();
                    PopUpsViewModels.ClosePopUp(PopUpsViewModels.TipoPopUp.SANCIONES_INFORME_VIGILANCIA_FUERO_FEDERAL);
                    break;

                #endregion

                default:
                    //no action
                    break;
            }
        }

        private void ValidaHuellasNavegacion()
        {
            try
            {
                PopUpsViewModels.ShowPopUp(this, PopUpsViewModels.TipoPopUp.OSCURECER_FONDO);
                HuellaWindow = new BuscarPorHuellaYNipView();
                HuellaWindow.DataContext = this;
                ConstructorHuella(0);
                HuellaWindow.Owner = PopUpsViewModels.MainWindow;
                HuellaWindow.Closed += HuellaClosed;
                HuellaWindow.ShowDialog();
                //if (SelectRegistro == null)
                //    return false;
                //else
                //    return true;

                string xx = NombreUltimoTabElegido;
            }
            catch (System.Exception exc)
            {
                throw exc;
            }
        }
        private void ProcesaDias(short _DiasNuevos, short DiasViejos)
        {
            try
            {//le quita los dias viejos y le suma los dias nuevos
                DiasLaboradosEfectivos = short.Parse(((DiasLaboradosEfectivos - DiasViejos) + (_DiasNuevos)).ToString());
            }
            catch (System.Exception exc)
            {
                throw exc;
            }
        }
        private void Imprimir(PERSONALIDAD Entity)
        {       //COMPLEMENTAR LA IMPRESION CON EL ESTUDIO DE PERSONALIDAD QUE LE CORRESPONDE
            try
            {
                string RutaReporte = string.Empty;

                if (IdVentanaAcual == decimal.Zero)
                    return;

                #region Iniciliza el entorno para mostrar el reporte al usuario
                var View = new ReportesView();
                PopUpsViewModels.ShowPopUp(this, PopUpsViewModels.TipoPopUp.OSCURECER_FONDO);
                View.Owner = PopUpsViewModels.MainWindow;
                View.Report.LocalReport.DataSources.Clear();
                View.Closed += (s, e) => { PopUpsViewModels.ClosePopUp(PopUpsViewModels.TipoPopUp.OSCURECER_FONDO); };
                //View.Show();
                #endregion

                if (IdVentanaAcual != decimal.Zero)
                {
                    switch (IdVentanaAcual)
                    {
                        case (short)eVentanasProceso.ACTIV_PRODUCTIVAS_CAPACITACION_FUERO_FEDERAL:
                            View.Report.LocalReport.DataSources.Add(EncabezadoReportesFueroFederal());
                            View.Report.LocalReport.DataSources.Add(DatosCuerpoActividadesProductCapacFueroFederal(Entity));
                            View.Report.LocalReport.DataSources.Add(DatosDiasLaboradosCapacitacionFueroFederal(Entity));
                            View.Report.LocalReport.DataSources.Add(DatosCursosCapacitacionFueroFederal(Entity));
                            RutaReporte = "Reportes/rInformeActivProducCapFF.rdlc";
                            break;

                        case (short)eVentanasProceso.CONSEJO_TENICO_INTERD_FUERO_FEDERAL:
                            View.Report.LocalReport.DataSources.Add(EncabezadoReportesFueroFederal());
                            View.Report.LocalReport.DataSources.Add(DatosAreasTecnicasActaInterd(Entity));
                            View.Report.LocalReport.DataSources.Add(DatosActaConsejoTecnicoInteridsciplinarioFueroFederal(Entity));
                            RutaReporte = "Reportes/rActaConsejoTecInterdFF.rdlc";
                            break;

                        case (short)eVentanasProceso.ESTUDIO_ACTIV_EDUCAT_CULT_DEP_RECR_CIV_FUERO_FEDERAL:
                            View.Report.LocalReport.DataSources.Add(EncabezadoReportesFueroFederal());
                            View.Report.LocalReport.DataSources.Add(DatosCuerpoInformeActivEducCultFueroFederal(Entity));
                            View.Report.LocalReport.DataSources.Add(DatosParticipacionesFueroFederal(Entity));
                            RutaReporte = "Reportes/rInformeActEducCultDepRecCivFF.rdlc";
                            break;

                        case (short)eVentanasProceso.ESTUDIO_CAPACITACION_TRABAJO_PENITENCIARIO_FUERO_COMUN:
                            View.Report.LocalReport.DataSources.Add(EncabezadoReportesFueroComun());
                            View.Report.LocalReport.DataSources.Add(DatosEstudioCapacitacionTrabajoPenitenciarioFueroComun(Entity));
                            View.Report.LocalReport.DataSources.Add(DatosCapacitacionLaboral(Entity));//Capacitacion laboral
                            View.Report.LocalReport.DataSources.Add(DatosActivNoGratificadas(Entity));//Actividades no gratificadas
                            View.Report.LocalReport.DataSources.Add(DatosActivGratificadas(Entity));//Actividades gratificadas
                            RutaReporte = "Reportes/rEstudioCapTrabPenitFC.rdlc";
                            break;

                        case (short)eVentanasProceso.ESTUDIO_CRIMINODIAGNOSTICO_FUERO_COMUN:
                            View.Report.LocalReport.DataSources.Add(EncabezadoReportesFueroComun());
                            View.Report.LocalReport.DataSources.Add(DatosEstudioCriminodiagnosticoFueroComun(Entity));
                            RutaReporte = "Reportes/rCriminoDiagnosticoFC.rdlc";
                            break;

                        case (short)eVentanasProceso.ESTUDIO_CRIMINOLOGICOO_FUERO_FEDERAL:
                            View.Report.LocalReport.DataSources.Add(EncabezadoReportesFueroFederal());
                            View.Report.LocalReport.DataSources.Add(DatosCuerpoCriminologicoFueroFederal(Entity));
                            RutaReporte = "Reportes/rEstudioCriminologicoFF.rdlc";
                            break;

                        case (short)eVentanasProceso.ESTUDIO_EDUCATIVO_CULTURAL_DEPORTIVO_FUERO_COMUN://
                            View.Report.LocalReport.DataSources.Add(EncabezadoReportesFueroComun());
                            View.Report.LocalReport.DataSources.Add(DatosEstudioEducativoCulturalDeportivoFueroComun(Entity));
                            View.Report.LocalReport.DataSources.Add(DatosEscolaridadAnterior(Entity));
                            View.Report.LocalReport.DataSources.Add(DatosActividadesEscolares(Entity));
                            View.Report.LocalReport.DataSources.Add(DatosActividadesCulturales(Entity));
                            View.Report.LocalReport.DataSources.Add(DatosActividadesDeportivas(Entity));
                            RutaReporte = "Reportes/rEStudiodioEducCultDepFC.rdlc";
                            break;

                        case (short)eVentanasProceso.ESTUDIO_MEDICO_FUERO_COMUN:
                            View.Report.LocalReport.DataSources.Add(EncabezadoReportesFueroComun());
                            View.Report.LocalReport.DataSources.Add(DatosEstudioMedicoFueroComun(Entity));
                            RutaReporte = "Reportes/rEstudioMedicoFueroComun.rdlc";
                            break;

                        case (short)eVentanasProceso.ESTUDIO_MEDICO_FUERO_FEDERAL:
                            View.Report.LocalReport.DataSources.Add(EncabezadoReportesFueroFederal());
                            View.Report.LocalReport.DataSources.Add(DatosCuerpoMedicoFueroFederal(Entity));
                            View.Report.LocalReport.DataSources.Add(DatosToxicosMedicoFueroFederal(Entity));
                            RutaReporte = "Reportes/rEstudioMedicoFF.rdlc";
                            break;

                        case (short)eVentanasProceso.ESTUDIO_PSICOLOGICO_FUERO_COMUN:
                            View.Report.LocalReport.DataSources.Add(EncabezadoReportesFueroComun());
                            View.Report.LocalReport.DataSources.Add(DatosEstudioPsicologicoFueroComun(Entity));
                            View.Report.LocalReport.DataSources.Add(DatosProgUno(Entity));
                            View.Report.LocalReport.DataSources.Add(DatosProgDos(Entity));
                            View.Report.LocalReport.DataSources.Add(DatosProgTres(Entity));
                            View.Report.LocalReport.DataSources.Add(DatosProgCuatro(Entity));
                            RutaReporte = "Reportes/rEstudioPsicologicoFueroComun.rdlc";
                            break;

                        case (short)eVentanasProceso.ESTUDIO_PSIQUIATRICO_FUERO_COMUN:
                            View.Report.LocalReport.DataSources.Add(EncabezadoReportesFueroComun());
                            View.Report.LocalReport.DataSources.Add(DatosEstudioPsiquiatricoFueroComun(Entity));
                            RutaReporte = "Reportes/rEstudioPsiquiatricoFueroComun.rdlc";
                            break;

                        case (short)eVentanasProceso.ESTUDIO_SEGURIDAD_CUSTODIA_FUERO_COMUN:
                            View.Report.LocalReport.DataSources.Add(EncabezadoReportesFueroComun());
                            View.Report.LocalReport.DataSources.Add(DatosInformeAreaSeguridadCustodiaFueroComun(Entity));
                            View.Report.LocalReport.DataSources.Add(SancionesInformeAreaSeguridadCustodiaFueroComun(Entity));
                            RutaReporte = "Reportes/RInformeAreaSeguridadCustodiaFC.rdlc";
                            break;

                        case (short)eVentanasProceso.ESTUDIO_SOCIOFAMILIAR_FUERO_COMUN:
                            View.Report.LocalReport.DataSources.Add(EncabezadoReportesFueroComun());
                            View.Report.LocalReport.DataSources.Add(DatosEstudioSocioFamiliarFueroComun(Entity));
                            View.Report.LocalReport.DataSources.Add(DatosVisitas(Entity));
                            View.Report.LocalReport.DataSources.Add(DatosProgramasFortalecimientoComun(Entity));
                            View.Report.LocalReport.DataSources.Add(DatosProgramasReligiososComun(Entity));
                            RutaReporte = "Reportes/rEstudioSocioFamFC.rdlc";
                            break;

                        case (short)eVentanasProceso.ESTUDIO_TRABAJO_SOCIAL_FUERO_FEDERAL:
                            RutaReporte = "Reportes/rEstudioTrabajoSocFF.rdlc";
                            View.Report.LocalReport.DataSources.Add(EncabezadoReportesFueroFederal());
                            View.Report.LocalReport.DataSources.Add(DatosCuerpoTSFueroFederal(Entity));
                            View.Report.LocalReport.DataSources.Add(DatosGrupoFamiliarPrimarioFueroFederal(Entity));
                            View.Report.LocalReport.DataSources.Add(DatosGrupoFamiliarSecundarioFueroFederal(Entity));
                            break;

                        case (short)eVentanasProceso.VIGILANCIA_FUERO_FEDERAL:
                            View.Report.LocalReport.DataSources.Add(EncabezadoReportesFueroFederal());
                            View.Report.LocalReport.DataSources.Add(DatosCuerpoInformeVigilanciaFueroFederal(Entity));
                            View.Report.LocalReport.DataSources.Add(DatosCorrectivosVigiFueroFederal(Entity));
                            RutaReporte = "Reportes/rInformeSeccVigilanciaFF.rdlc";
                            break;

                        case (short)eVentanasProceso.ESTUDIO_PSICOLOGICO_FUERO_FEDERAL:
                            View.Report.LocalReport.DataSources.Add(EncabezadoReportesFueroFederal());
                            View.Report.LocalReport.DataSources.Add(DatosCuerpoPsicologicoFueroFederal(Entity));
                            RutaReporte = "Reportes/rEstudioPsicologicoFF.rdlc";
                            break;

                        default:
                            //no action
                            break;
                    }

                    View.Report.LocalReport.ReportPath = RutaReporte;
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
                            tc.editor.IsSpellCheckingEnabled = true;
                            tc.editor.TextFrameMarkerLines = false;
                            tc.editor.EditMode = TXTextControl.EditMode.ReadAndSelect;
                            tc.editor.Load(renderedBytes, TXTextControl.BinaryStreamType.MSWord);//ESTE ES EL FORMATO CON MAYOR COMPATIBILIDAD CON RESPECTO AL MANEJO DE TEXTO 
                            tc.editor.ParagraphFormat.Alignment = TXTextControl.HorizontalAlignment.Justify;
                            foreach (var item in tc.editor.Paragraphs)
                            {
                                var algo = item as TXTextControl.Paragraph;
                                if (algo.Text.Contains(".:."))
                                    algo.Format.Alignment = TXTextControl.HorizontalAlignment.Left;

                                if (algo.Text.Contains("~.:.~"))
                                    algo.Format.Alignment = TXTextControl.HorizontalAlignment.Center;
                            };
                        }
                        catch (System.Exception ex)
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

            catch (System.Exception exc)
            {
                throw exc;
            }
        }
        #region Encabezados
        private Microsoft.Reporting.WinForms.ReportDataSource EncabezadoReportesFueroComun()
        {
            try
            {
                string NombreImputado = SelectIngreso != null ? SelectIngreso.IMPUTADO != null ?
                    string.Format("NOMBRE DEL INTERNO: {0} {1} {2}", !string.IsNullOrEmpty(SelectIngreso.IMPUTADO.NOMBRE) ? SelectIngreso.IMPUTADO.NOMBRE.Trim() : string.Empty,
                                  !string.IsNullOrEmpty(SelectIngreso.IMPUTADO.PATERNO) ? SelectIngreso.IMPUTADO.PATERNO.Trim() : string.Empty,
                                  !string.IsNullOrEmpty(SelectIngreso.IMPUTADO.MATERNO) ? SelectIngreso.IMPUTADO.MATERNO.Trim() : string.Empty) : string.Empty : string.Empty;

                var ds1 = new List<cEncabezado>();
                Microsoft.Reporting.WinForms.ReportDataSource rds1 = new Microsoft.Reporting.WinForms.ReportDataSource();
                cEncabezado Encabezado = new cEncabezado();
                Encabezado.TituloUno = "SECRETARÍA DE SEGURIDAD PÚBLICA";
                Encabezado.TituloDos = Parametro.ENCABEZADO2;
                Encabezado.ImagenIzquierda = Parametro.LOGO_ESTADO;
                Encabezado.ImagenFondo = Parametro.REPORTE_LOGO2;
                Encabezado.NoImputado = NombreImputado;
                Encabezado.NombreReporte = Parametro.ENCABEZADO_FUERO_COMUN;
                Encabezado.ImagenFondo = Parametro.REPORTE_LOGO_ISO;
                Encabezado.PieUno = Parametro.DESCR_ISO_1;
                Encabezado.PieDos = Parametro.DESCR_ISO_2;
                Encabezado.PieTres = Parametro.DESCR_ISO_3;
                ds1.Add(Encabezado);
                rds1.Name = "DataSet1";
                rds1.Value = ds1;
                return rds1;
            }

            catch (System.Exception exc)
            {
                throw exc;
            }
        }
        private Microsoft.Reporting.WinForms.ReportDataSource EncabezadoReportesFueroFederal()
        {
            try
            {
                var ds1 = new List<cReporte>();
                Microsoft.Reporting.WinForms.ReportDataSource rds1 = new Microsoft.Reporting.WinForms.ReportDataSource();
                cReporte Encabezado = new cReporte();
                Encabezado.Encabezado1 = Parametro.ENCABEZADO_FUERO_FEDERAL_1;
                Encabezado.Encabezado2 = Parametro.ENCABEZADO_FUERO_FEDERAL_2;
                Encabezado.Encabezado3 = Parametro.ENCABEZADO_FUERO_FEDERAL_3;
                Encabezado.Encabezado4 = Parametro.ENCABEZADO_FUERO_FEDERAL_4;
                Encabezado.ImagenIzquierda = Parametro.REPORTE_LOGO_FUERO_FEDERAL_2;
                Encabezado.ImagenMedio = Parametro.REPORTE_LOGO_FUERO_FEDERAL_1;
                Encabezado.ImagenDerecha = Parametro.REPORTE_LOGO_FUERO_FEDERAL_3;
                ds1.Add(Encabezado);
                rds1.Name = "DataSet1";
                rds1.Value = ds1;
                return rds1;
            }

            catch (System.Exception exc)
            {
                throw exc;
            }
        }
        #endregion

        private void SeleccionaIngreso()
        {
            try
            {
                if (SelectIngreso != null)
                {
                    _UltimoEstudioPersonalidadConcluido = new PERSONALIDAD();
                    var validacionEstudio = new cEstudioPersonalidad().GetData(x => x.ID_INGRESO == SelectIngreso.ID_INGRESO && x.ID_IMPUTADO == SelectIngreso.ID_IMPUTADO && x.ID_ANIO == SelectIngreso.ID_ANIO && x.INGRESO.ID_UB_CENTRO == GlobalVar.gCentro);
                    if (!validacionEstudio.Any())//No se le ha agendado nunca un estudio de personalidad
                    {
                        new Dialogos().ConfirmacionDialogo("Validación", "El interno seleccionado no cuenta con un estudio de personalidad programado");
                        return;
                    };

                    var _EstudioPorHacer = new cEstudioPersonalidad().GetData(x => x.ID_INGRESO == SelectIngreso.ID_INGRESO && x.ID_IMPUTADO == SelectIngreso.ID_IMPUTADO && x.ID_CENTRO == SelectIngreso.ID_CENTRO && x.ID_ANIO == SelectIngreso.ID_ANIO).OrderByDescending(x => x.ID_ESTUDIO).FirstOrDefault();
                    if (_EstudioPorHacer != null)
                    {
                        if (_EstudioPorHacer.PERSONALIDAD_DETALLE != null && _EstudioPorHacer.PERSONALIDAD_DETALLE.Any())
                        {
                            var DetalleProgramado = _EstudioPorHacer.PERSONALIDAD_DETALLE.ToList();
                            if (DetalleProgramado.TrueForAll(x => x.ID_ESTATUS != 5))
                            {
                                new Dialogos().ConfirmacionDialogo("Validación", "Verifique el estatus del desarrollo del estudio de personalidad");
                                return;
                            };

                            if (DetalleProgramado.TrueForAll(x => !x.PERSONALIDAD_DETALLE_DIAS.Any()))
                            {
                                new Dialogos().ConfirmacionDialogo("Validación", "El interno seleccionado no cuenta con fechas para el desarrollo del estudio de personalidad programadas");
                                return;
                            };

                            if (DetalleProgramado.TrueForAll(x => x.PERSONALIDAD_DETALLE_DIAS.ToList().TrueForAll(y => !y.FECHA_INICIO.HasValue)))
                            {
                                new Dialogos().ConfirmacionDialogo("Validación", "El imputado seleccionado no cuenta con fechas para el desarrollo del estudio de personalidad programadas");
                                return;
                            };

                            var _Dias = DetalleProgramado.Where(x => x.PERSONALIDAD_DETALLE_DIAS.Any()).Select(y => y.PERSONALIDAD_DETALLE_DIAS);
                            if (_Dias != null && _Dias.Any())
                            {
                                var _detalle = new cPersonalidadDetalleDias().ValidaFechas(_EstudioPorHacer, Fechas.GetFechaDateServer);
                                if (!_detalle.Any())
                                {
                                    new Dialogos().ConfirmacionDialogo("Validación", "El interno seleccionado no cuenta con fechas programadas para el día de hoy");
                                    return;
                                }
                            };
                        }
                        else
                        {
                            new Dialogos().ConfirmacionDialogo("Validación", "El interno seleccionado no cuenta con fechas programadas");
                            return;
                        }

                        var ubicacion = SelectIngreso.INGRESO_UBICACION.OrderByDescending(w => w.ID_CONSEC).FirstOrDefault();
                        if (ubicacion != null)
                        {
                            if (ubicacion.ESTATUS == 0)
                            {
                                new Dialogos().ConfirmacionDialogo("Validación", "El interno seleccionado se encuentra aun en su celda");
                                return;
                            }
                        }
                        else
                        {
                            new Dialogos().ConfirmacionDialogo("Validación", "El interno seleccionado se encuentra aun en su celda");
                            return;
                        }

                        PopUpsViewModels.ShowPopUp(this, PopUpsViewModels.TipoPopUp.OSCURECER_FONDO);
                        HuellaWindow = new BuscarPorHuellaYNipView();
                        HuellaWindow.DataContext = this;
                        ConstructorHuella(0);
                        HuellaWindow.Owner = PopUpsViewModels.MainWindow;
                        HuellaWindow.Closed += HuellaClosed;
                        HuellaWindow.ShowDialog();
                    }
                }
            }
            catch (System.Exception ex)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al seleccionar ingreso", ex);
            }
        }

        private async void InicializaEntornoRealizacionEstudiosPersonalidad()
        {
            try
            {
                if (SelectIngreso == null)
                    return;

                VisiblePrincipal = true;//Hace visibles los tab principales, es el inicio del procesamiento de los tab padre y los tab consecuentes

                #region Datos Basicos
                AnioD = SelectIngreso.ID_ANIO;
                FolioD = SelectIngreso.ID_IMPUTADO;
                PaternoD = SelectIngreso.IMPUTADO != null ? !string.IsNullOrEmpty(SelectIngreso.IMPUTADO.PATERNO) ? SelectIngreso.IMPUTADO.PATERNO.Trim() : string.Empty : string.Empty;
                MaternoD = SelectIngreso.IMPUTADO != null ? !string.IsNullOrEmpty(SelectIngreso.IMPUTADO.MATERNO) ? SelectIngreso.IMPUTADO.MATERNO.Trim() : string.Empty : string.Empty;
                NombreD = SelectIngreso.IMPUTADO != null ? !string.IsNullOrEmpty(SelectIngreso.IMPUTADO.NOMBRE) ? SelectIngreso.IMPUTADO.NOMBRE.Trim() : string.Empty : string.Empty;
                IngresosD = SelectIngreso.ID_INGRESO;

                if (SelectIngreso.CAMA != null)
                    UbicacionD = string.Format("{0}-{1}{2}-{3}", SelectIngreso.CAMA != null ?
                        SelectIngreso.CAMA.CELDA != null ? SelectIngreso.CAMA.CELDA.SECTOR != null ? SelectIngreso.CAMA.CELDA.SECTOR.EDIFICIO != null ? !string.IsNullOrEmpty(SelectIngreso.CAMA.CELDA.SECTOR.EDIFICIO.DESCR) ? SelectIngreso.CAMA.CELDA.SECTOR.EDIFICIO.DESCR.Trim() : string.Empty : string.Empty : string.Empty : string.Empty : string.Empty,
                        SelectIngreso.CAMA != null ? SelectIngreso.CAMA.CELDA != null ? SelectIngreso.CAMA.CELDA.SECTOR != null ? !string.IsNullOrEmpty(SelectIngreso.CAMA.CELDA.SECTOR.DESCR) ? SelectIngreso.CAMA.CELDA.SECTOR.DESCR.Trim() : string.Empty : string.Empty : string.Empty : string.Empty,
                        SelectIngreso.CAMA != null ? SelectIngreso.CAMA.CELDA != null ? !string.IsNullOrEmpty(SelectIngreso.CAMA.CELDA.ID_CELDA) ? SelectIngreso.CAMA.CELDA.ID_CELDA.Trim() : string.Empty : string.Empty : string.Empty, SelectIngreso.ID_UB_CAMA);
                else
                    UbicacionD = string.Empty;

                TipoSeguridadD = SelectIngreso.TIPO_SEGURIDAD != null ? !string.IsNullOrEmpty(SelectIngreso.TIPO_SEGURIDAD.DESCR) ? SelectIngreso.TIPO_SEGURIDAD.DESCR.Trim() : string.Empty : string.Empty;
                FecIngresoD = SelectIngreso.FEC_INGRESO_CERESO;
                ClasificacionJuridicaD = SelectIngreso.CLASIFICACION_JURIDICA != null ? !string.IsNullOrEmpty(SelectIngreso.CLASIFICACION_JURIDICA.DESCR) ? SelectIngreso.CLASIFICACION_JURIDICA.DESCR.Trim() : string.Empty : string.Empty;
                EstatusD = SelectIngreso.ESTATUS_ADMINISTRATIVO != null ? !string.IsNullOrEmpty(SelectIngreso.ESTATUS_ADMINISTRATIVO.DESCR) ? SelectIngreso.ESTATUS_ADMINISTRATIVO.DESCR.Trim() : string.Empty : string.Empty;
                #endregion

                //ESTE ES EL ULTIMO ESTUDIO ACTIVO, (EL QUE RECIEN SE LE VA A HACER)
                var _EstudioPorHacer = new cEstudioPersonalidad().GetData(x => x.ID_INGRESO == SelectIngreso.ID_INGRESO && x.ID_IMPUTADO == SelectIngreso.ID_IMPUTADO && x.ID_CENTRO == SelectIngreso.ID_CENTRO && x.ID_ANIO == SelectIngreso.ID_ANIO).OrderByDescending(x => x.ID_ESTUDIO).FirstOrDefault();
                if (_EstudioPorHacer != null)
                {
                    if (_EstudioPorHacer.ID_SITUACION != 5)
                    {
                        new Dialogos().ConfirmacionDialogo("Validación", "El estudio de personalidad no ha sido asignado");
                        return;
                    }

                    _UltimoEstudioPersonalidadConcluido = new cEstudioPersonalidad().GetData(x => x.ID_INGRESO == SelectIngreso.ID_INGRESO && x.ID_IMPUTADO == SelectIngreso.ID_IMPUTADO && x.ID_CENTRO == SelectIngreso.ID_CENTRO && x.ID_ANIO == SelectIngreso.ID_ANIO && x.ID_ESTUDIO != _EstudioPorHacer.ID_ESTUDIO).OrderByDescending(x => x.ID_ESTUDIO).FirstOrDefault();
                };

                //Validacion con respecto a estudio de personalidad
                var Fuero = new cCausaPenal().GetData(x => x.ID_INGRESO == SelectIngreso.ID_INGRESO && x.ID_IMPUTADO == SelectIngreso.ID_IMPUTADO && x.ID_ESTATUS_CP == (short)eEstatusCausaPenal.ACTIVO).FirstOrDefault();

                if (Fuero != null)
                    FueroImputado = Fuero.CP_FUERO;

                if (!string.IsNullOrEmpty(FueroImputado))
                {
                    if (FueroImputado == "C")
                    {
                        IsEnabledFueroFederalPrincipal = VisibleFederal = VisibleMainFederal = false;
                        IsEnablesFueroComunPrincipal = VisibleComun = VisibleComunMain = true;
                    }

                    if (FueroImputado == "F")
                    {
                        IsEnabledFueroFederalPrincipal = VisibleFederal = VisibleMainFederal = TabFueroFederal = true;
                        IsEnablesFueroComunPrincipal = VisibleComun = VisibleComunMain = false;
                    }
                }

                else
                {
                    new Dialogos().ConfirmacionDialogo("Validación", "VERIFIQUE EL ESTATUS DE LA CAUSA PENAL DEL IMPUTADO");
                    return;
                }

                if (SelectIngreso == null)
                {
                    new Dialogos().ConfirmacionDialogo("Validación", "Seleccione un ingreso valido");
                    return;
                }

                #region Configuracion en base a rol de ventanas padre

                //Inicia el valor de las variables booleanas que determinan la visibilidad de un tab especifico
                InicializaPerfilUsuario();
                #endregion



                if (_UltimoEstudioPersonalidadConcluido != null && _EstudioPorHacer.PERSONALIDAD_FUERO_COMUN == null && _EstudioPorHacer.PERSONALIDAD_FUERO_FEDERAL == null)
                {
                    int result = 0;
                    result = await (new Dialogos()).ConfirmacionTresBotonesDinamico("Realización de estudios de personalidad", "Existe un estudio de personalidad previo ¿Desea usarlo de referencia, o iniciar uno nuevo?", "Cancelar", 0, "Usar estudio previo", 1, "Iniciar estudio nuevo", 2);
                    switch (result)
                    {
                        case 0:
                            ((System.Windows.Controls.ContentControl)PopUpsViewModels.MainWindow.FindName("contentControl")).Content = new RealizacionEstudiosPersonalidadView();
                            ((System.Windows.Controls.ContentControl)PopUpsViewModels.MainWindow.FindName("contentControl")).DataContext = new ControlPenales.RealizacionEstudiosViewModel(); return;

                        case 1:
                            ProcesoIndividualEstudios(_UltimoEstudioPersonalidadConcluido);
                            break;

                        case 2:
                            ProcesoIndividualEstudios(_EstudioPorHacer);
                            break;

                        default:
                            //no action
                            return;
                    };
                }
                else
                    ProcesoIndividualEstudios(_EstudioPorHacer);


                //#region EL IMPUTADO LLEGO AL MODULO EN FISICO (AVISA A CONTROL DE INTERNOS)
                ////DESCOMENTAR PARA LA PRUEBA
                //var _ExisteEstatusLlegada = new cIngresoUbicacion().ObtenerUltimaUbicacion(SelectIngreso.ID_ANIO, SelectIngreso.ID_CENTRO, SelectIngreso.ID_IMPUTADO, SelectIngreso.ID_INGRESO);
                //if (_ExisteEstatusLlegada != null)
                //{
                //    if (_ExisteEstatusLlegada.ESTATUS != 2)
                //    {
                //        var IngresoUbic = new INGRESO_UBICACION()
                //        {
                //            ACTIVIDAD = ProcesaAreaTecnicaPorEstudio(),
                //            ESTATUS = 2,
                //            ID_ANIO = SelectIngreso.ID_ANIO,
                //            ID_AREA = _ExisteEstatusLlegada.ID_AREA,
                //            ID_CENTRO = SelectIngreso.ID_CENTRO,
                //            ID_IMPUTADO = SelectIngreso.ID_IMPUTADO,
                //            ID_INGRESO = SelectIngreso.ID_INGRESO,
                //            INTERNO_UBICACION = string.Empty,
                //            MOVIMIENTO_FEC = Fechas.GetFechaDateServer
                //        };

                //        List<INGRESO_UBICACION> lstIngresos = new List<INGRESO_UBICACION>();
                //        lstIngresos.Add(IngresoUbic);
                //        new cIngresoUbicacion().CambiarUbicaciones(lstIngresos);
                //    }
                //    else
                //    {//CON ESTO LE INDICO QUE SU ULTIMO MOVIMIENTO ES HACIENDO EL ESTUDIO EN ESTA AREA
                //        var IngresoUbic = new INGRESO_UBICACION()
                //        {
                //            ACTIVIDAD = ProcesaAreaTecnicaPorEstudio(),
                //            ESTATUS = 2,
                //            ID_ANIO = SelectIngreso.ID_ANIO,
                //            ID_AREA = _ExisteEstatusLlegada.ID_AREA,
                //            ID_CENTRO = SelectIngreso.ID_CENTRO,
                //            ID_IMPUTADO = SelectIngreso.ID_IMPUTADO,
                //            ID_INGRESO = SelectIngreso.ID_INGRESO,
                //            INTERNO_UBICACION = string.Empty,
                //            MOVIMIENTO_FEC = Fechas.GetFechaDateServer
                //        };

                //        List<INGRESO_UBICACION> lstIngresos2 = new List<INGRESO_UBICACION>();
                //        lstIngresos2.Add(IngresoUbic);
                //        new cIngresoUbicacion().CambiarUbicaciones(lstIngresos2);
                //    };
                //};
                //#endregion

            }
            catch (System.Exception exc)
            {
                throw exc;
            }
        }

        private async System.Threading.Tasks.Task<List<IMPUTADO>> SegmentarResultadoBusqueda(int _Pag = 1)
        {
            try
            {
                if (string.IsNullOrEmpty(ApellidoPaternoBuscar) && string.IsNullOrEmpty(ApellidoMaternoBuscar) && string.IsNullOrEmpty(NombreBuscar) && !AnioBuscar.HasValue && !FolioBuscar.HasValue)
                    return new List<IMPUTADO>();

                Pagina = _Pag;
                var result = await StaticSourcesViewModel.CargarDatosAsync<ObservableCollection<IMPUTADO>>(() => new cImputado().ObtenerTodos(ApellidoPaternoBuscar, ApellidoMaternoBuscar, NombreBuscar, AnioBuscar, FolioBuscar, _Pag));
                if (result.Any())
                {
                    Pagina++;
                    SeguirCargando = true;
                }
                else
                    SeguirCargando = false;

                return result.ToList();
            }
            catch (System.Exception ex)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al querer guardar.", ex);
                return new List<IMPUTADO>();
            }
        }

        private async void ModelEnter(System.Object obj)
        {
            try
            {
                if (obj != null)
                {
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
                        var EstatusInactivos = Parametro.ESTATUS_ADMINISTRATIVO_INACT;
                        foreach (var item in EstatusInactivos)
                        {
                            if (ListExpediente[0].INGRESO.Any())
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
                        var toleranciaTraslado = Parametro.TOLERANCIA_TRASLADO_EDIFICIO;
                        if (SelectIngreso.TRASLADO_DETALLE.Any(a => (a.ID_ESTATUS != "CA" ? a.TRASLADO.ORIGEN_TIPO != "F" : false) && a.TRASLADO.TRASLADO_FEC.AddHours(-toleranciaTraslado) <= Fechas.GetFechaDateServer))
                        {
                            new Dialogos().ConfirmacionDialogo("Notificación!", "El interno [" + SelectIngreso.ID_ANIO.ToString() + "/" +
                                SelectIngreso.ID_IMPUTADO.ToString() + "] tiene un traslado proximo y no tiene permitido ningun cambio de informacion.");
                            return;
                        }
                        SelectedInterno = SelectExpediente;
                        SeleccionaIngreso();
                        StaticSourcesViewModel.SourceChanged = false;
                        PopUpsViewModels.ClosePopUp(PopUpsViewModels.TipoPopUp.BUSQUEDA);
                        return;
                    }
                    else
                    {
                        SelectExpediente = null;
                        SelectIngreso = null;
                        ImagenImputado = ImagenIngreso = new Imagenes().getImagenPerson();
                        PopUpsViewModels.ShowPopUp(this, PopUpsViewModels.TipoPopUp.BUSQUEDA);
                    }
                }
                else
                {
                    ListExpediente = new RangeEnabledObservableCollection<IMPUTADO>();
                    ListExpediente.InsertRange(await SegmentarResultadoBusqueda());
                    if (ListExpediente.Count > 0)
                        EmptyExpedienteVisible = false;
                    else
                        EmptyExpedienteVisible = true;

                    ImagenImputado = ImagenIngreso = new Imagenes().getImagenPerson();
                    PopUpsViewModels.ShowPopUp(this, PopUpsViewModels.TipoPopUp.BUSQUEDA);
                }
            }
            catch (System.Exception ex)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al ingresar búsqueda", ex);
            }
        }

        private async void ClickEnter(System.Object obj)
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
            catch (System.Exception ex)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al ingresar búsqueda", ex);
            }
        }

        #region Consulta de Informacion para mostrar informacion base a partir de formatos de estudios de fuero comun y federal

        //private void DatosBase()
        //{
        //    try
        //    {
        //        var _EstudioRealizado = new cEstudioPersonalidadFueroComun().Obtener(SelectIngreso.ID_IMPUTADO, SelectIngreso.ID_INGRESO);
        //        if (_EstudioRealizado != null)
        //            EstudioRealizado = _EstudioRealizado;
        //        else
        //            EstudioRealizado = null;

        //        PrepararListas();
        //        //ValidacionesSuperUsuario();//Se comentan las validaciones a espera se determine bajo que condiciones se puede editar un estudio ya hecho
        //        EstudiosPersonalidadSuperUsuario();
        //    }

        //    catch (Exception exc)
        //    {
        //        throw;
        //    }
        //}

        private void PreparaListasCapacitacion(string Tipo)
        {
            try
            {
                if (!string.IsNullOrEmpty(Tipo))
                {
                    LstCapacitacion = new ObservableCollection<ACTIVIDAD>(new cActividad().GetData());
                    Application.Current.Dispatcher.Invoke((System.Action)(delegate
                    {
                        LstCapacitacion.Insert(0, (new ACTIVIDAD() { ID_ACTIVIDAD = -1, DESCR = "SELECCIONE" }));
                    }));
                }
            }
            catch (System.Exception exc)
            {
                throw exc;
            }
        }

        private void PrepararListas()
        {
            try
            {

                if (LstNivelIntelectualComun == null)
                {
                    LstNivelIntelectualComun = new ObservableCollection<PFC_IV_NIVEL_INTELECTUAL>(new cNivelIntelectualComun().ObtenerTodos(string.Empty));
                    Application.Current.Dispatcher.Invoke((System.Action)(delegate
                    {
                        LstNivelIntelectualComun.Insert(0, (new PFC_IV_NIVEL_INTELECTUAL() { ID_NIVEL = -1, DESCR = "SELECCIONE" }));
                    }));
                }

                if (ListDisfuncionNComun == null)
                {
                    ListDisfuncionNComun = new ObservableCollection<PFC_IV_DISFUNCION>(new cDisfuncionNeurologicaComun().ObtenerTodos(string.Empty));
                    Application.Current.Dispatcher.Invoke((System.Action)(delegate
                    {
                        ListDisfuncionNComun.Insert(0, (new PFC_IV_DISFUNCION() { ID_DISFUNCION = -1, DESCR = "SELECCIONE" }));
                    }));
                }

                if (ListClasifCriminologica == null)
                {
                    ListClasifCriminologica = new ObservableCollection<PFC_V_CRIMINOLOGICA>(new cClasificCriminologicaComun().ObtenerTodos(string.Empty));
                    Application.Current.Dispatcher.Invoke((System.Action)(delegate
                    {
                        ListClasifCriminologica.Insert(0, (new PFC_V_CRIMINOLOGICA() { ID_CLASIFICACION = -1, DESCR = "SELECCIONE" }));
                    }));
                }

                if (LstCapacidad == null)
                {
                    LstCapacidad = new ObservableCollection<PFC_V_CAPACIDAD>(new cCapacidad().ObtenerTodos(string.Empty));
                    Application.Current.Dispatcher.Invoke((System.Action)(delegate
                    {
                        LstCapacidad.Insert(0, (new PFC_V_CAPACIDAD() { ID_CAPACIDAD = -1, DESCR = "SELECCIONE" }));
                    }));
                }

                if (LstPeligrosidad == null)
                {
                    LstPeligrosidad = new ObservableCollection<PFC_V_PELIGROSIDAD>(new cPeligrosidad().ObtenerTodos(string.Empty));
                    Application.Current.Dispatcher.Invoke((System.Action)(delegate
                    {
                        LstPeligrosidad.Insert(0, (new PFC_V_PELIGROSIDAD() { ID_PELIGROSIDAD = -1, DESCR = "SELECCIONE" }));
                    }));
                }

                if (LstEscolaridad == null)
                {
                    LstEscolaridad = new ObservableCollection<EDUCACION_GRADO>(new cEducacionGrado().GetData());
                    Application.Current.Dispatcher.Invoke((System.Action)(delegate
                    {
                        LstEscolaridad.Insert(0, (new EDUCACION_GRADO() { ID_GRADO = -1, DESCR = "SELECCIONE" }));
                    }));
                }

                if (LstProgramasActiv == null)
                {
                    LstProgramasActiv = new ObservableCollection<ACTIVIDAD>(new cActividad().GetData());
                    Application.Current.Dispatcher.Invoke((System.Action)(delegate
                    {
                        LstProgramasActiv.Insert(0, (new ACTIVIDAD() { ID_ACTIVIDAD = -1, DESCR = "SELECCIONE" }));
                    }));
                }
            }
            catch (System.Exception exc)
            {
                throw exc;
            }
        }


        private short ProcesaEMICapacidad(short IdDato)
        {
            try
            {
                switch (IdDato)
                {
                    case (short)eEMIFactorNivel.ALTO:
                        IdDato = (short)eCapacidadEstudio.ALTA;
                        break;

                    case (short)eEMIFactorNivel.BAJO:
                        IdDato = (short)eCapacidadEstudio.BAJA;
                        break;

                    case (short)eEMIFactorNivel.MEDIO:
                        IdDato = (short)eCapacidadEstudio.MEDIA;
                        break;

                    case (short)eEMIFactorNivel.MEDIO_BAJO:
                        IdDato = (short)eCapacidadEstudio.MEDIA_BAJA;
                        break;

                    default:
                        IdDato = -1;
                        break;
                }

                return IdDato;
            }

            catch (System.Exception exc)
            {
                throw exc;
            }
        }

        private void ProcesoIndividualEstudios(PERSONALIDAD Entity)
        {
            try
            {
                if (SelectIngreso != null)
                {
                    LoadListasFederales();
                    PrepararListas();
                    #region Fuero Comun
                    #region Medico
                    if (EnabledMedicoFueroComun && EnabledComun1)
                        MedicoInfoComun(Entity);
                    #endregion
                    #region Psiquiatrico
                    if (EnabledPsiquiatricoFueroComun && EnabledComun2)
                        PsiquiatricoComunInfo(Entity);
                    #endregion
                    #region Psicologico
                    if (EnabledPsicologicoFueroComun && EnabledComun3)
                        PsicoInfoComun(Entity);
                    #endregion
                    #region Criminodiagnostico
                    if (EnabledCriminodiagnosticoFueroComun && EnabledComun4)
                        InfoCriminodComun(Entity);
                    #endregion
                    #region Socio Familiar
                    if (EnabledSocioFamiliarFueroComun && EnabledComun5)
                        EstudioSocioEComun(Entity);
                    #endregion
                    #region Capacitacion y Trabajo
                    if (EnabledCapacitacionFueroComun && EnabledComun7)
                        InfoCapacitacionComun(Entity);
                    #endregion
                    #region Estudio Educativo
                    if (EnabledEducativoFueroComun && EnabledComun6)
                        InfoEducativoComun(Entity);
                    #endregion
                    #region Seguridad
                    if (EnabledSeguridadFueroComun && EnabledComun8)
                        InfoSeguridad(Entity);
                    #endregion
                    #endregion

                    #region Fuero Federal
                    #region Acta Consejo Tecnico Interdisciplinario
                    //if (IsEnabledActaConsejoT)
                    //        ActaConsejoTecnicoInterd();
                    #endregion
                    #region Estudio Medico Federal
                    if (IsEnabledMedicoFederal && EnabledFederal1)
                        EstudioMedicoFueroFederal(Entity);
                    #endregion
                    #region Estudio Psicologico Federal
                    if (IsEnabledPsicoFederal && EnabledFederal2)
                        EstudioPsicologicoFueroFederalDatos(Entity);
                    #endregion
                    #region Actividades Productivas y Capacitacion de Fuero Federal
                    if (IsEnabledActivProd && EnabledFederal4)
                        InformeActividadesCapacitacionFueroFederalInfo(Entity);
                    #endregion
                    #region Actividades Educativas, Culturales , Deportivas, Recreativas y cicivas de Fuero Federal
                    if (IsEnabledActivEducFederal && EnabledFederal5)
                        InformeActivFF(Entity);
                    #endregion
                    #region Vigilancia de fuero federal
                    if (IsEnabledVigilanciaFederal && EnabledFederal6)
                        VigilanciaDatosFF(Entity);
                    #endregion
                    #region Criminog Fuero federal
                    if (IsEnabledCriminologicoFederal && EnabledFederal7)
                        CriminoDiagnosticoInfoFF(Entity);
                    #endregion
                    #region Trabajo Social de Fuero Federal
                    if (IsTrabajoSocialEnabled && EnabledFederal3)
                        SocioFamiliarFueroFederalInfo(Entity);
                    #endregion
                    #endregion
                };
            }
            catch (System.Exception exc)
            {
                throw exc;
            }
        }

        #endregion



        private async void OnModelChangedSwitch(object parametro)
        {
            switch (parametro.ToString())
            {
                case "cambio_tabs_realizacion_personalidad":
                    LocalizaIndexActual();
                    break;
                #region Cambio SelectedItem de Busqueda de Expediente
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
                #endregion
            }
        }

        private string ProcesaAreaTecnicaPorEstudio()
        {
            try
            {
                if (TabEstudioMedicoFC)
                    return "ESTUDIO DE PERSONALIDAD, ESTUDIO MEDICO";
                if (TabEstudioPsiqFC)
                    return "ESTUDIO DE PERSONALIDAD, ESTUDIO PSIQUIÁTRICO";
                if (TabEstudioPsicFC)
                    return "ESTUDIO DE PERSONALIDAD, ESTUDIO PSICOLÓGICO";
                if (TabCriminoDFC)
                    return "ESTUDIO DE PERSONALIDAD, ESTUDIO CRIMINODIAGNÓSTICO";
                if (TabEstudioSocioFamFC)
                    return "ESTUDIO DE PERSONALIDAD, ESTUDIO SOCIO - FAMILIAR";
                if (TabEstudioEducCultDepFC)
                    return "ESTUDIO DE PERSONALIDAD, ESTUDIO EDUCATIVO, CULTURAL Y DEPORTIVO";
                if (TabEstudioCapacitacionTrabajoPenitFC)
                    return "ESTUDIO DE PERSONALIDAD, ESTUDIO SOBRE CAPACITACIÓN Y TRABAJO PENITENCIARIO";
                if (TabSeguriddCustodiaFC)
                    return "ESTUDIO DE PERSONALIDAD, INFORME DEL ÁREA DE SEGURIDAD Y CUSTODIA";
                if (TabEstudioMedicoFF)
                    return "ESTUDIO DE PERSONALIDAD, ESTUDIO MEDICO";
                if (TabEstudioPsicoFF)
                    return "ESTUDIO DE PERSONALIDAD, ESTUDIO PSICOLÓGICO";
                if (TabEstudioTrabajoSocialFF)
                    return "ESTUDIO DE PERSONALIDAD, ESTUDIO DE TRABAJO SOCIAL";
                if (TabActivProductCapacitFF)
                    return "ESTUDIO DE PERSONALIDAD, INFORME DE LAS ACTIVIDADES PRODUCTIVAS DE CAPACITACIÓN";
                if (TabActEducCultDepRecCivFF)
                    return "ESTUDIO DE PERSONALIDAD, INFORME DE LAS ACTIVIDADES EDUCATIVAS, CULTURALES, DEPORTIVAS, RECREATIVAS Y CÍVICAS";
                if (TabVigilanciaFF)
                    return "INFORME DE LA SECCIÓN DE VIGILANCIA";
                if (TabEstudioCriminologico)
                    return "ESTUDIO CRIMINOLÓGICO";

                return "ESTUDIO DE PERSONALIDAD";
            }
            catch (System.Exception exc)
            {
                throw exc;
            }
        }
        /// <summary>
        /// METODO PRIMITIVO QUE EVALUA SI EXISTE ALGUN INDICE ACTIVOI, SI NO ASIGNA UNO POR DEFECTO, PARA QUE NO SE MUESTRA LA PANTALLA EN BLANCO AL USUARIO Y LE REDIRECCION HACIA DONDE DEBE HACER EL ESTUDIO
        /// </summary>
        /// <returns></returns>
        private int ValidaIndicesComunes()
        {
            try
            {
                CuantosActivos = 0;
                bool[] IndicesComunes = new bool[] { TabEstudioMedicoFC, TabEstudioPsiqFC, TabEstudioPsicFC, TabCriminoDFC, TabEstudioSocioFamFC, TabEstudioEducCultDepFC, TabEstudioCapacitacionTrabajoPenitFC, TabSeguriddCustodiaFC };
                CuantosActivos = IndicesComunes.Count(x => x.ToString() == "True");
                return CuantosActivos;
            }
            catch (System.Exception exc)
            {
                throw exc;
            }
        }

        private int ValidaIndicesFederales()
        {
            try
            {
                CuantosActivos = 0;
                bool[] IndicesFederales = new bool[] { TabEstudioMedicoFF, TabEstudioPsicoFF, TabEstudioTrabajoSocialFF, TabActivProductCapacitFF, TabActEducCultDepRecCivFF, TabVigilanciaFF, TabEstudioCriminologico };
                CuantosActivos = IndicesFederales.Count(x => x.ToString() == "True");
                return CuantosActivos;
            }
            catch (System.Exception exc)
            {
                throw exc;
            }
        }

        private void RegresaTabAnterior()
        {
            try
            {
                if (string.IsNullOrEmpty(NombreUltimoTabElegido))
                    return;

                #region COMUNES
                //INICIA EL PROCESO PARA SABER CUAL FUE EL ULTIMO TAB QUE SE VISITO, PARA REGRESARLO Y NO PERMITIRLE CONTINUAR HACIA EL SIGUIENTE SIN HABERSE AUTENTIFICADO
                if (NombreUltimoTabElegido == eNombresInternosTabsComunes.TabEstudioMedicoFC.ToString())
                {
                    NombreUltimoTabElegido = "TabEstudioMedicoFC";
                    TabEstudioPsiqFC = TabEstudioPsicFC = TabCriminoDFC = TabEstudioSocioFamFC = TabEstudioEducCultDepFC = TabEstudioCapacitacionTrabajoPenitFC = TabSeguriddCustodiaFC = false;
                    OnPropertyChanged("TabEstudioPsiqFC");
                    OnPropertyChanged("TabEstudioPsicFC");
                    OnPropertyChanged("TabCriminoDFC");
                    OnPropertyChanged("TabEstudioSocioFamFC");
                    OnPropertyChanged("TabEstudioEducCultDepFC");
                    OnPropertyChanged("TabEstudioCapacitacionTrabajoPenitFC");
                    OnPropertyChanged("TabSeguriddCustodiaFC");
                    IndexComun = 0;
                    TabEstudioMedicoFC = true;
                    IdVentanaAcual = (short)eVentanasProceso.ESTUDIO_MEDICO_FUERO_COMUN;
                    OnPropertyChanged("TabEstudioMedicoFC");
                    OnPropertyChanged("IdVentanaAcual");
                    OnPropertyChanged("IndexComun");
                    return;
                }

                if (NombreUltimoTabElegido == eNombresInternosTabsComunes.TabEstudioPsicFC.ToString())
                {
                    NombreUltimoTabElegido = "TabEstudioPsicFC";
                    TabEstudioMedicoFC = TabEstudioPsiqFC = TabCriminoDFC = TabEstudioSocioFamFC = TabEstudioEducCultDepFC = TabEstudioCapacitacionTrabajoPenitFC = TabSeguriddCustodiaFC = false;
                    IndexComun = 2;
                    TabEstudioPsicFC = true;
                    IdVentanaAcual = (short)eVentanasProceso.ESTUDIO_PSICOLOGICO_FUERO_COMUN;
                    OnPropertyChanged("TabEstudioPsicFC");
                    OnPropertyChanged("IdVentanaAcual");
                    return;
                }

                if (NombreUltimoTabElegido == eNombresInternosTabsComunes.TabEstudioPsiqFC.ToString())
                {
                    NombreUltimoTabElegido = "TabEstudioPsiqFC";
                    TabEstudioMedicoFC = TabEstudioPsicFC = TabCriminoDFC = TabEstudioSocioFamFC = TabEstudioEducCultDepFC = TabEstudioCapacitacionTrabajoPenitFC = TabSeguriddCustodiaFC = false;
                    IndexComun = 1;
                    IdVentanaAcual = (short)eVentanasProceso.ESTUDIO_PSIQUIATRICO_FUERO_COMUN;
                    TabEstudioPsiqFC = true;
                    OnPropertyChanged("TabEstudioPsiqFC");
                    OnPropertyChanged("IdVentanaAcual");
                    return;
                }

                if (NombreUltimoTabElegido == eNombresInternosTabsComunes.TabEstudioSocioFamFC.ToString())
                {
                    NombreUltimoTabElegido = "TabEstudioSocioFamFC";
                    TabEstudioMedicoFC = TabEstudioPsiqFC = TabEstudioPsicFC = TabCriminoDFC = TabEstudioEducCultDepFC = TabEstudioCapacitacionTrabajoPenitFC = TabSeguriddCustodiaFC = false;
                    IndexComun = 4;
                    TabEstudioSocioFamFC = true;
                    IdVentanaAcual = (short)eVentanasProceso.ESTUDIO_SOCIOFAMILIAR_FUERO_COMUN;
                    OnPropertyChanged("TabEstudioSocioFamFC");
                    OnPropertyChanged("IdVentanaAcual");
                    return;
                }

                if (NombreUltimoTabElegido == eNombresInternosTabsComunes.TabSeguriddCustodiaFC.ToString())
                {
                    NombreUltimoTabElegido = "TabSeguriddCustodiaFC";
                    TabEstudioMedicoFC = TabEstudioPsiqFC = TabEstudioPsicFC = TabCriminoDFC = TabEstudioSocioFamFC = TabEstudioEducCultDepFC = TabEstudioCapacitacionTrabajoPenitFC = false;
                    IndexComun = 7;
                    TabSeguriddCustodiaFC = true;
                    IdVentanaAcual = (short)eVentanasProceso.ESTUDIO_SEGURIDAD_CUSTODIA_FUERO_COMUN;
                    OnPropertyChanged("TabSeguriddCustodiaFC");
                    OnPropertyChanged("IdVentanaAcual");
                    return;
                }

                if (NombreUltimoTabElegido == eNombresInternosTabsComunes.TabCriminoDFC.ToString())
                {
                    NombreUltimoTabElegido = "TabCriminoDFC";
                    TabEstudioMedicoFC = TabEstudioPsiqFC = TabEstudioPsicFC = TabEstudioSocioFamFC = TabEstudioEducCultDepFC = TabEstudioCapacitacionTrabajoPenitFC = TabSeguriddCustodiaFC = false;
                    IdVentanaAcual = (short)eVentanasProceso.ESTUDIO_CRIMINODIAGNOSTICO_FUERO_COMUN;
                    IndexComun = 3;
                    TabCriminoDFC = true;
                    OnPropertyChanged("TabCriminoDFC");
                    OnPropertyChanged("IdVentanaAcual");
                    return;
                }

                if (NombreUltimoTabElegido == eNombresInternosTabsComunes.TabEstudioCapacitacionTrabajoPenitFC.ToString())
                {
                    NombreUltimoTabElegido = "TabEstudioCapacitacionTrabajoPenitFC";
                    TabEstudioMedicoFC = TabEstudioPsiqFC = TabEstudioPsicFC = TabCriminoDFC = TabEstudioSocioFamFC = TabEstudioEducCultDepFC = TabSeguriddCustodiaFC = false;
                    IndexComun = 6;
                    TabEstudioCapacitacionTrabajoPenitFC = true;
                    IdVentanaAcual = (short)eVentanasProceso.ESTUDIO_CAPACITACION_TRABAJO_PENITENCIARIO_FUERO_COMUN;
                    OnPropertyChanged("TabEstudioCapacitacionTrabajoPenitFC");
                    OnPropertyChanged("IdVentanaAcual");
                    return;
                }

                if (NombreUltimoTabElegido == eNombresInternosTabsComunes.TabEstudioEducCultDepFC.ToString())
                {
                    NombreUltimoTabElegido = "TabEstudioEducCultDepFC";
                    TabEstudioMedicoFC = TabEstudioPsiqFC = TabEstudioPsicFC = TabCriminoDFC = TabEstudioSocioFamFC = TabEstudioCapacitacionTrabajoPenitFC = TabSeguriddCustodiaFC = false;
                    IndexComun = 5;
                    TabEstudioEducCultDepFC = true;
                    IdVentanaAcual = (short)eVentanasProceso.ESTUDIO_EDUCATIVO_CULTURAL_DEPORTIVO_FUERO_COMUN;
                    OnPropertyChanged("TabEstudioEducCultDepFC");
                    OnPropertyChanged("IdVentanaAcual");
                    return;
                }

                #endregion
                #region FEDERALES
                if (NombreUltimoTabElegido == eNombresInternosTabsFederales.TabEstudioMedicoFF.ToString())
                {
                    NombreUltimoTabElegido = "TabEstudioMedicoFF";
                    TabEstudioPsicoFF = TabEstudioTrabajoSocialFF = TabActivProductCapacitFF = TabActEducCultDepRecCivFF = TabVigilanciaFF = TabEstudioCriminologico = false;
                    IndexFederal = 0;
                    TabEstudioMedicoFF = true;
                    IdVentanaAcual = (short)eVentanasProceso.ESTUDIO_MEDICO_FUERO_FEDERAL;
                    OnPropertyChanged("TabEstudioMedicoFF");
                    OnPropertyChanged("IdVentanaAcual");
                    return;
                }

                if (NombreUltimoTabElegido == eNombresInternosTabsFederales.TabEstudioPsicoFF.ToString())
                {
                    NombreUltimoTabElegido = "TabEstudioPsicoFF";
                    TabEstudioMedicoFF = TabEstudioTrabajoSocialFF = TabActivProductCapacitFF = TabActEducCultDepRecCivFF = TabVigilanciaFF = TabEstudioCriminologico = false;
                    IndexFederal = 1;
                    TabEstudioPsicoFF = true;
                    IdVentanaAcual = (short)eVentanasProceso.ESTUDIO_PSICOLOGICO_FUERO_FEDERAL;
                    OnPropertyChanged("TabEstudioPsicoFF");
                    OnPropertyChanged("IdVentanaAcual");
                    return;
                }

                if (NombreUltimoTabElegido == eNombresInternosTabsFederales.TabEstudioTrabajoSocialFF.ToString())
                {
                    NombreUltimoTabElegido = "TabEstudioTrabajoSocialFF";
                    TabEstudioMedicoFF = TabEstudioPsicoFF = TabActivProductCapacitFF = TabActEducCultDepRecCivFF = TabVigilanciaFF = TabEstudioCriminologico = false;
                    IndexFederal = 2;
                    TabEstudioTrabajoSocialFF = true;
                    IdVentanaAcual = (short)eVentanasProceso.ESTUDIO_TRABAJO_SOCIAL_FUERO_FEDERAL;
                    OnPropertyChanged("TabEstudioTrabajoSocialFF");
                    OnPropertyChanged("IdVentanaAcual");
                    return;
                }

                if (NombreUltimoTabElegido == eNombresInternosTabsFederales.TabVigilanciaFF.ToString())
                {
                    NombreUltimoTabElegido = "TabVigilanciaFF";
                    TabEstudioMedicoFF = TabEstudioPsicoFF = TabEstudioTrabajoSocialFF = TabActivProductCapacitFF = TabActEducCultDepRecCivFF = TabEstudioCriminologico = false;
                    IndexFederal = 5;
                    TabVigilanciaFF = true;
                    IdVentanaAcual = (short)eVentanasProceso.VIGILANCIA_FUERO_FEDERAL;
                    OnPropertyChanged("TabVigilanciaFF");
                    OnPropertyChanged("IdVentanaAcual");
                    return;
                }

                if (NombreUltimoTabElegido == eNombresInternosTabsFederales.TabActEducCultDepRecCivFF.ToString())
                {
                    NombreUltimoTabElegido = "TabActEducCultDepRecCivFF";
                    TabEstudioMedicoFF = TabEstudioPsicoFF = TabEstudioTrabajoSocialFF = TabActivProductCapacitFF = TabVigilanciaFF = TabEstudioCriminologico = false;
                    IndexFederal = 4;
                    TabActEducCultDepRecCivFF = true;
                    IdVentanaAcual = (short)eVentanasProceso.ESTUDIO_ACTIV_EDUCAT_CULT_DEP_RECR_CIV_FUERO_FEDERAL;
                    OnPropertyChanged("TabActEducCultDepRecCivFF");
                    OnPropertyChanged("IdVentanaAcual");
                    return;
                }

                if (NombreUltimoTabElegido == eNombresInternosTabsFederales.TabActivProductCapacitFF.ToString())
                {
                    NombreUltimoTabElegido = "TabActivProductCapacitFF";
                    TabEstudioMedicoFF = TabEstudioPsicoFF = TabEstudioTrabajoSocialFF = TabActEducCultDepRecCivFF = TabVigilanciaFF = TabEstudioCriminologico = false;
                    IndexFederal = 3;
                    TabActivProductCapacitFF = true;
                    IdVentanaAcual = (short)eVentanasProceso.ACTIV_PRODUCTIVAS_CAPACITACION_FUERO_FEDERAL;
                    OnPropertyChanged("TabActivProductCapacitFF");
                    OnPropertyChanged("IdVentanaAcual");
                    return;
                }

                if (NombreUltimoTabElegido == eNombresInternosTabsFederales.TabEstudioCriminologico.ToString())
                {
                    NombreUltimoTabElegido = "TabEstudioCriminologico";
                    TabEstudioMedicoFF = TabEstudioPsicoFF = TabEstudioTrabajoSocialFF = TabActivProductCapacitFF = TabActEducCultDepRecCivFF = TabVigilanciaFF = false;
                    IndexFederal = 6;
                    TabEstudioCriminologico = true;
                    IdVentanaAcual = (short)eVentanasProceso.ESTUDIO_CRIMINOLOGICOO_FUERO_FEDERAL;
                    OnPropertyChanged("TabEstudioCriminologico");
                    OnPropertyChanged("IdVentanaAcual");
                }

                #endregion
            }
            catch (System.Exception exc)
            {
                throw exc;
            }
        }

        private void IngresaNuevaUbicacion(short TipoEstudio)
        {
            try
            {
                if (SelectIngreso == null)
                    return;

                System.DateTime FechaAactual = Fechas.GetFechaDateServer;
                var _detallesProgramacion = new cPersonalidadDetalleDias().GetData(x => x.ID_ANIO == SelectIngreso.ID_ANIO && x.ID_CENTRO == SelectIngreso.ID_CENTRO && x.ID_IMPUTADO == SelectIngreso.ID_IMPUTADO && x.ID_INGRESO == SelectIngreso.ID_INGRESO &&
                    x.PERSONALIDAD_DETALLE != null && x.PERSONALIDAD_DETALLE.ID_TIPO == TipoEstudio && x.FECHA_INICIO.Value.Year == FechaAactual.Year &&
                    x.FECHA_INICIO.Value.Month == FechaAactual.Month && x.FECHA_INICIO.Value.Day == FechaAactual.Day).OrderByDescending(y => y.ID_CONSEC).FirstOrDefault();

                var IngresoUbic = new INGRESO_UBICACION()
                {
                    ACTIVIDAD = ProcesaAreaTecnicaPorEstudio(),
                    ESTATUS = 2,
                    ID_ANIO = SelectIngreso.ID_ANIO,
                    ID_AREA = _detallesProgramacion != null ? _detallesProgramacion.ID_AREA : (short)eAreaVacia.SIN_AREA,
                    ID_CENTRO = SelectIngreso.ID_CENTRO,
                    ID_IMPUTADO = SelectIngreso.ID_IMPUTADO,
                    ID_INGRESO = SelectIngreso.ID_INGRESO,
                    INTERNO_UBICACION = string.Empty,
                    MOVIMIENTO_FEC = Fechas.GetFechaDateServer
                };

                new cIngresoUbicacion().Insertar(IngresoUbic);
            }
            catch (System.Exception exc)
            {
                throw exc;
            }
        }

        private void TabSiguiente()
        {
            try
            {
                if (VisibleComun)
                {
                    //IndexComun = -1;
                    if (EnabledComun1)
                        if (TabEstudioMedicoFC)
                        {
                            NombreUltimoTabElegido = "TabEstudioMedicoFC";
                            TabEstudioPsiqFC = TabEstudioPsicFC = TabCriminoDFC = TabEstudioSocioFamFC = TabEstudioEducCultDepFC = TabEstudioCapacitacionTrabajoPenitFC = TabSeguriddCustodiaFC = false;
                            IndexComun = 0;
                            TabEstudioMedicoFC = true;
                            IdVentanaAcual = (short)eVentanasProceso.ESTUDIO_MEDICO_FUERO_COMUN;
                            OnPropertyChanged("TabEstudioMedicoFC");
                            OnPropertyChanged("IdVentanaAcual");
                            OnPropertyChanged("IndexComun");
                            //IngresaNuevaUbicacion((short)eProcEstudiosNuevasUbicacionesP.ESTUDIO_MEDICO_DE_FUERO_COMUN);
                        }

                    if (EnabledComun2)
                        if (TabEstudioPsiqFC)
                        {
                            NombreUltimoTabElegido = "TabEstudioPsiqFC";
                            TabEstudioMedicoFC = TabEstudioPsicFC = TabCriminoDFC = TabEstudioSocioFamFC = TabEstudioEducCultDepFC = TabEstudioCapacitacionTrabajoPenitFC = TabSeguriddCustodiaFC = false;
                            IndexComun = 1;
                            IdVentanaAcual = (short)eVentanasProceso.ESTUDIO_PSIQUIATRICO_FUERO_COMUN;
                            TabEstudioPsiqFC = true;
                            OnPropertyChanged("TabEstudioPsiqFC");
                            OnPropertyChanged("IdVentanaAcual");
                            //IngresaNuevaUbicacion((short)eProcEstudiosNuevasUbicacionesP.ESTUDIO_PSIQUIATRICO_DE_FUERO_COMUN);
                        }

                    if (EnabledComun3)
                        if (TabEstudioPsicFC)
                        {
                            NombreUltimoTabElegido = "TabEstudioPsicFC";
                            TabEstudioMedicoFC = TabEstudioPsiqFC = TabCriminoDFC = TabEstudioSocioFamFC = TabEstudioEducCultDepFC = TabEstudioCapacitacionTrabajoPenitFC = TabSeguriddCustodiaFC = false;
                            IndexComun = 2;
                            TabEstudioPsicFC = true;
                            IdVentanaAcual = (short)eVentanasProceso.ESTUDIO_PSICOLOGICO_FUERO_COMUN;
                            OnPropertyChanged("TabEstudioPsicFC");
                            OnPropertyChanged("IdVentanaAcual");
                            //IngresaNuevaUbicacion((short)eProcEstudiosNuevasUbicacionesP.ESTUDIO_PSICOLOGICO_DE_FUERO_COMUN);
                        }

                    if (EnabledComun4)
                        if (TabCriminoDFC)
                        {
                            NombreUltimoTabElegido = "TabCriminoDFC";
                            TabEstudioMedicoFC = TabEstudioPsiqFC = TabEstudioPsicFC = TabEstudioSocioFamFC = TabEstudioEducCultDepFC = TabEstudioCapacitacionTrabajoPenitFC = TabSeguriddCustodiaFC = false;
                            IdVentanaAcual = (short)eVentanasProceso.ESTUDIO_CRIMINODIAGNOSTICO_FUERO_COMUN;
                            IndexComun = 3;
                            TabCriminoDFC = true;
                            OnPropertyChanged("TabCriminoDFC");
                            OnPropertyChanged("IdVentanaAcual");
                            //IngresaNuevaUbicacion((short)eProcEstudiosNuevasUbicacionesP.ESTUDIO_CRIMINOLOGICO_DE_FUERO_COMUN);
                        }

                    if (EnabledComun5)
                        if (TabEstudioSocioFamFC)
                        {
                            NombreUltimoTabElegido = "TabEstudioSocioFamFC";
                            TabEstudioMedicoFC = TabEstudioPsiqFC = TabEstudioPsicFC = TabCriminoDFC = TabEstudioEducCultDepFC = TabEstudioCapacitacionTrabajoPenitFC = TabSeguriddCustodiaFC = false;
                            IndexComun = 4;
                            TabEstudioSocioFamFC = true;
                            IdVentanaAcual = (short)eVentanasProceso.ESTUDIO_SOCIOFAMILIAR_FUERO_COMUN;
                            OnPropertyChanged("TabEstudioSocioFamFC");
                            OnPropertyChanged("IdVentanaAcual");
                            //IngresaNuevaUbicacion((short)eProcEstudiosNuevasUbicacionesP.ESTUDIO_DE_TRABAJO_SOCIAL_DE_FUERO_COMUN);
                        }

                    if (EnabledComun6)
                        if (TabEstudioEducCultDepFC)
                        {
                            NombreUltimoTabElegido = "TabEstudioEducCultDepFC";
                            TabEstudioMedicoFC = TabEstudioPsiqFC = TabEstudioPsicFC = TabCriminoDFC = TabEstudioSocioFamFC = TabEstudioCapacitacionTrabajoPenitFC = TabSeguriddCustodiaFC = false;
                            IndexComun = 5;
                            TabEstudioEducCultDepFC = true;
                            IdVentanaAcual = (short)eVentanasProceso.ESTUDIO_EDUCATIVO_CULTURAL_DEPORTIVO_FUERO_COMUN;
                            OnPropertyChanged("TabEstudioEducCultDepFC");
                            OnPropertyChanged("IdVentanaAcual");
                            //IngresaNuevaUbicacion((short)eProcEstudiosNuevasUbicacionesP.ESTUDIO_PEDAGOGICO_DE_FUERO_COMUN);
                        }

                    if (EnabledComun7)
                        if (TabEstudioCapacitacionTrabajoPenitFC)
                        {
                            NombreUltimoTabElegido = "TabEstudioCapacitacionTrabajoPenitFC";
                            TabEstudioMedicoFC = TabEstudioPsiqFC = TabEstudioPsicFC = TabCriminoDFC = TabEstudioSocioFamFC = TabEstudioEducCultDepFC = TabSeguriddCustodiaFC = false;
                            IndexComun = 6;
                            TabEstudioCapacitacionTrabajoPenitFC = true;
                            IdVentanaAcual = (short)eVentanasProceso.ESTUDIO_CAPACITACION_TRABAJO_PENITENCIARIO_FUERO_COMUN;
                            OnPropertyChanged("TabEstudioCapacitacionTrabajoPenitFC");
                            OnPropertyChanged("IdVentanaAcual");
                            //IngresaNuevaUbicacion((short)eProcEstudiosNuevasUbicacionesP.ESTUDIO_LABORAL_DE_FUERO_COMUN);
                        }

                    if (EnabledComun8)
                        if (TabSeguriddCustodiaFC)
                        {
                            NombreUltimoTabElegido = "TabSeguriddCustodiaFC";
                            TabEstudioMedicoFC = TabEstudioPsiqFC = TabEstudioPsicFC = TabCriminoDFC = TabEstudioSocioFamFC = TabEstudioEducCultDepFC = TabEstudioCapacitacionTrabajoPenitFC = false;
                            IndexComun = 7;
                            TabSeguriddCustodiaFC = true;
                            IdVentanaAcual = (short)eVentanasProceso.ESTUDIO_SEGURIDAD_CUSTODIA_FUERO_COMUN;
                            OnPropertyChanged("TabSeguriddCustodiaFC");
                            OnPropertyChanged("IdVentanaAcual");
                            //IngresaNuevaUbicacion((short)eProcEstudiosNuevasUbicacionesP.ESTUDIO_DE_SEGURIDAD_DE_FUERO_COMÚN);
                        }

                    OnPropertyChanged("IndexComun");
                }
                else
                {
                    if (VisibleFederal)
                    {
                        if (EnabledFederal1)
                            if (TabEstudioMedicoFF)
                            {
                                NombreUltimoTabElegido = "TabEstudioMedicoFF";
                                TabEstudioPsicoFF = TabEstudioTrabajoSocialFF = TabActivProductCapacitFF = TabActEducCultDepRecCivFF = TabVigilanciaFF = TabEstudioCriminologico = false;
                                IndexFederal = 0;
                                TabEstudioMedicoFF = true;
                                IdVentanaAcual = (short)eVentanasProceso.ESTUDIO_MEDICO_FUERO_FEDERAL;
                                OnPropertyChanged("TabEstudioMedicoFF");
                                OnPropertyChanged("IdVentanaAcual");
                                //IngresaNuevaUbicacion((short)eProcEstudiosNuevasUbicacionesP.ESTUDIO_MEDICO_DE_FUERO_FEDERAL);
                            }

                        if (EnabledFederal2)
                            if (TabEstudioPsicoFF)
                            {
                                NombreUltimoTabElegido = "TabEstudioPsicoFF";
                                TabEstudioMedicoFF = TabEstudioTrabajoSocialFF = TabActivProductCapacitFF = TabActEducCultDepRecCivFF = TabVigilanciaFF = TabEstudioCriminologico = false;
                                IndexFederal = 1;
                                TabEstudioPsicoFF = true;
                                IdVentanaAcual = (short)eVentanasProceso.ESTUDIO_PSICOLOGICO_FUERO_FEDERAL;
                                OnPropertyChanged("TabEstudioPsicoFF");
                                OnPropertyChanged("IdVentanaAcual");
                                //IngresaNuevaUbicacion((short)eProcEstudiosNuevasUbicacionesP.ESTUDIO_PSICOLOGICO_DE_FUERO_FEDERAL);
                            }

                        if (EnabledFederal3)
                            if (TabEstudioTrabajoSocialFF)
                            {
                                NombreUltimoTabElegido = "TabEstudioTrabajoSocialFF";
                                TabEstudioMedicoFF = TabEstudioPsicoFF = TabActivProductCapacitFF = TabActEducCultDepRecCivFF = TabVigilanciaFF = TabEstudioCriminologico = false;
                                IndexFederal = 2;
                                TabEstudioTrabajoSocialFF = true;
                                IdVentanaAcual = (short)eVentanasProceso.ESTUDIO_TRABAJO_SOCIAL_FUERO_FEDERAL;
                                OnPropertyChanged("TabEstudioTrabajoSocialFF");
                                OnPropertyChanged("IdVentanaAcual");
                                //IngresaNuevaUbicacion((short)eProcEstudiosNuevasUbicacionesP.ESTUDIO_DE_TRABAJO_SOCIAL_DE_FUERO_FEDERAL);
                            }

                        if (EnabledFederal4)
                            if (TabActivProductCapacitFF)
                            {
                                NombreUltimoTabElegido = "TabActivProductCapacitFF";
                                TabEstudioMedicoFF = TabEstudioPsicoFF = TabEstudioTrabajoSocialFF = TabActEducCultDepRecCivFF = TabVigilanciaFF = TabEstudioCriminologico = false;
                                IndexFederal = 3;
                                TabActivProductCapacitFF = true;
                                IdVentanaAcual = (short)eVentanasProceso.ACTIV_PRODUCTIVAS_CAPACITACION_FUERO_FEDERAL;
                                OnPropertyChanged("TabActivProductCapacitFF");
                                OnPropertyChanged("IdVentanaAcual");
                                //IngresaNuevaUbicacion((short)eProcEstudiosNuevasUbicacionesP.ESTUDIO_LABORAL_DE_FUERO_FEDERAL);
                            }

                        if (EnabledFederal5)
                            if (TabActEducCultDepRecCivFF)
                            {
                                NombreUltimoTabElegido = "TabActEducCultDepRecCivFF";
                                TabEstudioMedicoFF = TabEstudioPsicoFF = TabEstudioTrabajoSocialFF = TabActivProductCapacitFF = TabVigilanciaFF = TabEstudioCriminologico = false;
                                IndexFederal = 4;
                                TabActEducCultDepRecCivFF = true;
                                IdVentanaAcual = (short)eVentanasProceso.ESTUDIO_ACTIV_EDUCAT_CULT_DEP_RECR_CIV_FUERO_FEDERAL;
                                OnPropertyChanged("TabActEducCultDepRecCivFF");
                                OnPropertyChanged("IdVentanaAcual");
                                //IngresaNuevaUbicacion((short)eProcEstudiosNuevasUbicacionesP.ESTUDIO_PEDAGOGICO_DE_FUERO_FEDERAL);
                            }

                        if (EnabledFederal6)
                            if (TabVigilanciaFF)
                            {
                                NombreUltimoTabElegido = "TabVigilanciaFF";
                                TabEstudioMedicoFF = TabEstudioPsicoFF = TabEstudioTrabajoSocialFF = TabActivProductCapacitFF = TabActEducCultDepRecCivFF = TabEstudioCriminologico = false;
                                IndexFederal = 5;
                                TabVigilanciaFF = true;
                                IdVentanaAcual = (short)eVentanasProceso.VIGILANCIA_FUERO_FEDERAL;
                                OnPropertyChanged("TabVigilanciaFF");
                                OnPropertyChanged("IdVentanaAcual");
                                //IngresaNuevaUbicacion((short)eProcEstudiosNuevasUbicacionesP.ESTUDIO_DE_SEGURIDAD_DE_FUERO_FEDERAL);
                            }

                        if (EnabledFederal7)
                            if (TabEstudioCriminologico)
                            {
                                NombreUltimoTabElegido = "TabEstudioCriminologico";
                                TabEstudioMedicoFF = TabEstudioPsicoFF = TabEstudioTrabajoSocialFF = TabActivProductCapacitFF = TabActEducCultDepRecCivFF = TabVigilanciaFF = false;
                                IndexFederal = 6;
                                TabEstudioCriminologico = true;
                                IdVentanaAcual = (short)eVentanasProceso.ESTUDIO_CRIMINOLOGICOO_FUERO_FEDERAL;
                                OnPropertyChanged("TabEstudioCriminologico");
                                OnPropertyChanged("IdVentanaAcual");
                                //IngresaNuevaUbicacion((short)eProcEstudiosNuevasUbicacionesP.ESTUDIO_CRIMINOLOGICO_DE_FUERO_FEDERAL);
                            }

                        OnPropertyChanged("IndexFederal");
                    }
                }
            }
            catch (System.Exception exc)
            {
                throw exc;
            }
        }

        /// <summary>
        /// METODO PRIMITIVO QUE LOCALIZA EL INDEX CON EL QUE VA A EMPEZAR EL USUARIO EN BASE AL ROL Y AL ESTUDIO ACTIVO POR CONTROL DE INTERNOS
        /// </summary>
        private void LocalizaIndexActual()
        {
            try
            {
                if (SelectIngreso != null)
                    InicializaPerfilUsuario();

                if (VisibleComun)
                {
                    ValidaIndicesComunes();
                    IndexComun = -1;
                    if (EnabledComun1)
                    {
                        if (CuantosActivos == decimal.Zero)
                        {
                            var _UltimoMovimiento = new cIngresoUbicacion().ObtenerUltimaUbicacion(SelectIngreso.ID_ANIO, SelectIngreso.ID_CENTRO, SelectIngreso.ID_IMPUTADO, SelectIngreso.ID_INGRESO);

                            var _EstudioPorValidar = new cEstudioPersonalidad().GetData(x => x.ID_INGRESO == SelectIngreso.ID_INGRESO && x.ID_IMPUTADO == SelectIngreso.ID_IMPUTADO && x.ID_CENTRO == SelectIngreso.ID_CENTRO && x.ID_ANIO == SelectIngreso.ID_ANIO).OrderByDescending(x => x.ID_ESTUDIO).FirstOrDefault();

                            if (_EstudioPorValidar != null)
                            {
                                var _detallesPersonalidad = new cPersonalidadDetalle().GetData(x => x.ID_INGRESO == SelectIngreso.ID_INGRESO && x.ID_IMPUTADO == SelectIngreso.ID_IMPUTADO && x.ID_CENTRO == SelectIngreso.ID_CENTRO && x.ID_ANIO == SelectIngreso.ID_ANIO && x.ID_ESTUDIO == _EstudioPorValidar.ID_ESTUDIO);

                                var _detallePersonalidadDetallePsicologicoComun = _detallesPersonalidad != null ? _detallesPersonalidad.Any() ? _detallesPersonalidad.FirstOrDefault(x => x.ID_TIPO == 4) : null : null;
                                System.DateTime _fechaH = Fechas.GetFechaDateServer;
                                if (_detallePersonalidadDetallePsicologicoComun != null)
                                {
                                    if (_detallePersonalidadDetallePsicologicoComun.PERSONALIDAD_DETALLE_DIAS != null && _detallePersonalidadDetallePsicologicoComun.PERSONALIDAD_DETALLE_DIAS.Any())
                                    {
                                        var _detalleDiasProgramados = _detallePersonalidadDetallePsicologicoComun.PERSONALIDAD_DETALLE_DIAS.Where(x => x.FECHA_INICIO.Value.Year == _fechaH.Year && x.FECHA_INICIO.Value.Month == _fechaH.Month && x.FECHA_INICIO.Value.Day == _fechaH.Day && x.PERSONALIDAD_DETALLE.ID_ESTATUS == (short)eEstatudDetallePersonalidad.ASIGNADO && x.ID_ESTUDIO == _detallePersonalidadDetallePsicologicoComun.ID_ESTUDIO);
                                        if (_detalleDiasProgramados != null && _detalleDiasProgramados.Any())
                                        {
                                            if (_detalleDiasProgramados.Any(x => x.FECHA_INICIO.Value.Year == _fechaH.Year && x.FECHA_INICIO.Value.Month == _fechaH.Month && x.FECHA_INICIO.Value.Day == _fechaH.Day))
                                                if (_UltimoMovimiento.ESTATUS == 1 || _UltimoMovimiento.ESTATUS == 2)
                                                {
                                                    if (_detalleDiasProgramados.Any(x => x.ID_AREA == _UltimoMovimiento.ID_AREA))
                                                    {
                                                        NombreUltimoTabElegido = "TabEstudioMedicoFC";
                                                        TabEstudioMedicoFC = true;
                                                    };
                                                };
                                        };
                                    };
                                };
                            }; //= validaciones
                        }

                        if (TabEstudioMedicoFC)
                        {
                            if (NombreUltimoTabElegido != "TabEstudioMedicoFC")
                            {
                                ///ESTA TRATANDO DE CAMBIAR DE TAB , HAY QUE VALIDAR POR BIOMETRIA AL INTERNO
                                EstaNavegandoTabs = true;
                                GuardaAislado(NombreUltimoTabElegido);//GUARDA ANTES DE IRTE
                                ValidaHuellasNavegacion();
                            }

                            NombreUltimoTabElegido = "TabEstudioMedicoFC";
                            TabEstudioPsiqFC = TabEstudioPsicFC = TabCriminoDFC = TabEstudioSocioFamFC = TabEstudioEducCultDepFC = TabEstudioCapacitacionTrabajoPenitFC = TabSeguriddCustodiaFC = false;
                            IndexComun = 0;
                            TabEstudioMedicoFC = true;
                            IdVentanaAcual = (short)eVentanasProceso.ESTUDIO_MEDICO_FUERO_COMUN;
                            OnPropertyChanged("TabEstudioMedicoFC");
                            OnPropertyChanged("IdVentanaAcual");
                            IngresaNuevaUbicacion((short)eProcEstudiosNuevasUbicacionesP.ESTUDIO_MEDICO_DE_FUERO_COMUN);
                        };
                    }

                    if (EnabledComun2)
                    {
                        if (ValidaIndicesComunes() == decimal.Zero)
                        {
                            var _UltimoMovimiento = new cIngresoUbicacion().ObtenerUltimaUbicacion(SelectIngreso.ID_ANIO, SelectIngreso.ID_CENTRO, SelectIngreso.ID_IMPUTADO, SelectIngreso.ID_INGRESO);

                            var _EstudioPorValidar = new cEstudioPersonalidad().GetData(x => x.ID_INGRESO == SelectIngreso.ID_INGRESO && x.ID_IMPUTADO == SelectIngreso.ID_IMPUTADO && x.ID_CENTRO == SelectIngreso.ID_CENTRO && x.ID_ANIO == SelectIngreso.ID_ANIO).OrderByDescending(x => x.ID_ESTUDIO).FirstOrDefault();

                            if (_EstudioPorValidar != null)
                            {
                                var _detallesPersonalidad = new cPersonalidadDetalle().GetData(x => x.ID_INGRESO == SelectIngreso.ID_INGRESO && x.ID_IMPUTADO == SelectIngreso.ID_IMPUTADO && x.ID_CENTRO == SelectIngreso.ID_CENTRO && x.ID_ANIO == SelectIngreso.ID_ANIO && x.ID_ESTUDIO == _EstudioPorValidar.ID_ESTUDIO);

                                var _detallePersonalidadDetallePsicologicoComun = _detallesPersonalidad != null ? _detallesPersonalidad.Any() ? _detallesPersonalidad.FirstOrDefault(x => x.ID_TIPO == 6) : null : null;
                                System.DateTime _fechaH = Fechas.GetFechaDateServer;
                                if (_detallePersonalidadDetallePsicologicoComun != null)
                                {
                                    if (_detallePersonalidadDetallePsicologicoComun.PERSONALIDAD_DETALLE_DIAS != null && _detallePersonalidadDetallePsicologicoComun.PERSONALIDAD_DETALLE_DIAS.Any())
                                    {
                                        var _detalleDiasProgramados = _detallePersonalidadDetallePsicologicoComun.PERSONALIDAD_DETALLE_DIAS.Where(x => x.FECHA_INICIO.Value.Year == _fechaH.Year && x.FECHA_INICIO.Value.Month == _fechaH.Month && x.FECHA_INICIO.Value.Day == _fechaH.Day && x.PERSONALIDAD_DETALLE.ID_ESTATUS == (short)eEstatudDetallePersonalidad.ASIGNADO && x.ID_ESTUDIO == _detallePersonalidadDetallePsicologicoComun.ID_ESTUDIO);
                                        if (_detalleDiasProgramados != null && _detalleDiasProgramados.Any())
                                        {
                                            if (_detalleDiasProgramados.Any(x => x.FECHA_INICIO.Value.Year == _fechaH.Year && x.FECHA_INICIO.Value.Month == _fechaH.Month && x.FECHA_INICIO.Value.Day == _fechaH.Day))
                                                if (_UltimoMovimiento.ESTATUS == 1 || _UltimoMovimiento.ESTATUS == 2)
                                                {
                                                    if (_detalleDiasProgramados.Any(x => x.ID_AREA == _UltimoMovimiento.ID_AREA))
                                                    {
                                                        NombreUltimoTabElegido = "TabEstudioPsiqFC";
                                                        TabEstudioPsiqFC = true;
                                                    };
                                                };
                                        };
                                    };
                                };
                            }; //= validaciones
                        };

                        if (TabEstudioPsiqFC)
                        {
                            if (NombreUltimoTabElegido != "TabEstudioPsiqFC")
                            {
                                EstaNavegandoTabs = true;
                                ///ESTA TRATANDO DE CAMBIAR DE TAB , HAY QUE VALIDAR POR BIOMETRIA AL INTERNO
                                GuardaAislado(NombreUltimoTabElegido);//GUARDA ANTES DE IRTE
                                ValidaHuellasNavegacion();
                            };

                            NombreUltimoTabElegido = "TabEstudioPsiqFC";
                            TabEstudioMedicoFC = TabEstudioPsicFC = TabCriminoDFC = TabEstudioSocioFamFC = TabEstudioEducCultDepFC = TabEstudioCapacitacionTrabajoPenitFC = TabSeguriddCustodiaFC = false;
                            IndexComun = 1;
                            IdVentanaAcual = (short)eVentanasProceso.ESTUDIO_PSIQUIATRICO_FUERO_COMUN;
                            TabEstudioPsiqFC = true;
                            OnPropertyChanged("TabEstudioPsiqFC");
                            OnPropertyChanged("IdVentanaAcual"); IngresaNuevaUbicacion((short)eProcEstudiosNuevasUbicacionesP.ESTUDIO_PSIQUIATRICO_DE_FUERO_COMUN);
                        }
                    }

                    if (EnabledComun3)
                    {
                        if (ValidaIndicesComunes() == decimal.Zero)
                        {
                            var _UltimoMovimiento = new cIngresoUbicacion().ObtenerUltimaUbicacion(SelectIngreso.ID_ANIO, SelectIngreso.ID_CENTRO, SelectIngreso.ID_IMPUTADO, SelectIngreso.ID_INGRESO);

                            var _EstudioPorValidar = new cEstudioPersonalidad().GetData(x => x.ID_INGRESO == SelectIngreso.ID_INGRESO && x.ID_IMPUTADO == SelectIngreso.ID_IMPUTADO && x.ID_CENTRO == SelectIngreso.ID_CENTRO && x.ID_ANIO == SelectIngreso.ID_ANIO).OrderByDescending(x => x.ID_ESTUDIO).FirstOrDefault();

                            if (_EstudioPorValidar != null)
                            {
                                var _detallesPersonalidad = new cPersonalidadDetalle().GetData(x => x.ID_INGRESO == SelectIngreso.ID_INGRESO && x.ID_IMPUTADO == SelectIngreso.ID_IMPUTADO && x.ID_CENTRO == SelectIngreso.ID_CENTRO && x.ID_ANIO == SelectIngreso.ID_ANIO && x.ID_ESTUDIO == _EstudioPorValidar.ID_ESTUDIO);

                                var _detallePersonalidadDetallePsicologicoComun = _detallesPersonalidad != null ? _detallesPersonalidad.Any() ? _detallesPersonalidad.FirstOrDefault(x => x.ID_TIPO == 5) : null : null;
                                System.DateTime _fechaH = Fechas.GetFechaDateServer;
                                if (_detallePersonalidadDetallePsicologicoComun != null)
                                {
                                    if (_detallePersonalidadDetallePsicologicoComun.PERSONALIDAD_DETALLE_DIAS != null && _detallePersonalidadDetallePsicologicoComun.PERSONALIDAD_DETALLE_DIAS.Any())
                                    {
                                        var _detalleDiasProgramados = _detallePersonalidadDetallePsicologicoComun.PERSONALIDAD_DETALLE_DIAS.Where(x => x.FECHA_INICIO.Value.Year == _fechaH.Year && x.FECHA_INICIO.Value.Month == _fechaH.Month && x.FECHA_INICIO.Value.Day == _fechaH.Day && x.PERSONALIDAD_DETALLE.ID_ESTATUS == (short)eEstatudDetallePersonalidad.ASIGNADO && x.ID_ESTUDIO == _detallePersonalidadDetallePsicologicoComun.ID_ESTUDIO);
                                        if (_detalleDiasProgramados != null && _detalleDiasProgramados.Any())
                                        {
                                            if (_detalleDiasProgramados.Any(x => x.FECHA_INICIO.Value.Year == _fechaH.Year && x.FECHA_INICIO.Value.Month == _fechaH.Month && x.FECHA_INICIO.Value.Day == _fechaH.Day))
                                                if (_UltimoMovimiento.ESTATUS == 1 || _UltimoMovimiento.ESTATUS == 2)
                                                {
                                                    if (_detalleDiasProgramados.Any(x => x.ID_AREA == _UltimoMovimiento.ID_AREA))
                                                    {
                                                        NombreUltimoTabElegido = "TabEstudioPsicFC";
                                                        TabEstudioPsicFC = true;
                                                    };
                                                };
                                        };
                                    };
                                };
                            }; //= validaciones
                        };// = 0

                        if (TabEstudioPsicFC)
                        {
                            if (NombreUltimoTabElegido != "TabEstudioPsicFC")
                            {
                                EstaNavegandoTabs = true;
                                ///ESTA TRATANDO DE CAMBIAR DE TAB , HAY QUE VALIDAR POR BIOMETRIA AL INTERNO
                                GuardaAislado(NombreUltimoTabElegido);//GUARDA ANTES DE IRTE
                                ValidaHuellasNavegacion();
                            };

                            NombreUltimoTabElegido = "TabEstudioPsicFC";
                            TabEstudioMedicoFC = TabEstudioPsiqFC = TabCriminoDFC = TabEstudioSocioFamFC = TabEstudioEducCultDepFC = TabEstudioCapacitacionTrabajoPenitFC = TabSeguriddCustodiaFC = false;
                            IndexComun = 2;
                            TabEstudioPsicFC = true;
                            IdVentanaAcual = (short)eVentanasProceso.ESTUDIO_PSICOLOGICO_FUERO_COMUN;
                            OnPropertyChanged("TabEstudioPsicFC");
                            OnPropertyChanged("IdVentanaAcual");
                            IngresaNuevaUbicacion((short)eProcEstudiosNuevasUbicacionesP.ESTUDIO_PSICOLOGICO_DE_FUERO_COMUN);
                        }
                    }

                    if (EnabledComun4)
                    {
                        if (ValidaIndicesComunes() == decimal.Zero)
                        {
                            var _UltimoMovimiento = new cIngresoUbicacion().ObtenerUltimaUbicacion(SelectIngreso.ID_ANIO, SelectIngreso.ID_CENTRO, SelectIngreso.ID_IMPUTADO, SelectIngreso.ID_INGRESO);

                            var _EstudioPorValidar = new cEstudioPersonalidad().GetData(x => x.ID_INGRESO == SelectIngreso.ID_INGRESO && x.ID_IMPUTADO == SelectIngreso.ID_IMPUTADO && x.ID_CENTRO == SelectIngreso.ID_CENTRO && x.ID_ANIO == SelectIngreso.ID_ANIO).OrderByDescending(x => x.ID_ESTUDIO).FirstOrDefault();

                            if (_EstudioPorValidar != null)
                            {
                                var _detallesPersonalidad = new cPersonalidadDetalle().GetData(x => x.ID_INGRESO == SelectIngreso.ID_INGRESO && x.ID_IMPUTADO == SelectIngreso.ID_IMPUTADO && x.ID_CENTRO == SelectIngreso.ID_CENTRO && x.ID_ANIO == SelectIngreso.ID_ANIO && x.ID_ESTUDIO == _EstudioPorValidar.ID_ESTUDIO);

                                var _detallePersonalidadDetallePsicologicoComun = _detallesPersonalidad != null ? _detallesPersonalidad.Any() ? _detallesPersonalidad.FirstOrDefault(x => x.ID_TIPO == 1) : null : null;
                                System.DateTime _fechaH = Fechas.GetFechaDateServer;
                                if (_detallePersonalidadDetallePsicologicoComun != null)
                                {
                                    if (_detallePersonalidadDetallePsicologicoComun.PERSONALIDAD_DETALLE_DIAS != null && _detallePersonalidadDetallePsicologicoComun.PERSONALIDAD_DETALLE_DIAS.Any())
                                    {
                                        var _detalleDiasProgramados = _detallePersonalidadDetallePsicologicoComun.PERSONALIDAD_DETALLE_DIAS.Where(x => x.FECHA_INICIO.Value.Year == _fechaH.Year && x.FECHA_INICIO.Value.Month == _fechaH.Month && x.FECHA_INICIO.Value.Day == _fechaH.Day && x.PERSONALIDAD_DETALLE.ID_ESTATUS == (short)eEstatudDetallePersonalidad.ASIGNADO && x.ID_ESTUDIO == _detallePersonalidadDetallePsicologicoComun.ID_ESTUDIO);
                                        if (_detalleDiasProgramados != null && _detalleDiasProgramados.Any())
                                        {
                                            if (_detalleDiasProgramados.Any(x => x.FECHA_INICIO.Value.Year == _fechaH.Year && x.FECHA_INICIO.Value.Month == _fechaH.Month && x.FECHA_INICIO.Value.Day == _fechaH.Day))
                                                if (_UltimoMovimiento.ESTATUS == 1 || _UltimoMovimiento.ESTATUS == 2)
                                                {
                                                    if (_detalleDiasProgramados.Any(x => x.ID_AREA == _UltimoMovimiento.ID_AREA))
                                                    {
                                                        NombreUltimoTabElegido = "TabCriminoDFC";
                                                        TabCriminoDFC = true;
                                                    };
                                                };
                                        };
                                    };
                                };
                            }; //= validaciones
                        };

                        if (TabCriminoDFC)
                        {
                            if (NombreUltimoTabElegido != "TabCriminoDFC")
                            {
                                EstaNavegandoTabs = true;
                                ///ESTA TRATANDO DE CAMBIAR DE TAB , HAY QUE VALIDAR POR BIOMETRIA AL INTERNO
                                GuardaAislado(NombreUltimoTabElegido);//GUARDA ANTES DE IRTE
                                ValidaHuellasNavegacion();
                            };

                            NombreUltimoTabElegido = "TabCriminoDFC";
                            TabEstudioMedicoFC = TabEstudioPsiqFC = TabEstudioPsicFC = TabEstudioSocioFamFC = TabEstudioEducCultDepFC = TabEstudioCapacitacionTrabajoPenitFC = TabSeguriddCustodiaFC = false;
                            IdVentanaAcual = (short)eVentanasProceso.ESTUDIO_CRIMINODIAGNOSTICO_FUERO_COMUN;
                            IndexComun = 3;
                            TabCriminoDFC = true;
                            OnPropertyChanged("TabCriminoDFC");
                            OnPropertyChanged("IdVentanaAcual");
                            IngresaNuevaUbicacion((short)eProcEstudiosNuevasUbicacionesP.ESTUDIO_CRIMINOLOGICO_DE_FUERO_COMUN);
                        }
                    }

                    if (EnabledComun5)
                    {
                        if (ValidaIndicesComunes() == decimal.Zero)
                        {
                            var _UltimoMovimiento = new cIngresoUbicacion().ObtenerUltimaUbicacion(SelectIngreso.ID_ANIO, SelectIngreso.ID_CENTRO, SelectIngreso.ID_IMPUTADO, SelectIngreso.ID_INGRESO);

                            var _EstudioPorValidar = new cEstudioPersonalidad().GetData(x => x.ID_INGRESO == SelectIngreso.ID_INGRESO && x.ID_IMPUTADO == SelectIngreso.ID_IMPUTADO && x.ID_CENTRO == SelectIngreso.ID_CENTRO && x.ID_ANIO == SelectIngreso.ID_ANIO).OrderByDescending(x => x.ID_ESTUDIO).FirstOrDefault();

                            if (_EstudioPorValidar != null)
                            {
                                var _detallesPersonalidad = new cPersonalidadDetalle().GetData(x => x.ID_INGRESO == SelectIngreso.ID_INGRESO && x.ID_IMPUTADO == SelectIngreso.ID_IMPUTADO && x.ID_CENTRO == SelectIngreso.ID_CENTRO && x.ID_ANIO == SelectIngreso.ID_ANIO && x.ID_ESTUDIO == _EstudioPorValidar.ID_ESTUDIO);

                                var _detallePersonalidadDetallePsicologicoComun = _detallesPersonalidad != null ? _detallesPersonalidad.Any() ? _detallesPersonalidad.FirstOrDefault(x => x.ID_TIPO == 2) : null : null;
                                System.DateTime _fechaH = Fechas.GetFechaDateServer;
                                if (_detallePersonalidadDetallePsicologicoComun != null)
                                {
                                    if (_detallePersonalidadDetallePsicologicoComun.PERSONALIDAD_DETALLE_DIAS != null && _detallePersonalidadDetallePsicologicoComun.PERSONALIDAD_DETALLE_DIAS.Any())
                                    {
                                        var _detalleDiasProgramados = _detallePersonalidadDetallePsicologicoComun.PERSONALIDAD_DETALLE_DIAS.Where(x => x.FECHA_INICIO.Value.Year == _fechaH.Year && x.FECHA_INICIO.Value.Month == _fechaH.Month && x.FECHA_INICIO.Value.Day == _fechaH.Day && x.PERSONALIDAD_DETALLE.ID_ESTATUS == (short)eEstatudDetallePersonalidad.ASIGNADO && x.ID_ESTUDIO == _detallePersonalidadDetallePsicologicoComun.ID_ESTUDIO);
                                        if (_detalleDiasProgramados != null && _detalleDiasProgramados.Any())
                                        {
                                            if (_detalleDiasProgramados.Any(x => x.FECHA_INICIO.Value.Year == _fechaH.Year && x.FECHA_INICIO.Value.Month == _fechaH.Month && x.FECHA_INICIO.Value.Day == _fechaH.Day))
                                                if (_UltimoMovimiento.ESTATUS == 1 || _UltimoMovimiento.ESTATUS == 2)
                                                {
                                                    if (_detalleDiasProgramados.Any(x => x.ID_AREA == _UltimoMovimiento.ID_AREA))
                                                    {
                                                        NombreUltimoTabElegido = "TabEstudioSocioFamFC";
                                                        TabEstudioSocioFamFC = true;
                                                    };
                                                };
                                        };
                                    };
                                };
                            }; //= validaciones
                        };

                        if (TabEstudioSocioFamFC)
                        {
                            if (NombreUltimoTabElegido != "TabEstudioSocioFamFC")
                            {
                                EstaNavegandoTabs = true;
                                ///ESTA TRATANDO DE CAMBIAR DE TAB , HAY QUE VALIDAR POR BIOMETRIA AL INTERNO
                                GuardaAislado(NombreUltimoTabElegido);//GUARDA ANTES DE IRTE
                                ValidaHuellasNavegacion();
                            };

                            NombreUltimoTabElegido = "TabEstudioSocioFamFC";
                            TabEstudioMedicoFC = TabEstudioPsiqFC = TabEstudioPsicFC = TabCriminoDFC = TabEstudioEducCultDepFC = TabEstudioCapacitacionTrabajoPenitFC = TabSeguriddCustodiaFC = false;
                            IndexComun = 4;
                            TabEstudioSocioFamFC = true;
                            IdVentanaAcual = (short)eVentanasProceso.ESTUDIO_SOCIOFAMILIAR_FUERO_COMUN;
                            OnPropertyChanged("TabEstudioSocioFamFC");
                            OnPropertyChanged("IdVentanaAcual");
                            IngresaNuevaUbicacion((short)eProcEstudiosNuevasUbicacionesP.ESTUDIO_DE_TRABAJO_SOCIAL_DE_FUERO_COMUN);
                        }
                    }

                    if (EnabledComun6)
                    {
                        if (ValidaIndicesComunes() == decimal.Zero)
                        {
                            var _UltimoMovimiento = new cIngresoUbicacion().ObtenerUltimaUbicacion(SelectIngreso.ID_ANIO, SelectIngreso.ID_CENTRO, SelectIngreso.ID_IMPUTADO, SelectIngreso.ID_INGRESO);

                            var _EstudioPorValidar = new cEstudioPersonalidad().GetData(x => x.ID_INGRESO == SelectIngreso.ID_INGRESO && x.ID_IMPUTADO == SelectIngreso.ID_IMPUTADO && x.ID_CENTRO == SelectIngreso.ID_CENTRO && x.ID_ANIO == SelectIngreso.ID_ANIO).OrderByDescending(x => x.ID_ESTUDIO).FirstOrDefault();

                            if (_EstudioPorValidar != null)
                            {
                                var _detallesPersonalidad = new cPersonalidadDetalle().GetData(x => x.ID_INGRESO == SelectIngreso.ID_INGRESO && x.ID_IMPUTADO == SelectIngreso.ID_IMPUTADO && x.ID_CENTRO == SelectIngreso.ID_CENTRO && x.ID_ANIO == SelectIngreso.ID_ANIO && x.ID_ESTUDIO == _EstudioPorValidar.ID_ESTUDIO);

                                var _detallePersonalidadDetallePsicologicoComun = _detallesPersonalidad != null ? _detallesPersonalidad.Any() ? _detallesPersonalidad.FirstOrDefault(x => x.ID_TIPO == 7) : null : null;
                                System.DateTime _fechaH = Fechas.GetFechaDateServer;
                                if (_detallePersonalidadDetallePsicologicoComun != null)
                                {
                                    if (_detallePersonalidadDetallePsicologicoComun.PERSONALIDAD_DETALLE_DIAS != null && _detallePersonalidadDetallePsicologicoComun.PERSONALIDAD_DETALLE_DIAS.Any())
                                    {
                                        var _detalleDiasProgramados = _detallePersonalidadDetallePsicologicoComun.PERSONALIDAD_DETALLE_DIAS.Where(x => x.FECHA_INICIO.Value.Year == _fechaH.Year && x.FECHA_INICIO.Value.Month == _fechaH.Month && x.FECHA_INICIO.Value.Day == _fechaH.Day && x.PERSONALIDAD_DETALLE.ID_ESTATUS == (short)eEstatudDetallePersonalidad.ASIGNADO && x.ID_ESTUDIO == _detallePersonalidadDetallePsicologicoComun.ID_ESTUDIO);
                                        if (_detalleDiasProgramados != null && _detalleDiasProgramados.Any())
                                        {
                                            if (_detalleDiasProgramados.Any(x => x.FECHA_INICIO.Value.Year == _fechaH.Year && x.FECHA_INICIO.Value.Month == _fechaH.Month && x.FECHA_INICIO.Value.Day == _fechaH.Day))
                                                if (_UltimoMovimiento.ESTATUS == 1 || _UltimoMovimiento.ESTATUS == 2)
                                                {
                                                    if (_detalleDiasProgramados.Any(x => x.ID_AREA == _UltimoMovimiento.ID_AREA))
                                                    {
                                                        NombreUltimoTabElegido = "TabEstudioEducCultDepFC";
                                                        TabEstudioEducCultDepFC = true;
                                                    };
                                                };
                                        };
                                    };
                                };
                            }; //= validaciones
                        }

                        if (TabEstudioEducCultDepFC)
                        {
                            if (NombreUltimoTabElegido != "TabEstudioEducCultDepFC")
                            {
                                EstaNavegandoTabs = true;
                                ///ESTA TRATANDO DE CAMBIAR DE TAB , HAY QUE VALIDAR POR BIOMETRIA AL INTERNO
                                GuardaAislado(NombreUltimoTabElegido);//GUARDA ANTES DE IRTE
                                ValidaHuellasNavegacion();
                            };

                            NombreUltimoTabElegido = "TabEstudioEducCultDepFC";
                            TabEstudioMedicoFC = TabEstudioPsiqFC = TabEstudioPsicFC = TabCriminoDFC = TabEstudioSocioFamFC = TabEstudioCapacitacionTrabajoPenitFC = TabSeguriddCustodiaFC = false;
                            IndexComun = 5;
                            TabEstudioEducCultDepFC = true;
                            IdVentanaAcual = (short)eVentanasProceso.ESTUDIO_EDUCATIVO_CULTURAL_DEPORTIVO_FUERO_COMUN;
                            OnPropertyChanged("TabEstudioEducCultDepFC");
                            OnPropertyChanged("IdVentanaAcual");
                            IngresaNuevaUbicacion((short)eProcEstudiosNuevasUbicacionesP.ESTUDIO_PEDAGOGICO_DE_FUERO_COMUN);
                        }
                    }

                    if (EnabledComun7)
                    {
                        if (ValidaIndicesComunes() == decimal.Zero)
                        {
                            var _UltimoMovimiento = new cIngresoUbicacion().ObtenerUltimaUbicacion(SelectIngreso.ID_ANIO, SelectIngreso.ID_CENTRO, SelectIngreso.ID_IMPUTADO, SelectIngreso.ID_INGRESO);

                            var _EstudioPorValidar = new cEstudioPersonalidad().GetData(x => x.ID_INGRESO == SelectIngreso.ID_INGRESO && x.ID_IMPUTADO == SelectIngreso.ID_IMPUTADO && x.ID_CENTRO == SelectIngreso.ID_CENTRO && x.ID_ANIO == SelectIngreso.ID_ANIO).OrderByDescending(x => x.ID_ESTUDIO).FirstOrDefault();

                            if (_EstudioPorValidar != null)
                            {
                                var _detallesPersonalidad = new cPersonalidadDetalle().GetData(x => x.ID_INGRESO == SelectIngreso.ID_INGRESO && x.ID_IMPUTADO == SelectIngreso.ID_IMPUTADO && x.ID_CENTRO == SelectIngreso.ID_CENTRO && x.ID_ANIO == SelectIngreso.ID_ANIO && x.ID_ESTUDIO == _EstudioPorValidar.ID_ESTUDIO);

                                var _detallePersonalidadDetallePsicologicoComun = _detallesPersonalidad != null ? _detallesPersonalidad.Any() ? _detallesPersonalidad.FirstOrDefault(x => x.ID_TIPO == 8 && x.ID_ESTUDIO == _EstudioPorValidar.ID_ESTUDIO && x.ID_INGRESO == SelectIngreso.ID_INGRESO && x.ID_IMPUTADO == SelectIngreso.ID_IMPUTADO && x.ID_ANIO == SelectIngreso.ID_ANIO && x.ID_CENTRO == SelectIngreso.ID_CENTRO) : null : null;
                                System.DateTime _fechaH = Fechas.GetFechaDateServer;
                                if (_detallePersonalidadDetallePsicologicoComun != null)
                                {
                                    if (_detallePersonalidadDetallePsicologicoComun.PERSONALIDAD_DETALLE_DIAS != null && _detallePersonalidadDetallePsicologicoComun.PERSONALIDAD_DETALLE_DIAS.Any())
                                    {
                                        var _detalleDiasProgramados = _detallePersonalidadDetallePsicologicoComun.PERSONALIDAD_DETALLE_DIAS.Where(x => x.FECHA_INICIO.Value.Year == _fechaH.Year && x.FECHA_INICIO.Value.Month == _fechaH.Month && x.FECHA_INICIO.Value.Day == _fechaH.Day && x.PERSONALIDAD_DETALLE.ID_ESTATUS == (short)eEstatudDetallePersonalidad.ASIGNADO && x.ID_ESTUDIO == _detallePersonalidadDetallePsicologicoComun.ID_ESTUDIO);
                                        if (_detalleDiasProgramados != null && _detalleDiasProgramados.Any())
                                        {
                                            if (_detalleDiasProgramados.Any(x => x.FECHA_INICIO.Value.Year == _fechaH.Year && x.FECHA_INICIO.Value.Month == _fechaH.Month && x.FECHA_INICIO.Value.Day == _fechaH.Day))
                                                if (_UltimoMovimiento.ESTATUS == 1 || _UltimoMovimiento.ESTATUS == 2)
                                                {
                                                    if (_detalleDiasProgramados.Any(x => x.ID_AREA == _UltimoMovimiento.ID_AREA))
                                                    {
                                                        NombreUltimoTabElegido = "TabEstudioCapacitacionTrabajoPenitFC";
                                                        TabEstudioCapacitacionTrabajoPenitFC = true;
                                                    };
                                                };
                                        };
                                    };
                                };
                            }; //= validaciones
                        };

                        if (TabEstudioCapacitacionTrabajoPenitFC)
                        {
                            if (NombreUltimoTabElegido != "TabEstudioCapacitacionTrabajoPenitFC")
                            {
                                EstaNavegandoTabs = true;
                                ///ESTA TRATANDO DE CAMBIAR DE TAB , HAY QUE VALIDAR POR BIOMETRIA AL INTERNO
                                GuardaAislado(NombreUltimoTabElegido);//GUARDA ANTES DE IRTE
                                ValidaHuellasNavegacion();
                            };

                            NombreUltimoTabElegido = "TabEstudioCapacitacionTrabajoPenitFC";
                            TabEstudioMedicoFC = TabEstudioPsiqFC = TabEstudioPsicFC = TabCriminoDFC = TabEstudioSocioFamFC = TabEstudioEducCultDepFC = TabSeguriddCustodiaFC = false;
                            IndexComun = 6;
                            TabEstudioCapacitacionTrabajoPenitFC = true;
                            IdVentanaAcual = (short)eVentanasProceso.ESTUDIO_CAPACITACION_TRABAJO_PENITENCIARIO_FUERO_COMUN;
                            OnPropertyChanged("TabEstudioCapacitacionTrabajoPenitFC");
                            OnPropertyChanged("IdVentanaAcual");
                            IngresaNuevaUbicacion((short)eProcEstudiosNuevasUbicacionesP.ESTUDIO_LABORAL_DE_FUERO_COMUN);
                        }
                    }

                    if (EnabledComun8)
                    {
                        if (ValidaIndicesComunes() == decimal.Zero)
                        {
                            var _UltimoMovimiento = new cIngresoUbicacion().ObtenerUltimaUbicacion(SelectIngreso.ID_ANIO, SelectIngreso.ID_CENTRO, SelectIngreso.ID_IMPUTADO, SelectIngreso.ID_INGRESO);

                            var _EstudioPorValidar = new cEstudioPersonalidad().GetData(x => x.ID_INGRESO == SelectIngreso.ID_INGRESO && x.ID_IMPUTADO == SelectIngreso.ID_IMPUTADO && x.ID_CENTRO == SelectIngreso.ID_CENTRO && x.ID_ANIO == SelectIngreso.ID_ANIO).OrderByDescending(x => x.ID_ESTUDIO).FirstOrDefault();

                            if (_EstudioPorValidar != null)
                            {
                                var _detallesPersonalidad = new cPersonalidadDetalle().GetData(x => x.ID_INGRESO == SelectIngreso.ID_INGRESO && x.ID_IMPUTADO == SelectIngreso.ID_IMPUTADO && x.ID_CENTRO == SelectIngreso.ID_CENTRO && x.ID_ANIO == SelectIngreso.ID_ANIO && x.ID_ESTUDIO == _EstudioPorValidar.ID_ESTUDIO);

                                var _detallePersonalidadDetallePsicologicoComun = _detallesPersonalidad != null ? _detallesPersonalidad.Any() ? _detallesPersonalidad.FirstOrDefault(x => x.ID_TIPO == 3) : null : null;
                                System.DateTime _fechaH = Fechas.GetFechaDateServer;
                                if (_detallePersonalidadDetallePsicologicoComun != null)
                                {
                                    if (_detallePersonalidadDetallePsicologicoComun.PERSONALIDAD_DETALLE_DIAS != null && _detallePersonalidadDetallePsicologicoComun.PERSONALIDAD_DETALLE_DIAS.Any())
                                    {
                                        var _detalleDiasProgramados = _detallePersonalidadDetallePsicologicoComun.PERSONALIDAD_DETALLE_DIAS.Where(x => x.FECHA_INICIO.Value.Year == _fechaH.Year && x.FECHA_INICIO.Value.Month == _fechaH.Month && x.FECHA_INICIO.Value.Day == _fechaH.Day && x.PERSONALIDAD_DETALLE.ID_ESTATUS == (short)eEstatudDetallePersonalidad.ASIGNADO && x.ID_ESTUDIO == _detallePersonalidadDetallePsicologicoComun.ID_ESTUDIO);
                                        if (_detalleDiasProgramados != null && _detalleDiasProgramados.Any())
                                        {
                                            if (_detalleDiasProgramados.Any(x => x.FECHA_INICIO.Value.Year == _fechaH.Year && x.FECHA_INICIO.Value.Month == _fechaH.Month && x.FECHA_INICIO.Value.Day == _fechaH.Day))
                                                if (_UltimoMovimiento.ESTATUS == 1 || _UltimoMovimiento.ESTATUS == 2)
                                                {
                                                    if (_detalleDiasProgramados.Any(x => x.ID_AREA == _UltimoMovimiento.ID_AREA))
                                                    {
                                                        NombreUltimoTabElegido = "TabSeguriddCustodiaFC";
                                                        TabSeguriddCustodiaFC = true;
                                                    };
                                                };
                                        };
                                    };
                                };
                            }; //= validaciones
                        }

                        if (TabSeguriddCustodiaFC)
                        {
                            if (NombreUltimoTabElegido != "TabSeguriddCustodiaFC")
                            {
                                EstaNavegandoTabs = true;
                                ///ESTA TRATANDO DE CAMBIAR DE TAB , HAY QUE VALIDAR POR BIOMETRIA AL INTERNO
                                GuardaAislado(NombreUltimoTabElegido);//GUARDA ANTES DE IRTE
                                ValidaHuellasNavegacion();
                            };

                            NombreUltimoTabElegido = "TabSeguriddCustodiaFC";
                            TabEstudioMedicoFC = TabEstudioPsiqFC = TabEstudioPsicFC = TabCriminoDFC = TabEstudioSocioFamFC = TabEstudioEducCultDepFC = TabEstudioCapacitacionTrabajoPenitFC = false;
                            IndexComun = 7;
                            TabSeguriddCustodiaFC = true;
                            IdVentanaAcual = (short)eVentanasProceso.ESTUDIO_SEGURIDAD_CUSTODIA_FUERO_COMUN;
                            OnPropertyChanged("TabSeguriddCustodiaFC");
                            OnPropertyChanged("IdVentanaAcual");
                            IngresaNuevaUbicacion((short)eProcEstudiosNuevasUbicacionesP.ESTUDIO_DE_SEGURIDAD_DE_FUERO_COMÚN);
                        }
                    }

                    OnPropertyChanged("IndexComun");
                }
                else
                {
                    if (VisibleFederal)
                    {
                        ValidaIndicesFederales();
                        IndexFederal = -1;
                        if (EnabledFederal1)
                        {
                            if (ValidaIndicesFederales() == decimal.Zero)
                            {
                                var _UltimoMovimiento = new cIngresoUbicacion().ObtenerUltimaUbicacion(SelectIngreso.ID_ANIO, SelectIngreso.ID_CENTRO, SelectIngreso.ID_IMPUTADO, SelectIngreso.ID_INGRESO);

                                var _EstudioPorValidar = new cEstudioPersonalidad().GetData(x => x.ID_INGRESO == SelectIngreso.ID_INGRESO && x.ID_IMPUTADO == SelectIngreso.ID_IMPUTADO && x.ID_CENTRO == SelectIngreso.ID_CENTRO && x.ID_ANIO == SelectIngreso.ID_ANIO).OrderByDescending(x => x.ID_ESTUDIO).FirstOrDefault();

                                if (_EstudioPorValidar != null)
                                {
                                    var _detallesPersonalidad = new cPersonalidadDetalle().GetData(x => x.ID_INGRESO == SelectIngreso.ID_INGRESO && x.ID_IMPUTADO == SelectIngreso.ID_IMPUTADO && x.ID_CENTRO == SelectIngreso.ID_CENTRO && x.ID_ANIO == SelectIngreso.ID_ANIO && x.ID_ESTUDIO == _EstudioPorValidar.ID_ESTUDIO);

                                    var _detallePersonalidadDetallePsicologicoComun = _detallesPersonalidad != null ? _detallesPersonalidad.Any() ? _detallesPersonalidad.FirstOrDefault(x => x.ID_TIPO == 12) : null : null;
                                    System.DateTime _fechaH = Fechas.GetFechaDateServer;
                                    if (_detallePersonalidadDetallePsicologicoComun != null)
                                    {
                                        if (_detallePersonalidadDetallePsicologicoComun.PERSONALIDAD_DETALLE_DIAS != null && _detallePersonalidadDetallePsicologicoComun.PERSONALIDAD_DETALLE_DIAS.Any())
                                        {
                                            var _detalleDiasProgramados = _detallePersonalidadDetallePsicologicoComun.PERSONALIDAD_DETALLE_DIAS.Where(x => x.FECHA_INICIO.Value.Year == _fechaH.Year && x.FECHA_INICIO.Value.Month == _fechaH.Month && x.FECHA_INICIO.Value.Day == _fechaH.Day && x.PERSONALIDAD_DETALLE.ID_ESTATUS == (short)eEstatudDetallePersonalidad.ASIGNADO && x.ID_ESTUDIO == _detallePersonalidadDetallePsicologicoComun.ID_ESTUDIO);
                                            if (_detalleDiasProgramados != null && _detalleDiasProgramados.Any())
                                            {
                                                if (_detalleDiasProgramados.Any(x => x.FECHA_INICIO.Value.Year == _fechaH.Year && x.FECHA_INICIO.Value.Month == _fechaH.Month && x.FECHA_INICIO.Value.Day == _fechaH.Day))
                                                    if (_UltimoMovimiento.ESTATUS == 1 || _UltimoMovimiento.ESTATUS == 2)
                                                    {
                                                        if (_detalleDiasProgramados.Any(x => x.ID_AREA == _UltimoMovimiento.ID_AREA))
                                                        {
                                                            NombreUltimoTabElegido = "TabEstudioMedicoFF";
                                                            TabEstudioMedicoFF = true;
                                                        };
                                                    };
                                            };
                                        };
                                    };
                                }; //= validaciones
                            };

                            if (TabEstudioMedicoFF)
                            {
                                if (NombreUltimoTabElegido != "TabEstudioMedicoFF")
                                {
                                    EstaNavegandoTabs = true;
                                    GuardaAislado(NombreUltimoTabElegido);
                                    ValidaHuellasNavegacion();
                                };

                                NombreUltimoTabElegido = "TabEstudioMedicoFF";
                                TabEstudioPsicoFF = TabEstudioTrabajoSocialFF = TabActivProductCapacitFF = TabActEducCultDepRecCivFF = TabVigilanciaFF = TabEstudioCriminologico = false;
                                IndexFederal = 0;
                                TabEstudioMedicoFF = true;
                                IdVentanaAcual = (short)eVentanasProceso.ESTUDIO_MEDICO_FUERO_FEDERAL;
                                OnPropertyChanged("TabEstudioMedicoFF");
                                OnPropertyChanged("IdVentanaAcual");
                                IngresaNuevaUbicacion((short)eProcEstudiosNuevasUbicacionesP.ESTUDIO_MEDICO_DE_FUERO_FEDERAL);
                            }
                        }

                        if (EnabledFederal2)
                        {
                            if (ValidaIndicesFederales() == decimal.Zero)
                            {
                                var _UltimoMovimiento = new cIngresoUbicacion().ObtenerUltimaUbicacion(SelectIngreso.ID_ANIO, SelectIngreso.ID_CENTRO, SelectIngreso.ID_IMPUTADO, SelectIngreso.ID_INGRESO);

                                var _EstudioPorValidar = new cEstudioPersonalidad().GetData(x => x.ID_INGRESO == SelectIngreso.ID_INGRESO && x.ID_IMPUTADO == SelectIngreso.ID_IMPUTADO && x.ID_CENTRO == SelectIngreso.ID_CENTRO && x.ID_ANIO == SelectIngreso.ID_ANIO).OrderByDescending(x => x.ID_ESTUDIO).FirstOrDefault();

                                if (_EstudioPorValidar != null)
                                {
                                    var _detallesPersonalidad = new cPersonalidadDetalle().GetData(x => x.ID_INGRESO == SelectIngreso.ID_INGRESO && x.ID_IMPUTADO == SelectIngreso.ID_IMPUTADO && x.ID_CENTRO == SelectIngreso.ID_CENTRO && x.ID_ANIO == SelectIngreso.ID_ANIO && x.ID_ESTUDIO == _EstudioPorValidar.ID_ESTUDIO);

                                    var _detallePersonalidadDetallePsicologicoComun = _detallesPersonalidad != null ? _detallesPersonalidad.Any() ? _detallesPersonalidad.FirstOrDefault(x => x.ID_TIPO == 14) : null : null;
                                    System.DateTime _fechaH = Fechas.GetFechaDateServer;
                                    if (_detallePersonalidadDetallePsicologicoComun != null)
                                    {
                                        if (_detallePersonalidadDetallePsicologicoComun.PERSONALIDAD_DETALLE_DIAS != null && _detallePersonalidadDetallePsicologicoComun.PERSONALIDAD_DETALLE_DIAS.Any())
                                        {
                                            var _detalleDiasProgramados = _detallePersonalidadDetallePsicologicoComun.PERSONALIDAD_DETALLE_DIAS.Where(x => x.FECHA_INICIO.Value.Year == _fechaH.Year && x.FECHA_INICIO.Value.Month == _fechaH.Month && x.FECHA_INICIO.Value.Day == _fechaH.Day && x.PERSONALIDAD_DETALLE.ID_ESTATUS == (short)eEstatudDetallePersonalidad.ASIGNADO && x.ID_ESTUDIO == _detallePersonalidadDetallePsicologicoComun.ID_ESTUDIO);
                                            if (_detalleDiasProgramados != null && _detalleDiasProgramados.Any())
                                            {
                                                if (_detalleDiasProgramados.Any(x => x.FECHA_INICIO.Value.Year == _fechaH.Year && x.FECHA_INICIO.Value.Month == _fechaH.Month && x.FECHA_INICIO.Value.Day == _fechaH.Day))
                                                    if (_UltimoMovimiento.ESTATUS == 1 || _UltimoMovimiento.ESTATUS == 2)
                                                    {
                                                        if (_detalleDiasProgramados.Any(x => x.ID_AREA == _UltimoMovimiento.ID_AREA))
                                                        {
                                                            NombreUltimoTabElegido = "TabEstudioPsicoFF";
                                                            TabEstudioPsicoFF = true;
                                                        };
                                                    };
                                            };
                                        };
                                    };
                                }; //= validaciones
                            };

                            if (TabEstudioPsicoFF)
                            {
                                if (NombreUltimoTabElegido != "TabEstudioPsicoFF")
                                {
                                    EstaNavegandoTabs = true;
                                    GuardaAislado(NombreUltimoTabElegido);
                                    ValidaHuellasNavegacion();
                                };

                                NombreUltimoTabElegido = "TabEstudioPsicoFF";
                                TabEstudioMedicoFF = TabEstudioTrabajoSocialFF = TabActivProductCapacitFF = TabActEducCultDepRecCivFF = TabVigilanciaFF = TabEstudioCriminologico = false;
                                IndexFederal = 1;
                                TabEstudioPsicoFF = true;
                                IdVentanaAcual = (short)eVentanasProceso.ESTUDIO_PSICOLOGICO_FUERO_FEDERAL;
                                OnPropertyChanged("TabEstudioPsicoFF");
                                OnPropertyChanged("IdVentanaAcual");
                                IngresaNuevaUbicacion((short)eProcEstudiosNuevasUbicacionesP.ESTUDIO_PSICOLOGICO_DE_FUERO_FEDERAL);
                            }
                        }

                        if (EnabledFederal3)
                        {
                            if (ValidaIndicesFederales() == decimal.Zero)
                            {
                                var _UltimoMovimiento = new cIngresoUbicacion().ObtenerUltimaUbicacion(SelectIngreso.ID_ANIO, SelectIngreso.ID_CENTRO, SelectIngreso.ID_IMPUTADO, SelectIngreso.ID_INGRESO);

                                var _EstudioPorValidar = new cEstudioPersonalidad().GetData(x => x.ID_INGRESO == SelectIngreso.ID_INGRESO && x.ID_IMPUTADO == SelectIngreso.ID_IMPUTADO && x.ID_CENTRO == SelectIngreso.ID_CENTRO && x.ID_ANIO == SelectIngreso.ID_ANIO).OrderByDescending(x => x.ID_ESTUDIO).FirstOrDefault();

                                if (_EstudioPorValidar != null)
                                {
                                    var _detallesPersonalidad = new cPersonalidadDetalle().GetData(x => x.ID_INGRESO == SelectIngreso.ID_INGRESO && x.ID_IMPUTADO == SelectIngreso.ID_IMPUTADO && x.ID_CENTRO == SelectIngreso.ID_CENTRO && x.ID_ANIO == SelectIngreso.ID_ANIO && x.ID_ESTUDIO == _EstudioPorValidar.ID_ESTUDIO);

                                    var _detallePersonalidadDetallePsicologicoComun = _detallesPersonalidad != null ? _detallesPersonalidad.Any() ? _detallesPersonalidad.FirstOrDefault(x => x.ID_TIPO == 10) : null : null;
                                    System.DateTime _fechaH = Fechas.GetFechaDateServer;
                                    if (_detallePersonalidadDetallePsicologicoComun != null)
                                    {
                                        if (_detallePersonalidadDetallePsicologicoComun.PERSONALIDAD_DETALLE_DIAS != null && _detallePersonalidadDetallePsicologicoComun.PERSONALIDAD_DETALLE_DIAS.Any())
                                        {
                                            var _detalleDiasProgramados = _detallePersonalidadDetallePsicologicoComun.PERSONALIDAD_DETALLE_DIAS.Where(x => x.FECHA_INICIO.Value.Year == _fechaH.Year && x.FECHA_INICIO.Value.Month == _fechaH.Month && x.FECHA_INICIO.Value.Day == _fechaH.Day && x.PERSONALIDAD_DETALLE.ID_ESTATUS == (short)eEstatudDetallePersonalidad.ASIGNADO && x.ID_ESTUDIO == _detallePersonalidadDetallePsicologicoComun.ID_ESTUDIO);
                                            if (_detalleDiasProgramados != null && _detalleDiasProgramados.Any())
                                            {
                                                if (_detalleDiasProgramados.Any(x => x.FECHA_INICIO.Value.Year == _fechaH.Year && x.FECHA_INICIO.Value.Month == _fechaH.Month && x.FECHA_INICIO.Value.Day == _fechaH.Day))
                                                    if (_UltimoMovimiento.ESTATUS == 1 || _UltimoMovimiento.ESTATUS == 2)
                                                    {
                                                        if (_detalleDiasProgramados.Any(x => x.ID_AREA == _UltimoMovimiento.ID_AREA))
                                                        {
                                                            NombreUltimoTabElegido = "TabEstudioTrabajoSocialFF";
                                                            TabEstudioTrabajoSocialFF = true;
                                                        };
                                                    };
                                            };
                                        };
                                    };
                                }; //= validaciones
                            };

                            if (TabEstudioTrabajoSocialFF)
                            {

                                if (NombreUltimoTabElegido != "TabEstudioTrabajoSocialFF")
                                {
                                    EstaNavegandoTabs = true;
                                    GuardaAislado(NombreUltimoTabElegido);
                                    ValidaHuellasNavegacion();
                                };

                                NombreUltimoTabElegido = "TabEstudioTrabajoSocialFF";
                                TabEstudioMedicoFF = TabEstudioPsicoFF = TabActivProductCapacitFF = TabActEducCultDepRecCivFF = TabVigilanciaFF = TabEstudioCriminologico = false;
                                IndexFederal = 2;
                                TabEstudioTrabajoSocialFF = true;
                                IdVentanaAcual = (short)eVentanasProceso.ESTUDIO_TRABAJO_SOCIAL_FUERO_FEDERAL;
                                OnPropertyChanged("TabEstudioTrabajoSocialFF");
                                OnPropertyChanged("IdVentanaAcual");
                                IngresaNuevaUbicacion((short)eProcEstudiosNuevasUbicacionesP.ESTUDIO_DE_TRABAJO_SOCIAL_DE_FUERO_FEDERAL);
                            }

                        }

                        if (EnabledFederal4)
                        {
                            if (ValidaIndicesFederales() == decimal.Zero)
                            {
                                var _UltimoMovimiento = new cIngresoUbicacion().ObtenerUltimaUbicacion(SelectIngreso.ID_ANIO, SelectIngreso.ID_CENTRO, SelectIngreso.ID_IMPUTADO, SelectIngreso.ID_INGRESO);

                                var _EstudioPorValidar = new cEstudioPersonalidad().GetData(x => x.ID_INGRESO == SelectIngreso.ID_INGRESO && x.ID_IMPUTADO == SelectIngreso.ID_IMPUTADO && x.ID_CENTRO == SelectIngreso.ID_CENTRO && x.ID_ANIO == SelectIngreso.ID_ANIO).OrderByDescending(x => x.ID_ESTUDIO).FirstOrDefault();

                                if (_EstudioPorValidar != null)
                                {
                                    var _detallesPersonalidad = new cPersonalidadDetalle().GetData(x => x.ID_INGRESO == SelectIngreso.ID_INGRESO && x.ID_IMPUTADO == SelectIngreso.ID_IMPUTADO && x.ID_CENTRO == SelectIngreso.ID_CENTRO && x.ID_ANIO == SelectIngreso.ID_ANIO && x.ID_ESTUDIO == _EstudioPorValidar.ID_ESTUDIO);

                                    var _detallePersonalidadDetallePsicologicoComun = _detallesPersonalidad != null ? _detallesPersonalidad.Any() ? _detallesPersonalidad.FirstOrDefault(x => x.ID_TIPO == 16) : null : null;
                                    System.DateTime _fechaH = Fechas.GetFechaDateServer;
                                    if (_detallePersonalidadDetallePsicologicoComun != null)
                                    {
                                        if (_detallePersonalidadDetallePsicologicoComun.PERSONALIDAD_DETALLE_DIAS != null && _detallePersonalidadDetallePsicologicoComun.PERSONALIDAD_DETALLE_DIAS.Any())
                                        {
                                            var _detalleDiasProgramados = _detallePersonalidadDetallePsicologicoComun.PERSONALIDAD_DETALLE_DIAS.Where(x => x.FECHA_INICIO.Value.Year == _fechaH.Year && x.FECHA_INICIO.Value.Month == _fechaH.Month && x.FECHA_INICIO.Value.Day == _fechaH.Day && x.PERSONALIDAD_DETALLE.ID_ESTATUS == (short)eEstatudDetallePersonalidad.ASIGNADO && x.ID_ESTUDIO == _detallePersonalidadDetallePsicologicoComun.ID_ESTUDIO);
                                            if (_detalleDiasProgramados != null && _detalleDiasProgramados.Any())
                                            {
                                                if (_detalleDiasProgramados.Any(x => x.FECHA_INICIO.Value.Year == _fechaH.Year && x.FECHA_INICIO.Value.Month == _fechaH.Month && x.FECHA_INICIO.Value.Day == _fechaH.Day))
                                                    if (_UltimoMovimiento.ESTATUS == 1 || _UltimoMovimiento.ESTATUS == 2)
                                                    {
                                                        if (_detalleDiasProgramados.Any(x => x.ID_AREA == _UltimoMovimiento.ID_AREA))
                                                        {
                                                            NombreUltimoTabElegido = "TabActivProductCapacitFF";
                                                            TabActivProductCapacitFF = true;
                                                        };
                                                    };
                                            };
                                        };
                                    };
                                }; //= validaciones
                            };

                            if (TabActivProductCapacitFF)
                            {
                                if (NombreUltimoTabElegido != "TabActivProductCapacitFF")
                                {
                                    EstaNavegandoTabs = true;
                                    GuardaAislado(NombreUltimoTabElegido);
                                    ValidaHuellasNavegacion();
                                };

                                NombreUltimoTabElegido = "TabActivProductCapacitFF";
                                TabEstudioMedicoFF = TabEstudioPsicoFF = TabEstudioTrabajoSocialFF = TabActEducCultDepRecCivFF = TabVigilanciaFF = TabEstudioCriminologico = false;
                                IndexFederal = 3;
                                TabActivProductCapacitFF = true;
                                IdVentanaAcual = (short)eVentanasProceso.ACTIV_PRODUCTIVAS_CAPACITACION_FUERO_FEDERAL;
                                OnPropertyChanged("TabActivProductCapacitFF");
                                OnPropertyChanged("IdVentanaAcual");
                                IngresaNuevaUbicacion((short)eProcEstudiosNuevasUbicacionesP.ESTUDIO_LABORAL_DE_FUERO_FEDERAL);
                            }
                        }

                        if (EnabledFederal5)
                        {
                            if (ValidaIndicesFederales() == decimal.Zero)
                            {
                                var _UltimoMovimiento = new cIngresoUbicacion().ObtenerUltimaUbicacion(SelectIngreso.ID_ANIO, SelectIngreso.ID_CENTRO, SelectIngreso.ID_IMPUTADO, SelectIngreso.ID_INGRESO);

                                var _EstudioPorValidar = new cEstudioPersonalidad().GetData(x => x.ID_INGRESO == SelectIngreso.ID_INGRESO && x.ID_IMPUTADO == SelectIngreso.ID_IMPUTADO && x.ID_CENTRO == SelectIngreso.ID_CENTRO && x.ID_ANIO == SelectIngreso.ID_ANIO).OrderByDescending(x => x.ID_ESTUDIO).FirstOrDefault();

                                if (_EstudioPorValidar != null)
                                {
                                    var _detallesPersonalidad = new cPersonalidadDetalle().GetData(x => x.ID_INGRESO == SelectIngreso.ID_INGRESO && x.ID_IMPUTADO == SelectIngreso.ID_IMPUTADO && x.ID_CENTRO == SelectIngreso.ID_CENTRO && x.ID_ANIO == SelectIngreso.ID_ANIO && x.ID_ESTUDIO == _EstudioPorValidar.ID_ESTUDIO);

                                    var _detallePersonalidadDetallePsicologicoComun = _detallesPersonalidad != null ? _detallesPersonalidad.Any() ? _detallesPersonalidad.FirstOrDefault(x => x.ID_TIPO == 15) : null : null;
                                    System.DateTime _fechaH = Fechas.GetFechaDateServer;
                                    if (_detallePersonalidadDetallePsicologicoComun != null)
                                    {
                                        if (_detallePersonalidadDetallePsicologicoComun.PERSONALIDAD_DETALLE_DIAS != null && _detallePersonalidadDetallePsicologicoComun.PERSONALIDAD_DETALLE_DIAS.Any())
                                        {
                                            var _detalleDiasProgramados = _detallePersonalidadDetallePsicologicoComun.PERSONALIDAD_DETALLE_DIAS.Where(x => x.FECHA_INICIO.Value.Year == _fechaH.Year && x.FECHA_INICIO.Value.Month == _fechaH.Month && x.FECHA_INICIO.Value.Day == _fechaH.Day && x.PERSONALIDAD_DETALLE.ID_ESTATUS == (short)eEstatudDetallePersonalidad.ASIGNADO && x.ID_ESTUDIO == _detallePersonalidadDetallePsicologicoComun.ID_ESTUDIO);
                                            if (_detalleDiasProgramados != null && _detalleDiasProgramados.Any())
                                            {
                                                if (_detalleDiasProgramados.Any(x => x.FECHA_INICIO.Value.Year == _fechaH.Year && x.FECHA_INICIO.Value.Month == _fechaH.Month && x.FECHA_INICIO.Value.Day == _fechaH.Day))
                                                    if (_UltimoMovimiento.ESTATUS == 1 || _UltimoMovimiento.ESTATUS == 2)
                                                    {
                                                        if (_detalleDiasProgramados.Any(x => x.ID_AREA == _UltimoMovimiento.ID_AREA))
                                                        {
                                                            NombreUltimoTabElegido = "TabActEducCultDepRecCivFF";
                                                            TabActEducCultDepRecCivFF = true;
                                                        };
                                                    };
                                            };
                                        };
                                    };
                                }; //= validaciones
                            };

                            if (TabActEducCultDepRecCivFF)
                            {

                                if (NombreUltimoTabElegido != "TabActEducCultDepRecCivFF")
                                {
                                    EstaNavegandoTabs = true;
                                    GuardaAislado(NombreUltimoTabElegido);
                                    ValidaHuellasNavegacion();
                                };

                                NombreUltimoTabElegido = "TabActEducCultDepRecCivFF";
                                TabEstudioMedicoFF = TabEstudioPsicoFF = TabEstudioTrabajoSocialFF = TabActivProductCapacitFF = TabVigilanciaFF = TabEstudioCriminologico = false;
                                IndexFederal = 4;
                                TabActEducCultDepRecCivFF = true;
                                IdVentanaAcual = (short)eVentanasProceso.ESTUDIO_ACTIV_EDUCAT_CULT_DEP_RECR_CIV_FUERO_FEDERAL;
                                OnPropertyChanged("TabActEducCultDepRecCivFF");
                                OnPropertyChanged("IdVentanaAcual");
                                IngresaNuevaUbicacion((short)eProcEstudiosNuevasUbicacionesP.ESTUDIO_PEDAGOGICO_DE_FUERO_FEDERAL);
                            }
                        }

                        if (EnabledFederal6)
                        {
                            if (ValidaIndicesFederales() == decimal.Zero)
                            {
                                var _UltimoMovimiento = new cIngresoUbicacion().ObtenerUltimaUbicacion(SelectIngreso.ID_ANIO, SelectIngreso.ID_CENTRO, SelectIngreso.ID_IMPUTADO, SelectIngreso.ID_INGRESO);

                                var _EstudioPorValidar = new cEstudioPersonalidad().GetData(x => x.ID_INGRESO == SelectIngreso.ID_INGRESO && x.ID_IMPUTADO == SelectIngreso.ID_IMPUTADO && x.ID_CENTRO == SelectIngreso.ID_CENTRO && x.ID_ANIO == SelectIngreso.ID_ANIO).OrderByDescending(x => x.ID_ESTUDIO).FirstOrDefault();

                                if (_EstudioPorValidar != null)
                                {
                                    var _detallesPersonalidad = new cPersonalidadDetalle().GetData(x => x.ID_INGRESO == SelectIngreso.ID_INGRESO && x.ID_IMPUTADO == SelectIngreso.ID_IMPUTADO && x.ID_CENTRO == SelectIngreso.ID_CENTRO && x.ID_ANIO == SelectIngreso.ID_ANIO && x.ID_ESTUDIO == _EstudioPorValidar.ID_ESTUDIO);

                                    var _detallePersonalidadDetallePsicologicoComun = _detallesPersonalidad != null ? _detallesPersonalidad.Any() ? _detallesPersonalidad.FirstOrDefault(x => x.ID_TIPO == 11) : null : null;
                                    System.DateTime _fechaH = Fechas.GetFechaDateServer;
                                    if (_detallePersonalidadDetallePsicologicoComun != null)
                                    {
                                        if (_detallePersonalidadDetallePsicologicoComun.PERSONALIDAD_DETALLE_DIAS != null && _detallePersonalidadDetallePsicologicoComun.PERSONALIDAD_DETALLE_DIAS.Any())
                                        {
                                            var _detalleDiasProgramados = _detallePersonalidadDetallePsicologicoComun.PERSONALIDAD_DETALLE_DIAS.Where(x => x.FECHA_INICIO.Value.Year == _fechaH.Year && x.FECHA_INICIO.Value.Month == _fechaH.Month && x.FECHA_INICIO.Value.Day == _fechaH.Day && x.PERSONALIDAD_DETALLE.ID_ESTATUS == (short)eEstatudDetallePersonalidad.ASIGNADO && x.ID_ESTUDIO == _detallePersonalidadDetallePsicologicoComun.ID_ESTUDIO);
                                            if (_detalleDiasProgramados != null && _detalleDiasProgramados.Any())
                                            {
                                                if (_detalleDiasProgramados.Any(x => x.FECHA_INICIO.Value.Year == _fechaH.Year && x.FECHA_INICIO.Value.Month == _fechaH.Month && x.FECHA_INICIO.Value.Day == _fechaH.Day))
                                                    if (_UltimoMovimiento.ESTATUS == 1 || _UltimoMovimiento.ESTATUS == 2)
                                                    {
                                                        if (_detalleDiasProgramados.Any(x => x.ID_AREA == _UltimoMovimiento.ID_AREA))
                                                        {
                                                            NombreUltimoTabElegido = "TabVigilanciaFF";
                                                            TabVigilanciaFF = true;
                                                        };
                                                    };
                                            };
                                        };
                                    };
                                }; //= validaciones
                            };

                            if (TabVigilanciaFF)
                            {
                                if (NombreUltimoTabElegido != "TabVigilanciaFF")
                                {
                                    EstaNavegandoTabs = true;
                                    GuardaAislado(NombreUltimoTabElegido);
                                    ValidaHuellasNavegacion();
                                };

                                NombreUltimoTabElegido = "TabVigilanciaFF";
                                TabEstudioMedicoFF = TabEstudioPsicoFF = TabEstudioTrabajoSocialFF = TabActivProductCapacitFF = TabActEducCultDepRecCivFF = TabEstudioCriminologico = false;
                                IndexFederal = 5;
                                TabVigilanciaFF = true;
                                IdVentanaAcual = (short)eVentanasProceso.VIGILANCIA_FUERO_FEDERAL;
                                OnPropertyChanged("TabVigilanciaFF");
                                OnPropertyChanged("IdVentanaAcual");
                                IngresaNuevaUbicacion((short)eProcEstudiosNuevasUbicacionesP.ESTUDIO_DE_SEGURIDAD_DE_FUERO_FEDERAL);
                            }
                        }

                        if (EnabledFederal7)
                        {
                            if (ValidaIndicesFederales() == decimal.Zero)
                            {
                                var _UltimoMovimiento = new cIngresoUbicacion().ObtenerUltimaUbicacion(SelectIngreso.ID_ANIO, SelectIngreso.ID_CENTRO, SelectIngreso.ID_IMPUTADO, SelectIngreso.ID_INGRESO);

                                var _EstudioPorValidar = new cEstudioPersonalidad().GetData(x => x.ID_INGRESO == SelectIngreso.ID_INGRESO && x.ID_IMPUTADO == SelectIngreso.ID_IMPUTADO && x.ID_CENTRO == SelectIngreso.ID_CENTRO && x.ID_ANIO == SelectIngreso.ID_ANIO).OrderByDescending(x => x.ID_ESTUDIO).FirstOrDefault();

                                if (_EstudioPorValidar != null)
                                {
                                    var _detallesPersonalidad = new cPersonalidadDetalle().GetData(x => x.ID_INGRESO == SelectIngreso.ID_INGRESO && x.ID_IMPUTADO == SelectIngreso.ID_IMPUTADO && x.ID_CENTRO == SelectIngreso.ID_CENTRO && x.ID_ANIO == SelectIngreso.ID_ANIO && x.ID_ESTUDIO == _EstudioPorValidar.ID_ESTUDIO);

                                    var _detallePersonalidadDetallePsicologicoComun = _detallesPersonalidad != null ? _detallesPersonalidad.Any() ? _detallesPersonalidad.FirstOrDefault(x => x.ID_TIPO == 9) : null : null;
                                    System.DateTime _fechaH = Fechas.GetFechaDateServer;
                                    if (_detallePersonalidadDetallePsicologicoComun != null)
                                    {
                                        if (_detallePersonalidadDetallePsicologicoComun.PERSONALIDAD_DETALLE_DIAS != null && _detallePersonalidadDetallePsicologicoComun.PERSONALIDAD_DETALLE_DIAS.Any())
                                        {
                                            var _detalleDiasProgramados = _detallePersonalidadDetallePsicologicoComun.PERSONALIDAD_DETALLE_DIAS.Where(x => x.FECHA_INICIO.Value.Year == _fechaH.Year && x.FECHA_INICIO.Value.Month == _fechaH.Month && x.FECHA_INICIO.Value.Day == _fechaH.Day && x.PERSONALIDAD_DETALLE.ID_ESTATUS == (short)eEstatudDetallePersonalidad.ASIGNADO && x.ID_ESTUDIO == _detallePersonalidadDetallePsicologicoComun.ID_ESTUDIO);
                                            if (_detalleDiasProgramados != null && _detalleDiasProgramados.Any())
                                            {
                                                if (_detalleDiasProgramados.Any(x => x.FECHA_INICIO.Value.Year == _fechaH.Year && x.FECHA_INICIO.Value.Month == _fechaH.Month && x.FECHA_INICIO.Value.Day == _fechaH.Day))
                                                    if (_UltimoMovimiento.ESTATUS == 1 || _UltimoMovimiento.ESTATUS == 2)
                                                    {
                                                        if (_detalleDiasProgramados.Any(x => x.ID_AREA == _UltimoMovimiento.ID_AREA))
                                                        {
                                                            NombreUltimoTabElegido = "TabEstudioCriminologico";
                                                            TabEstudioCriminologico = true;
                                                        };
                                                    };
                                            };
                                        };
                                    };
                                }; //= validaciones
                            };


                            if (TabEstudioCriminologico)
                            {
                                if (NombreUltimoTabElegido != "TabEstudioCriminologico")
                                {
                                    EstaNavegandoTabs = true;
                                    GuardaAislado(NombreUltimoTabElegido);
                                    ValidaHuellasNavegacion();
                                };

                                NombreUltimoTabElegido = "TabEstudioCriminologico";
                                TabEstudioMedicoFF = TabEstudioPsicoFF = TabEstudioTrabajoSocialFF = TabActivProductCapacitFF = TabActEducCultDepRecCivFF = TabVigilanciaFF = false;
                                IndexFederal = 6;
                                TabEstudioCriminologico = true;
                                IdVentanaAcual = (short)eVentanasProceso.ESTUDIO_CRIMINOLOGICOO_FUERO_FEDERAL;
                                OnPropertyChanged("TabEstudioCriminologico");
                                OnPropertyChanged("IdVentanaAcual");
                                IngresaNuevaUbicacion((short)eProcEstudiosNuevasUbicacionesP.ESTUDIO_CRIMINOLOGICO_DE_FUERO_FEDERAL);
                            }
                        }

                        OnPropertyChanged("IndexFederal");
                    }
                }
            }
            catch (System.Exception exc)
            {
                throw exc;
            }
        }
    }
}