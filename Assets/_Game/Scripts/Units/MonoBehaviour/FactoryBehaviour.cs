using System.Collections.Generic;
using UnityEngine;

namespace Hackathon
{
    public class FactoryBehaviour : UnitBehaviour
    {
        private float _spawnTime;
        public FactoryAttributes FactoryAttributes => (FactoryAttributes)_attributes;
        private List<GameObject> _spaceshipPool;

        // Start is called before the first frame update
        void Start()
        {
            _spawnTime = 0;
            _spaceshipPool = new List<GameObject>();

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
            if (_spaceshipPool[0].activeInHierarchy)
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
    }

}
