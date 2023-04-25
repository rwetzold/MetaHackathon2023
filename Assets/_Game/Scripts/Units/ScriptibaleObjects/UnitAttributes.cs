using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Data", menuName = "ScriptableObjects/UnitAttributes", order = 1)]

public class UnitAttributes : ScriptableObject
{
    [SerializeField]
    private float _priceValue;

    public float PriceValue => _priceValue;
}
