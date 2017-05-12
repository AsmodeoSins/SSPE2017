using SSP.Servidor;
using SSP.Modelo;
using System;
using System.Text.RegularExpressions;
using System.Collections.Generic;
using System.Linq;
using System.Collections.ObjectModel;
using LinqKit;
using System.Transactions;

namespace SSP.Controlador.Catalogo.Justicia
{
    public class cCama : EntityManagerServer<CAMA>
    {
        /// <summary>
        /// constructor de la clase
        /// </summary>
        public cCama()
        { }

        /// <summary>
        /// metodo que conecta con la base de datos para extraer todos los registros
        /// </summary>
        /// <returns>listado de tipo "ETNIA"</returns>
        public IQueryable<CAMA> ObtenerTodos(string buscar = "", MUNICIPIO municipio = null, CENTRO centro = null, EDIFICIO edificio = null, SECTOR sector = null, CELDA celda = null)
        {
            try
            {
                getDbSet();

                if (string.IsNullOrEmpty(celda.ID_CELDA) || celda.ID_CELDA == "SELECCIONE")
                {
                    #region buscar
                    if (string.IsNullOrEmpty(buscar))
                    {
                        #region municipio
                        if (municipio.ID_MUNICIPIO == 0)
                        {
                            #region centro
                            if (centro.ID_CENTRO == 0)
                            {
                                #region edificio
                                if (edificio.ID_EDIFICIO == 0)
                                {
                                    #region sector
                                    if (sector.ID_SECTOR == 0)
                                    {
                                        return GetData();
                                    }
                                    else
                                    {
                                        return GetData().Where(w => w.ID_SECTOR == sector.ID_SECTOR && w.ID_EDIFICIO == sector.ID_EDIFICIO && w.ID_CENTRO == sector.ID_CENTRO);
                                    }
                                    #endregion
                                }
                                else
                                {
                                    #region sector
                                    if (sector.ID_SECTOR == 0)
                                    {
                                        return GetData().Where(w => w.ID_EDIFICIO == edificio.ID_EDIFICIO && w.ID_CENTRO == edificio.ID_CENTRO);
                                    }
                                    else
                                    {
                                        return GetData().Where(w => w.ID_EDIFICIO == edificio.ID_EDIFICIO && w.ID_CENTRO == edificio.ID_CENTRO && w.ID_SECTOR == sector.ID_SECTOR);
                                    }
                                    #endregion
                                }
                                #endregion
                            }
                            else
                            {
                                #region edificio
                                if (edificio.ID_EDIFICIO == 0)
                                {
                                    #region sector
                                    if (sector.ID_SECTOR == 0)
                                    {
                                        return GetData().Where(w => w.ID_CENTRO == centro.ID_CENTRO);
                                    }
                                    else
                                    {
                                        return GetData().Where(w => w.ID_CENTRO == centro.ID_CENTRO && w.ID_SECTOR == sector.ID_SECTOR && w.ID_EDIFICIO == sector.ID_EDIFICIO);
                                    }
                                    #endregion
                                }
                                else
                                {
                                    #region sector
                                    if (sector.ID_SECTOR == 0)
                                    {
                                        return GetData().Where(w => w.ID_CENTRO == centro.ID_CENTRO && w.ID_EDIFICIO == edificio.ID_EDIFICIO);
                                    }
                                    else
                                    {
                                        return GetData().Where(w => w.ID_CENTRO == centro.ID_CENTRO && w.ID_EDIFICIO == edificio.ID_EDIFICIO && w.ID_SECTOR == sector.ID_SECTOR);
                                    }
                                    #endregion
                                }
                                #endregion
                            }
                            #endregion
                        }
                        else
                        {
                            #region centro
                            if (centro.ID_CENTRO == 0)
                            {
                                #region edificio
                                if (edificio.ID_EDIFICIO == 0)
                                {
                                    #region sector
                                    if (sector.ID_SECTOR == 0)
                                    {
                                        return GetData().Where(w => w.CELDA.SECTOR.EDIFICIO.CENTRO.ID_MUNICIPIO == municipio.ID_MUNICIPIO);
                                    }
                                    else
                                    {
                                        return GetData().Where(w => w.CELDA.SECTOR.EDIFICIO.CENTRO.ID_MUNICIPIO == municipio.ID_MUNICIPIO && w.ID_SECTOR == sector.ID_SECTOR
                                                                    && w.ID_EDIFICIO == sector.ID_EDIFICIO && w.ID_CENTRO == sector.ID_CENTRO);
                                    }
                                    #endregion
                                }
                                else
                                {
                                    #region sector
                                    if (sector.ID_SECTOR == 0)
                                    {
                                        return GetData().Where(w => w.CELDA.SECTOR.EDIFICIO.CENTRO.ID_MUNICIPIO == municipio.ID_MUNICIPIO && w.ID_EDIFICIO == edificio.ID_EDIFICIO
                                                                    && w.ID_CENTRO == edificio.ID_CENTRO);
                                    }
                                    else
                                    {
                                        return GetData().Where(w => w.CELDA.SECTOR.EDIFICIO.CENTRO.ID_MUNICIPIO == municipio.ID_MUNICIPIO && w.ID_EDIFICIO == edificio.ID_EDIFICIO
                                                                    && w.ID_SECTOR == sector.ID_SECTOR && w.ID_CENTRO == edificio.ID_CENTRO);
                                    }
                                    #endregion
                                }
                                #endregion
                            }
                            else
                            {
                                #region edificio
                                if (edificio.ID_EDIFICIO == 0)
                                {
                                    #region sector
                                    if (sector.ID_SECTOR == 0)
                                    {
                                        return GetData().Where(w => w.CELDA.SECTOR.EDIFICIO.CENTRO.ID_MUNICIPIO == municipio.ID_MUNICIPIO && w.ID_CENTRO == centro.ID_CENTRO);
                                    }
                                    else
                                    {
                                        return GetData().Where(w => w.CELDA.SECTOR.EDIFICIO.CENTRO.ID_MUNICIPIO == municipio.ID_MUNICIPIO && w.ID_CENTRO == centro.ID_CENTRO
                                                                    && w.ID_SECTOR == sector.ID_SECTOR && w.ID_EDIFICIO == sector.ID_EDIFICIO);
                                    }
                                    #endregion
                                }
                                else
                                {
                                    #region sector
                                    if (sector.ID_SECTOR == 0)
                                    {
                                        return GetData().Where(w => w.CELDA.SECTOR.EDIFICIO.CENTRO.ID_MUNICIPIO == municipio.ID_MUNICIPIO && w.ID_CENTRO == centro.ID_CENTRO
                                                                    && w.ID_EDIFICIO == edificio.ID_EDIFICIO);
                                    }
                                    else
                                    {
                                        return GetData().Where(w => w.CELDA.SECTOR.EDIFICIO.CENTRO.ID_MUNICIPIO == municipio.ID_MUNICIPIO && w.ID_CENTRO == centro.ID_CENTRO
                                                                    && w.ID_EDIFICIO == edificio.ID_EDIFICIO && w.ID_SECTOR == sector.ID_SECTOR);
                                    }
                                    #endregion
                                }
                                #endregion
                            }
                            #endregion
                        }
                        #endregion
                    }
                    else
                    {
                        short intBuscar;
                        if (short.TryParse(buscar, out intBuscar))
                        {
                            #region municipio
                            if (municipio.ID_MUNICIPIO == 0)
                            {
                                #region centro
                                if (centro.ID_CENTRO == 0)
                                {
                                    #region edificio
                                    if (edificio.ID_EDIFICIO == 0)
                                    {
                                        #region sector
                                        if (sector.ID_SECTOR == 0)
                                        {
                                            return GetData().Where(w => (w.ID_CAMA == intBuscar || w.CELDA.ID_CELDA.Contains(buscar) || w.CELDA.SECTOR.DESCR.Contains(buscar) ||
                                                                        w.CELDA.SECTOR.EDIFICIO.DESCR.Contains(buscar) || w.CELDA.SECTOR.EDIFICIO.CENTRO.DESCR.Contains(buscar)));
                                        }
                                        else
                                        {
                                            return GetData().Where(w => (w.ID_CAMA == intBuscar || w.CELDA.ID_CELDA.Contains(buscar) || w.CELDA.SECTOR.EDIFICIO.DESCR.Contains(buscar) ||
                                                                        w.CELDA.SECTOR.EDIFICIO.CENTRO.DESCR.Contains(buscar)) && w.ID_SECTOR == sector.ID_SECTOR && w.ID_EDIFICIO == sector.ID_EDIFICIO
                                                                        && w.ID_CENTRO == sector.ID_CENTRO);
                                        }
                                        #endregion
                                    }
                                    else
                                    {
                                        #region sector
                                        if (sector.ID_SECTOR == 0)
                                        {
                                            return GetData().Where(w => (w.ID_CAMA == intBuscar || w.CELDA.ID_CELDA.Contains(buscar) || w.CELDA.SECTOR.DESCR.Contains(buscar) ||
                                                                        w.CELDA.SECTOR.EDIFICIO.CENTRO.DESCR.Contains(buscar)) && w.ID_EDIFICIO == edificio.ID_EDIFICIO &&
                                                                        w.ID_CENTRO == edificio.ID_CENTRO);
                                        }
                                        else
                                        {
                                            return GetData().Where(w => (w.ID_CAMA == intBuscar || w.CELDA.ID_CELDA.Contains(buscar) || w.CELDA.SECTOR.EDIFICIO.CENTRO.DESCR.Contains(buscar))
                                                                        && w.ID_EDIFICIO == edificio.ID_EDIFICIO && w.ID_SECTOR == sector.ID_SECTOR && w.ID_CENTRO == edificio.ID_CENTRO);
                                        }
                                        #endregion
                                    }
                                    #endregion
                                }
                                else
                                {
                                    #region edificio
                                    if (edificio.ID_EDIFICIO == 0)
                                    {
                                        #region sector
                                        if (sector.ID_SECTOR == 0)
                                        {
                                            return GetData().Where(w => (w.ID_CAMA == intBuscar || w.CELDA.ID_CELDA.Contains(buscar) || w.CELDA.SECTOR.DESCR.Contains(buscar) ||
                                                                        w.CELDA.SECTOR.EDIFICIO.DESCR.Contains(buscar)) && w.ID_CENTRO == centro.ID_CENTRO);
                                        }
                                        else
                                        {
                                            return GetData().Where(w => (w.ID_CAMA == intBuscar || w.CELDA.ID_CELDA.Contains(buscar) || w.CELDA.SECTOR.EDIFICIO.DESCR.Contains(buscar))
                                                                        && w.ID_CENTRO == centro.ID_CENTRO && w.ID_SECTOR == sector.ID_SECTOR && w.ID_EDIFICIO == sector.ID_EDIFICIO);
                                        }
                                        #endregion
                                    }
                                    else
                                    {
                                        #region sector
                                        if (sector.ID_SECTOR == 0)
                                        {
                                            return GetData().Where(w => (w.ID_CAMA == intBuscar || w.CELDA.ID_CELDA.Contains(buscar) || w.CELDA.SECTOR.DESCR.Contains(buscar))
                                                                        && w.ID_CENTRO == centro.ID_CENTRO && w.ID_EDIFICIO == edificio.ID_EDIFICIO);
                                        }
                                        else
                                        {
                                            return GetData().Where(w => (w.ID_CAMA == intBuscar || w.CELDA.ID_CELDA.Contains(buscar)) && w.ID_CENTRO == centro.ID_CENTRO &&
                                                                        w.ID_EDIFICIO == edificio.ID_EDIFICIO && w.ID_SECTOR == sector.ID_SECTOR);
                                        }
                                        #endregion
                                    }
                                    #endregion
                                }
                                #endregion
                            }
                            else
                            {
                                #region centro
                                if (centro.ID_CENTRO == 0)
                                {
                                    #region edificio
                                    if (edificio.ID_EDIFICIO == 0)
                                    {
                                        #region sector
                                        if (sector.ID_SECTOR == 0)
                                        {
                                            return GetData().Where(w => (w.ID_CAMA == intBuscar || w.CELDA.ID_CELDA.Contains(buscar) || w.CELDA.SECTOR.DESCR.Contains(buscar) ||
                                                                        w.CELDA.SECTOR.EDIFICIO.DESCR.Contains(buscar) || w.CELDA.SECTOR.EDIFICIO.CENTRO.DESCR.Contains(buscar)) &&
                                                                        w.CELDA.SECTOR.EDIFICIO.CENTRO.ID_MUNICIPIO == municipio.ID_MUNICIPIO);
                                        }
                                        else
                                        {
                                            return GetData().Where(w => (w.ID_CAMA == intBuscar || w.CELDA.ID_CELDA.Contains(buscar) ||
                                                                        w.CELDA.SECTOR.EDIFICIO.CENTRO.DESCR.Contains(buscar)) && w.CELDA.SECTOR.EDIFICIO.CENTRO.ID_MUNICIPIO == municipio.ID_MUNICIPIO
                                                                        && w.ID_SECTOR == sector.ID_SECTOR && w.ID_EDIFICIO == sector.ID_EDIFICIO);
                                        }
                                        #endregion
                                    }
                                    else
                                    {
                                        #region sector
                                        if (sector.ID_SECTOR == 0)
                                        {
                                            return GetData().Where(w => (w.ID_CAMA == intBuscar || w.CELDA.ID_CELDA.Contains(buscar) || w.CELDA.SECTOR.DESCR.Contains(buscar) ||
                                                                        w.CELDA.SECTOR.EDIFICIO.CENTRO.DESCR.Contains(buscar)) && w.CELDA.SECTOR.EDIFICIO.CENTRO.ID_MUNICIPIO == municipio.ID_MUNICIPIO
                                                                        && w.ID_EDIFICIO == edificio.ID_EDIFICIO && w.ID_CENTRO == edificio.ID_CENTRO);
                                        }
                                        else
                                        {
                                            return GetData().Where(w => (w.ID_CAMA == intBuscar || w.CELDA.ID_CELDA.Contains(buscar) || w.CELDA.SECTOR.EDIFICIO.CENTRO.DESCR.Contains(buscar))
                                                                        && w.CELDA.SECTOR.EDIFICIO.CENTRO.ID_MUNICIPIO == municipio.ID_MUNICIPIO && w.ID_EDIFICIO == edificio.ID_EDIFICIO &&
                                                                        w.ID_SECTOR == sector.ID_SECTOR && w.ID_CENTRO == edificio.ID_CENTRO);
                                        }
                                        #endregion
                                    }
                                    #endregion
                                }
                                else
                                {
                                    #region edificio
                                    if (edificio.ID_EDIFICIO == 0)
                                    {
                                        #region sector
                                        if (sector.ID_SECTOR == 0)
                                        {
                                            return GetData().Where(w => (w.ID_CAMA == intBuscar || w.CELDA.ID_CELDA.Contains(buscar) || w.CELDA.SECTOR.DESCR.Contains(buscar) ||
                                                                        w.CELDA.SECTOR.EDIFICIO.DESCR.Contains(buscar)) && w.CELDA.SECTOR.EDIFICIO.CENTRO.ID_MUNICIPIO == municipio.ID_MUNICIPIO
                                                                        && w.ID_CENTRO == centro.ID_CENTRO);
                                        }
                                        else
                                        {
                                            return GetData().Where(w => (w.ID_CAMA == intBuscar || w.CELDA.ID_CELDA.Contains(buscar)) &&
                                                                        w.CELDA.SECTOR.EDIFICIO.CENTRO.ID_MUNICIPIO == municipio.ID_MUNICIPIO && w.ID_CENTRO == centro.ID_CENTRO &&
                                                                        w.ID_SECTOR == sector.ID_SECTOR && w.ID_EDIFICIO == sector.ID_EDIFICIO);
                                        }
                                        #endregion
                                    }
                                    else
                                    {
                                        #region sector
                                        if (sector.ID_SECTOR == 0)
                                        {
                                            return GetData().Where(w => (w.ID_CAMA == intBuscar || w.CELDA.ID_CELDA.Contains(buscar) || w.CELDA.SECTOR.DESCR.Contains(buscar)) &&
                                                                        w.CELDA.SECTOR.EDIFICIO.CENTRO.ID_MUNICIPIO == municipio.ID_MUNICIPIO && w.ID_CENTRO == centro.ID_CENTRO &&
                                                                        w.ID_EDIFICIO == edificio.ID_EDIFICIO);
                                        }
                                        else
                                        {
                                            return GetData().Where(w => (w.ID_CAMA == intBuscar || w.CELDA.ID_CELDA.Contains(buscar)) &&
                                                                        w.CELDA.SECTOR.EDIFICIO.CENTRO.ID_MUNICIPIO == municipio.ID_MUNICIPIO && w.ID_CENTRO == centro.ID_CENTRO &&
                                                                        w.ID_EDIFICIO == edificio.ID_EDIFICIO && w.ID_SECTOR == sector.ID_EDIFICIO);
                                        }
                                        #endregion
                                    }
                                    #endregion
                                }
                                #endregion
                            }
                            #endregion
                        }
                        else
                        {
                            #region municipio
                            if (municipio.ID_MUNICIPIO == 0)
                            {
                                #region centro
                                if (centro.ID_CENTRO == 0)
                                {
                                    #region edificio
                                    if (edificio.ID_EDIFICIO == 0)
                                    {
                                        #region sector
                                        if (sector.ID_SECTOR == 0)
                                        {
                                            return GetData().Where(w => (w.CELDA.ID_CELDA.Contains(buscar) || w.CELDA.SECTOR.DESCR.Contains(buscar) ||
                                                                        w.CELDA.SECTOR.EDIFICIO.DESCR.Contains(buscar) || w.CELDA.SECTOR.EDIFICIO.CENTRO.DESCR.Contains(buscar)));
                                        }
                                        else
                                        {
                                            return GetData().Where(w => (w.CELDA.ID_CELDA.Contains(buscar) || w.CELDA.SECTOR.EDIFICIO.DESCR.Contains(buscar) ||
                                                                        w.CELDA.SECTOR.EDIFICIO.CENTRO.DESCR.Contains(buscar)) && w.ID_SECTOR == sector.ID_SECTOR && w.ID_EDIFICIO == sector.ID_EDIFICIO
                                                                        && w.ID_CENTRO == sector.ID_CENTRO);
                                        }
                                        #endregion
                                    }
                                    else
                                    {
                                        #region sector
                                        if (sector.ID_SECTOR == 0)
                                        {
                                            return GetData().Where(w => (w.CELDA.ID_CELDA.Contains(buscar) || w.CELDA.SECTOR.DESCR.Contains(buscar) ||
                                                                        w.CELDA.SECTOR.EDIFICIO.CENTRO.DESCR.Contains(buscar)) && w.ID_EDIFICIO == edificio.ID_EDIFICIO &&
                                                                        w.ID_CENTRO == edificio.ID_CENTRO);
                                        }
                                        else
                                        {
                                            return GetData().Where(w => (w.CELDA.ID_CELDA.Contains(buscar) || w.CELDA.SECTOR.EDIFICIO.CENTRO.DESCR.Contains(buscar))
                                                                        && w.ID_EDIFICIO == edificio.ID_EDIFICIO && w.ID_SECTOR == sector.ID_SECTOR && w.ID_CENTRO == edificio.ID_CENTRO);
                                        }
                                        #endregion
                                    }
                                    #endregion
                                }
                                else
                                {
                                    #region edificio
                                    if (edificio.ID_EDIFICIO == 0)
                                    {
                                        #region sector
                                        if (sector.ID_SECTOR == 0)
                                        {
                                            return GetData().Where(w => (w.CELDA.ID_CELDA.Contains(buscar) || w.CELDA.SECTOR.DESCR.Contains(buscar) ||
                                                                        w.CELDA.SECTOR.EDIFICIO.DESCR.Contains(buscar)) && w.ID_CENTRO == centro.ID_CENTRO);
                                        }
                                        else
                                        {
                                            return GetData().Where(w => (w.CELDA.ID_CELDA.Contains(buscar) || w.CELDA.SECTOR.EDIFICIO.DESCR.Contains(buscar))
                                                                        && w.ID_CENTRO == centro.ID_CENTRO && w.ID_SECTOR == sector.ID_SECTOR && w.ID_EDIFICIO == sector.ID_EDIFICIO);
                                        }
                                        #endregion
                                    }
                                    else
                                    {
                                        #region sector
                                        if (sector.ID_SECTOR == 0)
                                        {
                                            return GetData().Where(w => (w.CELDA.ID_CELDA.Contains(buscar) || w.CELDA.SECTOR.DESCR.Contains(buscar))
                                                                        && w.ID_CENTRO == centro.ID_CENTRO && w.ID_EDIFICIO == edificio.ID_EDIFICIO);
                                        }
                                        else
                                        {
                                            return GetData().Where(w => (w.CELDA.ID_CELDA.Contains(buscar)) && w.ID_CENTRO == centro.ID_CENTRO &&
                                                                        w.ID_EDIFICIO == edificio.ID_EDIFICIO && w.ID_SECTOR == sector.ID_SECTOR);
                                        }
                                        #endregion
                                    }
                                    #endregion
                                }
                                #endregion
                            }
                            else
                            {
                                #region centro
                                if (centro.ID_CENTRO == 0)
                                {
                                    #region edificio
                                    if (edificio.ID_EDIFICIO == 0)
                                    {
                                        #region sector
                                        if (sector.ID_SECTOR == 0)
                                        {
                                            return GetData().Where(w => (w.CELDA.ID_CELDA.Contains(buscar) || w.CELDA.SECTOR.DESCR.Contains(buscar) ||
                                                                        w.CELDA.SECTOR.EDIFICIO.DESCR.Contains(buscar) || w.CELDA.SECTOR.EDIFICIO.CENTRO.DESCR.Contains(buscar)) &&
                                                                        w.CELDA.SECTOR.EDIFICIO.CENTRO.ID_MUNICIPIO == municipio.ID_MUNICIPIO);
                                        }
                                        else
                                        {
                                            return GetData().Where(w => (w.CELDA.ID_CELDA.Contains(buscar) ||
                                                                        w.CELDA.SECTOR.EDIFICIO.CENTRO.DESCR.Contains(buscar)) && w.CELDA.SECTOR.EDIFICIO.CENTRO.ID_MUNICIPIO == municipio.ID_MUNICIPIO
                                                                        && w.ID_SECTOR == sector.ID_SECTOR && w.ID_EDIFICIO == sector.ID_EDIFICIO);
                                        }
                                        #endregion
                                    }
                                    else
                                    {
                                        #region sector
                                        if (sector.ID_SECTOR == 0)
                                        {
                                            return GetData().Where(w => (w.CELDA.ID_CELDA.Contains(buscar) || w.CELDA.SECTOR.DESCR.Contains(buscar) ||
                                                                        w.CELDA.SECTOR.EDIFICIO.CENTRO.DESCR.Contains(buscar)) && w.CELDA.SECTOR.EDIFICIO.CENTRO.ID_MUNICIPIO == municipio.ID_MUNICIPIO
                                                                        && w.ID_EDIFICIO == edificio.ID_EDIFICIO && w.ID_CENTRO == edificio.ID_CENTRO);
                                        }
                                        else
                                        {
                                            return GetData().Where(w => (w.CELDA.ID_CELDA.Contains(buscar) || w.CELDA.SECTOR.EDIFICIO.CENTRO.DESCR.Contains(buscar))
                                                                        && w.CELDA.SECTOR.EDIFICIO.CENTRO.ID_MUNICIPIO == municipio.ID_MUNICIPIO && w.ID_EDIFICIO == edificio.ID_EDIFICIO &&
                                                                        w.ID_SECTOR == sector.ID_SECTOR && w.ID_CENTRO == edificio.ID_CENTRO);
                                        }
                                        #endregion
                                    }
                                    #endregion
                                }
                                else
                                {
                                    #region edificio
                                    if (edificio.ID_EDIFICIO == 0)
                                    {
                                        #region sector
                                        if (sector.ID_SECTOR == 0)
                                        {
                                            return GetData().Where(w => (w.CELDA.ID_CELDA.Contains(buscar) || w.CELDA.SECTOR.DESCR.Contains(buscar) ||
                                                                        w.CELDA.SECTOR.EDIFICIO.DESCR.Contains(buscar)) && w.CELDA.SECTOR.EDIFICIO.CENTRO.ID_MUNICIPIO == municipio.ID_MUNICIPIO
                                                                        && w.ID_CENTRO == centro.ID_CENTRO);
                                        }
                                        else
                                        {
                                            return GetData().Where(w => (w.CELDA.ID_CELDA.Contains(buscar)) &&
                                                                        w.CELDA.SECTOR.EDIFICIO.CENTRO.ID_MUNICIPIO == municipio.ID_MUNICIPIO && w.ID_CENTRO == centro.ID_CENTRO &&
                                                                        w.ID_SECTOR == sector.ID_SECTOR && w.ID_EDIFICIO == sector.ID_EDIFICIO);
                                        }
                                        #endregion
                                    }
                                    else
                                    {
                                        #region sector
                                        if (sector.ID_SECTOR == 0)
                                        {
                                            return GetData().Where(w => (w.CELDA.ID_CELDA.Contains(buscar) || w.CELDA.SECTOR.DESCR.Contains(buscar)) &&
                                                                        w.CELDA.SECTOR.EDIFICIO.CENTRO.ID_MUNICIPIO == municipio.ID_MUNICIPIO && w.ID_CENTRO == centro.ID_CENTRO &&
                                                                        w.ID_EDIFICIO == edificio.ID_EDIFICIO);
                                        }
                                        else
                                        {
                                            return GetData().Where(w => (w.CELDA.ID_CELDA.Contains(buscar)) &&
                                                                        w.CELDA.SECTOR.EDIFICIO.CENTRO.ID_MUNICIPIO == municipio.ID_MUNICIPIO && w.ID_CENTRO == centro.ID_CENTRO &&
                                                                        w.ID_EDIFICIO == edificio.ID_EDIFICIO && w.ID_SECTOR == sector.ID_EDIFICIO);
                                        }
                                        #endregion
                                    }
                                    #endregion
                                }
                                #endregion
                            }
                            #endregion
                        }
                    }
                    #endregion
                }
                else
                {
                    #region buscar
                    if (string.IsNullOrEmpty(buscar))
                    {
                        #region municipio
                        if (municipio.ID_MUNICIPIO == 0)
                        {
                            #region centro
                            if (centro.ID_CENTRO == 0)
                            {
                                #region edificio
                                if (edificio.ID_EDIFICIO == 0)
                                {
                                    #region sector
                                    if (sector.ID_SECTOR == 0)
                                    {
                                        return GetData(w => w.ID_CELDA == celda.ID_CELDA);
                                    }
                                    else
                                    {
                                        return GetData().Where(w => w.ID_SECTOR == sector.ID_SECTOR && w.ID_EDIFICIO == sector.ID_EDIFICIO &&
                                                                    w.ID_CENTRO == sector.ID_CENTRO && w.ID_CELDA == celda.ID_CELDA);
                                    }
                                    #endregion
                                }
                                else
                                {
                                    #region sector
                                    if (sector.ID_SECTOR == 0)
                                    {
                                        return GetData().Where(w => w.ID_EDIFICIO == edificio.ID_EDIFICIO && w.ID_CENTRO == edificio.ID_CENTRO && w.ID_CELDA == celda.ID_CELDA);
                                    }
                                    else
                                    {
                                        return GetData().Where(w => w.ID_EDIFICIO == edificio.ID_EDIFICIO && w.ID_CENTRO == edificio.ID_CENTRO &&
                                                                    w.ID_SECTOR == sector.ID_SECTOR && w.ID_CELDA == celda.ID_CELDA);
                                    }
                                    #endregion
                                }
                                #endregion
                            }
                            else
                            {
                                #region edificio
                                if (edificio.ID_EDIFICIO == 0)
                                {
                                    #region sector
                                    if (sector.ID_SECTOR == 0)
                                    {
                                        return GetData().Where(w => w.ID_CENTRO == centro.ID_CENTRO && w.ID_CELDA == celda.ID_CELDA);
                                    }
                                    else
                                    {
                                        return GetData().Where(w => w.ID_CENTRO == centro.ID_CENTRO && w.ID_SECTOR == sector.ID_SECTOR &&
                                                                    w.ID_EDIFICIO == sector.ID_EDIFICIO && w.ID_CELDA == celda.ID_CELDA);
                                    }
                                    #endregion
                                }
                                else
                                {
                                    #region sector
                                    if (sector.ID_SECTOR == 0)
                                    {
                                        return GetData().Where(w => w.ID_CENTRO == centro.ID_CENTRO && w.ID_EDIFICIO == edificio.ID_EDIFICIO && w.ID_CELDA == celda.ID_CELDA);
                                    }
                                    else
                                    {
                                        return GetData().Where(w => w.ID_CENTRO == centro.ID_CENTRO && w.ID_EDIFICIO == edificio.ID_EDIFICIO &&
                                                                    w.ID_SECTOR == sector.ID_SECTOR && w.ID_CELDA == celda.ID_CELDA);
                                    }
                                    #endregion
                                }
                                #endregion
                            }
                            #endregion
                        }
                        else
                        {
                            #region centro
                            if (centro.ID_CENTRO == 0)
                            {
                                #region edificio
                                if (edificio.ID_EDIFICIO == 0)
                                {
                                    #region sector
                                    if (sector.ID_SECTOR == 0)
                                    {
                                        return GetData().Where(w => w.CELDA.SECTOR.EDIFICIO.CENTRO.ID_MUNICIPIO == municipio.ID_MUNICIPIO && w.ID_CELDA == celda.ID_CELDA);
                                    }
                                    else
                                    {
                                        return GetData().Where(w => w.CELDA.SECTOR.EDIFICIO.CENTRO.ID_MUNICIPIO == municipio.ID_MUNICIPIO && w.ID_SECTOR == sector.ID_SECTOR
                                                                    && w.ID_EDIFICIO == sector.ID_EDIFICIO && w.ID_CENTRO == sector.ID_CENTRO && w.ID_CELDA == celda.ID_CELDA);
                                    }
                                    #endregion
                                }
                                else
                                {
                                    #region sector
                                    if (sector.ID_SECTOR == 0)
                                    {
                                        return GetData().Where(w => w.CELDA.SECTOR.EDIFICIO.CENTRO.ID_MUNICIPIO == municipio.ID_MUNICIPIO && w.ID_EDIFICIO == edificio.ID_EDIFICIO
                                                                    && w.ID_CENTRO == edificio.ID_CENTRO && w.ID_CELDA == celda.ID_CELDA);
                                    }
                                    else
                                    {
                                        return GetData().Where(w => w.CELDA.SECTOR.EDIFICIO.CENTRO.ID_MUNICIPIO == municipio.ID_MUNICIPIO && w.ID_EDIFICIO == edificio.ID_EDIFICIO
                                                                    && w.ID_SECTOR == sector.ID_SECTOR && w.ID_CENTRO == edificio.ID_CENTRO && w.ID_CELDA == celda.ID_CELDA);
                                    }
                                    #endregion
                                }
                                #endregion
                            }
                            else
                            {
                                #region edificio
                                if (edificio.ID_EDIFICIO == 0)
                                {
                                    #region sector
                                    if (sector.ID_SECTOR == 0)
                                    {
                                        return GetData().Where(w => w.CELDA.SECTOR.EDIFICIO.CENTRO.ID_MUNICIPIO == municipio.ID_MUNICIPIO && w.ID_CENTRO == centro.ID_CENTRO && w.ID_CELDA == celda.ID_CELDA);
                                    }
                                    else
                                    {
                                        return GetData().Where(w => w.CELDA.SECTOR.EDIFICIO.CENTRO.ID_MUNICIPIO == municipio.ID_MUNICIPIO && w.ID_CENTRO == centro.ID_CENTRO
                                                                    && w.ID_SECTOR == sector.ID_SECTOR && w.ID_EDIFICIO == sector.ID_EDIFICIO && w.ID_CELDA == celda.ID_CELDA);
                                    }
                                    #endregion
                                }
                                else
                                {
                                    #region sector
                                    if (sector.ID_SECTOR == 0)
                                    {
                                        return GetData().Where(w => w.CELDA.SECTOR.EDIFICIO.CENTRO.ID_MUNICIPIO == municipio.ID_MUNICIPIO && w.ID_CENTRO == centro.ID_CENTRO
                                                                    && w.ID_EDIFICIO == edificio.ID_EDIFICIO && w.ID_CELDA == celda.ID_CELDA);
                                    }
                                    else
                                    {
                                        return GetData().Where(w => w.CELDA.SECTOR.EDIFICIO.CENTRO.ID_MUNICIPIO == municipio.ID_MUNICIPIO && w.ID_CENTRO == centro.ID_CENTRO
                                                                    && w.ID_EDIFICIO == edificio.ID_EDIFICIO && w.ID_SECTOR == sector.ID_SECTOR && w.ID_CELDA == celda.ID_CELDA);
                                    }
                                    #endregion
                                }
                                #endregion
                            }
                            #endregion
                        }
                        #endregion
                    }
                    else
                    {
                        short intBuscar;
                        if (short.TryParse(buscar, out intBuscar))
                        {
                            #region municipio
                            if (municipio.ID_MUNICIPIO == 0)
                            {
                                #region centro
                                if (centro.ID_CENTRO == 0)
                                {
                                    #region edificio
                                    if (edificio.ID_EDIFICIO == 0)
                                    {
                                        #region sector
                                        if (sector.ID_SECTOR == 0)
                                        {
                                            return GetData().Where(w => (w.ID_CAMA == intBuscar || w.CELDA.SECTOR.DESCR.Contains(buscar) ||
                                                                        w.CELDA.SECTOR.EDIFICIO.DESCR.Contains(buscar) || w.CELDA.SECTOR.EDIFICIO.CENTRO.DESCR.Contains(buscar)) && w.CELDA.ID_CELDA == celda.ID_CELDA);
                                        }
                                        else
                                        {
                                            return GetData().Where(w => w.ID_CAMA == intBuscar && w.CELDA.ID_CELDA == celda.ID_CELDA &&
                                                                        w.ID_SECTOR == sector.ID_SECTOR && w.ID_EDIFICIO == sector.ID_EDIFICIO && w.ID_CENTRO == sector.ID_CENTRO);
                                        }
                                        #endregion
                                    }
                                    else
                                    {
                                        #region sector
                                        if (sector.ID_SECTOR == 0)
                                        {
                                            return GetData().Where(w => (w.ID_CAMA == intBuscar || w.CELDA.SECTOR.DESCR.Contains(buscar)) && w.CELDA.ID_CELDA == celda.ID_CELDA &&
                                                                        w.ID_EDIFICIO == edificio.ID_EDIFICIO && w.ID_CENTRO == edificio.ID_CENTRO);
                                        }
                                        else
                                        {
                                            return GetData().Where(w => w.ID_CAMA == intBuscar && w.CELDA.ID_CELDA == celda.ID_CELDA && w.ID_EDIFICIO == edificio.ID_EDIFICIO &&
                                                                        w.ID_SECTOR == sector.ID_SECTOR && w.ID_CENTRO == edificio.ID_CENTRO);
                                        }
                                        #endregion
                                    }
                                    #endregion
                                }
                                else
                                {
                                    #region edificio
                                    if (edificio.ID_EDIFICIO == 0)
                                    {
                                        #region sector
                                        if (sector.ID_SECTOR == 0)
                                        {
                                            return GetData().Where(w => (w.ID_CAMA == intBuscar || w.CELDA.SECTOR.DESCR.Contains(buscar) || w.CELDA.SECTOR.EDIFICIO.DESCR.Contains(buscar)) &&
                                                                        w.CELDA.ID_CELDA == celda.ID_CELDA && w.ID_CENTRO == centro.ID_CENTRO);
                                        }
                                        else
                                        {
                                            return GetData().Where(w => w.ID_CAMA == intBuscar && w.CELDA.ID_CELDA == celda.ID_CELDA && w.ID_CENTRO == centro.ID_CENTRO &&
                                                                        w.ID_SECTOR == sector.ID_SECTOR && w.ID_EDIFICIO == sector.ID_EDIFICIO);
                                        }
                                        #endregion
                                    }
                                    else
                                    {
                                        #region sector
                                        if (sector.ID_SECTOR == 0)
                                        {
                                            return GetData().Where(w => (w.ID_CAMA == intBuscar || w.CELDA.SECTOR.DESCR.Contains(buscar)) && w.CELDA.ID_CELDA == celda.ID_CELDA &&
                                                                        w.ID_CENTRO == centro.ID_CENTRO && w.ID_EDIFICIO == edificio.ID_EDIFICIO);
                                        }
                                        else
                                        {
                                            return GetData().Where(w => w.ID_CAMA == intBuscar && w.CELDA.ID_CELDA == celda.ID_CELDA && w.ID_CENTRO == centro.ID_CENTRO &&
                                                                        w.ID_EDIFICIO == edificio.ID_EDIFICIO && w.ID_SECTOR == sector.ID_SECTOR);
                                        }
                                        #endregion
                                    }
                                    #endregion
                                }
                                #endregion
                            }
                            else
                            {
                                #region centro
                                if (centro.ID_CENTRO == 0)
                                {
                                    #region edificio
                                    if (edificio.ID_EDIFICIO == 0)
                                    {
                                        #region sector
                                        if (sector.ID_SECTOR == 0)
                                        {
                                            return GetData().Where(w => (w.ID_CAMA == intBuscar || w.CELDA.SECTOR.DESCR.Contains(buscar) ||
                                                                        w.CELDA.SECTOR.EDIFICIO.DESCR.Contains(buscar) || w.CELDA.SECTOR.EDIFICIO.CENTRO.DESCR.Contains(buscar)) &&
                                                                        w.CELDA.ID_CELDA == celda.ID_CELDA && w.CELDA.SECTOR.EDIFICIO.CENTRO.ID_MUNICIPIO == municipio.ID_MUNICIPIO);
                                        }
                                        else
                                        {
                                            return GetData().Where(w => w.ID_CAMA == intBuscar && w.CELDA.ID_CELDA == celda.ID_CELDA &&
                                                                        w.CELDA.SECTOR.EDIFICIO.CENTRO.ID_MUNICIPIO == municipio.ID_MUNICIPIO
                                                                        && w.ID_SECTOR == sector.ID_SECTOR && w.ID_EDIFICIO == sector.ID_EDIFICIO);
                                        }
                                        #endregion
                                    }
                                    else
                                    {
                                        #region sector
                                        if (sector.ID_SECTOR == 0)
                                        {
                                            return GetData().Where(w => (w.ID_CAMA == intBuscar || w.CELDA.SECTOR.DESCR.Contains(buscar)) &&
                                                                        w.CELDA.ID_CELDA == celda.ID_CELDA && w.CELDA.SECTOR.EDIFICIO.CENTRO.ID_MUNICIPIO == municipio.ID_MUNICIPIO
                                                                        && w.ID_EDIFICIO == edificio.ID_EDIFICIO);
                                        }
                                        else
                                        {
                                            return GetData().Where(w => w.ID_CAMA == intBuscar && w.CELDA.ID_CELDA == celda.ID_CELDA &&
                                                                        w.CELDA.SECTOR.EDIFICIO.CENTRO.ID_MUNICIPIO == municipio.ID_MUNICIPIO
                                                                        && w.ID_EDIFICIO == edificio.ID_EDIFICIO && w.ID_SECTOR == sector.ID_SECTOR);
                                        }
                                        #endregion
                                    }
                                    #endregion
                                }
                                else
                                {
                                    #region edificio
                                    if (edificio.ID_EDIFICIO == 0)
                                    {
                                        #region sector
                                        if (sector.ID_SECTOR == 0)
                                        {
                                            return GetData().Where(w => (w.ID_CAMA == intBuscar || w.CELDA.SECTOR.DESCR.Contains(buscar) ||
                                                                        w.CELDA.SECTOR.EDIFICIO.DESCR.Contains(buscar)) && w.CELDA.ID_CELDA == celda.ID_CELDA &&
                                                                        w.CELDA.SECTOR.EDIFICIO.CENTRO.ID_MUNICIPIO == municipio.ID_MUNICIPIO && w.ID_CENTRO == centro.ID_CENTRO);
                                        }
                                        else
                                        {
                                            return GetData().Where(w => w.ID_CAMA == intBuscar && w.CELDA.ID_CELDA == celda.ID_CELDA &&
                                                                        w.CELDA.SECTOR.EDIFICIO.CENTRO.ID_MUNICIPIO == municipio.ID_MUNICIPIO && w.ID_CENTRO == centro.ID_CENTRO &&
                                                                        w.ID_SECTOR == sector.ID_SECTOR && w.ID_EDIFICIO == sector.ID_EDIFICIO);
                                        }
                                        #endregion
                                    }
                                    else
                                    {
                                        #region sector
                                        if (sector.ID_SECTOR == 0)
                                        {
                                            return GetData().Where(w => (w.ID_CAMA == intBuscar || w.CELDA.SECTOR.DESCR.Contains(buscar)) && w.CELDA.ID_CELDA == celda.ID_CELDA &&
                                                                        w.CELDA.SECTOR.EDIFICIO.CENTRO.ID_MUNICIPIO == municipio.ID_MUNICIPIO && w.ID_CENTRO == centro.ID_CENTRO &&
                                                                        w.ID_EDIFICIO == edificio.ID_EDIFICIO);
                                        }
                                        else
                                        {
                                            return GetData().Where(w => w.ID_CAMA == intBuscar && w.CELDA.ID_CELDA == celda.ID_CELDA &&
                                                                        w.CELDA.SECTOR.EDIFICIO.CENTRO.ID_MUNICIPIO == municipio.ID_MUNICIPIO && w.ID_CENTRO == centro.ID_CENTRO &&
                                                                        w.ID_EDIFICIO == edificio.ID_EDIFICIO && w.ID_SECTOR == sector.ID_SECTOR);
                                        }
                                        #endregion
                                    }
                                    #endregion
                                }
                                #endregion
                            }
                            #endregion
                        }
                        else
                        {
                            #region municipio
                            if (municipio.ID_MUNICIPIO == 0)
                            {
                                #region centro
                                if (centro.ID_CENTRO == 0)
                                {
                                    #region edificio
                                    if (edificio.ID_EDIFICIO == 0)
                                    {
                                        #region sector
                                        if (sector.ID_SECTOR == 0)
                                        {
                                            return GetData().Where(w => (w.CELDA.SECTOR.DESCR.Contains(buscar) ||
                                                                        w.CELDA.SECTOR.EDIFICIO.DESCR.Contains(buscar) || w.CELDA.SECTOR.EDIFICIO.CENTRO.DESCR.Contains(buscar)) && w.CELDA.ID_CELDA == celda.ID_CELDA);
                                        }
                                        else
                                        {
                                            return GetData().Where(w => w.CELDA.ID_CELDA == celda.ID_CELDA &&
                                                                        w.ID_SECTOR == sector.ID_SECTOR && w.ID_EDIFICIO == sector.ID_EDIFICIO && w.ID_CENTRO == sector.ID_CENTRO);
                                        }
                                        #endregion
                                    }
                                    else
                                    {
                                        #region sector
                                        if (sector.ID_SECTOR == 0)
                                        {
                                            return GetData().Where(w => (w.CELDA.SECTOR.DESCR.Contains(buscar)) && w.CELDA.ID_CELDA == celda.ID_CELDA &&
                                                                        w.ID_EDIFICIO == edificio.ID_EDIFICIO && w.ID_CENTRO == edificio.ID_CENTRO);
                                        }
                                        else
                                        {
                                            return GetData().Where(w => w.CELDA.ID_CELDA == celda.ID_CELDA && w.ID_EDIFICIO == edificio.ID_EDIFICIO &&
                                                                        w.ID_SECTOR == sector.ID_SECTOR && w.ID_CENTRO == edificio.ID_CENTRO);
                                        }
                                        #endregion
                                    }
                                    #endregion
                                }
                                else
                                {
                                    #region edificio
                                    if (edificio.ID_EDIFICIO == 0)
                                    {
                                        #region sector
                                        if (sector.ID_SECTOR == 0)
                                        {
                                            return GetData().Where(w => (w.CELDA.SECTOR.DESCR.Contains(buscar) || w.CELDA.SECTOR.EDIFICIO.DESCR.Contains(buscar)) &&
                                                                        w.CELDA.ID_CELDA == celda.ID_CELDA && w.ID_CENTRO == centro.ID_CENTRO);
                                        }
                                        else
                                        {
                                            return GetData().Where(w => w.ID_CAMA == intBuscar && w.CELDA.ID_CELDA == celda.ID_CELDA && w.ID_CENTRO == centro.ID_CENTRO &&
                                                                        w.ID_SECTOR == sector.ID_SECTOR && w.ID_EDIFICIO == sector.ID_EDIFICIO);
                                        }
                                        #endregion
                                    }
                                    else
                                    {
                                        #region sector
                                        if (sector.ID_SECTOR == 0)
                                        {
                                            return GetData().Where(w => (w.CELDA.SECTOR.DESCR.Contains(buscar)) && w.CELDA.ID_CELDA == celda.ID_CELDA &&
                                                                        w.ID_CENTRO == centro.ID_CENTRO && w.ID_EDIFICIO == edificio.ID_EDIFICIO);
                                        }
                                        else
                                        {
                                            return GetData().Where(w => w.CELDA.ID_CELDA == celda.ID_CELDA && w.ID_CENTRO == centro.ID_CENTRO &&
                                                                        w.ID_EDIFICIO == edificio.ID_EDIFICIO && w.ID_SECTOR == sector.ID_SECTOR);
                                        }
                                        #endregion
                                    }
                                    #endregion
                                }
                                #endregion
                            }
                            else
                            {
                                #region centro
                                if (centro.ID_CENTRO == 0)
                                {
                                    #region edificio
                                    if (edificio.ID_EDIFICIO == 0)
                                    {
                                        #region sector
                                        if (sector.ID_SECTOR == 0)
                                        {
                                            return GetData().Where(w => (w.CELDA.SECTOR.DESCR.Contains(buscar) ||
                                                                        w.CELDA.SECTOR.EDIFICIO.DESCR.Contains(buscar) || w.CELDA.SECTOR.EDIFICIO.CENTRO.DESCR.Contains(buscar)) &&
                                                                        w.CELDA.ID_CELDA == celda.ID_CELDA && w.CELDA.SECTOR.EDIFICIO.CENTRO.ID_MUNICIPIO == municipio.ID_MUNICIPIO);
                                        }
                                        else
                                        {
                                            return GetData().Where(w => w.CELDA.ID_CELDA == celda.ID_CELDA &&
                                                                        w.CELDA.SECTOR.EDIFICIO.CENTRO.ID_MUNICIPIO == municipio.ID_MUNICIPIO
                                                                        && w.ID_SECTOR == sector.ID_SECTOR && w.ID_EDIFICIO == sector.ID_EDIFICIO);
                                        }
                                        #endregion
                                    }
                                    else
                                    {
                                        #region sector
                                        if (sector.ID_SECTOR == 0)
                                        {
                                            return GetData().Where(w => (w.CELDA.SECTOR.DESCR.Contains(buscar)) &&
                                                                        w.CELDA.ID_CELDA == celda.ID_CELDA && w.CELDA.SECTOR.EDIFICIO.CENTRO.ID_MUNICIPIO == municipio.ID_MUNICIPIO
                                                                        && w.ID_EDIFICIO == edificio.ID_EDIFICIO);
                                        }
                                        else
                                        {
                                            return GetData().Where(w => w.CELDA.ID_CELDA == celda.ID_CELDA &&
                                                                        w.CELDA.SECTOR.EDIFICIO.CENTRO.ID_MUNICIPIO == municipio.ID_MUNICIPIO
                                                                        && w.ID_EDIFICIO == edificio.ID_EDIFICIO && w.ID_SECTOR == sector.ID_SECTOR);
                                        }
                                        #endregion
                                    }
                                    #endregion
                                }
                                else
                                {
                                    #region edificio
                                    if (edificio.ID_EDIFICIO == 0)
                                    {
                                        #region sector
                                        if (sector.ID_SECTOR == 0)
                                        {
                                            return GetData().Where(w => (w.CELDA.SECTOR.DESCR.Contains(buscar) ||
                                                                        w.CELDA.SECTOR.EDIFICIO.DESCR.Contains(buscar)) && w.CELDA.ID_CELDA == celda.ID_CELDA &&
                                                                        w.CELDA.SECTOR.EDIFICIO.CENTRO.ID_MUNICIPIO == municipio.ID_MUNICIPIO && w.ID_CENTRO == centro.ID_CENTRO);
                                        }
                                        else
                                        {
                                            return GetData().Where(w => w.ID_CAMA == intBuscar && w.CELDA.ID_CELDA == celda.ID_CELDA &&
                                                                        w.CELDA.SECTOR.EDIFICIO.CENTRO.ID_MUNICIPIO == municipio.ID_MUNICIPIO && w.ID_CENTRO == centro.ID_CENTRO &&
                                                                        w.ID_SECTOR == sector.ID_SECTOR && w.ID_EDIFICIO == sector.ID_EDIFICIO);
                                        }
                                        #endregion
                                    }
                                    else
                                    {
                                        #region sector
                                        if (sector.ID_SECTOR == 0)
                                        {
                                            return GetData().Where(w => (w.CELDA.SECTOR.DESCR.Contains(buscar)) && w.CELDA.ID_CELDA == celda.ID_CELDA &&
                                                                        w.CELDA.SECTOR.EDIFICIO.CENTRO.ID_MUNICIPIO == municipio.ID_MUNICIPIO && w.ID_CENTRO == centro.ID_CENTRO &&
                                                                        w.ID_EDIFICIO == edificio.ID_EDIFICIO);
                                        }
                                        else
                                        {
                                            return GetData().Where(w => w.CELDA.ID_CELDA == celda.ID_CELDA &&
                                                                        w.CELDA.SECTOR.EDIFICIO.CENTRO.ID_MUNICIPIO == municipio.ID_MUNICIPIO && w.ID_CENTRO == centro.ID_CENTRO &&
                                                                        w.ID_EDIFICIO == edificio.ID_EDIFICIO && w.ID_SECTOR == sector.ID_SECTOR);
                                        }
                                        #endregion
                                    }
                                    #endregion
                                }
                                #endregion
                            }
                            #endregion
                        }
                    }
                    #endregion
                }

            }
            catch (Exception ex)
            {
                throw new ApplicationException(ex.Message);
            }
        }


