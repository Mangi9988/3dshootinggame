using System;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public static CameraFollow Instance;
    
    public Transform Target;
    public Transform TPSTarget;
 
    private Transform _currentTarget;
    
    public void Awake()
    {
        Instance = this;
        _currentTarget = Target;
    }
    
    private void Update()
    {
        transform.position = _currentTarget.position;
        transform.rotation = _currentTarget.rotation;
    }

    public void SetToFirstPerson()
    {
        _currentTarget = Target;
    }

    public void SetToThirdPerson()
    {
        _currentTarget = TPSTarget;
    }
}
