using System.Collections;
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
        [SerializeField]
        public GameObject _damagePhase01;
        [SerializeField]
        public GameObject _damagePhase02;

        [SerializeField]
        public GameObject _damagePhase03;

        [SerializeField]
        public GameObject _damageDeath;

        [SerializeField]
        public GameObject _body;

        public SpaceshipAttributes spaceshipAttributes
        {
            get
            {
                if (Attributes is SpaceshipAttributes)
                    return (SpaceshipAttributes)Attributes;
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
                _navMeshAgent.SetDestination(_factory.target.transform.position);
                _target = _factory.target;
            }

            _currentHealth = spaceshipAttributes.HealthValue;
        
        }

        void Update()
        {
            if (_target != null)
            {
                Vector3 targetPos = _target.transform.position;
                targetPos.y = 0;
                _navMeshAgent.SetDestination(targetPos);
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
            if (_currentHealth <= spaceshipAttributes.HealthValue/4*3)
            {
                if (_currentHealth <= spaceshipAttributes.HealthValue / 4 * 2)
                {
                    if (_currentHealth <= spaceshipAttributes.HealthValue / 4)
                    {
                        if (_currentHealth <= 0)
                        {
                            KillIt();
                            return true;
                        }
                        else
                        {
                            _damagePhase01.SetActive(false);
                            _damagePhase02.SetActive(false);
                            _damagePhase03.SetActive(true);
                            _damageDeath.SetActive(false);
                        }
                    }
                    else
                    {
                        _damagePhase01.SetActive(false);
                        _damagePhase02.SetActive(true);
                        _damagePhase03.SetActive(false);
                        _damageDeath.SetActive(false);
                    }
                }
                else
                {
                    _damagePhase01.SetActive(true);
                    _damagePhase02.SetActive(false);
                    _damagePhase03.SetActive(false);
                    _damageDeath.SetActive(false);
                }

            }
            else
            {

            }
            return false;
        }

        public void KillIt()
        {
            StartCoroutine(KillSlow());
        }

        IEnumerator KillSlow()
        {
            _damagePhase01.SetActive(false);
            _damagePhase02.SetActive(false);
            _damagePhase03.SetActive(false);
            _damageDeath.SetActive(true);
            _body.SetActive(false);
            yield return new WaitForSeconds(0.5f);
            _damagePhase01.SetActive(false);
            _damagePhase02.SetActive(false);
            _damagePhase03.SetActive(false);
            _damageDeath.SetActive(false);
            _body.SetActive(true);
            gameObject.SetActive(false);
        }


        void OnCollisionEnter(Collision collision)
        {
        }
    }
}


