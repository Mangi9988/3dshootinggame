using System.Collections;
using UnityEngine;
using UnityEngine.Serialization;

// 폭발 가능 대상은 IDamageable 구현
public class Barrel : MonoBehaviour, IDamageable
{
    [Header("체력")]
    [SerializeField] private int _maxHealth = 100;
    private int _currentHealth;
    private bool _isDead;
    
    [Header("이펙트")]
    public Explosion ExplosionPrefab;
    
    [Header("오브젝트 변경")]
    [SerializeField] private GameObject _beforeObject;
    [SerializeField] private GameObject _afterObject;
    [SerializeField] private float _destroyDelayTime;
    
    [Header("날리기")]
    [SerializeField] private float _flightExplosionForce = 2f;
    [SerializeField] private float _flightExplosionTorque = 10f;
    public float MinForceRandomRange = -0.5f;
    public float MaxForceRandomRange = 0.5f;
    public float ForceY = 1f;
    // public float MinTorqueRandomRange = -0.5f;
    // public float MaxTorqueRandomRange = 0.5f;
    // public float TorqueX = 1f;
    
    private Rigidbody _rigidbody;
    
    private void Start()
    {
        _currentHealth = _maxHealth;
        _rigidbody = GetComponent<Rigidbody>();
    }
    
    public void TakeDamage(Damage damage)
    {
        if (_isDead)
        {
            return;
        }
        
        _currentHealth -= damage.Value;
        if (_currentHealth <= 0)
        {
            Die();
        }
            
    }
    
    private void Die()
    {
        _isDead = true;

        Explosion explosion = Instantiate(
            ExplosionPrefab,
            transform.position,
            Quaternion.identity
        );

        explosion.Explode();
        
        _beforeObject.SetActive(false);
        _afterObject.SetActive(true);
        
        Vector3 flightDirection = new Vector3
            (Random.Range(MinForceRandomRange, MaxForceRandomRange), 
            ForceY,
            Random.Range(MinForceRandomRange, MaxForceRandomRange)).normalized;
        
        _rigidbody.AddForce(flightDirection * _flightExplosionForce, ForceMode.Impulse);

        
        // 4) 로컬 Z축(배럴 길이 방향)으로만 토크 주기
        float torqueAmount = Random.Range(-_flightExplosionTorque, _flightExplosionTorque);
        _rigidbody.AddRelativeTorque(Vector3.forward * torqueAmount,
            ForceMode.Impulse);
        // 이거 공부좀 더해
        
        
        // Vector3 flightTorque = new Vector3
        //     (TorqueX, Random.Range(MinTorqueRandomRange, MaxTorqueRandomRange), 
        //     Random.Range(MinTorqueRandomRange, MaxTorqueRandomRange)).normalized * _flightExplosionTorque;
        // _rigidbody.AddTorque(flightTorque, ForceMode.Impulse);

        // 4) 지정 시간 후 오브젝트 제거
        StartCoroutine(DestroyAfterDelay());
    }

    private IEnumerator DestroyAfterDelay()
    {
        yield return new WaitForSeconds(_destroyDelayTime);
        Destroy(gameObject);
    }
}