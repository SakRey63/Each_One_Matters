using System;
using UnityEngine;

public class SpawnerSegmentBridge : MonoBehaviour
{
    [SerializeField] private BridgeSectionCentered _sectionCentered;
    [SerializeField] private BridgeSectionEnd _sectionEnd;
    [SerializeField] private SegmentDamagedBridge _damagedBridge;
    [SerializeField] private BridgeConnector _bridgeConnector;
    [SerializeField] private Base _base;
    [SerializeField] private float _rotationAngle = 180;
    
    public void CreateDamageSegmentBridge(Vector3 positionSegment, Quaternion targetRotation, int numberPosition, int indexDamageSegment)
    {
        if (numberPosition == indexDamageSegment)
        {
            SegmentDamagedBridge damagedBridge = Instantiate(_damagedBridge, positionSegment, targetRotation);
                    damagedBridge.SetStatus(numberPosition);
        }
        else
        {
            CreatedNormalSegmentBridge(positionSegment, targetRotation, numberPosition);
        }
        
    }

    public BridgeConnector GetBridgeConnector(Vector3 nextSpawnPosition, Quaternion targetRotation)
    {
        BridgeConnector connector = Instantiate(_bridgeConnector, nextSpawnPosition, targetRotation);
        return connector;
    }

    public Base GetBase(Vector3 nextSpawnPosition, Quaternion targetRotation)
    {
        Base basePoliceOfficer = Instantiate(_base, nextSpawnPosition, targetRotation);
        
        return basePoliceOfficer;
    }

    public void CreateNormalSegment(Vector3 position, Quaternion rotation, int number)
    {
        CreatedNormalSegmentBridge(position, rotation, number);
    }
    
    private void CreatedNormalSegmentBridge(Vector3 positionSegment, Quaternion targetRotation, int numberPosition)
    {
        if (numberPosition == 0)
        {
            Instantiate(_sectionEnd, positionSegment, targetRotation);
        }
        else if (numberPosition == 1)
        {
            Instantiate(_sectionCentered, positionSegment, targetRotation);
        }
        else
        {
            Quaternion rotationOffset = Quaternion.Euler(0, _rotationAngle, 0);
            targetRotation *= rotationOffset;
            Instantiate(_sectionEnd, positionSegment, targetRotation);
        }
    }
}