using SSP.Servidor;
using System.Text.RegularExpressions;

namespace ControlPenales
{
    public class Mensajes
    {
        public bool Seleccionado { get; set; }
        public USUARIO_MENSAJE UsuarioMensaje { get; set; }
        public bool Documento { get; set; }
        public bool IsGeneraNotificacionVisible
        {
            get
            {
                if (!string.IsNullOrEmpty(UsuarioMensaje.MENSAJE.NUC) && UsuarioMensaje.MENSAJE.ID_MEN_TIPO.HasValue && UsuarioMensaje.MENSAJE.ID_MEN_TIPO.Value==Parametro.ID_TIPO_MENSAJE_INTERCONECCION)
                    return true;
                else
                    return false;
            }
        }
    }
}
