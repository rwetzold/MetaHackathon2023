using System;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;

namespace Hackathon
{
    public class NetworkManager : MonoBehaviourPunCallbacks
    {
        private const string ROOM_NAME = "towAR defense";
        private const int UuidSize = 16;

        public enum NetworkState
        {
            Idle,
            Connecting,
            Connected,
            InRoom,
            Disconnected
        }

        public event Action<NetworkState> OnNetworkState;
        public event Action OnPlayersReady;

        private void Start()
        {
            OnNetworkState?.Invoke(NetworkState.Connecting);
            PhotonNetwork.ConnectUsingSettings();
        }

        private void OnApplicationPause(bool pause)
        {
            if (!pause)
            {
                if (PhotonNetwork.IsConnected)
                {
                    Debug.Log("Application Un-paused: Attempting to reconnect and rejoin a Photon room");
                    PhotonNetwork.ReconnectAndRejoin();
                }
                else
                {
                    Debug.Log("Application Un-paused: Connecting to a Photon server. Please join or create a room.");
                    PhotonNetwork.ConnectUsingSettings();
                }
            }
        }

        public override void OnConnectedToMaster()
        {
            Debug.Log("Photon::OnConnectedToMaster: successfully connected to photon: " + PhotonNetwork.CloudRegion);

            PhotonNetwork.JoinLobby();
            OnNetworkState?.Invoke(NetworkState.Connected);
        }

        public override void OnDisconnected(DisconnectCause cause)
        {
            Debug.Log("Photon::OnDisconnected: failed to connect: " + cause);
            OnNetworkState?.Invoke(NetworkState.Disconnected);

            if (cause != DisconnectCause.DisconnectByClientLogic)
            {
                OnNetworkState?.Invoke(NetworkState.Connecting);
                PhotonNetwork.ReconnectAndRejoin();
            }
        }

        public override void OnJoinRoomFailed(short returnCode, string message)
        {
            Debug.Log("Photon::OnJoinRoomFailed: " + message);
        }

        public override void OnJoinedRoom()
        {
            Debug.Log("Photon::OnJoinedRoom: joined room: " + PhotonNetwork.CurrentRoom.Name);
            PhotonNetwork.Instantiate("NetworkPlayer", Vector3.zero, Quaternion.identity);

            GameObject sceneCaptureController = GameObject.Find("SceneCaptureController");
            if (sceneCaptureController)
            {
                if (PhotonNetwork.IsMasterClient)
                {
                    // sceneCaptureController.GetComponent<SceneApiSceneCaptureStrategy>().InitSceneCapture();
                    // sceneCaptureController.GetComponent<SceneApiSceneCaptureStrategy>().BeginCaptureScene();
                }
                else
                {
                    LoadRoomFromProperties();
                }
            }

            OnNetworkState?.Invoke(NetworkState.InRoom);
        }

        public override void OnJoinedLobby()
        {
            RoomOptions roomOptions = new() { IsVisible = true, MaxPlayers = 16, EmptyRoomTtl = 0, PlayerTtl = 300000 };

            PhotonNetwork.JoinOrCreateRoom(ROOM_NAME, roomOptions, TypedLobby.Default);
        }

        public override void OnPlayerEnteredRoom(Player newPlayer)
        {
            Debug.Log("Photon::OnPlayerEnteredRoom: new player joined room: " + newPlayer.NickName);

            // Invoke(nameof(WaitToSendAnchor), 1);
        }

        public override void OnPlayerLeftRoom(Player otherPlayer)
        {
            Debug.Log("Photon::OnPlayerLeftRoom: player left room: " + otherPlayer.NickName);
        }

        public override void OnCreatedRoom()
        {
            Debug.Log("Photon::OnCreatedRoom: created room: " + PhotonNetwork.CurrentRoom.Name);
        }

