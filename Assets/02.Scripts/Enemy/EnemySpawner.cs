using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class EnemySpawner : MonoBehaviour
{
    [Header("스폰")] 
    [SerializeField] private int spawnCount;
    [SerializeField] private float spawnRange = 5f; // 중심 기준 랜덤 거리
    [SerializeField] private float delayBetweenSpawns = 5f;
    [SerializeField] private int _maxSpawnTry = 10;
    private Coroutine _spawnCoroutine;
    
    private void Update()
    {
        if (_spawnCoroutine == null)
        {
            _spawnCoroutine = StartCoroutine(Spawn_Coroutine());
        }
    }

    private IEnumerator Spawn_Coroutine()
    {
        for (int i = 0; i < spawnCount; i++) 
        { 
            Vector3 position = GetRandomPositionOnNavMesh(transform.position, spawnRange); 
            if (position != Vector3.zero) 
            { 
                float percentage = Random.Range(0f, 1f);

                if (percentage <= 0.5f)
                {
                    Enemy enemy = GameManager.Instance.PoolManager.GetFromPool<Enemy>();
                    enemy.transform.position = position;
                }
                else
                {
                    FollowingZombie followingZombie = GameManager.Instance.PoolManager.GetFromPool<FollowingZombie>();
                    followingZombie.transform.position = position;
                }
            }
        } 
        yield return new WaitForSeconds(delayBetweenSpawns);
        
        _spawnCoroutine = null;
    }

    private Vector3 GetRandomPositionOnNavMesh(Vector3 center, float range)
    {
        for (int i = 0; i < _maxSpawnTry; i++)
        {
            Vector3 randomPos = new Vector3(
                Random.Range(-range, range),
                0f,
                Random.Range(-range, range)
            ) + center;

            if (NavMesh.SamplePosition(randomPos, out NavMeshHit hit, 2.0f, NavMesh.AllAreas))
            {
                return hit.position;
            }
        }
        return Vector3.zero;
    }
}
