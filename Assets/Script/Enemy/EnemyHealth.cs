using UnityEngine;
using System.Collections;

public class EnemyHealth : MonoBehaviour, IDamageable
{
    [Header("Health Settings")]
    public int maxHealth = 3;
    private int currentHp;

    [Header("Effect Settings")]
    public GameObject hitEffect;

    [Header("Knockback Settings")]
    public float stunDuration = 0.6f;

    private bool isHurt = false;

    private bool isKnockback;
    private Rigidbody2D rb;
    private Transform player;
    private Animator animator;


    void Start()
    {
        animator = GetComponent<Animator>();

        rb = GetComponent<Rigidbody2D>();
        currentHp = maxHealth;

        player = GameObject.FindGameObjectWithTag("Player")?.transform;

        if (player == null)
        {
            Debug.LogError("Player tidak ditemukan! Pastikan tag Player ada.");
        }

    }

    public void TakeDamage(int damage, float knockbackX, float knockbackY)
    {
        if (isHurt) return;

        currentHp -= damage;

        Debug.Log(gameObject.name + " terkena damage. Sisa Hp: " + currentHp);

        if (hitEffect != null)
        {
            GameObject effect = Instantiate(hitEffect, transform.position, Quaternion.identity);
            Destroy(effect, 0.3f);
        }

        if (rb != null && player != null)
        {
            float dir = Mathf.Sign(transform.position.x - player.position.x);

            Vector2 force = new Vector2(dir * knockbackX, knockbackY);

            isKnockback = true;

            rb.linearVelocity = Vector2.zero;
            rb.AddForce(force, ForceMode2D.Impulse);
            rb.gravityScale = 1f;

            Debug.Log("Velocity setelah knockback: " + rb.linearVelocity);

            EnemyAI ai = GetComponent<EnemyAI>();

            if (ai != null)
            {
                ai.CancelAttack();
            }

        }

        if (currentHp <= 0)
        {
            Die();
            return;
        }

        StartCoroutine(Hurt());

    }

    public bool IsHurt()
    {
        return isHurt;
    }

    public bool IsKnockback()
    {
        return isKnockback;
    }

    IEnumerator Hurt()
    {
        isHurt = true;

        EnemyAI ai = GetComponent<EnemyAI>();
        if (ai != null)
        {
            ai.SetHurtState();
        }

        animator.SetTrigger("Hurt");

        yield return new WaitForSeconds(stunDuration);

        if (ai != null)
        {
            ai.RecoverFromHurt();
        }

        isKnockback = false;
        isHurt = false;
    }
    void Die()
    {
        Debug.Log(gameObject.name + " Mati");
        Destroy(gameObject);
    }
}