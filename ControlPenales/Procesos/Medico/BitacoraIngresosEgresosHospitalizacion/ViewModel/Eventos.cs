using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Input;

namespace ControlPenales
{
    public partial class BitacoraIngresosEgresosHospitalizacionViewModel
    {
        /// <summary>
        /// Tipo: Comando
        /// Delega al método "CargarVentana" la responsabilidad de manejar la acción posterior al evento "Loaded".
        /// Para más detalles, refierase al método delegado como guía.
        /// </summary>
        public ICommand WindowLoading
        {
            get { return new DelegateCommand<BitacoraIngresosEgresosHospitalizacionView>(OnLoad); }
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


        //private ICommand _enterClick;
        //public ICommand EnterClick
        //{
        //    get
        //    {
        //        return _enterClick ?? (_enterClick = new RelayCommand(ClickEnter));
        //    }
        //}


        public ICommand HospitalizacionesPorFecha
        {
            get { return new DelegateCommand<BitacoraIngresosEgresosHospitalizacionView>(CargarHospitalizacionesPorFecha); }
        }

        private ICommand cmdModelChanged;
        public ICommand CmdModelChanged
        {
            get { return cmdModelChanged ?? (cmdModelChanged = new RelayCommand(OnModelChangedSwitch)); }
        }

        #region Agenda
        private ICommand _ClickAgenda;
        public ICommand ClickAgenda
        {
            get
            {
                return _ClickAgenda ?? (_ClickAgenda = new RelayCommand(AgendaSwitch));
            }
        }
        #endregion
    }
}
