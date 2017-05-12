using SSP.Servidor;
using SSP.Modelo;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Collections.ObjectModel;
using System.Data.Objects.SqlClient;

namespace SSP.Controlador.Catalogo.Justicia
{
    public class cDepartamentosAcceso : EntityManagerServer<DEPARTAMENTO_ACCESO>
    {
        public IQueryable<DEPARTAMENTO_ACCESO> ObtenerCoordinadorPorCentro(short id_departamento, short id_centro)
        {
            try
            {
                return GetData(w => w.ID_DEPARTAMENTO == id_departamento && w.USUARIO.EMPLEADO.ID_CENTRO==id_centro);
            }
            catch(Exception ex)
            {
                throw new ApplicationException(ex.Message + " " + (ex.InnerException != null ? ex.InnerException.InnerException.Message : ""));
            }
        }

    }
}