using UnityEngine;

namespace src
{
    public class WaterSpring
    {
        public Vector3 Position;
        public float VelocityY { get; set; }

        public void Update(float delta)
        {
            float mass = 1.0f;
            float accelerationY = -WaterInteraction.K*(Position.y - WaterManager.BaseHeight)/mass - VelocityY*0.05f;
            Position.y += delta * VelocityY;
            VelocityY += delta * accelerationY;
        }
    }
}