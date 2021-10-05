using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GameHUD : MonoBehaviour
{
    public float ShieldMax = 100f;
    public float HealthMax = 100f;

    [Header("Components")]
    public Slider HealthSlider;
    public Slider ShieldSlider;
    public TMP_Text Ammo;

    [Header("Cursor")]
    public Texture2D cursorTexture;
    private Vector2 hotSpot = new Vector2(128, 128);

    void Start()
    {
        Cursor.SetCursor(cursorTexture, hotSpot, CursorMode.Auto);
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
