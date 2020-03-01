﻿using System;
using src.FactoryPattern;
using UnityEngine;

namespace src.Liquids
{
    public class WaterInteraction : MonoBehaviour, IInitializable
    {
        private WaterData _waterData;
        private EdgeCollider2D _edgeCollider2D;
        private BoxCollider2D _rectCollider2D;

        public void Init(IData waterData)
        {
            _waterData = waterData as WaterData;

            _edgeCollider2D = gameObject.AddComponent<EdgeCollider2D>();
            _rectCollider2D = gameObject.AddComponent<BoxCollider2D>();
            Vector2[] points = new Vector2[2];

            points[0] = _waterData.WaterSprings[0].Position;
            points[1] = _waterData.WaterSprings[_waterData.WaterSprings.Length - 1].Position;


            _edgeCollider2D.points = points;
            _edgeCollider2D.isTrigger = true;

            _rectCollider2D.isTrigger = true;
            _rectCollider2D.size = new Vector2(_waterData.Width, _waterData.Height);
        }

        private Vector2Int ObjPosToArrayIdxs(Transform objTransform)
        {
            var transformPosition = objTransform.position;
            var localScale = objTransform.localScale;
            float start = transformPosition.x - localScale.x / 2;
            float end = transformPosition.x + localScale.x / 2;

            float globalStartPosX = transform.TransformPoint(_waterData.WaterSprings[0].Position).x;
            float globalScaleX = _waterData.Step * transform.localScale.x;

            int startI = Math.Max(0, (int) ((start - globalStartPosX) / globalScaleX));
            int endI = Math.Min(_waterData.WaterSprings.Length - 1,
                (int) ((end - globalStartPosX) / globalScaleX));
            
            return new Vector2Int(startI, endI);
        }

        private void ApplyForce(Rigidbody2D rigidbody, Collider2D other)
        {
            Vector2Int range = ObjPosToArrayIdxs(other.transform);
            // todo update collider points or interpolate
            float velocityY = rigidbody.velocity.y * rigidbody.mass / 20f;
            for (int j = range.x; j < range.y; j++)
            {
                SetVelocity(j, velocityY);
            }
            
            var rigidbodyVelocity = rigidbody.velocity;
            
            rigidbodyVelocity = new Vector2(rigidbodyVelocity.x, rigidbodyVelocity.y/1.18f);
            rigidbody.velocity = rigidbodyVelocity;
        }
        
        private void OnTriggerEnter2D(Collider2D other)
        {
            var rigidbody = other.GetComponent<Rigidbody2D>();
            if (rigidbody == null) return;
            if (!other.IsTouching(_edgeCollider2D)) return;

            ApplyForce(rigidbody, other);
        }

        private void ControlAscent(Rigidbody2D rigidbody)
        {
            var rigidbodyVelocity = rigidbody.velocity;
            var x = rigidbodyVelocity.x/2;
            float archimedesForce = 10.8f / rigidbody.mass * Time.deltaTime;
            float resistance = rigidbodyVelocity.y * 0.01f;

            float waterTop = transform.position.y + _waterData.Top;
            float deadZoneLen = 0.05f;

            
            var rigidbodyPos = rigidbody.transform.position;
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
                    rigidbody.Sleep();
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
                
            rigidbody.velocity = rigidbodyVelocity;
        }
        private void OnTriggerStay2D(Collider2D other)
        {
            var rigidbody = other.GetComponent<Rigidbody2D>();
            if (rigidbody == null) return;
            if (!other.IsTouching(_rectCollider2D)) return;
            
            ControlAscent(rigidbody);
        }

        public void SetVelocity(int i, float velocity)
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
            {
                _waterData.WaterSprings[i].Update(0.5f, _waterData.Top, _waterData.K);
            }
        }

        private void InterpolateVelocity()
        {
            for (int i = 0; i < _waterData.SpringNum; i++)
            {
                float koeff =  0.009f / _waterData.Step;
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
    }
}