using MahApps.Metro.Controls;
using MahApps.Metro.Controls.Dialogs;
using SSP.Controlador.Catalogo.Justicia;
using SSP.Servidor;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Threading.Tasks;
using System.Windows;
using System.Linq;
using System.Collections.Generic;
using System.Configuration;
namespace ControlPenales
{
    partial class EquipoAreaViewModel : ValidationViewModelBase
    {
        #region Constructor
        public EquipoAreaViewModel()
        {
           
        }
        #endregion

        #region Metodos
        private async void clickSwitch(Object obj)
        {
            switch (obj.ToString())
            {
               case "menu_guardar":
                    //GuardarLoginSeleccionado();
                    GuardarArea();
               break;
               case "menu_salir":
                PrincipalViewModel.SalirMenu();
                break;
                case "agregar_area":
                AgregarArea();
                break;
            }
        }
        
        private async void WindowLoad(EquipoAreaView obj)
        {
            try
            {
                StaticSourcesViewModel.CargarDatosMetodoAsync(CargarLista);
            }
            catch (Exception ex)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al cargar los datos iniciales.", ex);
            }
        }

        private void CargarLista() 
        {
            try
            {
                LstAreas = new ObservableCollection<AREA>(new cArea().ObtenerTodos());
                LstEquiposArea = new ObservableCollection<EQUIPO_AREA>(new cEquipo_Area().Seleccionar(GlobalVar.gIP,GlobalVar.gMAC_ADDRESS));
                System.Windows.Application.Current.Dispatcher.Invoke((Action)(delegate
                {
                    LstAreas.Insert(0, new AREA() { ID_AREA = -1, DESCR = "SELECCIONE" });
                    ConfiguraPermisos();
                    StaticSourcesViewModel.SourceChanged = false;
                }));
            }
            catch (Exception ex)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al cargar listas.", ex);
            }
        }
        #endregion

        #region Area
        private void GuardarLoginSeleccionado()
        {
            try
            {

#if DEBUG
                var oConfigDebug = ConfigurationManager.OpenExeConfiguration(System.Reflection.Assembly.GetEntryAssembly().Location);
                oConfigDebug.AppSettings.Settings["EquipoArea"].Value = Area.ToString();
                oConfigDebug.Save(ConfigurationSaveMode.Modified);
                ConfigurationManager.RefreshSection("appSettings");
#else
                        var filePath = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
                        var path = filePath.FilePath.Substring(0, (filePath.FilePath.Length - (".Config").Length));
                        var oConfig = ConfigurationManager.OpenExeConfiguration(path);
                        oConfig.AppSettings.Settings["EquipoArea"].Value = Area.ToString();
                        oConfig.Save(ConfigurationSaveMode.Modified);
                        ConfigurationManager.RefreshSection("appSettings");
#endif
                GlobalVar.gArea = Area.Value;
                new Dialogos().ConfirmacionDialogo("Éxito", "El área se guardo correctamente.");
                StaticSourcesViewModel.SourceChanged = false;
            }
            catch (Exception ex)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al guardar el área.", ex); 
            }
        }

        private void AgregarArea() {
            try
            {
                if (Area == -1)
                {
                    new Dialogos().ConfirmacionDialogo("Validación", "Favor de seleccionar un área");
                    return;
                }
                if (LstEquiposArea == null)
                    LstEquiposArea = new ObservableCollection<EQUIPO_AREA>();
                if (LstEquiposArea.Count(w => w.ID_AREA == Area) == 0)
                {
                    LstEquiposArea.Add(new EQUIPO_AREA() { IP = GlobalVar.gIP, MAC_ADDRESS = GlobalVar.gMAC_ADDRESS, AREA = SelectedArea});
                }
                else
                    new Dialogos().ConfirmacionDialogo("Validación", "El área seleccionada ya existe");
                Area = -1;
            }
            catch (Exception ex)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al agregar área.", ex); 
            }
        }

        private void GuardarArea()
        {
            try
            {
                var ea = new List<EQUIPO_AREA>();
                if(LstEquiposArea != null)
                { 
                    ea = LstEquiposArea.Select(w => new EQUIPO_AREA(){ IP = w.IP, MAC_ADDRESS = w.MAC_ADDRESS, ID_AREA = w.AREA.ID_AREA, REGISTRO_FEC = Hoy}).ToList();
                    if (new cEquipo_Area().Guardar(GlobalVar.gIP, GlobalVar.gMAC_ADDRESS, ea))
                    { 
                        new Dialogos().ConfirmacionDialogo("Éxito", "La información se guardo correctamente");
                        StaticSourcesViewModel.SourceChanged = false;
                    }
                }
            }
            catch (Exception ex)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al guardar área.", ex); 
            }
        }
        #endregion

        #region Permisos
        private void ConfiguraPermisos()
        {
            try
            {
                var permisos = new cProcesoUsuario().Obtener(enumProcesos.EQUIPO_AREA.ToString(), StaticSourcesViewModel.UsuarioLogin.Username);
                foreach (var p in permisos)
                {
                    if (p.EDITAR == 1)
                        GuardarMenuEnabled = true;
                }
            }
            catch (Exception ex)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al configurar permisos en la pantalla", ex);
            }
        }
        #endregion
    }
    
   
}