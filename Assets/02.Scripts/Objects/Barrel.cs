using UnityEngine;

// 폭발 가능 대상은 IDamageable 구현
public class Barrel : MonoBehaviour, IDamageable
{
    // 미리 프리팹으로 만들어 둔 Explosion 컴포넌트
    [SerializeField] private Explosion _barrelExplosionPrefab;

    // 데미지를 받으면 곧바로 폭발
    public void TakeDamage(Damage damage)
    {
        // 1) 폭발 위치에 Explosion 인스턴스 띄우고
        Explosion explosion = Instantiate(
            _barrelExplosionPrefab,
            transform.position,
            Quaternion.identity
        );

        // 2) 즉시 폭발 로직 실행
        explosion.Explode();

        // 3) Barrel 오브젝트는 제거
        Destroy(gameObject);
    }
}