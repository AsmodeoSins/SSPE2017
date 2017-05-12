using ControlPenales.Clases;
using SSP.Servidor;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows.Media.Imaging;
//using MvvmFramework;

namespace ControlPenales
{
    partial class RegistroIngresoViewModel
    {
        #region Listas
        private ObservableCollection<ANATOMIA_TOPOGRAFICA> listRegionCuerpo;
        public ObservableCollection<ANATOMIA_TOPOGRAFICA> ListRegionCuerpo
        {
            get { return listRegionCuerpo; }
            set { listRegionCuerpo = value; OnPropertyChanged("ListRegionCuerpo"); }
        }
        private ObservableCollection<TATUAJE> listTipoTatuaje;
        public ObservableCollection<TATUAJE> ListTipoTatuaje
        {
            get { return listTipoTatuaje; }
            set { listTipoTatuaje = value; OnPropertyChanged("ListTipoTatuaje"); }
        }
        private ObservableCollection<SENAS_PARTICULARES> listSenasParticulares;
        public ObservableCollection<SENAS_PARTICULARES> ListSenasParticulares
        {
            get { return listSenasParticulares; }
            set { listSenasParticulares = value; OnPropertyChanged("ListSenasParticulares"); }
        }
        #endregion

        #region Textos
        private string textTipoSenia;
        public string TextTipoSenia
        {
            get { return textTipoSenia; }
            set { textTipoSenia = value; OnPropertyChanged("TextTipoSenia"); }
        }
        private string textExpediente;
        public string TextExpediente
        {
            get { return textExpediente; }
            set { textExpediente = value; OnPropertyChanged("TextExpediente"); }
        }
        private string textNombreCompleto;
        public string TextNombreCompleto
        {
            get { return textNombreCompleto; }
            set { textNombreCompleto = value; OnPropertyChanged("TextNombreCompleto"); }
        }
        private string textCantidad;
        public string TextCantidad
        {
            get { return textCantidad; }
            set
            {
                var x = 0;
                if (Int32.TryParse(value, out x))
                {
                    textCantidad = value;
                    OnPropertyChanged("TextCantidad");
                    if (SelectTipoSenia > 0 && SelectAnatomiaTopografica != null && !string.IsNullOrEmpty(TextCantidad) && !string.IsNullOrEmpty(RegionValorCodigo))
                    {
                        CodigoSenia = SelectTipoSenia.ToString() + "" + RegionValorCodigo + "" + SelectAnatomiaTopografica.LADO + "" + (!string.IsNullOrEmpty(value) ? (value.Length == 1 ? "0" + value : value) : string.Empty);
                    }
                }
                //var x = 0;
                //if (Int32.TryParse(value, out x))
                //{
                //    if (x < 10)
                //        textCantidad = "0" + value;
                //    else
                //        textCantidad = value;
                //    OnPropertyChanged("TextCantidad");
                //    if (SelectTipoSenia > 0 && SelectAnatomiaTopografica != null && !string.IsNullOrEmpty(TextCantidad) && !string.IsNullOrEmpty(RegionValorCodigo))
                //    {
                //        CodigoSenia = SelectTipoSenia.ToString() + "" + RegionValorCodigo + "" + SelectAnatomiaTopografica.LADO + "" + TextCantidad;
                //    }
                //}
            }
        }
        private string textCodigo;
        public string TextCodigo
        {
            get { return textCodigo; }
            set { textCodigo = value; OnPropertyChanged("TextCodigo"); }
        }
        private string textSignificado;
        public string TextSignificado
        {
            get { return textSignificado; }
            set { textSignificado = value; OnPropertyChanged("TextSignificado"); }
        }
        #endregion

        #region Select
        private int selectTipoSenia;
        public int SelectTipoSenia
        {
            get { return selectTipoSenia; }
            set
            {
                selectTipoSenia = value;
                OnPropertyChanged("SelectTipoSenia");
                if (SelectTipoSenia == 1)
                {
                    TextTipoSenia = "CICATRIZ";
                }
                else if (SelectTipoSenia == 2)
                {
                    TextTipoSenia = "TATUAJE";
                }
                else if (SelectTipoSenia == 3)
                {
                    TextTipoSenia = "LUNAR";
                }
                else if (SelectTipoSenia == 4)
                {
                    TextTipoSenia = "DEFECTO FISICO";
                }
                else if (SelectTipoSenia == 5)
                {
                    TextTipoSenia = "PROTESIS";
                }
                else if (SelectTipoSenia == 6)
                {
                    TextTipoSenia = "AMPUTACION";
                }

            }
        }
        private SENAS_PARTICULARES selectSenaParticular;
        public SENAS_PARTICULARES SelectSenaParticular
        {
            get { return selectSenaParticular; }
            set { selectSenaParticular = value; OnPropertyChanged("SelectSenaParticular"); }
        }
        private ANATOMIA_TOPOGRAFICA selectAnatomiaTopografica;
        public ANATOMIA_TOPOGRAFICA SelectAnatomiaTopografica
        {
            get { return selectAnatomiaTopografica; }
            set { selectAnatomiaTopografica = value; OnPropertyChanged("SelectAnatomiaTopografica"); }
        }
        //private bool selectRegionCuerpo;
        //public bool SelectRegionCuerpo
        //{
        //    get { return selectRegionCuerpo; }
        //    set
        //    {
        //        selectRegionCuerpo = value;
        //        OnPropertyChanged("SelectRegionCuerpo");

