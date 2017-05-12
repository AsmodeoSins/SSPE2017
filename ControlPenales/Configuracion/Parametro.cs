using SSP.Modelo;
using SSP.Servidor;
using System.IO;
using System.Xml;
using System.Xml.Serialization;
using System.Linq;
using System;

namespace ControlPenales
{
    public class Parametro
    {
        /// <summary>
        /// Indica si sera requerido guardar la huella en la busqueda dentro del modulo de "REGISTRO", valor default: [true]
        /// </summary>
        public static bool GuardarHuellaEnBusquedaRegistro
        {
            get
            {
                var query = new EntityManagerServer<PARAMETRO>().GetData().Where(w => (w.ID_CENTRO == 0 || w.ID_CENTRO == 4) && w.ID_CLAVE.Contains("GUARDAR_HUELLA_BUSQ_REGISTRO"));
                return query.Any() ? query.Count() > 1 ? Convert.ToBoolean(query.Where(w => w.ID_CENTRO == 4).FirstOrDefault().VALOR) : Convert.ToBoolean(query.FirstOrDefault().VALOR) : true;
            }
        }

        /// <summary>
        /// Indica si sera requerido guardar la huella en la busqueda dentro del modulo de "JURIDICO", valor default: [false]
        /// </summary>
        public static bool GuardarHuellaEnBusquedaJuridico
        {
            get
            {
                var query = new EntityManagerServer<PARAMETRO>().GetData().Where(w => (w.ID_CENTRO == 0 || w.ID_CENTRO == 4) && w.ID_CLAVE.Contains("GUARDAR_HUELLA_BUSQ_JURIDICO"));
                return query.Any() ? query.Count() > 1 ? Convert.ToBoolean(query.Where(w => w.ID_CENTRO == 4).FirstOrDefault().VALOR) : Convert.ToBoolean(query.FirstOrDefault().VALOR) : false;
            }
        }

        /// <summary>
        /// Indica el lapso minimo para otro estudio de personalidad, valor default: [6 meses]
        /// </summary>
        public static string LAPSO_ESTUDIOS
        {
            get
            {
                var query = new EntityManagerServer<PARAMETRO>().GetData().Where(w => (w.ID_CENTRO == 0 || w.ID_CENTRO == 4) && w.ID_CLAVE.Contains("LAPSO_ENTRE_ESTUDIOS"));
                return query.Any() ? query.Where(w => w.ID_CENTRO == 0).FirstOrDefault().VALOR : null;
            }
        }

        /// <summary>
        /// Indica si sera requerido guardar la huella en la busqueda dentro del modulo de "ESTATUS ADMINISTRATIVO", valor default: [false]
        /// </summary>
        public static bool GuardarHuellaEnBusquedaEstatusAdministrativo
        {
            get
            {
                var query = new EntityManagerServer<PARAMETRO>().GetData().Where(w => (w.ID_CENTRO == 0 || w.ID_CENTRO == 4) && w.ID_CLAVE.Contains("GUARDAR_HUELLA_BUSQ_EST_ADMIN"));
                return query.Any() ? query.Count() > 1 ? Convert.ToBoolean(query.Where(w => w.ID_CENTRO == 4).FirstOrDefault().VALOR) : Convert.ToBoolean(query.FirstOrDefault().VALOR) : false;
            }
        }

        /// <summary>
        /// Indica si sera requerido guardar la huella en la busqueda dentro del modulo de "PADRON VISITA", valor default: [false]
        /// </summary>
        public static bool GuardarHuellaEnBusquedaPadronVisita
        {
            get
            {
                var query = new EntityManagerServer<PARAMETRO>().GetData().Where(w => (w.ID_CENTRO == 0 || w.ID_CENTRO == 4) && w.ID_CLAVE.Contains("GUARDAR_HUELLA_BUSQ_VISITA"));
                return query.Any() ? query.Count() > 1 ? Convert.ToBoolean(query.Where(w => w.ID_CENTRO == 4).FirstOrDefault().VALOR) : Convert.ToBoolean(query.FirstOrDefault().VALOR) : false;
            }
        }

        /// <summary>
        /// Indica la edad de un menor de sexo femenino, valor default: [10]
        /// </summary>
        public static short EDAD_MENOR_F
        {
            get
            {
                var query = new EntityManagerServer<PARAMETRO>().GetData().Where(w => (w.ID_CENTRO == 0 || w.ID_CENTRO == 4) && w.ID_CLAVE.Contains("EDAD_MENOR_F"));
                return query.Any() ? query.Count() > 1 ? Convert.ToInt16(query.Where(w => w.ID_CENTRO == 4).FirstOrDefault().VALOR) : Convert.ToInt16(query.FirstOrDefault().VALOR) : (short)10;
            }
        }

        /// <summary>
        /// Indica el maximo de bytes a permitir de tamaño dentro de la subida de imagenes, valor default: [260000]
        /// </summary>
        public static long MAXIMO_BYTES_IMAGENES
        {
            get
            {
                var query = new EntityManagerServer<PARAMETRO>().GetData().Where(w => (w.ID_CENTRO == 0 || w.ID_CENTRO == 4) && w.ID_CLAVE.Contains("MAXIMO_BYTE_IMAGEN"));
                return query.Any() ? query.Count() > 1 ? Convert.ToInt64(query.Where(w => w.ID_CENTRO == 4).FirstOrDefault().VALOR_NUM) : Convert.ToInt64(query.FirstOrDefault().VALOR_NUM) : 260000;
            }
        }



        /// <summary>
        /// Indica el maximo de bytes a permitir de tamaño dentro de la subida de archivos dentro del proceso medico, valor default: [ILIMITADO]
        /// </summary>
        public static long MAXIMO_KILOBYTES_ARCHIVOS_MEDICOS
        {
            get
            {
                var query = new EntityManagerServer<PARAMETRO>().GetData().Where(w => (w.ID_CENTRO == 0 || w.ID_CENTRO == 4) && w.ID_CLAVE.Contains("TAM_MAXIMO_ARCH_MEDICO"));
                return query.Any() ? query.Count() > 1 ? Convert.ToInt32(query.Where(w => w.ID_CENTRO == 4).FirstOrDefault().VALOR_NUM) : Convert.ToInt32(query.FirstOrDefault().VALOR_NUM) : 0;
            }
        }


        /// <summary>
        /// Indica la edad de un menor de sexo masculino, valor default: [10]
        /// </summary>
        public static short EDAD_MENOR_M
        {
            get
            {
                var query = new EntityManagerServer<PARAMETRO>().GetData().Where(w => (w.ID_CENTRO == 0 || w.ID_CENTRO == 4) && w.ID_CLAVE.Contains("EDAD_MENOR_M"));
                return query.Any() ? query.Count() > 1 ? Convert.ToInt16(query.Where(w => w.ID_CENTRO == 4).FirstOrDefault().VALOR) : Convert.ToInt16(query.FirstOrDefault().VALOR) : (short)10;
            }
        }

        /// <summary>
        /// Indica la mayoria de edad de cualquier persona, valor default: [18]
        /// </summary>
        public static short MAYORIA_EDAD
        {
            get
            {
                var query = new EntityManagerServer<PARAMETRO>().GetData().Where(w => (w.ID_CENTRO == 0 || w.ID_CENTRO == 4) && w.ID_CLAVE.Contains("MAYORIA_EDAD"));
                return query.Any() ? query.Count() > 1 ? Convert.ToInt16(query.Where(w => w.ID_CENTRO == 4).FirstOrDefault().VALOR) : Convert.ToInt16(query.FirstOrDefault().VALOR) : (short)18;
            }
        }

        /// <summary>
        /// Indica que documentos seran necesarios para digitalizar y otorgar el pase unico.
        /// Forma de implementar:
        ///     valor base de datos: {x-y},(n...), Donde x = id_tipo_visita,
        ///                                              - = composicion de llave,
        ///                                              y = id_tipo_documento,
        ///                                              , = separacion de valores,
        ///                                              
        ///     resultado: listado con la llave compuesta de la tabla "TIPO_DOCUMENTO", hacer un split('-') para obtener los valores por separado, donde: * = posicion del valor a obtener,
        ///                                                                                                                                               resultado[*].split('-')[0] = id_tipo_visita,
        ///                                                                                                                                               resultado[*].split('-')[1] = id_tipo_documento,
        /// Valor default: [""]
        /// </summary>
        public static string[] DOCUMENTO_ACCESO_UNICO
        {
            get
            {
                var query = new EntityManagerServer<PARAMETRO>().GetData().Where(w => (w.ID_CENTRO == 0 || w.ID_CENTRO == 4) && w.ID_CLAVE.Contains("DOCUMENTO_ACCESO_UNICO"));
                return query.Any() ? query.Count() > 1 ? query.Where(w => w.ID_CENTRO == 4).FirstOrDefault().VALOR.Split(',') : query.FirstOrDefault().VALOR.Split(',') : new string[] { string.Empty };
            }
        }

        /// <summary>
        /// Indica que documentos seran necesarios para digitalizar y otorgar el pase unico.
        /// Forma de implementar:
        ///     valor base de datos: {x-y},(n...), Donde x = id_tipo_visita,
        ///                                              - = composicion de llave,
        ///                                              y = id_tipo_documento,
        ///                                              , = separacion de valores,
        ///                                              
        ///     resultado: listado con la llave compuesta de la tabla "TIPO_DOCUMENTO", hacer un split('-') para obtener los valores por separado, donde: * = posicion del valor a obtener,
        ///                                                                                                                                               resultado[*].split('-')[0] = id_tipo_visita,
        ///                                                                                                                                               resultado[*].split('-')[1] = id_tipo_documento,
        /// Valor default: [""]
        /// </summary>
        public static string[] DOCUMENTOS_FORANEOS
        {
            get
            {
                var query = new EntityManagerServer<PARAMETRO>().GetData().Where(w => (w.ID_CENTRO == 0 || w.ID_CENTRO == 4) && w.ID_CLAVE.Contains("DOCUMENTO_FORANEOS"));
                return query.Any() ? query.Count() > 1 ? query.Where(w => w.ID_CENTRO == 4).FirstOrDefault().VALOR.Split(',') : query.FirstOrDefault().VALOR.Split(',') : new string[] { string.Empty };
            }
        }

        /// <summary>
        /// Indica que dias seran marcados como festivos.
        /// Forma de implementar:
        ///     valor base de datos: {x-y},(n...), Donde x = mes,
        ///                                              - = composicion del mes-dia,
        ///                                              y = dia,
        ///                                              , = separacion de valores,
        ///                                              
        ///     resultado: listado con los dias en formato "MES-DIA", hacer un split('-') para obtener los valores por separado, donde: * = posicion del valor a obtener,
        ///                                                                                                                                               resultado[*].split('-')[0] = mes,
        ///                                                                                                                                               resultado[*].split('-')[1] = dia,
        /// Valor default: [""]
        /// </summary>
        public static string[] DIAS_FESTIVOS
        {
            get
            {
                var query = new EntityManagerServer<PARAMETRO>().GetData().Where(w => (w.ID_CENTRO == 0 || w.ID_CENTRO == 4) && w.ID_CLAVE.Contains("DIAS_FESTIVOS"));
                return query.Any() ? query.Count() > 1 ? query.Where(w => w.ID_CENTRO == 4).FirstOrDefault().VALOR.Split(',') : query.FirstOrDefault().VALOR.Split(',') : new string[] { string.Empty };
            }
        }

        /// <summary>
        /// Indica que documentos no seran necesarios para digitalizar.
        /// Forma de implementar:
        ///     valor base de datos: {x-y},(n...), Donde x = id_tipo_visita,
        ///                                              - = composicion de llave,
        ///                                              y = id_tipo_documento,
        ///                                              , = separacion de valores,
        ///                                              
        ///     resultado: listado con la llave compuesta de la tabla "TIPO_DOCUMENTO", hacer un split('-') para obtener los valores por separado, donde: * = posicion del valor a obtener,
        ///                                                                                                                                               resultado[*].split('-')[0] = id_tipo_visita,
        ///                                                                                                                                               resultado[*].split('-')[1] = id_tipo_documento,
        /// Valor default: [""]
        /// </summary>
        public static string[] DOCUMENTOS_NO_NECESARIOS
        {
            get
            {
                var query = new EntityManagerServer<PARAMETRO>().GetData().Where(w => (w.ID_CENTRO == 0 || w.ID_CENTRO == 4) && w.ID_CLAVE.Contains("DOCUMENTOS_NO_NECESARIOS"));
                return query.Any() ? query.Count() > 1 ? query.Where(w => w.ID_CENTRO == 4).FirstOrDefault().VALOR.Split(',') : query.FirstOrDefault().VALOR.Split(',') : new string[] { string.Empty };
            }
        }

        /// <summary>
        /// Indica el numero de personas que puede autorizar el imputado para que lo visiten, valor default: [12]
        /// </summary>
        public static short LIMITE_VISITA_AUTORIZADA
        {
            get
            {
                var query = new EntityManagerServer<PARAMETRO>().GetData().Where(w => (w.ID_CENTRO == 0 || w.ID_CENTRO == 4) && w.ID_CLAVE.Contains("LIMITE_VISITA_AUTORIZADA"));
                return query.Any() ? query.Count() > 1 ? Convert.ToInt16(query.Where(w => w.ID_CENTRO == 4).FirstOrDefault().VALOR) : Convert.ToInt16(query.FirstOrDefault().VALOR) : (short)12;
            }
        }

