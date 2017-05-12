using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SSP.Servidor;
using System.Collections.ObjectModel;
using System.Windows;

namespace ControlPenales
{
    partial class ConfiguracionDepartamentoAreaTecnicaViewModel
    {
        #region Variables privadas
        private short selectedAreaValue;
        private IQueryable<PROCESO_USUARIO> permisos;
        #endregion

        #region Variables Manejo Controles

        private bool menuInsertarEnabled = false;
        public bool MenuInsertarEnabled
        {
            get { return menuInsertarEnabled; }
            set { menuInsertarEnabled = value; RaisePropertyChanged("MenuInsertarEnabled"); }
        }

        private bool menuEliminarEnabled = false;
        public bool MenuEliminarEnabled
        {
            get { return menuEliminarEnabled; }
            set { menuEliminarEnabled = value; RaisePropertyChanged("MenuEliminarEnabled"); }
        }

        #endregion

        #region Variables para Visibilidad de Controles
        private Visibility isListadoConfiguracionVisible=Visibility.Collapsed;
        public Visibility IsListadoConfiguracionVisible
        {
            get { return isListadoConfiguracionVisible; }
            set { isListadoConfiguracionVisible = value; RaisePropertyChanged("IsListadoConfiguracionVisible"); }
        }

        private Visibility isAgregarConfiguracionVisible = Visibility.Collapsed;
        public Visibility IsAgregarConfiguracionVisible
        {
            get { return isAgregarConfiguracionVisible; }
            set { isAgregarConfiguracionVisible = value; RaisePropertyChanged("IsAgregarConfiguracionVisible"); }
        }
        #endregion

        #region Configuracion
        private ObservableCollection<AREA_TECNICA> lstAreas;
        public ObservableCollection<AREA_TECNICA> LstAreas
        {
            get { return lstAreas; }
            set { lstAreas = value; RaisePropertyChanged("LstAreas"); }
        }

        private AREA_TECNICA selectedArea;
        public AREA_TECNICA SelectedArea
        {
            get { return selectedArea; }
            set { selectedArea = value; RaisePropertyChanged("SelectedArea"); }
        }

        private string areaSeleccionada = string.Empty;
        public string AreaSeleccionada
        {
            get { return areaSeleccionada; }
            set { areaSeleccionada = value; RaisePropertyChanged("AreaSeleccionada"); }
        }

        private ObservableCollection<DEPARTAMENTO_AREA_TECNICA> lstDepartamentoAreaTecnica;
        public ObservableCollection<DEPARTAMENTO_AREA_TECNICA> LstDepartamentoAreaTecnica
        {
            get { return lstDepartamentoAreaTecnica; }
            set { lstDepartamentoAreaTecnica = value; RaisePropertyChanged("LstDepartamentoAreaTecnica"); }
        }

        private DEPARTAMENTO_AREA_TECNICA selectedDepartamentoAreaTecnica;
        public DEPARTAMENTO_AREA_TECNICA SelectedDepartamentoAreaTecnica
        {
            get { return selectedDepartamentoAreaTecnica; }
            set { selectedDepartamentoAreaTecnica = value; RaisePropertyChanged("SelectedDepartamentoAreaTecnica"); }
        }

        #region Agregar Configuracion

        private ObservableCollection<DEPARTAMENTO> lstDepartamento;
        public ObservableCollection<DEPARTAMENTO> LstDepartamento
        {
            get { return lstDepartamento; }
            set { lstDepartamento = value; RaisePropertyChanged("LstDepartamento"); }
        }

        private short selectedDepartamentoValue = -1;
        public short SelectedDepartamentoValue
        {
            get { return selectedDepartamentoValue; }
            set { selectedDepartamentoValue = value; OnPropertyValidateChanged("SelectedDepartamentoValue"); }
        }
        #endregion
        #endregion

        #region Privilegios
        private bool pInsertar = false;
        private bool pEditar = false;
        private bool pConsultar = false;
        private bool pImprimir = false;
        #endregion
    }
}
