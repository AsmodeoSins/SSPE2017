using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Input;

namespace ControlPenales
{
    public partial class ActividadesNoProgramadasViewModel
    {
        /// <summary>
        /// Tipo: Comando
        /// Delega al método "CargarVentana" la responsabilidad de manejar la acción posterior al evento "Loaded".
        /// Para más detalles, refierase al método delegado como guía.
        /// </summary>
        public ICommand WindowLoading
        {
            get { return new DelegateCommand<ActividadesNoProgramadasView>(CargarVentana); }
        }

        /// <summary>
        /// Tipo: Comando
        /// Delega al método "ClickSwitch" la responsabilidad de manejar la acción posterior al evento "MouseLeftButtonDown"
        /// Para más detalles, refierase al método delegado como guía.
        /// </summary>
        private ICommand _onClick;
        public ICommand OnClick
        {
            get
            {
                return _onClick ?? (_onClick = new RelayCommand(ClickSwitch));
            }
        }

        

    }
}
