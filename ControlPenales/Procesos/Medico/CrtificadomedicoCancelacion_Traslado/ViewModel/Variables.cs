using ControlPenales.BiometricoServiceReference;
using SSP.Controlador.Catalogo.Justicia;
using SSP.Servidor;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Windows;
using System.Text.RegularExpressions;
namespace ControlPenales
{
    partial class CertificadoMedicoCancelacion_TrasladoViewModel 
    {
        //private ObservableCollection<TRASLADO_DETALLE> _ListaCertificadoMed;

        //public ObservableCollection<TRASLADO_DETALLE> ListaCertificadoMed
        //{
        //    get { return _ListaCertificadoMed; }
        //    set { _ListaCertificadoMed = value; OnPropertyValidateChanged("ListaCertificadoMed"); }
        //}

        //private TRASLADO_DETALLE _SelectCertificadoMedico;

        //public TRASLADO_DETALLE SelectCertificadoMedico
        //{
        //    get { return _SelectCertificadoMedico; }
        //    set { _SelectCertificadoMedico = value; OnPropertyValidateChanged("SelectCertificadoMedico"); }
        //}
        #region Variables Busqueda

        private string nombreBuscar;
        public string NombreBuscarCertMed
        {
            get { return nombreBuscar; }
            set
            {
                nombreBuscar = value; OnPropertyChanged("NombreBuscarCertMed");
            }
        }

        private string _ApellidoPaternoBuscar;



        public string ApellidoPaternoBuscarCertMed
        {
            get { return _ApellidoPaternoBuscar; }
            set { _ApellidoPaternoBuscar = value; OnPropertyChanged("ApellidoPaternoBuscarCertMed"); }
        }

        private string _ApellidoMaternoBuscar;

        public string ApellidoMaternoBuscarCertMed
        {
            get { return _ApellidoMaternoBuscar; }
            set { _ApellidoMaternoBuscar = value; OnPropertyChanged("ApellidoMaternoBuscarCertMed"); }
        }


        private short? anioBuscar;
        public short? AnioBuscarCertMed
        {
            get { return anioBuscar; }
            set
            {
                anioBuscar = value;
                //if (value != null)
                //{
                //string NewValor= Regex.Replace(value.Trim(), "[^0-9]+", string.Empty);
                //value = NewValor;
                //bool NoNumber = false;
                //var Valor = value.ToString().Trim();
                //for (int i = 0; i < Valor.Length; i++)
                //{

                //    var chars = Valor.Substring(i, 1);
                //    char s = char.Parse(chars.ToString());

                //    if ((char.IsNumber(s)))
                //    {


                //    }
                //    else
                //    {

                //        Valor = Valor.Remove(i, 1);
                //        NoNumber = true;
                //    }

                //}
                //if (NoNumber==false)
                //{
                //    anioBuscar = Valor;
                //}
                //}

                OnPropertyChanged("AnioBuscarCertMed");
            }
        }

        private short? folioBuscar;
        public short? FolioBuscarCertMed
        {
            get { return folioBuscar; }
            set
            {
                folioBuscar = value;
                //if (value != null)
                //{
                //    value = Regex.Replace(value.Trim(), "[^0-9]+", string.Empty);
                //}


                OnPropertyChanged("FolioBuscarCertMed");
            }
        }
        #endregion

        private string _Busqueda;


        public string Busqueda
        {
            get { return _Busqueda; }
            set { _Busqueda = value; OnPropertyChanged("Busqueda"); }
        }

        private string _TituloBusquedaGrid;


        public string TituloBusquedaGrid
        {
            get { return _TituloBusquedaGrid; }
            set { _TituloBusquedaGrid = value; OnPropertyChanged("TituloBusquedaGrid"); }
        }
        #region Menus
        private bool _MenuReporteEnabled;


        public bool MenuReporteEnabled
        {
            get { return _MenuReporteEnabled; }
            set { _MenuReporteEnabled = value; OnPropertyChanged("MenuReporteEnabled"); }
        }
        private bool MenuFichaEnabled;

