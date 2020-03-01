using UnityEngine;

namespace src.Liquids
{
    public class WaterSpring
    {
        public Vector3 position;
        public float VelocityY { get; set; }

        public void Update(float delta, float baseHeight, float k)
        {
            float mass = 1.0f;
            float accelerationY = -k * (position.y - baseHeight) / mass - VelocityY * 0.05f;
            position.y += delta * VelocityY;
            VelocityY += delta * accelerationY;
        }
    }
}