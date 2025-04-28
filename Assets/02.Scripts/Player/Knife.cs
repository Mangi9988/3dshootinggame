using UnityEngine;

public class Knife : IWeapon
{
    private PlayerWeaponData _data;
    private Transform _attackOrigin;
    private GameObject _owner;

    private float _attackCooldownRemaining;
    private LayerMask _targetMask;

    public Knife(PlayerWeaponData data, Transform attackOrigin, GameObject owner, LayerMask targetMask)
    {
        _data = data;
        _attackOrigin = attackOrigin;
        _owner = owner;
        _targetMask = targetMask;

        _attackCooldownRemaining = 0f;
    }

    public void Update()
    {
        if (_attackCooldownRemaining > 0f)
        {
            _attackCooldownRemaining -= Time.deltaTime;
            if (_attackCooldownRemaining < 0f)
                _attackCooldownRemaining = 0f;
        }
    }

    public void Attack()
    {
        if (_attackCooldownRemaining > 0f)
            return;

        Fire();
    }

    private void Fire()
    {
        Debug.Log("칼");
        _attackCooldownRemaining = _data.FireCooldown;

        Collider[] hits = Physics.OverlapSphere(_attackOrigin.position, _data.Range, _targetMask);
        foreach (var hit in hits)
        {
            Vector3 toTarget = (hit.transform.position - _attackOrigin.position).normalized;
            float angle = Vector3.Angle(_attackOrigin.forward, toTarget);

            if (angle <= _data.Angle / 2f)
            {
                if (hit.TryGetComponent<IDamageable>(out IDamageable damageable))
                {
                    Damage damage = new Damage
                    {
                        Value = _data.Damage,
                        From = _owner,
                        KnockbackForce = 0f
                    };
                    damageable.TakeDamage(damage);
                }
            }
        }
    }
}
