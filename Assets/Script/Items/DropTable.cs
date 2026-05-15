using UnityEngine;

public class DropTable : MonoBehaviour
{
    public DropData[] drops;
    public  GameObject worldItemPrefab;

    public void DropItems()
    {
        foreach (DropData drop in drops)
        {
            float roll = Random.Range(0f, 100f);

            if (roll > drop.dropChance)
            continue;

            int amount = Random.Range(
                drop.minAmount,
                drop.maxAmount + 1
            );

            SpawnItem(drop.item, amount);
        }
    }

    void SpawnItem(ItemData itemData, int amount)
    {
        GameObject obj = Instantiate(
            worldItemPrefab,
            transform.position,
            Quaternion.identity
        );

        WorldItem worldItem = obj.GetComponent<WorldItem>();

        if (worldItem != null)
        {
            worldItem.Setup(itemData, amount);
        }
    }
}