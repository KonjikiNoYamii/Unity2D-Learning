using UnityEngine;

public class WorldItem : MonoBehaviour
{
    public ItemData itemData;
    public int amount = 1;
    private SpriteRenderer spriteRenderer;

    void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    public void Setup(ItemData data, int value)
    {
        itemData = data;
        amount = value;
        if (itemData != null)
        {
            spriteRenderer.sprite = itemData.icon;
        }
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (!collision.CompareTag("Player")) return;

        Debug.Log("Pickup Item: " + itemData.itemName);

        Destroy(gameObject);
    }
}