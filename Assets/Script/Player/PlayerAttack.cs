using UnityEngine;
using System.Collections.Generic;

public class PlayerAttack : MonoBehaviour
{
    public AttackData[] attacks;

    public Hitbox hitbox;
    private AttackData currentAttack;
    public GameObject hitEffect;

    [Header("Combo Settings")]
    public int maxCombo = 3;
    private int comboIndex = 0;

    private float comboTimer;
    public float comboTimerReset = 0.8f;

    [Header("Cooldown Settings")]
    public float attackCooldown = 0.5f;
    private float nextAttackTime = 0f;

    [Header("Attack Direction")]
    public float InputBufferTime = 0.2f;
    private float InputBufferCounter;
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

        if (attacks == null || attacks.Length == 0)
        {
            Debug.Log("Attacks belum diisi");
        }

        hitbox.gameObject.SetActive(false);

    }

    void Update()
    {
        HandleInput();

        comboTimer += Time.deltaTime;

        if (comboTimer > comboTimerReset)
        {
            comboTimer = 0;
            comboIndex = 0;
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
        if (Time.time < nextAttackTime) return false;
        if (player.isAttacking) return false;
        if (player.isHurt) return false;

        return true;
    }

    void StartAttack()
    {

        comboIndex++;

        if (comboIndex > attacks.Length)
        {
            comboIndex = 1;
        }

        int index = Mathf.Clamp(comboIndex - 1, 0, attacks.Length - 1);
        currentAttack = attacks[index];

        hitbox.SetAttackData(currentAttack);

        player.isAttacking = true;

        if (animator != null)
        {
            animator.SetInteger("Combo", comboIndex);
            animator.SetTrigger("Attack");

            Debug.Log("Combo" + comboIndex);
        }

        nextAttackTime = Time.time + attackCooldown;

        InputBufferCounter = 0f;
        comboTimer = 0;
    }

    public void EndAttack()
    {
        player.isAttacking = false;
    }

    public void EnableHit()
    {
        float direction = player.isFacingRight ? 1f : -1f;

        Vector2 offset = currentAttack.offset;

        hitbox.transform.localPosition = new Vector3(offset.x, currentAttack.offset.y, 0);
        hitbox.gameObject.SetActive(true);
    }

    public void DisableHit()
    {
        hitbox.gameObject.SetActive(false);
    }

    void OnDrawGizmosSelected()
    {
        Player p = GetComponent<Player>();
        if (p == null) return;

        if (attacks == null || attacks.Length == 0) return;
        if (attacks[0] == null) return;

        float direction = p.isFacingRight ? 1f : -1f;

        AttackData previewAttack = currentAttack != null ? currentAttack : attacks[0];

        Vector2 offset = previewAttack.offset;
        float range = previewAttack.range;

        Vector2 attackPos = (Vector2)transform.position +
            new Vector2(offset.x * direction, offset.y);

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(attackPos, range);
    }
}