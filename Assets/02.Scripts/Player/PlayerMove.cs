using UnityEngine;

public class PlayerMove : MonoBehaviour
{
    // 목표: wasd를 누르면 캐릭터을 카메라 방향에 맞게 이동시키고 싶다.
    // 필요 속성:
    // - 이동속도
    [Header("Basic Movement")]
    public float BasicMoveSpeed = 7f;    // 기본 이동 속도
    public float SprintSpeedBonus = 3f;  // 달리기 시 추가되는 속도
    public float JumpPower = 5f;
    
    [Header("Stamina System")]
    public float MaxStamina = 100f;
    public float StaminaUseRate = 20f;
    public float StaminaRechargeRate = 10f;
    
    // private 변수들
    private float _currentStamina;
    private bool _isSprinting;
    private bool _isJumping;
    private float _yVelocity;
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
        
        // 최종 이동 적용
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
        // 착지 상태 체크 추가
        if (_characterController.isGrounded)
        {
            HandleGrounded();
        }

        // 점프 입력 처리
        if (Input.GetButtonDown("Jump") && !_isJumping)
        {
            Jump();
        }

        // 중력 적용
        ApplyGravity();
    }

    private void Jump()
    {
        _yVelocity = JumpPower;
        _isJumping = true;
    }

    private void ApplyGravity()
    {
        _yVelocity += GRAVITY * Time.deltaTime;
    }

    private void HandleGrounded()
    {
        _isJumping = false;
        if (_yVelocity < 0)
        {
            _yVelocity = 0f;
        }
    }

    // UI용 스태미나 비율 반환
    public float GetStaminaRatio()
    {
        return _currentStamina / MaxStamina;
    }
}
