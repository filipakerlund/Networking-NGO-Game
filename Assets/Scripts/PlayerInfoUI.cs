using UnityEngine;
using TMPro;

public class PlayerInfoUI : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI healthText;

    private void OnEnable()
    {
        HealthSystem.OnHealthChange += ChangeHealthAmount;
    }

    private void OnDisable()
    {
        HealthSystem.OnHealthChange -= ChangeHealthAmount;
    }

    private void ChangeHealthAmount(int newHealth)
    {
        healthText.text = newHealth.ToString();
    }
}
