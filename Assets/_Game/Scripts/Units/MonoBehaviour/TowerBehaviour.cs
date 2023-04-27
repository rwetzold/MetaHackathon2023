using Oculus.Interaction.HandGrab;
using Photon.Pun;
using UnityEngine;

namespace Hackathon.Commands
{
    public class TowerBehaviour : UnitBehaviour, IPunInstantiateMagicCallback
    {
        [SerializeField] private HandGrabInteractable _interactable, _inverseInteractable;

        [SerializeField] private Transform _cannonRotator;

        [SerializeField] private Transform _turret;

        [SerializeField] private GameObject _muzzleFlashes;

        [SerializeField] private Animator _animator;

        private bool _towerActiv = false;

        private AimTargetCommand _aimTarget = null;
        private float _lastShot = 0f;
        private float _muzzleFlashesTimer;
        private PhotonView _photonView;

        private void Awake()
        {
            _photonView = GetComponent<PhotonView>();
        }

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
                    LookAtY(_aimTarget.nextTarget.transform, _turret);
                    LookAtX(_aimTarget.nextTarget.transform, _cannonRotator);

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
            float oldEulerX = viewer.eulerAngles.x;
            float oldEulerZ = viewer.eulerAngles.z;

            viewer.LookAt(target /*new Vector3(viewer.position.x, target.position.y, viewer.position.z)*/);
            viewer.eulerAngles = new Vector3(oldEulerX, viewer.eulerAngles.y + 180, oldEulerZ);
        }

        void LookAtX(Transform target, Transform viewer)
        {
            float oldEulerY = viewer.eulerAngles.y;
            float oldEulerZ = viewer.eulerAngles.z;

            viewer.LookAt(target /*new Vector3(viewer.position.x, target.position.y, viewer.position.z)*/);
            viewer.eulerAngles = new Vector3(viewer.eulerAngles.y + 180, oldEulerY, oldEulerZ);
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

        public void OnPhotonInstantiate(PhotonMessageInfo info)
        {
            ownerPlayer = GameManager.Instance.remotePlayer;
        }
    }
}