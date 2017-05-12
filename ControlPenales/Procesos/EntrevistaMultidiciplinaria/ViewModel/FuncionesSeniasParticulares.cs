using System;
using System.Windows;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.ObjectModel;
using SSP.Servidor;
using SSP.Controlador.Catalogo.Justicia;
using SSP.Controlador.Catalogo.Justicia.EstudioMI;
using System.Windows.Media.Imaging;
using System.Windows.Controls;
using ControlPenales.Clases;
using System.Windows.Interop;


namespace ControlPenales
{
    partial class EntrevistaMultidiciplinariaViewModel : ValidationViewModelBase
    {
        //VARIABLES
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

        private ObservableCollection<SENAS_PARTICULARES> listSenasParticulares;
        public ObservableCollection<SENAS_PARTICULARES> ListSenasParticulares
        {
            get { return listSenasParticulares; }
            set { listSenasParticulares = value; OnPropertyChanged("ListSenasParticulares"); }
        }

        private SENAS_PARTICULARES selectSenaParticular;
        public SENAS_PARTICULARES SelectSenaParticular
        {
            get { return selectSenaParticular; }
            set { selectSenaParticular = value; OnPropertyChanged("SelectSenaParticular"); }
        }

        private bool _selectPresentaIngresar;
        public bool SelectPresentaIngresar
        {
            get { return _selectPresentaIngresar; }
            set
            {
                _selectPresentaIngresar = value;
                OnPropertyChanged("SelectPresentaIngresar");
            }
        }

        private bool selectPresentaIntramuros = true;
        public bool SelectPresentaIntramuros
        {
            get { return selectPresentaIntramuros; }
            set
            {
                selectPresentaIntramuros = value;
                OnPropertyChanged("SelectPresentaIntramuros");
            }
        }

        private bool selectTipoCicatriz;
        public bool SelectTipoCicatriz
        {
            get { return selectTipoCicatriz; }
            set
            {
                selectTipoCicatriz = value;
                OnPropertyChanged("SelectTipoCicatriz");
            }
        }

        private bool selectTipoTatuaje;
        public bool SelectTipoTatuaje
        {
            get { return selectTipoTatuaje; }
            set
            {
                selectTipoTatuaje = value;
                TipoTatuajeEnabled = value;
                SelectClasificacionTatuaje = ListClasificacionTatuaje.Where(w => string.IsNullOrEmpty(w.ID_TATUAJE_CLA)).FirstOrDefault();
                SelectTatuaje = ListTipoTatuaje.Where(w => w.ID_TATUAJE == -1).FirstOrDefault();
                OnPropertyChanged("SelectTipoTatuaje");
            }
        }

        private bool selectTipoLunar;
        public bool SelectTipoLunar
        {
            get { return selectTipoLunar; }
            set
            {
                selectTipoLunar = value;
                OnPropertyChanged("SelectTipoLunar");
            }
        }

        private bool selectTipoDefecto;
        public bool SelectTipoDefecto
        {
            get { return selectTipoDefecto; }
            set
            {
                selectTipoDefecto = value;
                OnPropertyChanged("SelectTipoDefecto");
            }
        }

        private bool selectTipoProtesis;
        public bool SelectTipoProtesis
        {
            get { return selectTipoProtesis; }
            set
            {
                selectTipoProtesis = value;
                OnPropertyChanged("SelectTipoProtesis");
            }
        }

        private bool selectTipoAmputacion;
        public bool SelectTipoAmputacion
        {
            get { return selectTipoAmputacion; }
            set
            {
                selectTipoAmputacion = value;
                OnPropertyChanged("SelectTipoAmputacion");
            }
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
                //textCantidad = value;
                //if (SelectTipoSenia > 0 && SelectAnatomiaTopografica != null && !string.IsNullOrEmpty(TextCantidad) && !string.IsNullOrEmpty(RegionValorCodigo))
                //{
                //    CodigoSenia = string.Format("{0}{1}{2}{3}", SelectTipoSenia.ToString(), RegionValorCodigo, SelectAnatomiaTopografica.LADO, TextCantidad.PadLeft(2, '0'));
                //} 
                //OnPropertyChanged("TextCantidad");
            }
        }

        private string codigoSenia;
        public string CodigoSenia
        {
            get { return codigoSenia; }
            set { codigoSenia = value; OnPropertyChanged("CodigoSenia"); }
        }

        private ObservableCollection<TATUAJE_CLASIFICACION> listClasificacionTatuaje;
        public ObservableCollection<TATUAJE_CLASIFICACION> ListClasificacionTatuaje
        {
            get { return listClasificacionTatuaje; }
            set { listClasificacionTatuaje = value; OnPropertyChanged("ListClasificacionTatuaje"); }
        }

