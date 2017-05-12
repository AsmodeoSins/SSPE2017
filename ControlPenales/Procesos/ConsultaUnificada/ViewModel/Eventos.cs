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
using MahApps.Metro.Controls.Dialogs;
using System.Windows.Controls;
using SSP.Servidor;

namespace ControlPenales
{
    partial class ConsultaUnificadaViewModel
    {
        #region Click
        private ICommand onClick;
        public ICommand OnClick
        {
            get
            {
                return onClick ?? (onClick = new RelayCommand(clickSwitch));
            }
        }

        private ICommand buscarClick;
        public ICommand BuscarClick
        {
            get
            {
                return buscarClick ?? (buscarClick = new RelayCommand(ClickEnter));
            }
        }

        private ICommand _onClickSelect;
        public ICommand OnClickSelect
        {
            get
            {
                return _onClickSelect ?? (_onClickSelect = new RelayCommand(clickSwitch));
            }
        }
        #endregion

        #region Load
        public ICommand OnLoaded
        {
            get { return new DelegateCommand<ConsultaUnificadaView>(OnLoad); }
        }
        #endregion
        
        #region Scroll
        private ICommand _CargarMasResultados;
        public ICommand CargarMasResultados
        {
            get
            {
                return _CargarMasResultados ?? (_CargarMasResultados = new RelayCommand(async (e) =>
                {
                    if (((ScrollChangedEventArgs)e).VerticalOffset != 0 && ((((ScrollChangedEventArgs)e).ExtentHeight - ((ScrollChangedEventArgs)e).ViewportHeight)) != 0)
                        if (((ScrollChangedEventArgs)e).VerticalOffset == (((ScrollChangedEventArgs)e).ExtentHeight - ((ScrollChangedEventArgs)e).ViewportHeight))
                        {
                            if (SeguirCargando)
                                LstConsultaUnificada.InsertRange(await SegmentarResultadoBusqueda(Pagina));
                        }
                }));
            }
        }
        #endregion
    }
}
