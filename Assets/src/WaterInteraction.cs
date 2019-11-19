using System;
using UnityEngine;

namespace src
{
    public class WaterInteraction : MonoBehaviour
    {
        public int SpringNum { get; set; }
        public static float K = 0.01f;
        
        private WaterSpring[] _water;
        private EdgeCollider2D _edgeCollider2D;
        public void Init(WaterSpring[] water, int springNum)
        {
            SpringNum = springNum;
            _water = water;

            _edgeCollider2D = gameObject.AddComponent<EdgeCollider2D>();
            Vector2[] points = new Vector2[SpringNum];
            for (int i = 0; i < SpringNum; i++)
            {
                points[i] = water[i].Position;
            }

            _edgeCollider2D.points = points;
            _edgeCollider2D.isTrigger = true;
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            var rigidbody = other.GetComponent<Rigidbody2D>();
            if (rigidbody != null)
            {
                var transformPosition = rigidbody.transform.position;
                float start = transformPosition.x - rigidbody.transform.localScale.x / 2;
                float end = transformPosition.x + rigidbody.transform.localScale.x / 2;
                
                
                int startI = Math.Max(0, (int)((start - _water[0].Position.x) * 10));
                int endI = Math.Min(_water.Length-1, (int)((end - _water[0].Position.x) * 10));

                float velocityY = rigidbody.velocity.y * rigidbody.mass / 20f;
                for (int j = startI; j < endI; j++)
                {
                    SetVelocity(j, velocityY);
                }

//                Debug.Log(rigidbody);
            }
        }

        public void SetVelocity(int i, float velocity)
        {
            _water[i].VelocityY = velocity;
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