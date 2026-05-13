using System;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAI : MonoBehaviour
{
    public enum EnemyState
    {
        Idle,
        Patrol,
        Chase,
        Attack,
        Hurt,
        Dead
    }

    private EnemyState currentState;

    public float attackRange = 1.5f;
    public float attackCooldown = 1.5f;

    public Vector2 attackBoxSize = new Vector2(1.5f, 1f);
    public Vector2 attackOffset = new Vector2(1f, 0.5f);

    public Transform pointA;
    public Transform pointB;
    private Transform currentPoint;

    public float moveSpeed = 1.5f;
    public float chaseRange = 5f;

    public LayerMask playerLayer;

    public Transform edgeCheck;

    public float edgeCheckDistance = 1f;

    public LayerMask groundLayer;

    private bool isFacingRight = true;
    private bool isAttacking;
    private bool canDealDamage;

    private float nextTimeAttack;

    private Transform player;
    private EnemyHealth enemyHealth;
    private Animator animator;
    private Rigidbody2D rb;

    private HashSet<Collider2D> hitTarget = new HashSet<Collider2D>();

    void Start()
    {
        currentPoint = pointB;
        player = GameObject.FindGameObjectWithTag("Player")?.transform;
        enemyHealth = GetComponent<EnemyHealth>();
        animator = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();

        currentState = EnemyState.Patrol;

        if (player == null)
        {
            Debug.Log("Player tidak ditemukan!");
        }
    }

    void Update()
    {
        if (player == null) return;

        if (enemyHealth != null && enemyHealth.IsKnockback())
        {
            return;
        }

        HandleFlip();
        UpdateAnimator();

        switch (currentState)
        {
            case EnemyState.Idle:
                HandleIdle();
                break;

            case EnemyState.Patrol:
                HandlePatrol();
                break;

            case EnemyState.Chase:
                HandleChase();
                break;

            case EnemyState.Attack:
                HandleAttack();
                break;

            case EnemyState.Hurt:
                HandleHurt();
                break;

            case EnemyState.Dead:
                break;
        }
    }

    bool isGroundAhead()
    {
        RaycastHit2D hit = Physics2D.Raycast(
            edgeCheck.position,
            Vector2.down,
            edgeCheckDistance,
            groundLayer
        );

        Debug.DrawRay(
            edgeCheck.position,
            Vector2.down * edgeCheckDistance,
            Color.red
        );

        return hit.collider != null;
    }

    void HandlePatrol()
    {
        float distanceToPlayer = Vector2.Distance(transform.position, player.position);
        if (distanceToPlayer <= chaseRange)
        {
            ChangeState(EnemyState.Chase);
            return;
        }

        if (!isGroundAhead())
        {
            if (currentPoint == pointA)
            {
                currentPoint = pointB;
            }
            else
            {
                currentPoint = pointA;
            }

            return;
        }

        Vector2 direction = (currentPoint.position - transform.position).normalized;
        rb.linearVelocity = new Vector2(
            direction.x * moveSpeed,
            rb.linearVelocity.y
        );

        float distanceToPoint = Vector2.Distance(transform.position, currentPoint.position);
        if (distanceToPoint <= 0.2f)
        {
            if (currentPoint == pointA)
            {
                currentPoint = pointB;
            }
            else
            {
                currentPoint = pointA;
            }
        }
    }

    void HandleIdle()
    {
        StopMoving();

        float distance = Vector2.Distance(transform.position, player.position);

        if (distance <= chaseRange)
        {
            ChangeState(EnemyState.Chase);
        }
    }

    void HandleChase()
    {
        float distance = Vector2.Distance(transform.position, player.position);

        if (distance <= attackRange)
        {
            ChangeState(EnemyState.Attack);
            return;
        }

        if (distance > chaseRange)
        {
            ChangeState(EnemyState.Patrol);
            return;
        }

        ChasePlayer();
    }

    void HandleAttack()
    {
        StopMoving();

        float distance = Vector2.Distance(transform.position, player.position);

        if (distance > attackRange)
        {
            ChangeState(EnemyState.Chase);
            return;
        }

        if (!isAttacking && Time.time >= nextTimeAttack)
        {
            AttackPlayer();
        }
    }

    void HandleHurt()
    {
        StopMoving();
    }

    void AttackPlayer()
    {
        if (isAttacking) return;

        hitTarget.Clear();

        isAttacking = true;

        if (animator != null)
        {
            animator.SetTrigger("Attack");
        }
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

            PlayerHealth playerHealth = hit.GetComponent<PlayerHealth>();


            if (playerHealth != null)
            {
                playerHealth.TakeDamage(1);
            }
        }
    }

    void ChasePlayer()
    {
        float direction = player.position.x > transform.position.x ? 1f : -1f;

        float targetSpeed = direction * moveSpeed;

        float newVelocityX = Mathf.Lerp(rb.linearVelocity.x, targetSpeed, 0.2f);

        rb.linearVelocity = new Vector2(newVelocityX, rb.linearVelocity.y);
    }

    void StopMoving()
    {
        float newX = Mathf.Lerp(rb.linearVelocity.x, 0f, 0.2f);
        rb.linearVelocity = new Vector2(newX, rb.linearVelocity.y);
    }

    void HandleFlip()
    {
        Transform target = player;

        if (currentState == EnemyState.Patrol)
        {
            target = currentPoint;
        }

        bool shouldFaceRight = target.position.x > transform.position.x;

        if (shouldFaceRight != isFacingRight)
        {
            isFacingRight = shouldFaceRight;
            Flip();
        }
    }

    void Flip()
    {
        Vector3 scale = transform.localScale;
        scale.x = Math.Abs(scale.x) * (isFacingRight ? 1 : -1);
        transform.localScale = scale;
    }

    void UpdateAnimator()
    {
        if (animator != null)
        {
            animator.SetFloat("Walk", Mathf.Abs(rb.linearVelocity.x));
        }
    }

    void ChangeState(EnemyState newState)
    {
        if (currentState == newState) return;

        currentState = newState;
    }

    public void SetHurtState()
    {
        CancelAttack();
        ChangeState(EnemyState.Hurt);
    }

    public void RecoverFromHurt()
    {
        ChangeState(EnemyState.Chase);
    }

    public void EndAttack()
    {
        isAttacking = false;
        canDealDamage = false;
        nextTimeAttack = Time.time + attackCooldown;
    }

    public void EnableHit()
    {
        canDealDamage = true;
    }

    public void DisableHit()
    {
        canDealDamage = false;
    }

    public void CancelAttack()
    {
        isAttacking = false;
        canDealDamage = false;

        if (animator != null)
        {
            animator.ResetTrigger("Attack");
            animator.Play("Walk");
        }

        StopMoving();
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