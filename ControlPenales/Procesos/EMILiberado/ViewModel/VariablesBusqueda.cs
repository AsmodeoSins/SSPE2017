using SSP.Servidor;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Media.Imaging;
using ControlPenales.Clases;
using System.IO;
using System.Collections.Generic;
using System.Windows.Media;
using ControlPenales.BiometricoServiceReference;
using System.Windows;
using SSP.Controlador.Catalogo.Justicia;
//using MvvmFramework;

namespace ControlPenales
{
    partial class EMILiberadoViewModel
    {
        #region Busqueda
        private string apellidoPaternoBuscar = string.Empty;
        public string ApellidoPaternoBuscar
        {
            get { return apellidoPaternoBuscar; }
            set { apellidoPaternoBuscar = value; OnPropertyChanged("ApellidoPaternoBuscar"); }
        }
        private string apellidoMaternoBuscar = string.Empty;
        public string ApellidoMaternoBuscar
        {
            get { return apellidoMaternoBuscar; }
            set { apellidoMaternoBuscar = value; OnPropertyChanged("ApellidoMaternoBuscar"); }
        }
        private string nombreBuscar = string.Empty;
        public string NombreBuscar
        {
            get { return nombreBuscar; }
            set
            {
                nombreBuscar = value; OnPropertyChanged("NombreBuscar");
            }
        }
        private short? anioBuscar;
        public short? AnioBuscar
        {
            get { return anioBuscar; }
            set { anioBuscar = value; OnPropertyChanged("AnioBuscar"); }
        }
        private int? folioBuscar;
        public int? FolioBuscar
        {
            get { return folioBuscar; }
            set { folioBuscar = value; OnPropertyChanged("FolioBuscar"); }
        }

        private bool nuevoProcesoEnabled = false;
        public bool NuevoProcesoEnabled
        {
            get { return nuevoProcesoEnabled; }
            set { nuevoProcesoEnabled = value; OnPropertyChanged("NuevoProcesoEnabled"); }
        }
        
        private bool seleccionarProcesoEnabled = false;
        public bool SeleccionarProcesoEnabled
        {
            get { return seleccionarProcesoEnabled; }
            set { seleccionarProcesoEnabled = value; OnPropertyChanged("SeleccionarProcesoEnabled"); }
        }
        #endregion

        #region Botones
        private bool crearNuevoExpedienteEnabled;
        public bool CrearNuevoExpedienteEnabled
        {
            get { return crearNuevoExpedienteEnabled; }
            set { crearNuevoExpedienteEnabled = value; OnPropertyChanged("CrearNuevoExpedienteEnabled"); }
        }
        private string textBotonSeleccionarIngreso = "Seleccionar ingreso";
        public string TextBotonSeleccionarIngreso
        {
            get { return textBotonSeleccionarIngreso; }
            set { textBotonSeleccionarIngreso = value; OnPropertyChanged("TextBotonSeleccionarIngreso"); }
        }
        #endregion

        #region Buscar
        private byte[] imagenIngreso;
        public byte[] ImagenIngreso
        {
            get { return imagenIngreso; }
            set
            {
                imagenIngreso = value;
                OnPropertyChanged("ImagenIngreso");
            }
        }
        
        private byte[] imagenImputado;
        public byte[] ImagenImputado
        {
            get { return imagenImputado; }
            set
            {
                imagenImputado = value;
                OnPropertyChanged("ImagenImputado");
            }
        }
        
        private bool emptyExpedienteVisible;
        public bool EmptyExpedienteVisible
        {
            get { return emptyExpedienteVisible; }
            set { emptyExpedienteVisible = value; OnPropertyChanged("EmptyExpedienteVisible"); }
        }
        
        private bool emptyIngresoVisible;
        public bool EmptyIngresoVisible
        {
            get { return emptyIngresoVisible; }
            set { emptyIngresoVisible = value; OnPropertyChanged("EmptyIngresoVisible"); }
        }
        
        private RangeEnabledObservableCollection<IMPUTADO> listExpediente;
        public RangeEnabledObservableCollection<IMPUTADO> ListExpediente
        {
            get { return listExpediente; }
            set
            {
                listExpediente = value;
                OnPropertyChanged("ListExpediente");
            }
        }

        private Visibility emptyProceso = Visibility.Visible;
        public Visibility EmptyProceso
        {
            get { return emptyProceso; }
            set { emptyProceso = value; OnPropertyChanged("EmptyProceso"); }
        }


        private PROCESO_LIBERTAD auxProcesoLibertad;

        private PROCESO_LIBERTAD selectedProcesoLibertad;
        public PROCESO_LIBERTAD SelectedProcesoLibertad
        {
            get { return selectedProcesoLibertad; }
            set { selectedProcesoLibertad = value;
            if (value != null)
                SeleccionarProcesoEnabled = true;
            else
                SeleccionarProcesoEnabled = false;
                OnPropertyChanged("SelectedProcesoLibertad"); }
        }

