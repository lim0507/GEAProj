using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class enemy : MonoBehaviour
{
    public enum EnemyState { Idle, Trace, Attack, RunAway }
    public EnemyState state = EnemyState.Idle;

    [Header("기본 설정")]
    public float moveSpeed = 2f;
    public float traceRange = 15f;
    public float attackRange = 6f;
    public float runAwayDistance = 2f;
    public float attackCooldown = 1.5f;

    [Header("공격 관련")]
    public GameObject bulletPrefab;
    public Transform firePoint;

    [Header("체력 관련")]
    public int maxHP = 5;
    private int currentHP;
    public Slider hpSlider;

    private Transform player;
    private Rigidbody rb;
    private float lastAttackTime;

    public delegate void EnemyDeathHandler();
    public event EnemyDeathHandler OnEnemyDeath;

    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player")?.transform;
        rb = GetComponent<Rigidbody>();

        if (rb == null)
        {
            rb = gameObject.AddComponent<Rigidbody>();
        }

        rb.useGravity = true;           // 필요시 끔
        rb.isKinematic = false;         // MovePosition 쓰므로 false
        rb.freezeRotation = true;       // 충돌 시 회전 방지

        currentHP = maxHP;
        if (hpSlider != null)
            hpSlider.value = 1f;

        lastAttackTime = -attackCooldown;
    }

    void Update()
    {
        if (player == null) return;

        float dist = Vector3.Distance(player.position, transform.position);
        float hpPercent = (float)currentHP / maxHP;

        if (hpPercent <= 0.2f && state != EnemyState.RunAway)
            state = EnemyState.RunAway;

        switch (state)
        {
            case EnemyState.Idle:
                if (dist < traceRange) state = EnemyState.Trace;
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
        Vector3 nextPos = rb.position + dir * moveSpeed * Time.deltaTime;
        rb.MovePosition(nextPos);

        // 부드럽게 회전
        Quaternion targetRot = Quaternion.LookRotation(dir);
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRot, 0.1f);
    }

    void RunAwayFromPlayer()
    {
        Vector3 dir = (transform.position - player.position).normalized;
        Vector3 nextPos = rb.position + dir * (moveSpeed * 1.5f) * Time.deltaTime;
        rb.MovePosition(nextPos);

        Quaternion targetRot = Quaternion.LookRotation(dir);
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRot, 0.1f);
    }

    void AttackPlayer()
    {
        if (Time.time >= lastAttackTime + attackCooldown)
        {
            lastAttackTime = Time.time;
            ShootBullet();
        }
    }

    void ShootBullet()
    {
        if (bulletPrefab != null && firePoint != null)
        {
            transform.LookAt(player.position);
            GameObject bp = Instantiate(bulletPrefab, firePoint.position, firePoint.rotation);
            EnemyBullet eb = bp.GetComponent<EnemyBullet>();
            if (eb != null)
            {
                Vector3 dir = (player.position - firePoint.position).normalized;
                eb.SetDirection(dir);
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
        OnEnemyDeath?.Invoke();
        Destroy(gameObject);
    }
}
