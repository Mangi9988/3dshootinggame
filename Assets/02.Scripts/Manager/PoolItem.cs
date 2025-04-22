using System;
using Redcode.Pools;
using UnityEngine;
public class PoolItem : MonoBehaviour, IPoolObject
{
    public void OnCreatedInPool()
    {
        
    }
    public void OnGettingFromPool()
    {
        
    }

    public void ReturnToPoolAs<T>() where T : Component
    {
        GameManager.Instance.PoolManager.TakeToPool<T>(GetComponent<T>());

        gameObject.SetActive(false);
    }
}