        /// <summary>
        /// metodo que se conectara a la base de datos para regresar un registro 
        /// </summary>
        /// <param name="Id">valor de la llave primaria para obtener registro</param>
        /// <returns>objeto de tipo "ETNIA"</returns>
        public CAMA ObtenerCama(short ID_CENTRO, short ID_EDIFICIO, short ID_SECTOR, string ID_CELDA, short ID_CAMA)
        {
            try
            {
                return GetData().Where(w =>
                    w.ID_CENTRO == ID_CENTRO &&
                    w.ID_EDIFICIO == ID_EDIFICIO &&
                    w.ID_SECTOR == ID_SECTOR &&
                    w.ID_CELDA == ID_CELDA &&
                    w.ID_CAMA == ID_CAMA).FirstOrDefault();
            }
            catch (Exception ex)
            {
                throw new ApplicationException(ex.Message);
            }
        }

        /// <summary>
        /// metodo que se conectara a la base de datos para regresar un registro 
        /// </summary>
        /// <param name="Id">valor de la llave primaria para obtener registro</param>
        /// <returns>objeto de tipo "ETNIA"</returns>
        public IQueryable<CAMA> Obtener(int Id)
        {
            try
            {
                return GetData().Where(w => w.ID_CAMA == Id);
            }
            catch (Exception ex)
            {
                throw new ApplicationException(ex.Message);
            }
        }


