using System;
using System.Collections.Generic;
using System.Linq;
using ExitGames.Client.Photon;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;
using UnityEngine.Events;

namespace Hackathon
{
    public class NetworkManager : MonoBehaviourPunCallbacks
    {
        private const string ROOM_NAME = "towAR defense";
        private const int UuidSize = 16;
        private const string UserIdsKey = "userids";
        private const char Separator = ',';
        private const byte PacketFormat = 0;

        public enum NetworkState
        {
            Idle,
            Connecting,
            Connected,
            InRoom,
            Disconnected
        }

        public event Action<NetworkState> OnNetworkState;
        public UnityEvent OnPlayersReady;

        public static NetworkManager Instance;

        // Reusable buffer to serialize the data into
        private byte[] _sendUuidBuffer = new byte[1];
        private byte[] _getUuidBuffer = new byte[UuidSize];
        private byte[] _fakePacket = new byte[1];
        private string _oculusUsername;
        private ulong _oculusUserId;
        private Guid _fakeUuid;
        private readonly HashSet<string> _usernameList = new HashSet<string>();

        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
            }
            else
            {
                Destroy(this);
            }
        }

        private void Start()
        {
            OnNetworkState?.Invoke(NetworkState.Connecting);
            PhotonNetwork.ConnectUsingSettings();

            Array.Resize(ref _fakePacket, 1 + UuidSize);
            _fakePacket[0] = PacketFormat;

            int offset = 1;
            byte[] fakeBytes = new byte[]
                { 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff };

            _fakeUuid = new Guid(fakeBytes);
            PackUuid(_fakeUuid, _fakePacket, ref offset);
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

            AddUserToUserListState(_oculusUserId);

            foreach (Player player in PhotonNetwork.CurrentRoom.Players.Values)
            {
                AddToUsernameList(player.NickName);
            }

            PhotonNetwork.Instantiate("NetworkPlayer", Vector3.zero, Quaternion.identity);

            GameObject sceneCaptureController = GameObject.Find("SceneModel");
            if (sceneCaptureController)
            {
                if (PhotonNetwork.IsMasterClient)
                {
                    sceneCaptureController.GetComponent<SceneApiSceneCaptureStrategy>().LoadRoomLayout();
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

            AddToUsernameList(newPlayer.NickName);

            if (SampleController.Instance.automaticCoLocation)
            {
                Invoke(nameof(WaitToSendAnchor), 1);
            }
            else if (SampleController.Instance.cachedAnchorSample)
            {
                Invoke(nameof(WaitToReshareAnchor), 1);
            }
        }

        [ContextMenu("Send Anchor")]
        private void WaitToSendAnchor()
        {
            SampleController.Instance.colocationAnchor.OnShareButtonPressed();
        }

        private void WaitToReshareAnchor()
        {
            if (SampleController.Instance.colocationCachedAnchor != null)
            {
                SampleController.Instance.colocationCachedAnchor.ReshareAnchor();
            }
        }

        public override void OnPlayerLeftRoom(Player otherPlayer)
        {
            Debug.Log("Photon::OnPlayerLeftRoom: player left room: " + otherPlayer.NickName);
        }

        public override void OnCreatedRoom()
        {
            Debug.Log("Photon::OnCreatedRoom: created room: " + PhotonNetwork.CurrentRoom.Name);
        }

        public void PublishAnchorUuids(Guid[] uuids, uint numUuids, bool isBuffered)
        {
            Debug.Log("PublishAnchorUuids: numUuids: " + numUuids);

            Array.Resize(ref _sendUuidBuffer, 1 + UuidSize * (int)numUuids);
            _sendUuidBuffer[0] = PacketFormat;

            int offset = 1;
            for (int i = 0; i < numUuids; i++)
            {
                PackUuid(uuids[i], _sendUuidBuffer, ref offset);
            }

            RpcTarget rpcTarget = isBuffered ? RpcTarget.OthersBuffered : RpcTarget.Others;
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
                Debug.Log(
                    $"{nameof(CheckForAnchorsShared)}: invalid packet size: {uuidsPacket.Length} should be 1+{UuidSize}*numUuidsShared");
                return;
            }

            bool isInvalidPacketType = uuidsPacket[0] != PacketFormat;

            if (isInvalidPacketType)
            {
                Debug.Log(nameof(CheckForAnchorsShared) + " : invalid packet type: " +
                          uuidsPacket.Length);
                return;
            }

            int numUuidsShared = (uuidsPacket.Length - 1) / UuidSize;
            bool isEmptyUuids = numUuidsShared == 0;

            if (isEmptyUuids)
            {
                Debug.Log(nameof(CheckForAnchorsShared) + " : we received a no-op packet");
                return;
            }

            Debug.Log(nameof(CheckForAnchorsShared) + " : we received a valid uuid packet");

            HashSet<Guid> uuids = new HashSet<Guid>();
            int offset = 1;

            for (int i = 0; i < numUuidsShared; i++)
            {
                // We need to copy exactly 16 bytes here because Guid() expects a byte buffer sized to exactly 16 bytes

                Buffer.BlockCopy(uuidsPacket, offset, _getUuidBuffer, 0, UuidSize);
                offset += UuidSize;

                Guid uuid = new Guid(_getUuidBuffer);

                Debug.Log(nameof(CheckForAnchorsShared) + " : unpacked uuid: " + uuid);

                bool shouldExit = uuid == _fakeUuid;

                if (shouldExit)
                {
                    Debug.Log(nameof(CheckForAnchorsShared) + " : received the fakeUuid/noop... exiting");
                    return;
                }

                uuids.Add(uuid);
            }

            Debug.Log(nameof(CheckForAnchorsShared) + " : set of uuids shared: " + uuids.Count);
            SharedAnchorLoader.Instance.LoadAnchorsFromRemote(uuids);
        }

        public override void OnRoomPropertiesUpdate(Hashtable propertiesThatChanged)
        {
            if (propertiesThatChanged.ContainsKey(UserIdsKey))
            {
                foreach (SharedAnchor anchor in SampleController.Instance.GetLocalPlayerSharedAnchors())
                {
                    anchor.ReshareAnchor();
                }
            }

            if (propertiesThatChanged.TryGetValue("roomData", out object data))
            {
                Debug.Log("Room data received from master client.");
                DeserializeToScene((byte[])data);
            }
        }

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

            GameObject worldGenerationController = GameObject.Find("WorldGeneration");
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

        public static HashSet<ulong> GetUserList()
        {
            if (PhotonNetwork.CurrentRoom == null ||
                !PhotonNetwork.CurrentRoom.CustomProperties.ContainsKey(UserIdsKey))
            {
                return new HashSet<ulong>();
            }

            var userListAsString = (string)PhotonNetwork.CurrentRoom.CustomProperties[UserIdsKey];
            var parsedList = userListAsString.Split(Separator).Select(ulong.Parse);

            return new HashSet<ulong>(parsedList);
        }

        private void AddUserToUserListState(ulong userId)
        {
            var userList = GetUserList();
            var isKnownUserId = userList.Contains(userId);

            if (isKnownUserId)
            {
                return;
            }

            userList.Add(userId);
            SaveUserList(userList);
        }

        public void RemoveUserFromUserListState(ulong userId)
        {
            HashSet<ulong> userList = GetUserList();
            bool isUnknownUserId = !userList.Contains(userId);

            if (isUnknownUserId)
            {
                return;
            }

            userList.Remove(userId);
            SaveUserList(userList);
        }

        private static void SaveUserList(HashSet<ulong> userList)
        {
            string userListAsString = string.Join(Separator.ToString(), userList);
            Hashtable setValue = new ExitGames.Client.Photon.Hashtable { { UserIdsKey, userListAsString } };

            PhotonNetwork.CurrentRoom.SetCustomProperties(setValue);
        }

        private void AddToUsernameList(string username)
        {
            bool isKnownUserName = _usernameList.Contains(username);

            if (isKnownUserName)
            {
                return;
            }

            _usernameList.Add(username);
        }

        private void RemoveFromUsernameList(string username)
        {
            var isUnknownUserName = !_usernameList.Contains(username);

            if (isUnknownUserName)
            {
                return;
            }

            _usernameList.Remove(username);
        }

        public static string[] GetUsers()
        {
            var userIdsProperty = (string)PhotonNetwork.CurrentRoom.CustomProperties[UserIdsKey];

            Debug.Log("GetUsers: " + userIdsProperty);

            var userIds = userIdsProperty.Split(',');
            return userIds;
        }
    }
}