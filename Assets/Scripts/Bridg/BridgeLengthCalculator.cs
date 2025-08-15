using System;
using UnityEngine;

public class BridgeLengthCalculator : MonoBehaviour
{
    [SerializeField] private int _maxLevelsBeforeTurn = 5;
    [SerializeField] private int _lengthIncrementPerLevel = 20;
    [SerializeField] private int _baseLengthBridge = 100;
    [SerializeField] private int _maxLengthBridge = 200;
    
    private int _stageNumber;
    private int _lengthBridge;
    private int _countBridgeConnector;
    
    public int SpanCount { get; private set; }

    public int LenghtBridge => _lengthBridge;
    
    public void CalculateLengthBridge(int levelPlayer)
    {
        SpanCount = (int)Math.Ceiling((double)levelPlayer / _maxLevelsBeforeTurn);

        _stageNumber = levelPlayer % _maxLevelsBeforeTurn;

        if (_stageNumber == 0)
        { 
            _lengthBridge = _maxLengthBridge;
        }
        else
        {
            _lengthBridge = _baseLengthBridge;
                            
            for (int i = 0; i < _stageNumber; i++)
            { 
                _lengthBridge += _lengthIncrementPerLevel; 
            }
        }
    }
}