        public IQueryable<CAMA> ObtenerTodos(short ID_CENTRO, short? ID_EDIFICIO = 0, short? ID_SECTOR = 0, string ID_CELDA = null)
        {
            try
            {
                var predicate = PredicateBuilder.True<CAMA>();
                predicate = predicate.And(x => x.ID_CENTRO == ID_CENTRO);
                if (ID_EDIFICIO != 0)
                    predicate = predicate.And(x => x.ID_EDIFICIO == ID_EDIFICIO);
                if (ID_SECTOR != 0)
                    predicate = predicate.And(x => x.ID_SECTOR == ID_SECTOR);
                if (ID_CELDA != null)
                    predicate = predicate.And(x => x.ID_CELDA == ID_CELDA);
                return GetData(predicate.Expand());
            }
            catch (Exception ex)
            {

                throw new ApplicationException(ex.Message);
            }
        }

        public IQueryable<CAMA> ObtenerCamasEstancias(List<CELDA> Estancias)
        {
            var predicate = PredicateBuilder.False<CAMA>();
            foreach (var Estancia in Estancias)
            {
                predicate = predicate.Or(x =>
                    x.ID_CENTRO == Estancia.ID_CENTRO &&
                    x.ID_EDIFICIO == Estancia.ID_EDIFICIO &&
                    x.ID_SECTOR == Estancia.ID_SECTOR &&
                    x.ID_CELDA == Estancia.ID_CELDA);
            }
            return GetData(predicate.Expand());
        }