        #endregion

        private string _CatalogoHeader = "Comprobación Certificado  Médico";


        public string CatalogoHeader
        {
            get { return _CatalogoHeader; }
            set { _CatalogoHeader = value; OnPropertyChanged("CatalogoHeader"); }
        }


        private ObservableCollection<Procesos> _ListProceso;

        public ObservableCollection<Procesos> ListProceso
        {
            get { return _ListProceso; }
            set { _ListProceso = value; OnPropertyChanged("ListProceso"); }
        }

        private bool _EnableTipoCertificado;

        public bool EnableTipoCertificado
        {
            get { return _EnableTipoCertificado; }
            set { _EnableTipoCertificado = value; OnPropertyChanged("EnableTipoCertificado"); }
        }

        private short? _SelectProceso = -1;

        public short? SelectProceso
        {
            get { return _SelectProceso; }
            set { _SelectProceso = value; OnPropertyChanged("SelectProceso"); }
        }

        private Procesos _SelectedProceso;

        public Procesos SelectedProceso
        {
            get { return _SelectedProceso; }
            set
            {
                _SelectedProceso = value;
                if (value != null)
                {
                    if (value.Id_proceso > -1)
                    {
                        if (value.Id_proceso == 0)
                        {
                            //Se pone en enable combobox false ya que solo tendra un Tipo de certificado a seleccionar el cual se manda en la Variable SelectTipoCertificado = 0;
                            EnableTipoCertificado = false;
                            SelectTipoCertificado = 0;
                        }
                        else
                        {
                            if (value.Id_proceso != 0)
                            {
                                SelectTipoCertificado = -1;
                            }
                            EnableTipoCertificado = true;
                        }
                    }
                    else
                    {
                        SelectTipoCertificado = -1;
                    }
                }
                OnPropertyChanged("SelectedProceso");
            }
        }




        private ObservableCollection<TipoCertificado> _ListTipoCertificado;

        public ObservableCollection<TipoCertificado> ListTipoCertificado
        {
            get { return _ListTipoCertificado; }
            set { _ListTipoCertificado = value; OnPropertyChanged("ListTipoCertificado"); }
        }

        private short? _SelectTipoCertificado;

        public short? SelectTipoCertificado
        {
            get { return _SelectTipoCertificado; }
            set
            {
                //Verificar sientraen el Set del SelectedTipoCertificado
                _SelectTipoCertificado = value;

                OnPropertyChanged("SelectTipoCertificado");
            }
        }

        private TipoCertificado _SelectedTipoCertificado;

        public TipoCertificado SelectedTipoCertificado
        {
            get { return _SelectedTipoCertificado; }
            set
            {
                _SelectedTipoCertificado = value;
                if (SelectedTipoCertificado != null)
                {
                    if (SelectedTipoCertificado.Id_TipoCertificado > -1)
                    {
                        if (SelectedProceso != null && SelectedProceso.Id_proceso > -1)
                        {
                            // SeleccionProcesosCertificadomedico(SelectedProceso.DESCR, SelectedTipoCertificado.DESCR);
                        }
                        else
                        {
                            (new Dialogos()).ConfirmacionDialogo("Validación", "DEBE DE SELECCIONAR UN PROCESO.");
                        }

                    }
                    else
                    {
                        //Limpiara el Grid 
                        // (new Dialogos()).ConfirmacionDialogo("Validación", "DEBE DE SELECCIONAR EL TIPO DE CERTIFICACION.");
                    }
                }





                OnPropertyChanged("SelectedTipoCertificado");
            }
        }
        public class TipoCertificado : CertificadoMedicoCancelacion_TrasladoViewModel
        {
            private short? _Id_TipoCertificado;

            public short? Id_TipoCertificado
            {
                get { return _Id_TipoCertificado; }
                set { _Id_TipoCertificado = value; OnPropertyChanged("Id_TipoCertificado"); }
            }
            private string _DESCR;

            public string DESCR
            {
                get { return _DESCR; }
                set { _DESCR = value; OnPropertyChanged("DESCR"); }
            }

        }

