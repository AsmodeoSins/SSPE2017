using System.Collections.Generic;

namespace WCF_BiometricoService.Modelo.Entidades
{
    public class CompareResponsePersona
    {
        public bool Identify { get; set; }
        public List<cHuellasPersona> Result { get; set; }
    }
}