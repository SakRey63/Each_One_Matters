using UnityEngine;

public class PositionGenerationBase : MonoBehaviour
{
    [SerializeField] private float _stepOffset = 1f; 
    [SerializeField] private int _maxCountToLine = 11;
    
    private int _countPoliceOfficerToLine;
    private Vector3 _nexPositionSpawn;
    private Vector3 _startPositionToBase;
    private Vector3 _lastPositionToBaseRight;
    private Vector3 _lastPositionToBaseLeft;
    private bool _isStart = true;
    private bool _isRight;
    private bool _isLeft;
    private bool _isMovingDown;
    
    public Vector3 GetPositionOnBase(Vector3 startPositionToBase)
    {
        if (_isStart)
        {
            _startPositionToBase = startPositionToBase;
            
            _nexPositionSpawn = _startPositionToBase;
            _countPoliceOfficerToLine++;
            _isLeft = true;
            _isStart = false;
        }
        else if (_isLeft)
        {
            _nexPositionSpawn = new Vector3(_lastPositionToBaseLeft.x + _stepOffset, _nexPositionSpawn.y,
                _nexPositionSpawn.z);
            
            _lastPositionToBaseLeft = _nexPositionSpawn;
            _countPoliceOfficerToLine++;
            _isRight = true;
            _isLeft = false;
        }
        else if (_isRight)
        {
            _nexPositionSpawn = new Vector3(_lastPositionToBaseRight.x - _stepOffset, _nexPositionSpawn.y,
                _nexPositionSpawn.z);
            
            _lastPositionToBaseRight = _nexPositionSpawn;
            _countPoliceOfficerToLine++;
            _isRight = false;
            _isLeft = true;
        }
        else if(_isMovingDown)
        {
            _nexPositionSpawn = new Vector3(_startPositionToBase.x, _nexPositionSpawn.y,
                _nexPositionSpawn.z + _stepOffset);
            
            _countPoliceOfficerToLine++;
            _isLeft = true;
            _isMovingDown = false;
        }
        
        if (_countPoliceOfficerToLine >= _maxCountToLine)
        {
            _lastPositionToBaseRight = _startPositionToBase;
            _lastPositionToBaseLeft = _startPositionToBase;
            _countPoliceOfficerToLine = 0;
            _isMovingDown = true;
            _isRight = false;
            _isLeft = false;
        }

        return _nexPositionSpawn;
    }
}