        private ObservableCollection<CertificadoMedicoIngresos> _ListaCertificadoMed;

        public ObservableCollection<CertificadoMedicoIngresos> ListaCertificadoMed
        {
            get { return _ListaCertificadoMed; }
            set { _ListaCertificadoMed = value; OnPropertyChanged("ListaCertificadoMed"); }
        }

        private CertificadoMedicoIngresos _SelectCertificadoMedico;

        public CertificadoMedicoIngresos SelectCertificadoMedico
        {
            get { return _SelectCertificadoMedico; }
            set { _SelectCertificadoMedico = value; OnPropertyChanged("SelectCertificadoMedico"); }
        }

        //Se utiliza para popular datos de Ingresos Detalles o Excarcelacion 
        public class CertificadoMedicoIngresos : CertificadoMedicoCancelacion_TrasladoViewModel
        {


            private TRASLADO_DETALLE _ObjetoTrasladoDetalle;

            public TRASLADO_DETALLE ObjetoTrasladoDetalle
            {
                get { return _ObjetoTrasladoDetalle; }
                set { _ObjetoTrasladoDetalle = value; OnPropertyChanged("ObjetoTrasladoDetalle"); }
            }


            private EXCARCELACION _ObjetoExcarcelacion;

            public EXCARCELACION ObjetoExcarcelacion
            {
                get { return _ObjetoExcarcelacion; }
                set { _ObjetoExcarcelacion = value; OnPropertyChanged("ObjetoExcarcelacion"); }
            }

            private int? _ID_IMUPATOCM;

            public int? ID_IMUPATOCM
            {
                get { return _ID_IMUPATOCM; }
                set { _ID_IMUPATOCM = value; OnPropertyChanged("ID_IMUPATOCM"); }
            }

            private short? _ID_INGRESOCM;

            public short? ID_INGRESOCM
            {
                get { return _ID_INGRESOCM; }
                set { _ID_INGRESOCM = value; OnPropertyChanged("ID_INGRESOCM"); }
            }

            private int _FOLIOCM;
            public int FOLIOCM
            {
                get { return _FOLIOCM; }
                set { _FOLIOCM = value; OnPropertyChanged("FOLIOCM"); }
            }

            private short? _ANIOCM;

            public short? ANIOCM
            {
                get { return _ANIOCM; }
                set { _ANIOCM = value; OnPropertyChanged("ANIOCM"); }
            }
            private short? _CENTROCM;

            public short? CENTROCM
            {
                get { return _CENTROCM; }
                set { _CENTROCM = value; OnPropertyChanged("CENTROCM"); }
            }


            private string _NombreCM;

            public string NombreCM
            {
                get { return _NombreCM; }
                set { _NombreCM = value; OnPropertyChanged("NombreCM"); }
            }
            private string _ApPaternoCM;

            public string ApPaternoCM
            {
                get { return _ApPaternoCM; }
                set { _ApPaternoCM = value; OnPropertyChanged("ApPaternoCM"); }
            }
            private string _ApMaternoCM;

            public string ApMaternoCM
            {
                get { return _ApMaternoCM; }
                set { _ApMaternoCM = value; OnPropertyChanged("ApMaternoCM"); }
            }


            private bool _IsSelectedCertificado;

            public bool IsSelectedCertificado
            {
                get { return _IsSelectedCertificado; }
                set { _IsSelectedCertificado = value;
                OnPropertyValidateChanged("IsSelectedCertificado");
                     }
            }

        }

        public class Procesos : CertificadoMedicoCancelacion_TrasladoViewModel
        {
            private short? _Id_proceso;

            public short? Id_proceso
            {
                get { return _Id_proceso; }
                set { _Id_proceso = value; OnPropertyChanged("Id_proceso"); }
            }
            private string _DESCR;

            public string DESCR
            {
                get { return _DESCR; }
                set { _DESCR = value; OnPropertyChanged("DESCR"); }
            }

        }

    }




}
