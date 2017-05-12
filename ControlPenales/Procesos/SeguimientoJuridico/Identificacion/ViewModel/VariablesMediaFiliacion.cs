using SSP.Servidor;
using System.Collections.ObjectModel;

namespace ControlPenales
{
    partial class JuridicoIdentificacionViewModel
    {

        #region Listas

        #region TIPO SANGRE
        private ObservableCollection<TIPO_FILIACION> tipoSangre;
        public ObservableCollection<TIPO_FILIACION> TipoSangre
        {
            get { return tipoSangre; }
            set { tipoSangre = value; OnPropertyChanged("TipoSangre"); }
        }

        private ObservableCollection<TIPO_FILIACION> factorSangre;
        public ObservableCollection<TIPO_FILIACION> FactorSangre
        {
            get { return factorSangre; }
            set { factorSangre = value; OnPropertyChanged("FactorSangre"); }
        }
        #endregion

        #region CABELLO
        private ObservableCollection<TIPO_FILIACION> cantidadCabello;
        public ObservableCollection<TIPO_FILIACION> CantidadCabello
        {
            get { return cantidadCabello; }
            set { cantidadCabello = value; OnPropertyChanged("CantidadCabello"); }
        }

        private ObservableCollection<TIPO_FILIACION> colorCabello;
        public ObservableCollection<TIPO_FILIACION> ColorCabello
        {
            get { return colorCabello; }
            set { colorCabello = value; OnPropertyChanged("ColorCabello"); }
        }

        private ObservableCollection<TIPO_FILIACION> calvicieCabello;
        public ObservableCollection<TIPO_FILIACION> CalvicieCabello
        {
            get { return calvicieCabello; }
            set { calvicieCabello = value; OnPropertyChanged("CalvicieCabello"); }
        }

        private ObservableCollection<TIPO_FILIACION> formaCabello;
        public ObservableCollection<TIPO_FILIACION> FormaCabello
        {
            get { return formaCabello; }
            set { formaCabello = value; OnPropertyChanged("FormaCabello"); }
        }

        private ObservableCollection<TIPO_FILIACION> implantacionCabello;
        public ObservableCollection<TIPO_FILIACION> ImplantacionCabello
        {
            get { return implantacionCabello; }
            set { implantacionCabello = value; OnPropertyChanged("ImplantacionCabello"); }
        }
        #endregion

        #region CEJA
        private ObservableCollection<TIPO_FILIACION> direccionCeja;
        public ObservableCollection<TIPO_FILIACION> DireccionCeja
        {
            get { return direccionCeja; }
            set { direccionCeja = value; OnPropertyChanged("DireccionCeja"); }
        }

        private ObservableCollection<TIPO_FILIACION> implantacionCeja;
        public ObservableCollection<TIPO_FILIACION> ImplantacionCeja
        {
            get { return implantacionCeja; }
            set { implantacionCeja = value; OnPropertyChanged("ImplantacionCeja"); }
        }

        private ObservableCollection<TIPO_FILIACION> formaCeja;
        public ObservableCollection<TIPO_FILIACION> FormaCeja
        {
            get { return formaCeja; }
            set { formaCeja = value; OnPropertyChanged("FormaCeja"); }
        }

        private ObservableCollection<TIPO_FILIACION> tamanioCeja;
        public ObservableCollection<TIPO_FILIACION> TamanioCeja
        {
            get { return tamanioCeja; }
            set { tamanioCeja = value; OnPropertyChanged("TamanioCeja"); }
        }
        #endregion

        #region FRENTE
        private ObservableCollection<TIPO_FILIACION> alturaFrente;
        public ObservableCollection<TIPO_FILIACION> AlturaFrente
        {
            get { return alturaFrente; }
            set { alturaFrente = value; OnPropertyChanged("AlturaFrente"); }
        }

        private ObservableCollection<TIPO_FILIACION> inclinacionFrente;
        public ObservableCollection<TIPO_FILIACION> InclinacionFrente
        {
            get { return inclinacionFrente; }
            set { inclinacionFrente = value; OnPropertyChanged("InclinacionFrente"); }
        }

