using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Data", menuName = "ScriptableObjects/OffenceAttributes", order = 3)]
public class OffenceAttributes : ArmedUnitAttributes
{

    [SerializeField]
    private int _shield;
    public int ShieldValue => _shield;

    [SerializeField]
    private float _speed;
    public float SpeedValue => SpeedValue;

    [SerializeField]
    private int _health;
    public int HealthValue => _health;


}
