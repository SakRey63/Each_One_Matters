using UnityEngine;

public class SegmentDamagedBridge : MonoBehaviour
{
    [SerializeField] private Transform _centrPoint;

    private void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent(out PoliceOfficer policeOfficer))
        {
            if (policeOfficer.IsDead == false)
            {
                policeOfficer.SetCenterPoint(_centrPoint);
            }
        }
    }
}
