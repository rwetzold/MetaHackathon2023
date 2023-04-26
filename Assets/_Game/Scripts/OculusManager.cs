using Oculus.Platform;
using Photon.Pun;
using UnityEngine;
using UnityEngine.Events;

namespace Hackathon
{
    public class OculusManager : Singleton<OculusManager>
    {
        public UnityEvent InitDone;
        public UnityEvent LoginDone;

        private string _oculusUsername;
        private ulong _oculusUserId;

        private void Start()
        {
            Core.AsyncInitialize().OnComplete(OnOculusPlatformInitialized);
        }

        private void OnOculusPlatformInitialized(Message<Oculus.Platform.Models.PlatformInitialize> message)
        {
            InitDone?.Invoke();
            if (message.IsError)
            {
                Debug.LogError("Failed to initialize Oculus Platform SDK: " + message.GetError());
                return;
            }

            Debug.Log("Oculus Platform SDK initialized successfully");

            Entitlements.IsUserEntitledToApplication().OnComplete(msg =>
            {
                if (msg.IsError)
                {
                    Debug.LogError("You are not entitled to use this app: " + msg.GetError().Message);
                    // return;
                }

                // launchType = ApplicationLifecycle.GetLaunchDetails().LaunchType;
                // GroupPresence.SetJoinIntentReceivedNotificationCallback(OnJoinIntentReceived);
                // GroupPresence.SetInvitationsSentNotificationCallback(OnInvitationsSent);

                Users.GetLoggedInUser().OnComplete(OnLoggedInUser);
            });
        }

        private void OnLoggedInUser(Message msg)
        {
            if (msg.IsError)
            {
                Debug.LogError("GetLoggedInUserCallback: failed with error: " + msg.GetError().Message);
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
            LoginDone?.Invoke();
        }
    }
}