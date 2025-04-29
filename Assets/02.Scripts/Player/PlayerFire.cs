using System;
using UnityEngine;
using Redcode.Pools;
using TMPro;
using UnityEngine.Serialization;
using UnityEngine.UI;
public class PlayerFire : MonoBehaviour
{
    public GameObject FirePosition;
    public PlayerWeaponData GunData;
    public PlayerWeaponData KnifeData;
    public LayerMask TargetMask;
    public Image UI_SniperZoom;
    private bool _isZooming;
    
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

    /*[Header("총알 발사")]
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
    [SerializeField] private Image _reloadGaugeBar;*/

    [Header("총 UI")]
    [SerializeField] private Image _reloadGaugeBar;
    [SerializeField] private TextMeshProUGUI _bulletText;
    
    private Gun _gun;
    private Knife _knife;
    private IWeapon _currentWeapon;
    private Animator _animator;

    private void Start()
    {        
        _gun = new Gun(GunData, FirePosition.transform, gameObject);
        _knife = new Knife(KnifeData, FirePosition.transform, gameObject, TargetMask);

        _currentWeapon = _gun;
        
        _animator = GetComponentInChildren<Animator>();
        Cursor.lockState = CursorLockMode.Locked;
        _haveBombsCount = MaxBombsCount;
        _chargeGaugeBar.fillAmount = 0f;
        UpdateBombUI();
        
        _reloadGaugeBar.fillAmount = 0f;
        UpdateBulletUI();
    }

    private void Update()
    {
        HandleBombChargeStart();
        HandleBombCharging();
        HandleBombRelease();
        
        HandleWeaponSwitch();

        if (_currentWeapon == _gun)
        {
            _gun.Update();
        }
        else if (_currentWeapon == _knife)
        {
            _knife.Update();
        }

        HandleAttackInput();
        HandleReloadInput();
        
        UpdateBulletUI();
        UpdateReloadUI();
    }

    private void HandleWeaponSwitch()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            _currentWeapon = _gun;
            Debug.Log("원거리 무기 장착 (Gun)");
        }
        else if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            _currentWeapon = _knife;
            Debug.Log("근거리 무기 장착 (Sword)");
        }
    }
    
    private void HandleAttackInput()
    {
        if (Input.GetMouseButton(0))
        {
            _currentWeapon.Attack();
        }
    }
    
    private void HandleReloadInput()
    {
        if (Input.GetKeyDown(KeyCode.R))
        {
            if (_currentWeapon == _gun)
            {
                _gun.StartReload();
            }
        }
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

    /*private void UpdateFireCooldown()
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
            _animator.SetTrigger("Shot");
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
    }*/
    
    private void UpdateBulletUI()
    {
        if (_currentWeapon == _gun)
        {
            _bulletText.gameObject.SetActive(true);
            _bulletText.text = $"총알 : {_gun.CurrentBulletCount} / {_gun.MaxBulletCount}";
        }
        else
        {
            _bulletText.gameObject.SetActive(false);
        }
    }

    private void UpdateReloadUI()
    {
        if (_currentWeapon == _gun)
        {
            _reloadGaugeBar.gameObject.SetActive(true);
            _reloadGaugeBar.fillAmount = _gun.ReloadProgress;
        }
        else
        {
            _reloadGaugeBar.gameObject.SetActive(false);
        }
    }
}
