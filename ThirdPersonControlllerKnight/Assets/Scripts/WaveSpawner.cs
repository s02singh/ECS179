using UnityEngine;

public class WaveSpawner : MonoBehaviour
{
    public GameObject zombiePrefab;
    public GameObject archerPrefab; 
    public GameObject warrokPrefab; 
    public GameObject reaperPrefab; 
    public Transform[] spawnPoints;
    private int currentWave = 0;
    private bool waveInProgress = false;
    // waveTimer and spawnInterval do not do anything as of now.
    // we can decide whether to remove or implement enemies spawning in
    // certain intervals throughout the wave
    // TODO: SPAWN INTERVALS
    private float waveTimer;
    private float spawnInterval;
    private float zombiesPerWave, archersPerWave, warroksPerWave, reapersPerWave;
    private float zombiesSpawned, archersSpawned, warroksSpawned, reapersSpawned;

    void Update()
    {
        waveTimer += Time.deltaTime;
        // If a wave is not currently in progress and there's no enemies left alive, start the next wave
        if (!waveInProgress && GameObject.FindGameObjectsWithTag("Enemy").Length == 0)
        {
            StartNextWave();
        }
        // Check to see if the wave is over at every frame
        checkEndOfWave();
    }

    void StartNextWave()
    {
        currentWave++;
        waveTimer = 0f;
        // set the amount of enemies to be spawned in next wave 
        // TODO: NEEDS BALANCING CHANGES!!!
        zombiesPerWave = currentWave * 2;
        archersPerWave = currentWave * 1; 

        // Calculate warroks and reapers per wave (warroks start spawning on wave 3, reapers on wave 5)
        // TODO: NEEDS BALANCING
        if (currentWave >= 3)
        {
            warroksPerWave = Mathf.RoundToInt(currentWave * 0.3f);
        }
        else
        {
            warroksPerWave = 0;
        }
        
        if (currentWave >= 5)
        {
            reapersPerWave = Mathf.RoundToInt(currentWave * 0.2f);
        }
        else
        {
            reapersPerWave = 0;
        }

        Debug.Log("Current Wave: " + currentWave);


        // Begin the wave
        waveInProgress = true;
        SpawnWave(currentWave);
    }

    void SpawnWave(int waveNumber)
    {
        for (int i = 0; i < zombiesPerWave; i++)
        {
            SpawnEnemy(zombiePrefab);
            zombiesSpawned++;
        }

        for (int i = 0; i < archersPerWave; i++)
        {
            SpawnEnemy(archerPrefab);
            archersSpawned++;
        }

        for (int i = 0; i < warroksPerWave; i++)
        {
            SpawnEnemy(warrokPrefab);
            warroksSpawned++;
        }

        for (int i = 0; i < reapersPerWave; i++)
        {
            SpawnEnemy(reaperPrefab);
            reapersSpawned++;
        }
    }

    void SpawnEnemy(GameObject enemyPrefab)
    {
        Transform spawnPoint = spawnPoints[Random.Range(0, spawnPoints.Length)];
        Vector3 randomOffset = new Vector3(Random.Range(-1f, 1f), 0f, Random.Range(-1f, 1f));

        // Instantiate the enemy prefab at the randomized spawn position
        Instantiate(enemyPrefab, spawnPoint.position + randomOffset, Quaternion.identity);
    }

    void checkEndOfWave()
    {
        // Ends the wave spawning when enough enemies have been spawned
        // TODO: NEEDS BALANCING (we may change how the end of wave logic works)
        if (waveInProgress && zombiesSpawned + archersSpawned + warroksSpawned + reapersSpawned >= zombiesPerWave+ archersPerWave + warroksPerWave + reapersPerWave)
        {
            waveInProgress = false;
        }
    }
}
