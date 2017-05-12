using System;
using System.Collections.ObjectModel;
using System.Windows;
using System.Threading.Tasks;

namespace ControlPenales
{
    partial class ConsultaExpedienteInternoViewModel
    {

        #region Agenda
        private AgendaCitaInternoView _agendaInternoView;
        public AgendaCitaInternoView AgendaInternoView
        {
            get { return _agendaInternoView; }
            set { _agendaInternoView = value; }
        }
        private ObservableCollection<Appointment> _lstAgendaInterno;
        public ObservableCollection<Appointment> LstAgendaInterno
        {
            get { return _lstAgendaInterno; }
            set
            {
                _lstAgendaInterno = value;
                OnPropertyChanged("LstAgendaInterno");
            }
        }

        private bool _agregarHoraAgenda = false;
        public bool AgregarHoraAgenda
        {
            get { return _agregarHoraAgenda; }
            set { _agregarHoraAgenda = value; OnPropertyChanged("AgregarHoraAgenda"); }
        }

        private byte[] _ssImagen = new Imagenes().getImagenPerson();
        public byte[] SSImagen
        {
            get { return _ssImagen; }
            set { _ssImagen = value; OnPropertyChanged("SSImagen"); }
        }


        private DateTime? _aaFecha;
        public DateTime? AAFecha
        {
            get { return _aaFecha; }
            set
            {
                _aaFecha = value;
                if (value.HasValue)
                {
                    var hoy = Fechas.GetFechaDateServer;
                    var now = new DateTime(hoy.Year, hoy.Month, hoy.Day);
                    if (value >= now)
                        AAFechaValid = true;
                    else
                    {
                        AAFechaValid = false;
                        AAFechaMensaje = "La fecha a agendar debe ser igual o mayor al dia de hoy.";
                    }
                }
                else
                {
                    AAFechaMensaje = "La fecha es requerida..";
                    AAFechaValid = false;
                }
                OnPropertyChanged("AAFecha");
            }
        }

        private bool _aaFechaValid = false;
        public bool AAFechaValid
        {
            get { return _aaFechaValid; }
            set { _aaFechaValid = value; OnPropertyChanged("AAFechaValid"); }
        }

        private DateTime? _aaHoraI;
        public DateTime? AAHoraI
        {
            get { return _aaHoraI; }
            set
            {
                _aaHoraI = value;
                OnPropertyChanged("AAHoraI");
            }
        }

        private DateTime? _aaHoraF;
        public DateTime? AAHoraF
        {
            get { return _aaHoraF; }
            set
            {
                _aaHoraF = value;
                if (!value.HasValue)
                    AAHorasValid = false;
                else
                    if (!AAHoraI.HasValue)
                        AAHorasValid = false;
                    else
                        if (value <= AAHoraI)
                            AAHorasValid = false;
                        else
                            AAHorasValid = true;

                OnPropertyChanged("AAHoraF");
            }
        }

        private bool _aaHorasValid = false;
        public bool AAHorasValid
        {
            get { return _aaHorasValid; }
            set { _aaHorasValid = value; OnPropertyChanged("AAHorasValid"); }

        }

        private string _aaFechaMensaje = "La fecha es requerida.";
        public string AAFechaMensaje
        {
            get { return _aaFechaMensaje; }
            set { _aaFechaMensaje = value; OnPropertyChanged("AAFechaMensaje"); }
        }

        private string _mmensajeError;
        public string MMensajeError
        {
            get { return _mmensajeError; }
            set
            {
                _mmensajeError = value;
                OnPropertyChanged("MMensajeError");
                Application.Current.Dispatcher.Invoke((System.Action)(delegate
                {
                    Task.Factory.StartNew(async () => { await TaskEx.Delay(4000); MMensajeError = string.Empty; });
                }));
            }
        }

        #endregion
    }
}
