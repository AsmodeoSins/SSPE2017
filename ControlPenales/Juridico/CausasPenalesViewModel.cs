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
using System.Windows.Controls;

namespace ControlPenales
{
    class CausasPenalesViewModel : ValidationViewModelBase, IPageViewModel
    {
        #region constructor
        public CausasPenalesViewModel()
        {
            TabVisible = false;
            DatosIngresoVisible = true;
            IngresosVisible = false;
            DatosCausaPenalVisible = false;
            AgregarCausaPenalVisible = false;
            DiscrecionalVisible = false;
            HeaderRegistro = "Registro Discrecional";

            IngresoTab = true;
            CausaPenalTab = true;
            CoparticipeTab = true;
            SentenciaTab = true;
            RdiTab = true;
            DelitosTab = true;
            DelitoTab = true;
            RecursoTab = true;

            Ingreso2Tab = true;
            
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
                return "causas_penales";
            }
        }
        #endregion

        #region metodos
        void IPageViewModel.inicializa()
        { }
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
        private ICommand onClick;
        public ICommand OnClick
        {
            get
            {
                return onClick ?? (onClick = new RelayCommand(clickSwitch));
            }
        }

        private ICommand onClickTreeView;
        public ICommand OnClickTreeView
        {
            get
            {
                return onClickTreeView ?? (onClickTreeView = new RelayCommand(ClickTreeView));
            }
        }
        #endregion

        private void ClickTreeView(Object obj) 
        {


            var x = (TreeView)obj;
            var i = x.SelectedItem.ToString();
            i = i.Replace("System.Windows.Controls.TreeViewItem Header:", string.Empty);

            if (i.StartsWith("Ingresos"))
            {
                TabVisible = false;
                DiscrecionVisible = true;
                DatosIngresoVisible = false;
                IngresosVisible = true;
                AgregarCausaPenalVisible = false;

                IngresoTab = false;
                CausaPenalTab = false;
                CoparticipeTab = false;
                SentenciaTab = false;
                RdiTab = false;
                DelitosTab = false;
                DelitoTab = false;
                RecursoTab = false;
                PopUpsViewModels.ClosePopUp(PopUpsViewModels.TipoPopUp.BUSQUEDA);
            }
            else if(i.StartsWith("Ingreso:"))
            {
                TabVisible = false;
                DiscrecionVisible = true;
                DatosIngresoVisible = false;
                IngresosVisible = true;
                AgregarCausaPenalVisible = false;

                IngresoTab = false;
                CausaPenalTab = false;
                CoparticipeTab = false;
                SentenciaTab = false;
                RdiTab = false;
                DelitosTab = false;
                DelitoTab = false;
                RecursoTab = false;
                PopUpsViewModels.ClosePopUp(PopUpsViewModels.TipoPopUp.BUSQUEDA);
            }
            else if(i.StartsWith("2013"))
            {
                //GeneralVisible = false;
                //BusquedaVisible = false;
                //TabVisible = true;
                //DiscrecionVisible = false;
                //DatosIngresoVisible = true;
                //IngresosVisible = false;
                //AgregarCausaPenalVisible = false;
                TabVisible = false;
                DatosIngresoVisible = true;
                IngresosVisible = false;
                DatosCausaPenalVisible = false;
                AgregarCausaPenalVisible = false;
                DiscrecionalVisible = false;
            

                IngresoTab = false;
                CausaPenalTab = true;
                CoparticipeTab = true;
                SentenciaTab = true;
                RdiTab = false;
                DelitosTab = false;
                DelitoTab = false;
                RecursoTab = false;

                CausaPenal2Tab = true;
                PopUpsViewModels.ClosePopUp(PopUpsViewModels.TipoPopUp.BUSQUEDA);
            }
            else if (i.StartsWith("Delito Causa")) 
            {
                TabVisible = false;
                DiscrecionVisible = false;
                DatosIngresoVisible = true;
                IngresosVisible = false;
                AgregarCausaPenalVisible = false;

                IngresoTab = false;
                CausaPenalTab = false;
                CoparticipeTab = false;
                SentenciaTab = false;
                RdiTab = false;
                DelitosTab = true;
                DelitoTab = false;
                RecursoTab = false;

                CausaPenal2Tab = false;
                Delitos2Tab = true;
                PopUpsViewModels.ClosePopUp(PopUpsViewModels.TipoPopUp.BUSQUEDA);
            
            }
            else if (i.StartsWith("Delitos")) 
            {
                TabVisible = false;
                DiscrecionVisible = false;
                DatosIngresoVisible = true;
                IngresosVisible = false;
                AgregarCausaPenalVisible = false;

                IngresoTab = false;
                CausaPenalTab = false;
                CoparticipeTab = false;
                SentenciaTab = false;
                RdiTab = false;
                DelitosTab = true;
                DelitoTab = false;
                RecursoTab = false;

                CausaPenal2Tab = false;
                Delitos2Tab = true;
                PopUpsViewModels.ClosePopUp(PopUpsViewModels.TipoPopUp.BUSQUEDA);
            }
            else if (i.StartsWith("Partida"))
            {
                TabVisible = false;
                DiscrecionVisible = false;
                DatosIngresoVisible = true;
                IngresosVisible = false;
                AgregarCausaPenalVisible = false;

                IngresoTab = false;
                CausaPenalTab = false;
                CoparticipeTab = false;
                SentenciaTab = false;
                RdiTab = true;
                DelitosTab = false;
                DelitoTab = false;
                RecursoTab = false;

                CausaPenal2Tab = false;
                Delitos2Tab = false;
                Rdi2Tab = true;
                PopUpsViewModels.ClosePopUp(PopUpsViewModels.TipoPopUp.BUSQUEDA);
            }
            else 
            {
                TabVisible = false;
                DiscrecionVisible = false;
                DatosIngresoVisible = true;
                IngresosVisible = false;
                AgregarCausaPenalVisible = false;

                IngresoTab = false;
                CausaPenalTab = false;
                CoparticipeTab = false;
                SentenciaTab = false;
                RdiTab = false;
                DelitosTab = false;
                DelitoTab = false;
                RecursoTab = true;

                CausaPenal2Tab = false;
                Delitos2Tab = false;
                Rdi2Tab = false;
                Recurso2Tab = true;
                PopUpsViewModels.ClosePopUp(PopUpsViewModels.TipoPopUp.BUSQUEDA);
            }
        }

