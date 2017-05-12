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
    partial class ProgramasLibertadViewModel 
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
        #endregion

        #region Load
        public ICommand OnLoaded
        {
            get { return new DelegateCommand<ProgramasLibertadView>(OnLoad); }
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
                                //ListExpediente.InsertRange(await SegmentarResultadoBusqueda(Pagina));
                                lstLiberados.InsertRange(await SegmentarResultadoBusquedaLiberados(Pagina));
                        }
                }));
            }
        }
        #endregion

        #region [Huellas Digitales]
        private ICommand _Open442;
        public ICommand Open442
        {
            get { return _Open442 ?? (_Open442 = new RelayCommand(ShowIdentification)); }
        }

        public ICommand OnClickOk
        {
            get { return new DelegateCommand<Image>(OkClick); }
        }

        public ICommand BuscarHuella
        {
            get { return new DelegateCommand<string>(OnBuscarPorHuella); }
        }
        #endregion
    }
}