        /// <summary>
        /// Indica la edad de un adolecente de sexo femenino, valor default: [17]
        /// </summary>
        public static short EDAD_ADOLECENTE_F
        {
            get
            {
                var query = new EntityManagerServer<PARAMETRO>().GetData().Where(w => (w.ID_CENTRO == 0 || w.ID_CENTRO == 4) && w.ID_CLAVE.Contains("EDAD_ADOLECENTE_F"));
                return query.Any() ? query.Count() > 1 ? Convert.ToInt16(query.Where(w => w.ID_CENTRO == 4).FirstOrDefault().VALOR) : Convert.ToInt16(query.FirstOrDefault().VALOR) : (short)17;
            }
        }

        /// <summary>
        /// Indica la edad de un adolecente de sexo masculino, valor default: [17]
        /// </summary>
        public static short EDAD_ADOLECENTE_M
        {
            get
            {
                var query = new EntityManagerServer<PARAMETRO>().GetData().Where(w => (w.ID_CENTRO == 0 || w.ID_CENTRO == 4) && w.ID_CLAVE.Contains("EDAD_ADOLECENTE_M"));
                return query.Any() ? query.Count() > 1 ? Convert.ToInt16(query.Where(w => w.ID_CENTRO == 4).FirstOrDefault().VALOR) : Convert.ToInt16(query.FirstOrDefault().VALOR) : (short)17;
            }
        }

        /// <summary>
        /// Indica el texto que describe a los formatos de fuero comun
        /// </summary>
        public static string ENCABEZADO_FUERO_COMUN
        {
            get
            {
                var query = new EntityManagerServer<PARAMETRO>().GetData().Where(w => (w.ID_CENTRO == 0 || w.ID_CENTRO == 4) && w.ID_CLAVE.Contains("LEYENDA_ESTUDIOS_FUERO_COMUN"));
                return query.Any() ? query.Where(w => w.ID_CENTRO == 0).FirstOrDefault().VALOR : null;
            }
        }

        public static string ENCABEZADO_FUERO_FEDERAL_1
        {
            get
            {
                var query = new EntityManagerServer<PARAMETRO>().GetData().Where(w => (w.ID_CENTRO == 0 || w.ID_CENTRO == 4) && w.ID_CLAVE.Contains("LEYENDA_ENC_FUERO_FEDERAL_1"));
                return query.Any() ? query.Where(w => w.ID_CENTRO == 0).FirstOrDefault().VALOR : null;
            }
        }

        public static string ENCABEZADO_FUERO_FEDERAL_2
        {
            get
            {
                var query = new EntityManagerServer<PARAMETRO>().GetData().Where(w => (w.ID_CENTRO == 0 || w.ID_CENTRO == 4) && w.ID_CLAVE.Contains("LEYENDA_ENC_FUERO_FEDERAL_2"));
                return query.Any() ? query.Where(w => w.ID_CENTRO == 0).FirstOrDefault().VALOR : null;
            }
        }

        public static string ENCABEZADO_FUERO_FEDERAL_3
        {
            get
            {
                var query = new EntityManagerServer<PARAMETRO>().GetData().Where(w => (w.ID_CENTRO == 0 || w.ID_CENTRO == 4) && w.ID_CLAVE.Contains("LEYENDA_ENC_FUERO_FEDERAL_3"));
                return query.Any() ? query.Where(w => w.ID_CENTRO == 0).FirstOrDefault().VALOR : null;
            }
        }

        public static string ENCABEZADO_FUERO_FEDERAL_4
        {
            get
            {
                var query = new EntityManagerServer<PARAMETRO>().GetData().Where(w => (w.ID_CENTRO == 0 || w.ID_CENTRO == 4) && w.ID_CLAVE.Contains("LEYENDA_ENC_FUERO_FEDERAL_4"));
                return query.Any() ? query.Where(w => w.ID_CENTRO == 0).FirstOrDefault().VALOR : null;
            }
        }

        /// <summary>
        /// Imagen a usar dentro del encabezado de los formatos de fuero federal (CNS al momento de escribir esta descripcion) 
        /// </summary>
        public static byte[] REPORTE_LOGO_FUERO_FEDERAL_1
        {
            get
            {
                var query = new EntityManagerServer<PARAMETRO>().GetData().Where(w => (w.ID_CENTRO == 0 || w.ID_CENTRO == 4) && w.ID_CLAVE.Contains("LOGO_FUERO_FEDERAL_1"));
                return query.Any() ? query.Where(w => w.ID_CENTRO == 0).FirstOrDefault().CONTENIDO : null;
            }
        }

        /// <summary>
        /// Imagen  de la certificacion ISO a usar dentro de formatos de fuero comun (provisto el dia 14 de marzo de 2016) 
        /// </summary>
        public static byte[] REPORTE_LOGO_ISO
        {
            get
            {
                var query = new EntityManagerServer<PARAMETRO>().GetData().Where(w => (w.ID_CENTRO == 0 || w.ID_CENTRO == 4) && w.ID_CLAVE.Contains("LOGO_ISO"));
                return query.Any() ? query.Where(w => w.ID_CENTRO == 0).FirstOrDefault().CONTENIDO : null;
            }
        }

        //LOGO_BC_ACTA_COMUN
        public static byte[] LOGO_BC_ACTA_COMUN
        {
            get
            {
                var query = new EntityManagerServer<PARAMETRO>().GetData().Where(w => (w.ID_CENTRO == 0 || w.ID_CENTRO == 4) && w.ID_CLAVE.Contains("LOGO_BC_ACTA_COMUN"));
                return query.Any() ? query.Where(w => w.ID_CENTRO == 0).FirstOrDefault().CONTENIDO : null;
            }
        }


        public static string DESCR_ISO_1
        {
            get
            {
                var query = new EntityManagerServer<PARAMETRO>().GetData().Where(w => (w.ID_CENTRO == 0 || w.ID_CENTRO == 4) && w.ID_CLAVE.Contains("DESCR_ISO_1"));
                return query.Any() ? query.Where(w => w.ID_CENTRO == 0).FirstOrDefault().VALOR : null;
            }
        }

        public static string DESCR_ISO_2
        {
            get
            {
                var query = new EntityManagerServer<PARAMETRO>().GetData().Where(w => (w.ID_CENTRO == 0 || w.ID_CENTRO == 4) && w.ID_CLAVE.Contains("DESCR_ISO_2"));
                return query.Any() ? query.Where(w => w.ID_CENTRO == 0).FirstOrDefault().VALOR : null;
            }
        }


        public static string DESCR_ISO_3
        {
            get
            {
                var query = new EntityManagerServer<PARAMETRO>().GetData().Where(w => (w.ID_CENTRO == 0 || w.ID_CENTRO == 4) && w.ID_CLAVE.Contains("DESCR_ISO_3"));
                return query.Any() ? query.Where(w => w.ID_CENTRO == 0).FirstOrDefault().VALOR : null;
            }
        }

        /// <summary>
        /// Imagen a usar dentro del encabezado de los formatos de fuero federal (SEGOB al momento de escribir esta descripcion) 
        /// </summary>
        public static byte[] REPORTE_LOGO_FUERO_FEDERAL_2
        {
            get
            {
                var query = new EntityManagerServer<PARAMETRO>().GetData().Where(w => (w.ID_CENTRO == 0 || w.ID_CENTRO == 4) && w.ID_CLAVE.Contains("LOGO_FUERO_FEDERAL_2"));
                return query.Any() ? query.Where(w => w.ID_CENTRO == 0).FirstOrDefault().CONTENIDO : null;
            }
        }

        /// <summary>
        /// Imagen a usar dentro del encabezado de los formatos de fuero federal (SEGOB-MEXICO al momento de escribir esta descripcion) 
        /// </summary>
        public static byte[] REPORTE_LOGO_FUERO_FEDERAL_3
        {
            get
            {
                var query = new EntityManagerServer<PARAMETRO>().GetData().Where(w => (w.ID_CENTRO == 0 || w.ID_CENTRO == 4) && w.ID_CLAVE.Contains("LOGO_FUERO_FEDERAL_3"));
                return query.Any() ? query.Where(w => w.ID_CENTRO == 0).FirstOrDefault().CONTENIDO : null;
            }
        }
        /// <summary>
        /// Indica el logo superior izquierdo de un reporte,valor default: null
        /// </summary>
        public static byte[] REPORTE_LOGO1
        {
            get
            {
                var query = new EntityManagerServer<PARAMETRO>().GetData().Where(w => (w.ID_CENTRO == 0 || w.ID_CENTRO == 4) && w.ID_CLAVE.Contains("REPORTE_LOGO1"));
                return query.Any() ? query.Where(w => w.ID_CENTRO == 0).FirstOrDefault().CONTENIDO : null;
            }
        }

        /// <summary>
        /// Indica el logo superior izquierdo de un reporte,valor default: null
        /// </summary>
        public static byte[] REPORTE_LOGO2
        {
            get
            {
                var query = new EntityManagerServer<PARAMETRO>().GetData().Where(w => (w.ID_CENTRO == 0 || w.ID_CENTRO == 4) && w.ID_CLAVE.Contains("REPORTE_LOGO2"));
                return query.Any() ? query.Where(w => w.ID_CENTRO == 0).FirstOrDefault().CONTENIDO : null;
            }
        }

        /// <summary>
        /// metodo que retorna el logo internacional de medicina 
        /// </summary>
        public static byte[] REPORTE_LOGO_MEDICINA
        {
            get
            {
                var query = new EntityManagerServer<PARAMETRO>().GetData().Where(w => (w.ID_CENTRO == 0 || w.ID_CENTRO == 4) && w.ID_CLAVE.Contains("VARA_ESCULAPIO"));
                return query.Any() ? query.Where(w => w.ID_CENTRO == 0).FirstOrDefault().CONTENIDO : null;
            }
        }

        /// <summary>
        /// Obtiene la imagen para mostrar en los reportes cuando una zona corporal esta seleccionada,valor default: null
        /// </summary>
        public static byte[] IMAGEN_ZONA_CORPORAL
        {
            get
            {
                var query = new EntityManagerServer<PARAMETRO>().GetData().Where(w => (w.ID_CENTRO == 0 || w.ID_CENTRO == 4) && w.ID_CLAVE.Contains("IMAGEN_ZONA_CORPORAL"));
                return query.Any() ? query.Where(w => w.ID_CENTRO == 0).FirstOrDefault().CONTENIDO : null;
            }
        }

        /// <summary>
        /// Obtiene la imagen trasera de la anatomia topografica,valor default: null
        /// </summary>
        public static byte[] CUERPO_DORSO
        {
            get
            {
                var query = new EntityManagerServer<PARAMETRO>().GetData().Where(w => (w.ID_CENTRO == 0 || w.ID_CENTRO == 4) && w.ID_CLAVE.Contains("CUERPO_DORSO"));
                var ret = query.Any() ? query.Where(w => w.ID_CENTRO == 0).FirstOrDefault().CONTENIDO : null;
                return ret;
            }
        }

        /// <summary>
        /// Obtiene la imagen frontal de la anatomia topografica,valor default: null
        /// </summary>
        public static byte[] CUERPO_FRENTE
        {
            get
            {
                var query = new EntityManagerServer<PARAMETRO>().GetData().Where(w => (w.ID_CENTRO == 0 || w.ID_CENTRO == 4) && w.ID_CLAVE.Contains("CUERPO_FRENTE"));
                return query.Any() ? query.Where(w => w.ID_CENTRO == 0).FirstOrDefault().CONTENIDO : null;
            }
        }

        /// <summary>
        /// Indica el logo del estado de Baja California,valor default: null
        /// </summary>
        public static byte[] LOGO_ESTADO
        {
            get
            {
                var query = new EntityManagerServer<PARAMETRO>().GetData().Where(w => (w.ID_CENTRO == 0 || w.ID_CENTRO == 4) && w.ID_CLAVE.Contains("LOGO_ESTADO"));
                return query.Any() ? query.Where(w => w.ID_CENTRO == 0).FirstOrDefault().CONTENIDO : null;
            }
        }


        /// <summary>
        /// Indica el logo del estado de Baja California CON LA IMAGEN DEL SELLO ARRIBA Y EL TEXTO DE BAJA CALIFORNIA ABAJO.
        /// </summary>
        public static byte[] LOGO_ESTADO_BC
        {
            get
            {
                var query = new EntityManagerServer<PARAMETRO>().GetData().Where(w => (w.ID_CENTRO == 0 || w.ID_CENTRO == 4) && w.ID_CLAVE.Contains("LOGO_BC_FORMATOS"));
                return query.Any() ? query.Where(w => w.ID_CENTRO == 4).FirstOrDefault().CONTENIDO : null;
            }
        }

        /// <summary>
        /// Indica encabezado 1 utilizado en los reportes, valor default:""
        /// </summary>
        public static string ENCABEZADO1
        {
            get
            {
                var query = new EntityManagerServer<PARAMETRO>().GetData().Where(w => (w.ID_CENTRO == 0 || w.ID_CENTRO == 4) && w.ID_CLAVE.Contains("ENCABE1"));
                return query.Any() ? query.Where(w => w.ID_CENTRO == 0).FirstOrDefault().VALOR : string.Empty;
            }
        }

        /// <summary>
        /// Indica encabezado 2 utilizado en los reportes, valor default:""
        /// </summary>
        public static string ENCABEZADO2
        {
            get
            {
                var query = new EntityManagerServer<PARAMETRO>().GetData().Where(w => (w.ID_CENTRO == 0 || w.ID_CENTRO == 4) && w.ID_CLAVE.Contains("ENCABE2"));
                return query.Any() ? query.Where(w => w.ID_CENTRO == 0).FirstOrDefault().VALOR : string.Empty;
            }
        }

