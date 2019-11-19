namespace src
{
    public class WaterData
    {
        public float Step { get; set; }
        public float BaseHeight { get; set; }
        public float Bottom { get; set; }
        public float K { get; set; }
        public int SpringNum { get; set; }
        public WaterMesh Mesh { get; set; }
        public WaterSpring[] WaterSprings { get; set; }
        public WaterInteraction Phys { get; set; }
    }
}