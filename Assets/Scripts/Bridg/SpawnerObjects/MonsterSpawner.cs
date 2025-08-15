using UnityEngine;

public class MonsterSpawner : MonoBehaviour
{
    [SerializeField] private Monster _monster;
    [SerializeField] private float _turnAngle = 180f;
    [SerializeField] private float _zOffset = 2.5f;
    
    public void CreateAndSetupMonster(Vector3 positionMonster, BridgeObjectSelector objectSelector, SegmentPositionGenerator positionGenerator, Quaternion targetRotation)
    {
        if (objectSelector.IsMonsterActive)
        {
            float zOffset = _zOffset;

            positionMonster = positionGenerator.GetMonsterPosition(positionMonster);
            
            Quaternion rotationMonster = targetRotation;
            
            if (positionGenerator.IsMonsterPositionRight)
            {
                float currentYAngle = targetRotation.eulerAngles.y;
                currentYAngle -= _turnAngle;
                rotationMonster = Quaternion.Euler(targetRotation.eulerAngles.x, currentYAngle, targetRotation.z);
            }
            else
            {
                zOffset = -zOffset;
            } 
            
            Monster monster = Instantiate(_monster, positionMonster, rotationMonster);
            
            monster.SetPositionScannerZombie(zOffset);
            
            objectSelector.ResetAllStats();
        }
    }
}
