using System;
using UnityEngine;

[Serializable]
public class SpawnRule
{
    public GameObject prefab;
    public float initialDelay = 0f;
    public float delay = 1f;
}