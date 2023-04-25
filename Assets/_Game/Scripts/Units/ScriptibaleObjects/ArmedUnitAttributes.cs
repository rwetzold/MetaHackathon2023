using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Data", menuName = "ScriptableObjects/ArmedUnitAttributes", order = 2)]
public class ArmedUnitAttributes : UnitAttributes
{
    [SerializeField]
    private int _damage;
    public int DamageValue => _damage;

    [SerializeField]
    private int _range;
    public int RangeValue => _range;

    [SerializeField]
    private float _fireRate;
    public float FireRageValue => _fireRate;



}
