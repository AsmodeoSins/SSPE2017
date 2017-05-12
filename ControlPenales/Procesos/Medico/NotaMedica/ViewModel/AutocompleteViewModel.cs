using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using System.Collections;
using System.Web;
using System.Xml;
using System.Net;
using System.Diagnostics;
using SSP.Controlador.Catalogo.Justicia;
using SSP.Servidor;
using SSP.Controlador.Catalogo.Almacenes;

namespace ControlPenales
{
    public class AutocompleteViewModel : INotifyPropertyChanged
    {
        private List<string> _WaitMessage = new List<string>() { "Por favor espere..." };
        public IEnumerable WaitMessage { get { return _WaitMessage; } }

        private string _QueryText;
        public string QueryText
        {
            get { return _QueryText; }
            set
            {
                if (_QueryText != value)
                {
                    _QueryText = value;
                    OnPropertyChanged("QueryText");
                    _QueryCollection = null;
                    OnPropertyChanged("QueryCollection");
                    Debug.Print("QueryText: " + value);
                }
            }
        }
        private string _TextReceta;
        public string TextReceta
        {
            get { return _TextReceta; }
            set
            {
                if (_TextReceta != value)
                {
                    _TextReceta = value;
                    OnPropertyChanged("TextReceta");
                    _QueryReceta = null;
                    OnPropertyChanged("QueryReceta");
                    Debug.Print("TextReceta: " + value);
                }
            }
        }
        private string _TextProcedimiento;
        public string TextProcedimiento
        {
            get { return _TextProcedimiento; }
            set
            {
                if (_TextProcedimiento != value)
                {
                    _TextProcedimiento = value;
                    OnPropertyChanged("TextProcedimiento");
                    _QueryProcedimiento = null;
                    OnPropertyChanged("QueryProcedimiento");
                    Debug.Print("TextReceta: " + value);
                }
            }
        }

        public IEnumerable _QueryCollection = null;
        public IEnumerable QueryCollection
        {
            get
            {
                Debug.Print("---" + _QueryCollection);
                QueryEnfermedades(QueryText);
                return _QueryCollection;
            }
        }

        public IEnumerable _QueryReceta = null;
        public IEnumerable QueryReceta
        {
            get
            {
                Debug.Print("---" + _QueryReceta);
                QueryRecetas(TextReceta);
                return _QueryReceta;
            }
        }

        public IEnumerable _QueryProcedimiento = null;
        public IEnumerable QueryProcedimiento
        {
            get
            {
                Debug.Print("---" + _QueryProcedimiento);
                QueryRecetas(TextProcedimiento);
                return _QueryReceta;
            }
        }
        private void QueryEnfermedades(string SearchTerm)
        {
            try
            {
                if (!string.IsNullOrEmpty(SearchTerm) ? SearchTerm.Length > 3 : false)
                {
                    var x = new cEnfermedades().ObtenerEnfermedadXBusqueda(SearchTerm);
                    _QueryCollection = x.Any() ? x.OrderBy(o => o.NOMBRE.Length).ToList() : new List<ENFERMEDAD>();
                }
                else
                    _QueryCollection = new List<ENFERMEDAD>();
            }
            catch (Exception EX)
            {
                throw EX;
            }
        }

        private void QueryRecetas(string SearchTerm)
        {
            try
            {
                if (!string.IsNullOrEmpty(SearchTerm) ? SearchTerm.Length > 2 : false)
                {
                    var x = new cProducto().GetData(w => w.ID_CATEGORIA.HasValue ?
                        (w.PRODUCTO_CATEGORIA.ID_PROD_GRUPO == "M" && w.PRODUCTO_CATEGORIA.ACTIVO == "S") ?
                            (w.DESCRIPCION.Contains(SearchTerm) || w.NOMBRE.Contains(SearchTerm) || w.PRODUCTO_CATEGORIA.NOMBRE.Contains(SearchTerm) || w.PRODUCTO_CATEGORIA.DESCR.Contains(SearchTerm))
                        : false
                    : false);
                    _QueryReceta = x.Any() ? x.OrderBy(o => o.DESCRIPCION).Select(s =>
                        new RecetaMedica
                        {
                            CANTIDAD = new Nullable<short>(),
                            HORA_MANANA = false,
                            HORA_TARDE = false,
                            HORA_NOCHE = false,
                            PRODUCTO = s,
                            MEDIDA = -1
                        }).ToList() : new List<RecetaMedica>();
                }
                else
                    _QueryReceta = new List<RecetaMedica>();
            }
            catch (Exception EX)
            {
                throw EX;
            }
        }

        #region INotifyPropertyChanged Members

        public event PropertyChangedEventHandler PropertyChanged;

        #endregion

        protected void OnPropertyChanged(string prop)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(prop));
        }
    }
}