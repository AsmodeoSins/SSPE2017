using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;

namespace ControlPenales
{
    class RegistroHistoricoViewModel : ValidationViewModelBase, IPageViewModel
    {
        #region constructor
        public RegistroHistoricoViewModel()
        {
            TabVisible = false;
            DatosIngresoVisible = true;
            IngresosVisible = false;
            DatosCausaPenalVisible = false;
            AgregarCausaPenalVisible = false;
            DiscrecionalVisible = true;
            HeaderRegistro = "Registro Historico";
        }
        #endregion

        #region variables
        private string headerRegistro;
        public string HeaderRegistro
        {
            get { return headerRegistro; }
            set { headerRegistro = value; OnPropertyChanged("HeaderRegistro"); }
        }
        private bool bandera_buscar = false;
        private bool discrecionalVisible;
        public bool DiscrecionalVisible
        {
            get { return discrecionalVisible; }
            set { discrecionalVisible = value; OnPropertyChanged("DiscrecionalVisible"); }
        }
        private bool agregarCausaPenalVisible;
        public bool AgregarCausaPenalVisible
        {
            get { return agregarCausaPenalVisible; }
            set { agregarCausaPenalVisible = value; OnPropertyChanged("AgregarCausaPenalVisible"); }
        }
        private bool datosCausaPenalVisible;
        public bool DatosCausaPenalVisible
        {
            get { return datosCausaPenalVisible; }
            set { datosCausaPenalVisible = value; OnPropertyChanged("DatosCausaPenalVisible"); }
        }
        private bool popupBuscarDelitoVisible;
        public bool PopupBuscarDelitoVisible
        {
            get { return popupBuscarDelitoVisible; }
            set { popupBuscarDelitoVisible = value; OnPropertyChanged("PopupBuscarDelitoVisible"); }
        }
        private bool datosIngresoVisible;
        public bool DatosIngresoVisible
        {
            get { return datosIngresoVisible; }
            set { datosIngresoVisible = value; OnPropertyChanged("DatosIngresoVisible"); }
        }
        private bool ingresosVisible;
        public bool IngresosVisible
        {
            get { return ingresosVisible; }
            set { ingresosVisible = value; OnPropertyChanged("IngresosVisible"); }
        }
        private bool discrecionVisible;
        public bool DiscrecionVisible
        {
            get { return discrecionVisible; }
            set { discrecionVisible = value; OnPropertyChanged("DiscrecionVisible"); }
        }
        private bool tabVisible;
        public bool TabVisible
        {
            get { return tabVisible; }
            set { tabVisible = value; OnPropertyChanged("TabVisible"); }
        }
        public string Name
        {
            get
            {
                return "registro_historico";
            }
        }
        #endregion

        #region metodos
        void IPageViewModel.inicializa()
        {

        }

        private void clickSwitch(Object obj)
        {
            switch (obj.ToString())
            {

                case "buscar_visible":
                    PopUpsViewModels.ShowPopUp(this, PopUpsViewModels.TipoPopUp.BUSQUEDA);
                    TabVisible = false;
                    DiscrecionVisible = false;
                    AgregarCausaPenalVisible = false;
                    break;
                case "buscar_salir":
                    if (bandera_buscar)
                    {
                        TabVisible = false;
                        DiscrecionVisible = true;
                        AgregarCausaPenalVisible = false;
                    }
                    else
                    {
                        TabVisible = false;
                        DiscrecionVisible = false;
                        AgregarCausaPenalVisible = false;
                    }
                        PopUpsViewModels.ClosePopUp(PopUpsViewModels.TipoPopUp.BUSQUEDA);
                    break;
                case "buscar_seleccionar":
                    if (bandera_buscar)
                    {
                        TabVisible = false;
                        DiscrecionVisible = true;
                        AgregarCausaPenalVisible = false;
                    }
                    else
                    {
                        TabVisible = true;
                        DiscrecionVisible = false;
                        AgregarCausaPenalVisible = true;
                    }
                        PopUpsViewModels.ClosePopUp(PopUpsViewModels.TipoPopUp.BUSQUEDA);
                    break;
                case "agregar_causa_penal":
                    TabVisible = false;
                    DiscrecionVisible = true;
                    AgregarCausaPenalVisible = false;
                    bandera_buscar = true;
                    PopUpsViewModels.ClosePopUp(PopUpsViewModels.TipoPopUp.BUSQUEDA);
                    break;
                case "ingresos_discrecionales":
                    TabVisible = false;
                    DiscrecionVisible = true;
                    DatosIngresoVisible = false;
                    IngresosVisible = true;
                    AgregarCausaPenalVisible = false;
                    //bandera_buscar = true;
                    //CHECAR SI YA SE GUARDO LA CAUSA PENAL CON ingreso, documentacion, ubicacion y compurgacion
                    //SI YA LOS TIENE MOSTRAR EL  IngresosVisible; SI NO, MOSTRAR MENSAJE DE QUE HACEN FALTA GUARDAR DATOS
                    PopUpsViewModels.ClosePopUp(PopUpsViewModels.TipoPopUp.BUSQUEDA);
                    break;
                case "seleccionar_delito_buscar":
                    PopupBuscarDelitoVisible = false;
                    break;
                case "cancelar_seleccionar_delito":
                    PopupBuscarDelitoVisible = false;
                    break;
                case "insertar_delito":
                    PopupBuscarDelitoVisible = true;
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
        #endregion
    }
}
