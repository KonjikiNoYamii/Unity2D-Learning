using UnityEngine;
using System.Collections.Generic;

public class Hitbox : MonoBehaviour
{
    private AttackData attackData;

    private CircleCollider2D circleCollider2D;

    public GameObject hitEffect;
    private HashSet<Hurtbox> hitTargets = new HashSet<Hurtbox>();

    void Awake()
    {
        circleCollider2D = GetComponent<CircleCollider2D>();
    }

    public void SetAttackData(AttackData data)
    {
        attackData = data;

        if (circleCollider2D != null)
        {
            circleCollider2D.radius = attackData.range;
        }
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        Hurtbox hurtbox = collision.GetComponent<Hurtbox>();

        if (hurtbox == null) return;

        if (hitTargets.Contains(hurtbox)) return;

        hitTargets.Add(hurtbox);

        if (hitEffect != null)
        {
            Instantiate(
               hitEffect,
               collision.transform.position,
               Quaternion.identity
           );
        }

        hurtbox.TakeHit(attackData);

        if (HitStopManager.Instance != null)
        {
            HitStopManager.Instance.Stop(
                attackData.hitStop
            );

        }
    }

    void OnEnable()
    {
        hitTargets.Clear();
    }
}