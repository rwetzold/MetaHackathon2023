using UnityEngine;

public class GameManager : Singleton<GameManager>
{
    public enum GameState
    {
        Lobby,
        InGame,
        GameOver
    }

    [SerializeField] private NetworkManager networkManager;

    private GameState _gameState;

    private void Start()
    {
        _gameState = GameState.Lobby;
        networkManager.OnPlayersReady += OnPlayersReady;
    }

    private void OnPlayersReady()
    {
        StartGame();
    }

    private void StartGame()
    {
        _gameState = GameState.InGame;
    }
}