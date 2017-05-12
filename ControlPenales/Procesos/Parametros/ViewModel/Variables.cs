using SSP.Servidor;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;

namespace ControlPenales
{
    partial class AdministracionParametrosViewModel
    {
        #region MAX_LENGHT
        private int _Clave_Max;

        public int Clave_Max
        {
            get { return _Clave_Max; }
            set { _Clave_Max = value; OnPropertyChanged("Clave_Max"); }
        }

        private int _Valor_MAX;

        public int Valor_MAX
        {
            get { return _Valor_MAX; }
            set { _Valor_MAX = value; OnPropertyChanged("Valor_MAX"); }
        }
        private int _ValorNumerico_MAX;

        public int ValorNumerico_MAX
        {
            get { return _ValorNumerico_MAX; }
            set { _ValorNumerico_MAX = value; OnPropertyChanged("ValorNumerico_MAX"); }
        }
        private int _Descr_Max;

        public int Descr_Max
        {
            get { return _Descr_Max; }
            set { _Descr_Max = value; OnPropertyChanged("Descr_Max"); }
        }

        #endregion

        #region [Datos de Busqueda]
        private string _DescricionBuscar;

        public string DescricionBuscar
        {
            get { return _DescricionBuscar; }
            set { _DescricionBuscar = value; OnPropertyChanged("DescricionBuscar"); }
        }
        private string _ClaveBuscar;

        public string ClaveBuscar
        {
            get { return _ClaveBuscar; }
            set { _ClaveBuscar = value; OnPropertyChanged("ClaveBuscar"); }
        }
        private string _ValorBuscar;

        public string ValorBuscar
        {
            get { return _ValorBuscar; }
            set { _ValorBuscar = value;
           
                
                OnPropertyChanged("ValorBuscar"); }
        }

        private int? _ValorNumericoBuscar;

        public int? ValorNumericoBuscar
        {
            get { return _ValorNumericoBuscar; }
            set { _ValorNumericoBuscar = value; OnPropertyChanged("ValorNumericoBuscar"); }
        }
        #endregion

        //NOTA: NO BORRAR YA QUE DESPUES DE DESCOMENTARA PARA LOS PERMISOSO DE USUARIO
        #region [CONFIGURACION PERMISOS]
        private bool _agregarMenuEnabled = true;
        public bool AgregarMenuEnabled
        {
            get { return _agregarMenuEnabled; }
            set { _agregarMenuEnabled = value; OnPropertyChanged("AgregarMenuEnabled"); }
        }

        private bool _editarEnabled = true;
        public bool EditarEnabled
        {
            get { return _editarEnabled; }
            set { _editarEnabled = value; OnPropertyChanged("EditarEnabled"); }
        }

        private bool _agregarVisible = true;
        public bool AgregarVisible
        {
            get { return _agregarVisible; }
            set { _agregarVisible = value; OnPropertyChanged("AgregarVisible"); }
        }

        private bool _editarVisible = true;
        public bool EditarVisible
        {
            get { return _editarVisible; }
            set { _editarVisible = value; OnPropertyChanged("EditarVisible"); }
        }

        private bool _descrEnabled = true;
        public bool DescrEnabled
        {
            get { return _descrEnabled; }
            set { _descrEnabled = value; OnPropertyChanged("DescrEnabled"); }
        }

        private bool _buscarEnabled = true;
        public bool BuscarEnabled
        {
            get { return _buscarEnabled; }
            set { _buscarEnabled = value; OnPropertyChanged("BuscarEnabled"); }
        }

        private bool _idEnabled;
        public bool IdEnabled
        {
            get { return _idEnabled; }
            set { _idEnabled = value; OnPropertyChanged("IdEnabled"); }
        }

        private bool _valorEnabled;
        public bool ValorEnabled
        {
            get { return _valorEnabled; }
            set { _valorEnabled = value; OnPropertyChanged("ValorEnabled"); }
        }

        private bool _valorNumHabilitado;
        public bool ValorNumHabilitado
        {
            get { return _valorNumHabilitado; }
            set { _valorNumHabilitado = value; OnPropertyChanged("ValorNumHabilitado"); }
        }
        #endregion

        private string _AGREGAR_EDITAR;

        public string AGREGAR_EDITAR
        {
            get { return _AGREGAR_EDITAR; }
            set { _AGREGAR_EDITAR = value; OnPropertyValidateChanged("AGREGAR_EDITAR"); }
        }

        private bool _claveEnable;

        public bool ClaveEnable
        {
            get { return _claveEnable; }
            set { _claveEnable = value; OnPropertyChanged("ClaveEnable"); }
        }

        private string _lblNombreArchivo;

        public string LblNombreArchivo
        {
            get { return _lblNombreArchivo; }
            set { _lblNombreArchivo = value; OnPropertyChanged("LblNombreArchivo"); }
        }

        private ObservableCollection<PARAMETRO> _LstParametros;

        public ObservableCollection<PARAMETRO> LstParametros
        {
            get { return _LstParametros; }
            set { _LstParametros = value;
            OnPropertyChanged("LstParametros");
            }
        }
        private PARAMETRO _SelectParametros;

        public PARAMETRO SelectParametros
        {
            get { return _SelectParametros; }
            set { _SelectParametros = value; OnPropertyChanged("SelectParametros"); }
        }

        
        private int? _ValorNumerico;

