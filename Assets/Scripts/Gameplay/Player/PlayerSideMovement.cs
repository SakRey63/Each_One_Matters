using UnityEngine;
using YG;

namespace EachOneMatters.Gameplay.Player
{
    public class PlayerSideMovement : MonoBehaviour
    {
        [SerializeField] private float _borderX = 5.7f;
        [SerializeField] private Transform _transformSquad;
    
        private float _speed;
        private float _targetPositionX;

        public Transform SquadPosition => _transformSquad;

        private void Start()
        {
            _speed = YG2.saves.settings.SpeedSideMovement;
        }

        public void Move(float direction)
        {
            Vector3 position = _transformSquad.localPosition;
            position.x = Mathf.Clamp(position.x + (-direction) * _speed * Time.deltaTime, -_borderX, _borderX);
            _transformSquad.localPosition = position;
        }

        public void ResetPositionSquad()
        {
            _transformSquad.localPosition = Vector3.zero;
        }
    }   
}