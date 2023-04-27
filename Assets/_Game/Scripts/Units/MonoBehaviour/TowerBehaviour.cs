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

        [SerializeField]
        private Animator _animator;

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
//                    LookAtX(_aimTarget.nextTarget.transform,_cannonRotator);
//                    LookAtY(_aimTarget.nextTarget.transform, _turret);

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

        void LookAtY(Transform target, Transform viewer)
        {
            Vector3 lookPos = target.position - viewer.transform.position;
            Quaternion lookRot = Quaternion.LookRotation(lookPos, Vector3.up);
            float eulerY = lookRot.eulerAngles.y;
            Quaternion rotation = Quaternion.Euler(0, eulerY, 0);
            viewer.rotation = rotation;
        }

        void LookAtX(Transform target, Transform viewer)
        {
            Vector3 lookPos = target.position - viewer.transform.position;
            Quaternion lookRot = Quaternion.LookRotation(lookPos, Vector3.up);
            float eulerX = lookRot.eulerAngles.x;
            Quaternion rotation = Quaternion.Euler(eulerX, 0, 0);
            viewer.rotation = rotation;
        }

        private void OnCollisionEnter(Collision collision)
        {
            if (collision.gameObject.CompareTag("Plane"))
            {
                Debug.Log("TowerActiv");
                _animator.SetBool("OpenHatch", true);
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
