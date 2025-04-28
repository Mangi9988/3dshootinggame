using UnityEngine;

public class Knife : IWeapon
{
    private PlayerWeaponData _data;
    private Transform _attackOrigin;
    private GameObject _owner;

    private float _attackCooldownRemaining;
    private bool _isReloading;
    private float _reloadTimer;

    private float _attackRange;
    private float _attackAngle;

    public Knife(PlayerWeaponData data, Transform attackOrigin, GameObject owner)
    {
        _data = data;
        _attackOrigin = attackOrigin;
        _owner = owner;

        _attackCooldownRemaining = 0f;
        _attackRange = _data.Range;
        _attackAngle = _data.Angle;
    }

    public void Update()
    {
        if (_attackCooldownRemaining > 0f)
        {
            _attackCooldownRemaining -= Time.deltaTime;
            if (_attackCooldownRemaining < 0f)
                _attackCooldownRemaining = 0f;
        }

        Reloading();
    }

    public void Attack()
    {
        if (_isReloading)
        {
            CancelReload();
        }

        if (_attackCooldownRemaining > 0f)
            return;

        Fire();
    }

    private void Fire()
    {
        _attackCooldownRemaining = _data.FireCooldown;

        Collider[] hitColliders = Physics.OverlapSphere(_attackOrigin.position, _attackRange);
        foreach (var hitCollider in hitColliders)
        {
            if (hitCollider.TryGetComponent<IDamageable>(out IDamageable damageable))
            {
                Vector3 toTarget = (hitCollider.transform.position - _attackOrigin.position).normalized;
                float angle = Vector3.Angle(_attackOrigin.forward, toTarget);

                if (angle <= _attackAngle * 0.5f)
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

    public void StartReload()
    {
        if (_isReloading) return;

        _isReloading = true;
        _reloadTimer = 0f;
    }

    private void Reloading()
    {
        if (_isReloading)
        {
            _reloadTimer += Time.deltaTime;
            if (_reloadTimer >= _data.ReloadTime)
            {
                _isReloading = false;
            }
        }
    }

    public void CancelReload()
    {
        if (_isReloading)
        {
            _isReloading = false;
            _reloadTimer = 0f;
        }
    }
}
