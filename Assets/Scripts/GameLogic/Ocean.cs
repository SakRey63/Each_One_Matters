using UnityEngine;

public class Ocean : MonoBehaviour
{
    private Transform _transform;

    private void Awake()
    {
        _transform = transform;
    }

    public void UpdateOceanPositionRelativeToPlayer(Transform player)
    {
        Vector3 position = new Vector3(player.position.x, _transform.position.y, player.position.z);

        _transform.position = position;
    }
}