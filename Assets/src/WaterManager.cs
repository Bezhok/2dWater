using System;
using UnityEngine;

namespace src
{
    public class WaterManager : MonoBehaviour
    {
        [SerializeField] private GameObject cubePrefab;

        private void Start()
        {
            if (cubePrefab == null)
            {
                throw new NullReferenceException();
            }

            new GameObject().AddComponent<Water.Water>();
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.Mouse0))
            {
                var obj = Instantiate(cubePrefab);
                var position = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                position = new Vector3(position.x, position.y, 5);
                obj.transform.position = position;
            }
        }
    }
}