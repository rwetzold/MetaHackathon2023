using UnityEngine;

public abstract class UnitBehaviour : MonoBehaviour
{
    [SerializeField] private UnitAttributes _attributes;
    public GameObject ownerPlayer;

}
