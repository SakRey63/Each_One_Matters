using UnityEngine;

public class PositionGeneratorSquad : MonoBehaviour
{
    [SerializeField] private float _xStepOffset = 1f; 
    [SerializeField] private float _zStepOffset = 1f; 
    [SerializeField] private float _xRowOffset = 0.5f;
    [SerializeField] private Transform _positionSpawnPoliceOfficer;

    private Vector3 _lastPositionPoliceOfficerSpawn;
    private int _countRings;
    private int _numberRings;
    private Vector3 _nexPositionSpawn;
    private bool _isCentered = true;
    private bool _isMovingLeft;
    private bool _isMovingRight;
    private bool _isMovingUpRight;
    private bool _isMovingUpLeft;
    private bool _isMovingDownRight;
    private bool _isMovingDownLeft;
    private bool _isExpandingRadius;

    public void ResetAllPositions()
    {
        _numberRings = 0;
        _countRings = 0;
        _isCentered = true;
        _isMovingLeft = false;
        _isMovingRight = false;
        _isMovingUpRight = false;
        _isMovingUpLeft = false;
        _isMovingDownRight = false;
        _isMovingDownLeft = false;
        _isExpandingRadius = false;
    }
    
    public Vector3 CreateNextSpawnPosition()
    {
        if (_isCentered)
        {
            _isCentered = false;
            _nexPositionSpawn = _positionSpawnPoliceOfficer.localPosition;
            _isExpandingRadius = true;
        }
        else if(_isExpandingRadius)
        {
            _countRings++;
            _nexPositionSpawn = new Vector3(_positionSpawnPoliceOfficer.localPosition.x - (_xStepOffset * _countRings), _nexPositionSpawn.y,
                _positionSpawnPoliceOfficer.localPosition.z);
            _isExpandingRadius = false;
            _isMovingUpRight = true;
            _numberRings = 0;
        }
        else if(_isMovingUpRight)
        {
            _numberRings++;
            
            _nexPositionSpawn = new Vector3(_lastPositionPoliceOfficerSpawn.x + _xRowOffset, _nexPositionSpawn.y,
                _lastPositionPoliceOfficerSpawn.z + _zStepOffset);

            if (_numberRings == _countRings)
            {
                _numberRings = 0;
                _isMovingUpRight = false;
                _isMovingRight = true;
            }
        }
        else if(_isMovingRight)
        {
            _numberRings++;
            _nexPositionSpawn = new Vector3(_lastPositionPoliceOfficerSpawn.x + _xStepOffset, _nexPositionSpawn.y,
                _lastPositionPoliceOfficerSpawn.z);
            
            if (_numberRings == _countRings)
            {
                _numberRings = 0;
                _isMovingRight = false;
                _isMovingDownRight = true;
            }
        }
        else if(_isMovingDownRight)
        {
            _numberRings++;
            _nexPositionSpawn = new Vector3(_lastPositionPoliceOfficerSpawn.x + _xRowOffset, _nexPositionSpawn.y,
                _lastPositionPoliceOfficerSpawn.z - _zStepOffset);
            
            if (_numberRings == _countRings)
            {
                _numberRings = 0;
                _isMovingDownRight = false;
                _isMovingDownLeft = true;
            }
        }
        else if(_isMovingDownLeft)
        {
            _numberRings++;
            _nexPositionSpawn = new Vector3(_lastPositionPoliceOfficerSpawn.x - _xRowOffset, _nexPositionSpawn.y,
                _lastPositionPoliceOfficerSpawn.z - _zStepOffset);
            
            if (_numberRings == _countRings)
            {
                _numberRings = 0;
                _isMovingDownLeft = false;
                _isMovingLeft = true;
            }
        }
        else if(_isMovingLeft)
        {
            _numberRings++;
            _nexPositionSpawn = new Vector3(_lastPositionPoliceOfficerSpawn.x - _xStepOffset, _nexPositionSpawn.y,
                _lastPositionPoliceOfficerSpawn.z);
            
            if (_numberRings == _countRings)
            {
                _numberRings = 0;
                _isMovingLeft = false;
                _isMovingUpLeft = true;
            }
        }
        else if (_isMovingUpLeft)
        {
            _numberRings++;
            
            if (_numberRings == _countRings)
            {
                _isMovingUpLeft = false;
                _countRings++;
                _nexPositionSpawn = new Vector3(_positionSpawnPoliceOfficer.localPosition.x - (_xStepOffset * _countRings), _nexPositionSpawn.y,
                    _positionSpawnPoliceOfficer.localPosition.z);
                _isExpandingRadius = false;
                _isMovingUpRight = true;
                _numberRings = 0;
            }
            else
            {
                _nexPositionSpawn = new Vector3(_lastPositionPoliceOfficerSpawn.x - _xRowOffset, _nexPositionSpawn.y,
                    _lastPositionPoliceOfficerSpawn.z + _zStepOffset);
            }
        }

        _lastPositionPoliceOfficerSpawn = _nexPositionSpawn;

        return _nexPositionSpawn;
    }
}
