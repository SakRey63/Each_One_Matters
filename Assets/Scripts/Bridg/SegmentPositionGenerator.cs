using UnityEngine;

public class SegmentPositionGenerator : MonoBehaviour
{
    [SerializeField] private float _sectionOffset = 5f;
    [SerializeField] private float _angleRotate = 90f;
    [SerializeField] private float _monsterRightOffset = 3.5f;
    [SerializeField] private float _monsterLeftOffset = 13.5f;
    [SerializeField] private Transform _startPositionAllTriggers;

    private Transform _startPositionBridgeSegments;
    private bool _isFirstTurn;
    private bool _isMonsterPositionRight;
    private int _maxIndexExclusive = 3;
    private int _maxRandomValue = 2;
    private int _rotationCount;
    private BridgeDirection _currentDirection = BridgeDirection.VerticalUp;
    private BridgeDirection _firstTurnDirection;
    private BridgeDirection _lastHorizontalDirection;
    private BridgeDirection _lastHorizontalRotation;
    private ObstacleSide _obstacleSide;

    public Transform StartPositionBridgeSegments => _startPositionBridgeSegments;
    public ObstacleSide Side => _obstacleSide;
    public BridgeDirection FirstTurnDirection => _firstTurnDirection;

    public BridgeDirection ToggleMovementDirection()
    {
        if (_isFirstTurn == false)
        {
            int randomNumber = GetRandomIndex(0, _maxRandomValue);
            
            _currentDirection = randomNumber > 0 ? BridgeDirection.HorizontalRight : BridgeDirection.HorizontalLeft;
            _firstTurnDirection = _currentDirection;
            _lastHorizontalDirection = _currentDirection;
            _lastHorizontalRotation = _currentDirection;
            _isFirstTurn = true;
        }
        else
        {
            _currentDirection = _currentDirection == BridgeDirection.VerticalUp 
                ? GetDirection()
                : BridgeDirection.VerticalUp;
        }

        return _lastHorizontalRotation;
    }
    
    private BridgeDirection GetDirection()
    {
        BridgeDirection tempDirection = _lastHorizontalDirection == BridgeDirection.HorizontalLeft ? BridgeDirection.HorizontalRight : BridgeDirection.HorizontalLeft;
        
        _lastHorizontalDirection = tempDirection;

        return tempDirection;
    }
    
    public Vector3 GetNextPositionAlongLength(Vector3 currentPosition)
    {
        return _currentDirection switch
        {
            BridgeDirection.HorizontalRight => new Vector3(currentPosition.x + _sectionOffset, currentPosition.y, currentPosition.z),
            BridgeDirection.HorizontalLeft   => new Vector3(currentPosition.x - _sectionOffset, currentPosition.y, currentPosition.z),
            BridgeDirection.VerticalUp       => new Vector3(currentPosition.x, currentPosition.y, currentPosition.z + _sectionOffset),
            _ => currentPosition
        };
    }
    
    public float GetAngelAndCreateNextStartPositionBridgeSegment(float currentYAngle, Transform targetRight, Transform targetLeft)
    {
        _startPositionBridgeSegments = _lastHorizontalRotation == BridgeDirection.HorizontalRight ? targetRight : targetLeft;

        if (_lastHorizontalRotation == BridgeDirection.HorizontalRight)
        {
            currentYAngle += _angleRotate;
        }
        else
        {
            currentYAngle -= _angleRotate;
        }

        UpdateRotationForNextSegment();

        return currentYAngle;
    }
    
    public Vector3 GetNextPositionAlongWidth(Vector3 currentPosition)
    {
        return _currentDirection switch
        {
            BridgeDirection.HorizontalRight => new Vector3(currentPosition.x, currentPosition.y, currentPosition.z - _sectionOffset),
            BridgeDirection.HorizontalLeft  => new Vector3(currentPosition.x, currentPosition.y, currentPosition.z + _sectionOffset),
            BridgeDirection.VerticalUp      => new Vector3(currentPosition.x + _sectionOffset, currentPosition.y, currentPosition.z),
            _ => currentPosition
        };
    }

    public Vector3 GetPositionToBaseOfConnector(Vector3 nextPosition, float offset)
    {
        return _currentDirection switch
        {
            BridgeDirection.HorizontalRight => new Vector3(
                nextPosition.x + offset, 
                _startPositionAllTriggers.position.y, 
                nextPosition.z - _sectionOffset),

            BridgeDirection.HorizontalLeft => new Vector3(
                nextPosition.x - offset, 
                _startPositionAllTriggers.position.y, 
                nextPosition.z + _sectionOffset),

            BridgeDirection.VerticalUp => new Vector3(
                nextPosition.x + _sectionOffset, 
                _startPositionAllTriggers.position.y, 
                nextPosition.z + offset),

            _ => nextPosition
        };
    }

    public Vector3 GetRandomPositionToLevel(Vector3 position)
    {
        float randomSectionOffset = _sectionOffset * Random.Range(0, _maxIndexExclusive);

        return _currentDirection switch
        {
            BridgeDirection.HorizontalRight => new Vector3(position.x, position.y, position.z - randomSectionOffset),
            BridgeDirection.HorizontalLeft  => new Vector3(position.x, position.y, position.z + randomSectionOffset),
            BridgeDirection.VerticalUp      => new Vector3(position.x + randomSectionOffset, position.y, position.z),
            _ => position
        };
    }

    public void CreateRandomSide()
    {
        int randomMonsterPosition = GetRandomIndex(0, _maxRandomValue);

        _obstacleSide = randomMonsterPosition > 0 ? ObstacleSide.Right : ObstacleSide.Left;
    }
    
    public Vector3 GetObstaclePosition(Vector3 basePosition)
    {
        return _currentDirection switch
        {
            BridgeDirection.HorizontalRight => new Vector3(
                basePosition.x, 
                basePosition.y, 
                basePosition.z + (_obstacleSide == ObstacleSide.Right ? _monsterRightOffset : -_monsterLeftOffset)),

            BridgeDirection.HorizontalLeft => new Vector3(
                basePosition.x, 
                basePosition.y, 
                basePosition.z + (_obstacleSide == ObstacleSide.Right ? -_monsterRightOffset : _monsterLeftOffset)),

            BridgeDirection.VerticalUp => new Vector3(
                basePosition.x + (_obstacleSide == ObstacleSide.Right ? -_monsterRightOffset : _monsterLeftOffset), 
                basePosition.y, 
                basePosition.z),
            
            _ => basePosition
        };
    }
    
    public int GetIndexNumberPosition()
    {
        return GetRandomIndex(0, _maxIndexExclusive);
    }
    
    private void UpdateRotationForNextSegment()
    {
        _lastHorizontalRotation = _lastHorizontalRotation == BridgeDirection.HorizontalRight ? 
                BridgeDirection.HorizontalLeft : 
                BridgeDirection.HorizontalRight;
    }
    
    private int GetRandomIndex(int minValue, int maxValue)
    {
        return Random.Range(minValue, maxValue);
    }
}