        private bool ingresoTab;
        public bool IngresoTab
        {
            get { return ingresoTab; }
            set { ingresoTab = value; OnPropertyChanged("IngresoTab"); }
        }

        private bool causaPenalTab;
        public bool CausaPenalTab
        {
            get { return causaPenalTab; }
            set { causaPenalTab = value; OnPropertyChanged("CausaPenalTab"); }
        }

        private bool coparticipeTab;
        public bool CoparticipeTab
        {
            get { return coparticipeTab; }
            set { coparticipeTab = value; OnPropertyChanged("CoparticipeTab"); }
        }

        private bool sentenciaTab;
        public bool SentenciaTab
        {
            get { return sentenciaTab; }
            set { sentenciaTab = value; OnPropertyChanged("SentenciaTab"); }
        }

        private bool rdiTab;
        public bool RdiTab
        {
            get { return rdiTab; }
            set { rdiTab = value; OnPropertyChanged("RdiTab"); }
        }

        private bool delitosTab;
        public bool DelitosTab
        {
            get { return delitosTab; }
            set { delitosTab = value; OnPropertyChanged("DelitosTab"); }
        }

        private bool delitoTab;
        public bool DelitoTab
        {
            get { return delitoTab; }
            set { delitoTab = value; OnPropertyChanged("DelitoTab"); }
        }

        private bool recursoTab;
        public bool RecursoTab
        {
            get { return recursoTab; }
            set { recursoTab = value; OnPropertyChanged("RecursoTab"); }
        }

        /////////////////////////////////////////
        private bool ingreso2Tab;
        public bool Ingreso2Tab
        {
            get { return ingreso2Tab; }
            set { ingreso2Tab = value; OnPropertyChanged("Ingreso2Tab"); }
        }

        private bool causaPenal2Tab;
        public bool CausaPenal2Tab
        {
            get { return causaPenal2Tab; }
            set { causaPenal2Tab = value; OnPropertyChanged("CausaPenal2Tab"); }
        }

        private bool coparticipe2Tab;
        public bool Coparticipe2Tab
        {
            get { return coparticipe2Tab; }
            set { coparticipe2Tab = value; OnPropertyChanged("Coparticipe2Tab"); }
        }

        private bool sentencia2Tab;
        public bool Sentencia2Tab
        {
            get { return sentencia2Tab; }
            set { sentencia2Tab = value; OnPropertyChanged("Sentencia2Tab"); }
        }

        private bool rdi2Tab;
        public bool Rdi2Tab
        {
            get { return rdi2Tab; }
            set { rdi2Tab = value; OnPropertyChanged("Rdi2Tab"); }
        }

        private bool delitos2Tab;
        public bool Delitos2Tab
        {
            get { return delitos2Tab; }
            set { delitos2Tab = value; OnPropertyChanged("Delitos2Tab"); }
        }

        private bool delito2Tab;
        public bool Delito2Tab
        {
            get { return delito2Tab; }
            set { delito2Tab = value; OnPropertyChanged("Delito2Tab"); }
        }

        private bool recurso2Tab;
        public bool Recurso2Tab
        {
            get { return recurso2Tab; }
            set { recurso2Tab = value; OnPropertyChanged("Recurso2Tab"); }
        }
    }
}
