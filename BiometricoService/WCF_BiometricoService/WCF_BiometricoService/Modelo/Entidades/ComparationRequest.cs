using WCF_BiometricoService.Helpers;

namespace WCF_BiometricoService.Modelo.Entidades
{
    public class ComparationRequest
    {
        public enumTipoBiometrico ID_TIPO_BIOMETRICO { get; set; }
        public enumTipoFormato ID_TIPO_FORMATO { get; set; }
        public enumTipoPersona? ID_TIPO_PERSONA { get; set; }
        public byte[] BIOMETRICO { get; set; }
        public short? ID_CENTRO { get; set; }
        public short? ID_EDIFICIO { get; set; }
        public short? ID_SECTOR { get; set; }
    }
}