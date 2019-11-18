using UnityEngine;

namespace src
{
    public class WaterSpring
    {
        private Vector3 _position;

        public Vector3 Position
        {
            get => _position;
            set => _position = value;
        }

        private float _velocityY;

        public float VelocityY
        {
            get => _velocityY;
            set => _velocityY = value;
        }

        public void Update(float delta)
        {
            float mass = 1.0f;
            float accelerationY = -WaterManager.K*(_position.y - WaterManager.BaseHeight)/mass - _velocityY*0.05f;
            _position.y += delta * _velocityY;
            _velocityY += delta * accelerationY;
        }
    }
}