using SSP.Servidor;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
//using MvvmFramework;

namespace ControlPenales
{
    partial class ConsultaExpedienteInternoViewModel
    {
        #region Listas
        private ObservableCollection<ANATOMIA_TOPOGRAFICA> listRegionCuerpo;
        public ObservableCollection<ANATOMIA_TOPOGRAFICA> ListRegionCuerpo
        {
            get { return listRegionCuerpo; }
            set { listRegionCuerpo = value; OnPropertyChanged("ListRegionCuerpo"); }
        }
        private List<RadioButton> listRadioButons;
        public List<RadioButton> ListRadioButons
        {
            get { return listRadioButons; }
            set
            {
                listRadioButons = value;
                OnPropertyChanged("ListRadioButons");
                if (listRadioButons.Count > 0)
                {
                    foreach (var item in listRadioButons)
                    {
                        if (item.CommandParameter != null)
                        {
                            item.IsChecked = false;
                        }
                    }
                }
            }
        }
        private ObservableCollection<TATUAJE> listTipoTatuaje;
        public ObservableCollection<TATUAJE> ListTipoTatuaje
        {
            get { return listTipoTatuaje; }
            set { listTipoTatuaje = value; OnPropertyChanged("ListTipoTatuaje"); }
        }
        private ObservableCollection<TATUAJE_CLASIFICACION> listClasificacionTatuaje;
        public ObservableCollection<TATUAJE_CLASIFICACION> ListClasificacionTatuaje
        {
            get { return listClasificacionTatuaje; }
            set { listClasificacionTatuaje = value; OnPropertyChanged("ListClasificacionTatuaje"); }
        }
        private ObservableCollection<SENAS_PARTICULARES> listSenasParticulares = new ObservableCollection<SENAS_PARTICULARES>();
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
            }
        }

        private string textSignificado;
        public string TextSignificado
        {
            get { return textSignificado; }
            set { textSignificado = value; OnPropertyChanged("TextSignificado"); }
        }
        private string textAmpliarDescripcion;
        public string TextAmpliarDescripcion
        {
            get { return textAmpliarDescripcion; }
            set { textAmpliarDescripcion = value; OnPropertyChanged("TextAmpliarDescripcion"); }
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

        private TATUAJE_CLASIFICACION selectClasificacionTatuaje;
        public TATUAJE_CLASIFICACION SelectClasificacionTatuaje
        {
            get { return selectClasificacionTatuaje; }
            set
            {
                selectClasificacionTatuaje = value;
                OnPropertyChanged("SelectClasificacionTatuaje");
                if (!string.IsNullOrEmpty(TextTipoSenia) && SelectAnatomiaTopografica != null)
                {
                    var clasif = string.Empty;
                    if (SelectClasificacionTatuaje != null)
                        clasif = string.IsNullOrEmpty(SelectClasificacionTatuaje.ID_TATUAJE_CLA) ? "" : " " + SelectClasificacionTatuaje.DESCR;
                    if (selectClasificacionTatuaje != null && !string.IsNullOrEmpty(selectClasificacionTatuaje.ID_TATUAJE_CLA) && SelectTatuaje != null)
                        TextSignificado = TextTipoSenia + clasif + " EN " + SelectAnatomiaTopografica.DESCR + " CON IMAGEN(ES) DE " + selectTatuaje.DESCR + " " + TextAmpliarDescripcion;
                    else
                        TextSignificado = TextTipoSenia + clasif + " EN " + SelectAnatomiaTopografica.DESCR + " " + TextAmpliarDescripcion;
                }
            }
        }
        private TATUAJE selectTatuaje;
        public TATUAJE SelectTatuaje
        {
            get { return selectTatuaje; }
            set
            {
                selectTatuaje = value;
                OnPropertyChanged("SelectTatuaje");
                if (!string.IsNullOrEmpty(TextTipoSenia) && SelectAnatomiaTopografica != null)
                {
                    var clasif = string.Empty;
                    if (SelectClasificacionTatuaje != null)
                        clasif = string.IsNullOrEmpty(SelectClasificacionTatuaje.ID_TATUAJE_CLA) ? "" : " " + SelectClasificacionTatuaje.DESCR;
                    if (selectTatuaje != null && selectTatuaje.ID_TATUAJE > 0 && SelectClasificacionTatuaje != null)
                        TextSignificado = TextTipoSenia + clasif + " EN " + SelectAnatomiaTopografica.DESCR + " CON IMAGEN(ES) DE " + selectTatuaje.DESCR + " " + TextAmpliarDescripcion;
                    else
                        TextSignificado = TextTipoSenia + clasif + " EN " + SelectAnatomiaTopografica.DESCR + " " + TextAmpliarDescripcion;
                }
            }
        }

        private bool selectPresentaIngresar = true;
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
            set
            {
                selectTipoTatuaje = value;
                OnPropertyChanged("SelectTipoTatuaje");
            }
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
        private Visibility lineasGuiaFoto = Visibility.Collapsed;
        public Visibility LineasGuiaFoto
        {
            get { return lineasGuiaFoto; }
            set { lineasGuiaFoto = value; OnPropertyChanged("LineasGuiaFoto"); }
        }
        private bool botonTomarFotoEnabled;
        public bool BotonTomarFotoEnabled
        {
            get { return botonTomarFotoEnabled; }
            set { botonTomarFotoEnabled = value; OnPropertyChanged("BotonTomarFotoEnabled"); }
        }
        private bool datosEnabled = false;
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
        private Visibility comboFrontBackFotoVisible = Visibility.Collapsed;
        public Visibility ComboFrontBackFotoVisible
        {
            get { return comboFrontBackFotoVisible; }
            set { comboFrontBackFotoVisible = value; OnPropertyChanged("ComboFrontBackFotoVisible"); }
        }
        private BitmapSource imagenTatuaje;
        public BitmapSource ImagenTatuaje
        {
            get { return imagenTatuaje; }
            set { imagenTatuaje = value; OnPropertyChanged("ImagenTatuaje"); }
        }
        private bool tabFrente;
        public bool TabFrente
        {
            get { return tabFrente; }
            set { tabFrente = value; OnPropertyChanged("TabFrente"); }
        }
        private bool tabDorso;
        public bool TabDorso
        {
            get { return tabDorso; }
            set { tabDorso = value; OnPropertyChanged("TabDorso"); }
        }
        private char[] regionCodigo;
        public char[] RegionCodigo
        {
            get { return regionCodigo; }
            set { regionCodigo = value; OnPropertyChanged("RegionCodigo"); }
        }
        #endregion
    }
}