        /*
        public void PublishAnchorUuids(Guid[] uuids, uint numUuids, bool isBuffered)
        {
            SampleController.Instance.Log("PublishAnchorUuids: numUuids: " + numUuids);
    
            Array.Resize(ref _sendUuidBuffer, 1 + UuidSize * (int)numUuids);
            _sendUuidBuffer[0] = PacketFormat;
    
            var offset = 1;
            for (var i = 0; i < numUuids; i++)
            {
                PackUuid(uuids[i], _sendUuidBuffer, ref offset);
            }
    
            var rpcTarget = isBuffered ? RpcTarget.OthersBuffered : RpcTarget.Others;
            photonView.RPC(nameof(CheckForAnchorsShared), rpcTarget, _sendUuidBuffer);
        }
    
        private static void PackUuid(Guid uuid, byte[] buf, ref int offset)
        {
            Debug.Log("PackUuid: packing uuid: " + uuid);
    
            Buffer.BlockCopy(uuid.ToByteArray(), 0, buf, offset, UuidSize);
            offset += 16;
        }
    
        [PunRPC]
        private void CheckForAnchorsShared(byte[] uuidsPacket)
        {
            Debug.Log(nameof(CheckForAnchorsShared) + " : found a packet...");
    
            bool isInvalidPacketSize = uuidsPacket.Length % UuidSize != 1;
    
            if (isInvalidPacketSize)
            {
                SampleController.Instance.Log(
                    $"{nameof(CheckForAnchorsShared)}: invalid packet size: {uuidsPacket.Length} should be 1+{UuidSize}*numUuidsShared");
                return;
            }
    
            var isInvalidPacketType = uuidsPacket[0] != PacketFormat;
    
            if (isInvalidPacketType)
            {
                SampleController.Instance.Log(nameof(CheckForAnchorsShared) + " : invalid packet type: " +
                                              uuidsPacket.Length);
                return;
            }
    
            var numUuidsShared = (uuidsPacket.Length - 1) / UuidSize;
            var isEmptyUuids = numUuidsShared == 0;
    
            if (isEmptyUuids)
            {
                SampleController.Instance.Log(nameof(CheckForAnchorsShared) + " : we received a no-op packet");
                return;
            }
    
            SampleController.Instance.Log(nameof(CheckForAnchorsShared) + " : we received a valid uuid packet");
    
            var uuids = new HashSet<Guid>();
            var offset = 1;
    
            for (var i = 0; i < numUuidsShared; i++)
            {
                // We need to copy exactly 16 bytes here because Guid() expects a byte buffer sized to exactly 16 bytes
    
                Buffer.BlockCopy(uuidsPacket, offset, _getUuidBuffer, 0, UuidSize);
                offset += UuidSize;
    
                var uuid = new Guid(_getUuidBuffer);
    
                Debug.Log(nameof(CheckForAnchorsShared) + " : unpacked uuid: " + uuid);
    
                var shouldExit = uuid == _fakeUuid;
    
                if (shouldExit)
                {
                    SampleController.Instance.Log(
                        nameof(CheckForAnchorsShared) + " : received the fakeUuid/noop... exiting");
                    return;
                }
    
                uuids.Add(uuid);
            }
    
            Debug.Log(nameof(CheckForAnchorsShared) + " : set of uuids shared: " + uuids.Count);
            SharedAnchorLoader.Instance.LoadAnchorsFromRemote(uuids);
        }
    
        public override void OnRoomPropertiesUpdate(ExitGames.Client.Photon.Hashtable propertiesThatChanged)
        {
            if (propertiesThatChanged.ContainsKey(UserIdsKey))
            {
                foreach (SharedAnchor anchor in SampleController.Instance.GetLocalPlayerSharedAnchors())
                {
                    anchor.ReshareAnchor();
                }
            }
    
            object data;
            if (propertiesThatChanged.TryGetValue("roomData", out data))
            {
                SampleController.Instance.Log("Room data received from master client.");
                DeserializeToScene((byte[])data);
            }
        }
    */
        private void LoadRoomFromProperties()
        {
            if (PhotonNetwork.CurrentRoom == null)
            {
                Debug.Log("no room?");
                return;
            }

            if (PhotonNetwork.CurrentRoom.CustomProperties.TryGetValue("roomData", out object data))
            {
                DeserializeToScene((byte[])data);
            }
        }

        private void DeserializeToScene(byte[] byteData)
        {
            string jsonData = System.Text.Encoding.UTF8.GetString(byteData);
            Scene deserializedScene = JsonUtility.FromJson<Scene>(jsonData);
            if (deserializedScene != null)
            {
                Debug.Log("deserializedScene num walls: " + deserializedScene.walls.Length);
            }
            else
            {
                Debug.Log("deserializedScene is NULL");
            }

            GameObject worldGenerationController = GameObject.Find("WorldGenerationController");
            if (worldGenerationController)
                worldGenerationController.GetComponent<WorldGenerationController>().GenerateWorld(deserializedScene);
        }

        //Two users are now confirmed to be on the same anchor
        public void SessionStart()
        {
            photonView.RPC("SendSessionStart", RpcTarget.Others);
            SendSessionStart();
        }

        [PunRPC]
        public void SendSessionStart()
        {
            OnPlayersReady?.Invoke();
        }
    }
}