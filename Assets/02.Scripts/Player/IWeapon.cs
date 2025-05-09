using UnityEngine;

public interface IWeapon
{
    public void Attack();
    public void Update();
    bool IsAttackAvailable { get; }
}

public interface IThrowableWeapon : IWeapon
{
    public void Throw();
}