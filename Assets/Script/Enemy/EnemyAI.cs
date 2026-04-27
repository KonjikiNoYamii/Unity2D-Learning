using UnityEngine;

public class EnemyAI : MonoBehaviour
{
    public float attackRange = 1.5f;
    public float attackCooldown = 1.5f;

    private float nextTimeAttack;

    private Transform player;

    private EnemyHealth enemyHealth;

    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player")?.transform;

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
            nextTimeAttack = Time.time + attackCooldown;
        }
    }

    void AttackPlayer()
    {
        Debug.Log("Enemy Menyerang!");

        Player p = GetComponent<Player>();

        if (p != null)
        {
            p.TakeHit();
        }
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;

        Gizmos.DrawWireSphere(transform.position, attackRange);
    }
}