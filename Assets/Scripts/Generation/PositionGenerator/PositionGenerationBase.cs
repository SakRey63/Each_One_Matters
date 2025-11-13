using EachOneMatters.Gameplay.Player.Class;
using UnityEngine;

namespace EachOneMatters.Generation.PositionGeneration
{
    public class PositionGenerationBase : MonoBehaviour
    {
        [SerializeField] private float _stepOffset = 1f;
        [SerializeField] private int _maxCountToLine = 11;

        private int _countPoliceOfficerToLine;
        private Vector3 _nexPositionSpawn;
        private Vector3 _startPositionToBase;
        private Vector3 _lastPositionToBaseRight;
        private Vector3 _lastPositionToBaseLeft;
        private SquadFormationPhase _squadFormationPhase = SquadFormationPhase.Centered;

        public Vector3 GetPositionOnBase(Vector3 startPositionToBase)
        {
            switch (_squadFormationPhase)
            {
                case SquadFormationPhase.Centered:
                    _startPositionToBase = startPositionToBase;
                    _nexPositionSpawn = _startPositionToBase;
                    _squadFormationPhase = SquadFormationPhase.MovingLeft;
                    break;

                case SquadFormationPhase.MovingLeft:
                    _nexPositionSpawn = new Vector3(_lastPositionToBaseLeft.x + _stepOffset, _nexPositionSpawn.y, _nexPositionSpawn.z);
                    _lastPositionToBaseLeft = _nexPositionSpawn;
                    _squadFormationPhase = SquadFormationPhase.MovingRight;
                    break;

                case SquadFormationPhase.MovingRight:
                    _nexPositionSpawn = new Vector3(_lastPositionToBaseRight.x - _stepOffset, _nexPositionSpawn.y, _nexPositionSpawn.z);
                    _lastPositionToBaseRight = _nexPositionSpawn;
                    _squadFormationPhase = SquadFormationPhase.MovingLeft;
                    break;

                case SquadFormationPhase.MovingDown:
                    _nexPositionSpawn = new Vector3(_startPositionToBase.x, _nexPositionSpawn.y, _nexPositionSpawn.z + _stepOffset);
                    _squadFormationPhase = SquadFormationPhase.MovingLeft;
                    break;
            }

            _countPoliceOfficerToLine++;

            if (_countPoliceOfficerToLine >= _maxCountToLine)
            {
                _lastPositionToBaseRight = _startPositionToBase;
                _lastPositionToBaseLeft = _startPositionToBase;
                _countPoliceOfficerToLine = 0;
                _squadFormationPhase = SquadFormationPhase.MovingDown;
            }

            return _nexPositionSpawn;
        }
    }
}