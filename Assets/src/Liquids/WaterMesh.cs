﻿using src.FactoryPattern;
using UnityEngine;

namespace src.Liquids
{
    public class WaterMesh : MonoBehaviour, IInitializable
    {
        private Mesh _mesh;
        private WaterData _waterData;

        public void Init(IData waterData)
        {
            _waterData = waterData as WaterData;

            var meshRenderer = gameObject.AddComponent<MeshRenderer>();
            meshRenderer.material = Resources.Load<Material>("Materials/Default");
            _mesh = gameObject.AddComponent<MeshFilter>().mesh;

            GenerateData();
        }

        public void UpdateMesh()
        {
            Vector3[] vertices = _mesh.vertices;
            for (int vertIdx = 0, springIdx = 0; springIdx < _waterData.SpringNum - 1; vertIdx += 4, springIdx++)
            {
                Vector3 pos = _waterData.WaterSprings[springIdx].position;
                vertices[vertIdx] = pos;

                pos.y = _waterData.Bottom;
                vertices[vertIdx + 1] = pos;

                pos = _waterData.WaterSprings[springIdx + 1].position;
                vertices[vertIdx + 2] = pos;

                pos.y = _waterData.Bottom;
                vertices[vertIdx + 3] = pos;
            }

            _mesh.vertices = vertices;
        }

        public void GenerateData()
        {
            _mesh.Clear();
            _mesh.MarkDynamic();

            var colors = new Color[_waterData.SpringNum * 4];
            var vertices = new Vector3[_waterData.SpringNum * 4];
            int[] eboTemplate = {0, 3, 1, 3, 0, 2};
            var ebo = new int[_waterData.SpringNum * eboTemplate.Length];

            for (int vertIdx = 0, springIdx = 0, eboIdx = 0;
                springIdx < _waterData.SpringNum - 1;
                vertIdx += 4, eboIdx += eboTemplate.Length, springIdx++)
            {
                Vector3 pos = _waterData.WaterSprings[springIdx].position;
                vertices[vertIdx] = pos;

                pos.y = _waterData.Bottom;
                vertices[vertIdx + 1] = pos;

                pos = _waterData.WaterSprings[springIdx + 1].position;
                vertices[vertIdx + 2] = pos;

                pos.y = _waterData.Bottom;
                vertices[vertIdx + 3] = pos;

                //0--2
                //|  |
                //1--3
                // clock
                for (int k = 0; k < eboTemplate.Length; k++) ebo[eboIdx + k] = vertIdx + eboTemplate[k];

                colors[vertIdx] = colors[vertIdx + 2] = new Color(0, 1, 1, 0.6f);
                colors[vertIdx + 1] = colors[vertIdx + 3] = new Color(0.2f, 0.1f, 0.99f, 0.8f);
            }

            _mesh.vertices = vertices;
            _mesh.triangles = ebo;
            _mesh.colors = colors;
        }
    }
}