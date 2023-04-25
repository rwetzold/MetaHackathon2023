using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class UnitAttributes : ScriptableObject
{
    [SerializeField]
    private float _priceValue;

    public float PriceValue => _priceValue;
}
