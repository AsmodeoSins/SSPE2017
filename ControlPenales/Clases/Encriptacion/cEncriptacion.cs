using System.Text.RegularExpressions;

namespace ControlPenales
{
     public class cEncriptacion
    {
        private const int iAlt = 6;

        /// <summary>
        /// Metodo que regresar el texto encriptado con hash
        /// </summary>
        /// <param name="text"></param>
        /// <returns></returns>
        public static string HarshedText(string text)
        {
            //genera algoritmo para verificacion de hash
            var pass = BCrypt.Net.BCrypt.GenerateSalt(iAlt);

            //aqui esta el password en hash este es el que debe de ir en la base de datos, el pass es donde se verifica que 
            //coincida el hash es mucho mas seguro que otros algoritmos como el SHA o el MD5
            return BCrypt.Net.BCrypt.HashPassword(text, pass);
        }

        /// <summary>
        /// Compara si la clave en hash es igual a la que pusimos en nuestra clave o texto
        /// </summary>
        /// <param name="text"></param>
        /// <param name="hash"></param>
        /// <returns></returns>
        public static bool IsEquals(string text, string hash)
        {
            return BCrypt.Net.BCrypt.Verify(text, hash);
        }
    }
}
