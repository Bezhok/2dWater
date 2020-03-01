using System;
using System.Data;
using System.Drawing;
using src.FactoryPattern;
using UnityEngine;

namespace src.Liquids
{
    public class WaterData : IData
    {
        public WaterData(float width, float height, float K)
        {
            Width = width;
            Height = height;
            
            Step = 0.1f;
            this.K = K;
            Bottom = -height/2;
            Top = height/2;
            Left = -width/2;

            SpringNum = (int) Math.Ceiling(width / Step) + 1;
            WaterSprings = new WaterSpring[SpringNum];
        }
        
        public float Width { get; set; }
        public float Height { get; set; }
        public float Top { get; set; }
        public float Bottom { get; set; }
        public float Left { get; set; }
        public float Step { get; set; }
        public float K;
        public int SpringNum { get; set; }
        public WaterSpring[] WaterSprings { get; set; }
    }
}