using ControlPenales;

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Windows.Input;

namespace ControlPenales
{
    class InternoViewModel : ValidationViewModelBase,IPageViewModel
    {
        public delegate void ParameterChange(string parameter);
        public ParameterChange OnParameterChange { get; set; }

        public string Name
        {
            get
            {
                return "interno";
            }
        }

        private bool buscarVisible;
        public bool BuscarVisible
        {
            get { return buscarVisible; }
            set { buscarVisible = value; OnPropertyChanged("BuscarVisible"); }
        }

        private bool expedienteVisible;
        public bool ExpedienteVisible
        {
            get { return expedienteVisible; }
            set { expedienteVisible = value; OnPropertyChanged("ExpedienteVisible"); }
        }

        /*MENU LATERAL EXPEDIENTE*/
        private bool identificacionGBVisible;

        public bool IdentificacionGBVisible
        {
            get { return identificacionGBVisible; }
            set { identificacionGBVisible = value; OnPropertyChanged("IdentificacionGBVisible"); }
        }

        private bool juridicoGBVisible;

        public bool JuridicoGBVisible
        {
            get { return juridicoGBVisible; }
            set { juridicoGBVisible = value; OnPropertyChanged("JuridicoGBVisible"); }
        }

        private bool administrativoGBVisible;

        public bool AdministrativoGBVisible
        {
            get { return administrativoGBVisible; }
            set { administrativoGBVisible = value; OnPropertyChanged("AdministrativoGBVisible"); }
        }

        private bool estudioGBVisible;

        public bool EstudioGBVisible
        {
            get { return estudioGBVisible; }
            set { estudioGBVisible = value; OnPropertyChanged("EstudioGBVisible"); }
        }

        private bool vistasGBVisible;

        public bool VistasGBVisible
        {
            get { return vistasGBVisible; }
            set { vistasGBVisible = value; OnPropertyChanged("VistasGBVisible"); }
        }

        private bool agendaGBVisible;

        public bool AgendaGBVisible
        {
            get { return agendaGBVisible; }
            set { agendaGBVisible = value; OnPropertyChanged("AgendaGBVisible"); }
        }

        private DateTime fecha;

        public DateTime Fecha
        {
            get { return fecha; }
            set { fecha = value; FechaLetra = Fechas.fechaLetra(fecha); OnPropertyChanged("Fecha"); }
        }

        private string fechaLetra;

        public string FechaLetra
        {
            get { return fechaLetra; }
            set { fechaLetra = value; OnPropertyChanged("FechaLetra"); }
        }

        private bool expedienteFisicoGBVisible;

        public bool ExpedienteFisicoGBVisible
        {
            get { return expedienteFisicoGBVisible; }
            set { expedienteFisicoGBVisible = value; OnPropertyChanged("ExpedienteFisicoGBVisible"); }
        }

        public InternoViewModel()
        {

        }


        #region metodos
        void IPageViewModel.inicializa()
        {
            BuscarVisible = true;
            ExpedienteVisible = !BuscarVisible;

            /*MENU LATERAL EXPEDIENTE*/
            IdentificacionGBVisible = true;
            JuridicoGBVisible = !IdentificacionGBVisible;
            AdministrativoGBVisible = !IdentificacionGBVisible;
            EstudioGBVisible = !IdentificacionGBVisible;
            VistasGBVisible = !IdentificacionGBVisible;
            AgendaGBVisible = !IdentificacionGBVisible;
            ExpedienteFisicoGBVisible = !IdentificacionGBVisible;

            Fecha = Fechas.GetFechaDateServer;
        }
        
