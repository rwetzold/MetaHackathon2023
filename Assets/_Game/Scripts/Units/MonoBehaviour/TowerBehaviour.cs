using Oculus.Interaction.HandGrab;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace Hackathon.Commands
{
    public class TowerBehaviour : UnitBehaviour
    {
        [SerializeField]
        private HandGrabInteractable _interactable, _inverseInteractable;

        [SerializeField]
        private Transform _cannonRotator;

        [SerializeField]
        private Transform _turret;

        [SerializeField]
        private GameObject _muzzleFlashes;

        private bool _towerActiv = false;

        private AimTargetCommand _aimTarget = null;
        private float _lastShot = 0f;
        private float _muzzleFlashesTimer;
        public ArmedUnitAttributes armedAttributes
        {
            get
            {
                if (Attributes is ArmedUnitAttributes)
                    return (ArmedUnitAttributes)Attributes;
                else
                    return ScriptableObject.CreateInstance<ArmedUnitAttributes>();
            }
        }

        public void SetTowerActiv()
        {
            _towerActiv = true;
        }

        private void Start()
        {
            _lastShot = Time.time;
        }

        private void Update()
        {
            if (_towerActiv)
            {
                if (_muzzleFlashesTimer < Time.time)
                    _muzzleFlashes.SetActive(false);
                if (_aimTarget == null)
                    _aimTarget = new AimTargetCommand(this);
                _aimTarget.Execute();

                if (_aimTarget.nextTarget != null)
                {
                    Vector3 oldRotation = _cannonRotator.eulerAngles;
                    _cannonRotator.LookAt(_aimTarget.nextTarget.transform);
                    _cannonRotator.eulerAngles = new Vector3(_cannonRotator.eulerAngles.x, oldRotation.y, oldRotation.z);

                    oldRotation = _turret.eulerAngles;
                    _turret.LookAt(_aimTarget.nextTarget.transform);
                    _turret.eulerAngles = new Vector3(oldRotation.x, _turret.eulerAngles.y + 180, oldRotation.z);

                    if (Time.time - _lastShot > armedAttributes.FireRageValue)
                    {
                        _muzzleFlashes.SetActive(true);
                        new AttacCommand(this, _aimTarget.nextTarget).Execute();
                        _lastShot = Time.time;
                        _muzzleFlashesTimer = Time.time + 0.1f;
                    }
                }
            }
        }

        private void OnCollisionEnter(Collision collision)
        {
            if (collision.gameObject.CompareTag("Plane"))
            {
                var eulerRotation = transform.eulerAngles;
                eulerRotation.x = 0;
                eulerRotation.z = 0;
                transform.rotation = Quaternion.Euler(eulerRotation);
                _towerActiv = true;
                _interactable.gameObject.SetActive(false);
                _inverseInteractable.gameObject.SetActive(false);
            }
        }
    }
}
