using UnityEngine;
using UnityEngine.UI;

public class UI_Player : MonoBehaviour
{
    [Header("UI References")]
    public Image staminaGauge;
    
    private PlayerMove playerMove;

    private void Start()
    {
        playerMove = FindObjectOfType<PlayerMove>();
    }

    private void Update()
    {
        if (playerMove != null && staminaGauge != null)
        {
            // 스태미나 비율을 게이지에 반영
            staminaGauge.fillAmount = playerMove.GetStaminaRatio();
        }
    }
}
