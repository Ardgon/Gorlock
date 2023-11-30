using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaveSpawner : MonoBehaviour
{
    [SerializeField] private float countdown;
    [SerializeField] private GameObject spawnPoint;
    public List<Wave> waves;
    public int currentWaveIndex = 0;
    private bool readyToCountDown;

    private void Start()
    {
        readyToCountDown = true;

        // Init enemy spawn countdown
        foreach (var wave in waves)
        {
            foreach (var enemyType in wave.enemies)
            {
                enemyType.enemiesLeft = enemyType.amount;
            }
        }
    }

    private void Update()
    {
        if (currentWaveIndex >= waves.Count)
        {
            Debug.Log("END GAME");
            return;
        }

        if (readyToCountDown)
        {
            countdown -= Time.deltaTime;
        }

        if (countdown <= 0)
        {
            readyToCountDown = false;
            countdown = waves[currentWaveIndex].timeToNextWave;
            StartCoroutine(SpawnWave());
        }
    }

    private IEnumerator SpawnWave()
    {
        if (currentWaveIndex >= waves.Count)
            yield break;

        Wave currentWave = waves[currentWaveIndex];

        // Create a list to hold the sequence of enemy spawns
        List<WaveEnemyType> spawnSequence = new List<WaveEnemyType>();

        // Populate the spawn sequence with a random order of enemy types
        foreach (var enemyType in currentWave.enemies)
        {
            for (int i = 0; i < enemyType.amount; i++)
            {
                spawnSequence.Add(enemyType);
            }
        }

        // Shuffle the spawn sequence manually
        for (int i = 0; i < spawnSequence.Count; i++)
        {
            int randomIndex = Random.Range(i, spawnSequence.Count);
            var temp = spawnSequence[i];
            spawnSequence[i] = spawnSequence[randomIndex];
            spawnSequence[randomIndex] = temp;
        }

        List<WaveEnemyType> spawnOrder = new List<WaveEnemyType>(spawnSequence);

        foreach (var enemyType in spawnOrder)
        {
            // Spawn enemy
            Instantiate(enemyType.enemyType, spawnPoint.transform.position, spawnPoint.transform.rotation);

            // Reduce enemiesLeft for that enemy type by 1
            enemyType.enemiesLeft--;

            // Wait for the specified time before spawning the next enemy
            yield return new WaitForSeconds(currentWave.timeToNextEnemy);

            // Check if all enemies of this type have been spawned
            if (enemyType.enemiesLeft == 0)
                continue; // Skip to the next enemy type if all are spawned
        }

        // Reset countdown for the next wave
        readyToCountDown = true;
        currentWaveIndex++;
    }
}

[System.Serializable]
public class Wave
{
    public List<WaveEnemyType> enemies;
    public float timeToNextEnemy;
    public float timeToNextWave;
    public int numberOfCrumbs;
}

[System.Serializable]
public class WaveEnemyType
{
    public GameObject enemyType;
    public int amount;
    public int baseLevel;
    [HideInInspector] public int enemiesLeft;
}
