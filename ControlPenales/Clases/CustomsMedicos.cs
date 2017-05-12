using SSP.Servidor;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows;
namespace ControlPenales
{
    public class PatologicosMedicos
    {
        public PATOLOGICO_CAT PATOLOGICO_CAT { get; set; }
        public bool SELECCIONADO { get; set; }
    }
    public enum enumTipoNota
    {
        NOTA_URGENCIA = 3,
        NOTA_EVOLUCION = 4,
        NOTA_TRASLADO = 5,
        NOTA_INTERCONSULTA = 6,
        NOTA_PREOPERATORIA = 7,
        NOTA_PREANESTESICA = 8,
        NOTA_POSTOPERAORIO = 9
    }
    public class HistoriaclinicaPatologica : ValidationViewModelBase
    {
        public HISTORIA_CLINICA_PATOLOGICOS HISTORIA_CLINICA_PATOLOGICOS { get; set; }
        private bool _RECUPERADO;
        public bool RECUPERADO
        {
            get { return _RECUPERADO; }
            set { _RECUPERADO = value; OnPropertyChanged("RECUPERADO"); }
        }
        //public bool RECUPERADO { get; set; }
        public bool DESHABILITADO { get; set; }
        public bool ELIMINABLE { get; set; }
        public DateTime REGISTRO_FEC { get; set; }
        public PATOLOGICO_CAT PATOLOGICO_CAT { get; set; }
        public string OBSERVACIONES { get; set; }
    }
    public class cReporteCompromiso
    {
        public string Fecha { get; set; }
        public string NombrePaciente { get; set; }
        public string Sexo { get; set; }
        public string Expediente { get; set; }
        public string Ubicacion { get; set; }
        public string Dieta { get; set; }
        public string IDX { get; set; }
        public string Medico { get; set; }
        public string YoNombre { get; set; }
        public string Centro { get; set; }
        public byte[] Logo1 { get; set; }
        public byte[] Logo2 { get; set; }
    }
    public class cReporteInsumos
    {
        public string Estatus { get; set; }
        public string Producto { get; set; }
        public string Insumo { get; set; }
        public int ID_PROC_MED { get; set; }
        public int ID_ATENCION_MEDICA { get; set; }
    }
    public class cReporteProcsMeds
    {
        public string ProcedimientoMedico { get; set; }
        public int ID_ATENCION_MEDICA { get; set; }
        public int ID_PROC_MED { get; set; }
    }
    public class cReporteOdontogramaSeguimiento
    {
        public string DESCR { get; set; }
        public decimal ID { get; set; }
    }
    public class cReporteCertificadoNuevoIngreso
    {
        #region PROCEDIMIENTOS MEDICOS
        public int ID_ATENCION_MEDICA { get; set; }
        #endregion

        #region NOTAS MEDICAS
        public string EstudiosRealizados { get; set; }
        public string SolicitaInterconsulta { get; set; }
        public string Pronostico { get; set; }
        public string Dietas { get; set; }
        public string Enfermedades { get; set; }
        public string ExploracionFisica { get; set; }
        public string Folio { get; set; }
        public string Medicamentos { get; set; }
        public byte[] OdontogramaImagen { get; set; }
        public string DENTAL { get; set; }
        #endregion

