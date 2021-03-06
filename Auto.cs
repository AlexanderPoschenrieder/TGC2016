﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TgcViewer;
using TgcViewer.Utils.TgcSceneLoader;
using Microsoft.DirectX;
using TgcViewer.Utils.Input;
using Microsoft.DirectX.DirectInput;

namespace AlumnoEjemplos.MiGrupo
{
    public class Auto
    {
        #region Constantes

        const float CONSTANTEAVANZAR = 600f;
        const float CONSTANTEFRENAR = 800f;
        const float CONSTANTEMARCHAATRAS = 400f;
        const float ROZAMIENTOCOEF = 200f;

        #endregion

        #region Atributos

        public float velocidad;
        float velocidadMaxima = -2000f;
        float velocidadMinima = 3000f;
        public float rotacion;
        public float elapsedTime;
        List<TgcViewer.Utils.TgcSceneLoader.TgcMesh> ruedas;
        public TgcMesh meshAuto;
        float direccion;

        #endregion

        #region Constructor
        
        public Auto(TgcMesh mesh)
        {
            meshAuto = mesh;
        }

        public Auto(float rot, TgcMesh auto, List<TgcViewer.Utils.TgcSceneLoader.TgcMesh> unasRuedas = null)
        {
            this.rotacion = rot;
            this.ruedas = unasRuedas;
            meshAuto = auto;
        }

        #endregion

        #region Métodos
        public void Mover()
        {
            ///////////////INPUT//////////////////
            //conviene deshabilitar ambas camaras para que no haya interferencia
            TgcD3dInput input = GuiController.Instance.D3dInput;
            if (input.keyDown(Key.Left) || input.keyDown(Key.A))
            {
                Rotar(-1);
            }
            else if (input.keyDown(Key.Right) || input.keyDown(Key.D))
            {
                Rotar(1);
            }

            if (input.keyDown(Key.Up) || input.keyDown(Key.W))
            {
                Acelerar(CONSTANTEAVANZAR);
            }
            else if (input.keyDown(Key.Down) || input.keyDown(Key.S))
            {
                Retroceder();
            }
            else
            {
                Acelerar(0);    
            }

            MoverMesh();
        }

        private void MoverMesh()
        {
            this.meshAuto.Rotation = new Vector3(0f, this.rotacion, 0f);
            meshAuto.moveOrientedY(-this.velocidad * elapsedTime);
        }


        public void Retroceder()
        {
            if (velocidad > 0) Frenar();
            if (velocidad < 0) MarchaAtras();
        }

        public void Rotar(float unaDireccion)
        {
            if (velocidad == 0)
            {
                return;
            }

            direccion = unaDireccion;
            rotacion += (elapsedTime * direccion * (velocidad / 1000)); //direccion puede ser 1 o -1, 1 es derecha y -1 izquierda
            AjustarRotacion();
            
        }

        public float RotarRueda(int i)
        {
            if (i == 0 || i == 2)
            {
                return 0.5f * direccion;
            }
            return 0;
        }
        //Metodos Internos
        //De Velocidad

        private void Acelerar(float aumento)
        {
            velocidad += (aumento - Rozamiento()) * elapsedTime;
            AjustarVelocidad();
        }

        public void AjustarVelocidad()
        {
            if (velocidad > velocidadMinima) velocidad = velocidadMinima;
            if (velocidad < velocidadMaxima) velocidad = velocidadMaxima;
        }

        public void EstablecerVelocidadMáximaEn(float velMaxima)
        {
            velocidadMinima = velMaxima;
        }

        public float Rozamiento()
        {
            return ROZAMIENTOCOEF * Math.Sign(velocidad);
        }

        private void Frenar()
        {
            Acelerar(-CONSTANTEFRENAR);
        }

        public void FrenoDeMano()
        {
            Acelerar(-CONSTANTEFRENAR * 2.5f);
        }

        public void MarchaAtras()
        {
            Acelerar(-CONSTANTEMARCHAATRAS);
        }


        //DeRotacion
        private void AjustarRotacion()
        {
            while (rotacion > 360)
            {
                rotacion -= 360;
            }
        }

        //para que el auto se quede quieto cuando perdio el jugador
        public void Estatico()
        {
            velocidadMinima = 0f;
            velocidadMaxima = 0f;
            AjustarVelocidad();
        }

        public float GetRotacion()
        {
            return this.rotacion;
        }

        #endregion
    }
}