        private ObservableCollection<TATUAJE> listTipoTatuaje;
        public ObservableCollection<TATUAJE> ListTipoTatuaje
        {
            get { return listTipoTatuaje; }
            set { listTipoTatuaje = value; OnPropertyChanged("ListTipoTatuaje"); }
        }

        private string textSignificado;
        public string TextSignificado
        {
            get { return textSignificado; }
            set { textSignificado = value; OnPropertyChanged("TextSignificado"); }
        }

        private bool tipoTatuajeEnabled;
        public bool TipoTatuajeEnabled
        {
            get { return tipoTatuajeEnabled; }
            set { tipoTatuajeEnabled = value; OnPropertyChanged("TipoTatuajeEnabled"); }
        }

        private BitmapSource imagenTatuaje;
        public BitmapSource ImagenTatuaje
        {
            get { return imagenTatuaje; }
            set { imagenTatuaje = value; OnPropertyChanged("ImagenTatuaje"); }
        }

        //FRENTE /DORSO
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

        private List<RadioButton> listRadioButons;
        public List<RadioButton> ListRadioButons
        {
            get { return listRadioButons; }
            set { listRadioButons = value; OnPropertyChanged("ListRadioButons"); }
        }

        private string textTipoSenia;
        public string TextTipoSenia
        {
            get { return textTipoSenia; }
            set { textTipoSenia = value; OnPropertyChanged("TextTipoSenia"); }
        }

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

        private string regionValorCodigo;
        public string RegionValorCodigo
        {
            get { return regionValorCodigo; }
            set { regionValorCodigo = value; OnPropertyChanged("RegionValorCodigo"); }
        }

        private ANATOMIA_TOPOGRAFICA selectAnatomiaTopografica;
        public ANATOMIA_TOPOGRAFICA SelectAnatomiaTopografica
        {
            get { return selectAnatomiaTopografica; }
            set { selectAnatomiaTopografica = value; OnPropertyChanged("SelectAnatomiaTopografica"); }
        }

        private string textAmpliarDescripcion;
        public string TextAmpliarDescripcion
        {
            get { return textAmpliarDescripcion; }
            set { textAmpliarDescripcion = value; OnPropertyChanged("TextAmpliarDescripcion"); }
        }

