using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class EnemySpawner : MonoBehaviour
{
    public GameObject enemyPrefab;
    public GameObject bossPrefab;
    public GameObject keyPrefab;
    public Transform keySpawnPoint;

    public float spawnInterval = 3f;
    public float spawnRange = 5f;
    private float timer = 0f;

    private int enemiesToSpawn = 0;
    private int spawnedCount = 0;
    private int enemiesAlive = 0;
    private bool keyDropped = false;

    // Start is called before the first frame update
    void Start()
    {
        SetEnemiesToSpawnByScene();
    }

    // Update is called once per frame
    void Update()
    {
        if (spawnedCount >= enemiesToSpawn)
            return;

        timer += Time.deltaTime;

        if (timer >= spawnInterval)
        {
            Vector3 spawnPos = new Vector3(
                transform.position.x + Random.Range(-spawnRange, spawnRange),
                transform.position.y,
                transform.position.z + Random.Range(-spawnRange, spawnRange)
            );

            GameObject enemyInstance;

            if (enemiesToSpawn == 1 && bossPrefab != null)
            {
                enemyInstance = Instantiate(bossPrefab, spawnPos, Quaternion.identity);
            }
            else
            {
                enemyInstance = Instantiate(enemyPrefab, spawnPos, Quaternion.identity);
            }

            spawnedCount++;
            enemiesAlive++;

            enemy enemyScript = enemyInstance.GetComponent<enemy>();
            if (enemyScript != null)
            {
                enemyScript.OnEnemyDeath += HandleEnemyDeath;
            }

            timer = 0f;
        }
    }
    void HandleEnemyDeath()
    {
        enemiesAlive--;
        if (spawnedCount >= enemiesToSpawn && enemiesAlive <= 0 && !keyDropped)
        {
            DropKey();
        }
    }

    void DropKey()
    {
        keyDropped = true;
        Vector3 spawnPos = keySpawnPoint != null ? keySpawnPoint.position : transform.position;
        Instantiate(keyPrefab, spawnPos, Quaternion.identity);
    }

    void SetEnemiesToSpawnByScene()
    {
        int sceneIndex = SceneManager.GetActiveScene().buildIndex;

        switch (sceneIndex)
        {
            case 0:
                enemiesToSpawn = 3;
                break;
            case 1:
                enemiesToSpawn = 5;
                break;
            case 2:
                enemiesToSpawn = 8;
                break;
            case 3:
                enemiesToSpawn = 10;
                break;
            case 4:
                enemiesToSpawn = 1;
                break;
            default:
                enemiesToSpawn = 3;
                break;
        }
    }
    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(transform.position, new Vector3(spawnRange * 2, 0.1f, spawnRange * 2));
    }
}
