using LinqKit;
using MahApps.Metro.Controls;
using SSP.Controlador.Catalogo.Justicia;
using SSP.Controlador.Principales.Compartidos;
using SSP.Servidor;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;
using System.Windows;
using System.Windows.Threading;

namespace ControlPenales
{
    partial class AdministracionParametrosViewModel : ValidationViewModelBase
    {
        cParametro ParametroControlador = new cParametro();
        cParametroClave ParametroControladorClave = new cParametroClave();
        Usuario User = StaticSourcesViewModel.UsuarioLogin;
        EMPLEADO ObjEmpleadoCentro;
        private async void clickSwitch(object op)
        {
            switch (op.ToString())
            {
                #region [Parametros ventana Emergente]
                case "cancelar_parametro":
                    if (StaticSourcesViewModel.SourceChanged)
                    {
                        var dialogresult = await (new Dialogos()).ConfirmarEliminar("Advertencia", "Hay cambios sin guardar, ¿Seguro que desea salir sin guardar?");
                        if (dialogresult != 0)
                            StaticSourcesViewModel.SourceChanged = false;
                        else
                            return;
                    }
                    PopUpsViewModels.ClosePopUp(PopUpsViewModels.TipoPopUp.AGREGAR_PARAMETROS);
                    //   ClearValidaciones();

                    break;

                case "guardar_parametro":

                    if (base.HasErrors)
                    {
                        (new Dialogos()).ConfirmacionDialogo("Validación", "Debe de llenar todos los campo Obligatorios.");
                    }
                    else
                    {
                        if (ValidacionTipoParametros())
                        {


                            var ObjParam = new PARAMETRO();

                            ObjParam.ID_CLAVE = ValidacionString(Clave);
                            ObjParam.VALOR = ValidacionString(Valor);
                            ObjParam.VALOR_NUM = ValorNumerico;


                            ObjParam.CONTENIDO = ArchSelect != null ? ArchSelect : null;
                            using (TransactionScope transaccion = new TransactionScope(TransactionScopeOption.RequiresNew, new TransactionOptions() { IsolationLevel = System.Transactions.IsolationLevel.ReadCommitted }))
                            {
                                try
                                {
                                    if (AGREGAR_EDITAR.Equals("AGREGAR"))
                                    {
                                        if (!pInsertar)
                                        {
                                            new Dialogos().ConfirmacionDialogo("Validación", "Su usuario no tiene privilegios para realizar esta acción");
                                            break;
                                        }
                                        ObjParam.ID_CENTRO = SelectedCentro.ID_CENTRO;

                                        if (ParametroControladorClave.Insertar(new PARAMETRO_CLAVE() { ID_CLAVE = ObjParam.ID_CLAVE = Clave, DESCR = DESCR, SISTEMA = "J" }))
                                        {

                                            if (ParametroControlador.Insertar(ObjParam))
                                            {


                                                transaccion.Complete();
                                                new Dialogos().ConfirmacionDialogo("Éxito", "La información se registro correctamente");

                                            }
                                        }
                                    }
                                    else
                                    {
                                        if (AGREGAR_EDITAR.Equals("EDITAR"))
                                        {
                                            if (!pEditar)
                                            {
                                                new Dialogos().ConfirmacionDialogo("Validación", "Su usuario no tiene privilegios para realizar esta acción");
                                                break;
                                            }
                                            ObjParam.ID_CENTRO = (short)ObjEmpleadoCentro.ID_CENTRO;
                                            if (ParametroControladorClave.Editar(new PARAMETRO_CLAVE() { ID_CLAVE = ObjParam.ID_CLAVE = Clave, DESCR = DESCR, SISTEMA = "J" }))
                                            {
                                                if (ParametroControlador.Editar(ObjParam))
                                                {

                                                    transaccion.Complete();
                                                    new Dialogos().ConfirmacionDialogo("Éxito", "La información se actualizo correctamente");


                                                }
                                            }

                                        }
                                    }


                                }
                                catch (Exception ex)
                                {
                                    StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al Al Guardar la informacion", ex);

                                }
                            }
                            PopUpsViewModels.ClosePopUp(PopUpsViewModels.TipoPopUp.AGREGAR_PARAMETROS);
                            // ClearValidaciones();
                            await StaticSourcesViewModel.CargarDatosMetodoAsync(BusquedaParametro);
                        }
                        else
                        {
                            (new Dialogos()).ConfirmacionDialogo("Validación", "Debe de registrar por lo menos un tipo de parametro (Valor,Valor Numérico o Contenido).");
                        }
                    }



                    break;
                case "seleccionar_archivo":
                    SeleccionarArchivo();

                    break;
                #endregion
                case "menu_agregar":
                    if (!pInsertar)
                    {
                        new Dialogos().ConfirmacionDialogo("Validación", "Su usuario no tiene privilegios para realizar esta acción");
                        break;
                    }
                    AGREGAR_EDITAR = "AGREGAR";
                    SelectCentro = -1;
                    EnableCentro = true;
                    ClaveEnable = true;
                    LimpiarControles();
                    //   SelectTipoParametro = "SELECCIONE";
                    PopUpsViewModels.ShowPopUp(this, PopUpsViewModels.TipoPopUp.AGREGAR_PARAMETROS);
                    break;
                case "editar_parametro":
                    if (!pEditar)
                    {
                        new Dialogos().ConfirmacionDialogo("Validación", "Su usuario no tiene privilegios para realizar esta acción");
                        break;
                    }
                    AGREGAR_EDITAR = "EDITAR";
                    EnableCentro = false;
                    // ObtenerTipoParametro();
                    LimpiarControles();
                    OnPropertyChanged("SelectParametros");
                    if (SelectParametros != null)
                    {
                        SelectCentro = SelectParametros.ID_CENTRO;
                        ClaveEnable = false;
                        Clave = ValidacionString(SelectParametros.ID_CLAVE);
                        Valor = ValidacionString(SelectParametros.VALOR);
                        ValorNumerico = SelectParametros.VALOR_NUM;
                        DESCR = ValidacionString(SelectParametros.PARAMETRO_CLAVE.DESCR);
                        ArchSelect = null;
                        ArchSelect = SelectParametros.CONTENIDO;
                        PopUpsViewModels.ShowPopUp(this, PopUpsViewModels.TipoPopUp.AGREGAR_PARAMETROS);
                    }
                    else
                    {
                        new Dialogos().ConfirmacionDialogo("Validacion", "Debe de seleccionar un registro");
                    }
                    break;
                case "menu_editar":
                    break;
                case "menu_cancelar":
                    break;
                case "menu_limpiar":
                    if (StaticSourcesViewModel.SourceChanged)
                    {
                        var dialogresult = await (new Dialogos()).ConfirmarEliminar("Advertencia", "Hay cambios sin guardar, ¿Seguro que desea limpiar la pantalla sin guardar?");
                        if (dialogresult != 0)
                            StaticSourcesViewModel.SourceChanged = false;
                        else
                            return;
                    }
                    var metro = Application.Current.Windows[0] as MetroWindow;
                    ((System.Windows.Controls.ContentControl)metro.FindName("contentControl")).Content = null;
                    ((System.Windows.Controls.ContentControl)metro.FindName("contentControl")).DataContext = null;
                    GC.Collect();
                    ((System.Windows.Controls.ContentControl)PopUpsViewModels.MainWindow.FindName("contentControl")).Content = new AdministracionParametrosView();
                    ((System.Windows.Controls.ContentControl)PopUpsViewModels.MainWindow.FindName("contentControl")).DataContext = new AdministracionParametrosViewModel();
                    break;
                case "menu_ayuda":
                    break;
                case "menu_salir":
                    if (StaticSourcesViewModel.SourceChanged)
                    {
                        var dialogresult = await (new Dialogos()).ConfirmarEliminar("Advertencia", "Hay cambios sin guardar, ¿Seguro que desea salir sin guardar?");
                        if (dialogresult != 0)
                            StaticSourcesViewModel.SourceChanged = false;
                        else
                            return;
                    }
                    PrincipalViewModel.SalirMenu();
                    break;
                case "buscar_menu":
                    if (!pConsultar)
                    {
                        new Dialogos().ConfirmacionDialogo("Validación", "Su usuario no tiene privilegios para realizar esta acción");
                        break;
                    }
                    await StaticSourcesViewModel.CargarDatosMetodoAsync(BusquedaParametro);
                    break;
                case "Busqueda_Enter":
                    if (!pConsultar)
                    {
                        new Dialogos().ConfirmacionDialogo("Validación", "Su usuario no tiene privilegios para realizar esta acción");
                        break;
                    }
                    await StaticSourcesViewModel.CargarDatosMetodoAsync(BusquedaParametro);
                    break;
            }
        }

