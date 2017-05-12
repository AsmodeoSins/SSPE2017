using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MVVMShared.Converters
{
    public class Fechas
    {
        public static string fechaLetra(DateTime fecha)
        {
            if (fecha != null)
            {
                string[] mes = { null, "enero", "febrero", "marzo", "abril", "mayo", "junio", "julio", "agosto", "septiembre", "octubre", "noviembre", "diciembre" };
                string[] dia = { "domingo", "lunes", "martes", "miercoles", "jueves", "viernes", "sabado" };
                return string.Format("Agenda el dia {0} {1} de {2} de {3}", dia[(int)fecha.DayOfWeek], fecha.Day, mes[fecha.Month], fecha.Year);
            }
            else
                return string.Empty;
        }


        public String DiferenciaFechas(DateTime newdt, DateTime olddt, out int a, out int m, out int d)
        {
            Int32 anios;
            Int32 meses;
            Int32 dias;
            String str = string.Empty;
            a = m = d = 0;
            anios = (newdt.Year - olddt.Year);
            meses = (newdt.Month - olddt.Month);
            dias = (newdt.Day - olddt.Day);

            if (meses < 0)
            {
                anios -= 1;
                meses += 12;
            }
            if (dias < 0)
            {
                meses -= 1;
                dias += DateTime.DaysInMonth(newdt.Year, newdt.Month);
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
    }
}
