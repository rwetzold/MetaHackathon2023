using System;
using UnityEngine;

namespace Hackathon
{
    [Serializable]
    public class SpawnRule
    {
        public GameObject prefab;
        public float initialDelay = 0f;
        public float delay = 1f;
    }
}