using UnityEngine;

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

    private Rigidbody2D rb;
    private Transform player;

    void Start()
    {
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
            rb.AddForce(force, ForceMode2D.Impulse);
        }

        if (currentHp <= 0)
        {
            Die();
        }
    }

    void Die()
    {
        Debug.Log(gameObject.name + " Mati");
        Destroy(gameObject);
    }
}