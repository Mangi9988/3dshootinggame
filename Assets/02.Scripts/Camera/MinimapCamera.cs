using System;
using UnityEngine;

public class MinimapCamera : MonoBehaviour
{
    public Transform Target;
    public float YOffset = 10f;

    private Camera _camera;

    private float _cameraZoomAmount = 0.5f;
    private float _cameraMaxZoomAmount = 20f;
    private float _cameraMinZoomAmount = 1f;
    
    private void Awake()
    {
        _camera = GetComponent<Camera>();
    }

    private void LateUpdate()
    {
        Vector3 newPosition = Target.position;
        newPosition.y += YOffset;
        
        transform.position = newPosition;
        
        Vector3 newEulerAngles = Target.eulerAngles;
        newEulerAngles.x = 90;
        newEulerAngles.z = 0;
        transform.eulerAngles = newEulerAngles;

        if (Input.GetKeyDown(KeyCode.Minus))
        {
            IncreaseMinimapSize();
        }

        if (Input.GetKeyDown(KeyCode.Equals))
        {
            DecreaseMinimapSize();
        }
    }

    private void IncreaseMinimapSize()
    {
        _camera.orthographicSize += _cameraZoomAmount;
        if (_camera.orthographicSize >= _cameraMaxZoomAmount)
        {
            _camera.orthographicSize = _cameraMaxZoomAmount;
        }
    }

    private void DecreaseMinimapSize()
    {
        _camera.orthographicSize -= _cameraZoomAmount;
        if (_camera.orthographicSize <= _cameraMinZoomAmount)
        {
            _camera.orthographicSize = _cameraMinZoomAmount;
        }
    }
}
