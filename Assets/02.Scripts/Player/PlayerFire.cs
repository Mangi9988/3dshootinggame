using System;
using UnityEngine;
using Redcode.Pools;
using TMPro;
using UnityEngine.Serialization;
using UnityEngine.UI;
public class PlayerFire : MonoBehaviour
{
    public GameObject FirePosition;
    
    [Header("수류탄")]
    public int MaxBombsCount = 3;
    [SerializeField] private int _haveBombsCount;
    [SerializeField] private TextMeshProUGUI _bombText;
    [SerializeField] private Image _chargeGaugeBar;

    [Header("던지기")]
    [SerializeField] private float MinThrowPower = 5f;
    [SerializeField] private float MaxThrowPower = 20f;
    [SerializeField] private float MaxChargeTime = 2f;
    private float _chargeTimer;
    private bool _isCharging;

    [Header("총알 발사")]
    [SerializeField] private float FireCooldown;
    public int MaxBulletCount = 50;
    [SerializeField] private int _currentBulletCount;
    [SerializeField] private TextMeshProUGUI _bulletText;
    private float _cooldownRemaining = 0f;
    [SerializeField] private float _bulletDamage = 10;
    [SerializeField] private float _bulletKnockbackForce = 5f;
    
    [Header("재장전")]
    [SerializeField] private float ReloadTime = 2f;
    private float _reloadTimer = 0f;
    private bool _isReloading = false;
    [SerializeField] private Image _reloadGaugeBar;
    

    private void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        _haveBombsCount = MaxBombsCount;
        _chargeGaugeBar.fillAmount = 0f;
        UpdateBombUI();
        _currentBulletCount = MaxBulletCount;
        _reloadGaugeBar.fillAmount = 0f;
        UpdateBulletUI();
    }

    private void Update()
    {
        HandleBombChargeStart();
        HandleBombCharging();
        HandleBombRelease();
        UpdateFireCooldown();
        HandleBulletFire();
        StartReload();
        Reloading();
    }

    private void HandleBombChargeStart()
    {
        if (Input.GetMouseButtonDown(1))
        {
            if (_haveBombsCount <= 0) return;

            _isCharging = true;
            _chargeTimer = 0f;
        }
    }

    private void HandleBombCharging()
    {
        if (_isCharging && Input.GetMouseButton(1))
        {
            _chargeTimer += Time.deltaTime;
            _chargeTimer = Mathf.Min(_chargeTimer, MaxChargeTime);
            _chargeGaugeBar.fillAmount = _chargeTimer / MaxChargeTime;
        }
    }

    private void HandleBombRelease()
    {
        if (_isCharging && Input.GetMouseButtonUp(1))
        {
            _isCharging = false;

            float timeCalculate = _chargeTimer / MaxChargeTime;
            float finalPower = Mathf.Lerp(MinThrowPower, MaxThrowPower, timeCalculate);
            _chargeGaugeBar.fillAmount = 0f;

            ThrowBomb(finalPower);
        }
    }
    
    private void ThrowBomb(float power)
    {
        Bomb bomb = GameManager.Instance.PoolManager.GetFromPool<Bomb>();

        bomb.transform.position = FirePosition.transform.position;

        Rigidbody bombRigidbody = bomb.GetComponent<Rigidbody>();
        bombRigidbody.AddForce(Camera.main.transform.forward * power, ForceMode.Impulse);
        bombRigidbody.AddTorque(Vector3.one);

        _haveBombsCount--;
        UpdateBombUI();
    }

    private void UpdateBombUI()
    {
        if (_bombText != null)
        {
            _bombText.text = $"수류탄 : {_haveBombsCount} / {MaxBombsCount}";
        }
    }

    private void UpdateFireCooldown()
    {
        if (_cooldownRemaining < FireCooldown)
        {
            _cooldownRemaining += Time.deltaTime;
            if (_cooldownRemaining >= FireCooldown)
            {
                _cooldownRemaining = 0f;
            }
        }
    }

    private void HandleBulletFire()
    {
        if (_isReloading)
        {
            if (Input.GetMouseButtonDown(0))
            {
                _isReloading = false;
                _reloadTimer = 0f;
                _reloadGaugeBar.fillAmount = 0f;
            }
            return;
        }
        if (_isReloading)
        {
            return;
        }
        if (Input.GetMouseButton(0) && _cooldownRemaining <= 0f)
        {
            if (_currentBulletCount <= 0)
            {
                StartReload(true);
                return;
            }

            _cooldownRemaining = 0f;
            _currentBulletCount--;
            UpdateBulletUI();

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
                        Value          = _bulletDamage,
                        From           = gameObject,
                        KnockbackForce = _bulletKnockbackForce
                    };
                    damageable.TakeDamage(damage);
                }
            }
        }
    }

    private void StartReload(bool isAuto = false)
    {
        if (isAuto || Input.GetKeyDown(KeyCode.R) && _currentBulletCount < MaxBulletCount && !_isReloading)
        {
            _isReloading = true;
            _reloadTimer = 0f;
            _reloadGaugeBar.fillAmount = 0f;
        }
    }
    
    private void Reloading()
    {
        if (_isReloading)
        {
            _reloadTimer += Time.deltaTime;
            _reloadGaugeBar.fillAmount = _reloadTimer / ReloadTime;

            if (_reloadTimer >= ReloadTime)
            {
                _isReloading = false;
                _currentBulletCount = MaxBulletCount;
                UpdateBulletUI();
                _reloadGaugeBar.fillAmount = 0f;
            }
        }
    }
    
    private void UpdateBulletUI()
    { 
        _bulletText.text = $"총알 : {_currentBulletCount} / {MaxBulletCount}";
    }
}