        /// <summary>
        /// Indica encabezado 3 utilizado en los reportes, valor default:""
        /// </summary>
        public static string ENCABEZADO3
        {
            get
            {
                var query = new EntityManagerServer<PARAMETRO>().GetData().Where(w => (w.ID_CENTRO == 0 || w.ID_CENTRO == 4) && w.ID_CLAVE.Contains("ENCABE3"));
                return query.Any() ? query.Where(w => w.ID_CENTRO == 0).FirstOrDefault().VALOR : string.Empty;
            }
        }

        /// <summary>
        /// Obtiene los ID y DESCR de los CERESOS para la impresion del gafete de ABOGADOS
        /// </summary>
        public static string[] ID_DESCRIPCION_CERESOS
        {
            get
            {
                var query = new EntityManagerServer<PARAMETRO>().GetData().Where(w => w.ID_CENTRO == 0 && w.ID_CLAVE.Contains("ID_DESCRIPCION_CERESOS"));
                //return query.Any() ? query.FirstOrDefault().VALOR : string.Empty;
                return query.Any() ? query.FirstOrDefault().VALOR.Split(',') : new string[] { string.Empty };
            }
        }

        /// <summary>
        /// Obtiene los ID de los documentos que se necesitan para la union entre ABOGADO e INGRESO-CAUSA_PENAL
        /// </summary>
        public static string[] DOCUMENTOS_ABOGADO_INGRESO
        {
            get
            {
                var query = new EntityManagerServer<PARAMETRO>().GetData().Where(w => w.ID_CENTRO == 0 && w.ID_CLAVE.Contains("DOCUMENTOS_ABOGADO_INGRESO"));
                //return query.Any() ? query.FirstOrDefault().VALOR : string.Empty;
                return query.Any() ? query.FirstOrDefault().VALOR.Split(',') : new string[] { string.Empty };
            }
        }

        /// <summary>
        /// Obtiene los ID de los documentos que se necesitan para los ABOGADOS
        /// </summary>
        public static string[] DOCUMENTOS_ABOGADOS
        {
            get
            {
                var query = new EntityManagerServer<PARAMETRO>().GetData().Where(w => w.ID_CENTRO == 0 && w.ID_CLAVE.Contains("DOCUMENTOS_ABOGADOS"));
                //return query.Any() ? query.FirstOrDefault().VALOR : string.Empty;
                return query.Any() ? query.FirstOrDefault().VALOR.Split(',') : new string[] { string.Empty };
            }
        }

        /// <summary>
        /// Obtiene los ID de los documentos que se necesitan para los COLABORADORES
        /// </summary>
        public static string[] DOCUMENTOS_COLABORADORES
        {
            get
            {
                var query = new EntityManagerServer<PARAMETRO>().GetData().Where(w => w.ID_CENTRO == 0 && w.ID_CLAVE.Contains("DOCUMENTOS_COLABORADORES"));
                //return query.Any() ? query.FirstOrDefault().VALOR : string.Empty;
                return query.Any() ? query.FirstOrDefault().VALOR.Split(',') : new string[] { string.Empty };
            }
        }

        /// <summary>
        /// Obtiene los ID de los documentos que se necesitan para los ACTUARIOS
        /// </summary>
        public static string[] DOCUMENTOS_ACTUARIOS
        {
            get
            {
                var query = new EntityManagerServer<PARAMETRO>().GetData().Where(w => w.ID_CENTRO == 0 && w.ID_CLAVE.Contains("DOCUMENTOS_ACTUARIOS"));
                //return query.Any() ? query.FirstOrDefault().VALOR : string.Empty;
                return query.Any() ? query.FirstOrDefault().VALOR.Split(',') : new string[] { string.Empty };
            }
        }

        /// <summary>
        /// Obtiene los ID de los documentos que se necesitan para los ACTUARIOS
        /// </summary>
        public static string[] ESTATUS_ABOGADOS
        {
            get
            {
                var query = new EntityManagerServer<PARAMETRO>().GetData().Where(w => w.ID_CENTRO == 0 && w.ID_CLAVE.Contains("ESTATUS_ABOGADOS"));
                //return query.Any() ? query.FirstOrDefault().VALOR : string.Empty;
                return query.Any() ? query.FirstOrDefault().VALOR.Split(',') : new string[] { string.Empty };
            }
        }

        /// <summary>
        /// Obtiene el ID de CANCELADO del ESTATUS_VISITA
        /// </summary>
        public static short ID_ESTATUS_VISITA_CANCELADO
        {
            get
            {
                var query = new EntityManagerServer<PARAMETRO>().GetData().Where(w => w.ID_CENTRO == 0 && w.ID_CLAVE.Contains("ID_ESTATUS_VISITA_CANCELADO"));
                return query.Any() ? query.Count() > 1 ? Convert.ToInt16(query.Where(w => w.ID_CENTRO == 0).FirstOrDefault().VALOR) : Convert.ToInt16(query.FirstOrDefault().VALOR) : (short)14;
            }
        }

        /// <summary>
        /// Obtiene el ID de SUSPENDIDO del ESTATUS_VISITA
        /// </summary>
        public static short ID_ESTATUS_VISITA_SUSPENDIDO
        {
            get
            {
                var query = new EntityManagerServer<PARAMETRO>().GetData().Where(w => w.ID_CENTRO == 0 && w.ID_CLAVE.Contains("ID_ESTATUS_VISITA_SUSPENDIDO"));
                return query.Any() ? query.Count() > 1 ? Convert.ToInt16(query.Where(w => w.ID_CENTRO == 0).FirstOrDefault().VALOR) : Convert.ToInt16(query.FirstOrDefault().VALOR) : (short)15;
            }
        }

        /// <summary>
        /// Obtiene el ID de REGISTRO del ESTATUS_VISITA
        /// </summary>
        public static short ID_ESTATUS_VISITA_REGISTRO
        {
            get
            {
                var query = new EntityManagerServer<PARAMETRO>().GetData().Where(w => w.ID_CENTRO == 0 && w.ID_CLAVE.Contains("ID_ESTATUS_VISITA_REGISTRO"));
                return query.Any() ? query.Count() > 1 ? Convert.ToInt16(query.Where(w => w.ID_CENTRO == 0).FirstOrDefault().VALOR) : Convert.ToInt16(query.FirstOrDefault().VALOR) : (short)13;
            }
        }

        /// <summary>
        /// Obtiene el ID de EN_TRAMITE del ESTATUS_VISITA
        /// </summary>
        public static short ID_ESTATUS_VISITA_EN_REVISION
        {
            get
            {
                var query = new EntityManagerServer<PARAMETRO>().GetData().Where(w => w.ID_CENTRO == 0 && w.ID_CLAVE.Contains("ID_ESTATUS_VISITA_EN_REVISION"));
                return query.Any() ? query.Count() > 1 ? Convert.ToInt16(query.Where(w => w.ID_CENTRO == 0).FirstOrDefault().VALOR) : Convert.ToInt16(query.FirstOrDefault().VALOR) : (short)12;
            }
        }

        /// <summary>
        /// Obtiene el ID de AUTORIZADO del ESTATUS_VISITA
        /// </summary>
        public static short ID_ESTATUS_VISITA_AUTORIZADO
        {
            get
            {
                var query = new EntityManagerServer<PARAMETRO>().GetData().Where(w => w.ID_CENTRO == 0 && w.ID_CLAVE.Contains("ID_ESTATUS_VISITA_AUTORIZADO"));
                return query.Any() ? query.Count() > 1 ? Convert.ToInt16(query.Where(w => w.ID_CENTRO == 0).FirstOrDefault().VALOR) : Convert.ToInt16(query.FirstOrDefault().VALOR) : (short)1;
            }
        }

        /// <summary>
        /// Obtiene el ID de SOLO DEPOSITANTE del TIPO_VISITANTE
        /// </summary>
        public static short ID_TIPO_VISITANTE_DEPOSITANTE
        {
            get
            {
                var query = new EntityManagerServer<PARAMETRO>().GetData().Where(w => w.ID_CENTRO == 0 && w.ID_CLAVE.Contains("ID_TIPO_VISITANTE_DEPOSITANTE"));
                return query.Any() ? query.Count() > 1 ? Convert.ToInt16(query.Where(w => w.ID_CENTRO == 0).FirstOrDefault().VALOR) : Convert.ToInt16(query.FirstOrDefault().VALOR) : (short)0;
            }
        }

        /// <summary>
        /// Obtiene el ID de DISCAPACITADO del TIPO_VISITANTE
        /// </summary>
        public static short ID_TIPO_VISITANTE_DISCAPACITADO
        {
            get
            {
                var query = new EntityManagerServer<PARAMETRO>().GetData().Where(w => w.ID_CENTRO == 0 && w.ID_CLAVE.Contains("ID_TIPO_VISITANTE_DISCAPACITAD"));
                return query.Any() ? query.Count() > 1 ? Convert.ToInt16(query.Where(w => w.ID_CENTRO == 0).FirstOrDefault().VALOR) : Convert.ToInt16(query.FirstOrDefault().VALOR) : (short)0;
            }
        }

        /// <summary>
        /// Obtiene el ID de ORDINARIO del TIPO_VISITANTE
        /// </summary>
        public static short ID_TIPO_VISITANTE_ORDINARIO
        {
            get
            {
                var query = new EntityManagerServer<PARAMETRO>().GetData().Where(w => w.ID_CENTRO == 0 && w.ID_CLAVE.Contains("ID_TIPO_VISITANTE_ORDINARIO"));
                return query.Any() ? query.Count() > 1 ? Convert.ToInt16(query.Where(w => w.ID_CENTRO == 0).FirstOrDefault().VALOR) : Convert.ToInt16(query.FirstOrDefault().VALOR) : (short)0;
            }
        }

        /// <summary>
        /// Obtiene el ID de FORANEO del TIPO_VISITANTE
        /// </summary>
        public static short ID_TIPO_VISITANTE_FORANEO
        {
            get
            {
                var query = new EntityManagerServer<PARAMETRO>().GetData().Where(w => w.ID_CENTRO == 0 && w.ID_CLAVE.Contains("ID_TIPO_VISITANTE_FORANEO"));
                return query.Any() ? query.Count() > 1 ? Convert.ToInt16(query.Where(w => w.ID_CENTRO == 0).FirstOrDefault().VALOR) : Convert.ToInt16(query.FirstOrDefault().VALOR) : (short)0;
            }
        }

        /// <summary>
        /// Obtiene el ID de INTIMA del TIPO_VISITANTE
        /// </summary>
        public static short ID_TIPO_VISITANTE_INTIMA
        {
            get
            {
                var query = new EntityManagerServer<PARAMETRO>().GetData().Where(w => w.ID_CENTRO == 0 && w.ID_CLAVE.Contains("ID_TIPO_VISITANTE_INTIMA"));
                return query.Any() ? query.Count() > 1 ? Convert.ToInt16(query.Where(w => w.ID_CENTRO == 0).FirstOrDefault().VALOR) : Convert.ToInt16(query.FirstOrDefault().VALOR) : (short)0;
            }
        }

        /// <summary>
        /// Obtiene el ID del AREA al que se dirigira el abogado
        /// </summary>
        public static short AREA_VISITA_ABOGADO
        {
            get
            {
                var query = new EntityManagerServer<PARAMETRO>().GetData().Where(w => w.ID_CENTRO == GlobalVar.gCentro && w.ID_CLAVE.Contains("AREA_VISITA_ABOGADO"));
                return query.Any() ? query.Count() > 1 ? Convert.ToInt16(query.Where(w => w.ID_CENTRO == GlobalVar.gCentro).FirstOrDefault().VALOR_NUM) : Convert.ToInt16(query.FirstOrDefault().VALOR_NUM) : (short)1;
            }
        }

        /// <summary>
        /// Obtiene el ID de FAMILIAR del TIPO_VISITA
        /// </summary>
        public static short ID_TIPO_VISITA_FAMILIAR
        {
            get
            {
                var query = new EntityManagerServer<PARAMETRO>().GetData().Where(w => w.ID_CENTRO == 0 && w.ID_CLAVE.Contains("ID_TIPO_VISITA_FAMILIAR"));
                return query.Any() ? query.Count() > 1 ? Convert.ToInt16(query.Where(w => w.ID_CENTRO == 0).FirstOrDefault().VALOR) : Convert.ToInt16(query.FirstOrDefault().VALOR) : (short)2;
            }
        }

        /// <summary>
        /// Obtiene el ID de INTIMA del TIPO_VISITA
        /// </summary>
        public static short ID_TIPO_VISITA_INTIMA
        {
            get
            {
                var query = new EntityManagerServer<PARAMETRO>().GetData().Where(w => w.ID_CENTRO == 0 && w.ID_CLAVE.Contains("ID_TIPO_VISITA_INTIMA"));
                return query.Any() ? query.Count() > 1 ? Convert.ToInt16(query.Where(w => w.ID_CENTRO == 0).FirstOrDefault().VALOR) : Convert.ToInt16(query.FirstOrDefault().VALOR) : (short)1;
            }
        }

        /// <summary>
        /// Obtiene el ID de INTIMA del TIPO_EMPLEADO
        /// </summary>
        public static short ID_TIPO_VISITA_EMPLEADO
        {
            get
            {
                var query = new EntityManagerServer<PARAMETRO>().GetData().Where(w => w.ID_CENTRO == 0 && w.ID_CLAVE.Contains("ID_TIPO_VISITA_EMPLEADO"));
                return query.Any() ? query.Count() > 1 ? Convert.ToInt16(query.Where(w => w.ID_CENTRO == 0).FirstOrDefault().VALOR) : Convert.ToInt16(query.FirstOrDefault().VALOR) : (short)1;
            }
        }

