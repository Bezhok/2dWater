using UnityEngine;

namespace src
{
    public class WaterMesh : MonoBehaviour
    {
        private WaterSpring[] _water;
        public int SpringNum { get; set; }
        public int Bottom { get; set; }
        private Mesh _mesh;
        
        public void Init(WaterSpring[] water, int springNum, int bottom)
        {
            _water = water;
            SpringNum = springNum;
            Bottom = bottom;
            
            var meshRenderer = gameObject.AddComponent<MeshRenderer>();
            meshRenderer.material = Resources.Load<Material>("Materials/Default");
            _mesh = gameObject.AddComponent<MeshFilter>().mesh;

            GenerateData();
        }
        
        public void UpdateMesh()
        {
            Vector3[] vertices = _mesh.vertices;
            for (int i = 0, j = 0; j < SpringNum-1; i+=4, j++)
            {
                var pos = _water[j].Position;
                vertices[i] = pos;
                pos.y = Bottom;
                vertices[i+1] = pos;

                pos = _water[j+1].Position;
                vertices[i+2] = pos;
                pos.y = Bottom;
                vertices[i+3] = pos;
            }

            _mesh.vertices = vertices;
        }
        public void GenerateData()
        {
            _mesh.Clear();
            _mesh.MarkDynamic();

            Color[] colors = new Color[SpringNum*4];
            Vector3[] vertices = new Vector3[SpringNum*4];
            int[] eboTemplate = {0, 3, 1, 3, 0, 2};
            int[] ebo = new int[SpringNum*eboTemplate.Length];

            for (int vertIdx = 0, springIdx = 0, eboIdx = 0; springIdx < SpringNum-1; vertIdx+=4, eboIdx+=eboTemplate.Length, springIdx++)
            {
                var pos = _water[springIdx].Position;
                vertices[vertIdx] = pos;
                pos.y = Bottom;
                vertices[vertIdx+1] = pos;

                pos = _water[springIdx+1].Position;
                vertices[vertIdx+2] = pos;
                pos.y = Bottom;
                vertices[vertIdx+3] = pos;
                
                //0--2
                //|  |
                //1--3
                // clock
                for (int k = 0; k < eboTemplate.Length; k++)
                {
                    ebo[eboIdx+k] = vertIdx+eboTemplate[k];
                }

                colors[vertIdx] = colors[vertIdx+2] = new Color(0, 1, 1, 1);
                colors[vertIdx + 1] = colors[vertIdx + 3] = new Color(0, 0, 1, 1);
            }

            _mesh.vertices = vertices;
            _mesh.triangles = ebo;
            _mesh.colors = colors;
        }
    }
}