        private ObservableCollection<TIPO_FILIACION> anchoFrente;
        public ObservableCollection<TIPO_FILIACION> AnchoFrente
        {
            get { return anchoFrente; }
            set { anchoFrente = value; OnPropertyChanged("AnchoFrente"); }
        }
        #endregion

        #region OJOS
        private ObservableCollection<TIPO_FILIACION> colorOjos;
        public ObservableCollection<TIPO_FILIACION> ColorOjos
        {
            get { return colorOjos; }
            set { colorOjos = value; OnPropertyChanged("ColorOjos"); }
        }

        private ObservableCollection<TIPO_FILIACION> formaOjos;
        public ObservableCollection<TIPO_FILIACION> FormaOjos
        {
            get { return formaOjos; }
            set { formaOjos = value; OnPropertyChanged("FormaOjos"); }
        }

        private ObservableCollection<TIPO_FILIACION> tamanioOjos;
        public ObservableCollection<TIPO_FILIACION> TamanioOjos
        {
            get { return tamanioOjos; }
            set { tamanioOjos = value; OnPropertyChanged("TamanioOjos"); }
        }
        #endregion

        #region NARIZ
        private ObservableCollection<TIPO_FILIACION> raizNariz;
        public ObservableCollection<TIPO_FILIACION> RaizNariz
        {
            get { return raizNariz; }
            set { raizNariz = value; OnPropertyChanged("RaizNariz"); }
        }

        private ObservableCollection<TIPO_FILIACION> dorsoNariz;
        public ObservableCollection<TIPO_FILIACION> DorsoNariz
        {
            get { return dorsoNariz; }
            set { dorsoNariz = value; OnPropertyChanged("DorsoNariz"); }
        }

        private ObservableCollection<TIPO_FILIACION> anchoNariz;
        public ObservableCollection<TIPO_FILIACION> AnchoNariz
        {
            get { return anchoNariz; }
            set { anchoNariz = value; OnPropertyChanged("AnchoNariz"); }
        }

        private ObservableCollection<TIPO_FILIACION> baseNariz;
        public ObservableCollection<TIPO_FILIACION> BaseNariz
        {
            get { return baseNariz; }
            set { baseNariz = value; OnPropertyChanged("BaseNariz"); }
        }

        private ObservableCollection<TIPO_FILIACION> alturaNariz;
        public ObservableCollection<TIPO_FILIACION> AlturaNariz
        {
            get { return alturaNariz; }
            set { alturaNariz = value; OnPropertyChanged("AlturaNariz"); }
        }
        #endregion

        #region LABIO
        private ObservableCollection<TIPO_FILIACION> alturaLabio;
        public ObservableCollection<TIPO_FILIACION> AlturaLabio
        {
            get { return alturaLabio; }
            set { alturaLabio = value; OnPropertyChanged("AlturaLabio"); }
        }

        private ObservableCollection<TIPO_FILIACION> prominenciaLabio;
        public ObservableCollection<TIPO_FILIACION> ProminenciaLabio
        {
            get { return prominenciaLabio; }
            set { prominenciaLabio = value; OnPropertyChanged("ProminenciaLabio"); }
        }

        private ObservableCollection<TIPO_FILIACION> espesorLabio;
        public ObservableCollection<TIPO_FILIACION> EspesorLabio
        {
            get { return espesorLabio; }
            set { espesorLabio = value; OnPropertyChanged("EspesorLabio"); }
        }
        #endregion

        #region BOCA
        private ObservableCollection<TIPO_FILIACION> tamanioBoca;
        public ObservableCollection<TIPO_FILIACION> TamanioBoca
        {
            get { return tamanioBoca; }
            set { tamanioBoca = value; OnPropertyChanged("TamanioBoca"); }
        }

        private ObservableCollection<TIPO_FILIACION> comisuraBoca;
        public ObservableCollection<TIPO_FILIACION> ComisuraBoca
        {
            get { return comisuraBoca; }
            set { comisuraBoca = value; OnPropertyChanged("ComisuraBoca"); }
        }
        #endregion

        #region MENTON
        private ObservableCollection<TIPO_FILIACION> tipoMenton;
        public ObservableCollection<TIPO_FILIACION> TipoMenton
        {
            get { return tipoMenton; }
            set { tipoMenton = value; OnPropertyChanged("TipoMenton"); }
        }

