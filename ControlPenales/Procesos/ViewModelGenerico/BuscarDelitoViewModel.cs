using MahApps.Metro.Controls;
using MahApps.Metro.Controls.Dialogs;
using SSP.Controlador.Catalogo.Justicia;
using SSP.Servidor;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace ControlPenales
{
    public class BuscarDelitoViewModel : ValidationViewModelBase
    {
        #region Eventos
        public ICommand WindowLoading
        {
            get { return new DelegateCommand<BuscarDelitoView>(Load); }
        }

        private ICommand buscarClick;
        public ICommand BuscarClick
        {
            get
            {
               return buscarClick ?? (buscarClick = new RelayCommand(ClickEnter));
            }
        }

        private ICommand _onClick;
        public ICommand OnClick
        {
            get { return _onClick ?? (_onClick = new RelayCommand(clickSwitch)); }
        }

        private ICommand _MouseDoubleClickArbolCommand;
        public ICommand MouseDoubleClickArbolCommand
        {
            get { return _MouseDoubleClickArbolCommand ?? (_MouseDoubleClickArbolCommand = new RelayCommand(SeleccionaDelito)); }
        }
        #endregion

        #region Valiables
        private string buscar;
        public string Buscar
        {
            get { return buscar; }
            set { buscar = value;
            if (!string.IsNullOrEmpty(value))
                SearchByName(value);
            else
                StaticSourcesViewModel.CargarDatosMetodoAsync(CargarDelitos);
                OnPropertyChanged("Buscar"); }
        }

        private ObservableCollection<DELITO_TITULO> lstDelitoTitulo;
        public ObservableCollection<DELITO_TITULO> LstDelitoTitulo
        {
            get { return lstDelitoTitulo; }
            set { lstDelitoTitulo = value; OnPropertyChanged("LstDelitoTitulo"); }
        }

        private ObservableCollection<DELITO> lstDelitos;
        public ObservableCollection<DELITO> LstDelitos
        {
            get { return lstDelitos; }
            set { lstDelitos = value; OnPropertyChanged("LstDelitos"); }
        }

        private DELITO delito;
        public DELITO Delito
        {
            get { return delito; }
            set { delito = value; OnPropertyChanged("Delito"); }
        }

        private string fuero = "";
        public string Fuero
        {
            get { return fuero; }
            set { fuero = value;
                LstDelitoTitulo = new ObservableCollection<DELITO_TITULO>(new cDelitoTitulo().ObtenerTodos(fuero));
                OnPropertyChanged("Fuero"); 
            }
        }

        private BuscarDelitoView ventana;
        #endregion

        public BuscarDelitoViewModel() { }

        #region Metodos
        private async void Load(BuscarDelitoView window)
        {
            try
            {
                ventana = window;
                await StaticSourcesViewModel.CargarDatosMetodoAsync(CargarDelitos);
            }
            catch (Exception ex)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al cargar pantalla.", ex);
            }
        }

        private void CargarDelitos()
        {
            try
            {
                LstDelitoTitulo = new ObservableCollection<DELITO_TITULO>(new cDelitoTitulo().ObtenerTodos());
            }
            catch (Exception ex)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al cargar listado de delitos.", ex);
            }
        }

        private void SearchByName(string Partial)
        {
            try
            {
                if (string.IsNullOrEmpty(Partial))
                    return;
                else
                    if (Partial.Length < 3)
                        return;

                LstDelitoTitulo.Clear();//Limpia la lista para que se vaya formando en base a los filtros que ingrese el usuario
                LstDelitos = new ObservableCollection<DELITO>(new cDelito().ObtenerTodos().Where(x => x.DESCR.Contains(Partial) || x.DELITO_GRUPO.DESCR.Contains(Partial) || x.DELITO_GRUPO.DELITO_TITULO.DESCR.Contains(Partial)));

                //consulta los delitos para crear una lista inicial.
                foreach (var item in LstDelitos)
                {
                    if (item.DELITO_GRUPO != null && item.DELITO_GRUPO.DELITO_TITULO != null)
                        if (!LstDelitoTitulo.AsQueryable().Contains(item.DELITO_GRUPO.DELITO_TITULO))
                            LstDelitoTitulo.Add(item.DELITO_GRUPO.DELITO_TITULO);
                        else
                            continue;
                    else
                        continue;
                };

                //Una vez formada la lista, se refinan los resultados una vez mas para hacerla mas precisa y refina los grupos :D
                if (LstDelitoTitulo.AsQueryable().Any())
                {
                    foreach (var item in LstDelitoTitulo.ToList().AsQueryable())
                    {
                        if (item.DELITO_GRUPO.AsQueryable().Any())
                        {
                            foreach (var item2 in item.DELITO_GRUPO.ToList().AsQueryable())//N GRUPOS POR CADA MODALIDD
                            {
                                if (item2.DELITO.AsQueryable().Any())//N DELITOS POR CADA GRUPO
                                    foreach (var item3 in item2.DELITO.ToList().AsQueryable())
                                    {
                                        if (item3.DESCR.Contains(Partial))//el delito si coincide, se agrega ala lista
                                            continue;

                                        item2.DELITO.Remove(item3);//el delito no coincide
                                    };

                                if (!item2.DESCR.Contains(Partial))
                                    item.DELITO_GRUPO.Remove(item2);//el nombre del grupo no coincide con el parametro ingresado por el usuario

                                else
                                    continue;
                            };
                        };

                        if (!item.DESCR.Contains(Partial) && item.DELITO_GRUPO.Count == 0)
                            LstDelitoTitulo.Remove(item);
                    };
                };
            }

            catch (Exception exc)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al obtener el detalle de los delitos", exc);
                return;
            }

            return;
        }

        private async void ClickEnter(Object obj)
        {
            try
            {
                if (obj != null)
                {
                    //cuando es boton no se hace nada porque solamente existe el de buscar, si hay otro habra que castearlos a button y hacer la comparacion
                    var textbox = obj as TextBox;

                    if (textbox != null)
                    {
                        switch (textbox.Name)
                        {
                            case "buscar":
                                Buscar = textbox.Text;
                                break;
                        }
                    }
                }
                if (!string.IsNullOrEmpty(Buscar))
                   SearchByName(Buscar);
                else
                    await StaticSourcesViewModel.CargarDatosMetodoAsync(CargarDelitos);
            }
            catch (Exception ex)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al ingresar búsqueda", ex);
            }
        }

        private void SeleccionaDelito(object obj = null)
        {
            try
            {
                if (obj == null)
                    return;
                if (obj is TreeView)
                { 
                    var arbol = (TreeView)obj;
                    var selected = arbol.SelectedItem;
                    if (selected != null)
                    {
                        if (selected is MODALIDAD_DELITO)
                        { 
                            var md = (MODALIDAD_DELITO)selected;
                            if (md.DELITO != null)
                                Delito = md.DELITO;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al seleccionar delito", ex);
            }
        }

        private void clickSwitch(Object obj)
        {
            try
            {
                switch (obj.ToString())
                { 
                    case "seleccionar_delito":
                        if (Delito == null)
                        {
                            mensajeAlerta("Validación", "Favor de seleccionar un delito");
                            break;
                        }
                        ventana.Close();
                        break;
                    case "cancelar_delito":
                        Delito = null;
                        ventana.Close();
                        break;
                }
            }
            catch (Exception ex)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error en el evento click", ex);
            }
        }

        public async void mensajeAlerta(string titulo = "Advertencia", string mensaje = "Este imputado esta activo en el centro.")
        {
            var metro = Application.Current.Windows[(Application.Current.Windows.Count - 1)] as MetroWindow;
            var mySettings = new MetroDialogSettings()
            {
                AffirmativeButtonText = "Cerrar"
            };
            await metro.ShowMessageAsync(titulo, mensaje, MessageDialogStyle.Affirmative, mySettings);
        }
        #endregion
    }
}
