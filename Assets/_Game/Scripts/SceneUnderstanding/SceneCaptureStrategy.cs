using System;
using UnityEngine;

namespace Hackathon
{
    public abstract class SceneCaptureStrategy : MonoBehaviour
    {
        public abstract void Start();

        public abstract void CaptureScene(Action<Scene> onComplete);
    }
}