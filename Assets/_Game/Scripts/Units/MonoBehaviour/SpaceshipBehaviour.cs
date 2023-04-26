using UnityEngine;
using UnityEngine.AI;

namespace Hackathon
{
    public class SpaceshipBehaviour : UnitBehaviour
    {
        private GameObject _target;
        private NavMeshAgent _navMeshAgent;

        // Temp
        public FactoryBehaviour _factory;

        private void OnEnable()
        {
            if (_navMeshAgent == null)
            {
                _navMeshAgent = GetComponent<NavMeshAgent>();
            }

            if(_factory != null)
            {
                _navMeshAgent.SetDestination(_factory.FactoryAttributes.TargetPosition);
            }
        }

        public void ApplyDamage(int damage)
        {

        }
    }
}

