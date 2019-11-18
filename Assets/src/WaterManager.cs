using System;
using UnityEngine;

namespace src
{
    public class WaterManager : MonoBehaviour
    {
        private int scaller = 1;
        public static float BaseHeight = 0;
        public static float K = 0.01f;

        int _springNum = 176;
        private WaterMesh _waterMesh;
        private WaterSpring[] _water;
        private void Start()
        {
            _springNum *= scaller;
            
            float z = 0;

            var halfheight = Camera.main.orthographicSize;
            var halfWidth = halfheight * Screen.width/Screen.height;
            
            _water = new WaterSpring[_springNum];



            float step = 0.1f/scaller;//Screen.width / (float)verticiesCount;
            for (int i = 0; i < _springNum; i++)
            {
                _water[i] = new WaterSpring();
                var pos = new Vector3(step*i - halfWidth, BaseHeight, z);
                _water[i].Position = pos;
            }

            for (int i = 20; i < 30; i++)
            {
                SetVelocity(i, -2f*scaller);   
            }

            _waterMesh = gameObject.AddComponent<WaterMesh>();
            _waterMesh.Init(_water, _springNum, -5);
        }
        
        private void SetVelocity(int i, float velocity)
        {
            _water[i].VelocityY = velocity;
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.LeftControl))
            {
                for (int i = 20; i < 20 + 30 * scaller; i++)
                {
                    SetVelocity(i, -0.5f);
                }
            }

            if (Input.GetKeyDown(KeyCode.RightControl))
            {
                for (int i = 50; i < 50 + 30 * scaller; i++)
                {
                    SetVelocity(i, -1.52f);
                }
            }

            for (int i = 0; i < _springNum; i++)
            {
                _water[i].Update(0.5f);
            }

            float[] leftDeltas = new float[_springNum];
            float[] rightDeltas = new float[_springNum];

            for (int k = 0; k < 6; k++)
            {
                for (int i = 0; i < _springNum; i++)
                {
//                    float koeff = 0.001f;
                    float koeff = 0.01f;
                    if (i > 0)
                    {
                        leftDeltas[i] = koeff * (_water[i].Position.y - _water[i - 1].Position.y);
                        _water[i - 1].VelocityY += leftDeltas[i];
                    }

                    if (i < _springNum - 1)
                    {
                        rightDeltas[i] =  koeff* (_water[i].Position.y - _water[i + 1].Position.y);
                        _water[i + 1].VelocityY += rightDeltas[i];
                    }
                }

//                for (int i = 0; i < verticiesCount; i++)
//                {
//                    if (i > 0)
//                    {
//                        _water[i-1].Position += new Vector3(0, leftDeltas[i]);
//                    }
//
//                    if (i < verticiesCount - 1)
//                    {
//                        _water[i+1].Position += new Vector3(0, leftDeltas[i]);
//                    }
//
//                }
            }

            _waterMesh.UpdateMesh();
        }
        
    }
}