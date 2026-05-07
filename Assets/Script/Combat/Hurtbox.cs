using UnityEngine;

public class Hurtbox : MonoBehaviour
{
    private IDamageable damageable;

    void Awake()
    {
        damageable = GetComponentInParent<IDamageable>();

        if (damageable == null)
        {
            Debug.LogError("IDamageable tidak ditemukan!");
        }
    }

    public void TakeHit(AttackData attackData)
    {
        if (damageable != null)
        {
            damageable.TakeDamage(
                attackData.damage,
                attackData.knockbackX,
                attackData.knockbackY
            );
        }
    }
}