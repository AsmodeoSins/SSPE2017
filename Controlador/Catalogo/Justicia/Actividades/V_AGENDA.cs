using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SSP.Modelo;
using SSP.Servidor;
using LinqKit;

namespace SSP.Controlador.Catalogo.Justicia.Actividades
{
    public class V_AGENDA
    {
        private int Conteo;
        private int Limite = 50;

        public int REGISTRO { get; set; }
        public string TIPO { get; set; }
        public int ID { get; set; }
        public int ID_CENTRO { get; set; }
        public DateTime INICIO { get; set; }
        public DateTime FINAL { get; set; }
        public int HORA { get; set; }
        public int MIN { get; set; }
        public string DESCRIPCION { get; set; }
        public int ID_AREA { get; set; }
        public string AREA { get; set; }
        public int PRIORIDAD { get; set; }
        public int? IMP_CENTRO { get; set; }
        public int? IMP_ANIO { get; set; }
        public int? IMP_IMPUTADO { get; set; }
        public int? IMP_INGRESO { get; set; }

        public string NOMBRE_RESPONSABLE { get; set; }

        public List<V_AGENDA> Obtener(List<V_AGENDA> agenda)
        {
            var lstGrupos = new List<GRUPO>();
            var grupo = new cGrupo();

            var predicado = PredicateBuilder.False<GRUPO>();

            foreach (var item in agenda)
            {
                predicado = predicado.Or(o => o.ID_GRUPO == item.ID);

                if (Conteo == Limite)
                {
                    var resultado = grupo.GetData(predicado.Expand()).ToList();

                    if (resultado.Count > 0)
                    {
                        lstGrupos.AddRange(resultado);
                        predicado = PredicateBuilder.False<GRUPO>();
                        Conteo = 0;
                    }
                }

                Conteo++;
            }

            if (Conteo > 0)
            {
                var resultado = grupo.GetData(predicado.Expand()).ToList();
                if (resultado.Count > 0)
                {
                    lstGrupos.AddRange(resultado);
                }
            }

            foreach (var item in lstGrupos)
            {
                var consulta = agenda.Where(w => w.ID == item.ID_GRUPO).Select(s => s);

                if (consulta != null)
                {
                    foreach (var age in consulta)
                    {
                        age.NOMBRE_RESPONSABLE = string.Format("{2} {1} {0}", item.PERSONA.NOMBRE.TrimEnd(), item.PERSONA.PATERNO.TrimEnd(), item.PERSONA.MATERNO.TrimEnd());
                    }

                    //TODO POR EL MOMENTO SOLAMENTE PONDREMOS A LOS EVENTOS SIN RESPONSABLE
                    agenda.Where(w => w.TIPO == "EVE").ForEach((evento) =>
                    {
                        evento.NOMBRE_RESPONSABLE = "SIN RESPONSABLE";
                    });
                }
            }

            return agenda;
        }
    }
}
