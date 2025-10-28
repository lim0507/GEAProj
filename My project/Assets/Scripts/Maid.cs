using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Maid : MonoBehaviour
{
    [Header("체력 설정")]
    public int maxHP = 10;
    private int currentHP;

    [Header("소환 관련")]
    public GameObject spawnEnemyPrefab; 
    public float spawnInterval = 5f;    
    public float spawnRadius = 2f;      
    private float lastSpawnTime;

    [Header("UI")]
    public Slider hpSlider;

    private Rigidbody rb;

    void Start()
    {
        currentHP = maxHP;

        if (hpSlider != null)
            hpSlider.value = 1f;

        rb = GetComponent<Rigidbody>();
        if (rb == null)
            rb = gameObject.AddComponent<Rigidbody>();

        rb.useGravity = true;
        rb.isKinematic = false;
        rb.freezeRotation = true;

        lastSpawnTime = Time.time;
    }

    void Update()
    {
        if (Time.time >= lastSpawnTime + spawnInterval)
        {
            lastSpawnTime = Time.time;
            SpawnEnemy();
        }
    }

    void SpawnEnemy()
    {
        if (spawnEnemyPrefab != null)
        {
            if (spawnEnemyPrefab != null)
            {
                Vector3 spawnPos = transform.position + new Vector3(
                    Random.Range(-spawnRadius, spawnRadius),
                    0.5f,
                    Random.Range(-spawnRadius, spawnRadius)
                );

                GameObject enemy = Instantiate(spawnEnemyPrefab, spawnPos, Quaternion.identity);

                Collider maidCollider = GetComponent<Collider>();
                Collider enemyCollider = enemy.GetComponent<Collider>();

                if (maidCollider != null && enemyCollider != null)
                    Physics.IgnoreCollision(maidCollider, enemyCollider);
            }
        }
        
    }

    public void TakeDamage(int damage)
    {
        currentHP -= damage;

        if (hpSlider != null)
            hpSlider.value = (float)currentHP / maxHP;

        if (currentHP <= 0)
            Die();
    }

    void Die()
    {
        Destroy(gameObject);
    }
}
