using SSP.Modelo;
using SSP.Servidor;
using System;
using System.Linq;

namespace SSP.Controlador.Catalogo.Justicia
{
    public class cGrupoParticipanteCancelado : EntityManagerServer<GRUPO_PARTICIPANTE_CANCELADO>
    {
        public T ObtenerConsecutivo<T>(short IdCentro, short IdTipoPrograma, short IdActividad, short idConsec) where T : struct
        {
            var max = GetData().Where(w => w.ID_CENTRO == IdCentro && w.ID_TIPO_PROGRAMA == IdTipoPrograma && w.ID_ACTIVIDAD == IdActividad && w.ID_CONSEC == idConsec).Max(m => (short?)(new { m.ID_CENTRO, m.ID_TIPO_PROGRAMA, m.ID_ACTIVIDAD, m.ID_CONSEC, m.ID_CONS_CANCELADO }.ID_CONS_CANCELADO));
            return (T)Convert.ChangeType(max.HasValue ? ++max : 1, typeof(T));
        }

        public bool InsertarParticipanteCancelado(GRUPO_PARTICIPANTE_CANCELADO Entity)
        {
            Entity.ID_CONS_CANCELADO = ObtenerConsecutivo<short>(Entity.ID_CENTRO, Entity.ID_TIPO_PROGRAMA, Entity.ID_ACTIVIDAD, Entity.ID_CONSEC);
            return Insert(Entity);
        }

        public GRUPO_PARTICIPANTE_CANCELADO InsertarParticipanteAsistencia(GRUPO_PARTICIPANTE_CANCELADO Entity)
        {
            Entity.ID_CONSEC = ObtenerConsecutivo<short>(Entity.ID_CENTRO, Entity.ID_TIPO_PROGRAMA, Entity.ID_ACTIVIDAD, Entity.ID_CONSEC);
            Insert(Entity);
            return Entity;
        }

        public GRUPO_PARTICIPANTE_CANCELADO ActualizarParticipanteAsistencia(GRUPO_PARTICIPANTE_CANCELADO Entity)
        {
            Update(Entity);
            return Entity;
        }
    }
}