        private ObservableCollection<TIPO_FILIACION> formaMenton;
        public ObservableCollection<TIPO_FILIACION> FormaMenton
        {
            get { return formaMenton; }
            set { formaMenton = value; OnPropertyChanged("FormaMenton"); }
        }

        private ObservableCollection<TIPO_FILIACION> inclinacionMenton;
        public ObservableCollection<TIPO_FILIACION> InclinacionMenton
        {
            get { return inclinacionMenton; }
            set { inclinacionMenton = value; OnPropertyChanged("InclinacionMenton"); }
        }
        #endregion

        #region OREJA
        private ObservableCollection<TIPO_FILIACION> formaOreja;
        public ObservableCollection<TIPO_FILIACION> FormaOreja
        {
            get { return formaOreja; }
            set { formaOreja = value; OnPropertyChanged("FormaOreja"); }
        }

        private ObservableCollection<TIPO_FILIACION> helixOriginalOreja;
        public ObservableCollection<TIPO_FILIACION> HelixOriginalOreja
        {
            get { return helixOriginalOreja; }
            set { helixOriginalOreja = value; OnPropertyChanged("HelixOriginalOreja"); }
        }

        private ObservableCollection<TIPO_FILIACION> helixSuperiorOreja;
        public ObservableCollection<TIPO_FILIACION> HelixSuperiorOreja
        {
            get { return helixSuperiorOreja; }
            set { helixSuperiorOreja = value; OnPropertyChanged("HelixSuperiorOreja"); }
        }

        private ObservableCollection<TIPO_FILIACION> helixPosteriorOreja;
        public ObservableCollection<TIPO_FILIACION> HelixPosteriorOreja
        {
            get { return helixPosteriorOreja; }
            set { helixPosteriorOreja = value; OnPropertyChanged("HelixPosteriorOreja"); }
        }

        private ObservableCollection<TIPO_FILIACION> helixAdherenciaOreja;
        public ObservableCollection<TIPO_FILIACION> HelixAdherenciaOreja
        {
            get { return helixAdherenciaOreja; }
            set { helixAdherenciaOreja = value; OnPropertyChanged("HelixAdherenciaOreja"); }
        }

        private ObservableCollection<TIPO_FILIACION> lobuloContornoOreja;
        public ObservableCollection<TIPO_FILIACION> LobuloContornoOreja
        {
            get { return lobuloContornoOreja; }
            set { lobuloContornoOreja = value; OnPropertyChanged("LobuloContornoOreja"); }
        }

        private ObservableCollection<TIPO_FILIACION> lobuloAdherenciaOreja;
        public ObservableCollection<TIPO_FILIACION> LobuloAdherenciaOreja
        {
            get { return lobuloAdherenciaOreja; }
            set { lobuloAdherenciaOreja = value; OnPropertyChanged("LobuloAdherenciaOreja"); }
        }

        private ObservableCollection<TIPO_FILIACION> lobuloParticularidadOreja;
        public ObservableCollection<TIPO_FILIACION> LobuloParticularidadOreja
        {
            get { return lobuloParticularidadOreja; }
            set { lobuloParticularidadOreja = value; OnPropertyChanged("LobuloParticularidadOreja"); }
        }

        private ObservableCollection<TIPO_FILIACION> lobuloDimensionOreja;
        public ObservableCollection<TIPO_FILIACION> LobuloDimensionOreja
        {
            get { return lobuloDimensionOreja; }
            set { lobuloDimensionOreja = value; OnPropertyChanged("LobuloDimensionOreja"); }
        }
        #endregion

        #region SENIAS GENERALES
        private ObservableCollection<TIPO_FILIACION> complexion;
        public ObservableCollection<TIPO_FILIACION> Complexion
        {
            get { return complexion; }
            set { complexion = value; OnPropertyChanged("Complexion"); }
        }

        private ObservableCollection<TIPO_FILIACION> colorPiel;
        public ObservableCollection<TIPO_FILIACION> ColorPiel
        {
            get { return colorPiel; }
            set { colorPiel = value; OnPropertyChanged("ColorPiel"); }
        }

