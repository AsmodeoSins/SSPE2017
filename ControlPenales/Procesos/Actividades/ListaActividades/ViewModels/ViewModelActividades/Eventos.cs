using System.Windows.Input;
namespace ControlPenales
{
    public partial class ActividadesViewModel
    {

        # region Copyright Quadro – 2016
        //
        // Todos los derechos reservados. La reproducción o trasmisión en su
        // totalidad o en parte, en cualquier forma o medio electrónico, mecánico
        // o similar es prohibida sin autorización expresa y por escrito del
        // propietario de este código.
        //
        // Archivo: Eventos.cs
        //
        #endregion

        #region Commands
        /// <summary>
        /// Tipo: Comando
        /// Delega al método "DoubleClick" la responsabilidad de manejar la acción posterior al evento "MouseDoubleClick".
        /// Para más detalles, refierase al método delegado como guía.
        /// </summary>
        private ICommand mouseDoubleClickCommand;
        public ICommand MouseDoubleClickCommand
        {
            get
            {
                return mouseDoubleClickCommand ?? (mouseDoubleClickCommand = new RelayCommand(DoubleClick));
            }
        }

        /// <summary>
        /// Tipo: Comando
        /// Delega al método "ClickSwitch" la responsabilidad de manejar la acción posterior al evento "MouseLeftButtonDown" y al evento
        /// ocasionado sobre un botón (En este caso, tambien se maneja el evento de presionar la tecla "Enter", únicamente para la búsqueda de internos).
        /// Para más detalles, refierase al método delegado como guía.
        /// </summary>
        private ICommand onClick;
        public ICommand OnClick
        {
            get
            {
                return onClick ?? (onClick = new RelayCommand(ClickSwitch));
            }
        }

        /// <summary>
        /// Tipo: Comando
        /// Delega al método "CargarVentana" la responsabilidad de manejar la acción posterior al evento "Loaded".
        /// Para más detalles, refierase al método delegado como guía.
        /// </summary>
        public ICommand WindowLoading
        {
            get { return new DelegateCommand<ActividadesView>(CargarVentana); }
        }

        /// <summary>
        /// Tipo: Comando
        /// Delega al método "CargarActividadesPorFecha" la responsabilidad de manejar la acción posterior al evento "SelectionChanged".
        /// Para más detalles, refierase al método delegado como guía.
        /// </summary>
        public ICommand WindowLoadingPorFecha
        {
            get { return new DelegateCommand<ActividadesView>(CargarActividadesPorFecha); }
        }
        #endregion
    }
}
