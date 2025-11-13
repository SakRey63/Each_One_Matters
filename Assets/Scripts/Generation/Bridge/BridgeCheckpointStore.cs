using UnityEngine;

public class BridgeCheckpointStore : MonoBehaviour
{
    private Transform[] _points;
    
    public void CreateCheckpointArray(int size)
    {
        _points = new Transform[size];
    }

    public void AddCheckpointAtIndex(int index, Transform point)
    {
        _points[index] = point;
    }

    public Transform GetCheckpointAtIndex(int indexPoint)
    {
        return _points[indexPoint];
    }
}
