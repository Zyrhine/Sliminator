using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GameHUD : MonoBehaviour
{
    public float ShieldMax = 100f;
    public float HealthMax = 100f;

    private GameObject bossHUD;
    private Slider bossHealthSlider;
    private GameObject waveHUD;
    private TMP_Text waveText;
    private TMP_Text enemiesText;
    private GameObject mortarIcon;

    [Header("Components")]
    public Slider HealthSlider;
    public Slider ShieldSlider;
    public GameObject ShieldIcon;
    public TMP_Text Ammo;
    public TMP_Text MortarAmmo;

    [Header("Cursor")]
    public Texture2D cursorTexture;
    private Vector2 hotSpot = new Vector2(128, 128);

    void Start()
    {
        Cursor.SetCursor(cursorTexture, hotSpot, CursorMode.Auto);
        HealthSlider.maxValue = HealthMax;
        ShieldSlider.maxValue = ShieldMax;

        waveHUD = transform.Find("WaveHUD").gameObject;
        bossHUD = transform.Find("BossHUD").gameObject;
        bossHealthSlider = transform.Find("BossHUD/BossHealthSlider").GetComponent<Slider>();
        waveText = transform.Find("WaveHUD/WaveText").GetComponent<TMP_Text>();
        enemiesText = transform.Find("WaveHUD/EnemiesText").GetComponent<TMP_Text>();
        mortarIcon = transform.Find("MortarMineIcon").gameObject;
        DisplayWaves(false);
        DisplayBoss(false);
        DisplayMortarCharges(false);
    }

    void Update()
    {
        
    }

    public void DisplayWaves(bool state)
    {
        waveHUD.SetActive(state);
    }

    public void DisplayBoss(bool state)
    {
        bossHUD.SetActive(state);
        if (state == true)
        {
            var anim = bossHUD.GetComponent<Animator>();
            anim.SetTrigger("Activate");
        }
    }

    public void UpdateBoss(float health, float healthMax)
    {
        bossHealthSlider.maxValue = healthMax;
        bossHealthSlider.value = health;
    }

    public void DisplayShield(bool state)
    {
        ShieldIcon.SetActive(state);
        ShieldSlider.gameObject.SetActive(state);
    }

    public void UpdateAmmo(int ammo)
    {
        Ammo.text = ammo.ToString();
    }

    public void DisplayMortarCharges(bool state)
    {
        mortarIcon.SetActive(state);
        MortarAmmo.gameObject.SetActive(state);
    }

    public void UpdateMortarCharges(int charges)
    {
        MortarAmmo.text = charges.ToString();
    }

    public void UpdateHealth(float health)
    {
        HealthSlider.value = health;
    }

    public void UpdateShield(float shield)
    {
        ShieldSlider.value = shield;
    }

    public void UpdateWave(int wave, int enemiesRemaining)
    {
        waveText.text = "Wave " + wave.ToString();
        enemiesText.text = "Enemies Remaining: " + enemiesRemaining.ToString();
    }
}
