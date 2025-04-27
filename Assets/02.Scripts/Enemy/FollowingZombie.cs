using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class FollowingZombie : MonoBehaviour, IDamageable
{
    // 1, 상태를 열거형으로 정의한다
    public enum EnemyState
    {
        Trace,
        Attack,
        Damaged,
        Die
    }
    [SerializeField] private GameObject _player;
    private CharacterController _characterController;
    private NavMeshAgent _agent;
    
    [Header("범위")]
    public float AttackDistance = 2f; // 공격 범위

    [Header("공격")]
    public float AttackCooltime = 0.7f;
    private float _attackCooltimer = 0f;
    public int AttackDamageValue = 8;
    
    [Header("스텟")]
    public float MoveSpeed = 10f;
    public float MaxHealth = 70;
    public float _currentHealth;
    public Image HealthGauge;
    
    [Header("시간")]
    public float DamagedTime = 0.5f;
    public float DieTime = 2f;
    
    private PoolItem _poolItem;
    private void Awake()
    {
        _poolItem = GetComponent<PoolItem>();
    }
    
    private void Start()
    {
        _currentHealth = MaxHealth;
        UpdateHealthBar();
        _agent = GetComponent<NavMeshAgent>();
        _agent.speed = MoveSpeed;
        
        _characterController = GetComponent<CharacterController>();
        _player = GameObject.FindGameObjectWithTag("Player");
    }
    
    // 2. 현재 상태를 지정한다
    public EnemyState CurrentState = EnemyState.Trace;

    private void Update()
    {
        // 나의 현재 상태에 따라 상태 함수를 호출한다.
        switch (CurrentState)
        {
            case EnemyState.Trace:
            {
                Trace();
                break;
            }
            case EnemyState.Attack:
            {
                Attack();
                break;
            }
        }
    }

    public void TakeDamage(Damage damage)
    {
        // 사망했거나 공격받고 있는 중이면
        if(CurrentState == EnemyState.Damaged || CurrentState == EnemyState.Die)
        {
            return;
        }
        
        _currentHealth -= damage.Value;
        UpdateHealthBar();
        
        Vector3 knockbackDir = (transform.position - damage.From.transform.position).normalized;
        _characterController.Move(knockbackDir * damage.KnockbackForce);

        if (_currentHealth <= 0)
        {
            Debug.Log($"상태 전환 : {CurrentState} -> Die");
            CurrentState = EnemyState.Die;
            StartCoroutine(Die_Coroutine());
        }
        CurrentState = EnemyState.Damaged;

        StartCoroutine(Damaged_Coroutine());
    }
    
    private void UpdateHealthBar()
    {
        HealthGauge.fillAmount = _currentHealth / MaxHealth;
    }
    

    private void Trace()
    {
        // 전이 : 공격 범위 만큼 가까워 지면 -> Attack
        if (Vector3.Distance(transform.position, _player.transform.position) < AttackDistance)
        {
            Debug.Log("Trace -> Attack");
            CurrentState = EnemyState.Attack;
            return;
        }
        
        _agent.SetDestination(_player.transform.position);
    }
    
    private void Attack()
    {
        // 전이 : 공격 범위보다 멀어지면 -> Trace
        if (Vector3.Distance(transform.position, _player.transform.position) >= AttackDistance)
        {
            Debug.Log("Attack -> Trace");
            CurrentState = EnemyState.Trace;
            _attackCooltimer = 0f;
            return;
        }
        
        // 공격한다
        _attackCooltimer += Time.deltaTime;
        if (_attackCooltimer >= AttackCooltime)
        {
            _attackCooltimer = 0f;
            
            if (_player.TryGetComponent<IDamageable>(out IDamageable damageable))
            {
                Damage damage = new Damage
                {
                    Value = AttackDamageValue,
                    From = gameObject,
                    KnockbackForce = 0f
                };

                damageable.TakeDamage(damage);
            }
        }
    }

    private IEnumerator Damaged_Coroutine()
    {
        _agent.isStopped = true;
        _agent.ResetPath();
        yield return new WaitForSeconds(DamagedTime);
        Debug.Log("Damaged -> Trace");
        CurrentState = EnemyState.Trace;
    }

    private IEnumerator Die_Coroutine()
    {
        yield return new WaitForSeconds(DieTime);
        Debug.Log("크아악");
        _poolItem.ReturnToPoolAs<FollowingZombie>();
        // 죽는다
    }
}
