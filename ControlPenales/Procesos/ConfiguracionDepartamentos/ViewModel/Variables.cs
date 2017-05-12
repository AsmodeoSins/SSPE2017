using ControlPenales.Clases.ConfigDepartamentos;
using LinqKit;
using SSP.Servidor;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;


namespace ControlPenales
{
    partial class ConfiguracionDepartamentosViewModel
    {

        private string _Guardar_Editar;

        public string Guardar_Editar
        {
            get { return _Guardar_Editar; }
            set { _Guardar_Editar = value; OnPropertyChanged("Guardar_Editar"); }
        }

        private bool _OperacionActivaEnable = true;

        public bool OperacionActivaEnable
        {
            get { return _OperacionActivaEnable; }
            set { _OperacionActivaEnable = value; OnPropertyChanged("OperacionActivaEnable"); }
        }
        private System.Windows.Visibility _GroupCentroSeleccionado = System.Windows.Visibility.Hidden;
        public System.Windows.Visibility GroupCentroSeleccionado
        {
            get { return _GroupCentroSeleccionado; }
            set { _GroupCentroSeleccionado = value; OnPropertyChanged("GroupCentroSeleccionado"); }
        }
        private System.Windows.Visibility _BotonGuardarVisibilty = System.Windows.Visibility.Hidden;

        public System.Windows.Visibility BotonGuardarVisibilty
        {
            get { return _BotonGuardarVisibilty; }
            set { _BotonGuardarVisibilty = value; OnPropertyChanged("BotonGuardarVisibilty"); }
        }



        private System.Windows.Visibility _VisibilityBotonCancelar = System.Windows.Visibility.Hidden;

        public System.Windows.Visibility VisibilityBotonCancelar
        {
            get { return _VisibilityBotonCancelar; }
            set { _VisibilityBotonCancelar = value; OnPropertyChanged("VisibilityBotonCancelar"); }
        }



        private System.Windows.Visibility _VisibilityCoordinadoresDepSelect = System.Windows.Visibility.Hidden;

        public System.Windows.Visibility VisibilityCoordinadoresDepSelect
        {
            get { return _VisibilityCoordinadoresDepSelect; }
            set { _VisibilityCoordinadoresDepSelect = value; OnPropertyChanged("VisibilityCoordinadoresDepSelect"); }
        }



        private ObservableCollection<DEPARTAMENTO> _LstDepartamentos;

        public ObservableCollection<DEPARTAMENTO> LstDepartamentos
        {
            get { return _LstDepartamentos; }
            set { _LstDepartamentos = value; OnPropertyChanged("LstDepartamentos"); }
        }

        private ObservableCollection<DEPARTAMENTO_ACCESO> _LstDepAcceso;

        public ObservableCollection<DEPARTAMENTO_ACCESO> LstDepAcceso
        {
            get { return _LstDepAcceso; }
            set { _LstDepAcceso = value; OnPropertyChanged("LstDepAcceso"); }
        }
        private DEPARTAMENTO_ACCESO _SelectDepAcceso;

        public DEPARTAMENTO_ACCESO SelectDepAcceso
        {
            get { return _SelectDepAcceso; }
            set { _SelectDepAcceso = value; OnPropertyChanged("SelectDepAcceso"); }
        }

        private DEPARTAMENTO _SelectDep;

        public DEPARTAMENTO SelectDep
        {
            get { return _SelectDep; }
            set { _SelectDep = value; OnPropertyChanged("SelectDep"); }
        }
        #region [PAIS ,ESTADO , MUNICIPIO, COORDINADOR , CENTRO]

        private bool _IsEnablePais = true;

        public bool IsEnablePais
        {
            get { return _IsEnablePais; }
            set { _IsEnablePais = value; OnPropertyChanged("IsEnablePais"); }
        }
        private ObservableCollection<PAIS_NACIONALIDAD> _ListPaisNacimiento;
        public ObservableCollection<PAIS_NACIONALIDAD> ListPaisNacimiento
        {
            get { return _ListPaisNacimiento; }
            set { _ListPaisNacimiento = value; OnPropertyChanged("ListPaisNacimiento"); }
        }

        private bool _IsEnableEstado = true;

        public bool IsEnableEstado
        {
            get { return _IsEnableEstado; }
            set { _IsEnableEstado = value; OnPropertyChanged("IsEnableEstado"); }
        }

