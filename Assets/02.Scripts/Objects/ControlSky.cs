using UnityEngine;

public class ControlSky : MonoBehaviour
{
    public float MoveSpeed = 0.5f;
    private void Update()
    {
        RenderSettings.skybox.SetFloat("_Rotation", Time.time * MoveSpeed);
    }
}
