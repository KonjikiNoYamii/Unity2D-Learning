using UnityEngine;

[CreateAssetMenu(fileName = "NewAttackData", menuName = "Combat/Attack Data")]
public class AttackData : ScriptableObject
{
    public string attackName;
    public int damage;
    public float range;
    public Vector2 offset;
    public float hitStop;
    public float knockbackX;
    public float knockbackY;
}