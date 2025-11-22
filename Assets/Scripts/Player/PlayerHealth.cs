using UnityEngine;


public class PlayerHealth : MonoBehaviour
{
    [Header("Health Settings")]
    public float maxHealth = 100f;
    public float currentHealth;

    [Header("UI Sliders")]
    public UnityEngine.UI.Slider redSlider;    // anlık sağlık
    public UnityEngine.UI.Slider yellowSlider; // delayed / damage bar
    public float delaySpeed = 1f; // sarı barın düşüş hızı
    public ShakeAnimations shakeAnimation;

    private void Start()
    {
        currentHealth = maxHealth;
        UpdateHealthUIInstant();
        yellowSlider.value = redSlider.value;
    }
    

    private void Update()
    {
        
        if (yellowSlider.value > redSlider.value)
        {
            yellowSlider.value -= delaySpeed * Time.deltaTime;
            if (yellowSlider.value < redSlider.value)
                yellowSlider.value = redSlider.value;
        }

        if (Input.GetKeyDown(KeyCode.H))
        {
            TakeDamage(20);
        }
    }

    public void TakeDamage(float amount)
    {
        currentHealth -= amount;
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);

        UpdateHealthUIInstant();
        if (shakeAnimation != null)
            shakeAnimation.Shake();

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    void UpdateHealthUIInstant()
    {
        if (redSlider != null)
            redSlider.value = currentHealth / maxHealth;
    }
    public bool Heal(float amount)
    {
        if (currentHealth >= maxHealth)
            return false; // Zaten full can

        currentHealth += amount;
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);
        UpdateHealthUIInstant();
        return true;
    }

    void Die()
    {
        Debug.Log("Player Dead!");
        
    }
}