using UnityEngine;
using UnityEngine.UI;

public class MenuController : MonoBehaviour
{
    [Header("Menu Panels")]
    public GameObject MainPanel;
    public GameObject SettingsPanel;
    public GameObject CreditsPanel;
    public GameObject LevelSelectPanel;

    void Start()
    {
        ShowMainPanel();
    }

    // Show the main panel
    public void ShowMainPanel()
    {
        MainPanel.SetActive(true);
        SettingsPanel.SetActive(false);
        CreditsPanel.SetActive(false);
        LevelSelectPanel.SetActive(false);
    }

    // Show the settings panel
    public void ShowSettingsPanel()
    {
        MainPanel.SetActive(false);
        SettingsPanel.SetActive(true);
    }

    // Show the credits panel
    public void ShowCreditsPanel()
    {
        MainPanel.SetActive(false);
        CreditsPanel.SetActive(true);
    }

    // Show the level select panel
    public void ShowLevelSelectPanel()
    {
        MainPanel.SetActive(false);
        LevelSelectPanel.SetActive(true);
    }
}
