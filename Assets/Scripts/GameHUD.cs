using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GameHUD : MonoBehaviour
{
    public float ShieldMax = 100f;
    public float HealthMax = 100f;
    public Slider HealthSlider;
    public Slider ShieldSlider;
    public TMP_Text Ammo;

    void Start()
    {
        HealthSlider.maxValue = HealthMax;
        ShieldSlider.maxValue = ShieldMax;
    }

    void Update()
    {
        
    }

    public void DisplayShield(bool state)
    {
        ShieldSlider.enabled = state;
    }

    public void UpdateAmmo(int ammo)
    {
        Ammo.text = ammo.ToString();
    }

    public void UpdateMortarCharges(int charges)
    {
        // Mortar charges
    }

    public void UpdateHealth(float health)
    {
        HealthSlider.value = health;
    }

    public void UpdateShield(float shield)
    {
        ShieldSlider.value = shield;
    }
}
