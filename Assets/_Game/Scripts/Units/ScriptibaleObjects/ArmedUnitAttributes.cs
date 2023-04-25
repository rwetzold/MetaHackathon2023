using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Data", menuName = "ScriptableObjects/ArmedUnitAttributes", order = 2)]
public class ArmedUnitAttributes : UnitAttributes
{
    [SerializeField]
    public int damage;

    [SerializeField]
    public int range;

    [SerializeField]
    public float fireRate;

}
