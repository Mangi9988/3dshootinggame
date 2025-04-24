using System.Collections;
using UnityEngine;

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
    [SerializeField] private float _pieceExplosionForce = 2f;
    [SerializeField] private float _pieceExplosionTorque = 10f;
    public float MinForceRandomRange = -0.5f;
    public float MaxForceRandomRange = 0.5f;
    public float MinTorqueRandomRange = -0.5f;
    public float MaxTorqueRandomRange = 0.5f;
    
    private Rigidbody _rigidbody;
    
    private void Start()
    {
        _currentHealth = _maxHealth;
        _rigidbody = GetComponent<Rigidbody>();
    }
    
    public void TakeDamage(Damage damage)
    {
        if (_isDead)
            return;

        _currentHealth -= damage.Value;
        if (_currentHealth <= 0)
            Die();
    }
    
    private void Die()
    {
        _isDead = true;

        // 1) 폭발 이펙트 생성 및 폭발 로직 호출

        Explosion explosion = Instantiate(
            ExplosionPrefab,
            transform.position,
            Quaternion.identity
        );
        explosion.Explode();
        

        // 2) 외형 전환
        _beforeObject.SetActive(false);
        _afterObject.SetActive(true);

        // 3) 파편 물리 넉백
        Vector3 randDirection = new Vector3
            (Random.Range(MinForceRandomRange, MaxForceRandomRange), 
            1f,
            Random.Range(MinForceRandomRange, MaxForceRandomRange)).normalized;
        
        _rigidbody.AddForce(randDirection * _pieceExplosionForce, ForceMode.Impulse);

        Vector3 randTorque = new Vector3
            (1f, Random.Range(MinTorqueRandomRange, MaxTorqueRandomRange), 
            Random.Range(MinTorqueRandomRange, MaxTorqueRandomRange)).normalized * _pieceExplosionTorque;
        _rigidbody.AddTorque(randTorque, ForceMode.Impulse);

        // 4) 지정 시간 후 오브젝트 제거
        StartCoroutine(DestroyAfterDelay());
    }

    private IEnumerator DestroyAfterDelay()
    {
        yield return new WaitForSeconds(_destroyDelayTime);
        Destroy(gameObject);
    }
}