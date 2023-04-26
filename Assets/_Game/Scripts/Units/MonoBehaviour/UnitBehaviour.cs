using Oculus.Interaction;
using UnityEngine;

public abstract class UnitBehaviour : MonoBehaviour
{
    [HideInInspector] public PlayerBehaviour ownerPlayer;
    [SerializeField] protected UnitAttributes _attributes;
    public UnitAttributes attributes => _attributes;
}