        private PAIS_NACIONALIDAD selectedPaisNacimiento;
        public PAIS_NACIONALIDAD SelectedPaisNacimiento
        {
            get { return selectedPaisNacimiento; }
            set
            {
                selectedPaisNacimiento = value;
                ListEntidadNacimiento = value != null ? new ObservableCollection<ENTIDAD>(value.ENTIDAD) : new ObservableCollection<ENTIDAD>();
                ListEntidadNacimiento.Insert(0, new ENTIDAD() { ID_ENTIDAD = -1, DESCR = "SELECCIONE" });

                if (!string.IsNullOrEmpty(Guardar_Editar) ? Guardar_Editar.Equals("Guardar") : false)
                {
                    SelectEntidadNacimiento = -1;
                }
                OnPropertyChanged("SelectedPaisNacimiento");
            }
        }
        private short? selectPaisNacimiento = Parametro.PAIS;
        public short? SelectPaisNacimiento
        {
            get { return selectPaisNacimiento; }
            set
            {
                selectPaisNacimiento = value;
                OnPropertyChanged("SelectPaisNacimiento");
            }
        }

        private short? selectEntidadNacimiento;
        public short? SelectEntidadNacimiento
        {
            get { return selectEntidadNacimiento; }
            set
            {
                selectEntidadNacimiento = value;
                OnPropertyValidateChanged("SelectEntidadNacimiento");
            }
        }

        private ObservableCollection<ENTIDAD> listEntidadNacimiento;
        public ObservableCollection<ENTIDAD> ListEntidadNacimiento
        {
            get { return listEntidadNacimiento; }
            set { listEntidadNacimiento = value; OnPropertyChanged("ListEntidadNacimiento"); }
        }


        private ENTIDAD selectedEntidadNacimiento;
        public ENTIDAD SelectedEntidadNacimiento
        {
            get { return selectedEntidadNacimiento; }
            set
            {
                selectedEntidadNacimiento = value;
                ListMunicipioNacimiento = value != null ? new ObservableCollection<MUNICIPIO>(value.MUNICIPIO) : new ObservableCollection<MUNICIPIO>();
                ListMunicipioNacimiento.Insert(0, new MUNICIPIO() { ID_MUNICIPIO = -1, MUNICIPIO1 = "SELECCIONE" });
                if (!string.IsNullOrEmpty(Guardar_Editar) ? Guardar_Editar.Equals("Guardar") : false)
                {
                    SelectMunicipioNacimiento = -1;
                }
                OnPropertyChanged("SelectedEntidadNacimiento");
            }
        }

        private bool _IsEnableMunicipio = true;

        public bool IsEnableMunicipio
        {
            get { return _IsEnableMunicipio; }
            set { _IsEnableMunicipio = value; OnPropertyChanged("IsEnableMunicipio"); }
        }

        private ObservableCollection<MUNICIPIO> listMunicipioNacimiento;
        public ObservableCollection<MUNICIPIO> ListMunicipioNacimiento
        {
            get { return listMunicipioNacimiento; }
            set { listMunicipioNacimiento = value; OnPropertyChanged("ListMunicipioNacimiento"); }
        }
        private short? selectMunicipioNacimiento;
        public short? SelectMunicipioNacimiento
        {
            get { return selectMunicipioNacimiento; }
            set
            {
                selectMunicipioNacimiento = value;
                if (value != null)
                {//<-----------------------------OBTIENE TODOS LOS CENTROS RELACIONADOS CON EL MUNICIPIO Y EL ESTADO SELECCIONADO ----------------------->
                    ObtenerCentroCoordinador();
                }
                else
                {
                    LstCentros = new ObservableCollection<CENTRO>();

                    LstCentros.Insert(0, new CENTRO() { ID_CENTRO = -1, DESCR = "SELECCIONE" });
                }

                if (!string.IsNullOrEmpty(Guardar_Editar) ? Guardar_Editar.Equals("Guardar") : false)
                {
                    SelectCentro = -1;
                }

                OnPropertyValidateChanged("SelectMunicipioNacimiento");
            }
        }




        private async void CargarCoordinadoresAsync()
        {
            await StaticSourcesViewModel.CargarDatosMetodoAsync(PopularListaCoordinadores);
        }


