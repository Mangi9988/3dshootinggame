using System;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public static CameraFollow Instance;
    
    public Transform Target;
    private Transform _transform;
    
    public void Awake()
    {
        Instance = this;
    }

    private void Update()
    {
        transform.position = Target.position;
    }
}
