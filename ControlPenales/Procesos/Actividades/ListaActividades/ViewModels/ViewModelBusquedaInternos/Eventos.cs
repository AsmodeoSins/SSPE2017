using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Input;

namespace ControlPenales
{
    public partial class BusquedaInternoProgramasViewModel
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
        /// Delega al método "OnLoad" la responsabilidad de manejar la acción posterior al evento "Loaded".
        /// Para más detalles, refierase al método delegado como guía.
        /// </summary>
        public ICommand WindowLoading
        {
            get { return new DelegateCommand<BuscarInternosProgramas>(OnLoad); }
        }

        /// <summary>
        /// Tipo: Comando
        /// Delega al método "EnterKeyPressed" la responsabilidad de manejar la acción posterior al evento ocurrido al presionar la tecla "Enter".
        /// Para más detalles, refierase al método delegado como guía.
        /// </summary>
        private ICommand buscarEnter;
        public ICommand BuscarEnter
        {
            get
            {
                return buscarEnter ?? (buscarEnter = new RelayCommand(EnterKeyPressed));
            }
        }

        /// <summary>
        /// Tipo: Comando
        /// Delega al método "EnterKeyPressedID" la responsabilidad de manejar la acción posterior al evento ocurrido al presionar la tecla "Enter".
        /// Para más detalles, refierase al método delegado como guía.
        /// </summary>
        private ICommand buscarNIPEnter;
        public ICommand BuscarNIPEnter
        {
            get
            {
                return buscarNIPEnter ?? (buscarNIPEnter = new RelayCommand(EnterKeyPressedID));
            }
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
        #endregion
    }
}
