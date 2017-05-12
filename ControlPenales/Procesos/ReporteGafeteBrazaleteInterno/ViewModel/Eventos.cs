using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Controls;
using System.Windows.Input;

namespace ControlPenales
{
    public partial class ReporteBrazaleteGafeteViewModel
    {
        public ICommand WindowLoading
        {
            get { return new DelegateCommand<ReporteBrazaleteGafete>(OnLoad); }
        }

        private ICommand _onClick;
        public ICommand OnClick
        {
            get
            {
                return _onClick ?? (_onClick = new RelayCommand(ClickSwitch));
            }
        }


        private ICommand _enterClick;
        public ICommand EnterClick
        {
            get
            {
                return _enterClick ?? (_enterClick = new RelayCommand(ClickEnter));
            }
        }


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
                            if (SeguirCargandoIngresos)
                                await StaticSourcesViewModel.CargarDatosMetodoAsync(() =>
                                {
                                    ObtenerIngresos();
                                });
                        }
                }));
            }
        }
    }
}