        public bool CamaDisponible(short ID_CENTRO, short ID_EDIFICIO, short ID_SECTOR, string ID_CELDA, short ID_CAMA)
        {
            try
            {
                return GetData(g =>
                    g.ID_CENTRO == ID_CENTRO &&
                    g.ID_EDIFICIO == ID_EDIFICIO &&
                    g.ID_SECTOR == ID_SECTOR &&
                    g.ID_CELDA == ID_CELDA &&
                    g.ID_CAMA == ID_CAMA).
                    FirstOrDefault().ESTATUS == "S";
            }
            catch (Exception ex)
            {

                throw new ApplicationException(ex.Message);
            }
        }


        public bool Liberar(List<CAMA> Camas_Por_Liberar)
        {
            try
            {
                using (TransactionScope transaccion = new TransactionScope(TransactionScopeOption.RequiresNew, new TransactionOptions() { IsolationLevel = System.Transactions.IsolationLevel.ReadCommitted }))
                {
                    //ESTATUS DE LA CAMA = LIBERADA CUANDO SE LE ASIGNA EL VALOR "S"
                    foreach (var Cama in Camas_Por_Liberar)
                    {
                        Cama.ESTATUS = "S";
                        Context.CAMA.Attach(Cama);
                        Context.Entry(Cama).Property(x => x.ESTATUS).IsModified = true;
                    }
                    Context.SaveChanges();
                    return true;
                }
            }
            catch (Exception ex)
            {

                throw new ApplicationException(ex.Message);
            }
        }



