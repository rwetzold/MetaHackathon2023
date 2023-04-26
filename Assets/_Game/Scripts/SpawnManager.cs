using System.Collections;
using UnityEngine;
using UnityEngine.AI;

namespace Hackathon
{
    public class SpawnManager : MonoBehaviour
    {
        [SerializeField] private Transform spawnPoint;
        [SerializeField] private Transform targetPoint;
        [SerializeField] private SpawnRule[] spawnRules;

        private void Start()
        {
            foreach (SpawnRule rule in spawnRules)
            {
                StartCoroutine(DoSpawning(rule));
            }
        }

        private IEnumerator DoSpawning(SpawnRule rule)
        {
            yield return new WaitForSeconds(rule.initialDelay);

            do
            {
                GameObject go = Instantiate(rule.prefab);
                go.transform.SetPositionAndRotation(spawnPoint.position, spawnPoint.rotation);
                NavMeshAgent agent = go.GetComponentInChildren<NavMeshAgent>();
                SpaceshipBehaviour spaceship = go.GetComponentInChildren<SpaceshipBehaviour>();
                spaceship.ownerPlayer = gameObject;
                agent.SetDestination(targetPoint.position);

                yield return new WaitForSeconds(rule.delay);
            } while (true);
        }
    }
}