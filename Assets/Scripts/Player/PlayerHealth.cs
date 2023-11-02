using UnityEngine;
using TMPro;

public class PlayerHealth : MonoBehaviour
{
    [SerializeField] private TMP_Text healthDisplay;
    [SerializeField] private Damageable playerHP;
    private Damageable trinHP;

    private void Awake()
    {
        playerHP = gameObject.GetComponent<Damageable>();
    }

    private void Start()
    {
        UpdateHealthDisplay();
    }

    private void Update()
    {
        UpdateHealthDisplay();
    }

    private void UpdateHealthDisplay()
    {
        healthDisplay.SetText("HP: " + playerHP.Health + "/" + playerHP.MaxHealth);
    }

}
