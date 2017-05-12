using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Input;

namespace ControlPenales
{
    public class ExpedienteDepartamentoViewModel : ValidationViewModelBase, IPageViewModel
    {
        #region variables
        public string Name
        {
            get
            {
                return "expediente_departamento";
            }
        }
        #endregion

        #region constructor
        public ExpedienteDepartamentoViewModel()
        {
        }

        #endregion

        #region metodos

        
        void IPageViewModel.inicializa()
        { }
        
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