        public IQueryable<CAMA> ObtenerCamasPlanimetria(short? Centro = null, short? Edificio = null, short? Sector = null, int? SectorObservacion = null)
        {
            try
            {
                var predicate = PredicateBuilder.True<CAMA>();
                if (Centro.HasValue)
                    predicate = predicate.And(x => x.ID_CENTRO == Centro);
                if (Edificio.HasValue)
                    predicate = predicate.And(x => x.ID_EDIFICIO == Edificio);
                if (Sector.HasValue)
                    predicate = predicate.And(x => x.ID_SECTOR == Sector);
                //if(SectorObservacion.HasValue)
                //predicate = predicate.And(x => x.ID_CAMA);
                return GetData();
            }
            catch (Exception ex)
            {
                throw new ApplicationException(ex.Message);
            }
        }
        /// <summary>
        /// metodo que se conecta a la base de datos para insertar un registro nuevo
        /// </summary>
        /// <param name="Entity">objeto de tipo "ESTADO" con valores a insertar</param>
        public void Insertar(CAMA Entity)
        {
            try
            {
                Entity.ID_CAMA = GetIDCatalogo<short>("CAMA");
                //Entity.ID_CAMA = GetSequence<short>("CAMA_SEQ");
                Insert(Entity);
            }
            catch (Exception ex)
            {
                throw new ApplicationException(ex.Message);
            }
        }

