using SSP.Servidor;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;

namespace ControlPenales
{
    public class Fechas
    {
        public static string fechaLetra(DateTime? fecha,bool agenda = true)
        {
            if (fecha != null)
            {
                string[] mes = { null, "enero", "febrero", "marzo", "abril", "mayo", "junio", "julio", "agosto", "septiembre", "octubre", "noviembre", "diciembre" };
                string[] dia = { "domingo", "lunes", "martes", "miercoles", "jueves", "viernes", "sabado" };
                if(agenda)
                    return string.Format("Agenda el dia {0} {1} de {2} de {3}", dia[(int)fecha.Value.DayOfWeek], fecha.Value.Day, mes[fecha.Value.Month], fecha.Value.Year);
                else
                    return string.Format("{0} de {1} de {2}",fecha.Value.Day, mes[fecha.Value.Month], fecha.Value.Year);
            }
            else
                return string.Empty;
        }

        public string TiempoResidencia(DateTime fecha)
        {
            if (fecha != null)
            {
                int anios = 0, meses = 0, dias = 0;
                DateTime hoy = GetFechaDateServer.Date;
                anios = (hoy.Year - fecha.Year);
                meses = (hoy.Month - fecha.Month);
                dias = (hoy.Day - fecha.Day);

                if (meses < 0)
                {
                    anios -= 1;
                    meses += 12;
                }
                if (dias < 0)
                {
                    meses -= 1;
                    dias += DateTime.DaysInMonth(hoy.Year, fecha.Month);
                }
                if (anios < 0)
                {
                    return string.Empty;
                }

                return string.Format("{0} Años {1} Meses", anios, meses);
            }
            else
                return string.Empty;
        }

        //public int CalculaEdad(DateTime? fecha)
        //{
        //    if (fecha != null)
        //    {
        //        DateTime hoy = GetFechaDateServer.Date;
        //        int edad = hoy.Year - fecha.Value.Year;
        //        if (hoy.Month < fecha.Value.Month || (hoy.Month == fecha.Value.Month && hoy.Day < fecha.Value.Day))
        //            edad--;
        //        return edad;
        //    }
        //    return 0;
        //}

        public short CalculaEdad(DateTime? fecha)
        {
            if (fecha != null)
            {
                DateTime hoy = GetFechaDateServer.Date;
                int edad = hoy.Year - fecha.Value.Year;
                if (hoy.Month < fecha.Value.Month || (hoy.Month == fecha.Value.Month && hoy.Day < fecha.Value.Day))
                    edad--;
                if(edad > 0)
                    return (short)edad;
            }
            return 0;
        }

        public int CalculaEdad(DateTime? fecha,DateTime hoy)
        {
            if (fecha != null)
            {
                int edad = hoy.Year - fecha.Value.Year;
                if (hoy.Month < fecha.Value.Month || (hoy.Month == fecha.Value.Month && hoy.Day < fecha.Value.Day))
                    edad--;
                if (edad > 0)
                    return edad;
            }
            return 0;
        }


        private int CalculateDays(DateTime oldDate, DateTime newDate)
        {
            // Diferencia de fechas
            TimeSpan ts = newDate - oldDate;

            // Diferencia de días
            return ts.Days;
        }


        public string DiferenciaFechas365(DateTime newdt, DateTime olddt, out int a, out int m, out int d)
        {
            try
            {
                int anios = 0, meses = 0;
                string str = string.Empty;
                a = m = d = 0;
                TimeSpan ts = newdt.Date - olddt.Date;
                var dias = ts.Days;

                anios = dias / 365;
                meses = (dias % 365)/30;
                dias = (dias % 365) % 30;

                if (anios < 0)
                {
                    return "Fecha Invalida";
                }
                if (anios > 0)
                {
                    a = anios;
                    str = str + anios.ToString() + " años ";
                }
                if (meses > 0)
                {
                    m = meses;
                    str = str + meses.ToString() + " meses ";
                }
                if (dias > 0)
                {
                    d = dias;
                    str = str + dias.ToString() + " dias ";
                }
                return str;

            }
            catch (Exception ex)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al obtener la fecha del server", ex);
                a = m = d = 0;
                return string.Empty;
            }
        }

        public string DiferenciaFechas30(DateTime newdt, DateTime olddt, out int a, out int m, out int d)
        {
            try
            {
                int anios = 0 , meses = 0;
                string str = string.Empty;
                a = m = d = 0;
                TimeSpan ts = newdt.Date - olddt.Date;
                var dias = ts.Days;
                while(dias >= 30)
                {
                    meses++;
                    dias = dias - 30;
                }

                while(meses >= 12)
                {
                    anios++;
                    meses = meses - 12;
                }
                
                if (anios < 0)
                {
                    return "Fecha Invalida";
                }
                if (anios > 0)
                {
                    a = anios;
                    str = str + anios.ToString() + " años ";
                }
                if (meses > 0)
                {
                    m = meses;
                    str = str + meses.ToString() + " meses ";
                }
                if (dias > 0)
                {
                    d = dias;
                    str = str + dias.ToString() + " dias ";
                }
                return str;

            }
            catch (Exception ex)
            {
                StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al obtener la fecha del server", ex);
                a = m = d = 0;
                return string.Empty;
            }
        }

        private int DiasMes(int Anio, int Mes)
        {
            DateTime MyDate = new DateTime(Anio,Mes,1,0,0,0);
            MyDate = MyDate.AddDays(-3);//-2
            return MyDate.Day;
        }

