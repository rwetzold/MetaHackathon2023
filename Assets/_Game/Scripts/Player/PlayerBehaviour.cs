using Hackathon;
using UnityEngine;

public class PlayerBehaviour : MonoBehaviour
{
    [SerializeField] private bool isLocal;
    [SerializeField] private PlayerAttributes _playerScriptable;

    private CreatorPanel _creatorPanel;

    public PlayerAttributes playerScriptable => _playerScriptable;

    private int _currentCurrency;
    public int currentCurrency => _currentCurrency;
    private int _currentHealth;
    public int currentHealth => _currentHealth;

    private bool _healthWarningDone;

    // Start is called before the first frame update
    private void Start()
    {
        _creatorPanel = FindObjectOfType<CreatorPanel>();
        _currentCurrency = _playerScriptable.StartCurrency;
        _currentHealth = _playerScriptable.Health;
        _creatorPanel.player = this;
    }

    public bool ApplyDamage(int damage)
    {
        _currentHealth -= damage;

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