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
    private Vector3 _nextPositionSpawn;
    private SquadFormationPhase _currentState = SquadFormationPhase.Centered;

    public void ResetAllPositions()
    {
        _numberRings = 0;
        _countRings = 0;
        _currentState = SquadFormationPhase.Centered;
    }
    
   public Vector3 CreateNextSpawnPosition()
    {
        switch (_currentState)
        {
            case SquadFormationPhase.Centered:
                _nextPositionSpawn = _positionSpawnPoliceOfficer.localPosition;
                _currentState = SquadFormationPhase.ExpandingRadius;
                break;

            case SquadFormationPhase.ExpandingRadius:
                _countRings++;
                _nextPositionSpawn = new Vector3(
                    _positionSpawnPoliceOfficer.localPosition.x - (_xStepOffset * _countRings),
                    _nextPositionSpawn.y,
                    _positionSpawnPoliceOfficer.localPosition.z);
                _currentState = SquadFormationPhase.MovingUpRight;
                _numberRings = 0;
                break;

            case SquadFormationPhase.MovingUpRight:
                _numberRings++;
                _nextPositionSpawn = new Vector3(
                    _lastPositionPoliceOfficerSpawn.x + _xRowOffset,
                    _nextPositionSpawn.y,
                    _lastPositionPoliceOfficerSpawn.z + _zStepOffset);

                if (_numberRings == _countRings)
                {
                    _currentState = SquadFormationPhase.MovingRight;
                    _numberRings = 0;
                }
                break;

            case SquadFormationPhase.MovingRight:
                _numberRings++;
                _nextPositionSpawn = new Vector3(
                    _lastPositionPoliceOfficerSpawn.x + _xStepOffset,
                    _nextPositionSpawn.y,
                    _lastPositionPoliceOfficerSpawn.z);

                if (_numberRings == _countRings)
                {
                    _currentState = SquadFormationPhase.MovingDownRight;
                    _numberRings = 0;
                }
                break;

            case SquadFormationPhase.MovingDownRight:
                _numberRings++;
                _nextPositionSpawn = new Vector3(
                    _lastPositionPoliceOfficerSpawn.x + _xRowOffset,
                    _nextPositionSpawn.y,
                    _lastPositionPoliceOfficerSpawn.z - _zStepOffset);

                if (_numberRings == _countRings)
                {
                    _currentState = SquadFormationPhase.MovingDownLeft;
                    _numberRings = 0;
                }
                break;

            case SquadFormationPhase.MovingDownLeft:
                _numberRings++;
                _nextPositionSpawn = new Vector3(
                    _lastPositionPoliceOfficerSpawn.x - _xRowOffset,
                    _nextPositionSpawn.y,
                    _lastPositionPoliceOfficerSpawn.z - _zStepOffset);

                if (_numberRings == _countRings)
                {
                    _currentState = SquadFormationPhase.MovingLeft;
                    _numberRings = 0;
                }
                break;

            case SquadFormationPhase.MovingLeft:
                _numberRings++;
                _nextPositionSpawn = new Vector3(
                    _lastPositionPoliceOfficerSpawn.x - _xStepOffset,
                    _nextPositionSpawn.y,
                    _lastPositionPoliceOfficerSpawn.z);

                if (_numberRings == _countRings)
                {
                    _currentState = SquadFormationPhase.MovingUpLeft;
                    _numberRings = 0;
                }
                break;

            case SquadFormationPhase.MovingUpLeft:
                _numberRings++;

                if (_numberRings == _countRings)
                {
                    _countRings++;
                    _nextPositionSpawn = new Vector3(
                        _positionSpawnPoliceOfficer.localPosition.x - (_xStepOffset * _countRings),
                        _nextPositionSpawn.y,
                        _positionSpawnPoliceOfficer.localPosition.z);
                    _currentState = SquadFormationPhase.MovingUpRight;
                    _numberRings = 0;
                }
                else
                {
                    _nextPositionSpawn = new Vector3(
                        _lastPositionPoliceOfficerSpawn.x - _xRowOffset,
                        _nextPositionSpawn.y,
                        _lastPositionPoliceOfficerSpawn.z + _zStepOffset);
                }
                break;
        }

        _lastPositionPoliceOfficerSpawn = _nextPositionSpawn;
        return _nextPositionSpawn;
    }
}