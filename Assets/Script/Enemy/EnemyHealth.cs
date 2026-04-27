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
    public float knockbackForceX = 5f;
    public float knockbackForceY = 3f;

    private bool isHurt = false;
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

    public void TakeDamage(int damage)
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

            Vector2 force = new Vector2(dir * knockbackForceX, knockbackForceY);

            rb.linearVelocity = Vector2.zero;
            rb.AddForce(force, ForceMode2D.Impulse);
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

    IEnumerator Hurt()
    {
        isHurt = true;

        animator.SetTrigger("Hurt");

        yield return new WaitForSeconds(0.1f);

        isHurt = false;
    }
    void Die()
    {
        Debug.Log(gameObject.name + " Mati");
        Destroy(gameObject);
    }
}