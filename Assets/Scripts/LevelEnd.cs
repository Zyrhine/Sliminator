using UnityEngine;

public class LevelEnd : MonoBehaviour
{
    public GameObject Canvas;
    public string NextSceneName;

    // Start is called before the first frame update
    void Start()
    {
        Canvas.SetActive(false);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            ShowEndScreen();
        }
    }

    void ShowEndScreen()
    {
        // Calculate and set scores


        // Show the canvas
        Canvas.SetActive(true);
    }

    public void Continue()
    {
        // Save level progress


        // Transition
        AppManager.LoadScene(NextSceneName);
    }
}