        /// <summary>
        /// Metodo que valida que solo aparescan los centros que no tienen coordinador asignado con ese departamento
        /// </summary>
        private void ObtenerCentroCoordinador()
        {
            if (SelectDep != null && SelectMunicipioNacimiento > 0)
            {
                var LstaDepartamentoAcceso = DepAccesoControlador.ObtenerTodos().Where(w => w.ID_DEPARTAMENTO == SelectDep.ID_DEPARTAMENTO);
                //.Select(s => s.USUARIO.EMPLEADO.ID_CENTRO).Distinct().ToList();

                //Validacion  cuando se edita un Registro debe de estar seleccionado el Centro seleccionado para que aparesca la lista de coordinadores que estan relacionados al centro seleccionado
                var LstaDepartamentoAccesoFiltrada = Guardar_Editar.Equals("Editar") ? LstaDepartamentoAcceso.Where(w => w.ID_USUARIO != SelectedCoordinadoresAsignados.OBJETO_DEPARTAMENTO_ACCESO.ID_USUARIO).Select(s => s.USUARIO.EMPLEADO.ID_CENTRO).Distinct().ToList() : LstaDepartamentoAcceso.Select(s => s.USUARIO.EMPLEADO.ID_CENTRO).Distinct().ToList();
                //-----------------------------------Obtiene todos los centros y Se filtra los centros obtenidos de la lista  de centros que ya fueron registrados anteriormente para obntener todos lso que son diferentes de la lista de centros
                LstCentros = new ObservableCollection<CENTRO>(CentroControlador.ObtenerTodos("", (int)SelectEntidadNacimiento, (int)SelectMunicipioNacimiento).Where(w => !LstaDepartamentoAccesoFiltrada.Contains(w.ID_CENTRO)));
                LstCentros.Insert(0, new CENTRO() { ID_CENTRO = -1, DESCR = "SELECCIONE" });

            }
            else
            {
                LstCentros = new ObservableCollection<CENTRO>();
                LstCentros.Insert(0, new CENTRO() { ID_CENTRO = -1, DESCR = "SELECCIONE" });
            }
            //-----------------------------------Lista de los Centros que ya estan relacionados a un COORDINADOR del Departamento Seleccionado

        }

        private bool _IsEnableCentro = true;

        public bool IsEnableCentro
        {
            get { return _IsEnableCentro; }
            set { _IsEnableCentro = value; OnPropertyChanged("IsEnableCentro"); }
        }

        private ObservableCollection<CENTRO> _LstCentros;

        public ObservableCollection<CENTRO> LstCentros
        {
            get { return _LstCentros; }
            set { _LstCentros = value; OnPropertyChanged("LstCentros"); }
        }
        private short? _SelectCentro;

        public short? SelectCentro
        {
            get { return _SelectCentro; }
            set
            {
                _SelectCentro = value;

                if (value != null && !string.IsNullOrEmpty(Guardar_Editar) ? Guardar_Editar.Equals("Guardar") : false)
                {
                    //Validar departamento seleccioando
                    if (value.HasValue ? value.Value > -1 : false)
                        CargarCoordinadoresAsync();

                }
                else
                {
                    LstCoordinadores = new ObservableCollection<cSELECCION_COORDINADORES>();
                    LstCoordinadores.Insert(0, new cSELECCION_COORDINADORES() { ID_EMPLEADO = -1, COORDINADOR_NOMBRE = "SELECCIONE" });
                    SelectedCoordinador = LstCoordinadores.FirstOrDefault(w => w.ID_EMPLEADO == -1);
                }

                //if (!string.IsNullOrEmpty(Guardar_Editar) ? Guardar_Editar.Equals("Guardar") : false)
                //{
                //  SelectCoordinador = 0;

                //  }

                OnPropertyValidateChanged("SelectCentro");
            }
        }