        private IMPUTADO selectExpediente;
        public IMPUTADO SelectExpediente
        {
            get { return selectExpediente; }
            set
            {
                selectExpediente = value;
                if (value != null)
                {
                    var foto = value.IMPUTADO_BIOMETRICO.Where(w => w.ID_TIPO_BIOMETRICO == (short)enumTipoBiometrico.FOTO_FRENTE_REGISTRO).SingleOrDefault();
                    if (foto != null)
                        ImagenInterno = foto.BIOMETRICO;
                    else
                    {
                        if (value.INGRESO != null && value.INGRESO.Count>0)
                        {
                            var ingreso = value.INGRESO.OrderByDescending(w => w.ID_INGRESO).FirstOrDefault();
                            if (ingreso != null)
                            {
                                var fotoIngreso = ingreso.INGRESO_BIOMETRICO.Where(w => w.ID_TIPO_BIOMETRICO == (short)enumTipoBiometrico.FOTO_FRENTE_REGISTRO).SingleOrDefault();
                                if (fotoIngreso != null)
                                    ImagenInterno = fotoIngreso.BIOMETRICO;
                                else
                                    ImagenInterno = new Imagenes().getImagenPerson();
                            }
                        }
                    }
                    if (value.PROCESO_LIBERTAD != null)
                    {
                        if (value.PROCESO_LIBERTAD.Count == 0)
                        {
                            EmptyProceso = Visibility.Visible;
                        }
                        else
                            EmptyProceso = Visibility.Collapsed;
                    }
                    else
                        EmptyProceso = Visibility.Visible;
                    //var l = new cLiberado().Obtener(value.ID_CENTRO, value.ID_ANIO, value.ID_IMPUTADO);
                    //if (l != null)
                    //{
                    //    LstLiberadoMJ = new ObservableCollection<LIBERADO_MEDIDA_JUDICIAL>(l.LIBERADO_MEDIDA_JUDICIAL);
                    //}
                    //else
                    //    LstLiberadoMJ = null;
                }
                else
                {
                    //LstLiberadoMJ = null;
                    EmptyProceso = Visibility.Visible;
                    ImagenInterno = new Imagenes().getImagenPerson();
                }
                //SelectMJ = null;
                //EmptyMJVisible = LstLiberadoMJ != null ? LstLiberadoMJ.Count > 0 ? Visibility.Collapsed : Visibility.Visible : Visibility.Visible;
                OnPropertyChanged("SelectExpediente");
            }
        }
        
        private INGRESO selectIngreso;
        public INGRESO SelectIngreso
        {
            get { return selectIngreso; }
            set
            {
                if (selectIngreso == null)
                {
                    OnPropertyChanged("SelectIngreso");
                    return;
                }
                if (SelectIngreso.INGRESO_BIOMETRICO.Where(w => w.ID_TIPO_BIOMETRICO == (short)enumTipoBiometrico.FOTO_FRENTE_REGISTRO && w.ID_FORMATO == (short)enumTipoFormato.FMTO_JPG).Any())
                    ImagenImputado = SelectIngreso.INGRESO_BIOMETRICO.Where(w => w.ID_TIPO_BIOMETRICO == (short)enumTipoBiometrico.FOTO_FRENTE_REGISTRO && w.ID_FORMATO == (short)enumTipoFormato.FMTO_JPG).FirstOrDefault().BIOMETRICO;
                else
                    ImagenImputado = new Imagenes().getImagenPerson();
                if (selectIngreso.INGRESO_BIOMETRICO.Any(w => w.ID_TIPO_BIOMETRICO == (short)enumTipoBiometrico.FOTO_FRENTE_SEGUIMIENTO && w.ID_FORMATO == (short)enumTipoFormato.FMTO_JPG))
                {
                    ImagenIngreso = selectIngreso.INGRESO_BIOMETRICO.Where(w => w.ID_TIPO_BIOMETRICO == (short)enumTipoBiometrico.FOTO_FRENTE_SEGUIMIENTO && w.ID_FORMATO == (short)enumTipoFormato.FMTO_JPG).FirstOrDefault().BIOMETRICO;
                    
                }
                else
                    ImagenIngreso = new Imagenes().getImagenPerson();

                OnPropertyChanged("SelectIngreso");
            }
        }
        
        private ImageSource _HuellaBusqueda;
        public ImageSource HuellaBusqueda
        {
            get { return _HuellaBusqueda; }
            set
            {
                _HuellaBusqueda = value;
                OnPropertyChanged("HuellaBusqueda");
            }
        }

        //VARIABLES SEGMANTACION
        private int Pagina { get; set; }
        private bool SeguirCargando { get; set; }

        private LIBERADO_MEDIDA_JUDICIAL selectMJ;
        public LIBERADO_MEDIDA_JUDICIAL SelectMJ
        {
            get { return selectMJ; }
            set { selectMJ = value;
            if (value != null)
                SelectMJEnabled = true;
            else
                SelectMJEnabled = false;
                OnPropertyChanged("SelectMJ"); }
        }

        private Visibility emptyMJVisible = Visibility.Collapsed;
        public Visibility EmptyMJVisible
        {
            get { return emptyMJVisible; }
            set { emptyMJVisible = value; OnPropertyChanged("EmptyMJVisible"); }
        }

        private ObservableCollection<LIBERADO_MEDIDA_JUDICIAL> lstLiberadoMJ;
        public ObservableCollection<LIBERADO_MEDIDA_JUDICIAL> LstLiberadoMJ
        {
            get { return lstLiberadoMJ; }
            set { lstLiberadoMJ = value; OnPropertyChanged("LstLiberadoMJ"); }
        }

        private byte[] imagenInterno = new Imagenes().getImagenPerson();
        public byte[] ImagenInterno
        {
            get { return imagenInterno; }
            set { imagenInterno = value; OnPropertyChanged("ImagenInterno"); }
        }
        #endregion
    }
}
