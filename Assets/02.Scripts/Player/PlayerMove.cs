using UnityEngine;
using UnityEngine.Serialization;

public class PlayerMove : MonoBehaviour, IDamageable
{
    [SerializeField] private PlayerStatData _statData;
    
    private bool _isJumping;
    private float _yVelocity;
    private int _currentJumpCount;
    private float _currentStamina;
    private bool _isSprinting;
    
    private bool _isRolling;
    private float _rollTimeLeft;
    private Vector3 _rollDirection;
    
    [SerializeField] private LayerMask _wallLayer;
    private bool _isWallClimbing;
    private bool _canWallClimb = true;
    private RaycastHit _wallHit;

    private Vector3 _moveDirection;
    private CharacterController _characterController;
    private const float GRAVITY = -9.8f;

    private void Awake()
    {
        _characterController = GetComponent<CharacterController>();
        _currentStamina = _statData.MaxStamina;
    }
    
    private void Update()
    {
        Vector3 moveVector = CalculateMovement();
        HandleStamina();
        HandleWallClimb();
        HandleRoll();
        
        if (!_isWallClimbing)
        {
            HandleJumpAndGravity();
        }
        
        if (_isRolling)
        {
            moveVector += _rollDirection * _statData.RollSpeed;
        }
        
        _characterController.Move(moveVector * Time.deltaTime);
    }

    private Vector3 CalculateMovement()
    {
        float h = Input.GetAxisRaw("Horizontal");
        float v = Input.GetAxisRaw("Vertical");

        _moveDirection = new Vector3(h, 0, v).normalized;
        _moveDirection = Camera.main.transform.TransformDirection(_moveDirection);

        Vector3 moveVector = _moveDirection * (_isSprinting ? 
            _statData.BasicMoveSpeed + _statData.SprintSpeedBonus : _statData.BasicMoveSpeed);
        moveVector.y = _yVelocity;

        return moveVector;
    }

    private void HandleStamina()
    {
        if (_isWallClimbing) return;

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
        _currentStamina -= _statData.StaminaUseRate * Time.deltaTime;
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
            _currentStamina += _statData.StaminaRechargeRate * Time.deltaTime;
            _currentStamina = Mathf.Min(_currentStamina, _statData.MaxStamina);
        }
    }

    private void HandleJumpAndGravity()
    {
        if (Input.GetButtonDown("Jump") && _currentJumpCount <= _statData.BonusJumpCount)
        {
            Jump();
        }
        
        if (_characterController.isGrounded)
        {
            HandleGrounded();
        }

        ApplyGravity();
    }

    private void Jump()
    {
        if (_currentJumpCount >= _statData.BonusJumpCount) return;
        
        _yVelocity = _statData.JumpPower;
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
        _canWallClimb = true;
        
        if (_yVelocity < 0)
        {
            _yVelocity = 0f;
        }
    }

    private void HandleRoll()
    {
        if (Input.GetKeyDown(KeyCode.E) && !_isRolling && _currentStamina >= _statData.RollStaminaUseRate)
        {
            StartRoll();
        }

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
        _rollTimeLeft = _statData.RollDuration;
        
        if (_moveDirection.magnitude > 0)
        {
            _rollDirection = _moveDirection;
        }
        else
        {
            _rollDirection = transform.forward;
        }
        
        _currentStamina = Mathf.Max(0, _currentStamina - _statData.RollStaminaUseRate);
    }

    private void EndRoll()
    {
        _isRolling = false;
        _rollDirection = Vector3.zero;
    }

    public float GetStaminaRatio()
    {
        return _currentStamina / _statData.MaxStamina;
    }

    private void HandleWallClimb()
    {
        bool wallInFront = IsWallInFront();

        if (wallInFront && Input.GetButton("Jump") && _currentStamina > 0 && !_isRolling && _canWallClimb)
        {
            _isWallClimbing = true;
            _yVelocity = _statData.WallClimbSpeed;
            
            _currentStamina -= _statData.WallClimbStaminaCost * Time.deltaTime;
            _currentStamina = Mathf.Max(0, _currentStamina);

            if (_currentStamina <= 0)
            {
                _isWallClimbing = false;
                _canWallClimb = false;
            }
        }
        else
        {
            _isWallClimbing = false;
        }
    }

    private bool IsWallInFront()
    {
        bool wallAhead = Physics.Raycast(transform.position, transform.forward, out _wallHit, _statData.WallCheckDistance, _wallLayer);
        return wallAhead;
    }

    public void TakeDamage(Damage damage)
    {
        
    }
}
