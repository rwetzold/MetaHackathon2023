using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace Hackathon.Commands
{
    public class TowerBehaviour : UnitBehaviour
    {
        [SerializeField]
        private Transform _cannonRotator;

        [SerializeField]
        private Transform _turret;

        [SerializeField]
        private GameObject _muzzleFlashes;

        [SerializeField]
        private PlayerBehaviour testPlayer;

        private AimTargetCommand _aimTarget = null;
        private float _lastShot = 0f;
        private float _muzzleFlashesTimer;
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
            if (_muzzleFlashesTimer < Time.time)
                _muzzleFlashes.SetActive(false);
            if (_aimTarget == null)
                _aimTarget = new AimTargetCommand(this);
            _aimTarget.Execute();

            Vector3 oldRotation = _cannonRotator.eulerAngles;
            _cannonRotator.LookAt(_aimTarget.nextTarget.transform);
            _cannonRotator.eulerAngles = new Vector3(_cannonRotator.eulerAngles.x, oldRotation.y, oldRotation.z);

             oldRotation = _turret.eulerAngles;
            _turret.LookAt(_aimTarget.nextTarget.transform);
            _turret.eulerAngles = new Vector3(oldRotation.x, _turret.eulerAngles.y+180, oldRotation.z);

            if (_aimTarget.nextTarget != null && Time.time - _lastShot > armedAttributes.FireRageValue)
            {
                _muzzleFlashes.SetActive(true);
                new AttacCommand(this, _aimTarget.nextTarget).Execute();
                _lastShot = Time.time;
                _muzzleFlashesTimer = Time.time + 1000 / 60 * 10;
            }
        }

    }
}
