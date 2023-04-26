using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace Hackathon.Commands
{
    public class TowerBehaviour : UnitBehaviour
    {

        private AimTarget _aimTarget = null;
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
            _aimTarget = new AimTarget(this);
            _lastShot = Time.time;
        }

        private void Update()
        {
            _aimTarget.Execute();

            if (_aimTarget.nextTarget != null && Time.time - _lastShot > armedAttributes.FireRageValue)
            {
                new AttacCommand(this, _aimTarget.nextTarget).Execute();
                _lastShot = Time.time;
            }
        }

    }
}