        /// <summary>
        /// Obtiene el ID de LEGAL del TIPO_VISITA
        /// </summary>
        public static short ID_TIPO_VISITA_LEGAL
        {
            get
            {
                var query = new EntityManagerServer<PARAMETRO>().GetData().Where(w => w.ID_CENTRO == 0 && w.ID_CLAVE.Contains("ID_TIPO_VISITA_LEGAL"));
                return query.Any() ? query.Count() > 1 ? Convert.ToInt16(query.Where(w => w.ID_CENTRO == 0).FirstOrDefault().VALOR) : Convert.ToInt16(query.FirstOrDefault().VALOR) : (short)3;
            }
        }

        /// <summary>
        /// Obtiene el ID de EXTERNA del TIPO_VISITA
        /// </summary>
        public static short ID_TIPO_VISITA_EXTERNA
        {
            get
            {
                var query = new EntityManagerServer<PARAMETRO>().GetData().Where(w => w.ID_CENTRO == 0 && w.ID_CLAVE.Contains("ID_TIPO_VISITA_EXTERNA"));
                return query.Any() ? query.Count() > 1 ? Convert.ToInt16(query.Where(w => w.ID_CENTRO == 0).FirstOrDefault().VALOR) : Convert.ToInt16(query.FirstOrDefault().VALOR) : (short)4;
            }
        }

        /// <summary>
        /// Obtiene el ID de LIBERADO del ESTATUS_ADMINISTRATIVO
        /// </summary>
        public static short ID_ESTATUS_ADMVO_LIBERADO
        {
            get
            {
                var query = new EntityManagerServer<PARAMETRO>().GetData().Where(w => w.ID_CENTRO == 0 && w.ID_CLAVE.Contains("ID_ESTATUS_ADMVO_LIBERADO"));
                return query.Any() ? query.Count() > 1 ? Convert.ToInt16(query.Where(w => w.ID_CENTRO == 0).FirstOrDefault().VALOR) : Convert.ToInt16(query.FirstOrDefault().VALOR) : (short)4;
            }
        }

        /// <summary>
        /// Obtiene el ID de TRASLADO del ID_TRASLADO_ACTIVO
        /// </summary>
        public static string ID_TRASLADO_ACTIVO
        {
            get
            {
                var query = new EntityManagerServer<PARAMETRO>().GetData().Where(w => w.ID_CENTRO == 0 && w.ID_CLAVE.Contains("ID_TRASLADO_ACTIVO"));
                return query.Any() ? query.Count() > 1 ? query.Where(w => w.ID_CENTRO == 0).FirstOrDefault().VALOR.ToString() : query.FirstOrDefault().VALOR.ToString() : string.Empty;
            }
        }

        /// <summary>
        /// Obtiene el ID de TRASLADO del ID_TRASLADO_PROCESO
        /// </summary>
        public static string ID_TRASLADO_PROCESO
        {
            get
            {
                var query = new EntityManagerServer<PARAMETRO>().GetData().Where(w => w.ID_CENTRO == 0 && w.ID_CLAVE.Contains("ID_TRASLADO_PROCESO"));
                return query.Any() ? query.Count() > 1 ? query.Where(w => w.ID_CENTRO == 0).FirstOrDefault().VALOR.ToString() : query.FirstOrDefault().VALOR.ToString() : string.Empty;
            }
        }

        /// <summary>
        /// Obtiene el ID de TRASLADO del ID_TRASLADO_PROGRAMADO
        /// </summary>
        public static string ID_TRASLADO_PROGRAMADO
        {
            get
            {
                var query = new EntityManagerServer<PARAMETRO>().GetData().Where(w => w.ID_CENTRO == 0 && w.ID_CLAVE.Contains("ID_TRASLADO_PROGRAMADO"));
                return query.Any() ? query.Count() > 1 ? query.Where(w => w.ID_CENTRO == 0).FirstOrDefault().VALOR.ToString() : query.FirstOrDefault().VALOR.ToString() : string.Empty;
            }
        }

        /// <summary>
        /// Obtiene el ID de EXCARCELACION del ID_EXCARCELACION_PROGRAMADA
        /// </summary>
        public static string ID_EXCARCELACION_PROGRAMADA
        {
            get
            {
                var query = new EntityManagerServer<PARAMETRO>().GetData().Where(w => w.ID_CENTRO == 0 && w.ID_CLAVE.Contains("ID_EXCARCELACION_PROGRAMADA"));
                return query.Any() ? query.Count() > 1 ? query.Where(w => w.ID_CENTRO == 0).FirstOrDefault().VALOR.ToString() : query.FirstOrDefault().VALOR.ToString() : string.Empty;
            }
        }

        /// <summary>
        /// Obtiene el ID de EXCARCELACION del ID_EXCARCELACION_ACTIVA
        /// </summary>
        public static string ID_EXCARCELACION_ACTIVA
        {
            get
            {
                var query = new EntityManagerServer<PARAMETRO>().GetData().Where(w => w.ID_CENTRO == 0 && w.ID_CLAVE.Contains("ID_EXCARCELACION_ACTIVA"));
                return query.Any() ? query.Count() > 1 ? query.Where(w => w.ID_CENTRO == 0).FirstOrDefault().VALOR.ToString() : query.FirstOrDefault().VALOR.ToString() : string.Empty;
            }
        }

        /// <summary>
        /// Obtiene el ID de EXCARCELACION del ID_EXCARCELACION_PROCESO
        /// </summary>
        public static string ID_EXCARCELACION_PROCESO
        {
            get
            {
                var query = new EntityManagerServer<PARAMETRO>().GetData().Where(w => w.ID_CENTRO == 0 && w.ID_CLAVE.Contains("ID_EXCARCELACION_PROCESO"));
                return query.Any() ? query.Count() > 1 ? query.Where(w => w.ID_CENTRO == 0).FirstOrDefault().VALOR.ToString() : query.FirstOrDefault().VALOR.ToString() : string.Empty;
            }
        }

        /// <summary>
        /// Obtiene el ID de EXCARCELACION del ID_EXCARCELACION_AUTORIZADO
        /// </summary>
        public static string ID_EXCARCELACION_AUTORIZADO
        {
            get
            {
                var query = new EntityManagerServer<PARAMETRO>().GetData().Where(w => w.ID_CENTRO == 0 && w.ID_CLAVE.Contains("ID_EXCARCELACION_AUTORIZADO"));
                return query.Any() ? query.Count() > 1 ? query.Where(w => w.ID_CENTRO == 0).FirstOrDefault().VALOR.ToString() : query.FirstOrDefault().VALOR.ToString() : string.Empty;
            }
        }

        /// <summary>
        /// Obtiene UBICACION EN ESTANCIA
        /// </summary>
        public static string UBICACION_ESTANCIA
        {
            get
            {
                var query = new EntityManagerServer<PARAMETRO>().GetData().Where(w => w.ID_CENTRO == 0 && w.ID_CLAVE.Contains("UBICACION_ESTANCIA"));
                return query.Any() ? query.Count() > 1 ? query.Where(w => w.ID_CENTRO == 0).FirstOrDefault().VALOR.ToString() : query.FirstOrDefault().VALOR.ToString() : string.Empty;
            }
        }

        /// <summary>
        /// Obtiene UBICACION EN TRANSITO
        /// </summary>
        public static string UBICACION_TRANSITO
        {
            get
            {
                var query = new EntityManagerServer<PARAMETRO>().GetData().Where(w => w.ID_CENTRO == 0 && w.ID_CLAVE.Contains("UBICACION_TRANSITO"));
                return query.Any() ? query.Count() > 1 ? query.Where(w => w.ID_CENTRO == 0).FirstOrDefault().VALOR.ToString() : query.FirstOrDefault().VALOR.ToString() : string.Empty;
            }
        }


        /// <summary>
        /// Obtiene el ID de TRASLADO del ESTATUS_ADMINISTRATIVO
        /// </summary>
        public static short ID_ESTATUS_ADMVO_TRASLADO
        {
            get
            {
                var query = new EntityManagerServer<PARAMETRO>().GetData().Where(w => w.ID_CENTRO == 0 && w.ID_CLAVE.Contains("ID_ESTATUS_ADMVO_TRASLADO"));
                return query.Any() ? query.Count() > 1 ? Convert.ToInt16(query.Where(w => w.ID_CENTRO == 0).FirstOrDefault().VALOR) : Convert.ToInt16(query.FirstOrDefault().VALOR) : (short)5;
            }
        }

        /// <summary>
        /// Obtiene el ID de ABOGADO del catalogo ABOGADO_TITULAR
        /// </summary>
        public static string ID_ABOGADO_TITULAR_ABOGADO
        {
            get
            {
                var query = new EntityManagerServer<PARAMETRO>().GetData().Where(w => w.ID_CENTRO == 0 && w.ID_CLAVE.Contains("ID_ABOGADO_TITULAR_ABOGADO"));
                return query.Any() ? query.Count() > 1 ? query.Where(w => w.ID_CENTRO == 0).FirstOrDefault().VALOR.ToString() : query.FirstOrDefault().VALOR.ToString() : "T";
            }
        }

        /// <summary>
        /// Obtiene el ID de COLABORADOR del catalogo ABOGADO_TITULAR
        /// </summary>
        public static string ID_ABOGADO_TITULAR_COLABORADOR
        {
            get
            {
                var query = new EntityManagerServer<PARAMETRO>().GetData().Where(w => w.ID_CENTRO == 0 && w.ID_CLAVE.Contains("ID_ABOGADO_TITULAR_COLABORADOR"));
                return query.Any() ? query.Count() > 1 ? query.Where(w => w.ID_CENTRO == 0).FirstOrDefault().VALOR.ToString() : query.FirstOrDefault().VALOR.ToString() : "T";
            }
        }

        /// <summary>
        /// Obtiene el ID de COLABORADOR del catalogo ABOGADO_TITULAR
        /// </summary>
        public static string ID_ABOGADO_TITULAR_ACTUARIO
        {
            get
            {
                var query = new EntityManagerServer<PARAMETRO>().GetData().Where(w => w.ID_CENTRO == 0 && w.ID_CLAVE.Contains("ID_ABOGADO_TITULAR_ACTUARIO"));
                return query.Any() ? query.Count() > 1 ? query.Where(w => w.ID_CENTRO == 0).FirstOrDefault().VALOR.ToString() : query.FirstOrDefault().VALOR.ToString() : "T";
            }
        }

        /// <summary>
        /// Obtiene los ID de los documentos que se necesitan para la union entre ABOGADO e INGRESO-CAUSA_PENAL
        /// </summary>
        public static string[] DOCUMENTOS_COLABORADOR_INGRESO
        {
            get
            {
                var query = new EntityManagerServer<PARAMETRO>().GetData().Where(w => w.ID_CENTRO == 0 && w.ID_CLAVE.Contains("DOCUMENTOS_COLABORADOR_INGRESO"));
                //return query.Any() ? query.FirstOrDefault().VALOR : string.Empty;
                return query.Any() ? query.FirstOrDefault().VALOR.Split(',') : new string[] { string.Empty };
            }
        }

        /// <summary>
        /// Obtiene el ID de EMPLEADO del catalogo TIPO_PERSONA
        /// </summary>
        public static string ID_TIPO_PERSONA_EMPLEADO
        {
            get
            {
                var query = new EntityManagerServer<PARAMETRO>().GetData().Where(w => w.ID_CENTRO == 0 && w.ID_CLAVE.Contains("ID_TIPO_PERSONA_EMPLEADO"));
                return query.Any() ? query.Count() > 1 ? query.Where(w => w.ID_CENTRO == 0).FirstOrDefault().VALOR.ToString() : query.FirstOrDefault().VALOR.ToString() : "1";
            }
        }

        /// <summary>
        /// Obtiene el ID de VISITA FAMILIAR del catalogo TIPO_PERSONA
        /// </summary>
        public static string ID_TIPO_PERSONA_VISITA
        {
            get
            {
                var query = new EntityManagerServer<PARAMETRO>().GetData().Where(w => w.ID_CENTRO == 0 && w.ID_CLAVE.Contains("ID_TIPO_PERSONA_VISITA"));
                return query.Any() ? query.Count() > 1 ? query.Where(w => w.ID_CENTRO == 0).FirstOrDefault().VALOR.ToString() : query.FirstOrDefault().VALOR.ToString() : "1";
            }
        }

        /// <summary>
        /// Obtiene el ID de EXTERNA del catalogo TIPO_PERSONA
        /// </summary>
        public static string ID_TIPO_PERSONA_EXTERNA
        {
            get
            {
                var query = new EntityManagerServer<PARAMETRO>().GetData().Where(w => w.ID_CENTRO == 0 && w.ID_CLAVE.Contains("ID_TIPO_PERSONA_EXTERNA"));
                return query.Any() ? query.Count() > 1 ? query.Where(w => w.ID_CENTRO == 0).FirstOrDefault().VALOR.ToString() : query.FirstOrDefault().VALOR.ToString() : "1";
            }
        }

