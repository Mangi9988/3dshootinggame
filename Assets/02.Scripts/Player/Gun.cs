using UnityEngine;

public class Gun : IWeapon
{
    private PlayerWeaponData _data;
    private Transform _firePosition;
    private GameObject _owner;
    
    private int _currentBulletCount;
    private float _fireCooldown;
    private bool _isReloading;
    private float _reloadTimer;
    
    public int CurrentBulletCount => _currentBulletCount;
    public int MaxBulletCount => _data.MaxBulletCount;
    public bool IsReloading => _isReloading;
    public float ReloadProgress => _isReloading ? _reloadTimer / _data.ReloadTime : 0f;
    
    public Gun(PlayerWeaponData data, Transform firePosition, GameObject owner)
    {
        _data = data;
        _firePosition = firePosition;
        _owner = owner;
        
        _currentBulletCount = _data.MaxBulletCount;
    }
    
    public void Update()
    {
        if (_fireCooldown > 0f)
        {
            _fireCooldown -= Time.deltaTime;
            if (_fireCooldown < 0f)
            {
                _fireCooldown = 0f;
            }
        }

        Reloading();
    }

    
    public void Attack()
    {
        if (_isReloading)
        {
            CancelReload();
        }

        if (_fireCooldown > 0f)
            return;

        if (_currentBulletCount <= 0)
        {
            StartReload();
            return;
        }

        Fire();
    }

    public void Fire()
    {
        _currentBulletCount--;
        _fireCooldown =  _data.FireCooldown;
        
        Ray ray = new Ray(Camera.main.transform.position, Camera.main.transform.forward);
        RaycastHit hitInfo = new RaycastHit();
        bool isHit = Physics.Raycast(ray, out hitInfo);
        if (isHit)
        {
            BulletParticle bulletParticle = GameManager.Instance.PoolManager.GetFromPool<BulletParticle>();

            Transform transform = bulletParticle.transform;
            transform.position = hitInfo.point;
            transform.rotation = Quaternion.LookRotation(hitInfo.normal);


            if (hitInfo.collider.TryGetComponent<IDamageable>(out IDamageable damageable))
            {
                Damage damage = new Damage
                {
                    Value          = _data.Damage,
                    From           = _owner,
                    KnockbackForce = _data.BulletKnockbackForce
                };
                damageable.TakeDamage(damage);
            }
        }
    }


    public void StartReload()
    {
        if (_isReloading) return;
        
        _isReloading = true;
        _reloadTimer = 0f;
    }
    
    
    public void Reloading()
    {
        if (_isReloading)
        {
            _reloadTimer += Time.deltaTime;
            if (_reloadTimer >= _data.ReloadTime)
            {
                _isReloading = false;
                _currentBulletCount = _data.MaxBulletCount;
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
