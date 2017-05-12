using System.Windows.Controls;
using System.Windows.Input;

namespace ControlPenales
{
    partial class EMILiberadoViewModel
    {
        #region command
        private ICommand _buscarCommand;
        public ICommand BuscarCommand
        {
            get
            {
                return _buscarCommand ?? (_buscarCommand = new RelayCommand(buscar));
            }
        }
        private ICommand _salirCommand;
        public ICommand SalirCommand
        {
            get
            {
                return _salirCommand ?? (_salirCommand = new RelayCommand(salir));
            }
        }
        private ICommand _onClick;
        public ICommand OnClick
        {
            get
            {
                return _onClick ?? (_onClick = new RelayCommand(clickSwitch));
            }
        }

        private ICommand _TabChange;
        public ICommand TabChange
        {
            get { return _TabChange ?? (_TabChange = new RelayCommand(OnTabChange)); }
        }

        private ICommand buscarClick;
        public ICommand BuscarClick
        {
            get
            {
                return buscarClick ?? (buscarClick = new RelayCommand(ClickEnter));
            }
        }

        private ICommand regionClick;
        public ICommand RegionClick
        {
            get
            {
                return regionClick ?? (regionClick = new RelayCommand(RegionSwitch));
            }
        }

        private ICommand tipoClick;
        public ICommand TipoClick
        {
            get
            {
                return tipoClick ?? (tipoClick = new RelayCommand(TipoSwitch));
            }
        }

        private ICommand buscarEmpleadoClick;
        public ICommand BuscarEmpleadoClick
        {
            get
            {
                return buscarEmpleadoClick ?? (buscarEmpleadoClick = new RelayCommand(ClickEnter));
            }
        }
        #endregion



        public ICommand OnLoaded
        {
            get { return new DelegateCommand<EMILiberadoView>(Load_Window); }
        }
        #region [LOADS]
        public ICommand FactoresLoading
        {
            get { return new DelegateCommand<Factores>(FactoresLoad); }
        }

        public ICommand EstudioTrasladoLoading
        {
            get { return new DelegateCommand<EstudioTraslado>(EstudioTrasladoLoad); }

        }

        public ICommand IngresoAnteriorLoading
        {
            get { return new DelegateCommand<IngresoAnteriorCeresoLiberado>(IngresoAnteriorLoad); }
        }

        public ICommand IngresoAnteriorMenorLoading
        {
            get { return new DelegateCommand<IngresoAnteriorCeresoMenorLiberado>(IngresoAnteriorMenorLoad); }
        }

        public ICommand FactorCriminodiagnosticoLoading
        {
            get { return new DelegateCommand<FactoresCriminodiagnosticoView>(FactorCriminodiagnosticoLoad); }
        }

        public ICommand ClasificacionCriminoloficaLoading
        {
            get { return new DelegateCommand<ClasificacionCriminologicaView>(ClasificacionCriminologicaLoad); }
        }

        public ICommand DatosGrupoFamiliarLoading
        {
            get { return new DelegateCommand<DatosGrupoFamiliarLiberado>(DatosGrupoFamiliarLoad); }
        }

        public ICommand FichaIdentificacionLoading
        {
            get { return new DelegateCommand<FichaIdentificacion>(FichaIdentificacionLoad); }
        }
        public ICommand SituacionJuridicaLoading
        {
            get { return new DelegateCommand<SituacionJuridica>(SituacionJuridicaLoad); }
        }
        public ICommand HPSLoading
        {
            get { return new DelegateCommand<HomosexualidadPandillaSexualidad>(HPSLoad); }
        }

        public ICommand TatuajesLoading
        {
            get { return new DelegateCommand<Tatuajes>(TatuajesLoad); }
        }

        public ICommand EnfermedadesLoading
        {
            get { return new DelegateCommand<Enfermedades>(EnfermedadesLoad); }
        }

        public ICommand ConductaParasocialLoading
        {
            get { return new DelegateCommand<ConductaParasocialLiberado>(ConductaParasocialLoad); }
        }

        public ICommand ClasCrimLoading
        {
            get { return new DelegateCommand<ClasCrim>(ClasCrimLoad); }
        }

        public ICommand SenaParticularLoading
        {
            get { return new DelegateCommand<TopografiaHumanaView>(SenaParticularLoad); }
        }

        public ICommand SeniasFrenteLoading
        {
            get { return new DelegateCommand<SeniasFrenteView>(SeniasFrenteLoad); }
        }

        public ICommand SeniasDorsoLoading
        {
            get { return new DelegateCommand<SeniasDorsoView>(SeniasDorsoLoad); }
        }

        public ICommand AntecedenteFamiliarLoading
        {
            get { return new DelegateCommand<AntecedentesGrupoFamilliar>(AntecedenteFamiliarLoad); }
        }

