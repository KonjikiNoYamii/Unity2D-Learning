using System.Collections;
using UnityEngine;

public class PlayerHealth : MonoBehaviour
{
    public int maxHealth = 5;
    private int currentHealth;

    public float invicibleTime = 1f;
    private bool isInvicible;

    private Animator animator;

    private Player player;

    void Start()
    {
        currentHealth = maxHealth;

        player = GetComponent<Player>();

        animator = GetComponent<Animator>();

    }

    public void TakeDamage(int damage)
    {
        if (isInvicible) return;
        if (player.isHurt) return;

        currentHealth -= damage;

        Debug.Log("Current Health: " + currentHealth);

        if (currentHealth <= 0)
        {
            Die();
            return;
        }

        StartCoroutine(HurtRoutine());
    }

    IEnumerator HurtRoutine()
    {
        player.isHurt = true;
        isInvicible = true;

        player.isAttacking = false;

        animator.SetTrigger("Hurt");

        Debug.Log("Invicible is ON");

        yield return new WaitForSeconds(0.3f);

        player.isHurt = false;

        yield return new WaitForSeconds(invicibleTime);

        isInvicible = false;

        Debug.Log("Invicible is OFF");
    }

    void Die()
    {
        player.isDead = true;

        animator.SetTrigger("Dead");

        Debug.Log("Player Is Dead");

        StartCoroutine(RespawnCoroutine());
    }

    public int GetMaxHealth()
    {
        return maxHealth;
    }

    public int GetCurrentHealth()
    {
        return currentHealth;
    }

    IEnumerator RespawnCoroutine()
    {
        yield return new WaitForSeconds(2f);

        currentHealth = maxHealth;

        player.isDead = false;

        transform.position = Vector2.zero;

        isInvicible = false;

        animator.Play("Idle");
    }
}