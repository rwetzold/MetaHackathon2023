using Oculus.Interaction;
using UnityEngine;

public abstract class UnitBehaviour : MonoBehaviour
{
    [HideInInspector] public PlayerBehaviour ownerPlayer;
    [SerializeField] protected UnitAttributes _attributes;
    public UnitAttributes attributes => _attributes;

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Plane"))
        {
            transform.rotation = Quaternion.Euler(Vector3.zero);
        }
    }
}