        #region CERTIFICADO NUEVO INGRESO
        public string Fecha { get; set; }
        public string Titulo { get; set; }
        public string Centro { get; set; }
        public string Ciudad { get; set; }
        public string Director { get; set; }
        public string NombreMedico { get; set; }
        public string Cedula { get; set; }
        public string NombreImputado { get; set; }
        public string Edad { get; set; }
        public string FechaNacimiento { get; set; }
        public string Originario { get; set; }
        public string Escolaridad { get; set; }
        public string Sexo { get; set; }
        public string FechaIngreso { get; set; }
        public string TipoDelito { get; set; }
        public string AntecedentesPatologicos { get; set; }
        public string Toxicomanias { get; set; }
        public string PadecimientoTratamiento { get; set; }
        public string Interrogatorio { get; set; }
        public string AmeritaHospitalizacion { get; set; }
        public string PeligraVida { get; set; }
        public string DiasEnSanar { get; set; }
        public string Diagnostico { get; set; }
        public string Seguimiento { get; set; }
        public string PlanTerapeutico { get; set; }
        public string Observaciones { get; set; }
        public string SignosVitales_Observaciones { get; set; }
        public string SignosVitales_Peso { get; set; }
        public string SignosVitales_Talla { get; set; }
        public string SignosVitales_Glucemia { get; set; }
        public string SignosVitales_TA { get; set; }
        public string SignosVitales_T { get; set; }
        public string SignosVitales_FR { get; set; }
        public string SignosVitales_FC { get; set; }
        public byte[] Logo1 { get; set; }
        public byte[] Logo2 { get; set; }
        public byte[] Check { get; set; }
        public byte[] Frente { get; set; }
        public byte[] Dorso { get; set; }
        public bool F_VERTICE_CABEZA { get; set; }
        public bool F_LATERAL_CABEZA_DERECHO { get; set; }
        public bool F_LATERAL_CABEZA_IZQUIERDO { get; set; }
        public bool F_OREJA_IZQUIERDA { get; set; }
        public bool F_OREJA_DERECHA { get; set; }
        public bool F_FRONTAL { get; set; }
        public bool F_CARA_DERECHA { get; set; }
        public bool F_CARA_IZQUIERDA { get; set; }
        public bool F_MANDIBULA { get; set; }
        public bool F_ANTERIOR_CUELLO { get; set; }
        public bool F_CLAVICULAR_DERECHA { get; set; }
        public bool F_CLAVICULAR_IZQUIERDA { get; set; }
        public bool F_HOMBRO_DERECHO { get; set; }
        public bool F_HOMBRO_IZQUIERDO { get; set; }
        public bool F_TORAX_DERECHO { get; set; }
        public bool F_TORAX_IZQUIERDO { get; set; }
        public bool F_AXILA_DERECHA { get; set; }
        public bool F_AXILA_IZQUIERDA { get; set; }
        public bool F_BRAZO_DERECHO { get; set; }
        public bool F_BRAZO_IZQUIERDO { get; set; }
        public bool F_EPIGASTRIO { get; set; }
        public bool F_HIPOCONDRIA_DERECHA { get; set; }
        public bool F_HIPOCONDRIA_IZQUIERDA { get; set; }
        public bool F_UMBILICAL_MESOGASTIO { get; set; }
        public bool F_BAJO_VIENTRE_HIPOGASTRIO { get; set; }
        public bool F_VACIO_DERECHA { get; set; }
        public bool F_VACIO_IZQUIERDA { get; set; }
        public bool F_INGUINAL_DERECHA { get; set; }
        public bool F_INGUINAL_IZQUIERDA { get; set; }
        public bool F_ANTEBRAZO_DERECHO { get; set; }
        public bool F_ANTEBRAZO_IZQUIERDO { get; set; }
        public bool F_MUÑECA_ANTERIOR_DERECHA { get; set; }
        public bool F_MUÑECA_ANTERIOR_IZQUIERDA { get; set; }
        public bool F_METACARPIANOS_DERECHA { get; set; }
        public bool F_METACARPIANOS_IZQUIERDA { get; set; }
        public bool F_ESCROTO { get; set; }
        public bool F_PENE_VAGINA { get; set; }
        public bool F_FALANGES_MANO_DERECHA { get; set; }
        public bool F_FALANGES_MANO_IZQUIERDA { get; set; }
        public bool F_MUSLO_ANTERIOR_DERECHO { get; set; }
        public bool F_MUSLO_ANTERIOR_IZQUIERDO { get; set; }
        public bool F_RODILLA_DERECHA { get; set; }
        public bool F_RODILLA_IZQUIERDA { get; set; }
        public bool F_ESPINILLA_DERECHA { get; set; }
        public bool F_ESPINILLA_IZQUIERDA { get; set; }
        public bool F_TOBILLO_DERECHO { get; set; }
        public bool F_TOBILLO_IZQUIERDO { get; set; }
        public bool F_METATARZIANA_DORSAL_DERECHO { get; set; }
        public bool F_METATARZIANA_DORSAL_IZQUIERDO { get; set; }
        public bool D_POSTERIOR_CABEZA { get; set; }
        public bool D_OCCIPITAL_NUCA { get; set; }
        public bool D_POSTERIOR_CUELLO { get; set; }
        public bool D_VERTEBRAL_CERVICAL { get; set; }
        public bool D_POSTERIOR_HOMBRO_DERECHO { get; set; }
        public bool D_POSTERIOR_HOMBRO_IZQUIERDO { get; set; }
        public bool D_ESCAPULAR_DERECHA { get; set; }
        public bool D_ESCAPULAR_IZQUIERDA { get; set; }
        public bool D_TORACICA_DORSAL { get; set; }
        public bool D_BRAZO_POSTERIOR_DERECHO { get; set; }
        public bool D_BRAZO_POSTERIOR_IZQUIERDO { get; set; }
        public bool D_VERTEBRAL_TORACICA { get; set; }
        public bool D_CODO_DERECHO { get; set; }
        public bool D_CODO_IZQUIERDO { get; set; }
        public bool D_LUMBAR_RENAL_DERECHA { get; set; }
        public bool D_LUMBAR_RENAL_IZQUIERDA { get; set; }
        public bool D_VERTEBRAL_LUMBAR { get; set; }
        public bool D_ANTEBRAZO_DERECHO { get; set; }
        public bool D_ANTEBRAZO_IZQUIERDO { get; set; }
        public bool D_SACRA { get; set; }
        public bool D_GLUTEA_DERECHA { get; set; }
        public bool D_GLUTEA_IZQUIERDA { get; set; }
        public bool D_DORSAL_DERECHA { get; set; }
        public bool D_DORSAL_IZQUIERDA { get; set; }
        public bool D_FALANGES_POSTERIORES_DERECHA { get; set; }
        public bool D_FALANGES_POSTERIORES_IZQUIERDA { get; set; }
        public bool D_MUSLO_POSTERIOR_DERECHO { get; set; }
        public bool D_MUSLO_POSTERIOR_IZQUIERDO { get; set; }
        public bool D_POPLITEA_DERECHA { get; set; }
        public bool D_POPLITEA_IZQUIERDA { get; set; }
        public bool D_PANTORRILLA_DERECHA { get; set; }
        public bool D_PANTORRILLA_IZQUIERDA { get; set; }
        public bool D_METATARZIANA_PLANTA_DERECHA { get; set; }
        public bool D_METATARZIANA_PLANTA_IZQUIERDA { get; set; }
        public bool D_CALCANEO_DERECHO { get; set; }
        public bool D_CALCANEO_IZQUIERDA { get; set; }
        public bool F_ORBITAL_DERECHO { get; set; }
        public bool F_ORBITAL_IZQUIERDO { get; set; }
        public bool F_NARIZ { get; set; }
        public bool F_TORAX_CENTRAL { get; set; }
        public bool F_PEZON_DERECHO { get; set; }
        public bool F_PEZON_IZQUIERDO { get; set; }
        public bool F_ENTREPIERNA_ANTERIOR_DERECHA { get; set; }
        public bool F_ENTREPIERNA_ANTERIOR_IZQUIERDA { get; set; }
        public bool F_FALANGES_PIE_DERECHO { get; set; }
        public bool F_FALANGES_PIE_IZQUIERDO { get; set; }
        public bool F_CODO_DERECHO { get; set; }
        public bool F_CODO_IZQUIERDO { get; set; }
        public bool D_OREJA_POSTERIOR_DERECHA { get; set; }
        public bool D_OREJA_POSTERIOR_IZQUIERDA { get; set; }
        public bool D_COSTILLAS_COSTADO_DERECHO { get; set; }
        public bool D_COSTILLAS_COSTADO_IZQUIERDO { get; set; }
        public bool D_MUÑECA_POSTERIOR_DERECHA { get; set; }
        public bool D_MUÑECA_POSTERIOR_IZQUIERDA { get; set; }
        public bool D_POSTERIOR_ENTREPIERNA_DERECHA { get; set; }
        public bool D_POSTERIOR_ENTREPIERNA_IZQUIERDA { get; set; }
        public bool D_TOBILLO_DERECHO { get; set; }
        public bool D_TOBILLO_IZQUIERDO { get; set; }
        #endregion
    }
    public class CustomProcedimientosMedicosSeleccionados : ValidationViewModelBase
    {
        public bool ENABLE { get; set; }
        public string OBSERV { get; set; }
        public string AGENDA { get; set; }
        public string PROC_MED_DESCR { get; set; }
        public string ID_USUARIO { get; set; }
        public short ID_PROC_MED { get; set; }
        public int? ID_ATENCION_MEDICA{ get; set; }
        public short? ID_CENTRO_UBI { get; set; }
        public int? ID_CITA { get; set; }
        public List<CustomCitasProcedimientosMedicos> CITAS { get; set; }
    }
    public class CustomCitasProcedimientosMedicos
    {
        public DateTime FECHA_INICIAL { get; set; }
        public DateTime FECHA_FINAL { get; set; }
        public int ENFERMERO { get; set; }
        public short ID_PROC_MED { get; set; }
        public string PROC_MED_DESCR { get; set; }
        public List<PROC_MED> PROC_MED { get; set; }
    }
    public class CustomProcedimientosMedicosCitados : ValidationViewModelBase
    {
        public Visibility IsVisible { get; set; }
        private PROC_MED _PROC_MED;
        public PROC_MED PROC_MED
        {
            get { return _PROC_MED; }
            set
            {
                _PROC_MED = value;
                OnPropertyChanged("PROC_MED");
            }
        }
        private ObservableCollection<CustomProcedimientosMaterialesCitados> _PROCEDIMIENTOS_MATERIALES;
        public ObservableCollection<CustomProcedimientosMaterialesCitados> PROCEDIMIENTOS_MATERIALES
        {
            get { return _PROCEDIMIENTOS_MATERIALES; }
            set
            {
                _PROCEDIMIENTOS_MATERIALES = value;
                OnPropertyChanged("PROCEDIMIENTOS_MATERIALES");
            }
        }
    }
    public class CustomProcedimientosMaterialesCitados : ValidationViewModelBase
    {
        private int? _cantidad;
        public int? CANTIDAD
        {
            get { return _cantidad; }
            set
            {
                _cantidad = value;
                RaisePropertyChanged("CANTIDAD");
            }
        }
        private PRODUCTO _PRODUCTO;
        public PRODUCTO PRODUCTO
        {
            get { return _PRODUCTO; }
            set
            {
                _PRODUCTO = value;
                RaisePropertyChanged("PRODUCTO");
            }
        }
        private PROC_MATERIAL _PROC_MATERIAL;
        public PROC_MATERIAL PROC_MATERIAL
        {
            get { return _PROC_MATERIAL; }
            set
            {
                _PROC_MATERIAL = value;
                RaisePropertyChanged("PROC_MATERIAL");
            }
        }
        public CustomProcedimientosMaterialesCitados()
        {
            setValidationRules();
        }
        private void setValidationRules()
        {
            base.ClearRules();
            base.AddRule(() => CANTIDAD, () => CANTIDAD.HasValue ? CANTIDAD.Value >= 0 : false, "LA CANTIDAD UTILIZADA ES OBLIGATORIA!");
            RaisePropertyChanged("CANTIDAD");
            RaisePropertyChanged("PROC_MATERIAL");
            RaisePropertyChanged("PRODUCTO");
            RaisePropertyChanged("CANTIDAD");
        }
    }
    public class CustomMedicos
    {
        public string NOMBRE_COMPLETO { get; set; }
        public string ID_USUARIO { get; set; }
        public SSP.Servidor.PERSONA PERSONA { get; set; }
    }
    public class CustomOdontograma
    {
        public short ID_POSICION { get; set; }
        public short ID_DIENTE { get; set; }
    }
    public class CustomTratamientoDental
    {
        public short ID_TIPO_TRATAMIENTO { get; set; }
        public short ID_TRATAMIENTO { get; set; }
        public string OBSERV { get; set; }
        public string TIPO { get; set; }
        public string TRATAMIENTO { get; set; }
        public DateTime FECHA { get; set; }
        public IEnumerable<CustomOdontograma> SELECCIONADOS { get; set; }
    }
}