        private ObservableCollection<ANATOMIA_TOPOGRAFICA> listRegionCuerpo;
        public ObservableCollection<ANATOMIA_TOPOGRAFICA> ListRegionCuerpo
        {
            get { return listRegionCuerpo; }
            set { listRegionCuerpo = value; OnPropertyChanged("ListRegionCuerpo"); }
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
       
        #region Focus
        private bool focusCantidad;
        public bool FocusCantidad
        {
            get { return focusCantidad; }
            set { focusCantidad = value; OnPropertyChanged("FocusCantidad"); }
        }
        #endregion

        #region Otros
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
     
        private bool seniasParticularesEditable;
        public bool SeniasParticularesEditable
        {
            get { return seniasParticularesEditable; }
            set { seniasParticularesEditable = value; OnPropertyChanged("SeniasParticularesEditable"); }
        }

        private char[] regionCodigo;
        public char[] RegionCodigo
        {
            get { return regionCodigo; }
            set { regionCodigo = value; OnPropertyChanged("RegionCodigo"); }
        }
        #endregion
        
        //SELECTED
        private bool tabSeniaParticularSelected;
        public bool TabSeniaParticularSelected
        {
            get { return tabSeniaParticularSelected; }
            set { tabSeniaParticularSelected = value; OnPropertyChanged("TabSeniaParticularSelected"); }
        }

        //FUNCIONES
        private void PopulateGeneralesSenias()
        {
            try
            {
                if (emiActual != null)
                {
                    if (SelectIngreso != null)
                    {
                        if (SelectIngreso.IMPUTADO != null)
                        {
                            TextExpediente = string.Format("{0}/{1}", SelectIngreso.IMPUTADO.ID_ANIO, SelectIngreso.IMPUTADO.ID_IMPUTADO);
                            TextNombreCompleto = string.Format("{0} {1} {2}", SelectIngreso.IMPUTADO.NOMBRE.Trim(), SelectIngreso.IMPUTADO.PATERNO.Trim(), SelectIngreso.IMPUTADO.MATERNO.Trim());
                            if (SelectIngreso.IMPUTADO.SENAS_PARTICULARES != null)
                            {
                                ListSenasParticulares = new ObservableCollection<SENAS_PARTICULARES>(SelectIngreso.IMPUTADO.SENAS_PARTICULARES);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al establecer señas generales", ex);
            }
        }

        private void PopulateSeniaParticular()
        {
            try
            {
                //EnfermedadesEnabled = true;
                if (emiActual != null)
                    if (SelectSenaParticular != null)
                    {
                        if (SelectSenaParticular.INTRAMUROS.Equals("S"))
                        {
                            SelectPresentaIntramuros = true;
                        }
                        else
                        {
                            SelectPresentaIngresar = true;
                        }

                        switch (SelectSenaParticular.TIPO)
                        {
                            case 1:
                                SelectTipoCicatriz = true;
                                break;
                            case 2:
                                SelectTipoTatuaje = true;
                                break;
                            case 3:
                                SelectTipoLunar = true;
                                break;
                            case 4:
                                SelectTipoDefecto = true;
                                break;
                            case 5:
                                SelectTipoProtesis = true;
                                break;
                            case 6:
                                SelectTipoAmputacion = true;
                                break;
                        }

                        SelectTipoSenia = (int)SelectSenaParticular.TIPO;
                        TextCantidad = SelectSenaParticular.CANTIDAD != null ? SelectSenaParticular.CANTIDAD.ToString() : string.Empty;
                        TextSignificado = SelectSenaParticular.SIGNIFICADO;
                        var clasificacion = ListClasificacionTatuaje.Where(w => w.ID_TATUAJE_CLA == (SelectSenaParticular.CLASIFICACION != null ? SelectSenaParticular.CLASIFICACION : string.Empty));
                        if (clasificacion != null)
                            SelectClasificacionTatuaje = clasificacion.FirstOrDefault();

                        var tatuaje = ListTipoTatuaje.Where(w => w.ID_TATUAJE == (SelectSenaParticular.ID_TIPO_TATUAJE != null ? SelectSenaParticular.ID_TIPO_TATUAJE : (short)0));
                        if (tatuaje != null)
                            SelectTatuaje = tatuaje.FirstOrDefault();
                        if (SelectSenaParticular.IMAGEN != null)
                            ImagenTatuaje = new Imagenes().ConvertByteToBitmap(SelectSenaParticular.IMAGEN);
                        CodigoSenia = SelectSenaParticular.CODIGO;
                        RegionValorCodigo = CodigoSenia[1].ToString() + CodigoSenia[2].ToString() + CodigoSenia[3].ToString() + "";
                        SelectAnatomiaTopografica = SelectSenaParticular.ANATOMIA_TOPOGRAFICA;
                        if (!string.IsNullOrEmpty(CodigoSenia))
                        {
                            bool buscar = false;
                            if (CodigoSenia[4].ToString() == "F")
                            {
                                if (TabFrente)
                                    buscar = true;
                                TabFrente = true;
                            }
                            else if (CodigoSenia[4].ToString() == "D")
                            {
                                if (TabDorso)
                                    buscar = true;
                                TabDorso = true;
                            }
                            if (buscar)
                            {
                                if (!string.IsNullOrEmpty(CodigoSenia))
                                {
                                    if (ListRadioButons != null)
                                        foreach (var item in ListRadioButons)
                                        {
                                            if (item.CommandParameter != null)
                                            {
                                                if (item.CommandParameter.ToString().Contains(SelectSenaParticular.ID_REGION.ToString() + "-" + CodigoSenia[1] + CodigoSenia[2] + CodigoSenia[3]))
                                                {
                                                    item.IsChecked = true;
                                                }
                                                else
                                                {
                                                    item.IsChecked = false;
                                                }
                                            }
                                        }
                                }
                            }
                        }

                    }
            }
            catch (Exception ex)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al establecer señas particulares", ex);
            }
        }

        private bool ValidarSeniasParticulares() {
            try
            {
                if (!SelectPresentaIngresar && !SelectPresentaIntramuros)
                    return false;
                if (!SelectTipoCicatriz && !SelectTipoTatuaje && !SelectTipoLunar && !SelectTipoDefecto && !SelectTipoProtesis && !SelectTipoAmputacion)
                    return false;
                if (string.IsNullOrEmpty(TextCantidad) || string.IsNullOrEmpty(CodigoSenia) || string.IsNullOrEmpty(TextSignificado) || ImagenTatuaje == null || SelectAnatomiaTopografica == null || SelectTipoSenia == 0)
                    return false;
                if (SelectTipoTatuaje)
                {
                    if (SelectClasificacionTatuaje == null || SelectTatuaje == null)
                        return false;
                    else
                        if (string.IsNullOrEmpty(SelectClasificacionTatuaje.ID_TATUAJE_CLA) || SelectTatuaje.ID_TATUAJE == -1)
                            return false;
                }
            }
            catch (Exception ex)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al validar señas particulares", ex);
            }
            return true;
        }
      
        private bool GuardarSenasParticulares()
        {
            try
            {
                if (SelectIngreso == null)
                {
                    new Dialogos().ConfirmacionDialogo("Notificación", "Favor de seleccionar un imputado.");
                    return false;
                }
                if (emiActual == null)
                {
                    new Dialogos().ConfirmacionDialogo("Notificación", "Favor de seleccionar un imputado.");
                    return false;
                }

                if (ValidarSeniasParticulares())
                {
                    var sp = new SENAS_PARTICULARES();
                    sp.ID_CENTRO = SelectIngreso.ID_CENTRO;
                    sp.ID_ANIO = SelectIngreso.ID_ANIO;
                    sp.ID_IMPUTADO = SelectIngreso.ID_IMPUTADO;
                    sp.ID_INGRESO = SelectIngreso.ID_INGRESO;
                    sp.ID_REGION = SelectAnatomiaTopografica.ID_REGION;
                    sp.INTRAMUROS = SelectPresentaIntramuros ? "S" : "N";
                    sp.TIPO = (short)SelectTipoSenia;
                    sp.CANTIDAD = short.Parse(TextCantidad);
                    sp.CODIGO = CodigoSenia;
                    if (SelectTipoTatuaje)
                    {
                        sp.CLASIFICACION = SelectClasificacionTatuaje.ID_TATUAJE_CLA;
                        sp.ID_TIPO_TATUAJE = SelectTatuaje.ID_TATUAJE;
                    }
                    sp.SIGNIFICADO = TextSignificado;
                    sp.IMAGEN = new Imagenes().ConvertBitmapToByte(ImagenTatuaje);
                    if (SelectSenaParticular != null)//Editar
                    {
                        if (PEditar)
                        {
                            sp.ID_SENA = SelectSenaParticular.ID_SENA;
                            if (!new cSenasParticulares().ActualizarSP(sp))
                            {
                                MensajeDialogo("Señas Particulares", false);
                                return false;
                            }
                        }
                        else
                        {
                            StaticSourcesViewModel.Mensaje("Validación", "Su usuario no cuenta con permisos para realizar esta acción.", StaticSourcesViewModel.enumTipoMensaje.MESNAJE_ADVERTENCIA, 5);
                            return false;
                        }
                        
                    }
                    else//true
                    {
                        if (PInsertar)
                        {
                            if (!new cSenasParticulares().Insertar(sp))
                            {
                                MensajeDialogo("Señas Particulares", false);
                                return false;
                            }
                        }
                    }
                    //CARGAMOS LA LISTA DE SENIAS PARTICULARES
                    ListSenasParticulares = new ObservableCollection<SENAS_PARTICULARES>(new cSenasParticulares().ObtenerTodos(SelectIngreso.ID_CENTRO, SelectIngreso.ID_ANIO, SelectIngreso.ID_IMPUTADO, SelectIngreso.ID_INGRESO));
                    MensajeDialogo("Señas Particulares", true);
                    return true;
                }
                else
                    Mensaje(false, "Señas Particulares");
            }
            catch (Exception ex)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al guardar señas particulares", ex);
            }
            return false;
        }
        
        private void LimpiarSenias()
        {
            try
            {
                SelectPresentaIngresar = false;
                SelectPresentaIntramuros = true;
                SelectTipoCicatriz = SelectTipoTatuaje = TipoTatuajeEnabled = SelectTipoLunar = SelectTipoDefecto = SelectTipoProtesis = SelectTipoAmputacion = false;
                TextCantidad = CodigoSenia = string.Empty;
                TextSignificado = string.Empty;
                TextAmpliarDescripcion = string.Empty;
                SelectClasificacionTatuaje = null;
                SelectTatuaje = null;
                ImagenTatuaje = null;
                SelectAnatomiaTopografica = null;
                
                foreach (var item in ListRadioButons)
                {
                    item.IsChecked = false;
                }
            }
            catch (Exception ex)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al limpiar señas", ex);
            }
        }

        private void TipoSwitch(Object obj)
        {
            try
            {
                //int.TryParse(obj.ToString);
                if (Int32.TryParse(obj.ToString(), out selectTipoSenia))
                {
                    SelectTipoSenia = selectTipoSenia;
                }
                if (!string.IsNullOrEmpty(CodigoSenia))
                    RegionValorCodigo = string.Format("{0}{1}{2}", CodigoSenia[1].ToString(), CodigoSenia[2].ToString(), CodigoSenia[3].ToString());
                if (SelectTipoSenia > 0 && SelectAnatomiaTopografica != null && !string.IsNullOrEmpty(TextCantidad) && !string.IsNullOrEmpty(RegionValorCodigo))
                {
                    CodigoSenia = string.Format("{0}{1}{2}{3}", SelectTipoSenia.ToString(), RegionValorCodigo, SelectAnatomiaTopografica.LADO, TextCantidad.PadLeft(2, '0'));
                }
                if (SelectAnatomiaTopografica != null)
                {
                    TipoTatuajeEnabled = false;
                    if (!string.IsNullOrEmpty(TextTipoSenia))
                    {
                        var clasif = string.Empty;
                        if (SelectClasificacionTatuaje != null)
                            clasif = string.IsNullOrEmpty(SelectClasificacionTatuaje.ID_TATUAJE_CLA) ? string.Empty : " " + SelectClasificacionTatuaje.DESCR;
                        if (SelectTatuaje != null && SelectTatuaje.ID_TATUAJE > 0)
                            TextSignificado = TextTipoSenia + clasif + " EN " + SelectAnatomiaTopografica.DESCR + " CON IMAGEN(ES) DE " + SelectTatuaje.DESCR + " " + TextAmpliarDescripcion;
                        else
                            TextSignificado = TextTipoSenia + clasif + " EN " + SelectAnatomiaTopografica.DESCR + " " + TextAmpliarDescripcion;
                    }
                }
                if (SelectTipoSenia == 2)
                {
                    TipoTatuajeEnabled = true;
                    SelectTatuaje = ListTipoTatuaje.Where(w => w.ID_TATUAJE == -1).FirstOrDefault();
                    SelectClasificacionTatuaje = ListClasificacionTatuaje.Where(w => w.ID_TATUAJE_CLA == string.Empty).FirstOrDefault();
                }
                else
                {
                    TipoTatuajeEnabled = false;
                    SelectTatuaje = ListTipoTatuaje.Where(w => w.ID_TATUAJE == 0).FirstOrDefault();
                    SelectClasificacionTatuaje = ListClasificacionTatuaje.Where(w => w.ID_TATUAJE_CLA == string.Empty).FirstOrDefault();
                }
            }
            catch (Exception ex)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error en el flujo del modulo", ex);
            }
        }

