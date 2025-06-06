using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Serialization;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class Enemy : MonoBehaviour, IDamageable
{
    // 1, 상태를 열거형으로 정의한다
    public enum EnemyState
    {
        Idle,
        Patrol,
        RandomPatrol,
        Trace,
        Return,
        Attack,
        Damaged,
        Die
    }
    public GameObject Player;
    private CharacterController _characterController;
    private NavMeshAgent _agent;
    private Animator _animator;
    
    [Header("몬스터 종류")]
    public bool IsFollowingZombie = false;
    
    [Header("범위")]
    public float FindDistance = 7f;   // 탐색 범위
    public float AttackDistance = 2f; // 공격 범위
    public float ReturnDistance = 10f;// 복귀 범위
    public float BackToReturnPositionDistance = 1.2f;
    private Vector3 _lastPosition;
    private Vector3 _startPosition;

    [Header("공격")]
    public float AttackCooltime = 1.5f;
    private float _attackCooltimer = 0f;
    public int AttackDamageValue = 10;
    
    [Header("스텟")]
    public float MoveSpeed = 3.3f;
    public float MaxHealth = 100;
    public float _currentHealth;
    public Image HealthGauge;
    
    [Header("시간")]
    public float DamagedTime = 0.5f;
    public float DieTime = 2f;
    public float IdleTime = 5f;
    
    [Header("순찰")]
    private Coroutine _idleCoroutine;
    [SerializeField] private PatrolPointData _patrolPointData;
    public Vector3[] PatrolPoints;
    private List<Transform> _runtimePatrolPoints = new List<Transform>();
    private Vector3 _currentDestination;
    public int PatrolCount = 3;
    private int _currentPatrolIndex = 0;
    private bool _isCompletePatrol = false;
    
    [Header("랜덤순찰")]
    private Vector3 _randomTarget;
    private bool _hasRandomTarget = false;
    public float RandomPartolDistance = 7f;
    
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
        
        _animator = _animator = GetComponentInChildren<Animator>();
        _animator.applyRootMotion = false;
        
        PatrolPoints = _patrolPointData.PatrolPositions;
        
        _startPosition = transform.position;
        _lastPosition = _startPosition;
        _characterController = GetComponent<CharacterController>();
        Player = GameObject.FindGameObjectWithTag("Player");
        SetRandomPatrolPoint();

        if (IsFollowingZombie)
        {
            FindDistance = 300f;
        }
    }
    
    // 2. 현재 상태를 지정한다
    public EnemyState CurrentState = EnemyState.Idle;

    private void Update()
    {
        // 나의 현재 상태에 따라 상태 함수를 호출한다.
        switch (CurrentState)
        {
            case EnemyState.Idle:
            {
                Idle();
                break;
            }
            case EnemyState.Patrol:
            {
                Patrol();
                break;
            }
            case EnemyState.RandomPatrol:
            {
                RandomPatrol();
                break;
            }
            case EnemyState.Trace:
            {
                Trace();
                break;
            }
            case EnemyState.Return:
            {
                Return();
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
        
        _animator.SetTrigger("Hit");
        _currentHealth -= damage.Value;
        UpdateHealthBar();
        
        Vector3 knockbackDir = (transform.position - damage.From.transform.position).normalized;
        _characterController.Move(knockbackDir * damage.KnockbackForce);

        if (_currentHealth <= 0)
        {
            Debug.Log($"상태 전환 : {CurrentState} -> Die");
            CurrentState = EnemyState.Die;
            StartCoroutine(Die_Coroutine());
            return;
        }
        CurrentState = EnemyState.Damaged;

        StartCoroutine(Damaged_Coroutine());
    }

    private void UpdateHealthBar()
    {
        HealthGauge.fillAmount = _currentHealth / MaxHealth;
    }

    // 3. 상태 함수들을 구현한다
    private void Idle()
    {
        // 플레이어 감지를 먼저 체크
        if (Vector3.Distance(transform.position, Player.transform.position) <= FindDistance)
        {
            if (_idleCoroutine != null)
            {
                StopCoroutine(_idleCoroutine);
                _idleCoroutine = null;
            }
            Debug.Log("Idle -> Trace");
            CurrentState = EnemyState.Trace;
            _animator.SetTrigger("IdleToTrace");
            return;
        }
        
        // 플레이어가 없을 때만 순찰 코루틴 시작
        if (_idleCoroutine == null)
        {
            _idleCoroutine = StartCoroutine(IdleWaitAndPatrol());
        }
    }

    private IEnumerator IdleWaitAndPatrol()
    {
        yield return new WaitForSeconds(IdleTime);

        _idleCoroutine = null;
        if (_isCompletePatrol == false)
        {
            Debug.Log("Idle -> Patrol");
            CurrentState = EnemyState.Patrol; 
            _animator.SetTrigger("IdleToWalk");
        }
        else if (_isCompletePatrol == true)
        {
            Debug.Log("Idle -> RandomPatrol");
            CurrentState = EnemyState.RandomPatrol;
            _animator.SetTrigger("IdleToWalk");
        }
    }
    
    private void Patrol()
    {
        if (Vector3.Distance(transform.position, Player.transform.position) <= FindDistance)
        {
            Debug.Log("Patrol -> Trace");
            _animator.SetTrigger("WalkToTrace");
            CurrentState = EnemyState.Trace;
            return;
        }
        
        if (Vector3.Distance(transform.position, _currentDestination) < BackToReturnPositionDistance)
        {
            _currentPatrolIndex++;
            if (_currentPatrolIndex >= PatrolCount)
            {
                Debug.Log("순찰완료");
                _isCompletePatrol = true;
                Debug.Log($"랜덤 순찰 타겟 생성 기준: {_lastPosition}");
            }
            else
            {
                SetRandomPatrolPoint();
            }

            _lastPosition = transform.position;
            Debug.Log("Patrol -> Idle");
            _animator.SetTrigger("WalkToIdle");
            CurrentState = EnemyState.Idle;
            return;
        }

        // Vector3 direction = (_currentDestination.position - transform.position).normalized;
        // _characterController.Move(direction * MoveSpeed * Time.deltaTime);
        _agent.SetDestination(_currentDestination);
    }
    
    private void SetRandomPatrolPoint()
    {
        List<Vector3> candidatePoints = new List<Vector3>();
        foreach (Vector3 position in PatrolPoints)
        {
            if (_currentDestination != Vector3.zero && position == _currentDestination)
                continue;

            candidatePoints.Add(position);
        }

        if (candidatePoints.Count > 0)
        {
            int randomIndex = Random.Range(0, candidatePoints.Count);
            _currentDestination = candidatePoints[randomIndex];
        }
    }

    private void RandomPatrol()
    {
        // 플레이어 감지되면 Trace 상태로 전환
        if (Vector3.Distance(transform.position, Player.transform.position) <= FindDistance)
        {
            Debug.Log("RandomPatrol -> Trace");
            _hasRandomTarget = false;
            CurrentState = EnemyState.Trace;
            _animator.SetTrigger("WalkToTrace");
            return;
        }

        // 아직 랜덤 타겟이 없다면 하나 생성
        if (!_hasRandomTarget)
        {
            _randomTarget = SetRandomPointAround(_lastPosition, RandomPartolDistance);
            _hasRandomTarget = true;
        }

        // 이동
        // Vector3 direction = (_randomTarget - transform.position).normalized;
        // _characterController.Move(direction * MoveSpeed * Time.deltaTime);
        _agent.SetDestination(_randomTarget);

        // 도착했으면 -> Idle
        if (Vector3.Distance(transform.position, _randomTarget) <= BackToReturnPositionDistance)
        {
            Debug.Log("RandomPatrol -> Idle");
            _animator.SetTrigger("WalkToIdle");
            _hasRandomTarget = false;
            _lastPosition = _randomTarget;
            CurrentState = EnemyState.Idle;
        }
    }

    private Vector3 SetRandomPointAround(Vector3 center, float range)
    {
        while (true)
        {
            Vector3 randomPos = new Vector3(
                Random.Range(-range, range),
                0f,
                Random.Range(-range, range)
            ) + center;

            NavMeshHit hit;
            if (NavMesh.SamplePosition(randomPos, out hit, 2.0f, NavMesh.AllAreas))
            {
                return hit.position;
            }
        }
    }

    private void Trace()
    {
        // 전이 : 플레이어와 멀어지거나 복귀 지점과 멀어지면 -> Return
        if (Vector3.Distance(transform.position, Player.transform.position) >= FindDistance)
        {
            Debug.Log("Trace -> Return");
            _animator.SetTrigger("TraceToWalk");
            CurrentState = EnemyState.Return;
            return;
        }
        
        // 전이 : 공격 범위 만큼 가까워 지면 -> Attack
        if (Vector3.Distance(transform.position, Player.transform.position) < AttackDistance)
        {
            Debug.Log("Trace -> Attack");
            _animator.SetTrigger("MoveToAttackDelay");
            CurrentState = EnemyState.Attack;
            return;
        }
        
        
        // 쫓아간다
        //Vector3 diraction = (_player.transform.position - transform.position).normalized;
        //_characterController.Move(diraction * MoveSpeed * Time.deltaTime);
        _agent.SetDestination(Player.transform.position);
    }

    private void Return()
    {
        // 전이 : 시작 위치와 가까워 지면 -> Idle
        if (Vector3.Distance(transform.position, _lastPosition) <= BackToReturnPositionDistance)
        {
            Debug.Log("Return -> Idle");
            _animator.SetTrigger("MoveToIdle");
            transform.position = _lastPosition;
            CurrentState = EnemyState.Idle;
            return;
        }
        
        // 전이 : 되돌아 가는 도중 적을 찾으면 다시 Trace
        if (Vector3.Distance(transform.position, Player.transform.position) <= FindDistance &&
            Vector3.Distance(_lastPosition, Player.transform.position) <= ReturnDistance)
        {
            Debug.Log("Return -> Trace");
            _animator.SetTrigger("WalkToTrace");
            CurrentState = EnemyState.Trace;
            return;
        }
        
        // 시작 위치와되돌아간다
        //Vector3 diraction = (_returnPosition - transform.position).normalized;
        //_characterController.Move(diraction * MoveSpeed * Time.deltaTime);
        _agent.SetDestination(_lastPosition);
    }

    public void Attack()
    {
        // 전이 : 공격 범위보다 멀어지면 -> Trace
        if (Vector3.Distance(transform.position, Player.transform.position) >= AttackDistance)
        {
            Debug.Log("Attack -> Trace");
            _animator.SetTrigger("AttackDelayToMove");
            CurrentState = EnemyState.Trace;
            _attackCooltimer = 0f;
            return;
        }
        
        // 공격한다
        _attackCooltimer += Time.deltaTime;
        if (_attackCooltimer >= AttackCooltime)
        {
            _attackCooltimer = 0f;
            _animator.SetTrigger("AttackDelayToAttack");
        }
    }

    private IEnumerator Damaged_Coroutine()
    {
        // 행동 : 일정 시간동안 멈춰있다가 -> Trace
        // _damagedTimer += Time.deltaTime;
        // if (_damagedTimer >= DamagedTime)
        // {
        //     _damagedTimer = 0f;
        //     Debug.Log("Damaged -> Trace");
        //     CurrentState = EnemyState.Trace;
        // }
        //
        // if (Health <= 0)
        // {
        //     Debug.Log("Damaged -> Die");
        //     CurrentState = EnemyState.Die;
        // }
        _agent.isStopped = true;
        _agent.ResetPath();
        _animator.SetTrigger("Hit");
        yield return new WaitForSeconds(DamagedTime);
        Debug.Log("Damaged -> Trace");
        _animator.SetTrigger("AttackDelayToMove");
        CurrentState = EnemyState.Trace;
    }

    private IEnumerator Die_Coroutine()
    {
        _agent.isStopped = true;
        _agent.ResetPath();
        _animator.SetTrigger("Die");
        yield return new WaitForSeconds(DieTime);
        Debug.Log("크아악");
        _poolItem.ReturnToPoolAs<Enemy>();
        // 죽는다
    }
}
