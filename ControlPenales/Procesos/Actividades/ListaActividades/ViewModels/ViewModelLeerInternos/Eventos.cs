using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Input;

namespace ControlPenales
{
    public partial class LeerInternosViewModel
    {
        #region Commands
        /// <summary>
        /// Tipo: Comando
        /// Delega al método "OnLoad" la responsabilidad de manejar la acción posterior al evento "Loaded".
        /// Para más detalles, refierase al método delegado como guía.
        /// </summary>
        public ICommand WindowLoading
        {
            get { return new DelegateCommand<LeerInternos>(OnLoad); }
        }

        /// <summary>
        /// Tipo: Comando
        /// Delega al método "MouseEnterSwitch" la responsabilidad de manejar la acción posterior al colocar el cursor sobre un botón que
        /// tenga vinculado una acción de acuerdo al evento "MouseEnter".
        /// Para más detalles, refierase al método delegado como guía.
        /// </summary>
        private ICommand buttonMouseEnter;
        public ICommand ButtonMouseEnter
        {
            get { return buttonMouseEnter ?? (buttonMouseEnter = new RelayCommand(MouseEnterSwitch)); }
        }

        /// <summary>
        /// Tipo: Comando
        /// Delega al método "MouseLeaveSwitch" la responsabilidad de manejar la acción posterior al colocar el cursor sobre un botón que
        /// tenga vinculado una acción de acuerdo al evento "MouseLeave".
        /// Para más detalles, refierase al método delegado como guía.
        /// </summary>
        private ICommand buttonMouseLeave;
        public ICommand ButtonMouseLeave
        {
            get { return buttonMouseLeave ?? (buttonMouseLeave = new RelayCommand(MouseLeaveSwitch)); }
        }

        /// <summary>
        /// Tipo: Comando
        /// Delega al método "ClickSwitch" la responsabilidad de manejar la acción posterior al evento "MouseLeftButtonDown" y al evento
        /// ocasionado sobre un botón.
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

        /// <summary>
        /// Tipo: Comando
        /// Delega al método "SeleccionHuella" la responsabilidad de manejar la acción posterior al evento "SelectionChanged".
        /// Para más detalles, refierase al método delegado como guía.
        /// </summary>
        private ICommand busquedaHuellas;
        public ICommand BusquedaHuellas
        {
            get { return busquedaHuellas ?? (busquedaHuellas = new RelayCommand(SeleccionHuella)); }
        }

        private ICommand buscarNIPEnter;
        public ICommand BuscarNIPEnter
        {
            get
            {
                return buscarNIPEnter ?? (buscarNIPEnter = new RelayCommand(EnterKeyPressedID));
            }
        }
        #endregion
    }
}
