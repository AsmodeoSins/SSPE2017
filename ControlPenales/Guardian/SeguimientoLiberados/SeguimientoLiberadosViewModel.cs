using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Input;

namespace ControlPenales
{
    public class SeguimientoLiberadosViewModel : ValidationViewModelBase, IPageViewModel
    {
        public delegate void ParameterChange(string parameter);
        public ParameterChange OnParameterChange { get; set; }

        #region variables
        public string Name
        {
            get
            {
                return "seguimiento_liberados";
            }
        }

        //TEST/////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        private List<ModeloPrueba> imputados;

        public List<ModeloPrueba> Imputados
        {
            get { return imputados; }
            set { imputados = value; OnPropertyChanged("Imputados"); }
        }

        private ModeloPrueba imputado;

        public ModeloPrueba Imputado
        {
            get { return imputado; }
            set { imputado = value; OnPropertyChanged("Imputado"); }
        }
        //FIN TEST/////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

        private bool buscarVisible;

        public bool BuscarVisible
        {
            get { return buscarVisible; }
            set { buscarVisible = value; OnPropertyChanged("BuscarVisible"); }
        }

        private bool datosVisible;

        public bool DatosVisible
        {
            get { return datosVisible; }
            set { datosVisible = value; OnPropertyChanged("DatosVisible"); }
        }

        private string tituloHeader;

        public string TituloHeader
        {
            get { return tituloHeader; }
            set { tituloHeader = value; OnPropertyChanged("TituloHeader"); }
        }

        private bool personalesVisible;

        public bool PersonalesVisible
        {
            get { return personalesVisible; }
            set { personalesVisible = value; OnPropertyChanged("PersonalesVisible"); }
        }
        private bool referenciasVisible;

        public bool ReferenciasVisible
        {
            get { return referenciasVisible; }
            set { referenciasVisible = value; OnPropertyChanged("ReferenciasVisible"); }
        }

        private bool seniasParticularesVisible;

        public bool SeniasParticularesVisible
        {
            get { return seniasParticularesVisible; }
            set { seniasParticularesVisible = value; OnPropertyChanged("SeniasParticularesVisible"); }
        }
        private bool juridicoVisible;

        public bool JuridicoVisible
        {
            get { return juridicoVisible; }
            set { juridicoVisible = value; OnPropertyChanged("JuridicoVisible"); }
        }

        private bool beneficioVisible;

        public bool BeneficioVisible
        {
            get { return beneficioVisible; }
            set { beneficioVisible = value; OnPropertyChanged("BeneficioVisible"); }
        }

        private bool internacionVisible;

        public bool InternacionVisible
        {
            get { return internacionVisible; }
            set { internacionVisible = value; OnPropertyChanged("InternacionVisible"); }
        }

        private bool obligacionVisible;

        public bool ObligacionVisible
        {
            get { return obligacionVisible; }
            set { obligacionVisible = value; OnPropertyChanged("ObligacionVisible"); }
        }

        private bool fotoVisible;

        public bool FotoVisible
        {
            get { return fotoVisible; }
            set { fotoVisible = value; OnPropertyChanged("FotoVisible"); }
        }

        private bool huellasVisible;

        public bool HuellasVisible
        {
            get { return huellasVisible; }
            set { huellasVisible = value; OnPropertyChanged("HuellasVisible"); }
        }

        private bool aliasVisible;

        public bool AliasVisible
        {
            get { return aliasVisible; }
            set { aliasVisible = value; OnPropertyChanged("AliasVisible"); }
        }

        private bool documentoVisible;

        public bool DocumentoVisible
        {
            get { return documentoVisible; }
            set { documentoVisible = value; OnPropertyChanged("DocumentoVisible"); }
        }
        #endregion

        #region constructor
        public SeguimientoLiberadosViewModel()
        {
            Imputados = new List<ModeloPrueba>();
            Imputado = new ModeloPrueba();
            //{
            //    apellido_paterno = "PEREZ",
            //    apellido_materno = "LOPEZ",
            //    nombre = "JUAN",
            //    causa_penal = "03/2015",
            //    estatus = "ACTIVO",
            //    tipo_registro = "PERSONA",
            //    unidad = "MEXICALI - DEPARTAMENTO DE VIGILANCIA",
            //    foto = "/ControlPenales;component/Imagen/juanperez.jpg",
            //    fecha_registro = DateTime.Now,
            //    edad = 40,
            //    fecha_nacimiento = DateTime.Now.AddYears(-40),
            //    pais_nacimiento = "MEXICO",
            //    estado_nacimiento = "BAJA CALIFORNIA",
            //    municipio_nacimiento = "MEXICALI",
            //    pais_domicilio = "MEXICO",
            //    estado_domicilio = "BAJA CALIFORNIA",
            //    municipio_domicilio = "MEXICALI",
            //    calle_domicilio = "DE LOS FAISANES",
            //    numero_exterior_domicilio = "100",
            //    codigo_postal_domicilio = "21140",
            //    colonia_domicilio = "FRACC. CONDOR",
            //    telefono_casa = "5575859",
            //    telefono_movil = "6861474948",
            //    radio = "152*445685",
            //    correo_electronico = "JUAN.PEREZ@GMAIL.COM",
            //    observaciones = "PERSONA VIOLENTA"
            //};
        }

