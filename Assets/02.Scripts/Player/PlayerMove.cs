using UnityEngine;
using UnityEngine.Serialization;

public class PlayerMove : MonoBehaviour
{
    // 목표: wasd를 누르면 캐릭터을 카메라 방향에 맞게 이동시키고 싶다.
    // 필요 속성:
    // - 이동속도
    [Header("이동")]
    public float BasicMoveSpeed = 7f;    // 기본 이동 속도
    public float SprintSpeedBonus = 3f;  // 달리기 시 추가되는 속도
    public float JumpPower = 5f;
    [SerializeField]private int _bonusJumpCount = 1;
    [SerializeField]private bool _isJumping;
    [SerializeField]private float _yVelocity;
    [SerializeField]private int _currentJumpCount;  // 현재 점프 횟수
    
    [Header("스태미너너")]
    public float MaxStamina = 100f;
    public float StaminaUseRate = 20f;
    public float StaminaRechargeRate = 10f;
    [SerializeField]private float _currentStamina;
    [SerializeField]private bool _isSprinting;

    [Header("구르기")]
    [SerializeField] private float _rollSpeed = 15f;        // 구르기 속도
    [SerializeField] private float _rollDuration = 0.5f;    // 구르기 지속 시간
    [SerializeField] private float _rollStaminaCost = 30f;  // 구르기 스태미나 소모량
    private bool _isRolling;                                // 구르기 중인지 여부
    private float _rollTimeLeft;                            // 남은 구르기 시간
    private Vector3 _rollDirection;                         // 구르기 방향
    
    private Vector3 _moveDirection;
    private CharacterController _characterController;
    private const float GRAVITY = -9.8f;

    private void Awake()
    {
        _characterController = GetComponent<CharacterController>();
        _currentStamina = MaxStamina;
    }
    
    private void Update()
    {
        Vector3 moveVector = CalculateMovement();
        HandleStamina();
        HandleJumpAndGravity();
        HandleRoll();  // 구르기 처리 추가
        
        // 최종 이동 적용 (구르기 중일 때는 구르기 이동이 추가됨)
        if (_isRolling)
        {
            moveVector += _rollDirection * _rollSpeed;
        }
        
        _characterController.Move(moveVector * Time.deltaTime);
    }

    private Vector3 CalculateMovement()
    {
        // 입력 처리
        float h = Input.GetAxisRaw("Horizontal");
        float v = Input.GetAxisRaw("Vertical");

        // 이동 방향 계산
        _moveDirection = new Vector3(h, 0, v).normalized;
        _moveDirection = Camera.main.transform.TransformDirection(_moveDirection);

        // 속도 계산
        Vector3 moveVector = _moveDirection * (_isSprinting ? BasicMoveSpeed + SprintSpeedBonus : BasicMoveSpeed);
        moveVector.y = _yVelocity;

        return moveVector;
    }

    private void HandleStamina()
    {
        bool isMoving = _moveDirection.magnitude > 0;

        if (Input.GetKey(KeyCode.LeftShift) && isMoving && _currentStamina > 0)
        {
            UseStamina();
        }
        else
        {
            StopSprinting();
            RechargeStamina();
        }
    }

    private void UseStamina()
    {
        _isSprinting = true;
        _currentStamina -= StaminaUseRate * Time.deltaTime;
        _currentStamina = Mathf.Max(0, _currentStamina);
    }

    private void StopSprinting()
    {
        _isSprinting = false;
    }

    private void RechargeStamina()
    {
        if (!Input.GetKey(KeyCode.LeftShift))
        {
            _currentStamina += StaminaRechargeRate * Time.deltaTime;
            _currentStamina = Mathf.Min(_currentStamina, MaxStamina);
        }
    }

    private void HandleJumpAndGravity()
    {
        // 착지 상태 체크
        if (_characterController.isGrounded)
        {
            HandleGrounded();
        }

        // 점프 입력 처리
        if (Input.GetButtonDown("Jump") && _currentJumpCount <= _bonusJumpCount)  // < 대신 <= 사용
        {
            Jump();
        }

        // 중력 적용
        ApplyGravity();
    }

    private void Jump()
    {
        if (_currentJumpCount >= _bonusJumpCount) return;
        
        _yVelocity = JumpPower;
        _isJumping = true;
        _currentJumpCount++;
    }

    private void ApplyGravity()
    {
        _yVelocity += GRAVITY * Time.deltaTime;
    }

    private void HandleGrounded()
    {
        _isJumping = false;
        _currentJumpCount = 0;
    }

    private void HandleRoll()
    {
        // 구르기 시도
        if (Input.GetKeyDown(KeyCode.E) && !_isRolling && _currentStamina >= _rollStaminaCost)
        {
            StartRoll();
        }

        // 구르기 진행 중
        if (_isRolling)
        {
            _rollTimeLeft -= Time.deltaTime;
            if (_rollTimeLeft <= 0)
            {
                EndRoll();
            }
        }
    }

    private void StartRoll()
    {
        _isRolling = true;
        _rollTimeLeft = _rollDuration;
        
        // 이동 입력이 있는 경우 해당 방향으로 구르기
        if (_moveDirection.magnitude > 0)
        {
            _rollDirection = _moveDirection;
        }
        // 이동 입력이 없는 경우 캐릭터가 바라보는 방향으로 구르기
        else
        {
            _rollDirection = transform.forward;
        }
        
        // 스태미나 소모
        _currentStamina = Mathf.Max(0, _currentStamina - _rollStaminaCost);
    }

    private void EndRoll()
    {
        _isRolling = false;
        _rollDirection = Vector3.zero;
    }

    // UI용 스태미나 비율 반환
    public float GetStaminaRatio()
    {
        return _currentStamina / MaxStamina;
    }
}
