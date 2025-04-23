using UnityEngine;

public class PlayerRotate : MonoBehaviour
{
    public float RotationSpeed = 200f; // 카메라와 회전 속도가 똑같아야 한다
    
    private float _rotationX = 0f;

    private void Update()
    {
        // 1. 마우스 입력을 받는다 ( 마우스의 커서의 움직임 방향 )
        float mouseX = Input.GetAxis("Mouse X");

        _rotationX += mouseX * RotationSpeed * Time.deltaTime;
   
        transform.eulerAngles = new Vector3(0f, _rotationX, 0f);
        
        HandleMouseSensitivity();
    }

    private void HandleMouseSensitivity()
    {
        if (Input.GetKeyDown(KeyCode.Period))
        {
            RotationSpeed += 10f;
        }
        if (Input.GetKeyDown(KeyCode.Comma))
        {
            RotationSpeed = Mathf.Max(10f, RotationSpeed - 10f); // 감도 0 이하 방지
        }
    }
}
