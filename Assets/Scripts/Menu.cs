using UnityEngine;
using UnityEngine.SceneManagement;

public class Menu : MonoBehaviour
{
    public GameObject mainMenu;
    public GameObject settingsMenu;
    private bool isPaused = false;

    void Start()
    {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        mainMenu.SetActive(true);
        settingsMenu.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape) && isPaused)
        {
            Resume();
        }

    }

    public void Play()
    {
        SceneManager.LoadScene("LvlOneCutscene");
    }
    public void Settings()
    {
        mainMenu.SetActive(false);
        settingsMenu.SetActive(true);
        isPaused = true;

    }
    public void Resume()
    {
        mainMenu.SetActive(true);
        settingsMenu.SetActive(false);
        isPaused = false;
    }
    public void QuitGame()
    {

        Debug.Log("Quitting game...");
        Application.Quit();
    }

    public void RestartGame()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}
