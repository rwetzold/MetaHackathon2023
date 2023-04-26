using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpaceshipBehaviour : UnitBehaviour
{
    private GameObject _target;
    private int _currentHealth = 0;

    public OffenceAttributes offenceAttributes
    {
        get
        {
            if (attributes is OffenceAttributes)
                return (OffenceAttributes)attributes;
            else
                return ScriptableObject.CreateInstance<OffenceAttributes>();
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        _currentHealth = offenceAttributes.HealthValue;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void ApplyDamage(int damage)
    {
        _currentHealth -= damage;
        if (_currentHealth <= 0)
            Destroy(this.gameObject);
    }
}
