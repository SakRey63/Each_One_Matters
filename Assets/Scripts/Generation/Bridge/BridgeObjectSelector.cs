using UnityEngine;

public class BridgeObjectSelector : MonoBehaviour
{
    [SerializeField] private int _maxRandomValue = 2;
    [SerializeField] private int _countBridgeObstacle = 2;
    [SerializeField] private GameObject[] _bridgeObstacles;
    [SerializeField] private GameObject[] _playerUpgrades;

    private GameObject _currentSpawnObject;
    private int _randomFirstSpawnObject;
    private int _indexSpawnObject;
    
    public BridgeObjectType CurrentType { get; private set; }

    public void CreateObjectToBridge()
    {
        if (_randomFirstSpawnObject > 0)
        {
            _indexSpawnObject++;
            _currentSpawnObject = GetRandomObjectBridge(_bridgeObstacles);
           
            if (_indexSpawnObject == _countBridgeObstacle)
            {
                _randomFirstSpawnObject--;
                _indexSpawnObject = 0;
            }
        }
        else
        {
            _currentSpawnObject = GetRandomObjectBridge(_playerUpgrades);
            _randomFirstSpawnObject++;
        }

        if (_currentSpawnObject.TryGetComponent<IBridgeObject>(out var bridgeObject))
        {
            CurrentType = bridgeObject.Type;
        }
    }
    
    public void CreateFirstSpawnObject()
    {
        _randomFirstSpawnObject = GetRandomIndex(0, _maxRandomValue);
    }
    
    private GameObject GetRandomObjectBridge(GameObject[] objectsBridge)
    {
        return objectsBridge[Random.Range(0, objectsBridge.Length)];
    }
    
    private int GetRandomIndex(int minValue, int maxValue)
    {
        return Random.Range(minValue, maxValue);
    }
}
