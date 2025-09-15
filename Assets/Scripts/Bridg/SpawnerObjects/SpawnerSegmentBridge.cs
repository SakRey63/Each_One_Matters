using System;
using UnityEngine;
using UnityEngine.Serialization;

public class SpawnerSegmentBridge : MonoBehaviour
{
    [SerializeField] private BridgeSectionCentered _sectionCentered;
    [SerializeField] private BridgeSectionEnd _sectionEnd;
    [SerializeField] private SegmentDamagedBridge _damagedBridge;
    [SerializeField] private BridgeConnector _bridgeConnector;
    [SerializeField] private Base _base;
    [SerializeField] private float _bridgeConnectorOffset = 23f;
    [SerializeField] private float _baseOffset = 13f;
    [SerializeField] private float _rotationAngle = 180;

    public event Action<BridgeConnector, Quaternion, Transform> OnBridgeConnectorSpawned;
    
    public void CreateSegmentBridge(Vector3 positionSegment, BridgeObjectSelector objectSelector, bool isHorizontal, Quaternion targetRotation, Vector3 randomPositionSection, int numberPosition)
    {
        if (objectSelector.IsDamagedSegment && isHorizontal == false && randomPositionSection.x == positionSegment.x || objectSelector.IsDamagedSegment && isHorizontal && randomPositionSection.z == positionSegment.z)
        {
            SegmentDamagedBridge damagedBridge = Instantiate(_damagedBridge, positionSegment, targetRotation);
            damagedBridge.SetStatus(numberPosition);
            objectSelector.ResetAllStats();
        }
        else
        {
            CreatedNormalSegmentBridge(positionSegment, targetRotation, numberPosition);
        }
    }

    public void SetBridgeConnectorOrFinish(int index, Vector3 nextSpawnPosition, SegmentPositionGenerator positionGenerator, Quaternion targetRotation, BridgeCheckpointStore checkpointStore, int spanCount)
    {
        int number = index;

        if (++number < spanCount)
        {
            nextSpawnPosition = positionGenerator.GetPositionToBaseOfConnector(nextSpawnPosition, _bridgeConnectorOffset);

            BridgeConnector connector = Instantiate(_bridgeConnector, nextSpawnPosition, targetRotation);
            checkpointStore.AddCheckpointAtIndex(index, connector.RotationTarget);
            positionGenerator.ToggleMovementDirection();
            connector.SetIndex(number,positionGenerator.IsTurnedRight);
            float  currentYAngle = positionGenerator.GetAngelAndCreateNextStartPositionBridgeSegment(targetRotation.eulerAngles.y, connector.BridgeStartPointRight, connector.BridgeStartPointLeft);
            Transform startPositionBridgeSegments = positionGenerator.StartPositionBridgeSegments;
            targetRotation = Quaternion.Euler(targetRotation.x, currentYAngle, targetRotation.z);
                
            OnBridgeConnectorSpawned?.Invoke(connector, targetRotation, startPositionBridgeSegments);
        }
        else
        {
            nextSpawnPosition = positionGenerator.GetPositionToBaseOfConnector(nextSpawnPosition, _baseOffset);

            Base exitBridge = Instantiate(_base, nextSpawnPosition, targetRotation);
            checkpointStore.AddCheckpointAtIndex(index, exitBridge.EndPositionPlayer);
        }
    }

    public void CreateNormalSegment(Transform transform, int number)
    {
        CreatedNormalSegmentBridge(transform.position, transform.rotation, number);
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
