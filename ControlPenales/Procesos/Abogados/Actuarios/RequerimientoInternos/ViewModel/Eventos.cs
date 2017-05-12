using SSP.Controlador.Catalogo.Justicia;
using SSP.Servidor;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using WPFPdfViewer;

namespace ControlPenales
{
    partial class RequerimientoInternosViewModel
    {
        private ICommand _HeaderClick;
        public ICommand HeaderClick
        {
            get
            {
                return _HeaderClick ?? (_HeaderClick = new RelayCommand(HeaderSort));
            }
        }
        public ICommand OnLoaded
        {
            get { return new DelegateCommand<RequerimientoInternosView>(Load_Window); }
        }
        /*public ICommand OnUnLoaded
        {
            get { return new DelegateCommand<PadronActuariosView>(UnLoad_Window); }
        }*/
        public ICommand BuscarHuella
        {
            get { return new DelegateCommand<string>(OnBuscarPorHuella); }
        }
        private ICommand _onClick;
        public ICommand OnClick
        {
            get { return _onClick ?? (_onClick = new RelayCommand(clickSwitch)); }
        }
        private ICommand _CapturaClick;
        public ICommand CapturaClick
        {
            get { return _CapturaClick ?? (_CapturaClick = new RelayCommand(EnterAbogados)); }
        }
        private ICommand _Checked;
        public ICommand Checked
        {
            get
            {
                return _Checked ?? (_Checked = new DelegateCommand<object>((SelectedItem) => { CheckBoxInternoSelecccionado(SelectedItem); }));
            }
        }
        private ICommand _EnterClick;
        public ICommand EnterClick
        {
            get { return _EnterClick ?? (_EnterClick = new RelayCommand(EnterPersonas)); }
        }
        private ICommand _CargarMasPersonas;
        public ICommand CargarMasPersonas
        {
            get
            {
                return _CargarMasPersonas ?? (_CargarMasPersonas = new RelayCommand(async (e) =>
                {
                    if (PConsultar)
                    {
                        if (((ScrollChangedEventArgs)e).VerticalOffset != 0 && ((((ScrollChangedEventArgs)e).ExtentHeight - ((ScrollChangedEventArgs)e).ViewportHeight)) != 0)
                            if (((ScrollChangedEventArgs)e).VerticalOffset == (((ScrollChangedEventArgs)e).ExtentHeight - ((ScrollChangedEventArgs)e).ViewportHeight))
                            {
                                if (SeguirCargandoPersonas)
                                {
                                    ListPersonasAuxiliar = new RangeEnabledObservableCollection<SSP.Servidor.PERSONA>();
                                    ListPersonas.InsertRange(await SegmentarPersonasBusqueda(Pagina));
                                    ListPersonasAuxiliar.InsertRange(ListPersonas);
                                }
                            }
                    }
                }));
            }
        }
    }
}