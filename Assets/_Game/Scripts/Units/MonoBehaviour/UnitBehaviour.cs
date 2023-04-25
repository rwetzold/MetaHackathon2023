using UnityEngine;

public abstract class UnitBehaviour : MonoBehaviour
{
    [SerializeField] protected UnitAttributes _attributes;
    public GameObject ownerPlayer;
}
