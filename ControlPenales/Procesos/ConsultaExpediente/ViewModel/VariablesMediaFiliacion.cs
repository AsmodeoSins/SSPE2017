using SSP.Servidor;
using System.Collections.ObjectModel;

namespace ControlPenales
{
    partial class ConsultaExpedienteInternoViewModel
    {
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

        #region SENIAS GENERALES
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
        #endregion

        #region SANGRE
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
        #endregion

        #region CABELLO
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
        #endregion

        #region FRENTE
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
        #endregion

        #region CEJAS
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
        #endregion

        #region OJOS
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
        #endregion

        #region NARIZ
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
        #endregion

        #region LABIOS
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
        #endregion

        #region BOCA
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
        #endregion

        #region MENTON
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
        #endregion

        #region OREJA DERECHA
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

        #region HELIX DERECHA
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
        #endregion

        #region LOBULO DERECHO
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

        #endregion
    }
}
