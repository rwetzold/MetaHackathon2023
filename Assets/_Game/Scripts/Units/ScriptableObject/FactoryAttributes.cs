using UnityEngine;

[CreateAssetMenu(fileName = "Data", menuName = "ScriptableObjects/FactoryAttribute", order = 3)]
public class FactoryAttributes : UnitAttributes
{
    [SerializeField] private float _spawnRate;

    [SerializeField] private int _pollSize;

    [SerializeField] private Vector3 _localSpawnPosition;

    [SerializeField] private string _prefabUnit;

    // Temp
    [SerializeField] private Vector3 _targetPosition;

    public float SpawnRate => _spawnRate;
    public int PollSize => _pollSize;
    public Vector3 LocalSpawnPosition => _localSpawnPosition;
    public string PrefabUnit => _prefabUnit;
    public Vector3 TargetPosition => _targetPosition;
}