        private void clickSwitch(Object obj)
        {
            switch (obj.ToString())
            {
                case "menu_imputado_definir":
                    break;
                case "menu_imputado_ejecutar":
                    break;
                case "menu_imputado_reporte":
                    break;
                case "menu_imputado_inicio":
                    break;
                case "menu_imputado_atras":
                    break;
                case "menu_imputado_siguiente":
                    break;
                case "menu_imputado_final":
                    break;
                case "menu_imputado_ayuda":
                    break;
                case "menu_imputado_salir":
                    OnParameterChange("salir");
                    break;
                //PANTALLA DE BUSQUEDA
                case "buscar_seleccionar":
                    BuscarVisible = false;
                    ExpedienteVisible = !BuscarVisible;
                    /*MENU LATERAL EXPEDIENTE*/
                    IdentificacionGBVisible = true;
                    JuridicoGBVisible = !IdentificacionGBVisible;
                    AdministrativoGBVisible = !IdentificacionGBVisible;
                    EstudioGBVisible = !IdentificacionGBVisible;
                    VistasGBVisible = !IdentificacionGBVisible;
                    AgendaGBVisible = !IdentificacionGBVisible;
                    ExpedienteFisicoGBVisible = !IdentificacionGBVisible;
                    break;
                case "buscar_salir":
                    OnParameterChange("salir");
                    break;
                //MENU LATERAL PANTALLA EXPEDIENTE
                case "menu_lateral_identificacion":
                    /*MENU LATERAL EXPEDIENTE*/
                    IdentificacionGBVisible = true;
                    JuridicoGBVisible = !IdentificacionGBVisible;
                    AdministrativoGBVisible = !IdentificacionGBVisible;
                    EstudioGBVisible = !IdentificacionGBVisible;
                    VistasGBVisible = !IdentificacionGBVisible;
                    AgendaGBVisible = !IdentificacionGBVisible;
                    ExpedienteFisicoGBVisible = !IdentificacionGBVisible;
                    break;
                case "menu_lateral_informacion_juridica":
                    /*MENU LATERAL EXPEDIENTE*/
                    JuridicoGBVisible = true;
                    IdentificacionGBVisible = !JuridicoGBVisible;
                    AdministrativoGBVisible = !JuridicoGBVisible;
                    EstudioGBVisible = !JuridicoGBVisible;
                    VistasGBVisible = !JuridicoGBVisible;
                    AgendaGBVisible = !JuridicoGBVisible;
                    ExpedienteFisicoGBVisible = !JuridicoGBVisible;
                    break;
                case "menu_lateral_informacion_administrativa":
                    /*MENU LATERAL EXPEDIENTE*/
                    AdministrativoGBVisible = true;
                    IdentificacionGBVisible = !AdministrativoGBVisible;
                    JuridicoGBVisible = !AdministrativoGBVisible;
                    EstudioGBVisible = !AdministrativoGBVisible;
                    VistasGBVisible = !AdministrativoGBVisible;
                    AgendaGBVisible = !AdministrativoGBVisible;
                    ExpedienteFisicoGBVisible = !AdministrativoGBVisible;
                    break;
                case "menu_lateral_estudios":
                    /*MENU LATERAL EXPEDIENTE*/
                    EstudioGBVisible = true;
                    IdentificacionGBVisible = !EstudioGBVisible;
                    JuridicoGBVisible = !EstudioGBVisible;
                    AdministrativoGBVisible = !EstudioGBVisible;
                    VistasGBVisible = !EstudioGBVisible;
                    AgendaGBVisible = !EstudioGBVisible;
                    ExpedienteFisicoGBVisible = !EstudioGBVisible;
                    break;
                case "menu_lateral_visitas":
                    /*MENU LATERAL EXPEDIENTE*/
                    VistasGBVisible = true;
                    IdentificacionGBVisible = !VistasGBVisible;
                    JuridicoGBVisible = !VistasGBVisible;
                    AdministrativoGBVisible = !VistasGBVisible;
                    EstudioGBVisible = !VistasGBVisible;
                    AgendaGBVisible = !VistasGBVisible;
                    ExpedienteFisicoGBVisible = !VistasGBVisible;
                    break;
                case "menu_lateral_agenda":
                    /*MENU LATERAL EXPEDIENTE*/
                    AgendaGBVisible = true;
                    IdentificacionGBVisible = !AgendaGBVisible;
                    JuridicoGBVisible = !AgendaGBVisible;
                    AdministrativoGBVisible = !AgendaGBVisible;
                    EstudioGBVisible = !AgendaGBVisible;
                    VistasGBVisible = !AgendaGBVisible;
                    ExpedienteFisicoGBVisible = !AgendaGBVisible;
                    break;

                case "menu_lateral_expediente_fisico":
                    /*MENU LATERAL EXPEDIENTE*/
                    ExpedienteFisicoGBVisible = true;
                    IdentificacionGBVisible = !ExpedienteFisicoGBVisible;
                    JuridicoGBVisible = !ExpedienteFisicoGBVisible;
                    AdministrativoGBVisible = !ExpedienteFisicoGBVisible;
                    EstudioGBVisible = !ExpedienteFisicoGBVisible;
                    VistasGBVisible = !ExpedienteFisicoGBVisible;
                    AgendaGBVisible = !ExpedienteFisicoGBVisible;
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
