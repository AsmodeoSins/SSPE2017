namespace ControlPenales
{
    public class PlantillaBiometrico
    {
        public BiometricoServiceReference.enumTipoBiometrico ID_TIPO_BIOMETRICO { get; set; }
        public BiometricoServiceReference.enumTipoFormato ID_TIPO_FORMATO { get; set; }
        public short? CALIDAD { get; set; }
        public byte[] BIOMETRICO { get; set; }
    }
}
