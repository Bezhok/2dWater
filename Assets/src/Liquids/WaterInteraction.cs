using System;
using src.FactoryPattern;
using UnityEngine;

namespace src.Liquids
{
    public class WaterInteraction : MonoBehaviour, IInitializable
    {
        private EdgeCollider2D _edgeCollider2D;
        private BoxCollider2D _rectCollider2D;
        private Transform _transform;
        private WaterData _waterData;

        public void Init(IData waterData)
        {
            _waterData = waterData as WaterData;
            _transform = transform;

            _edgeCollider2D = gameObject.AddComponent<EdgeCollider2D>();
            _rectCollider2D = gameObject.AddComponent<BoxCollider2D>();
            var points = new Vector2[2];

            points[0] = _waterData.WaterSprings[0].position;
            points[1] = _waterData.WaterSprings[_waterData.WaterSprings.Length - 1].position;


            _edgeCollider2D.points = points;
            _edgeCollider2D.isTrigger = true;

            _rectCollider2D.isTrigger = true;
            _rectCollider2D.size = new Vector2(_waterData.Width, _waterData.Height);
        }

        private Vector2Int ObjPosToArrayIdxs(Transform objTransform)
        {
            Vector3 transformPosition = objTransform.position;
            Vector3 localScale = objTransform.localScale;
            float start = transformPosition.x - localScale.x / 2;
            float end = transformPosition.x + localScale.x / 2;

            float globalStartPosX = _transform.TransformPoint(_waterData.WaterSprings[0].position).x;
            float globalScaleX = _waterData.Step * _transform.localScale.x;

            int startI = Math.Max(0, (int) ((start - globalStartPosX) / globalScaleX));
            int endI = Math.Min(_waterData.WaterSprings.Length - 1,
                (int) ((end - globalStartPosX) / globalScaleX));

            return new Vector2Int(startI, endI);
        }

        private void ApplyForce(Rigidbody2D otherRigidbody, Collider2D other)
        {
            Vector2Int range = ObjPosToArrayIdxs(other.transform);
            // todo update collider points or interpolate
            float velocityY = otherRigidbody.velocity.y * otherRigidbody.mass / 20f;
            for (int j = range.x; j < range.y; j++) SetVelocity(j, velocityY);

            Vector2 rigidbodyVelocity = otherRigidbody.velocity;

            rigidbodyVelocity = new Vector2(rigidbodyVelocity.x, rigidbodyVelocity.y / 1.18f);
            otherRigidbody.velocity = rigidbodyVelocity;
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            var otherRigidbody = other.GetComponent<Rigidbody2D>();
            if (otherRigidbody == null) return;
            if (!other.IsTouching(_edgeCollider2D)) return;

            ApplyForce(otherRigidbody, other);
        }

        private void ControlAscent(Rigidbody2D otherRigidbody)
        {
            Vector2 rigidbodyVelocity = otherRigidbody.velocity;
            float x = rigidbodyVelocity.x / 2;
            float archimedesForce = 10.8f / otherRigidbody.mass * Time.deltaTime;
            float resistance = rigidbodyVelocity.y * 0.01f;

            float waterTop = _transform.position.y + _waterData.Top;
            float deadZoneLen = 0.05f;


            Vector3 rigidbodyPos = otherRigidbody.transform.position;
            bool isAboveWaterLine = rigidbodyPos.y >= waterTop + deadZoneLen;
            bool isInDeadZone = rigidbodyPos.y < waterTop + deadZoneLen &&
                                rigidbodyPos.y > waterTop - deadZoneLen;
            //todo local to world position
            if (isAboveWaterLine)
            {
                if (Mathf.Abs(rigidbodyVelocity.y) > 0.18f)
                {
                    rigidbodyVelocity = new Vector2(x,
                        rigidbodyVelocity.y + archimedesForce / 4);
                    rigidbodyVelocity = new Vector2(x, rigidbodyVelocity.y / 1.05f);
                }
                else
                {
                    rigidbodyVelocity = new Vector2(x, 0);
                }
            }
            else if (isInDeadZone)
            {
                if (Mathf.Abs(rigidbodyVelocity.y) < 0.18f)
                {
                    rigidbodyVelocity = new Vector2(x, 0);
                    otherRigidbody.Sleep();
                }
                else
                {
                    rigidbodyVelocity = new Vector2(x,
                        rigidbodyVelocity.y + archimedesForce / 2);
                    rigidbodyVelocity = new Vector2(x, rigidbodyVelocity.y / 1.05f);
                }
            }
            else
            {
                rigidbodyVelocity = new Vector2(x, rigidbodyVelocity.y + archimedesForce - resistance);
            }

            otherRigidbody.velocity = rigidbodyVelocity;
        }

        private void OnTriggerStay2D(Collider2D other)
        {
            var otherRigidbody = other.GetComponent<Rigidbody2D>();
            if (otherRigidbody == null) return;
            if (!other.IsTouching(_rectCollider2D)) return;

            ControlAscent(otherRigidbody);
        }

        private void SetVelocity(int i, float velocity)
        {
            _waterData.WaterSprings[i].VelocityY = velocity;
        }

        public void UpdatePhys()
        {
            for (int k = 0; k < 4; ++k)
            {
                UpdateSpringsPhys();
                InterpolateVelocity();
            }
        }

        private void UpdateSpringsPhys()
        {
            for (int i = 0; i < _waterData.SpringNum; i++)
                _waterData.WaterSprings[i].Update(0.5f, _waterData.Top, _waterData.K);
        }

        private void InterpolateVelocity()
        {
            for (int i = 0; i < _waterData.SpringNum; i++)
            {
                float coefficient = 0.009f / _waterData.Step;
                if (i > 0)
                {
                    float leftDelta = coefficient * (_waterData.WaterSprings[i].position.y -
                                                     _waterData.WaterSprings[i - 1].position.y);
                    _waterData.WaterSprings[i - 1].VelocityY += leftDelta;
                }

                if (i < _waterData.SpringNum - 1)
                {
                    float rightDelta = coefficient * (_waterData.WaterSprings[i].position.y -
                                                      _waterData.WaterSprings[i + 1].position.y);
                    _waterData.WaterSprings[i + 1].VelocityY += rightDelta;
                }
            }
        }
    }
}