        /// <summary>
        /// Obtiene el ID de LEGAL del catalogo TIPO_PERSONA
        /// </summary>
        public static string ID_TIPO_PERSONA_LEGAL
        {
            get
            {
                var query = new EntityManagerServer<PARAMETRO>().GetData().Where(w => w.ID_CENTRO == 0 && w.ID_CLAVE.Contains("ID_TIPO_PERSONA_LEGAL"));
                return query.Any() ? query.Count() > 1 ? query.Where(w => w.ID_CENTRO == 0).FirstOrDefault().VALOR.ToString() : query.FirstOrDefault().VALOR.ToString() : "1";
            }
        }

        /// <summary>
        /// Horario minimo de entrada permitida de abogados
        /// </summary>
        public static string HORARIO_MINIMO_ENTRADA_ABOGADOS
        {
            get
            {
                var query = new EntityManagerServer<PARAMETRO>().GetData().Where(w => w.ID_CENTRO == 0 && w.ID_CLAVE.Contains("HORARIO_MINIMO_ENTRADA_ABO"));
                return query.Any() ? query.Count() > 1 ? query.Where(w => w.ID_CENTRO == 0).FirstOrDefault().VALOR.ToString() : query.FirstOrDefault().VALOR.ToString() : string.Empty;
            }
        }

        /// <summary>
        /// Horario maximo de entrada permitida de abogados
        /// </summary>
        public static string HORARIO_MAXIMO_ENTRADA_ABOGADOS
        {
            get
            {
                var query = new EntityManagerServer<PARAMETRO>().GetData().Where(w => w.ID_CENTRO == 0 && w.ID_CLAVE.Contains("HORARIO_MAXIMO_ENTRADA_ABO"));
                return query.Any() ? query.Count() > 1 ? query.Where(w => w.ID_CENTRO == 0).FirstOrDefault().VALOR.ToString() : query.FirstOrDefault().VALOR.ToString() : string.Empty;
            }
        }

        /// <summary>
        /// Obtiene los ID de los documentos que se necesitan para la union entre ABOGADO e INGRESO-CAUSA_PENAL
        /// </summary>
        public static string[] DOCUMENTO_ASIGNACION_JUEZ
        {
            get
            {
                var query = new EntityManagerServer<PARAMETRO>().GetData().Where(w => w.ID_CENTRO == 0 && w.ID_CLAVE.Contains("DOCUMENTO_ASIGNACION_JUEZ"));
                //return query.Any() ? query.FirstOrDefault().VALOR : string.Empty;
                return query.Any() ? query.FirstOrDefault().VALOR.Split('-') : new string[] { string.Empty };
            }
        }

        /// <summary>
        /// Obtiene los ID de los elementos que no se necesitan mostrar en la NOTA MEDICA
        /// </summary>
        public static string[] NOTA_MEDICA_TIPO_SERVICIO
        {
            get
            {
                var query = new EntityManagerServer<PARAMETRO>().GetData().Where(w => w.ID_CENTRO == 0 && w.ID_CLAVE.Contains("NOTA_MEDICA_TIPO_SERVICIO"));
                //return query.Any() ? query.FirstOrDefault().VALOR : string.Empty;
                return query.Any() ? query.FirstOrDefault().VALOR.Split(',') : new string[] { string.Empty };
            }
        }

        /// <summary>
        /// Obtiene los ID de los documentos que se necesitan para la union entre ABOGADO e INGRESO-CAUSA_PENAL
        /// </summary>
        public static string[] DOCUMENTO_ASIGNACION_INTERNO
        {
            get
            {
                var query = new EntityManagerServer<PARAMETRO>().GetData().Where(w => w.ID_CENTRO == 0 && w.ID_CLAVE.Contains("DOCUMENTO_ASIGNACION_INTERNO"));
                //return query.Any() ? query.FirstOrDefault().VALOR : string.Empty;
                return query.Any() ? query.FirstOrDefault().VALOR.Split('-') : new string[] { string.Empty };
            }
        }

        /// <summary>
        /// Obtiene el ID de LIBERADO del ESTATUS_ADMINISTRATIVO
        /// </summary>
        public static short ID_TIPO_ABO_ADMVO
        {
            get
            {
                var query = new EntityManagerServer<PARAMETRO>().GetData().Where(w => w.ID_CENTRO == 0 && w.ID_CLAVE.Contains("ID_TIPO_ABO_ADMVO"));
                return query.Any() ? query.Count() > 1 ? Convert.ToInt16(query.Where(w => w.ID_CENTRO == 0).FirstOrDefault().VALOR) : Convert.ToInt16(query.FirstOrDefault().VALOR) : (short)4;
            }
        }

        /// <summary>
        /// Obtiene el ID de LIBERADO del ESTATUS_ADMINISTRATIVO
        /// </summary>
        public static short ID_TIPO_ABO_PROCESADO
        {
            get
            {
                var query = new EntityManagerServer<PARAMETRO>().GetData().Where(w => w.ID_CENTRO == 0 && w.ID_CLAVE.Contains("ID_TIPO_ABO_PROCESADO"));
                return query.Any() ? query.Count() > 1 ? Convert.ToInt16(query.Where(w => w.ID_CENTRO == 0).FirstOrDefault().VALOR) : Convert.ToInt16(query.FirstOrDefault().VALOR) : (short)4;
            }
        }

        /// <summary>
        /// Obtiene el ID de LIBERADO del ESTATUS_ADMINISTRATIVO
        /// </summary>
        public static short ID_TIPO_ABO_SENTENCIADO
        {
            get
            {
                var query = new EntityManagerServer<PARAMETRO>().GetData().Where(w => w.ID_CENTRO == 0 && w.ID_CLAVE.Contains("ID_TIPO_ABO_SENTENCIADO"));
                return query.Any() ? query.Count() > 1 ? Convert.ToInt16(query.Where(w => w.ID_CENTRO == 0).FirstOrDefault().VALOR) : Convert.ToInt16(query.FirstOrDefault().VALOR) : (short)4;
            }
        }

        /// <summary>
        /// Obtiene el ID de IFE de TIPO_VISITA LEGAL en TIPO_DOCTO
        /// </summary>
        public static short TIPO_DOCTO_IFE_LEGAL
        {
            get
            {
                var query = new EntityManagerServer<PARAMETRO>().GetData().Where(w => w.ID_CENTRO == 0 && w.ID_CLAVE.Contains("TIPO_DOCTO_IFE_LEGAL"));
                return query.Any() ? query.Count() > 1 ? Convert.ToInt16(query.Where(w => w.ID_CENTRO == 0).FirstOrDefault().VALOR.Split('-')[1]) : Convert.ToInt16(query.FirstOrDefault().VALOR.Split('-')[1]) : (short)0;
            }
        }

        /// <summary>
        /// Obtiene el ID de CREDENCIAL ESCOLAR de TIPO_VISITA LEGAL en TIPO_DOCTO
        /// </summary>
        public static short TIPO_DOCTO_CRED_LEGAL
        {
            get
            {
                var query = new EntityManagerServer<PARAMETRO>().GetData().Where(w => w.ID_CENTRO == 0 && w.ID_CLAVE.Contains("TIPO_DOCTO_CRED_LEGAL"));
                return query.Any() ? query.Count() > 1 ? Convert.ToInt16(query.Where(w => w.ID_CENTRO == 0).FirstOrDefault().VALOR.Split('-')[1]) : Convert.ToInt16(query.FirstOrDefault().VALOR.Split('-')[1]) : (short)0;
            }
        }

        /// <summary>
        /// Obtiene el ID de CARTA PASANTE de TIPO_VISITA LEGAL en TIPO_DOCTO
        /// </summary>
        public static short TIPO_DOCTO_CARTA_LEGAL
        {
            get
            {
                var query = new EntityManagerServer<PARAMETRO>().GetData().Where(w => w.ID_CENTRO == 0 && w.ID_CLAVE.Contains("TIPO_DOCTO_CARTA_LEGAL"));
                return query.Any() ? query.Count() > 1 ? Convert.ToInt16(query.Where(w => w.ID_CENTRO == 0).FirstOrDefault().VALOR.Split('-')[1]) : Convert.ToInt16(query.FirstOrDefault().VALOR.Split('-')[1]) : (short)0;
            }
        }

        /// <summary>
        /// Obtiene el ID de TITULO de TIPO_VISITA LEGAL en TIPO_DOCTO
        /// </summary>
        public static short TIPO_DOCTO_TITULO_LEGAL
        {
            get
            {
                var query = new EntityManagerServer<PARAMETRO>().GetData().Where(w => w.ID_CENTRO == 0 && w.ID_CLAVE.Contains("TIPO_DOCTO_TITUL_LEGAL"));
                return query.Any() ? query.Count() > 1 ? Convert.ToInt16(query.Where(w => w.ID_CENTRO == 0).FirstOrDefault().VALOR.Split('-')[1]) : Convert.ToInt16(query.FirstOrDefault().VALOR.Split('-')[1]) : (short)0;
            }
        }

        /// <summary>
        /// Obtiene el ID de TITULO de TIPO_VISITA LEGAL en TIPO_DOCTO
        /// </summary>
        public static short TIPO_DOCTO_CEDULA_LEGAL
        {
            get
            {
                var query = new EntityManagerServer<PARAMETRO>().GetData().Where(w => w.ID_CENTRO == 0 && w.ID_CLAVE.Contains("TIPO_DOCTO_CEDULA_LEGAL"));
                return query.Any() ? query.Count() > 1 ? Convert.ToInt16(query.Where(w => w.ID_CENTRO == 0).FirstOrDefault().VALOR.Split('-')[1]) : Convert.ToInt16(query.FirstOrDefault().VALOR.Split('-')[1]) : (short)0;
            }
        }

        /// <summary>
        /// Obtiene los ID de los documentos que se necesitan para el registro de VISITA INTIMA
        /// </summary>
        public static string[] DOCTOS_NECESARIOS_INTIMA
        {
            get
            {
                var query = new EntityManagerServer<PARAMETRO>().GetData().Where(w => w.ID_CENTRO == 0 && w.ID_CLAVE.Contains("DOCTOS_NECESARIOS_INTIMA"));
                //return query.Any() ? query.FirstOrDefault().VALOR : string.Empty;
                return query.Any() ? query.FirstOrDefault().VALOR.Split(',') : new string[] { string.Empty };
            }
        }

        /// <summary>
        /// Obtiene los ID de los documentos que se necesitan para el registro de VISITA FAMILIAR
        /// </summary>
        public static string[] DOCTOS_NECESARIOS_FAMILIAR
        {
            get
            {
                var query = new EntityManagerServer<PARAMETRO>().GetData().Where(w => w.ID_CENTRO == 0 && w.ID_CLAVE.Contains("DOCTOS_NECESARIOS_FAMILIAR"));
                //return query.Any() ? query.FirstOrDefault().VALOR : string.Empty;
                return query.Any() ? query.FirstOrDefault().VALOR.Split(',') : new string[] { string.Empty };
            }
        }

        /// <summary>
        /// Otiene los IDs de los documentos para iniciar el registro de VISITA_INTIMA
        /// </summary>
        public static string[] DOCTOS_FAMILIAR_INTIMA
        {
            get
            {
                var query = new EntityManagerServer<PARAMETRO>().GetData().Where(w => w.ID_CENTRO == 0 && w.ID_CLAVE.Contains("DOCTOS_FAMILIAR_INTIMA"));
                //return query.Any() ? query.FirstOrDefault().VALOR : string.Empty;
                return query.Any() ? query.FirstOrDefault().VALOR.Split(',') : new string[] { string.Empty };
            }
        }

        /// <summary>
        /// Obtiene el ID del Motivo del Traslado para Salida/Egreso de un Imputado
        /// </summary>
        public static int MOTIVO_TRASLADO
        {
            get
            {
                var query = new EntityManagerServer<PARAMETRO>().GetData().Where(w => w.ID_CENTRO == 0 && w.ID_CLAVE.Contains("MOTIVO_TRASLADO"));
                return query.Any() ? query.Count() > 1 ? query.Where(w => w.ID_CENTRO == 0).FirstOrDefault().VALOR_NUM.Value : query.FirstOrDefault().VALOR_NUM.Value : 5;
            }
        }

        /// <summary>
        /// Obtiene el tipo de ingreso relacionado con el traslado
        /// </summary>
        public static int TRASLADO_TIPO_INGRESO
        {
            get
            {
                var query = new EntityManagerServer<PARAMETRO>().GetData().Where(w => w.ID_CENTRO == 0 && w.ID_CLAVE.Contains("TRASLADO_TIPO_INGRESO"));
                return query.Any() ? query.Count() > 1 ? query.Where(w => w.ID_CENTRO == 0).FirstOrDefault().VALOR_NUM.Value : query.FirstOrDefault().VALOR_NUM.Value : 3;
            }
        }

        /// <summary>
        /// Obtiene el tipo de ingreso relacionado con el traslado foraneo
        /// </summary>
        public static int TRASLADO_FOREANO_TIPO_INGRESO
        {
            get
            {
                var query = new EntityManagerServer<PARAMETRO>().GetData().Where(w => w.ID_CENTRO == 0 && w.ID_CLAVE.Contains("TRASLADO_FOREANO_TIPO_INGRESO"));
                return query.Any() ? query.Count() > 1 ? query.Where(w => w.ID_CENTRO == 0).FirstOrDefault().VALOR_NUM.Value : query.FirstOrDefault().VALOR_NUM.Value : 7;
            }
        }

        public static string AUTORIDAD_TRASLADO
        {
            get
            {
                var query = new EntityManagerServer<PARAMETRO>().GetData(w => w.ID_CENTRO == 0 && w.ID_CLAVE.Contains("AUTORIDAD_TRASLADO"));
                return query.Any() ? query.Count() > 1 ? query.Where(w => w.ID_CENTRO == 0).FirstOrDefault().VALOR : query.FirstOrDefault().VALOR : "";
            }
        }

