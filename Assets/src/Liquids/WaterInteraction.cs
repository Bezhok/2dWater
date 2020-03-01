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
            var transformPosition = objTransform.position;
            var localScale = objTransform.localScale;
            var start = transformPosition.x - localScale.x / 2;
            var end = transformPosition.x + localScale.x / 2;

            var globalStartPosX = _transform.TransformPoint(_waterData.WaterSprings[0].position).x;
            var globalScaleX = _waterData.Step * _transform.localScale.x;

            var startI = Math.Max(0, (int) ((start - globalStartPosX) / globalScaleX));
            var endI = Math.Min(_waterData.WaterSprings.Length - 1,
                (int) ((end - globalStartPosX) / globalScaleX));

            return new Vector2Int(startI, endI);
        }

        private void ApplyForce(Rigidbody2D otherRigidbody, Collider2D other)
        {
            var range = ObjPosToArrayIdxs(other.transform);
            // todo update collider points or interpolate
            var velocityY = otherRigidbody.velocity.y * otherRigidbody.mass / 20f;
            for (var j = range.x; j < range.y; j++) SetVelocity(j, velocityY);

            var rigidbodyVelocity = otherRigidbody.velocity;

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
            var rigidbodyVelocity = otherRigidbody.velocity;
            var x = rigidbodyVelocity.x / 2;
            var archimedesForce = 10.8f / otherRigidbody.mass * Time.deltaTime;
            var resistance = rigidbodyVelocity.y * 0.01f;

            var waterTop = _transform.position.y + _waterData.Top;
            var deadZoneLen = 0.05f;


            var rigidbodyPos = otherRigidbody.transform.position;
            var isAboveWaterLine = rigidbodyPos.y >= waterTop + deadZoneLen;
            var isInDeadZone = rigidbodyPos.y < waterTop + deadZoneLen &&
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
            for (var k = 0; k < 4; ++k)
            {
                UpdateSpringsPhys();
                InterpolateVelocity();
            }
        }

        private void UpdateSpringsPhys()
        {
            for (var i = 0; i < _waterData.SpringNum; i++)
                _waterData.WaterSprings[i].Update(0.5f, _waterData.Top, _waterData.K);
        }

        private void InterpolateVelocity()
        {
            for (var i = 0; i < _waterData.SpringNum; i++)
            {
                var coefficient = 0.009f / _waterData.Step;
                if (i > 0)
                {
                    var leftDelta = coefficient * (_waterData.WaterSprings[i].position.y -
                                                   _waterData.WaterSprings[i - 1].position.y);
                    _waterData.WaterSprings[i - 1].VelocityY += leftDelta;
                }

                if (i < _waterData.SpringNum - 1)
                {
                    var rightDelta = coefficient * (_waterData.WaterSprings[i].position.y -
                                                    _waterData.WaterSprings[i + 1].position.y);
                    _waterData.WaterSprings[i + 1].VelocityY += rightDelta;
                }
            }
        }
    }
}