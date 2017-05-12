using ControlPenales.BiometricoServiceReference;
using ControlPenales.Clases;
using DPUruNet;
using MahApps.Metro.Controls;
using MahApps.Metro.Controls.Dialogs;
using Oracle.ManagedDataAccess.Client;
using SSP.Controlador.Catalogo.Justicia;
using SSP.Servidor;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;

namespace ControlPenales
{
    public class VerHistorialTratamientoViewModel : ValidationViewModelBase 
    {
        #region Constructores
        public VerHistorialTratamientoViewModel()
        {
        }
        public VerHistorialTratamientoViewModel(INGRESO ingreso)
        {
            this.Ingreso = ingreso;
        }
        #endregion

        #region Propiedades
        private INGRESO ingreso;
        public INGRESO Ingreso
        {
            get { return ingreso; }
            set { ingreso = value; OnPropertyChanged("Ingreso"); }
        }

        private int ROW;

        private int COLUMN;

        private bool isalertcolumnas;

        private bool isalertcolumnassame;
        
        private bool _handleSelection = true;
        
        public bool HandleSelection
        {
            get { return _handleSelection; }
            set
            {
                if (value)
                    isalertcolumnas = true;

                _handleSelection = value;
            }
        }
  
        private IList<EJE> ListadoEjes;
        
        private List<Reticula> ListaReticula;
        
        private string _ErrorText = string.Empty;
        
        public string ErrorText
        {
            get { return _ErrorText; }
            set
            {
                _ErrorText = value;
                OnPropertyChanged("ErrorText");
            }
        }
        
        private bool _IsEnabledTratamiento;
        
        public bool IsEnabledTratamiento
        {
            get { return _IsEnabledTratamiento; }
            set
            {
                _IsEnabledTratamiento = value;
                OnPropertyChanged("IsEnabledTratamiento");
            }
        }
        
        System.Windows.Controls.Grid _DynamicGrid;
        #endregion

        #region Comandos
        public ICommand WindowLoading
        {
            get { return new DelegateCommand<VerHistorialTratamientoView>(OnLoad); }
        }
        #endregion
    
        #region Metodos
        private void OnLoad(VerHistorialTratamientoView Window)
        {
            try
            {
                OnLoadTratamiento(Window.DynamicGridHistorico);
            }
            catch(Exception ex)
            {
            
            }
        }

        #region [Metodos Tratamiento]
        #region [Funcionalidad Ventana]
        private void OnLoadTratamiento(Grid DynamicGrid)
        {
            try
            {
                IsEnabledTratamiento = true;
                ListadoEjes = new cEjes().GetData().ToList();

                _DynamicGrid = DynamicGrid;
                LoadGrid(DynamicGrid);
            }
            catch (Exception ex)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al cargar reticula", ex);
            }
        }

        private void AddEje(Grid DynamicGrid, object SelectedValue = null)
        {
            try
            {
                if (COLUMN >= ListadoEjes.Count)
                {
                    SetAlert("No puede agregar mas ejes");
                    return;
                }

                if (COLUMN != 0)
                    if (ValidarCampos(DynamicGrid))
                        return;

                DynamicGrid.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(625) });

                var Ejes = DynamicGrid.Children.Cast<UIElement>().Where(w => Grid.GetRow(w) == 0 && w is ComboBox).Cast<ComboBox>().Select(s => s.SelectedValue).ToList();

                AgregarComboBoxEje(DynamicGrid, ListadoEjes.Where(w => !Ejes.Contains(w.ID_EJE)).ToList(), "DESCR", "ID_EJE", SelectedValue);

