using System;
using UnityEngine;
using Redcode.Pools;
using TMPro;
using UnityEngine.UI;
public class PlayerFire : MonoBehaviour
{
    // 목표 : 마우스 왼쪽 버튼을 누르면 카메라가 바라보는 방향으로 총을 발사하고 싶다   

    
    // 필요 속성
    // -발사 위치
    public GameObject FirePosition;
    // - 던지는 힘
    public float ThrowPower = 15f;

    public int MaxBombsCount = 3;
    [SerializeField]private int _haveBombsCount;
    public TextMeshProUGUI BombText;
    
    private void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        _haveBombsCount = MaxBombsCount;
        UpdateBombUI();
    }
    
    private void Update()
    {
        // 2. 오른쪽 버튼 입력 받기
        // 0: 왼쪽, 1: 오른쪽, 2: 휠
        if (Input.GetMouseButtonDown(1))
        {
            if (_haveBombsCount <= 0)
            {
                return;
            }
            
            Bomb bomb = GameManager.Instance.PoolManager.GetFromPool<Bomb>();

            bomb.transform.position = FirePosition.transform.position;

            // 4. 생성된 수류탄을 카메라 방향으로 물리적인 힘 가하기
            Rigidbody bombRigidbody = bomb.GetComponent<Rigidbody>();
            bombRigidbody.AddForce(Camera.main.transform.forward * ThrowPower, ForceMode.Impulse);
            bombRigidbody.AddTorque(Vector3.one);

            _haveBombsCount--;
            UpdateBombUI();
        }
        
        // 1. 왼쪽 버튼 입력 받기
        if (Input.GetMouseButton(0))
        {
            // 2. 레이케스트를 생성하고 발사 위치와 진행 방향을 설정
            Ray ray = new Ray(FirePosition.transform.position, Camera.main.transform.forward);
            // 3. 레이케스트와 부딛힌 물체의 정보를 저장할 변수 생성, 이 변수에 데이터가 있다면(부딛혔다면) 피격 이펙트 생성(표시)
            RaycastHit hitInfo = new RaycastHit();
            // 4. 레이케이트를 발사한 다음 
            bool isHit = Physics.Raycast(ray, out hitInfo);
            if (isHit)
            {
                BulletParticle bulletParticle = GameManager.Instance.PoolManager.GetFromPool<BulletParticle>();
                
                Transform transform = bulletParticle.transform;
                transform.position = hitInfo.point;
                transform.rotation = Quaternion.LookRotation(hitInfo.normal);
            }
        }
        // 2. 레이케스트를 생성하고 발사 위치와 진행 방향을 설정
        // 3. 레이케스트와 부딛힌 물체의 정보를 저장할 변수 생성, 이 변수에 데이터가 있다면(부딛혔다면) 피격 이펙트 생성(표시)
        // 4. 레이케이트를 발사한 다음 
    
        // Ray : 레이저(시작 위치, 방향)
        // RayCast : 레이저를 발사
        // RayCastHit : 레이저와 물체가 부딛혔다면 그 정보를 저장하는 구조체
    }
    
    private void UpdateBombUI()
    {
        if (BombText != null)
        {
            BombText.text = $"수류탄 : {_haveBombsCount} / {MaxBombsCount}";
        }
    }
}
