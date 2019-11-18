namespace src
{
    public class WaterInteraction
    {
        public int SpringNum { get; set; }
        public static float K = 0.01f;
        
        private WaterSpring[] _water;
        public WaterInteraction(WaterSpring[] water, int springNum)
        {
            SpringNum = springNum;
            _water = water;
        }

        public void UpdatePhys()
        {
            for (int i = 0; i < SpringNum; i++)
            {
                _water[i].Update(0.5f);
            }
            
            for (int i = 0; i < SpringNum; i++)
            {
                float koeff = 0.06f;
                if (i > 0)
                {
                    float leftDelta = koeff * (_water[i].Position.y - _water[i - 1].Position.y);
                    _water[i - 1].VelocityY += leftDelta;
                }

                if (i < SpringNum - 1)
                {
                    float rightDelta = koeff * (_water[i].Position.y - _water[i + 1].Position.y);
                    _water[i + 1].VelocityY += rightDelta;
                }
            }
        }

        public void UpdatePhysExperimental()
        {
            for (int i = 0; i < SpringNum; i++)
            {
                _water[i].Update(0.5f);
            }

            float[] leftDeltas = new float[SpringNum];
            float[] rightDeltas = new float[SpringNum];

            for (int k = 0; k < 6; k++)
            {
                for (int i = 0; i < SpringNum; i++)
                {
                    float koeff = 0.001f;
                    if (i > 0)
                    {
                        leftDeltas[i] = koeff * (_water[i].Position.y - _water[i - 1].Position.y);
                        _water[i - 1].VelocityY += leftDeltas[i];
                    }

                    if (i < SpringNum - 1)
                    {
                        rightDeltas[i] = koeff * (_water[i].Position.y - _water[i + 1].Position.y);
                        _water[i + 1].VelocityY += rightDeltas[i];
                    }
                }

                for (int i = 0; i < SpringNum; i++)
                {
                    if (i > 0)
                    {
                        _water[i-1].Position += new UnityEngine.Vector3(0, leftDeltas[i]);
                    }

                    if (i < SpringNum - 1)
                    {
                        _water[i+1].Position += new UnityEngine.Vector3(0, leftDeltas[i]);
                    }

                }
            }
        }
    }
}