        public ICommand UsoDrogaLoading
        {
            get { return new DelegateCommand<UsoDrogas>(UsoDrogaLoad); }
        }

        public ICommand ActividadLoading
        {
            get { return new DelegateCommand<Actividades>(ActividadLoad); }
        }
        #endregion

        #region [UNLOADS]
        public ICommand EstudioTrasladoUnloading
        {
            get { return new DelegateCommand<EstudioTraslado>(EstudioTrasladoUnload); }
        }

        public ICommand IngresoAnteriorUnloading
        {
            get { return new DelegateCommand<IngresoAnteriorCeresoLiberado>(IngresoAnteriorUnload); }
        }

        public ICommand IngresoAnteriorMenorUnloading
        {
            get { return new DelegateCommand<IngresoAnteriorCeresoMenorLiberado>(IngresoAnteriorMenorUnload); }
        }

        public ICommand FactorCriminodiagnosticoUnloading
        {
            get { return new DelegateCommand<FactoresCriminodiagnosticoView>(FactorCriminodiagnosticoUnload); }
        }

        public ICommand ClasificacionCriminoloficaUnloading
        {
            get { return new DelegateCommand<ClasificacionCriminologicaView>(ClasificacionCriminologicaUnload); }
        }

        public ICommand FichaIdentificacionUnloading
        {
            get { return new DelegateCommand<FichaIdentificacion>(FichaIdentificacionUnload); }
        }
        public ICommand SituacionJuridicaUnloading
        {
            get { return new DelegateCommand<SituacionJuridicaLiberacionView>(SituacionJuridicaUnload); }
        }



        public ICommand FactoresUnloading
        {
            get { return new DelegateCommand<Factores>(FactoresUnload); }
        }

        public ICommand DatosGrupoFamiliarUnloading
        {
            get { return new DelegateCommand<DatosGrupoFamiliarLiberado>(DatosGrupoFamiliarUnload); }
        }

        public ICommand AntecedenteFamiliarUnloading
        {
            get { return new DelegateCommand<AntecedentesGrupoFamilliar>(AntecedenteFamiliarUnload); }
        }

        public ICommand UsoDrogaUnloading
        {
            get { return new DelegateCommand<UsoDrogas>(UsoDrogaUnload); }
        }

        public ICommand HPSUnloading
        {
            get { return new DelegateCommand<HomosexualidadPandillaSexualidad>(HPSUnload); }
        }

        public ICommand TatuajesUnloading
        {
            get { return new DelegateCommand<Tatuajes>(TatuajesUnload); }
        }

        public ICommand EnfermedadesUnloading
        {
            get { return new DelegateCommand<Enfermedades>(EnfermedadesUnload); }
        }

        public ICommand ActividadUnloading
        {
            get { return new DelegateCommand<Actividades>(ActividadUnload); }
        }

        public ICommand SenasParticularUnloading
        {
            get { return new DelegateCommand<TopografiaHumanaView>(SeniaParticularUnload); }
        }

        public ICommand FactoresSocioFamiliaresUnloading
        {
            get { return new DelegateCommand<FactoresSocioFamiliaresLiberado>(FactoresSocioFamiliaresUnload); }
        }

        #endregion

        #region [BUSQUEDA INGRESOS ANTERIORES MENORES]
        public ICommand BuscarIngAnt
        {
            get { return new DelegateCommand<string>(OnBuscarIngresoMenor); }
        }
        #endregion

        #region [WEBCAM]
        public ICommand CaptureImage
        {
            get { return new DelegateCommand<Image>(OnTakePicture); }
        }
        //public ICommand SaveImages
        //{
        //    get { return new DelegateCommand<string>(SaveImagesTo); }
        //}
        public ICommand CamSettings
        {
            get { return new DelegateCommand<string>(OpenSetting); }
        }
                        
        public ICommand FotoSenaParticularLoading
        {
            get { return new DelegateCommand<TomarFotoSenaParticularView>(TomarFotoLoad); }
        }
     
        #endregion

        //BUSQUEDA SEGMENTACION
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

        #region [Comandos tratamiento]
        public ICommand AgregarEje
        {
            get { return new DelegateCommand<Grid>((DynamicGrid) => { AddEje(DynamicGrid); }); }
        }
        public ICommand AgregarActividad
        {
            get { return new DelegateCommand<Grid>(AddActividad); }
        }
        public ICommand GuardarActividad
        {
            get { return new DelegateCommand<Grid>(SaveReticula); }
        }
        public ICommand LimpiarGrid
        {
            get { return new DelegateCommand<Grid>(CleanGrid); }
        }
        public ICommand CargarGrid
        {
            get { return new DelegateCommand<Grid>(LoadGrid); }
        }
        #endregion
    }
}