        private ObservableCollection<TIPO_FILIACION> cara;
        public ObservableCollection<TIPO_FILIACION> Cara
        {
            get { return cara; }
            set { cara = value; OnPropertyChanged("Cara"); }
        }
        #endregion

        #endregion

        #region Selected

        #region SENIAS GENERALES
        private short selectComplexion;
        public short SelectComplexion
        {
            get { return selectComplexion; }
            set
            {
                selectComplexion = value;
                OnPropertyValidateChanged("SelectComplexion");
                ChecarValidaciones(); 
            }
        }
        
        private short selectColorPiel;
        public short SelectColorPiel
        {
            get { return selectColorPiel; }
            set
            {
                selectColorPiel = value;
                OnPropertyValidateChanged("SelectColorPiel");
                ChecarValidaciones(); 
            }
        }
        
        private short selectCara;
        public short SelectCara
        {
            get { return selectCara; }
            set
            {
                selectCara = value;
                OnPropertyValidateChanged("SelectCara");
                ChecarValidaciones();
            }
        }
        #endregion

        #region SANGRE
        private short selectTipoSangre;
        public short SelectTipoSangre
        {
            get { return selectTipoSangre; }
            set
            {
                selectTipoSangre = value;
                ChecarValidaciones();
                OnPropertyValidateChanged("SelectTipoSangre");
            }
        }
        
        private short selectFactorSangre;
        public short SelectFactorSangre
        {
            get { return selectFactorSangre; }
            set
            {
                selectFactorSangre = value;
                ChecarValidaciones();
                OnPropertyValidateChanged("SelectFactorSangre");
            }
        }        
        #endregion 
        
        #region CABELLO
        private short selectCantidadCabello;
        public short SelectCantidadCabello
        {
            get { return selectCantidadCabello; }
            set
            {
                selectCantidadCabello = value;
                ChecarValidaciones();
                OnPropertyValidateChanged("SelectCantidadCabello");
            }
        }
        
        private short selectColorCabello;
        public short SelectColorCabello
        {
            get { return selectColorCabello; }
            set
            {
                selectColorCabello = value;
                ChecarValidaciones();
                OnPropertyValidateChanged("SelectColorCabello");
            }
        }
        
        private short selectFormaCabello;
        public short SelectFormaCabello
        {
            get { return selectFormaCabello; }
            set
            {
                selectFormaCabello = value;
                ChecarValidaciones();
                OnPropertyValidateChanged("SelectFormaCabello");
            }
        }
        
        private short selectCalvicieCabello;
        public short SelectCalvicieCabello
        {
            get { return selectCalvicieCabello; }
            set
            {
                selectCalvicieCabello = value;
                ChecarValidaciones();
                OnPropertyValidateChanged("SelectCalvicieCabello");
            }
        }
        
        private short selectImplantacionCabello;
        public short SelectImplantacionCabello
        {
            get { return selectImplantacionCabello; }
            set
            {
                selectImplantacionCabello = value;
                ChecarValidaciones();
                OnPropertyValidateChanged("SelectImplantacionCabello");
            }
        }
        #endregion

        #region FRENTE
        private short selectAlturaFrente;
        public short SelectAlturaFrente
        {
            get { return selectAlturaFrente; }
            set
            {
                selectAlturaFrente = value;
                ChecarValidaciones();
                OnPropertyValidateChanged("SelectAlturaFrente");
            }
        }
        
        private short selectInclinacionFrente;
        public short SelectInclinacionFrente
        {
            get { return selectInclinacionFrente; }
            set
            {
                selectInclinacionFrente = value;
                ChecarValidaciones();
                OnPropertyValidateChanged("SelectInclinacionFrente");
            }
        }
        
        private short selectAnchoFrente;
        public short SelectAnchoFrente
        {
            get { return selectAnchoFrente; }
            set
            {
                selectAnchoFrente = value;
                ChecarValidaciones();
                OnPropertyValidateChanged("SelectAnchoFrente");
            }
        }
        #endregion

        #region CEJAS
        private short selectDireccionCeja;
        public short SelectDireccionCeja
        {
            get { return selectDireccionCeja; }
            set
            {
                selectDireccionCeja = value;
                ChecarValidaciones();
                OnPropertyValidateChanged("SelectDireccionCeja");
            }
        }
        
