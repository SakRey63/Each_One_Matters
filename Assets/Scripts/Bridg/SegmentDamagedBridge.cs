using UnityEngine;

public class SegmentDamagedBridge : MonoBehaviour, IBridgeObject
{
    [SerializeField] private Transform _centrPoint;

    private int _number;

    public BridgeObjectType Type => BridgeObjectType.DamagedSegment;
    public int NumberPosition => _number;

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

    public void SetStatus(int numberPosition)
    {
        _number = numberPosition;
    }
}