        public int? ValorNumerico
        {
            get { return _ValorNumerico; }
            set { _ValorNumerico = value;

            
                OnPropertyChanged("ValorNumerico"); }
        }
        private ObservableCollection<CENTRO> _LstCentros;

        public ObservableCollection<CENTRO> LstCentros
        {
            get { return _LstCentros; }
            set { _LstCentros = value; OnPropertyChanged("LstCentros"); }
        }

        private CENTRO _SelectedCentro;


        public CENTRO SelectedCentro
        {
            get { return _SelectedCentro; }
            set { _SelectedCentro = value; OnPropertyChanged("SelectedCentro"); }
        }
        private short? _SelectCentro;

        public short? SelectCentro
        {
            get { return _SelectCentro; }
            set { _SelectCentro = value; OnPropertyChanged("SelectCentro"); }
        }

        #region [Parametros ventana Emergente]
        private string _Clave;

        public string Clave
        {
            get { return _Clave; }
            set { _Clave = value; OnPropertyChanged("Clave"); }
        }

        private string _Valor;

        public string Valor
        {
            get { return _Valor; }
            set { _Valor = value; OnPropertyChanged("Valor"); }
        }

        private string _DESCR;

        public string DESCR
        {
            get { return _DESCR; }
            set { 
            if (!string.IsNullOrEmpty(value))
            {
                value = value.Trim();
            }
            _DESCR = value;
                OnPropertyChanged("DESCR"); }
        }


        private byte[] _ArchSelect;

        public byte[] ArchSelect
        {
            get { return _ArchSelect; }
            set { _ArchSelect = value;

        
                OnPropertyChanged("ArchSelect"); }
        }

        #endregion

        #region [Validacion Popup Enable]

     

        private string _SelectTipoParametro;

        public string SelectTipoParametro
        {
            get { return _SelectTipoParametro; }
            set { _SelectTipoParametro = value;


            //if (!string.IsNullOrEmpty(value) ? value != "SELECCIONE" : false)
            //{


            //    ValidacionSelectTipoParametro();
            //}
                OnPropertyChanged("SelectTipoParametro"); }
        }

        private async void ValidacionSelectTipoParametro()
        {

            await StaticSourcesViewModel.CargarDatosMetodoAsync(ValidacionSelectTipoParametroAsync);
    }

        private void ValidacionSelectTipoParametroAsync()
        {
            switch (_SelectTipoParametro)
            {

                case "VALOR":
                    ArchSelect = null;
                    ValorNumerico = null;
                   
                    base.RemoveRule("Valor"); OnPropertyChanged("Valor");
                    base.AddRule(() => Valor, () => !string.IsNullOrEmpty(Valor), "VALOR ES REQUERIDO!");
                    OnPropertyChanged("Valor");
                    base.RemoveRule("ArchSelect"); OnPropertyChanged("ArchSelect");
                    base.RemoveRule("ValorNumerico"); OnPropertyChanged("ValorNumerico");
                    break;
                case "VALOR NUMÉRICO":

                    Valor = string.Empty;
                    ArchSelect = null;
                   
                    base.RemoveRule("ValorNumerico"); OnPropertyChanged("ValorNumerico");
                    base.AddRule(() => ValorNumerico, () => ValorNumerico != null, "VALOR NUMÉRICO ES REQUERIDO!");
                    OnPropertyChanged("ValorNumerico");
                    base.RemoveRule("Valor"); OnPropertyChanged("Valor");
                    base.RemoveRule("ArchSelect"); OnPropertyChanged("ArchSelect");

                    break;
                case "CONTENIDO":
                    Valor = string.Empty;
                    ValorNumerico = null;
                    
                    base.RemoveRule("ArchSelect"); OnPropertyChanged("ArchSelect");
                    base.AddRule(() => ArchSelect, () => ArchSelect != null, "ARCHIVO ES REQUERIDO!");
                    OnPropertyChanged("ArchSelect");
                    base.RemoveRule("Valor"); OnPropertyChanged("Valor");
                    base.RemoveRule("ValorNumerico"); OnPropertyChanged("ValorNumerico");
                    break;
                default:
                    break;
            }

        }

        //private bool _ArchivoEnable=false;
        //public bool ArchivoEnable
        //{
        //    get { return _ArchivoEnable; }
        //    set { _ArchivoEnable = value; OnPropertyChanged("ArchivoEnable"); }
        //}


        private bool _EnableCentro;

        public bool EnableCentro
        {
            get { return _EnableCentro; }
            set { _EnableCentro = value; OnPropertyChanged("EnableCentro"); }
        }

        //private bool _ValorNumEnable = false;

        //public bool ValorNumEnable
        //{
        //    get { return _ValorNumEnable; }
        //    set { _ValorNumEnable = value; OnPropertyChanged("ValorNumEnable"); }
        //}

        //private bool _ValorEnable = false;

        //public bool ValorEnable
        //{
        //    get { return _ValorEnable; }
        //    set { _ValorEnable = value; OnPropertyChanged("ValorEnable"); }
        //}
        #endregion

        #region Privilegios
        private bool pInsertar = false;
        private bool pEditar = false;
        private bool pConsultar = false;
        private bool pImprimir = false;
        #endregion
    }
}
