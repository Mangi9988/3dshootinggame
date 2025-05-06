using System;
using UnityEngine;

public class EnemyAttackEvent : MonoBehaviour
{
    public Enemy _enemy;

    private void Start()
    {
        _enemy = GetComponentInParent<Enemy>();
    }

    public void AttackEvent()
    {
        if (_enemy.Player.TryGetComponent<IDamageable>(out IDamageable damageable))
        {
            Damage damage = new Damage
            {
                Value = _enemy.AttackDamageValue,
                From = gameObject,
                KnockbackForce = 0f
            };
            damageable.TakeDamage(damage);
        }
    }
}
