using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class enemy : MonoBehaviour
{
    public enum EnemyState { Idle, Trace, Attack, RunAway}
    public EnemyState state = EnemyState.Idle;

    public float moveSpeed = 2f;
    public float traceRange = 15f;
    public float attackRange = 6f;
    public float runAwayDistance = 2f;
    public float attackCooldown = 1.5f;

    public GameObject bulletPrefab;
    public Transform firePoint;

    private Transform player;
    private float lastAttackTime;
    public int maxHP = 5;
    private int currentHP;

    public Slider hpSlider;
    public delegate void EnemyDeathHandler();
    public event EnemyDeathHandler OnEnemyDeath;

    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;
        lastAttackTime = -attackCooldown;
        currentHP = maxHP;
        hpSlider.value = 1f;
    }

    // Update is called once per frame
    void Update()
    {
        if (player == null) return;

        float dist = Vector3.Distance(player.position, transform.position);
        float hpPercent = (float)currentHP / maxHP;

        if (hpPercent <= 0.2f && state != EnemyState.RunAway)
        {
            state = EnemyState.RunAway;
        }

        switch (state)
        {
            case EnemyState.Idle:
                if (dist < traceRange)
                    state = EnemyState.Trace;
                break;
            case EnemyState.Trace:
                if (dist < attackRange)
                    state = EnemyState.Attack;
                else if (dist > traceRange)
                    state = EnemyState.Idle;
                else
                    TracePlayer();
                break;
            case EnemyState.Attack:
                if (dist > attackRange)
                    state = EnemyState.Trace;
                else
                    AttackPlayer();
                break;
            case EnemyState.RunAway:
                RunAwayFromPlayer();
                if (dist > runAwayDistance)
                    state = EnemyState.Idle;
                break;
        }
    }
    void TracePlayer()
    {
        Vector3 dir = (player.position - transform.position).normalized;
        transform.position += dir * moveSpeed * Time.deltaTime;
        transform.LookAt(player.position);
    }
    void AttackPlayer()
    {
        if(Time.time >= lastAttackTime + attackCooldown)
        {
            lastAttackTime = Time.time;
            ShootBullet();
        }
    }
    void ShootBullet()
    {
        if(bulletPrefab != null && firePoint != null)
        {
            transform.LookAt(player.position);
            GameObject bp = Instantiate(bulletPrefab, firePoint.position, firePoint.rotation);
            EnemyBullet eb = bp.GetComponent<EnemyBullet>();
            if(eb != null)
            {
                Vector3 dir = (player.position - firePoint.position).normalized;
                eb.SetDirection(dir);
            }
        }
    }
    void RunAwayFromPlayer()
    {
        Vector3 dir = (transform.position - player.position).normalized;
        float runSpeed = moveSpeed * 1.5f;
        transform.position += dir * runSpeed * Time.deltaTime;
        transform.LookAt(transform.position + dir);
    }
    public void TakeDamage(int damage)
    {
        currentHP -= damage;
        hpSlider.value = (float)currentHP / maxHP;

        if (currentHP <= 0)
        {
            Die();
        }
    }
    public void Die()
    {
        OnEnemyDeath?.Invoke();

        Destroy(gameObject);
    }
}
