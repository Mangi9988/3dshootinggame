using System;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public Transform Target;

    private void Update()
    {
        transform.position = Target.position;
        
    }
}
