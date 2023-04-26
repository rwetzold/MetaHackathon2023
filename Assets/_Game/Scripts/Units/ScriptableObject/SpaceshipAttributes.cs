using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Spaceship", menuName = "ScriptableObjects/SpaceshipAttributes", order = 1)]
public class SpaceshipAttributes : ArmedUnitAttributes
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

    [SerializeField]
    private int _coinsGain;
    public int coinsGain => _coinsGain;


}
