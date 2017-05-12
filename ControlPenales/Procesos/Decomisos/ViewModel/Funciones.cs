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
using SSP.Controlador.Catalogo.Justicia;
using ControlPenales.Clases;
using System.Windows.Forms;
using System.Windows.Controls;
using ControlPenales.BiometricoServiceReference;

namespace ControlPenales
{
    partial class RegistroDecomisoViewModel
    {
        #region Tabs
        void getTab()
        {
            if (banderaTab == "Oficiales")
            {
                OficialesEnabled = true;
                ObjetosEnabled = false;
                InternosEnabled = false;
                ProveedoresEnabled = false;
                ResumenEnabled = false;
                EmpleadosEnabled = false;
                VisitasEnabled = false;
            }
            else if (banderaTab == "Objetos")
            {
                OficialesEnabled = false;
                ObjetosEnabled = true;
                InternosEnabled = false;
                ProveedoresEnabled = false;
                ResumenEnabled = false;
                EmpleadosEnabled = false;
                VisitasEnabled = false;
            }
            else if (banderaTab == "Internos")
            {
                OficialesEnabled = false;
                ObjetosEnabled = false;
                InternosEnabled = true;
                ProveedoresEnabled = false;
                ResumenEnabled = false;
                EmpleadosEnabled = false;
                VisitasEnabled = false;
            }
            else if (banderaTab == "Proveedores")
            {
                OficialesEnabled = false;
                ObjetosEnabled = false;
                InternosEnabled = false;
                ProveedoresEnabled = true;
                ResumenEnabled = false;
                EmpleadosEnabled = false;
                VisitasEnabled = false;
            }
            else if (banderaTab == "Resumen")
            {
                OficialesEnabled = false;
                ObjetosEnabled = false;
                InternosEnabled = false;
                ProveedoresEnabled = false;
                ResumenEnabled = true;
                EmpleadosEnabled = false;
                VisitasEnabled = false;
            }
            else if (banderaTab == "Empleados")
            {
                OficialesEnabled = false;
                ObjetosEnabled = false;
                InternosEnabled = false;
                ProveedoresEnabled = false;
                ResumenEnabled = false;
                EmpleadosEnabled = true;
                VisitasEnabled = false;
            }
            else if (banderaTab == "Visitas")
            {
                OficialesEnabled = false;
                ObjetosEnabled = false;
                InternosEnabled = false;
                ProveedoresEnabled = false;
                ResumenEnabled = false;
                EmpleadosEnabled = false;
                VisitasEnabled = true;
            }
        }
        
        void setTab()
        {
            OficialesEnabled = true;
            ObjetosEnabled = true;
            InternosEnabled = true;
            ProveedoresEnabled = true;
            ResumenEnabled = true;
            EmpleadosEnabled = true;
            VisitasEnabled = true;
        }
        #endregion

