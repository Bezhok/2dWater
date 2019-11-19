using System;
using UnityEngine;

namespace src
{
    public class Water : MonoBehaviour
    {
        private WaterData _waterData;

        private void Start()
        {
            _waterData = new WaterData();
            _waterData.Step = 0.1f;
            _waterData.Bottom = -5;
            _waterData.K = 0.01f;
            
            var halfheight = Camera.main.orthographicSize;
            var halfWidth = halfheight * Screen.width / Screen.height;
            _waterData.SpringNum = (int) Math.Ceiling(halfWidth * 2 / _waterData.Step) + 4;
            _waterData.WaterSprings = new WaterSpring[_waterData.SpringNum];

            for (int i = 0; i < _waterData.SpringNum; i++)
            {
                _waterData.WaterSprings[i] = new WaterSpring();
                var pos = new Vector3(_waterData.Step * i - halfWidth - _waterData.Step / 4, _waterData.BaseHeight);
                _waterData.WaterSprings[i].Position = pos;
            }

            _waterData.Mesh = gameObject.AddComponent<WaterMesh>();
            _waterData.Mesh.Init(_waterData);

            _waterData.Phys = gameObject.AddComponent<WaterInteraction>();
            _waterData.Phys.Init(_waterData);
        }
        
        private void Update()
        {
            _waterData.Phys.UpdatePhys();
            _waterData.Mesh.UpdateMesh();
        }
    }
}