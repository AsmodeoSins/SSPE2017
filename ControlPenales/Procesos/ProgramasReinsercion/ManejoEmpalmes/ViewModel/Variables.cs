using SSP.Servidor;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Controls;

namespace ControlPenales
{
    public partial class ManejoEmpalmesViewModel : ViewModelBase
    {
        /* [descripcion de clase]
         * clase donde se definen las variables para el modulo manejo empalmes
         * 
         * la propiedad importante es: SelectedParticipante
         * 
         */

        #region [Propiedades]
        ObservableCollection<MaestroEmpalme> _ListEmpalmesParticipantes;
        public ObservableCollection<MaestroEmpalme> ListEmpalmesParticipantes
        {
            get { return _ListEmpalmesParticipantes; }
            set
            {
                _ListEmpalmesParticipantes = value;
                OnPropertyChanged("ListEmpalmesParticipantes");
            }
        }

        MaestroEmpalme _SelectedParticipante;
        public MaestroEmpalme SelectedParticipante
        {
            get { return _SelectedParticipante; }
            set
            {
                _SelectedParticipante = value;
                OnPropertyChanged("SelectedParticipante");


                if (value != null)
                {
                    ImagenParticipante = SelectedParticipante.ImagenParticipante;
                    LoadEmpalmes(DynamicWrapPanel, value.PorHora.Where(w => w.Entity.GRUPO_PARTICIPANTE.GRUPO != null).ToList(), value.PorHora.Where(w => w.Entity.GRUPO_PARTICIPANTE.GRUPO != null).GroupBy(g => g.Entity.GRUPO_PARTICIPANTE.GRUPO).Select(s => s.Key).OrderBy(o => o.ACTIVIDAD.PRIORIDAD).ToList());
                    ObtenerCursosAprovadosTotalHoras(value.Entity.GRUPO_PARTICIPANTE, value.Entity.GRUPO_PARTICIPANTE.Count);
                }
                else
                {
                    HorasTratamiento = "0/0";
                    MaxValueProBar = 1;
                    CantidadActividadesAprovadas = 0;
                    HorasTratamiento = "0/0";
                    AvanceTratamiento = "0/0";
                    varauxSentencia = string.Empty;
                    ImagenParticipante = new Imagenes().getImagenPerson();
                }
            }
        }

        WrapPanel DynamicWrapPanel;
        Grid DynamicGrid;

        int _MaxValueProBar = 1;
        public int MaxValueProBar
        {
            get { return _MaxValueProBar; }
            set
            {
                _MaxValueProBar = value;
                OnPropertyChanged("MaxValueProBar");
            }
        }
        int _CantidadActividadesAprovadas = 0;
        public int CantidadActividadesAprovadas
        {
            get { return _CantidadActividadesAprovadas; }
            set
            {
                _CantidadActividadesAprovadas = value;
                OnPropertyChanged("CantidadActividadesAprovadas");
            }
        }
        string _HorasTratamiento = "0/0";
        public string HorasTratamiento
        {
            get { return _HorasTratamiento; }
            set
            {
                _HorasTratamiento = value;
                OnPropertyChanged("HorasTratamiento");
            }
        }
        string _AvanceTratamiento = "0/0";
        public string AvanceTratamiento
        {
            get { return _AvanceTratamiento; }
            set
            {
                _AvanceTratamiento = value;
                OnPropertyChanged("AvanceTratamiento");
            }
        }
        string varauxSentencia;
        #endregion

        #region [CONFIGURACION PERMISOS]
        private bool banderaConsulta;

        private bool _agregarMenuEnabled;
        public bool AgregarMenuEnabled
        {
            get { return _agregarMenuEnabled; }
            set { _agregarMenuEnabled = value; OnPropertyChanged("AgregarMenuEnabled"); }
        }

        private bool _editarMenuEnabled;
        public bool EditarMenuEnabled
        {
            get { return _editarMenuEnabled; }
            set { _editarMenuEnabled = value; OnPropertyChanged("EditarMenuEnabled"); }
        }

        private bool _listaEmpalmeEnabled;
        public bool ListaEmpalmeEnabled
        {
            get { return _listaEmpalmeEnabled; }
            set { _listaEmpalmeEnabled = value; OnPropertyChanged("ListaEmpalmeEnabled"); }
        }

        private bool _eliminarMenuEnabled;
        public bool EliminarMenuEnabled
        {
            get { return _eliminarMenuEnabled; }
            set { _eliminarMenuEnabled = value; OnPropertyChanged("EliminarMenuEnabled"); }
        }
        #endregion

        byte[] _ImagenParticipante = new Imagenes().getImagenPerson();
        public byte[] ImagenParticipante
        {
            get { return _ImagenParticipante; }
            set
            {
                _ImagenParticipante = value;
                OnPropertyChanged("ImagenParticipante");
            }
        }
    }

    public class MaestroEmpalme
    {
        public INGRESO Entity { get; set; }
        public string NOMBRE { get; set; }
        public string PATERNO { get; set; }
        public string MATERNO { get; set; }
        public short ID_CENTRO { get; set; }
        public short ID_ANIO { get; set; }
        public int ID_IMPUTADO { get; set; }
        public short ID_INGRESO { get; set; }
        public string UBICACION { get; set; }
        public string PLANIMETRIA { get; set; }
        public string PLANIMETRIACOLOR { get; set; }
        public string DELITO { get; set; }
        public string RESTANTE { get; set; }
        public string SENTENCIA { get; set; }
        public byte[] ImagenParticipante { get; set; }
        public List<DetalleEmpalmeHora> PorHora { get; set; }
    }

    public class DetalleEmpalmeHora
    {
        public GRUPO_ASISTENCIA Entity { get; set; }
        public short ID_GRUPO { get; set; }
        public short ID_EJE { get; set; }
        public string GRUPO { get; set; }
        public DateTime? HORA_INICIO { get; set; }
        public DateTime? HORA_TERMINO { get; set; }
        public DateTime? FEC_REGISTRO { get; set; }
        public short? PRIORIDAD { get; set; }
        public string DEPARTAMENTO { get; set; }
        public string PROGRAMA { get; set; }
        public string ACTIVIDAD { get; set; }
        public short IDGRUPOHORARIO { get; set; }
        public bool Check { get; set; }
    }
}
