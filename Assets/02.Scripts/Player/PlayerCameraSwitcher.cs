using UnityEngine;

public class PlayerCameraSwitcher : MonoBehaviour
{
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha8))
        {
            CameraFollow.Instance.SetToFirstPerson();
        }
        else if (Input.GetKeyDown(KeyCode.Alpha9))
        {
            CameraFollow.Instance.SetToThirdPerson();
        }
        
    }
}
