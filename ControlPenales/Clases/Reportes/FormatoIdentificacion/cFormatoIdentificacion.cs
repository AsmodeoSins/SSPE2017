using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ControlPenales
{
    public class cFormatoIdentificacion
    {
        public bool Seleccionado { get; set; }

        public int Id_Anio { get; set; }
        public int Id_Imputado { get; set; }
        public int Id_ingreso { get; set; }
        public int Edad { get; set; }
        public int Estatura { get; set; }
        public int Peso { get; set; }

        public DateTime Fec_Ingreso { get; set; }
        public DateTime Fec_Nacimiento { get; set; }

        public byte[] Perfil_Derecho { get; set; }
        public byte[] Fotografia_Frente { get; set; }
        public byte[] Perfil_Izquierdo { get; set; }

        public string Folio_Gob { get; set; }
        public string Paterno { get; set; }
        public string Materno { get; set; }
        public string Nombre { get; set; }
        public string Alias { get; set; }
        public string Apodos { get; set; }
        public string Estatus { get; set; }
        public string Tipo_Ingreso { get; set; }
        public string Oficio_Internacion { get; set; }
        public string Autoridad { get; set; }
        public string A_Disposicion { get; set; }
        public string Padre_Nombre_Completo { get; set; }
        public string Madre_Nombre_Completo { get; set; }
        public string Origen { get; set; }
        public string Domicilio { get; set; }
        public string Numero_Domicilio { get; set; }
        public string Colonia_Domicilio { get; set; }
        public string Ciudad { get; set; }
        public string Sexo { get; set; }
        public string Estado_Civil { get; set; }
        public string Religion { get; set; }
        public string Ocupacion { get; set; }
        public string Grado_Maximo_Estudios { get; set; }
        public string Lugar_Nacimiento_Extranjeros { get; set; }
        public string Complexion { get; set; }
        public string Color_Piel { get; set; }
        public string Color_Cabello { get; set; }
        public string Forma_Cabello { get; set; }
        public string Frente_Alta { get; set; }
        public string Frente_Inclinada { get; set; }
        public string Frente_Ancha { get; set; }
        public string Color_Ojos { get; set; }
        public string Forma_Ojos { get; set; }
        public string Tamano_Ojos { get; set; }
        public string Raiz_Nariz { get; set; }
        public string Ancho_Nariz { get; set; }
        public string Tamano_Boca { get; set; }
        public string Comisuras_Boca { get; set; }
        public string Espesor_Labios { get; set; }
        public string Altura_Labios { get; set; }
        public string Promedio_Labios { get; set; }
        public string Tipo_Menton { get; set; }
        public string Forma_Menton { get; set; }
        public string Inclinacion_Menton { get; set; }



    }
}