        private short selectImplantacionCeja;
        public short SelectImplantacionCeja
        {
            get { return selectImplantacionCeja; }
            set
            {
                selectImplantacionCeja = value;
                ChecarValidaciones();
                OnPropertyValidateChanged("SelectImplantacionCeja");
            }
        }
        
        private short selectFormaCeja;
        public short SelectFormaCeja
        {
            get { return selectFormaCeja; }
            set
            {
                selectFormaCeja = value;
                ChecarValidaciones();
                OnPropertyValidateChanged("SelectFormaCeja");
            }
        }
        
        private short selectTamanioCeja;
        public short SelectTamanioCeja
        {
            get { return selectTamanioCeja; }
            set
            {
                selectTamanioCeja = value;
                ChecarValidaciones();
                OnPropertyValidateChanged("SelectTamanioCeja");
            }
        }
        #endregion

        #region OJOS
        private short selectColorOjos;
        public short SelectColorOjos
        {
            get { return selectColorOjos; }
            set
            {
                selectColorOjos = value;
                ChecarValidaciones();
                OnPropertyValidateChanged("SelectColorOjos");
            }
        }
        
        private short selectFormaOjos;
        public short SelectFormaOjos
        {
            get { return selectFormaOjos; }
            set
            {
                selectFormaOjos = value;
                ChecarValidaciones();
                OnPropertyValidateChanged("SelectFormaOjos");
            }
        }
        
        private short selectTamanioOjos;
        public short SelectTamanioOjos
        {
            get { return selectTamanioOjos; }
            set
            {
                selectTamanioOjos = value;
                ChecarValidaciones();
                OnPropertyValidateChanged("SelectTamanioOjos");
            }
        }
        #endregion

        #region NARIZ
        private short selectRaizNariz;
        public short SelectRaizNariz
        {
            get { return selectRaizNariz; }
            set
            {
                selectRaizNariz = value;
                ChecarValidaciones();
                OnPropertyValidateChanged("SelectRaizNariz");
            }
        }
        
        private short selectDorsoNariz;
        public short SelectDorsoNariz
        {
            get { return selectDorsoNariz; }
            set
            {
                selectDorsoNariz = value;
                ChecarValidaciones();
                OnPropertyValidateChanged("SelectDorsoNariz");
            }
        }
        
        private short selectAnchoNariz;
        public short SelectAnchoNariz
        {
            get { return selectAnchoNariz; }
            set
            {
                selectAnchoNariz = value;
                ChecarValidaciones();
                OnPropertyValidateChanged("SelectAnchoNariz");
            }
        }
        
        private short selectBaseNariz;
        public short SelectBaseNariz
        {
            get { return selectBaseNariz; }
            set
            {
                selectBaseNariz = value;
                ChecarValidaciones();
                OnPropertyValidateChanged("SelectBaseNariz");
            }
        }
        
        private short selectAlturaNariz;
        public short SelectAlturaNariz
        {
            get { return selectAlturaNariz; }
            set
            {
                selectAlturaNariz = value;
                ChecarValidaciones();
                OnPropertyValidateChanged("SelectAlturaNariz");
            }
        }
        #endregion

        #region LABIOS
        private short selectEspesorLabio;
        public short SelectEspesorLabio
        {
            get { return selectEspesorLabio; }
            set
            {
                selectEspesorLabio = value;
                ChecarValidaciones();
                OnPropertyValidateChanged("SelectEspesorLabio");
            }
        }
        
        private short selectAlturaLabio;
        public short SelectAlturaLabio
        {
            get { return selectAlturaLabio; }
            set
            {
                selectAlturaLabio = value;
                ChecarValidaciones();
                OnPropertyValidateChanged("SelectAlturaLabio");
            }
        }
        
        private short selectProminenciaLabio;
        public short SelectProminenciaLabio
        {
            get { return selectProminenciaLabio; }
            set
            {
                selectProminenciaLabio = value;
                ChecarValidaciones();
                OnPropertyValidateChanged("SelectProminenciaLabio");
            }
        }
        #endregion

        #region BOCA
        private short selectTamanioBoca;
        public short SelectTamanioBoca
        {
            get { return selectTamanioBoca; }
            set
            {
                selectTamanioBoca = value;
                ChecarValidaciones();
                OnPropertyValidateChanged("SelectTamanioBoca");
            }
        }
        
