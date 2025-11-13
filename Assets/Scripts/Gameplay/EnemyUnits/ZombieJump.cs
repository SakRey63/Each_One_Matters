using System.Collections;
using UnityEngine;

namespace EachOneMatters.Gameplay.EnemyUnits
{
    public class ZombieJump : MonoBehaviour
    {
        [SerializeField] private float _jumpForce = 150f;
        [SerializeField] private  float _jumpDuration = 1f;
    
        private bool _isJumping;
    
        public void Jump()
        {
            if (_isJumping == false)
            {
                StartCoroutine(JumpRoutine());
            }
        }

        private IEnumerator JumpRoutine()
        {
            _isJumping = true;
            float elapsedTime = 0f;
        
            transform.position = new Vector3(transform.position.x, transform.position.y * _jumpForce * Time.deltaTime, transform.position.z);

            while (elapsedTime < _jumpDuration)
            {
                elapsedTime += Time.deltaTime;
                yield return null;
            }

            _isJumping = false;
        }
    }
}