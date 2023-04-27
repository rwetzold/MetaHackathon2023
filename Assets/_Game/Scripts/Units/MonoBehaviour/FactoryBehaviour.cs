using Oculus.Interaction.HandGrab;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;

namespace Hackathon
{
    public class FactoryBehaviour : UnitBehaviour
    {
        private PlayerBehaviour _target;

        [SerializeField] private HandGrabInteractable _interactable, _inverseInteractable;

        private bool _canSpawn = false;
        private float _spawnTime;
        private List<GameObject> _spaceshipPool;
        private Animator _animator;

        [SerializeField] private bool _spwanOnAwake = false;

        public PlayerBehaviour target => _target;
        public FactoryAttributes FactoryAttributes => (FactoryAttributes)_attributes;

        // Start is called before the first frame update
        private void Start()
        {
            _spawnTime = 0;
            _spaceshipPool = new List<GameObject>();
            _target = GameManager.Instance.remotePlayer.GetComponent<PlayerBehaviour>();
            _animator = GetComponentInChildren<Animator>(true);

            _canSpawn = _spwanOnAwake;
            for (var i = 0; i < FactoryAttributes.PollSize; i++)
            {
                var spaceship = PhotonNetwork.Instantiate(FactoryAttributes.PrefabUnit, transform.position,
                    transform.rotation);
                spaceship.GetComponent<SpaceshipBehaviour>()._factory = this;
                spaceship.SetActive(false);
                _spaceshipPool.Add(spaceship);
            }
        }

        // Update is called once per frame
        private void Update()
        {
            if (_spaceshipPool.Count == 0 || _spaceshipPool[0].activeInHierarchy || !_canSpawn)
            {
                return;
            }

            _spawnTime += Time.deltaTime;
            if (_spawnTime >= FactoryAttributes.SpawnRate)
            {
                InstantiateSpaceship();
                _animator.SetTrigger("OpenDoor");
                _spawnTime = 0;
            }
        }

        private void InstantiateSpaceship()
        {
            _spaceshipPool[0].SetActive(true);
            _spaceshipPool[0].transform.position = transform.TransformPoint(FactoryAttributes.LocalSpawnPosition);
            _spaceshipPool[0].transform.forward = -transform.forward;
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