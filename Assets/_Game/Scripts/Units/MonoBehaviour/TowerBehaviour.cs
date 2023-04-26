using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace Hackathon.Commands
{
    public class TowerBehaviour : UnitBehaviour
    {
        [SerializeField]
        private PlayerBehaviour testPlayer;

        private AimTargetCommand _aimTarget = null;
        private float _lastShot = 0f;

        public ArmedUnitAttributes armedAttributes
        {
            get
            {
                if (attributes is ArmedUnitAttributes)
                    return (ArmedUnitAttributes)attributes;
                else
                    return ScriptableObject.CreateInstance<ArmedUnitAttributes>();
            }
        }

        private void Start()
        {
            ownerPlayer = testPlayer;
            _lastShot = Time.time;
        }

        private void Update()
        {
            if (_aimTarget == null)
                _aimTarget = new AimTargetCommand(this);
            _aimTarget.Execute();

            if (_aimTarget.nextTarget != null && Time.time - _lastShot > armedAttributes.FireRageValue)
            {
                new AttacCommand(this, _aimTarget.nextTarget).Execute();
                _lastShot = Time.time;
            }
        }

    }
}
