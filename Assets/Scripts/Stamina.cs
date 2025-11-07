using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class Stamina : MonoBehaviour
{
    public Slider staminaBar;
    public TMP_Text staminaText;

    public float maxStamina = 100f;
    public float currentStamina;

    public float drainRate = 25f;
    public float regenRate = 20f;

    public bool canSprint = true;

    private void Start()
    {
        currentStamina = maxStamina;

        if (staminaBar != null)
        {
            staminaBar.maxValue = maxStamina;
            staminaBar.value = maxStamina;
        }

        UpdateStaminaUI();
    }

    private void Update()
    {
        bool isSprinting = Input.GetKey(KeyCode.LeftShift);

        // --- Draining stamina while sprinting ---
        if (isSprinting && canSprint)
        {
            currentStamina -= drainRate * Time.deltaTime;
            if (currentStamina <= 0)
            {
                currentStamina = 0;
                canSprint = false;
            }
        }
        else
        {
            // --- Regeneration ---
            currentStamina += regenRate * Time.deltaTime;

            if (currentStamina >= maxStamina * 0.5f)
                canSprint = true;
        }

        currentStamina = Mathf.Clamp(currentStamina, 0, maxStamina);
        UpdateStaminaUI();
    }

    // Subtract stamina for jump or other actions
    public void UseStamina(float amount)
    {
        currentStamina -= amount;
        currentStamina = Mathf.Clamp(currentStamina, 0, maxStamina);

        if (currentStamina <= 0)
            canSprint = false;

        UpdateStaminaUI();
    }

    public bool HasStamina(float amount)
    {
        return currentStamina >= amount;
    }

    private void UpdateStaminaUI()
    {
        if (staminaBar != null)
            staminaBar.value = currentStamina;

        if (staminaText != null)
            staminaText.text = Mathf.RoundToInt(currentStamina).ToString();
    }
}