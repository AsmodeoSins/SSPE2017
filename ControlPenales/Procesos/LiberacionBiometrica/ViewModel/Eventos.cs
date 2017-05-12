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
using SSP.Servidor;
using System.Windows.Controls;

namespace ControlPenales
{
    partial class LiberacionBiometricaViewModel
    {
        #region General
        public ICommand OnLoading
        {
            get { return new DelegateCommand<LiberacionBiometricaView>(OnLoad); }
        }

        private ICommand _onClick;
        public ICommand OnClick
        {
            get
            {
                return _onClick ?? (_onClick = new RelayCommand(clickSwitch));
            }
        }
        #endregion

        #region Foto
        public ICommand CaptureImage
        {
            get { return new DelegateCommand<Image>(OnTakePicture); }
        }
        #endregion

        #region Grid
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
                            //if (SeguirCargandoIngresos)
                            //    await ObtenerTodosActivos(Pagina);
                        }
                }));
            }
        }
        #endregion
    }
}