        #region Listado
        private void PopulateListado()
        {
            try
            {
                //System.Windows.Application.Current.Dispatcher.Invoke((Action)(delegate
                //{

                //LstDecomisos = new ObservableCollection<cDecomisos>();
                //if (TipoB == 0) //TODOS
                //{
                //    //PERSONAS
                //    var personas = new cDecomisoPersona().ObtenerTodos(GlobalVar.gCentro, PaternoB, MaternoB, NombreB, 0);
                //    if (personas != null)
                //    {
                //        foreach (var p in personas)
                //        {
                //            byte[] img;
                //            if (p.PERSONA.PERSONA_BIOMETRICO.Where(w => w.ID_TIPO_BIOMETRICO == (short)enumTipoBiometrico.FOTO_FRENTE_REGISTRO && w.ID_FORMATO == (short)enumTipoFormato.FMTO_JPG).Any())
                //                img = p.PERSONA.PERSONA_BIOMETRICO.Where(w => w.ID_TIPO_BIOMETRICO == (short)enumTipoBiometrico.FOTO_FRENTE_REGISTRO && w.ID_FORMATO == (short)enumTipoFormato.FMTO_JPG).FirstOrDefault().BIOMETRICO;
                //            else
                //                img = new Imagenes().getImagenPerson();
                //            LstDecomisos.Add(new cDecomisos() { Nombre = p.PERSONA.NOMBRE, Paterno = p.PERSONA.PATERNO, Materno = p.PERSONA.MATERNO, Tipo = p.PERSONA.TIPO_PERSONA.DESCR,Decomiso = p.DECOMISO, ImagenVisitante = img });
                //        }
                //    }
                //    //INTERNOS
                //    var imputados = new cDecomisoIngreso().ObtenerTodos(GlobalVar.gCentro, PaternoB, MaternoB, NombreB);
                //    if (imputados != null)
                //    {
                //        foreach (var i in imputados)
                //        {
                //            byte[] img;
                //            if (i.INGRESO.INGRESO_BIOMETRICO.Where(w => w.ID_TIPO_BIOMETRICO == (short)enumTipoBiometrico.FOTO_FRENTE_REGISTRO && w.ID_FORMATO == (short)enumTipoFormato.FMTO_JPG).Any())
                //                img = i.INGRESO.INGRESO_BIOMETRICO.Where(w => w.ID_TIPO_BIOMETRICO == (short)enumTipoBiometrico.FOTO_FRENTE_REGISTRO && w.ID_FORMATO == (short)enumTipoFormato.FMTO_JPG).FirstOrDefault().BIOMETRICO;
                //            else
                //                img = new Imagenes().getImagenPerson();
                //            LstDecomisos.Add(new cDecomisos() { Nombre = i.INGRESO.IMPUTADO.NOMBRE, Paterno = i.INGRESO.IMPUTADO.PATERNO, Materno = i.INGRESO.IMPUTADO.MATERNO, Tipo = "IMPUTADO", Decomiso = i.DECOMISO, ImagenVisitante = img});
                //        }
                //    }
                //}
                //else if (TipoB == 5)//IMPUTADOS
                //{
                //    var imputados = new cDecomisoIngreso().ObtenerTodos(GlobalVar.gCentro, PaternoB, MaternoB, NombreB);
                //    if (imputados != null)
                //    {
                //        foreach (var i in imputados)
                //        {
                //            byte[] img;
                //            if (i.INGRESO.INGRESO_BIOMETRICO.Where(w => w.ID_TIPO_BIOMETRICO == (short)enumTipoBiometrico.FOTO_FRENTE_REGISTRO && w.ID_FORMATO == (short)enumTipoFormato.FMTO_JPG).Any())
                //                img = i.INGRESO.INGRESO_BIOMETRICO.Where(w => w.ID_TIPO_BIOMETRICO == (short)enumTipoBiometrico.FOTO_FRENTE_REGISTRO && w.ID_FORMATO == (short)enumTipoFormato.FMTO_JPG).FirstOrDefault().BIOMETRICO;
                //            else
                //                img = new Imagenes().getImagenPerson();
                //            LstDecomisos.Add(new cDecomisos() { Nombre = i.INGRESO.IMPUTADO.NOMBRE, Paterno = i.INGRESO.IMPUTADO.PATERNO, Materno = i.INGRESO.IMPUTADO.MATERNO, Tipo = "IMPUTADO" ,Decomiso = i.DECOMISO, ImagenVisitante = img});
                //        }
                //    }
                //}
                //else if (TipoB == 6)//OFICIALES A CARGO
                //{
                //    var personas = new cDecomisoPersona().ObtenerTodos(GlobalVar.gCentro, PaternoB, MaternoB, NombreB, (short)enumTipoPersona.PERSONA_EMPLEADO, true);
                //    if (personas != null)
                //    {
                //        foreach (var p in personas)
                //        {
                //            byte[] img;
                //            if (p.PERSONA.PERSONA_BIOMETRICO.Where(w => w.ID_TIPO_BIOMETRICO == (short)enumTipoBiometrico.FOTO_FRENTE_REGISTRO && w.ID_FORMATO == (short)enumTipoFormato.FMTO_JPG).Any())
                //                img = p.PERSONA.PERSONA_BIOMETRICO.Where(w => w.ID_TIPO_BIOMETRICO == (short)enumTipoBiometrico.FOTO_FRENTE_REGISTRO && w.ID_FORMATO == (short)enumTipoFormato.FMTO_JPG).FirstOrDefault().BIOMETRICO;
                //            else
                //                img = new Imagenes().getImagenPerson();
                //            LstDecomisos.Add(new cDecomisos() { Nombre = p.PERSONA.NOMBRE, Paterno = p.PERSONA.PATERNO, Materno = p.PERSONA.MATERNO, Tipo = p.PERSONA.TIPO_PERSONA.DESCR, Decomiso = p.DECOMISO, ImagenVisitante = img });
                //        }
                //    }
                //}
                //else//PERSONA
                //{
                //    var personas = new cDecomisoPersona().ObtenerTodos(GlobalVar.gCentro, PaternoB, MaternoB, NombreB, TipoB);
                //    if (personas != null)
                //    {
                //        foreach (var p in personas)
                //        {
                //            byte[] img;
                //            if (p.PERSONA.PERSONA_BIOMETRICO.Where(w => w.ID_TIPO_BIOMETRICO == (short)enumTipoBiometrico.FOTO_FRENTE_REGISTRO && w.ID_FORMATO == (short)enumTipoFormato.FMTO_JPG).Any())
                //                img = p.PERSONA.PERSONA_BIOMETRICO.Where(w => w.ID_TIPO_BIOMETRICO == (short)enumTipoBiometrico.FOTO_FRENTE_REGISTRO && w.ID_FORMATO == (short)enumTipoFormato.FMTO_JPG).FirstOrDefault().BIOMETRICO;
                //            else
                //                img = new Imagenes().getImagenPerson();
                //            LstDecomisos.Add(new cDecomisos() { Nombre = p.PERSONA.NOMBRE, Paterno = p.PERSONA.PATERNO, Materno = p.PERSONA.MATERNO, Tipo = p.PERSONA.TIPO_PERSONA.DESCR, Decomiso = p.DECOMISO, ImagenVisitante = img });
                //        }
                //    }
                //}
                //LstDecomisos.OrderBy(w => w.Decomiso.ID_DECOMISO);
                //DecomisosEmpty = LstDecomisos.Count > 0 ? Visibility.Collapsed : Visibility.Visible;
                //}));
            }
            catch (Exception ex)
            { 
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al consultar decomisos", ex);
            }
        }
        #endregion

