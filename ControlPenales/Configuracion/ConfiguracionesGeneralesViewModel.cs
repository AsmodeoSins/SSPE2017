using MahApps.Metro.Controls.Dialogs;
using System.Threading.Tasks;
using System.Windows.Input;
namespace ControlPenales
{
    public class ConfiguracionesGeneralesViewModel : ValidationViewModelBase
    {
        public ConfiguracionesGeneralesViewModel() { ConfiguracionGuardar = new Parametro(); }

        #region [Propiedades]
        private bool isLoad = true;
        private Parametro ConfiguracionGuardar { get; set; }

        private bool _GuardarHuellaEnBusquedaRegistro;
        public bool GuardarHuellaEnBusquedaRegistro
        {
            get { return _GuardarHuellaEnBusquedaRegistro; }
            set
            {
                _GuardarHuellaEnBusquedaRegistro = value;
                OnPropertyValidateChanged("GuardarHuellaEnBusquedaRegistro");
                //ConfiguracionGuardar.GuardarHuellaEnBusquedaRegistro = value;
            }
        }
        private bool _GuardarHuellaEnBusquedaJuridico;
        public bool GuardarHuellaEnBusquedaJuridico
        {
            get { return _GuardarHuellaEnBusquedaJuridico; }
            set
            {
                _GuardarHuellaEnBusquedaJuridico = value;
                OnPropertyValidateChanged("GuardarHuellaEnBusquedaJuridico");
                //ConfiguracionGuardar.GuardarHuellaEnBusquedaJuridico = value;
            }
        }
        private bool _GuardarHuellaEnBusquedaEstatusAdministrativo;
        public bool GuardarHuellaEnBusquedaEstatusAdministrativo
        {
            get { return _GuardarHuellaEnBusquedaEstatusAdministrativo; }
            set
            {
                _GuardarHuellaEnBusquedaEstatusAdministrativo = value;
                OnPropertyValidateChanged("GuardarHuellaEnBusquedaEstatusAdministrativo");
                //ConfiguracionGuardar.GuardarHuellaEnBusquedaEstatusAdministrativo = value;
            }
        }
        private bool _GuardarHuellaEnBusquedaPadronVisita;
        public bool GuardarHuellaEnBusquedaPadronVisita
        {
            get { return _GuardarHuellaEnBusquedaPadronVisita; }
            set
            {
                _GuardarHuellaEnBusquedaPadronVisita = value;
                OnPropertyValidateChanged("GuardarHuellaEnBusquedaPadronVisita");
                //ConfiguracionGuardar.GuardarHuellaEnBusquedaPadronVisita = value;
            }
        }
        #endregion

        #region [Comandos]
        private ICommand _WindowLoaded;
        public ICommand WindowLoaded
        {
            get
            {
                return _WindowLoaded ?? (_WindowLoaded = new RelayCommand(OnWindowLoad));
            }
        }

        private ICommand _CommandAceptar;
        public ICommand CommandAceptar
        {
            get
            {
                return _CommandAceptar ?? (_CommandAceptar = new RelayCommand(OnAcept));
            }
        }

        private ICommand _CommandCancelar;
        public ICommand CommandCancelar
        {
            get
            {
                return _CommandCancelar ?? (_CommandCancelar = new RelayCommand(OnCancel));
            }
        }
        #endregion

        #region [Metodos]
        private void OnWindowLoad(object obj)
        {
            GuardarHuellaEnBusquedaRegistro = Parametro.GuardarHuellaEnBusquedaRegistro;
            GuardarHuellaEnBusquedaJuridico = Parametro.GuardarHuellaEnBusquedaJuridico;
            GuardarHuellaEnBusquedaEstatusAdministrativo = Parametro.GuardarHuellaEnBusquedaEstatusAdministrativo;
            GuardarHuellaEnBusquedaPadronVisita = Parametro.GuardarHuellaEnBusquedaPadronVisita;

            StaticSourcesViewModel.SourceChanged = false;
            isLoad = false;
        }

        private async void OnCancel(object obj)
        {
            if (StaticSourcesViewModel.SourceChanged)
            {
                var result = await ((ConfiguracionesGeneralesView)obj).ShowMessageAsync("Advertencia", "Hay cambios sin guardar, ¿Seguro que desea salir sin guardar?", MessageDialogStyle.AffirmativeAndNegative, new MetroDialogSettings()
                {
                    AffirmativeButtonText = "Si",
                    NegativeButtonText = "No"
                });
                if (result == MessageDialogResult.Affirmative)
                    StaticSourcesViewModel.SourceChanged = false;
                else
                    return;
            }
            ((ConfiguracionesGeneralesView)obj).Close();
        }

        private async void OnAcept(object obj)
        {
            //if (StaticSourcesViewModel.SourceChanged)
            //    if (Configuracion.GuardarCambios(ConfiguracionGuardar))
            //    {
            //        await ((ConfiguracionesGeneralesView)obj).ShowMessageAsync("Configuración Actualizada", "Cambios guardados exitosamente", MessageDialogStyle.Affirmative, new MetroDialogSettings()
            //        {
            //            AffirmativeButtonText = "Cerrar"
            //        });
            //        StaticSourcesViewModel.SourceChanged = false;
            //    }
            //    else
            //    {
            //        await ((ConfiguracionesGeneralesView)obj).ShowMessageAsync("Error en configuración", "Hubo un problema al guardar los cambios", MessageDialogStyle.Affirmative, new MetroDialogSettings()
            //        {
            //            AffirmativeButtonText = "Cerrar"
            //        });
            //        return;
            //    }
            ((ConfiguracionesGeneralesView)obj).Close();
        }
        #endregion
    }
}