        /// <summary>
        /// metodo que se conecta a la base de datos para obtener el numero de cama siguiente
        /// </summary>
        /// <param name="Entity">objeto de tipo "CAMA" con valores a insertar</param>
        public short CamaSiguiete(short Centro = 0, short Edificio = 0, short Sector = 0, string Celda = "")
        {
            try
            {
                return GetIDProceso<short>("CAMA", "ID_CAMA", string.Format("ID_CENTRO = {0} AND ID_EDIFICIO = {1} AND ID_SECTOR = {2} AND ID_CELDA = '{3}'", Centro, Edificio, Sector, Celda));
            }
            catch (Exception ex)
            {
                throw new ApplicationException(ex.Message);
            }
        }

        /// <summary>
        /// metodo que se conecta a la base de datos para actualizar un registro
        /// </summary>
        /// <param name="Entity">objeto de tipo "ESTADO" con valores a actualizar</param>
        public bool Actualizar(CAMA Entity)
        {
            try
            {
                return Update(Entity);
            }
            catch (Exception ex)
            {
                if (ex.Message.Contains("part of the object's key information"))
                    throw new ApplicationException("La llave principal no se puede cambiar");
                else
                    throw new ApplicationException(ex.Message);
            }
        }

        /// <summary>
        /// metodo que se conecta a la base de datos para eliminar un registro
        /// </summary>
        /// <param name="Id">valor de la llave primaria para obtener registro</param>
        /// <returns>"True" para eliminado, "False" para no encontrado</returns>
        public bool Eliminar(int Id)
        {
            try
            {
                var Entity = GetData().Where(w => w.ID_CAMA == Id).SingleOrDefault();
                if (Entity != null)
                    return Delete(Entity);
                else
                    return false;
            }
            catch (Exception ex)
            {
                //if (ex.InnerException != null)
                //{
                //    if (ex.InnerException.InnerException.Message.Contains("child record found") || ex.InnerException.InnerException.Message.Contains("registro secundario encontrado"))
                //        throw new ApplicationException("Este registro se encuentra ligado a otro registro, por lo tanto no se puede eliminar");
                //}
                //throw new ApplicationException(ex.Message + " " + (ex.InnerException != null ? ex.InnerException.InnerException.Message : ""));

                if (ex.Message.Contains("child record found"))
                {
                    return false;
                }
            }
            return false;
        }

