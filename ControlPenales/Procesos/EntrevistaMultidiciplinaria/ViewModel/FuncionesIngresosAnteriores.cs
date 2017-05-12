using ControlPenales.Clases;
using SSP.Controlador.Catalogo.Justicia;
using SSP.Controlador.Catalogo.Justicia.EstudioMI;
using SSP.Servidor;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
namespace ControlPenales
{
    partial class EntrevistaMultidiciplinariaViewModel : ValidationViewModelBase
    {
        //MAYORES
        private void AgregarIngresoAnterior()
        {
            try
            {
                if (LstIngresosAnteriores == null)
                    LstIngresosAnteriores = new ObservableCollection<EMI_INGRESO_ANTERIOR>();

                if (SelectedIngresoAnterior == null)//agregamos un nuevo ingreso
                {
                    LstIngresosAnteriores.Add(new EMI_INGRESO_ANTERIOR()
                    {///TODO: cambios delito
                        ID_TIPO = 2,
                        ID_EMISOR = selectedEmisorIngreso.ID_EMISOR,
                        PERIODO_RECLUSION = PeriodoReclusionIngreso,
                        SANCIONES = SancionesIngreso,
                        ID_DELITO = SelectedDelitoIngreso != null ? SelectedDelitoIngreso.ID_DELITO : new long?(),
                        ID_FUERO = SelectedDelitoIngreso != null ? SelectedDelitoIngreso.ID_FUERO : string.Empty,
                        EMISOR = SelectedEmisorIngreso,
                        DELITO = DelitoDescripcion
                        //DELITO = SelectedDelitoIngreso
                    });
                }
                else//editamos un ingreso
                {///TODO: cambios delito
                    SelectedIngresoAnterior.ID_EMISOR = selectedEmisorIngreso.ID_EMISOR;
                    SelectedIngresoAnterior.PERIODO_RECLUSION = PeriodoReclusionIngreso;
                    SelectedIngresoAnterior.SANCIONES = SancionesIngreso;
                    SelectedIngresoAnterior.ID_DELITO = SelectedDelitoIngreso != null ? SelectedDelitoIngreso.ID_DELITO : new long?();
                    SelectedIngresoAnterior.ID_FUERO = SelectedDelitoIngreso != null ? SelectedDelitoIngreso.ID_FUERO : string.Empty;
                    SelectedIngresoAnterior.EMISOR = SelectedEmisorIngreso;
                    SelectedIngresoAnterior.DELITO = DelitoDescripcion;
                    //SelectedIngresoAnterior.DELITO = SelectedDelitoIngreso;
                    LstIngresosAnteriores = new ObservableCollection<EMI_INGRESO_ANTERIOR>(LstIngresosAnteriores);
                }
                //LIMPIAMOS LOS CAMPOS
                EmptyIngresosAnteriores = true;
                LimpiarIngresoanterior();
                if (LstIngresosAnteriores.Count > 0)
                    EmptyIngresosAnteriores = false;
                else
                    EmptyIngresosAnteriores = true;
            }
            catch (Exception ex)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al agregar ingreso anterior", ex);
            }
        }