        private void PopularListaCoordinadores()
        {
            var ListaCoordinadores = new List<cSELECCION_COORDINADORES>();
            LstCoordinadores = new ObservableCollection<cSELECCION_COORDINADORES>();
            //---------------------------------------------Obtiene lista de Usuarios ACTIVOS y los que pertenecen al centro seleccionado
            var predicate = PredicateBuilder.True<USUARIO>();
            //Si se debe filtrar a los coordinadores
            //--Obtiene los empleados que estan relacionados a un Usuario donde pertenescan al centro seleccionado
            predicate = predicate.And(w => w.ESTATUS.Equals("S") && w.EMPLEADO.ID_CENTRO == SelectCentro&&w.USUARIO_ROL.Select(s=>(short?)s.ID_ROL).Contains(SelectDep.ID_ROL));
            if (SelectedCoordinadoresAsignados != null && SelectCentro > 0)
            {
                // Filtra a la persona que ya esta registrada como coordinadora para que no aparesca en la lista de coordinadores
                predicate = predicate.And(w => w.ID_PERSONA != SelectedCoordinadoresAsignados.OBJETO_DEPARTAMENTO_ACCESO.USUARIO.ID_PERSONA);
            }///Filtra Usuario seleccioando para que no vuelva aparecer en al alista
            foreach (var PERSONA in UsuarioControlador.GetData(predicate.Expand()).Select(s => s.EMPLEADO.PERSONA).OrderBy(o => o.PATERNO).ThenBy(then => then.MATERNO).ThenBy(then2 => then2.NOMBRE))
            {
                var ObjCoordinador = new cSELECCION_COORDINADORES();
                System.Text.StringBuilder sb = new System.Text.StringBuilder();

                ObjCoordinador.ID_EMPLEADO = PERSONA.ID_PERSONA;
                sb.Append(!string.IsNullOrEmpty(PERSONA.PATERNO) ? PERSONA.PATERNO.Trim() : "");
                sb.Append("  ");
                sb.Append(!string.IsNullOrEmpty(PERSONA.MATERNO) ? PERSONA.MATERNO.Trim() : "");
                sb.Append("  ");
                sb.Append(!string.IsNullOrEmpty(PERSONA.NOMBRE) ? PERSONA.NOMBRE.Trim() : "");
                ObjCoordinador.COORDINADOR_NOMBRE = sb.ToString();
                ListaCoordinadores.Add(ObjCoordinador);
            }
            System.Windows.Application.Current.Dispatcher.Invoke((Action)(delegate
            {

                LstCoordinadores = new ObservableCollection<cSELECCION_COORDINADORES>(ListaCoordinadores);
                LstCoordinadores.Insert(0, new cSELECCION_COORDINADORES() { ID_EMPLEADO = -1, COORDINADOR_NOMBRE = "SELECCIONE" });
                SelectedCoordinador = LstCoordinadores.FirstOrDefault(w => w.ID_EMPLEADO == -1);
            }));
        }
       

        private ObservableCollection<cSELECCION_COORDINADORES> _LstCoordinadores;

        public ObservableCollection<cSELECCION_COORDINADORES> LstCoordinadores
        {
            get { return _LstCoordinadores; }
            set { _LstCoordinadores = value; OnPropertyChanged("LstCoordinadores"); }
        }
        private cSELECCION_COORDINADORES _SelectedCoordinador;

        public cSELECCION_COORDINADORES SelectedCoordinador
        {
            get { return _SelectedCoordinador; }
            set { _SelectedCoordinador = value; OnPropertyChanged("SelectedCoordinador"); }
        }

        //private SELECCION_COORDINADORES _SelectedCoordinador;

        //public SELECCION_COORDINADORES SelectedCoordinador
        //{
        //    get { return _SelectedCoordinador; }
        //    set { _SelectedCoordinador = value; OnPropertyValidateChanged("SelectedCoordinador"); }
        //}
        #endregion
        private ObservableCollection<cCOORDINADORESASIGNADOS> _ListaCoordinadoresAsignados;

        public ObservableCollection<cCOORDINADORESASIGNADOS> ListaCoordinadoresAsignados
        {
            get { return _ListaCoordinadoresAsignados; }
            set { _ListaCoordinadoresAsignados = value; OnPropertyValidateChanged("ListaCoordinadoresAsignados"); }
        }
        private cCOORDINADORESASIGNADOS _SelectedCoordinadoresAsignados;

        public cCOORDINADORESASIGNADOS SelectedCoordinadoresAsignados
        {
            get { return _SelectedCoordinadoresAsignados; }
            set { _SelectedCoordinadoresAsignados = value; OnPropertyChanged("SelectedCoordinadoresAsignados"); }
        }


        #region Privilegios
        private bool pInsertar = false;
        private bool pEditar = false;
        private bool pConsultar = false;
        private bool pImprimir = false;
        #endregion

        /// <summary>
        /// COMBOBOX
        /// </summary>
       
    }
}
