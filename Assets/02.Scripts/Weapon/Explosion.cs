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

        foreach (Collider hit in hits)
        {
            // 1) 물리 넉백
            Rigidbody rigidbody = hit.attachedRigidbody;
            if (rigidbody != null)
            {
                Vector3 dir  = (hit.transform.position - center).normalized;
                float  dist = Vector3.Distance(center, hit.transform.position);
                float  atten = 1f - Mathf.Clamp01(dist / Radius);
                float  force = KnockbackForce * atten;
                rigidbody.AddForce(dir * force, ForceMode.Impulse);
            }



            // 2) 데미지 전파
            if (hit.TryGetComponent<IDamageable>(out var receiver))
            {
                Damage damage = new Damage
                {
                    Value          = DamageValue,
                    From           = gameObject,
                    KnockbackForce = KnockbackForce
                };
                receiver.TakeDamage(damage);
            }
        }
    }

    private void OnDisable()
    {
        Destroy(gameObject);
    }
    
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = new Color(1, 0.5f, 0, 0.3f);
        Gizmos.DrawSphere(transform.position, Radius);
    }
}
