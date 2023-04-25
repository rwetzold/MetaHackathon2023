using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Data", menuName = "ScriptableObjects/OffenceAttributes", order = 3)]
public class OffenceAttributes : ArmedUnitAttributes
{

    [SerializeField]
    public int shield;

    [SerializeField]
    public float speed;

    [SerializeField]
    public int health;


}
