using System.Collections.Generic;

namespace WCF_BiometricoService.Modelo.Entidades
{
    public class CompareResponseImputado
    {
        public bool Identify { get; set; }
        public List<cHuellasImputado> Result { get; set; }
        public string MensajeError { get; set; }
    }
}