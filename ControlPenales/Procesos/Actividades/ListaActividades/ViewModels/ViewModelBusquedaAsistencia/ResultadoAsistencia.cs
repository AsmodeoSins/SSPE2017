using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ControlPenales{
    public enum enumResultadoAsistencia : int
    {
        ACCESO_NEGADO = 1,
        INTERNO_NO_PERMITIDO = 2,
        INTERNO_NO_AUTORIZADO = 3,
        ASISTENCIA_CAPTURADA = 4,
        ASISTENCIA_PREVIA = 5,
        EN_PROCESO = 6,
        CAPTURE_HUELLA = 7,
        CAPTURE_DE_NUEVO = 8,
        FALSO_POSITIVO = 9
    }
}
