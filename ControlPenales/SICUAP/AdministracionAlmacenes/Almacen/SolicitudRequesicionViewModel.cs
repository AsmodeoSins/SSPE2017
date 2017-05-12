using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using System.Threading;
using System.Threading.Tasks;
using MahApps.Metro.Controls;
using MahApps.Metro.Controls.Dialogs;

namespace ControlPenales
{
    class SolicitudRequesicionViewModel : ValidationViewModelBase, IPageViewModel
    {

        #region constructor
        public SolicitudRequesicionViewModel()
        {
            AgregarVisible = false;
            BotonesNuevaVisible = false;
            NuevaDetalleVisible = false;
            ProductosSeleccionadosVisible = false;
            ProductosSeleccionadosSolosVisible = false;
            BotonesProductosSeleccionadosVisible = false;
            CantidadEditableVisible = false;
            CantidadEstaticaVisible = false;
            BotonesImprimirProductosSeleccionadosVisible = false;
            bandera = false;
        }
        #endregion

        #region variables
        public bool bandera;
        private string nuevoDetalleHeader;
        public string NuevoDetalleHeader
        {
            get { return nuevoDetalleHeader; }
            set { nuevoDetalleHeader = value; OnPropertyChanged("NuevoDetalleHeader"); }
        }
        private bool botonesImprimirProductosSeleccionadosVisible;
        public bool BotonesImprimirProductosSeleccionadosVisible
        {
            get { return botonesImprimirProductosSeleccionadosVisible; }
            set { botonesImprimirProductosSeleccionadosVisible = value; OnPropertyChanged("BotonesImprimirProductosSeleccionadosVisible"); }
        }
        private bool cantidadEstaticaVisible;
        public bool CantidadEstaticaVisible
        {
            get { return cantidadEstaticaVisible; }
            set { cantidadEstaticaVisible = value; OnPropertyChanged("CantidadEstaticaVisible"); }
        }
        private bool cantidadEditableVisible;
        public bool CantidadEditableVisible
        {
            get { return cantidadEditableVisible; }
            set { cantidadEditableVisible = value; OnPropertyChanged("CantidadEditableVisible"); }
        }
        private bool botonesProductosSeleccionadosVisible;
        public bool BotonesProductosSeleccionadosVisible
        {
            get { return botonesProductosSeleccionadosVisible; }
            set { botonesProductosSeleccionadosVisible = value; OnPropertyChanged("BotonesProductosSeleccionadosVisible"); }
        }
        private bool productosSeleccionadosSolosVisible;
        public bool ProductosSeleccionadosSolosVisible
        {
            get { return productosSeleccionadosSolosVisible; }
            set { productosSeleccionadosSolosVisible = value; OnPropertyChanged("ProductosSeleccionadosSolosVisible"); }
        }
        private bool productosSeleccionadosVisible;
        public bool ProductosSeleccionadosVisible
        {
            get { return productosSeleccionadosVisible; }
            set { productosSeleccionadosVisible = value; OnPropertyChanged("ProductosSeleccionadosVisible"); }
        }
        private bool botonesNuevaVisible;
        public bool BotonesNuevaVisible
        {
            get { return botonesNuevaVisible; }
            set { botonesNuevaVisible = value; OnPropertyChanged("BotonesNuevaVisible"); }
        }
        private bool nuevadetalleVisible;
        public bool NuevaDetalleVisible
        {
            get { return nuevadetalleVisible; }
            set { nuevadetalleVisible = value; OnPropertyChanged("NuevaDetalleVisible"); }
        }
        private bool agregarVisible;
        public bool AgregarVisible
        {
            get { return agregarVisible; }
            set { agregarVisible = value; OnPropertyChanged("AgregarVisible"); }
        }
        public string Name
        {
            get
            {
                return "solicitud_requesiciones";
            }
        }
        #endregion