        private short selectComisuraBoca;
        public short SelectComisuraBoca
        {
            get { return selectComisuraBoca; }
            set
            {
                selectComisuraBoca = value;
                ChecarValidaciones();
                OnPropertyValidateChanged("SelectComisuraBoca");
            }
        }
        #endregion

        #region MENTON
        private short selectFormaMenton;
        public short SelectFormaMenton
        {
            get { return selectFormaMenton; }
            set
            {
                selectFormaMenton = value;
                ChecarValidaciones();
                OnPropertyValidateChanged("SelectFormaMenton");
            }
        }
        
        private short selectTipoMenton;
        public short SelectTipoMenton
        {
            get { return selectTipoMenton; }
            set
            {
                selectTipoMenton = value;
                ChecarValidaciones();
                OnPropertyValidateChanged("SelectTipoMenton");
            }
        }
        
        private short selectInclinacionMenton;
        public short SelectInclinacionMenton
        {
            get { return selectInclinacionMenton; }
            set
            {
                selectInclinacionMenton = value;
                ChecarValidaciones();
                OnPropertyValidateChanged("SelectInclinacionMenton");
            }
        }
        #endregion

        #region OREJA DERECHA
        private short selectFormaOrejaDerecha;
        public short SelectFormaOrejaDerecha
        {
            get { return selectFormaOrejaDerecha; }
            set
            {
                selectFormaOrejaDerecha = value;
                ChecarValidaciones();
                OnPropertyValidateChanged("SelectFormaOrejaDerecha");
            }
        }

        #region HELIX DERECHA
        private short selectHelixOriginalOrejaDerecha;
        public short SelectHelixOriginalOrejaDerecha
        {
            get { return selectHelixOriginalOrejaDerecha; }
            set
            {
                selectHelixOriginalOrejaDerecha = value;
                ChecarValidaciones();
                OnPropertyValidateChanged("SelectHelixOriginalOrejaDerecha");
            }
        }
        
        private short selectHelixSuperiorOrejaDerecha;
        public short SelectHelixSuperiorOrejaDerecha
        {
            get { return selectHelixSuperiorOrejaDerecha; }
            set
            {
                selectHelixSuperiorOrejaDerecha = value;
                ChecarValidaciones();
                OnPropertyValidateChanged("SelectHelixSuperiorOrejaDerecha");
            }
        }
        
        private short selectHelixPosteriorOrejaDerecha;
        public short SelectHelixPosteriorOrejaDerecha
        {
            get { return selectHelixPosteriorOrejaDerecha; }
            set
            {
                selectHelixPosteriorOrejaDerecha = value;
                ChecarValidaciones();
                OnPropertyValidateChanged("SelectHelixPosteriorOrejaDerecha");
            }
        }
        
        private short selectHelixAdherenciaOrejaDerecha;
        public short SelectHelixAdherenciaOrejaDerecha
        {
            get { return selectHelixAdherenciaOrejaDerecha; }
            set
            {
                selectHelixAdherenciaOrejaDerecha = value;
                ChecarValidaciones();
                OnPropertyValidateChanged("SelectHelixAdherenciaOrejaDerecha");
            }
        }
        #endregion

        #region LOBULO DERECHO
        private short selectLobuloContornoOrejaDerecha;
        public short SelectLobuloContornoOrejaDerecha
        {
            get { return selectLobuloContornoOrejaDerecha; }
            set
            {
                selectLobuloContornoOrejaDerecha = value;
                OnPropertyValidateChanged("SelectLobuloContornoOrejaDerecha");
                ChecarValidaciones();
            }
        }
        
        private short selectLobuloAdherenciaOrejaDerecha;
        public short SelectLobuloAdherenciaOrejaDerecha
        {
            get { return selectLobuloAdherenciaOrejaDerecha; }
            set
            {
                selectLobuloAdherenciaOrejaDerecha = value;
                OnPropertyValidateChanged("SelectLobuloAdherenciaOrejaDerecha");
                ChecarValidaciones();
            }
        }
        
