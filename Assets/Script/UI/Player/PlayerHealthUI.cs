using UnityEngine;
using UnityEngine.UI;

public class PlayerHealthUI : MonoBehaviour
{
    public PlayerHealth playerHealth;

    public Image healthFill;
    void Update()
    {
        float current = playerHealth.GetCurrentHealth();

        float max = playerHealth.GetMaxHealth();

        healthFill.fillAmount = current / max;
    }
}