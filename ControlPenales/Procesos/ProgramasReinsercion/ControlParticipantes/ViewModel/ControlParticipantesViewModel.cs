using ControlPenales.BiometricoServiceReference;
using SSP.Controlador.Catalogo.Justicia;
using SSP.Servidor;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace ControlPenales
{
    partial class ControlParticipantesViewModel : ValidationViewModelBase
    {
        /* [descripcion de clase]
         * 
         * 
         */

        private void clickSwitch(Object obj)
        {
            try
            {
                switch (obj.ToString())
                {
                    case "limpiar_Busqueda":
                        ((System.Windows.Controls.ContentControl)PopUpsViewModels.MainWindow.FindName("contentControl")).Content = null;
                        ((System.Windows.Controls.ContentControl)PopUpsViewModels.MainWindow.FindName("contentControl")).DataContext = null;
                        ((System.Windows.Controls.ContentControl)PopUpsViewModels.MainWindow.FindName("contentControl")).Content = new ControlParticipantesView();
                        ((System.Windows.Controls.ContentControl)PopUpsViewModels.MainWindow.FindName("contentControl")).DataContext = new ControlParticipantesViewModel();
                        break;
                    case "nueva_busqueda":
                        AnioBuscar = FolioBuscar = null;
                        NombreBuscar = ApellidoPaternoBuscar = ApellidoMaternoBuscar = null;
                        ImagenIngreso = ImagenImputado = new Imagenes().getImagenPerson();
                        ListExpediente = new RangeEnabledObservableCollection<IMPUTADO>();
                        break;
                    case "buscar_imputado":
                        if (ApellidoPaternoBuscar == null)
                            ApellidoPaternoBuscar = string.Empty;
                        if (ApellidoMaternoBuscar == null)
                            ApellidoMaternoBuscar = string.Empty;
                        if (NombreBuscar == null)
                            NombreBuscar = string.Empty;
                        BuscarImputado();
                        break;
                    case "buscar_salir":
                        var ingA = SelectIngresoAuxiliar;
                        SelectIngreso = ingA;
                        var expA = SelectExpedienteAuxiliar;
                        SelectExpediente = expA;
                        PopUpsViewModels.ClosePopUp(PopUpsViewModels.TipoPopUp.BUSQUEDA);
                        break;
                    case "buscar_seleccionar":
                        try
                        {
                            if (SelectExpediente == null)
                            {
                                (new Dialogos()).ConfirmacionDialogo("Error", "Debes seleccionar un imputado.");
                                break;
                            }
                            if (SelectIngreso == null)
                            {
                                (new Dialogos()).ConfirmacionDialogo("Error", "Debes seleccionar un ingreso.");
                                break;
                            }
                            var EstatusInactivos = Parametro.ESTATUS_ADMINISTRATIVO_INACT;
                            foreach (var item in EstatusInactivos)
                            {
                                if (SelectIngreso.ID_ESTATUS_ADMINISTRATIVO == item)
                                {
                                    new Dialogos().ConfirmacionDialogo("Notificación!", "El ingreso seleccionado no esta activo.");
                                    return;
                                }
                            }
                            if (SelectIngreso.ID_UB_CENTRO != GlobalVar.gCentro)
                            {
                                new Dialogos().ConfirmacionDialogo("Validación", "El ingreso seleccionado no pertenece a su centro.");
                                return;
                            }

                            //SE ELIMINA CANDADO DE TRASLADO DEBIDO A QUE LOS TECNICOS NO DEBEN DE ESTAR BLOQUEADOS DE PODER CALIFICAR A UN IMPUTADO DEBIDO A UN TRASLADO PROXIMO.
                            //var toleranciaTraslado = Parametro.TOLERANCIA_TRASLADO_EDIFICIO;
                            //if (SelectIngreso.TRASLADO_DETALLE.Any(a => (a.ID_ESTATUS != "CA" ? a.TRASLADO.ORIGEN_TIPO != "F" : false) && a.TRASLADO.TRASLADO_FEC.AddHours(-toleranciaTraslado).TimeOfDay <= Fechas.GetFechaDateServer.TimeOfDay))
                            //{
                            //    new Dialogos().ConfirmacionDialogo("Notificación!", "El interno [" + SelectIngreso.ID_ANIO.ToString() + "/" +
                            //        SelectIngreso.ID_IMPUTADO.ToString() + "] tiene un traslado proximo y no puede recibir visitas.");
                            //    return;
                            //}
                            var expedient = SelectExpediente;
                            var ingres = SelectIngreso;
                            SelectIngresoAuxiliar = null;
                            SelectExpedienteAuxiliar = null;
                            SelectExpediente = expedient;
                            SelectIngreso = ingres;
                            CargarInformacionParticipante();
                            PopUpsViewModels.ClosePopUp(PopUpsViewModels.TipoPopUp.BUSQUEDA);
                        }
                        catch (Exception ex)
                        {
                            StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al cargar datos del imputado.", ex);
                        }
                        break;
                }
            }
            catch (Exception ex)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error en el flujo del modulo.", ex);
            }
        }

        #region [PERMISOS]
        //private void ConfiguraPermisos()
        //{
        //    try
        //    {
        //        var permisos = new cProcesoUsuario().Obtener(enumProcesos.CONTROL_PARTICIPACION.ToString(), StaticSourcesViewModel.UsuarioLogin.Username);
        //        foreach (var p in permisos)
        //        {
        //            if (p.INSERTAR == 1)
        //            {
        //                AgregarMenuEnabled = true;
        //            }
        //            if (p.CONSULTAR == 1)
        //            {
        //                GroupBoxGeneralesEnabled = true;
        //                EjeEnabled = true;
        //                GrupoEnabled = true;
        //            }
        //            if (p.EDITAR == 1)
        //                EditarMenuEnabled = true;
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al configurar permisos en la pantalla", ex);
        //    }
        //}
        #endregion

        /// <summary>
        /// metodo que carga informacion al seleccionar modulo
        /// </summary>
        /// <param name="obj">usercontrol del modulo</param>
        private void ControlParticipantesLoad(ControlParticipantesView obj)
        {
            try
            {
                //ConfiguraPermisos();
            }
            catch (Exception ex)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al cargar modulo de control participantes", ex);
            }
        }

        private string CalcularSentencia(INGRESO ingres)
        {
            try
            {
                if (ingres != null)
                {
                    int anios = 0, meses = 0, dias = 0, anios_abono = 0, meses_abono = 0, dias_abono = 0;
                    DateTime? FechaInicioCompurgacion = null, FechaFinCompurgacion = null;
                    if (ingres.CAUSA_PENAL != null)
                    {
                        foreach (var cp in ingres.CAUSA_PENAL)
                        {
                            var segundaInstancia = false;
                            if (cp.SENTENCIA != null)
                            {
                                if (cp.SENTENCIA.Count > 0)
                                {
                                    //BUSCAMOS SI TIENE 2DA INSTANCIA
                                    if (cp.RECURSO.Count > 0)
                                    {
                                        var r = cp.RECURSO.Where(w => w.SENTENCIA_ANIOS > 0 || w.SENTENCIA_MESES > 0 || w.SENTENCIA_DIAS > 0);
                                        if (r != null)
                                        {
                                            var res = r.FirstOrDefault();
                                            if (res != null)
                                            {
                                                //SENTENCIA
                                                anios = anios + (res.SENTENCIA_ANIOS != null ? res.SENTENCIA_ANIOS.Value : 0);
                                                meses = meses + (res.SENTENCIA_MESES != null ? res.SENTENCIA_MESES.Value : 0);
                                                dias = dias + (res.SENTENCIA_DIAS != null ? res.SENTENCIA_DIAS.Value : 0);

                                                segundaInstancia = true;
                                            }
                                        }
                                    }
                                    var s = cp.SENTENCIA.FirstOrDefault();
                                    if (s != null)
                                    {
                                        if (FechaInicioCompurgacion == null)
                                        {
                                            FechaInicioCompurgacion = s.FEC_INICIO_COMPURGACION;
                                        }
                                        else
                                        {
                                            if (FechaInicioCompurgacion > s.FEC_INICIO_COMPURGACION)
                                                FechaInicioCompurgacion = s.FEC_INICIO_COMPURGACION;
                                        }

                                        //SENTENCIA
                                        if (!segundaInstancia)
                                        {
                                            anios = anios + (s.ANIOS != null ? s.ANIOS.Value : 0);
                                            meses = meses + (s.MESES != null ? s.MESES.Value : 0);
                                            dias = dias + (s.DIAS != null ? s.DIAS.Value : 0);
                                        }

                                        //ABONO
                                        anios_abono = anios_abono + (s.ANIOS_ABONADOS != null ? s.ANIOS_ABONADOS.Value : 0);
                                        meses_abono = meses_abono + (s.MESES_ABONADOS != null ? s.MESES_ABONADOS.Value : 0);
                                        dias_abono = dias_abono + (s.DIAS_ABONADOS != null ? s.DIAS_ABONADOS.Value : 0);
                                    }
                                }
                            }
                        }
                    }

                    while (dias > 29)
                    {
                        meses++;
                        dias = dias - 30;
                    }
                    while (meses > 11)
                    {
                        anios++;
                        meses = meses - 12;
                    }

                    if (FechaInicioCompurgacion != null)
                    {
                        FechaFinCompurgacion = FechaInicioCompurgacion;
                        FechaFinCompurgacion = FechaFinCompurgacion.Value.AddYears(anios);
                        FechaFinCompurgacion = FechaFinCompurgacion.Value.AddMonths(meses);
                        FechaFinCompurgacion = FechaFinCompurgacion.Value.AddDays(dias);
                        //
                        FechaFinCompurgacion = FechaFinCompurgacion.Value.AddYears(-anios_abono);
                        FechaFinCompurgacion = FechaFinCompurgacion.Value.AddMonths(-meses_abono);
                        FechaFinCompurgacion = FechaFinCompurgacion.Value.AddDays(-dias_abono);

                        int a = 0, m = 0, d = 0;
                        new Fechas().DiferenciaFechas(Fechas.GetFechaDateServer.Date, FechaInicioCompurgacion.Value.Date, out a, out  m, out d);
                        a = m = d = 0;
                        new Fechas().DiferenciaFechas(FechaFinCompurgacion.Value.Date, Fechas.GetFechaDateServer.Date, out a, out  m, out d);

                        TextSentencia = anios + (anios == 1 ? " Año " : " Años ") + meses + (meses == 1 ? " Mes " : " Meses ") + dias + (dias == 1 ? " Dia" : " Dias");
                        return a + (a == 1 ? " Año " : " Años ") + m + (m == 1 ? " Mes " : " Meses ") + d + (d == 1 ? " Dia" : " Dias");
                    }
                }
            }
            catch (Exception ex)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al calcular sentencia", ex);
            }
            return string.Empty;
        }

        private void BuscarInterno(Object obj)
        {
            try
            {
                if (obj is TextBox)
                    if (((TextBox)obj).Name == "FolioInterno")
                    {
                        TextNombreImputado = NombreBuscar = string.Empty;
                        TextPaternoImputado = ApellidoPaternoBuscar = string.Empty;
                        TextMaternoImputado = ApellidoMaternoBuscar = string.Empty;
                    }

                var ing = SelectIngreso;
                SelectIngresoAuxiliar = ing;
                var exp = SelectExpediente;
                SelectExpedienteAuxiliar = exp;
                AnioBuscar = !string.IsNullOrEmpty(TextAnio) ? int.Parse(TextAnio) : new Nullable<int>();
                FolioBuscar = !string.IsNullOrEmpty(TextFolio) ? int.Parse(TextFolio) : new Nullable<int>();
                NombreBuscar = !string.IsNullOrEmpty(TextNombreImputado) ? TextNombreImputado : string.Empty;
                ApellidoPaternoBuscar = !string.IsNullOrEmpty(TextPaternoImputado) ? TextPaternoImputado : string.Empty;
                ApellidoMaternoBuscar = !string.IsNullOrEmpty(TextMaternoImputado) ? TextMaternoImputado : string.Empty;
                SelectExpediente = null;
                BuscarImputado();
            }
            catch (Exception ex)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al traer datos del visitante.", ex);
            }
        }

        private void BuscarInternoPopup(Object obj)
        {
            try
            {
                var ing = SelectIngreso;
                SelectIngresoAuxiliar = ing;
                var exp = SelectExpediente;
                SelectExpedienteAuxiliar = exp;
                BuscarImputado();
            }
            catch (Exception ex)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al traer datos del participante.", ex);
            }
        }

        private async void BuscarImputado()
        {
            try
            {
                ImagenIngreso = new Imagenes().getImagenPerson();
                ImagenImputado = new Imagenes().getImagenPerson();
                ListExpediente = new RangeEnabledObservableCollection<IMPUTADO>();
                ListExpediente.InsertRange(await SegmentarResultadoBusqueda());
                if (ListExpediente.Count == 1)
                {
                    if (AnioBuscar != null && FolioBuscar != null)
                    {
                        var EstatusInactivos = Parametro.ESTATUS_ADMINISTRATIVO_INACT;
                        foreach (var item in EstatusInactivos)
                        {
                            if (ListExpediente[0].INGRESO.OrderByDescending(o => o.ID_INGRESO).FirstOrDefault().ID_ESTATUS_ADMINISTRATIVO == item)
                            {
                                new Dialogos().ConfirmacionDialogo("Notificación!", "El ingreso seleccionado no esta activo.");
                                clickSwitch("limpiar_Busqueda");
                                return;
                            }
                        }
                        if (ListExpediente[0].INGRESO.OrderByDescending(o => o.ID_INGRESO).FirstOrDefault().ID_UB_CENTRO != GlobalVar.gCentro)
                        {
                            new Dialogos().ConfirmacionDialogo("Validación", "El ingreso seleccionado no pertenece a su centro.");
                            clickSwitch("limpiar_Busqueda");
                            return;
                        }
                        //SE ELIMINA CANDADO DE TRASLADO DEBIDO A QUE LOS TECNICOS NO DEBEN DE ESTAR BLOQUEADOS DE PODER CALIFICAR A UN IMPUTADO DEBIDO A UN TRASLADO PROXIMO.
                        //var toleranciaTraslado = Parametro.TOLERANCIA_TRASLADO_EDIFICIO;
                        //if (ListExpediente[0].INGRESO.OrderByDescending(o => o.ID_INGRESO).FirstOrDefault().TRASLADO_DETALLE.Any(a => (a.ID_ESTATUS != "CA" ? a.TRASLADO.ORIGEN_TIPO != "F" : false) && a.TRASLADO.TRASLADO_FEC.AddHours(-toleranciaTraslado).TimeOfDay <= Fechas.GetFechaDateServer.TimeOfDay))
                        //{
                        //    new Dialogos().ConfirmacionDialogo("Notificación!", "El interno [" + ListExpediente[0].INGRESO.OrderByDescending(o => o.ID_INGRESO).FirstOrDefault().ID_ANIO.ToString() + "/" +
                        //        ListExpediente[0].INGRESO.OrderByDescending(o => o.ID_INGRESO).FirstOrDefault().ID_IMPUTADO.ToString() + "] tiene un traslado proximo y no puede recibir visitas.");
                        //    clickSwitch("limpiar_Busqueda");
                        //    return;
                        //}
                        SelectExpediente = ListExpediente[0];
                        SelectIngreso = ListExpediente[0].INGRESO.Where(w => w.ID_ESTATUS_ADMINISTRATIVO != Parametro.ID_ESTATUS_ADMVO_LIBERADO).FirstOrDefault();
                        var expedient = SelectExpediente;
                        var ingres = SelectIngreso;
                        SelectExpediente = expedient;
                        SelectIngreso = ingres;
                        CargarInformacionParticipante();
                        PopUpsViewModels.ClosePopUp(PopUpsViewModels.TipoPopUp.BUSQUEDA);
                        return;
                    }
                }
                if (ListExpediente.Count == 0)
                {
                    if (!string.IsNullOrEmpty(NombreD) || !string.IsNullOrEmpty(PaternoD) || !string.IsNullOrEmpty(MaternoD))
                    {
                        new Dialogos().ConfirmacionDialogo("Notificacion!", "No se encontro ningun imputado con esos datos.");
                        clickSwitch("limpiar_Busqueda");
                    }
                }

                EmptyExpedienteVisible = ListExpediente.Count <= 0;
                PopUpsViewModels.ShowPopUp(this, PopUpsViewModels.TipoPopUp.BUSQUEDA);
            }
            catch (Exception ex)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al cargar la busqueda.", ex);
                clickSwitch("limpiar_Busqueda");
            }
        }

        private async Task<List<IMPUTADO>> SegmentarResultadoBusqueda(int _Pag = 1)
        {
            try
            {
                if (string.IsNullOrEmpty(ApellidoPaternoBuscar) && string.IsNullOrEmpty(ApellidoMaternoBuscar) && string.IsNullOrEmpty(NombreBuscar) && !AnioBuscar.HasValue && !FolioBuscar.HasValue)
                    return new List<IMPUTADO>();
                Pagina = _Pag;
                var result = await StaticSourcesViewModel.CargarDatosAsync<ObservableCollection<IMPUTADO>>(() =>
                    new cImputado().ObtenerTodos(ApellidoPaternoBuscar, ApellidoMaternoBuscar, NombreBuscar, AnioBuscar, FolioBuscar, _Pag));
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
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al segmentar busqueda.", ex);
                return new List<IMPUTADO>();
            }
        }

        void CargarInformacionParticipante()
        {
            try
            {
                TextAnio = SelectExpediente.ID_ANIO.ToString();
                TextFolio = SelectExpediente.ID_IMPUTADO.ToString();
                TextPaternoImputado = SelectExpediente.PATERNO.Trim();
                TextMaternoImputado = SelectExpediente.MATERNO.Trim();
                TextNombreImputado = SelectExpediente.NOMBRE.Trim();
                TextUbicacion = SelectIngreso.CAMA.CELDA.SECTOR.EDIFICIO.DESCR.Trim() + "-" + SelectIngreso.CAMA.CELDA.SECTOR.DESCR.Trim() + SelectIngreso.CAMA.CELDA.ID_CELDA.Trim() + "-" + (string.IsNullOrEmpty(SelectIngreso.CAMA.DESCR) ? SelectIngreso.CAMA.ID_CAMA.ToString().Trim() : SelectIngreso.CAMA.ID_CAMA + " " + SelectIngreso.CAMA.DESCR.Trim());
                TextPlanimetria = SelectIngreso.CAMA != null ? SelectIngreso.CAMA.SECTOR_OBSERVACION_CELDA != null ? SelectIngreso.CAMA.SECTOR_OBSERVACION_CELDA.SECTOR_OBSERVACION != null ? SelectIngreso.CAMA.SECTOR_OBSERVACION_CELDA.SECTOR_OBSERVACION.SECTOR_CLASIFICACION != null ? SelectIngreso.CAMA.SECTOR_OBSERVACION_CELDA.SECTOR_OBSERVACION.SECTOR_CLASIFICACION.POBLACION : string.Empty : string.Empty : string.Empty : string.Empty;
                Planimetriacolor = SelectIngreso.CAMA != null ? SelectIngreso.CAMA.SECTOR_OBSERVACION_CELDA != null ? SelectIngreso.CAMA.SECTOR_OBSERVACION_CELDA.SECTOR_OBSERVACION != null ? SelectIngreso.CAMA.SECTOR_OBSERVACION_CELDA.SECTOR_OBSERVACION.SECTOR_CLASIFICACION != null ? SelectIngreso.CAMA.SECTOR_OBSERVACION_CELDA.SECTOR_OBSERVACION.SECTOR_CLASIFICACION.COLOR : string.Empty : string.Empty : string.Empty : string.Empty;
                TextSentenciaRes = CalcularSentencia(SelectIngreso);
                TextEstatus = SelectIngreso.GRUPO_PARTICIPANTE.Any() ? "CON TRATAMIENTO" : "SIN TRATAMIENTO";

                ObtenerCursosAprovadosTotalHoras(SelectIngreso.GRUPO_PARTICIPANTE, SelectIngreso.GRUPO_PARTICIPANTE.Count);

                SelectedEje = null;
                var index = 0;
                ListaEjes = new ObservableCollection<EJE>(SelectIngreso.GRUPO_PARTICIPANTE.Select(s => s.EJE1).Distinct().OrderBy(o => o.COMPLEMENTARIO == "S").ThenBy(o => o.ORDEN));
                if (ListaEjes.Count > 0)
                {
                    ListaEjes.Insert(0, new EJE() { COMPLEMENTARIO = "MODELO" });
                    index = ListaEjes.IndexOf(ListaEjes.Where(w => w.COMPLEMENTARIO != "MODELO").OrderBy(o => o.COMPLEMENTARIO == "S").ThenBy(t => t.ORDEN).Where(w => w.COMPLEMENTARIO == "S").FirstOrDefault());
                    if (index > 0)
                        ListaEjes.Insert(index, new EJE() { COMPLEMENTARIO = "COMPLEMENTARIO" });
                }
                SelectedGrupo = null;
                ListaGrupo = new ObservableCollection<GRUPO>(SelectIngreso.GRUPO_PARTICIPANTE.Where(w => w.GRUPO != null).Select(s => s.GRUPO).Distinct().OrderBy(o => o.ACTIVIDAD.ACTIVIDAD_EJE.Select(se => se.EJE.COMPLEMENTARIO).FirstOrDefault() == "S").ThenBy(o => o.DESCR));
                if (ListaGrupo.Count > 0)
                {
                    ListaGrupo.Insert(0, new GRUPO() { RECURRENCIA = "MODELO" });
                    index = ListaGrupo.IndexOf(ListaGrupo.Where(w => w.RECURRENCIA != "MODELO").OrderBy(o => o.ACTIVIDAD.ACTIVIDAD_EJE.Where(w => w.ID_ACTIVIDAD == o.ID_ACTIVIDAD && w.ID_TIPO_PROGRAMA == o.ID_TIPO_PROGRAMA).Select(s => s.EJE).FirstOrDefault().COMPLEMENTARIO == "S").ThenBy(t => t.ACTIVIDAD.ACTIVIDAD_EJE.Where(w => w.ID_ACTIVIDAD == t.ID_ACTIVIDAD && w.ID_TIPO_PROGRAMA == t.ID_TIPO_PROGRAMA).Select(s => s.EJE).FirstOrDefault().ORDEN).Where(w => w.ACTIVIDAD.ACTIVIDAD_EJE.Where(wh => wh.ID_ACTIVIDAD == w.ID_ACTIVIDAD && wh.ID_TIPO_PROGRAMA == w.ID_TIPO_PROGRAMA).Select(s => s.EJE).FirstOrDefault().COMPLEMENTARIO == "S").FirstOrDefault());
                    if (index > 0)
                        ListaGrupo.Insert(index, new GRUPO() { RECURRENCIA = "COMPLEMENTARIO" });
                }

                var IdGH = new Nullable<DateTime>();
                var listaemp = new List<EmpalmeParticipante>();
                var cont = -1;
                var horariolist = new cGrupoAsistencia().GetData().Where(w => w.GRUPO_PARTICIPANTE.ING_ID_CENTRO == SelectIngreso.ID_UB_CENTRO && w.GRUPO_PARTICIPANTE.ING_ID_ANIO == SelectIngreso.ID_ANIO && w.GRUPO_PARTICIPANTE.ING_ID_IMPUTADO == SelectIngreso.ID_IMPUTADO && w.GRUPO_PARTICIPANTE.ING_ID_INGRESO == SelectIngreso.ID_INGRESO).OrderByDescending(o => o.GRUPO_HORARIO.HORA_INICIO).ToList();
                
                if (horariolist.Count > 1)
                    foreach (var item in horariolist)
                    {
                        if (item == null)
                            continue;
                        if (IdGH == item.GRUPO_HORARIO.HORA_INICIO.Value.Date)
                        {
                            listaemp[cont].ListHorario.Add(new ListaEmpalmes()
                            {
                                ACTIVIDAD = item.GRUPO_HORARIO.GRUPO.ACTIVIDAD.DESCR,
                                EJE = item.GRUPO_PARTICIPANTE.EJE1.DESCR,
                                ELEGIDA = item.EMP_APROBADO == 1,
                                GRUPO = item.GRUPO_HORARIO.GRUPO.DESCR,
                                HORARIO = item.GRUPO_HORARIO.HORA_INICIO.Value.ToShortTimeString() + " - " + item.GRUPO_HORARIO.HORA_TERMINO.Value.ToShortTimeString(),
                                PROGRAMA = item.GRUPO_HORARIO.GRUPO.ACTIVIDAD.TIPO_PROGRAMA.NOMBRE
                            });
                            listaemp[cont].ListHorario = listaemp[cont].ListHorario.OrderByDescending(o => o.ELEGIDA).ThenBy(t => t.HORARIO).ToList();
                            continue;
                        }
                        IdGH = item.GRUPO_HORARIO.HORA_INICIO.Value.Date;

                        listaemp.Add(new EmpalmeParticipante()
                        {
                            HEADEREXPANDER = item.GRUPO_HORARIO.HORA_INICIO.Value.Date.ToShortDateString(),
                            ListHorario = new List<ListaEmpalmes>() { new ListaEmpalmes()
                        {
                            ACTIVIDAD = item.GRUPO_HORARIO.GRUPO.ACTIVIDAD.DESCR,
                            EJE = item.GRUPO_PARTICIPANTE.EJE1.DESCR,
                            ELEGIDA = item.EMP_APROBADO == 1,
                            GRUPO = item.GRUPO_HORARIO.GRUPO.DESCR,
                            HORARIO = item.GRUPO_HORARIO.HORA_INICIO.Value.ToShortTimeString() + " - " + item.GRUPO_HORARIO.HORA_TERMINO.Value.ToShortTimeString(),
                            PROGRAMA = item.GRUPO_HORARIO.GRUPO.ACTIVIDAD.TIPO_PROGRAMA.NOMBRE
                        }}
                        });
                        cont++;
                    }
                ListEmpalme = new ObservableCollection<EmpalmeParticipante>(listaemp.Where(w => w.ListHorario.Count > 1).OrderByDescending(o => o.HEADEREXPANDER));

                #region Listas Solicitud de Citas
                if (LstAreaTecnica == null)
                {
                    LstAreaTecnica = new ObservableCollection<AREA_TECNICA>(new cAreaTecnica().ObtenerTodo());
                    LstAreaTecnica.Insert(0, new AREA_TECNICA() { ID_TECNICA = -1, DESCR = "SELECCIONE" });
                }
                if (LstAreaTraslado == null)
                {
                    LstAreaTraslado = new ObservableCollection<AREA>(new cArea().ObtenerTodos());
                    LstAreaTraslado.Insert(0, new AREA() { ID_AREA = -1, DESCR = "SELECCIONE" });
                }
                #endregion

                StaticSourcesViewModel.SourceChanged = false;
            }
            catch (Exception ex)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al cargar la información del participante.", ex);
            }
        }

        private void ObtenerCursosAprovadosTotalHoras(IEnumerable<GRUPO_PARTICIPANTE> enumerable, int TotalActividades)
        {
            var acreditados = 0;
            try
            {
                var HorasAsistencia = 0;
                var TotalAsistencia = 0;
                foreach (var item in enumerable)
                {
                    acreditados = acreditados + item.NOTA_TECNICA.Where(w => w.ESTATUS == 1).Count();
                    TotalAsistencia = TotalAsistencia + (item.ID_GRUPO.HasValue ? item.GRUPO.GRUPO_HORARIO.Where(w => w.ID_GRUPO == item.ID_GRUPO && w.ID_GRUPO == item.ID_GRUPO && w.ESTATUS == 1).Count() : 0);
                    HorasAsistencia = HorasAsistencia + item.GRUPO_ASISTENCIA.Where(w => w.GRUPO_HORARIO.ESTATUS == 1 && (w.ESTATUS == 1 || w.ESTATUS == 3) && enumerable.Where(wh => wh.GRUPO != null && wh.GRUPO.GRUPO_HORARIO.Where(whe => whe.ESTATUS == 1).Any()).Contains(w.GRUPO_PARTICIPANTE) && w.ASISTENCIA == 1).Count();
                }
                MaxValueProBar = TotalActividades == 0 ? 1 : TotalActividades;
                CantidadActividadesAprovadas = acreditados;

                HorasTratamiento = HorasAsistencia + "/" + TotalAsistencia;
                AvanceTratamiento = acreditados + "/" + TotalActividades;
            }
            catch (Exception ex)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al obtener avances del participante.", ex);
            }
        }

        private void CargarNotasTecnicas(ICollection<GRUPO_PARTICIPANTE> collection, EJE selected)
        {
            try
            {
                var preNotaTecnica = new List<Nota_Tecnica>();
                foreach (var item in collection.Where(w => w.EJE == selected.ID_EJE).OrderBy(o => o.GRUPO == null).ThenBy(t => t.ACTIVIDAD.TIPO_PROGRAMA.ORDEN).ThenBy(t => t.ACTIVIDAD.ORDEN).ThenBy(t => t.GRUPO != null ? t.GRUPO.DESCR : string.Empty))
                    preNotaTecnica.Add(new Nota_Tecnica()
                    {
                        EJE = item.EJE1.DESCR,
                        PROGRAMA = item.ACTIVIDAD.TIPO_PROGRAMA.NOMBRE,
                        ACTIVIDAD = item.ACTIVIDAD.DESCR,
                        GRUPO = item.GRUPO != null ? item.GRUPO.DESCR : item.GRUPO_PARTICIPANTE_ESTATUS.DESCR,
                        EntityGRUPO = item.GRUPO,
                        INICIO = item.GRUPO != null ? item.GRUPO.GRUPO_HORARIO.Any() ? item.GRUPO.GRUPO_HORARIO.OrderBy(o => o.HORA_INICIO).FirstOrDefault().HORA_INICIO.Value.ToShortDateString() : string.Empty : string.Empty,
                        FIN = item.GRUPO != null ? item.GRUPO.GRUPO_HORARIO.Any() ? item.GRUPO.GRUPO_HORARIO.OrderByDescending(o => o.HORA_TERMINO).FirstOrDefault().HORA_TERMINO.Value.ToShortDateString() : string.Empty : string.Empty,
                        ASISTENCIA = ObtenerPorcentajeAsistencia(item, collection),
                        NOTA = item.NOTA_TECNICA.Count != 0 ? item.NOTA_TECNICA.Where(w => w.ID_CENTRO == item.ID_CENTRO && w.ID_TIPO_PROGRAMA == item.ID_TIPO_PROGRAMA && w.ID_ACTIVIDAD == item.ID_ACTIVIDAD && w.ID_GRUPO == item.ID_GRUPO).FirstOrDefault() != null ? item.NOTA_TECNICA.Where(w => w.ID_CENTRO == item.ID_CENTRO && w.ID_TIPO_PROGRAMA == item.ID_TIPO_PROGRAMA && w.ID_ACTIVIDAD == item.ID_ACTIVIDAD && w.ID_GRUPO == item.ID_GRUPO).FirstOrDefault().NOTA : "NO CAPTURADA" : "NO CAPTURADA",
                        ACREDITADO = item.NOTA_TECNICA.Count != 0 ? item.NOTA_TECNICA.Where(w => w.ID_CENTRO == item.ID_CENTRO && w.ID_TIPO_PROGRAMA == item.ID_TIPO_PROGRAMA && w.ID_ACTIVIDAD == item.ID_ACTIVIDAD && w.ID_GRUPO == item.ID_GRUPO).FirstOrDefault() != null ? item.NOTA_TECNICA.Where(w => w.ID_CENTRO == item.ID_CENTRO && w.ID_TIPO_PROGRAMA == item.ID_TIPO_PROGRAMA && w.ID_ACTIVIDAD == item.ID_ACTIVIDAD && w.ID_GRUPO == item.ID_GRUPO).FirstOrDefault().NOTA_TECNICA_ESTATUS.DESCR : "NO CAPTURADA" : "NO CAPTURADA"
                    });

                ListNotasTecnicas = new ObservableCollection<Nota_Tecnica>(preNotaTecnica);
            }
            catch (Exception ex)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al cargar notas tecnicas.", ex);
            }
        }

        private string ObtenerPorcentajeAsistencia(GRUPO_PARTICIPANTE item, ICollection<GRUPO_PARTICIPANTE> collection)
        {
            try
            {
                var TotalHoras = 0.0;
                var AsistenciaHoras = 0.0;
                TotalHoras = item.ID_GRUPO.HasValue ? item.GRUPO.GRUPO_HORARIO.Where(w => w.ID_GRUPO == item.ID_GRUPO && w.ESTATUS == 1).Count() : 0;
                AsistenciaHoras = item.GRUPO_ASISTENCIA.Where(w => w.GRUPO_HORARIO.ESTATUS == 1 && (w.ESTATUS == 1 || w.ESTATUS == 3) && collection.Where(wh => wh.GRUPO != null && wh.GRUPO.GRUPO_HORARIO.Where(whe => whe.ESTATUS == 1).Any()).Contains(w.GRUPO_PARTICIPANTE) && w.ASISTENCIA == 1).Count();

                if (double.IsNaN((AsistenciaHoras / TotalHoras)))
                    return string.Empty;

                return string.Format("{0:P2}", (AsistenciaHoras / TotalHoras));
            }
            catch (Exception ex)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al cargar la información del participante.", ex);
                return string.Empty;
            }
        }

        private void CargarHorarioParticipante(ICollection<GRUPO_PARTICIPANTE> collection, GRUPO selected)
        {
            try
            {
                var preHorarioParticipante = new List<HorarioParticipante>();
                foreach (var item in collection.Where(w => w.ID_GRUPO == selected.ID_GRUPO).OrderBy(t => t.ACTIVIDAD.TIPO_PROGRAMA.ORDEN).ThenBy(t => t.ACTIVIDAD.ORDEN).ThenBy(t => t.GRUPO != null ? t.GRUPO.DESCR : string.Empty))
                    foreach (var subitem in item.GRUPO.GRUPO_HORARIO.OrderBy(o => o.HORA_INICIO))
                        preHorarioParticipante.Add(new HorarioParticipante()
                        {
                            EJE = item.EJE1.DESCR,
                            PROGRAMA = item.ACTIVIDAD.TIPO_PROGRAMA.NOMBRE,
                            ACTIVIDAD = item.ACTIVIDAD.DESCR,
                            FECHA = subitem.HORA_INICIO.Value.ToShortDateString(),
                            HORARIO = subitem.HORA_INICIO.Value.ToShortTimeString() + " - " + subitem.HORA_TERMINO.Value.ToShortTimeString(),
                            GRUPO = item.GRUPO != null ? item.GRUPO.DESCR : item.GRUPO_PARTICIPANTE_ESTATUS.DESCR,
                            ASISTENCIA = subitem.GRUPO_ASISTENCIA.Where(w => w.ID_GRUPO_HORARIO == subitem.ID_GRUPO_HORARIO && w.ID_CENTRO == subitem.ID_CENTRO && w.ID_TIPO_PROGRAMA == subitem.ID_TIPO_PROGRAMA && w.ID_ACTIVIDAD == subitem.ID_ACTIVIDAD && w.ID_GRUPO == subitem.ID_GRUPO && w.ID_CONSEC == subitem.GRUPO.GRUPO_PARTICIPANTE.Where(wh => wh == item).FirstOrDefault().ID_CONSEC).FirstOrDefault().ASISTENCIA == 1,
                            ESTATUS = subitem.ESTATUS != 1 ? subitem.GRUPO_HORARIO_ESTATUS.DESCR : subitem.GRUPO_ASISTENCIA.Where(w => w.ID_GRUPO_HORARIO == subitem.ID_GRUPO_HORARIO && w.ID_CENTRO == subitem.ID_CENTRO && w.ID_TIPO_PROGRAMA == subitem.ID_TIPO_PROGRAMA && w.ID_ACTIVIDAD == subitem.ID_ACTIVIDAD && w.ID_GRUPO == subitem.ID_GRUPO && w.ID_CONSEC == subitem.GRUPO.GRUPO_PARTICIPANTE.Where(wh => wh == item).FirstOrDefault().ID_CONSEC).FirstOrDefault().ESTATUS != 1 ? subitem.GRUPO_ASISTENCIA.Where(w => w.ID_GRUPO_HORARIO == subitem.ID_GRUPO_HORARIO && w.ID_CENTRO == subitem.ID_CENTRO && w.ID_TIPO_PROGRAMA == subitem.ID_TIPO_PROGRAMA && w.ID_ACTIVIDAD == subitem.ID_ACTIVIDAD && w.ID_GRUPO == subitem.ID_GRUPO && w.ID_CONSEC == subitem.GRUPO.GRUPO_PARTICIPANTE.Where(wh => wh == item).FirstOrDefault().ID_CONSEC).FirstOrDefault().GRUPO_ASISTENCIA_ESTATUS.DESCR : string.Empty,
                            FechaHorario = subitem.HORA_INICIO,

                            ShowCheck = subitem.ESTATUS != 1 ? Visibility.Collapsed : subitem.GRUPO_ASISTENCIA.Where(w => w.ID_GRUPO_HORARIO == subitem.ID_GRUPO_HORARIO && w.ID_CENTRO == subitem.ID_CENTRO && w.ID_TIPO_PROGRAMA == subitem.ID_TIPO_PROGRAMA && w.ID_ACTIVIDAD == subitem.ID_ACTIVIDAD && w.ID_GRUPO == subitem.ID_GRUPO && w.ID_CONSEC == subitem.GRUPO.GRUPO_PARTICIPANTE.Where(wh => wh == item).FirstOrDefault().ID_CONSEC).FirstOrDefault().ESTATUS != 1 ? Visibility.Collapsed : Visibility.Visible,
                            ShowLabel = subitem.ESTATUS != 1 ? Visibility.Visible : subitem.GRUPO_ASISTENCIA.Where(w => w.ID_GRUPO_HORARIO == subitem.ID_GRUPO_HORARIO && w.ID_CENTRO == subitem.ID_CENTRO && w.ID_TIPO_PROGRAMA == subitem.ID_TIPO_PROGRAMA && w.ID_ACTIVIDAD == subitem.ID_ACTIVIDAD && w.ID_GRUPO == subitem.ID_GRUPO && w.ID_CONSEC == subitem.GRUPO.GRUPO_PARTICIPANTE.Where(wh => wh == item).FirstOrDefault().ID_CONSEC).FirstOrDefault().ESTATUS != 1 ? Visibility.Visible : Visibility.Collapsed,
                        });

                ListHorario = new ObservableCollection<HorarioParticipante>(preHorarioParticipante);
            }
            catch (Exception ex)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al cargar la información del participante.", ex);
            }
        }

        private async void OnAgregarCommand(object obj)
        {
            try
            {
                if (SelectExpediente == null || SelectIngreso == null)
                    return;
                var index = 0;
                switch (obj.ToString())
                {
                    case "GRUPO":
                        AgregarListEje = await StaticSourcesViewModel.CargarDatosAsync<ObservableCollection<EJE>>(() => new ObservableCollection<EJE>(new cGrupo().GetData().Select(s => s.ACTIVIDAD.ACTIVIDAD_EJE.Where(w => w.ID_TIPO_PROGRAMA == s.ID_TIPO_PROGRAMA && w.ID_ACTIVIDAD == s.ID_ACTIVIDAD && w.EJE.COMPLEMENTARIO == "N").Distinct().Select(se => se.EJE).FirstOrDefault()).Where(w => w != null).Distinct().OrderBy(o => o.ORDEN)));

                        PopUpsViewModels.ShowPopUp(this, PopUpsViewModels.TipoPopUp.AGREGAR_PARTICIPANTE_GRUPO);
                        break;
                    case "guardar_AgregarGrupo":
                        var result = await StaticSourcesViewModel.OperacionesAsync<bool>("Agregando Participante A Grupo", () =>
                        {
                            try
                            {
                                var grupoParticipante = SelectIngreso.GRUPO_PARTICIPANTE.Where(w => w.ID_ACTIVIDAD == AgregarGrupoSelectedActividad.ID_ACTIVIDAD && w.ID_CENTRO == GlobalVar.gCentro && w.ID_TIPO_PROGRAMA == AgregarGrupoSelectedPrograma.ID_TIPO_PROGRAMA && w.ING_ID_CENTRO == SelectIngreso.ID_CENTRO && w.ING_ID_ANIO == SelectIngreso.ID_ANIO && w.ING_ID_IMPUTADO == SelectIngreso.ID_IMPUTADO && w.ING_ID_INGRESO == SelectIngreso.ID_INGRESO).FirstOrDefault();

                                var entityGP = new cGrupoParticipante().ActualizarParticipanteAsistencia(new GRUPO_PARTICIPANTE()
                                {
                                    EJE = grupoParticipante.EJE,
                                    ESTATUS = 2,
                                    ID_ACTIVIDAD = grupoParticipante.ID_ACTIVIDAD,
                                    ID_CENTRO = grupoParticipante.ID_CENTRO,
                                    ID_GRUPO = AgregarGrupoSelectedGrupo.ID_GRUPO,
                                    ID_CONSEC = grupoParticipante.ID_CONSEC,
                                    ID_TIPO_PROGRAMA = grupoParticipante.ID_TIPO_PROGRAMA,
                                    ING_ID_CENTRO = grupoParticipante.ID_CENTRO,
                                    ING_ID_ANIO = grupoParticipante.ING_ID_ANIO,
                                    ING_ID_IMPUTADO = grupoParticipante.ING_ID_IMPUTADO,
                                    ING_ID_INGRESO = grupoParticipante.ING_ID_INGRESO,
                                    FEC_REGISTRO = grupoParticipante.FEC_REGISTRO
                                });

                                foreach (var ListGH in new cGrupo().GetData().Where(w => w.ID_GRUPO == entityGP.ID_GRUPO).FirstOrDefault().GRUPO_HORARIO)
                                    new cGrupoAsistencia().Insert(new GRUPO_ASISTENCIA()
                                    {
                                        ID_CENTRO = GlobalVar.gCentro,
                                        ID_TIPO_PROGRAMA = ListGH.ID_TIPO_PROGRAMA,
                                        ID_ACTIVIDAD = ListGH.ID_ACTIVIDAD,
                                        ID_GRUPO = AgregarGrupoSelectedGrupo.ID_GRUPO,
                                        ID_GRUPO_HORARIO = ListGH.ID_GRUPO_HORARIO,
                                        ID_CONSEC = entityGP.ID_CONSEC,
                                        FEC_REGISTRO = Fechas.GetFechaDateServer,
                                        ASISTENCIA = null,
                                        EMPALME = 0,
                                        EMP_COORDINACION = 0,
                                        EMP_APROBADO = null,
                                        EMP_FECHA = null,
                                        ESTATUS = 1
                                    });

                                if (ListActividadParticipante.Where(w => w.Revision && w.ListHorario != null && w.State.Equals("Empalme")).Any())
                                {
                                    var listGA = new List<ListaEmpalmesInterno>();
                                    var entitylistGH = new List<GRUPO_HORARIO>();
                                    foreach (var item in ListActividadParticipante.Where(w => w.Revision && w.ListHorario != null && w.State.Equals("Empalme")))
                                    {
                                        foreach (var itemLisAct in item.ListHorario.Where(whe => whe.State.Equals("Empalme")))
                                            entitylistGH.Add(itemLisAct.GrupoHorarioEntity);
                                        listGA.Add(new ListaEmpalmesInterno() { EntityGrupoParticipante = entityGP, ListGrupoHorario = entitylistGH });
                                    }
                                    new cGrupoAsistencia().GenerarEmpalmes(listGA);
                                }
                                return true;
                            }
                            catch (Exception ex)
                            {
                                return false;
                            }
                        });
                        if (result)
                        {
                            BuscarImputado();
                            await new Dialogos().ConfirmacionDialogoReturn("Agregar Participante A Grupo", "El interno a sido ingresado correctamente");
                        }
                        PopUpsViewModels.ClosePopUp(PopUpsViewModels.TipoPopUp.AGREGAR_PARTICIPANTE_GRUPO);
                        break;
                    case "cancelar_AgregarGrupo":
                        PopUpsViewModels.ClosePopUp(PopUpsViewModels.TipoPopUp.AGREGAR_PARTICIPANTE_GRUPO);
                        break;
                    case "FECHA":
                        AgregarListEstatusGrupoHorario = new ObservableCollection<GRUPO_HORARIO_ESTATUS>(new cGrupoHorarioEstatus().GetData().OrderBy(o => o.DESCR));

                        AgregarListEje = await StaticSourcesViewModel.CargarDatosAsync<ObservableCollection<EJE>>(() => new ObservableCollection<EJE>(new cGrupoParticipante().GetData().Where(w => w.ID_CENTRO == SelectIngreso.ID_UB_CENTRO && w.ING_ID_ANIO == SelectIngreso.ID_ANIO && w.ING_ID_IMPUTADO == SelectIngreso.ID_IMPUTADO && w.ING_ID_INGRESO == SelectIngreso.ID_INGRESO).Select(se => se.EJE1).Distinct().OrderBy(o => o.ORDEN)));
                        if (AgregarListEje.Count > 0)
                        {
                            AgregarListEje.Insert(0, new EJE() { COMPLEMENTARIO = "MODELO" });
                            index = AgregarListEje.IndexOf(AgregarListEje.Where(w => w.COMPLEMENTARIO != "MODELO").OrderBy(o => o.COMPLEMENTARIO == "S").ThenBy(t => t.ORDEN).Where(w => w.COMPLEMENTARIO == "S").FirstOrDefault());
                            if (index > 0)
                                AgregarListEje.Insert(index, new EJE() { COMPLEMENTARIO = "COMPLEMENTARIO" });
                        }

                        PopUpsViewModels.ShowPopUp(this, PopUpsViewModels.TipoPopUp.AGREGAR_FECHA_PARTICIPANTE);
                        break;
                    case "guardar_AgregarFecha":
                        PopUpsViewModels.ClosePopUp(PopUpsViewModels.TipoPopUp.AGREGAR_FECHA_PARTICIPANTE);
                        break;
                    case "cancelar_AgregarFecha":
                        PopUpsViewModels.ClosePopUp(PopUpsViewModels.TipoPopUp.AGREGAR_FECHA_PARTICIPANTE);
                        break;
                    case "CITA":
                        var hoy = Fechas.GetFechaDateServer;
                        if (new cAtencionIngreso().ObtenerSolicitudesPorMes(SelectIngreso.ID_CENTRO, SelectIngreso.ID_ANIO, SelectIngreso.ID_IMPUTADO, SelectIngreso.ID_INGRESO, hoy.Year, hoy.Month) > VCitasMes)
                        {
                            new Dialogos().ConfirmacionDialogo("Validación", "En interno ha sobrepasado el número de solicitudes por mes.");
                            break;
                        }
                        LimpiarSolicitudCita();
                        VaidarSolicitadCita();
                        PopUpsViewModels.ShowPopUp(this, PopUpsViewModels.TipoPopUp.SOLICITUD_CITA);
                        break;
                    case "guardar_SolicitudCita":
                        GuardarSolicitudCita();
                        break;
                    case "cancelar_SolicitudCita":
                        LimpiarSolicitudCita();
                        base.ClearRules();
                        PopUpsViewModels.ClosePopUp(PopUpsViewModels.TipoPopUp.SOLICITUD_CITA);
                        break;
                    case "COMPLEMENTARIA":
                        AgregarListEje = await StaticSourcesViewModel.CargarDatosAsync<ObservableCollection<EJE>>(() => new ObservableCollection<EJE>(new cGrupo().GetData().Select(s => s.ACTIVIDAD.ACTIVIDAD_EJE.Where(w => w.ID_TIPO_PROGRAMA == s.ID_TIPO_PROGRAMA && w.ID_ACTIVIDAD == s.ID_ACTIVIDAD && w.EJE.COMPLEMENTARIO == "S").Distinct().Select(se => se.EJE).FirstOrDefault()).Where(w => w != null).Distinct().OrderBy(o => o.ORDEN)));
                        PopUpsViewModels.ShowPopUp(this, PopUpsViewModels.TipoPopUp.AGREGAR_PARTICIPANTE_COMPLEMENTARIO);
                        break;
                    case "guardar_AgregarComplementaria":
                        var resultcompl = await StaticSourcesViewModel.OperacionesAsync<bool>("Agregando Participante A Complementario", () =>
                        {
                            try
                            {
                                var grupoParticipante = SelectIngreso.GRUPO_PARTICIPANTE.Where(w => w.ID_ACTIVIDAD == AgregarGrupoSelectedActividad.ID_ACTIVIDAD && w.ID_CENTRO == GlobalVar.gCentro && w.ID_TIPO_PROGRAMA == AgregarGrupoSelectedPrograma.ID_TIPO_PROGRAMA && w.ING_ID_CENTRO == SelectIngreso.ID_CENTRO && w.ING_ID_ANIO == SelectIngreso.ID_ANIO && w.ING_ID_IMPUTADO == SelectIngreso.ID_IMPUTADO && w.ING_ID_INGRESO == SelectIngreso.ID_INGRESO).FirstOrDefault();

                                if (grupoParticipante == null)
                                    grupoParticipante = new cGrupoParticipante().InsertarParticipanteAsistencia(new GRUPO_PARTICIPANTE()
                                    {
                                        EJE = AgregarGrupoSelectedEje.ID_EJE,
                                        ESTATUS = 2,
                                        ID_ACTIVIDAD = AgregarGrupoSelectedActividad.ID_ACTIVIDAD,
                                        ID_CENTRO = GlobalVar.gCentro,
                                        ID_GRUPO = AgregarGrupoSelectedGrupo.ID_GRUPO,
                                        ID_TIPO_PROGRAMA = AgregarGrupoSelectedPrograma.ID_TIPO_PROGRAMA,
                                        ING_ID_CENTRO = SelectIngreso.ID_CENTRO,
                                        ING_ID_ANIO = SelectIngreso.ID_ANIO,
                                        ING_ID_IMPUTADO = SelectIngreso.ID_IMPUTADO,
                                        ING_ID_INGRESO = SelectIngreso.ID_INGRESO,
                                        FEC_REGISTRO = Fechas.GetFechaDateServer
                                    });
                                else
                                    grupoParticipante = new cGrupoParticipante().ActualizarParticipanteAsistencia(new GRUPO_PARTICIPANTE()
                                    {
                                        EJE = grupoParticipante.EJE,
                                        ESTATUS = 2,
                                        ID_ACTIVIDAD = grupoParticipante.ID_ACTIVIDAD,
                                        ID_CENTRO = grupoParticipante.ID_CENTRO,
                                        ID_GRUPO = AgregarGrupoSelectedGrupo.ID_GRUPO,
                                        ID_CONSEC = grupoParticipante.ID_CONSEC,
                                        ID_TIPO_PROGRAMA = grupoParticipante.ID_TIPO_PROGRAMA,
                                        ING_ID_CENTRO = grupoParticipante.ID_CENTRO,
                                        ING_ID_ANIO = grupoParticipante.ING_ID_ANIO,
                                        ING_ID_IMPUTADO = grupoParticipante.ING_ID_IMPUTADO,
                                        ING_ID_INGRESO = grupoParticipante.ING_ID_INGRESO,
                                        FEC_REGISTRO = grupoParticipante.FEC_REGISTRO
                                    });

                                foreach (var ListGH in new cGrupo().GetData().Where(w => w.ID_GRUPO == grupoParticipante.ID_GRUPO).FirstOrDefault().GRUPO_HORARIO)
                                    new cGrupoAsistencia().Insert(new GRUPO_ASISTENCIA()
                                    {
                                        ID_CENTRO = GlobalVar.gCentro,
                                        ID_TIPO_PROGRAMA = ListGH.ID_TIPO_PROGRAMA,
                                        ID_ACTIVIDAD = ListGH.ID_ACTIVIDAD,
                                        ID_GRUPO = AgregarGrupoSelectedGrupo.ID_GRUPO,
                                        ID_GRUPO_HORARIO = ListGH.ID_GRUPO_HORARIO,
                                        ID_CONSEC = grupoParticipante.ID_CONSEC,
                                        FEC_REGISTRO = Fechas.GetFechaDateServer,
                                        ASISTENCIA = null,
                                        EMPALME = 0,
                                        EMP_COORDINACION = 0,
                                        EMP_APROBADO = null,
                                        EMP_FECHA = null,
                                        ESTATUS = 1
                                    });

                                if (ListActividadParticipante.Where(w => w.Revision && w.ListHorario != null && w.State.Equals("Empalme")).Any())
                                {
                                    var listGA = new List<ListaEmpalmesInterno>();
                                    var entitylistGH = new List<GRUPO_HORARIO>();
                                    foreach (var item in ListActividadParticipante.Where(w => w.Revision && w.ListHorario != null && w.State.Equals("Empalme")))
                                    {
                                        foreach (var itemLisAct in item.ListHorario.Where(whe => whe.State.Equals("Empalme")))
                                            entitylistGH.Add(itemLisAct.GrupoHorarioEntity);
                                        listGA.Add(new ListaEmpalmesInterno() { EntityGrupoParticipante = grupoParticipante, ListGrupoHorario = entitylistGH });
                                    }
                                    new cGrupoAsistencia().GenerarEmpalmes(listGA);
                                }
                                return true;
                            }
                            catch (Exception ex)
                            {
                                return false;
                            }
                        });
                        if (resultcompl)
                        {
                            BuscarImputado();
                            await new Dialogos().ConfirmacionDialogoReturn("Agregar Participante A Complementario", "El interno a sido ingresado correctamente");
                        }
                        PopUpsViewModels.ClosePopUp(PopUpsViewModels.TipoPopUp.AGREGAR_PARTICIPANTE_COMPLEMENTARIO);
                        break;
                    case "cancelar_AgregarComplementaria":
                        PopUpsViewModels.ClosePopUp(PopUpsViewModels.TipoPopUp.AGREGAR_PARTICIPANTE_COMPLEMENTARIO);
                        break;
                    case "SANCIONES":
                        var ListEstatus = new ObservableCollection<GRUPO_PARTICIPANTE_ESTATUS>(new cGrupoParticipanteEstatus().GetData().Where(w => w.ID_ESTATUS == 2 || w.ID_ESTATUS == 3 || w.ID_ESTATUS == 4));
                        ListGruposInterno = new ObservableCollection<GruposCancelarSuspender>(new cGrupoParticipante().GetData().Where(w => w.ID_GRUPO != null && w.GRUPO.ID_ESTATUS_GRUPO == 1 && w.ING_ID_CENTRO == SelectIngreso.ID_UB_CENTRO && w.ING_ID_ANIO == SelectIngreso.ID_ANIO && w.ING_ID_IMPUTADO == SelectIngreso.ID_IMPUTADO && w.ING_ID_INGRESO == SelectIngreso.ID_INGRESO && (w.GRUPO_PARTICIPANTE_CANCELADO.Where(wh => (wh.ID_CENTRO == w.ID_CENTRO && wh.ID_TIPO_PROGRAMA == w.ID_TIPO_PROGRAMA && wh.ID_ACTIVIDAD == w.ID_ACTIVIDAD && wh.ID_CONSEC == w.ID_CONSEC && wh.ID_GRUPO == w.ID_GRUPO)).Any() ?

                                w.GRUPO_PARTICIPANTE_CANCELADO.Where(wh => (wh.ID_CENTRO == w.ID_CENTRO && wh.ID_TIPO_PROGRAMA == w.ID_TIPO_PROGRAMA && wh.ID_ACTIVIDAD == w.ID_ACTIVIDAD && wh.ID_CONSEC == w.ID_CONSEC) &&
                                    (wh.RESPUESTA_FEC == null ?

                                    (wh.RESPUESTA_FEC == null && wh.ESTATUS == 0) :

                                    (w.GRUPO_PARTICIPANTE_CANCELADO.Where(whe => (whe.ID_CENTRO == w.ID_CENTRO && whe.ID_TIPO_PROGRAMA == w.ID_TIPO_PROGRAMA && whe.ID_ACTIVIDAD == w.ID_ACTIVIDAD && whe.ID_CONSEC == w.ID_CONSEC) && whe.RESPUESTA_FEC != null && (whe.ESTATUS == 0 || whe.ESTATUS == 2)).Count() == w.GRUPO_PARTICIPANTE_CANCELADO.Where(whe => (whe.ID_CENTRO == w.ID_CENTRO && whe.ID_TIPO_PROGRAMA == w.ID_TIPO_PROGRAMA && whe.ID_ACTIVIDAD == w.ID_ACTIVIDAD && whe.ID_CONSEC == w.ID_CONSEC)).Count()))).Any()

                                    : true)).OrderBy(o => o.ING_ID_ANIO).ThenBy(t => t.ING_ID_IMPUTADO).AsEnumerable().Select(s => new GruposCancelarSuspender()
                                    {
                                        Entity = s,
                                        EJE = s.EJE1.DESCR,
                                        PROGRAMA = s.ACTIVIDAD.TIPO_PROGRAMA.NOMBRE,
                                        ACTIVIDAD = s.ACTIVIDAD.DESCR,
                                        GRUPO = s.GRUPO.DESCR,
                                        ListEstatusGrupoParticipante = ListEstatus,
                                        SelectEstatus = s.ESTATUS
                                    }));

                        PopUpsViewModels.ShowPopUp(this, PopUpsViewModels.TipoPopUp.AGREGAR_CANCELAR_SUSPENDER);
                        break;
                    case "guardar_AgregarSanciones":
                        var respuesta = await StaticSourcesViewModel.OperacionesAsync<bool>("Actualizando Estatus A Los Internos", () =>
                            {
                                try
                                {
                                    var dal = new cGrupoParticipanteCancelado();
                                    foreach (var item in ListGruposInterno.Where(w => w.SelectEstatus != 2))
                                    {
                                        dal.InsertarParticipanteCancelado(new GRUPO_PARTICIPANTE_CANCELADO()
                                        {
                                            ID_CENTRO = item.Entity.ID_CENTRO,
                                            ID_ACTIVIDAD = item.Entity.ID_ACTIVIDAD,
                                            ID_TIPO_PROGRAMA = item.Entity.ID_TIPO_PROGRAMA,
                                            ID_CONSEC = item.Entity.ID_CONSEC,
                                            ID_GRUPO = item.Entity.ID_GRUPO.Value,
                                            ID_USUARIO = GlobalVariables.gUser,
                                            SOLICITUD_FEC = Fechas.GetFechaDateServer,
                                            RESPUESTA_FEC = null,
                                            MOTIVO = item.MOTIVO,
                                            ID_ESTATUS = item.SelectEstatus
                                        });
                                    }
                                    return true;
                                }
                                catch (Exception ex)
                                {
                                    StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al modificar el estatus de los integrantes del grupo", ex);
                                    return false;
                                }
                            });

                        if (respuesta)
                        {
                            PopUpsViewModels.ClosePopUp(PopUpsViewModels.TipoPopUp.EDITAR_INTEGRANTES_GRUPO);
                            await new Dialogos().ConfirmacionDialogoReturn("Control Participantes", "El Cambio de Estatus Se Actualizo Exitosamente");
                        }
                        PopUpsViewModels.ClosePopUp(PopUpsViewModels.TipoPopUp.AGREGAR_CANCELAR_SUSPENDER);
                        break;
                    case "cancelar_AgregarSanciones":
                        PopUpsViewModels.ClosePopUp(PopUpsViewModels.TipoPopUp.AGREGAR_CANCELAR_SUSPENDER);
                        break;
                    default:
                        break;
                }
            }
            catch (Exception ex)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error en el proceso", ex);
            }
        }

        private async void RevisarHorarioCompatible()
        {
            try
            {
                ListActividadParticipante = new ObservableCollection<ListaActividad>();
                if (_AgregarGrupoSelectedEje != null && _AgregarGrupoSelectedPrograma != null && _AgregarGrupoSelectedActividad != null && _AgregarGrupoSelectedGrupo != null)
                    await StaticSourcesViewModel.CargarDatosMetodoAsync(() =>
                    {
                        var HorarioSelectedGrupo = new cGrupoHorario().GetData().Where(w => w.ID_CENTRO == GlobalVar.gCentro && w.ID_GRUPO == AgregarGrupoSelectedGrupo.ID_GRUPO && w.ID_TIPO_PROGRAMA == AgregarGrupoSelectedPrograma.ID_TIPO_PROGRAMA && w.ID_ACTIVIDAD == AgregarGrupoSelectedActividad.ID_ACTIVIDAD).ToList();
                        var HorarioSelectedParticipante = new cGrupoHorario().GetData().Where(w => w.GRUPO.GRUPO_PARTICIPANTE.Where(wh => (wh.ID_CENTRO == SelectIngreso.ID_UB_CENTRO && wh.ING_ID_ANIO == SelectIngreso.ID_ANIO && wh.ING_ID_IMPUTADO == SelectIngreso.ID_IMPUTADO && wh.ING_ID_INGRESO == SelectIngreso.ID_INGRESO)).Any()).ToList();

                        var GrupoHorarioEmpalme = new List<GRUPO_HORARIO>();
                        foreach (var itemGrupo in HorarioSelectedGrupo)
                            if (HorarioSelectedParticipante.Where(w => (w.HORA_INICIO >= itemGrupo.HORA_INICIO && w.HORA_TERMINO <= itemGrupo.HORA_TERMINO) && w.ESTATUS == 1 && w.GRUPO.ID_ESTATUS_GRUPO == 1).Any())
                                GrupoHorarioEmpalme.Add(HorarioSelectedParticipante.Where(w => (w.HORA_INICIO >= itemGrupo.HORA_INICIO && w.HORA_TERMINO <= itemGrupo.HORA_TERMINO) && w.ESTATUS == 1 && w.GRUPO.ID_ESTATUS_GRUPO == 1).FirstOrDefault());

                        var Actividades = new List<ListaActividad>();
                        foreach (var item in GrupoHorarioEmpalme.GroupBy(g => g.GRUPO))
                        {
                            Actividades.Add(new ListaActividad()
                            {
                                NombreGrupo = item.Key.DESCR,
                                NombreActividad = item.Key.ACTIVIDAD.DESCR,
                                RecurrenciaActividad = item.Key.RECURRENCIA,
                                InicioActividad = item.Key.FEC_INICIO.Value.ToShortDateString(),
                                FinActividad = item.Key.FEC_FIN.Value.ToShortDateString(),
                                ListHorario = new ObservableCollection<ListHorario>(item.OrderBy(o => o.HORA_INICIO).Select(s => new ListHorario()
                                                {
                                                    GrupoHorarioEntity = s,
                                                    AREADESCR = s.AREA.DESCR,
                                                    GRUPO_HORARIO_ESTATUSDESCR = s.GRUPO_HORARIO_ESTATUS.DESCR,
                                                    DESCRDIA = s.HORA_INICIO.Value.ToLongDateString(),
                                                    strHORA_INICIO = s.HORA_INICIO.Value.ToShortTimeString(),
                                                    strHORA_TERMIINO = s.HORA_TERMINO.Value.ToShortTimeString(),
                                                    State = "Empalme"
                                                }).OrderBy(o => o.HORA_INICIO)),
                                orden = item.Key.ACTIVIDAD.PRIORIDAD,
                                State = "Empalme"
                            });
                        }
                        ListActividadParticipante = new ObservableCollection<ListaActividad>(Actividades.OrderBy(o => o.orden).ToList());
                    });
            }
            catch (Exception ex)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al revisar horarios", ex);
            }
        }

        private async void RevisarFechasParticipante()
        {
            try
            {
                ListActividadParticipante = new ObservableCollection<ListaActividad>();
                if (_AgregarFechaSelectedEje != null && _AgregarFechaSelectedPrograma != null && _AgregarFechaSelectedActividad != null && InicioDia != null && TerminoDia != null)
                    await StaticSourcesViewModel.CargarDatosMetodoAsync(() =>
                    {
                        var horainicio = new DateTime(AgregarFecha.Value.Year, AgregarFecha.Value.Month, AgregarFecha.Value.Day, InicioDia.Value.Hour, InicioDia.Value.Minute, 0);
                        var horafinal = new DateTime(AgregarFecha.Value.Year, AgregarFecha.Value.Month, AgregarFecha.Value.Day, TerminoDia.Value.Hour, TerminoDia.Value.Minute, 0);
                        var HorarioSelectedParticipante = new cGrupoHorario().GetData().Where(w => w.GRUPO.GRUPO_PARTICIPANTE.Where(wh => (wh.ID_CENTRO == SelectIngreso.ID_UB_CENTRO && wh.ING_ID_ANIO == SelectIngreso.ID_ANIO && wh.ING_ID_IMPUTADO == SelectIngreso.ID_IMPUTADO && wh.ING_ID_INGRESO == SelectIngreso.ID_INGRESO)).Any());

                        var GrupoHorarioEmpalme = new List<GRUPO_HORARIO>();

                        if (HorarioSelectedParticipante.Where(w => (w.HORA_INICIO >= horainicio && w.HORA_TERMINO <= horafinal) && w.ESTATUS == 1 && w.GRUPO.ID_ESTATUS_GRUPO == 1).Any())
                            GrupoHorarioEmpalme = HorarioSelectedParticipante.Where(w => (w.HORA_INICIO >= horainicio && w.HORA_TERMINO <= horafinal) && w.ESTATUS == 1 && w.GRUPO.ID_ESTATUS_GRUPO == 1).ToList();

                        var Actividades = new List<ListaActividad>();
                        foreach (var item in GrupoHorarioEmpalme.GroupBy(g => g.GRUPO))
                        {
                            Actividades.Add(new ListaActividad()
                            {
                                NombreGrupo = item.Key.DESCR,
                                NombreActividad = item.Key.ACTIVIDAD.DESCR,
                                RecurrenciaActividad = item.Key.RECURRENCIA,
                                InicioActividad = item.Key.FEC_INICIO.Value.ToShortDateString(),
                                FinActividad = item.Key.FEC_FIN.Value.ToShortDateString(),
                                ListHorario = new ObservableCollection<ListHorario>(item.OrderBy(o => o.HORA_INICIO).Select(s => new ListHorario()
                                {
                                    GrupoHorarioEntity = s,
                                    AREADESCR = s.AREA.DESCR,
                                    GRUPO_HORARIO_ESTATUSDESCR = s.GRUPO_HORARIO_ESTATUS.DESCR,
                                    DESCRDIA = s.HORA_INICIO.Value.ToLongDateString(),
                                    strHORA_INICIO = s.HORA_INICIO.Value.ToShortTimeString(),
                                    strHORA_TERMIINO = s.HORA_TERMINO.Value.ToShortTimeString(),
                                    State = "Empalme"
                                }).OrderBy(o => o.HORA_INICIO)),
                                orden = item.Key.ACTIVIDAD.PRIORIDAD,
                                State = "Empalme"
                            });
                        }
                        ListActividadParticipante = new ObservableCollection<ListaActividad>(Actividades.OrderBy(o => o.orden).ToList());
                    });
            }
            catch (Exception ex)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al revisar fechas de participantes ", ex);
            }
        }

        #region [Solicitud de Citas]
        private void LimpiarSolicitudCita()
        {
            CAreaTraslado = CAreaSolicita = -1;
            CActividad = string.Empty;//CAutorizacion = COficialTraslado = string.Empty;
        }

        private void VaidarSolicitadCita()
        {
            try
            {
                base.ClearRules();
                base.AddRule(() => CAreaTraslado, () => CAreaTraslado != -1, "ÁREA TRASLADO ES REQUERIDA!");
                base.AddRule(() => CActividad, () => !string.IsNullOrEmpty(CActividad), "ACTIVIDAD ES REQUERIDA!");
                base.AddRule(() => CAreaSolicita, () => CAreaSolicita != -1, "ÁREA QUE SOLICITA ES REQUERIDA!");
               // base.AddRule(() => CAutorizacion, () => !string.IsNullOrEmpty(CAutorizacion), "AUTORIZACIÓN ES REQUERIDA!");
                //base.AddRule(() => COficialTraslado, () => !string.IsNullOrEmpty(COficialTraslado), "OFICIAL QUE TRASLADA ES REQUERIDO!");
                OnPropertyChanged("CAreaTraslado");
                OnPropertyChanged("CActividad");
                OnPropertyChanged("CAreaSolicita");
                //OnPropertyChanged("CAutorizacion");
                //OnPropertyChanged("COficialTraslado");
            }
            catch (Exception ex)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al validar solicitud de cita ", ex);
            }
        }

        private async void GuardarSolicitudCita()
        {
            try
            {
                if (SelectIngreso == null)
                {
                    new Dialogos().ConfirmacionDialogo("Validación", "Debes seleccionar un imputado.");
                    return;
                }
                if (base.HasErrors)
                {
                    new Dialogos().ConfirmacionDialogo("Validación", "Debes capturar los datos requeridos.");
                    return;
                }

                var resultado = await StaticSourcesViewModel.OperacionesAsync<bool>("Guardando Solicitud de Atención", () =>
                {
                    try
                    {
                        DateTime fecha = Fechas.GetFechaDateServer;
                        var obj = new ATENCION_SOLICITUD();
                        obj.ID_TECNICA = CAreaSolicita;
                        obj.ID_AREA = CAreaTraslado;
                        obj.SOLICITUD_FEC = fecha;
                        obj.ACTIVIDAD = CActividad;
                        obj.AUTORIZACION = string.Empty;// CAutorizacion;
                        obj.OFICIAL_TRASLADA = string.Empty;// COficialTraslado;
                        obj.ID_CENTRO = GlobalVar.gCentro;
                        obj.ESTATUS = (short)enumSolicitudCita.SOLICITADA;
                        #region Atencion Ingreso
                        List<ATENCION_INGRESO> ai = new List<ATENCION_INGRESO>();
                        ai.Add(new ATENCION_INGRESO()
                        {
                            ID_CENTRO = SelectIngreso.ID_CENTRO,
                            ID_ANIO = SelectIngreso.ID_ANIO,
                            ID_IMPUTADO = SelectIngreso.ID_IMPUTADO,
                            ID_INGRESO = SelectIngreso.ID_INGRESO,
                            REGISTRO_FEC = fecha,
                            ID_CENTRO_UBI = (short)SelectIngreso.ID_UB_CENTRO,
                        });
                        obj.ATENCION_INGRESO = ai;
                        #endregion
                        obj.ID_ATENCION = new cAtencionSolicitud().Agregar(obj);
                        if (obj.ID_ATENCION > 0)
                        {
                            return true;
                        }
                        else
                            return false;
                    }
                    catch (Exception ex)
                    {
                        StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al guardar solicitud de cita ", ex);
                    }
                    return false;
                });

                if (resultado)
                {
                    new Dialogos().ConfirmacionDialogo("Éxito", "La solicitud se registrado correctamente.");
                    LimpiarSolicitudCita();
                    base.ClearRules();
                    PopUpsViewModels.ClosePopUp(PopUpsViewModels.TipoPopUp.SOLICITUD_CITA);
                }
                else
                {
                    new Dialogos().ConfirmacionDialogo("Error", "Ocurrio un problema al registrar la solicitud.");
                }
            }
            catch (Exception ex)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al guardar solicitud de cita ", ex);
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
