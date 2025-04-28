using UnityEngine;

public class EnemyAttackEvent : MonoBehaviour
{
    public Enemy MyEnemy;

    public void AttackEvent()
    {
        Debug.Log("공격");
    }
}