        private void EliminarIngresoAnterior()
        {
            try
            {
                if (SelectedIngresoAnterior != null)
                {
                    LstIngresosAnteriores.Remove(SelectedIngresoAnterior);
                    if (LstIngresosAnteriores.Count > 0)
                        EmptyIngresosAnteriores = false;
                    else
                        EmptyIngresosAnteriores = true;
                }
            }
            catch (Exception ex)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al eliminar ingreso anterior", ex);
            }
        }

        private void PopulateIngrsoAnteriorPop()
        {
            try
            {
                if (SelectedIngresoAnterior != null)
                {
                    SelectedEmisorIngreso = LstEmisor.Where(w => w.ID_EMISOR == SelectedIngresoAnterior.ID_EMISOR).FirstOrDefault();
                    DelitoDescripcion = SelectedIngresoAnterior.DELITO;
                    //SelectedDelitoIngreso = LstDelitosCP.Where(w => w.ID_DELITO == SelectedIngresoAnterior.ID_DELITO && w.ID_FUERO == SelectedIngresoAnterior.ID_FUERO).FirstOrDefault();
                    PeriodoReclusionIngreso = SelectedIngresoAnterior.PERIODO_RECLUSION;
                    SancionesIngreso = SelectedIngresoAnterior.SANCIONES;
                }
            }
            catch (Exception ex)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al establecer ingreso anterior", ex);
            }
        }

        private void GuardarIngresoAnterior()
        {
            try
            {
                if (emiActual != null)
                {
                    if (LstIngresosAnteriores != null)
                    {
                        //short index = 1;
                        //var ingresos = new List<EMI_INGRESO_ANTERIOR>();
                        //foreach (var ingreso in LstIngresosAnteriores)
                        //{
                        //    ingresos.Add(new EMI_INGRESO_ANTERIOR()
                        //    {
                        //        ID_EMI = emiActual.ID_EMI,
                        //        ID_EMI_CONS = emiActual.ID_EMI_CONS,
                        //        ID_TIPO = ingreso.ID_TIPO,
                        //        ID_CONSEC = index,
                        //        ID_EMISOR = ingreso.ID_EMISOR,
                        //        PERIODO_RECLUSION = ingreso.PERIODO_RECLUSION,
                        //        SANCIONES = ingreso.SANCIONES,
                        //        ID_FUERO = ingreso.ID_FUERO,
                        //        ID_DELITO = ingreso.ID_DELITO,
                        //        ID_CENTRO = ingreso.ID_CENTRO,
                        //        ID_ANIO = ingreso.ID_ANIO,
                        //        DELITO = ingreso.DELITO,
                        //        ID_IMPUTADO = ingreso.ID_IMPUTADO,
                        //        ID_INGRESO = ingreso.ID_INGRESO
                        //    });
                        //    index++;
                        //}
                        var ingresos = new List<EMI_INGRESO_ANTERIOR>(LstIngresosAnteriores == null ? null : LstIngresosAnteriores.Select((w, i) => new EMI_INGRESO_ANTERIOR()
                        {
                                    ID_EMI = emiActual.ID_EMI,
                                    ID_EMI_CONS = emiActual.ID_EMI_CONS,
                                    ID_TIPO = w.ID_TIPO,
                                    ID_CONSEC = Convert.ToInt16(i + 1),
                                    ID_EMISOR = w.ID_EMISOR,
                                    PERIODO_RECLUSION = w.PERIODO_RECLUSION,
                                    SANCIONES = w.SANCIONES,
                                    ID_FUERO = w.ID_FUERO,
                                    ID_DELITO = w.ID_DELITO,
                                    ID_CENTRO = w.ID_CENTRO,
                                    ID_ANIO = w.ID_ANIO,
                                    DELITO = w.DELITO,
                                    ID_IMPUTADO = w.ID_IMPUTADO,
                                    ID_INGRESO = w.ID_INGRESO
                        }));

                        if (emiActual.EMI_INGRESO_ANTERIOR.Count(w => w.ID_TIPO == 2) > 0)
                        {
                            if (!PEditar)
                            {
                                StaticSourcesViewModel.Mensaje("Validación", "Su usuario no cuenta con permisos para realizar esta acción.", StaticSourcesViewModel.enumTipoMensaje.MESNAJE_ADVERTENCIA, 5);
                                return;
                            }
                        }
                        else
                        {
                            if (!PInsertar)
                            {
                                StaticSourcesViewModel.Mensaje("Validación", "Su usuario no cuenta con permisos para realizar esta acción.", StaticSourcesViewModel.enumTipoMensaje.MESNAJE_ADVERTENCIA, 5);
                                return;
                            }
                        }
                        
                        if (new cEMIIngresosAnteriores().Insertar(emiActual.ID_EMI, emiActual.ID_EMI_CONS, 2, ingresos))
                        {
                                IngresoAnteriorMenorEnabled = true;
                                LstIngresosAnteriores = new ObservableCollection<EMI_INGRESO_ANTERIOR>(new cEMIIngresosAnteriores().Obtener(emiActual.ID_EMI, emiActual.ID_EMI_CONS, 2));
                                Mensaje(true, "Ingresos Anteriores CE.RE.SO.");
                            }
                            else
                                Mensaje(false, "Ingresos Anteriores CE.RE.SO.");
                        

                    }
                }
                else
                    AvisoImputadoVacio();
            }
            catch (Exception ex)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al guardar ingreso anterior", ex);
            }
        }

        //private void PopulateIngresosAnteriores2()
        //{
        //    if (emiActual != null)
        //    {
        //        LstIngresosAnteriores = new ObservableCollection<EMI_INGRESO_ANTERIOR>(new cEMIIngresosAnteriores().Obtener(emiActual.ID_EMI, emiActual.ID_EMI_CONS, 2));
        //        if (LstIngresosAnteriores != null)
        //        {
        //            if (LstIngresosAnteriores.Count > 0)
        //                EmptyIngresosAnteriores = false;
        //            else
        //                EmptyIngresosAnteriores = true;
        //        }
        //        else
        //        {
        //            EmptyIngresosAnteriores = true;
        //        }
        //    }
        //    else
        //    {
        //        LstIngresosAnteriores = new ObservableCollection<EMI_INGRESO_ANTERIOR>();
        //        EmptyIngresosAnteriores = true;
        //    }
        //}

        private void PopulateIngresosAnterioresSistema()
        {
            try
            {
                LstIAS = new ObservableCollection<Clases.IngresoAinterior>();
                int? anios, meses, dias;
                if (emiActual != null)
                {
                    if (LstEmisor == null)
                        LstEmisor = new ObservableCollection<EMISOR>(new cEmisor().Obtener());
                    if (LstDelitosCP == null)
                        LstDelitosCP = new ObservableCollection<DELITO>(new cDelito().ObtenerTodos());
                    //if (emiActual.INGRESO != null)
                    //if (SelectIngreso != null)
                        //if (emiActual.INGRESO.IMPUTADO != null)
                        //if (SelectIngreso.IMPUTADO != null)
                            //if (emiActual.INGRESO.IMPUTADO.INGRESO != null)
                            if (SelectIngreso != null)
                            {
                                //var anteriores = emiActual.INGRESO.IMPUTADO.INGRESO.Where(w => w.ID_INGRESO < emiActual.INGRESO.ID_INGRESO);
                                var anteriores = SelectIngreso.IMPUTADO.INGRESO.Where(w => w.ID_INGRESO < SelectIngreso.ID_INGRESO);
                                if (anteriores != null)
                                {
                                    foreach (var ing in anteriores)
                                    {

                                        if (LstIngresosAnteriores.Where(w => w.ID_CENTRO == ing.ID_CENTRO && w.ID_ANIO == ing.ID_ANIO && w.ID_IMPUTADO == ing.ID_IMPUTADO && w.ID_INGRESO == ing.ID_INGRESO).Count() == 0)
                                        {

                                            anios = meses = dias = 0;
                                            var obj = new IngresoAinterior();
                                            obj.Seleccione = false;
                                            obj.IdCentro = ing.ID_CENTRO;
                                            obj.IdAnio = ing.ID_ANIO;
                                            obj.IdImputado = ing.ID_IMPUTADO;
                                            obj.IdIngreso = ing.ID_INGRESO;
                                            ////EMISOR
                                            obj.Emisor = ing.CENTRO.EMISOR;
                                            //DELITO
                                            if (ing.CAUSA_PENAL != null)
                                            {
                                                var cp = ing.CAUSA_PENAL.Where(w => w.ID_ESTATUS_CP == 4).FirstOrDefault();//estatus concluido
                                                if (cp != null)
                                                {
                                                    if (cp.CAUSA_PENAL_DELITO != null)
                                                    {
                                                        if (cp.CAUSA_PENAL_DELITO.Count > 0)
                                                        {
                                                            var del = cp.CAUSA_PENAL_DELITO.FirstOrDefault();
                                                            if (del != null)
                                                                obj.Delito = LstDelitosCP.Where(w => w.ID_FUERO == del.ID_FUERO && w.ID_DELITO == del.ID_DELITO).FirstOrDefault();
                                                        }
                                                    }
                                                }

                                                //OBTENEMOS LA SENTENCIA
                                                foreach (var x in ing.CAUSA_PENAL)
                                                {
                                                    foreach (var y in x.SENTENCIA)
                                                    {
                                                        anios = anios + y.ANIOS;
                                                        meses = meses + y.MESES;
                                                        dias = dias + y.DIAS;
                                                    }
                                                }
                                            }
                                            while (dias > 30)
                                            {
                                                meses++;
                                                dias = dias - 30;
                                            }
                                            while (meses > 12)
                                            {
                                                anios++;
                                                meses = meses - 12;
                                            }
                                            //CARGAMOS LOS INRSOS ANTERIORES
                                            var periodo = string.Empty;
                                            if (anios > 0)
                                                periodo = string.Format("{0} AÑOS ", anios);
                                            if (meses > 0)
                                                periodo = string.Format("{0}{1} MESES ", periodo, meses);
                                            if (dias > 0)
                                                periodo = string.Format("{0}{1} DIAS ", periodo, dias);
                                            obj.PerioroReclusion = periodo;
                                            LstIAS.Add(obj);
                                        }
                                    }
                                }
                            }
                }
                if (LstIAS.Count > 0)
                    EmptyIAS = false;
                else
                    EmptyIAS = true;
            }
            catch (Exception ex)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al establecer el ingreso anterior sistema", ex);
            }
        }

        private void AgregarIngresosAnterioresSistema()
        {
            try
            {
                if (emiActual != null)
                {
                    if (LstIAS != null)
                    {
                        if (LstIngresosAnteriores == null)
                            LstIngresosAnteriores = new ObservableCollection<EMI_INGRESO_ANTERIOR>();
                        foreach (var ias in LstIAS)
                        {
                            if (ias.Seleccione != null)
                                if (ias.Seleccione.Value)
                                {

                                    ///TODO: cambios delito
                                    LstIngresosAnteriores.Add(new EMI_INGRESO_ANTERIOR()
                                    {
                                        ID_TIPO = 2,
                                        ID_EMISOR = ias.Emisor.ID_EMISOR,
                                        PERIODO_RECLUSION = ias.PerioroReclusion,
                                        SANCIONES = ias.Sanciones,
                                        ID_DELITO = ias.Delito != null ? (long?)ias.Delito.ID_DELITO : null,
                                        ID_FUERO = ias.Delito != null ? ias.Delito.ID_FUERO : string.Empty,
                                        EMISOR = ias.Emisor,
                                        DELITO = ias.Delito != null ? ias.Delito.DESCR : string.Empty,
                                        ID_CENTRO = ias.IdCentro,
                                        ID_ANIO = ias.IdAnio,
                                        ID_IMPUTADO = ias.IdImputado,
                                        ID_INGRESO = ias.IdIngreso

                                    });
                                }
                        }
                    }
                    if (LstIngresosAnteriores != null)
                    {
                        if (LstIngresosAnteriores.Count > 0)
                            EmptyIngresosAnteriores = false;
                        else
                            EmptyIngresosAnteriores = true;
                    }
                    else
                        EmptyIngresosAnteriores = true;
                }
            }
            catch (Exception ex)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error agregar ingreso anterior sistema", ex);
            }
        }
        //MENORES
        private void AgregarIngresoAnteriorMenor()
        {
            try
            {
                if (LstIngresosAnterioresMenor == null)
                    LstIngresosAnterioresMenor = new ObservableCollection<EMI_INGRESO_ANTERIOR>();
                if (SelectedIngresoAnteriorMenor == null)//agregamos un nuevo ingreso
                {///TODO: cambios delito
                    LstIngresosAnterioresMenor.Add(new EMI_INGRESO_ANTERIOR()
                    {
                        ID_TIPO = 1,
                        ID_EMISOR = selectedEmisorIngreso.ID_EMISOR,
                        PERIODO_RECLUSION = PeriodoReclusionIngreso,
                        SANCIONES = SancionesIngreso,
                        ID_DELITO = SelectedDelitoIngreso.ID_DELITO,
                        ID_FUERO = SelectedDelitoIngreso.ID_FUERO,
                        EMISOR = SelectedEmisorIngreso,
                        DELITO = DelitoDescripcion,
                        //DELITO = SelectedDelitoIngreso
                    });
                }
                else//editamos un ingreso
                {///TODO: cambios delito
                    SelectedIngresoAnteriorMenor.ID_EMISOR = selectedEmisorIngreso.ID_EMISOR;
                    SelectedIngresoAnteriorMenor.PERIODO_RECLUSION = PeriodoReclusionIngreso;
                    SelectedIngresoAnteriorMenor.SANCIONES = SancionesIngreso;
                    SelectedIngresoAnteriorMenor.ID_DELITO = /*SelectedDelitoIngreso != null ? SelectedDelitoIngreso.ID_DELITO : */new long?();
                    SelectedIngresoAnteriorMenor.ID_FUERO = /*SelectedDelitoIngreso != null ? SelectedDelitoIngreso.ID_FUERO : */string.Empty;
                    SelectedIngresoAnteriorMenor.EMISOR = SelectedEmisorIngreso;
                    SelectedIngresoAnteriorMenor.DELITO = DelitoDescripcion;
                    //SelectedIngresoAnteriorMenor.DELITO = SelectedDelitoIngreso;
                    LstIngresosAnterioresMenor = new ObservableCollection<EMI_INGRESO_ANTERIOR>(LstIngresosAnterioresMenor);
                }
                EmptyIngresosAnterioresMenores = LstIngresosAnterioresMenor.Count > 0 ? false : true;
                //LIMPIAMOS LOS CAMPOS
                LimpiarIngresoanterior();
            }
            catch (Exception ex)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al agregar ingreso anterior menor", ex);
            }
        }

        private void EliminarIngresoAnteriorMenor()
        {
            try
            {
                if (SelectedIngresoAnteriorMenor != null)
                {
                    LstIngresosAnterioresMenor.Remove(SelectedIngresoAnteriorMenor);
                    EmptyIngresosAnterioresMenores = LstIngresosAnterioresMenor.Count > 0 ? false : true;
                }
            }
            catch (Exception ex)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al eliminar ingreso anterior menor", ex);
            }
        }

        private void PopulateIngrsoAnterioMenorPop()
        {
            try
            {
                if (SelectedIngresoAnteriorMenor != null)
                {
                    SelectedEmisorIngreso = LstEmisor.Where(w => w.ID_EMISOR == SelectedIngresoAnteriorMenor.ID_EMISOR).FirstOrDefault();
                    SelectedDelitoIngreso = LstDelitosCP.Where(w => w.ID_DELITO == SelectedIngresoAnteriorMenor.ID_DELITO && w.ID_FUERO == SelectedIngresoAnteriorMenor.ID_FUERO).FirstOrDefault();
                    PeriodoReclusionIngreso = SelectedIngresoAnteriorMenor.PERIODO_RECLUSION;
                    SancionesIngreso = SelectedIngresoAnteriorMenor.SANCIONES;
                    DelitoDescripcion = SelectedIngresoAnteriorMenor.DELITO;
                }
            }
            catch (Exception ex)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al establecer ingreso anterior menor", ex);
            }
        }

        private void GuardarIngresoAnteriorMenor()
        {
            try
            {
                if (LstIngresosAnterioresMenor != null)
                {
                    short index = 1;
                    var ingresos = new List<EMI_INGRESO_ANTERIOR>();
                    foreach (var ingreso in LstIngresosAnterioresMenor)
                    {
                        ingresos.Add(new EMI_INGRESO_ANTERIOR()
                        {
                            ID_EMI = emiActual.ID_EMI,
                            ID_EMI_CONS = emiActual.ID_EMI_CONS,
                            ID_TIPO = ingreso.ID_TIPO,
                            ID_CONSEC = index,
                            ID_EMISOR = ingreso.ID_EMISOR,
                            PERIODO_RECLUSION = ingreso.PERIODO_RECLUSION,
                            SANCIONES = ingreso.SANCIONES,
                            ID_FUERO = ingreso.ID_FUERO,
                            ID_DELITO = ingreso.ID_DELITO,
                            DELITO = ingreso.DELITO,
                            ID_CENTRO = ingreso.ID_CENTRO,
                            ID_ANIO = ingreso.ID_ANIO,
                            ID_IMPUTADO = ingreso.ID_IMPUTADO,
                            ID_INGRESO = ingreso.ID_INGRESO
                        });
                        index++;
                    }

                    if (emiActual.EMI_INGRESO_ANTERIOR.Count(w => w.ID_TIPO == 1) > 0)
                    {
                        if (!PEditar)
                        {
                            StaticSourcesViewModel.Mensaje("Validación", "Su usuario no cuenta con permisos para realizar esta acción.", StaticSourcesViewModel.enumTipoMensaje.MESNAJE_ADVERTENCIA, 5);
                            return;
                        }
                    }
                    else
                    {
                        if (!PInsertar)
                        {
                            StaticSourcesViewModel.Mensaje("Validación", "Su usuario no cuenta con permisos para realizar esta acción.", StaticSourcesViewModel.enumTipoMensaje.MESNAJE_ADVERTENCIA, 5);
                            return;
                        }
                    }

                    if (new cEMIIngresosAnteriores().Insertar(emiActual.ID_EMI, emiActual.ID_EMI_CONS, 1, ingresos))
                    {
                        FactoresSocioFamiliaresEnabled = FactoresEnabled = true;
                        LstIngresosAnterioresMenor = new ObservableCollection<EMI_INGRESO_ANTERIOR>(new cEMIIngresosAnteriores().Obtener(emiActual.ID_EMI, emiActual.ID_EMI_CONS, 1));
                        Mensaje(true, "Ingresos Anteriores Centro Menores");
                    }
                    else
                        Mensaje(false, "Ingresos Anteriores Centro Menores");

                }
            }
            catch (Exception ex)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al guardar ingreso anterior menor", ex);
            }
        }

        private void LimpiarIngresoanterior()
        {
            try
            {
                SelectedEmisorIngreso = null;
                SelectedDelitoIngreso = null;
                PeriodoReclusionIngreso = SancionesIngreso = DelitoDescripcion = string.Empty;
                SelectedIngresoAnterior = null;
                SelectedIngresoAnterior = null;
            }
            catch (Exception ex)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al limpiar ingreso anterior", ex);
            }
        }

        //private void PopulateIngresosAnterioresMenores()
        //{
        //    if (emiActual != null)
        //    {
        //        LstIngresosAnterioresMenor = new ObservableCollection<EMI_INGRESO_ANTERIOR>(new cEMIIngresosAnteriores().Obtener(emiActual.ID_EMI, emiActual.ID_EMI_CONS, 1));
        //        if (LstIngresosAnterioresMenor != null)
        //        {
        //            if (LstIngresosAnterioresMenor.Count > 0)
        //                EmptyIngresosAnterioresMenores = false;
        //            else
        //                EmptyIngresosAnterioresMenores = true;
        //        }
        //        else
        //        {
        //            EmptyIngresosAnteriores = true;
        //        }
        //    }
        //    else
        //    {
        //        LstIngresosAnterioresMenor = new ObservableCollection<EMI_INGRESO_ANTERIOR>();
        //        EmptyIngresosAnterioresMenores = true;
        //    }
        //}

        private void OnBuscarIngresoMenor(string obj = "")
        {
            try
            {
                PopUpsViewModels.ShowPopUp(this, PopUpsViewModels.TipoPopUp.HUELLAS);
                var windowBusqueda = new BuscarIngresoMenorView();
                windowBusqueda.DataContext = new BuscarIngresoMenorViewModel(LstIngresosAnterioresMenor);
                windowBusqueda.KeyDown += (s, e) => {
                    try
                    {
                        if (e.Key == System.Windows.Input.Key.Escape) windowBusqueda.Close();
                    }
                    catch (Exception ex)
                    {
                        StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error en la búsqueda del ingreso menor", ex);
                    }
                };
                windowBusqueda.Closed += (s, e) =>
                {
                    try
                    {
                        PopUpsViewModels.ClosePopUp(PopUpsViewModels.TipoPopUp.HUELLAS);

                        if (!((BuscarIngresoMenorViewModel)windowBusqueda.DataContext).IsSucceed)
                            return;

                        if (((BuscarIngresoMenorViewModel)windowBusqueda.DataContext).LstIngAntMen != null)
                        {
                            var anteriores = ((BuscarIngresoMenorViewModel)windowBusqueda.DataContext).LstIngAntMen;
                            foreach (var anterior in anteriores)
                            {
                                if (LstIngresosAnterioresMenor == null)
                                    LstIngresosAnterioresMenor = new ObservableCollection<EMI_INGRESO_ANTERIOR>();
                                LstIngresosAnterioresMenor.Add(new EMI_INGRESO_ANTERIOR()
                                {
                                    ID_TIPO = 1,
                                    ID_EMISOR = anterior.ID_EMISOR,
                                    PERIODO_RECLUSION = anterior.PERIODO_RECLUSION,
                                    SANCIONES = anterior.SANCIONES,
                                    ID_FUERO = anterior.ID_FUERO,
                                    ID_DELITO = anterior.ID_DELITO,
                                    ID_CENTRO = anterior.ID_CENTRO,
                                    ID_ANIO = anterior.ID_ANIO,
                                    ID_IMPUTADO = anterior.ID_IMPUTADO,
                                    ID_INGRESO = anterior.ID_INGRESO,
                                    DELITO = anterior.DELITO,
                                    EMISOR = anterior.EMISOR,
                                });
                            }
                            EmptyIngresosAnterioresMenores = LstIngresosAnterioresMenor.Count > 0 ? false : true;

                        }
                        //    LstIngresosAnterioresMenor = new ObservableCollection<EMI_INGRESO_ANTERIOR>(((BuscarIngresoMenorViewModel)windowBusqueda.DataContext).LstIngAntMen);
                        //else
                        //    LstIngresosAnterioresMenor = new ObservableCollection<EMI_INGRESO_ANTERIOR>();
                    }
                    catch (Exception ex)
                    {
                        StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error en al cerrar búsqueda", ex);
                    }
                };
                windowBusqueda.ShowDialog();
            }
            catch (Exception ex)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error en la búsqueda del ingreso menor", ex);
            }
        }

        //LOAD
        private void FichaIdentificacionLoad(FichaIdentificacion Window = null)
        {
            try
            {
                if (TabFichaIdentificacion)
                {
                    IndexMenu = 1;
                    SituacionJuridicaEnabled = EstudioTrasladoEnabled = true;
                    //LlenarCombosFichaIdentificacion();
                    //if (SelectIngreso != null)
                    setValidacionesFichaIdentificacion();
                }
            }
            catch (Exception ex)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al cargar ficha identificación", ex);
            }
        }

        private void EstudioTrasladoLoad(EstudioTraslado Window = null)
        {
            try
            {
                if (TabSituacionJuridicaSelected)//LA OPCION ESTA SELECCIONADA
                {
                    if (TabEstudioTrasladoSelected)
                    {
                        if (SelectIngreso != null)
                        {
                            IngresoAnteriorEnabled = true;
                            setValidacionesEstudiosTraslado();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al cargar estudio traslado", ex);
            }
        }

        private void IngresoAnteriorLoad(IngresoAnteriorCereso Window = null)
        {
            try
            {
                if (TabSituacionJuridicaSelected)
                {
                    if (TabIngresoAnteriorSelected)//LA OPCION ESTA SELECCIONADA
                    {
                        IngresoAnteriorMenorEnabled = true;
                        setValidacionesIngresosAnteriores();
                    }
                }
            }
            catch (Exception ex)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al cargar ingreso anterior", ex);
            }
        }

        private void IngresoAnteriorMenorLoad(IngresoAnteriorCeresoMenor Window = null)
        {
            try
            {
                if (TabSituacionJuridicaSelected)//LA OPCION ESTA SELECCIONADA
                {
                    if (TabIngresoAnteriorMenorSelected)//LA OPCION ESTA SELECCIONADA
                    {
                        FactoresSocioFamiliaresEnabled = FactoresEnabled = true;
                        setValidacionesIngresosAnteriores();
                    }
                }
            }
            catch (Exception ex)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al cargar ingreso anterior menor", ex);
            }
        }

        private void SituacionJuridicaLoad(SituacionJuridica Window = null)
        {
            try
            {
                if (TabSituacionJuridicaSelected)
                {

                }
            }
            catch (Exception ex)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al cargar situación jurídica", ex);
            }
        }


        //UNLOAD
        private void FichaIdentificacionUnload(FichaIdentificacion Window = null)
        {
            try
            {
                if (SelectIngreso != null)
                {
                    if (!base.HasErrors)
                    {
                        //GuardarUltimosEmpleos();
                        GuardarFichaIdentificacion();
                    }
                    else
                        TabFichaIdentificacion = true;
                }
            }
            catch (Exception ex)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al salir de ficha identificación", ex);
            }
        }

        private void EstudioTrasladoUnload(EstudioTraslado Window = null)
        {
            try
            {
                // if (TabSituacionJuridicaSelected && SelectIngreso != null)
                if (SelectIngreso != null)
                {
                    if (!base.HasErrors)
                    {
                        //GuardarSituacionJuridica();
                        GuardarEstudiosTraslados();
                    }
                    else
                    {
                        TabSituacionJuridicaSelected = true;
                        TabEstudioTrasladoSelected = true;
                    }
                }
            }
            catch (Exception ex)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al salir del estudio traslado", ex);
            }
        }

        private void IngresoAnteriorUnload(IngresoAnteriorCereso Window = null)
        {
            try
            {
                if (!base.HasErrors)
                { 
                   GuardarIngresoAnterior(); 
                }
            }
            catch (Exception ex)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al salir del ingreso anterior", ex);
            }
        }

        private void IngresoAnteriorMenorUnload(IngresoAnteriorCeresoMenor Window = null)
        {
            try
            {
                if (!base.HasErrors)
                { 
                    GuardarIngresoAnteriorMenor(); 
                }
            }
            catch (Exception ex)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al salir del ingreso anterior menor", ex);
            }
        }

        private void SituacionJuridicaUnload(SituacionJuridica Window = null)
        {
            try
            {
                //switch (IndexMenu)+
                //{
                //    case 2:
                //        TabSituacionJuridicaSelected = TabEstudioTrasladoSelected = true;
                //        break;
                //    case 3:
                //        TabSituacionJuridicaSelected= TabIngresoAnteriorSelected = true;
                //        break;
                //    case 4:
                //        TabSituacionJuridicaSelected = TabIngresoAnteriorMenorSelected = true;
                //        break;
                //}
            }
            catch (Exception ex)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al salir de situación jurídica", ex);
            }
        }

        private void FactoresSocioFamiliaresUnload(FactoresSocioFamiliares Window = null)
        {
            try
            {

            }
            catch (Exception ex)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al salir de factores socio familiares", ex);
            }
        }
        
    }
}