        /// <summary>
        /// Obtiene la vigencia en dias que tiene un password una vez que este ha sido modificado
        /// </summary>
        public static int DIAS_PASSWORD
        {
            get
            {
                var query = new EntityManagerServer<PARAMETRO>().GetData().Where(w => w.ID_CENTRO == 0 && w.ID_CLAVE.Contains("DIAS_PASSWORD"));
                return query.Any() ? query.Count() > 1 ? query.Where(w => w.ID_CENTRO == 0).FirstOrDefault().VALOR_NUM.Value : query.FirstOrDefault().VALOR_NUM.Value : 30;
            }
        }

        /// <summary>
        /// Obtiene los IDs de los ESTATUS que no estan activos en un centro.
        /// </summary>
        public static short?[] ESTATUS_ADMINISTRATIVO_INACT
        {
            get
            {
                var query = new EntityManagerServer<PARAMETRO>().GetData().Where(w => w.ID_CENTRO == 0 && w.ID_CLAVE.Contains("ESTATUS_ADMINISTRATIVO_INACT"));
                //return query.Any() ? query.FirstOrDefault().VALOR : string.Empty;
                return query.Any() ? query.FirstOrDefault().VALOR.Split(',').Select(s => string.IsNullOrEmpty(s) ? new Nullable<short>() : short.Parse(s)).ToArray() : new short?[] { new Nullable<short>() };
            }
        }

        /// <summary>
        /// Obtiene las horas maximas para que muestre una notificacion en la bitacora de registro de aduana.
        /// </summary>
        public static int TIEMPO_DENTRO_CENTRO
        {
            get
            {
                var query = new EntityManagerServer<PARAMETRO>().GetData().Where(w => w.ID_CENTRO == 0 && w.ID_CLAVE.Contains("TIEMPO_DENTRO_CENTRO"));
                return query.Any() ? query.Count() > 1 ? int.Parse(query.Where(w => w.ID_CENTRO == 0).FirstOrDefault().VALOR) : int.Parse(query.FirstOrDefault().VALOR) : 0;
            }
        }

        /// <summary>
        /// Obtiene el ID del EMISOR definido como OTROS 
        /// </summary>
        public static int ID_EMISOR_OTROS
        {
            get
            {
                var query = new EntityManagerServer<PARAMETRO>().GetData().Where(w => w.ID_CENTRO == 0 && w.ID_CLAVE.Contains("ID_EMISOR_OTROS"));
                return query.Any() ? query.Count() > 1 ? query.Where(w => w.ID_CENTRO == 0).FirstOrDefault().VALOR_NUM.Value : query.FirstOrDefault().VALOR_NUM.Value : 999999;
            }
        }

        /// <summary>
        /// Obtiene el ID del HOSPITAL definido como OTROS 
        /// </summary>
        public static int ID_HOSPITAL_OTROS
        {
            get
            {
                var query = new EntityManagerServer<PARAMETRO>().GetData().Where(w => w.ID_CENTRO == 0 && w.ID_CLAVE.Contains("ID_HOSPITAL_OTROS"));
                return query.Any() ? query.Count() > 1 ? query.Where(w => w.ID_CENTRO == 0).FirstOrDefault().VALOR_NUM.Value : query.FirstOrDefault().VALOR_NUM.Value : 8;
            }
        }

        /// <summary>
        /// Obtiene el ID del PAIS preconfigurado para el sistema
        /// </summary>
        public static short PAIS
        {
            get
            {
                var query = new EntityManagerServer<PARAMETRO>().GetData().Where(w => w.ID_CENTRO == 0 && w.ID_CLAVE.Contains("PAIS"));
                return (short)(query.Any() ? query.Count() > 1 ? query.Where(w => w.ID_CENTRO == 0).FirstOrDefault().VALOR_NUM.Value : query.FirstOrDefault().VALOR_NUM.Value : 82);
            }
        }
        /// <summary>
        /// Obtiene el ID del IDIOMA 
        /// </summary>
        public static short IDIOMA
        {
            get
            {
                var query = new EntityManagerServer<PARAMETRO>().GetData().Where(w => w.ID_CENTRO == 0 && w.ID_CLAVE.Contains("IDIOMA"));
                return (short)(query.Any() ? query.Count() > 1 ? query.Where(w => w.ID_CENTRO == 0).FirstOrDefault().VALOR_NUM.Value : query.FirstOrDefault().VALOR_NUM.Value : 8);
            }
        }

        /// <summary>
        /// Obtiene el ID del ESTADO preconfigurado para el sistema
        /// </summary>
        public static short ESTADO
        {
            get
            {
                var query = new EntityManagerServer<PARAMETRO>().GetData().Where(w => w.ID_CENTRO == 0 && w.ID_CLAVE.Trim() == "ESTADO");
                return Int16.Parse((query.Any() ? query.Count() > 1 ? query.Where(w => w.ID_CENTRO == 0).FirstOrDefault().VALOR : query.FirstOrDefault().VALOR : "2"));
            }
        }

        /// <summary>
        /// Obtiene el ID del tipo de mensaje para interconeccion
        /// </summary>
        public static short ID_TIPO_MENSAJE_INTERCONECCION
        {
            get
            {
                var query = new EntityManagerServer<PARAMETRO>().GetData().Where(w => w.ID_CENTRO == 0 && w.ID_CLAVE.Contains("ID_TIPO_MENSAJE_INTERCONECCION"));
                return (short)(query.Any() ? query.Count() > 1 ? query.Where(w => w.ID_CENTRO == 0).FirstOrDefault().VALOR_NUM.Value : query.FirstOrDefault().VALOR_NUM.Value : 80);
            }
        }

        /// <summary>
        /// Obtiene el ID de Formato de Imagen para Documentos
        /// </summary>
        public static short ID_FORMATO_IMAGEN
        {
            get
            {
                var query = new EntityManagerServer<PARAMETRO>().GetData().Where(w => w.ID_CENTRO == 0 && w.ID_CLAVE.Trim()=="ID_FORMATO_IMAGEN");
                return (short)(query.Any() ? query.Count() > 1 ? query.Where(w => w.ID_CENTRO == 0).FirstOrDefault().VALOR_NUM.Value : query.FirstOrDefault().VALOR_NUM.Value : 5);
            }
        }

        /// <summary>
        /// Obtiene si se debe de verificar el serial
        /// </summary>
        public static bool VERIFICA_HD_SERIAL
        {
            get
            {
                var query = new EntityManagerServer<PARAMETRO>().GetData().Where(w => w.ID_CENTRO == GlobalVar.gCentro && w.ID_CLAVE.Contains("VERIFICA_HD_SERIAL"));
                return (bool)(query.Any() ? query.Count() > 1 ? query.Where(w => w.ID_CENTRO == GlobalVar.gCentro).FirstOrDefault().VALOR_NUM.Value == 1 ? true : false : query.FirstOrDefault().VALOR_NUM.Value == 1 ? true : false : false);
            }
        }

        /// <summary>
        /// Obtiene el rango maximo de dias para busqueda en la bandeja de entrada
        /// </summary>
        public static short DIAS_BANDEJA
        {
            get
            {
                var query = new EntityManagerServer<PARAMETRO>().GetData().Where(w => w.ID_CENTRO == 0 && w.ID_CLAVE.Contains("DIAS_BANDEJA"));
                return (short)(query.Any() ? query.Count() > 1 ? query.Where(w => w.ID_CENTRO == 0).FirstOrDefault().VALOR_NUM.Value : query.FirstOrDefault().VALOR_NUM.Value : 3);
            }
        }

        /// <summary>
        /// Obtiene el numero de solicitudes por mes a los que tiene derecho un interno
        /// </summary>
        public static short SOLICITUD_ATENCION_POR_MES
        {
            get
            {
                var query = new EntityManagerServer<PARAMETRO>().GetData().Where(w => w.ID_CENTRO == 0 && w.ID_CLAVE.Contains("SOLICITUD_ATENCION_POR_MES"));
                return (short)(query.Any() ? query.Count() > 1 ? query.Where(w => w.ID_CENTRO == 0).FirstOrDefault().VALOR_NUM.Value : query.FirstOrDefault().VALOR_NUM.Value : 2);
            }
        }

        /// <summary>
        /// Obtiene el modo de visita de los centros
        /// </summary>
        public static short MODO_VISITA
        {
            get
            {
                var query = new EntityManagerServer<PARAMETRO>().GetData().Where(w => w.ID_CENTRO == GlobalVar.gCentro && w.ID_CLAVE.Contains("MODO_VISITA"));
                return (short)(query.Any() ? query.Count() > 1 ? query.Where(w => w.ID_CENTRO == GlobalVar.gCentro).FirstOrDefault().VALOR_NUM.Value : query.FirstOrDefault().VALOR_NUM.Value : 1);
            }
        }

        /// <summary>
        /// Obtiene la tolerancia de asistencia de una actividad
        /// </summary>
        public static short TOLERANCIA_ASISTENCIA
        {
            get
            {
                var query = new EntityManagerServer<PARAMETRO>().GetData().Where(w => w.ID_CENTRO == 0 && w.ID_CLAVE.Contains("TOLERANCIA_ASISTENCIA"));
                return (short)(query.Any() ? query.Count() > 1 ? query.Where(w => w.ID_CENTRO == 0).FirstOrDefault().VALOR_NUM.Value : query.FirstOrDefault().VALOR_NUM.Value : 25);
            }
        }


        /// <summary>
        /// Obtiene si el certificado medico es obligatorio por centro
        /// </summary>
        public static short CERT_MEDICO_OBLIGATORIO
        {
            get
            {
                var query = new EntityManagerServer<PARAMETRO>().GetData().Where(w => w.ID_CENTRO == GlobalVar.gCentro && w.ID_CLAVE.Contains("CERT_MEDICO_OBLIGATORIO"));
                return (short)(query.Any() ? query.Count() > 1 ? query.Where(w => w.ID_CENTRO == GlobalVar.gCentro).FirstOrDefault().VALOR_NUM.Value : query.FirstOrDefault().VALOR_NUM.Value : 0);
            }
        }

        /// <summary>
        /// Obtiene si el certificado medico es obligatorio por centro
        /// </summary>
        public static short TIPO_VISITA_POR_CENTRO
        {
            get
            {
                var query = new EntityManagerServer<PARAMETRO>().GetData().Where(w => w.ID_CENTRO == GlobalVar.gCentro && w.ID_CLAVE.Contains("TIPO_VISITA_POR_CENTRO"));
                return short.Parse(query.Any() ? query.Count() > 1 ? query.Where(w => w.ID_CENTRO == GlobalVar.gCentro).FirstOrDefault().VALOR : query.FirstOrDefault().VALOR : "0");
            }
        }

        /// <summary>
        /// Indica cuantos interno puede visitar por dia cada visitante.
        /// </summary>
        public static short INTERNOS_PERMITIDOS_POR_DIA
        {
            get
            {
                var query = new EntityManagerServer<PARAMETRO>().GetData().Where(w => w.ID_CENTRO == GlobalVar.gCentro && w.ID_CLAVE.Contains("INTERNOS_PERMITIDOS_POR_DIA"));
                return short.Parse(query.Any() ? query.Count() > 1 ? query.Where(w => w.ID_CENTRO == GlobalVar.gCentro).FirstOrDefault().VALOR : query.FirstOrDefault().VALOR : "0");
            }
        }

        /// <summary>
        /// Indica cuantos visitantes puede recibir cada imputado a la vez.
        /// </summary>
        public static short VISITA_POR_IMPUTADO
        {
            get
            {
                var query = new EntityManagerServer<PARAMETRO>().GetData().Where(w => w.ID_CENTRO == GlobalVar.gCentro && w.ID_CLAVE.Contains("VISITA_POR_IMPUTADO"));
                return short.Parse(query.Any() ? query.Count() > 1 ? query.Where(w => w.ID_CENTRO == GlobalVar.gCentro).FirstOrDefault().VALOR : query.FirstOrDefault().VALOR : "0");
            }
        }

        /// <summary>
        /// Indica los estatus visibles en modulo de autorización de excarcelaciones para la dir. del CERESO
        /// </summary>
        public static string AUT_ESTATUS_EXCARC_DIRECCION
        {
            get
            {
                var query = new EntityManagerServer<PARAMETRO>().GetData().Where(w => w.ID_CENTRO == 0 && w.ID_CLAVE.Contains("AUT_ESTATUS_EXCARC_DIRECCION"));
                return query.Any() ? query.Count() > 1 ? query.Where(w => w.ID_CENTRO == 0).FirstOrDefault().VALOR : query.FirstOrDefault().VALOR : "PR,CA,AU";
            }
        }

        /// <summary>
        /// Indica el ID del area correspondiente a traslados/excarcelaciones
        /// </summary>
        public static short ID_AREA_TRASLADO
        {
            get
            {
                var query = new EntityManagerServer<PARAMETRO>().GetData().Where(w => w.ID_CENTRO == 0 && w.ID_CLAVE.Contains("ID_AREA_TRASLADO"));
                return (short)(query.Any() ? query.Count() > 1 ? query.Where(w => w.ID_CENTRO == 0).FirstOrDefault().VALOR_NUM.HasValue ? query.Where(w => w.ID_CENTRO == 0).FirstOrDefault().VALOR_NUM.Value :
                    0 : query.FirstOrDefault().VALOR_NUM.HasValue ? query.FirstOrDefault().VALOR_NUM.Value : 111 : 111);
            }
        }

