using UnityEngine;

public class Explosion : MonoBehaviour
{
    public float Radius = 5f;            // 폭발 반경
    public int DamageValue = 50;         // 데미지
    public float KnockbackForce = 10f;   // 최대 넉백 세기

    public void Explode()
    {
        Vector3 center = transform.position;
        Collider[] hits = Physics.OverlapSphere(center, Radius);

        foreach (var hit in hits)
        {
            // 1) 물리 넉백
            Rigidbody rigidbody = hit.attachedRigidbody;

            // 폭발 중심에서 대상까지의 방향
            Vector3 direction = (hit.transform.position - center).normalized;

            // 거리 기반으로 힘을 선형 감쇠
            float distance = Vector3.Distance(center, hit.transform.position);
            float atten = 1f - Mathf.Clamp01(distance / Radius);
            float force = KnockbackForce * atten;

            // 원하는 축(예: 수평)만 밀어내려면 dir.y = 0; 으로 제어
            rigidbody.AddForce(direction * force, ForceMode.Impulse);

            // 2) 데미지 전파
            IDamageable dmgReceiver = hit.GetComponent<IDamageable>();
            Damage damage = new Damage { 
                Value = DamageValue, 
                From = gameObject, 
                KnockbackForce = KnockbackForce };
            dmgReceiver.TakeDamage(damage);
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = new Color(1, 0.5f, 0, 0.3f);
        Gizmos.DrawSphere(transform.position, Radius);
    }
}
