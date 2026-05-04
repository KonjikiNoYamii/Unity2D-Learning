using UnityEngine;
using System.Collections.Generic;

public class PlayerAttack : MonoBehaviour
{
    public GameObject hitEffect;

    [Header("Attack Settings")]
    public int attackDamage = 1;
    public float attackRange = 0.5f;
    public LayerMask enemyLayer;

    [Header("Combo Settings")]

    public int maxCombo = 3;
    private int comboIndex = 0;
    private float comboTimer;
    private float comboResetTime = 0.8f;

    private bool canDealDamage;

    [Header("Cooldown Settings")]
    public float attackCooldown = 0.5f;
    private float nextAttackTime = 0f;

    [Header("Attack Direction")]
    public Vector2 attackOffset = new Vector2(0.5f, 0.7f);

    public float InputBufferTime = 0.2f;
    private float InputBufferCounter;

    private HashSet<Collider2D> hitTargets = new HashSet<Collider2D>();

    private Player player;
    private Animator animator;

    void Start()
    {
        player = GetComponent<Player>();

        if (player == null)
        {
            Debug.Log("player tidak ditemukan");
        }

        animator = GetComponent<Animator>();

        if (animator == null)
        {
            Debug.Log("Animator tidak ditemukan di player");
        }
    }

    void Update()
    {
        HandleInput();

        if (comboIndex > 0)
        {
            comboTimer += Time.deltaTime;

            if (comboTimer > comboResetTime)
            {
                comboIndex = 0;
                comboTimer = 0;
            }
        }

        HandleBuffer();
    }

    void HandleInput()
    {
        if (Input.GetMouseButtonDown(0))
        {
            InputBufferCounter = InputBufferTime;
        }
    }

    void HandleBuffer()
    {
        if (InputBufferCounter > 0)
        {
            TryAttack();
            InputBufferCounter -= Time.deltaTime;
        }
    }

    void TryAttack()
    {
        if (!CanAttack()) return;

        StartAttack();
    }

    bool CanAttack()
    {
        if (player.isAttacking) return false;
        if (player.isHurt) return false;

        return true;
    }

    void StartAttack()
    {
        hitTargets.Clear();

        comboIndex++;

        if (comboIndex > maxCombo)
        {
            comboIndex = 1;
        }

        player.isAttacking = true;

        if (animator != null)
        {
            animator.SetInteger("Combo", comboIndex);
            animator.SetTrigger("Attack");

            Debug.Log("Combo " + comboIndex);
        }

        nextAttackTime = Time.time + attackCooldown;

        InputBufferCounter = 0f;
        comboTimer = 0;
    }

    public void Attack()
    {
        if (!canDealDamage) return;

        Debug.Log("Attack");

        float direction = player.isFacingRight ? 1f : -1f;

        Vector2 attackPos = (Vector2)transform.position +
        new Vector2(attackOffset.x * direction, attackOffset.y);

        Collider2D[] hitEnemies = Physics2D.OverlapCircleAll(
            attackPos,
            attackRange,
            enemyLayer
        );


        foreach (Collider2D obj in hitEnemies)
        {
            if (hitTargets.Contains(obj)) continue;

            hitTargets.Add(obj);

            Debug.Log("Hit: " + obj.name);

            Instantiate(hitEffect, obj.transform.position, Quaternion.identity);

            IDamageable damageable = obj.GetComponent<IDamageable>();


            if (damageable != null)
            {
                damageable.TakeDamage(attackDamage);

                if (HitStopManager.Instance != null)
                {
                    HitStopManager.Instance.Stop(0.2f);
                }
            }

        }
    }

    public void EndAttack()
    {
        player.isAttacking = false;
        canDealDamage = false;
    }

    public void EnableHit()
    {
        canDealDamage = true;
    }

    public void DisableHit()
    {
        canDealDamage = false;
    }

    void OnDrawGizmosSelected()
    {
        Player p = GetComponent<Player>();
        if (p == null) return;

        float direction = p.isFacingRight ? 1f : -1f;

        Vector2 attackPos = (Vector2)transform.position +
        new Vector2(attackOffset.x * direction, attackOffset.y);

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(attackPos, attackRange);
    }
}