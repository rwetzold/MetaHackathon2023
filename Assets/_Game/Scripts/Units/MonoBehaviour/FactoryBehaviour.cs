using Oculus.Interaction.HandGrab;
using System.Collections.Generic;
using UnityEngine;

namespace Hackathon
{
    public class FactoryBehaviour : UnitBehaviour
    {
        [SerializeField]
        private PlayerBehaviour _target;

        [SerializeField]
        private HandGrabInteractable _interactable, _inverseInteractable;

        private bool _canSpawn = false;
        private float _spawnTime;
        private List<GameObject> _spaceshipPool;
        private Animator _animator;

        public PlayerBehaviour target => _target;
        public FactoryAttributes FactoryAttributes => (FactoryAttributes)_attributes;

        // Start is called before the first frame update
        void Start()
        {
            _spawnTime = 0;
            _spaceshipPool = new List<GameObject>();
            _animator = GetComponentInChildren<Animator>(true);

            for (var i = 0; i < FactoryAttributes.PollSize; i++)
            {
                var spaceship = Instantiate(FactoryAttributes.PrefabUnit);
                spaceship.GetComponent<SpaceshipBehaviour>()._factory = this;
                spaceship.SetActive(false);
                _spaceshipPool.Add(spaceship);
            }
        }

        // Update is called once per frame
        void Update()
        {
            if (_spaceshipPool[0].activeInHierarchy || !_canSpawn)
            {
                return;
            }

            _spawnTime += Time.deltaTime;
            if (_spawnTime >= FactoryAttributes.SpawnRate)
            {
                InstantiateSpaceship();
                _spawnTime = 0;
            }
        }

        private void InstantiateSpaceship()
        {
            _spaceshipPool[0].SetActive(true);
            _spaceshipPool[0].transform.SetPositionAndRotation(transform.TransformPoint(FactoryAttributes.LocalSpawnPosition), transform.rotation);
            _spaceshipPool.ShiftList();
        }

        private void OnCollisionEnter(Collision collision)
        {
            if (collision.gameObject.CompareTag("Plane"))
            {
                var eulerRotation = transform.eulerAngles;
                eulerRotation.x = 0;
                eulerRotation.z = 0;
                transform.rotation = Quaternion.Euler(eulerRotation);
                _canSpawn = true;
                _interactable.gameObject.SetActive(false);
                _inverseInteractable.gameObject.SetActive(false);
            }
        }
    }

}
