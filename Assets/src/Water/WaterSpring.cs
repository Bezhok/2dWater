using UnityEngine;

namespace src.Water
{
    public class WaterSpring
    {
        public Vector3 Position;
        public float VelocityY { get; set; }

        public void Update(float delta, float baseHeight, float K)
        {
            float mass = 1.0f;
            float accelerationY = -K * (Position.y - baseHeight) / mass - VelocityY * 0.05f;
            Position.y += delta * VelocityY;
            VelocityY += delta * accelerationY;
        }
    }
}