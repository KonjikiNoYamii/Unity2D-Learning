using UnityEngine;

public class EnemyAI : MonoBehaviour
{
    public Vector2 attackBoxSize = new Vector2(1.5f, 1f);
    public Vector2 attackOffset = new Vector2(1f, 0.5f);
    public LayerMask playerLayer;

    public float attackRange = 1.5f;
    public float attackCooldown = 1.5f;

    private float nextTimeAttack;

    private Transform player;

    private bool isFacingRight = true;

    private bool isAttacking;

    private Animator animator;

    private EnemyHealth enemyHealth;

    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player")?.transform;

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

        if (distance <= attackRange && Time.time >= nextTimeAttack)
        {
            AttackPlayer();
        }

        if (isAttacking) return;
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
        scale.x = Mathf.Abs(scale.x) * (isFacingRight ? 1 : -1);
        transform.localScale = scale;
    }

    public void EndAttack()
    {
        isAttacking = false;
        nextTimeAttack = Time.time + attackCooldown;

    }

    public void DealDamage()
    {
        float direction = isFacingRight ? 1f : -1f;

        Vector2 boxCenter = (Vector2)transform.position +
        new Vector2(attackOffset.x * direction, attackOffset.y);

        Collider2D hitPlayer = Physics2D.OverlapBox(
            boxCenter,
            attackBoxSize,
            0f,
            playerLayer
        );

        if (hitPlayer != null)
        {
            Player p = hitPlayer.GetComponent<Player>();

            if (p != null)
            {
                p.TakeHit();
            }
        }
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;

        float direction = isFacingRight ? 1f : -1f;

        Vector2 boxCenter = (Vector2)transform.position +
        new Vector2(attackOffset.x * direction, attackOffset.y);

        Gizmos.DrawWireCube(boxCenter, attackBoxSize);
    }
}