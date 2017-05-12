using SSP.Servidor;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ControlPenales
{
    class cEMIPendienteListado
    {
        #region Reporte
        public short Anio{set; get;}
        
        public int Folio { set; get; }
        
        public string Paterno { set; get; }
        
        public string Materno { set; get; }
        
        public string Nombre { set; get; }
        
        private string ubicacion;
        public string Ubicacion
        {
            get { return ubicacion; }
            set { ubicacion = value; }
        }
        
        public string Pendiente { set; get; }
        #endregion

        #region Ubicacion
        private CAMA cama;
        public CAMA Cama
        {
            get { return cama; }
            set { cama = value;
            if (value != null)
                contador++;
            }
        }

        private CELDA celda;
        public CELDA Celda
        {
            get { return celda; }
            set { celda = value;
            if (value != null)
                contador++;
            }
        }

        private SECTOR sector;
        public SECTOR Sector
        {
            get { return sector; }
            set { sector = value;
            if (value != null)
                contador++;
            }
        }

        private EDIFICIO edificio;
        public EDIFICIO Edificio
        {
            get { return edificio; }
            set { edificio = value;
            if (value != null)
            { 
                contador++;
                if (contador == 4)
                {
                    ubicacion = string.Format("{0}-{1}{2}-{3}",
                                              Edificio.DESCR.Trim(),
                                              Sector.DESCR.Trim(),
                                              Celda.ID_CELDA.Trim(),
                                              Cama.ID_CAMA);
                }
            }

            }
        }

        private short contador = 0;
        #endregion

        #region Pendiente
        private short esPendiente;
        public short EsPendiente
        {
            get { return esPendiente; }
            set { esPendiente = value;
            if (value == 1)
                Pendiente = "FICHA DE IDENTIFICACIÓN";
            }
        }

        private EMI_SITUACION_JURIDICA pSituacionJuridica;
        public EMI_SITUACION_JURIDICA PSituacionJuridica
        {
            get { return pSituacionJuridica; }
            set { pSituacionJuridica = value;
            if (value != null)
                Pendiente = "SITUACIÓN JURÍDICA";
            }
        }

        private EMI_FACTORES_SOCIO_FAMILIARES pFactoresGrupoFamiliar;
        public EMI_FACTORES_SOCIO_FAMILIARES PFactoresGrupoFamiliar
        {
            get { return pFactoresGrupoFamiliar; }
            set { pFactoresGrupoFamiliar = value;
            if (value != null)
                Pendiente = "FACTORES SOCIO FAMILIARES";
            }
        }

        private int pDrogas;
        public int PDrogas
        {
            get { return pDrogas; }
            set { pDrogas = value;
            if (value > 0)
                Pendiente = "CONDUCTAS PARASOCIALES";
            }
        }

        private EMI_HPS pHPS;
        public EMI_HPS PHPS
        {
            get { return pHPS; }
            set { pHPS = value; 
                if(value != null)
                    Pendiente = "CONDUCTAS PARASOCIALES";
            }
        }

        private EMI_CLAS_CRIMINOLOGICA pClasificacionCriminologica;
        public EMI_CLAS_CRIMINOLOGICA PClasificacionCriminologica
        {
            get { return pClasificacionCriminologica; }
            set { pClasificacionCriminologica = value;
                if(value != null)
                    Pendiente = "CLASIFICACIÓN CRIMINOLOGICA";
            }
        }

 
        #endregion
    }
}
