using UnityEngine;

public interface IWeapon
{
    public void Attack();
    public void Update();
}

public interface IThrowableWeapon : IWeapon
{
    public void Throw();
}