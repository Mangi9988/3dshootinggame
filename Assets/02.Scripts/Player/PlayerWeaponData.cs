using System;using UnityEngine;

public enum WeaponType
{
    Ranged,
    Melee
}

[CreateAssetMenu(fileName = "PlayerWeaponData", menuName = "Scriptable Objects/PlayerWeaponData")]
public class PlayerWeaponData : ScriptableObject
{
    public string WeaponName;
    public WeaponType Type;
    public float Damage;
    public float FireCooldown;
    public float Range;
    public float Angle;
    public int MaxBulletCount;
    public float ReloadTime;
    public float BulletKnockbackForce;
}