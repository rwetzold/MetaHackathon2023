using UnityEngine;
using UnityEngine.AI;

namespace Hackathon
{
    public class SpaceshipBehaviour : UnitBehaviour
    {
        private GameObject _target;
        private NavMeshAgent _navMeshAgent;
        private int _currentHealth = 0;

		public OffenceAttributes offenceAttributes
    {
        get
        {
            if (attributes is OffenceAttributes)
                return (OffenceAttributes)attributes;
            else
                return ScriptableObject.CreateInstance<OffenceAttributes>();
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

            if(_factory != null)
            {
                _navMeshAgent.SetDestination(_factory.FactoryAttributes.TargetPosition);
            }

            _currentHealth = offenceAttributes.HealthValue;
        }

    public void ApplyDamage(int damage)
    {
        _currentHealth -= damage;
        if (_currentHealth <= 0)
            Destroy(this.gameObject);
    }
}

