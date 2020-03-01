using System;
using src.FactoryPattern;

namespace src.Liquids
{
    public class WaterData : IData
    {
        public float K;

        public WaterData(float width, float height, float k)
        {
            Width = width;
            Height = height;

            Step = 0.1f;
            K = k;
            Bottom = -height / 2;
            Top = height / 2;
            Left = -width / 2;

            SpringNum = (int) Math.Ceiling(width / Step) + 1;
            WaterSprings = new WaterSpring[SpringNum];
        }

        public float Width { get; set; }
        public float Height { get; set; }
        public float Top { get; set; }
        public float Bottom { get; set; }
        public float Left { get; set; }
        public float Step { get; set; }
        public int SpringNum { get; set; }
        public WaterSpring[] WaterSprings { get; set; }
    }
}