        /// <summary>
        /// Indica el ID del area de sala de cabos
        /// </summary>
        public static short ID_AREA_SALA_CABOS
        {
            get
            {
                var query = new EntityManagerServer<PARAMETRO>().GetData().Where(w => w.ID_CENTRO == 0 && w.ID_CLAVE.Contains("ID_AREA_SALA_CABOS"));
                return (short)(query.Any() ? query.Count() > 1 ? query.Where(w => w.ID_CENTRO == 0).FirstOrDefault().VALOR_NUM.HasValue ? query.Where(w => w.ID_CENTRO == 0).FirstOrDefault().VALOR_NUM.Value :
                    0 : query.FirstOrDefault().VALOR_NUM.HasValue ? query.FirstOrDefault().VALOR_NUM.Value : 42 : 42);
            }
        }

        /// <summary>
        /// Indica los minutos de tolerancia para la toma de asistencia de actividades programadas de una hora
        /// </summary>
        public static short TOLERANCIA_ASISTENCIA_HORA
        {
            get
            {
                var query = new EntityManagerServer<PARAMETRO>().GetData().Where(w => w.ID_CENTRO == 0 && w.ID_CLAVE.Contains("TOLERANCIA_ASISTENCIA_HORA"));
                return (short)(query.Any() ? query.Count() > 1 ? query.Where(w => w.ID_CENTRO == 0).FirstOrDefault().VALOR_NUM.HasValue ? query.Where(w => w.ID_CENTRO == 0).FirstOrDefault().VALOR_NUM.Value :
                    0 : query.FirstOrDefault().VALOR_NUM.HasValue ? query.FirstOrDefault().VALOR_NUM.Value : 25 : 25);
            }
        }

        /// <summary>
        /// Indica los minutos de tolerancia para la toma de asistencia de actividades programadas de más de una hora
        /// </summary>
        public static short TOLERANCIA_ASISTENCIA_HORAS
        {
            get
            {
                var query = new EntityManagerServer<PARAMETRO>().GetData().Where(w => w.ID_CENTRO == 0 && w.ID_CLAVE.Contains("TOLERANCIA_ASISTENCIA_HORAS"));
                return (short)(query.Any() ? query.Count() > 1 ? query.Where(w => w.ID_CENTRO == 0).FirstOrDefault().VALOR_NUM.HasValue ? query.Where(w => w.ID_CENTRO == 0).FirstOrDefault().VALOR_NUM.Value :
                    0 : query.FirstOrDefault().VALOR_NUM.HasValue ? query.FirstOrDefault().VALOR_NUM.Value : 50 : 50);
            }
        }


        /// <summary>
        /// Indica los minutos de tolerancia para la toma de asistencia de actividades programadas de más de una hora
        /// </summary>
        public static short TOLERANCIA_EXC_EDIFICIO
        {
            get
            {
                var query = new EntityManagerServer<PARAMETRO>().GetData().Where(w => w.ID_CENTRO == 0 && w.ID_CLAVE.Contains("TOLERANCIA_EXC_EDIFICIO"));
                return (short)(query.Any() ? query.Count() > 1 ? query.Where(w => w.ID_CENTRO == 0).FirstOrDefault().VALOR_NUM.HasValue ? query.Where(w => w.ID_CENTRO == 0).FirstOrDefault().VALOR_NUM.Value :
                    0 : query.FirstOrDefault().VALOR_NUM.HasValue ? query.FirstOrDefault().VALOR_NUM.Value : 3 : 3);
            }
        }


        /// <summary>
        /// Indica los minutos de tolerancia para dar salida  a los internos del edificio a sus actividades y/o yardas
        /// </summary>
        public static short TOLERANCIA_ACTIVIDAD_EDIFICIO
        {
            get
            {
                var query = new EntityManagerServer<PARAMETRO>().GetData().Where(w => w.ID_CENTRO == GlobalVar.gCentro && w.ID_CLAVE.Contains("TOLERANCIA_ACTIVIDAD_EDIFICIO"));
                return (short)(query.Any() ? query.Count() > 1 ? query.Where(w => w.ID_CENTRO == GlobalVar.gCentro).FirstOrDefault().VALOR_NUM : query.FirstOrDefault().VALOR_NUM : 0);
            }
        }

        /// <summary>
        /// Indica la tolerancia eh hrs para dar salida a los internos con traslado
        /// </summary>
        public static short TOLERANCIA_TRASLADO_EDIFICIO
        {
            get
            {
                var query = new EntityManagerServer<PARAMETRO>().GetData().Where(w => w.ID_CENTRO == GlobalVar.gCentro && w.ID_CLAVE.Contains("TOLERANCIA_TRASLADO_EDIFICIO"));
                return (short)(query.Any() ? query.Count() > 1 ? query.Where(w => w.ID_CENTRO == GlobalVar.gCentro).FirstOrDefault().VALOR_NUM : query.FirstOrDefault().VALOR_NUM : 0);
            }
        }

        /// <summary>
        /// Obtiene los IDs de areas tecnicas a los cuales tiene acceso el coord. de areas tecnicas
        /// </summary>
        public static string[] COORD_AT_AREAS_TECNICAS
        {
            get
            {
                var query = new EntityManagerServer<PARAMETRO>().GetData().Where(w => w.ID_CENTRO == 0 && w.ID_CLAVE.Contains("COORD_AT_AREAS_TECNICAS"));
                return query.Any() ? query.FirstOrDefault().VALOR.Split(',') : new string[] { string.Empty };
            }
        }

        /// <summary>
        /// Obtiene los IDs de areas tecnicas a los cuales tiene acceso el coord. de criminodiagnostico
        /// </summary>
        public static string[] COORD_CRIMDIAG_AREAS_TECNICAS
        {
            get
            {
                var query = new EntityManagerServer<PARAMETRO>().GetData().Where(w => w.ID_CENTRO == 0 && w.ID_CLAVE.Contains("COORD_CRIMDIAG_AREAS_TECNICAS"));
                return query.Any() ? query.FirstOrDefault().VALOR.Split(',') : new string[] { string.Empty };
            }
        }

        /// <summary>
        /// Obtiene los IDs de areas tecnicas a los cuales tiene acceso el coord. educativo
        /// </summary>
        public static string[] COORD_EDUCATIVO_AREAS_TECNICAS
        {
            get
            {
                var query = new EntityManagerServer<PARAMETRO>().GetData().Where(w => w.ID_CENTRO == 0 && w.ID_CLAVE.Contains("COORD_EDUCATIVO_AREAS_TECNICAS"));
                return query.Any() ? query.FirstOrDefault().VALOR.Split(',') : new string[] { string.Empty };
            }
        }

        /// <summary>
        /// Obtiene los IDs de areas tecnicas a los cuales tiene acceso el coord. medico
        /// </summary>
        public static string[] COORD_MEDICO_AREAS_TECNICAS
        {
            get
            {
                var query = new EntityManagerServer<PARAMETRO>().GetData().Where(w => w.ID_CENTRO == 0 && w.ID_CLAVE.Contains("COORD_MEDICO_AREAS_TECNICAS"));
                return query.Any() ? query.FirstOrDefault().VALOR.Split(',') : new string[] { string.Empty };
            }
        }

        /// <summary>
        /// Obtiene los IDs de areas tecnicas a los cuales tiene acceso el coord. psicologia
        /// </summary>
        public static string[] COORD_PSIC_AREAS_TECNICAS
        {
            get
            {
                var query = new EntityManagerServer<PARAMETRO>().GetData().Where(w => w.ID_CENTRO == 0 && w.ID_CLAVE.Contains("COORD_PSIC_AREAS_TECNICAS"));
                return query.Any() ? query.FirstOrDefault().VALOR.Split(',') : new string[] { string.Empty };
            }
        }

        /// <summary>
        /// Obtiene el password con el que se crearan los usuarios de bd de oracle
        /// </summary>
        public static string PASSWORD_USUARIO_BD
        {
            get
            {
                var query = new EntityManagerServer<PARAMETRO>().GetData().Where(w => w.ID_CENTRO == GlobalVar.gCentro && w.ID_CLAVE.Contains("PASSWORD_USUARIO_BD"));
                return query.Any() ? query.FirstOrDefault().VALOR : string.Empty;
            }
        }

        /// <summary>
        /// Obtiene la cantidad maxima de dias permisibles para que se reagende una atención médica.
        /// </summary>
        public static short MAX_DIAS_ATENCION_MEDICA
        {
            get
            {
                var query = new EntityManagerServer<PARAMETRO>().GetData().Where(w => w.ID_CENTRO == GlobalVar.gCentro && w.ID_CLAVE.Contains("MAX_DIAS_ATENCION_MEDICA"));
                return (short)(query.Any() ? query.Count() > 1 ? query.Where(w => w.ID_CENTRO == GlobalVar.gCentro).FirstOrDefault().VALOR_NUM.Value : query.FirstOrDefault().VALOR_NUM.Value : 5);
            }
        }

        /// <summary>
        /// Obtiene el ID de los CANCELADOS de ATENCION_CITA_ESTATUS
        /// </summary>
        public static string AT_CITA_ESTATUS_CANCELADO
        {
            get
            {
                var query = new EntityManagerServer<PARAMETRO>().GetData().Where(w => w.ID_CENTRO == 0 && w.ID_CLAVE.Contains("AT_CITA_ESTATUS_CANCELADO"));
                return query.Any() ? query.Count() > 1 ? query.Where(w => w.ID_CENTRO == 0).FirstOrDefault().VALOR.ToString() : query.FirstOrDefault().VALOR.ToString() : string.Empty;
            }
        }

        /// <summary>
        /// Obtiene el ID de los CONCLUIDOS de ATENCION_CITA_ESTATUS
        /// </summary>
        public static string AT_CITA_ESTATUS_CONCLUIDO
        {
            get
            {
                var query = new EntityManagerServer<PARAMETRO>().GetData().Where(w => w.ID_CENTRO == 0 && w.ID_CLAVE.Contains("AT_CITA_ESTATUS_CONCLUIDO"));
                return query.Any() ? query.Count() > 1 ? query.Where(w => w.ID_CENTRO == 0).FirstOrDefault().VALOR.ToString() : query.FirstOrDefault().VALOR.ToString() : string.Empty;
            }
        }

        /// <summary>
        /// Obtiene el ID de los SIN ATENDER de ATENCION_CITA_ESTATUS
        /// </summary>
        public static string AT_CITA_ESTATUS_SIN_ATENDER
        {
            get
            {
                var query = new EntityManagerServer<PARAMETRO>().GetData().Where(w => w.ID_CENTRO == 0 && w.ID_CLAVE.Contains("AT_CITA_ESTATUS_SIN_ATENDER"));
                return query.Any() ? query.Count() > 1 ? query.Where(w => w.ID_CENTRO == 0).FirstOrDefault().VALOR.ToString() : query.FirstOrDefault().VALOR.ToString() : string.Empty;
            }
        }

        /// <summary>
        /// Obtiene el ID de los ATENDIENDO de ATENCION_CITA_ESTATUS
        /// </summary>
        public static string AT_CITA_ESTATUS_ATENDIENDO
        {
            get
            {
                var query = new EntityManagerServer<PARAMETRO>().GetData().Where(w => w.ID_CENTRO == 0 && w.ID_CLAVE.Contains("AT_CITA_ESTATUS_ATENDIENDO"));
                return query.Any() ? query.Count() > 1 ? query.Where(w => w.ID_CENTRO == 0).FirstOrDefault().VALOR.ToString() : query.FirstOrDefault().VALOR.ToString() : string.Empty;
            }
        }

        /// <summary>
        /// Obtiene la plantilla en formato .docx
        /// </summary>
        public static byte[] PLANTILLA_ATENCION_INTERNO
        {
            get
            {
                var query = new EntityManagerServer<PARAMETRO>().GetData().Where(w => w.ID_CENTRO == GlobalVar.gCentro && w.ID_CLAVE.Contains("PLANTILLA_ATENCION_INTERNO"));
                return query.Any() ? query.Count() > 1 ? query.Where(w => w.ID_CENTRO == GlobalVar.gCentro).FirstOrDefault().CONTENIDO : query.FirstOrDefault().CONTENIDO : null;
            }
        }

        /// <summary>
        /// Indica el area para visitas legales, por default es 85
        /// </summary>
        public static short ID_AREA_VISITA_LEGAL
        {
            get
            {
                var query = new EntityManagerServer<PARAMETRO>().GetData().Where(w => w.ID_CENTRO == GlobalVar.gCentro && w.ID_CLAVE.Contains("ID_AREA_VISITA_LEGAL"));
                return (short)(query.Any() ? query.Count() > 1 ? query.Where(w => w.ID_CENTRO == GlobalVar.gCentro).FirstOrDefault().VALOR_NUM : query.FirstOrDefault().VALOR_NUM : 0);
            }
        }



        /// <summary>
        /// Indica el area para visitas legales donde sea un abogado
        /// </summary>
        public static short UBICACION_VISITA_ABOGADO
        {
            get
            {
                var query = new EntityManagerServer<PARAMETRO>().GetData().Where(w => w.ID_CENTRO == 0 && w.ID_CLAVE.Contains("UBICACION_VISITA_ABOGADO"));
                return (short)(query.Any() ? query.Count() > 1 ? query.Where(w => w.ID_CENTRO == 0).FirstOrDefault().VALOR_NUM : query.FirstOrDefault().VALOR_NUM : 0);
            }
        }