        #endregion

        #region metodos


        void IPageViewModel.inicializa()
        {
            BuscarVisible = true;
            DatosVisible = !BuscarVisible;

            //MENU DATOS
            TituloHeader = "Datos Personales";
            PersonalesVisible = true;
            ReferenciasVisible = !PersonalesVisible;
            SeniasParticularesVisible = !PersonalesVisible;
            JuridicoVisible = !PersonalesVisible;
            BeneficioVisible = !PersonalesVisible;
            InternacionVisible = !PersonalesVisible;
            ObligacionVisible = !PersonalesVisible;
            FotoVisible = !PersonalesVisible;
            HuellasVisible = !PersonalesVisible;
            AliasVisible = !PersonalesVisible;
            DocumentoVisible = !PersonalesVisible;

            //PRUEBAS//////////////////////////////
            List<referencias> referencias = new List<ControlPenales.referencias>();
            referencias.Add(new referencias { apellido_materno = "REREZ", apellido_paterno = "PEREZ", nombre = "JUANITA DE JESUS", relacion = "MADRE", domicilio = "AV. DE LOS FAISANES 100 FRACC. CONDOR", telefono_casa = "5575859" });
            referencias.Add(new referencias { apellido_materno = "HERNANDEZ", apellido_paterno = "SANCHEZ", nombre = "MARIA", relacion = "ESPOSA", domicilio = "AV. DE LOS FAISANES 100 FRACC. CONDOR", telefono_casa = "5575859" });
            List<seniasParticulares> seniasParticulares = new List<ControlPenales.seniasParticulares>();
            seniasParticulares.Add(new seniasParticulares { sp_tipo = "TATUAJE", sp_cantidad = "1", sp_lado = "DERECHO", sp_vista = "FRONTAL", sp_region = "FRONTAL", sp_descripcion = "TATUAJE DE LA VIRGEN DE GUADALUPE" });
            List<alias> alias = new List<ControlPenales.alias>();
            alias.Add(new alias { apellido_materno = "PEREZ", apellido_paterno = "LOPEZ", nombre = "JOSE" });
            List<apodos> apodos = new List<ControlPenales.apodos>();
            apodos.Add(new apodos { apodo = "WERO" });
            List<internaciones> internaciones = new List<ControlPenales.internaciones>();
            internaciones.Add(new internaciones { fecha = "30/01/2015", internacion = "21:00", cumplimiento = "1", lugar = "CERESO MEXICALI", salida = "7:00" });
            internaciones.Add(new internaciones { fecha = "29/01/2015", internacion = "21:00", cumplimiento = "1", lugar = "CERESO MEXICALI", salida = "7:00" });
            internaciones.Add(new internaciones { fecha = "28/01/2015", internacion = "21:00", cumplimiento = "1", lugar = "CERESO MEXICALI", salida = "7:00" });
            internaciones.Add(new internaciones { fecha = "27/01/2015", internacion = "21:00", cumplimiento = "1", lugar = "CERESO MEXICALI", salida = "7:00" });
            List<documentos> documentos = new List<ControlPenales.documentos>();
            documentos.Add(new documentos { asesor = "LIC.JUAN ESCUTIA", asunto = "REVISION DE CASO", dependencia = "SSP", documento = "DOCUMENTO 1", fecha = "26/02/2015", fuero = "COMUN", no_oficio = "0001" });
            documentos.Add(new documentos { asesor = "LIC.JUAN ESCUTIA", asunto = "REVISION DE CASO", dependencia = "SSP", documento = "DOCUMENTO 2", fecha = "26/02/2015", fuero = "COMUN", no_oficio = "0002" });
            documentos.Add(new documentos { asesor = "LIC.JUAN ESCUTIA", asunto = "REVISION DE CASO", dependencia = "SSP", documento = "DOCUMENTO 3", fecha = "26/02/2015", fuero = "COMUN", no_oficio = "0003" });
            documentos.Add(new documentos { asesor = "LIC.JUAN ESCUTIA", asunto = "REVISION DE CASO", dependencia = "SSP", documento = "DOCUMENTO 4", fecha = "26/02/2015", fuero = "COMUN", no_oficio = "0004" });
            documentos.Add(new documentos { asesor = "LIC.JUAN ESCUTIA", asunto = "REVISION DE CASO", dependencia = "SSP", documento = "DOCUMENTO 5", fecha = "26/02/2015", fuero = "COMUN", no_oficio = "0005" });
            Imputados = new List<ModeloPrueba>();
            Imputados.Add(new ModeloPrueba { apellido_paterno = "MONTANA", apellido_materno = "MARTINEZ", nombre = "ANTONIO", causa_penal = "01/2015", estatus = "ACTIVO", tipo_registro = "PERSONA", unidad = "MEXICALI - DEPARTAMENTO DE VIGILANCIA", foto = "/ControlPenales;component/Imagen/detenido1.jpg", fecha_registro = Fechas.GetFechaDateServer });
            Imputados.Add(new ModeloPrueba { apellido_paterno = "LOPEZ", apellido_materno = "GUTIERREZ", nombre = "JACINTO", causa_penal = "02/2015", estatus = "ACTIVO", tipo_registro = "PERSONA", unidad = "MEXICALI - DEPARTAMENTO DE VIGILANCIA", foto = "/ControlPenales;component/Imagen/detenido2.jpg", fecha_registro = Fechas.GetFechaDateServer });
            Imputados.Add(new ModeloPrueba
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
                mf_ojos = "CAFES",
                referencias = referencias,
                senias_particulares = seniasParticulares,
                alias = alias,
                apodos = apodos,
                internaciones = internaciones,
                documentos = documentos
            });
            ///////////////////////////////////////

        }

