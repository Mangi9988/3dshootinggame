using UnityEngine;

public class PlayerCameraSwitcher : MonoBehaviour
{
    private bool _isFirstPerson = true;

    // private void Update()
    // {
    //     if (Input.GetKeyDown(KeyCode.V))
    //     {
    //         if (_isFirstPerson)
    //         {
    //             CameraFollow.Instance.SetToThirdPerson();
    //         }
    //         else
    //         {
    //             CameraFollow.Instance.SetToFirstPerson();
    //         }
    //         _isFirstPerson = !_isFirstPerson;
    //     }
    // }
}
