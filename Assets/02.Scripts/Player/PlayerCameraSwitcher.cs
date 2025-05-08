using UnityEngine;

public class PlayerCameraSwitcher : MonoBehaviour
{
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha8))
        {
            CameraFollow.Instance.SetToFirstPerson();
            SetLayerVisibility(fpsVisible: true);
        }
        else if (Input.GetKeyDown(KeyCode.Alpha9))
        {
            CameraFollow.Instance.SetToThirdPerson();
            SetLayerVisibility(fpsVisible: false);
        }
        else if (Input.GetKeyDown(KeyCode.Alpha0))
        {
            CameraFollow.Instance.SetToQuarterView();
            SetLayerVisibility(fpsVisible: false);
        }
    }
    
    private void SetLayerVisibility(bool fpsVisible)
    {
        Camera camera = Camera.main;
        
        int fpsLayer = LayerMask.NameToLayer("FPSPlayer");
        int playerLayer = LayerMask.NameToLayer("Player");

        int fpsMask = 1 << fpsLayer;
        int playerMask = 1 << playerLayer;

        if (fpsVisible)
        {
            camera.cullingMask |= fpsMask;
            camera.cullingMask &= ~playerMask;
        }
        else
        {
            camera.cullingMask |= playerMask;
            camera.cullingMask &= ~fpsMask;
        }
    }
}
