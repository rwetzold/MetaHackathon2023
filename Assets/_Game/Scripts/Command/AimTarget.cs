using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Hackathon.Commands
{

    public class AimTarget : ICommand
    {
        private const int MAX_COLLIDERS = 10;
        private const int ENEMY_LAYER = 6;

        private TowerBehaviour _attacer;

        private SpaceshipBehaviour _nextTarget;
        public SpaceshipBehaviour nextTarget => _nextTarget;

        private float _distanceToTarget = 100000f;
        public float distanceToTarget => _distanceToTarget;

        private Collider[] _hits;

        private GameObject _ownerPlayer;

        public AimTarget(TowerBehaviour attacer)
        {
            _attacer = attacer;
            _ownerPlayer = attacer.ownerPlayer;
            _hits = new Collider[MAX_COLLIDERS];
        }

        public void Execute()
        {
            int colliders = Physics.OverlapSphereNonAlloc(_attacer.transform.position, _attacer.armedAttributes.RangeValue, _hits,
           1 << ENEMY_LAYER);
            _distanceToTarget = 10000000f;

            _nextTarget = null;

            for (int i = 0; i < colliders; i++)
            {
                Vector3 offset = _ownerPlayer.transform.position - _hits[i].ClosestPoint(_attacer.transform.position);
                float distance = offset.sqrMagnitude;
                SpaceshipBehaviour spaceship = _hits[i].GetComponentInParent<SpaceshipBehaviour>();
                if (_distanceToTarget > distance && spaceship != null && _ownerPlayer != spaceship.ownerPlayer)
                {
                    _nextTarget = spaceship;
                    _distanceToTarget = distance;

                }
                Debug.DrawLine(_attacer.transform.position, _nextTarget.transform.position);
            }

        }

    }
}
