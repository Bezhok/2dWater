using System;
using System.Collections;
using System.Collections.Generic;
using src.FactoryPattern;
using src.Liquids;
using UnityEngine;

namespace src
{
    public class WaterManager : MonoBehaviour
    {
        [SerializeField] private GameObject cubePrefab;

        private WaterData waterData;
        private List<GameObject> objs = new List<GameObject>();
        private void Start()
        {
            if (cubePrefab == null)
            {
                throw new NullReferenceException();
            }

            {
                float height = 5f;
                var water = new GameObject("Screen water");
                float K = 0.01f;
  
                var halfheight = Camera.main.orthographicSize;
                var halfWidth = halfheight * Screen.width / Screen.height;

                float width = halfWidth * 2;


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
                var position = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                position = new Vector3(position.x, position.y, 5);
                obj.transform.position = position;
                objs.Add(obj);
            }
        }
    }
}