        private void RegionSwitch(Object obj)
        {
            try
            {
                string[] splits = obj.ToString().Split('-');
                //SelectSenaParticular
                RegionValorCodigo = splits[1];
                SelectAnatomiaTopografica = ListRegionCuerpo.Where(w => w.ID_REGION.ToString() == splits[0]).FirstOrDefault();
                if (SelectTipoSenia > 0 && SelectAnatomiaTopografica != null && !string.IsNullOrEmpty(TextCantidad) && !string.IsNullOrEmpty(RegionValorCodigo))
                {
                    CodigoSenia = string.Format("{0}{1}{2}{3}", SelectTipoSenia.ToString(), RegionValorCodigo, SelectAnatomiaTopografica.LADO, TextCantidad);
                }
                if (!string.IsNullOrEmpty(TextTipoSenia) && SelectAnatomiaTopografica != null)
                {
                    var clasif = string.Empty;
                    if (SelectClasificacionTatuaje != null)
                        clasif = string.IsNullOrEmpty(SelectClasificacionTatuaje.ID_TATUAJE_CLA) ? string.Empty : SelectClasificacionTatuaje.DESCR;
                    if (SelectTatuaje != null && SelectTatuaje.ID_TATUAJE > 0)
                        TextSignificado = TextTipoSenia + clasif + " EN " + SelectAnatomiaTopografica.DESCR + " CON IMAGEN(ES) DE " + SelectTatuaje.DESCR + " " + TextAmpliarDescripcion;
                    else
                        TextSignificado = TextTipoSenia + clasif + " EN " + SelectAnatomiaTopografica.DESCR + " " + TextAmpliarDescripcion;
                }
            }
            catch (Exception ex)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error en el flujo del modulo", ex);
            }
        }

        private async void TomarFotoLoad(TomarFotoSenaParticularView Window = null)
        {
            try
            {
                if (!((System.Windows.UIElement)(Window.TomarFotoSenaParticularWindow)).IsVisible) return;
                CamaraWeb = new WebCam(new WindowInteropHelper(Application.Current.Windows[0]).Handle);
                await CamaraWeb.InitializeWebCam(new List<System.Windows.Controls.Image> { Window.ImgSenaParticular });
            }
            catch (Exception ex)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al cargar cámara", ex);
            }
        }
  
    }
}
