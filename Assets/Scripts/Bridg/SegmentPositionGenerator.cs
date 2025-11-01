using UnityEngine;

public class SegmentPositionGenerator : MonoBehaviour
{
    [SerializeField] private float _sectionOffset = 5f;
    [SerializeField] private float _angleRotate = 90f;
    [SerializeField] private float _monsterRightOffset = 3.5f;
    [SerializeField] private float _monsterLeftOffset = 13.5f;
    [SerializeField] private Transform _startPositionAllTriggers;

    private Transform _startPositionBridgeSegments;
    private bool _isTurnedRight;
    private bool _isHorizontal;
    private bool _isMonsterPositionRight;
    private bool _isFirstTurnedRight;
    private int _maxIndexExclusive = 3;
    private int _maxRandomValue = 2;
    private int _rotationCount;
    private BridgeDirection _currentDirection;

    public Transform StartPositionBridgeSegments => _startPositionBridgeSegments;
    public bool IsMonsterPositionRight => _isMonsterPositionRight;
    public bool IsFirstTurnedRight => _isFirstTurnedRight;

    public bool ToggleMovementDirection()
    {
        _isHorizontal = !_isHorizontal;

        if (_rotationCount == 0)
        {
            int randomNumber = GetRandomIndex(0, _maxRandomValue);
            
                
            if (randomNumber > 0)
            {
                _isTurnedRight = true;
            }
            else
            {
                _isTurnedRight = false;
            }

            _isFirstTurnedRight = _isTurnedRight;
            _rotationCount++;
        }
        else
        {
            _isTurnedRight = !_isTurnedRight;
        }

        return _isTurnedRight;
    }
    
    public Vector3 GetNextPositionAlongLength(Vector3 nextPosition)
    {
        if (_isHorizontal)
        {
            if (_isTurnedRight)
            {
                nextPosition = new Vector3(nextPosition.x + _sectionOffset, nextPosition.y, nextPosition.z );
            }
            else
            {
                nextPosition = new Vector3(nextPosition.x - _sectionOffset , nextPosition.y, nextPosition.z);
            }
        }
        else
        {
            nextPosition = new Vector3(nextPosition.x, nextPosition.y, nextPosition.z + _sectionOffset);
        }

        return nextPosition;
    }
    
    public float GetAngelAndCreateNextStartPositionBridgeSegment(float currentYAngle, Transform targetRight, Transform targetLeft)
    {
        if (_isTurnedRight)
        {
            _startPositionBridgeSegments = targetRight;
            currentYAngle += _angleRotate;
        }
        else
        {
            _startPositionBridgeSegments = targetLeft;
            currentYAngle -= _angleRotate;
        }

        return currentYAngle;
    }
    
    public Vector3 GetNextPositionAlongWidth(Vector3 position)
    {
        if (_isHorizontal)
        {
            if (_isTurnedRight)
            {
                position = new Vector3(position.x, position.y, position.z - _sectionOffset);
            }
            else
            {
                position = new Vector3(position.x, position.y, position.z + _sectionOffset);
            }
        }
        else
        {
            position = new Vector3(position.x + _sectionOffset, position.y, position.z);
        }

        return position;
    }

    public Vector3 GetPositionToBaseOfConnector(Vector3 nextPosition, float offset)
    {
        if (_isHorizontal)
        {
            if (_isTurnedRight)
            {
                nextPosition = new Vector3(nextPosition.x + offset, _startPositionAllTriggers.position.y,
                    nextPosition.z - _sectionOffset);
            }
            else
            {
                nextPosition = new Vector3(nextPosition.x - offset, _startPositionAllTriggers.position.y,
                    nextPosition.z + _sectionOffset);
            }
        }
        else
        {
            nextPosition = new Vector3(nextPosition.x + _sectionOffset, _startPositionAllTriggers.position.y, nextPosition.z + offset);
        }

        return nextPosition;
    }

    public Vector3 GetRandomPositionToLevel(Vector3 position)
    {
        float randomSectionOffset = _sectionOffset * GetRandomIndex(0, _maxIndexExclusive);

        if (_isHorizontal)
        {
            if (_isTurnedRight)
            {
                position = new Vector3(position.x , position.y, position.z - randomSectionOffset);
            }
            else
            {
                position = new Vector3(position.x , position.y, position.z + randomSectionOffset);
            }
        }
        else
        {
            position = new Vector3(position.x + randomSectionOffset, position.y, position.z );
        }
        
        return position;
    }

    public void CreateRandomSide()
    {
        int randomMonsterPosition = GetRandomIndex(0, _maxRandomValue);

        _isMonsterPositionRight = randomMonsterPosition > 0;
    }
    
    public Vector3 GetObstaclePosition(Vector3 basePosition)
    {
        if (_isHorizontal)
        {
            float zOffset = _isMonsterPositionRight ? _monsterRightOffset : -_monsterLeftOffset;
            if (!_isTurnedRight) zOffset *= -1;

            return new Vector3(basePosition.x, basePosition.y, basePosition.z + zOffset);
        }
        else
        {
            float xOffset = _isMonsterPositionRight ? -_monsterRightOffset : _monsterLeftOffset;
            return new Vector3(basePosition.x + xOffset, basePosition.y, basePosition.z);
        }
    }
    
    public int GetIndexNumberPosition()
    {
        return GetRandomIndex(0, _maxIndexExclusive);
    }
    
    private int GetRandomIndex(int minValue, int maxValue)
    {
        return Random.Range(minValue, maxValue);
    }
}