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
            Step = 0.1f;
            this.K = K;
            Bottom = -height/2;
            BaseHeight = height/2;
            Start = -width/2;

            SpringNum = (int) Math.Ceiling(width / Step) + 1;
            WaterSprings = new WaterSpring[SpringNum];
        }
        public float BaseHeight { get; set; }
        public float Bottom { get; set; }
        public float Start { get; set; }
        public float Step { get; set; }
        public float K;// { get; set; }
        public int SpringNum { get; set; }
        public WaterSpring[] WaterSprings { get; set; }
    }
}