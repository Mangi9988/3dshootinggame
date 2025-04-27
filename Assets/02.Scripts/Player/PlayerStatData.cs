using UnityEngine;
using UnityEngine.Serialization;

[CreateAssetMenu(fileName = "PlayerStat", menuName = "Scriptable Objects/PlayerStat")]
public class PlayerStatData : ScriptableObject
{
    [Header("이동")]
    public float BasicMoveSpeed = 7f;
    public float SprintSpeedBonus = 3f;
    public float JumpPower = 5f;
    public int BonusJumpCount = 1;

    [Header("스태미나 및 체력")]
    public float MaxHealth = 100;
    public float MaxStamina = 100f;
    public float StaminaUseRate = 20f;
    public float StaminaRechargeRate = 10f;

    [Header("구르기")]
    public float RollSpeed = 15f;
    public float RollDuration = 0.5f;
    public float RollStaminaUseRate = 30f;

    [Header("벽타기")]
    public float WallCheckDistance = 1f;
    public float WallClimbSpeed = 5f;
    public float WallClimbStaminaCost = 30f;
}
