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
        private GameObject splashPrefab;
        public void Init(IData data)
        {
            _waterData = data as WaterData;
            splashPrefab = Resources.Load<GameObject>("Prefabs\\SplashNew");
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

        private void OnTriggerEnter2D(Collider2D other)
        {
            var rigidbody = other.GetComponent<Rigidbody2D>();
            if (rigidbody != null)
            {
                var transformPosition = rigidbody.transform.position;
                var localScale = rigidbody.transform.localScale;

                float bottomY = transformPosition.y - localScale.y / 2;
                float bottomX = transformPosition.x;

                float velocityY = rigidbody.velocity.y;
                var splashobj = Instantiate(splashPrefab);
                splashobj.transform.position = new Vector3(bottomX, bottomY-Mathf.Abs(velocityY)*0.08f*transform.localScale.y, 10);
                
                var particles = splashobj.GetComponent<ParticleSystem>();
                particles.transform.localScale = transform.localScale;
                float lifetime = 1f + Mathf.Abs(velocityY)*0.09f;
                
                ParticleSystem.MainModule main = particles.main;
                main.startSpeed = 2 + Mathf.Abs(velocityY)*0.5f;
                main.startLifetime = lifetime;
                var particlesEmission = particles.emission;
                particlesEmission.rateOverTime = 2 + Mathf.Abs(velocityY)*2;
                
                
                particles.Play();
                Destroy(splashobj, lifetime);
            }
        }
    }
}