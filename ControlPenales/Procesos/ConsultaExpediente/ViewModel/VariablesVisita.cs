using SSP.Controlador.Catalogo.Justicia;
using SSP.Servidor;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace ControlPenales
{
    partial class ConsultaExpedienteInternoViewModel
    {
        private ObservableCollection<VISITANTE_INGRESO> _ListVisitantes;
        public ObservableCollection<VISITANTE_INGRESO> ListVisitantes
        {
            get { return _ListVisitantes; }
            set { _ListVisitantes = value; OnPropertyChanged("ListVisitantes"); }
        }
        private VISITANTE_INGRESO _SelectVisitante;
        public VISITANTE_INGRESO SelectVisitante
        {
            get { return _SelectVisitante; }
            set
            {
                _SelectVisitante = value;
                if (value != null)
                    GetDatosVisita();

                OnPropertyChanged("SelectVisitante");
            }
        }

        private byte[] _ImagenVisitante;
        public byte[] ImagenVisitante
        {
            get { return _ImagenVisitante; }
            set { _ImagenVisitante = value; OnPropertyChanged("ImagenVisitante"); }
        }
        private ObservableCollection<ListaVisitaAgenda> ListProgramacionVisitaAux;
        private ObservableCollection<ListaVisitaAgenda> _ListProgramacionVisita;
        public ObservableCollection<ListaVisitaAgenda> ListProgramacionVisita
        {
            get { return _ListProgramacionVisita; }
            set
            {
                _ListProgramacionVisita = value;
                OnPropertyChanged("ListProgramacionVisita");
            }
        }
        private List<string> ListLetras = new List<string>(new string[] { "A", "B", "C", "D", "E", "F", "G", "H", "I", "J", "K", "L", "M", "N", "O", "P", "Q", "R", "S", "T", "U", "V", "W", "X", "Y", "Z" });
        private ObservableCollection<TIPO_REFERENCIA> _ListTipoRelacion;
        public ObservableCollection<TIPO_REFERENCIA> ListTipoRelacion
        {
            get { return _ListTipoRelacion; }
            set { _ListTipoRelacion = value; OnPropertyChanged("ListTipoRelacion"); }
        }
        private ObservableCollection<ESTATUS_VISITA> _ListSituacion;
        public ObservableCollection<ESTATUS_VISITA> ListSituacion
        {
            get { return _ListSituacion; }
            set { _ListSituacion = value; OnPropertyChanged("ListSituacion"); }
        }

        
        private short? _SelectParentescoVisitante;
        public short? SelectParentescoVisitante
        {
            get { return _SelectParentescoVisitante; }
            set { _SelectParentescoVisitante = value; OnPropertyChanged("SelectParentescoVisitante"); }
        }
        private short? _SelectSituacionVisitante;
        public short? SelectSituacionVisitante
        {
            get { return _SelectSituacionVisitante; }
            set { _SelectSituacionVisitante = value; OnPropertyChanged("SelectSituacionVisitante"); }
        }
        private string _TextCodigoVisitante;
        public string TextCodigoVisitante
        {
            get { return _TextCodigoVisitante; }
            set { _TextCodigoVisitante = value; OnPropertyChanged("TextCodigoVisitante"); }
        }
        private string _TextNombreVisitante;
        public string TextNombreVisitante
        {
            get { return _TextNombreVisitante; }
            set { _TextNombreVisitante = value; OnPropertyChanged("TextNombreVisitante"); }
        }
        private string _TextPaternoVisitante;
        public string TextPaternoVisitante
        {
            get { return _TextPaternoVisitante; }
            set { _TextPaternoVisitante = value; OnPropertyChanged("TextPaternoVisitante"); }
        }
        private string _TextMaternoVisitante;
        public string TextMaternoVisitante
        {
            get { return _TextMaternoVisitante; }
            set { _TextMaternoVisitante = value; OnPropertyChanged("TextMaternoVisitante"); }
        }
        private string _SelectSexoVisitante;
        public string SelectSexoVisitante
        {
            get { return _SelectSexoVisitante; }
            set { _SelectSexoVisitante = value; OnPropertyChanged("SelectSexoVisitante"); }
        }
        private string _TextEdadVisitante;
        public string TextEdadVisitante
        {
            get { return _TextEdadVisitante; }
            set { _TextEdadVisitante = value; OnPropertyChanged("TextEdadVisitante"); }
        }
        private string _TextFechaUltimaModificacionVisitante;
        public string TextFechaUltimaModificacionVisitante
        {
            get { return _TextFechaUltimaModificacionVisitante; }
            set { _TextFechaUltimaModificacionVisitante = value; OnPropertyChanged("TextFechaUltimaModificacionVisitante"); }
        }
        private string _TextTelefonoVisitante;
        public string TextTelefonoVisitante
        {
            get { return _TextTelefonoVisitante; }
            set { _TextTelefonoVisitante = value; OnPropertyChanged("TextTelefonoVisitante"); }
        }
        private string _TextCalleVisitante;
        public string TextCalleVisitante
        {
            get { return _TextCalleVisitante; }
            set { _TextCalleVisitante = value; OnPropertyChanged("TextCalleVisitante"); }
        }
        private string _TextNumExtVisitante;
        public string TextNumExtVisitante
        {
            get { return _TextNumExtVisitante; }
            set { _TextNumExtVisitante = value; OnPropertyChanged("TextNumExtVisitante"); }
        }
        private string _TextNumIntVisitante;
        public string TextNumIntVisitante
        {
            get { return _TextNumIntVisitante; }
            set { _TextNumIntVisitante = value; OnPropertyChanged("TextNumIntVisitante"); }
        }
        private string _TextCodigoPostalVisitante;
        public string TextCodigoPostalVisitante
        {
            get { return _TextCodigoPostalVisitante; }
            set { _TextCodigoPostalVisitante = value; OnPropertyChanged("TextCodigoPostalVisitante"); }
        }
        private string _TextFechaAltaVisitante;
        public string TextFechaAltaVisitante
        {
            get { return _TextFechaAltaVisitante; }
            set { _TextFechaAltaVisitante = value; OnPropertyChanged("TextFechaAltaVisitante"); }
        }
        private short? selectPaisVisitante;
        public short? SelectPaisVisitante
        {
            get { return selectPaisVisitante; }
            set
            {
                if (value == 223)
                    selectPaisVisitante = Parametro.PAIS;//82;
                else
                    selectPaisVisitante = value;
                if (selectPaisVisitante > 0)
                {
                    //llenarEntidades();
                    //ListEntidad = new ObservableCollection<ENTIDAD>((new cEntidad()).ObtenerTodos().OrderBy(o => o.DESCR));
                }
                else
                { }// ListEntidadVisitante = new ObservableCollection<ENTIDAD>();
                if (selectPaisVisitante == Parametro.PAIS)//82)
                    SelectEntidadVisitante = Parametro.ESTADO;//2;
                else if (selectPaisVisitante == -1)
                    SelectEntidadVisitante = -1;
                else
                    SelectEntidadVisitante = 33;


                OnPropertyChanged("SelectPaisVisitante");
            }
        }
        private short? selectEntidadVisitante;
        public short? SelectEntidadVisitante
        {
            get { return selectEntidadVisitante; }
            set
            {
                selectEntidadVisitante = value;
                if (selectEntidadVisitante > 0)
                {
                    //llenarMunicipiosVisitante(value);
                    //ListMunicipio = new ObservableCollection<MUNICIPIO>((new cMunicipio()).ObtenerTodos(string.Empty, value).OrderBy(o => o.MUNICIPIO1));
                }
                else { }
                //ListMunicipioVisitante = new ObservableCollection<MUNICIPIO>();
                //ListMunicipioVisitante.Insert(0, new MUNICIPIO() { ID_MUNICIPIO = -1, MUNICIPIO1 = "SIN DATO" });
                if (selectEntidadVisitante == 33)
                    SelectMunicipioVisitante = 1001;
                else
                    SelectMunicipioVisitante = -1;


                OnPropertyChanged("SelectEntidadVisitante");
            }
        }
        public async void llenarMunicipiosVisitante(short? entidad)
        {
            ListMunicipioVisitante = await StaticSourcesViewModel.CargarDatos<ObservableCollection<MUNICIPIO>>(() =>
                new ObservableCollection<MUNICIPIO>((new cMunicipio()).ObtenerTodos(string.Empty, entidad).OrderBy(o => o.MUNICIPIO1)));
        }
        private short? selectMunicipioVisitante;
        public short? SelectMunicipioVisitante
        {
            get { return selectMunicipioVisitante; }
            set
            {
                selectMunicipioVisitante = value;
                OnPropertyChanged("SelectMunicipioVisitante");
            }
        }
        public async void llenarColoniasVisitante(short? municipio, short? entidad)
        {
            var colonias = await StaticSourcesViewModel.CargarDatos<List<COLONIA>>(() => { return (new cColonia()).ObtenerTodos(string.Empty, municipio, entidad).OrderBy(o => o.DESCR).ToList(); });
            ListColoniaVisitante = new ObservableCollection<COLONIA>(colonias);
        }
        private int? selectColoniaVisitante;
        public int? SelectColoniaVisitante
        {
            get { return selectColoniaVisitante; }
            set
            {
                selectColoniaVisitante = value;

                OnPropertyChanged("SelectColoniaVisitante");
            }
        }
        private ObservableCollection<PAIS_NACIONALIDAD> listPaisVisitante;
        public ObservableCollection<PAIS_NACIONALIDAD> ListPaisVisitante
        {
            get { return listPaisVisitante; }
            set { listPaisVisitante = value; OnPropertyChanged("ListPaisVisitante"); }
        }

        private ObservableCollection<ENTIDAD> listEntidadVisitante;
        public ObservableCollection<ENTIDAD> ListEntidadVisitante
        {
            get { return listEntidadVisitante; }
            set { listEntidadVisitante = value; OnPropertyChanged("ListEntidadVisitante"); }
        }

        private ObservableCollection<MUNICIPIO> listMunicipioVisitante;
        public ObservableCollection<MUNICIPIO> ListMunicipioVisitante
        {
            get { return listMunicipioVisitante; }
            set { listMunicipioVisitante = value; OnPropertyChanged("ListMunicipioVisitante"); }
        }

        private ObservableCollection<COLONIA> listColoniaVisitante;
        public ObservableCollection<COLONIA> ListColoniaVisitante
        {
            get { return listColoniaVisitante; }
            set { listColoniaVisitante = value; OnPropertyChanged("ListColoniaVisitante"); }
        }
    }
}
