using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

public abstract class ObjectPool<T> : MonoBehaviour where T : MonoBehaviour
{
    [SerializeField] private T _object;
    [SerializeField] private int _initialPoolSize = 20;

    private Queue<T> _objects = new Queue<T>();

    private void Awake()
    {
        InitializePool();
    }

    private void InitializePool()
    {
        for (int i = 0; i < _initialPoolSize; i++)
        {
            ExpandPool();
        }
    }

    protected T GetObject()
    {
        if (_objects.Count == 0)
        {
            ExpandPool();
        }

        T obj = _objects.Dequeue();
        obj.gameObject.SetActive(true);
        
        return obj;
    }

    protected void ReturnObject(T obj)
    {
        obj.gameObject.SetActive(false);
        _objects.Enqueue(obj);
    }

    private void ExpandPool()
    {
        T obj = Instantiate(_object, Vector3.zero, quaternion.identity);
        obj.gameObject.SetActive(false);
        _objects.Enqueue(obj);
    }
}
