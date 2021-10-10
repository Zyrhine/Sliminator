using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelEnd : MonoBehaviour
{
    public GameObject Canvas;
    public string NextSceneName;
    private TMP_Text infoText;
    private TMP_Text gradeText;
    private LevelManager levelManager;

    // Start is called before the first frame update
    void Start()
    {
        Canvas.SetActive(false);
        levelManager = GameObject.FindGameObjectWithTag("LevelManager").GetComponent<LevelManager>();
        infoText = transform.Find("Canvas/Body/InfoText").GetComponent<TMP_Text>();
        gradeText = transform.Find("Canvas/Body/Grade/GradeText").GetComponent<TMP_Text>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            ShowEndScreen();
        }
    }

    /// <summary>
    /// Display the floor complete screen
    /// </summary>
    void ShowEndScreen()
    {
        // Get Scores
        gradeText.text = levelManager.GetGrade();

        // Format the time
        string minutes = Mathf.Floor(levelManager.TimeTaken / 60).ToString("00");
        string seconds = (levelManager.TimeTaken % 60).ToString("00");

        infoText.text = "Time Taken: " + string.Format("{0}:{1}", minutes, seconds)
            + "\nAmmo Used: " + levelManager.AmmoUsed 
            + "\nSlimes Eliminated: " + levelManager.Kills 
            + "\nUnlocks Collected: " + levelManager.UnlocksCollected + "/" + levelManager.UnlocksAvailable;

        // Show the canvas
        Canvas.SetActive(true);
    }

    /// <summary>
    /// Continue to the next level
    /// </summary>
    public void Continue()
    {
        // Save level progress
        string sceneName = SceneManager.GetActiveScene().name;
        PlayerPrefs.SetInt(sceneName, 1);

        // Transition
        AppManager.LoadScene(NextSceneName);
    }
}
