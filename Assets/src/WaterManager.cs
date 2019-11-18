using System;
using UnityEngine;

namespace src
{
    public class WaterManager : MonoBehaviour
    {
        private int scaller = 1;
        public static float BaseHeight = 0;
        private WaterMesh _waterMesh;
        private WaterSpring[] _water;
        private WaterInteraction _phys;
        private void Start()
        {
            int _springNum = 176;
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
            
            _phys = new WaterInteraction(_water, _springNum);
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
            
            _phys.UpdatePhys();
            _waterMesh.UpdateMesh();
        }
        
    }
}