        public String DiferenciaFechas(DateTime newdt, DateTime olddt, out int a, out int m, out int d)
        {
            Int32 anios;
            Int32 meses;
            Int32 dias;
            String str = string.Empty;
            a = m = d = 0;

            int y1 = newdt.Year, m1 = newdt.Month, d1 = newdt.Day;
            int y2 = olddt.Year, m2 = olddt.Month, d2 = olddt.Day;
            if (d1 < d2)
            {
                m1--;
                d1 += DiasMes(y2, m2);
            }
            if (m1 < m2)
            {
                y1--;
                m1 += 12;
            }
            anios = y1 - y2; 
            meses = m1 - m2;
            dias = d1 - d2;

            if (anios < 0)
            {
                return "Fecha Invalida";
            }
            if (anios > 0)
            {
                a = anios;
                str = str + anios.ToString() + " años ";
            }
            if (meses > 0)
            {
                m = meses;
                str = str + meses.ToString() + " meses ";
            }
            if (dias > 0)
            {
                d = dias;
                str = str + dias.ToString() + " dias ";
            }
            return str;
        }

        /// <summary>
        /// Metodo que retorna la fecha del servidor
        /// </summary>
        /// <returns></returns>
        public static DateTime GetFechaDateServer
        {
            get
            {
                using (SSPEntidades obj = new SSPEntidades())
                {
                    try
                    {
                        obj.Conexion();
                        //obj.Database.Connection.ConnectionString = string.Format("DATA SOURCE=SSPE;PASSWORD={0};USER ID={1}", GlobalVar.gPass, GlobalVar.gUsr);
                        return obj.Database.SqlQuery<DateTime>("SELECT SYSDATE FROM DUAL").SingleOrDefault();
                    }
                    catch (Exception ex)
                    {
                        StaticSourcesViewModel.ShowMessageError("Algo pasó...", "Ocurrió un error al obtener la fecha del server", ex);
                    }
                    return DateTime.Now;
                }
            }
        }

        public static string GetFechaDateServerString
        {
            get
            {
                using (var obj = new SSPEntidades())
                {
                    obj.Conexion();
                    return obj.Database.SqlQuery<string>("SELECT TO_CHAR(SYSDATE, 'DD/MM/YYYY HH:MI:SS AM') FROM DUAL").SingleOrDefault();
                }
            }
        }

        public DateTime AgregarDiasHabiles(DateTime dt, int nDays)
        {
            int weeks = nDays / 5;
            nDays %= 5;
            while (dt.DayOfWeek == DayOfWeek.Saturday || dt.DayOfWeek == DayOfWeek.Sunday)
                dt = dt.AddDays(1);

            while (nDays-- > 0)
            {
                dt = dt.AddDays(1);
                if (dt.DayOfWeek == DayOfWeek.Saturday)
                    dt = dt.AddDays(2);
            }
            var x = dt.AddDays(weeks * 7);
            return x;
        }

        public static CalendarBlackoutDatesCollection DeshabilitarDiasEnCalendario(List<DayOfWeek> dias,int beforeToday = 0, int afterToday = 0)
        {
            //if(Parametros.DIAS_VISITA_FAMILIAR == "FINES_DE_SEMANA")
            var i = 0;
            var hoy = Fechas.GetFechaDateServer;
            var seisMeses = new List<DateTime>();
            for (i = 0; i < beforeToday; i++)
            {
                seisMeses.Add(hoy.AddDays(-i));
            }
            for (i = 0; i < afterToday; i++)
            {
                seisMeses.Add(hoy.AddDays(i));
            }
            var dates = new CalendarBlackoutDatesCollection(new System.Windows.Controls.Calendar());
            foreach (var item in seisMeses)
            {
                if (!dias.Contains(item.DayOfWeek))
                {
                    dates.Add(new CalendarDateRange(item));
                }
            }
            return dates;
        }

    }

    class AttachedProperties : DependencyObject
    {

        #region RegisterBlackoutDates

        // Adds a collection of command bindings to a date picker's existing BlackoutDates collection, since the collections are immutable and can't be bound to otherwise.
        //
        // Usage: <DatePicker hacks:AttachedProperties.RegisterBlackoutDates="{Binding BlackoutDates}" >

        public static DependencyProperty RegisterBlackoutDatesProperty = DependencyProperty.RegisterAttached("RegisterBlackoutDates", typeof(System.Windows.Controls.CalendarBlackoutDatesCollection), typeof(AttachedProperties), new PropertyMetadata(null, OnRegisterCommandBindingChanged));

        public static void SetRegisterBlackoutDates(UIElement element, System.Windows.Controls.CalendarBlackoutDatesCollection value)
        {
            if (element != null)
                element.SetValue(RegisterBlackoutDatesProperty, value);
        }
        public static System.Windows.Controls.CalendarBlackoutDatesCollection GetRegisterBlackoutDates(UIElement element)
        {
            return (element != null ? (System.Windows.Controls.CalendarBlackoutDatesCollection)element.GetValue(RegisterBlackoutDatesProperty) : null);
        }
        private static void OnRegisterCommandBindingChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            System.Windows.Controls.DatePicker element = sender as System.Windows.Controls.DatePicker;
            if (element != null)
            {
                System.Windows.Controls.CalendarBlackoutDatesCollection bindings = e.NewValue as System.Windows.Controls.CalendarBlackoutDatesCollection;
                if (bindings != null)
                {
                    element.BlackoutDates.Clear();
                    foreach (var dateRange in bindings)
                    {
                        element.BlackoutDates.Add(dateRange);
                    }
                }
            }
        }

        #endregion
    }
}
