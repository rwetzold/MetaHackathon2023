using System.Collections;
using System.Linq;
using Meta.WitAi.TTS.Utilities;
using Photon.Pun;
using TMPro;
using UnityEngine;
using UnityEngine.Events;

namespace Hackathon
{
    public class GameManager : Singleton<GameManager>
    {
        public enum GameState
        {
            Lobby,
            InGame,
            GameOver
        }

        [Header("Static references")] public Transform playerHead;
        public Transform leftHand;
        public Transform rightHand;
        public TextMeshPro countdown;
        public TTSSpeaker tts;

        [Header("Runtime")] public PlayerBehaviour remotePlayer;

        [Header("Events")] public UnityEvent OnGameStart;
        [Header("Events")] public UnityEvent OnGameOver;
        [Header("Events")] public UnityEvent OnGameWon;
        [Header("Events")] public UnityEvent OnGameLost;

        private GameState _gameState;

        private void Start()
        {
            _gameState = GameState.Lobby;

            countdown.gameObject.SetActive(true);
            countdown.text = "Waiting for other player...";
        }

        public void StartGame()
        {
            StartCoroutine(StartCountdown());
        }

        private void SetGameOver()
        {
            _gameState = GameState.GameOver;

            FindObjectsByType<SpaceshipBehaviour>(FindObjectsInactive.Include, FindObjectsSortMode.None).ToList()
                .ForEach(ship => { ship.KillIt(); });

            OnGameOver?.Invoke();
            countdown.gameObject.SetActive(true);
        }

        [ContextMenu("Set game lost")]
        public void SetGameLost()
        {
            SetGameOver();
            OnGameLost?.Invoke();
            NetworkManager.Instance.SendLocalPlayerLost();

            countdown.text = "You Lost";
        }

        [ContextMenu("Set game won")]
        public void SetGameWon()
        {
            SetGameOver();
            OnGameWon?.Invoke();
            countdown.text = "You won";
        }

        private IEnumerator StartCountdown()
        {
            tts.Speak("Get ready " + PhotonNetwork.NickName + ". Starting Game!");
            countdown.gameObject.SetActive(true);
            for (int i = 3; i >= 0; i--)
            {
                countdown.text = i == 0 ? "GO!" : i.ToString();
                yield return new WaitForSeconds(1f);
            }

            countdown.gameObject.SetActive(false);

            _gameState = GameState.InGame;
            OnGameStart?.Invoke();
        }

        public void CreateTowerCommand()
        {
            Debug.Log("Create Tower");
        }

        public void ModifyTowerCommand()
        {
            Debug.Log("Modify Tower");
        }

        public void DeleteTowerCommand()
        {
            Debug.Log("Delete Tower");
        }

        public void CreateFactoryCommand()
        {
            Debug.Log("Create Factory");
        }

        public void Speak(string text)
        {
            tts.Speak(text);
        }
    }
}