        //    }
        //}
        private TATUAJE selectClasificacionTipoTatuaje;
        public TATUAJE SelectClasificacionTipoTatuaje
        {
            get { return selectClasificacionTipoTatuaje; }
            set
            {
                selectClasificacionTipoTatuaje = value;
                OnPropertyChanged("SelectClasificacionTipoTatuaje");
                if (!string.IsNullOrEmpty(TextTipoSenia) && SelectAnatomiaTopografica != null)
                {
                    if (SelectClasificacionTipoTatuaje != null && SelectClasificacionTipoTatuaje.ID_TATUAJE > 0)
                        TextSignificado = TextTipoSenia + " EN " + SelectAnatomiaTopografica.DESCR + " CON IMAGEN(ES) DE " + SelectClasificacionTipoTatuaje.DESCR;
                    else
                        TextSignificado = TextTipoSenia + " EN " + SelectAnatomiaTopografica.DESCR;
                }
            }
        }
        private bool selectClasificacionAntisocial;
        public bool SelectClasificacionAntisocial
        {
            get { return selectClasificacionAntisocial; }
            set { selectClasificacionAntisocial = value; OnPropertyChanged("SelectClasificacionAntisocial"); }
        }
        private bool selectClasificacionIdentificacion;
        public bool SelectClasificacionIdentificacion
        {
            get { return selectClasificacionIdentificacion; }
            set { selectClasificacionIdentificacion = value; OnPropertyChanged("SelectClasificacionIdentificacion"); }
        }
        private bool selectClasificacionErotico;
        public bool SelectClasificacionErotico
        {
            get { return selectClasificacionErotico; }
            set { selectClasificacionErotico = value; OnPropertyChanged("SelectClasificacionErotico"); }
        }
        private bool selectClasificacionDecorativo;
        public bool SelectClasificacionDecorativo
        {
            get { return selectClasificacionDecorativo; }
            set { selectClasificacionDecorativo = value; OnPropertyChanged("SelectClasificacionDecorativo"); }
        }
        private bool selectClasificacionReligioso;
        public bool SelectClasificacionReligioso
        {
            get { return selectClasificacionReligioso; }
            set { selectClasificacionReligioso = value; OnPropertyChanged("SelectClasificacionReligioso"); }
        }
        private bool selectClasificacionSentimental;
        public bool SelectClasificacionSentimental
        {
            get { return selectClasificacionSentimental; }
            set { selectClasificacionSentimental = value; OnPropertyChanged("SelectClasificacionSentimental"); }
        }
        /*private bool selectClasificacion;*/
        private bool selectPresentaIngresar;
        public bool SelectPresentaIngresar
        {
            get { return selectPresentaIngresar; }
            set { selectPresentaIngresar = value; OnPropertyChanged("SelectPresentaIngresar"); }
        }
        private bool selectPresentaIntramuros;
        public bool SelectPresentaIntramuros
        {
            get { return selectPresentaIntramuros; }
            set { selectPresentaIntramuros = value; OnPropertyChanged("SelectPresentaIntramuros"); }
        }
        /*private bool selectPresenta;*/
        private bool selectTipoCicatriz;
        public bool SelectTipoCicatriz
        {
            get { return selectTipoCicatriz; }
            set { selectTipoCicatriz = value; OnPropertyChanged("SelectTipoCicatriz"); }
        }
        private bool selectTipoLunar;
        public bool SelectTipoLunar
        {
            get { return selectTipoLunar; }
            set { selectTipoLunar = value; OnPropertyChanged("SelectTipoLunar"); }
        }
        private bool selectTipoTatuaje;
        public bool SelectTipoTatuaje
        {
            get { return selectTipoTatuaje; }
            set { selectTipoTatuaje = value; OnPropertyChanged("SelectTipoTatuaje"); }
        }
        private bool selectTipoDefecto;
        public bool SelectTipoDefecto
        {
            get { return selectTipoDefecto; }
            set { selectTipoDefecto = value; OnPropertyChanged("SelectTipoDefecto"); }
        }
        private bool selectTipoProtesis;
        public bool SelectTipoProtesis
        {
            get { return selectTipoProtesis; }
            set { selectTipoProtesis = value; OnPropertyChanged("SelectTipoProtesis"); }
        }
        private bool selectTipoAmputacion;
        public bool SelectTipoAmputacion
        {
            get { return selectTipoAmputacion; }
            set { selectTipoAmputacion = value; OnPropertyChanged("SelectTipoAmputacion"); }
        }
        /*private bool selectTipo;*/
        #endregion

        #region Focus
        private bool focusCantidad;
        public bool FocusCantidad
        {
            get { return focusCantidad; }
            set { focusCantidad = value; OnPropertyChanged("FocusCantidad"); }
        }
        #endregion

        #region Otros
        private bool datosEnabled;
        public bool DatosEnabled
        {
            get { return datosEnabled; }
            set { datosEnabled = value; OnPropertyChanged("DatosEnabled"); }
        }
        private string codigoSenia;
        public string CodigoSenia
        {
            get { return codigoSenia; }
            set
            {
                codigoSenia = value;
                OnPropertyChanged("CodigoSenia");
            }
        }
        private string regionValorCodigo;
        public string RegionValorCodigo
        {
            get { return regionValorCodigo; }
            set { regionValorCodigo = value; OnPropertyChanged("RegionValorCodigo"); }
        }
        private bool seniasParticularesEditable;
        public bool SeniasParticularesEditable
        {
            get { return seniasParticularesEditable; }
            set { seniasParticularesEditable = value; OnPropertyChanged("SeniasParticularesEditable"); }
        }
        private BitmapSource imagenTatuaje;
        public BitmapSource ImagenTatuaje
        {
            get { return imagenTatuaje; }
            set { imagenTatuaje = value; OnPropertyChanged("ImagenTatuaje"); }
        }
        #endregion
    }
}