        public bool GetEstatusCama(short IdCama, string IdCelda, short IdSector, short IdEdificio)
        {
            try
            {
                var Camas = GetData().Where(w => w.ID_CAMA == IdCama && w.ID_CELDA == IdCelda && w.ID_SECTOR == IdSector && w.ID_EDIFICIO == IdEdificio);
                if (Camas.Any())
                    foreach (var item in Camas)
                        if (!string.IsNullOrEmpty(item.ESTATUS) ? item.ESTATUS.Equals("S") : false)
                            return true;

                return false;
            }
            catch (Exception exc)
            {
                return false;
            }
        }

        public bool ActualizarEstatusCama(CAMA Entity)
        {
            try
            {
                Context.CAMA.Attach(Entity);
                Context.Entry(Entity).Property(x => x.ID_CENTRO).IsModified = true;
                Context.Entry(Entity).Property(x => x.ID_CELDA).IsModified = true;
                Context.Entry(Entity).Property(x => x.ID_SECTOR).IsModified = true;
                Context.Entry(Entity).Property(x => x.ID_EDIFICIO).IsModified = true;
                Context.Entry(Entity).Property(x => x.ID_CAMA).IsModified = true;
                Context.SaveChanges();
                //if (Update(Entity))
                return true;
            }
            catch (Exception ex)
            {
                throw new ApplicationException(ex.Message);
            }
            return false;
        }
    }
}