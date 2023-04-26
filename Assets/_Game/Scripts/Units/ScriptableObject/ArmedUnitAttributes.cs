using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class ArmedUnitAttributes : UnitAttributes
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
