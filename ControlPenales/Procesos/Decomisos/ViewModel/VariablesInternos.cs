using ControlPenales;

using System;
using System.Threading.Tasks;
using System.Windows;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Windows.Input;
using MahApps.Metro.Controls;
using MahApps.Metro.Controls.Dialogs;
using SSP.Servidor;
using ControlPenales.BiometricoServiceReference;

namespace ControlPenales
{
    partial class RegistroDecomisoViewModel
    {
        #region Internos
        private short? anioI;
        public short? AnioI
        {
            get { return anioI; }
            set { 
                anioI = value; 
                OnPropertyChanged("AnioI"); }
        }

        private int? folioI;
        public int? FolioI
        {
            get { return folioI; }
            set { 
                folioI = value; 
                OnPropertyChanged("FolioI"); }
        }

        private string nombreI;
        public string NombreI
        {
            get { return nombreI; }
            set { 
                nombreI = value; 
                OnPropertyChanged("NombreI"); }
        }

        private string paternoI;
        public string PaternoI
        {
            get { return paternoI; }
            set { 
                paternoI = value; 
                OnPropertyChanged("PaternoI"); }
        }

        private string maternoI;
        public string MaternoI
        {
            get { return maternoI; }
            set { 
                maternoI = value; 
                OnPropertyChanged("MaternoI"); }
        }

        private INGRESO selectedIngreso;
        public INGRESO SelectedIngreso
        {
            get { return selectedIngreso; }
            set { selectedIngreso = value;
            if (value != null)
            {
                if (value.INGRESO_BIOMETRICO.Where(w => w.ID_TIPO_BIOMETRICO == (short)enumTipoBiometrico.FOTO_FRENTE_REGISTRO && w.ID_FORMATO == (short)enumTipoFormato.FMTO_JPG).Any())
                    ImagenIngresoPop = value.INGRESO_BIOMETRICO.Where(w => w.ID_TIPO_BIOMETRICO == (short)enumTipoBiometrico.FOTO_FRENTE_REGISTRO && w.ID_FORMATO == (short)enumTipoFormato.FMTO_JPG).FirstOrDefault().BIOMETRICO;
                else
                    ImagenIngresoPop = new Imagenes().getImagenPerson();
                //Ubicacion
                if (value.ID_UB_CAMA != null)
                    if (value.ID_UB_CAMA > 0)
                {
                    UbicacionInterno = string.Format("{0}-{1}-{2}-{3}", value.CAMA.CELDA.SECTOR.EDIFICIO.DESCR.Trim(),
                                                   value.CAMA.CELDA.SECTOR.DESCR.Trim(),
                                                   value.CAMA.CELDA.ID_CELDA.Trim(),
                                                   value.ID_UB_CAMA);
                }
                else
                    UbicacionInterno = string.Empty;
                
            }
            else
            {
                ImagenIngresoPop = new Imagenes().getImagenPerson();
                UbicacionInterno = string.Empty;
            }
                OnPropertyChanged("SelectedIngreso"); }
        }

        private bool internosEmpty;
        public bool InternosEmpty
        {
            get { return internosEmpty; }
            set { internosEmpty = value; OnPropertyChanged("InternosEmpty"); }
        }

        private byte[] imagenIngresoPop = new Imagenes().getImagenPerson();
        public byte[] ImagenIngresoPop
        {
            get { return imagenIngresoPop; }
            set
            {
                imagenIngresoPop = value;
                OnPropertyChanged("ImagenIngresoPop");
            }
        }

        private byte[] imagenIngresoKardex = new Imagenes().getImagenPerson();
        public byte[] ImagenIngresoKardex
        {
            get { return imagenIngresoKardex; }
            set
            {
                imagenIngresoKardex = value;
                OnPropertyChanged("ImagenIngresoKardex");
            }
        }
        #endregion
    }
}
