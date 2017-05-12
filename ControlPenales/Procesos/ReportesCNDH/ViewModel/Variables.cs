namespace ControlPenales
{
    public partial class ReportesCNDHViewModel
    {
        private System.Collections.ObjectModel.ObservableCollection<SSP.Servidor.CLASIFICACION_JURIDICA> lstClasificaciones;
        public System.Collections.ObjectModel.ObservableCollection<SSP.Servidor.CLASIFICACION_JURIDICA> LstClasificaciones
        {
            get { return lstClasificaciones; }
            set { lstClasificaciones = value; OnPropertyChanged("LstClasificaciones"); }
        }

        private string _ClasificacionI;
        public string ClasificacionI
        {
            get { return _ClasificacionI; }
            set { _ClasificacionI = value; OnPropertyChanged("ClasificacionI"); }
        }

        private short? _SelectedReporte;
        public short? SelectedReporte
        {
            get { return _SelectedReporte; }
            set
            {
                _SelectedReporte = value;
                if (value.HasValue)
                {
                    if (value == (short)eTipoReporte.ADULTO_MAYOR)
                    {
                        EnabledClasif = EnabledFuero = EnabledSexo = true;
                        OnPropertyChanged("EnabledClasif");
                        OnPropertyChanged("EnabledFuero");
                        OnPropertyChanged("EnabledSexo");

                        SelectedSexo = SelectedFuero = ClasificacionI = string.Empty;
                        OnPropertyChanged("SelectedSexo");
                        OnPropertyChanged("SelectedFuero");
                        OnPropertyChanged("ClasificacionI");
                    }

                    if (value == (short)eTipoReporte.ETNIAS)
                    {
                        EnabledClasif = EnabledFuero = EnabledSexo = true;
                        OnPropertyChanged("EnabledClasif");
                        OnPropertyChanged("EnabledFuero");
                        OnPropertyChanged("EnabledSexo");

                        SelectedSexo = SelectedFuero = ClasificacionI = string.Empty;
                        OnPropertyChanged("SelectedSexo");
                        OnPropertyChanged("SelectedFuero");
                        OnPropertyChanged("ClasificacionI");
                    }

                    if (value == (short)eTipoReporte.PREFERENCIAS_SEXUALES_DIFERENTES)
                    {
                        EnabledClasif = EnabledFuero = EnabledSexo = true;
                        OnPropertyChanged("EnabledClasif");
                        OnPropertyChanged("EnabledFuero");
                        OnPropertyChanged("EnabledSexo");

                        SelectedSexo = SelectedFuero = ClasificacionI = string.Empty;
                        OnPropertyChanged("SelectedSexo");
                        OnPropertyChanged("SelectedFuero");
                        OnPropertyChanged("ClasificacionI");
                    }

                    if (value == (short)eTipoReporte.VIH)
                    {
                        EnabledClasif = EnabledFuero = EnabledSexo = true;
                        OnPropertyChanged("EnabledClasif");
                        OnPropertyChanged("EnabledFuero");
                        OnPropertyChanged("EnabledSexo");

                        SelectedSexo = SelectedFuero = ClasificacionI = string.Empty;
                        OnPropertyChanged("SelectedSexo");
                        OnPropertyChanged("SelectedFuero");
                        OnPropertyChanged("ClasificacionI");
                    }

                    if (value == (short)eTipoReporte.ADICCIONES)
                    {
                        EnabledClasif = EnabledFuero = EnabledSexo = true;
                        OnPropertyChanged("EnabledClasif");
                        OnPropertyChanged("EnabledFuero");
                        OnPropertyChanged("EnabledSexo");

                        SelectedSexo = SelectedFuero = ClasificacionI = string.Empty;
                        OnPropertyChanged("SelectedSexo");
                        OnPropertyChanged("SelectedFuero");
                        OnPropertyChanged("ClasificacionI");
                    }

                    if (value == (short)eTipoReporte.DISCAPACIDAD_FISICA)
                    {
                        EnabledClasif = EnabledFuero = EnabledSexo = true;
                        OnPropertyChanged("EnabledClasif");
                        OnPropertyChanged("EnabledFuero");
                        OnPropertyChanged("EnabledSexo");

                        SelectedSexo = SelectedFuero = ClasificacionI = string.Empty;
                        OnPropertyChanged("SelectedSexo");
                        OnPropertyChanged("SelectedFuero");
                        OnPropertyChanged("ClasificacionI");
                    }

                    if (value == (short)eTipoReporte.DISCAPACIDAD_MENTAL)
                    {
                        EnabledClasif = EnabledFuero = EnabledSexo = true;
                        OnPropertyChanged("EnabledClasif");
                        OnPropertyChanged("EnabledFuero");
                        OnPropertyChanged("EnabledSexo");

                        SelectedSexo = SelectedFuero = ClasificacionI = string.Empty;
                        OnPropertyChanged("SelectedSexo");
                        OnPropertyChanged("SelectedFuero");
                        OnPropertyChanged("ClasificacionI");
                    }

                    if (value == (short)eTipoReporte.LISTADO_GENERAL)
                    {
                        EnabledClasif = EnabledFuero = EnabledSexo = true;
                        OnPropertyChanged("EnabledClasif");
                        OnPropertyChanged("EnabledFuero");
                        OnPropertyChanged("EnabledSexo");

                        SelectedSexo = SelectedFuero = ClasificacionI = string.Empty;
                        OnPropertyChanged("SelectedSexo");
                        OnPropertyChanged("SelectedFuero");
                        OnPropertyChanged("ClasificacionI");
                    }
                }

                OnPropertyChanged("SelectedReporte");
            }
        }

        private enum eTipoReporte
        {
            //ESTE ES EL TIPO DE REPORTE A IMPRIMIR
            ADULTO_MAYOR = 1,
            ETNIAS = 2,
            PREFERENCIAS_SEXUALES_DIFERENTES = 3,
            VIH = 4,
            LISTADO_GENERAL = 5,
            ADICCIONES = 6,
            DISCAPACIDAD_FISICA = 7,
            DISCAPACIDAD_MENTAL = 8
        };

        private enum eEtniasOmotidas
        {
            SIN_DATO = 9999
        };

        private enum eclasificacionJuridicaContemplada
        {
            INDICIADO_IMPUTADO = 1,
            PROCESADO = 2,
            SENTENCIADO = 3,
            DISCRECIONAL = 4
        };

        private enum eTipoDiscapacidad
        {
            MENTAL = 1,
            FISICA = 4
        }

        private string _SelectedSexo;
        public string SelectedSexo
        {
            get { return _SelectedSexo; }
            set { _SelectedSexo = value; OnPropertyChanged("SelectedSexo"); }
        }

        private string _SelectedFuero;
        public string SelectedFuero
        {
            get { return _SelectedFuero; }
            set { _SelectedFuero = value; OnPropertyChanged("SelectedFuero"); }
        }

        #region Panatalla
        private System.Windows.Visibility _reportViewerVisible = System.Windows.Visibility.Visible;
        public System.Windows.Visibility ReportViewerVisible
        {
            get { return _reportViewerVisible; }
            set
            {
                _reportViewerVisible = value;
                OnPropertyChanged("ReportViewerVisible");
            }
        }

        Microsoft.Reporting.WinForms.ReportViewer reporte;
        public Microsoft.Reporting.WinForms.ReportViewer Reporte
        {
            get { return reporte; }
            set { reporte = value; OnPropertyChanged("Reporte"); }
        }

        #endregion

        private bool _EnabledSexo = false;
        public bool EnabledSexo
        {
            get { return _EnabledSexo; }
            set { _EnabledSexo = value; OnPropertyChanged("EnabledSexo"); }
        }

        private bool _EnabledFuero = false;
        public bool EnabledFuero
        {
            get { return _EnabledFuero; }
            set { _EnabledFuero = value; OnPropertyChanged("EnabledFuero"); }
        }

        private bool _EnabledClasif = false;
        public bool EnabledClasif
        {
            get { return _EnabledClasif; }
            set { _EnabledClasif = value; OnPropertyChanged("EnabledClasif"); }
        }

        #region Privilegios
        private bool pInsertar = false;
        private bool pEditar = false;
        private bool pConsultar = false;
        private bool pImprimir = false;
        #endregion


        const short CP_ACTIVA = 1;
        const short CP_EN_PROCESO = 6;
    }
}