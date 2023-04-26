using UnityEngine;

public abstract class UnitBehaviour : MonoBehaviour
{
    [HideInInspector] public GameObject ownerPlayer;
    [SerializeField] protected UnitAttributes _attributes;
}
