using UnityEngine;
using UnityEngine.UI;

public class UI_Player : MonoBehaviour
{
    [Header("UI References")]
    public Image StaminaGauge;
    public Image HealthGauge;
    
    private PlayerMove _playerMove;
    private PlayerHealth _playerHealth;

    private void Start()
    {
        _playerMove = FindObjectOfType<PlayerMove>();
        _playerHealth = FindObjectOfType<PlayerHealth>();
    }

    private void Update()
    {
            StaminaGauge.fillAmount = _playerMove.GetStaminaRatio();
            HealthGauge.fillAmount = _playerHealth.GetHealthRatio();
    }
}
