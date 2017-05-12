using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ControlPenales
{
    class cBiometricos
    {
        //FOTOS
        private byte[] fotoIzquerda;
        public byte[] FotoIzquerda
        {
            get { return fotoIzquerda; }
            set { fotoIzquerda = value; }
        }

        private byte[] fotoCentro;
        public byte[] FotoCentro
        {
            get { return fotoCentro; }
            set { fotoCentro = value; }
        }

        private byte[] fotoDerecha;
        public byte[] FotoDerecha
        {
            get { return fotoDerecha; }
            set { fotoDerecha = value; }
        }

        //HUELLAS
        private byte[] pulgarIzquierda;
        public byte[] PulgarIzquierda
        {
            get { return pulgarIzquierda; }
            set { pulgarIzquierda = value; }
        }

        private byte[] indiceIzquierda;
        public byte[] IndiceIzquierda
        {
            get { return indiceIzquierda; }
            set { indiceIzquierda = value; }
        }

        private byte[] medioIzquierda;
        public byte[] MedioIzquierda
        {
            get { return medioIzquierda; }
            set { medioIzquierda = value; }
        }

        private byte[] anularIzquierda;
        public byte[] AnularIzquierda
        {
            get { return anularIzquierda; }
            set { anularIzquierda = value; }
        }

        private byte[] meniqueIzquierda;
        public byte[] MeniqueIzquierda
        {
            get { return meniqueIzquierda; }
            set { meniqueIzquierda = value; }
        }

        private byte[] pulgarDerecha;
        public byte[] PulgarDerecha
        {
            get { return pulgarDerecha; }
            set { pulgarDerecha = value; }
        }

        private byte[] indiceDerecha;
        public byte[] IndiceDerecha
        {
            get { return indiceDerecha; }
            set { indiceDerecha = value; }
        }

        private byte[] medioDerecha;
        public byte[] MedioDerecha
        {
            get { return medioDerecha; }
            set { medioDerecha = value; }
        }

        private byte[] anularDerecha;
        public byte[] AnularDerecha
        {
            get { return anularDerecha; }
            set { anularDerecha = value; }
        }

        private byte[] meniqueDerecha;
        public byte[] MeniqueDerecha
        {
            get { return meniqueDerecha; }
            set { meniqueDerecha = value; }
        }

        //PRUEBAS
        private string temp;
        public string Temp
        {
            get { return temp; }
            set { temp = value; }
        }
    }
}
