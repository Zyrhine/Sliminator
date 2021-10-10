using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
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
    private GameObject failScreen;

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
        // Set default values
        Cursor.SetCursor(cursorTexture, hotSpot, CursorMode.Auto);
        HealthSlider.maxValue = HealthMax;
        ShieldSlider.maxValue = ShieldMax;

        // Get component references
        waveHUD = transform.Find("WaveHUD").gameObject;
        bossHUD = transform.Find("BossHUD").gameObject;
        failScreen = transform.Find("FailScreen").gameObject;
        bossHealthSlider = transform.Find("BossHUD/BossHealthSlider").GetComponent<Slider>();
        waveText = transform.Find("WaveHUD/WaveText").GetComponent<TMP_Text>();
        enemiesText = transform.Find("WaveHUD/EnemiesText").GetComponent<TMP_Text>();
        mortarIcon = transform.Find("MortarMineIcon").gameObject;

        // Disable optional HUDs by default
        DisplayFailScreen(false);
        DisplayWaves(false);
        DisplayBoss(false);
        DisplayMortarCharges(false);
    }

    /// <summary>
    /// Control whether the fail screen is displayed.
    /// </summary>
    /// <param name="state"></param>
    public void DisplayFailScreen(bool state)
    {
        failScreen.SetActive(state);
    }

    /// <summary>
    /// Control whether the WaveHUD is displayed.
    /// </summary>
    /// <param name="state"></param>
    public void DisplayWaves(bool state)
    {
        waveHUD.SetActive(state);
    }

    /// <summary>
    /// Control whether the BossHUD is displayed.
    /// </summary>
    /// <param name="state"></param>
    public void DisplayBoss(bool state)
    {
        bossHUD.SetActive(state);
        if (state == true)
        {
            var anim = bossHUD.GetComponent<Animator>();
            anim.SetTrigger("Activate");
        }
    }

    /// <summary>
    /// Update the values of the BossHUD with the current stats.
    /// </summary>
    /// <param name="health"></param>
    /// <param name="healthMax"></param>
    public void UpdateBoss(float health, float healthMax)
    {
        bossHealthSlider.maxValue = healthMax;
        bossHealthSlider.value = health;
    }

    /// <summary>
    /// Control whether the shield bar is displayed.
    /// </summary>
    /// <param name="state"></param>
    public void DisplayShield(bool state)
    {
        ShieldIcon.SetActive(state);
        ShieldSlider.gameObject.SetActive(state);
    }

    /// <summary>
    /// Update the value of the primary ammunition counter.
    /// </summary>
    /// <param name="ammo"></param>
    public void UpdateAmmo(int ammo)
    {
        Ammo.text = ammo.ToString();
    }

    /// <summary>
    /// Control whether the mortar charge counter is displayed.
    /// </summary>
    /// <param name="state"></param>
    public void DisplayMortarCharges(bool state)
    {
        mortarIcon.SetActive(state);
        MortarAmmo.gameObject.SetActive(state);
    }

    /// <summary>
    /// Update the value of the mortar charges counter.
    /// </summary>
    /// <param name="charges"></param>
    public void UpdateMortarCharges(int charges)
    {
        MortarAmmo.text = charges.ToString();
    }

    /// <summary>
    /// Update the value of the health bar.
    /// </summary>
    /// <param name="health"></param>
    public void UpdateHealth(float health)
    {
        HealthSlider.value = health;
    }

    /// <summary>
    /// Update the value of the shield bar.
    /// </summary>
    /// <param name="shield"></param>
    public void UpdateShield(float shield)
    {
        ShieldSlider.value = shield;
    }

    /// <summary>
    /// Update the WaveHUD with the current wave and enemies remaining.
    /// </summary>
    /// <param name="wave"></param>
    /// <param name="enemiesRemaining"></param>
    public void UpdateWave(int wave, int enemiesRemaining)
    {
        waveText.text = "Wave " + wave.ToString();
        enemiesText.text = "Enemies Remaining: " + enemiesRemaining.ToString();
    }

    /// <summary>
    /// Restart the current level.
    /// </summary>
    public void RetryLevel()
    {
        string sceneName = SceneManager.GetActiveScene().name;
        SceneManager.LoadScene(sceneName);
    }

    /// <summary>
    /// Return to the main menu.
    /// </summary>
    public void QuitToMenu()
    {
        SceneManager.LoadScene("MainMenu");
    }
}