        private void ObtenerTipoParametro()
        {
            if (!string.IsNullOrEmpty(SelectParametros.VALOR))
            {
                SelectTipoParametro = "VALOR";
            }
            if (SelectParametros.VALOR_NUM != null)
            {
                SelectTipoParametro = "VALOR NUMÉRICO";
            }
            if (SelectParametros.CONTENIDO != null)
            {
                SelectTipoParametro = "CONTENIDO";
            }
        }

        private void BusquedaParametro()
        {
            LstParametros = new System.Collections.ObjectModel.ObservableCollection<PARAMETRO>();
            var ObtenerParametrosUsuario = new cParametro().Obtener(ObjEmpleadoCentro.ID_CENTRO, ClaveBuscar, ValorBuscar, ValorNumericoBuscar, DescricionBuscar);
            if (ObtenerParametrosUsuario != null)
            {
                LstParametros = new ObservableCollection<PARAMETRO>(ObtenerParametrosUsuario);
            }
            else
            {
                LstParametros = new ObservableCollection<PARAMETRO>();
            }
            // LstParametros = new System.Collections.ObjectModel.ObservableCollection<PARAMETRO>(new cParametro().Obtener(User.Centro, ClaveBuscar, ValorBuscar, ValorNumericoBuscar, DescricionBuscar));
            OnPropertyChanged("LstParametros");
        }

