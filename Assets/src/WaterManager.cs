using System;
using System.Collections.Generic;
using src.FactoryPattern;
using src.Liquids;
using UnityEngine;

namespace src
{
    public class WaterManager : MonoBehaviour
    {
        private Camera _camera;
        [SerializeField] private GameObject cubePrefab;
        private readonly List<GameObject> objs = new List<GameObject>();

        private void Start()
        {
            if (cubePrefab == null) throw new NullReferenceException();

            _camera = Camera.main;
            {
                var height = 5f;
                var water = new GameObject("Screen water");
                var K = 0.01f;

                var halfHeight = _camera.orthographicSize;
                var halfWidth = halfHeight * Screen.width / Screen.height;

                var width = halfWidth * 2;


                var waterData = new WaterData(width, height, K);
                Factory<Water>.CreateInstance(water, waterData);
                water.transform.position = new Vector3(0, -2.5f);
            }
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.Mouse0))
            {
                var obj = Instantiate(cubePrefab);
                var position = _camera.ScreenToWorldPoint(Input.mousePosition);
                position = new Vector3(position.x, position.y, 5);
                obj.transform.position = position;
                objs.Add(obj);
            }
        }
    }
}