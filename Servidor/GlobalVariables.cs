using SSP.Servidor;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Validation;
using System.Linq;
using System.Configuration;
using System.Data.EntityClient;


namespace SSP.Servidor
{
    public static class GlobalVariables
    {
        private static string _gUser;
        public static string gUser
        {
            get { return GlobalVariables._gUser; }
            set { GlobalVariables._gUser = value; }
        }

        private static string _gPass;
        public static string gPass
        {
            get { return GlobalVariables._gPass; }
            set { GlobalVariables._gPass = value; }
        }

        private static short _gCentro;
        public static short gCentro
        {
            get { return GlobalVariables._gCentro; }
            set { GlobalVariables._gCentro = value; }
        }
    }
}