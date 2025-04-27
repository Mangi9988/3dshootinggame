using UnityEngine;

public class Explosion : MonoBehaviour
{
    public float Radius = 5f;            // 폭발 반경
    public float DamageValue = 50;         // 데미지
    public float KnockbackForce = 10f;   // 최대 넉백 세기
    
    public void Explode()
    {
        Vector3 center = transform.position;
        Collider[] hits = Physics.OverlapSphere(center, Radius);
       
        foreach (Collider hit in hits)
        {
            if (hit.TryGetComponent<IDamageable>(out IDamageable damageable))
            {
                Damage damage = new Damage
                {
                    Value          = DamageValue,
                    From           = gameObject,
                    KnockbackForce = KnockbackForce
                };
                damageable.TakeDamage(damage);
            }
        }
    }

    private void OnDisable()
    {
        Destroy(gameObject);
    }
}
