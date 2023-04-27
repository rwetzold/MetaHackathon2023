using Hackathon;
using UnityEngine;

public class PlayerBehaviour : MonoBehaviour
{
    public bool isLocal;
    [SerializeField] private PlayerAttributes _playerScriptable;

    private CreatorPanel _creatorPanel;

    public PlayerAttributes playerScriptable => _playerScriptable;

    private int _currentCurrency;
    public int currentCurrency => _currentCurrency;
    private int _currentHealth;
    public int currentHealth => _currentHealth;

    private bool _healthWarningDone;

    private void Start()
    {
        _currentCurrency = _playerScriptable.StartCurrency;
        _currentHealth = _playerScriptable.Health;

        if (isLocal)
        {
            _creatorPanel = FindObjectOfType<CreatorPanel>();
            _creatorPanel.player = this;
            _creatorPanel.UpdatePlayerHealth(_currentHealth);
        }
    }

    public bool ApplyDamage(int damage)
    {
        if (!isLocal) return false;

        _currentHealth -= damage;
        _creatorPanel.UpdatePlayerHealth(_currentHealth);

        if (_currentHealth < 10 && !_healthWarningDone)
        {
            GameManager.Instance.Speak("Beware! Your health is running low.");
        }

        if (_currentHealth <= 0)
        {
            // Death
            GameManager.Instance.SetGameLost();
            return true;
        }

        return false;
    }

    public void AddCurrency(int coins)
    {
        if (!isLocal) return;

        _currentCurrency += coins;
    }

    public bool TryPay(int price)
    {
        if (price > currentCurrency) return false;

        _currentCurrency -= price;
        _creatorPanel.UpdateResourceText(_currentCurrency);
        return true;
    }
}