        /// <summary>
        /// Indica el area para visitas legales donde sea un actuario
        /// </summary>
        public static short UBICACION_VISITA_ACTUARIO
        {
            get
            {
                var query = new EntityManagerServer<PARAMETRO>().GetData().Where(w => w.ID_CENTRO == 0 && w.ID_CLAVE.Contains("UBICACION_VISITA_ACTUARIO"));
                return (short)(query.Any() ? query.Count() > 1 ? query.Where(w => w.ID_CENTRO == 0).FirstOrDefault().VALOR_NUM : query.FirstOrDefault().VALOR_NUM : 0);
            }
        }

        public static string SUBDIRECTOR_CENTRO
        {
            get
            {
                var query = new EntityManagerServer<PARAMETRO>().GetData().Where(w => w.ID_CENTRO == GlobalVar.gCentro && w.ID_CLAVE.Contains("SUBDIRECTOR_CENTRO"));
                return query.Any() ? query.Count() > 1 ? query.Where(w => w.ID_CENTRO == 0).FirstOrDefault().VALOR.ToString() : query.FirstOrDefault().VALOR.ToString() : string.Empty;
            }
        }

        /// <summary>
        /// Indica el numero de pases cuando un abogado va por una causa penal
        /// </summary>
        public static short NO_PASES_CP
        {
            get
            {
                var query = new EntityManagerServer<PARAMETRO>().GetData().Where(w => w.ID_CENTRO == 0 && w.ID_CLAVE.Contains("NO_PASES_CP"));
                return (short)(query.Any() ? query.Count() > 1 ? query.Where(w => w.ID_CENTRO == 0).FirstOrDefault().VALOR_NUM : query.FirstOrDefault().VALOR_NUM : 0);
            }
        }

        /// <summary>
        /// Indica el numero de pases cuando un abogado va como administrativo
        /// </summary>
        public static short NO_PASES_ADMIN
        {
            get
            {
                var query = new EntityManagerServer<PARAMETRO>().GetData().Where(w => w.ID_CENTRO == 0 && w.ID_CLAVE.Contains("NO_PASES_ADMIN"));
                return (short)(query.Any() ? query.Count() > 1 ? query.Where(w => w.ID_CENTRO == 0).FirstOrDefault().VALOR_NUM : query.FirstOrDefault().VALOR_NUM : 0);
            }
        }
        /// <summary>
        /// Indica limite de hojas a escanear para guardar en base de datos
        /// </summary>
        public static short LIMITES_HOJAS_ESCANER
        {
            get
            {
                var query = new EntityManagerServer<PARAMETRO>().GetData().Where(w => w.ID_CENTRO == 0 && w.ID_CLAVE.Contains("LIMITES_HOJAS_ESCANER"));
                return (short)(query.Any() ? query.Count() > 1 ? query.Where(w => w.ID_CENTRO == 0).FirstOrDefault().VALOR_NUM : query.FirstOrDefault().VALOR_NUM : 0);
            }
        }


        /// <summary>
        /// Obtiene el id de ubicacion para estancia de los imputados recibidos por traslado
        /// </summary>
        public static short UB_RECEPCION_TRASLADO
        {
            get
            {
                var query = new EntityManagerServer<PARAMETRO>().GetData().Where(w => w.ID_CENTRO == GlobalVar.gCentro && w.ID_CLAVE.Contains("UB_RECEPCION_TRASLADO"));
                return (short)(query.Any() ? query.Count() > 1 ? query.Where(w => w.ID_CENTRO == GlobalVar.gCentro).FirstOrDefault().VALOR_NUM : query.FirstOrDefault().VALOR_NUM : 0);
            }
        }

        /// <summary>
        /// Indica la edad de un interno de tercera edad, valor default: [60]
        /// </summary>
        public static short ID_TERCERA_EDAD
        {
            get
            {
                var query = new EntityManagerServer<PARAMETRO>().GetData().Where(w => (w.ID_CENTRO == 0 || w.ID_CENTRO == 4) && w.ID_CLAVE.Contains("ID_TERCERA_EDAD"));
                return query.Any() ? query.Count() > 1 ? Convert.ToInt16(query.Where(w => w.ID_CENTRO == 4).FirstOrDefault().VALOR) : Convert.ToInt16(query.FirstOrDefault().VALOR) : (short)60;
            }
        }

        /// <summary>
        /// Indica el nombre del Sub Secretario de Seguridad Publica del Estado
        /// </summary>
        public static string SUB_SECRETARIO_SSP
        {
            get
            {
                var query = new EntityManagerServer<PARAMETRO>().GetData().Where(w => w.ID_CENTRO == 0 && w.ID_CLAVE.Contains("SUB_SECRETARIO_SSP"));
                return query.Any() ? query.Count() > 1 ? query.Where(w => w.ID_CENTRO == 0).FirstOrDefault().VALOR.ToString() : query.FirstOrDefault().VALOR.ToString() : string.Empty;
            }
        }

        /// <summary>
        /// Indica el nombre del Coordinador Estatal de Analisis de la unidad de Investigacion Penitenciaria
        /// </summary>
        public static string COORD_UNIDAD_INVESTIGACION
        {
            get
            {
                var query = new EntityManagerServer<PARAMETRO>().GetData().Where(w => w.ID_CENTRO == 0 && w.ID_CLAVE.Contains("COORD_UNIDAD_INVESTIGACION"));
                return query.Any() ? query.Count() > 1 ? query.Where(w => w.ID_CENTRO == 0).FirstOrDefault().VALOR.ToString() : query.FirstOrDefault().VALOR.ToString() : string.Empty;
            }
        }

        /// <summary>
        /// Indica los tipos de visitantes que se muestran en el reporte de control de visitantes
        /// </summary>
        public static string CONTROL_VISITANTES_REPORTE
        {
            get
            {
                var query = new EntityManagerServer<PARAMETRO>().GetData().Where(w => w.ID_CENTRO == 0 && w.ID_CLAVE.Contains("CONTROL_VISITANTES_REPORTE"));
                return query.Any() ? query.Count() > 1 ? query.Where(w => w.ID_CENTRO == 0).FirstOrDefault().VALOR.ToString() : query.FirstOrDefault().VALOR.ToString() : string.Empty;
            }
        }

        /// <summary>
        /// Indica los tipos de visitantes que se muestran en el reporte de control de visitantes
        /// </summary>
        public static int ID_CAUSA_PENAL_ACTIVA
        {
            get
            {
                var query = new EntityManagerServer<PARAMETRO>().GetData().Where(w => w.ID_CENTRO == 0 && w.ID_CLAVE.Contains("ID_CAUSA_PENAL_ACTIVA"));
                return query.Any() ? query.Count() > 1 ? Convert.ToInt32(query.Where(w => w.ID_CENTRO == 0).FirstOrDefault().VALOR) : Convert.ToInt32(query.FirstOrDefault().VALOR) : 1;
            }
        }

        /// <summary>
        /// Indica los tipos de visitantes que se muestran en el reporte de control de visitantes
        /// </summary>
        public static string COMANDANTE_ESTATAL_CENTROS
        {
            get
            {
                var query = new EntityManagerServer<PARAMETRO>().GetData().Where(w => w.ID_CENTRO == 0 && w.ID_CLAVE.Contains("COMANDANTE_ESTATAL_CENTROS"));
                return query.Any() ? query.Count() > 1 ? query.Where(w => w.ID_CENTRO == 0).FirstOrDefault().VALOR.ToString() : query.FirstOrDefault().VALOR.ToString() : string.Empty;
            }
        }

        /// <summary>
        /// Indica el intervalo de tiempo para la validacion de la session
        /// </summary>
        public static int TIEMPO_VALIDA_SESION
        {
            get
            {
                var query = new EntityManagerServer<PARAMETRO>().GetData().Where(w => (w.ID_CENTRO == 0 || w.ID_CENTRO == GlobalVar.gCentro) && w.ID_CLAVE.Contains("TIEMPO_VALIDA_SESION"));
                return query.Any() ? query.Count() > 1 ? Convert.ToInt32(query.Where(w => w.ID_CENTRO == GlobalVar.gCentro).FirstOrDefault().VALOR) : Convert.ToInt32(query.FirstOrDefault().VALOR) : 600000;
            }
        }

        /// <summary>
        /// Obtiene los IDs de los formatos de imagenes del catalogo FORMATO_DOCUMENTO
        /// </summary>
        public static string[] ID_FORMATO_IMAGENES
        {
            get
            {
                var query = new EntityManagerServer<PARAMETRO>().GetData().Where(w => w.ID_CENTRO == 0 && w.ID_CLAVE.Contains("ID_FORMATO_IMAGENES"));
                return query.Any() ? query.FirstOrDefault().VALOR.Split(',') : new string[] { string.Empty };
            }
        }

        /// <summary>
        /// Obtienes los IDs de los formatos de documentos del catalogo FORMATO_DOCUMENTO
        /// </summary>
        public static string[] ID_FORMATO_DOCUMENTOS
        {
            get
            {
                var query = new EntityManagerServer<PARAMETRO>().GetData().Where(w => w.ID_CENTRO == 0 && w.ID_CLAVE.Contains("ID_FORMATO_DOCUMENTOS"));
                return query.Any() ? query.FirstOrDefault().VALOR.Split(',') : new string[] { string.Empty };
            }
        }


        /// <summary>
        /// Obtiene los ID de los empleados que se refieren a los custodios.
        /// </summary>
        public static string[] ID_TIPO_EMPLEADO_CUSTODIO
        {
            get
            {
                var query = new EntityManagerServer<PARAMETRO>().GetData().Where(w => w.ID_CENTRO == 0 && w.ID_CLAVE.Contains("ID_TIPO_EMPLEADO_CUSTODIO"));
                return query.Any() ? query.FirstOrDefault().VALOR.Split(',') : new string[] { string.Empty };
            }
        }

        /// <summary>
        /// Obtiene nombre completo del organo administrativo
        /// </summary>
        public static string ORGANO_ADMINISTRATIVO
        {
            get
            {
                var query = new EntityManagerServer<PARAMETRO>().GetData().Where(w => w.ID_CENTRO == 0 && w.ID_CLAVE == "ORGANO_ADMINISTRATIVO");
                return query.Any() ? query.FirstOrDefault().VALOR : string.Empty;
            }
        }
        /// <summary>
        /// Nombre de el director de ejecucion de penas y medidas judiciales
        /// </summary>
        public static string DIRECTOR_EJECUCION_PENAS
        {
            get
            {
                var query = new EntityManagerServer<PARAMETRO>().GetData().Where(w => w.ID_CENTRO == 0 && w.ID_CLAVE == "DIRECTOR_EJECUCION_PENAS");
                return query.Any() ? query.FirstOrDefault().VALOR : string.Empty;
            }
        }

        /// <summary>
        /// Nombre de el director de programas a nivel estatal
        /// </summary>
        public static string DIR_PROGRAMAS_ESTATAL
        {
            get
            {
                var query = new EntityManagerServer<PARAMETRO>().GetData().Where(w => w.ID_CENTRO == 0 && w.ID_CLAVE == "DIR_PROGRAMAS_ESTATAL");
                return query.Any() ? query.FirstOrDefault().VALOR : string.Empty;
            }
        }

        /// <summary>
        /// Nombre de el jefe de programas a nivel estatal
        /// </summary>
        public static string JEFE_PROGRAMAS_ESTATAL
        {
            get
            {
                var query = new EntityManagerServer<PARAMETRO>().GetData().Where(w => w.ID_CENTRO == 0 && w.ID_CLAVE == "JEFE_PROGRAMAS_ESTATAL");
                return query.Any() ? query.FirstOrDefault().VALOR : string.Empty;
            }
        }

        /// <summary>
        /// Nombre de el coordinador medico a nivel estatal
        /// </summary>
        public static string COORD_MEDICO_ESTATAL
        {
            get
            {
                var query = new EntityManagerServer<PARAMETRO>().GetData().Where(w => w.ID_CENTRO == 0 && w.ID_CLAVE == "COORD_MEDICO_ESTATAL");
                return query.Any() ? query.FirstOrDefault().VALOR : string.Empty;
            }
        }


        public static string RESPONSABLE_LIBERADOS
        {
            get
            {
                var query = new EntityManagerServer<PARAMETRO>().GetData().Where(w => w.ID_CENTRO == 0 && w.ID_CLAVE == "RESPONSABLE_LIBERADOS");
                return query.Any() ? query.FirstOrDefault().VALOR : string.Empty;
            }
        }

        public static string DIRECCION_LIBERADOS
        {
            get
            {
                var query = new EntityManagerServer<PARAMETRO>().GetData().Where(w => w.ID_CENTRO == 0 && w.ID_CLAVE == "DIRECCION_LIBERADOS");
                return query.Any() ? query.FirstOrDefault().VALOR : string.Empty;
            }
        }

        public static string JEFE_EVALUACION
        {
            get
            {
                var query = new EntityManagerServer<PARAMETRO>().GetData().Where(w => w.ID_CENTRO == 0 && w.ID_CLAVE == "JEFE_EVALUACION");
                return query.Any() ? query.FirstOrDefault().VALOR : string.Empty;
            }
        }

        public static string JEFE_ATENCION_SEGUIMIENTO
        {
            get
            {
                var query = new EntityManagerServer<PARAMETRO>().GetData().Where(w => w.ID_CENTRO == 0 && w.ID_CLAVE == "JEFE_ATENCION_SEGUIMIENTO");
                return query.Any() ? query.FirstOrDefault().VALOR : string.Empty;
            }
        }

    }
}
