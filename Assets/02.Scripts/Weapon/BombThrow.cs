using UnityEngine;
using UnityEngine.UI;
using Redcode.Pools;

public class BombThrow : IThrowableWeapon
{
    private int _haveBombsCount;

    private float _chargeTimer;
    private float _maxChargeTime;

    private float _minThrowPower;
    private float _maxThrowPower;

    private bool _isCharging;
    
    public bool IsAttackAvailable => _haveBombsCount > 0 && !_isCharging;
    
    public float ChargeProgress => _isCharging ? (_chargeTimer / _maxChargeTime) : 0f;
    
    public Transform FirePosition;

    public BombThrow(int maxBombsCount, float maxChargeTime, float minPower, float maxPower, Transform firePosition)
    {
        _haveBombsCount = maxBombsCount;
        _maxChargeTime = maxChargeTime;
        _minThrowPower = minPower;
        _maxThrowPower = maxPower;
        FirePosition = firePosition;
    }
    
    public void Attack()
    {
        if (_haveBombsCount <= 0)
        {
            return;
        }
        StartCharging();
    }

    public void Update()
    {
        if (_isCharging)
        {
            Charging();
        }
    }
    
    public void StartCharging()
    {
        if (_haveBombsCount <= 0 || _isCharging)
        {
            return;
        }

        _isCharging = true;
        _chargeTimer = 0f;
    }
    
    private void Charging()
    {
        _chargeTimer += Time.deltaTime;
        _chargeTimer = Mathf.Min(_chargeTimer, _maxChargeTime);
    }

    public void Throw()
    {
        if (_haveBombsCount <= 0)
        {
            return;
        }
        
        _isCharging = false;

        float timeCalculate = _chargeTimer / _maxChargeTime;
        float finalPower = Mathf.Lerp(_minThrowPower, _maxThrowPower, timeCalculate);
        
        Bomb bomb = GameManager.Instance.PoolManager.GetFromPool<Bomb>();
        bomb.transform.position = FirePosition.position;

        Rigidbody bombRigidbody = bomb.GetComponent<Rigidbody>();
        bombRigidbody.AddForce(Camera.main.transform.forward * finalPower, ForceMode.Impulse);
        bombRigidbody.AddTorque(Vector3.one);

        _haveBombsCount--;
    }

    public int GetCurrentBombCount()
    {
        return _haveBombsCount;
    }
}
