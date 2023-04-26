using UnityEngine;
using UnityEngine.AI;

namespace Hackathon
{
    public class SpaceshipBehaviour : UnitBehaviour
    {
        [SerializeField]
        private PlayerBehaviour _target;
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
        public void SetTarget(PlayerBehaviour targetPlayer)
        {
            _target = targetPlayer;
        }

        private void OnEnable()
        {
           
            if (_navMeshAgent == null)
            {
                _navMeshAgent = GetComponent<NavMeshAgent>();
            }

            if (_factory != null)
            {
                _navMeshAgent.SetDestination(_factory.FactoryAttributes.TargetPosition);
                _target = _factory.target;
            }

            _currentHealth = spaceshipAttributes.HealthValue;
        
        }

        void Update()
        {
            if (_target != null)
            {
                _navMeshAgent.SetDestination(_target.transform.position);
                Vector3 targetPos = _target.transform.position;
                targetPos.y = 0;
                Vector3 offset = targetPos - transform.position;
                float dist = offset.sqrMagnitude;
                if (dist<0.3f)
                { 
                    if (_target != null && _target != ownerPlayer)
                    {
                        Commands.HitPlayerCommand hitPlayer = new Commands.HitPlayerCommand(this, _target);
                        hitPlayer.Execute();
                    }
                }

            }

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

        public void Destroy()
        {
            Destroy(gameObject);
        }

        void OnCollisionEnter(Collision collision)
        {
        }
    }
}


