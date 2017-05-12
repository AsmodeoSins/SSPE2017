using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ControlPenales
{
    public class ModeloPrueba
    {
        public string apellido_paterno { set; get; }
        public string apellido_materno { set; get; }
        public string nombre { set; get; }
        public string foto { set; get; }
        public string estatus { set; get; }
        public string tipo_registro { set; get; }
        public string causa_penal { set; get; }
        public string unidad { set; get; }
        public DateTime fecha_registro { set; get; }
        public int edad { set; get; }
        //Nacimiento
        public DateTime fecha_nacimiento { set; get; }
        public string pais_nacimiento { set; get; }
        public string estado_nacimiento { set; get; }
        public string municipio_nacimiento { set; get; }
        //Domicilio
        public string pais_domicilio { set; get; }
        public string estado_domicilio { set; get; }
        public string municipio_domicilio { set; get; }
        public string colonia_domicilio { set; get; }
        public string calle_domicilio { set; get; }
        public string numero_exterior_domicilio { set; get; }
        public string codigo_postal_domicilio { set; get; }
        //Contacto
        public string telefono_casa { set; get; }
        public string telefono_movil { set; get; }
        public string radio { set; get; }
        public string correo_electronico { set; get; }
        //Observaciones
        public string observaciones { set; get; }
        //Media Filiacion
        public string mf_estatura { set; get; }
        public string mf_peso { set; get; }
        public string mf_complexion { set; get; }
        public string mf_tez { set; get; }
        public string mf_ojos { set; get; }
        public string mf_pelo { set; get; }
        public string mf_color_pelo { set; get; }
        //Relacion
        public List<referencias> referencias { set; get; }
        //Senias Particulares
        public List<seniasParticulares> senias_particulares { set; get; }
        //Alias
        public List<alias> alias { set; get; }
        //Apodos
        public List<apodos> apodos { set; get; }
        //Internaciones
        public List<internaciones> internaciones { set; get; }
        //Documentos
        public List<documentos> documentos { set; get; }
        
    }

    public class referencias {
        public string apellido_paterno { set; get; }
        public string apellido_materno { set; get; }
        public string nombre { set; get; }
        public string telefono_casa { set; get; }
        public string telefono_movil { set; get; }
        public string relacion { set; get; }
        public string domicilio { set; get; }
    }

    public class seniasParticulares {
        public string sp_tipo { set; get; }
        public string sp_lado { set; get; }
        public string sp_region { set; get; }
        public string sp_cantidad { set; get; }
        public string sp_vista { set; get; }
        public string sp_descripcion { set; get; }
    }

    public class alias {
        public string apellido_paterno { set; get; }
        public string apellido_materno { set; get; }
        public string nombre { set; get; }
    }

    public class apodos {
        public string apodo { set; get; }
    }

    public class internaciones {
        public string fecha { set; get; }
        public string salida { set; get; }
        public string internacion { set; get; }
        public string cumplimiento { set; get; }
        public string lugar { set; get; }
    }

    public class documentos {
        public string dependencia { set; get; }
        public string no_oficio { set; get; }
        public string fuero { set; get; }
        public string documento { set; get; }
        public string asunto { set; get; }
        public string fecha { set; get; }
        public string asesor { set; get; }
        public string sentenciado { set; get; }
    }

    public class expediente { 
                public string nuc { set; get; }
                public string fecha_inicio { set; get; }
                public string aud { set; get; }
                public string res { set; get; }
                public string prevpgje { set; get; }
                public string notif_pgje { set; get; }
                public string valid_dom { set; get; }
                public string visitas { set; get; }
                public string docs_pje { set; get; }
                public string scan_medidas { set; get; }
                public string otros_in { set; get; }
                public string otros_out { set; get; }
                public string medidas { set; get; }
                public string seg { set; get; }
                public string orie { set; get; }
                public string asesor { set; get; }
                public string lugar { set; get; }
                public ModeloPrueba imputado { set; get; }
    }
}
