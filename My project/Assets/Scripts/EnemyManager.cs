using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyManager : MonoBehaviour
{
    [Header("열쇠 드랍 설정")]
    public GameObject keyPrefab;
    public Transform keySpawnPoint;

    private bool keyDropped = false;

    void Update()
    {
        if (keyDropped) return;

        // 씬 내 모든 Enemy 태그 오브젝트 확인
        GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");

        // 모두 사라지면 열쇠 드랍
        if (enemies.Length == 0)
        {
            DropKey();
        }
    }

    void DropKey()
    {
        keyDropped = true;
        Vector3 spawnPos = keySpawnPoint != null ? keySpawnPoint.position : transform.position;

        Instantiate(keyPrefab, spawnPos, Quaternion.identity);
        Debug.Log("🔑 모든 적이 사라졌습니다. 열쇠를 드랍했습니다!");
    }
}
