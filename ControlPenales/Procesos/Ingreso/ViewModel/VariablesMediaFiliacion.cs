using SSP.Servidor;
using System.Collections.ObjectModel;

namespace ControlPenales
{
    partial class RegistroIngresoViewModel
    {
        private ObservableCollection<IMPUTADO_FILIACION> imputadoFiliacion;
        public ObservableCollection<IMPUTADO_FILIACION> ImputadoFiliacion
        {
            get { return imputadoFiliacion; }
            set { imputadoFiliacion = value; OnPropertyChanged("ImputadoFiliacion"); }
        }

        #region Listas
        //TIPO SANGRE
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
        //CABELLO
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
        //CEJA
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
        //FRENTE
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
        //OJOS
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
        //NARIZ
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
        //LABIO
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
        //BOCA
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
        //MENTON
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
        //OREJA
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
        //SENIAS GENERALES
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

        #region Selected
        //SENIAS GENERALES
        private short selectComplexion;
        public short SelectComplexion
        {
            get { return selectComplexion; }
            set
            {
                selectComplexion = value;
                OnPropertyChanged("SelectComplexion");
                
            }
        }
        private short selectColorPiel;
        public short SelectColorPiel
        {
            get { return selectColorPiel; }
            set
            {
                selectColorPiel = value;
                OnPropertyChanged("SelectColorPiel");
                
            }
        }
        private short selectCara;
        public short SelectCara
        {
            get { return selectCara; }
            set
            {
                selectCara = value;
                OnPropertyChanged("SelectCara");
                
            }
        }
        //SANGRE
        private short selectTipoSangre;
        public short SelectTipoSangre
        {
            get { return selectTipoSangre; }
            set
            {
                selectTipoSangre = value;
                
                OnPropertyChanged("SelectTipoSangre");
            }
        }
        private short selectFactorSangre;
        public short SelectFactorSangre
        {
            get { return selectFactorSangre; }
            set
            {
                selectFactorSangre = value;
                
                OnPropertyChanged("SelectFactorSangre");
            }
        }
        //CABELLO
        private short selectCantidadCabello;
        public short SelectCantidadCabello
        {
            get { return selectCantidadCabello; }
            set
            {
                selectCantidadCabello = value;
                
                OnPropertyChanged("SelectCantidadCabello");
            }
        }
        private short selectColorCabello;
        public short SelectColorCabello
        {
            get { return selectColorCabello; }
            set
            {
                selectColorCabello = value;
                
                OnPropertyChanged("SelectColorCabello");
            }
        }
        private short selectFormaCabello;
        public short SelectFormaCabello
        {
            get { return selectFormaCabello; }
            set
            {
                selectFormaCabello = value;
                
                OnPropertyChanged("SelectFormaCabello");
            }
        }
        private short selectCalvicieCabello;
        public short SelectCalvicieCabello
        {
            get { return selectCalvicieCabello; }
            set
            {
                selectCalvicieCabello = value;
                
                OnPropertyChanged("SelectCalvicieCabello");
            }
        }
        private short selectImplantacionCabello;
        public short SelectImplantacionCabello
        {
            get { return selectImplantacionCabello; }
            set
            {
                selectImplantacionCabello = value;
                
                OnPropertyChanged("SelectImplantacionCabello");
            }
        }
        //FRENTE
        private short selectAlturaFrente;
        public short SelectAlturaFrente
        {
            get { return selectAlturaFrente; }
            set
            {
                selectAlturaFrente = value;
                
                OnPropertyChanged("SelectAlturaFrente");
            }
        }
        private short selectInclinacionFrente;
        public short SelectInclinacionFrente
        {
            get { return selectInclinacionFrente; }
            set
            {
                selectInclinacionFrente = value;
                
                OnPropertyChanged("SelectInclinacionFrente");
            }
        }
        private short selectAnchoFrente;
        public short SelectAnchoFrente
        {
            get { return selectAnchoFrente; }
            set
            {
                selectAnchoFrente = value;
                
                OnPropertyChanged("SelectAnchoFrente");
            }
        }
        //CEJAS
        private short selectDireccionCeja;
        public short SelectDireccionCeja
        {
            get { return selectDireccionCeja; }
            set
            {
                selectDireccionCeja = value;
                
                OnPropertyChanged("SelectDireccionCeja");
            }
        }
        private short selectImplantacionCeja;
        public short SelectImplantacionCeja
        {
            get { return selectImplantacionCeja; }
            set
            {
                selectImplantacionCeja = value;
                
                OnPropertyChanged("SelectImplantacionCeja");
            }
        }
        private short selectFormaCeja;
        public short SelectFormaCeja
        {
            get { return selectFormaCeja; }
            set
            {
                selectFormaCeja = value;
                
                OnPropertyChanged("SelectFormaCeja");
            }
        }
        private short selectTamanioCeja;
        public short SelectTamanioCeja
        {
            get { return selectTamanioCeja; }
            set
            {
                selectTamanioCeja = value;
                
                OnPropertyChanged("SelectTamanioCeja");
            }
        }
        //OJOS
        private short selectColorOjos;
        public short SelectColorOjos
        {
            get { return selectColorOjos; }
            set
            {
                selectColorOjos = value;
                
                OnPropertyChanged("SelectColorOjos");
            }
        }
        private short selectFormaOjos;
        public short SelectFormaOjos
        {
            get { return selectFormaOjos; }
            set
            {
                selectFormaOjos = value;
                
                OnPropertyChanged("SelectFormaOjos");
            }
        }
        private short selectTamanioOjos;
        public short SelectTamanioOjos
        {
            get { return selectTamanioOjos; }
            set
            {
                selectTamanioOjos = value;
                
                OnPropertyChanged("SelectTamanioOjos");
            }
        }
        //NARIZ
        private short selectRaizNariz;
        public short SelectRaizNariz
        {
            get { return selectRaizNariz; }
            set
            {
                selectRaizNariz = value;
                
                OnPropertyChanged("SelectRaizNariz");
            }
        }
        private short selectDorsoNariz;
        public short SelectDorsoNariz
        {
            get { return selectDorsoNariz; }
            set
            {
                selectDorsoNariz = value;
                
                OnPropertyChanged("SelectDorsoNariz");
            }
        }
        private short selectAnchoNariz;
        public short SelectAnchoNariz
        {
            get { return selectAnchoNariz; }
            set
            {
                selectAnchoNariz = value;
                
                OnPropertyChanged("SelectAnchoNariz");
            }
        }
        private short selectBaseNariz;
        public short SelectBaseNariz
        {
            get { return selectBaseNariz; }
            set
            {
                selectBaseNariz = value;
                
                OnPropertyChanged("SelectBaseNariz");
            }
        }
        private short selectAlturaNariz;
        public short SelectAlturaNariz
        {
            get { return selectAlturaNariz; }
            set
            {
                selectAlturaNariz = value;
                
                OnPropertyChanged("SelectAlturaNariz");
            }
        }
        //LABIOS
        private short selectEspesorLabio;
        public short SelectEspesorLabio
        {
            get { return selectEspesorLabio; }
            set
            {
                selectEspesorLabio = value;
                
                OnPropertyChanged("SelectEspesorLabio");
            }
        }
        private short selectAlturaLabio;
        public short SelectAlturaLabio
        {
            get { return selectAlturaLabio; }
            set
            {
                selectAlturaLabio = value;
                
                OnPropertyChanged("SelectAlturaLabio");
            }
        }
        private short selectProminenciaLabio;
        public short SelectProminenciaLabio
        {
            get { return selectProminenciaLabio; }
            set
            {
                selectProminenciaLabio = value;
                
                OnPropertyChanged("SelectProminenciaLabio");
            }
        }
        //BOCA
        private short selectTamanioBoca;
        public short SelectTamanioBoca
        {
            get { return selectTamanioBoca; }
            set
            {
                selectTamanioBoca = value;
                
                OnPropertyChanged("SelectTamanioBoca");
            }
        }
        private short selectComisuraBoca;
        public short SelectComisuraBoca
        {
            get { return selectComisuraBoca; }
            set
            {
                selectComisuraBoca = value;
                
                OnPropertyChanged("SelectComisuraBoca");
            }
        }
        //MENTON
        private short selectFormaMenton;
        public short SelectFormaMenton
        {
            get { return selectFormaMenton; }
            set
            {
                selectFormaMenton = value;
                
                OnPropertyChanged("SelectFormaMenton");
            }
        }
        private short selectTipoMenton;
        public short SelectTipoMenton
        {
            get { return selectTipoMenton; }
            set
            {
                selectTipoMenton = value;
                
                OnPropertyChanged("SelectTipoMenton");
            }
        }
        private short selectInclinacionMenton;
        public short SelectInclinacionMenton
        {
            get { return selectInclinacionMenton; }
            set
            {
                selectInclinacionMenton = value;
                
                OnPropertyChanged("SelectInclinacionMenton");
            }
        }
        //OREJA DERECHA
        private short selectFormaOrejaDerecha;
        public short SelectFormaOrejaDerecha
        {
            get { return selectFormaOrejaDerecha; }
            set
            {
                selectFormaOrejaDerecha = value;
                
                OnPropertyChanged("SelectFormaOrejaDerecha");
            }
        }
        //HELIX DERECHA
        private short selectHelixOriginalOrejaDerecha;
        public short SelectHelixOriginalOrejaDerecha
        {
            get { return selectHelixOriginalOrejaDerecha; }
            set
            {
                selectHelixOriginalOrejaDerecha = value;
                
                OnPropertyChanged("SelectHelixOriginalOrejaDerecha");
            }
        }
        private short selectHelixSuperiorOrejaDerecha;
        public short SelectHelixSuperiorOrejaDerecha
        {
            get { return selectHelixSuperiorOrejaDerecha; }
            set
            {
                selectHelixSuperiorOrejaDerecha = value;
                
                OnPropertyChanged("SelectHelixSuperiorOrejaDerecha");
            }
        }
        private short selectHelixPosteriorOrejaDerecha;
        public short SelectHelixPosteriorOrejaDerecha
        {
            get { return selectHelixPosteriorOrejaDerecha; }
            set
            {
                selectHelixPosteriorOrejaDerecha = value;
                
                OnPropertyChanged("SelectHelixPosteriorOrejaDerecha");
            }
        }
        private short selectHelixAdherenciaOrejaDerecha;
        public short SelectHelixAdherenciaOrejaDerecha
        {
            get { return selectHelixAdherenciaOrejaDerecha; }
            set
            {
                selectHelixAdherenciaOrejaDerecha = value;
                
                OnPropertyChanged("SelectHelixAdherenciaOrejaDerecha");
            }
        }
        //LOBULO DERECHO
        private short selectLobuloContornoOrejaDerecha;
        public short SelectLobuloContornoOrejaDerecha
        {
            get { return selectLobuloContornoOrejaDerecha; }
            set
            {
                selectLobuloContornoOrejaDerecha = value;
                OnPropertyChanged("SelectLobuloContornoOrejaDerecha");
                
            }
        }
        private short selectLobuloAdherenciaOrejaDerecha;
        public short SelectLobuloAdherenciaOrejaDerecha
        {
            get { return selectLobuloAdherenciaOrejaDerecha; }
            set
            {
                selectLobuloAdherenciaOrejaDerecha = value;
                OnPropertyChanged("SelectLobuloAdherenciaOrejaDerecha");
                
            }
        }
        private short selectLobuloParticularidadOrejaDerecha;
        public short SelectLobuloParticularidadOrejaDerecha
        {
            get { return selectLobuloParticularidadOrejaDerecha; }
            set
            {
                selectLobuloParticularidadOrejaDerecha = value;
                OnPropertyChanged("SelectLobuloParticularidadOrejaDerecha");
                
            }
        }
        private short selectLobuloDimensionOrejaDerecha;
        public short SelectLobuloDimensionOrejaDerecha
        {
            get { return selectLobuloDimensionOrejaDerecha; }
            set
            {
                selectLobuloDimensionOrejaDerecha = value;
                OnPropertyChanged("SelectLobuloDimensionOrejaDerecha");
                
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

    }
}
