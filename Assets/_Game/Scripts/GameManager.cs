using System.Collections;
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

        [Header("Runtime")] public PlayerBehaviour remotePlayer;

        [Header("Events")] public UnityEvent OnGameStart;

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

        private IEnumerator StartCountdown()
        {
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
    }
}