using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Player", menuName = "ScriptableObjects/PlayerAttributes", order = 4)]
public class PlayerAttributes : ScriptableObject
{
    [SerializeField]
    private int _startCurrency;
    public int StartCurrency => _startCurrency;

    [SerializeField]
    private int _health;
    public int Health => _health;

}

