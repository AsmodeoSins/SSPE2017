using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace ControlPenales
{
    partial class CreacionGruposViewModel
    {
        /* [descripcion de clase]
         * clase que define todo los eventos que se usan en el modulo creacion de grupos
         * 
         * comando para la carga del modulo: CreacionProgramasLoading
         * comando para la accion click: OnClick
         * 
         */

        #region [Commands]
        private ICommand onClick;
        public ICommand OnClick
        {
            get
            {
                return onClick ?? (onClick = new RelayCommand(clickSwitch));
            }
        }

        //LOADS
        public ICommand CreacionProgramasLoading
        {
            get { return new DelegateCommand<CreacionGruposView>(CreacionGruposLoad); }
        }
        private ICommand _Checked;
        public ICommand Checked
        {
            get
            {
                return _Checked ?? (_Checked = new DelegateCommand<object>((SelectedItem) => { SetIsSelectedProperty(SelectedItem, true); }));
            }
        }

        private ICommand _Unchecked;
        public ICommand Unchecked
        {
            get
            {
                return _Unchecked ?? (_Unchecked = new DelegateCommand<object>((SelectedItem) => { SetIsSelectedProperty(SelectedItem, false); }));
            }
        }
        private ICommand _CheckedCompl;
        public ICommand CheckedCompl
        {
            get
            {
                return _CheckedCompl ?? (_CheckedCompl = new DelegateCommand<object>((SelectedItem) => { SetIsSelectedPropertyCompl(SelectedItem, true); }));
            }
        }

        private ICommand _UncheckedCompl;
        public ICommand UncheckedCompl
        {
            get
            {
                return _UncheckedCompl ?? (_UncheckedCompl = new DelegateCommand<object>((SelectedItem) => { SetIsSelectedPropertyCompl(SelectedItem, false); }));
            }
        }

        private ICommand _UncheckedRevision;
        public ICommand UncheckedRevision
        {
            get
            {
                return _UncheckedRevision ?? (_UncheckedRevision = new DelegateCommand<object>(RevisarListadoRevision));
            }
        }

        private ICommand _UncheckedRevisionCompl;
        public ICommand UncheckedRevisionCompl
        {
            get
            {
                return _UncheckedRevisionCompl ?? (_UncheckedRevisionCompl = new DelegateCommand<object>(RevisarListadoRevisionCompl));
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
                            if (SeguirCargando)
                                ListGrupoParticipanteCompl.InsertRange(await SegmentarResultadoParticipantes(Pagina));
                        }
                }));
            }
        }
        #endregion
    }
}
