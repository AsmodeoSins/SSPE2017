using SSP.Servidor;
using SSP.Modelo;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;
using System.Threading.Tasks;

namespace ControlPenales
{
    partial class EMILiberadoViewModel
    {
        #region [Actividades]
        private ObservableCollection<EMI_TIPO_ACTIVIDAD> lstTipoActividad;// = new ObservableCollection<EMI_TIPO_ACTIVIDAD>(new SSP.Controlador.Catalogo.Justicia.EstudioMI.cEMITipoActividad().Obtener());
        private ObservableCollection<EMI_ACTIVIDAD> lstEmiActividades;
        private ObservableCollection<AuxiliarActividades> lstAuxiliarActividades;
        private ObservableCollection<EMI_ESTATUS_PROGRAMA> lstEstatusPrograma;
        private EMI_ACTIVIDAD _SelectedActividad;
        private EMI_ACTIVIDAD _PopUpActividad = new EMI_ACTIVIDAD();
        private short _RowSelectedIndex;
        #endregion

        #region listas
        public ObservableCollection<EMI_TIPO_ACTIVIDAD> LstTipoActividad
        {
            get { return lstTipoActividad; }
            set
            {
                lstTipoActividad = value;
                OnPropertyChanged("LstTipoActividad");
            }
        }
        public ObservableCollection<EMI_ACTIVIDAD> LstEmiActividades
        {
            get { return lstEmiActividades; }
            set
            {
                lstEmiActividades = value;
                OnPropertyChanged("LstEmiActividades");
            }
        }
        public ObservableCollection<AuxiliarActividades> LstAuxiliarActividades
        {
            get { return lstAuxiliarActividades; }
            set
            {
                lstAuxiliarActividades = value;
                OnPropertyChanged("LstAuxiliarActividades");

            }
        }
        public ObservableCollection<EMI_ESTATUS_PROGRAMA> LstEstatusPrograma
        {
            get { return lstEstatusPrograma; }
            set
            {
                lstEstatusPrograma = value;
                OnPropertyChanged("LstEstatusPrograma");
            }
        }
        #endregion
        #region Variables
        public EMI_ACTIVIDAD SelectedActividad
        {
            get { return _SelectedActividad; }
            set
            {
                if (_SelectedActividad == null)
                    _SelectedActividad = value;
                else if (_SelectedActividad != null)
                {
                    _SelectedActividad = null;
                    OnPropertyChanged("SelectedActividad");
                    _SelectedActividad = value;
                    OnPropertyChanged("SelectedActividad");
                }

                OnPropertyChanged("SelectedActividad");
                //RowSelectedIndex = (short)-1;
                //PopUpActividad = value;
                if (SelectedActividad != null)
                {

                    var isInDB = new SSP.Controlador.Catalogo.Justicia.EstudioMI.cEMIActividad().GetData(w => w.ID_EMI == emiActual.ID_EMI && w.ID_EMI_CONS == emiActual.ID_EMI_CONS && w.ID_CONSEC == SelectedActividad.ID_CONSEC);
                    EliminarItemMenu = isInDB.Count() > 0 ? false : true;
                }
            }
        }
        public short RowSelectedIndex { get { return _RowSelectedIndex; } set { _RowSelectedIndex = value; OnPropertyChanged("RowSelectedIndex"); } }
        public EMI_ACTIVIDAD PopUpActividad
        {
            get { return _PopUpActividad; }
            set
            {
                _PopUpActividad = value;
                OnPropertyChanged("PopUpActividad");
            }
        }
        public string popupDescrActividades
        {
            get {
                if (PopUpActividad != null)
                    return PopUpActividad.DESCRIPCION_ACTIVIDADES;
                else
                    return string.Empty;
            }
            set
            {
                if (PopUpActividad != null)   
                    PopUpActividad.DESCRIPCION_ACTIVIDADES = value;
                OnPropertyChanged("popupDescrActividades");
            }
        }
        public short? popupAnioActividad
        {
            get {
                if (PopUpActividad != null)
                    return PopUpActividad.ANO_ACTIVIDADES;
                else
                    return null;
            }
            set
            {
                if (PopUpActividad != null)
                PopUpActividad.ANO_ACTIVIDADES = value;
                OnPropertyChanged("popupAnioActividad");
            }
        }
        public short? popupNoProg
        {
            get {
                if (PopUpActividad != null)
                    return PopUpActividad.PROGRAMA_TERMINADO;
                else
                    return null;
            }
            set
            {
                if (PopUpActividad != null)
                PopUpActividad.PROGRAMA_TERMINADO = value;
                OnPropertyChanged("popupNoProg");
            }
        }
        public string popupDuracionActividad
        {
            get {
                if (PopUpActividad != null)
                    return PopUpActividad.DURACION_ACTIVIDADES;
                else
                    return string.Empty;
            }
            set
            {
                if (PopUpActividad != null)
                PopUpActividad.DURACION_ACTIVIDADES = value;
                OnPropertyChanged("popupDuracionActividad");
            }
        }
        public short? popupEstatusPrograma
        {
            get {
                if (PopUpActividad != null)
                    return PopUpActividad.ESTATUS_ACTIVIDADES;
                else
                    return null;
            }
            set
            {
                if (PopUpActividad != null)
                { 
                    PopUpActividad.ESTATUS_ACTIVIDADES = value;
                    PopUpActividad.EMI_ESTATUS_PROGRAMA = new SSP.Controlador.Catalogo.Justicia.EstudioMI.cEMIEstatusPrograma().GetData(w=>w.ID_ESTATUS == value).SingleOrDefault();
                }
                OnPropertyChanged("popupEstatusPrograma");
            }
        }
        public short popupTipoActividad
        {
            get {
                if (PopUpActividad != null)
                    return PopUpActividad.ID_EMI_ACTIVIDAD;
                else
                    return 0;
            }
            set
            {
                if (PopUpActividad != null)
                PopUpActividad.ID_EMI_ACTIVIDAD = value;
                OnPropertyChanged("popupTipoActividad");
            }
        }
        #endregion
    }

    public class AuxiliarActividades : ValidationViewModelBase
    {
        //private ObservableCollection<EMI_ACTIVIDAD> _Actividades;
        //public short Id { get; set; }
        //public string Actividad { get; set; }
        //public ObservableCollection<EMI_ACTIVIDAD> Actividades
        //{
        //    get { return _Actividades; }
        //    set
        //    {
        //        _Actividades = value;
        //        OnPropertyChanged("Actividades");
        //    }
        //}
        //public bool IsGridEmpty { get; set; }
        //public bool IsGridVisible { get; set; }
        //public short SelectedIndexActividades { get; set; }


      
    }
}