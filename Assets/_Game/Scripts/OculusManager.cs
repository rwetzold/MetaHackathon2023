using Oculus.Platform;
using Photon.Pun;
using UnityEngine;

public class OculusManager : Singleton<OculusManager>
{
    private string _oculusUsername;
    private ulong _oculusUserId;

    private void Start()
    {
        Core.Initialize();
        Users.GetLoggedInUser().OnComplete(GetLoggedInUserCallback);
    }

    private void GetLoggedInUserCallback(Message msg)
    {
        if (msg.IsError)
        {
            Debug.LogError("GetLoggedInUserCallback: failed with error: " + msg.GetError());
            return;
        }

        Debug.Log("GetLoggedInUserCallback: success with message: " + msg + " type: " + msg.Type);

        bool isLoggedInUserMessage = msg.Type == Message.MessageType.User_GetLoggedInUser;
        if (!isLoggedInUserMessage) return;

        _oculusUsername = msg.GetUser().OculusID;
        _oculusUserId = msg.GetUser().ID;

        Debug.Log("GetLoggedInUserCallback: oculus user name: " + _oculusUsername + " oculus id: " + _oculusUserId);

        if (_oculusUserId == 0)
            Debug.Log("You are not authenticated to use this app. Shared Spatial Anchors will not work.");

        PhotonNetwork.LocalPlayer.NickName = _oculusUsername;
    }
}