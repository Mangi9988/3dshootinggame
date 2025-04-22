using System;
using UnityEngine;

public class BulletParticle : MonoBehaviour
{
    private PoolItem _poolItem;

    private void Awake()
    {
        _poolItem = GetComponent<PoolItem>();
    }

    private void OnDisable()
    {
        _poolItem.ReturnToPoolAs<BulletParticle>();
    }
}
