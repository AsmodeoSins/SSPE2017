using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ControlPenales
{
    public enum enumAtencionServicio
    {
        // MEDICO
        CITA_MEDICA = 1,
        CERTIFICADO_NUEVO_INGRESO = 2,
        CERTIFICADO_INTEGRIDAD_FISICA = 3,
        HISTORIA_CLINICA_MEDICA = 4,
        PROCEDIMIENTOS_MEDICOS = 5,
        ESPECIALIDAD_INTERNA = 6,
        NOTA_EVOLUCION = 7,
        NOTA_EGRESO=8,
        TRASLADO_VIRTUAL=9,

        //DENTAL
        CITA_MEDICA_DENTAL = 1,
        CERTIFICADO_DENTAL_NUEVO_INGRESO = 2,
        CERTIFICADO_DENTISTA_INTEGRIDAD_FISICA = 3,
        HISTORIA_CLINICA_DENTAL = 4,
        PROCEDIMIENTOS_DENTALES = 5,
        ESPECIALIDAD_INTERNA_DENTAL = 6,
        NOTA_EVOLUCIÓN_DENTAL = 7,
        TRASLADO_VIRTUAL_DENTAL=9
    }
}
