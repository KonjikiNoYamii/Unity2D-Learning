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

        Inventory inventory = collision.GetComponent<Inventory>();

        if (inventory != null)
        {
            inventory.AddItem(itemData, amount);
        }

        Debug.Log("Pickup Item: " + itemData.itemName);

        Destroy(gameObject);
    }
}