        public void clickSwitch(Object obj)
        {
            switch (obj.ToString())
            {
                case "menu_salir":
                    OnParameterChange("salir");
                    break;
                case "agregar_causa_penal":
                    BuscarVisible = !BuscarVisible;
                    DatosVisible = !BuscarVisible;
                    break;
                case "buscar":

                    break;
                //MENU DATOS
                case "menu_personales":
                    TituloHeader = "Datos Personales";
                    PersonalesVisible = true;
                    ReferenciasVisible = !PersonalesVisible;
                    SeniasParticularesVisible = !PersonalesVisible;
                    JuridicoVisible = !PersonalesVisible;
                    BeneficioVisible = !PersonalesVisible;
                    InternacionVisible = !PersonalesVisible;
                    ObligacionVisible = !PersonalesVisible;
                    FotoVisible = !PersonalesVisible;
                    HuellasVisible = !PersonalesVisible;
                    AliasVisible = !PersonalesVisible;
                    DocumentoVisible = !PersonalesVisible;
                    break;
                case "menu_referencias":
                    TituloHeader = "Referencias";
                    ReferenciasVisible = true;
                    PersonalesVisible = !ReferenciasVisible;
                    SeniasParticularesVisible = !ReferenciasVisible;
                    JuridicoVisible = !ReferenciasVisible;
                    BeneficioVisible = !ReferenciasVisible;
                    InternacionVisible = !ReferenciasVisible;
                    ObligacionVisible = !ReferenciasVisible;
                    FotoVisible = !ReferenciasVisible;
                    HuellasVisible = !ReferenciasVisible;
                    AliasVisible = !ReferenciasVisible;
                    DocumentoVisible = !ReferenciasVisible;
                    break;
                case "menu_senias_particulares":
                    TituloHeader = "Señas Particulares";
                    SeniasParticularesVisible = true;
                    PersonalesVisible = !SeniasParticularesVisible;
                    ReferenciasVisible = !SeniasParticularesVisible;
                    JuridicoVisible = !SeniasParticularesVisible;
                    BeneficioVisible = !SeniasParticularesVisible;
                    InternacionVisible = !SeniasParticularesVisible;
                    ObligacionVisible = !SeniasParticularesVisible;
                    FotoVisible = !SeniasParticularesVisible;
                    HuellasVisible = !SeniasParticularesVisible;
                    AliasVisible = !SeniasParticularesVisible;
                    DocumentoVisible = !SeniasParticularesVisible;
                    break;
                case "menu_juridico":
                    TituloHeader = "Jurídico";
                    JuridicoVisible = true;
                    PersonalesVisible = !JuridicoVisible;
                    ReferenciasVisible = !JuridicoVisible;
                    SeniasParticularesVisible = !JuridicoVisible;
                    BeneficioVisible = !JuridicoVisible;
                    InternacionVisible = !JuridicoVisible;
                    ObligacionVisible = !JuridicoVisible;
                    FotoVisible = !JuridicoVisible;
                    HuellasVisible = !JuridicoVisible;
                    AliasVisible = !JuridicoVisible;
                    DocumentoVisible = !JuridicoVisible;
                    break;
                case "menu_beneficio":
                    TituloHeader = "Beneficio";
                    BeneficioVisible = true;
                    PersonalesVisible = !BeneficioVisible;
                    ReferenciasVisible = !BeneficioVisible;
                    SeniasParticularesVisible = !BeneficioVisible;
                    JuridicoVisible = !BeneficioVisible;
                    InternacionVisible = !BeneficioVisible;
                    ObligacionVisible = !BeneficioVisible;
                    FotoVisible = !BeneficioVisible;
                    HuellasVisible = !BeneficioVisible;
                    AliasVisible = !BeneficioVisible;
                    DocumentoVisible = !BeneficioVisible;
                    break;
                case "menu_internacion":
                    TituloHeader = "Internaciones";
                    InternacionVisible = true;
                    PersonalesVisible = !InternacionVisible;
                    ReferenciasVisible = !InternacionVisible;
                    SeniasParticularesVisible = !InternacionVisible;
                    JuridicoVisible = !InternacionVisible;
                    BeneficioVisible = !InternacionVisible;
                    ObligacionVisible = !InternacionVisible;
                    FotoVisible = !InternacionVisible;
                    HuellasVisible = !InternacionVisible;
                    AliasVisible = !InternacionVisible;
                    DocumentoVisible = !InternacionVisible;
                    break;
                case "menu_obligacion":
                    TituloHeader = "Obligaciones";
                    ObligacionVisible = true;
                    PersonalesVisible = !ObligacionVisible;
                    ReferenciasVisible = !ObligacionVisible;
                    SeniasParticularesVisible = !ObligacionVisible;
                    JuridicoVisible = !ObligacionVisible;
                    BeneficioVisible = !ObligacionVisible;
                    InternacionVisible = !ObligacionVisible;
                    FotoVisible = !ObligacionVisible;
                    HuellasVisible = !ObligacionVisible;
                    AliasVisible = !ObligacionVisible;
                    DocumentoVisible = !ObligacionVisible;
                    break;
                case "menu_foto":
                    TituloHeader = "Foto";
                    FotoVisible = true;
                    PersonalesVisible = !FotoVisible;
                    ReferenciasVisible = !FotoVisible;
                    SeniasParticularesVisible = !FotoVisible;
                    JuridicoVisible = !FotoVisible;
                    BeneficioVisible = !FotoVisible;
                    InternacionVisible = !FotoVisible;
                    ObligacionVisible = !FotoVisible;
                    HuellasVisible = !FotoVisible;
                    AliasVisible = !FotoVisible;
                    DocumentoVisible = !FotoVisible;
                    break;
                case "menu_huella":
                    TituloHeader = "Huellas";
                    HuellasVisible = true;
                    PersonalesVisible = !HuellasVisible;
                    ReferenciasVisible = !HuellasVisible;
                    SeniasParticularesVisible = !HuellasVisible;
                    JuridicoVisible = !HuellasVisible;
                    BeneficioVisible = !HuellasVisible;
                    InternacionVisible = !HuellasVisible;
                    ObligacionVisible = !HuellasVisible;
                    FotoVisible = !HuellasVisible;
                    AliasVisible = !HuellasVisible;
                    DocumentoVisible = !HuellasVisible;
                    break;
                case "menu_alias":
                    TituloHeader = "Alias";
                    AliasVisible = true;
                    PersonalesVisible = !AliasVisible;
                    ReferenciasVisible = !AliasVisible;
                    SeniasParticularesVisible = !AliasVisible;
                    JuridicoVisible = !AliasVisible;
                    BeneficioVisible = !AliasVisible;
                    InternacionVisible = !AliasVisible;
                    ObligacionVisible = !AliasVisible;
                    FotoVisible = !AliasVisible;
                    HuellasVisible = !AliasVisible;
                    DocumentoVisible = !AliasVisible;
                    break;
                case "menu_documento":
                    TituloHeader = "Documentos";
                    DocumentoVisible = true;
                    PersonalesVisible = !DocumentoVisible;
                    ReferenciasVisible = !DocumentoVisible;
                    SeniasParticularesVisible = !DocumentoVisible;
                    JuridicoVisible = !DocumentoVisible;
                    BeneficioVisible = !DocumentoVisible;
                    InternacionVisible = !DocumentoVisible;
                    ObligacionVisible = !DocumentoVisible;
                    FotoVisible = !DocumentoVisible;
                    HuellasVisible = !DocumentoVisible;
                    AliasVisible = !DocumentoVisible;
                    break;
            }
        }

        private void selectImputado(Object obj)
        {
            Imputado = (ModeloPrueba)obj;
            BuscarVisible = !BuscarVisible;
            DatosVisible = !BuscarVisible;
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