                for (int i = 1; i <= ROW; i++)
                    for (int j = COLUMN - 1; j < COLUMN; j++)
                        GenerateFields(DynamicGrid, i, j);
            }
            catch (Exception ex)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al agregar eje", ex);
            }
        }

        private void AddActividad(Grid DynamicGrid)
        {
            try
            {
                if (ROW != 1)
                    if (ValidarCampos(DynamicGrid))
                        return;

                if (ROW == 0 && COLUMN == 0)
                    DynamicGrid.RowDefinitions.Add(new RowDefinition() { Height = new GridLength(60) });
                else
                    DynamicGrid.RowDefinitions.Add(new RowDefinition() { Height = GridLength.Auto });

                for (int j = 0; j < COLUMN; j++)
                    GenerateFields(DynamicGrid, ROW, j);
            }
            catch (Exception ex)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al agregar actividad", ex);
            }
        }

        private void LoadGrid(Grid DynamicGrid)
        {
            try
            {
                SetAlert("Espere Por Favor, Cargando Retícula...", 1000);

                DynamicGrid.Children.Clear();
                DynamicGrid.RowDefinitions.Clear();
                DynamicGrid.ColumnDefinitions.Clear();
                ROW = 0;
                COLUMN = 0;
                var Columnas = new List<IGrouping<EJE, GRUPO_PARTICIPANTE>>();

                if (Ingreso != null)
                    Columnas = new cGrupoParticipante().GetData()
                        .Where(w => w.ID_CENTRO == Ingreso.ID_UB_CENTRO && w.ING_ID_ANIO == Ingreso.ID_ANIO && w.ING_ID_IMPUTADO == Ingreso.ID_IMPUTADO && w.ING_ID_INGRESO == Ingreso.ID_INGRESO)
                        .GroupBy(g => g.EJE1)
                        .OrderBy(o => o.Key.ORDEN)
                        .ToList();

                ListaReticula = new List<Reticula>();
                var ListActividades = new List<TratamientoActividades>();
                foreach (var item in Columnas)
                {
                    ListActividades = new List<TratamientoActividades>();
                    foreach (var subitem in item.OrderBy(o => o.ACTIVIDAD.ORDEN).ToList())
                    {
                        ListActividades.Add(new TratamientoActividades
                        {
                            ActividadValue = subitem.ID_ACTIVIDAD,
                            DepartamentoValue = subitem.ACTIVIDAD.TIPO_PROGRAMA.DEPARTAMENTO.ID_DEPARTAMENTO,
                            ProgramaValue = subitem.ACTIVIDAD.TIPO_PROGRAMA.ID_TIPO_PROGRAMA,
                            EstatusValue = subitem.ESTATUS,
                            grupo_participante = subitem
                        });
                    }
                    ListaReticula.Add(new Reticula
                    {
                        Eje = item.Key.ID_EJE,
                        FechaRegistroValue = item.FirstOrDefault().FEC_REGISTRO,
                        Actividad = ListActividades
                    });
                }

                if (ListaReticula.Count > 0)
                    for (int i = 0; i < ListaReticula.Count; i++)
                    {
                        DynamicGrid.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(625) });

                        var Ejes = DynamicGrid.Children.Cast<UIElement>().Where(w => Grid.GetRow(w) == 0 && w is ComboBox).Cast<ComboBox>().Select(s => s.SelectedValue).ToList();

                        AgregarComboBoxEje(DynamicGrid, ListadoEjes.Where(w => !Ejes.Contains(w.ID_EJE)).ToList(), "DESCR", "ID_EJE", ListaReticula[i].Eje, false);

                        if (ROW == 0)
                        {
                            DynamicGrid.RowDefinitions.Add(new RowDefinition() { Height = new GridLength(60) });
                            AddRow();
                        }
                        LoadFields(DynamicGrid, ROW, i, ListaReticula[i].Eje, ListaReticula[i].Actividad, ListaReticula[i].FechaRegistroValue);
                    }
                else
                    CleanGrid(DynamicGrid);
            }
            catch (Exception ex)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al cargar la información", ex);
            }
        }

        private void CleanGrid(Grid DynamicGrid)
        {
            try
            {
                DynamicGrid.Children.Clear();
                DynamicGrid.RowDefinitions.Clear();
                DynamicGrid.ColumnDefinitions.Clear();
                ROW = 0;
                COLUMN = 0;

                AddEje(DynamicGrid);
                DynamicGrid.RowDefinitions.Add(new RowDefinition() { Height = new GridLength(60) });
                AddRow();
                AddActividad(DynamicGrid);
            }
            catch (Exception ex)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al limpiar la retícula", ex);
            }
        }

        private void SetAlert(string Mensaje, short Duracion = 2500)
        {
            try
            {
                if (!ErrorText.Equals(string.Empty))
                    return;
                ErrorText = Mensaje.ToUpper();
                Task.Factory.StartNew(async () => { await TaskEx.Delay(Duracion); ErrorText = string.Empty; });
            }
            catch (Exception ex)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al mostrar alerta", ex);
            }
        }

        private bool ValidarCampos(Grid DynamicGrid)
        {
            try
            {
                var ValidarEjes = DynamicGrid.Children.Cast<UIElement>().Where(w => w is ComboBox).Cast<ComboBox>().ToList();
                if (ValidarEjes.Any(a => a.SelectedValue == null))
                {
                    SetAlert("Selecciona Eje Para Continuar");
                    return true;
                }
            }
            catch (Exception ex)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al mostrar alerta", ex);
                return true;
            }
            return false;
        }
        #endregion
        #region [Funcionalidad Grid]
        private void GenerateFields(Grid DynamicGrid, int Row, int Column)
        {
            try
            {
                CreateDataGrid(DynamicGrid, Row, Column);
                AgregarTextBlock(DynamicGrid, null, "REGISTRO " + Fechas.GetFechaDateServer.ToShortDateString(), 0, Column);
                AgregarCheckBox(DynamicGrid, "Desmarcar Todos", 0, Column);
                AgregarButton(DynamicGrid, "Quitar", 0, Column);
            }
            catch (Exception ex)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al generar los campos de la celda", ex);
            }
        }

        private void LoadFields(Grid DynamicGrid, int Row, int Column, short? Eje, List<TratamientoActividades> ListValue = null, DateTime? FechaRegistro = null)
        {
            try
            {
                if (ROW == 0 && COLUMN == 0)
                    DynamicGrid.RowDefinitions.Add(new RowDefinition() { Height = new GridLength(60) });
                else
                    DynamicGrid.RowDefinitions.Add(new RowDefinition() { Height = GridLength.Auto });

                if (ListValue.Count > 0)
                {
                    if (Row != 0)
                    {
                        var dg = CreateDataGrid(DynamicGrid, Row, Column);
                        var listactividades = new cActividadEje().GetData().Where(w => w.ID_EJE == Eje.Value).OrderBy(o => o.ACTIVIDAD.TIPO_PROGRAMA.DEPARTAMENTO.ORDEN).ThenBy(t => t.ACTIVIDAD.TIPO_PROGRAMA.ORDEN).ThenBy(t => t.ACTIVIDAD.ORDEN).ToList();

                        var _departamento = string.Empty;
                        var _programa = string.Empty;
                        var _actividad = string.Empty;

                        var itemsourceDatagrid = new List<GridTratamientoActividad>();
                        foreach (var item in listactividades)
                        {
                            itemsourceDatagrid.Add(new GridTratamientoActividad
                            {
                                DEPARTAMENTO = _departamento.Equals(item.ACTIVIDAD.TIPO_PROGRAMA.DEPARTAMENTO.DESCR) ? string.Empty : item.ACTIVIDAD.TIPO_PROGRAMA.DEPARTAMENTO.DESCR,
                                PROGRAMA = _programa.Equals(item.ACTIVIDAD.TIPO_PROGRAMA.NOMBRE) ? string.Empty : item.ACTIVIDAD.TIPO_PROGRAMA.NOMBRE,
                                ACTIVIDAD = _actividad.Equals(item.ACTIVIDAD.DESCR) ? string.Empty : item.ACTIVIDAD.DESCR,
                                ELIGE = ListValue.Where(w => w.grupo_participante.ID_CENTRO == Ingreso.ID_UB_CENTRO && w.grupo_participante.ID_TIPO_PROGRAMA == item.ID_TIPO_PROGRAMA && w.grupo_participante.ID_ACTIVIDAD == item.ID_ACTIVIDAD && w.grupo_participante.ING_ID_CENTRO == Ingreso.ID_CENTRO && w.grupo_participante.ING_ID_ANIO == Ingreso.ID_ANIO && w.grupo_participante.ING_ID_IMPUTADO == Ingreso.ID_IMPUTADO && w.grupo_participante.ING_ID_INGRESO == Ingreso.ID_INGRESO).Any(),

                                ESTATUS =

                                ListValue.Where(w => w.grupo_participante.ID_CENTRO == Ingreso.ID_UB_CENTRO && w.grupo_participante.ID_TIPO_PROGRAMA == item.ID_TIPO_PROGRAMA && w.grupo_participante.ID_ACTIVIDAD == item.ID_ACTIVIDAD && w.grupo_participante.ING_ID_CENTRO == Ingreso.ID_CENTRO && w.grupo_participante.ING_ID_ANIO == Ingreso.ID_ANIO && w.grupo_participante.ING_ID_IMPUTADO == Ingreso.ID_IMPUTADO && w.grupo_participante.ING_ID_INGRESO == Ingreso.ID_INGRESO).Any() ?

                                ListValue.Where(w => w.grupo_participante.ID_CENTRO == Ingreso.ID_UB_CENTRO && w.grupo_participante.ID_TIPO_PROGRAMA == item.ID_TIPO_PROGRAMA && w.grupo_participante.ID_ACTIVIDAD == item.ID_ACTIVIDAD && w.grupo_participante.ING_ID_CENTRO == Ingreso.ID_CENTRO && w.grupo_participante.ING_ID_ANIO == Ingreso.ID_ANIO && w.grupo_participante.ING_ID_IMPUTADO == Ingreso.ID_IMPUTADO && w.grupo_participante.ING_ID_INGRESO == Ingreso.ID_INGRESO).FirstOrDefault().grupo_participante.GRUPO != null ?

                                ListValue.Where(w => w.grupo_participante.ID_CENTRO == Ingreso.ID_UB_CENTRO && w.grupo_participante.ID_TIPO_PROGRAMA == item.ID_TIPO_PROGRAMA && w.grupo_participante.ID_ACTIVIDAD == item.ID_ACTIVIDAD && w.grupo_participante.ING_ID_CENTRO == Ingreso.ID_CENTRO && w.grupo_participante.ING_ID_ANIO == Ingreso.ID_ANIO && w.grupo_participante.ING_ID_IMPUTADO == Ingreso.ID_IMPUTADO && w.grupo_participante.ING_ID_INGRESO == Ingreso.ID_INGRESO).FirstOrDefault().grupo_participante.GRUPO.ID_ESTATUS_GRUPO != 1 ?

                                ListValue.Where(w => w.grupo_participante.ID_CENTRO == Ingreso.ID_UB_CENTRO && w.grupo_participante.ID_TIPO_PROGRAMA == item.ID_TIPO_PROGRAMA && w.grupo_participante.ID_ACTIVIDAD == item.ID_ACTIVIDAD && w.grupo_participante.ING_ID_CENTRO == Ingreso.ID_CENTRO && w.grupo_participante.ING_ID_ANIO == Ingreso.ID_ANIO && w.grupo_participante.ING_ID_IMPUTADO == Ingreso.ID_IMPUTADO && w.grupo_participante.ING_ID_INGRESO == Ingreso.ID_INGRESO).FirstOrDefault().grupo_participante.GRUPO.GRUPO_ESTATUS.DESCR :

                                ListValue.Where(w => w.grupo_participante.ID_CENTRO == Ingreso.ID_UB_CENTRO && w.grupo_participante.ID_TIPO_PROGRAMA == item.ID_TIPO_PROGRAMA && w.grupo_participante.ID_ACTIVIDAD == item.ID_ACTIVIDAD && w.grupo_participante.ING_ID_CENTRO == Ingreso.ID_CENTRO && w.grupo_participante.ING_ID_ANIO == Ingreso.ID_ANIO && w.grupo_participante.ING_ID_IMPUTADO == Ingreso.ID_IMPUTADO && w.grupo_participante.ING_ID_INGRESO == Ingreso.ID_INGRESO).FirstOrDefault().grupo_participante.GRUPO_PARTICIPANTE_ESTATUS.DESCR

                                : ListValue.Where(w => w.grupo_participante.ID_CENTRO == Ingreso.ID_UB_CENTRO && w.grupo_participante.ID_TIPO_PROGRAMA == item.ID_TIPO_PROGRAMA && w.grupo_participante.ID_ACTIVIDAD == item.ID_ACTIVIDAD && w.grupo_participante.ING_ID_CENTRO == Ingreso.ID_CENTRO && w.grupo_participante.ING_ID_ANIO == Ingreso.ID_ANIO && w.grupo_participante.ING_ID_IMPUTADO == Ingreso.ID_IMPUTADO && w.grupo_participante.ING_ID_INGRESO == Ingreso.ID_INGRESO).FirstOrDefault().grupo_participante.GRUPO_PARTICIPANTE_ESTATUS.DESCR

                                : string.Empty,

                                ESTATUSVALUE =

                                ListValue.Where(w => w.grupo_participante.ID_CENTRO == Ingreso.ID_UB_CENTRO && w.grupo_participante.ID_TIPO_PROGRAMA == item.ID_TIPO_PROGRAMA && w.grupo_participante.ID_ACTIVIDAD == item.ID_ACTIVIDAD && w.grupo_participante.ING_ID_CENTRO == Ingreso.ID_CENTRO && w.grupo_participante.ING_ID_ANIO == Ingreso.ID_ANIO && w.grupo_participante.ING_ID_IMPUTADO == Ingreso.ID_IMPUTADO && w.grupo_participante.ING_ID_INGRESO == Ingreso.ID_INGRESO).Any() ?

                                 ListValue.Where(w => w.grupo_participante.ID_CENTRO == Ingreso.ID_UB_CENTRO && w.grupo_participante.ID_TIPO_PROGRAMA == item.ID_TIPO_PROGRAMA && w.grupo_participante.ID_ACTIVIDAD == item.ID_ACTIVIDAD && w.grupo_participante.ING_ID_CENTRO == Ingreso.ID_CENTRO && w.grupo_participante.ING_ID_ANIO == Ingreso.ID_ANIO && w.grupo_participante.ING_ID_IMPUTADO == Ingreso.ID_IMPUTADO && w.grupo_participante.ING_ID_INGRESO == Ingreso.ID_INGRESO).FirstOrDefault().grupo_participante.GRUPO != null ?

                                ListValue.Where(w => w.grupo_participante.ID_CENTRO == Ingreso.ID_UB_CENTRO && w.grupo_participante.ID_TIPO_PROGRAMA == item.ID_TIPO_PROGRAMA && w.grupo_participante.ID_ACTIVIDAD == item.ID_ACTIVIDAD && w.grupo_participante.ING_ID_CENTRO == Ingreso.ID_CENTRO && w.grupo_participante.ING_ID_ANIO == Ingreso.ID_ANIO && w.grupo_participante.ING_ID_IMPUTADO == Ingreso.ID_IMPUTADO && w.grupo_participante.ING_ID_INGRESO == Ingreso.ID_INGRESO).FirstOrDefault().grupo_participante.GRUPO.ID_ESTATUS_GRUPO != 1 ?

                                 ListValue.Where(w => w.grupo_participante.ID_CENTRO == Ingreso.ID_UB_CENTRO && w.grupo_participante.ID_TIPO_PROGRAMA == item.ID_TIPO_PROGRAMA && w.grupo_participante.ID_ACTIVIDAD == item.ID_ACTIVIDAD && w.grupo_participante.ING_ID_CENTRO == Ingreso.ID_CENTRO && w.grupo_participante.ING_ID_ANIO == Ingreso.ID_ANIO && w.grupo_participante.ING_ID_IMPUTADO == Ingreso.ID_IMPUTADO && w.grupo_participante.ING_ID_INGRESO == Ingreso.ID_INGRESO).FirstOrDefault().grupo_participante.GRUPO.GRUPO_ESTATUS.ID_ESTATUS_GRUPO :

                                ListValue.Where(w => w.grupo_participante.ID_CENTRO == Ingreso.ID_UB_CENTRO && w.grupo_participante.ID_TIPO_PROGRAMA == item.ID_TIPO_PROGRAMA && w.grupo_participante.ID_ACTIVIDAD == item.ID_ACTIVIDAD && w.grupo_participante.ING_ID_CENTRO == Ingreso.ID_CENTRO && w.grupo_participante.ING_ID_ANIO == Ingreso.ID_ANIO && w.grupo_participante.ING_ID_IMPUTADO == Ingreso.ID_IMPUTADO && w.grupo_participante.ING_ID_INGRESO == Ingreso.ID_INGRESO).FirstOrDefault().grupo_participante.GRUPO_PARTICIPANTE_ESTATUS.ID_ESTATUS

                                : ListValue.Where(w => w.grupo_participante.ID_CENTRO == Ingreso.ID_UB_CENTRO && w.grupo_participante.ID_TIPO_PROGRAMA == item.ID_TIPO_PROGRAMA && w.grupo_participante.ID_ACTIVIDAD == item.ID_ACTIVIDAD && w.grupo_participante.ING_ID_CENTRO == Ingreso.ID_CENTRO && w.grupo_participante.ING_ID_ANIO == Ingreso.ID_ANIO && w.grupo_participante.ING_ID_IMPUTADO == Ingreso.ID_IMPUTADO && w.grupo_participante.ING_ID_INGRESO == Ingreso.ID_INGRESO).FirstOrDefault().grupo_participante.GRUPO_PARTICIPANTE_ESTATUS.ID_ESTATUS

                                : new Nullable<short>(),

                                actividad_eje = item,

                                APROVADO =
                                ListValue.Where(w => w.grupo_participante.ID_CENTRO == Ingreso.ID_UB_CENTRO && w.grupo_participante.ID_TIPO_PROGRAMA == item.ID_TIPO_PROGRAMA && w.grupo_participante.ID_ACTIVIDAD == item.ID_ACTIVIDAD && w.grupo_participante.ING_ID_CENTRO == Ingreso.ID_CENTRO && w.grupo_participante.ING_ID_ANIO == Ingreso.ID_ANIO && w.grupo_participante.ING_ID_IMPUTADO == Ingreso.ID_IMPUTADO && w.grupo_participante.ING_ID_INGRESO == Ingreso.ID_INGRESO).Any() ?

                                ListValue.Where(w => w.grupo_participante.ID_CENTRO == Ingreso.ID_UB_CENTRO && w.grupo_participante.ID_TIPO_PROGRAMA == item.ID_TIPO_PROGRAMA && w.grupo_participante.ID_ACTIVIDAD == item.ID_ACTIVIDAD && w.grupo_participante.ING_ID_CENTRO == Ingreso.ID_CENTRO && w.grupo_participante.ING_ID_ANIO == Ingreso.ID_ANIO && w.grupo_participante.ING_ID_IMPUTADO == Ingreso.ID_IMPUTADO && w.grupo_participante.ING_ID_INGRESO == Ingreso.ID_INGRESO).FirstOrDefault().grupo_participante.NOTA_TECNICA.Any() ?

                                ListValue.Where(w => w.grupo_participante.ID_CENTRO == Ingreso.ID_UB_CENTRO && w.grupo_participante.ID_TIPO_PROGRAMA == item.ID_TIPO_PROGRAMA && w.grupo_participante.ID_ACTIVIDAD == item.ID_ACTIVIDAD && w.grupo_participante.ING_ID_CENTRO == Ingreso.ID_CENTRO && w.grupo_participante.ING_ID_ANIO == Ingreso.ID_ANIO && w.grupo_participante.ING_ID_IMPUTADO == Ingreso.ID_IMPUTADO && w.grupo_participante.ING_ID_INGRESO == Ingreso.ID_INGRESO).FirstOrDefault().grupo_participante.NOTA_TECNICA.Where(w => w.ID_ACTIVIDAD == item.ID_ACTIVIDAD && w.ID_CENTRO == Ingreso.ID_CENTRO && w.ID_CONSEC == ListValue.Where(wh => wh.grupo_participante.ID_CENTRO == Ingreso.ID_UB_CENTRO && wh.grupo_participante.ID_TIPO_PROGRAMA == item.ID_TIPO_PROGRAMA && wh.grupo_participante.ID_ACTIVIDAD == item.ID_ACTIVIDAD && wh.grupo_participante.ING_ID_CENTRO == Ingreso.ID_CENTRO && wh.grupo_participante.ING_ID_ANIO == Ingreso.ID_ANIO && wh.grupo_participante.ING_ID_IMPUTADO == Ingreso.ID_IMPUTADO && wh.grupo_participante.ING_ID_INGRESO == Ingreso.ID_INGRESO).FirstOrDefault().grupo_participante.ID_CONSEC && w.ID_GRUPO == ListValue.Where(wh => wh.grupo_participante.ID_CENTRO == Ingreso.ID_UB_CENTRO && wh.grupo_participante.ID_TIPO_PROGRAMA == item.ID_TIPO_PROGRAMA && wh.grupo_participante.ID_ACTIVIDAD == item.ID_ACTIVIDAD && wh.grupo_participante.ING_ID_CENTRO == Ingreso.ID_CENTRO && wh.grupo_participante.ING_ID_ANIO == Ingreso.ID_ANIO && wh.grupo_participante.ING_ID_IMPUTADO == Ingreso.ID_IMPUTADO && wh.grupo_participante.ING_ID_INGRESO == Ingreso.ID_INGRESO).FirstOrDefault().grupo_participante.ID_GRUPO && w.ID_TIPO_PROGRAMA == item.ID_TIPO_PROGRAMA).FirstOrDefault() != null ?

                                ListValue.Where(w => w.grupo_participante.ID_CENTRO == Ingreso.ID_UB_CENTRO && w.grupo_participante.ID_TIPO_PROGRAMA == item.ID_TIPO_PROGRAMA && w.grupo_participante.ID_ACTIVIDAD == item.ID_ACTIVIDAD && w.grupo_participante.ING_ID_CENTRO == Ingreso.ID_CENTRO && w.grupo_participante.ING_ID_ANIO == Ingreso.ID_ANIO && w.grupo_participante.ING_ID_IMPUTADO == Ingreso.ID_IMPUTADO && w.grupo_participante.ING_ID_INGRESO == Ingreso.ID_INGRESO).FirstOrDefault().grupo_participante.NOTA_TECNICA.Where(w => w.ID_ACTIVIDAD == item.ID_ACTIVIDAD && w.ID_CENTRO == Ingreso.ID_CENTRO && w.ID_CONSEC == ListValue.Where(wh => wh.grupo_participante.ID_CENTRO == Ingreso.ID_UB_CENTRO && wh.grupo_participante.ID_TIPO_PROGRAMA == item.ID_TIPO_PROGRAMA && wh.grupo_participante.ID_ACTIVIDAD == item.ID_ACTIVIDAD && wh.grupo_participante.ING_ID_CENTRO == Ingreso.ID_CENTRO && wh.grupo_participante.ING_ID_ANIO == Ingreso.ID_ANIO && wh.grupo_participante.ING_ID_IMPUTADO == Ingreso.ID_IMPUTADO && wh.grupo_participante.ING_ID_INGRESO == Ingreso.ID_INGRESO).FirstOrDefault().grupo_participante.ID_CONSEC && w.ID_GRUPO == ListValue.Where(wh => wh.grupo_participante.ID_CENTRO == Ingreso.ID_UB_CENTRO && wh.grupo_participante.ID_TIPO_PROGRAMA == item.ID_TIPO_PROGRAMA && wh.grupo_participante.ID_ACTIVIDAD == item.ID_ACTIVIDAD && wh.grupo_participante.ING_ID_CENTRO == Ingreso.ID_CENTRO && wh.grupo_participante.ING_ID_ANIO == Ingreso.ID_ANIO && wh.grupo_participante.ING_ID_IMPUTADO == Ingreso.ID_IMPUTADO && wh.grupo_participante.ING_ID_INGRESO == Ingreso.ID_INGRESO).FirstOrDefault().grupo_participante.ID_GRUPO && w.ID_TIPO_PROGRAMA == item.ID_TIPO_PROGRAMA).FirstOrDefault().NOTA_TECNICA_ESTATUS.ID_ESTATUS == 1
                                : false : false : false,
                                NOTA = ListValue.Where(w => w.grupo_participante.ID_CENTRO == Ingreso.ID_UB_CENTRO && w.grupo_participante.ID_TIPO_PROGRAMA == item.ID_TIPO_PROGRAMA && w.grupo_participante.ID_ACTIVIDAD == item.ID_ACTIVIDAD && w.grupo_participante.ING_ID_CENTRO == Ingreso.ID_CENTRO && w.grupo_participante.ING_ID_ANIO == Ingreso.ID_ANIO && w.grupo_participante.ING_ID_IMPUTADO == Ingreso.ID_IMPUTADO && w.grupo_participante.ING_ID_INGRESO == Ingreso.ID_INGRESO).Any() ?

                                ListValue.Where(w => w.grupo_participante.ID_CENTRO == Ingreso.ID_UB_CENTRO && w.grupo_participante.ID_TIPO_PROGRAMA == item.ID_TIPO_PROGRAMA && w.grupo_participante.ID_ACTIVIDAD == item.ID_ACTIVIDAD && w.grupo_participante.ING_ID_CENTRO == Ingreso.ID_CENTRO && w.grupo_participante.ING_ID_ANIO == Ingreso.ID_ANIO && w.grupo_participante.ING_ID_IMPUTADO == Ingreso.ID_IMPUTADO && w.grupo_participante.ING_ID_INGRESO == Ingreso.ID_INGRESO).FirstOrDefault().grupo_participante.NOTA_TECNICA.Any() ?

                                ListValue.Where(w => w.grupo_participante.ID_CENTRO == Ingreso.ID_UB_CENTRO && w.grupo_participante.ID_TIPO_PROGRAMA == item.ID_TIPO_PROGRAMA && w.grupo_participante.ID_ACTIVIDAD == item.ID_ACTIVIDAD && w.grupo_participante.ING_ID_CENTRO == Ingreso.ID_CENTRO && w.grupo_participante.ING_ID_ANIO == Ingreso.ID_ANIO && w.grupo_participante.ING_ID_IMPUTADO == Ingreso.ID_IMPUTADO && w.grupo_participante.ING_ID_INGRESO == Ingreso.ID_INGRESO).FirstOrDefault().grupo_participante.NOTA_TECNICA.Where(w => w.ID_ACTIVIDAD == item.ID_ACTIVIDAD && w.ID_CENTRO == Ingreso.ID_CENTRO && w.ID_CONSEC == ListValue.Where(wh => wh.grupo_participante.ID_CENTRO == Ingreso.ID_UB_CENTRO && wh.grupo_participante.ID_TIPO_PROGRAMA == item.ID_TIPO_PROGRAMA && wh.grupo_participante.ID_ACTIVIDAD == item.ID_ACTIVIDAD && wh.grupo_participante.ING_ID_CENTRO == Ingreso.ID_CENTRO && wh.grupo_participante.ING_ID_ANIO == Ingreso.ID_ANIO && wh.grupo_participante.ING_ID_IMPUTADO == Ingreso.ID_IMPUTADO && wh.grupo_participante.ING_ID_INGRESO == Ingreso.ID_INGRESO).FirstOrDefault().grupo_participante.ID_CONSEC && w.ID_GRUPO == ListValue.Where(wh => wh.grupo_participante.ID_CENTRO == Ingreso.ID_UB_CENTRO && wh.grupo_participante.ID_TIPO_PROGRAMA == item.ID_TIPO_PROGRAMA && wh.grupo_participante.ID_ACTIVIDAD == item.ID_ACTIVIDAD && wh.grupo_participante.ING_ID_CENTRO == Ingreso.ID_CENTRO && wh.grupo_participante.ING_ID_ANIO == Ingreso.ID_ANIO && wh.grupo_participante.ING_ID_IMPUTADO == Ingreso.ID_IMPUTADO && wh.grupo_participante.ING_ID_INGRESO == Ingreso.ID_INGRESO).FirstOrDefault().grupo_participante.ID_GRUPO && w.ID_TIPO_PROGRAMA == item.ID_TIPO_PROGRAMA).FirstOrDefault() != null ?

                                ListValue.Where(w => w.grupo_participante.ID_CENTRO == Ingreso.ID_UB_CENTRO && w.grupo_participante.ID_TIPO_PROGRAMA == item.ID_TIPO_PROGRAMA && w.grupo_participante.ID_ACTIVIDAD == item.ID_ACTIVIDAD && w.grupo_participante.ING_ID_CENTRO == Ingreso.ID_CENTRO && w.grupo_participante.ING_ID_ANIO == Ingreso.ID_ANIO && w.grupo_participante.ING_ID_IMPUTADO == Ingreso.ID_IMPUTADO && w.grupo_participante.ING_ID_INGRESO == Ingreso.ID_INGRESO).FirstOrDefault().grupo_participante.NOTA_TECNICA.Where(w => w.ID_ACTIVIDAD == item.ID_ACTIVIDAD && w.ID_CENTRO == Ingreso.ID_CENTRO && w.ID_CONSEC == ListValue.Where(wh => wh.grupo_participante.ID_CENTRO == Ingreso.ID_UB_CENTRO && wh.grupo_participante.ID_TIPO_PROGRAMA == item.ID_TIPO_PROGRAMA && wh.grupo_participante.ID_ACTIVIDAD == item.ID_ACTIVIDAD && wh.grupo_participante.ING_ID_CENTRO == Ingreso.ID_CENTRO && wh.grupo_participante.ING_ID_ANIO == Ingreso.ID_ANIO && wh.grupo_participante.ING_ID_IMPUTADO == Ingreso.ID_IMPUTADO && wh.grupo_participante.ING_ID_INGRESO == Ingreso.ID_INGRESO).FirstOrDefault().grupo_participante.ID_CONSEC && w.ID_GRUPO == ListValue.Where(wh => wh.grupo_participante.ID_CENTRO == Ingreso.ID_UB_CENTRO && wh.grupo_participante.ID_TIPO_PROGRAMA == item.ID_TIPO_PROGRAMA && wh.grupo_participante.ID_ACTIVIDAD == item.ID_ACTIVIDAD && wh.grupo_participante.ING_ID_CENTRO == Ingreso.ID_CENTRO && wh.grupo_participante.ING_ID_ANIO == Ingreso.ID_ANIO && wh.grupo_participante.ING_ID_IMPUTADO == Ingreso.ID_IMPUTADO && wh.grupo_participante.ING_ID_INGRESO == Ingreso.ID_INGRESO).FirstOrDefault().grupo_participante.ID_GRUPO && w.ID_TIPO_PROGRAMA == item.ID_TIPO_PROGRAMA).FirstOrDefault().NOTA
                                : string.Empty : string.Empty : string.Empty,

                                ASISTENCIA =
                                ListValue.Where(w => w.grupo_participante.ID_CENTRO == Ingreso.ID_UB_CENTRO && w.grupo_participante.ID_TIPO_PROGRAMA == item.ID_TIPO_PROGRAMA && w.grupo_participante.ID_ACTIVIDAD == item.ID_ACTIVIDAD && w.grupo_participante.ING_ID_CENTRO == Ingreso.ID_CENTRO && w.grupo_participante.ING_ID_ANIO == Ingreso.ID_ANIO && w.grupo_participante.ING_ID_IMPUTADO == Ingreso.ID_IMPUTADO && w.grupo_participante.ING_ID_INGRESO == Ingreso.ID_INGRESO).Any() ?

                                ObtenerPorcentajeAsistencia(ListValue.Where(w => w.grupo_participante.ID_CENTRO == Ingreso.ID_UB_CENTRO && w.grupo_participante.ID_TIPO_PROGRAMA == item.ID_TIPO_PROGRAMA && w.grupo_participante.ID_ACTIVIDAD == item.ID_ACTIVIDAD && w.grupo_participante.ING_ID_CENTRO == Ingreso.ID_CENTRO && w.grupo_participante.ING_ID_ANIO == Ingreso.ID_ANIO && w.grupo_participante.ING_ID_IMPUTADO == Ingreso.ID_IMPUTADO && w.grupo_participante.ING_ID_INGRESO == Ingreso.ID_INGRESO).FirstOrDefault().grupo_participante, ListValue.Where(w => w.grupo_participante.ID_CENTRO == Ingreso.ID_UB_CENTRO && w.grupo_participante.ID_TIPO_PROGRAMA == item.ID_TIPO_PROGRAMA && w.grupo_participante.ID_ACTIVIDAD == item.ID_ACTIVIDAD && w.grupo_participante.ING_ID_CENTRO == Ingreso.ID_CENTRO && w.grupo_participante.ING_ID_ANIO == Ingreso.ID_ANIO && w.grupo_participante.ING_ID_IMPUTADO == Ingreso.ID_IMPUTADO && w.grupo_participante.ING_ID_INGRESO == Ingreso.ID_INGRESO).Select(s => s.grupo_participante).ToList())
                                : string.Empty
                            });

                            if (!_departamento.Equals(item.ACTIVIDAD.TIPO_PROGRAMA.DEPARTAMENTO.DESCR))
                                _departamento = item.ACTIVIDAD.TIPO_PROGRAMA.DEPARTAMENTO.DESCR;

                            if (!_programa.Equals(item.ACTIVIDAD.TIPO_PROGRAMA.NOMBRE))
                                _programa = item.ACTIVIDAD.TIPO_PROGRAMA.NOMBRE;

                            if (!_actividad.Equals(item.ACTIVIDAD.DESCR))
                                _actividad = item.ACTIVIDAD.DESCR;
                        }
                        dg.ItemsSource = itemsourceDatagrid;
                        AgregarTextBlock(DynamicGrid, null, "REGISTRO " + FechaRegistro.Value.ToShortDateString(), 0, Column);
                    }
                }
            }
            catch (Exception ex)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al cargar los campos desde la base de datos", ex);
            }
        }

        private int AddRow()
        {
            return ROW++;
        }

        private int AddColumn()
        {
            return COLUMN++;
        }
        #endregion
        #region [Funcionalidad Agregar Elementos]
        private DataGrid CreateDataGrid(Grid DynamicGrid, int Row, int Column, short EstatusValue = 1, bool Enabled = true)
        {
            var DynamicDataGrid = new DataGrid();
            try
            {
                DynamicDataGrid.Name = "ContenedorGrid";
                DynamicDataGrid.Columns.Add(new DataGridTextColumn() { Binding = new Binding("DEPARTAMENTO"), Header = "DEPARTAMENTO", CanUserSort = false, IsReadOnly = true, FontSize = 12, MaxWidth = 200, ElementStyle = new Style(typeof(TextBlock)) { Setters = { new Setter(TextBlock.TextWrappingProperty, TextWrapping.Wrap) } } });
                DynamicDataGrid.Columns.Add(new DataGridTextColumn() { Binding = new Binding("PROGRAMA"), Header = "PROGRAMA", CanUserSort = false, IsReadOnly = true, FontSize = 12, MaxWidth = 200, ElementStyle = new Style(typeof(TextBlock)) { Setters = { new Setter(TextBlock.TextWrappingProperty, TextWrapping.Wrap) } } });
                DynamicDataGrid.Columns.Add(new DataGridTextColumn() { Binding = new Binding("ACTIVIDAD"), Header = "ACTIVIDAD", CanUserSort = false, IsReadOnly = true, FontSize = 12, MaxWidth = 135, ElementStyle = new Style(typeof(TextBlock)) { Setters = { new Setter(TextBlock.TextWrappingProperty, TextWrapping.Wrap) } } });

                var RadioFactoryElige = new FrameworkElementFactory(typeof(CheckBox));
                RadioFactoryElige.SetValue(CheckBox.HorizontalAlignmentProperty, HorizontalAlignment.Center);
                //RadioFactoryElige.AddHandler(CheckBox.CheckedEvent, new RoutedEventHandler(chkSelect_Checked));
                //RadioFactoryElige.AddHandler(CheckBox.UncheckedEvent, new RoutedEventHandler(chkSelect_Unchecked));
                RadioFactoryElige.SetBinding(CheckBox.IsCheckedProperty, new Binding("ELIGE") { Mode = BindingMode.TwoWay });

                DynamicDataGrid.Columns.Add(new DataGridTemplateColumn() { CellStyle = new Style() { Setters = { new Setter(CheckBox.HorizontalContentAlignmentProperty, HorizontalAlignment.Center) } }, Header = "ELIGE", CellTemplate = new DataTemplate() { VisualTree = RadioFactoryElige } });

                DynamicDataGrid.Columns.Add(new DataGridTextColumn() { Binding = new Binding("ESTATUS"), Header = "ESTATUS", CanUserSort = false, IsReadOnly = true, FontSize = 12, MaxWidth = 200, ElementStyle = new Style(typeof(TextBlock)) { Setters = { new Setter(TextBlock.TextWrappingProperty, TextWrapping.Wrap) } } });

                var RadioFactoryAprovado = new FrameworkElementFactory(typeof(CheckBox));
                RadioFactoryAprovado.SetValue(CheckBox.HorizontalAlignmentProperty, HorizontalAlignment.Center);
                RadioFactoryAprovado.SetValue(CheckBox.IsEnabledProperty, false);
                RadioFactoryAprovado.SetBinding(CheckBox.IsCheckedProperty, new Binding("APROVADO") { Mode = BindingMode.TwoWay });

                DynamicDataGrid.Columns.Add(new DataGridTemplateColumn() { CellStyle = new Style() { Setters = { new Setter(CheckBox.HorizontalContentAlignmentProperty, HorizontalAlignment.Center) } }, IsReadOnly = true, Header = "APROVADO", CellTemplate = new DataTemplate() { VisualTree = RadioFactoryAprovado } });

                DynamicDataGrid.Columns.Add(new DataGridTextColumn() { Binding = new Binding("NOTA"), Header = "NOTA", CanUserSort = false, IsReadOnly = true, FontSize = 12, MaxWidth = 200, ElementStyle = new Style(typeof(TextBlock)) { Setters = { new Setter(TextBlock.TextWrappingProperty, TextWrapping.Wrap) } } });

                DynamicDataGrid.Columns.Add(new DataGridTextColumn() { Binding = new Binding("ASISTENCIA"), Header = "ASISTENCIA", CanUserSort = false, IsReadOnly = true, FontSize = 12, MaxWidth = 100, ElementStyle = new Style(typeof(TextBlock)) { Setters = { new Setter(TextBlock.TextWrappingProperty, TextWrapping.Wrap), new Setter(TextBlock.HorizontalAlignmentProperty, HorizontalAlignment.Center) } } });


                DynamicDataGrid.FontSize = 14;
                DynamicDataGrid.HorizontalAlignment = HorizontalAlignment.Stretch;
                DynamicDataGrid.VerticalAlignment = VerticalAlignment.Top;
                DynamicDataGrid.Margin = new Thickness(5, 5, 5, 5);
                DynamicDataGrid.AutoGenerateColumns = false;
                DynamicDataGrid.Style = (Style)Application.Current.FindResource("MetroDataGrid");
                DynamicDataGrid.CanUserAddRows = false;
                DynamicDataGrid.CanUserDeleteRows = false;
                DynamicDataGrid.VerticalGridLinesBrush = Brushes.Black;
                DynamicDataGrid.GridLinesVisibility = DataGridGridLinesVisibility.Vertical;
                DynamicDataGrid.SelectionMode = DataGridSelectionMode.Single;
                DynamicDataGrid.AutoGenerateColumns = false;

                Grid.SetRow(DynamicDataGrid, Row);
                Grid.SetColumn(DynamicDataGrid, Column);

                DynamicGrid.Children.Add(DynamicDataGrid);
            }
            catch (Exception ex)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al crear celda", ex);
            }
            return DynamicDataGrid;
        }

        private void AgregarComboBoxEje(Grid DynamicGrid, IEnumerable ItemSource, string DisplayMemberPath, string SelectedValuePath, object SelectedValue, bool Enabled = true)
        {
            try
            {
                var addComboBox = new ComboBox();
                addComboBox.Name = "EJE_" + ROW + "_" + COLUMN;
                addComboBox.ItemsSource = ItemSource;
                addComboBox.DisplayMemberPath = DisplayMemberPath;
                addComboBox.SelectedValuePath = SelectedValuePath;
                addComboBox.SelectedValue = SelectedValue;
                addComboBox.SelectionChanged += (s, e) =>
                {
                    SelectionChanged(DynamicGrid, s, e);
                };
                addComboBox.FontSize = 14;
                addComboBox.FontWeight = FontWeights.Bold;
                addComboBox.VerticalAlignment = VerticalAlignment.Top;
                addComboBox.MaxHeight = 25;
                addComboBox.IsEnabled = Enabled;
                Grid.SetRow(addComboBox, 0);
                Grid.SetColumn(addComboBox, AddColumn());

                DynamicGrid.Children.Add(addComboBox);
            }
            catch (Exception ex)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al crear la lista de ejes", ex);
            }
        }

        private async void SelectionChanged(Grid DynamicGrid, object s, SelectionChangedEventArgs e)
        {
            try
            {
                if (HandleSelection)
                {
                    var previoscombo = ((Grid)((ComboBox)s).Parent).Children.Cast<UIElement>().Where(w => Grid.GetColumn(w) == (((int)((ComboBox)s).GetValue(Grid.ColumnProperty)) - 1) && w is ComboBox).Cast<ComboBox>().FirstOrDefault();
                    if (previoscombo != null)
                        if (previoscombo.SelectedValue == null)
                        {
                            ((ComboBox)s).SelectedValue = null;
                            SetAlert("No Se Puede Dejar Columnas Sin Eje");
                        }

                    var nextcombo = ((Grid)((ComboBox)s).Parent).Children.Cast<UIElement>().Where(w => Grid.GetColumn(w) == (((int)((ComboBox)s).GetValue(Grid.ColumnProperty)) + 1) && w is ComboBox).Cast<ComboBox>().FirstOrDefault();
                    if (nextcombo != null)
                        if (nextcombo.SelectedValue == null)
                            isalertcolumnas = false;

                    if (((int)((ComboBox)s).GetValue(Grid.ColumnProperty) + 1) == COLUMN)
                        isalertcolumnas = true;

                    for (int i = (((int)((ComboBox)s).GetValue(Grid.ColumnProperty)) + 1); i < COLUMN; i++)
                    {
                        if (i == (((int)((ComboBox)s).GetValue(Grid.ColumnProperty)) + 1))
                        {
                            if (isalertcolumnas)
                                isalertcolumnas = await new Dialogos().ConfirmarEliminar("Tratamiento", "la información de las columnas de la derecha serán borradas, desea continuar?") != 1;
                            if (isalertcolumnas)
                            {
                                HandleSelection = false;
                                ((ComboBox)s).SelectedValue = ((EJE)(((object[])(e.RemovedItems))[0])).ID_EJE;
                                isalertcolumnassame = true;
                                break;
                            }
                        }
                        var Eje = ((Grid)((ComboBox)s).Parent).Children.Cast<UIElement>().Where(w => Grid.GetColumn(w) == i && Grid.GetRow(w) == 0 && w is ComboBox).Cast<ComboBox>().FirstOrDefault();

                        Eje.ItemsSource = null;
                        var Ejes = ((Grid)((ComboBox)s).Parent).Children.Cast<UIElement>().Where(w => Grid.GetRow(w) == 0 && Grid.GetColumn(w) <= (int)((ComboBox)s).GetValue(Grid.ColumnProperty) && w is ComboBox).Cast<ComboBox>().Select(se => se.SelectedValue).Where(w => w != null).ToList();

                        Eje.ItemsSource = ListadoEjes.Where(w => !Ejes.Contains(w.ID_EJE)).ToList();
                    }

                    if (isalertcolumnassame)
                    {
                        isalertcolumnassame = false;
                        return;
                    }

                    var DynamicDataGrid = ((Grid)((ComboBox)s).Parent).Children.Cast<UIElement>().Where(w => Grid.GetColumn(w) == (int)((ComboBox)s).GetValue(Grid.ColumnProperty) && w is DataGrid).Cast<DataGrid>().FirstOrDefault();

                    if (((ComboBox)s).SelectedValue != null)
                    {
                        var checkbox = ((Grid)((ComboBox)s).Parent).Children.Cast<UIElement>().Where(w => Grid.GetColumn(w) == (int)((ComboBox)s).GetValue(Grid.ColumnProperty) && w is CheckBox).Cast<CheckBox>().FirstOrDefault();
                        var listactividades = new cActividadEje().GetData().Where(w => w.ID_EJE == (short)((ComboBox)s).SelectedValue).OrderBy(o => o.ACTIVIDAD.TIPO_PROGRAMA.DEPARTAMENTO.ORDEN).ThenBy(t => t.ACTIVIDAD.TIPO_PROGRAMA.ORDEN).ThenBy(t => t.ACTIVIDAD.ORDEN).ToList();

                        var _departamento = string.Empty;
                        var _programa = string.Empty;
                        var _actividad = string.Empty;

                        var itemsourceDatagrid = new List<GridTratamientoActividad>();
                        foreach (var item in listactividades)
                        {
                            itemsourceDatagrid.Add(new GridTratamientoActividad
                            {
                                DEPARTAMENTO = _departamento.Equals(item.ACTIVIDAD.TIPO_PROGRAMA.DEPARTAMENTO.DESCR) ? string.Empty : item.ACTIVIDAD.TIPO_PROGRAMA.DEPARTAMENTO.DESCR,
                                PROGRAMA = _programa.Equals(item.ACTIVIDAD.TIPO_PROGRAMA.NOMBRE) ? string.Empty : item.ACTIVIDAD.TIPO_PROGRAMA.NOMBRE,
                                ACTIVIDAD = _actividad.Equals(item.ACTIVIDAD.DESCR) ? string.Empty : item.ACTIVIDAD.DESCR,
                                ELIGE = checkbox != null ? checkbox.IsChecked.Value : true,
                                ESTATUS = string.Empty,
                                ESTATUSVALUE = new Nullable<short>(),
                                actividad_eje = item
                            });

                            if (!_departamento.Equals(item.ACTIVIDAD.TIPO_PROGRAMA.DEPARTAMENTO.DESCR))
                                _departamento = item.ACTIVIDAD.TIPO_PROGRAMA.DEPARTAMENTO.DESCR;

                            if (!_programa.Equals(item.ACTIVIDAD.TIPO_PROGRAMA.NOMBRE))
                                _programa = item.ACTIVIDAD.TIPO_PROGRAMA.NOMBRE;

                            if (!_actividad.Equals(item.ACTIVIDAD.DESCR))
                                _actividad = item.ACTIVIDAD.DESCR;
                        }

                        DynamicDataGrid.ItemsSource = itemsourceDatagrid;
                    }
                    else
                    {
                        CreateDataGrid(DynamicGrid, (int)(DynamicDataGrid).GetValue(Grid.RowProperty), (int)(DynamicDataGrid).GetValue(Grid.ColumnProperty));
                        ((Grid)((ComboBox)s).Parent).Children.Remove(DynamicDataGrid);
                    }
                }
                HandleSelection = true;
            }
            catch (Exception ex)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al seleccionar un eje", ex);
            }
        }

        private void AgregarTextBlock(Grid DynamicGrid, string Name, string Text, int Row, int Column)
        {
            try
            {
                var addTextBlock = new TextBlock();
                addTextBlock.Text = Text;
                addTextBlock.FontSize = 12;
                addTextBlock.Foreground = Brushes.Black;
                addTextBlock.FontWeight = FontWeights.Bold;
                addTextBlock.Margin = new Thickness(10, 35, 0, 0);

                Grid.SetRow(addTextBlock, Row);
                Grid.SetColumn(addTextBlock, Column);

                DynamicGrid.Children.Add(addTextBlock);
            }
            catch (Exception ex)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al crear etiqueta", ex);
            }
        }

        private void AgregarCheckBox(Grid DynamicGrid, string Text, int Row, int Column)
        {
            try
            {
                var addCheckBox = new CheckBox();
                addCheckBox.Content = Text;
                addCheckBox.IsChecked = true;
                addCheckBox.FontSize = 12;
                addCheckBox.FontWeight = FontWeights.Bold;
                addCheckBox.HorizontalAlignment = HorizontalAlignment.Right;
                addCheckBox.FlowDirection = FlowDirection.RightToLeft;
                addCheckBox.Margin = new Thickness(0, 30, 10, 0);
                addCheckBox.Checked += (s, e) =>
                {
                    try
                    {
                        ((CheckBox)s).Content = "Desmarcar Todo";
                        var Datagrid = ((Grid)((CheckBox)s).Parent).Children.Cast<UIElement>().Where(w => Grid.GetColumn(w) == (((int)((CheckBox)s).GetValue(Grid.ColumnProperty))) && w is DataGrid).Cast<DataGrid>().FirstOrDefault();
                        if (Datagrid.ItemsSource == null)
                            return;
                        var ItemsSource = Datagrid.ItemsSource.Cast<GridTratamientoActividad>().ToList();
                        foreach (var item in ItemsSource)
                            item.ELIGE = true;
                        Datagrid.ItemsSource = ItemsSource;
                    }
                    catch (Exception ex)
                    {
                        StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al Marcar Todo", ex);
                    }
                };
                addCheckBox.Unchecked += (s, e) =>
                {
                    try
                    {
                        ((CheckBox)s).Content = "Marcar Todo";
                        var Datagrid = ((Grid)((CheckBox)s).Parent).Children.Cast<UIElement>().Where(w => Grid.GetColumn(w) == (((int)((CheckBox)s).GetValue(Grid.ColumnProperty))) && w is DataGrid).Cast<DataGrid>().FirstOrDefault();
                        if (Datagrid.ItemsSource == null)
                            return;
                        var ItemsSource = Datagrid.ItemsSource.Cast<GridTratamientoActividad>().ToList();
                        foreach (var item in ItemsSource)
                            item.ELIGE = false;
                        Datagrid.ItemsSource = ItemsSource;
                    }
                    catch (Exception ex)
                    {
                        StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al Desmarcar Todo", ex);
                    }
                };

                Grid.SetRow(addCheckBox, Row);
                Grid.SetColumn(addCheckBox, Column);

                DynamicGrid.Children.Add(addCheckBox);
            }
            catch (Exception ex)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al crear checkbox", ex);
            }
        }

        private void AgregarButton(Grid DynamicGrid, string Content, int Row, int Column)
        {
            var addButton = new Button();
            addButton.Content = Content;
            addButton.FontSize = 12;
            addButton.FontWeight = FontWeights.Bold;
            addButton.Height = 30;
            addButton.Width = 60;
            addButton.Style = Application.Current.FindResource("AccentedSquareButtonStyle") as Style;
            addButton.HorizontalAlignment = HorizontalAlignment.Center;
            addButton.VerticalAlignment = VerticalAlignment.Center;
            addButton.Margin = new Thickness(0, 25, 0, 0);
            addButton.Click += (s, e) =>
            {
                ((Grid)((Button)s).Parent).Children.Cast<UIElement>().Where(w => Grid.GetColumn(w) == (((int)((Button)s).GetValue(Grid.ColumnProperty))) && w is CheckBox).Cast<CheckBox>().FirstOrDefault().IsChecked = false;

                var Datagrid = ((Grid)((Button)s).Parent).Children.Cast<UIElement>().Where(w => Grid.GetColumn(w) == (((int)((Button)s).GetValue(Grid.ColumnProperty))) && w is DataGrid).Cast<DataGrid>().FirstOrDefault();
                if (Datagrid.ItemsSource == null)
                    return;
                var ItemsSource = Datagrid.ItemsSource.Cast<GridTratamientoActividad>().ToList();
                foreach (var item in ItemsSource)
                    item.ELIGE = false;
                Datagrid.ItemsSource = ItemsSource;
            };
            Grid.SetRow(addButton, Row);
            Grid.SetColumn(addButton, Column);

            DynamicGrid.Children.Add(addButton);
        }
        #endregion
        /// <summary>
        /// metodo que obtiene el porcentaje de asistencia del interno
        /// </summary>
        /// <param name="item">objeto de tipo grupo participante</param>
        /// <param name="collection"> colleccion de grupo participante</param>
        /// <returns>cadena de texto con el resultado de la operacion %</returns>
        private string ObtenerPorcentajeAsistencia(GRUPO_PARTICIPANTE item, ICollection<GRUPO_PARTICIPANTE> collection)
        {
            try
            {
                var TotalHoras = 0.0;
                var AsistenciaHoras = 0.0;

                TotalHoras = item.ID_GRUPO.HasValue ? item.GRUPO.GRUPO_HORARIO.Where(w => w.ID_GRUPO == item.ID_GRUPO && w.ESTATUS == 1).Count() : 0;
                AsistenciaHoras = item.GRUPO_ASISTENCIA.Where(w => w.GRUPO_HORARIO.ESTATUS == 1 && (w.ESTATUS == 1 || w.ESTATUS == 3) && collection.Where(wh => wh.GRUPO != null && wh.GRUPO.GRUPO_HORARIO.Where(whe => whe.ESTATUS == 1).Any()).Contains(w.GRUPO_PARTICIPANTE) && w.ASISTENCIA == 1).Count();

                if (double.IsNaN((AsistenciaHoras / TotalHoras)))
                    return string.Empty;

                return string.Format("{0:P2}", (AsistenciaHoras / TotalHoras));
            }
            catch (Exception ex)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al calcular porcentaje de asistencia", ex);
                return string.Empty;
            }
        }
        #endregion
        #endregion
    }
}
