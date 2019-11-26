using System;
using src.FactoryPattern;
using UnityEngine;

namespace src.Liquids
{
    public class Water : MonoBehaviour, IInitializable
    {
        private WaterData _waterData;
        private WaterMesh _mesh;
        private WaterInteraction _phys;
        public void Init(IData data)
        {
            _waterData = data as WaterData;
            
            for (int i = 0; i < _waterData.SpringNum; i++)
            {
                _waterData.WaterSprings[i] = new WaterSpring();
                var pos = new Vector3(_waterData.Step * i + _waterData.Start, _waterData.BaseHeight);
                _waterData.WaterSprings[i].Position = pos;
            }

            _mesh = Factory<WaterMesh>.CreateInstance(gameObject, _waterData);
            _phys = Factory<WaterInteraction>.CreateInstance(gameObject, _waterData);
        }
        
        private void Update()
        {
            _mesh.UpdateMesh();
        }

        private void FixedUpdate()
        {
            _phys.UpdatePhys();
        }
    }
}