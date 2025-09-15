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

    public Transform StartPositionBridgeSegments => _startPositionBridgeSegments;
    public bool IsMonsterPositionRight => _isMonsterPositionRight;
    public bool IsHorizontal => _isHorizontal;
    public bool IsFirstTurnedRight => _isFirstTurnedRight;
    public bool IsTurnedRight => _isTurnedRight;

    public void ToggleMovementDirection()
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
    
    public Vector3 GetPositionCenterLevel(Vector3 position, float verticalPosition)
    {
        if (_isHorizontal)
        {
            if (_isTurnedRight)
            {
                position = new Vector3(position.x, verticalPosition, position.z - _sectionOffset);
            }
            else
            {
                position = new Vector3(position.x, verticalPosition, position.z + _sectionOffset);
            }
            
        }
        else
        {
            position = new Vector3(position.x + _sectionOffset, verticalPosition, position.z);

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
    
    public Vector3 GetObstaclePosition(Vector3 positionMonster)
    {
        int randomMonsterPosition = GetRandomIndex(0, _maxRandomValue);

        if (randomMonsterPosition > 0)
        {
            _isMonsterPositionRight = true;
            
            if (_isHorizontal)
            {
                if (_isTurnedRight)
                {
                    positionMonster = new Vector3(positionMonster.x , positionMonster.y, positionMonster.z + _monsterRightOffset); 
                }
                else
                {
                   positionMonster = new Vector3(positionMonster.x , positionMonster.y, positionMonster.z - _monsterRightOffset); 
                }
            }
            else
            {
                positionMonster = new Vector3(positionMonster.x - _monsterRightOffset , positionMonster.y, positionMonster.z );
            }
        }
        else
        {
            _isMonsterPositionRight = false;
            
            if (_isHorizontal)
            {
                if (_isTurnedRight)
                {
                    positionMonster = new Vector3(positionMonster.x , positionMonster.y, positionMonster.z - _monsterLeftOffset);
                }
                else
                {
                    positionMonster = new Vector3(positionMonster.x , positionMonster.y, positionMonster.z + _monsterLeftOffset);
                }
            }
            else
            {
                positionMonster = new Vector3(positionMonster.x + _monsterLeftOffset , positionMonster.y, positionMonster.z );
            }
        }
        
        return positionMonster;
    }
    
    private int GetRandomIndex(int minValue, int maxValue)
    {
        return Random.Range(minValue, maxValue);
    }
}
