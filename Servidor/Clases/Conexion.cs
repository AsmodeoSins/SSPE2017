using System;
using System.Collections.Generic;
using System.Data.Objects.DataClasses;
using System.Linq;
using System.Text;

namespace SSP.Servidor
{
    public partial class SSPEntidades
    {
        public void Conexion()
        {
#if DEBUG
           this.Database.Connection.ConnectionString = string.Format("DATA SOURCE=SSPE;PASSWORD={1};USER ID={0}", GlobalVariables.gUser, GlobalVariables.gPass);
//#else
            //                this.Database.Connection.ConnectionString = string.Format("DATA SOURCE=SSPE;PASSWORD={1};USER ID={0}", GlobalVariables.gUser, GlobalVariables.gPass);
#endif
        }

        /// <summary>
        ///     This method exists for use in LINQ queries,
        ///     as a stub that will be converted to a SQL CAST statement.
        /// </summary>
        [EdmFunction("Modelo", "ParseDouble")]
        public static double ParseDouble(string stringvalue)
        {
            return Double.Parse(stringvalue);
        }
    }
}
