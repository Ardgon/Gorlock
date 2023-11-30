using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaveSpawner : MonoBehaviour
{
    [SerializeField] private float waveCooldown; // Time between waves
    [SerializeField] private GameObject spawnPoint;
    [SerializeField] private int crumbId;
    [SerializeField] private float enemySizeScalingMultiplier;
    public List<Wave> waves;
    public int currentWaveIndex = 0;
    private bool readyToSpawnWave;
    private GridPlacementSystem gridPlacementSystem;
    private int spawnerLevel = 0;

    public void LevelUp()
    {
        spawnerLevel += 1;
    }

    private void Start()
    {
        gridPlacementSystem = FindObjectOfType<GridPlacementSystem>();
        readyToSpawnWave = false;

        // Init enemy spawn countdown
        foreach (var wave in waves)
        {
            foreach (var enemyType in wave.enemies)
            {
                enemyType.enemiesLeft = enemyType.amount;
            }
        }

        // Spawn crumbs at the start of the game
        SpawnCrumbs();
    }

    private void Update()
    {
        if (currentWaveIndex >= waves.Count)
        {
            return;
        }

        if (!readyToSpawnWave)
        {
            waveCooldown -= Time.deltaTime;

            if (waveCooldown <= 0)
            {
                readyToSpawnWave = true;
                waveCooldown = waves[currentWaveIndex].timeToNextWave;
                StartCoroutine(SpawnWave());
            }
        }
    }

    private void SpawnCrumbs()
    {
        if (gridPlacementSystem != null)
        {
            for (int i = 0; i < waves[currentWaveIndex].numberOfCrumbs; i++)
            {
                var instantiatedCrumb = gridPlacementSystem.SpawnRandomlyOnGrid(crumbId);

                if (!instantiatedCrumb)
                {
                    // If the position is not valid, decrement the loop counter to try again
                    i--;
                }
            }
        }
        else
        {
            Debug.LogError("GridPlacementSystem not found!");
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
            GameObject spawnedEnemy = Instantiate(enemyType.enemyType, spawnPoint.transform.position, spawnPoint.transform.rotation);

            spawnedEnemy.GetComponent<AIController>().SetLevel(enemyType.baseLevel + spawnerLevel);
            spawnedEnemy.transform.localScale += (enemySizeScalingMultiplier * spawnerLevel * Vector3.one);

            // Reduce enemiesLeft for that enemy type by 1
            enemyType.enemiesLeft--;

            // Wait for the specified time before spawning the next enemy
            yield return new WaitForSeconds(currentWave.timeToNextEnemy);

            // Check if all enemies of this type have been spawned
            if (enemyType.enemiesLeft == 0)
                continue; // Skip to the next enemy type if all are spawned
        }

        // Reset cooldown for the next wave
        readyToSpawnWave = false;
        waveCooldown = waves[currentWaveIndex].timeToNextWave;
        currentWaveIndex++;

        // Spawn crumbs before next wave
        SpawnCrumbs();
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
