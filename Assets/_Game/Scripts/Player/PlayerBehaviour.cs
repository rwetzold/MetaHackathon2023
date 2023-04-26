using Hackathon;
using UnityEngine;

public class PlayerBehaviour : MonoBehaviour
{
    [SerializeField]
    private PlayerAttributes _playerScriptable;

    private CreatorPanel _creatorPanel;

    public PlayerAttributes playerScriptable => _playerScriptable;

    private int _currentCurrency = 0;
    public int currentCurrency => _currentCurrency;
    private int _currentHealth = 0;
    public int currentHealth => _currentHealth;

    // Start is called before the first frame update
    void Start()
    {
        _creatorPanel = FindObjectOfType<CreatorPanel>();
        _currentCurrency = _playerScriptable.StartCurrency;
        _currentHealth = _playerScriptable.Health;
        _creatorPanel.player = this;
    }

    public bool ApplyDamage(int damage)
    {
        _currentHealth -= damage;

        if (_currentHealth <= 0)
        {
            // Death
            Debug.Log("Death");
            return true;
        }
        return false;
    }

    public void AddCurreny(int coins)
    {
        _currentCurrency += coins;
    }

    public bool TryPay(int price)
    {
        if (price > currentCurrency)
            return false;

        _currentCurrency -= price;
        _creatorPanel.UpdateResourceText(_currentCurrency);
        return true;
    }

}
