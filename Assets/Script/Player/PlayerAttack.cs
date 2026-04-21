using UnityEngine;

public class PlayerAttack : MonoBehaviour
{
    public GameObject hitEffect;

    [Header("Attack Settings")]
    public Transform attackPoint;
    public int attackDamage = 1;
    public float attackRange = 0.5f;
    public LayerMask enemyLayer;

    [Header("Cooldown Settings")]
    public float attackCooldown = 0.5f;
    private float nextAttackTime = 0f;

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

        animator.ResetTrigger("Attack");

        if (animator == null)
        {
            Debug.Log("Animator tidak ditemukan di player");
        }
    }

    void Update()
    {

        if (Input.GetKeyDown(KeyCode.F) && Time.time >= nextAttackTime && !player.isAttacking)
        {
            player.isAttacking = true;

            animator.SetTrigger("Attack");

            nextAttackTime = Time.time + attackCooldown;
        }
    }

    public void Attack()
    {
        Debug.Log("Attack");

        Collider2D[] hitEnemies = Physics2D.OverlapCircleAll(
            attackPoint.position,
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
        if (attackPoint == null)
            return;

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(attackPoint.position, attackRange);
    }
}