using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Input;

namespace ControlPenales
{
    public class SeguimientoMedidasJudicialesViewModel : ValidationViewModelBase, IPageViewModel
    {
        public delegate void ParameterChange(string parameter);
        public ParameterChange OnParameterChange { get; set; }

        #region variables
        public string Name
        {
            get
            {
                return "seguimiento_medidas_judiciales";
            }
        }

        /*PRUEBAS*/
        private List<expediente> expedientes;

        public List<expediente> Expedientes
        {
            get { return expedientes; }
            set { expedientes = value; OnPropertyChanged("Expedientes"); }
        }

        private expediente expediente;

        public expediente Expediente
        {
            get { return expediente; }
            set { expediente = value; OnPropertyChanged("Expediente"); }
        }
        /*FIN PRUEBAS*/

        private bool consultaVisible;

        public bool ConsultaVisible
        {
            get { return consultaVisible; }
            set { consultaVisible = value; OnPropertyChanged("ConsultaVisible"); }
        }
        private bool expedienteVisible;

        public bool ExpedienteVisible
        {
            get { return expedienteVisible; }
            set { expedienteVisible = value; OnPropertyChanged("ExpedienteVisible"); }
        }
        #endregion

        #region constructor
        public SeguimientoMedidasJudicialesViewModel()
        {
          
        }

        #endregion

        #region metodos

        
        void IPageViewModel.inicializa()
        {
            ConsultaVisible = true;
            ExpedienteVisible = !ConsultaVisible;

            /*MODELO PRUEBAS*/
            Expediente = new ControlPenales.expediente();
            Expedientes = new List<expediente>();

            Expedientes.Add(new expediente
            {
                asesor = "LIC.JUAN ESCUTIA",
                aud = "1",
                docs_pje = "DOCUMENTO1",
                fecha_inicio = "26/02/2015",
                lugar = "SSP",
                medidas = "0",
                notif_pgje = "1",
                nuc = "012345",
                orie = "0",
                otros_in = "0",
                otros_out = "0",
                prevpgje = "1",
                res = "0",
                scan_medidas = "1",
                seg = "0",
                valid_dom = "0",
                visitas = "0",
                imputado = new ModeloPrueba
                    {
                        apellido_paterno = "PEREZ",
                        apellido_materno = "LOPEZ",
                        nombre = "JUAN",
                        causa_penal = "03/2015",
                        estatus = "ACTIVO",
                        tipo_registro = "PERSONA",
                        unidad = "MEXICALI - DEPARTAMENTO DE VIGILANCIA",
                        foto = "/ControlPenales;component/Imagen/juanperez.jpg",
                        fecha_registro = Fechas.GetFechaDateServer,
                        edad = 40,
                        fecha_nacimiento = Fechas.GetFechaDateServer.AddYears(-40),
                        pais_nacimiento = "MEXICO",
                        estado_nacimiento = "BAJA CALIFORNIA",
                        municipio_nacimiento = "MEXICALI",
                        pais_domicilio = "MEXICO",
                        estado_domicilio = "BAJA CALIFORNIA",
                        municipio_domicilio = "MEXICALI",
                        calle_domicilio = "DE LOS FAISANES",
                        numero_exterior_domicilio = "100",
                        codigo_postal_domicilio = "21140",
                        colonia_domicilio = "FRACC. CONDOR",
                        telefono_casa = "5575859",
                        telefono_movil = "6861474948",
                        radio = "152*445685",
                        correo_electronico = "JUAN.PEREZ@GMAIL.COM",
                        observaciones = "PERSONA VIOLENTA",
                        mf_estatura = "180",
                        mf_peso = "75",
                        mf_complexion = "DELGADA",
                        mf_tez = "MORENO CLARO",
                        mf_pelo = "LACIO",
                        mf_color_pelo = "OSCURO",
                        mf_ojos = "CAFES"
                    }
            });
        }
        
        private void clickSwitch(Object obj)
        {
            switch (obj.ToString())
            { 
                case "consulta":
                    ExpedienteVisible = true;
                    ConsultaVisible = !ExpedienteVisible;
                break;
            }
        }

        private void selectImputado(Object obj) 
        {
            Expediente = (expediente)obj;
            ExpedienteVisible = true;
            ConsultaVisible = !ExpedienteVisible;
        }
        #endregion

        #region command
        private ICommand _onClick;
        public ICommand OnClick
        {
            get
            {
                return _onClick ?? (_onClick = new RelayCommand(clickSwitch));
            }
        }

        private ICommand _onClickSelect;
        public ICommand OnClickSelect
        {
            get
            {
                return _onClickSelect ?? (_onClickSelect = new RelayCommand(selectImputado));
            }
        }
        #endregion

    }
}
