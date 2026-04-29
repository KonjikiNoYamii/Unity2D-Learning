using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class EnemyAI : MonoBehaviour
{
    public float attackRange = 1.5f;
    public float attackCooldown = 1.5f;

    public float moveSpeed = 2f;
    public float chaseRange = 5f;

    public Vector2 attackBoxSize = new Vector2(1.5f, 1f);
    public Vector2 attackOffset = new Vector2(1f, 0.5f);

    public LayerMask playerLayer;

    private bool isFacingRight = true;

    private HashSet<Collider2D> hitTarget = new HashSet<Collider2D>();

    private bool isAttacking;

    private bool canDealDamage;

    private float nextTimeAttack;

    private Transform player;

    private EnemyHealth enemyHealth;

    private Animator animator;

    private Rigidbody2D rb;

    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player")?.transform;

        rb = GetComponent<Rigidbody2D>();

        animator = GetComponent<Animator>();

        enemyHealth = GetComponent<EnemyHealth>();

        if (player == null)
        {
            Debug.Log("Player tidak ditemukan!");
        }
    }

    void Update()
    {
        if (player == null) return;

        if (enemyHealth != null && enemyHealth.IsHurt()) return;

        float distance = Vector2.Distance(transform.position, player.position);

        animator.SetFloat("Walk", Mathf.Abs(rb.linearVelocity.x));

        if (!isAttacking && distance <= attackRange && Time.time >= nextTimeAttack)
        {
            AttackPlayer();
        }

        if (isAttacking) return;

        if (distance <= chaseRange && distance > attackRange)
        {
            Chaseplayer();
        }
        else
        {
            StopMoving();
        }

        bool shouldFaceRight = player.position.x > transform.position.x;
        if (shouldFaceRight != isFacingRight)
        {
            isFacingRight = shouldFaceRight;
            Flip();
        }
    }

    void AttackPlayer()
    {
        if (isAttacking) return;

        hitTarget.Clear();

        Debug.Log("Enemy Menyerang!");

        isAttacking = true;

        if (animator != null)
        {
            animator.SetTrigger("Attack");
        }
    }

    void Flip()
    {
        Vector3 scale = transform.localScale;
        scale.x = Math.Abs(scale.x) * (isFacingRight ? 1 : -1);
        transform.localScale = scale;
    }

    void Chaseplayer()
    {
        float direction = player.position.x > transform.position.x ? 1f : -1f;

        rb.linearVelocity = new Vector2(direction * moveSpeed, rb.linearVelocity.y);
    }

    void StopMoving()
    {
        rb.linearVelocity = new Vector2(0f, rb.linearVelocity.y);
    }

    public void EnableHit()
    {
        canDealDamage = true;
    }

    public void DisableHit()
    {
        canDealDamage = false;
    }

    public void EndAttack()
    {
        isAttacking = false;
        canDealDamage = false;
        nextTimeAttack = Time.time + attackCooldown;

    }

    public void CancelAttack()
    {
        isAttacking = false;
        StopMoving();
    }

    public void DealDamage()
    {
        if (!canDealDamage) return;

        float direction = isFacingRight ? 1f : -1f;

        Vector2 boxCenter = (Vector2)transform.position +
        new Vector2(attackOffset.x * direction, attackOffset.y);

        Collider2D[] hits = Physics2D.OverlapBoxAll(
            boxCenter,
            attackBoxSize,
            0f,
            playerLayer
        );

        foreach (Collider2D hit in hits)
        {
            if (hitTarget.Contains(hit)) continue;

            hitTarget.Add(hit);

            if (hit != null)
            {
                Player p = hit.GetComponent<Player>();

                if (p != null)
                {
                    p.TakeHit();
                }
            }
        }
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;

        float direction = isFacingRight ? 1f : -1f;

        Vector2 boxCenter = (Vector2)transform.position +
        new Vector2(attackOffset.x * direction, attackOffset.y);

        Gizmos.DrawWireCube(boxCenter, new Vector3(attackBoxSize.x, attackBoxSize.y, 1f));
    }
}