        private short selectLobuloParticularidadOrejaDerecha;
        public short SelectLobuloParticularidadOrejaDerecha
        {
            get { return selectLobuloParticularidadOrejaDerecha; }
            set
            {
                selectLobuloParticularidadOrejaDerecha = value;
                OnPropertyValidateChanged("SelectLobuloParticularidadOrejaDerecha");
                ChecarValidaciones();
            }
        }
        
        private short selectLobuloDimensionOrejaDerecha;
        public short SelectLobuloDimensionOrejaDerecha
        {
            get { return selectLobuloDimensionOrejaDerecha; }
            set
            {
                selectLobuloDimensionOrejaDerecha = value;
                OnPropertyValidateChanged("SelectLobuloDimensionOrejaDerecha");
                ChecarValidaciones();
            }
        }

        //OREJA IZQUIERDA
        //private short selectFormaOrejaIzquierda;
        //public short SelectFormaOrejaIzquierda
        //{
        //    get { return selectFormaOrejaIzquierda; }
        //    set { selectFormaOrejaIzquierda = value; OnPropertyChanged("SelectFormaOrejaIzquierda"); }
        //}
        ////HELIX IZQUIERDA
        //private short selectHelixOriginalOrejaIzquierda;
        //public short SelectHelixOriginalOrejaIzquierda
        //{
        //    get { return selectHelixOriginalOrejaIzquierda; }
        //    set { selectHelixOriginalOrejaIzquierda = value; OnPropertyChanged("SelectHelixOriginalOrejaIzquierda"); }
        //}
        //private short selectHelixSuperiorOrejaIzquierda;
        //public short SelectHelixSuperiorOrejaIzquierda
        //{
        //    get { return selectHelixSuperiorOrejaIzquierda; }
        //    set { selectHelixSuperiorOrejaIzquierda = value; OnPropertyChanged("SelectHelixSuperiorOrejaIzquierda"); }
        //}
        //private short selectHelixPosteriorOrejaIzquierda;
        //public short SelectHelixPosteriorOrejaIzquierda
        //{
        //    get { return selectHelixPosteriorOrejaIzquierda; }
        //    set { selectHelixPosteriorOrejaIzquierda = value; OnPropertyChanged("SelectHelixPosteriorOrejaIzquierda"); }
        //}
        //private short selectHelixAdherenciaOrejaIzquierda;
        //public short SelectHelixAdherenciaOrejaIzquierda
        //{
        //    get { return selectHelixAdherenciaOrejaIzquierda; }
        //    set { selectHelixAdherenciaOrejaIzquierda = value; OnPropertyChanged("SelectHelixAdherenciaOrejaIzquierda"); }
        //}
        ////LOBULO IZQUIERDO
        //private short selectLobuloContornoOrejaIzquierda;
        //public short SelectLobuloContornoOrejaIzquierda
        //{
        //    get { return selectLobuloContornoOrejaIzquierda; }
        //    set { selectLobuloContornoOrejaIzquierda = value; OnPropertyChanged("SelectLobuloContornoOrejaIzquierda"); }
        //}
        //private short selectLobuloAdherenciaOrejaIzquierda;
        //public short SelectLobuloAdherenciaOrejaIzquierda
        //{
        //    get { return selectLobuloAdherenciaOrejaIzquierda; }
        //    set { selectLobuloAdherenciaOrejaIzquierda = value; OnPropertyChanged("SelectLobuloAdherenciaOrejaIzquierda"); }
        //}
        //private short selectLobuloParticularidadOrejaIzquierda;
        //public short SelectLobuloParticularidadOrejaIzquierda
        //{
        //    get { return selectLobuloParticularidadOrejaIzquierda; }
        //    set { selectLobuloParticularidadOrejaIzquierda = value; OnPropertyChanged("SelectLobuloParticularidadOrejaIzquierda"); }
        //}
        //private short selectLobuloDimensionOrejaIzquierda;
        //public short SelectLobuloDimensionOrejaIzquierda
        //{
        //    get { return selectLobuloDimensionOrejaIzquierda; }
        //    set { selectLobuloDimensionOrejaIzquierda = value; OnPropertyChanged("SelectLobuloDimensionOrejaIzquierda"); }
        //}
        #endregion

        #endregion

        #endregion

    }
}
