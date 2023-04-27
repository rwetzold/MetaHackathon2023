using Oculus.Voice;
using UnityEngine;

namespace Hackathon
{
    public class VoiceActivator : MonoBehaviour
    {
        public float interval;
        public AppVoiceExperience ape;

        private float _nextActivation;

        private void Update()
        {
            if (Time.time < _nextActivation) return;

            ape.Activate();

            _nextActivation = Time.time + interval;
        }
    }
}