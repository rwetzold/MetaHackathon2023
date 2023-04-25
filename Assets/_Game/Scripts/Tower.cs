using UnityEngine;

public class Tower : MonoBehaviour
{
    private const int MAX_COLLIDERS = 10;
    private const int ENEMY_LAYER = 6;

    [SerializeField] private float damage = 1f;
    [SerializeField] private float range = 1f;
    [SerializeField] private float fireRate = 1f;

    private Collider[] _hits;

    private void Start()
    {
        _hits = new Collider[MAX_COLLIDERS];
    }

    private void Update()
    {
        int colliders = Physics.OverlapSphereNonAlloc(transform.position, range, _hits, 1 << ENEMY_LAYER);
        for (int i = 0; i < colliders; i++)
        {
            Debug.DrawLine(transform.position, _hits[i].ClosestPoint(transform.position));
        }
    }
}