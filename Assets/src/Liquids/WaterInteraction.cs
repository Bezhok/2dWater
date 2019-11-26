using System;
using src.FactoryPattern;
using UnityEngine;

namespace src.Liquids
{
    public class WaterInteraction : MonoBehaviour, IInitializable
    {
        private WaterData _waterData;
        private EdgeCollider2D _edgeCollider2D;

        public void Init(IData waterData)
        {
            _waterData = waterData as WaterData;

            _edgeCollider2D = gameObject.AddComponent<EdgeCollider2D>();
            Vector2[] points = new Vector2[2];

            points[0] = _waterData.WaterSprings[0].Position;
            points[1] = _waterData.WaterSprings[_waterData.WaterSprings.Length - 1].Position;


            _edgeCollider2D.points = points;
            _edgeCollider2D.isTrigger = true;
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            var rigidbody = other.GetComponent<Rigidbody2D>();
            if (rigidbody != null)
            {
                var transformPosition = rigidbody.transform.position;
                var localScale = rigidbody.transform.localScale;
                float start = transformPosition.x - localScale.x / 2;
                float end = transformPosition.x + localScale.x / 2;
                
                float globalStartPosX = transform.TransformPoint(_waterData.WaterSprings[0].Position).x;
                float globalScaleX = _waterData.Step * transform.localScale.x;

                int startI = Math.Max(0, (int) ((start - globalStartPosX) / globalScaleX));
                int endI = Math.Min(_waterData.WaterSprings.Length - 1,
                    (int) ((end - globalStartPosX) / globalScaleX));

                // todo update collider points or interpolate
                float velocityY = rigidbody.velocity.y * rigidbody.mass / 20f;
                for (int j = startI; j < endI; j++)
                {
                    SetVelocity(j, velocityY);
                }
            }
        }

        public void SetVelocity(int i, float velocity)
        {
            _waterData.WaterSprings[i].VelocityY = velocity;
        }

        public void UpdatePhys()
        {
            float scaller = 3;
            for (int i = 0; i < _waterData.SpringNum; i++)
            {
                _waterData.WaterSprings[i].Update(0.5f * scaller, _waterData.BaseHeight, _waterData.K);
            }

            for (int i = 0; i < _waterData.SpringNum; i++)
            {
                float koeff = scaller * 0.09f / 10f / _waterData.Step;
                if (i > 0)
                {
                    float leftDelta = koeff * (_waterData.WaterSprings[i].Position.y -
                                               _waterData.WaterSprings[i - 1].Position.y);
                    _waterData.WaterSprings[i - 1].VelocityY += leftDelta;
                }

                if (i < _waterData.SpringNum - 1)
                {
                    float rightDelta = koeff * (_waterData.WaterSprings[i].Position.y -
                                                _waterData.WaterSprings[i + 1].Position.y);
                    _waterData.WaterSprings[i + 1].VelocityY += rightDelta;
                }
            }
        }

        public void UpdatePhysExperimental()
        {
            for (int i = 0; i < _waterData.SpringNum; i++)
            {
                _waterData.WaterSprings[i].Update(0.5f, _waterData.BaseHeight, _waterData.K);
            }

            float[] leftDeltas = new float[_waterData.SpringNum];
            float[] rightDeltas = new float[_waterData.SpringNum];

            for (int k = 0; k < 6; k++)
            {
                for (int i = 0; i < _waterData.SpringNum; i++)
                {
                    float koeff = 0.005f;
                    if (i > 0)
                    {
                        leftDeltas[i] = koeff * (_waterData.WaterSprings[i].Position.y -
                                                 _waterData.WaterSprings[i - 1].Position.y);
                        _waterData.WaterSprings[i - 1].VelocityY += leftDeltas[i];
                    }

                    if (i < _waterData.SpringNum - 1)
                    {
                        rightDeltas[i] = koeff * (_waterData.WaterSprings[i].Position.y -
                                                  _waterData.WaterSprings[i + 1].Position.y);
                        _waterData.WaterSprings[i + 1].VelocityY += rightDeltas[i];
                    }
                }

                for (int i = 0; i < _waterData.SpringNum; i++)
                {
                    if (i > 0)
                    {
                        _waterData.WaterSprings[i - 1].Position += new Vector3(0, leftDeltas[i]);
                    }

                    if (i < _waterData.SpringNum - 1)
                    {
                        _waterData.WaterSprings[i + 1].Position += new Vector3(0, leftDeltas[i]);
                    }
                }
            }
        }
    }
}