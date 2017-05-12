﻿using ControlPenales.Clases;
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
    partial class EstatusAdministrativoViewModel
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
            set
            {
                listSenasParticulares = value;
                OnPropertyChanged("ListSenasParticulares");
            }
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
                if (string.IsNullOrWhiteSpace(value))
                {
                    textCantidad = string.Empty;
                    RaisePropertyChanged("TextCantidad");
                    CodigoSenia = string.Empty;
                }
                else
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
                        //if (x>0)
                        //{
                        //    textCantidad = value.PadLeft(2, '0');
                        //    RaisePropertyChanged("TextCantidad");
                        //    if (SelectTipoSenia > 0 && SelectAnatomiaTopografica != null && !string.IsNullOrEmpty(textCantidad) && !string.IsNullOrEmpty(RegionValorCodigo))
                        //    {
                        //        CodigoSenia = SelectTipoSenia.ToString() + "" + RegionValorCodigo + "" + SelectAnatomiaTopografica.LADO + "" + textCantidad;
                        //    }
                        //}
                        //else
                        //{
                        //    textCantidad = value;
                        //    RaisePropertyChanged("TextCantidad");
                        //    CodigoSenia = string.Empty;
                        //}
                    }
                }
                

            }
        }
        /*private string textCodigo;
        public string TextCodigo
        {
            get { return textCodigo; }
            set { textCodigo = value; OnPropertyChanged("TextCodigo"); }
        }*/
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
                OnPropertyValidateChanged("SelectTipoSenia");
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
            set { selectSenaParticular = value; OnPropertyValidateChanged("SelectSenaParticular"); }
        }
        private ANATOMIA_TOPOGRAFICA selectAnatomiaTopografica;
        public ANATOMIA_TOPOGRAFICA SelectAnatomiaTopografica
        {
            get { return selectAnatomiaTopografica; }
            set { selectAnatomiaTopografica = value; OnPropertyValidateChanged("SelectAnatomiaTopografica"); }
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
        private TATUAJE_CLASIFICACION selectClasificacionTatuaje;
        public TATUAJE_CLASIFICACION SelectClasificacionTatuaje
        {
            get { return selectClasificacionTatuaje; }
            set
            {
                selectClasificacionTatuaje = value;
                OnPropertyValidateChanged("SelectClasificacionTatuaje");
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
                OnPropertyValidateChanged("SelectTatuaje");
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
        /*
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
        }*/

        private bool selectPresentaIngresar = false;
        public bool SelectPresentaIngresar
        {
            get { return selectPresentaIngresar; }
            set
            {
                selectPresentaIngresar = value;
                OnPropertyValidateChanged("SelectPresentaIngresar");
                if (selectPresentaIngresar || selectPresentaIntramuros)
                    IsPresentaSelected = true;
                else
                    IsPresentaSelected = false;

            }
        }
        private bool selectPresentaIntramuros=false;
        public bool SelectPresentaIntramuros
        {
            get { return selectPresentaIntramuros; }
            set
            {
                selectPresentaIntramuros = value;
                OnPropertyValidateChanged("SelectPresentaIntramuros");
                if (selectPresentaIngresar || selectPresentaIntramuros)
                    IsPresentaSelected = true;
                else
                    IsPresentaSelected = false;
            }
        }
        private bool selectTipoCicatriz=false;
        public bool SelectTipoCicatriz
        {
            get { return selectTipoCicatriz; }
            set
            {
                selectTipoCicatriz = value;
                OnPropertyValidateChanged("SelectTipoCicatriz");
                if (selectTipoCicatriz || selectTipoTatuaje || selectTipoLunar || selectTipoDefecto || selectTipoProtesis || selectTipoAmputacion)
                    IsTipoSelected = true;
                else
                    IsTipoSelected = false;
            }
        }
        private bool selectTipoLunar;
        public bool SelectTipoLunar
        {
            get { return selectTipoLunar; }
            set 
            {
                selectTipoLunar = value;
                OnPropertyValidateChanged("SelectTipoLunar");
                if (selectTipoCicatriz || selectTipoTatuaje || selectTipoLunar || selectTipoDefecto || selectTipoProtesis || selectTipoAmputacion)
                    IsTipoSelected = true;
                else
                    IsTipoSelected = false;
            }
        }
        private bool selectTipoTatuaje;
        public bool SelectTipoTatuaje
        {
            get { return selectTipoTatuaje; }
            set
            {
                selectTipoTatuaje = value;
                if (value)
                {
                    base.AddRule(() => SelectClasificacionTatuaje, () => SelectClasificacionTatuaje != null && SelectClasificacionTatuaje.ID_TATUAJE_CLA != string.Empty, "CLASIFICACIÓN TATUAJE ES REQUERIDO");
                    base.AddRule(() => SelectTatuaje, () => SelectTatuaje != null && SelectTatuaje.ID_TATUAJE > 0, "IMAGEN ES REQUERIDA");
                    RaisePropertyChanged("SelectClasificacionTatuaje");
                    RaisePropertyChanged("SelectTatuaje");
                    TipoTatuajeEnabled = true;
                }
                else
                {
                    base.RemoveRule("SelectClasificacionTatuaje");
                    base.RemoveRule("SelectTatuaje");
                    RaisePropertyChanged("SelectClasificacionTatuaje");
                    RaisePropertyChanged("SelectTatuaje");
                }
                OnPropertyValidateChanged("SelectTipoTatuaje");
                if (selectTipoCicatriz || selectTipoTatuaje || selectTipoLunar || selectTipoDefecto || selectTipoProtesis || selectTipoAmputacion)
                    IsTipoSelected = true;
                else
                    IsTipoSelected = false;
            }
        }
        private bool selectTipoDefecto;
        public bool SelectTipoDefecto
        {
            get { return selectTipoDefecto; }
            set 
            {
                selectTipoDefecto = value;
                OnPropertyValidateChanged("SelectTipoDefecto");
                if (selectTipoCicatriz || selectTipoTatuaje || selectTipoLunar || selectTipoDefecto || selectTipoProtesis || selectTipoAmputacion)
                    IsTipoSelected = true;
                else
                    IsTipoSelected = false;

            }
        }
        private bool selectTipoProtesis;
        public bool SelectTipoProtesis
        {
            get { return selectTipoProtesis; }
            set
            {
                selectTipoProtesis = value;
                OnPropertyValidateChanged("SelectTipoProtesis");
                if (selectTipoCicatriz || selectTipoTatuaje || selectTipoLunar || selectTipoDefecto || selectTipoProtesis || selectTipoAmputacion)
                    IsTipoSelected = true;
                else
                    IsTipoSelected = false;
            }
        }
        private bool selectTipoAmputacion;
        public bool SelectTipoAmputacion
        {
            get { return selectTipoAmputacion; }
            set 
            {
                selectTipoAmputacion = value;
                OnPropertyValidateChanged("SelectTipoAmputacion");
                if (selectTipoCicatriz || selectTipoTatuaje || selectTipoLunar || selectTipoDefecto || selectTipoProtesis || selectTipoAmputacion)
                    IsTipoSelected = true;
                else
                    IsTipoSelected = false;
            }
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
        private bool datosEnabled = true;
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
        private bool visibleTomarFotoSenasParticulares = false;
        public bool VisibleTomarFotoSenasParticulares
        {
            get { return visibleTomarFotoSenasParticulares; }
            set { visibleTomarFotoSenasParticulares = value; OnPropertyChanged("VisibleTomarFotoSenasParticulares"); }
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
            set { 
                imagenTatuaje = value;
                OnPropertyValidateChanged("ImagenTatuaje");
                if (value != null)
                    IsImagenTatuajeCapturada = true;
            }
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

        #region Validaciones Banderas
        private bool isPresentaSelected = false;
        public bool IsPresentaSelected
        {
            get { return isPresentaSelected; }
            set { isPresentaSelected = value; RaisePropertyChanged("IsPresentaSelected"); }
        }

        private bool isTipoSelected = false;
        public bool IsTipoSelected
        {
            get { return isTipoSelected; }
            set { isTipoSelected = value; RaisePropertyChanged("IsTipoSelected"); }
        }

        private bool isImagenTatuajeCapturada = false;
        public bool IsImagenTatuajeCapturada
        {
            get { return isImagenTatuajeCapturada; }
            set { isImagenTatuajeCapturada = value; RaisePropertyChanged("IsImagenTatuajeCapturada"); }
        }
        #endregion
    }
}