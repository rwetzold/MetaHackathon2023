using TMPro;
using UnityEngine;

namespace Hackathon
{
    public class CreatorPanel : MonoBehaviour
    {
        [SerializeField]
        private TMP_Text _resourcesText;

        [SerializeField]
        private TMP_Text _lifeText;

        [HideInInspector]
        public PlayerBehaviour player;

        public void UpdateResourceText(int newValue)
        {
            _resourcesText.text = newValue.ToString();
        }
    }

}
