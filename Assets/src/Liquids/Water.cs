using src.FactoryPattern;
using UnityEngine;

namespace src.Liquids
{
    public class Water : MonoBehaviour, IInitializable
    {
        private WaterMesh _mesh;
        private WaterInteraction _phys;
        private GameObject _splashPrefab;
        private WaterData _waterData;

        public void Init(IData data)
        {
            _waterData = data as WaterData;
            _splashPrefab = Resources.Load<GameObject>("Prefabs\\SplashNew");
            for (int i = 0; i < _waterData.SpringNum; i++)
            {
                _waterData.WaterSprings[i] = new WaterSpring();
                var pos = new Vector3(_waterData.Step * i + _waterData.Left, _waterData.Top);
                _waterData.WaterSprings[i].position = pos;
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
            var otherRigidbody = other.GetComponent<Rigidbody2D>();
            if (otherRigidbody == null) return;

            CreateSplash(otherRigidbody);
        }

        private void CreateSplash(Rigidbody2D otherRigidbody)
        {
            Transform transformOther = otherRigidbody.transform;
            Vector3 transformPosition = transformOther.position;
            Vector3 localScale = transformOther.localScale;

            float velocityY = otherRigidbody.velocity.y;
            Vector2 splashPos = transformPosition;
            splashPos.y -= localScale.y * 0.5f + Mathf.Abs(velocityY) * 0.04f * transform.localScale.y;

            GameObject splashobj = Instantiate(_splashPrefab);
            splashobj.transform.position = new Vector3(splashPos.x, splashPos.y, 10);

            float lifetime = 1f + Mathf.Abs(velocityY) * 0.09f;

            ParticleSystem particles = CreateParticles(splashobj, velocityY, lifetime);

            particles.Play();
            Destroy(splashobj, lifetime);
        }

        private ParticleSystem CreateParticles(GameObject splashObj, float velocityY, float lifetime)
        {
            var particles = splashObj.GetComponent<ParticleSystem>();
            particles.transform.localScale = transform.localScale;

            ParticleSystem.MainModule main = particles.main;
            main.startSpeed = 2 + Mathf.Abs(velocityY) * 0.5f;
            main.startLifetime = lifetime;
            ParticleSystem.EmissionModule particlesEmission = particles.emission;
            int particleCount = (int) Mathf.Abs(velocityY) * 2;
            particlesEmission.SetBurst(0, new ParticleSystem.Burst(0, particleCount, 1, 0.01f));

            return particles;
        }
    }
}