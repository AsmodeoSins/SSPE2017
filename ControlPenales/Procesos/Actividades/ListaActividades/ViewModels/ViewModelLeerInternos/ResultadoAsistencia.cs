using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ControlPenales
{
    public enum enumResultadoAsistencia : int
    {
        INTERNO_NO_PERMITIDO = 1,
        ASISTENCIA_CAPTURADA = 2,
        EN_PROCESO = 3,
        CAPTURE_HUELLA = 4,
        CAPTURE_DE_NUEVO = 5,
        FALSO_POSITIVO = 6
    }
}
