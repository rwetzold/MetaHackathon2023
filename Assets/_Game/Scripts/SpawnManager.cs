using System.Collections;
using UnityEngine;
using UnityEngine.AI;

namespace Hackathon
{
    public class SpawnManager : MonoBehaviour
    {
        [SerializeField] private Transform spawnPoint;
        [SerializeField] private PlayerBehaviour targetPoint;
        [SerializeField] private SpawnRule[] spawnRules;
        [SerializeField] private PlayerBehaviour ownerPlayer;

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
                spaceship.ownerPlayer = ownerPlayer;
                spaceship.SetTarget(targetPoint);
                if (spaceship.ownerPlayer == null)
                    Debug.Log("Player is null");
                agent.SetDestination(targetPoint.transform.position);

                yield return new WaitForSeconds(rule.delay);
            } while (true);
        }
    }
}