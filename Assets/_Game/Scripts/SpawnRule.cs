using System;
using UnityEngine;

namespace Hackathon
{
    [Serializable]
    public class SpawnRule
    {
        public string prefab;
        public float initialDelay = 0f;
        public float delay = 1f;
    }
}