        #region Ubicacion Celda
        private void ViewModelArbolUbicacion()
        {
            var u = (new cCentro()).Obtener(GlobalVar.gCentro).FirstOrDefault();
            if (u != null)
            {
                Ubicaciones = u;
            }
        }

        private void SeleccionaUbicacionArbol(Object obj)
        {
            try
            {
                var arbol = (System.Windows.Controls.TreeView)obj;
                var x = arbol.SelectedItem;
                var t = x.GetType();
                if (t.BaseType.Name.ToString().Equals("CELDA"))
                {
                    SelectedCelda = (CELDA)x;
                    Celda = string.Format("{0}-{1}-{2}", SelectedCelda.SECTOR.EDIFICIO.DESCR, SelectedCelda.SECTOR.DESCR, SelectedCelda.ID_CELDA);
                    Celda = Celda.Replace(" ", string.Empty);
                }
                PopUpsViewModels.ClosePopUp(PopUpsViewModels.TipoPopUp.SELECCIONA_UBICACION_CELDA);
            }
            catch (Exception ex)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al seleccionar ubicación en árbol", ex);
            }
        }
        #endregion

        #region Objeto Visible
        private void SetConfiguracion() 
        {
            if (SelectedObjetoTipo != null)
            {
                switch (SelectedObjetoTipo.CONFIGURACION)
                {
                    case 1:
                        SetValidacionesObjetos(1);
                        Configuracion1Visible = true;
                        Configuracion2Visible = Configuracion3Visible = Configuracion4Visible = Configuracion5Visible = Configuracion6Visible = Configuracion7Visible = Configuracion8Visible = false;
                        break;
                    case 2:
                        SetValidacionesObjetos(2);
                        Configuracion2Visible = true;
                        Configuracion1Visible = Configuracion3Visible = Configuracion4Visible = Configuracion5Visible = Configuracion6Visible = Configuracion7Visible = Configuracion8Visible = false;
                        break;
                    case 3:
                        SetValidacionesObjetos(3);
                        Configuracion3Visible = true;
                        Configuracion2Visible = Configuracion1Visible = Configuracion4Visible = Configuracion5Visible = Configuracion6Visible = Configuracion7Visible = Configuracion8Visible = false;
                        break;
                    case 4:
                        SetValidacionesObjetos(4);
                        Configuracion4Visible = true;
                        Configuracion2Visible = Configuracion3Visible = Configuracion1Visible = Configuracion5Visible = Configuracion6Visible = Configuracion7Visible = Configuracion8Visible = false;
                        break;
                    case 5:
                        SetValidacionesObjetos(5);
                        Configuracion5Visible = true;
                        Configuracion2Visible = Configuracion3Visible = Configuracion4Visible = Configuracion1Visible = Configuracion6Visible = Configuracion7Visible = Configuracion8Visible = false;
                        break;
                    case 6:
                        SetValidacionesObjetos(6);
                        Configuracion6Visible = true;
                        Configuracion2Visible = Configuracion3Visible = Configuracion4Visible = Configuracion5Visible = Configuracion1Visible = Configuracion7Visible = Configuracion8Visible = false;
                        break;
                    case 7:
                        SetValidacionesObjetos(7);
                        Configuracion7Visible = true;
                        Configuracion2Visible = Configuracion3Visible = Configuracion4Visible = Configuracion5Visible = Configuracion6Visible = Configuracion1Visible = Configuracion8Visible = false;
                        break;
                    case 8:
                        SetValidacionesObjetos(8);
                        Configuracion8Visible = true;
                        Configuracion2Visible = Configuracion3Visible = Configuracion4Visible = Configuracion5Visible = Configuracion6Visible = Configuracion7Visible = Configuracion1Visible = false;
                        break;
                    default:
                        Configuracion1Visible = Configuracion2Visible = Configuracion3Visible = Configuracion4Visible = Configuracion5Visible = Configuracion6Visible = Configuracion7Visible = Configuracion8Visible = false;
                        break;
                }
            }
            else
            { 
                Configuracion1Visible = Configuracion2Visible = Configuracion3Visible = Configuracion4Visible = Configuracion5Visible = Configuracion6Visible = Configuracion7Visible = Configuracion8Visible = false; 
            }
        }
        #endregion
    }
}
