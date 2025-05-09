using System;
using UnityEngine;
using TMPro;
using UnityEngine.Serialization;
using UnityEngine.UI;
public class PlayerFire : MonoBehaviour
{
    public GameObject FirePosition;
    public PlayerWeaponData GunData;
    public PlayerWeaponData KnifeData;
    public LayerMask TargetMask;
    
    [Header("Zoom")]
    public Image UI_SniperZoom;
    public Image UI_Aim;
    private bool _isZooming;
    public float ZoomInSize  = 15f;
    public float ZoomOutSize = 60f;
    
    [Header("수류탄")]
    public int MaxBombsCount = 3;
    [SerializeField] private float MinThrowPower = 5f;
    [SerializeField] private float MaxThrowPower = 20f;
    [SerializeField] private float MaxChargeTime = 2f;
    private float _chargeTimer;
    private bool _isCharging;

    [Header("UI")]
    [SerializeField] private Image _reloadGaugeBar;
    [SerializeField] private TextMeshProUGUI _bulletText;
    [SerializeField] private Image _chargeGaugeBar;
    
    private Gun _gun;
    private Knife _knife;
    private BombThrow _bombThrow;
    private IWeapon _currentWeapon;
    private Animator _animator;

    public GameObject GunObject;
    public GameObject BatObject;
    //public GameObject BombObject;
    
    private void Start()
    {        
        _gun = new Gun(GunData, FirePosition.transform, gameObject);
        _knife = new Knife(KnifeData, FirePosition.transform, gameObject, TargetMask);
        _bombThrow = new BombThrow(MaxBombsCount, MaxChargeTime, MinThrowPower, MaxThrowPower, FirePosition.transform);
        
        _currentWeapon = _gun;
        
        _animator = GetComponentInChildren<Animator>();
        Cursor.lockState = CursorLockMode.Locked;
 
        _chargeGaugeBar.fillAmount = 0f;
        
        _reloadGaugeBar.fillAmount = 0f;
        UpdateBulletUI();
    }

    private void Update()
    {
        HandleWeaponSwitch();

        _currentWeapon.Update();

        HandleAttackInput();
        HandleChargeThrow();
        HandleReloadInput();
        HandleZoomToggle();
        
        UpdateBulletUI();
        UpdateReloadUI();
        UpdateChargeGaugeUI();
    }

    private void HandleWeaponSwitch()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            _currentWeapon = _gun;
            ExitZoom();
            GunObject.SetActive(true);
            BatObject.SetActive(false);
            //BombObject.SetActive(false);
            _animator.SetLayerWeight(3, 0f);
            _animator.SetLayerWeight(5, 0f);
            Debug.Log("원거리 무기 장착 (Gun)");
        }
        else if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            _currentWeapon = _knife;
            ExitZoom();
            GunObject.SetActive(false);
            BatObject.SetActive(true);
            //BombObject.SetActive(false);
            _animator.SetLayerWeight(3, 1f);
            _animator.SetLayerWeight(5, 0f);
            Debug.Log("근거리 무기 장착 (Sword)");
        }
        else if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            _currentWeapon = _bombThrow;
            ExitZoom();
            GunObject.SetActive(false);
            BatObject.SetActive(false);
            //BombObject.SetActive(true);
            _animator.SetLayerWeight(3, 0f);
            _animator.SetLayerWeight(5, 1f);
            Debug.Log("수류탄 장착");
        }
    }
    
    private void HandleAttackInput()
    {
        if (Input.GetMouseButton(0) && _currentWeapon.IsAttackAvailable)
        {
            _currentWeapon.Attack();
            
            if (_currentWeapon == _gun)
            {
                _animator.SetTrigger("Shot");
            }
            else if (_currentWeapon == _knife)
            {
                _animator.SetTrigger("MeleeAttack");
            }
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
    
    private void HandleZoomToggle()
    {
        if (Input.GetMouseButtonDown(1) && _currentWeapon == _gun)
        {
            _isZooming = !_isZooming;
            UI_SniperZoom.gameObject.SetActive(_isZooming);
            UI_Aim.gameObject.SetActive(!_isZooming);
            Camera.main.fieldOfView = _isZooming ? ZoomInSize : ZoomOutSize;
        }
    }
    
    private void ExitZoom()
    {
        if (_isZooming)
        {
            _isZooming = false;
            UI_SniperZoom.gameObject.SetActive(false);
            UI_Aim.gameObject.SetActive(true);
            Camera.main.fieldOfView = ZoomOutSize;
        }
    }

    private void HandleChargeThrow()
    {
        if (_currentWeapon == _bombThrow && Input.GetMouseButtonUp(0))
        {
            _animator.SetTrigger("Throw");
            (_currentWeapon as IThrowableWeapon)?.Throw();
            UpdateBulletUI();
        }
    }
    
    private void UpdateChargeGaugeUI()
    {
        if (_currentWeapon == _bombThrow)
        {
            _chargeGaugeBar.gameObject.SetActive(true);
            _chargeGaugeBar.fillAmount = _bombThrow.ChargeProgress;
        }
        else
        {
            _chargeGaugeBar.gameObject.SetActive(false);
        }
    }
    
    private void UpdateBulletUI()
    {
        if (_currentWeapon == _gun)
        {
            _bulletText.gameObject.SetActive(true);
            _bulletText.text = $"총알 : {_gun.CurrentBulletCount} / {_gun.MaxBulletCount}";
        }
        else if (_currentWeapon == _knife)
        {
            _bulletText.gameObject.SetActive(false);
        }
        else if (_currentWeapon == _bombThrow)
        {
            _bulletText.gameObject.SetActive(true);
            _bulletText.text = $"수류탄 : {_bombThrow.GetCurrentBombCount()} / {MaxBombsCount}";
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
