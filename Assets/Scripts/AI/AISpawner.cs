using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class AISpawner : MonoBehaviour
{
    [SerializeField]
    private GameObject enemyPrefab;  // The enemy prefab to be spawned
    [SerializeField]
    private float spawnRadius = 3f;  // The radius within which enemies will be spawned
    [SerializeField]
    private float spawnIntervalMin = 5f;  // Minimum time between spawns
    [SerializeField]
    private float spawnIntervalMax = 10f; // Maximum time between spawns
    [SerializeField]
    private int maxRetries = 5; // Maximum time between spawns

    private void Start()
    {
        StartCoroutine(SpawnEnemies());
    }

    private IEnumerator SpawnEnemies()
    {
        while (true)
        {
            yield return new WaitForSeconds(Random.Range(spawnIntervalMin, spawnIntervalMax));

            bool spawned = false;

            for (int i = 0; i < maxRetries; i++)
            {
                Vector3 randomPoint = Random.insideUnitSphere * spawnRadius;
                randomPoint.y = 0;  // Ensure the y-coordinate is at the same level as the spawner

                // Calculate the spawn position by adding the random point to the spawner's position
                Vector3 spawnPosition = transform.position + randomPoint;

                NavMeshHit hit;
                if (NavMesh.SamplePosition(spawnPosition, out hit, 10f, NavMesh.AllAreas))
                {
                    spawnPosition = hit.position;

                    var spawnedEnemy = Instantiate(enemyPrefab, spawnPosition, Quaternion.identity);
                    spawned = true;
                    break;
                }
            }

            if (!spawned)
            {
                Debug.LogWarning("Failed to find a valid spawn position after multiple retries.");
            }
        }
    }
}