        #region metodos
        void IPageViewModel.inicializa()
        { }
        private async void clickSwitch(Object obj)
        {
            switch (obj.ToString())
            {
                case "solicitud_cancelar":
                    var metro = Application.Current.Windows[0] as MetroWindow;
                    var mySettings = new MetroDialogSettings()
                    {
                        AffirmativeButtonText = "Aceptar",
                        NegativeButtonText = "Cancelar"
                    };
                    if (await metro.ShowMessageAsync("Cancelar!", "Estas seguro que deseas cancelar esta Requisicion? ", MessageDialogStyle.AffirmativeAndNegative, mySettings) == MessageDialogResult.Affirmative)
                    {
                    }
                    break;
                case "solicitud_nueva":
                    NuevaDetalleVisible = true;
                    BotonesNuevaVisible = true;
                    AgregarVisible = false;
                    ProductosSeleccionadosVisible = false;
                    ProductosSeleccionadosSolosVisible = false;
                    BotonesProductosSeleccionadosVisible = false;
                    NuevoDetalleHeader = "Nueva Requisicion";
                    BotonesImprimirProductosSeleccionadosVisible = false;
                    bandera = false;
                    break;
                case "solicitud_detalle":
                    AgregarVisible = false;
                    NuevaDetalleVisible = true;
                    BotonesNuevaVisible = false;
                    CantidadEditableVisible = false;
                    CantidadEstaticaVisible = true;
                    BotonesProductosSeleccionadosVisible = false;
                    NuevoDetalleHeader = "Detalle de Requisicion";
                    ProductosSeleccionadosSolosVisible = false;
                    ProductosSeleccionadosVisible = true;
                    BotonesImprimirProductosSeleccionadosVisible = true;
                    break;
                case "boton_guardar_nueva_solicitud":
                    if (NuevoDetalleHeader == "Editar Requisicion")
                    {
                        AgregarVisible = false;
                        BotonesImprimirProductosSeleccionadosVisible = true;
                        BotonesProductosSeleccionadosVisible = true;
                        NuevaDetalleVisible = false;
                    }
                    else
                    {
                        BotonesProductosSeleccionadosVisible = true;
                        if (bandera == false)
                        {
                            AgregarVisible = true;
                        }
                        else
                        {
                            AgregarVisible = false;
                            NuevaDetalleVisible = false;
                        }
                    }
                    break;
                case "solicitud_cancelar_nueva":
                    AgregarVisible = false;
                    NuevaDetalleVisible = false;
                    break;
                case "boton_agregar_catalogo":
                    AgregarVisible = true;
                    break;
                case "boton_cancelar_catalogo":
                    AgregarVisible = false;
                    break;
                case "boton_agregar_producto":
                    AgregarVisible = false;
                    BotonesNuevaVisible = true;
                    CantidadEditableVisible = true;
                    CantidadEstaticaVisible = false;
                    bandera = true;
                    break;
                case "boton_cancelar_producto":
                    AgregarVisible = false;
                    break;
                case "boton_regresar_solicitud":
                    NuevaDetalleVisible = false;
                    AgregarVisible = false;
                    break;
                case "boton_cancelar_nueva_solicitud":
                    BotonesImprimirProductosSeleccionadosVisible = false;
                    BotonesProductosSeleccionadosVisible = false;
                    ProductosSeleccionadosSolosVisible = false;
                    ProductosSeleccionadosVisible = false;
                    NuevaDetalleVisible = false;
                    break;
                case "boton_cancelar_productos_seleccionados":
                    BotonesImprimirProductosSeleccionadosVisible = false;
                    ProductosSeleccionadosVisible = false;
                    NuevaDetalleVisible = false;
                    break;
                case "solicitud_editar_requisicion":
                    NuevoDetalleHeader = "Editar Requisicion";
                    ProductosSeleccionadosSolosVisible = false;
                    ProductosSeleccionadosVisible = true;
                    NuevaDetalleVisible = true;
                    BotonesImprimirProductosSeleccionadosVisible = true;
                    BotonesProductosSeleccionadosVisible = true;
                    CantidadEditableVisible = true;
                    CantidadEstaticaVisible = false;
                    bandera = true;
                    break;
            }
        }
        #endregion

        #region command
        private ICommand _onClick;
        public ICommand OnClick
        {
            get
            {
                return _onClick ?? (_onClick = new RelayCommand(clickSwitch));
            }
        }
        Usuario _selectedItem = null;
        public Usuario SelectedItem
        {
            get
            {
                return _selectedItem;
            }
            set
            {
                _selectedItem = value;
            }
        }
        #endregion

    }
}
