using System;
using System.IO;

namespace WCF_BiometricoService.Helpers
{
    public static class Converter
    {
        /// <summary>
        /// Function to save byte array to a file
        /// </summary>
        /// <param name="_FileName">File name to save byte array</param>
        /// <param name="_ByteArray">Byte array to save to external file</param>
        /// <returns>Return true if byte array save successfully, if not return false</returns>
        public static bool ByteArrayToFile(string _Directory, string _FileName, byte[] _ByteArray)
        {
            try
            {
                if (!Directory.Exists(_Directory))
                    Directory.CreateDirectory(_Directory);

                // Open file for reading
                var _FileStream = new FileStream((_Directory + _FileName), FileMode.Create, FileAccess.Write);

                // Writes a block of bytes to this stream using data from a byte array.
                _FileStream.Write(_ByteArray, 0, _ByteArray.Length);

                // close file stream
                _FileStream.Close();

                return true;
            }
            catch (Exception _Exception)
            {
                // Error
                Console.WriteLine("Exception caught in process: {0}", _Exception.ToString());
            }

            // error occured, return false
            return false;
        }
    }
}