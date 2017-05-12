using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Input;

namespace ControlPenales
{
    public class SeguimientoLiberadosDocumentoGeneralViewModel : ValidationViewModelBase, IPageViewModel
    {
        public delegate void ParameterChange(string parameter);
        public ParameterChange OnParameterChange { get; set; }

        private List<documentos> documentos;

        public List<documentos> Documentos
        {
            get { return documentos; }
            set { documentos = value; OnPropertyChanged("Documentos"); }
        }
            

        #region variables
        public string Name
        {
            get
            {
                return "seguimiento_liberados_documentos";
            }
        }

        
        #endregion

        public SeguimientoLiberadosDocumentoGeneralViewModel() { }

        #region metodos

        
        void IPageViewModel.inicializa()
        {
            Documentos = new List<documentos>();
            Documentos.Add(new documentos { asesor = "LIC.JUAN ESCUTIA", asunto = "REVISION DE CASO", dependencia = "SSP", documento = "DOCUMENTO 1", fecha = "26/02/2015", fuero = "COMUN", no_oficio = "0001", sentenciado="PEREZ LOPEZ JUAN" });
            Documentos.Add(new documentos { asesor = "LIC.JUAN ESCUTIA", asunto = "REVISION DE CASO", dependencia = "SSP", documento = "DOCUMENTO 2", fecha = "26/02/2015", fuero = "COMUN", no_oficio = "0002", sentenciado="PEREZ LOPEZ JUAN" });
            Documentos.Add(new documentos { asesor = "LIC.JUAN ESCUTIA", asunto = "REVISION DE CASO", dependencia = "SSP", documento = "DOCUMENTO 3", fecha = "26/02/2015", fuero = "COMUN", no_oficio = "0003", sentenciado="HERNANDEZ MONTANA ANTONIO" });
            Documentos.Add(new documentos { asesor = "LIC.JUAN ESCUTIA", asunto = "REVISION DE CASO", dependencia = "SSP", documento = "DOCUMENTO 4", fecha = "26/02/2015", fuero = "COMUN", no_oficio = "0004", sentenciado = "HERNANDEZ MONTANA ANTONIO" });
      
        }
        
        private void clickSwitch(Object obj)
        {
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
