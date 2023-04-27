using UnityEngine;
using Photon.Pun;

namespace Hackathon
{
    public class NetworkPlayer : MonoBehaviour, IPunObservable, IPunInstantiateMagicCallback
    {
        public bool createAvatars;
        public GameObject headPrefab, leftPrefab, rightPrefab;
        private Transform head, right, left, body;
        private PhotonView photonView;

        private void Awake()
        {
            photonView = GetComponent<PhotonView>();
        }

        private void Start()
        {
            if (!photonView.IsMine && createAvatars)
            {
                body = new GameObject("Player" + photonView.CreatorActorNr).transform;
                head = headPrefab == null
                    ? new GameObject("head").transform
                    : Instantiate(headPrefab, Vector3.zero, Quaternion.identity).transform;
                right = rightPrefab == null
                    ? new GameObject("right").transform
                    : Instantiate(rightPrefab, Vector3.zero, Quaternion.identity).transform;
                left = leftPrefab == null
                    ? new GameObject("left").transform
                    : Instantiate(leftPrefab, Vector3.zero, Quaternion.identity).transform;
                head.SetParent(body);
                right.SetParent(body);
                left.SetParent(body);
            }
        }

        public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
        {
            if (stream.IsWriting)
            {
                stream.SendNext(GameManager.Instance.playerHead.position);
                stream.SendNext(GameManager.Instance.playerHead.eulerAngles);
                stream.SendNext(GameManager.Instance.leftHand.position);
                stream.SendNext(GameManager.Instance.leftHand.eulerAngles);
                stream.SendNext(GameManager.Instance.rightHand.position);
                stream.SendNext(GameManager.Instance.rightHand.eulerAngles);
            }
            else
            {
                head.position = (Vector3)stream.ReceiveNext();
                head.eulerAngles = (Vector3)stream.ReceiveNext();
                left.position = (Vector3)stream.ReceiveNext();
                left.eulerAngles = (Vector3)stream.ReceiveNext();
                right.position = (Vector3)stream.ReceiveNext();
                right.eulerAngles = (Vector3)stream.ReceiveNext();
            }
        }

        private void OnDisable()
        {
            if (body != null) Destroy(body.gameObject);
        }

        public void OnPhotonInstantiate(PhotonMessageInfo info)
        {
            if (!photonView.IsMine) GameManager.Instance.remotePlayer = GetComponent<PlayerBehaviour>();
        }
    }
}