        private void AsignarMaxLenght()
        {
            Clave_Max = 30;
            Valor_MAX = 100;
            ValorNumerico_MAX = 10;
            Descr_Max = 100;
        }

        private String ValidacionString(string value)
        {
            return !string.IsNullOrEmpty(value) ? value.Trim() : string.Empty;
        }

        private void LimpiarControles()
        {
            Clave = string.Empty;
            Valor = string.Empty;
            ValorNumerico = null;
            DESCR = string.Empty;
            ArchSelect = null;
        }

        /// <summary>
        /// Valida que usuario seleccione un tipo de parametro para agregar
        /// </summary>
        /// <returns></returns>
        private bool ValidacionTipoParametros()
        {
            return ArchSelect != null || !string.IsNullOrEmpty(Valor) || ValorNumerico != null;
        }

        //private void ClearValidaciones()
        //{
        //    base.RemoveRule("Valor"); OnPropertyChanged("Valor"); OnPropertyChanged();
        //    base.RemoveRule("ArchSelect"); OnPropertyChanged("ArchSelect");
        //    base.RemoveRule("ValorNumerico"); OnPropertyChanged("ValorNumerico");
        //}

        private void SeleccionarArchivo()
        {
            var op = new System.Windows.Forms.OpenFileDialog();
            op.Title = "Seleccione un  documento";
            //op.Filter = "JPEG Files (*.jpeg)|*.jpeg|PNG Files (*.png)|*.png|JPG Files (*.jpg)|*.jpg";//"Formatos Validos:|*.pdf";
            if (op.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                if (new System.IO.FileInfo(op.FileName).Length > 5000000)
                    StaticSourcesViewModel.Mensaje("Arvhivo no soportada", "El archivo debe ser de menos de 5 Mb", StaticSourcesViewModel.enumTipoMensaje.MESNAJE_ADVERTENCIA, 5);
                else
                {
                    LblNombreArchivo = op.FileName;
                    ArchSelect = System.IO.File.ReadAllBytes(op.FileName);
                }
            }
        }

        //NOTA: NO BORRAR YA QUE DESPUES SE DESCOMENTARA PARA LOS PERMISOS DEUSUARIO
        //#region [PERMISOS]
        //private void ConfiguraPermisos()
        //{
        //    try
        //    {
        //        var permisos = new cProcesoUsuario().Obtener(enumProcesos.CAT_PARAMETROS.ToString(), StaticSourcesViewModel.UsuarioLogin.Username);
        //        foreach (var p in permisos)
        //        {
        //            if (p.INSERTAR == 1)
        //                AgregarMenuEnabled = true;
        //            if (p.CONSULTAR == 1)
        //            {
        //                IdEnabled = true;
        //                ValorEnabled = true;
        //                ValorNumHabilitado = true;
        //                DescrEnabled = true;
        //                BuscarEnabled = true;
        //            }
        //            if (p.EDITAR == 1)
        //                EditarEnabled = true;
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al configurar permisos en la pantalla", ex);
        //    }
        //}
        //#endregion

        #region [Load]
        private async void OnLoad(AdministracionParametrosView obj = null)
        {
            try
            {

                ObjEmpleadoCentro = new cUsuario().Obtener(GlobalVar.gUsr).EMPLEADO;
                await StaticSourcesViewModel.CargarDatosMetodoAsync(CargarListas);
                SetValidaciones();
                //NOTA: NO BORRAR, DESPUES SE DESCOMENTARA PARA LOS PERMISOS DE USUARIO
                ConfiguraPermisos();
                StaticSourcesViewModel.SourceChanged = false;//Restea Propiedades
            }
            catch (Exception ex)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al cargar decomisos", ex);
            }
        }
        #endregion

        private void CargarListas()
        {
            var ObtenerParametrosUsuario = new cParametro().Obtener(ObjEmpleadoCentro.ID_CENTRO);
            if (ObtenerParametrosUsuario != null)
            {
                LstParametros = new ObservableCollection<PARAMETRO>(ObtenerParametrosUsuario);
            }
            else
            {
                LstParametros = new ObservableCollection<PARAMETRO>();
            }
            var predicate = PredicateBuilder.True<CENTRO>();
            predicate = predicate.And(where => where.ESTATUS.Equals("S"));
            LstCentros = new System.Collections.ObjectModel.ObservableCollection<CENTRO>(new cCentro().GetData(predicate.Expand()));
            System.Windows.Application.Current.Dispatcher.Invoke((Action)(delegate
            {
 
                LstCentros.Insert(0, new CENTRO() { ID_CENTRO = -1, DESCR = "SELECCIONE" });

            }));
            AsignarMaxLenght();
        }

        #region Permisos
        private void ConfiguraPermisos()
        {
            try
            {
                var permisos = new cProcesoUsuario().Obtener(enumProcesos.CAT_PARAMETROS.ToString(), StaticSourcesViewModel.UsuarioLogin.Username);
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
            catch (Exception ex)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al configurar permisos en la pantalla", ex);
            }
        }
        #endregion

      
    }
}
