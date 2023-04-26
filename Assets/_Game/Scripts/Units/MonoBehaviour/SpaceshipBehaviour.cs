using UnityEngine;
using UnityEngine.AI;

namespace Hackathon
{
    public class SpaceshipBehaviour : UnitBehaviour
    {
        private GameObject _target;
        private NavMeshAgent _navMeshAgent;
        private int _currentHealth = 0;
        public int currentHealth => _currentHealth;

        public SpaceshipAttributes spaceshipAttributes
        {
            get
            {
                if (attributes is SpaceshipAttributes)
                    return (SpaceshipAttributes)attributes;
                else
                    return ScriptableObject.CreateInstance<SpaceshipAttributes>();
            }
        }

        // Temp
        public FactoryBehaviour _factory;

        private void OnEnable()
        {
            if (_navMeshAgent == null)
            {
                _navMeshAgent = GetComponent<NavMeshAgent>();
            }

            if (_factory != null)
            {
                _navMeshAgent.SetDestination(_factory.FactoryAttributes.TargetPosition);
            }

            _currentHealth = spaceshipAttributes.HealthValue;
        }

        public bool ApplyDamage(int damage)
        {
            _currentHealth -= damage;
            if (_currentHealth <= 0)
            {
                Destroy(this.gameObject);
                return true;
            }
            return false;
        }
    }
}


