using UnityEngine;

public class PlayerAttack : MonoBehaviour
{
    public GameObject hitEffect;

    [Header("Attack Settings")]
    public int attackDamage = 1;
    public float attackRange = 0.5f;
    public LayerMask enemyLayer;

    [Header("Cooldown Settings")]
    public float attackCooldown = 0.5f;
    private float nextAttackTime = 0f;

    [Header("Attack Direction")]
    public Vector2 attackOffset = new Vector2(0.5f, 0.7f);

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

        if (Input.GetMouseButtonDown(0) && Time.time >= nextAttackTime && !player.isAttacking && !player.isHurt)
        {
            player.isAttacking = true;

            animator.SetTrigger("Attack");

            nextAttackTime = Time.time + attackCooldown;
        }
    }

    public void Attack()
    {
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