using UnityEngine;

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

        private GameState _gameState;

        private void Start()
        {
            _gameState = GameState.Lobby;
        }

        public void StartGame()
        {
            _gameState = GameState.InGame;
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