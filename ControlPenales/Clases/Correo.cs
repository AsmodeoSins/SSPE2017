using System.Text.RegularExpressions;

namespace ControlPenales
{
    public class Correo
    {
        public Correo() { }

        public bool ValidarCorreo(string strIn)
        {
            if (string.IsNullOrEmpty(strIn))
                return true;
            try
            {
                return Regex.IsMatch(strIn,
                          @"^(?("")("".+?(?<!\\)""@)|(([0-9a-z]((\.(?!\.))|[-!#\$%&'\*\+/=\?\^`\{\}\|~\w])*)(?<=[0-9a-z])@))" +
                          @"(?(\[)(\[(\d{1,3}\.){3}\d{1,3}\])|(([0-9a-z][-\w]*[0-9a-z]*\.)+[a-z0-9][\-a-z0-9]{0,22}[a-z0-9]))$",
                          RegexOptions.IgnoreCase);
            }
            catch
            {
                return false;
            }
        }
    }
}
