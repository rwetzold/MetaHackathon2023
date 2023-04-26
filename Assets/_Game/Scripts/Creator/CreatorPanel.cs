using System.Collections;
using System.Collections.Generic;
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

        // Start is called before the first frame update
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {

        }
    }

}
